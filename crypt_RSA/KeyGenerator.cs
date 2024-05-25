using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace crypt_RSA
{
    class KeyGenerator
    {
        public BigInteger result, d;
        public int f;

        public void randomGenerator()                            //create random number
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] randomNumber = new byte[64];
            rng.GetBytes(randomNumber);
            result = new BigInteger(randomNumber);
            result = BigInteger.Abs(result);
        }

        public bool MillerRabinTest(int k)    //k - система счисления         
        {
            if (result == 2 || result == 3) //если число равно 2 или 3 - число простое
                return true;
            if (result < 2 || result % 2 == 0) //если число меньше 2 или число делится на 2 без остатка - число составное
                return false;

            BigInteger d = result - 1; 
            int s = 0;

            while (d % 2 == 0) 
            {
                d /= 2;
                s += 1;

            }

            for (int i = 0; i < k; i++)
            {
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] _a = new byte[result.ToByteArray().LongLength]; 
                BigInteger a;

                do
                {
                    rng.GetBytes(_a); //генерируем возможный свидетель простоты
                    a = new BigInteger(_a);
                }
                while (a < 2 || a >= result - 2);

                BigInteger x = BigInteger.ModPow(a, d, result);

                if (x == 1 || x == result - 1) //число простое
                    continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, result);

                    if (x == 1)
                        return false;
                    if (x == result - 1)
                        break;
                }

                if (x != result - 1)
                    return false;
            }
            return true;
        }

        public BigInteger GetNearestPrime()                  //get the next prime
        {
            while (MillerRabinTest(10) == false)
            {
                result++;
            }
            return result;
        }

        public BigInteger EulerFunction(BigInteger p, BigInteger q)
        {
            result = (p - 1) * (q - 1);
            return result;
        }

        public BigInteger GeneratePublicKey(BigInteger e, BigInteger n)        // public key
        {
            while (e > 1)
            {
                BigInteger ef = BigInteger.Pow(e, f);
                ef = 1 % n;
            }
            return e;


        }

        public void GeneratePrivateKey(BigInteger e)                  //private key
        {


        }

        static (BigInteger, BigInteger, BigInteger) gcdex(BigInteger a, BigInteger b)
        {
            if (a == 0)
                return (b, 0, 1);
            (BigInteger gcd, BigInteger x, BigInteger y) = gcdex(b % a, a);
            return (gcd, y - (b / a) * x, x);
        }

        // поиск обратного в поле
        public BigInteger invmod(BigInteger a, BigInteger n)
        {
            BigInteger m = n;
            (BigInteger g, BigInteger x, BigInteger y) = gcdex(a, m);
            return g > 1 ? 0 : (x % m + m) % m;
        }
    }
}
