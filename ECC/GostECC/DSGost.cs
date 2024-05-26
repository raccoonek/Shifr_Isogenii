using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GostECC
{
    class DSGost
    {
        private BigInteger p = new BigInteger();
        private BigInteger a = new BigInteger();
        private BigInteger b = new BigInteger();
        private BigInteger n = new BigInteger();
        private byte[] xG;
        private ECPoint G = new ECPoint();
        private ECPoint Q = new ECPoint();

        public DSGost(BigInteger p, BigInteger a, BigInteger b, BigInteger n, byte[] xG)
        {
            this.a = a;
            this.b = b;
            this.n = n;
            this.p = p;
            this.xG = xG;
        }

        //Генерируем секретный ключ заданной длины
        public BigInteger GenPrivateKey(int BitSize)
        {
            BigInteger d = new BigInteger();
            do
            {
                d.genRandomBits(BitSize, new Random());
            } while ((d < 0) || (d > n));
            return d;
        }

        //С помощью секретного ключа d вычисляем точку Q=d*G, это и будет наш публичный ключ
        public ECPoint GenPublicKey(BigInteger d)
        {
            G=GDecompression();
            Q = ECPoint.multiply(d, G);
            return Q;
        }

        //Восстанавливаем координату y из координаты x и бита четности y 
        private ECPoint GDecompression()
        {
            byte y = xG[0];
            byte[] x=new byte[xG.Length-1];
            Array.Copy(xG, 1, x, 0, xG.Length - 1);
            BigInteger Xcord = new BigInteger(x);
            BigInteger temp = (Xcord * Xcord * Xcord + a * Xcord + b) % p;
            BigInteger beta = ModSqrt(temp, p);
            BigInteger Ycord = new BigInteger();
            if ((beta % 2) == (y % 2))
                Ycord = beta;
            else
                Ycord = p - beta;
            ECPoint G = new ECPoint();
            G.a = a;
            G.b = b;
            G.FieldChar = p;
            G.x = Xcord;
            G.y = Ycord;
            this.G = G;
            return G;
        }

        //функция вычисления квадратоного корня по модулю простого числа q
        public BigInteger ModSqrt(BigInteger a, BigInteger q)
        {
            BigInteger b = new BigInteger();
            do
            {
                b.genRandomBits(255, new Random());
            } while (Legendre(b, q) == 1);
            BigInteger s = 0;
            BigInteger t = q - 1;
            while ((t & 1) != 1)
            {
                s++;
                t = t >> 1;
            }
            BigInteger InvA = a.modInverse(q);
            BigInteger c = b.modPow(t, q);
            BigInteger r = a.modPow(((t + 1) / 2), q);
            BigInteger d = new BigInteger();
            for (int i = 1; i < s; i++)
            {
                BigInteger temp = 2;
                temp = temp.modPow((s - i - 1), q);
                d = (r.modPow(2, q) * InvA).modPow(temp, q);
                if (d == (q - 1))
                    r = (r * c) % q;
                c = c.modPow(2, q);
            }
            return r;
        }

        //Вычисляем символ Лежандра
        public BigInteger Legendre(BigInteger a, BigInteger q)
        {
            return a.modPow((q - 1) / 2, q);
        }

        //подписываем сообщение
        public string SingGen(byte[] h, BigInteger d)
        {
            BigInteger alpha = new BigInteger(h);
            BigInteger temp = ((((alpha * alpha)%p) * alpha)%p + a * alpha + b) % p;
            ECPoint P = new ECPoint();
            P.x = alpha;
            P.y = temp;
            P.a = a; P.b = b;P.FieldChar = p;

            BigInteger k = new BigInteger();
            ECPoint C=new ECPoint();
            ECPoint C1 = new ECPoint();
            BigInteger r=new BigInteger();
            BigInteger s = new BigInteger();
            BigInteger r1 = new BigInteger();
            BigInteger s1 = new BigInteger();
            do
            {
                do
                {
                    k.genRandomBits(n.bitCount(), new Random());
                } while ((k < 0) || (k > n));
                C = ECPoint.multiply(k, G);
                C1 = P + ECPoint.multiply(k, Q);
                r = C.x ; 
                s = C.y;
                r1 = C1.x;
                s1 = C1.y;
            } while ((r == 0)||(s==0));
            string Rvector = padding(r.ToHexString(),n.bitCount()/4);
            string Svector = padding(s.ToHexString(), n.bitCount() / 4);
            string R1vector = padding(r1.ToHexString(), n.bitCount() / 4);
            string S1vector = padding(s1.ToHexString(), n.bitCount() / 4);
            return Rvector + Svector+ R1vector + S1vector;
        }

        //проверяем подпись 
        public string SingVer(string sing, BigInteger d)
        {

            string Rvector = sing.Substring(0, n.bitCount() / 4);
            string Svector = sing.Substring(n.bitCount() / 4, n.bitCount() / 4);
            string R1vector = sing.Substring(n.bitCount() / 2, n.bitCount() / 4);
            string S1vector = sing.Substring(3*n.bitCount() / 4, n.bitCount() / 4);
            BigInteger r = new BigInteger(Rvector, 16);
            BigInteger s = new BigInteger(Svector, 16);
            BigInteger r1 = new BigInteger(R1vector, 16);
            BigInteger s1 = new BigInteger(S1vector, 16);

            ECPoint C = new ECPoint();
            C.x = r; C.y = s; C.a = a;C.b = b;C.FieldChar = p;

            ECPoint C1 = new ECPoint();
            C1.x = r1; C1.y = s1; C1.a = a; C1.b = b; C1.FieldChar = p;

            ECPoint Res = ECPoint.multiply(d,C);
            Res.y = p - Res.y;
            Res = C1+Res;

            r = Res.x; s = Res.y;
            Rvector = r.ToHexString();
             Svector = padding(s.ToHexString(), n.bitCount() / 4);
            //BigInteger R = C.x % n;
            //if (R == r)
            //    return true;
            //else
            //    return false;
            return Rvector ;
        }

        //дополняем подпись нулями слева до длины n, где n - длина модуля в битах
        private string padding(string input, int size)
        {
            if (input.Length < size)
            {
                do
                {
                    input = "0" + input;
                } while (input.Length < size);
            }
            return input;
        }
    }
}
