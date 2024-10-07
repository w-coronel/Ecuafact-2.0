using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Ecuafact.Web.Domain.Entities
{
    public class SalesNoteRequestModel: DocumentRequestBase
    {
        public SalesNoteRequestModel()
        {
            IssuingId = 0;
            Payments = new List<PaymentModel>();            
            Details = new List<DocumentDetailModel>();
        }        
        public long? IssuingId { get; set; }
        [StringLength(9)]
        [Required(ErrorMessage = "El secuencial es requerido")]
        public string Sequential { get; set; }
        [Required(ErrorMessage = "El emisor es requerido")]
        public string IssuingRuc { get; set; }       
        public string IssuingBussinesName { get; set; }
        public string IssuingTradeName { get; set; }
        public string IssuingAddress { get; set; }
        public string IssuingEmailAddresses { get; set; }
        public string IssuingPhone { get; set; }
        [Required(ErrorMessage = "El número de autización del SRI es requerido")]
        public string AuthorizationNumber { get; set; }        
        public string DocumentTypeCode { get; set; }
        public string ReferralGuide { get; set; }       
        public decimal Subtotal { get; set; }
        public decimal TotalDiscount { get; set; } 
        public List<DocumentDetailModel> Details { get; set; }
        public List<PaymentModel> Payments { get; set; }
        public int Term { get; set; }
        public string TimeUnit { get; set; }
        public long ModifiedDocumentId { get; set; }
        public string IsSpecialContributor { get; set; }
        public bool IsAccountingRequired { get; set; }
        public string FileType { get; set; }
        [JsonIgnore]
        public HttpPostedFileBase DocumentPdfFile { get; set; }
        public byte[] DocumentPdfRaw => (documentPdfRaw ?? (documentPdfRaw = DocumentPdfFile.GetBytes()));
        private byte[] documentPdfRaw;

    }
}
