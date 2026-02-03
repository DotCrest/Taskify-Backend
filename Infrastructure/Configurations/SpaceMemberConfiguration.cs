using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class SpaceMemberConfiguration : IEntityTypeConfiguration<SpaceMember>
{
    public void Configure(EntityTypeBuilder<SpaceMember> builder)
    {
        builder.HasKey(sm => new { sm.SpaceId, sm.UserId });

        builder
            .HasOne(sm => sm.User)
            .WithMany(u => u.SpaceMembers)
            .HasForeignKey(sm => sm.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(sm => sm.Space)
            .WithMany(s => s.SpaceMembers)
            .HasForeignKey(sm => sm.SpaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
