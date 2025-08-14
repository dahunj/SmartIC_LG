using System;

namespace SmartICAVI
{
    class StrobeALT : IStrobe
    {
        protected Rs232_Byte rs232 = null;
        protected string readData = "";

        public StrobeALT()
        {
            if (null == rs232)
                rs232 = new Rs232_Byte();
        }

        #region Interfaces

        public bool IsConnected
        {
            get 
            {
                if (null != rs232)
                    return rs232.IsConnected;
                else
                    return false;
            }
        }

        public int Connect(string port =  "COM1", int baudrate = 19200, int dataBits = 8, System.IO.Ports.StopBits stopBits = System.IO.Ports.StopBits.One, System.IO.Ports.Parity parity = System.IO.Ports.Parity.None, System.IO.Ports.Handshake handShake = System.IO.Ports.Handshake.None, int readTimeout = 300, int writeTimeout = 300)
        {
            rs232.ConnectEvent += OnConnectEvent;
            rs232.MessageEvent += OnMessageEvent;

            rs232.Connect(port, baudrate, dataBits, stopBits, parity, handShake, readTimeout, writeTimeout);

            return 0;
        }

        public int Disconnect()
        {
            rs232.ConnectEvent -= OnConnectEvent;
            rs232.MessageEvent -= OnMessageEvent;

            rs232.Disconnect();

            return 0;
        }

        public int Write(byte ch1=0, byte ch2=0, byte ch3=0, byte ch4=0, byte page=0)
        {
            Byte[] data = new Byte[10];
            data[0] = 0xEF;
            data[1] = 0xEF;
            data[2] = 0x00;
            data[3] = ch1;
            data[4] = ch2;
            data[5] = ch3;
            data[6] = ch4;
            data[7] = 0x00;
            data[7] ^= ch1;
            data[7] ^= ch2;
            data[7] ^= ch3;
            data[7] ^= (Byte)(ch4+0x01);
            data[8] = 0xEE;
            data[9] = 0xEE;

            rs232.Write(data, 0, data.Length);
                        
            return 0;
        }
        #endregion


        #region EVENT
        void OnMessageEvent(string message)
        {
            readData = message;

            ReadingProcess(readData);

           
        }

        void OnConnectEvent(int connect)
        {
            
        }
        #endregion

        private void ReadingProcess(string message)
        {
            ;
        }
       
    }
}
