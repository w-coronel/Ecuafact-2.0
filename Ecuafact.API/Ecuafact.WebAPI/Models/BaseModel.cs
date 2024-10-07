using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecuafact.WebAPI.Models
{
    public class BaseModel
    {
        public string draw { get; set; }
        public string recordsFiltered { get; set; }
        public string recordsTotal { get; set; }
    }
}