using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Ecuafact.WebAPI.Models
{
    internal static class CatalogsExtensions
    {
        internal static IdentificationTypesDto ToIdentificationTypesDto(this Domain.Entities.IdentificationType idtype)
        {
            return new IdentificationTypesDto { Id = idtype.Id, Name = idtype.Name, SriCode = idtype.SriCode };
        }

        internal static PaymentMethodDto ToPaymentMethodDto(this Domain.Entities.PaymentMethod paymentMethod)
        {
            return new PaymentMethodDto { Id = paymentMethod.Id, Name = paymentMethod.Name, SriCode = paymentMethod.SriCode };
        }

        internal static VatRatesDto ToVatRatesDto(this Domain.Entities.VatRate vat)
        {
            return new VatRatesDto { Id = vat.Id, Name = vat.Name, SriCode = vat.SriCode, RateValue = vat.RateValue };
        }

        internal static TaxTypeDto ToTaxTypeDto(this Domain.Entities.TaxType tax)
        {
            return new TaxTypeDto { Id = tax.Id, Name = tax.Name, SriCode = tax.SriCode };
        }

        internal static DocumentTypeDto ToDocumentTypeDto(this Domain.Entities.DocumentType docType)
        {
            return new DocumentTypeDto { Id = docType.Id, Name = docType.Name, SriCode = docType.SriCode };
        }

        internal static IceRateDto ToIceRateDto(this Domain.Entities.IceRate iceRate)
        {
            return new IceRateDto { Id = iceRate.Id, Name = iceRate.Name, SriCode = iceRate.SriCode, Rate = iceRate.Rate, EspecificRate = iceRate.EspecificRate, EspecificRateDescription = iceRate.EspecificRateDescription };
        }

        internal static ProductTypeDto ToProductTypeDto(this Domain.Entities.ProductType proType)
        {
            return new ProductTypeDto {Id = proType.Id, Name = proType.Name };
        }

        internal static ContributorTypeDto ToContributorTypeDto(this Domain.Entities.ContributorType contribType)
        {
            return new ContributorTypeDto {Id = contribType.Id, Name = contribType.Name};
        }

        internal static IdentificationSupplierTypeDto ToIdentificationSupplierTypeDto(this Domain.Entities.IdentificationSupplierType indeType)
        {
            return new IdentificationSupplierTypeDto { Id = indeType.Id, SriCode= indeType.SriCode, Name = indeType.Name };
        }

        internal static BeneficiaryDto ToBeneficiaryDto(this Domain.Entities.Beneficiary BeneficiaryDto)
        {
            var mapper = new MapperConfiguration(config => config.CreateMap<Domain.Entities.Beneficiary, BeneficiaryDto>()).CreateMapper();
            var result = mapper.Map<BeneficiaryDto>(BeneficiaryDto);
            return result;
        }

        internal static Domain.Entities.Beneficiary ToBeneficiary(this BeneficiaryRequestModel request, Domain.Entities.ReferenceCodes code)
        {
            var result = new Domain.Entities.Beneficiary()
            {
                Identification = request.Identification,
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Status = Domain.Entities.BeneficiaryStatusEnum.Activo,
                IsEnabled = true,
                CreatedOn = DateTime.Now,
                AgreementId = code.Agreement.Id,
                StatusMsg = "Benificiario activo",
                BeneficiaryReferenceCode = new Domain.Entities.BeneficiaryReferenceCode()
                {
                    Identification = request.Identification,
                    DiscountCode = $"{code.Code.Trim()}-{request.Identification.Trim()}",
                    ReferenceCodId = code.Id,
                    Status = 0,
                    CreatedOn = DateTime.Now                   
                }
            };

            return result;
        }


        internal static SubscriptionDto ToSubscriptionDto(this Domain.Entities.Subscription subscription)
        {
            var mapper = new MapperConfiguration(config => config.CreateMap<Domain.Entities.Subscription, SubscriptionDto>()).CreateMapper();
            var result = mapper.Map<SubscriptionDto>(subscription);                      
            return result;
        }

        internal static LicenceTypeDto ToLicenceTypeDto(this Domain.Entities.LicenceType plans)
        {
            var mapper = new MapperConfiguration(config => config.CreateMap<Domain.Entities.LicenceType, LicenceTypeDto>()).CreateMapper();
            var result = mapper.Map<LicenceTypeDto>(plans);           
            return result;
        }

        internal static LicenceTypeModel ToLicenceTypeModel(this Domain.Entities.LicenceType plans)
        {
            var mapper = new MapperConfiguration(config => config.CreateMap<Domain.Entities.LicenceType, LicenceTypeModel>()).CreateMapper();
            var result = mapper.Map<LicenceTypeModel>(plans);
            return result;
        }

        internal static SupportTypeDto ToSupportTypeDto(this Domain.Entities.SupportType support)
        {
            var mapper = new MapperConfiguration(config => config.CreateMap<Domain.Entities.SupportType, SupportTypeDto>()).CreateMapper();
            var result = mapper.Map<SupportTypeDto>(support);
            return result;
        }

        internal static Domain.Entities.UserCampaigns ToUserCampaigns(this UserCampaignsRequest request)
        {            
            return new Domain.Entities.UserCampaigns() {
                Identification = request.Identification,
                Name = request.Name,
                Phone = request.Phone,
                Email = request.Email,
                UserId = request.UserId,
                MediaCampaigns = new Domain.Entities.MediaCampaigns() { 
                    utm_source = request.UTMS.source,
                    utm_medium = request.UTMS.medium,
                    utm_campaign = request.UTMS.campaign
                }
            } ;
        }

        internal static NotificationDto ToNotification(this Domain.Entities.Notification request)
        {
            return new NotificationDto
            {
                Code = request.Code,
                Message = request.Message ??  "",
                NameImage = request.NameImage ?? "",
                NotificationType = request.NotificationType,
                Title = request.Title,
                UrlImage = request.UrlImage
            };
        }

    }
}