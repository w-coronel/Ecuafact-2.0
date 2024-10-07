using System;

namespace Ecuafact.Web.Domain.Entities
{
    public class EcuafactEnumAttribute : Attribute
    {
        public string CoreValue { get; set; }

        public string DisplayValue { get; set; }

        public string Style { get; set; }

        internal EcuafactEnumAttribute(string corevalue, string displayvalue, string style = "default")
        {
            CoreValue = corevalue;
            DisplayValue = displayvalue;
            Style = style;
        }
    }
}
