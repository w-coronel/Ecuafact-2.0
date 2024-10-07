using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities
{
    public class UserPermissionsModel
    { 
         public string Username { get; set; }

         public UserRolEnum Role { get; set; }

         public long IssuerId { get; set; }

        public string Modules { get; set; }


         public IssuerDto Issuer { get; set; }
    }


    public enum UserRolEnum : int
    {
        None = 0,
        User = 1,
        Issuer = 2,
        Admin = 3
    }

    public enum SubscriptionStatusEnum : short
    {
        [EcuafactEnum("-1", "No Registra")]
        Unregistered = -1,
        [EcuafactEnum("0", "Salvado")]
        Saved = 0,
        [EcuafactEnum("1", "Activa")]
        Activa = 1,
        [EcuafactEnum("1", "Inactiva")]
        Inactiva = 2,
        [EcuafactEnum("3", "Validando Pago")]
        ValidatingPayment = 3,
    }
}
