using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Models
{
    public class ProductDto
    {
        public long Id { get; set; }
        public string MainCode { get; set; }
        public string AuxCode { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public short ProductTypeId { get; set; }
        public long IssuerId { get; set; }
        public short IvaRateId { get; set; }
        public short IceRateId { get; set; }
        public bool IsEnabled { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedOn { get; set; }
        public string Name1 { get; set; }
        public string Value1 { get; set; }
        public string Name2 { get; set; }
        public string Value2 { get; set; }
        public string Name3 { get; set; }
        public string Value3 { get; set; }
    }
}
