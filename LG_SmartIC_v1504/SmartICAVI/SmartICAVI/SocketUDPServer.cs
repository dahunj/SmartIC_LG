using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace SmartICAVI
{
    class SocketUDPServer
    {
        public event EventHandler ConnectEvent;
        public event EventHandler MessageEvent;


        private Thread threadListener = null;
        private UdpClient udpServer = null;
        private IPEndPoint remoteEP = null;

        private bool listening = false;

        private string readValue = "";

        public bool IsOpened { get; set; }

        public bool IsConnected { get; set; }

        public string ReadValue { get { return readValue; } }

        public int Open(int port)
        {
            Close();

            Log_Trace.WriteLine(string.Format("UDP Server Open (IP : {0}, Port : {1})", IPAddress.Any, port));

            try
            {
                udpServer = new UdpClient(port);
                remoteEP = new IPEndPoint(IPAddress.Any, port);
            }
            catch (Exception e)
            {
                Log_Exception.WriteLine("Exception : SocketUdpServer.Open() => " + e.Message);
                return -1;
            }

            IsOpened = true;
            listening = true;
            //threadListener = new Thread(new ThreadStart(this.Listening));
            //threadListener.Start();
            threadListener = new Thread(Listening);
            threadListener.IsBackground = true;
            threadListener.Start();

            return 0;
        }

        public void Close()
        {
            Log_Trace.WriteLine("UDP Close");
            IsOpened = false;
            IsConnected = false;
            listening = false;
            if (null != threadListener)
            {
                if (ThreadState.Running == threadListener.ThreadState)
                {
                    threadListener.Abort();

                    while (ThreadState.Running == threadListener.ThreadState)
                    {
                        Thread.Sleep(50);
                    }
                }

                threadListener = null;
            }

            if (null != udpServer)
            {
                try
                {
                    udpServer.Close();
                    udpServer = null;
                }
                catch (Exception e)
                {
                    Log_Exception.WriteLine("Exception : SocketUdpServer.Close() => " + e.Message);
                }
            }
        }

        private void Listening()
        {
            try
            {
                while (true == listening)
                {
                    byte[] data = udpServer.Receive(ref remoteEP);
                    readValue = Encoding.ASCII.GetString(data);
                    FireMessageEvent();

                    //string str = remoteEP.ToString();

                    //System.Diagnostics.Debug.WriteLine(remoteEP.ToString());
                    //System.Diagnostics.Debug.WriteLine(readValue);
                }
            }
            catch (Exception e)
            {
                Log_Exception.WriteLine("Exception : SocketUdpServer.Listening() => " + e.Message);
            }
            finally
            {
                IsOpened = false;
                IsConnected = false;
                if (null != udpServer)
                {
                    udpServer.Close();
                    udpServer = null;
                }
            }
        }

        public void Read(out string read)
        {
            read = readValue;
        }

        public void Write(string write)
        {
            if (null != udpServer)
            {
                try
                {
                    if (false == IsConnected)
                    {
                        if (null != remoteEP)
                        {
                            udpServer.Connect(remoteEP);
                            IsConnected = true;
                        }
                    }

                    if (true == IsOpened)
                    {
                        byte[] sendBytes = Encoding.ASCII.GetBytes(write);
                        udpServer.Send(sendBytes, sendBytes.Length);
                    }
                }
                catch (Exception e)
                {
                    Log_Exception.WriteLine("Exception : SocketUdpServer.Write() => " + write + " => " + e.Message);
                }

                //sendMessage = write;
                //FireMessageEvent();
            }
        }

        private void FireConnectEvent()
        {
            if (null != ConnectEvent)
                ConnectEvent(this, null);
        }

        private void FireMessageEvent()
        {
            if (null != MessageEvent)
                MessageEvent(this, null);
        }
    }
}
