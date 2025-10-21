using BillWise.Domain.Entities.Common;
using System;
using System.Collections.Generic;

namespace BillWise.Domain.Entities
{
   
    public class Company : BaseEntity,IAuditableEntity
    {                                   
         public string Name { get; set; } = string.Empty;

        public string TaxNumber { get; set; } = string.Empty;


        public string? Address { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public bool IsActive { get; set; } = true;

        // IAuditableEntity properties
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }

        // ========== NAVIGATION PROPERTIES ==========
        // TODO: Diğer entity'ler yazıldıktan sonra açılacak

         public List<User> Users { get; set; } = new();
         public List<Customer> Customers { get; set; } = new();
         public List<Invoice> Invoices { get; set; } = new();
         public List<Product> Products { get; set; } = new();

        // ========== COMPUTED PROPERTIES ==========

        /// <summary>
        /// Şirketin görüntüleme adı
        /// </summary>
        public string DisplayName => $"{Name} (VN: {TaxNumber})";

        // ========== MINIMAL BUSINESS METHODS ==========

        /// <summary>
        /// Şirketi deaktif yapar
        /// Basit state change - complex logic Service'de
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
            SetUpdated();
        }

        /// <summary>
        /// Şirketi aktif yapar
        /// Basit state change - complex logic Service'de
        /// </summary>
        public void Activate()
        {
            IsActive = true;
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
    }
}