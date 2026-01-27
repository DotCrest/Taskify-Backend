using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
namespace Infrastructure.Configurations;

public class QuestConfiguration : IEntityTypeConfiguration<Quest>
{
    public void Configure(EntityTypeBuilder<Quest> builder)
    {
        var QuestStatusEnumConverter = new EnumToStringConverter<QuestStatusEnum>();
        var PriorityEnumConverter = new EnumToStringConverter<PriorityEnum>();

        builder
            .HasOne(q => q.Category)
            .WithMany(c => c.Quests)
            .HasForeignKey(q => q.CategoryId);

        builder
            .HasOne(q => q.Group)
            .WithMany(g => g.Quests)
            .HasForeignKey(q => q.GroupId);

        builder
            .HasOne(q => q.Author)
            .WithMany(u => u.CreatedQuests)
            .HasForeignKey(q => q.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(q => q.Tags)
            .WithMany(t => t.Quests)
            .UsingEntity(J => J.ToTable("QuestTags"));

        builder
          .Property(q => q.Status)
          .HasConversion(QuestStatusEnumConverter);

        builder
            .Property(q => q.Priority)
            .HasConversion(PriorityEnumConverter);
    }
}
