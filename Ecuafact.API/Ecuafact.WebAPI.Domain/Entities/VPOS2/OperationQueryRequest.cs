using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Entities.VPOS2
{
    public class OperationQueryRequest
    { 
        public string idAcquirer { get; set; }
        public string idCommerce { get; set; }
        public string operationNumber { get; set; }
        public string purchaseVerification { get; set; }

    }
}
