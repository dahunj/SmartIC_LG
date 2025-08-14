using System;
using System.Windows.Threading;

namespace SmartICAVI
{
    class DioService
    {
        #region Enums
        public enum Inports
        {
            ems = 0, mainAir, airSwitch, ionizerAlarm, supplyAirChuck, supplyClutch, collectAirChuck, collectClutch, startSensor, holeSensor1,
            holeSensor2, holeSensor3, holeSensor4, endSensor, in14, in15, supplyGuideUp, supplyGuideDown, collectGuideUp, collectGuideDown,
            in20, in21, in22, in23, leftRollerUp, leftRollerDown, rightRollerUp, rightRollerDown,
        };

        public enum Outports
        {
            twLampRed = 0, twLampYellow, twLampGreen, twBuzzer, ledSupplyAir, ledSupplyClutch, ledCollectAir, ledCollectClutch, supplyAirChuck, collectAirChuck,
            supplyGuide, collectGuide, ionizerAir, out13, out14, out15, torquePowerOn, supplyClutch, collectClutch, ionizerInterlock,
            out20, out21, out22, out23, ampReset1, ampReset2, ampReset3, ampReset4, axis4CW, axis4CCW, axis4EXT,
        };
        #endregion

        #region EventHandlers
        public delegate void EventHandlerDio(int port, int value);
        public event EventHandlerDio EventInport;
        public event EventHandlerDio EventOutport;

        public void FireEventInport(int port, int value)
        {
            if (null != EventInport)
                EventInport(port, value);
        }

        public void FireEventOutport(int port, int value)
        {
            if (null != EventOutport)
                EventOutport(port, value);
        }

        #endregion

        #region Variables
        private IDio iDio = null;

        private DispatcherTimer timer = null;

        private bool emo1 = false;
        private bool emo2 = false;

        //private SystemService sysService = null;
        //private MsgService msgService = null;
        #endregion

        #region Properties
        public UInt32 Inports0
        {
            get
            {
                if (null != iDio)
                    return iDio.Inports0;

                return 0;
            }
        }

        public UInt32 Inports1
        {
            get
            {
                if (null != iDio)
                    return iDio.Inports1;

                return 1;
            }
        }

        public UInt32 Outports0
        {
            get
            {
                if (null != iDio)
                    return iDio.Outports0;

                return 0;
            }
        }

        public UInt32 Outports1
        {
            get
            {
                if (null != iDio)
                    return iDio.Outports1;

                return 1;
            }
        }
        #endregion

        #region Singleton

        private static DioService singleton = null;

        public static DioService Singleton
        {
            get
            {
                if (null == singleton)
                {
                    singleton = new DioService();
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
            switch (DataService.Singleton.DataSystem.MachineName)
            {
                case "SmartICAVI":
                    iDio = new DioAXT();
                    break;
                case "Virtual":
                    iDio = new DioVirtual();
                    break;
                default:
                    iDio = new DioAXT();
                    break;
            }
            
            int ret = iDio.Initialize();

            // Set Timer
            timer = new DispatcherTimer();
            timer.Interval = new System.TimeSpan(0, 0, 0, 0, 50);
            timer.IsEnabled = true;
            timer.Tick += new EventHandler(timer_Tick);
                        
            timer_Tick(null, null);

            return ret;
        }

        public int UnInitialize()
        {
            timer.IsEnabled = false;
            timer.Tick -= timer_Tick;

            if (null != iDio)
                return iDio.UnInitialize();

            return -1;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (null == iDio)
                return;

            iDio.ReadInports();
            iDio.ReadOutports();

            CheckEMG();
        }

        public int SetOutport(int index)
        {
            if (null == iDio)
                return -1;

            return iDio.SetOutport(index);

            //if (index == (int)EnumSmartIC.Outports.buzzer1)
            //{
            //    SequenceService.Singleton.Stop();
            //    System.Windows.Forms.Application.DoEvents();
            //    int j = 0;
            //}
            //return 0;
            ////return 0;
        }

        public int ResetOutport(int index)
        {
            if (null == iDio)
                return -1;

            return iDio.ResetOutport(index);
            //return 0;
        }

        public int ToggleOutport(int index)
        {
            if (null == iDio)
                return -1;

            return iDio.ToggleOutport(index);
        }

        public bool IsInportOn(int index)
        {
            if (null == iDio)
                return false;

            return iDio.GetInportOn(index);

            //return false;
        }

        public bool IsOutportOn(int index)
        {
            if (null == iDio)
                return false;

            return iDio.GetOutportOn(index);
            //return false;
        }

        private void CheckEMG()
        {
            // EMO
            if (true == IsInportOn((int)EnumSmartIC.Inports.emo1))
            {
                if (false == emo1)
                {
                    emo1 = true;
                    SystemService.Singleton.State = SystemService.States.stop;
                    MsgService.Singleton.ShowAlarm((int)EnumSmartIC.HeavyAlarms.emo1);
                }
            }
            else
            {
                emo1 = false;
            }

            if (true == IsInportOn((int)EnumSmartIC.Inports.emo2))
            {
                if (false == emo2)
                {
                    emo2 = true;
                    SystemService.Singleton.State = SystemService.States.stop;
                    MsgService.Singleton.ShowAlarm((int)EnumSmartIC.HeavyAlarms.emo2);
                }
            }
            else
            {
                emo2 = false;
            }

            //// runCheck // 20170117 정상적인 상황에서 Reset 안되는 경우 발생하여 삭제함.
            //if (true == IsOutportOn((int)EnumSmartIC.Outports.runCheck))
            //{
            //    ResetOutport((int)EnumSmartIC.Outports.runCheck);
            //}
        }
    }
}
