using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cipher
{     
    class Program
    {
        static public char[] alphabet = { 'а', 'б', 'в', 'г', 'д', 'е', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я' };
        //Фильтрация текста
        static string Filter(string text)
        {   
            string result = "";
            int iterator = 0;
            text = text.ToLower();
            for (int i = 0; i < text.Length; i++)
            {
                for (int j = 0; j < alphabet.Length; j++)
                {
                    if (text[i] == alphabet[j])
                    {
                        result += text[i];
                    }
                    else
                    {
                        iterator++;
                    }

                }
                if (iterator == alphabet.Length)
                {
                    result += "";
                }
                iterator = 0;
            }
            
            return result;
        }
        // Дублируем ключ столько раз, чтобы сравняться с длиной текста
        static string Duplicate(string message, string key)
        {
            string resultKey = "";

            while (resultKey.Length <= message.Length)
            {
                for (int j = 0; j < key.Length; j++)
                {
                    resultKey += key[j];
                    if (resultKey.Length == message.Length)
                    {
                        break;
                    }
                }
                if (resultKey.Length == message.Length)
                {
                    break;
                }
            }
            return resultKey;
        }

        // Каждой букве из текста ставим в соответствие индекс этой буквы в алфавите
        static List<int> letter_to_num(string text)
        {
            List<int> IntFromString = new List<int>();
            for (int i = 0; i < text.Length; i++)
            {
                for (int j = 0; j < alphabet.Length; j++)
                {
                    if (text[i] == alphabet[j])
                    {
                        IntFromString.Add(j);
                    }
                }


            }
            return IntFromString;
        }
        static string num_to_letter(List<int> N)
        {
            string message = "";
            for (int i = 0; i < N.Count; i++)
            {
                for (int j = 0; j < alphabet.Length; j++)
                {
                    if (N[i] == j)
                    {
                        message += alphabet[j];
                    }
                }


            }
            return message;
        }

        static List<int> SumMod(List<int> messageInt, List<int> keyInt, int method)
        {
            List<int> encrypt = new List<int>();
            for (int i = 0; i < keyInt.Count; i++)
            {
                if (method == 1)
                {
                    encrypt.Add((messageInt[i] + keyInt[i]) % alphabet.Length);
                }
                if (method == 2)
                {
                    encrypt.Add(messageInt[i] - keyInt[i]);
                }

                while (encrypt[i] < 0)
                {
                    encrypt[i] += alphabet.Length;
                }
            }
            return encrypt;
        }
        static string Encrypt_Decrypt(string message, string key, int method)
        {

            key = Filter(key);
            string duplicateKey = Duplicate(message, key);
            List<int> encryptInt = SumMod(letter_to_num(message), letter_to_num(duplicateKey), method);
            string encrypt = num_to_letter(encryptInt);
            return encrypt;

        }
        // Количество повторений буквы в тексте
        static int letter_repetition(string text, string letter)
        {
            int count = 0;

            for (int j = 0; j < text.Length; j++)
            {
                if (Convert.ToChar(letter) == text[j])
                {
                    count++;
                }
            }
            return count;
        }
        //Индекс соответствия
        static double Index(string text)
        {
            List<int> ind1 = new List<int>();
            text = Filter(text);
            int count = 0;

            for (int i = 0; i < alphabet.Length; i++)
            {
                for (int j = 0; j < text.Length; j++)
                {
                    if (alphabet[i] == text[j])
                    {
                        count++;
                    }

                }

                if (count > 0)
                {
                    ind1.Add(count * (count - 1));
                    count = 0;
                }
            }
            double index = 0;
            for (int i = 0; i < ind1.Count; i++)
            {
                index += ind1[i];
            }
            index = index / (text.Length * (text.Length - 1));
            return index;
        }

        // Разбиваем шифртекст на n-блоков, где n - длина ключа
        static List<string> SplitToBlocks(string str, int num)
        {
            List<string> newList = new List<string>();
            for (int i = 0; i < num; i++)
            {
                string newStr = "";
                for (int j = 0; j < str.Length; j = j + num)
                {
                    if (!((j + i) > str.Length - 1))
                    {
                        newStr += str[j + i];
                    }
                }

                newList.Add(newStr);
            }

            return newList;
        }

        //Считаем индекс соответствия для каждой группы блоков
        static Dictionary<int, double> Average_Index(string message)
        {
            Dictionary<int, double> result = new Dictionary<int, double>();
            for (int i = 2; i < 31; i++)
            {
                List<string> newList = SplitToBlocks(message, i);
                List<double> index = new List<double>();
                double item = 0;
                foreach (string value in newList)
                {
                    index.Add(Index(value));
                }
                for (int j = 0; j < index.Count; j++)
                {
                    item += index[j];
                }

                item = item / index.Count();
                result.Add(i, item);
            }
            return result;
        }
        // Вероятность появления каждой буквы в языке
        static Dictionary<char, double> Frequency_Char(string content)

        {
            Dictionary<char, double> Freq = new Dictionary<char, double>();
            float count = 0;

            for (int i = 0; i < alphabet.Length; i++)
            {
                for (int j = 0; j < content.Length; j++)
                {
                    if (alphabet[i] == content[j])
                    {
                        count++;
                    }

                }
                count = count / content.Length;
                Freq.Add(alphabet[i], count);
                count = 0;

            }
            ////сортировка словаря по значеню
            //var items = from pair in Freq
            // orderby pair.Value descending
            // select pair;
            ///////////////////////////////////////////

            return Freq;

        }
        // Нахождения ключа 
        static string Find_Key( string message, int probableKey)
        {
            List<string> splitBlocks = SplitToBlocks(message, probableKey);
            string keyResult = "";
            for (int i = 0; i < splitBlocks.Count; i++)
            {
                Dictionary<string, int> result = new Dictionary<string, int>();
                for (int j = 0; j < alphabet.Length; j++)
                {
                    int count = letter_repetition(splitBlocks[i], Convert.ToString(alphabet[j]));
                    if (count > 0)
                    {
                        result.Add(Convert.ToString(alphabet[j]), count);
                    }


                }
                string letterMax = result.FirstOrDefault(x => x.Value == result.Values.Max()).Key;
                Console.Write("какую букву вставить?" + "(" + (i + 1) + "): ");
                string letter = Console.ReadLine();
                int index = Array.IndexOf(alphabet, Convert.ToChar(letterMax)) - Array.IndexOf(alphabet, Convert.ToChar(letter));
                while (index < 0)
                {
                    index += alphabet.Length;
                }
                keyResult += alphabet[index];
                result = new Dictionary<string, int>();
            }
            return keyResult;
        }

        // Нахождение ключа для ШТ через функцию M(g)


        static string Find_Key_Mg(Dictionary<char, double> etalonValue, string message, int probableKey)
        {
            List<string> splitBlocks = SplitToBlocks(message, probableKey);
            string keyResult = "";
            for (int i = 0; i < splitBlocks.Count; i++)
            {
                Dictionary<int, double> resultM = new Dictionary<int, double>();
                double M_g = 0;
                for (int g = 0; g < alphabet.Length; g++)
                {

                    for (int j = 0; j < alphabet.Length; j++)
                    {

                        M_g += etalonValue[alphabet[j]] * letter_repetition(splitBlocks[i], Convert.ToString(alphabet[(j + g) % alphabet.Length]));
                    }
                    resultM.Add(g, M_g);
                    M_g = 0;
                }
                int g_ForMax_Mg = resultM.FirstOrDefault(x => x.Value == resultM.Values.Max()).Key;
                keyResult += Convert.ToString(alphabet[g_ForMax_Mg]);
                resultM = new Dictionary<int, double>();
            }

            return keyResult;

        }


        static void Main(string[] args)
        {


            //Console.Write("Введите что хотите сделать(1 - зашифровать, 2 - дешифровать): ");
            //string method1 = Console.ReadLine(); int method = Convert.ToInt32(method1);
            //Console.Write("Введите текст для обработки: ");
            //string message = Console.ReadLine();
            //message = Filter(message);
            //Console.WriteLine("Индекс открытого текста : " + Index(message));
            //Console.WriteLine();

            //do
            //{
            //    Console.Write("Введите ключ: ");
            //    string key = Console.ReadLine();
            //    string encrypt = Encrypt_Decrypt(message, key, method);
            //    Console.WriteLine("Индекс ШТ  : " + Index(encrypt));
            //    //Console.WriteLine(encrypt);
            //    Console.WriteLine();

            //}
            //while (true);



            ///////////////////////////////////////////////////
            string path = @"C:\Users\ellun\Desktop\Laba2\Cipherq.txt";
            string content = File.ReadAllText(path);
            Dictionary<char, double> etalon_value = Frequency_Char(content);
            //////////////////////////////////////////////////
            //string path1 = @"C:\Users\ellun\Desktop\Laba2\Cipher.txt";
            string path2 = @"C:\Users\ellun\Desktop\Laba2\Cipherq.txt";
            string lubava = File.ReadAllText(path2);
            // string text = File.ReadAllText(path1);
            lubava = Filter(lubava);
            foreach (KeyValuePair<int, double> item in Average_Index(lubava))
            {
                Console.WriteLine(item.Key + " - " + item.Value);
            }
            Console.WriteLine();
            Console.Write("Введите предполагаемую длину ключа: ");
            string method1 = Console.ReadLine(); int probableKey = Convert.ToInt32(method1);
            // string key = Find_Key_Mg(etalon_value, text, probableKey);
            string key_lubava = Find_Key_Mg(etalon_value, lubava, probableKey);
            //string key = Find_Key(text, probableKey);
            //Console.WriteLine("Предполагаемый ключ: " + key);
            Console.WriteLine("Предполагаемый ключ: " + key_lubava);
            Console.WriteLine();
            //Console.WriteLine("Расшифрованний текст: " + Encrypt_Decrypt(text, key, 2));
            Console.WriteLine("Расшифрованний текст: " + Encrypt_Decrypt(lubava, key_lubava, 2));








        }
    }

}
