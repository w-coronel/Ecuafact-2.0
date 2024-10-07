using Ecuafact.WebAPI.Dal.Repository;
using Ecuafact.WebAPI.Dal.Repository.Extensions;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Repository;
using Ecuafact.WebAPI.Domain.Services;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;

namespace Ecuafact.WebAPI.Dal.Services
{
    public class IssuersService : EntityRepository<Issuer>, IIssuersService
    {
        private readonly IEntityRepository<Issuer> _issuerService;
        private readonly IEngineService _engineService;
        private readonly IUserService _userService;
        private readonly IEntityRepository<Beneficiary> _beneficiaryService;
        private readonly IEntityRepository<Establishments> _establishmentService;
        private readonly IEntityRepository<IssuePoint> _issuePointService;

        public IssuersService(IEntityRepository<Issuer> issuerRepository, IEngineService engineService, IUserService userService, 
            IEntityRepository<Beneficiary> beneficiaryService, IEntityRepository<Establishments> establishmentService, IEntityRepository<IssuePoint> issuePointService, DbContext entitiesContext) 
            : base(entitiesContext)
        {
            _engineService = engineService;
            _issuerService = issuerRepository;
            _userService = userService;
            _beneficiaryService = beneficiaryService;
            _establishmentService = establishmentService;
            _issuePointService = issuePointService;
        }

        #region  Issuer
        public Issuer GetIssuer(long id)
        {
            return _issuerService.One(id);
        }

        public Issuer GetIssuer(string issuerRuc)
        {
            return _issuerService.GetIssuerByRuc(issuerRuc);
        }

        public bool Exists(object id)
        {
            if (id is long)
            {
                return _issuerService.Exists(model => model.Id == (long)id);
            }

            return _issuerService.Exists(model => model.RUC == (string)id || model.BussinesName == (string)id);
        }

        public OperationResult<Issuer> AddIssuer(Issuer model, string username = null)
        {
            var issuerId = _issuerService.FindBy(pr => pr.RUC.Equals(model.RUC)).Select(m => m.Id).FirstOrDefault();

            if (issuerId > 0)
                return new OperationResult<Issuer>(false, HttpStatusCode.Conflict) { DevMessage = "El Emisor ya existe", UserMessage = "El Emisor que desea crear, ya existe." };

            try
            {                
                _issuerService.Add(model);
                _issuerService.Save();

                //Agregamos el emisor en la base de datos del Engine:
                _engineService.UpdateEmisor(model);
                
                //Configuramos los permisos del usuario:
                SetPermissions(model, username);

                //Verificar si es benificiario:
                Beneficiary(model);

                return new OperationResult<Issuer>(true, HttpStatusCode.OK)
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

                return new OperationResult<Issuer>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el emisor" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<Issuer>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el emisor" };
            }
            catch (Exception e)
            {
                return new OperationResult<Issuer>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar el emisor" };
            }
        }

