using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SoftwareLicenses.Security
{
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            // просто и надёжно для диплома: SHA256 + соль
            var salt = "LozovayaAppSalt_v1"; // можно вынести
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password + salt);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }

        public static bool Verify(string password, string hash)
            => Hash(password) == hash;
    }
}
