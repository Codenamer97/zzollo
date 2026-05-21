namespace zzollo
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                currentRequestCts?.Dispose();
                ollamaClient.Dispose();
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            topPanel = new Panel();
            lblFileCount = new Label();
            cboRecentFolders = new ComboBox();
            btnRefresh = new Button();
            btnSelectFolder = new Button();
            txtProjectPath = new TextBox();
            lblProjectPath = new Label();
            mainSplit = new SplitContainer();
            leftPanel = new Panel();
            fileList = new CheckedListBox();
            fileOptionsPanel = new FlowLayoutPanel();
            chkExcludeBinObj = new CheckBox();
            chkIncludeDesigner = new CheckBox();
            lblExtensionFilter = new Label();
            cboExtensionFilter = new ComboBox();
            btnToggleAllFiles = new Button();
            rightSplit = new SplitContainer();
            workPanel = new TableLayoutPanel();
            connectionTabs = new TabControl();
            localTab = new TabPage();
            localFlow = new FlowLayoutPanel();
            lblLocalHost = new Label();
            txtLocalHost = new TextBox();
            lblLocalPort = new Label();
            numLocalPort = new NumericUpDown();
            lblLocalModel = new Label();
            cboLocalModel = new ComboBox();
            btnLocalTest = new Button();
            btnLocalModels = new Button();
            lblTemperature = new Label();
            numTemperature = new NumericUpDown();
            lblTimeout = new Label();
            numTimeout = new NumericUpDown();
            remoteTab = new TabPage();
            remoteLayout = new TableLayoutPanel();
            lblRemoteWarning = new Label();
            remoteFlow = new FlowLayoutPanel();
            lblRemoteAlias = new Label();
            txtRemoteAlias = new TextBox();
            lblRemoteIp = new Label();
            txtRemoteIp = new TextBox();
            lblRemotePort = new Label();
            numRemotePort = new NumericUpDown();
            lblRemoteModel = new Label();
            cboRemoteModel = new ComboBox();
            btnRemoteTest = new Button();
            btnRemoteModels = new Button();
            btnSaveRemote = new Button();
            questionPanel = new TableLayoutPanel();
            questionTopPanel = new FlowLayoutPanel();
            lblQuestionMode = new Label();
            cboQuestionMode = new ComboBox();
            lblTemplate = new Label();
            cboQuestionTemplate = new ComboBox();
            lblAnalysisScope = new Label();
            cboAnalysisScope = new ComboBox();
            btnSelectOcrImage = new Button();
            btnPreviewPrompt = new Button();
            btnSend = new Button();
            btnCancel = new Button();
            txtQuestion = new TextBox();
            responsePanel = new TableLayoutPanel();
            responseTopPanel = new FlowLayoutPanel();
            btnCopyResponse = new Button();
            btnSaveResponse = new Button();
            lblResponseTime = new Label();
            txtResponse = new RichTextBox();
            statusPanel = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            progressBar = new ToolStripProgressBar();
            folderBrowserDialog = new FolderBrowserDialog();
            saveFileDialog = new SaveFileDialog();
            openImageDialog = new OpenFileDialog();
            topPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)mainSplit).BeginInit();
            mainSplit.Panel1.SuspendLayout();
            mainSplit.Panel2.SuspendLayout();
            mainSplit.SuspendLayout();
            leftPanel.SuspendLayout();
            fileOptionsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)rightSplit).BeginInit();
            rightSplit.Panel1.SuspendLayout();
            rightSplit.Panel2.SuspendLayout();
            rightSplit.SuspendLayout();
            workPanel.SuspendLayout();
            connectionTabs.SuspendLayout();
            localTab.SuspendLayout();
            localFlow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numLocalPort).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numTemperature).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numTimeout).BeginInit();
            remoteTab.SuspendLayout();
            remoteLayout.SuspendLayout();
            remoteFlow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numRemotePort).BeginInit();
            questionPanel.SuspendLayout();
            questionTopPanel.SuspendLayout();
            responsePanel.SuspendLayout();
            responseTopPanel.SuspendLayout();
            statusPanel.SuspendLayout();
            SuspendLayout();
            // 
            // topPanel
            // 
            topPanel.Controls.Add(lblFileCount);
            topPanel.Controls.Add(cboRecentFolders);
            topPanel.Controls.Add(btnRefresh);
            topPanel.Controls.Add(btnSelectFolder);
            topPanel.Controls.Add(txtProjectPath);
            topPanel.Controls.Add(lblProjectPath);
            topPanel.Dock = DockStyle.Top;
            topPanel.Location = new Point(0, 0);
            topPanel.Name = "topPanel";
            topPanel.Padding = new Padding(12, 10, 12, 8);
            topPanel.Size = new Size(1244, 70);
            topPanel.TabIndex = 0;
            // 
            // lblFileCount
            // 
            lblFileCount.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblFileCount.AutoSize = true;
            lblFileCount.Location = new Point(1143, 40);
            lblFileCount.Name = "lblFileCount";
            lblFileCount.Size = new Size(82, 15);
            lblFileCount.TabIndex = 5;
            lblFileCount.Text = "파일 0개 선택";
            // 
            // cboRecentFolders
            // 
            cboRecentFolders.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cboRecentFolders.DropDownStyle = ComboBoxStyle.DropDownList;
            cboRecentFolders.FormattingEnabled = true;
            cboRecentFolders.Location = new Point(97, 37);
            cboRecentFolders.Name = "cboRecentFolders";
            cboRecentFolders.Size = new Size(738, 23);
            cboRecentFolders.TabIndex = 4;
            // 
            // btnRefresh
            // 
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefresh.Location = new Point(1049, 10);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(88, 26);
            btnRefresh.TabIndex = 3;
            btnRefresh.Text = "새로고침";
            btnRefresh.UseVisualStyleBackColor = true;
            // 
            // btnSelectFolder
            // 
            btnSelectFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSelectFolder.Location = new Point(1143, 10);
            btnSelectFolder.Name = "btnSelectFolder";
            btnSelectFolder.Size = new Size(89, 26);
            btnSelectFolder.TabIndex = 2;
            btnSelectFolder.Text = "폴더 선택";
            btnSelectFolder.UseVisualStyleBackColor = true;
            // 
            // txtProjectPath
            // 
            txtProjectPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtProjectPath.Location = new Point(97, 11);
            txtProjectPath.Name = "txtProjectPath";
            txtProjectPath.ReadOnly = true;
            txtProjectPath.Size = new Size(946, 23);
            txtProjectPath.TabIndex = 1;
            // 
            // lblProjectPath
            // 
            lblProjectPath.AutoSize = true;
            lblProjectPath.Location = new Point(13, 15);
            lblProjectPath.Name = "lblProjectPath";
            lblProjectPath.Size = new Size(83, 15);
            lblProjectPath.TabIndex = 0;
            lblProjectPath.Text = "프로젝트 폴더";
            // 
            // mainSplit
            // 
            mainSplit.Dock = DockStyle.Fill;
            mainSplit.Location = new Point(0, 70);
            mainSplit.Name = "mainSplit";
            // 
            // mainSplit.Panel1
            // 
            mainSplit.Panel1.Controls.Add(leftPanel);
            mainSplit.Panel1MinSize = 280;
            // 
            // mainSplit.Panel2
            // 
            mainSplit.Panel2.Controls.Add(rightSplit);
            mainSplit.Panel2MinSize = 560;
            mainSplit.Size = new Size(1244, 671);
            mainSplit.SplitterDistance = 348;
            mainSplit.TabIndex = 1;
            // 
            // leftPanel
            // 
            leftPanel.Controls.Add(fileList);
            leftPanel.Controls.Add(fileOptionsPanel);
            leftPanel.Dock = DockStyle.Fill;
            leftPanel.Location = new Point(0, 0);
            leftPanel.Name = "leftPanel";
            leftPanel.Padding = new Padding(12, 0, 8, 8);
            leftPanel.Size = new Size(348, 671);
            leftPanel.TabIndex = 0;
            // 
            // fileList
            // 
            fileList.CheckOnClick = true;
            fileList.Dock = DockStyle.Fill;
            fileList.FormattingEnabled = true;
            fileList.HorizontalScrollbar = true;
            fileList.Location = new Point(12, 62);
            fileList.Name = "fileList";
            fileList.Size = new Size(328, 601);
            fileList.TabIndex = 1;
            // 
            // fileOptionsPanel
            // 
            fileOptionsPanel.Controls.Add(chkExcludeBinObj);
            fileOptionsPanel.Controls.Add(chkIncludeDesigner);
            fileOptionsPanel.Controls.Add(lblExtensionFilter);
            fileOptionsPanel.Controls.Add(cboExtensionFilter);
            fileOptionsPanel.Controls.Add(btnToggleAllFiles);
            fileOptionsPanel.Dock = DockStyle.Top;
            fileOptionsPanel.Location = new Point(12, 0);
            fileOptionsPanel.Name = "fileOptionsPanel";
            fileOptionsPanel.Size = new Size(328, 62);
            fileOptionsPanel.TabIndex = 0;
            // 
            // chkExcludeBinObj
            // 
            chkExcludeBinObj.AutoSize = true;
            chkExcludeBinObj.Checked = true;
            chkExcludeBinObj.CheckState = CheckState.Checked;
            chkExcludeBinObj.Location = new Point(3, 3);
            chkExcludeBinObj.Name = "chkExcludeBinObj";
            chkExcludeBinObj.Size = new Size(93, 19);
            chkExcludeBinObj.TabIndex = 0;
            chkExcludeBinObj.Text = "bin/obj 제외";
            chkExcludeBinObj.UseVisualStyleBackColor = true;
            // 
            // chkIncludeDesigner
            // 
            chkIncludeDesigner.AutoSize = true;
            chkIncludeDesigner.Location = new Point(102, 3);
            chkIncludeDesigner.Name = "chkIncludeDesigner";
            chkIncludeDesigner.Size = new Size(115, 19);
            chkIncludeDesigner.TabIndex = 1;
            chkIncludeDesigner.Text = "Designer.cs 포함";
            chkIncludeDesigner.UseVisualStyleBackColor = true;
            // 
            // lblExtensionFilter
            // 
            lblExtensionFilter.AutoSize = true;
            lblExtensionFilter.Location = new Point(223, 0);
            lblExtensionFilter.Name = "lblExtensionFilter";
            lblExtensionFilter.Size = new Size(71, 15);
            lblExtensionFilter.TabIndex = 2;
            lblExtensionFilter.Text = "확장자 필터";
            // 
            // cboExtensionFilter
            // 
            cboExtensionFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cboExtensionFilter.FormattingEnabled = true;
            cboExtensionFilter.Location = new Point(3, 28);
            cboExtensionFilter.Name = "cboExtensionFilter";
            cboExtensionFilter.Size = new Size(111, 23);
            cboExtensionFilter.TabIndex = 3;
            // 
            // btnToggleAllFiles
            // 
            btnToggleAllFiles.Location = new Point(120, 28);
            btnToggleAllFiles.Name = "btnToggleAllFiles";
            btnToggleAllFiles.Size = new Size(88, 25);
            btnToggleAllFiles.TabIndex = 4;
            btnToggleAllFiles.Text = "전체 선택";
            btnToggleAllFiles.UseVisualStyleBackColor = true;
            // 
            // rightSplit
            // 
            rightSplit.Dock = DockStyle.Fill;
            rightSplit.Location = new Point(0, 0);
            rightSplit.Name = "rightSplit";
            rightSplit.Orientation = Orientation.Horizontal;
            // 
            // rightSplit.Panel1
            // 
            rightSplit.Panel1.Controls.Add(workPanel);
            rightSplit.Panel1MinSize = 280;
            // 
            // rightSplit.Panel2
            // 
            rightSplit.Panel2.Controls.Add(responsePanel);
            rightSplit.Panel2MinSize = 230;
            rightSplit.Size = new Size(892, 671);
            rightSplit.SplitterDistance = 325;
            rightSplit.TabIndex = 0;
            // 
            // workPanel
            // 
            workPanel.ColumnCount = 1;
            workPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            workPanel.Controls.Add(connectionTabs, 0, 0);
            workPanel.Controls.Add(questionPanel, 0, 1);
            workPanel.Dock = DockStyle.Fill;
            workPanel.Location = new Point(0, 0);
            workPanel.Name = "workPanel";
            workPanel.RowCount = 2;
            workPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 125F));
            workPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            workPanel.Size = new Size(892, 325);
            workPanel.TabIndex = 0;
            // 
            // connectionTabs
            // 
            connectionTabs.Controls.Add(localTab);
            connectionTabs.Controls.Add(remoteTab);
            connectionTabs.Dock = DockStyle.Fill;
            connectionTabs.Location = new Point(3, 3);
            connectionTabs.Name = "connectionTabs";
            connectionTabs.SelectedIndex = 0;
            connectionTabs.Size = new Size(886, 119);
            connectionTabs.TabIndex = 0;
            // 
            // localTab
            // 
            localTab.Controls.Add(localFlow);
            localTab.Location = new Point(4, 24);
            localTab.Name = "localTab";
            localTab.Padding = new Padding(8);
            localTab.Size = new Size(878, 91);
            localTab.TabIndex = 0;
            localTab.Text = "Local Ollama";
            localTab.UseVisualStyleBackColor = true;
            // 
            // localFlow
            // 
            localFlow.Controls.Add(lblLocalHost);
            localFlow.Controls.Add(txtLocalHost);
            localFlow.Controls.Add(lblLocalPort);
            localFlow.Controls.Add(numLocalPort);
            localFlow.Controls.Add(lblLocalModel);
            localFlow.Controls.Add(cboLocalModel);
            localFlow.Controls.Add(btnLocalTest);
            localFlow.Controls.Add(btnLocalModels);
            localFlow.Controls.Add(lblTemperature);
            localFlow.Controls.Add(numTemperature);
            localFlow.Controls.Add(lblTimeout);
            localFlow.Controls.Add(numTimeout);
            localFlow.Dock = DockStyle.Fill;
            localFlow.Location = new Point(8, 8);
            localFlow.Name = "localFlow";
            localFlow.Size = new Size(862, 75);
            localFlow.TabIndex = 0;
            // 
            // lblLocalHost
            // 
            lblLocalHost.AutoSize = true;
            lblLocalHost.Location = new Point(3, 7);
            lblLocalHost.Margin = new Padding(3, 7, 3, 0);
            lblLocalHost.Name = "lblLocalHost";
            lblLocalHost.Size = new Size(32, 15);
            lblLocalHost.TabIndex = 0;
            lblLocalHost.Text = "Host";
            // 
            // txtLocalHost
            // 
            txtLocalHost.Location = new Point(41, 3);
            txtLocalHost.Name = "txtLocalHost";
            txtLocalHost.Size = new Size(110, 23);
            txtLocalHost.TabIndex = 1;
            txtLocalHost.Text = "localhost";
            // 
            // lblLocalPort
            // 
            lblLocalPort.AutoSize = true;
            lblLocalPort.Location = new Point(157, 7);
            lblLocalPort.Margin = new Padding(3, 7, 3, 0);
            lblLocalPort.Name = "lblLocalPort";
            lblLocalPort.Size = new Size(29, 15);
            lblLocalPort.TabIndex = 2;
            lblLocalPort.Text = "Port";
            // 
            // numLocalPort
            // 
            numLocalPort.Location = new Point(192, 3);
            numLocalPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            numLocalPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numLocalPort.Name = "numLocalPort";
            numLocalPort.Size = new Size(72, 23);
            numLocalPort.TabIndex = 3;
            numLocalPort.Value = new decimal(new int[] { 11434, 0, 0, 0 });
            // 
            // lblLocalModel
            // 
            lblLocalModel.AutoSize = true;
            lblLocalModel.Location = new Point(270, 7);
            lblLocalModel.Margin = new Padding(3, 7, 3, 0);
            lblLocalModel.Name = "lblLocalModel";
            lblLocalModel.Size = new Size(41, 15);
            lblLocalModel.TabIndex = 4;
            lblLocalModel.Text = "Model";
            // 
            // cboLocalModel
            // 
            cboLocalModel.FormattingEnabled = true;
            cboLocalModel.Location = new Point(317, 3);
            cboLocalModel.Name = "cboLocalModel";
            cboLocalModel.Size = new Size(150, 23);
            cboLocalModel.TabIndex = 5;
            // 
            // btnLocalTest
            // 
            btnLocalTest.Location = new Point(473, 3);
            btnLocalTest.Name = "btnLocalTest";
            btnLocalTest.Size = new Size(126, 26);
            btnLocalTest.TabIndex = 6;
            btnLocalTest.Text = "연결 테스트 / 준비";
            btnLocalTest.UseVisualStyleBackColor = true;
            // 
            // btnLocalModels
            // 
            btnLocalModels.Location = new Point(567, 3);
            btnLocalModels.Name = "btnLocalModels";
            btnLocalModels.Size = new Size(98, 26);
            btnLocalModels.TabIndex = 7;
            btnLocalModels.Text = "모델 목록";
            btnLocalModels.UseVisualStyleBackColor = true;
            // 
            // lblTemperature
            // 
            lblTemperature.AutoSize = true;
            lblTemperature.Location = new Point(671, 7);
            lblTemperature.Margin = new Padding(3, 7, 3, 0);
            lblTemperature.Name = "lblTemperature";
            lblTemperature.Size = new Size(74, 15);
            lblTemperature.TabIndex = 8;
            lblTemperature.Text = "Temperature";
            // 
            // numTemperature
            // 
            numTemperature.DecimalPlaces = 1;
            numTemperature.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numTemperature.Location = new Point(751, 3);
            numTemperature.Maximum = new decimal(new int[] { 2, 0, 0, 0 });
            numTemperature.Name = "numTemperature";
            numTemperature.Size = new Size(56, 23);
            numTemperature.TabIndex = 9;
            numTemperature.Value = new decimal(new int[] { 2, 0, 0, 65536 });
            // 
            // lblTimeout
            // 
            lblTimeout.AutoSize = true;
            lblTimeout.Location = new Point(3, 39);
            lblTimeout.Margin = new Padding(3, 7, 3, 0);
            lblTimeout.Name = "lblTimeout";
            lblTimeout.Size = new Size(64, 15);
            lblTimeout.TabIndex = 10;
            lblTimeout.Text = "Timeout(s)";
            // 
            // numTimeout
            // 
            numTimeout.Location = new Point(73, 35);
            numTimeout.Maximum = new decimal(new int[] { 600, 0, 0, 0 });
            numTimeout.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            numTimeout.Name = "numTimeout";
            numTimeout.Size = new Size(70, 23);
            numTimeout.TabIndex = 11;
            numTimeout.Value = new decimal(new int[] { 300, 0, 0, 0 });
            // 
            // remoteTab
            // 
            remoteTab.Controls.Add(remoteLayout);
            remoteTab.Location = new Point(4, 24);
            remoteTab.Name = "remoteTab";
            remoteTab.Padding = new Padding(8);
            remoteTab.Size = new Size(878, 91);
            remoteTab.TabIndex = 1;
            remoteTab.Text = "Remote Ollama";
            remoteTab.UseVisualStyleBackColor = true;
            // 
            // remoteLayout
            // 
            remoteLayout.ColumnCount = 1;
            remoteLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            remoteLayout.Controls.Add(lblRemoteWarning, 0, 0);
            remoteLayout.Controls.Add(remoteFlow, 0, 1);
            remoteLayout.Dock = DockStyle.Fill;
            remoteLayout.Location = new Point(8, 8);
            remoteLayout.Name = "remoteLayout";
            remoteLayout.RowCount = 2;
            remoteLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 27F));
            remoteLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            remoteLayout.Size = new Size(862, 75);
            remoteLayout.TabIndex = 0;
            // 
            // lblRemoteWarning
            // 
            lblRemoteWarning.AutoSize = true;
            lblRemoteWarning.Dock = DockStyle.Fill;
            lblRemoteWarning.ForeColor = Color.Firebrick;
            lblRemoteWarning.Location = new Point(3, 0);
            lblRemoteWarning.Name = "lblRemoteWarning";
            lblRemoteWarning.Size = new Size(856, 27);
            lblRemoteWarning.TabIndex = 0;
            lblRemoteWarning.Text = "주의: 원격 Ollama 사용 시 선택한 코드 파일 내용이 사내망의 다른 PC로 전송됩니다. 민감 정보, 비밀번호, API Key, 개인정보를 확인하십시오.";
            lblRemoteWarning.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // remoteFlow
            // 
            remoteFlow.Controls.Add(lblRemoteAlias);
            remoteFlow.Controls.Add(txtRemoteAlias);
            remoteFlow.Controls.Add(lblRemoteIp);
            remoteFlow.Controls.Add(txtRemoteIp);
            remoteFlow.Controls.Add(lblRemotePort);
            remoteFlow.Controls.Add(numRemotePort);
            remoteFlow.Controls.Add(lblRemoteModel);
            remoteFlow.Controls.Add(cboRemoteModel);
            remoteFlow.Controls.Add(btnRemoteTest);
            remoteFlow.Controls.Add(btnRemoteModels);
            remoteFlow.Controls.Add(btnSaveRemote);
            remoteFlow.Dock = DockStyle.Fill;
            remoteFlow.Location = new Point(3, 30);
            remoteFlow.Name = "remoteFlow";
            remoteFlow.Size = new Size(856, 42);
            remoteFlow.TabIndex = 1;
            // 
            // lblRemoteAlias
            // 
            lblRemoteAlias.AutoSize = true;
            lblRemoteAlias.Location = new Point(3, 7);
            lblRemoteAlias.Margin = new Padding(3, 7, 3, 0);
            lblRemoteAlias.Name = "lblRemoteAlias";
            lblRemoteAlias.Size = new Size(31, 15);
            lblRemoteAlias.TabIndex = 0;
            lblRemoteAlias.Text = "별칭";
            // 
            // txtRemoteAlias
            // 
            txtRemoteAlias.Location = new Point(40, 3);
            txtRemoteAlias.Name = "txtRemoteAlias";
            txtRemoteAlias.Size = new Size(100, 23);
            txtRemoteAlias.TabIndex = 1;
            txtRemoteAlias.Text = "개발팀 GPU PC";
            // 
            // lblRemoteIp
            // 
            lblRemoteIp.AutoSize = true;
            lblRemoteIp.Location = new Point(146, 7);
            lblRemoteIp.Margin = new Padding(3, 7, 3, 0);
            lblRemoteIp.Name = "lblRemoteIp";
            lblRemoteIp.Size = new Size(17, 15);
            lblRemoteIp.TabIndex = 2;
            lblRemoteIp.Text = "IP";
            // 
            // txtRemoteIp
            // 
            txtRemoteIp.Location = new Point(169, 3);
            txtRemoteIp.Name = "txtRemoteIp";
            txtRemoteIp.Size = new Size(93, 23);
            txtRemoteIp.TabIndex = 3;
            txtRemoteIp.Text = "192.168.0.50";
            // 
            // lblRemotePort
            // 
            lblRemotePort.AutoSize = true;
            lblRemotePort.Location = new Point(268, 7);
            lblRemotePort.Margin = new Padding(3, 7, 3, 0);
            lblRemotePort.Name = "lblRemotePort";
            lblRemotePort.Size = new Size(29, 15);
            lblRemotePort.TabIndex = 4;
            lblRemotePort.Text = "Port";
            // 
            // numRemotePort
            // 
            numRemotePort.Location = new Point(303, 3);
            numRemotePort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            numRemotePort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numRemotePort.Name = "numRemotePort";
            numRemotePort.Size = new Size(72, 23);
            numRemotePort.TabIndex = 5;
            numRemotePort.Value = new decimal(new int[] { 11434, 0, 0, 0 });
            // 
            // lblRemoteModel
            // 
            lblRemoteModel.AutoSize = true;
            lblRemoteModel.Location = new Point(381, 7);
            lblRemoteModel.Margin = new Padding(3, 7, 3, 0);
            lblRemoteModel.Name = "lblRemoteModel";
            lblRemoteModel.Size = new Size(41, 15);
            lblRemoteModel.TabIndex = 6;
            lblRemoteModel.Text = "Model";
            // 
            // cboRemoteModel
            // 
            cboRemoteModel.FormattingEnabled = true;
            cboRemoteModel.Location = new Point(428, 3);
            cboRemoteModel.Name = "cboRemoteModel";
            cboRemoteModel.Size = new Size(145, 23);
            cboRemoteModel.TabIndex = 7;
            // 
            // btnRemoteTest
            // 
            btnRemoteTest.Location = new Point(579, 3);
            btnRemoteTest.Name = "btnRemoteTest";
            btnRemoteTest.Size = new Size(126, 26);
            btnRemoteTest.TabIndex = 8;
            btnRemoteTest.Text = "연결 테스트 / 준비";
            btnRemoteTest.UseVisualStyleBackColor = true;
            // 
            // btnRemoteModels
            // 
            btnRemoteModels.Location = new Point(673, 3);
            btnRemoteModels.Name = "btnRemoteModels";
            btnRemoteModels.Size = new Size(88, 26);
            btnRemoteModels.TabIndex = 9;
            btnRemoteModels.Text = "모델 목록";
            btnRemoteModels.UseVisualStyleBackColor = true;
            // 
            // btnSaveRemote
            // 
            btnSaveRemote.Location = new Point(767, 3);
            btnSaveRemote.Name = "btnSaveRemote";
            btnSaveRemote.Size = new Size(82, 26);
            btnSaveRemote.TabIndex = 10;
            btnSaveRemote.Text = "서버 저장";
            btnSaveRemote.UseVisualStyleBackColor = true;
            // 
            // questionPanel
            // 
            questionPanel.ColumnCount = 1;
            questionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            questionPanel.Controls.Add(questionTopPanel, 0, 0);
            questionPanel.Controls.Add(txtQuestion, 0, 1);
            questionPanel.Dock = DockStyle.Fill;
            questionPanel.Location = new Point(3, 128);
            questionPanel.Name = "questionPanel";
            questionPanel.RowCount = 2;
            questionPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            questionPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            questionPanel.Size = new Size(886, 194);
            questionPanel.TabIndex = 1;
            // 
            // questionTopPanel
            // 
            questionTopPanel.Controls.Add(lblQuestionMode);
            questionTopPanel.Controls.Add(cboQuestionMode);
            questionTopPanel.Controls.Add(lblTemplate);
            questionTopPanel.Controls.Add(cboQuestionTemplate);
            questionTopPanel.Controls.Add(lblAnalysisScope);
            questionTopPanel.Controls.Add(cboAnalysisScope);
            questionTopPanel.Controls.Add(btnSelectOcrImage);
            questionTopPanel.Controls.Add(btnPreviewPrompt);
            questionTopPanel.Controls.Add(btnSend);
            questionTopPanel.Controls.Add(btnCancel);
            questionTopPanel.Dock = DockStyle.Fill;
            questionTopPanel.Location = new Point(3, 3);
            questionTopPanel.Name = "questionTopPanel";
            questionTopPanel.Size = new Size(880, 36);
            questionTopPanel.TabIndex = 0;
            questionTopPanel.WrapContents = false;
            // 
            // lblQuestionMode
            // 
            lblQuestionMode.AutoSize = true;
            lblQuestionMode.Location = new Point(3, 8);
            lblQuestionMode.Margin = new Padding(3, 8, 3, 0);
            lblQuestionMode.Name = "lblQuestionMode";
            lblQuestionMode.Size = new Size(59, 15);
            lblQuestionMode.TabIndex = 0;
            lblQuestionMode.Text = "질문 모드";
            // 
            // cboQuestionMode
            // 
            cboQuestionMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cboQuestionMode.Location = new Point(68, 3);
            cboQuestionMode.Name = "cboQuestionMode";
            cboQuestionMode.Size = new Size(95, 23);
            cboQuestionMode.TabIndex = 1;
            // 
            // lblTemplate
            // 
            lblTemplate.AutoSize = true;
            lblTemplate.Location = new Point(169, 8);
            lblTemplate.Margin = new Padding(3, 8, 3, 0);
            lblTemplate.Name = "lblTemplate";
            lblTemplate.Size = new Size(71, 15);
            lblTemplate.TabIndex = 2;
            lblTemplate.Text = "질문 템플릿";
            // 
            // cboQuestionTemplate
            // 
            cboQuestionTemplate.DropDownStyle = ComboBoxStyle.DropDownList;
            cboQuestionTemplate.Location = new Point(246, 3);
            cboQuestionTemplate.Name = "cboQuestionTemplate";
            cboQuestionTemplate.Size = new Size(158, 23);
            cboQuestionTemplate.TabIndex = 3;
            // 
            // lblAnalysisScope
            // 
            lblAnalysisScope.AutoSize = true;
            lblAnalysisScope.Location = new Point(410, 8);
            lblAnalysisScope.Margin = new Padding(3, 8, 3, 0);
            lblAnalysisScope.Name = "lblAnalysisScope";
            lblAnalysisScope.Size = new Size(59, 15);
            lblAnalysisScope.TabIndex = 4;
            lblAnalysisScope.Text = "분석 범위";
            // 
            // cboAnalysisScope
            // 
            cboAnalysisScope.DropDownStyle = ComboBoxStyle.DropDownList;
            cboAnalysisScope.Location = new Point(475, 3);
            cboAnalysisScope.Name = "cboAnalysisScope";
            cboAnalysisScope.Size = new Size(120, 23);
            cboAnalysisScope.TabIndex = 5;
            // 
            // btnSelectOcrImage
            // 
            btnSelectOcrImage.Location = new Point(601, 3);
            btnSelectOcrImage.Name = "btnSelectOcrImage";
            btnSelectOcrImage.Size = new Size(80, 26);
            btnSelectOcrImage.TabIndex = 6;
            btnSelectOcrImage.Text = "이미지";
            btnSelectOcrImage.UseVisualStyleBackColor = true;
            btnSelectOcrImage.Visible = false;
            // 
            // btnPreviewPrompt
            // 
            btnPreviewPrompt.Location = new Point(687, 3);
            btnPreviewPrompt.Name = "btnPreviewPrompt";
            btnPreviewPrompt.Size = new Size(80, 26);
            btnPreviewPrompt.TabIndex = 7;
            btnPreviewPrompt.Text = "미리보기";
            btnPreviewPrompt.UseVisualStyleBackColor = true;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(773, 3);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(70, 26);
            btnSend.TabIndex = 8;
            btnSend.Text = "질문";
            btnSend.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Enabled = false;
            btnCancel.Location = new Point(849, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(60, 26);
            btnCancel.TabIndex = 9;
            btnCancel.Text = "취소";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // txtQuestion
            // 
            txtQuestion.Dock = DockStyle.Fill;
            txtQuestion.Location = new Point(3, 45);
            txtQuestion.Multiline = true;
            txtQuestion.Name = "txtQuestion";
            txtQuestion.ScrollBars = ScrollBars.Vertical;
            txtQuestion.Size = new Size(880, 146);
            txtQuestion.TabIndex = 1;
            // 
            // responsePanel
            // 
            responsePanel.ColumnCount = 1;
            responsePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            responsePanel.Controls.Add(responseTopPanel, 0, 0);
            responsePanel.Controls.Add(txtResponse, 0, 1);
            responsePanel.Dock = DockStyle.Fill;
            responsePanel.Location = new Point(0, 0);
            responsePanel.Name = "responsePanel";
            responsePanel.RowCount = 2;
            responsePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            responsePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            responsePanel.Size = new Size(892, 342);
            responsePanel.TabIndex = 0;
            // 
            // responseTopPanel
            // 
            responseTopPanel.Controls.Add(btnCopyResponse);
            responseTopPanel.Controls.Add(btnSaveResponse);
            responseTopPanel.Controls.Add(lblResponseTime);
            responseTopPanel.Dock = DockStyle.Fill;
            responseTopPanel.Location = new Point(3, 3);
            responseTopPanel.Name = "responseTopPanel";
            responseTopPanel.Size = new Size(886, 32);
            responseTopPanel.TabIndex = 0;
            // 
            // btnCopyResponse
            // 
            btnCopyResponse.Location = new Point(3, 3);
            btnCopyResponse.Name = "btnCopyResponse";
            btnCopyResponse.Size = new Size(75, 26);
            btnCopyResponse.TabIndex = 0;
            btnCopyResponse.Text = "답변 복사";
            btnCopyResponse.UseVisualStyleBackColor = true;
            // 
            // btnSaveResponse
            // 
            btnSaveResponse.Location = new Point(84, 3);
            btnSaveResponse.Name = "btnSaveResponse";
            btnSaveResponse.Size = new Size(75, 26);
            btnSaveResponse.TabIndex = 1;
            btnSaveResponse.Text = "답변 저장";
            btnSaveResponse.UseVisualStyleBackColor = true;
            // 
            // lblResponseTime
            // 
            lblResponseTime.AutoSize = true;
            lblResponseTime.Location = new Point(165, 8);
            lblResponseTime.Margin = new Padding(3, 8, 3, 0);
            lblResponseTime.Name = "lblResponseTime";
            lblResponseTime.Size = new Size(71, 15);
            lblResponseTime.TabIndex = 2;
            lblResponseTime.Text = "응답 시간: -";
            // 
            // txtResponse
            // 
            txtResponse.Dock = DockStyle.Fill;
            txtResponse.Font = new Font("Consolas", 10F);
            txtResponse.Location = new Point(3, 41);
            txtResponse.Name = "txtResponse";
            txtResponse.ReadOnly = true;
            txtResponse.Size = new Size(886, 298);
            txtResponse.TabIndex = 1;
            txtResponse.Text = "";
            // 
            // statusPanel
            // 
            statusPanel.Items.AddRange(new ToolStripItem[] { statusLabel, progressBar });
            statusPanel.Location = new Point(0, 741);
            statusPanel.Name = "statusPanel";
            statusPanel.Size = new Size(1244, 22);
            statusPanel.TabIndex = 2;
            // 
            // statusLabel
            // 
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(43, 17);
            statusLabel.Text = "준비됨";
            // 
            // progressBar
            // 
            progressBar.Alignment = ToolStripItemAlignment.Right;
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(140, 16);
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Visible = false;
            // 
            // saveFileDialog
            // 
            saveFileDialog.Filter = "Markdown (*.md)|*.md|Text (*.txt)|*.txt";
            saveFileDialog.Title = "답변 저장";
            // 
            // openImageDialog
            // 
            openImageDialog.Filter = "Image files|*.png;*.jpg;*.jpeg;*.bmp;*.webp|All files|*.*";
            openImageDialog.Title = "OCR 이미지 선택";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1244, 763);
            Controls.Add(mainSplit);
            Controls.Add(topPanel);
            Controls.Add(statusPanel);
            MinimumSize = new Size(1020, 680);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "zzollo - Ollama 코드 분석 도구";
            topPanel.ResumeLayout(false);
            topPanel.PerformLayout();
            mainSplit.Panel1.ResumeLayout(false);
            mainSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)mainSplit).EndInit();
            mainSplit.ResumeLayout(false);
            leftPanel.ResumeLayout(false);
            fileOptionsPanel.ResumeLayout(false);
            fileOptionsPanel.PerformLayout();
            rightSplit.Panel1.ResumeLayout(false);
            rightSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)rightSplit).EndInit();
            rightSplit.ResumeLayout(false);
            workPanel.ResumeLayout(false);
            connectionTabs.ResumeLayout(false);
            localTab.ResumeLayout(false);
            localFlow.ResumeLayout(false);
            localFlow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numLocalPort).EndInit();
            ((System.ComponentModel.ISupportInitialize)numTemperature).EndInit();
            ((System.ComponentModel.ISupportInitialize)numTimeout).EndInit();
            remoteTab.ResumeLayout(false);
            remoteLayout.ResumeLayout(false);
            remoteLayout.PerformLayout();
            remoteFlow.ResumeLayout(false);
            remoteFlow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numRemotePort).EndInit();
            questionPanel.ResumeLayout(false);
            questionPanel.PerformLayout();
            questionTopPanel.ResumeLayout(false);
            questionTopPanel.PerformLayout();
            responsePanel.ResumeLayout(false);
            responseTopPanel.ResumeLayout(false);
            responseTopPanel.PerformLayout();
            statusPanel.ResumeLayout(false);
            statusPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private Panel topPanel;
        private Label lblFileCount;
        private ComboBox cboRecentFolders;
        private Button btnRefresh;
        private Button btnSelectFolder;
        private TextBox txtProjectPath;
        private Label lblProjectPath;
        private SplitContainer mainSplit;
        private Panel leftPanel;
        private CheckedListBox fileList;
        private FlowLayoutPanel fileOptionsPanel;
        private CheckBox chkExcludeBinObj;
        private CheckBox chkIncludeDesigner;
        private ComboBox cboExtensionFilter;
        private Label lblExtensionFilter;
        private Button btnToggleAllFiles;
        private SplitContainer rightSplit;
        private TableLayoutPanel workPanel;
        private TabControl connectionTabs;
        private TabPage localTab;
        private FlowLayoutPanel localFlow;
        private Label lblLocalHost;
        private TextBox txtLocalHost;
        private Label lblLocalPort;
        private NumericUpDown numLocalPort;
        private Label lblLocalModel;
        private ComboBox cboLocalModel;
        private Button btnLocalTest;
        private Button btnLocalModels;
        private Label lblTemperature;
        private NumericUpDown numTemperature;
        private Label lblTimeout;
        private NumericUpDown numTimeout;
        private TabPage remoteTab;
        private TableLayoutPanel remoteLayout;
        private Label lblRemoteWarning;
        private FlowLayoutPanel remoteFlow;
        private Label lblRemoteAlias;
        private TextBox txtRemoteAlias;
        private Label lblRemoteIp;
        private TextBox txtRemoteIp;
        private Label lblRemotePort;
        private NumericUpDown numRemotePort;
        private Label lblRemoteModel;
        private ComboBox cboRemoteModel;
        private Button btnRemoteTest;
        private Button btnRemoteModels;
        private Button btnSaveRemote;
        private TableLayoutPanel questionPanel;
        private FlowLayoutPanel questionTopPanel;
        private Label lblQuestionMode;
        private ComboBox cboQuestionMode;
        private Label lblTemplate;
        private ComboBox cboQuestionTemplate;
        private Label lblAnalysisScope;
        private ComboBox cboAnalysisScope;
        private Button btnSelectOcrImage;
        private Button btnPreviewPrompt;
        private Button btnSend;
        private Button btnCancel;
        private TextBox txtQuestion;
        private TableLayoutPanel responsePanel;
        private FlowLayoutPanel responseTopPanel;
        private Button btnCopyResponse;
        private Button btnSaveResponse;
        private Label lblResponseTime;
        private RichTextBox txtResponse;
        private StatusStrip statusPanel;
        private ToolStripStatusLabel statusLabel;
        private ToolStripProgressBar progressBar;
        private FolderBrowserDialog folderBrowserDialog;
        private SaveFileDialog saveFileDialog;
        private OpenFileDialog openImageDialog;
    }
}
