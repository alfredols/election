using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System {

    public static class StringTemplateExtensions {

        public static string xLoadTemplate(this string any, string resourceLocation) {
            var assembly = Assembly.GetExecutingAssembly();
            var resource = assembly.GetManifestResourceStream(resourceLocation);
            if (resource == null) {
                var msg = string.Format("Requested Resource [{0}] not found in [{1}].", resourceLocation, assembly.FullName);
                throw new InvalidOperationException(msg);
            }

            using (var stream = new StreamReader(resource)) {
                var contents = stream.ReadToEnd();
                return contents;
            }
        }

        public static string xFormat(this string format, params object[] args) {
            return string.Format(format, args);
        }

        public static string xFormatWith(this string format, object source) {
            if (format == null)
                return format;

            if (source == null)
                return format;

            var outString = format;

            try {
                var propertyNamesAndValues = source.GetType().GetProperties()
                    .Where(pi => pi.GetGetMethod() != null)
                    .Select(pi => new {
                        pi.Name,
                        Value = pi.GetGetMethod().Invoke(source, null)
                    });

                propertyNamesAndValues.ToList()
                    .ForEach(p => {
                        outString = outString
                            .Replace("{" + p.Name + "}", p.Value != null ? p.Value.ToString() : String.Empty, StringComparison.OrdinalIgnoreCase);
                    });
            }
            catch (Exception ex) {
                var msg = ex.Message;
            }

            return outString;
        }

        public static string Replace(this string source, string oldString, string newString, StringComparison stringComparison) {
            int index = source.IndexOf(oldString, stringComparison);

            // Determine if we found a match
            bool found = index >= 0;

            if (found) {
                // Remove the old text
                source = source.Remove(index, oldString.Length);

                // Add the replacemenet text
                source = source.Insert(index, newString);
            }

            return source;
        }
    }
}