using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities
{
    public class CountryDto
    {
        public string Name => "ECUADOR";
        public List<ProvinceDto> Provinces => new List<ProvinceDto>
        {
            new ProvinceDto{ Name = "", Cities = new List<string>{ } }

        };
    }
    public class ProvinceDto
    {
        public string Name { get; set; }
        public List<string> Cities { get; set; }
    }
}
