using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Ecuafact.WebAPI.Domain.Entities;

namespace Ecuafact.WebAPI.Models
{
    /// <summary>
    /// Base para los Request de Documentos para el SRI
    /// </summary>
    public abstract class DocumentRequestBase
    {
        /// <summary>
        /// Fecha de Emision del Documento (REQUERIDO)
        /// </summary>
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string IssuedOn { get; set; }

        /// <summary>
        /// Tipo de identificacion del Contribuyente
        /// </summary>
        public string IdentificationType { get; set; } = "05";

        /// <summary>
        /// Identificacion del Contribuyente
        /// </summary>
        public string Identification { get; set; }

        /// <summary>
        /// Nombre del Contribuyente
        /// </summary>
        public string ContributorName { get; set; }

        /// <summary>
        /// Telefono del Contribuyente
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Direccion del Contribuyente
        /// </summary>
        public string Address { get; set; } = "";

        /// <summary>
        /// Correos a los que se enviara el documento electronico
        /// </summary>
        public string EmailAddresses { get; set; } = "";

        /// <summary>
        /// Moneda (DOLAR de forma predeterminada)
        /// </summary>
        public string Currency { get; set; } = "DOLAR";

        /// <summary>
        /// Observaciones
        /// </summary>
        public string Reason { get; set; } = "";

        /// <summary>
        /// Importe Total del Documento. Formato decimal 0.00
        /// </summary>
        public decimal Total { get; set; } = 0M;


        /// <summary>
        /// Campos adicionales
        /// </summary>
        public List<AdditionalFieldModel> AdditionalFields { get; set; }

        /// <summary>
        /// Emitir el documento
        /// </summary>
        public NewDocumentStatusEnum Status { get; set; }

        /// <summary>
        /// [USO INTERNO] Identificador Unico del Contribuyente 
        /// </summary>
        [Obsolete]
        public long ContributorId { get; set; }

        /// <summary>
        /// Codigo Establecimiento
        /// </summary>       
        public string EstablishmentCode { get; set; }

        /// <summary>
        /// Punto de Emisión del Comprobante
        /// </summary>        
        public string IssuePointCode { get; set; }

        /// <summary>
        /// Direccion del establecimiento
        /// </summary>        
        public string EstablishmentAddress { get; set; }
    }

    /// <summary>
    /// Base para los Request de Documentos que usan Referencia de Otros Documentos para el SRI
    /// </summary>
    public class ReferencedDocumentRequestBase : DocumentRequestBase
    {
        /// <summary>
        /// [USO INTERNO] identificador único del Documento Referencia (OPCIONAL Si se envian los datos)
        /// </summary>
        public long? ReferenceDocumentId { get; set; }
        /// <summary>
        /// Codigo del Tipo de Documento Referencia  (REQUERIDO)
        /// </summary>
        public string ReferenceDocumentCode { get; set; }
        /// <summary>
        /// Numero del Documento Referencia  (REQUERIDO)
        /// </summary>
        public string ReferenceDocumentNumber { get; set; }
        /// <summary>
        /// Fecha de Emision del Documento Referencia  (REQUERIDO)
        /// </summary>
        public string ReferenceDocumentDate { get; set; }

        /// <summary>
        /// Numero de Autorizacion del Documento Referencia (OPCIONAL)
        /// </summary>
        public string ReferenceDocumentAuth { get; set; }
        /// <summary>
        /// Valor Total del Documento Referencia  (OPCIONAL)
        /// </summary>
        public decimal ReferenceDocumentAmount { get; set; }
        /// <summary>
        /// Valor IVA del Documento Referencia (OPCIONAL)
        /// </summary>
        public decimal ReferenceDocumentVat { get; set; }
        /// <summary>
        /// Valor Total del Documento Referencia  (OPCIONAL)
        /// </summary>
        public decimal ReferenceDocumentTotal { get; set; }
        /// <summary>
        /// Subtotal IVA 0 Documento sustento   (OPCIONAL)
        /// </summary>
        public decimal? ReferenceDocumentSubtotalVatZero { get; set; }
        /// <summary>
        /// Subtotal No Objeto Documento sustento   (OPCIONAL)
        /// </summary>
        public decimal? ReferenceDocumentSubtotalNotSubject { get; set; }
        /// <summary>
        /// Subtotal Exento Documento sustento   (OPCIONAL)
        /// </summary>
        public decimal? ReferenceDocumentSubtotalExempt { get; set; }
        /// <summary>
        /// Subtotal del IVA Documento sustento   (OPCIONAL)
        /// </summary>
        public decimal? ReferenceDocumentSubtotalVat { get; set; }        
        /// <summary>
        /// Total Impuestos documento soporte
        /// </summary>
        public List<TotalTaxModel> ReferenceDocumentTotalTax { get; set; }
        /// <summary>
        /// Tipos de pagos del documento soporte
        /// </summary> 
        public List<PaymentModel> ReferenceDocumentPayments { get; set; }
       
    }
}