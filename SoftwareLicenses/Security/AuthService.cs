using Microsoft.EntityFrameworkCore;
using SoftwareLicenses.Data;
using SoftwareLicenses.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareLicenses.Security
{
    public class AuthService
    {
        public Account? CurrentUser { get; private set; }

        public bool Login(string login, string password)
        {
            using var db = new AppDbContext();

            var user = db.Accounts.AsNoTracking()
                .FirstOrDefault(u => u.Login == login && u.IsActive);

            if (user == null) return false;
            if (!PasswordHasher.Verify(password, user.PasswordHash)) return false;

            CurrentUser = user;
            return true;
        }

        public void Logout() => CurrentUser = null;

        public bool IsInRole(UserRole role)
            => CurrentUser?.Role == role;
    }
}
