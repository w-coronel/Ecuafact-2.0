using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ecuafact.Web.Models
{
    public class ReporteModel
    {
        /*[Display(Name = "Tipo Emisión")]
        public string IdEmision { get; set; }

        [Display(Name = "Tipo Docuento")]
        public string IdDocumento { get; set; }

        [Display(Name = "Emisor")]
        public string Emisor { get; set; }

        [Display(Name = "Desde")]
        public string Desde { get; set; }

        [Display(Name = "Hasta")]
        public string Hasta { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; }

        [Display(Name = "Establecimiento")]
        public string Establecimiento { get; set; }

        [Display(Name = "PuntoEmision")]
        public string PuntoEmision { get; set; }

		[Display(Name = "Region")]
		public string Region { get; set; }*/

        [Display(Name = "Mensaje")]
        public string Mensaje { get; set; }

		[Display(Name = "Emisor")]
        public string Emisor { get; set; }

		[Display(Name = "Tipo Documento")]
        public string TipoDoc { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; }

        [Display(Name = "Desde")]
        public string Desde { get; set; }

        [Display(Name = "Hasta")]
        public string Hasta { get; set; }

        [Display(Name = "Tipo Emisión")]
		public string TipoComprobante { get; set; }

		[Display(Name = "Establecimiento")]
		public string Establecimiento { get; set; }

		[Display(Name = "PuntoEmision")]
		public string PuntoEmision { get; set; }

		[Display(Name = "Region")]
		public string Region { get; set; }

    }

}