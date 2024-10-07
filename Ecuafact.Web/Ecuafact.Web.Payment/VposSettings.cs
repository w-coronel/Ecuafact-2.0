using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Payment
{
    /// <summary>
    /// Carga la informacion desde
    /// </summary>
    public class VPOSSettings
    {
        public string RootDirectory { get; set; } // "D:\Proyectos\EcuaNexus\ECUAFACT_2019\Ecuafact.Web\Ecuafact.Web\VPOS"
        public string UrlVPOS { get; set; } = "https://integracion.alignetsac.com/VPOS/MM/transactionStart20.do";
        public string IdCommerce { get; set; } = "7580";
        public string IdAcquirer { get; set; } = "8";
        public string TerminalCode { get; set; } = "000001";
        public string VectorInitialize { get; set; } = "94DE49FE0A97D2FD";
         
        public string LlaveCifradoVPOSPublic { get; set; } = "ALIGNET.TESTING.NOPHP.CRYPTO.PUBLIC.txt";
        public string LlaveFirmaVPOSPublic { get; set; } = "ALIGNET.TESTING.NOPHP.SIGNATURE.PUBLIC.txt";
        public string LlaveCifradoComPrivate { get; set; } = @"Testing\LLAVEPRIVADA_CIFRADO_RSA.txt";
        public string LlaveFirmaComPrivate { get; set; } = @"Testing\LLAVEPRIVADA_FIRMA_RSA.txt";

        static VPOSSettings __settings = null;

        public static VPOSSettings Current
        {
            get
            {
                if (__settings == null)
                {
                    var file = ConfigurationManager.AppSettings["VPOS:Settings"] ?? "VPOS_Development.json";
                    __settings = VPOSSettings.Load(file);
                }

                return __settings;
            }
        }

        public static VPOSSettings Load(string filename)
        {
            try
            {
                var directory = Path.GetDirectoryName(filename);

                if (!File.Exists(filename))
                {
                    directory = ConfigurationManager.AppSettings["VPOS:Directory"] ?? Path.GetDirectoryName(typeof(VPOSSettings).Assembly.Location).Replace("/bin", "");

                    filename = Path.Combine(directory, filename);
                }

                if (File.Exists(filename))
                {
                    var json = File.ReadAllText(filename);
                    var result = JsonConvert.DeserializeObject<VPOSSettings>(json);
                    if (string.IsNullOrEmpty(result.RootDirectory))
                    {
                        result.RootDirectory = directory;
                    }
                    return result;
                }
            }
            catch (Exception) { }
            

            return new VPOSSettings();
        }
    }
}
