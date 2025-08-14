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

using SmartICAVI.UserControls;

namespace SmartICAVI
{
    /// <summary>
    /// TeachBottomPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TeachBottomPage : Page
    {
        private SystemService sysService = null;
        private DataService dataService = null;
        private MsgService msgService = null;
        private SequenceService seqService = null;

        static bool InvokeRequired
        {
            get { return Dispatcher.CurrentDispatcher != Application.Current.Dispatcher; }
        }
       
        public TeachBottomPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dataService = DataService.Singleton;
            msgService = MsgService.Singleton;
            sysService = SystemService.Singleton;
            seqService = SequenceService.Singleton;

            SetEventState(this, null);

            sysService.EventState += OnEventState;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            sysService.EventState -= OnEventState;

            GC.Collect();
        }

        private void cbxScanMove_Click(object sender, RoutedEventArgs e)
        {
            if (false == cbxScanMove.IsChecked)
            {
                cbxScanMove.IsChecked = true;
                return;
            }

            if (0 == seqService.SetSequence((int)EnumSmartIC.Sequences.teachBottom, 1, 0.0))
            {
                cbxScanMove.IsChecked = false;
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (true == btnStop.IsChecked)
                sysService.State = SystemService.States.stop;
            else
                sysService.State = SystemService.States.reset;
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
                    break;
                case SystemService.States.ready:
                    break;
                case SystemService.States.idle:
                    break;
                case SystemService.States.run:
                    break;
                case SystemService.States.pause:
                    break;
                case SystemService.States.resume:
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
                    cbxScanMove.IsEnabled = true;
                    break;
                case SystemService.States.lightAlarm:
                    break;
                case SystemService.States.stop:
                case SystemService.States.heavyAlarm:
                case SystemService.States.emg:
                    cbxScanMove.IsEnabled = false;
                    cbxScanMove.IsChecked = false;
                    break;
            }
        }

    }
}
