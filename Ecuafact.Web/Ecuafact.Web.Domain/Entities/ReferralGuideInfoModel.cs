using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities
{
    /// <summary>
    /// Informacion de la Guia de Remision
    /// </summary>
    public class ReferralGuideInfoModel
    {
        /// <summary>
        /// Guia de Remision
        /// </summary>
        public long ReferralGuideInfoId { get; set; }
        public string IssuedOn { get; set; }
        public long ContributorId { get; set; }
        public string IdentificationType { get; set; }
        public string Identification { get; set; }
        public string BusinessName { get; set; }
        public string SenderAddress { get; set; }
        public string Currency { get; set; }
        public string OriginAddress { get; set; }
        public string DestinationAddress { get; set; }

        public long DriverId { get; set; }
        public string DriverIdentificationType { get; set; }
        public string DriverIdentification { get; set; }
        public string DriverName { get; set; }

        public string CarPlate { get; set; }

        public string ShippingStartDate { get; set; }
        public string ShippingEndDate { get; set; }
        public string ShippingAddress { get; set; }

        public long RecipientId { get; set; }
        public string RecipientIdentificationType { get; set; }
        public string RecipientIdentification { get; set; }
        public string RecipientName { get; set; }
        public string RecipientAddress { get; set; }

        public string Reason { get; set; }
        public string DAU { get; set; }

        public string RecipientEstablishment { get; set; }

        public string ShipmentRoute { get; set; }

        public long? ReferenceDocumentId { get; set; }
        public string ReferenceDocumentCode { get; set; }
        public string ReferenceDocumentNumber { get; set; }
        public string ReferenceDocumentDate { get; set; }
        public string ReferenceDocumentAuth { get; set; }

        public List<ReferralGuideDetailModel> Details { get; set; } 

    }
}
