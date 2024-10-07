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
    public class DocumentDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }

        public long? ProductId { get; set; }

        [StringLength(500)]
        public string MainCode { get; set; }

        [StringLength(500)]
        public string AuxCode { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Cantidad
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Precio Unitario
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Descuento
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// Total del detalle sin impuestos
        /// </summary>
        public decimal SubTotal { get; set; }


        /// <summary>
        /// Impuestos
        /// </summary>
        public List<Tax> Taxes { get; set; }

        [MaxLength(300)]
        public string Name1 { get; set; }

        [MaxLength(300)]
        public string Value1 { get; set; }

        [MaxLength(300)]
        public string Name2 { get; set; }

        [MaxLength(300)]
        public string Value2 { get; set; }

        [MaxLength(300)]
        public string Name3 { get; set; }

        [MaxLength(300)]
        public string Value3 { get; set; }
        public long DocumentId { get; set; }
        public long? InvoiceInfoId { get; set; }
        public long? CreditNoteInfoId { get; set; }
        public long? SettlementInfoId { get; set; }

        [JsonIgnore]
        public Document Document { get; set; }

        [JsonIgnore]
        public InvoiceInfo InvoiceInfo { get; set; }

        [JsonIgnore]
        public CreditNoteInfo CreditNoteInfo { get; set; }

        [JsonIgnore]
        public SettlementInfo SettlementInfo { get; set; }

     
    }
}
