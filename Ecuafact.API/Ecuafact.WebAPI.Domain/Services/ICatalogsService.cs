using System.Collections.Generic;
using System.Linq;
using Ecuafact.WebAPI.Domain.Entities;

namespace Ecuafact.WebAPI.Domain.Services
{
    public interface ICatalogsService
    {
        IQueryable<VatRate> GetVatRates();
        IQueryable<IceRate> GetIceRates();
        IQueryable<IdentificationType> GetIdentificationTypes();
        IQueryable<PaymentMethod> GetPaymentMethods();
        IQueryable<ProductType> GetProductTypes();
        IQueryable<ContributorType> GetContributorTypes();
        IQueryable<TaxType> GetTaxTypes();
        IQueryable<DocumentType> GetDocumentTypes();
        IQueryable<LicenceType> GetLicenceTypes();
        IQueryable<SupportType> GetSupportType();
        IQueryable<Notification> GetNotification();
        IQueryable<IdentificationSupplierType> GetIdentificationSupplierTypes();
    }
}
