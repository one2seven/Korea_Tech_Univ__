﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using Motion;
using DigitalIO;
using MMCWHPNET; 

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
        public void MoveInitPos()
        {

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

        /// <summary>
        /// XY Table 좌표 이동
        /// </summary>
        /// <param name="ptNo">좌표 번호 : 1 ~ 8</param>
        public void XY_PosMove(int ptNo)
        {

        }
        /// <summary>
        /// XY 좌표 저장
        /// </summary>
        public void XY_PosSave()
        {

        }
        /// <summary>
        /// Z축 좌표 이동
        /// </summary>
        /// <param name="ptNo">좌표 번호 : 1 ~ 2</param>
        public void Z_PosMove(int ptNo)
        {

        }
        /// <summary>
        /// Z축 좌표 저장
        /// </summary>
        public void Z_PosSave(int ptNo)
        {

        }
        /// <summary>
        /// 소재 이동
        /// </summary>
        /// <param name="startPtNo">시작 위치 : 1 ~ 8</param>
        /// <param name="destPtNo">도착 위치 : 1 ~ 8</param>
        public void PickupPlace(int startPtNo, int destPtNo)
        {

        }

        void ITeach.Set_NSW_Limit(double pos)
        {
        }

        void ITeach.Set_PSW_Limit(double pos)
        {
        }
        #endregion


    }

    public enum Msg
    {
        CHECK, START, END, READY, BUSY, COMPLETE
    }
}