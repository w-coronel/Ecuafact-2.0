using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities
{
    public class ElectronicSignModel
    {
        public int Id { get; set; }

        public string RUC { get; set; }
        
        public string Identification { get; set; }

        public string BusinessName { get; set; }
        public string BusinessAddress { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        public string Email { get; set; }
          
        public string Phone { get; set; }
        public string Email2 { get; set; }
        public string Phone2 { get; set; }

        public string Address { get; set; }
        public string Skype { get; set; }
       
        public SignatureValidyEnum SignatureValidy { get; set; }
       
        public VerificationTypeEnum VerificationType { get; set; }
       
        public FileFormatEnum FileFormat { get; set; }

        public RucTypeEnum SignType { get; set; }
       
        public IdentificationTypeEnum DocumentType { get; set; }
       
        public string IdentificationCardFile { get; set; }
       
        public string ElectionsTicketFile { get; set; }
       
        public string RucFile { get; set; }
       
        public string AuthorizationLegalRepFile { get; set; }
       
        public string DesignationFile { get; set; }
       
        public string ConstitutionFile { get; set; }
         
        public int PurchaseOrderId { get; set; }

        public PurchaseOrderModel PurchaseOrder { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public DateTime? LastModifiedOn { get; set; }
       
        public ElectronicSignStatusEnum Status { get; set; }

        public string StatusMsg { get; set; }


        public string CertificateFile { get; set; }
        public string CertificatePass { get; set; }
        public DateTime? CertificateExpirationDate { get; set; }


        public DateTime? PaymentDate { get; set; }
        public string PaymentResult { get; set; }

        public DateTime? InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public long? InvoiceId { get; set; }

        public DateTime? RequestDate { get; set; }
        public string RequestResult { get; set; }

        public DateTime? ReceivedDate { get; set; }

        public string City { get; set; }

        public string Province { get; set; }

        public string Country { get; set; }

        public string CedulaFrontFile { get; set; }        
        public string CedulaBackFile { get; set; }       
        public string SelfieFile { get; set; }
        public SexoEnum Sexo { get; set; }       
        public string BirthDate { get; set; }
        public string WorkPosition { get; set; }
        public bool SendRequest { get; set; } = false;
        public string FingerPrintCode { get; set; }
        public string AuthorizationAgeFile { get; set; }        
    }

    public enum ElectronicSignStatusEnum : short
    {
        ValidatingPayment = -2,
        Error = -1,
        Saved = 0,
        Approved = 1,
        Processed = 2,      
    }

    public class SubscriptionModel
    {
        public long Id { get; set; }
        public string RUC { get; set; }
        public long IssuerId { get; set; }
        public long LicenceId { get; set; }
        public SubscriptionStatusEnum Status { get; set; }
        public string StatusMsg { get; set; }
        public IssuerDto Issuer { get; set; }
        public bool Notify { get; set; }
        public long Day { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionExpirationDate { get; set; }
        public LicenceType LicenceType { get; set; }
        public string AmountDocument { get; set; }
        public int? IssuedDocument { get; set; }
        public int? BalanceDocument { get; set; }
        public int? AmountIssuePoint { get; set; }
        public int? RequestElectronicSign { get; set; }        
    }  

}
