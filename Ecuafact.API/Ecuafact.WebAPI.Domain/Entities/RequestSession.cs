using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class RequestSession
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }

        [StringLength(100)]
        public string Token { get; set; }

        public long IssuerId { get; set; }

        public string IssuerRUC { get; set; }

        [StringLength(50)]
        public string Username { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ClosedOn { get; set; }

        public bool IsEnabled { get; set; }

        public string Software { get; set; }

        public Issuer Issuer { get; set; }

    }
}
