using System;
using System.Windows;
using System.Windows.Threading;

namespace SmartICAVI
{
    class MotionService
    {
        #region EventHandlers

        public delegate void EventHandlerMotion(int axis, int value);
        public event EventHandlerMotion EventMotionDone;
        public void FireEventMotionDone(int axis, int value)
        {
            if (null != EventMotionDone)
                EventMotionDone(axis, value);
        }
         
        #endregion

        #region Variables
        IMotion iMotion = null;
        //private SystemService sysService = null;
        #endregion

        static bool InvokeRequired
        {
            get { return Dispatcher.CurrentDispatcher != Application.Current.Dispatcher; }
        }

        #region Singleton
        private static MotionService singleton = null;

        public static MotionService Singleton
        {
            get
            {
                if (null == singleton)
                {
                    singleton = new MotionService();
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

        public int InitOnly()
        {
            switch (DataService.Singleton.DataSystem.MachineName)
            {
                case "SmartICAVI":
                    iMotion = new MotionAXT();
                    break;
                case "Virtual":
                    iMotion = new MotionVirtual();
                    break;
                default:
                    iMotion = new MotionAXT();
                    break;
            }

            return iMotion.InitOnly();
        }

        public int Initialize()
        {
            //sysService = SystemService.Singleton;
            //sysService.EventState += OnEventState;

            switch (DataService.Singleton.DataSystem.MachineName)
            {
                case "SmartICAVI":
                    iMotion = new MotionAXT();
                    break;
                case "Virtual":
                    iMotion = new MotionVirtual();
                    break;
                default:
                    iMotion = new MotionAXT();
                    break;
            }

            return iMotion.Initialize();
        }

        public int UnInitialize()
        {
            //if (null != sysService)
            //    sysService.EventState -= OnEventState;

            if (null != iMotion)
                return iMotion.UnInitialize();

            return 0;
        }
                
        public void Stop(int axis)
        {
            if (null == iMotion)
                return;

            iMotion.Stop(axis);
        }

        public void Reset()
        {
            if (null == iMotion)
                return;

            int axis = (int)EnumSmartIC.Axis.buffer;

            bool isAmpFault = false;
            bool isServoOn = true;

            TimeSpan timeoutSpan = new TimeSpan(0, 0, 0, 0, 30);
            DateTime timeout = DateTime.Now;

            for (int i = 0; i < axis; ++i)
            {
                if (1 == iMotion.GetStateAmpAlarm(i))
                {
                    iMotion.ServoOn(i, false);

                    isAmpFault = true;
                    iMotion.AmpReset(i, true);
                }

                if (0 == iMotion.GetStateServoOn(i))
                {
                    isServoOn = false;
                }
            }

            if (true == isAmpFault)
            {
                timeoutSpan = new TimeSpan(0, 0, 0, 0, 500);
                timeout = DateTime.Now;
                timeout = timeout.Add(timeoutSpan);

                while (timeout > DateTime.Now)
                {
                    ;
                }

                for (int i = 0; i < axis; ++i)
                {
                    iMotion.AmpReset(i, false);

                    iMotion.ServoOn(i, true);
                }
            }

            if (false == isServoOn)
            {
                for (int i = 0; i < axis; ++i)
                {
                    iMotion.ServoOn(i, true);
                }
            }

            // Retry
            if (true == isAmpFault || false == isServoOn)
            {

                timeoutSpan = new TimeSpan(0, 0, 0, 0, 50);
                timeout = DateTime.Now;
                timeout = timeout.Add(timeoutSpan);

                while (timeout > DateTime.Now)
                {
                    ;
                }

                isAmpFault = false;
                isServoOn = true;

                for (int i = 0; i < axis; ++i)
                {
                    if (1 == iMotion.GetStateAmpAlarm(i))
                    {
                        iMotion.ServoOn(i, false);

                        isAmpFault = true;
                        iMotion.AmpReset(i, true);
                    }

                    if (0 == iMotion.GetStateServoOn(i))
                    {
                        isServoOn = false;
                    }
                }

                if (true == isAmpFault)
                {
                    timeoutSpan = new TimeSpan(0, 0, 0, 0, 500);
                    timeout = DateTime.Now;
                    timeout = timeout.Add(timeoutSpan);

                    while (timeout > DateTime.Now)
                    {
                        ;
                    }

                    for (int i = 0; i < axis; ++i)
                    {
                        iMotion.AmpReset(i, false);

                        iMotion.ServoOn(i, true);
                    }
                }

                if (false == isServoOn)
                {
                    for (int i = 0; i < axis; ++i)
                    {
                        iMotion.ServoOn(i, true);
                    }
                }
            }
        }

        public void JogN(int axis, double vel, double accel)
        {
            if (null == iMotion)
                return;

            iMotion.JogN(axis, vel, accel);
        }

        public void JogP(int axis, double vel, double accel)
        {
            if (null == iMotion)
                return;

            iMotion.JogP(axis, vel, accel);
        }

        public void AMove(int axis, double pos, double vel, double accel)
        {
            if (null == iMotion)
                return;

            iMotion.AMove(axis, pos, vel, accel);
        }

        public void RMove(int axis, double pos, double vel, double accel)
        {
            if (null == iMotion)
                return;

            iMotion.RMove(axis, pos, vel, accel);
        }

        public void HomeSearch(int axis)
        {
            if (null == iMotion)
                return;

            iMotion.HomeSearch(axis);
        }

        public void SeqMove(int axis, int seq)
        {
            if (null == iMotion)
                return;

            iMotion.SeqMove(axis, seq);
        }

        public bool IsMotionDone(int axis)
        {
            if (null == iMotion)
                return false;

            return iMotion.IsMotionDone(axis);
        }

        public int SetPosition(int axis, double pos)
        {
            if (null == iMotion)
                return -1;

            return iMotion.SetPosition(axis, pos);
        }

        public double GetCurrentPosition(int axis)
        {
            if (null == iMotion)
                return 0.0;

            return iMotion.GetCurrentPosition(axis);
        }

        public int GetStateLimitP(int axis)
        {
            if (null == iMotion)
                return -1;

            return iMotion.GetStateLimitP(axis);
        }

        public int GetStateLimitN(int axis)
        {
            if (null == iMotion)
                return -1;

            return iMotion.GetStateLimitN(axis);
        }

        public int GetStateInposition(int axis)
        {
            if (null == iMotion)
                return -1;

            return iMotion.GetStateInposition(axis);
        }

        public int GetStateAmpAlarm(int axis)
        {
            if (null == iMotion)
                return -1;

            return iMotion.GetStateAmpAlarm(axis);
        }

        public int GetStateServoOn(int axis)
        {
            if (null == iMotion)
                return -1;

            return iMotion.GetStateServoOn(axis);
        }

        public int GetStateHome(int axis)
        {
            if (null == iMotion)
                return -1;

            return iMotion.GetStateHome(axis);
        }

        public int GetStateHomeDone(int axis)
        {
            if (null == iMotion)
                return -1;

            return iMotion.GetStateHomeDone(axis);
        }

        public int ApplyParams()
        {
            if (null == iMotion)
                return -1;

            return iMotion.ApplyParams();
        }

        public int SetServoOn(int axis, bool on)
        {
            if (null == iMotion)
                return -1;

            return iMotion.ServoOn(axis, on);
        }


        public int SetOverridePos(int axis, double pos)
        {
            if (null == iMotion)
                return -1;

            return iMotion.SetOverridePos(axis, pos);
        }

        public int SetOverrideVel(int axis, double vel)
        {
            if (null == iMotion)
                return -1;

            return iMotion.SetOverrideVel(axis, vel);
        }

        #region Events

        //private void OnEventState(object send, EventArgs e)
        //{
        //    switch (sysService.State)
        //    {
        //        case SystemService.States.none:// = 0, 
        //            break;
        //        case SystemService.States.ready:
        //            break;
        //        case SystemService.States.idle:
        //            break;
        //        case SystemService.States.run:
        //            break;
        //        case SystemService.States.pause:
        //            break;
        //        case SystemService.States.resume:
        //            break;
        //        case SystemService.States.homing:
        //            break;
        //        case SystemService.States.homeDone:
        //            break;
        //        case SystemService.States.jobDone:
        //            break;
        //        case SystemService.States.systemLock:
        //            break;
        //        case SystemService.States.systemRelease:
        //            break;
        //        case SystemService.States.reset:
        //            break;
        //        case SystemService.States.lightAlarm:
        //            break;
        //        case SystemService.States.stop:
        //        case SystemService.States.heavyAlarm:
        //        case SystemService.States.emg:
        //            Stop(-1);
        //            break;
        //    }
        //}
        #endregion
    }
}
