using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareLicenses.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Inn { get; set; }
        public string? Contact { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}
