using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public partial class SalesReport
    {
        public long Id { get; set; }
        public long? IssuerId { get; set; }
        public DateTime? IssuedOn { get; set; }
        public string DocumentTypeCode { get; set; }
        public string DocumentType { get; set; }
        public string EstablishmentCode { get; set; }
        public string IssuePointCode { get; set; }
        public string Sequential { get; set; }
        public string DocumentNumber { get; set; }
        public string AuthorizationNumber { get; set; }
        public string AuthorizationDate { get; set; }
        public string ContributorRUC { get; set; }
        public string ContributorName { get; set; }
        public decimal? Base0 { get; set; }
        public decimal? Base12 { get; set; }
        public decimal? IVA { get; set; }
        public decimal? Total { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public long? RetentionId { get; set; }
        public string RetentionContributorRUC { get; set; }
        public string RetentionContributor { get; set; }
        public DateTime? RetentionIssuedOn { get; set; }
        public string RetentionNumber { get; set; }
        public string RetentionAuthorizationNumber { get; set; }
        public string RetentionReferenceType { get; set; }
        public string RetentionReferenceNumber { get; set; }
        public string RetentionReferenceDate { get; set; }
        public string Retention104TaxCode { get; set; }
        public decimal? RetentionTaxBase { get; set; }
        public decimal? RetentionTaxRate { get; set; }
        public decimal? RetentionTaxValue { get; set; }
        public string RetentionVatCode { get; set; }
        public decimal? RetentionVatBase { get; set; }
        public decimal? RetentionVatRate { get; set; }
        public decimal? RetentionVatValue { get; set; }
        public string RetentionISDCode { get; set; }
        public decimal? RetentionISDBase { get; set; }
        public decimal? RetentionISDRate { get; set; }
        public decimal? RetentionISDValue { get; set; }
        public string RetentionReason { get; set; }
        public string RetentionDescription { get; set; }
        public string RetentionNotes { get; set; }
        public string RetentionRUC { get; set; }
        public string RetentionBusinessName { get; set; }
        public DateTime? RetentionDate { get; set; }
        public string RUC { get; set; }
        public string BussinesName { get; set; }
        public string Payment { get; set; }
        
    }

    public partial class Sales
    {
        public long Id { get; set; }
        public long? IssuerId { get; set; }
        public DateTime? IssuedOn { get; set; }
        public string DocumentTypeCode { get; set; }
        public string DocumentType { get; set; }
        public string EstablishmentCode { get; set; }
        public string IssuePointCode { get; set; }
        public string Sequential { get; set; }
        public string DocumentNumber { get; set; }
        public string AuthorizationNumber { get; set; }
        public string AuthorizationDate { get; set; }
        public string ContributorRUC { get; set; }
        public string ContributorName { get; set; }
        public decimal? Base0 { get; set; }
        public decimal? Base12 { get; set; }
        public decimal? IVA { get; set; }
        public decimal? Total { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }        
        public string RUC { get; set; }
        public string BussinesName { get; set; }
        public string Payment { get; set; }
    }

    public partial class VistaRetenciones
    {
        [Key]
        public long IdRetencion { get; set; }
        public string Retencion_RUC { get; set; }
        public string Retencion_Comprador { get; set; }
        public DateTime? Retencion_FechaEmision { get; set; }
        public string Retencion_Numero { get; set; }
        public string Retencion_Autorizacion { get; set; }
        public string Retencion_CodigoReferencia { get; set; }
        public string Retencion_NumeroReferencia { get; set; }
        public string Retencion_FechaReferencia { get; set; }
        public string Retencion_Codigo104 { get; set; }
        public decimal? Renta_BaseImponible { get; set; }
        public decimal? Renta_Porcentaje { get; set; }
        public decimal? Renta_Valor { get; set; }
        public string Retencion_CodigoIVA { get; set; }
        public decimal? Retencion_BaseIVA { get; set; }
        public decimal? Retencion_PorcentajeIVA { get; set; }
        public decimal? Retencion_IVARetenido { get; set; }
        public string Retencion_CodigoISD { get; set; }
        public decimal? Retencion_BaseISD { get; set; }
        public decimal? Retencion_PorcentajeISD { get; set; }
        public decimal? Retencion_ISDRetenido { get; set; }
        public string Retencion_Motivo { get; set; }
        public string Retencion_Descripcion { get; set; }
        public string Retencion_Observaciones { get; set; }
        public string Retencion_RUCProveedor { get; set; }
        public string Retencion_Proveedor { get; set; }
        public DateTime? Documento_FechaEmision { get; set; }
    }
}
