using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities
{
    /// <summary>
    /// Detalle de los impuestos para las Retenciones
    /// </summary>
    public class RetentionDetail
    {
        /// <summary>
        /// Id del Impuesto 
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
        /// Valor SubTotal del Documento Referencia para la Retencion
        /// </summary>
        public decimal? ReferenceDocumentAmount { get; set; }
        /// <summary>
        /// Valor Total del Documento Referencia para la Retencion
        /// </summary>
        public decimal? ReferenceDocumentTotal { get; set; }

        /// <summary>
        /// Numero de Autorizacion del Documento Referencia para la Retencion
        /// </summary>
        public string ReferenceDocumentAuth { get; set; }

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

    }
}
