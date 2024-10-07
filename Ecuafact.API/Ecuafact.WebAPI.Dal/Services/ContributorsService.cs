using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using Ecuafact.WebAPI.Dal.Repository.Extensions;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Repository;
using Ecuafact.WebAPI.Domain.Services;

namespace Ecuafact.WebAPI.Dal.Services
{
    public class ContributorsService : IContributorsService
    {
        private readonly IEntityRepository<Contributor> _contributorRepository;
        private readonly IEntityRepository<IdentificationType> _identificationTypesRepository;

        public ContributorsService(IEntityRepository<IdentificationType> identificationTypesRepository,
                IEntityRepository<Contributor> contributorRepository)
        {
            _identificationTypesRepository = identificationTypesRepository;
            _contributorRepository = contributorRepository;
        }

        public Contributor GetFinalConsumer()
        {
            var idType = _identificationTypesRepository.All.FirstOrDefault(x => x.SriCode == "07") ?? 
                            new IdentificationType { Id = 0, SriCode = "07", Name = "Consumidor Final" };

            var contributor = new Contributor
            {
                Id = 0,
                Identification = "9999999999999",
                BussinesName = "Consumidor final",
                TradeName = "Consumidor final",
                Phone = "9999999",
                Address = "Av. Principal",
                IdentificationType = idType,
                IdentificationTypeId = idType?.Id ?? 0,
                EmailAddresses = "facturacion@ecuafact.com"
            };

            return contributor;
        }

        public Contributor GetContributorById(long contributorId, long issuerId)
        {
            return _contributorRepository.One(contributorId, issuerId);
        }

        public Contributor GetContributorByRUC(string contributorRUC, long issuerId)
        {
            return _contributorRepository.One(contributorRUC, issuerId);
        }

        public IQueryable<Contributor> GetContributors(long issuerId)
        {
            return _contributorRepository.GetContributorsByIssuer(issuerId);
        }

        public OperationResult<Contributor> Add(Contributor newContributor)
        {
            // Verifica si este registro existe en otro lugar
            var contributor = _contributorRepository
                .FindBy(cr => cr.Identification.Equals(newContributor.Identification)
                    && cr.IssuerId == newContributor.IssuerId).FirstOrDefault();

            if (contributor != null)
                return new OperationResult<Contributor>(false, HttpStatusCode.Conflict)
                {
                    DevMessage = "El contribuyente que desea registrar ya existe.",
                    UserMessage = "El contibuyente que desea registrar ya existe."
                };

            newContributor.CreatedOn = DateTime.Now;

            _contributorRepository.Add(newContributor);

            
            try
            {
                _contributorRepository.Save();

                return new OperationResult<Contributor>(true, HttpStatusCode.OK)
                {
                    Entity = newContributor
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

                return new OperationResult<Contributor>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el registro" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<Contributor>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el contributoro" };
            }
            catch (Exception e)
            {
                return new OperationResult<Contributor>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar el registro" };
            }
        }

        public OperationResult<Contributor> Update(Contributor contributorToUpdate)
        {
            try
            {
                // revisa si existe otro contribuyente con la misma informacion
                var existsAnotherContrib = _contributorRepository.FindBy(cr => cr.Id != contributorToUpdate.Id
                        && cr.Identification.Equals(contributorToUpdate.Identification)
                        && cr.IssuerId == contributorToUpdate.IssuerId).Any();

                if (existsAnotherContrib)
                {
                    return new OperationResult<Contributor>(false, HttpStatusCode.Conflict)
                    {
                        DevMessage = "Ya existe otro contibuyente con la misma informacion.",
                        UserMessage = "Ya existe otro contibuyente con la misma informacion."
                    };
                }

                _contributorRepository.Edit(contributorToUpdate);
                _contributorRepository.Save();

                return new OperationResult<Contributor>(true, HttpStatusCode.OK)
                {
                    Entity = contributorToUpdate
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

                return new OperationResult<Contributor>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el registro" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<Contributor>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el contributoro" };
            }
            catch (Exception e)
            {
                return new OperationResult<Contributor>(false, HttpStatusCode.InternalServerError) { DevMessage = e.Message, UserMessage = "Ocurrio un error guardar el contributoro" };
            }
        }

        public IQueryable<Contributor> SearchContributors(string searchTerm, long issuerId)
        {
            return _contributorRepository.Search(searchTerm, issuerId);
        }

        public OperationResult<Contributor> AddImportContributor(Contributor newContributor)
        {

            try
            {
                newContributor.CreatedOn = DateTime.Now;
                _contributorRepository.Add(newContributor);
                _contributorRepository.Save();
                return new OperationResult<Contributor>(true, HttpStatusCode.OK)
                {
                    Entity = newContributor
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

                return new OperationResult<Contributor>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el Contribuyente" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<Contributor>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el Contribuyente" };
            }
            catch (Exception e)
            {
                return new OperationResult<Contributor>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar el Contribuyente" };
            }
        }
        public OperationResult<List<Contributor>> ImportContributor(List<Contributor> newContributor)
        {
            try
            {                
                if (newContributor.Count() > 0)
                {
                    var addContributor = new List<Contributor>();
                    var editContributor = new List<Contributor>();
                    foreach (var p in newContributor)
                    {
                        var contributor = GetContributorByRUC(p.Identification, p.IssuerId);
                        if (contributor == null)
                        {
                            addContributor.Add(p);
                            //AddImportContributor(p);
                        }
                        else {
                            contributor.BussinesName = p.BussinesName;
                            contributor.Address = p.Address;
                            contributor.EmailAddresses = p.EmailAddresses;
                            contributor.Phone = p.Phone;
                            contributor.TradeName = p.TradeName;
                            contributor.LastModifiedOn = DateTime.Now;
                            contributor.IsEnabled = true;
                            //Update(contributor);
                            editContributor.Add(contributor);
                        }
                    }
                    // Regestra nuevo contribuyentes
                    if(addContributor.Count > 0)
                    {
                        _contributorRepository.AddRange(addContributor);
                        _contributorRepository.Save();                       
                    }
                    // Actualiza los contribuyentes
                    if (editContributor.Count > 0)
                    {
                        _contributorRepository.EditRange(editContributor);
                        _contributorRepository.Save();
                    }

                    return new OperationResult<List<Contributor>>(true, HttpStatusCode.OK)
                    {
                        Entity = newContributor,
                        DevMessage = $"Se registraron los Contribuyentes exitosamente.",
                        UserMessage = $"Se registraron los Contribuyentes exitosamente."
                    };

                }

                return new OperationResult<List<Contributor>>(false, HttpStatusCode.BadRequest) { UserMessage = "Ocurrio un error guardar el Contribuyente" };

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

                return new OperationResult<List<Contributor>>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el Contribuyente" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<List<Contributor>>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el Contribuyente" };
            }
            catch (Exception e)
            {
                return new OperationResult<List<Contributor>>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar el Contribuyente" };
            }
        }
    }
}
