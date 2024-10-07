using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities
{
    public class SettlementRequestModel : DocumentRequestBase
    {
        public SettlementRequestModel()
        {
            ContributorId = 0;
            Payments = new List<PaymentModel>();
            AdditionalFields = new List<AdditionalFieldModel>();
            Details = new List<DocumentDetailModel>();
        }


        public string DocumentTypeCode { get; set; }
        public string ReferralGuide { get; set; }
        public decimal SubtotalVat { get; set; }
        public decimal SubtotalVatZero { get; set; }
        public decimal SubtotalNotSubject { get; set; }
        public decimal SubtotalExempt { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal SpecialConsumTax { get; set; }
        public decimal ValueAddedTax { get; set; }
        public decimal Tip { get; set; }

        public List<DocumentDetailModel> Details { get; set; }
        public List<PaymentModel> Payments { get; set; }

        public int Term { get; set; }
        public string TimeUnit { get; set; }
        public long ModifiedDocumentId { get; set; }

    }
}
