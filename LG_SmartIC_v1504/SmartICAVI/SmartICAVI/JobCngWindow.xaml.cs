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
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;

namespace SmartICAVI
{
    /// <summary>
    /// JobCngWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class JobCngWindow : Window
    {
        private JobWindowService jobWindowService = null;
        private DioService dioService = null;
        private DataService dataService = null;

        private DispatcherTimer timer = null;
        private DispatcherTimer timerTwLamp = null;

        private Image img;
        //private string imgPath;

        private int count = 0;

        public bool IsStart { get; set; }

        public JobCngWindow()
        {
            InitializeComponent();

            IsStart = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataService = DataService.Singleton;
            dioService = DioService.Singleton;
            jobWindowService = JobWindowService.Singleton;

            jobWindowService.IsCNGWindowClosed = false;

            count = 0;

            DisplayText();

            dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);

            SetTimer();
            SetTimerTwLamp();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ResetTimer();
            ResetTimerTwLamp();

            dioService.ResetOutport((int)EnumSmartIC.Outports.buzzer1);
            dioService.ResetOutport((int)EnumSmartIC.Outports.twlampYellow);

            jobWindowService.IsCNGWindowClosed = true;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;

            ResetTimer();
            ResetTimerTwLamp();

            dioService.ResetOutport((int)EnumSmartIC.Outports.buzzer1);
            dioService.ResetOutport((int)EnumSmartIC.Outports.twlampYellow);

            jobWindowService.ResultHoleWindow = true;
            this.Close();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;

            ResetTimer();
            ResetTimerTwLamp();

            dioService.ResetOutport((int)EnumSmartIC.Outports.buzzer1);
            dioService.ResetOutport((int)EnumSmartIC.Outports.twlampYellow);

            jobWindowService.ResultHoleWindow = false;
            this.Close();
        }

        private void btnBuzzer_Click(object sender, RoutedEventArgs e)
        {
            ResetTimer();
            dioService.ResetOutport((int)EnumSmartIC.Outports.buzzer1);
        }

        private void SetTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = new System.TimeSpan(0, 0, 0, 30);
            timer.IsEnabled = true;
            timer.Tick += new EventHandler(timer_Tick);


            //timerTwLamp = new DispatcherTimer();
            //timerTwLamp.Interval = new System.TimeSpan(0, 0, 0, 0, 500);
            //timerTwLamp.IsEnabled = true;
            //timerTwLamp.Tick += new EventHandler(timerTwLamp_Tick);
        }

        private void ResetTimer()
        {
            if (null != timer)
            {
                timer.IsEnabled = false;
                timer.Tick -= timerTwLamp_Tick;
            }

            //if (null != timerTwLamp)
            //{
            //    timerTwLamp.IsEnabled = false;
            //    timerTwLamp.Tick -= timerTwLamp_Tick;
            //}
        }

        private void SetTimerTwLamp()
        {
            timerTwLamp = new DispatcherTimer();
            timerTwLamp.Interval = new System.TimeSpan(0, 0, 0, 0, 500);
            timerTwLamp.IsEnabled = true;
            timerTwLamp.Tick += new EventHandler(timerTwLamp_Tick);
        }

        private void ResetTimerTwLamp()
        {
            if (null != timerTwLamp)
            {
                timerTwLamp.IsEnabled = false;
                timerTwLamp.Tick -= timerTwLamp_Tick;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (null != dioService)
            {
                if (0 == ++count % 3)
                {
                    dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                }
                else
                {
                    dioService.ResetOutport((int)EnumSmartIC.Outports.buzzer1);
                }
            }
        }

        private void timerTwLamp_Tick(object sender, EventArgs e)
        {
            if (null != dioService)
            {
                dioService.ToggleOutport((int)EnumSmartIC.Outports.twlampYellow);
            }
        }

        private void DisplayText()
        {
            int number = (int)EnumSmartIC.LightAlarms.cngStart;

            if( false == IsStart )
                number = (int)EnumSmartIC.LightAlarms.cngEnd;

            string strNumber = string.Format("메시지 코드({0})", (int)number);
            string message;
            string description;
            string imgPath = dataService.DataPath + "\\Images\\LightAlarm";

            string path = dataService.IniPath + "\\LightAlarm.ini";
            string section = "LIGHT_ALARM";
            string key = string.Format("{0}", number);
            string key_description = string.Format("{0}_Description", number);
            string key_Image = string.Format("{0}_Image", number);

            IniFile ini = new IniFile();

            message = ini.Read(section, key, "Unknown Message", path);
            description = ini.Read(section, key_description, "설정되지 않은 메시지입니다. 메시지 번호를 통보 해 주십시오.", path);
            imgPath = dataService.DataPath + "\\Images\\LightAlarm\\" + ini.Read(section, key_Image, "Default.jpg", path);

            description = description.Replace("\\n", Environment.NewLine);

            Log_Trace.WriteLine(message);

            //msg.TitleText = title;
            tblMessageNum.Text = strNumber;
            tblMessage.Text = message;
            tblDescription.Text = description;

            img = new Image();

            Uri uriImage = new Uri(imgPath);
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
            }

            if (null != bmp)
            {
                image.Source = bmp;
                image.Stretch = Stretch.Fill;
            }
        }
    }
}
