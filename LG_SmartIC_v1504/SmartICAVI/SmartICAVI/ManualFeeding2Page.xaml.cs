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
    /// ManualFeeding2Page.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ManualFeeding2Page : Page
    {
        private SystemService sysService = null;
        private MotionService motionService = null;
        private DataService dataService = null;
        private MsgService msgService = null;
        private SequenceService seqService = null;

        //private DispatcherTimer timer = null;

        private string oldValue = "";

        static bool InvokeRequired
        {
            get { return Dispatcher.CurrentDispatcher != Application.Current.Dispatcher; }
        }
        
        public ManualFeeding2Page()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            motionService = MotionService.Singleton;
            dataService = DataService.Singleton;
            msgService = MsgService.Singleton;
            sysService = SystemService.Singleton;
            seqService = SequenceService.Singleton;

            cbxAutoRun.IsChecked = seqService.IsFlag((int)EnumSmartIC.SeqFlags.bufferReady);

            SetEventState(this, null);

            sysService.EventState += OnEventState;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            sysService.EventState -= OnEventState;

            GC.Collect();
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

        private void cbxAutoRun_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cbxJogN_Click(object sender, RoutedEventArgs e)
        {
            if (false == cbxJogN.IsChecked)
            {
                //cbxJogN.IsChecked = true;
                //return;
                seqService.SetSequence((int)EnumSmartIC.Sequences.feedJogN, 0);

                //cbxJogN.IsEnabled = true;
                cbxJogP.IsEnabled = true;
                cbxRMoveN.IsEnabled = true;
                cbxRMoveP.IsEnabled = true;
                cbxAMove.IsEnabled = true;
            }
            else
            {
                //cbxJogN.IsEnabled = false;
                cbxJogP.IsEnabled = false;
                cbxRMoveN.IsEnabled = false;
                cbxRMoveP.IsEnabled = false;
                cbxAMove.IsEnabled = false;

                seqService.FireEventManualJog(1);
                seqService.SetSequence((int)EnumSmartIC.Sequences.feedJogN, 1);
                seqService.FireEventManualJog(0);
            }
        }

        private void cbxJogP_Click(object sender, RoutedEventArgs e)
        {
            if (false == cbxJogP.IsChecked)
            {
                //cbxJogP.IsChecked = true;
                //return;

                seqService.SetSequence((int)EnumSmartIC.Sequences.feedJogP, 0);

                cbxJogN.IsEnabled = true;
                cbxJogP.IsEnabled = true;
                cbxRMoveN.IsEnabled = true;
                cbxRMoveP.IsEnabled = true;
                cbxAMove.IsEnabled = true;
            }
            else
            {
                cbxJogN.IsEnabled = false;
                //cbxJogP.IsEnabled = false;
                cbxRMoveN.IsEnabled = false;
                cbxRMoveP.IsEnabled = false;
                cbxAMove.IsEnabled = false;

                seqService.FireEventManualJog(1);
                seqService.SetSequence((int)EnumSmartIC.Sequences.feedJogP, 1);
                seqService.FireEventManualJog(0);
            }
        }

        private void cbxRMoveN_Click(object sender, RoutedEventArgs e)
        {
            if (false == cbxRMoveN.IsChecked)
            {
                //cbxRMoveN.IsChecked = true;
                //return;
                seqService.SetSequence((int)EnumSmartIC.Sequences.feedRMoveN, 0);

                cbxJogN.IsEnabled = true;
                cbxJogP.IsEnabled = true;
                cbxRMoveN.IsEnabled = true;
                cbxRMoveP.IsEnabled = true;
                cbxAMove.IsEnabled = true;
            }
            else
            {
                cbxJogN.IsEnabled = false;
                cbxJogP.IsEnabled = false;
                //cbxRMoveN.IsEnabled = false;
                cbxRMoveP.IsEnabled = false;
                cbxAMove.IsEnabled = false;

                double pos = double.Parse(tbxRMove.Text);

                seqService.FireEventManualJog(1);

                if (0 == seqService.SetSequence((int)EnumSmartIC.Sequences.feedRMoveN, 1, pos))
                {
                    cbxJogN.IsEnabled = true;
                    cbxJogP.IsEnabled = true;
                    //cbxRMoveN.IsEnabled = true;
                    cbxRMoveP.IsEnabled = true;
                    cbxAMove.IsEnabled = true;

                    cbxRMoveN.IsChecked = false;
                }
                seqService.FireEventManualJog(0);
            }
        }

        private void cbxRMoveP_Click(object sender, RoutedEventArgs e)
        {
            if (false == cbxRMoveP.IsChecked)
            {
                //cbxRMoveP.IsChecked = true;
                //return;
                seqService.SetSequence((int)EnumSmartIC.Sequences.feedRMoveP, 0);

                cbxJogN.IsEnabled = true;
                cbxJogP.IsEnabled = true;
                cbxRMoveN.IsEnabled = true;
                cbxRMoveP.IsEnabled = true;
                cbxAMove.IsEnabled = true;
            }
            else
            {
                cbxJogN.IsEnabled = false;
                cbxJogP.IsEnabled = false;
                cbxRMoveN.IsEnabled = false;
                //cbxRMoveP.IsEnabled = false;
                cbxAMove.IsEnabled = false;

                double pos = double.Parse(tbxRMove.Text);

                seqService.FireEventManualJog(1);

                if (0 == seqService.SetSequence((int)EnumSmartIC.Sequences.feedRMoveP, 1, pos))
                {
                    cbxJogN.IsEnabled = true;
                    cbxJogP.IsEnabled = true;
                    cbxRMoveN.IsEnabled = true;
                    //cbxRMoveP.IsEnabled = true;
                    cbxAMove.IsEnabled = true;

                    cbxRMoveP.IsChecked = false;
                }
                seqService.FireEventManualJog(0);
            }
        }

        private void cbxAMove_Click(object sender, RoutedEventArgs e)
        {
            if (false == cbxAMove.IsChecked)
            {
                seqService.SetSequence((int)EnumSmartIC.Sequences.feedAMove, 0, 0.0);

                cbxJogN.IsEnabled = true;
                cbxJogP.IsEnabled = true;
                cbxRMoveN.IsEnabled = true;
                cbxRMoveP.IsEnabled = true;
                cbxAMove.IsEnabled = true;
            }
            else
            {

                cbxJogN.IsEnabled = false;
                cbxJogP.IsEnabled = false;
                cbxRMoveN.IsEnabled = false;
                cbxRMoveP.IsEnabled = false;
                //cbxAMove.IsEnabled = false;

                double pos = double.Parse(tbxAMove.Text);

                seqService.FireEventManualJog(1);

                if (0 == seqService.SetSequence((int)EnumSmartIC.Sequences.feedAMove, 1, pos))
                {
                    cbxJogN.IsEnabled = true;
                    cbxJogP.IsEnabled = true;
                    cbxRMoveN.IsEnabled = true;
                    cbxRMoveP.IsEnabled = true;
                    //cbxAMove.IsEnabled = true;

                    cbxAMove.IsChecked = false;
                }

                seqService.FireEventManualJog(0);
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            // Uncoiler 정지
            seqService.SetSequence((int)EnumSmartIC.Sequences.uncoilerBuffer, 0);
            // Recoiler 정지
            seqService.SetSequence((int)EnumSmartIC.Sequences.recoilerBuffer, 0);

            seqService.FireEventManualJog(0);
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
                    cbxAMove.IsEnabled = true;
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
                    cbxAMove.IsEnabled = false;

                    cbxJogN.IsChecked = false;
                    cbxJogP.IsChecked = false;
                    cbxRMoveN.IsChecked = false;
                    cbxRMoveP.IsChecked = false;
                    cbxAMove.IsChecked = false;
                    break;
            }
        }


    }
}
