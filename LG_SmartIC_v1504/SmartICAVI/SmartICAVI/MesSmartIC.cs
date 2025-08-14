using System;
using System.Windows.Threading;

namespace SmartICAVI
{
    class MesSmartIC : IMes
    {
        private TsMesForm form = null;
        private AxaxTibcoRv.AxucTibcoRv tibco = null;
        private DispatcherTimer timer = null;
        private DataService dataService = null;
        private SystemService sysService = null;
        private SequenceService seqService = null;

        public bool IsInitialized { get; set; }
        private bool IsSend { get; set; }

        #region Interfaces
        public string MachineID { get; set; }
        public string SendID { get; set; }
        //public DateTime DateTime { get; set; }
        public string ProcessType { get; set; }
        public string LotID { get; set; }
        public string OPID { get; set; }
        public string ToolID1 { get; set; }
        public string ToolID2 { get; set; }

        public int LotUnits { get; set; }
        public bool IsLot { get; set; }
        public int ToolID1Count { get; set; }
        public int ToolID2Count { get; set; }

        public int GoodUnits { get; set; }
        public int NGUnits { get; set; }
        public int LossUnits { get; set; }

        public int LotUnits1 { get; set; }
        public int GoodUnits1 { get; set; }
        public int NGUnits1 { get; set; }
        public int LossUnits1 { get; set; }

        public int LotUnits2 { get; set; }
        public int GoodUnits2 { get; set; }
        public int NGUnits2 { get; set; }
        public int LossUnits2 { get; set; }

        public int LotUnits3 { get; set; }
        public int GoodUnits3 { get; set; }
        public int NGUnits3 { get; set; }
        public int LossUnits3 { get; set; }

        public string NextOper { get; set; }
        public string MapData1 { get; set; }
        public string MapData2 { get; set; }
        public string MapData3 { get; set; }

        public string Comment { get; set; }

        public int MachineStatus { get; set; }

        public bool IsReply { get; set; }

        public bool IsJobPermission { get; set; }
        public string JobPermission { get; set; }

        public int TOK { get; set; }
        public int TNG { get; set; }
        public double YIELD { get; set; }
        public int TCNG { get; set; }
        public int TJCNT { get; set; }
        public string TJPOINT { get; set; }
        public int UNITPITCH { get; set; }

        public bool IsConnected
        {
            get
            {
                try
                {
                    if (null != tibco)
                    {
                        if ((true == tibco.PubTransOpen) && (true == tibco.TransOpen))
                            return true;
                    }
                }
                catch (Exception exc)
                {
                    Log_Exception.WriteLine("MesSmartIC.IsConnected : " + exc.Message);
                }

                return false;
            }
        }

        public Object Control { get { return (Object)tibco; } }
                
        public int[] CountNGs;
        public string[] CountNGIDs;
            
        private string ReceiveName { get; set; }
        private string ReceiveID { get; set; }
        private string ReceiveBody { get; set; }

        public int Initialize()
        {
            dataService = DataService.Singleton;
            sysService = SystemService.Singleton;
            seqService = SequenceService.Singleton;

            IsSend = false;
            IsInitialized = false;
            //tibco = new AxaxTibcoRv.AxucTibcoRv();
            //tibco.MessageReceived += new AxaxTibcoRv.__ucTibcoRv_MessageReceivedEventHandler(tibco_MessageReceived);
            //tibco.TibRvError += new AxaxTibcoRv.__ucTibcoRv_TibRvErrorEventHandler(tibco_TibRvError);

            //sysTime = new SysTime();

            ReceiveName = "";
            ReceiveID = "";
            ReceiveBody = "";

            

            IsJobPermission = false;
            JobPermission = "";

            form = new TsMesForm();
            //form.Width = 0;
            //form.Height = 0;
            SetParams();
            form.Left = -1000;      // 화면에 보이지 않도록 
            form.Show();
            form.Visible = false;
            tibco = form.Tibco;

            IsInitialized = true;

            timer = new DispatcherTimer();
            timer.Interval = new System.TimeSpan(0, 0, 0, 0, 50);
            timer.IsEnabled = true;
            timer.Tick += new EventHandler(timer_Tick);

            // 시간 동기화
            RequestTime();

            sysService.EventState += OnEventState;

            OnEventState(this, null);
            return 0;
        }

