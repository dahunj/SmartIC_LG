using System;
using System.Windows.Threading;

namespace SmartICAVI
{
    class VisionUtill
    {
        public enum Replys
        {
            connect = 0,    status,     model,      load,       scan,
            inspect,        review,     position,   amove,      rmove,
            movedone,       result,     lot,        strip,      error,
            lload,          laser,  
        }

        public enum Flags {none = 0,    scanDone,       inspectDone, }

        public delegate void ReceiveEventHandler(string message);
        public delegate void SendEventHandler(string message);
        public delegate void ConnectEventHandler(int connect);

        public event ReceiveEventHandler ReceiveEvent;
        public event SendEventHandler SendEvent;
        public event ConnectEventHandler ConnectEvent;

        public event EventHandler EventInitPos;

        protected string VisionType { get; set; }

        private bool IsAddData { get; set; }

        protected virtual void FireSendEvent(string message)
        {
            if (null != SendEvent)
                SendEvent(message);
        }
        protected virtual void FireReceiveEvent(string message)
        {
            if (null != ReceiveEvent)
                ReceiveEvent(message);
        }
        protected virtual void FireConnectEvent(int connect)
        {
            if (null != ConnectEvent)
                ConnectEvent(connect);
        }
        protected virtual void FireEventInitPos()
        {
            if (null != EventInitPos)
                EventInitPos(this, null);
        }

        protected Replys SendFuncs { get; set; }
        protected bool[] replys = new bool[32];

        protected bool[] flags = new bool[32];

        protected bool IsMoveComplete { get; set; }

        public double OffsetX { get; set; }
        public double OffsetY { get; set; }
        public int Result { get; set; }

        public string ModelName { get; set; }

        protected bool ThreadStart = false;

        protected SocketUDPServer server = null;
        protected SocketUDPClient client = null;

        protected DataService dataService = null;
        protected SystemService sysService = null;
        protected MotionService motionService = null;
        protected MsgService msgService = null;
        protected SequenceService seqService = null;

        protected VisionTopService topSerivce = null;
        protected VisionTop2Service top2Serivce = null;
        protected VisionBottomService bottomSerivce = null;
        protected VisionBottom2Service bottom2Serivce = null;
        protected VisionMonoService monoSerivce = null;
        protected VisionMono2Service mono2Serivce = null;
        

        protected bool isConnected = false;

        protected DispatcherTimer timer = null;

        protected string posX = "";
        protected string posY = "";
        protected string posZ = "";
        protected string posR = "";

        protected int countMotion = 0;

        public bool IsOpened
        {
            get
            {
                if (null != server)
                    return server.IsOpened;
                return false;
            }
        }

        public bool IsConnected
        {
            get
            {
                return isConnected;
            }
        }

        public bool IsInspectCompleted { get; set; }

        public bool IsLaserCompleted { get; set; }

        public bool IsModelCompleted { get; set; }

        public bool IsModelChangeSuccess { get; set; }

        protected bool CanMove
        {
            get
            {
                if (null != sysService)
                {
                    sysService = SystemService.Singleton;
                }

                switch (sysService.State)
                {
                    case SystemService.States.none:// = 0, 
                        return false;
                    case SystemService.States.ready:
                        break;
                    case SystemService.States.idle:
                        break;
                    case SystemService.States.run:
                        break;
                    case SystemService.States.pause:
                        return false;
                    case SystemService.States.resume:
                        break;
                    case SystemService.States.homing:
                        return false;
                    case SystemService.States.homeDone:
                        return false;
                    case SystemService.States.jobDone:
                        break;
                    case SystemService.States.systemLock:
                        return false;
                    case SystemService.States.systemRelease:
                        break;
                    case SystemService.States.reset:
                        break;
                    case SystemService.States.lightAlarm:
                        break;
                    case SystemService.States.stop:
                    case SystemService.States.heavyAlarm:
                    case SystemService.States.emg:
                        return false;
                }

                switch (sysService.Mode)
                {
                    case SystemService.Modes.none:// = 0, 
                        return false;
                    case SystemService.Modes.home:
                        return false;
                    case SystemService.Modes.auto:
                        
                        break;
                    case SystemService.Modes.recipe:
                    case SystemService.Modes.setup:
                    case SystemService.Modes.manual:
                    case SystemService.Modes.pm:
                        break;
                    case SystemService.Modes.syslock:
                        return false;
                    default:
                        break;
                }

                return true;
            }
        }

