using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartICAVI
{
    class Log_TopVision2
    {
        static LogUtill log = new LogUtill("TOPVISION2");
        //static string logHead = " <TOPVISION2> ";

        public delegate void LogEventHandler(string log);
        public static event LogEventHandler LogEvent;


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

        public static void Write(string message)
        {
            log.Write(message);

            if (null != LogEvent)
                LogEvent(message);
        }

        public static void WriteLine(string formatString, params object[] args)
        {
            log.WriteLine(formatString, args);

            if (null != LogEvent)
                LogEvent(string.Format(formatString, args));
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
