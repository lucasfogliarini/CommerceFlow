using CommerceFlow.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceFlow.Infrastructure.EntityConfigurations;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.OrderId).IsRequired();

        builder.OwnsOne(s => s.ShippingAddress);

        builder.OwnsOne(s => s.Carrier, cb =>
        {
            cb.Property(c => c.Name).IsRequired();
            cb.Property(c => c.ServiceLevel).IsRequired();
        });

        builder.OwnsOne(s => s.Tracking, tb =>
        {
            tb.Property(t => t.TrackingCode).HasColumnName("TrackingCode");
            tb.OwnsMany(t => t.Events, eb =>
            {
                eb.Property<Guid>("Id");
                eb.HasKey("Id");
                eb.Property(e => e.OccurredAt).IsRequired();
                eb.Property(e => e.Description).IsRequired();
                eb.Property(e => e.Location).IsRequired();
            });
        });

        builder.HasMany(typeof(ShipmentItem))
            .WithOne()
            .Metadata.PrincipalToDependent?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
