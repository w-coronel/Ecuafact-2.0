using System.ComponentModel.DataAnnotations;

namespace Ecuafact.Web.Models
{
    public class UsuarioModel
    {
        [Display(Name = "Id")]
        public long Id { get; set; }
        [Display(Name = "Usuario")]
        public string Nombre { get; set; }
        [Display(Name = "Asignado")]
        public bool IsChecked { get; set; }
    }
}