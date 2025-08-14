using System;
using System.Windows.Threading;

namespace SmartICAVI
{
    class SystemService
    {
        #region Enums
       
        public enum States
        {
            none = 0, ready, idle, run, pause, resume, homing, homeDone, jobDone, systemLock, systemRelease, reset, lightAlarm, stop, heavyAlarm, emg
        }

        public enum Modes
        {
            none = 0, home, auto, recipe, setup, manual, pm, syslock,
        }

        public enum Users
        {
            none = 0, user, admin, maint
        }

        #endregion

        #region Variables
        private VisionTopService topVision = null;
        private VisionTop2Service top2Vision = null;
        private VisionBottomService bottomVision = null;
        private VisionBottom2Service bottom2Vision = null;
        private VisionMonoService monoVision = null;
        private VisionMono2Service mono2Vision = null;

        private DioService dioService = null;
        private SequenceService seqService = null;
        private DataService dataService = null;

        private DispatcherTimer timerReset = null;
        #endregion

        #region EventHandlers

        public event EventHandler EventState;
        public void FireEventState()
        {
            if (null != EventState)
                EventState(this, null);
        }

        public event EventHandler EventMode;
        public void FireEventMode()
        {
            if (null != EventMode)
                EventMode((object)this, null);
        }

        #endregion

        #region Properties
        private States oldState = States.none;
        private States state;
        public States State
        {
            get
            {
                return state;
            }
            set
            {
                if (state == value)
                    return;

                oldState = state;
                state = value;

                if ((value == States.reset) || (States.systemRelease == value) )
                {
                    if (value == States.reset)
                    {
                        MotionService.Singleton.Reset();
                    }
                    SetTimerReset();
                }
                else if (States.heavyAlarm == value)
                {
                    // Heavy Alaram 일 경우 무조건 부저음.
                    if (null != dioService)
                    {
                        dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);

                        //dioService.ResetOutport(30);
                        dioService.ResetOutport(31);
                    }
                }

                FireEventState();
                SendState();

                SetTwLamp();
            }
        }

        private Modes mode;
        public Modes Mode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;

                FireEventMode();

