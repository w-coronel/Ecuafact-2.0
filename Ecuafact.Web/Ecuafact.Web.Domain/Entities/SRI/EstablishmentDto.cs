using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities.SRI
{
    public class EstablishmentDto
    {
        public long Id { get; set; }
        public string EstablishmentNumber { get; set; }
        public string CommercialName { get; set; }
        public string Street { get; set; }
        public string AddressNumber { get; set; }
        public string Intersection { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Town { get; set; }
        public string CIIU { get; set; }
        public string Activity { get; set; }
        public SRIEstabStatus Status { get; set; }
        public string FullAddress { get; set; }
    }

    public enum SRIEstabStatus
    {
        Closed, Open
    }

}