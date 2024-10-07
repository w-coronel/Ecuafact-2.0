using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Ecuafact.Web.Domain.Entities
{
    public class PaymentRequestModel
    { 
        public PurchaseOrderModel PurchaseOrder { get; set; }
        public TransactionRequest transaction { get; set; }
        public bool development { get; set; } = false;
        public string acquirerId { get; set; }
        public string idCommerce { get; set; }
        public string purchaseOperationNumber { get; set; }
        public string purchaseAmount { get; set; }
        public string purchaseCurrencyCode { get; set; }
        public string language { get; set; }
        public string shippingFirstName { get; set; }
        public string shippingLastName { get; set; }
        public string shippingEmail { get; set; }
        public string shippingAddress { get; set; }
        public string shippingZIP { get; set; }
        public string shippingCity { get; set; }
        public string shippingState { get; set; }
        public string shippingCountry { get; set; }
        public string userCommerce { get; set; }
        public string userCodePayme { get; set; }
        public string descriptionProducts { get; set; }
        public string programmingLanguage { get; set; }
        public string purchaseVerification { get; set; }
        public string reserved1 { get; set; }
        public string reserved2 { get; set; }
        public string reserved3 { get; set; }
        public string reserved4 { get; set; }
        public string reserved5 { get; set; }
        public string reserved6 { get; set; }
        public string reserved9 { get; set; }
        public string reserved11 { get; set; }
        public string reserved12 { get; set; }
        public bool subscriptionActiva { get; set; }
        public long tipoProceso { get; set; }
        public bool modalComercio { get; set; }
        public string urlComercio { get; set; }
        public long paymentId { get; set; }
        public string carrierCode { get; set; }
        public string currentStatus { get; set; }
        public string transactionId { get; set; }
        public string paymentDate { get; set; }
        public string authorizationCode { get; set; }
        public string status { get; set; }
        


    }

    public class PaymentVoucherRequest
    {
        public PurchaseOrderModel PurchaseOrder { get; set; }
        public long PurchaseOrderId { get; set; }
        [JsonIgnore]
        public HttpPostedFileBase PaymentVoucher { get; set; }
        public byte[] PaymentVoucherRaw => paymentVoucherRaw ?? (paymentVoucherRaw = PaymentVoucher.GetBytes());
        private byte[] paymentVoucherRaw;
        public string Ext { get; set; }

    }

    public class PaymentVoucherModel
    {
        public ElectronicSignModel ElectronicSign { get; set; }
        public PurchaseSubscription PurchaseSubscription { get; set; }

    }

    public class TransactionRequest
    {
        public string status { get; set; }
        public string authorization_code { get; set; }
        public short status_detail { get; set; }
        public string message { get; set; }
        public string id { get; set; }
        public string payment_date { get; set; }
        public string payment_method_type { get; set; }
        public string dev_reference { get; set; }
        public string carrier_code { get; set; }
        public string product_description { get; set; }
        public string current_status { get; set; }
        public long amount { get; set; }
        public string carrier { get; set; }
        public long installments { get; set; }
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
        public TransactionRequest transaction { get; set; }
        public CardRequest card { get; set; }
        public PurchaseOrderModel PurchaseOrder { get; set; }

    }
}
