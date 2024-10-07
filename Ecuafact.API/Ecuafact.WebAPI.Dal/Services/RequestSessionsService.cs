using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml.Serialization;
using Ecuafact.WebAPI.Dal.Core;
using Ecuafact.WebAPI.Dal.Repository;
using Ecuafact.WebAPI.Dal.Repository.Extensions;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Repository;
using Ecuafact.WebAPI.Domain.Services;
using Newtonsoft.Json;

namespace Ecuafact.WebAPI.Dal.Services
{
    public class RequestSessionsService : IRequestSessionsService
    { 
        private readonly IEntityRepository<RequestSession> _requestSessionRepository;
        private readonly IEntityRepository<Issuer> _issuerRepository;
        private readonly IEntityRepository<UserPermissions> _userRepository;

        public RequestSessionsService(IEntityRepository<RequestSession> requestSessionRepository, IEntityRepository<Issuer> issuerRepository, IEntityRepository<UserPermissions> userRepository)
        {
            _requestSessionRepository = requestSessionRepository;
            _issuerRepository = issuerRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Devuelve una sesion existente y si no existe la crea
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public RequestSession GenerateSession(string user, string issuer, string issuerToken)
        {
            // Se procede con la verificacion de la sesion:
            // Primero buscamos el emisor:

            var issuerInfo = _issuerRepository.GetIssuerByRuc(issuer);

            if (issuerInfo != null)
            {
                bool hasAccess = false;
                 
                // Luego verificamos el acceso a este emisor:
                // Esto analiza si le di acceso a mi informacion a alguien mas
                hasAccess = (user == "admin" || issuer.StartsWith(user)) || // Si yo soy el mismo emisor porque tengo que pedir permiso?
                    _userRepository.FindBy(p => p.Username == user && p.IssuerId == issuerInfo.Id).Any();
                  
                if (hasAccess)
                {
                    // Verifico si ya existe la sesión, de otra forma debe ser creada
                    var session = _requestSessionRepository.All.FirstOrDefault(
                        o => o.Username == user
                            && o.IssuerId == issuerInfo.Id
                            && o.Token == issuerToken);

                    if (session == null)
                    {
                        session = new RequestSession
                        {
                            Id = 0, // APLICACIONES EXTERNAS
                            Token = issuerToken,
                            Username = user, // Username
                            CreatedOn = DateTime.Now, // Current DateTime
                            IsEnabled = true,
                            Software = "EXTERNAL",
                            IssuerRUC = issuerInfo.RUC,
                            IssuerId = issuerInfo.Id
                        };

                        _requestSessionRepository.Add(session);
                        _requestSessionRepository.Save();
                    } 

                    // Se devuelve la sesion
                    return session;
                }
                else
                {
                    throw new Exception($"El usuario [{user}] no tiene acceso para administrar el emisor especificado!"); 
                }
            }
            else
            {
                throw new Exception($"No existe el emisor con el RUC # {issuer}!");
            }
        }
        

        public RequestSession GetSessionByToken(string token)
        {
            // Primero buscamos si existe una sesion registrada para este token
            var result = _requestSessionRepository.All.FirstOrDefault(o => o.Token == token);

            // Si ya existe una sesion devolvemos esa conexion de lo contrario analizamos el token
            if (result != null)
            {
                // Si el registro de sesion esta desactivado entonces el token es invalido
                if (result.IsEnabled)
                {
                    return result;
                }
            }
            else
            {
                // Desencryptamos el token y su informacion confidencial:
                var sessionData = EncryptionHelpers.Decrypt(token);

                // PROCESO DE ANALISIS DEL TOKEN CODIFICADO O SERIALIZADO
                // Si el token contiene un limitador esta codificado
                RequestSession session = sessionData.Contains("|") ?
                        DecodeToken(token, sessionData) :
                        JsonConvert.DeserializeObject<RequestSession>(sessionData);

                if (session != null)
                {
                    // Volvemos a buscar el codigo de la session
                    result = _requestSessionRepository.All.FirstOrDefault(o => o.Token == session.Token);

                    // En el caso que el token codificado si exista entonces se retorna el objeto
                    if (result != null)
                    {
                        if (result.IsEnabled)
                        {
                            return result;
                        } // Si el registro de sesion esta desactivado entonces el token es invalido
                    }
                    else
                    {
                        // Si el registro de la sesion con la informacion codificada no existe se tiene que registrar
                        // para los inicios de sesion posteriores
                        var repository = new EntityRepository<Issuer>(new EcuafactExpressContext());
                        var issuer = repository.GetIssuerByRuc(session.IssuerRUC);

                        if (issuer != null)
                        {
                            session.Id = 0;
                            session.IssuerId = issuer.Id;
                            session.CreatedOn = DateTime.Now;
                            session.Issuer = null;
                            session.IsEnabled = true;
                            // En el caso de los token de empresa se usa el mismo token encryptado.
                            session.Token = string.IsNullOrEmpty(session.Token) ? token : session.Token;

                            _requestSessionRepository.Add(session);
                            _requestSessionRepository.Save();

                            return session;
                        } // Si no existe el ruc del emisor el token es invalido

                    }
                } // Si los datos enviados no se pueden decodificar entonces el token es invalido
            }

            throw new System.Exception("Invalid Token!");

        }

        private static RequestSession DecodeToken(string token, string sessionData)
        {
            var arrayData = sessionData.Split('|');
            
            if (arrayData.Length > 2)
            {
                var user = arrayData[1];

                return new RequestSession
                {
                    Id = 0, // APLICACIONES EXTERNAS
                    Token = token,
                    Username = user, // Username
                    CreatedOn = DateTime.Now, // Current DateTime
                    IsEnabled = true,
                    Software = "EXTERNAL",
                    IssuerRUC = user
                };
            }

            return null;
        }

    }
}
