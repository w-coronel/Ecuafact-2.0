using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Ecuafact.WebAPI.Models
{
    /// <summary>
    /// DOCUMENTO: FACTURA
    /// </summary>
    public class InvoiceRequestModel: DocumentRequestBase
    {
        /// <summary>
        /// Subtotal Iva. Formato decimal 0.00
        /// </summary>
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public decimal SubtotalVat { get; set; }

        /// <summary>
        /// Subtotal IVA 0. Formato decimal 0.00
        /// </summary>
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public decimal SubtotalVatZero { get; set; }

        /// <summary>
        /// Subtotal No Objeto. Formato decimal 0.00
        /// </summary>
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public decimal SubtotalNotSubject { get; set; }

        /// <summary>
        /// Subtotal Exento. Formato decimal 0.00
        /// </summary>
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public decimal SubtotalExempt { get; set; }

        /// <summary>
        /// Subtotal. Formato decimal 0.00
        /// </summary>
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public decimal Subtotal { get; set; }

        /// <summary>
        /// Total Descuento. Formato decimal 0.00
        /// </summary>
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public decimal TotalDiscount { get; set; }

        /// <summary>
        /// ICE, Impuesto a los consumos especiales. Special Consum Tax. Formato decimal 0.00
        /// </summary>
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public decimal SpecialConsumTax { get; set; }

        /// <summary>
        /// Valor del IVA. Formato decimal 0.00
        /// </summary>
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public decimal ValueAddedTax { get; set; }

        /// <summary>
        /// Propina. Formato decimal 0.00
        /// </summary>
        public decimal Tip { get; set; }

        /// <summary>
        /// Detalle del metodo de pago para la factura. NOTA: Si no es especificado se asume EFECTIVO.
        /// </summary>
        public List<PaymentModel> Payments { get; set; }

        /// <summary>
        /// Detalle de productos de la factura
        /// </summary>
        public List<InvoiceDetailModel> Details { get; set; }

        /// <summary>
        /// Guia de Remision (OPCIONAL)
        /// </summary>
        public string ReferralGuide { get; set; }

        /// <summary>
        /// Orden de Compra
        /// </summary>
        public int? PurchaseOrderId { get; set; }

    }

    /// <summary>
    /// Detalle de la Factura
    /// </summary>
    public class InvoiceDetailModel : DocumentDetailModel
    {

    }


    /// <summary>
    /// Factura de Exportacion
    /// </summary>
    public class ExportInvoiceRequestModel : InvoiceRequestModel
    {

        /// <summary>
        /// incoTermFactura
        /// </summary>
        [MaxLength(10)]
        public string InvoiceIncoTerm { get; set; }

        /// <summary>
        /// lugarIncoTerm
        /// </summary>
        [MaxLength(300)]
        public string PlaceIncoTerm { get; set; }

        /// <summary>
        /// PaisOrigen
        /// </summary>
        [MaxLength(3)]
        public string OriginCountry { get; set; }

        /// <summary>
        /// PuertoEmbarque
        /// </summary>
        [MaxLength(300)]
        public string PortBoarding { get; set; }

        /// <summary>
        /// PuertoDestino
        /// </summary>
        [MaxLength(300)]
        public string DestinationPort { get; set; }

        /// <summary>
        /// Pais Destino
        /// </summary>
        [MaxLength(3)]
        public string DestinationCountry { get; set; }

        /// <summary>
        /// PaisAdquisicion
        /// </summary>
        [MaxLength(3)]
        public string CountryAcquisition { get; set; }

        /// <summary>
        /// incoTermTotalSinImpuestos 
        /// </summary>
        [MaxLength(10)]
        public string TotalWithoutTaxesIncoTerm { get; set; }

    }
}