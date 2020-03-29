using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Crypto1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            foreach (ModesEnum comboItem in Enum.GetValues(typeof(ModesEnum)))
                comboBox.Items.Add(comboItem);

            InitialText initialText = new InitialText();
            textBlock.Text = initialText.Text;
        }

        private string encryptedTextUnicode;
        private string encryptedTextBase64;

        private async Task<List<byte[]>> readConvertTextTo64Bits
            (Microsoft.Win32.OpenFileDialog dialog)
        {
            string filename = dialog.FileName;
            char[] readChars = await InOut.ReadTextFromFile(filename);
            string readString = new string(readChars);
            textBlock.Text = readString;

            textBlock.Text += Environment.NewLine;

            foreach (int i in readChars)
                textBlock.Text += i.ToString() + " ";
          
            byte[] textAsBytes = Converters.CharArrayToByte(readChars);
            textBlock.Text += Environment.NewLine;
            foreach (byte b in textAsBytes)
                textBlock.Text += Convert.ToString(b, 2) + " ";

            List<byte[]> blocks64bit = Converters.To64BitBlocks(textAsBytes);

            textBlock.Text += Environment.NewLine + "64 bit:" + Environment.NewLine;

            foreach(var block64 in blocks64bit)
            {
                foreach (var b in block64)
                {
                    textBlock.Text += Converters.ByteAsStringLength8(b);
                }
                textBlock.Text += " ";
            }

            return blocks64bit;
        }

        private void encryptAndConvert(List<byte[]> blocks64Bit)
        {
            //generate and write key
            byte[] key = Operations.Generate64BitKey();
            textBlock.Text += Environment.NewLine + "Key:" + Environment.NewLine;
            foreach (var b in key)
                textBlock.Text += Converters.ByteAsStringLength8(b);

            List<byte[]> encrypted64BitBlocks = new List<byte[]>();
            //encrypt
            switch (comboBox.SelectedIndex)
            {
                case 0: encrypted64BitBlocks = Modes.ECB(blocks64Bit, key);
                    break;
                case 1:
                    {
                        byte[] vectorIV = new byte[8];
                        encrypted64BitBlocks = Modes.CBC(blocks64Bit, key, ref vectorIV);
                        textBlock.Text += Environment.NewLine + "Vector IV:" + Environment.NewLine;
                        foreach (var b in vectorIV)
                            textBlock.Text += Converters.ByteAsStringLength8(b);
                        break;
                    }
                case 2:
                    {
                        byte[] vectorIV = new byte[8];
                        encrypted64BitBlocks = Modes.CFB(blocks64Bit, key, ref vectorIV);
                        textBlock.Text += Environment.NewLine + "Vector IV:" + Environment.NewLine;
                        foreach (var b in vectorIV)
                            textBlock.Text += Converters.ByteAsStringLength8(b);
                        break;
                    }
                case 3:
                    {
                        byte[] vectorIV = new byte[8];
                        DES des = new DES(blocks64Bit, key, ref vectorIV);
                        encrypted64BitBlocks = des.EncryptedBlocks;
                        textBlock.Text += Environment.NewLine + "Vector IV:" + Environment.NewLine;
                        foreach (var b in vectorIV)
                            textBlock.Text += Converters.ByteAsStringLength8(b);
                        break;
                    }
                default: MessageBox.Show("You have not chosen any option.");
                    break;
            }

            textBlock.Text += Environment.NewLine + "Encrypted 64-bit blocks:" + Environment.NewLine;
            foreach (var arr in encrypted64BitBlocks)
            {
                foreach (var b in arr)
                    textBlock.Text += Converters.ByteAsStringLength8(b);
                textBlock.Text += " ";
            }

            textBlock.Text += Environment.NewLine + "8 bit: " + Environment.NewLine;
            encryptedTextUnicode += "Unicode: ";
            encryptedTextBase64 += Environment.NewLine;
            encryptedTextBase64 += "Base-64: ";

            foreach (var arr in encrypted64BitBlocks)
            {
                foreach (var b in arr)
                    textBlock.Text += Convert.ToString(b, 2) + " ";
                textBlock.Text += " ";

                encryptedTextUnicode += System.Text.Encoding.Unicode.GetString(arr);
                encryptedTextBase64 += Convert.ToBase64String(arr);
            }

            textBlock.Text += Environment.NewLine + encryptedTextUnicode;
            textBlock.Text += encryptedTextBase64;
        }

        private async void filePickerButton_Click(object sender, RoutedEventArgs e)
        {
            saveFilePickerButton.Visibility = Visibility.Visible;
            textBlock.Text = "";
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            var result = dialog.ShowDialog();

            if (result == true)
            {
                List<byte[]> blocks64Bit = await readConvertTextTo64Bits(dialog);
                encryptAndConvert(blocks64Bit);
            } 
        }

        private async void saveFilePickerButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            var result = dialog.ShowDialog();

            if (result == true)
            {
                await InOut.WriteTextToFile(dialog.FileName, encryptedTextUnicode, encryptedTextBase64);
            }
        }

        private void comboBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            filePickerButton.Visibility = Visibility.Visible;
        }
    }
}
