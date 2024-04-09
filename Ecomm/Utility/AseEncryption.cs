using System;
using System.IO;
using System.Security.Cryptography;

namespace Ecomm.Utility
{
    public class AesEncryption
    {
        public static byte[] Encrypt_AES(string plainText, byte[] key, byte[] iv)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));

            byte[] encrypted;
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        // Convert the plaintext to bytes
                        byte[] plaintextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);

                        // Write the plaintext bytes to the crypto stream
                        csEncrypt.Write(plaintextBytes, 0, plaintextBytes.Length);
                    }
                    // Get the encrypted bytes from the memory stream
                    encrypted = msEncrypt.ToArray();
                }
            }
            return encrypted;
        }
    }
}
