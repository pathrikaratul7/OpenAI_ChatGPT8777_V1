using Newtonsoft.Json;
using System.Collections.Generic;

namespace OpenAI_ChatGPT8777.Service.OpenAI
{
    public class CompletionRequest
    {
        [JsonProperty("model")]
        public string? Model { get; set; }

        [JsonProperty("messages")]
        public List<RequestMessage>? Messages { get; set; }

        [JsonProperty("temperature")]
        public float Temperature { get; set; }

        [JsonProperty("max_tokens")]
        public int MaxTokens { get; set; }
    }

    public class RequestMessage
    {
        [JsonProperty("role")]
        public string? Role { get; set; }

        [JsonProperty("content")]
        public string? Content { get; set; }
    }
}
