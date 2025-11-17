using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GestordeTareas.BL.Services
{
    public class SeguridadService : ISeguridadService
    {
        public string HashPassword(string password)
        {
            using var md5 = MD5.Create();
            var hashBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(password));
            return string.Concat(hashBytes.Select(b => b.ToString("x2").ToLower()));
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }
    }
}
