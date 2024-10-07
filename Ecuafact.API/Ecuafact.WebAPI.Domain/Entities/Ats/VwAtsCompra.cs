using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public partial class VwAtsCompra
    {
        public string IdInformante { get; set; }
        public string RazonSocial { get; set; } 
        public int Anio { get; set; }
        public int Mes { get; set; }
        public string NumEstabRuc { get; set; }
        public string CodSustento { get; set; }
        public string TpIdProv { get; set; }
        public string IdProv { get; set; }
        public string DenoProv { get; set; }
        public string TipoComprobante { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string Establecimiento { get; set; }
        public string PuntoEmision { get; set; }
        public string Secuencial { get; set; }
        public DateTime? FechaEmision { get; set; }
        public string Autorizacion { get; set; }
        public decimal BaseNoGraIva { get; set; }
        public decimal BaseImpExe { get; set; }
        public decimal BaseImponible { get; set; }
        public decimal BaseImpGrav { get; set; }
        public int MontoIce { get; set; }
        public decimal MontoIva { get; set; }
        public string FormaPago { get; set; }
        public string DocModificado { get; set; }
        public string EstabModificado { get; set; }
        public string PtoEmiModificado { get; set; }
        public string SecModificado { get; set; }
        public string AutModificado { get; set; }

    }
    public partial class VwAtsRetencionVenta
    {
        public string IdInformante { get; set; }
        public string DocumentoCodigoReferencia { get; set; }
        public decimal? ValorRetIva { get; set; }
        public decimal? ValorRetRenta { get; set; }
        public string IdCliente { get; set; }
        public int? Anio { get; set; }
        public int? Mes { get; set; }
    }
    public partial class VwAtsFactura
    {
        public long Id { get; set; }
        public DateTime? FechaEmision { get; set; }
        public string TipoDocumento { get; set; }
        public int? Anio { get; set; }
        public int? Mes { get; set; }
        public string DocumentoType { get; set; }
        public string NumeroDocumento { get; set; }
        public string Numeroautorizacion { get; set; }
        public string FechaAutorizacion { get; set; }
        public string Rucproveedor { get; set; }
        public string RazonSocialProveedor { get; set; } 
        public decimal Total { get; set; }
        public string Ruc { get; set; }
        public string RazonSocial { get; set; } 
        public string CodSustento { get; set; }
        public int TipoEmision { get; set; }       
        
    }
    public partial class StatisticsAts
    {
        public int Factura { get; set; }
        public int NotaVenta { get; set; }        
        public int NotaCredito { get; set; }
        public int NotaDebito { get; set; }
        public int Retencion { get; set; }
        [NotMapped]
        public int CountDocument
        {
            get
            {
                return (Factura+ NotaVenta + NotaCredito + NotaDebito + Retencion);
            }
            set { } // do nothing x) por si acaso
        }
    }
}
