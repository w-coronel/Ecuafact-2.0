using Ecuafact.WebAPI.Dal.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecuafact.WebAPI.Domain.Dal.Core
{
    [Table("infoFactura")]
    public partial class DocumentInfo
    {
        [Key]
        [Column("pk")] public long Id { get; set; }        
        [Column("PkDocumento")] public long DocumentPk { get; set; }
        [Column("fechaEmision")] public string IssuedOn { get; set; }
        [Column("dirEstablecimiento")] public string EstablishmentAddress { get; set; }
        [Column("contribuyenteEspecial")] public string IsSpecialContributor { get; set; }
        [Column("obligadoContabilidad")] public string IsAccountingRequired { get; set; }
        [Column("tipoIdentificacionComprador")] public string IdentificationType { get; set; }
        [Column("guiaRemision")] public string ReferralGuide { get; set; }
        [Column("razonSocialComprador")] public string BusinessName { get; set; }
        [Column("identificacionComprador")] public string Identification { get; set; }
        [Column("totalSinImpuestos")] public decimal Subtotal { get; set; }
        [Column("totalDescuento")] public decimal TotalDiscount { get; set; }
        [Column("propina")] public decimal Tip { get; set; }
        [Column("importeTotal")] public decimal Total { get; set; }
        [Column("moneda")] public string Currency { get; set; }
        [Column("SubTotal12")] public decimal? SubtotalVat { get; set; }
        [Column("SubTotal15")] public decimal? SubtotalVat15 { get; set; }        
        [Column("SubTotal0")] public decimal? SubtotalVatZero { get; set; }
        [Column("IVA")] public decimal? ValueAddedTax { get; set; }
        [Column("rise")] public string RISE { get; set; }
        [Column("codDocModificado")] public string ModifiedDocumentCode { get; set; }
        [Column("numDocModificado")] public string ModifiedDocumentNumber { get; set; }
        [Column("fechaEmisionDocSustento")] public string SupportDocumentIssueDate { get; set; }
        [Column("valorModificacion")] public decimal? ModifiedValue { get; set; }
        [Column("motivo")] public string Reason { get; set; }
        [Column("periodoFiscal")] public string FiscalPeriod { get; set; }
        [Column("dirpartida")] public string OriginAddress { get; set; }
        [Column("fechainitransporte")] public string ShippingStartDate { get; set; }
        [Column("fechafintransporte")] public string ShippingEndDate { get; set; }
        [Column("placa")] public string CarPlate { get; set; }
        [Column("identificaciondestinatario")] public string RecipientIdentificationType { get; set; }
        [Column("razonsocialdestinatario")] public string RecipientName { get; set; }
        [Column("dirdestinatario")] public string RecipientAddress { get; set; }
        [Column("descuentoAdicional")] public decimal? AdditionalDiscount { get; set; }
        [Column("numAutDocSustento")] public string ReferenceDocumentAuth { get; set; }
        [Column("SubTotalNoObjetoIVA")] public decimal? SubtotalNotSubject { get; set; }
        [Column("SubTototalExcentoIVA")] public decimal? SubtotalExempt { get; set; }
        [Column("incoTermFactura")] public string InvoiceIncoTerm { get; set; }
        [Column("lugarIncoTerm")] public string PlaceIncoTerm { get; set; }
        [Column("paisOrigen")] public string OriginCountry { get; set; }
        [Column("puertoEmbarque")] public string PortBoarding { get; set; }
        [Column("puertoDestino")] public string DestinationPort { get; set; }
        [Column("paisDestino")] public string DestinationCountry { get; set; }
        [Column("paisAdquisicion")] public string CountryAcquisition { get; set; }
        [Column("direccionComprador")] public string Address { get; set; }
        [Column("incoTermTotalSinImpuestos")] public string TotalWithoutTaxesIncoTerm { get; set; }
        [Column("fleteInternacional")] public decimal? InternationalFreight { get; set; }
        [Column("seguroInternacional")] public decimal? InternationalInsurance { get; set; }
        [Column("gastosAduaneros")] public decimal? CustomsFees { get; set; }
        [Column("gastosTransporteOtros")] public decimal? TransportCostsOthers { get; set; }
        [Column("SubTotal14")] public decimal? SubtotalVat14 { get; set; }
        public virtual List<TotalTaxInfo> TotalTaxes { get; set; }
        
    }
}
