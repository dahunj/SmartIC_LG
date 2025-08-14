using System;

namespace SmartICAVI
{
    class DataSystem
    {
        private string m_strPath { get; set; }

        // Current Info
        public string RecipeName { get; set; }
        public string MachineName { get; set; }
        public string CamType { get; set; }
        public string LotID { get; set; }
        public string UserID { get; set; }
        public string ToolID { get; set; }
        public string NextProcess { get; set; }

        public string Comment { get; set; }

        public int JobType { get; set; }      // 0:continuous,    1:total,        2:good
        public int TotalCount { get; set; }   // Total 수량작업
        public int GoodCount { get; set; }    // Good 수량작업
        
        public TimeSpan LightTimeTop { get; set; }
        public TimeSpan LightTimeBottom { get; set; }
        public TimeSpan LightTimeMono { get; set; }
        

        // Use
        public bool UseVirtualKeyboard { get; set; }
        public bool UseSelectParam { get; set; }
        public bool UseJobPermission { get; set; }
        public bool UseJobDoneBuzzer { get; set; }

        public bool UseLightAlarmBuzzer { get; set; }
        public bool UseHeavyAlarmBuzzer { get; set; }

        public bool UseCheckGuide { get; set; }

        // Machine
        public int LogSave { get; set; }
        public int ResultSave { get; set; }
        public int ReportSave { get; set; }
        public int ReportPeriod { get; set; }
        public int BarcodeCOMPort { get; set; }
        public int JobDoneBuzzerTime { get; set; }

        public int LightAlarmBuzzerTime { get; set; }
        public int HeavyAlarmBuzzerTime { get; set; }

        public int DelayPunch { get; set; }       // yjs 20161205 Punch Up 시 Delay Time 
        public int DelayFeeding { get; set; }     // yjs 20161216 PunchFeeding 완료후 대기시간 

        public double BottomDistance { get; set; }
        public double MonoDistance { get; set; }
        public double PunchDistance { get; set; }

        public double UncoilerReference { get; set; }
        public double RecoilerReference { get; set; }
        public double BufferReference { get; set; }
        public double BufferLimitN { get; set; }
        public double BufferLimitP { get; set; }

        public double PunchStroke { get; set; }
        public double PunchOffsetX { get; set; }    // 얼라인 카메라의 중심점에서 옵셋값
        public double PunchOffsetY { get; set; }
        public double PunchOffsetBX { get; set; }   // LineA 에 대한 LineB 펀치 옵셋값
        public double PunchOffsetBY { get; set; }

        public double PunchOffsetCenterX { get; set; }   // LineA 에 대한 Center 펀치 옵셋값
        public double PunchOffsetCenterY { get; set; }
               
        public double AlignVisionX { get; set; }
        public double AlignVisionY { get; set; }

        public double TopVisionZ { get; set; }
        public double BottomVisionZ { get; set; }

        public double CalX { get; set; }
        public double CalY { get; set; }

        public double ScanTolerance { get; set; }       // 스캔시작 - 트리거 시작 위치
        public double ImageTolerance { get; set; }      // 이미지 획득 여유분
        public double MotionTolerance { get; set; }     // yjs 20161206 Inposition Check 
        public double AlignTolerance { get; set; }      // 펀치 얼라인 위치 허용공차 ; 설정된 값 이상의 위치가 측정되면 알람처리 한다.

        public double ScanDummyTop { get; set; }
        public double ScanDummyBottom { get; set; }
        public double ScanDummyMono { get; set; }

        public double IndexTolerance { get; set; }      // (현재피딩위치 - IndexTolerance)/PF 가 검사 Index 보다 클 경우 속도를 50% 줄인다.

        public int PunchCountLimit { get; set; }     // 최대 펀치 타발 수 : 50000
        public int PunchCount { get; set; }
        public int MES_PunchCount { get; set; }         // 2020.07.14 khs - MES 전달시 진행한 Lot에 사용된 펀치 카운트만 보내야 한다. (원호연 선임 요청사항)
        public int PunchImageLimit { get; set; }      // 펀치 이미지 Display 수

        public int SectionMinUnits { get; set; }      // 1,2 구간 분리 최소 Unit 수 18000 개 이상 일 겨우 조인트로 인식

        public string MachineID { get; set; }
        public string ProcessID { get; set; }
        public string LogPath { get; set; }
        public string ResultPath { get; set; }
        public string ReportPath { get; set; }
        public string PunchPath { get; set; }

        public string Top1Path { get; set; }
        public string Top2Path { get; set; }
        public string Bottom1Path { get; set; }
        public string Bottom2Path { get; set; }
        public string Mono1Path { get; set; }
        public string Mono2Path { get; set; }
        public string ServerPath { get; set; }

        public string VerifyPath { get; set; }


        // MES
        public int OnlineState { get; set; } // 1: Offline, 2:Local, 3:Remote
        public int TimeoutReply { get; set; }
        public int TimeoutTransaction { get; set; }
        public int TimeoutConversation { get; set; }

        public int MachinePort { get; set; }
        public int ServerPort { get; set; }

        public string ServerIP { get; set; }
        //public string CurrentModel { get; set; }

        public string Service { get; set; }
        public string Network { get; set; }
        public string Daemon { get; set; }
        public int PubRvType { get; set; }
        public string PubSubject { get; set; }
        public int PubTimeout { get; set; }
        public int SubRvType { get; set; }
        public string SubSubject { get; set; }


        // IP Address
        public string TopIPAddress { get; set; }
        public string BottomIPAddress { get; set; }
        public string MonoIPAddress { get; set; }

        public string Top2IPAddress { get; set; }
        public string Bottom2IPAddress { get; set; }
        public string Mono2IPAddress { get; set; }

        public int TopPort { get; set; }
        public int BottomPort { get; set; }
        public int MonoPort { get; set; }

        public int Top2Port { get; set; }
        public int Bottom2Port { get; set; }
        public int Mono2Port { get; set; }

        public int TopHPort { get; set; }
        public int BottomHPort { get; set; }
        public int MonoHPort { get; set; }
       
        public int Top2HPort { get; set; }
        public int Bottom2HPort { get; set; }
        public int Mono2HPort { get; set; }

        public string ServerPCIPAddress { get; set; }
        public int ServerPCPort { get; set; }
        public int ServerPCHPort { get; set; }

         
        public string CamIPAddress { get; set; }
        public int CamPort { get; set; }
        public int CamHPort { get; set; }
        
        public bool IsSelectedVision { get; set; }
        public bool IsSelectedModify { get; set; }
        public bool IsSelectedTop { get; set; }
        public bool IsSelectedBottom { get; set; }
        public bool IsSelectedMono { get; set; }
        public bool IsSelectedPunch { get; set; }
        public bool IsSelectedPunchInspect { get; set; }
        public bool IsSelectedPunchTolerance { get; set; }   // yjs 20161215 펀칭홀 검사 후 허용공차 검사
        public bool IsSelectedCNGPunch { get; set; }     // CNG Punch 시 
        public bool IsSelectedHoleStop { get; set; }
        public bool IsSelectedSectionYield { get; set; }
        public bool IsSelectedAlwaysAlign { get; set; }
        public bool IsSelectedJointStop { get; set; }        // 조인트 발생시 사용자 확인여부 설정.
        public bool IsSelectedCleanRoller { get; set; }      // 크린 롤러 사용유무 설정.
        public bool IsSelectedMissPrint { get; set; }        // 노광편차 사용유무 설정.
        public bool IsSelectedDualErrorStop { get; set; }    // Top, Bottom 동시 불량시 확인 설정. 
        public bool IsSelectedBackFeeding { get; set; }
        public bool IsSelectedAlignTolerance { get; set; }
        public bool IsSelectedPunchFirstUnit { get; set; }      // 첫번째 유닛 펀칭 설정.
        public bool IsSelectedPunchLastUnit { get; set; }       // 마지막 유닛 펀칭 설정.
        public bool IsSelectedPunchSetUnit { get; set; }        // 선택된 유닛 펀칭 설정.
        public int PunchUnitIndex { get; set; }

        public double PosBackFeeding { get; set; }

        public int PunchInspectUnit { get; set; }

        public string NTDriveTop1 { get; set; }
        public string NTDriveTop2 { get; set; }
        public string NTDriveBottom1 { get; set; }
        public string NTDriveBottom2 { get; set; }
        public string NTDriveMono1 { get; set; }
        public string NTDriveMono2 { get; set; }
        public string NTDriveServer { get; set; }

        public string PublicFolderTop1 { get; set; }
        public string PublicFolderTop2 { get; set; }
        public string PublicFolderBottom1 { get; set; }
        public string PublicFolderBottom2 { get; set; }
        public string PublicFolderMono1 { get; set; }
        public string PublicFolderMono2 { get; set; }
        public string PublicFolderServer { get; set; }

        // REVIEW
        public int ReviewImageSize { get; set; }
        public int ReviewImageArray { get; set; }
        public int ReviewImageNumber { get; set; }
        public int VerifyMethod { get; set; }     // 0:전체판정, 1:개별판정

        public string[] verifyCodes { get; set; }
        public bool IsUseVerifyCodes { get; set; }

        // Trigger
        public double TriggerColor_Period { get; set; }
        public double TriggerColor_Width { get; set; }
        public int TriggerColor_Level { get; set; }

