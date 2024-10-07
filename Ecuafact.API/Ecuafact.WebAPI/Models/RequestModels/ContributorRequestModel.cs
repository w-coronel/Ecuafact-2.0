using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Models
{
    /// <summary>
    /// Modelo de Datos del Contribuyente
    /// </summary>
    public class ContributorRequestModel
    {
        /// <summary>
        /// Identificacion
        /// </summary>
        [Required(ErrorMessage = "Debe especificar el número de identificación del contribuyente")]
        [MaxLength(50)]
        public string Identification { get; set; }

        /// <summary>
        /// Id del Tipo de Identificacion
        /// </summary>
        [Required]
        public short IdentificationTypeId { get; set; }

        /// <summary>
        /// Razon Social
        /// </summary>
        [MaxLength(300)]
        [Required(ErrorMessage = "Debe especificar la razón social del contribuyente")]
        public string BussinesName { get; set; }

        /// <summary>
        /// Nombre Comercial
        /// </summary>
        [MaxLength(300)]
        public string TradeName { get; set; }

        /// <summary>
        /// Direccion
        /// </summary>
        [MaxLength(300)]
        public string Address { get; set; }

        /// <summary>
        /// Telefono
        /// </summary>
        [MaxLength(300)]
        [RegularExpression(Constants.MultiplePhoneRegex, ErrorMessage = "El numero de teléfono especificado no es válido")]
        public string Phone { get; set; }

        /// <summary>
        /// Correos Electronicos de Contacto
        /// </summary>
        [MaxLength(1500)]
        [RegularExpression(Constants.MultipleEmailRegex, ErrorMessage = "El email especificado no es válido")]
        public string EmailAddresses { get; set; }
         
        /// <summary>
        /// Si esta Activo
        /// </summary>
        public bool IsEnabled { get; set; }

    }


    /// <summary>
    /// Modelo de Datos del Benificiario
    /// </summary>
    public class BeneficiaryRequestModel
    {
        /// <summary>
        /// Codigo del convenio
        /// </summary>
        [Required(ErrorMessage = "Debe especificar el codigo del convenio")]
        [MaxLength(15)]
        public string CodeAgreement { get; set; }

        /// <summary>
        /// Identificacion
        /// </summary>
        [Required(ErrorMessage = "Debe especificar el ruc o cedula del benificiario")]
        [MaxLength(13)]
        public string Identification { get; set; }        

        /// <summary>
        /// Nombre completo
        /// </summary>
        [MaxLength(300)]
        [Required(ErrorMessage = "Debe especificar el nombre completo del benificiario")]
        public string Name { get; set; }

        /// <summary>
        /// Telefono
        /// </summary>
        [MaxLength(50)]
        [RegularExpression(Constants.MultiplePhoneRegex, ErrorMessage = "El numero de teléfono especificado no es válido")]
        public string Phone { get; set; }

        /// <summary>
        /// Correos Electronicos de Contacto
        /// </summary>
        [MaxLength(300)]
        [RegularExpression(Constants.MultipleEmailRegex, ErrorMessage = "El email especificado no es válido")]
        public string Email { get; set; }        

    }

    /// <summary>
    /// Inserción masiva de clientes
    /// </summary>
    public class ContributorImportModel:ImportModel
    {
        
    }

}
