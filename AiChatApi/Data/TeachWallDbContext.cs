using AiChatApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiChatApi.Data;

public class TeachWallDbContext : DbContext
{
    public TeachWallDbContext(DbContextOptions<TeachWallDbContext> options)
        : base(options)
    {
    }

    public DbSet<Post> Posts { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<TeachBackEvaluation> TeachBackEvaluations { get; set; }
    public DbSet<Upvote> Upvotes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Post Entity Configuration
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasMany(e => e.Answers)
                .WithOne(a => a.Post)
                .HasForeignKey(a => a.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Answer Entity Configuration
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(e => e.Post)
                .WithMany(p => p.Answers)
                .HasForeignKey(e => e.PostId);
        });

        // TeachBackEvaluation Entity Configuration
        modelBuilder.Entity<TeachBackEvaluation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EvaluatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // Upvote Entity Configuration
        modelBuilder.Entity<Upvote>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UpvotedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(e => e.Post)
                .WithMany()
                .HasForeignKey(e => e.PostId);
            entity.HasIndex(e => new { e.PostId, e.UserIdentifier }).IsUnique();
        });
    }
}