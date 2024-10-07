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

    /// <summary>
    /// Detalle de los motivos o razon de la nota debito
    /// </summary>
    public class DebitNoteDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [JsonIgnore]
        public long Id { get; set; }

        /// <summary>
        /// Motivo o razon de la nota debito 
        /// </summary>
        [MaxLength(300)]
        public string Reason { get; set; }

        /// <summary>
        /// Valor del motivo o razon de la nota debito
        /// </summary>        
        public decimal Value { get; set; }

        /// <summary>
        /// Codigo del Tipo de Impuesto (IVA 0%, 12%, 14% y objeto de iva)
        /// </summary>
        public string TaxCode { get; set; }


        /// <summary>
        /// Codigo del Tipo de Impuesto (IVA 0%, 12%, 14% y objeto de iva)
        /// </summary>
        public string PercentageCode { get; set; }


        /// <summary>
        /// Porcentaje del impuesto (IVA) 
        /// </summary>
        public decimal TaxRate { get; set; }

        /// <summary>
        /// Valor del Impuesto (IVA)
        /// </summary>
        public decimal TaxValue { get; set; }


        /// <summary>
        /// Subtotal 
        /// </summary>
        public decimal SubTotal { get; set; }


        /// <summary>
        /// ID del Documento de la nota debito
        /// </summary>
        public long DebitNoteInfoId { get; set; }

        /// <summary>
        /// Documento
        /// </summary>
        [JsonIgnore]
        [ForeignKey("DebitNoteInfoId")]
        public DebitNoteInfo DebitNoteInfo { get; set; }
    }
}
