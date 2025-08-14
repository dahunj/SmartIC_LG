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
    /// ManualDioPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ManualDioPage : Page
    {
        //private Label[] ledInports;
        //private Label[] lblInports;
        private CheckBox[] cbxOutports;
        private Label[] lblOutports;

        private DioService dioService = null;
        private SystemService sysService = null;

        private DispatcherTimer timer = null;

        public ManualDioPage()
        {
            InitializeComponent();

            cbxOutports = new CheckBox[32]{
                cbxOutport000,      cbxOutport001,      cbxOutport002,      cbxOutport003,      cbxOutport004,      cbxOutport005,      cbxOutport006,      cbxOutport007,      cbxOutport008,      cbxOutport009, 
                cbxOutport010,      cbxOutport011,      cbxOutport012,      cbxOutport013,      cbxOutport014,      cbxOutport015,      cbxOutport016,      cbxOutport017,      cbxOutport018,      cbxOutport019, 
                cbxOutport020,      cbxOutport021,      cbxOutport022,      cbxOutport023,      cbxOutport024,      cbxOutport025,      cbxOutport026,      cbxOutport027,      cbxOutport028,      cbxOutport029, 
                cbxOutport030,      cbxOutport031,
            };

            lblOutports = new Label[32]{
                lbOutport000,       lbOutport001,      lbOutport002,      lbOutport003,      lbOutport004,      lbOutport005,      lbOutport006,      lbOutport007,      lbOutport008,      lbOutport009, 
                lbOutport010,      lbOutport011,      lbOutport012,      lbOutport013,      lbOutport014,      lbOutport015,      lbOutport016,      lbOutport017,      lbOutport018,      lbOutport019, 
                lbOutport020,      lbOutport021,      lbOutport022,      lbOutport023,      lbOutport024,      lbOutport025,      lbOutport026,      lbOutport027,      lbOutport028,      lbOutport029, 
                lbOutport030,      lbOutport031,
            };

            for (int i = 0; i < 32; ++i)
            {
                cbxOutports[i].Click += cbxOutport_Check;
                cbxOutports[i].Tag = i;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dioService = DioService.Singleton;
            sysService = SystemService.Singleton;

            IniFile ini = new IniFile();
            string path = DataService.Singleton.IniPath + "\\string.ini";
            string section = "INPORT";
            string key;
            string def;
            string value = "";

            section = "OUTPORT";
            for (int i = 0; i < lblOutports.Length; ++i)
            {
                key = string.Format("Y{0:000}", i);
                def = key;
                value = ini.Read(section, key, def, path);
                lblOutports[i].Content = value;
            }

            DisplayLED();

            //dioService.EventInport += OnEventInport;
            //dioService.EventOutport += OnEventOutport;
            SetTimer();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            //dioService.EventInport -= OnEventInport;
            //dioService.EventOutport -= OnEventOutport;
            ResetTimer();
        }

        private void DisplayLED()
        {
            if (null == dioService)
                return;

            UInt32 outports0 = dioService.Outports0;

            UInt32 flag_out0 = 0;

            for (int i = 0; i < 32; ++i)
            {
                flag_out0 = outports0 & 0x0001;

                cbxOutports[i].IsChecked = (1 == flag_out0) ? true : false;
                outports0 = outports0 >> 1;
            }
        }

        private void OnEventInport(int port, int value)
        {
            //ledInports[port].IsEnabled = (1 == value) ? true : false;
        }

        private void OnEventOutport(int port, int value)
        {
            cbxOutports[port].IsChecked = (1 == value) ? true : false;
        }

        private void SetTimer()
        {
            ResetTimer();

            timer = new DispatcherTimer();
            timer.Interval = new System.TimeSpan(0, 0, 0, 0, 200);
            timer.IsEnabled = true;
            timer.Tick += new EventHandler(timer_Tick);

            //timer_Tick(null, null);
        }

        private void ResetTimer()
        {
            if (null != timer)
            {
                timer.IsEnabled = false;
                timer.Tick -= timer_Tick;
                timer = null;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            DisplayLED();
        }

        private void cbxOutport_Check(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;

            if (null != cbx)
            {
                int port = int.Parse(cbx.Tag.ToString());

                if (true == cbx.IsChecked)
                    dioService.SetOutport(port);
                else
                    dioService.ResetOutport(port);
            }
        }
    }
}
