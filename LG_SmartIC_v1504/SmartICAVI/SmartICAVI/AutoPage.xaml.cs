using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using HalconDotNet;

namespace SmartICAVI
{
    /// <summary>
    /// AutoPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AutoPage : Page
    {
        #region Variables
        private HWindowWPF hWPF = null;
        private HWindow hWindow = null;
        private SystemService sysService = null;
        private SequenceService seqService = null;
        private GrabService grabService = null;
        private DataService dataService = null;
        private ReviewService reviewService = null;
        private MsgService msgService = null;
        private DioService dioService = null;
       

        private VisionTopService topVision = null;
        private VisionTop2Service top2Vision = null;
        private VisionBottomService bottomVision = null;
        private VisionBottom2Service bottom2Vision = null;
        private VisionMonoService monoVision = null;
        private VisionMono2Service mono2Vision = null;

        private VisionCamService camService = null;

        private JobDefectInfoWindow winDefectInfo = null;

        private DispatcherTimer timer = null;
        private DispatcherTimer timer2 = null;

        private int countTop;
        private int countBottom;
        private int countMono;
        private int countPunch;

        private Label[] lblTopTitles;
        private Label[] lblTopValue1s;
        private Label[] lblTopValue2s;
        private Label[] lblTopValue3s;
        private TextBlock[] tblTopTitles;

        private Label[] lblBottomTitles;
        private Label[] lblBottomValue1s;
        private Label[] lblBottomValue2s;
        private Label[] lblBottomValue3s;
        private TextBlock[] tblBottomTitles;

        private Label[] lblMonoTitles;
        private Label[] lblMonoValue1s;
        private Label[] lblMonoValue2s;
        private Label[] lblMonoValue3s;
        private TextBlock[] tblMonoTitles;

        private Label[] lblPunchTitles;
        private Label[] lblPunchValue1s;
        private Label[] lblPunchValue2s;
        private Label[] lblPunchValue3s;
        //private Label[] lblPunchTopValue1s;
        //private Label[] lblPunchTopValue2s;
        //private Label[] lblPunchBottomValue1s;
        //private Label[] lblPunchBottomValue2s;
        //private Label[] lblPunchMonoValue1s;
        //private Label[] lblPunchMonoValue2s;
        private TextBlock[] tblPunchTitles;
        #endregion

        private string TextBoxOldValue { get; set; }

        private int IndexPunch { get; set; }

        private bool IsTimerOn { get; set; }

        static bool InvokeRequired
        {
            get { return Dispatcher.CurrentDispatcher != Application.Current.Dispatcher; }
        }

        public AutoPage()
        {
            InitializeComponent();

            countTop = 0;
            countBottom = 0;
            countMono = 0;
            countPunch = 0;

            IndexPunch = -1;

            // Top
            lblTopTitles = new Label[15] {
                lblTopLineNo01, lblTopLineNo02, lblTopLineNo03, lblTopLineNo04, lblTopLineNo05,
                lblTopLineNo06, lblTopLineNo07, lblTopLineNo08, lblTopLineNo09, lblTopLineNo10,

                lblTopLineNo11, lblTopLineNo12, lblTopLineNo13, lblTopLineNo14, lblTopLineNo15
            };

            lblTopValue1s = new Label[15]{
                lblTopLine101, lblTopLine102, lblTopLine103, lblTopLine104, lblTopLine105,
                lblTopLine106, lblTopLine107, lblTopLine108, lblTopLine109, lblTopLine110,

                lblTopLine111, lblTopLine112, lblTopLine113, lblTopLine114, lblTopLine115
            };

            lblTopValue2s = new Label[15]{
                lblTopLine201, lblTopLine202, lblTopLine203, lblTopLine204, lblTopLine205,
                lblTopLine206, lblTopLine207, lblTopLine208, lblTopLine209, lblTopLine210,

                lblTopLine211, lblTopLine212, lblTopLine213, lblTopLine214, lblTopLine215
            };

            lblTopValue3s = new Label[15]{
                lblTopLine301, lblTopLine302, lblTopLine303, lblTopLine304, lblTopLine305,
                lblTopLine306, lblTopLine307, lblTopLine308, lblTopLine309, lblTopLine310,

                lblTopLine311, lblTopLine312, lblTopLine313, lblTopLine314, lblTopLine315
            };

            tblTopTitles = new TextBlock[15];

            for (int i = 0; i < 15; ++i)
            {
                tblTopTitles[i] = new TextBlock();
                tblTopTitles[i].TextWrapping = TextWrapping.Wrap;

                tblTopTitles[i].Text = string.Format("{0:00}", i + 1);

                lblTopTitles[i].Content = tblTopTitles[i];
            }

            // Bottom
            lblBottomTitles = new Label[15] {
                lblBottomLineNo01, lblBottomLineNo02, lblBottomLineNo03, lblBottomLineNo04, lblBottomLineNo05,
                lblBottomLineNo06, lblBottomLineNo07, lblBottomLineNo08, lblBottomLineNo09, lblBottomLineNo10,

                lblBottomLineNo11, lblBottomLineNo12, lblBottomLineNo13, lblBottomLineNo14, lblBottomLineNo15
            };

            lblBottomValue1s = new Label[15]{
                lblBottomLine101, lblBottomLine102, lblBottomLine103, lblBottomLine104, lblBottomLine105,
                lblBottomLine106, lblBottomLine107, lblBottomLine108, lblBottomLine109, lblBottomLine110,

                lblBottomLine111, lblBottomLine112, lblBottomLine113, lblBottomLine114, lblBottomLine115
            };

            lblBottomValue2s = new Label[15]{
                lblBottomLine201, lblBottomLine202, lblBottomLine203, lblBottomLine204, lblBottomLine205,
                lblBottomLine206, lblBottomLine207, lblBottomLine208, lblBottomLine209, lblBottomLine210,

                lblBottomLine211, lblBottomLine212, lblBottomLine213, lblBottomLine214, lblBottomLine215
            };

            lblBottomValue3s = new Label[15]{
                lblBottomLine301, lblBottomLine302, lblBottomLine303, lblBottomLine304, lblBottomLine305,
                lblBottomLine306, lblBottomLine307, lblBottomLine308, lblBottomLine309, lblBottomLine310,

                lblBottomLine311, lblBottomLine312, lblBottomLine313, lblBottomLine314, lblBottomLine315
            };

            tblBottomTitles = new TextBlock[15];

            for (int i = 0; i < 15; ++i)
            {
                tblBottomTitles[i] = new TextBlock();
                tblBottomTitles[i].TextWrapping = TextWrapping.Wrap;

                tblBottomTitles[i].Text = string.Format("{0:00}", i + 1);

                lblBottomTitles[i].Content = tblBottomTitles[i];
            }


            // Mono
            lblMonoTitles = new Label[15] {
                lblMonoLineNo01, lblMonoLineNo02, lblMonoLineNo03, lblMonoLineNo04, lblMonoLineNo05,
                lblMonoLineNo06, lblMonoLineNo07, lblMonoLineNo08, lblMonoLineNo09, lblMonoLineNo10,

                lblMonoLineNo11, lblMonoLineNo12, lblMonoLineNo13, lblMonoLineNo14, lblMonoLineNo15
            };

            lblMonoValue1s = new Label[15]{
                lblMonoLine101, lblMonoLine102, lblMonoLine103, lblMonoLine104, lblMonoLine105,
                lblMonoLine106, lblMonoLine107, lblMonoLine108, lblMonoLine109, lblMonoLine110,

                lblMonoLine111, lblMonoLine112, lblMonoLine113, lblMonoLine114, lblMonoLine115
            };

            lblMonoValue2s = new Label[15]{
                lblMonoLine201, lblMonoLine202, lblMonoLine203, lblMonoLine204, lblMonoLine205,
                lblMonoLine206, lblMonoLine207, lblMonoLine208, lblMonoLine209, lblMonoLine210,

                lblMonoLine211, lblMonoLine212, lblMonoLine213, lblMonoLine214, lblMonoLine215
            };

            lblMonoValue3s = new Label[15]{
                lblMonoLine301, lblMonoLine302, lblMonoLine303, lblMonoLine304, lblMonoLine305,
                lblMonoLine306, lblMonoLine307, lblMonoLine308, lblMonoLine309, lblMonoLine310,

                lblMonoLine311, lblMonoLine312, lblMonoLine313, lblMonoLine314, lblMonoLine315
            };

            tblMonoTitles = new TextBlock[15];

            for (int i = 0; i < 15; ++i)
            {
                tblMonoTitles[i] = new TextBlock();
                tblMonoTitles[i].TextWrapping = TextWrapping.Wrap;

                tblMonoTitles[i].Text = string.Format("{0:00}", i + 1);

                lblMonoTitles[i].Content = tblMonoTitles[i];
            }


            // Punch
            lblPunchTitles = new Label[45] {
                lblPunchLineNo01, lblPunchLineNo02, lblPunchLineNo03, lblPunchLineNo04, lblPunchLineNo05,
                lblPunchLineNo06, lblPunchLineNo07, lblPunchLineNo08, lblPunchLineNo09, lblPunchLineNo10,

                lblPunchLineNo11, lblPunchLineNo12, lblPunchLineNo13, lblPunchLineNo14, lblPunchLineNo15,
                lblPunchLineNo16, lblPunchLineNo17, lblPunchLineNo18, lblPunchLineNo19, lblPunchLineNo20,

                lblPunchLineNo21, lblPunchLineNo22, lblPunchLineNo23, lblPunchLineNo24, lblPunchLineNo25,
                lblPunchLineNo26, lblPunchLineNo27, lblPunchLineNo28, lblPunchLineNo29, lblPunchLineNo30,

                lblPunchLineNo31, lblPunchLineNo32, lblPunchLineNo33, lblPunchLineNo34, lblPunchLineNo35,
                lblPunchLineNo36, lblPunchLineNo37, lblPunchLineNo38, lblPunchLineNo39, lblPunchLineNo40,

                lblPunchLineNo41, lblPunchLineNo42, lblPunchLineNo43, lblPunchLineNo44, lblPunchLineNo45
            };

            lblPunchValue1s = new Label[45]{
                lblPunchLine101, lblPunchLine102, lblPunchLine103, lblPunchLine104, lblPunchLine105,
                lblPunchLine106, lblPunchLine107, lblPunchLine108, lblPunchLine109, lblPunchLine110,

                lblPunchLine111, lblPunchLine112, lblPunchLine113, lblPunchLine114, lblPunchLine115,
                lblPunchLine116, lblPunchLine117, lblPunchLine118, lblPunchLine119, lblPunchLine120,

                lblPunchLine121, lblPunchLine122, lblPunchLine123, lblPunchLine124, lblPunchLine125,
                lblPunchLine126, lblPunchLine127, lblPunchLine128, lblPunchLine129, lblPunchLine130,

                lblPunchLine131, lblPunchLine132, lblPunchLine133, lblPunchLine134, lblPunchLine135,
                lblPunchLine136, lblPunchLine137, lblPunchLine138, lblPunchLine139, lblPunchLine140,

                lblPunchLine141, lblPunchLine142, lblPunchLine143, lblPunchLine144, lblPunchLine145,
            };

            lblPunchValue2s = new Label[45]{
                lblPunchLine201, lblPunchLine202, lblPunchLine203, lblPunchLine204, lblPunchLine205,
                lblPunchLine206, lblPunchLine207, lblPunchLine208, lblPunchLine209, lblPunchLine210,

                lblPunchLine211, lblPunchLine212, lblPunchLine213, lblPunchLine214, lblPunchLine215,
                lblPunchLine216, lblPunchLine217, lblPunchLine218, lblPunchLine219, lblPunchLine220,

                lblPunchLine221, lblPunchLine222, lblPunchLine223, lblPunchLine224, lblPunchLine225,
                lblPunchLine226, lblPunchLine227, lblPunchLine228, lblPunchLine229, lblPunchLine230,

                lblPunchLine231, lblPunchLine232, lblPunchLine233, lblPunchLine234, lblPunchLine235,
                lblPunchLine236, lblPunchLine237, lblPunchLine238, lblPunchLine239, lblPunchLine240,

                lblPunchLine241, lblPunchLine242, lblPunchLine243, lblPunchLine244, lblPunchLine245,
            };

            lblPunchValue3s = new Label[45]{
                lblPunchLine301, lblPunchLine302, lblPunchLine303, lblPunchLine304, lblPunchLine305,
                lblPunchLine306, lblPunchLine307, lblPunchLine308, lblPunchLine309, lblPunchLine310,

                lblPunchLine311, lblPunchLine312, lblPunchLine313, lblPunchLine314, lblPunchLine315,
                lblPunchLine316, lblPunchLine317, lblPunchLine318, lblPunchLine319, lblPunchLine320,

                lblPunchLine321, lblPunchLine322, lblPunchLine323, lblPunchLine324, lblPunchLine325,
                lblPunchLine326, lblPunchLine327, lblPunchLine328, lblPunchLine329, lblPunchLine330,

                lblPunchLine331, lblPunchLine332, lblPunchLine333, lblPunchLine334, lblPunchLine335,
                lblPunchLine336, lblPunchLine337, lblPunchLine338, lblPunchLine339, lblPunchLine340,

                lblPunchLine341, lblPunchLine342, lblPunchLine343, lblPunchLine344, lblPunchLine345,
            };

            tblPunchTitles = new TextBlock[45];

            for (int i = 0; i < 45; ++i)
            {
                tblPunchTitles[i] = new TextBlock();
                tblPunchTitles[i].TextWrapping = TextWrapping.Wrap;

                tblPunchTitles[i].Text = string.Format("{0:00}", i + 1);

                lblPunchTitles[i].Content = tblPunchTitles[i];
            }

            cbNextMachine.Items.Add("자동편집(TSET00)");   // 자동편집
            cbNextMachine.Items.Add("가편집(TSET01)");     // 가편집
            cbNextMachine.Items.Add("PI면검사(TSSG00)");   // PI면 검사
            cbNextMachine.Items.Add("회로면검사(TSCG00)"); // 회로면 검사
            cbNextMachine.Items.Add("AFVI(TSAI02)");      // AFVI
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            sysService = SystemService.Singleton;
            seqService = SequenceService.Singleton;
            grabService = GrabService.Singleton;
            dataService = DataService.Singleton;
            reviewService = ReviewService.Singleton;
            msgService = MsgService.Singleton;
            dioService = DioService.Singleton;

            sysService.Mode = SystemService.Modes.auto;

            topVision = VisionTopService.Singleton;
            top2Vision = VisionTop2Service.Singleton;
            bottomVision = VisionBottomService.Singleton;
            bottom2Vision = VisionBottom2Service.Singleton;
            monoVision = VisionMonoService.Singleton;
            mono2Vision = VisionMono2Service.Singleton;

            camService = VisionCamService.Singleton;

            if( null != sysService )
                sysService.EventState += OnEventState;

            if (null != seqService)
            {
                seqService.EventPunch += OnEventPunch;
                seqService.EventAutoJog += OnEventAutoJog;
            }

            dataService.EventChangedRecipe += OnEventChangedRecipe;
   
            switch (dataService.DataSystem.JobType)
            {
                case 0:
                    cbxNormalJob.IsChecked = true;
                    cbxTotalCount.IsChecked = false;
                    cbxGoodCount.IsChecked = false;
                    tblStart.Text = "연속 작업시작";
                    break;
                case 1:
                    cbxNormalJob.IsChecked = false;
                    cbxTotalCount.IsChecked = true;
                    cbxGoodCount.IsChecked = false;
                    tblStart.Text = "Total 작업시작";
                    break;
                case 2:
                    cbxNormalJob.IsChecked = false;
                    cbxTotalCount.IsChecked = false;
                    cbxGoodCount.IsChecked = true;
                    tblStart.Text = "Good 작업시작";
                    break;
                default:
                    cbxNormalJob.IsChecked = true;
                    cbxTotalCount.IsChecked = false;
                    cbxGoodCount.IsChecked = false;
                    tblStart.Text = "연속 작업시작";
                    break;
            }

            tbxLotID.Text = dataService.DataSystem.LotID;
            tbxUserID.Text = dataService.DataSystem.UserID;
            tbxToolID.Text = dataService.DataSystem.ToolID;
            tbxTotalCount.Text = dataService.DataSystem.TotalCount.ToString();
            tbxGoodCount.Text = dataService.DataSystem.GoodCount.ToString();
            tbxPFHole.Text = dataService.DataRecipe.PF.ToString();
            tbxNGContinue.Text = dataService.DataRecipe.NGContinue.ToString();
            tbxSectionMinUnits.Text = dataService.DataRecipe.SectionMinUnits.ToString();

            cbxVerify.IsChecked = dataService.DataSystem.IsSelectedModify;

            cbxYield.IsChecked = dataService.DataSystem.IsSelectedSectionYield;

            // Top Summary
            if( true == dataService.DataSystem.IsSelectedTop )
            {
                lblTotalCountSection1.Background = Brushes.Olive;
                lblGoodCountSection1.Background = Brushes.Olive;
                lblNGCountSection1.Background = Brushes.Olive;
                lblYieldSection1.Background = Brushes.Olive;
                lblJointCountSection1.Background = Brushes.Olive;

                tblTopVision.Background = Brushes.Olive;
                tblTopVision.Text = "TOP";
            }
            else
            {
                lblTotalCountSection1.Background = Brushes.Gray;
                lblGoodCountSection1.Background = Brushes.Gray;
                lblNGCountSection1.Background = Brushes.Gray;
                lblYieldSection1.Background = Brushes.Gray;
                lblJointCountSection1.Background = Brushes.Gray;

                tblTopVision.Background = Brushes.Gray;
                tblTopVision.Text = "TOP OFF";
            }
            
            // Bottom Summary
            if( true == dataService.DataSystem.IsSelectedBottom )
            {
                lblTotalCountSection2.Background = Brushes.Olive;
                lblGoodCountSection2.Background = Brushes.Olive;
                lblNGCountSection2.Background = Brushes.Olive;
                lblYieldSection2.Background = Brushes.Olive;
                lblJointCountSection2.Background = Brushes.Olive;

                tblBottomVision.Background = Brushes.Olive;
                tblBottomVision.Text = "BOTTOM";
            }
            else
            {
                lblTotalCountSection2.Background = Brushes.Gray;
                lblGoodCountSection2.Background = Brushes.Gray;
                lblNGCountSection2.Background = Brushes.Gray;
                lblYieldSection2.Background = Brushes.Gray;
                lblJointCountSection2.Background = Brushes.Gray;

                tblBottomVision.Background = Brushes.Gray;
                tblBottomVision.Text = "BOTTOM OFF";
            }

            // Mono Summary
            if (true == dataService.DataSystem.IsSelectedMono)
            {
                lblTotalCountSection3.Background = Brushes.Olive;
                lblGoodCountSection3.Background = Brushes.Olive;
                lblNGCountSection3.Background = Brushes.Olive;
                lblYieldSection3.Background = Brushes.Olive;
                lblJointCountSection3.Background = Brushes.Olive;

                tblMonoVision.Background = Brushes.Olive;
                tblMonoVision.Text = "MONO";
            }
            else
            {
                lblTotalCountSection3.Background = Brushes.Gray;
                lblGoodCountSection3.Background = Brushes.Gray;
                lblNGCountSection3.Background = Brushes.Gray;
                lblYieldSection3.Background = Brushes.Gray;
                lblJointCountSection3.Background = Brushes.Gray;

                tblMonoVision.Background = Brushes.Gray;
                tblMonoVision.Text = "MONO OFF";
            }

            // Punch
            if (true == dataService.DataSystem.IsSelectedPunch)
            {
                tblPunch.Background = Brushes.Olive;
                tblPunch.Text = "PUNCH";

                // Punch Align
                if (true == dataService.DataSystem.IsSelectedPunchInspect)
                {
                    tblAlignVision.Background = Brushes.Olive;
                    tblAlignVision.Text = "PUNCH ALIGN";
                }
                else
                {
                    tblAlignVision.Background = Brushes.Gray;
                    tblAlignVision.Text = "PUNCH ALIGN ON / INSPECT OFF";
                }
            }
            else
            {
                tblPunch.Background = Brushes.Gray;
                tblPunch.Text = "PUNCH OFF";

                tblAlignVision.Background = Brushes.Gray;
                tblAlignVision.Text = "PUNCH ALIGN OFF";
            }

            lblLightTime1.Content = "0";
            lblLightTime2.Content = "0";
            lblLightTime3.Content = "0";
            
            //다음 공정 Node ID ( TSSF00, TSWG00, TSET00, TSAI00, TSQC00 )
            //cbNextMachine.SelectedItem = dataService.DataSystem.NextProcess;
            // TSET00 추가 2024.11.20
            if      ("TSET00" == dataService.DataSystem.NextProcess) cbNextMachine.SelectedIndex = 0;
            else if ("TSET01" == dataService.DataSystem.NextProcess) cbNextMachine.SelectedIndex = 1;
            else if ("TSSG00" == dataService.DataSystem.NextProcess) cbNextMachine.SelectedIndex = 2;
            else if ("TSCG00" == dataService.DataSystem.NextProcess) cbNextMachine.SelectedIndex = 3;
            else                                                     cbNextMachine.SelectedIndex = 4;

            CheckNetworkDrive();

            OnEventState(this, null);

            SetTimer2();

            if ("EXTERN" == dataService.DataSystem.CamType)
            {
                camService.SendMode("INSPECT");
                canvas.Visibility = Visibility.Hidden;
            }
            else
            {
                hWPF = new HWindowWPF((int)canvas.ActualWidth, (int)canvas.ActualHeight);
                canvas.Children.Add(hWPF);
                hWindow = hWPF.HalconWindow;

                HOperatorSet.SetPart(hWindow, 0, 0, (HTuple)grabService.Height, (HTuple)grabService.Width);

                seqService.SetHWindow(hWindow);
            }

            lblLineValue.Content = dataService.DataRecipe.Line.ToString();

            // 3Line
            if (3 == dataService.DataRecipe.Line)
            {
                for (int i = 0; i < lblTopValue3s.Length; ++i)
                {
                    lblTopValue3s[i].Visibility = System.Windows.Visibility.Visible;
                    lblBottomValue3s[i].Visibility = System.Windows.Visibility.Visible;
                    lblMonoValue3s[i].Visibility = System.Windows.Visibility.Visible;
                }

                for (int i = 0; i < lblPunchValue1s.Length; ++i)
                {
                    lblPunchValue3s[i].Visibility = System.Windows.Visibility.Visible;
                }
            }
            else
            {
                for (int i = 0; i < lblTopValue3s.Length; ++i)
                {
                    lblTopValue3s[i].Visibility = System.Windows.Visibility.Hidden;
                    lblBottomValue3s[i].Visibility = System.Windows.Visibility.Hidden;
                    lblMonoValue3s[i].Visibility = System.Windows.Visibility.Hidden;
                }

                for (int i = 0; i < lblPunchValue1s.Length; ++i)
                {
                    lblPunchValue3s[i].Visibility = System.Windows.Visibility.Hidden;
                }
            }
           

            this.Focus();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (null != camService)
            {
                camService.SendMode("MANUAL");
            }

            dataService.EventChangedRecipe -= OnEventChangedRecipe;

            ResetJog();

            dataService.DataSystem.LotID = tbxLotID.Text;
            dataService.DataSystem.UserID = tbxUserID.Text;
            dataService.DataSystem.ToolID = tbxToolID.Text;
            dataService.DataRecipe.PF = int.Parse(tbxPFHole.Text);
            dataService.DataRecipe.SectionMinUnits = int.Parse(tbxSectionMinUnits.Text);

            // TSET00 추가 2024.11.20
            if      (0 == cbNextMachine.SelectedIndex) { dataService.DataSystem.NextProcess = "TSET00"; }  // 자동편집
            else if (1 == cbNextMachine.SelectedIndex) { dataService.DataSystem.NextProcess = "TSET01"; }  // 가편집
            else if (2 == cbNextMachine.SelectedIndex) { dataService.DataSystem.NextProcess = "TSSG00"; }  // PI면 검사
            else if (3 == cbNextMachine.SelectedIndex) { dataService.DataSystem.NextProcess = "TSCG00"; }  // 회로면 검사
            else                                       { dataService.DataSystem.NextProcess = "TSAI02"; }  // AFVI

            dataService.DataSystem.Save();

            dataService.DataRecipe.NGContinue = int.Parse(tbxNGContinue.Text);
            dataService.DataRecipe.PF = int.Parse(tbxPFHole.Text);

            dataService.DataRecipe.Save();
            

            ResetTimer2();

            ResetTimer();

            if( null != seqService )
                seqService.EventPunch -= OnEventPunch;

            if (null != sysService)
            {
                sysService.EventState -= OnEventState;
                seqService.EventAutoJog -= OnEventAutoJog;
            }

            if (null != hWindow)
                hWindow.Dispose();

            if (null != hWPF)
                hWPF.Dispose();
            
            GC.Collect();
        }

        private void Page_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (true == this.IsFocused)
            {
                if( null != camService )
                    camService.SendMode("TOPMOST", 1);
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;
            dioService.ResetOutport((int)EnumSmartIC.Outports.laserOn);
            GC.Collect();

            if (null != cbx)
            {
                Log_Trace.WriteLine("AutoPage.btnStart_Click({0})", cbx.IsChecked);

                if (cbx.IsChecked.Value)
                {
                    if (!dataService.DataSystem.FirstIndexPause)
                    {
                        ResetJog();

                        if (0 != CheckNetworkDrive())
                        {
                            sysService.State = SystemService.States.stop;
                            msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.networkDrive);
                            cbx.IsChecked = false;
                            return;
                        }
                    }

                    string lotID = tbxLotID.Text;
                    string userID = tbxUserID.Text;
                    string pfHole = tbxPFHole.Text;

                    lotID = lotID.ToUpper();
                    userID = userID.ToUpper();

                    if (dataService.DataSystem.FirstIndexPause == true)
                    {

                    }
                    else
                    {
                        if (0 != CheckLotID(lotID))
                        {
                            sysService.State = SystemService.States.stop;
                            return;
                        }
                    }

                    tbxLotID.Text = lotID;
                    tbxUserID.Text = userID;

                    dataService.DataSystem.LotID = tbxLotID.Text;
                    dataService.DataSystem.UserID = tbxUserID.Text;
                    dataService.DataSystem.ToolID = tbxToolID.Text;

                    dataService.DataRecipe.NGContinue = int.Parse(tbxNGContinue.Text);
                    dataService.DataRecipe.PF = int.Parse(tbxPFHole.Text);
                    dataService.DataRecipe.SectionMinUnits = int.Parse(tbxSectionMinUnits.Text);
                    dataService.DataSystem.IsSelectedSectionYield = (bool)cbxYield.IsChecked;

                    // TSET00 추가 2024.11.20
                    if      (0 == cbNextMachine.SelectedIndex) { dataService.DataSystem.NextProcess = "TSET00"; }  // 자동편집
                    else if (1 == cbNextMachine.SelectedIndex) { dataService.DataSystem.NextProcess = "TSET01"; }  // 가편집
                    else if (2 == cbNextMachine.SelectedIndex) { dataService.DataSystem.NextProcess = "TSSG00"; }  // PI면 검사
                    else if (3 == cbNextMachine.SelectedIndex) { dataService.DataSystem.NextProcess = "TSCG00"; }  // 회로면 검사
                    else                                       { dataService.DataSystem.NextProcess = "TSAI02"; }  // AFVI

                    topVision.SetLoad(lotID, userID, pfHole);
                    top2Vision.SetLoad(lotID, userID, pfHole);
                    bottomVision.SetLoad(lotID, userID, pfHole);
                    bottom2Vision.SetLoad(lotID, userID, pfHole);
                    monoVision.SetLoad(lotID, userID, pfHole);
                    mono2Vision.SetLoad(lotID, userID, pfHole);

                    if (true == cbxNormalJob.IsChecked)
                        dataService.DataSystem.JobType = 0;
                    else if (true == cbxTotalCount.IsChecked)
                    {
                        dataService.DataSystem.JobType = 1;
                        dataService.DataSystem.TotalCount = int.Parse(tbxTotalCount.Text);
                    }
                    else if (true == cbxGoodCount.IsChecked)
                    {
                        dataService.DataSystem.JobType = 2;
                        dataService.DataSystem.GoodCount = int.Parse(tbxGoodCount.Text);
                    }

                    countTop = -1;
                    countBottom = -1;
                    countMono = -1;
                    countPunch = -1;

                    IndexPunch = -1;

                    dataService.IsEndTop = false;
                    dataService.IndexEndTop = 0;
                    dataService.IsEndBottom = false;
                    dataService.IndexEndBottom = 0;
                    dataService.IsEndMono = false;

                    if (dataService.DataSystem.FirstIndexPause == true)
                    {
                        dataService.DataSystem.FirstIndexPause = false;

                        System.Windows.Forms.Application.DoEvents();


                        sysService.State = SystemService.States.resume;

                        SetTimer();

                        btnPause.IsChecked = false;
                    }
                    else
                    {
                        if (false == seqService.IsOnlineMode)
                        {
                            // Show Offline Message
                            if (false == msgService.ShowMessage((int)EnumSmartIC.LightAlarms.jobOffline, false))
                            {
                                cbx.IsChecked = false;
                                sysService.State = SystemService.States.stop;
                                return;
                            }
                        }

                        dataService.DataSystem.Save();
                        dataService.DataRecipe.Save();

                        System.Windows.Forms.Application.DoEvents();


                        sysService.State = SystemService.States.run;

                        SetTimer();
                    }


                }
                else
                {
                    if (SystemService.States.pause == sysService.State)
                    {
                        cbx.IsChecked = true;
                        if (0 == seqService.CheckMachine())
                        {
                            seqService.SetSequence((int)EnumSmartIC.Sequences.backFeeding);
                            sysService.State = SystemService.States.resume;
                        }
                    }
                }
            }
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;

            if (dataService.DataSystem.FirstIndexPause == true)
            {
                return;
            }


            if (null != cbx)
            {
                Log_Trace.WriteLine("AutoPage.btnPause_Click({0})", cbx.IsChecked);

                if (true == cbx.IsChecked)
                {
                    sysService.State = SystemService.States.pause;
                    GC.Collect();
                }
                else
                {
                    if (0 == seqService.CheckMachine())
                    {
                        seqService.SetSequence((int)EnumSmartIC.Sequences.backFeeding);

                        dataService.DataRecipe.SectionMinUnits = int.Parse(tbxSectionMinUnits.Text);

                        sysService.State = SystemService.States.resume;

                    }
                    else
                    {
                        cbx.IsChecked = true;
                    }
                }
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {

            CheckBox cbx = sender as CheckBox;
            dioService.ResetOutport((int)EnumSmartIC.Outports.laserOn);
            if (null != cbx)
            {
                Log_Trace.WriteLine("AutoPage.btnPause_Click({0})", cbx.IsChecked);

                // Run 상태일 경우에는 Pause 를 만들고, Abort 처리 여부를 사용자 확인 후 진행한다. 
                if ((SystemService.States.run == sysService.State) || (SystemService.States.pause == sysService.State))
                {
                    if (null != msgService)
                    {
                        btnStart.IsChecked = true;
                        btnPause.IsChecked = true;
                        // Set Pause
                        sysService.State = SystemService.States.pause;

                        System.Windows.Forms.Application.DoEvents();

                        // Show Job Abort Message
                        if (true == msgService.ShowMessage((int)EnumSmartIC.LightAlarms.jobAbort, false))
                        {
                            ResetTimer();
                            sysService.State = SystemService.States.stop;

                            btnStart.IsChecked = false;
                            btnPause.IsChecked = false;
                            dataService.DataSystem.FirstIndexPause = false;
                        }
                        else
                        {
                            btnStop.IsChecked = false;
                        }
                    }
                    else
                    {
                        if (true == cbx.IsChecked)
                        {
                            ResetTimer();
                            sysService.State = SystemService.States.stop;
                        }
                        else
                            sysService.State = SystemService.States.reset;
                    }
                }
                else
                {
                    if (true == cbx.IsChecked)
                    {
                        ResetTimer();
                        sysService.State = SystemService.States.stop;
                    }
                    else
                        sysService.State = SystemService.States.reset;
                }
            }

            GC.Collect();
        }

        private void cbxLotID_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;

            if (null != cbx)
            {
                cbx.IsChecked = false;

                tbxLotID.Focus();
                tbxLotID.SelectAll();
            }
        }

        private void cbxUserID_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;

            if (null != cbx)
            {
                cbx.IsChecked = false;

                tbxUserID.Focus();
                tbxUserID.SelectAll();
            }
        }

