using Ecuafact.Web.Domain.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities
{
    /// <summary>
    /// Modelo de Datos para los Documentos
    /// </summary>
    public class DocumentModel
    {
        /// <summary>
        /// Identificador Unico del Modelo de Datos
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Registro Unico de Contribuyente
        /// </summary>
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
        /// Fecha Emision
        /// </summary>
        public DateTime CreatedOn { get; set; }
        
        /// <summary>
        /// Razon Social
        /// </summary>
        [MaxLength(300)]
        
        public string BussinesName { get; set; }

        /// <summary>
        /// Nombre Comercial
        /// </summary>
        [MaxLength(300)]
        public string TradeName { get; set; }

        /// <summary>
        /// Si se encuentra activo o ha sido eliminado
        /// </summary>
        public bool IsEnabled { get; set; }


        /// <summary>
        /// Id del Emisor
        /// </summary>
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
        /// Numero de Estado
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
        /// Numero del Documento
        /// </summary>
        [NotMapped]
        public string DocumentNumber
        {
            get {
                return $"{EstablishmentCode}-{IssuePointCode}-{Sequential}";
            }
            set { } // do nothing x) por si acaso
        }       

        public DateTime IssuedOn { get; set; }
        public long ContributorId { get; set; }
        public string ContributorIdentificationType { get; set; }
        public string ContributorName { get; set; }
        public string ContributorIdentification { get; set; }
        public string ContributorAddress { get; set; }
        public string Currency { get; set; }
        public string Reason { get; set; }
        public decimal Total { get; set; }
        public bool? EmailSent { get; set; }
        public InvoiceInfo InvoiceInfo { get; set; }
        public CreditNoteInfoModel CreditNoteInfo { get; set; }
        public ReferralGuideInfoModel ReferralGuideInfo { get; set; }
        public RetentionInfo RetentionInfo { get; set; }
        public List<AdditionalFieldModel> AdditionalFields { get; set; }

        [NotMapped]
        public string DocumentName { get; set; }
        [NotMapped]
        public string Controller { get; set; }
        [NotMapped]
        public string IconClass { get; set; }
        [NotMapped]
        public string IconType { get; set; }
        [NotMapped]
        public string StyleClass { get; set; }
        [NotMapped]
        public string StatusMessage { get; set; }
        [NotMapped]
        public string StatusType { get; set; }
        [NotMapped]
        public string StyleColor { get; set; }
        [NotMapped]
        public string PdfUrl{ get; set; }

    }

    public partial class AtsFactura
    {
        public long Id { get; set; }
        public DateTime? FechaEmision { get; set; }
        public string TipoDocumento { get; set; }
        public int? Anio { get; set; }
        public int? Mes { get; set; }
        public string DocumentoType { get; set; }
        public string NumeroDocumento { get; set; }
        public string Numeroautorizacion { get; set; }
        public string FechaAutorizacion { get; set; }
        public string Rucproveedor { get; set; }
        public string RazonSocialProveedor { get; set; }
        public decimal Total { get; set; }
        public string Ruc { get; set; }
        public string RazonSocial { get; set; }
        public string CodSustento { get; set; }
        public int TipoEmision { get; set; }
    }

    public partial class AtsFacturaReportResponse : OperationResult<List<AtsFactura>>
    {
        public AtsFacturaReportResponse()
            : this(true, HttpStatusCode.OK)
        {

        }

        public AtsFacturaReportResponse(bool isSuccess, HttpStatusCode statusCode) : base(isSuccess, statusCode)
        {
            Entity = new List<AtsFactura>();
        }
    }

    public partial class StatisticsAts
    {
        public int Factura { get; set; }
        public int NotaVenta { get; set; }
        public int NotaCredito { get; set; }
        public int NotaDebito { get; set; }
        public int Retencion { get; set; }        
        public int CountDocument { get; set; }

    }
        
}