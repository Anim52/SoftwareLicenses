using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareLicenses.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string? Position { get; set; }
        public string? Department { get; set; }
        public string? Email { get; set; }
    }
}
