using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SmartICAVI
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables

        private DispatcherTimer timer = null;       // Run/Work Time

        private DataService dataService = null;
        private MsgService msgService = null;

        // Interface
        private SystemService sysService = null;
        private BaseboardService baseService = null;
        private DioService dioService = null;
        private AioService aioService = null;
        private MotionService motionService = null;
        private CntService cntService = null;
        private SequenceService seqService = null;
        private StrobeService strobeService = null;
        private GrabService grabService = null;
        private VirtualKeyboardService vkService = null;
        private InspectService inspectService = null;
        private VisionTopService topService = null;
        private VisionTop2Service top2Service = null;
        private VisionBottomService bottomService = null;
        private VisionBottom2Service bottom2Service = null;
        private VisionMonoService monoService = null;
        private VisionMono2Service mono2Service = null;
        private VisionCamService camService = null;
        private JobWindowService jobWindowService = null;
        private KeyHookingService keyHookingService = null;
        private ReviewService reviewService = null;
        private MesService mesService = null;

        private MonitorWindow winMonitor = null;
        private ReportWindow winReport = null;

        private VirtualKeyboard winKeyboard = null;
        private VirtualKeyNumber winKeyNumber = null;

        private DateTime startTime;
        private bool IsStartTime { get; set; }

        private new bool IsInitialized { get; set; }

        #endregion


        static bool InvokeRequired
        {
            get { return Dispatcher.CurrentDispatcher != Application.Current.Dispatcher; }
        }

        public MainWindow()
        {
            IsInitialized = false;

            //bool isNew;
            //Mutex mutex = new Mutex(true, "SmartIC_AVI.exe", out isNew);
            //if (false == isNew)
            //{
            //    MessageBox.Show("SmartIC_AVI 프로그램이 이미 실행중입니다.");
            //    this.Close();
            //}
            //else
            //{
            //    mutex.ReleaseMutex();
            //}

            Process[] proc = Process.GetProcessesByName("SmartICAVI");

            if (1 < proc.Length)
            {
                MessageBox.Show("Application already started.", "Error", MessageBoxButton.OK, MessageBoxImage.Information); 
                
                Application.Current.Shutdown();
            }
            else
            {
                Application.Current.DispatcherUnhandledException += UnExcept;

                InitializeComponent();

                startTime = DateTime.Now;
                IsStartTime = false;
                IsInitialized = false;
            }
        }

        public void UnExcept(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"D:\" + string.Format("DumpFile_{0:yyyyMMdd}.txt", DateTime.Now), true))
            {
                file.WriteLine(string.Format("[ {0:yyyy-MM-dd hh:mm:ss.ff} ] SmartICAVI", DateTime.Now));
                file.WriteLine("Module : " + e.Exception.Source);
                file.WriteLine("Message : " + e.Exception.Message);
                file.WriteLine(e.Exception.StackTrace.ToString());
                file.WriteLine("\r\n");
                file.Flush();
                file.Close();
            }

            MotionService mot = new MotionService();
            mot.InitOnly();
            mot.Stop(-1);
            mot.UnInitialize();

            //motionService.Stop(-1);
            //motionService.UnInitialize();

            //using( )
            //{
            //    mot.InitOnly();
            //    mot.UnInitialize();
            //}

            //seqService.Stop();
            //motionService.Stop(-1);
            
            //DateTime start = DateTime.Now;
            //TimeSpan duration = new TimeSpan(0, 0, 0, 0, 500);
            //DateTime timeout = start.Add(duration);

            //do
            //{
            //    System.Windows.Forms.Application.DoEvents();
            //    Thread.Sleep(1);
            //} while (timeout > DateTime.Now);

            //motionService.UnInitialize();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Initialize();

            btnInit_Click(this, null);

            //msgService.ShowAlarm(10);
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            //UnInitialize();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (null != winReport)
            {
                winReport.Close();
                winReport = null;
            }

            UnInitialize();
        }


        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (false == IsInitialized)
                return;

            if (null == sysService)
                return;

            if (null == reviewService)
                return;

            // 검사 중일 경우에는 Key 이벤트를 Review 로 넘긴다.
            if (SystemService.Modes.auto == sysService.Mode)
            {
                if (SystemService.States.run == sysService.State)
                {
                    reviewService.SetFocus();
                }
            }
        }


        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            //if (false == IsInitialized)
            //    return;

            //if (null == sysService)
            //    return;

            //if (null == reviewService)
            //    return;

            //// 검사 중일 경우에는 Key 이벤트를 Review 로 넘긴다.
            //if (SystemService.Modes.auto == sysService.Mode)
            //{
            //    if (SystemService.States.run == sysService.State)
            //    {
            //        reviewService.SetFocus();
            //    }
            //}
        }


        private void btnAuto_Click(object sender, RoutedEventArgs e)
        {
            Log_Trace.WriteLine("MainWindow.btnAuto_Click()");

            ReleaseButtons();

            btnAuto.IsChecked = true;
            string page = "AutoPage.xaml";

            mainFrame.Source = new Uri(page, UriKind.Relative);
        }

        private void btnRecipe_Click(object sender, RoutedEventArgs e)
        {
            Log_Trace.WriteLine("MainWindow.btnRecipe_Click()");

            ReleaseButtons();

            btnRecipe.IsChecked = true;
            string page = "RecipePage.xaml";

            mainFrame.Source = new Uri(page, UriKind.Relative);
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            Log_Trace.WriteLine("MainWindow.btnReport_Click()");

            camService.SendMode("TOPMOST", 0);

            if (null != winReport)
            {
                if (false == winReport.IsLoaded)
                {
                    winReport = null;
                }
                else
                {
                    //winReport.IsMES = IsMESSend;
                    winReport.Activate();
                }
            }

            if (null == winReport)
            {
                //Log_Trace.WriteLine("Show Report Window");
                winReport = new ReportWindow();
                //winReport.IsMES = IsMESSend;

                winReport.IsJobEndView = false;
                winReport.IsVisibleNextMachine = false;
                winReport.Show();
                winReport.Owner = this;
            }
        }

        private void btnMonitor_Click(object sender, RoutedEventArgs e)
        {
            Log_Trace.WriteLine("MainWindow.btnMonitor_Click()");

            if (null != winMonitor)
            {
                if (false == winMonitor.IsLoaded)
                {
                    winMonitor = null;
                }
                else
                {
                    winMonitor.Activate();
                }
            }

            if (null == winMonitor)
            {
                winMonitor = new MonitorWindow();
                winMonitor.Show();
                winMonitor.Owner = this;
            }
        }

        private void btnTeach_Click(object sender, RoutedEventArgs e)
        {
            if (false == dataService.IsLogOn)
            {
                JobLogonWindow win = new JobLogonWindow();
                win.ShowDialog();
            }

            if (dataService.IsLogOn)
            {
                Log_Trace.WriteLine("MainWindow.btnTeach_Click()");

                // 0번째 인덱스.....
                if (!(sysService.State == SystemService.States.pause && dataService.DataSystem.FirstIndexPause))
                {
                    ReleaseButtons();
                }

                btnTeach.IsChecked = true;
                string page = "TeachPage.xaml";

                mainFrame.Source = new Uri(page, UriKind.Relative);
            }
            else
            {
                btnTeach.IsChecked = false;
            }
        }

        private void btnSetup_Click(object sender, RoutedEventArgs e)
        {
            if (false == dataService.IsLogOn)
            {
                JobLogonWindow win = new JobLogonWindow();
                win.ShowDialog();
            }

            if (true == dataService.IsLogOn)
            {
                Log_Trace.WriteLine("MainWindow.btnSetup_Click()");

                ReleaseButtons();

                btnSetup.IsChecked = true;
                string page = "SetupPage.xaml";

                mainFrame.Source = new Uri(page, UriKind.Relative);
            }
            else
            {
                btnSetup.IsChecked = false;
            }
        }

        private void btnSystemLock_Click(object sender, RoutedEventArgs e)
        {
            Log_Trace.WriteLine("MainWindow.btnSystemLock_Click()");

            ReleaseButtons();

            btnSystemLock.IsChecked = true;
            string page = "SystemLockPage.xaml";

            mainFrame.Source = new Uri(page, UriKind.Relative);
        }

        private void btnManual_Click(object sender, RoutedEventArgs e)
        {
            Log_Trace.WriteLine("MainWindow.btnManual_Click()");

            ReleaseButtons();

            btnManual.IsChecked = true;
            string page = "ManualPage.xaml";

            mainFrame.Source = new Uri(page, UriKind.Relative);
        }

        private void btnInit_Click(object sender, RoutedEventArgs e)
        {
            Log_Trace.WriteLine("MainWindow.btnInit_Click()");

            ReleaseButtons();

            btnInit.IsChecked = true;
            string page = "InitPage.xaml";

            mainFrame.Source = new Uri(page, UriKind.Relative);
        }

        private void btnShutDown_Click(object sender, RoutedEventArgs e)
        {
            Log_Trace.WriteLine("MainWindow.btnShutDown_Click()");

            if (true == msgService.ShowMessage((int)EnumSmartIC.LightAlarms.close, false))
            {
                //DoEvents(100);
                this.Close();
            }
        }


        private void Initialize()
        {
            IsInitialized = false;
            sysService = SystemService.Singleton;
            dataService = DataService.Singleton;
            msgService = MsgService.Singleton;
            mesService = MesService.Singleton;

            baseService = BaseboardService.Singleton;
            dioService = DioService.Singleton;
            aioService = AioService.Singleton;
            motionService = MotionService.Singleton;
            cntService = CntService.Singleton;
            strobeService = StrobeService.Singleton;
            grabService = GrabService.Singleton;

            topService = VisionTopService.Singleton;
            top2Service = VisionTop2Service.Singleton;
            bottomService = VisionBottomService.Singleton;
            bottom2Service = VisionBottom2Service.Singleton;
            monoService = VisionMonoService.Singleton;
            mono2Service = VisionMono2Service.Singleton;
            camService = VisionCamService.Singleton;

            keyHookingService = KeyHookingService.Singleton;

            inspectService = InspectService.Singleton;
            seqService = SequenceService.Singleton;
            jobWindowService = JobWindowService.Singleton;

            vkService = VirtualKeyboardService.Singleton;

            reviewService = ReviewService.Singleton;

            vkService.VirtualKeyboardEvent += OnVirtualKeyboardEvent;
            vkService.VirtualKeyNumberEvent += OnVirtualKeyNumberEvent;


            
            dataService.Initialize();
            msgService.Initialize(this);
            jobWindowService.Initialize(this);
            reviewService.Initialize(this);

            // Interface
            mesService.Initialize();
            baseService.Initialize();
            dioService.Initialize();
            aioService.Initialize();
            motionService.Initialize();
            cntService.Initialize();
            strobeService.Initialize();
            grabService.Initialize();
            topService.Initialize();
            top2Service.Initialize();
            bottomService.Initialize();
            bottom2Service.Initialize();
            monoService.Initialize();
            mono2Service.Initialize();
            camService.Initialize();

            inspectService.Initialize();
            seqService.Initialize();

            sysService.Initialize();

                        
            lbVelocity.Content = "이송속도(mm/s) : " + dataService.FeedVelocity.ToString("0.0");

            LoadImage();

            // Set Timer
            timer = new DispatcherTimer();
            timer.Interval = new System.TimeSpan(0, 0, 0, 0, 1000);
            timer.IsEnabled = true;
            timer.Tick += new EventHandler(timer_Tick);

            //startTime = DateTime.Now;

            strobeService.Connect();
            //strobeService.Write(100);

            if (0 != dataService.LoadLastRecipe())
            {
                //dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                sysService.State = SystemService.States.stop;
                msgService.SetShowAlarm((int)EnumSmartIC.HeavyAlarms.noRecipe);
            }
            //else
            //{
            //    strobeService.Write((byte)dataService.DataRecipe.strobe1);
            //}

            vkService.VirtualKeyboardEvent += OnVirtualKeyboardEvent;
            vkService.VirtualKeyNumberEvent += OnVirtualKeyNumberEvent;

            sysService.EventState += OnEventState;
            seqService.EventAutoJog += OnEventAutoJog;
            seqService.EventManualJog += OnEventManualJog;

            IsInitialized = true;

            SetEventState(this, null);

            timer_Tick(null, null);

            StartExternProcess();
        }

        private void UnInitialize()
        {
            if (false == IsInitialized)
                return;

            IsInitialized = false;

            if (null != timer)
            {
                timer.Stop();
                timer.IsEnabled = false;
                timer.Tick -= timer_Tick;
            }

            StopExternProcess();

            if (null != vkService)
            {
                vkService.VirtualKeyboardEvent -= OnVirtualKeyboardEvent;
                vkService.VirtualKeyNumberEvent -= OnVirtualKeyNumberEvent;
            }

            if (null != winKeyboard)
            {
                if (winKeyboard.IsLoaded)
                {
                    winKeyboard.Close();
                }

                winKeyboard = null;
            }

            if (null != winKeyNumber)
            {
                if (winKeyNumber.IsLoaded)
                {
                    winKeyNumber.Close();
                }

                winKeyNumber = null;
            }


            if (null != reviewService)
                reviewService.UnInitialize();

            if (null != jobWindowService)
                jobWindowService.UnInitialize();

            // UnInitialize
            if (null != inspectService)
                inspectService.UnInitialize();

            if (null != seqService)
            {
                seqService.EventAutoJog -= OnEventAutoJog;
                seqService.EventManualJog -= OnEventManualJog;
                seqService.UnInitialize();
            }

            if (null != topService)
                topService.UnInitialize();

            if (null != top2Service)
                top2Service.UnInitialize();

            if (null != bottomService)
                bottomService.UnInitialize();

            if (null != bottom2Service)
                bottom2Service.UnInitialize();

            if (null != monoService)
                monoService.UnInitialize();

            if (null != mono2Service)
                mono2Service.UnInitialize();

            if (null != grabService)
                grabService.UnInitialize();

            if (null != strobeService)
                strobeService.UnInitialize();

            if (null != cntService)
                cntService.UnInitialize();
            if (null != motionService)
                motionService.UnInitialize();
            if (null != dioService)
                dioService.UnInitialize();
            if (null != aioService)
                aioService.UnInitialize();
            if (null != baseService)
                baseService.UnInitialize();

            if (null != mesService)
                mesService.UnInitialize();

            if (null != sysService)
            {
                sysService.EventState -= OnEventState;
                sysService.UnInitialize();
            }

            if (null != msgService)
                msgService.UnInitialize();

            if (null != dataService)
                dataService.UnInitialize();

     

            // Release Singleton
            if (null != vkService)
            {
                vkService.ReleaseSingleton();
                vkService = null;
            }

            if (null != keyHookingService)
            {
                keyHookingService.ReleaseSingleton();
                KeyHookingService.UnHook();
            }

            if (null != inspectService)
            {
                inspectService.ReleaseSingleton();
                inspectService = null;
            }

            if (null != reviewService)
            {
                reviewService.ReleaseSingleton();
                reviewService = null;
            }

            if (null != jobWindowService)
            {
                jobWindowService.ReleaseSingleton();
                jobWindowService = null;
            }

            if (null != seqService)
            {
                seqService.ReleaseSingleton();
                seqService = null;
            }

            if (null != topService)
            {
                topService.ReleaseSingleton();
                topService = null;
            }

            if (null != top2Service)
            {
                top2Service.ReleaseSingleton();
                top2Service = null;
            }

            if (null != bottomService)
            {
                bottomService.ReleaseSingleton();
                bottomService = null;
            }

            if (null != bottom2Service)
            {
                bottom2Service.ReleaseSingleton();
                bottom2Service = null;
            }

            if (null != monoService)
            {
                monoService.ReleaseSingleton();
                monoService = null;
            }

            if (null != mono2Service)
            {
                mono2Service.ReleaseSingleton();
                mono2Service = null;
            }

            if (null != grabService)
            {
                grabService.ReleaseSingleton();
                grabService = null;
            }

            if (null != camService)
            {
                camService.UnInitialize();
                camService.ReleaseSingleton();
                camService = null;
            }

            if (null != strobeService)
            {
                strobeService.ReleaseSingleton();
                strobeService = null;
            }

            if (null != cntService)
            {
                cntService.ReleaseSingleton();
                cntService = null;
            }

            if (null != motionService)
            {
                motionService.ReleaseSingleton();
                motionService = null;
            }

            if (null != aioService)
            {
                aioService.ReleaseSingleton();
                aioService = null;
            }

            if (null != dioService)
            {
                dioService.ReleaseSingleton();
                dioService = null;
            }

            if (null != baseService)
            {
                baseService.ReleaseSingleton();
                baseService = null;
            }

            if (null != mesService)
            {
                mesService.ReleaseSingleton();
                mesService = null;
            }

            if (null != sysService)
            {
                sysService.ReleaseSingleton();
                sysService = null;
            }

            if (null != msgService)
            {
                msgService.ReleaseSingleton();
                msgService = null;
            }

            if (null != dataService)
            {
                dataService.ReleaseSingleton();
                dataService = null;
            }
            

            timer.IsEnabled = false;
            timer.Tick -= timer_Tick;

            GC.Collect();
        }

        private void LoadImage()
        {
            if (null == dataService)
                return;
            
            Uri uriImage = new Uri(dataService.DataPath + "\\Images\\Common\\SynapseLogo.png");
            BitmapImage bmp = null;

            try
            {
                bmp = new BitmapImage(uriImage);
            }
            catch (Exception exc)
            {
                // 기본 이미지로 연결해야 함. 
                string error = exc.Message;

                uriImage = new Uri("pack://application:,,/Images/Stop48.png");
                bmp = new BitmapImage(uriImage);

                Log_Exception.WriteLine(error);
            }

            if (null != bmp)
            {
                imageSynapse.Source = bmp;
                imageSynapse.Stretch = Stretch.Uniform;
            }


            uriImage = new Uri(dataService.DataPath + "\\Images\\Common\\UserLogo.png");

            try
            {
                bmp = new BitmapImage(uriImage);
            }
            catch (Exception exc)
            {
                // 기본 이미지로 연결해야 함. 
                string error = exc.Message;

                uriImage = new Uri("pack://application:,,/Images/Stop48.png");
                bmp = new BitmapImage(uriImage);

                Log_Exception.WriteLine(error);
            }

            

            if (null != bmp)
            {
                imageUser.Source = bmp;
                imageUser.Stretch = Stretch.Uniform;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            DateTime curTime = DateTime.Now;

            if (true == IsStartTime)
            {
                TimeSpan span = curTime - startTime;
                lbRunTime.Content = string.Format("작업시간 : {0:00}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds);
            }

            lbCurrentTime.Content = string.Format("현재시간 : {0}", curTime.ToLocalTime());

            if (null != dataService)
            {
                lbVelocity.Content = "이송속도(mm/s) : " + dataService.FeedVelocity.ToString("0.0");
                //lbRecipeName.Content = "레시피 : " + dataService.DataSystem.RecipeName;
                tblRecipeName.Text = "레시피 : " + dataService.DataSystem.RecipeName;
                lbCurrentStatus.Content = "현재상태 : " + dataService.CurrentStatus;
                lbPunchCount.Content = "펀칭누적수 : " + dataService.DataSystem.PunchCount.ToString() + "/" + dataService.DataSystem.PunchCountLimit.ToString();
                
                if (dataService.DataSystem.PunchCount >= dataService.DataSystem.PunchCountLimit)
                    lbPunchCount.Background = Brushes.Red;
                else
                    lbPunchCount.Background = Brushes.Transparent;

                dataService.AddLightTime();
            }

            if (null != mesService.Mes)
                ledMESConnect.IsEnabled = mesService.Mes.IsConnected;
            else
                ledMESConnect.IsEnabled = false;

            if( null != seqService )
                ledOnlineState.IsEnabled = seqService.IsOnlineMode;

        }

        private void ReleaseButtons()
        {
            btnAuto.IsChecked = false;
            btnRecipe.IsChecked = false;
            btnTeach.IsChecked = false;
            btnSetup.IsChecked = false;
            btnManual.IsChecked = false;
            btnSystemLock.IsChecked = false;
            btnInit.IsChecked = false;
        }

        #region Events
        private void OnVirtualKeyboardEvent(object sender, int type)
        {
            if ((true == DataService.Singleton.DataSystem.UseVirtualKeyboard) || (0 != type))
            {
                if (null == sender)
                {
                    if (null != winKeyboard)
                    {
                        if (winKeyboard.IsLoaded)
                        {
                            winKeyboard.Close();
                        }

                        winKeyboard = null;
                    }
                }
                else
                {
                    if (null != winKeyboard)
                    {
                        if (winKeyboard.IsLoaded)
                        {
                            winKeyboard.Close();
                        }

                        winKeyboard = null;
                    }

                    if (null == winKeyboard)
                    {
                        winKeyboard = new VirtualKeyboard(sender);
                        winKeyboard.Owner = this;

                        if (1 == type)
                            winKeyboard.ShowDialog();
                        else
                            winKeyboard.Show();
                    }
                }
            }
        }

        private void OnVirtualKeyNumberEvent(object sender)
        {
            if (true == DataService.Singleton.DataSystem.UseVirtualKeyboard)
            {
                if (null == sender)
                {
                    if (null != winKeyNumber)
                    {
                        if (winKeyNumber.IsLoaded)
                        {
                            winKeyNumber.Close();
                        }

                        winKeyNumber = null;
                    }
                }
                else
                {
                    if (null != winKeyNumber)
                    {
                        if (winKeyNumber.IsLoaded)
                        {
                            winKeyNumber.Close();
                        }

                        winKeyNumber = null;
                    }

                    if (null == winKeyNumber)
                    {
                        winKeyNumber = new VirtualKeyNumber(sender);
                        winKeyNumber.Show();
                        winKeyNumber.Owner = this;
                    }
                }
            }
        }

        private void OnEventState(object send, EventArgs e)
        {
            if (InvokeRequired)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                            (ThreadStart)delegate()
                                            {
                                                SetEventState(send, e);
                                            }
                );
            }
            else
            {
                SetEventState(send, e);
            }
        }

        private void SetEventState(object send, EventArgs e)
        {
            switch (sysService.State)
            {
                case SystemService.States.none:// = 0, 
                case SystemService.States.ready:
                case SystemService.States.idle:
                    SetMenuButtons(true);
                    break;

                case SystemService.States.run:
                    startTime = DateTime.Now;
                    IsStartTime = true;
                    SetMenuButtons(false);
                    break;

                case SystemService.States.pause:
                    // 0번째 인덱스......
                    if (dataService.DataSystem.FirstIndexPause == true)
                    {
                        btnTeach.IsEnabled = true;
                    }
                    break;

                case SystemService.States.resume:
                    if (dataService.DataSystem.FirstIndexPause != true)
                    {
                        btnTeach.IsChecked = false;
                        btnTeach.IsEnabled = false;
                    }
                    break;

                case SystemService.States.homing:
                    break;

                case SystemService.States.homeDone:
                    IsStartTime = false;
                    break;

                case SystemService.States.jobDone:
                    IsStartTime = false;
                    break;

                case SystemService.States.systemLock:
                    SetMenuButtons(false);
                    break;

                case SystemService.States.systemRelease:
                    SetMenuButtons(true);
                    if (null != seqService)
                    {
                        if (true == seqService.IsHomeDone)
                            btnAuto_Click(this, null);
                        else
                            btnInit_Click(this, null);
                    }
                    break;

                case SystemService.States.reset:
                    SetMenuButtons(true);
                    break;

                case SystemService.States.lightAlarm:
                    break;

                case SystemService.States.stop:
                case SystemService.States.heavyAlarm:
                case SystemService.States.emg:
                    IsStartTime = false;
                    SetMenuButtons(true);
                    break;
            }
        }

        private void OnEventAutoJog(int run)
        {
            if (InvokeRequired)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                            (ThreadStart)delegate()
                                            {
                                                SetEventAutoJog(run);
                                            }
                );
            }
            else
            {
                SetEventAutoJog(run);
            }
        }
        private void SetEventAutoJog(int run)
        {
            if (1 == run)
            {
                btnRecipe.IsEnabled = false;
                btnTeach.IsEnabled = false;
                btnSetup.IsEnabled = false;
                btnSystemLock.IsEnabled = false;
                btnInit.IsEnabled = false;
                btnManual.IsEnabled = false;
            }
            else
            {
                btnRecipe.IsEnabled = true;
                btnTeach.IsEnabled = true;
                btnSetup.IsEnabled = true;
                btnSystemLock.IsEnabled = true;
                btnInit.IsEnabled = true;
                btnManual.IsEnabled = true;
            }
        }

        private void OnEventManualJog(int run)
        {
            if (InvokeRequired)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                            (ThreadStart)delegate()
                                            {
                                                SetEventManualJog(run);
                                            }
                );
            }
            else
            {
                SetEventManualJog(run);
            }
        }
        private void SetEventManualJog(int run)
        {
            if (1 == run)
            {
                btnRecipe.IsEnabled = false;
                btnTeach.IsEnabled = false;
                btnSetup.IsEnabled = false;
                btnSystemLock.IsEnabled = false;
                btnInit.IsEnabled = false;
                btnAuto.IsEnabled = false;
            }
            else
            {
                btnRecipe.IsEnabled = true;
                btnTeach.IsEnabled = true;
                btnSetup.IsEnabled = true;
                btnSystemLock.IsEnabled = true;
                btnInit.IsEnabled = true;
                btnAuto.IsEnabled = true;
            }
        }
        #endregion

        private void SetMenuButtons(bool enable)
        {
            if (true == enable)
            {
                if (null != seqService)
                {
                    if (true == seqService.IsHomeDone)
                    {
                        btnAuto.IsEnabled = enable;
                        btnTeach.IsEnabled = enable;
                    }
                    else
                    {
                        btnAuto.IsEnabled = false;
                        btnTeach.IsEnabled = false;
                    }
                }

                btnInit.IsEnabled = true;
                btnManual.IsEnabled = true;
                btnSystemLock.IsEnabled = true;
                btnRecipe.IsEnabled = true;
                btnReport.IsEnabled = true;
                btnSetup.IsEnabled = true;
            }
            else
            {
                if (false == btnAuto.IsChecked)
                    btnAuto.IsEnabled = false;
                if (false == btnTeach.IsChecked)
                    btnTeach.IsEnabled = false;
                if (false == btnInit.IsChecked)
                    btnInit.IsEnabled = false;
                if (false == btnManual.IsChecked)
                    btnManual.IsEnabled = false;
                if (false == btnSystemLock.IsChecked)
                    btnSystemLock.IsEnabled = false;

                btnRecipe.IsEnabled = false;
                btnReport.IsEnabled = false;
                btnSetup.IsEnabled = false;
            }
        }

        private void lbOnline_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (null != dataService)
            {
                if (1 < dataService.DataSystem.OnlineState)
                    dataService.DataSystem.OnlineState = 1;
                else
                    dataService.DataSystem.OnlineState = 3;

                Log_Trace.WriteLine("MainWindow.lbOnline_PreviewMouseDoubleClick({0})", dataService.DataSystem.OnlineState);
            }
        }

        private int StartExternProcess()
        {
            if ("Virtual" == DataService.Singleton.DataSystem.MachineName)
            {
                return -1;
            }


            if (0 < StopExternProcess())
            {
                DoEvents(500);
            }

            string path = dataService.CurrentPath + "\\SmartIC_Motion";
            System.Diagnostics.Process.Start(path);

            return 0;
        }

        private int StopExternProcess()
        {
            int countProc = 0;

            System.Diagnostics.Process[] mProcess = System.Diagnostics.Process.GetProcessesByName("SmartIC_Motion");

            foreach (System.Diagnostics.Process p in mProcess)
            {
                p.Kill();
                ++countProc;
            }

            return countProc;
        }

        private void DoEvents(int delay)
        {
            DateTime start = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, delay);
            DateTime timeout = start.Add(duration);

            while (true)
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                {
                    ;
                }   ));

                if (timeout < DateTime.Now)
                    break;
            }
        }

    }
}
