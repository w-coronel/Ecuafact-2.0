using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{

    public class BeneficiaryReferenceCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }
        public string DiscountCode { get; set; }
        public string Identification { get; set; }
        [ForeignKey("ReferenceCodes")]  
        public long ReferenceCodId { get; set; }        
        public long BeneficiaryId { get; set; }
        public long IssuerId { get; set; }
        public ReferenceCodeStatusEnum Status { get; set; }
        public string StatusMsg { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? LastModifiedOn { get; set; }
        public ReferenceCodes ReferenceCodes { get; set; }
    }
   
    public class ReferenceCodes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DiscountTypeEnum DiscountType { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal DiscountValue { get; set; }        
        public bool SkipPaymentOrder { get; set; }        
        public bool IsEnabled { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [ForeignKey("Agreement")]
        public int AgreementId { get; set; }
        public bool? AgreementInvoice { get; set; }
        public string RUC { get; set; }
        public string BusinessName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool? ElectronicSignSendProcess { get; set; }
        public bool? DiscountCommission { get; set; }
        public decimal? PlanL02 { get; set; }
        public decimal? PlanL03 { get; set; }
        public bool ValidateData { get; set; }
        public string PromotionByPlan { get; set; }
        public string ApplyDiscount { get; set; }
        public bool SpecialPromotion { get; set; }
        public Agreement Agreement { get; set; }
    }

    public class Agreement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int Id { get; set; }
        public string Code { get; set; }        
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime CreatedOn { get; set; }        
    }

    public class Beneficiary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }
        public string Identification { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsEnabled { get; set; }        
        public BeneficiaryStatusEnum Status { get; set; }
        public string StatusMsg { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? LastModifiedOn { get; set; }
        public int AgreementId { get; set; }        
        public long IssuerId { get; set; }
        public long? MediaCampaignsId { get; set; }        

        [NotMapped]
        public BeneficiaryReferenceCode BeneficiaryReferenceCode { get; set; }

        
    }

    public enum BeneficiaryStatusEnum : short
    {
        [EcuafactEnum("0", "Sin Regitrarse")]
        Inactiva = 0,
        [EcuafactEnum("1", "Registrado")]
        Activo = 1 
    }

    public enum ReferenceCodeStatusEnum : short
    {
        /// <summary>No se haplicado el descuento</summary>
        [EcuafactEnum("0", "Sin Aplicar")]
        WithoutApply = 0,
        /// <summary>Descuento aplicado</summary>
        [EcuafactEnum("1", "Aplicado")]
        Applied = 1
    }

    public enum DiscountTypeEnum : short
    {
        Rate = 1,
        Value = 2
    }
}
