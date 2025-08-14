using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using HalconDotNet;
using Microsoft.Win32;

namespace SmartICAVI
{
    /// <summary>
    /// TeachPunchPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TeachPunchPage : Page
    {
        private DispatcherTimer timer = null;

        private GrabService grabService = null;
        private StrobeService strobeService = null;
        private MotionService motionService = null;
        private DataService dataService = null;
        private MsgService msgService = null;
        private SystemService sysService = null;
        private InspectService inspectService = null;
        private SequenceService seqService = null;
        private DioService dioService = null;

        private VisionCamService camService = null;

        private HWindowWPF hWPF = null;
        private HWindow hWindow = null;
        private HObject hImage = null;

        private double winRatio = 1.0;

        private int axisX = 0;
        private int axisY = 0;
        private int imgWidth = 0;
        private int imgHeight = 0;

        private HTuple inspectRow1, inspectCol1, button;
        private HTuple inspectRow2, inspectCol2;

        private HTuple grayMax, grayMin;

        private HTuple searchRow1, searchCol1, searchRow2, searchCol2;

        private string oldValue = "";

        private bool IsUnloaded { get; set; }

        private bool IsInitHWindow { get; set; }

        private bool IsGrab { get; set; }


        private enum enTools { none = 0, select, delete, inspectROI, searchROI, dontcare }
        private enTools tool;

        static bool InvokeRequired
        {
            get { return Dispatcher.CurrentDispatcher != Application.Current.Dispatcher; }
        }
       
        public TeachPunchPage()
        {
            InitializeComponent();

            tool = enTools.none;

            cbIPHole.Items.Add("0");
            cbIPHole.Items.Add("1");
            cbIPHole.Items.Add("2");
            cbIPHole.Items.Add("3");
            cbIPHole.Items.Add("4");
            cbIPHole.Items.Add("5");
            cbIPHole.Items.Add("6");
            cbIPHole.Items.Add("7");
            cbIPHole.Items.Add("8");
            cbIPHole.Items.Add("9");
            cbIPHole.Items.Add("10");
            //cbLine.Items.Add("A");
            //cbLine.Items.Add("B");

            cbIPHole.SelectedIndex = 0;
            //cbLine.SelectedIndex = 0;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            sysService = SystemService.Singleton;
            grabService = GrabService.Singleton;
            strobeService = StrobeService.Singleton;
            motionService = MotionService.Singleton;
            dataService = DataService.Singleton;
            msgService = MsgService.Singleton;
            inspectService = InspectService.Singleton;
            seqService = SequenceService.Singleton;
            dioService = DioService.Singleton;
            camService = VisionCamService.Singleton;

            IsInitHWindow = false;
            IsGrab = false;

            imgWidth = grabService.Width;
            imgHeight = grabService.Height;

            inspectRow1 = 0;
            inspectCol1 = 0;
            button = 0;
            inspectRow2 = 0;
            inspectCol2 = 0;

            searchRow1 = searchCol1 = searchRow2 = searchCol2 = 0;

            winRatio = (double)imgHeight / (double)imgWidth;

           
            canvas.Height = canvas.ActualWidth * winRatio;

            //hWPF = new HWindowWPF((int)canvas.ActualWidth, (int)canvas.ActualHeight);

            //canvas.Children.Add(hWPF);


            //hWindow = hWPF.HalconWindow;

            //HOperatorSet.GenEmptyObj(out hImage);

            //HOperatorSet.SetPart(hWindow, 0, 0, (HTuple)grabService.Height, (HTuple)grabService.Width);

            axisX = (int)EnumSmartIC.Axis.punchX;
            axisY = (int)EnumSmartIC.Axis.punchY;

            grayMax = dataService.DataTeach.grayMax;
            grayMin = dataService.DataTeach.grayMin;

            tbxLight.Text = dataService.DataTeach.strobe1.ToString();

            SetEventState(this, null);

            sysService.EventState += OnEventState;

            if ("EXTERN" == dataService.DataSystem.CamType)
            {
                grabView.Visibility = System.Windows.Visibility.Hidden;

                camService.SendMode("TEACH");

                IsGrab = true;
                IsInitHWindow = true;
            }

            if (3 == dataService.DataTeach.Line)
            {
                cbLine.Items.Add("A");
                cbLine.Items.Add("B");
                cbLine.Items.Add("C");
            }
            else
            {
                cbLine.Items.Add("A");
                cbLine.Items.Add("B");
            }

            cbLine.SelectedIndex = 0;

            Cancel();

            if (sysService.State == SystemService.States.pause && dataService.DataSystem.FirstIndexPause == true)
            {
                tbxAlignPosOffsetX.IsEnabled= true;
                tbxAlignPosOffsetY.IsEnabled= true;
                cbxAlignVisionPosOffset.IsEnabled = true;
            }
            else
            {
                tbxAlignPosOffsetX.IsEnabled = false;
                tbxAlignPosOffsetY.IsEnabled = false;
                cbxAlignVisionPosOffset.IsEnabled = false;
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (null != camService)
            {
                camService.SendMode("MANUAL");
            }

            motionService.Stop(axisX);
            motionService.Stop(axisY);

            IsUnloaded = true;

            ResetTimer();

            if (null != hWindow)
                hWindow.Dispose();

            if (null != hWPF)
                hWPF.Dispose();

            if (null != hImage)
                hImage.Dispose();

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


        private void cbxBufferReady_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;

            if (null != cbx)
            {
                if (false == cbx.IsChecked)
                {
                    cbx.IsChecked = true;
                    return;
                }

                SetMotionButtons(false);
                cbx.IsChecked = true;

                if (0 == seqService.SetSequence((int)EnumSmartIC.Sequences.bufferReference, 1, 0.01))
                {
                    SetMotionButtons(true);
                }
                cbx.IsChecked = false;
            }
        }

        private void cbxReelMoveToPunch_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;

            if (null != cbx)
            {
                if (false == cbx.IsChecked)
                {
                    cbx.IsChecked = true;
                    return;
                }

                SetMotionButtons(false);
                cbx.IsChecked = true;

                seqService.FireEventManualJog(1);
                if (0 == seqService.SetSequence((int)EnumSmartIC.Sequences.teachPunch, 1, 0.01))
                {
                    SetMotionButtons(true);
                }
                seqService.FireEventManualJog(0);
                cbx.IsChecked = false;
            }
        }

        private void cbxReviewMoveToPunch_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;

            if (null != cbx)
            {
                if (false == cbx.IsChecked)
                {
                    cbx.IsChecked = true;
                    return;
                }

                int axisAlignX = (int)EnumSmartIC.Axis.punchX;
                int axisAlignY = (int)EnumSmartIC.Axis.punchY;

                double velAlignX = dataService.DataMotion[axisAlignX].VelMove;
                double accelAlignX = dataService.DataMotion[axisAlignX].AccelMove;
                double velAlignY = dataService.DataMotion[axisAlignY].VelMove;
                double accelAlignY = dataService.DataMotion[axisAlignY].AccelMove;

                double posAlignX = dataService.DataSystem.AlignVisionX;
                double posAlignY = dataService.DataSystem.AlignVisionY;

                SetMotionButtons(false);

                motionService.AMove(axisAlignX, posAlignX, velAlignX, accelAlignX);
                motionService.AMove(axisAlignY, posAlignY, velAlignY, accelAlignY);

                while (true)
                {
                    if (true == motionService.IsMotionDone(axisAlignX))
                    {
                        if (true == motionService.IsMotionDone(axisAlignY))
                            break;
                    }

                    System.Windows.Forms.Application.DoEvents();
                }

                SetMotionButtons(true);
                cbx.IsChecked = false;
            }
        }

