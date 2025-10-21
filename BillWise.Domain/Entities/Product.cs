// 📁 BillWise.Domain/Entities/Product.cs

using BillWise.Domain.Entities.Common;
using System;

namespace BillWise.Domain.Entities
{
    /// <summary>
    /// Satılan ürün ve hizmetleri temsil eder
    /// Her ürün bir şirkete bağlıdır ve faturalarda kullanılır
    /// </summary>
    public class Product : BaseEntity, IAuditableEntity
    {
        // ========== TEMEL BİLGİLER ==========

        /// <summary>
        /// Ürün adı
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Ürün açıklaması
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Ürün kodu (SKU - Stock Keeping Unit)
        /// Benzersiz olmalı, stok takibi için
        /// </summary>
        public string? ProductCode { get; set; }

        /// <summary>
        /// Barkod numarası
        /// </summary>
        public string? Barcode { get; set; }

        // ========== FİYAT BİLGİLERİ ==========

        /// <summary>
        /// Birim fiyat (TL)
        /// </summary>
        public decimal UnitPrice { get; set; } = 0;

        /// <summary>
        /// Maliyet fiyatı (TL) - Kar hesabı için
        /// </summary>
        public decimal CostPrice { get; set; } = 0;

        /// <summary>
        /// KDV oranı (%) - Varsayılan %20
        /// </summary>
        public decimal TaxRate { get; set; } = 20;

        /// <summary>
        /// Para birimi - Varsayılan TL
        /// </summary>
        public string Currency { get; set; } = "TL";

        // ========== ÖLÇÜ BİLGİLERİ ==========

        /// <summary>
        /// Birim (Adet, KG, M2, Saat, vs.)
        /// </summary>
        public string Unit { get; set; } = "Adet";

        /// <summary>
        /// Ağırlık (KG)
        /// </summary>
        public decimal? Weight { get; set; }

        /// <summary>
        /// Boyutlar (Uzunluk x Genişlik x Yükseklik cm)
        /// </summary>
        public string? Dimensions { get; set; }

        // ========== STOK BİLGİLERİ ==========

        /// <summary>
        /// Stok takibi yapılıyor mu?
        /// </summary>
        public bool IsStockTracked { get; set; } = false;

        /// <summary>
        /// Mevcut stok miktarı
        /// </summary>
        public decimal StockQuantity { get; set; } = 0;

        /// <summary>
        /// Minimum stok seviyesi (uyarı için)
        /// </summary>
        public decimal MinimumStock { get; set; } = 0;

        /// <summary>
        /// Maksimum stok seviyesi
        /// </summary>
        public decimal MaximumStock { get; set; } = 0;

        // ========== KATEGORİ VE DURUM ==========

        /// <summary>
        /// Ürün kategorisi
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Ürün markası
        /// </summary>
        public string? Brand { get; set; }

        /// <summary>
        /// Ürün tipi (Fiziksel ürün mü, hizmet mi?)
        /// </summary>
        public ProductType Type { get; set; } = ProductType.Physical;

        /// <summary>
        /// Ürünün aktif olup olmadığı
        /// Pasif ürünler faturalarda kullanılamaz
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Satışa uygun mu?
        /// </summary>
        public bool IsForSale { get; set; } = true;

        /// <summary>
        /// Hangi şirkete ait
        /// </summary>
        public int CompanyId { get; set; }

        // ========== AUDIT BİLGİLERİ ==========
        // IAuditableEntity'den gelen zorunlu özellikler

        /// <summary>
        /// Bu ürünü sisteme ekleyen kullanıcı
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Bu ürünü son güncelleyen kullanıcı
        /// </summary>
        public string? UpdatedBy { get; set; }

        // ========== NAVİGASYON ÖZELLİKLERİ ==========
        // TODO: Diğer entity'ler yazıldıktan sonra açılacak

        /// <summary>
        /// Ürünün bağlı olduğu şirket
        /// </summary>
        // public Company Company { get; set; }

        /// <summary>
        /// Bu ürünün kullanıldığı fatura kalemleri
        /// </summary>
        // public List<InvoiceItem> InvoiceItems { get; set; } = new();

        // ========== COMPUTED PROPERTIES ==========

        /// <summary>
        /// Ürün numarası (ID'ye dayalı)
        /// </summary>
        public string ProductNumber => $"PRD-{Id:D6}"; // PRD-000001

        /// <summary>
        /// Görüntüleme adı (kod varsa kod ile birlikte)
        /// </summary>
        public string DisplayName => 
            !string.IsNullOrEmpty(ProductCode) ? $"{Name} ({ProductCode})" : Name;

        /// <summary>
        /// Kar marjı (%)
        /// </summary>
        public decimal ProfitMargin => 
            CostPrice > 0 ? ((UnitPrice - CostPrice) / CostPrice) * 100 : 0;

        /// <summary>
        /// Kar tutarı (TL)
        /// </summary>
        public decimal ProfitAmount => UnitPrice - CostPrice;

        /// <summary>
        /// KDV dahil fiyat
        /// </summary>
        public decimal PriceWithTax => UnitPrice * (1 + TaxRate / 100);

        /// <summary>
        /// KDV tutarı
        /// </summary>
        public decimal TaxAmount => UnitPrice * (TaxRate / 100);

        /// <summary>
        /// Stok durumu kritik mi?
        /// </summary>
        public bool IsStockCritical => 
            IsStockTracked && StockQuantity <= MinimumStock;

        /// <summary>
        /// Stokta var mı?
        /// </summary>
        public bool IsInStock => 
            !IsStockTracked || StockQuantity > 0;

