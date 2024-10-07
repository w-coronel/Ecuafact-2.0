using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities
{    
    // <summary>
    /// DOCUMENTO: NOTA DEBITO
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
        /// Valor del IVA. Formato decimal 0.00
        /// </summary>
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public decimal ValueAddedTax { get; set; }
       
        /// <summary>
        /// Valor Modificado. Formato decimal 0.00
        /// </summary>
        public decimal ModifiedValue { get; set; }


        /// <summary>
        /// Motivo de la Nota de Dedito
        /// </summary>
        public List<DebitNoteDetailModel> Details { get; set; }

        public List<PaymentModel> Payments { get; set; } = new List<PaymentModel>();

        public string DocumentTypeCode { get; set; }
        public int Term { get; set; }
        public string TimeUnit { get; set; }


    }   

}
