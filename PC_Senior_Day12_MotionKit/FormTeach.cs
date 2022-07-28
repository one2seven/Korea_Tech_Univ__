using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
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
            Display_teach();

            
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


        private void Display_teach()
        {
            tbXY_Pos01.Text = $"X: {_cart.XY_Pt[0].X.ToString()}\r\nY: {_cart.XY_Pt[0].Y.ToString()}";
            tbXY_Pos02.Text = $"X: {_cart.XY_Pt[1].X.ToString()}\r\nY: {_cart.XY_Pt[1].Y.ToString()}";
            tbXY_Pos03.Text = $"X: {_cart.XY_Pt[2].X.ToString()}\r\nY: {_cart.XY_Pt[2].Y.ToString()}";
            tbXY_Pos04.Text = $"X: {_cart.XY_Pt[3].X.ToString()}\r\nY: {_cart.XY_Pt[3].Y.ToString()}";
            tbXY_Pos05.Text = $"X: {_cart.XY_Pt[4].X.ToString()}\r\nY: {_cart.XY_Pt[4].Y.ToString()}";
            tbXY_Pos06.Text = $"X: {_cart.XY_Pt[5].X.ToString()}\r\nY: {_cart.XY_Pt[5].Y.ToString()}";
            tbXY_Pos07.Text = $"X: {_cart.XY_Pt[6].X.ToString()}\r\nY: {_cart.XY_Pt[6].Y.ToString()}";
            tbXY_Pos08.Text = $"X: {_cart.XY_Pt[7].X.ToString()}\r\nY: {_cart.XY_Pt[7].Y.ToString()}";
            tbZ_Pos01.Text = $"Z_U:{_cart.ZPt[0].ToString()}";
            tbZ_Pos02.Text = $"Z_D:{_cart.ZPt[1].ToString()}";

        }

        private bool Zero_Check()
        {
            if (_cart.Origin_Check)
            {
                return false;
            }
            else
            {
                MessageBox.Show("원점복귀가 되지않았습니다.");
                return true;
            }


        }

        private void btnXY_PosSave_Click(object sender, EventArgs e)
        {
            if (Zero_Check()) return;
     
            _cart.XY_PosSave();
            Display_teach();
            _cart.Teaching_Save();
           
        }

        private void btnZ_Save_Click(object sender, EventArgs e)
        {
            if (Zero_Check()) return;

            Button btn = sender as Button;

            int p_num = int.Parse(btn.Tag.ToString());

            _cart.Z_PosSave(p_num);
            Display_teach();
            _cart.Teaching_Save();

        }

        private void btnXY_PosMove_Click(object sender, EventArgs e)
        {
            if (Zero_Check()) return;

            int ps;
            if (int.TryParse(cbXY_PosSelect.SelectedIndex.ToString(),out ps))
                _cart.XY_PosMove(ps);

        }

        private void btnZ_PosMove_Click(object sender, EventArgs e)
        {
            if (Zero_Check()) return;

            int ps;
            if (int.TryParse(cbZ_PosSelect.SelectedIndex.ToString(), out ps))
                _cart.Z_PosMove(ps);

        }

        private void btnPnPMove_Click(object sender, EventArgs e)
        {
            if (Zero_Check()) return;

            int start_p; 
            int end_p;

            if (int.TryParse(cbPnPStartPos.SelectedIndex.ToString(), out start_p)
                && int.TryParse(cbPnPEndPos.SelectedIndex.ToString(), out end_p))
            {
                _cart.PickupPlace(start_p, end_p);
            }

        }

        private void btnCylOpen_Click(object sender, EventArgs e)
        {
            _cart.Gripper.Open();

        }

        private void btnCylClose_Click(object sender, EventArgs e)
        {
            _cart.Gripper.Close();


        }
    }
}
