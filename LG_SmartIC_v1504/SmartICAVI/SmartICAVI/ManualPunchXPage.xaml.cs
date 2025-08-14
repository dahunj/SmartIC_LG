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
    /// ManualPunchXYPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ManualPunchXPage : Page
    {
        private SystemService sysService = null;
        private MotionService motionService = null;
        private DataService dataService = null;
        private MsgService msgService = null;

        private DispatcherTimer timer = null;

        private string oldValue = "";

        private GroupControl gpJog = null;

        private int Axis { get; set; }
        private bool IsUnload { get; set; }
        
        public ManualPunchXPage()
        {
            InitializeComponent();
            Axis = (int)EnumSmartIC.Axis.punchX;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            IsUnload = false;

            motionService = MotionService.Singleton;
            dataService = DataService.Singleton;
            msgService = MsgService.Singleton;
            sysService = SystemService.Singleton;

            btnJogN.Axis = Axis;
            btnJogP.Axis = Axis;
            btnRMoveN.Axis = Axis;
            btnRMoveP.Axis = Axis;
            btnAMove.Axis = Axis;

            gpJog = new GroupControl();
            gpJog.Add(cbxJog);
            gpJog.Add(cbxRMove);
            gpJog.Add(cbxAMove);

            cbxJog.IsChecked = false;
            cbxRMove.IsChecked = false;
            cbxAMove.IsChecked = false;

            cbxJog.Add((ISelectControl)btnJogN);
            cbxJog.Add((ISelectControl)btnJogP);

            cbxRMove.Add((ISelectControl)btnRMoveN);
            cbxRMove.Add((ISelectControl)btnRMoveP);

            cbxAMove.Add((ISelectControl)btnAMove);

            //SetTimer();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            IsUnload = true;

            if (null != gpJog)
            {
                gpJog.RemoveAll();
            }

            //ResetTimer();

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

            if (null != cbx)
            {
                switch (cbx.Tag.ToString())
                {
                    case "btnAMove":
                        pos = double.Parse(tbxAMove.Text);
                        motionService.AMove(Axis, pos, dataService.DataMotion[Axis].VelNormal, dataService.DataMotion[Axis].AccelNormal);

                        btnJogN.IsEnabled = false;
                        btnJogP.IsEnabled = false;
                        //btnAMove.IsEnabled = false;
                        btnRMoveN.IsEnabled = false;
                        btnRMoveP.IsEnabled = false;
                        break;
                    case "btnRMoveN":
                        pos = double.Parse(tbxRMove.Text);
                        motionService.RMove(Axis, -pos, dataService.DataMotion[Axis].VelNormal, dataService.DataMotion[Axis].AccelNormal);

                        btnJogN.IsEnabled = false;
                        btnJogP.IsEnabled = false;
                        btnAMove.IsEnabled = false;
                        //btnRMoveN.IsEnabled = false;
                        btnRMoveP.IsEnabled = false;
                        break;
                    case "btnRMoveP":
                        pos = double.Parse(tbxRMove.Text);
                        motionService.RMove(Axis, pos, dataService.DataMotion[Axis].VelNormal, dataService.DataMotion[Axis].AccelNormal);

                        btnJogN.IsEnabled = false;
                        btnJogP.IsEnabled = false;
                        btnAMove.IsEnabled = false;
                        btnRMoveN.IsEnabled = false;
                        //btnRMoveP.IsEnabled = false;
                        break;
                    default:
                        return;
                }

                while (false == motionService.IsMotionDone(Axis))
                {
                    if (true == IsUnload)
                        break;

                    System.Windows.Forms.Application.DoEvents();
                }

                cbx.IsChecked = false;

                btnJogN.IsEnabled = true;
                btnJogP.IsEnabled = true;
                btnAMove.IsEnabled = true;
                btnRMoveN.IsEnabled = true;
                btnRMoveP.IsEnabled = true;
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

            switch (btn.Tag.ToString())
            {
                case "btnJogN":
                    motionService.JogN(Axis, dataService.DataMotion[Axis].VelNormal, dataService.DataMotion[Axis].AccelNormal);

                    btnJogP.IsEnabled = false;
                    btnAMove.IsEnabled = false;
                    btnRMoveN.IsEnabled = false;
                    btnRMoveP.IsEnabled = false;
                    break;
                case "btnJogP":
                    motionService.JogP(Axis, dataService.DataMotion[Axis].VelNormal, dataService.DataMotion[Axis].AccelNormal);

                    btnJogN.IsEnabled = false;
                    btnAMove.IsEnabled = false;
                    btnRMoveN.IsEnabled = false;
                    btnRMoveP.IsEnabled = false;
                    break;
                default:
                    break;
            }
        }

        private void btnJog_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (null == motionService)
                return;

            motionService.Stop(Axis);

            btnJogN.IsEnabled = true;
            btnJogP.IsEnabled = true;
            btnAMove.IsEnabled = true;
            btnRMoveN.IsEnabled = true;
            btnRMoveP.IsEnabled = true;
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            DisplayState();
        }

        private void DisplayState()
        {
            if (null != motionService)
            {
                //ledLimitN.IsEnabled = (1 == motionService.GetStateLimitN(Axis)) ? true : false;
                //ledLimitP.IsEnabled = (1 == motionService.GetStateLimitP(Axis)) ? true : false;
                //lbPosition.Content = motionService.GetCurrentPosition(Axis).ToString("0.000");
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
