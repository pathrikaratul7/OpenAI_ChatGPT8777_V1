using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiChatApi.Data.Entities;

[Table("Upvotes")]
public class Upvote
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int PostId { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpvotedAt { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = [];

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
    [ForeignKey("PostId")]
    public virtual Post? Post { get; set; }
}