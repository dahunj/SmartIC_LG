namespace SmartICAVI
{
    class AioAXT : IAio
    {
        #region Interfaces
        private bool isInitialized = false;

        public bool IsInitialized
        {
            get { return isInitialized; }
        }

        public int Initialize()
        {
            isInitialized = false;

            if (CAxtAIO.AIOIsInitialized() == 0)				// AIO모듈을 사용할 수 있도록 라이브러리가 초기화되었는지 확인한다
            {
                if (CAxtAIO.InitializeAIO() == 0)				// AIO모듈을 초기화한다. 열려있는 모든베이스보드에서 AIO모듈을 검색하여 초기화한다
                {
                    Log_Trace.WriteLine("Error AioAXT.Initialize().InitializeAIO()");

                    return -1;
                }
            }

            isInitialized = true;

            DataService dataService = DataService.Singleton;

            SetRange(0, -10.0, 10.0);
            SetRange(1, -10.0, 10.0);

            SetOutvalue(0, dataService.DataMotion[(int)EnumSmartIC.Axis.uncoilerReel].Voffset);
            SetOutvalue(1, dataService.DataMotion[(int)EnumSmartIC.Axis.recoilerReel].Voffset);

            if (0 == AxtCNT.CNTIsInitialized())
            {
                AxtCNT.InitializeCNT(1);

                int init = AxtCNT.CNTIsInitialized();
            }

            return 0;
        }

        public int UnInitialize()
        {
            if (1 == CAxtLib.AxtIsInitialized() )
            {
                SetOutvalue(0, 0.0);
                SetOutvalue(1, 0.0);
            }
            return 0;
        }

        public int SetRange(int channel, double min, double max)
        {
            if (false == IsInitialized)
                return -1;

            CAxtAIO.AIOset_range_dac((short)channel, min, max);

            return 0;
        }

        public int GetRange(int channel, ref double min, ref double max)
        {
            if (false == IsInitialized)
                return -1;

            CAxtAIO.AIOget_range_dac((short)channel, ref min, ref max);

            return 0;
        }

        public int SetOutvalue(int channel, double value)
        {
            if (false == IsInitialized)
                return -1;

            CAxtAIO.AIOwrite_dac((short)channel, value);

            return 0;
        }

        public double GetOutvalue(int channel)
        {
            if (false == IsInitialized)
                return 0.0;

            return CAxtAIO.AIOread_dac((short)channel);
        }
        #endregion
    }
}
