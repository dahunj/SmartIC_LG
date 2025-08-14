using System;
using System.IO;
using System.Text;
using System.Windows;

namespace SmartICAVI
{
    class DataService
    {
        #region Properties
        public DataSystem DataSystem { get; set; }
        public DataMotion DataMotion { get; set; }
        public DataRecipe DataRecipe { get; set; }
        public DataResult DataResult { get; set; }
        public DataDefect DataDefect { get; set; }
        public DataDefectInfo DataDefectInfo { get; set; }

        public DataResultTemp DataTempTop { get; set; }
        public DataResultTemp DataTempBottom { get; set; }
        public DataResultTemp DataTempMono { get; set; }

        public DataUserAccounts DataUserAccounts { get; set; }

        public DataRecipe DataTeach { get; set; }
        public string TeachName { get; set; }

        public double JobInitOffset { get; set; }   // Start 위치 옵셋
        public int JobInitUnit { get; set; }      // 처음 Skip Unit 갯수
        public bool IsJobDone { get; set; }  // Job End 시 true
        public bool IsEndTop { get; set; }   // Top Vision End 시 true
        public bool IsEndBottom { get; set; }
        public bool IsEndMono { get; set; }

        public bool IsVisionPaused { get; set; }

        public int IndexEndTop { get; set; }  // Top Vision End 시의 Index 값
        public int IndexEndBottom { get; set; }  // Top Vision End 시의 Index 값

        public string CurrentStatus { get; set; }
        public string ServerMapPath { get; set; }   // map data 서버전송 Path
        public string LocalMapPathName { get; set; }    // Local Map Data 저장파일

        public bool IsBackFeeding { get; set; }   // 재검사 시 BackFeeding 시 true : Pause ->Resume 할 때 이 값이 true 이면 Resume 에서 재 피딩 하지 않고, BackFeedProcess 에서 처리한다.


        public bool IsOnlineMode
        {
            get
            {
                // 1: Offline, 2:Local, 3:Remote
                if (1 < DataSystem.OnlineState)
                    return true;

                return false;
            }
        }

        public bool IsUncoilerRun { get; set; }
        public bool IsUncoilerReady { get; set; }
        public bool IsRecoilerRun { get; set; }
        public bool IsRecoilerReady { get; set; }

         
        public bool IsLightOnTop { get; set; }
        public bool IsLightOnBottom { get; set; }
        public bool IsLightOnMono { get; set; }

        private bool isInitialized;
        public bool IsInitialized { get { return isInitialized; } }

        private string currentPath="";
        public string CurrentPath { get { return currentPath; } }

        private string systemFilePath;
        public string SystemFilePath { get{ return systemFilePath; } }

        private string dataPath;
        public string DataPath { get{ return dataPath; } }

        private string recipePath;
        public string RecipePath { get { return recipePath; } }

        private string iniPath;
        public string IniPath { get { return iniPath; } }

        private string logTracePath;
        public string LogTracePath { get { return logTracePath; } }

        private string logLastPunchCheckPath;
        public string LogLastPunchCheckPath { get { return logLastPunchCheckPath; } }

        private string logErrorPath;
        public string LogErrorPath { get { return logErrorPath; } }

        private string logHistoryPath;
        public string LogHistoryPath { get { return logHistoryPath; } }

        private string logMesPath;
        public string LogMesPath { get { return logMesPath; } }

        private string logExceptionPath;
        public string LogExceptionPath { get { return logExceptionPath; } }

        private string logTopVisionPath;
        public string LogTopVisionPath { get { return logTopVisionPath; } }

        private string logTopVision2Path;
        public string LogTopVision2Path { get { return logTopVision2Path; } }

        private string logBottomVisionPath;
        public string LogBottomVisionPath { get { return logBottomVisionPath; } }

        private string logBottomVision2Path;
        public string LogBottomVision2Path { get { return logBottomVision2Path; } }

        private string logMonoVisionPath;
        public string LogMonoVisionPath { get { return logMonoVisionPath; } }

        private string logMonoVision2Path;
        public string LogMonoVision2Path { get { return logMonoVision2Path; } }

        private string logCamPath;
        public string LogCamPath { get { return logCamPath; } }

        private string logSeqPunchPath;

        public double FeedVelocity { get; set; }

        public string LogonID { get; set; }
        public int LogonType { get; set; }
        public DateTime LogonTime { get; set; }

