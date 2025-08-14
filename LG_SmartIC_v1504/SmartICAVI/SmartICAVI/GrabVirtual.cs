namespace SmartICAVI
{
    class GrabVirtual : IGrab
    {
        #region Interfaces
        public bool IsConnected
        {
            get
            {
                return true;
            }
        }

        public int Width { get { return 600; } }
        public int Height { get { return 400; } }

        public int Initialize()
        {
            Connect();

            return 0;
        }

        public int UnInitialize()
        {
            Disconnect();

            return 0;
        }

        public int Connect()
        {
            return 0;
        }

        public int Disconnect()
        {
            return 0;
        }

        public int Grab(out HalconDotNet.HObject HImage)
        {
            HImage = null;
            return 0;
        }

        public int Reset()
        {
            return 0;
        }

        public int Save(string path)
        {
            return 0;
        }
        #endregion
    }
}
