using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }

        [Required]
        [StringLength(500)]
        [Index("IX_PROD_CODE")]
        public string MainCode { get; set; }

        [StringLength(500)]
        public string AuxCode { get; set; }

        [StringLength(1000)]
        public string Name { get; set; }

        public decimal UnitPrice { get; set; }

        public ProductType ProductType { get; set; }
        public short ProductTypeId { get; set; }
        public Issuer Issuer { get; set; }
        public long IssuerId { get; set; }
        public VatRate IvaRate { get; set; }
        public short IvaRateId { get; set; }
        public IceRate IceRate { get; set; }
        public short IceRateId { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? LastModifiedOn { get; set; }
        public string Name1 { get; set; }
        public string Value1 { get; set; }
        public string Name2 { get; set; }
        public string Value2 { get; set; }
        public string Name3 { get; set; }
        public string Value3 { get; set; }
        /// <summary>
        /// Numero del Documento
        /// </summary>
        [NotMapped]
        public string ProductCode
        {
            get
            {
                return $"{MainCode}{(string.IsNullOrEmpty(AuxCode) ? "." : string.Empty)}{AuxCode}";
            }
            set { } // do nothing x) por si acaso
        }

    }

    public class ProductType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public short Id { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        public bool IsEnabled { get; set; }

    }

}