        public int UnInitialize()
        {
            if (true == IsInitialized)
            {
                tibco.MessageReceived -= tibco_MessageReceived;
                tibco.TibRvError -= tibco_TibRvError;

                timer.IsEnabled = false;
                timer.Tick -= timer_Tick;

                sysService.EventState -= OnEventState;
            }

            if (null != form)
                form.Close();

            IsInitialized = false;

            return 0;
        }

        public int Send(object command, int value = 0, string strValue = "")
        {
            switch ((MesService.commands)command)
            {
                case MesService.commands.req_time://=0, 
                    return RequestTime();
                case MesService.commands.send_jobstart:
                    return SendJobStart();
                case MesService.commands.req_lot:
                    return RequestLot();
                case MesService.commands.send_jobend:
                    return SendJobEnd();
                case MesService.commands.send_status:
                    return SendStatus();
                case MesService.commands.send_alarm:
                    return SendAlarm(value, strValue);
                default:
                    break;
            }

            return 0;
        }

        public int Open()
        {
            if (null == tibco)
                return -1;

            tibco.TibRvOpen();

            return 0;
        }

        public int Close()
        {
            if (null == tibco)
                return -1;

            tibco.TibRvClose();

            return 0;
        }

        public int SetParams()
        {
            if (null == dataService)
                return -1;
            
            if (null == form)
                return -1;

            MachineID = dataService.DataSystem.MachineID;
            ProcessType = dataService.DataSystem.ProcessID;
                
            form.ServiceName = dataService.DataSystem.Service;
            form.NetworkName = dataService.DataSystem.Network;
            form.DaemonName = dataService.DataSystem.Daemon;

            form.PubSubject = dataService.DataSystem.PubSubject;
            form.PubTimeOut = dataService.DataSystem.PubTimeout;
            form.PubRvType = dataService.DataSystem.PubRvType;

            form.SubSubject = dataService.DataSystem.SubSubject;
            form.SubRvType = dataService.DataSystem.SubRvType;

            form.SetParams();

            return 0;
        }

        public int SetCountNGs(int[] countNGs)
        {
            if (CountNGs.Length != countNGs.Length)
                return -1;

            for (int i = 0; i < countNGs.Length; ++i)
                CountNGs[i] = countNGs[i];

            return 0;
        }
        #endregion

        public MesSmartIC()
        {
            IsInitialized = false;

            MachineID = "TSFN402";
            ProcessType = "FN";

            SendID = "1";

            LotID = "";
            OPID = "";
            ToolID1 = "";
            ToolID2 = "";

            LotUnits = 0;
            IsLot = false;
            ToolID1Count = 0;
            ToolID2Count = 0;

            GoodUnits = 0;
            NGUnits = 0;
            LossUnits = 0;

            NextOper = "TSET00";
            MapData1 = "";
            MapData2 = "";

            Comment = "";

            ReceiveName = "";
            ReceiveID = "";
            ReceiveBody = "";

            IsJobPermission = false;
            JobPermission = "";

            MachineStatus = -1;

            IsReply = false;

            CountNGs = new int[63];

            for (int i = 0; i < CountNGs.Length; ++i)
                CountNGs[i] = 0;

            CountNGIDs = new string[63]{
                        "1",        "2",        "3",        "4",        "5",
                        "A",        "B",        "BB001",    "BB006",    "BB012",

                        "BB018",    "BB019",    "BB025",    "BB026",    "BB029",
                        "BB034",    "BB038",    "BB039",    "BB040",    "BB042",

                        "BB043",    "BB045",    "BB047",    "BB053",    "BB062",
                        "BB064",    "BB066",    "BB068",    "BB072",    "BB074",

                        "BB088",    "BB089",    "BB090",    "BB091",    "BB092",
                        "BB093",    "BB094",    "BB095",    "BB096",    "BB097",

                        "BB098",    "BB099",    "BB100",    "C",        "D",
                        "E",        "F",        "H",        "J",        "K",

                        "L",        "M",        "N",        "O",        "P",    
                        "Q",        "R",        "S",        "T",        "U",

                        "V",        "W",        "Y",    
            };
        }

