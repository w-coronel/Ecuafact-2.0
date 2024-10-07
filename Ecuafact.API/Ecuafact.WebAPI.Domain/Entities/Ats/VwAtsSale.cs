using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class VwAtsSale
    {
        public string IdInformante { get; set; } 
        public string RazonSocial { get; set; }
        public int? Anio { get; set; }
        public int? Mes { get; set; }
        public string EstablishmentCode { get; set; } 
        public string NumEstabRuc { get; set; }
        public string TpIdCliente { get; set; }
        public string IdCliente { get; set; }        
        public string TipoComprobante { get; set; } 
        public string DocumentName { get; set; }
        public string FormaPago { get; set; }        
        public int? NumeroComprobantes { get; set; }
        public decimal? BaseNoGraIva { get; set; }
        public decimal? BaseImponible { get; set; }
        public decimal? BaseImpGrav { get; set; }
        public decimal? MontoIva { get; set; }
    }
    public partial class VwAtsVoidedDocument
    {
        public string IdInformante { get; set; } 
        public string RazonSocial { get; set; } 
        public int? Anio { get; set; }
        public int? Mes { get; set; }
        public string EstablishmentCode { get; set; } 
        public int TotalVentas { get; set; }
        public string TipoComprobante { get; set; } 
        public string Establecimiento { get; set; } 
        public string PuntoEmision { get; set; } 
        public string SecuencialInicio { get; set; } 
        public string SecuencialFin { get; set; } 
        public string Autorizacion { get; set; }
        public DateTime? FechaEmision { get; set; }
    }
    public partial class VwAtsPurchasesRetetion
    {
        public long? Id { get; set; }
        public string IdInformante { get; set; }
        public string EstabRetencion1 { get; set; }
        public string PtoEmiRetencion1 { get; set; }
        public string SecRetencion1 { get; set; }
        public string AutRetencion1 { get; set; }
        public DateTime? FechaEmiRet1 { get; set; }
        public string IdProv { get; set; } 
        public string DenoProv { get; set; } 
        public string RetentionReferenceType { get; set; }
        public string RetentionReferenceNumber { get; set; } 
        public string RetentionReferenceDocumentAuth { get; set; } 
        public decimal ValRetBien10 { get; set; }
        public decimal ValRetServ20 { get; set; }
        public decimal ValRetServ30 { get; set; }
        public decimal ValRetServ50 { get; set; }
        public decimal ValRetServ70 { get; set; }
        public decimal ValRetServ100 { get; set; }
        public decimal TotbasesImpReemb { get; set; }
        public string CodRetAir { get; set; }
        public decimal BaseImpAir { get; set; }
        public decimal PorcentajeAir { get; set; }
        public decimal ValRetAir { get; set; }
    }
    public partial class VwAtsPurchasesSettlement : VwAtsCompra
    {
       
    }
    public partial class VwAtsSettlement : VwAtsFactura
    {

    }


}
