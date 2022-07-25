using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MainControl; 

namespace PC_Senior_Day12_MotionKit
{
    public partial class FormConfig : Form
    {
        IConfig cart; 
        public FormConfig(IConfig cart)
        {
            InitializeComponent();
            this.cart = cart;
        }

        private void btnAx_X_Save_Click(object sender, EventArgs e)
        {
            foreach ( var ctr in Controls)
            {
                TextBox tb = ctr as TextBox;

                if (tb.Tag.ToString() == "X")
                {

                }

            }


        }
    }
}
