
using System.Collections.Generic;
namespace Ecuafact.WebAPI.Domain.Pagination
{
    public class PaginationList<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public List<T> Data { get; set; } = new List<T>();
    }
}