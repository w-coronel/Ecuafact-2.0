using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Ecuafact.WebAPI.Domain.Entities;

namespace Ecuafact.WebAPI.Models.Dtos
{
    /// <summary>
    /// COMPROBANTE DE RETENCION
    /// </summary>
    public class RetentionDto
    {
        /// <summary>
        /// Identificador Unico del Documento
        /// </summary>
        [Required]
        public long Id { get; set; }

        /// <summary>
        /// Registro Unico de Contribuyente
        /// </summary>

        [MaxLength(13)]
        [Required]
        public string RUC { get; set; }

        /// <summary>
        /// Código del Documento a Emitir (Tabla 3 Ficha Tecnica)
        /// </summary>
        [StringLength(2)]
        public string DocumentTypeCode { get; set; }

        /// <summary>
        /// Codigo Establecimiento
        /// </summary>
        [StringLength(5)]
        public string EstablishmentCode { get; set; }

        /// <summary>
        /// Punto de Emisión del Comprobante
        /// </summary>
        [StringLength(5)]
        public string IssuePointCode { get; set; }

        /// <summary>
        /// Secuencial
        /// </summary>
        [StringLength(9)]
        public string Sequential { get; set; }


        /// <summary>
        /// Emails, Emails para enviar el comprobante.
        /// </summary>
        [StringLength(500)]
        public string Emails { get; set; }

        /// <summary>
        /// Fecha de creacion del documento
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Razon Social
        /// </summary>
        [MaxLength(300)]
        [Required]
        public string BussinesName { get; set; }

        /// <summary>
        /// Nombre Comercial
        /// </summary>
        [MaxLength(300)]
        public string TradeName { get; set; }
        /// <summary>
        /// Direccion Principal
        /// </summary>
        [MaxLength(300)]
        public string MainAddress { get; set; }
        /// <summary>
        /// Estado Habilitado del Documento
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// Informacion de la Factura
        /// </summary>
        public RetentionInfo RetentionInfo { get; set; }

        /// <summary>
        /// Campos Adicionales
        /// </summary>
        public List<AdditionalField> AdditionalFields { get; set; }

        /// <summary>
        /// Id del Emisor
        /// </summary>
        [Required]
        public long IssuerId { get; set; }

        /// <summary>
        /// Clave de Acceso
        /// </summary>
        public string AccessKey { get; set; }
        /// <summary>
        /// Fecha de autorizacion
        /// </summary>
        public string AuthorizationDate { get; set; }
        /// <summary>
        /// Numero de Autorizacion
        /// </summary>
        public string AuthorizationNumber { get; set; }
        /// <summary>
        /// PDF
        /// </summary>
        public string PDF { get; set; }
        /// <summary>
        /// XML
        /// </summary>
        public string XML { get; set; }
        /// <summary>
        /// Estado actual del documento
        /// </summary>
        public DocumentStatusEnum? Status { get; set; }
        /// <summary>
        /// Mensaje del Estado
        /// </summary>
        public string StatusMsg { get; set; }

        /// <summary>
        /// Fecha de Modificacion
        /// </summary>
        public DateTime? LastModifiedOn { get; set; }

        /// <summary>
        /// Numero del Documento
        /// </summary>
        [NotMapped]
        public string DocumentNumber
        {
            get
            {
                return $"{EstablishmentCode}-{IssuePointCode}-{Sequential}";
            }
            set { } // do nothing x) por si acaso
        }

        public DateTime IssuedOn { get; set; }
        public long ContributorId { get; set; }
        public string ContributorIdentificationType { get; set; }
        public string ContributorName { get; set; }
        public string ContributorIdentification { get; set; }
        public string ContributorAddress { get; set; }
        public string Currency { get; set; }
        public string Reason { get; set; }
        public decimal Total { get; set; }
    }
}