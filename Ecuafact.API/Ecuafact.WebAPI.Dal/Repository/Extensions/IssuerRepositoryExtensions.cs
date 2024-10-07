using System.Data.Entity;
using System.Linq;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Repository;

namespace Ecuafact.WebAPI.Dal.Repository.Extensions
{
    public static class IssuerRepositoryExtensions
    {
        public static Issuer One(this IEntityRepository<Issuer> entityRepository, long id)
        {
            return entityRepository.All.Include(s => s.Establishments.Select(x => x.IssuePoint)).FirstOrDefault(iss => iss.Id == id);
            //return entityRepository.All.FirstOrDefault(iss => iss.Id == id);
        }

        public static Issuer GetIssuerByRuc(this IEntityRepository<Issuer> entityRepository, string ruc)
        {            
            //return entityRepository.All.FirstOrDefault(iss => iss.RUC.Equals(ruc) || iss.RUC.StartsWith(ruc));
            return entityRepository.FindBy(iss => iss.RUC.Equals(ruc) || iss.RUC.StartsWith(ruc)).Include(s=> s.Establishments.Select(x=> x.IssuePoint)).FirstOrDefault();
        } 
    }
}