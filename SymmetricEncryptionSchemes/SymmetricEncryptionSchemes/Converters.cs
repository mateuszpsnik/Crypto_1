using System;
using System.Collections.Generic;
using System.Text;

namespace Crypto1
{
    public static class Converters
    {
        public static List<string> To64BitStrings(string[] binaryStrings)
        {
            List<string> blocks64bit = new List<string>();
            int iterator = 0;
            int k = 0;
            while (iterator < binaryStrings.Length)
            {
                string block64bit = "";

                for (int j = k; j < k + 8; j++)
                {
                    if (iterator < binaryStrings.Length)
                        block64bit += binaryStrings[j];
                    iterator++;
                }

                blocks64bit.Add(block64bit);
                k += 8;
            }
            return blocks64bit;
        }

        public static List<string> To8BitBinaryStrings(List<string> listOf64bitStrings)
        {
            List<string> listof8BitStrings = new List<string>();
            foreach (string s in listOf64bitStrings)
            {
                string string8Bits = "";
                int i = 0;
                foreach (char c in s)
                {
                    string8Bits += c;
                    if (i == 7)
                    {
                        listof8BitStrings.Add(string8Bits);
                        string8Bits = "";
                        i = 0;
                    }
                    else
                        i++;
                }
            }
            return listof8BitStrings;
        }

        public static string FromBinaryToString(List<string> binary8Bit)
        {
            string stringToReturn = "";

            foreach (string s in binary8Bit)
                stringToReturn += (char)Operations.BinaryToDecimal(s);

            return stringToReturn;
        }

        public static string[] CharArrayToBinary(char[] inputArray)
        {
            string[] charsAsBinaryStrings = new string[inputArray.Length];

            for (int i = 0; i < charsAsBinaryStrings.Length; i++)
            {
                string s = "";
                if (inputArray[i] < 64)
                    s = "00";
                else
                    s = "0";
                charsAsBinaryStrings[i] = s + Convert.ToString(inputArray[i], 2);
            }

            return charsAsBinaryStrings;
        }
    }
}
