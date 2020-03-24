using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using OfficeOpenXml;
using System.Text.RegularExpressions;

namespace Sym.Cripta
{
    class Program
    {
        static public char[] alphabet = { 'а', 'б', 'в', 'г', 'д', 'е', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я', ' ' };
        static string Filter(string text, string condition)
        {
            text = text.ToLower();
            if (condition == "withoutSpaces")
            {
                string pattern = @"[^а-я]";
                text = Regex.Replace(text, pattern, "");
            }
            if (condition == "withSpaces")
            {
                string pattern = @"[^\sа-я]";
                text = Regex.Replace(text, pattern, "");
            }
            return text;
        }

        ///Подсчет частоты для символов

        static Dictionary<string, double> Frequency_Char(string text)

        {
            Dictionary<string, double> freq = new Dictionary<string, double>();
            for (int i = 0; i < text.Length; i++)
            {
                string symbol = text[i].ToString();
                double count = 0;
                freq.TryGetValue(symbol, out count);
                freq[symbol] = (count + 1);

            }
            Dictionary<string, double> frequency = freq.ToDictionary
                (item => item.Key, item => item.Value / (text.Length));

            ///сортировка словаря по значеню//////////
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
        // Подсчет прямых биграмм
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
            return frequency;
        }
        // Подсчет пересекающихся и прямых
        static Dictionary<string, double> BigramX(string text)

        {
            Dictionary<string, double> freq = new Dictionary<string, double>();
            for (int i = 1; i < text.Length; i++)
            {
                string bigramm = text[i - 1].ToString() + text[i].ToString();
                double count = 0;
                freq.TryGetValue(bigramm, out count);
                freq[bigramm] = (count + 1);

            }
            Dictionary<string, double> frequency = freq.ToDictionary
                (item => item.Key, item => item.Value / (text.Length - 1));
            return frequency;
        }

        // Энтропия для символов
        static double Entropy(Dictionary<string, double> frequency)
        {
            double H1 = 0;
            foreach (double item in frequency.Values)
            {
                H1 -= item * Math.Log(item, 2);
            }

            return H1;
        }
        // Энтропия для биграмм
        static double Entropy_Bigram(Dictionary<string, double> frequency)
        {
            double H2 = 0;
            foreach (double item in frequency.Values)
            {
                H2 -= (item * Math.Log(item, 2));
            }
            return H2 / 2;
        }
        static void Exel(Dictionary<string, double> freq2, string filename)
        {
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("Sheet1");


                FileInfo excelFile = new FileInfo(@"C:\Users\ellun\Desktop\Laba2\" + filename);
                for (int i = 0; i < alphabet.Length; i++)
                {
                    workSheet.Cells[1, i + 2].Value = alphabet[i].ToString();
                    workSheet.Cells[i + 2, 1].Value = alphabet[i].ToString();
                }
                foreach (KeyValuePair<string, double> pair in freq2)
                {
                    string key = pair.Key;


                    for (int i = 0; i < alphabet.Length; i++)
                    {
                        for (int j = 0; j < alphabet.Length; j++)
                        {
                            if (workSheet.Cells[1, i + 2].Value.ToString().Contains(key[0].ToString()) &
                                   workSheet.Cells[j + 2, 1].Value.ToString().Contains(key[1].ToString()))
                            {
                                workSheet.Cells[i + 2, j + 2].Value = pair.Value;
                            }

                        }
                    }
                }
                FileInfo fi = new FileInfo(@"C:\Users\ellun\Desktop\Laba2\" + filename);
                excelPackage.SaveAs(fi);
            }

        }
        static void Main(string[] args)
        {
            string path = @"C:\Users\ellun\Desktop\Laba2\TEXT.txt";// текст 1-2 мб, для которого считается єнтропия 
            string content = File.ReadAllText(path);
            string textWithSpaces = Filter(content, "withSpaces");
            string textWithoutSpaces = Filter(content, "withoutSpaces");


            string pathSave1 = @"C:\Users\ellun\Desktop\Laba2\Символы с пробелом.txt";
            string pathSave2 = @"C:\Users\ellun\Desktop\Laba2\Символы без пробела.txt";
            Dictionary<string, double> Freq1 = Frequency_Char(textWithoutSpaces);
            Dictionary<string, double> Freq2 = Frequency_Char(textWithSpaces);
            foreach (KeyValuePair<string, double> pair in Freq1)
            {
                System.IO.File.AppendAllText(pathSave2, string.Format("{0} {1} {2}",
                pair.Key, pair.Value.ToString(), Environment.NewLine));
            }
            foreach (KeyValuePair<string, double> pair in Freq2)
            {
                System.IO.File.AppendAllText(pathSave1, string.Format("{0} {1} {2}",
                pair.Key, pair.Value.ToString(), Environment.NewLine));
            }

            double entropySymbols = Entropy(Freq2);
            Console.WriteLine(" H1 для текста с пробелами: " + entropySymbols);
            double entropySymbols1 = Entropy(Freq1);
            Console.WriteLine(" H1 для текста без пробелов: " + entropySymbols1);
            Console.WriteLine();
            ////Прямые биграммы
            Dictionary<string, double> straightBigrams1 = Bigram(textWithSpaces);
            Dictionary<string, double> straightBigrams2 = Bigram(textWithoutSpaces);

            double H2_1 = Entropy_Bigram(straightBigrams1);
            Console.WriteLine("Энтропия для прямых биграмм : ");
            Console.WriteLine("  c пробелами : " + H2_1);
            double H2_2 = Entropy_Bigram(straightBigrams2);
            Console.WriteLine("  без пробелов : " + H2_2);
            Console.WriteLine();
            // Пересекающиеся 
            Dictionary<string, double> freq1 = BigramX(textWithSpaces);
            Dictionary<string, double> freq2 = BigramX(textWithoutSpaces);
            double H2_3 = Entropy_Bigram(freq1);
            Console.WriteLine("Энтропия для пересекающихся биграмм : ");
            Console.WriteLine("  c пробелами : " + H2_3);
            double H2_4 = Entropy_Bigram(freq2);
            Console.WriteLine("  без пробелов : " + H2_4);

            Exel(straightBigrams1, "Прямые с пробелом.xlsx");
            Exel(straightBigrams1, "Прямые без пробела.xlsx");
            Exel(freq1, " Все с пробелом.xlsx ");
            Exel(freq2, " Все без пробелов.xlsx ");








        }
    }
}