        public VisionUtill()
        {
            server = new SocketUDPServer();
            client = new SocketUDPClient();

            VisionType = "";

            IsAddData = false;
        }

        public int Initialize()
        {
            dataService = DataService.Singleton;
            sysService = SystemService.Singleton;
            motionService = MotionService.Singleton;
            msgService = MsgService.Singleton;
            seqService = SequenceService.Singleton;

            topSerivce = VisionTopService.Singleton;
            top2Serivce = VisionTop2Service.Singleton;
            bottomSerivce = VisionBottomService.Singleton;
            bottom2Serivce = VisionBottom2Service.Singleton;
            monoSerivce = VisionMonoService.Singleton;
            mono2Serivce = VisionMono2Service.Singleton;

            server.MessageEvent += OnMessageEvent;
            client.SendMessageEvent += OnSendMessageEvent;

            IsMoveComplete = false;
            IsInspectCompleted = false;
            IsLaserCompleted = false;

            Open();
            Connect();

            for (int i = 0; i < flags.Length; ++i)
                flags[i] = false;

            OffsetX = 0.0;
            OffsetY = 0.0;
            Result = 0;

            SetConnectRequest();

            return 0;
        }

        public void UnInitialize()
        {
            SetConnectEnd();


            if( null != server )
                server.MessageEvent -= OnMessageEvent;
            if( null != client )
                client.SendMessageEvent -= OnSendMessageEvent;

            Close();
            Disconnect();
        }

        protected virtual void OnMessageEvent(object sender, EventArgs e)
        {
            string message = server.ReadValue;
            string strNew;
            int startIndex = 0;
            int endIndex = 0;

            while (-1 != (startIndex = message.IndexOf("@", endIndex))) // 시작문자가 있을 경우
            {
                if (-1 == (endIndex = message.IndexOf("\n", startIndex)))   // 끝 문자가 없을 경우 종료
                    break;

                strNew = message.Substring(startIndex + 1, endIndex - startIndex - 1);
                ReadProc(strNew);

                startIndex = endIndex + 1;
            }
        }

        protected virtual void OnSendMessageEvent(object sender, EventArgs e)
        {
            FireSendEvent(client.SendMessage);
        }

        protected virtual void OnMotionDoneEvent(int axis, bool done)
        {
        }

        public virtual void Open()
        {
            
        }

        public virtual void Close()
        {
            Log_Trace.WriteLine("UDP Service server close");
            server.Close();
        }

        public virtual void Connect()
        {
            
        }

        public virtual void Disconnect()
        {
            Log_Trace.WriteLine("UDP Service client disconnect");
            client.Disconnect();
        }

        public virtual void Read(out string read)
        {
            read = server.ReadValue;
        }

        public virtual void Write(string message)
        {
            WriteLog("<Send> " + message);
            client.Write("@" + message + "\n");
        }

        #region Flags
        protected virtual void SetReply(int reply)
        {
            replys[reply] = true;
        }

        protected virtual void ResetReply(int reply)
        {
            replys[reply] = false;
        }

        public virtual bool IsReply(int reply)
        {
            return replys[reply];
        }

        protected virtual void SetFlag(int flag)
        {
            flags[flag] = true;
        }

        protected virtual void ResetFlag(int flag)
        {
            flags[flag] = false;
        }

        public virtual bool IsFlag(int flag)
        {
            return flags[flag];
        }
        #endregion

