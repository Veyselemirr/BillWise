using Microsoft.EntityFrameworkCore;
using BillWise.Domain.Entities;
using BillWise.Infrastructure.Data.Configurations;

namespace BillWise.Infrastructure.Data
{
    /// <summary>
    /// BillWise uygulaması için Entity Framework DbContext
    /// </summary>
    public class BillWiseDbContext : DbContext
    {
        public BillWiseDbContext(DbContextOptions<BillWiseDbContext> options) : base(options)
        {
        }

        // ========== DB SETS ==========

        public DbSet<Company> Companies { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Invoice> Invoices { get; set; } = null!;
        public DbSet<InvoiceItem> InvoiceItems { get; set; } = null!;

        // ========== MODEL CONFIGURATION ==========

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations from assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BillWiseDbContext).Assembly);

            // Global Query Filters - Soft Delete için
            modelBuilder.Entity<Company>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Customer>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Product>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Invoice>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<InvoiceItem>().HasQueryFilter(e => !e.IsDeleted);
        }

        // ========== SAVE CHANGES OVERRIDE ==========
        // Audit alanlarını otomatik doldur

        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    // CreatedAt otomatik set edilir (BaseEntity'de default var)
                    // CreatedBy service layer'da set edilmeli
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                    // UpdatedBy service layer'da set edilmeli
                }
            }
        }
    }
}
