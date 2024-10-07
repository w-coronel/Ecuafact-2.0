using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities
{
    public class InvoiceInfo
    {
        public long InvoiceInfoId { get; set; }

        /// <summary>
        /// Fecha de Emision
        /// </summary>
        [MaxLength(10)]
        [Required]
        public string IssuedOn { get; set; }

        /// <summary>
        /// Dirección del Establecimiento
        /// </summary>
        [StringLength(300)]
        [Required]
        public string EstablishmentAddress { get; set; }

        /// <summary>
        /// tipoIdentificacionComprador
        /// </summary>
        [MaxLength(2)]
        [Required]
        public string IdentificationType { get; set; }

        /// <summary>
        /// identificacion Comprador 
        /// </summary>
        [MaxLength(100)]
        [Required]
        public string Identification { get; set; }

        /// <summary>
        /// Razon Social Comprador
        /// </summary>
        [StringLength(300)]
        [Required]
        public string BussinesName { get; set; }

        public string Currency { get; set; }

        /// <summary>
        /// Subtotal Iva
        /// </summary>
        public decimal SubtotalVat { get; set; }

        /// <summary>
        /// Subtotal IVA 0
        /// </summary>
        public decimal SubtotalVatZero { get; set; }

        /// <summary>
        /// Subtotal No Objeto
        /// </summary>
        public decimal SubtotalNotSubject { get; set; }

        /// <summary>
        /// Subtotal Exento
        /// </summary>
        public decimal SubtotalExempt { get; set; }

        /// <summary>
        /// Subtotal
        /// </summary>
        public decimal Subtotal { get; set; }

        /// <summary>
        /// Total Descuento
        /// </summary>
        public decimal TotalDiscount { get; set; }

        /// <summary>
        /// ICE, Impuesto a los consumos especiales. Special Consum Tax
        /// </summary>
        public decimal SpecialConsumTax { get; set; }

        /// <summary>
        /// Valor del IVA
        /// </summary>
        public decimal ValueAddedTax { get; set; }

        /// <summary>
        /// Propina
        /// </summary>
        public decimal Tip { get; set; }

        /// <summary>
        /// Importe Total
        /// </summary>
        public decimal Total { get; set; }
        
        /// <summary>
        /// Guia de Remision
        /// </summary>
        [MaxLength(20)]
        public string ReferralGuide { get; set; }

        /// <summary>
        /// Direccion Comprador
        /// </summary>
        [StringLength(300)]
        public string Address { get; set; }

        /// <summary>
        /// codDocModificado
        /// </summary>
        [MaxLength(2)]
        public string ModifiedDocumentCode { get; set; }

        /// <summary>
        /// numDocModificado
        /// </summary>
        [MaxLength(17)]
        public string ModifiedDocumentNumber { get; set; }

        /// <summary>
        /// fechaEmisionDocSustento
        /// </summary>
        [MaxLength(10)]
        public string  SupportDocumentIssueDate { get; set; }

        /// <summary>
        /// Id de referencia del Documento modificado por la Nota de Debito/Credito
        /// </summary>
        public long? ModifiedDocumentId { get; set; }

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
        /// telefono del comprador
        /// </summary>
        [MaxLength(3)]
        public string Phone { get; set; } =  "9999999";        

        /// <summary>
        /// incoTermTotalSinImpuestos 
        /// </summary>
        [MaxLength(10)]
        public string TotalWithoutTaxesIncoTerm { get; set; }

        public List<DocumentDetailModel> Details { get; set; } = new List<DocumentDetailModel>();

        public List<PaymentModel> Payments { get; set; } = new List<PaymentModel>();

        public List<TotalTaxModel> TotalTaxes { get; set; }


    }
}
