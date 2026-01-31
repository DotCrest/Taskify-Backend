using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder
            .HasOne(t => t.Workspace)
            .WithMany(w => w.Tags)
            .HasForeignKey(t => t.WorkspaceId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
