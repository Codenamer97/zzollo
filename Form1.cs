using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace zzollo
{
    public partial class Form1 : Form
    {
        private const int MaxFileSizeBytes = 100 * 1024;
        private const int MaxFilesPerRequest = 10;
        private const int MaxPromptCharacters = 80_000;
        private const string GeneralConversationModel = "dev-ko";
        private const string OcrVisionModel = "llava:latest";

        private static readonly string[] IncludedExtensions =
        [
            ".sln", ".csproj", ".cs", ".json", ".config", ".xml", ".md",
            ".png", ".jpg", ".jpeg", ".bmp", ".webp"
        ];

        private static readonly string[] ImageExtensions =
        [
            ".png", ".jpg", ".jpeg", ".bmp", ".webp"
        ];

        private static readonly string[] ExcludedDirectories =
        [
            ".git", ".vs", "packages", "node_modules"
        ];

        private static readonly string[] QuestionTemplates =
        [
            "선택한 파일 코드 리뷰",
            "전체 프로젝트 구조 분석",
            "버그 가능성 분석",
            "성능 개선점 찾기",
            "async/await 문제 찾기",
            "WinForms UI Thread 문제 찾기",
            "예외 처리 누락 찾기",
            "보안 취약점 찾기",
            "리팩토링 제안",
            "테스트 코드 작성 요청"
        ];

        private static readonly string[] QuestionModes =
        [
            "코드 분석",
            "일반 질문",
            "OCR"
        ];

        private readonly OllamaClient ollamaClient = new();
        private readonly string settingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "zzollo",
            "settings.json");
        private readonly List<ProjectFile> scannedFiles = [];

        private AppSettings settings = new();
        private CancellationTokenSource? currentRequestCts;
        private bool nextToggleSelectsAll = true;
        private string? localCodeModelBeforeGeneral;
        private string? remoteCodeModelBeforeGeneral;
        private string? selectedOcrImagePath;

        public Form1()
        {
            InitializeComponent();
            InitializeApp();
        }

        private void InitializeApp()
        {
            LoadSettings();
            BindStaticOptions();
            ApplySettingsToControls();
            WireEvents();
            ApplyQuestionMode();
            UpdateStatus("준비됨");
        }

        private void BindStaticOptions()
        {
            cboQuestionMode.Items.AddRange(QuestionModes);
            cboQuestionMode.SelectedIndex = 0;

            cboQuestionTemplate.Items.AddRange(QuestionTemplates);
            cboQuestionTemplate.SelectedIndex = 0;

            cboAnalysisScope.Items.AddRange(new object[]
            {
                "체크한 파일들",
                "현재 선택한 파일만",
                "주요 파일 자동 선택"
            });
            cboAnalysisScope.SelectedIndex = 0;

            cboExtensionFilter.Items.Add("전체");
            cboExtensionFilter.Items.AddRange(IncludedExtensions.Cast<object>().ToArray());
            cboExtensionFilter.SelectedIndex = 0;

            cboLocalModel.Items.Add(settings.Local.Model);
            cboRemoteModel.Items.Add(settings.Remote.Model);
            cboLocalModel.Text = settings.Local.Model;
            cboRemoteModel.Text = settings.Remote.Model;
            txtQuestion.Text = BuildQuestionFromTemplate(cboQuestionTemplate.Text);
        }

        private void WireEvents()
        {
            btnSelectFolder.Click += (_, _) => SelectFolder();
            btnRefresh.Click += async (_, _) => await ScanCurrentFolderAsync();
            cboRecentFolders.SelectedIndexChanged += async (_, _) =>
            {
                if (cboRecentFolders.SelectedItem is string path && Directory.Exists(path))
                {
                    txtProjectPath.Text = path;
                    await ScanCurrentFolderAsync();
                }
            };
            chkExcludeBinObj.CheckedChanged += async (_, _) => await ScanCurrentFolderAsync();
            chkIncludeDesigner.CheckedChanged += async (_, _) => await ScanCurrentFolderAsync();
            cboExtensionFilter.SelectedIndexChanged += (_, _) => RenderFileList();
            fileList.ItemCheck += (_, _) => BeginInvoke(UpdateFileCount);
            btnToggleAllFiles.Click += (_, _) => ToggleAllVisibleFiles();
            cboQuestionMode.SelectedIndexChanged += (_, _) => ApplyQuestionMode();
            cboQuestionTemplate.SelectedIndexChanged += (_, _) => txtQuestion.Text = BuildQuestionFromTemplate(cboQuestionTemplate.Text);
            btnLocalTest.Click += async (_, _) => await TestConnectionAsync(GetSelectedServer());
            btnRemoteTest.Click += async (_, _) => await TestConnectionAsync(GetSelectedServer());
            btnLocalModels.Click += async (_, _) => await LoadModelsAsync(GetSelectedServer(), cboLocalModel);
            btnRemoteModels.Click += async (_, _) => await LoadModelsAsync(GetSelectedServer(), cboRemoteModel);
            btnSaveRemote.Click += (_, _) => SaveRemoteServer();
            connectionTabs.SelectedIndexChanged += (_, _) => ApplyRecommendedModelForCurrentMode();
            btnSelectOcrImage.Click += (_, _) => SelectOcrImage();
            btnPreviewPrompt.Click += (_, _) => PreviewPrompt();
            btnSend.Click += async (_, _) => await SendQuestionAsync();
            btnCancel.Click += (_, _) => currentRequestCts?.Cancel();
            btnCopyResponse.Click += (_, _) => CopyResponse();
            btnSaveResponse.Click += (_, _) => SaveResponse();
            FormClosing += (_, _) => SaveSettings();
        }

        private void LoadSettings()
        {
            try
            {
                if (!File.Exists(settingsPath))
                {
                    return;
                }

                var json = File.ReadAllText(settingsPath);
                settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
            catch
            {
                settings = new AppSettings();
            }
        }

        private void ApplySettingsToControls()
        {
            txtLocalHost.Text = settings.Local.Host;
            numLocalPort.Value = settings.Local.Port;
            cboLocalModel.Text = settings.Local.Model;

            txtRemoteAlias.Text = settings.Remote.Alias;
            txtRemoteIp.Text = settings.Remote.Ip;
            numRemotePort.Value = settings.Remote.Port;
            cboRemoteModel.Text = settings.Remote.Model;

            chkExcludeBinObj.Checked = settings.Analysis.ExcludeBinObj;
            chkIncludeDesigner.Checked = !settings.Analysis.ExcludeDesignerFiles;
            foreach (var folder in settings.RecentFolders.Where(Directory.Exists).Distinct().Take(10))
            {
                cboRecentFolders.Items.Add(folder);
            }
        }

        private void SaveSettings()
        {
            try
            {
                settings.Local.Host = txtLocalHost.Text.Trim();
                settings.Local.Port = (int)numLocalPort.Value;
                settings.Local.Model = cboLocalModel.Text.Trim();
                settings.Remote.Alias = txtRemoteAlias.Text.Trim();
                settings.Remote.Ip = txtRemoteIp.Text.Trim();
                settings.Remote.Port = (int)numRemotePort.Value;
                settings.Remote.Model = cboRemoteModel.Text.Trim();
                settings.Analysis.ExcludeBinObj = chkExcludeBinObj.Checked;
                settings.Analysis.ExcludeDesignerFiles = !chkIncludeDesigner.Checked;

                Directory.CreateDirectory(Path.GetDirectoryName(settingsPath)!);
                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(settingsPath, json);
            }
            catch
            {
                // Settings persistence must not block normal application shutdown.
            }
        }

        private void SelectFolder()
        {
            folderBrowserDialog.Description = "분석할 C# 프로젝트 폴더를 선택하세요.";
            if (Directory.Exists(txtProjectPath.Text))
            {
                folderBrowserDialog.SelectedPath = txtProjectPath.Text;
            }

            if (folderBrowserDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            txtProjectPath.Text = folderBrowserDialog.SelectedPath;
            RememberRecentFolder(folderBrowserDialog.SelectedPath);
            _ = ScanCurrentFolderAsync();
        }

        private void RememberRecentFolder(string path)
        {
            settings.RecentFolders.RemoveAll(p => string.Equals(p, path, StringComparison.OrdinalIgnoreCase));
            settings.RecentFolders.Insert(0, path);
            settings.RecentFolders = settings.RecentFolders.Take(10).ToList();
            cboRecentFolders.Items.Clear();
            cboRecentFolders.Items.AddRange(settings.RecentFolders.Cast<object>().ToArray());
        }

        private async Task ScanCurrentFolderAsync()
        {
            var root = txtProjectPath.Text.Trim();
            if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root))
            {
                return;
            }

            SetBusy(true, "파일 스캔 중...");
            try
            {
                scannedFiles.Clear();
                await Task.Run(() =>
                {
                    foreach (var file in Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories))
                    {
                        if (ShouldIncludeFile(root, file))
                        {
                            var info = new FileInfo(file);
                            scannedFiles.Add(new ProjectFile(root, file, info.Length));
                        }
                    }
                });

                RenderFileList();
                UpdateStatus($"스캔 완료: {scannedFiles.Count:N0}개 파일");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"파일 스캔 중 오류가 발생했습니다.\n{ex.Message}", "스캔 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                UpdateStatus("스캔 오류");
            }
            finally
            {
                SetBusy(false);
            }
        }

        private bool ShouldIncludeFile(string root, string file)
        {
            var relative = Path.GetRelativePath(root, file);
            var extension = Path.GetExtension(file);
            if (!IncludedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            var parts = relative.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (chkExcludeBinObj.Checked && parts.Any(p => p.Equals("bin", StringComparison.OrdinalIgnoreCase) || p.Equals("obj", StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            if (parts.Any(p => ExcludedDirectories.Contains(p, StringComparer.OrdinalIgnoreCase)))
            {
                return false;
            }

            var fileName = Path.GetFileName(file);
            if (!chkIncludeDesigner.Checked && fileName.EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return !fileName.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase)
                && !fileName.EndsWith(".AssemblyInfo.cs", StringComparison.OrdinalIgnoreCase);
        }

        private void RenderFileList()
        {
            fileList.BeginUpdate();
            try
            {
                fileList.Items.Clear();
                var extension = cboExtensionFilter.SelectedItem?.ToString();
                var filtered = scannedFiles
                    .Where(f => extension == "전체" || f.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(f => f.RelativePath)
                    .ToList();

                foreach (var file in filtered)
                {
                    fileList.Items.Add(file, ShouldPreselect(file));
                }

                nextToggleSelectsAll = true;
                btnToggleAllFiles.Text = "전체 선택";
            }
            finally
            {
                fileList.EndUpdate();
                UpdateFileCount();
            }
        }

        private void ToggleAllVisibleFiles()
        {
            fileList.BeginUpdate();
            try
            {
                for (var i = 0; i < fileList.Items.Count; i++)
                {
                    fileList.SetItemChecked(i, nextToggleSelectsAll);
                }
            }
            finally
            {
                fileList.EndUpdate();
            }

            nextToggleSelectsAll = !nextToggleSelectsAll;
            btnToggleAllFiles.Text = nextToggleSelectsAll ? "전체 선택" : "전체 해제";
            UpdateFileCount();
        }

        private static bool ShouldPreselect(ProjectFile file)
        {
            var name = Path.GetFileName(file.FullPath);
            return file.Size <= MaxFileSizeBytes
                && (name.Equals("Program.cs", StringComparison.OrdinalIgnoreCase)
                    || name.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase)
                    || name.EndsWith(".sln", StringComparison.OrdinalIgnoreCase)
                    || (file.Extension.Equals(".cs", StringComparison.OrdinalIgnoreCase)
                        && !name.EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase)));
        }

        private void UpdateFileCount()
        {
            lblFileCount.Text = $"파일 {fileList.CheckedItems.Count}개 선택";
        }

        private OllamaServer GetSelectedServer()
        {
            if (connectionTabs.SelectedTab == remoteTab)
            {
                return new OllamaServer(
                    "remote",
                    $"http://{txtRemoteIp.Text.Trim()}:{(int)numRemotePort.Value}",
                    cboRemoteModel.Text.Trim());
            }

            return new OllamaServer(
                "local",
                $"http://{txtLocalHost.Text.Trim()}:{(int)numLocalPort.Value}",
                cboLocalModel.Text.Trim());
        }

        private async Task TestConnectionAsync(OllamaServer server)
        {
            if (string.IsNullOrWhiteSpace(server.Model))
            {
                MessageBox.Show(this, "모델명을 입력하거나 모델 목록에서 선택하세요.", "모델명 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetBusy(true, "연결 테스트 중...");
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(8));
                _ = await ollamaClient.GetModelsAsync(server.BaseUrl, cts.Token);

                UpdateStatus("모델 준비 중...");
                using var warmupCts = new CancellationTokenSource(TimeSpan.FromSeconds((double)numTimeout.Value));
                var warmup = await ollamaClient.WarmUpAsync(server.BaseUrl, server.Model, warmupCts.Token);
                lblResponseTime.Text = BuildPerformanceText(warmup);
                UpdateStatus("모델 준비 완료");
                ShowSlowLoadHint(warmup, appendToResponse: false);
                MessageBox.Show(
                    this,
                    $"Ollama 서버 연결 및 모델 워밍업에 성공했습니다.\n\n{BuildPerformanceText(warmup)}",
                    "모델 준비 완료",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (OperationCanceledException)
            {
                UpdateStatus("요청 취소됨");
                MessageBox.Show(this, $"모델 준비 요청이 취소되었거나 Timeout({numTimeout.Value:N0}초)을 초과했습니다.", "모델 준비 취소", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (OllamaRequestException ex)
            {
                UpdateStatus("오류 발생");
                MessageBox.Show(this, $"모델 준비에 실패했습니다.\n주소: {server.BaseUrl}\n모델: {server.Model}\n원인: {ex.Message}", "모델 준비 실패", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                UpdateStatus("오류 발생");
                MessageBox.Show(this, $"예상하지 못한 오류가 발생했습니다.\n주소: {server.BaseUrl}\n모델: {server.Model}\n원인: {ex.Message}", "모델 준비 실패", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task LoadModelsAsync(OllamaServer server, ComboBox target)
        {
            SetBusy(true, "모델 목록 조회 중...");
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(8));
                var modelNames = await ollamaClient.GetModelsAsync(server.BaseUrl, cts.Token);

                target.Items.Clear();
                target.Items.AddRange(modelNames.Cast<object>().ToArray());
                if (modelNames.Count > 0)
                {
                    target.Text = modelNames[0];
                }

                UpdateStatus($"모델 {modelNames.Count}개 조회");
            }
            catch (OllamaRequestException ex)
            {
                MessageBox.Show(this, $"모델 목록 조회에 실패했습니다. 모델명은 직접 입력할 수 있습니다.\n{ex.Message}", "모델 조회 실패", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                UpdateStatus("오류 발생");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"모델 목록 조회에 실패했습니다. 모델명은 직접 입력할 수 있습니다.\n{ex.Message}", "모델 조회 실패", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                UpdateStatus("오류 발생");
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void SaveRemoteServer()
        {
            SaveSettings();
            MessageBox.Show(this, "원격 서버 정보를 저장했습니다.", "저장 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ApplyQuestionMode()
        {
            var codeMode = IsCodeAnalysisMode();
            var ocrMode = IsOcrMode();
            lblTemplate.Enabled = codeMode;
            cboQuestionTemplate.Enabled = codeMode;
            lblAnalysisScope.Enabled = codeMode;
            cboAnalysisScope.Enabled = codeMode;
            fileList.Enabled = codeMode || ocrMode;
            btnSelectOcrImage.Visible = ocrMode;
            btnSelectOcrImage.Enabled = ocrMode;
            btnPreviewPrompt.Text = codeMode ? "미리보기" : "내용확인";

            if (ocrMode)
            {
                txtQuestion.Text = "이미지에 보이는 글자를 줄바꿈과 문단 구조를 최대한 유지해서 그대로 추출해줘. 설명은 붙이지 말고 OCR 결과만 출력해줘.";
                UpdateStatus($"OCR 모드: 이미지 선택 후 질문하세요. 이미지 입력 지원 모델({OcrVisionModel} 등)이 필요합니다.");
            }
            else if (!codeMode)
            {
                txtQuestion.Text = "";
                UpdateStatus($"일반 질문 모드: 파일 선택 없이 Ollama에 질문할 수 있습니다. 추천 모델 {GeneralConversationModel}을 사용합니다.");
            }
            else if (string.IsNullOrWhiteSpace(txtQuestion.Text))
            {
                txtQuestion.Text = BuildQuestionFromTemplate(cboQuestionTemplate.Text);
            }

            ApplyRecommendedModelForCurrentMode();
        }

        private bool IsCodeAnalysisMode()
        {
            return cboQuestionMode.SelectedItem?.ToString() == "코드 분석";
        }

        private bool IsOcrMode()
        {
            return cboQuestionMode.SelectedItem?.ToString() == "OCR";
        }

        private void ApplyRecommendedModelForCurrentMode()
        {
            if (IsCodeAnalysisMode())
            {
                RestoreCodeModelForActiveTab();
                return;
            }

            RememberCodeModelForActiveTab();
            SetActiveModel(IsOcrMode() ? OcrVisionModel : GeneralConversationModel);
        }

        private void RememberCodeModelForActiveTab()
        {
            if (connectionTabs.SelectedTab == remoteTab)
            {
                remoteCodeModelBeforeGeneral ??= cboRemoteModel.Text.Trim();
                return;
            }

            localCodeModelBeforeGeneral ??= cboLocalModel.Text.Trim();
        }

        private void RestoreCodeModelForActiveTab()
        {
            if (connectionTabs.SelectedTab == remoteTab)
            {
                if (!string.IsNullOrWhiteSpace(remoteCodeModelBeforeGeneral))
                {
                    SetComboText(cboRemoteModel, remoteCodeModelBeforeGeneral);
                }

                remoteCodeModelBeforeGeneral = null;
                return;
            }

            if (!string.IsNullOrWhiteSpace(localCodeModelBeforeGeneral))
            {
                SetComboText(cboLocalModel, localCodeModelBeforeGeneral);
            }

            localCodeModelBeforeGeneral = null;
        }

        private void SetActiveModel(string model)
        {
            SetComboText(connectionTabs.SelectedTab == remoteTab ? cboRemoteModel : cboLocalModel, model);
        }

        private static void SetComboText(ComboBox comboBox, string model)
        {
            if (!comboBox.Items.Cast<object>().Any(item => string.Equals(item.ToString(), model, StringComparison.OrdinalIgnoreCase)))
            {
                comboBox.Items.Add(model);
            }

            comboBox.Text = model;
        }

        private void SelectOcrImage()
        {
            if (openImageDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            selectedOcrImagePath = openImageDialog.FileName;
            btnSelectOcrImage.Text = "이미지 선택됨";
            UpdateStatus($"OCR 이미지 선택: {Path.GetFileName(selectedOcrImagePath)}");
        }

        private void PreviewPrompt()
        {
            if (!TryBuildPrompt(out var prompt, out var metadata))
            {
                return;
            }

            var sensitive = DetectSensitiveInfo(prompt);
            var preview = new StringBuilder();
            preview.AppendLine($"포함 파일: {metadata.FileCount}개");
            preview.AppendLine($"총 문자 수: {metadata.CharacterCount:N0}");
            if (IsOcrMode())
            {
                preview.AppendLine($"OCR 이미지: {selectedOcrImagePath}");
            }

            preview.AppendLine($"민감정보 의심: {(sensitive.Count == 0 ? "없음" : string.Join(", ", sensitive))}");
            preview.AppendLine();
            preview.AppendLine(prompt);

            using var dialog = new Form
            {
                Text = "프롬프트 미리보기",
                StartPosition = FormStartPosition.CenterParent,
                Width = 900,
                Height = 700
            };
            var textBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                ReadOnly = true,
                Font = new Font("Consolas", 10F),
                Text = preview.ToString()
            };
            dialog.Controls.Add(textBox);
            dialog.ShowDialog(this);
        }

        private async Task SendQuestionAsync()
        {
            var server = GetSelectedServer();
            if (string.IsNullOrWhiteSpace(server.Model))
            {
                MessageBox.Show(this, "모델명을 입력하거나 모델 목록에서 선택하세요.", "모델명 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!TryBuildPrompt(out var prompt, out _))
            {
                return;
            }

            if (server.Mode == "remote")
            {
                var remoteWarning = IsCodeAnalysisMode()
                    ? "주의: 원격 Ollama 사용 시 선택한 코드 파일 내용이 사내망의 다른 PC로 전송됩니다.\n민감 정보, 비밀번호, API Key, 개인정보가 포함되어 있지 않은지 확인하십시오."
                    : IsOcrMode()
                        ? "주의: 원격 Ollama 사용 시 선택한 이미지와 입력한 질문 내용이 사내망의 다른 PC로 전송됩니다."
                        : "주의: 원격 Ollama 사용 시 입력한 질문 내용이 사내망의 다른 PC로 전송됩니다.";
                var remoteConfirm = MessageBox.Show(
                    this,
                    $"{remoteWarning}\n\n계속 전송하시겠습니까?",
                    "원격 전송 보안 확인",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                if (remoteConfirm != DialogResult.Yes)
                {
                    return;
                }
            }

            var sensitive = DetectSensitiveInfo(prompt);
            if (sensitive.Count > 0)
            {
                var result = MessageBox.Show(
                    this,
                    $"선택한 파일에 민감정보로 보이는 문자열이 포함되어 있습니다.\n탐지 항목: {string.Join(", ", sensitive)}\n\n전송을 계속하시겠습니까?",
                    "민감정보 의심 경고",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                if (result != DialogResult.Yes)
                {
                    return;
                }
            }

            currentRequestCts = new CancellationTokenSource(TimeSpan.FromSeconds((double)numTimeout.Value));
            var stopwatch = Stopwatch.StartNew();
            SetRequesting(true);
            txtResponse.Clear();

            try
            {
                var userMessage = new OllamaMessage { Role = "user", Content = prompt };
                if (IsOcrMode())
                {
                    userMessage.Images = [Convert.ToBase64String(File.ReadAllBytes(selectedOcrImagePath!))];
                }

                var request = new OllamaChatRequest
                {
                    Model = server.Model,
                    KeepAlive = OllamaClient.DefaultKeepAlive,
                    Stream = false,
                    Options = new OllamaOptions
                    {
                        Temperature = (double)numTemperature.Value,
                        NumContext = OllamaClient.DefaultNumContext,
                        NumPredict = IsCodeAnalysisMode()
                            ? OllamaClient.CodeReviewNumPredict
                            : IsOcrMode()
                                ? OllamaClient.OcrNumPredict
                                : OllamaClient.DefaultNumPredict
                    },
                    Messages =
                    [
                        new OllamaMessage { Role = "system", Content = BuildSystemPrompt(IsCodeAnalysisMode(), IsOcrMode()) },
                        userMessage
                    ]
                };

                var chat = await ollamaClient.ChatAsync(server.BaseUrl, request, currentRequestCts.Token);
                txtResponse.Text = chat.Message?.Content ?? "Ollama 응답에 message.content가 없습니다.";
                stopwatch.Stop();
                lblResponseTime.Text = BuildPerformanceText(chat);
                ShowSlowLoadHint(chat, appendToResponse: true);
                ShowLengthLimitHint(chat);
                UpdateStatus("응답 수신 완료");
            }
            catch (OperationCanceledException)
            {
                var message = currentRequestCts?.IsCancellationRequested == true
                    ? $"요청이 취소되었거나 설정한 Timeout({numTimeout.Value:N0}초)을 초과했습니다."
                    : "HTTP 요청이 취소되었습니다.";
                UpdateStatus("요청 취소됨");
                txtResponse.Text = $"{message}\n\n첫 요청이라면 Ollama가 모델을 메모리에 올리는 시간이 포함될 수 있습니다. Timeout 값을 늘리거나 `ollama ps`로 모델 로딩 상태를 확인해 보세요.";
            }
            catch (OllamaRequestException ex)
            {
                txtResponse.Text = $"Ollama 호출 중 오류가 발생했습니다.\n\n서버: {server.BaseUrl}\n모델: {server.Model}\n오류: {ex.Message}";
                UpdateStatus("오류 발생");
            }
            catch (Exception ex)
            {
                txtResponse.Text = $"Ollama 호출 중 오류가 발생했습니다.\n\n서버: {server.BaseUrl}\n모델: {server.Model}\n오류: {ex.Message}";
                UpdateStatus("오류 발생");
            }
            finally
            {
                stopwatch.Stop();
                currentRequestCts.Dispose();
                currentRequestCts = null;
                SetRequesting(false);
            }
        }

        private bool TryBuildPrompt(out string prompt, out PromptMetadata metadata)
        {
            prompt = string.Empty;
            metadata = new PromptMetadata();

            var question = txtQuestion.Text.Trim();
            if (string.IsNullOrWhiteSpace(question))
            {
                MessageBox.Show(this, "질문을 입력하세요.", "질문 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (IsOcrMode())
            {
                var imagePath = ResolveOcrImagePath();
                if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
                {
                    MessageBox.Show(this, "OCR에 사용할 이미지 파일을 선택하세요. 이미지 버튼을 누르거나 폴더 목록에서 이미지 파일을 선택할 수 있습니다.", "이미지 선택 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                selectedOcrImagePath = imagePath;
                prompt = question;
                metadata = new PromptMetadata(1, prompt.Length);
                return true;
            }

            if (!IsCodeAnalysisMode())
            {
                prompt = question;
                metadata = new PromptMetadata(0, prompt.Length);
                return true;
            }

            var files = SelectFilesForPrompt();
            if (files.Count == 0)
            {
                MessageBox.Show(this, "Ollama에 보낼 파일을 선택하세요.", "파일 선택 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            var builder = new StringBuilder();
            builder.AppendLine("[User Question]");
            builder.AppendLine(question);
            builder.AppendLine();
            builder.AppendLine("[Project Info]");
            builder.AppendLine($"- 프로젝트 경로: {txtProjectPath.Text}");
            builder.AppendLine($"- 프로젝트 유형: {InferProjectType(files)}");
            builder.AppendLine("- .NET 버전: 추가 확인 필요");
            builder.AppendLine("- 실행 환경: Windows / Client PC");
            builder.AppendLine("- 사용 목적: 추가 확인 필요");
            builder.AppendLine("선택 파일 목록:");
            foreach (var file in files)
            {
                builder.AppendLine($"- {file.RelativePath} ({file.Size:N0} bytes)");
            }
            builder.AppendLine();
            builder.AppendLine("[Review Focus]");
            builder.AppendLine("특히 중점적으로 봐야 할 부분:");
            builder.AppendLine("- 동시성");
            builder.AppendLine("- 보안");
            builder.AppendLine("- UI Thread");
            builder.AppendLine("- 메모리 누수");
            builder.AppendLine("- API 설계");
            builder.AppendLine();
            builder.AppendLine("[Code Context]");

            var includedCount = 0;
            foreach (var file in files)
            {
                if (includedCount >= MaxFilesPerRequest)
                {
                    builder.AppendLine($"[제외] 1회 요청 최대 파일 수({MaxFilesPerRequest}) 초과로 이후 파일은 제외했습니다.");
                    break;
                }

                if (file.Size > MaxFileSizeBytes)
                {
                    builder.AppendLine($"[제외] {file.RelativePath}: 파일 크기 {file.Size:N0} bytes로 100KB를 초과합니다.");
                    continue;
                }

                if (IsImageFile(file.FullPath))
                {
                    builder.AppendLine($"[제외] {file.RelativePath}: 이미지 파일은 코드 분석 본문에 포함하지 않습니다. OCR 모드에서 사용하세요.");
                    continue;
                }

                try
                {
                    var content = File.ReadAllText(file.FullPath);
                    builder.AppendLine($"--- FILE: {file.RelativePath} ---");
                    builder.AppendLine(content);
                    builder.AppendLine($"--- END FILE: {file.RelativePath} ---");
                    builder.AppendLine();
                    includedCount++;
                }
                catch (Exception ex)
                {
                    builder.AppendLine($"[읽기 실패] {file.RelativePath}: {ex.Message}");
                }

                if (builder.Length > MaxPromptCharacters)
                {
                    builder.AppendLine($"[중단] 1회 요청 최대 문자 수({MaxPromptCharacters:N0})를 초과하여 이후 내용은 제외했습니다.");
                    break;
                }
            }

            builder.AppendLine("[Output Format]");
            builder.AppendLine("다음 형식으로 답변해라.");
            builder.AppendLine();
            builder.AppendLine("1. 요약");
            builder.AppendLine("   - 전체 평가");
            builder.AppendLine("   - 가장 중요한 위험 1~3개");
            builder.AppendLine();
            builder.AppendLine("2. 심각한 문제");
            builder.AppendLine("   각 항목은 다음 형식으로 작성:");
            builder.AppendLine("   - 문제:");
            builder.AppendLine("   - 심각도: Critical / High / Medium / Low");
            builder.AppendLine("   - 근거:");
            builder.AppendLine("   - 영향:");
            builder.AppendLine("   - 수정 방향:");
            builder.AppendLine();
            builder.AppendLine("3. 개선 권장 사항");
            builder.AppendLine("   - 단기 수정");
            builder.AppendLine("   - 장기 개선");
            builder.AppendLine();
            builder.AppendLine("4. 수정 예시 코드");
            builder.AppendLine("   - 최소 수정 예시");
            builder.AppendLine("   - 필요 시 구조 개선 예시");
            builder.AppendLine();
            builder.AppendLine("5. 추가 확인이 필요한 부분");
            builder.AppendLine("   - 제공된 코드만으로 판단 불가능한 항목");
            builder.AppendLine();
            builder.AppendLine("6. 우선순위");
            builder.AppendLine("   - 지금 바로 수정");
            builder.AppendLine("   - 다음 단계에서 수정");
            builder.AppendLine("   - 여유가 있을 때 개선");

            prompt = builder.ToString();
            metadata = new PromptMetadata(includedCount, prompt.Length);
            return includedCount > 0;
        }

        private List<ProjectFile> SelectFilesForPrompt()
        {
            var scope = cboAnalysisScope.SelectedItem?.ToString();
            if (scope == "현재 선택한 파일만" && fileList.SelectedItem is ProjectFile selected)
            {
                return [selected];
            }

            if (scope == "주요 파일 자동 선택")
            {
                return scannedFiles
                    .Where(ShouldPreselect)
                    .OrderBy(f => f.Extension.Equals(".sln", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                    .ThenBy(f => f.RelativePath)
                    .Take(MaxFilesPerRequest)
                    .ToList();
            }

            return fileList.CheckedItems.Cast<ProjectFile>().ToList();
        }

        private static string InferProjectType(IReadOnlyCollection<ProjectFile> files)
        {
            if (files.Any(file => file.RelativePath.EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase)))
            {
                return "WinForms";
            }

            if (files.Any(file => Path.GetFileName(file.FullPath).Equals("appsettings.json", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(file.FullPath).Equals("Program.cs", StringComparison.OrdinalIgnoreCase)
                || file.RelativePath.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase)))
            {
                foreach (var file in files.Where(file => file.Extension.Equals(".csproj", StringComparison.OrdinalIgnoreCase) || Path.GetFileName(file.FullPath).Equals("Program.cs", StringComparison.OrdinalIgnoreCase)))
                {
                    try
                    {
                        var content = File.ReadAllText(file.FullPath);
                        if (content.Contains("UseWindowsForms", StringComparison.OrdinalIgnoreCase))
                        {
                            return "WinForms";
                        }

                        if (content.Contains("Microsoft.NET.Sdk.Web", StringComparison.OrdinalIgnoreCase)
                            || content.Contains("WebApplication.CreateBuilder", StringComparison.OrdinalIgnoreCase))
                        {
                            return "ASP.NET Core";
                        }

                        if (content.Contains("Microsoft.NET.Sdk.Worker", StringComparison.OrdinalIgnoreCase)
                            || content.Contains("BackgroundService", StringComparison.OrdinalIgnoreCase))
                        {
                            return "Worker Service";
                        }
                    }
                    catch
                    {
                        return "추가 확인 필요";
                    }
                }
            }

            return "추가 확인 필요";
        }

        private string? ResolveOcrImagePath()
        {
            if (!string.IsNullOrWhiteSpace(selectedOcrImagePath) && File.Exists(selectedOcrImagePath))
            {
                return selectedOcrImagePath;
            }

            if (fileList.SelectedItem is ProjectFile selected && IsImageFile(selected.FullPath))
            {
                return selected.FullPath;
            }

            return fileList.CheckedItems
                .Cast<ProjectFile>()
                .FirstOrDefault(file => IsImageFile(file.FullPath))
                ?.FullPath;
        }

        private static bool IsImageFile(string path)
        {
            return ImageExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);
        }

        private static string BuildSystemPrompt(bool codeAnalysisMode, bool ocrMode)
        {
            if (ocrMode)
            {
                return """
                    너는 OCR 전용 assistant다.
                    이미지에 보이는 글자를 가능한 한 정확히 추출한다.
                    원문 줄바꿈, 번호, 표기, 문장부호를 최대한 유지한다.
                    보이지 않는 내용은 추측하지 말고 읽을 수 없는 부분은 [판독 불가]로 표시한다.
                    설명, 요약, 의견을 붙이지 말고 OCR 결과만 출력한다.
                    """;
            }

            if (!codeAnalysisMode)
            {
                return """
                    너는 사용자의 질문에 정확하고 실용적으로 답하는 한국어 AI assistant다.
                    질문 의도가 불명확하면 필요한 가정을 짧게 밝히고 답한다.
                    코드, 문서, 번역, 요약, 아이디어 정리 등 일반적인 요청을 자연스럽게 처리한다.
                    모르는 내용은 꾸며내지 말고 "추가 확인 필요"라고 말한다.
                    """;
            }

            return """
                너는 실무 중심의 C#/.NET 코드 리뷰어다.
                운영 환경에서 장애, 보안 사고, 성능 저하, 유지보수 비용을 줄이는 관점으로 코드를 분석한다.

                다음 기준으로 분석한다.
                - 컴파일 오류 가능성
                - 런타임 오류 가능성
                - NullReferenceException 가능성
                - 예외 처리 누락
                - async/await 오용
                - 동시성 문제
                - thread-safety 문제
                - WinForms UI Thread 문제
                - 메모리 누수 가능성
                - IDisposable 처리
                - 성능 문제
                - 보안 문제
                - 입력값 검증
                - 인증/인가 필요성
                - 로그/감사 추적 필요성
                - HTTP API 설계 문제
                - 유지보수성
                - 네이밍
                - 중복 코드
                - 설계 구조

                분석 규칙:
                - 제공된 코드만으로 확실히 판단 가능한 문제와 추가 확인이 필요한 문제를 구분한다.
                확실하지 않은 내용은 추측하지 말고 "추가 확인 필요"라고 표시한다.
                - 프로젝트 유형에 해당하지 않는 기준은 억지로 지적하지 말고 "해당 없음"으로 표시한다.
                - 문제를 지적할 때는 "왜 문제인지"와 "어떻게 고칠지"를 함께 설명한다.
                수정 제안이 있으면 가능한 한 C# 코드 예시를 제공한다.
                - 코드 예시는 전체 파일을 임의로 재작성하지 말고, 핵심 수정 부분 위주로 제공한다.
                - 전체 구조 변경이 필요한 경우에는 별도 섹션에서 장기 개선안으로 제안한다.

                금지 사항:
                - 근거 없이 문제라고 단정하지 않는다.
                - 코드에 없는 요구사항이나 프레임워크를 사용 중이라고 가정하지 않는다.
                - thread-safe 여부가 불명확한 객체를 무작정 Task.Run으로 병렬화하라고 제안하지 않는다.
                - 클라이언트에 내부 예외 메시지를 그대로 반환하는 코드를 운영용 개선안으로 제안하지 않는다.
                - 모든 오류를 200 OK 문자열 응답으로 처리하는 코드를 좋은 방식으로 제안하지 않는다.
                - 단순히 "MVC로 바꿔라", "리팩토링해라" 같은 추상적인 조언만 하지 않는다.

                출력은 구체적이고 실무적으로 작성한다.
                """;
        }

        private static string BuildQuestionFromTemplate(string template)
        {
            return template switch
            {
                "전체 프로젝트 구조 분석" => "선택된 주요 파일을 바탕으로 프로젝트 구조와 책임 분리를 분석해줘.",
                "버그 가능성 분석" => "선택한 코드에서 컴파일 오류, 런타임 오류, 예외 처리 누락 가능성을 찾아줘.",
                "성능 개선점 찾기" => "선택한 코드에서 성능 병목 가능성과 개선 방안을 찾아줘.",
                "async/await 문제 찾기" => "선택한 코드에서 async/await 오용, deadlock 가능성, 취소 처리 누락을 찾아줘.",
                "WinForms UI Thread 문제 찾기" => "선택한 코드에서 WinForms UI Thread 접근 문제와 응답성 저하 가능성을 찾아줘.",
                "예외 처리 누락 찾기" => "선택한 코드에서 예외 처리 누락 또는 사용자에게 불친절한 오류 처리를 찾아줘.",
                "보안 취약점 찾기" => "선택한 코드에서 보안 취약점, 민감정보 노출, 입력값 검증 누락을 찾아줘.",
                "리팩토링 제안" => "선택한 코드의 유지보수성을 높이기 위한 리팩토링 방향과 예시 코드를 제안해줘.",
                "테스트 코드 작성 요청" => "선택한 코드에 대해 우선순위가 높은 테스트 케이스와 예시 테스트 코드를 작성해줘.",
                _ => "선택한 파일을 코드 리뷰해줘. 심각한 문제부터 우선순위 순서로 알려줘."
            };
        }

        private static List<string> DetectSensitiveInfo(string text)
        {
            var detections = new List<string>();
            var patterns = new Dictionary<string, string>
            {
                ["API Key/Token"] = @"(?i)\b(api[_-]?key|apikey|access[_-]?token|bearer\s+[a-z0-9._\-]+)\b",
                ["Password/Secret"] = @"(?i)\b(password|pwd|secret)\b\s*[:=]",
                ["DB Connection String"] = @"(?i)\b(Server|Data Source)\s*=.+\b(User Id|UID|Password|PWD)\s*=",
                ["JWT Secret"] = @"(?i)(Jwt:Secret|IssuerSigningKey)",
                ["Email"] = @"[A-Z0-9._%+\-]+@[A-Z0-9.\-]+\.[A-Z]{2,}",
                ["Phone"] = @"(?<!\d)(01[016789]-?\d{3,4}-?\d{4})(?!\d)"
            };

            foreach (var (name, pattern) in patterns)
            {
                if (Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline))
                {
                    detections.Add(name);
                }
            }

            return detections;
        }

        private void CopyResponse()
        {
            if (!string.IsNullOrWhiteSpace(txtResponse.Text))
            {
                Clipboard.SetText(txtResponse.Text);
                UpdateStatus("답변을 클립보드에 복사했습니다.");
            }
        }

        private void SaveResponse()
        {
            if (string.IsNullOrWhiteSpace(txtResponse.Text))
            {
                MessageBox.Show(this, "저장할 답변이 없습니다.", "답변 없음", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            saveFileDialog.FileName = $"zzollo-response-{DateTime.Now:yyyyMMdd-HHmmss}.md";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, txtResponse.Text, Encoding.UTF8);
                UpdateStatus("답변 저장 완료");
            }
        }

        private void SetBusy(bool busy, string? message = null)
        {
            progressBar.Visible = busy;
            progressBar.Style = busy ? ProgressBarStyle.Marquee : ProgressBarStyle.Continuous;
            if (!string.IsNullOrWhiteSpace(message))
            {
                UpdateStatus(message);
            }
        }

        private void SetRequesting(bool requesting)
        {
            btnSend.Enabled = !requesting;
            btnCancel.Enabled = requesting;
            btnPreviewPrompt.Enabled = !requesting;
            btnSelectOcrImage.Enabled = !requesting && IsOcrMode();
            SetBusy(requesting, requesting ? "Ollama 응답 대기 중..." : null);
        }

        private void UpdateStatus(string message)
        {
            statusLabel.Text = message;
        }

        private static double NsToSeconds(long nanoseconds)
        {
            return nanoseconds / 1_000_000_000.0;
        }

        private static string BuildPerformanceText(OllamaChatResponse response)
        {
            return $"전체: {NsToSeconds(response.TotalDuration):N2}s / 로딩: {NsToSeconds(response.LoadDuration):N2}s / 프롬프트: {NsToSeconds(response.PromptEvalDuration):N2}s / 생성: {NsToSeconds(response.EvalDuration):N2}s / 프롬프트토큰: {response.PromptEvalCount:N0} / 생성토큰: {response.EvalCount:N0}";
        }

        private void ShowSlowLoadHint(OllamaChatResponse response, bool appendToResponse)
        {
            if (NsToSeconds(response.LoadDuration) < 5)
            {
                return;
            }

            const string hint = "모델 로딩에 시간이 오래 걸렸습니다. 최초 요청에서는 정상일 수 있습니다. keep_alive 설정으로 이후 요청은 빨라질 수 있습니다.";
            UpdateStatus(hint);
            if (appendToResponse)
            {
                txtResponse.Text = $"{hint}{Environment.NewLine}{Environment.NewLine}{txtResponse.Text}";
            }
        }

        private void ShowLengthLimitHint(OllamaChatResponse response)
        {
            if (!string.Equals(response.DoneReason, "length", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var limit = IsCodeAnalysisMode()
                ? OllamaClient.CodeReviewNumPredict
                : IsOcrMode()
                    ? OllamaClient.OcrNumPredict
                    : OllamaClient.DefaultNumPredict;
            var hint = $"답변이 생성 토큰 제한으로 중간에 멈췄습니다. 현재 제한: {limit:N0} 토큰";
            txtResponse.Text = $"{txtResponse.Text}{Environment.NewLine}{Environment.NewLine}{hint}";
            UpdateStatus(hint);
        }

        private sealed record ProjectFile(string RootPath, string FullPath, long Size)
        {
            public string RelativePath => Path.GetRelativePath(RootPath, FullPath);
            public string Extension => Path.GetExtension(FullPath);
            public override string ToString() => $"{RelativePath} ({Size / 1024.0:N1} KB)";
        }

        private sealed record OllamaServer(string Mode, string BaseUrl, string Model);

        private sealed record PromptMetadata(int FileCount = 0, int CharacterCount = 0);

        private sealed class AppSettings
        {
            public LocalSettings Local { get; set; } = new();
            public RemoteSettings Remote { get; set; } = new();
            public AnalysisSettings Analysis { get; set; } = new();
            public List<string> RecentFolders { get; set; } = [];
        }

        private sealed class LocalSettings
        {
            public string Host { get; set; } = "localhost";
            public int Port { get; set; } = 11434;
            public string Model { get; set; } = "llama3.1";
        }

        private sealed class RemoteSettings
        {
            public string Alias { get; set; } = "개발팀 GPU PC";
            public string Ip { get; set; } = "192.168.0.50";
            public int Port { get; set; } = 11434;
            public string Model { get; set; } = "qwen2.5-coder";
        }

        private sealed class AnalysisSettings
        {
            public bool ExcludeBinObj { get; set; } = true;
            public bool ExcludeDesignerFiles { get; set; } = true;
        }

    }
}
