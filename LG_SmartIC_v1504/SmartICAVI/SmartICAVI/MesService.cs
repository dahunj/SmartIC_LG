namespace SmartICAVI
{
    class MesService
    {
        public enum commands { req_time = 0, send_jobstart, req_lot, send_jobend, send_status, send_alarm, }
        public enum status { idle = 0, run, down }

        private IMes iMes = null;
        public IMes Mes { get { return iMes; } }

        public bool IsConnected
        {
            get
            {
                if (null != iMes)
                    return iMes.IsConnected;

                return false;
            }
        }

        #region Singleton
        private static MesService singleton = null;

        public static MesService Singleton
        {
            get
            {
                if (null == singleton)
                {
                    singleton = new MesService();
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
            if ("Virtual" == DataService.Singleton.DataSystem.MachineName)
                iMes = new MesVirtual();
            else
                iMes = new MesSmartIC();

            int ret = iMes.Initialize();

            return ret;
        }
        
        public int UnInitialize()
        {
            if (null == iMes)
                return -1;

            return iMes.UnInitialize();
        }

        public int Send(commands command, int value=0, string str="")
        {
            if (null == iMes)
                return -1;

            return iMes.Send((object)command, value, str);
        }
    }
}
