using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System {

    public class MinMaxTime {

        public DateTime StartOfDay { get; set; }

        public DateTime EndOfDay { get; set; }
    }

    public static class DateTimeExtensions {

        /// <summary>
        /// Formata um nullable DateTime.
        /// </summary>
        /// <param name="format">String de Formato de data padrão ou customizado.</param>
        /// <param name="nullReplacement">String a ser utilizada caso o DateTime seja null.</param>
        /// <returns>DateTime formado de acordo com o formato especificado.</returns>
        public static String xToString(this DateTime? d, string format, string nullReplacement) {
            if (d != null) {
                return d.Value.ToString(format);
            }

            return nullReplacement;
        }

        /// <summary>
        /// Formata um nullable DateTime para o formato dd/MM/yyyy.
        /// </summary>
        /// <returns>DateTime formatado como ShortDate</returns>
        public static String xToShortDateString(this DateTime? d) {
            return xToString(d, "dd/MM/yyyy", string.Empty);
        }

        /// <summary>
        /// Formata um nullable DateTime para o formato dd/MM/yyyy.
        /// </summary>
        /// <returns>DateTime formatado como ShortDate</returns>
        public static String xToShortDateStringYYYYMMDD(this DateTime? d)
        {
            return xToString(d, "yyyyMMdd", string.Empty);
        }
        
        /// <summary>
        /// Formata DateTime para o formato dd/MM/yyyy.
        /// </summary>
        /// <returns>DateTime formatado como ShortDate</returns>
        public static String xToShortDateString(this DateTime d) {
            return xToString(d, "dd/MM/yyyy", string.Empty);
        }

        /// <summary>
        /// Formata um nullable DateTime para o formato dd/MM/yyyy hh:mm:ss.
        /// </summary>
        /// <returns>DateTime formatado como ShortDate</returns>
        public static String xToDateTimeString(this DateTime d) {
            return xToString(d, "dd/MM/yyyy hh:mm:ss", string.Empty);
        }

        public static String xToTimeString(this DateTime? d)
        {
            return xToString(d, "hhmmss", string.Empty);
        }

        /// <summary>
        /// Formata um nullable DateTime para o formato dd/MM/yyyy
        /// </summary>
        /// <param name="nullReplacement">String a ser utilizada caso o DateTime seja null.</param>
        /// <returns></returns>
        public static String xToShortDateString(this DateTime? d, string nullReplacement) {
            return xToString(d, "dd/MM/yyyy", nullReplacement);
        }

        /// <summary>
        /// Recupera o dia completo da data ignorando a hora.
        /// </summary>
        public static MinMaxTime xFullDay(this DateTime d) {
            var minMax = new MinMaxTime();
            minMax.StartOfDay = d.Date;
            minMax.EndOfDay = d.Date.AddDays(1).AddMilliseconds(-1);
            return minMax;
        }

        public static DateTime xStartOfDay(this DateTime d) {
            return d.Date;
        }

        public static DateTime xEndOfDay(this DateTime d) {
            return d.Date.AddDays(1).AddMilliseconds(-1);
        }
    }
}