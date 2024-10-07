using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities.Engine
{
    public class LogProceso
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }
        public long pkDocumento { get; set; }
        public int Secuencial { get; set; }
        public string Proceso { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime? Fin { get; set; }
        public bool Exito { get; set; }
        public string Error { get; set; }
        public string SOAP { get; set; } 
    }
}
