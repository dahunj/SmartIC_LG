using System;
using System.Threading;
using System.Windows.Threading;

using HalconDotNet;

namespace SmartICAVI
{
    class SeqManual : ISequence
    {
        #region Variables
        protected bool IsRun { get; set;}
        protected bool IsPause { get; set; }
        protected bool IsStop { get; set; }

        //protected bool IsBackFeeding { get; set; }   // 재검사 시 BackFeeding 시 true : Pause ->Resume 할 때 이 값이 true 이면 Resume 에서 재 피딩 하지 않고, BackFeedProcess 에서 처리한다.

        protected int NextPunchInspect { get; set; }  // 다음 펀치 검사 인덱스
        
        protected DataService dataService = null;
        protected DioService dioService = null;
        protected AioService aioService = null;
        protected CntService cntService = null;
        protected MotionService motionService = null;
        protected SequenceService seqService = null;
        protected SystemService sysService = null;
        protected MsgService msgService = null;
        protected InspectService inspectSrvice = null;
        protected GrabService grabService = null;
        protected StrobeService strobeService = null;
        protected MesService mesService = null;

        protected VisionTopService topService = null;
        protected VisionTop2Service top2Service = null;
        protected VisionBottomService bottomService = null;
        protected VisionBottom2Service bottom2Service = null;
        protected VisionMonoService monoService = null;
        protected VisionMono2Service mono2Service = null;

        protected bool[] flags = new bool[32];

        protected bool[] lights = new bool[6];

        protected Thread threadUncoiler = null;
        protected Thread threadRecoiler = null;
        protected Thread threadRun = null;

        protected Thread threadVision = null;
        protected Thread threadPunch = null;

        protected HWindow hWindow;

        //protected double posBuffer = 0.0;

        protected DateTime timeout = new DateTime();
        protected TimeSpan timeoutSpan;

        private DispatcherTimer timerUncoiler = null;
        private DispatcherTimer timerRecoiler = null;

        private int countPunchUp = 0;
        #endregion

        public SeqManual()
        {
            mode = (int)SystemService.Modes.manual;

            NextPunchInspect = 0;
            countPunchUp = 0;

            for (int i = 0; i < 6; ++i)
                lights[i] = false;
        }

        #region Interfaces
        protected bool isInitialized = false;
        public bool IsInitialized
        {
            get { return isInitialized; }
        }

        protected int mode = 0;
        public int Mode { get { return mode; } }

        public int Initialize()
        {
            isInitialized = false;

            dataService = DataService.Singleton;
            dioService = DioService.Singleton;
            aioService = AioService.Singleton;
            motionService = MotionService.Singleton;
            seqService = SequenceService.Singleton;
            sysService = SystemService.Singleton;
            msgService = MsgService.Singleton;
            inspectSrvice = InspectService.Singleton;
            grabService = GrabService.Singleton;
            cntService = CntService.Singleton;
            strobeService = StrobeService.Singleton;
            mesService = MesService.Singleton;

            topService = VisionTopService.Singleton;
            top2Service = VisionTop2Service.Singleton;
            bottomService = VisionBottomService.Singleton;
            bottom2Service = VisionBottom2Service.Singleton;
            monoService = VisionMonoService.Singleton;
            mono2Service = VisionMono2Service.Singleton;


            IsRun = false;
            IsPause = false;
            IsStop = false;

            for (int i = 0; i < flags.Length; ++i)
                flags[i] = false;

            
            isInitialized = true;

            ResetFlag((int)EnumSmartIC.SeqFlags.uncoilerRunning);
            ResetFlag((int)EnumSmartIC.SeqFlags.recoilerRunning);

            return 0;
        }

        public int UnInitialize()
        {
            Stop();

            EndProcess();

            ResetFlag((int)EnumSmartIC.SeqFlags.autoJogRun);
            ResetFlag((int)EnumSmartIC.SeqFlags.uncoilerRun);
            ResetFlag((int)EnumSmartIC.SeqFlags.recoilerRun);

            //int flagRun = (int)EnumSmartIC.SeqFlags.uncoilerRun;
            //int flagReady = (int)EnumSmartIC.SeqFlags.uncoilerReady;

            SetTimeout(500);
            while (false == IsTimeout())
            {
                if (false == IsFlag((int)EnumSmartIC.SeqFlags.uncoilerRun))
                    if (false == IsFlag((int)EnumSmartIC.SeqFlags.uncoilerReady))
                        break;
            }


            SetTimeout(500);
            while (false == IsTimeout())
            {
                if (false == IsFlag((int)EnumSmartIC.SeqFlags.recoilerRun))
                    if (false == IsFlag((int)EnumSmartIC.SeqFlags.recoilerReady))
                        break;
            }

            isInitialized = false;

            return 0;
        }

        public int Start()
        {
            IsRun = true;
            IsPause = false;
            IsStop = false;

            ResetFlag((int)EnumSmartIC.SeqFlags.autoJogRun);

            //threadRun = new Thread(new ThreadStart(this.ThreadRun));
            //threadRun.Start();

            threadRun = new Thread(ThreadRun);
            threadRun.IsBackground = true;
            threadRun.Start();

            return 0;
        }

        public int Pause()
        {
            IsPause = true;

            return 0;
        }

        public int Resume()
        {
            IsPause = false;

            return 0;
        }

        public int Stop()
        {
            IsRun = false;
            IsPause = false;
            IsStop = true;

            ResetFlag((int)EnumSmartIC.SeqFlags.bufferRun);
            ResetFlag((int)EnumSmartIC.SeqFlags.autoJogRun);

            if (null != motionService)
            {
                motionService.Stop((int)EnumSmartIC.Axis.visionFeed);
                motionService.Stop((int)EnumSmartIC.Axis.visionTop);
                motionService.Stop((int)EnumSmartIC.Axis.visionBottom);
                motionService.Stop((int)EnumSmartIC.Axis.punchX);
                motionService.Stop((int)EnumSmartIC.Axis.punchY);
                motionService.Stop((int)EnumSmartIC.Axis.punchFeed);
            }

            
                        
            if (null != dioService)
            {
                dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
                dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);
            }

            topService.SetStatusUpdate();
            top2Service.SetStatusUpdate();
            bottomService.SetStatusUpdate();
            bottom2Service.SetStatusUpdate();
            monoService.SetStatusUpdate();
            mono2Service.SetStatusUpdate();

            cntService.ResetTriggerEnable(0);
            cntService.ResetTriggerEnable(1);
            cntService.ResetTriggerEnable(2);
            cntService.ResetTriggerEnable(3);

            

            if (null != aioService)
            {
                int axis = (int)EnumSmartIC.Axis.uncoilerReel;
                aioService.SetOutvalue(0, dataService.DataMotion[axis].Voffset);
                axis = (int)EnumSmartIC.Axis.recoilerReel;
                aioService.SetOutvalue(1, dataService.DataMotion[axis].Voffset);
            }


            // Punch Up
            dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
            dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);

            // Cleaner Off
            ResetCleanRoller();

