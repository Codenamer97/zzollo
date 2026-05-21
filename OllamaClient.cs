using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace zzollo
{
    public sealed class OllamaClient : IDisposable
    {
        public const string DefaultKeepAlive = "30m";
        public const int DefaultNumContext = 8192;
        public const int DefaultNumPredict = 512;
        public const int CodeReviewNumPredict = 4096;
        public const int OcrNumPredict = 2048;
        public const int WarmupNumPredict = 8;

        private readonly HttpClient httpClient = new()
        {
            Timeout = Timeout.InfiniteTimeSpan
        };

        public async Task<IReadOnlyList<string>> GetModelsAsync(string baseUrl, CancellationToken cancellationToken)
        {
            try
            {
                var tags = await httpClient.GetFromJsonAsync<OllamaTagsResponse>($"{baseUrl}/api/tags", cancellationToken);
                return tags?.Models
                    .Select(m => m.Name)
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(n => n)
                    .ToArray() ?? [];
            }
            catch (HttpRequestException ex)
            {
                throw new OllamaRequestException("Ollama 서버에 연결할 수 없습니다. 주소, 포트, 방화벽 상태를 확인하세요.", ex);
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                throw new OllamaRequestException("Ollama 서버 응답 시간이 초과되었습니다.", ex);
            }
        }

        public async Task<OllamaChatResponse> WarmUpAsync(string baseUrl, string model, CancellationToken cancellationToken)
        {
            var request = new OllamaChatRequest
            {
                Model = model,
                KeepAlive = DefaultKeepAlive,
                Stream = false,
                Options = new OllamaOptions
                {
                    Temperature = 0.2,
                    NumContext = DefaultNumContext,
                    NumPredict = WarmupNumPredict
                },
                Messages =
                [
                    new OllamaMessage { Role = "user", Content = "ok" }
                ]
            };

            return await ChatAsync(baseUrl, request, cancellationToken);
        }

        public async Task<OllamaChatResponse> ChatAsync(string baseUrl, OllamaChatRequest request, CancellationToken cancellationToken)
        {
            try
            {
                using var response = await httpClient.PostAsJsonAsync($"{baseUrl}/api/chat", request, cancellationToken);
                var raw = await response.Content.ReadAsStringAsync(cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw CreateRequestException(response.StatusCode, raw);
                }

                var chat = JsonSerializer.Deserialize<OllamaChatResponse>(raw);
                if (chat is null)
                {
                    throw new OllamaRequestException($"Ollama 응답 JSON을 해석할 수 없습니다. 원본 응답 일부: {TrimRaw(raw)}");
                }

                return chat;
            }
            catch (OllamaRequestException)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                throw new OllamaRequestException("Ollama 서버에 연결할 수 없습니다. 주소, 포트, 방화벽 상태를 확인하세요.", ex);
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                throw new OllamaRequestException("Ollama 요청 시간이 초과되었습니다.", ex);
            }
            catch (JsonException ex)
            {
                throw new OllamaRequestException($"Ollama 응답 JSON 파싱에 실패했습니다. {ex.Message}", ex);
            }
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        private static OllamaRequestException CreateRequestException(HttpStatusCode statusCode, string raw)
        {
            var message = TryReadOllamaError(raw);
            var detail = string.IsNullOrWhiteSpace(message) ? TrimRaw(raw) : message;
            if (statusCode == HttpStatusCode.NotFound || detail.Contains("model", StringComparison.OrdinalIgnoreCase))
            {
                return new OllamaRequestException($"모델명 오류 또는 모델을 찾을 수 없습니다. 모델명을 확인하세요. 상세: {detail}");
            }

            return new OllamaRequestException($"Ollama 오류가 발생했습니다. HTTP {(int)statusCode}. 상세: {detail}");
        }

        private static string TryReadOllamaError(string raw)
        {
            try
            {
                using var doc = JsonDocument.Parse(raw);
                return doc.RootElement.TryGetProperty("error", out var error) ? error.GetString() ?? "" : "";
            }
            catch
            {
                return "";
            }
        }

        private static string TrimRaw(string raw)
        {
            return raw.Length <= 500 ? raw : raw[..500] + "...";
        }
    }

    public sealed class OllamaRequestException : Exception
    {
        public OllamaRequestException(string message)
            : base(message)
        {
        }

        public OllamaRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public sealed class OllamaChatRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = "";

        [JsonPropertyName("keep_alive")]
        public string KeepAlive { get; set; } = OllamaClient.DefaultKeepAlive;

        [JsonPropertyName("stream")]
        public bool Stream { get; set; }

        [JsonPropertyName("options")]
        public OllamaOptions Options { get; set; } = new();

        [JsonPropertyName("messages")]
        public List<OllamaMessage> Messages { get; set; } = [];
    }

    public sealed class OllamaOptions
    {
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 0.2;

        [JsonPropertyName("num_ctx")]
        public int NumContext { get; set; } = OllamaClient.DefaultNumContext;

        [JsonPropertyName("num_predict")]
        public int NumPredict { get; set; } = OllamaClient.DefaultNumPredict;
    }

    public sealed class OllamaChatResponse
    {
        [JsonPropertyName("message")]
        public OllamaMessage? Message { get; set; }

        [JsonPropertyName("done")]
        public bool Done { get; set; }

        [JsonPropertyName("done_reason")]
        public string? DoneReason { get; set; }

        [JsonPropertyName("total_duration")]
        public long TotalDuration { get; set; }

        [JsonPropertyName("load_duration")]
        public long LoadDuration { get; set; }

        [JsonPropertyName("prompt_eval_duration")]
        public long PromptEvalDuration { get; set; }

        [JsonPropertyName("eval_duration")]
        public long EvalDuration { get; set; }

        [JsonPropertyName("prompt_eval_count")]
        public int PromptEvalCount { get; set; }

        [JsonPropertyName("eval_count")]
        public int EvalCount { get; set; }
    }

    public sealed class OllamaMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = "";

        [JsonPropertyName("content")]
        public string Content { get; set; } = "";

        [JsonPropertyName("images")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Images { get; set; }
    }

    public sealed class OllamaTagsResponse
    {
        [JsonPropertyName("models")]
        public List<OllamaModel> Models { get; set; } = [];
    }

    public sealed class OllamaModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
    }
}
