using System;

namespace SmartICAVI
{
    class MesVirtual : IMes
    {

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
                return true;
            }
        }

        public Object Control { get { return null; } }

        private string ReceiveName { get; set; }
        private string ReceiveID { get; set; }
        private string ReceiveBody { get; set; }

        public int[] CountNGs;
        public string[] CountNGIDs;
        //public int CountNG1 { get; set; }
        //public int CountNG2 { get; set; }
        //public int CountNG3 { get; set; }
        //public int CountNG4 { get; set; }
        //public int CountNG5 { get; set; }
        //public int CountNGA { get; set; }
        //public int CountNGB { get; set; }
        //public int CountNGC { get; set; }
        //public int CountNGD { get; set; }
        //public int CountNGE { get; set; }
        //public int CountNGF { get; set; }
        //public int CountNGH { get; set; }
        //public int CountNGJ { get; set; }
        //public int CountNGK { get; set; }
        //public int CountNGL { get; set; }
        //public int CountNGM { get; set; }
        //public int CountNGN { get; set; }
        //public int CountNGO { get; set; }
        //public int CountNGP { get; set; }
        //public int CountNGQ { get; set; }
        //public int CountNGR { get; set; }
        //public int CountNGS { get; set; }
        //public int CountNGT { get; set; }
        //public int CountNGU { get; set; }
        //public int CountNGW { get; set; }
        //public int CountNGY { get; set; }

        public int Initialize()
        {
            return 0;
        }

        public int UnInitialize()
        {

            return 0;
        }

        public int Send(object command, int value = 0, string strValue = "")
        {
            switch ((MesService.commands)command)
            {
                case MesService.commands.req_time://=0, 
                    break;
                case MesService.commands.send_jobstart:
                    IsJobPermission = true;
                    JobPermission = "Y";
                    break;
                case MesService.commands.req_lot:
                    break;
                case MesService.commands.send_jobend:
                    return SendJobEnd();
                case MesService.commands.send_status:
                    break;
                case MesService.commands.send_alarm:
                    break;
                default:
                    break;
            }

            return 0;
        }

        public int Open()
        {
            return 0;
        }

        public int Close()
        {
            return 0;
        }

        public int SetParams()
        {
            return 0;
        }

        public int SetCountNGs(int[] countNGs)
        {
            for (int i = 0; i < countNGs.Length; ++i)
                CountNGs[i] = countNGs[i];

            return 0;
        }
        #endregion

        public MesVirtual()
        {
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
            string inspectMode = "INSPMODE" + "&";
            // 19. Total Count of Rows
            string totalCountRows = "TROW=" + "&";
            // 20. Map Data of Row1
            string rawData1 = "MAP1=" + MapData1 + "&";
            // 21. Map Data of Row2
            string rawData2 = "MAP2=" + MapData2 + "&";
            // 22. 특이사항
            string comment = "COMMENT=" + Comment + "&";


            //string sendBody = machineID + transactionTime + processType + lotID + opID 
            //                    + toolID1 + toolID1Count + toolID2 + toolID2Count + lotCount 
            //                    + goodCount + ngCount + totalJudgeCode + sequenceJudeCode + judgeCode 
            //                    + totalJCD + nextOper + inspectMode + totalCountRows + rawData1 
            //                    + rawData2 + comment;

            string sendBody = machineID + transactionTime + processType + lotID + opID
                                + toolID1 + toolID1Count + toolID2 + toolID2Count + lotCount
                                + goodCount + ngCount + totalJudgeCode + JCDs
                                + nextOper + inspectMode + totalCountRows + rawData1
                                + rawData2 + comment;

            ReceiveBody = "";


            Log_Mes.WriteLine("[SEND] " + "S6F91, " + SendID + ", " + sendBody);


            return -1;
        }
    }
}
