using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Configurations;

public class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
{
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
        var InvitationStatusEnumConverter = new EnumToStringConverter<InvitationStatusEnum>();

        builder
            .HasOne(i => i.Sender)
            .WithMany(u => u.SentInvitations)
            .HasForeignKey(i => i.SenderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasOne(i => i.Space)
            .WithMany(s => s.Invitations)
            .HasForeignKey(i => i.SpaceId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(i => i.Workspace)
            .WithMany(w => w.Invitations)
            .HasForeignKey(i => i.WorkspaceId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .Property(i => i.Status)
            .HasConversion(InvitationStatusEnumConverter);
    }
}
