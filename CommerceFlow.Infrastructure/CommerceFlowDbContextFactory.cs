using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CommerceFlow.Infrastructure;

/// <summary>
/// This class is used by EF Core tools to create a DbContext instance at design time. It is not used at runtime.
/// 
/// dotnet ef migrations add InitSchema --project CommerceFlow.Infrastructure
/// </summary>
public class CommerceFlowDbContextFactory : IDesignTimeDbContextFactory<CommerceFlowDbContext>
{
    public CommerceFlowDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder =
            new DbContextOptionsBuilder<CommerceFlowDbContext>();

        optionsBuilder.UseNpgsql(
            "Server=localhost;Database=CommerceFlow;Trusted_Connection=True;TrustServerCertificate=True");

        return new CommerceFlowDbContext(null, optionsBuilder.Options);
    }
}
