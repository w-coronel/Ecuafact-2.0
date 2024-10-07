using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities
{
    public class SubscriptionRequest
    {
        public long Id { get; set; }
        public string RUC { get; set; }
        public long IssuerId { get; set; }        
        public DateTime SubscriptionStartDate { get; set; }
        public DateTime SubscriptionExpirationDate { get; set; }
        public SubscriptionStatusEnum Status { get; set; }
        public string StatusMsg { get; set; }
        public IssuerDto Issuer { get; set; }
        public decimal Price { get; set; }
        public SubscriptionInvoice InvoiceInfo { get; set; }
        public long LicenceTypeId { get; set; }
        public LicenceType LicenceType { get; set; }        
    }

    public class SubscriptionInvoice
    {
        public string Identification { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Product { get; set; }
        public string DiscountCode { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Iva { get; set; }
        public decimal IvaRate { get; set; }
    }

    public class PurchaseSubscription
    {
        public long Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public long IssuerId { get; set; }
        public long SubscriptionId { get; set; }
        public int PurchaseOrderId { get; set; }
        public PurchaseOrderSubscriptionStatusEnum Status { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public long? InvoiceId { get; set; }
        public string InvoiceResult { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public SubscriptionModel Subscription { get; set; }
        public PurchaseOrderModel PurchaseOrder { get; set; }
    }

    public class LicenceType
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public Decimal Price { get; set; }
        public Decimal TaxBase { get; set; }
        public Decimal Discount { get; set; }
        public string AmountDocument { get; set; }
        public bool IncludeCertificate { get; set; }
    }

    public enum PurchaseOrderSubscriptionStatusEnum : short
    {
        Saved,
        Payed,
        Rejected,
        ValidatingPayment
    }

    public class ReferenceCodes
    {       
        public long Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal DiscountRate { get; set; }
        public bool SkipPaymentOrder { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }        
        public int AgreementId { get; set; }
        public bool? AgreementInvoice { get; set; }
        public string RUC { get; set; }
        public string BusinessName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PromotionByPlan { get; set; }
        public string ApplyDiscount { get; set; }
        public bool SpecialPromotion { get; set; }
        public decimal DiscountValue { get; set; }
        public DiscountTypeEnum DiscountType { get; set; }
    }

    public class ECommerce
    {

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public OpeningModesEnum OpeningModes { get; set; }
        public string UrlCommerce { get; set; }
        public string UrlApiRestService { get; set; }
        public string UrlService { get; set; }
        public string ClientAppCode { get; set; }
        public string ClientAppKey { get; set; }
        public string ServerAppCode { get; set; }
        public string ServerAppKey { get; set; }
        public AmbientEnum Ambient { get; set; }
        public string FileClaveService { get; set; }
        public string FileClaveModal { get; set; }
        public string IDCommerceCode { get; set; }
        public string IDCommerceMall { get; set; }
        public string IDAcquirer { get; set; }
        public string IDWalletCode { get; set; }
        public string TerminalNumber { get; set; }
        public bool Priority { get; set; }

    }

    public class NotificationMessage
    {
        public string Message { get; set; } 
        public NotificationType? NotificationType { get; set; }
        public string UrlImage { get; set; }
        public string NameImage { get; set; }
        public string Title { get; set; }
    }

    public enum NotificationType:short
    {
        Message = 1,
        Image = 2
    }
    public enum OpeningModesEnum : short
    {
        Modal = 1,
        Redirect = 2
    }

    public enum AmbientEnum : short
    {
        Development = 1,
        Production = 2
    }

    public enum DiscountTypeEnum : short
    {
        Rate = 1,
        Value = 2
    }
}
