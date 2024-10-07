using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class DocumentSequential
    {
        [Key]
        [Column(Order = 1)]
        [Required(ErrorMessage = "{0} es requerido")]
        public long IssuerId { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(5)]
        [Required(ErrorMessage = "{0} es requerido")]
        public string EstablishmentCode { get; set; }

        [Key]
        [Column(Order = 3)]
        [Required(ErrorMessage = "{0} es requerido")]
        [StringLength(5)]
        public string IssuePointCode { get; set; }

        [Key]
        [Column(Order = 4)]
        [Required(ErrorMessage = "{0} es requerido")]
        public short DocumentTypeId { get; set; }

        [StringLength(100)]
        public string DocumentType { get; set; }

        public long Sequential { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
