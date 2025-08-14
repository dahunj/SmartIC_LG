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
    /// JobInitWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class JobInitWindow : Window
    {
        private DataService dataService = null;
        private MotionService motionService = null;
        private SystemService sysService = null;
        private SequenceService seqService = null;
        private VisionTopService topService = null;
        private int axisVision = 0;
        private int axisPunch = 0;

        public Window OwnerWindow { get; set; }

        private int Vel { get; set; }

        static bool InvokeRequired
        {
            get { return Dispatcher.CurrentDispatcher != Application.Current.Dispatcher; }
        }

        public JobInitWindow()
        {
            InitializeComponent();

            OwnerWindow = null;

            //cbRMove.Items.Add("0.1");
            //cbRMove.Items.Add("0.5");
            //cbRMove.Items.Add("1.0");
            cbRMove.Items.Add("4.75");
            cbRMove.Items.Add("10.0");
            cbRMove.Items.Add("50.0");
            cbRMove.Items.Add("100.0");
            cbRMove.Items.Add("500.0");
            cbRMove.Items.Add("1000.0");
            cbRMove.Items.Add("1500.0");
            cbRMove.Items.Add("2000.0");
            cbRMove.Items.Add("2200.0");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnOK.IsDefault = true;

            dataService = DataService.Singleton;
            motionService = MotionService.Singleton;
            sysService = SystemService.Singleton;
            seqService = SequenceService.Singleton;
            topService = VisionTopService.Singleton;

            axisVision = (int)EnumSmartIC.Axis.visionFeed;
            axisPunch = (int)EnumSmartIC.Axis.punchFeed;

            tbxInitOffset.Text = dataService.JobInitOffset.ToString("0.000");
            tbxStartUnit.Text = dataService.JobInitUnit.ToString();

            cbRMove.SelectedItem = "1.0";

            topService.EventInitPos += OnEventInitPos;

            Vel = 2;
            tblJogVel.Text = string.Format("속도 : {0:0.0}", dataService.DataMotion[axisVision].VelNormal * 2);

            tblOK.Foreground = Brushes.Red;

            cbRMove.SelectedIndex = 2;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            if( null != topService )
                topService.EventInitPos -= OnEventInitPos;
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;

            this.DialogResult = false;

            if (null != sysService)
                sysService.State = SystemService.States.stop;

            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;

            dataService.JobInitUnit = int.Parse(tbxStartUnit.Text);

            this.DialogResult = true;

            this.Close();
        }

        private void btnJogN_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;

            if (null != cbx)
            {
                if (null == motionService)
                {
                    cbx.IsChecked = false;
                    return;
                }
                if (null == dataService)
                {
                    cbx.IsChecked = false;
                    return;
                }

                if (true == cbx.IsChecked)
                {
                    double vel = dataService.DataMotion[axisVision].VelNormal;
                    switch (Vel)
                    {
                        case 1:
                            vel = dataService.DataMotion[axisVision].VelNormal;
                            break;
                        case 2:
                            vel = dataService.DataMotion[axisVision].VelNormal*2.0;
                            break;
                        case 3:
                            vel = dataService.DataMotion[axisVision].VelNormal * 3.0;
                            break;
                        default:
                            vel = dataService.DataMotion[axisVision].VelNormal*0.5;
                            break;
                    }

                    motionService.JogN(axisVision, vel, dataService.DataMotion[axisVision].AccelNormal);
                    motionService.JogN(axisPunch, vel, dataService.DataMotion[axisVision].AccelNormal);

                    //btnJogN.IsEnabled = false;
                    btnJogP.IsEnabled = false;
                    btnRMoveN.IsEnabled = false;
                    btnRMoveP.IsEnabled = false;
                    btnInitPosSearch.IsEnabled = false;
                    btnInitOffset.IsEnabled = false;
                    btnOK.IsEnabled = false;
                }
                else
                {
                    motionService.Stop(axisVision);
                    motionService.Stop(axisPunch);

                    //btnJogN.IsEnabled = true;
                    btnJogP.IsEnabled = true;
                    btnRMoveN.IsEnabled = true;
                    btnRMoveP.IsEnabled = true;
                    btnInitPosSearch.IsEnabled = true;
                    btnInitOffset.IsEnabled = true;
                    btnOK.IsEnabled = true;
                }
            }
        }

        private void btnJogP_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;

            if (null != cbx)
            {
                if (null == motionService)
                {
                    cbx.IsChecked = false;
                    return;
                }
                if (null == dataService)
                {
                    cbx.IsChecked = false;
                    return;
                }


                if (true == cbx.IsChecked)
                {
                    double vel = dataService.DataMotion[axisVision].VelNormal;
                    switch (Vel)
                    {
                        case 1:
                            vel = dataService.DataMotion[axisVision].VelNormal;
                            break;
                        case 2:
                            vel = dataService.DataMotion[axisVision].VelNormal * 2.0;
                            break;
                        case 3:
                            vel = dataService.DataMotion[axisVision].VelNormal * 3.0;
                            break;
                        default:
                            vel = dataService.DataMotion[axisVision].VelNormal * 0.5;
                            break;
                    }

                    motionService.JogP(axisVision, vel, dataService.DataMotion[axisVision].AccelNormal);
                    motionService.JogP(axisPunch, vel, dataService.DataMotion[axisVision].AccelNormal);

                    btnJogN.IsEnabled = false;
                    //btnJogP.IsEnabled = false;
                    btnRMoveN.IsEnabled = false;
                    btnRMoveP.IsEnabled = false;
                    btnInitPosSearch.IsEnabled = false;
                    btnInitOffset.IsEnabled = false;
                    btnOK.IsEnabled = false;
                }
                else
                {
                    motionService.Stop(axisVision);
                    motionService.Stop(axisPunch);

                    btnJogN.IsEnabled = true;
                    //btnJogP.IsEnabled = true;
                    btnRMoveN.IsEnabled = true;
                    btnRMoveP.IsEnabled = true;
                    btnInitPosSearch.IsEnabled = true;
                    btnInitOffset.IsEnabled = true;
                    btnOK.IsEnabled = true;
                }
            }
        }

        private void btnRMoveN_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;

            if (null != cbx)
            {
                if (null == motionService)
                {
                    cbx.IsChecked = false;
                    return;
                }
                if (null == dataService)
                {
                    cbx.IsChecked = false;
                    return;
                }

                double pos = double.Parse(cbRMove.SelectedItem.ToString());

                if (true == cbx.IsChecked)
                {
                    double vel = dataService.DataMotion[axisVision].VelNormal;
                    switch (Vel)
                    {
                        case 1:
                            vel = dataService.DataMotion[axisVision].VelNormal;
                            break;
                        case 2:
                            vel = dataService.DataMotion[axisVision].VelNormal * 2.0;
                            break;
                        case 3:
                            vel = dataService.DataMotion[axisVision].VelNormal * 3.0;
                            break;
                        default:
                            vel = dataService.DataMotion[axisVision].VelNormal * 0.5;
                            break;
                    }

                    motionService.RMove(axisVision, -pos, vel, dataService.DataMotion[axisVision].AccelNormal);
                    motionService.RMove(axisPunch, -pos, vel, dataService.DataMotion[axisVision].AccelNormal);

                    btnJogN.IsEnabled = false;
                    btnJogP.IsEnabled = false;
                    //btnRMoveN.IsEnabled = false;
                    btnRMoveP.IsEnabled = false;
                    btnInitPosSearch.IsEnabled = false;
                    btnInitOffset.IsEnabled = false;
                    btnOK.IsEnabled = false;
                }
                else
                {
                    motionService.Stop(axisVision);
                    motionService.Stop(axisPunch);
                }

                while ((false == motionService.IsMotionDone(axisVision)) && (false == motionService.IsMotionDone(axisPunch)))
                {
                    System.Windows.Forms.Application.DoEvents();
                }
                cbx.IsChecked = false;

                btnJogN.IsEnabled = true;
                btnJogP.IsEnabled = true;
                btnRMoveN.IsEnabled = true;
                btnRMoveP.IsEnabled = true;
                btnInitPosSearch.IsEnabled = true;
                btnInitOffset.IsEnabled = true;
                btnOK.IsEnabled = true;
            }
        }

        private void btnRMoveP_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;

            if (null != cbx)
            {
                if (null == motionService)
                {
                    cbx.IsChecked = false;
                    return;
                }
                if (null == dataService)
                {
                    cbx.IsChecked = false;
                    return;
                }

                double pos = double.Parse(cbRMove.SelectedItem.ToString());

                if (true == cbx.IsChecked)
                {
                    double vel = dataService.DataMotion[axisVision].VelNormal;
                    switch (Vel)
                    {
                        case 1:
                            vel = dataService.DataMotion[axisVision].VelNormal;
                            break;
                        case 2:
                            vel = dataService.DataMotion[axisVision].VelNormal * 2.0;
                            break;
                        case 3:
                            vel = dataService.DataMotion[axisVision].VelNormal * 3.0;
                            break;
                        default:
                            vel = dataService.DataMotion[axisVision].VelNormal * 0.5;
                            break;
                    }

                    motionService.RMove(axisVision, pos, vel, dataService.DataMotion[axisVision].AccelNormal);
                    motionService.RMove(axisPunch, pos, vel, dataService.DataMotion[axisVision].AccelNormal);

                    btnJogN.IsEnabled = false;
                    btnJogP.IsEnabled = false;
                    btnRMoveN.IsEnabled = false;
                    //btnRMoveP.IsEnabled = false;
                    btnInitPosSearch.IsEnabled = false;
                    btnInitOffset.IsEnabled = false;
                    btnOK.IsEnabled = false;
                }
                else
                {
                    motionService.Stop(axisVision);
                    motionService.Stop(axisPunch);
                }

                while ((false == motionService.IsMotionDone(axisVision)) && (false == motionService.IsMotionDone(axisPunch)))
                {
                    System.Windows.Forms.Application.DoEvents();
                }
                cbx.IsChecked = false;

                btnJogN.IsEnabled = true;
                btnJogP.IsEnabled = true;
                btnRMoveN.IsEnabled = true;
                btnRMoveP.IsEnabled = true;
                btnInitPosSearch.IsEnabled = true;
                btnInitOffset.IsEnabled = true;
                btnOK.IsEnabled = true;
            }
        }

        private void btnInitCancel_Click(object sender, RoutedEventArgs e)
        {
            sysService.State = SystemService.States.stop;
        }

        private void btnJogStop_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;

            if (null != cbx)
            {
                if (null == motionService)
                {
                    cbx.IsChecked = false;
                    return;
                }
                if (null == dataService)
                {
                    cbx.IsChecked = false;
                    return;
                }


                if (true == cbx.IsChecked)
                {
                    motionService.Stop(axisVision);
                    motionService.Stop(axisPunch);

                    btnJogN.IsEnabled = false;
                    btnJogP.IsEnabled = false;
                    btnRMoveN.IsEnabled = false;
                    btnRMoveP.IsEnabled = false;
                    btnInitPosSearch.IsEnabled = false;
                    btnOK.IsEnabled = false;
                }
                else
                {
                    btnJogN.IsEnabled = true;
                    btnJogP.IsEnabled = true;
                    btnRMoveN.IsEnabled = true;
                    btnRMoveP.IsEnabled = true;
                    btnInitPosSearch.IsEnabled = true;
                    btnInitOffset.IsEnabled = true;
                    btnOK.IsEnabled = true;
                }

                btnJogN.IsChecked = false;
                btnJogP.IsChecked = false;
                btnRMoveN.IsChecked = false;
                btnRMoveP.IsChecked = false;
                btnInitPosSearch.IsChecked = false;
            }
        }

        private void btnInitPosSearch_Click(object sender, RoutedEventArgs e)
        {
            btnJogN.IsEnabled = false;
            btnJogP.IsEnabled = false;
            btnRMoveN.IsEnabled = false;
            btnRMoveP.IsEnabled = false;
            //btnInitPosSearch.IsEnabled = false;
            btnInitOffset.IsEnabled = false;
            btnOK.IsEnabled = false;

            dataService.DataResult.TimeStart = DateTime.Now;

            tblOK.Foreground = Brushes.Red;

            int ret = seqService.SetSequence((int)EnumSmartIC.Sequences.initSearch, 1);

            btnJogN.IsEnabled = true;
            btnJogP.IsEnabled = true;
            btnRMoveN.IsEnabled = true;
            btnRMoveP.IsEnabled = true;
            //btnInitPosSearch.IsEnabled = true;
            btnInitOffset.IsEnabled = true;
            if( 0 == ret )
                tblOK.Foreground = Brushes.White;
            btnOK.IsEnabled = true;

            btnInitPosSearch.IsChecked = false;



            tbxInitOffset.Text = dataService.JobInitOffset.ToString("0.000");
        }

        private void btnInitOffset_Click(object sender, RoutedEventArgs e)
        {
            double pos = double.Parse(tbxInitOffset.Text);

            btnJogN.IsEnabled = false;
            btnJogP.IsEnabled = false;
            btnRMoveN.IsEnabled = false;
            btnRMoveP.IsEnabled = false;
            btnInitPosSearch.IsEnabled = false;
            btnOK.IsEnabled = false;

            //dataService.JobInitOffset = double.Parse(tbxInitOffset.Text);
            if (0 == seqService.SetSequence((int)EnumSmartIC.Sequences.initRMove, 1, pos))
            {
                btnJogN.IsEnabled = true;
                btnJogP.IsEnabled = true;
                btnRMoveN.IsEnabled = true;
                btnRMoveP.IsEnabled = true;
                btnInitPosSearch.IsEnabled = true;
                btnOK.IsEnabled = true;
            }
        }

        private void OnEventInitPos(object sender, EventArgs e)
        {
            if (null == OwnerWindow)
                return;

            if (InvokeRequired)
            {
                OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                            (ThreadStart)delegate()
                                            {
                                                tbxInitOffset.Text = dataService.JobInitOffset.ToString("0.000");
                                            }
                );
            }
            else
            {
                tbxInitOffset.Text = dataService.JobInitOffset.ToString("0.000");
            }
        }

        private void btnJogVel_Click(object sender, RoutedEventArgs e)
        {
            if (++Vel > 3)
                Vel = 0;

            switch (Vel)
            {
                case 1:
                    tblJogVel.Text = string.Format("속도 : {0:0.0}", dataService.DataMotion[axisVision].VelNormal);
                    break;
                case 2:
                    tblJogVel.Text = string.Format("속도 : {0:0.0}", dataService.DataMotion[axisVision].VelNormal*2);
                    break;
                case 3:
                    tblJogVel.Text = string.Format("속도 : {0:0.0}", dataService.DataMotion[axisVision].VelNormal * 3);
                    break;
                default:
                    tblJogVel.Text = string.Format("속도 : {0:0.0}", dataService.DataMotion[axisVision].VelNormal*0.5);
                    break;
            }
        }
    }
}
