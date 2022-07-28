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

        Dictionary<string,TextBox> pair = new Dictionary<string,TextBox>();
        string path; 
        public FormConfig(IConfig cart)
        {
            InitializeComponent();
            path = @"f:\Parameter.txt";

            this.cart = cart;
            pair = new Dictionary<string, TextBox>();
            Init_config();
            
        }


        private void Init_config()
        {
            foreach (var ct in Controls)
            {
                if (ct is GroupBox)
                {
                    GroupBox gb = ct as GroupBox;

                    foreach (var ctr in gb.Controls)
                    {
                        if (ctr is TextBox)
                        {
                            TextBox tb = ctr as TextBox;
                            string key = tb.Name.ToString();
                            pair[key] = tb;
                        }
                    }
                }
            }

            File_Read();
            Apply_para();
        }

        private void Apply_para()
        {
            foreach(var value in pair.Values)
            {
                if (value.Text == "")
                {
                    return;
                }
            }

            for (int i=0; i<3; i++)
            {
                string axis = i == 0 ? "X" : i == 1 ? "Y" : "Z";
                string key = $"tbAx_{axis}_";

                cart.CfgAxInfo[i].InchSpd = double.Parse(pair[$"{key}InchSpd"].Text.ToString());
                cart.CfgAxInfo[i].InchAcc = short.Parse(pair[$"{key}InchAcc"].Text.ToString());
                cart.CfgAxInfo[i].InchDec = short.Parse(pair[$"{key}InchDec"].Text.ToString());
                cart.CfgAxInfo[i].JogSpd = double.Parse(pair[$"{key}JogSpd"].Text.ToString());
                cart.CfgAxInfo[i].JogAcc = short.Parse(pair[$"{key}JogAcc"].Text.ToString());
                cart.CfgAxInfo[i].JogDec = short.Parse(pair[$"{key}JogDec"].Text.ToString());
                cart.CfgAxInfo[i].PosSpd = double.Parse(pair[$"{key}PosSpd"].Text.ToString());
                cart.CfgAxInfo[i].PosAcc = short.Parse(pair[$"{key}PosAcc"].Text.ToString());
                cart.CfgAxInfo[i].PosDec = short.Parse(pair[$"{key}PosDec"].Text.ToString());
            }

            cart.PosMovSpd = double.Parse(pair[$"tbAx_X_PosSpd"].Text.ToString());
            cart.PosMovAcc = short.Parse(pair[$"tbAx_X_PosAcc"].Text.ToString());
            cart.PosMovDec = short.Parse(pair[$"tbAx_X_PosDec"].Text.ToString());
            cart.ZRSpd3 = double.Parse(pair[$"tbAx_X_OrgSpd3"].Text.ToString());
            cart.ZRSpd2 = double.Parse(pair[$"tbAx_X_OrgSpd2"].Text.ToString());
            cart.ZRSpd1 = double.Parse(pair[$"tbAx_X_OrgSpd1"].Text.ToString());
            cart.ZRAcc1 =  short.Parse(pair[$"tbAx_X_OrgAcc"].Text.ToString());
            cart.ZRAcc2 =  short.Parse(pair[$"tbAx_X_OrgAcc"].Text.ToString());
            cart.ZRAcc3 =  short.Parse(pair[$"tbAx_X_OrgAcc"].Text.ToString());
            cart.ZRDec1 =  short.Parse(pair[$"tbAx_X_OrgDec"].Text.ToString());
            cart.ZRDec2 =  short.Parse(pair[$"tbAx_X_OrgDec"].Text.ToString());
            cart.ZRDec3 =  short.Parse(pair[$"tbAx_X_OrgDec"].Text.ToString());        

        }

        private void File_Read()
        {
            try
            {
                string[] para_file = File.ReadAllLines(path);

                foreach (string para in para_file)
                {
                    string key = para.Split('=')[0];
                    string val = para.Split('=')[1];

                    pair[key].Text = val;
                }
            }

            catch (FileNotFoundException e)
            {
                MessageBox.Show($"파일 경로 확인바랍니다 : {e.Message}");
            }
            catch
            {

            }
        }


        private void File_Save()
        {          

            using (FileStream fs = File.Create(path))
            {
                foreach (TextBox tb in pair.Values)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"{tb.Name.ToString()}={tb.Text.ToString()}");
                    AddText(fs, sb.ToString());
                }
            }
            void AddText(FileStream fs, string value)
            {
                byte[] info = new UTF8Encoding(true).GetBytes(value);
                fs.Write(info, 0, info.Length);
            }

            Apply_para();
        }



        private void btnAx_Save_Click(object sender, EventArgs e)
        {

            Button btn = sender as Button;

            string Axis = btn.Name.ToString().Split('_')[1];

            string[] para_file = File.ReadAllLines(path);

            foreach (string para in para_file)
            {
                string key = para.Split('=')[0];
                string val = para.Split('=')[1];

                if (key.ToString().Split('_')[1] == Axis)
                    continue;

                pair[key].Text = val;
            }



            File_Save();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            File_Save();
            File_Read();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();

        }
    }
}
