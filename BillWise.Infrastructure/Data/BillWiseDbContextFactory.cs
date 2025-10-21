using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BillWise.Infrastructure.Data
{
    /// <summary>
    /// Design-time DbContext factory for EF Core migrations
    /// </summary>
    public class BillWiseDbContextFactory : IDesignTimeDbContextFactory<BillWiseDbContext>
    {
        public BillWiseDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BillWiseDbContext>();

            // Connection string for migrations
            optionsBuilder.UseNpgsql("Host=localhost;Database=billwisedb;Username=veyselemiryurtseven");

            return new BillWiseDbContext(optionsBuilder.Options);
        }
    }
}
