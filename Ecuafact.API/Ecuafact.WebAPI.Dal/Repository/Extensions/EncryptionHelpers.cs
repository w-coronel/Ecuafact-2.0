using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Dal.Repository.Extensions
{
    public class EncryptionHelpers
    { 
        public static string Decrypt(string encryptedText)
        {
            try
            {
                byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
                byte[] keyBytes = new Rfc2898DeriveBytes(Constants.EncryptionKeys.PasswordHash, Encoding.ASCII.GetBytes(Constants.EncryptionKeys.SaltKey)).GetBytes(256 / 8);

                using (var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None })
                {

                    var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(Constants.EncryptionKeys.VIKey));
                    var memoryStream = new MemoryStream(cipherTextBytes);
                    var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                    byte[] plainTextBytes = new byte[cipherTextBytes.Length];

                    int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

                    memoryStream.Close();
                    cryptoStream.Close();

                    return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("El token de seguridad enviado no es el correcto.", ex);
            }
        }


    }
}
