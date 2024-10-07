using System.Security.Cryptography;

namespace Ecuafact.WebAPI.Domain.Cryptography
{
    internal static class AsymmetricCryptography
    {
        internal static byte[] Encrypt(byte[] data, byte[] publicKey)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportCspBlob(publicKey);
                return rsa.Encrypt(data, false);
            }
        }
    }
}
