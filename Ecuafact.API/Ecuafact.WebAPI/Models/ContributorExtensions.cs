using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using AutoMapper;
using Ecuafact.WebAPI.Domain.Entities;

namespace Ecuafact.WebAPI.Models
{
    public static class ContributorExtensions
    {
        internal static MapperConfiguration ContributorMapper =>
            new MapperConfiguration(config =>
            {
                config.CreateMap<DateTime, string>().ConvertUsing(dt => dt.ToString("yyyy-MM-dd"));

                config.CreateMap<Contributor, ContributorDto>()
                    .ForMember(member => member.IdentificationType, opt => opt
                          .MapFrom(model => model.IdentificationType.Name))
                    .ForMember(member => member.IdentificationTypeCode, opt => opt
                          .MapFrom(model => model.IdentificationType.SriCode));

                
                config.CreateMap<ContributorRequestModel, Contributor>()
                    .ForMember(member => member.BussinesName, opt => opt
                    .MapFrom(model => string.IsNullOrEmpty(model.BussinesName) ? model.TradeName : model.BussinesName));
            });



        
        internal static Contributor ToContributor(this ContributorRequestModel request)
        {
            var result = ContributorMapper
                .CreateMapper()
                .Map<Contributor>(request);

            result.CreatedOn = DateTime.Now;

            return result;
        }

        internal static Contributor ToContributor(this ContributorRequestModel request, Contributor model)
        {
            model = ContributorMapper
                .CreateMapper()
                .Map(request, model);

            model.LastModifiedOn = DateTime.Now;

            return model;
        }

        internal static ContributorDto ToContributorDto(this Contributor contributor)
        {
            var contributorDto = ContributorMapper
                .CreateMapper()
                .Map<ContributorDto>(contributor);

            if (contributor.IdentificationType != null)
            {
                contributorDto.IdentificationType = contributor.IdentificationType.Name;
                contributorDto.IdentificationTypeCode = contributor.IdentificationType.SriCode;
            }

            return contributorDto;
        }

        internal static List<Contributor> ImportContributor(this ContributorImportModel contributor)
        {
            var lst = new List<Contributor>();
            try
            {
                if (contributor.FormatType == "csv")
                {
                    Stream arch = new MemoryStream(contributor.FileImportRaw);
                    using (BufferedStream bs = new BufferedStream(arch))
                    {
                        using (StreamReader sr = new StreamReader(bs))
                        {
                            int i = 0;

                            while (!sr.EndOfStream)
                            {
                                var line = sr.ReadLine();
                                var value = line.Split(';');
                                if (i > 0)
                                {
                                    var perNatural = Convert.ToString(value[2]);
                                    var perJuridica = Convert.ToString(value[3]);
                                    var tradeName = Convert.ToString(value[4]);
                                    var bussinesName = "";
                                    if (!String.IsNullOrEmpty(perNatural)) { bussinesName = perNatural; }
                                    if (!String.IsNullOrEmpty(perJuridica)) { bussinesName = perJuridica; }
                                    lst.Add(new Contributor
                                    {
                                        Identification = (Convert.ToString(value[0]).Length < 10 && Convert.ToInt16(int.TryParse(value[1], out i) ? value[1] : "2") == 1) ?  Convert.ToString(value[0]).PadLeft(10, '0') : Convert.ToString(value[0]),
                                        IdentificationTypeId = Convert.ToInt16(int.TryParse(value[1], out i) ? value[1] : "2"),
                                        BussinesName = bussinesName,
                                        TradeName = tradeName,
                                        Address = Convert.ToString(value[5]),
                                        Phone = Convert.ToString(value[6]),
                                        EmailAddresses = Convert.ToString(value[7]),
                                        IssuerId = contributor.IssuerId,
                                        IsSupplier = false,
                                        IsCustomer = true,
                                        IsDriver = false,
                                        IsEnabled = true, 
                                        CreatedOn = DateTime.Now,
                                    });
                                }
                                i++;
                            }
                        }
                    }

                }
                else
                {
                    lst = MySystemExtensions.ImportData<Contributor>(new MemoryStream(contributor.FileImportRaw));
                    lst?.ForEach(x => x.IssuerId = contributor.IssuerId);
                }
            }
            catch (Exception ex)
            {
                var errors = ex.Message;
            }

            return lst;
        }

    }

    /// <summary>
    /// Tipo de Contribuyente
    /// </summary>
    public enum ContributorTypeEnum
    {
        /// <summary>
        /// Todos
        /// </summary>
        All,
        /// <summary>
        /// Cliente
        /// </summary>
        Customer,
        /// <summary>
        /// Proveedor
        /// </summary>
        Supplier,
        /// <summary>
        /// Chofer o Transportista
        /// </summary>
        Driver
    }
}