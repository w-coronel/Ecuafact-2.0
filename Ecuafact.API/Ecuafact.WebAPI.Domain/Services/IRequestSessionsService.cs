using Ecuafact.WebAPI.Domain.Entities;

namespace Ecuafact.WebAPI.Domain.Services
{
    public interface IRequestSessionsService
    {
        RequestSession GetSessionByToken(string token);

        RequestSession GenerateSession(string user, string issuer, string token);
    }
}
