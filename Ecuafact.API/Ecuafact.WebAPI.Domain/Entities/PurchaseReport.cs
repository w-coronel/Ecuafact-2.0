using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public partial class PurchaseReport
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
        public string Retention103TaxCode { get; set; }
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
        public int? CodDeductible { get; set; }
        public string Deductible { get; set; }
    }

    public partial class Purchases
    {   
        public long Id { get; set; }
        public long RetentionId { get; set; }
        public string RetentionContributorRUC { get; set; }
        public string RetentionContributor { get; set; }
        public DateTime? RetentionIssuedOn { get; set; }
        public string RetentionNumber { get; set; }
        public string RetentionAuthorizationNumber { get; set; }
        public string RetentionReferenceType { get; set; }
        public string RetentionReferenceNumber { get; set; }
        public string RetentionReferenceDocumentAuth { get; set; }        
        public string RetentionReferenceDate { get; set; }
        public string Retention103TaxCode { get; set; }
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
    }
    public partial class vwFacturas
    {
        [Key]
        public long Id { get; set; }
        public DateTime? fechaEmision { get; set; }
        public string tipoDocumento { get; set; }
        public string documentoType { get; set; }
        public string Establecimiento { get; set; }
        public string PuntoEmision { get; set; }
        public string secuencial { get; set; }
        public string NumeroDocumento { get; set; }
        public string numeroautorizacion { get; set; }
        public string fechaAutorizacion { get; set; }
        public string RUCProveedor { get; set; }
        public string RazonSocialProveedor { get; set; }
        public decimal? SubTotal0 { get; set; }
        public decimal? SubTotal12 { get; set; }
        public decimal? IVA { get; set; }
        public decimal? Total { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public string RUC { get; set; }
        public string RazonSocial { get; set; }
        public int? CodDeducible { get; set; }
        public string Deducible { get; set; }
    }
    
}