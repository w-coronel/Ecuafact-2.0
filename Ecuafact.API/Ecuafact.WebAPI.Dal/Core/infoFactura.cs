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
    
    public partial class infoFactura
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public infoFactura()
        {
            this.totalImpuesto = new HashSet<totalImpuesto>();
        }
    
        public long pk { get; set; }
        public long PkDocumento { get; set; }
        public string fechaEmision { get; set; }
        public string dirEstablecimiento { get; set; }
        public string contribuyenteEspecial { get; set; }
        public string obligadoContabilidad { get; set; }
        public string tipoIdentificacionComprador { get; set; }
        public string guiaRemision { get; set; }
        public string razonSocialComprador { get; set; }
        public string identificacionComprador { get; set; }
        public decimal totalSinImpuestos { get; set; }
        public decimal totalDescuento { get; set; }
        public decimal propina { get; set; }
        public decimal importeTotal { get; set; }
        public string moneda { get; set; }
        public Nullable<decimal> SubTotal12 { get; set; }
        public Nullable<decimal> SubTotal0 { get; set; }
        public string rise { get; set; }
        public string codDocModificado { get; set; }
        public string numDocModificado { get; set; }
        public string fechaEmisionDocSustento { get; set; }
        public Nullable<decimal> valorModificacion { get; set; }
        public string motivo { get; set; }
        public string periodoFiscal { get; set; }
        public string dirpartida { get; set; }
        public string fechainitransporte { get; set; }
        public string fechafintransporte { get; set; }
        public string placa { get; set; }
        public string identificaciondestinatario { get; set; }
        public string razonsocialdestinatario { get; set; }
        public string dirdestinatario { get; set; }
        public Nullable<decimal> descuentoAdicional { get; set; }
        public string numAutDocSustento { get; set; }
        public Nullable<decimal> SubTotalNoObjetoIVA { get; set; }
        public Nullable<decimal> SubTototalExcentoIVA { get; set; }
        public string incoTermFactura { get; set; }
        public string lugarIncoTerm { get; set; }
        public string paisOrigen { get; set; }
        public string puertoEmbarque { get; set; }
        public string puertoDestino { get; set; }
        public string paisDestino { get; set; }
        public string paisAdquisicion { get; set; }
        public string direccionComprador { get; set; }
        public string incoTermTotalSinImpuestos { get; set; }
        public Nullable<decimal> fleteInternacional { get; set; }
        public Nullable<decimal> seguroInternacional { get; set; }
        public Nullable<decimal> gastosAduaneros { get; set; }
        public Nullable<decimal> gastosTransporteOtros { get; set; }
        public Nullable<decimal> SubTotal14 { get; set; }
    
        public virtual documento documento { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<totalImpuesto> totalImpuesto { get; set; }
    }
}
