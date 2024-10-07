using Ecuafact.WebAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Ecuafact.WebAPI.Models
{

    /// <summary>
    /// Pago detalle
    /// </summary>
    public class PaymentModel
    {
        /// <summary>
        /// CodigoSRI de Forma de Pago del Catalogo PaymentMethods
        /// </summary>
        [Required]
        public string PaymentMethodCode { get; set; }

        /// <summary>
        /// Nombre de la Forma de Pago del Catalogo PaymentMethods
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Valor total
        /// </summary>
        [Required]
        public decimal Total { get; set; }

        /// <summary>
        /// Plazo
        /// </summary>
        public int Term { get; set; }

        /// <summary>
        /// Unidad de tiempo
        /// </summary>
        public string TimeUnit { get; set; }

    }


    /// <summary>
    /// suscripción
    /// </summary>
    public class SubscriptionRequest
    {
        public long Id { get; set; }
        public string RUC { get; set; }
        public long IssuerId { get; set; }
        public DateTime SubscriptionStartDate { get; set; }
        public DateTime SubscriptionExpirationDate { get; set; }
        public SubscriptionStatusEnum Status { get; set; }
        public string StatusMsg { get; set; }
        public IssuerDto Issuer { get; set; }
        public decimal Price { get; set; }
        public SubscriptionInvoice InvoiceInfo { get; set; }
        public long LicenceTypeId { get; set; }
        public LicenceTypeDto LicenceType { get; set; }
    }

    /// <summary>
    /// suscripción factura
    /// </summary>
    public class SubscriptionInvoice
    {
        public string Identification { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }        
        public string Email { get; set; }        
        public string Phone { get; set; }
        public string DiscountCode { get; set; }
        public string Product { get; set; }
    }


    /// <summary>
    /// suscripción
    /// </summary>
    public class RequestPalnModel
    {
        [Required(ErrorMessage = "El número de identiciacion")]
        public string Identification { get; set; }
        [Required(ErrorMessage = "Nombre del usuario")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Dirección del usuario")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Eamil del usuario")]
        [RegularExpression(Constants.MultipleEmailRegex, ErrorMessage = "Debe ingresar un email válido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Telefóno del usuario")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "El codigo del plan")]
        public string ProductCode { get; set; }
        [Required(ErrorMessage = "El codigo del convenio")]
        public long ReferenceCode { get; set; }
    }

}
