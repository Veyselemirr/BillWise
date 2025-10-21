using System;

namespace BillWise.Domain.Entities.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public void MarkAsDeleted()
        {
            IsDeleted = true;
            SetUpdated();
        }
        public void RestoreFromDeleted()
        {
            IsDeleted = false;
            SetUpdated();
        }
        public void SetUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}