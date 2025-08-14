namespace SmartICAVI
{
    class CntService
    {
        private ICnt iCnt = null;

        public bool IsInitialized
        {
            get
            {
                if (null != iCnt)
                    return iCnt.IsInitialized;
                return false;
            }
        }

        #region Singleton

        private static CntService singleton = null;

        public static CntService Singleton
        {
            get
            {
                if (null == singleton)
                {
                    singleton = new CntService();
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
                    iCnt = new CntAXT();
                    break;
                case "Virtual":
                    iCnt = new CntVirtual();
                    break;
                default:
                    iCnt = new CntAXT();
                    break;
            }

            return iCnt.Initialize();
        }

        public int UnInitialize()
        {
            if (null != iCnt)
                return iCnt.UnInitialize();

            return -1;
        }


        public int SetTriggerEnable(int channel)
        {
            if (false == IsInitialized)
                return -1;

            return iCnt.SetTriggerEnable(channel);
        }

        public int ResetTriggerEnable(int channel)
        {
            if (false == IsInitialized)
                return -1;

            return iCnt.ResetTriggerEnable(channel);
        }

        public int SetPosition(int channel, double pos)
        {
            if (false == IsInitialized)
                return -1;

            return iCnt.SetPosition(channel, pos);
        }

        public int SetTriggerParams(int channel, double row, double high, double period, double width=20.0, int level = 0, int dir = 1)
        {
            if (false == IsInitialized)
                return -1;

            return iCnt.SetTriggerParams(channel, row, high, period, width, level, dir);
        }

    }
}
