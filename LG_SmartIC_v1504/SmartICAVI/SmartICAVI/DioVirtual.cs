using System;

namespace SmartICAVI
{
    class DioVirtual : IDio
    {
        #region Interfaces
        private bool isInitialized = false;
        public bool IsInitialized
        {
            get { return isInitialized; }
        }

        private UInt32 inports0 = 0;
        private UInt32 inports1 = 0;
        private UInt32 outports0 = 0;
        private UInt32 outports1 = 0;

        public UInt32 Inports0 { get { return inports0; } }
        public UInt32 Inports1 { get { return inports1; } }
        public UInt32 Outports0 { get { return outports0; } }
        public UInt32 Outports1 { get { return outports1; } }

        public int Initialize()
        {
            isInitialized = true;

            Log_Trace.WriteLine("DioVirtual.Initialize().InitializeDIO()");
                    
            return 0;
        }

        public int UnInitialize()
        {
            isInitialized = false;

            return 0;
        }

        public int SetOutport(int index)
        {
            if (false == IsInitialized)
                return -1;

            CAxtDIO.DIOwrite_outport((ushort)index, 1);

            return 0;
        }

        public int ResetOutport(int index)
        {
            return 0;
        }

        public int ToggleOutport(int index)
        {
            return 0;
        }


        public bool GetInportOn(int index)
        {
            switch (index)
            {
                case (int)EnumSmartIC.Inports.emo1:
                    return false;
                case (int)EnumSmartIC.Inports.emo2:
                    return false;
                default:
                    break;
            }

            return true;
        }

        public bool GetOutportOn(int index)
        {
            

            return false;
        }

        public int SetVirtual(int command, int value, double value2 = 0.0)
        {
            return 0;
        }

        public int GetVirtual(int command, ref int value, ref double value2)
        {
            return 0;
        }

        public int ReadInports()
        {
            return 0;
        }

        public int ReadOutports()
        {
            return 0;
        }
        #endregion
    }
}
