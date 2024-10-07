using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities.App
{
    
    public partial class ApiV3_DocumentosData
    {
        [Key]
        public long Pk { get; set; }
        public short? EstadoProceso { get; set; }
        public bool? Offline { get; set; }
        public string PkDocumento { get; set; } 
        public DateTime? FechaEnvioSri { get; set; }
        public string AutSri { get; set; } 
        public int? EstadoExtra1 { get; set; }
        public DateTime? FechaEstadoExtra1 { get; set; }
        public int? EstadoExtra2 { get; set; }
        public DateTime? FechaEstadoExtra2 { get; set; }
        public string EstadoExtraObservacion1 { get; set; }
        public string EstadoExtraObservacion2 { get; set; }
        public DateTime? Fecha { get; set; }
        public string FechaAut { get; set; }
        public int IdDeducible { get; set; }
        public string NumeroIdentificacionEmisor { get; set; }
        public string NombreEmisor { get; set; } 
        public string NumeroIdentificacion { get; set; }
        public string Nombre { get; set; } 
        public string CodTipoDoc { get; set; }
        public string Secuencia { get; set; }
        public decimal Subtotal12 { get; set; }
        public decimal Subtotal15 { get; set; }
        public decimal Subtotal0 { get; set; }
        public decimal Subtotal { get; set; }
        public decimal? Iva { get; set; }
        public decimal Total { get; set; }
        public int? Estado { get; set; }
        public string ClaveAcceso { get; set; }
        public string NumDocRel { get; set; }
        public string Numeroautorizacion { get; set; }
        public string Pdf { get; set; }
        public string Xml { get; set; }
        public string Msg { get; set; }
        public string Idmail { get; set; }
        public short? EstadoUltimaGestion { get; set; }
        public DateTime? FechaUltimaGestion { get; set; }
        public DateTime? FechaSys { get; set; }
        public string Secuencial { get; set; }
        public string RazonSocial { get; set; } 
        public string NombreComercial { get; set; }
        public int Replicado { get; set; }
        public DateTime? FechaEmision { get; set; }
        public string CodSustento { get; set; }
    }


    public class ElementByPage
    {
        public int TotalRows { get; set; }
        public int TotalPages { get; set; }
    }

}
