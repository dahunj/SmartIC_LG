using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace SmartICAVI
{
    class SocketTCPClient
    {
        public event EventHandler ConnectEvent;
        private void FireConnectEvent()
        {
            if (null != ConnectEvent)
                ConnectEvent(this, null);
        }

        public event EventHandler MessageEvent;
        private void FireMessageEvent()
        {
            if (null != MessageEvent)
                MessageEvent(this, null);
        }

        private string serverIP = "";
        private string clientIP = "";
        private int serverPort = 0;
        private int clientPort = 0;

        private Thread threadListener = null;
        private TcpClient client = null;
        private NetworkStream stream = null;
        private bool listen = false;
        //private bool isConnected = false;

        private string readValue;

        public bool IsConnected
        {
            get
            {
                if (null != client)
                    return client.Connected;
                else
                    return false;
            }
        }
        public string ReadValue { get { return readValue; } }

        public int Connect(string ipServer, int portServer, string ipClient, int portClient)
        {
            serverIP = ipServer;
            serverPort = portServer;
            clientIP = ipClient;
            clientPort = portClient;

            listen = true;

            Disconnect();
            //threadListener = new Thread(new ThreadStart(Listening));
            //threadListener.Start();
            threadListener = new Thread(Listening);
            threadListener.IsBackground = true;
            threadListener.Start();

            return 0;
        }

        public void Disconnect()
        {
            if (null != threadListener)
            {
                if (listen)
                {
                    listen = false;

                    if (null != stream)
                    {
                        stream.Close();
                        stream = null;
                    }

                    if (null != client)
                    {
                        client.Close();
                        client = null;
                    }

                    if (ThreadState.Running == threadListener.ThreadState)
                        threadListener.Abort();

                    if (ThreadState.Running == threadListener.ThreadState)
                    {
                        Thread.Sleep(100);
                    }

                    //client.Close();
                    //client = null;
                }

                threadListener = null;
            }
            //threadListener.Join();
        }

        private void Listening()
        {
            try
            {
                client = new TcpClient();
                client.Connect(serverIP, serverPort);

                stream = client.GetStream();

                int length;
                //string data = null;
                byte[] bytes = new byte[1024];

                while (listen)
                {

                    try
                    {
                        length = stream.Read(bytes, 0, bytes.Length);
                        //if (0 == length) // bytes 길이가 0 이면 통신 disconnect
                        //{
                        //    listen = false;
                        //    break;
                        //}
                        //else
                        if (0 != length)
                        {
                            readValue = Encoding.Default.GetString(bytes, 0, length);
                            FireMessageEvent();
                        }
                    }
                    catch (Exception e)
                    {
                        string str = e.Message;
                        //int j = 0;
                        //;

                        if (null != stream)
                        {
                            stream.Close();
                            stream = null;
                        }

                        if (null != client)
                        {
                            client.Close();
                            client = null;
                        }

                        listen = false;
                    }
                    //finally
                    //{
                    //    if (null != stream)
                    //    {
                    //        stream.Close();
                    //        stream = null;
                    //    }

                    //    if (null != client)
                    //    {
                    //        client.Close();
                    //        client = null;
                    //    }

                    //    listen = false;
                    //}
                }

                if (null != stream)
                {
                    stream.Close();
                    stream = null;
                }

                if (null != client)
                {
                    client.Close();
                    client = null;
                }

                listen = false;
            }
            catch (SocketException e)
            {
                //EventMessage(e.Message);
                string str = e.Message;
            }
            //finally
            //{
            //    if (null != stream)
            //    {
            //        stream.Close();
            //        stream = null;
            //    }

            //    if (null != client)
            //    {
            //        client.Close();
            //        client = null;
            //    }

            //    listen = false;
            //}

            //EventConnect(0);
            FireConnectEvent();
        }

        public int Send(string data)
        {
            if ((null != client) && (null != stream))
            {
                if (stream.CanWrite)
                {
                    byte[] msg = Encoding.Default.GetBytes(data);
                    stream.Write(msg, 0, msg.Length);
                }
            }

            return 0;
        }
    }
}
