using System;
using System.Linq;

namespace Ecuafact.Web.Domain.Entities
{
    public static class EnumExtensions
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum value)
        where TAttribute : Attribute
        {
            var type = value?.GetType() ?? typeof(TAttribute);
            var name = Enum.GetName(type, value);

            if (!string.IsNullOrEmpty(name))
            {
                return type.GetField(name)?
                   .GetCustomAttributes(false)?
                   .OfType<TAttribute>()?
                   .SingleOrDefault();
            }

            return null;    
        }
         
        public static string GetStyle(this Enum value)
        {
            return value.GetAttribute<EcuafactEnumAttribute>()?.Style;
        }

        public static string GetValorCore(this Enum value)
        {
            return value.GetAttribute<EcuafactEnumAttribute>()?.CoreValue;
        }

        public static string GetDisplayValue(this Enum value)
        {
            return value.GetAttribute<EcuafactEnumAttribute>()?.DisplayValue;
        }


    }
}
