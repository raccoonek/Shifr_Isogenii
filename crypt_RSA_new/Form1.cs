﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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

        public Form1()
        {
            InitializeComponent();
            GenerateKeys();
        }

        private void GenerateKeys()
        {
            KeyGenerator p = new KeyGenerator();
            KeyGenerator q = new KeyGenerator();
            KeyGenerator e = new KeyGenerator();
            KeyGenerator f = new KeyGenerator();

            p.randomGenerator();
            q.randomGenerator();

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
            e.randomGenerator();
            
            publicKey = e.invmod(e.result, f.result);
            privateKey = e.result;

            //richTextBox2.Text = privateKey.ToString();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string inputText = richTextBox1.Text;
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(inputText);
            BigInteger message = new BigInteger(bytes);

            EncryptorDecryptor cipher = new EncryptorDecryptor();
            cipher.Encrypt(message, publicKey, n);

            richTextBox2.Text = cipher.c.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            BigInteger encryptedMessage = BigInteger.Parse(richTextBox2.Text);

            EncryptorDecryptor plain = new EncryptorDecryptor();
            plain.Decrypt(encryptedMessage, privateKey, n);

            byte[] bytes = plain.m.ToByteArray();
            string decryptedText = Encoding.UTF8.GetString(bytes);


            //StringBuilder sb = new StringBuilder();
            //foreach (var b in bytes)
            //{
            //    sb.Append(b);
            //}
            //string outVAR = sb.ToString();
            richTextBox2.Text = decryptedText;
            //richTextBox2.Text = plain.m.ToString();


            // richTextBox1.Text = n.ToString();

            //Form1 form1 = new Form1();
            //form1.richTextBox2.Text = d.ToString();
        }

    }
    
}
