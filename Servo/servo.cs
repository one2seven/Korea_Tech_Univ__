using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMCWHPNET;
using System.Threading;

namespace Motion
{
    public class Servo : Actuator, ICfgServo, ITeachServo
    {
        #region Field
        #region Limit sensor
        private bool _home;
        private bool _fls;
        private bool _rls;
        #endregion
        #region Motion Info
        private double _actPos;
        private double _cmdPos;
        private double _errPos;
        private double _actSpd;
        private double _encoder;
        private bool _svAmpEnable;
        private double _jogSpd;
        private short _jogAcc;
        private short _jogDec;
        private double _inchSpd;
        private short _inchAcc;
        private short _inchDec;
        private double _posSpd;
        private short _posAcc;
        private short _posDec;
        private double _pSWLimit, _nSWLimit;
        private bool _axisDone;

        #endregion
        #endregion
        #region Property
        /// <summary>
        /// 홈 센서 감지 상태
        /// </summary>
        public bool Home { get { return _home; } set { _home = value; } }
        /// <summary>
        /// 정방향 리밋 센서 감지 상태
        /// </summary>
        public bool FLS { get { return _fls; } set { _fls = value; } }
        /// <summary>
        /// 역방향 리밋 센서 감지 상태 
        /// </summary>
        public bool RLS { get { return _rls; } set { _rls = value; } }
        /// <summary>
        /// 현재 위치 값
        /// </summary>
        public double ActPos { get { return _actPos; } set { _actPos = value; } }
        /// <summary>
        /// 명령 위치 값
        /// </summary>
        public double CmdPos { get { return _cmdPos; } set { _cmdPos = value; } }
        /// <summary>
        /// 위치 오차 값
        /// </summary>
        public double ErrPos { get { return _errPos; } set { _errPos = value; } }
        /// <summary>
        /// 동작 속도 값
        /// </summary>
        public double ActSpd { get { return _actSpd; } set { _actSpd = value; } }
        /// <summary>
        /// 엔코더 값
        /// </summary>
        public double Encoder { get { return _encoder; } set { _encoder = value; } }
        /// <summary>
        /// 서보 모터 전원 인가 상태
        /// </summary>
        public bool ServoEnable { get { return _svAmpEnable; } set { _svAmpEnable = value; } }
        /// <summary>
        /// Jog 운전 속도
        /// </summary>
        public double JogSpd
        {
            get { return _jogSpd; }
            set { _jogSpd = value; }
        }
        /// <summary>
        /// Jog 운전 가속 시간
        /// </summary>
        public short JogAcc
        {
            get { return _jogAcc; }
            set { _jogAcc = value; }
        }
        /// <summary>
        /// Jog 운전 감속 시간
        /// </summary>
        public short JogDec
        {
            get { return _jogDec; }
            set { _jogDec = value; }
        }
        /// <summary>
        /// 인칭 운전 속도 
        /// </summary>
        public double InchSpd
        {
            get { return _inchSpd; }
            set { _inchSpd = value; }
        }
        /// <summary>
        /// 인칭 운전 가속 시간 
        /// </summary>
        public short InchAcc
        {
            get { return _inchAcc; }
            set { _inchAcc = value; }
        }
        /// <summary>
        /// 인칭 운전 감속 시간 
        /// </summary>
        public short InchDec
        {
            get { return _inchDec; }
            set { _inchDec = value; }
        }
        /// <summary>
        /// 절대 좌표 이동 속도
        /// </summary>
        public double PosSpd
        {
            get { return _posSpd; }
            set { _posSpd = value; }
        }
        /// <summary>
        /// 절대 자표 이동 가속 시간 
        /// </summary>
        public short PosAcc
        {
            get { return _posAcc; }
            set { _posAcc = value; }
        }
        /// <summary>
        /// 절대 좌표 이동 감속 시간
        /// </summary>
        public short PosDec
        {
            get { return _posDec; }
            set { _posDec = value; }
        }
        public double PSWLimit
        {
            get { return _pSWLimit; }
        }
        /// <summary>
        /// Negative Software Limit
        /// </summary>
        public double NSWLimit
        {
            get { return _nSWLimit; }
        }

        public bool AxisDone 
        { get => _axisDone;
            set { 
                if (_axisDone != value)
                {
                    _axisDone = value;
                }
                
                _axisDone = value; } 
        
        }

