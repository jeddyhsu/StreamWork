using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace StreamWork.Services
{
    public class EncryptionService
    {
        public string EncryptPassword(string password) //Hash for passwords
        {
            byte[] salt = new byte[128 / 8];
            using (var randomNumber = RandomNumberGenerator.Create())
            {
                randomNumber.GetBytes(salt);
            }
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA1, 10000, (256 / 8)));
            return hashed + "|" + Convert.ToBase64String(salt);
        }

        public string DecryptPassword(string salt, string password) //Compare Hash
        {
            var decrypt = salt.Split('|');
            var bytesSalt = Convert.FromBase64String(decrypt[1]);
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(password, bytesSalt, KeyDerivationPrf.HMACSHA1, 10000, (256 / 8)));
            return hashed + "|" + decrypt[1];
        }

        public string EncryptString(string s)
        {
            if (s != null || s != "")
            {
                byte[] bArray = Encoding.ASCII.GetBytes(s);
                string encrypted = Convert.ToBase64String(bArray);
                return encrypted;
            }

            return null;
        }

        public string DecryptString(string s)
        {
            byte[] bArray;
            if (s != null && s != "")
            {
                bArray = Convert.FromBase64String(s);
                return Encoding.ASCII.GetString(bArray);
            }

            return null;
        }
    }
}
