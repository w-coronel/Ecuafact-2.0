using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities
{

    public class UserSession
    {
        public string Token { get; set; }

        public long IssuerId { get; set; }

        public string IssuerRUC { get; set; }

        public string Username { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ClosedOn { get; set; }

        public bool IsEnabled { get; set; }

        public string Software { get; set; }

        public IssuerDto Issuer { get; set; }

        public bool SRIConnected { get; set; }

        public List<string> Messages { get; set; } = new List<string>();

        public ElectronicSignModel ElectronicSign { get; set; }

        public SubscriptionModel Subscription { get; set; }
    }
}
