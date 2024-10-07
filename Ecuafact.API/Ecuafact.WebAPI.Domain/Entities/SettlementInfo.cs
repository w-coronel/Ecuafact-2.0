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
    public class SettlementInfo
    {
        [ForeignKey("Document")]
        public long SettlementInfoId { get; set; }

        /// <summary>
        /// Fecha de Emision de la factura
        /// </summary>
        [MaxLength(10)]
        [Required]
        public string IssuedOn { get; set; }

        /// <summary>
        /// tipoIdentificacionComprador
        /// </summary>
        [MaxLength(2)]
        [Required]
        public string IdentificationType { get; set; }

        /// <summary>
        /// identificacion Comprador 
        /// </summary>
        [MaxLength(100)]
        [Required]
        public string Identification { get; set; }

        /// <summary>
        /// Razon Social Comprador
        /// </summary>
        [StringLength(300)]
        [Required]
        public string BussinesName { get; set; }

        /// <summary>
        /// Direccion Comprador
        /// </summary>
        [StringLength(300)]
        public string Address { get; set; }

        /// <summary>
        /// Moneda
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Subtotal Iva
        /// </summary>
        public decimal SubtotalVat { get; set; }

        /// <summary>
        /// Subtotal IVA 0
        /// </summary>
        public decimal SubtotalVatZero { get; set; }

        /// <summary>
        /// Subtotal No Objeto
        /// </summary>
        public decimal SubtotalNotSubject { get; set; }

        /// <summary>
        /// Subtotal Exento
        /// </summary>
        public decimal SubtotalExempt { get; set; }

        /// <summary>
        /// Subtotal
        /// </summary>
        public decimal Subtotal { get; set; }

        /// <summary>
        /// Total Descuento
        /// </summary>
        public decimal TotalDiscount { get; set; }

        /// <summary>
        /// ICE, Impuesto a los consumos especiales. Special Consum Tax
        /// </summary>
        public decimal SpecialConsumTax { get; set; }

        /// <summary>
        /// Valor del IVA
        /// </summary>
        public decimal ValueAddedTax { get; set; }
          
        /// <summary>
        /// Importe Total
        /// </summary>
        public decimal Total { get; set; }


        public List<DocumentDetail> Details { get; set; } = new List<DocumentDetail>();
        public List<TotalTax> TotalTaxes { get; set; } = new List<TotalTax>();
        public List<Payment> Payments { get; set; } = new List<Payment>();


        [JsonIgnore]
        public Document Document { get; set; }
    }
}
