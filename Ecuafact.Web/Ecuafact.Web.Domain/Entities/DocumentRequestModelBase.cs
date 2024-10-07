using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecuafact.Web.Domain.Entities
{

    /// <summary>
    /// Base para los Request de Documentos para el SRI
    /// </summary>
    public abstract class DocumentRequestBase
    {
        public long? Id { get; set; }

        public string DocumentNumber { get; set; }

        /// <summary>
        /// Fecha de Emision del Documento (REQUERIDO)
        /// </summary>
        [Required(ErrorMessage = "El campo Fecha de emisión es requerido")]        
        public string IssuedOn { get; set; } = DateTime.Today.ToString("dd/MM/yyyy");

        /// <summary>
        /// ID de la lista de Contribuyentes (Opcional, si no existe se lo crea automaticamente con la informacion enviada)
        /// </summary>
        public long? ContributorId { get; set; }

        /// <summary>
        /// Tipo de identificacion del Contribuyente
        /// </summary>      
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string IdentificationType { get; set; } = "05";

        /// <summary>
        /// Identificacion del Contribuyente
        /// </summary>        
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Identification { get; set; }

        /// <summary>
        /// Nombre del Contribuyente
        /// </summary>
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string ContributorName { get; set; }

        /// <summary>
        /// Telefono del Contribuyente
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Direccion del Contribuyente
        /// </summary>
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Address { get; set; } = "";

        /// <summary>
        /// Correos a los que se enviara el documento electronico
        /// </summary>
        public string EmailAddresses { get; set; } = "";

        /// <summary>
        /// Moneda (DOLAR de forma predeterminada)
        /// </summary>
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Currency { get; set; } = "DOLAR";

        /// <summary>
        /// Observaciones
        /// </summary>
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Reason { get; set; } = "";

        /// <summary>
        /// Importe Total del Documento. Formato decimal 0.00
        /// </summary>
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public decimal Total { get; set; } = 0M;


        /// <summary>
        /// Campos adicionales
        /// </summary>
        public List<AdditionalFieldModel> AdditionalFields { get; set; }


        /// <summary>
        /// Estado del documento
        /// </summary>
        public DocumentStatusEnum? Status { get; set; }

        /// <summary>
        /// Codigo Establecimiento
        /// </summary>   
        [Required(ErrorMessage = "El campo Establecimiento es requerido")] 
        public string EstablishmentCode { get; set; }

        /// <summary>
        /// Punto de Emisión del Comprobante
        /// </summary>   
        [Required(ErrorMessage = "El campo Punto de Emisión es requerido")]
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
        /// ID del Documento Referencia (OPCIONAL Si se envian los datos)
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
        /// Valor Subtotal del Documento Referencia  (OPCIONAL)
        /// </summary>
        public decimal ReferenceDocumentAmount { get; set; }
        /// <summary>
        /// Valor IVA del Documento Referencia (OPCIONAL)
        /// </summary>
        public decimal ReferenceDocumentVat { get; set; }
        /// <summary>
        /// Valor Total del Documento Referencia  (OPCIONAL)
        /// </summary>
        public decimal? ReferenceDocumentTotal { get; set; }
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
        /// Subtotal IVA Documento sustento   (OPCIONAL)
        /// </summary>
        public decimal? ReferenceDocumentSubtotalVat { get; set; }
        /// <summary>
        /// Total Impuestos documento Referencia (OPCIONAL)
        /// </summary>
        public List<TotalTaxModel> ReferenceDocumentTotalTax { get; set; }
        /// <summary>
        /// tipos de pagos del documento Referencia (OPCIONAL)
        /// </summary> 
        public List<PaymentModel> ReferenceDocumentPayments { get; set; }
    }

    /// <summary>
    /// Detalle de los Documentos
    /// </summary>
    public class DocumentDetailModel
    {

        /// <summary>
        /// Id del Producto
        /// </summary>
        public long? ProductId { get; set; }

        /// <summary>
        /// Codigo Principal del Producto
        /// </summary>
        [StringLength(500)]
        public string MainCode { get; set; }

        /// <summary>
        /// Codigo Auxiliar del Producto
        /// </summary>
        [StringLength(500)]
        public string AuxCode { get; set; }

        /// <summary>
        /// Descripcion del Producto
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Precio Unitario
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Cantidad
        /// </summary>
        public decimal Amount { get; set; }

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
        public List<TaxModel> Taxes { get; set; }

        /// <summary>
        /// Nombre del Campo del Detalle Adicional
        /// </summary>
        [MaxLength(300)]
        public string Name1 { get; set; }

        /// <summary>
        /// Valor del Campo del Detalle Adicional
        /// </summary>
        [MaxLength(300)]
        public string Value1 { get; set; }

        /// <summary>
        /// Nombre del Campo del Detalle Adicional
        /// </summary>
        [MaxLength(300)]
        public string Name2 { get; set; }

        /// <summary>
        /// Valor del Campo del Detalle Adicional
        /// </summary>

        [MaxLength(300)]
        public string Value2 { get; set; }

        /// <summary>
        /// Nombre del Campo del Detalle Adicional
        /// </summary>
        [MaxLength(300)]
        public string Name3 { get; set; }

        /// <summary>
        /// Valor del Campo del Detalle Adicional
        /// </summary>

        [MaxLength(300)]
        public string Value3 { get; set; }

        /// <summary>
        /// Codigo del Impuesto al Valor Agregado (IVA) que se aplica al producto
        /// </summary>
        public string ValueAddedTaxCode { get; set; }

        /// <summary>
        /// Valor del Impuesto al Valor Agregado (IVA) que se aplica al producto
        /// </summary>
        public decimal ValueAddedTaxValue { get; set; }

        /// <summary>
        /// Codigo del Impuesto a los Consumos Especiales (ICE) en caso que aplique
        /// </summary>
        public string SpecialConsumTaxCode { get; set; }

        /// <summary>
        /// Valor del Impuesto a los Consumos Especiales (ICE) en caso que aplique
        /// </summary>
        public decimal SpecialConsumTaxValue { get; set; }

    }
        
}