        private void tibco_MessageReceived(object sender, AxaxTibcoRv.__ucTibcoRv_MessageReceivedEvent e)
        {
            string name = e.receiveName;
            string id = e.receiveID;
            string body = e.receiveBody;

            //System.Diagnostics.Debug.WriteLine("Name=" + name);
            //System.Diagnostics.Debug.WriteLine("ID=" + id);
            //System.Diagnostics.Debug.WriteLine("Body=" + body);

            switch (name)
            {
                case "S2F18":   // 시간동기화 Reply Data를 Equipment에 전송.
                    ReceiveTime(body);
                    break;

                case "S7F38":   // 수량COUNT 착공LOT의 Unit수량을 Equipment에 전송
                    ReceiveLotInfo(body);
                    break;

                default:
                    break;
            }
        }

        void tibco_TibRvError(object sender, AxaxTibcoRv.__ucTibcoRv_TibRvErrorEvent e)
        {
            //throw new NotImplementedException();
            //System.Diagnostics.Debug.WriteLine("Error=" + e.sErrorDesc);
            Log_Trace.WriteLine("[TsMes] " + e.sErrorDesc);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            //if ((true == IsInitialized) && (true == IsSend))
            if (true == IsInitialized)
            {
                if (ReceiveBody != tibco.PubReceiveBody)
                {
                    ReceiveBody = tibco.PubReceiveBody;

                    switch (tibco.PubReceiveName)
                    {
                        case "S2F18":   // 시간동기화 Reply Data를 Equipment에 전송.
                            ReceiveTime(ReceiveBody);
                            break;
                        case "S7F38":   // SmartIC 수량COUNT 착공LOT의 Unit수량을 Equipment에 전송
                            ReceiveLotInfo(ReceiveBody);
                            break;
                        default:
                            break;
                    }

                    //if (null != seqService)
                    //{
                    //    // OffLine
                    //    if (false == seqService.IsOnlineMode)
                    //    {
                    //        Log_Mes.WriteLine("[OFFLINE][RECEIVE] " + tibco.PubReceiveName + ", " + tibco.PubReceiveID + ", " + ReceiveBody);
                    //        return;
                    //    }
                    //}

                    Log_Mes.WriteLine("[RECEIVE] " + tibco.PubReceiveName + ", " + tibco.PubReceiveID + ", " + ReceiveBody);

                    //System.Diagnostics.Debug.WriteLine(ReceiveBody);
                }
            }
        }

        private int RequestTime()
        {
            if (null == tibco)
                return -1;

            string sendBody = "MID=" + MachineID + "&";

            //IsSend = true;
            ReceiveBody = "";

            if (null != seqService)
            {
                // OffLine
                if (false == seqService.IsOnlineMode)
                {
                    Log_Mes.WriteLine("[OFFLINE][SEND] " + "S2F17, " + SendID + ", " + sendBody);
                    return 0;
                }
            }

            Log_Mes.WriteLine("[SEND] " + "S2F17, " + SendID + ", " + sendBody);

            if (true == tibco.PublishSend("S2F17", SendID, sendBody))
                return 0;

            return -1;
        }

