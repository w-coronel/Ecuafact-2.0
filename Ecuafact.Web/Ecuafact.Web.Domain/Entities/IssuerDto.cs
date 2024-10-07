using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace Ecuafact.Web.Domain.Entities
{
    public class IssuerDto
    {
        public int Id { get; set; }

        [DisplayName("RUC #")]
        public string RUC { get; set; }

        [DisplayName("Razón Social")]
        public string BussinesName { get; set; }

        [DisplayName("Nombre Comercial")]
        public string TradeName { get; set; }

        [DisplayName("Dirección Principal")]
        public string MainAddress { get; set; }

        [DisplayName("Dirección Establecimiento")]
        public string EstablishmentAddress { get; set; }

        [DisplayName("Código Establecimiento")]
        public string EstablishmentCode { get; set; }

        [DisplayName("Punto de Emisión")]
        public string IssuePointCode { get; set; }

        [DisplayName("Contribuyente Especial")]
        public bool IsSpecialContributor { get; set; }

        [DisplayName("Resolución No.")]
        public string ResolutionNumber { get; set; }

        /// <summary>
        /// Moneda
        /// </summary>
        [DisplayName("Moneda")]
        public string Currency { get; set; } = "DOLAR";

        [DisplayName("Obligado a llevar contabilidad")]
        public bool IsAccountingRequired { get; set; }

        [DisplayName("Tipo de Entorno")]
        public EnvironmentTypeEnum EnvironmentType { get; set; } = EnvironmentTypeEnum.Producción;

        [DisplayName("Tipo de Emisión")]
        public IssueTypeEnum IssueType { get; set; } = IssueTypeEnum.Normal;

        [DisplayName("Logo")]
        public string Logo { get; set; }

        [DisplayName("Habilitado")]
        public bool IsEnabled { get; set; }

        // Columnas para agregar: 

        [DisplayName("E-mail")]
        public string Email { get; set; }
         
        [DisplayName("Teléfono")]
        public string Phone { get; set; }
         
        [DisplayName("Certificado Digital")]
        public string Certificate { get; set; }

        [DisplayName("Clave del Certificado")]
        public string CertificatePass { get; set; }
         
        [DisplayName("Clave del SRI")]
        public string SRIPassword { get; set; }

        public string CertificateIssuedTo { get; set; }
        
        public string CertificateRUC { get; set; }

        public string CertificateUsage { get; set; }

        public string CertificateSubject { get; set; }

        public DateTime? CertificateExpirationDate { get; set; }

        public byte[] LogoFile { get; set; }

        public byte[] CertificateFile { get; set; }
         
        [JsonIgnore]
        public HttpPostedFileBase LogoRaw { get; set; }

        [JsonIgnore]
        public HttpPostedFileBase CertificateRaw { get; set; }

        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }

        [DisplayName("Contribuyente Régimen RIMPE (Emprendedores)")]
        public bool IsRimpe { get; set; }

        [DisplayName("Contribuyente Negocio Popular - Regimen RIMPE")]
        public bool IsPopularBusiness { get; set; }        

        [DisplayName("Agente Retención")]
        public bool IsRetentionAgent { get; set; }

        [DisplayName("Resolución No.")]
        public string AgentResolutionNumber { get; set; }
        public bool viewRequests { get; set; }
        [DisplayName("Estabecimiento")]
        public List<Establishments> Establishments { get; set; } = new List<Establishments>();

        [DisplayName("Contribuyente Regimen General")]
        public bool IsGeneralRegime { get; set; }

        [DisplayName("Regimen Sociedades Simplificadas")]
        public bool IsSimplifiedCompaniesRegime { get; set; }

        [DisplayName("Artesano Calificado")]
        public bool IsSkilledCraftsman { get; set; }

        [DisplayName("Calificación Nº")]
        public string SkilledCraftsmanNumber { get; set; }
       
        public bool IsCooperativeCarrier { get; set; }

        [DisplayName("Es Transportista")]
        public bool IsCarrier { get; set; }

        public int AmountIssuePoints { get; set; } = 0;

        public DateTime CreatedOn { get; set; }



    }

    public class CertificateInfo
    { 
        [DisplayName("Clave del Certificado")]
        public string CertificatePass { get; set; }

        public HttpPostedFileBase CertificateRaw { get; set; } 
    }

    public class CertificateConf
    {
        public string Ruc { get; set; }
        public string CertificatePass { get; set; }
        public HttpPostedFileBase CertificateBase { get; set; }       
        public byte[] CertificateRaw => certificateRaw ?? (certificateRaw = CertificateBase.GetBytes());

        private byte[] certificateRaw;
    }

    public class SRIRequestModel
    {
        public string RUC { get; set; }
        public string SRIPassword { get; set; }
    }

    public class Establishments
    {        
        public long Id { get; set; }        
        public string Code { get; set; }        
        public string Name { get; set; }        
        public string Address { get; set; }
        public long IssuerId { get; set; }
        public int AmountIssuePoint { get; set; } 
        public List<IssuePoint> IssuePoint { get; set; } = new List<IssuePoint>();
    }

    public class IssuePoint
    {        
        public long Id { get; set; }        
        public string Code { get; set; }       
        public string Name { get; set; }       
        public long EstablishmentsId { get; set; }
        public long IssuerId { get; set; }
        public string CarrierRUC { get; set; }
        public string CarPlate { get; set; }
        public string CarrierEmail { get; set; }
        public string CarrierPhone { get; set; }
        public string CarrierContribuyente { get; set; }
        public string CarrierResolutionNumber { get; set; }
        [NotMapped]
        public Establishments Establishments { get; set; }
    }

    public class CarrierIssuer
    {
        public IssuerDto IssuerDto { get; set; }
        public SubscriptionModel SubscriptionDto { get; set; }
    }
}
