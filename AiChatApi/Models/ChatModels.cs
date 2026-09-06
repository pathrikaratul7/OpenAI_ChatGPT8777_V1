using System.Text.Json.Serialization;

namespace AiChatApi.Models;

public class ChatRequestDto
{
    public string Message { get; set; } = string.Empty;
}

public class ChatResponseDto
{
    public string Reply { get; set; } = string.Empty;
}

// --- Models used internally to talk to Groq's OpenAI-compatible API ---
// Groq expects lowercase JSON keys (model, messages, role, content), so we
// pin the exact wire names with JsonPropertyName rather than relying on
// C# property casing.

public class GroqRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("messages")]
    public List<GroqMessage> Messages { get; set; } = new();
}

public class GroqMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

public class GroqResponse
{
    [JsonPropertyName("choices")]
    public List<GroqChoice> Choices { get; set; } = new();
}

public class GroqChoice
{
    [JsonPropertyName("message")]
    public GroqMessage Message { get; set; } = new();
}