        private void cbxMoveToOrigin_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;

            if (null != cbx)
            {
                if (false == cbx.IsChecked)
                {
                    cbx.IsChecked = true;
                    return;
                }

                SetMotionButtons(false);
                cbx.IsChecked = true;

                seqService.FireEventManualJog(1);
                if (0 == seqService.SetSequence((int)EnumSmartIC.Sequences.teachToZero, 1))
                {
                    SetMotionButtons(true);
                }
                cbx.IsChecked = false;
                seqService.FireEventManualJog(0);
            }
        }

        private void btnImageOpen_Click(object sender, RoutedEventArgs e)
        {
            //grabService.Reset();
        }

        private void btnImageSave_Click(object sender, RoutedEventArgs e)
        {
            if (true == IsGrab)
            {
                SaveFileDialog saveFileDlg = new SaveFileDialog();
                saveFileDlg.Filter = "비트맵 파일(*.bmp)|*.bmp";
                saveFileDlg.Title = "이미지 파일 저장";
                saveFileDlg.DefaultExt = "bmp";
                //openFileDlg.InitialDirectory = dataService.RecipePath;

                if (true == saveFileDlg.ShowDialog())
                {
                    HOperatorSet.WriteImage(hImage, "bmp", 0, saveFileDlg.FileName);
                }
            }
        }

        private void btnImageZoomIn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnImageZoomIn_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnImageZoomOut_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnImageSelect_Click(object sender, RoutedEventArgs e)
        {
            tool = enTools.select;

            //btnImageSelect.IsChecked = false;
            btnImageInspectArea.IsChecked = false;
        }

        private void btnImageDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnImageInspectArea_Click(object sender, RoutedEventArgs e)
        {
            tool = enTools.searchROI;

            btnImageSelect.IsChecked = false;
            //btnImageInspectArea.IsChecked = false;

            HOperatorSet.GetMbutton(hWindow, out inspectRow1, out inspectCol1, out button);
            HOperatorSet.SetColor(hWindow, "blue");
            HOperatorSet.SetDraw(hWindow, "margin");

            inspectRow2 = inspectRow1;
            inspectCol2 = inspectCol1;

            while (tool == enTools.searchROI)
            {
                try
                {
                    HOperatorSet.GetMposition(hWindow, out inspectRow2, out inspectCol2, out button);


                    //System.Diagnostics.Debug.WriteLine(string.Format("{0}, {1}, {2}", row.I, column.I, button.I));
                    HOperatorSet.DispObj(hImage, hWindow);

                    HOperatorSet.DispRectangle1(hWindow, inspectRow1, inspectCol1, inspectRow2, inspectCol2);

                    System.Windows.Forms.Application.DoEvents();

                    if (0 == button.I)
                        break;
                }
                catch (Exception exc)
                {
                    Log_Exception.WriteLine("TeachPunchPage.btnImageInspectArea_Click() : " + exc.Message);
                    return;
                }
            }

            double cx = (double)(imgWidth / 2);
            double cy = (double)(imgHeight / 2);

            double sizeX = (dataService.DataTeach.SearchAreaSize * 0.5) / dataService.DataSystem.CalX;
            double sizeY = (dataService.DataTeach.SearchAreaSize * 0.5) / dataService.DataSystem.CalY;


            searchRow1 = cy - sizeY;
            searchCol1 = cx - sizeX;
            searchRow2 = cy + sizeY;
            searchCol2 = cx + sizeX;

            if (null != inspectService)
            {
                //// A 열 
                //if (true == cbxLineA.IsChecked)
                //{
                //    dataService.DataTeach.inspectRow1A = inspectRow1;
                //    dataService.DataTeach.inspectCol1A = inspectCol1;
                //    dataService.DataTeach.inspectRow2A = inspectRow2;
                //    dataService.DataTeach.inspectCol2A = inspectCol2;

                //    dataService.DataTeach.searchRow1A = searchRow1;
                //    dataService.DataTeach.searchCol1A = searchCol1;
                //    dataService.DataTeach.searchRow2A = searchRow2;
                //    dataService.DataTeach.searchCol2A = searchCol2;

                //    dataService.DataTeach.grayMinA = grayMin;
                //    dataService.DataTeach.grayMaxA = grayMax;

                //    dataService.DataTeach.grayMin = grayMin;
                //    dataService.DataTeach.grayMax = grayMax;

                //    inspectService.SetTeaching("A", hWindow, hImage);
                //}
                //else
                //{
                //    dataService.DataTeach.inspectRow1B = inspectRow1;
                //    dataService.DataTeach.inspectCol1B = inspectCol1;
                //    dataService.DataTeach.inspectRow2B = inspectRow2;
                //    dataService.DataTeach.inspectCol2B = inspectCol2;

                //    dataService.DataTeach.searchRow1B = searchRow1;
                //    dataService.DataTeach.searchCol1B = searchCol1;
                //    dataService.DataTeach.searchRow2B = searchRow2;
                //    dataService.DataTeach.searchCol2B = searchCol2;

                //    dataService.DataTeach.grayMinB = grayMin;
                //    dataService.DataTeach.grayMaxB = grayMax;

                //    dataService.DataTeach.grayMin = grayMin;
                //    dataService.DataTeach.grayMax = grayMax;

                //    inspectService.SetTeaching("B", hWindow, hImage);
                //}

                dataService.DataTeach.inspectRow1 = inspectRow1;
                dataService.DataTeach.inspectCol1 = inspectCol1;
                dataService.DataTeach.inspectRow2 = inspectRow2;
                dataService.DataTeach.inspectCol2 = inspectCol2;

                dataService.DataTeach.searchRow1 = searchRow1;
                dataService.DataTeach.searchCol1 = searchCol1;
                dataService.DataTeach.searchRow2 = searchRow2;
                dataService.DataTeach.searchCol2 = searchCol2;

                dataService.DataTeach.grayMin = grayMin;
                dataService.DataTeach.grayMax = grayMax;

                inspectService.SetTeaching(hWindow, hImage, inspectRow1, inspectCol1, inspectRow2, inspectCol2,
                                            searchRow1, searchCol1, searchRow2, searchCol2, grayMin, grayMax);
            }

        }

        private void btnImageTeach_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnImageDontCare_Click(object sender, RoutedEventArgs e)
        {

        }


        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            double offsetX = 0, offsetY = 0;

            if (true == IsGrab)
            {
                if (null != inspectService)
                {

                    if (null != hImage)
                        hImage.DispObj(hWindow);

                    inspectService.InspectTeach(hWindow, hImage, out offsetX, out offsetY);
                }
            }
        }

        private void btnLight_Click(object sender, RoutedEventArgs e)
        {
            if (null != strobeService)
            {
                Byte ch1 = Byte.Parse(tbxLight.Text);
                strobeService.Write(ch1);
            }
        }

        private void btnImageGrab_Click(object sender, RoutedEventArgs e)
        {
            SetHWindow();
            DisplayImage();

            btnImageSelect.IsChecked = false;
            btnImageInspectArea.IsChecked = false;
        }

        private void btnImageLive_Click(object sender, RoutedEventArgs e)
        {
            SetHWindow();

            if (true == btnImageLive.IsChecked)
            {
                SetTimer();
                btnImageGrab.IsEnabled = false;
                btnImageSelect.IsEnabled = false;
                btnImageDelete.IsEnabled = false;
                btnImageInspectArea.IsEnabled = false;
                btnImageTeach.IsEnabled = false;
                btnImageDontCare.IsEnabled = false;

                scbMin.IsEnabled = false;
                scbMax.IsEnabled = false;
                cbxAlignVision.IsEnabled = false;
                cbxPunchVision.IsEnabled = false;
            }
            else
            {
                ResetTimer();
                btnImageGrab.IsEnabled = true;
                btnImageSelect.IsEnabled = true;
                btnImageDelete.IsEnabled = true;
                btnImageInspectArea.IsEnabled = true;
                btnImageTeach.IsEnabled = true;
                btnImageDontCare.IsEnabled = true;

                scbMin.IsEnabled = true;
                scbMax.IsEnabled = true;
                cbxAlignVision.IsEnabled = true;

                if( SystemService.States.stop > sysService.State )
                    cbxPunchVision.IsEnabled = true;
            }

            btnImageSelect.IsChecked = false;
            btnImageInspectArea.IsChecked = false;

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
            if (null != timer)
            {
                timer.IsEnabled = false;
                timer.Tick -= timer_Tick;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            DisplayImage();
        }

        private void DisplayImage()
        {
            if ("EXTERN" == DataService.Singleton.DataSystem.CamType)
                return;

            if (null != grabService)
            {
                if (true == grabService.IsConnected)
                {
                    if (null != hWindow)
                        hImage.Dispose();

                    GC.Collect();

                    if (0 == grabService.Grab(out hImage))
                    {
                        double cx = (double)(imgWidth / 2);
                        double cy = (double)(imgHeight / 2);

                        double stepX = 0.1 / dataService.DataSystem.CalX;
                        double stepY = 0.1 / dataService.DataSystem.CalY;

                        //HTuple pointer, type;
                        //HTuple width, height;
                        //HOperatorSet.GetImagePointer1(hImage, out pointer, out type, out width, out height);
                        HOperatorSet.DispObj(hImage, hWindow);
                        HOperatorSet.SetColor(hWindow, "green");
                        HOperatorSet.DispLine(hWindow, 0, imgWidth / 2, imgHeight, imgWidth / 2);
                        HOperatorSet.DispLine(hWindow, imgHeight / 2, 0, imgHeight / 2, imgWidth);
                        //HOperatorSet.WriteImage(hImage, "bmp", 0, "D:\\Test.bmp");

                        // Draw 0.1mm
                        int ten = 0;
                        double line = 10.0;
                        // X
                        for (double x = cx + stepX; x < (double)imgWidth; x += stepX)
                        {
                            if (0 == ++ten % 10)
                                line = 20;
                            else if (0 == ten % 5)
                                line = 12;
                            else
                                line = 5;

                            HOperatorSet.DispLine(hWindow, cy - line, x, cy + line, x);
                        }
                        ten = 0;
                        for (double x = cx - stepX; x > 0; x -= stepX)
                        {
                            if (0 == ++ten % 10)
                                line = 20;
                            else if (0 == ten % 5)
                                line = 12;
                            else
                                line = 5;

                            HOperatorSet.DispLine(hWindow, cy - line, x, cy + line, x);
                        }

                        // Y
                        ten = 0;
                        for (double y = cy + stepY; y < (double)imgHeight; y += stepY)
                        {
                            if (0 == ++ten % 10)
                                line = 20;
                            else if (0 == ten % 5)
                                line = 12;
                            else
                                line = 5;

                            HOperatorSet.DispLine(hWindow, y, cx-line, y, cx+line);
                        }
                        ten = 0;
                        for (double y = cy - stepY; y > 0; y -= stepY)
                        {
                            if (0 == ++ten % 10)
                                line = 20;
                            else if (0 == ten % 5)
                                line = 12;
                            else
                                line = 5;

                            HOperatorSet.DispLine(hWindow, y, cx - line, y, cx + line);
                        }

                        IsGrab = true;
                    }
                }
            }
        }

        private void Cancel()
        {
            //tbxPunchPosX.Text = dataService.DataRecipe.PunchX.ToString("0.00");
            //tbxPunchPosY.Text = dataService.DataRecipe.PunchY.ToString("0.00");
            //tbxBufferPosZ.Text = dataService.DataRecipe.BufferZ.ToString("0.00");
            //tbxPFHole.Text = dataService.DataRecipe.PF.ToString();
            //tbxNGContinue.Text = dataService.DataRecipe.NGContinue.ToString();

            //tbSelectedModel.Text = dataService.DataSystem.RecipeName;

            scbMin.Value = dataService.DataTeach.grayMin.I;
            scbMax.Value = dataService.DataTeach.grayMax.I;

            tbxPunchX.Text = dataService.DataTeach.PunchX.ToString("0.000");
            tbxPunchY.Text = dataService.DataTeach.PunchY.ToString("0.000");

            tbxAlignPosOffsetX.Text = dataService.DataSystem.AlignPosOffsetX.ToString("0.000");
            tbxAlignPosOffsetY.Text = dataService.DataSystem.AlignPosOffsetY.ToString("0.000");

        }

        private void scbMin_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Byte value = (Byte)scbMin.Value;
            Color clr = Color.FromRgb(value, value, value);
            Brush brush = new SolidColorBrush(clr);

            scbMin.Background = brush;

            dataService.DataTeach.grayMin = value;

            if (true == IsGrab)
            {
                if (null != hWindow)
                {
                    hImage.DispObj(hWindow);
                    inspectService.SetThreshold(hWindow, hImage, dataService.DataTeach.inspectRow1, dataService.DataTeach.inspectCol1, dataService.DataTeach.inspectRow2, dataService.DataTeach.inspectCol2, dataService.DataTeach.grayMin, dataService.DataTeach.grayMax);
                }
            }
        }

        private void scbMax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Byte value = (Byte)scbMax.Value;
            Color clr = Color.FromRgb(value, value, value);
            Brush brush = new SolidColorBrush(clr);

            scbMax.Background = brush;

            dataService.DataTeach.grayMax = value;

            if (true == IsGrab)
            {
                if (null != hWindow)
                {
                    hImage.DispObj(hWindow);
                    inspectService.SetThreshold(hWindow, hImage, dataService.DataTeach.inspectRow1, dataService.DataTeach.inspectCol1, dataService.DataTeach.inspectRow2, dataService.DataTeach.inspectCol2, dataService.DataTeach.grayMin, dataService.DataTeach.grayMax);
                }
            }
        }

        private void cbxAlignXN_Click(object sender, RoutedEventArgs e)
        {
            if (null == motionService)
                return;

            CheckBox cbx = sender as CheckBox;

            if (false == cbx.IsChecked)
                cbx.IsChecked = true;

            double pos = double.Parse(tbxAlignX.Text);

            SetMotionButtons(false);
            motionService.RMove(axisX, -pos, dataService.DataMotion[axisX].VelNormal, dataService.DataMotion[axisX].AccelNormal);

            while (false == motionService.IsMotionDone(axisX))
            {
                System.Windows.Forms.Application.DoEvents();
            }

            SetMotionButtons(true);
            cbx.IsChecked = false;
        }

        private void cbxAlignXP_Click(object sender, RoutedEventArgs e)
        {
            if (null == motionService)
                return;

            CheckBox cbx = sender as CheckBox;

            if (false == cbx.IsChecked)
                cbx.IsChecked = true;

            double pos = double.Parse(tbxAlignX.Text);

            SetMotionButtons(false);
            motionService.RMove(axisX, pos, dataService.DataMotion[axisX].VelNormal, dataService.DataMotion[axisX].AccelNormal);

            while (false == motionService.IsMotionDone(axisX))
            {
                System.Windows.Forms.Application.DoEvents();
            }

            SetMotionButtons(true);
            cbx.IsChecked = false;
        }

        private void cbxAlignYN_Click(object sender, RoutedEventArgs e)
        {
            if (null == motionService)
                return;

            CheckBox cbx = sender as CheckBox;

            if (false == cbx.IsChecked)
                cbx.IsChecked = true;

            double pos = double.Parse(tbxAlignY.Text);

            SetMotionButtons(false);
            motionService.RMove(axisY, -pos, dataService.DataMotion[axisY].VelNormal, dataService.DataMotion[axisY].AccelNormal);

            while (false == motionService.IsMotionDone(axisY))
            {
                System.Windows.Forms.Application.DoEvents();
            }

            SetMotionButtons(true);
            cbx.IsChecked = false;
        }

        private void cbxAlignYP_Click(object sender, RoutedEventArgs e)
        {
            if (null == motionService)
                return;

            CheckBox cbx = sender as CheckBox;

            if (false == cbx.IsChecked)
                cbx.IsChecked = true;

            double pos = double.Parse(tbxAlignY.Text);

            SetMotionButtons(false);
            motionService.RMove(axisY, pos, dataService.DataMotion[axisY].VelNormal, dataService.DataMotion[axisY].AccelNormal);

            while (false == motionService.IsMotionDone(axisY))
            {
                System.Windows.Forms.Application.DoEvents();
            }

            SetMotionButtons(true);
            cbx.IsChecked = false;
        }

        private void btnApplyAlignOffset_Click(object sender, RoutedEventArgs e)
        {
            if (null == motionService)
                return;

            double posX = motionService.GetCurrentPosition(axisX);
            double posY = motionService.GetCurrentPosition(axisY);

            double offsetX = posX - dataService.DataSystem.AlignVisionX;
            double offsetY = posY - dataService.DataSystem.AlignVisionY;

            tbxAlignOffsetX.Text = offsetX.ToString("0.000");
            tbxAlignOffsetY.Text = offsetY.ToString("0.000");

            dataService.DataTeach.AlignX = posX;
            dataService.DataTeach.AlignY = posY;
        }

        private void cbxPunchTest_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;

            dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
            dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);

            SetHWindow();

            if (null != cbx)
            {
                if (false == cbx.IsChecked)
                {
                    cbx.IsChecked = true;
                    return;
                }

                if (true == IsInitHWindow)
                {
                    if (null != inspectService)
                    {
                        SetMotionButtons(false);
                        cbx.IsChecked = true;

                        //int ipHole = int.Parse(tbxPunchIPHole.Text);
                        int ipHole = cbIPHole.SelectedIndex;
                        int line = cbLine.SelectedIndex;
                        
                        //double posX = dataService.DataTeach.AlignX;
                        //double posY = dataService.DataTeach.AlignY;

                        dataService.DataTeach.PunchX = double.Parse(tbxPunchX.Text);
                        dataService.DataTeach.PunchY = double.Parse(tbxPunchY.Text);


                        seqService.SetHWindow(hWindow);
                        seqService.SetSequence((int)EnumSmartIC.Sequences.teachPunchTest, ipHole, (double)line);



                        //double offsetX = 0, offsetY = 0;
                        //double posX = 0.0, posY = 0.0;

                        //double velX = dataService.DataMotion[axisX].VelMove;
                        //double accelX = dataService.DataMotion[axisX].AccelMove;

                        //double velY = dataService.DataMotion[axisY].VelMove;
                        //double accelY = dataService.DataMotion[axisY].AccelMove;
                        
                        //// 현재 위치 IP Hole Check
                        //DisplayImage();
                        //inspectService.InspectTeach(hWindow, hImage, out offsetX, out offsetY);

                        //posX = -(dataService.DataSystem.PunchOffsetX - offsetX + dataService.DataTeach.PunchX);
                        ////posY = offsetY + dataService.DataTeach.PunchY;
                        //posY = offsetY + dataService.DataTeach.PunchY - dataService.DataSystem.PunchOffsetY;

                        //motionService.RMove(axisX, posX, velX, accelX);
                        //motionService.RMove(axisY, posY, velY, accelY);

                        //while (true)
                        //{
                        //    if (true == motionService.IsMotionDone(axisX))
                        //    {
                        //        if (true == motionService.IsMotionDone(axisY))
                        //            break;
                        //    }
                        //    System.Windows.Forms.Application.DoEvents();
                        //}

                        //if (SystemService.States.stop <= sysService.State)
                        //    return;
                         
                        //// Punch Down
                        //dioService.SetOutport((int)EnumSmartIC.Outports.punchDown);
                        //dioService.ResetOutport((int)EnumSmartIC.Outports.punchUp);

                        //while (true)
                        //{
                        //    if (true == dioService.IsInportOn((int)EnumSmartIC.Inports.punchDown) )
                        //    {
                        //        break;
                        //    }
                        //    System.Windows.Forms.Application.DoEvents();
                        //}

                        //if (SystemService.States.stop <= sysService.State)
                        //    return;

                        //dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
                        //dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);

                        //while (true)
                        //{
                        //    if (true == dioService.IsInportOn((int)EnumSmartIC.Inports.punchUp))
                        //    {
                        //        break;
                        //    }
                        //    System.Windows.Forms.Application.DoEvents();
                        //}

                        //posX = dataService.DataSystem.PunchOffsetX;
                        //posY = dataService.DataSystem.PunchOffsetY;

                        //motionService.RMove(axisX, posX, velX, accelX);
                        //motionService.RMove(axisY, posY, velY, accelY);

                        //while (true)
                        //{
                        //    if (true == motionService.IsMotionDone(axisX))
                        //    {
                        //        if( true == motionService.IsMotionDone(axisY) )
                        //            break;
                        //    }
                        //    System.Windows.Forms.Application.DoEvents();
                        //}

                        //DisplayImage();

                        SetMotionButtons(true);
                        cbx.IsChecked = false;
                    }
                }

            }

            SetMotionButtons(true);
            cbx.IsChecked = false;

        }

        private void btnApplyPunchOffset_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cbxPunchTestInspect_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void SetHWindow()
        {
            if ("EXTERN" == DataService.Singleton.DataSystem.CamType)
                return;

            if (false == IsInitHWindow)
            {
                hWPF = new HWindowWPF((int)canvas.ActualWidth, (int)canvas.ActualHeight);
                canvas.Children.Add(hWPF);

                hWindow = hWPF.HalconWindow;

                HOperatorSet.GenEmptyObj(out hImage);

                HOperatorSet.SetPart(hWindow, 0, 0, (HTuple)grabService.Height, (HTuple)grabService.Width);

                IsInitHWindow = true;
            }
        }

        private void SetMotionButtons(bool enable = false)
        {
            cbxBufferReady.IsEnabled = enable;
            cbxReelMoveToPunch.IsEnabled = enable;
            cbxReviewMoveToPunch.IsEnabled = enable;
            cbxMoveToOrigin.IsEnabled = enable;
            cbxPunchTestInspect.IsEnabled = enable;


            cbxAlignXN.IsEnabled = enable;
            cbxAlignXP.IsEnabled = enable;
            cbxAlignYN.IsEnabled = enable;
            cbxAlignYP.IsEnabled = enable;
            cbxPunchTest.IsEnabled = enable;

            if (true == enable)
            {
                if( false == btnImageLive.IsChecked )
                    cbxPunchVision.IsEnabled = enable;
            }
            else
            {
                cbxPunchVision.IsEnabled = enable;
                cbxPunchVision.IsChecked = enable;
            }
        }

        private void SetMotionButtonsCheck(bool enable = false)
        {
            cbxBufferReady.IsChecked = enable;
            cbxReelMoveToPunch.IsChecked = enable;
            cbxReviewMoveToPunch.IsChecked = enable;
            cbxMoveToOrigin.IsChecked = enable;
            cbxPunchTestInspect.IsChecked = enable;


            cbxAlignXN.IsChecked = enable;
            cbxAlignXP.IsChecked = enable;
            cbxAlignYN.IsChecked = enable;
            cbxAlignYP.IsChecked = enable;
            cbxPunchTest.IsChecked = enable;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (true == btnStop.IsChecked)
            {
                sysService.State = SystemService.States.stop;
                seqService.FireEventManualJog(0);
            }
            else
                sysService.State = SystemService.States.reset;
        }

        private void cbxAlignVision_Click(object sender, RoutedEventArgs e)
        {
            double offsetX = 0, offsetY = 0;

            //if ("EXTERN" == DataService.Singleton.DataSystem.CamType)
            //{
            //    if (0 == grabService.Grab(out hImage))
            //    {
            //        inspectService.Inspect(hWindow, hImage, out offsetX, out offsetY);
            //    }
            //}
            //else
            {
                SetHWindow();

                //if (true == IsGrab)
                //{
                if (null != inspectService)
                {
                    //hImage.DispObj(hWindow);
                    DisplayImage();

                    if (true == IsGrab)
                    {
                        //inspectService.InspectTeach(hWindow, hImage, out offsetX, out offsetY);

                        motionService.RMove(axisX, offsetX, dataService.DataMotion[axisX].VelMove, dataService.DataMotion[axisY].AccelMove);
                        motionService.RMove(axisY, offsetY, dataService.DataMotion[axisY].VelMove, dataService.DataMotion[axisY].AccelMove);

                        while (true)
                        {
                            if (true == motionService.IsMotionDone(axisX))
                            {
                                if (true == motionService.IsMotionDone(axisY))
                                    break;
                            }
                            System.Windows.Forms.Application.DoEvents();
                        }

                        Thread.Sleep(50);
                        DisplayImage();
                        inspectService.Inspect(hWindow, hImage, out offsetX, out offsetY);
                    }
                }
            }

           
            cbxAlignVision.IsChecked = false;
        }

        private void cbxPunchVision_Click(object sender, RoutedEventArgs e)
        {
            //if ("EXTERN" == DataService.Singleton.DataSystem.CamType)
            //{
            //    grabService.Grab(out hImage);

            //}
            //else
            {

                double offsetX = 0, offsetY = 0;

                SetHWindow();

                if (true == IsGrab)
                {
                    if (null != inspectService)
                    {
                        DisplayImage();

                        if (true == IsGrab)
                        {
                            if( null != hImage )
                                hImage.DispObj(hWindow);

                            inspectService.InspectCircle(hWindow, hImage, out offsetX, out offsetY,
                                dataService.DataTeach.searchRow1, dataService.DataTeach.searchCol1, dataService.DataTeach.searchRow2, dataService.DataTeach.searchCol2,
                                dataService.DataTeach.grayMin, dataService.DataTeach.grayMax);
                        }
                    }
                }
            }

            cbxPunchVision.IsChecked = false;

        }


        private void cbxLineA_Click(object sender, RoutedEventArgs e)
        {
            btnImageSelect.IsChecked = false;
            btnImageInspectArea.IsChecked = false;

            tool = enTools.none;
        }


        private void cbxLineB_Click(object sender, RoutedEventArgs e)
        {
            btnImageSelect.IsChecked = false;
            btnImageInspectArea.IsChecked = false;

            tool = enTools.none;
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
                    ButtonLock_Pause();
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
                    SetMotionButtons(true);
                    break;
                case SystemService.States.lightAlarm:
                    break;
                case SystemService.States.stop:
                case SystemService.States.heavyAlarm:
                case SystemService.States.emg:
                    SetMotionButtons(false);
                    SetMotionButtonsCheck(false);
                    break;
            }
        }
        // 0번째...
        private void ButtonLock_Pause()
        {
            cbxReelMoveToPunch.IsEnabled = false;
            cbxReviewMoveToPunch.IsEnabled = false;
            cbxMoveToOrigin.IsEnabled = false;
            btnStop.IsEnabled = false;

            tbxPunchX.IsEnabled = false;
            tbxPunchY.IsEnabled = false;
            cbxPunchVision.IsEnabled = false;
            cbxPunchTest.IsEnabled = false;
            cbIPHole.IsEnabled = false;
            cbLine.IsEnabled = false;

            cbxAlignVision.IsEnabled = false;
        }

        private void cbxAlignVisionPosOffset_Click(object sender, RoutedEventArgs e)
        {
            //dataService.DataSystem.AlignPosOffsetX = double.Parse(tbxAlignPosOffsetX.ToString());
            //dataService.DataSystem.AlignPosOffsetY = double.Parse(tbxAlignPosOffsetY.ToString());
            dataService.DataSystem.AlignPosOffsetX = double.Parse(tbxAlignPosOffsetX.Text);
            dataService.DataSystem.AlignPosOffsetY = double.Parse(tbxAlignPosOffsetY.Text);
            dataService.DataSystem.OffSetSave();

            int axisX = (int)EnumSmartIC.Axis.punchX;
            int axisY = (int)EnumSmartIC.Axis.punchY;
            Double velX = dataService.DataMotion[axisX].VelMove;
            Double accelX = dataService.DataMotion[axisX].AccelMove;
            Double velY = dataService.DataMotion[axisY].VelMove;
            Double accelY = dataService.DataMotion[axisY].AccelMove;
            Double posX = dataService.DataSystem.AlignVisionX + dataService.DataSystem.AlignPosOffsetX;
            Double posY = dataService.DataSystem.AlignVisionY + dataService.DataSystem.AlignPosOffsetY;

            motionService.AMove(axisX, posX, velX, accelX);
            motionService.AMove(axisY, posY, velY, accelY);
        }

    }
}
