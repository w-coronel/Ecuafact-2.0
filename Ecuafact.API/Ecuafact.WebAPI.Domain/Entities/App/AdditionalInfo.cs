using System.ComponentModel.DataAnnotations.Schema;
    
namespace Ecuafact.WebAPI.Dal.Core
{
    [Table("campoAdicional")]
    public partial class AdditionalInfo
    {
        [Column("pk")] public long Id { get; set; }
        [ForeignKey("documento")]
        [Column("pkFactura")] public long DocumentPk { get; set; }
        [Column("nombre")] public string Name { get; set; }
        [Column("valor")] public string Value { get; set; }
        [Column("NumLinea")] public int? LineNumber { get; set; }

        public virtual SupplierDocument documento { get; set; }
    }
}
