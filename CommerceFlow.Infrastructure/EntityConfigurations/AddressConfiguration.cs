using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceFlow.Infrastructure.EntityConfigurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasKey(address => address.Id);
        builder.Property(address => address.Street).IsRequired();
        builder.Property(address => address.Number).IsRequired();
        builder.Property(address => address.City).IsRequired();
        builder.Property(address => address.State).IsRequired();
        builder.Property(address => address.ZipCode).IsRequired();
        builder.Property(address => address.Country).IsRequired();
    }
}