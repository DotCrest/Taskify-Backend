using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class SpaceConfiguration : IEntityTypeConfiguration<Space>
{
    public void Configure(EntityTypeBuilder<Space> builder)
    {
        builder
            .HasOne(s => s.Workspace)
            .WithMany(w => w.Spaces)
            .HasForeignKey(s => s.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
