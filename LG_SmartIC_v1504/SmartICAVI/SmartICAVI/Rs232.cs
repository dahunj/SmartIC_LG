using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace SmartICAVI
{
    class Rs232
    {
        public enum Connects
        {
            disconnect = 0, connect, wirtetimeout, readtimeout,
        };

        public delegate void ConnectEventHandler(int connect);
        public event ConnectEventHandler ConnectEvent;
        protected void FireConnectEvent(Connects connect)
        {
            if (null != ConnectEvent)
                ConnectEvent((int)connect);
        }

        public delegate void MessageEventHandler(string message);
        public event MessageEventHandler MessageEvent;
        protected void FireMessageEvent(string message)
        {
            if (null != MessageEvent)
                MessageEvent(message);
        }

        protected bool readContinue = false;
        protected SerialPort serial;
        protected Thread threadRead = null;
        protected string readData;

        public bool IsConnected
        {
            get
            {
                return serial.IsOpen;
            }
        }

        public Rs232()
        {
            serial = new SerialPort();
        }

        public int Connect(string port = "COM1", int baudrate = 19200, int dataBits = 8, StopBits stopBits = StopBits.One, Parity parity = Parity.None, Handshake handShake = Handshake.None, int readTimeout = 300, int writeTimeout = 300)
        {
            Disconnect();

            try
            {
                serial.PortName = port;
                serial.BaudRate = baudrate;
                serial.Parity = parity;
                serial.DataBits = dataBits;
                serial.StopBits = stopBits;
                serial.Handshake = handShake;
                serial.ReadTimeout = readTimeout;
                serial.WriteTimeout = writeTimeout;
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine("Exception Rs232.Connect() => " + exc.Message);
                FireConnectEvent(Connects.disconnect);
                return -1;
            }

            //threadRead = new Thread(new ThreadStart(this.ThreadRead));
            threadRead = new Thread(ThreadRead);
            threadRead.IsBackground = true;
            //threadRead.Start();

            try
            {
                serial.Open();
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine("Exception Rs232.Connect().Open => " + exc.Message);
                FireConnectEvent(Connects.disconnect);
            }

            if (true == serial.IsOpen)
            {
                readContinue = true;
                threadRead.Start();
                FireConnectEvent(Connects.connect);

                return 0;
            }
            else
            {
                FireConnectEvent(Connects.disconnect);
                return -1;
            }
        }

        public void Disconnect()
        {
            readContinue = false;

            if (null != serial)
            {
                if (serial.IsOpen)
                {
                    //System.Windows.Forms.Application.DoEvents();
                    //Thread.Sleep(50);
                    Delay(50);
                    if (serial.IsOpen)
                    {
                        serial.Close();
                    }
                }
            }

            if (null != threadRead)
            {
                //Thread.Sleep(serial.ReadTimeout + 10);

                if (threadRead.ThreadState == ThreadState.Running)
                {
                    threadRead.Abort();

                    if (threadRead.ThreadState == ThreadState.Running)
                        Delay(50);
                }
                threadRead = null;
            }

            FireConnectEvent(Connects.disconnect);

        }

        public void Write(string write)
        {
            if (true == serial.IsOpen)
            {
                try
                {
                    serial.WriteLine(write);
                }
                catch (TimeoutException exc)
                {
                    Log_Exception.WriteLine("Exception Rs232.Write().TimeoutException => " + exc.Message);
                    FireConnectEvent(Connects.wirtetimeout);
                }
                catch (Exception exc)
                {
                    Log_Exception.WriteLine("Exception Rs232.Write() => " + exc.Message);
                }
            }
        }

        public void Write(Byte[] buffer, int offset, int count)
        {
            if (true == serial.IsOpen)
            {
                try
                {
                    serial.Write(buffer, offset, count);
                }
                catch (TimeoutException exc)
                {
                    Log_Exception.WriteLine("Exception Rs232.Write().TimeoutException => " + exc.Message);
                    FireConnectEvent(Connects.wirtetimeout);
                }
                catch (Exception exc)
                {
                    Log_Exception.WriteLine("Exception Rs232.Write() => " + exc.Message);
                }
            }
        }

        public void Read(out string read)
        {
            read = readData;
        }

        protected virtual void ThreadRead()
        {
            while (true == readContinue)
            {
                Byte[] bt = new Byte[256];
                try
                {
                    //serial.Read(bt, 0, 255);
                    readData = serial.ReadLine();
                    FireMessageEvent(readData);
                }
                catch (TimeoutException exc)
                {
                    string str = exc.Message;
                }
                catch (Exception exc)
                {
                    readContinue = false;
                    Log_Exception.WriteLine("Exception Rs232.ThreadRead() => " + exc.Message);
                }
            }

            if (serial.IsOpen)
            {
                serial.Close();
            }
        }

        private void Delay(int mSec)
        {
            DateTime dateTimeNow = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, mSec);
            DateTime dateTimeAdd = dateTimeNow.Add(duration);

            while (dateTimeAdd >= dateTimeNow)
            {
                Thread.Sleep(1);
                System.Windows.Forms.Application.DoEvents();
                dateTimeNow = DateTime.Now;
            }
        }
    }
}
