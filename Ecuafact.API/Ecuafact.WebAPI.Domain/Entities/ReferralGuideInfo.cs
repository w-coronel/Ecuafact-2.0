using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    /// <summary>
    /// Informacion de la Guia de Remision
    /// </summary>
    public class ReferralGuideInfo
    {
        /// <summary>
        /// Guia de Remision
        /// </summary>
        [ForeignKey("Document")]
        public long ReferralGuideInfoId { get; set; }
        public string IssuedOn { get; set; }
        public long? ContributorId { get; set; }
        public string IdentificationType { get; set; }
        public string Identification { get; set; }
        public string BusinessName { get; set; }
        public string SenderAddress { get; set; }
        public string OriginAddress { get; set; }
        public string DestinationAddress { get; set; }
        public long DriverId { get; set; }
        public string DriverIdentificationType { get; set; }
        public string DriverIdentification { get; set; }
        public string DriverName { get; set; }
        public string CarPlate { get; set; }
        public string ShippingStartDate { get; set; }
        public string ShippingEndDate { get; set; }
        public long RecipientId { get; set; }
        public string RecipientIdentificationType { get; set; }
        public string RecipientIdentification { get; set; }
        public string RecipientName { get; set; }
        public string RecipientAddress { get; set; }
        public string Reason { get; set; }
        public string DAU { get; set; }
        public string RecipientEstablishment { get; set; }
        public string ShipmentRoute { get; set; }
        /// <summary>
        /// ID del Documento Referencia (OPCIONAL)
        /// </summary>
        public long? ReferenceDocumentId { get; set; }
        /// <summary>
        /// Codigo del Tipo de Documento Referencia  (REQUERIDO)
        /// </summary>
        public string ReferenceDocumentCode { get; set; }
        /// <summary>
        /// Numero del Documento Referencia  (REQUERIDO)
        /// </summary>
        public string ReferenceDocumentNumber { get; set; }
        /// <summary>
        /// Fecha de Emision del Documento Referencia  (REQUERIDO)
        /// </summary>
        public string ReferenceDocumentDate { get; set; }

        /// <summary>
        /// Numero de Autorizacion del Documento Referencia (OPCIONAL)
        /// </summary>
        public string ReferenceDocumentAuth { get; set; }

        public List<ReferralGuideDetail> Details { get; set; } = new List<ReferralGuideDetail>();

        [JsonIgnore]
        public Document Document { get; set; }

    }
}
