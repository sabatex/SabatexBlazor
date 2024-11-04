using System.Text;
using System.Collections.Specialized;
using System;
using System.Globalization;
using System.Collections.Generic;

namespace sabatex.Extensions.ClassExtensions
{
    public static class StringExtension
    {
        public static string Parse(this string value, char delimiter)
        {
            if (value.Trim().Length == 0) return "";
            string result = value.Substring(0, value.IndexOf(delimiter)).Trim();
            return result;
        }
        public static string Parse(this string value)
        {
            // Parse on default delimiter
            return value.Parse(';');
        }
        public static bool CheckString(this string value, string check)
        {
            foreach (char c in value)
            {
                if (check.IndexOf(c) == -1) return false;
            }
            return true;
        }
        /// <summary>
        /// Разделение строки на подстроки с разделителем delimiter 
        /// </summary>
        /// <param name="str">Строка с разделителями </param>
        /// <param name="delimiter">Символ разделитель</param>
        /// <returns>Коллекция строк</returns>
        public static StringCollection ToStringCollection(this string str, char delimiter = ';')
        {
            StringCollection result = new StringCollection();
            int i = 0;
            while (i < str.Length)
            {
                int pos = str.IndexOf(delimiter, i);
                if (pos == -1)                                          // End
                {
                    // last substring
                    result.Add(str.Substring(i).Trim());
                    return result;
                }
                result.Add(str.Substring(i, pos - i).Trim());
                i = pos + 1;
            }
            return result;
        }

