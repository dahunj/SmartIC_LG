using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;

using SmartICAVI.Animation;

namespace SmartICAVI
{
    /// <summary>
    /// ManualPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ManualPage : Page
    {
        private DioService dioService = null;
        private MotionService motionService = null;
        //private SequenceService seqService = null;
        private DataService dataService = null;
        private SystemService sysService = null;
        private MsgService msgService = null;

        private DispatcherTimer timer = null;

        private Label[] ledInports;
        private Label[] lblInports;

        private double[] poss = new double[4];
        private int[] limitNs = new int[4];
        private int[] limitPs = new int[4];

        private Label[] lbPoss;
        private Label[] lbLimitNs;
        private Label[] lbLimitPs;
        private Label[] lbAxisNames;

        private double ratioX = 1.0;
        private double ratioY = 1.0;

        private BitmapImage bmpMachine = null;

        public ManualPage()
        {
            InitializeComponent();

            ledInports = new Label[64]{
                ledInport000,    ledInport001,     ledInport002,     ledInport003,     ledInport004,     ledInport005,     ledInport006,     ledInport007,     ledInport008,      ledInport009, 
                ledInport010,    ledInport011,    ledInport012,    ledInport013,    ledInport014,    ledInport015,    ledInport016,    ledInport017,    ledInport018,      ledInport019, 
                ledInport020,    ledInport021,    ledInport022,    ledInport023,    ledInport024,    ledInport025,    ledInport026,    ledInport027,    ledInport028,      ledInport029, 
                ledInport030,    ledInport031,

                ledInport100,     ledInport101,     ledInport102,     ledInport103,     ledInport104,     ledInport105,     ledInport106,     ledInport107,     ledInport108,      ledInport109, 
                ledInport110,    ledInport111,    ledInport112,    ledInport113,    ledInport114,    ledInport115,    ledInport116,    ledInport117,    ledInport118,      ledInport119, 
                ledInport120,    ledInport121,    ledInport122,    ledInport123,    ledInport124,    ledInport125,    ledInport126,    ledInport127,    ledInport128,      ledInport129, 
                ledInport130,    ledInport131,
            };

            lblInports = new Label[64]{
                lblInport000,      lblInport001,      lblInport002,      lblInport003,      lblInport004,      lblInport005,      lblInport006,      lblInport007,      lblInport008,      lblInport009, 
                lblInport010,     lblInport011,      lblInport012,      lblInport013,      lblInport014,      lblInport015,      lblInport016,      lblInport017,      lblInport018,      lblInport019, 
                lblInport020,     lblInport021,      lblInport022,      lblInport023,      lblInport024,      lblInport025,      lblInport026,      lblInport027,      lblInport028,      lblInport029, 
                lblInport030,     lblInport031,

                lblInport100,      lblInport101,      lblInport102,      lblInport103,      lblInport104,      lblInport105,      lblInport106,      lblInport107,      lblInport108,      lblInport109, 
                lblInport110,     lblInport111,      lblInport112,      lblInport113,      lblInport114,      lblInport115,      lblInport116,      lblInport117,      lblInport118,      lblInport119, 
                lblInport120,     lblInport121,      lblInport122,      lblInport123,      lblInport124,      lblInport125,      lblInport126,      lblInport127,      lblInport128,      lblInport129, 
                lblInport130,     lblInport131,
            };

            lbPoss = new Label[9] { lbCoordAxis00, lbCoordAxis01, lbCoordAxis02, lbCoordAxis03, lbCoordAxis04, lbCoordAxis05, lbCoordAxis06, lbCoordAxis07, lbCoordAxis08 };
            lbLimitNs = new Label[9] { ledLimitNAxis00, ledLimitNAxis01, ledLimitNAxis02, ledLimitNAxis03, ledLimitNAxis04, ledLimitNAxis05, ledLimitNAxis06, ledLimitNAxis07, ledLimitNAxis08 };
            lbLimitPs = new Label[9] { ledLimitPAxis00, ledLimitPAxis01, ledLimitPAxis02, ledLimitPAxis03, ledLimitPAxis04, ledLimitPAxis05, ledLimitPAxis06, ledLimitPAxis07, ledLimitPAxis08 };
            lbAxisNames = new Label[9] { lbAxis00, lbAxis01, lbAxis02, lbAxis03, lbAxis04, lbAxis05, lbAxis06, lbAxis07, lbAxis08 };

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dioService = DioService.Singleton;
            motionService = MotionService.Singleton;
            dataService = DataService.Singleton;
            msgService = MsgService.Singleton;
            sysService = SystemService.Singleton;

            sysService.Mode = SystemService.Modes.manual;

            IniFile ini = new IniFile();
            string path = DataService.Singleton.IniPath + "\\string.ini";
            string section = "INPORT";
            string key;
            string def;
            string value = "";

            for (int i = 0; i < 32; ++i)
            {
                key = string.Format("X{0:000}", i);
                def = key;
                value = ini.Read(section, key, def, path);
                lblInports[i].Content = value;
            }

            for (int i = 0; i < 32; ++i)
            {
                key = string.Format("X{0:000}", i + 100);
                def = key;
                value = ini.Read(section, key, def, path);
                lblInports[i + 32].Content = value;
            }

            section = "AXIS";
            for (int i = 0; i < 9; ++i)
            {
                key = string.Format("AXIS{0:00}", i);
                def = key;
                value = ini.Read(section, key, def, path);
                lbAxisNames[i].Content = value;
            }

            string imgPath = DataService.Singleton.DataPath + "\\Images\\Common\\Machine_Front.jpg";

            Uri uriMachine = new Uri(imgPath);

            bmpMachine = null;

            // Image Load
            try
            {
                bmpMachine = new BitmapImage(uriMachine);
            }
            catch (Exception exc)
            {
                uriMachine = new Uri("pack://application:,,/Images/Stop48.png");
                bmpMachine = new BitmapImage(uriMachine);

                Log_Exception.WriteLine("ManualPage.Page_Loaded() : " + exc.Message);
            }

            if (null != bmpMachine)
            {
                image.Source = bmpMachine;

                InitButtons();
            }

            //cbxDio.IsChecked = true;
            

            DisplayState();
            SetTimer();

            cbxFeeding.IsChecked = true;
            //cbx_Click((object)cbxDio, null);

            cbxLight.IsChecked = dioService.IsOutportOn((int)EnumSmartIC.Outports.machineLight);
            
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            ResetTimer();

            GC.Collect();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if( true == btnStop.IsChecked )
                sysService.State = SystemService.States.stop;
            else
                sysService.State = SystemService.States.reset;
        }

        private void InitButtons()
        {
            if (null == bmpMachine)
                return;

            double mX = bmpMachine.Width;
            double mY = bmpMachine.Height;

            double canvX = canvas.ActualWidth;
            double canvY = canvas.ActualHeight;

            ratioX = canvX / mX;
            ratioY = canvY / mY;

            // Axis0 Reel Uncoiler
            cbxUncoiler.Width = 120.0 * ratioX;
            cbxUncoiler.Height = 120.0 * ratioY;
            Canvas.SetLeft(cbxUncoiler, 90.0 * ratioX);
            Canvas.SetTop(cbxUncoiler, 250.0 * ratioY);

            // Axis1 Vision - Recoiler Sprocket
            cbxVisionFeed.Width = 120.0 * ratioX;
            cbxVisionFeed.Height = 60.0 * ratioY;
            Canvas.SetLeft(cbxVisionFeed, 510.0 * ratioX);
            Canvas.SetTop(cbxVisionFeed, 410.0 * ratioY);

            // Axis1 Vision - Recoiler Sprocket
            cbxRecoiler.Width = 120.0 * ratioX;
            cbxRecoiler.Height = 120.0 * ratioY;
            Canvas.SetLeft(cbxRecoiler, 1015.0 * ratioX);
            Canvas.SetTop(cbxRecoiler, 250.0 * ratioY);

            // Axis2 Vision - Top Z
            cbxVisionTop.Width = 120.0 * ratioX;
            cbxVisionTop.Height = 80.0 * ratioY;
            Canvas.SetLeft(cbxVisionTop, 390.0 * ratioX);
            Canvas.SetTop(cbxVisionTop, 265.0 * ratioY);


            // Axis3 Vision - Bottom Z
            cbxVisionBottom.Width = 120.0 * ratioX;
            cbxVisionBottom.Height = 80.0 * ratioY;
            Canvas.SetLeft(cbxVisionBottom, 420.0 * ratioX);
            Canvas.SetTop(cbxVisionBottom, 490.0 * ratioY);


            // Punch - X
            cbxPunchX.Width = 120.0 * ratioX;
            cbxPunchX.Height = 60.0 * ratioY;
            Canvas.SetLeft(cbxPunchX, 720.0 * ratioX);
            Canvas.SetTop(cbxPunchX, 340.0 * ratioY);

            // Punch - Y
            cbxPunchY.Width = 120.0 * ratioX;
            cbxPunchY.Height = 60.0 * ratioY;
            Canvas.SetLeft(cbxPunchY, 720.0 * ratioX);
            Canvas.SetTop(cbxPunchY, 410.0 * ratioY);

            // Axis4 Punch - Recoiler Sprocket
            cbxPunchFeed.Width = 120.0 * ratioX;
            cbxPunchFeed.Height = 60.0 * ratioY;
            Canvas.SetLeft(cbxPunchFeed, 850.0 * ratioX);
            Canvas.SetTop(cbxPunchFeed, 410.0 * ratioY);

            // 입출력
            cbxDio.Width = 120.0 * ratioX;
            cbxDio.Height = 60.0 * ratioY;
            Canvas.SetLeft(cbxDio, 1015.0 * ratioX);
            Canvas.SetTop(cbxDio, 50.0 * ratioY);

            // 피딩
            cbxFeeding.Width = 500.0 * ratioX;
            cbxFeeding.Height = 60.0 * ratioY;
            Canvas.SetLeft(cbxFeeding, 400.0 * ratioX);
            Canvas.SetTop(cbxFeeding, 600.0 * ratioY);

            // 수동완공
            cbxEnd.Width = 120.0 * ratioX;
            cbxEnd.Height = 60.0 * ratioY;
            Canvas.SetLeft(cbxEnd, 90.0 * ratioX);
            Canvas.SetTop(cbxEnd, 50.0 * ratioY);

            // 조명
            cbxLight.Width = 500.0 * ratioX;
            cbxLight.Height = 60.0 * ratioY;
            Canvas.SetLeft(cbxLight, 400.0 * ratioX);
            Canvas.SetTop(cbxLight, 170.0 * ratioY);
        }

        private void cbx_Click(object sender, RoutedEventArgs e)
        {
            //if (null == motionService)
            //    return;
            //if (null == dioService)
            //    return;

            CheckBox btn = sender as CheckBox;

            //string page = "ManualDioPage.xaml";

            cbxDio.IsChecked = false;
            cbxPunchFeed.IsChecked = false;
            cbxPunchX.IsChecked = false;
            cbxPunchY.IsChecked = false;
            cbxRecoiler.IsChecked = false;
            cbxUncoiler.IsChecked = false;
            cbxVisionBottom.IsChecked = false;
            cbxVisionFeed.IsChecked = false;
            cbxVisionTop.IsChecked = false;
            cbxFeeding.IsChecked = false;
            cbxEnd.IsChecked = false;

            if (null != btn)
                btn.IsChecked = true;

            Page targetPage;

            switch (btn.Tag.ToString())
            {
                case "cbxDio":
                    //page = "ManualDioPage.xaml";
                    targetPage = new ManualDioPage();
                    break;
                case "cbxEnd":
                    targetPage = new ManualEndPage();
                    break;
                case "cbxPunchFeed":
                    ////page = "ManualPunchFeedPage.xaml";
                    targetPage = new ManualPunchFeedPage();
                    break;
                case "cbxPunchX":
                    //page = "ManualPunchXPage.xaml";
                    targetPage = new ManualPunchXPage();
                    break;
                case "cbxPunchY":
                    //page = "ManualPunchYPage.xaml";
                    targetPage = new ManualPunchYPage();
                    break;
                case "cbxRecoiler":
                    //page = "ManualRecoilerPage.xaml";
                    targetPage = new ManualRecoilerPage();
                    break;
                case "cbxUncoiler":
                    //page = "ManualUncoilerPage.xaml";
                    targetPage = new ManualUncoilerPage();
                    break;
                case "cbxVisionBottom":
                    //page = "ManualVisionBottomPage.xaml";
                    targetPage = new ManualVisionBottomPage();
                    break;
                case "cbxVisionFeed":
                    //page = "ManualVisionFeedPage.xaml";
                    targetPage = new ManualVisionFeedPage();
                    break;
                case "cbxVisionTop":
                    //page = "ManualVisionTopPage.xaml";
                    targetPage = new ManualVisionTopPage();
                    break;
                case "cbxFeeding":
                    //page = "ManualFeeding2Page.xaml";
                    targetPage = new ManualFeeding2Page();
                    break;
                default:
                    return;
            }

            //jogPage.Source = new Uri(page, UriKind.Relative);

            this.jogPage.NavigateToExample(targetPage);

            GC.Collect();
        }


        private void tbx_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //VirtualKeyboardService.GetSingleton().FireVirtualKeyboard(sender);
        }

