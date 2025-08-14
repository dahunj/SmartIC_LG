using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace SmartICAVI
{
    class LogUtill
    {
        protected StreamWriter writer;
        protected bool created = false;
        protected DateTime startTime;
        protected int type = 0;
        protected string filePath;
        protected string period = "";
        protected int saveDays = 30;
        protected string path;
        protected string log;

        protected string FileName { get; set; }
        protected bool FileNameTime { get; set; }
        protected bool WriteTime { get; set; }

        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                if (true == created)
                {
                    JustWrite("[LOG] 저장폴더 변경 : " + value);
                    Close();

                    string oldPath = path;
                    path = value;

                    Write("[LOG] 이전 저장폴더 : " + oldPath);
                    Write("[LOG] 저장폴더 변경 : " + value);
                }
                else
                {
                    path = value;
                }
            }
        }

        public string FilePath
        {
            get
            {
                return filePath;
            }
        }

        public int Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }

        public int SaveDays
        {
            get
            {
                return saveDays;
            }
            set
            {
                saveDays = value;
            }

        }

        public LogUtill()
        {
            FileName = "Log";
            WriteTime = true;
            FileNameTime = false;
        }

        public LogUtill(string fileName, bool writeTime = true, bool fileNameTime = true)
        {
            FileName = fileName;
            WriteTime = writeTime;
            FileNameTime = fileNameTime;
        }

        public void Open()
        {
            CheckPeriod();
            Create();
        }

        public void Write(string message)
        {
            Monitor.Enter(this);
            CheckPeriod();
            Create();

            JustWrite(message);
            Monitor.Exit(this);
        }

        public void WriteLine(string formatString, params object[] args)
        {
            Monitor.Enter(this);
            CheckPeriod();
            Create();

            JustWrite(string.Format(formatString, args));
            Monitor.Exit(this);
        }

        protected void JustWrite(string message)
        {
            if (true == created)
            {
                if (true == WriteTime)
                    log = string.Format("{0}{1}", DateTime.Now.ToString("yyyy-MM-dd,   HH:mm:ss.fff,   "), message);
                else
                    log = string.Format("{0}", message);

                writer.WriteLine(log);
                writer.Flush();
            }
        }

        public void Close()
        {
            if (true == created)
            {
                //writer.WriteLine("");
                try
                {
                    if (null != writer)
                    {
                        writer.Flush();
                        writer.Close();
                        created = false;
                    }
                }
                catch (Exception exc)
                {
                    Log_Exception.WriteLine(exc.Message);
                }
            }
        }

        public void Create()
        {
            if (false == created)
            {
                try
                {
                    if (Directory.Exists(Path))
                    {
                        startTime = DateTime.Now;
                        string fileName;

                        if (true == FileNameTime)
                        {
                            switch (Type)
                            {
                                case 0:     // Run Time
                                    fileName = string.Format("{0}{1}.txt", FileName, startTime.ToString("_yyyy-MM-dd(HH-mm-ss)"));
                                    break;
                                case 1:     // 1 Hour
                                    fileName = string.Format("{0}{1}.txt", FileName, startTime.ToString("_yyyy-MM-dd(HH)"));
                                    break;
                                default:    // 1 Day
                                    fileName = string.Format("{0}{1}.txt", FileName, startTime.ToString("_yyyy-MM-dd"));
                                    break;
                            }
                        }
                        else
                        {
                            fileName = string.Format("{0}.txt", FileName);
                        }

                        string fileLast = "";
                        filePath = Path + "\\" + fileName;

                        DateTime timeLast = DateTime.Now;
                        TimeSpan timeSpan = new TimeSpan(1000000000000000000);
                        TimeSpan minTimeSpan = new TimeSpan(1000000000000000000);


                        string[] files = Directory.GetFiles(Path);

                        // Search all logfile
                        //foreach (string file in files)
                        //{
                        for (int i = 0; i < files.Length; ++i)
                        {
                            string file = files[i];
                            
                            if (-1 != file.IndexOf(FileName))
                            {
                                FileInfo info = new FileInfo(file);

                                //timeSpan = startTime - info.LastWriteTime;
                                timeSpan = startTime - info.CreationTime;

                                if (minTimeSpan > timeSpan)
                                {
                                    fileLast = file;
                                    //timeLast = info.LastWriteTime;
                                    timeLast = info.CreationTime;
                                    minTimeSpan = timeSpan;
                                }
                            }
                        }

                        // Check Period
                        if ("" != fileLast)
                        {
                            if (1 == Type)      // Hour
                            {
                                if ((timeLast.Day == startTime.Day) && (timeLast.Hour == startTime.Hour))
                                {
                                    filePath = fileLast;
                                }
                            }
                            else if (2 == Type) // Day
                            {
                                if ((timeLast.Day == startTime.Day))
                                {
                                    filePath = fileLast;
                                }
                            }
                        }


                        if (File.Exists(filePath))
                        {
                            writer = File.AppendText(filePath);
                        }
                        else
                        {
                            MoveOldFiles();
                            writer = File.CreateText(filePath);
                        }

                        //writer.AutoFlush = true;
                        created = true;

                        //writer.WriteLine("");

                        ClearOldFiles();
                    }
                }
                catch (Exception exc)
                {
                    string str = exc.Message;
                }
            }
        }

        public void CheckPeriod()
        {
            if (true == created)
            {
                if (1 == Type)      // Hour
                {
                    DateTime time = DateTime.Now;
                    if ((startTime.Hour != time.Hour) || (startTime.Day != time.Day))
                    {
                        //JustWrite("[로그] 로그 저장시간 초과 (생성 주기 : 시간)");
                        Close();
                        //period = string.Format("[로그] 이전 로그파일 : {0}", filePath);
                    }
                }
                else if (2 == type)     // Day
                {
                    if (startTime.Day != DateTime.Now.Day)
                    {
                        //JustWrite("[로그] 로그 저장시간 초과 (생성 주기 : 날짜)");
                        Close();
                        //period = string.Format("[로그] 이전 로그파일 : {0}", filePath);
                    }
                }
            }
        }

        public void MoveOldFiles()
        {
            try
            {
                string oldLogPath = Path + "\\Old";
                string moveFile;
                if (!Directory.Exists(oldLogPath))
                {
                    Directory.CreateDirectory(oldLogPath);
                }

                string[] files = Directory.GetFiles(Path);

                //foreach (string file in files)
                //{
                for (int i = 0; i < files.Length; ++i)
                {
                    string file = files[i];

                    if (-1 != file.IndexOf(FileName))
                    {
                        if (file != filePath)
                        {
                            if (true == FileNameTime)
                            {
                                moveFile = file.Replace(Path, oldLogPath);
                            }
                            else
                            {
                                string rename = string.Format("{0}{1}.txt", FileName, startTime.ToString("_yyyy-MM-dd_HH-mm-ss"));
                                moveFile = oldLogPath + "\\" + rename;
                            }

                            File.Move(file, moveFile);
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                string msg = exc.Message;
            }
        }

        public void ClearOldFiles()
        {
            string oldLogPath = Path + "\\Old";

            string[] files = Directory.GetFiles(oldLogPath);
            DateTime time = DateTime.Now;

            //foreach (string file in files)
            //{
            for (int i = 0; i < files.Length; ++i)
            {
                string file = files[i];

                FileInfo info = new FileInfo(file);
                //TimeSpan span = time - info.CreationTime;
                TimeSpan span = time - info.LastWriteTime;

                if (SaveDays < span.Days)
                {
                    Write(string.Format("[로그] 이전 로그 삭제 (저장일 {0}) : {1}", SaveDays, file));
                    File.Delete(file);
                }
            }
        }
    }
}