        #endregion
        #region Method
        /// <summary>
        /// 서보 객체 생성
        /// </summary>
        /// <param name="axNo">축 번호</param>
        public Servo(short axNo) : base(axNo)
        {
            _jogSpd = 5000;
            _jogAcc = 10;
            _jogDec = 10;
            _inchSpd = 5000;
            _inchAcc = 5;
            _inchDec = 5;
            _posSpd = 30000;
            _posAcc = 10;
            _posDec = 10;
            _pSWLimit = 2000000;
            _nSWLimit = -2000000;

        }
        public void Init()
        {
            MMCLib.set_amp_enable_level(_axNo, 0);
            MMCLib.set_stop_rate(_axNo, 10);

            const short HIGH = (short)MMCLib.MMCDef.HIGH;
            const short LOW = (short)MMCLib.MMCDef.LOW;
            const short STOP = (short)MMCLib.MMCDef.STOP_EVENT;
            const short E_STOP = (short)MMCLib.MMCDef.E_STOP_EVENT;
            const short NO_Event = (short)MMCLib.MMCDef.NO_EVENT;
            const short COOR_CW = (short)MMCLib.MMCDef.CORD_CW;
            const short COOR_CCW = (short)MMCLib.MMCDef.CORD_CCW;


            short coordDir = -1, encDir = -1;


            if (_axNo == 0)
            {
                //pSWLimit = -128310;
                //nSWLimit = 17080;
                coordDir = COOR_CCW;
                encDir = COOR_CCW;
            }
            else if (_axNo == 1)
            {
                //pSWLimit = -77000;
                //nSWLimit = 18000;
                coordDir = COOR_CW;
                encDir = COOR_CW;
            }
            else if (_axNo == 2)
            {
                //pSWLimit = -21448;
                //nSWLimit = 13820;
                coordDir = COOR_CCW;
                encDir = COOR_CCW;
            }

            MMCLib.fset_positive_sw_limit(_axNo, _pSWLimit, STOP);
            MMCLib.fset_negative_sw_limit(_axNo, _nSWLimit, STOP);
            MMCLib.fset_positive_limit(_axNo, STOP);
            MMCLib.fset_positive_level(_axNo, HIGH);
            MMCLib.fset_negative_limit(_axNo, STOP);
            MMCLib.fset_negative_level(_axNo, HIGH);
            MMCLib.fset_home(_axNo, NO_Event);
            MMCLib.fset_home_level(_axNo, LOW);
            MMCLib.fset_amp_fault_level(_axNo, HIGH);
            MMCLib.fset_amp_fault(_axNo, STOP);
            MMCLib.fset_amp_reset_level(_axNo, LOW);
            MMCLib.fset_inposition_level(_axNo, LOW);
            MMCLib.fset_coordinate_direction(_axNo, coordDir);
            MMCLib.fset_encoder_direction(_axNo, encDir);

        }

        /// <summary>
        /// Amp driver 사용 인가
        /// </summary>
        public void ServoOn()
        {
            MMCLib.set_amp_enable(_axNo, 1);
        }
        /// <summary>
        ///  Amp driver 사용 비인가
        /// </summary>
        public void ServoOff()
        {
            MMCLib.set_amp_enable(_axNo, 0);
        }
        /// <summary>
        /// 모션 정지
        /// </summary>
        public void Stop()
        {
            MMCLib.set_stop(_axNo);
            MMCLib.mmcDelay(10);
            MMCLib.clear_status(_axNo);
        }
        /// <summary>
        /// 설정되 상태 해제 및 기본 재설정
        /// </summary>
        public void Clear()
        {
            MMCLib.clear_status(_axNo);
            MMCLib.frames_clear(_axNo);
            MMCLib.set_position(_axNo, 0);
        }
        /// <summary>
        /// Amp Fault 해제 및 재설정
        /// </summary>
        public void AmpReset()
        {
            MMCLib.amp_fault_reset(_axNo);
            MMCLib.mmcDelay(10);
            MMCLib.amp_fault_set(_axNo);
        }
        /// <summary>
        /// 서보 모터 상태 갱신
        /// </summary>
        public void InfoUpdate()
        {
            short state = 0;
            MMCLib.get_amp_enable(_axNo, ref state);
            _svAmpEnable = state == 1 ? true : false;

            MMCLib.get_command(_axNo, ref _cmdPos);
            MMCLib.get_position(_axNo, ref _encoder);
            MMCLib.get_error(_axNo, ref _errPos);
            MMCLib.get_counter(_axNo, ref _actPos);
            _actSpd = MMCLib.get_velocity(_axNo);
            AxisDone= MMCLib.in_sequence(_axNo) == 0 ? true : false; 
            Update_Sensor_sts();
        }

