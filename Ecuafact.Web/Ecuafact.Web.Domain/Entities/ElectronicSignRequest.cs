using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Ecuafact.Web.Domain.Entities
{

    public class ElectronicSignRequest
    {
        public long Id { get; set; }

        [MaxLength(13)]
        [Required]
        public string Identification { get; set; }
        [MaxLength(300)]
        [Required]
        public string FirstName { get; set; }
        [MaxLength(300)]
        [Required]
        public string LastName { get; set; }
        [EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido")]
        [Required]
        public string Email { get; set; }
        [Phone(ErrorMessage = "Debe ingresar un número de teléfono válido")]
        [Required]
        public string Phone { get; set; }
        [MaxLength(300)]
        [Required]
        public string Address { get; set; }
        [MaxLength(13)]
        [Required]
        public string RUC { get; set; }
        [MaxLength(300)]
        [Required]
        public string BusinessName { get; set; }
        [MaxLength(300)]
        [Required]
        public string BusinessAddress { get; set; }
        [MaxLength(100)]
        [Required]
        public string City { get; set; }
        [MaxLength(100)]
        [Required]
        public string Province { get; set; }
        [MaxLength(100)]
        [Required]
        public string Country { get; set; }
        public bool SendProcessElectronicSign { get; set; } = false;
        [Required]
        public string ConfirmEmail { get; set; }
        [Required]
        public string ConfirmPhone { get; set; }
        public string Email2 { get; set; }
        public string Phone2 { get; set; }
        public string ConfirmEmail2 { get; set; }
        public string ConfirmPhone2 { get; set; }
        /// <summary>
        /// Not required --Solo se incluye por compatibilidad con el servicio
        /// </summary>
        public string Skype { get; set; }
        [Required]
        public SignatureValidyEnum SignatureValidy { get; set; }
        [Required]
        public VerificationTypeEnum VerificationType { get; set; }
        [Required]
        public FileFormatEnum FileFormat { get; set; }
        public RucTypeEnum SignType { get; set; }
        [Required]
        public IdentificationTypeEnum DocumentType { get; set; }
        [Required]
        public SexoEnum Sexo { get; set; }
        [Required]
        public string BirthDate { get; set; }
        public string WorkPosition { get; set; }
        public string DiscountCode { get; set; }
        public string FingerPrintCode { get; set; }
        [JsonIgnore]
        public HttpPostedFileBase CedulaFrontFile { get; set; }
        [JsonIgnore]
        public HttpPostedFileBase CedulaBackFile { get; set; }
        [JsonIgnore]
        public HttpPostedFileBase SelfieFile { get; set; }
        [JsonIgnore]
        public HttpPostedFileBase IdentificationCardFile { get; set; }
        [JsonIgnore]
        public HttpPostedFileBase ElectionsTicketFile { get; set; }
        [JsonIgnore]
        public HttpPostedFileBase AuthorizationLegalRepFile { get; set; }
        [JsonIgnore]
        public HttpPostedFileBase RucFile { get; set; }
        [JsonIgnore]
        public HttpPostedFileBase DesignationFile { get; set; }
        [JsonIgnore]
        public HttpPostedFileBase ConstitutionFile { get; set; }
        [JsonIgnore]
        public HttpPostedFileBase SelfieStatFile { get; set; }
        [JsonIgnore]
        public HttpPostedFileBase AuthorizationFile { get; set; }
        [JsonIgnore]
        public HttpPostedFileBase AuthorizationAgeFile { get; set; }
        [Required]
        public byte[] CedulaFrontRaw => cedulaFrontRaw ?? (cedulaFrontRaw = CedulaFrontFile.GetBytes());
        [Required]
        public byte[] CedulaBackRaw => (cedulaBackRaw ?? (cedulaBackRaw = CedulaBackFile.GetBytes()));
        [Required]
        public byte[] SelfieRaw => (selfieRaw ?? (selfieRaw = SelfieFile != null ? SelfieFile.GetBytes() : SelfieStatFile.GetBytes()));
        [Required]
        public byte[] IdentificationCardRaw => (identificationCardRaw ?? (identificationCardRaw = IdentificationCardFile.GetBytes()));
        [Required]
        public byte[] ElectionsTicketRaw => (electionsTicketRaw ?? (electionsTicketRaw = ElectionsTicketFile.GetBytes()));
        [Required]
        public byte[] AuthorizationLegalRepRaw => (authorizationLegalRepRaw ?? (authorizationLegalRepRaw = AuthorizationLegalRepFile.GetBytes()));
        [Required]
        public byte[] RucRaw => (rucRaw ?? (rucRaw = RucFile.GetBytes()));
        [Required]
        public byte[] DesignationRaw => (designationRaw ?? (designationRaw = DesignationFile.GetBytes()));
        [Required]
        public byte[] ConstitutionRaw => (constitutionRaw ?? (constitutionRaw = ConstitutionFile.GetBytes()));
        public byte[] AuthorizationRaw => (authorizationRaw ?? (authorizationRaw = AuthorizationFile.GetBytes()));
        public byte[] AuthorizationAgeRaw => (authorizationAgeRaw ?? (authorizationAgeRaw = AuthorizationAgeFile.GetBytes()));

        private byte[] cedulaFrontRaw;
        private byte[] cedulaBackRaw;
        private byte[] selfieRaw;
        private byte[] identificationCardRaw;
        private byte[] electionsTicketRaw;
        private byte[] authorizationLegalRepRaw;
        private byte[] rucRaw;
        private byte[] designationRaw;
        private byte[] constitutionRaw;
        private byte[] authorizationRaw;
        private byte[] authorizationAgeRaw;

        public ElectronicSignInvoice InvoiceInfo { get; set; }

        public bool FileCedulaFront { get; set; }
        public bool FileCedulaBack { get; set; }
        public bool FileSelfie { get; set; }
        public bool FileRuc { get; set; }
        public bool FileConstitution { get; set; }
        public bool FileDesignation { get; set; }
        public bool FileAuthorization { get; set; }
        public bool FileAuthorizationAge { get; set; }
        public long? LicenceTypeId { get; set; }
        public string AuthorizationAgeFormat { get; set; }
    }


    public class ElectronicSignInvoice
    {
        public string Identification { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        [EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido")]
        public string Email { get; set; }
        [Phone(ErrorMessage = "Debe ingresar un número de teléfono válido")]
        public string Phone { get; set; }
        public string DiscountCode { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Iva { get; set; }
        public decimal IvaRate { get; set; }
        public string Product { get; set; }     

    }

    public enum IdentificationTypeEnum : short
    {
        IdentityCard = 1,
        RUC = 4,
        Passport = 6
    }

    public enum SignatureValidyEnum : short
    {
        OneYear = 1,
        TwoYears = 2
    }


    public enum RucTypeEnum : short
    {
        Natural = 1,
        Juridical = 2
    }

    public enum FileFormatEnum : short
    {
        File = 0,        
        File1 = 1,
        Token = 1,
        Token2 = 2,
        Nube = 2
    }


    public enum VerificationTypeEnum : short
    {
        Office = 1,
        Skype = 2,
        Home = 3,
        Ecuanexus = 7
    }

    public enum SexoEnum : short
    {
        Masculino = 1,
        Femenino = 2,
    }

    /// <summary>
    /// Proveedor vigente para procesar firma
    /// </summary>
    public enum SupplierElectronicSignEnum : short
    {
        /// <summary>
        /// GSV operador de firma electrónica 
        /// </summary>
        GSV = 1,
        /// <summary>
        ///Uanataca operador de firma electrónica 
        /// </summary>
        Uanataca = 2,

    }    

}
