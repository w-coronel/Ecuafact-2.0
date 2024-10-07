using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecuafact.WebAPI.Dal.Core
{
    [Table("detalle")]
    public partial class DocumentInfoDetail
    {
        public DocumentInfoDetail()
        {
            this.Taxes = new List<DetailTaxInfo>();
        }

        [Column("pk")] public long Id { get; set; }
        [ForeignKey("documento")]
        [Column("PkDocumento")] public long DocumentPk { get; set; }
        [Column("Secuencia")] public string Sequence { get; set; }
        [Column("codigoPrincipal")] public string MainCode { get; set; }
        [Column("codigoAuxiliar")] public string AuxCode { get; set; }
        [Column("descripcion")] public string Description { get; set; }
        [Column("cantidad")] public decimal Amount { get; set; }
        [Column("precioUnitario")] public decimal UnitPrice { get; set; }
        [Column("descuento")] public decimal Discount { get; set; }
        [Column("precioTotalSinImpuesto")] public decimal Subtotal { get; set; }
        [Column("nom1")] public string Name1 { get; set; }
        [Column("val1")] public string Value1 { get; set; }
        [Column("nom2")] public string Name2 { get; set; }
        [Column("val2")] public string Value2 { get; set; }
        [Column("nom3")] public string Name3 { get; set; }
        [Column("val3")] public string Value3 { get; set; }
        [Column("unidadMedida")] public string UnitsOfMeasure { get; set; }
        [Column("esDeducible")] public bool? IsDeductible { get; set; }

        public virtual SupplierDocument documento { get; set; }

        public virtual List<DetailTaxInfo> Taxes { get; set; }
    }
}
