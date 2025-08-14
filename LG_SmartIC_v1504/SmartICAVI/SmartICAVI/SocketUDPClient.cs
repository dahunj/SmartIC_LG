using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace SmartICAVI
{
    class SocketUDPClient
    {
        public event EventHandler SendMessageEvent;
        public event EventHandler MessageEvent;

        private UdpClient udpClient = null;
        public bool IsConnected { get; set; }

        private string sendMessage;
        public string SendMessage { get { return sendMessage; } }

        //private NetworkStream stream = null;
        private Thread threadListener = null;
        private IPEndPoint remoteEP = null;
        private bool listening = false;
        private string readValue = "";

        public string ReadValue { get { return readValue; } }

        public SocketUDPClient()
        {
            IsConnected = false;
        }

        public int Connect(string ip, int port)
        {
            Log_Trace.WriteLine(string.Format("UDP Client Connect (IP : {0}, Port : {1})", ip, port));
            Disconnect();

            try
            {
                udpClient = new UdpClient();
                udpClient.Connect(ip, port);
                remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);


            }
            catch (Exception e)
            {
                Log_Trace.WriteLine("Exception : SocketUdpClient.Connect() => " + e.Message);
                return -1;
            }

            IsConnected = true;

            return 0;
        }

        public int StartListening()
        {
            if (true == IsConnected)
            {
                listening = true;

                //threadListener = new Thread(new ThreadStart(Listening));
                //threadListener.Start();
                threadListener = new Thread(Listening);
                threadListener.IsBackground = true;
                threadListener.Start();
            }

            return 0;
        }

        private void Listening()
        {
            try
            {
                while (true == listening)
                {
                    byte[] data = udpClient.Receive(ref remoteEP);
                    readValue = Encoding.ASCII.GetString(data);
                    FireMessageEvent();
                    Thread.Sleep(1);
                }
            }
            catch (Exception e)
            {
                Log_Exception.WriteLine("Exception : SocketUdpClient.Listening() => " + e.Message);
            }
            finally
            {
                IsConnected = false;
                if (null != udpClient)
                {
                    udpClient.Close();
                    udpClient = null;
                }
            }
        }

        public void Disconnect()
        {
            IsConnected = false;

            if (true == listening)
            {
                listening = false;
                Thread.Sleep(50);
            }
            try
            {
                if (null != udpClient)
                {
                    udpClient.Close();
                    udpClient = null;
                }
            }
            catch (Exception e)
            {
                Log_Exception.WriteLine("Exception : SocketUdpClient.Disconnect() => " + e.Message);
            }
        }

        public void Write(string write)
        {
            if (null != udpClient)
            {
                try
                {
                    if (true == IsConnected)
                    {
                        byte[] sendBytes = Encoding.ASCII.GetBytes(write);
                        udpClient.Send(sendBytes, sendBytes.Length);
                    }
                }
                catch (Exception e)
                {
                    Log_Exception.WriteLine("Exception : SocketUdpClient.Write() => " + write + " => " + e.Message);
                }

                sendMessage = write;
                FireSendMessageEvent();
            }
        }

        private void FireSendMessageEvent()
        {
            if (null != SendMessageEvent)
                SendMessageEvent(this, null);
        }

        private void FireMessageEvent()
        {
            if (null != MessageEvent)
                MessageEvent(this, null);
        }
    }
}
