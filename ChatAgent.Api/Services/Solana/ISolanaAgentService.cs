namespace ChatAgent.Api.Services.Solana;

public interface ISolanaAgentService
{
    Task<string> ExecuteAsync(string walletAddress, string command);
}