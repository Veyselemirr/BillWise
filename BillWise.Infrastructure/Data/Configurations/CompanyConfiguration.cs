using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BillWise.Domain.Entities;

namespace BillWise.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Company entity için Fluent API konfigürasyonu
    /// Database şeması, index'ler, ilişkiler burada tanımlanır
    /// </summary>
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            // ========== TABLE NAME ==========
            builder.ToTable("Companies");

            // ========== PRIMARY KEY ==========
            builder.HasKey(c => c.Id);

            // ========== PROPERTIES ==========

            // Name
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            // TaxNumber - Unique constraint
            builder.Property(c => c.TaxNumber)
                .IsRequired()
                .HasMaxLength(11);

            builder.HasIndex(c => c.TaxNumber)
                .IsUnique()
                .HasDatabaseName("IX_Companies_TaxNumber");

            // Address
            builder.Property(c => c.Address)
                .HasMaxLength(500);

            // Phone
            builder.Property(c => c.Phone)
                .HasMaxLength(20);

            // Email
            builder.Property(c => c.Email)
                .HasMaxLength(100);

            // IsActive
            builder.Property(c => c.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // ========== AUDIT FIELDS ==========

            builder.Property(c => c.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Property(c => c.UpdatedAt);

            // ========== SOFT DELETE ==========

            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // Index for soft delete queries
            builder.HasIndex(c => c.IsDeleted)
                .HasDatabaseName("IX_Companies_IsDeleted");

            // ========== RELATIONSHIPS ==========

            // Company -> Users (One-to-Many)
            builder.HasMany(c => c.Users)
                .WithOne(u => u.Company)
                .HasForeignKey(u => u.CompanyID)
                .OnDelete(DeleteBehavior.Restrict); // Şirket silindiğinde kullanıcılar silinmesin

            // Company -> Customers (One-to-Many)
            // TODO: Customer navigation property comment'ten çıkınca açılacak
            // builder.HasMany(c => c.Customers)
            //     .WithOne(cu => cu.Company)
            //     .HasForeignKey(cu => cu.CompanyId)
            //     .OnDelete(DeleteBehavior.Restrict);

            // Company -> Products (One-to-Many)
            // TODO: Product navigation property comment'ten çıkınca açılacak
            // builder.HasMany(c => c.Products)
            //     .WithOne(p => p.Company)
            //     .HasForeignKey(p => p.CompanyId)
            //     .OnDelete(DeleteBehavior.Restrict);

            // Company -> Invoices (One-to-Many)
            builder.HasMany(c => c.Invoices)
                .WithOne(i => i.Company)
                .HasForeignKey(i => i.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== INDEXES FOR PERFORMANCE ==========

            // Aktif şirketleri hızlı getirmek için
            builder.HasIndex(c => c.IsActive)
                .HasDatabaseName("IX_Companies_IsActive");

            // Arama için
            builder.HasIndex(c => c.Name)
                .HasDatabaseName("IX_Companies_Name");

            // ========== COMPUTED COLUMNS (Ignore) ==========
            // DisplayName computed property, database'de saklanmaz
            builder.Ignore(c => c.DisplayName);
        }
    }
}
