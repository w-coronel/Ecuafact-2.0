using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ecuafact.WebAPI.Domain.Entities
{
    /// <summary>
    /// Detalle de Guias de Remision
    /// </summary>
    public class ReferralGuideDetail
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]

        public long Id { get; set; }
        /// <summary>
        /// Informacion de la Guia de Remision
        /// </summary>
        [Required]
        public long ReferralGuideInfoId { get; set; }
        /// <summary>
        /// Producto
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// Codigo principal del Producto
        /// </summary>
        public string MainCode { get; set; }
        /// <summary>
        /// Codigo Auxiliar del Producto
        /// </summary>
        public string AuxCode { get; set; }
        /// <summary>
        /// Descripcion del Producto
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Cantidad
        /// </summary>
        public decimal Quantity { get; set; }
        /// <summary>
        /// Detalle 1
        /// </summary>
        [MaxLength(300)]
        public string Name1 { get; set; }
        /// <summary>
        /// Valor 1
        /// </summary>
        [MaxLength(300)]
        public string Value1 { get; set; }
        /// <summary>
        /// Detalle 2
        /// </summary>
        [MaxLength(300)]
        public string Name2 { get; set; }
        /// <summary>
        /// Valor 2
        /// </summary>
        [MaxLength(300)]
        public string Value2 { get; set; }
        /// <summary>
        /// Detalle 3
        /// </summary>
        [MaxLength(300)]
        public string Name3 { get; set; }
        /// <summary>
        /// Valor 3
        /// </summary>
        [MaxLength(300)]
        public string Value3 { get; set; }
        /// <summary>
        /// Guia de Remision
        /// </summary>
        [JsonIgnore]
        public ReferralGuideInfo ReferralGuideInfo { get; set; }
    }
}
