using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Repository;

namespace Ecuafact.WebAPI.Dal.Repository.Extensions
{
    public static class ProductRepositoryExtensions
    {
        public static IQueryable<Product> GetProductsByIssuer(this IEntityRepository<Product> productRepository, long issuerId)
        {
            return productRepository.GetAll().Where(pr => pr.IssuerId == issuerId);
        }

        public static Product One(this IEntityRepository<Product> productRepository, long productId, long issuerId)
        {
            return productRepository.FindBy(pr => pr.Id == productId && pr.IssuerId == issuerId).FirstOrDefault();
        }

        public static IEnumerable<Product> Search(this IEntityRepository<Product> productRepository, string searchTerm, long issuerId)
        {
            SqlParameter issuerIdParam = new SqlParameter("@issuerId", SqlDbType.BigInt) { Value = issuerId };
            SqlParameter searchTermParam = new SqlParameter("@searchTerm", SqlDbType.NVarChar) { Value = searchTerm };

            var searchResult = productRepository.ExecSearchesWithStoreProcedure("SpSearchProducts @searchTerm, @issuerId", searchTermParam,issuerIdParam);
            return searchResult;
        }
    }
}
