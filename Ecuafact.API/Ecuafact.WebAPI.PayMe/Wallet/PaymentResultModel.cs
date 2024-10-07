using Ecuafact.WebAPI.Entities.VPOS2;
using Newtonsoft.Json;
using System;
using Ecuafact.WebAPI.Domain.Entities;

namespace Ecuafact.WebAPI.PayMe
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
        [JsonProperty("authorizationResult")] public OperationResultEnum? AuthorizationResult { get; set; }
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
        public DateTime? Date { get; set; }
        public string Result { get; set; }
    }

    public class PaymentVoucherRequest
    {        
        public long PurchaseOrderId { get; set; }
        public  byte[] PaymentVoucherRaw { get; set; }
        public bool BankTransfer { get; set; }
        public string Ext { get; set; }

    }

    public class PaymentVouchersModel
    {
        public ElectronicSign ElectronicSign { get; set; }
        public PurchaseSubscription PurchaseSubscription { get; set; }

    }

    public class TransactionRequest
    {
        public string status { get; set; }
        public string authorization_code { get; set; }
        public StatusDetailEnum status_detail { get; set; }
        public string message { get; set; }
        public string id { get; set; }
        public string payment_date { get; set; }
        public string payment_method_type { get; set; }
        public string dev_reference { get; set; }
        public string carrier_code { get; set; }
        public string product_description { get; set; }
        public string current_status { get; set; }
        public decimal amount { get; set; }
        public string carrier { get; set; }
        public int installments { get; set; }
        public string installments_type { get; set; }
        public string origin { get; set; }
        public string originId { get; set; }
        public string typeCard { get; set; }
    }

    public class CardRequest
    {
        public string bin { get; set; }
        public string status { get; set; }
        public long token { get; set; }
        public string expiry_year { get; set; }
        public string expiry_month { get; set; }
        public string transaction_reference { get; set; }
        public string type { get; set; }
        public string number { get; set; }
        public string origin { get; set; }

    }

    public class PaymentTransaction
    {
        public ECommerce ECommerce { get; set; }
        public TransactionRequest transaction { get; set; }
        public CardRequest card { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
    }
}
