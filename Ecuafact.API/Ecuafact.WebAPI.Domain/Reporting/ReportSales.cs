using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecuafact.WebAPI.Domain.Entities;
using Microsoft.Reporting.WebForms;

namespace Ecuafact.WebAPI.Domain.Reporting
{
    public class ReportSales : ReportBase<IEnumerable<SalesReport>>
    {
        public ReportSales(string search = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            Parameters.Add(new ReportParameter("searchTerm", search));
            Parameters.Add(new ReportParameter("startDate", startDate?.ToString("yyyy-MM-dd")));
            Parameters.Add(new ReportParameter("endDate", endDate?.ToString("yyyy-MM-dd")));
        }


        protected override List<ReportDataSource> GetDataSources(IEnumerable<SalesReport> documents)
        {
            return new List<ReportDataSource>
            {
                new ReportDataSource("ReportSales", documents) 
            };
        }

        protected override string ReportName => "SalesReport";
    }


    public class ReportPurchases : ReportBase<IEnumerable<PurchaseReport>>
    {
        public ReportPurchases(string search = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            Parameters.Add(new ReportParameter("searchTerm", search));
            Parameters.Add(new ReportParameter("startDate", startDate?.ToString("yyyy-MM-dd")));
            Parameters.Add(new ReportParameter("endDate", endDate?.ToString("yyyy-MM-dd")));
        }

        protected override List<ReportDataSource> GetDataSources(IEnumerable<PurchaseReport> documents)
        {
            return new List<ReportDataSource>
            {
                new ReportDataSource("PurchasesReport", documents)
            };
        }

        protected override string ReportName => "PurchaseReport";
    }
}
