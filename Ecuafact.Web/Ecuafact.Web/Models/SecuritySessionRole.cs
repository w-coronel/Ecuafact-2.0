using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecuafact.Web.Models
{
    public enum SecuritySessionRole
    {
        // Ninguno
        None = 0,
        // Usuario
        User = 1,
        // Emisor
        Issuer = 2,
        // Administrador
        Admin = 3,
        // Cooperativa
        Cooperative = 4
    }
}