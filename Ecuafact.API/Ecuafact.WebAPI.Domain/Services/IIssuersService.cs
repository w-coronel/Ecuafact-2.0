using Ecuafact.WebAPI.Domain.Entities;
using System.Linq;

namespace Ecuafact.WebAPI.Domain.Services
{
    public interface IIssuersService
    {
        #region Issuer
        Issuer GetIssuer(long id);
        Issuer GetIssuer(string issuerRuc);
        bool Exists(object issuer);
        OperationResult<Issuer> AddIssuer(Issuer model, string user = null);
        OperationResult<Issuer> UpdateIssuer(Issuer model, string user = null);
        #endregion

        #region Establishments y IssuePoint
        OperationResult<Establishments> GetEstablishmentsById(long id);
        IQueryable<Establishments> GetEstablishments(long issuerId);
        OperationResult<Establishments> AddEstablishments(Establishments model);
        OperationResult<Establishments> UpdateEstablishments(Establishments model);
        OperationResult<IssuePoint> GetIssuePointById(long id);
        IQueryable<IssuePoint> GetIssuePoint(long establishmentId);
        IQueryable<IssuePoint> GetIssuePointByIssuer(long issuerId);
        OperationResult<IssuePoint> AddIssuePoint(IssuePoint model);
        OperationResult<IssuePoint> UpdateIssuePoint(IssuePoint model);
        OperationResult<IssuePoint> AddIssuePointCarrier(IssuePoint model);
        OperationResult<IssuePoint> UpdateIssuePointCarrie(IssuePoint model);
        OperationResult DeleteIssuePointCarrie(IssuePoint model);
        #endregion
    }
}
