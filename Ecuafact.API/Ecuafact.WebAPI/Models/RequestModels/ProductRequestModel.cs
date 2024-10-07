using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Models
{
    /// <summary>
    /// Producto
    /// </summary>
    public class ProductRequestModel
    {
        /// <summary>
        /// Codigo Principal
        /// </summary>
        [Required]
        [StringLength(500)]
        public string MainCode { get; set; }
        /// <summary>
        /// Codigo auxiliar
        /// </summary>
        [StringLength(500)]
        public string AuxCode { get; set; }
        /// <summary>
        /// Nombre del producto
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Name { get; set; }
        /// <summary>
        /// Precio unitario 
        /// </summary>
        [Required]
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// Tipo de producto.
        /// </summary>
        [Required]
        public short ProductTypeId { get; set; }
        /// <summary>
        /// Tarifa del Iva para el producto
        /// </summary>
        [Required]
        public short IvaRateId { get; set; }
        /// <summary>
        /// Tarifa del ICE para el producto
        /// </summary>
        public short IceRateId { get; set; }
        /// <summary>
        /// Esta habilitado?
        /// </summary>
        [Required]
        public bool IsEnabled { get; set; }
        /// <summary>
        /// Nombre detalle adicional 1
        /// </summary>        
        public string Name1 { get; set; }
        /// <summary>
        /// Valor detalle adicional 1
        /// </summary> 
        public string Value1 { get; set; }
        /// <summary>
        /// Nombre detalle adicional 2
        /// </summary> 
        public string Name2 { get; set; }
        /// <summary>
        /// Valor detalle adicional 2
        /// </summary> 
        public string Value2 { get; set; }
        /// <summary>
        /// Nombre detalle adicional 3
        /// </summary> 
        public string Name3 { get; set; }
        /// <summary>
        /// Valor detalle adicional 3
        /// </summary> 
        public string Value3 { get; set; }
    }


    /// <summary>
    /// Inserción masiva de productos
    /// </summary>
    public class ProductImportModel : ImportModel
    {
        
    }

}
