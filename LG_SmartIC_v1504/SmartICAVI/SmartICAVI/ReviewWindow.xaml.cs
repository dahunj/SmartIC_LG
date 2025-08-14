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
using System.IO;

namespace SmartICAVI
{
    /// <summary>
    /// ReviewWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ReviewWindow : Window
    {
        
        #region Variables
        private Label[] lblLines;
        private Label[] lblLineAs;
        private Label[] lblLineBs;
        private Label[] lblLineCs;
        private TextBlock[] tblLines;


        private List<int> listNG;


        private SystemService sysService = null;
        private DataService dataService = null;
        private SequenceService seqService = null;
        private DispatcherTimer timer = null;

        private DateTime timeUp = DateTime.Now;
        private DateTime timeDown = DateTime.Now;

        private int countTimer = 0;
        private int countTest = 0;

        private int curIndexPunch = -1;
        private string curLinePunch = "";

        protected Thread threadDisplay = null;

        private bool isClearedImageTop; //이미지를 삭제하면 TRUE
        private bool isClearedImageBottom;    //이미지를 삭제하면 TRUE
        private bool isClearedImageMono;    //이미지를 삭제하면 TRUE

        private int oldEnd = -1;
        private int countOldEnd = 0;

        private int timeoutDat = 1000;
        private int timeoutImage = 200;

        private bool m_bStop_KeyIn = false;
        #endregion

        #region Properties
        private Window OwnerWindow { get; set; }

        private int SelectedListBox { get; set; }

        private int IndexNG { get; set; }
        private int IndexDisplay { get; set; }
        private int IndexSelected { get; set; }
        private string LineSelected { get; set; }
        private bool IsModifyA { get; set; }
        private bool IsModifyB { get; set; }
        private bool IsModifyC { get; set; }
        private bool IsDisplayedA { get; set; }
        private bool IsDisplayedB { get; set; }
        private bool IsDisplayedC { get; set; }
        private bool IsAutoModify { get; set; }
        private string LineNG { get; set; }

        private int ModifyValue1 { get; set; }
        private int ModifyValue2 { get; set; }
        private int ModifyValue3 { get; set; }

        private string TempValue1 { get; set; }
        private string TempValue2 { get; set; }
        private string TempValue3 { get; set; }

        private int ImageWidth { get; set; }      // Image Width;
        private int ImageHeight { get; set; }     // Image Height
        private int ImageColumns { get; set; }    // Image Array
        private Thickness ImageMargin { get; set; } // Image Margin

        private bool IsThreadDisplay { get; set; }
        private bool IsDrawing { get; set; }

        private int CountNGListView { get; set; }

        private bool IsDisplayDone { get; set; }

        private bool IsClearedImages
        {
            get
            {
                if (false == isClearedImageMono)
                    return false;
                if (false == isClearedImageBottom)
                    return false;
                if (false == isClearedImageTop)
                    return false;

                return true;
            }
        }
        
        #endregion

        static bool InvokeRequired
        {
            get { return Dispatcher.CurrentDispatcher != Application.Current.Dispatcher; }
        }

        public ReviewWindow(Window win)
        {
            InitializeComponent();

            OwnerWindow = win;

            ImageColumns = 0;

            CountNGListView = 0;

            IndexNG = -1;
            LineNG = "";
            IsModifyA = true;
            IsModifyB = true;
            IsModifyC = true;
            IsDisplayedA = true;
            IsDisplayedB = true;
            IsDisplayedC = true;
            IsThreadDisplay = false;
            IsDrawing = false;

            isClearedImageTop = true;
            isClearedImageBottom = true;
            isClearedImageMono = true;
            IsDisplayDone = false;
            
            listNG = new List<int>();

            lblLines = new Label[50] {
                lblLine01, lblLine02, lblLine03, lblLine04, lblLine05,
                lblLine06, lblLine07, lblLine08, lblLine09, lblLine10,

                lblLine11, lblLine12, lblLine13, lblLine14, lblLine15,
                lblLine16, lblLine17, lblLine18, lblLine19, lblLine20,

                lblLine21, lblLine22, lblLine23, lblLine24, lblLine25,
                lblLine26, lblLine27, lblLine28, lblLine29, lblLine30,

                lblLine31, lblLine32, lblLine33, lblLine34, lblLine35,
                lblLine36, lblLine37, lblLine38, lblLine39, lblLine40,

                lblLine41, lblLine42, lblLine43, lblLine44, lblLine45,
                lblLine46, lblLine47, lblLine48, lblLine49, lblLine50,
            };

            lblLineAs = new Label[50]{
                lblLineA01, lblLineA02, lblLineA03, lblLineA04, lblLineA05,
                lblLineA06, lblLineA07, lblLineA08, lblLineA09, lblLineA10,

                lblLineA11, lblLineA12, lblLineA13, lblLineA14, lblLineA15,
                lblLineA16, lblLineA17, lblLineA18, lblLineA19, lblLineA20,

                lblLineA21, lblLineA22, lblLineA23, lblLineA24, lblLineA25,
                lblLineA26, lblLineA27, lblLineA28, lblLineA29, lblLineA30,

                lblLineA31, lblLineA32, lblLineA33, lblLineA34, lblLineA35,
                lblLineA36, lblLineA37, lblLineA38, lblLineA39, lblLineA40,

                lblLineA41, lblLineA42, lblLineA43, lblLineA44, lblLineA45,
                lblLineA46, lblLineA47, lblLineA48, lblLineA49, lblLineA50,
            };

            lblLineBs = new Label[50]{
                lblLineB01, lblLineB02, lblLineB03, lblLineB04, lblLineB05,
                lblLineB06, lblLineB07, lblLineB08, lblLineB09, lblLineB10,

                lblLineB11, lblLineB12, lblLineB13, lblLineB14, lblLineB15,
                lblLineB16, lblLineB17, lblLineB18, lblLineB19, lblLineB20,

                lblLineB21, lblLineB22, lblLineB23, lblLineB24, lblLineB25,
                lblLineB26, lblLineB27, lblLineB28, lblLineB29, lblLineB30,

                lblLineB31, lblLineB32, lblLineB33, lblLineB34, lblLineB35,
                lblLineB36, lblLineB37, lblLineB38, lblLineB39, lblLineB40,

                lblLineB41, lblLineB42, lblLineB43, lblLineB44, lblLineB45,
                lblLineB46, lblLineB47, lblLineB48, lblLineB49, lblLineB50,
            };

            lblLineCs = new Label[50]{
                lblLineC01, lblLineC02, lblLineC03, lblLineC04, lblLineC05,
                lblLineC06, lblLineC07, lblLineC08, lblLineC09, lblLineC10,

                lblLineC11, lblLineC12, lblLineC13, lblLineC14, lblLineC15,
                lblLineC16, lblLineC17, lblLineC18, lblLineC19, lblLineC20,

                lblLineC21, lblLineC22, lblLineC23, lblLineC24, lblLineC25,
                lblLineC26, lblLineC27, lblLineC28, lblLineC29, lblLineC30,

                lblLineC31, lblLineC32, lblLineC33, lblLineC34, lblLineC35,
                lblLineC36, lblLineC37, lblLineC38, lblLineC39, lblLineC40,

                lblLineC41, lblLineC42, lblLineC43, lblLineC44, lblLineC45,
                lblLineC46, lblLineC47, lblLineC48, lblLineC49, lblLineC50,
            };

            tblLines = new TextBlock[50];

            for (int i = 0; i < 50; ++i)
            {
                tblLines[i] = new TextBlock();
                tblLines[i].TextWrapping = TextWrapping.Wrap;

                tblLines[i].Text = string.Format("{0:00}", i + 1);

                lblLines[i].Content = tblLines[i];
            }

            cbImageArray.Items.Add("상하");
            cbImageArray.Items.Add("좌우");

            cbImageNumber.Items.Add("Auto");
            cbImageNumber.Items.Add("1");
            cbImageNumber.Items.Add("2");
            cbImageNumber.Items.Add("3");
            cbImageNumber.Items.Add("4");
            cbImageNumber.Items.Add("5");
            cbImageNumber.Items.Add("6");
            cbImageNumber.Items.Add("7");
            cbImageNumber.Items.Add("8");
            cbImageNumber.Items.Add("9");
            cbImageNumber.Items.Add("10");

            cbImageSize.Items.Add("Auto");
            cbImageSize.Items.Add("100");
            cbImageSize.Items.Add("150");
            cbImageSize.Items.Add("200");
            cbImageSize.Items.Add("250");
            cbImageSize.Items.Add("300");
            cbImageSize.Items.Add("350");
            cbImageSize.Items.Add("400");
            cbImageSize.Items.Add("500");

            cbVerifyMethod.Items.Add("전체판정");
            cbVerifyMethod.Items.Add("개별판정");

            ImageMargin = new Thickness(10, 0, 10, 0);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            sysService = SystemService.Singleton;
            dataService = DataService.Singleton;
            seqService = SequenceService.Singleton;

            
            isClearedImageTop = true;
            isClearedImageBottom = true;
            isClearedImageMono = true; 
            

            System.Windows.Forms.Screen[] screens = System.Windows.Forms.Screen.AllScreens;

            int posX = 0;
            int width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            int height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

            for (int i = 0; i < screens.Length; ++i)
            {
                if (posX < screens[i].Bounds.Left)
                {
                    posX = screens[i].Bounds.Left;
                    width = screens[i].Bounds.Width;
                    height = screens[i].Bounds.Height;
                }
            }

            this.Left = posX;
            this.Top = 0;
            this.Width = width;
            this.Height = height;
            this.ResizeMode = ResizeMode.NoResize;

            cbImageArray.SelectedIndex = dataService.DataSystem.ReviewImageArray;
            cbImageNumber.SelectedIndex = dataService.DataSystem.ReviewImageNumber;
            cbImageSize.SelectedIndex = dataService.DataSystem.ReviewImageSize;
            cbVerifyMethod.SelectedIndex = dataService.DataSystem.VerifyMethod;

            rdViewVerify.IsChecked = true;
            lbTitle.Content = "Verify View";
            
            SetVerifyEnable();

            seqService.EventPunch += OnEventPunch;
            seqService.EventPunchImage += OnEventPunchImage;

            // 3Line
            if (3 == dataService.DataRecipe.Line)
            {
                for (int i = 0; i < lblLineCs.Length; ++i)
                {
                    lblLineCs[i].Visibility = Visibility.Visible;
                }

                lblLineC.Visibility = Visibility.Visible;
            }
            else
            {
                for (int i = 0; i < lblLineCs.Length; ++i)
                {
                    lblLineCs[i].Visibility = Visibility.Hidden;
                }

                lblLineC.Visibility = Visibility.Hidden;
            }

            dataService.EventChangedRecipe += OnEventChangedRecipe;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            seqService.EventPunch -= OnEventPunch;
            seqService.EventPunchImage -= OnEventPunchImage;

            dataService.EventChangedRecipe -= OnEventChangedRecipe;

            listNG.Clear();
            StopThread();
            ResetTimer();
            GC.Collect();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //DragMove();
        }

        private void lbxTop_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            ShowImageViewer();
        }

