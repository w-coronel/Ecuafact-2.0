using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Dal.Core
{
    [Table("formapago")]
    public class PaymentInfo
    {
        [Column("Id")]
        public long Id { get; set; }

        [Column("pkDocumento")]
        public long DocumentPk { get; set; }

        [Column("formapago")]
        public string PaymentMethodCode { get; set; }

        [Column("total")]
        public decimal? Total { get; set; }

        [Column("plazo")]
        public int? Term { get; set; }

        [Column("unidadTiempo")]
        public string TimeUnit { get; set; }
    }

    [Table("motivoDocumento")]
    public class DocumentReason
    {
        [Column("pk")]
        public long Id { get; set; }

        [Column("pkDocumento")]
        public long DocumentPk { get; set; }

        [Column("razon")]
        public string Reason { get; set; }

        [Column("valor")]
        public float? Value { get; set; }
    }

    
}
