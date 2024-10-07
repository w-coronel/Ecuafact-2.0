using System.Collections.Generic;
using System.Linq;
using Ecuafact.WebAPI.Domain.Entities;

namespace Ecuafact.WebAPI.Domain.Services
{
    public interface ITaxesService
    {
        RetentionTax GetTax(long taxId);

        RetentionTax GetTaxByCode(string taxCode);

        IQueryable<RetentionTax> GetAllTaxes();

        OperationResult<RetentionTax> AddTax(RetentionTax newTax);

        OperationResult<RetentionTax> UpdateTax(RetentionTax taxToUpdate);

        IQueryable<RetentionTax> SearchTaxes(string searchTerm, long? type = null);


    }
}
