using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class SpaceMemberConfiguration : IEntityTypeConfiguration<SpaceMember>
{
    public void Configure(EntityTypeBuilder<SpaceMember> builder)
    {
        builder
            .HasKey(us => new { us.SpaceId, us.UserId });

        builder
            .HasOne(us => us.User)
            .WithMany(u => u.SpaceMembers)
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(us => us.Space)
            .WithMany(s => s.SpaceMembers)
            .HasForeignKey(us => us.SpaceId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
