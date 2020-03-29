using System;
using System.Collections.Generic;
using System.Text;

namespace Crypto1
{
    public static class Operations
    {
        public static byte[] Generate64BitKey()
        {
            Random random = new Random();
            byte[] key = new byte[8];

            random.NextBytes(key);

            return key;
        }
    }
}
