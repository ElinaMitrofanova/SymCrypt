using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Affine_cipher
{
    class Program
    {
        static public char[] alphabet = { 'а', 'б', 'в', 'г', 'д', 'е', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я' };
        static string Filter(string text)
        {
                string pattern = @"[^а-я]";
                text = Regex.Replace(text, pattern, "");
                return text;
        }
        // Разбиваем текст на биграммы
            static Dictionary<string, double> Bigram(string text)
        {
            Dictionary<string, double> freq = new Dictionary<string, double>();
            for (int i = 1; i < text.Length; i = i + 2)
            {
                string bigramm = text[i - 1].ToString() + text[i].ToString();
                double count = 0;
                freq.TryGetValue(bigramm, out count);
                freq[bigramm] = (count + 1);

            }
            Dictionary<string, double> frequency = freq.ToDictionary
                (item => item.Key, item => item.Value / (text.Length / 2));

            var items = from pair in frequency
                        orderby pair.Value descending
                        select pair;
            /////////////////////////////////////////
            Dictionary<string, double> sortedFreq = new Dictionary<string, double>();
            foreach (KeyValuePair<string, double> pair in items)
            {
                sortedFreq.Add(pair.Key, pair.Value);
            }
            return sortedFreq;

        }
        static List<int> Num (string text)
        {
           
            List<int> Y = new List<int>();
            Dictionary<string, double> sortedFreq = Bigram(text);
            foreach (string item in sortedFreq.Keys)
            {
                int x = 0;
                int y = 0;
                for (int i = 0; i < alphabet.Length; i++)
                {
                    if (item[0] == alphabet[i])
                    {
                        x = i;
                    }
                    if (item[1] == alphabet[i])
                    {
                        y = i;
                    }
                }
                Y.Add(x * alphabet.Length - y);
            }

            return Y;
        }
        // составление системы сравнений
        


        static int GCD(int a ,int b,out int u, out int v)
        {
            if (a == 0)
            {
                u = 0;
                v = 1;
                return b;
            }
            int u1, v1;
            int d = GCD(b % a, a, out u1, out v1);
            u = v1 - (b / a) * u1;
            v = u1;

            return d;
           
        }
    
        static void Main(string[] args)
        {
            string path = @"C:\Users\ellun\Desktop\LAB3\TEXT.txt";
            string textwithspaces = File.ReadAllText(path);//шт
            string text = Filter(textwithspaces);
           
        }
    }
}
