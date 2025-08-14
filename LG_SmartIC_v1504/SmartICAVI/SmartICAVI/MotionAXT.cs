using System;
using System.Threading;

namespace SmartICAVI
{
    class MotionAXT : IMotion
    {
        #region Variables
        protected int[] homeDones = new int[32];

        
        #endregion

        #region Interfaces
        private bool isInitialized = false;

        public bool IsInitialized
        {
            get { return isInitialized; }
        }

        public int InitOnly()
        {
            if (CAxtCAMCFS20.CFS20IsInitialized() == 0)				// 모듈을 사용할 수 있도록 라이브러리가 초기화되었는지 확인한다
            {
                if (CAxtCAMCFS20.InitializeCAMCFS20(1) == 0)		// 모듈을 초기화한다. 열려있는 모든베이스보드에서 모듈을 검색하여 초기화한다
                {
                    return -1;
                }
            }

            return 0;
        }

        public int Initialize()
        {
            isInitialized = false;

            if (CAxtCAMCFS20.CFS20IsInitialized() == 0)				// 모듈을 사용할 수 있도록 라이브러리가 초기화되었는지 확인한다
            {
                if (CAxtCAMCFS20.InitializeCAMCFS20(1) == 0)		// 모듈을 초기화한다. 열려있는 모든베이스보드에서 모듈을 검색하여 초기화한다
                {
                    Log_Trace.WriteLine("Error MotionAXT.Initialize().InitializeCAMCFS20()");

                    return -1;
                }
            }

            //byte[] nFilename = new byte[255];
            //string temp = null;

            //nFilename = StringToByte(temp);

            //Log_Trace.WriteLine("Load Motion Params " + temp);

            //if (CAxtCAMCFS20.CFS20load_parameter_all(ref nFilename[0]) != 1)
            //{
            //    Log_Trace.WriteLine("Error MotionAXT.Initialize().CFS20load_parameter_all()");

            //    return -1;
            //}

            ApplyParams();

            isInitialized = true;


            for (int i = 0; i < 32; ++i)
                homeDones[i] = 0;


            for (int i = 0; i < 8; ++i)
                ServoOn(i, true);
                        
            return 0;
        }

        public int UnInitialize()
        {
            for (int i = 0; i < 8; ++i)
                Stop(i);

            for (int i = 0; i < 8; ++i)
                ServoOn(i, false);

            //if (1 == CAxtCAMCFS20.CFS20IsInitialized() )
            //{
                CAxtCAMCFS20.CFS20StopService();
            //}

            isInitialized = false;
            return 0;
        }

        public int ServoOn(int axis, bool servoOn)
        {
            if (false == IsInitialized)
                return -1;

            if( true == servoOn )
                CAxtCAMCFS20.CFS20set_output_bit((short)axis, 0);
            else
                CAxtCAMCFS20.CFS20reset_output_bit((short)axis, 0);

            return 0;
        }

        public int GetStateServoOn(int axis)
        {
            if (false == IsInitialized)
                return -1;

            return CAxtCAMCFS20.CFS20output_bit_on((short)axis, 0);
        }

        public int AmpReset(int axis, bool on)
        {
            if (false == IsInitialized)
                return -1;

            if (true == on)
                CAxtCAMCFS20.CFS20set_output_bit((short)axis, 1);
            else
                CAxtCAMCFS20.CFS20reset_output_bit((short)axis, 1);

            return 0;
        }

        public int HomeSearch(int axis)
        {
            homeDones[axis] = 0;

            if (false == IsInitialized)
                return -1;

            // 신호 검출 구동-====================================================================================================
            // 지정 신호의 상향/하향 에지를 검색하여 급정지 또는 감속정지를 할 수 있다.
            // detect_signal : 검색 신호 설정(typedef : DETECT_DESTINATION_SIGNAL)
            // PElmNegativeEdge    = 0x0,        // +Elm(End limit) 하강 edge
            // NElmNegativeEdge    = 0x1,        // -Elm(End limit) 하강 edge
            // PSlmNegativeEdge    = 0x2,        // +Slm(Slowdown limit) 하강 edge
            // NSlmNegativeEdge    = 0x3,        // -Slm(Slowdown limit) 하강 edge
            // In0DownEdge         = 0x4,        // IN0(ORG) 하강 edge
            // In1DownEdge         = 0x5,        // IN1(Z상) 하강 edge
            // In2DownEdge         = 0x6,        // IN2(범용) 하강 edge
            // In3DownEdge         = 0x7,        // IN3(범용) 하강 edge
            // PElmPositiveEdge    = 0x8,        // +Elm(End limit) 상승 edge
            // NElmPositiveEdge    = 0x9,        // -Elm(End limit) 상승 edge
            // PSlmPositiveEdge    = 0xa,        // +Slm(Slowdown limit) 상승 edge
            // NSlmPositiveEdge    = 0xb,        // -Slm(Slowdown limit) 상승 edge
            // In0UpEdge           = 0xc,        // IN0(ORG) 상승 edge
            // In1UpEdge           = 0xd,        // IN1(Z상) 상승 edge
            // In2UpEdge           = 0xe,        // IN2(범용) 상승 edge
            // In3UpEdge           = 0xf         // IN3(범용) 상승 edge
            // Signal Search1 : 구동 시작후 입력 속도까지 가속하여, 신호 검출후 감속 정지.

            int step = 0;

            SystemService sysService = SystemService.Singleton;
            DataService dataService = DataService.Singleton;

            DateTime start = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 2000);
            DateTime timeout = DateTime.Now;// start.Add(duration);

            

