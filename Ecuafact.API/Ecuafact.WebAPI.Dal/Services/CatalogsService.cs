using System.Collections.Generic;
using System.Linq;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Repository;
using Ecuafact.WebAPI.Domain.Services;

namespace Ecuafact.WebAPI.Dal.Services
{
    public class CatalogsService : ICatalogsService
    {
        private readonly IEntityRepository<DocumentType> _documentTypesRepository;
        private readonly IEntityRepository<VatRate> _vatRatesRepository;
        private readonly IEntityRepository<IdentificationType> _identificationTypesRepository;
        private readonly IEntityRepository<PaymentMethod> _paymentMethodsRepository;
        private readonly IEntityRepository<ProductType> _productTypesRepository;
        private readonly IEntityRepository<ContributorType> _contributorTypesRepository;
        private readonly IEntityRepository<IceRate> _iceRatesRepository;
        private readonly IEntityRepository<TaxType> _taxTypesRepository;
        private readonly IEntityRepository<LicenceType> _licenceTypeRepository;
        private readonly IEntityRepository<SupportType> _supportTypeRepository;
        private readonly IEntityRepository<Notification> _notificationRepository;
        private readonly IEntityRepository<IdentificationSupplierType> _identificationSupplierTypeRepository;

        public CatalogsService(IEntityRepository<VatRate> vatRatesRepository, IEntityRepository<IceRate> iceRatesRepository,
            IEntityRepository<IdentificationType> identificationTypesRepository,
            IEntityRepository<PaymentMethod> paymentMethodsRepository,
            IEntityRepository<ProductType> productTypesRepository,
            IEntityRepository<ContributorType> contributorTypesRepository,
            IEntityRepository<TaxType> taxTypesRepository,
            IEntityRepository<DocumentType> documentTypesRepository,
            IEntityRepository<LicenceType> licenceTypeRepository,
            IEntityRepository<SupportType> supportTypeRepository,
            IEntityRepository<Notification> notificationRepository,
            IEntityRepository<IdentificationSupplierType> identificationSupplierTypeRepository)
        {
            _vatRatesRepository = vatRatesRepository;
            _iceRatesRepository = iceRatesRepository;
            _documentTypesRepository = documentTypesRepository;
            _identificationTypesRepository = identificationTypesRepository;
            _paymentMethodsRepository = paymentMethodsRepository;
            _productTypesRepository = productTypesRepository;
            _contributorTypesRepository = contributorTypesRepository;
            _taxTypesRepository = taxTypesRepository;
            _licenceTypeRepository = licenceTypeRepository;
            _supportTypeRepository = supportTypeRepository;
            _notificationRepository = notificationRepository;
            _identificationSupplierTypeRepository = identificationSupplierTypeRepository;
        }
        
        public IQueryable<VatRate> GetVatRates()
        {
            return _vatRatesRepository.GetAll();
        }

        public IQueryable<TaxType> GetTaxTypes()
        {
            return _taxTypesRepository.GetAll();
        } 

        public IQueryable<IceRate> GetIceRates()
        {
            return _iceRatesRepository.GetAll();
        }

        public IQueryable<DocumentType> GetDocumentTypes()
        {
            return _documentTypesRepository.GetAll();
        }

        public IQueryable<IdentificationType> GetIdentificationTypes()
        {
            return _identificationTypesRepository.GetAll();
        }

        public IQueryable<PaymentMethod> GetPaymentMethods()
        {
            return _paymentMethodsRepository.GetAll();
        }

        public IQueryable<ProductType> GetProductTypes()
        {
            return _productTypesRepository.GetAll();
        }

        public IQueryable<ContributorType> GetContributorTypes()
        {
            return _contributorTypesRepository.GetAll();
        }

        public IQueryable<LicenceType> GetLicenceTypes()
        {
            return _licenceTypeRepository.GetAll();
        }

        public IQueryable<SupportType> GetSupportType()
        {
            return _supportTypeRepository.GetAll();
        }

        public IQueryable<Notification> GetNotification()
        {
            return _notificationRepository.GetAll();
        }

        public IQueryable<IdentificationSupplierType> GetIdentificationSupplierTypes()
        {
            return _identificationSupplierTypeRepository.GetAll();
        }
    }
}
