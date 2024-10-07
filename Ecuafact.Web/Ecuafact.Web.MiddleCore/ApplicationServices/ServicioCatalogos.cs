using System;
using System.Collections.Generic;
using Ecuafact.Web.Domain.Entities;
using System.Net.Http;
using System.Security.Policy;
using System.Net.Http.Headers;
using System.Web.Caching;
using Ecuafact.Web.Domain.Entities.API;

namespace Ecuafact.Web.MiddleCore.ApplicationServices
{
    public class ServicioCatalogos
    { 
        public static List<IvaRatesDto> ObtenerTiposIVA(string token)
        {
            var contribuyentes = new List<IvaRatesDto>();
            
            var httpClient = ClientHelper.GetClient(token);

            var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Catalogs/vat-rates").Result;

            if (response.IsSuccessStatusCode)
            {
                contribuyentes = response.GetContent<List<IvaRatesDto>>();
            }

            return contribuyentes;
        }


        public static List<DocumentTypesDto> ObtenerTiposDocumento(string token)
        {
            var tipos = new List<DocumentTypesDto>();
            
            var httpClient = ClientHelper.GetClient(token);

            var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Catalogs/document-types").Result;

            if (response.IsSuccessStatusCode)
            {
                tipos = response.GetContent<List<DocumentTypesDto>>();
            }

            return tipos;
        }


        public static List<IdentificationTypesDto> ObtenerTiposIdentificacion(string token)
        {
            var tipos = new List<IdentificationTypesDto>();
            var httpClient = ClientHelper.GetClient(token);

            var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Catalogs/id-types").Result;

            if (response != null)
            {
                tipos = response.GetContent<List<IdentificationTypesDto>>();
            }

            return tipos;
        }

        public static List<PaymentMethodDto> ObtenerMetodosPago(string token)
        {
            var tipos = new List<PaymentMethodDto>();
            var httpClient = ClientHelper.GetClient(token);

            var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Catalogs/payment-methods").Result;

            if (response.IsSuccessStatusCode)
            {
                tipos = response.GetContent<List<PaymentMethodDto>>();
            }

            return tipos;
        }

        public static List<IceRate> ObtenerTiposICE(string token)
        {
            var tipos = new List<IceRate>();
            
            var httpClient = ClientHelper.GetClient(token);

            var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Catalogs/ice-rates").Result;
            
            if (response.IsSuccessStatusCode)
            {
                tipos = response.GetContent<List<IceRate>>();
            }

            return tipos;
        }

        public static List<TaxType> ObtenerTiposImpuesto(string token)
        {
            var tipos = new List<TaxType>();
            var httpClient = ClientHelper.GetClient(token);

            var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Catalogs/tax-types").Result;

            if (response != null)
            {
                tipos = response.GetContent<List<TaxType>>();
            }

            return tipos;
        }

        public static List<ProductTypeDto> ObtenerTiposProducto(string token)
        {
            var tipos = new List<ProductTypeDto>();
            
            var httpClient = ClientHelper.GetClient(token);

            var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Catalogs/product-types").Result;

            if (response.IsSuccessStatusCode)
            {
                tipos = response.GetContent<List<ProductTypeDto>>();
            }

            return tipos;
        }

        public static List<ContributorTypeDto> ObtenerTiposContribuyente(string token)
        {
            var tipos = new List<ContributorTypeDto>(); 
            var httpClient = ClientHelper.GetClient(token);

            var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Catalogs/contributor-types").Result;

            if (response.IsSuccessStatusCode)
            { 
                tipos = response.GetContent<List<ContributorTypeDto>>();
            }

            return tipos;
        }


        public static List<ProductServicesEcuafact> ObtenerProductServicesEcuafact(string token)
        {
            var contribuyentes = new List<ProductServicesEcuafact>();

            var httpClient = ClientHelper.GetClient(token);

            var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Catalogs/productService-ecuafact").Result;

            if (response.IsSuccessStatusCode)
            {
                contribuyentes = response.GetContent<List<ProductServicesEcuafact>>();
            }

            return contribuyentes;
        }

        public static List<LicenceType> ObtenerTiposLicencias(string token)
        {
            var tipos = new List<LicenceType>();
            var httpClient = ClientHelper.GetClient(token);
            var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Catalogs/type-Licence").Result;
            if (response.IsSuccessStatusCode)
            {
                tipos = response.GetContent<List<LicenceType>>();
            }
            return tipos;
        }

        public static List<ECommerce> ObtenerTiposPagos(string token)
        {
            var tipos = new List<ECommerce>();
            var httpClient = ClientHelper.GetClient(token);
            var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Payment/checkout-payment").Result;
            if (response.IsSuccessStatusCode)
            {
                tipos = response.GetContent<List<ECommerce>>();
            }
            return tipos;
        }

        public static List<SupportType> ObtenerTiposSustento(string token)
        {
            var tipos = new List<SupportType>();
            var httpClient = ClientHelper.GetClient(token);

            var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Catalogs/support-types").Result;

            if (response != null)
            {
                tipos = response.GetContent<List<SupportType>>();
            }

            return tipos;
        }

        public static NotificationMessage GetMessageNotification(string token)
        {
            var tipos = new NotificationMessage();
            var httpClient = ClientHelper.GetClient(token);
            var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Catalogs/message-notification").Result;
            if (response.IsSuccessStatusCode)
            {
                tipos = response.GetContent<NotificationMessage>();
            }
            return tipos;
        }

        public static List<TipoSustento> GetSustenanceTypes(string token)
        {
            var tipos = new List<TipoSustento>();
            var httpClient = ClientHelper.GetClient(token);
            var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Catalogs/sustenance-types").Result;
            if (response != null)
            {
                tipos = response.GetContent<List<TipoSustento>>();
            }

            return tipos;
        }

        public static List<IdentificationSupplierTypeDto> ObtenerTipoSujetoSustento(string token)
        {
            var tipos = new List<IdentificationSupplierTypeDto>();
            var httpClient = ClientHelper.GetClient(token);
            var response = httpClient.GetAsync($"{Constants.WebApiUrl}/catalogs/type-identificationSupplier").Result;
            if (response != null)
            {
                tipos = response.GetContent<List<IdentificationSupplierTypeDto>>();
            }

            return tipos;
        }

    }
}
