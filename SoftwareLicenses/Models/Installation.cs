using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareLicenses.Models
{
    public class Installation
    {
        public int Id { get; set; }

        public int DeviceId { get; set; }
        public Device Device { get; set; } = null!;

        public int SoftwareId { get; set; }
        public Software Software { get; set; } = null!;

        public string? InstalledVersion { get; set; }
        public DateOnly InstallDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        public int? LicenseId { get; set; }
        public License? License { get; set; }

        public string? InstalledBy { get; set; }
        public string? Notes { get; set; }
        public int? InstalledByEmployeeId { get; set; }
        public Employee? InstalledByEmployee { get; set; }

    }
}
