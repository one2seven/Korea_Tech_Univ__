using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DigitalIO; 

namespace PC_Senior_Day12_MotionKit
{
    public partial class FormDIO : Form
    {
        DIO _dio;
        Dictionary<string, CheckBox> din_pair;
        Dictionary<string, CheckBox> dout_pair;
        public FormDIO(DIO dio)
        {
            InitializeComponent();
            _dio = dio;
            _dio.In[0] = sts.ON;
            din_pair = new Dictionary<string, CheckBox>();
            dout_pair = new Dictionary<string, CheckBox>();
            init_dio();
            DIO_timer.Start();
           
        }

        private void init_dio()
        {
            foreach (var ctrl in this.Controls)
            {
                if (ctrl is GroupBox)
                {
                    GroupBox gb = ctrl as GroupBox;
                    foreach(var cb in gb.Controls)
                    {
                        if (cb is CheckBox)
                        {
                            CheckBox t_cb = cb as CheckBox;
                            if (t_cb.Name.ToString().Contains("IN"))
                            {
                                din_pair[$"{t_cb.Name.ToString()}"] = t_cb;
                            }
                            else
                            {
                                dout_pair[$"{t_cb.Name.ToString()}"] = t_cb;
                            }                            
                        }
                    }
                }
            }
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            DIO_timer.Stop();
            this.Close();
        }

        private void cbOUT_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
           
            short num = short.Parse(cb.Name.ToString().Substring(5, 2));

            sts bit = _dio.GetOutBit(num) == sts.ON ? sts.OFF : sts.ON;

            _dio.SetBit(num, bit);


        }

        private void DIO_timer_Tick(object sender, EventArgs e)
        {
            foreach (string key in din_pair.Keys)
            {
                int num = int.Parse(key.Substring(4, 2));
                din_pair[key].CheckState = _dio.GetBit(num)== sts.ON? CheckState.Checked: CheckState.Unchecked;
            }

            foreach (string key in dout_pair.Keys)
            {
                int num = int.Parse(key.Substring(5, 2));
                dout_pair[key].CheckState = _dio.GetOutBit(num) == sts.ON ? CheckState.Checked : CheckState.Unchecked;

            }


        }
    }
}
