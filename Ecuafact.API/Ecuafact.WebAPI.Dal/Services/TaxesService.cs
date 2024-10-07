using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using Ecuafact.WebAPI.Dal.Core;
using Ecuafact.WebAPI.Dal.Repository.Extensions;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Repository;
using Ecuafact.WebAPI.Domain.Services;

namespace Ecuafact.WebAPI.Dal.Services
{
    public class TaxesService : ITaxesService
    {
        private readonly IEntityRepository<RetentionTax> _taxRepository;
        private readonly IEntityRepository<RetentionRate> _rateRepository;

        public TaxesService(IEntityRepository<RetentionTax> taxRepository, IEntityRepository<RetentionRate> ratesRepository)
        {
            _taxRepository = taxRepository;
            _rateRepository = ratesRepository;
        }

        public RetentionTax GetTaxByCode(string taxCode)
        {
            var tax = _taxRepository.FindBy(t => t.SriCode == taxCode).FirstOrDefault();
            if (tax != null)
            {
                tax.RetentionRate = _rateRepository.FindBy(r => r.RetentionTaxId == tax.Id).ToList();
            }

            return tax;
        }
        public RetentionTax GetTax(long taxId)
        {
            var tax = _taxRepository.FindBy(t => t.Id == taxId).FirstOrDefault();
            if (tax != null)
            {
                tax.RetentionRate = _rateRepository.FindBy(r => r.RetentionTaxId == tax.Id).ToList();
            }
         
            return tax;
        }

        public IQueryable<RetentionTax> GetAllTaxes()
        {
            return _taxRepository.All
                .Include(tax => tax.RetentionRate); 
        }

        public OperationResult<RetentionTax> AddTax(RetentionTax newTax)
        {
            var tax = _taxRepository.FindBy(pr => pr.SriCode.Equals(newTax.SriCode)).FirstOrDefault();

            if (tax != null)
                return new OperationResult<RetentionTax>(false, HttpStatusCode.Conflict) { DevMessage = "El código de impuesto ya existe", UserMessage = "El código del impuesto que desea crear, ya existe." };

            try
            {
                newTax.CreatedOn = DateTime.Now;
                _taxRepository.Add(newTax); 
                _taxRepository.Save();
                return new OperationResult<RetentionTax>(true, HttpStatusCode.OK)
                {
                    Entity = newTax
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

                return new OperationResult<RetentionTax>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el impuesto" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<RetentionTax>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el impuesto"};
            }
            catch (Exception e)
            {
                return new OperationResult<RetentionTax>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar el impuesto" };
            }
        }

        public OperationResult<RetentionTax> UpdateTax(RetentionTax taxToUpdate)
        {
            try
            {
                //_taxRepository.Edit(taxToUpdate);
                _taxRepository.Save();
                return new OperationResult<RetentionTax>(true, HttpStatusCode.OK)
                {
                    Entity = taxToUpdate
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

                return new OperationResult<RetentionTax>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el impuesto" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<RetentionTax>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el impuesto" };
            }
            catch (Exception e)
            {
                return new OperationResult<RetentionTax>(false, HttpStatusCode.InternalServerError) { DevMessage = e.Message, UserMessage = "Ocurrio un error guardar el impuesto" };
            }
        }

        public IQueryable<RetentionTax> SearchTaxes(string searchTerm, long? type = null)
        {
            IQueryable<RetentionTax> taxes;

            // Realizamos la busqueda de los impuestos
            if (type.HasValue)
            {
                taxes = _taxRepository.FindBy(t => t.TaxTypeId == type.Value && t.SriCode.Contains(searchTerm) || t.Name.Contains(searchTerm)).Include(m => m.RetentionRate);
            }
            else
            {
                taxes = _taxRepository.FindBy(t => t.SriCode.Contains(searchTerm) || t.Name.Contains(searchTerm)).Include(m => m.RetentionRate);
            }
             

            return taxes;

        }
    }
}
