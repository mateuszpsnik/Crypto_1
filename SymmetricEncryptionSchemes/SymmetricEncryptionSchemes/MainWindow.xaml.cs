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
using SymmetricEncryptionSchemes;

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

            foreach (SchemesEnum comboItem in Enum.GetValues(typeof(SchemesEnum)))
                comboBox.Items.Add(comboItem);
        }

        private async Task<List<string>> readConvertTextTo64Bits(Microsoft.Win32.OpenFileDialog dialog)
        {
            string filename = dialog.FileName;
            char[] readChars = await InOut.ReadTextFromFile(filename);
            string readString = new string(readChars);
            textBlock.Text = readString;

            textBlock.Text += Environment.NewLine;

            foreach (int i in readChars)
                textBlock.Text += i.ToString() + " ";
          
            string[] intsAsBinaryStrings = Converters.CharArrayToBinary(readChars);
            textBlock.Text += Environment.NewLine;
            foreach (string s in intsAsBinaryStrings)
                textBlock.Text += $"{s} ";

            List<string> blocks64bit = Converters.To64BitStrings(intsAsBinaryStrings);

            textBlock.Text += Environment.NewLine + "64 bit:" + Environment.NewLine;

            foreach(string s in blocks64bit)
                textBlock.Text += s + Environment.NewLine;

            return blocks64bit;
        }

        private string encryptAndConvert(List<string> blocks64Bit)
        {
            //generate and write key
            string key = Operations.generate64BitKey();
            textBlock.Text += "Key:" + Environment.NewLine + key;

            List<string> encrypted64BitBlocks = new List<string>();
            //encrypt
            switch (comboBox.SelectedIndex)
            {
                case 0: encrypted64BitBlocks = Schemes.ECB(blocks64Bit, key);
                    break;
                default: MessageBox.Show("Wrong.");
                    break;
            }
            

            List<string> blocks8Bit = Converters.To8BitBinaryStrings(encrypted64BitBlocks);

            textBlock.Text += Environment.NewLine + "8 bit: " + Environment.NewLine;

            foreach (string s in blocks8Bit)
                textBlock.Text += s + " ";

            string encryptedText = Converters.FromBinaryToString(blocks8Bit);
            textBlock.Text += Environment.NewLine + encryptedText;

            return encryptedText;
        }

        private async void filePickerButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();

            var result = dialog.ShowDialog();

            if (result == true)
            {
                List<string> blocks64Bit = await readConvertTextTo64Bits(dialog);
                string encryptedText = encryptAndConvert(blocks64Bit);
                await InOut.WriteTextToFile(dialog.FileName, encryptedText);
            }
        }
    }
}
