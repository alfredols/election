using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System {

    public static class Int32Extensions {

        public static string xFormatCcm(this Int32? value)
        {
            if (value.HasValue)
                return xFormatCcm(value.Value);

            return null;
        }

        public static string xFormatCcm(this Int32 value)
        {
            return value.ToString(@"0\.000\.000-0");
        }

        public static string xAsInt32Pad(this Int32 value, int totalWidth, char paddingChar) {
            return value.ToString().PadLeft(totalWidth, paddingChar);
        }

        public static int? xAsInt32Null(this Int32 value) {
            if (value == 0)
                return null;

            return value;
        }
    }
}