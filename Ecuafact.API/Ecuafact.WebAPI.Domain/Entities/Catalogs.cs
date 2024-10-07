using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class DocumentCatalogBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public short Id { get; set; }

        [StringLength(10)]
        public string SriCode { get; set; }

        [StringLength(300)]
        [Required]
        public string Name { get; set; }

        public bool IsEnabled { get; set; }
    }

    [Table("PaymentMethod")]
    public class PaymentMethod : DocumentCatalogBase { }

    [Table("IdentificationType")]
    public class IdentificationType : DocumentCatalogBase { }

    /// <summary>
    /// Tipos de Documentos Electronicos que puede generar un contribuyente
    /// </summary>
    [Table("DocumentType")]
    public class DocumentType : DocumentCatalogBase { }

    [Table("IVARate")]
    public class VatRate : DocumentCatalogBase
    {
        public decimal RateValue { get; set; }
    }

    [Table("ICERate")]
    public class IceRate : DocumentCatalogBase
    {
        /// <summary>
        /// Tarifa AdValorem
        /// </summary>
        public decimal? Rate { get; set; }

        /// <summary>
        /// Tarifa Especifica
        /// </summary>
        public decimal? EspecificRate { get; set; }

        /// <summary>
        /// Descripcion de la Tarifa Especifica
        /// </summary>
        public string EspecificRateDescription { get; set; }

    }

    [Table("SupportType")]
    public class SupportType : DocumentCatalogBase { }


    [Table("IdentificationSupplierType")]
    public class IdentificationSupplierType : DocumentCatalogBase { }

}
