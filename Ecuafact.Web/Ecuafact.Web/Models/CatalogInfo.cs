using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.Domain.Entities.API;
using Ecuafact.Web.MiddleCore.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using static SessionInfo;

namespace Ecuafact.Web.Models
{
    public class CatalogInfo
    {
        private HttpSessionState Session
        {
            get { return HttpContext.Current.Session; }
        }

        public IEnumerable<PaymentMethodDto> PaymentMethod
        {
            get
            {
                var paymentMethod = Session["PAYMENT_METHOD"] as IEnumerable<PaymentMethodDto>;

                if (paymentMethod == null)
                {
                    paymentMethod = ServicioCatalogos.ObtenerMetodosPago(SessionInfo.ApplicationToken);
                    Session["PAYMENT_METHOD"] = paymentMethod;
                }

                return paymentMethod;
            }
        }

        public IEnumerable<ContributorTypeDto> ContributorType
        {
            get
            {
                var contributorType = Session["CONTRIBUTOR_TYPES"] as IEnumerable<ContributorTypeDto>;

                if (contributorType == null)
                {
                    contributorType = ServicioCatalogos.ObtenerTiposContribuyente(SessionInfo.ApplicationToken);
                    Session["CONTRIBUTOR_TYPES"] = contributorType;
                }

                return contributorType;
            }
        }

        public IEnumerable<DocumentTypesDto> DocumentTypes
        {
            get
            {
                var documentType = Session["DOCUMENT_TYPES"] as IEnumerable<DocumentTypesDto>;

                if (documentType == null)
                {
                    documentType = ServicioCatalogos.ObtenerTiposDocumento(SessionInfo.ApplicationToken);
                    Session["DOCUMENT_TYPES"] = documentType;
                }

                return documentType;
            }
        }

        public IEnumerable<IceRate> ICERates
        {
            get
            {
                var iceRates = Session["ICE_RATES"] as IEnumerable<IceRate>;

                if (iceRates == null)
                {
                    iceRates = ServicioCatalogos.ObtenerTiposICE(SessionInfo.ApplicationToken);
                    Session["ICE_RATES"] = iceRates;
                }

                return iceRates;
            }
        }

        public IEnumerable<IvaRatesDto> IVARates
        {
            get
            {
                var ivaRates = Session["IVA_RATES"] as IEnumerable<IvaRatesDto>;

                if (ivaRates == null)
                {
                    ivaRates = ServicioCatalogos.ObtenerTiposIVA(SessionInfo.ApplicationToken);
                    Session["IVA_RATES"] = ivaRates;
                }

                return ivaRates;
            }
        }

        public IEnumerable<IdentificationTypesDto> IdentificationTypes
        {
            get
            {
                var idTypes = Session["IDENTIFICATION_TYPES"] as IEnumerable<IdentificationTypesDto>;

                if (idTypes == null)
                {
                    idTypes = ServicioCatalogos.ObtenerTiposIdentificacion(SessionInfo.ApplicationToken);
                    Session["IDENTIFICATION_TYPES"] = idTypes;
                }

                return idTypes;
            }
        }

        public IEnumerable<TaxType> TaxTypes
        {
            get
            {
                var taxTypes = Session["TAX_TYPES"] as IEnumerable<TaxType>;

                if (taxTypes == null)
                {
                    taxTypes = ServicioCatalogos.ObtenerTiposImpuesto(SessionInfo.ApplicationToken);
                    Session["TAX_TYPES"] = taxTypes;
                }

                return taxTypes;
            }
        }

        public IEnumerable<ProductTypeDto> ProductTypes
        {
            get
            {
                var productTypes = Session["PRODUCT_TYPES"] as IEnumerable<ProductTypeDto>;

                if (productTypes == null)
                {
                    productTypes = ServicioCatalogos.ObtenerTiposProducto(SessionInfo.ApplicationToken);
                    Session["PRODUCT_TYPES"] = productTypes;
                }

                return productTypes;
            }
        }

        public IEnumerable<RetentionTax> Taxes
        {
            get
            {
                var retentionTaxes = Session["RETENTION_TAXES"] as IEnumerable<RetentionTax>;

                if (retentionTaxes == null)
                {
                    retentionTaxes = ServicioImpuestos.ObtenerImpuestos(SessionInfo.ApplicationToken);
                    Session["RETENTION_TAXES"] = retentionTaxes;
                }

                return retentionTaxes;
            }
        }

        public IEnumerable<ProductServicesEcuafact> ProductServicesEcuafact
        {
            get
            {
                var prodServ = Session["Product_Services_Ecuafact"] as IEnumerable<ProductServicesEcuafact>;
                if (prodServ == null)
                {
                    prodServ = ServicioCatalogos.ObtenerProductServicesEcuafact(SessionInfo.ApplicationToken);
                    Session["Product_Services_Ecuafact"] = prodServ;
                }

                return prodServ;
            }
        }

        public IEnumerable<LicenceType> LicenceType
        {
            get
            {
                var typesLicences = Session["Types_Licences"] as IEnumerable<LicenceType>;
                if (typesLicences == null)
                {
                    typesLicences = ServicioCatalogos.ObtenerTiposLicencias(SessionInfo.ApplicationToken);
                    Session["Types_Licences"] = typesLicences;
                }

                return typesLicences;
            }           
        }

        public IEnumerable<ECommerce> eCommerceType
        {
            get
            {
                var typesLicences = Session["eCommerce-Type"] as IEnumerable<ECommerce>;
                if (typesLicences == null || typesLicences?.Count() == 0)
                {
                    typesLicences = ServicioCatalogos.ObtenerTiposPagos(SessionInfo.ApplicationToken);
                    Session["eCommerce-Type"] = typesLicences;
                }

                return typesLicences;
            }

        }

        public NotificationMessage MessageNotification
        {
            get
            {
                var messageNotification = Session["MessageNotification"] as NotificationMessage;                
                if (messageNotification == null)
                {
                    messageNotification = ServicioCatalogos.GetMessageNotification(SessionInfo.ApplicationToken);
                    Session["MessageNotification"] = messageNotification;                   
                }                
                return messageNotification;
            }

        }

        public List<TipoSustento> SustenanceType
        {
            get
            {
                if (!(Session["Sustenance-Type"] is List<TipoSustento> types) || types?.Count == 0)
                {
                    types = ServicioCatalogos.GetSustenanceTypes(SessionInfo.ApplicationToken);
                    Session["Sustenance-Type"] = types;
                }
                return types;
            }
        }
    }
}