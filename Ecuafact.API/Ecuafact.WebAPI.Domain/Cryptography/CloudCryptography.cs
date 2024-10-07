using System;
using System.Security.Cryptography;
using System.Text;

namespace Ecuafact.WebAPI.Domain.Cryptography
{
    public static class CloudCryptography
    {
        public static string EncryptCloudString(string value)
        {
            if (value == null)
            {
                return null;
            }
            else if (value == String.Empty)
            {
                return String.Empty;
            }
            else
            {
                return EncryptCloudStringPrivate(value);
            }
        }

        private static string EncryptCloudStringPrivate(string value)
        {
            // Recuperar la clave pública del CLOUD
            var publicKey = Convert.FromBase64String(CloudCryptographyResources.CloudPublicKey);

            // Generar claves simétricas para este cifrado puntual
            byte[] key, iv;
            SymmetricCryptography.GenerateSymmetricCryptographyKey(out key, out iv);

            // Protegemos la clave simétrica
            var keyBuffer = new byte[SymmetricCryptography.KeySize + SymmetricCryptography.IVSize];
            Array.Copy(key, 0, keyBuffer, 0, SymmetricCryptography.KeySize);
            Array.Copy(iv, 0, keyBuffer, SymmetricCryptography.KeySize, SymmetricCryptography.IVSize);

            var safeKey = AsymmetricCryptography.Encrypt(keyBuffer, publicKey);

            // Protegemos los datos generados
            var rawData = Encoding.UTF8.GetBytes(value);
            var safeData = SymmetricCryptography.Encrypt(rawData, key, iv);

            // Combinamos los datos
            var safeKeyLength = safeKey.Length;
            var safeDataLength = safeData.Length;
            var outputBuffer = new byte[safeKeyLength + safeDataLength + 1];

            outputBuffer[0] = (byte)safeKeyLength;
            Array.Copy(safeKey, 0, outputBuffer, 1, safeKeyLength);
            Array.Copy(safeData, 0, outputBuffer, safeKeyLength + 1, safeDataLength);

            return Convert.ToBase64String(outputBuffer);
        }

         

        public static string EncryptString(string value)
        {
            return Encrypt(value, true);
        }


        private static string Encrypt(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            //System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            //// Get the key from config file

            //string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));
            string key = "3CU473C_NOSAPOS_3CU473C";

            //System.Windows.Forms.MessageBox.Show(key);
            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //Always release the resources and flush data
                //of the Cryptographic service provide. Best Practice

                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray = cTransform.TransformFinalBlock
                    (toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

    }
}
