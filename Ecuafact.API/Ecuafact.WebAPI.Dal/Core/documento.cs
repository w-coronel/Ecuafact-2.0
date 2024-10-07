//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ecuafact.WebAPI.Dal.Core
{
    using System;
    using System.Collections.Generic;
    
    public partial class documento
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public documento()
        {
            this.campoAdicional = new HashSet<campoAdicional>();
            this.detalle = new HashSet<detalle>();
            this.FormaPago = new HashSet<FormaPago>();
            this.infoFactura = new HashSet<infoFactura>();
        }
    
        public long pk { get; set; }
        public string PkDocumento { get; set; }
        public Nullable<int> SecuencialUltimoProceso { get; set; }
        public string version { get; set; }
        public string id { get; set; }
        public string ambiente { get; set; }
        public string tipoEmision { get; set; }
        public string razonSocial { get; set; }
        public string nombreComercial { get; set; }
        public string ruc { get; set; }
        public string claveAcceso { get; set; }
        public string codDoc { get; set; }
        public string estab { get; set; }
        public string ptoEmi { get; set; }
        public string secuencial { get; set; }
        public string dirMatriz { get; set; }
        public string numeroautorizacion { get; set; }
        public Nullable<int> Estado { get; set; }
        public string StrEstado { get; set; }
        public string fechaAutorizacion { get; set; }
        public Nullable<System.DateTime> FechaIngreso { get; set; }
        public Nullable<System.DateTime> FechaUltimoIntento { get; set; }
        public Nullable<System.DateTime> FechaRecepcionCorreo { get; set; }
        public Nullable<System.DateTime> FechaEnvioCorreo { get; set; }
        public Nullable<System.DateTime> FechaVisualizaciónCliente { get; set; }
        public Nullable<System.DateTime> FechaReenvioCorreo { get; set; }
        public string emails { get; set; }
        public Nullable<decimal> PorcentajeDescuento { get; set; }
        public Nullable<short> EstadoProceso { get; set; }
        public Nullable<System.DateTime> FechaHibernacion { get; set; }
        public string UsuarioIngreso { get; set; }
        public Nullable<System.DateTime> FechaEnvio { get; set; }
        public string PDF { get; set; }
        public string XML { get; set; }
        public string MSG { get; set; }
        public string IDMAIL { get; set; }
        public string CodDoc2 { get; set; }
        public string XMLC { get; set; }
        public string PDFC { get; set; }
        public Nullable<int> Prioridad { get; set; }
        public Nullable<int> idDeducible { get; set; }
        public string Destino { get; set; }
        public string Origen { get; set; }
        public Nullable<byte> ResultadoEnviado { get; set; }
        public Nullable<byte> Replicado { get; set; }
        public Nullable<byte> Validado { get; set; }
        public Nullable<bool> AutorizarAhora { get; set; }
        public Nullable<int> EstadoExtra1 { get; set; }
        public Nullable<System.DateTime> FechaEstadoExtra1 { get; set; }
        public Nullable<int> EstadoExtra2 { get; set; }
        public Nullable<System.DateTime> FechaEstadoExtra2 { get; set; }
        public string EstadoExtraObservacion1 { get; set; }
        public string EstadoExtraObservacion2 { get; set; }
        public Nullable<int> numregistros { get; set; }
        public Nullable<bool> MigracionNube { get; set; }
        public string ErrorMigracion { get; set; }
        public Nullable<bool> MigracionRegion { get; set; }
        public string ErrorMigracionRegion { get; set; }
        public Nullable<bool> Offline { get; set; }
        public Nullable<System.DateTime> FechaEnvioSRI { get; set; }
        public string CodSucursal { get; set; }
        public Nullable<short> EstadoUltimaGestion { get; set; }
        public Nullable<System.DateTime> FechaUltimaGestion { get; set; }
        public Nullable<bool> DatosRecepcionEnviados { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<campoAdicional> campoAdicional { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<detalle> detalle { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormaPago> FormaPago { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<infoFactura> infoFactura { get; set; }
    }
}
