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
    public class Establishments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }
        [Required]
        [StringLength(3)]
        public string Code { get; set; }
        [StringLength(500)]
        public string Name { get; set; }
        [MaxLength(300)]
        public string Address { get; set; }        
        public long IssuerId { get; set; }
        [JsonIgnore]
        public Issuer Issuer { get; set; }
        public List<IssuePoint> IssuePoint { get; set; }

    }

    public class IssuePoint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }
        [Required]
        [StringLength(3)]
        public string Code { get; set; }
        [StringLength(300)]
        public string Name { get; set; }        
        public long EstablishmentsId { get; set; }
        public long IssuerId { get; set; }
        public string CarrierRUC { get; set; }
        public string CarPlate { get; set; }
        public string CarrierEmail { get; set; }
        public string CarrierPhone { get; set; }
        public string CarrierContribuyente { get; set; }
        public string CarrierResolutionNumber { get; set; }        
        [JsonIgnore]
        public Establishments Establishments { get; set; }
    }
}
