using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.Configurations;

public class QuestConfiguration : IEntityTypeConfiguration<Quest>
{
    public void Configure(EntityTypeBuilder<Quest> builder)
    {
        builder
            .HasOne(q => q.Category)
            .WithMany(c => c.Quests)
            .HasForeignKey(q => q.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(q => q.Space)
            .WithMany(s => s.Quests)
            .HasForeignKey(q => q.SpaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(q => q.Author)
            .WithMany(u => u.CreatedQuests)
            .HasForeignKey(q => q.AuthorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasMany(q => q.Tags)
            .WithMany(t => t.Quests)
            .UsingEntity(J => J.ToTable("QuestTags"));
    }
}