        private int RequestLot()
        {
            return 0;
            ////MID=TSAV401&PRTY=&ITM=20070315201444&LOTID=DY73838BA&
            //if (null == tibco)
            //    return -1;

            //// OffLine
            ////if (false == seqService.IsOnlineMode)
            ////    return 0;

            //string machineID = "MID=" + MachineID + "&";
            //string processType = "PRTY=" + ProcessType + "&";
            //string transactionTime = "ITM=" + DateTime.Now.ToString("yyyyMMddHHmmss") + "&";
            //string lotID = "LOTID=" + LotID + "&";

            //string sendBody = machineID + processType + transactionTime + lotID;

            //ReceiveBody = "";

            //if (null != seqService)
            //{
            //    // OffLine
            //    if (false == seqService.IsOnlineMode)
            //    {
            //        Log_Mes.WriteLine("[OFFLINE][SEND] " + "S6F13, " + "1, " + sendBody);
            //        return 0;
            //    }
            //}

            //Log_Mes.WriteLine("[SEND] " + "S6F13, " + "1, " + sendBody);

            //if (true == tibco.PublishSend("S6F13", "1", sendBody))
            //    return 0;

            //return -1;
        }

        private int SendJobStart()
        {
            //MID=TSSF401&PRTY=FN&ITM=20090330202638&LOTID=DY73B16CB&OPE=D0371&TID1=&TID2=&

            if (null == tibco)
                return -1;

            string machineID = "MID=" + MachineID + "&";
            string processType = "PRTY=" + ProcessType + "&";
            string transactionTime = "ITM=" + DateTime.Now.ToString("yyyyMMddHHmmss") + "&";
            string lotID = "LOTID=" + LotID + "&";
            string opID = "OPE=" + OPID + "&";
            string toolID1 = "TID1=" + ToolID1 + "&";
            string toolID2 = "TID2=" + ToolID2 + "&";

            string sendBody = machineID + processType + transactionTime + lotID + opID + toolID1 + toolID2;

            string sfun = "S7F37";

            TOK = 0;
            TNG = 0;
            YIELD = 0.0;
            TCNG = 0;
            TJCNT = 0;
            TJPOINT = "";
            UNITPITCH = 0;

            //if (0 != dataService.DataRecipe.ProductType)
            //    sfun = "S7F37";

            IsJobPermission = false;
            JobPermission = "";
            ReceiveBody = "";

            if (null != seqService)
            {
                // OffLine
                if (false == seqService.IsOnlineMode)
                {
                    // OffLine 일 경우 무조건 작업을 진행한다. 
                    IsJobPermission = true;
                    JobPermission = "Y";
                    Log_Mes.WriteLine("[OFFLINE][SEND] " + sfun + ", " + "1, " + sendBody);
                    return 0;
                }
            }

            Log_Mes.WriteLine("[SEND] " + sfun + ", " + "1, " + sendBody);

            if (true == tibco.PublishSend(sfun, "1", sendBody))
                return 0;

            return -1;
        }