        public OperationResult<Issuer> UpdateIssuer(Issuer model, string username = null)
        {
            try
            {

                this.ValidateDependants(model);

                //_issuerRepository.Edit(model);
                _issuerService.Save();

                //ACtualizamos el emisor en la base de datos del Engine:
                _engineService.UpdateEmisor(model);

                //Configuramos los permisos del usuario:
                SetPermissions(model, username);

                //Verificar si es benificiario:
                Beneficiary(model);


                return new OperationResult<Issuer>(true, HttpStatusCode.OK)
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

                return new OperationResult<Issuer>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el emisor" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<Issuer>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el emisor" };
            }
            catch (Exception e)
            {
                return new OperationResult<Issuer>(false, HttpStatusCode.InternalServerError) { DevMessage = e.Message, UserMessage = "Ocurrio un error guardar el emisor" };
            }
        }

        private OperationResult<UserPermissions> SetPermissions(Issuer model, string username)
        {
            if (!string.IsNullOrEmpty(username))
            {

                // Ahora agregamos la autorizacion para este usuario que use al emisor creado:
                var userRole = (username == model.RUC) ? UserRolEnum.Admin : UserRolEnum.User;

                return _userService.SetPermissions(username, model.Id, userRole);
            }

            return new OperationResult<UserPermissions>(false, HttpStatusCode.NotModified);
        }

        private OperationResult<Beneficiary> Beneficiary(Issuer model)
        {            
            try
            {
                var beneficiary = _beneficiaryService.FindBy(pr => pr.Identification.Equals(model.RUC.Substring(0, 10))).FirstOrDefault();

                if (beneficiary != null)
                {
                    if (beneficiary.IssuerId == 0)
                    {
                        beneficiary.Status = BeneficiaryStatusEnum.Activo;
                        beneficiary.StatusMsg = "Se ha registrado en el Express.";
                        beneficiary.IssuerId = model.Id;
                        beneficiary.LastModifiedOn = DateTime.Now;                        
                        _beneficiaryService.Save();
                    }
                }
                return new OperationResult<Beneficiary>(true, HttpStatusCode.OK) { DevMessage = "OK", UserMessage = "Registro actualizado" };
                
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

                return new OperationResult<Beneficiary>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error actualizar benificiario" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<Beneficiary>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error actualizar benificiario" };
            }
            catch (Exception e)
            {
                return new OperationResult<Beneficiary>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error actualizar benificiario" };
            }
        }

        private void ValidateDependants(Issuer issuer)
        {
            if (issuer.Establishments != null)
            {
                var establishments = issuer.Establishments.Where(x => x.Id > 0);
                if (establishments.Any() && issuer.Establishments.Any(x => x.Id == 0))
                {
                    establishments.Where(x => x.IssuePoint != null).ToList()
                        .ForEach(p => DataContext.Set<IssuePoint>()
                            .RemoveRange(p.IssuePoint));

                    DataContext.Set<Establishments>()
                        .RemoveRange(establishments);
                }
            }

        }

        #endregion

        #region Establishments & IssuePoint

        public IQueryable<Establishments> GetEstablishments(long issuerId)
        {
            try
            {
                return _establishmentService.FindBy(pr => pr.IssuerId == issuerId);

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

            }
            catch (DbUpdateException dbex)
            {
                var msg = "";
                msg = $"{dbex.Message}";
            }
            catch (Exception e)
            {
                var msg = "";
                msg = $"{e.Message}";
            }
            return null;
        }
        public OperationResult<Establishments> GetEstablishmentsById(long id)
        {

            try
            {
                var _establishment = _establishmentService.FindBy(s => s.Id == id).FirstOrDefault();

                return new OperationResult<Establishments>(true, HttpStatusCode.OK)
                {
                    Entity = _establishment
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

                return new OperationResult<Establishments>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error al listar el establecimiento" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<Establishments>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error al listar el establecimiento" };
            }
            catch (Exception e)
            {
                return new OperationResult<Establishments>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error al listar el establecimiento" };
            }
        }
        public OperationResult<Establishments> AddEstablishments(Establishments model)
        {
            try
            {
                var establishment = _establishmentService.FindBy(s => s.Code == model.Code && s.IssuerId == model.IssuerId).FirstOrDefault();
                if (establishment != null)
                {
                    return new OperationResult<Establishments>(false, HttpStatusCode.Conflict) { DevMessage = $"Ya existe un establecimiento con el código {model.Code}", UserMessage = $"Ya existe un establecimiento con el cócdigo {model.Code}" };
                }
                _establishmentService.Add(model);
                _establishmentService.Save();

                return new OperationResult<Establishments>(true, HttpStatusCode.OK)
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

                return new OperationResult<Establishments>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el establecimiento" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<Establishments>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el establecimiento" };
            }
            catch (Exception e)
            {
                return new OperationResult<Establishments>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar el establecimiento" };
            }
        }
        public OperationResult<Establishments> UpdateEstablishments(Establishments model)
        {

            try
            {
                var establishment = _establishmentService.FindBy(s => s.Id == model.Id).FirstOrDefault();
                if (establishment != null)
                {
                    if (establishment.Code != model.Code)
                    {
                        var _iestablishment = _establishmentService.FindBy(pr => pr.Code.Equals(model.Code) && pr.IssuerId == model.IssuerId).FirstOrDefault();
                        if (_iestablishment != null)
                        {
                            return new OperationResult<Establishments>(false, HttpStatusCode.Conflict) { DevMessage = $"Ya existe un establecimiento con el código {model.Code}", UserMessage = $"Ya existe un establecimiento con el código {model.Code}" };
                        }
                    }
                }
                establishment.Code = model.Code;
                establishment.Name = model.Name;
                establishment.Address = model.Address;
                _establishmentService.Save();

                return new OperationResult<Establishments>(true, HttpStatusCode.OK)
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

                return new OperationResult<Establishments>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error al actualizar el establecimiento" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<Establishments>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error al actualizar el establecimiento" };
            }
            catch (Exception e)
            {
                return new OperationResult<Establishments>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error al actualizar el establecimiento" };
            }
        }
        public OperationResult<IssuePoint> GetIssuePointById(long id)
        {

            try
            {
                var _issuePoint = _issuePointService.FindBy(s => s.Id == id).FirstOrDefault();

                return new OperationResult<IssuePoint>(true, HttpStatusCode.OK)
                {
                    Entity = _issuePoint
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

                return new OperationResult<IssuePoint>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error al listar el punto de emisisión" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<IssuePoint>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error al listar el punto de emisisión" };
            }
            catch (Exception e)
            {
                return new OperationResult<IssuePoint>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error al listar el punto de emisisión" };
            }
        }
        public IQueryable<IssuePoint> GetIssuePoint(long establishmentId)
        {
            try
            {
                return _issuePointService.FindBy(pr => pr.EstablishmentsId == establishmentId);

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


            }
            catch (DbUpdateException dbex)
            {
                var msg = "";
                msg = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}";
            }
            catch (Exception e)
            {
                var msg = "";
                msg = e.ToString();
            }

            return null;
        }
        public IQueryable<IssuePoint> GetIssuePointByIssuer(long issuerId)
        {
            try
            {
                return _issuePointService.FindBy(pr => pr.IssuerId == issuerId);

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


            }
            catch (DbUpdateException dbex)
            {
                var msg = "";
                msg = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}";
            }
            catch (Exception e)
            {
                var msg = "";
                msg = e.ToString();
            }

            return null;
        }
        public OperationResult<IssuePoint> AddIssuePoint(IssuePoint model)
        {

            try
            {
                _issuePointService.Add(model);
                _issuePointService.Save();


                return new OperationResult<IssuePoint>(true, HttpStatusCode.OK)
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

                return new OperationResult<IssuePoint>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el punto de emisión" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<IssuePoint>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el punto de emisión" };
            }
            catch (Exception e)
            {
                return new OperationResult<IssuePoint>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar el punto de emisión" };
            }
        }
        public OperationResult<IssuePoint> UpdateIssuePoint(IssuePoint model)
        {

            try
            {
                _issuePointService.Save();

                return new OperationResult<IssuePoint>(true, HttpStatusCode.OK)
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

                return new OperationResult<IssuePoint>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error al actualizar el punto de emisión" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<IssuePoint>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error al actualizar el punto de emisión" };
            }
            catch (Exception e)
            {
                return new OperationResult<IssuePoint>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error al actualizar el punto de emisión" };
            }
        }
        public OperationResult<IssuePoint> AddIssuePointCarrier(IssuePoint model)
        {

            try
            { 
                //verificar que el ruc del transportista no sea nulo
                if (string.IsNullOrWhiteSpace(model.CarrierRUC))
                {
                    return new OperationResult<IssuePoint>(false, HttpStatusCode.Conflict) { DevMessage = "El ruc del transportista es obligatorio", UserMessage = "El ruc del transportista es obligatorio." };
                }
                //verificar que el ruc del transportista exista en el sistema
                var issuerId = _issuerService.FindBy(pr => pr.RUC.Equals(model.CarrierRUC)).FirstOrDefault();
                if (issuerId == null)
                {
                    return new OperationResult<IssuePoint>(false, HttpStatusCode.Conflict) { DevMessage = "El transportista no existe", UserMessage = $"No se puede crear el punto de emisión, el ruc {model.CarrierRUC} no esta registrado como emisor en el sistema" };
                }
                //verificar si ya hay un putno emisíon con el ruc del transportista
                var issuePoints = _issuePointService.FindBy(pr => pr.IssuerId == model.IssuerId).ToList();              
                if (issuePoints != null)
                {
                    if (issuePoints.Where(i => i.CarrierRUC == model.CarrierRUC && i.EstablishmentsId == model.EstablishmentsId)?.Count() > 0)
                    {
                        return new OperationResult<IssuePoint>(false, HttpStatusCode.Conflict) { DevMessage = $"Ya existe un punto emisión con el ruc {model.CarrierRUC}", UserMessage = $"Ya existe un punto emisión con el ruc {model.CarrierRUC}" };
                    }
                    if (issuePoints.Where(i => i.Code.Equals(model.Code) && i.EstablishmentsId == model.EstablishmentsId)?.Count() > 0)
                    {
                        return new OperationResult<IssuePoint>(false, HttpStatusCode.Conflict) { DevMessage = $"Ya existe un punto emisión con el código {model.Code}", UserMessage = $"Ya existe un punto emisión con el cócdigo {model.Code}" };
                    }
                    if (issuePoints.Where(i => i.CarrierRUC == model.CarrierRUC)?.Count() > 0)
                    {
                        return new OperationResult<IssuePoint>(false, HttpStatusCode.Conflict) { DevMessage = $"Ya existe un punto emisión con el ruc {model.CarrierRUC}", UserMessage = $"Ya existe un punto emisión con el ruc {model.CarrierRUC}" };
                    }
                }
                _issuePointService.Add(model);
                _issuePointService.Save();
                _userService.SetPermissions(issuerId.RUC, model.IssuerId, UserRolEnum.Cooperative, null);

                return new OperationResult<IssuePoint>(true, HttpStatusCode.OK)
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

                return new OperationResult<IssuePoint>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el punto de emisión" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<IssuePoint>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el punto de emisión" };
            }
            catch (Exception e)
            {
                return new OperationResult<IssuePoint>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar el punto de emisión" };
            }
        }
        public OperationResult<IssuePoint> UpdateIssuePointCarrie(IssuePoint model)
        {
            try
            {
                var issuePoint = new IssuePoint();
                var oldCarrierRUC = "";
                if (string.IsNullOrWhiteSpace(model.CarrierRUC))
                {
                    return new OperationResult<IssuePoint>(false, HttpStatusCode.Conflict) { DevMessage = "El ruc del transportista es obligatorio", UserMessage = "El ruc del transportista es obligatorio." };
                }
                var issuerId = _issuerService.FindBy(pr => pr.RUC.Equals(model.CarrierRUC)).FirstOrDefault();
                if (issuerId == null)
                {
                    return new OperationResult<IssuePoint>(false, HttpStatusCode.Conflict) { DevMessage = "El transportista no existe", UserMessage = $"No se puede editar el punto de emisión, el ruc {model.CarrierRUC} no esta registrado como emisor en el sistema" };
                }
                //se valida que el ruc del transportista y el código del punto emisión
                var issuePoints = _issuePointService.FindBy(s => s.IssuerId == model.IssuerId).ToList();
                if (issuePoints != null)
                {
                    issuePoint = issuePoints.Where(i => i.Id == model.Id).FirstOrDefault();
                    oldCarrierRUC = issuePoint.CarrierRUC;
                    if (issuePoint.CarrierRUC != model.CarrierRUC)
                    {
                        var _issuePoint = _issuePointService.FindBy(pr => pr.CarrierRUC.Equals(model.CarrierRUC) && pr.EstablishmentsId == model.EstablishmentsId).FirstOrDefault();
                        if (_issuePoint != null)
                        {
                            return new OperationResult<IssuePoint>(false, HttpStatusCode.Conflict) { DevMessage = $"Ya existe un punto emisón con el ruc {model.CarrierRUC}", UserMessage = $"Ya existe un punto emisón con el ruc {model.CarrierRUC}" };
                        }
                    }
                    if (issuePoint.Code != model.Code)
                    {
                        var _issuePoint = _issuePointService.FindBy(pr => pr.Code.Equals(model.Code) && pr.EstablishmentsId == model.EstablishmentsId).FirstOrDefault();
                        if (_issuePoint != null)
                        {
                            return new OperationResult<IssuePoint>(false, HttpStatusCode.Conflict) { DevMessage = $"Ya existe un punto emisón con el código {model.Code}", UserMessage = $"Ya existe un punto emisón con el código {model.Code}" };
                        }
                    }
                    if(issuePoints.Where(i=> i.CarrierRUC == model.CarrierRUC && i.EstablishmentsId != model.EstablishmentsId).Count() > 0)
                    {
                        return new OperationResult<IssuePoint>(false, HttpStatusCode.Conflict) { DevMessage = $"Ya existe un punto emisón con el ruc {model.CarrierRUC}", UserMessage = $"Ya existe un punto emisón con el ruc {model.CarrierRUC}" };
                    }  
                }

                issuePoint.Code = model.Code;
                issuePoint.CarrierRUC = model.CarrierRUC;
                issuePoint.CarrierEmail = model.CarrierEmail;
                issuePoint.CarrierPhone = model.CarrierPhone;
                issuePoint.CarPlate = model.CarPlate;
                issuePoint.Name = model.Name;
                issuePoint.CarrierContribuyente = model.CarrierContribuyente;
                issuePoint.CarrierResolutionNumber = model.CarrierResolutionNumber;

                _issuePointService.Edit(issuePoint);
                _issuePointService.Save();
                if (oldCarrierRUC != model.CarrierRUC)
                {
                    _userService.RevokePermissions(oldCarrierRUC, model.IssuerId);
                }               
                _userService.SetPermissions(issuerId.RUC, model.IssuerId, UserRolEnum.Cooperative, null);

                return new OperationResult<IssuePoint>(true, HttpStatusCode.OK)
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

                return new OperationResult<IssuePoint>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error al actualizar el punto de emisión" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<IssuePoint>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error al actualizar el punto de emisión" };
            }
            catch (Exception e)
            {
                return new OperationResult<IssuePoint>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error al actualizar el punto de emisión" };
            }
        }
        public OperationResult DeleteIssuePointCarrie(IssuePoint model)
        {
            try
            {
                var issuer = _issuerService.FindBy(pr => pr.RUC.Equals(model.CarrierRUC)).FirstOrDefault();
                _issuePointService.Delete(model);
                _issuePointService.Save();
                if (issuer != null) {
                    _userService.RevokePermissions(issuer.RUC, model.IssuerId);
                }
                return new OperationResult(true, HttpStatusCode.OK);
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

                return new OperationResult(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error al eliminar el punto de emisíon" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error al eliminar el punto de emisíon" };
            }
            catch (Exception e)
            {
                return new OperationResult<IssuePoint>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error al eliminar el punto de emisíon" };
            }
        }

        #endregion
    }
}
