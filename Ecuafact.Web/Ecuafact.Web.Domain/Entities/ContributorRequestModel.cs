using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Ecuafact.Web.Domain.Entities
{
    public class ContributorRequestModel
    {
        public long Id { get; set; }

        /// <summary>
        /// Identificacion
        /// </summary>
        [Required(ErrorMessage = "Debe especificar el número de identificación del contribuyente")]
        [MaxLength(50)]
        public string Identification { get; set; }

        /// <summary>
        /// Id del Tipo de Identificacion
        /// </summary>
        [Required(ErrorMessage = "Debe especificar el tipo de identidad del contribuyente")]
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
        [MaxLength(500)]
        [Required(ErrorMessage = "Debe especificar la dirección del contribuyente")]
        public string Address { get; set; }

        /// <summary>
        /// Telefono
        /// </summary>
        [MaxLength(500)]
        [RegularExpression(Constants.MultiplePhoneRegex, ErrorMessage = "El numero de teléfono especificado no es válido")]
        public string Phone { get; set; }

        /// <summary>
        /// Correos Electronicos de Contacto
        /// </summary>
        [MaxLength(500)]
        [RegularExpression(Constants.MultipleEmailRegex, ErrorMessage = "El email especificado no es válido")]
        public string EmailAddresses { get; set; }

        /// <summary>
        /// Id del Tipo de Contribuyente
        /// </summary>
        [Required]
        public short ContributorTypeId { get; set; }
        
        /// <summary>
        /// Si esta Activo
        /// </summary>
        public bool IsEnabled { get; set; }

    }

    public class ContributorImportModel
    {
        /// <summary>
        /// id del usuario
        /// </summary>    
        public long IssuerId { get; set; }

        /// <summary>
        /// Formato del archivo csv, txt, excel
        /// </summary>    
        public string FormatType { get; set; }

        [JsonIgnore]
        public HttpPostedFileBase ImportContributorFile { get; set; }

        /// <summary>
        /// Archivo importar 
        /// </summary>   
        [Required]
        public byte[] FileImportRaw => fileimportRaw ?? (fileimportRaw = ImportContributorFile.GetBytes());

        private byte[] fileimportRaw;
    }
}
