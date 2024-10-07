using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ecuafact.WebAPI.Models
{
    public class SalesNoteRequestModel: DocumentRecibiRequestModel
    {
        public SalesNoteRequestModel()
        {
            IssuingId = 0;
            Payments = new List<PaymentModel>();
            Details = new List<DocumentDetailModel>();
        }
        public long? IssuingId { get; set; }
        [StringLength(9)]
        [Required(ErrorMessage = "El secuencial es requerido")]
        public string Sequential { get; set; }
        [Required(ErrorMessage = "El emisor es requerido")]
        public string IssuingRuc { get; set; }
        public string IssuingBussinesName { get; set; }
        public string IssuingTradeName { get; set; }
        public string IssuingAddress { get; set; }
        public string IssuingEmailAddresses { get; set; }
        public string IssuingPhone { get; set; }
        [Required(ErrorMessage = "El número de autización del SRI es requerido")]
        public string AuthorizationNumber { get; set; }
        public string DocumentTypeCode { get; set; }
        public string ReferralGuide { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public decimal TotalDiscount { get; set; }
        public List<DocumentDetailModel> Details { get; set; }
        public List<PaymentModel> Payments { get; set; }
        public int Term { get; set; }
        public string TimeUnit { get; set; }
        public long ModifiedDocumentId { get; set; }
        public string IsSpecialContributor { get; set; }
        public bool IsAccountingRequired { get; set; }
        public byte[] DocumentPdfRaw { get; set; }
        public string FileType { get; set; }
    }

    /// <summary>
    /// Modelo de datos para documentos genericos
    /// </summary>
    public class DocumentRecibiRequestModel : DocumentRequestBase
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
        public decimal SubtotalVat { get; set; }

        /// <summary>
        /// Subtotal IVA 0. Formato decimal 0.00
        /// </summary>
       
        public decimal SubtotalVatZero { get; set; }

        /// <summary>
        /// Subtotal No Objeto. Formato decimal 0.00
        /// </summary>
       
        public decimal SubtotalNotSubject { get; set; }

        /// <summary>
        /// Subtotal Exento. Formato decimal 0.00
        /// </summary>       
        public decimal SubtotalExempt { get; set; }

        /// <summary>
        /// Subtotal. Formato decimal 0.00
        /// </summary>
        
        public decimal Subtotal { get; set; }      

        /// <summary>
        /// Total Descuento. Formato decimal 0.00
        /// </summary>       
        public decimal TotalDiscount { get; set; }

        /// <summary>
        /// ICE, Impuesto a los consumos especiales. Special Consum Tax. Formato decimal 0.00
        /// </summary>
        
        public decimal SpecialConsumTax { get; set; }

        /// <summary>
        /// Valor del IVA. Formato decimal 0.00
        /// </summary>       
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
}