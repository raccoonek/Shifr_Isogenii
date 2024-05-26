using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace crypt_RSA
{
    public partial class Form1 : Form
    {
        private BigInteger publicKey;
        private BigInteger privateKey;
        private BigInteger n;

        private Stopwatch stopwatch = new Stopwatch();
        public Form1()
        {
            InitializeComponent();
            GenerateKeys(); //генерация ключей
        }

        private void GenerateKeys()
        {
            stopwatch.Restart(); // Начать отсчет времени

            KeyGenerator p = new KeyGenerator();
            KeyGenerator q = new KeyGenerator();
            KeyGenerator e = new KeyGenerator();
            KeyGenerator f = new KeyGenerator(); //Функция Эйлера

            p.randomGenerator();
            q.randomGenerator();

            //проверяем что сгенерированные числа простые 
            if (p.MillerRabinTest(10) && q.MillerRabinTest(10)) 
            {
                n = p.result * q.result;
            }
            else
            {
                p.GetNearestPrime();
                q.GetNearestPrime();
                n = p.result * q.result;
            }

            f.EulerFunction(p.result, q.result);

            do
            {
                e.randomGenerator(); // Генерация случайного числа e
            } while (gcd(e.result, f.result) != 1); //пока не будет взаимно просто с функцией Эйлера

            

            publicKey = e.result;
           // richTextBox2.Text = publicKey.ToString();
            privateKey = e.invmod(e.result, f.result);

            stopwatch.Stop(); // Остановить отсчет времени
            textBox3.Text = stopwatch.Elapsed.TotalMilliseconds.ToString("0.000 мс"); // Показать время в микросекундах
        }

        //алгоритм Евклида 
        static BigInteger gcd(BigInteger a, BigInteger b)
        {
            while (b != 0)
            {
                BigInteger temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            stopwatch.Restart(); // Начать отсчет времени

            string inputText = richTextBox1.Text;
            List<string> blocks = SplitIntoBlocks(inputText, 64);

            StringBuilder encryptedText = new StringBuilder();

            foreach (string block in blocks)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(block);
                BigInteger message = new BigInteger(bytes);

                EncryptorDecryptor cipher = new EncryptorDecryptor();
                cipher.Encrypt(message, publicKey, n);

                encryptedText.Append(cipher.c.ToString() + " ");
            }

            stopwatch.Stop(); // Остановить отсчет времени
            textBox1.Text = stopwatch.Elapsed.TotalMilliseconds.ToString("0.000 мс"); // Показать время в микросекундах
            richTextBox2.Text = encryptedText.ToString().Trim();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            stopwatch.Restart(); // Начать отсчет времени
            string[] encryptedBlocks = richTextBox2.Text.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);

            StringBuilder decryptedText = new StringBuilder();

            foreach (string encBlock in encryptedBlocks)
            {
                BigInteger encryptedMessage = BigInteger.Parse(encBlock);

                EncryptorDecryptor plain = new EncryptorDecryptor();
                plain.Decrypt(encryptedMessage, privateKey, n);

                byte[] bytes = plain.m.ToByteArray();
                string decryptedBlock = Encoding.UTF8.GetString(bytes);

                decryptedText.Append(decryptedBlock);
            }

            stopwatch.Stop(); // Остановить отсчет времени
            textBox2.Text = stopwatch.Elapsed.TotalMilliseconds.ToString("0.000 мс"); // Показать время в микросекундах

            richTextBox3.Text = decryptedText.ToString();
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

        
    }
}