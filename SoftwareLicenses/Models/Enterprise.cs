using System.Collections.Generic;

namespace SoftwareLicenses.Models
{
    public class Enterprise
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Inn { get; set; }
        public string? Address { get; set; }
        public string? ContactPerson { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Notes { get; set; }

        public List<Device> Devices { get; set; } = new();
        public List<License> Licenses { get; set; } = new();
    }
}
