using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareLicenses.ViewModels.Reports
{
    public class ExpiringLicenseReportRow
    {
        public string Software { get; set; } = "";
        public string? Supplier { get; set; }
        public string Status { get; set; } = "";
        public string? KeyOrContract { get; set; }
        public int Seats { get; set; }
        public int Used { get; set; }
        public DateOnly? ExpireDate { get; set; }
    }

    public class NoLicenseInstallationReportRow
    {
        public string DeviceInv { get; set; } = "";
        public string DeviceHost { get; set; } = "";
        public string Software { get; set; } = "";
        public DateOnly InstallDate { get; set; }
        public string? InstalledBy { get; set; }
    }

    public class LicenseUsageReportRow
    {
        public string Software { get; set; } = "";
        public string? KeyOrContract { get; set; }
        public int Seats { get; set; }
        public int Used { get; set; }
        public int Over => Math.Max(0, Used - Seats);
        public DateOnly? ExpireDate { get; set; }
        public string Status { get; set; } = "";
    }
}