            //Ionizer Off
            dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer1On);
            dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer2On);
            dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer3On);
            dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer4On);
            dioService.ResetOutport((int)EnumSmartIC.Outports.loaderIonizerBlow);
            dioService.ResetOutport((int)EnumSmartIC.Outports.unloaderIonizerBlow);

            StopUncoilerRun();
            StopRecoilerRun();
            
            return 0;
        }

        public int End()
        {
            return 0;
        }

        public int Reset()
        {
            IsStop = false;

            topService.SetStatusUpdate();
            top2Service.SetStatusUpdate();
            bottomService.SetStatusUpdate();
            bottom2Service.SetStatusUpdate();
            monoService.SetStatusUpdate();
            mono2Service.SetStatusUpdate();

            dioService.ResetOutport((int)EnumSmartIC.Outports.buzzer1);
            dioService.ResetOutport((int)EnumSmartIC.Outports.buzzer2);

            return 0;
        }

        public virtual void RunProcess()
        {
            return;
        }

        public int SetSequence(int seq, int type, double value = 0)
        {
            EnumSmartIC.Sequences enumType = (EnumSmartIC.Sequences)seq;

            switch (enumType)
            {
                case EnumSmartIC.Sequences.uncoilerBuffer:
                    if (1 == type)
                    {
                        StartUncoilerRun();
                    }
                    else
                    {
                        StopUncoilerRun();
                    }
                    break;

                case EnumSmartIC.Sequences.recoilerBuffer:
                    if (1 == type)
                    {
                        StartRecoilerRun();
                    }
                    else
                    {
                        StopRecoilerRun();
                    }
                    break;

                case EnumSmartIC.Sequences.bufferInit:
                    if (1 == type)
                    {
                        SetBufferInit();
                    }
                    break;

                case EnumSmartIC.Sequences.bufferReference:
                    if (1 == type)
                    {
                        if (false == IsFlag((int)EnumSmartIC.SeqFlags.bufferRun))
                        {
                            SetFlag((int)EnumSmartIC.SeqFlags.bufferRun);
                            ResetFlag((int)EnumSmartIC.SeqFlags.bufferReady);
                            SetBufferReference(value);
                        }
                    }
                    else
                    {
                        ResetFlag((int)EnumSmartIC.SeqFlags.bufferRun);
                    }
                    break;

                case EnumSmartIC.Sequences.visionForward:
                    if( 1==type )
                        SetFlag((int)EnumSmartIC.SeqFlags.visionForward);
                    else
                        ResetFlag((int)EnumSmartIC.SeqFlags.visionForward);
                    break;

                case EnumSmartIC.Sequences.visionBackward:
                    if (1 == type)
                        SetFlag((int)EnumSmartIC.SeqFlags.visionBackward);
                    else
                        ResetFlag((int)EnumSmartIC.SeqFlags.visionBackward);
                    break;

                case EnumSmartIC.Sequences.punchForward:
                    if (1 == type)
                        SetFlag((int)EnumSmartIC.SeqFlags.punchForward);
                    else
                        ResetFlag((int)EnumSmartIC.SeqFlags.punchForward);
                    break;

                case EnumSmartIC.Sequences.punchBackward:
                    if (1 == type)
                        SetFlag((int)EnumSmartIC.SeqFlags.punchBackward);
                    else
                        ResetFlag((int)EnumSmartIC.SeqFlags.punchBackward);
                    break;

                case EnumSmartIC.Sequences.feedJogN:
                case EnumSmartIC.Sequences.feedJogP:
                case EnumSmartIC.Sequences.feedRMoveN:
                case EnumSmartIC.Sequences.feedRMoveP:
                case EnumSmartIC.Sequences.feedAMove:
                    if (1 == type)
                        return SetManualFeed(seq, type, value);
                    else
                        SetFlag((int)EnumSmartIC.SeqFlags.manualFeedStop);
                    break;

                case EnumSmartIC.Sequences.teachTop:
                case EnumSmartIC.Sequences.teachBottom:
                case EnumSmartIC.Sequences.teachMono:
                    return SetTeachScan(seq, type, value);

                case EnumSmartIC.Sequences.teachPunch:
                    return SetTeachPunch();

                case EnumSmartIC.Sequences.teachToZero:
                    return SetTeachToZero();

                case EnumSmartIC.Sequences.teachPunchTest:
                    return SetPunchTest(type, (int)value);

                case EnumSmartIC.Sequences.initSearch:
                    if (1 == type)
                        return SetInitSearch();
                    break;

                case EnumSmartIC.Sequences.initRMove:
                    if (1 == type)
                    {
                        return SetInitRMove(value);
                    }
                    break;

                case EnumSmartIC.Sequences.autoJog:                    
                    if (1 == type)
                    {
                        if (false == IsFlag((int)EnumSmartIC.SeqFlags.autoJogRun))
                        {
                            SetFlag((int)EnumSmartIC.SeqFlags.autoJogRun);
                            return SetAutoJog();
                        }
                    }
                    else
                    {
                        ResetFlag((int)EnumSmartIC.SeqFlags.autoJogRun);
                    }
                    break;

                case EnumSmartIC.Sequences.autoJogP:
                case EnumSmartIC.Sequences.autoJogN:
                    if (1 == type)
                        return SetAutoJog(seq, type, value);
                    else
                        ResetFlag((int)EnumSmartIC.SeqFlags.autoJogStop);
                    break;

                case EnumSmartIC.Sequences.backFeeding:
                    return SetBackFeeding();

                case EnumSmartIC.Sequences.laserMove:
                    return LastPunchMove();

                default:
                    break;
            }
            return 0;
        }

        public bool IsFlag(int flag)
        {
            if (0 > flag)
                return false;
            if (flags.Length < flag)
                return false;

            return flags[flag];
        }

        public void SetHWindow(HWindow hWindow)
        {
            this.hWindow = hWindow;
        }

        public int SetLight(int light, bool on)
        {
            if (0 > light || 5 < light)
                return -1;

            lights[light] = on;

            if( null != dataService )
            {
                switch (light)
                {
                    case 0:     // Top
                    case 1:
                        dataService.IsLightOnTop = on;
                        break;
                    case 2:     // Bottom
                    case 3:
                        dataService.IsLightOnBottom = on;
                        break;
                    case 4:     // Mono
                    case 5:
                        dataService.IsLightOnMono = on;
                        break;
                }
            }

            if (true == on)
            {
                dioService.SetOutport((int)EnumSmartIC.Outports.topVisionCooling);
                dioService.SetOutport((int)EnumSmartIC.Outports.bottomVisionCooling);
            }
            else
            {
                for (int i = 0; i < 6; ++i)
                {
                    if (true == lights[i])
                        return 0;
                }

                dioService.ResetOutport((int)EnumSmartIC.Outports.topVisionCooling);
                dioService.ResetOutport((int)EnumSmartIC.Outports.bottomVisionCooling);
            }

            return 0;
        }

        public int CheckMachine(int type=0, bool isStop = true)
        {
            int ret = 0;

            switch (type)
            {
                case 1:
                    return CheckSensor(isStop);
                case 2:
                    return CheckLimit(isStop);
                case 3:
                    return CheckPunchUp(isStop);
                default:
                    if (0 != CheckSensor(isStop))
                        ret = -1;

                    if (0 != CheckLimit(isStop))
                        ret = -1;

                    if (0 != CheckPunchUp(isStop))
                        ret = -1;   
                    break;

            }

            return ret;
        }

        #endregion

        protected int SetUncoilerDancer()
        {
            //threadUncoiler = new Thread(new ThreadStart(this.ThreadUncoiler));
            //threadUncoiler.Start();

            dataService.IsUncoilerRun = true;
            dataService.IsUncoilerReady = false;

            threadUncoiler = new Thread(ThreadUncoiler);
            threadUncoiler.IsBackground = true;
            threadUncoiler.Start();

            return 0;
        }

        protected int SetRecoilerDancer()
        {
            //threadRecoiler = new Thread(new ThreadStart(this.ThreadRecoiler));
            //threadRecoiler.Start();

            dataService.IsRecoilerRun = true;
            dataService.IsRecoilerReady = false;

            threadRecoiler = new Thread(ThreadRecoiler);
            threadRecoiler.IsBackground = true;
            threadRecoiler.Start();

            return 0;
        }

        protected void ThreadUncoiler()
        {
            int axisFeed = (int)EnumSmartIC.Axis.visionFeed;
            //int axis = (int)EnumSmartIC.Axis.uncoilerReel;
            //int channel = 0;
            //int flagRun = (int)EnumSmartIC.SeqFlags.uncoilerRun;
            //int flagReady = (int)EnumSmartIC.SeqFlags.uncoilerReady;
            //int flagRunning = (int)EnumSmartIC.SeqFlags.uncoilerRunning;
            //double dir = -1.0;
            //double pos = dataService.DataSystem.UncoilerReference;

            //DoEvents(5000);

            while (false == motionService.IsMotionDone(axisFeed))
            {
                DoEvents(100);
            }

            //if (true == dioService.IsInportOn((int)EnumSmartIC.Inports.uncoilerReelDir))
            //    dir = 1.0;

            Log_Trace.WriteLine("Dancer Control (Uncoiler) : Start");
            //DancerControl(axis, channel, flagRun, flagReady, flagRunning, dir, pos);
            DancerControl_Uncoiler();
            Log_Trace.WriteLine("Dancer Control (Uncoiler) : End");
        }

        protected void ThreadRecoiler()
        {
            int axisFeed = (int)EnumSmartIC.Axis.punchFeed;
            //int axis = (int)EnumSmartIC.Axis.recoilerReel;
            //int channel = 1;
            //int flagRun = (int)EnumSmartIC.SeqFlags.recoilerRun;
            //int flagReady = (int)EnumSmartIC.SeqFlags.recoilerReady;
            //int flagRunning = (int)EnumSmartIC.SeqFlags.recoilerRunning;
            //double dir = 1.0;
            //double pos = dataService.DataSystem.RecoilerReference;

            while (false == motionService.IsMotionDone(axisFeed))
            {
                DoEvents(100);
            }

            //if (true == dioService.IsInportOn((int)EnumSmartIC.Inports.recoilerReelDir))
            //    dir = -1.0;

            Log_Trace.WriteLine("Dancer Control (Recoiler) : Start");
            //DancerControl(axis, channel, flagRun, flagReady, flagRunning, dir, pos);
            DancerControl_Recoiler();
            Log_Trace.WriteLine("Dancer Control (Recoiler) : End");
            
        }

        protected void ThreadRun()
        {
            RunProcess();
        }

        protected virtual int DancerControl(int axis, int channel, int flagRun, int flagReady, int flagRunning, double dir=1.0, double pos = 160.0)
        {
            if (null == aioService)
                return -1;
            if (null == dioService)
                return -1;
            if (null == motionService)
                return -1;

            int step = 0;

            double DMVn = 0.0;
            double MVn_1 = 0.0;
            double vac = 0.0;

            double En = 0.0, En_1 = 0.0, En_2 = 0.0;

            double Kp = 1.0;
            double Ki = 0.0;
            double Kd = 0.0;

            double Vmin = 0.1;
            double Vmax = 4.0;
            double Inpos = 0.1;

            double accel = 0.001;

            ResetFlag(flagReady);

            DateTime start = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 100);
            DateTime timeout = start.Add(duration);

            int oldStep = -1;

            while (true == IsFlag(flagRun) )
            {
                if (oldStep != step)
                {
                    Log_Trace.WriteLine("Dancer Control ({0}) : {1}", flagRun, step);
                    oldStep = step;
                }

                switch (step)
                {
                    case 0:
                        //motionService.SetServoOn(axis, true);

                        //start = DateTime.Now;
                        //duration = new TimeSpan(0, 0, 0, 0, 100);
                        //timeout = start.Add(duration);

                        SetFlag(flagRunning);

                        step += 10;
                        break;
                    case 10:
                        //if (timeout < DateTime.Now)
                            step += 10;
                        break;
                    case 20:
                        //aioService.SetOutvalue(channel, 0.0);
                        aioService.SetOutvalue(channel, dataService.DataMotion[axis].Voffset);
                        
                        En = En_1 = En_2 = 0.0;

                        Kp = dataService.DataMotion[axis].Kp0;
                        Ki = dataService.DataMotion[axis].Ki0;
                        Kd = dataService.DataMotion[axis].Kd0;
                        Vmin = dataService.DataMotion[axis].Vmin0;
                        Vmax = dataService.DataMotion[axis].Vmax0;
                        Inpos = dataService.DataMotion[axis].Inpos0;

                        accel = 0.01;

                        step += 10;

                        break;

                    case 30:    // Start
                        En = (pos - motionService.GetCurrentPosition(axis));

                        DMVn = Kp * En + Ki * (En - En_1) + Kd * ((En - En_1) - (En_1 - En_2));

                        //System.Diagnostics.Debug.WriteLine(DMVn.ToString("0.000"));

                        vac = DMVn * accel;

                        //vac = (DMVn * Vmax / 20.0) * accel;

                        if (vac > Vmax)
                            vac = Vmax;
                        else if (vac < -Vmax)
                            vac = -Vmax;

                        if (Inpos > Math.Abs(En))
                        {
                            vac = 0.0;
                            SetFlag(flagReady);

                            aioService.SetOutvalue(channel, vac);

                            Kp = dataService.DataMotion[axis].Kp;
                            Ki = dataService.DataMotion[axis].Ki;
                            Kd = dataService.DataMotion[axis].Kd;
                            Vmin = dataService.DataMotion[axis].Vmin;
                            Vmax = dataService.DataMotion[axis].Vmax;
                            Inpos = dataService.DataMotion[axis].Inpos;

                            step = 100;
                        }
                        else
                        {
                            if (accel >= 1.0)
                            {
                                if (0.0 <= vac)
                                {
                                    if (Vmin > vac)
                                        vac = Vmin;
                                }
                                else
                                {
                                    if (-Vmin < vac)
                                        vac = -Vmin;
                                }
                            }
                            aioService.SetOutvalue(channel, vac * dir);

                            //System.Diagnostics.Debug.WriteLine(vac.ToString("0.000"));
                        }

                        En_2 = En_1;
                        En_1 = En;
                        MVn_1 = DMVn;

                        if ((accel += 0.001) >= 1.0)
                            accel = 1.0;
                        //System.Diagnostics.Debug.WriteLine(vac.ToString("0.000"));
                        break;
                    case 100:       // Run
                        En = (pos - motionService.GetCurrentPosition(axis));

                        DMVn = Kp * En + Ki * (En - En_1) + Kd * ((En - En_1) - (En_1 - En_2));

                        vac = DMVn;
                        //vac = (DMVn * Vmax / 20.0) * accel;

                        if (vac > Vmax)
                            vac = Vmax;
                        else if (vac < -Vmax)
                            vac = -Vmax;

                        //if (Inpos > Math.Abs(En))
                        //{
                        //    vac = 0.0;

                        //    aioService.SetOutvalue(channel, vac * dir);
                        //}
                        //else
                        //{
                        //    if (0.0 <= vac)
                        //    {
                        //        if (Vmin > vac)
                        //            vac = Vmin;
                        //    }
                        //    else
                        //    {
                        //        if (-Vmin < vac)
                        //            vac = -Vmin;
                        //    }

                        //    aioService.SetOutvalue(channel, vac * dir);
                        //}

                        aioService.SetOutvalue(channel, vac * dir);

                        En_2 = En_1;
                        En_1 = En;
                        MVn_1 = DMVn;
                        break;
                    default:
                        ResetFlag(flagRun);
                        ResetFlag(flagReady);
                        ResetFlag(flagRunning);
                        //aioService.SetOutvalue(channel, 0.0);

                        aioService.SetOutvalue(channel, dataService.DataMotion[axis].Voffset);

                        //motionService.SetServoOn(axis, false);
                        return -1;
                }
                //System.Windows.Forms.Application.DoEvents();
                DoEvents(10);
            }

            ResetFlag(flagRun);
            ResetFlag(flagReady);
            ResetFlag(flagRunning);

            //aioService.SetOutvalue(channel, 0.0);
            aioService.SetOutvalue(channel, dataService.DataMotion[axis].Voffset);

            //motionService.SetServoOn(axis, false);

            Log_Trace.WriteLine("Stop Dancer {0}", flagRun);


            return 0;
        }

        protected virtual int DancerControl_Uncoiler()
        {
            if (null == aioService)
                return -1;
            if (null == dioService)
                return -1;
            if (null == motionService)
                return -1;

            int step = 0;

            double DMVn = 0.0;
            double MVn_1 = 0.0;
            double vac = 0.0;

            double En = 0.0, En_1 = 0.0, En_2 = 0.0;

            double Kp = 1.0;
            double Ki = 0.0;
            double Kd = 0.0;

            double Vmin = 0.1;
            double Vmax = 4.0;
            double Inpos = 0.1;

            double accel = 0.001;

            int axis = (int)EnumSmartIC.Axis.uncoilerReel;
            int channel = 0;
            double dir = -1.0;
            double pos = dataService.DataSystem.UncoilerReference;

            if (true == dioService.IsInportOn((int)EnumSmartIC.Inports.uncoilerReelDir))
                dir = 1.0;

            DateTime start = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 100);
            DateTime timeout = start.Add(duration);

            while (true == dataService.IsUncoilerRun)
            {
                switch (step)
                {
                    case 0:
                        step += 10;
                        break;
                    case 10:
                        step += 10;
                        break;
                    case 20:
                        aioService.SetOutvalue(channel, dataService.DataMotion[axis].Voffset);

                        En = En_1 = En_2 = 0.0;

                        Kp = dataService.DataMotion[axis].Kp0;
                        Ki = dataService.DataMotion[axis].Ki0;
                        Kd = dataService.DataMotion[axis].Kd0;
                        Vmin = dataService.DataMotion[axis].Vmin0;
                        Vmax = dataService.DataMotion[axis].Vmax0;
                        Inpos = dataService.DataMotion[axis].Inpos0;

                        accel = 0.01;

                        step += 10;

                        break;

                    case 30:    // Start
                        En = (pos - motionService.GetCurrentPosition(axis));

                        DMVn = Kp * En + Ki * (En - En_1) + Kd * ((En - En_1) - (En_1 - En_2));

                        vac = DMVn * accel;

                        if (vac > Vmax)
                            vac = Vmax;
                        else if (vac < -Vmax)
                            vac = -Vmax;

                        if (Inpos > Math.Abs(En))
                        {
                            vac = 0.0;

                            aioService.SetOutvalue(channel, vac);

                            Kp = dataService.DataMotion[axis].Kp;
                            Ki = dataService.DataMotion[axis].Ki;
                            Kd = dataService.DataMotion[axis].Kd;
                            Vmin = dataService.DataMotion[axis].Vmin;
                            Vmax = dataService.DataMotion[axis].Vmax;
                            Inpos = dataService.DataMotion[axis].Inpos;

                            dataService.IsUncoilerReady = true;

                            step = 100;
                        }
                        else
                        {
                            if (accel >= 1.0)
                            {
                                if (0.0 <= vac)
                                {
                                    if (Vmin > vac)
                                        vac = Vmin;
                                }
                                else
                                {
                                    if (-Vmin < vac)
                                        vac = -Vmin;
                                }
                            }
                            aioService.SetOutvalue(channel, vac * dir);
                        }

                        En_2 = En_1;
                        En_1 = En;
                        MVn_1 = DMVn;

                        if ((accel += 0.001) >= 1.0)
                            accel = 1.0;
                        break;
                    case 100:       // Run
                        En = (pos - motionService.GetCurrentPosition(axis));

                        DMVn = Kp * En + Ki * (En - En_1) + Kd * ((En - En_1) - (En_1 - En_2));

                        vac = DMVn;
                        
                        if (vac > Vmax)
                            vac = Vmax;
                        else if (vac < -Vmax)
                            vac = -Vmax;

                        aioService.SetOutvalue(channel, vac * dir);

                        En_2 = En_1;
                        En_1 = En;
                        MVn_1 = DMVn;
                        break;
                    default:
                        
                        aioService.SetOutvalue(channel, dataService.DataMotion[axis].Voffset);

                        dataService.IsUncoilerReady = false;
                        dataService.IsUncoilerRun = false;
                        return -1;
                }
                //System.Windows.Forms.Application.DoEvents();
                DoEvents(10);
            }

            aioService.SetOutvalue(channel, dataService.DataMotion[axis].Voffset);

            dataService.IsUncoilerReady = false;
            dataService.IsUncoilerRun = false;

            return 0;
        }

        protected virtual int DancerControl_Recoiler()
        {
            if (null == aioService)
                return -1;
            if (null == dioService)
                return -1;
            if (null == motionService)
                return -1;

            int step = 0;

            double DMVn = 0.0;
            double MVn_1 = 0.0;
            double vac = 0.0;

            double En = 0.0, En_1 = 0.0, En_2 = 0.0;

            double Kp = 1.0;
            double Ki = 0.0;
            double Kd = 0.0;

            double Vmin = 0.1;
            double Vmax = 4.0;
            double Inpos = 0.1;

            double accel = 0.001;

            int axis = (int)EnumSmartIC.Axis.recoilerReel;
            int channel = 1;
            double dir = 1.0;
            double pos = dataService.DataSystem.RecoilerReference;

            if (true == dioService.IsInportOn((int)EnumSmartIC.Inports.recoilerReelDir))
                dir = -1.0;

            DateTime start = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 100);
            DateTime timeout = start.Add(duration);

            //System.Diagnostics.Debug.WriteLine("Dancer Start");

            while (true == dataService.IsRecoilerRun)
            {
                switch (step)
                {
                    case 0:
                        //System.Diagnostics.Debug.WriteLine("Dancer Step = 0");
                        step += 10;
                        break;
                    case 10:
                        //System.Diagnostics.Debug.WriteLine("Dancer Step = 10");
                        step += 10;
                        break;
                    case 20:
                        //System.Diagnostics.Debug.WriteLine("Dancer Step = 20");
                        aioService.SetOutvalue(channel, dataService.DataMotion[axis].Voffset);

                        En = En_1 = En_2 = 0.0;

                        Kp = dataService.DataMotion[axis].Kp0;
                        Ki = dataService.DataMotion[axis].Ki0;
                        Kd = dataService.DataMotion[axis].Kd0;
                        Vmin = dataService.DataMotion[axis].Vmin0;
                        Vmax = dataService.DataMotion[axis].Vmax0;
                        Inpos = dataService.DataMotion[axis].Inpos0;

                        accel = 0.01;

                        step += 10;

                        break;

                    case 30:    // Start

                        //System.Diagnostics.Debug.WriteLine("Dancer Step = 30");

                        En = (pos - motionService.GetCurrentPosition(axis));

                        DMVn = Kp * En + Ki * (En - En_1) + Kd * ((En - En_1) - (En_1 - En_2));

                        //System.Diagnostics.Debug.WriteLine(DMVn.ToString("0.000"));

                        vac = DMVn * accel;

                        //vac = (DMVn * Vmax / 20.0) * accel;

                        if (vac > Vmax)
                            vac = Vmax;
                        else if (vac < -Vmax)
                            vac = -Vmax;

                        if (Inpos > Math.Abs(En))
                        {
                            vac = 0.0;

                            aioService.SetOutvalue(channel, vac);

                            Kp = dataService.DataMotion[axis].Kp;
                            Ki = dataService.DataMotion[axis].Ki;
                            Kd = dataService.DataMotion[axis].Kd;
                            Vmin = dataService.DataMotion[axis].Vmin;
                            Vmax = dataService.DataMotion[axis].Vmax;
                            Inpos = dataService.DataMotion[axis].Inpos;

                            dataService.IsRecoilerReady = true;

                            step = 100;
                        }
                        else
                        {
                            if (accel >= 1.0)
                            {
                                if (0.0 <= vac)
                                {
                                    if (Vmin > vac)
                                        vac = Vmin;
                                }
                                else
                                {
                                    if (-Vmin < vac)
                                        vac = -Vmin;
                                }
                            }
                            aioService.SetOutvalue(channel, vac * dir);
                        }

                        En_2 = En_1;
                        En_1 = En;
                        MVn_1 = DMVn;

                        if ((accel += 0.001) >= 1.0)
                            accel = 1.0;
                        break;
                    case 100:       // Run
                        En = (pos - motionService.GetCurrentPosition(axis));

                        DMVn = Kp * En + Ki * (En - En_1) + Kd * ((En - En_1) - (En_1 - En_2));

                        vac = DMVn;

                        if (vac > Vmax)
                            vac = Vmax;
                        else if (vac < -Vmax)
                            vac = -Vmax;

                        aioService.SetOutvalue(channel, vac * dir);

                        En_2 = En_1;
                        En_1 = En;
                        MVn_1 = DMVn;
                        break;
                    default:

                        //System.Diagnostics.Debug.WriteLine("Dancer Default = " + step.ToString());

                        aioService.SetOutvalue(channel, dataService.DataMotion[axis].Voffset);

                        dataService.IsRecoilerReady = false;
                        dataService.IsRecoilerRun = false;
                        return -1;
                }
                //System.Windows.Forms.Application.DoEvents();
                DoEvents(10);
            }
            
            aioService.SetOutvalue(channel, dataService.DataMotion[axis].Voffset);
            //System.Diagnostics.Debug.WriteLine("Dancer End : " + dataService.IsRecoilerRun.ToString());

            dataService.IsRecoilerReady = false;
            dataService.IsRecoilerRun = false;


            return 0;
        }

        protected virtual int SetBufferReference(double tolerance = 0.01)
        {
            if (null == aioService)
                return -1;
            if (null == dioService)
                return -1;
            if (null == motionService)
                return -1;

            int step = 0;
            int flagRun = (int)EnumSmartIC.SeqFlags.bufferRun;
            int flagReady = (int)EnumSmartIC.SeqFlags.bufferReady;
            int axis = (int)EnumSmartIC.Axis.buffer;
            int axisFeed = (int)EnumSmartIC.Axis.punchFeed;

            double reference = dataService.DataSystem.BufferReference;
            double pos = motionService.GetCurrentPosition(axis);
            //double vel = 120.0;
            //double oldVel = 0.0;
            //double velMax = 120.0;
            //double slowdown = 10.0;
            double movePos = 0.0;

            double percent = 1.0;

            if (tolerance < 0.01)
                tolerance = 0.01;

            ResetFlag(flagReady);

            while (true == IsFlag(flagRun))
            {
                switch (step)
                {
                    case 0:
                        step += 10;
                        break;

                    case 10:       // + 이동
                        pos = motionService.GetCurrentPosition(axis);

                        if (tolerance > Math.Abs(reference - pos))
                        {
                            step = 20000;
                        }
                        else
                        {
                            movePos = (reference - pos) * 4.0;
                            motionService.RMove(axisFeed, movePos, 120.0, 120.0);
                            step += 10;
                        }
                        break;
                    case 20:
                        if (true == motionService.IsMotionDone(axisFeed))
                            step += 10;
                        break;
                    case 30:
                        SetTimeout(300);
                        step += 10;
                        break;
                    case 40:
                        step = 100;
                        break;

                    case 100:
                        if (true == IsTimeout())
                        {
                            pos = motionService.GetCurrentPosition(axis);
                            movePos = (reference - pos) * 4.0;

                            if (tolerance > Math.Abs(reference - pos))
                            {
                                //System.Diagnostics.Debug.WriteLine(string.Format("{0:0.000}, {1:0.000}", pos, movePos));
                                step = 20000;
                            }
                            else
                                step += 10;
                        }
                        break;
                    case 110:       // 이동
                        pos = motionService.GetCurrentPosition(axis);
                        movePos = (reference - pos) * 4.0 * percent;

                        motionService.RMove(axisFeed, movePos, 50.0, 100.0);

                        step += 10;
                        break;
                    case 120:
                        if (true == motionService.IsMotionDone(axisFeed))
                        {
                            SetTimeout(300);
                            step += 10;
                        }
                        break;
                    case 130:
                        percent *= 0.9;

                        if (0.7 > percent)
                            percent = 0.7;

                        step = 100;
                        break;

                    case 20000:
                        ResetFlag(flagRun);
                        SetFlag(flagReady);
                        return 0;

                    default:
                        ResetFlag(flagRun);
                        ResetFlag(flagReady);

                        return -1;
                }
                System.Windows.Forms.Application.DoEvents();
                //DoEvents(1);
            }

            motionService.Stop(axisFeed);

            ResetFlag(flagRun);
            ResetFlag(flagReady);

            

            return 0;
        }

        protected virtual int SetBufferInit()     // 버퍼의 초기위치 확인
        {
            if (null == sysService)
                return -1;
            if (null == dioService)
                return -1;
            if (null == motionService)
                return -1;
            if (null == seqService)
                return -1;

            int step = 0;
            int oldStep = -1;

            int axisBuffer = (int)EnumSmartIC.Axis.buffer;
            int axisPunchFeed = (int)EnumSmartIC.Axis.punchFeed;

            seqService.IsBufferInitDone = false;

            while (SystemService.States.stop > sysService.State)
            {
                if (oldStep != step)
                {
                    oldStep = step;

                    //System.Diagnostics.Debug.WriteLine(string.Format("{0}", step));
                }
                switch (step)
                {
                    case 0:
                        step += 10;
                        SetTimeout(1000);
                        break;
                    case 10:            // Buffer Limit N 센서 확인
                        if (true == IsTimeout())
                        {
                            if (1 == motionService.GetStateLimitN(axisBuffer))
                            {
                                step = 100;     // Search Init
                            }
                            else
                            {
                                step += 10;     // - Move
                            }
                        }
                        break;
                    case 20:            // Feed Back
                        motionService.JogN(axisPunchFeed, 50.0, 200.0);
                        step += 10;
                        break;
                    case 30:            // Limit Check
                        if (1 == motionService.GetStateLimitN(axisBuffer))
                        {
                            motionService.Stop(axisPunchFeed);
                            step += 10;
                        }
                        break;
                    case 40:            // Wait Move Done
                        if (true == motionService.IsMotionDone(axisPunchFeed))
                            step += 10;
                        break;
                    case 50:            
                        step = 100;
                        break;


                    case 100:       // + Move
                        motionService.JogP(axisPunchFeed, 50.0, 200.0);
                        step += 10;
                        break;
                    case 110:       // Check Limit N off
                        if (0 == motionService.GetStateLimitN(axisBuffer))
                        {
                            motionService.Stop(axisPunchFeed);
                            step += 10;
                        }
                        break;
                    case 120:       // Wait Motion Done 
                        if (true == motionService.IsMotionDone(axisPunchFeed))
                        {
                            SetTimeout(1000);
                            step += 10;
                        }
                        break;
                    case 130:       // Recheck - Lime
                        if (true == IsTimeout())
                        {
                            motionService.JogN(axisPunchFeed, 5.0, 5.0);
                            step += 10;
                        }
                        break;
                    case 140:
                        if (1 == motionService.GetStateLimitN(axisBuffer))
                        {
                            motionService.Stop(axisPunchFeed);
                            step += 10;
                        }
                        break;
                    case 150:
                        if (true == motionService.IsMotionDone(axisPunchFeed))
                        {
                            SetTimeout(1000);
                            step += 10;
                        }
                        break;
                    case 160:
                        if (true == IsTimeout())
                        {
                            motionService.JogP(axisPunchFeed, 5.0, 5.0);
                            step += 10;
                        }
                        break;
                    case 170:
                        if (0 == motionService.GetStateLimitN(axisBuffer))
                        {
                            motionService.Stop(axisPunchFeed);
                            step += 10;
                        }
                        break;
                    case 180:
                        if (true == motionService.IsMotionDone(axisPunchFeed))
                        {
                            SetTimeout(1000);
                            step += 10;
                        }
                        break;
                    case 190:
                        if( true == IsTimeout() )
                            step += 10;
                        break;
                    case 200:
                        motionService.SetPosition(axisBuffer, 0.0);
                        step += 10;
                        break;
                    case 210:
                        step = 20000;
                        break;

                    case 20000:
                        seqService.IsBufferInitDone = true;
                        return 0;
                }
                System.Windows.Forms.Application.DoEvents();
            }

            motionService.Stop(axisPunchFeed);
            return 0;
        }

        protected virtual int SetManualFeed(int seq, int type, double value)
        {
            int step = 0;

            ResetFlag((int)EnumSmartIC.SeqFlags.visionForward);
            ResetFlag((int)EnumSmartIC.SeqFlags.visionBackward);
            ResetFlag((int)EnumSmartIC.SeqFlags.punchForward);
            ResetFlag((int)EnumSmartIC.SeqFlags.punchBackward);


            int axisVision = (int)EnumSmartIC.Axis.visionFeed;
            int axisPunch = (int)EnumSmartIC.Axis.punchFeed;

            int axisTop = (int)EnumSmartIC.Axis.visionTop;
            int axisBottom = (int)EnumSmartIC.Axis.visionBottom;

            double posPunch = 0.0;
            double posVision = 0.0;

            dataService.FeedVelocity = dataService.DataMotion[axisVision].VelRapid;

            bool isLimitCheck = false;


            if (0 != CheckSensor())
            {
                sysService.State = SystemService.States.stop;
                return -1;
            }

            if (0 != CheckPunchUp())
            {
                sysService.State = SystemService.States.stop;
                return -1;
            }

            if (0 != CheckSwitch())
            {
                sysService.State = SystemService.States.stop;
                return -1;
            }


            // Punch Up
            dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
            dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);

            // Cleaner On
            if (true == dataService.DataSystem.IsSelectedCleanRoller)
                SetCleanRoller();
            else
                ResetCleanRoller();

            //Ionizer On
            dioService.SetOutport((int)EnumSmartIC.Outports.ionizer1On);
            dioService.SetOutport((int)EnumSmartIC.Outports.ionizer2On);
            dioService.SetOutport((int)EnumSmartIC.Outports.ionizer3On);
            dioService.SetOutport((int)EnumSmartIC.Outports.ionizer4On);
            dioService.SetOutport((int)EnumSmartIC.Outports.loaderIonizerBlow);
            dioService.SetOutport((int)EnumSmartIC.Outports.unloaderIonizerBlow);


            while (SystemService.States.stop > sysService.State)
            {
                //if (oldStep != step)
                //{
                //    System.Diagnostics.Debug.WriteLine("{0}", step);
                //    oldStep = step;
                //}
                if (0 != CheckSensor())
                    return -1;
            
                if (true == isLimitCheck)
                    if (0 != CheckLimit())
                        return -1;

                switch (step)
                {
                    case 0:
                        ResetFlag((int)EnumSmartIC.SeqFlags.manualFeedStop);
                        step += 10;
                        break;
                    case 10:
                        //// Uncoiler 가동
                        //if (false == IsFlag((int)EnumSmartIC.SeqFlags.uncoilerReady))
                        //{
                        //    SetSequence((int)EnumSmartIC.Sequences.uncoilerBuffer, 1);
                        //    step += 10;
                        //}
                        if (false == dataService.IsUncoilerReady)
                        {
                            SetSequence((int)EnumSmartIC.Sequences.uncoilerBuffer, 1);
                            step += 10;
                        }
                        break;
                    case 20:
                        //// Recoiler 가동
                        //if (false == IsFlag((int)EnumSmartIC.SeqFlags.recoilerReady))
                        //{
                        //    SetSequence((int)EnumSmartIC.Sequences.recoilerBuffer, 1);
                        //    step += 10;
                        //}
                        if (false == dataService.IsRecoilerReady)
                        {
                            SetSequence((int)EnumSmartIC.Sequences.recoilerBuffer, 1);
                            step += 10;
                        }
                        break;
                    case 30:
                        // Uncoiler Ready Check
                        //if (true == IsFlag((int)EnumSmartIC.SeqFlags.uncoilerReady))
                        if (true == dataService.IsUncoilerReady)
                            step += 10;
                        break;
                    case 40:
                        //if (true == IsFlag((int)EnumSmartIC.SeqFlags.recoilerReady))
                        if (true == dataService.IsRecoilerReady)
                            step += 10;
                        break;
                    case 50:
                        if (false == seqService.IsBufferInitDone)
                            step += 10;
                        else
                            step = 100;
                        break;
                    case 60:
                        SetSequence((int)EnumSmartIC.Sequences.bufferInit, 1);
                        step += 10;
                        break;
                    case 70:
                        if (true == seqService.IsBufferInitDone)
                            step += 10;
                        break;
                    case 80:
                        step = 100;
                        break;

                    case 100:
                        SetSequence((int)EnumSmartIC.Sequences.bufferReference, 1, 10.0);
                        step += 10;
                        break;
                    case 110:
                        if (true == motionService.IsMotionDone(axisPunch))
                        {
                            isLimitCheck = true;
                            step += 10;
                        }
                        break;
                    case 120:
                        //motionService.AMove(axisTop, posTop, velTop, accelTop);
                        //motionService.AMove(axisBottom, posBottom, velBottom, accelBottom);
                        step += 10;
                        break;
                    case 130:
                        if (true == motionService.IsMotionDone(axisTop))
                            step += 10;
                        break;
                    case 140:
                        if (true == motionService.IsMotionDone(axisBottom))
                            step += 10;
                        break;
                    case 150:

                        step = 200;
                        break;

                    case 200:
                        if ((int)EnumSmartIC.Sequences.feedJogN == seq)
                            step = 1000;
                        else if ((int)EnumSmartIC.Sequences.feedJogP == seq)
                            step = 2000;
                        else if ((int)EnumSmartIC.Sequences.feedRMoveN == seq)
                            step = 3000;
                        else if ((int)EnumSmartIC.Sequences.feedRMoveP == seq)
                            step = 4000;
                        else if ((int)EnumSmartIC.Sequences.feedAMove == seq)
                            step = 5000;
                        break;

                    case 1000:      
                        // Jog N
                        motionService.JogN(axisVision, dataService.DataMotion[axisVision].VelRapid, dataService.DataMotion[axisVision].AccelRapid);
                        motionService.JogN(axisPunch, dataService.DataMotion[axisVision].VelRapid, dataService.DataMotion[axisVision].AccelRapid);
                        step += 10;
                        break;
                    case 1010:
                        if (true == IsFlag((int)EnumSmartIC.SeqFlags.manualFeedStop))
                        {
                            //ResetFlag((int)EnumSmartIC.SeqFlags.manualFeedStop);
                           
                            motionService.Stop(axisVision);
                            motionService.Stop(axisPunch);

                            step = 10000;
                        }
                        break;

                    case 2000:
                        // Jog P
                        motionService.JogP(axisVision, dataService.DataMotion[axisVision].VelRapid, dataService.DataMotion[axisVision].AccelRapid);
                        motionService.JogP(axisPunch, dataService.DataMotion[axisVision].VelRapid, dataService.DataMotion[axisVision].AccelRapid);
                        step += 10;
                        break;
                    case 2010:
                        if (true == IsFlag((int)EnumSmartIC.SeqFlags.manualFeedStop))
                        {
                            //ResetFlag((int)EnumSmartIC.SeqFlags.manualFeedStop);

                            motionService.Stop(axisVision);
                            motionService.Stop(axisPunch);

                            step = 10000;
                        }
                        break;


                    case 3000:
                        // R Move N
                        motionService.RMove(axisVision, -value, dataService.DataMotion[axisVision].VelRapid, dataService.DataMotion[axisVision].AccelRapid);
                        motionService.RMove(axisPunch, -value, dataService.DataMotion[axisVision].VelRapid, dataService.DataMotion[axisVision].AccelRapid);
                        step += 10;
                        break;
                    case 3010:
                        step = 10000;
                        break;


                    case 4000:
                        // R Move P
                        motionService.RMove(axisVision, value, dataService.DataMotion[axisVision].VelRapid, dataService.DataMotion[axisVision].AccelRapid);
                        motionService.RMove(axisPunch, value, dataService.DataMotion[axisVision].VelRapid, dataService.DataMotion[axisVision].AccelRapid);
                        step += 10;
                        break;
                    case 4010:
                        step = 10000;
                        break;


                    case 5000:
                        posVision = motionService.GetCurrentPosition(axisVision);
                        posPunch = value - posVision;
                        // A Move
                        motionService.AMove(axisVision, value, dataService.DataMotion[axisVision].VelRapid, dataService.DataMotion[axisVision].AccelRapid);
                        motionService.RMove(axisPunch, posPunch, dataService.DataMotion[axisVision].VelRapid, dataService.DataMotion[axisVision].AccelRapid);
                        step += 10;
                        break;
                    case 5010:
                        step = 10000;
                        break;


                        
                    case 10000:
                        if (true == IsFlag((int)EnumSmartIC.SeqFlags.manualFeedStop))
                        {
                            motionService.Stop(axisVision);
                            motionService.Stop(axisPunch);

                            //SetSequence((int)EnumSmartIC.Sequences.uncoilerBuffer, 0);
                            //SetSequence((int)EnumSmartIC.Sequences.recoilerBuffer, 0);

                            step += 10;

                            ResetFlag((int)EnumSmartIC.SeqFlags.manualFeedStop);
                        }
                        else
                        {
                            if (true == motionService.IsMotionDone(axisVision))
                            {
                                if (true == motionService.IsMotionDone(axisPunch))
                                    step += 10;
                            }
                        }
                        break;
                    case 10010:
                        step = 20000;
                        break;


                    case 20000:
                        motionService.Stop(axisVision);
                        motionService.Stop(axisPunch);

                        SetSequence((int)EnumSmartIC.Sequences.uncoilerBuffer, 0);
                        SetSequence((int)EnumSmartIC.Sequences.recoilerBuffer, 0);

                        // Punch Up
                        dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);

                        // Cleaner Off
                        ResetCleanRoller();

                        //Ionizer Off
                        dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer1On);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer2On);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer3On);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer4On);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.loaderIonizerBlow);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.unloaderIonizerBlow);

                        return 0;

                    default:
                        break;
                }
                System.Windows.Forms.Application.DoEvents();
            }

            // Punch Up
            dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
            dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);

            // Cleaner Off
            ResetCleanRoller();

            //Ionizer Off
            dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer1On);
            dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer2On);
            dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer3On);
            dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer4On);
            dioService.ResetOutport((int)EnumSmartIC.Outports.loaderIonizerBlow);
            dioService.ResetOutport((int)EnumSmartIC.Outports.unloaderIonizerBlow);


            SetSequence((int)EnumSmartIC.Sequences.uncoilerBuffer, 0);
            SetSequence((int)EnumSmartIC.Sequences.recoilerBuffer, 0);

            return -1;
        }

        protected virtual int SetAutoJog(int seq, int type, double value)
        {
            int step = 0;

            ResetFlag((int)EnumSmartIC.SeqFlags.visionForward);
            ResetFlag((int)EnumSmartIC.SeqFlags.visionBackward);
            ResetFlag((int)EnumSmartIC.SeqFlags.punchForward);
            ResetFlag((int)EnumSmartIC.SeqFlags.punchBackward);


            int axisVision = (int)EnumSmartIC.Axis.visionFeed;
            int axisPunch = (int)EnumSmartIC.Axis.punchFeed;

            if (0.0 >= value)
                value = 1.0;

            double vel = dataService.DataMotion[axisVision].VelNormal * value;
            double accel = dataService.DataMotion[axisVision].AccelNormal;

            dataService.FeedVelocity = vel;

            bool isLimitCheck = false;


            if (0 != CheckSensor())
            {
                sysService.State = SystemService.States.stop;
                return -1;
            }

            if (0 != CheckPunchUp())
            {
                sysService.State = SystemService.States.stop;
                return -1;
            }

            if (0 != CheckSwitch())
            {
                sysService.State = SystemService.States.stop;
                return -1;
            }


            // Punch Up
            dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
            dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);

            // Cleaner On
            if (true == dataService.DataSystem.IsSelectedCleanRoller)
                SetCleanRoller();
            else
                ResetCleanRoller();

            //Ionizer On
            dioService.SetOutport((int)EnumSmartIC.Outports.ionizer1On);
            dioService.SetOutport((int)EnumSmartIC.Outports.ionizer2On);
            dioService.SetOutport((int)EnumSmartIC.Outports.ionizer3On);
            dioService.SetOutport((int)EnumSmartIC.Outports.ionizer4On);
            dioService.SetOutport((int)EnumSmartIC.Outports.loaderIonizerBlow);
            dioService.SetOutport((int)EnumSmartIC.Outports.unloaderIonizerBlow);


            while (SystemService.States.stop > sysService.State)
            {
                //if (oldStep != step)
                //{
                //    System.Diagnostics.Debug.WriteLine("{0}", step);
                //    oldStep = step;
                //}
                if (0 != CheckSensor())
                    return -1;

                if (true == isLimitCheck)
                    if (0 != CheckLimit())
                        return -1;

                switch (step)
                {
                    case 0:
                        ResetFlag((int)EnumSmartIC.SeqFlags.autoJogStop);
                        step += 10;
                        break;
                    case 10:
                        // Uncoiler 가동
                        if (false == dataService.IsUncoilerReady)
                        {
                            SetSequence((int)EnumSmartIC.Sequences.uncoilerBuffer, 1);
                            step += 10;
                        }
                        break;
                    case 20:
                        // Recoiler 가동
                        if (false == dataService.IsRecoilerReady)
                        {
                            SetSequence((int)EnumSmartIC.Sequences.recoilerBuffer, 1);
                            step += 10;
                        }
                        break;
                    case 30:
                        // Uncoiler Ready Check
                        //if (true == IsFlag((int)EnumSmartIC.SeqFlags.uncoilerReady))
                        if (true == dataService.IsUncoilerReady)
                            step += 10;
                        break;
                    case 40:
                        //if (true == IsFlag((int)EnumSmartIC.SeqFlags.recoilerReady))
                        if (true == dataService.IsRecoilerReady)
                            step += 10;
                        break;
                    case 50:
                        if (false == seqService.IsBufferInitDone)
                            step += 10;
                        else
                            step = 100;
                        break;
                    case 60:
                        SetSequence((int)EnumSmartIC.Sequences.bufferInit, 1);
                        step += 10;
                        break;
                    case 70:
                        if (true == seqService.IsBufferInitDone)
                            step += 10;
                        break;
                    case 80:
                        step = 100;
                        break;

                    case 100:
                        SetSequence((int)EnumSmartIC.Sequences.bufferReference, 1, 10.0);
                        step += 10;
                        break;
                    case 110:
                        if (true == motionService.IsMotionDone(axisPunch))
                        {
                            isLimitCheck = true;
                            step += 10;
                        }
                        break;
                    case 120:
                        step += 10;
                        break;
                    case 130:
                            step += 10;
                        break;
                    case 140:
                            step += 10;
                        break;
                    case 150:
                        step = 200;
                        break;

                    case 200:
                        if ((int)EnumSmartIC.Sequences.autoJogN == seq)
                        {
                            // Jog N
                            motionService.JogN(axisVision, vel, accel);
                            motionService.JogN(axisPunch, vel, accel);
                        }
                        else if ((int)EnumSmartIC.Sequences.autoJogP == seq)
                        {
                            // Jog P
                            motionService.JogP(axisVision, vel, accel);
                            motionService.JogP(axisPunch, vel, accel);
                        }
                        step += 10;
                        break;
                    case 210:
                        //if (true == IsFlag((int)EnumSmartIC.SeqFlags.autoJogStop))
                        //{
                        //    motionService.Stop(axisVision);
                        //    motionService.Stop(axisPunch);
                        //    ResetFlag((int)EnumSmartIC.SeqFlags.autoJogStop);
                        //    step += 10;
                        //}
                        step += 10;
                        break;
                    case 220:
                        step = 20000;
                        break;

                    case 20000:
                        //motionService.Stop(axisVision);
                        //motionService.Stop(axisPunch);

                        //SetSequence((int)EnumSmartIC.Sequences.uncoilerBuffer, 0);
                        //SetSequence((int)EnumSmartIC.Sequences.recoilerBuffer, 0);

                        //// Punch Up
                        //dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
                        //dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);

                        //// Cleaner Off
                        //dioService.SetOutport((int)EnumSmartIC.Outports.cleanUnitOff);
                        //dioService.ResetOutport((int)EnumSmartIC.Outports.cleanUnitOn);

                        ////Ionizer Off
                        //dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer1On);
                        //dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer2On);
                        //dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer3On);
                        //dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer4On);
                        //dioService.ResetOutport((int)EnumSmartIC.Outports.loaderIonizerBlow);
                        //dioService.ResetOutport((int)EnumSmartIC.Outports.unloaderIonizerBlow);

                        //dataService.FeedVelocity = dataService.DataMotion[axisVision].VelMove;

                        return 0;

                    default:
                        break;
                }
                System.Windows.Forms.Application.DoEvents();
            }

            // Punch Up
            dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
            dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);

            // Cleaner Off
            ResetCleanRoller();

            //Ionizer Off
            dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer1On);
            dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer2On);
            dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer3On);
            dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer4On);
            dioService.ResetOutport((int)EnumSmartIC.Outports.loaderIonizerBlow);
            dioService.ResetOutport((int)EnumSmartIC.Outports.unloaderIonizerBlow);

            dataService.FeedVelocity = dataService.DataMotion[axisVision].VelMove;

            return -1;
        }

       
        protected virtual int SetTeachScan(int seq, int type, double value)
        {
            int step = 0;

            ResetFlag((int)EnumSmartIC.SeqFlags.visionForward);
            ResetFlag((int)EnumSmartIC.SeqFlags.visionBackward);
            ResetFlag((int)EnumSmartIC.SeqFlags.punchForward);
            ResetFlag((int)EnumSmartIC.SeqFlags.punchBackward);


            int axisVision = (int)EnumSmartIC.Axis.visionFeed;
            int axisPunch = (int)EnumSmartIC.Axis.punchFeed;

            int axisTop = (int)EnumSmartIC.Axis.visionTop;
            int axisBottom = (int)EnumSmartIC.Axis.visionBottom;

            double velFeed = dataService.DataMotion[axisVision].VelMove;
            double accelFeed = dataService.DataMotion[axisVision].AccelMove;

            double posTop = dataService.DataSystem.TopVisionZ;
            double velTop = dataService.DataMotion[axisTop].VelMove;
            double accelTop = dataService.DataMotion[axisTop].AccelMove;

            double posBottom = dataService.DataSystem.BottomVisionZ;
            double velBottom = dataService.DataMotion[axisBottom].VelMove;
            double accelBottom = dataService.DataMotion[axisBottom].AccelMove;

            int triggerChannel = 0;
            double triggerStart = 0.0;
            double triggerEnd = 0.0;
            double triggerPeriod = 0.01;
            double scanLength = (double)(dataService.DataTeach.PF * dataService.DataTeach.ScanUnits) * 4.75;
            double scanOffset = (double)(dataService.DataTeach.PF) * 4.75 * dataService.DataTeach.PreScan;
            double scanDummyTop = dataService.DataSystem.ScanDummyTop;
            double scanDummyBottom = dataService.DataSystem.ScanDummyBottom;
            double scanDummyMono = dataService.DataSystem.ScanDummyMono;

            double scanDummy = scanDummyTop;

            //System.Diagnostics.Debug.WriteLine(string.Format("scanLength = {0:0.000}", scanLength));
            //System.Diagnostics.Debug.WriteLine(string.Format("scanOffset = {0:0.000}", scanOffset));

            sysService.Mode = SystemService.Modes.recipe;

            double pos = 0.0;
            double add = 0.0;
            double width = 10.0;

            bool isLimitCheck = false;

            if (0 != CheckSensor())
            {
                sysService.State = SystemService.States.stop;
                return -1;
            }

            if (0 != CheckPunchUp())
            {
                sysService.State = SystemService.States.stop;
                return -1;
            }

            if (0 != CheckSwitch())
            {
                sysService.State = SystemService.States.stop;
                return -1;
            }

            //dataService.DataMotion[axisVision].VelRapid = velFeed;

            // Punch Up
            dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
            dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);

            // Cooling On
            dioService.SetOutport((int)EnumSmartIC.Outports.topVisionCooling);
            dioService.SetOutport((int)EnumSmartIC.Outports.bottomVisionCooling);

            // Cleaner On
            if (true == dataService.DataSystem.IsSelectedCleanRoller)
                SetCleanRoller();
            else
                ResetCleanRoller();

            //Ionizer On
            dioService.SetOutport((int)EnumSmartIC.Outports.ionizer1On);
            dioService.SetOutport((int)EnumSmartIC.Outports.ionizer2On);
            dioService.SetOutport((int)EnumSmartIC.Outports.ionizer3On);
            dioService.SetOutport((int)EnumSmartIC.Outports.ionizer4On);
            dioService.SetOutport((int)EnumSmartIC.Outports.loaderIonizerBlow);
            dioService.SetOutport((int)EnumSmartIC.Outports.unloaderIonizerBlow);

            value = 0;


            //0.0011875
            while (SystemService.States.stop > sysService.State)
            {
                //if (oldStep != step)
                //{
                //    System.Diagnostics.Debug.WriteLine("{0}", step);
                //    oldStep = step;
                //}
                if (0 != CheckSensor())
                    return -1;

                if (true == isLimitCheck)
                    if (0 != CheckLimit())
                        return -1;

                switch (step)
                {
                    case 0:
                        step += 10;
                        break;
                    case 10:
                        // Uncoiler 가동
                        if (false == dataService.IsUncoilerReady)
                        {
                            SetSequence((int)EnumSmartIC.Sequences.uncoilerBuffer, 1);
                            step += 10;
                        }
                        break;
                    case 20:
                        // Recoiler 가동
                        if (false == dataService.IsRecoilerReady)
                        {
                            SetSequence((int)EnumSmartIC.Sequences.recoilerBuffer, 1);
                            step += 10;
                        }
                        break;
                    case 30:
                        // Uncoiler Ready Check
                        //if (true == IsFlag((int)EnumSmartIC.SeqFlags.uncoilerReady))
                        if (true == dataService.IsUncoilerReady)
                            step += 10;
                        break;
                    case 40:
                        //if (true == IsFlag((int)EnumSmartIC.SeqFlags.recoilerReady))
                        if (true == dataService.IsRecoilerReady)
                            step += 10;
                        break;
                    case 50:
                        if (false == seqService.IsBufferInitDone)
                            step += 10;
                        else
                            step = 100;
                        break;
                    case 60:
                        SetSequence((int)EnumSmartIC.Sequences.bufferInit, 1);
                        step += 10;
                        break;
                    case 70:
                        if (true == seqService.IsBufferInitDone)
                            step += 10;
                        break;
                    case 80:
                        step = 100;
                        break;

                    case 100:
                        SetSequence((int)EnumSmartIC.Sequences.bufferReference, 1, 10.0);
                        step += 10;
                        break;
                    case 110:
                        if (true == motionService.IsMotionDone(axisPunch))
                        {
                            isLimitCheck = true;
                            step += 10;
                        }
                        break;
                    case 120:
                        motionService.AMove(axisTop, posTop, velTop, accelTop);
                        motionService.AMove(axisBottom, posBottom, velBottom, accelBottom);
                        step += 10;
                        break;
                    case 130:
                        if (true == motionService.IsMotionDone(axisTop))
                            step += 10;
                        break;
                    case 140:
                        if (true == motionService.IsMotionDone(axisBottom))
                            step += 10;
                        break;
                    case 150:

                        step = 200;
                        break;

                    case 200:
                        motionService.SetPosition(axisVision, -value);
                        motionService.SetPosition(axisPunch, -value);

                        cntService.SetPosition(0, -value);
                        cntService.SetPosition(1, -value);
                        cntService.SetPosition(2, -value);
                        cntService.SetPosition(3, -value);

                        System.Diagnostics.Debug.WriteLine(string.Format("Current Position = {0:0.000}", -value));

                        if ((int)EnumSmartIC.Sequences.teachTop == seq)
                        {
                            scanDummy = scanDummyTop;

                            triggerChannel = 0;
                            //triggerStart = -scanOffset - scanDummy;
                            //triggerEnd = scanLength + scanOffset + scanDummy * 2.0;

                            triggerStart = -scanDummy;
                            triggerEnd = scanLength + scanDummy * 2.0;

                            //AxtCNT.CNTset_moveunit_perpulse(0, 0.0011875);

                            //triggerStart = 0;
                            //triggerEnd = 20.0;
                            triggerPeriod = dataService.DataSystem.TriggerColor_Period;
                            width = dataService.DataSystem.TriggerColor_Width;

                            System.Diagnostics.Debug.WriteLine(string.Format("TriggerStart = {0:0.000}", triggerStart));
                            System.Diagnostics.Debug.WriteLine(string.Format("triggerEnd = {0:0.000}", triggerEnd));

                        }
                        else if ((int)EnumSmartIC.Sequences.teachBottom == seq)
                        {
                            scanDummy = scanDummyBottom;

                            triggerChannel = 2;
                            //triggerStart = dataService.DataSystem.BottomDistance - scanOffset - scanDummy;
                            //triggerEnd = dataService.DataSystem.BottomDistance + scanLength + scanOffset + scanDummy * 2.0;

                            triggerStart = dataService.DataSystem.BottomDistance - scanDummy;
                            triggerEnd = dataService.DataSystem.BottomDistance + scanLength + scanDummy * 2.0;

                            //triggerPeriod = 0.01;
                            triggerPeriod = dataService.DataSystem.TriggerColor_Period;

                            width = dataService.DataSystem.TriggerColor_Width;

                            add = dataService.DataSystem.BottomDistance;
                        }
                        else if ((int)EnumSmartIC.Sequences.teachMono == seq)
                        {
                            scanDummy = scanDummyMono;

                            triggerChannel = 1;
                            //triggerStart = dataService.DataSystem.MonoDistance - scanOffset - scanDummy;
                            //triggerEnd = dataService.DataSystem.MonoDistance + scanLength + scanOffset + scanDummy * 2.0;

                            triggerStart = dataService.DataSystem.MonoDistance - scanDummy;
                            triggerEnd = dataService.DataSystem.MonoDistance + scanLength + scanDummy * 2.0;

                            //triggerPeriod = 0.005;
                            triggerPeriod = dataService.DataSystem.TriggerMono_Period;

                            width = dataService.DataSystem.TriggerMono_Width;

                            add = dataService.DataSystem.MonoDistance;


                            //System.Diagnostics.Debug.WriteLine(string.Format("Mono start = {0}, end = {1}", triggerStart, triggerEnd ));
                            
                        }

                        step = 1000;
                        break;

                    case 1000:
                        step += 10;
                        break;
                    case 1010:
                        // Offset 과 Init Unit 갯수만큼 이동한다. 
                        //pos = scanOffset + value + dataService.DataSystem.ScanTolerance + scanDummy;
                        pos = dataService.DataSystem.ScanTolerance + scanDummy;

                        motionService.RMove(axisVision, -pos, velFeed, accelFeed);
                        motionService.RMove(axisPunch, -pos, velFeed, accelFeed);

                        System.Diagnostics.Debug.WriteLine(string.Format("OffSet Move = {0:0.000}", -pos));


                        step += 10;
                        break;
                     case 1020:
                       if (true == motionService.IsMotionDone(axisVision))
                        {
                            if (true == motionService.IsMotionDone(axisPunch))
                            {
                                SetTimeout(100);
                                step += 10;
                            }
                        }
                        break;
                    case 1030:
                        if (true == IsTimeout())
                        {
                            cntService.SetTriggerParams(triggerChannel, triggerStart, triggerEnd, triggerPeriod, width);
                            //cntService.SetTriggerParams(triggerChannel, 0, triggerEnd, triggerPeriod);
                            step += 10;    
                        }
                        break;
                    case 1040:
                        cntService.SetTriggerEnable(triggerChannel);
                        step += 10;
                        break;
                    case 1050:
                        if ((int)EnumSmartIC.Sequences.teachTop == seq)
                        {
                            topService.SetScanStart();
                            top2Service.SetScanStart();
                        }
                        else if ((int)EnumSmartIC.Sequences.teachBottom == seq)
                        {
                            bottomService.SetScanStart();
                            bottom2Service.SetScanStart();
                        }
                        else if ((int)EnumSmartIC.Sequences.teachMono == seq)
                        {
                            monoService.SetScanStart();
                            mono2Service.SetScanStart();
                        }
                        //SetTimeout(500);
                        step += 10;
                        break;
                    case 1060:
                        if ((int)EnumSmartIC.Sequences.teachTop == seq)
                        {
                            if (true == topService.IsReply((int)EnumSmartIC.VisionReplys.scan))
                            {
                                //if (true == top2Service.IsReply((int)EnumSmartIC.VisionReplys.scan))
                                    step += 10;
                            }
                        }
                        else if ((int)EnumSmartIC.Sequences.teachBottom == seq)
                        {
                            if (true == bottomService.IsReply((int)EnumSmartIC.VisionReplys.scan))
                            {
                                //if (true == bottom2Service.IsReply((int)EnumSmartIC.VisionReplys.scan))
                                    step += 10;
                            }
                        }
                        else if ((int)EnumSmartIC.Sequences.teachMono == seq)
                        {
                            if (true == monoService.IsReply((int)EnumSmartIC.VisionReplys.scan))
                            {
                                //if (true == mono2Service.IsReply((int)EnumSmartIC.VisionReplys.scan))
                                    step += 10;
                            }
                        }
                        //if (true == IsTimeout())

                        //    step += 10;
                        break;
                    case 1070:
                        pos = scanOffset * 2.0 + value + dataService.DataSystem.ScanTolerance * 2.0 + scanLength + scanDummy * 2.0 + 5.0 + add;

                        //System.Diagnostics.Debug.WriteLine(string.Format("RMove = {0:0.000}", pos));

                        // Scan Start
                        motionService.RMove(axisVision, pos, velFeed, accelFeed);
                        motionService.RMove(axisPunch, pos, velFeed, accelFeed);

                        //System.Diagnostics.Debug.WriteLine(string.Format("Scan Move = {0:0.000}", pos));

                        step += 10;
                        break;
                    //case 1070:
                    //    pos = scanOffset * 2.0 + value + dataService.DataSystem.ScanTolerance * 2.0 + scanLength + scanDummy * 2.0 + 5.0 + add;

                    //    //System.Diagnostics.Debug.WriteLine(string.Format("RMove = {0:0.000}", pos));

                    //    // Scan Start
                    //    motionService.RMove(axisVision, 50.0, velFeed, accelFeed);
                    //    motionService.RMove(axisPunch, 50.0, velFeed, accelFeed);

                    //    //System.Diagnostics.Debug.WriteLine(string.Format("Scan Move = {0:0.000}", pos));

                    //    step += 1;
                    //    break;
                    case 1071:
                        if (true == motionService.IsMotionDone(axisVision))
                        {
                            if (true == motionService.IsMotionDone(axisPunch))
                            {
                                DoEvents(3000);
                                step += 1;
                            }
                        }
                        break;

                    case 1072:
                        // Scan Start
                        motionService.RMove(axisVision, -100.0, velFeed, accelFeed);
                        motionService.RMove(axisPunch, -100.0, velFeed, accelFeed);

                        System.Diagnostics.Debug.WriteLine(string.Format("Scan Move = {0:0.000}", pos));

                        step += 1;
                        break;
                    case 1073:
                        if (true == motionService.IsMotionDone(axisVision))
                        {
                            if (true == motionService.IsMotionDone(axisPunch))
                            {
                                DoEvents(100);
                                step += 1;
                            }
                        }
                        break;
                    case 1074:
                        // Scan Start
                        motionService.RMove(axisVision, pos, velFeed, accelFeed);
                        motionService.RMove(axisPunch, pos, velFeed, accelFeed);

                        //System.Diagnostics.Debug.WriteLine(string.Format("Scan Move = {0:0.000}", pos));

                        step = 1080;
                        break;


                    case 1080:
                        if (true == motionService.IsMotionDone(axisVision))
                        {
                            if (true == motionService.IsMotionDone(axisPunch))
                            {
                                cntService.ResetTriggerEnable(0);
                                cntService.ResetTriggerEnable(1);
                                cntService.ResetTriggerEnable(2);
                                cntService.ResetTriggerEnable(3);

                                SetTimeout(100);
                                step += 10;
                            }
                        }
                        break;
                    case 1090:
                        // Move To 0.0
                        if (true == IsTimeout())
                        {
                            motionService.AMove(axisVision, 0.0, velFeed, accelFeed);
                            motionService.AMove(axisPunch, 0.0, velFeed, accelFeed);
                            step += 10;
                        }
                        break;
                    case 1100:
                        if (true == motionService.IsMotionDone(axisVision))
                        {
                            if (true == motionService.IsMotionDone(axisPunch))
                            {
                                SetTimeout(100);
                                step += 10;
                            }
                        }
                        break;
                    case 1110:
                        if (true == IsTimeout())
                        {
                            step = 20000;
                        }
                        break;


                   


                    case 20000:
                        motionService.Stop(axisVision);
                        motionService.Stop(axisPunch);

                        SetSequence((int)EnumSmartIC.Sequences.uncoilerBuffer, 0);
                        SetSequence((int)EnumSmartIC.Sequences.recoilerBuffer, 0);

                        cntService.ResetTriggerEnable(0);
                        cntService.ResetTriggerEnable(1);
                        cntService.ResetTriggerEnable(2);
                        cntService.ResetTriggerEnable(3);

                        // Punch Up
                        dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);

                        // Cleaner Off
                        ResetCleanRoller();

                        //Ionizer Off
                        dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer1On);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer2On);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer3On);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer4On);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.loaderIonizerBlow);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.unloaderIonizerBlow);
                        return 0;

                    default:
                        break;
                }
                System.Windows.Forms.Application.DoEvents();
            }

            cntService.ResetTriggerEnable(0);
            cntService.ResetTriggerEnable(1);
            cntService.ResetTriggerEnable(2);
            cntService.ResetTriggerEnable(3);

            // Punch Up
            dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
            dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);

            // Cleaner Off
            ResetCleanRoller();

            //Ionizer Off
            dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer1On);
            dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer2On);
            dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer3On);
            dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer4On);
            dioService.ResetOutport((int)EnumSmartIC.Outports.loaderIonizerBlow);
            dioService.ResetOutport((int)EnumSmartIC.Outports.unloaderIonizerBlow);

            SetSequence((int)EnumSmartIC.Sequences.uncoilerBuffer, 0);
            SetSequence((int)EnumSmartIC.Sequences.recoilerBuffer, 0);

            return -1;
        }

        protected virtual int SetTeachPunch()
        {
            int step = 0;

            ResetFlag((int)EnumSmartIC.SeqFlags.visionForward);
            ResetFlag((int)EnumSmartIC.SeqFlags.visionBackward);
            ResetFlag((int)EnumSmartIC.SeqFlags.punchForward);
            ResetFlag((int)EnumSmartIC.SeqFlags.punchBackward);
            
            int axisVision = (int)EnumSmartIC.Axis.visionFeed;
            int axisPunch = (int)EnumSmartIC.Axis.punchFeed;

            int axisTop = (int)EnumSmartIC.Axis.visionTop;
            int axisBottom = (int)EnumSmartIC.Axis.visionBottom;

            int axisAlignX = (int)EnumSmartIC.Axis.punchX;
            int axisAlignY = (int)EnumSmartIC.Axis.punchY;

            double velFeed = dataService.DataMotion[axisVision].VelMove;
            double accelFeed = dataService.DataMotion[axisVision].AccelMove;

            double posTop = dataService.DataSystem.TopVisionZ;
            double velTop = dataService.DataMotion[axisTop].VelMove;
            double accelTop = dataService.DataMotion[axisTop].AccelMove;

            double posBottom = dataService.DataSystem.BottomVisionZ;
            double velBottom = dataService.DataMotion[axisBottom].VelMove;
            double accelBottom = dataService.DataMotion[axisBottom].AccelMove;

            double velAlignX = dataService.DataMotion[axisAlignX].VelMove;
            double accelAlignX = dataService.DataMotion[axisAlignX].AccelMove;
            double velAlignY = dataService.DataMotion[axisAlignY].VelMove;
            double accelAlignY = dataService.DataMotion[axisAlignY].AccelMove;
                        
            double punchDistance = dataService.DataSystem.PunchDistance + dataService.DataTeach.OffsetInitX;
            double posAlignX = dataService.DataSystem.AlignVisionX;
            double posAlignY = dataService.DataSystem.AlignVisionY;

            //Log_Trace.WriteLine("(Teach) punchDistance = PunchDistance + OffsetInitX : {0:0.000} = {1:0.000} + {2:0.000}",
            //    punchDistance, dataService.DataSystem.PunchDistance, dataService.DataRecipe.OffsetInitX);

            double pos = 0.0;

            bool isLimitCheck = false;

            if (0 != CheckSensor())
            {
                sysService.State = SystemService.States.stop;
                return -1;
            }

            if (0 != CheckPunchUp())
            {
                sysService.State = SystemService.States.stop;
                return -1;
            }

            if (0 != CheckSwitch())
            {
                sysService.State = SystemService.States.stop;
                return -1;
            }

            // 검사데이터를 모두 삭제한다 : 위치 값이 초기화 됨. 
            dataService.DataResult.Clear();
            dataService.DataDefect.Clear();



            //0.0011875
            while (SystemService.States.stop > sysService.State)
            {
                //if (oldStep != step)
                //{
                //    System.Diagnostics.Debug.WriteLine("{0}", step);
                //    oldStep = step;
                //}
                if (0 != CheckSensor())
                    return -1;

                if (true == isLimitCheck)
                    if (0 != CheckLimit())
                        return -1;

                switch (step)
                {
                    case 0:
                        step += 10;
                        break;
                    case 10:
                        // Uncoiler 가동
                        if (false == dataService.IsUncoilerReady)
                        {
                            SetSequence((int)EnumSmartIC.Sequences.uncoilerBuffer, 1);
                            step += 10;
                        }
                        break;
                    case 20:
                        // Recoiler 가동
                        if (false == dataService.IsRecoilerReady)
                        {
                            SetSequence((int)EnumSmartIC.Sequences.recoilerBuffer, 1);
                            step += 10;
                        }
                        break;
                    case 30:
                        // Uncoiler Ready Check
                        //if (true == IsFlag((int)EnumSmartIC.SeqFlags.uncoilerReady))
                        if (true == dataService.IsUncoilerReady)
                            step += 10;
                        break;
                    case 40:
                        //if (true == IsFlag((int)EnumSmartIC.SeqFlags.recoilerReady))
                        if (true == dataService.IsRecoilerReady)
                            step += 10;
                        break;
                    case 50:
                        if (false == seqService.IsBufferInitDone)
                            step += 10;
                        else
                            step = 100;
                        break;
                    case 60:
                        SetSequence((int)EnumSmartIC.Sequences.bufferInit, 1);
                        step += 10;
                        break;
                    case 70:
                        if (true == seqService.IsBufferInitDone)
                            step += 10;
                        break;
                    case 80:
                        step = 100;
                        break;

                    case 100:
                        SetSequence((int)EnumSmartIC.Sequences.bufferReference, 1, 10.0);
                        step += 10;
                        break;
                    case 110:
                        if (true == motionService.IsMotionDone(axisPunch))
                        {
                            isLimitCheck = true;
                            step += 10;
                        }
                        break;
                    case 120:
                        motionService.AMove(axisTop, posTop, velTop, accelTop);
                        motionService.AMove(axisBottom, posBottom, velBottom, accelBottom);
                        step += 10;
                        break;
                    case 130:
                        if (true == motionService.IsMotionDone(axisTop))
                            step += 10;
                        break;
                    case 140:
                        if (true == motionService.IsMotionDone(axisBottom))
                            step += 10;
                        break;
                    case 150:
                        step = 200;
                        break;

                    case 200:
                        // Vision Move to 0.0;
                        //Log_Trace.WriteLine("Current Teach Pos (Vision) : {0:0.000}", motionService.GetCurrentPosition(axisVision));
                        //Log_Trace.WriteLine("Current Teach Pos (Punch) : {0:0.000}", motionService.GetCurrentPosition(axisPunch));
                        //Log_Trace.WriteLine("Current Teach Pos (Buffer) : {0:0.000}", motionService.GetCurrentPosition((int)EnumSmartIC.Axis.buffer));

                        pos = motionService.GetCurrentPosition(axisVision);
                        motionService.SetPosition(axisPunch, pos);

                        //Log_Trace.WriteLine("Current Teach Pos (Vision) : {0:0.000}", motionService.GetCurrentPosition(axisVision));
                        //Log_Trace.WriteLine("Current Teach Pos (Punch) : {0:0.000}", motionService.GetCurrentPosition(axisPunch));
                        //Log_Trace.WriteLine("Current Teach Pos (Buffer) : {0:0.000}", motionService.GetCurrentPosition((int)EnumSmartIC.Axis.buffer));

                        step += 10;
                        break;
                    case 210:
                        motionService.AMove(axisVision, 0.0, velFeed, accelFeed);
                        motionService.AMove(axisPunch, 0.0, velFeed, accelFeed);
                        step += 10;
                        break;
                    case 220:
                        if (true == motionService.IsMotionDone(axisVision))
                        {
                            if (true == motionService.IsMotionDone(axisPunch))
                            {
                                SetTimeout(200);
                                step += 10;
                            }
                        }
                        break;
                    case 230:
                        if (true == IsTimeout())
                        {
                            step += 10;
                        }
                        break;
                    case 240:
                        //Log_Trace.WriteLine("Current Teach Pos (Vision) : {0:0.000}", motionService.GetCurrentPosition(axisVision));
                        //Log_Trace.WriteLine("Current Teach Pos (Punch) : {0:0.000}", motionService.GetCurrentPosition(axisPunch));
                        //Log_Trace.WriteLine("Current Teach Pos (Buffer) : {0:0.000}", motionService.GetCurrentPosition((int)EnumSmartIC.Axis.buffer));

                        step = 400;
                        break;

                    case 400:
                        // Buffer Resetting
                        if (0 == SetSequence((int)EnumSmartIC.Sequences.bufferReference, 1, 0.01))
                        {
                            SetTimeout(100);
                            step += 10;
                        }
                        break;
                    case 410:
                        // Set Punch Position
                        if (true == IsTimeout())
                        {
                            motionService.SetPosition(axisPunch, 0.0);
                            step += 10;
                        }
                        break;
                    case 420:
                        step = 1000;
                        break;

                    case 1000:
                        //Log_Trace.WriteLine("Current Teach Pos (Vision) : {0:0.000}", motionService.GetCurrentPosition(axisVision));
                        //Log_Trace.WriteLine("Current Teach Pos (Punch) : {0:0.000}", motionService.GetCurrentPosition(axisPunch));
                        //Log_Trace.WriteLine("Current Teach Pos (Buffer) : {0:0.000}", motionService.GetCurrentPosition((int)EnumSmartIC.Axis.buffer));

                        step += 10;
                        break;
                    case 1010:
                        motionService.AMove(axisVision, punchDistance, velFeed, accelFeed);
                        motionService.AMove(axisPunch, punchDistance, velFeed, accelFeed);

                        motionService.AMove(axisAlignX, posAlignX, velAlignX, accelAlignX);
                        motionService.AMove(axisAlignY, posAlignY, velAlignY, accelAlignY);

                        step += 10;
                        break;
                    case 1020:
                        if (true == motionService.IsMotionDone(axisAlignX))
                        {
                            if (true == motionService.IsMotionDone(axisAlignY))
                            {
                                step += 10;
                            }
                        }
                        break;

                    case 1030:
                        if (true == motionService.IsMotionDone(axisVision))
                        {
                            if (true == motionService.IsMotionDone(axisPunch))
                            {
                                step += 10;
                            }
                        }
                        break;

                    case 1040:
                        //Log_Trace.WriteLine("Current Teach Pos (Vision) : {0:0.000}", motionService.GetCurrentPosition(axisVision));
                        //Log_Trace.WriteLine("Current Teach Pos (Punch) : {0:0.000}", motionService.GetCurrentPosition(axisPunch));
                        //Log_Trace.WriteLine("Current Teach Pos (Buffer) : {0:0.000}", motionService.GetCurrentPosition((int)EnumSmartIC.Axis.buffer));

                        step = 20000;
                        break;

                    case 20000:
                        motionService.Stop(axisVision);
                        motionService.Stop(axisPunch);
                        motionService.Stop(axisAlignX);
                        motionService.Stop(axisAlignY);

                        SetSequence((int)EnumSmartIC.Sequences.uncoilerBuffer, 0);
                        SetSequence((int)EnumSmartIC.Sequences.recoilerBuffer, 0);
                        return 0;

                    default:
                        break;
                }
                System.Windows.Forms.Application.DoEvents();
            }

            motionService.Stop(axisVision);
            motionService.Stop(axisPunch);
            motionService.Stop(axisAlignX);
            motionService.Stop(axisAlignY);

            SetSequence((int)EnumSmartIC.Sequences.uncoilerBuffer, 0);
            SetSequence((int)EnumSmartIC.Sequences.recoilerBuffer, 0);

            return -1;
        }

        protected virtual int SetTeachToZero()
        {
            int step = 0;

            ResetFlag((int)EnumSmartIC.SeqFlags.visionForward);
            ResetFlag((int)EnumSmartIC.SeqFlags.visionBackward);
            ResetFlag((int)EnumSmartIC.SeqFlags.punchForward);
            ResetFlag((int)EnumSmartIC.SeqFlags.punchBackward);

            int axisVision = (int)EnumSmartIC.Axis.visionFeed;
            int axisPunch = (int)EnumSmartIC.Axis.punchFeed;

            int axisTop = (int)EnumSmartIC.Axis.visionTop;
            int axisBottom = (int)EnumSmartIC.Axis.visionBottom;

            int axisAlignX = (int)EnumSmartIC.Axis.punchX;
            int axisAlignY = (int)EnumSmartIC.Axis.punchY;

            double velFeed = dataService.DataMotion[axisVision].VelMove;
            double accelFeed = dataService.DataMotion[axisVision].AccelMove;

            double posTop = dataService.DataSystem.TopVisionZ;
            double velTop = dataService.DataMotion[axisTop].VelMove;
            double accelTop = dataService.DataMotion[axisTop].AccelMove;

            double posBottom = dataService.DataSystem.BottomVisionZ;
            double velBottom = dataService.DataMotion[axisBottom].VelMove;
            double accelBottom = dataService.DataMotion[axisBottom].AccelMove;

            double velAlignX = dataService.DataMotion[axisAlignX].VelMove;
            double accelAlignX = dataService.DataMotion[axisAlignX].AccelMove;
            double velAlignY = dataService.DataMotion[axisAlignY].VelMove;
            double accelAlignY = dataService.DataMotion[axisAlignY].AccelMove;

            
            //double punchDistance = dataService.DataSystem.PunchDistance;
            double posAlignX = dataService.DataSystem.AlignVisionX;
            double posAlignY = dataService.DataSystem.AlignVisionY;
                       

            bool isLimitCheck = false;

            if (0 != CheckSensor())
            {
                sysService.State = SystemService.States.stop;
                return -1;
            }

            if (0 != CheckPunchUp())
            {
                sysService.State = SystemService.States.stop;
                return -1;
            }

            if (0 != CheckSwitch())
            {
                sysService.State = SystemService.States.stop;
                return -1;
            }

            //0.0011875
            while (SystemService.States.stop > sysService.State)
            {
                //if (oldStep != step)
                //{
                //    System.Diagnostics.Debug.WriteLine("{0}", step);
                //    oldStep = step;
                //}
                if (0 != CheckSensor())
                    return -1;

                if (true == isLimitCheck)
                    if (0 != CheckLimit())
                        return -1;

                switch (step)
                {
                    case 0:
                        step += 10;
                        break;
                    case 10:
                        // Uncoiler 가동
                        if (false == dataService.IsUncoilerReady)
                        {
                            SetSequence((int)EnumSmartIC.Sequences.uncoilerBuffer, 1);
                            step += 10;
                        }
                        break;
                    case 20:
                        // Recoiler 가동
                        if (false == dataService.IsRecoilerReady)
                        {
                            SetSequence((int)EnumSmartIC.Sequences.recoilerBuffer, 1);
                            step += 10;
                        }
                        break;
                    case 30:
                        // Uncoiler Ready Check
                        //if (true == IsFlag((int)EnumSmartIC.SeqFlags.uncoilerReady))
                        if (true == dataService.IsUncoilerReady)
                            step += 10;
                        break;
                    case 40:
                        //if (true == IsFlag((int)EnumSmartIC.SeqFlags.recoilerReady))
                        if (true == dataService.IsRecoilerReady)
                            step += 10;
                        break;
                    case 50:
                        if (false == seqService.IsBufferInitDone)
                            step += 10;
                        else
                            step = 100;
                        break;
                    case 60:
                        SetSequence((int)EnumSmartIC.Sequences.bufferInit, 1);
                        step += 10;
                        break;
                    case 70:
                        if (true == seqService.IsBufferInitDone)
                            step += 10;
                        break;
                    case 80:
                        step = 100;
                        break;

                    case 100:
                        SetSequence((int)EnumSmartIC.Sequences.bufferReference, 1, 10.0);
                        step += 10;
                        break;
                    case 110:
                        if (true == motionService.IsMotionDone(axisPunch))
                        {
                            isLimitCheck = true;
                            step += 10;
                        }
                        break;
                    case 120:
                        motionService.AMove(axisTop, posTop, velTop, accelTop);
                        motionService.AMove(axisBottom, posBottom, velBottom, accelBottom);
                        step += 10;
                        break;
                    case 130:
                        if (true == motionService.IsMotionDone(axisTop))
                            step += 10;
                        break;
                    case 140:
                        if (true == motionService.IsMotionDone(axisBottom))
                            step += 10;
                        break;
                    case 150:
                        step = 200;
                        break;

                    case 200:
                        // Vision Move to 0.0;
                        Log_Trace.WriteLine("Current TeachZero Pos (Vision) : {0:0.000}", motionService.GetCurrentPosition(axisVision));
                        Log_Trace.WriteLine("Current TeachZero Pos (Punch) : {0:0.000}", motionService.GetCurrentPosition(axisPunch));
                        Log_Trace.WriteLine("Current TeachZero Pos (Buffer) : {0:0.000}", motionService.GetCurrentPosition((int)EnumSmartIC.Axis.buffer));

                        step += 10;
                        break;
                    case 210:
                        motionService.AMove(axisVision, 0.0, velFeed, accelFeed);
                        motionService.AMove(axisPunch, 0.0, velFeed, accelFeed);
                        step += 10;
                        break;
                    case 220:
                        if (true == motionService.IsMotionDone(axisVision))
                        {
                            if (true == motionService.IsMotionDone(axisPunch))
                            {
                                SetTimeout(200);
                                step += 10;
                            }
                        }
                        break;
                    case 230:
                        if (true == IsTimeout())
                        {
                            step += 10;
                        }
                        break;
                    case 240:

                        Log_Trace.WriteLine("Current TeachZero Pos (Vision) : {0:0.000}", motionService.GetCurrentPosition(axisVision));
                        Log_Trace.WriteLine("Current TeachZero Pos (Punch) : {0:0.000}", motionService.GetCurrentPosition(axisPunch));
                        Log_Trace.WriteLine("Current TeachZero Pos (Buffer) : {0:0.000}", motionService.GetCurrentPosition((int)EnumSmartIC.Axis.buffer));

                        step = 20000;
                        break;

                    case 20000:
                        motionService.Stop(axisVision);
                        motionService.Stop(axisPunch);
                        motionService.Stop(axisAlignX);
                        motionService.Stop(axisAlignY);

                        SetSequence((int)EnumSmartIC.Sequences.uncoilerBuffer, 0);
                        SetSequence((int)EnumSmartIC.Sequences.recoilerBuffer, 0);
                        return 0;

                    default:
                        break;
                }
                System.Windows.Forms.Application.DoEvents();
            }

            motionService.Stop(axisVision);
            motionService.Stop(axisPunch);
            motionService.Stop(axisAlignX);
            motionService.Stop(axisAlignY);

            SetSequence((int)EnumSmartIC.Sequences.uncoilerBuffer, 0);
            SetSequence((int)EnumSmartIC.Sequences.recoilerBuffer, 0);

            return -1;
        }

        protected virtual int SetBackFeeding()
        {
            if (false == dataService.DataSystem.IsSelectedBackFeeding)
                return 0;

            if (0.0 >= dataService.DataSystem.PosBackFeeding)
                return 0;

            int step = 0;

            int axisVision = (int)EnumSmartIC.Axis.visionFeed;
            int axisBuffer = (int)EnumSmartIC.Axis.buffer;

            double vel = dataService.DataMotion[axisVision].VelMove;
            double accel = dataService.DataMotion[axisVision].AccelMove;
            double pos = dataService.DataSystem.PosBackFeeding;

            DateTime start = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 500);
            DateTime timeout = start.Add(duration);

            double limitP = dataService.DataSystem.BufferLimitP;
            double posBuffer = motionService.GetCurrentPosition(axisBuffer);

            if (limitP < posBuffer)
                return 0;

            if (50.0 < pos)
                pos = 50.0;

            if (0 != CheckSensor())
            {
                sysService.State = SystemService.States.stop;
                return -1;
            }

            if (0 != CheckPunchUp())
            {
                sysService.State = SystemService.States.stop;
                return -1;
            }

            if (0 != CheckSwitch())
            {
                sysService.State = SystemService.States.stop;
                return -1;
            }

            while (SystemService.States.stop > sysService.State)
            {
                if (0 != CheckSensor())
                    return -1;

                if (0 != CheckLimit())
                    return -1;

                switch (step)
                {
                    case 0:
                        motionService.RMove(axisVision, -pos, vel, accel);
                        step += 10;
                        break;
                    case 10:
                        if (true == motionService.IsMotionDone(axisVision))
                        {
                            start = DateTime.Now;
                            duration = new TimeSpan(0, 0, 0, 0, 100);
                            timeout = start.Add(duration);

                            step += 10;
                        }
                        break;
                    case 20:
                        if( timeout < DateTime.Now )
                            step = 20000;
                        break;

                    case 20000:
                        return 0;

                    default:
                        break;
                }
                System.Windows.Forms.Application.DoEvents();
            }

            motionService.Stop(axisVision);

            return -1;
        }

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

        protected void SetFlag(int index)
        {
            if (0 > index)
                return;
            if (flags.Length < index)
                return;

            flags[index] = true;
        }

        protected void ResetFlag(int index)
        {
            if (0 > index)
                return;
            if (flags.Length < index)
                return;

            flags[index] = false;
        }
        
        protected virtual int CheckSensor(bool isStop = true)
        {
            if (null == dioService)
                return -1;
            if (null == sysService)
                return -1;
            if (null == msgService)
                return -1;

            int ret = 0;

            

            // Vision Stopper
            if ((true == dioService.IsInportOn((int)EnumSmartIC.Inports.visionStopperDown)) || (false == dioService.IsInportOn((int)EnumSmartIC.Inports.visionStopperUp)))
            {
                if( true == isStop )
                    sysService.State = SystemService.States.stop;
                
                msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.stopperDown_vision);
                dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);

                ret = -1;
            }

            // Buffer Stopper
            if ((true == dioService.IsInportOn((int)EnumSmartIC.Inports.bufferStopperDown)) || (false == dioService.IsInportOn((int)EnumSmartIC.Inports.bufferStopperUp)))
            {
                if (true == isStop)
                    sysService.State = SystemService.States.stop;
                msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.stopperDown_buffer);
                dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);

                ret = -1;
            }

            // Punch Stopper
            if ((true == dioService.IsInportOn((int)EnumSmartIC.Inports.punchStopperDown)) || (false == dioService.IsInportOn((int)EnumSmartIC.Inports.punchStopperUp)))
            {
                if (true == isStop)
                    sysService.State = SystemService.States.stop;
                msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.stopperDown_punch);
                dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);

                ret = -1;
            }

            // 2019.07.12 khs - 소재이탈 방지센서 추가
            if (true == dataService.DataSystem.UseCheckGuide)
            {
                // Vision Guide
                if (false == dioService.IsInportOn((int)EnumSmartIC.Inports.visionDeviation))
                {
                    if (true == isStop)
                        sysService.State = SystemService.States.stop;
                    msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.deviation_vision);
                    dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);

                    ret = -1;
                }


                // Punch Guide
                if (false == dioService.IsInportOn((int)EnumSmartIC.Inports.punchDeviation))
                {
                    if (true == isStop)
                        sysService.State = SystemService.States.stop;
                    msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.deviation_punch);
                    dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);

                    ret = -1;
                }
            }
            
            return ret;
        }

        protected virtual int CheckLimit(bool isStop = true)
        {
            int ret = 0;

            // Uncoiler Limit
            if (1 == motionService.GetStateLimitN((int)EnumSmartIC.Axis.uncoilerReel))
            {
                if (true == isStop)
                    sysService.State = SystemService.States.stop;
                dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.limitN_uncoiler);

                ret = -1;
            }
            if (1 == motionService.GetStateLimitP((int)EnumSmartIC.Axis.uncoilerReel))
            {
                if (true == isStop)
                    sysService.State = SystemService.States.stop;
                dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.limitP_uncoiler);

                ret = -1;
            }

            // Buffer Limit
            if (1 == motionService.GetStateLimitN((int)EnumSmartIC.Axis.buffer))
            {
                if (true == isStop)
                    sysService.State = SystemService.States.stop;
                dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.limitN_buffer);

                ret = -1;
            }
            if (1 == motionService.GetStateLimitP((int)EnumSmartIC.Axis.buffer))
            {
                if (true == isStop)
                    sysService.State = SystemService.States.stop;
                dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.limitP_buffer);

                ret = -1;
            }

            // Recoiler Limit
            if (1 == motionService.GetStateLimitN((int)EnumSmartIC.Axis.recoilerReel))
            {
                if (true == isStop)
                    sysService.State = SystemService.States.stop;
                dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.limitN_recoiler);

                ret = -1;
            }
            if (1 == motionService.GetStateLimitP((int)EnumSmartIC.Axis.recoilerReel))
            {
                if (true == isStop)
                    sysService.State = SystemService.States.stop;
                dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.limitP_recoiler);

                ret = -1;
            }

            for (int i = 0; i < 8; ++i)
            {
                if (0 == motionService.GetStateServoOn(i))
                {
                    if (true == isStop)
                        sysService.State = SystemService.States.stop;

                    dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                    msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.servoAlarm_Axis0 + i*10);

                    return -1;
                }
            }

            return ret;
        }

        protected virtual int CheckPunchUp(bool isStop = true)
        {
            int ret = 0;

            // Punch Up Check
            if (false == dioService.IsInportOn((int)EnumSmartIC.Inports.punchUp))
            {
                if (++countPunchUp > 3)
                {
                    if (true == isStop)
                        sysService.State = SystemService.States.stop;
                    dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                    msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.punch_down);

                    ret = -1;
                }
            }
            else if (true == dioService.IsInportOn((int)EnumSmartIC.Inports.punchDown))
            {
                if (++countPunchUp > 3)
                {
                    if (true == isStop)
                        sysService.State = SystemService.States.stop;
                    dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                    msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.punch_down);

                    ret = -1;
                }
            }
            else
            {
                countPunchUp = 0;
            }

            DoEvents(1);

            return ret;
        }

        protected virtual int CheckSwitch()
        {
            if( null == dioService )
                return -1;

            int ret = 0;

            // Uncoiler Reel Clutch Check
            if (false == dioService.IsInportOn((int)EnumSmartIC.Inports.uncoilerClutch))
            {
                sysService.State = SystemService.States.stop;
                dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.uncoilerReel_clutch);

                ret = -1;
            }

            // Uncoiler Reel Air Chuck
            if (false == dioService.IsInportOn((int)EnumSmartIC.Inports.uncoilerAirChuck))
            {
                sysService.State = SystemService.States.stop;
                dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.uncoilerReel_airchuck);

                ret = -1;
            }

            // Uncoiler Sheet Clutch Check
            if (false == dioService.IsInportOn((int)EnumSmartIC.Inports.uncoilerSheetClutch))
            {
                sysService.State = SystemService.States.stop;
                dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.uncoilerSheet_clutch);

                ret = -1;
            }

            // Uncoiler Sheet Air Chuck
            if (false == dioService.IsInportOn((int)EnumSmartIC.Inports.uncoilerSheetClutch))
            {
                sysService.State = SystemService.States.stop;
                dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.uncoilerSheet_airchuck);

                ret = -1;
            }

            // Uncoiler Sheet Torque
            if (false == dioService.IsInportOn((int)EnumSmartIC.Inports.uncoilerSheetTorque))
            {
                sysService.State = SystemService.States.stop;
                dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.uncoilerSheet_torque);

                ret = -1;
            }




            // Recoiler Reel Clutch Check
            if (false == dioService.IsInportOn((int)EnumSmartIC.Inports.recoilerReelClutch))
            {
                sysService.State = SystemService.States.stop;
                dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.recoilerReel_clutch);

                ret = -1;
            }

            // Recoiler Reel Air Chuck
            if (false == dioService.IsInportOn((int)EnumSmartIC.Inports.recoilerReelAirChuck))
            {
                sysService.State = SystemService.States.stop;
                dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.recoilerReel_airchuck);

                ret = -1;
            }

            // Recoiler Sheet Clutch Check
            if (false == dioService.IsInportOn((int)EnumSmartIC.Inports.recoilerSheetClutch))
            {
                sysService.State = SystemService.States.stop;
                dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.recoilerSheet_clutch);

                ret = -1;
            }

            // Recoiler Sheet Air Chuck
            if (false == dioService.IsInportOn((int)EnumSmartIC.Inports.recoilerSheetClutch))
            {
                sysService.State = SystemService.States.stop;
                dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.recoilerSheet_airchuck);

                ret = -1;
            }

            // Recoiler Sheet Torque
            if (false == dioService.IsInportOn((int)EnumSmartIC.Inports.recoilerSheetTorque))
            {
                sysService.State = SystemService.States.stop;
                dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.recoilerSheet_torque);

                ret = -1;
            }



            return ret;
        }

        protected void SetVisionAutoFeeding()
        {
            //threadVision = new Thread(new ThreadStart(this.ThreadVision));
            //threadVision.Start();

            threadVision = new Thread(ThreadVision);
            threadVision.IsBackground = true;
            threadVision.Start();

        }

        protected void SetPunchAutoFeeding()
        {
            //threadPunch = new Thread(new ThreadStart(this.ThreadPunch));
            //threadPunch.Start();

            threadPunch = new Thread(ThreadPunch);
            threadPunch.IsBackground = true;
            threadPunch.Start();
        }

        protected void ThreadVision()
        {
            VisionProcess();
        }

        protected virtual void VisionProcess()
        {
            int step = 0;
            int axisFeed = (int)EnumSmartIC.Axis.visionFeed;
            int axisBuffer = (int)EnumSmartIC.Axis.buffer;
            
            double vel = dataService.DataMotion[axisFeed].VelMove;
            double accel = dataService.DataMotion[axisFeed].AccelMove;

            double limitN = dataService.DataSystem.BufferLimitN;
            double limitP = dataService.DataSystem.BufferLimitP;

            Log_Trace.WriteLine("LimitN = {0}", limitN);

            double middle = limitN + (limitP - limitN) * 0.5;

            double posBuffer = motionService.GetCurrentPosition(axisBuffer);

            DateTime start = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 500);
            DateTime timeout = start.Add(duration);

            while (true == IsRun)
            {
                switch (step)
                {
                    case 0:
                        step += 10;
                        break;
                    case 10:
                        step += 10;
                        break;
                    case 20:
                        step += 10;
                        break;
                    case 30:
                        step = 100;
                        break;

                    case 100:
                        if (false == dataService.IsBackFeeding)        // BackFeeding 이 아닐 경우에만 진행
                            step += 10;
                        break;
                    case 110:
                        // Vision Feeding
                        motionService.JogP(axisFeed, vel, accel);
                        step += 10;
                        break;
                    case 120:
                        if (SystemService.States.pause == sysService.State)
                        {
                            step = 200;
                        }
                        else    // Check Limit N
                        {
                            posBuffer = motionService.GetCurrentPosition(axisBuffer);

                            if (posBuffer < limitN)
                            {
                                Log_Trace.WriteLine("posBuffer ={0} < LimitN = {1}", posBuffer, limitN);

                                motionService.Stop(axisFeed);
                                step += 5;
                            }
                            else if (true == dataService.IsBackFeeding)
                            {
                                step = 100;
                            }
                        }
                        break;
                    case 125:
                        if (true == motionService.IsMotionDone(axisFeed))
                        {
                            start = DateTime.Now;
                            duration = new TimeSpan(0, 0, 0, 0, 100);
                            timeout = start.Add(duration);
                            step += 1;
                        }
                        break;
                    case 126:
                        if (timeout < DateTime.Now)
                            step += 1;
                        break;
                    case 127:
                        if (0 == SetBackFeeding())
                            step = 130;
                        break;
                    case 130:
                        // Wait middle Position
                        posBuffer = motionService.GetCurrentPosition(axisBuffer);

                        if (posBuffer > limitP)
                            step = 100;
                        break;

                        // Pause
                    case 200:
                        step += 10;
                        break;
                    case 210:
                        motionService.Stop(axisFeed);
                        step += 10;
                        break;
                    case 220:
                        if (SystemService.States.pause != sysService.State)
                            step = 100;
                        break;

                    default:
                        break;
                }

            }

            motionService.Stop(axisFeed);
        }

        protected void ThreadPunch()
        {
            //if (true == dataService.DataSystem.IsSelectedPunch)
                PunchProcess();
            //else
            //    PunchProcess_Disuse();
        }

        protected virtual void PunchProcess()
        {
            return;
        }

        protected virtual void EndProcess()
        {
        }

        protected virtual int SetInitSearch()
        {
            if (null == sysService)
                return -1;
            if (null == dioService)
                return -1;
            if (null == motionService)
                return -1;
            if (null == seqService)
                return -1;

            int step = 0;
            int oldStep = -1;
            int ret = 0;

            int axisVision = (int)EnumSmartIC.Axis.visionFeed;
            int axisPunch = (int)EnumSmartIC.Axis.punchFeed;

            double vel = dataService.DataMotion[axisVision].VelMove;
            double accel = dataService.DataMotion[axisVision].AccelMove;

            double scanTolerance = dataService.DataSystem.ScanTolerance;

            int triggerChannel = 0;

            double scanLength = (double)(dataService.DataRecipe.PF * dataService.DataRecipe.ScanUnits) * 4.75;
            double scanOffset = 0.0;// (double)(dataService.DataTeach.PF) * 4.75 * dataService.DataTeach.PreScan;
            double scanDummyTop = dataService.DataSystem.ScanDummyTop;
            double scanDummyBottom = dataService.DataSystem.ScanDummyBottom;
            double scanDummyMono = dataService.DataSystem.ScanDummyMono;

            double scanDummy = scanDummyTop;

            double triggerStart = - scanDummy;
            double triggerEnd = scanLength * 1.5 + scanDummy * 2.0;
            double triggerPeriod = dataService.DataSystem.TriggerColor_Period;
            double width = dataService.DataSystem.TriggerColor_Width;

            double pos = scanOffset + dataService.DataSystem.ScanTolerance + scanDummy;
            double value = 0.0;
            double add = 0.0;

            dataService.JobInitOffset = 0.0;

            const int stepInitScan = 1000;
            const int stepRePos = 2000;

            bool isChecked = false;

            int countSearch = 0;


            while (SystemService.States.systemLock > sysService.State)
            {
                if (oldStep != step)
                {
                    oldStep = step;

                    //System.Diagnostics.Debug.WriteLine(string.Format("{0}", step));
                }

                // 임시로 Teaching 모드로 변경
                //topService.Write("MODE,UPDATE,TEACH");
                switch (step)
                {
                    case 0:
                        step += 10;
                        break;
                    case 10:
                        if (true == motionService.IsMotionDone(axisVision))
                        {
                            if (true == motionService.IsMotionDone(axisPunch))
                            {
                                // Set Position 
                                motionService.SetPosition(axisVision, 0);
                                motionService.SetPosition(axisPunch, 0);
                                cntService.SetPosition(0, 0);
                                step += 10;
                            }
                        }
                        break;

                    case 20:
                        // SetTrigger Position
                        triggerChannel = 0;
                        triggerStart = - scanDummy;
                        //triggerStart = 0.0;
                        triggerEnd = scanLength * 1.5 + scanDummy * 2.0;
                        triggerPeriod = dataService.DataSystem.TriggerColor_Period;
                        width = dataService.DataSystem.TriggerColor_Width;

                        step += 10;
                        break;
                    case 30:
                        step = 1000;
                        break;


                    case stepInitScan:
                        step += 10;
                        break;
                    case stepInitScan + 10:
                        // Offset 과 Init Unit 갯수만큼 이동한다. 
                        pos = scanOffset + value + dataService.DataSystem.ScanTolerance + scanDummy;

                        motionService.RMove(axisVision, -pos, vel, accel);
                        motionService.RMove(axisPunch, -pos, vel, accel);

                        step += 10;
                        break;
                    case stepInitScan + 20:
                        if (true == motionService.IsMotionDone(axisVision))
                        {
                            if (true == motionService.IsMotionDone(axisPunch))
                            {
                                SetTimeout(1000);
                                step += 10;
                            }
                        }
                        break;
                    case stepInitScan + 30:
                        if (true == IsTimeout())
                        {
                            cntService.SetTriggerParams(triggerChannel, triggerStart, triggerEnd, triggerPeriod, width);

                            //if (true == dataService.DataSystem.IsSelectedMono)
                            //    cntService.SetTriggerParams(1, triggerStart, triggerEnd, 0.00475, 10.0);

                            step += 10;
                        }
                        break;
                    case stepInitScan + 40:
                        cntService.SetTriggerEnable(triggerChannel);

                        //if (true == dataService.DataSystem.IsSelectedMono)
                        //    cntService.SetTriggerEnable(1);
                        step += 10;
                        break;
                    case stepInitScan + 50:
                        // Send Init Scan
                        topService.SetScanInit();
                        //if (true == dataService.DataSystem.IsSelectedMono)
                        //    monoService.SetScanInit();
                        step += 10;
                        break;
                    case stepInitScan + 60:
                        if (true == topService.IsReply((int)EnumSmartIC.VisionReplys.scan))
                        {
                            step += 10;
                        }
                        break;
                    case stepInitScan + 70:
                        pos = scanOffset * 2.0 + value + dataService.DataSystem.ScanTolerance * 2.0 + scanLength + scanDummy * 2.0 + 5.0 + add;

                        //System.Diagnostics.Debug.WriteLine(string.Format("RMove = {0:0.000}", pos));

                        // Scan Start
                        motionService.RMove(axisVision, pos, vel, accel);
                        motionService.RMove(axisPunch, pos, vel, accel);

                        //System.Diagnostics.Debug.WriteLine(string.Format("Scan Move = {0:0.000}", pos));

                        step += 10;
                        break;
                    case stepInitScan + 80:
                        if (true == motionService.IsMotionDone(axisVision))
                        {
                            if (true == motionService.IsMotionDone(axisPunch))
                            {
                                cntService.ResetTriggerEnable(0);
                                cntService.ResetTriggerEnable(1);
                                cntService.ResetTriggerEnable(2);
                                cntService.ResetTriggerEnable(3);

                                SetTimeout(100);
                                step += 10;
                            }
                        }
                        break;
                    case stepInitScan + 90:
                        // Move To 0.0
                        if (true == IsTimeout())
                        {
                            motionService.AMove(axisVision, 0.0, vel, accel);
                            motionService.AMove(axisPunch, 0.0, vel, accel);
                            step += 10;
                        }
                        break;
                    case stepInitScan + 100:
                        if (true == motionService.IsMotionDone(axisVision))
                        {
                            if (true == motionService.IsMotionDone(axisPunch))
                            {
                                SetTimeout(100);
                                step += 10;
                            }
                        }
                        break;
                    case stepInitScan + 110:
                        if (true == IsTimeout())
                        {
                            if( false == isChecked )
                                SetTimeout(1000);
                            else
                                SetTimeout(500);
                            step += 10;
                        }
                        break;
                    case stepInitScan + 120:
                        if (true == topService.IsFlag((int)EnumSmartIC.VisionFlags.initPos))
                            step = stepRePos;
                        else
                        {
                            if (true == IsTimeout())
                            {
                                if (false == isChecked)
                                {
                                    msgService.ShowMessage((int)EnumSmartIC.LightAlarms.jobInitPos);
                                    ret = -1;
                                }
                                step = 20000;
                            }
                        }
                        break;


                    case stepRePos:
                        step += 10;
                        break;
                    case stepRePos + 10:
                        pos = dataService.JobInitOffset;

                        motionService.RMove(axisVision, pos, vel, accel);
                        motionService.RMove(axisPunch, pos, vel, accel);

                        step += 10;
                        break;
                    case stepRePos + 20:
                        if (true == motionService.IsMotionDone(axisVision))
                        {
                            if (true == motionService.IsMotionDone(axisPunch))
                            {
                                SetTimeout(500);
                                step += 10;
                            }
                        }
                        break;
                    case stepRePos + 30:
                        if (true == IsTimeout())
                        {
                            // Set Position 
                            motionService.SetPosition(axisVision, 0);
                            motionService.SetPosition(axisPunch, 0);
                            cntService.SetPosition(0, 0);
                            step += 10;
                        }
                        break;
                    case stepRePos + 40:
                        isChecked = true;

                        if (5 < ++countSearch)
                        {
                            msgService.ShowMessage((int)EnumSmartIC.LightAlarms.jobInitPos);
                            ret = -1;
                            step = 20000;
                        }
                        else
                        {
                            step = 0;
                        }
                        break;
                        
                    case 20000:
                        motionService.Stop(axisVision);
                        motionService.Stop(axisPunch);

                        cntService.ResetTriggerEnable(0);

                        //topService.Write("MODE,UPDATE,INSPECT");

                        return ret;
                    default:
                        return -1;
                }
                System.Windows.Forms.Application.DoEvents();
            }

            motionService.Stop(axisVision);
            motionService.Stop(axisPunch);
            cntService.ResetTriggerEnable(0);

            //topService.Write("MODE,UPDATE,INSPECT");

            return -1;
        }

        protected virtual int SetInitRMove(double value)
        {
            int step = 0;
            int axisVision = (int)EnumSmartIC.Axis.visionFeed;
            int axisPunch = (int)EnumSmartIC.Axis.punchFeed;

            double vel = dataService.DataMotion[axisVision].VelMove;
            double accel = dataService.DataMotion[axisVision].AccelMove;


            dataService.FeedVelocity = vel;

            while (SystemService.States.stop > sysService.State)
            {
                switch (step)
                {
                    case 0:
                        step += 10;
                        break;
                    case 10:
                        // R Move P
                        motionService.RMove(axisVision, value, vel, accel);
                        motionService.RMove(axisPunch, value, vel, accel);
                        step += 10;
                        break;
                    case 20:
                        step += 10;
                        break;

                    case 30:
                        if (true == motionService.IsMotionDone(axisVision))
                        {
                            if (true == motionService.IsMotionDone(axisPunch))
                                step += 10;
                        }
                        break;
                    case 40:
                        step = 20000;
                        break;


                    case 20000:
                        motionService.Stop(axisVision);
                        motionService.Stop(axisPunch);

                        return 0;
                    default:
                        break;
                }
                System.Windows.Forms.Application.DoEvents();
            }

            motionService.Stop(axisVision);
            motionService.Stop(axisPunch);

            return -1;
        }

        // Modify 창에서 NG/Good 유무 판정값을 받아와서 펀칭을 한다. 
        protected int SetModifyPunch(double offsetX, double offsetY, int index, int ipHole)
        {
            // Punching 설정이 안되어 있을 경우는 Modify 완료 신호만 기다린다. 
            return 0;
        }

        // Modify 없이 자동으로 펀칭한다. (offsetX:비전옵셋값, offsetY:비전옵셋값, index:전체인덱스값, ipHole:펀칭 시작위치에서의 ipHole 위치)
        protected virtual int SetPunch(double offsetX, double offsetY, int index, int ipHole, bool isTHolePunch = true, bool isPunchAll = false)
        {
            // 3열
            if (3 == dataService.DataRecipe.Line)
            {
                return SetPunch3(offsetX, offsetY, index, ipHole, isTHolePunch, isPunchAll);
            }

            if (true == dataService.DataSystem.IsSelectedPunch)
            {
                int axisX = (int)EnumSmartIC.Axis.punchX;
                int axisY = (int)EnumSmartIC.Axis.punchY;

                double posX = 0.0, posY = 0.0;

                double posAlignX = 0.0, posAlignY = 0.0;

                double velX = dataService.DataMotion[axisX].VelMove;
                double accelX = dataService.DataMotion[axisX].AccelMove;

                double velY = dataService.DataMotion[axisY].VelMove;
                double accelY = dataService.DataMotion[axisY].AccelMove;

                double width = dataService.DataRecipe.Width;

                bool isCheckPunchUp = true;

                int step = 0;
                int delay = 50;
                int delayPunch = dataService.DataSystem.DelayPunch;

                dataService.DataRecipe.AlignOffsetX = 0.0;
                dataService.DataRecipe.AlignOffsetY = 0.0;

                // 0번째
                posAlignX = dataService.DataSystem.AlignVisionX + dataService.DataRecipe.AlignOffsetX - 4.75 * (double)ipHole * dataService.DataRecipe.PF + dataService.DataSystem.AlignPosOffsetX;
                posAlignY = dataService.DataSystem.AlignVisionY + dataService.DataRecipe.AlignOffsetY + dataService.DataSystem.AlignPosOffsetY;

                while (SystemService.States.stop > sysService.State)
                {
                    if (true == isCheckPunchUp)
                    {
                        if (0 != CheckPunchUp())
                            break;
                    }

                    switch (step)
                    {
                        case 0:
                            step += 10;

                            if (dataService.DataResult.IndexLastPunch == index)
                            {
                                if ("A" == dataService.DataResult.LineLastPunch)
                                    step = 100;
                            }
                            break;

                        case 10:
                            isCheckPunchUp = true;

                            if ("G" != dataService.DataResult.Total.ListRaw[index].Value1 || isPunchAll)
                            {
                                posX = posAlignX + offsetX - dataService.DataRecipe.PunchX - dataService.DataSystem.PunchOffsetX;
                                posY = posAlignY + offsetY + dataService.DataRecipe.PunchY - dataService.DataSystem.PunchOffsetY;

                                dataService.DataSystem.AlignXLastPos = posX;
                                dataService.DataSystem.AlignYLastPos = posY;

                                // NG 홀일 경우에는 다시 펀칭 하지 않는다.
                                if ("C" != dataService.DataResult.Total.ListRaw[index].Value1 || isPunchAll)
                                {
                                    motionService.AMove(axisX, posX, velX, accelX);
                                    motionService.AMove(axisY, posY, velY, accelY);

                                    step += 10;
                                }
                                else
                                {
                                    step = 100;
                                }

                                seqService.FireEventPunch(index, (int)EnumSmartIC.PunchStates.punchLine, "A");
                            }
                            else
                            {
                                step = 100;
                            }
                            break;

                        case 20:
                            // Wait Motion Done
                            if (motionService.IsMotionDone(axisX))
                            {
                                if (motionService.IsMotionDone(axisY))
                                {
                                    if (IsInPos(axisX, posX, dataService.DataSystem.MotionTolerance))
                                    {
                                        SetTimeout(delay);
                                        step += 10;
                                    }
                                }
                            }
                            break;

                        case 30:
                            // Punch Down
                            isCheckPunchUp = false;
                            if (IsTimeout())
                            {
                                SetPunchDown();

                                dataService.DataResult.IndexLastPunch = index;
                                dataService.DataResult.LineLastPunch = "A";
                                step += 10;
                            }
                            break;

                        case 40:
                            // Wait Punch Down
                            if (IsPunchDown())
                            {
                                step += 10;
                            }
                            break;

                        case 50:
                            // Punch Up
                            {
                                SetPunchUp();
                                step += 10;
                            }
                            break;

                        case 60:
                            // Wait Punch Up
                            if (IsPunchUp())
                            {
                                isCheckPunchUp = true;

                                dataService.DataResult.AddPunch(index, "A");

                                SetTimeout(delayPunch);
                                step += 10;
                            }
                            break;

                        case 70:
                            if (IsTimeout())
                                step = 100;
                            break;

                        case 100:
                            step += 10;

                            dataService.DataSystem.AlignXLastPos = posAlignX + offsetX - dataService.DataRecipe.PunchX - dataService.DataSystem.PunchOffsetX;
                            dataService.DataSystem.AlignYLastPos = posAlignY + offsetY + dataService.DataRecipe.PunchY - dataService.DataSystem.PunchOffsetY;

                            if (dataService.DataResult.IndexLastPunch == index)
                            {
                                if ("B" == dataService.DataResult.LineLastPunch)
                                {
                                    dataService.DataSystem.AlignXLastPos = posAlignX + offsetX - dataService.DataRecipe.PunchX - dataService.DataSystem.PunchOffsetX - dataService.DataSystem.PunchOffsetBX;
                                    dataService.DataSystem.AlignYLastPos = posAlignY + offsetY + (width - dataService.DataRecipe.PunchY) - dataService.DataSystem.PunchOffsetY - dataService.DataSystem.PunchOffsetBY;
                                    step = 20000;
                                }
                            }
                            break;

                        case 110:
                            isCheckPunchUp = true;

                            if ("G" != dataService.DataResult.Total.ListRaw[index].Value2 || isPunchAll)
                            {
                                posX = posAlignX + offsetX - dataService.DataRecipe.PunchX - dataService.DataSystem.PunchOffsetX - dataService.DataSystem.PunchOffsetBX;
                                posY = posAlignY + offsetY + (width - dataService.DataRecipe.PunchY) - dataService.DataSystem.PunchOffsetY - dataService.DataSystem.PunchOffsetBY;

                                dataService.DataSystem.AlignXLastPos = posX;
                                dataService.DataSystem.AlignYLastPos = posY;

                                if ("C" != dataService.DataResult.Total.ListRaw[index].Value2 || isPunchAll)
                                {
                                    motionService.AMove(axisX, posX, velX, accelX);
                                    motionService.AMove(axisY, posY, velY, accelY);

                                    step += 10;
                                }
                                else
                                {
                                    dataService.DataSystem.AlignXLastPos = motionService.GetCurrentPosition((int)EnumSmartIC.Axis.punchX);
                                    step = 20000;
                                }

                                seqService.FireEventPunch(index, (int)EnumSmartIC.PunchStates.punchLine, "B");
                            }
                            else
                            {
                                step = 20000;
                            }
                            break;

                        case 120:
                            // Wait Motion Done
                            if (motionService.IsMotionDone(axisX))
                            {
                                if (motionService.IsMotionDone(axisY))
                                {
                                    if (IsInPos(axisX, posX, dataService.DataSystem.MotionTolerance))
                                    {
                                        SetTimeout(delay);
                                        step += 10;
                                    }
                                }
                            }
                            break;

                        case 130:
                            // Punch Down
                            isCheckPunchUp = false;
                            if (IsTimeout())
                            {
                                SetPunchDown();

                                dataService.DataResult.IndexLastPunch = index;
                                dataService.DataResult.LineLastPunch = "B";
                                step += 10;
                            }
                            break;

                        case 140:
                            // Wait Punch Down
                            if (IsPunchDown())
                            {
                                step += 10;
                            }
                            break;

                        case 150:
                            // Punch Up
                            {
                                SetPunchUp();
                                step += 10;
                            }
                            break;

                        case 160:
                            // Wait Punch Up
                            if (IsPunchUp())
                            {
                                isCheckPunchUp = true;
                                dataService.DataResult.AddPunch(index, "B");

                                SetTimeout(delayPunch);
                                step += 10;
                            }
                            break;

                        case 170:
                            if (IsTimeout())
                                step = 20000;
                            break;

                        case 20000:
                            // [2호기] 마지막 펀칭 위치 이송
                            // 피드 및 x y 값을 갱신한다.
                            dataService.DataSystem.VisionLastPos = motionService.GetCurrentPosition((int)EnumSmartIC.Axis.visionFeed);
                            dataService.DataSystem.FeedLastPos = motionService.GetCurrentPosition((int)EnumSmartIC.Axis.punchFeed);
                            dataService.DataSystem.BufferLastPos = motionService.GetCurrentPosition((int)EnumSmartIC.Axis.buffer);
                            dataService.DataSystem.LaserIndex = index;

                            Log_SeqPunch.WriteLine("Set punch Vision Feed {0:0.000}", dataService.DataSystem.VisionLastPos);
                            Log_SeqPunch.WriteLine("Set punch Punch Feed {0:0.000}", dataService.DataSystem.FeedLastPos);
                            Log_SeqPunch.WriteLine("Set punch Align X {0:0.000}", dataService.DataSystem.AlignXLastPos);
                            Log_SeqPunch.WriteLine("Set punch Align Y {0:0.000}", dataService.DataSystem.AlignYLastPos);
                            Log_SeqPunch.WriteLine("Set punch buffer {0:0.000}", dataService.DataSystem.BufferLastPos);
                            Log_SeqPunch.WriteLine("Set punch index {0} {1}", dataService.DataSystem.LaserIndex, index);
                            Log_SeqPunch.WriteLine("Set punch VALUE1 {0} VALUE2{1}", dataService.DataResult.Total.ListRaw[index].Value1, dataService.DataResult.Total.ListRaw[index].Value2);

                            SetPunchUp();
                            return 0;

                        default:
                            SetPunchUp();
                            return -1;

                    }

                    System.Windows.Forms.Application.DoEvents();
                }

                // Punch Up
                SetPunchUp();
            }

            // Punch Up
            SetPunchUp();
            return -1;
        }

        protected virtual int SetPunch3(double offsetX, double offsetY, int index, int ipHole, bool isTHolePunch = true, bool isPunchAll = false)
        {
            if (true == dataService.DataSystem.IsSelectedPunch)
            {
                int axisX = (int)EnumSmartIC.Axis.punchX;
                int axisY = (int)EnumSmartIC.Axis.punchY;

                double posX = 0.0, posY = 0.0;

                double posAlignX = 0.0, posAlignY = 0.0;

                double velX = dataService.DataMotion[axisX].VelMove;
                double accelX = dataService.DataMotion[axisX].AccelMove;

                double velY = dataService.DataMotion[axisY].VelMove;
                double accelY = dataService.DataMotion[axisY].AccelMove;

                double width = dataService.DataRecipe.Width;

                bool isCheckPunchUp = true;

                int step = 0;
                int delay = 50;
                int delayPunch = dataService.DataSystem.DelayPunch;

                dataService.DataRecipe.AlignOffsetX = 0.0;
                dataService.DataRecipe.AlignOffsetY = 0.0;

                // 0번째
                posAlignX = dataService.DataSystem.AlignVisionX + dataService.DataRecipe.AlignOffsetX - 4.75 * (double)ipHole * dataService.DataRecipe.PF + dataService.DataSystem.AlignPosOffsetX;
                posAlignY = dataService.DataSystem.AlignVisionY + dataService.DataRecipe.AlignOffsetY + dataService.DataSystem.AlignPosOffsetY;

                while (SystemService.States.stop > sysService.State)
                {
                    if (true == isCheckPunchUp)
                    {
                        if (0 != CheckPunchUp())
                            break;
                    }

                    switch (step)
                    {
                        #region 1LINE
                        case 0:
                            step += 10;

                            if (dataService.DataResult.IndexLastPunch == index)
                            {
                                if ("A" == dataService.DataResult.LineLastPunch)
                                    step = 100;
                            }
                            break;
                        case 10:
                            isCheckPunchUp = true;

                            if ("G" != dataService.DataResult.Total.ListRaw[index].Value1 || true == isPunchAll)
                            {
                                posX = posAlignX + offsetX - dataService.DataRecipe.PunchX - dataService.DataSystem.PunchOffsetX;
                                posY = posAlignY + offsetY + dataService.DataRecipe.PunchY - dataService.DataSystem.PunchOffsetY;

                                dataService.DataSystem.AlignXLastPos = posX;
                                dataService.DataSystem.AlignYLastPos = posY;

                                // NG 홀일 경우에는 다시 펀칭 하지 않는다.
                                if ("C" != dataService.DataResult.Total.ListRaw[index].Value1)
                                {
                                    motionService.AMove(axisX, posX, velX, accelX);
                                    motionService.AMove(axisY, posY, velY, accelY);

                                    step += 10;
                                }
                                else
                                {
                                    step = 100;
                                }

                                seqService.FireEventPunch(index, (int)EnumSmartIC.PunchStates.punchLine, "A");
                            }
                            else
                            {
                                step = 100;
                            }
                            break;
                        case 20:
                            // Wait Motion Done
                            if (true == motionService.IsMotionDone(axisX))
                            {
                                if (true == motionService.IsMotionDone(axisY))
                                {
                                    if (true == IsInPos(axisX, posX, dataService.DataSystem.MotionTolerance))
                                    {
                                        SetTimeout(delay);
                                        step += 10;
                                    }
                                }
                            }
                            break;
                        case 30:
                            // Punch Down
                            isCheckPunchUp = false;
                            if (true == IsTimeout())
                            {
                                SetPunchDown();

                                dataService.DataResult.IndexLastPunch = index;
                                dataService.DataResult.LineLastPunch = "A";
                                step += 10;
                            }
                            break;
                        case 40:
                            // Wait Punch Down
                            if (true == IsPunchDown())
                            {
                                step += 10;
                            }
                            //step += 10;
                            break;
                        case 50:
                            // Punch Up
                            {
                                SetPunchUp();
                                step += 10;
                            }
                            break;
                        case 60:
                            // Wait Punch Up
                            if (true == IsPunchUp())
                            {
                                isCheckPunchUp = true;

                                dataService.DataResult.AddPunch(index, "A");

                                SetTimeout(delayPunch);
                                step += 10;
                            }
                            break;
                        case 70:
                            if (true == IsTimeout())
                                step = 100;
                            break;
                        #endregion

                        #region 2LINE
                        case 100:
                            step += 10;

                            dataService.DataSystem.AlignXLastPos = posAlignX + offsetX - dataService.DataRecipe.PunchX - dataService.DataSystem.PunchOffsetX;
                            dataService.DataSystem.AlignYLastPos = posAlignY + offsetY + dataService.DataRecipe.PunchY - dataService.DataSystem.PunchOffsetY;

                            if (dataService.DataResult.IndexLastPunch == index)
                            {
                                if ("B" == dataService.DataResult.LineLastPunch)
                                {
                                    dataService.DataSystem.AlignXLastPos = posAlignX + offsetX - dataService.DataRecipe.PunchCenterX - dataService.DataSystem.PunchOffsetX - dataService.DataSystem.PunchOffsetCenterX;
                                    dataService.DataSystem.AlignYLastPos = posAlignY + offsetY + dataService.DataRecipe.PunchCenterY - dataService.DataSystem.PunchOffsetY - dataService.DataSystem.PunchOffsetCenterY;
                                    step = 200;
                                }
                            }
                            break;
                        case 110:
                            isCheckPunchUp = true;

                            if ("G" != dataService.DataResult.Total.ListRaw[index].Value2 || true == isPunchAll)
                            {
                                posX = posAlignX + offsetX - dataService.DataRecipe.PunchCenterX - dataService.DataSystem.PunchOffsetX - dataService.DataSystem.PunchOffsetCenterX;
                                posY = posAlignY + offsetY + dataService.DataRecipe.PunchCenterY - dataService.DataSystem.PunchOffsetY - dataService.DataSystem.PunchOffsetCenterY;

                                dataService.DataSystem.AlignXLastPos = posX;
                                dataService.DataSystem.AlignYLastPos = posY;

                                if ("C" != dataService.DataResult.Total.ListRaw[index].Value2)
                                {
                                    motionService.AMove(axisX, posX, velX, accelX);
                                    motionService.AMove(axisY, posY, velY, accelY);

                                    step += 10;
                                }
                                else
                                {
                                    step = 200;
                                }

                                seqService.FireEventPunch(index, (int)EnumSmartIC.PunchStates.punchLine, "B");
                            }
                            else
                            {
                                step = 200;
                            }
                            break;
                        case 120:
                            // Wait Motion Done
                            if (true == motionService.IsMotionDone(axisX))
                            {
                                if (true == motionService.IsMotionDone(axisY))
                                {
                                    if (true == IsInPos(axisX, posX, dataService.DataSystem.MotionTolerance))
                                    {
                                        SetTimeout(delay);
                                        step += 10;
                                    }
                                }
                            }
                            break;
                        case 130:
                            // Punch Down
                            isCheckPunchUp = false;
                            if (true == IsTimeout())
                            {
                                SetPunchDown();

                                dataService.DataResult.IndexLastPunch = index;
                                dataService.DataResult.LineLastPunch = "B";
                                step += 10;
                            }
                            break;
                        case 140:
                            // Wait Punch Down
                            if (true == IsPunchDown())
                            {
                                step += 10;
                            }
                            //step += 10;
                            break;
                        case 150:
                            // Punch Up
                            {
                                SetPunchUp();
                                step += 10;
                            }
                            break;
                        case 160:
                            // Wait Punch Up
                            if (true == IsPunchUp())
                            {
                                isCheckPunchUp = true;
                                dataService.DataResult.AddPunch(index, "B");

                                SetTimeout(delayPunch);
                                step += 10;
                            }
                            break;
                        case 170:
                            if (true == IsTimeout())
                                step = 200;
                            break;
                        #endregion


                        #region 3LINE
                        case 200:
                            step += 10;

                            dataService.DataSystem.AlignXLastPos = posAlignX + offsetX - dataService.DataRecipe.PunchCenterX - dataService.DataSystem.PunchOffsetX - dataService.DataSystem.PunchOffsetCenterX;
                            dataService.DataSystem.AlignYLastPos = posAlignY + offsetY + dataService.DataRecipe.PunchCenterY - dataService.DataSystem.PunchOffsetY - dataService.DataSystem.PunchOffsetCenterY;

                            if (dataService.DataResult.IndexLastPunch == index)
                            {
                                if ("C" == dataService.DataResult.LineLastPunch)
                                {
                                    dataService.DataSystem.AlignXLastPos = posAlignX + offsetX - dataService.DataRecipe.PunchX - dataService.DataSystem.PunchOffsetX - dataService.DataSystem.PunchOffsetBX;
                                    dataService.DataSystem.AlignYLastPos = posAlignY + offsetY + (width - dataService.DataRecipe.PunchY) - dataService.DataSystem.PunchOffsetY - dataService.DataSystem.PunchOffsetBY;
                                    step = 20000;
                                }
                            }
                            break;
                        case 210:
                            isCheckPunchUp = true;

                            if ("G" != dataService.DataResult.Total.ListRaw[index].Value3 || true == isPunchAll)
                            {
                                posX = posAlignX + offsetX - dataService.DataRecipe.PunchX - dataService.DataSystem.PunchOffsetX - dataService.DataSystem.PunchOffsetBX;
                                posY = posAlignY + offsetY + (width - dataService.DataRecipe.PunchY) - dataService.DataSystem.PunchOffsetY - dataService.DataSystem.PunchOffsetBY;

                                dataService.DataSystem.AlignXLastPos = posX;
                                dataService.DataSystem.AlignYLastPos = posY;

                                if ("C" != dataService.DataResult.Total.ListRaw[index].Value3)
                                {
                                    motionService.AMove(axisX, posX, velX, accelX);
                                    motionService.AMove(axisY, posY, velY, accelY);

                                    step += 10;
                                }
                                else
                                {
                                    step = 20000;
                                }

                                seqService.FireEventPunch(index, (int)EnumSmartIC.PunchStates.punchLine, "C");
                            }
                            else
                            {
                                dataService.DataSystem.AlignXLastPos = motionService.GetCurrentPosition((int)EnumSmartIC.Axis.punchX);
                                step = 20000;
                            }
                            break;
                        case 220:
                            // Wait Motion Done
                            if (true == motionService.IsMotionDone(axisX))
                            {
                                if (true == motionService.IsMotionDone(axisY))
                                {
                                    if (true == IsInPos(axisX, posX, dataService.DataSystem.MotionTolerance))
                                    {
                                        SetTimeout(delay);
                                        step += 10;
                                    }
                                }
                            }
                            break;
                        case 230:
                            // Punch Down
                            isCheckPunchUp = false;
                            if (true == IsTimeout())
                            {
                                SetPunchDown();

                                dataService.DataResult.IndexLastPunch = index;
                                dataService.DataResult.LineLastPunch = "C";
                                step += 10;
                            }
                            break;
                        case 240:
                            // Wait Punch Down
                            if (true == IsPunchDown())
                            {
                                step += 10;
                            }
                            break;
                        case 250:
                            // Punch Up
                            {
                                SetPunchUp();
                                step += 10;
                            }
                            break;
                        case 260:
                            // Wait Punch Up
                            if (true == IsPunchUp())
                            {
                                isCheckPunchUp = true;
                                dataService.DataResult.AddPunch(index, "C");

                                SetTimeout(delayPunch);
                                step += 10;
                            }
                            break;
                        case 270:
                            if (true == IsTimeout())
                                step = 20000;
                            break;
                        #endregion

                        case 20000:
                            SetPunchUp();
                            return 0;

                        default:
                            SetPunchUp();
                            return -1;

                    }

                    System.Windows.Forms.Application.DoEvents();
                }

                // Punch Up
                SetPunchUp();
            }

            // Punch Up
            SetPunchUp();
            return -1;
        }

        protected virtual int SetPunchInspect(double offsetX, double offsetY, int index, int ipHole, bool isTHolePunch, bool isPunchAll = false)
        {
            // 3열
            if (3 == dataService.DataRecipe.Line)
            {
                return SetPunchInspect3(offsetX, offsetY, index, ipHole, isTHolePunch, isPunchAll);
            }

            if (false == dataService.DataSystem.IsSelectedPunchInspect)
                return 0;

            if (0 == dataService.DataSystem.PunchInspectUnit)
                return 0;

            if (index < NextPunchInspect)
                return 0;


            JobWindowService jobWindowService = JobWindowService.Singleton;

            NextPunchInspect += dataService.DataSystem.PunchInspectUnit;
            

            HObject hImage;
            HOperatorSet.GenEmptyObj(out hImage);


            int axisX = (int)EnumSmartIC.Axis.punchX;
            int axisY = (int)EnumSmartIC.Axis.punchY;
            int inspectResult = 0;

            double posX = 0.0, posY = 0.0;

            double posAlignX = 0.0, posAlignY = 0.0;

            double velX = dataService.DataMotion[axisX].VelMove;
            double accelX = dataService.DataMotion[axisX].AccelMove;

            double velY = dataService.DataMotion[axisY].VelMove;
            double accelY = dataService.DataMotion[axisY].AccelMove;

            double width = dataService.DataRecipe.Width;

            double punchOffsetX = 0.0;
            double punchOffsetY = 0.0;

            bool isCheckPunchUp = true;
            //bool isXMove = false;
            int step = 0;
            int delay = 50;
            //int delayPunch = 100;

            string imageName;
            string path;

            dataService.DataRecipe.AlignOffsetX = 0.0;
            dataService.DataRecipe.AlignOffsetY = 0.0;

            // 0번째......
            posAlignX = dataService.DataSystem.AlignVisionX + dataService.DataRecipe.AlignOffsetX - 4.75 * (double)ipHole * dataService.DataRecipe.PF + dataService.DataSystem.AlignPosOffsetX;
            posAlignY = dataService.DataSystem.AlignVisionY + dataService.DataRecipe.AlignOffsetY + dataService.DataSystem.AlignPosOffsetY;

            while (SystemService.States.stop > sysService.State)
            {
                if (true == isCheckPunchUp)
                {
                    if (0 != CheckPunchUp())
                        break;
                }

                switch (step)
                {
                    #region 1LINE
                    case 0:
                        step += 10;
                        if ("BB039" == dataService.DataResult.Total.ListRaw[index].Value1)
                        {
                            if (false == isTHolePunch) step = 100;
                        }
                        break;
                    case 10:
                        isCheckPunchUp = true;
                        if ("G" != dataService.DataResult.Total.ListRaw[index].Value1 || true == isPunchAll)
                        {
                            if ("C" != dataService.DataResult.Total.ListRaw[index].Value1)
                            {
                                posX = posAlignX + offsetX - dataService.DataRecipe.PunchX;
                                posY = posAlignY + offsetY + dataService.DataRecipe.PunchY;

                                //Log_Trace.WriteLine("Punch Pos A = posAlingX + offsetX - PunchX : {0:0.000} = {1:0.000} + {2:0.000} - {3:0.000}",
                                //    posX, posAlignX, offsetX, dataService.DataRecipe.PunchX);

                                motionService.AMove(axisX, posX, velX, accelX);
                                motionService.AMove(axisY, posY, velY, accelY);
                                step += 10;
                            }
                            else
                            {
                                step = 100;
                            }
                        }
                        else
                        {
                            step = 100;
                        }
                        break;
                    case 20:
                        // Wait Motion Done
                        if (true == motionService.IsMotionDone(axisX))
                        {
                            if (true == motionService.IsMotionDone(axisY))
                            {
                                if (true == IsInPos(axisX, posX, dataService.DataSystem.MotionTolerance))
                                {
                                    if (null != hImage)
                                        hImage.Dispose();
                                    GC.Collect();

                                    //Log_Trace.WriteLine("Real PunchPos X : {0:0.000}", motionService.GetCurrentPosition(axisX));

                                    SetTimeout(delay);
                                    step += 10;
                                }
                            }
                        }
                        break;
                    case 30:
                        if (true == IsTimeout())
                        {
                            grabService.Grab(out hImage);
                            if (null != hImage)
                                HOperatorSet.DispObj(hImage, hWindow);

                            imageName = string.Format("index{0:000000}_Punch_A.jpg", index + 1);
                            path = dataService.DataSystem.PunchPath + "\\" + imageName;

                            inspectResult = inspectSrvice.InspectCircle(hWindow, hImage, out punchOffsetX, out punchOffsetY,
                                dataService.DataRecipe.searchRow1, dataService.DataRecipe.searchCol1, dataService.DataRecipe.searchRow2, dataService.DataRecipe.searchCol2,
                                dataService.DataRecipe.grayMin, dataService.DataRecipe.grayMax, path);

                            

                            //HOperatorSet.WriteImage(hImage, "jpg", 0, path);
                            //inspectSrvice.SaveImage(path);
                            dataService.DataDefect.AddPunchInspect(index, "A", punchOffsetX, punchOffsetY, imageName);

                            seqService.FireEventPunchImage(path, index, "A", punchOffsetX, punchOffsetY);
                            step += 10;
                        }
                        break;
                    case 40:
                        // 펀치 검사여부 확인
                        if (0 > inspectResult)
                        {
                            // Show Punch Inspect Error
                            jobWindowService.ShowMessageWindow((int)EnumSmartIC.LightAlarms.punchInspect);
                            step += 5;
                        }
                        else
                        {
                            step += 10;
                        }
                        break;
                    case 45:
                        // 확인 할 때 까지 대기
                        if (true == jobWindowService.IsMessageWindowClosed)
                            step += 5;
                        break;
                    case 50:
                        // 펀치  허용공차 검사 
                        if (true == dataService.DataSystem.IsSelectedPunchTolerance)
                        {
                            if (Math.Abs(punchOffsetX) > dataService.DataRecipe.PunchToleranceX || Math.Abs(punchOffsetY) > dataService.DataRecipe.PunchToleranceY)
                            {
                                // Show Punch Error Message
                                jobWindowService.ShowMessageWindow((int)EnumSmartIC.LightAlarms.punchTolerance);
                                step += 5;
                            }
                            else
                            {
                                step += 10;
                            }
                        }
                        else
                            step += 10;
                        break;
                    case 55:
                        // 확인 할 때 까지 대기
                        if (true == jobWindowService.IsMessageWindowClosed)
                            step += 5;
                        break;

                    case 60:
                        step += 10;
                        break;
                    case 70:
                        step = 100;
                        break;
                    #endregion

                    #region 2LINE
                    case 100:
                        step += 10;
                        if ("BB039" == dataService.DataResult.Total.ListRaw[index].Value2)
                        {
                            if (false == isTHolePunch) step = 20000;
                        }
                        break;
                    case 110:
                        isCheckPunchUp = true;
                        if ("G" != dataService.DataResult.Total.ListRaw[index].Value2 || true == isPunchAll)
                        {
                            if ("C" != dataService.DataResult.Total.ListRaw[index].Value2)
                            {
                                posX = posAlignX + offsetX - dataService.DataRecipe.PunchX;// -dataService.DataSystem.PunchOffsetBX;
                                posY = posAlignY + offsetY + (width - dataService.DataRecipe.PunchY);// +dataService.DataSystem.PunchOffsetBY;

                                //Log_Trace.WriteLine("Punch Pos B : {0:0.000} = {1:0.000} + {2:0.000} - {3:0.000}",
                                //    posX, posAlignX, offsetX, dataService.DataRecipe.PunchX);

                                motionService.AMove(axisY, posY, velY, accelY);
                                motionService.AMove(axisX, posX, velX, accelX);
                                step += 10;
                            }
                            else
                            {
                                step = 20000;
                            }
                        }
                        else
                        {
                            step = 20000;
                        }
                        break;
                    case 120:
                        // Wait Motion Done
                        if (true == motionService.IsMotionDone(axisX))
                        {
                            if (true == motionService.IsMotionDone(axisY))
                            {
                                if (true == IsInPos(axisX, posX, dataService.DataSystem.MotionTolerance))
                                {
                                    if (null != hImage)
                                        hImage.Dispose();
                                    GC.Collect();

                                    //Log_Trace.WriteLine("Real PunchPos X : {0:0.000}", motionService.GetCurrentPosition(axisX));

                                    SetTimeout(delay);
                                    step += 10;
                                }
                            }
                        }
                        break;
                    case 130:
                        // Punch Down
                        if (true == IsTimeout())
                        {
                            grabService.Grab(out hImage);
                            if (null != hImage)
                                HOperatorSet.DispObj(hImage, hWindow);

                            imageName = string.Format("index{0:000000}_Punch_B.jpg", index + 1);
                            path = dataService.DataSystem.PunchPath + "\\" + imageName;

                            inspectResult = inspectSrvice.InspectCircle(hWindow, hImage, out punchOffsetX, out punchOffsetY,
                                dataService.DataRecipe.searchRow1, dataService.DataRecipe.searchCol1, dataService.DataRecipe.searchRow2, dataService.DataRecipe.searchCol2,
                                dataService.DataRecipe.grayMin, dataService.DataRecipe.grayMax, path);

                            

                            //HOperatorSet.WriteImage(hImage, "jpg", 0, path);
                            //inspectSrvice.SaveImage(path);

                            dataService.DataDefect.AddPunchInspect(index, "B", punchOffsetX, punchOffsetY, imageName);

                            seqService.FireEventPunchImage(path, index, "B", punchOffsetX, punchOffsetY);
                            step += 10;
                        }
                        break;
                    case 140:
                        // 펀치 검사여부 확인
                        if (0 > inspectResult)
                        {
                            // Show Punch Inspect Error
                            jobWindowService.ShowMessageWindow((int)EnumSmartIC.LightAlarms.punchInspect);
                            step += 5;
                        }
                        else
                        {
                            step += 10;
                        }
                        break;
                    case 145:
                        // 확인 할 때 까지 대기
                        if (true == jobWindowService.IsMessageWindowClosed)
                            step += 5;
                        break;
                    case 150:
                        // 펀치  허용공차 검사 
                        if (true == dataService.DataSystem.IsSelectedPunchTolerance)
                        {
                            if (Math.Abs(punchOffsetX) > dataService.DataRecipe.PunchToleranceX || Math.Abs(punchOffsetY) > dataService.DataRecipe.PunchToleranceY)
                            {
                                // Show Punch Error Message
                                jobWindowService.ShowMessageWindow((int)EnumSmartIC.LightAlarms.punchTolerance);
                                step += 5;
                            }
                            else
                            {
                                step += 10;
                            }
                        }
                        else
                            step += 10;
                        break;
                    case 155:
                        // 확인 할 때 까지 대기
                        if (true == jobWindowService.IsMessageWindowClosed)
                            step += 5;
                        break;
                    case 160:
                        step += 10;
                        break;
                    case 170:
                        step = 20000;
                        break;
                    #endregion

                    case 20000:
                        dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);

                        if (null != hImage)
                            hImage.Dispose();
                        GC.Collect();
                        return 0;

                    default:
                        dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);
                        if (null != hImage)
                            hImage.Dispose();
                        GC.Collect();
                        return -1;

                }

                System.Windows.Forms.Application.DoEvents();
            }

            // Punch Up
            dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
            dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);


            if (null != hImage)
                hImage.Dispose();
            GC.Collect();

            return -1;
        }

        protected virtual int SetPunchInspect3(double offsetX, double offsetY, int index, int ipHole, bool isTHolePunch, bool isPunchAll = false)
        {
            if (false == dataService.DataSystem.IsSelectedPunchInspect)
                return 0;

            if (0 == dataService.DataSystem.PunchInspectUnit)
                return 0;

            if (index < NextPunchInspect)
                return 0;


            JobWindowService jobWindowService = JobWindowService.Singleton;

            NextPunchInspect += dataService.DataSystem.PunchInspectUnit;


            HObject hImage;
            HOperatorSet.GenEmptyObj(out hImage);


            int axisX = (int)EnumSmartIC.Axis.punchX;
            int axisY = (int)EnumSmartIC.Axis.punchY;
            int inspectResult = 0;

            double posX = 0.0, posY = 0.0;

            double posAlignX = 0.0, posAlignY = 0.0;

            double velX = dataService.DataMotion[axisX].VelMove;
            double accelX = dataService.DataMotion[axisX].AccelMove;

            double velY = dataService.DataMotion[axisY].VelMove;
            double accelY = dataService.DataMotion[axisY].AccelMove;

            double width = dataService.DataRecipe.Width;

            double punchOffsetX = 0.0;
            double punchOffsetY = 0.0;

            bool isCheckPunchUp = true;
            //bool isXMove = false;
            int step = 0;
            int delay = 50;
            //int delayPunch = 100;

            string imageName;
            string path;

            dataService.DataRecipe.AlignOffsetX = 0.0;
            dataService.DataRecipe.AlignOffsetY = 0.0;

            // 0번째......
            posAlignX = dataService.DataSystem.AlignVisionX + dataService.DataRecipe.AlignOffsetX - 4.75 * (double)ipHole * dataService.DataRecipe.PF + dataService.DataSystem.AlignPosOffsetX;
            posAlignY = dataService.DataSystem.AlignVisionY + dataService.DataRecipe.AlignOffsetY + dataService.DataSystem.AlignPosOffsetY;

            while (SystemService.States.stop > sysService.State)
            {
                if (true == isCheckPunchUp)
                {
                    if (0 != CheckPunchUp())
                        break;
                }

                switch (step)
                {
                    #region 1LINE
                    case 0:
                        step += 10;
                        if ("BB039" == dataService.DataResult.Total.ListRaw[index].Value1)
                        {
                            if (false == isTHolePunch) step = 100;
                        }
                        break;
                    case 10:
                        isCheckPunchUp = true;
                        if ("G" != dataService.DataResult.Total.ListRaw[index].Value1 || true == isPunchAll)
                        {
                            if ("C" != dataService.DataResult.Total.ListRaw[index].Value1)
                            {
                                posX = posAlignX + offsetX - dataService.DataRecipe.PunchX;
                                posY = posAlignY + offsetY + dataService.DataRecipe.PunchY;
                                                               

                                //Log_Trace.WriteLine("Punch Pos A = posAlingX + offsetX - PunchX : {0:0.000} = {1:0.000} + {2:0.000} - {3:0.000}",
                                //    posX, posAlignX, offsetX, dataService.DataRecipe.PunchX);

                                motionService.AMove(axisX, posX, velX, accelX);
                                motionService.AMove(axisY, posY, velY, accelY);
                                step += 10;
                            }
                            else
                            {
                                step = 100;
                            }
                        }
                        else
                        {
                            step = 100;
                        }
                        break;
                    case 20:
                        // Wait Motion Done
                        if (true == motionService.IsMotionDone(axisX))
                        {
                            if (true == motionService.IsMotionDone(axisY))
                            {
                                if (true == IsInPos(axisX, posX, dataService.DataSystem.MotionTolerance))
                                {
                                    if (null != hImage)
                                        hImage.Dispose();
                                    GC.Collect();

                                    //Log_Trace.WriteLine("Real PunchPos X : {0:0.000}", motionService.GetCurrentPosition(axisX));

                                    SetTimeout(delay);
                                    step += 10;
                                }
                            }
                        }
                        break;
                    case 30:
                        if (true == IsTimeout())
                        {
                            grabService.Grab(out hImage);
                            if (null != hImage)
                                HOperatorSet.DispObj(hImage, hWindow);

                            imageName = string.Format("index{0:000000}_Punch_A.jpg", index + 1);
                            path = dataService.DataSystem.PunchPath + "\\" + imageName;

                            inspectResult = inspectSrvice.InspectCircle(hWindow, hImage, out punchOffsetX, out punchOffsetY,
                                dataService.DataRecipe.searchRow1, dataService.DataRecipe.searchCol1, dataService.DataRecipe.searchRow2, dataService.DataRecipe.searchCol2,
                                dataService.DataRecipe.grayMin, dataService.DataRecipe.grayMax, path);



                            //HOperatorSet.WriteImage(hImage, "jpg", 0, path);
                            //inspectSrvice.SaveImage(path);
                            dataService.DataDefect.AddPunchInspect(index, "A", punchOffsetX, punchOffsetY, imageName);

                            seqService.FireEventPunchImage(path, index, "A", punchOffsetX, punchOffsetY);
                            step += 10;
                        }
                        break;
                    case 40:
                        // 펀치 검사여부 확인
                        if (0 > inspectResult)
                        {
                            // Show Punch Inspect Error
                            jobWindowService.ShowMessageWindow((int)EnumSmartIC.LightAlarms.punchInspect);
                            step += 5;
                        }
                        else
                        {
                            step += 10;
                        }
                        break;
                    case 45:
                        // 확인 할 때 까지 대기
                        if (true == jobWindowService.IsMessageWindowClosed)
                            step += 5;
                        break;
                    case 50:
                        // 펀치  허용공차 검사 
                        if (true == dataService.DataSystem.IsSelectedPunchTolerance)
                        {
                            if (Math.Abs(punchOffsetX) > dataService.DataRecipe.PunchToleranceX || Math.Abs(punchOffsetY) > dataService.DataRecipe.PunchToleranceY)
                            {
                                // Show Punch Error Message
                                jobWindowService.ShowMessageWindow((int)EnumSmartIC.LightAlarms.punchTolerance);
                                step += 5;
                            }
                            else
                            {
                                step += 10;
                            }
                        }
                        else
                            step += 10;
                        break;
                    case 55:
                        // 확인 할 때 까지 대기
                        if (true == jobWindowService.IsMessageWindowClosed)
                            step += 5;
                        break;

                    case 60:
                        step += 10;
                        break;
                    case 70:
                        step = 100;
                        break;
                    #endregion

                    #region 2LINE
                    case 100:
                        step += 10;
                        if ("BB039" == dataService.DataResult.Total.ListRaw[index].Value2)
                        {
                            if (false == isTHolePunch) step = 20000;
                        }
                        break;
                    case 110:
                        isCheckPunchUp = true;
                        if ("G" != dataService.DataResult.Total.ListRaw[index].Value2 || true == isPunchAll)
                        {
                            if ("C" != dataService.DataResult.Total.ListRaw[index].Value2)
                            {
                                posX = posAlignX + offsetX - dataService.DataRecipe.PunchCenterX;
                                posY = posAlignY + offsetY + dataService.DataRecipe.PunchCenterY;

                                //Log_Trace.WriteLine("Punch Pos B : {0:0.000} = {1:0.000} + {2:0.000} - {3:0.000}",
                                //    posX, posAlignX, offsetX, dataService.DataRecipe.PunchX);

                                motionService.AMove(axisY, posY, velY, accelY);
                                motionService.AMove(axisX, posX, velX, accelX);
                                step += 10;
                            }
                            else
                            {
                                step = 200;
                            }
                        }
                        else
                        {
                            step = 200;
                        }
                        break;
                    case 120:
                        // Wait Motion Done
                        if (true == motionService.IsMotionDone(axisX))
                        {
                            if (true == motionService.IsMotionDone(axisY))
                            {
                                if (true == IsInPos(axisX, posX, dataService.DataSystem.MotionTolerance))
                                {
                                    if (null != hImage)
                                        hImage.Dispose();
                                    GC.Collect();

                                    //Log_Trace.WriteLine("Real PunchPos X : {0:0.000}", motionService.GetCurrentPosition(axisX));

                                    SetTimeout(delay);
                                    step += 10;
                                }
                            }
                        }
                        break;
                    case 130:
                        // Punch Down
                        if (true == IsTimeout())
                        {
                            grabService.Grab(out hImage);
                            if (null != hImage)
                                HOperatorSet.DispObj(hImage, hWindow);

                            imageName = string.Format("index{0:000000}_Punch_B.jpg", index + 1);
                            path = dataService.DataSystem.PunchPath + "\\" + imageName;

                            inspectResult = inspectSrvice.InspectCircle(hWindow, hImage, out punchOffsetX, out punchOffsetY,
                                dataService.DataRecipe.searchRow1, dataService.DataRecipe.searchCol1, dataService.DataRecipe.searchRow2, dataService.DataRecipe.searchCol2,
                                dataService.DataRecipe.grayMin, dataService.DataRecipe.grayMax, path);



                            //HOperatorSet.WriteImage(hImage, "jpg", 0, path);
                            //inspectSrvice.SaveImage(path);

                            dataService.DataDefect.AddPunchInspect(index, "B", punchOffsetX, punchOffsetY, imageName);

                            seqService.FireEventPunchImage(path, index, "B", punchOffsetX, punchOffsetY);
                            step += 10;
                        }
                        break;
                    case 140:
                        // 펀치 검사여부 확인
                        if (0 > inspectResult)
                        {
                            // Show Punch Inspect Error
                            jobWindowService.ShowMessageWindow((int)EnumSmartIC.LightAlarms.punchInspect);
                            step += 5;
                        }
                        else
                        {
                            step += 10;
                        }
                        break;
                    case 145:
                        // 확인 할 때 까지 대기
                        if (true == jobWindowService.IsMessageWindowClosed)
                            step += 5;
                        break;
                    case 150:
                        // 펀치  허용공차 검사 
                        if (true == dataService.DataSystem.IsSelectedPunchTolerance)
                        {
                            if (Math.Abs(punchOffsetX) > dataService.DataRecipe.PunchToleranceX || Math.Abs(punchOffsetY) > dataService.DataRecipe.PunchToleranceY)
                            {
                                // Show Punch Error Message
                                jobWindowService.ShowMessageWindow((int)EnumSmartIC.LightAlarms.punchTolerance);
                                step += 5;
                            }
                            else
                            {
                                step += 10;
                            }
                        }
                        else
                            step += 10;
                        break;
                    case 155:
                        // 확인 할 때 까지 대기
                        if (true == jobWindowService.IsMessageWindowClosed)
                            step += 5;
                        break;
                    case 160:
                        step += 10;
                        break;
                    case 170:
                        step = 200;
                        break;
                    #endregion

                    // 3열
                    #region 3LINE
                    case 200:
                        step += 10;
                        if ("BB039" == dataService.DataResult.Total.ListRaw[index].Value3)
                        {
                            if (false == isTHolePunch) step = 20000;
                        }
                        break;
                    case 210:
                        isCheckPunchUp = true;
                        if ("G" != dataService.DataResult.Total.ListRaw[index].Value3 || true == isPunchAll)
                        {
                            if ("C" != dataService.DataResult.Total.ListRaw[index].Value3)
                            {
                                posX = posAlignX + offsetX - dataService.DataRecipe.PunchX;// -dataService.DataSystem.PunchOffsetBX;
                                posY = posAlignY + offsetY + (width - dataService.DataRecipe.PunchY);// +dataService.DataSystem.PunchOffsetBY;

                                //Log_Trace.WriteLine("Punch Pos B : {0:0.000} = {1:0.000} + {2:0.000} - {3:0.000}",
                                //    posX, posAlignX, offsetX, dataService.DataRecipe.PunchX);

                                motionService.AMove(axisY, posY, velY, accelY);
                                motionService.AMove(axisX, posX, velX, accelX);
                                step += 10;
                            }
                            else
                            {
                                step = 20000;
                            }
                        }
                        else
                        {
                            step = 20000;
                        }
                        break;
                    case 220:
                        // Wait Motion Done
                        if (true == motionService.IsMotionDone(axisX))
                        {
                            if (true == motionService.IsMotionDone(axisY))
                            {
                                if (true == IsInPos(axisX, posX, dataService.DataSystem.MotionTolerance))
                                {
                                    if (null != hImage)
                                        hImage.Dispose();
                                    GC.Collect();

                                    //Log_Trace.WriteLine("Real PunchPos X : {0:0.000}", motionService.GetCurrentPosition(axisX));

                                    SetTimeout(delay);
                                    step += 10;
                                }
                            }
                        }
                        break;
                    case 230:
                        // Punch Down
                        if (true == IsTimeout())
                        {
                            grabService.Grab(out hImage);
                            if (null != hImage)
                                HOperatorSet.DispObj(hImage, hWindow);

                            imageName = string.Format("index{0:000000}_Punch_C.jpg", index + 1);
                            path = dataService.DataSystem.PunchPath + "\\" + imageName;

                            inspectResult = inspectSrvice.InspectCircle(hWindow, hImage, out punchOffsetX, out punchOffsetY,
                                dataService.DataRecipe.searchRow1, dataService.DataRecipe.searchCol1, dataService.DataRecipe.searchRow2, dataService.DataRecipe.searchCol2,
                                dataService.DataRecipe.grayMin, dataService.DataRecipe.grayMax, path);
                            
                            dataService.DataDefect.AddPunchInspect(index, "C", punchOffsetX, punchOffsetY, imageName);

                            seqService.FireEventPunchImage(path, index, "C", punchOffsetX, punchOffsetY);
                            step += 10;
                        }
                        break;
                    case 240:
                        // 펀치 검사여부 확인
                        if (0 > inspectResult)
                        {
                            // Show Punch Inspect Error
                            jobWindowService.ShowMessageWindow((int)EnumSmartIC.LightAlarms.punchInspect);
                            step += 5;
                        }
                        else
                        {
                            step += 10;
                        }
                        break;
                    case 245:
                        // 확인 할 때 까지 대기
                        if (true == jobWindowService.IsMessageWindowClosed)
                            step += 5;
                        break;
                    case 250:
                        // 펀치  허용공차 검사 
                        if (true == dataService.DataSystem.IsSelectedPunchTolerance)
                        {
                            if (Math.Abs(punchOffsetX) > dataService.DataRecipe.PunchToleranceX || Math.Abs(punchOffsetY) > dataService.DataRecipe.PunchToleranceY)
                            {
                                // Show Punch Error Message
                                jobWindowService.ShowMessageWindow((int)EnumSmartIC.LightAlarms.punchTolerance);
                                step += 5;
                            }
                            else
                            {
                                step += 10;
                            }
                        }
                        else
                            step += 10;
                        break;
                    case 255:
                        // 확인 할 때 까지 대기
                        if (true == jobWindowService.IsMessageWindowClosed)
                            step += 5;
                        break;
                    case 260:
                        step += 10;
                        break;
                    case 270:
                        step = 20000;
                        break;
                    #endregion
                        

                    case 20000:
                        dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);

                        if (null != hImage)
                            hImage.Dispose();
                        GC.Collect();
                        return 0;

                    default:
                        dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);
                        if (null != hImage)
                            hImage.Dispose();
                        GC.Collect();
                        return -1;

                }

                System.Windows.Forms.Application.DoEvents();
            }

            // Punch Up
            dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
            dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);


            if (null != hImage)
                hImage.Dispose();
            GC.Collect();

            return -1;
        }

        protected void SetTimeout(int mSec)
        {
            timeoutSpan = new TimeSpan(0, 0, 0, 0, mSec);
            timeout = DateTime.Now;
            timeout = timeout.Add(timeoutSpan);
        }

        protected bool IsTimeout()
        {
            if (timeout < DateTime.Now)
                return true;

            return false;
        }

        protected int SetPunchTest(int iphole = 0, int line=0)
        {
            if (3 == dataService.DataTeach.Line)
            {
                return SetPunchTest3(iphole, line);
            }

            HObject hImage;
            HOperatorSet.GenEmptyObj(out hImage);

            int axisX = (int)EnumSmartIC.Axis.punchX;
            int axisY = (int)EnumSmartIC.Axis.punchY;

            double offsetX = 0, offsetY = 0;
            double posX = 0.0, posY = 0.0;

            double velX = dataService.DataMotion[axisX].VelMove;
            double accelX = dataService.DataMotion[axisX].AccelMove;

            double velY = dataService.DataMotion[axisY].VelMove;
            double accelY = dataService.DataMotion[axisY].AccelMove;

            int step = 0;

            bool isCheckPunchUp = true;

            //// IP Hole 검사위치 세팅
            //posX = dataService.DataTeach.AlignX + dataService.DataTeach.AlignOffsetX - 4.75 * (double)iphole;
            //posY = dataService.DataTeach.AlignY + dataService.DataTeach.AlignOffsetY;

            posX = dataService.DataSystem.AlignVisionX + dataService.DataTeach.AlignOffsetX - 4.75 * (double)iphole;
            posY = dataService.DataSystem.AlignVisionY + dataService.DataTeach.AlignOffsetY;

            if (0 != CheckPunchUp())
                return -1;

            while (SystemService.States.stop > sysService.State)
            {
                if (true == isCheckPunchUp)
                {
                    if (0 != CheckPunchUp())
                        break;
                }

                switch (step)
                {
                    case 0:
                        step += 10;
                        break;
                    case 10:
                        // Move To Align position
                        isCheckPunchUp = true;
                        motionService.AMove(axisX, posX, velX, accelX);
                        motionService.AMove(axisY, posY, velY, accelY);
                        step += 10;
                        break;
                    case 20:
                        // Wait Motion Done
                        if (true == motionService.IsMotionDone(axisX))
                        {
                            if (true == motionService.IsMotionDone(axisY))
                            {
                                isCheckPunchUp = true;
                                SetTimeout(300);
                                step += 10;
                            }
                        }
                        break;
                    case 30:
                        // Inspect IP Hole
                        if (true == IsTimeout())
                        {
                            grabService.Grab(out hImage);
                            if( null != hImage)
                                HOperatorSet.DispObj(hImage, hWindow);
                            inspectSrvice.Inspect(hWindow, hImage, out offsetX, out offsetY);

                            step += 10;
                        }
                        break;
                    case 40:
                        isCheckPunchUp = true;
                        if (0 == line)
                        {
                            posX = offsetX - dataService.DataTeach.PunchX - dataService.DataSystem.PunchOffsetX;
                            posY = offsetY + dataService.DataTeach.PunchY - dataService.DataSystem.PunchOffsetY;
                        }
                        else
                        {
                            posX = offsetX - dataService.DataTeach.PunchX - dataService.DataSystem.PunchOffsetX - dataService.DataSystem.PunchOffsetBX;
                            posY = offsetY + (dataService.DataTeach.Width - dataService.DataTeach.PunchY) - dataService.DataSystem.PunchOffsetY - dataService.DataSystem.PunchOffsetBY;
                        }
                        motionService.RMove(axisX, posX, velX, accelX);
                        motionService.RMove(axisY, posY, velY, accelY);
                        step += 10;
                        break;
                    case 50:
                        // Wait Motion Done
                        if (true == motionService.IsMotionDone(axisX))
                        {
                            if (true == motionService.IsMotionDone(axisY))
                            {
                                SetTimeout(300);
                                step += 10;
                            }
                        }
                        break;
                    case 60:
                        // Punch Down
                        isCheckPunchUp = false;
                        dioService.SetOutport((int)EnumSmartIC.Outports.punchDown);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.punchUp);
                        step += 10;
                        break;
                    case 70:
                        // Wait Punch Down
                        if (true == dioService.IsInportOn((int)EnumSmartIC.Inports.punchDown))
                        {
                            if (false == dioService.IsInportOn((int)EnumSmartIC.Inports.punchUp))
                            {
                                step += 10;
                            }
                        }
                        break;
                    case 80:
                        // Punch Up
                        dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);
                        step += 10;
                        break;
                    case 90:
                        // Wait Punch Up
                        if (true == dioService.IsInportOn((int)EnumSmartIC.Inports.punchUp))
                        {
                            if (false == dioService.IsInportOn((int)EnumSmartIC.Inports.punchDown))
                            {
                                step += 10;
                            }
                        }
                        break;
                    case 100:
                        // Move To Punch Position
                        isCheckPunchUp = true;

                        //posX = dataService.DataSystem.PunchOffsetX;
                        //posY = dataService.DataSystem.PunchOffsetY;

                        if (0 == line)
                        {
                            posX = dataService.DataSystem.PunchOffsetX;
                            posY = dataService.DataSystem.PunchOffsetY;
                        }
                        else
                        {
                            posX = dataService.DataSystem.PunchOffsetX;
                            posY = dataService.DataSystem.PunchOffsetY + dataService.DataSystem.PunchOffsetBY;
                        }


                        motionService.RMove(axisX, posX, velX, accelX);
                        motionService.RMove(axisY, posY, velY, accelY);
                        step += 10;
                        break;
                    case 110:
                        // Wait Motion Done
                        if (true == motionService.IsMotionDone(axisX))
                        {
                            if (true == motionService.IsMotionDone(axisY))
                            {
                                SetTimeout(300);
                                step += 10;
                            }
                        }
                        break;
                    case 120:
                        if (true == IsTimeout())
                        {
                            grabService.Grab(out hImage);
                            if (null != hImage)
                                HOperatorSet.DispObj(hImage, hWindow);
                            step += 10;
                        }
                        break;
                    case 130:
                        step = 20000;
                        break;

                    case 20000:
                        dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);
                        if( null != hImage)
                            hImage.Dispose();
                        return 0;
                }
                System.Windows.Forms.Application.DoEvents();
            }

            dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
            dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);
            
            if (null != hImage)
                hImage.Dispose();
            return -1;
        }

        protected int SetPunchTest3(int iphole = 0, int line = 0)
        {
            HObject hImage;
            HOperatorSet.GenEmptyObj(out hImage);

            int axisX = (int)EnumSmartIC.Axis.punchX;
            int axisY = (int)EnumSmartIC.Axis.punchY;

            double offsetX = 0, offsetY = 0;
            double posX = 0.0, posY = 0.0;

            double velX = dataService.DataMotion[axisX].VelMove;
            double accelX = dataService.DataMotion[axisX].AccelMove;

            double velY = dataService.DataMotion[axisY].VelMove;
            double accelY = dataService.DataMotion[axisY].AccelMove;

            int step = 0;

            bool isCheckPunchUp = true;

            //// IP Hole 검사위치 세팅
            //posX = dataService.DataTeach.AlignX + dataService.DataTeach.AlignOffsetX - 4.75 * (double)iphole;
            //posY = dataService.DataTeach.AlignY + dataService.DataTeach.AlignOffsetY;

            posX = dataService.DataSystem.AlignVisionX + dataService.DataTeach.AlignOffsetX - 4.75 * (double)iphole;
            posY = dataService.DataSystem.AlignVisionY + dataService.DataTeach.AlignOffsetY;

            if (0 != CheckPunchUp())
                return -1;

            while (SystemService.States.stop > sysService.State)
            {
                if (true == isCheckPunchUp)
                {
                    if (0 != CheckPunchUp())
                        break;
                }

                switch (step)
                {
                    case 0:
                        step += 10;
                        break;
                    case 10:
                        // Move To Align position
                        isCheckPunchUp = true;
                        motionService.AMove(axisX, posX, velX, accelX);
                        motionService.AMove(axisY, posY, velY, accelY);
                        step += 10;
                        break;
                    case 20:
                        // Wait Motion Done
                        if (true == motionService.IsMotionDone(axisX))
                        {
                            if (true == motionService.IsMotionDone(axisY))
                            {
                                isCheckPunchUp = true;
                                SetTimeout(300);
                                step += 10;
                            }
                        }
                        break;
                    case 30:
                        // Inspect IP Hole
                        if (true == IsTimeout())
                        {
                            grabService.Grab(out hImage);
                            if (null != hImage)
                                HOperatorSet.DispObj(hImage, hWindow);
                            inspectSrvice.Inspect(hWindow, hImage, out offsetX, out offsetY);

                            step += 10;
                        }
                        break;
                    case 40:
                        isCheckPunchUp = true;
                        if (0 == line)
                        {
                            posX = offsetX - dataService.DataTeach.PunchX - dataService.DataSystem.PunchOffsetX;
                            posY = offsetY + dataService.DataTeach.PunchY - dataService.DataSystem.PunchOffsetY;
                        }
                        else if (1 == line)
                        {
                            //posX = offsetX - dataService.DataTeach.PunchX - dataService.DataSystem.PunchOffsetCenterX;
                            //posY = offsetY + dataService.DataTeach.PunchY - dataService.DataSystem.PunchOffsetCenterY;

                            posX = offsetX - dataService.DataTeach.PunchCenterX - dataService.DataSystem.PunchOffsetX - dataService.DataSystem.PunchOffsetCenterX;
                            posY = offsetY + dataService.DataTeach.PunchCenterY - dataService.DataSystem.PunchOffsetY - dataService.DataSystem.PunchOffsetCenterY;
                        }
                        else
                        {
                            posX = offsetX - dataService.DataTeach.PunchX - dataService.DataSystem.PunchOffsetX - dataService.DataSystem.PunchOffsetBX;
                            posY = offsetY + (dataService.DataTeach.Width - dataService.DataTeach.PunchY) - dataService.DataSystem.PunchOffsetY - dataService.DataSystem.PunchOffsetBY;
                        }
                        motionService.RMove(axisX, posX, velX, accelX);
                        motionService.RMove(axisY, posY, velY, accelY);
                        step += 10;
                        break;
                    case 50:
                        // Wait Motion Done
                        if (true == motionService.IsMotionDone(axisX))
                        {
                            if (true == motionService.IsMotionDone(axisY))
                            {
                                SetTimeout(300);
                                step += 10;
                            }
                        }
                        break;
                    case 60:
                        // Punch Down
                        isCheckPunchUp = false;
                        dioService.SetOutport((int)EnumSmartIC.Outports.punchDown);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.punchUp);
                        step += 10;
                        break;
                    case 70:
                        // Wait Punch Down
                        if (true == dioService.IsInportOn((int)EnumSmartIC.Inports.punchDown))
                        {
                            if (false == dioService.IsInportOn((int)EnumSmartIC.Inports.punchUp))
                            {
                                step += 10;
                            }
                        }
                        break;
                    case 80:
                        // Punch Up
                        dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);
                        step += 10;
                        break;
                    case 90:
                        // Wait Punch Up
                        if (true == dioService.IsInportOn((int)EnumSmartIC.Inports.punchUp))
                        {
                            if (false == dioService.IsInportOn((int)EnumSmartIC.Inports.punchDown))
                            {
                                step += 10;
                            }
                        }
                        break;
                    case 100:
                        // Move To Punch Position
                        isCheckPunchUp = true;

                        //posX = dataService.DataSystem.PunchOffsetX;
                        //posY = dataService.DataSystem.PunchOffsetY;

                        if (0 == line)
                        {
                            posX = dataService.DataSystem.PunchOffsetX;
                            posY = dataService.DataSystem.PunchOffsetY;
                        }
                        else if (1 == line)
                        {
                            posX = dataService.DataSystem.PunchOffsetX + dataService.DataSystem.PunchOffsetCenterX;
                            posY = dataService.DataSystem.PunchOffsetY + dataService.DataSystem.PunchOffsetCenterY;
                        }
                        else
                        {
                            posX = dataService.DataSystem.PunchOffsetX;
                            posY = dataService.DataSystem.PunchOffsetY + dataService.DataSystem.PunchOffsetBY;
                        }


                        motionService.RMove(axisX, posX, velX, accelX);
                        motionService.RMove(axisY, posY, velY, accelY);
                        step += 10;
                        break;
                    case 110:
                        // Wait Motion Done
                        if (true == motionService.IsMotionDone(axisX))
                        {
                            if (true == motionService.IsMotionDone(axisY))
                            {
                                SetTimeout(300);
                                step += 10;
                            }
                        }
                        break;
                    case 120:
                        if (true == IsTimeout())
                        {
                            grabService.Grab(out hImage);
                            if (null != hImage)
                                HOperatorSet.DispObj(hImage, hWindow);
                            step += 10;
                        }
                        break;
                    case 130:
                        step = 20000;
                        break;

                    case 20000:
                        dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);
                        if (null != hImage)
                            hImage.Dispose();
                        return 0;
                }
                System.Windows.Forms.Application.DoEvents();
            }

            dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
            dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);

            if (null != hImage)
                hImage.Dispose();
            return -1;
        }

        protected int StartUncoilerRun()
        {
            //if (null != timerUncoiler)
            //{
            //    if (true == timerUncoiler.IsEnabled)
            //    {
            //        ResetFlag((int)EnumSmartIC.SeqFlags.uncoilerRun);

            //        DateTime start = DateTime.Now;
            //        TimeSpan duration = new TimeSpan(0, 0, 0, 0, 300);
            //        DateTime timeout = start.Add(duration);
                    
            //        while (timeout > DateTime.Now)
            //        {
            //            System.Windows.Forms.Application.DoEvents();
            //        }
            //    }

            //    timerUncoiler.IsEnabled = false;
            //    timerUncoiler.Tick -= timerUncoiler_Tick;
                
            //}
            int axisFeed = (int)EnumSmartIC.Axis.visionFeed;
            ResetFlag((int)EnumSmartIC.SeqFlags.uncoilerRun);
            ResetFlag((int)EnumSmartIC.SeqFlags.uncoilerReady);
            

            motionService.Stop(axisFeed);

            dataService.IsUncoilerRun = false;

            DateTime start = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 500);
            DateTime timeout = start.Add(duration);

            if (null != timerUncoiler)
            {
                if (true == timerUncoiler.IsEnabled)
                {
                    System.Windows.Forms.Application.DoEvents();
                }

                timerUncoiler.IsEnabled = false;
                timerUncoiler.Tick -= timerUncoiler_Tick;
            }

            if (true == dataService.IsUncoilerReady)
            {
                //start = DateTime.Now;
                //duration = new TimeSpan(0, 0, 0, 0, 500);
                //timeout = start.Add(duration);

                while (true == dataService.IsUncoilerReady)
                {
                    //if (timeout < DateTime.Now)
                    //    break;

                    System.Windows.Forms.Application.DoEvents();

                    if (SystemService.States.stop <= sysService.State)
                        return -1;
                }

            }

            

            start = DateTime.Now;
            duration = new TimeSpan(0, 0, 0, 0, 500);
            timeout = start.Add(duration);

            while (false == motionService.IsMotionDone(axisFeed))
            {
                if (timeout < DateTime.Now)
                    break;

                if (SystemService.States.stop <= sysService.State)
                    return -1;
                
                DoEvents(100);
                
            }

            if (false == dataService.IsUncoilerReady)
            {
                SetFlag((int)EnumSmartIC.SeqFlags.uncoilerRun);
                SetUncoilerDancer();
            }

            return 0;
        }

        protected int StopUncoilerRun()
        {
            // Set Timer
            ResetFlag((int)EnumSmartIC.SeqFlags.uncoilerReady);

            if (true == dataService.IsUncoilerRun)
            {
                if (null != timerUncoiler)
                {
                    if (true == timerUncoiler.IsEnabled)
                    {
                        //return 0;
                        timerUncoiler.IsEnabled = false;
                        timerUncoiler.Tick -= timerUncoiler_Tick;
                        ResetFlag((int)EnumSmartIC.SeqFlags.uncoilerRun);
                        DoEvents(100);
                        dataService.IsUncoilerRun = false;
                        

                        return 0;
                    }
                }

                timerUncoiler = new DispatcherTimer();
                timerUncoiler.Interval = new System.TimeSpan(0, 0, 0, 0, 1000);
                timerUncoiler.IsEnabled = true;
                timerUncoiler.Tick += new EventHandler(timerUncoiler_Tick);
            }

            return 0;
        }

        protected void timerUncoiler_Tick(object sender, EventArgs e)
        {
            ResetFlag((int)EnumSmartIC.SeqFlags.uncoilerRun);
            dataService.IsUncoilerRun = false;

            DispatcherTimer timer = sender as DispatcherTimer;
            if (null != timer)
            {
                timer.Stop();
                timer.IsEnabled = false;
                timer.Tick -= timerRecoiler_Tick;
            }

            timerUncoiler.Stop();
            timerUncoiler.IsEnabled = false;
            timerUncoiler.Tick -= timerUncoiler_Tick;
        }


        protected int StartRecoilerRun()
        {
            int axisFeed = (int)EnumSmartIC.Axis.punchFeed;
            
            ResetFlag((int)EnumSmartIC.SeqFlags.recoilerRun);
            ResetFlag((int)EnumSmartIC.SeqFlags.recoilerReady);

            motionService.Stop(axisFeed);

            dataService.IsRecoilerRun = false;

            DateTime start = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 500);
            DateTime timeout = start.Add(duration);

            if (null != timerRecoiler)
            {
                if (true == timerRecoiler.IsEnabled)
                {
                    System.Windows.Forms.Application.DoEvents();
                }

                timerRecoiler.IsEnabled = false;
                timerRecoiler.Tick -= timerUncoiler_Tick;
            }

            //if (true == IsFlag((int)EnumSmartIC.SeqFlags.recoilerRunning))
            if (true == dataService.IsRecoilerReady)
            {
                //start = DateTime.Now;
                //duration = new TimeSpan(0, 0, 0, 0, 500);
                //timeout = start.Add(duration);

                //while(true == IsFlag((int)EnumSmartIC.SeqFlags.recoilerRunning))
                while (true == dataService.IsRecoilerReady)
                {
                    //if (timeout < DateTime.Now)
                    //    break;
                    if (SystemService.States.stop <= sysService.State)
                        return -1;
                    System.Windows.Forms.Application.DoEvents();
                }

            }
            //if (null != timerRecoiler)
            //{
            //    if (true == timerRecoiler.IsEnabled)
            //    {
            //        ResetFlag((int)EnumSmartIC.SeqFlags.recoilerRun);

            //        DateTime start = DateTime.Now;
            //        TimeSpan duration = new TimeSpan(0, 0, 0, 0, 300);
            //        DateTime timeout = start.Add(duration);

            //        while (timeout > DateTime.Now)
            //        {
            //            System.Windows.Forms.Application.DoEvents();
            //        }
            //    }

            //    timerRecoiler.IsEnabled = false;
            //    timerRecoiler.Tick -= timerUncoiler_Tick;
            //}

            
            start = DateTime.Now;
            duration = new TimeSpan(0, 0, 0, 0, 500);
            timeout = start.Add(duration);

            while (false == motionService.IsMotionDone(axisFeed))
            {
                if (timeout < DateTime.Now)
                    break;
                if (SystemService.States.stop <= sysService.State)
                    return -1;
                DoEvents(100);
            }

            if (false == IsFlag((int)EnumSmartIC.SeqFlags.recoilerRun))
            {
                SetFlag((int)EnumSmartIC.SeqFlags.recoilerRun);
                SetRecoilerDancer();
            }

            return 0;
        }

        protected int StopRecoilerRun()
        {
            // Set Timer
            ResetFlag((int)EnumSmartIC.SeqFlags.recoilerReady);

            if (true == IsFlag((int)EnumSmartIC.SeqFlags.recoilerRun))
            {
                if (null != timerRecoiler)
                {
                    if (true == timerRecoiler.IsEnabled)
                    {
                        timerRecoiler.IsEnabled = false;
                        timerRecoiler.Tick -= timerRecoiler_Tick;
                        ResetFlag((int)EnumSmartIC.SeqFlags.recoilerRun);
                        DoEvents(100);
                        dataService.IsRecoilerRun = false;
                        return 0;
                    }
                }

                
                timerRecoiler = new DispatcherTimer();
                timerRecoiler.Interval = new System.TimeSpan(0, 0, 0, 0, 1000);
                timerRecoiler.IsEnabled = true;
                timerRecoiler.Tick += new EventHandler(timerRecoiler_Tick);
            }

            return 0;
        }

        protected void timerRecoiler_Tick(object sender, EventArgs e)
        {
            ResetFlag((int)EnumSmartIC.SeqFlags.recoilerRun);
            dataService.IsRecoilerRun = false;

            DispatcherTimer timer = sender as DispatcherTimer;
            if (null != timer)
            {
                timer.Stop();
                timer.IsEnabled = false;
                timer.Tick -= timerRecoiler_Tick;
            }
            
            timerRecoiler.IsEnabled = false;
            timerRecoiler.Tick -= timerRecoiler_Tick;
            timerRecoiler.Stop();
        }

        protected virtual int SetAutoJog()
        {
            ResetFlag((int)EnumSmartIC.SeqFlags.autoJogRun);

            return 0;
        }

        protected virtual int SetPunchDown()
        {
            dioService.SetOutport((int)EnumSmartIC.Outports.punchDown);
            dioService.ResetOutport((int)EnumSmartIC.Outports.punchUp);

            if (null != dataService)
            {
                dataService.DataSystem.PunchCount += 1;
                dataService.DataSystem.MES_PunchCount += 1;
            }
                
            return 0;
        }

        protected virtual int SetPunchUp()
        {
            dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
            dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);

            return 0;
        }

        protected virtual bool IsPunchUp()
        {
            if (true == dioService.IsInportOn((int)EnumSmartIC.Inports.punchUp))
            {
                if (false == dioService.IsInportOn((int)EnumSmartIC.Inports.punchDown))
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual bool IsPunchDown()
        {
            if (true == dioService.IsInportOn((int)EnumSmartIC.Inports.punchDown))
            {
                if (false == dioService.IsInportOn((int)EnumSmartIC.Inports.punchUp))
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual int SetCleanRoller()
        {
            dioService.SetOutport((int)EnumSmartIC.Outports.cleanUnitOn);
            dioService.ResetOutport((int)EnumSmartIC.Outports.cleanUnitOff);
            return 0;
        }

        protected virtual int ResetCleanRoller()
        {
            dioService.ResetOutport((int)EnumSmartIC.Outports.cleanUnitOn);
            dioService.SetOutport((int)EnumSmartIC.Outports.cleanUnitOff);
            return 0;
        }

        protected bool IsInPos(int axis, double pos, double tolerance = 0.01)
        {
            double realPos = motionService.GetCurrentPosition(axis);

            if (Math.Abs(pos - realPos) < tolerance)
                return true;

            return false;
        }

        protected int LastPunchMove()
        {
            double strokeLimit = dataService.DataRecipe.PunchStroke;
            int pf = dataService.DataRecipe.PF;
            double totalUnits = strokeLimit / ((double)pf * 4.75);
            int units = (int)totalUnits;
            double posStroke = (double)(units * pf) * 4.75;

            int axisVision = (int)EnumSmartIC.Axis.visionFeed;
            int axisPunch = (int)EnumSmartIC.Axis.punchFeed;
            int axisX = (int)EnumSmartIC.Axis.punchX;
            int axisY = (int)EnumSmartIC.Axis.punchY;
            int bufferW = (int)EnumSmartIC.Axis.buffer;

            double vel = dataService.DataMotion[axisVision].VelMove;
            double accel = dataService.DataMotion[axisVision].AccelMove;

            double velX = dataService.DataMotion[axisX].VelMove;
            double accelX = dataService.DataMotion[axisX].AccelMove;

            double velY = dataService.DataMotion[axisY].VelMove;
            double accelY = dataService.DataMotion[axisY].AccelMove;

            Log_Debug.WriteLine("LastPunchMove Start ");
            dataService.DataSystem.VisionCurrPos = motionService.GetCurrentPosition(axisVision);
            dataService.DataSystem.FeedCurrPos = motionService.GetCurrentPosition(axisPunch);
            dataService.DataSystem.BufferCurrPos = motionService.GetCurrentPosition(bufferW);
            dataService.DataSystem.AlignXCurrPos = motionService.GetCurrentPosition(axisX);
            dataService.DataSystem.AlignYCurrPos = motionService.GetCurrentPosition(axisY);

            // Motion Stop
            motionService.Stop(axisVision);
            motionService.Stop(axisPunch);

            motionService.AMove(axisVision, dataService.DataSystem.VisionLastPos, dataService.DataMotion[axisVision].VelMove, dataService.DataMotion[axisVision].AccelMove);
            motionService.AMove(axisPunch, dataService.DataSystem.FeedLastPos, dataService.DataMotion[axisVision].VelMove, dataService.DataMotion[axisVision].AccelMove);

            if (dataService.DataRecipe.PF == 2)
            {
                motionService.AMove(axisX, dataService.DataSystem.AlignXLastPos - dataService.DataSystem.LaserXpos - ((double)pf * 4.75), velX, accelX);
                Log_Debug.WriteLine("PF 2 LASER X POS {0:0.000}", dataService.DataSystem.AlignXLastPos - dataService.DataSystem.LaserXpos - ((double)pf * 4.75));
            }
            else if (dataService.DataRecipe.PF == 3)
            {
                motionService.AMove(axisX, dataService.DataSystem.AlignXLastPos - dataService.DataSystem.LaserXpos - ((double)pf * 4.75) + 5, velX, accelX);
                Log_Debug.WriteLine("PF 3 LASER X POS {0:0.000}", dataService.DataSystem.AlignXLastPos - dataService.DataSystem.LaserXpos - ((double)pf * 4.75) + 5);
            }
            motionService.AMove(axisY, dataService.DataSystem.LaserYpos, velY, accelY);
            SetTimeout(100);
            while (true)
            {
                if (true == IsTimeout())
                {
                    if (motionService.IsMotionDone(axisX) == true && motionService.IsMotionDone(axisY) == true &&
                    motionService.IsMotionDone(axisVision) == true && motionService.IsMotionDone(axisPunch) == true)
                    {
                        break;
                    }
                    else
                    {
                        SetTimeout(100);
                    }
                }
            }
            dioService.SetOutport((int)EnumSmartIC.Outports.laserOn);

            dataService.DataSystem.UseLaser = true;

            Log_Debug.WriteLine("LastPunchMove end ");

            return 0;
        }
    }
}
