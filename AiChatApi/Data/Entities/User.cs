using AiChatApi.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiChatApi.Data.Entities;

[Table("Users")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(256)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(256)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public long AvailableTokens { get; set; } = 25000;

    [Required]
    public long UsedTokens { get; set; } = 0;

    [Required]
    public UserRole Role { get; set; } = UserRole.User;

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedAt { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = [];

    // Navigation properties
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();
    public virtual ICollection<Upvote> Upvotes { get; set; } = new List<Upvote>();
    public virtual ICollection<TokenUsageLog> TokenUsageLogs { get; set; } = new List<TokenUsageLog>();
}