        private int SendJobEnd()
        {
            // 1 Machine ID
            string machineID = "MID=" + MachineID + "&";
            // 2 Transaction Time
            string transactionTime = "ITM=" + DateTime.Now.ToString("yyyyMMddHHmmss") + "&";
            // 3 Process Type
            string processType = "PRTY=" + ProcessType + "&";
            // 4 Lot ID
            string lotID = "LOTID=" + LotID + "&";
            // 5 OP ID
            string opID = "OPE=" + OPID + "&";
            // 6 Tool ID1
            string toolID1 = "TID1=" + ToolID1 + "&";
            // 7 Tool ID1 Count
            string toolID1Count = "TCNT1=" + ToolID1Count.ToString() + "&";
            // 8 Tool ID2
            string toolID2 = "TID2=" + ToolID2 + "&";
            // 9. Tool ID2 Count
            string toolID2Count = "TCNT2=" + "&";
            // 10. Lot 검사 Unit 수
            string lotCount = "TINS=" + LotUnits.ToString() + "&";
            // 11. lOt 검사 양품 수
            string goodCount = "TOK=" + GoodUnits.ToString() + "&";
            // 12. Lot 검사 불량 수
            string ngCount = "TNG=" + NGUnits.ToString() + "&";

            // 13. Total Count of Judge Code
            string totalJudgeCode = "TJCD=" + "63&";

            string JCDs = "";
            for (int i = 0; i < CountNGs.Length; ++i)
            {
                JCDs += string.Format("NJCD={0}&JCD={1}&NJTY={2}&", i + 1, CountNGIDs[i], CountNGs[i]);
            }
            // NG1
            //JCDs += "NJCD=1&JCD=1&NJTY=" + CountNG1.ToString() + "&";      // Scratch(회로)       
            //JCDs += "NJCD=2&JCD=2&NJTY=" + CountNG2.ToString() + "&";      // 얼룩
            //JCDs += "NJCD=3&JCD=3&NJTY=" + CountNG3.ToString() + "&";      // 이물(PI)
            //JCDs += "NJCD=4&JCD=4&NJTY=" + CountNG4.ToString() + "&";      // Scratch(PI)
            //JCDs += "NJCD=5&JCD=5&NJTY=" + CountNG5.ToString() + "&";      // TOP 패임
            //JCDs += "NJCD=6&JCD=A&NJTY=" + CountNGA.ToString() + "&";      // 비금속 이물
            //JCDs += "NJCD=7&JCD=B&NJTY=" + CountNGB.ToString() + "&";      // SR B/O
            //JCDs += "NJCD=8&JCD=C&NJTY=" + CountNGC.ToString() + "&";      // 전공정(AOI Punching)불량
            //JCDs += "NJCD=9&JCD=D&NJTY=" + CountNGD.ToString() + "&";      // Dent
            //JCDs += "NJCD=10&JCD=E&NJTY=" + CountNGE.ToString() + "&";     // 금속이물
            //JCDs += "NJCD=11&JCD=F&NJTY=" + CountNGF.ToString() + "&";     // 변색
            //JCDs += "NJCD=12&JCD=H&NJTY=" + CountNGH.ToString() + "&";     // SR 핀홀
            //JCDs += "NJCD=13&JCD=J&NJTY=" + CountNGJ.ToString() + "&";     // 잔류 Cu
            //JCDs += "NJCD=14&JCD=K&NJTY=" + CountNGK.ToString() + "&";     // 검은이물
            //JCDs += "NJCD=15&JCD=L&NJTY=" + CountNGL.ToString() + "&";     // 도금불량
            //JCDs += "NJCD=16&JCD=M&NJTY=" + CountNGM.ToString() + "&";     // 패임
            //JCDs += "NJCD=17&JCD=N&NJTY=" + CountNGN.ToString() + "&";     // SR 불량
            //JCDs += "NJCD=18&JCD=O&NJTY=" + CountNGO.ToString() + "&";     // Open
            //JCDs += "NJCD=19&JCD=P&NJTY=" + CountNGP.ToString() + "&";     // 돌출
            //JCDs += "NJCD=20&JCD=Q&NJTY=" + CountNGQ.ToString() + "&";     // SR Misalign
            //JCDs += "NJCD=21&JCD=R&NJTY=" + CountNGR.ToString() + "&";     // S/C AM
            //JCDs += "NJCD=22&JCD=S&NJTY=" + CountNGS.ToString() + "&";     // Short
            //JCDs += "NJCD=23&JCD=T&NJTY=" + CountNGT.ToString() + "&";     // SR 튐
            //JCDs += "NJCD=24&JCD=U&NJTY=" + CountNGU.ToString() + "&";     // 오염
            //JCDs += "NJCD=25&JCD=W&NJTY=" + CountNGW.ToString() + "&";     // SR 기포
            //JCDs += "NJCD=26&JCD=Y&NJTY=" + CountNGY.ToString() + "&";     // Punching Misalign
            
            //// 14. Sequence Number of a Judge Code
            //string sequenceJudeCode = "NJCD=" + "&";
            //// 15. Judge Code
            //string judgeCode = "JCD=" + "&";
            //// 16. Total Count of JCD
            //string totalJCD = "NJTY=" + "&";

            // 17. 다음공정(TSSF00, TSWG00, TSET00, TSAI00, TSQC00)
            string nextOper = "NEXTOPER=" + NextOper + "&";
            // 18. 외관검사 Mode (1: 반사, 2:투과)
            string inspectMode;
            if (NextOper == "TSSG00" || NextOper == "TSCG00") inspectMode = "INSPMODE=" + "0&"; // PI면 검사, 회로면 검사
            else                                              inspectMode = "INSPMODE=" + "1&"; // 가편집
            // 19. Total Count of Rows
            string totalCountRows = "TROW=" + "2&";
            // 20. Map Data of Row1
            string rawData1 = "MAP1=" + MapData1 + "&";
            // 21. Map Data of Row2
            string rawData2 = "MAP2=" + MapData2 + "&";
            // 21. Map Data of Row3
            string rawData3 = "MAP3=" + MapData3 + "&";
            // 22. 특이사항
            string comment = "COMMENT=" + Comment + "&";


            //string sendBody = machineID + transactionTime + processType + lotID + opID 
            //                    + toolID1 + toolID1Count + toolID2 + toolID2Count + lotCount 
            //                    + goodCount + ngCount + totalJudgeCode + sequenceJudeCode + judgeCode 
            //                    + totalJCD + nextOper + inspectMode + totalCountRows + rawData1 
            //                    + rawData2 + comment;

            //string sendBody = machineID + transactionTime + processType + lotID + opID
            //                    + toolID1 + toolID1Count + toolID2 + toolID2Count + lotCount
            //                    + goodCount + ngCount + totalJudgeCode + JCDs
            //                    + nextOper + inspectMode + totalCountRows + rawData1
            //                    + rawData2 + comment;

            string sendBody = machineID + transactionTime + processType + lotID + opID
                                + toolID1 + toolID1Count + toolID2 + toolID2Count + lotCount
                                + goodCount + ngCount + totalJudgeCode + JCDs
                                + nextOper + inspectMode + totalCountRows + rawData1
                                + rawData2 + rawData3 + comment;

            ReceiveBody = "";

            if (null != seqService)
            {
                // OffLine
                if (false == seqService.IsOnlineMode)
                {
                    Log_Mes.WriteLine("[OFFLINE][SEND] " + "S6F91, " + SendID +", " + sendBody);
                    return 0;
                }
            }

            Log_Mes.WriteLine("[SEND] " + "S6F91, " + SendID + ", " + sendBody);


            if (true == tibco.PublishSend("S6F91", SendID, sendBody))
                return 0;

            return -1;
        }

