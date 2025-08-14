using System;

namespace SmartICAVI
{
    interface IStrobe
    {
        bool IsConnected { get; }

        int Connect(string port = "COM1", int baudrate = 19200, int dataBits = 8, System.IO.Ports.StopBits stopBits = System.IO.Ports.StopBits.One, System.IO.Ports.Parity parity = System.IO.Ports.Parity.None, System.IO.Ports.Handshake handShake = System.IO.Ports.Handshake.None, int readTimeout = 300, int writeTimeout = 300);
        int Disconnect();

        int Write(Byte ch1, Byte ch2, Byte ch3, Byte ch4, Byte page);
    }
}
