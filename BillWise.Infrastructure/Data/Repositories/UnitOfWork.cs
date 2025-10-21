using Microsoft.EntityFrameworkCore.Storage;
using BillWise.Domain.Interfaces;
using BillWise.Domain.Interfaces.Repositories;

namespace BillWise.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Unit of Work pattern implementasyonu
    /// Tüm repository'leri koordine eder ve transaction yönetimini sağlar
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BillWiseDbContext _context;
        private IDbContextTransaction? _currentTransaction;

        // Repository instances - lazy initialization
        private ICompanyRepository? _companies;

        // TODO: Diğer repository'ler eklenecek
        // private IUserRepository? _users;
        // private ICustomerRepository? _customers;
        // private IProductRepository? _products;
        // private IInvoiceRepository? _invoices;

        public UnitOfWork(BillWiseDbContext context)
        {
            _context = context;
        }

        // ========== REPOSITORY PROPERTIES ==========

        public ICompanyRepository Companies
        {
            get
            {
                // Lazy initialization - sadece kullanıldığında oluştur
                _companies ??= new CompanyRepository(_context);
                return _companies;
            }
        }

        // TODO: Diğer repository property'leri
        // public IUserRepository Users => _users ??= new UserRepository(_context);
        // public ICustomerRepository Customers => _customers ??= new CustomerRepository(_context);
        // public IProductRepository Products => _products ??= new ProductRepository(_context);
        // public IInvoiceRepository Invoices => _invoices ??= new InvoiceRepository(_context);

        // ========== TRANSACTION METHODS ==========

        public bool HasActiveTransaction => _currentTransaction != null;

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                throw new InvalidOperationException("Zaten aktif bir transaction var.");
            }

            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_currentTransaction == null)
            {
                throw new InvalidOperationException("Commit edilecek bir transaction yok.");
            }

            try
            {
                await _context.SaveChangesAsync();
                await _currentTransaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_currentTransaction == null)
            {
                throw new InvalidOperationException("Rollback edilecek bir transaction yok.");
            }

            try
            {
                await _currentTransaction.RollbackAsync();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        // ========== DISPOSE PATTERN ==========

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _currentTransaction?.Dispose();
                _context.Dispose();
            }
        }
    }
}
