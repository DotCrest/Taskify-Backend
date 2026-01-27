using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder
            .HasOne(w => w.User)
            .WithMany(u => u.OwnedWorkspaces)
            .HasForeignKey(w => w.OwnerId);
    }
}
