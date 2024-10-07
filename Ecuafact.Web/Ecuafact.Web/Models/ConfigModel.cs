using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.Domain.Entities.API;
using Ecuafact.Web.MiddleCore.NexusApiServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecuafact.Web.Models
{
    public class ConfigModel
    {

        public IssuerDto Issuer { get; set; } = SessionInfo.Issuer;
        public ClientModel UserProfile { get; set; } = SessionInfo.UserInfo;
        public DeductibleLimitResponse Budget { get; set; }

        public ConfigModel()
        {            
        }
    }
}