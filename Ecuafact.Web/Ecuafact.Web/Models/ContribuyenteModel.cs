using System;
using System.ComponentModel.DataAnnotations;

namespace Ecuafact.Web.Models
{
    public abstract class ContribuyenteBaseModel
    {
        /************//*Por comprobar*/
        [Display(Name = "Id")]
        public long Id { get; set; }
        /************/

        [Display(Name = "Tipo de Identificación")]
        [Required(ErrorMessage = "{0} es obligatorio.")]
        public short TipoIdentificacionId { get; set; }

        [Display(Name = "Identificación")]
        [Required(ErrorMessage = "{0} es obligatoria.")]
        [StringLength(13, ErrorMessage = "{0} debe contener entre {2} y {1} caracteres.", MinimumLength = 10)]
        public string Identificacion { get; set; }

        [Display(Name = "Razón Social")]
        [Required(ErrorMessage = "{0} es obligatoria.")]
        [StringLength(100, ErrorMessage = "{0} debe contener máximo {1} caracteres.")]
        public string RazonSocial { get; set; }

        [Display(Name = "Dirección")]
        [StringLength(100, ErrorMessage = "{0} debe contener máximo {1} caracteres.")]
        public string Direccion { get; set; }

        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "{0} es obligatorio.")]
        [RegularExpression(RegularExpressionPatterns.TelefonoPattern, ErrorMessage = @"{0} puede contener números y/o guiones.")]
        public string Telefono { get; set; }

        public string NumeroDocumento { get; set; }
        public string ClaveAcesso { get; set; }
        public string NumeroAutorizacion { get; set; }
        public string FechaEmision { get; set; }

        public long IdEmisor { get; set; }
        public string Codigo { get; set; }
        [NonSerialized()]
        public bool __Selected = false;
        


        protected string CombinarEmail(params string[] emails)
        {
            var emailCombinado = "";

            if ((emails != null) && (emails.Length > 0))
            {
                foreach (var email in emails)
                {
                    if (!String.IsNullOrWhiteSpace(email))
                    {
                        if (emailCombinado.Length > 0)
                        {
                            emailCombinado += ";";
                        }

                        emailCombinado += email.Trim();
                    }
                }
            }

            return emailCombinado;
        }

        protected string[] SepararEmails(string emailCombinado)
        {
            var emails = new string[3];

            if (!String.IsNullOrWhiteSpace(emailCombinado))
            {
                var emailsToken = emailCombinado.Split(new[] { ';' }, 3);

                for (var i = 0; i < emailsToken.Length; i++)
                {
                    emails[i] = emailsToken[i];
                }
            }

            return emails;
        }

    }

    public class ContribuyenteModel : ContribuyenteBaseModel
    {
        [Display(Name = "Código")]
        public int? IdContribuyente { get; set; }

        [Display(Name = "Nombre Comercial")]
        [Required(ErrorMessage = "{0} es obligatorio.")]
        [StringLength(200, ErrorMessage = "{0} debe contener máximo {1} caracteres.")]
        public string NombreComercial { get; set; }

        [Display(Name = "Email 1")]
        [Required(ErrorMessage = "{0} es obligatorio.")]
        [RegularExpression(RegularExpressionPatterns.CorreoElectronicoPattern, ErrorMessage = "{0} no es un correo electrónico válido.")]
        public string Email1 { get; set; }

        [Display(Name = "Email 2")]
        [RegularExpression(RegularExpressionPatterns.CorreoElectronicoPattern, ErrorMessage = "{0} no es un correo electrónico válido.")]
        public string Email2 { get; set; }

        [Display(Name = "Email 3")]
        [RegularExpression(RegularExpressionPatterns.CorreoElectronicoPattern, ErrorMessage = "{0} no es un correo electrónico válido.")]
        public string Email3 { get; set; }

        [Display(Name = "Tipo de Contribuyente")]
        public short TipoContribuyente { get; set; }

        [Display(Name = "Es Cliente")]
        public bool EsCliente
        {
            get
            {
                return TipoContribuyente == 1;
            }
        }

        [Display(Name = "Es Proveedor")]
        public bool EsProveedor {
            get
            {
                return TipoContribuyente == 2;
            }
        }

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "{0} es obligatorio.")]
        public bool Estado { get; set; }

        public string GetEmail()
        {
            return CombinarEmail(new string[] { Email1, Email2, Email3 });
        }

        public void SetEmail(string emailAddresses)
        {
            string[] emailList  = SepararEmails(emailAddresses);
            if (emailList.Length > 0 && !string.IsNullOrEmpty(emailList[0]))
            {
                Email1 = emailList[0];
            }
            if (emailList.Length > 1 && !string.IsNullOrEmpty(emailList[1]))
            {
                Email1 = emailList[1];
            }
            if (emailList.Length > 2 && !string.IsNullOrEmpty(emailList[2]))
            {
                Email1 = emailList[2];
            }
        }

        public ContribuyenteModel()
        {
            this.TipoContribuyente=1;
            this.Estado = true;
        }

        public ContribuyenteModel(ContribuyenteModel entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this.IdContribuyente = Convert.ToInt32(entity.Id);
            this.Identificacion = entity.Identificacion;
            this.RazonSocial = entity.RazonSocial;
            this.NombreComercial = entity.NombreComercial;
            this.TipoContribuyente = entity.TipoContribuyente;
            this.Email1 = entity.Email1;
            this.Email2 = entity.Email2;
            this.Email3 = entity.Email3;
            this.Estado = entity.Estado;
            this.Telefono = entity.Telefono;
            this.Direccion = entity.Direccion;
            this.TipoIdentificacionId = 1;/* (int)EnumExtensions.GetEnumValue<TipoIdentificacion>(
                 entity.TipoIdentificacion, TipoIdentificacion.Pasaporte);*/
        }

        public ContribuyenteModel ToEntity()
        {
            return new ContribuyenteModel()
            {
                TipoIdentificacionId = this.TipoIdentificacionId,
                Identificacion = this.Identificacion,
                RazonSocial = this.RazonSocial,
                NombreComercial = this.NombreComercial,
                Direccion = this.Direccion,
                Telefono = this.Telefono,
                Email1 = this.Email1,
                Email2 = this.Email2,
                Email3 = this.Email3,
                TipoContribuyente = this.TipoContribuyente,
                Estado = this.Estado,
            };
        }

    }

    public class CompradorModel : ContribuyenteBaseModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "{0} es obligatorio.")]
        [RegularExpression(RegularExpressionPatterns.CorreoElectronicoListaPattern, ErrorMessage = "{0} no es un correo electrónico válido.")]
        public string Email { get; set; }

        public CompradorModel()
        {
        }

        public CompradorModel(ContribuyenteModel entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this.Identificacion = entity.Identificacion;
            this.RazonSocial = entity.RazonSocial;
            this.Email = this.CombinarEmail(entity.Email1, entity.Email2, entity.Email3);
            this.Telefono = entity.Telefono;
            this.Direccion = entity.Direccion;
            this.TipoIdentificacionId = 1;/* (int)EnumExtensions.GetEnumValue<TipoIdentificacion>(
                 entity.TipoIdentificacion, TipoIdentificacion.Pasaporte);*/
        }

        public ContribuyenteModel ToEntity()
        {
            var emails = this.SepararEmails(this.Email);

            return new ContribuyenteModel()
            {
                TipoIdentificacionId = this.TipoIdentificacionId,
                Identificacion = this.Identificacion,
                RazonSocial = this.RazonSocial,
                NombreComercial = "",
                Direccion = this.Direccion,
                Telefono = this.Telefono,
                Email1 = emails[0],
                Email2 = emails[1],
                Email3 = emails[2],
                TipoContribuyente = 1,
                Estado = true,
            };
        }
    }
}
