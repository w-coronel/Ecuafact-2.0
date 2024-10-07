using System;
namespace Ecuafact.Web.Domain.Entities
{
    public class ConsultaDocumentosParamsDto
    {
        public string RucEmisor { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaHasta { get; set; }
        public string Contenido { get; set; }
        public string TipoDocumento { get; set; }
        public string Deducible { get; set; }  
        public string TipoFecha { get; set; }
        public int? NumeroPagina { get; set; } = null;       
        public int? CantidadPorPagina { get; set; } = null;
    }
}