using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Repository;
using Ecuafact.WebAPI.Domain.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Dal.Services
{
    public class AgreementService : IAgreementService
    {

        private readonly IEntityRepository<Agreement> _agreementRepository;
        private readonly IEntityRepository<Beneficiary> _beneficiaryRepository;
        private readonly IEntityRepository<BeneficiaryReferenceCode> _beneficiaryRefCodeRepository;
        private readonly IEntityRepository<ReferenceCodes> _referenceCodesRepository;

        public AgreementService(IEntityRepository<Agreement> agreementRepository,
            IEntityRepository<Beneficiary> beneficiaryRepository,
            IEntityRepository<BeneficiaryReferenceCode> beneficiaryRefCodeRepository,
            IEntityRepository<ReferenceCodes> referenceCodesRepository)
        {
            _agreementRepository = agreementRepository;
            _beneficiaryRepository = beneficiaryRepository;
            _beneficiaryRefCodeRepository = beneficiaryRefCodeRepository;
            _referenceCodesRepository = referenceCodesRepository;
        }

        #region Convenios
        public OperationResult<List<Agreement>> GetAgreement()
        {
            try
            {
                var agreement = _agreementRepository.All;

                if (agreement?.Count() > 0)
                {
                    return new OperationResult<List<Agreement>>(false, HttpStatusCode.NotFound, "No hay convenios activos");
                }

                return new OperationResult<List<Agreement>>(true, HttpStatusCode.OK) { Entity = agreement.ToList() };
            }
            catch (Exception e)
            {
                return new OperationResult<List<Agreement>>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "No hay convenios activos!" };
            }

        }

        public OperationResult<Agreement> GetAgreementById(long Id)
        {
            try
            {
                var agreement = _agreementRepository.FindBy(x => x.Id == Id).FirstOrDefault();

                if (agreement == null)
                {
                    return new OperationResult<Agreement>(false, HttpStatusCode.NotFound, "No se encontro el convenio");
                }

                return new OperationResult<Agreement>(true, HttpStatusCode.OK) { Entity = agreement };
            }
            catch (Exception e)
            {
                return new OperationResult<Agreement>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "No se encontro el convenio!" };
            }

        }

        public OperationResult<Agreement> GetAgreementByCode(string code)
        {
            try
            {
                var agreement = _agreementRepository.FindBy(x => x.Code == code).FirstOrDefault();

                if (agreement == null)
                {
                    return new OperationResult<Agreement>(false, HttpStatusCode.NotFound, "No se encontro el convenio");
                }

                return new OperationResult<Agreement>(true, HttpStatusCode.OK) { Entity = agreement };
            }
            catch (Exception e)
            {
                return new OperationResult<Agreement>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "No se encontro el convenio!" };
            }

        }

        public OperationResult<ReferenceCodes> GetReferenceCodeByAgreementCode(string code)
        {
            try
            {
                var referenceCode = _referenceCodesRepository.FindBy(x => x.Code == code).Include(s => s.Agreement).FirstOrDefault();
                if (referenceCode == null)
                {
                    return new OperationResult<ReferenceCodes>(false, HttpStatusCode.NotFound, "No se encontro el codigo de referncia del convenio");
                }

                if (referenceCode == null)
                {
                    return new OperationResult<ReferenceCodes>(false, HttpStatusCode.NotFound, "No se encontro el codigo de referncia del convenio");
                }

                return new OperationResult<ReferenceCodes>(true, HttpStatusCode.OK) { Entity = referenceCode };
            }
            catch (Exception e)
            {
                return new OperationResult<ReferenceCodes>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "No se encontro el codigo de referncia del convenio!" };
            }

        }

        #endregion

        #region Benificiarios

        public OperationResult<Beneficiary> AddBeneficiary(Beneficiary model)
        {
            try
            {
                var beneficiary = _beneficiaryRepository.FindBy(pr => pr.Identification.Equals(model.Identification)).FirstOrDefault();
                if (beneficiary != null)
                {
                    model.Id = beneficiary.Id;
                    model.BeneficiaryReferenceCode.BeneficiaryId = beneficiary.Id;
                    AddBeneficiaryReferenceCode(model.BeneficiaryReferenceCode);
                }
                else
                {

                    _beneficiaryRepository.Add(model);
                    _beneficiaryRepository.Save();
                    if (model.Id > 0)
                    {
                        model.BeneficiaryReferenceCode.BeneficiaryId = model.Id;
                        AddBeneficiaryReferenceCode(model.BeneficiaryReferenceCode);
                    }
                }

                return new OperationResult<Beneficiary>(true, HttpStatusCode.OK)
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

                return new OperationResult<Beneficiary>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el benificiario al conevenio" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<Beneficiary>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el benificiario al conevenio" };
            }
            catch (Exception e)
            {
                return new OperationResult<Beneficiary>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar el benificiario al conevenio" };
            }
        }
        public OperationResult<Beneficiary> UpdateBeneficiary(Beneficiary model)
        {
            try
            {
                _beneficiaryRepository.Save();
                return new OperationResult<Beneficiary>(true, HttpStatusCode.OK)
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

                return new OperationResult<Beneficiary>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el benificiario al conevenio" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<Beneficiary>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el benificiario al conevenio" };
            }
            catch (Exception e)
            {
                return new OperationResult<Beneficiary>(false, HttpStatusCode.InternalServerError) { DevMessage = e.Message, UserMessage = "Ocurrio un error guardar el benificiario al conevenio" };
            }
        }
        public OperationResult<Beneficiary> GetBeneficiary(long id)
        {
            try
            {
                var beneficiary = _beneficiaryRepository.FindBy(x => x.Id == id).FirstOrDefault();

                if (beneficiary == null)
                {
                    return new OperationResult<Beneficiary>(false, HttpStatusCode.NotFound, "No se encontraron benificiarios");
                }

                return new OperationResult<Beneficiary>(true, HttpStatusCode.OK) { Entity = beneficiary };
            }
            catch (Exception e)
            {
                return new OperationResult<Beneficiary>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "No se encontraron benificiarios!" };
            }

        }
        public OperationResult<List<Beneficiary>> GetBeneficiaryByAgreementId(long id)
        {
            try
            {
                var beneficiary = _beneficiaryRepository.FindBy(x => x.AgreementId == id);

                if (beneficiary?.Count() > 0)
                {
                    return new OperationResult<List<Beneficiary>>(false, HttpStatusCode.NotFound, "No se encontraron benificiarios");
                }

                return new OperationResult<List<Beneficiary>>(true, HttpStatusCode.OK) { Entity = beneficiary.ToList() };
            }
            catch (Exception e)
            {
                return new OperationResult<List<Beneficiary>>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "No se encontraron benificiarios!" };
            }

        }
        public OperationResult<BeneficiaryReferenceCode> AddBeneficiaryReferenceCode(BeneficiaryReferenceCode model)
        {
            var benefRefCode = _beneficiaryRefCodeRepository.FindBy(m => (m.Identification == model.Identification || m.Identification == model.Identification.Substring(0, 10)) && m.Status != ReferenceCodeStatusEnum.Applied)
                   .FirstOrDefault();
            if (benefRefCode != null)
            {
                return new OperationResult<BeneficiaryReferenceCode>(false, HttpStatusCode.BadRequest) { DevMessage = $"El ruc {model.Identification} tiene activo codigo de descuento para aplicar", UserMessage = $"El ruc {model.Identification} tiene activo codigo de descuento para aplicar" };
            }

            try
            {
                model.BeneficiaryId = model.BeneficiaryId;
                _beneficiaryRefCodeRepository.Add(model);
                _beneficiaryRepository.Save();

                return new OperationResult<BeneficiaryReferenceCode>(true, HttpStatusCode.OK)
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

                return new OperationResult<BeneficiaryReferenceCode>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error generar el codigo de descuento del benificiario al conevenio" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<BeneficiaryReferenceCode>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error generar el codigo de descuento del benificiario al conevenio" };
            }
            catch (Exception e)
            {
                return new OperationResult<BeneficiaryReferenceCode>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error generar el codigo de descuento del benificiario al conevenio" };
            }
        }

        #endregion
    }
}

