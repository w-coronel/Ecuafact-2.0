using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class MediaCampaigns
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }
        public string Name { get; set; }     
        public string Url { get; set; }      
        public string utm_source { get; set; }
        public string utm_medium { get; set; }        
        public string utm_campaign { get; set; }
        public bool IsEnabled { get; set; }
        public long? ReferenceCodesId { get; set; }
        

    }

    public class UserCampaigns
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }
        public string Identification { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public long? UserId { get; set; }
        public string UserEntry { get; set; }
        public long MediaCampaignsId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        [NotMapped]
        public MediaCampaigns MediaCampaigns { get; set; }
    }

    
}
