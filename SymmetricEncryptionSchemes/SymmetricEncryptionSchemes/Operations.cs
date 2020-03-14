using System;
using System.Collections.Generic;
using System.Text;

namespace Crypto1
{
    public static class Operations
    {
        public static int Xor(char bit1, char bit2)
        {
            int b1 = (int)bit1 - 48;
            int b2 = (int)bit2 - 48;

            return (b1 + b2) % 2;
        }

        public static int BinaryToDecimal(string binary8Bit)
        {
            int dec = 0;
            int j = 0;
            for (int i = 7; i >= 0; i--)
            {
                dec += ((int)binary8Bit[j] - 48) * (int)Math.Pow(2, i);
                j++;
            }

            return dec;
        }

        public static string generate64BitKey()
        {
            Random random = new Random();
            string key = "";

            for (int i = 0; i < 64; i++)
                key += random.Next(0, 2);

            return key;
        }
    }
}
