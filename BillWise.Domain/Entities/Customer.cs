// 📁 BillWise.Domain/Entities/Customer.cs

using BillWise.Domain.Entities.Common;
using System;

namespace BillWise.Domain.Entities
{
    /// <summary>
    /// Fatura kesilen müşterileri temsil eder
    /// Her müşteri bir şirkete bağlıdır
    /// </summary>
    public class Customer : BaseEntity, IAuditableEntity
    {
        // ========== TEMEL BİLGİLER ==========

        /// <summary>
        /// Müşteri adı (kişi adı veya şirket adı)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Müşterinin vergi numarası (opsiyonel - kurumsal müşteriler için)
        /// </summary>
        public string? TaxNumber { get; set; }

        /// <summary>
        /// TC Kimlik numarası (opsiyonel - bireysel müşteriler için)
        /// </summary>
        public string? IdentityNumber { get; set; }

        // ========== İLETİŞİM BİLGİLERİ ==========

        /// <summary>
        /// Müşteri adresi
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Telefon numarası
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Email adresi
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Website (kurumsal müşteriler için)
        /// </summary>
        public string? Website { get; set; }

        // ========== İŞ BİLGİLERİ ==========

        /// <summary>
        /// Müşteri tipi - Bireysel mi Kurumsal mı?
        /// </summary>
        public CustomerType Type { get; set; } = CustomerType.Individual;

        /// <summary>
        /// Kredi limiti (TL)
        /// </summary>
        public decimal CreditLimit { get; set; } = 0;

        /// <summary>
        /// Mevcut borç (TL)
        /// </summary>
        public decimal CurrentDebt { get; set; } = 0;

        /// <summary>
        /// Müşterinin aktif olup olmadığı
        /// Pasif müşterilere fatura kesilemez
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// VIP müşteri mi?
        /// </summary>
        public bool IsVip { get; set; } = false;

        /// <summary>
        /// Hangi şirkete bağlı
        /// </summary>
        public int CompanyId { get; set; }

        // ========== AUDIT BİLGİLERİ ==========
        // IAuditableEntity'den gelen zorunlu özellikler

        /// <summary>
        /// Bu müşteriyi sisteme ekleyen kullanıcı
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Bu müşteriyi son güncelleyen kullanıcı
        /// </summary>
        public string? UpdatedBy { get; set; }

        // ========== NAVİGASYON ÖZELLİKLERİ ==========
        // TODO: Diğer entity'ler yazıldıktan sonra açılacak

        /// <summary>
        /// Müşterinin bağlı olduğu şirket
        /// </summary>
        // public Company Company { get; set; }

        /// <summary>
        /// Bu müşteriye kesilen faturalar
        /// </summary>
        // public List<Invoice> Invoices { get; set; } = new();

        // ========== COMPUTED PROPERTIES ==========

        /// <summary>
        /// Müşterinin görüntüleme adı
        /// UI'da gösterim için kullanılır
        /// </summary>
        public string DisplayName => 
            !string.IsNullOrEmpty(Phone) ? $"{Name} ({Phone})" : Name;

        /// <summary>
        /// Müşteri numarası (ID'ye dayalı)
        /// </summary>
        public string CustomerNumber => $"MST-{Id:D6}"; // MST-000001

        /// <summary>
        /// Kalan kredi tutarı
        /// </summary>
        public decimal AvailableCredit => CreditLimit - CurrentDebt;

        /// <summary>
        /// Kredi limitini aşmış mı?
        /// </summary>
        public bool IsOverCreditLimit => CurrentDebt > CreditLimit;

        /// <summary>
        /// Borcu var mı?
        /// </summary>
        public bool HasDebt => CurrentDebt > 0;

        /// <summary>
        /// Kurumsal müşteri mi?
        /// </summary>
        public bool IsCorporate => Type == CustomerType.Corporate;

        /// <summary>
        /// Bireysel müşteri mi?
        /// </summary>
        public bool IsIndividual => Type == CustomerType.Individual;

