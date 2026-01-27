using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class WorkspaceMemberConfiguration : IEntityTypeConfiguration<WorkspaceMember>
{
    public void Configure(EntityTypeBuilder<WorkspaceMember> builder)
    {
        builder
            .HasKey(wm => new { wm.WorkspaceId, wm.UserId });

        builder
            .HasOne(um => um.Workspace)
            .WithMany(w => w.WorkspaceMembers)
            .HasForeignKey(um => um.WorkspaceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(um => um.User)
            .WithMany(u => u.WorkspaceMembers)
            .HasForeignKey(um => um.UserId);

        builder.HasIndex(wm => new { wm.UserId, wm.WorkspaceId })
            .HasFilter("[IsDeleted] = 0")
            .HasDatabaseName("IX_WorkspaceMember_User_Workspace_Active");

    }
}
