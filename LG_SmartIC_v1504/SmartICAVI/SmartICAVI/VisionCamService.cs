using System;
using System.Threading;

namespace SmartICAVI
{
    class VisionCamService : VisionUtill
    {
        //public double OffsetX { get; set; }
        //public double OffsetY { get; set; }

        #region Singletons
        private static VisionCamService singleton = null;

        public static VisionCamService Singleton
        {
            get
            {
                if (null == singleton)
                {
                    singleton = new VisionCamService();
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


        public override void Open()
        {
            VisionType = "CAM";
            if (null != dataService)
            {
                Log_Trace.WriteLine(string.Format("Cam Vision UDP Service server open Port:{0}", dataService.DataSystem.CamHPort));
                if (0 != server.Open(dataService.DataSystem.CamHPort))
                {
                    // Error 
                    Log_Trace.WriteLine("Cam Vision UDP Service server open failed");
                }
            }
        }

        public override void Connect()
        {
            if (null != dataService)
            {
                Log_Trace.WriteLine(string.Format("Cam Vision UDP Service client connect IP:{0}, Port:{1}", dataService.DataSystem.CamIPAddress, dataService.DataSystem.CamPort));

                if (0 != client.Connect(dataService.DataSystem.CamIPAddress, dataService.DataSystem.CamPort))
                {
                    // Error
                    Log_Trace.WriteLine("Cam Vision UDP Service client connect failed");
                }
            }
        }

        public override void SetConnectEnd()
        {
            Write("CONNECT,END");

            SendMode("CLOSE");
        }

        protected override void ReadProc_Connect(string[] messages)
        {
            switch (messages[1])
            {
                case "REQUEST":
                    Write("CONNECT,REPLY");
                    SendInitParams();
                    FireConnectEvent(1);
                    isConnected = true;
                    break;
                case "REPLY":
                    SendInitParams();
                    SetReply((int)EnumSmartIC.VisionReplys.connect);
                    SetFlag((int)EnumSmartIC.VisionFlags.connect);
                    FireConnectEvent(1);
                    isConnected = true;
                    break;
                case "END":
                    isConnected = false;
                    ResetFlag((int)EnumSmartIC.VisionFlags.connect);
                    FireConnectEvent(0);
                    break;
                default:
                    break;
            }
        }

        protected override void ReadProc_Inspect(string[] messages)
        {
            if ("COMPLETE" == messages[1])
            {
                try
                {
                    OffsetX = double.Parse(messages[2]);
                    OffsetY = double.Parse(messages[3]);

                    if (4 < messages.Length)
                        Result = int.Parse(messages[4]);
                    else
                        Result = 0;

                    SetFlag((int)EnumSmartIC.VisionFlags.inspect);
                }
                catch (Exception exc)
                {
                    Log_Exception.WriteLine("VisionCamService.ReadProc_Inspect() : " + exc.Message);
                }
            }
        }

        protected override void ReadProc_Light(string[] messages)
        {
            if ("UPDATE" == messages[1])
            {
                int value = int.Parse(messages[2]);
                dataService.DataTeach.strobe1 = value;
            }
        }

        protected override void WriteLog(string log)
        {
            Log_Cam.WriteLine(log);
        }


        public int SendInitParams()
        {
            if (null == sysService)
                return -1;

            if (null == dataService)
                return -1;

            // Send Mode
            switch (sysService.Mode)
            {
                case SystemService.Modes.none:// = 0, 
                case SystemService.Modes.home:
                    SendMode("MANUAL");
                    break;
                case SystemService.Modes.auto:
                    SendMode("INSPECT");
                    break;
                case SystemService.Modes.recipe:
                    //SendLoad(dataService.DataSystem.RecipeName);
                    //DoEvents(50);
                    SendMode("TEACH");
                    break;
                case SystemService.Modes.setup:
                case SystemService.Modes.manual:
                case SystemService.Modes.pm:
                case SystemService.Modes.syslock:
                    SendMode("MANUAL");
                    break;
                default:
                    break;
            }

            // Send Current Model File
            DoEvents(10);
            SendLoad(dataService.DataSystem.RecipeName);

            return 0;
        }

        public int SendMode(string mode, int value = 1)
        {
            if (null == sysService)
                return -1;

            switch (mode)
            {
                case "CLOSE":
                    Write("MODE,UPDATE,CLOSE,0,0,0,0");
                    isConnected = false;
                    break;
                case "INSPECT":
                    Write("MODE,UPDATE,INSPECT,10,95,480,320");
                    break;
                case "MANUAL":
                    Write("MODE,UPDATE,MANUAL,0,0,0,0");
                    break;
                case "TEACH":
                    Write("MODE,UPDATE,TEACH,20,115,945,640");
                    break;
                case "TOPMOST":
                    if (0 == value)
                        Write("MODE,UPDATE,TOPMOST,0,0,0,0");
                    else
                        Write("MODE,UPDATE,TOPMOST,1,1,1,1");
                    break;
                default:
                    Write("MODE,UPDATE,MANUAL,0,0,0,0");
                    break;
            }
            return 0;
        }

        public int SendLoad(string name)
        {
            string modelPathName = dataService.RecipePath + "\\" + name + ".rcp";

            Write("RECIPE,UPDATE," + modelPathName);

            return 0;
        }

        public int SendSave(string pathName)
        {
            Write("RECIPE,SAVE," + pathName);

            return 0;
        }

        public int SendGrab()
        {
            ResetReply((int)EnumSmartIC.VisionReplys.grab);
            ResetFlag((int)EnumSmartIC.VisionFlags.grab);

            Write("GRAB,REQUEST");

            return 0;
        }

        public int SendInspect(string path)
        {
            ResetReply((int)EnumSmartIC.VisionReplys.inspect);
            ResetFlag((int)EnumSmartIC.VisionFlags.inspect);

            Result = 0;

            Write("INSPECT,REQUEST," + path);

            return 0;
        }

        public int SendInspectCircle(string path)
        {
            ResetReply((int)EnumSmartIC.VisionReplys.inspect);
            ResetFlag((int)EnumSmartIC.VisionFlags.inspect);

            Result = 0;

            Write("INSPECT,CIRCLE,"+path);

            return 0;
        }

        public int SaveImage(string path)
        {
            Write("SAVE," + path);

            return 0;
        }

        public int SendSearchArea(double area = 4.0)
        {
            Write("SEARCHAREA,UPDATE," + area.ToString("0.000"));
            return 0;
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

    }
}
