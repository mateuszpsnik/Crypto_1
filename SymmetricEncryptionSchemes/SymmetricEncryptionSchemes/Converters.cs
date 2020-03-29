using System;
using System.Collections.Generic;
using System.Text;

namespace Crypto1
{
    public static class Converters
    {
        public static List<byte[]> To64BitBlocks(byte[] bytes)
        {
            List<byte[]> byteBlocks = new List<byte[]>();
            byte[] block64bit = new byte[8];

            int i = 0;
            for (int j = 0; j < bytes.Length; j++)
            {
                block64bit[i] = bytes[j];
                i++;
                if (i == 8 || j == bytes.Length - 1)
                {
                    byteBlocks.Add(block64bit);
                    i = 0;
                    block64bit = new byte[8];
                }
            }
            return byteBlocks;
        }

        public static string ByteAsStringLength8(byte b)
        {
            string s = Convert.ToString(b, 2);
            int length = s.Length;
            string toReturn = "";

            for (int i = 0; i < (8 - length); i++)
                toReturn += "0";

            toReturn += s;
            return toReturn;
        }

        public static byte[] CharArrayToByte(char[] inputArray)
        {
            byte[] charsAsBytes = new byte[inputArray.Length];

            for (int i = 0; i < inputArray.Length; i++)
                charsAsBytes[i] = (byte)inputArray[i];

            return charsAsBytes;
        }
    }
}
