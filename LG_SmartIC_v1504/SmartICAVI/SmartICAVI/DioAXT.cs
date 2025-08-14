using System;

namespace SmartICAVI
{
    class DioAXT : IDio
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
            isInitialized = false;

            if (CAxtDIO.DIOIsInitialized() == 0)				// DIO모듈을 사용할 수 있도록 라이브러리가 초기화되었는지 확인한다
            {
                if (CAxtDIO.InitializeDIO() == 0)				// DIO모듈을 초기화한다. 열려있는 모든베이스보드에서 DIO모듈을 검색하여 초기화한다
                {
                    Log_Trace.WriteLine("Error DioAXT.Initialize().InitializeDIO()");

                    return -1;
                }
            }

            short i = 0;
            short nBoardNo = 0;
            short nModulePos = 0;
            uint uModuleID = 0;
            string strData = "";

            for (i = 0; i < CAxtDIO.DIOget_module_count(); i++)
            {
                CAxtDIO.DIOget_module_info(i, ref nBoardNo, ref nModulePos);
                uModuleID = CAxtDIO.DIOget_module_id(i);

                switch ((AXT_FUNC_MODULE)uModuleID)
                {
                    case AXT_FUNC_MODULE.AXT_SIO_DI32:
                        strData = string.Format("[{0:D2}:{0:D2}] SIO-DI32", nBoardNo, i);
                        break;

                    case AXT_FUNC_MODULE.AXT_SIO_DO32P:
                        strData = string.Format("[{0:D2}:{0:D2}] SIO-DO32P", nBoardNo, i);
                        break;

                    case AXT_FUNC_MODULE.AXT_SIO_DB32P:
                        strData = string.Format("[{0:D2}:{0:D2}] SIO-DB32P", nBoardNo, i);
                        break;

                    case AXT_FUNC_MODULE.AXT_SIO_DO32T:
                        strData = string.Format("[{0:D2}:{0:D2}] SIO-DO32T", nBoardNo, i);
                        break;

                    case AXT_FUNC_MODULE.AXT_SIO_DB32T:
                        strData = string.Format("[{0:D2}:{1:D2}] SIO-DB32T", nBoardNo, i);
                        break;
                }

                Log_Trace.WriteLine("DioAXT.Initialize() : " + strData);
            }

            isInitialized = true;

            //ResetOutport(30);
            ResetOutport(31);
            
            return 0;
        }

        public int UnInitialize()
        {
            //ResetOutport(30);
            ResetOutport(31);

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
            if (false == IsInitialized)
                return -1;

            CAxtDIO.DIOwrite_outport((ushort)index, 0);

            return 0;
        }

        public int ToggleOutport(int index)
        {
            if (false == IsInitialized)
                return -1;

            if( 0 == CAxtDIO.DIOread_outport((ushort)index) )
                CAxtDIO.DIOwrite_outport((ushort)index, 1);
            else
                CAxtDIO.DIOwrite_outport((ushort)index, 0);

            return 0;
        }

        public bool GetInportOn(int index)
        {
            if (false == IsInitialized)
                return false;

            return (1 == CAxtDIO.DIOread_inport((ushort)index) ) ? true : false;
        }

        public bool GetOutportOn(int index)
        {
            if (false == IsInitialized)
                return false;

            return (1 == CAxtDIO.DIOread_outport((ushort)index)) ? true : false;
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
            if (false == IsInitialized)
                return -1;

            inports0 = CAxtDIO.DIOread_inport_dword(0, 0);
            inports1 = CAxtDIO.DIOread_inport_dword(1, 0);

            return 0;
        }

        public int ReadOutports()
        {
            if (false == IsInitialized)
                return -1;

            outports0 = CAxtDIO.DIOread_outport_dword(2, 0);

            return 0;
        }
        #endregion
    }
}
