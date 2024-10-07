using Ecuafact.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecuafact.WebAPI.Models
{
    /// <summary>
    /// Modelo de datos para documentos genericos
    /// </summary>
    public class DocumentRequestModel : ReferencedDocumentRequestBase
    { 
        
        /// <summary>
        /// Código del Documento a Emitir (Tabla 3 Ficha Tecnica)
        /// </summary>
        [StringLength(2)]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string DocumentTypeCode { get; set; }
 
        /// <summary>
        /// Guia de Remision
        /// </summary>
        public string ReferralGuide { get; set; }

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

        /// <summary>
        /// Pagos
        /// </summary>
        public List<PaymentModel> Payments { get; set; }
          
        /// <summary>
        /// Detalle
        /// </summary>
        public List<CustomDocumentDetailModel> Details { get; set; }        

    }

    /// <summary>
    /// Modelo de datos para documentos genericos [OBSOLETO] | v1.0
    /// </summary>
    public class CustomDocumentRequestModel : DocumentRequestModel
    {
        /// <summary>
        /// Tipo de Identificacion del Contribuyente [OBSOLETO] - Reemplazado por "IdentificationType"
        /// </summary>
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string ContributorType { get; set; }

        /************************************************************************************/
        /// <summary>
        /// Tipo de Identificacion del Contribuyente [OBSOLETO] - Reemplazado por "ReferenceDocumentCode"
        /// </summary>
        public string ModifiedDocumentCode { get; set; }
        /// <summary>
        /// Numero del Documento Soporte [OBSOLETO] - Reemplazado por "ReferenceDocumentNumber"
        /// </summary>
        public string ModifiedDocumentNumber { get; set; }
        /// <summary>
        /// Fecha de Emision del Documento Soporte [OBSOLETO] - Reemplazado por "ReferenceDocumentDate"
        /// </summary>
        public string SupportDocumentIssueDate { get; set; }
    }

    /// <summary>
    /// Detalle de los documentos genericos [OBSOLETO] | v1.0
    /// </summary>
    public class CustomDocumentDetailModel : DocumentDetailModel
    {
        /// <summary>
        /// Descripcion del Producto [OBSOLETO] - Reemplazado por "Description" // Se lo usa por versiones anteriores.
        /// </summary>
        [Obsolete]
        public string Name { get; set; }
    }


    //public class CustomDocumentReasonModel : DocumentReasonModel
    //{
    //    /// <summary>
    //    /// Descripcion del Producto [OBSOLETO] - Reemplazado por "Description" // Se lo usa por versiones anteriores.
    //    /// </summary>
    //    [Obsolete]
    //    public string Reason { get; set; }

    //    public decimal Value { get; set; }
    //}
}
