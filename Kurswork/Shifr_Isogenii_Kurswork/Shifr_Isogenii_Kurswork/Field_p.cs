using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shifr_Isogenii_Kurswork
{
    internal class Field_p
    {
        // характеристика поля
        UInt32 p;
        public Int64 N;
        public Field_p(UInt32 p1) { 
            p=p1;
            N = QNV();
        }
        // вычисление квадратичного невычета в поле
        public Int64 QNV()
        {
            Int64 N1 = 3;
            while (Leg(N1, p) != -1)
                N1++;
            return N1;
        }
        // сумма в поле
        public Int64 get_Sum(Int64 a, Int64 b, Int64 c)
        {
            Int64 s = ((a + b + c) % p + p) % p;
            return s;
        }
        //умножение в поле
        public Int64 get_Umn(Int64 a, Int64 b)
        {
            switch (b)
            {
                case 1:
                    return a;
                case 2:
                    return get_Umn2(a);
                
                default:
                    Int64 r = 0;
                    Int64 a1 = b; 
                    while (a1 > 0)
                    {
                        Int64 r1 = a;
                        int k = 0;
                        Boolean z = true;
                        while (z)
                            if (Math.Pow(2, k) <= a1)
                                k++;
                            else z = false;
                        for (int i = 0; i < k - 1; i++)//
                            r1 = get_Umn2(r1);

                        r = get_Sum(r,r1,0);//
                        a1 = a1 - (Int64)Math.Pow(2, k - 1);
                    }

                    return r;

            }

        }
        public Int64 get_Umn2(Int64 a)
        {
            Int64 s = ((a * 2) % p + p) % p;
            return s;
        }
        
        // возведение в степень в поле а^b
        public Int64 get_Pow(Int64 a, Int64 b)
        {
            Int64 s = 1;
            if (b < 0)
            {
                a = invmod((int)a);
                b = Math.Abs(b);
            }
              
            for(int i = 0; i < b; i++)
               s = get_Umn(a, s);
            
            return s;
        }

        public Int64 Leg(Int64 a, Int64 b)
        {
            if(a>b)
                return Leg(a%b, b);
            if (a == 1)
                return 1;
            if (a == -1)
                return (Int64)Math.Pow(-1, (b - 1) / 2);
            if (a == 2)
                return (Int64)Math.Pow(-1, (b*b - 1) / 8);
            if (a%2==0)
            {
                Int64 a1 = a;
                int k = 0;
                Boolean z = true;
                while (z)
                {
                    if (a1 % 2 == 0)
                    {
                        k++;
                        a1 /= 2;
                    }
                    else z = false;
                }
                
                if(k%2==0)
                    return Leg(a1,b);
                else
                    return (Int64)Math.Pow(-1, (b * b - 1) / 8) * Leg(a1, b);

            }
            return (Int64)Math.Pow(-1, (b - 1) * (a - 1) / 4)*Leg(b, a);
        }

        static (int, int, int) gcdex(int a, int b)
        {
            if (a == 0)
                return (b, 0, 1);
            (int gcd, int x, int y) = gcdex(b % a, a);
            return (gcd, y - (b / a) * x, x);
        }

        // поиск обратного в поле
        public int invmod(int a)
        {
            int m = (int) p;
            (int g, int x, int y) = gcdex(a, m);
            return g > 1 ? 0 : (x % m + m) % m;
        }

        // поиск квадратичного вычета: y^2= a mod p -  поиск у
        public (Int64, Int64) qv_y(Int64 a)
        {
            if (a == 0) return (0,0);
            if (Leg(a, p) == 1)
            {
                int ost_4 = (int)p % 4;
                int ost_8 = (int)p % 8;
                int m = ((int)p - ost_4) / 4;
                int m8 = ((int)p - ost_8) / 8;
                if (ost_4 == 3)
                {
                    Int64 x = get_Pow(a, m + 1);
                    return (x, get_Sum(0, -1 * x, 0));
                }
                else
                {
                    m = m8;
                    if (ost_8 == 5)
                    {
                        Int64 x;
                        if (get_Pow(a, 2 * m + 1) == 1)
                        {
                            x = get_Pow(a, m + 1);
                            return (x, get_Sum(0, -1 * x, 0));
                        }

                        x = get_Umn(get_Pow(2, 2 * m + 1), get_Pow(a, m + 1));
                        return (x, get_Sum(0, -1 * x, 0));

                    }
                    else
                    {// Алгоритм Тонелли-Шенкса
                        int h = (int)p - 1;
                        int s = 0;
                        while (h % 2 == 0)
                        {
                            s++;
                            h /= 2;
                        }
                        Int64 a1 = get_Pow(a, (h + 1) / 2);
                        Int64 a2 = invmod((int)a);

                        Int64 N2 = 1;
                        Int64 N1 = get_Pow(N, h);

                        Int64 b = 0, c = 0, d = 0;
                        for (int i = 0; i < s; i++)
                        {
                            b = get_Umn(a1, N2);
                            c = get_Umn(a2, get_Pow(b, 2));
                            d = get_Pow(c, (Int64)Math.Pow(2, s - 2 - i));
                            if (d == p-1)
                                N2 = get_Umn(N2, get_Pow(N1, (Int64)Math.Pow(2, i)));
                        }
                        return (b, get_Sum(0, -1 * b, 0));
                    }
                }
                
            }
            return (-1, -1);

        }

    }
}
