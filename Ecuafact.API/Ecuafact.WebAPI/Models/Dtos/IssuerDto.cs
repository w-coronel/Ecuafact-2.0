using Ecuafact.WebAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Ecuafact.WebAPI.Models
{
    /// <summary>
    /// EMISOR
    /// </summary>
    public class IssuerDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Registro Unico de Contribuyente
        /// </summary>
        public string RUC { get; set; }

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
        /// Direccion del Establecimiento
        /// </summary>
        public string EstablishmentAddress { get; set; }

        /// <summary>
        /// Codigo del Establecimiento
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
        /// Esta Habilitado para Emitir?
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Logo
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// Fecha de Creación
        /// </summary>
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// Fecha de Modificación
        /// </summary>
        public DateTime? LastModifiedOn { get; set; }



        /// <summary>
        /// Correo Electronico
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Telefono
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Certificado digital
        /// </summary>
        public string Certificate { get; set; }

        /// <summary>
        /// Contraseña del Certificado
        /// </summary>
        public string CertificatePass { get; set; }

        /// <summary>
        /// Nombre del Certificado
        /// </summary>
        public string CertificateIssuedTo { get; set; }

        /// <summary>
        /// Descripcion del Certificado
        /// </summary>
        public string CertificateSubject { get; set; }

        /// <summary>
        /// Fecha de Expiración del Certificado
        /// </summary>
        public DateTime? CertificateExpirationDate { get; set; }

        /// <summary>
        /// RUC del certificado
        /// </summary>
        public string CertificateRUC { get; set; }

        /// <summary>
        /// Usos del Certificado
        /// </summary>
        public string CertificateUsage { get; set; }

        /// <summary>
        /// Datos del Logo
        /// </summary>
        public byte[] LogoFile { get; set; }

        /// <summary>
        /// Ciudad
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Provincia
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// Pais
        /// </summary>
        public string Country { get; set; }


        /// <summary>
        /// Es Agente de Retencion
        /// </summary>
        public bool IsRetentionAgent { get; set; }

        /// <summary>
        /// Numero Agente de Retencion
        /// </summary>
        public string AgentResolutionNumber { get; set; }

        ///// <summary>
        ///// Es Rimpe
        ///// </summary>
        public bool IsRimpe { get; set; }

        /// <summary>
        /// Régimen General
        /// </summary>
        public bool IsGeneralRegime { get; set; }

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
        /// Tipo de Negocio:
        ///  - Persona Natural
        ///  - Regimen Microempresas
        ///  - Agente de Retención
        /// </summary>
        public int BusinessScheme { get; set; }

        /// <summary>
        /// Tipo de RUC
        ///  - Persona Natural
        ///  - Personería Juridica
        /// </summary>
        public int RucType { get; set; }


        /// <summary>
        /// Visualizar solicitudes
        /// </summary>
        public bool viewRequests { get; set; }

        /// <summary>
        /// Contribuyente Negocio Popular - Regimen RIMPE
        /// </summary>
        public bool IsPopularBusiness { get; set; }


        /// <summary>
        /// si es una coopertiva
        /// </summary>
        public bool IsCooperativeCarrier { get; set; }

        /// <summary>
        /// si es transportista
        /// </summary>
        public bool IsCarrier { get; set; }


        /// <summary>
        /// Establecimientos
        /// </summary>
        public List<Establishments> Establishments { get; set; } = new List<Establishments>();

    }

    public enum RucType
    {
        Natural = 0,
        Juridical = 1
    }

    public class CarrierIssuer {

        public IssuerDto IssuerDto { get; set; }
        public SubscriptionDto SubscriptionDto { get; set; }
        
    }
}
