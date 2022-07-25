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
using Motion;

namespace PC_Senior_Day12_MotionKit
{
    public partial class FormTeach : Form
    {
        ITeach _cart;
        public FormTeach(ITeach kit)
        {
            InitializeComponent();
            _cart = kit;
            
           
        }


        private void btnJogMove_MouseDown(object sender, MouseEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl == null)
                return;
            switch (Convert.ToString(ctrl.Tag))
            {
                case "Up":
                    _cart.IAxes[2].JogMove(Dir.UP);
                    break;
                case "Down":
                    _cart.IAxes[2].JogMove(Dir.DOWN);
                    break;
                case "Fwd":
                    _cart.IAxes[1].JogMove(Dir.FWD);
                    break;
                case "Bwd":
                    _cart.IAxes[1].JogMove(Dir.BWD);
                    break;
                case "Left":
                    _cart.IAxes[0].JogMove(Dir.LEFT);
                    break;
                case "Right":
                    _cart.IAxes[0].JogMove(Dir.RIGHT);
                    break;
            }
        }
        private void btnJogMove_MouseUp(object sender, MouseEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl == null)
                return;
            switch (Convert.ToString(ctrl.Tag))
            {
                case "Up":
                case "Down":
                    _cart.IAxes[2].JogStop();
                    break;
                case "Fwd":
                case "Bwd":
                    _cart.IAxes[1].JogStop();
                    break;
                case "Left":
                case "Right":
                    _cart.IAxes[0].JogStop();
                    break;
            }
        }

        private void Inch_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            int Axis = int.Parse(Convert.ToString(btn.Tag).Split(',')[0]);
            double Dist = double.Parse(Convert.ToString(btn.Tag).Split(',')[1]);

            if (Dist == 100)
            {
                if (Axis == 0) Dist = double.Parse(tbAx_X_Inch_Dist.Text);
                else if (Axis == 1) Dist = double.Parse(tbAx_Y_Inch_Dist.Text);
                else if (Axis == 2) Dist = double.Parse(tbAx_Z_Inch_Dist.Text);
            }

            _cart.IAxes[Axis].InchMove(Dist);
        }
    }
}
