using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Models
{
    /// <summary>
    /// Datos del Contribuyente
    /// </summary>
    public class ContributorDto
    {
        /// <summary>
        /// ID del Registro
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Numero de Identificacion
        /// </summary>
        public string Identification { get; set; }
        /// <summary>
        /// Tipo de Identificacion
        /// </summary>
        public short IdentificationTypeId { get; set; }

        /// <summary>
        /// Tipo de Identificacion
        /// </summary>
        public string IdentificationType { get; set; }

        /// <summary>
        /// Tipo de Identificacion
        /// </summary>
        public string IdentificationTypeCode { get; set; }

        /// <summary>
        /// Nombre Comercial
        /// </summary>
        public string BussinesName { get; set; }
        /// <summary>
        /// Razon Social
        /// </summary>
        public string TradeName { get; set; }
        /// <summary>
        /// Direccion
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Telefono
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Direcciones de correo electronico
        /// </summary>
        public string EmailAddresses { get; set; } 
        /// <summary>
        /// Emisor
        /// </summary>
        public long IssuerId { get; set; }
        /// <summary>
        /// Si el contribuyente Es Proveedor
        /// </summary>
        public bool IsSupplier { get; set; }
        /// <summary>
        /// Si el contribuyente Es Cliente
        /// </summary>
        public bool IsCustomer { get; set; }
        /// <summary>
        /// Si el contribuyente Es Transportista
        /// </summary>
        public bool IsDriver { get; set; }
        /// <summary>
        /// Si el registro esta habilitado y visible
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// Fecha de creacion del registro
        /// </summary>
        public string CreatedOn { get; set; }
        /// <summary>
        /// Fecha de la ultima modificacion del registro
        /// </summary>
        public string LastModifiedOn { get; set; }
    }

    public class ContributorViewModel : BaseModel
    {
        public List<ContributorDto> Contributor { get; set; }
    }
}
