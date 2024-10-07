
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ecuafact.WebAPI.Models
{/// <summary>
 /// DOCUMENTO: NOTA DE DEBITO
 /// </summary>
    public class DebitNoteRequestModel : ReferencedDocumentRequestBase
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
        /// Valor Modificado. Formato decimal 0.00
        /// </summary>
        public decimal ModifiedValue { get; set; }


        /// <summary>
        /// Razon o motivo de la Nota de Debito
        /// </summary>
        public List<DebitNoteDetailModel> Details { get; set; }

        /// <summary>
        /// Detalle del metodo de pago para la nota debito. NOTA: Si no es especificado se asume EFECTIVO.
        /// </summary>
        public List<PaymentModel> Payments { get; set; }
    }

    /// <summary>
    /// Razon o motivo de la Nota de Débito
    /// </summary>
    public class DebitNoteDetailModel 
    {

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
        /// Codigo del Tipo de Impuesto (IVA)
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

        public decimal SubTotal { get; set; }
    }
}