                SendMode();
            }
        }
        #endregion

        #region Singleton

        private static SystemService singleton = null;

        public static SystemService Singleton
        {
            get
            {
                if (null == singleton)
                {
                    singleton = new SystemService();
                }

                return singleton;
            }
        }

        public void ReleaseSingleton()
        {
            if (null != singleton)
            {
                singleton.UnInitialize();
                singleton = null;
            }
        }
        #endregion

        public int Initialize()
        {
            topVision = VisionTopService.Singleton;
            top2Vision = VisionTop2Service.Singleton;
            bottomVision = VisionBottomService.Singleton;
            bottom2Vision = VisionBottom2Service.Singleton;
            monoVision = VisionMonoService.Singleton;
            mono2Vision = VisionMono2Service.Singleton;

            dioService = DioService.Singleton;
            seqService = SequenceService.Singleton;

            dataService = DataService.Singleton;

            return 0;
        }

        public int UnInitialize()
        {
            ResetTimerReset();

            return 0;
        }

        private int SendMode()
        {
            if (null == topVision)
                return -1;

            
            topVision.SetMode();
            top2Vision.SetMode();
            bottomVision.SetMode();
            bottom2Vision.SetMode();
            monoVision.SetMode();
            mono2Vision.SetMode();
            

            return 0;
        }

        private int SendState()
        {
            if (null == topVision)
                return -1;


            topVision.SetStatusUpdate();
            top2Vision.SetStatusUpdate();
            bottomVision.SetStatusUpdate();
            bottom2Vision.SetStatusUpdate();
            monoVision.SetStatusUpdate();
            mono2Vision.SetStatusUpdate();


            return 0;
        }

        private int SetTwLamp()
        {
            if (null == dioService)
                return -1;
            if (null == seqService)
                return -1;

            switch (State)
            {
                case SystemService.States.none:// = 0, 
                    dioService.ResetOutport((int)EnumSmartIC.Outports.twlampRed);
                    dioService.ResetOutport((int)EnumSmartIC.Outports.twlampYellow);
                    dioService.ResetOutport((int)EnumSmartIC.Outports.twlampGreen);
                    break;
                case SystemService.States.ready:
                case SystemService.States.idle:
                    dioService.ResetOutport((int)EnumSmartIC.Outports.twlampRed);
                    dioService.SetOutport((int)EnumSmartIC.Outports.twlampYellow);
                    dioService.ResetOutport((int)EnumSmartIC.Outports.twlampGreen);

                    dataService.CurrentStatus = "대기 상태";
                    break;
                case SystemService.States.run:
                    if (Mode == Modes.auto)
                    {
                        dioService.ResetOutport((int)EnumSmartIC.Outports.twlampRed);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.twlampYellow);
                        dioService.SetOutport((int)EnumSmartIC.Outports.twlampGreen);
                    }
                    else
                    {
                        dioService.ResetOutport((int)EnumSmartIC.Outports.twlampRed);
                        dioService.SetOutport((int)EnumSmartIC.Outports.twlampYellow);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.twlampGreen);
                    }
                    break;
                case SystemService.States.pause:
                    dioService.ResetOutport((int)EnumSmartIC.Outports.twlampRed);
                    dioService.SetOutport((int)EnumSmartIC.Outports.twlampYellow);
                    dioService.SetOutport((int)EnumSmartIC.Outports.twlampGreen);
                    break;
                case SystemService.States.resume:
                    dioService.ResetOutport((int)EnumSmartIC.Outports.twlampRed);
                    dioService.ResetOutport((int)EnumSmartIC.Outports.twlampYellow);
                    dioService.SetOutport((int)EnumSmartIC.Outports.twlampGreen);
                    break;
                case SystemService.States.homing:
                    dioService.ResetOutport((int)EnumSmartIC.Outports.twlampRed);
                    dioService.SetOutport((int)EnumSmartIC.Outports.twlampYellow);
                    dioService.ResetOutport((int)EnumSmartIC.Outports.twlampGreen);
                    break;
                case SystemService.States.homeDone:
                    dioService.ResetOutport((int)EnumSmartIC.Outports.twlampRed);
                    dioService.SetOutport((int)EnumSmartIC.Outports.twlampYellow);
                    dioService.ResetOutport((int)EnumSmartIC.Outports.twlampGreen);
                    break;
                case SystemService.States.jobDone:
                    dioService.ResetOutport((int)EnumSmartIC.Outports.twlampRed);
                    dioService.SetOutport((int)EnumSmartIC.Outports.twlampYellow);
                    dioService.ResetOutport((int)EnumSmartIC.Outports.twlampGreen);
                    break;
                case SystemService.States.systemLock:
                    dioService.SetOutport((int)EnumSmartIC.Outports.twlampRed);
                    dioService.SetOutport((int)EnumSmartIC.Outports.twlampYellow);
                    dioService.SetOutport((int)EnumSmartIC.Outports.twlampGreen);
                    break;
                case SystemService.States.systemRelease:
                    dioService.ResetOutport((int)EnumSmartIC.Outports.twlampRed);
                    dioService.ResetOutport((int)EnumSmartIC.Outports.twlampYellow);
                    dioService.ResetOutport((int)EnumSmartIC.Outports.twlampGreen);
                    break;
                case SystemService.States.reset:
                    if (true == seqService.IsHomeDone)
                    {
                        dioService.ResetOutport((int)EnumSmartIC.Outports.twlampRed);
                        dioService.SetOutport((int)EnumSmartIC.Outports.twlampYellow);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.twlampGreen);
                    }
                    else
                    {
                        dioService.ResetOutport((int)EnumSmartIC.Outports.twlampRed);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.twlampYellow);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.twlampGreen);
                    }
                    dataService.CurrentStatus = "리셋 상태";
                    break;
                case SystemService.States.lightAlarm:
                    break;
                case SystemService.States.stop:
                case SystemService.States.heavyAlarm:
                case SystemService.States.emg:
                    dioService.SetOutport((int)EnumSmartIC.Outports.twlampRed);
                    dioService.ResetOutport((int)EnumSmartIC.Outports.twlampYellow);
                    dioService.ResetOutport((int)EnumSmartIC.Outports.twlampGreen);

                    dataService.CurrentStatus = "알람 상태";
                    break;
            }

            return 0;
        }

        private void SetTimerReset()
        {
            if (null != timerReset)
            {
                if (true == timerReset.IsEnabled)
                    return;
            }

            timerReset = new DispatcherTimer();
            timerReset.Interval = new System.TimeSpan(0, 0, 0, 0, 100);
            timerReset.IsEnabled = true;
            timerReset.Tick += timer_Reset;

        }

        private void ResetTimerReset()
        {
            if (null != timerReset)
            {
                timerReset.IsEnabled = false;
                timerReset.Tick -= timer_Reset;
                timerReset = null;
            }
        }

        private void timer_Reset(object sender, EventArgs e)
        {
            ResetTimerReset();
            if ((SystemService.States.reset == State) || (SystemService.States.systemRelease == State))
            {
                if (true == seqService.IsHomeDone)
                    State = States.ready;
                else
                    State = States.none;
            }
        }
    }
}
