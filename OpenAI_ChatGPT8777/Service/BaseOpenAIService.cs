using System.Net.Http.Headers;
using OpenAI_ChatGPT8777.NewConfig;

namespace OpenAI_ChatGPT8777.Service
{
    public class BaseOpenAIService
    {
        protected readonly string _apiKey;
        protected readonly HttpClient _httpClient;

        public BaseOpenAIService(HttpClient httpClient)
        {
            this._apiKey = OpenAIConfig.AuthSecret;
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.BaseAddress = new System.Uri(OpenAIConfig.BaseAddress);
        }
    }
}
