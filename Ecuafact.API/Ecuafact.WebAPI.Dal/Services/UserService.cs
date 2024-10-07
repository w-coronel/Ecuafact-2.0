using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Repository;
using Ecuafact.WebAPI.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Net;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

namespace Ecuafact.WebAPI.Dal.Services
{
    public class UserService : IUserService
    {
        private readonly IEntityRepository<Issuer> _issuerRepository;
        private readonly IEntityRepository<UserPermissions> _userRepository;
        private readonly IEntityRepository<MediaCampaigns> _mediaCampaignsRepository;
        private readonly IEntityRepository<UserCampaigns> _userCampaignsRepository;
        private readonly IEntityRepository<Beneficiary> _beneficiaryRepository;
        private readonly IEntityRepository<BeneficiaryReferenceCode> _benRefeCodeRepository;
        private readonly IEntityRepository<ReferenceCodes> _referenceCodesRepository;
        private readonly IEntityRepository<UserPayment> _userPaymentRepository;

        public UserService(IEntityRepository<Issuer> issuerRepository, IEntityRepository<UserPermissions> userRepository, IEntityRepository<MediaCampaigns> mediaCampaignsRepository,
            IEntityRepository<UserCampaigns> userCampaignsRepository, IEntityRepository<Beneficiary> beneficiaryRepository, IEntityRepository<BeneficiaryReferenceCode> benRefeCodeRepository,
            IEntityRepository<ReferenceCodes> referenceCodesRepository, IEntityRepository<UserPayment> userPaymentRepository)
        {
            _issuerRepository = issuerRepository;
            _userRepository = userRepository;
            _mediaCampaignsRepository = mediaCampaignsRepository;
            _userCampaignsRepository = userCampaignsRepository;
            _beneficiaryRepository = beneficiaryRepository;
            _benRefeCodeRepository = benRefeCodeRepository;
            _referenceCodesRepository = referenceCodesRepository;
            _userPaymentRepository = userPaymentRepository;
        }

