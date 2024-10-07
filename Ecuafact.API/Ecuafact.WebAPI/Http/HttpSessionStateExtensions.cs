using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Services;
using Ecuafact.WebAPI.Models;
using Ecuafact.WebAPI.Models.Authentication;

namespace Ecuafact.WebAPI.Http
{
    public static class HttpSessionStateExtensions
    {
        internal static IssuerDto GetAuthenticatedIssuerSession(this HttpSessionState session)
        {
            var issuer = session["issuer"] as IssuerDto;
            return issuer;
        }

        internal static OperationResult<LoginResponseModel> GetAuthenticatedUser(this HttpSessionState session)
        {
            var user = session["login"] as OperationResult<LoginResponseModel>;

            return user ?? new OperationResult<LoginResponseModel>(false, System.Net.HttpStatusCode.Unauthorized);
        }

        internal static UserRolEnum GetAuthenticatedUserRol(this HttpSessionState session)
        {
            var user = session["login"] as OperationResult<LoginResponseModel>; 
            var userRol = user?.Entity?.Issuers?.Where(s => s.UserRole == Domain.Entities.UserRolEnum.Cooperative)?.FirstOrDefault()?.UserRole ?? UserRolEnum.Admin;
            return userRol;
        }

        internal static IssuePoint GetissuePointCode(this HttpSessionState session)
        {
            var user = session["login"] as OperationResult<LoginResponseModel>;
            var issuer = session["issuer"] as IssuerDto;
            var _issuePoint = issuer?.Establishments?
                  .Where(c => c.Id == c.IssuePoint?.Where(i => i?.CarrierRUC == user.Entity.Client.Username)?
                  .FirstOrDefault()?.EstablishmentsId)
                  .FirstOrDefault()?.IssuePoint?.Where(d => d?.CarrierRUC == user.Entity.Client.Username)?.FirstOrDefault() ?? null;

            return _issuePoint;
        }

    }
}