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
    public class AdditionalField
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }

        [MaxLength(300)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string Value { get; set; }

        public int LineNumber { get; set; }

        public short? IsCarrier { get; set; } = 0;       

        [JsonIgnore]
        public Document Document { get; set; }
        public long DocumentId { get; set; }
    }   
}
