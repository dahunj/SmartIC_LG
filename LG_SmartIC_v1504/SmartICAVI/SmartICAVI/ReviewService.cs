using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;
using System.Windows.Threading;

namespace SmartICAVI
{
    class ReviewService
    {
        private Window OwnerWindow { get; set; }
        private ReviewWindow reviewWindow = null;

        private DataService dataService = null;


        static bool InvokeRequired
        {
            get { return Dispatcher.CurrentDispatcher != Application.Current.Dispatcher; }
        }

        #region Singleton
        private static ReviewService singleton = null;

        public static ReviewService Singleton
        {
            get
            {
                if (null == singleton)
                {
                    singleton = new ReviewService();
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

        public void Initialize(Window win)
        {
            OwnerWindow = win;

            dataService = DataService.Singleton;

            ShowReviewWindow(win);
        }

        public void UnInitialize()
        {
            HideReviewWindow();
        }

        public void ShowReviewWindow(Window win)
        {
            if (InvokeRequired)
            {
                OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                            (ThreadStart)delegate()
                                            {
                                                SetReviewWindow(win);
                                            }
                );
            }
            else
            {
                SetReviewWindow(win);
            }
        }

        public void HideReviewWindow()
        {
            if (InvokeRequired)
            {
                OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                            (ThreadStart)delegate()
                                            {
                                                ResetReviewWindow();
                                            }
                );
            }
            else
            {
                ResetReviewWindow();
            }
        }

        public void Start()
        {
            if (null != reviewWindow)
            {
                reviewWindow.Start();
            }
        }

        public void Stop()
        {
            if (null != reviewWindow)
            {
                reviewWindow.Stop();
            }
        }

        public void SetFocus()
        {
            if (null != reviewWindow)
                reviewWindow.Focus();
        }

        public void SetVerify()
        {
            if (null != reviewWindow)
            {
                reviewWindow.SetVerifyEnable();
            }
        }

        public void DisposeImages()
        {
            if (null != reviewWindow)
            {
                reviewWindow.DisposeImages();
            }
        }

        public void Restart(int index)
        {
            if (null != reviewWindow)
            {
                if (0 > index)
                    index = 0;

                reviewWindow.Restart(index);
            }
        }

        private void SetReviewWindow(Window win)
        {
            if (null == reviewWindow)
            {
                reviewWindow = new ReviewWindow(win);
            }

            if ("Virtual" != dataService.DataSystem.MachineName)
            {
                if (null != OwnerWindow)
                    reviewWindow.Owner = OwnerWindow;
            }

            reviewWindow.Show();
        }

        private void ResetReviewWindow()
        {
            if (null != reviewWindow)
            {
                reviewWindow.Close();
                reviewWindow = null;
            }
        }

    }
}
