using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace GestordeTareas.BL.Services
{
    public class SeguridadService : ISeguridadService
    {
        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return null;
            }
            return BCrypt.Net.BCrypt.HashPassword(password, 10);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
            {
                return false;
            }
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
