using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Pagination
{
    public class Pagination
    {       
        private int _pagesize = 10;
        private int _pageNumber = 1;
        public int PageNumber
        {
            get
            {
                return _pageNumber;
            }
            set
            {
                _pageNumber = ((value <= 0) ? 1 : value);
            }
        }

        public int PageSize
        {
            get
            {
                return _pagesize;
            }
            set
            {
                _pagesize = ((value > 50) ? 50 : ((value < 5) ? 10 : value));
            }
        }
        public string OrderBy { get; set; }
        public bool OrderAsc { get; set; } = true;
    }
}
