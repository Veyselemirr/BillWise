
using System;

namespace BillWise.Domain.Entities.Common
{
    /// <summary>
    /// Tüm entity'lerin miras alacağı temel sınıf
    /// Her iş nesnesinin ortak özelliklerini içerir
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Benzersiz kimlik numarası - Her kaydın farklı ID'si var
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Kayıt oluşturulma tarihi - Otomatik olarak şimdiki zaman atanır
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Son güncellenme tarihi - Kayıt değiştirildiğinde güncellenir
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Silinip silinmediği bilgisi - Gerçekten silmek yerine işaretleriz
        /// Bu "Soft Delete" denen yöntem
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        // ========== İŞ METHODLARı (Business Methods) ==========

        /// <summary>
        /// Kaydı "silinmiş" olarak işaretler
        /// Gerçekten veritabanından silmez, sadece gizler
        /// </summary>
        public void MarkAsDeleted()
        {
            IsDeleted = true;
            SetUpdated();
        }

        /// <summary>
        /// Silme işaretini kaldırır - Kaydı geri getirir
        /// </summary>
        public void RestoreFromDeleted()
        {
            IsDeleted = false;
            SetUpdated();
        }

        /// <summary>
        /// Güncellenme tarihini şimdiki zamana ayarlar
        /// Her değişiklikten sonra çağrılır
        /// </summary>
        public void SetUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Kaydın aktif (silinmemiş) olup olmadığını kontrol eder
        /// </summary>
        public bool IsActive => !IsDeleted;
    }
}