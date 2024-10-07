using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities
{
    public class PurchaseOrderModel
    {
        public int PurchaseOrderId { get; set; }
        public string UserCodePayme { get; set; }
        public DateTime IssuedOn { get; set; }
        public string Identification { get; set; }
        public string BusinessName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string ZIP { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string Products { get; set; }
        public decimal Subtotal0 { get; set; }
        public decimal Subtotal12 { get; set; }
        public decimal IVA { get; set; }
        public decimal Additional { get; set; }
        public decimal Interests { get; set; }
        public decimal ICE { get; set; }
        public decimal Total { get; set; }
        public decimal Subtotal { get { return Subtotal0 + Subtotal12; } }
        public PurchaseOrderStatusEnum Status { get; set; }
        public long IssuerId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public ReferenceCodesModel ReferenceCodes { get; set; }
        public string UrlRedirect { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal Discount { get; set; }
        public string DiscountCode { get; set; }
    }
    public enum PurchaseOrderStatusEnum : short
    {
        Saved,
        Payed,
        Rejected,
        Transfer
    }

    public class ReferenceCodesModel
    {  
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal DiscountRate { get; set; }
        public bool SkipPaymentOrder { get; set; }        
        public int AgreementId { get; set; }

    }
}
