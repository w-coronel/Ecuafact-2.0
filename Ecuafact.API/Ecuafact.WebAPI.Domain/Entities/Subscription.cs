using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class Subscription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }
        public string RUC { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;          
        public long IssuerId { get; set; }        
        public long LicenceTypeId { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionExpirationDate { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public SubscriptionStatusEnum Status { get; set; }  
        public string AmountDocument { get; set; }
        public int? IssuedDocument { get; set; }
        public int? BalanceDocument { get; set; }
        public int? AmountIssuePoint { get; set; }
        public string StatusMsg { get; set; }        
        public Issuer Issuer { get; set; }
        public LicenceType LicenceType { get; set; }
    }

    public class SubscriptionLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }
        public string RUC { get; set; }        
        public long IssuerId { get; set; }       
        public long SubscriptionId { get; set; }
        public long LicenceTypeId { get; set; }       
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionExpirationDate { get; set; }       
        public int? IssuedDocument { get; set; }
        public int? BalanceDocument { get; set; }      
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public SubscriptionStatusEnum Status { get; set; }
        public string Observation { get; set; }
    }


    public class LicenceType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
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
        public bool TradeAgreement { get; set; }
        
    }

    public class PurchaseSubscription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public long IssuerId { get; set; }
        [ForeignKey("Subscription")]
        public long SubscriptionId { get; set; }
        [ForeignKey("PurchaseOrder")]
        public int PurchaseOrderId { get; set; }
        public PurchaseOrderSubscriptionStatusEnum Status  { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public long? InvoiceId { get; set; }
        public string InvoiceResult { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
        public Subscription Subscription { get; set; }
        public RequestElectronicSignEnum? RequestElectronicSign { get; set; }
        public string RequestElectronicSignMsg { get; set; }  
        public long? LicenceTypeId { get; set; }
        public PaymentTypeEnum? PaymentType { get; set; }
        public long? UserPaymentId { get; set; }
        [NotMapped]
        public SubscriptionLog SubscriptionLog { get; set; }
        public bool InvoicePrrocessed { get; set; }
    }



    public enum SubscriptionStatusEnum : short
    {
        [EcuafactEnum("-1", "No Registra")]
        Unregistered = -1,
        [EcuafactEnum("0", "Salvado")]
        Saved = 0,
        [EcuafactEnum("1", "Activa")]
        Activa = 1,
        [EcuafactEnum("2", "Inactiva")]
        Inactiva = 2,
        [EcuafactEnum("3", "Validando Pago")]
        ValidatingPayment = 3,
    }

    public enum LicenceTypeEnum : long
    {
        Free = 1,
        Basic = 2,
        Full = 3
    }

    public enum PurchaseOrderSubscriptionStatusEnum : short
    {        
        Saved,
        Payed,
        Rejected,
        ValidatingPayment
    }

    public enum RequestElectronicSignEnum : short
    {

        /// <summary>
        /// Pendiente de enviar solicitud
        /// </summary> 
        [EcuafactEnum("-1", "No incluye firma electrónica")]
        NotInclude = -1,
        /// <summary>
        /// Pendiente de enviar solicitud
        /// </summary> 
        [EcuafactEnum("0", "Pendiente de enviar solicitud")]
        Pending = 0,
        /// <summary>
        /// Enviada y procesada la solciitud
        /// </summary> 
        [EcuafactEnum("1", "Enviada y procesada la solciitud")]
        Sent = 1,        
    }

    public enum PaymentTypeEnum : short
    {
        CreditCard = 1,
        BankTransfer = 2,
        DebitCard = 3
    }





}
