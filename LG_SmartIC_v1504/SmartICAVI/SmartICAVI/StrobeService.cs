using System;

namespace SmartICAVI
{
    class StrobeService
    {
        private IStrobe iStrobe = null;

        public bool IsConnected
        {
            get
            {
                if (null != iStrobe)
                    return iStrobe.IsConnected;
                else
                    return false;
            }
        }

        #region Singleton
        private static StrobeService singleton = null;

        public static StrobeService Singleton
        {
            get
            {
                if (null == singleton)
                {
                    singleton = new StrobeService();
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
            //iStrobe = new StrobeALT();

            if ("EXTERN" == DataService.Singleton.DataSystem.CamType)
                iStrobe = new StrobeVirtual();
            else
                iStrobe = new StrobeALT();

            return iStrobe.Connect();
        }

        public int UnInitialize()
        {
            
            return Disconnect();
        }

        public int Connect()
        {
            if( null == iStrobe )
                return -1;

            if (false == IsConnected)
                return iStrobe.Connect();

            return -1;
        }

        public int Disconnect()
        {
            if (null == iStrobe)
                return -1;

            if (true == IsConnected)
            {
                try
                {
                    Write();

                    System.Windows.Forms.Application.DoEvents();
                    System.Threading.Thread.Sleep(100);
                    //System.Windows.Forms.Application.DoEvents();

                    return iStrobe.Disconnect();
                }
                catch (Exception exc)
                {
                    Log_Exception.WriteLine("StrobeService.Disconnect() : " + exc.Message);
                }
            }

            return -1;
        }

        public int Write(byte ch1 = 0, byte ch2 = 0, byte ch3 = 0, byte ch4 = 0, byte page = 0)
        {
            if (null == iStrobe)
                return -1;

            if (true == IsConnected)
                return iStrobe.Write(ch1, ch2, ch3, ch4, page);

            return -1;
        }
    }
}