        public void Update_Sensor_sts()
        {
            short bitMask = 1;
            short source = MMCLib.axis_source(_axNo);
            _home = (source & bitMask << 0) != 0 ? true : false;
            _fls = (source & bitMask << 1) != 0 ? true : false;
            _rls = (source & bitMask << 2) != 0 ? true : false;
        }

        public void Limit_Set()
        {

            if (_axNo == 0)
            {
                _pSWLimit = 128410;
                _nSWLimit = -18530;
            }
            else if (_axNo == 1)
            {
                _pSWLimit = 79000;
                _nSWLimit = -25500;
            }
            else if (_axNo == 2)
            {
                _pSWLimit = 17670;
                _nSWLimit = -20448;
            }

            Set_PSW_Limit(_pSWLimit);
            Set_NSW_Limit(_nSWLimit);

        }


        /// <summary>
        /// Jog 운전 
        /// </summary>
        public void JogMove(Dir dir)
        {
            switch (dir)
            {
                case Dir.FWD:
                case Dir.DOWN:
                case Dir.RIGHT:
                    MMCLib.v_move(_axNo, _jogSpd, _jogAcc);
                    break;
                case Dir.BWD:
                case Dir.UP:
                case Dir.LEFT:
                    MMCLib.v_move(_axNo, -_jogSpd, _jogAcc);
                    break;                    
            }
        }
        /// <summary>
        ///  Jog 운전 정지
        /// </summary>
        public void JogStop()
        {
            MMCLib.v_move_stop(_axNo);
        }


        public void PosMove(double pnt)
        {
           // AxisDone = false;
            MMCLib.start_move(_axNo, pnt, _posSpd, _posAcc);

        }


        /// <summary>
        /// 인칭 운전
        /// </summary>
        public void InchMove(double dist)
        {
            double ratio = 1000;
            MMCLib.start_tr_move(_axNo, dist*ratio, _inchSpd, _inchAcc, _inchDec);

         }

        public void Set_PSW_Limit(double pos)
        {
            _pSWLimit = pos;
            int rslt= MMCLib.fset_positive_sw_limit(_axNo, pos,
                (short)MMCLib.MMCDef.STOP_EVENT);
        }
        /// <summary>
        /// Negative Software Limit설정
        /// </summary>
        /// <param name="pos"></param>
        public void Set_NSW_Limit(double pos)
        {
            _nSWLimit = pos;
            int rslt =MMCLib.fset_negative_sw_limit(_axNo, pos,
                (short)MMCLib.MMCDef.STOP_EVENT);
        }

        public void origin()
        {
            MMCLib.set_positive_sw_limit(_axNo, -2555550, (short)MMCLib.MMCDef.NO_EVENT);
            MMCLib.set_negative_sw_limit(_axNo, 2555550, (short)MMCLib.MMCDef.NO_EVENT);
            MMCLib.set_positive_limit(_axNo, (short)MMCLib.MMCDef.NO_EVENT);
            MMCLib.set_negative_limit(_axNo, (short)MMCLib.MMCDef.NO_EVENT);


         MMCLib.v_move(_axNo, -_jogSpd, _jogAcc);
         while (true)
         {
             Update_Sensor_sts();

             if (_rls || _home)
             {
                 JogStop();

                 if (_rls)
                 {
                     MMCLib.v_move(_axNo, _jogSpd, _jogAcc);
                     Thread.Sleep(1000);
                 }

                 else if (_home)
                 {
                     JogStop();
                     break;
                 }
             }
         }
          

            this.Init();
            Clear();
            while (true)
            {
                if (MMCLib.axis_done(_axNo) == 1)
                {
                    Limit_Set();
                    break;
                }

            }
            

        }


        #endregion
    }
    public enum Dir
    {
        FWD, BWD, UP, DOWN, LEFT, RIGHT
    }

   


}
