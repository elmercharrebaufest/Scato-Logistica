using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Molinos.Scato.Dominio.Helpers
{

    public static class ExtensionesEnum
    {
        public static string DisplayEnum<TEnum>(this TEnum enumValue) where TEnum : struct
        {
            //You can't use a type constraints on the special class Enum. So I use this workaround
            if (!typeof (TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum must be of type System.Enum");
            }

            Type type = typeof (TEnum);
            MemberInfo[] memberInfo = type.GetMember(enumValue.ToString());
            if (memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof (DisplayAttribute), false);
                if (attrs.Length > 0)
                {
                    return ((DisplayAttribute) attrs[0]).GetName();
                }
            }
            return enumValue.ToString();
        }

        public static string Text(this Enum enumValue)
        {
            string displayText = enumValue.ToString();

            var field = enumValue.GetType().GetField(displayText);

            if (field == null)
            {
                return string.Empty;
            }

            var attributes = (DisplayAttribute[])field.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attributes.Length > 0)
            {
                var resourceType = attributes[0].ResourceType;
                var property = attributes[0].Name ?? attributes[0].Description;
                if (resourceType != null)
                {
                    displayText = resourceType.GetProperty(property).GetValue(resourceType, null).ToString();
                }
                else
                {
                    displayText = property;
                }
            }
            return displayText;
        }
    }
}