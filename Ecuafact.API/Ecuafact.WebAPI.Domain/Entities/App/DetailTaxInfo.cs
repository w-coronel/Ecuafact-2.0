using Ecuafact.WebAPI.Domain.Dal.Core;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecuafact.WebAPI.Dal.Core
{
    [Table("impuesto")]
    public partial class DetailTaxInfo
    {
        [Column("pk")] public long Id { get; set; }
        [Column("pkDetalle")] public long DocumentDetailId { get; set; }
        [Column("codigo")] public string Code { get; set; }
        [Column("codigoPorcentaje")] public string PercentageCode { get; set; }
        [Column("tarifa")] public decimal Rate { get; set; }
        [Column("baseImponible")] public decimal TaxableBase { get; set; }
        [Column("valor")] public decimal TaxValue { get; set; }

         public virtual DocumentInfoDetail detalle { get; set; }
    }

    [Table("totalImpuesto")]
    public partial class TotalTaxInfo
    {
        [Column("pk")] public long Id { get; set; }
        [Column("pkInfoFactura")] public long DocumentInfoId { get; set; }
        [Column("codigo")] public string Code { get; set; }
        [Column("codigoPorcentaje")] public string PercentageCode { get; set; }
        [Column("tarifa")] public decimal? Rate { get; set; }
        [Column("baseImponible")] public decimal? TaxableBase { get; set; }
        [Column("valor")] public decimal? TaxValue { get; set; }
        [Column("codDocSustento")] public string ReferenceDocumentCode { get; set; }
        [Column("numDocSustento")] public string ReferenceDocumentNumber { get; set; }
        [Column("fechaEmisionDocSustento")] public string ReferenceDocumentDate { get; set; }
        [Column("descuentoAdicional")] public decimal? ReferenceDocumentDiscount { get; set; }


        public virtual DocumentInfo infoFactura { get; set; }
    }
}
