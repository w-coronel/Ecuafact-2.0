using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities.API
{
    public class PasswordModel
    {
        public string UniqueId { get; set; }
        public string NewPassword { get; set; }
        public string NewPassword2 { get; set; }
        public string Password { get; set; }
    }
}
