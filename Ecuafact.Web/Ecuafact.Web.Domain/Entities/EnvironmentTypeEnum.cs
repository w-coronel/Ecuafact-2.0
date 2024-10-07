using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities
{
    public enum EnvironmentTypeEnum : int
    {
        [EcuafactEnum("1", "Pruebas")]
        [Description("Pruebas")]
        Pruebas = 1,

        [EcuafactEnum("2", "Produccion")]
        [Description("Producción")]
        Producción = 2
    }
}