        private void tbxNumber_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //VirtualKeyboardService.GetSingleton().FireVirtualKeyNumber(sender);
        }

        private void btnMove_Click(object sender, RoutedEventArgs e)
        {
        }

        private void tbx_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //TextBox textBox = sender as TextBox;

            //double value;

            //if (false == double.TryParse(textBox.Text, out value))
            //{
            //    // Error 처리    
            //    msgService.ShowMessage(MessageService.Messages.inputerror, false);
            //    textBox.Text = oldValue;
            //}
            //else
            //{
            //    oldValue = textBox.Text;
            //}
        }

        private void btnJog_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //if (null == motionService)
            //    return;

            //Button btn = sender as Button;

            //switch (btn.Tag.ToString())
            //{
            //    case "JogN":
            //        motionService.JogN(0, (double.Parse(tbJog.Text)) * 1000.0 / 60.0, 1000);
            //        break;
            //    case "JogP":
            //        motionService.JogP(0, (double.Parse(tbJog.Text)) * 1000.0 / 60.0, 1000);
            //        break;
            //    default:
            //        break;
            //}
        }

        private void btnJog_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //if (null == motionService)
            //    return;

            //motionService.Stop(0);
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            DisplayState();
        }

        private void DisplayState()
        {
            if (null == motionService)
                return;

            if (null == dioService)
                return;

            UInt32 inports0 = dioService.Inports0;
            UInt32 inports1 = dioService.Inports1;

            UInt32 flag_in0 = 0;
            UInt32 flag_in1 = 0;

            for (int i = 0; i < 32; ++i)
            {
                flag_in0 = inports0 & 0x0001;
                flag_in1 = inports1 & 0x0001;

                ledInports[i].IsEnabled = (1 == flag_in0) ? true : false;
                ledInports[i + 32].IsEnabled = (1 == flag_in1) ? true : false;

                inports0 = inports0 >> 1;
                inports1 = inports1 >> 1;
            }

            for (int i = 0; i < 9; ++i)
            {
                lbPoss[i].Content = motionService.GetCurrentPosition(i).ToString("0.000");
                lbLimitNs[i].IsEnabled = (1 == motionService.GetStateLimitN(i)) ? true : false;
                lbLimitPs[i].IsEnabled = (1 == motionService.GetStateLimitP(i)) ? true : false;
            }

            
            UInt32 outports0 = dioService.Outports0;
            UInt32 flag_out0 = outports0 & 0x0040;

            cbxLight.IsChecked = (0x0040 == flag_out0) ? true : false;
        }

        private void SetTimer()
        {
            // Set Timer
            timer = new DispatcherTimer();
            timer.Interval = new System.TimeSpan(0, 0, 0, 0, 100);
            timer.IsEnabled = true;
            timer.Tick += new EventHandler(timer_Tick);

            //timer_Tick(null, null);
        }

        private void ResetTimer()
        {
            timer.IsEnabled = false;
            timer.Tick -= timer_Tick;
        }

        private void cbxLight_Click(object sender, RoutedEventArgs e)
        {
            if( true == cbxLight.IsChecked )
                dioService.SetOutport((int)EnumSmartIC.Outports.machineLight);
            else
                dioService.ResetOutport((int)EnumSmartIC.Outports.machineLight);
        }
    }
}
