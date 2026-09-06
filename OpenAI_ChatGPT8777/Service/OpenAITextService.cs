using Newtonsoft.Json;
using OpenAI_ChatGPT8777.Interfaces;
using OpenAI_ChatGPT8777.Service.OpenAI;
using System.Text;

namespace OpenAI_ChatGPT8777.Service
{
    public class OpenAITextService : IOpenAITextService
    {
        private readonly HttpClient _httpClient;
        private const int MaxRetries = 3;
        private const int InitialDelayMs = 2000;

        public OpenAITextService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") 
                ?? Environment.GetEnvironmentVariable("openAiSecret")
                ?? throw new NullReferenceException("OpenAI API key not found");
            
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
        }

        public async Task<CompletionResponse> CompletePrompt(CompletionRequest request)
        {
            for (int attempt = 0; attempt < MaxRetries; attempt++)
            {
                try
                {
                    string json = JsonConvert.SerializeObject(request);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    
                    HttpResponseMessage response = await _httpClient.PostAsync("chat/completions", content);
                    
                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        if (attempt < MaxRetries - 1)
                        {
                            int delayMs = InitialDelayMs * (int)Math.Pow(2, attempt);
                            System.Diagnostics.Debug.WriteLine($"Rate limited. Retrying in {delayMs}ms (Attempt {attempt + 1}/{MaxRetries})");
                            await Task.Delay(delayMs);
                            continue;
                        }
                    }
                    
                    response.EnsureSuccessStatusCode();
                    string responseContent = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<CompletionResponse>(responseContent);
                }
                catch (HttpRequestException ex)
                {
                    if (attempt == MaxRetries - 1)
                    {
                        throw;
                    }

                    if (ex.Message.Contains("429"))
                    {
                        int delayMs = InitialDelayMs * (int)Math.Pow(2, attempt);
                        System.Diagnostics.Debug.WriteLine($"Rate limited. Retrying in {delayMs}ms (Attempt {attempt + 1}/{MaxRetries})");
                        await Task.Delay(delayMs);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            throw new HttpRequestException("Request failed after maximum retries");
        }
    }
}