using System;
using HalconDotNet;

namespace SmartICAVI
{
    class SequenceService
    {
        #region Variables
        private ISequence iSeq = null;

        private SystemService sysService = null;
        private MesService mesService = null;
        private DataService dataService = null;

        
        #endregion

        #region Properties
        public bool IsBufferInitDone { get; set; }

        public bool IsHomeDone { get; set; }

        public bool IsOnlineMode
        {
            get
            {
                if ((null != dataService) && (null != mesService))
                {
                    if ((true == dataService.IsOnlineMode) && (true == mesService.IsConnected))
                        return true;
                }

                return false;
            }
        }
        #endregion

        #region Events
        public delegate void EventHandlerInitStep(int step);
        public event EventHandlerInitStep EventInitStep;
        public void FireEventInitStep(int step)
        {
            if (null != EventInitStep)
                EventInitStep(step);
        }

        public delegate void EventHandlerInitString(string text);
        public event EventHandlerInitString EventInitString;
        public void FireEventInitString(string text)
        {
            if (null != EventInitString)
                EventInitString(text);
        }

        public delegate void EventHandlerPunchImage(string path, int index, string line, double offsetX, double offsetY);
        public event EventHandlerPunchImage EventPunchImage;
        public void FireEventPunchImage(string path, int index, string line, double offsetX, double offsetY)
        {
            if (null != EventPunchImage)
                EventPunchImage(path, index, line, offsetX, offsetY);
        }

        // type:0(Hole), 1(CNG),  unit:unit number, state:(1:start), (0:end)

        // state -1:THole, 0:Punch, 1:PunchStartUnit, 2:PunchEndUnit, 3
        public delegate void EventHandlerPunch(int unit, int state, string line);
        public event EventHandlerPunch EventPunch;
        public void FireEventPunch(int unit, int state, string line = "")
        {
            if (null != EventPunch)
                EventPunch(unit, state, line);
        }

        public delegate void EnvetHanderAutoJog(int run);
        public event EnvetHanderAutoJog EventAutoJog;
        public void FireEventAutoJog(int run)
        {
            if (null != EventAutoJog)
                EventAutoJog(run);
        }


        public event EnvetHanderAutoJog EventManualJog;
        public void FireEventManualJog(int run)
        {
            if (null != EventManualJog)
                EventManualJog(run);
        }

        #endregion

        #region Singleton
        private static SequenceService singleton = null;

