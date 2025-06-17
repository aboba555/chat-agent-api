using System.Net.Http.Headers;

namespace ChatAgent.Api.Services.OpenAI
{
    public class OpenAiService : IOpenAiService
    {
        private readonly HttpClient _http;
        private readonly string     _apiKey;
        private readonly string     _model;

        public OpenAiService(IConfiguration config, HttpClient http)
        {
            _http   = http;
            _apiKey = config["OpenAI:ApiKey"]!;
            _model  = config["OpenAI:Model"]!;
        }

        public async Task<string> GenerateAsync(string prompt)
        {
            var req = new
            {
                model    = _model,
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful Solana blockchain assistant." },
                    new { role = "user",   content = prompt }
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
            {
                Content = JsonContent.Create(req)
            };
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>();
            return payload!.choices[0].message.content;
        }

        private class ChatCompletionResponse
        {
            public Choice[] choices { get; set; } = default!;
            public class Choice { public Message message { get; set; } = default!; }
            public class Message { public string role { get; set; } = default!; public string content { get; set; } = default!; }
        }
    }
}