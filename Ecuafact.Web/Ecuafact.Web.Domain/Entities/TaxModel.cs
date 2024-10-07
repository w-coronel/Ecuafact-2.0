using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities
{
    public class TaxModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string PercentageCode { get; set; }
        public decimal Rate { get; set; }
        public decimal TaxableBase { get; set; }
        public decimal TaxValue { get; set; }

    }
    
}
