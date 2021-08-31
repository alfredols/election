
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace System
{
    public static class StringExtensions
    {
        public static string xAppendIfNotNull(this string value, string prePend, string posPend)
        {
            string str = string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                str = string.Format("{0}{1}{2}", prePend, value, posPend);
            };

            return str;
        }

     

        public static string xAsNumber(this string value, string defaultValue = "0")
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var intStr = Regex.Replace(value, @"\D", "");
                return intStr;
            }
            return defaultValue;
        }

        public static string xAsNumberPad(this string value, int totalWidth, char paddingChar)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var intStr = Regex.Replace(value, @"\D", "");
                return intStr.PadLeft(totalWidth, paddingChar);
            }
            return value.PadLeft(totalWidth, paddingChar);
        }

        public static string xClearCPFCNPJ(this string value)
        {
            value = value.Replace(".", "");
            value = value.Replace("-", "");
            value = value.Replace("/", "");
            return value;
        }

        public static string xClearCharacter(this string value)
        {
            value = value.Replace(".", "");
            value = value.Replace("-", "");
            value = value.Replace("/", "");
            value = value.Replace("<", "");
            value = value.Replace(">", "");
            value = value.Replace("(", "");
            value = value.Replace(")", "");
            value = value.Replace("_", "");
            return value;
        }

      

        public static string xFormatCep(this string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var intStr = Regex.Replace(value, @"\D", "0");
                return string.Format("{0:00000-000}", Convert.ToInt32(intStr));
            }
            return null;
        }

     

        public static string xFormatCpfCnpj(this string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var format = string.Empty;
                var intStr = Regex.Replace(value, @"\D", "0");

                if (intStr.Length == 11) {
                    format = string.Format(@"{000\.000\.000\-00}", Convert.ToInt64(intStr));
                } else {
                    format = string.Format(@"{00\.000\.000\/0000\-00}", Convert.ToInt64(intStr));
                }

                return format;
            }
            return null;
        }

        public static string xFormatFone(this string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var format = string.Empty;
                var intStr = Regex.Replace(value, @"\D", "0");

                if (intStr.Length == 10)
                    format = string.Format("{0:(00) 0000-0000}", Convert.ToInt64(intStr));

                if (intStr.Length == 11)
                    format = string.Format("{0:(00) 00000-0000}", Convert.ToInt64(intStr));

                if (intStr.Length > 11)
                    format = string.Format("{0:(00) 0000-0000}", Convert.ToInt64(intStr));

                return format;
            }
            return null;
        }

     
     


        public static string xFormatSufixoCnpj(this string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var intStr = Regex.Replace(value, @"\D", "0");
                return string.Format("{0:0000-00}", Convert.ToInt64(intStr));
            }
            return null;
        }

        public static string xFormatCnpjRaiz(this string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var intStr = Regex.Replace(value, @"\D", "0");
                return string.Format("{0:00.000.000}", Convert.ToInt64(intStr));
            }
            return null;
        }

        public static bool xIsNull(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static string xNormalize(this string value, bool asLowerCase = false)
        {
            if (value != null)
            {
                value = value.Trim();
                value = Regex.Replace(value, @"\s+", " ");
                if (asLowerCase)
                    value = value.ToLower();
            }

            return value;
        }

        public static DateTime? xNullDateTime(this string value)
        {
            if (value != null)
            {
                DateTime dt;

                var valid = DateTime.TryParse(value, out dt);
                if (valid && dt != DateTime.MinValue)
                    return dt;
            }

            return null;
        }

        public static Int16? xNullInt16(this string value)
        {
            short int16 = 0;

            if (!string.IsNullOrWhiteSpace(value))
            {
                var int16str = Regex.Replace(value, @"\D", "");
                Int16.TryParse(int16str, out int16);
                return int16;
            }

            return null;
        }

        public static Int32? xNullInt32(this string value)
        {
            int int32 = 0;

            if (!string.IsNullOrWhiteSpace(value))
            {
                var int32str = Regex.Replace(value, @"\D", "");
                Int32.TryParse(int32str, out int32);
                return int32;
            }

            return null;
        }

        public static Int64? xNullInt64(this string value)
        {
            long int64 = 0;

            if (!string.IsNullOrWhiteSpace(value))
            {
                var int64str = Regex.Replace(value, @"\D", "");
                Int64.TryParse(int64str, out int64);
                return int64;
            }

            return null;
        }

        public static string xNullReplace(this string value, string replace)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return replace;
            }
            return value;
        }

        public static string xPosPendIfNotNull(this string value, string posPend)
        {
            return value.xAppendIfNotNull(null, posPend);
        }

        public static string xPrePendIfNotNull(this string value, string prePend)
        {
            return value.xAppendIfNotNull(prePend, null);
        }

        public static DateTime xSafeDateTime(this string value)
        {
            if (value != null)
            {
                DateTime dt;

                DateTime.TryParse(value, out dt);
                return dt;
            }

            return DateTime.MinValue;
        }

      
        public static Int16 xSafeInt16(this string value)
        {
            short int16 = 0;

            if (!string.IsNullOrWhiteSpace(value))
            {
                var int16str = Regex.Replace(value, @"\D", "");
                Int16.TryParse(int16str, out int16);
            }

            return int16;
        }

        public static Int32 xSafeInt32(this string value)
        {
            int int32 = 0;

            if (!string.IsNullOrWhiteSpace(value))
            {
                var int32str = Regex.Replace(value, @"\D", "");
                Int32.TryParse(int32str, out int32);
            }
            return int32;
        }

        public static Int64 xSafeInt64(this string value)
        {
            long int64 = 0;

            if (!string.IsNullOrWhiteSpace(value))
            {
                var int64str = Regex.Replace(value, @"\D", "");
                Int64.TryParse(int64str, out int64);
            }

            return int64;
        }

        public static string xTitleCase(this string value)
        {
            var name = string.Empty;

            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            name = value.ToLower();

            var wordsQueue = new Queue<string>();
            var lower_case = new string[] { "da", "das", "de", "do", "dos", "e", "di", "ap" };
            var upper_case = new string[] {
                // UF Nacional
                "ac", "al", "am", "ap", "ba", "ce", "df", "es", "go", "ma", "mg", "ms", "mt", "pa", "pb", "pe", "pi", "pr", "rj", "rn", "ro", "rr", "rs", "sc", "se", "sp", "to",
                // Abreviações
                "s/a", "s.a.", "sa", "sa.", "s.a", "ltda", "mei", "me", "cep", "s/c"
            };
            var words_list = name.Split(' ');
            var final_word = string.Empty;

            foreach (var word in words_list)
            {
                if (!string.IsNullOrEmpty(word))
                {
                    var letters = word.ToCharArray();
                    final_word = word;

                    if (lower_case.Any(e => e == word))
                    {
                        final_word = new string(letters).ToLower();
                    }
                    else if (upper_case.Any(e => e == word))
                    {
                        final_word = new string(letters).ToUpper();
                    }
                    else if (!lower_case.Any(e => e == word))
                    {
                        letters[0] = char.ToUpper(letters[0]);
                        final_word = new string(letters);
                    }

                    wordsQueue.Enqueue(final_word);
                }
            }
            return string.Join(" ", wordsQueue);
        }

        public static string xTruncate(this string value, int maxSize)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                maxSize = value.Length < maxSize ? value.Length : maxSize;
                var tmpStr = value.Substring(0, maxSize);
                return tmpStr;
            }
            return value;
        }

        //        function CalculaDigitoMod11(NumDado, NumDig, LimMult, X10)

        //    dim Dado, Mult, Soma, Dig, i, n

        //    if X10 = false then
        //        NumDig = 1
        //    end if
        //    Dado = NumDado
        //    for n = 1 to NumDig
        //        Soma = 0
        //        Mult = 2
        //        for i = Len(Dado) to 1 step -1
        //            Soma = Soma + Mult * CInt(mid(Dado,i,1))
        //            Mult = Mult + 1
        //            if Mult > LimMult then Mult = 2
        //        next
        //        if X10 = true then
        //            Dig = ((Soma * 10) mod 11) mod 10
        //        else
        //            Dig = Soma mod 11
        //            if Dig = 10 then
        //                Dig = "X"
        //            end if
        //        end if
        //        Dado = Dado & CStr(Dig)
        //    next
        //    CalculaDigitoMod11 = right(Dado, NumDig)
        //end function

        /**
         * Retorna o(s) numDig Dígitos de Controle Módulo 11 do
         * dado, limitando o Valor de Multiplicação em limMult,
         * multiplicando a soma por 10, se indicado:
         *
         *    Números Comuns:   numDig:   limMult:   x10:
         *      CPF                2         12      true
         *      CNPJ               2          9      true
         *      PIS,C/C,Age        1          9      true
         *      RG SSP-SP          1          9      false
         *
         * @version                V5.0 - Mai/2001~Out/2015
         * @author                 CJDinfo
         * @param  String  dado    String dado contendo o número (sem o DV)
         * @param  int     numDig  Número de dígitos a calcular
         * @param  int     limMult Limite de multiplicação
         * @param  boolean x10     Se true multiplica soma por 10
         * @return String          Dígitos calculados
         */

        public static String xCalculaDigitoMod11(this string dado, int numDig, int limMult, bool x10)
        {
            int dig;
            int n = 0;
            int soma = 0;
            int mult = 0;
            int tamanho = dado.Length;

            if (!x10) numDig = 1;
            for (n = 1; n <= numDig; n++)
            {
                soma = 0;
                mult = 2;
                for (var i = tamanho - 1; i >= 0; i--)
                {
                    soma += (mult * Convert.ToInt32(dado.Substring(i, 1)));
                    if (++mult > limMult) mult = 2;
                }

                if (x10)
                {
                    dig = ((soma * 10) % 11) % 10;
                }
                else
                {
                    dig = soma % 11;
                    //if (dig == 10) dig = "X";
                }
                dado += (dig);
            }
            return dado.Substring(dado.Length - numDig, numDig);
        }

        public static bool xIsForaMunicipio(this string cep)
        {
            var num = int.Parse(cep.xAsNumber());
            if (!((num >= 1000000 && num <= 5999999) || (num >= 8000000 && num <= 8499999)))
            {
                return true;
            }

            return false;
        }

        public static String xValueForProduction(this string value)
        {
            //#if DEBUG
            return "\n" + value;
            //#else
            //          return "";
            //#endif
        }
        public static bool xValidarCpf(this string cpf)
        {
            string valor = cpf.Replace(".", "");
            valor = valor.Replace("-", "");


            if (valor.Length != 11)
                return false;


            bool igual = true;
            for (int i = 1; i < 11 && igual; i++)
                if (valor[i] != valor[0])
                    igual = false;


            if (igual || valor == "12345678909")
                return false;


            int[] numeros = new int[11];


            for (int i = 0; i < 11; i++)
                numeros[i] = int.Parse(
                  valor[i].ToString());


            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += (10 - i) * numeros[i];


            int resultado = soma % 11;


            if (resultado == 1 || resultado == 0)
            {
                if (numeros[9] != 0)
                    return false;
            }
            else if (numeros[9] != 11 - resultado)
                return false;


            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += (11 - i) * numeros[i];


            resultado = soma % 11;


            if (resultado == 1 || resultado == 0)
            {
                if (numeros[10] != 0)
                    return false;
            }
            else
                if (numeros[10] != 11 - resultado)
                    return false;


            return true;

        }
        public static bool xisNumber(this string value, string defaultValue = "0")
        {
            var intStr = Regex.IsMatch(value, @"^[0-9]*$");
            return intStr;
        }


    }
}