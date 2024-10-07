using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Ecuafact.WebAPI.Domain.Entities;

namespace Ecuafact.WebAPI.Models
{
    public static class ProductRequestModelExtensions
    {
        internal static Product ToProduct(this ProductRequestModel requestModel)
        {
            var conf = new MapperConfiguration(config => config.CreateMap<ProductRequestModel, Product>());
            var mapper = conf.CreateMapper();
            return mapper.Map<Product>(requestModel); 
        }

        internal static Product ToProduct(this ProductRequestModel requestModel, Product existingProduct)
        {
            existingProduct.Name = requestModel.Name;
            existingProduct.MainCode = requestModel.MainCode;
            existingProduct.AuxCode = requestModel.AuxCode;
            existingProduct.ProductTypeId = requestModel.ProductTypeId;
            existingProduct.IvaRateId = requestModel.IvaRateId;
            existingProduct.IceRateId = requestModel.IceRateId;
            existingProduct.UnitPrice = requestModel.UnitPrice;
            existingProduct.IsEnabled = requestModel.IsEnabled;
            existingProduct.LastModifiedOn = DateTime.Now;
            existingProduct.Name1 = requestModel.Name1 ?? "";
            existingProduct.Value1 = requestModel.Value1 ?? "";
            existingProduct.Name2 = requestModel.Name2 ?? "";
            existingProduct.Value2 = requestModel.Value2 ?? "";
            existingProduct.Name3 = requestModel.Name3 ?? "";
            existingProduct.Value3 = requestModel.Value3 ?? "";

            return existingProduct;
        }
    }
}