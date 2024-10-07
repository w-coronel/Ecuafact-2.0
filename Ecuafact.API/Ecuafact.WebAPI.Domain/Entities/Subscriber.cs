using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class Subscriber
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }

        public Guid SubscriberId { get; set; }
        public string Identification { get; set; }
        public string Description { get; set; }
        public string Message { get; set; }
        public string SRIPassword { get; set; }

        public long VendorId { get; set; }

        public Vendor Vendor { get; set; }

    }
}
