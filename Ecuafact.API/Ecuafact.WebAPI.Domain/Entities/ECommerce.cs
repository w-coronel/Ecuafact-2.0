using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class ECommerce
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }
        public string Code { get; set; }        
        public string Name { get; set; }
        public OpeningModesEnum OpeningModes { get; set; }
        public string UrlCommerce { get; set; }
        public string UrlApiRestService { get; set; }
        public string UrlService { get; set; }
        public string ClientAppCode { get; set; }
        public string ClientAppKey { get; set; }
        public string ServerAppCode { get; set; }
        public string ServerAppKey { get; set; }
        public AmbientEnum Ambient  { get; set; }
        public string FileClaveService { get; set; }
        public string FileClaveModal { get; set; }
        public string IDCommerceCode { get; set; }
        public string IDCommerceMall { get; set; }
        public string IDAcquirer { get; set; }
        public string IDWalletCode { get; set; }
        public string TerminalNumber { get; set; }
        public bool IsEnabled { get; set; }
        public bool Priority { get; set; }        

    }


    public enum OpeningModesEnum : short
    {
        Modal = 1,
        Redirect = 2    
    }

    public enum AmbientEnum : short
    {
        Development = 1,
        Production = 2
    }


}
