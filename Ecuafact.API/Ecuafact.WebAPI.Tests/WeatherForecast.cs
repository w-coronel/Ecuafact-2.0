using System;

namespace Ecuafact.WebAPI.Tests
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }


    public class ElectronicSignServiceRequest
    {
        public string apikey { get; set; }
        public string uid { get; set; }
        public short cobro_directo { get; set; } = 1;

        public SignTypeEnum tipo_firma { get; set; }
        public string id_tipodocumento { get; set; }


        public string id_numerodocumento { get; set; }
        public string id_nombre { get; set; }

        public string id_apellido { get; set; }

        public string id_telefono { get; set; }
        public string id_correo { get; set; }
        public string direccion { get; set; }

        public SignatureValidyEnum id_vigenciafirma { get; set; }

        public VerificationTypeEnum id_tipoverificacion { get; set; }
        public FileFormatEnum id_formato { get; set; }
        public string id_direccion_skype { get; set; }
        public string id_direccion_fisica { get; set; }
        public byte[] id_copiacedula { get; set; }
        public byte[] id_copiapapvotacion { get; set; }
        public byte[] id_copiaruc { get; set; }
        public byte[] id_autreprelegal { get; set; }
        public byte[] id_nombramiento { get; set; }
        public byte[] id_const { get; set; }

    }

    public class ElectronicSignServiceResult
    {
        public ElectronicSignServiceResult()
        {

        }

        public ElectronicSignServiceResult(bool isOK, string msg)
        {
            this.result = isOK;
            this.message = msg;
        }

        public string message { get; set; }
        public bool result { get; set; }
    }


    public enum ElectronicSignStatusEnum : short
    {
        Error = -1,
        Saved = 0,
        Approved = 1,
        Processed = 2
    }


    public enum SignatureValidyEnum : short
    {
        OneYear = 1,
        TwoYears = 2
    }


    public enum SignTypeEnum : short
    {
        Natural = 1,
        Juridical = 2
    }

    public enum FileFormatEnum : short
    {
        File = 1,
        Token = 2
    }


    public enum VerificationTypeEnum : short
    {
        Office = 1,
        Skype = 2,
        Home = 3,
        External = 4,
        Other = 5,
        Unknown = 6,
        Ecuanexus = 7,
        Company = 8,
        Ecuafact = 9
    }


    /// <summary>
    /// Resultado de Proceso de Firma Electronica.
    /// </summary>
    public class ElectronicSignFileRequest
    {
        /// <summary>
        /// Token de Seguridad
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// Identificador del Proveedor
        /// </summary>
        public string suscriptor { get; set; }
        /// <summary>
        /// RUC del Cliente
        /// </summary>

        public string ruc { get; set; }
        /// <summary>
        /// Archivo del certificado .p12 en formato codificado 64 (bytes[])
        /// </summary>

        public byte[] bytesCertificado { get; set; }
        /// <summary>
        /// La clave de la firma electrónica
        /// </summary>
        public string claveCertificado { get; set; }
        /// <summary>
        /// La fecha de caducidad de la firma electrónica
        /// </summary>
        public DateTime fechaCaducidad { get; set; }
        /// <summary>
        /// Resultado del proceso de Generacion
        /// </summary>

        public bool result { get; set; }
        /// <summary>
        /// Mensaje de respuesta del proceso de generacion de certificados
        /// </summary>

        public string message { get; set; }
    }

}