        public bool IsLogOn
        {
            get
            {
                TimeSpan duration = new TimeSpan(0, 0, 0, 0, 200);
                DateTime timeout = LogonTime.Add(duration);

                if (timeout < DateTime.Now)
                    return false;
                else
                    return true;
            }
        }


        #endregion

        #region EVENTs
        public event EventHandler EventChangedRecipe;

        #endregion


        #region Singleton
        private static DataService singleton = null;

        public static DataService Singleton
        {
            get
            {
                if (null == singleton)
                {
                    singleton = new DataService();
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

        public DataService()
        {
            isInitialized = false;

            IsVisionPaused = false;

            DataSystem = new DataSystem();
            DataMotion = new DataMotion();
            DataRecipe = new DataRecipe();
            DataResult = new DataResult();
            DataDefect = new DataDefect();
            DataDefectInfo = new DataDefectInfo();

            DataTempTop = new DataResultTemp();
            DataTempBottom = new DataResultTemp();
            DataTempMono = new DataResultTemp();

            DataTeach = new DataRecipe();

            DataUserAccounts = new DataUserAccounts();

            JobInitOffset = 0.0;
            JobInitUnit = 0;

            IsEndTop = false;
            IndexEndTop = 0;

            IsEndBottom = false;
            IndexEndBottom = 0;

            IsEndMono = false;

            IsJobDone = false;

            IsBackFeeding = false;

            CurrentStatus = "Power On";
            ServerMapPath = "";
            LocalMapPathName = "";


            IsLightOnTop = false;
            IsLightOnBottom = false;
            IsLightOnMono = false;

            IsUncoilerRun = false;
            IsUncoilerReady = false;
            IsRecoilerRun = false;
            IsRecoilerReady = false;
        }

        public int Initialize()
        {
            LogonTime = new DateTime(0);

            LogonID = "";
            LogonType = 0;

            currentPath = Directory.GetCurrentDirectory();

            //AppDomain.CurrentDomain.BaseDirectory
            //System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)


            dataPath = CurrentPath + "\\Data";
            recipePath = CurrentPath + "\\Recipe";
            iniPath = DataPath + "\\Ini";

            systemFilePath = iniPath + "\\System.ini";

            string defectInfoPath = iniPath + "\\DefectInfo.ini";

            
            // Check Data Directory
            if (false == Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }

            // Check Recipe Directory
            if (false == Directory.Exists(recipePath))
            {
                Directory.CreateDirectory(recipePath);
            }
            
            // Check Ini Directory
            if (false == Directory.Exists(iniPath))
            {
                Directory.CreateDirectory(iniPath);
            }


            // Load System Data
            DataSystem.Load(systemFilePath);

            DataDefectInfo.Load(defectInfoPath);

            
            // Check Log Path
            if (false == Directory.Exists(DataSystem.LogPath))
            {
                Directory.CreateDirectory(DataSystem.LogPath);
            }

            logTracePath = DataSystem.LogPath + "\\Trace";
            logLastPunchCheckPath = DataSystem.LogPath + "\\LastPunchCheck";
            logErrorPath = DataSystem.LogPath + "\\Error";
            logHistoryPath = DataSystem.LogPath + "\\History";
            logMesPath = DataSystem.LogPath + "\\Mes";
            logExceptionPath = DataSystem.LogPath + "\\Exception";

            logTopVisionPath = DataSystem.LogPath + "\\TopVision";
            logTopVision2Path = DataSystem.LogPath + "\\TopVision2";
            logBottomVisionPath = DataSystem.LogPath + "\\BottomVision";
            logBottomVision2Path = DataSystem.LogPath + "\\BottomVision2";
            logMonoVisionPath = DataSystem.LogPath + "\\MonoVision";
            logMonoVision2Path = DataSystem.LogPath + "\\MonoVision2";
            logCamPath = DataSystem.LogPath + "\\CamVision";

            logSeqPunchPath = DataSystem.LogPath + "\\SeqPunch";

            string pathTop = DataSystem.LogPath + "\\Top";
            string pathBottom = DataSystem.LogPath + "\\Bottom";
            string pathMono = DataSystem.LogPath + "\\Mono";

            string pathTemp = DataSystem.LogPath + "\\Temp";
            string pathReview = DataSystem.LogPath + "\\Review";

            string pathDebug = DataSystem.LogPath + "\\Debug";


            // Trace
            if (false == Directory.Exists(LogTracePath))
            {
                Directory.CreateDirectory(LogTracePath);
            }
            if (false == Directory.Exists(LogTracePath + "\\Old"))
            {
                Directory.CreateDirectory(LogTracePath + "\\Old");
            }
            // LastPunchCheck
            if (false == Directory.Exists(LogLastPunchCheckPath))
            {
                Directory.CreateDirectory(LogLastPunchCheckPath);
            }
            if (false == Directory.Exists(LogLastPunchCheckPath + "\\Old"))
            {
                Directory.CreateDirectory(LogLastPunchCheckPath + "\\Old");
            }
            // Error/Alarm
            if (false == Directory.Exists(LogErrorPath))
            {
                Directory.CreateDirectory(LogErrorPath);
            }
            if (false == Directory.Exists(LogErrorPath + "\\Old"))
            {
                Directory.CreateDirectory(LogErrorPath + "\\Old");
            }
            // History
            if (false == Directory.Exists(LogHistoryPath))
            {
                Directory.CreateDirectory(LogHistoryPath);
            }
            if (false == Directory.Exists(LogHistoryPath + "\\Old"))
            {
                Directory.CreateDirectory(LogHistoryPath + "\\Old");
            }
            // Mes
            if (false == Directory.Exists(LogMesPath))
            {
                Directory.CreateDirectory(LogMesPath);
            }
            if (false == Directory.Exists(LogMesPath + "\\Old"))
            {
                Directory.CreateDirectory(LogMesPath + "\\Old");
            }
            // Exception
            if (false == Directory.Exists(LogExceptionPath))
            {
                Directory.CreateDirectory(LogExceptionPath);
            }
            if (false == Directory.Exists(LogExceptionPath + "\\Old"))
            {
                Directory.CreateDirectory(LogExceptionPath + "\\Old");
            }

            

            // TopVision
            if (false == Directory.Exists(LogTopVisionPath))
            {
                Directory.CreateDirectory(LogTopVisionPath);
            }
            if (false == Directory.Exists(LogTopVisionPath + "\\Old"))
            {
                Directory.CreateDirectory(LogTopVisionPath + "\\Old");
            }

            // TopVision2
            if (false == Directory.Exists(LogTopVision2Path))
            {
                Directory.CreateDirectory(LogTopVision2Path);
            }
            if (false == Directory.Exists(LogTopVision2Path + "\\Old"))
            {
                Directory.CreateDirectory(LogTopVision2Path + "\\Old");
            }

            // BottomVision
            if (false == Directory.Exists(LogBottomVisionPath))
            {
                Directory.CreateDirectory(LogBottomVisionPath);
            }
            if (false == Directory.Exists(LogBottomVisionPath + "\\Old"))
            {
                Directory.CreateDirectory(LogBottomVisionPath + "\\Old");
            }
            // BottomVision2
            if (false == Directory.Exists(LogBottomVision2Path))
            {
                Directory.CreateDirectory(LogBottomVision2Path);
            }
            if (false == Directory.Exists(LogBottomVision2Path + "\\Old"))
            {
                Directory.CreateDirectory(LogBottomVision2Path + "\\Old");
            }

            // MonoVision
            if (false == Directory.Exists(LogMonoVisionPath))
            {
                Directory.CreateDirectory(LogMonoVisionPath);
            }
            if (false == Directory.Exists(LogMonoVisionPath + "\\Old"))
            {
                Directory.CreateDirectory(LogMonoVisionPath + "\\Old");
            }
            // MonoVision2
            if (false == Directory.Exists(LogMonoVision2Path))
            {
                Directory.CreateDirectory(LogMonoVision2Path);
            }
            if (false == Directory.Exists(LogMonoVision2Path + "\\Old"))
            {
                Directory.CreateDirectory(LogMonoVision2Path + "\\Old");
            }

            // Top
            if (false == Directory.Exists(pathTop))
            {
                Directory.CreateDirectory(pathTop);
            }
            if (false == Directory.Exists(pathTop + "\\Old"))
            {
                Directory.CreateDirectory(pathTop + "\\Old");
            }

            // Bottom
            if (false == Directory.Exists(pathBottom))
            {
                Directory.CreateDirectory(pathBottom);
            }
            if (false == Directory.Exists(pathBottom + "\\Old"))
            {
                Directory.CreateDirectory(pathBottom + "\\Old");
            }

            // Mono
            if (false == Directory.Exists(pathMono))
            {
                Directory.CreateDirectory(pathMono);
            }
            if (false == Directory.Exists(pathMono + "\\Old"))
            {
                Directory.CreateDirectory(pathMono + "\\Old");
            }

            // Cam
            if (false == Directory.Exists(logCamPath))
            {
                Directory.CreateDirectory(logCamPath);
            }
            if (false == Directory.Exists(logCamPath + "\\Old"))
            {
                Directory.CreateDirectory(logCamPath + "\\Old");
            }

            // SeqPunch
            if (false == Directory.Exists(logSeqPunchPath))
            {
                Directory.CreateDirectory(logSeqPunchPath);
            }
            if (false == Directory.Exists(logSeqPunchPath + "\\Old"))
            {
                Directory.CreateDirectory(logSeqPunchPath + "\\Old");
            }


            // Temp
            if (false == Directory.Exists(pathTemp))
            {
                Directory.CreateDirectory(pathTemp);
            }
            if (false == Directory.Exists(pathTemp + "\\Old"))
            {
                Directory.CreateDirectory(pathTemp + "\\Old");
            }

            // Review
            if (false == Directory.Exists(pathReview))
            {
                Directory.CreateDirectory(pathReview);
            }
            if (false == Directory.Exists(pathReview + "\\Old"))
            {
                Directory.CreateDirectory(pathReview + "\\Old");
            }

            // Check Punch Directory
            if (false == Directory.Exists(DataSystem.PunchPath))
            {
                Directory.CreateDirectory(DataSystem.PunchPath);
            }


            // Check Result Path
            if (false == Directory.Exists(DataSystem.ResultPath))
            {
                Directory.CreateDirectory(DataSystem.ResultPath);
            }
            if (false == Directory.Exists(DataSystem.ResultPath + "\\Old"))
            {
                Directory.CreateDirectory(DataSystem.ResultPath + "\\Old");
            }

            // Check Report Path
            if (false == Directory.Exists(DataSystem.ReportPath))
            {
                Directory.CreateDirectory(DataSystem.ReportPath);
            }


            // Debug
            if (false == Directory.Exists(pathDebug))
            {
                Directory.CreateDirectory(pathDebug);
            }
            if (false == Directory.Exists(pathDebug + "\\Old"))
            {
                Directory.CreateDirectory(pathDebug + "\\Old");
            }



            Log_Trace.Type = 2;
            Log_Trace.Path = LogTracePath;
            Log_Trace.SaveDays = DataSystem.LogSave;
            Log_Trace.Open();
            Log_Trace.WriteLine("DataService Initialze");

            // LastPunchCheck Log
            Log_LastPunchCheck.Type = 2;
            Log_LastPunchCheck.Path = LogLastPunchCheckPath;
            Log_LastPunchCheck.SaveDays = DataSystem.LogSave;
            Log_LastPunchCheck.Open();
            //Log_LastPunchCheck.WriteLine("DataService Initialze");

            // Error Log
            Log_Error.Type = 2;
            Log_Error.Path = LogErrorPath;
            Log_Error.SaveDays = DataSystem.LogSave;
            Log_Error.Open();

            // History Log
            Log_History.Type = 2;
            Log_History.Path = LogHistoryPath;
            Log_History.SaveDays = DataSystem.LogSave;
            Log_History.Open();

            // Mes Log
            Log_Mes.Type = 2;
            Log_Mes.Path = LogMesPath;
            Log_Mes.SaveDays = DataSystem.LogSave;
            Log_Mes.Open();

            // Exception Log
            Log_Exception.Type = 2;
            Log_Exception.Path = LogExceptionPath;
            Log_Exception.SaveDays = DataSystem.LogSave;
            Log_Exception.Open();

            // 0:Run, 1:Hour, 2:Day
            // TopVision Log
            Log_TopVision.Type = 1;
            Log_TopVision.Path = LogTopVisionPath;
            Log_TopVision.SaveDays = DataSystem.LogSave;
            Log_TopVision.Open();
            // TopVision2 Log
            Log_TopVision2.Type = 1;
            Log_TopVision2.Path = LogTopVision2Path;
            Log_TopVision2.SaveDays = DataSystem.LogSave;
            Log_TopVision2.Open();
            // BottomVision Log
            Log_BottomVision.Type = 1;
            Log_BottomVision.Path = LogBottomVisionPath;
            Log_BottomVision.SaveDays = DataSystem.LogSave;
            Log_BottomVision.Open();
            // BottomVision2 Log
            Log_BottomVision2.Type = 1;
            Log_BottomVision2.Path = LogBottomVision2Path;
            Log_BottomVision2.SaveDays = DataSystem.LogSave;
            Log_BottomVision2.Open();
            // MonoVision Log
            Log_MonoVision.Type = 1;
            Log_MonoVision.Path = LogMonoVisionPath;
            Log_MonoVision.SaveDays = DataSystem.LogSave;
            Log_MonoVision.Open();
            // MonoVision2 Log
            Log_MonoVision2.Type = 1;
            Log_MonoVision2.Path = LogMonoVision2Path;
            Log_MonoVision2.SaveDays = DataSystem.LogSave;
            Log_MonoVision2.Open();

            // Cam Log
            Log_Cam.Type = 2;
            Log_Cam.Path = LogCamPath;
            Log_Cam.SaveDays = DataSystem.LogSave;
            Log_Cam.Open();

            // Top Log
            Log_Top.Type = 1;
            Log_Top.Path = pathTop;
            Log_Top.SaveDays = DataSystem.LogSave;
            Log_Top.Open();
            // Bottom Log
            Log_Bottom.Type = 1;
            Log_Bottom.Path = pathBottom;
            Log_Bottom.SaveDays = DataSystem.LogSave;
            Log_Bottom.Open();
            // Mono Log
            Log_Mono.Type = 1;
            Log_Mono.Path = pathMono;
            Log_Mono.SaveDays = DataSystem.LogSave;
            Log_Mono.Open();
            // Temp Log
            Log_Temp.Type = 1;
            Log_Temp.Path = pathTemp;
            Log_Temp.SaveDays = DataSystem.LogSave;
            Log_Temp.Open();
            // Review Log
            Log_Review.Type = 1;
            Log_Review.Path = pathReview;
            Log_Review.SaveDays = DataSystem.LogSave;
            Log_Review.Open();

            // Debug Log
            Log_Debug.Type = 1;
            Log_Debug.Path = pathDebug;
            Log_Debug.SaveDays = DataSystem.LogSave;
            Log_Debug.Open();

            //// SeqPunch Log
            //Log_SeqPunch.Type = 1;
            //Log_SeqPunch.Path = logSeqPunchPath;
            //Log_SeqPunch.SaveDays = DataSystem.LogSave;
            //Log_SeqPunch.Open();



            // Load Motion Data
            if( null != DataMotion )
                DataMotion.Load(iniPath + "\\Motion.ini");

            //if( null != DataRecipe )
            //{
            //    if( 0 != DataRecipe.Load(recipePath + "\\" + RecipePath + ".rcp") )
            //}
            DataSystem.UseSelectParam = false;


            DataUserAccounts.Load(iniPath + "\\UserAccount.ini");

            isInitialized = true;

            //DataSystem.MachineName = "SmartIC-AVI";
            //DataSystem.MachineName = "Virtual";

            if ("Virtual" == DataSystem.MachineName)
            {
                MessageBox.Show("Virtual Mode");
            }

            FeedVelocity = DataMotion[1].VelNormal;
            return 0;
        }

        public int UnInitialize()
        {
            //Log_SeqPunch.Close();

            // Save Motion Data
            if (null != DataMotion)
                DataMotion.Save(iniPath + "\\Motion.ini");

            if (null != DataSystem)
                DataSystem.Save();

            if (null != DataRecipe)
                DataRecipe.Dispose();

            if (null != DataDefectInfo)
                DataDefectInfo.Save();

            isInitialized = false;
            return 0;
        }

        public int LoadLastRecipe()
        {
            if (null == DataRecipe)
                return -1;

            if( null == DataSystem )
                return -1;

            if (0 == DataRecipe.Load(recipePath + "\\" + DataSystem.RecipeName + ".rcp"))
            {
                if (null != EventChangedRecipe)
                    EventChangedRecipe(this, null);

                return 0;
            }

            return -1;
        }

        public int SaveRecipe(string name)
        {
            if (null == DataRecipe)
                return -1;

            if (null == DataSystem)
                return -1;

            string path = RecipePath + "\\" + name + ".rcp";

            if (0 == DataRecipe.Save(path))
            {
                DataSystem.RecipeName = name;
                return 0;
            }

            return -1;
        }

        public int LoadRecipe(string name)
        {
            if (null == DataRecipe)
                return -1;

            if (null == DataSystem)
                return -1;

            string path = RecipePath + "\\" + name + ".rcp";

            if (0 == DataRecipe.Load(path))
            {
                DataSystem.RecipeName = name;

                if( null != EventChangedRecipe )
                    EventChangedRecipe(this, null);
                return 0;
            }

            return -1;
        }

        // start 서부터 end 까지의 Review 데이터를 모두 삭제한다. index~.dat -- 해당 이미지들
        public int DeleteReviewData(int indexStart, int indexEnd)
        {
            // Top1
            DeleteReviewData(indexStart, indexEnd, DataSystem.Top1Path);

            // Top2
            DeleteReviewData(indexStart, indexEnd, DataSystem.Top2Path);

            // Bottom1
            DeleteReviewData(indexStart, indexEnd, DataSystem.Bottom1Path);

            // Bottom2
            DeleteReviewData(indexStart, indexEnd, DataSystem.Bottom2Path);

            // Mono1
            DeleteReviewData(indexStart, indexEnd, DataSystem.Mono1Path);

            // Mono2
            DeleteReviewData(indexStart, indexEnd, DataSystem.Mono2Path);

            return 0;
        }

        private int DeleteReviewData(int indexStart, int indexEnd, string path)
        {
            int pathLength = path.Length;

            if ('\\' != path[pathLength - 1])
                path += "\\";

            if ("" == DataResult.LotID)
                return -1;

            string fileName = "";
            path += DataResult.LotID;

            for (int i = indexStart; i < indexEnd; ++i)
            {
                fileName = string.Format("index{0:000000}_A.dat", i + 1);
                DeleteReviewData(path, fileName);

                fileName = string.Format("index{0:000000}_B.dat", i + 1);
                DeleteReviewData(path, fileName);
            }

            return 0;
        }

        private int DeleteReviewData(string path, string fileName)
        {
            FileStream stream;
            StreamReader reader;
            string item;

            string pathData = path + "\\" + fileName;
            string pathImage = "";

            if (false == File.Exists(pathData))
                return -1;

            try
            {
                stream = File.OpenRead(pathData);
                reader = new StreamReader(stream, Encoding.Default);

                string[] arrays = new string[10];

                while (true)
                {
                    item = reader.ReadLine();

                    if ("" == item)
                        item = reader.ReadLine();

                    if (null == item)
                        break;

                    if (("EOF" == item) || "" == item)
                        break;
                    else
                    {
                        arrays = item.Split(',');

                        if (10 < arrays.Length)
                        {
                            //defectID = arrays[0];
                            //defectName = arrays[1];
                            //area = arrays[2];
                            //length = arrays[3];
                            //sizeX = arrays[4];
                            //sizeY = arrays[5];
                            //posX = arrays[6];
                            //posY = arrays[7];

                            // Color Image
                            if ("" != arrays[8])
                            {
                                pathImage = path + "\\" + arrays[8];
                                File.Delete(pathImage);
                            }
                            // HSI Image
                            if ("" != arrays[9])
                            {
                                pathImage = path + "\\" + arrays[9];
                                File.Delete(pathImage);
                            }

                            // Big Image
                            if ("" != arrays[10])
                            {
                                pathImage = path + "\\" + arrays[10];
                                File.Delete(pathImage);
                            }
                        }

                    }
                }//while (true)
                reader.Close();
                stream.Close();

                File.Delete(pathData);
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine("DataService.DeleteReviewData() : " + exc.Message);
            }
            return 0;
        }

        public void AddLightTime()
        {
            if( null == DataSystem )
                return;

            //TimeSpan duration = new TimeSpan(0, 0, 0, 0, mSec);
            //DateTime timeout = start.Add(duration);

            if (true == IsLightOnTop)
                DataSystem.LightTimeTop.Add(new TimeSpan(0, 0, 1));
            if (true == IsLightOnBottom)
                DataSystem.LightTimeBottom.Add(new TimeSpan(0, 0, 1));
            if (true == IsLightOnMono)
                DataSystem.LightTimeMono.Add(new TimeSpan(0, 0, 1));
                
        }
    }
}
