using AiChatApi.Data;
using AiChatApi.Data.Entities;
using AiChatApi.Models;

namespace AiChatApi.Services;

public class TeachBackService : ITeachBackService
{
    private readonly IAiService _aiService;
    private readonly TeachWallDbContext _context;
    private readonly ITokenService _tokenService;

    public TeachBackService(IAiService aiService, TeachWallDbContext context, ITokenService tokenService)
    {
        _aiService = aiService;
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<TeachBackResponse> EvaluateExplanationAsync(int userId, string topic, string explanation)
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

            // Some models wrap "JSON-only" output in markdown code fences anyway — strip them defensively.
            var cleaned = aiResponse.Trim();
            if (cleaned.StartsWith("```"))
            {
                var firstNewline = cleaned.IndexOf('\n');
                if (firstNewline >= 0)
                {
                    cleaned = cleaned[(firstNewline + 1)..];
                }
                var lastFence = cleaned.LastIndexOf("```", StringComparison.Ordinal);
                if (lastFence >= 0)
                {
                    cleaned = cleaned[..lastFence];
                }
                cleaned = cleaned.Trim();
            }

            // AI responses commonly use camelCase (scoreOutOf10) while our C# model
            // uses PascalCase (ScoreOutOf10) — without this option, Deserialize
            // silently returns an object with every property at its default (0/null)
            // instead of throwing, which is why scores were saving as 0.
            var jsonOptions = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var evaluation = System.Text.Json.JsonSerializer.Deserialize<TeachBackResponse>(cleaned, jsonOptions);

            // Guard against both a null result AND a "successfully" deserialized
            // but empty/default result (e.g. field names still didn't match).
            var isEmptyResult = evaluation == null
                || (evaluation.ScoreOutOf10 == 0
                    && evaluation.StarRating == 0
                    && string.IsNullOrWhiteSpace(evaluation.Feedback));

            if (isEmptyResult)
            {
                evaluation = new TeachBackResponse
                {
                    Score = 5,
                    ScoreOutOf10 = 5,
                    StarRating = 2,
                    Level = "Fair",
                    Feedback = string.IsNullOrWhiteSpace(aiResponse)
                        ? "The evaluator returned an unreadable response. Defaulted to a fair rating."
                        : aiResponse
                };
            }
            else
            {
                evaluation!.Score = evaluation.ScoreOutOf10 * 10;
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

            // Deduct tokens for successful evaluation
            var combinedContent = $"{topic} {explanation}";
            var (success, message, remainingTokens) = await _tokenService.DeductTokensAsync(
                userId, 
                combinedContent, 
                "EvaluationCompleted");

            if (!success)
            {
                // Log the failed token deduction but don't fail the evaluation
                System.Diagnostics.Debug.WriteLine($"Token deduction failed for user {userId}: {message}");
            }

            return evaluation;
        }
        catch (Exception ex)
        {
            var errorEvaluation = new TeachBackResponse
            {
                Score = 0,
                ScoreOutOf10 = 0,
                StarRating = 0,
                Level = "Error",
                Feedback = $"Error evaluating explanation: {ex.Message}"
            };

            // Persist error evaluations too, so failures are visible in the DB
            // instead of only existing in the API response.
            var dbEvaluation = new TeachBackEvaluation
            {
                Topic = topic,
                StudentExplanation = explanation,
                Score = errorEvaluation.Score,
                ScoreOutOf10 = errorEvaluation.ScoreOutOf10,
                StarRating = errorEvaluation.StarRating,
                Level = errorEvaluation.Level,
                Feedback = errorEvaluation.Feedback,
                EvaluatedAt = DateTime.UtcNow
            };
            _context.TeachBackEvaluations.Add(dbEvaluation);
            await _context.SaveChangesAsync();

            return errorEvaluation;
        }
    }
}