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
    /// ManualUncoilerPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ManualUncoilerPage : Page
    {
        #region Variables
        private DataService dataService = null;
        private SequenceService seqService = null;
        private MotionService motionService = null;
        private AioService aioService = null;
        private DioService dioService = null;

        private DispatcherTimer timer = null;

        //private int axis = 0;

        private GroupControl gpJog = null;

        private ManualTunningWindow tunningWindow = null;
        #endregion

        public ManualUncoilerPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dataService = DataService.Singleton;
            seqService = SequenceService.Singleton;
            motionService = MotionService.Singleton;
            aioService = AioService.Singleton;
            dioService = DioService.Singleton;

            //axis = (int)EnumSmartIC.Axis.uncoilerReel;

            gpJog = new GroupControl();
            gpJog.Add(cbxJog);
            gpJog.Add(cbxRollMove);
            gpJog.Add(cbxRollAutoMove);

            cbxJog.IsChecked = false;
            cbxRollMove.IsChecked = false;
            cbxRollAutoMove.IsChecked = false;

            cbxJog.Add((ISelectControl)btnJogN);
            cbxJog.Add((ISelectControl)btnJogP);

            cbxRollMove.Add((ISelectControl)btnRollMove);

            cbxRollAutoMove.Add((ISelectControl)btnRollAutoMove);

            //DisplayState();
            //SetTimer();

            btnRollAutoMove.IsChecked = seqService.IsFlag((int)EnumSmartIC.SeqFlags.uncoilerRun);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            //ResetTimer();

            if (null != gpJog)
            {
                gpJog.RemoveAll();
            }

            if (null != tunningWindow)
            {
                if( true == tunningWindow.IsLoaded )
                    tunningWindow.Close();
            }

            GC.Collect();
        }

        private void tbx_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //VirtualKeyboardService.GetSingleton().FireVirtualKeyboard(sender);
        }

        private void tbxNumber_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //VirtualKeyboardService.GetSingleton().FireVirtualKeyNumber(sender);
        }

        private void btnMove_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;

            if (null != cbx)
            {
                switch (cbx.Tag.ToString())
                {
                    case "btnRollAutoMove":
                        if (true == cbx.IsChecked)
                            seqService.SetSequence((int)EnumSmartIC.Sequences.uncoilerBuffer, 1);
                        else
                            seqService.SetSequence((int)EnumSmartIC.Sequences.uncoilerBuffer, 0);
                        break;
                    default:
                        break;
                }
            }
        }

        private void tbx_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //TextBox textBox = sender as TextBox;

            //double value;

            //if (false == double.TryParse(textBox.Text, out value))
            //{
            //    // Error 처리    
            //    msgService.ShowMessage(MessageService.Messages.inputerror, false);
            //    textBox.Text = oldValue;
            //}
            //else
            //{
            //    oldValue = textBox.Text;
            //}
        }

        private void btnJog_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (null == aioService)
                return;

            Button btn = sender as Button;

            double jog = dataService.DataMotion[0].Vjog;
            double offset = dataService.DataMotion[0].Voffset;

            double dir = 1.0;

            if( true == dioService.IsInportOn((int)EnumSmartIC.Inports.uncoilerReelDir ))
            {
                dir = -1.0;
            }

            //motionService.SetServoOn((int)EnumSmartIC.Axis.uncoilerReel, true);
            //DoEvents(100);

            switch (btn.Tag.ToString())
            {
                case "btnJogN":
                    aioService.SetOutvalue(0, offset + jog * dir);
                    break;
                case "btnJogP":
                    aioService.SetOutvalue(0, offset - jog * dir);
                    break;
                default:
                    break;
            }
        }

        private void btnJog_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (null == aioService)
                return;

            double offset = dataService.DataMotion[0].Voffset;

            aioService.SetOutvalue(0, offset);

            //DoEvents(100);
            //motionService.SetServoOn((int)EnumSmartIC.Axis.uncoilerReel, false);
            
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            DisplayState();
        }

        private void DisplayState()
        {
            //if (null != motionService)
            //{
            //    ledLimitN.IsEnabled = (1 == motionService.GetStateLimitN(axis)) ? true : false;
            //    ledLimitP.IsEnabled = (1 == motionService.GetStateLimitP(axis)) ? true : false;
            //    lbPosition.Content = motionService.GetCurrentPosition(axis).ToString("0.000");
            //}
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

        private void btnTunning_Click(object sender, RoutedEventArgs e)
        {
            if (null == tunningWindow)
            {
                tunningWindow = new ManualTunningWindow();
                tunningWindow.Show();
            }
            else
            {
                if (false == tunningWindow.IsLoaded)
                {
                    tunningWindow = new ManualTunningWindow();
                    tunningWindow.Show();
                }
            }
        }

        private void DoEvents(int mSec)
        {
            DateTime start = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, mSec);
            DateTime timeout = start.Add(duration);

            do
            {
                System.Windows.Forms.Application.DoEvents();
                Thread.Sleep(1);
            } while (timeout > DateTime.Now);
        }
    }
}
