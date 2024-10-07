using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public partial class TaxType
    {
        public TaxType()
        {
            RetentionTax = new List<RetentionTax>();
        }
        [JsonIgnore]
        public short Id { get; set; }
        [Required]
        [StringLength(10)]
        public string SriCode { get; set; }
        [Required]
        [StringLength(300)]
        public string Name { get; set; }
        [Required]
        public bool IsEnabled { get; set; }

        [InverseProperty("TaxType")]
        public virtual List<RetentionTax> RetentionTax { get; set; }
    }
}
