using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AiChatApi.Models;

namespace AiChatApi.Services;

public class GroqAiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GroqAiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> GetAiResponseAsync(string userMessage)
    {
        var apiKey = _configuration["GroqSettings:ApiKey"];
        var baseUrl = _configuration["GroqSettings:BaseUrl"];
        var model = _configuration["GroqSettings:Model"];

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(baseUrl))
        {
            throw new InvalidOperationException("Groq API Key or BaseUrl is not configured.");
        }

        var requestBody = new GroqRequest
        {
            Model = model ?? "llama-3.3-70b-versatile",
            Messages = new List<GroqMessage>
            {
                new GroqMessage { Role = "system", Content = "You are a helpful assistant." },
                new GroqMessage { Role = "user", Content = userMessage }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, baseUrl)
        {
            Content = content
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        try
        {
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Groq API error ({response.StatusCode}): {errorBody}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var groqResponse = JsonSerializer.Deserialize<GroqResponse>(responseJson, options);

            return groqResponse?.Choices?.FirstOrDefault()?.Message?.Content
                   ?? "No response from AI.";
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"HTTP request failed: {ex.Message}", ex);
        }
    }
}