        private void lbxBottom_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            ShowImageViewer();
        }

        private void lbxMono_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            ShowImageViewer();
        }

        private void lbxTop_GotFocus(object sender, RoutedEventArgs e)
        {
            SelectedListBox = 0;
        }

        private void lbxBottom_GotFocus(object sender, RoutedEventArgs e)
        {
            SelectedListBox = 1;
        }

        private void lbxMono_GotFocus(object sender, RoutedEventArgs e)
        {
            SelectedListBox = 2;
        }
        
        private void Window_Activated(object sender, EventArgs e)
        {
            //ledActiveState.IsEnabled = true;
            //lblActiveState.Content = "리뷰 활성화";
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            //ledActiveState.IsEnabled = false;
            //lblActiveState.Content = "리뷰 비활성화";
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
           
            //switch (e.Key)
            //{
            //    case Key.Return:// = 6
            //        //ShowImageViewer();
            //        break;
            //    case Key.Escape:// = 13
            //        //if (null != dataService)
            //        //{
            //        //    dataService.DataSystem.IsSelectedModify = false;
            //        //    SetVerifyEnable();
            //        //}
            //        break;
            //    case Key.Space:// = 18
            //        //if (null != dataService)
            //        //{
            //        //    dataService.DataSystem.IsSelectedModify = true;
            //        //    SetVerifyEnable();
            //        //}
            //        break;
            //    case Key.Left:// = 23,
            //        break;
            //    case Key.Up:// = 24,
            //        //Up();
            //        break;
            //    case Key.Right:// = 25,
            //        break;
            //    case Key.Down:// = 26,
            //        //Down();
            //        break;
                
            //    case Key.D0:// = 34
            //    case Key.D1:// = 35
            //    case Key.D2:// = 36
            //    case Key.D3:// = 37
            //    case Key.D4:// = 38
            //    case Key.D5:// = 39
            //    case Key.D6:// = 40
            //    case Key.D7:// = 41
            //    case Key.D8:// = 42
            //    case Key.D9:// = 43
            //        Log_Trace.WriteLine(string.Format("Modify Key = {0}", (int)e.Key));
            //        Modify((int)(e.Key - Key.D0));
            //        break;
            //    case Key.NumPad0:// = 74
            //    case Key.NumPad1:// = 75
            //    case Key.NumPad2:// = 76
            //    case Key.NumPad3:// = 77
            //    case Key.NumPad4:// = 78
            //    case Key.NumPad5:// = 79
            //    case Key.NumPad6:// = 80
            //    case Key.NumPad7:// = 81
            //    case Key.NumPad8:// = 82
            //    case Key.NumPad9:// = 83
            //        Log_Trace.WriteLine(string.Format("Modify Key = {0}", (int)e.Key));
            //        Modify((int)(e.Key - Key.NumPad0));
            //        break;
            //    default:
            //        break;
            //}
        }


        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Return:// = 6
                    //ShowImageViewer();
                    break;
                case Key.Escape:// = 13
                    //if (null != dataService)
                    //{
                    //    dataService.DataSystem.IsSelectedModify = false;
                    //    SetVerifyEnable();
                    //}
                    break;
                case Key.Space:// = 18
                    //if (null != dataService)
                    //{
                    //    dataService.DataSystem.IsSelectedModify = true;
                    //    SetVerifyEnable();
                    //}
                    // 검사결과 그대로 적용
                    if (m_bStop_KeyIn == false)
                    {
                        Log_Trace.WriteLine("Modify Key = Space");
                        //ModifyInspected();
                        Modify(1);
                    }
                    break;
                case Key.Left:// = 23,
                    break;
                case Key.Up:// = 24,
                    //Up();
                    break;
                case Key.Right:// = 25,
                    break;
                case Key.Down:// = 26,
                    //Down();
                    break;

                case Key.D0:// = 34
                case Key.D1:// = 35
                case Key.D2:// = 36
                case Key.D3:// = 37
                case Key.D4:// = 38
                case Key.D5:// = 39
                case Key.D6:// = 40
                case Key.D7:// = 41
                case Key.D8:// = 42
                case Key.D9:// = 43
                    if (m_bStop_KeyIn == false)
                    {
                        Log_Trace.WriteLine(string.Format("Modify Key = {0}", (int)e.Key));
                        Modify((int)(e.Key - Key.D0));
                    }
                    break;
                case Key.NumPad0:// = 74
                case Key.NumPad1:// = 75
                case Key.NumPad2:// = 76
                case Key.NumPad3:// = 77
                case Key.NumPad4:// = 78
                case Key.NumPad5:// = 79
                case Key.NumPad6:// = 80
                case Key.NumPad7:// = 81
                case Key.NumPad8:// = 82
                case Key.NumPad9:// = 83
                    if (m_bStop_KeyIn == false)
                    {
                        Log_Trace.WriteLine(string.Format("Modify Key = {0}", (int)e.Key));
                        Modify((int)(e.Key - Key.NumPad0));
                    }
                    break;
                default:
                    break;
            }
        }


        private void Up()
        {
            if (0 == SelectedListBox)
                return;

            if (false == rdViewVerify.IsChecked)
                return;

            if (false == dataService.DataSystem.IsSelectedModify)
                return;

            //if (200 > DateTime.Now.Ticks - timeUp.Ticks)
            //    return;
            //timeUp = DateTime.Now;

            try
            {
                // Bottom -> Top
                if (1 == SelectedListBox)
                {
                    if (0 < lbxTop.Items.Count)
                    {
                        if (lbxTop.Items.Count > lbxBottom.SelectedIndex)
                        {
                            lbxTop.SelectedIndex = lbxBottom.SelectedIndex;
                        }
                        else
                        {
                            lbxTop.SelectedIndex = lbxTop.Items.Count - 1;
                        }

                        var lbxItem = (ListBoxItem)lbxTop.ItemContainerGenerator.ContainerFromItem(lbxTop.SelectedItem);
                        lbxItem.Focus();
                    }
                }

                // Mono -> Bottom
                else if (2 == SelectedListBox)
                {
                    if (0 < lbxBottom.Items.Count)
                    {
                        if (lbxBottom.Items.Count > lbxMono.SelectedIndex)
                        {
                            lbxBottom.SelectedIndex = lbxMono.SelectedIndex;
                        }
                        else
                        {
                            lbxBottom.SelectedIndex = lbxBottom.Items.Count - 1;
                        }

                        var lbxItem = (ListBoxItem)lbxBottom.ItemContainerGenerator.ContainerFromItem(lbxBottom.SelectedItem);
                        lbxItem.Focus();
                    }
                    else
                    {
                        if (0 < lbxTop.Items.Count)
                        {
                            if (lbxTop.Items.Count > lbxBottom.SelectedIndex)
                            {
                                lbxTop.SelectedIndex = lbxBottom.SelectedIndex;
                            }
                            else
                            {
                                lbxTop.SelectedIndex = lbxTop.Items.Count - 1;
                            }

                            var lbxItem = (ListBoxItem)lbxTop.ItemContainerGenerator.ContainerFromItem(lbxTop.SelectedItem);
                            lbxItem.Focus();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine("ReviewWindow.Up() : " + exc.Message);
            }
        }

        private void Down()
        {
            if (2 == SelectedListBox)
                return;

            if (false == rdViewVerify.IsChecked)
                return;

            if (false == dataService.DataSystem.IsSelectedModify)
                return;

            try
            {
                // Bottom -> Mono
                if (1 == SelectedListBox)
                {
                    if (0 < lbxMono.Items.Count)
                    {
                        if (lbxMono.Items.Count > lbxBottom.SelectedIndex)
                        {
                            lbxMono.SelectedIndex = lbxBottom.SelectedIndex;
                        }
                        else
                        {
                            lbxMono.SelectedIndex = lbxMono.Items.Count - 1;
                        }

                        var lbxItem = (ListBoxItem)lbxMono.ItemContainerGenerator.ContainerFromItem(lbxMono.SelectedItem);
                        lbxItem.Focus();
                    }
                }

                else if (0 == SelectedListBox)
                {
                    if (0 < lbxBottom.Items.Count)
                    {
                        if (lbxBottom.Items.Count > lbxTop.SelectedIndex)
                        {
                            lbxBottom.SelectedIndex = lbxTop.SelectedIndex;
                        }
                        else
                        {
                            lbxBottom.SelectedIndex = lbxBottom.Items.Count - 1;
                        }

                        var lbxItem = (ListBoxItem)lbxBottom.ItemContainerGenerator.ContainerFromItem(lbxBottom.SelectedItem);
                        lbxItem.Focus();
                    }
                    else
                    {
                        if (0 < lbxMono.Items.Count)
                        {
                            if (lbxMono.Items.Count > lbxBottom.SelectedIndex)
                            {
                                lbxMono.SelectedIndex = lbxBottom.SelectedIndex;
                            }
                            else
                            {
                                lbxMono.SelectedIndex = lbxMono.Items.Count - 1;
                            }

                            var lbxItem = (ListBoxItem)lbxMono.ItemContainerGenerator.ContainerFromItem(lbxMono.SelectedItem);
                            lbxItem.Focus();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine("ReviewWindow.Down() : " + exc.Message);
            }
        }

        private void Test()
        {
            if ("Virtual" != dataService.DataSystem.MachineName)
                return;

            if (true == IsDrawing)
                return;

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            {
                IsDrawing = true;

                ClearDisplay();
                ClearPunchView();
                ClearNGListView();

                lbxTop.Visibility = System.Windows.Visibility.Hidden;
                lbxBottom.Visibility = System.Windows.Visibility.Hidden;
                lbxMono.Visibility = System.Windows.Visibility.Hidden;

                string image1 = "D:/My Project/x32/13_SmartIC_AVI/Images/1.jpg";
                string image2 = "D:/My Project/x32/13_SmartIC_AVI/Images/2.jpg";
                string image3 = "D:/My Project/x32/13_SmartIC_AVI/Images/3.jpg";
                string image4 = "D:/My Project/x32/13_SmartIC_AVI/Images/4.jpg";

                ReviewImageControl imgControl1 = new ReviewImageControl();
                ReviewImageControl imgControl2 = new ReviewImageControl();
                ReviewImageControl imgControl3 = new ReviewImageControl();
                ReviewImageControl imgControl4 = new ReviewImageControl();

                ReviewImageControl imgControl11 = new ReviewImageControl();
                ReviewImageControl imgControl12 = new ReviewImageControl();
                ReviewImageControl imgControl13 = new ReviewImageControl();
                ReviewImageControl imgControl14 = new ReviewImageControl();

                ReviewImageControl imgControl5 = new ReviewImageControl();
                ReviewImageControl imgControl6 = new ReviewImageControl();
                ReviewImageControl imgControl7 = new ReviewImageControl();
                ReviewImageControl imgControl8 = new ReviewImageControl();


                ReviewImageControl imgPunch1 = new ReviewImageControl();
                ReviewImageControl imgPunch2 = new ReviewImageControl();
                ReviewImageControl imgPunch3 = new ReviewImageControl();
                ReviewImageControl imgPunch4 = new ReviewImageControl();

                //int height = ((int)lbxTop.ActualHeight) / 2 - 6 - (int)(imgControl1.TitleFontSize + 2.0);
                //int height = ((int)lbxMono.ActualHeight) - 12 - (int)(imgControl1.TitleFontSize + 2.0);

                if (0 == countTest || 1 == countTest)
                {
                    imgControl1.Title = "Image1, Image2";
                    imgControl1.PathColor = image1;
                    //imgControl1.PathHSI = image2;
                    imgControl1.ImageWidth = ImageWidth;
                    imgControl1.ImageHeight = ImageHeight;
                    imgControl1.Margin = ImageMargin;
                    imgControl1.ColorName = "Image1";


                    imgControl2.Title = "Image3, x";
                    imgControl2.PathColor = image2;
                    imgControl2.ImageWidth = ImageWidth;
                    imgControl2.ImageHeight = ImageHeight;
                    imgControl2.Margin = ImageMargin;
                    imgControl2.ColorName = "Image3";

                    imgControl3.Title = "x, Image4";
                    imgControl3.PathColor = image3;
                    //imgControl3.PathHSI = image3
                    imgControl3.ImageWidth = ImageWidth;
                    imgControl3.ImageHeight = ImageHeight;
                    imgControl3.Margin = ImageMargin;
                    imgControl3.ColorName = "Imagex";

                    imgControl4.PathColor = image4;
                    imgControl4.ImageWidth = ImageWidth;
                    imgControl4.ImageHeight = ImageHeight;
                    imgControl4.Margin = ImageMargin;

                    lbxTop.Items.Add(imgControl1);
                    lbxTop.Items.Add(imgControl2);
                    lbxTop.Items.Add(imgControl3);
                    lbxTop.Items.Add(imgControl4);

                    for (int i = 0; i < 20; ++i)
                    {
                        ReviewImageControl imgCtrl = new ReviewImageControl();

                        imgCtrl.Title = "Image_" + i.ToString();
                        imgCtrl.PathColor = image1;
                        //imgControl1.PathHSI = image2;
                        imgCtrl.ImageWidth = ImageWidth;
                        imgCtrl.ImageHeight = ImageHeight;
                        imgCtrl.Margin = ImageMargin;
                        imgCtrl.ColorName = "Image1";

                        lbxTop.Items.Add(imgCtrl);

                        DoEvents();
                    }
                }

                if (0 == countTest || 2 == countTest)
                {
                    imgControl11.Title = "Image1, Image2";
                    imgControl11.PathColor = image1;
                    imgControl11.PathHSI = image2;
                    imgControl11.ImageWidth = ImageWidth;
                    imgControl11.ImageHeight = ImageHeight;
                    imgControl11.Margin = ImageMargin;
                    imgControl11.ColorName = "Image1";


                    imgControl12.Title = "Image3, x";
                    imgControl12.PathColor = image3;
                    imgControl12.ImageWidth = ImageWidth;
                    imgControl12.ImageHeight = ImageHeight;
                    imgControl12.Margin = ImageMargin;
                    imgControl12.ColorName = "Image3";

                    imgControl13.Title = "x, Image4";
                    imgControl12.PathColor = "pack://application:,,/Images/Transparent.png";
                    imgControl13.PathHSI = image4;
                    imgControl13.ImageWidth = ImageWidth;
                    imgControl13.ImageHeight = ImageHeight;
                    imgControl13.Margin = ImageMargin;
                    imgControl13.ColorName = "x";

                    imgControl14.ImageWidth = ImageWidth;
                    imgControl14.ImageHeight = ImageHeight;
                    imgControl14.Margin = ImageMargin;

                    lbxBottom.Items.Add(imgControl11);
                    lbxBottom.Items.Add(imgControl12);
                    lbxBottom.Items.Add(imgControl13);
                    lbxBottom.Items.Add(imgControl14);

                    for (int i = 0; i < 20; ++i)
                    {
                        ReviewImageControl imgCtrl = new ReviewImageControl();

                        imgCtrl.Title = "Image_" + i.ToString();
                        imgCtrl.PathColor = image1;
                        //imgControl1.PathHSI = image2;
                        imgCtrl.ImageWidth = ImageWidth;
                        imgCtrl.ImageHeight = ImageHeight;
                        imgCtrl.Margin = ImageMargin;
                        imgCtrl.ColorName = "Image1";

                        lbxBottom.Items.Add(imgCtrl);

                        DoEvents();
                    }

                    DoEvents();
                }
                if (0 == countTest || 3 == countTest)
                {
                    imgControl5.IsVisibleHSI = false;
                    imgControl5.PathColor = image1;
                    imgControl5.ImageWidth = ImageWidth;
                    imgControl5.ImageHeight = ImageHeight;
                    imgControl5.Margin = ImageMargin;
                    imgControl13.ColorName = "image1";

                    imgControl6.IsVisibleHSI = false;
                    imgControl6.PathColor = image2;
                    imgControl6.ImageWidth = ImageWidth;
                    imgControl6.ImageHeight = ImageHeight;
                    imgControl6.Margin = ImageMargin;
                    imgControl13.ColorName = "image2";

                    imgControl7.IsVisibleHSI = false;
                    imgControl7.PathColor = image3;
                    imgControl7.ImageWidth = ImageWidth;
                    imgControl7.ImageHeight = ImageHeight;
                    imgControl7.Margin = ImageMargin;
                    imgControl13.ColorName = "image3";

                    imgControl8.IsVisibleHSI = false;
                    imgControl8.PathColor = image4;
                    imgControl8.ImageWidth = ImageWidth;
                    imgControl8.ImageHeight = ImageHeight;
                    imgControl8.Margin = ImageMargin;
                    imgControl13.ColorName = "image4";

                    lbxMono.Items.Add(imgControl5);

                    lbxMono.Items.Add(imgControl6);

                    lbxMono.Items.Add(imgControl7);

                    lbxMono.Items.Add(imgControl8);

                    DoEvents();

                }

                if (++countTest > 4)
                    countTest = 0;

                if (0 < lbxTop.Items.Count)
                {
                    lbxTop.SelectedIndex = 0;
                    //lbxTop.Focus();
                    SelectedListBox = 0;
                }
                if (0 < lbxBottom.Items.Count)
                {
                    lbxBottom.SelectedIndex = 0;
                    //lbxBottom.Focus();5
                    //SelectedListBox = 1;
                }
                if (0 < lbxMono.Items.Count)
                {
                    lbxMono.SelectedIndex = 0;
                    //lbxMono.Focus();
                    //SelectedListBox = 2;
                }


                imgPunch1.IsVisibleHSI = false;
                imgPunch1.PathColor = image1;
                imgPunch1.ImageWidth = ImageWidth;
                imgPunch1.ImageHeight = ImageHeight;
                imgPunch1.Margin = ImageMargin;
                imgPunch1.Title = "image1";

                imgPunch2.IsVisibleHSI = false;
                imgPunch2.PathColor = image2;
                imgPunch2.ImageWidth = ImageWidth;
                imgPunch2.ImageHeight = ImageHeight;
                imgPunch2.Margin = ImageMargin;
                imgPunch2.Title = "image2";

                imgPunch3.IsVisibleHSI = false;
                imgPunch3.PathColor = image3;
                imgPunch3.ImageWidth = ImageWidth;
                imgPunch3.ImageHeight = ImageHeight;
                imgPunch3.Margin = ImageMargin;
                imgPunch3.Title = "image3";

                imgPunch4.IsVisibleHSI = false;
                imgPunch4.PathColor = image4;
                imgPunch4.ImageWidth = ImageWidth;
                imgPunch4.ImageHeight = ImageHeight;
                imgPunch4.Margin = ImageMargin;
                imgPunch4.Title = "image4";

                lbxPunch.Items.Insert(0, imgPunch1);

                lbxPunch.Items.Insert(0, imgPunch2);

                lbxPunch.Items.Insert(0, imgPunch3);

                lbxPunch.Items.Insert(0, imgPunch4);

                for (int i = 0; i < 10; ++i)
                {
                    ReviewImageControl img = new ReviewImageControl();

                    img.IsVisibleHSI = false;
                    img.PathColor = image4;
                    img.ImageWidth = ImageWidth;
                    img.ImageHeight = ImageHeight;
                    img.Margin = ImageMargin;
                    img.Title = string.Format("{0}_{1} ({2:0.000}, {3:0.000})", i, i, i, i);

                    lbxPunch.Items.Insert(0, img);

                    System.Windows.Forms.Application.DoEvents();
                }

                lbxTop.Visibility = System.Windows.Visibility.Visible;
                lbxBottom.Visibility = System.Windows.Visibility.Visible;
                lbxMono.Visibility = System.Windows.Visibility.Visible;

                AddNGListView(1, 1, "A", "J");
                AddNGListView(2, 10, "A", "A");
                AddNGListView(3, 100, "A", "C");
                AddNGListView(4, 1000, "A", "U");
                AddNGListView(5, 10000, "A", "V");
                AddNGListView(6, 20000, "A", "K");
                AddNGListView(7, 30000, "A", "B");
                AddNGListView(8, 40000, "A", "O");

                imgControl1 = null;
                imgControl2 = null;
                imgControl3 = null;
                imgControl4 = null;

                imgControl11 = null;
                imgControl12 = null;
                imgControl13 = null;
                imgControl14 = null;

                imgControl5 = null;
                imgControl6 = null;
                imgControl7 = null;
                imgControl8 = null;

                IsDrawing = false;
            } 
            ));
        }

 
        private void SetTimer()
        {
            //System.Diagnostics.Debug.WriteLine("Review SetTimer()");
            if (null != timer)
            {
                if (true == timer.IsEnabled)
                    ResetTimer();
            }

            timer = new DispatcherTimer();
            timer.Interval = new System.TimeSpan(0, 0, 0, 0, 100);
            timer.IsEnabled = true;
            timer.Tick += new EventHandler(timer_Tick);

            countTimer = 0;
        }

        private void ResetTimer()
        {
            if (null != timer)
            {
                //System.Diagnostics.Debug.WriteLine("Review ResetTimer()");

                timer.IsEnabled = false;
                timer.Tick -= timer_Tick;
            }
        }

        private void StartThread()
        {
            IndexNG = -1;
            IsModifyA = true;
            IsModifyB = true;
            IsModifyC = true;
            IsDisplayedA = true;
            IsDisplayedB = true;
            IsDisplayedC = true;

            IsAutoModify = false;

            oldEnd = -1;
            countOldEnd = 0;

            listNG.Clear();

            IsThreadDisplay = true;
            //threadDisplay = new Thread(new ThreadStart(this.ThreadDisplay));
            //threadDisplay.Priority = ThreadPriority.Lowest;
            //threadDisplay.Start();

            if (3 == dataService.DataRecipe.Line)
            {
                timeoutDat = 50;
                timeoutImage = 50;
            }
            else
            {
                timeoutDat = 1000;
                timeoutImage = 200;
            }

            threadDisplay = new Thread(ThreadDisplay);
            threadDisplay.IsBackground = true;
            threadDisplay.Priority = ThreadPriority.Lowest;
            threadDisplay.Start();
        }

        private void StopThread()
        {
            IsThreadDisplay = false;
        }

        protected void ThreadDisplay()
        {
            while (true == IsThreadDisplay)
            {
                if (null != dataService)
                {
                    if (false == dataService.IsBackFeeding)
                    {
                        DisplayReview();

                        //Thread.Sleep(10);
                        ThreadDoEvents(50);

                        // 수정 모드가 아닐경우 200mSec 마다 NG 이미지를 디스플레이 한다.
                        if (false == dataService.DataSystem.IsSelectedModify)
                        {
                            // Image Display
                            if (listNG.Count > 0)
                            {
                                NotModify();
                            }
                        }

                        //if (true == IsThreadDisplay)
                        //    Test();

                        ThreadDoEvents(100);
                    }
                }
            }
        }

        protected void ThreadDoEvents(int mSec)
        {
            DateTime start = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, mSec);
            DateTime timeout = start.Add(duration);

            do
            {
                if (false == IsThreadDisplay)
                    return;

                System.Windows.Forms.Application.DoEvents();
                Thread.Sleep(1);
            } while (timeout > DateTime.Now);
        }

        private void SetImageSize()
        {
            int size = ((int)lbxTop.ActualHeight) / 2 - 6 - 13;   // Height/2 - Margin - TitleHeight/2

            ImageWidth = size;
            ImageHeight = size;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (3 < ++countTimer)
                ResetTimer();

            SetStop();
        }

        private void DisplayReview()
        {
            if (null == sysService)
                return;
            if (null == dataService)
                return;

            // 3Line
            if (3 == dataService.DataRecipe.Line)
            {
                DisplayReview3();
                return;
            }

            string value1 = "G";
            string value2 = "G";
            int start = 0;
            int end = 0;

            bool isAdd = true;
            
            // 데이터를 한번에 다 Display 해서는 안된다. 
            // 50 - IndexNG 까지의 데이터 까지만 Add 해야 한다. 

            // Mono 를 사용하는 경우
            if (true == dataService.DataSystem.IsSelectedMono)
            {
                start = dataService.DataResult.Temp.ListRaw.Count;
                end = dataService.DataResult.Mono.ListRaw.Count;

                if (start < end)
                {
                    //Log_Debug.WriteLine("ReviewWindow.Displayreview() : start({0}), end({1})", start, end);

                    // Bottom Data 확인
                    if (true == dataService.DataSystem.IsSelectedBottom)
                    {
                        if (end > dataService.DataResult.Bottom.ListRaw.Count)
                            isAdd = false;
                    }

                    // Top Data 확인
                    if (true == dataService.DataSystem.IsSelectedTop)
                    {
                        // yjs 20161208 Mono 검사가 먼저 끝났을 때 END 처리시 end 값을 Top 의 EndIndex 로 바꿔준다.
                        //if (end > dataService.DataResult.Top.ListRaw.Count)
                        //    isAdd = false;
                        if (end >= dataService.DataResult.Top.ListRaw.Count)
                        {
                            if (true == dataService.IsEndTop)
                            {
                                if (end > dataService.IndexEndTop)
                                {
                                    end = dataService.DataResult.Top.ListRaw.Count;

                                    //Log_Debug.WriteLine("ReviewWindow.Displayreview().if (end > dataService.IndexEndTop) : end({0})", end);

                                    if (end > dataService.IndexEndTop + 1)
                                    {
                                        end = dataService.IndexEndTop + 1;

                                        //Log_Debug.WriteLine("ReviewWindow.Displayreview().if (end > dataService.IndexEndTop + 1) : end({0})", end);
                                    }
                                }
                                //else
                                //    isAdd = false;
                            }
                            else
                            {
                                isAdd = false;
                            }
                        }
                    }

                    if (true == isAdd)
                    {
                        if (0 < listNG.Count)
                        {
                            if (50 < (end - listNG[0]))
                            {
                                //if (end != oldEnd)
                                //{
                                //    if( 10 > ++countOldEnd )
                                //        Log_Debug.WriteLine("ReviewWindow.Displayreview() : start({0}), end({1})", start, end);
                                //}

                                end = listNG[0] + 49;

                                //if (end != oldEnd)
                                //{
                                //    if (10 > countOldEnd)
                                //    {
                                //        Log_Debug.WriteLine("ReviewWindow.Displayreview().if (50 < (end - listNG[0])) : end({0}) = listNG[0]({1}) + 49", end, listNG[0]);
                                //    }
                                //if (end != oldEnd)
                                //{
                                //    oldEnd = end;
                                //}
                            }
                        }

                        if (end > start)
                        {
                            countOldEnd = 0;

                            if (end > dataService.DataResult.Mono.ListRaw.Count)
                            {
                                end = dataService.DataResult.Mono.ListRaw.Count;
                                //Log_Debug.WriteLine("ReviewWindow.Displayreview().if (end > dataService.DataResult.Mono.ListRaw.Count) : end({0})", end);
                            }

                            for (int i = start; i < end; ++i)
                            {
                                //value1 = dataService.DataResult.Mono.ListRaw[i].Value1;
                                //value2 = dataService.DataResult.Mono.ListRaw[i].Value2;

                                // Top 우선
                                if (true == dataService.DataSystem.IsSelectedTop)
                                {
                                    value1 = dataService.DataResult.Top.ListRaw[i].Value1;
                                    value2 = dataService.DataResult.Top.ListRaw[i].Value2;
                                }

                                if (true == dataService.DataSystem.IsSelectedBottom)
                                {
                                    if ("G" == value1)
                                        value1 = dataService.DataResult.Bottom.ListRaw[i].Value1;
                                    if ("G" == value2)
                                        value2 = dataService.DataResult.Bottom.ListRaw[i].Value2;

                                    if ("BB006" != value1 && "C" != value1)
                                        if ("BB039" == dataService.DataResult.Bottom.ListRaw[i].Value1) value1 = "BB039";

                                    if ("BB006" != value2 && "C" != value2)
                                        if ("BB039" == dataService.DataResult.Bottom.ListRaw[i].Value2) value2 = "BB039";

                                    // Bottom 이 Joint 일 경우 Joint 로 등록
                                    if ("BB006" == dataService.DataResult.Bottom.ListRaw[i].Value1) value1 = "BB006";
                                    if ("BB006" == dataService.DataResult.Bottom.ListRaw[i].Value2) value2 = "BB006";
                                }

                                try
                                {
                                    if ("G" == value1)
                                        value1 = dataService.DataResult.Mono.ListRaw[i].Value1;
                                    if ("G" == value2)
                                        value2 = dataService.DataResult.Mono.ListRaw[i].Value2;

                                    dataService.DataResult.AddTemp(i, value1, value2, "REVIEW");

                                    if (("G" != value1) || ("G" != value2))
                                        AddNG(i);
                                }
                                catch (Exception exc)
                                {
                                    Log_Exception.WriteLine("ReviewWindow.DisplayReview() : " + exc.Message);
                                    Log_Exception.WriteLine("ReviewWindow.DisplayReview().MonoIndexError : Index({0}), Length({1})", i, dataService.DataResult.Mono.ListRaw.Count);

                                    //Log_Debug.WriteLine("ReviewWindow.DisplayReview().MonoIndexError : Index({0}), Length({1})", i, dataService.DataResult.Mono.ListRaw.Count);

                                    sysService.State = SystemService.States.heavyAlarm;
                                    MsgService.Singleton.ShowAlarm((int)EnumSmartIC.HeavyAlarms.index_verify);

                                    return;
                                }
                                
                            }
                            AddReviewData();
                            DisplayTemp();
                        }
                    }
                }
            }
            // Bottom 을 사용하는 경우
            else if (true == dataService.DataSystem.IsSelectedBottom)
            {
                start = dataService.DataResult.Temp.ListRaw.Count;
                end = dataService.DataResult.Bottom.ListRaw.Count;

                //System.Diagnostics.Debug.WriteLine("Start={0}, End={1}", start, end);

                if (start < end)
                {
                    // Top Data 확인
                    if (true == dataService.DataSystem.IsSelectedTop)
                    {
                        if (end >= dataService.DataResult.Top.ListRaw.Count)
                        {
                            if (true == dataService.IsEndTop)
                            {
                                if (end > dataService.IndexEndTop)
                                {
                                    end = dataService.DataResult.Top.ListRaw.Count;

                                    if (end > dataService.IndexEndTop + 1)
                                        end = dataService.IndexEndTop + 1;
                                }
                                //else
                                //    isAdd = false;
                            }
                            else
                            {
                                isAdd = false;
                            }
                        }
                    }

                    if (true == isAdd)
                    {
                        if (0 < listNG.Count)
                        {
                            if (50 < (end - listNG[0]))
                                end = listNG[0] + 49;
                        }

                        if (end > start)
                        {
                            for (int i = start; i < end; ++i)
                            {
                                //value1 = dataService.DataResult.Bottom.ListRaw[i].Value1;
                                //value2 = dataService.DataResult.Bottom.ListRaw[i].Value2;

                                // Top 우선
                                if (true == dataService.DataSystem.IsSelectedTop)
                                {
                                    value1 = dataService.DataResult.Top.ListRaw[i].Value1;
                                    value2 = dataService.DataResult.Top.ListRaw[i].Value2;
                                }

                                if ("G" == value1 )
                                    value1 = dataService.DataResult.Bottom.ListRaw[i].Value1;
                                if ("G" == value2 )
                                    value2 = dataService.DataResult.Bottom.ListRaw[i].Value2;

                                if ("BB006" != value1 && "C" != value1)
                                    if ("BB039" == dataService.DataResult.Bottom.ListRaw[i].Value1) value1 = "BB039";
                                if ("BB006" != value2 && "C" != value2)
                                    if ("BB039" == dataService.DataResult.Bottom.ListRaw[i].Value2) value2 = "BB039";

                                // Bottom 이 Joint 일 경우 Joint 로 등록
                                if ("BB006" == dataService.DataResult.Bottom.ListRaw[i].Value1) value1 = "BB006";
                                if ("BB006" == dataService.DataResult.Bottom.ListRaw[i].Value2) value2 = "BB006";

                                dataService.DataResult.AddTemp(i, value1, value2, "REVIEW");

                                if (("G" != value1) || ("G" != value2))
                                    AddNG(i);
                            }

                            AddReviewData();
                            DisplayTemp();
                        }
                    }
                }
            }
            // Top 만 사용하는 경우
            else
            {
               
                start = dataService.DataResult.Temp.ListRaw.Count;
                end = dataService.DataResult.Top.ListRaw.Count;

                //System.Diagnostics.Debug.WriteLine(string.Format("{0}, {1}", start, end));

                if (start < end)
                {
                    //sysService.State = SystemService.States.stop;

                    if (0 < listNG.Count)
                    {
                        if (50 < (end - listNG[0]))
                            end = listNG[0] + 49;
                    }

                    if (end > start)
                    {
                        for (int i = start; i < end; ++i)
                        {
                            value1 = dataService.DataResult.Top.ListRaw[i].Value1;
                            value2 = dataService.DataResult.Top.ListRaw[i].Value2;

                            //System.Diagnostics.Debug.WriteLine(string.Format("{0:0000} = {1}, {2}", i, value1, value2));

                            dataService.DataResult.AddTemp(i, value1, value2, "REVIEW");

                            if (("G" != value1) || ("G" != value2))
                                AddNG(i);
                        }

                        AddReviewData();
                        DisplayTemp();
                    }
                }
            }
        }

        private void DisplayReview3()
        {
            if (null == sysService)
                return;
            if (null == dataService)
                return;


            string value1 = "G";
            string value2 = "G";
            string value3 = "G";

            int start = 0;
            int end = 0;

            bool isAdd = true;

            // 데이터를 한번에 다 Display 해서는 안된다. 
            // 50 - IndexNG 까지의 데이터 까지만 Add 해야 한다. 

            // Mono 를 사용하는 경우
            if (true == dataService.DataSystem.IsSelectedMono)
            {
                start = dataService.DataResult.Temp.ListRaw.Count;
                end = dataService.DataResult.Mono.ListRaw.Count;

                if (start < end)
                {
                    //Log_Debug.WriteLine("ReviewWindow.Displayreview() : start({0}), end({1})", start, end);

                    // Bottom Data 확인
                    if (true == dataService.DataSystem.IsSelectedBottom)
                    {
                        if (end > dataService.DataResult.Bottom.ListRaw.Count)
                            isAdd = false;
                    }

                    // Top Data 확인
                    if (true == dataService.DataSystem.IsSelectedTop)
                    {
                        // yjs 20161208 Mono 검사가 먼저 끝났을 때 END 처리시 end 값을 Top 의 EndIndex 로 바꿔준다.
                        //if (end > dataService.DataResult.Top.ListRaw.Count)
                        //    isAdd = false;
                        if (end >= dataService.DataResult.Top.ListRaw.Count)
                        {
                            if (true == dataService.IsEndTop)
                            {
                                if (end > dataService.IndexEndTop)
                                {
                                    end = dataService.DataResult.Top.ListRaw.Count;

                                    //Log_Debug.WriteLine("ReviewWindow.Displayreview().if (end > dataService.IndexEndTop) : end({0})", end);

                                    if (end > dataService.IndexEndTop + 1)
                                    {
                                        end = dataService.IndexEndTop + 1;

                                        //Log_Debug.WriteLine("ReviewWindow.Displayreview().if (end > dataService.IndexEndTop + 1) : end({0})", end);
                                    }
                                }
                                //else
                                //    isAdd = false;
                            }
                            else
                            {
                                isAdd = false;
                            }
                        }
                    }

                    if (true == isAdd)
                    {
                        if (0 < listNG.Count)
                        {
                            if (50 < (end - listNG[0]))
                            {
                                //if (end != oldEnd)
                                //{
                                if (10 > ++countOldEnd)
                                    Log_Debug.WriteLine("ReviewWindow.Displayreview() : start({0}), end({1})", start, end);
                                //}

                                end = listNG[0] + 49;

                                //if (end != oldEnd)
                                //{
                                if (10 > countOldEnd)
                                {
                                    Log_Debug.WriteLine("ReviewWindow.Displayreview().if (50 < (end - listNG[0])) : end({0}) = listNG[0]({1}) + 49", end, listNG[0]);
                                }
                                if (end != oldEnd)
                                {
                                    oldEnd = end;
                                }
                            }
                        }

                        if (end > start)
                        {
                            countOldEnd = 0;

                            if (end > dataService.DataResult.Mono.ListRaw.Count)
                            {
                                end = dataService.DataResult.Mono.ListRaw.Count;
                                //Log_Debug.WriteLine("ReviewWindow.Displayreview().if (end > dataService.DataResult.Mono.ListRaw.Count) : end({0})", end);
                            }

                            for (int i = start; i < end; ++i)
                            {
                                //value1 = dataService.DataResult.Mono.ListRaw[i].Value1;
                                //value2 = dataService.DataResult.Mono.ListRaw[i].Value2;

                                // Top 우선
                                if (true == dataService.DataSystem.IsSelectedTop)
                                {
                                    value1 = dataService.DataResult.Top.ListRaw[i].Value1;
                                    value2 = dataService.DataResult.Top.ListRaw[i].Value2;
                                    value3 = dataService.DataResult.Top.ListRaw[i].Value3;
                                }

                                if (true == dataService.DataSystem.IsSelectedBottom)
                                {
                                    if ("G" == value1)
                                        value1 = dataService.DataResult.Bottom.ListRaw[i].Value1;
                                    if ("G" == value2)
                                        value2 = dataService.DataResult.Bottom.ListRaw[i].Value2;
                                    if ("G" == value3)
                                        value3 = dataService.DataResult.Bottom.ListRaw[i].Value3;

                                    if ("BB006" != value1 && "C" != value1)
                                        if ("BB039" == dataService.DataResult.Bottom.ListRaw[i].Value1)
                                            value1 = "BB039";

                                    if ("BB006" != value2 && "C" != value2)
                                        if ("BB039" == dataService.DataResult.Bottom.ListRaw[i].Value2)
                                            value2 = "BB039";

                                    if ("BB006" != value3 && "C" != value3)
                                        if ("BB039" == dataService.DataResult.Bottom.ListRaw[i].Value3)
                                            value3 = "BB039";

                                    // Bottom 이 Joint 일 경우 Joint 로 등록
                                    if ("BB006" == dataService.DataResult.Bottom.ListRaw[i].Value1)
                                        value1 = "BB006";
                                    if ("BB006" == dataService.DataResult.Bottom.ListRaw[i].Value2)
                                        value2 = "BB006";
                                    if ("BB006" == dataService.DataResult.Bottom.ListRaw[i].Value3)
                                        value3 = "BB006";
                                }

                                try
                                {
                                    if ("G" == value1)
                                        value1 = dataService.DataResult.Mono.ListRaw[i].Value1;
                                    if ("G" == value2)
                                        value2 = dataService.DataResult.Mono.ListRaw[i].Value2;
                                    if ("G" == value3)
                                        value3 = dataService.DataResult.Mono.ListRaw[i].Value3;

                                    dataService.DataResult.AddTemp3(i, value1, value2, value3, "REVIEW");

                                    if (("G" != value1) || ("G" != value2) || ("G" != value3))
                                        AddNG(i);
                                }
                                catch (Exception exc)
                                {
                                    Log_Exception.WriteLine("ReviewWindow.DisplayReview() : " + exc.Message);
                                    Log_Exception.WriteLine("ReviewWindow.DisplayReview().MonoIndexError : Index({0}), Length({1})", i, dataService.DataResult.Mono.ListRaw.Count);

                                    //Log_Debug.WriteLine("ReviewWindow.DisplayReview().MonoIndexError : Index({0}), Length({1})", i, dataService.DataResult.Mono.ListRaw.Count);

                                    sysService.State = SystemService.States.heavyAlarm;
                                    MsgService.Singleton.ShowAlarm((int)EnumSmartIC.HeavyAlarms.index_verify);

                                    return;
                                }

                            }
                            AddReviewData();
                            DisplayTemp();
                        }
                    }
                }
            }
            // Bottom 을 사용하는 경우
            else if (true == dataService.DataSystem.IsSelectedBottom)
            {
                start = dataService.DataResult.Temp.ListRaw.Count;
                end = dataService.DataResult.Bottom.ListRaw.Count;

                //System.Diagnostics.Debug.WriteLine("Start={0}, End={1}", start, end);

                if (start < end)
                {
                    // Top Data 확인
                    if (true == dataService.DataSystem.IsSelectedTop)
                    {
                        if (end >= dataService.DataResult.Top.ListRaw.Count)
                        {
                            if (true == dataService.IsEndTop)
                            {
                                if (end > dataService.IndexEndTop)
                                {
                                    end = dataService.DataResult.Top.ListRaw.Count;

                                    if (end > dataService.IndexEndTop + 1)
                                        end = dataService.IndexEndTop + 1;
                                }
                                //else
                                //    isAdd = false;
                            }
                            else
                            {
                                isAdd = false;
                            }
                        }
                    }

                    if (true == isAdd)
                    {
                        if (0 < listNG.Count)
                        {
                            if (50 < (end - listNG[0]))
                                end = listNG[0] + 49;
                        }

                        if (end > start)
                        {
                            for (int i = start; i < end; ++i)
                            {
                                // Top 우선
                                if (true == dataService.DataSystem.IsSelectedTop)
                                {
                                    value1 = dataService.DataResult.Top.ListRaw[i].Value1;
                                    value2 = dataService.DataResult.Top.ListRaw[i].Value2;
                                    value3 = dataService.DataResult.Top.ListRaw[i].Value3;
                                }

                                if ("G" == value1)
                                    value1 = dataService.DataResult.Bottom.ListRaw[i].Value1;
                                if ("G" == value2)
                                    value2 = dataService.DataResult.Bottom.ListRaw[i].Value2;
                                if ("G" == value3)
                                    value3 = dataService.DataResult.Bottom.ListRaw[i].Value3;

                                if ("BB006" != value1 && "C" != value1)
                                    if ("BB039" == dataService.DataResult.Bottom.ListRaw[i].Value1)
                                        value1 = "BB039";
                                if ("BB006" != value2 && "C" != value2)
                                    if ("BB039" == dataService.DataResult.Bottom.ListRaw[i].Value2)
                                        value2 = "BB039";
                                if ("BB006" != value3 && "C" != value3)
                                    if ("BB039" == dataService.DataResult.Bottom.ListRaw[i].Value3)
                                        value3 = "BB039";

                                // Bottom 이 Joint 일 경우 Joint 로 등록
                                if ("BB006" == dataService.DataResult.Bottom.ListRaw[i].Value1)
                                    value1 = "BB006";
                                if ("BB006" == dataService.DataResult.Bottom.ListRaw[i].Value2)
                                    value2 = "BB006";
                                if ("BB006" == dataService.DataResult.Bottom.ListRaw[i].Value3)
                                    value3 = "BB006";

                                dataService.DataResult.AddTemp3(i, value1, value2, value3, "REVIEW");

                                if (("G" != value1) || ("G" != value2) || ("G" != value3))
                                    AddNG(i);
                            }

                            AddReviewData();
                            DisplayTemp();
                        }
                    }
                }
            }
            // Top 만 사용하는 경우
            else
            {
                start = dataService.DataResult.Temp.ListRaw.Count;
                end = dataService.DataResult.Top.ListRaw.Count;

                if (start < end)
                {
                    if (0 < listNG.Count)
                    {
                        if (50 < (end - listNG[0]))
                            end = listNG[0] + 49;
                    }

                    if (end > start)
                    {
                        for (int i = start; i < end; ++i)
                        {
                            value1 = dataService.DataResult.Top.ListRaw[i].Value1;
                            value2 = dataService.DataResult.Top.ListRaw[i].Value2;
                            value3 = dataService.DataResult.Top.ListRaw[i].Value3;

                            dataService.DataResult.AddTemp3(i, value1, value2, value3, "REVIEW");

                            if (("G" != value1) || ("G" != value2) || ("G" != value3))
                                AddNG(i);
                        }

                        AddReviewData();
                        DisplayTemp();
                    }
                }
            }
        }

        private void AddNG(int index)
        {
            // 수정을 하든 안 하든 NG 리스트에 저장 -> 타이머 에서 수정모드가 아닐 경우 자동으로 맨 앞의 NG 데이터를 삭제한다.
            //if (true == dataService.DataSystem.IsSelectedModify)
            {
                listNG.Add(index);

                Log_Debug.WriteLine("ReviewWindow.AddNG() : Index({0}), listNG.Count({1})", index, listNG.Count);

                SetNGData();
            }
        }

        private void AddReviewData()
        {
            //sysService.State = SystemService.States.stop;

            if (3 == dataService.DataRecipe.Line)
            {
                AddReviewData3();
                return;
            }

            int start = 0;
            int end = 0;

            string value1 = "G";
            string value2 = "G";

            if (0 < listNG.Count)
            {
                start = dataService.DataResult.Review.ListRaw.Count;
                //end = listNG.Count-1;     // 현재 NG 는 Modify 확인 후 저장한다.
                end = listNG[0];
            }
            else
            {
                start = dataService.DataResult.Review.ListRaw.Count;
                end = dataService.DataResult.Temp.ListRaw.Count;

                value1 = "G";
                value2 = "G";
            }


            //if (end > start)
            //////{
            //    //Log_Debug.WriteLine("ReviewWindow.AddReviewData() : start = {0}, end = {1}", start, end);
            //}

            for (int i = start; i < end; ++i)
            {
                value1 = dataService.DataResult.Temp.ListRaw[i].Value1;
                value2 = dataService.DataResult.Temp.ListRaw[i].Value2;

                dataService.DataResult.AddReview(i, value1, value2, "REVIEW");

                //System.Diagnostics.Debug.WriteLine(string.Format("Review Data {0:0000} = {1}, {2}", dataService.DataResult.Review.ListRaw.Count, value1, value2));
            }
        }

        private void AddReviewData3()
        {
            //sysService.State = SystemService.States.stop;
            
            int start = 0;
            int end = 0;

            string value1 = "G";
            string value2 = "G";
            string value3 = "G";

            if (0 < listNG.Count)
            {
                start = dataService.DataResult.Review.ListRaw.Count;
                //end = listNG.Count-1;     // 현재 NG 는 Modify 확인 후 저장한다.
                end = listNG[0];
            }
            else
            {
                start = dataService.DataResult.Review.ListRaw.Count;
                end = dataService.DataResult.Temp.ListRaw.Count;

                value1 = "G";
                value2 = "G";
                value3 = "G";
            }


            //if (end > start)
            //////{
            //    //Log_Debug.WriteLine("ReviewWindow.AddReviewData() : start = {0}, end = {1}", start, end);
            //}

            for (int i = start; i < end; ++i)
            {
                value1 = dataService.DataResult.Temp.ListRaw[i].Value1;
                value2 = dataService.DataResult.Temp.ListRaw[i].Value2;
                value3 = dataService.DataResult.Temp.ListRaw[i].Value3;

                dataService.DataResult.AddReview3(i, value1, value2, value3, "REVIEW");

                //System.Diagnostics.Debug.WriteLine(string.Format("Review Data {0:0000} = {1}, {2}", dataService.DataResult.Review.ListRaw.Count, value1, value2));
            }
        }

        private void DisplayTemp()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate 
                {
                    SetDisplayTemp();
                }));

            //if (InvokeRequired)
            //{
            //    OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            //                                (ThreadStart)delegate()
            //                                {
            //                                    SetDisplayTemp();
            //                                }
            //    );
            //}
            //else
            //{
            //    SetDisplayTemp();
            //}
        }

        private void SetDisplayTemp()
        {
            if (false == rdViewVerify.IsChecked)
                return;

            int length = dataService.DataResult.Temp.ListRaw.Count;
            if (50 > length)
            {
                for (int i = 0; i < length; ++i)
                {
                    DisplayTempLine(i, i + 1);

                    DisplayTempValue(i, i);
                }
                for (int i = length; i < 50; ++i)
                {
                    DisplayTempLine(i, i + 1);

                    lblLineAs[i].Content = "";
                    lblLineAs[i].Background = Brushes.Beige;

                    lblLineBs[i].Content = "";
                    lblLineBs[i].Background = Brushes.Beige;

                    lblLineCs[i].Content = "";
                    lblLineCs[i].Background = Brushes.Beige;
                }
            }
            else
            {
                int indexLabel;

                for (int i = length - 50; i < length; ++i)
                {
                    indexLabel = i - (length - 50);
                    DisplayTempLine(indexLabel, i + 1);
                    DisplayTempValue(indexLabel, i);
                }
            }
        }

        private void DisplayTempLine(int indexLabel, int indexValue)
        {
            tblLines[indexLabel].Text = indexValue.ToString();

            if (indexValue == IndexDisplay + 1)
            {
                lblLines[indexLabel].Background = Brushes.Red;
            }
            else
            {
                lblLines[indexLabel].Background = Brushes.AliceBlue;
            }
        }

        private void RedrawTempLine()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            {
                SetRedrawTempLine();
            }));
            //if (InvokeRequired)
            //{
            //    OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            //                                (ThreadStart)delegate()
            //                                {
            //                                    SetRedrawTempLine();
            //                                }
            //    );
            //}
            //else
            //{
            //    SetRedrawTempLine();
            //}
        }

        private void SetRedrawTempLine()
        {
            if (false == rdViewVerify.IsChecked)
                return;

            string ng = (IndexDisplay + 1).ToString();

            for (int i = 0; i < 50; ++i)
            {
                if (ng == tblLines[i].Text)
                {
                    lblLines[i].Background = Brushes.Red;
                }
                else
                {
                    lblLines[i].Background = Brushes.AliceBlue;
                }
            }
        }

        private void DisplayTempValue(int indexLabel, int indexValue)
        {
            if (3 == dataService.DataRecipe.Line)
            {
                DisplayTempValue3(indexLabel, indexValue);
                return;
            }

            // Line A
            lblLineAs[indexLabel].Content = dataService.DataResult.Temp.ListRaw[indexValue].Value1;

            if ("G" == dataService.DataResult.Temp.ListRaw[indexValue].Value1)
            {
                lblLineAs[indexLabel].Background = Brushes.Beige;
                lblLineAs[indexLabel].Foreground = Brushes.Black;
            }
            else
            {
                if (indexValue != IndexNG)
                {
                    lblLineAs[indexLabel].Background = Brushes.Beige;
                    lblLineAs[indexLabel].Foreground = Brushes.Red;
                }
                else
                {
                    if (true == IsModifyA)
                    {
                        lblLineAs[indexLabel].Background = Brushes.Beige;
                        lblLineAs[indexLabel].Foreground = Brushes.Red;
                    }
                    else
                    {
                        lblLineAs[indexLabel].Background = Brushes.Red;
                        lblLineAs[indexLabel].Foreground = Brushes.White;
                    }
                }
            }

            // Line B
            lblLineBs[indexLabel].Content = dataService.DataResult.Temp.ListRaw[indexValue].Value2;
            if ("G" == dataService.DataResult.Temp.ListRaw[indexValue].Value2)
            {
                lblLineBs[indexLabel].Background = Brushes.Beige;
                lblLineBs[indexLabel].Foreground = Brushes.Black;
            }
            else
            {
                if (indexValue != IndexNG)
                {
                    lblLineBs[indexLabel].Background = Brushes.Beige;
                    lblLineBs[indexLabel].Foreground = Brushes.Red;
                }
                else
                {
                    if (true == IsModifyB)
                    {
                        lblLineBs[indexLabel].Background = Brushes.Beige;
                        lblLineBs[indexLabel].Foreground = Brushes.Red;
                    }
                    else
                    {
                        if (true == IsModifyA)
                        {
                            lblLineBs[indexLabel].Background = Brushes.Red;
                            lblLineBs[indexLabel].Foreground = Brushes.White;
                        }
                        else
                        {
                            lblLineBs[indexLabel].Background = Brushes.Beige;
                            lblLineBs[indexLabel].Foreground = Brushes.Red;
                        }
                    }
                }
            }
        }

        private void DisplayTempValue3(int indexLabel, int indexValue)
        {

            // Line A
            lblLineAs[indexLabel].Content = dataService.DataResult.Temp.ListRaw[indexValue].Value1;

            if ("G" == dataService.DataResult.Temp.ListRaw[indexValue].Value1)
            {
                lblLineAs[indexLabel].Background = Brushes.Beige;
                lblLineAs[indexLabel].Foreground = Brushes.Black;
            }
            else
            {
                if (indexValue != IndexNG)
                {
                    lblLineAs[indexLabel].Background = Brushes.Beige;
                    lblLineAs[indexLabel].Foreground = Brushes.Red;
                }
                else
                {
                    if (true == IsModifyA)
                    {
                        lblLineAs[indexLabel].Background = Brushes.Beige;
                        lblLineAs[indexLabel].Foreground = Brushes.Red;
                    }
                    else
                    {
                        lblLineAs[indexLabel].Background = Brushes.Red;
                        lblLineAs[indexLabel].Foreground = Brushes.White;
                    }
                }
            }

            // Line B
            lblLineBs[indexLabel].Content = dataService.DataResult.Temp.ListRaw[indexValue].Value2;
            if ("G" == dataService.DataResult.Temp.ListRaw[indexValue].Value2)
            {
                lblLineBs[indexLabel].Background = Brushes.Beige;
                lblLineBs[indexLabel].Foreground = Brushes.Black;
            }
            else
            {
                if (indexValue != IndexNG)
                {
                    lblLineBs[indexLabel].Background = Brushes.Beige;
                    lblLineBs[indexLabel].Foreground = Brushes.Red;
                }
                else
                {
                    if (true == IsModifyB)
                    {
                        lblLineBs[indexLabel].Background = Brushes.Beige;
                        lblLineBs[indexLabel].Foreground = Brushes.Red;
                    }
                    else
                    {
                        if (true == IsModifyA)
                        {
                            lblLineBs[indexLabel].Background = Brushes.Red;
                            lblLineBs[indexLabel].Foreground = Brushes.White;
                        }
                        else
                        {
                            lblLineBs[indexLabel].Background = Brushes.Beige;
                            lblLineBs[indexLabel].Foreground = Brushes.Red;
                        }
                    }
                }
            }


            // Line C
            lblLineCs[indexLabel].Content = dataService.DataResult.Temp.ListRaw[indexValue].Value3;
            if ("G" == dataService.DataResult.Temp.ListRaw[indexValue].Value3)
            {
                lblLineCs[indexLabel].Background = Brushes.Beige;
                lblLineCs[indexLabel].Foreground = Brushes.Black;
            }
            else
            {
                if (indexValue != IndexNG)
                {
                    lblLineCs[indexLabel].Background = Brushes.Beige;
                    lblLineCs[indexLabel].Foreground = Brushes.Red;
                }
                else
                {
                    if (true == IsModifyC)
                    {
                        lblLineCs[indexLabel].Background = Brushes.Beige;
                        lblLineCs[indexLabel].Foreground = Brushes.Red;
                    }
                    else
                    {
                        if (true == IsModifyB)
                        {
                            lblLineCs[indexLabel].Background = Brushes.Red;
                            lblLineCs[indexLabel].Foreground = Brushes.White;
                        }
                        else
                        {
                            lblLineCs[indexLabel].Background = Brushes.Beige;
                            lblLineCs[indexLabel].Foreground = Brushes.Red;
                        }
                    }
                }
            }
        }

        private void SetNGData()
        {
            if (3 == dataService.DataRecipe.Line)
            {
                SetNGData3();
                return;
            }

            if (0 < listNG.Count)
            {
                if (IndexNG != listNG[0])
                {
                    IndexNG = listNG[0];

                    Log_Debug.WriteLine("ReviewWindow.SetNGData() : IndexNG({0})", IndexNG);

                    RedrawTempLine();

                    //System.Diagnostics.Debug.WriteLine("SetNGData {0}", IndexNG);

                    if ("G" != dataService.DataResult.Temp.ListRaw[IndexNG].Value1)
                    {
                        IsModifyA = false;
                        ModifyValue1 = 1;

                        IsDisplayedA = false;
                        TempValue1 = dataService.DataResult.Temp.ListRaw[IndexNG].Value1;
                        
                        //System.Diagnostics.Debug.WriteLine("NG Detect {0} A", IndexNG);

                        Log_Debug.WriteLine("NG Detect {0} A", IndexNG);
                    }
                    else
                    {
                        IsModifyA = true;
                        ModifyValue1 = 0;
                        TempValue1 = "G";

                        IsDisplayedA = true;

                        //Log_Debug.WriteLine("ReviewWindow.SetNGData().TempValue1 = {0}", TempValue1);                                                
                    }


                    if ("G" != dataService.DataResult.Temp.ListRaw[IndexNG].Value2)
                    {
                        IsModifyB = false;
                        ModifyValue2 = 1;

                        IsDisplayedB = false;
                        TempValue2 = dataService.DataResult.Temp.ListRaw[IndexNG].Value2;
                        //System.Diagnostics.Debug.WriteLine("NG Detect {0} B", IndexNG);

                        Log_Debug.WriteLine("NG Detect {0} B", IndexNG);
                    }
                    else
                    {
                        IsModifyB = true;
                        ModifyValue2 = 0;
                        TempValue2 = "G";

                        IsDisplayedB = true;

                        //Log_Debug.WriteLine("ReviewWindow.SetNGData().TempValue2 = {0}", TempValue2);                        
                    }


                    //System.Diagnostics.Debug.WriteLine("DisplayNGImage listNG={0}", listNG.Count);
                    DisplayNGImage();
                    //System.Diagnostics.Debug.WriteLine("DisplayNGImage...Done listNG={0}", listNG.Count);
                }
            }
        }

        private void SetNGData3()
        {
            if (0 < listNG.Count)
            {
                if (IndexNG != listNG[0])
                {
                    IndexNG = listNG[0];

                    RedrawTempLine();

                    if ("G" != dataService.DataResult.Temp.ListRaw[IndexNG].Value1)
                    {
                        IsModifyA = false;
                        ModifyValue1 = 1;

                        IsDisplayedA = false;
                        TempValue1 = dataService.DataResult.Temp.ListRaw[IndexNG].Value1;
                    }
                    else
                    {
                        IsModifyA = true;
                        ModifyValue1 = 0;
                        TempValue1 = "G";

                        IsDisplayedA = true;
                    }


                    if ("G" != dataService.DataResult.Temp.ListRaw[IndexNG].Value2)
                    {
                        IsModifyB = false;
                        ModifyValue2 = 1;

                        IsDisplayedB = false;
                        TempValue2 = dataService.DataResult.Temp.ListRaw[IndexNG].Value2;
                    }
                    else
                    {
                        IsModifyB = true;
                        ModifyValue2 = 0;
                        TempValue2 = "G";

                        IsDisplayedB = true;
                    }


                    if ("G" != dataService.DataResult.Temp.ListRaw[IndexNG].Value3)
                    {
                        IsModifyC = false;
                        ModifyValue3 = 1;

                        IsDisplayedC = false;
                        TempValue3 = dataService.DataResult.Temp.ListRaw[IndexNG].Value3;
                    }
                    else
                    {
                        IsModifyC = true;
                        ModifyValue3 = 0;
                        TempValue3 = "G";

                        IsDisplayedC = true;
                    }

                    DisplayNGImage();
                }
            }
        }

        private void DisplayNGImage()
        {
            
            DateTime start = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 5000);
            DateTime timeout = start.Add(duration);

            int delay = 10;
            while (true == IsDrawing)
            {
                ThreadDoEvents(10);
                delay = 100;

                if (timeout < DateTime.Now)
                    break;
            }

            IsDrawing = true;
            ThreadDoEvents(delay);


            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            {
                SetDisplayNGImage();
            }));

            //if (InvokeRequired)
            //{
            //    OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            //                                (ThreadStart)delegate()
            //                                {
            //                                    SetDisplayNGImage();
            //                                }
            //    );
            //}
            //else
            //{
            //    SetDisplayNGImage();
            //}
        }

        private void SetDisplayNGImage()
        {
            if (3 == dataService.DataRecipe.Line)
            {
                SetDisplayNGImage3();
                return;
            }

            //if (false == rdViewVerify.IsChecked)
            //    return;

            //System.Diagnostics.Debug.WriteLine("SetDisplayNGImage() listNG={0}, A={1}, B={2}", listNG.Count, IsModifyA, IsModifyB);

            if (0 < listNG.Count)
            {
                IsDisplayDone = false;
                if (true == rdViewVerify.IsChecked)
                {
                    ClearImages();

                    DateTime start = DateTime.Now;
                    TimeSpan duration = new TimeSpan(0, 0, 0, 0, 500);
                    DateTime timeout = start.Add(duration);
                    
                    while (false == IsClearedImages)
                    {
                        if (DateTime.Now > timeout)
                            break;
                        System.Windows.Forms.Application.DoEvents();
                        Thread.Sleep(1);
                    }
                }
                
                //System.Diagnostics.Debug.WriteLine("SetDisplayNGImage..Done");

                lbxTop.Visibility = System.Windows.Visibility.Hidden;
                lbxBottom.Visibility = System.Windows.Visibility.Hidden;
                lbxMono.Visibility = System.Windows.Visibility.Hidden;

                if (false == IsModifyA)
                {
                    //System.Diagnostics.Debug.WriteLine("Display Image A {0}", IndexNG);
                    //return;

                    lbLine.Content = string.Format("{0}-{1}", IndexNG + 1, "A");

                    IndexDisplay = IndexNG;

                    //System.Diagnostics.Debug.WriteLine("Display Image A {0}", IndexNG);

                    //if ("V" != TempValue1) // Joint 도 이미지 있음.
                    {
                        //if (true == rdViewVerify.IsChecked)
                        //{
                            DisplayNGImageTop(IndexNG, "A");
                            DisplayNGImageBottom(IndexNG, "A");
                            DisplayNGImageMono(IndexNG, "A");
                        //}
                    }
                    IsDisplayedA = true;
                    //System.Diagnostics.Debug.WriteLine("Display Image A {0} Done", IndexNG);

                    //Log_Debug.WriteLine("ReviewWindow.SetDisplayNGImage() : Image A {0}", IndexNG);
                }
                else if (false == IsModifyB)
                {
                    //System.Diagnostics.Debug.WriteLine("Display Image B {0}", IndexNG);
                    ///return;

                    lbLine.Content = string.Format("{0}-{1}", IndexNG + 1, "B");

                    IndexDisplay = IndexNG;

                    ////System.Diagnostics.Debug.WriteLine("Display Image B {0}", IndexNG);

                    //if ("V" != TempValue2)  // Joint 도 이미지 있음.
                    {
                        DisplayNGImageTop(IndexNG, "B");
                        DisplayNGImageBottom(IndexNG, "B");
                        DisplayNGImageMono(IndexNG, "B");
                    }
                    IsDisplayedB = true;
                    //System.Diagnostics.Debug.WriteLine("Display Image B {0} Done", IndexNG);

                    //Log_Debug.WriteLine("ReviewWindow.SetDisplayNGImage() : Image B {0}", IndexNG);
                }

                if (0 < lbxTop.Items.Count)
                {
                    lbxTop.Focus();
                }
                else if (0 < lbxBottom.Items.Count)
                    lbxBottom.Focus();
                else if (0 < lbxMono.Items.Count)
                    lbxMono.Focus();


                lbxTop.Visibility = System.Windows.Visibility.Visible;
                lbxBottom.Visibility = System.Windows.Visibility.Visible;
                lbxMono.Visibility = System.Windows.Visibility.Visible;

                IsDisplayDone = true;
            }

            IsDrawing = false;
        }

        private void SetDisplayNGImage3()
        {
            if (0 < listNG.Count)
            {
                IsDisplayDone = false;
                if (true == rdViewVerify.IsChecked)
                {
                    ClearImages();

                    DateTime start = DateTime.Now;
                    TimeSpan duration = new TimeSpan(0, 0, 0, 0, 500);
                    DateTime timeout = start.Add(duration);

                    while (false == IsClearedImages)
                    {
                        if (DateTime.Now > timeout)
                            break;
                        System.Windows.Forms.Application.DoEvents();
                        Thread.Sleep(1);
                    }
                }

                //System.Diagnostics.Debug.WriteLine("SetDisplayNGImage..Done");

                lbxTop.Visibility = System.Windows.Visibility.Hidden;
                lbxBottom.Visibility = System.Windows.Visibility.Hidden;
                lbxMono.Visibility = System.Windows.Visibility.Hidden;

                if (false == IsModifyA)
                {
                    lbLine.Content = string.Format("{0}-{1}", IndexNG + 1, "A");

                    IndexDisplay = IndexNG;

                    //System.Diagnostics.Debug.WriteLine("Display Image A {0}", IndexNG);

                    //if ("V" != TempValue1) // Joint 도 이미지 있음.
                    {
                        //if (true == rdViewVerify.IsChecked)
                        //{
                        DisplayNGImageTop(IndexNG, "A");
                        DisplayNGImageBottom(IndexNG, "A");
                        DisplayNGImageMono(IndexNG, "A");
                        //}
                    }
                    IsDisplayedA = true;
                    //System.Diagnostics.Debug.WriteLine("Display Image A {0} Done", IndexNG);

                    //Log_Debug.WriteLine("ReviewWindow.SetDisplayNGImage() : Image A {0}", IndexNG);
                }
                else if (false == IsModifyB)
                {
                    //System.Diagnostics.Debug.WriteLine("Display Image B {0}", IndexNG);
                    ///return;

                    lbLine.Content = string.Format("{0}-{1}", IndexNG + 1, "B");

                    IndexDisplay = IndexNG;

                    ////System.Diagnostics.Debug.WriteLine("Display Image B {0}", IndexNG);

                    //if ("V" != TempValue2)  // Joint 도 이미지 있음.
                    {
                        DisplayNGImageTop(IndexNG, "B");
                        DisplayNGImageBottom(IndexNG, "B");
                        DisplayNGImageMono(IndexNG, "B");
                    }
                    IsDisplayedB = true;
                    //System.Diagnostics.Debug.WriteLine("Display Image B {0} Done", IndexNG);

                    //Log_Debug.WriteLine("ReviewWindow.SetDisplayNGImage() : Image B {0}", IndexNG);
                }
                else if (false == IsModifyC)
                {
                    lbLine.Content = string.Format("{0}-{1}", IndexNG + 1, "C");

                    IndexDisplay = IndexNG;

                    ////System.Diagnostics.Debug.WriteLine("Display Image B {0}", IndexNG);

                    //if ("V" != TempValue2)  // Joint 도 이미지 있음.
                    {
                        DisplayNGImageTop(IndexNG, "C");
                        DisplayNGImageBottom(IndexNG, "C");
                        DisplayNGImageMono(IndexNG, "C");
                    }
                    IsDisplayedC = true;
                    //System.Diagnostics.Debug.WriteLine("Display Image B {0} Done", IndexNG);

                    //Log_Debug.WriteLine("ReviewWindow.SetDisplayNGImage() : Image B {0}", IndexNG);
                }

                if (0 < lbxTop.Items.Count)
                {
                    lbxTop.Focus();
                }
                else if (0 < lbxBottom.Items.Count)
                    lbxBottom.Focus();
                else if (0 < lbxMono.Items.Count)
                    lbxMono.Focus();


                lbxTop.Visibility = System.Windows.Visibility.Visible;
                lbxBottom.Visibility = System.Windows.Visibility.Visible;
                lbxMono.Visibility = System.Windows.Visibility.Visible;

                IsDisplayDone = true;
            }

            IsDrawing = false;
        }

        private void DisplayNGImageTop(int index, string type, int viewType=0)
        {
            if( index > dataService.DataResult.Top.ListRaw.Count-1 )
                return;
            
            if ("A" == type)
            {
                if ("G" != dataService.DataResult.Top.ListRaw[index].Value1)
                {
                    string fileName = string.Format("index{0:000000}_A", index+1);
                    string path;
                    // Top1
                    if ("TOP1" == dataService.DataResult.Top.ListRaw[index].Type)
                    {
                        path = dataService.DataSystem.Top1Path;
                    }
                    // Top2
                    else if ("TOP2" == dataService.DataResult.Top.ListRaw[index].Type)
                    {
                        path = dataService.DataSystem.Top2Path;
                    }
                    else
                        return;

                    DisplayNGImageListBox(index + 1, "TOP", type, path, fileName, lbxTop, true, viewType);
                }
            }
            else if ("B" == type)
            {
                if ("G" != dataService.DataResult.Top.ListRaw[index].Value2)
                {
                    string fileName = string.Format("index{0:000000}_B", index + 1);
                    string path;
                    // Top1
                    if ("TOP1" == dataService.DataResult.Top.ListRaw[index].Type)
                    {
                        path = dataService.DataSystem.Top1Path;
                    }
                    // Top2
                    else if ("TOP2" == dataService.DataResult.Top.ListRaw[index].Type)
                    {
                        path = dataService.DataSystem.Top2Path;
                    }
                    else
                        return;

                    DisplayNGImageListBox(index + 1, "TOP", type, path, fileName, lbxTop, true, viewType);
                }
            }
            else if ("C" == type)
            {
                if ("G" != dataService.DataResult.Top.ListRaw[index].Value3)
                {
                    string fileName = string.Format("index{0:000000}_C", index + 1);
                    string path;
                    // Top1
                    if ("TOP1" == dataService.DataResult.Top.ListRaw[index].Type)
                    {
                        path = dataService.DataSystem.Top1Path;
                    }
                    // Top2
                    else if ("TOP2" == dataService.DataResult.Top.ListRaw[index].Type)
                    {
                        path = dataService.DataSystem.Top2Path;
                    }
                    else
                        return;

                    DisplayNGImageListBox(index + 1, "TOP", type, path, fileName, lbxTop, true, viewType);
                }
            }
            
        }

        private void DisplayNGImageBottom(int index, string type, int viewType = 0)
        {
            if( index > dataService.DataResult.Bottom.ListRaw.Count-1 )
                return;


            if ("A" == type)
            {
                if ("G" != dataService.DataResult.Bottom.ListRaw[index].Value1)
                {
                    string fileName = string.Format("index{0:000000}_A", index+1);
                    string path;
                    // Bottom1
                    if ("BOTTOM1" == dataService.DataResult.Bottom.ListRaw[index].Type)
                    {
                        path = dataService.DataSystem.Bottom1Path;
                    }
                    // Bottom2
                    else if ("BOTTOM2" == dataService.DataResult.Bottom.ListRaw[index].Type)
                    {
                        path = dataService.DataSystem.Bottom2Path;
                    }
                    else
                        return;

                    DisplayNGImageListBox(index + 1, "BOTTOM", type, path, fileName, lbxBottom, true, viewType);
                }
            }
            else if ("B" == type)
            {
                if ("G" != dataService.DataResult.Bottom.ListRaw[index].Value2)
                {
                    string fileName = string.Format("index{0:000000}_B", index + 1);
                    string path;
                    // Bottom1
                    if ("BOTTOM1" == dataService.DataResult.Bottom.ListRaw[index].Type)
                    {
                        path = dataService.DataSystem.Bottom1Path;
                    }
                    // Bottom2
                    else if ("BOTTOM2" == dataService.DataResult.Bottom.ListRaw[index].Type)
                    {
                        path = dataService.DataSystem.Bottom2Path;
                    }
                    else
                        return;

                    DisplayNGImageListBox(index + 1, "BOTTOM", type, path, fileName, lbxBottom, true, viewType);
                }
            }
            else if ("C" == type)
            {
                if ("G" != dataService.DataResult.Bottom.ListRaw[index].Value3)
                {
                    string fileName = string.Format("index{0:000000}_C", index + 1);
                    string path;
                    // Bottom1
                    if ("BOTTOM1" == dataService.DataResult.Bottom.ListRaw[index].Type)
                    {
                        path = dataService.DataSystem.Bottom1Path;
                    }
                    // Bottom2
                    else if ("BOTTOM2" == dataService.DataResult.Bottom.ListRaw[index].Type)
                    {
                        path = dataService.DataSystem.Bottom2Path;
                    }
                    else
                        return;

                    DisplayNGImageListBox(index + 1, "BOTTOM", type, path, fileName, lbxBottom, true, viewType);
                }
            }
        }

        private void DisplayNGImageMono(int index, string type, int viewType = 0)
        {
            if( index > dataService.DataResult.Mono.ListRaw.Count-1 )
                return;

            if ("A" == type)
            {
                if ("G" != dataService.DataResult.Mono.ListRaw[index].Value1)
                {
                    string fileName = string.Format("index{0:000000}_A", index+1);
                    string path;
                    // Mono1
                    if ("MONO1" == dataService.DataResult.Mono.ListRaw[index].Type)
                    {
                        path = dataService.DataSystem.Mono1Path;
                    }
                    // Mono2
                    else if ("MONO2" == dataService.DataResult.Mono.ListRaw[index].Type)
                    {
                        path = dataService.DataSystem.Mono2Path;
                    }
                    else
                        return;

                    DisplayNGImageListBox(index + 1, "MONO", type, path, fileName, lbxMono, false, viewType);
                }
            }
            else if ("B" == type)
            {
                if ("G" != dataService.DataResult.Mono.ListRaw[index].Value2)
                {
                    string fileName = string.Format("index{0:000000}_B", index + 1);
                    string path;
                    // Mono1
                    if ("MONO1" == dataService.DataResult.Mono.ListRaw[index].Type)
                    {
                        path = dataService.DataSystem.Mono1Path;
                    }
                    // Mono2
                    else if ("MONO2" == dataService.DataResult.Mono.ListRaw[index].Type)
                    {
                        path = dataService.DataSystem.Mono2Path;
                    }
                    else
                        return;

                    DisplayNGImageListBox(index + 1, "MONO", type, path, fileName, lbxMono, false, viewType);
                }
            }
            else if ("C" == type)
            {
                if ("G" != dataService.DataResult.Mono.ListRaw[index].Value3)
                {
                    string fileName = string.Format("index{0:000000}_C", index + 1);
                    string path;
                    // Mono1
                    if ("MONO1" == dataService.DataResult.Mono.ListRaw[index].Type)
                    {
                        path = dataService.DataSystem.Mono1Path;
                    }
                    // Mono2
                    else if ("MONO2" == dataService.DataResult.Mono.ListRaw[index].Type)
                    {
                        path = dataService.DataSystem.Mono2Path;
                    }
                    else
                        return;

                    DisplayNGImageListBox(index + 1, "MONO", type, path, fileName, lbxMono, false, viewType);
                }
            }
        }

        private void DisplayNGImageListBox(int index, string visionType, string lineType, string path, string fileName, ListBox listBox, bool isColor = false, int viewType = 0)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            {
                SetDisplayNGImageListBox(index, visionType, lineType, path, fileName, listBox, isColor, viewType);
            }));

            //if (InvokeRequired)
            //{
            //    OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            //                                (ThreadStart)delegate()
            //                                {
            //                                    SetDisplayNGImageListBox(index, visionType, lineType, path, fileName, listBox, isColor, viewType);
            //                                }
            //    );
            //}
            //else
            //{
            //    SetDisplayNGImageListBox(index, visionType, lineType, path, fileName, listBox, isColor, viewType);
            //}
        }

        private void SetDisplayNGImageListBox(int index, string visionType, string lineType, string path, string fileName, ListBox listBox, bool isColor = false, int viewType = 0)
        {
            int pathLength = path.Length;

            if ('\\' != path[pathLength - 1])
                path += "\\";

            if ("" == dataService.DataResult.LotID)
                return;

            string pathName = path + dataService.DataResult.LotID + "\\" + fileName + ".dat";
            string pathImage = path + dataService.DataResult.LotID;
            FileStream stream;
            StreamReader reader;
            string item;

            //System.Diagnostics.Debug.WriteLine(pathName);

            DateTime start = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, timeoutDat);
            DateTime timeout = start.Add(duration);

            // .dat 파일이 존재할 때 까지 10초 대기 -> 무한정 대기
            while( false == File.Exists(pathName) )
            {
                if (false == IsThreadDisplay)
                    return;

                if (timeout < DateTime.Now)
                {
                    Log_Trace.WriteLine("SetDisplayNGImageListBox() Fail : " + pathName);
                    break;
                }

                switch (viewType)
                {
                        // Verify 는 무조건 진행
                    //case 0:     // Verify
                    //    //if (false == rdViewVerify.IsChecked)
                    //    //    return;
                    //    break;
                    case 1:     // Punch
                        if (false == rdViewPunch.IsChecked)
                            return;
                        break;  
                    case 2:     // Select
                        if (false == rdViewSelect.IsChecked)
                            return;
                        break;
                    default:
                        break;
                }

                System.Windows.Forms.Application.DoEvents();
                Thread.Sleep(1);

                if (true == dataService.IsBackFeeding)
                    return;
            } 


            try
            {
                stream = File.OpenRead(pathName);
                reader = new StreamReader(stream, Encoding.Default);

                string[] arrays = new string[10];
                string defectID = "";
                string defectName = "";
                string area = "";
                string length = "";
                string sizeX = "";
                string sizeY = "";
                string posX = "";
                string posY = "";

                string pathColor;
                string pathHSI;
                string pathBig;

                while (true)
                {
                    // defectID,defectName,colorImage,hsiImage,bigImage;
                    item = reader.ReadLine();

                    //System.Diagnostics.Debug.WriteLine(item);

                    if ("" == item)
                        item = reader.ReadLine();

                    if (null == item)
                        break;

                    if (("EOF" == item) || "" == item)
                        break;
                    else
                    {
                        arrays = item.Split(',');

                        if (10 < arrays.Length)
                        {
                            defectID = arrays[0];
                            defectName = arrays[1];
                            area = arrays[2];
                            length = arrays[3];
                            sizeX = arrays[4];
                            sizeY = arrays[5];
                            posX = arrays[6];
                            posY = arrays[7];

                            // Verify View 이면 Defect 데이터 추가
                            if (0 == viewType)
                            {
                                dataService.DataDefect.Add(index, pathImage, visionType, lineType, defectID, defectName, double.Parse(area), double.Parse(length), double.Parse(sizeX), double.Parse(sizeY),
                                    double.Parse(posX), double.Parse(posY), arrays[8], arrays[9], arrays[10]);
                            }

                            // View Type 이 베리파이이고, 베리파이 뷰 선택이 아닐경우 Skip 한다.
                            if ((0 == viewType) && (false == rdViewVerify.IsChecked))
                            {
                                ;
                            }
                            else
                            {
                                ReviewImageControl img = new ReviewImageControl();

                                img.Margin = ImageMargin;

                                if (true == isColor)
                                    img.Columns = ImageColumns;

                                img.IsVisibleHSI = isColor;

                                // Color Image
                                if ("" != arrays[8])
                                {
                                    img.ColorName = arrays[8];
                                    pathColor = pathImage + "\\" + arrays[8];

                                    start = DateTime.Now;
                                    duration = new TimeSpan(0, 0, 0, 0, timeoutImage);
                                    timeout = start.Add(duration);

                                    while (false == File.Exists(pathColor))
                                    {
                                        if (false == IsThreadDisplay)
                                            return;

                                        if (true == dataService.IsBackFeeding)
                                            return;

                                        System.Windows.Forms.Application.DoEvents();

                                        if (timeout < DateTime.Now)
                                            break;

                                        Thread.Sleep(1);
                                    }

                                    img.PathColor = pathColor;
                                    System.Windows.Forms.Application.DoEvents();
                                }
                                // HSI Image
                                if (true == isColor)
                                {
                                    if ("" != arrays[9])
                                    {
                                        img.HSIName = arrays[9];

                                        pathHSI = pathImage + "\\" + arrays[9];

                                        start = DateTime.Now;
                                        duration = new TimeSpan(0, 0, 0, 0, timeoutImage);
                                        timeout = start.Add(duration);

                                        while (false == File.Exists(pathHSI))
                                        {
                                            if (false == IsThreadDisplay)
                                                return;

                                            if (true == dataService.IsBackFeeding)
                                                return;

                                            System.Windows.Forms.Application.DoEvents();

                                            if (timeout < DateTime.Now)
                                                break;

                                            Thread.Sleep(1);
                                        }

                                        img.PathHSI = pathHSI;
                                        System.Windows.Forms.Application.DoEvents();
                                    }
                                }


                                img.Title = defectName;
                                img.ImageWidth = this.ImageWidth;
                                img.ImageHeight = this.ImageHeight;

                                listBox.Items.Add(img);


                                // Big Image
                                if ("" != arrays[10])
                                {
                                    ReviewImageControl imgBig = new ReviewImageControl();
                                    imgBig.Margin = ImageMargin;

                                    imgBig.IsVisibleHSI = false;
                                    imgBig.Title = defectName;
                                    if (0 == ImageColumns)
                                    {
                                        imgBig.ImageWidth = this.ImageWidth * 2;
                                        imgBig.ImageHeight = this.ImageHeight * 2;
                                    }
                                    else
                                    {
                                        imgBig.ImageWidth = this.ImageWidth;
                                        imgBig.ImageHeight = this.ImageHeight;
                                    }



                                    pathBig = pathImage + "\\" + arrays[10];

                                    start = DateTime.Now;
                                    duration = new TimeSpan(0, 0, 0, 0, timeoutImage);
                                    timeout = start.Add(duration);

                                    while (false == File.Exists(pathBig))
                                    {
                                        if (false == IsThreadDisplay)
                                            return;

                                        if (true == dataService.IsBackFeeding)
                                            return;

                                        System.Windows.Forms.Application.DoEvents();

                                        if (timeout < DateTime.Now)
                                            break;

                                        Thread.Sleep(1);
                                    }

                                    imgBig.ColorName = arrays[10];
                                    imgBig.PathColor = pathBig;

                                    listBox.Items.Add(imgBig);
                                    System.Windows.Forms.Application.DoEvents();
                                }
                            }
                        }
                        else
                        {
                            Log_Trace.WriteLine("ReviewWidnow.DisplayNGImageListBox() Length Error : (10 < arrays.Length({0}) : ", arrays.Length);
                        }
                    }
                }//while (true)
                reader.Close();
                stream.Close();

                if (0 < listBox.Items.Count)
                    listBox.SelectedIndex = 0;

                System.Windows.Forms.Application.DoEvents();

                //listBox.Items.Clear();
                //dataService.DataDefect.Save(dataService.DataSystem.ServerPath);
            }
            catch (Exception exc)
            {
                // Image 데이터 파일이 없을 경우
                Log_Exception.WriteLine("ReviewWidnow.DisplayNGImageListBox() : " + exc.Message);

                ReviewImageControl img = new ReviewImageControl();

                img.Margin = ImageMargin;

                img.IsVisibleHSI = isColor;

                img.Title = "NO IMAGE";
                img.ImageWidth = this.ImageWidth;
                img.ImageHeight = this.ImageHeight;

                listBox.Items.Add(img);
            }
        }

        private void DisplayResults(bool bGood)
        {
            m_bStop_KeyIn = true;

            // 리스트 뷰 이미지 제거
            ClearImages();

            DateTime start = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 500);
            DateTime timeout = start.Add(duration);

            while (false == IsClearedImages)
            {
                if (DateTime.Now > timeout)
                    break;
               System.Windows.Forms.Application.DoEvents();
                Thread.Sleep(1);
            }

            if (bGood)
            {
                lblJudgeResult.Content = "Good";
                lblJudgeResult.Foreground = Brushes.Blue;
                Log_Review.WriteLine("{0} = GOOD", IndexNG + 1);
            }
            else
            {
                lblJudgeResult.Content = "NG";
                lblJudgeResult.Foreground = Brushes.Red;
                Log_Review.WriteLine("{0} = NG", IndexNG + 1);
            }

            lblJudgeResult.Visibility = Visibility.Visible;

            // 0.5초간 결과를 표시
            start = DateTime.Now;
            duration = new TimeSpan(0, 0, 0, 0, 500);
            timeout = start.Add(duration);

            while (true)
            {
                if (DateTime.Now > timeout)
                    break;
                System.Windows.Forms.Application.DoEvents();
                Thread.Sleep(1);
            }

            lblJudgeResult.Visibility = Visibility.Collapsed;

            m_bStop_KeyIn = false;
        }

        private void Modify(int modify)
        {
            if (3 == dataService.DataRecipe.Line)
            {
                Modify3(modify);
                return;
            }

            if (false == dataService.DataSystem.IsSelectedModify)
                return;

            if (false == rdViewVerify.IsChecked)
                return;

            if (false == IsDisplayDone)
                return;

            if ( (modify > 9) || (modify < 0) )
                modify = 0;

            if (true == IsAutoModify)
            {
                Log_Debug.WriteLine("Manual Modify Code({0})", modify);
                IsAutoModify = false;
            }

            string modifyValue = "G";

            //if (0 != modify)
            //    modifyValue = modify.ToString();
            if (0 != modify)
                if( modify < dataService.DataSystem.verifyCodes.Length )
                    modifyValue = dataService.DataSystem.verifyCodes[modify];

            Log_Trace.WriteLine( string.Format("Modify Index = {0}", IndexNG));

            // 전체 판정
            if (0 == cbVerifyMethod.SelectedIndex)
            {
                Log_Trace.WriteLine(string.Format("Modify listNG.Count = {0}", listNG.Count));

                if (0 < listNG.Count)
                {
                    if ((false == IsModifyA) || (false == IsModifyB))
                    {
                        if (false == IsModifyA && true == IsDisplayedA)
                        {
                            ModifyValue1 = modify;
                            IsModifyA = true;

                            if (!dataService.DataSystem.IsUseVerifyCodes && modifyValue != "G")
                            {
                                modifyValue = TempValue1;
                            }

                            dataService.DataResult.Modify1(IndexNG, modifyValue);

                            dataService.DataDefect.Verify(IndexNG, "A", modifyValue);
                            
                            if( "G" != modifyValue )
                                AddNGListView(++CountNGListView, IndexNG + 1, "A", modifyValue);

                            //DisplayResults("G" == modifyValue);

                            // SetNGData() 에서는 A/B 중 하나만 보여줌으로, A 변경 시 B 를 Display 하기위해 호출해 준다.
                            if (false == IsModifyB)
                                DisplayNGImage();
                        }
                        else if (false == IsModifyB && true == IsDisplayedB)
                        {
                            ModifyValue2 = modify;
                            IsModifyB = true;

                            if (!dataService.DataSystem.IsUseVerifyCodes && modifyValue != "G")
                            {
                                modifyValue = TempValue2;
                            }

                            dataService.DataResult.Modify2(IndexNG, modifyValue);
                            dataService.DataDefect.Verify(IndexNG, "B", modifyValue);

                            if ("G" != modifyValue)
                                AddNGListView(++CountNGListView, IndexNG + 1, "B", modifyValue);

                            //DisplayResults("G" == modifyValue);
                        }

                        if ((true == IsModifyA) && (true == IsModifyB))
                        {
                            if( 0 < listNG.Count )
                                listNG.RemoveAt(0);

                            //dataService.DataResult.AddReview(ModifyValue1, ModifyValue2);  // Timer 에서 자동으로 해줌.
                            SetNGData();
                        }
                    }

                    AddReviewData();
                    DisplayTemp();
                }
            }
            // 이미지별 개별 판정.
            else
            {
                // Top
                if (0 == SelectedListBox)
                {
                    ReviewImageControl imgCtrl = (ReviewImageControl)lbxTop.SelectedItem;

                    if (null == imgCtrl)
                        return;

                    // End 처리
                    if ("G" != modifyValue)
                    {
                        if ((false == IsModifyA) || (false == IsModifyB))
                        {
                            if (false == IsModifyA)
                            {
                                ModifyValue1 = modify;
                                IsModifyA = true;

                                dataService.DataResult.Modify1(IndexNG, modifyValue);

                                dataService.DataDefect.VerifyImage(IndexNG, "A", imgCtrl.ColorName, modifyValue);

                                if ("G" != modifyValue)
                                    AddNGListView(++CountNGListView, IndexNG + 1, "A", modifyValue);

                                // SetNGData() 에서는 A/B 중 하나만 보여줌으로, A 변경 시 B 를 Display 하기위해 호출해 준다.
                                if (false == IsModifyB)
                                    DisplayNGImage();
                            }
                            else if (false == IsModifyB)
                            {
                                ModifyValue2 = modify;
                                IsModifyB = true;

                                dataService.DataResult.Modify2(IndexNG, modifyValue);
                                dataService.DataDefect.VerifyImage(IndexNG, "B", imgCtrl.ColorName, modifyValue);

                                if ("G" != modifyValue)
                                    AddNGListView(++CountNGListView, IndexNG + 1, "B", modifyValue);
                            }

                            if ((true == IsModifyA) && (true == IsModifyB))
                            {
                                listNG.RemoveAt(0);
                                //DisplayResults("G" == modifyValue);

                                //dataService.DataResult.AddReview(ModifyValue1, ModifyValue2);  // Timer 에서 자동으로 해줌.
                                SetNGData();
                            }
                        }
                    }
                    else    // Good 처리
                    {
                        if (false == IsModifyA)
                        {
                            dataService.DataDefect.VerifyImage(IndexNG, "A", imgCtrl.ColorName, modifyValue);
                        }
                        else if (false == IsModifyB)
                        {
                            dataService.DataDefect.VerifyImage(IndexNG, "B", imgCtrl.ColorName, modifyValue);
                        }


                        if (lbxTop.SelectedIndex == lbxTop.Items.Count - 1)
                        {
                            if (0 < lbxBottom.Items.Count)
                            {
                                lbxBottom.SelectedIndex = 0;
                                lbxBottom.Focus();
                            }
                            else if (0 < lbxMono.Items.Count)
                            {
                                lbxMono.SelectedIndex = 0;
                                lbxMono.Focus();
                            }
                            else // End 처리
                            {
                                if (false == IsModifyA)
                                {
                                    ModifyValue1 = modify;
                                    IsModifyA = true;

                                    dataService.DataResult.Modify1(IndexNG, modifyValue);
                                }
                                else if (false == IsModifyB)
                                {
                                    ModifyValue2 = modify;
                                    IsModifyB = true;

                                    dataService.DataResult.Modify2(IndexNG, modifyValue);
                                }
                                if ((true == IsModifyA) && (true == IsModifyB))
                                {
                                    listNG.RemoveAt(0);
                                    //DisplayResults("G" == modifyValue);

                                    //dataService.DataResult.AddReview(ModifyValue1, ModifyValue2);  // Timer 에서 자동으로 해줌.
                                    SetNGData();
                                }
                            }
                        }
                        else
                        {
                            lbxTop.SelectedIndex = lbxTop.SelectedIndex + 1;
                        }
                    }
                    
                }
                else if (1 == SelectedListBox)
                {
                    ReviewImageControl imgCtrl = (ReviewImageControl)lbxBottom.SelectedItem;

                    if (null == imgCtrl)
                        return;

                    // End 처리
                    if ("G" != modifyValue)
                    {
                        if ((false == IsModifyA) || (false == IsModifyB))
                        {
                            if (false == IsModifyA)
                            {
                                ModifyValue1 = modify;
                                IsModifyA = true;

                                dataService.DataResult.Modify1(IndexNG, modifyValue);

                                dataService.DataDefect.VerifyImage(IndexNG, "A", imgCtrl.ColorName, modifyValue);

                                if ("G" != modifyValue)
                                    AddNGListView(++CountNGListView, IndexNG + 1, "A", modifyValue);

                                // SetNGData() 에서는 A/B 중 하나만 보여줌으로, A 변경 시 B 를 Display 하기위해 호출해 준다.
                                if (false == IsModifyB)
                                    DisplayNGImage();
                            }
                            else if (false == IsModifyB)
                            {
                                ModifyValue2 = modify;
                                IsModifyB = true;

                                dataService.DataResult.Modify2(IndexNG, modifyValue);
                                dataService.DataDefect.VerifyImage(IndexNG, "B", imgCtrl.ColorName, modifyValue);

                                if ("G" != modifyValue)
                                    AddNGListView(++CountNGListView, IndexNG + 1, "B", modifyValue);
                            }

                            if ((true == IsModifyA) && (true == IsModifyB))
                            {
                                listNG.RemoveAt(0);
                                //DisplayResults("G" == modifyValue);

                                //dataService.DataResult.AddReview(ModifyValue1, ModifyValue2);  // Timer 에서 자동으로 해줌.
                                SetNGData();
                            }
                        }
                    }
                    else    // Good 처리
                    {
                        if (false == IsModifyA)
                        {
                            dataService.DataDefect.VerifyImage(IndexNG, "A", imgCtrl.ColorName, modifyValue);
                        }
                        else if (false == IsModifyB)
                        {
                            dataService.DataDefect.VerifyImage(IndexNG, "B", imgCtrl.ColorName, modifyValue);
                        }

                        if (lbxBottom.SelectedIndex == lbxBottom.Items.Count - 1)
                        {
                            if (0 < lbxMono.Items.Count)
                            {
                                lbxMono.SelectedIndex = 0;
                                lbxMono.Focus();
                            }
                            else // End 처리
                            {
                                if (false == IsModifyA)
                                {
                                    ModifyValue1 = modify;
                                    IsModifyA = true;

                                    dataService.DataResult.Modify1(IndexNG, modifyValue);
                                }
                                else if (false == IsModifyB)
                                {
                                    ModifyValue2 = modify;
                                    IsModifyB = true;

                                    dataService.DataResult.Modify2(IndexNG, modifyValue);
                                }
                                if ((true == IsModifyA) && (true == IsModifyB))
                                {
                                    listNG.RemoveAt(0);
                                    //DisplayResults("G" == modifyValue);

                                    //dataService.DataResult.AddReview(ModifyValue1, ModifyValue2);  // Timer 에서 자동으로 해줌.
                                    SetNGData();
                                }
                            }
                        }
                        else
                        {
                            lbxBottom.SelectedIndex = lbxBottom.SelectedIndex + 1;
                        }
                    }
                }
                else if (2 == SelectedListBox)
                {
                    ReviewImageControl imgCtrl = (ReviewImageControl)lbxMono.SelectedItem;

                    if (null == imgCtrl)
                        return;

                    // End 처리
                    if ("G" != modifyValue)
                    {
                        if ((false == IsModifyA) || (false == IsModifyB))
                        {
                            if (false == IsModifyA)
                            {
                                ModifyValue1 = modify;
                                IsModifyA = true;

                                dataService.DataResult.Modify1(IndexNG, modifyValue);

                                dataService.DataDefect.VerifyImage(IndexNG, "A", imgCtrl.ColorName, modifyValue);

                                if ("G" != modifyValue)
                                    AddNGListView(++CountNGListView, IndexNG + 1, "A", modifyValue);

                                // SetNGData() 에서는 A/B 중 하나만 보여줌으로, A 변경 시 B 를 Display 하기위해 호출해 준다.
                                if (false == IsModifyB)
                                    DisplayNGImage();
                            }
                            else if (false == IsModifyB)
                            {
                                ModifyValue2 = modify;
                                IsModifyB = true;

                                dataService.DataResult.Modify2(IndexNG, modifyValue);
                                dataService.DataDefect.VerifyImage(IndexNG, "B", imgCtrl.ColorName, modifyValue);

                                if ("G" != modifyValue)
                                    AddNGListView(++CountNGListView, IndexNG + 1, "B", modifyValue);
                            }

                            if ((true == IsModifyA) && (true == IsModifyB))
                            {
                                listNG.RemoveAt(0);
                                //DisplayResults("G" == modifyValue);

                                //dataService.DataResult.AddReview(ModifyValue1, ModifyValue2);  // Timer 에서 자동으로 해줌.
                                SetNGData();
                            }
                        }
                    }
                    else
                    {
                        if (false == IsModifyA)
                        {
                            dataService.DataDefect.VerifyImage(IndexNG, "A", imgCtrl.ColorName, modifyValue);
                        }
                        else if (false == IsModifyB)
                        {
                            dataService.DataDefect.VerifyImage(IndexNG, "B", imgCtrl.ColorName, modifyValue);
                        }

                        if (lbxMono.SelectedIndex == lbxMono.Items.Count - 1)
                        {
                            // End 처리
                            if (false == IsModifyA)
                            {
                                ModifyValue1 = modify;
                                IsModifyA = true;

                                dataService.DataResult.Modify1(IndexNG, modifyValue);
                            }
                            else if (false == IsModifyB)
                            {
                                ModifyValue2 = modify;
                                IsModifyB = true;

                                dataService.DataResult.Modify2(IndexNG, modifyValue);
                            }
                            if ((true == IsModifyA) && (true == IsModifyB))
                            {
                                listNG.RemoveAt(0);
                                //DisplayResults("G" == modifyValue);

                                //dataService.DataResult.AddReview(ModifyValue1, ModifyValue2);  // Timer 에서 자동으로 해줌.
                                SetNGData();
                            }
                        }
                        else
                        {
                            lbxMono.SelectedIndex = lbxMono.SelectedIndex + 1;
                        }
                    }
                }
            }
            
        }

        private void Modify3(int modify)
        {
            if (false == dataService.DataSystem.IsSelectedModify)
                return;

            if (false == rdViewVerify.IsChecked)
                return;

            if (false == IsDisplayDone)
                return;

            if (modify > 9 || (0 > modify))
                modify = 0;

            if (true == IsAutoModify)
            {
                Log_Debug.WriteLine("Manual Modify Code({0})", modify);
                IsAutoModify = false;
            }

            string modifyValue = "G";

            //if (0 != modify)
            //    modifyValue = modify.ToString();
            if (0 != modify)
                if (modify < dataService.DataSystem.verifyCodes.Length)
                    modifyValue = dataService.DataSystem.verifyCodes[modify];

            Log_Trace.WriteLine(string.Format("Modify Index = {0}", IndexNG));

            // 전체 판정
            if (0 == cbVerifyMethod.SelectedIndex)
            {
                if (0 < listNG.Count)
                {
                    if ((false == IsModifyA) || (false == IsModifyB) || (false == IsModifyC))
                    {
                        if (false == IsModifyA && true == IsDisplayedA)
                        {
                            ModifyValue1 = modify;
                            IsModifyA = true;

                            dataService.DataResult.Modify1(IndexNG, modifyValue);

                            dataService.DataDefect.Verify(IndexNG, "A", modifyValue);

                            if ("G" != modifyValue)
                                AddNGListView(++CountNGListView, IndexNG + 1, "A", modifyValue);

                            //DisplayResults("G" == modifyValue);

                            // SetNGData() 에서는 A/B 중 하나만 보여줌으로, A 변경 시 B 를 Display 하기위해 호출해 준다.
                            if (false == IsModifyB)
                                DisplayNGImage();
                        }
                        else if (false == IsModifyB && true == IsDisplayedB)
                        {
                            ModifyValue2 = modify;
                            IsModifyB = true;

                            dataService.DataResult.Modify2(IndexNG, modifyValue);
                            dataService.DataDefect.Verify(IndexNG, "B", modifyValue);

                            if ("G" != modifyValue)
                                AddNGListView(++CountNGListView, IndexNG + 1, "B", modifyValue);

                            //DisplayResults("G" == modifyValue);

                            if (false == IsModifyC)
                                DisplayNGImage();
                        }
                        else if (false == IsModifyC && true == IsDisplayedC)
                        {
                            ModifyValue3 = modify;
                            IsModifyC = true;

                            dataService.DataResult.Modify3(IndexNG, modifyValue);
                            dataService.DataDefect.Verify(IndexNG, "C", modifyValue);

                            if ("G" != modifyValue)
                                AddNGListView(++CountNGListView, IndexNG + 1, "C", modifyValue);

                            //DisplayResults("G" == modifyValue);
                        }

                        if ((true == IsModifyA) && (true == IsModifyB) && (true == IsModifyC))
                        {
                            if( 0 < listNG.Count )
                                listNG.RemoveAt(0);

                            //dataService.DataResult.AddReview(ModifyValue1, ModifyValue2);  // Timer 에서 자동으로 해줌.
                            SetNGData();
                        }
                    }

                    AddReviewData();
                    DisplayTemp();
                }
            }
            // 이미지별 개별 판정.
            else
            {
                // Top
                if (0 == SelectedListBox)
                {
                    ReviewImageControl imgCtrl = (ReviewImageControl)lbxTop.SelectedItem;

                    if (null == imgCtrl)
                        return;

                    // End 처리
                    if ("G" != modifyValue)
                    {
                        if ((false == IsModifyA) || (false == IsModifyB) || (false == IsModifyC))
                        {
                            if (false == IsModifyA)
                            {
                                ModifyValue1 = modify;
                                IsModifyA = true;

                                dataService.DataResult.Modify1(IndexNG, modifyValue);

                                dataService.DataDefect.VerifyImage(IndexNG, "A", imgCtrl.ColorName, modifyValue);

                                if ("G" != modifyValue)
                                    AddNGListView(++CountNGListView, IndexNG + 1, "A", modifyValue);

                                // SetNGData() 에서는 A/B 중 하나만 보여줌으로, A 변경 시 B 를 Display 하기위해 호출해 준다.
                                if (false == IsModifyB)
                                    DisplayNGImage();
                            }
                            else if (false == IsModifyB)
                            {
                                ModifyValue2 = modify;
                                IsModifyB = true;

                                dataService.DataResult.Modify2(IndexNG, modifyValue);
                                dataService.DataDefect.VerifyImage(IndexNG, "B", imgCtrl.ColorName, modifyValue);

                                if ("G" != modifyValue)
                                    AddNGListView(++CountNGListView, IndexNG + 1, "B", modifyValue);
                            }
                            else if (false == IsModifyC)
                            {
                                ModifyValue3 = modify;
                                IsModifyC = true;

                                dataService.DataResult.Modify2(IndexNG, modifyValue);
                                dataService.DataDefect.VerifyImage(IndexNG, "C", imgCtrl.ColorName, modifyValue);

                                if ("G" != modifyValue)
                                    AddNGListView(++CountNGListView, IndexNG + 1, "C", modifyValue);
                            }

                            if ((true == IsModifyA) && (true == IsModifyB) && (true == IsModifyC))
                            {
                                listNG.RemoveAt(0);
                                //DisplayResults("G" == modifyValue);

                                //dataService.DataResult.AddReview(ModifyValue1, ModifyValue2);  // Timer 에서 자동으로 해줌.
                                SetNGData();
                            }
                        }
                    }
                    else    // Good 처리
                    {
                        if (false == IsModifyA)
                        {
                            dataService.DataDefect.VerifyImage(IndexNG, "A", imgCtrl.ColorName, modifyValue);
                        }
                        else if (false == IsModifyB)
                        {
                            dataService.DataDefect.VerifyImage(IndexNG, "B", imgCtrl.ColorName, modifyValue);
                        }
                        else if (false == IsModifyC)
                        {
                            dataService.DataDefect.VerifyImage(IndexNG, "C", imgCtrl.ColorName, modifyValue);
                        }


                        if (lbxTop.SelectedIndex == lbxTop.Items.Count - 1)
                        {
                            if (0 < lbxBottom.Items.Count)
                            {
                                lbxBottom.SelectedIndex = 0;
                                lbxBottom.Focus();
                            }
                            else if (0 < lbxMono.Items.Count)
                            {
                                lbxMono.SelectedIndex = 0;
                                lbxMono.Focus();
                            }
                            else // End 처리
                            {
                                if (false == IsModifyA)
                                {
                                    ModifyValue1 = modify;
                                    IsModifyA = true;

                                    dataService.DataResult.Modify1(IndexNG, modifyValue);
                                }
                                else if (false == IsModifyB)
                                {
                                    ModifyValue2 = modify;
                                    IsModifyB = true;

                                    dataService.DataResult.Modify2(IndexNG, modifyValue);
                                }
                                else if (false == IsModifyC)
                                {
                                    ModifyValue3 = modify;
                                    IsModifyC = true;

                                    dataService.DataResult.Modify3(IndexNG, modifyValue);
                                }

                                if ((true == IsModifyA) && (true == IsModifyB) && (true == IsModifyC))
                                {
                                    listNG.RemoveAt(0);
                                    //DisplayResults("G" == modifyValue);

                                    //dataService.DataResult.AddReview(ModifyValue1, ModifyValue2);  // Timer 에서 자동으로 해줌.
                                    SetNGData();
                                }
                            }
                        }
                        else
                        {
                            lbxTop.SelectedIndex = lbxTop.SelectedIndex + 1;
                        }
                    }

                }
                else if (1 == SelectedListBox)
                {
                    ReviewImageControl imgCtrl = (ReviewImageControl)lbxBottom.SelectedItem;

                    if (null == imgCtrl)
                        return;

                    // End 처리
                    if ("G" != modifyValue)
                    {
                        if ((false == IsModifyA) || (false == IsModifyB) || (false == IsModifyC))
                        {
                            if (false == IsModifyA)
                            {
                                ModifyValue1 = modify;
                                IsModifyA = true;

                                dataService.DataResult.Modify1(IndexNG, modifyValue);

                                dataService.DataDefect.VerifyImage(IndexNG, "A", imgCtrl.ColorName, modifyValue);

                                if ("G" != modifyValue)
                                    AddNGListView(++CountNGListView, IndexNG + 1, "A", modifyValue);

                                // SetNGData() 에서는 A/B 중 하나만 보여줌으로, A 변경 시 B 를 Display 하기위해 호출해 준다.
                                if (false == IsModifyB)
                                    DisplayNGImage();
                            }
                            else if (false == IsModifyB)
                            {
                                ModifyValue2 = modify;
                                IsModifyB = true;

                                dataService.DataResult.Modify2(IndexNG, modifyValue);
                                dataService.DataDefect.VerifyImage(IndexNG, "B", imgCtrl.ColorName, modifyValue);

                                if ("G" != modifyValue)
                                    AddNGListView(++CountNGListView, IndexNG + 1, "B", modifyValue);
                            }
                            else if (false == IsModifyC)
                            {
                                ModifyValue3 = modify;
                                IsModifyC = true;

                                dataService.DataResult.Modify2(IndexNG, modifyValue);
                                dataService.DataDefect.VerifyImage(IndexNG, "C", imgCtrl.ColorName, modifyValue);

                                if ("G" != modifyValue)
                                    AddNGListView(++CountNGListView, IndexNG + 1, "C", modifyValue);
                            }

                            if ((true == IsModifyA) && (true == IsModifyB) && (true == IsModifyC))
                            {
                                listNG.RemoveAt(0);
                                //DisplayResults("G" == modifyValue);

                                //dataService.DataResult.AddReview(ModifyValue1, ModifyValue2);  // Timer 에서 자동으로 해줌.
                                SetNGData();
                            }
                        }
                    }
                    else    // Good 처리
                    {
                        if (false == IsModifyA)
                        {
                            dataService.DataDefect.VerifyImage(IndexNG, "A", imgCtrl.ColorName, modifyValue);
                        }
                        else if (false == IsModifyB)
                        {
                            dataService.DataDefect.VerifyImage(IndexNG, "B", imgCtrl.ColorName, modifyValue);
                        }
                        else if (false == IsModifyC)
                        {
                            dataService.DataDefect.VerifyImage(IndexNG, "C", imgCtrl.ColorName, modifyValue);
                        }

                        if (lbxBottom.SelectedIndex == lbxBottom.Items.Count - 1)
                        {
                            if (0 < lbxMono.Items.Count)
                            {
                                lbxMono.SelectedIndex = 0;
                                lbxMono.Focus();
                            }
                            else // End 처리
                            {
                                if (false == IsModifyA)
                                {
                                    ModifyValue1 = modify;
                                    IsModifyA = true;

                                    dataService.DataResult.Modify1(IndexNG, modifyValue);
                                }
                                else if (false == IsModifyB)
                                {
                                    ModifyValue2 = modify;
                                    IsModifyB = true;

                                    dataService.DataResult.Modify2(IndexNG, modifyValue);
                                }
                                else if (false == IsModifyC)
                                {
                                    ModifyValue3 = modify;
                                    IsModifyC = true;

                                    dataService.DataResult.Modify3(IndexNG, modifyValue);
                                }

                                if ((true == IsModifyA) && (true == IsModifyB))
                                {
                                    listNG.RemoveAt(0);
                                    //DisplayResults("G" == modifyValue);

                                    //dataService.DataResult.AddReview(ModifyValue1, ModifyValue2);  // Timer 에서 자동으로 해줌.
                                    SetNGData();
                                }
                            }
                        }
                        else
                        {
                            lbxBottom.SelectedIndex = lbxBottom.SelectedIndex + 1;
                        }
                    }
                }
                else if (2 == SelectedListBox)
                {
                    ReviewImageControl imgCtrl = (ReviewImageControl)lbxMono.SelectedItem;

                    if (null == imgCtrl)
                        return;

                    // End 처리
                    if ("G" != modifyValue)
                    {
                        if ((false == IsModifyA) || (false == IsModifyB) || (false == IsModifyC))
                        {
                            if (false == IsModifyA)
                            {
                                ModifyValue1 = modify;
                                IsModifyA = true;

                                dataService.DataResult.Modify1(IndexNG, modifyValue);

                                dataService.DataDefect.VerifyImage(IndexNG, "A", imgCtrl.ColorName, modifyValue);

                                if ("G" != modifyValue)
                                    AddNGListView(++CountNGListView, IndexNG + 1, "A", modifyValue);

                                // SetNGData() 에서는 A/B 중 하나만 보여줌으로, A 변경 시 B 를 Display 하기위해 호출해 준다.
                                if (false == IsModifyB)
                                    DisplayNGImage();
                            }
                            else if (false == IsModifyB)
                            {
                                ModifyValue2 = modify;
                                IsModifyB = true;

                                dataService.DataResult.Modify2(IndexNG, modifyValue);
                                dataService.DataDefect.VerifyImage(IndexNG, "B", imgCtrl.ColorName, modifyValue);

                                if ("G" != modifyValue)
                                    AddNGListView(++CountNGListView, IndexNG + 1, "B", modifyValue);
                            }
                            else if (false == IsModifyC)
                            {
                                ModifyValue3 = modify;
                                IsModifyC = true;

                                dataService.DataResult.Modify2(IndexNG, modifyValue);
                                dataService.DataDefect.VerifyImage(IndexNG, "C", imgCtrl.ColorName, modifyValue);

                                if ("G" != modifyValue)
                                    AddNGListView(++CountNGListView, IndexNG + 1, "B", modifyValue);
                            }

                            if ((true == IsModifyA) && (true == IsModifyB) && (true == IsModifyC))
                            {
                                listNG.RemoveAt(0);
                                //DisplayResults("G" == modifyValue);

                                //dataService.DataResult.AddReview(ModifyValue1, ModifyValue2);  // Timer 에서 자동으로 해줌.
                                SetNGData();
                            }
                        }
                    }
                    else
                    {
                        if (false == IsModifyA)
                        {
                            dataService.DataDefect.VerifyImage(IndexNG, "A", imgCtrl.ColorName, modifyValue);
                        }
                        else if (false == IsModifyB)
                        {
                            dataService.DataDefect.VerifyImage(IndexNG, "B", imgCtrl.ColorName, modifyValue);
                        }
                        else if (false == IsModifyC)
                        {
                            dataService.DataDefect.VerifyImage(IndexNG, "C", imgCtrl.ColorName, modifyValue);
                        }

                        if (lbxMono.SelectedIndex == lbxMono.Items.Count - 1)
                        {
                            // End 처리
                            if (false == IsModifyA)
                            {
                                ModifyValue1 = modify;
                                IsModifyA = true;

                                dataService.DataResult.Modify1(IndexNG, modifyValue);
                            }
                            else if (false == IsModifyB)
                            {
                                ModifyValue2 = modify;
                                IsModifyB = true;

                                dataService.DataResult.Modify2(IndexNG, modifyValue);
                            }
                            else if (false == IsModifyC)
                            {
                                ModifyValue3 = modify;
                                IsModifyC = true;

                                dataService.DataResult.Modify3(IndexNG, modifyValue);
                            }
                            if ((true == IsModifyA) && (true == IsModifyB) && (true == IsModifyB))
                            {
                                listNG.RemoveAt(0);
                                //DisplayResults("G" == modifyValue);

                                //dataService.DataResult.AddReview(ModifyValue1, ModifyValue2);  // Timer 에서 자동으로 해줌.
                                SetNGData();
                            }
                        }
                        else
                        {
                            lbxMono.SelectedIndex = lbxMono.SelectedIndex + 1;
                        }
                    }
                }
            }

        }

        private void NotModify()
        {
            if (3 == dataService.DataRecipe.Line)
            {
                NotModify3();
                return;
            }

            if (true == dataService.DataSystem.IsSelectedModify)
                return;

            if (false == IsAutoModify)
            {
                Log_Debug.WriteLine("AutoModify");
                IsAutoModify = true;
            }

            ModifyAuto();
        }

        private void NotModify3()
        {
            if (true == dataService.DataSystem.IsSelectedModify)
                return;

            if (false == IsAutoModify)
            {
                Log_Debug.WriteLine("AutoModify");
                IsAutoModify = true;
            }

            ModifyAuto3();
        }

        private void ModifyInspected()
        {
            if (false == dataService.DataSystem.IsSelectedModify)
                return;

            if (false == rdViewVerify.IsChecked)
                return;

            if (false == IsDisplayDone)
                return;

            Log_Trace.WriteLine(string.Format("Modify Inspected listNG.Count = {0}", listNG.Count));

            if (0 < listNG.Count)
            {
                if (3 == dataService.DataRecipe.Line)
                {
                    ModifyAuto3();
                    return;
                }
                else
                {
                    ModifyAuto();
                }
            }
        }

        private void ModifyAuto()
        {
            if (0 < listNG.Count)
            {
                if ((false == IsModifyA) || (false == IsModifyB))
                {
                    //System.Diagnostics.Debug.WriteLine("Verify Index {0}, A({1} / {2}), B({3} / {4})", IndexNG, IsModifyA, IsDisplayedA, IsModifyB, IsDisplayedB);

                    if (false == IsModifyA && true == IsDisplayedA)
                    {
                        IsModifyA = true;


                        // 수정을 하게되면 Top, Bottom, Mono 같이 수정됨. 
                        //dataService.DataResult.Modify1(IndexNG, ModifyValue1);

                        dataService.DataDefect.VerifyAuto(IndexNG, "A", "", TempValue1);

                        AddNGListView(++CountNGListView, IndexNG + 1, "A", TempValue1);

                        TempValue1 = "";

                        //System.Diagnostics.Debug.WriteLine("Verify A {0}", IndexNG);

                        Log_Debug.WriteLine("Auto Verify A {0}", IndexNG);

                        // SetNGData() 에서는 A/B 중 하나만 보여줌으로, A 변경 시 B 를 Display 하기위해 호출해 준다.
                        if (false == IsModifyB)
                            DisplayNGImage();
                    }
                    else if (false == IsModifyB && true == IsDisplayedB)
                    {
                        IsModifyB = true;


                        //dataService.DataResult.Modify2(IndexNG, ModifyValue2);
                        dataService.DataDefect.VerifyAuto(IndexNG, "B", "", TempValue2);

                        AddNGListView(++CountNGListView, IndexNG + 1, "B", TempValue2);

                        TempValue2 = "";

                        //System.Diagnostics.Debug.WriteLine("Verify B {0}", IndexNG);

                        Log_Debug.WriteLine("Auto Verify B {0}", IndexNG);
                    }

                    if ((true == IsModifyA) && (true == IsModifyB))
                    {

                        Log_Debug.WriteLine("ReviewWindow.NotModify() : Remove Index({0})", listNG[0]);

                        listNG.RemoveAt(0);

                        //System.Diagnostics.Debug.WriteLine("NOTModify");

                        //dataService.DataDefect.VerifyAuto(IndexNG, "B", "", "");

                        SetNGData();
                    }
                }

                AddReviewData();
                DisplayTemp();
            }
        }

        private void ModifyAuto3()
        {
            if (0 < listNG.Count)
            {
                if ((false == IsModifyA) || (false == IsModifyB) || (false == IsModifyC))
                {
                    //System.Diagnostics.Debug.WriteLine("Verify Index {0}, A({1} / {2}), B({3} / {4})", IndexNG, IsModifyA, IsDisplayedA, IsModifyB, IsDisplayedB);

                    if (false == IsModifyA && true == IsDisplayedA)
                    {
                        IsModifyA = true;


                        // 수정을 하게되면 Top, Bottom, Mono 같이 수정됨. 
                        //dataService.DataResult.Modify1(IndexNG, ModifyValue1);

                        dataService.DataDefect.VerifyAuto(IndexNG, "A", "", TempValue1);

                        AddNGListView(++CountNGListView, IndexNG + 1, "A", TempValue1);

                        TempValue1 = "";

                        //System.Diagnostics.Debug.WriteLine("Verify A {0}", IndexNG);

                        Log_Debug.WriteLine("Auto Verify A {0}", IndexNG);

                        // SetNGData() 에서는 A/B 중 하나만 보여줌으로, A 변경 시 B 를 Display 하기위해 호출해 준다.
                        if (false == IsModifyB)
                            DisplayNGImage();
                    }
                    else if (false == IsModifyB && true == IsDisplayedB)
                    {
                        IsModifyB = true;


                        //dataService.DataResult.Modify2(IndexNG, ModifyValue2);
                        dataService.DataDefect.VerifyAuto(IndexNG, "B", "", TempValue2);

                        AddNGListView(++CountNGListView, IndexNG + 1, "B", TempValue2);

                        TempValue2 = "";

                        //System.Diagnostics.Debug.WriteLine("Verify B {0}", IndexNG);

                        Log_Debug.WriteLine("Auto Verify B {0}", IndexNG);

                        if (false == IsModifyC)
                            DisplayNGImage();
                    }
                    else if (false == IsModifyC && true == IsDisplayedC)
                    {
                        IsModifyC = true;


                        //dataService.DataResult.Modify2(IndexNG, ModifyValue2);
                        dataService.DataDefect.VerifyAuto(IndexNG, "C", "", TempValue3);

                        AddNGListView(++CountNGListView, IndexNG + 1, "C", TempValue3);

                        TempValue3 = "";

                        //System.Diagnostics.Debug.WriteLine("Verify B {0}", IndexNG);

                        Log_Debug.WriteLine("Auto Verify C {0}", IndexNG);
                    }

                    if ((true == IsModifyA) && (true == IsModifyB) && (true == IsModifyC))
                    {

                        Log_Debug.WriteLine("ReviewWindow.NotModify() : Remove Index({0})", listNG[0]);

                        listNG.RemoveAt(0);

                        //System.Diagnostics.Debug.WriteLine("NOTModify");

                        //dataService.DataDefect.VerifyAuto(IndexNG, "B", "", "");

                        SetNGData();
                    }
                }

                AddReviewData();
                DisplayTemp();
            }
        }

        public void Start()
        {
            IsDrawing = false;
            IndexDisplay = -1;

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            {
                ClearDisplay();
                ClearNGListView();
                ClearPunchView();

                cbImageSize.IsEnabled = false;
                cbImageNumber.IsEnabled = false;
                cbImageArray.IsEnabled = false;

                curIndexPunch = -1;
                curLinePunch = "";
            }));

            //if (InvokeRequired)
            //{
            //    OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            //                                (ThreadStart)delegate()
            //                                {
            //                                    ClearDisplay();
            //                                    ClearNGListView();
            //                                    ClearPunchView();

            //                                    cbImageSize.IsEnabled = false;
            //                                    cbImageNumber.IsEnabled = false;
            //                                    cbImageArray.IsEnabled = false;
            //                                }
            //    );
            //}
            //else
            //{
            //    ClearDisplay();
            //    ClearNGListView();
            //    ClearPunchView();

            //    cbImageSize.IsEnabled = false;
            //    cbImageNumber.IsEnabled = false;
            //    cbImageArray.IsEnabled = false;
            //}


            curIndexPunch = -1;
            curLinePunch = "";

            StartThread();
            //SetTimer();
        }

        public void Stop()
        {
            StopThread();
            SetTimer();

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            {
                SetStop();
            }));

            //if (InvokeRequired)
            //{
            //    OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            //                                (ThreadStart)delegate()
            //                                {
            //                                    SetStop();
            //                                }
            //    );
            //}
            //else
            //{
            //    SetStop();
            //}

            IsDrawing = false;
        }

        private void SetStop()
        {
            ClearImages();
            ClearNGListView();

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            {
                lbLine.Content = "";

                cbImageSize.IsEnabled = true;
                cbImageNumber.IsEnabled = true;
                cbImageArray.IsEnabled = true;
            }));

            //if (InvokeRequired)
            //{
            //    OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            //                                (ThreadStart)delegate()
            //                                {
            //                                    lbLine.Content = "";

            //                                    cbImageSize.IsEnabled = true;
            //                                    cbImageNumber.IsEnabled = true;
            //                                    cbImageArray.IsEnabled = true;
            //                                }
            //    );
            //}
            //else
            //{
            //    lbLine.Content = "";

            //    cbImageSize.IsEnabled = true;
            //    cbImageNumber.IsEnabled = true;
            //    cbImageArray.IsEnabled = true;
            //}

            ResetImages();

            GC.Collect();
        }


        public void Restart(int index)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            {
                SetRestart(index);
            }));

            //if (InvokeRequired)
            //{
            //    OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            //                                (ThreadStart)delegate()
            //                                {
            //                                    SetRestart(index);
            //                                }
            //    );
            //}
            //else
            //{
            //    SetRestart(index);
            //}
        }

        private void SetRestart(int index)
        {
            if (0 > index)
                index = 0;

            listNG.Clear();

            int count = listViewNG.Items.Count;

            for (int i = 0; i < count; ++i)
            {
                NGData data = (NGData)listViewNG.Items[0];
                if (data.NgIndex - 1 >= index)
                {
                    listViewNG.Items.RemoveAt(0);
                    --CountNGListView;
                }
                else
                {
                    break;
                }
            }

            //for (int i = listViewNG.Items.Count-1; i >= 0; --i)
            //{
            //    NGData data = (NGData)listViewNG.Items[i];

            //    if (data.NgIndex - 1 >= index)
            //    {
            //        listViewNG.Items.RemoveAt(i);
            //        --CountNGListView;
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}
        }

        private void ClearDisplay()
        {
            ClearImages();

            for (int i = 0; i < 50; ++i)
            {
                //DisplayTempLine(i, i + 1);

                tblLines[i].Text = (i+1).ToString();
                lblLines[i].Background = Brushes.AliceBlue;

                lblLineAs[i].Content = "";
                lblLineAs[i].Background = Brushes.Beige;

                lblLineBs[i].Content = "";
                lblLineBs[i].Background = Brushes.Beige;

                lblLineCs[i].Content = "";
                lblLineCs[i].Background = Brushes.Beige;
            }
        }

        public void DisposeImages()
        {
            ClearImages();

            SetTimer();
        }

        private void ClearImages()
        {
            IsDisplayDone = false;
            isClearedImageTop = false;
            isClearedImageBottom = false;
            isClearedImageMono = false;

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            {
                ClearListBoxImages(lbxTop, ref isClearedImageTop);
                ClearListBoxImages(lbxBottom, ref isClearedImageBottom);
                ClearListBoxImages(lbxMono, ref isClearedImageMono);

                GC.Collect();
            }));
            //if (InvokeRequired)
            //{
            //    OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            //                                (ThreadStart)delegate()
            //                                {
            //                                    ClearListBoxImages(lbxTop, ref isClearedImageTop);
            //                                    ClearListBoxImages(lbxBottom, ref isClearedImageBottom);
            //                                    ClearListBoxImages(lbxMono, ref isClearedImageMono);
            //                                }
            //    );
            //}
            //else
            //{
            //    ClearListBoxImages(lbxTop, ref isClearedImageTop);
            //    ClearListBoxImages(lbxBottom, ref isClearedImageBottom);
            //    ClearListBoxImages(lbxMono, ref isClearedImageMono);
            //}

            //GC.Collect();
        }

        private void ClearListBoxImages(ListBox lbx, ref bool isCleared)
        {
            try
            {
                isCleared = false;

                if (0 < lbx.Items.Count)
                {
                    for (int i = 0; i < lbx.Items.Count; ++i)
                    {
                        ReviewImageControl ctr = (ReviewImageControl)lbx.Items[i];
                        //ctr.PathColor = "";
                        //ctr.PathHSI = "";
                        ctr.Dispose();
                    }
                    lbx.Items.Clear();
                }

                isCleared = true;
                //lbx.Items.Clear();
                //GC.Collect();
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine("ReviewWindow.ClearListBoxImages(,) : " + exc.Message);
            }
        }

        private void ClearListBoxLastImage(ListBox lbx)
        {
            if (0 < lbx.Items.Count)
            {
                try
                {
                    int index = lbx.Items.Count - 1;

                    ReviewImageControl ctr = (ReviewImageControl)lbx.Items[index];
                    ctr.Dispose();

                    lbx.Items.RemoveAt(index);
                }
                catch (Exception exc)
                {
                    Log_Exception.WriteLine("ReviewWindow.ClearListBoxLastImage() : " + exc.Message);
                }
                finally
                {
                    GC.Collect();
                }
            }
        }

        private void ResetImages()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            {
                SetResetImages();
            }));

            //if (InvokeRequired)
            //{
            //    OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            //                                (ThreadStart)delegate()
            //                                {
            //                                    SetResetImages();
            //                                }
            //    );
            //}
            //else
            //{
            //    SetResetImages();
            //}
        }

        private void SetResetImages()
        {
            //GC.Collect();
            //ClearListBoxImages(lbxTop);
            //ClearListBoxImages(lbxBottom);
            //ClearListBoxImages(lbxMono);
            //GC.Collect();
            //lbxTop.Items.Clear();
            //lbxBottom.Items.Clear();
            //lbxMono.Items.Clear();

            //ReviewImageControl ctr = new ReviewImageControl();
            //lbxTop.Items.Add(ctr);
            //ReviewImageControl ctr2 = new ReviewImageControl();
            //lbxBottom.Items.Add(ctr2);
            //ReviewImageControl ctr3 = new ReviewImageControl();
            //lbxMono.Items.Add(ctr3);

            //lbxTop.Items.Clear();
            //lbxBottom.Items.Clear();
            //lbxMono.Items.Clear();

            //lbxTop.Items.Add(ctr);
            //lbxBottom.Items.Add(ctr2);
            //lbxMono.Items.Add(ctr3);

            //lbxTop.Items.Clear();
            //lbxBottom.Items.Clear();
            //lbxMono.Items.Clear();

            //lbxTop.Items.Add(ctr);
            //lbxBottom.Items.Add(ctr2);
            //lbxMono.Items.Add(ctr3);

            //lbxTop.Items.Clear();
            //lbxBottom.Items.Clear();
            //lbxMono.Items.Clear();

            //GC.Collect();
        }

        private void ShowImageViewer()
        {
            return;

            //if (false == rdViewVerify.IsChecked)
            //    return;

            //if (false == dataService.DataSystem.IsSelectedModify)
            //    return;

            //// Top
            //if (0 == SelectedListBox)
            //{
            //    if (0 < lbxTop.Items.Count)
            //    {
            //        ReviewImageControl control = (ReviewImageControl)lbxTop.SelectedItem;

            //        if (null != control)
            //        {
            //            ImageViewerWindow win = new ImageViewerWindow();
            //            win.Owner = this;

            //            if (true == control.IsVisibleColor)
            //                win.PathImage1 = control.PathColor;
            //            else
            //                win.PathImage1 = "";

            //            if (true == control.IsVisibleHSI)
            //                win.PathImage2 = control.PathHSI;
            //            else
            //                win.PathImage2 = "";

            //            win.Show();
            //        }
            //    }
            //}

            //else if (1 == SelectedListBox)
            //{
            //    if (0 < lbxBottom.Items.Count)
            //    {
            //        ReviewImageControl control = (ReviewImageControl)lbxBottom.SelectedItem;

            //        if (null != control)
            //        {
            //            ImageViewerWindow win = new ImageViewerWindow();
            //            win.Owner = this;

            //            if (true == control.IsVisibleColor)
            //                win.PathImage1 = control.PathColor;
            //            else
            //                win.PathImage1 = "";

            //            if (true == control.IsVisibleHSI)
            //                win.PathImage2 = control.PathHSI;
            //            else
            //                win.PathImage2 = "";

            //            win.Show();
            //        }
            //    }
            //}

            //else if (2 == SelectedListBox)
            //{
            //    if (0 < lbxMono.Items.Count)
            //    {
            //        ReviewImageControl control = (ReviewImageControl)lbxMono.SelectedItem;

            //        if (null != control)
            //        {
            //            ImageViewerWindow win = new ImageViewerWindow();
            //            win.Owner = this;

            //            if (true == control.IsVisibleColor)
            //                win.PathImage1 = control.PathColor;
            //            else
            //                win.PathImage1 = "";

            //            if (true == control.IsVisibleHSI)
            //                win.PathImage2 = control.PathHSI;
            //            else
            //                win.PathImage2 = "";

            //            win.Show();
            //        }
            //    }
            //}

            

        }

        public void SetVerifyEnable()
        {
            if (null != dataService)
            {
                if (true == dataService.DataSystem.IsSelectedModify)
                {
                    ledActiveState.IsEnabled = true;
                    lblActiveState.Content = "Verify Enabled";
                }
                else
                {
                    ledActiveState.IsEnabled = false;
                    lblActiveState.Content = "Verify Disabled";
                }
            }
        }

        private void lblActive_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (null != dataService)
            {
                if (true == dataService.DataSystem.IsSelectedModify)
                    dataService.DataSystem.IsSelectedModify = false;
                else
                    dataService.DataSystem.IsSelectedModify = true;

                SetVerifyEnable();
            }

            //////if (0 < listNG.Count)
            ////{
            //lbxTop.Items.Clear();
            //lbxBottom.Items.Clear();
            //lbxMono.Items.Clear();
            //GC.Collect();

            //int index = 1;
            //string fileName = string.Format("index{0:000000}_A", index);
            //string path;
            ////// Top1
            //path = dataService.DataSystem.Top1Path;


            //DisplayNGImageListBox(1, "TOP", "A", path, fileName, lbxTop, true);

        }

        private void cbImageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dataService.DataSystem.ReviewImageSize = cbImageSize.SelectedIndex;

            // Auto
            if (0 == cbImageSize.SelectedIndex)
            {
                int size = ((int)lbxTop.ActualHeight) / 2 - 6 - 13;   // Height/2 - Margin - TitleHeight/2

                ImageWidth = size;
                ImageHeight = size;
            }
            else
            {
                ImageWidth = ImageHeight = int.Parse(cbImageSize.SelectedItem.ToString());
            }

            RedrawImages();

            lbxTop.Focus();
        }

        private void cbImageNumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dataService.DataSystem.ReviewImageNumber = cbImageNumber.SelectedIndex;
            
            RedrawImages();

            lbxTop.Focus();
        }

        private void cbImageArray_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dataService.DataSystem.ReviewImageArray = cbImageArray.SelectedIndex;

            ImageColumns = cbImageArray.SelectedIndex;

            RedrawImages();

            lbxTop.Focus();
        }

        private void cbVerifyMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dataService.DataSystem.VerifyMethod = cbVerifyMethod.SelectedIndex;

            lbxTop.Focus();
        }

        private void RedrawImages()
        {
            if (null == cbImageNumber.SelectedItem)
                return;
            if (null == cbImageSize.SelectedItem)
                return;
            if (null == cbImageArray.SelectedItem)
                return;

            // ImageSize 가 최우선
            
            // 이미지 갯수가 Auto 가 아닐경우 Margin 값을 다시 적용한다.
            if (0 == cbImageNumber.SelectedIndex)
            {
                ImageMargin = new Thickness(10, 0, 10, 0);
            }
            else
            {
                int lbxSize = (int)(lbxTop.ActualWidth - lbxTop.Padding.Left + lbxTop.Padding.Right);
                int imageNumber = int.Parse(cbImageNumber.SelectedItem.ToString());
                int ctrSize = ImageWidth + 30;

                if (0 != cbImageArray.SelectedIndex)
                {
                    ctrSize = ImageWidth * 2 + 30;
                }
                int margin = (lbxSize - ctrSize * imageNumber);

                // Image 갯수를 다 넣지 못할 경우 Auto 로 다시 세팅
                if (0 > margin)
                {
                    ImageMargin = new Thickness(10, 0, 10, 0);
                    cbImageNumber.SelectedIndex = 0;
                }
                // Image Margin 값을 조정한다.
                else
                {
                    margin = margin / (imageNumber * 2);

                    ImageMargin = new Thickness((double)margin, 0, (double)margin, 0);
                }
            }


            // Top
            RedrawListBoxImages(lbxTop);

            // Bottom
            RedrawListBoxImages(lbxBottom);

            // Mono
            RedrawListBoxImages(lbxMono);

        }

        private void RedrawListBoxImages(ListBox lbx)
        {
            if (0 < lbx.Items.Count)
            {
                for (int i = 0; i < lbx.Items.Count; ++i)
                {
                    ReviewImageControl ctr = (ReviewImageControl)lbx.Items[i];

                    ctr.Margin = ImageMargin;

                    ctr.ImageWidth = ImageWidth;
                    ctr.ImageHeight = ImageHeight;
                    ctr.Columns = ImageColumns;
                }
            }
        }

        #region EVENTs

        private void OnEventPunch(int punchUnit, int state, string line)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            {
                SetEventPunch(punchUnit, state, line);
            }));
        }

        private void SetEventPunch(int punchUnit, int state, string line)
        {
            EnumSmartIC.PunchStates punchState = (EnumSmartIC.PunchStates)state;

            switch (punchState)
            {
                case EnumSmartIC.PunchStates.punch://=0, 
                    break;
                case EnumSmartIC.PunchStates.punchDone:
                    break;
                case EnumSmartIC.PunchStates.punchStart:
                case EnumSmartIC.PunchStates.punchEnd:
                    break;
                case EnumSmartIC.PunchStates.tHole:
                    if (true == rdViewPunch.IsChecked)
                    {
                        DisplaySelectedValue(punchUnit, line, 1);
                    }

                    curIndexPunch = punchUnit;
                    curLinePunch = line;
                    break;
                case EnumSmartIC.PunchStates.tHoleDone:
                    break;
                case EnumSmartIC.PunchStates.sectionYield:
                    break;
                case EnumSmartIC.PunchStates.sectionYieldDone:
                    break;
                case EnumSmartIC.PunchStates.punchLine:
                    if (true == rdViewPunch.IsChecked)
                    {
                        DisplaySelectedValue(punchUnit, line, 1);
                    }
                    curIndexPunch = punchUnit;
                    curLinePunch = line;
                    break;
                case EnumSmartIC.PunchStates.cngStart:
                case EnumSmartIC.PunchStates.cngEnd:
                case EnumSmartIC.PunchStates.missPrintStart:
                case EnumSmartIC.PunchStates.missPrintEnd:
                    if (true == rdViewPunch.IsChecked)
                    {
                        DisplaySelectedValue(punchUnit, line, 1);
                    }
                    curIndexPunch = punchUnit;
                    curLinePunch = line;
                    break;
                default:
                    break;
            }
        }


        private void OnEventPunchImage(string path, int index, string line, double offsetX, double offsetY)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            {
                SetEventPunchImage(path, index, line, offsetX, offsetY);
            }));

            //if (InvokeRequired)
            //{
            //    OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            //                                (ThreadStart)delegate()
            //                                {
            //                                    SetEventPunchImage(path, index, line, offsetX, offsetY);
            //                                }
            //    );
            //}
            //else
            //{
            //    SetEventPunchImage(path, index, line, offsetX, offsetY);
            //}
        }

        private void SetEventPunchImage(string path, int index, string line, double offsetX, double offsetY)
        {
            //DoEvents();
            //Thread.Sleep(10);
            //DoEvents();

            System.Diagnostics.Debug.WriteLine(path);

            DateTime start = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 200);
            DateTime timeout = start.Add(duration);

            while (false == File.Exists(path))
            {
                ThreadDoEvents(10);
                if (timeout < DateTime.Now)
                    break;
            }

            if (true == File.Exists(path))
            {
                try
                {

                    ReviewImageControl img = new ReviewImageControl();

                    img.IsVisibleHSI = false;
                    img.PathColor = path;
                    img.ImageWidth = ImageWidth;
                    img.ImageHeight = ImageHeight;
                    img.Margin = ImageMargin;
                    img.Title = string.Format("{0}{1} ({2:0.000}, {3:0.000})", index+1, line, offsetX, offsetY);


                    //img.Margin = ImageMargin;
                    //img.ImageWidth = ImageWidth;
                    //img.ImageHeight = ImageHeight;

                    ////img.Columns = 1;
                    //img.IsVisibleHSI = false;
                    //img.Title = string.Format("{0}_{1} ({2:0.00}, {3:0.00})", index, line, offsetX, offsetY);
                    //img.PathColor = path;

                    lbxPunch.Items.Insert(0, img);

                    System.Windows.Forms.Application.DoEvents();

                    //System.Diagnostics.Debug.WriteLine(path + "draw");

                    if (lbxPunch.Items.Count > dataService.DataSystem.PunchImageLimit)
                    {
                        ClearListBoxLastImage(lbxPunch);
                    }
                }
                catch (Exception exc)
                {
                    Log_Exception.WriteLine("ReviewWidnow.SetEventPunchImage() : " + exc.Message);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(path + "fail");
            }

            //Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate 
            //    {
            //        if (true == File.Exists(path))
            //        {
            //            try
            //            {

            //                ReviewImageControl img = new ReviewImageControl();
            //                img.Margin = ImageMargin;
            //                img.ImageWidth = ImageWidth;
            //                img.ImageHeight = ImageHeight;

            //                img.Columns = 1;
            //                img.IsVisibleHSI = false;
            //                img.Title = string.Format("{0}_{1} ({2:0.00}, {3:0.00})", index, line, offsetX, offsetY);
            //                img.PathColor = path;

            //                lbxPunch.Items.Insert(0, img);

            //                System.Windows.Forms.Application.DoEvents();

            //                System.Diagnostics.Debug.WriteLine(path + "draw");
            //            }
            //            catch (Exception exc)
            //            {
            //                Log_Exception.WriteLine("ReviewWidnow.SetEventPunchImage() : " + exc.Message);
            //            }
            //        }
            //        else
            //        {
            //            System.Diagnostics.Debug.WriteLine(path + "fail");
            //        }
            //    }));
            

            
        }

        private void OnEventChangedRecipe(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            {
                // 3Line
                if (3 == dataService.DataRecipe.Line)
                {
                    for (int i = 0; i < lblLineCs.Length; ++i)
                    {
                        lblLineCs[i].Visibility = System.Windows.Visibility.Visible;
                    }

                    lblLineC.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    for (int i = 0; i < lblLineCs.Length; ++i)
                    {
                        lblLineCs[i].Visibility = System.Windows.Visibility.Hidden;
                    }

                    lblLineC.Visibility = System.Windows.Visibility.Hidden;
                }

            }));

            
        }

        #endregion

        private void lbxViewNG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("SelectionChanged");
            if (true == rdViewSelect.IsChecked)
            {
                NGData data = (NGData)listViewNG.SelectedItem;

                if (null != data)
                {
                    int index = data.NgIndex-1;
                    string line = data.NgLine;

                    //System.Diagnostics.Debug.WriteLine("SelectionChanged Index={0}, line={1}", index, line);

                    DisplaySelectedValue(index, line, 2);
                }
            }
        }

        private void ClearNGListView()
        {
            CountNGListView = 0;

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            {
                if( 0 < listViewNG.Items.Count )
                    listViewNG.Items.Clear();
            }));

            //if (InvokeRequired)
            //{
            //    OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            //                                (ThreadStart)delegate()
            //                                {
            //                                    listViewNG.Items.Clear();
            //                                }
            //    );
            //}
            //else
            //{
            //    listViewNG.Items.Clear();
            //}
        }

        private void ClearPunchView()
        {
            bool isClearedPunch = false;

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            {
                ClearListBoxImages(lbxPunch, ref isClearedPunch);
            }));

            //if (InvokeRequired)
            //{
            //    OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            //                                (ThreadStart)delegate()
            //                                {
            //                                    ClearListBoxImages(lbxPunch, ref isClearedPunch);
            //                                }
            //    );
            //}
            //else
            //{
            //    ClearListBoxImages(lbxPunch, ref isClearedPunch);
            //}

            GC.Collect();
        }

        private void AddNGListView(int no, int index, string line, string defectCode)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            {
                string name = "";
                for (int i = 0; i < dataService.DataDefect.CountNGIDs.Length; ++i)
                {
                    if (defectCode == dataService.DataDefect.CountNGIDs[i])
                    {
                        name = dataService.DataDefect.CountNGNames[i];
                        break;
                    }
                }

                NGData data = new NGData(no, index, line, name);

                //listViewNG.Items.Add(data);
                listViewNG.Items.Insert(0, data);
            }));
            //if (InvokeRequired)
            //{
            //    OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            //                                (ThreadStart)delegate()
            //                                {
            //                                    string name = "";
            //                                    for (int i = 0; i < dataService.DataDefect.CountNGIDs.Length; ++i)
            //                                    {
            //                                        if (defectCode == dataService.DataDefect.CountNGIDs[i])
            //                                        {
            //                                            name = dataService.DataDefect.CountNGNames[i];
            //                                            break;
            //                                        }
            //                                    }

            //                                    NGData data = new NGData(no, index, line, name);
                                                
            //                                    //listViewNG.Items.Add(data);
            //                                    listViewNG.Items.Insert(0, data);
            //                                }
            //    );
            //}
            //else
            //{
            //    string name = "";
            //    for (int i = 0; i < dataService.DataDefect.CountNGIDs.Length; ++i)
            //    {
            //        if (defectCode == dataService.DataDefect.CountNGIDs[i])
            //        {
            //            name = dataService.DataDefect.CountNGNames[i];
            //            break;
            //        }
            //    }

            //    NGData data = new NGData(no, index, line, name);

            //    //listViewNG.Items.Add(data);
            //    listViewNG.Items.Insert(0, data);
            //}
        }

        private void rdViewVerify_Click(object sender, RoutedEventArgs e)
        {
            lbTitle.Content = "Verify View";
        }

        private void rdViewPunch_Click(object sender, RoutedEventArgs e)
        {
            if ("Punch View" != lbTitle.Content.ToString())
            {
                ClearDisplay();
                lbTitle.Content = "Punch View";

                if( (0 < curIndexPunch) && ("" != curLinePunch) )
                    DisplaySelectedValue(curIndexPunch, curLinePunch, 1);
            }
        }

        private void rdViewSelect_Click(object sender, RoutedEventArgs e)
        {
            if ("Select View" != lbTitle.Content.ToString())
            {
                ClearDisplay();
                lbTitle.Content = "Select View";
            }
        }

        private void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
        }

        #region SelectedDisplay
        private void DisplaySelectedValue(int index, string line, int viewType = 0)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            {
                SetDisplaySelectedValue(index, line, viewType);
            }));

            //if (InvokeRequired)
            //{
            //    OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            //                                (ThreadStart)delegate()
            //                                {
            //                                    SetDisplaySelectedValue(index, line, viewType);
            //                                }
            //    );
            //}
            //else
            //{
            //    SetDisplaySelectedValue(index, line, viewType);
            //}
        }

        private void SetDisplaySelectedValue(int index, string line, int viewType = 0)
        {
            IndexSelected = index;
            LineSelected = line;
            DisplaySelectedLine(index);
            DisplaySelectedValues(index);
            DisplaySelectedImages(index, line, viewType);

            lbLine.Content = string.Format("{0}-{1}", index + 1, line);
        }

        // 선택된 index 가 처음으로 오도록 설정.
        private void DisplaySelectedLine(int index)
        {
            for (int i = 0; i < 50; ++i)
            {
                //tblLines[i].Text = (index + 1).ToString();

                tblLines[i].Text = (index + 1 + i).ToString();

                lblLines[i].Background = Brushes.AliceBlue;

                //if (i == IndexSelected)
                //{
                //    lblLines[i].Background = Brushes.Red;
                //}
            }
        }

        // 선택된 데이터를 보여준다. 
        private void DisplaySelectedValues(int index)
        {
            if (3 == dataService.DataRecipe.Line)
            {
                DisplaySelectedValues3(index);
                return;
            }

            // Review 데이터 에서 가져온다. 
            int last = dataService.DataResult.Review.ListRaw.Count;
            int count = 0;

            if (last >= index + 50)
                last = index + 50;

            for (int i = index; i < last; ++i)
            {
                lblLineAs[count].Content = dataService.DataResult.Review.ListRaw[i].Value1;
                lblLineBs[count].Content = dataService.DataResult.Review.ListRaw[i].Value2;

                if ("G" == lblLineAs[count].Content.ToString())
                {
                    lblLineAs[count].Background = Brushes.Beige;
                    lblLineAs[count].Foreground = Brushes.Black;
                }
                else
                {
                    if (i == IndexSelected && "A" == LineSelected)
                    {
                        lblLineAs[count].Background = Brushes.Red;
                        lblLineAs[count].Foreground = Brushes.White;
                    }
                    else
                    {
                        lblLineAs[count].Background = Brushes.Beige;
                        lblLineAs[count].Foreground = Brushes.Red;
                    }
                }

                if ("G" == lblLineBs[count].Content.ToString())
                {
                    lblLineBs[count].Background = Brushes.Beige;
                    lblLineBs[count].Foreground = Brushes.Black;
                }
                else
                {
                    if (i == IndexSelected && "B" == LineSelected)
                    {
                        lblLineBs[count].Background = Brushes.Red;
                        lblLineBs[count].Foreground = Brushes.White;
                    }
                    else
                    {
                        lblLineBs[count].Background = Brushes.Beige;
                        lblLineBs[count].Foreground = Brushes.Red;
                    }
                }

                ++count;
            }

            for (int i = count; i < 50; ++i)
            {
                lblLineAs[i].Content = "";
                lblLineBs[i].Content = "";

                lblLineAs[i].Background = Brushes.Beige;
                lblLineBs[i].Background = Brushes.Beige;
            }
        }

        private void DisplaySelectedValues3(int index)
        {
            // Review 데이터 에서 가져온다. 
            int last = dataService.DataResult.Review.ListRaw.Count;
            int count = 0;

            if (last >= index + 50)
                last = index + 50;

            for (int i = index; i < last; ++i)
            {
                lblLineAs[count].Content = dataService.DataResult.Review.ListRaw[i].Value1;
                lblLineBs[count].Content = dataService.DataResult.Review.ListRaw[i].Value2;
                lblLineCs[count].Content = dataService.DataResult.Review.ListRaw[i].Value3;

                if ("G" == lblLineAs[count].Content.ToString())
                {
                    lblLineAs[count].Background = Brushes.Beige;
                    lblLineAs[count].Foreground = Brushes.Black;
                }
                else
                {
                    if (i == IndexSelected && "A" == LineSelected)
                    {
                        lblLineAs[count].Background = Brushes.Red;
                        lblLineAs[count].Foreground = Brushes.White;
                    }
                    else
                    {
                        lblLineAs[count].Background = Brushes.Beige;
                        lblLineAs[count].Foreground = Brushes.Red;
                    }
                }

                if ("G" == lblLineBs[count].Content.ToString())
                {
                    lblLineBs[count].Background = Brushes.Beige;
                    lblLineBs[count].Foreground = Brushes.Black;
                }
                else
                {
                    if (i == IndexSelected && "B" == LineSelected)
                    {
                        lblLineBs[count].Background = Brushes.Red;
                        lblLineBs[count].Foreground = Brushes.White;
                    }
                    else
                    {
                        lblLineBs[count].Background = Brushes.Beige;
                        lblLineBs[count].Foreground = Brushes.Red;
                    }
                }

                if ("G" == lblLineCs[count].Content.ToString())
                {
                    lblLineCs[count].Background = Brushes.Beige;
                    lblLineCs[count].Foreground = Brushes.Black;
                }
                else
                {
                    if (i == IndexSelected && "C" == LineSelected)
                    {
                        lblLineCs[count].Background = Brushes.Red;
                        lblLineCs[count].Foreground = Brushes.White;
                    }
                    else
                    {
                        lblLineCs[count].Background = Brushes.Beige;
                        lblLineCs[count].Foreground = Brushes.Red;
                    }
                }

                ++count;
            }

            for (int i = count; i < 50; ++i)
            {
                lblLineAs[i].Content = "";
                lblLineBs[i].Content = "";
                lblLineCs[i].Content = "";

                lblLineAs[i].Background = Brushes.Beige;
                lblLineBs[i].Background = Brushes.Beige;
                lblLineCs[i].Background = Brushes.Beige;
            }
        }

        // 선택된 이미지를 보여준다. 
        private void DisplaySelectedImages(int index, string line, int viewType = 0)
        {
            int delay = 10;

            DateTime start = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 3000);
            DateTime timeout = start.Add(duration);

            while (true == IsDrawing)
            {
                ThreadDoEvents(10);
                delay = 100;

                if (timeout < DateTime.Now)
                    break;
            }

            IsDrawing = true;

            ClearImages();
            
            start = DateTime.Now;
            duration = new TimeSpan(0, 0, 0, 0, 500);
            timeout = start.Add(duration);

            while (false == IsClearedImages)
            {
                if (DateTime.Now > timeout)
                    break;
                System.Windows.Forms.Application.DoEvents();
            }

            ThreadDoEvents(delay);

            //System.Diagnostics.Debug.WriteLine("DisplaySelectedImages {0}, {1}", index, line);

            DisplayNGImageTop(index, line, viewType);
            DisplayNGImageBottom(index, line, viewType);
            DisplayNGImageMono(index, line, viewType);

            IsDrawing = false;
        }
        #endregion
    }

    class NGData
    {
        public int NgNo { get; set; }
        public int NgIndex { get; set; }
        public string NgLine { get; set; }
        public string NgName { get; set; }

        public NGData(int no, int index, string line, string name)
        {
            NgNo = no;
            NgIndex = index;
            NgLine = line;
            NgName = name;
        }
    }
}
