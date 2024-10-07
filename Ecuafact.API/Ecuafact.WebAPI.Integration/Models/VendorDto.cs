using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecuafact.WebAPI.Integration.Models
{
    public class VendorDto
    {
        public long Id { get; set; }
        public Guid AppId { get; set; }
        public string ApplicationName { get; set; }
        public string BusinessName { get; set; }
        public string ApiKey { get; set; }
    }
}