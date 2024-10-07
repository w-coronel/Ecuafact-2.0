using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public partial class RetentionRate
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public int RetentionTaxId { get; set; }
        public decimal RateValue { get; set; }
        public bool IsEnabled { get; set; }

        [JsonIgnore]
        [ForeignKey("RetentionTaxId")]
        [InverseProperty("RetentionRate")]
        public virtual RetentionTax RetentionTax { get; set; }
    }
}
