using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiChatApi.Data.Entities;

[Table("TeachBackEvaluations")]
public class TeachBackEvaluation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Topic { get; set; } = string.Empty;

    [Required]
    [StringLength(4000)]
    public string StudentExplanation { get; set; } = string.Empty;

    [Required]
    public double Score { get; set; }

    [Required]
    public double ScoreOutOf10 { get; set; }

    [Required]
    public int StarRating { get; set; }

    [Required]
    [StringLength(50)]
    public string Level { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Feedback { get; set; } = string.Empty;

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;

    [Timestamp]
    public byte[] RowVersion { get; set; } = [];
}