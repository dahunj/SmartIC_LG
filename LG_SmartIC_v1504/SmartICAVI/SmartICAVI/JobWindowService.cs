using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;
using System.Windows.Threading;

namespace SmartICAVI
{
    class JobWindowService
    {
        
        private Window OwnerWindow { get; set; }

        public bool IsInitJobWindowDone { get; set; }
        public bool InitJobResult { get; set; }

        public bool IsReportWindowClosed { get; set; }
        public bool ResultReportWindow { get; set; }

        public bool IsHoleWindowClosed { get; set; }
        public bool ResultHoleWindow { get; set; }

        public bool IsCNGWindowClosed { get; set; }
        public bool ResultCNGWindow { get; set; }

        public bool IsSectionYieldWindowClosed { get; set; }
        public bool ResultSectionYieldSWindow { get; set; }

        public bool IsMessageWindowClosed { get; set; }
        public int ResultMessageWindow { get; set; }

        public bool IsRecheckWindowClosed { get; set; }
        public int ResultRecheckWindow { get; set; }

        private MsgService msgService = null;

        static bool InvokeRequired
        {
            get { return Dispatcher.CurrentDispatcher != Application.Current.Dispatcher; }
        }

        #region Singleton
        private static JobWindowService singleton = null;

        public static JobWindowService Singleton
        {
            get
            {
                if (null == singleton)
                {
                    singleton = new JobWindowService();
                }

                return singleton;
            }
        }

        public void ReleaseSingleton()
        {
            if (null != singleton)
            {
                singleton = null;
            }
        }
        #endregion

        public int Initialize(Window win)
        {
            OwnerWindow = win;

            msgService = MsgService.Singleton;

            IsReportWindowClosed = true;
            IsInitJobWindowDone = true;
            IsCNGWindowClosed = true;
            IsSectionYieldWindowClosed = true;
            IsMessageWindowClosed = true;

            IsRecheckWindowClosed = true;
            return 0;
        }

        public int UnInitialize()
        {
            return 0;
        }

        public int ShowInitJobWindow()
        {
            IsInitJobWindowDone = false;

            if (InvokeRequired)
            {
                OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                            (ThreadStart)delegate()
                                            {
                                                SetInitJobWidnow();
                                            }
                );
            }
            else
            {
                SetInitJobWidnow();
            }

            return 0;
        }

        private int SetInitJobWidnow()
        {
            JobInitWindow win = new JobInitWindow();

            win.OwnerWindow = this.OwnerWindow;

            InitJobResult = (bool)win.ShowDialog();

            IsInitJobWindowDone = true;
            return 0;
        }


        public bool ShowReportWindow()
        {
            bool ret = false;
            IsReportWindowClosed = false;

            if (InvokeRequired)
            {
                OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                            (ThreadStart)delegate()
                                            {
                                                ret = SetReportWindow();
                                            }
                );
            }
            else
            {
                ret = SetReportWindow();
            }

            return ret;
        }

        private bool SetReportWindow()
        {
            ReportWindow winReport = new ReportWindow();
            winReport.IsJobEndView = true;
            //winReport.IsVisiblePitchMove = true;
            winReport.IsVisibleNextMachine = true;
            ResultReportWindow = (bool)winReport.ShowDialog();

            IsReportWindowClosed = true;

            return ResultReportWindow;
        }


        public bool ShowHoleWindow()
        {
            bool ret = false;
            IsHoleWindowClosed = false;
            ResultHoleWindow = false;

            if (InvokeRequired)
            {
                OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                            (ThreadStart)delegate()
                                            {
                                                ret = SetHoleWindow();
                                            }
                );
            }
            else
            {
                ret = SetHoleWindow();
            }

            return ret;
        }

        private bool SetHoleWindow()
        {
            IsHoleWindowClosed = false;

            JobHoleWindow winHole = new JobHoleWindow();
            winHole.Show();

            return true;
        }


        public bool ShowCNGWindow(bool start)
        {
            bool ret = false;
            IsCNGWindowClosed = false;
            if (InvokeRequired)
            {
                OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                            (ThreadStart)delegate()
                                            {
                                                ret = SetCNGWindow(start);
                                            }
                );
            }
            else
            {
                ret = SetCNGWindow(start);
            }

            return ret;
        }

        private bool SetCNGWindow(bool start)
        {
            IsCNGWindowClosed = false;

            JobCngWindow winCNG = new JobCngWindow();
            winCNG.IsStart = start;
            winCNG.Show();

            return true;
        }


        public bool ShowSectionYieldWindow()
        {
            bool ret = false;
            IsSectionYieldWindowClosed = false;
            if (InvokeRequired)
            {
                OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                            (ThreadStart)delegate()
                                            {
                                                ret = SetSectionYieldWindow();
                                            }
                );
            }
            else
            {
                ret = SetSectionYieldWindow();
            }

            return ret;
        }

        private bool SetSectionYieldWindow()
        {
            return true;
        }

        public int ShowMessageWindow(int index, string message = "")
        {
            IsMessageWindowClosed = false;
            if (InvokeRequired)
            {
                OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                            (ThreadStart)delegate()
                                            {
                                                SetShowMessageWindow(index, message);
                                            }
                );
            }
            else
            {
                SetShowMessageWindow(index, message);
            }

            return 0;
        }

        private int SetShowMessageWindow(int index, string message = "")
        {
            IsMessageWindowClosed = false;

            JobMessageWindow winMessage = new JobMessageWindow();
            winMessage.Index = index;
            winMessage.Message = message;
            winMessage.Show();

            return 0;
        }

        public int ShowRecheckWindow(int index, string message = "")
        {
            IsRecheckWindowClosed = false;
            if (InvokeRequired)
            {
                OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                            (ThreadStart)delegate()
                                            {
                                                SetShowRecheckWindow(index, message);
                                            }
                );
            }
            else
            {
                SetShowRecheckWindow(index, message);
            }

            return 0;
        }

        public int SetShowRecheckWindow(int index, string message = "")
        {
            IsRecheckWindowClosed = false;

            JobRecheckWindow winRecheck = new JobRecheckWindow();
            winRecheck.Index = index;
            winRecheck.Message = message;
            winRecheck.Show();

            return 0;
        }

    }
}
