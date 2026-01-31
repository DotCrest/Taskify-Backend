using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class UserQuestConfiguration : IEntityTypeConfiguration<UserQuest>
{
    public void Configure(EntityTypeBuilder<UserQuest> builder)
    {
        builder.HasKey(uq => new { uq.UserId, uq.QuestId });

        builder
            .HasOne(uq => uq.User)
            .WithMany(u => u.AssignedQuests)
            .HasForeignKey(uq => uq.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(uq => uq.Quest)
            .WithMany(q => q.Assignees)
            .HasForeignKey(uq => uq.QuestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
