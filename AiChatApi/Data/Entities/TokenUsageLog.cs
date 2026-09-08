using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiChatApi.Data.Entities;

[Table("TokenUsageLogs")]
public class TokenUsageLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public long TokensUsed { get; set; }

    [Required]
    [StringLength(100)]
    public string Action { get; set; } = string.Empty; // "PostCreated", "AnswerCreated", etc.

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedAt { get; set; }

    // Navigation property
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}