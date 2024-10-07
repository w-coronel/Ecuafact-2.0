using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecuafact.WebAPI.Models
{
    /// <summary>
    /// DOCUMENTO: GUIA DE REMISION
    /// </summary>
    public class ReferralGuideRequestModel : ReferencedDocumentRequestBase
    {
        /// <summary>
        /// ID del Transportista
        /// </summary>
        public long DriverId { get; set; }
        /// <summary>
        /// Codigo del Tipo de Identificacion del Transportista
        /// </summary>
        public string DriverIdentificationType { get; set; }
        /// <summary>
        /// Numero de Identificacion del Transportista
        /// </summary>
        public string DriverIdentification { get; set; }
        /// <summary>
        /// Nombre del Transportista
        /// </summary>
        public string DriverName { get; set; }
        /// <summary>
        /// Numero de Placa del Vehiculo del Transportista
        /// </summary>
        public string CarPlate { get; set; }
        /// <summary>
        /// Inicio del Traslado
        /// </summary>
        public string ShippingStartDate { get; set; }
        /// <summary>
        /// Fin del Traslado
        /// </summary>
        public string ShippingEndDate { get; set; }
        /// <summary>
        /// Direccion del Destinatario
        /// </summary>
        public string OriginAddress { get; set; }
        /// <summary>
        /// Direccion del Destinatario
        /// </summary>
        public string DestinationAddress { get; set; }
        /// <summary>
        /// ID del Destinatario
        /// </summary>
        public long RecipientId { get; set; }
        /// <summary>
        /// Codigo del Tipo de Identificacion del Destinatario
        /// </summary>
        public string RecipientIdentificationType { get; set; }
        /// <summary>
        /// Numero de Identificacion del Destinatario
        /// </summary>
        public string RecipientIdentification { get; set; }
        /// <summary>
        /// Nombre del Destinatario
        /// </summary>
        public string RecipientName { get; set; }
        /// <summary>
        /// Codigo del Establecimiento del Destinatario
        /// </summary>
        public string RecipientEstablishment { get; set; }
        /// <summary>
        /// Numero del Documento Aduanero Unico (DAU) de Ecuador
        /// </summary>
        public string DAU { get; set; }
        /// <summary>
        /// Ruta de Envio
        /// </summary>
        public string ShipmentRoute { get; set; }


        /// <summary>
        /// Detalle de la Guia de Remision
        /// </summary>
        public List<ReferralGuideDetailModel> Details { get; set; }
         
    }



    /// <summary>
    /// Detalle de la Factura
    /// </summary>
    public class ReferralGuideDetailModel
    {
        /// <summary>
        /// Id del Producto
        /// </summary>
        public long ProductId { get; set; }
        
        /// <summary>
        /// Codigo Principal
        /// </summary>
        public string MainCode { get; set; }

        /// <summary>
        /// Codigo Auxiliar
        /// </summary>
        public string AuxCode { get; set; }


        /// <summary>
        /// Descripcion del Producto
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Cantidad
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Detalle 1
        /// </summary>
        public string Name1 { get; set; }

        /// <summary>
        /// Valor 1
        /// </summary>        
        public string Value1 { get; set; }

        /// <summary>
        /// Detalle 2
        /// </summary>       
        public string Name2 { get; set; }

        /// <summary>
        /// Valor 2
        /// </summary>    
        public string Value2 { get; set; }

        /// <summary>
        /// Detalle 3
        /// </summary>       
        public string Name3 { get; set; }

        /// <summary>
        /// Valor 3
        /// </summary>        
        public string Value3 { get; set; }

    }

}
