using Newtonsoft.Json;

namespace OpenAI_ChatGPT8777.Service.OpenAI
{
    public class Choice
    {
        [JsonProperty("message")]
        public ResponseMessage? Message { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("finish_reason")]
        public string? FinishReason { get; set; }
    }

    public class ResponseMessage
    {
        [JsonProperty("role")]
        public string? Role { get; set; }

        [JsonProperty("content")]
        public string? Content { get; set; }
    }
}
