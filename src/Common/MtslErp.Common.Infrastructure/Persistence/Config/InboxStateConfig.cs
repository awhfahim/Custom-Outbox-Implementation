using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MtslErp.Common.Domain.Entities;

namespace MtslErp.Common.Infrastructure.Persistence.Config;

public class InboxStateConfig : IEntityTypeConfiguration<InboxState>
{
    public void Configure(EntityTypeBuilder<InboxState> builder)
    {
        builder.ToTable("InboxStates".Underscore().ToUpper());
        builder.Property(e => e.MessageId)
            .HasMaxLength(40)
            .IsRequired();
        builder.HasKey(e => e.Id);
    }
}