        #region Read
        protected virtual void ReadProc(string message)
        {
            try
            {
                string[] arrays = new string[message.Length];

                arrays = message.Split(',');

                isConnected = true;

                WriteLog("[RECEIVE] " + message);

                switch (arrays[0])
                {
                    case "CONNECT":
                        ReadProc_Connect(arrays);
                        break;
                    case "GRAB":
                        ReadProc_Grab(arrays);
                        break;
                    case "INITPOS":
                        ReadProc_InitPos(arrays);
                        break;
                    case "INSPECT":
                        ReadProc_Inspect(arrays);
                        break;
                    case "LIGHT":
                        ReadProc_Light(arrays);
                        break;
                    case "LOAD":
                        ReadProc_Load(arrays);
                        break;
                    case "MODE":
                        ReadProc_Mode(arrays);
                        break;
                    case "OVERFRAME":
                        ReadProc_OverFrame(arrays);
                        break;
                    case "RECIPE":
                        ReadProc_Recipe(arrays);
                        break;
                    case "RESULT":
                        if( 3 == dataService.DataRecipe.Line )
                            ReadProc_Result3(arrays);
                        else
                            ReadProc_Result2(arrays);
                        break;
                    case "SCAN":
                        ReadProc_Scan(arrays);
                        break;
                    case "STATUS":
                        ReadProc_Status(arrays);
                        break;
                    default:
                        break;
                }
                
                FireReceiveEvent(message);
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine("VisionUtill : " + exc.Message);
            }
        }

