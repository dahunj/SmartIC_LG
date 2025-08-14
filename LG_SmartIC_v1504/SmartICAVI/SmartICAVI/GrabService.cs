using HalconDotNet;

namespace SmartICAVI
{
    class GrabService
    {
        private IGrab iGrab = null;

        public bool IsConnected
        {
            get
            {
                if (null != iGrab)
                    return iGrab.IsConnected;
                else
                    return false;
            }
        }

        public int Width
        {
            get
            {
                if (null != iGrab)
                    return iGrab.Width;
                else
                    return 0;
            }
        }

        public int Height
        {
            get
            {
                if (null != iGrab)
                    return iGrab.Height;
                else
                    return 0;
            }
        }

        #region Singleton
        private static GrabService singleton = null;

        public static GrabService Singleton
        {
            get
            {
                if (null == singleton)
                {
                    singleton = new GrabService();
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
            switch( DataService.Singleton.DataSystem.MachineName )
            {
                case "SmartICAVI":
                    if( "EXTERN" == DataService.Singleton.DataSystem.CamType )
                        iGrab = new GrabExtern();
                    else
                        iGrab = new GrabPointGray();
                    break;
                case "Virtual":
                    if ("EXTERN" == DataService.Singleton.DataSystem.CamType)
                        iGrab = new GrabExtern();
                    else
                        iGrab = new GrabVirtual();
                    break;
                default:
                    if ("EXTERN" == DataService.Singleton.DataSystem.CamType)
                        iGrab = new GrabExtern();
                    else
                        iGrab = new GrabPointGray();
                    break;
            }
            
            return iGrab.Initialize();
        }

        public int UnInitialize()
        {
            if( null != iGrab )
                return iGrab.UnInitialize();

            return 0;
        }

        public int Connect()
        {
            if (null == iGrab)
                return -1;

            if (false == IsConnected)
                return iGrab.Connect();

            return -1;
        }

        public int Disconnect()
        {
            if (null == iGrab)
                return -1;

            if (true == IsConnected)
            {
                return iGrab.Disconnect();
            }

            return -1;
        }

        public int Grab(out HObject HImage)
        {
            HOperatorSet.GenEmptyObj(out HImage);

            if (null != iGrab)
                return iGrab.Grab(out HImage);

            return -1;
        }

        public int Reset()
        {
            if (null != iGrab)
                return iGrab.Reset();

            return -1;
        }
    }
}
