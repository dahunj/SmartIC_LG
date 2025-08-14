using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace SmartICAVI
{
    class SocketTCPServer
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

        private Thread threadListener = null;
        private Thread mainThreadListener = null;

        private TcpListener listener = null;
        private TcpClient client = null;
        private NetworkStream stream = null;

        private bool mainListening = false;
        //private bool isConnected = false;

        private string readValue = "";

        private string serverIP = "";
        private int serverPort = 0;

        private int connections = 0;

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
        public bool IsOpened { get { return mainListening; } }
        public string ReadValue { get { return readValue; } }

        public void Open(string ip, int port)
        {
            serverIP = ip;
            serverPort = port;

            mainListening = true;


            //mainThreadListener = new Thread(new ThreadStart(MainListening));
            //mainThreadListener.Start();

            mainThreadListener = new Thread(MainListening);
            mainThreadListener.IsBackground = true;
            mainThreadListener.Start();
        }

        public void Close()
        {
            if (null != threadListener)
            {
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

                threadListener.Abort();

                if (ThreadState.Running == threadListener.ThreadState)
                {
                    Thread.Sleep(100);
                }

                threadListener = null;

            }

            if (null != mainThreadListener)
            {
                mainListening = false;

                Thread.Sleep(50);

                if (null != listener)
                {
                    listener.Stop();
                    listener = null;
                }

                mainThreadListener.Abort();

                if (ThreadState.Running == mainThreadListener.ThreadState)
                {
                    Thread.Sleep(100);
                }

                mainThreadListener = null;
                connections = 0;
            }
        }

        public void Disconnect()
        {
            if (null != threadListener)
            {
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

                threadListener.Abort();

                if (ThreadState.Running == threadListener.ThreadState)
                {
                    Thread.Sleep(100);
                }

                threadListener = null;
                --connections;
            }
        }

        private void MainListening()
        {
            listener = new TcpListener(IPAddress.Parse(serverIP), serverPort);
            listener.Start();

            while (true == mainListening)
            {
                while (false == listener.Pending())
                {
                    if (false == mainListening)
                        break;

                    Thread.Sleep(1);
                }

                if (true == mainListening)
                {
                    if (0 < connections)
                    {
                        Disconnect();
                        Thread.Sleep(50);
                        ////EventMessage("New Client connect");
                        //threadListener = new Thread(new ThreadStart(Listening));
                        //threadListener.Start();
                        threadListener = new Thread(Listening);
                        threadListener.IsBackground = true;
                        threadListener.Start();
                        Thread.Sleep(50);
                    }
                    else
                    {
                        ++connections;

                        Disconnect();
                        //threadListener = new Thread(new ThreadStart(Listening));
                        //threadListener.Start();
                        threadListener = new Thread(Listening);
                        threadListener.IsBackground = true;
                        threadListener.Start();
                        Thread.Sleep(50);
                    }
                }
            }
        }

        private void Listening()
        {
            int length;

            try
            {
                client = listener.AcceptTcpClient();
                //EventConnect(1);
                FireConnectEvent();

                stream = client.GetStream();

                //EventMessage("Client connected to this Server");

                while (true)
                {
                    byte[] bytes = new byte[1024];
                    length = stream.Read(bytes, 0, bytes.Length);

                    if (0 == length)    // disconnect
                    {
                        break;
                    }
                    else
                    {
                        readValue = Encoding.Default.GetString(bytes, 0, length);
                        //int index = read.IndexOf('\n');
                        //read = read.Substring(0, index);

                        FireMessageEvent();
                    }
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


                FireConnectEvent();
            }
            catch (Exception e)
            {
                string str = e.Message;
            }
            finally
            {
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
            }

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