        public double TriggerMono_Period { get; set; }
        public double TriggerMono_Width { get; set; }
        public int TriggerMono_Level { get; set; }

        // laser
        public bool UseLaser { get; set; }
        public double LaserXpos { get; set; }
        public double LaserYpos { get; set; }
        public int LaserIndex { get; set; }

        public double VisionLastPos { get; set; }   // 마지막 펀칭 비전 피드 모터 
        public double FeedLastPos { get; set; }     // 마지막 펀칭 펀칭 피드 모터
        public double AlignXLastPos { get; set; }   // 마지막 펀칭 펀치 x 모터
        public double AlignYLastPos { get; set; }   // 마지막 펀칭 펀치 y 모터
        public double VisionCurrPos { get; set; }   // 일시정지 후 재작업 시 사용
        public double FeedCurrPos { get; set; }     // 일시정지 후 재작업 시 사용
        public double BufferLastPos { get; set; }   // 일시정지 후 재작업 시 사용
        public double BufferCurrPos { get; set; }   // 일시정지 후 재작업 시 사용
        public double AlignXCurrPos { get; set; }   // 마지막 펀칭 펀치 x 모터
        public double AlignYCurrPos { get; set; }   // 마지막 펀칭 펀치 y 모터

        // punch align position offset
        public bool FirstIndexPause { get; set; }
        public double AlignPosOffsetX { get; set; }
        public double AlignPosOffsetY { get; set; }


        public DataSystem()
        {
            RecipeName = "Default";
            MachineName = "SmartICAVI";
            CamType = "EXTERN";

            LotID = "LOTID";
            UserID = "USERID";
            ToolID = "PUNCH";
            NextProcess = "TSSF00";
            Comment = "";

            JobType = 0;
            TotalCount = 10000;
            GoodCount = 10000;

            UseVirtualKeyboard = false;
            UseSelectParam = false;
            UseJobPermission = false;
            UseJobDoneBuzzer = false;
            UseLightAlarmBuzzer = false;
            UseHeavyAlarmBuzzer = false;
            UseCheckGuide = false;

            IsSelectedVision = true;
            IsSelectedModify = false;
            IsSelectedTop = true;
            IsSelectedBottom = true;
            IsSelectedMono = true;
            IsSelectedPunch = true;

            IsSelectedPunchInspect = true;
            IsSelectedPunchTolerance = false;
            IsSelectedCNGPunch = false;
            IsSelectedHoleStop = true;
            IsSelectedSectionYield = true;
            IsSelectedAlwaysAlign = true;
            IsSelectedJointStop = true;
            IsSelectedCleanRoller = false;
            IsSelectedMissPrint = true;
            IsSelectedDualErrorStop = true;
            IsSelectedBackFeeding = false;
            IsSelectedAlignTolerance = true;
            IsSelectedPunchFirstUnit = false;
            IsSelectedPunchLastUnit = false;
            IsSelectedPunchSetUnit = false;
            PunchUnitIndex = 0;

            PosBackFeeding = 5.0;

            PunchInspectUnit = 1000;

            LogSave = 30;
            ResultSave = 30;
            ReportSave = 30;
            ReportPeriod = 30;
            BarcodeCOMPort = 1;
            JobDoneBuzzerTime = 3;
            LightAlarmBuzzerTime = 3;
            HeavyAlarmBuzzerTime = 360;
            DelayPunch = 100;
            DelayFeeding = 200;

            //SupportReference = 10000;
            //CollectReference = 10000;

            BottomDistance = 154.0;
            MonoDistance = 308.0;
            PunchDistance = 2647.0;
            //Velocity = 150.0;
            //AccelTime = 1000.0;

            //JogVelocity = 100.0;
            //JogAccelTime = 300.0;

            UncoilerReference = 150.0;
            RecoilerReference = 150.0;
            BufferReference = 150.0;
            BufferLimitN = 70.0;
            BufferLimitP = 200.0;

            PunchStroke = 200.0;
            PunchOffsetX = 0.0;
            PunchOffsetY = 8.35;
            PunchOffsetBX = 0.0;
            PunchOffsetBY = 0.0;
            PunchOffsetCenterX = 0.0;
            PunchOffsetCenterY = 0.0;
            AlignVisionX = 63.0;
            AlignVisionY = 0.0;

            TopVisionZ = 18.8;
            BottomVisionZ = 23.0;

            CalX = 0.0083;
            CalY = 0.0083;

            ScanTolerance = 10.0;
            ImageTolerance = 5.0;
            MotionTolerance = 0.01;
            IndexTolerance = 1000.0;
            AlignTolerance = 1.8;

            ScanDummyTop = 0.95;
            ScanDummyBottom = 0.95;
            ScanDummyMono = 0.475;

            PunchCount = 0;
            MES_PunchCount = 0;
            PunchCountLimit = 50000;
            PunchImageLimit = 200;

            SectionMinUnits = 13000;

            MachineID = "TSFN402";
            ProcessID = "FN";
            LogPath = ".\\Data\\Log";
            ResultPath = ".\\Result";
            ReportPath = ".\\Report";
            PunchPath = "C:\\PunchData";

            Top1Path = ".\\Top1";
            Top2Path = ".\\Top2";
            Bottom1Path = ".\\Bottom1";
            Bottom2Path = ".\\Bottom2";
            Mono1Path = ".\\Mono1";
            Mono2Path = ".\\Mono2";

            ServerPath = "\\S:\\PublicData";
            VerifyPath = "";


            TimeoutReply = 60;
            TimeoutTransaction = 60;
            TimeoutConversation = 60;

            MachinePort = 8001;
            ServerPort = 8000;

            ServerIP = "127.0.0.1";
            //CurrentModel = "";

            Service = "7700";
            Network = ";239.100.100.2";
            Daemon = "156.147.113.15:7700";
            PubRvType = 2;
            PubSubject = "TS3.REQ.SPCSRV";
            PubTimeout = 30;
            SubRvType = 0;
            SubSubject = "TS3.ECS";

            OnlineState = 0;


            TopIPAddress = "192.168.10.11";
            Top2IPAddress = "192.168.10.12";
            BottomIPAddress = "192.168.10.13";
            Bottom2IPAddress = "192.168.10.14";
            MonoIPAddress = "192.168.10.15";
            Mono2IPAddress = "192.168.10.16";

            ServerPCIPAddress = "192.168.10.100";
            ServerPCPort = 8151;
            ServerPCHPort = 8051;

            CamIPAddress = "127.0.0.1";
            CamPort = 8801;
            CamHPort = 8800;

            TopPort = 8111;
            Top2Port = 8121;
            BottomPort = 8211;
            Bottom2Port = 8221;
            MonoPort = 8311;
            Mono2Port = 8321;

            TopHPort = 8110;
            Top2HPort = 8120;
            BottomHPort = 8210;
            Bottom2HPort = 8220;
            MonoHPort = 8310;
            Mono2HPort = 8320;

            NTDriveTop1 = "Y:";
            NTDriveTop2 = "X:";
            NTDriveBottom1 = "W:";
            NTDriveBottom2 = "V:";
            NTDriveMono1 = "U:";
            NTDriveMono2 = "T:";
            NTDriveServer = "Z:";

            PublicFolderTop1 = "Defect";
            PublicFolderTop2 = "Defect";
            PublicFolderBottom1 = "Defect";
            PublicFolderBottom2 = "Defect";
            PublicFolderMono1 = "Defect";
            PublicFolderMono2 = "Defect";
            PublicFolderServer = "PublicData";

            // REVIEW
            ReviewImageSize = 0;
            ReviewImageArray = 0;
            ReviewImageNumber = 0;
            VerifyMethod = 0;

            verifyCodes = new string[10] { "G", "1", "2", "3", "4", "5", "A", "B", "C", "D" };
            IsUseVerifyCodes = false;

            LightTimeTop = new TimeSpan(0, 0, 0);
            LightTimeBottom = new TimeSpan(0, 0, 0);
            LightTimeMono = new TimeSpan(0, 0, 0);


            // Trigger
            TriggerColor_Period = 0.0095;
            TriggerColor_Width = 10.0;
            TriggerColor_Level = 0;

            TriggerMono_Period = 0.00475;
            TriggerMono_Width = 5.0;
            TriggerMono_Level = 0;

            // laser
            UseLaser = false;
            LaserXpos = 0.0;
            LaserYpos = 0.0;
            LaserIndex = -1;

            VisionLastPos = 0.0;
            FeedLastPos = 0.0;
            AlignXLastPos = 0.0;
            AlignYLastPos = 0.0;
            VisionCurrPos = 0.0;
            FeedCurrPos = 0.0;
            BufferLastPos = 0.0;
            BufferCurrPos = 0.0;
            AlignXCurrPos = 0.0;
            AlignYCurrPos = 0.0;

            // punch align position offset
            FirstIndexPause = false;
            AlignPosOffsetX = 0.0;
            AlignPosOffsetY = 0.0;
        }

        public void Load(string path)
        {
            m_strPath = path;
            Load();
        }

        public void Save(string path)
        {
            m_strPath = path;
            Save();
        }

