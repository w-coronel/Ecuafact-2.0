using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class Document
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }

        /// <summary>
        /// Numero del Documento
        /// </summary>
        [NotMapped]
        public string DocumentNumber
        {
            get
            {
                return $"{EstablishmentCode}-{IssuePointCode}-{Sequential}";
            }
            set { } // do nothing x) por si acaso
        }

        /// <summary>
        /// Registro Unico de Contribuyente
        /// </summary>
        [Index("IX_DOCUMENT_RUC")]
        [MaxLength(13)]
        [Required]
        public string RUC { get; set; }

        /// <summary>
        /// Código del Documento a Emitir (Tabla 3 Ficha Tecnica)
        /// </summary>
        [StringLength(2)]
        public string DocumentTypeCode { get; set; }

        /// <summary>
        /// Codigo Establecimiento
        /// </summary>
        [StringLength(5)]
        public string EstablishmentCode { get; set; }

        /// <summary>
        /// Punto de Emisión del Comprobante
        /// </summary>
        [StringLength(5)]
        public string IssuePointCode { get; set; }

        /// <summary>
        /// Secuencial
        /// </summary>
        [StringLength(9)]
        public string Sequential { get; set; }


        /// <summary>
        /// Emails, Emails para enviar el comprobante.
        /// </summary>
        [StringLength(500)]
        public string Emails { get; set; }

        /// <summary>
        /// Fecha de creacion del documento
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Razon Social
        /// </summary>
        [MaxLength(300)]
        [Required]
        public string BussinesName { get; set; }

        /// <summary>
        /// Nombre Comercial
        /// </summary>
        [MaxLength(300)]
        public string TradeName { get; set; }

        /// <summary>
        /// Direccion Principal
        /// </summary>
        [MaxLength(300)]
        public string MainAddress { get; set; }

        /// <summary>
        /// Direccion del establecimiento
        /// </summary>
        [MaxLength(300)]
        public string EstablishmentAddress { get; set; }
        /// <summary>
        /// Fecha de Emision
        /// </summary>
        public DateTime IssuedOn { get; set; }
        /// <summary>
        /// Id del Contribuyente
        /// </summary>
        public long? ContributorId { get; set; }
        /// <summary>
        /// Tipo del Contribuyente
        /// </summary>
        public string ContributorIdentificationType { get; set; }
        /// <summary>
        /// Nombre del contribuyente
        /// </summary>
        [MaxLength(300)]
        public string ContributorName { get; set; }
        /// <summary>
        /// Identificacion del Contribuyente
        /// </summary>
        [MaxLength(20)]
        public string ContributorIdentification { get; set; }
        /// <summary>
        /// Direccion del Contribuyente
        /// </summary>
        [MaxLength(300)]
        public string ContributorAddress { get; set; }
        /// <summary>
        /// Moneda: DOLAR
        /// </summary>
        [MaxLength(50)]
        public string Currency { get; set; }
        /// <summary>
        /// Motivo
        /// </summary>
        [MaxLength(300)]
        public string Reason { get; set; }
        /// <summary>
        /// Total
        /// </summary>
        public decimal Total { get; set; }


        /// <summary>
        /// Id del Emisor
        /// </summary>
        [Required]
        public long IssuerId { get; set; }

        /// <summary>
        /// Clave de Acceso
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// Fecha de autorizacion
        /// </summary>
        public string AuthorizationDate { get; set; }

        /// <summary>
        /// Numero de Autorizacion
        /// </summary>
        public string AuthorizationNumber { get; set; }

        /// <summary>
        /// PDF
        /// </summary>
        public string PDF { get; set; }

        /// <summary>
        /// XML
        /// </summary>
        public string XML { get; set; }

        /// <summary>
        /// Estado actual del documento
        /// </summary>
        public DocumentStatusEnum? Status { get; set; }        
        /// <summary>
        /// Mensaje del Estado
        /// </summary>
        public string StatusMsg { get; set; }

        /// <summary>
        /// Fecha de Modificacion
        /// </summary>
        public DateTime? LastModifiedOn { get; set; }


        /// <summary>
        /// Habilitado?
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Envio de correo
        /// </summary>
        public bool? EmailSent { get; set; }

        /// <summary>
        /// Se envio aprocesar el documento al SRI
        /// </summary>
        //public bool? Processed { get; set; }

        public string PurchaseOrder { get; set; }

        public InvoiceInfo InvoiceInfo { get; set; }

        public RetentionInfo RetentionInfo { get; set; }

        public CreditNoteInfo CreditNoteInfo { get; set; }

        public SettlementInfo SettlementInfo { get; set; }

        public ReferralGuideInfo ReferralGuideInfo { get; set; }

        public DebitNoteInfo DebitNoteInfo { get; set; }

        public List<AdditionalField> AdditionalFields { get; set; } = new List<AdditionalField>();


    }
}