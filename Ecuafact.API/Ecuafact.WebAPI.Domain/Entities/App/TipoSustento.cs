
using System.ComponentModel.DataAnnotations;
namespace Ecuafact.WebAPI.Domain.Entities.App
{
    public partial class TipoSustento
    {
        [Required]
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
    }
}
