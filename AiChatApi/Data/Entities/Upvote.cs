using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiChatApi.Data.Entities;

[Table("Upvotes")]
[Index(nameof(PostId), nameof(UserIdentifier), IsUnique = true)]
public class Upvote
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int PostId { get; set; }

    [Required]
    [StringLength(200)]
    public string UserIdentifier { get; set; } = string.Empty;

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime UpvotedAt { get; set; } = DateTime.UtcNow;

    [Timestamp]
    public byte[] RowVersion { get; set; } = [];

    // Navigation property
    [ForeignKey("PostId")]
    public virtual Post? Post { get; set; }
}