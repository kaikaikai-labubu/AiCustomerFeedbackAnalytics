using FeedbackService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FeedbackService.Infrastructure.Data;

public sealed class FeedbackDbContext(DbContextOptions<FeedbackDbContext> options) : DbContext(options)
{
    public DbSet<Feedback> Feedback => Set<Feedback>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var feedback = modelBuilder.Entity<Feedback>();

        feedback.ToTable("feedback");
        feedback.HasKey(x => x.Id);
        feedback.Property(x => x.CustomerId)
            .IsRequired()
            .HasMaxLength(128);
        feedback.Property(x => x.Comment)
            .HasMaxLength(4000);
        feedback.Property(x => x.CreatedAtUtc)
            .IsRequired();
    }
}