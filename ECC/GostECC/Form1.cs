using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;

namespace GostECC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

       
        private ECPoint Q = new ECPoint();
        private BigInteger d = new BigInteger();
        private Stopwatch stopwatch = new Stopwatch();
        BigInteger p = new BigInteger("6277101735386680763835789423207666416083908700390324961279", 10);
        BigInteger a = new BigInteger("-3", 10);
        BigInteger b = new BigInteger("64210519e59c80e70fa7e9ab72243049feb8deecc146b9b1", 16);
        
        BigInteger n = new BigInteger("ffffffffffffffffffffffff99def836146bc9b1b4d22831", 16);

        private void button_Click(object sender, EventArgs e)
        {

            byte[] xG = FromHexStringToByte("03188da80eb03090f67cbf20eb43a18800f4ff0afd82ff1012");
            string inputText = richTextBox1.Text;
            List<string> blocks = SplitIntoBlocks(inputText, 24);

            StringBuilder encryptedText = new StringBuilder();

            stopwatch.Restart(); // Начать отсчет времени

            DS = new DSGost(p, a, b, n, xG);
            d = DS.GenPrivateKey(192);
            Q = DS.GenPublicKey(d);
            //GOST hash = new GOST(256);
            stopwatch.Stop(); // Остановить отсчет времени
            textBox3.Text = stopwatch.Elapsed.TotalMilliseconds.ToString("0.000 мкс"); // Показать время в микросекундах

            stopwatch.Restart(); // Начать отсчет времени
            foreach (string block in blocks)
            {
               
                byte[] H  = Encoding.UTF8.GetBytes(block);
                string sign = DS.SingGen(H, d);
                //textBox2.Text = sign;
               // bool result = DS.SingVer(H, sign, Q);

                encryptedText.Append(sign);
            }

            stopwatch.Stop(); // Остановить отсчет времени
            textBox1.Text = stopwatch.Elapsed.TotalMilliseconds.ToString("0.000 мкс"); // Показать время в микросекундах
            richTextBox2.Text = encryptedText.ToString().Trim();
        }
        DSGost DS;
        private void button20_Click(object sender, EventArgs e)
        {

            stopwatch.Restart(); // Начать отсчет времени
            //string[] encryptedBlocks = richTextBox2.Text.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
            
            StringBuilder decryptedText = new StringBuilder();

            
            //GOST hash = new GOST(256);

            string inputText = richTextBox2.Text;
            List<string> encryptedBlocks = SplitIntoBlocks(inputText, (n.bitCount()));


            foreach (string encBlock in encryptedBlocks)
            {
                
                string decryptedBlock = DS.SingVer(encBlock, d);

                byte[] decryptedBlockBytes = FromHexStringToByte((string)decryptedBlock);

                string ad = Encoding.UTF8.GetString(decryptedBlockBytes);

                decryptedText.Append(ad);
            }

            stopwatch.Stop(); // Остановить отсчет времени
            textBox2.Text = stopwatch.Elapsed.TotalMilliseconds.ToString("0.000 мкс"); // Показать время в микросекундах

            richTextBox2.Text = decryptedText.ToString();
        }

        private List<string> SplitIntoBlocks(string inputText, int blockSize)
        {
            List<string> blocks = new List<string>();

            for (int i = 0; i < inputText.Length; i += blockSize)
            {
                if (i + blockSize <= inputText.Length)
                {
                    blocks.Add(inputText.Substring(i, blockSize));
                }
                else
                {
                    blocks.Add(inputText.Substring(i));
                }
            }

            return blocks;
        }
        private byte[] FromHexStringToByte(string input)
        {
            byte[] data = new byte[input.Length / 2];
            string HexByte = "";
            for (int i = 0; i < data.Length; i++)
            {
                HexByte = input.Substring(i * 2, 2);
                data[i] = Convert.ToByte(HexByte, 16);
            }
            return data;
        }

        
    }
}
