using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shifr_Isogenii_Kurswork
{
    public class Elipt_Curve
    {
        //y^2=x^3+ax+b
        //4a^3+27b^2!=0
        Field_p F;
        // параметры элиптической кривой
        Int64 A = 0, B = 0, M = 0;
        //j-инвариант и след эндоморфизма Фробениуса
        Int64 j = 0, t=0;
        // Все точки на элиптической кривой
        List<Tuple<Int64, Int64>> arr_points;
        // нулевой элемент поля
        Tuple<Int64, Int64> O;
        public Elipt_Curve(Int64 a,Int64 b, UInt32 p) {

            A = a; B = b; M= p;
            O = Tuple.Create(M,M);
            arr_points = new List<Tuple<Int64, Int64>>();
            F = new Field_p(p);
            // проверка а и b
            Int64 s = F.get_Sum(F.get_Umn(4, F.get_Pow(a, 3)), F.get_Umn(27, F.get_Pow(b, 2)), 0);
            if (s == 0)
            {
                MessageBox.Show("It's bad!");
                return;
            }

            j = F.get_Umn(1728, F.get_Umn(F.get_Umn(4, F.get_Pow(a, 3)), F.invmod((int)s)));

            for (Int64 x =0; x<p; x++)
            {
                Int64 a1 = F.get_Sum(F.get_Pow(x, 3), F.get_Umn(a, x), b);
                (Int64 y1, Int64 y2) = F.qv_y(a1);
                if (y1 != -1)
                {
                    arr_points.Add(Tuple.Create(x, y1));
                    arr_points.Add(Tuple.Create(x, y2));
                }
                
            }
            //(Int64 y3, Int64 y4) = F.qv_y(561);

            t=p+1 - arr_points.Count();


        }


        //умножение точки на число
        public Tuple<Int64, Int64> Umn_Points(Tuple<Int64, Int64> P, Int64 a)
        {
            if (a == 0)
                return O;

            Tuple<Int64, Int64> U = P;

            if (a < 0)
            {
                P = Obr_Points(P);
                a = Math.Abs(a);
            }
            // доделать!!!!!!!!!!!!!!!!!!!!!!!!!!
            for (Int64 i = 1; i < a; i++)
            {
                U = Sum_Points(P, U);
            }

            return U;
        }


        // сложение точек на элипт кривой:

        public Tuple<Int64, Int64> Sum_Points(Tuple<Int64, Int64> P1, Tuple<Int64, Int64> P2)
        {
            if(P2 == Obr_Points(P1))
               return O;

            if (P1 == O)
                return P2;
            if (P2 == O)
                return P1;

            Int64 x1 = P1.Item1;
            Int64 y1 = P1.Item2;
            Int64 x2 = P2.Item1;
            Int64 y2 = P2.Item2;
            Int64 x3 = 0,y3 = 0;

            Int64 ly = F.get_Umn(F.get_Sum(y1, -1 * y2, 0), F.invmod((int)F.get_Sum(x1,-1* x2,0)));
            if (x1==x2&& y1 == y2)
            {
                ly = F.get_Umn(F.get_Sum(F.get_Umn(3,F.get_Pow(x1,2)), A, 0), F.invmod((int)F.get_Umn(y1, 2)));
            }
            
            

            x3 = F.get_Sum(-1 * x1, -1 * x2, ly * ly);
            y3 = F.get_Sum(-1 * y1, F.get_Umn(ly, F.get_Sum(x1, -1 * x3,0)), 0);

            return Tuple.Create(x3, y3);

        }

        //-P
        public Tuple<Int64, Int64> Obr_Points(Tuple<Int64, Int64> P)
        {
            Tuple<Int64, Int64> P1 = Tuple.Create(P.Item1, (-1 * P.Item2+M)%M);

            return P1;
        }
    }
}
