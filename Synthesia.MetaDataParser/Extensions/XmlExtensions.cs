using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Synthesia.MetaDataParser.Extensions
{
    public static class XmlExtensions
    {
        public static string AttributeOrDefault(this XElement e, string attributeName)
        {
            return AttributeOrDefault(e, attributeName, null);
        }

        public static string AttributeOrDefault(this XElement e, string attributeName, string defaultValue)
        {
            if (e == null) throw new InvalidOperationException("Passed-in element is null.");

            XAttribute a = e.Attribute(attributeName);

            if (a == null)
                return defaultValue;

            return a.Value;
        }

        public static void SetAttributeValueAndRemoveEmpty(this XElement element, XName name, object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                var a = element.Attribute(name);
                if (a != null) a.Remove();
            }
            else element.SetAttributeValue(name, value);
        }
    }
}
