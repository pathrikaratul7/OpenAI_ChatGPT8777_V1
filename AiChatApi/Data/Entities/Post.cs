using Microsoft.SqlServer.Management.Smo;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiChatApi.Data.Entities;

[Table("Posts")]
public class Post
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(500)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(4000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Author { get; set; } = string.Empty;

    
    public int Upvotes { get; set; } = 0;

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Timestamp]
    public byte[] RowVersion { get; set; } = [];

    // Navigation property
    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();
}