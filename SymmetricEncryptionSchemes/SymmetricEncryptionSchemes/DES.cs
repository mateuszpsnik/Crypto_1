using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Crypto1
{
    class DES
    {
        /*
         This DES implementation uses the CBC mode.
         NOTE: As requested, there is only one round of DES.

         The implementation of this class is based on this website:
         http://page.math.tu-berlin.de/~kant/teaching/hess/krypto-ws2006/des.htm
         */

        public DES(List<byte[]> blocksToEncrypt, byte[] key, ref byte[] vectorIV)
        {
            vectorIV = Operations.Generate64BitKey();

            for (int i = 0; i < blocksToEncrypt.Count; i++)
            {
                byte[] blockAfterInitialXOR = new byte[8];
                byte[] encryptedBlock = new byte[8];

                if (i == 0)
                    blockAfterInitialXOR = xorOneBlock(blocksToEncrypt[i], vectorIV);
                else
                    blockAfterInitialXOR = xorOneBlock(blocksToEncrypt[i], blocksToEncrypt[i - 1]);

                encryptedBlock = encrypt(blockAfterInitialXOR, key);
                encryptedBlocks.Add(encryptedBlock);
            }
        }

        private List<byte[]> encryptedBlocks = new List<byte[]>();
        public List<byte[]> EncryptedBlocks => encryptedBlocks;

        private byte[] xorOneBlock(byte[] blockToEncrypt, byte[] key)
        {
            byte[] encryptedBlock = new byte[8];

            for (int i = 0; i < 8; i++)
                encryptedBlock[i] = (byte)(blockToEncrypt[i] ^ key[i]);

            return encryptedBlock;
        }

        private byte[] encrypt(byte[] blockToEncrypt, byte[] key)
        {
            byte[] encryptedBlock = new byte[8];
            BitArray newKey = createSubkey(key);
            BitArray blockAsBits = new BitArray(blockToEncrypt);
            BitArray blockAfterInitialPerm = new BitArray(64);

            for (int i = 0; i < 64; i++)
                blockAfterInitialPerm[i] = blockAsBits[IP[i] - 1];

            BitArray L0 = new BitArray(32);
            BitArray R0 = new BitArray(32);
            for (int i = 0; i < 32; i++)
            {
                L0[i] = blockAfterInitialPerm[i];
                R0[i] = blockAfterInitialPerm[i + 32];
            }

            BitArray L1 = R0;
            BitArray blockR0AfterFunction = function(R0, newKey);
            BitArray R1 = L0.Xor(blockR0AfterFunction);

            BitArray L1R1 = new BitArray(64);
            for (int i = 0; i < 64; i++)
            {
                if (i < 32)
                    L1R1[i] = L1[i];
                else
                    L1R1[i] = R1[i - 32];
            }

            BitArray encryptedBlockBit = new BitArray(64);
            for (int i = 0; i < 64; i++)
                encryptedBlockBit[i] = L1R1[reverseIP[i] - 1];

            List<BitArray> blocks8bit = new List<BitArray>(); 
            int iterator = 0;
            BitArray block8bit = new BitArray(8);
            for (int i = 0; i < 64; i++)
            {
                block8bit[iterator] = encryptedBlockBit[i];
                iterator++;
                if (iterator == 8)
                {
                    iterator = 0;
                    blocks8bit.Add(block8bit);
                    block8bit = new BitArray(8);
                }
            }
            byte[] encryptedBytes = new byte[8];
            for (int i = 0; i < 8; i++)
                encryptedBytes[i] = convertToByte(blocks8bit[i]);

            return encryptedBytes;
        }

        private BitArray createSubkey(byte[] key)
        {
            BitArray subkey = new BitArray(48);
            BitArray keyAsBits = new BitArray(key);
            BitArray originalKey56bits = new BitArray(56);

            for (int i = 0; i < 48; i++)
                originalKey56bits[i] = keyAsBits[PC_1[i] - 1];

            BitArray C0 = new BitArray(28);
            BitArray D0 = new BitArray(28);
            for (int i = 0; i < 28; i++)
            {
                C0[i] = originalKey56bits[i];
                D0[i] = originalKey56bits[i + 28];
            }

            BitArray C1 = C0.LeftShift(1);
            BitArray D1 = D0.LeftShift(1);
            BitArray C1D1 = new BitArray(56);
            for (int i = 0; i < 56; i++)
            {
                if (i < 28)
                    C1D1[i] = C1[1];
                else
                    C1D1[i] = D1[i - 28];
            }

            for (int i = 0; i < 48; i++)
                subkey[i] = C1D1[PC_2[i] - 1];

            return subkey;
        }

        private BitArray function(BitArray block32bit, BitArray key)
        {
            BitArray block48bit = new BitArray(48);
            for (int i = 0; i < 48; i++)
                block48bit[i] = block32bit[selectionTable[i] - 1];

            BitArray blockBeforeSBoxes = block48bit.Xor(key);

            List<BitArray> blocks6bit = new List<BitArray>();
            int iterator = 0;
            BitArray block6bit = new BitArray(6);
            for (int i = 0; i < 48; i++)
            {
                block6bit[iterator] = blockBeforeSBoxes[i];
                iterator++;
                if (iterator == 6)
                {
                    iterator = 0;
                    blocks6bit.Add(block6bit);
                    block6bit = new BitArray(6);
                }
            }

            byte[] sBoxValues = new byte[8];
            for (int i = 0; i < 8; i++)
                sBoxValues[i] = sBox(blocks6bit[i], i + 1);

            BitArray blockAfterSBox = new BitArray(sBoxValues);
            BitArray blockAfterPermutation = new BitArray(32);

            for (int i = 0; i < 32; i++)
                blockAfterPermutation[i] = blockAfterSBox[p[i] - 1];

            return blockAfterPermutation;
        }

        private byte sBox(BitArray input, int boxNumber)
        {
            BitArray indexI = new BitArray(2);
            BitArray indexJ = new BitArray(4);
            indexI[0] = input[0];
            indexJ[0] = input[1];
            indexJ[1] = input[2];
            indexJ[2] = input[3];
            indexJ[3] = input[4];
            indexI[1] = input[5];

            byte i = convertToByte(indexI);
            byte j = convertToByte(indexJ);
            byte value = 0;

            switch(boxNumber)
            {
                case 1: 
                    value = s1[i, j];
                    break;
                case 2:
                    value = s2[i, j];
                    break;
                case 3:
                    value = s3[i, j];
                    break;
                case 4:
                    value = s4[i, j];
                    break;
                case 5:
                    value = s5[i, j];
                    break;
                case 6:
                    value = s6[i, j];
                    break;
                case 7:
                    value = s7[i, j];
                    break;
                case 8:
                    value = s8[i, j];
                    break;
                default: throw new Exception("Wrong box number");
            }

            return value;
        }

        private byte convertToByte(BitArray input)
        {
            byte[] bytes = new byte[1];
            input.CopyTo(bytes, 0);
            return bytes[0];
        }

        private byte[] PC_1 =
        {     
            57,   49,    41,   33,    25,    17,    9,
             1,   58,    50,   42,    34,    26,   18,
            10,    2,    59,   51,    43,    35,   27,
            19,   11,     3,   60,    52,    44,   36,
            63,   55,    47,   39,    31,    23,   15,
             7,   62,    54,   46,    38,    30,   22,
            14,    6,    61,   53,    45,    37,   29,
            21,   13,     5,   28,    20,    12,    4 
        };

        private byte[] PC_2 =
        {
            14,    17,   11,    24,     1,    5,
             3,    28,   15,     6,    21,   10,
            23,    19,   12,     4,    26,    8,
            16,     7,   27,    20,    13,    2,
            41,    52,   31,    37,    47,   55,
            30,    40,   51,    45,    33,   48,
            44,    49,   39,    56,    34,   53,
            46,    42,   50,    36,    29,   32
        };

        private byte[] IP =
        {
            58,    50,   42,    34,    26,   18,    10,    2,
            60,    52,   44,    36,    28,   20,    12,    4,
            62,    54,   46,    38,    30,   22,    14,    6,
            64,    56,   48,    40,    32,   24,    16,    8,
            57,    49,   41,    33,    25,   17,     9,    1,
            59,    51,   43,    35,    27,   19,    11,    3,
            61,    53,   45,    37,    29,   21,    13,    5,
            63,    55,   47,    39,    31,   23,    15,    7,
        };

        private byte[] selectionTable =
        {
            32,     1,    2,     3,     4,    5,
             4,     5,    6,     7,     8,    9,
             8,     9,   10,    11,    12,   13,
            12,    13,   14,    15,    16,   17,
            16,    17,   18,    19,    20,   21,
            20,    21,   22,    23,    24,   25,
            24,    25,   26,    27,    28,   29,
            28,    29,   30,    31,    32,    1
        };

        private byte[,] s1 =
        {
            { 14,  4,  13,  1,   2, 15,  11,  8,   3, 10,   6, 12,   5,  9,   0,  7 },
             { 0, 15,   7,  4,  14,  2,  13,  1,  10,  6,  12, 11,   9,  5,   3,  8 },
            { 4,  1,  14,  8,  13,  6,   2, 11,  15, 12,   9,  7,   3, 10 ,  5,  0 },
             { 15, 12,   8,  2,   4,  9,   1,  7,   5, 11,   3, 14,  10,  0 ,  6, 13 }
        };

        private byte[,] s2 =
        {
           { 15,  1,   8, 14,   6, 11,   3,  4,   9,  7,   2, 13, 12,  0,   5, 10 },
           { 3, 13,   4,  7,  15,  2 ,  8, 14,  12,  0,   1, 10,   6,  9,  11,  5 },
           { 0, 14,   7, 11,  10,  4,  13,  1,   5,  8 , 12,  6,   9,  3 ,  2, 15 },
           { 13 , 8,  10,  1,   3, 15,   4,  2,  11,  6 ,  7, 12,   0,  5,  14,  9 }
        };

        private byte[,] s3 =
        {
           {  10,  0,   9, 14,   6,  3 , 15,  5,   1, 13,  12,  7,  11,  4,   2,  8 },
     { 13,  7,   0,  9 ,  3,  4,   6, 10,   2,  8,   5, 14,  12, 11 , 15,  1 },
     { 13,  6,   4,  9,   8, 15,   3,  0,  11,  1,   2 ,12 ,  5 ,10 , 14,  7 },
      { 1, 10,  13,  0,   6,  9,   8,  7,   4, 15,  14,  3,  11,  5 ,  2, 12 }
        };

        private byte[,] s4 =
        {
          { 7, 13,  14,  3 ,  0,  6,   9, 10,   1,  2,   8,  5,  11, 12 ,  4 ,15 },
       { 13,  8,  11,  5,   6, 15,   0,  3,   4,  7,   2, 12,   1, 10,  14,  9 },
      { 10,  6,   9,  0,  12, 11,   7, 13,  15,  1,   3, 14,   5,  2,   8,  4 },
      { 3, 15,   0,  6,  10,  1,  13,  8,   9,  4,   5, 11,  12,  7,   2, 14 }
        };

        private byte[,] s5 =
        {
           { 2, 12,   4,  1,   7, 10,  11,  6,   8,  5,   3, 15,  13,  0,  14,  9 },
      { 14, 11,   2, 12,   4,  7,  13,  1,   5,  0,  15, 10,   3,  9,   8,  6 },
      { 4,  2,   1, 11,  10, 13,   7,  8,  15,  9,  12,  5,   6,  3,   0, 14 },
      { 11,  8 , 12,  7,   1, 14,   2, 13,   6, 15,   0,  9,  10,  4,   5,  3 }
        };

        private byte[,] s6 =
        {
           { 12,  1,  10, 15,   9,  2,   6,  8,   0, 13,   3,  4,  14,  7,   5, 11 },
      { 10, 15 ,  4,  2,   7, 12,   9,  5,   6,  1,  13, 14,   0, 11,   3,  8 },
      { 9, 14,  15,  5,   2,  8,  12,  3 ,  7,  0,   4, 10,   1, 13,  11,  6 },
      { 4,  3,   2, 12,   9,  5, 15, 10,  11, 14,   1,  7,   6,  0,   8, 13 }

        };

        private byte[,] s7 =
        {
           { 4, 11,   2, 14,  15,  0,   8, 13,   3, 12,   9,  7,   5, 10,   6,  1 },
      { 13,  0,  11,  7,   4,  9 ,  1, 10,  14,  3,   5, 12,   2 ,15,   8,  6 },
      { 1,  4,  11, 13,  12,  3,   7, 14,  10, 15,   6,  8,   0,  5,   9,  2 },
      { 6, 11,  13,  8,   1,  4,  10,  7,   9,  5,   0, 15,  14,  2,   3, 12 }
        };

        private byte[,] s8 =
        {
           { 13,  2,   8,  4,   6, 15,  11,  1,  10,  9,   3, 14,   5,  0 , 12,  7 },
       { 1, 15,  13,  8,  10,  3,   7,  4,  12,  5,   6, 11,   0, 14,   9,  2 },
      { 7, 11,   4,  1,   9, 12,  14,  2 ,  0 , 6 , 10 ,13 , 15,  3 ,  5,  8 },
      { 2,  1,  14,  7,   4, 10 ,  8 ,13 , 15, 12,   9,  0,   3,  5,   6, 11 }
        };

        private byte[] p =
        {
            16,   7,  20,  21,
            29,  12,  28,  17,
             1,  15,  23,  26,
             5,  18,  31,  10,
             2,   8,  24,  14,
            32,  27,   3,   9,
            19,  13,  30,   6,
            22,  11,   4,  25
        };

        private byte[] reverseIP =
        {
            40,     8,   48,    16,    56,   24,    64,   32,
            39,     7,   47,    15,    55,   23,    63,   31,
            38,     6,   46,    14,    54,   22,    62,   30,
            37,     5,   45,    13,    53,   21,    61,   29,
            36,     4,   44,    12,    52,   20,    60,   28,
            35,     3,   43,    11,    51,   19,    59,   27,
            34,     2,   42,    10,    50,   18,    58,   26,
            33,     1,   41,     9,    49,   17,    57,   25
        };
    }
}