            while (sysService.State < SystemService.States.systemLock)
            {
                switch (step)
                {
                    case 0:     // Check Sensor
                        if (1 == GetStateLimitN(axis))
                        {
                            // Move +
                            step = 30;
                        }
                        else
                        {
                            // Move -
                            step += 10;
                        }
                        break;
                    case 10:        // - Move
                        // NElmPositiveEdge    = 0x9,        // -Elm(End limit) 상승 edge
                        CAxtCAMCFS20.CFS20start_s_signal_search1((short)axis, -dataService.DataMotion[axis].VelSlow, dataService.DataMotion[axis].AccelSlow, 0x9);
                        step += 10;
                        break;
                    case 20:
                        if (true == IsMotionDone(axis))
                            step += 10;
                        break;
                    case 30:        // + 방향으로 // NElmNegativeEdge    = 0x1,        // -Elm(End limit) 하강 edge
                        CAxtCAMCFS20.CFS20start_s_signal_search1((short)axis, dataService.DataMotion[axis].VelSlow, dataService.DataMotion[axis].AccelSlow, 0x1);
                        step += 10;
                        break;
                    case 40:
                        if (true == IsMotionDone(axis))
                            step += 10;
                        break;
                    case 50:
                        start = DateTime.Now;
                        timeout = start.Add(duration);
                        step += 10;
                        break;
                    case 60:
                        if (timeout < DateTime.Now)
                        {
                            SetPosition(axis, 0.0);
                            step = 20000;
                        }
                        break;
                    case 20000:
                        homeDones[axis] = 1;
                        return 0;
                }
                System.Windows.Forms.Application.DoEvents();
            }
            
            return 0;
        }

        public int AMove(int axis, double pos, double vel, double accel)
        {
            if (false == IsInitialized)
                return -1;

            CAxtCAMCFS20.CFS20start_as_move((short)axis, pos, vel, accel, accel);
            
            return 0;
        }

        public int RMove(int axis, double pos, double vel, double accel)
        {
            if (false == IsInitialized)
                return -1;

            CAxtCAMCFS20.CFS20start_ras_move((short)axis, pos, vel, accel, accel);
            
            return 0;
        }

        public int JogP(int axis, double vel, double accel)
        {
            if (false == IsInitialized)
                return -1;

            CAxtCAMCFS20.CFS20v_s_move((short)axis, vel, accel);
            
            return 0;
        }

        public int JogN(int axis, double vel, double accel)
        {
            if (false == IsInitialized)
                return -1;

            CAxtCAMCFS20.CFS20v_s_move((short)axis, -vel, accel);

            return 0;
        }

        public int Stop(int axis)
        {
            if (false == IsInitialized)
                return -1;

            if (0 > axis)
            {
            }
            else
            {
                CAxtCAMCFS20.CFS20set_stop((short)axis);
            }
            
            
            return 0;
        }

        public int EStop(int axis)
        {
            if (false == IsInitialized)
                return -1;

            CAxtCAMCFS20.CFS20set_e_stop((short)axis);
            
            return 0;
        }

        public int Reset()
        {
            if (false == IsInitialized)
                return -1;
                       
            return 0;
        }
        
        public int SeqMove(int axis, int seq)
        {
            if (false == IsInitialized)
                return -1;
            
            return 0;
        }

        public bool IsMotionDone(int axis)
        {
            if (false == IsInitialized)
                return false;

            return (1 == CAxtCAMCFS20.CFS20motion_done((short)axis)) ? true : false;
        }

        public int SetPosition(int axis, double pos)
        {
            if (false == IsInitialized)
                return -1;

            CAxtCAMCFS20.CFS20set_command_position((short)axis, pos);
            CAxtCAMCFS20.CFS20set_actual_position((short)axis, pos);

            return 0;
        }

        public int SetLimitEnable(int axis, bool enable)
        {
            if (false == IsInitialized)
                return -1;

            byte on = (true == enable) ? (byte)1 : (byte)0;

            CAxtCAMCFS20.CFS20set_end_limit_enable((short)axis, on);
           
            return 0;
        }

        public double GetCurrentPosition(int axis)
        {
            if (false == IsInitialized)
                return -999.999;

            double pos = CAxtCAMCFS20.CFS20get_actual_position((short)axis);

            return pos;
        }

        public int GetStateLimitP(int axis)
        {
            if (false == IsInitialized)
                return -1;

            return (int)CAxtCAMCFS20.CFS20get_pend_limit_switch((short)axis);
        }

        public int GetStateLimitN(int axis)
        {
            if (false == IsInitialized)
                return -1;

            return (int)CAxtCAMCFS20.CFS20get_nend_limit_switch((short)axis);
        }

