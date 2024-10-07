using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities.SRI
{
    [Table("INFORUC")]
    public  class INFORUC
    {
        [Key, Column(Order =0), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string NUMERO_RUC { get; set; }
        public string RAZON_SOCIAL { get; set; }
        public string NOMBRE_COMERCIAL { get; set; }
        public string ESTADO_CONTRIBUYENTE { get; set; }
        public string CLASE_CONTRIBUYENTE { get; set; }
        public string FECHA_INICIO_ACTIVIDADES { get; set; }
        public string FECHA_ACTUALIZACION { get; set; }
        public string FECHA_SUSPENSION_DEFINITIVA { get; set; }
        public string FECHA_REINICIO_ACTIVIDADES { get; set; }
        public string OBLIGADO { get; set; }
        public string TIPO_CONTRIBUYENTE { get; set; }
        [Key, Column(Order = 1)]
        public string NUMERO_ESTABLECIMIENTO { get; set; }
        public string NOMBRE_FANTASIA_COMERCIAL { get; set; }
        public string CALLE { get; set; }
        public string NUMERO { get; set; }
        public string INTERSECCION { get; set; }
        public string ESTADO_ESTABLECIMIENTO { get; set; }
        public string DESCRIPCION_PROVINCIA { get; set; }
        public string DESCRIPCION_CANTON { get; set; }
        public string DESCRIPCION_PARROQUIA { get; set; }
        public string CODIGO_CIIU { get; set; }
        public string ACTIVIDAD_ECONOMICA { get; set; }

        [NotMapped]
        public long UID { get { return Convert.ToInt64(NUMERO_RUC) + Convert.ToInt64(NUMERO_ESTABLECIMIENTO) - 1; } }
    }

    [Table("INFOREGIMEN")]
    public class INFOREGIMEN
    {
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string NUMERO_RUC { get; set; }
        public string RAZON_SOCIAL { get; set; }
        public string ZONAL { get; set; }
        public string REGIMEN { get; set; }
        public string NEGOCIO_POPULAR { get; set; }
        
    }
}