        public static SequenceService Singleton
        {
            get
            {
                if (null == singleton)
                {
                    singleton = new SequenceService();
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
            IsBufferInitDone = false;
            IsHomeDone = false;

            

            mesService = MesService.Singleton;
            dataService = DataService.Singleton;
            //switch (DataService.Singleton.DataSystem.MachineName)
            //{
            //    case "SmartICAVI":
            //        iSeq = new SeqManual();
            //        break;
            //    case "Virtual":
            //        iSeq = new SeqVirtual();
            //        break;
            //    default:
            //        iSeq = new SeqManual();
            //        break;
            //}

            iSeq = new SeqManual();
            iSeq.Initialize();

            sysService = SystemService.Singleton;
            sysService.EventMode += OnEventMode;
            sysService.EventState += OnEventState;

            if ("Virtual" == DataService.Singleton.DataSystem.MachineName)
            {
                IsHomeDone = true;
                sysService.State = SystemService.States.ready;
            }

            return 0;
        }

        public int UnInitialize()
        {
            if (null != sysService)
            {
                sysService.EventMode -= OnEventMode;
                sysService.EventState -= OnEventState;
            }

            if (null != iSeq)
            {
                iSeq.UnInitialize();
            }
            return 0;
        }

        private void OnEventMode(object send, EventArgs e)
        {
            if ("Virtual" == DataService.Singleton.DataSystem.MachineName)
            {
                iSeq = new SeqVirtual();
                iSeq.Initialize();

                return;
            }

            switch (sysService.Mode)
            {
                case SystemService.Modes.none:// = 0, 
                    if ((int)SystemService.Modes.manual != iSeq.Mode)
                    {
                        iSeq.UnInitialize();

                        iSeq = new SeqManual();
                        iSeq.Initialize();
                    }
                    break;
                case SystemService.Modes.home:
                    if ((int)sysService.Mode != iSeq.Mode)
                    {
                        iSeq.UnInitialize();

                        iSeq = new SeqInit();
                        iSeq.Initialize();
                    }
                    break;
                case SystemService.Modes.auto:
                    if ((int)SystemService.Modes.auto != iSeq.Mode)
                    {
                        iSeq.UnInitialize();

                        iSeq = new SeqAuto();
                        iSeq.Initialize();
                    }
                    break;
                case SystemService.Modes.recipe:
                case SystemService.Modes.setup:
                case SystemService.Modes.manual:
                case SystemService.Modes.pm:
                case SystemService.Modes.syslock:
                    if ((int)SystemService.Modes.manual != iSeq.Mode)
                    {
                        iSeq.UnInitialize();

                        iSeq = new SeqManual();
                        iSeq.Initialize();
                    }
                    break;
                default:
                    if ((int)SystemService.Modes.manual != iSeq.Mode)
                    {
                        iSeq.UnInitialize();

                        iSeq = new SeqManual();
                        iSeq.Initialize();
                    }
                    break;
            }

            GC.Collect();
        }

        private void OnEventState(object send, EventArgs e)
        {
            switch (sysService.State)
            {
                case SystemService.States.none:// = 0, 
                    break;
                case SystemService.States.ready:
                    break;
                case SystemService.States.idle:
                    break;
                case SystemService.States.run:
                    Start();
                    break;
                case SystemService.States.pause:
                    Pause();
                    break;
                case SystemService.States.resume:
                    Resume();
                    break;
                case SystemService.States.homing:
                    break;
                case SystemService.States.homeDone:
                    break;
                case SystemService.States.jobDone:
                    break;
                case SystemService.States.systemLock:
                    break;
                case SystemService.States.systemRelease:
                    break;
                case SystemService.States.reset:
                    Reset();
                    break;
                case SystemService.States.lightAlarm:
                    break;
                case SystemService.States.stop:
                case SystemService.States.heavyAlarm:
                case SystemService.States.emg:
                    Stop();
                    
                    break;
            }
        }


        public int Start()
        {
            if (null == iSeq)
                return -1;

            return iSeq.Start();
        }

        public int Pause()
        {
            if (null == iSeq)
                return -1;

            return iSeq.Pause();
        }

        public int Resume()
        {
            if (null == iSeq)
                return -1;

            return iSeq.Resume();
        }

        public int Stop()
        {
            if (null == iSeq)
                return -1;

            return iSeq.Stop();
        }

        public int End()
        {
            if (null == iSeq)
                return -1;

            return iSeq.End();
        }

        public int Reset()
        {
            if (null == iSeq)
                return -1;

            return iSeq.Reset();
        }

        public int SetSequence(int seq, int type = 0, double value = 0.0)
        {
            if (null == iSeq)
                return -1;

            return iSeq.SetSequence(seq, type, value);
        }

        public bool IsFlag(int flag)
        {
            if (null == iSeq)
                return false;

            return iSeq.IsFlag(flag);
        }

        public void SetHWindow(HWindow hWindow)
        {
            if (null == iSeq)
                return; ;

            iSeq.SetHWindow(hWindow);
        }

        public int SetLight(int vision, bool on)
        {
            if (null == iSeq)
                return -1;

            return iSeq.SetLight(vision, on);
        }

        public int CheckMachine(int type = 0, bool isStop = true)
        {
            if (null == iSeq)
                return -1;

            return iSeq.CheckMachine(type, isStop);
        }
    }
}