        /// <summary>
        /// İletişim bilgileri tam mı?
        /// </summary>
        public bool HasCompleteContactInfo => 
            !string.IsNullOrEmpty(Phone) || !string.IsNullOrEmpty(Email);

        // ========== BASİT BUSINESS METHODS ==========

        /// <summary>
        /// Müşteriyi deaktif yapar
        /// Pasif müşterilere fatura kesilemez
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
            SetUpdated();
        }

        /// <summary>
        /// Müşteriyi aktif yapar
        /// </summary>
        public void Activate()
        {
            IsActive = true;
            SetUpdated();
        }

        /// <summary>
        /// VIP statüsü verir
        /// </summary>
        public void MarkAsVip()
        {
            IsVip = true;
            SetUpdated();
        }

        /// <summary>
        /// VIP statüsünü kaldırır
        /// </summary>
        public void RemoveVipStatus()
        {
            IsVip = false;
            SetUpdated();
        }

        /// <summary>
        /// Kredi limitini günceller
        /// </summary>
        /// <param name="newLimit">Yeni kredi limiti</param>
        public void UpdateCreditLimit(decimal newLimit)
        {
            CreditLimit = newLimit;
            SetUpdated();
        }

        /// <summary>
        /// Borç ekler
        /// </summary>
        /// <param name="amount">Eklenecek borç miktarı</param>
        public void AddDebt(decimal amount)
        {
            if (amount > 0)
            {
                CurrentDebt += amount;
                SetUpdated();
            }
        }

        /// <summary>
        /// Borç ödemesi yapar
        /// </summary>
        /// <param name="amount">Ödenecek miktar</param>
        public void PayDebt(decimal amount)
        {
            if (amount > 0 && amount <= CurrentDebt)
            {
                CurrentDebt -= amount;
                SetUpdated();
            }
        }

        /// <summary>
        /// İletişim bilgilerini günceller
        /// </summary>
        /// <param name="phone">Yeni telefon</param>
        /// <param name="email">Yeni email</param>
        /// <param name="address">Yeni adres</param>
        public void UpdateContactInfo(string? phone, string? email, string? address)
        {
            Phone = phone?.Trim();
            Email = email?.Trim();
            Address = address?.Trim();
            SetUpdated();
        }

        /// <summary>
        /// Audit bilgilerini günceller
        /// </summary>
        /// <param name="updatedBy">Güncelleyen kullanıcı</param>
        public void SetUpdatedBy(string updatedBy)
        {
            UpdatedBy = updatedBy;
            SetUpdated();
        }

        // ========== HELPER METHODS ==========

        /// <summary>
        /// Belirtilen miktarda kredi kullanılabilir mi kontrol eder
        /// </summary>
        /// <param name="amount">Kontrol edilecek miktar</param>
        /// <returns>Kredi yeterliyse true</returns>
        public bool CanUseCredit(decimal amount)
        {
            return IsActive && !IsDeleted && (CurrentDebt + amount) <= CreditLimit;
        }

        /// <summary>
        /// Fatura kesilebilir durumda mı kontrol eder
        /// </summary>
        /// <returns>Fatura kesilebilirse true</returns>
        public bool CanCreateInvoice()
        {
            return IsActive && !IsDeleted;
        }

        /// <summary>
        /// Email geçerli format mı kontrol eder
        /// </summary>
        /// <returns>Email geçerliyse true</returns>
        public bool HasValidEmail()
        {
            return !string.IsNullOrEmpty(Email) && Email.Contains("@");
        }

        /// <summary>
        /// Telefon numarası geçerli mi kontrol eder (basit kontrol)
        /// </summary>
        /// <returns>Telefon geçerliyse true</returns>
        public bool HasValidPhone()
        {
            return !string.IsNullOrEmpty(Phone) && Phone.Length >= 10;
        }
    }

    /// <summary>
    /// Müşteri tiplerini belirtir
    /// </summary>
    public enum CustomerType
    {
        /// <summary>
        /// Bireysel müşteri - TC kimlik numarası ile
        /// </summary>
        Individual = 1,

        /// <summary>
        /// Kurumsal müşteri - Vergi numarası ile
        /// </summary>
        Corporate = 2
    }
}