using Ecuafact.WebAPI.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Models
{
    public class CatalogBaseDto
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public string SriCode { get; set; }
    }

    /// <summary>
    /// Tipo de Identificacion
    /// </summary>
    public class IdentificationTypesDto : CatalogBaseDto { }

    /// <summary>
    /// Forma de Pago
    /// </summary>
    public class PaymentMethodDto : CatalogBaseDto { }

    /// <summary>
    /// Tipo de Producto
    /// </summary>
    public class ProductTypeDto
    {
        /// <summary>
        /// 
        /// </summary>
        public short Id { get; set; }
        /// <summary>
        /// Descripcion del tipo de producto
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Tipo de Contribuyente
    /// </summary>
    public class ContributorTypeDto : CatalogBaseDto { }

    /// <summary>
    /// Tarifas de IVA
    /// </summary>
    public class VatRatesDto : CatalogBaseDto
    {
        /// <summary>
        /// Porcentaje de IVA
        /// </summary>
        public decimal RateValue { get; set; }
    }

    /// <summary>
    /// Tipos de Documento
    /// </summary>
    public class DocumentTypeDto : CatalogBaseDto
    {
    }

    /// <summary>
    /// Tipos de Impuesto
    /// </summary>
    public class TaxTypeDto : CatalogBaseDto
    { 
    }

    /// <summary>
    /// Tipo subjeto sustento
    /// </summary>
    public class IdentificationSupplierTypeDto : CatalogBaseDto { }

    /// <summary>
    /// Tarifas de ICE
    /// </summary>
    public class IceRateDto : CatalogBaseDto
    {
        /// <summary>
        /// Porcentaje aplicado al impuesto
        /// </summary>
        public decimal? Rate { get; set; }
        /// <summary>
        /// Tasa especifica aplicada al impuesto
        /// </summary>
        public decimal? EspecificRate { get; set; }
        /// <summary>
        /// Explicacion de la tasa especifica aplicada al impuesto
        /// </summary>
        public string EspecificRateDescription { get; set; }
    }

    /// <summary>
    /// Tipos de Sustentos
    /// </summary>
    public class SupportTypeDto : CatalogBaseDto
    {
    }


    public class BeneficiaryDto
    {
        public long Id { get; set; }
        public string Identification { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsEnabled { get; set; }
        public BeneficiaryStatusEnum Status { get; set; }
        public string StatusMsg { get; set; }        
        public int AgreementId { get; set; }
    }

    public class SubscriptionDto
    {
        public long Id { get; set; }
        public string RUC { get; set; }         
        public long IssuerId { get; set; }       
        public long LicenceId { get; set; }     
        public SubscriptionStatusEnum Status { get; set; }
        public string StatusMsg { get; set; }
        public Issuer Issuer { get; set; }
        public bool Notify { get; set; } = false; 
        public long Day { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionExpirationDate { get; set; }
        public LicenceType LicenceType { get; set; }
        public string AmountDocument { get; set; }
        public int? IssuedDocument { get; set; }
        public int? BalanceDocument { get; set; }
        public int? AmountIssuePoint { get; set; }
        public int? RequestElectronicSign { get; set; }

    }

    public class ProductServicesEcuafact
    {
        public string Code { get; set; }
        public string Name { get; set; } 
        public decimal Price{ get; set; }
    }

    public class LicenceTypeDto
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }
        public Decimal Price { get; set; }
        public Decimal TaxBase { get; set; }
        public Decimal Discount { get; set; }
        public string AmountDocument { get; set; }
        public bool IncludeCertificate { get; set; }
    }

    public class LicenceTypeModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }       
        public string AmountDocument { get; set; }
        public bool IncludeCertificate { get; set; }
    }


    public class UTMSRequest
    {
        public string source { get; set; }
        public string medium { get; set; }
        public string campaign { get; set; }
    }

    public class UserCampaignsRequest
    {
       [JsonProperty("id")]
        public string Identification { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public long UserId { get; set; }
        public UTMSRequest UTMS { get; set; }
    }

    public class NotificationDto
    {        
        public string Code { get; set; }
        public string Message { get; set; }
        public NotificationType? NotificationType { get; set; }
        public string UrlImage { get; set; }
        public string NameImage { get; set; }       
        public string Title { get; set; }
    }
}
