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
    /// MonitorDioPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MonitorDioPage : Page
    {
        private Label[] ledInports;
        private Label[] lblInports;
        private Label[] ledOutports;
        private Label[] lblOutports;

        DioService dioService = null;
        SystemService sysService = null;

        private DispatcherTimer timer = null;

        public MonitorDioPage()
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

            ledOutports = new Label[32]{
                ledOutport000,      ledOutport001,      ledOutport002,      ledOutport003,      ledOutport004,      ledOutport005,      ledOutport006,      ledOutport007,      ledOutport008,      ledOutport009, 
                ledOutport010,      ledOutport011,      ledOutport012,      ledOutport013,      ledOutport014,      ledOutport015,      ledOutport016,      ledOutport017,      ledOutport018,      ledOutport019, 
                ledOutport020,      ledOutport021,      ledOutport022,      ledOutport023,      ledOutport024,      ledOutport025,      ledOutport026,      ledOutport027,      ledOutport028,      ledOutport029, 
                ledOutport030,      ledOutport031,
            };

            lblOutports = new Label[32]{
                lblOutport000,       lblOutport001,      lblOutport002,      lblOutport003,      lblOutport004,      lblOutport005,      lblOutport006,      lblOutport007,      lblOutport008,      lblOutport009, 
                lblOutport010,      lblOutport011,      lblOutport012,      lblOutport013,      lblOutport014,      lblOutport015,      lblOutport016,      lblOutport017,      lblOutport018,      lblOutport019, 
                lblOutport020,      lblOutport021,      lblOutport022,      lblOutport023,      lblOutport024,      lblOutport025,      lblOutport026,      lblOutport027,      lblOutport028,      lblOutport029, 
                lblOutport030,      lblOutport031,
            };
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

            for (int i = 0; i < 32; ++i)
            {
                key = string.Format("X{0:000}", i);
                def = key;
                value = ini.Read(section, key, def, path);
                lblInports[i].Content = value;
            }

            for (int i = 0; i < 32; ++i)
            {
                key = string.Format("X{0:000}", i+100);
                def = key;
                value = ini.Read(section, key, def, path);
                lblInports[i+32].Content = value;
            }

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
            ResetTimer();

            //dioService.EventInport -= OnEventInport;
            //dioService.EventOutport -= OnEventOutport;
        }

        private void DisplayLED()
        {
            if (null == dioService)
                return;

            UInt32 inports0 = dioService.Inports0;
            UInt32 inports1 = dioService.Inports1;
            UInt32 outports0 = dioService.Outports0;

            UInt32 flag_in0 = 0;
            UInt32 flag_in1 = 0;
            UInt32 flag_out0 = 0;

            for (int i = 0; i < 32; ++i)
            {
                flag_in0 = inports0 & 0x0001;
                flag_in1 = inports1 & 0x0001;
                flag_out0 = outports0 & 0x0001;

                ledInports[i].IsEnabled = (1 == flag_in0) ? true : false;
                ledInports[i + 32].IsEnabled = (1 == flag_in1) ? true : false;

                ledOutports[i].IsEnabled = (1 == flag_out0) ? true : false;

                inports0 = inports0 >> 1;
                inports1 = inports1 >> 1;
                outports0 = outports0 >> 1;
            }
        }

        private void OnEventInport(int port, int value)
        {
            ledInports[port].IsEnabled = (1 == value) ? true : false;
        }

        private void OnEventOutport(int port, int value)
        {
            ledOutports[port].IsEnabled = (1 == value) ? true : false;
        }

        private void SetTimer()
        {
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

        private void timer_Tick(object sender, EventArgs e)
        {
            DisplayLED();
        }

    }
}
