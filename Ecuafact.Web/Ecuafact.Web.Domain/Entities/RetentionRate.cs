using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecuafact.Web.Domain.Entities
{
    public partial class RetentionRate
    {
        public int Id { get; set; }
        public int RetentionTaxId { get; set; }
        public decimal RateValue { get; set; }
        public bool IsEnabled { get; set; }
    }
}
