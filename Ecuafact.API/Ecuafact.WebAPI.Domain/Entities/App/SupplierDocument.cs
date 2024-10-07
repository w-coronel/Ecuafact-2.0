using Ecuafact.WebAPI.Domain.Dal.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecuafact.WebAPI.Dal.Core
{
    [Table("documento")]
    public partial class SupplierDocument
    {
        public SupplierDocument()
        {
            this.AdditionalFields = new List<AdditionalInfo>();
            this.Details = new List<DocumentInfoDetail>();
            this.DocumentInfo = new List<DocumentInfo>();
            this.Payments = new List<PaymentInfo>();
        }

        [Key]
        [Column("pk")] public long DocumentPk { get; set; }
        [Column("PkDocumento")] public string DocumentId { get; set; }
        [Column("SecuencialUltimoProceso")] public int? LastProcessSequential { get; set; }
        [Column("version")] public string Version { get; set; }
        [Column("id")] public string Id { get; set; }
        [Column("ambiente")] public string Environment { get; set; }
        [Column("tipoEmision")] public string IssueType { get; set; }
        [Column("razonSocial")] public string BusinessName { get; set; }
        [Column("nombreComercial")] public string TradeName { get; set; }
        [Column("ruc")] public string RUC { get; set; }
        [Column("claveAcceso")] public string AccessKey { get; set; }
        [Column("codDoc")] public string DocumentTypeCode { get; set; }
        [Column("estab")] public string EstablishmentCode { get; set; }
        [Column("ptoEmi")] public string IssuePointCode { get; set; }
        [Column("secuencial")] public string Sequential { get; set; }
        [Column("dirMatriz")] public string MainAddress { get; set; }
        [Column("numeroautorizacion")] public string AuthorizationNumber { get; set; }
        [Column("Estado")] public int? Status { get; set; }
        [Column("StrEstado")] public string StatusMsg { get; set; }
        [Column("fechaAutorizacion")] public string AuthorizationDate { get; set; }
        [Column("FechaIngreso")] public DateTime? CreatedOn { get; set; }
        [Column("FechaUltimoIntento")] public DateTime? LastProcessDate { get; set; }
        [Column("FechaRecepcionCorreo")] public DateTime? EmailReceptionDate { get; set; }
        [Column("FechaEnvioCorreo")] public DateTime? EmailSentDate { get; set; }
        [Column("FechaVisualizaciónCliente")] public DateTime? CustomerDisplayDate { get; set; }
        [Column("FechaReenvioCorreo")] public DateTime? ResendMailDate { get; set; }
        [Column("emails")] public string Emails { get; set; }
        [Column("PorcentajeDescuento")] public decimal? DiscountPercentage { get; set; }
        [Column("EstadoProceso")] public short? ProcessStatus { get; set; }
        [Column("FechaHibernacion")] public DateTime? HibernationDate { get; set; }
        [Column("CodDoc2")] public string DocumentTypeCode2 { get; set; }
        [Column("UsuarioIngreso")] public string CreatedBy { get; set; }
        [Column("FechaEnvio")] public DateTime? SentDate { get; set; }
        [Column("PDF")] public string PDF { get; set; }
        [Column("XML")] public string XML { get; set; }
        [Column("MSG")] public string MSG { get; set; }
        [Column("IDMAIL")] public string IDMAIL { get; set; }
        [Column("XMLC")] public string XMLC { get; set; }
        [Column("PDFC")] public string PDFC { get; set; }
        [Column("Prioridad")] public int? Priority { get; set; }
        [Column("idDeducible")] public int? DeductibleId { get; set; }
        [Column("Destino")] public string Destiny { get; set; }
        [Column("Origen")] public string Origen { get; set; }
        [Column("ResultadoEnviado")] public byte? SentResult { get; set; }
        [Column("Replicado")] public byte? Replicated { get; set; }
        [Column("Validado")] public byte? Validated { get; set; }
        [Column("AutorizarAhora")] public bool? AuthorizeNow { get; set; }
        [Column("EstadoExtra1")] public int? ExtraStatus1 { get; set; }
        [Column("FechaEstadoExtra1")] public DateTime? ExtraStatusDate1 { get; set; }
        [Column("EstadoExtra2")] public int? ExtraStatus2 { get; set; }
        [Column("FechaEstadoExtra2")] public DateTime? ExtraStatusDate2 { get; set; }
        [Column("EstadoExtraObservacion1")] public string NotesExtraStatus1 { get; set; }
        [Column("EstadoExtraObservacion2")] public string NotesExtraStatus2 { get; set; }
        [Column("numregistros")] public int? TotalRecords { get; set; }
        [Column("MigracionNube")] public bool? CloudMigration { get; set; }
        [Column("ErrorMigracion")] public string ErrorMigration { get; set; }
        [Column("MigracionRegion")] public bool? MigracionRegion { get; set; }
        [Column("ErrorMigracionRegion")] public string ErrorMigracionRegion { get; set; }
        [Column("Offline")] public bool? Offline { get; set; }
        [Column("FechaEnvioSRI")] public DateTime? FechaEnvioSRI { get; set; }
        [Column("CodSucursal")] public string CodSucursal { get; set; }
        [Column("EstadoUltimaGestion")] public short? EstadoUltimaGestion { get; set; }
        [Column("FechaUltimaGestion")] public DateTime? FechaUltimaGestion { get; set; }
        [Column("DatosRecepcionEnviados")] public bool? DatosRecepcionEnviados { get; set; }
        [Column("codSustento")] public string SupportTypeCode { get; set; } 
        public virtual List<AdditionalInfo> AdditionalFields { get; set; }
        public virtual List<DocumentInfoDetail> Details { get; set; }
        public virtual List<DocumentInfo> DocumentInfo { get; set; }
        public virtual List<PaymentInfo> Payments { get; set; }
        public virtual List<DocumentReason> Reason { get; set; }
    }
}
