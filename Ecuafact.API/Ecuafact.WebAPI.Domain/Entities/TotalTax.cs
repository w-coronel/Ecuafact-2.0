using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class TotalTax
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }

        /// <summary>
        /// Codigo del Impuesto
        /// </summary>
        [MaxLength(1)]
        public string TaxCode { get; set; }

        /// <summary>
        /// Codigo del Porcentaje de Impuesto
        /// </summary>
        [MaxLength(5)]
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

        [JsonIgnore]
        public Document Document { get; set; }

        [JsonIgnore]
        public InvoiceInfo InvoiceInfo { get; set; }

        [JsonIgnore]
        public CreditNoteInfo CreditNoteInfo { get; set; }

        [JsonIgnore]
        public SettlementInfo SettlementInfo { get; set; }

        [JsonIgnore]
        public DebitNoteInfo DebitNoteInfo { get; set; }

        [JsonIgnore]
        public RetentionInfo RetentionInfo { get; set; }

    }
}
