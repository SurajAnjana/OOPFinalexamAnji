using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PasswordResetApp.Classes
{
    public class PasswordManager
    {
        internal const string Salt = "staticSalt"; // Internal salt

        public void CreateAndStorePassword(string password, string filePath)
        {
            string encryptedPassword = EncryptPassword(password);
            File.WriteAllText(filePath, encryptedPassword);
        }

        private string EncryptPassword(string password)
        {
            using (MD5 md5 = MD5.Create()) // Use MD5 instead of SHA-256
            {
                byte[] saltedPasswordBytes = Encoding.UTF8.GetBytes(password + Salt);
                byte[] hashBytes = md5.ComputeHash(saltedPasswordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}