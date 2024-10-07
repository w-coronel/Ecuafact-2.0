using Ecuafact.WebAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ecuafact.WebAPI.Models
{
    public class ElectronicSignRequest
    {
        [MaxLength(13)]
        [Required]
        public string Identification { get; set; }
        [MaxLength(300)]
        [Required]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(300)]
        public string LastName { get; set; }        
        [Required]
        [RegularExpression(Constants.MultipleEmailRegex, ErrorMessage = "Debe ingresar un email válido")]
        public string Email { get; set; }        
        [Required]
        [RegularExpression(Constants.MultiplePhoneRegex, ErrorMessage ="Debe ingresar un numero de telefono válido")]
        public string Phone { get; set; }
        [MaxLength(300)]
        [Required]
        public string Address { get; set; }    
        public string FingerPrintCode { get; set; }
        [MaxLength(20)]
        [Required]
        [Phone(ErrorMessage = "Debe Ingresar un numero de RUC válido")]
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
        public byte[] CedulaFrontRaw { get; set; }
        [Required]
        public byte[] CedulaBackRaw { get; set; }
        [Required]
        public byte[] SelfieRaw { get; set; }
        [Required]
        public byte[] IdentificationCardRaw { get; set; }
        [Required]
        public byte[] RucRaw { get; set; }
        [Required]
        public byte[] AuthorizationLegalRepRaw { get; set; }
        [Required]
        public byte[] DesignationRaw { get; set; }
        [Required]
        public byte[] ConstitutionRaw { get; set; }
        [Required]
        public byte[] ElectionsTicketRaw { get; set; }
        public byte[] AuthorizationRaw { get; set; }
        public byte[] AuthorizationAgeRaw { get; set; }        
        public SexoEnum Sexo { get; set; }
        public string BirthDate { get; set; }
        public string WorkPosition { get; set; }        
        public ElectronicSignInvoice InvoiceInfo { get; set; }
        public long? LicenceTypeId { get; set; }       
        [RegularExpression(Constants.MultipleEmailRegex, ErrorMessage = "Debe ingresar un email válido")]
        public string Email2 { get; set; }       
        [RegularExpression(Constants.MultiplePhoneRegex, ErrorMessage = "Debe ingresar un número de telefono válido")]
        public string Phone2 { get; set; }
        public string AuthorizationAgeFormat { get; set; }
    }


    public class ElectronicSignInvoice
    { 
        public string Identification { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        [RegularExpression(Constants.MultipleEmailRegex, ErrorMessage = "Debe ingresar un email válido")]
        public string Email { get; set; }
        [RegularExpression(Constants.MultiplePhoneRegex, ErrorMessage = "Debe ingresar un número de telefono válido")]
        public string Phone { get; set; }
        public string DiscountCode { get; set; }
        public decimal Price { get; set; }
        public string Product { get; set; }

    }

    public class CertificateConf
    {
        public string Ruc { get; set; }
        public string CertificatePass { get; set; }        
        public byte[] CertificateRaw { get; set; }
    }

}