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
    /// ManualFeedingPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ManualFeedingPage : Page
    {
        private SystemService sysService = null;
        private MotionService motionService = null;
        private DataService dataService = null;
        private MsgService msgService = null;
        private SequenceService seqService = null;

        private DispatcherTimer timer = null;

        private string oldValue = "";

        private GroupControl gpJog = null;

        private int AxisVision { get; set; }
        private int AxisPunch { get; set; }
        private bool IsUnload { get; set; }

        private bool IsRun { get; set; }


        static bool InvokeRequired
        {
            get { return Dispatcher.CurrentDispatcher != Application.Current.Dispatcher; }
        }

        
        public ManualFeedingPage()
        {
            InitializeComponent();

            AxisVision = (int)EnumSmartIC.Axis.visionFeed;
            AxisPunch = (int)EnumSmartIC.Axis.punchFeed;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            IsUnload = false;
            IsRun = false;

            motionService = MotionService.Singleton;
            dataService = DataService.Singleton;
            msgService = MsgService.Singleton;
            sysService = SystemService.Singleton;
            seqService = SequenceService.Singleton;

            btnVisionJogN.Axis = AxisVision;
            btnVisionJogP.Axis = AxisVision;
            btnVisionRMoveN.Axis = AxisVision;
            btnVisionRMoveP.Axis = AxisVision;
            btnVisionAMove.Axis = AxisVision;

            btnPunchJogN.Axis = AxisPunch;
            btnPunchJogP.Axis = AxisPunch;
            btnPunchRMoveN.Axis = AxisPunch;
            btnPunchRMoveP.Axis = AxisPunch;
            btnPunchAMove.Axis = AxisPunch;

            gpJog = new GroupControl();
            gpJog.Add(cbxVisionJog);
            gpJog.Add(cbxVisionRMove);
            gpJog.Add(cbxVisionAMove);

            gpJog.Add(cbxPunchJog);
            gpJog.Add(cbxPunchRMove);
            gpJog.Add(cbxPunchAMove);

            cbxVisionJog.IsChecked = false;
            cbxVisionRMove.IsChecked = false;
            cbxVisionAMove.IsChecked = false;

            cbxVisionJog.Add((ISelectControl)btnVisionJogN);
            cbxVisionJog.Add((ISelectControl)btnVisionJogP);

            cbxVisionRMove.Add((ISelectControl)btnVisionRMoveN);
            cbxVisionRMove.Add((ISelectControl)btnVisionRMoveP);
            //cbxVisionRMove.Add((ISelectControl)tbxVisionRMove);
 
            cbxVisionAMove.Add((ISelectControl)btnVisionAMove);
            //cbxVisionRMove.Add((ISelectControl)tbxVisionAMove);

            cbxPunchJog.IsChecked = false;
            cbxPunchRMove.IsChecked = false;
            cbxPunchAMove.IsChecked = false;

            cbxPunchJog.Add((ISelectControl)btnPunchJogN);
            cbxPunchJog.Add((ISelectControl)btnPunchJogP);

            cbxPunchRMove.Add((ISelectControl)btnPunchRMoveN);
            cbxPunchRMove.Add((ISelectControl)btnPunchRMoveP);
            //cbxVisionRMove.Add((ISelectControl)tbxPunchRMove);

            cbxPunchAMove.Add((ISelectControl)btnPunchAMove);
            //cbxVisionRMove.Add((ISelectControl)tbxPunchAMove);

            sysService.EventState += OnEventState;

            SetTimer();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            IsUnload = true;

            if (null != gpJog)
            {
                gpJog.RemoveAll();
            }

            ResetTimer();

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

        private void btnMove_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;

            if (null == motionService)
            {
                cbx.IsChecked = false;
                return;
            }

            if (false == cbx.IsChecked)
            {
                cbx.IsChecked = true;
                return;
            }

            double pos = 0;

            seqService.SetSequence((int)EnumSmartIC.Sequences.visionForward, 0);
            seqService.SetSequence((int)EnumSmartIC.Sequences.visionBackward, 0);
            seqService.SetSequence((int)EnumSmartIC.Sequences.punchForward, 0);
            seqService.SetSequence((int)EnumSmartIC.Sequences.punchBackward, 0);

            IsRun = true;

            if (null != cbx)
            {
                switch (cbx.Tag.ToString())
                {
                    case "btnPunchAMove":
                        pos = double.Parse(tbxPunchAMove.Text);
                        
                        if (0.0 < pos)
                            seqService.SetSequence((int)EnumSmartIC.Sequences.punchForward, 1);
                        else
                            seqService.SetSequence((int)EnumSmartIC.Sequences.punchBackward, 1);

                        System.Windows.Forms.Application.DoEvents();

                        motionService.AMove(AxisPunch, pos, dataService.DataMotion[AxisVision].VelNormal, dataService.DataMotion[AxisVision].AccelNormal);

                        btnPunchJogP.IsEnabled = false;
                        btnPunchJogN.IsEnabled = false;
                        //btnPunchAMove.IsEnabled = false;
                        btnPunchRMoveN.IsEnabled = false;
                        btnPunchRMoveP.IsEnabled = false;

                        btnVisionJogP.IsEnabled = false;
                        btnVisionJogN.IsEnabled = false;
                        //btnVisionAMove.IsEnabled = false;
                        btnVisionRMoveN.IsEnabled = false;
                        btnVisionRMoveP.IsEnabled = false;
                        break;
                    case "btnPunchRMoveN":
                        pos = double.Parse(tbxPunchRMove.Text);

                        seqService.SetSequence((int)EnumSmartIC.Sequences.punchBackward, 1);

                        System.Windows.Forms.Application.DoEvents();

                        motionService.RMove(AxisPunch, -pos, dataService.DataMotion[AxisVision].VelRapid, dataService.DataMotion[AxisVision].AccelRapid);

                        btnPunchJogP.IsEnabled = false;
                        btnPunchJogN.IsEnabled = false;
                        btnPunchAMove.IsEnabled = false;
                        //btnPunchRMoveN.IsEnabled = false;
                        btnPunchRMoveP.IsEnabled = false;

                        btnVisionJogP.IsEnabled = false;
                        btnVisionJogN.IsEnabled = false;
                        btnVisionAMove.IsEnabled = false;
                        //btnVisionRMoveN.IsEnabled = false;
                        btnVisionRMoveP.IsEnabled = false;
                        break;
                    case "btnPunchRMoveP":
                        pos = double.Parse(tbxPunchRMove.Text);

                        seqService.SetSequence((int)EnumSmartIC.Sequences.punchForward, 1);
                        
                        System.Windows.Forms.Application.DoEvents();

                        motionService.RMove(AxisPunch, pos, dataService.DataMotion[AxisVision].VelRapid, dataService.DataMotion[AxisVision].AccelRapid);

                        btnPunchJogP.IsEnabled = false;
                        btnPunchJogN.IsEnabled = false;
                        btnPunchAMove.IsEnabled = false;
                        btnPunchRMoveN.IsEnabled = false;
                        //btnPunchRMoveP.IsEnabled = false;

                        btnVisionJogP.IsEnabled = false;
                        btnVisionJogN.IsEnabled = false;
                        btnVisionAMove.IsEnabled = false;
                        btnVisionRMoveN.IsEnabled = false;
                        //btnVisionRMoveP.IsEnabled = false;
                        break;

                    case "btnVisionAMove":
                        pos = double.Parse(tbxVisionAMove.Text);
                        
                        if (0.0 < pos)
                            seqService.SetSequence((int)EnumSmartIC.Sequences.visionForward, 1);
                        else
                            seqService.SetSequence((int)EnumSmartIC.Sequences.visionBackward, 1);

                        System.Windows.Forms.Application.DoEvents();

                        motionService.AMove(AxisVision, pos, dataService.DataMotion[AxisVision].VelRapid, dataService.DataMotion[AxisVision].AccelRapid);

                        btnPunchJogP.IsEnabled = false;
                        btnPunchJogN.IsEnabled = false;
                        //btnPunchAMove.IsEnabled = false;
                        btnPunchRMoveN.IsEnabled = false;
                        btnPunchRMoveP.IsEnabled = false;

                        btnVisionJogP.IsEnabled = false;
                        btnVisionJogN.IsEnabled = false;
                        //btnVisionAMove.IsEnabled = false;
                        btnVisionRMoveN.IsEnabled = false;
                        btnVisionRMoveP.IsEnabled = false;
                        break;
                    case "btnVisionRMoveN":
                        pos = double.Parse(tbxVisionRMove.Text);

                        seqService.SetSequence((int)EnumSmartIC.Sequences.visionBackward, 1);

                        System.Windows.Forms.Application.DoEvents();


                        motionService.RMove(AxisVision, -pos, dataService.DataMotion[AxisVision].VelRapid, dataService.DataMotion[AxisVision].AccelRapid);

                        btnPunchJogP.IsEnabled = false;
                        btnPunchJogN.IsEnabled = false;
                        btnPunchAMove.IsEnabled = false;
                        //btnPunchRMoveN.IsEnabled = false;
                        btnPunchRMoveP.IsEnabled = false;

                        btnVisionJogP.IsEnabled = false;
                        btnVisionJogN.IsEnabled = false;
                        btnVisionAMove.IsEnabled = false;
                        //btnVisionRMoveN.IsEnabled = false;
                        btnVisionRMoveP.IsEnabled = false;
                        break;
                    case "btnVisionRMoveP":
                        pos = double.Parse(tbxVisionRMove.Text);
                        
                        seqService.SetSequence((int)EnumSmartIC.Sequences.visionForward, 1);
                        
                        System.Windows.Forms.Application.DoEvents();

                        motionService.RMove(AxisVision, pos, dataService.DataMotion[AxisVision].VelRapid, dataService.DataMotion[AxisVision].AccelRapid);

                        btnPunchJogP.IsEnabled = false;
                        btnPunchJogN.IsEnabled = false;
                        btnPunchAMove.IsEnabled = false;
                        btnPunchRMoveN.IsEnabled = false;

                        //btnPunchRMoveP.IsEnabled = false;
                        btnVisionJogP.IsEnabled = false;
                        btnVisionJogN.IsEnabled = false;
                        btnVisionAMove.IsEnabled = false;
                        btnVisionRMoveN.IsEnabled = false;
                        //btnVisionRMoveP.IsEnabled = false;
                        break;

                    
                    default:
                        return;
                }

                while (false == motionService.IsMotionDone(AxisVision))
                {
                    if (true == IsUnload)
                        break;

                    System.Windows.Forms.Application.DoEvents();
                }

                while (false == motionService.IsMotionDone(AxisPunch))
                {
                    if (true == IsUnload)
                        break;

                    System.Windows.Forms.Application.DoEvents();
                }

                seqService.SetSequence((int)EnumSmartIC.Sequences.visionForward, 0);
                seqService.SetSequence((int)EnumSmartIC.Sequences.visionBackward, 0);
                seqService.SetSequence((int)EnumSmartIC.Sequences.punchForward, 0);
                seqService.SetSequence((int)EnumSmartIC.Sequences.punchBackward, 0);

                System.Windows.Forms.Application.DoEvents();

                cbx.IsChecked = false;

                btnPunchJogP.IsEnabled = true;
                btnPunchJogN.IsEnabled = true;
                btnPunchAMove.IsEnabled = true;
                btnPunchRMoveN.IsEnabled = true;
                btnPunchRMoveP.IsEnabled = true;

                btnVisionJogP.IsEnabled = true;
                btnVisionJogN.IsEnabled = true;
                btnVisionAMove.IsEnabled = true;
                btnVisionRMoveN.IsEnabled = true;
                btnVisionRMoveP.IsEnabled = true;

                IsRun = false;


            }


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

        private void btnJog_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (null == motionService)
                return;

            Button btn = sender as Button;

            IsRun = true;

            seqService.SetSequence((int)EnumSmartIC.Sequences.visionForward, 0);
            seqService.SetSequence((int)EnumSmartIC.Sequences.visionBackward, 0);
            seqService.SetSequence((int)EnumSmartIC.Sequences.punchForward, 0);
            seqService.SetSequence((int)EnumSmartIC.Sequences.punchBackward, 0);

            switch (btn.Tag.ToString())
            {
                case "btnPunchJogN":
                    seqService.SetSequence((int)EnumSmartIC.Sequences.punchBackward, 1);
                    System.Windows.Forms.Application.DoEvents();

                    motionService.JogN(AxisPunch, dataService.DataMotion[AxisVision].VelRapid, dataService.DataMotion[AxisVision].AccelRapid);

                    btnPunchJogP.IsEnabled = false;
                    btnPunchAMove.IsEnabled = false;
                    btnPunchRMoveN.IsEnabled = false;
                    btnPunchRMoveP.IsEnabled = false;

                    btnVisionJogP.IsEnabled = false;
                    btnVisionAMove.IsEnabled = false;
                    btnVisionRMoveN.IsEnabled = false;
                    btnVisionRMoveP.IsEnabled = false;
                    break;
                case "btnPunchJogP":
                    seqService.SetSequence((int)EnumSmartIC.Sequences.punchForward, 1);
                    System.Windows.Forms.Application.DoEvents();

                    motionService.JogP(AxisPunch, dataService.DataMotion[AxisVision].VelRapid, dataService.DataMotion[AxisVision].AccelRapid);

                    btnPunchJogN.IsEnabled = false;
                    btnPunchAMove.IsEnabled = false;
                    btnPunchRMoveN.IsEnabled = false;
                    btnPunchRMoveP.IsEnabled = false;

                    btnVisionJogN.IsEnabled = false;
                    btnVisionAMove.IsEnabled = false;
                    btnVisionRMoveN.IsEnabled = false;
                    btnVisionRMoveP.IsEnabled = false;
                    break;

                case "btnVisionJogN":
                    seqService.SetSequence((int)EnumSmartIC.Sequences.visionBackward, 1);
                    System.Windows.Forms.Application.DoEvents();

                    motionService.JogN(AxisVision, dataService.DataMotion[AxisVision].VelNormal, dataService.DataMotion[AxisVision].AccelNormal);

                    btnPunchJogP.IsEnabled = false;
                    btnPunchAMove.IsEnabled = false;
                    btnPunchRMoveN.IsEnabled = false;
                    btnPunchRMoveP.IsEnabled = false;

                    btnVisionJogP.IsEnabled = false;
                    btnVisionAMove.IsEnabled = false;
                    btnVisionRMoveN.IsEnabled = false;
                    btnVisionRMoveP.IsEnabled = false;
                    break;
                case "btnVisionJogP":
                    seqService.SetSequence((int)EnumSmartIC.Sequences.visionForward, 1);
                    System.Windows.Forms.Application.DoEvents();

                    motionService.JogP(AxisVision, dataService.DataMotion[AxisVision].VelNormal, dataService.DataMotion[AxisVision].AccelNormal);

                    btnPunchJogN.IsEnabled = false;
                    btnPunchAMove.IsEnabled = false;
                    btnPunchRMoveN.IsEnabled = false;
                    btnPunchRMoveP.IsEnabled = false;

                    btnVisionJogN.IsEnabled = false;
                    btnVisionAMove.IsEnabled = false;
                    btnVisionRMoveN.IsEnabled = false;
                    btnVisionRMoveP.IsEnabled = false;
                    break;
                default:
                    IsRun = false;
                    break;
            }
        }

        private void btnJog_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (null == motionService)
                return;

            motionService.Stop(AxisPunch);
            motionService.Stop(AxisVision);

            seqService.SetSequence((int)EnumSmartIC.Sequences.visionForward, 0);
            seqService.SetSequence((int)EnumSmartIC.Sequences.visionBackward, 0);
            seqService.SetSequence((int)EnumSmartIC.Sequences.punchForward, 0);
            seqService.SetSequence((int)EnumSmartIC.Sequences.punchBackward, 0);

            System.Windows.Forms.Application.DoEvents();

            btnPunchJogP.IsEnabled = true;
            btnPunchJogN.IsEnabled = true;
            btnPunchAMove.IsEnabled = true;
            btnPunchRMoveN.IsEnabled = true;
            btnPunchRMoveP.IsEnabled = true;

            btnVisionJogP.IsEnabled = true;
            btnVisionJogN.IsEnabled = true;
            btnVisionAMove.IsEnabled = true;
            btnVisionRMoveN.IsEnabled = true;
            btnVisionRMoveP.IsEnabled = true;

            IsRun = false;
        }

        private void btnAutoRun_Click(object sender, RoutedEventArgs e)
        {
            if (true == btnAutoRun.IsChecked)
                sysService.State = SystemService.States.run;
            else
                sysService.State = SystemService.States.stop;
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
                    break;
                case SystemService.States.lightAlarm:
                    break;
                case SystemService.States.stop:
                case SystemService.States.heavyAlarm:
                case SystemService.States.emg:
                    IsRun = false;
                    btnAutoRun.IsChecked = false;
                    break;
            }
        }



        private void timer_Tick(object sender, EventArgs e)
        {
            DisplayState();
        }

        private void DisplayState()
        {
            if ( null == seqService)
                return;
            if (null == motionService)
                return;

            if (false == seqService.IsFlag((int)EnumSmartIC.SeqFlags.bufferReady))
            {
                if (true == btnVisionJogN.IsEnabled)
                {
                    btnVisionJogN.IsEnabled = false;
                    btnVisionJogP.IsEnabled = false;
                    btnVisionRMoveN.IsEnabled = false;
                    btnVisionRMoveP.IsEnabled = false;
                    btnVisionAMove.IsEnabled = false;

                    btnPunchJogN.IsEnabled = false;
                    btnPunchJogP.IsEnabled = false;
                    btnPunchRMoveN.IsEnabled = false;
                    btnPunchRMoveP.IsEnabled = false;
                    btnPunchAMove.IsEnabled = false;
                }
            }
            else
            {
                if (false == IsRun)
                {
                    if (false == btnVisionJogN.IsEnabled)
                    {
                        btnVisionJogN.IsEnabled = true;
                        btnVisionJogP.IsEnabled = true;
                        btnVisionRMoveN.IsEnabled = true;
                        btnVisionRMoveP.IsEnabled = true;
                        btnVisionAMove.IsEnabled = true;

                        btnPunchJogN.IsEnabled = true;
                        btnPunchJogP.IsEnabled = true;
                        btnPunchRMoveN.IsEnabled = true;
                        btnPunchRMoveP.IsEnabled = true;
                        btnPunchAMove.IsEnabled = true;
                    }
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
