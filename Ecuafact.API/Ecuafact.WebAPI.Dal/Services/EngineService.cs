using Ecuafact.WebAPI.Dal.Core;
using Ecuafact.WebAPI.Dal.Repository;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Entities.Engine;
using Ecuafact.WebAPI.Domain.Repository;
using Ecuafact.WebAPI.Domain.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Dal.Services
{
    public class EngineService : IEngineService
    {
        private readonly IEntityRepository<Emisor> _emisorRepository;
        private readonly IEntityRepository<Documento> _documentRepository;
        private readonly IEntityRepository<LogProceso> _logRepository;
        private readonly IEntityRepository<EmisorTextoInfoAdicional> _emisorTexInfoAdicRepository;
        private readonly IEntityRepository<Emisor> _emisorRepository2;
        private readonly IEntityRepository<EmisorTextoInfoAdicional> _emisorTexInfoAdicRepository2;

        private EcuafactEngineContext DB = new EcuafactEngineContext();
        private EcuafactEngineContext2 DB2 = new EcuafactEngineContext2();

        public EngineService()
        {
            _emisorRepository = new EntityRepository<Emisor>(DB);
            _documentRepository = new EntityRepository<Documento>(DB);
            _logRepository = new EntityRepository<LogProceso>(DB);
            _emisorTexInfoAdicRepository = new EntityRepository<EmisorTextoInfoAdicional>(DB);
            _emisorRepository2 = new EntityRepository<Emisor>(DB2);
            _emisorTexInfoAdicRepository2 = new EntityRepository<EmisorTextoInfoAdicional>(DB2);
        }

        public OperationResult  UpdateEmisor(Issuer issuer)
        { 
            try
            {
                var emisor = _emisorRepository.All.FirstOrDefault(pr => pr.RUC.Equals(issuer.RUC));
                var emisor2= _emisorRepository2.All.FirstOrDefault(pr => pr.RUC.Equals(issuer.RUC));
                if (emisor == null)
                {
                    emisor = new Emisor(issuer);
                    _emisorRepository.Add(emisor);                    
                }
                else
                {
                    emisor.MapTo(issuer); 
                }

                if (emisor2 == null)
                {
                    emisor2 = new Emisor(issuer);
                    _emisorRepository2.Add(emisor2);
                }
                else
                {
                    emisor2.MapTo(issuer);
                }

                //Se registra en la base de datos del ExpressGeneral
                _emisorRepository.Save();
                //Se registra en la base de datos del ExpressGeneral2
                _emisorRepository2.Save();

                EmisorTextoInfoAdicional(issuer);

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

                return new OperationResult (false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el emisor" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult (false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el emisor" };
            }
            catch (Exception e)
            {
                return new OperationResult<Issuer>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar el emisor" };
            }
        }       

        public DocumentStatusInfo GetDocumentInfo(long id)
        {
            var doc = _documentRepository.All
                .FirstOrDefault(pr => pr.Estado > 0 && pr.PkDocumento == id.ToString());

            if (doc != null)
            {
                var msg = "OK";

                if (doc.Estado != 100 || string.IsNullOrEmpty(doc.fechaAutorizacion))
                {
                    msg = _logRepository.All
                        .OrderByDescending(p => p.Id)
                        .FirstOrDefault(p => p.pkDocumento == doc.pk)?.Error ?? doc.StrEstado;
                } 

                return new DocumentStatusInfo
                {
                    AccessKey = doc.claveAcceso,
                    AuthorizationDate = doc.fechaAutorizacion,
                    AuthorizationNumber = doc.numeroautorizacion,
                    State = doc.Estado?.ToString(),
                    StatusMsg = msg,
                };
            }

            return null;
        }

        public OperationResult AddEmisorTextoInfoAdicional(List<EmisorTextoInfoAdicional> models)
        {
            try
            {
                _emisorTexInfoAdicRepository.AddRange(models);
                _emisorTexInfoAdicRepository2.AddRange(models);
                _emisorTexInfoAdicRepository.Save();
                _emisorTexInfoAdicRepository2.Save();
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

                return new OperationResult(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar la información adicional del emisor" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar la información adicional del emisor" };
            }
            catch (Exception e)
            {
                return new OperationResult<EmisorTextoInfoAdicional>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar la información adicional del emisor" };
            }
        }

        public OperationResult UpdateEmisorTextoInfoAdicional(EmisorTextoInfoAdicional model)
        {
            try
            {
                if(model.Id == 0)
                {
                    _emisorTexInfoAdicRepository.Add(model);
                    _emisorTexInfoAdicRepository2.Add(model);
                }               
                _emisorTexInfoAdicRepository.Save();
                _emisorTexInfoAdicRepository2.Save();
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

                return new OperationResult(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar la información adicional del emisor" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar la información adicional del emisor" };
            }
            catch (Exception e)
            {
                return new OperationResult<EmisorTextoInfoAdicional>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar la información adicional del emisor" };
            }
        }

        public OperationResult DeleteEmisorTextoInfoAdicional(List<EmisorTextoInfoAdicional> models)
        {
            try
            {               
                _emisorTexInfoAdicRepository.DeleteRange(models);               
                _emisorTexInfoAdicRepository.Save();               
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

                return new OperationResult(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error al eliminar información adicional del emisor" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error al eliminar información adicional del emisor" };
            }
            catch (Exception e)
            {
                return new OperationResult<EmisorTextoInfoAdicional>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error al eliminar información adicional del emisor" };
            }
        }

        public OperationResult DeleteEmisorTextoInfoAdicional2(List<EmisorTextoInfoAdicional> models)
        {
            try
            {               

                _emisorTexInfoAdicRepository2.DeleteRange(models);
                _emisorTexInfoAdicRepository2.Save();
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

                return new OperationResult(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error al eliminar información adicional del emisor" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error al eliminar información adicional del emisor" };
            }
            catch (Exception e)
            {
                return new OperationResult<EmisorTextoInfoAdicional>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error al eliminar información adicional del emisor" };
            }
        }

        public OperationResult<List<EmisorTextoInfoAdicional>> GetEmisorTextoInfoAdicional(string ruc)
        {
            try
            {
                var emiTextInfAdic = _emisorTexInfoAdicRepository.FindBy(pr => pr.RucEmisor.Equals(ruc));
                if (emiTextInfAdic?.Count() > 0)
                {
                    return new OperationResult<List<EmisorTextoInfoAdicional>>(true, HttpStatusCode.OK) { Entity = emiTextInfAdic.ToList() };
                }

                return new OperationResult<List<EmisorTextoInfoAdicional>>(false, HttpStatusCode.NotFound) { Entity = null};

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

                return new OperationResult<List<EmisorTextoInfoAdicional>>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar la información adicional del emisor" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<List<EmisorTextoInfoAdicional>>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar la información adicional del emisor" };
            }
            catch (Exception e)
            {
                return new OperationResult<List<EmisorTextoInfoAdicional>>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar la información adicional del emisor" };
            }
        }

        public OperationResult<List<EmisorTextoInfoAdicional>> GetEmisorTextoInfoAdicional2(string ruc)
        {
            try
            {
                var emiTextInfAdic = _emisorTexInfoAdicRepository2.FindBy(pr => pr.RucEmisor.Equals(ruc));
                if (emiTextInfAdic?.Count() > 0)
                {
                    return new OperationResult<List<EmisorTextoInfoAdicional>>(true, HttpStatusCode.OK) { Entity = emiTextInfAdic.ToList() };
                }

                return new OperationResult<List<EmisorTextoInfoAdicional>>(false, HttpStatusCode.NotFound) { Entity = null };

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

                return new OperationResult<List<EmisorTextoInfoAdicional>>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar la información adicional del emisor" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<List<EmisorTextoInfoAdicional>>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar la información adicional del emisor" };
            }
            catch (Exception e)
            {
                return new OperationResult<List<EmisorTextoInfoAdicional>>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar la información adicional del emisor" };
            }
        }

        public void EmisorTextoInfoAdicional(Issuer issuer)
        {
            if(issuer != null)
            {               
                var texInfoAdicionals = new List<EmisorTextoInfoAdicional>();
                var texInfoAdic = GetEmisorTextoInfoAdicional(issuer.RUC);
                var texInfoAdic2= GetEmisorTextoInfoAdicional2(issuer.RUC);
                if (texInfoAdic.IsSuccess)
                {
                    DeleteEmisorTextoInfoAdicional(texInfoAdic.Entity);                    
                }
                if (texInfoAdic2.IsSuccess)
                {
                    DeleteEmisorTextoInfoAdicional2(texInfoAdic2.Entity);
                }
                if (issuer.IsRetentionAgent)
                {
                    var model = new EmisorTextoInfoAdicional(){                       
                        RucEmisor = issuer.RUC,
                        nombre = TextoInfoAdicionalEnum.IsRetentionAgent.GetDisplayValue(),
                        valor = issuer.AgentResolutionNumber
                    };
                    texInfoAdicionals.Add(model);
                }
                if (issuer.IsRimpe)
                {
                    var model = new EmisorTextoInfoAdicional(){                        
                        RucEmisor = issuer.RUC,
                        nombre = TextoInfoAdicionalEnum.IsRimpe.GetDisplayValue(),
                        valor = TextoInfoAdicionalEnum.IsRimpe.GetCoreValue()
                    };
                    texInfoAdicionals.Add(model);
                }
                if (issuer.IsGeneralRegime)
                {
                    var model = new EmisorTextoInfoAdicional(){                       
                        RucEmisor = issuer.RUC,
                        nombre = TextoInfoAdicionalEnum.IsGeneralRegime.GetDisplayValue(),
                        valor = TextoInfoAdicionalEnum.IsGeneralRegime.GetCoreValue()
                    };
                    texInfoAdicionals.Add(model);
                }
                if (issuer.IsSimplifiedCompaniesRegime)
                {
                    var model = new EmisorTextoInfoAdicional() {
                        
                        RucEmisor = issuer.RUC,
                        nombre = TextoInfoAdicionalEnum.IsSimplifiedCompaniesRegime.GetDisplayValue(),
                        valor = TextoInfoAdicionalEnum.IsSimplifiedCompaniesRegime.GetCoreValue()
                    };

                    texInfoAdicionals.Add(model);
                }
                if (issuer.IsSkilledCraftsman)
                {
                    var model = new EmisorTextoInfoAdicional() {
                      
                        RucEmisor = issuer.RUC,
                        nombre = TextoInfoAdicionalEnum.IsSkilledCraftsman.GetDisplayValue(),
                        valor = issuer.SkilledCraftsmanNumber
                    };
                    texInfoAdicionals.Add(model);
                }
                if (issuer.IsPopularBusiness)
                {
                    var model = new EmisorTextoInfoAdicional(){
                        RucEmisor = issuer.RUC,
                        nombre = TextoInfoAdicionalEnum.IsPopularBusiness.GetDisplayValue(),
                        valor = TextoInfoAdicionalEnum.IsPopularBusiness.GetCoreValue()
                    };

                    texInfoAdicionals.Add(model);
                }

                if (texInfoAdicionals.Count > 0)
                {
                    AddEmisorTextoInfoAdicional(texInfoAdicionals);
                }
            }
        }
       
    }
}
