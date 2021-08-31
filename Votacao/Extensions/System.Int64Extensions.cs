using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;



namespace System {

    public static class Int64Extensions {


        public static string    xFormatCnpj(this Int64 value)  {
            return value.ToString(@"00\.000\.000\/0000\-00");
        }

        public static string    xFormatCnpj(this Int64? value) {
            if (value.HasValue)
                return value.Value.ToString(@"00\.000\.000\/0000\-00");

            return null;
        }

        public static string    xFormatCpf(this Int64 value)   {
            return value.ToString(@"000\.000\.000\-00");
        }

        public static string    xFormatCpf(this Int64? value)  {
            if (value.HasValue)
                return value.Value.ToString(@"000\.000\.000\-00");

            return null;
        }

        public static string xFormatCnae(this Int64? value)
        {
            if (value.HasValue)
                return value.Value.ToString(@"0000\-0\/00");

            return null;
        }

        public static string xFormatSufixoCnpj(this Int64? value)
        {
            if (value.HasValue)
                return xFormatSufixoCnpj(value.Value);

            return null;
        }

        public static string xFormatSufixoCnpj(this Int64 value)
        {
            return value.ToString(@"0000\-00");            
        }

        public static string xFormatRaizCnpj(this Int64? value)
        {
            if (value.HasValue)
                return xFormatRaizCnpj(value.Value);

            return null;
        }

        public static string xFormatRaizCnpj(this Int64 value)
        {
            return value.ToString(@"00\.000\.000");            
        }

        public static string    xAsInt64Pad(this Int64 value, int totalWidth, char paddingChar)  {
            return value.ToString().PadLeft(totalWidth, paddingChar);
        }

        public static string    xAsInt64Pad(this Int64? value, int totalWidth, char paddingChar) {
            if (value.HasValue) {
                return value.ToString().PadLeft(totalWidth, paddingChar);
            }

            return null;
        }

        public static string    xAsInt32Pad(this Int32? value, int totalWidth, char paddingChar) {
            if (value.HasValue) {
                return value.ToString().PadLeft(totalWidth, paddingChar);
            }

            return null;
        }

     

        public static bool      xIsValidCpf(this Int64 value)  {
            if (value == 0) {
                return false;
            }

            var input = value.ToString();
            var matchA = Regex.IsMatch(input, @"^\d{3,11}$", RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
            var matchB = Regex.IsMatch(input, @"^\d{3}\.\d{3}\.\d{3}-\d{2}$", RegexOptions.ECMAScript | RegexOptions.IgnoreCase);

            if (!matchA && !matchB) {
                return false;
            }
            else if (!ValidarDigitosCpf(input)) {
                return false;
            }

            return true;
        }

        public static bool      xIsValidCnpj(this Int64 value) {
            if (value == 0) {
                return false;
            }

            var input = value.ToString();
            var matchA = Regex.IsMatch(input, @"^\d{3,14}$", RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
            var matchB = Regex.IsMatch(input, @"^\d{2}\.\d{3}\.\d{3}\/\d{4}-\d{2}$", RegexOptions.ECMAScript | RegexOptions.IgnoreCase);

            if (!matchA && !matchB) {
                return false;
            }
            else if (!ValidarDigitosCnpj(input)) {
                return false;
            }

            return true;
        }

        #region -- Helper Methods -----------------------------------------------------------------

        /// <summary>
        /// Valida a representação <see cref="String"/> do número de CPF especificado.
        /// </summary>
        /// <returns>true, se o CPF especificado for válido; false, caso contrário.</returns>
        private static bool ValidarDigitosCpf(string cpf) {
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = string.Empty;
            string digito = string.Empty;

            int soma = 0;
            int resto = 0;

            // Deixa apenas os números do CPF
            cpf = string.IsNullOrWhiteSpace(cpf) ? string.Empty : Regex.Replace(cpf, @"\D", "");
            cpf = cpf.PadLeft(11, '0');

            tempCpf = cpf.Substring(0, 9);

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            resto = soma % 11;

            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();

            tempCpf = tempCpf + digito;

            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;

            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cpf.EndsWith(digito);
        }

        /// <summary>
        /// Valida a representação <see cref="String"/> do número de CNPJ especificado.
        /// </summary>
        /// <returns>true, se o CNPJ especificado for válido; false, caso contrário.</returns>
        private static bool ValidarDigitosCnpj(string cnpj) {
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            string digito;
            string tempCnpj;

            // Deixa apenas os números do CNPJ
            cnpj = string.IsNullOrWhiteSpace(cnpj) ? string.Empty : Regex.Replace(cnpj, @"\D", "");
            cnpj = cnpj.PadLeft(14, '0');

            if (Convert.ToInt64(cnpj) == 0)
                return false;

            if (cnpj.Length != 14)
                return false;

            tempCnpj = cnpj.Substring(0, 12);

            soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();

            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cnpj.EndsWith(digito);
        }

        #endregion -- Helper Methods -----------------------------------------------------------------
    }
}