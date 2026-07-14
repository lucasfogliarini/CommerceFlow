using CommerceFlow.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceFlow.Infrastructure.EntityConfigurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Number).IsUnique();
        builder.ComplexProperty(e => e.Payment);
        builder.ComplexProperty(e => e.Shipment);
    }
}
