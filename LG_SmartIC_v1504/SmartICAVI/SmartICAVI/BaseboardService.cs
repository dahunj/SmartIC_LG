namespace SmartICAVI
{
    class BaseboardService
    {
        private IBaseBoard iBaseBoard = null;
        #region Singleton
        private static BaseboardService singleton = null;

        public static BaseboardService Singleton
        {
            get
            {
                if (null == singleton)
                {
                    singleton = new BaseboardService();
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
                    iBaseBoard = new BaseBoardAXT();
                    break;
                case "Virtual":
                    iBaseBoard = new BaseBoardVirtual();
                    break;
                default:
                    iBaseBoard = new BaseBoardAXT();
                    break;
            }

            return iBaseBoard.Initialize();
        }

        public int UnInitialize()
        {
            if (null != iBaseBoard)
                return iBaseBoard.UnInitialize();

            return 0;
        }
    }
}
