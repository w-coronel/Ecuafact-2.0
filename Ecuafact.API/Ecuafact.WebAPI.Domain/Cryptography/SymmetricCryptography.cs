using System.IO;
using System.Security.Cryptography;

namespace Ecuafact.WebAPI.Domain.Cryptography
{
    internal static class SymmetricCryptography
    {
        internal const int KeySize = 32;
        internal const int IVSize = 16;

        private const int m_KeySizeBits = KeySize * 8;
        private const int m_BlockSizeBits = IVSize * 8;

        internal static void GenerateSymmetricCryptographyKey(out byte[] key, out byte[] iv)
        {
            using (var aes = GetAesCryptoServiceProvider())
            {
                aes.GenerateKey();
                aes.GenerateIV();

                key = aes.Key;
                iv = aes.IV;
            }
        }

        internal static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (var outputBuffer = new MemoryStream())
            {
                using (var aes = GetAesCryptoServiceProvider(key, iv))
                {
                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    {
                        using (var cryptoStream = new CryptoStream(outputBuffer, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(data, 0, data.Length);
                            cryptoStream.FlushFinalBlock();
                        }
                    }
                }

                return outputBuffer.ToArray();
            }
        }

        private static AesCryptoServiceProvider GetAesCryptoServiceProvider(byte[] key, byte[] iv)
        {
            var aes = GetAesCryptoServiceProvider();

            aes.Key = key;
            aes.IV = iv;

            return aes;
        }

        private static AesCryptoServiceProvider GetAesCryptoServiceProvider()
        {
            return new AesCryptoServiceProvider()
            {
                KeySize = m_KeySizeBits,
                BlockSize = m_BlockSizeBits,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
            };
        }
    }
}
