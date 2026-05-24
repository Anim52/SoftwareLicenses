using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareLicenses.Models
{
    public enum LicenseType { PerDevice, PerUser, Volume, Subscription }
    public enum LicenseStatus { Active, Expired, Revoked }

    public class License
    {
        public int Id { get; set; }

        public int SoftwareId { get; set; }
        public Software Software { get; set; } = null!;

        public LicenseType Type { get; set; }
        public LicenseStatus Status { get; set; } = LicenseStatus.Active;

        public string? KeyOrContract { get; set; }
        public int Seats { get; set; } = 1;

        public DateOnly PurchaseDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public DateOnly? ExpireDate { get; set; }

        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        public int? EnterpriseId { get; set; }
        public Enterprise? Enterprise { get; set; }

        public decimal? Cost { get; set; }
        public string? Notes { get; set; }

        public List<Installation> Installations { get; set; } = new();
    }
}
