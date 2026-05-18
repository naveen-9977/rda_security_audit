using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;
using System.IO;


public static class AesEncryptionHelper
{
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("12345678901234567890123456789012"); // 32 chars

    public static (string cipherText, string iv) Encrypt(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.GenerateIV();

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }

                return (
                    Convert.ToBase64String(ms.ToArray()),
                    Convert.ToBase64String(aes.IV)
                );
            }
        }
    }

    public static string Decrypt(string cipherText, string iv)
    {
        var cipherBytes = Convert.FromBase64String(cipherText);
        var ivBytes = Convert.FromBase64String(iv);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.IV = ivBytes;

            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream(cipherBytes))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }
}