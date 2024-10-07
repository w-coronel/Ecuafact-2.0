using System;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class EcuafactEnumAttribute : Attribute
    {
        public string CoreValue { get; set; }

        public string DisplayValue { get; set; }

        public string Prefix { get; set; }


        internal EcuafactEnumAttribute(string corevalue, string displayvalue, string prefix = null)
        {
            CoreValue = corevalue;
            DisplayValue = displayvalue;
            Prefix = prefix;
        }

        public static explicit operator short(EcuafactEnumAttribute v)
        {
            throw new NotImplementedException();
        }
    }
}
