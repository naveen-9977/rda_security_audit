using System;
using System.Security.Cryptography;
using System.Text;

namespace rda_raipur.Helpers
{
    public class CCAvenueCrypto
    {
        // 16-byte Initial Vector (IV) exactly as used in PHP
        private readonly byte[] iv = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f };

        public string Encrypt(string plainText, string workingKey)
        {
            byte[] key = GetMD5Hash(workingKey);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7; // Equivalent to PKCS5 in PHP

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    byte[] cipherBytes = encryptor.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);
                    return BitConverter.ToString(cipherBytes).Replace("-", "").ToLower();
                }
            }
        }

        public string Decrypt(string encryptedText, string workingKey)
        {
            byte[] key = GetMD5Hash(workingKey);
            byte[] cipherBytes = HexToByteArray(encryptedText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] plainTextBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                    return Encoding.UTF8.GetString(plainTextBytes);
                }
            }
        }

        private byte[] GetMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                return md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            }
        }

        private byte[] HexToByteArray(string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}