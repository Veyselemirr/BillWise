using System;
using BillWise.Domain.Entities.Common;
using BillWise.Domain.Entities;
using BillWise.Domain.Enums;
namespace BillWise.Domain.Entities
{
    public class User : BaseEntity, IAuditableEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public UserRole Role { get; set; } = UserRole.Employee;
        public int CompanyID { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
        public bool IsEmailVerified { get; set; } = false;
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string DisplayName => $"{FullName} ({Email})";
        public string Initials => $"{FirstName.FirstOrDefault()}{LastName.FirstOrDefault()}".ToUpper();
        public bool IsAdmin => Role == UserRole.Admin;
        public bool IsOwner => Role == UserRole.Owner;
        public bool IsManager => Role == UserRole.Manager || Role == UserRole.Owner || Role == UserRole.Admin;
        public void Deactivate()
        {
            IsActive = false;
            SetUpdated();
        }
        public void Activate()
        {
            IsActive = true;
            SetUpdated();
        }
        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            SetUpdated();
        }
        public void VerifyEmail()
        {
            IsEmailVerified = true;
            SetUpdated();
        }
        public void ChangeRole(UserRole newRole)
        {
            Role = newRole;
            SetUpdated();
        }
        public void SetUpdatedBy(string updatedBy)
        {
            UpdatedBy = updatedBy;
            SetUpdated();
        }
        public void UpdatePasswordHash(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
            SetUpdated();
        }
        public Company Company { get; set; } = null!;

        public bool HasRole(UserRole role)
        {
            return Role == role;
        }
        public bool HasMinimumRole(UserRole minimumRole)
        {
            return (int)Role <= (int)minimumRole; // Enum değerleri ters: Admin=1, Employee=4
        }
        public bool CanLogin()
        {
            return IsActive && !IsDeleted;
        }
    }
}