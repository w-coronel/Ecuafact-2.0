using Ecuafact.WebAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Ecuafact.WebAPI.Models
{
    /// <summary>
    /// EMISOR
    /// </summary>
    public class IssuerRequestModel : IssuerCreateModel
    { 

        /// <summary>
        /// Razon Social
        /// </summary>
        public string BussinesName { get; set; }

        /// <summary>
        /// Nombre Comercial
        /// </summary>
        public string TradeName { get; set; }

        /// <summary>
        /// Direccion Matriz
        /// </summary>
        public string MainAddress { get; set; }

        /// <summary>
        /// Direccion Matriz
        /// </summary>
        public string EstablishmentAddress { get; set; }

        /// <summary>
        /// Codigo Establecimiento
        /// </summary>
        public string EstablishmentCode { get; set; }

        /// <summary>
        /// Codigo de Punto de Emision
        /// </summary>
        public string IssuePointCode { get; set; }

        /// <summary>
        /// Es Contribuyente Especial
        /// </summary>
        public bool IsSpecialContributor { get; set; }

        /// <summary>
        /// Contribuyente Especial Nro Resolucion
        /// </summary>
        public string ResolutionNumber { get; set; }

        /// <summary>
        /// Moneda
        /// </summary>
        public string Currency { get; set; } = "DOLAR";

        /// <summary>
        /// Obligado a llevar Contabilidad
        /// </summary>
        public bool IsAccountingRequired { get; set; }

        /// <summary>
        /// Tipo de Ambiente
        /// </summary>
        public EnvironmentType EnvironmentType { get; set; }

        /// <summary>
        /// Tipo de Emisión
        /// </summary>
        public IssueType IssueType { get; set; }
           
        /// <summary>
        /// Correo Electrónico
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Telefono
        /// </summary>
        public string Phone { get; set; }
         
        /// <summary>
        /// Password del Certificado
        /// </summary>
        public string CertificatePass { get; set; }

        /// <summary>
        /// Es RIMPE 
        /// </summary>
        public bool IsRimpe { get; set; }

        /// <summary>
        /// Es Agente de Retencion
        /// </summary>
        public bool IsRetentionAgent { get; set; }

        /// <summary>
        /// Agente de Retencion Nro Resolucion
        /// </summary>        
        public string AgentResolutionNumber { get; set; }

        /// <summary>
        /// Régimen General
        /// </summary>
        public bool IsGeneralRegime { get; set; }
        
        /// <summary>
        /// Contribuyente Negocio Popular - Regimen RIMPE
        /// </summary>
        public bool IsPopularBusiness { get; set; } = false;

        /// <summary>
        /// Régimen Sociedades Simplificadas
        /// </summary>
        public bool IsSimplifiedCompaniesRegime { get; set; }

        /// <summary>
        /// Artesano Calificado
        /// </summary>
        public bool IsSkilledCraftsman { get; set; }

        /// <summary>
        /// Numero de Calificación del Artesano
        /// </summary>
        public string SkilledCraftsmanNumber { get; set; }

        /// <summary>
        /// Establecimientos
        /// </summary>
        public List<Establishments> Establishments { get; set; }

        /***************** SOLO UTILIZADAS PARA EL ENVIO DE ARCHIVOS ******************/
        /// <summary>
        /// Archivo del Logo
        /// </summary>
        public byte[] LogoFile { get; set; }

        /// <summary>
        /// Archivo del Certificado 
        /// </summary>
        public byte[] CertificateFile { get; set; }

        /// <summary>
        /// si es transportista
        /// </summary>
        public bool IsCarrier { get; set; }

    }

    /// <summary>
    /// Modelo para la generación de emisores
    /// </summary>
    public class IssuerCreateModel
    {
        /// <summary>
        /// Registro Unico de Contribuyente
        /// </summary>
        public string RUC { get; set; }
        /// <summary>
        /// Clave del SRI
        /// </summary>
        public string SRIPassword { get; set; }

    }

    public class RucRequestModel
    {
        public string RUC { get; set; }
    }

    public class EstablishmentsModel
    {
        [Required]
        [StringLength(3)]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        public long IssuerId { get; set; }
        public IssuerDto Issuer { get; set; }
        public List<IssuePointModel> IssuePoint { get; set; }
    }

    public class IssuePointModel
    {

        [StringLength(3)]
        [Required(ErrorMessage = "El campo código es requerido")]
        public string Code { get; set; }
        [StringLength(500)]
        public string Name { get; set; }
        public long EstablishmentsId { get; set; }
        public long IssuerId { get; set; }
        [Required(ErrorMessage = "El campo ruc es requerido")]
        public string CarrierRUC { get; set; }
        public string CarPlate { get; set; }
        public string CarrierEmail { get; set; }
        public string CarrierPhone { get; set; }
        public string CarrierContribuyente { get; set; }
        public string CarrierResolutionNumber { get; set; }        
        public EstablishmentsModel Establishments { get; set; }
    }

    /// <summary>
    /// Modelo para desvincular usuarios
    /// </summary>
    public class UserPermissionsModel
    {
        public string Username { get; set; }
        public string RUC { get; set; }
        public long IssuerId { get; set; }
    }
}
