using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using AutoMapper;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain;


namespace Ecuafact.WebAPI.Models
{
    public static class ProductExtensions
    {
        internal static ProductDto ToProductDto(this Product product)
        {
            var conf = new MapperConfiguration(config =>
            {
                config.CreateMap<Product, ProductDto>();
                config.CreateMap<DateTime, string>().ConvertUsing(dt => dt.ToString("yyyy-MM-dd"));
            });

            var mapper = conf.CreateMapper();
            var productDto = mapper.Map<ProductDto>(product);
            return productDto;
            
            //return new ProductDto
            //{
            //    Name = product.Name,
            //    MainCode = product.MainCode,
            //    AuxCode = product.AuxCode,
            //    UnitPrice = product.UnitPrice,
            //    IceRateId = product.IceRateId,
            //    IvaRateId = product.IvaRateId,
            //    IssuerId = product.IssuerId,
            //    IsEnabled = product.IsEnabled,
            //    ProductTypeId = product.ProductTypeId,
            //    CreatedOn = product.CreatedOn.ToString("yyyy-MM-dd"),
            //    LastModifiedOn = product.LastModifiedOn?.ToString("yyyy-MM-dd"),
            //    Id = product.Id
            //};
        }

        internal static List<Product> ImportProduct(this ProductImportModel product, List<VatRate> ivaRates)
        {
            var lst = new List<Product>();
            try
            {
                if (product.FormatType == "csv")
                {
                    Stream arch = new MemoryStream(product.FileImportRaw);
                    using (BufferedStream bs = new BufferedStream(arch))
                    {
                        using (StreamReader sr = new StreamReader(bs))
                        {
                            int i = 0;

                            while (!sr.EndOfStream)
                            {
                                var line = sr.ReadLine();
                                var value = line.Split(';');
                                var ivaCode = int.TryParse(value[5], out i) ? value[5] : "0";
                                var ivaRateId = ivaRates.Where(cod => cod.SriCode == ivaCode).FirstOrDefault().Id;
                                if (i > 0)
                                {
                                    lst.Add(new Product
                                    {
                                        MainCode = Convert.ToString(value[0].Trim()),
                                        AuxCode = Convert.ToString(value[1].Trim()),
                                        Name = Convert.ToString(value[2].Trim()),
                                        UnitPrice = Convert.ToDecimal(value[3]),
                                        ProductTypeId = Convert.ToInt16(int.TryParse(value[4], out i) ? value[4] : "1"),
                                        IvaRateId = ivaRateId,
                                        IceRateId = Convert.ToInt16(1),
                                        IsEnabled = true,
                                        Name1 = Convert.ToString(value[6].Trim()),
                                        Value1 = Convert.ToString(value[7].Trim()),
                                        Name2 = Convert.ToString(value[8].Trim()),
                                        Value2 = Convert.ToString(value[9].Trim()),
                                        IssuerId = product.IssuerId,
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
                    lst = MySystemExtensions.ImportData<Product>(new MemoryStream(product.FileImportRaw), ivaRates);
                    lst?.ForEach(x => x.IssuerId = product.IssuerId);
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
    /// Tipo de Producto
    /// </summary>
    public enum ProductTypeEnum : short
    {
        /// <summary>
        /// Todos
        /// </summary>
        All = 0,
        /// <summary>
        /// Bienes
        /// </summary>
        Goods = 1,
        /// <summary>
        /// Servicios
        /// </summary>
        Services = 2
    }

}