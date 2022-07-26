using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using Motion;
using DigitalIO;
using MMCWHPNET;
using System.IO;

namespace MainControl
{
    public class MotionKit : IConfig, ITeach
    {
        #region Field
        private Servo[] _axes;
        private Cylinder _gripper;
        private DIO _dio;
        private Point[] _xyPt;
        private int[] _zPt;
        private double _zrSpd1, _zrSpd2, _zrSpd3, tempJogSpd;
        private short _zrAcc1, _zrAcc2,  _zrAcc3, tempJogAcc;
        private short _zrDec1, _zrDec2, _zrDec3, tempJogDec;
        private Msg zrRpt;
        private int zrStep;
        double _posMovSpd;
        short _posMovAcc;
        short _posMovDec;
        bool _orgin_check;

        #endregion
        #region Property
        /// <summary>
        /// 서보 모터 접근 인덱서
        /// </summary>
        /// <param name="axNo">서보 축 번호 : 0(X), 1(Y), 2(Z)</param>
        /// <returns>서보 객체</returns>
        public Servo this[int axNo]
        { get { return _axes[axNo]; } }
        /// <summary>
        /// 실린더 그리퍼 객체
        /// </summary>
        public Cylinder Gripper
        { get { return _gripper; } }
        /// <summary>
        /// 원점 복귀 1단계 속도 : Home 감지 단계
        /// </summary>
        public double ZRSpd1
        {
            get { return _zrSpd1; }
            set { _zrSpd1 = value; }
        }
        /// <summary>
        /// 원점 복귀 2단계 속도 : Home 센서 리턴
        /// </summary>
        public double ZRSpd2
        {
            get { return _zrSpd2; }
            set { _zrSpd2 = value; }
        }
        /// <summary>
        /// 원점 복귀 3단계 속도 : Marking 
        /// </summary>
        public double ZRSpd3
        {
            get { return _zrSpd3; }
            set { _zrSpd3 = value; }
        }
        /// <summary>
        /// 원점 복귀 1단계 가속 시간 
        /// </summary>
        public short ZRAcc1
        { get { return _zrAcc1; } set { _zrAcc1 = value; } }
        /// <summary>
        /// 원점 복귀 2단계 가속 시간 
        /// </summary>
        public short ZRAcc2
        { get { return _zrAcc2; } set { _zrAcc2 = value; } }
        /// <summary>
        /// 원점 복귀 3단계 가속 시간 
        /// </summary>
        public short ZRAcc3
        { get { return _zrAcc3; } set { _zrAcc3 = value; } }
        /// <summary>
        /// 원점 복귀 1단계 감속 시간 
        /// </summary>
        public short ZRDec1
        {
            get { return _zrDec1; }
            set { _zrDec1 = value; }
        }
        /// <summary>
        /// 원점 복귀 2단계 감속 시간 
        /// </summary>
        public short ZRDec2
        {
            get { return _zrDec2; }
            set { _zrDec2 = value; }
        }
        /// <summary>
        /// 원점 복귀 3단계 감속 시간 
        /// </summary>
        public short ZRDec3
        {
            get { return _zrDec3; }
            set { _zrDec3 = value; }
        }
        /// <summary>
        /// Form Config에서 사용할 규약
        /// </summary>
        public ICfgServo[] CfgAxInfo
        {
            get { return _axes; }
        }
        /// <summary>
        /// Digtal IO 객체 
        /// </summary>
        public DIO Dio
        {
            get { return _dio; }
        }
        /// <summary>
        /// 서보 모터 전체 접근 : Array
        /// </summary>
        public Servo[] Axes
        {
            get { return _axes; }
        }
        /// <summary>
        /// MotionKit의 XY Table 좌표 : 8개
        /// </summary>
        public Point[] XY_Pt
        {
            get { return _xyPt; }
        }

        public int[] ZPt
        { get { return _zPt; } }
        /// <summary>
        /// 축별 서보 접근 : ITeachServo 적용한 접근
        /// </summary>
        public ITeachServo[] IAxes
        { get { return _axes; } }

        public int ZR_Step { get { return zrStep; } set { zrStep = value; } }

        public double PosMovSpd { get => _posMovSpd; set => _posMovSpd = value; }
        public short PosMovAcc { get => _posMovAcc; set => _posMovAcc = value; }
        public short PosMovDec { get => _posMovDec; set => _posMovDec = value; }

        public bool Origin_Check { get { return _orgin_check; } set { _orgin_check = value; } }


