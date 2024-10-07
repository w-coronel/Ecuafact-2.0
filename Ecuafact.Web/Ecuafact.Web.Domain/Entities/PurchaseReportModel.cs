using Ecuafact.Web.Domain.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Ecuafact.Web.Domain.Entities
{
    public partial class PurchaseReportModel
    {
        public long Id { get; set; }
        public long? IssuerId { get; set; }
        [Display(Name = "Fecha de Emisi�n")]
        public DateTime? IssuedOn { get; set; }
        [Display(Name = "Cod. Tipo")]
        public string DocumentTypeCode { get; set; }
        [Display(Name = "Tipo")]
        public string DocumentType { get; set; }
        [Display(Name = "Establecimiento")]
        public string EstablishmentCode { get; set; }
        [Display(Name = "Pto. Emisi�n")]
        public string IssuePointCode { get; set; }
        [Display(Name = "Secuencial")]
        public string Sequential { get; set; }
        [Display(Name = "No. Documento")]
        public string DocumentNumber { get; set; }
        [Display(Name = "No. Autorizaci�n")]
        public string AuthorizationNumber { get; set; }
        [Display(Name = "Fecha Autorizaci�n")]
        public string AuthorizationDate { get; set; }
        [Display(Name = "RUC")]
        public string ContributorRUC { get; set; }
        [Display(Name = "Raz�n Social")]
        public string ContributorName { get; set; }
        [Display(Name = "Base 0%")]
        public decimal? Base0 { get; set; }
        [Display(Name = "Base Iva%")]
        public decimal? Base12 { get; set; }
        [Display(Name = "IVA")]
        public decimal? IVA { get; set; }
        [Display(Name = "Total")]
        public decimal? Total { get; set; }
        [Display(Name = "Descripci�n")]
        public string Description { get; set; }
        [Display(Name = "Observaciones")]
        public string Notes { get; set; }
        [Display(Name = "ID Retenci�n")]
        public long? RetentionId { get; set; }
        [Display(Name = "RUC Retenci�n")]
        public string RetentionContributorRUC { get; set; }
        [Display(Name = "Proveedor Retenci�n")]
        public string RetentionContributor { get; set; }
        [Display(Name = "Fecha Retenci�n")]
        public DateTime? RetentionIssuedOn { get; set; }
        [Display(Name = "No. Retenci�n")]
        public string RetentionNumber { get; set; }
        [Display(Name = "Autorizacion Retenci�n")]
        public string RetentionAuthorizationNumber { get; set; }
        [Display(Name = "Tipo Ref. Retenci�n")]
        public string RetentionReferenceType { get; set; }
        [Display(Name = "No. Ref. Retenci�n")]
        public string RetentionReferenceNumber { get; set; }
        [Display(Name = "Fecha Ref. Retenci�n")]
        public string RetentionReferenceDate { get; set; }
        [Display(Name = "Codigo 103")]
        public string Retention103TaxCode { get; set; }
        [Display(Name = "Base Renta")]
        public decimal? RetentionTaxBase { get; set; }
        [Display(Name = "% Renta")]
        public decimal? RetentionTaxRate { get; set; }
        [Display(Name = "Valor Retenido")]
        public decimal? RetentionTaxValue { get; set; }
        [Display(Name = "C�digo IVA")]
        public string RetentionVatCode { get; set; }
        [Display(Name = "Base IVA")]
        public decimal? RetentionVatBase { get; set; }
        [Display(Name = "% Retenci�n IVA")]
        public decimal? RetentionVatRate { get; set; }
        [Display(Name = "Valor Retenido IVA")]
        public decimal? RetentionVatValue { get; set; }
        [Display(Name = "C�digo ISD")]
        public string RetentionISDCode { get; set; }
        [Display(Name = "Base ISD")]
        public decimal? RetentionISDBase { get; set; }
        [Display(Name = "% Retencion ISD")]
        public decimal? RetentionISDRate { get; set; }
        [Display(Name = "Valor Retenido ISD")]
        public decimal? RetentionISDValue { get; set; }
        [Display(Name = "Motivo Retenci�n")]
        public string RetentionReason { get; set; }
        [Display(Name = "Descripcion Retenci�n")]
        public string RetentionDescription { get; set; }
        [Display(Name = "Observaciones Retenci�n")]
        public string RetentionNotes { get; set; }
        [Display(Name = "Retenci�n RUC")]
        public string RetentionRUC { get; set; }
        [Display(Name = "Retenci�n Nombre")]
        public string RetentionBusinessName { get; set; }
        [Display(Name = "Retenci�n Fecha")]
        public DateTime? RetentionDate { get; set; }
        [Display(Name = "RUC Emisor")]
        public string RUC { get; set; }
        [Display(Name = "Raz�n Social Emisor")]
        public string BussinesName { get; set; }

        [Display(Name = "Deducible")]
        public string Deducible { get; set; }
    }


    public partial class PurchaseReportResponse : OperationResult<List<PurchaseReportModel>>
    {
        public PurchaseReportResponse()
            : this(true, HttpStatusCode.OK)
        {

        }

        public PurchaseReportResponse(bool isSuccess, HttpStatusCode statusCode) : base(isSuccess, statusCode)
        {
            Entity = new List<PurchaseReportModel>();
        }
    }
   
}