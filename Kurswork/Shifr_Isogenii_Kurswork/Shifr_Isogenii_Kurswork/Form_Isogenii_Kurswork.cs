using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shifr_Isogenii_Kurswork
{
    public partial class Form_Isogenii_Kurswork : Form
    {
        public Form_Isogenii_Kurswork()
        {
            InitializeComponent();
        }
        Field_p F;
        private void button_encrypt_message_Click(object sender, EventArgs e)
        {
            Int64 A = 513, B = 342;UInt32 P = 1009;

            F = new Field_p(P);
            Elipt_Curve El = new Elipt_Curve(A, B, P);
            Elipt_Curve E2 = new Elipt_Curve(F.get_Umn(A,F.get_Pow(F.N,2)), F.get_Umn(B, F.get_Pow(F.N, 3)), P);
        }

        private void button_decrypt_message_Click(object sender, EventArgs e)
        {

        }
    }
}
