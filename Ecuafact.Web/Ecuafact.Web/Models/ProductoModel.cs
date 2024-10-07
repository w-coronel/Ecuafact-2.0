using Ecuafact.Web.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Ecuafact.Web.Models
{
    public class ProductoModel
    {
        [Display(Name = "Código")]
        public int? IdProducto { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "{0} es obligatorio.")]
        [StringLength(100, ErrorMessage = "{0} debe contener máximo {1} caracteres.")]
        public string Nombre { get; set; }

        [Display(Name = "Código Principal")]
        [Required(ErrorMessage = "{0} es obligatorio.")]
        [StringLength(50, ErrorMessage = "{0} debe contener máximo {1} caracteres.")]
        public string CodigoPrincipal { get; set; }

        [Display(Name = "Código Auxiliar")]
        [StringLength(50, ErrorMessage = "{0} debe contener máximo {1} caracteres.")]
        public string CodAuxiliar { get; set; }

        [Display(Name = "Precio Unitario")]
        [Required(ErrorMessage = "{0} es obligatorio.")]
        public decimal PrecioUnitario { get; set; }

        [Display(Name = "Grava IVA")]
        [Required(ErrorMessage = "{0} es obligatorio.")]
        public bool GravaIVA { get; set; }

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "{0} es obligatorio.")]
        public bool Estado { get; set; }


        public ProductoModel()
        {
            this.GravaIVA = true;
            this.Estado = true;
        }

        public ProductoModel(ProductDto entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this.IdProducto = entity.Id;
            this.Nombre = entity.Name;
            this.CodigoPrincipal = entity.MainCode;
            this.CodAuxiliar = entity.AuxCode;
            this.PrecioUnitario = entity.UnitPrice;
            this.GravaIVA = entity.IvaRateId!=1;
            this.Estado = entity.IsEnabled;
        }

        public ProductDto ToEntity()
        {
            return new ProductDto()
            {
                Name = this.Nombre,
                MainCode = this.CodigoPrincipal,
                AuxCode = this.CodAuxiliar,
                UnitPrice = this.PrecioUnitario,
                IvaRateId= this.GravaIVA?1:0,
                IsEnabled = this.Estado,
            };
        }
    }
}