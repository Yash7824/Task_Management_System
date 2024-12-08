using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Security.Cryptography;
using System.Text;
using Task_Management_System.Constants;

namespace Task_Management_System.Services
{
    public class CommonMethod
    {
        public static string EncryptAES(string plainText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = AdjustKey(TaskConstant.Encryption_Key, 32); 
                aes.IV = AdjustKey(TaskConstant.Encyption_IV, 16);  

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string DecryptAES(string cipherText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = AdjustKey(TaskConstant.Encryption_Key, 32); 
                aes.IV = AdjustKey(TaskConstant.Encyption_IV, 16);  

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }

        private static byte[] AdjustKey(string key, int requiredSize)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            if (keyBytes.Length == requiredSize)
            {
                return keyBytes;
            }

            if (keyBytes.Length > requiredSize)
            {
                Array.Resize(ref keyBytes, requiredSize);
            }
            else
            {
                byte[] paddedKey = new byte[requiredSize];
                Array.Copy(keyBytes, paddedKey, keyBytes.Length);
                return paddedKey;
            }

            return keyBytes;
        }
    }
}
