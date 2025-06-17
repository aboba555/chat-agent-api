using ChatAgent.Api.Models;

namespace ChatAgent.Api.Services.Solana
{
    public class SolanaAgentService : ISolanaAgentService
    {
        private readonly HttpClient _http;

        public SolanaAgentService(HttpClient http)
        {
            _http = http;
        }

        public async Task<string> ExecuteAsync(string walletAddress, string command)
        {
            var payload = new
            {
                walletAddress,
                command
            };
            
            var response = await _http.PostAsJsonAsync("/agent", payload);
            
            response.EnsureSuccessStatusCode();
            
            var data = await response.Content.ReadFromJsonAsync<AgentResponse>();

            if (data == null)
                throw new Exception("Empty response from Solana agent");

            return data.Result;
        }
    }
}