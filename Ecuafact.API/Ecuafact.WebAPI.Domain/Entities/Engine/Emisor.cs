using Ecuafact.WebAPI.Domain.Cryptography;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecuafact.WebAPI.Domain.Entities.Engine
{
    public partial class Emisor
    {
        public Emisor()
        {
            Version = "1.1.0";
            Autorizacion = "1";
            Estado = true;
            PortalClientes = "https://app.ecuafact.com";
        }

        public Emisor(Issuer issuer) : this()
        {
            this.MapTo(issuer);
        }

        [Required] public int Id { get; set; }
        [Required] public string RUC { get; set; }
        [Required] public string RazonSocial { get; set; }
        [Required] public string NombreComercial { get; set; }
        [Required] public string DireccionMatriz { get; set; }
        [Required] public bool EsContribuyenteEspecial { get; set; } = false;
        public string NoResolucion { get; set; }
        [Required] public bool ObligadoContabilidad { get; set; } = false;
        public string Logo { get; set; }
        public string CertificadoDigital { get; set; }
        public string Clave { get; set; }
        [Required] public int TipoEmision { get; set; } = 1;
        [Required] public int TipoAmbiente { get; set; } = 2;
        [Required] public string Version { get; set; } = "1.1.0";
        [Required] public string Autorizacion { get; set; } = "1";
        [Required] public bool Estado { get; set; } = true;
        public string PortalClientes { get; set; } = "https://app.ecuafact.com";
        public string EmailNotificacion { get; set; }
        public string HoraInicio { get; set; }
        public int? Frecuencia { get; set; }
        public string ClaveSRI { get; set; }
        [Required] public bool Microempresas { get; set; } = false;
        [Required] public bool EsAgenteRetencion { get; set; } = false;
        public string NoAgentResolucion { get; set; }
        


        public void MapTo(Issuer issuer)
        {
            if (string.IsNullOrEmpty(RUC))
            {
                RUC = issuer.RUC;
            } 

            RazonSocial = issuer.BussinesName;
            NombreComercial = string.IsNullOrWhiteSpace(issuer.TradeName)  ? issuer.BussinesName: issuer.TradeName;
            DireccionMatriz = issuer.MainAddress;
            EsContribuyenteEspecial = issuer.IsSpecialContributor;
            NoResolucion = issuer.ResolutionNumber;
            ObligadoContabilidad = issuer.IsAccountingRequired;
            Logo = issuer.Logo;
            CertificadoDigital = issuer.Certificate;
            TipoEmision = Convert.ToInt32(issuer.IssueType);
            TipoAmbiente = Convert.ToInt32(issuer.EnvironmentType);
            EmailNotificacion = issuer.Email;
            //HoraInicio = null;
            //Frecuencia = 0;
            Microempresas = false;
            EsAgenteRetencion = issuer.IsRetentionAgent;
            NoAgentResolucion = issuer.AgentResolutionNumber;
            

            if (!string.IsNullOrEmpty(issuer.SRIPassword))
            {
                SetClaveSRI(issuer.SRIPassword);
            }
            if (!string.IsNullOrEmpty(issuer.CertificatePass))
            {
                SetClaveCertificado(issuer.CertificatePass);
            }
        }

        public void SetClaveCertificado(string clave)
        {
            Clave = CloudCryptography.EncryptString(clave);
        }

        public void SetClaveSRI(string clave)
        {
            ClaveSRI = CloudCryptography.EncryptCloudString(clave);
        }

    }

    public partial class EmisorTextoInfoAdicional
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int Id { get; set; }
        [Required]
        public string RucEmisor { get; set; }
        [Required]
        public string nombre { get; set; }       
        public string valor { get; set; }        

    }

    public enum TextoInfoAdicionalEnum : short
    {
        [EcuafactEnum("Agente de Retención", "Agente de Retención")]
        IsRetentionAgent = 1,

        [EcuafactEnum("Contribuyente Régimen RIMPE - Emprendedores", "RIMPE")]
        IsRimpe = 2,

        [EcuafactEnum("Contribuyente Régimen General", "General")]
        IsGeneralRegime = 3,

        [EcuafactEnum("Régimen Sociedades Simplificadas", "Sociedades Simplificadas")]
        IsSimplifiedCompaniesRegime = 4,

        [EcuafactEnum("Artesano Calificado Nº", "Artesano Calificado Nº")] 
        IsSkilledCraftsman = 5,

        [EcuafactEnum("Contribuyente Negocio Popular - Régimen RIMPE", "NegocioPopular")]
        IsPopularBusiness = 6,
    }

}
