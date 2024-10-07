using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class ReferredUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }

        public string RUC { get; set; }
        public string Name { get; set; }
        public decimal Discount { get; set; }
        public decimal SpecialPrice { get; set; }
        public long VendorId { get; set; }
        public string Notes { get; set; }


    }


}
