using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Repository;

namespace Ecuafact.WebAPI.Dal.Repository.Extensions
{
    public static class TaxRepositoryExtensions
    {
        public static IEnumerable<RetentionTax> GetTaxes(this IEntityRepository<RetentionTax> taxRepository)
        {
            return taxRepository.GetAll();
        }
        public static IEnumerable<RetentionRate> GetTaxRates(this IEntityRepository<RetentionRate> taxRepository)
        {
            return taxRepository.GetAll();
        }

        public static RetentionTax One(this IEntityRepository<RetentionTax> taxRepository, long taxId)
        {
            return taxRepository.FindBy(pr => pr.Id == taxId).FirstOrDefault();
        }
        
    }
}
