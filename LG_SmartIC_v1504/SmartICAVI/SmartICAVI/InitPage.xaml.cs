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
    /// InitPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InitPage : Page
    {
        #region Variables
        SystemService sysService = null;
        SequenceService seqService = null;
        #endregion

        static bool InvokeRequired
        {
            get { return Dispatcher.CurrentDispatcher != Application.Current.Dispatcher; }
        }

        public InitPage()
        {
            InitializeComponent();

            lblSensorCheck.IsEnabled = false;
            lblTopZ.IsEnabled = false;
            lblBottomZ.IsEnabled = false;
            lblPunchX.IsEnabled = false;
            lblPunchY.IsEnabled = false;
            lblInitDone.IsEnabled = false;

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Log_Trace.WriteLine("[InitPage] Show");

            ucMachine.IsShowMachine = true;

            sysService = SystemService.Singleton;
            seqService = SequenceService.Singleton;

            sysService.EventState += OnEventState;
            seqService.EventInitStep += OnEventInitStep;
            seqService.EventInitString += OnEventInitString;

            sysService.Mode = SystemService.Modes.home;

            OnEventState(this, null);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            sysService.EventState -= OnEventState;
            seqService.EventInitStep -= OnEventInitStep;
            seqService.EventInitString -= OnEventInitString;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;

            if (null != cbx)
            {
                if (true == cbx.IsChecked)
                    sysService.State = SystemService.States.stop;
                else
                    sysService.State = SystemService.States.reset;
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;

            if (null != cbx)
            {
                if (false == cbx.IsChecked)
                {
                    cbx.IsChecked = true;
                    return;
                }

                listbox.Items.Clear();
                sysService.State = SystemService.States.run;
            }
        }

        private void OnEventState(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                            (ThreadStart)delegate()
                                            {
                                                SetEventState(sender, e);
                                            }
                );
            }
            else
            {
                SetEventState(sender, e);
            }
        }

        private void SetEventState(object sender, EventArgs e)
        {
            switch (sysService.State)
            {
                case SystemService.States.none:// = 0, 
                    break;
                case SystemService.States.ready:
                    break;
                case SystemService.States.idle:
                    break;
                case SystemService.States.run:
                    Start();
                    break;
                case SystemService.States.pause:
                    Pause();
                    break;
                case SystemService.States.resume:
                    Resume();
                    break;
                case SystemService.States.homing:
                    break;
                case SystemService.States.homeDone:
                    break;
                case SystemService.States.jobDone:
                    break;
                case SystemService.States.systemLock:
                    break;
                case SystemService.States.systemRelease:
                    break;
                case SystemService.States.reset:
                    Reset();
                    break;
                case SystemService.States.lightAlarm:
                    break;
                case SystemService.States.stop:
                case SystemService.States.heavyAlarm:
                case SystemService.States.emg:
                    Stop();
                    break;
            }
        }

        private void OnEventInitStep(int step)
        {
            if (InvokeRequired)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                            (ThreadStart)delegate()
                                            {
                                                SetEventInitStep(step);
                                            }
                );
            }
            else
            {
                SetEventInitStep(step);
            }
        }

        private void SetEventInitStep(int step)
        {
            switch (step)
            {
                case 0:     // Sensor Check
                    lblSensorCheck.IsEnabled = true;
                    lblSensorCheck.Background = Brushes.SkyBlue;
                    break;

                case 1000:
                    lblSensorCheck.Background = Brushes.DarkGreen;
                    break;

                case 1100:   // TopZ   구동
                    lblTopZ.IsEnabled = true;
                    lblTopZ.Background = Brushes.SkyBlue;
                    break;
                case 1110:   // BottomZ   구동
                    lblBottomZ.IsEnabled = true;
                    lblBottomZ.Background = Brushes.SkyBlue;
                    break;
                case 1120:   // PunchX   구동
                    lblPunchX.IsEnabled = true;
                    lblPunchX.Background = Brushes.SkyBlue;
                    break;
                case 1130:   // PunchY   구동
                    lblPunchY.IsEnabled = true;
                    lblPunchY.Background = Brushes.SkyBlue;
                    break;

                case 1200:   // TopZ   완료
                    //lblTopZ.IsEnabled = false;
                    lblTopZ.Background = Brushes.DarkGreen;
                    break;
                case 1210:   // BottomZ   완료
                    //lblBottomZ.IsEnabled = false;
                    lblBottomZ.Background = Brushes.DarkGreen;
                    break;
                case 1220:   // PunchX   완료
                    //lblPunchX.IsEnabled = false;
                    lblPunchX.Background = Brushes.DarkGreen;
                    break;
                case 1230:   // PunchY   완료
                    //lblPunchY.IsEnabled = false;
                    lblPunchY.Background = Brushes.DarkGreen;
                    break;

                case 300:
                    //lblInitDone.Background = Brushes.DarkGreen;
                    //btnStart.IsChecked = false;
                    //lblInitDone.IsEnabled = true;
                    break;

                case 20000:
                    lblInitDone.Background = Brushes.DarkGreen;
                    btnStart.IsChecked = false;
                    lblInitDone.IsEnabled = true;
                    break;
                case 40000:
                    break;

            }
        }

        private void OnEventInitString(string text)
        {
            DateTime curTime = DateTime.Now;

            if (InvokeRequired)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                            (ThreadStart)delegate()
                                            {
                                                listbox.Items.Insert(0, string.Format("[{0}] {1}", curTime.ToLocalTime(), text));
                                            }
                );
            }
            else
            {
                listbox.Items.Insert(0, string.Format("[{0}] {1}", curTime.ToLocalTime(), text));
            }
            
            //listbox.Items.Add( string.Format("[{0}] {1}", curTime.ToLocalTime(), text) );
        }

        private void Start()
        {
        }

        private void Pause()
        {
        }

        private void Resume()
        {
        }

        private void Reset()
        {
            lblSensorCheck.IsEnabled = false;
            lblTopZ.IsEnabled = false;
            lblBottomZ.IsEnabled = false;
            lblPunchX.IsEnabled = false;
            lblPunchY.IsEnabled = false;
            lblInitDone.IsEnabled = false;

            lblSensorCheck.Background = Brushes.Transparent;
            lblTopZ.Background = Brushes.Transparent;
            lblBottomZ.Background = Brushes.Transparent;
            lblPunchX.Background = Brushes.Transparent;
            lblPunchY.Background = Brushes.Transparent;
            lblInitDone.Background = Brushes.Transparent;

            btnStart.IsEnabled = true;
        }

        private void Stop()
        {
            lblSensorCheck.IsEnabled = false;
            lblTopZ.IsEnabled = false;
            lblBottomZ.IsEnabled = false;
            lblPunchX.IsEnabled = false;
            lblPunchY.IsEnabled = false;
            lblInitDone.IsEnabled = false;

            lblSensorCheck.Background = Brushes.DarkRed;
            lblTopZ.Background = Brushes.DarkRed;
            lblBottomZ.Background = Brushes.DarkRed;
            lblPunchX.Background = Brushes.DarkRed;
            lblPunchY.Background = Brushes.DarkRed;
            lblInitDone.Background = Brushes.DarkRed;

            btnStart.IsChecked = false;
            btnStart.IsEnabled = false;
        }
    }
}
