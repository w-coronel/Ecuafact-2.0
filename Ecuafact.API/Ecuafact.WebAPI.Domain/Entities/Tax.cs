using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class Tax
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }

        /// <summary>
        /// codigo
        /// </summary>
        [MaxLength(1)]
        public string Code { get; set; }

        [MaxLength(4)]
        public string PercentageCode { get; set; }

        public decimal Rate { get; set; }

        public decimal TaxableBase { get; set; }

        public decimal TaxValue { get; set; }
        
        public long DocumentDetailId { get; set; }

        //public Document Document { get; set; }
        //public long DocumentId { get; set; }

        [JsonIgnore]
        public DocumentDetail DocumentDetail { get; set; }
    }
}
