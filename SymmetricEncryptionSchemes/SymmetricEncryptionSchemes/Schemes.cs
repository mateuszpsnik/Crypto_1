using System;
using System.Collections.Generic;
using System.Text;
using Crypto1;

namespace SymmetricEncryptionSchemes
{
    public static class Schemes
    {
        public static List<byte[]> ECB(List<byte[]> blocksToEncrypt, byte[] key)
        {
            List<byte[]> encryptedBlocks = new List<byte[]>();
            byte[] encryptedBlock = new byte[8];

            foreach(var block in blocksToEncrypt)
            {
                for (int i = 0; i < 8; i++)
                    encryptedBlock[i] = (byte)(block[i] ^ key[i]);
                encryptedBlocks.Add(encryptedBlock);
                encryptedBlock = new byte[8];
            }

            return encryptedBlocks;
        }
    }
}
