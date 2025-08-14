namespace SmartICAVI
{
    class AioService
    {

        private IAio iAio = null;

        #region Singleton

        private static AioService singleton = null;

        public static AioService Singleton
        {
            get
            {
                if (null == singleton)
                {
                    singleton = new AioService();
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

        public int Initialize()
        {
            switch (DataService.Singleton.DataSystem.MachineName)
            {
                case "SmartICAVI":
                    iAio = new AioAXT();
                    break;
                case "Virtual":
                    iAio = new AioVirtual();
                    break;
                default:
                    iAio = new AioAXT();
                    break;
            }

            return iAio.Initialize();
        }

        public int UnInitialize()
        {
            if (null != iAio)
                return iAio.UnInitialize();

            return -1;
        }

        public int SetRange(int channel, double min, double max)
        {
            if (null == iAio)
                return -1;

            return iAio.SetRange(channel, min, max);
        }

        public int GetRange(int channel, ref double min, ref double max)
        {
            if (null == iAio)
                return -1;

            return iAio.GetRange(channel, ref min, ref max);
        }

        public int SetOutvalue(int channel, double value)
        {
            if (null == iAio)
                return -1;

            return iAio.SetOutvalue(channel, value);
        }

        public double GetOutvalue(int channel)
        {
            if (null == iAio)
                return 0.0;

            return iAio.GetOutvalue(channel);
        }

    }
}
