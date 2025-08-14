using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace SmartICAVI
{
    class GrabExtern : IGrab
    {
        private DataService dataService = null;
        private VisionCamService camService = null;

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
            dataService = DataService.Singleton;
            camService = VisionCamService.Singleton;
            Connect();

            try
            {
                // Kill 
                System.Diagnostics.Process[] mProcess = System.Diagnostics.Process.GetProcessesByName("SmartIC_CAM");

                foreach (System.Diagnostics.Process p in mProcess)
                    p.Kill();

                DateTime start = DateTime.Now;
                TimeSpan duration = new TimeSpan(0, 0, 0, 0, 200);
                DateTime timeout = start.Add(duration);

                while (true)
                {
                    DoEvents();

                    if (timeout < DateTime.Now)
                        break;
                }

                // ReStart
                string path = dataService.CurrentPath + "\\SmartIC_CAM";
                System.Diagnostics.Process.Start(path);
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine("GrabExtern.Initialize() : " + exc.Message);
            }

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

            if (null != camService)
            {
                for (int i = 0; i < 3; ++i)
                {
                    camService.SendGrab();

                    DateTime start = DateTime.Now;
                    TimeSpan duration = new TimeSpan(0, 0, 5);
                    DateTime timeout = start.Add(duration);

                    //while (false == camService.IsFlag((int)EnumSmartIC.VisionFlags.grab))
                    //{
                    //    DoEvents();
                    //}

                    //return 0;

                    // for test
                    while (false == camService.IsFlag((int)EnumSmartIC.VisionFlags.grab))
                    {
                        DoEvents();

                        if (timeout < DateTime.Now)
                            break;
                    }

                    if (true == camService.IsFlag((int)EnumSmartIC.VisionFlags.grab))
                        return 0;
                    else
                    {
                        Log_Trace.WriteLine("GrabExtern.Grab() : Close");

                        camService.SendMode("CLOSE");
                        DoEvents();

                        start = DateTime.Now;
                        duration = new TimeSpan(0, 0, 0, 0, 1000);
                        timeout = start.Add(duration);

                        while (true)
                        {
                            DoEvents();

                            if (timeout < DateTime.Now)
                                break;
                        }


                        // Kill 
                        System.Diagnostics.Process[] mProcess = System.Diagnostics.Process.GetProcessesByName("SmartIC_CAM");
                        Log_Trace.WriteLine("GrabExtern.Grab() : Kill CAM");

                        foreach (System.Diagnostics.Process p in mProcess)
                            p.Kill();


                        start = DateTime.Now;
                        duration = new TimeSpan(0, 0, 0, 0, 1000);
                        timeout = start.Add(duration);

                        while (true)
                        {
                            DoEvents();

                            if (timeout < DateTime.Now)
                                break;
                        }

                        // ReStart
                        string path = dataService.CurrentPath + "\\SmartIC_CAM";
                        System.Diagnostics.Process.Start(path);

                        Log_Trace.WriteLine("GrabExtern.Grab() : Restart CAM");


                        start = DateTime.Now;
                        duration = new TimeSpan(0, 0, 5);
                        timeout = start.Add(duration);

                        while (true)
                        {
                            DoEvents();

                            if (timeout < DateTime.Now)
                                break;

                            if (true == camService.IsConnected)
                                break;
                        }

                        start = DateTime.Now;
                        duration = new TimeSpan(0, 0, 2);
                        timeout = start.Add(duration);

                        while (true)
                        {
                            DoEvents();

                            if (timeout < DateTime.Now)
                                break;
                        }

                        if( null != camService )
                            camService.SendInitParams();
                    }
                }
            }
            
            return -1;
        }

        public int Reset()
        {
            return 0;
        }

        public int Save(string path)
        {
            if (null != camService)
            {
                camService.SendGrab();
            }
            return 0;
        }
        #endregion

        private void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(
                delegate
                {
                }
            ));
        }
    }
}
