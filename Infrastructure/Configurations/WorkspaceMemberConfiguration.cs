using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class WorkspaceMemberConfiguration : IEntityTypeConfiguration<WorkspaceMember>
{
    public void Configure(EntityTypeBuilder<WorkspaceMember> builder)
    {
        builder.HasKey(wm => new { wm.WorkspaceId, wm.UserId });

        builder
            .HasOne(wm => wm.User)
            .WithMany(u => u.WorkspaceMembers)
            .HasForeignKey(wm => wm.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(wm => wm.Workspace)
            .WithMany(w => w.WorkspaceMembers)
            .HasForeignKey(wm => wm.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
