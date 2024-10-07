using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class AtsReporte
    {
        public List<VwAtsCompra> AtsCompraReporte { get; set; } = new List<VwAtsCompra>();
        public List<VwAtsPurchasesRetetion> AtsComprasRetncionReporte { get; set; } = new List<VwAtsPurchasesRetetion>();
        public List<VwAtsSale> AtsVentasReporte { get; set; } = new List<VwAtsSale>();
        public List<VwAtsRetencionVenta> AtsRetencionVentaReport { get; set; } = new List<VwAtsRetencionVenta>();
        public List<VwAtsVoidedDocument> AtsAnuladosReporte  { get; set; } = new List<VwAtsVoidedDocument>();

    }

    public enum PeriodTypeEnum : short
    {
        Monthly = 1,
        Biyearly = 2        
    }
}
