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
    /// MachineControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MachineControl : UserControl
    {
        private DioService dioService = null;
        private MotionService motionService = null;
        private SequenceService seqService = null;
        private DataService dataService = null;

        private double ratioX = 1.0;
        private double ratioY = 1.0;

        private BitmapImage bmpMachine = null;

        private DispatcherTimer timer = null;

        public bool IsShowMachine { get; set; }

        static bool InvokeRequired
        {
            get { return Dispatcher.CurrentDispatcher != Application.Current.Dispatcher; }
        }

        public MachineControl()
        {
            IsShowMachine = false;
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dioService = DioService.Singleton;
            motionService = MotionService.Singleton;
            seqService = SequenceService.Singleton;
            dataService = DataService.Singleton;

            if (true == IsShowMachine)
            {
                string imgPath = DataService.Singleton.DataPath + "\\Images\\Common\\Machine_Front.jpg";

                Uri uriMachine = new Uri(imgPath);

                // Image Load
                try
                {
                    bmpMachine = new BitmapImage(uriMachine);
                }
                catch (Exception exc)
                {
                    uriMachine = new Uri("pack://application:,,/Images/Stop48.png");
                    bmpMachine = new BitmapImage(uriMachine);

                    Log_Exception.WriteLine("MachineControl.UserControl_Loaded() : " + exc.Message);
                }

                if (null != bmpMachine)
                {
                    image.Source = bmpMachine;

                    InitImages();
                }

                DisplayState();

                SetTimer();
            }

            
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (true == IsShowMachine)
            {
                ResetTimer();
            }
        }

        private void InitImages()
        {
            double mX = bmpMachine.Width;
            double mY = bmpMachine.Height;

            double canvX = canvas.ActualWidth;
            double canvY = canvas.ActualHeight;

            ratioX = canvX / mX;
            ratioY = canvY / mY;

            // Axis0 Reel Uncoiler
            lblAxis0.Width = 60.0 * ratioX;
            lblAxis0.Height = 60.0 * ratioY;
            Canvas.SetLeft(lblAxis0, 120.0 * ratioX);
            Canvas.SetTop(lblAxis0, 282.0 * ratioY);

            // Axis1 Vision - Recoiler Sprocket
            lblAxis1.Width = 60.0 * ratioX;
            lblAxis1.Height = 60.0 * ratioY;
            Canvas.SetLeft(lblAxis1, 540.0 * ratioX);
            Canvas.SetTop(lblAxis1, 410.0 * ratioY);


            // Axis2 Vision - Top Z
            lblAxis2.Width = 60.0 * ratioX;
            lblAxis2.Height = 80.0 * ratioY;
            Canvas.SetLeft(lblAxis2, 420.0 * ratioX);
            Canvas.SetTop(lblAxis2, 265.0 * ratioY);


            // Axis3 Vision - Bottom Z
            lblAxis3.Width = 60.0 * ratioX;
            lblAxis3.Height = 80.0 * ratioY;
            Canvas.SetLeft(lblAxis3, 450.0 * ratioX);
            Canvas.SetTop(lblAxis3, 490.0 * ratioY);


            // Axis4 Punch - Recoiler Sprocket
            lblAxis4.Width = 60.0 * ratioX;
            lblAxis4.Height = 60.0 * ratioY;
            Canvas.SetLeft(lblAxis4, 880.0 * ratioX);
            Canvas.SetTop(lblAxis4, 410.0 * ratioY);


            // Axis5 Align & Punch X
            lblAxis5.Width = 60.0 * ratioX;
            lblAxis5.Height = 60.0 * ratioY;
            Canvas.SetLeft(lblAxis5, 815.0 * ratioX);
            Canvas.SetTop(lblAxis5, 410.0 * ratioY);

            // Axis6 Align & Punch Y
            lblAxis6.Width = 60.0 * ratioX;
            lblAxis6.Height = 60.0 * ratioY;
            Canvas.SetLeft(lblAxis6, 750.0 * ratioX);
            Canvas.SetTop(lblAxis6, 410.0 * ratioY);


            // Axis7 Reel Uncoiler
            lblAxis7.Width = 60.0 * ratioX;
            lblAxis7.Height = 60.0 * ratioY;
            Canvas.SetLeft(lblAxis7, 1045.0 * ratioX);
            Canvas.SetTop(lblAxis7, 282.0 * ratioY);


            // EMO 1
            lblEMO1.Width = 60.0 * ratioX;
            lblEMO1.Height = 60.0 * ratioY;
            Canvas.SetLeft(lblEMO1, 310.0 * ratioX);
            Canvas.SetTop(lblEMO1, 290.0 * ratioY);

            // EMO 2
            lblEMO2.Width = 60.0 * ratioX;
            lblEMO2.Height = 60.0 * ratioY;
            Canvas.SetLeft(lblEMO2, 935.0 * ratioX);
            Canvas.SetTop(lblEMO2, 290.0 * ratioY);
                        
            // Main Air
            lblMainAir.Width = 90.0 * ratioX;
            lblMainAir.Height = 60.0 * ratioY;
            Canvas.SetLeft(lblMainAir, 1100.0 * ratioX);
            Canvas.SetTop(lblMainAir, 570.0 * ratioY);

            // Light Curtain
            lblLightCurtain.Width = 30.0 * ratioX;
            lblLightCurtain.Height = 130.0 * ratioY;
            Canvas.SetLeft(lblLightCurtain, 710.0 * ratioX);
            Canvas.SetTop(lblLightCurtain, 340.0 * ratioY);


            // Uncoiler - Limit 
            lblUncoilerLimitN.Width = 60.0 * ratioX;
            lblUncoilerLimitN.Height = 60.0 * ratioY;
            Canvas.SetLeft(lblUncoilerLimitN, 320.0 * ratioX);
            Canvas.SetTop(lblUncoilerLimitN, 570.0 * ratioY);

            // Uncoiler + Limit 
            lblUncoilerLimitP.Width = 60.0 * ratioX;
            lblUncoilerLimitP.Height = 60.0 * ratioY;
            Canvas.SetLeft(lblUncoilerLimitP, 320.0 * ratioX);
            Canvas.SetTop(lblUncoilerLimitP, 440.0 * ratioY);

            // Buffer - Limit 
            lblBufferLimitN.Width = 60.0 * ratioX;
            lblBufferLimitN.Height = 60.0 * ratioY;
            Canvas.SetLeft(lblBufferLimitN, 620.0 * ratioX);
            Canvas.SetTop(lblBufferLimitN, 570.0 * ratioY);

            // Buffer + Limit 
            lblBufferLimitP.Width = 60.0 * ratioX;
            lblBufferLimitP.Height = 60.0 * ratioY;
            Canvas.SetLeft(lblBufferLimitP, 620.0 * ratioX);
            Canvas.SetTop(lblBufferLimitP, 440.0 * ratioY);

            // Recoiler - Limit 
            lblRecoilerLimitN.Width = 60.0 * ratioX;
            lblRecoilerLimitN.Height = 60.0 * ratioY;
            Canvas.SetLeft(lblRecoilerLimitN, 920.0 * ratioX);
            Canvas.SetTop(lblRecoilerLimitN, 570.0 * ratioY);

            // Recoiler + Limit 
            lblRecoilerLimitP.Width = 60.0 * ratioX;
            lblRecoilerLimitP.Height = 60.0 * ratioY;
            Canvas.SetLeft(lblRecoilerLimitP, 920.0 * ratioX);
            Canvas.SetTop(lblRecoilerLimitP, 440.0 * ratioY);
        }

        private void DisplayState()
        {
            if (null != dioService)
            {
                UInt32 inports0 = dioService.Inports0;

                UInt32 emo1 = (inports0 >> (int)(EnumSmartIC.Inports.emo1)) & 0x0001;
                UInt32 emo2 = (inports0 >> (int)(EnumSmartIC.Inports.emo2)) & 0x0001;
                UInt32 mainAir = (inports0 >> (int)(EnumSmartIC.Inports.mainAir)) & 0x0001;

                lblEMO1.Visibility = (1 == emo1) ? Visibility.Visible : Visibility.Hidden;
                lblEMO2.Visibility = (1 == emo2) ? Visibility.Visible : Visibility.Hidden;
                lblMainAir.Visibility = (0 == mainAir) ? Visibility.Visible : Visibility.Hidden;
                lblLightCurtain.Visibility = Visibility.Hidden;
            }

            if (null != motionService)
            {
                lblUncoilerLimitN.Visibility = (1 == motionService.GetStateLimitN((int)EnumSmartIC.Axis.uncoilerReel)) ? Visibility.Visible : Visibility.Hidden;
                lblUncoilerLimitP.Visibility = (1 == motionService.GetStateLimitP((int)EnumSmartIC.Axis.uncoilerReel)) ? Visibility.Visible : Visibility.Hidden;

                lblBufferLimitN.Visibility = (1 == motionService.GetStateLimitN((int)EnumSmartIC.Axis.buffer)) ? Visibility.Visible : Visibility.Hidden;
                lblBufferLimitP.Visibility = (1 == motionService.GetStateLimitP((int)EnumSmartIC.Axis.buffer)) ? Visibility.Visible : Visibility.Hidden;

                lblRecoilerLimitN.Visibility = (1 == motionService.GetStateLimitN((int)EnumSmartIC.Axis.recoilerReel)) ? Visibility.Visible : Visibility.Hidden;
                lblRecoilerLimitP.Visibility = (1 == motionService.GetStateLimitP((int)EnumSmartIC.Axis.recoilerReel)) ? Visibility.Visible : Visibility.Hidden;

                lblAxis0.Visibility = (false == motionService.IsMotionDone(0)) ? Visibility.Visible : Visibility.Hidden;
                lblAxis1.Visibility = (false == motionService.IsMotionDone(1)) ? Visibility.Visible : Visibility.Hidden;
                lblAxis2.Visibility = (false == motionService.IsMotionDone(1)) ? Visibility.Visible : Visibility.Hidden;
                lblAxis3.Visibility = (false == motionService.IsMotionDone(1)) ? Visibility.Visible : Visibility.Hidden;
                lblAxis4.Visibility = (false == motionService.IsMotionDone(1)) ? Visibility.Visible : Visibility.Hidden;
                lblAxis5.Visibility = (false == motionService.IsMotionDone(1)) ? Visibility.Visible : Visibility.Hidden;
                lblAxis6.Visibility = (false == motionService.IsMotionDone(1)) ? Visibility.Visible : Visibility.Hidden;
                lblAxis7.Visibility = (false == motionService.IsMotionDone(1)) ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private void SetTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = new System.TimeSpan(0, 0, 0, 0, 100);
            timer.IsEnabled = true;
            timer.Tick += new EventHandler(timer_Tick);
        }

        private void ResetTimer()
        {
            timer.IsEnabled = false;
            timer.Tick -= timer_Tick;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            DisplayState();
        }

    }
}
