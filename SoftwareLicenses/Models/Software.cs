using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SoftwareLicenses.Models
{
    public class Software
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Vendor { get; set; } = "";
        public string? Version { get; set; }
        public string? Category { get; set; }
        public bool IsFree { get; set; }
        public string? Notes { get; set; }

        public List<License> Licenses { get; set; } = new();
        public List<Installation> Installations { get; set; } = new();
    }
}
