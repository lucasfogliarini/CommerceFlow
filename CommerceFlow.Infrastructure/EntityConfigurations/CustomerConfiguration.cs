using CommerceFlow.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceFlow.Infrastructure.EntityConfigurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable(nameof(Customer));
        builder.HasMany(c => c.Orders)
            .WithOne()
            .HasForeignKey(o => o.CustomerId);
        builder.HasMany(c => c.Addresses)
            .WithOne()
            .HasForeignKey(address => address.CustomerId);
    }
}
