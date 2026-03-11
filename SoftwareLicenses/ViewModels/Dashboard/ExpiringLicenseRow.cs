using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareLicenses.ViewModels.Dashboard
{
    public class ExpiringLicenseRow
    {
        public int LicenseId { get; set; }
        public string SoftwareName { get; set; } = "";
        public string? KeyOrContract { get; set; }
        public string Status { get; set; } = "";
        public DateOnly? ExpireDate { get; set; }
        public int Seats { get; set; }
        public int UsedSeats { get; set; }
    }
}
