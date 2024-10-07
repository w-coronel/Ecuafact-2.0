using Ecuafact.WebAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecuafact.WebAPI.Models.Dtos
{
    public class UserSessionDto
    {
        public string Token { get; set; }
        public long IssuerId { get; set; }
        public string IssuerRUC { get; set; }
        public string Username { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ClosedOn { get; set; }
        public bool IsEnabled { get; set; }
        public string Software { get; set; }
        public Issuer Issuer { get; set; }
        public SubscriptionDto Subscription { get; set; }
        public bool SRIConnected { get; set; }        
        public List<string> Messages { get; set; } = new List<string>();
        
    }
}