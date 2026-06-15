using CommerceFlow.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceFlow.Infrastructure.EntityConfigurations;

public class TrackingConfiguration : IEntityTypeConfiguration<Tracking>
{
    public void Configure(EntityTypeBuilder<Tracking> builder)
    {
        builder.HasKey(s => s.Id);
    }
}
