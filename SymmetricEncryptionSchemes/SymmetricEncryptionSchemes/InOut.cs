﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SymmetricEncryptionSchemes
{
    public static class InOut
    {
        public static async Task<char[]> ReadTextFromFile(string filename)
        {
            char[] result = { 'n', 'o', ' ', 't', 'e', 'x', 't' };

            using (StreamReader reader = File.OpenText(filename))
            {
                Console.WriteLine("Opened the file.");
                result = new char[reader.BaseStream.Length];
                await reader.ReadAsync(result, 0, (int)reader.BaseStream.Length);

                foreach (char c in result)
                {
                    Console.Write($"{c} ");
                }
            }

            return result;
        }
    }
}