        public OperationResult<UserPermissions> SetPermissions(string user, long issuer, UserRolEnum? role = null, string modules = null)
        { 
            try
            {
                var permission = _userRepository.FindBy(pr => pr.IssuerId == issuer && pr.Username == user).FirstOrDefault();

                if (permission == null)
                {
                    permission = new UserPermissions
                    {
                        IssuerId = issuer,
                        Username = user,
                        Role = role ?? UserRolEnum.User,
                        Modules = modules
                    };

                    _userRepository.Add(permission);
                }
                else
                {
                    permission.Role = role ?? UserRolEnum.User;
                    permission.Modules = permission.Modules ?? modules;
                }

                _userRepository.Save();

                return new OperationResult<UserPermissions>(true, HttpStatusCode.OK)
                {
                    Entity = permission
                };
            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                var msg = "";
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }

                return new OperationResult<UserPermissions>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el permiso de usuario" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<UserPermissions>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el permiso de usuario" };
            }
            catch (Exception e)
            {
                return new OperationResult<UserPermissions>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar el permiso de usuario" };
            }
        }

        public OperationResult HasPermissions(string user, string ruc) 
        {
            try
            {
                if (user.Substring(0, 10) != ruc.Substring(0, 10))
                {
                    var issuer = _issuerRepository.All.FirstOrDefault(x => x.RUC == ruc);
                    
                    if (issuer == null)
                    {
                        return new OperationResult(false, HttpStatusCode.NotFound, "El emisor especificado no existe!");
                    }

                    var permission = _userRepository.All.FirstOrDefault(x => x.Username == user && x.IssuerId == issuer.Id);

                    if (permission == null)
                    {
                        return new OperationResult(false, HttpStatusCode.Unauthorized, "No Autorizado!");
                    }
                }

                return new OperationResult(true, HttpStatusCode.OK, "Acceso correcto!");
            }
            catch (Exception e)
            {
                return new OperationResult(e);
            }
        }

        public OperationResult HasPermissions(string user, int issuerid) 
        {
            try
            {
                var issuer = _issuerRepository.All.FirstOrDefault(x => x.Id == issuerid);

                if (issuer == null)
                {
                    return new OperationResult(false, HttpStatusCode.NotFound, "El emisor especificado no existe!");
                }

                if (user.Substring(0, 10) != issuer.RUC.Substring(0, 10))
                {
                    var permission = _userRepository.All.FirstOrDefault(x => x.IssuerId == issuer.Id);

                    if (permission == null)
                    {
                        return new OperationResult(false, HttpStatusCode.Unauthorized, "No Autorizado!");
                    }
                }

                return new OperationResult(true, HttpStatusCode.OK, "Acceso correcto!");
            }
            catch (Exception e)
            {
                return new OperationResult(e);
            }
        }

        public OperationResult RevokePermissions(string user, long issuerId)
        {
            try
            {
                // Primero localizamos los permisos para este usuario con el emisor:
                var permissions = _userRepository.All.Where(m => m.Username == user && m.IssuerId == issuerId);

                foreach (var item in permissions)
                {
                    _userRepository.Delete(item);
                }

                _userRepository.Save();
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "OK");
            }
            catch (Exception ex)
            {
                return new OperationResult(false, System.Net.HttpStatusCode.InternalServerError, $"{ex.Message} {ex.InnerException?.Message} ");
            }

        }

        public List<UserPermissions> GetIssuersByUser(string user)
        {
            var permissions = _userRepository.FindBy(model => model.Username.ToLower() == user.ToLower())
                                             .Include(p => p.Issuer);
            return permissions.ToList();
        }

        public OperationResult<UserCampaigns> SetUserCampaigns(UserCampaigns model)
        {
            try
            {
                var mediaCampaign = _mediaCampaignsRepository.FindBy(pr => pr.utm_source == model.MediaCampaigns.utm_source && pr.utm_medium == model.MediaCampaigns.utm_medium &&
                pr.utm_campaign == model.MediaCampaigns.utm_campaign).FirstOrDefault();
                
                if (mediaCampaign == null)
                {
                    return new OperationResult<UserCampaigns>(false, HttpStatusCode.Conflict) { DevMessage = "No existe Campaña" };
                }

                model.MediaCampaignsId = mediaCampaign.Id;
                model.UserEntry = mediaCampaign.Name;

                _userCampaignsRepository.Add(model);
                _userCampaignsRepository.Save();

                if (mediaCampaign.ReferenceCodesId.HasValue && mediaCampaign.ReferenceCodesId.Value > 0)
                {

                    var code = _referenceCodesRepository.FindBy(b => b.Id == mediaCampaign.ReferenceCodesId).FirstOrDefault();
                    if (code != null)
                    {
                        var beneficiary = new Beneficiary
                        {
                            Identification = model.Identification,
                            Name = model.Name,
                            Email = model.Email,
                            IsEnabled = true,
                            Status = 0,
                            AgreementId = code.AgreementId,
                            MediaCampaignsId = mediaCampaign.Id,
                        };

                        var beneRefeCode = new BeneficiaryReferenceCode
                        {
                            DiscountCode = code.Code,
                            Identification = model.Identification,
                            ReferenceCodId = code.Id,
                            Status = 0
                        };

                        _beneficiaryRepository.Add(beneficiary);
                        _benRefeCodeRepository.Add(beneRefeCode);

                        _beneficiaryRepository.Save();
                    }
                }

                return new OperationResult<UserCampaigns>(true, HttpStatusCode.OK)
                {
                    Entity = model
                };


            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                var msg = "";
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }

                return new OperationResult<UserCampaigns>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el regsitro" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<UserCampaigns>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el regsitro" };
            }
            catch (Exception e)
            {
                return new OperationResult<UserCampaigns>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar el regsitro" };
            }
        }

        public OperationResult<UserPayment> AddUserPayment(UserPayment newUserPayment)
        {
            try
            {
                var response = _userPaymentRepository.FindBy(u => u.OrderNumber == newUserPayment.OrderNumber).FirstOrDefault();
                if (response != null)
                {
                    return new OperationResult<UserPayment>(false, HttpStatusCode.Conflict) { DevMessage = "Numero Pedido", UserMessage = $"Ya existe el numero de pedido: {newUserPayment.OrderNumber}." };
                }
                _userPaymentRepository.Add(newUserPayment);
                _userPaymentRepository.Save();
                return new OperationResult<UserPayment>(true, HttpStatusCode.OK)
                {
                    Entity = newUserPayment
                };
            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                var msg = "";
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }

                return new OperationResult<UserPayment>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el pago del usuario" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<UserPayment>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el pago del usuario" };
            }
            catch (Exception e)
            {
                return new OperationResult<UserPayment>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar el pago del usuario" };
            }
        }

        public OperationResult<UserPayment> UpdateUserPayment(UserPayment userPaymentUpdate)
        {
            try
            {
                //_productRepository.Edit(productToUpdate);
                _userPaymentRepository.Save();
                return new OperationResult<UserPayment>(true, HttpStatusCode.OK)
                {
                    Entity = userPaymentUpdate
                };
            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                var msg = "";
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }

                return new OperationResult<UserPayment>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el pago del usuario" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<UserPayment>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el pago del usuario" };
            }
            catch (Exception e)
            {
                return new OperationResult<UserPayment>(false, HttpStatusCode.InternalServerError) { DevMessage = e.Message, UserMessage = "Ocurrio un error guardar el pago del usuario" };
            }
        }

        public OperationResult<UserPayment> GetUserPaymentByRuc(string ruc)
        {
            try
            {
                var userPayment = _userPaymentRepository.FindBy(pr => pr.Identification == ruc && pr.Status == UserPaymentStatusEnum.Validate).FirstOrDefault();
                if (userPayment != null)
                {
                    return new OperationResult<UserPayment>(false, HttpStatusCode.Conflict) { DevMessage = $"No hay pago registrado del Usuario: {ruc}" };
                }
                return new OperationResult<UserPayment>(true, HttpStatusCode.OK)
                {
                    Entity = userPayment
                };


            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                var msg = "";
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }

                return new OperationResult<UserPayment>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error obtener el pago del usuario" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<UserPayment>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un errorobtener el pago del usuario" };
            }
            catch (Exception e)
            {
                return new OperationResult<UserPayment>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error obtener el pago del usuario" };
            }

        }
    }
}
