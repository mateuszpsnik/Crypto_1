using System;
using System.Collections.Generic;
using System.Text;

namespace Crypto1
{
    public static class Modes
    {
        private static byte[] xorOneBlock(byte[] blockToEncrypt, byte[] key)
        {
            byte[] encryptedBlock = new byte[8];

            for (int i = 0; i < 8; i++)
                encryptedBlock[i] = (byte)(blockToEncrypt[i] ^ key[i]);

            return encryptedBlock;
        }

        public static List<byte[]> ECB(List<byte[]> blocksToEncrypt, byte[] key)
        {
            List<byte[]> encryptedBlocks = new List<byte[]>();
            
            foreach(var block in blocksToEncrypt)
            {
                byte[] encryptedBlock = new byte[8];
                encryptedBlock = xorOneBlock(block, key);
                encryptedBlocks.Add(encryptedBlock);
            }

            return encryptedBlocks;
        }

        public static List<byte[]> CBC(List<byte[]> blocksToEncrypt, byte[] key,
            ref byte[] vectorIV)
        {
            List<byte[]> encryptedBlocks = new List<byte[]>();
            vectorIV = Operations.Generate64BitKey();

            for (int i = 0; i < blocksToEncrypt.Count; i++)
            {
                byte[] blockAfterInitialXOR = new byte[8];
                byte[] encryptedBlock = new byte[8];

                if (i == 0)
                    blockAfterInitialXOR = xorOneBlock(blocksToEncrypt[i], vectorIV);
                else
                    blockAfterInitialXOR = xorOneBlock(blocksToEncrypt[i], blocksToEncrypt[i - 1]);

                encryptedBlock = xorOneBlock(blockAfterInitialXOR, key);
                encryptedBlocks.Add(encryptedBlock);
            }

            return encryptedBlocks;
        }

        public static List<byte[]> CFB(List<byte[]> blocksToEncrypt, byte[] key,
            ref byte[] vectorIV)
        {
            List<byte[]> encryptedBlocks = new List<byte[]>();
            vectorIV = Operations.Generate64BitKey();

            for (int i = 0; i < blocksToEncrypt.Count; i++)
            {
                byte[] encryptedPreviousBlock = new byte[8];
                byte[] encryptedBlock = new byte[8];

                if (i == 0)
                    encryptedPreviousBlock = xorOneBlock(vectorIV, key);
                else
                {
                    byte[] previousBlock = blocksToEncrypt[i - 1];
                    encryptedPreviousBlock = xorOneBlock(previousBlock, key);
                }

                encryptedBlock = xorOneBlock(blocksToEncrypt[i], encryptedPreviousBlock);
                encryptedBlocks.Add(encryptedBlock);
            }

            return encryptedBlocks;
        }
    }
}