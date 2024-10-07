using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ecuafact.WebAPI.Domain.Entities
{
    /// <summary>
    /// Modelo de la Base de datos para la Retencion
    /// </summary>
    public class RetentionInfo
    {
        /// <summary>
        /// ID del Documento de Retencion
        /// </summary>
        [ForeignKey("Document")]
        public long RetentionInfoId { get; set; }

        /// <summary>
        /// Fecha de Emision de la Retencion
        /// </summary>
        [Required]
        [StringLength(10)]
        public string IssuedOn { get; set; }

        /// <summary>
        /// ID Contribuyente del Proveedor al cual se le aplica la retencion
        /// </summary>
        [ForeignKey("Contributor")]
        public long? ContributorId { get; set; }

        /// <summary>
        /// Tipo de Identificacion del Proveedor
        /// </summary>
        [MaxLength(2), Required] public string IdentificationType { get; set; }

        /// <summary>
        /// Numero de Identificacion para el Proveedor
        /// </summary>
        [MaxLength(100), Required] public string Identification { get; set; }

        /// <summary>
        /// Razon Social del Proveedor
        /// </summary>
        [MaxLength(300), Required] public string BusinessName { get; set; }

        /// <summary>
        /// Moneda ("DOLAR" de forma Predeterminada)
        /// </summary>
        [MaxLength(10), Required] public string Currency { get; set; }

        /// <summary>
        /// Motivo de la Retencion
        /// </summary>
        [MaxLength(500), Required] public string Reason { get; set; }

        /// <summary>
        /// Periodo fiscal de la Retencion
        /// </summary>
        [MaxLength(7), Required] public string FiscalPeriod { get; set; }

        /// <summary>
        /// Monto del Valor fiscal Total de la Retencion
        /// </summary>
        [Required] public decimal FiscalAmount { get; set; }

        /// <summary>
        /// Detalles de la Retencion
        /// </summary>
        public List<RetentionDetail> Details { get; set; } = new List<RetentionDetail>();

        /// <summary>
        /// Contribuyente
        /// </summary>
        [JsonIgnore]
        public virtual Contributor Contributor { get; set; }

        /// <summary>
        /// Documento
        /// </summary>
        [JsonIgnore]
        public virtual Document Document { get; set; }

        /// <summary>
        /// Identificador del Documento Referencia para la Retencion
        /// </summary>
        public long? ReferenceDocumentId { get; set; }
        
        /// <summary>
        /// Codigo del Tipo de Documento Referencia para la Retencion
        /// </summary>
        public string ReferenceDocumentCode { get; set; }
        /// <summary>
        /// Numero del Documento Referencia para la Retencion
        /// </summary>
        public string ReferenceDocumentNumber { get; set; }
        /// <summary>
        /// Fecha de Emision del Documento Referencia para la Retencion
        /// </summary>
        public string ReferenceDocumentDate { get; set; }
        /// <summary>
        /// Numero de Autorizacion del Documento Referencia para la Retencion
        /// </summary>
        public string ReferenceDocumentAuth { get; set; }
        /// <summary>
        /// Valor total sin impuestos del Documento Referencia para la Retencion
        /// </summary>
        public decimal ReferenceDocumentAmount { get; set; }
        /// <summary>
        /// Valor IVA del Documento Referencia para la Retencion
        /// </summary>
        public decimal ReferenceDocumentVat { get; set; }
        /// <summary>
        /// Valor Total del Documento Referencia para la Retencion
        /// </summary>
        public decimal? ReferenceDocumentTotal { get; set; }
        /// <summary>
        /// Subtotal IVA 0 Documento sustento 
        /// </summary>
        public decimal? ReferenceDocumentSubtotalVatZero { get; set; }
        /// <summary>
        /// Subtotal No Objeto Documento sustento   
        /// </summary>
        public decimal? ReferenceDocumentSubtotalNotSubject { get; set; }
        /// <summary>
        /// Subtotal Exento Documento sustento   
        /// </summary>
        public decimal? ReferenceDocumentSubtotalExempt { get; set; }
        /// <summary>
        /// Subtotal IVA Documento sustento   
        /// </summary>
        public decimal? ReferenceDocumentSubtotalVat { get; set; }
        /// <summary>
        /// Tipo de Sustento del Comprobante
        /// </summary>
        public string SupportCode { get; set; }
        /// <summary>
        /// Fecha Registro Contable
        /// </summary>
        public string AccountingRegistrationDate { get; set; }
        /// <summary>
        /// Parte Relacionada
        /// </summary>
        public string RelatedParty { get; set; }
        /// <summary>
        /// Pago a Residente o no Residente
        /// </summary>
        public string PaymentResident { get; set; }
        /// <summary>
        /// Tipo subjeto retenido
        /// </summary>
        public string RetentionObjectType { get; set; }        

        public List<TotalTax> TotalTaxes { get; set; } = new List<TotalTax>();

        public List<Payment> Payments { get; set; } = new List<Payment>();
    }
}
