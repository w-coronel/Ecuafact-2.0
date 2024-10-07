using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities.SRI
{
    public class SRIContrib
    {
        public string RUC { get; set; }
        public string BusinessName { get; set; }
        public string TradeName { get; set; }

        public SRIContribStatus Status { get; set; }
        public SRIContribClass ContributorClass { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? LastUpdated { get; set; }
        public DateTime? SuspensionDate { get; set; }
        public DateTime? ReactivationDate { get; set; }
        public bool AccountingRequired { get; set; }
        public SRIContribType ContributorType { get; set; }

        public string MainAddress => this.GetMainAddress();

        public List<Establishment> Establishments { get; set; }
    }


    public enum SRIContribType
    {
        Natural, Juridical
    }

    public enum SRIContribStatus
    {
        Pasive, Active, Suspended
    }

    public enum SRIContribClass
    {
        Other,
        Special,
        RISE
    }
}
