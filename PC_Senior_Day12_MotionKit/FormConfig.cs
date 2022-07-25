using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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

        private void btnAx_Save_Click(object sender, EventArgs e)
        {
            string path = @"Parameter.txt";

            Button btn = sender as Button;
            Control ct = btn.Parent;            

            using (FileStream fs = File.Create(path))
            {

                foreach (var ctr in ct.Controls)
                {
                    if (ctr is TextBox)
                    {
                        TextBox tb = ctr as TextBox;
                        StringBuilder sb = new StringBuilder();
                        string Axis_ch = tb.Name.ToString().Split('_')[1];
                        string para_name = tb.Name.ToString().Split('_')[2];
                        string value = tb.Text.ToString();
                        int axis = Axis_ch == "X" ? 0 : Axis_ch == "Y" ? 1 : 2;

                        sb.AppendLine($"{Axis_ch}_{para_name}={value}");

                        AddText(fs,sb.ToString());
                    }
                }


            


            }

            void AddText(FileStream fs, string value)
            {
                byte[] info = new UTF8Encoding(true).GetBytes(value);
                fs.Write(info, 0, info.Length);
            }

        }


    }
}
