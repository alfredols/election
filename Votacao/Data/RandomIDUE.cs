using System;

namespace Votacao.Data
{
    public class RandomIDUE
    {
        public static string Generate(int length)
        {
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < length; i++)
                s = String.Concat(s, random.Next(1, 9).ToString());
            return s;
        }
    }
}
