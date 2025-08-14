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
    /// JobMessageWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class JobMessageWindow : Window
    {
        private JobWindowService jobWindowService = null;
        private DioService dioService = null;
        private DataService dataService = null;
        private SystemService sysService = null;

        private DispatcherTimer timer = null;
        private DispatcherTimer timerTwLamp = null;

        private Image img;

        private int count = 0;

        public int Index { get; set; }
        public string Message { get; set; }


        public JobMessageWindow()
        {
            InitializeComponent();

            Index = 0;
            Message = "";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataService = DataService.Singleton;
            dioService = DioService.Singleton;
            jobWindowService = JobWindowService.Singleton;
            sysService = SystemService.Singleton;

            jobWindowService.IsMessageWindowClosed = false;

            count = 0;

            DisplayText();

            if (true == dataService.DataSystem.UseLightAlarmBuzzer)
            {
                dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);

                SetTimer();
            }

            SetTimerTwLamp();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ResetTimer();
            ResetTimerTwLamp();

            dioService.ResetOutport((int)EnumSmartIC.Outports.buzzer1);
            dioService.ResetOutport((int)EnumSmartIC.Outports.twlampYellow);

            jobWindowService.IsMessageWindowClosed = true;
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;

            ResetTimer();
            ResetTimerTwLamp();

            dioService.ResetOutport((int)EnumSmartIC.Outports.buzzer1);
            dioService.ResetOutport((int)EnumSmartIC.Outports.twlampYellow);

            jobWindowService.ResultMessageWindow = 0;

            this.Close();
        }

        private void btnBuzzer_Click(object sender, RoutedEventArgs e)
        {
            ResetTimer();
            dioService.ResetOutport((int)EnumSmartIC.Outports.buzzer1);
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;

            ResetTimer();
            dioService.ResetOutport((int)EnumSmartIC.Outports.buzzer1);

            int step = 0;

            while (SystemService.States.stop > sysService.State)
            {
                switch (step)
                {
                    case 0:
                        step += 10;
                        break;
                    case 10:
                        jobWindowService.ShowRecheckWindow((int)EnumSmartIC.LightAlarms.jobAbort);
                        step += 10;
                        break;
                    case 20:
                        if (true == jobWindowService.IsRecheckWindowClosed)
                            step += 10;
                        break;
                    case 30:
                        if (0 == jobWindowService.ResultRecheckWindow)
                            step = 20000;
                        else
                            step = 10000;
                        break;

                    case 10000:
                        return;

                    case 20000:
                        jobWindowService.ResultMessageWindow = -1;

                        ResetTimer();
                        ResetTimerTwLamp();

                        dioService.ResetOutport((int)EnumSmartIC.Outports.buzzer1);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.twlampYellow);

                        this.Close();
                        break;
                    default:
                        return;
                }

                System.Windows.Forms.Application.DoEvents();
            }
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;

            ResetTimer();
            dioService.ResetOutport((int)EnumSmartIC.Outports.buzzer1);

            int step = 0;

            while (SystemService.States.stop > sysService.State)
            {
                switch (step)
                {
                    case 0:
                        step += 10;
                        break;
                    case 10:
                        jobWindowService.ShowRecheckWindow((int)EnumSmartIC.LightAlarms.reStart);
                        step += 10;
                        break;
                    case 20:
                        if (true == jobWindowService.IsRecheckWindowClosed)
                            step += 10;
                        break;
                    case 30:
                        if (0 == jobWindowService.ResultRecheckWindow)
                            step = 20000;
                        else
                            step = 10000;
                        break;

                    case 10000:
                        return;

                    case 20000:
                        jobWindowService.ResultMessageWindow = 1;

                        ResetTimer();
                        ResetTimerTwLamp();

                        dioService.ResetOutport((int)EnumSmartIC.Outports.buzzer1);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.twlampYellow);

                        this.Close();
                        break;
                    default:
                        return;
                }

                System.Windows.Forms.Application.DoEvents();
            }
        }

        private void SetTimer()
        {
            count = 0;

            timer = new DispatcherTimer();
            timer.Interval = new System.TimeSpan(0, 0, 0, 1);
            timer.IsEnabled = true;
            timer.Tick += new EventHandler(timer_Tick);
        }

        private void ResetTimer()
        {
            if (null != timer)
            {
                timer.IsEnabled = false;
                timer.Tick -= timerTwLamp_Tick;
            }
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
                
                if (count == dataService.DataSystem.LightAlarmBuzzerTime)
                {
                    dioService.ResetOutport((int)EnumSmartIC.Outports.buzzer1);
                }
                else if (count < dataService.DataSystem.LightAlarmBuzzerTime)
                {
                    dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                }

                if (++count > 60)
                    count = 0;
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
            int number = Index;


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

            message = Message + " " +ini.Read(section, key, "Unknown Message", path);
            description = ini.Read(section, key_description, "설정되지 않은 메시지입니다. 메시지 번호를 통보 해 주십시오.", path);
            imgPath = dataService.DataPath + "\\Images\\LightAlarm\\" + ini.Read(section, key_Image, "Default.jpg", path);

            description = description.Replace("\\n", Environment.NewLine);

            if ("" != Message)
            {
                description = Message + " " + description; 
            }

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
