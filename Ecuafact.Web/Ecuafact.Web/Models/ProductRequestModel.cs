using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Models
{
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
    }
}
