using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareLicenses.Models
{
    public class Device
    {
        public int Id { get; set; }
        public string InventoryNumber { get; set; } = ""; 
        public string Hostname { get; set; } = "";
        public string? SerialNumber { get; set; }
        public string? OperatingSystem { get; set; }
        public string? Location { get; set; }

        public int? ResponsibleEmployeeId { get; set; }
        public Employee? ResponsibleEmployee { get; set; }

        public string? Notes { get; set; }

        public List<Installation> Installations { get; set; } = new();
    }
}
