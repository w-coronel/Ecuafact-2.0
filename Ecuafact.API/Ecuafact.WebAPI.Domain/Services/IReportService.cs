using Ecuafact.WebAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Services
{
    public interface IReportService
    {
        IEnumerable<SalesReport> GetSalesReport(string issuer, DateTime? dateStart, DateTime? dateEnd, string search);
        IEnumerable<PurchaseReport> GetPurchaseReport(string issuer, DateTime? dateStart, DateTime? dateEnd, string search);
    }
}
