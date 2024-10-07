using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities
{

    public enum IssueTypeEnum : int
    {
        [EcuafactEnum("1", "Normal")]
        Normal = 1,
        [EcuafactEnum("2", "No Disponible")]
        [Description("No Disponible")]
        Indisponilidad = 2
    }

    public enum PeriodTypeEnum : short
    {
        Monthly = 1,
        Biyearly = 2
    }
}
