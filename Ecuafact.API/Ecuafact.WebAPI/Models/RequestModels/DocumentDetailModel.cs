using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Ecuafact.WebAPI.Models
{

    /// <summary>
    /// Detalle de los Documentos
    /// </summary>
    public class DocumentDetailModel
    {
        /// <summary>
        /// Detalle de los Documentos
        /// </summary>
        public DocumentDetailModel()
        {
            Taxes = new List<TaxModel>();
        }

        /// <summary>
        /// Id del Producto
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// Codigo Principal del Producto
        /// </summary>
        [StringLength(500)]
        public string MainCode { get; set; }

        /// <summary>
        /// Codigo Auxiliar del Producto
        /// </summary>
        [StringLength(500)]
        public string AuxCode { get; set; }

        /// <summary>
        /// Descripcion del Producto
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Precio Unitario
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Cantidad
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Descuento
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// Codigo del Impuesto al Valor Agregado (IVA) que se aplica al producto
        /// </summary>
        public string ValueAddedTaxCode { get; set; }

        /// <summary>
        /// Valor del Impuesto al Valor Agregado (IVA) que se aplica al producto
        /// </summary>
        public decimal ValueAddedTaxValue { get; set; }

        /// <summary>
        /// Codigo del Impuesto a los Consumos Especiales (ICE) en caso que aplique
        /// </summary>
        public string SpecialConsumTaxCode { get; set; }

        /// <summary>
        /// Valor del Impuesto a los Consumos Especiales (ICE) en caso que aplique
        /// </summary>
        public decimal SpecialConsumTaxValue { get; set; }

        /// <summary>
        /// Total del detalle sin impuestos
        /// </summary>
        public decimal SubTotal { get; set; }

        /// <summary>
        /// Impuestos [OPCIONAL] (En caso de existir información de IVA (ValueAddedTaxCode) o ICE (SpecialConsumTaxCode))
        /// </summary>
        public List<TaxModel> Taxes { get; set; }

        /// <summary>
        /// Nombre del Campo del Detalle Adicional
        /// </summary>
        [MaxLength(300)]
        public string Name1 { get; set; }

        /// <summary>
        /// Valor del Campo del Detalle Adicional
        /// </summary>
        [MaxLength(300)]
        public string Value1 { get; set; }

        /// <summary>
        /// Nombre del Campo del Detalle Adicional
        /// </summary>
        [MaxLength(300)]
        public string Name2 { get; set; }

        /// <summary>
        /// Valor del Campo del Detalle Adicional
        /// </summary>

        [MaxLength(300)]
        public string Value2 { get; set; }

        /// <summary>
        /// Nombre del Campo del Detalle Adicional
        /// </summary>
        [MaxLength(300)]
        public string Name3 { get; set; }

        /// <summary>
        /// Valor del Campo del Detalle Adicional
        /// </summary>

        [MaxLength(300)]
        public string Value3 { get; set; }
    }
}