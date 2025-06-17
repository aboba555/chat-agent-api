namespace ChatAgent.Api.Services.OpenAI;

public interface IOpenAiService
{
    Task<string> GenerateAsync(string prompt);
}