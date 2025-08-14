using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace SmartICAVI
{
    class ConnectionThread
    {
        bool listenContinue = false;

        public TcpListener threadListener;
        public static int connections = 0;

        // client 로 부터 접속 요청이 들어오면 진입
        public void HandleConnection()
        {
            int recv;
            byte[] data = new byte[1024];
            TcpClient client = threadListener.AcceptTcpClient();    // 클라이언트와 연결
            NetworkStream ns = client.GetStream();      // 데이터를 주고받기위한 서비스제공
            connections++;

            listenContinue = true;
            // 현재 접속된 Client IP 
            // client.Client.RemoteEndPoint.ToString();

            while (true == listenContinue)
            {
                try // 통신을 하다가 예상치 못한 에러나 인터넷이 끊기는 경우를 대비
                {
                    data = new byte[1024];
                    recv = ns.Read(data, 0, data.Length);   // client로 부터 데이터 수신이 들어올때 까지 대기모드 진입

                    if (0 == recv)       // byte의 크기가 0이면 통신이 끊어진 경우로 보기때문에 While 에서 나와 쓰레드 삭제
                    {
                        listenContinue = false;
                        break;
                    }
                    else
                    {
                        ;// 데이터 처리
                    }
                }
                catch
                {
                    break;
                }
            }
            listenContinue = false;
            ns.Close();
            client.Close();
            connections--;
        }
    }

    class SocketTCPMultiServer
    {
        private TcpListener mainListener;
        bool mainThreadContinue = false;
        Thread mainThread = null;

        public string ServerIP { get; set; }
        public int ServerPort { get; set; }

        public void Connect(string ip, int port)
        {
            ServerIP = ip;
            ServerPort = port;

            mainThreadContinue = true;

            //mainThread = new Thread(new ThreadStart(MainListening));
            //mainThread.Start();

            mainThread = new Thread(MainListening);
            mainThread.IsBackground = true;
            mainThread.Start();
        }

        public void Disconnect()
        {
            //if (null != threadListener)
            //{
            //    if (true == startListen)
            //    {
            //        startListen = false;

            //        threadListener.Abort();

            //        if (ThreadState.Running == threadListener.ThreadState)
            //        {
            //            Thread.Sleep(100);
            //        }

            //        client.Close();
            //        listener.Stop();

            //        client = null;
            //    }

            //    threadListener = null;
            //}
        }

        private void MainListening()
        {
            mainListener = new TcpListener(IPAddress.Parse(ServerIP), ServerPort);
            mainListener.Start();

            while (true == mainThreadContinue)
            {
                while (mainListener.Pending())
                {
                    if (false == mainThreadContinue)
                        break;

                    Thread.Sleep(50);
                }
                // Connection Thread 생성
                ConnectionThread newConnecting = new ConnectionThread();
                newConnecting.threadListener = this.mainListener;
                //Thread newThread = new Thread(new ThreadStart(newConnecting.HandleConnection));
                //newThread.Start();

                Thread newThread = new Thread(newConnecting.HandleConnection);
                newThread.IsBackground = true;
                newThread.Start();
            }
        }
    }
}
