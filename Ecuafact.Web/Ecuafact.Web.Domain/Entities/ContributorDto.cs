using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Web;

namespace Ecuafact.Web.Domain.Entities
{
    public class Contributors
    {
        public List<ContributorDto> Listado { get; set; }
    }
    public class ContributorDto
    {
        public ContributorDto()
            :this(false)
        {

        }

        public ContributorDto(bool consumidorFinal)
        {
            if (consumidorFinal)
            {
                Id = 0;
                Identification = "9999999999";
                BussinesName = TradeName = "Consumidor Final";
                EmailAddresses = "facturacion@ecuafact.com";
            }
            else
            {
                EmailAddresses = "";
            }


            /// Set DEFAULT VALUES
            IdentificationTypeId = 1;
            Address = "";
            Phone = "";
            ContributorTypeId = 1;
            IsCustomer = true;
            IsEnabled = true;
            CreatedOn = DateTime.Today.ToString("yyyy-MM-dd");
        }

        public long Id { get; set; }

        [Required(ErrorMessage = "Debe especificar el número de identificación del contribuyente")]
        public string Identification { get; set; }

        [Required(ErrorMessage = "Debe especificar el tipo de identidad del contribuyente")]
        public short IdentificationTypeId { get; set; }

        public string IdentificationType { get; set; }

        [Required(ErrorMessage = "Debe especificar la razón social del contribuyente")]
        public string BussinesName { get; set; }
        
        public string TradeName { get; set; }
        
        [Required(ErrorMessage = "Debe especificar la dirección del contribuyente")]
        public string Address { get; set; }
        
        [RegularExpression(Constants.MultiplePhoneRegex, ErrorMessage = "El numero de teléfono especificado no es válido")] 
        public string Phone { get; set; }
        
        [RegularExpression(Constants.MultipleEmailRegex, ErrorMessage = "El email especificado no es válido")]
        public string EmailAddresses { get; set; }
        
        public short ContributorTypeId { get; set; }
        
        public string CreatedOn { get; set; }
        
        public string LastModifiedOn { get; set; }
        
        public bool IsEnabled { get; set; }

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
    }

    public static class ContributorDtoExtensions
    {
        public static ContributorDto ToContributor(this DocumentRequestBase request)
        {
            if (request.ContributorId == 0)
            {
                return new ContributorDto(true);
            }

            return new ContributorDto
            {
                Id = request.ContributorId ?? 0,
                BussinesName = request.ContributorName,
                Address = request.Address,
                Identification = request.Identification,
                IdentificationType = request.IdentificationType,
                TradeName = request.ContributorName,
                EmailAddresses = request.EmailAddresses,
                Phone = request.Phone,
                ContributorTypeId = 1
            };
        }

        public static ContributorDto ToDriver(this ReferralGuideRequestModel request)
        {
            if (request.DriverId == 0)
            {
                return new ContributorDto(true);
            }

            return new ContributorDto
            {
                Id = request.DriverId,
                BussinesName = request.DriverName,               
                Identification = request.DriverIdentification,
                IdentificationType = request.DriverIdentificationType,
                TradeName = request.DriverName,               
                ContributorTypeId = 1
            };
        }

        public static ContributorDto ToRecipient(this ReferralGuideRequestModel request)
        {
            if (request.RecipientId == 0)
            {
                return new ContributorDto(true);
            }

            return new ContributorDto
            {
                Id = request.RecipientId,
                BussinesName = request.RecipientName,               
                Identification = request.RecipientIdentification,
                IdentificationType = request.RecipientIdentificationType,
                TradeName = request.RecipientName,
                ContributorTypeId = 1
            };
        }
    }
}