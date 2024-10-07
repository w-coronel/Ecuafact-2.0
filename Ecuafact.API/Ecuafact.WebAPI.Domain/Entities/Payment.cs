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
    public class Payment
    {
        [Key, Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// Codigo de la Forma de Pago
        /// </summary>
        [MaxLength(2)]
        public string PaymentMethodCode { get; set; }

        /// <summary>
        /// Forma de Pago
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Valor total
        /// </summary>
        public decimal? Total { get; set; }

        /// <summary>
        /// Plazo
        /// </summary>
        public int? Term { get; set; }

        /// <summary>
        /// Unidad de tiempo
        /// </summary>
        [MaxLength(30)]
        public string TimeUnit { get; set; }

        /// <summary>
        /// ID del Documento
        /// </summary>
        public long DocumentId { get; set; }

        /// <summary>
        /// Relacion con factura
        /// </summary>
        public long? InvoiceInfoId { get; set; }

        /// <summary>
        /// Relacion con Nota de Credito
        /// </summary>
        public long? CreditNoteInfoId { get; set; }

        /// <summary>
        /// Liquidacion de Compra
        /// </summary>
        public long? SettlementInfoId { get; set; }


        /// <summary>
        /// Relacion con Nota de Debito
        /// </summary>
        public long? DebitNoteInfoId { get; set; }

        /// <summary>
        /// Relacion con Retención
        /// </summary>
        public long? RetentionInfoId { get; set; }

        

        /// <summary>
        /// Documento
        /// </summary>
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