        private int SendStatus()
        {
            //MID=TSET402&ITM=20070315083602&STATUS=I&

            if (null == tibco)
                return -1;

            string machineID = "MID=" + MachineID + "&";
            string transactionTime = "ITM=" + DateTime.Now.ToString("yyyyMMddHHmmss") + "&";
            string status = "STATUS=";

            switch (MachineStatus)
            {
                case 0:     // IDLE
                    status += "I&";
                    break;
                case 1:     // Run
                    status += "R&";
                    break;
                case 10:    // Down
                    status += "D&";
                    break;
                default:
                    break;
            }

            string sendBody = machineID + transactionTime + status;

            ReceiveBody = "";

            if (null != seqService)
            {
                // OffLine
                if (false == seqService.IsOnlineMode)
                {
                    Log_Mes.WriteLine("[OFFLINE][SEND] " + "S6F51, " + SendID +", " + sendBody);
                    return 0;
                }
            }

            Log_Mes.WriteLine("[SEND] " + "S6F51, " + SendID +", " + sendBody);

            if (true == tibco.PublishSend("S6F51", SendID, sendBody))
                return 0;

            return -1;
        }

        private int SendAlarm(int alarm, string alarmCode)
        {
            if (null == tibco)
                return -1;

            string machineID = "MID=" + MachineID + "&";
            string transactionTime = "ITM=" + DateTime.Now.ToString("yyyyMMddHHmmss") + "&";
            string status = "ALYN=";
            string code = "ALCD=" + alarmCode + "&";

            if (0 != alarm)
                status += "N&";
            else
                status += "F&";

            string sendBody = machineID + transactionTime + status + code;

            ReceiveBody = "";

            if (null != seqService)
            {
                // OffLine
                if (false == seqService.IsOnlineMode)
                {
                    Log_Mes.WriteLine("[OFFLINE][SEND] " + "S5F11, " + SendID + ", " + sendBody);
                    return 0;
                }
            }

            Log_Mes.WriteLine("[SEND] " + "S5F11, " + SendID + ", " + sendBody);

            if (true == tibco.PublishSend("S5F11", SendID, sendBody))
                return 0;

            return -1;
        }