        public int GetStateInposition(int axis)
        {
            if (false == IsInitialized)
                return -1;

            return (int)CAxtCAMCFS20.CFS20get_inposition_switch((short)axis);
        }

        public int GetStateAmpAlarm(int axis)
        {
            if (false == IsInitialized)
                return -1;

            return (int)CAxtCAMCFS20.CFS20get_alarm_switch((short)axis);
        }

        public int GetStateHome(int axis)
        {
            if (false == IsInitialized)
                return -1;
            
            return (int)(CAxtCAMCFS20.CFS20get_input((short)axis) & 0x01);
        }

        public int GetStateHomeDone(int axis)
        {
            if (0 > axis)
                return -1;
            if (32 <= axis)
                return -1;

            return homeDones[axis];
        }

        public int SetVirtual(int command, int value, double value2 = 0.0)
        {
            return 0;
        }

        public int ApplyParams()
        {
            // SetParams
            DataService dataService = DataService.Singleton;

            for (short i = 0; i < 10; ++i)
            {
                // Level Limit P
                CAxtCAMCFS20.CFS20set_pend_limit_level(i, (byte)dataService.DataMotion[i].LevelLimitP);
                // Level Limit N
                CAxtCAMCFS20.CFS20set_nend_limit_level(i, (byte)dataService.DataMotion[i].LevelLimitN);
                // Level Inposition
                CAxtCAMCFS20.CFS20set_inposition_level(i, (byte)dataService.DataMotion[i].LevelInpos);
                // Level Alarm
                CAxtCAMCFS20.CFS20set_alarm_level(i, (byte)dataService.DataMotion[i].LevelAlarm);

                // Use Inposition
                CAxtCAMCFS20.CFS20set_inposition_enable(i, (byte)dataService.DataMotion[i].UseInpos);
                // Use Alarm
                CAxtCAMCFS20.CFS20set_alarm_enable(i, (byte)dataService.DataMotion[i].UseAlarm);


                // Pulse per Unit
                //CAxtCAMCFS20.CFS20set_movepulse_perunit(i, dataService.DataMotion[i].UnitPulse);
                CAxtCAMCFS20.CFS20set_moveunit_perpulse(i, dataService.DataMotion[i].UnitPulse);

                // Start Speed
                CAxtCAMCFS20.CFS20set_startstop_speed(i, dataService.DataMotion[i].StartSpeed);

                // Pulse Out Method
                CAxtCAMCFS20.CFS20set_pulse_out_method(i, (byte)dataService.DataMotion[i].PulseOut);

                // Encoder Type
                CAxtCAMCFS20.CFS20set_enc_input_method(i, (byte)dataService.DataMotion[i].EncoderType);

                // Max Speed
                CAxtCAMCFS20.CFS20set_max_speed(i, dataService.DataMotion[i].VelRapid + 10.0);


                //CAxtCAMCFS20.CFS20set_moveunit_perpulse(m_nAxis, dMoveUnit);
                //CAxtCAMCFS20.CFS20set_startstop_speed(m_nAxis, dStartStop);
                //CAxtCAMCFS20.CFS20set_max_speed(m_nAxis, CAxtCAMCFS20.CFS20get_max_speed(m_nAxis));

            }
            int axisVision = (int)EnumSmartIC.Axis.visionFeed;
            short axisPunch = (short)EnumSmartIC.Axis.punchFeed;

            CAxtCAMCFS20.CFS20set_max_speed(axisPunch, dataService.DataMotion[axisVision].VelRapid + 10.0);
            return 0;
        }

        public int SetOverridePos(int axis, double pos)
        {
            if (false == IsInitialized)
                return -1;

            return (int)CAxtCAMCFS20.CFS20position_override((short)axis, pos);
        }

        public int SetOverrideVel(int axis, double vel)
        {
            if (false == IsInitialized)
                return -1;

            return (int)CAxtCAMCFS20.CFS20velocity_override((short)axis, vel);
        }
        #endregion

        protected void DoEvents(int mSec)
        {
            DateTime start = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, mSec);
            DateTime timeout = start.Add(duration);

            do
            {
                System.Windows.Forms.Application.DoEvents();
                Thread.Sleep(1);
            } while (timeout > DateTime.Now);
        }


        // Trance string to byte
        private byte[] StringToByte(string str)
        {
            byte[] nFilename = new byte[255];
            short i = 0;

            // string >> Byte
            while (i < str.Length)
            {
                nFilename[i] = (byte)str[i++];
            }

            return nFilename;
            /*			i=0;
                        temp = null;
			
                        // Byte >> string
                        while (nFilename[i] != '\0')
                        {
                            temp += (char)nFilename[i++];
                        }
                        MessageBox.Show(temp);
            */
        }

        // Trance string to char
        private char[] StringToChar(string str)
        {
            char[] nFilename = new char[255];
            //			short  i   = 0;

            nFilename = str.ToCharArray(0, str.Length);
            // string >> Byte
            //			while (i < str.Length)
            //			{
            //				nFilename[i] = str[i++];
            //			}

            return nFilename;
        }

    }
}
