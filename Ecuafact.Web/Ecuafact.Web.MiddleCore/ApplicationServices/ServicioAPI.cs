using Ecuafact.Web.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Ecuafact.Web.MiddleCore.ApplicationServices
{
    public class ServicioAPI
    {       
        public static UserSession RequestSession(string securityToken, string issuer)
        {
            var session = new UserSession();
            var httpClient = ClientHelper.GetClient(securityToken);

            var response = httpClient.PostAsync($"{Constants.WebApiUrl}/RequestSession?id={issuer}", new StringContent(string.Empty)).Result;

            if (response.IsSuccessStatusCode)
            {
                session = response.GetContent<UserSession>();
            }

            return session;
        }

        public static SubscriptionModel RequestSubscription(string securityToken, string issuer)
        {
            var subscription = new SubscriptionModel();
            var httpClient = ClientHelper.GetClient(securityToken);

            var response = httpClient.PostAsync($"{Constants.WebApiUrl}/ValidateSubscription?id={issuer}", new StringContent(string.Empty)).Result;

            if (response.IsSuccessStatusCode)
            {
                subscription = response.GetContent<SubscriptionModel>();
            }

            return subscription;
        }

    }
}
