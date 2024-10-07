using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities
{
    public class TotalTaxModel
    {
        public string TaxCode { get; set; }
        public string PercentageTaxCode { get; set; }
        public decimal TaxableBase { get; set; }
        public decimal TaxValue { get; set; }
        public decimal TaxRate { get; set; }
        public decimal AditionalDiscount { get; set; }         

    }

    public class TotalTax
    {
        public long Id { get; set; }
        public string TaxCode { get; set; }
        public string PercentageTaxCode { get; set; }
        public decimal TaxableBase { get; set; }
        public decimal TaxValue { get; set; }
        public decimal TaxRate { get; set; }
        public decimal AditionalDiscount { get; set; }
        public long DocumentId { get; set; }
        public long? InvoiceInfoId { get; set; }
        public long? CreditNoteInfoId { get; set; }
        public long? SettlementInfoId { get; set; }
        public long? DebitNoteInfoId { get; set; }
        public long? RetentionInfoId { get; set; }

    }
}