        private int ReceiveTime(string body)
        {
            string[] arrays = new string[32];

            // LGIT 원호연 선임 요청으로 MES 메세지 파싱시 , 를 구분자로 사용하지 않도록 수정
            //arrays = body.Split('&', ',');
            arrays = body.Split('&');

            if (2 < arrays.Length)
            {
                string machineID = "MID=" + MachineID;

                if (machineID == arrays[0])
                {
                    if (-1 != arrays[1].IndexOf("ITM="))
                    {
                        string time = arrays[1].Replace("ITM=", "");
                        time = time.Replace(" ", "");
                        string year = time.Substring(0, 4);
                        string month = time.Substring(4, 2);
                        string day = time.Substring(6, 2);
                        string hour = time.Substring(8, 2);
                        string min = time.Substring(10, 2);
                        string sec = time.Substring(12, 2);

                        SysTime.SYSTEMTIME systemTime = new SysTime.SYSTEMTIME();

                        //SysTime.GetSystemTime(ref systemTime);  

                        systemTime.wYear = UInt16.Parse(year);
                        systemTime.wMonth = UInt16.Parse(month);
                        systemTime.wDay = UInt16.Parse(day);
                        systemTime.wHour = UInt16.Parse(hour);
                        systemTime.wMinute = UInt16.Parse(min);
                        systemTime.wSecond = UInt16.Parse(sec);

                        //sysTime.SetTime(TIME);
                        SysTime.SetLocalTime(ref systemTime);

                    }
                }
                else
                {
                    // Error Log
                    Log_Trace.WriteLine("[TsMes] ReceiveTime ReceiveError");
                }
            }

            return 0;
        }

