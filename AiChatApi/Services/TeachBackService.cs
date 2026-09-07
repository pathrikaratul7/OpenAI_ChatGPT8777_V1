using AiChatApi.Data;
using AiChatApi.Data.Entities;
using AiChatApi.Models;

namespace AiChatApi.Services;

public class TeachBackService : ITeachBackService
{
    private readonly IAiService _aiService;
    private readonly TeachWallDbContext _context;

    public TeachBackService(IAiService aiService, TeachWallDbContext context)
    {
        _aiService = aiService;
        _context = context;
    }

    public async Task<TeachBackResponse> EvaluateExplanationAsync(string topic, string explanation)
    {
        var prompt = $@"
Evaluate the following student explanation for accuracy and completeness.
Topic: {topic}
Student Explanation: {explanation}

Provide evaluation in this exact JSON format:
{{
    ""scoreOutOf10"": <0-10>,
    ""starRating"": <0-5>,
    ""level"": ""<Excellent/Good/Fair/Needs Improvement>"",
    ""feedback"": ""<Specific feedback on what's correct and what could be improved>""
}}

Only return valid JSON, no additional text.";

        try
        {
            var aiResponse = await _aiService.GetAiResponseAsync(prompt);
            
            // Parse AI response as JSON
            var evaluation = System.Text.Json.JsonSerializer.Deserialize<TeachBackResponse>(aiResponse);
            
            if (evaluation == null)
            {
                evaluation = new TeachBackResponse
                {
                    Score = 5,
                    ScoreOutOf10 = 5,
                    StarRating = 2,
                    Level = "Fair",
                    Feedback = aiResponse
                };
            }
            else
            {
                evaluation.Score = evaluation.ScoreOutOf10 * 10;
            }

            // Save evaluation to database
            var dbEvaluation = new TeachBackEvaluation
            {
                Topic = topic,
                StudentExplanation = explanation,
                Score = evaluation.Score,
                ScoreOutOf10 = evaluation.ScoreOutOf10,
                StarRating = evaluation.StarRating,
                Level = evaluation.Level,
                Feedback = evaluation.Feedback,
                EvaluatedAt = DateTime.UtcNow
            };

            _context.TeachBackEvaluations.Add(dbEvaluation);
            await _context.SaveChangesAsync();

            return evaluation;
        }
        catch (Exception ex)
        {
            return new TeachBackResponse
            {
                Score = 0,
                ScoreOutOf10 = 0,
                StarRating = 0,
                Level = "Error",
                Feedback = $"Error evaluating explanation: {ex.Message}"
            };
        }
    }
}