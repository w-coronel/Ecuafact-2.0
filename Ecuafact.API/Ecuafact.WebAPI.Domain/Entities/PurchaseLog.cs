using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class PurchaseLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }
        public string OperationNumber { get; set; }
        public DateTime Date { get; set; }
        public string Result { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }

    }
}
