using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public partial class RetentionTax
    {
        public RetentionTax()
        {
            RetentionRate = new List<RetentionRate>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string SriCode { get; set; }
        [Required]
        [StringLength(300)]
        public string Name { get; set; }
        public bool IsEnabled { get; set; }

        public short TaxTypeId { get; set; }

        public DateTime CreatedOn { get; set; }
        [JsonIgnore]
        public DateTime? LastModifiedOn { get; set; }

        [JsonIgnore]
        [ForeignKey("TaxTypeId")]
        [InverseProperty("RetentionTax")]
        public virtual TaxType TaxType { get; set; }

        [InverseProperty("RetentionTax")]
        public virtual List<RetentionRate> RetentionRate { get; set; }
    }
}
