using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Repository;

namespace Ecuafact.WebAPI.Dal.Repository.Extensions
{
    public static class ContributorRepositoryExtensions
    {
        public static IQueryable<Contributor> GetContributorsByIssuer(this IEntityRepository<Contributor> contributorRepository, long issuerId)
        {
            return contributorRepository.GetAll()
                .Include(contributor=>contributor.IdentificationType)
                .Where(pr => pr.IssuerId == issuerId);
        }

        public static Contributor One(this IEntityRepository<Contributor> entityRepository, long contributorId, long issuerId)
        {
            return entityRepository
                .FindBy(pr => pr.Id == contributorId && pr.IssuerId == issuerId)
                .Include(x=>x.IdentificationType)
                .FirstOrDefault();
        }

        public static Contributor One(this IEntityRepository<Contributor> entityRepository, string contributorRUC, long issuerId)
        {
            return entityRepository
                .FindBy(pr => pr.Identification == contributorRUC && pr.IssuerId == issuerId)
                .Include(x => x.IdentificationType)
                .FirstOrDefault();
        }

        public static IQueryable<Contributor> Search(this IEntityRepository<Contributor> contributorRepository, string searchTerm, long issuerId)
        {
            var searchResult = contributorRepository.GetContributorsByIssuer(issuerId)
                    .Where(x => x.Identification.Contains(searchTerm)
                        || x.BussinesName.Contains(searchTerm)
                        || x.TradeName.Contains(searchTerm));

            //SqlParameter issuerIdParam = new SqlParameter("@issuerId", SqlDbType.BigInt) { Value = issuerId };
            //SqlParameter searchTermParam = new SqlParameter("@searchTerm", SqlDbType.NVarChar) { Value = searchTerm };
            
            //var searchResult = contributorRepository.ExecSearchesWithStoreProcedure("SpSearchContributors @searchTerm, @issuerId", searchTermParam, issuerIdParam);
            return searchResult;
        }

    }
}