        private int ReceiveLotInfo(string body)
        {
            string[] arrays = new string[32];

            // LGIT 원호연 선임 요청으로 MES 메세지 파싱시 , 를 구분자로 사용하지 않도록 수정
            //arrays = body.Split('&', ',');       
            arrays = body.Split('&');

            if (12 < arrays.Length)
            {
                string machineID = "MID=" + MachineID;

                if (machineID == arrays[0])
                {
                    string lotID;
                    string lotUnits;
                    string rcd;
                    string temp;

                    // 1. Transaction Time "ITM" -->> Skip

                    // 2. Lot ID "LOTID"
                    if (-1 != arrays[2].IndexOf("LOTID="))
                    {
                        lotID = arrays[2].Replace("LOTID=", "");
                    }

                    // 3. MODEL "MODEL"

                    // 4. Lot Units "TUN"
                    if (-1 != arrays[4].IndexOf("TUN="))
                    {
                        lotUnits = arrays[4].Replace("TUN=", "");
                        if ("" != lotUnits)
                            LotUnits = (int)double.Parse(lotUnits);
                        else
                            LotUnits = 0;
                    }

                    // 5    TOK         Lot  양품 Unit 수
                    if (-1 != arrays[5].IndexOf("TOK="))
                    {
                        temp = arrays[5].Replace("TOK=", "");
                        if ("" != temp)
                            TOK = (int)double.Parse(temp);
                        else
                            TOK = 0;
                    }

                    // 6    TNG         Lot  불량 Unit 수
                    if (-1 != arrays[6].IndexOf("TNG="))
                    {
                        temp = arrays[6].Replace("TNG=", "");
                        if ("" != temp)
                            TNG = (int)double.Parse(temp);
                        else
                            TNG = 0;
                    }

                    // 7    YIELD       출하수율
                    if (-1 != arrays[7].IndexOf("YIELD="))
                    {
                        temp = arrays[7].Replace("YIELD=", "");
                        if ("" != temp)
                            YIELD = double.Parse(temp);
                        else
                            YIELD = 0.0;
                    }

                    // 8    TCNG        연속 불량수
                    if (-1 != arrays[8].IndexOf("TCNG="))
                    {
                        temp = arrays[8].Replace("TCNG=", "");
                        if ("" != temp)
                            TCNG = (int)double.Parse(temp);
                        else
                            TCNG = 0;
                    }
                    if (TCNG != 0) dataService.DataRecipe.NGContinue = TCNG;

                    // 9    TJCNT       JOINT 수량
                    if (-1 != arrays[9].IndexOf("TJCNT="))
                    {
                        temp = arrays[9].Replace("TJCNT=", "");
                        if ("" != temp)
                            TJCNT = (int)double.Parse(temp);
                        else
                            TJCNT = 0;
                    }

                    // 10   TJPOINT     Joint 거리(Joint 위치) // string Value
                    if (-1 != arrays[10].IndexOf("TJPOINT="))
                    {
                        TJPOINT = arrays[10].Replace("TJPOINT=", "");
                    }

                    // 11   UNITPITCH   PF
                    if (-1 != arrays[11].IndexOf("UNITPITCH="))
                    {
                        temp = arrays[11].Replace("UNITPITCH=", "");
                        if ("" != temp)
                            UNITPITCH = (int)double.Parse(temp);
                        else
                            UNITPITCH = 0;
                    }

                    // 12. "RCD" Y:존재, N:존재하지 않는 Lot
                    if (-1 != arrays[12].IndexOf("RCD="))
                    {
                        rcd = arrays[12].Replace("RCD=", "");
                        JobPermission = rcd;
                    }

                    dataService.DataRecipe.Save();
                    System.Windows.Forms.Application.DoEvents();
                }
                else
                {
                    // Error Log
                    Log_Trace.WriteLine("[TsMes] ReceiveLotInfo ReceiveError");
                }

                IsJobPermission = true;
            }

            return 0;
        }

        private void OnEventState(object sender, EventArgs e)
        {
            if (null == sysService)
                return;

            switch (sysService.State)
            {
                //case SystemService.States.none:             // 0
                //case SystemService.States.ready:            // 1    
                //case SystemService.States.idle:             // 2
                //    if (0 != MachineStatus)
                //    {
                //        MachineStatus = 0;
                //        SendStatus();
                //    }
                //    break;
                case SystemService.States.run:              // 3
                    if (1 != MachineStatus)
                    {
                        MachineStatus = 1;
                        SendStatus();
                    }
                    break;
                case SystemService.States.pause:            // 4
                    break;
                //case SystemService.States.homing:           // 5
                //case SystemService.States.homeDone:         // 6
                //case SystemService.States.jobDone:          // 7
                //    break;
                //case SystemService.States.reset:            // 8
                //    break;
                //case SystemService.States.lightAlarm:       // 9
                //    break;
                //case SystemService.States.stop:             // 10
                //case SystemService.States.heavyAlarm:       // 11
                //case SystemService.States.emg:              // 12
                //    break;
                default:
                    if (0 != MachineStatus)
                    {
                        MachineStatus = 0;
                        SendStatus();
                    }
                    break;
            } // switch (status)
        }
    }
}
