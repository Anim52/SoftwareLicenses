using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareLicenses.Models
{
    public enum UserRole
    {
        Admin,
        Manager,
        Technician
    }

    public class Account
    {
        public int Id { get; set; }
        public string Login { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public UserRole Role { get; set; } = UserRole.Technician;
        public bool IsActive { get; set; } = true;

        // опционально: привязка к сотруднику
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
    }
}
