using OpenAI_ChatGPT8777.Service.OpenAI;

namespace OpenAI_ChatGPT8777.Interfaces
{
    public interface IOpenAITextService
    {
        Task<CompletionResponse> CompletePrompt(CompletionRequest request);
    }
}