        protected virtual void ReadProc_Connect(string[] messages)
        {
            switch (messages[1])
            {
                case "REQUEST":
                    Write("CONNECT,REPLY");
                    isConnected = true;
                    FireConnectEvent(1);
                    SetLightRequest();
                    break;

                case "REPLY":
                    isConnected = true;
                    SetReply((int)EnumSmartIC.VisionReplys.connect);
                    SetFlag((int)EnumSmartIC.VisionFlags.connect);
                    FireConnectEvent(1);
                    SetLightRequest();
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

        protected virtual void ReadProc_Status(string[] messages)
        {
            switch (messages[1])
            {
                case "REQUEST":
                    if( SystemService.States.systemLock > sysService.State )
                        Write("STATUS,REPLY,READY");
                    else
                        Write("STATUS,REPLY,STOP");
                    break;
                case "REPLY":
                    SetReply((int)EnumSmartIC.VisionReplys.status);
                    if (2 < messages.Length)
                    {
                        if ("READY" == messages[2])
                            SetFlag((int)EnumSmartIC.VisionFlags.status);
                        else
                            ResetFlag((int)EnumSmartIC.VisionFlags.status);
                    }
                    break;
                case "UPDATE":
                    if (2 < messages.Length)
                    {
                        if ("READY" == messages[2])
                            SetFlag((int)EnumSmartIC.VisionFlags.status);
                        else
                            ResetFlag((int)EnumSmartIC.VisionFlags.status);
                    }
                    break;
            }
        }

        protected virtual void ReadProc_Mode(string[] messages)
        {
            switch (messages[1])
            {
                case "REQUEST":
                    if (SystemService.Modes.recipe == sysService.Mode)
                        Write("MODE,REPLY,TEACH");
                    else if (SystemService.Modes.auto == sysService.Mode)
                        Write("MODE,REPLY,INSPECT");
                    else
                        Write("MODE,REPLY,NONE");
                    break;
                case "REPLY":
                    SetReply((int)EnumSmartIC.VisionReplys.mode);
                    if (2 < messages.Length)
                    {
                        if ("TEACH" == messages[2])
                        {
                            SetFlag((int)EnumSmartIC.VisionFlags.modeTeach);
                            ResetFlag((int)EnumSmartIC.VisionFlags.modeInspect);
                        }
                        else if ("INSPECT" == messages[2])
                        {
                            ResetFlag((int)EnumSmartIC.VisionFlags.modeTeach);
                            SetFlag((int)EnumSmartIC.VisionFlags.modeInspect);
                        }
                    }
                    break;
                case "UPDATE":
                    if (2 < messages.Length)
                    {
                        if ("TEACH" == messages[2])
                        {
                            SetFlag((int)EnumSmartIC.VisionFlags.modeTeach);
                            ResetFlag((int)EnumSmartIC.VisionFlags.modeInspect);
                        }
                        else if ("INSPECT" == messages[2])
                        {
                            ResetFlag((int)EnumSmartIC.VisionFlags.modeTeach);
                            SetFlag((int)EnumSmartIC.VisionFlags.modeInspect);
                        }
                    }
                    break;
            }
        }

        protected virtual void ReadProc_Recipe(string[] messages)
        {
            switch (messages[1])
            {
                case "REPLY":
                    SetReply((int)EnumSmartIC.VisionReplys.recipe);
                    if (2 < messages.Length)
                    {
                        if ("ACK" == messages[2])
                        {
                            SetFlag((int)EnumSmartIC.VisionFlags.recipe);
                        }
                        else
                        {
                            ResetFlag((int)EnumSmartIC.VisionFlags.recipe);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        protected virtual void ReadProc_Load(string[] messages)
        {
            if ("REPLY" == messages[1])
            {
                SetReply((int)EnumSmartIC.VisionReplys.load);
            }
        }

        protected virtual void ReadProc_Scan(string[] messages)
        {
            if ("REPLY" == messages[1])
            {
                SetReply((int)EnumSmartIC.VisionReplys.scan);
            }
        }

        protected virtual void ReadProc_Result(string[] messages)
        {
            if ("UPDATE" == messages[1])
            {
                Write("RESULT,REPLY");

                if (4 < messages.Length)
                {
                    int index = int.Parse(messages[2]);
                    string value1 = messages[3];
                    string value2 = messages[4];

                    AddResultValues(index, value1, value2, "");
                }
            }
        }

        protected virtual void ReadProc_Result2(string[] messages)
        {
            if ("UPDATE" == messages[1])
            {
                Write("RESULT,REPLY");

                //RESULT,UPDATE,INDEX,LENGTH,VALUE1,VALUE2,.....VALUE1n,VALUE2n
                // 20160714 LIGHT 포함.
                //RESULT,UPDATE,INDEX,LIGHT,LENGTH,VALUE1,VALUE2,.....VALUE1n,VALUE2n
                //while (true == IsAddData)
                //    System.Windows.Forms.Application.DoEvents();

                //Monitor.Enter(this);
                //IsAddData = true;
                if (4 < messages.Length)
                {
                    int index = int.Parse(messages[2]);
                    int light = int.Parse(messages[3]);
                    int length = int.Parse(messages[4]);
                    int start = 5;

                    string value1;
                    string value2;

                    if ((true == dataService.IsEndTop) && (index > dataService.IndexEndTop))
                    {
                        return;
                    }

                    if (0 == CheckIndex(index, length, messages))
                    {
                        if (start + length * 3 <= messages.Length)
                        {
                            ShowLineError();
                        }
                        else if (start + length * 2 <= messages.Length)
                        {
                            for (int i = 0; i < length; ++i)
                            {
                                start = 5 + i * 2;
                                value1 = messages[start];
                                value2 = messages[start + 1];

                                AddResultValues(index + i, value1, value2, VisionType);
                            }
                        }
                        else
                        {
                            ShowLineError();
                        }
                    }
                    AddTempValues();
                    AddLightValue(index, light);
                }
            }
        }

        protected virtual void ReadProc_Result3(string[] messages)
        {
            if ("UPDATE" == messages[1])
            {
                Write("RESULT,REPLY");

                //RESULT,UPDATE,INDEX,LENGTH,VALUE1,VALUE2,.....VALUE1n,VALUE2n
                // 20160714 LIGHT 포함.
                //RESULT,UPDATE,INDEX,LIGHT,LENGTH,VALUE1,VALUE2,.....VALUE1n,VALUE2n
                //while (true == IsAddData)
                //    System.Windows.Forms.Application.DoEvents();

                //Monitor.Enter(this);
                //IsAddData = true;
                if (4 < messages.Length)
                {
                    int index = int.Parse(messages[2]);
                    int light = int.Parse(messages[3]);
                    int length = int.Parse(messages[4]);
                    int start = 5;

                    string value1 = "";
                    string value2 = "";
                    string value3 = "";

                    if ((true == dataService.IsEndTop) && (index > dataService.IndexEndTop))
                    {
                        return;
                    }

                    if (0 == CheckIndex3(index, length, messages))
                    {
                        if (start + length * 3 <= messages.Length)
                        {
                            for (int i = 0; i < length; ++i)
                            {
                                start = 5 + i * 3;
                                value1 = messages[start];
                                value2 = messages[start + 1];
                                value3 = messages[start + 2];

                                AddResultValues3(index + i, value1, value2, value3, VisionType);
                            }
                        }
                        else
                        {
                            ShowLineError();
                        }
                    }
                    AddTempValues3();
                    AddLightValue(index, light);
                }
            }
        }

        protected virtual void ReadProc_OverFrame(string[] messages)
        {
            if ("UPDATE" == messages[1])
            {
                Write("OVERFRAME,REPLY");

                SetOverFrame();
            }
        }

        protected virtual void ReadProc_InitPos(string[] messages)
        {
            if ("UPDATE" == messages[1])
            {
                Write("INITPOS,REPLY");
            }
        }

        protected virtual void ReadProc_Light(string[] messages)
        {
        }

        protected virtual void ReadProc_Grab(string[] messages)
        {
            if ("COMPLETE" == messages[1])
            {
                SetFlag((int)EnumSmartIC.VisionFlags.grab);
            }
        }

        protected virtual void ReadProc_Inspect(string[] messages)
        {
            if ("COMPLETE" == messages[1])
            {
                SetFlag((int)EnumSmartIC.VisionFlags.inspect);
            }
        }


        protected virtual int CheckIndex(int index, int length, string[] messages)
        {
            return 0;
        }

        protected virtual int CheckIndex3(int index, int length, string[] messages)
        {
            return 0;
        }

        protected virtual void AddResultValues(int index, string value1, string value2, string vision="")
        {

        }

        protected virtual void AddResultValues3(int index, string value1, string value2, string value3, string vision = "")
        {

        }

        protected virtual void AddTempValues()
        {
        }

        protected virtual void AddTempValues3()
        {
        }

        protected virtual void AddLightValue(int index, int value)
        {
        }

        #endregion

        #region Send
        public virtual void SetConnectRequest()
        {
            ResetReply((int)EnumSmartIC.VisionReplys.connect);
            Write("CONNECT,REQUEST");
        }
        public virtual void SetConnectEnd()
        {
            Write("CONNECT,END");
        }

        public virtual void SetStatusRequest()
        {
            ResetReply((int)EnumSmartIC.VisionReplys.status);
            Write("STATUS,REQUEST");
        }
        public virtual void SetStatusUpdate()
        {
            ResetReply((int)EnumSmartIC.VisionReplys.status);

            if (SystemService.States.stop > sysService.State)
                Write("STATUS,UPDATE,READY");
            else
                Write("STATUS,UPDATE,STOP");

        }

        public virtual void SetMode()
        {
            ResetReply((int)EnumSmartIC.VisionReplys.mode);

            if (SystemService.Modes.auto == sysService.Mode)
                Write("MODE,UPDATE,INSPECT");
            else
                Write("MODE,UPDATE,TEACH");
        }

        public virtual void SetRecipe(string recipeName)
        {
            ResetReply((int)EnumSmartIC.VisionReplys.recipe);

            Write("RECIPE,UPDATE," + recipeName);
        }

        public virtual void SetLoad(string lotID, string userID, string pfHole)
        {
            ResetReply((int)EnumSmartIC.VisionReplys.load);

            Write("LOAD,COMPLETE," + lotID + "," + userID + "," + pfHole);
        }

        public virtual void SetScanStart(int index = 0)
        {
            ResetReply((int)EnumSmartIC.VisionReplys.scan);
            
            Write(string.Format("SCAN,START,{0}", index));
        }

        public virtual void SetScanStop()
        {
            Write("STATUS,UPDATE,STOP");
        }

        public virtual void SetScanInit()
        {
            ResetReply((int)EnumSmartIC.VisionReplys.scan);
            ResetFlag((int)EnumSmartIC.VisionFlags.initPos);
            Write("SCAN,INIT");
        }

        public virtual void SetLightRequest()
        {
            ResetReply((int)EnumSmartIC.VisionReplys.light);
            Write("LIGHT,REQUEST");
        }
        #endregion

        protected virtual void WriteLog(string log)
        {
        }

        protected virtual void SetOverFrame()
        {
        }

        protected virtual void ShowLineError()
        {

        }
    }
}
