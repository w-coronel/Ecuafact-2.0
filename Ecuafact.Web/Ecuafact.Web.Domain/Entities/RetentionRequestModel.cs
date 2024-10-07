using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Ecuafact.Web.Domain.Entities
{
    /// <summary>
    /// DOCUMENTO: COMPROBANTE DE RETENCION
    /// </summary>
    public class RetentionRequestModel : ReferencedDocumentRequestBase
    {
        /// <summary>
        /// Periodo fiscal
        /// </summary>
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string FiscalPeriod { get; set; }

        /// <summary>
        /// Importe Total. Formato decimal 0.00
        /// </summary>
        public decimal FiscalAmount { get; set; }


        /// <summary>
        /// Importe Total del Documento. Formato decimal 0.00
        /// </summary>

        public decimal Balance { get; set; } = 0M;

        /// <summary>
        /// Total Impuestos
        /// </summary>
        public List<RetentionDetailModel> Details { get; set; }

        public string DocumentTypeCode { get; set; }

        /// <summary>
        /// Tipo de Sustento del Comprobante 
        /// </summary>
        [Required(ErrorMessage = "El campo Tipo de sustento es requerido")]
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
        /// Parte Relacionada
        /// </summary>
        public string RelatedParty { get; set; }

        /// <summary>
        /// Tipo Sujeto Retenido
        /// </summary>
        public string RetentionObjectType { get; set; }

    }


    /// <summary>
    /// Detalle de los impuestos para las Retenciones
    /// </summary>
    public class RetentionDetailModel
    {
        /// <summary>
        /// Id del Impuesto - Debe existir en la base de datos
        /// </summary>
        public long? RetentionTaxId { get; set; }

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
        public decimal ReferenceDocumentAmount { get; set; }

        /// <summary>
        /// Valor Total del Documento Referencia para la Retencion
        /// </summary>
        public decimal? ReferenceDocumentTotal { get; set; }

        /// <summary>
        /// Nombre del Impuesto - Debe existir en la base de datos
        /// </summary>
        public string RetentionTaxName { get; set; }

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

    public class DocumentSupportXml
    {
        public string Token { get; set; }
        public string Name { get; set; }
        public HttpPostedFileBase XmlRaw { get; set; }
    }



}
