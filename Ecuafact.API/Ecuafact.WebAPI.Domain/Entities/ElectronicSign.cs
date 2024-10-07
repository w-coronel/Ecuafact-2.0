using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class ElectronicSign
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int Id { get; set; }
        [MaxLength(13)]
        [Required]
        public string Identification { get; set; }
        [MaxLength(300)]
        [Required]
        public string FirstName { get; set; }
        [MaxLength(300)]
        [Required]
        public string LastName { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Phone]
        [Required]
        public string Phone { get; set; }
        [MaxLength(300)]
        [Required]
        public string Address { get; set; }
        public string Skype { get; set; }
        [Required]
        public string RUC { get; set; }
        [MaxLength(300)]
        [Required]
        public string BusinessName { get; set; }
        [MaxLength(300)]
        public string BusinessAddress { get; set; }
        [MaxLength(100)]
        public string City { get; set; }
        [MaxLength(100)]
        public string Province { get; set; }
        [MaxLength(100)]
        public string Country { get; set; }
        [Required]
        public SignatureValidyEnum SignatureValidy { get; set; }
        [Required]
        public VerificationTypeEnum VerificationType { get; set; } = VerificationTypeEnum.Ecuanexus;
        [Required]
        public FileFormatEnum FileFormat { get; set; } = FileFormatEnum.File;
        public RucTypeEnum SignType { get; set; }
        [Required]
        public IdentificationTypeEnum DocumentType { get; set; } = IdentificationTypeEnum.RUC;
        [Required]
        public string CedulaFrontFile { get; set; }
        [Required]
        public string CedulaBackFile { get; set; }
        [Required]
        public string SelfieFile { get; set; }
        [Required]
        public string IdentificationCardFile { get; set; }
        [Required]
        public string RucFile { get; set; }
        [Required]
        public string AuthorizationLegalRepFile { get; set; }
        [Required]
        public string ElectionsTicketFile { get; set; }
        [Required]
        public string DesignationFile { get; set; }
        [Required]
        public string ConstitutionFile { get; set; } 
        [ForeignKey("PurchaseOrder")]
        public int PurchaseOrderId { get; set; }         
        public PurchaseOrder PurchaseOrder { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? LastModifiedOn { get; set; }
        [Required]
        public ElectronicSignStatusEnum Status { get; set; } = ElectronicSignStatusEnum.Saved;
        public string StatusMsg { get; set; }
        public long IssuerId { get; set; }
        public string CertificateFile { get; set; }
        public string CertificatePass { get; set; }
        public DateTime? CertificateExpirationDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string PaymentResult { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public long? InvoiceId { get; set; }
        public string InvoiceResult { get; set; }
        public DateTime? RequestDate { get; set; }
        public string RequestResult { get; set; }
        public string RequestNumber { get; set; }        
        public DateTime? ReceivedDate { get; set; }   
        [Required]
        public SexoEnum Sexo { get; set; }
        [Required]
        public string BirthDate { get; set; }
        public string WorkPosition { get; set; }
        public bool? ActivateSubscription { get; set; }
        public long? LicenceTypeId { get; set; }       
        public string FingerPrintCode { get; set; }
        public string Email2 { get; set; }
        public string Phone2 { get; set; }
        public string AuthorizationFile { get; set; }
        public string AuthorizationAgeFile { get; set; }
    }

    public enum ElectronicSignStatusEnum : short
    {
        InValidation = -3,
        ValidatingPayment = -2,
        Error = -1,
        Saved = 0,
        Approved = 1,        
        Processed = 2,        
        
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
        File = 0, // para el nuevo proovedor
        File1 = 1, // proveedor actual
        Token = 1, // proveedor actual
        Token2 = 2, // para el nuevo proovedor
        Nube = 2 // para el nuevo proovedor
    }
    public enum VerificationTypeEnum:short
    {
        Office = 1,
        Skype = 2,
        Home = 3,
        External = 4,
        Other = 5,
        Unknown = 6,
        Ecuanexus = 7,
        Company = 8,
        Toni = 9
    }
    public enum SexoEnum : short
    {
        /// <summary>
        /// Masculino
        /// </summary>
        [EcuafactEnum("1", "HOMBRE")]
        Masculino = 1,

        /// <summary>
        /// Femenino
        /// </summary>
        [EcuafactEnum("1", "MUJER")]
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
    /// <summary>
    /// Resultado de Proceso de Firma Electronica.
    /// </summary>
    public class ElectronicSignFileRequest
    {
        /// <summary>
        /// id de acceso
        /// </summary>
        public int uid { get; set; }
        /// <summary>
        /// Token de Seguridad
        /// </summary>
        public string apikey { get; set; }
        /// <summary>
        /// token de la slocitud
        /// </summary>
        public string token { get; set; }        
        /// <summary>
        /// Archivo del certificado .p12 en formato codificado 64 (bytes[])
        /// </summary>
        public byte[] p12 { get; set; }
        /// <summary>
        /// La clave del certificado electronico
        /// </summary>
        public string pss { get; set; }        
    }

}
