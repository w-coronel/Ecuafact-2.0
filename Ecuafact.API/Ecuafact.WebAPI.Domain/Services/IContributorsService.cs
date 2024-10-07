using System.Collections.Generic;
using System.Linq;
using Ecuafact.WebAPI.Domain.Entities;

namespace Ecuafact.WebAPI.Domain.Services
{
    public interface IContributorsService
    {
        Contributor GetFinalConsumer();
        Contributor GetContributorById(long contributorId, long issuerId);
        Contributor GetContributorByRUC(string contributorRUC, long issuerId);
        IQueryable<Contributor> GetContributors(long issuerId);
        OperationResult<Contributor> Add(Contributor newContributor);
        OperationResult<Contributor> Update(Contributor contributorToUpdate);
        IQueryable<Contributor> SearchContributors(string searchTerm, long issuerId);
        OperationResult<List<Contributor>> ImportContributor(List<Contributor> newContributor);
    }
}
