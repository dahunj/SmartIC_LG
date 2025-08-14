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

namespace SmartICAVI
{
    /// <summary>
    /// MonitorMotionPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MonitorMotionPage : Page
    {
        private MotionService motionService = null;
        private DispatcherTimer timer = null;

        private double[] poss = new double[4];
        private int[] limitNs = new int[4];
        private int[] limitPs = new int[4];
        private int[] homes = new int[4];
        private int[] ampErrors = new int[4];

        private Label[] lbPoss;
        private Label[] lbLimitNs;
        private Label[] lbLimitPs;
        private Label[] lbHomes;
        private Label[] lbAmpErrors;
        private Label[] lbAxisNames;

        public MonitorMotionPage()
        {
            InitializeComponent();

            lbPoss = new Label[9] { lblCoordAxis00, lblCoordAxis01, lblCoordAxis02, lblCoordAxis03, lblCoordAxis04, lblCoordAxis05, lblCoordAxis06, lblCoordAxis07, lblCoordAxis08 };
            lbLimitNs = new Label[9] { ledLimitMAxis00, ledLimitMAxis01, ledLimitMAxis02, ledLimitMAxis03, ledLimitMAxis04, ledLimitMAxis05, ledLimitMAxis06, ledLimitMAxis07, ledLimitMAxis08 };
            lbLimitPs = new Label[9] { ledLimitPAxis00, ledLimitPAxis01, ledLimitPAxis02, ledLimitPAxis03, ledLimitPAxis04, ledLimitPAxis05, ledLimitPAxis06, ledLimitPAxis07, ledLimitPAxis08 };
            lbHomes = new Label[9] { ledHomeAxis00, ledHomeAxis01, ledHomeAxis02, ledHomeAxis03, ledHomeAxis04, ledHomeAxis05, ledHomeAxis06, ledHomeAxis07, ledHomeAxis08 };
            lbAmpErrors = new Label[9] { ledAmpFaultAxis00, ledAmpFaultAxis01, ledAmpFaultAxis02, ledAmpFaultAxis03, ledAmpFaultAxis04, ledAmpFaultAxis05, ledAmpFaultAxis06, ledAmpFaultAxis07, ledAmpFaultAxis08 };
            lbAxisNames = new Label[9] { lblNameAxis00, lblNameAxis01, lblNameAxis02, lblNameAxis03, lblNameAxis04, lblNameAxis05, lblNameAxis06, lblNameAxis07, lblNameAxis08 };
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            motionService = MotionService.Singleton;

            IniFile ini = new IniFile();
            string path = DataService.Singleton.IniPath + "\\string.ini";
            string section = "AXIS";
            string key;
            string def;
            string value = "";

            for (int i = 0; i < 9; ++i)
            {
                key = string.Format("AXIS{0:00}", i);
                def = key;
                value = ini.Read(section, key, def, path);
                lbAxisNames[i].Content = value;
            }


            DisplayStatus();

            SetTimer();
            
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            ResetTimer();
            
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            DisplayStatus();
        }

        private void DisplayStatus()
        {
            if (null != motionService)
            {
                for (int i = 0; i < 9; ++i)
                {
                    lbPoss[i].Content = motionService.GetCurrentPosition(i).ToString("0.000");
                    lbLimitNs[i].IsEnabled = (1 == motionService.GetStateLimitN(i)) ? true : false;
                    lbLimitPs[i].IsEnabled = (1 == motionService.GetStateLimitP(i)) ? true : false;
                    lbHomes[i].IsEnabled = (1 == motionService.GetStateHome(i)) ? true : false;
                    lbAmpErrors[i].IsEnabled = (1 == motionService.GetStateAmpAlarm(i)) ? true : false;
                }
            }
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
    }
}
