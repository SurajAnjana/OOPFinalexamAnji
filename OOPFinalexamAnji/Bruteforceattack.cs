using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PasswordResetApp.Classes
{
    public class BruteForceAttack
    {
        private const string Characters = "abcdefghijklmnopqrstuvwxyz0123456789!@#$%&*"; // Add numbers to the character set
        private readonly string encryptedPassword;
        private readonly int maxThreads;
        private bool isPasswordFound;
        private string foundPassword = ""; // Initialize foundPassword with an empty string
        private object lockObject = new object();

        public BruteForceAttack(string encryptedPassword, int maxThreads)
        {
            this.encryptedPassword = encryptedPassword;
            this.maxThreads = maxThreads;
        }

        public async Task<(string password, TimeSpan duration)> StartAttack()
        {
            DateTime startTime = DateTime.Now;

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < maxThreads; i++)
            {
                tasks.Add(Task.Run(() => BruteForceThread()));
            }

            await Task.WhenAny(Task.WhenAll(tasks));

            TimeSpan duration = DateTime.Now - startTime;
            return (foundPassword, duration);
        }

        private void BruteForceThread()
        {
            int maxLength = 6; // Maximum password length (You can adjust this value)
            for (int length = 1; length <= maxLength; length++)
            {
                BruteForce(new char[length], 0);
                if (isPasswordFound) return;
            }
        }

        private void BruteForce(char[] array, int position)
        {
            if (position == array.Length)
            {
                string guess = new string(array);
                if (CheckPassword(guess))
                {
                    lock (lockObject)
                    {
                        if (!isPasswordFound)
                        {
                            isPasswordFound = true;
                            foundPassword = guess;
                        }
                    }
                }
                return;
            }

            for (int i = 0; i < Characters.Length; i++)
            {
                array[position] = Characters[i];
                BruteForce(array, position + 1);
                if (isPasswordFound) return;
            }
        }

        private bool CheckPassword(string password)
        {
            using (MD5 md5 = MD5.Create()) // Use MD5 instead of SHA-256
            {
                byte[] saltedPasswordBytes = Encoding.UTF8.GetBytes(password + PasswordManager.Salt);
                byte[] hashBytes = md5.ComputeHash(saltedPasswordBytes);
                string hash = Convert.ToBase64String(hashBytes);
                return hash == encryptedPassword;
            }
        }
    }
}