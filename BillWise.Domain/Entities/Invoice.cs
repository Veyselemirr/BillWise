// 📁 BillWise.Domain/Entities/Invoice.cs

using BillWise.Domain.Entities.Common;
using BillWise.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BillWise.Domain.Entities
{
    /// <summary>
    /// Satış faturalarını temsil eder
    /// Her fatura bir müşteriye kesilir ve birden fazla kalem içerebilir
    /// </summary>
    public class Invoice : BaseEntity, IAuditableEntity
    {
        // ========== REFERANS BİLGİLERİ ==========

        /// <summary>
        /// Hangi müşteriye kesildiği
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Hangi şirkete ait olduğu
        /// </summary>
        public int CompanyId { get; set; }

        // ========== FATURA BİLGİLERİ ==========

        /// <summary>
        /// Fatura numarası (benzersiz)
        /// Otomatik generate edilir: INV-2024-000001
        /// </summary>
        public string InvoiceNumber { get; set; } = string.Empty;

        /// <summary>
        /// Fatura tarihi
        /// </summary>
        public DateTime InvoiceDate { get; set; } = DateTime.Today;

        /// <summary>
        /// Vade tarihi (ödeme tarihi)
        /// </summary>
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(30);

        /// <summary>
        /// Fatura açıklaması / notları
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Fatura durumu
        /// </summary>
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

        // ========== MÜŞTERİ BİLGİLERİ (Snapshot) ==========
        // Müşteri bilgileri fatura anındaki haliyle saklanır
        // Çünkü müşteri bilgileri değişse bile faturadaki bilgiler sabit kalmalı

        /// <summary>
        /// Müşteri adı (fatura anındaki hali)
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Müşteri adresi (fatura anındaki hali)
        /// </summary>
        public string? CustomerAddress { get; set; }

        /// <summary>
        /// Müşteri telefonu (fatura anındaki hali)
        /// </summary>
        public string? CustomerPhone { get; set; }

        /// <summary>
        /// Müşteri email'i (fatura anındaki hali)
        /// </summary>
        public string? CustomerEmail { get; set; }

        /// <summary>
        /// Müşteri vergi numarası (fatura anındaki hali)
        /// </summary>
        public string? CustomerTaxNumber { get; set; }

        // ========== TOPLAM TUTARLAR ==========

        /// <summary>
        /// Ara toplam (KDV hariç, indirim sonrası)
        /// </summary>
        public decimal SubTotal { get; private set; } = 0;

        /// <summary>
        /// Toplam indirim tutarı
        /// </summary>
        public decimal TotalDiscount { get; private set; } = 0;

        /// <summary>
        /// Toplam KDV tutarı
        /// </summary>
        public decimal TotalTax { get; private set; } = 0;

        /// <summary>
        /// Genel toplam (KDV dahil)
        /// </summary>
        public decimal GrandTotal { get; private set; } = 0;

        // ========== ÖDEME BİLGİLERİ ==========

        /// <summary>
        /// Ödenen tutar
        /// </summary>
        public decimal PaidAmount { get; set; } = 0;

        /// <summary>
        /// Kalan tutar
        /// </summary>
        public decimal RemainingAmount => GrandTotal - PaidAmount;

        /// <summary>
        /// Ödeme yöntemi
        /// </summary>
        public PaymentMethod? PaymentMethod { get; set; }

        /// <summary>
        /// Ödeme tarihi
        /// </summary>
        public DateTime? PaymentDate { get; set; }

        // ========== AUDIT BİLGİLERİ ==========

        /// <summary>
        /// Faturayı oluşturan kullanıcı
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Faturayı son güncelleyen kullanıcı
        /// </summary>
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Faturayı gönderen kullanıcı
        /// </summary>
        public string? SentBy { get; set; }

        /// <summary>
        /// Fatura gönderilme tarihi
        /// </summary>
        public DateTime? SentAt { get; set; }

        // ========== NAVİGASYON ÖZELLİKLERİ ==========

        /// <summary>
        /// Faturanın kesildiği müşteri
        /// </summary>
        public Customer Customer { get; set; } = null!;

        /// <summary>
        /// Faturanın ait olduğu şirket
        /// </summary>
        public Company Company { get; set; } = null!;

        /// <summary>
        /// Fatura kalemleri (faturadaki ürünler)
        /// </summary>
        public List<InvoiceItem> Items { get; set; } = new();

        // ========== COMPUTED PROPERTIES ==========

        /// <summary>
        /// Fatura tamamen ödenmiş mi?
        /// </summary>
        public bool IsFullyPaid => Math.Abs(RemainingAmount) < 0.01m; // Floating point karşılaştırması

        /// <summary>
        /// Kısmi ödeme yapılmış mı?
        /// </summary>
        public bool IsPartiallyPaid => PaidAmount > 0 && !IsFullyPaid;

        /// <summary>
        /// Ödenmemiş mi?
        /// </summary>
        public bool IsUnpaid => PaidAmount == 0;

        /// <summary>
        /// Vadesi geçmiş mi?
        /// </summary>
        public bool IsOverdue => DateTime.Today > DueDate && !IsFullyPaid;

        /// <summary>
        /// Düzenlenebilir durumda mı?
        /// </summary>
        public bool CanBeEdited => Status == InvoiceStatus.Draft && !IsDeleted;

        /// <summary>
        /// Gönderilebilir durumda mı?
        /// </summary>
        public bool CanBeSent => Status == InvoiceStatus.Draft && Items.Any() && !IsDeleted;

        /// <summary>
        /// İptal edilebilir durumda mı?
        /// </summary>
        public bool CanBeCancelled => Status != InvoiceStatus.Cancelled && 
                                     Status != InvoiceStatus.Paid && !IsDeleted;

        /// <summary>
        /// Kalem sayısı
        /// </summary>
        public int ItemCount => Items.Count;

        /// <summary>
        /// Ortalama kalem tutarı
        /// </summary>
        public decimal AverageItemAmount => ItemCount > 0 ? SubTotal / ItemCount : 0;

        // ========== BASİT BUSINESS METHODS ==========

        /// <summary>
        /// Fatura numarası generate eder
        /// Format: INV-YYYY-NNNNNN
        /// </summary>
        public void GenerateInvoiceNumber()
        {
            var year = InvoiceDate.Year;
            var sequence = Id; // Gerçek projede şirket bazlı sequence olacak
            InvoiceNumber = $"INV-{year}-{sequence:D6}";
            SetUpdated();
        }

        /// <summary>
        /// Faturayı gönderildi olarak işaretler
        /// </summary>
        /// <param name="sentBy">Gönderen kullanıcı</param>
        public void MarkAsSent(string sentBy)
        {
            Status = InvoiceStatus.Sent;
            SentBy = sentBy;
            SentAt = DateTime.UtcNow;
            SetUpdated();
        }

        /// <summary>
        /// Faturayı ödenmiş olarak işaretler
        /// </summary>
        /// <param name="paymentAmount">Ödenen tutar</param>
        /// <param name="paymentMethod">Ödeme yöntemi</param>
        /// <param name="paymentDate">Ödeme tarihi</param>
        public void MarkAsPaid(decimal paymentAmount, PaymentMethod paymentMethod, DateTime? paymentDate = null)
        {
            PaidAmount = paymentAmount;
            PaymentMethod = paymentMethod;
            PaymentDate = paymentDate ?? DateTime.Today;
            
            if (IsFullyPaid)
            {
                Status = InvoiceStatus.Paid;
            }
            
            SetUpdated();
        }

        /// <summary>
        /// Faturayı iptal eder
        /// </summary>
        /// <param name="cancelledBy">İptal eden kullanıcı</param>
        public void Cancel(string cancelledBy)
        {
            Status = InvoiceStatus.Cancelled;
            UpdatedBy = cancelledBy;
            SetUpdated();
        }

        /// <summary>
        /// Fatura vadesi geçmişse durumu günceller
        /// </summary>
        public void CheckOverdueStatus()
        {
            if (IsOverdue && Status == InvoiceStatus.Sent)
            {
                Status = InvoiceStatus.Overdue;
                SetUpdated();
            }
        }

        /// <summary>
        /// Müşteri bilgilerini faturaya kopyalar (snapshot)
        /// </summary>
        /// <param name="customer">Müşteri entity'si</param>
        public void CopyCustomerInfo(Customer customer)
        {
            CustomerName = customer.Name;
            CustomerAddress = customer.Address;
            CustomerPhone = customer.Phone;
            CustomerEmail = customer.Email;
            CustomerTaxNumber = customer.TaxNumber;
            SetUpdated();
        }

        /// <summary>
        /// Fatura kalem ekler
        /// </summary>
        /// <param name="product">Ürün</param>
        /// <param name="quantity">Miktar</param>
        /// <param name="unitPrice">Birim fiyat (opsiyonel, ürün fiyatı kullanılır)</param>
        /// <param name="discountRate">İndirim oranı</param>
        public void AddItem(Product product, decimal quantity, decimal? unitPrice = null, decimal discountRate = 0)
        {
            var item = new InvoiceItem
            {
                InvoiceId = Id,
                ProductId = product.Id,
                ProductName = product.Name,
                ProductCode = product.ProductCode,
                ProductDescription = product.Description,
                Quantity = quantity,
                Unit = product.Unit,
                UnitPrice = unitPrice ?? product.UnitPrice,
                DiscountRate = discountRate,
                TaxRate = product.TaxRate
            };

            Items.Add(item);
            RecalculateTotals();
        }

        /// <summary>
        /// Fatura kalem siler
        /// </summary>
        /// <param name="itemId">Silinecek kalem ID'si</param>
        public void RemoveItem(int itemId)
        {
            var item = Items.FirstOrDefault(x => x.Id == itemId);
            if (item != null)
            {
                Items.Remove(item);
                RecalculateTotals();
            }
        }

        /// <summary>
        /// Tüm toplamları yeniden hesaplar
        /// </summary>
        public void RecalculateTotals()
        {
            TotalDiscount = Items.Sum(x => x.DiscountAmount);
            SubTotal = Items.Sum(x => x.AmountAfterDiscount);
            TotalTax = Items.Sum(x => x.TaxAmount);
            GrandTotal = Items.Sum(x => x.TotalAmount);
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
        /// Faturanın geçerli olup olmadığını kontrol eder
        /// </summary>
        /// <returns>Geçerliyse true</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(InvoiceNumber) &&
                   !string.IsNullOrEmpty(CustomerName) &&
                   Items.Any() &&
                   Items.All(x => x.IsValid()) &&
                   InvoiceDate <= DateTime.Today &&
                   DueDate >= InvoiceDate;
        }

        /// <summary>
        /// Faturada belirli ürün var mı kontrol eder
        /// </summary>
        /// <param name="productId">Ürün ID'si</param>
        /// <returns>Varsa true</returns>
        public bool HasProduct(int productId)
        {
            return Items.Any(x => x.ProductId == productId);
        }

        /// <summary>
        /// Faturanın toplam ağırlığını hesaplar (fiziksel ürünler için)
        /// </summary>
        /// <returns>Toplam ağırlık (KG)</returns>
        public decimal CalculateTotalWeight()
        {
            return Items.Where(x => x.Product?.Weight.HasValue == true)
                       .Sum(x => x.Quantity * (x.Product?.Weight ?? 0));
        }

        /// <summary>
        /// En yüksek tutarlı kalemi getirir
        /// </summary>
        /// <returns>En yüksek tutarlı kalem</returns>
        public InvoiceItem? GetHighestValueItem()
        {
            return Items.OrderByDescending(x => x.TotalAmount).FirstOrDefault();
        }

        /// <summary>
        /// Fatura durumunu string olarak döndürür
        /// </summary>
        /// <returns>Durum string'i</returns>
        public string GetStatusText()
        {
            return Status switch
            {
                InvoiceStatus.Draft => "Taslak",
                InvoiceStatus.Sent => "Gönderildi",
                InvoiceStatus.Paid => "Ödendi",
                InvoiceStatus.Overdue => "Vadesi Geçti",
                InvoiceStatus.Cancelled => "İptal Edildi",
                _ => "Bilinmiyor"
            };
        }
    }
}