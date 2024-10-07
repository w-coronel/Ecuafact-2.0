using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Ecuafact.WebAPI.Domain.Entities
{
    public class Vendor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }

        public Guid AppId { get; set; }
        public string ApplicationName { get; set; }
        public string BusinessName { get; set; }
        public string ApiKey { get; set; }
    }

    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public NotificationType? NotificationType { get; set; }
        public string UrlImage { get; set; }
        public string NameImage { get; set; }
        public bool IsEnabled { get; set; }
        public string Title { get; set; }
    }

    public enum NotificationType : short
    {
        Message = 1,
        Image = 2
    }
}

