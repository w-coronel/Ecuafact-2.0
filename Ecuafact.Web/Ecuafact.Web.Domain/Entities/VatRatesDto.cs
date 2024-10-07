using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities
{
    
    public class IvaRatesDto: CatalogBaseDto
    {
        public decimal RateValue { get; set; }
    }

    public class ProductServicesEcuafact
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }


}