        public void Load()
        {
            IniFile ini = new IniFile();
            string strSection = "CURRENT_INFO";
            RecipeName = ini.Read(strSection, "RecipeName", RecipeName, m_strPath);
            MachineName = ini.Read(strSection, "MachineName", MachineName, m_strPath);
            CamType = ini.Read(strSection, "CamType", CamType, m_strPath);
            LotID = ini.Read(strSection, "LotID", LotID, m_strPath);
            UserID = ini.Read(strSection, "UserID", UserID, m_strPath);
            ToolID = ini.Read(strSection, "ToolID", ToolID, m_strPath);
            NextProcess = ini.Read(strSection, "NextProcess", NextProcess, m_strPath);
            JobType = int.Parse(ini.Read(strSection, "JobType", JobType.ToString(), m_strPath));
            TotalCount = int.Parse(ini.Read(strSection, "TotalCount", TotalCount.ToString(), m_strPath));
            GoodCount = int.Parse(ini.Read(strSection, "GoodCount", GoodCount.ToString(), m_strPath));

            LightTimeTop = TimeSpan.Parse(ini.Read(strSection, "LightTimeTop", LightTimeTop.ToString(), m_strPath));
            LightTimeBottom = TimeSpan.Parse(ini.Read(strSection, "LightTimeBottom", LightTimeBottom.ToString(), m_strPath));
            LightTimeMono = TimeSpan.Parse(ini.Read(strSection, "LightTimeMono", LightTimeMono.ToString(), m_strPath));


            strSection = "USE";
            UseVirtualKeyboard = bool.Parse(ini.Read(strSection, "UseVirtualKeyboard", UseVirtualKeyboard.ToString(), m_strPath));
            UseSelectParam = bool.Parse(ini.Read(strSection, "UseSelectParam", UseSelectParam.ToString(), m_strPath));
            UseJobPermission = bool.Parse(ini.Read(strSection, "UseJobPermission", UseJobPermission.ToString(), m_strPath));
            UseJobDoneBuzzer = bool.Parse(ini.Read(strSection, "UseJobDoneBuzzer", UseJobDoneBuzzer.ToString(), m_strPath));
            UseLightAlarmBuzzer = bool.Parse(ini.Read(strSection, "UseLightAlarmBuzzer", UseLightAlarmBuzzer.ToString(), m_strPath));
            UseHeavyAlarmBuzzer = bool.Parse(ini.Read(strSection, "UseHeavyAlarmBuzzer", UseHeavyAlarmBuzzer.ToString(), m_strPath));
            UseCheckGuide = bool.Parse(ini.Read(strSection, "UseCheckGuide", UseCheckGuide.ToString(), m_strPath));
            IsSelectedVision = bool.Parse(ini.Read(strSection, "IsSelectedVision", IsSelectedVision.ToString(), m_strPath));
            IsSelectedModify = bool.Parse(ini.Read(strSection, "IsSelectedModify", IsSelectedModify.ToString(), m_strPath));
            IsSelectedTop = bool.Parse(ini.Read(strSection, "IsSelectedTop", IsSelectedTop.ToString(), m_strPath));
            IsSelectedBottom = bool.Parse(ini.Read(strSection, "IsSelectedBottom", IsSelectedBottom.ToString(), m_strPath));
            IsSelectedMono = bool.Parse(ini.Read(strSection, "IsSelectedMono", IsSelectedMono.ToString(), m_strPath));
            IsSelectedPunch = bool.Parse(ini.Read(strSection, "IsSelectedPunch", IsSelectedPunch.ToString(), m_strPath));
            IsSelectedPunchInspect = bool.Parse(ini.Read(strSection, "IsSelectedPunchInspect", IsSelectedPunchInspect.ToString(), m_strPath));
            IsSelectedPunchTolerance = bool.Parse(ini.Read(strSection, "IsSelectedPunchTolerance", IsSelectedPunchTolerance.ToString(), m_strPath));
            IsSelectedCNGPunch = bool.Parse(ini.Read(strSection, "IsSelectedCNGPunch", IsSelectedCNGPunch.ToString(), m_strPath));
            PunchInspectUnit = int.Parse(ini.Read(strSection, "PunchInspectUnit", PunchInspectUnit.ToString(), m_strPath));
            IsSelectedHoleStop = bool.Parse(ini.Read(strSection, "IsSelectedHoleStop", IsSelectedHoleStop.ToString(), m_strPath));
            IsSelectedSectionYield = bool.Parse(ini.Read(strSection, "IsSelectedSectionYield", IsSelectedSectionYield.ToString(), m_strPath));
            IsSelectedAlwaysAlign = bool.Parse(ini.Read(strSection, "IsSelectedAlwaysAlign", IsSelectedAlwaysAlign.ToString(), m_strPath));
            IsSelectedJointStop = bool.Parse(ini.Read(strSection, "IsSelectedJointStop", IsSelectedJointStop.ToString(), m_strPath));
            IsSelectedCleanRoller = bool.Parse(ini.Read(strSection, "IsSelectedCleanRoller", IsSelectedCleanRoller.ToString(), m_strPath));
            IsSelectedMissPrint = bool.Parse(ini.Read(strSection, "IsSelectedMissPrint", IsSelectedMissPrint.ToString(), m_strPath));
            IsSelectedDualErrorStop = bool.Parse(ini.Read(strSection, "IsSelectedDualErrorStop", IsSelectedDualErrorStop.ToString(), m_strPath));
            IsSelectedBackFeeding = bool.Parse(ini.Read(strSection, "IsSelectedBackFeeding", IsSelectedBackFeeding.ToString(), m_strPath));
            IsSelectedAlignTolerance = bool.Parse(ini.Read(strSection, "IsSelectedAlignTolerance", IsSelectedAlignTolerance.ToString(), m_strPath));
            IsSelectedPunchFirstUnit = bool.Parse(ini.Read(strSection, "IsSelectedPunchFirstUnit", IsSelectedPunchFirstUnit.ToString(), m_strPath));
            IsSelectedPunchLastUnit = bool.Parse(ini.Read(strSection, "IsSelectedPunchLastUnit", IsSelectedPunchLastUnit.ToString(), m_strPath));
            IsSelectedPunchSetUnit = bool.Parse(ini.Read(strSection, "IsSelectedPunchSetUnit", IsSelectedPunchSetUnit.ToString(), m_strPath));
            PunchUnitIndex = int.Parse(ini.Read(strSection, "PunchUnitIndex", PunchUnitIndex.ToString(), m_strPath));

            strSection = "MACHINE";
            LogSave = int.Parse(ini.Read(strSection, "LogSave", LogSave.ToString(), m_strPath));
            ResultSave = int.Parse(ini.Read(strSection, "ResultSave", ResultSave.ToString(), m_strPath));
            ReportSave = int.Parse(ini.Read(strSection, "ReportSave", ReportSave.ToString(), m_strPath));
            ReportPeriod = int.Parse(ini.Read(strSection, "ReportPeriod", ReportPeriod.ToString(), m_strPath));
            BarcodeCOMPort = int.Parse(ini.Read(strSection, "BarcodeCOMPort", BarcodeCOMPort.ToString(), m_strPath));
            BottomDistance = double.Parse(ini.Read(strSection, "BottomDistance", BottomDistance.ToString(), m_strPath));
            MonoDistance = double.Parse(ini.Read(strSection, "MonoDistance", MonoDistance.ToString(), m_strPath));
            PunchDistance = double.Parse(ini.Read(strSection, "PunchDistance", PunchDistance.ToString(), m_strPath));
            PosBackFeeding = double.Parse(ini.Read(strSection, "PosBackFeeding", PosBackFeeding.ToString(), m_strPath));
            JobDoneBuzzerTime = int.Parse(ini.Read(strSection, "JobDoneBuzzerTime", JobDoneBuzzerTime.ToString(), m_strPath));
            LightAlarmBuzzerTime = int.Parse(ini.Read(strSection, "LightAlarmBuzzerTime", LightAlarmBuzzerTime.ToString(), m_strPath));
            HeavyAlarmBuzzerTime = int.Parse(ini.Read(strSection, "HeavyAlarmBuzzerTime", HeavyAlarmBuzzerTime.ToString(), m_strPath));
            DelayPunch = int.Parse(ini.Read(strSection, "DelayPunch", DelayPunch.ToString(), m_strPath));
            DelayFeeding = int.Parse(ini.Read(strSection, "DelayFeeding", DelayFeeding.ToString(), m_strPath));
            
            UncoilerReference = double.Parse(ini.Read(strSection, "UncoilerReference", UncoilerReference.ToString(), m_strPath));
            RecoilerReference = double.Parse(ini.Read(strSection, "RecoilerReference", RecoilerReference.ToString(), m_strPath));
            BufferReference = double.Parse(ini.Read(strSection, "BufferReference", BufferReference.ToString(), m_strPath));
            BufferLimitN = double.Parse(ini.Read(strSection, "BufferLimitN", BufferLimitN.ToString(), m_strPath));
            BufferLimitP = double.Parse(ini.Read(strSection, "BufferLimitP", BufferLimitP.ToString(), m_strPath));
            PunchStroke = double.Parse(ini.Read(strSection, "PunchStroke", PunchStroke.ToString(), m_strPath));
            PunchOffsetX = double.Parse(ini.Read(strSection, "PunchOffsetX", PunchOffsetX.ToString(), m_strPath));
            PunchOffsetY = double.Parse(ini.Read(strSection, "PunchOffsetY", PunchOffsetY.ToString(), m_strPath));
            PunchOffsetBX = double.Parse(ini.Read(strSection, "PunchOffsetBX", PunchOffsetBX.ToString(), m_strPath));
            PunchOffsetBY = double.Parse(ini.Read(strSection, "PunchOffsetBY", PunchOffsetBY.ToString(), m_strPath));
            PunchOffsetCenterX = double.Parse(ini.Read(strSection, "PunchOffsetCenterX", PunchOffsetCenterX.ToString(), m_strPath));
            PunchOffsetCenterY = double.Parse(ini.Read(strSection, "PunchOffsetCenterY", PunchOffsetCenterY.ToString(), m_strPath));
            AlignVisionX = double.Parse(ini.Read(strSection, "AlignVisionX", AlignVisionX.ToString(), m_strPath));
            AlignVisionY = double.Parse(ini.Read(strSection, "AlignVisionY", AlignVisionY.ToString(), m_strPath));
            TopVisionZ = double.Parse(ini.Read(strSection, "TopVisionZ", TopVisionZ.ToString(), m_strPath));
            BottomVisionZ = double.Parse(ini.Read(strSection, "BottomVisionZ", BottomVisionZ.ToString(), m_strPath));

            CalX = double.Parse(ini.Read(strSection, "CalX", CalX.ToString(), m_strPath));
            CalY = double.Parse(ini.Read(strSection, "CalY", CalY.ToString(), m_strPath));

            ScanTolerance = double.Parse(ini.Read(strSection, "ScanTolerance", ScanTolerance.ToString(), m_strPath));
            ImageTolerance = double.Parse(ini.Read(strSection, "ImageTolerance", ImageTolerance.ToString(), m_strPath));
            MotionTolerance = double.Parse(ini.Read(strSection, "MotionTolerance", MotionTolerance.ToString(), m_strPath));
            IndexTolerance = double.Parse(ini.Read(strSection, "IndexTolerance", IndexTolerance.ToString(), m_strPath));
            AlignTolerance = double.Parse(ini.Read(strSection, "AlignTolerance", AlignTolerance.ToString(), m_strPath));

            ScanDummyTop = double.Parse(ini.Read(strSection, "ScanDummyTop", ScanDummyTop.ToString(), m_strPath));
            ScanDummyBottom = double.Parse(ini.Read(strSection, "ScanDummyBottom", ScanDummyBottom.ToString(), m_strPath));
            ScanDummyMono = double.Parse(ini.Read(strSection, "ScanDummyMono", ScanDummyMono.ToString(), m_strPath));

            PunchCount = int.Parse(ini.Read(strSection, "PunchCount", PunchCount.ToString(), m_strPath));
            PunchCountLimit = int.Parse(ini.Read(strSection, "PunchCountLimit", PunchCountLimit.ToString(), m_strPath));
            PunchImageLimit = int.Parse(ini.Read(strSection, "PunchImageLimit", PunchImageLimit.ToString(), m_strPath));

            SectionMinUnits = int.Parse(ini.Read(strSection, "SectionMinUnits", SectionMinUnits.ToString(), m_strPath));

            MachineID = ini.Read(strSection, "MachineID", MachineID, m_strPath);
            ProcessID = ini.Read(strSection, "ProcessID", ProcessID, m_strPath);
            LogPath = ini.Read(strSection, "LogPath", LogPath, m_strPath);
            ResultPath = ini.Read(strSection, "ResultPath", ResultPath, m_strPath);
            ReportPath = ini.Read(strSection, "ReportPath", ReportPath, m_strPath);
            PunchPath = ini.Read(strSection, "PunchPath", PunchPath, m_strPath);

            Top1Path = ini.Read(strSection, "Top1Path", Top1Path, m_strPath);
            Top2Path = ini.Read(strSection, "Top2Path", Top2Path, m_strPath);
            Bottom1Path = ini.Read(strSection, "Bottom1Path", Bottom1Path, m_strPath);
            Bottom2Path = ini.Read(strSection, "Bottom2Path", Bottom2Path, m_strPath);
            Mono1Path = ini.Read(strSection, "Mono1Path", Mono1Path, m_strPath);
            Mono2Path = ini.Read(strSection, "Mono2Path", Mono2Path, m_strPath);
            ServerPath = ini.Read(strSection, "ServerPath", ServerPath, m_strPath);
            VerifyPath = ini.Read(strSection, "VerifyPath", VerifyPath, m_strPath);

            strSection = "MES";
            OnlineState = int.Parse(ini.Read(strSection, "OnlineState", OnlineState.ToString(), m_strPath));
            TimeoutReply = int.Parse(ini.Read(strSection, "TimeoutReply", TimeoutReply.ToString(), m_strPath));
            TimeoutTransaction = int.Parse(ini.Read(strSection, "TimeoutTransaction", TimeoutTransaction.ToString(), m_strPath));
            TimeoutConversation = int.Parse(ini.Read(strSection, "TimeoutConversation", TimeoutConversation.ToString(), m_strPath));

            MachinePort = int.Parse(ini.Read(strSection, "MachinePort", MachinePort.ToString(), m_strPath));
            ServerPort = int.Parse(ini.Read(strSection, "ServerPort", ServerPort.ToString(), m_strPath));

            ServerIP = ini.Read(strSection, "ServerIP", ServerIP, m_strPath);

            //CurrentModel = ini.Read(section, "CurrentModel", CurrentModel, Path);

            Service = ini.Read(strSection, "Service", Service, m_strPath);
            Network = ini.Read(strSection, "Network", Network, m_strPath);
            Daemon = ini.Read(strSection, "Daemon", Daemon, m_strPath);
            PubRvType = int.Parse(ini.Read(strSection, "PubRvType", PubRvType.ToString(), m_strPath));
            PubSubject = ini.Read(strSection, "PubSubject", PubSubject, m_strPath);
            PubTimeout = int.Parse(ini.Read(strSection, "PubTimeout", PubTimeout.ToString(), m_strPath));
            SubRvType = int.Parse(ini.Read(strSection, "SubRvType", SubRvType.ToString(), m_strPath));
            SubSubject = ini.Read(strSection, "SubSubject", SubSubject, m_strPath);


            strSection = "IPADDRESS";

            TopIPAddress = ini.Read(strSection, "TopIPAddress", TopIPAddress, m_strPath);
            TopPort = int.Parse(ini.Read(strSection, "TopPort", TopPort.ToString(), m_strPath));
            TopHPort = int.Parse(ini.Read(strSection, "TopHPort", TopHPort.ToString(), m_strPath));

            Top2IPAddress = ini.Read(strSection, "Top2IPAddress", Top2IPAddress, m_strPath);
            Top2Port = int.Parse(ini.Read(strSection, "Top2Port", Top2Port.ToString(), m_strPath));
            Top2HPort = int.Parse(ini.Read(strSection, "Top2HPort", Top2HPort.ToString(), m_strPath));

            BottomIPAddress = ini.Read(strSection, "BottomIPAddress", BottomIPAddress, m_strPath);
            BottomPort = int.Parse(ini.Read(strSection, "BottomPort", BottomPort.ToString(), m_strPath));
            BottomHPort = int.Parse(ini.Read(strSection, "BottomHPort", BottomHPort.ToString(), m_strPath));
            
            Bottom2IPAddress = ini.Read(strSection, "Bottom2IPAddress", Bottom2IPAddress, m_strPath);
            Bottom2Port = int.Parse(ini.Read(strSection, "Bottom2Port", Bottom2Port.ToString(), m_strPath));
            Bottom2HPort = int.Parse(ini.Read(strSection, "Bottom2HPort", Bottom2HPort.ToString(), m_strPath));
            
            MonoIPAddress = ini.Read(strSection, "MonoIPAddress", MonoIPAddress, m_strPath);
            MonoPort = int.Parse(ini.Read(strSection, "MonoPort", MonoPort.ToString(), m_strPath));
            MonoHPort = int.Parse(ini.Read(strSection, "MonoHPort", MonoHPort.ToString(), m_strPath));

            Mono2IPAddress = ini.Read(strSection, "Mono2IPAddress", Mono2IPAddress, m_strPath);
            Mono2Port = int.Parse(ini.Read(strSection, "Mono2Port", Mono2Port.ToString(), m_strPath));
            Mono2HPort = int.Parse(ini.Read(strSection, "Mono2HPort", Mono2HPort.ToString(), m_strPath));

            ServerPCIPAddress = ini.Read(strSection, "ServerPCIPAddress", ServerPCIPAddress, m_strPath);
            ServerPCPort = int.Parse(ini.Read(strSection, "ServerPCPort", ServerPCPort.ToString(), m_strPath));
            ServerPCHPort = int.Parse(ini.Read(strSection, "ServerPCHPort", ServerPCHPort.ToString(), m_strPath));

             
            CamIPAddress = ini.Read(strSection, "CamIPAddress", CamIPAddress, m_strPath);
            CamPort = int.Parse(ini.Read(strSection, "CamPort", CamPort.ToString(), m_strPath));
            CamHPort = int.Parse(ini.Read(strSection, "CamHPort", CamHPort.ToString(), m_strPath));
           
            NTDriveTop1 = ini.Read(strSection, "NTDriveTop1", NTDriveTop1, m_strPath);
            NTDriveTop2 = ini.Read(strSection, "NTDriveTop2", NTDriveTop2, m_strPath);
            NTDriveBottom1 = ini.Read(strSection, "NTDriveBottom1", NTDriveBottom1, m_strPath);
            NTDriveBottom2 = ini.Read(strSection, "NTDriveBottom2", NTDriveBottom2, m_strPath);
            NTDriveMono1 = ini.Read(strSection, "NTDriveMono1", NTDriveMono1, m_strPath);
            NTDriveMono2 = ini.Read(strSection, "NTDriveMono2", NTDriveMono2, m_strPath);
            NTDriveServer = ini.Read(strSection, "NTDriveServer", NTDriveServer, m_strPath);

            PublicFolderTop1 = ini.Read(strSection, "PublicFolderTop1", PublicFolderTop1, m_strPath);
            PublicFolderTop2 = ini.Read(strSection, "PublicFolderTop2", PublicFolderTop2, m_strPath);
            PublicFolderBottom1 = ini.Read(strSection, "PublicFolderBottom1", PublicFolderBottom1, m_strPath);
            PublicFolderBottom2 = ini.Read(strSection, "PublicFolderBottom2", PublicFolderBottom2, m_strPath);
            PublicFolderMono1 = ini.Read(strSection, "PublicFolderMono1", PublicFolderMono1, m_strPath);
            PublicFolderMono2 = ini.Read(strSection, "PublicFolderMono2", PublicFolderMono2, m_strPath);
            PublicFolderServer = ini.Read(strSection, "PublicFolderServer", PublicFolderServer, m_strPath);


            // REVIEW
            strSection = "REVIEW";
            ReviewImageSize = int.Parse(ini.Read(strSection, "ReviewImageSize", ReviewImageSize.ToString(), m_strPath));
            ReviewImageArray = int.Parse(ini.Read(strSection, "ReviewImageArray", ReviewImageArray.ToString(), m_strPath));
            ReviewImageNumber = int.Parse(ini.Read(strSection, "ReviewImageNumber", ReviewImageNumber.ToString(), m_strPath));
            VerifyMethod = int.Parse(ini.Read(strSection, "VerifyMethod", VerifyMethod.ToString(), m_strPath));

            for (int i = 0; i < 10; ++i)
            {
                verifyCodes[i] = ini.Read(strSection, "Code_" + i.ToString(), verifyCodes[i], m_strPath);
            }
            IsUseVerifyCodes = bool.Parse(ini.Read(strSection, "IsUseVerifyCodes", IsUseVerifyCodes.ToString(), m_strPath));

            // TRIGGER
            strSection = "TRIGGER";
            TriggerColor_Period = double.Parse(ini.Read(strSection, "TriggerColor_Period", TriggerColor_Period.ToString(), m_strPath));
            TriggerColor_Width = double.Parse(ini.Read(strSection, "TriggerColor_Width", TriggerColor_Width.ToString(), m_strPath));
            TriggerColor_Level = int.Parse(ini.Read(strSection, "TriggerColor_Level", TriggerColor_Level.ToString(), m_strPath));

            TriggerMono_Period = double.Parse(ini.Read(strSection, "TriggerMono_Period", TriggerMono_Period.ToString(), m_strPath));
            TriggerMono_Width = double.Parse(ini.Read(strSection, "TriggerMono_Width", TriggerMono_Width.ToString(), m_strPath));
            TriggerMono_Level = int.Parse(ini.Read(strSection, "TriggerMono_Level", TriggerMono_Level.ToString(), m_strPath));

            strSection = "LASER";
            LaserXpos = double.Parse(ini.Read(strSection, "Laser_X_Pos", LaserXpos.ToString(), m_strPath));
            LaserYpos = double.Parse(ini.Read(strSection, "Laser_Y_Pos", LaserYpos.ToString(), m_strPath));

            strSection = "OFFSET";
            AlignPosOffsetX = double.Parse(ini.Read(strSection, "AlignPosOffsetX", AlignPosOffsetX.ToString(), m_strPath));
            AlignPosOffsetY = double.Parse(ini.Read(strSection, "AlignPosOffsetY", AlignPosOffsetY.ToString(), m_strPath));

        }

