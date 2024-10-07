using Ecuafact.WebAPI.Domain.Entities;
using System.Collections.Generic;

namespace Ecuafact.WebAPI.Domain.Services
{
    public interface IAtsService
    {
        OperationResult<AtsReporte> GetReportAts(string ruc, PeriodTypeEnum periodType, int year, int month, int semester);
        OperationResult<List<VwAtsFactura>> GetReportInvoiceAts(string ruc, PeriodTypeEnum periodType, int year, int month, int semester);
        OperationResult<StatisticsAts> GetStatisticsAts(string ruc, PeriodTypeEnum periodType, int year, int month, int semester);
        OperationResult<VwAtsFactura> GetPurchasesAtsById(int id, int typeIssuance);
    }
}
