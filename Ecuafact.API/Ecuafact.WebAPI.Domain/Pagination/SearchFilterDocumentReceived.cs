using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecuafact.WebAPI.Domain.Pagination
{
    public class FilterDocumentReceived: Pagination
    {
        public string Ruc { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int? DeductibleId { get; set; }
        public string CodTypeDoc { get; set; }
        public string Search { get; set; }        
    }
}