        public void Save()
        {
            IniFile ini = new IniFile();
            string strSection = "CURRENT_INFO";

            CheckData(ini, strSection, "RecipeName", RecipeName, m_strPath);
            CheckData(ini, strSection, "MachineName", MachineName, m_strPath);
            CheckData(ini, strSection, "CamType", CamType, m_strPath);
            CheckData(ini, strSection, "LotID", LotID, m_strPath);
            CheckData(ini, strSection, "UserID", UserID, m_strPath);
            CheckData(ini, strSection, "ToolID", ToolID, m_strPath);
            CheckData(ini, strSection, "NextProcess", NextProcess, m_strPath);
            CheckData(ini, strSection, "JobType", JobType.ToString(), m_strPath);
            CheckData(ini, strSection, "TotalCount", TotalCount.ToString(), m_strPath);
            CheckData(ini, strSection, "GoodCount", GoodCount.ToString(), m_strPath);

            CheckData(ini, strSection, "LightTimeTop", LightTimeTop.ToString(), m_strPath);
            CheckData(ini, strSection, "LightTimeBottom", LightTimeBottom.ToString(), m_strPath);
            CheckData(ini, strSection, "LightTimeMono", LightTimeMono.ToString(), m_strPath);

            strSection = "USE";

            CheckData(ini, strSection, "UseVirtualKeyboard", UseVirtualKeyboard.ToString(), m_strPath);
            CheckData(ini, strSection, "UseSelectParam", UseSelectParam.ToString(), m_strPath);
            CheckData(ini, strSection, "UseJobPermission", UseJobPermission.ToString(), m_strPath);
            CheckData(ini, strSection, "UseJobDoneBuzzer", UseJobDoneBuzzer.ToString(), m_strPath);
            CheckData(ini, strSection, "UseLightAlarmBuzzer", UseLightAlarmBuzzer.ToString(), m_strPath);
            CheckData(ini, strSection, "UseHeavyAlarmBuzzer", UseHeavyAlarmBuzzer.ToString(), m_strPath);
            CheckData(ini, strSection, "UseCheckGuide", UseCheckGuide.ToString(), m_strPath);

            CheckData(ini, strSection, "IsSelectedVision", IsSelectedVision.ToString(), m_strPath);
            CheckData(ini, strSection, "IsSelectedModify", IsSelectedModify.ToString(), m_strPath);
            CheckData(ini, strSection, "IsSelectedTop", IsSelectedTop.ToString(), m_strPath);
            CheckData(ini, strSection, "IsSelectedBottom", IsSelectedBottom.ToString(), m_strPath);
            CheckData(ini, strSection, "IsSelectedMono", IsSelectedMono.ToString(), m_strPath);
            CheckData(ini, strSection, "IsSelectedPunch", IsSelectedPunch.ToString(), m_strPath);

            CheckData(ini, strSection, "IsSelectedPunchInspect", IsSelectedPunchInspect.ToString(), m_strPath);
            CheckData(ini, strSection, "IsSelectedPunchTolerance", IsSelectedPunchTolerance.ToString(), m_strPath);
            CheckData(ini, strSection, "IsSelectedCNGPunch", IsSelectedCNGPunch.ToString(), m_strPath);
            CheckData(ini, strSection, "PunchInspectUnit", PunchInspectUnit.ToString(), m_strPath);

            CheckData(ini, strSection, "IsSelectedHoleStop", IsSelectedHoleStop.ToString(), m_strPath);
            CheckData(ini, strSection, "IsSelectedSectionYield", IsSelectedSectionYield.ToString(), m_strPath);

            CheckData(ini, strSection, "IsSelectedAlwaysAlign", IsSelectedAlwaysAlign.ToString(), m_strPath);

            CheckData(ini, strSection, "IsSelectedJointStop", IsSelectedJointStop.ToString(), m_strPath);
            CheckData(ini, strSection, "IsSelectedCleanRoller", IsSelectedCleanRoller.ToString(), m_strPath);
            CheckData(ini, strSection, "IsSelectedMissPrint", IsSelectedMissPrint.ToString(), m_strPath);
            CheckData(ini, strSection, "IsSelectedDualErrorStop", IsSelectedDualErrorStop.ToString(), m_strPath);
            CheckData(ini, strSection, "IsSelectedBackFeeding", IsSelectedBackFeeding.ToString(), m_strPath);
            CheckData(ini, strSection, "IsSelectedAlignTolerance", IsSelectedAlignTolerance.ToString(), m_strPath);
            CheckData(ini, strSection, "IsSelectedPunchFirstUnit", IsSelectedPunchFirstUnit.ToString(), m_strPath);
            CheckData(ini, strSection, "IsSelectedPunchLastUnit", IsSelectedPunchLastUnit.ToString(), m_strPath);
            CheckData(ini, strSection, "IsSelectedPunchSetUnit", IsSelectedPunchSetUnit.ToString(), m_strPath);
            CheckData(ini, strSection, "PunchUnitIndex", PunchUnitIndex.ToString(), m_strPath);
            
            strSection = "MACHINE";
            CheckData(ini, strSection, "LogSave", LogSave.ToString(), m_strPath);
            CheckData(ini, strSection, "ResultSave", ResultSave.ToString(), m_strPath);
            CheckData(ini, strSection, "ReportSave", ReportSave.ToString(), m_strPath);
            CheckData(ini, strSection, "ReportPeriod", ReportPeriod.ToString(), m_strPath);
            CheckData(ini, strSection, "BarcodeCOMPort", BarcodeCOMPort.ToString(), m_strPath);
            CheckData(ini, strSection, "BottomDistance", BottomDistance.ToString(), m_strPath);
            CheckData(ini, strSection, "MonoDistance", MonoDistance.ToString(), m_strPath);
            CheckData(ini, strSection, "PunchDistance", PunchDistance.ToString(), m_strPath);
            CheckData(ini, strSection, "PosBackFeeding", PosBackFeeding.ToString(), m_strPath);

            CheckData(ini, strSection, "UncoilerReference", UncoilerReference.ToString(), m_strPath);
            CheckData(ini, strSection, "RecoilerReference", RecoilerReference.ToString(), m_strPath);
            CheckData(ini, strSection, "BufferReference", BufferReference.ToString(), m_strPath);
            CheckData(ini, strSection, "BufferLimitN", BufferLimitN.ToString(), m_strPath);
            CheckData(ini, strSection, "BufferLimitP", BufferLimitP.ToString(), m_strPath);
            CheckData(ini, strSection, "PunchStroke", PunchStroke.ToString(), m_strPath);

            CheckData(ini, strSection, "PunchOffsetX", PunchOffsetX.ToString(), m_strPath);
            CheckData(ini, strSection, "PunchOffsetY", PunchOffsetY.ToString(), m_strPath);
            CheckData(ini, strSection, "PunchOffsetBX", PunchOffsetBX.ToString(), m_strPath);
            CheckData(ini, strSection, "PunchOffsetBY", PunchOffsetBY.ToString(), m_strPath);
            CheckData(ini, strSection, "PunchOffsetCenterX", PunchOffsetCenterX.ToString(), m_strPath);
            CheckData(ini, strSection, "PunchOffsetCenterY", PunchOffsetCenterY.ToString(), m_strPath);
            CheckData(ini, strSection, "AlignVisionX", AlignVisionX.ToString(), m_strPath);
            CheckData(ini, strSection, "AlignVisionY", AlignVisionY.ToString(), m_strPath);
            CheckData(ini, strSection, "TopVisionZ", TopVisionZ.ToString(), m_strPath);
            CheckData(ini, strSection, "BottomVisionZ", BottomVisionZ.ToString(), m_strPath);

            CheckData(ini, strSection, "JobDoneBuzzerTime", JobDoneBuzzerTime.ToString(), m_strPath);
            CheckData(ini, strSection, "LightAlarmBuzzerTime", LightAlarmBuzzerTime.ToString(), m_strPath);
            CheckData(ini, strSection, "HeavyAlarmBuzzerTime", HeavyAlarmBuzzerTime.ToString(), m_strPath);
            CheckData(ini, strSection, "DelayPunch", DelayPunch.ToString(), m_strPath);
            CheckData(ini, strSection, "DelayFeeding", DelayFeeding.ToString(), m_strPath);

            CheckData(ini, strSection, "CalX", CalX.ToString(), m_strPath);
            CheckData(ini, strSection, "CalY", CalY.ToString(), m_strPath);

            CheckData(ini, strSection, "ScanTolerance", ScanTolerance.ToString(), m_strPath);
            CheckData(ini, strSection, "ImageTolerance", ImageTolerance.ToString(), m_strPath);
            CheckData(ini, strSection, "MotionTolerance", MotionTolerance.ToString(), m_strPath);
            CheckData(ini, strSection, "IndexTolerance", IndexTolerance.ToString(), m_strPath);
            CheckData(ini, strSection, "AlignTolerance", AlignTolerance.ToString(), m_strPath);

            CheckData(ini, strSection, "ScanDummyTop", ScanDummyTop.ToString(), m_strPath);
            CheckData(ini, strSection, "ScanDummyBottom", ScanDummyBottom.ToString(), m_strPath);
            CheckData(ini, strSection, "ScanDummyMono", ScanDummyMono.ToString(), m_strPath);

            CheckData(ini, strSection, "PunchCount", PunchCount.ToString(), m_strPath);
            CheckData(ini, strSection, "PunchCountLimit", PunchCountLimit.ToString(), m_strPath);
            CheckData(ini, strSection, "PunchImageLimit", PunchImageLimit.ToString(), m_strPath);

            CheckData(ini, strSection, "SectionMinUnits", SectionMinUnits.ToString(), m_strPath);

            CheckData(ini, strSection, "MachineID", MachineID, m_strPath);
            CheckData(ini, strSection, "ProcessID", ProcessID, m_strPath);
            CheckData(ini, strSection, "LogPath", LogPath, m_strPath);
            CheckData(ini, strSection, "ResultPath", ResultPath, m_strPath);
            CheckData(ini, strSection, "ReportPath", ReportPath, m_strPath);
            CheckData(ini, strSection, "PunchPath", PunchPath, m_strPath);

            CheckData(ini, strSection, "Top1Path", Top1Path, m_strPath);
            CheckData(ini, strSection, "Top2Path", Top2Path, m_strPath);
            CheckData(ini, strSection, "Bottom1Path", Bottom1Path, m_strPath);
            CheckData(ini, strSection, "Bottom2Path", Bottom2Path, m_strPath);
            CheckData(ini, strSection, "Mono1Path", Mono1Path, m_strPath);
            CheckData(ini, strSection, "Mono2Path", Mono2Path, m_strPath);
            CheckData(ini, strSection, "ServerPath", ServerPath, m_strPath);
            CheckData(ini, strSection, "VerifyPath", VerifyPath, m_strPath);

            strSection = "MES";
            CheckData(ini, strSection, "OnlineState", OnlineState.ToString(), m_strPath);
            CheckData(ini, strSection, "TimeoutReply", TimeoutReply.ToString(), m_strPath);
            CheckData(ini, strSection, "TimeoutTransaction", TimeoutTransaction.ToString(), m_strPath);
            CheckData(ini, strSection, "TimeoutConversation", TimeoutConversation.ToString(), m_strPath);

            CheckData(ini, strSection, "MachinePort", MachinePort.ToString(), m_strPath);
            CheckData(ini, strSection, "ServerPort", ServerPort.ToString(), m_strPath);

            CheckData(ini, strSection, "ServerIP", ServerIP, m_strPath);

            CheckData(ini, strSection, "Service", Service, m_strPath);
            CheckData(ini, strSection, "Network", Network, m_strPath);
            CheckData(ini, strSection, "Daemon", Daemon, m_strPath);
            CheckData(ini, strSection, "PubRvType", PubRvType.ToString(), m_strPath);
            CheckData(ini, strSection, "PubSubject", PubSubject, m_strPath);
            CheckData(ini, strSection, "PubTimeout", PubTimeout.ToString(), m_strPath);
            CheckData(ini, strSection, "SubRvType", SubRvType.ToString(), m_strPath);
            CheckData(ini, strSection, "SubSubject", SubSubject, m_strPath);

            strSection = "IPADDRESS";

            CheckData(ini, strSection, "TopIPAddress", TopIPAddress, m_strPath);
            CheckData(ini, strSection, "TopPort", TopPort.ToString(), m_strPath);
            CheckData(ini, strSection, "TopHPort", TopHPort.ToString(), m_strPath);

            CheckData(ini, strSection, "Top2IPAddress", Top2IPAddress, m_strPath);
            CheckData(ini, strSection, "Top2Port", Top2Port.ToString(), m_strPath);
            CheckData(ini, strSection, "Top2HPort", Top2HPort.ToString(), m_strPath);

            CheckData(ini, strSection, "BottomIPAddress", BottomIPAddress, m_strPath);
            CheckData(ini, strSection, "BottomPort", BottomPort.ToString(), m_strPath);
            CheckData(ini, strSection, "BottomHPort", BottomHPort.ToString(), m_strPath);

            CheckData(ini, strSection, "Bottom2IPAddress", Bottom2IPAddress, m_strPath);
            CheckData(ini, strSection, "Bottom2Port", Bottom2Port.ToString(), m_strPath);
            CheckData(ini, strSection, "Bottom2HPort", Bottom2HPort.ToString(), m_strPath);

            CheckData(ini, strSection, "MonoIPAddress", MonoIPAddress, m_strPath);
            CheckData(ini, strSection, "MonoPort", MonoPort.ToString(), m_strPath);
            CheckData(ini, strSection, "MonoHPort", MonoHPort.ToString(), m_strPath);

            CheckData(ini, strSection, "Mono2IPAddress", Mono2IPAddress, m_strPath);
            CheckData(ini, strSection, "Mono2Port", Mono2Port.ToString(), m_strPath);
            CheckData(ini, strSection, "Mono2HPort", Mono2HPort.ToString(), m_strPath);

            CheckData(ini, strSection, "ServerPCIPAddress", ServerPCIPAddress, m_strPath);
            CheckData(ini, strSection, "ServerPCPort", ServerPCPort.ToString(), m_strPath);
            CheckData(ini, strSection, "ServerPCHPort", ServerPCHPort.ToString(), m_strPath);

            CheckData(ini, strSection, "CamIPAddress", CamIPAddress, m_strPath);
            CheckData(ini, strSection, "CamPort", CamPort.ToString(), m_strPath);
            CheckData(ini, strSection, "CamHPort", CamHPort.ToString(), m_strPath);

            CheckData(ini, strSection, "NTDriveTop1", NTDriveTop1, m_strPath);
            CheckData(ini, strSection, "NTDriveTop2", NTDriveTop2, m_strPath);
            CheckData(ini, strSection, "NTDriveBottom1", NTDriveBottom1, m_strPath);
            CheckData(ini, strSection, "NTDriveBottom2", NTDriveBottom2, m_strPath);
            CheckData(ini, strSection, "NTDriveMono1", NTDriveMono1, m_strPath);
            CheckData(ini, strSection, "NTDriveMono2", NTDriveMono2, m_strPath);
            CheckData(ini, strSection, "NTDriveServer", NTDriveServer, m_strPath);

            CheckData(ini, strSection, "PublicFolderTop1", PublicFolderTop1, m_strPath);
            CheckData(ini, strSection, "PublicFolderTop2", PublicFolderTop2, m_strPath);
            CheckData(ini, strSection, "PublicFolderBottom1", PublicFolderBottom1, m_strPath);
            CheckData(ini, strSection, "PublicFolderBottom2", PublicFolderBottom2, m_strPath);
            CheckData(ini, strSection, "PublicFolderMono1", PublicFolderMono1, m_strPath);
            CheckData(ini, strSection, "PublicFolderMono2", PublicFolderMono2, m_strPath);
            CheckData(ini, strSection, "PublicFolderServer", PublicFolderServer, m_strPath);

            // REVIEW
            strSection = "REVIEW";
            CheckData(ini, strSection, "ReviewImageSize", ReviewImageSize.ToString(), m_strPath);
            CheckData(ini, strSection, "ReviewImageArray", ReviewImageArray.ToString(), m_strPath);
            CheckData(ini, strSection, "ReviewImageNumber", ReviewImageNumber.ToString(), m_strPath);
            CheckData(ini, strSection, "VerifyMethod", VerifyMethod.ToString(), m_strPath);

            for (int i = 0; i < 10; ++i)
            {
                CheckData(ini, strSection, "Code_" + i.ToString(), verifyCodes[i], m_strPath);
            }
            CheckData(ini, strSection, "IsUseVerifyCodes", IsUseVerifyCodes.ToString(), m_strPath);
            
            // TRIGGER
            strSection = "TRIGGER";
            CheckData(ini, strSection, "TriggerColor_Period", TriggerColor_Period.ToString(), m_strPath);
            CheckData(ini, strSection, "TriggerColor_Width", TriggerColor_Width.ToString(), m_strPath);
            CheckData(ini, strSection, "TriggerColor_Level", TriggerColor_Level.ToString(), m_strPath);

            CheckData(ini, strSection, "TriggerMono_Period", TriggerMono_Period.ToString(), m_strPath);
            CheckData(ini, strSection, "TriggerMono_Width", TriggerMono_Width.ToString(), m_strPath);
            CheckData(ini, strSection, "TriggerMono_Level", TriggerMono_Level.ToString(), m_strPath);

            // LASER
            strSection = "LASER";
            CheckData(ini, strSection, "Laser_X_Pos", LaserXpos.ToString(), m_strPath);
            CheckData(ini, strSection, "Laser_Y_Pos", LaserYpos.ToString(), m_strPath);

            #region OLD
            //ini.Write(section, "RecipeName", RecipeName, Path);
            //ini.Write(section, "MachineName", MachineName, Path);
            //ini.Write(section, "CamType", CamType, Path);
            //ini.Write(section, "LotID", LotID, Path);
            //ini.Write(section, "UserID", UserID, Path);
            //ini.Write(section, "ToolID", ToolID, Path);
            //ini.Write(section, "NextProcess", NextProcess, Path);
            //ini.Write(section, "JobType", JobType.ToString(), Path);
            //ini.Write(section, "TotalCount", TotalCount.ToString(), Path);
            //ini.Write(section, "GoodCount", GoodCount.ToString(), Path);

            //ini.Write(section, "LightTimeTop", LightTimeTop.ToString(), Path);
            //ini.Write(section, "LightTimeBottom", LightTimeBottom.ToString(), Path);
            //ini.Write(section, "LightTimeMono", LightTimeMono.ToString(), Path);

            //section = "USE";

            //ini.Write(section, "UseVirtualKeyboard", UseVirtualKeyboard.ToString(), Path);
            //ini.Write(section, "UseSelectParam", UseSelectParam.ToString(), Path);
            //ini.Write(section, "UseJobPermission", UseJobPermission.ToString(), Path);
            //ini.Write(section, "UseJobDoneBuzzer", UseJobDoneBuzzer.ToString(), Path);
            //ini.Write(section, "UseLightAlarmBuzzer", UseLightAlarmBuzzer.ToString(), Path);
            //ini.Write(section, "UseHeavyAlarmBuzzer", UseHeavyAlarmBuzzer.ToString(), Path);
            //ini.Write(section, "UseCheckGuide", UseCheckGuide.ToString(), Path);

            //ini.Write(section, "IsSelectedVision", IsSelectedVision.ToString(), Path);
            //ini.Write(section, "IsSelectedModify", IsSelectedModify.ToString(), Path);
            //ini.Write(section, "IsSelectedTop", IsSelectedTop.ToString(), Path);
            //ini.Write(section, "IsSelectedBottom", IsSelectedBottom.ToString(), Path);
            //ini.Write(section, "IsSelectedMono", IsSelectedMono.ToString(), Path);
            //ini.Write(section, "IsSelectedPunch", IsSelectedPunch.ToString(), Path);

            //ini.Write(section, "IsSelectedPunchInspect", IsSelectedPunchInspect.ToString(), Path);
            //ini.Write(section, "IsSelectedPunchTolerance", IsSelectedPunchTolerance.ToString(), Path);
            //ini.Write(section, "IsSelectedCNGPunch", IsSelectedCNGPunch.ToString(), Path);
            //ini.Write(section, "PunchInspectUnit", PunchInspectUnit.ToString(), Path);

            //ini.Write(section, "IsSelectedHoleStop", IsSelectedHoleStop.ToString(), Path);
            //ini.Write(section, "IsSelectedSectionYield", IsSelectedSectionYield.ToString(), Path);

            //ini.Write(section, "IsSelectedAlwaysAlign", IsSelectedAlwaysAlign.ToString(), Path);

            //ini.Write(section, "IsSelectedJointStop", IsSelectedJointStop.ToString(), Path);
            //ini.Write(section, "IsSelectedCleanRoller", IsSelectedCleanRoller.ToString(), Path);
            //ini.Write(section, "IsSelectedMissPrint", IsSelectedMissPrint.ToString(), Path);
            //ini.Write(section, "IsSelectedDualErrorStop", IsSelectedDualErrorStop.ToString(), Path);
            //ini.Write(section, "IsSelectedBackFeeding", IsSelectedBackFeeding.ToString(), Path);

            //section = "MACHINE";
            //ini.Write(section, "LogSave", LogSave.ToString(), Path);
            //ini.Write(section, "ResultSave", ResultSave.ToString(), Path);
            //ini.Write(section, "ReportSave", ReportSave.ToString(), Path);
            //ini.Write(section, "ReportPeriod", ReportPeriod.ToString(), Path);
            //ini.Write(section, "BarcodeCOMPort", BarcodeCOMPort.ToString(), Path);
            //ini.Write(section, "BottomDistance", BottomDistance.ToString(), Path);
            //ini.Write(section, "MonoDistance", MonoDistance.ToString(), Path);
            //ini.Write(section, "PunchDistance", PunchDistance.ToString(), Path);
            //ini.Write(section, "PosBackFeeding", PosBackFeeding.ToString(), Path);
            ////ini.Write(section, "Velocity", Velocity.ToString(), Path);
            ////ini.Write(section, "AccelTime", AccelTime.ToString(), Path);
            ////ini.Write(section, "JogVelocity", JogVelocity.ToString(), Path);
            ////ini.Write(section, "JogAccelTime", JogAccelTime.ToString(), Path);
            ////ini.Write(section, "SupportReference", SupportReference.ToString(), Path);
            ////ini.Write(section, "CollectReference", CollectReference.ToString(), Path);

            //ini.Write(section, "UncoilerReference", UncoilerReference.ToString(), Path);
            //ini.Write(section, "RecoilerReference", RecoilerReference.ToString(), Path);
            //ini.Write(section, "BufferReference", BufferReference.ToString(), Path);
            //ini.Write(section, "BufferLimitN", BufferLimitN.ToString(), Path);
            //ini.Write(section, "BufferLimitP", BufferLimitP.ToString(), Path);
            //ini.Write(section, "PunchStroke", PunchStroke.ToString(), Path);

            //ini.Write(section, "PunchOffsetX", PunchOffsetX.ToString(), Path);
            //ini.Write(section, "PunchOffsetY", PunchOffsetY.ToString(), Path);
            //ini.Write(section, "PunchOffsetBX", PunchOffsetBX.ToString(), Path);
            //ini.Write(section, "PunchOffsetBY", PunchOffsetBY.ToString(), Path);
            //ini.Write(section, "PunchOffsetCenterX", PunchOffsetCenterX.ToString(), Path);
            //ini.Write(section, "PunchOffsetCenterY", PunchOffsetCenterY.ToString(), Path);
            //ini.Write(section, "AlignVisionX", AlignVisionX.ToString(), Path);
            //ini.Write(section, "AlignVisionY", AlignVisionY.ToString(), Path);
            //ini.Write(section, "TopVisionZ", TopVisionZ.ToString(), Path);
            //ini.Write(section, "BottomVisionZ", BottomVisionZ.ToString(), Path);

            //ini.Write(section, "JobDoneBuzzerTime", JobDoneBuzzerTime.ToString(), Path);
            //ini.Write(section, "LightAlarmBuzzerTime", LightAlarmBuzzerTime.ToString(), Path);
            //ini.Write(section, "HeavyAlarmBuzzerTime", HeavyAlarmBuzzerTime.ToString(), Path);
            //ini.Write(section, "DelayPunch", DelayPunch.ToString(), Path);
            //ini.Write(section, "DelayFeeding", DelayFeeding.ToString(), Path);

            //ini.Write(section, "CalX", CalX.ToString(), Path);
            //ini.Write(section, "CalY", CalY.ToString(), Path);

            //ini.Write(section, "ScanTolerance", ScanTolerance.ToString(), Path);
            //ini.Write(section, "ImageTolerance", ImageTolerance.ToString(), Path);
            //ini.Write(section, "MotionTolerance", MotionTolerance.ToString(), Path);
            //ini.Write(section, "IndexTolerance", IndexTolerance.ToString(), Path);

            //ini.Write(section, "ScanDummyTop", ScanDummyTop.ToString(), Path);
            //ini.Write(section, "ScanDummyBottom", ScanDummyBottom.ToString(), Path);
            //ini.Write(section, "ScanDummyMono", ScanDummyMono.ToString(), Path);

            //ini.Write(section, "PunchCount", PunchCount.ToString(), Path);
            //ini.Write(section, "PunchCountLimit", PunchCountLimit.ToString(), Path);
            //ini.Write(section, "PunchImageLimit", PunchImageLimit.ToString(), Path);

            //ini.Write(section, "SectionMinUnits", SectionMinUnits.ToString(), Path);

            //ini.Write(section, "MachineID", MachineID, Path);
            //ini.Write(section, "ProcessID", ProcessID, Path);
            //ini.Write(section, "LogPath", LogPath, Path);
            //ini.Write(section, "ResultPath", ResultPath, Path);
            //ini.Write(section, "ReportPath", ReportPath, Path);
            //ini.Write(section, "PunchPath", PunchPath, Path);

            //ini.Write(section, "Top1Path", Top1Path, Path);
            //ini.Write(section, "Top2Path", Top2Path, Path);
            //ini.Write(section, "Bottom1Path", Bottom1Path, Path);
            //ini.Write(section, "Bottom2Path", Bottom2Path, Path);
            //ini.Write(section, "Mono1Path", Mono1Path, Path);
            //ini.Write(section, "Mono2Path", Mono2Path, Path);
            //ini.Write(section, "ServerPath", ServerPath, Path);
            //ini.Write(section, "VerifyPath", VerifyPath, Path);

            //section = "MES";
            //ini.Write(section, "OnlineState", OnlineState.ToString(), Path);
            //ini.Write(section, "TimeoutReply", TimeoutReply.ToString(), Path);
            //ini.Write(section, "TimeoutTransaction", TimeoutTransaction.ToString(), Path);
            //ini.Write(section, "TimeoutConversation", TimeoutConversation.ToString(), Path);

            //ini.Write(section, "MachinePort", MachinePort.ToString(), Path);
            //ini.Write(section, "ServerPort", ServerPort.ToString(), Path);

            //ini.Write(section, "ServerIP", ServerIP, Path);

            ////ini.Write(section, "CurrentModel", CurrentModel, Path);

            //ini.Write(section, "Service", Service, Path);
            //ini.Write(section, "Network", Network, Path);
            //ini.Write(section, "Daemon", Daemon, Path);
            //ini.Write(section, "PubRvType", PubRvType.ToString(), Path);
            //ini.Write(section, "PubSubject", PubSubject, Path);
            //ini.Write(section, "PubTimeout", PubTimeout.ToString(), Path);
            //ini.Write(section, "SubRvType", SubRvType.ToString(), Path);
            //ini.Write(section, "SubSubject", SubSubject, Path);

            //section = "IPADDRESS";

            //ini.Write(section, "TopIPAddress", TopIPAddress, Path);
            //ini.Write(section, "TopPort", TopPort.ToString(), Path);
            //ini.Write(section, "TopHPort", TopHPort.ToString(), Path);

            //ini.Write(section, "Top2IPAddress", Top2IPAddress, Path);
            //ini.Write(section, "Top2Port", Top2Port.ToString(), Path);
            //ini.Write(section, "Top2HPort", Top2HPort.ToString(), Path);

            //ini.Write(section, "BottomIPAddress", BottomIPAddress, Path);
            //ini.Write(section, "BottomPort", BottomPort.ToString(), Path);
            //ini.Write(section, "BottomHPort", BottomHPort.ToString(), Path);

            //ini.Write(section, "Bottom2IPAddress", Bottom2IPAddress, Path);
            //ini.Write(section, "Bottom2Port", Bottom2Port.ToString(), Path);
            //ini.Write(section, "Bottom2HPort", Bottom2HPort.ToString(), Path);

            //ini.Write(section, "MonoIPAddress", MonoIPAddress, Path);
            //ini.Write(section, "MonoPort", MonoPort.ToString(), Path);
            //ini.Write(section, "MonoHPort", MonoHPort.ToString(), Path);

            //ini.Write(section, "Mono2IPAddress", Mono2IPAddress, Path);
            //ini.Write(section, "Mono2Port", Mono2Port.ToString(), Path);
            //ini.Write(section, "Mono2HPort", Mono2HPort.ToString(), Path);

            //ini.Write(section, "ServerPCIPAddress", ServerPCIPAddress, Path);
            //ini.Write(section, "ServerPCPort", ServerPCPort.ToString(), Path);
            //ini.Write(section, "ServerPCHPort", ServerPCHPort.ToString(), Path);

            //ini.Write(section, "CamIPAddress", CamIPAddress, Path);
            //ini.Write(section, "CamPort", CamPort.ToString(), Path);
            //ini.Write(section, "CamHPort", CamHPort.ToString(), Path);

            //ini.Write(section, "NTDriveTop1", NTDriveTop1, Path);
            //ini.Write(section, "NTDriveTop2", NTDriveTop2, Path);
            //ini.Write(section, "NTDriveBottom1", NTDriveBottom1, Path);
            //ini.Write(section, "NTDriveBottom2", NTDriveBottom2, Path);
            //ini.Write(section, "NTDriveMono1", NTDriveMono1, Path);
            //ini.Write(section, "NTDriveMono2", NTDriveMono2, Path);
            //ini.Write(section, "NTDriveServer", NTDriveServer, Path);

            //ini.Write(section, "PublicFolderTop1", PublicFolderTop1, Path);
            //ini.Write(section, "PublicFolderTop2", PublicFolderTop2, Path);
            //ini.Write(section, "PublicFolderBottom1", PublicFolderBottom1, Path);
            //ini.Write(section, "PublicFolderBottom2", PublicFolderBottom2, Path);
            //ini.Write(section, "PublicFolderMono1", PublicFolderMono1, Path);
            //ini.Write(section, "PublicFolderMono2", PublicFolderMono2, Path);
            //ini.Write(section, "PublicFolderServer", PublicFolderServer, Path);

            //// REVIEW
            //section = "REVIEW";
            //ini.Write(section, "ReviewImageSize", ReviewImageSize.ToString(), Path);
            //ini.Write(section, "ReviewImageArray", ReviewImageArray.ToString(), Path);
            //ini.Write(section, "ReviewImageNumber", ReviewImageNumber.ToString(), Path);
            //ini.Write(section, "VerifyMethod", VerifyMethod.ToString(), Path);

            //for (int i = 0; i < 10; ++i)
            //{
            //    ini.Write(section, "Code_" + i.ToString(), verifyCodes[i], Path);
            //}


            //// TRIGGER
            //section = "TRIGGER";
            //ini.Write(section, "TriggerColor_Period", TriggerColor_Period.ToString(), Path);
            //ini.Write(section, "TriggerColor_Width", TriggerColor_Width.ToString(), Path);
            //ini.Write(section, "TriggerColor_Level", TriggerColor_Level.ToString(), Path);

            //ini.Write(section, "TriggerMono_Period", TriggerMono_Period.ToString(), Path);
            //ini.Write(section, "TriggerMono_Width", TriggerMono_Width.ToString(), Path);
            //ini.Write(section, "TriggerMono_Level", TriggerMono_Level.ToString(), Path);
            #endregion
        }

        public void OffSetSave()
        {
            IniFile ini = new IniFile();
            string section = "OffSet";
            CheckData(ini, section, "AlignPosOffsetX", AlignPosOffsetX.ToString(), m_strPath);
            CheckData(ini, section, "AlignPosOffsetY", AlignPosOffsetY.ToString(), m_strPath);
        }

        private void CheckData(IniFile ini, string section, string key, string value, string path)
        {
            string read = "";

            read = ini.Read(section, key, "0", path);
            if (read != value)
            {
                Log_History.WriteLine("{0,-30},{1,-30},{2,-30},{3,-30}", "[DataSystem]", key, read, value);
                ini.Write(section, key, value, path);
            }
        }
    }
}
