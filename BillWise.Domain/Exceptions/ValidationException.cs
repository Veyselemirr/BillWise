// 📁 BillWise.Domain/Exceptions/ValidationException.cs

using System.Collections.Generic;
using System.Linq;

namespace BillWise.Domain.Exceptions
{
    /// <summary>
    /// Veri doğrulama hatalarında fırlatılan exception
    /// Örnek: "Email boş olamaz", "Fiyat eksi olamaz", "Geçersiz telefon formatı"
    /// </summary>
    public class ValidationException : DomainException
    {
        /// <summary>
        /// Alan bazlı hata listesi
        /// Key = Alan adı (Email, Price, Name vs.)
        /// Value = O alan için hata mesajları
        /// </summary>
        public Dictionary<string, List<string>> Errors { get; }

        /// <summary>
        /// Genel doğrulama hatası oluştur
        /// Birden fazla alan hatası olacağı zaman
        /// </summary>
        public ValidationException() : base("Bir veya daha fazla doğrulama hatası oluştu.")
        {
            Errors = new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// Tek alan için doğrulama hatası oluştur
        /// En basit kullanım şekli
        /// </summary>
        /// <param name="fieldName">Hatalı alanın adı</param>
        /// <param name="errorMessage">Hata mesajı</param>
        public ValidationException(string fieldName, string errorMessage) : this()
        {
            AddError(fieldName, errorMessage);
        }

        /// <summary>
        /// Genel mesaj ve hata listesi ile doğrulama hatası oluştur
        /// FluentValidation sonuçları için kullanışlı
        /// </summary>
        /// <param name="message">Genel hata mesajı</param>
        /// <param name="errors">Hata listesi</param>
        public ValidationException(string message, List<string> errors) : base(message)
        {
            Errors = new Dictionary<string, List<string>>
            {
                ["General"] = errors
            };
        }

        /// <summary>
        /// Mevcut exception'a yeni hata ekle
        /// Birden fazla hata biriktirmek için
        /// </summary>
        /// <param name="fieldName">Hatalı alan adı</param>
        /// <param name="errorMessage">Hata mesajı</param>
        public void AddError(string fieldName, string errorMessage)
        {
            if (!Errors.ContainsKey(fieldName))
            {
                Errors[fieldName] = new List<string>();
            }
            Errors[fieldName].Add(errorMessage);
        }

        /// <summary>
        /// Tüm hata mesajlarını tek liste olarak döndür
        /// UI'da göstermek için kullanışlı
        /// </summary>
        /// <returns>Tüm hata mesajları listesi</returns>
        public List<string> GetAllErrors()
        {
            return Errors.Values.SelectMany(x => x).ToList();
            // SelectMany = İç içe listeleri tek listeye çevir
        }

        /// <summary>
        /// Belirli alan için hataları döndür
        /// Spesifik alan validation'ı için
        /// </summary>
        /// <param name="fieldName">Alan adı</param>
        /// <returns>O alan için hata mesajları</returns>
        public List<string> GetErrorsForField(string fieldName)
        {
            return Errors.ContainsKey(fieldName) ? Errors[fieldName] : new List<string>();
        }

        /// <summary>
        /// Hiç hata var mı kontrol et
        /// </summary>
        /// <returns>Hata varsa true</returns>
        public bool HasErrors()
        {
            return Errors.Any(kvp => kvp.Value.Any());
        }
    }

    /// <summary>
    /// Sık kullanılan doğrulama hataları için hazır factory
    /// Kod yazım kolaylığı ve tutarlılık için
    /// </summary>
    public static class ValidationExceptionFactory
    {
        /// <summary>
        /// "Bu alan zorunlu" hatası oluştur
        /// Kullanım: throw ValidationExceptionFactory.RequiredField("Email");
        /// </summary>
        public static ValidationException RequiredField(string fieldName)
        {
            return new ValidationException(fieldName, $"{fieldName} zorunlu alandır.");
        }

        /// <summary>
        /// "Geçersiz email" hatası oluştur
        /// Kullanım: throw ValidationExceptionFactory.InvalidEmail("test");
        /// </summary>
        public static ValidationException InvalidEmail(string email)
        {
            return new ValidationException("Email", $"'{email}' geçersiz email formatı.");
        }

        /// <summary>
        /// "Eksi değer olamaz" hatası oluştur
        /// Kullanım: throw ValidationExceptionFactory.NegativeValue("Price");
        /// </summary>
        public static ValidationException NegativeValue(string fieldName)
        {
            return new ValidationException(fieldName, $"{fieldName} eksi değer olamaz.");
        }

        /// <summary>
        /// "Çok uzun metin" hatası oluştur
        /// Kullanım: throw ValidationExceptionFactory.TooLong("Name", 50);
        /// </summary>
        public static ValidationException TooLong(string fieldName, int maxLength)
        {
            return new ValidationException(fieldName, $"{fieldName} en fazla {maxLength} karakter olabilir.");
        }

        /// <summary>
        /// "Çok kısa metin" hatası oluştur
        /// Kullanım: throw ValidationExceptionFactory.TooShort("Password", 8);
        /// </summary>
        public static ValidationException TooShort(string fieldName, int minLength)
        {
            return new ValidationException(fieldName, $"{fieldName} en az {minLength} karakter olmalıdır.");
        }

        /// <summary>
        /// "Aralık dışı değer" hatası oluştur
        /// Kullanım: throw ValidationExceptionFactory.OutOfRange("Age", 18, 65);
        /// </summary>
        public static ValidationException OutOfRange(string fieldName, object min, object max)
        {
            return new ValidationException(fieldName, $"{fieldName} {min} ile {max} arasında olmalıdır.");
        }
    }
}