        #endregion
        #region Method
        /// <summary>
        /// 생성자
        /// </summary>
        public MotionKit()
        {
            _axes = new Servo[3];
            for (short i = 0; i < _axes.Length; i++)
                _axes[i] = new Servo(i);
            _gripper = new Cylinder();
            _dio = new DIO();
            _xyPt = new Point[8];
            _zPt = new int[2];
            zrRpt = Msg.READY;
            zrStep = 0;
            _zrSpd1 = 10000;
            _zrAcc1 = 10;
            _zrDec1 = 10;
            _zrSpd2 = 1000;
            _zrAcc2 = 5;
            _zrDec2 = 5;
            _zrSpd3 = 500;
            _zrAcc3 = 5;
            _zrDec3 = 5;
            _posMovSpd = 30000;
            _posMovAcc = 10;
            _orgin_check = false;

            Teaching_Load();

        }

        

        public short Init()
        {
            short ret = bdInit();
            if (ret == 0)
            {
                for (int i = 0; i < _axes.Length; i++) _axes[i].Init();
            }
            return ret;
        }

        private static short bdInit()
        {
            short bdNum = 1;
            long[] addr = new long[1] { 0xD8000000 };
            short retErr = MMCLib.mmc_initx(bdNum, addr);
            return retErr;
        }

        /// <summary>
        /// 원점복귀
        /// </summary>
        public Msg ZeroReturn(Msg cmd, short Axis)
        {
            switch (zrStep)
            {
                case 0://ready
                    if (cmd == Msg.START)
                    {
                        zrRpt = Msg.BUSY;
                        zrStep = 1;
                        tempJogSpd = _axes[Axis].JogSpd;
                        tempJogAcc = _axes[Axis].JogAcc;
                        tempJogDec = _axes[Axis].JogDec;
                        _axes[Axis].Set_PSW_Limit(2000000);
                        _axes[Axis].Set_NSW_Limit(-2000000);
                    }
                    break;
                case 1: //start : find home
                    _axes[Axis].JogSpd = _zrSpd1;
                    _axes[Axis].JogAcc = _zrAcc1;
                    _axes[Axis].JogDec = _zrDec1;
                    _axes[Axis].JogMove(Dir.LEFT);
                    zrStep = 2;
                    break;           

                case 2:
                    if (_axes[Axis].Home == true)
                    {
                        _axes[Axis].JogStop();
                        _axes[Axis].JogSpd = _zrSpd2;
                        _axes[Axis].JogAcc = _zrAcc2;
                        _axes[Axis].JogDec = _zrDec2;
                        zrStep = 3;
                    }
                    else if (_axes[Axis].RLS == true)
                    {
                        double H_dist = Axis == 0 ? 25 : Axis==1 ? 35 : 30;
                        int time = (int)H_dist * 2*100; 

                        _axes[Axis].InchMove(H_dist);
                        Thread.Sleep(time);
                        
                        zrStep = 1;

                    }
                        break;
                case 3:
                    if (MMCLib.axis_done(0) == 1)
                    {
                        _axes[Axis].JogMove(Dir.RIGHT);
                        zrStep = 4;
                    }
                    break;
                case 4:
                    if (_axes[Axis].Home == false)
                    {
                        _axes[Axis].JogStop();
                        _axes[Axis].JogSpd = _zrSpd3;
                        _axes[Axis].JogAcc = _zrAcc3;
                        _axes[Axis].JogDec = _zrDec3;
                        zrStep = 5;
                    }
                    break;
                case 5:
                    if (MMCLib.axis_done(Axis) == 1)
                    {
                        _axes[Axis].JogMove(Dir.LEFT);
                        zrStep = 6;
                    }
                    break;
                case 6:
                    if (_axes[Axis].Home == true)
                    {
                        _axes[Axis].JogStop();
                        zrStep = 7;
                    }
                    break;
                case 7:
                    if (MMCLib.axis_done(0) == 1)
                    {
                        _axes[Axis].JogSpd = tempJogSpd;
                        _axes[Axis].JogAcc = tempJogAcc;
                        _axes[Axis].JogDec = tempJogDec;
                        _axes[Axis].Clear(); //*******
                        _axes[Axis].Limit_Set();
                        zrStep = 100;
                    }
                    break;
                case 100:// end
                    zrRpt = Msg.COMPLETE;
                    zrStep = 101;
                    _orgin_check = true;
                    break;
                case 101:
                    if (cmd == Msg.END)
                    {
                        zrStep = 0;
                        zrRpt = Msg.READY;               
                    }
                    break;
            }
            return zrRpt;
        }
        /// <summary>
        /// 초기 위치 이동
        /// </summary>
        /// 
   
        public void MoveInitPos()
        {
            if (Origin_Check == false) return;

            Axes[0].PosMove(XY_Pt[0].X);
            Axes[1].PosMove(XY_Pt[0].Y);
            Axes[2].PosMove(ZPt[0]);

        }
        /// <summary>
        /// 모션 키트 정보 갱신
        /// </summary>
        public void InfoUpdate()
        {
            _axes[0].InfoUpdate();
            _axes[1].InfoUpdate();
            _axes[2].InfoUpdate();
        }

