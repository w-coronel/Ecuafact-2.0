using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class AtsCompras
    {
        public string TipoIDInformante { get; set; }
        public string IdInformante { get; set; }
        public string RazonSocial { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }
        public string NumEstabRuc { get; set; }
        public decimal? TotalVentas { get; set; }
        public string CodigoOperativo { get; set; }
        public string CodSustento { get; set; }
        public string TpIdProv { get; set; }
        public string IdProv { get; set; }
        public string ParteRel { get; set; }
        public string TipoComprobante { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string Establecimiento { get; set; }
        public string PuntoEmision { get; set; }
        public string Secuencial { get; set; }
        public DateTime? FechaEmision { get; set; }
        public string Autorizacion { get; set; }
        public decimal BaseNoGraIva { get; set; }
        public decimal BaseImponible { get; set; }
        public decimal BaseImpGrav { get; set; }
        public decimal BaseImpExe { get; set; }
        public decimal MontoIce { get; set; }
        public decimal MontoIva { get; set; }
        public decimal ValRetBien10 { get; set; } = 0;
        public decimal ValRetServ20 { get; set; } = 0;
        public decimal ValorRetBienes { get; set; } = 0;
        public decimal ValRetServ50 { get; set; } = 0;
        public decimal ValorRetServicios { get; set; } = 0;
        public decimal ValRetServ100 { get; set; } = 0;
        public decimal TotbasesImpReemb { get; set; } = 0;
        public string PagoLocExt { get; set; } = "01";
        public string PaisEfecPago { get; set; } = "NA";
        public string AplicConvDobTrib { get; set; } = "NA";
        public string PagExtSujRetNorLeg { get; set; } = "NA";
        public string PagoRegFis { get; set; } = "NA";
        public string FormaPago { get; set; }
        public string CodRetAir { get; set; }
        public decimal BaseImpAir { get; set; } = 0;
        public decimal PorcentajeAir { get; set; } = 0;
        public decimal ValRetAir { get; set; } = 0;
        public int NumCajBan { get; set; }
        public decimal PrecCajBan { get; set; }
        public string EstabRetencion1 { get; set; }
        public string PtoEmiRetencion1 { get; set; }
        public string SecRetencion1 { get; set; }
        public string AutRetencion1 { get; set; }
        public string FechaEmiRet1 { get; set; }
        public string DocModificado { get; set; }
        public string EstabModificado { get; set; }
        public string PtoEmiModificado { get; set; }
        public string SecModificado { get; set; }
        public string AutModificado { get; set; }
        public string DenoProv { get; set; }    
    }

    public partial class AtsFactura
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
}