        public static string ToRussian(this string value)
        {
            StringBuilder s = new StringBuilder(value);
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = s[i].UpperKeyToRus();
            }
            return s.ToString();
        }
        public static string ToUkrainian(this string value)
        {
            StringBuilder s = new StringBuilder(value);
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = s[i].UpperKeyToUkraine();
            }
            return s.ToString();
        }


        public static string ToHeximal(this string value)
        {
            StringBuilder result = new StringBuilder();
            if (value != null)
            {
                foreach (char c in value)
                {
                    result.Append(((UInt16)c).ToString());
                    result.Append(',');
                }
            }
            return result.ToString();

        }

        public static string FromHeximal(this string value)
        {
            StringBuilder result = new StringBuilder();
            if (value != null)
            {
                var st = value.ToStringCollection(',');
                foreach (string s in st)
                {
                    if (UInt16.TryParse(s, out ushort rs))
                        result.Append((char)rs);
                    else
                        throw new Exception("System Error !!!");
                }
            }
            return result.ToString();

        }

        /// <summary>
        /// Use . or , for decimal separator
        /// </summary>
        /// <param name="value">string with decimal simbols</param>
        /// <param name="result">decimal value</param>
        /// <returns>true is succes</returns>
        public static bool TryToDecimal(this string value, out decimal result)
        {
            if (Decimal.TryParse(value, out result)) return true;
            if (Decimal.TryParse(value.Replace('.', ','), out result)) return true;
            return false;

        }
        static readonly string[] ukrToLatinChars = new string[]
        {
            "A",//А
            "B",//Б
            "V",//В
            "H",//Г
            "D",//Д
            "E",//Е
            "Zh",//Ж
            "Z",//З
            "Y",//И
            "I",//Й - last position
            "K",//K
            "L",//Л
            "M",//M
            "N",//H
            "O",//O
            "P",//П
            "R",//Р
            "S",//С
            "T",//T
            "U",//У
            "F",//Ф
            "Kh",//Х
            "Ts",//Ц
            "Ch",//Ч
            "Sh",//Ш
            "Shch",//Щ
            "",//Ъ
            "Y",//Ы
            "",//Ь
            "Е",//Э
            "Iu",//Ю Yu
            "Ia",//Я Ya
            "a",//А
            "b",//Б
            "v",//В
            "h",//Г
            "d",//Д
            "e",//Е
            "zh",//Ж
            "z",//З
            "y",//И
            "i",//Й - last position
            "k",//K
            "l",//Л
            "m",//M
            "n",//H
            "o",//O
            "p",//П
            "r",//Р
            "s",//С
            "t",//T
            "u",//У
            "f",//Ф
            "kh",//Х
            "ts",//Ц
            "ch",//Ч
            "sh",//Ш
            "shch",//Щ
            "",//Ъ
            "y",//Ы
            "",//Ь
            "e",//Э
            "iu",//Ю Yu
            "ia",//Я Ya
            };
        static readonly string[] ukrToLatinCharsFirst = new string[]
        {
            "A",//А
            "B",//Б
            "V",//В
            "H",//Г
            "D",//Д
            "E",//Е
            "Zh",//Ж
            "Z",//З
            "Y",//И
            "Y",//Й 
            "K",//K
            "L",//Л
            "M",//M
            "N",//H
            "O",//O
            "P",//П
            "R",//Р
            "S",//С
            "T",//T
            "U",//У
            "F",//Ф
            "Kh",//Х
            "Ts",//Ц
            "Ch",//Ч
            "Sh",//Ш
            "Shch",//Щ
            "",//Ъ
            "Y",//Ы
            "",//Ь
            "Е",//Э
            "Yu",//Ю Yu
            "Ya",//Я Ya
            "a",//А
            "b",//Б
            "v",//В
            "h",//Г
            "d",//Д
            "e",//Е
            "zh",//Ж
            "z",//З
            "y",//И
            "y",//Й
            "k",//K
            "l",//Л
            "m",//M
            "n",//H
            "o",//O
            "p",//П
            "r",//Р
            "s",//С
            "t",//T
            "u",//У
            "f",//Ф
            "kh",//Х
            "ts",//Ц
            "ch",//Ч
            "sh",//Ш
            "shch",//Щ
            "",//Ъ
            "y",//Ы
            "",//Ь
            "e",//Э
            "yu",//Ю Yu
            "ya",//Я Ya
        };


        public static string TranslitFromUkraineToLatin(this string value)
        {
            
            var result = new StringBuilder();
            int i = 0;
            bool isFirst = true;
            string getFromTable(char c, int lastIndex)
                {
                    int index = (int)c - (int)'А';
                    string s = isFirst ? ukrToLatinCharsFirst[index] : ukrToLatinChars[index];
                    if (Char.IsLower(c)) return s;
                    if (s.Length == 1) return s;
                    if (lastIndex >= value.Length) return s.ToUpper(); // upper last simbol  
                    if (char.IsUpper(value[lastIndex])) return s.ToUpper(); // last simbol is upper
                    return s;
                }

            while (i < value.Length)
            {
                char c = value[i];
                i++;
                if (c == 'З')
                {
                        if (i<value.Length){
                            if (value[i] == 'Г')
                            {
                                result.Append("ZGh");
                                i++;
                                isFirst = false;
                                continue;
                            }
                            if (value[i] == 'г')
                            {
                                result.Append("Zgh");
                                i++;
                                isFirst = false;
                                continue;
                            }

                        }
                }   
                if (c == 'з')
                {
                        if (i<value.Length){
                            if (value[i] == 'Г')
                            {
                                result.Append("zGh");
                                i++;
                                isFirst = false;
                                continue;
                            }
                            if (value[i] == 'г')
                            {
                                result.Append("zgh");
                                i++;
                                isFirst = false;
                                continue;
                            }
                        }
                }   



                switch (c)
                {
                    case '’':
                        continue;
                    case ' ':
                        result.Append(c);
                        isFirst = true;
                        continue;
                    case 'Ґ':
                        result.Append('G');
                        break;
                    case 'ґ':
                        result.Append('g');
                        break;
                    case 'Є':
                        result.Append(isFirst ? "Ye" : "Ie");
                        break;
                    case 'є':
                        result.Append(isFirst ? "ye" : "ie");
                        break;
                    case 'Ї':
                        result.Append(isFirst ? "Yi" : "I");
                        break;
                    case 'ї':
                        result.Append(isFirst ? "yi" : "i");
                        break;
                    case 'І':
                        result.Append('I');
                        break;
                    case 'і':
                        result.Append('i');
                        break;
                       
                    default:
                        if (c >= 'А' && c <= 'я')
                            result.Append(getFromTable(c, i));
                        else 
                           result.Append(c);
                        break;
                }
                isFirst = false;
            }
            return result.ToString();
        }
            
    }
}
