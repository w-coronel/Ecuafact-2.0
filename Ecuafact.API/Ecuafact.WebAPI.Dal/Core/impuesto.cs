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
    
    public partial class impuesto
    {
        public long pk { get; set; }
        public long pkDetalle { get; set; }
        public string codigo { get; set; }
        public string codigoPorcentaje { get; set; }
        public decimal tarifa { get; set; }
        public decimal baseImponible { get; set; }
        public decimal valor { get; set; }
    
        public virtual detalle detalle { get; set; }
    }
}
