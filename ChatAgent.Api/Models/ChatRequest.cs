namespace ChatAgent.Api.Models;

public class ChatRequest
{
    public string Message { get; set; } = default!;
    public string WalletJwt { get; set; } = default!;
}