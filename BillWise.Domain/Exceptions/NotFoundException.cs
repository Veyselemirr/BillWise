// 📁 BillWise.Domain/Exceptions/NotFoundException.cs

namespace BillWise.Domain.Exceptions
{
    /// <summary>
    /// Aranılan şey bulunamadığında fırlatılan exception
    /// Örnek: "123 ID'li müşteri bulunamadı", "Bu emaile sahip kullanıcı yok"
    /// </summary>
    public class NotFoundException : DomainException
    {
        /// <summary>
        /// Genel "bulunamadı" hatası oluştur
        /// </summary>
        /// <param name="entityName">Aranan şeyin adı (Müşteri, Fatura vs.)</param>
        /// <param name="key">Arama anahtarı (ID, email vs.)</param>
        public NotFoundException(string entityName, object key)
            : base($"{entityName} with key '{key}' was not found.")
        {
            // $"{entityName}..." = String interpolation (string birleştirme)
            // object key = hem int hem string hem her türlü veri olabilir
        }

        /// <summary>
        /// Özel mesaj ile "bulunamadı" hatası oluştur
        /// Daha spesifik durumlar için
        /// </summary>
        /// <param name="customMessage">Özel hata mesajı</param>
        public NotFoundException(string customMessage) : base(customMessage)
        {
        }
    }

    /// <summary>
    /// Önceden hazırlanmış exception'lar için fabrika
    /// Daha kolay ve tutarlı exception oluşturmak için
    /// </summary>
    public static class NotFoundExceptionFactory
    {
        /// <summary>
        /// Müşteri bulunamadı exception'ı oluştur
        /// Kullanım: throw NotFoundExceptionFactory.Customer(123);
        /// </summary>
        public static NotFoundException Customer(int customerId)
        {
            return new NotFoundException("Customer", customerId);
        }

        /// <summary>
        /// Fatura bulunamadı exception'ı oluştur
        /// Kullanım: throw NotFoundExceptionFactory.Invoice(456);
        /// </summary>
        public static NotFoundException Invoice(int invoiceId)
        {
            return new NotFoundException("Invoice", invoiceId);
        }

        /// <summary>
        /// Ürün bulunamadı exception'ı oluştur
        /// Kullanım: throw NotFoundExceptionFactory.Product(789);
        /// </summary>
        public static NotFoundException Product(int productId)
        {
            return new NotFoundException("Product", productId);
        }

        /// <summary>
        /// Şirket bulunamadı exception'ı oluştur
        /// Kullanım: throw NotFoundExceptionFactory.Company(321);
        /// </summary>
        public static NotFoundException Company(int companyId)
        {
            return new NotFoundException("Company", companyId);
        }

        /// <summary>
        /// Email ile kullanıcı bulunamadı exception'ı oluştur
        /// Kullanım: throw NotFoundExceptionFactory.UserByEmail("test@test.com");
        /// </summary>
        public static NotFoundException UserByEmail(string email)
        {
            return new NotFoundException("User", email);
        }

        /// <summary>
        /// Fatura numarası ile fatura bulunamadı exception'ı oluştur
        /// Kullanım: throw NotFoundExceptionFactory.InvoiceByNumber("INV-2024-001");
        /// </summary>
        public static NotFoundException InvoiceByNumber(string invoiceNumber)
        {
            return new NotFoundException("Invoice", invoiceNumber);
        }
    }
}