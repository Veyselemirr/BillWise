// 📁 BillWise.Domain/Entities/InvoiceItem.cs

using BillWise.Domain.Entities.Common;

namespace BillWise.Domain.Entities
{
    /// <summary>
    /// Fatura kalemlerini (faturadaki her satırı) temsil eder
    /// Her fatura kalemi bir ürün ve miktardan oluşur
    /// </summary>
    public class InvoiceItem : BaseEntity
    {
        // ========== REFERANS BİLGİLERİ ==========

        /// <summary>
        /// Hangi faturaya ait olduğu
        /// </summary>
        public int InvoiceId { get; set; }

        /// <summary>
        /// Hangi ürünün satıldığı
        /// </summary>
        public int ProductId { get; set; }

        // ========== ÜRÜN BİLGİLERİ (Snapshot) ==========
        // Ürün bilgileri fatura anındaki haliyle saklanır
        // Çünkü ürün bilgileri değişse bile faturadaki bilgiler sabit kalmalı

        /// <summary>
        /// Ürün adı (fatura anındaki hali)
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Ürün kodu (fatura anındaki hali)
        /// </summary>
        public string? ProductCode { get; set; }

        /// <summary>
        /// Ürün açıklaması (fatura anındaki hali)
        /// </summary>
        public string? ProductDescription { get; set; }

        // ========== MİKTAR VE FİYAT BİLGİLERİ ==========

        /// <summary>
        /// Satılan miktar
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Birim (Adet, KG, M2, Saat, vs.)
        /// </summary>
        public string Unit { get; set; } = "Adet";

        /// <summary>
        /// Birim fiyat (KDV hariç)
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// İndirim oranı (%)
        /// </summary>
        public decimal DiscountRate { get; set; } = 0;

        /// <summary>
        /// İndirim tutarı (TL)
        /// </summary>
        public decimal DiscountAmount { get; set; } = 0;

        /// <summary>
        /// KDV oranı (%)
        /// </summary>
        public decimal TaxRate { get; set; } = 20;

        // ========== COMPUTED PROPERTIES (Hesaplanan Değerler) ==========

        /// <summary>
        /// Ara toplam (Miktar × Birim Fiyat)
        /// </summary>
        public decimal SubTotal => Quantity * UnitPrice;

        /// <summary>
        /// İndirim sonrası tutar
        /// </summary>
        public decimal AmountAfterDiscount => SubTotal - DiscountAmount;

        /// <summary>
        /// KDV tutarı
        /// </summary>
        public decimal TaxAmount => AmountAfterDiscount * (TaxRate / 100);

        /// <summary>
        /// Toplam tutar (KDV dahil)
        /// </summary>
        public decimal TotalAmount => AmountAfterDiscount + TaxAmount;

        /// <summary>
        /// Etkili birim fiyat (indirim sonrası)
        /// </summary>
        public decimal EffectiveUnitPrice => 
            Quantity > 0 ? AmountAfterDiscount / Quantity : 0;

        // ========== NAVİGASYON ÖZELLİKLERİ ==========

        /// <summary>
        /// Bu kalemin ait olduğu fatura
        /// </summary>
        public Invoice Invoice { get; set; } = null!;

        /// <summary>
        /// Bu kalemde satılan ürün
        /// </summary>
        public Product Product { get; set; } = null!;

        // ========== BASİT BUSINESS METHODS ==========

        /// <summary>
        /// Miktarı günceller ve hesaplamaları yeniden yapar
        /// </summary>
        /// <param name="newQuantity">Yeni miktar</param>
        public void UpdateQuantity(decimal newQuantity)
        {
            if (newQuantity <= 0)
                return; // Hata handling Service katmanında yapılacak

            Quantity = newQuantity;
            RecalculateDiscountAmount();
            SetUpdated();
        }

        /// <summary>
        /// Birim fiyatı günceller
        /// </summary>
        /// <param name="newUnitPrice">Yeni birim fiyat</param>
        public void UpdateUnitPrice(decimal newUnitPrice)
        {
            if (newUnitPrice < 0)
                return; // Hata handling Service katmanında yapılacak

            UnitPrice = newUnitPrice;
            RecalculateDiscountAmount();
            SetUpdated();
        }

        /// <summary>
        /// İndirim oranını günceller
        /// </summary>
        /// <param name="newDiscountRate">Yeni indirim oranı (%)</param>
        public void UpdateDiscountRate(decimal newDiscountRate)
        {
            if (newDiscountRate < 0 || newDiscountRate > 100)
                return; // Hata handling Service katmanında yapılacak

            DiscountRate = newDiscountRate;
            RecalculateDiscountAmount();
            SetUpdated();
        }

        /// <summary>
        /// İndirim tutarını günceller
        /// </summary>
        /// <param name="newDiscountAmount">Yeni indirim tutarı</param>
        public void UpdateDiscountAmount(decimal newDiscountAmount)
        {
            if (newDiscountAmount < 0 || newDiscountAmount > SubTotal)
                return; // Hata handling Service katmanında yapılacak

            DiscountAmount = newDiscountAmount;
            // İndirim oranını yeniden hesapla
            DiscountRate = SubTotal > 0 ? (DiscountAmount / SubTotal) * 100 : 0;
            SetUpdated();
        }

        /// <summary>
        /// KDV oranını günceller
        /// </summary>
        /// <param name="newTaxRate">Yeni KDV oranı (%)</param>
        public void UpdateTaxRate(decimal newTaxRate)
        {
            if (newTaxRate < 0)
                return; // Hata handling Service katmanında yapılacak

            TaxRate = newTaxRate;
            SetUpdated();
        }

        // ========== HELPER METHODS ==========

        /// <summary>
        /// İndirim tutarını orana göre yeniden hesaplar
        /// </summary>
        private void RecalculateDiscountAmount()
        {
            DiscountAmount = SubTotal * (DiscountRate / 100);
        }

        /// <summary>
        /// Fatura kaleminin geçerli olup olmadığını kontrol eder
        /// </summary>
        /// <returns>Geçerliyse true</returns>
        public bool IsValid()
        {
            return Quantity > 0 && 
                   UnitPrice >= 0 && 
                   DiscountAmount >= 0 && 
                   DiscountAmount <= SubTotal &&
                   TaxRate >= 0 &&
                   !string.IsNullOrEmpty(ProductName);
        }

        /// <summary>
        /// İndirimli satış mı kontrol eder
        /// </summary>
        /// <returns>İndirim varsa true</returns>
        public bool HasDiscount()
        {
            return DiscountAmount > 0 || DiscountRate > 0;
        }

        /// <summary>
        /// KDV'li satış mı kontrol eder
        /// </summary>
        /// <returns>KDV varsa true</returns>
        public bool HasTax()
        {
            return TaxRate > 0;
        }
    }
}