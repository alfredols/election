using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
    public static class ObjectExtensions
    {
        public static object xValueForName(this object src, string name)
        {
            var prop = src.GetType().GetProperty(name,
                        BindingFlags.IgnoreCase |
                        BindingFlags.Public |
                        BindingFlags.Instance
            );

            if (prop != null) {
                return prop.GetValue(src, null);
            }

            var field = src.GetType().GetField(name,
                        BindingFlags.IgnoreCase |
                        BindingFlags.Public |
                        BindingFlags.Instance
            );

            if (field != null) {
                return field.GetValue(src);
            }

            return "";            
        }
    }
}
