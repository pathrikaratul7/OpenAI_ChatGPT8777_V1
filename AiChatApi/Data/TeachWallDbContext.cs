using AiChatApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiChatApi.Data;

public class TeachWallDbContext : DbContext
{
    public TeachWallDbContext(DbContextOptions<TeachWallDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<TeachBackEvaluation> TeachBackEvaluations { get; set; }
    public DbSet<Upvote> Upvotes { get; set; }
    public DbSet<TokenUsageLog> TokenUsageLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Entity Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.Username).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.AvailableTokens).HasDefaultValue(25000);
            entity.Property(e => e.UsedTokens).HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();
        });

        // Post Entity Configuration
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(e => e.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
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
            entity.HasOne(e => e.User)
                .WithMany(u => u.Answers)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction); // Changed from Cascade to NoAction to avoid cascade cycles
            entity.HasOne(e => e.Post)
                .WithMany(p => p.Answers)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);
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
            entity.HasOne(e => e.User)
                .WithMany(u => u.Upvotes)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction); // Changed from Cascade to NoAction to avoid cascade cycles
            entity.HasOne(e => e.Post)
                .WithMany()
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // TokenUsageLog Entity Configuration
        modelBuilder.Entity<TokenUsageLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne(e => e.User)
                .WithMany(u => u.TokenUsageLogs)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction); // Changed from Cascade to NoAction to avoid cascade cycles
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}