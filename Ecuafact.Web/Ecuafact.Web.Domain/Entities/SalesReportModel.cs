using Ecuafact.Web.Domain.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Ecuafact.Web.Domain.Entities
{
    public partial class SalesReportModel
    {
        public long Id { get; set; }
        public long? IssuerId { get; set; }
        [Display(Name="Fecha de Emisión")]
        public DateTime? IssuedOn { get; set; }
        [Display(Name = "Cod. Tipo")]
        public string DocumentTypeCode { get; set; }
        [Display(Name = "Tipo")]
        public string DocumentType { get; set; }
        [Display(Name = "Establecimiento")]
        public string EstablishmentCode { get; set; }
        [Display(Name = "Pto. Emisión")]
        public string IssuePointCode { get; set; }
        [Display(Name = "Secuencial")]
        public string Sequential { get; set; }
        [Display(Name = "No. Documento")]
        public string DocumentNumber { get; set; }
        [Display(Name = "No. Autorización")]
        public string AuthorizationNumber { get; set; }
        [Display(Name = "Fecha Autorización")]
        public string AuthorizationDate { get; set; }
        [Display(Name = "RUC")]
        public string ContributorRUC { get; set; }
        [Display(Name = "Razón Social")]
        public string ContributorName { get; set; }
        [Display(Name = "Base 0%")]
        public decimal? Base0 { get; set; }
        [Display(Name = "Base Iva%")]
        public decimal? Base12 { get; set; }
        [Display(Name = "IVA")]
        public decimal? IVA { get; set; }
        [Display(Name = "Total")]
        public decimal? Total { get; set; }
        [Display(Name = "Descripción")]
        public string Description { get; set; }
        [Display(Name = "Observaciones")]
        public string Notes { get; set; }
        [Display(Name = "ID Retención")]
        public long? RetentionId { get; set; }
        [Display(Name = "RUC Retención")]
        public string RetentionContributorRUC { get; set; }
        [Display(Name = "Proveedor Retención")]
        public string RetentionContributor { get; set; }
        [Display(Name = "Fecha Retención")]
        public DateTime? RetentionIssuedOn { get; set; }
        [Display(Name = "No. Retención")]
        public string RetentionNumber { get; set; }
        [Display(Name = "Autorización Retención")]
        public string RetentionAuthorizationNumber { get; set; }
        [Display(Name = "Tipo Ref. Retención")]
        public string RetentionReferenceType { get; set; }
        [Display(Name = "No. Ref. Retención")]
        public string RetentionReferenceNumber { get; set; }
        [Display(Name = "Fecha Ref. Retención")]
        public string RetentionReferenceDate { get; set; }
        [Display(Name = "Código 104")]
        public string Retention104TaxCode { get; set; }
        [Display(Name = "Base Renta")]
        public decimal? RetentionTaxBase { get; set; }
        [Display(Name = "% Renta")]
        public decimal? RetentionTaxRate { get; set; }
        [Display(Name = "Valor Retenido")]
        public decimal? RetentionTaxValue { get; set; }
        [Display(Name = "Código IVA")]
        public string RetentionVatCode { get; set; }
        [Display(Name = "Base IVA")]
        public decimal? RetentionVatBase { get; set; }
        [Display(Name = "% Retención IVA")]
        public decimal? RetentionVatRate { get; set; }
        [Display(Name = "Valor Retenido IVA")]
        public decimal? RetentionVatValue { get; set; }
        [Display(Name = "Código ISD")]
        public string RetentionISDCode { get; set; }
        [Display(Name = "Base ISD")]
        public decimal? RetentionISDBase { get; set; }
        [Display(Name = "% Retención ISD")]
        public decimal? RetentionISDRate { get; set; }
        [Display(Name = "Valor Retenido ISD")]
        public decimal? RetentionISDValue { get; set; }
        [Display(Name = "Motivo Retención")]
        public string RetentionReason { get; set; }
        [Display(Name = "Descripcion Retención")]
        public string RetentionDescription { get; set; }
        [Display(Name = "Observaciones Retención")]
        public string RetentionNotes { get; set; }
        [Display(Name = "Retención RUC")]
        public string RetentionRUC { get; set; }
        [Display(Name = "Retención Nombre")]
        public string RetentionBusinessName { get; set; }
        [Display(Name = "Retención Fecha")]
        public DateTime? RetentionDate { get; set; }
        [Display(Name = "RUC Emisor")]
        public string RUC { get; set; }
        [Display(Name = "Razón Social Emisor")]
        public string BussinesName { get; set; }

        [Display(Name = "Metodos de Pago")]
        public string Payment { get; set; }       

    }

    public partial class SalesReportResponse : OperationResult<List<SalesReportModel>>
    {
        public SalesReportResponse()
            : this(true, HttpStatusCode.OK )
        {
            
        }

        public SalesReportResponse(bool isSuccess, HttpStatusCode statusCode) : base(isSuccess, statusCode)
        {
            Entity = new List<SalesReportModel>();
        }
    }
}
