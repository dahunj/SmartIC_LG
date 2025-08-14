namespace SmartICAVI
{
    class StrobeVirtual : IStrobe
    {
        #region Interfaces

        public bool IsConnected
        {
            get
            {
                return true;
            }
        }

        public int Connect(string port = "COM1", int baudrate = 19200, int dataBits = 8, System.IO.Ports.StopBits stopBits = System.IO.Ports.StopBits.One, System.IO.Ports.Parity parity = System.IO.Ports.Parity.None, System.IO.Ports.Handshake handShake = System.IO.Ports.Handshake.None, int readTimeout = 300, int writeTimeout = 300)
        {
            return 0;
        }

        public int Disconnect()
        {
            return 0;
        }

        public int Write(byte ch1 = 0, byte ch2 = 0, byte ch3 = 0, byte ch4 = 0, byte page = 0)
        {
            return 0;
        }
        #endregion
    }
}
