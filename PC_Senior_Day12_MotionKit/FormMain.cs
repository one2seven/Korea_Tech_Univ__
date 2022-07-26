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
    public partial class FormMain : Form
    {
        MotionKit cart;
        Msg cmdZR, rptZR;
        short axis_num;
        public FormMain()
        {
            InitializeComponent();
            cart = new MotionKit();
        }
        private void FormMain_Load(object sender, EventArgs e)
        {
            short retErr = cart.Init();
            if (retErr != 0)
                lbMsg.Text = "MMC 보드 초기화 실패 Code : " + retErr.ToString();    
            else
                lbMsg.Text = "MMC 보드 초기화 성공";

            timer_MotionKitInfo_Update.Start();
        }
        private void btnFormConfig_Click(object sender, EventArgs e)
        {
            FormConfig frm = new FormConfig(cart);
            frm.ShowDialog();
        }

        private void btnFormTeaching_Click(object sender, EventArgs e)
        {
            FormTeach frm = new FormTeach(cart);
            frm.ShowDialog(); 
        }

        private void btnFormDIO_Click(object sender, EventArgs e)
        {
            FormDIO frm = new FormDIO(cart.Dio);
            frm.ShowDialog();
        }

        private void ServoOn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                int axNo = Convert.ToInt32(btn.Tag);
                cart[axNo].ServoOn(); 
            }
        }
        private void ServoOff_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                int axNo = Convert.ToInt32(btn.Tag);
                cart[axNo].ServoOff();
            }
        }
        private void Clear_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                int axNo = Convert.ToInt32(btn.Tag);
                cart[axNo].Clear();
            }
        }
        private void Reset_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                int axNo = Convert.ToInt32(btn.Tag);
                cart[axNo].AmpReset();
            }
        }

        private void timer_MotionKitInfo_Update_Tick(object sender, EventArgs e)
        {
            cart.InfoUpdate();

            cbAx_X_Servo_On.Checked = cart[0].ServoEnable;
            cbAx_X_Servo_Off.Checked = !cart[0].ServoEnable;
            cbAx_Y_Servo_On.Checked = cart[1].ServoEnable;
            cbAx_Y_Servo_Off.Checked = !cart[1].ServoEnable;
            cbAx_Z_Servo_On.Checked = cart[2].ServoEnable;
            cbAx_Z_Servo_Off.Checked = !cart[2].ServoEnable;

            tbAx_X_ComPos.Text = cart[0].CmdPos.ToString();
            tbAx_X_CurrPos.Text = cart[0].ActPos.ToString();
            tbAx_X_Encoder.Text = cart[0].Encoder.ToString();
            tbAx_X_ErrPos.Text = cart[0].ErrPos.ToString();
            tbAx_X_Velocity.Text = cart[0].ActSpd.ToString();


            tbAx_Y_ComPos.Text = cart[1].CmdPos.ToString();
            tbAx_Y_CurrPos.Text = cart[1].ActPos.ToString();
            tbAx_Y_Encoder.Text = cart[1].Encoder.ToString();
            tbAx_Y_ErrPos.Text = cart[1].ErrPos.ToString();
            tbAx_Y_Velocity.Text = cart[1].ActSpd.ToString();


            tbAx_Z_ComPos.Text = cart[2].CmdPos.ToString();
            tbAx_Z_CurrPos.Text = cart[2].ActPos.ToString();
            tbAx_Z_Encoder.Text = cart[2].Encoder.ToString();
            tbAx_Z_ErrPos.Text = cart[2].ErrPos.ToString();
            tbAx_Z_Velocity.Text = cart[2].ActSpd.ToString();

            cbAx_X_Home.Checked = cart[0].Home;
            cbAx_X_NLimit.Checked = cart[0].RLS;
            cbAx_X_PLimit.Checked = cart[0].FLS;

            cbAx_Y_Home.Checked = cart[1].Home;
            cbAx_Y_NLimit.Checked = cart[1].RLS;
            cbAx_Y_PLimit.Checked = cart[1].FLS;

            cbAx_Z_Home.Checked = cart[2].Home;
            cbAx_Z_NLimit.Checked = cart[2].RLS;
            cbAx_Z_PLimit.Checked = cart[2].FLS;


        }

        private void btn_Servo_Init_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 3; i++) 
            {
                cart[i].ServoOff();                
                cart[i].AmpReset();
                cart[i].Clear();
                cart[i].ServoOn();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            cart.Origin_Check = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            cart[1].origin();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            cart[2].origin();
        }

        private void btn_ZR_Click(object sender, EventArgs e)
        {
            cmdZR = Msg.START;
            axis_num = 0;
            Timer_ZR.Start();   
        }

        private void button1_Click(object sender, EventArgs e)
        {

            DialogResult dr = MessageBox.Show("프로그램을 종료 하시겠습니까?",
                                              "프로그램 종료 메세지",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Question);
            if (dr == DialogResult.Yes) Application.Exit();
        }

        private void btn_Init_Postion_Click(object sender, EventArgs e)
        {
            if (cart.Origin_Check == false)
            {
                MessageBox.Show("원점복귀가 되지않았습니다.");
            }

            cart.MoveInitPos();
        }

        private void Timer_ZR_Tick(object sender, EventArgs e)
        {
            if (axis_num > 2)
            {
                Timer_ZR.Stop();
            }

   
            rptZR = cart.ZeroReturn(cmdZR, axis_num);
            if (rptZR == Msg.READY && cmdZR == Msg.CHECK)
                cmdZR = Msg.START;
            else if (rptZR == Msg.BUSY)
                cmdZR = Msg.CHECK;
            else if (rptZR == Msg.COMPLETE)
            {
                cmdZR = axis_num <2? Msg.START:Msg.END;
                cart.ZR_Step = 0;
                axis_num++;
            }
            else if (rptZR == Msg.READY && cmdZR == Msg.END)
            {
                cmdZR = Msg.CHECK;
                Timer_ZR.Stop();
                cart.Origin_Check = true;
            }

        }
    }
}
