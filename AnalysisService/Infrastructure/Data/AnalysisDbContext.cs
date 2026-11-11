using Microsoft.EntityFrameworkCore;
using AnalysisService.Domain.Entities;

namespace AnalysisService.Infrastructure.Data
{
    public sealed class AnalysisDbContext(DbContextOptions<AnalysisDbContext> options) : DbContext(options)
    {
        public DbSet<FeedbackAnalysis> FeedbackAnalysis => Set<FeedbackAnalysis>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var feedback = modelBuilder.Entity<FeedbackAnalysis>();

            feedback.ToTable("analysis");
            feedback.HasKey(x => x.Id);
            feedback.Property(x => x.FeedbackId)
                .IsRequired();
            feedback.Property(x => x.Sentiment)
                .IsRequired()
                .HasMaxLength(128);
            feedback.Property(x => x.Summary)
                .IsRequired()
                .HasMaxLength(4000);
            feedback.Property(x => x.Confidence)
                .IsRequired();
            feedback.Property(x => x.PrimaryTopic)
                .IsRequired()
                .HasMaxLength(256);
            feedback.Property(x => x.Reason)
                .IsRequired()
                .HasMaxLength(512);
            feedback.Property(x => x.Priority)
                .IsRequired();
            feedback.Property(x => x.CreatedAtUtc)
                .IsRequired();
        }
    }
}
