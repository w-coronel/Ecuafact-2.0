using Ecuafact.WebAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Services
{
    public interface IUserService
    {
        List<UserPermissions> GetIssuersByUser(string user);
        OperationResult<UserPermissions> SetPermissions(string user, long issuer, UserRolEnum? role = null, string modules = null);
        OperationResult RevokePermissions(string user, long issuerId);
        OperationResult HasPermissions(string user, string ruc);
        OperationResult HasPermissions(string user, int issuerid);
        OperationResult<UserCampaigns> SetUserCampaigns(UserCampaigns model);
        OperationResult<UserPayment> AddUserPayment(UserPayment newUserPayment);
        OperationResult<UserPayment> UpdateUserPayment(UserPayment userPaymentUpdate);
        OperationResult<UserPayment> GetUserPaymentByRuc(string ruc);       
    }
}
