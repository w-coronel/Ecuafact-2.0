using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class UserPermissions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }

        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [Column("RoleId")]
        public UserRolEnum Role { get; set; }

        [Required]
        public long IssuerId { get; set; }

        public string Modules { get; set; }


        [ForeignKey("IssuerId")]
        public Issuer Issuer { get; set; }

    }
    public class UserPayment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }
        public string BussinesName { get; set; }
        public string Identification { get; set; }
        public long LicenceTypeId { get; set; }
        public string NamePlan { get; set; }
        public UserPaymentStatusEnum Status { get; set; } = UserPaymentStatusEnum.Validate;
        public string StatusMsg { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? LastModifiedOn { get; set; }
        public string OrderNumber { get; set; }
    }
    public enum UserRolEnum : int
    {
        None = 0,
        User = 1,
        Issuer = 2,
        Admin = 3,
        Cooperative = 4

    }
    public enum UserPaymentStatusEnum : short
    {
        Validate = 0,
        Applied = 1
    }
}
