using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities
{
    public class PaymentResultModel
    {
        [JsonProperty("acquirerId")] public string AcquirerId { get; set; }
        [JsonProperty("idCommerce")] public string IdCommerce { get; set; }
        [JsonProperty("purchaseOperationNumber")] public string OperationNumber { get; set; }
        [JsonProperty("purchaseAmount")] public string PurchaseAmount { get; set; }
        [JsonProperty("purchaseCurrencyCode")] public string PurchaseCurrencyCode { get; set; }
        [JsonProperty("commerceMallId")] public string CommerceMallId { get; set; }
        [JsonProperty("shippingFirstName")] public string ShippingFirstName { get; set; }
        [JsonProperty("shippingLastName")] public string ShippingLastName { get; set; }
        [JsonProperty("shippingEmail")] public string ShippingEmail { get; set; }
        [JsonProperty("shippingAddress")] public string ShippingAddress { get; set; }
        [JsonProperty("shippingZIP")] public string ShippingZIP { get; set; }
        [JsonProperty("shippingCity")] public string ShippingCity { get; set; }
        [JsonProperty("shippingState")] public string ShippingState { get; set; }
        [JsonProperty("shippingCountry")] public string ShippingCountry { get; set; }
        [JsonProperty("userCommerce")] public string UserCommerce { get; set; }
        [JsonProperty("userCodePayme")] public string UserCodePayme { get; set; }
        [JsonProperty("descriptionProducts")] public string DescriptionProducts { get; set; }
        [JsonProperty("authorizationResult")] public string AuthorizationResult { get; set; }
        [JsonProperty("authorizationCode")] public string AuthorizationCode { get; set; }
        [JsonProperty("errorCode")] public string ErrorCode { get; set; }
        [JsonProperty("errorMessage")] public string Message { get; set; }
        [JsonProperty("bin")] public string Bin { get; set; }
        [JsonProperty("reserved1")] public string Reserved1 { get; set; }
        [JsonProperty("reserved2")] public string Reserved2 { get; set; }
        [JsonProperty("reserved3")] public string Reserved3 { get; set; }
        [JsonProperty("reserved11")] public string Reserved11 { get; set; }
        [JsonProperty("reserved22")] public string Reserved22 { get; set; }
        [JsonProperty("reserved23")] public string Reserved23 { get; set; }
        [JsonProperty("answerCode")] public string AnswerCode { get; set; }
        [JsonProperty("answerMessage")] public string AnswerMessage { get; set; }
        [JsonProperty("txDateTime")] public string TxDateTime { get; set; }


        // Informacion Adicional
        public DateTime Date { get; set; }
        public string Result { get; set; }
        [JsonProperty("subscriptionActiva")] public bool SubscriptionActiva { get; set; }
        [JsonProperty("tipoProceso")] public long TipoProceso { get; set; }
    }

}
