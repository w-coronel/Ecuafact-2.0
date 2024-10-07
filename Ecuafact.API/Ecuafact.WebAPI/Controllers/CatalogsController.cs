using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Ecuafact.WebAPI.Domain.Entities.App;
using Ecuafact.WebAPI.Domain.Services;
using Ecuafact.WebAPI.Filters;
using Ecuafact.WebAPI.Models;

namespace Ecuafact.WebAPI.Controllers
{
    /// <summary>
    /// CATALOGOS Y TARIFARIOS
    /// </summary>
    /// <remarks>Catalogo de las tablas de informacion  para la generacion de documentos electronicos.</remarks>
    [DisplayName("Catálogo General")]
    public class CatalogsController : ApiController
    {
        private readonly ICatalogsService _catalogsService;
        private readonly IAppService _appService;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="catalogsService"></param>
        public CatalogsController(ICatalogsService catalogsService, IAppService appService)
        {
            _catalogsService = catalogsService;
            _appService = appService;
        }

        // GET: http://api.Ecuafact.WebAPI.com/v1/catalogs/idtypes
        /// <summary>
        /// Tipos de Identificación
        /// </summary>
        /// <remarks>
        /// Devuelve la tabla de tipos de identificación utilizados en los documentos electrónicos
        /// </remarks>
        /// <returns></returns>
        [HttpGet, Route("catalogs/id-types")]
        public IEnumerable<IdentificationTypesDto> GetIdentificationTypes()
        {
            try
            {
                var idTypes = _catalogsService.GetIdentificationTypes().Where(x => x.IsEnabled).ToList()
                    .Select(idt => idt.ToIdentificationTypesDto());

                return idTypes;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.BuildHttpErrorResponse(HttpStatusCode.NotFound, ex.Message, ex.InnerException?.Message));
            }
        }

        /// <summary>
        /// Métodos de Pago
        /// </summary>
        /// <remarks>
        ///     Devuelve una lista de métodos de pago utilizados en los documentos electrónicos
        /// </remarks>
        /// <returns></returns>
        [HttpGet, Route("catalogs/payment-methods")]
        public IEnumerable<PaymentMethodDto> GetPaymentMethods()
        {
            var paymenMethods = _catalogsService.GetPaymentMethods()
                .Where(o => o.IsEnabled).ToList()
                .Select(pym => pym.ToPaymentMethodDto())
                ;

            return paymenMethods;
        }


        /// <summary>
        /// Tipos de Documentos
        /// </summary>
        /// <remarks>
        ///     Devuelve los tipos de documentos electrónicos
        /// </remarks>
        /// <returns></returns>
        [HttpGet, Route("catalogs/document-types")]
        public IEnumerable<DocumentTypeDto> GetDocumentTypes()
        {
            var docTypes = _catalogsService.GetDocumentTypes()
                .Where(o => o.IsEnabled).ToList()
                .Select(doc => doc.ToDocumentTypeDto())
                ;

            return docTypes;
        }


        /// <summary>
        /// Tipos de Impuestos
        /// </summary>
        /// <remarks>
        ///     Devuelve los tipos de impuestos utilizados en los documentos electrónicos
        /// </remarks>
        /// <returns></returns>
        [HttpGet, Route("catalogs/tax-types")]
        public IEnumerable<TaxTypeDto> GetTaxTypes()
        {
            var taxRates = _catalogsService.GetTaxTypes()
                .Where(o => o.IsEnabled).ToList()
                .Select(iv => iv.ToTaxTypeDto())
                ;

            return taxRates;
        }

        /// <summary>
        /// Tipos de IVA
        /// </summary>
        /// <remarks>
        ///     Devuelve los tipos de IVA disponibles
        /// </remarks>
        /// <returns></returns>
        [HttpGet, Route("catalogs/vat-rates")]
        public IEnumerable<VatRatesDto> GetVatRates()
        {
            var vatRates = _catalogsService.GetVatRates()
                .Where(o => o.IsEnabled).ToList()
                .Select(iv => iv.ToVatRatesDto())
                ;
            return vatRates;
        }

        /// <summary>
        /// Tipos de ICE
        /// </summary>
        /// <remarks>
        ///     Devuelve los tipos de ICE disponibles
        /// </remarks>
        /// <returns></returns>
        [HttpGet, Route("catalogs/ice-rates")]
        public IEnumerable<IceRateDto> GetIceRates()
        {
            var iceRates = _catalogsService.GetIceRates()
                .Where(o => o.IsEnabled).ToList()
                .Select(ic => ic.ToIceRateDto())
                ;
            return iceRates;
        }