        public void Teaching_Load()
        {
            string path = @"f:\Teaching.txt";

            string[] fs = File.ReadAllLines(path);
            
            foreach(string line in fs)
            {
                string[] temp = line.Split(':');
                int num = int.Parse(temp[0].Split('_')[1]);
                string val = temp[1];

                //Position_8:{X=84000,Y=28000}
                Point pp = new Point();

                if (num < 9)
                {
                    pp.X = int.Parse(val.Split(',')[0].Split('=')[1].ToString());
                    pp.Y = int.Parse(val.Split(',')[1].Split('=')[1].Split('}')[0].ToString());

                    XY_Pt[num - 1] = pp;
                }
                else
                {
                    ZPt[num - 9] = int.Parse(val);
                }

            }
        }

        public void Teaching_Save()
        {
            string path = @"f:\Teaching.txt";
            StringBuilder sb = new StringBuilder();

            using (FileStream fs = File.Create(path))
            {
                for(int i =0; i<XY_Pt.Length; i++)
                {
                    sb.AppendLine($"Position_{i + 1}:{XY_Pt[i].ToString()}");                    
                }

                Addfile(fs, sb.ToString());
                Addfile(fs, $"Position_9:{ZPt[0]}\r\n");
                Addfile(fs, $"Position_10:{ZPt[1]}");
            }

            void Addfile(FileStream fs, string value)
            {
                byte[] info = new UTF8Encoding(true).GetBytes(value);
                fs.Write(info, 0, info.Length);
            }

        }




        /// <summary>
        /// XY Table 좌표 이동
        /// </summary>
        /// <param name="ptNo">좌표 번호 : 1 ~ 8</param>
        public void XY_PosMove(int ptNo)
        {

            if (ptNo < 0) return;

            //Axes[0].PosMove(XY_Pt[ptNo].X);
            //Axes[1].PosMove(XY_Pt[ptNo].Y);

            short axNum = 2;
            short[] axMap = new short[2] { 0, 1 };
            MMCLib.map_axes(axNum, axMap);
            MMCLib.set_move_speed(_posMovSpd);
            MMCLib.set_move_accel(_posMovAcc);
            MMCLib.move_2(XY_Pt[ptNo].X, XY_Pt[ptNo].Y);

        }
        /// <summary>
        /// XY 좌표 저장
        /// </summary>
        public void XY_PosSave()
        {
            Point start_p = new Point();

            start_p.X = int.Parse(Axes[0].ActPos.ToString());
            start_p.Y = int.Parse(Axes[1].ActPos.ToString());

            for ( int i=0; i<8; i++)
            {
                XY_Pt[i].X = start_p.X + (i%4)*28*1000;                
                XY_Pt[i].Y = start_p.Y + (i/4)*28*1000;
            }
        }
        /// <summary>
        /// Z축 좌표 이동
        /// </summary>
        /// <param name="ptNo">좌표 번호 : 1 ~ 2</param>
        public void Z_PosMove(int ptNo)
        {
            if (ptNo < 0) return;

            Axes[2].PosMove(ZPt[ptNo]);
        }
        /// <summary>
        /// Z축 좌표 저장
        /// </summary>
        public void Z_PosSave(int ptNo)
        {
           ZPt[ptNo]= int.Parse(Axes[2].ActPos.ToString());

        }
        /// <summary>
        /// 소재 이동
        /// </summary>
        /// <param name="startPtNo">시작 위치 : 1 ~ 8</param>
        /// <param name="destPtNo">도착 위치 : 1 ~ 8</param>
        public void PickupPlace(int startPtNo, int destPtNo)
        {

            if (startPtNo < 0 || destPtNo<0) return;

            Z_PosMove(0);
            WaitMove();

            XY_PosMove(startPtNo);
            WaitMove();

            Z_PosMove(1);
            WaitMove();

            Z_PosMove(0);
            WaitMove();

            XY_PosMove(destPtNo);
            WaitMove();

            Z_PosMove(1);
            WaitMove();

            Z_PosMove(0);
        }

        void WaitMove()
        {

            int cnt = 0;
            while (true)
            {
                Axes[0].InfoUpdate();
                Axes[1].InfoUpdate();
                Axes[2].InfoUpdate();

                if (Axes[0].AxisDone && Axes[1].AxisDone && Axes[2].AxisDone)
                    break;
                else
                {
                    int sss = 0;
                }

                Thread.Sleep(100);
                cnt++;
                if (cnt > 100)
                {
                    break;
                }

            }

        }

        #endregion


    }

    public enum Msg
    {
        CHECK, START, END, READY, BUSY, COMPLETE
    }
}
