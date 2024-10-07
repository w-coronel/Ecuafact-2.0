using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ecuafact.Web.Domain.Entities
{
    public class Products
    {
        public List<ProductDto> List { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ProductDto
    {
        public ProductDto()
        {
            // Set Default Values
            this.IsEnabled = true;
        }
        public int Id { get; set; }
        public string MainCode { get; set; }
        public string AuxCode { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public int ProductTypeId { get; set; }
        public int IssuerId { get; set; }
        public int IvaRateId { get; set; }
        public int IceRateId { get; set; }
        public bool IsEnabled { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedOn { get; set; }
        public ProductTypeDto ProductType { get; set; }
        public IvaRatesDto IvaRate { get; set; }
        public IceRate IceRate { get; set; }
        public decimal Cost { get; set; }
        public string Name1 { get; set; }
        public string Value1 { get; set; }
        public string Name2 { get; set; }
        public string Value2 { get; set; }
        public string Name3 { get; set; }
        public string Value3 { get; set; }
        public int Status { get; set; }
        public string Tipo { get; set; }
        public string Iva { get; set; }
        public string Ice { get; set; }





    }

    public class ProductImportModel
    {
        /// <summary>
        /// id del usuario
        /// </summary>    
        public long IssuerId { get; set; }

        /// <summary>
        /// Formato del archivo csv, txt, excel
        /// </summary>    
        public string FormatType { get; set; }

        [JsonIgnore]
        public HttpPostedFileBase ImportProductFile { get; set; }

        /// <summary>
        /// Archivo importar 
        /// </summary>   
        [Required]
        public byte[] FileImportRaw => fileimportRaw ?? (fileimportRaw = ImportProductFile.GetBytes());

        private byte[] fileimportRaw;
    }
}