        private void cbxNGContinue_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;

            if (null != cbx)
            {
                Log_Trace.WriteLine("AutoPage.cbxNGContinue_Click({0})", cbx.IsChecked);

                cbx.IsChecked = false;

                if (null != dataService)
                {
                    try
                    {
                        int count = int.Parse(tbxNGContinue.Text);
                        dataService.DataRecipe.NGContinue = count;
                    }
                    catch (Exception exc)
                    {
                        Log_Exception.WriteLine("AutoPage.cbxNGContinue_Click() : " + exc.Message);
                    }
                }
            }
        }

        private void cbNextMachine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //dataService.DataSystem.NextProcess = cbNextMachine.SelectedItem.ToString();
            // TSET00 추가 2024.11.20
            if      (0 == cbNextMachine.SelectedIndex) { dataService.DataSystem.NextProcess = "TSET00"; }  // 자동편집
            else if (1 == cbNextMachine.SelectedIndex) { dataService.DataSystem.NextProcess = "TSET01"; }  // 가편집
            else if (2 == cbNextMachine.SelectedIndex) { dataService.DataSystem.NextProcess = "TSSG00"; }  // PI면 검사
            else if (3 == cbNextMachine.SelectedIndex) { dataService.DataSystem.NextProcess = "TSCG00"; }  // 회로면 검사
            else                                       { dataService.DataSystem.NextProcess = "TSAI02"; }  // AFVI

            Log_Trace.WriteLine("AutoPage.cbNextMachine_SelectionChanged({0})", cbNextMachine.SelectedItem.ToString());
        }


        private void tbx_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            TextBoxOldValue = textBox.Text;

            //VirtualKeyboardService.GetSingleton().FireVirtualKeyboard(sender);
        }

        private void tbxNumber_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            TextBoxOldValue = textBox.Text;

            //VirtualKeyboardService.GetSingleton().FireVirtualKeyNumber(sender);
        }

        private void tbx_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void SetTimer()
        {
            IsTimerOn = false;

            timer = new DispatcherTimer();
            timer.Interval = new System.TimeSpan(0, 0, 0, 0, 100);
            timer.IsEnabled = true;
            timer.Tick += new EventHandler(timer_Tick);
        }

        private void ResetTimer()
        {
            IsTimerOn = false;

            if (null != timer)
            {
                timer.IsEnabled = false;
                timer.Tick -= timer_Tick;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (true == IsTimerOn)
                return;

            IsTimerOn = true;

            DisplayTop();
            DisplayBottom();
            DisplayMono();
            DisplayPunch();

            if (null != dataService)
            {
                cbxVerify.IsChecked = dataService.DataSystem.IsSelectedModify;
                tbxNGContinue.Text = dataService.DataRecipe.NGContinue.ToString();
            }

            IsTimerOn = false;
        }

        private void SetTimer2()
        {
            timer2 = new DispatcherTimer();
            timer2.Interval = new System.TimeSpan(0, 0, 0, 0, 1000);
            timer2.IsEnabled = true;
            timer2.Tick += new EventHandler(timer_Tick2);
        }

        private void ResetTimer2()
        {
            if (null != timer2)
            {
                timer2.IsEnabled = false;
                timer2.Tick -= timer_Tick;
            }
        }

        private void timer_Tick2(object sender, EventArgs e)
        {
            if (null != dataService)
            {
                if( cbxVerify.IsChecked != dataService.DataSystem.IsSelectedModify )
                    cbxVerify.IsChecked = dataService.DataSystem.IsSelectedModify;

                //lblLightTime1.Content = (dataService.DataSystem.LightTimeTop.TotalHours).ToString();
                //lblLightTime2.Content = (dataService.DataSystem.LightTimeBottom.TotalHours).ToString();
                //lblLightTime3.Content = (dataService.DataSystem.LightTimeMono.TotalHours).ToString();
            }
        }

        private void SetJog()
        {
            if (null != seqService)
            {
                if (false == seqService.IsFlag((int)EnumSmartIC.SeqFlags.autoJogRun))
                    seqService.SetSequence((int)EnumSmartIC.Sequences.autoJog, 1);

                //if( true == seqService.IsFlag((int)EnumSmartIC.SeqFlags.autoJogRun ) )
                //{
                //    btnStart.IsEnabled = false;
                //}
                //else
                //{
                //    btnStart.IsEnabled = true;
                //}
            }
        }

        private void ResetJog()
        {
            if (null != seqService)
            {
                seqService.SetSequence((int)EnumSmartIC.Sequences.autoJog, 0);
                //btnStart.IsEnabled = true;
            }
        }

        private void DisplayTop()
        {
            if (countTop != dataService.DataResult.Top.ListRaw.Count)
            {
                countTop = dataService.DataResult.Top.ListRaw.Count;

                if (countTop <= 15)
                {
                    for (int i = 0; i < 15; ++i)
                    {
                        DisplayTopValue(i, i);
                    }
                }
                else
                {
                    int index = 0;
                    for (int i = countTop-15; i < countTop; ++i)
                    {
                        DisplayTopValue(index, i);

                        ++index;
                    }
                }

                DisplayTopSummary();
            }
        }

        private void DisplayTopValue(int indexLabel, int indexValue)
        {
            try
            {
                tblTopTitles[indexLabel].Text = string.Format("{0}", indexValue + 1);

                if (indexValue < dataService.DataResult.Top.ListRaw.Count)
                {
                    lblTopValue1s[indexLabel].Content = dataService.DataResult.Top.ListRaw[indexValue].Value1;

                    switch (dataService.DataResult.Top.ListRaw[indexValue].Value1)
                    {
                        case "G":     // Good
                            lblTopValue1s[indexLabel].Background = Brushes.Beige;
                            break;
                        case "BB006":     // JOINT
                            lblTopValue1s[indexLabel].Background = Brushes.Goldenrod;
                            break;
                        case "C":     // NG Hole
                            lblTopValue1s[indexLabel].Background = Brushes.Cyan;
                            break;
                        case "BB039":     // Through Hole
                            lblTopValue1s[indexLabel].Background = Brushes.Blue;
                            break;
                        default:
                            lblTopValue1s[indexLabel].Background = Brushes.Red;
                            break;
                    }

                    lblTopValue2s[indexLabel].Content = dataService.DataResult.Top.ListRaw[indexValue].Value2;
                    switch (dataService.DataResult.Top.ListRaw[indexValue].Value2)
                    {
                        case "G":     // Good
                            lblTopValue2s[indexLabel].Background = Brushes.Beige;
                            break;
                        case "BB006":     // JOINT
                            lblTopValue2s[indexLabel].Background = Brushes.Goldenrod;
                            break;
                        case "C":     // NG Hole
                            lblTopValue2s[indexLabel].Background = Brushes.Cyan;
                            break;
                        case "BB039":     // T Hole
                            lblTopValue2s[indexLabel].Background = Brushes.Blue;
                            break;
                        default:
                            lblTopValue2s[indexLabel].Background = Brushes.Red;
                            break;
                    }

                    if (3 == dataService.DataRecipe.Line)
                    {
                        lblTopValue3s[indexLabel].Content = dataService.DataResult.Top.ListRaw[indexValue].Value3;
                        switch (dataService.DataResult.Top.ListRaw[indexValue].Value3)
                        {
                            case "G":     // Good
                                lblTopValue3s[indexLabel].Background = Brushes.Beige;
                                break;
                            case "BB006":     // JOINT
                                lblTopValue3s[indexLabel].Background = Brushes.Goldenrod;
                                break;
                            case "C":     // NG Hole
                                lblTopValue3s[indexLabel].Background = Brushes.Cyan;
                                break;
                            case "BB039":     // T Hole
                                lblTopValue3s[indexLabel].Background = Brushes.Blue;
                                break;
                            default:
                                lblTopValue3s[indexLabel].Background = Brushes.Red;
                                break;
                        }
                    }
                }
                else
                {
                    lblTopValue1s[indexLabel].Content = "";
                    lblTopValue1s[indexLabel].Background = Brushes.Beige;

                    lblTopValue2s[indexLabel].Content = "";
                    lblTopValue2s[indexLabel].Background = Brushes.Beige;

                    if (3 == dataService.DataRecipe.Line)
                    {
                        lblTopValue3s[indexLabel].Content = "";
                        lblTopValue3s[indexLabel].Background = Brushes.Beige;
                    }
                }
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine(string.Format("AutoPage.DisplayTopValue({0}, {1}) : ", indexLabel, indexValue) + exc.Message);
            }
        }

        private void DisplayTopSummary()
        {
            if (0 < dataService.DataResult.Top.ListRaw.Count)
            {
                double yield = ((double)dataService.DataResult.Top.CountGood / (double)dataService.DataResult.Top.CountTotal) * 100.0;
                
                lblTotalCountSection1.Content = dataService.DataResult.Top.CountTotal.ToString();
                lblGoodCountSection1.Content = dataService.DataResult.Top.CountGood.ToString();
                lblNGCountSection1.Content = (dataService.DataResult.Top.CountTotal - dataService.DataResult.Top.CountGood).ToString();
                lblYieldSection1.Content = yield.ToString("0.00");
                lblJointCountSection1.Content = dataService.DataResult.Top.CountJoint.ToString();
            }
            else
            {
                lblTotalCountSection1.Content = "";
                lblGoodCountSection1.Content = "";
                lblNGCountSection1.Content = "";
                lblYieldSection1.Content = "";
                lblJointCountSection1.Content = "";
            }
        }

        private void DisplayBottom()
        {
            if (countBottom != dataService.DataResult.Bottom.ListRaw.Count)
            {
                countBottom = dataService.DataResult.Bottom.ListRaw.Count;

                if (countBottom <= 15)
                {
                    for (int i = 0; i < 15; ++i)
                    {
                        DisplayBottomValue(i, i);
                    }
                }
                else
                {
                    int index = 0;
                    for (int i = countBottom - 15; i < countBottom; ++i)
                    {
                        DisplayBottomValue(index, i);

                        ++index;
                    }
                }

                DisplayBottomSummary();
            }
        }

        private void DisplayBottomValue(int indexLabel, int indexValue)
        {
            try
            {
                tblBottomTitles[indexLabel].Text = string.Format("{0}", indexValue + 1);

                if (indexValue < dataService.DataResult.Bottom.ListRaw.Count)
                {
                    lblBottomValue1s[indexLabel].Content = dataService.DataResult.Bottom.ListRaw[indexValue].Value1;

                    switch (dataService.DataResult.Bottom.ListRaw[indexValue].Value1)
                    {
                        case "G":     // Good
                            lblBottomValue1s[indexLabel].Background = Brushes.Beige;
                            break;
                        case "BB006":     // JOINT
                            lblBottomValue1s[indexLabel].Background = Brushes.Goldenrod;
                            break;
                        case "C":     // Hole
                            lblBottomValue1s[indexLabel].Background = Brushes.Cyan;
                            break;
                        case "BB039":     // T Hole
                            lblBottomValue1s[indexLabel].Background = Brushes.Blue;
                            break;
                        default:
                            lblBottomValue1s[indexLabel].Background = Brushes.Red;
                            break;
                    }

                    lblBottomValue2s[indexLabel].Content = dataService.DataResult.Bottom.ListRaw[indexValue].Value2;
                    switch (dataService.DataResult.Bottom.ListRaw[indexValue].Value2)
                    {
                        case "G":     // Good
                            lblBottomValue2s[indexLabel].Background = Brushes.Beige;
                            break;
                        case "BB006":     // JOINT
                            lblBottomValue2s[indexLabel].Background = Brushes.Goldenrod;
                            break;
                        case "C":     // NG Hole
                            lblBottomValue2s[indexLabel].Background = Brushes.Cyan;
                            break;
                        case "BB039":     // T Hole
                            lblBottomValue2s[indexLabel].Background = Brushes.Blue;
                            break;
                        default:
                            lblBottomValue2s[indexLabel].Background = Brushes.Red;
                            break;
                    }

                    if (3 == dataService.DataRecipe.Line)
                    {
                        lblBottomValue3s[indexLabel].Content = dataService.DataResult.Bottom.ListRaw[indexValue].Value3;
                        switch (dataService.DataResult.Bottom.ListRaw[indexValue].Value3)
                        {
                            case "G":     // Good
                                lblBottomValue3s[indexLabel].Background = Brushes.Beige;
                                break;
                            case "BB006":     // JOINT
                                lblBottomValue3s[indexLabel].Background = Brushes.Goldenrod;
                                break;
                            case "C":     // NG Hole
                                lblBottomValue3s[indexLabel].Background = Brushes.Cyan;
                                break;
                            case "BB039":     // T Hole
                                lblBottomValue3s[indexLabel].Background = Brushes.Blue;
                                break;
                            default:
                                lblBottomValue3s[indexLabel].Background = Brushes.Red;
                                break;
                        }
                    }
                }
                else
                {
                    lblBottomValue1s[indexLabel].Content = "";
                    lblBottomValue1s[indexLabel].Background = Brushes.Beige;

                    lblBottomValue2s[indexLabel].Content = "";
                    lblBottomValue2s[indexLabel].Background = Brushes.Beige;

                    if (3 == dataService.DataRecipe.Line)
                    {
                        lblBottomValue3s[indexLabel].Content = "";
                        lblBottomValue3s[indexLabel].Background = Brushes.Beige;
                    }
                }
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine(string.Format("AutoPage.DisplayBottomValue({0}, {1}) : ", indexLabel, indexValue) + exc.Message);
            }
        }

        private void DisplayBottomSummary()
        {
            if (0 < dataService.DataResult.Bottom.ListRaw.Count)
            {
                double yield = ((double)dataService.DataResult.Bottom.CountGood / (double)dataService.DataResult.Bottom.CountTotal) * 100.0;

                lblTotalCountSection2.Content = dataService.DataResult.Bottom.CountTotal.ToString();
                lblGoodCountSection2.Content = dataService.DataResult.Bottom.CountGood.ToString();
                lblNGCountSection2.Content = (dataService.DataResult.Bottom.CountTotal - dataService.DataResult.Bottom.CountGood).ToString();
                lblYieldSection2.Content = yield.ToString("0.00");
                lblJointCountSection2.Content = dataService.DataResult.Bottom.CountJoint.ToString();
            }
            else
            {
                lblTotalCountSection2.Content = "";
                lblGoodCountSection2.Content = "";
                lblNGCountSection2.Content = "";
                lblYieldSection2.Content = "";
                lblJointCountSection2.Content = "";
            }
        }

        private void DisplayMono()
        {
            if (countMono != dataService.DataResult.Mono.ListRaw.Count)
            {
                countMono = dataService.DataResult.Mono.ListRaw.Count;

                if (countMono <= 15)
                {
                    for (int i = 0; i < 15; ++i)
                    {
                        DisplayMonoValue(i, i);
                    }
                }
                else
                {
                    int index = 0;
                    for (int i = countMono - 15; i < countMono; ++i)
                    {
                        DisplayMonoValue(index, i);

                        ++index;
                    }
                }

                DisplayMonoSummary();
            }
        }

        private void DisplayMonoValue(int indexLabel, int indexValue)
        {
            try
            {
                tblMonoTitles[indexLabel].Text = string.Format("{0}", indexValue + 1);

                if (indexValue < dataService.DataResult.Mono.ListRaw.Count)
                {
                    lblMonoValue1s[indexLabel].Content = dataService.DataResult.Mono.ListRaw[indexValue].Value1;

                    switch (dataService.DataResult.Mono.ListRaw[indexValue].Value1)
                    {
                        case "G":     // Good
                            lblMonoValue1s[indexLabel].Background = Brushes.Beige;
                            break;
                        case "BB006":     // JOINT
                            lblMonoValue1s[indexLabel].Background = Brushes.Goldenrod;
                            break;
                        case "C":     // NG Hole
                            lblMonoValue1s[indexLabel].Background = Brushes.Cyan;
                            break;
                        case "BB039":     // Pin Hole
                            lblMonoValue1s[indexLabel].Background = Brushes.Blue;
                            break;
                        default:
                            lblMonoValue1s[indexLabel].Background = Brushes.Red;
                            break;
                    }

                    lblMonoValue2s[indexLabel].Content = dataService.DataResult.Mono.ListRaw[indexValue].Value2;
                    switch (dataService.DataResult.Mono.ListRaw[indexValue].Value2)
                    {
                        case "G":     // Good
                            lblMonoValue2s[indexLabel].Background = Brushes.Beige;
                            break;
                        case "BB006":     // JOINT
                            lblMonoValue2s[indexLabel].Background = Brushes.Goldenrod;
                            break;
                        case "C":     // NG Hole
                            lblMonoValue2s[indexLabel].Background = Brushes.Cyan;
                            break;
                        case "BB039":     // T Hole
                            lblMonoValue2s[indexLabel].Background = Brushes.Blue;
                            break;
                        default:
                            lblMonoValue2s[indexLabel].Background = Brushes.Red;
                            break;
                    }

                    if (3 == dataService.DataRecipe.Line)
                    {
                        lblMonoValue3s[indexLabel].Content = dataService.DataResult.Mono.ListRaw[indexValue].Value3;
                        switch (dataService.DataResult.Mono.ListRaw[indexValue].Value3)
                        {
                            case "G":     // Good
                                lblMonoValue3s[indexLabel].Background = Brushes.Beige;
                                break;
                            case "BB006":     // JOINT
                                lblMonoValue3s[indexLabel].Background = Brushes.Goldenrod;
                                break;
                            case "C":     // NG Hole
                                lblMonoValue3s[indexLabel].Background = Brushes.Cyan;
                                break;
                            case "BB039":     // T Hole
                                lblMonoValue3s[indexLabel].Background = Brushes.Blue;
                                break;
                            default:
                                lblMonoValue3s[indexLabel].Background = Brushes.Red;
                                break;
                        }
                    }
                }
                else
                {
                    lblMonoValue1s[indexLabel].Content = "";
                    lblMonoValue1s[indexLabel].Background = Brushes.Beige;

                    lblMonoValue2s[indexLabel].Content = "";
                    lblMonoValue2s[indexLabel].Background = Brushes.Beige;

                    if (3 == dataService.DataRecipe.Line)
                    {
                        lblMonoValue3s[indexLabel].Content = "";
                        lblMonoValue3s[indexLabel].Background = Brushes.Beige;
                    }
                }
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine(string.Format("AutoPage.DisplayMonoValue({0}, {1}) : ", indexLabel, indexValue) + exc.Message);
            }
        }

        private void DisplayMonoSummary()
        {
            if (0 < dataService.DataResult.Mono.ListRaw.Count)
            {
                double yield = ((double)dataService.DataResult.Mono.CountGood / (double)dataService.DataResult.Mono.CountTotal) * 100.0;

                lblTotalCountSection3.Content = dataService.DataResult.Mono.CountTotal.ToString();
                lblGoodCountSection3.Content = dataService.DataResult.Mono.CountGood.ToString();
                lblNGCountSection3.Content = (dataService.DataResult.Mono.CountTotal - dataService.DataResult.Mono.CountGood).ToString();
                lblYieldSection3.Content = yield.ToString("0.00");
                lblJointCountSection3.Content = dataService.DataResult.Mono.CountJoint.ToString();
            }
            else
            {
                lblTotalCountSection3.Content = "";
                lblGoodCountSection3.Content = "";
                lblNGCountSection3.Content = "";
                lblYieldSection3.Content = "";
                lblJointCountSection3.Content = "";
            }
        }


        private void DisplayPunch()
        {
            try
            {
                if (countPunch != dataService.DataResult.Total.ListRaw.Count)
                {
                    //System.Diagnostics.Debug.WriteLine("Display Punch countPunch = {0}, RawCount = {1}", countPunch, dataService.DataResult.Total.ListRaw.Count);
                    countPunch = dataService.DataResult.Total.ListRaw.Count;

                    if (countPunch <= 45)
                    {
                        //for (int i = 0; i < countPunch; ++i)
                        for (int i = 0; i < 45; ++i)
                        {
                            DisplayPunchValue(i, i);
                        }
                    }
                    else
                    {
                        int index = 0;
                        

                        for (int i = countPunch - 45; i < countPunch; ++i)
                        {
                            DisplayPunchValue(index, i);

                            ++index;
                        }
                    }

                    DisplayPunchSummary();
                }
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine("AutoPage.DisplayPunch() : " + exc.Message);
            }
        }

        private void DisplayPunchValue(int indexLabel, int indexValue)
        {
            try
            {
                if (indexLabel >= tblPunchTitles.Length || 0 > indexLabel)
                {
                    Log_Exception.WriteLine("AutoPage.DisplayPunchValue({0}, {1})", indexLabel, indexValue);
                    return;
                }

                tblPunchTitles[indexLabel].Text = string.Format("{0}", indexValue + 1);

                if (indexValue == IndexPunch)
                    lblPunchTitles[indexLabel].Background = Brushes.Red;
                else
                    lblPunchTitles[indexLabel].Background = Brushes.AliceBlue;

                //DisplayPunchTopValue(indexLabel, indexValue);
                //DisplayPunchBottomValue(indexLabel, indexValue);
                //DisplayPunchMonoValue(indexLabel, indexValue);


                if (indexValue < dataService.DataResult.Top.ListRaw.Count)
                {
                    lblPunchValue1s[indexLabel].Content = dataService.DataResult.Total.ListRaw[indexValue].Value1;
                    lblPunchValue2s[indexLabel].Content = dataService.DataResult.Total.ListRaw[indexValue].Value2;

                    switch (dataService.DataResult.Total.ListRaw[indexValue].Value1)
                    {
                        case "G":     // Good
                            lblPunchValue1s[indexLabel].Background = Brushes.Beige;
                            break;
                        case "BB006":     // JOINT
                            lblPunchValue1s[indexLabel].Background = Brushes.Goldenrod;
                            break;
                        case "C":     // NG Hole
                            lblPunchValue1s[indexLabel].Background = Brushes.Cyan;
                            break;
                        case "BB039":     // T Hole
                            lblPunchValue1s[indexLabel].Background = Brushes.Blue;
                            break;
                        default:
                            lblPunchValue1s[indexLabel].Background = Brushes.Red;
                            break;
                    }

                    switch (dataService.DataResult.Total.ListRaw[indexValue].Value2)
                    {
                        case "G":     // Good
                            lblPunchValue2s[indexLabel].Background = Brushes.Beige;
                            break;
                        case "BB006":     // JOINT
                            lblPunchValue2s[indexLabel].Background = Brushes.Goldenrod;
                            break;
                        case "C":     // NG Hole
                            lblPunchValue2s[indexLabel].Background = Brushes.Cyan;
                            break;
                        case "BB039":     // T Hole
                            lblPunchValue2s[indexLabel].Background = Brushes.Blue;
                            break;
                        default:
                            lblPunchValue2s[indexLabel].Background = Brushes.Red;
                            break;
                    }

                    if (3 == dataService.DataRecipe.Line)
                    {
                        lblPunchValue3s[indexLabel].Content = dataService.DataResult.Total.ListRaw[indexValue].Value3;

                        switch (dataService.DataResult.Total.ListRaw[indexValue].Value3)
                        {
                            case "G":     // Good
                                lblPunchValue3s[indexLabel].Background = Brushes.Beige;
                                break;
                            case "BB006":     // JOINT
                                lblPunchValue3s[indexLabel].Background = Brushes.Goldenrod;
                                break;
                            case "C":     // NG Hole
                                lblPunchValue3s[indexLabel].Background = Brushes.Cyan;
                                break;
                            case "BB039":     // T Hole
                                lblPunchValue3s[indexLabel].Background = Brushes.Blue;
                                break;
                            default:
                                lblPunchValue3s[indexLabel].Background = Brushes.Red;
                                break;
                        }
                    }
                }
                else
                {
                    if (indexLabel < lblPunchValue1s.Length)
                    {
                        lblPunchValue1s[indexLabel].Content = "";
                        lblPunchValue1s[indexLabel].Background = Brushes.Beige;

                        lblPunchValue2s[indexLabel].Content = "";
                        lblPunchValue2s[indexLabel].Background = Brushes.Beige;

                        if (3 == dataService.DataRecipe.Line)
                        {
                            lblPunchValue3s[indexLabel].Content = "";
                            lblPunchValue3s[indexLabel].Background = Brushes.Beige;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine(string.Format("AutoPage.DisplayPunchValue({0}, {1}) : ", indexLabel, indexValue) + exc.Message);
            }
            
        }

        //private void DisplayPunchTopValue(int indexLabel, int indexValue)
        //{
        //    if (indexValue < dataService.DataResult.Top.ListRaw.Count)
        //    {
        //        lblPunchTopValue1s[indexLabel].Content = dataService.DataResult.Top.ListRaw[indexValue].Value1;
        //        lblPunchTopValue2s[indexLabel].Content = dataService.DataResult.Top.ListRaw[indexValue].Value2;

        //        switch (dataService.DataResult.Top.ListRaw[indexValue].Value1)
        //        {
        //            case "G":     // Good
        //                lblPunchTopValue1s[indexLabel].Background = Brushes.Beige;
        //                break;
        //            case "BB006":     // JOINT
        //                lblPunchTopValue1s[indexLabel].Background = Brushes.Goldenrod;
        //                break;
        //            case "C":     // NG Hole
        //                lblPunchTopValue1s[indexLabel].Background = Brushes.Cyan;
        //                break;
        //            case "BB039":     // T Hole
        //                lblPunchTopValue1s[indexLabel].Background = Brushes.Blue;
        //                break;
        //            default:
        //                lblPunchTopValue1s[indexLabel].Background = Brushes.Red;
        //                break;
        //        }

        //        switch (dataService.DataResult.Top.ListRaw[indexValue].Value2)
        //        {
        //            case "G":     // Good
        //                lblPunchTopValue2s[indexLabel].Background = Brushes.Beige;
        //                break;
        //            case "BB006":     // JOINT
        //                lblPunchTopValue2s[indexLabel].Background = Brushes.Goldenrod;
        //                break;
        //            case "C":     // NG Hole
        //                lblPunchTopValue2s[indexLabel].Background = Brushes.Cyan;
        //                break;
        //            case "BB039":     // T Hole
        //                lblPunchTopValue2s[indexLabel].Background = Brushes.Blue;
        //                break;
        //            default:
        //                lblPunchTopValue2s[indexLabel].Background = Brushes.Red;
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        if (indexLabel < lblPunchTopValue1s.Length)
        //        {
        //            lblPunchTopValue1s[indexLabel].Content = "";
        //            lblPunchTopValue1s[indexLabel].Background = Brushes.Beige;

        //            lblPunchTopValue2s[indexLabel].Content = "";
        //            lblPunchTopValue2s[indexLabel].Background = Brushes.Beige;
        //        }
        //        else
        //        {
        //            ;
        //            //Log_Debug.WriteLine("Index Error DisplayPunchTopValue : index = {0}, Length = {1}", indexLabel, lblPunchTopValue1s.Length);
        //        }
        //    }
        //}

        //private void DisplayPunchBottomValue(int indexLabel, int indexValue)
        //{
        //    if (indexValue < dataService.DataResult.Bottom.ListRaw.Count)
        //    {
        //        lblPunchBottomValue1s[indexLabel].Content = dataService.DataResult.Bottom.ListRaw[indexValue].Value1;
        //        lblPunchBottomValue2s[indexLabel].Content = dataService.DataResult.Bottom.ListRaw[indexValue].Value2;

        //        switch (dataService.DataResult.Bottom.ListRaw[indexValue].Value1)
        //        {
        //            case "G":     // Good
        //                lblPunchBottomValue1s[indexLabel].Background = Brushes.Beige;
        //                break;
        //            case "BB006":     // JOINT
        //                lblPunchBottomValue1s[indexLabel].Background = Brushes.Goldenrod;
        //                break;
        //            case "C":     // NG Hole
        //                lblPunchBottomValue1s[indexLabel].Background = Brushes.Cyan;
        //                break;
        //            case "BB039":     // T Hole
        //                lblPunchBottomValue1s[indexLabel].Background = Brushes.Blue;
        //                break;
        //            default:
        //                lblPunchBottomValue1s[indexLabel].Background = Brushes.Red;
        //                break;
        //        }

        //        switch (dataService.DataResult.Bottom.ListRaw[indexValue].Value2)
        //        {
        //            case "G":     // Good
        //                lblPunchBottomValue2s[indexLabel].Background = Brushes.Beige;
        //                break;
        //            case "BB006":     // JOINT
        //                lblPunchBottomValue2s[indexLabel].Background = Brushes.Goldenrod;
        //                break;
        //            case "C":     // NG Hole
        //                lblPunchBottomValue2s[indexLabel].Background = Brushes.Cyan;
        //                break;
        //            case "BB039":     // T Hole
        //                lblPunchBottomValue2s[indexLabel].Background = Brushes.Blue;
        //                break;
        //            default:
        //                lblPunchBottomValue2s[indexLabel].Background = Brushes.Red;
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        if (indexLabel < lblPunchBottomValue1s.Length)
        //        {
        //            lblPunchBottomValue1s[indexLabel].Content = "";
        //            lblPunchBottomValue1s[indexLabel].Background = Brushes.Beige;

        //            lblPunchBottomValue2s[indexLabel].Content = "";
        //            lblPunchBottomValue2s[indexLabel].Background = Brushes.Beige;
        //        }
        //        else
        //        {
        //            //Log_Debug.WriteLine("Index Error DisplayPunchBottomValue : index = {0}, Length = {1}", indexLabel, lblPunchBottomValue1s.Length);
        //            ;
        //        }
        //    }
        //}

        //private void DisplayPunchMonoValue(int indexLabel, int indexValue)
        //{
            
        //    if (indexValue < dataService.DataResult.Mono.ListRaw.Count)
        //    {
        //        lblPunchMonoValue1s[indexLabel].Content = dataService.DataResult.Mono.ListRaw[indexValue].Value1;
        //        lblPunchMonoValue2s[indexLabel].Content = dataService.DataResult.Mono.ListRaw[indexValue].Value2;

        //        switch (dataService.DataResult.Mono.ListRaw[indexValue].Value1)
        //        {
        //            case "G":     // Good
        //                lblPunchMonoValue1s[indexLabel].Background = Brushes.Beige;
        //                break;
        //            case "BB006":     // JOINT
        //                lblPunchMonoValue1s[indexLabel].Background = Brushes.Goldenrod;
        //                break;
        //            case "C":     // NG Hole
        //                lblPunchMonoValue1s[indexLabel].Background = Brushes.Cyan;
        //                break;
        //            case "BB039":     // T Hole
        //                lblPunchMonoValue1s[indexLabel].Background = Brushes.Blue;
        //                break;
        //            default:
        //                lblPunchMonoValue1s[indexLabel].Background = Brushes.Red;
        //                break;
        //        }

        //        switch (dataService.DataResult.Mono.ListRaw[indexValue].Value2)
        //        {
        //            case "G":     // Good
        //                lblPunchMonoValue2s[indexLabel].Background = Brushes.Beige;
        //                break;
        //            case "BB006":     // JOINT
        //                lblPunchMonoValue2s[indexLabel].Background = Brushes.Goldenrod;
        //                break;
        //            case "C":     // NG Hole
        //                lblPunchMonoValue2s[indexLabel].Background = Brushes.Cyan;
        //                break;
        //            case "BB039":     // T Hole
        //                lblPunchMonoValue2s[indexLabel].Background = Brushes.Blue;
        //                break;
        //            default:
        //                lblPunchMonoValue2s[indexLabel].Background = Brushes.Red;
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        if (indexLabel < lblPunchMonoValue1s.Length)
        //        {
        //            lblPunchMonoValue1s[indexLabel].Content = "";
        //            lblPunchMonoValue1s[indexLabel].Background = Brushes.Beige;

        //            lblPunchMonoValue2s[indexLabel].Content = "";
        //            lblPunchMonoValue2s[indexLabel].Background = Brushes.Beige;
        //        }
        //        else
        //        {
        //            //Log_Debug.WriteLine("Index Error DisplayPunchMonoValue : index = {0}, Length = {1}", indexLabel, lblPunchMonoValue1s.Length);
        //            ;
        //        }
        //    }
        //}

        private void DisplayPunchSummary()
        {
            if (0 < dataService.DataResult.Total.ListRaw.Count)
            {
                double yield = ((double)dataService.DataResult.Total.CountGood / (double)dataService.DataResult.Total.CountTotal) * 100.0;
                double yield1 = 0.0;
                double yield2 = 0.0;

                if( 0 < dataService.DataResult.Total.CountTotal1 )
                    yield1 = ((double)dataService.DataResult.Total.CountGood1 / (double)dataService.DataResult.Total.CountTotal1) * 100.0;

                if (0 < dataService.DataResult.Total.CountTotal2)
                    yield2 = ((double)dataService.DataResult.Total.CountGood2 / (double)dataService.DataResult.Total.CountTotal2) * 100.0;

                lblTotalCountTotal.Content = dataService.DataResult.Total.CountTotal.ToString();
                lblGoodCountTotal.Content = dataService.DataResult.Total.CountGood.ToString();
                lblNGCountTotal.Content = (dataService.DataResult.Total.CountTotal - dataService.DataResult.Total.CountGood).ToString();
                lblYieldTotal.Content = yield.ToString("0.00");
                lblYieldTotal1.Content = yield1.ToString("0.00");
                lblYieldTotal2.Content = yield2.ToString("0.00");
                lblJointCountTotal.Content = dataService.DataResult.Total.CountJoint.ToString();

                double length = (double)(dataService.DataResult.Total.ListRaw.Count*dataService.DataResult.PF)*4.75*0.001;
                lblLengthValue.Content = length.ToString("0.000");
            }
            else
            {
                lblTotalCountTotal.Content = "";
                lblGoodCountTotal.Content = "";
                lblNGCountTotal.Content = "";
                lblYieldTotal.Content = "";
                lblYieldTotal1.Content = "";
                lblYieldTotal2.Content = "";
                lblJointCountTotal.Content = "";
                lblLengthValue.Content = "";
            }
        }


        #region Events
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
                case SystemService.States.ready:
                case SystemService.States.idle:
                    SetJog();
                    btnStop.IsChecked = false;
                    if (SystemService.States.ready <= sysService.State)
                        btnStart.IsEnabled = true;
                    else
                        btnStart.IsEnabled = false;

                    if (true == btnPause.IsEnabled)
                        btnPause.IsEnabled = false;
                    break;

                case SystemService.States.run:
                    ResetJog();
                    btnStop.IsChecked = false;
                    if (false == btnPause.IsEnabled)
                        btnPause.IsEnabled = true;
                    break;

                case SystemService.States.pause:
                    btnStop.IsChecked = false;
                    if (dataService.DataSystem.FirstIndexPause == true)
                    {
                        if (btnPause.IsChecked == false)
                        {
                            btnPause.IsChecked = true;
                        }
                        DisplayTop();
                        DisplayBottom();
                        DisplayMono();
                        DisplayPunch();
                    }
                    break;

                case SystemService.States.resume:
                    btnStop.IsChecked = false;
                    if (true == btnPause.IsChecked)
                        btnPause.IsChecked = false;
                    break;

                case SystemService.States.homing:
                    break;

                case SystemService.States.homeDone:
                    break;

                case SystemService.States.jobDone:
                    if (null != reviewService)
                        reviewService.Stop();
                    break;

                case SystemService.States.systemLock:
                    break;

                case SystemService.States.systemRelease:
                    break;

                case SystemService.States.reset:
                    if (false == btnStart.IsEnabled)
                        btnStart.IsEnabled = true;
                    btnStop.IsChecked = false;
                    break;

                case SystemService.States.lightAlarm:
                    break;

                case SystemService.States.stop:
                case SystemService.States.heavyAlarm:
                case SystemService.States.emg:
                    ResetJog();
                    btnStart.IsChecked = false;
                    btnPause.IsChecked = false;
                    btnStart.IsEnabled = false;
                    btnPause.IsEnabled = false;
                    btnStop.IsChecked = true;
                    if (null != reviewService)
                        reviewService.Stop();
                    break;
            }
        }

        private void OnEventPunch(int punchUnit, int state, string line)
        {
            if (InvokeRequired)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                            (ThreadStart)delegate()
                                            {
                                                SetEventPunch(punchUnit, state, line);
                                            }
                );
            }
            else
            {
                SetEventPunch(punchUnit, state, line);
            }
        }

        private void SetEventPunch(int punchUnit, int state, string line)
        {
            EnumSmartIC.PunchStates punchState = (EnumSmartIC.PunchStates)state;

            switch (punchState)
            {
                case EnumSmartIC.PunchStates.punch://=0, 
                    IndexPunch = punchUnit;
                    SetDisplayPunchIndex();
                    break;
                case EnumSmartIC.PunchStates.punchDone:
                    IndexPunch = -1;
                    ResetDisplayPunchIndex();
                    break;
                case EnumSmartIC.PunchStates.punchStart:
                case EnumSmartIC.PunchStates.punchEnd:
                    break;
                case EnumSmartIC.PunchStates.tHole:
                    IndexPunch = punchUnit;
                    SetDisplayPunchIndex();
                    break;
                case EnumSmartIC.PunchStates.tHoleDone:
                    IndexPunch = -1;
                    ResetDisplayPunchIndex();
                    break;
                case EnumSmartIC.PunchStates.sectionYield:
                    IndexPunch = punchUnit;
                    SetDisplayPunchIndex();
                    break;
                case EnumSmartIC.PunchStates.sectionYieldDone:
                    IndexPunch = -1;
                    ResetDisplayPunchIndex();
                    break;
                default:
                    IndexPunch = -1;
                    break;
            }
        }

        private void SetDisplayPunchIndex()
        {
            string index = (IndexPunch + 1).ToString();
            for (int i = 0; i < 45; ++i)
            {
                if (tblPunchTitles[i].Text == index)
                    lblPunchTitles[i].Background = Brushes.Red;
                else
                    lblPunchTitles[i].Background = Brushes.AliceBlue;
            }
        }

        private void ResetDisplayPunchIndex()
        {
            for (int i = 0; i < 45; ++i)
            {
                lblPunchTitles[i].Background = Brushes.AliceBlue;
            }
        }

        private void OnEventAutoJog(int run)
        {
            if (InvokeRequired)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                            (ThreadStart)delegate()
                                            {
                                                SetEventAutoJog(run);
                                            }
                );
            }
            else
            {
                SetEventAutoJog(run);
            }
        }

        private void SetEventAutoJog(int run)
        {
            if (1 == run)
                btnStart.IsEnabled = false;
            else
                btnStart.IsEnabled = true;
        }

        private void OnEventChangedRecipe(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            {
                // 3Line
                if (3 == dataService.DataRecipe.Line)
                {
                    for (int i = 0; i < lblTopValue3s.Length; ++i)
                    {
                        lblTopValue3s[i].Visibility = System.Windows.Visibility.Visible;
                        lblBottomValue3s[i].Visibility = System.Windows.Visibility.Visible;
                        lblMonoValue3s[i].Visibility = System.Windows.Visibility.Visible;
                    }

                    for (int i = 0; i < lblPunchValue1s.Length; ++i)
                    {
                        lblPunchValue3s[i].Visibility = System.Windows.Visibility.Visible;
                    }
                }
                else
                {
                    for (int i = 0; i < lblTopValue3s.Length; ++i)
                    {
                        lblTopValue3s[i].Visibility = System.Windows.Visibility.Hidden;
                        lblBottomValue3s[i].Visibility = System.Windows.Visibility.Hidden;
                        lblMonoValue3s[i].Visibility = System.Windows.Visibility.Hidden;
                    }

                    for (int i = 0; i < lblPunchValue1s.Length; ++i)
                    {
                        lblPunchValue3s[i].Visibility = System.Windows.Visibility.Hidden;
                    }
                }

            }));


        }
        #endregion

        private void cbxNormalJob_Click(object sender, RoutedEventArgs e)
        {
            if (null == dataService)
                return;

            CheckBox cbx = sender as CheckBox;

            if( null != cbx )
            {
                cbxTotalCount.IsChecked = false;
                cbxGoodCount.IsChecked = false;

                dataService.DataSystem.JobType = 0;

                if (false == cbx.IsChecked)
                    cbx.IsChecked = true;

                tblStart.Text = "연속 작업시작";

                Log_Trace.WriteLine("AutoPage.cbxNormalJob_Click({0})", cbx.IsChecked);
            }
        }

        private void cbxTotalCount_Click(object sender, RoutedEventArgs e)
        {
            if (null == dataService)
                return;

            CheckBox cbx = sender as CheckBox;

            if (null != cbx)
            {
                cbxNormalJob.IsChecked = false;
                //cbxTotalCount.IsChecked = false;
                cbxGoodCount.IsChecked = false;

                dataService.DataSystem.JobType = 1;

                if (false == cbx.IsChecked)
                    cbx.IsChecked = true;

                try
                {
                    int count = int.Parse(tbxTotalCount.Text);
                    dataService.DataSystem.TotalCount = count;
                }
                catch (Exception exc)
                {
                    Log_Exception.WriteLine("AutoPage.cbxTotalCount_Click() : " + exc.Message);
                }

                tblStart.Text = "Total 작업시작";

                Log_Trace.WriteLine("AutoPage.cbxTotalCount_Click({0})", dataService.DataSystem.TotalCount);
            }
        }

        private void cbxGoodCount_Click(object sender, RoutedEventArgs e)
        {
            if (null == dataService)
                return;

            CheckBox cbx = sender as CheckBox;

            if (null != cbx)
            {
                cbxNormalJob.IsChecked = false;
                cbxTotalCount.IsChecked = false;
                //cbxGoodCount.IsChecked = false;

                dataService.DataSystem.JobType = 2;

                if (false == cbx.IsChecked)
                    cbx.IsChecked = true;

                try
                {
                    int count = int.Parse(tbxTotalCount.Text);
                    dataService.DataSystem.GoodCount = count;
                }
                catch (Exception exc)
                {
                    Log_Exception.WriteLine("AutoPage.cbxGoodCount_Click() : " + exc.Message);
                }

                tblStart.Text = "Good 작업시작";

                Log_Trace.WriteLine("AutoPage.cbxGoodCount_Click({0})", dataService.DataSystem.GoodCount);
            }
        }

        private void cbxVerify_Click(object sender, RoutedEventArgs e)
        {
            if (null == dataService)
                cbxVerify.IsChecked = false;

            dataService.DataSystem.IsSelectedModify = (bool)cbxVerify.IsChecked;

            if (null != reviewService)
            {
                reviewService.SetVerify();
            }

            Log_Trace.WriteLine("AutoPage.cbxVerify_Click({0})", cbxVerify.IsChecked);
        }

        private void cbxYield_Click(object sender, RoutedEventArgs e)
        {
            if (null == dataService)
                cbxYield.IsChecked = false;

            dataService.DataSystem.IsSelectedSectionYield = (bool)cbxYield.IsChecked;


            Log_Trace.WriteLine("AutoPage.cbxYield_Click({0})", cbxYield.IsChecked);
        }

        private int CheckNetworkDrive()
        {
            if ("Virtual" == dataService.DataSystem.MachineName)
                return 0;


            return 0;

            //int ret = 0;
            //string networkDrive;
            //string shareFolder;
            //// Server Check
            //if (false == Directory.Exists(dataService.DataSystem.ServerPath))
            //{
            //    networkDrive = dataService.DataSystem.NTDriveServer;
            //    shareFolder = "\\\\" + dataService.DataSystem.ServerPCIPAddress + "\\" + dataService.DataSystem.PublicFolderServer;

            //    if (0 != FileUtill.ConnectNetworkDrive(networkDrive, shareFolder, "", ""))
            //    {
            //        ret = -1;
            //    }
            //}

            //// Top1 Check
            //if (false == Directory.Exists(dataService.DataSystem.Top1Path))
            //{
            //    networkDrive = dataService.DataSystem.NTDriveTop1;
            //    shareFolder = "\\\\" + dataService.DataSystem.TopIPAddress + "\\" + dataService.DataSystem.PublicFolderTop1;

            //    if (0 != FileUtill.ConnectNetworkDrive(networkDrive, shareFolder, "", ""))
            //    {
            //        ret = -1;
            //    }
            //}

            //// Top2 Check
            //if (false == Directory.Exists(dataService.DataSystem.Top2Path))
            //{
            //    networkDrive = dataService.DataSystem.NTDriveTop2;
            //    shareFolder = "\\\\" + dataService.DataSystem.Top2IPAddress + "\\" + dataService.DataSystem.PublicFolderTop2;

            //    if (0 != FileUtill.ConnectNetworkDrive(networkDrive, shareFolder, "", ""))
            //    {
            //        ret = -1;
            //    }
            //}

            //// Bottom1 Check
            //if (false == Directory.Exists(dataService.DataSystem.Bottom1Path))
            //{
            //    networkDrive = dataService.DataSystem.NTDriveBottom1;
            //    shareFolder = "\\\\" + dataService.DataSystem.BottomIPAddress + "\\" + dataService.DataSystem.PublicFolderBottom1;

            //    if (0 != FileUtill.ConnectNetworkDrive(networkDrive, shareFolder, "", ""))
            //    {
            //        ret = -1;
            //    }
            //}

            //// Bottom2 Check
            //if (false == Directory.Exists(dataService.DataSystem.Bottom2Path))
            //{
            //    networkDrive = dataService.DataSystem.NTDriveBottom2;
            //    shareFolder = "\\\\" + dataService.DataSystem.Bottom2IPAddress + "\\" + dataService.DataSystem.PublicFolderBottom2;

            //    if (0 != FileUtill.ConnectNetworkDrive(networkDrive, shareFolder, "", ""))
            //    {
            //        ret = -1;
            //    }
            //}

            //// Mono1 Check
            //if (false == Directory.Exists(dataService.DataSystem.Mono1Path))
            //{
            //    networkDrive = dataService.DataSystem.NTDriveMono1;
            //    shareFolder = "\\\\" + dataService.DataSystem.MonoIPAddress + "\\" + dataService.DataSystem.PublicFolderMono1;

            //    if (0 != FileUtill.ConnectNetworkDrive(networkDrive, shareFolder, "", ""))
            //    {
            //        ret = -1;
            //    }
            //}

            //// Mono2 Check
            //if (false == Directory.Exists(dataService.DataSystem.Mono2Path))
            //{
            //    networkDrive = dataService.DataSystem.NTDriveMono2;
            //    shareFolder = "\\\\" + dataService.DataSystem.Mono2IPAddress + "\\" + dataService.DataSystem.PublicFolderMono2;

            //    if (0 != FileUtill.ConnectNetworkDrive(networkDrive, shareFolder, "", ""))
            //    {
            //        ret = -1;
            //    }
            //}

            //return ret;
        }

        private int CheckLotID(string lotID)
        {
            string dayFolder = DateTime.Now.ToString("yyMMdd");
            string path = dataService.DataSystem.ReportPath + "\\" + dayFolder;

            try
            {
                string[] files = Directory.GetFiles(path);
                string fileName;

                lotID = (lotID + ".csv").ToUpper();

                //foreach (string file in files)
                for( int i=0; i<files.Length; ++i )
                {

                    //FileInfo info = new FileInfo(file);
                    FileInfo info = new FileInfo(files[i]);

                    fileName = info.Name;   // 확장명까지 포함

                    if (lotID == fileName.ToUpper())
                    {
                        dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                        if (false == msgService.ShowMessage((int)EnumSmartIC.LightAlarms.sameLotID, false))
                        {
                            dioService.ResetOutport((int)EnumSmartIC.Outports.buzzer1);
                            return -1;
                        }
                        else
                        {
                            dioService.ResetOutport((int)EnumSmartIC.Outports.buzzer1);
                            return 0;
                        }

                    }
                }
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine("AutoPage.CheckLotID() : " + exc.Message);
            }

            return 0;
        }

        private void btnCheckDefect_Click(object sender, RoutedEventArgs e)
        {
            if (null != winDefectInfo)
            {
                if (true == winDefectInfo.IsLoaded)
                    return;
            }

            winDefectInfo = new JobDefectInfoWindow();
            if (null != msgService.OwnerWindow)
                winDefectInfo.Owner = msgService.OwnerWindow;

            winDefectInfo.Show();
        }


    }
}