        /// <summary>
        /// Tipos de Productos
        /// </summary>
        /// <remarks>
        ///     Devuelve los tipos de productos: Bienes o Servicios
        /// </remarks>
        /// <returns></returns>
        [HttpGet, Route("catalogs/product-types")]
        public IEnumerable<ProductTypeDto> GetProductTypes()
        {
            var prodTypes = _catalogsService.GetProductTypes()
                .Where(o => o.IsEnabled).ToList()
                .Select(pt => pt.ToProductTypeDto())
                ;
            return prodTypes;
        }

        /// <summary>
        /// Tipos de Contribuyentes
        /// </summary>
        /// <remarks>
        ///     Devuelve una lista de los tipos de contribuyentes
        /// </remarks>
        /// <returns></returns>
        [HttpGet, Route("catalogs/contributor-types")]
        public IEnumerable<ContributorTypeDto> GetContributorTypes()
        {
            var contribTypes = _catalogsService.GetContributorTypes()
                .Where(o => o.IsEnabled).ToList()
                .Select(ct => ct.ToContributorTypeDto())
                ;
            return contribTypes;
        }


        /// <summary>
        /// Precios de los servicios y productos ecuanexust
        /// </summary>
        /// <remarks>
        ///     Devuelve una lista de los precios de productos y servicios
        /// </remarks>
        /// <returns></returns>
        [HttpGet, Route("catalogs/productService-ecuafact")]
        public IEnumerable<ProductServicesEcuafact> GetProductServicesEcuafact()
        {
            var prodService = new List<ProductServicesEcuafact>()
            {
                new ProductServicesEcuafact(){
                Code ="01",
                Name ="Firma electrónica",
                Price = Constants.ElectronicSign.Price},
                new ProductServicesEcuafact(){  
                Code ="02",
                Name ="Suscripción anual",
                Price = Constants.Subscription.Price},
            };
            return prodService;
        }


        /// <summary>
        /// Tipos de planes
        /// </summary>
        /// <remarks>
        /// Devuelve una lista de planes 
        /// </remarks>
        /// <returns></returns>
        [HttpGet, Route("catalogs/type-Licence")]
        public IEnumerable<LicenceTypeDto> GetTypePlans()
        {
            var plans = _catalogsService.GetLicenceTypes()
                .Where(o => o.IsEnabled && o.TradeAgreement == false).ToList()
                .Select(p => p.ToLicenceTypeDto()) ;

            return plans;
        }

        /// <summary>
        /// Tipos de sutentos 
        /// </summary>
        /// <remarks>
        ///     Devuelve una lista de tipos de sustento para los comprobantes
        /// </remarks>
        /// <returns></returns>
        [HttpGet, Route("catalogs/support-types")]
        public IEnumerable<SupportTypeDto> GetSupportTypes()
        {
            var supportType = _catalogsService.GetSupportType()
                .Where(o => o.IsEnabled).ToList()
                .Select(st => st.ToSupportTypeDto()); 

            return supportType;
        }

        /// <summary>
        /// Mensajes de notificación 
        /// </summary>
        /// <remarks>
        ///     Devuelve Mensaje de notificación
        /// </remarks>
        /// <returns></returns>
        [HttpGet, Route("catalogs/message-notification")]
        public NotificationDto GetNotification()
        {
            var notification = _catalogsService.GetNotification()
                .Where(not => not.IsEnabled).ToList()
                .Select(dat => dat.ToNotification()).FirstOrDefault(); 

            return notification;
        }

       
        /// <summary>
        /// Tipos sustento del comprobante
        /// </summary>
        /// <remarks>
        /// Devuelve la lista de tipos sustento del comprobante
        /// </remarks>
        /// <returns></returns>
        [HttpGet, Route("catalogs/sustenance-types")]
        public List<TipoSustento> GetSustenanceTypes()
        {
            try
            {
                var _types = _appService.GetSustenanceType();
                if (_types.IsSuccess)
                {
                    return _types.Entity;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.BuildHttpErrorResponse(HttpStatusCode.NotFound, ex.Message, ex.InnerException?.Message));
            }
        }

        /// <summary>
        /// Tipos de subjeto retenido
        /// </summary>
        /// <remarks>
        /// Devuelve una Tipos de subjeto retenidos
        /// </remarks>
        /// <returns></returns>
        [HttpGet, Route("catalogs/type-identificationSupplier")]
        public IEnumerable<IdentificationSupplierTypeDto> GetIdentificationSupplierType()
        {
            
            var _identificationSupplier = _catalogsService.GetIdentificationSupplierTypes()
                                                          .Where(o => o.IsEnabled).ToList()
                                                          .Select(p => p.ToIdentificationSupplierTypeDto());

            return _identificationSupplier;
        }
    }
}
