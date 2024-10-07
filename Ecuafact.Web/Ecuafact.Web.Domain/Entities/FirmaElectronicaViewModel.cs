using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ecuafact.Web.Domain.Entities
{
    public class FirmaElectronicaViewModel
    {
        /// <summary>
        /// Identificacion
        /// </summary>
        public string Identificacion { get; set; }

        /// <summary>
        /// Razon Social
        /// </summary>
        public string RazonSocial { get; set; }

        /// <summary>
        /// Nombre
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// Apellido
        /// </summary>
         public string Apellido { get; set; }
        /// <summary>
        /// Correo
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Telefono
        /// </summary>
         public string Telefono { get; set; }
        /// <summary>
        /// Direccion
        /// </summary>
         public string Direccion { get; set; }
        /// <summary>
        /// Provincia
        /// </summary>
        public string Provincia { get; set; }
        /// <summary>
        /// Ciudad
        /// </summary>
        public string Ciudad { get; set; }
        /// <summary>
        /// Pais
        /// </summary>
        public string Pais { get; set; }
        /// <summary>
        /// Skype
        /// </summary>
        public string Skype { get; set; }

        /// <summary>
        /// Vigencia de la firma en años:
        /// </summary>
        public SignatureValidyEnum VigenciaFirma { get; set; }
        /// <summary>
        /// Tipo de Verificacion
        /// </summary>
        public VerificationTypeEnum TipoVerificacion { get; set; }
        /// <summary>
        /// Formato
        /// </summary>
        public FileFormatEnum Formato { get; set; }
        /// <summary>
        /// UID
        /// </summary>
        public string UID { get; set; }
        /// <summary>
        /// Tipo de Firma
        /// </summary>
        public RucTypeEnum TipoFirma { get; set; }
        /// <summary>
        /// Tipo de Documento
        /// </summary>
        public IdentificationTypeEnum TipoDocumento { get; set; } = IdentificationTypeEnum.RUC;

        /// <summary>
        /// Copia de Cedula
        /// </summary>
        public HttpPostedFileBase CopiaCedulaRaw { get; set; }
        /// <summary>
        /// Copia Votacion 
        /// </summary>
        public HttpPostedFileBase CopiaVotacionRaw { get; set; }
        /// <summary>
        /// Copia RUC
        /// </summary>
        public HttpPostedFileBase CopiaRUCRaw { get; set; }
        /// <summary>
        /// Autorizacion Rep Legal 
        /// </summary>
        public HttpPostedFileBase AutorizacionRepLegalRaw { get; set; }
        /// <summary>
        /// Nombramiento 
        /// </summary>
        public HttpPostedFileBase NombramientoRaw { get; set; }
        /// <summary>
        /// Constitucion 
        /// </summary>
        public HttpPostedFileBase ConstitucionRaw { get; set; }
         

    }
     
    public struct TipoDocumentoEnum
    {
        public const string Cedula = "cedula";
        public const string Pasaporte = "pasaporte";
        public const string RUC = "ruc";
    }

    public enum TipoFirmaEnum : short
    {
        Natural = 1,
        Juridica = 2
    }

    public enum TipoVerificacionEnum : short
    {
        NuestrasOficinas = 1,
        Skype = 2,
        Domicilio = 3
    }

    public enum FormatoEnum : short
    {
        Archivo = 1,
        Token = 2
    }
     
}