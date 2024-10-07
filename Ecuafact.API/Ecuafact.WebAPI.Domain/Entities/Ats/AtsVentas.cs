using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class AtsVentas
    {
        public string TipoIDInformante { get; set; }
        public string IdInformante { get; set; }
        public string RazonSocial { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }
        public string NumEstabRuc { get; set; }
        public decimal TotalVentas { get; set; } 
        public string CodigoOperativo { get; set; }
        public string TpIdCliente { get; set; }
        public string IdCliente { get; set; }
        public string TipoComprobante { get; set; }
        public string TipoEmision { get; set; }
        public int? NumeroComprobantes { get; set; }
        public decimal BaseNoGraIva { get; set; }
        public decimal BaseImponible { get; set; }
        public decimal BaseImpGrav { get; set; }
        public decimal MontoIva { get; set; }
        public decimal MontoIce { get; set; }
        public decimal ValorRetIva { get; set; }
        public decimal ValorRetRenta { get; set; }
    }

    public class AtsAnulados
    {
        public string TipoIDInformante { get; set; }
        public string IdInformante { get; set; }
        public string RazonSocial { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }
        public string NumEstabRuc { get; set; }
        public decimal TotalVentas { get; set; }
        public string CodigoOperativo { get; set; }       
        public string TipoComprobante { get; set; }
        public string Establecimiento { get; set; }
        public string PuntoEmision { get; set; }
        public string SecuencialInicio { get; set; }
        public string SecuencialFin { get; set; }
        public string Autorizacion { get; set; }
        
    }

    public class AtsVentasEstablecimiento
    {
        public string TipoIDInformante { get; set; }
        public string IdInformante { get; set; }
        public string RazonSocial { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }
        public string NumEstabRuc { get; set; }
        public decimal TotalVentas { get; set; }
        public string CodigoOperativo { get; set; }
        public string CodEstab { get; set; }
        public decimal VentasEstab { get; set; }
    }
}
