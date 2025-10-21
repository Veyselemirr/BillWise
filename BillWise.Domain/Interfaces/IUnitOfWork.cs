using BillWise.Domain.Interfaces.Repositories;
using System;
using System.Threading.Tasks;

namespace BillWise.Domain.Interfaces
{
    /// <summary>
    /// Unit of Work pattern interface
    /// Şimdilik sadece Company için - diğer entity'ler sonra eklenecek
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // ========== REPOSITORY PROPERTIES ==========
        
        /// <summary>
        /// Company repository'sine erişim
        /// </summary>
        ICompanyRepository Companies { get; }

        // TODO: Diğer repository'ler entity'ler yazıldıktan sonra eklenecek
        // IUserRepository Users { get; }
        // ICustomerRepository Customers { get; }
        // IProductRepository Products { get; }
        // IInvoiceRepository Invoices { get; }
        // IInvoiceItemRepository InvoiceItems { get; }

        // ========== TRANSACTION METHODS ==========

        /// <summary>
        /// Tüm değişiklikleri veritabanına kaydeder
        /// </summary>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Transaction başlatır
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// Transaction'ı commit eder
        /// </summary>
        Task CommitTransactionAsync();

        /// <summary>
        /// Transaction'ı rollback eder
        /// </summary>
        Task RollbackTransactionAsync();

        /// <summary>
        /// Aktif transaction var mı kontrol eder
        /// </summary>
        bool HasActiveTransaction { get; }
    }
}