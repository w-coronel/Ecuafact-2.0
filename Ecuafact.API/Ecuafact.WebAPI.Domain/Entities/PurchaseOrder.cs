using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class PurchaseOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int PurchaseOrderId { get; set; }
        public DateTime IssuedOn { get; set; }
        public string Identification { get; set; }
        public string FirstName { get; set; }
        public string BusinessName { get; set; }
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
        public PurchaseOrderStatusEnum Status { get; set; }
        public long IssuerId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal TotalDiscount { get; set; }        
        public decimal Discount { get; set; }
        public long ? BeniReferCodeId { get; set; }
        public string PaymentVoucher { get; set; }
        public bool BankTransfer { get; set; } = false;
        public string DiscountCode { get; set; }
        public string Bank { get; set; }

        [NotMapped]
        public decimal Subtotal => Subtotal0 + Subtotal12;

        [NotMapped]        
        public ReferenceCodes ReferenceCodes { get; set;}
    }


    public enum PurchaseOrderStatusEnum : short
    {
        Saved,
        Payed,
        Rejected,
        Transfer,
    }
}