        /// <summary>
        /// Fiziksel ürün mü?
        /// </summary>
        public bool IsPhysical => Type == ProductType.Physical;

        /// <summary>
        /// Hizmet mi?
        /// </summary>
        public bool IsService => Type == ProductType.Service;

        /// <summary>
        /// Satılabilir durumda mı?
        /// </summary>
        public bool CanBeSold => IsActive && IsForSale && !IsDeleted && IsInStock;

        // ========== BASİT BUSINESS METHODS ==========

        /// <summary>
        /// Ürünü deaktif yapar
        /// Pasif ürünler faturalarda kullanılamaz
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
            SetUpdated();
        }

        /// <summary>
        /// Ürünü aktif yapar
        /// </summary>
        public void Activate()
        {
            IsActive = true;
            SetUpdated();
        }

        /// <summary>
        /// Satışa kapatır
        /// </summary>
        public void StopSelling()
        {
            IsForSale = false;
            SetUpdated();
        }

        /// <summary>
        /// Satışa açar
        /// </summary>
        public void StartSelling()
        {
            IsForSale = true;
            SetUpdated();
        }

        /// <summary>
        /// Fiyat günceller
        /// </summary>
        /// <param name="newPrice">Yeni birim fiyat</param>
        public void UpdatePrice(decimal newPrice)
        {
            UnitPrice = newPrice;
            SetUpdated();
        }

        /// <summary>
        /// Maliyet fiyatını günceller
        /// </summary>
        /// <param name="newCostPrice">Yeni maliyet fiyatı</param>
        public void UpdateCostPrice(decimal newCostPrice)
        {
            CostPrice = newCostPrice;
            SetUpdated();
        }

        /// <summary>
        /// KDV oranını günceller
        /// </summary>
        /// <param name="newTaxRate">Yeni KDV oranı (%)</param>
        public void UpdateTaxRate(decimal newTaxRate)
        {
            TaxRate = newTaxRate;
            SetUpdated();
        }

        /// <summary>
        /// Stok miktarını günceller
        /// </summary>
        /// <param name="newQuantity">Yeni stok miktarı</param>
        public void UpdateStock(decimal newQuantity)
        {
            if (IsStockTracked)
            {
                StockQuantity = newQuantity;
                SetUpdated();
            }
        }

        /// <summary>
        /// Stok ekler
        /// </summary>
        /// <param name="quantity">Eklenecek miktar</param>
        public void AddStock(decimal quantity)
        {
            if (IsStockTracked && quantity > 0)
            {
                StockQuantity += quantity;
                SetUpdated();
            }
        }

        /// <summary>
        /// Stok azaltır (satış yapıldığında)
        /// </summary>
        /// <param name="quantity">Azaltılacak miktar</param>
        public void ReduceStock(decimal quantity)
        {
            if (IsStockTracked && quantity > 0 && StockQuantity >= quantity)
            {
                StockQuantity -= quantity;
                SetUpdated();
            }
        }

        /// <summary>
        /// Stok takibini başlatır
        /// </summary>
        /// <param name="initialStock">Başlangıç stok miktarı</param>
        public void EnableStockTracking(decimal initialStock = 0)
        {
            IsStockTracked = true;
            StockQuantity = initialStock;
            SetUpdated();
        }

        /// <summary>
        /// Stok takibini durdurur
        /// </summary>
        public void DisableStockTracking()
        {
            IsStockTracked = false;
            StockQuantity = 0;
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
        /// Belirtilen miktarda satılabilir mi kontrol eder
        /// </summary>
        /// <param name="requestedQuantity">İstenen miktar</param>
        /// <returns>Satılabilirse true</returns>
        public bool CanSell(decimal requestedQuantity)
        {
            if (!CanBeSold) return false;
            
            if (IsStockTracked)
                return StockQuantity >= requestedQuantity;
                
            return true; // Stok takibi yoksa sınırsız satılabilir
        }

        /// <summary>
        /// Ürün kodunun benzersiz olup olmadığını kontrol eder
        /// (Bu kontrol Service katmanında yapılacak)
        /// </summary>
        /// <returns>Ürün kodu varsa true</returns>
        public bool HasProductCode()
        {
            return !string.IsNullOrEmpty(ProductCode);
        }

        /// <summary>
        /// Fiyat bilgilerinin geçerli olup olmadığını kontrol eder
        /// </summary>
        /// <returns>Fiyatlar geçerliyse true</returns>
        public bool HasValidPricing()
        {
            return UnitPrice >= 0 && CostPrice >= 0 && TaxRate >= 0;
        }

        /// <summary>
        /// Stok seviyelerinin mantıklı olup olmadığını kontrol eder
        /// </summary>
        /// <returns>Stok seviyeleri mantıklıysa true</returns>
        public bool HasValidStockLevels()
        {
            if (!IsStockTracked) return true;
            
            return MinimumStock >= 0 && 
                   MaximumStock >= MinimumStock && 
                   StockQuantity >= 0;
        }
    }

    /// <summary>
    /// Ürün tiplerini belirtir
    /// </summary>
    public enum ProductType
    {
        /// <summary>
        /// Fiziksel ürün - Stok takibi yapılabilir
        /// Örnek: Laptop, Kitap, Gıda
        /// </summary>
        Physical = 1,

        /// <summary>
        /// Hizmet - Genelde stok takibi yapılmaz
        /// Örnek: Danışmanlık, Eğitim, Tamir
        /// </summary>
        Service = 2,

        /// <summary>
        /// Dijital ürün - Sınırsız satılabilir
        /// Örnek: Yazılım lisansı, E-kitap, Online kurs
        /// </summary>
        Digital = 3
    }
}