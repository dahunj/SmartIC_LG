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
    /// TeachTopPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TeachTopPage : Page
    {
        private SystemService sysService = null;
        private DataService dataService = null;
        private MsgService msgService = null;
        private SequenceService seqService = null;

        private VisionTopService topService = null;

        private string oldValue = "";

        static bool InvokeRequired
        {
            get { return Dispatcher.CurrentDispatcher != Application.Current.Dispatcher; }
        }

        public TeachTopPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dataService = DataService.Singleton;
            msgService = MsgService.Singleton;
            sysService = SystemService.Singleton;
            seqService = SequenceService.Singleton;
            topService = VisionTopService.Singleton;

            topService.EventInitPos += OnEventInitPos;

            SetEventState(this, null);

            sysService.EventState += OnEventState;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            sysService.EventState -= OnEventState;

            topService.EventInitPos -= OnEventInitPos;

            GC.Collect();
        }

        private void cbxJogN_Click(object sender, RoutedEventArgs e)
        {
            if (false == cbxJogN.IsChecked)
            {
                seqService.SetSequence((int)EnumSmartIC.Sequences.feedJogN, 0);

                cbxJogN.IsEnabled = true;
                cbxJogP.IsEnabled = true;
                cbxRMoveN.IsEnabled = true;
                cbxRMoveP.IsEnabled = true;
                cbxScanMove.IsEnabled = true;
                cbxReScanMove.IsEnabled = true;
                cbxOffsetApply.IsEnabled = true;
            }
            else
            {
                //cbxJogN.IsEnabled = false;
                cbxJogP.IsEnabled = false;
                cbxRMoveN.IsEnabled = false;
                cbxRMoveP.IsEnabled = false;
                cbxScanMove.IsEnabled = false;
                cbxReScanMove.IsEnabled = false;
                cbxOffsetApply.IsEnabled = false;

                seqService.SetSequence((int)EnumSmartIC.Sequences.feedJogN, 1);
            }
        }

        private void cbxJogP_Click(object sender, RoutedEventArgs e)
        {
            if (false == cbxJogP.IsChecked)
            {
                seqService.SetSequence((int)EnumSmartIC.Sequences.feedJogP, 0);

                cbxJogN.IsEnabled = true;
                cbxJogP.IsEnabled = true;
                cbxRMoveN.IsEnabled = true;
                cbxRMoveP.IsEnabled = true;
                cbxScanMove.IsEnabled = true;
                cbxReScanMove.IsEnabled = true;
                cbxOffsetApply.IsEnabled = true;
            }
            else
            {
                cbxJogN.IsEnabled = false;
                //cbxJogP.IsEnabled = false;
                cbxRMoveN.IsEnabled = false;
                cbxRMoveP.IsEnabled = false;
                cbxScanMove.IsEnabled = false;
                cbxReScanMove.IsEnabled = false;
                cbxOffsetApply.IsEnabled = false;

                seqService.SetSequence((int)EnumSmartIC.Sequences.feedJogP, 1);
            }
        }

        private void cbxRMoveN_Click(object sender, RoutedEventArgs e)
        {
            if (false == cbxRMoveN.IsChecked)
            {
                seqService.SetSequence((int)EnumSmartIC.Sequences.feedRMoveN, 0);

                cbxJogN.IsEnabled = true;
                cbxJogP.IsEnabled = true;
                cbxRMoveN.IsEnabled = true;
                cbxRMoveP.IsEnabled = true;
                cbxScanMove.IsEnabled = true;
                cbxReScanMove.IsEnabled = true;
                cbxOffsetApply.IsEnabled = true;
            }
            else
            {

                cbxJogN.IsEnabled = false;
                cbxJogP.IsEnabled = false;
                //cbxRMoveN.IsEnabled = false;
                cbxRMoveP.IsEnabled = false;
                cbxScanMove.IsEnabled = false;
                cbxReScanMove.IsEnabled = false;
                cbxOffsetApply.IsEnabled = false;

                double pos = double.Parse(tbxRMove.Text);

                if (0 == seqService.SetSequence((int)EnumSmartIC.Sequences.feedRMoveN, 1, pos))
                {
                    cbxJogN.IsEnabled = true;
                    cbxJogP.IsEnabled = true;
                    //cbxRMoveN.IsEnabled = true;
                    cbxRMoveP.IsEnabled = true;
                    cbxScanMove.IsEnabled = true;
                    cbxReScanMove.IsEnabled = true;
                    cbxOffsetApply.IsEnabled = true;

                    cbxRMoveN.IsChecked = false;
                }
            }
        }

        private void cbxRMoveP_Click(object sender, RoutedEventArgs e)
        {
            if (false == cbxRMoveP.IsChecked)
            {
                seqService.SetSequence((int)EnumSmartIC.Sequences.feedRMoveP, 0);

                cbxJogN.IsEnabled = true;
                cbxJogP.IsEnabled = true;
                cbxRMoveN.IsEnabled = true;
                cbxRMoveP.IsEnabled = true;
                cbxScanMove.IsEnabled = true;
                cbxReScanMove.IsEnabled = true;
                cbxOffsetApply.IsEnabled = true;
            }
            else
            {
                cbxJogN.IsEnabled = false;
                cbxJogP.IsEnabled = false;
                cbxRMoveN.IsEnabled = false;
                //cbxRMoveP.IsEnabled = false;
                cbxScanMove.IsEnabled = false;
                cbxReScanMove.IsEnabled = false;
                cbxOffsetApply.IsEnabled = false;

                double pos = double.Parse(tbxRMove.Text);

                if (0 == seqService.SetSequence((int)EnumSmartIC.Sequences.feedRMoveP, 1, pos))
                {
                    cbxJogN.IsEnabled = true;
                    cbxJogP.IsEnabled = true;
                    cbxRMoveN.IsEnabled = true;
                    //cbxRMoveP.IsEnabled = true;
                    cbxScanMove.IsEnabled = true;
                    cbxReScanMove.IsEnabled = true;
                    cbxOffsetApply.IsEnabled = true;

                    cbxRMoveP.IsChecked = false;
                }
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (true == btnStop.IsChecked)
                sysService.State = SystemService.States.stop;
            else
                sysService.State = SystemService.States.reset;
        }

        private void cbxScanMove_Click(object sender, RoutedEventArgs e)
        {
            if (false == cbxScanMove.IsChecked)
            {
                cbxScanMove.IsChecked = true;
                return;
            }

            cbxJogN.IsEnabled = false;
            cbxJogP.IsEnabled = false;
            cbxRMoveN.IsEnabled = false;
            cbxRMoveP.IsEnabled = false;
            //cbxScanMove.IsEnabled = false;
            cbxReScanMove.IsEnabled = false;
            cbxOffsetApply.IsEnabled = false;

            double pos = 0.0;

            if (0 == seqService.SetSequence((int)EnumSmartIC.Sequences.teachTop, 1, pos))
            {
                cbxJogN.IsEnabled = true;
                cbxJogP.IsEnabled = true;
                cbxRMoveN.IsEnabled = true;
                cbxRMoveP.IsEnabled = true;
                //cbxScanMove.IsEnabled = true;
                cbxReScanMove.IsEnabled = true;
                cbxOffsetApply.IsEnabled = true;

                cbxScanMove.IsChecked = false;
            }
        }

        private void cbxOffsetApply_Click(object sender, RoutedEventArgs e)
        {
            if (false == cbxOffsetApply.IsChecked)
            {
                cbxOffsetApply.IsChecked = true;
                return;
            }

            cbxJogN.IsEnabled = false;
            cbxJogP.IsEnabled = false;
            cbxRMoveN.IsEnabled = false;
            cbxRMoveP.IsEnabled = false;
            cbxScanMove.IsEnabled = false;
            cbxReScanMove.IsEnabled = false;
            //cbxOffsetApply.IsEnabled = false;

            double pos = double.Parse(tbxOffset.Text);

            if (0 == seqService.SetSequence((int)EnumSmartIC.Sequences.feedRMoveP, 1, pos))
            {
                cbxJogN.IsEnabled = true;
                cbxJogP.IsEnabled = true;
                cbxRMoveN.IsEnabled = true;
                cbxRMoveP.IsEnabled = true;
                cbxScanMove.IsEnabled = true;
                cbxReScanMove.IsEnabled = true;
                //cbxOffsetApply.IsEnabled = true;

                cbxOffsetApply.IsChecked = false;
            }
        }

        private void cbxReScanMove_Click(object sender, RoutedEventArgs e)
        {
            if (false == cbxReScanMove.IsChecked)
            {
                cbxReScanMove.IsChecked = true;
                return;
            }

            cbxJogN.IsEnabled = false;
            cbxJogP.IsEnabled = false;
            cbxRMoveN.IsEnabled = false;
            cbxRMoveP.IsEnabled = false;
            cbxScanMove.IsEnabled = false;
            //cbxReScanMove.IsEnabled = false;

            double pos = double.Parse(tbxOffset.Text);

            if (0 == seqService.SetSequence((int)EnumSmartIC.Sequences.teachTop, 1, pos))
            {
                cbxJogN.IsEnabled = true;
                cbxJogP.IsEnabled = true;
                cbxRMoveN.IsEnabled = true;
                cbxRMoveP.IsEnabled = true;
                cbxScanMove.IsEnabled = true;
                //cbxReScanMove.IsEnabled = true;

                cbxReScanMove.IsChecked = false;
            }
        }

        private void tbx_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            VirtualKeyboardService.GetSingleton().FireVirtualKeyboard(sender);
        }

        private void tbxNumber_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            VirtualKeyboardService.GetSingleton().FireVirtualKeyNumber(sender);
        }

        private void tbx_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            double value;

            if (false == double.TryParse(textBox.Text, out value))
            {
                // Error 처리    
                msgService.ShowMessage((int)EnumSmartIC.LightAlarms.inputError, false);
                textBox.Text = oldValue;
            }
            else
            {
                oldValue = textBox.Text;
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
                    cbxJogN.IsEnabled = true;
                    cbxJogP.IsEnabled = true;
                    cbxRMoveN.IsEnabled = true;
                    cbxRMoveP.IsEnabled = true;
                    cbxScanMove.IsEnabled = true;
                    cbxReScanMove.IsEnabled = true;
                    break;
                case SystemService.States.lightAlarm:
                    break;
                case SystemService.States.stop:
                case SystemService.States.heavyAlarm:
                case SystemService.States.emg:
                    cbxJogN.IsEnabled = false;
                    cbxJogP.IsEnabled = false;
                    cbxRMoveN.IsEnabled = false;
                    cbxRMoveP.IsEnabled = false;
                    cbxScanMove.IsEnabled = false;
                    cbxReScanMove.IsEnabled = false;

                    cbxJogN.IsChecked = false;
                    cbxJogP.IsChecked = false;
                    cbxRMoveN.IsChecked = false;
                    cbxRMoveP.IsChecked = false;
                    cbxScanMove.IsChecked = false;
                    cbxReScanMove.IsChecked = false;
                    break;
            }
        }

        private void OnEventInitPos(object send, EventArgs e)
        {
            if (InvokeRequired)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                            (ThreadStart)delegate()
                                            {
                                                SetEventInitPos(send, e);
                                            }
                );
            }
            else
            {
                SetEventInitPos(send, e);
            }
        }

        private void SetEventInitPos(object send, EventArgs e)
        {
            tbxRMove.Text = dataService.JobInitOffset.ToString("0.000");
        }

    }
}
