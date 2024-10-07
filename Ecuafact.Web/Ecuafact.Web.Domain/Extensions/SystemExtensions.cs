using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Net.Http.Headers;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using System.Drawing;
using System.Drawing.Imaging;

namespace System
{
    public static class MySystemExtensions
    {
        public static T GetExtension<T>(this X509Certificate2 certificate)
            where T : X509Extension
        {
            var extensions = certificate.Extensions.Cast<X509Extension>();
            return (T)extensions.FirstOrDefault(x => typeof(T).IsAssignableFrom(x.GetType()));
        }

        public static string GetExtensionString<T>(this X509Certificate2 certificate) 
            where T : X509Extension
        {
            var info = certificate.GetExtension<T>();

            if (info != null && info.RawData != null && info.RawData.Length > 0)
            {
                var result = Encoding.Default.GetString(info.RawData) ?? string.Empty;

                return result.Replace("\r", "").Replace("\f", "").Replace("\n", "");
            }

            return default;
        }
        public static string GetExtensionString(this X509Certificate2 certificate, string oid)
        {
            var oids = oid.Split('%');

            var extensions = certificate.Extensions.Cast<X509Extension>();
            var info = extensions.FirstOrDefault(x => x.Oid.FriendlyName == oid || (
                x.Oid.Value.StartsWith(oids[0]) && x.Oid.Value.EndsWith(oids[1])));

            if (info != null && info.RawData != null && info.RawData.Length > 0)
            {
                var result = Encoding.Default.GetString(info.RawData) ?? string.Empty;

                return result.Replace("\r", "").Replace("\f", "").Replace("\n", "");
            }

            return default;
        }

        // Esto corrige el logo en los formatos de factura y otras imagenes
        public static byte[] ScaleImage(this Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(maxWidth, maxHeight);
            
            using (var graphics = Graphics.FromImage(newImage))
            {
                // Calculate x and y which center the image
                int y = (maxHeight / 2) - newHeight / 2;
                int x = (maxWidth / 2) - newWidth / 2;

                // Draw image on x and y with newWidth and newHeight
                graphics.DrawImage(image, x, y, newWidth, newHeight);

                return newImage.ToStream()?.ToArray();
            }
        }

        public static MemoryStream ToStream(this Image image)
        {
            var stream = new MemoryStream();
            image.Save(stream, ImageFormat.Png);
            return stream;
        }

        public static DateTime ToDateTime(this string text)
        {
            DateTime date;
            if (DateTime.TryParse(text, new CultureInfo("es-ES"), DateTimeStyles.AllowWhiteSpaces, out date))
            {
                return date;
            }

            return DateTime.Today.Date;
        }

        public static string GetValue(this HttpResponseHeaders header, string name)
        {
            IEnumerable<string> results;

            if (header.TryGetValues(name, out results) && results.Count() > 0)
            {
                var value = "";
                for (int i = 0; i < results.Count(); i++)
                {
                    if (i > 0) value += ",";
                    value += results.ElementAt(i);
                }
                return value;
            }

            return string.Empty;
        }


        public static string Encode(this string text)
        {
            text = $"{text}|{DateTime.Now.ToString("MMyyddd-yyddMM-dyyMM-ddMM-yy-yy")}";

            var byteArray = Encoding.UTF8.GetBytes(text);

            return Convert.ToBase64String(byteArray);
        }

        public static string Decode(this string text)
        {
            var byteArray = Convert.FromBase64String(text);
            var result = Encoding.UTF8.GetString(byteArray);
            return result.Split('|')[0];
        }

        public static string Ago(this TimeSpan time)
        {
            string text;

            if (time.Days > 0)
            {
                text = $"Hace {time.Days} dias";
            }
            else if (time.Hours > 0)
            {
                text = $"Hace {time.Hours} horas";
            }
            else if (time.Minutes > 0)
            {
                text = $"Hace {time.Minutes} minutos";
            }
            else
            {
                text = $"Hace {time.Seconds} segundos";
            }

            return text;
        }

        public static byte[] GetBytes(this HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {   
                return Array.Empty<byte>();
            }
             
            // creamos el lector
            using (MemoryStream ms = new MemoryStream())
            {
                // movemos el cursor del stream al inicio:
                file.InputStream.Position = 0;
                file.InputStream.CopyTo(ms);
                return ms.ToArray();
            }
        }


        //public static byte[] GetFileBytes(this HttpPostedFileBase rawFile)
        //{
        //    if (rawFile == null || rawFile.ContentLength == 0)
        //    {
        //        return new byte[0];
        //    }

        //    byte[] buffer = new byte[rawFile.ContentLength];
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        rawFile.InputStream.CopyTo(ms);
        //        return ms.ToArray();
        //    }


        //     //creamos el lector
        //     using (BinaryReader reader = new BinaryReader(file.InputStream))
        //     {
        //          var result = reader.ReadBytes((int)file.ContentLength);
        //          return result;
        //     }
        //}


        
    }
}
