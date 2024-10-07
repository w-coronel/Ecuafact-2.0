using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecuafact.WebAPI.Models
{
    /// <summary>
    /// DOCUMENTO: NOTA DE CREDITO
    /// </summary>
    public class CreditNoteRequestModel : ReferencedDocumentRequestBase
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
        /// Valor Modificado. Formato decimal 0.00
        /// </summary>
        public decimal ModifiedValue { get; set; }
        

        /// <summary>
        /// Detalle de la Nota de Credito
        /// </summary>
        public List<CreditNoteDetailModel> Details { get; set; }

        /// <summary>
        /// Detalle del metodo de pago para la factura. NOTA: Si no es especificado se asume EFECTIVO.
        /// </summary>
        public List<PaymentModel> Payments { get; set; }
    }

    /// <summary>
    /// Detalle de la Nota de Credito
    /// </summary>
    public class CreditNoteDetailModel : DocumentDetailModel
    {
    }
}