using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities
{
    public class PaymentModel
    {
        public string PaymentMethodCode { get; set; }
        public string Name { get; set; }
        public decimal Total { get; set; }
        public int Term { get; set; }
        public string TimeUnit { get; set; }

    }
    
}
