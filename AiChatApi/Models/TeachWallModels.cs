using System.Text.Json.Serialization;

namespace AiChatApi.Models;

/// <summary>
/// Request model for posting a doubt
/// </summary>
public class PostDoubtRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
}

/// <summary>
/// Response model for a posted doubt
/// </summary>
public class PostDoubtResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int Upvotes { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Request model for posting an answer
/// </summary>
public class PostAnswerRequest
{
    public int PostId { get; set; }
    public string Answer { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
}

/// <summary>
/// Response model for a posted answer
/// </summary>
public class PostAnswerResponse
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string Answer { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Request model for TeachBack scoring
/// </summary>
public class TeachBackRequest
{
    public string Topic { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

/// <summary>
/// Response model for TeachBack scoring
/// </summary>
public class TeachBackResponse
{
    public double Score { get; set; }
    public double ScoreOutOf10 { get; set; }
    public int StarRating { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Feedback { get; set; } = string.Empty;
}

/// <summary>
/// Request model for upvoting a post
/// </summary>
public class UpvoteRequest
{
    public int PostId { get; set; }
}

/// <summary>
/// Response model for upvote
/// </summary>
public class UpvoteResponse
{
    public int PostId { get; set; }
    public int TotalUpvotes { get; set; }
}