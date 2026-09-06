using Newtonsoft.Json;
using OpenAI_ChatGPT8777.Service.ServiceModel;

namespace OpenAI_ChatGPT8777.Service.OpenAI
{
    public class CompletionResponse
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("object")]
        public string? Object { get; set; }

        [JsonProperty("created")]
        public int Created { get; set; }

        [JsonProperty("model")]
        public string? Model { get; set; }

        [JsonProperty("choices")]
        public Choice[]? Choices { get; set; }

        [JsonProperty("usage")]
        public Usage? Usage { get; set; }
    }
}
