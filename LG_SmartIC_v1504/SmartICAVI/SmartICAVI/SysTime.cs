using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace SmartICAVI
{
    class SysTime
    {
        [DllImport("kernel32.dll")]
        public extern static void GetSystemTime(ref SYSTEMTIME lpSystemTime);

        [DllImport("kernel32.dll")]
        public extern static uint SetSystemTime(ref SYSTEMTIME lpSystemTime);

        [DllImport("kernel32.dll")]
        public extern static void GetLocalTime(ref SYSTEMTIME lpSystemTime);

        [DllImport("kernel32.dll")]
        public extern static uint SetLocalTime(ref SYSTEMTIME lpSystemTime);


        public struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        public SysTime()
        {
        }

        public void GetTime(ref SYSTEMTIME sysTime)
        {
            GetSystemTime(ref sysTime);
        }
        public void SetTime(SYSTEMTIME sysTime)
        {
            SetSystemTime(ref sysTime);
        }
    }
}
