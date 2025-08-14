using System;

namespace SmartICAVI
{
    interface IDio
    {
        bool IsInitialized { get; }

        UInt32 Inports0 { get; }
        UInt32 Inports1 { get; }
        UInt32 Outports0 { get; }
        UInt32 Outports1 { get; }

        int Initialize();
        int UnInitialize();

        int SetOutport(int index);
        int ResetOutport(int index);
        int ToggleOutport(int index);

        bool GetInportOn(int index);
        bool GetOutportOn(int index);

        int ReadInports();
        int ReadOutports();

        int SetVirtual(int command, int value, double value2 = 0.0);
        int GetVirtual(int command, ref int value, ref double value2);
    }
}
