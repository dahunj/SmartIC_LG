using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartICAVI
{
    class Log_Error
    {
        static LogUtill log = new LogUtill("Error");
        //static string logHead = "[Error]    ";

        public delegate void LogEventHandler(string log);
        public static event LogEventHandler LogEvent;

        public static int Type
        {
            get
            {
                return log.Type;
            }
            set
            {
                log.Type = value;
            }
        }

        public static string Path
        {
            get
            {
                return log.Path;
            }
            set
            {
                log.Path = value;
            }
        }

        public static int SaveDays
        {
            get
            {
                return log.SaveDays;
            }
            set
            {
                log.SaveDays = value;
            }

        }

        public static void WriteLine(int code, string content)
        {
            string logdata = string.Format("C:{0:0000},D:{1}", code, content);
            log.WriteLine(logdata);

            Log_Trace.WriteLine(logdata);

            if (null != LogEvent)
                LogEvent(logdata);
        }

        public static void WriteLine(int code, string unit, string content)
        {
            string logdata = string.Format("C:{0:0000},U:{1},D:{2}", code, unit, content);
            log.WriteLine(logdata);

            Log_Trace.WriteLine(logdata);

            if (null != LogEvent)
                LogEvent(logdata);
        }

        public static void Open()
        {
            log.Open();
        }

        public static void Close()
        {
            log.Close();
        }
    }
}
