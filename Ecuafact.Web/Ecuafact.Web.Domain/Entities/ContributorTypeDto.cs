using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities
{
    public class ContributorTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SriCode { get; set; }
        public class Input
        {
            public string TokenId { get; set; }
        }
    }
}
