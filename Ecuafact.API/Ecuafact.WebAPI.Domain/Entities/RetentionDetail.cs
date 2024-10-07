using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Ecuafact.WebAPI.Domain.Entities
{
    /// <summary>
    /// Detalle de los impuestos para las Retenciones
    /// </summary>
    public class RetentionDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [JsonIgnore]
        public long Id { get; set; }

        /// <summary>
        /// Id del Impuesto - Debe existir en la base de datos
        /// </summary>
        public long RetentionTaxId { get; set; }

        /// <summary>
        /// Codigo del Tipo de Impuesto (IVA, Renta, ISD)
        /// </summary>
        public string TaxTypeCode { get; set; }

        /// <summary>
        /// Codigo del Impuesto segun el SRI
        /// </summary>
        public string RetentionTaxCode { get; set; }

        /// <summary>
        /// Base Imponible de la Retencion
        /// </summary>
        public decimal TaxBase { get; set; }

        /// <summary>
        /// Porcentaje de la Retencion
        /// </summary>
        public decimal TaxRate { get; set; }

        /// <summary>
        /// Valor de la Retencion
        /// </summary>
        public decimal TaxValue { get; set; }

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
        /// Valor SubTotal del Documento Referencia para la Retencion
        /// </summary>
        public decimal? ReferenceDocumentAmount { get; set; }

        /// <summary>
        /// Valor Total del Documento Referencia para la Retencion
        /// </summary>
        public decimal? ReferenceDocumentTotal { get; set; }

        /// <summary>
        /// ID del Documento de Retencion
        /// </summary>
        public long RetentionInfoId { get; set; }

        /// <summary>
        /// Tipo de Sustento del Comprobante
        /// </summary>
        public string SupportCode { get; set; }

        /// <summary>
        /// Fecha Registro Contable
        /// </summary>
        public string AccountingRegistrationDate { get; set; }

        /// <summary>
        /// Pago a Residente o no Residente
        /// </summary>
        public string PaymentResident { get; set; }

        /// <summary>
        /// Documento
        /// </summary>
        [JsonIgnore]
        [ForeignKey("RetentionInfoId")]
        public RetentionInfo RetentionInfo { get; set; }
    }
}
