using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class Issuer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }

        /// <summary>
        /// Registro Unico de Contribuyente
        /// </summary>
        [Index("IX_ISSUER_RUC")]
        [MaxLength(13)]
        [Required]
        public string RUC { get; set; }

        /// <summary>
        /// Razon Social
        /// </summary>
        [MaxLength(300)]
        [Required]
        public string BussinesName { get; set; }

        /// <summary>
        /// Nombre Comercial
        /// </summary>
        [MaxLength(300)]
        public string TradeName { get; set; }

        /// <summary>
        /// Direccion Matriz
        /// </summary>
        [MaxLength(300)]
        public string MainAddress { get; set; }

        /// <summary>
        /// Direccion del Establecimiento
        /// </summary>
        [MaxLength(300)]
        public string EstablishmentAddress { get; set; }

        /// <summary>
        /// Codigo Establecimiento
        /// </summary>
        [StringLength(5)]
        public string EstablishmentCode { get; set; }

        /// <summary>
        /// Codigo de Punto de Emision
        /// </summary>
        [StringLength(5)]
        public string IssuePointCode { get; set; }

        /// <summary>
        /// Es Contribuyente Especial
        /// </summary>
        public bool IsSpecialContributor { get; set; }

        /// <summary>
        /// Contribuyente Especial Nro Resolucion
        /// </summary>
        [StringLength(50)]
        public string ResolutionNumber { get; set; }

        /// <summary>
        /// Obligado a llevar Contabilidad
        /// </summary>
        public bool IsAccountingRequired { get; set; }

        /// <summary>
        /// Tipo de Ambiente
        /// </summary>
        public EnvironmentType EnvironmentType { get; set; } = EnvironmentType.Production;

        /// <summary>
        /// Tipo de Emisión
        /// </summary>
        public IssueType IssueType { get; set; } = IssueType.Normal;

        /// <summary>
        /// Esta Activo
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Logo
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// Moneda
        /// </summary>
        public string Currency { get; set; } = "DOLAR";
        
        /// <summary>
        /// Fecha de Creación
        /// </summary>
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        
        /// <summary>
        /// Fecha de Modificación
        /// </summary>
        public DateTime? LastModifiedOn { get; set; }



        /// <summary>
        /// Correo Electrónico
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Telefono
        /// </summary>
        public string Phone { get; set; }


        /// <summary>
        /// Nombre del Certificado Digital
        /// </summary>
        public string Certificate { get; set; }

        /// <summary>
        /// Password del Certificado
        /// </summary>
        public string CertificatePass { get; set; }


        /// <summary>
        /// Clave de Acceso de Descarga del SRI
        /// </summary>
        public string SRIPassword { get; set; }

        public string CertificateIssuedTo { get; set; }

        public string CertificateSubject { get; set; }

        public DateTime? CertificateExpirationDate { get; set; }

        public string City { get; set; }

        public string Province { get; set; }

        public string Country { get; set; }
       
        /// <summary>
        /// Es Agente de Retencion
        /// </summary>
        public bool IsRetentionAgent { get; set; }

        /// <summary>
        /// Numero Agente de Retencion
        /// </summary>
        [StringLength(4)]
        public string AgentResolutionNumber { get; set; }

        /// <summary>
        /// Contribuyente régimen Rimpe
        /// </summary>
        public bool IsRimpe { get; set; }


        /// <summary>
        /// Contribuyente régimen General
        /// </summary>
        public bool IsGeneralRegime { get; set; }

        /// <summary>
        /// Contribuyente régimen Sociedades Simplificadas
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
        /// Contribuyente Negocio Popular - Regimen RIMPE
        /// </summary>
        public bool IsPopularBusiness { get; set; }

        /// <summary>
        /// Si es coopertiva de trasporte
        /// </summary>
        public bool IsCooperativeCarrier { get; set; } = false;

        /// <summary>
        /// Si es transportista
        /// </summary>
        public bool IsCarrier { get; set; } = false;
        

        public List<Establishments> Establishments { get; set; } = new List<Establishments>();

    }

    /// <summary>
    /// TIPO DE EMISION
    /// </summary>
    public enum IssueType
    {
        /// <summary>
        /// Normal
        /// </summary>
        [EcuafactEnum("1", "Normal")]
        Normal = 1,

        /// <summary>
        /// Por indisponibilidad del Sistema
        /// </summary>
        [EcuafactEnum("0", "Por indisponibilidad del Sistema")]
        Unavailable
    }

    /// <summary>
    /// TIPO DE AMBIENTE
    /// </summary>
    public enum EnvironmentType
    {
        /// <summary>
        /// Pruebas
        /// </summary>
        [EcuafactEnum("1", "Normal")]
        Testing = 1,
        
        /// <summary>
        /// Producción
        /// </summary>
        [EcuafactEnum("2", "Produccion")]
        Production = 2
    }
}
