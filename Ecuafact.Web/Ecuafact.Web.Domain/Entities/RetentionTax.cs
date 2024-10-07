using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecuafact.Web.Domain.Entities
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
        public short TaxTypeId { get; set; }
        public bool IsEnabled { get; set; }
        public string TaxType { get; set; }
        public virtual List<RetentionRate> RetentionRate { get; set; }
    }

    public partial class SupportType : CatalogBaseDto { }

    public partial class IdentificationSupplierTypeDto : CatalogBaseDto { }
}
