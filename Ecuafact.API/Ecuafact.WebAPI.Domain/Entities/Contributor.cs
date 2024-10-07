using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class Contributor
    {
        public Contributor() { }
          
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }

        [StringLength(50)]
        public string Identification { get; set; }

        public IdentificationType IdentificationType { get; set; }
        public short IdentificationTypeId { get; set; }

        /// <summary>
        /// Razon Social
        /// </summary>
        [StringLength(300)]
        [Required]
        public string BussinesName { get; set; }

        /// <summary>
        /// Nombre Comercial
        /// </summary>
        [StringLength(300)]
        public string TradeName { get; set; }

        [StringLength(300)]
        public string Address { get; set; }

        [StringLength(300)]
        public string Phone { get; set; }

        [StringLength(1500)]
        public string EmailAddresses { get; set; }
         
        public Issuer Issuer { get; set; }
        public long IssuerId { get; set; }

        public bool IsSupplier { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsDriver { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? LastModifiedOn { get; set; }

        public bool IsEnabled { get; set; }

    }

    public class ContributorType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public short Id { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        public bool IsEnabled { get; set; }

    }
}
