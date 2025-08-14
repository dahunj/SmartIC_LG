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
using System.Threading;
using System.Windows.Threading;
using System.IO;

namespace SmartICAVI
{
    public class ReportInfo
    {
        public string Name { get; set; }
        public DateTime Time { get; set; }

        public ReportInfo()
        {
            Name = "";
            Time = new DateTime(0, DateTimeKind.Local);
        }

        public ReportInfo(string name, DateTime time)
        {
            Name = name;
            Time = time;
        }
    }

    class DefectCountInfo
    {
        public int DefectNo { get; set; }
        public string DefectName { get; set; }
        public int DefectCount { get; set; }

        public DefectCountInfo()
        {
            DefectNo = 0;
            DefectName = "";
            DefectCount = 0;
        }

        public DefectCountInfo(int no, string name, int count)
        {
            DefectNo = no;
            DefectName = name;
            DefectCount = count;
        }
    }

    /// <summary>
    /// ReportWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ReportWindow : Window
    {
        public bool IsVisibleNextMachine { get; set; }
        public bool IsVisiblePitchMove { get; set; }
        public bool IsVisibleSectionEnd { get; set; }
        public bool IsJobEndView { get; set; }

        public bool IsJobEnd { get; set; }
        public bool IsSectionEnd { get; set; }

        public bool IsWindowClosed { get; set; }

        private int Line { get; set; }

        private int IndexSection { get; set; }
        private int IndexSectionMap { get; set; }
        private int ListCount { get; set; }

        //private bool IsLampOn { get; set; }

        private DataService dataService = null;
        private MsgService msgService = null;
        private SequenceService seqService = null;
        private MotionService motionService = null;
        private DioService dioService = null;
        private DataResult dataResult;

        private VisionCamService camService = null;

        private Label[] Lines01;
        private Label[] Lines02;
        private Label[] Lines03;
        private Label[] LineNums;

        private TextBlock[] tblLines;

        private DispatcherTimer timerBuzzer = null;
        private DispatcherTimer timerTwLamp = null;

        private DateTime selectedDate;

        public ReportWindow()
        {
            InitializeComponent();

            IsWindowClosed = false;

            dataResult = new DataResult();

            

            IsVisibleNextMachine = false;
            IsVisiblePitchMove = false;
            IsVisibleSectionEnd = false;
            IsJobEndView = false;

            IsJobEnd = false;
            IsSectionEnd = false;


            cbPtich.Items.Add("1");
            cbPtich.Items.Add("5");
            cbPtich.Items.Add("10");
            cbPtich.Items.Add("20");
            cbPtich.SelectedIndex = 0;

            Lines01 = new Label[50]
            {
                lbLine01_101, lbLine01_102, lbLine01_103, lbLine01_104, lbLine01_105, lbLine01_106, lbLine01_107, lbLine01_108, lbLine01_109, lbLine01_110,
                lbLine01_111, lbLine01_112, lbLine01_113, lbLine01_114, lbLine01_115, lbLine01_116, lbLine01_117, lbLine01_118, lbLine01_119, lbLine01_120,
                lbLine01_121, lbLine01_122, lbLine01_123, lbLine01_124, lbLine01_125, lbLine01_126, lbLine01_127, lbLine01_128, lbLine01_129, lbLine01_130,
                lbLine01_131, lbLine01_132, lbLine01_133, lbLine01_134, lbLine01_135, lbLine01_136, lbLine01_137, lbLine01_138, lbLine01_139, lbLine01_140,
                lbLine01_141, lbLine01_142, lbLine01_143, lbLine01_144, lbLine01_145, lbLine01_146, lbLine01_147, lbLine01_148, lbLine01_149, lbLine01_150,
            };

            Lines02 = new Label[50]
            {
                lbLine01_201, lbLine01_202, lbLine01_203, lbLine01_204, lbLine01_205, lbLine01_206, lbLine01_207, lbLine01_208, lbLine01_209, lbLine01_210,
                lbLine01_211, lbLine01_212, lbLine01_213, lbLine01_214, lbLine01_215, lbLine01_216, lbLine01_217, lbLine01_218, lbLine01_219, lbLine01_220,
                lbLine01_221, lbLine01_222, lbLine01_223, lbLine01_224, lbLine01_225, lbLine01_226, lbLine01_227, lbLine01_228, lbLine01_229, lbLine01_230,
                lbLine01_231, lbLine01_232, lbLine01_233, lbLine01_234, lbLine01_235, lbLine01_236, lbLine01_237, lbLine01_238, lbLine01_239, lbLine01_240,
                lbLine01_241, lbLine01_242, lbLine01_243, lbLine01_244, lbLine01_245, lbLine01_246, lbLine01_247, lbLine01_248, lbLine01_249, lbLine01_250,
            };

            Lines03 = new Label[50]
            {
                lbLine01_301, lbLine01_302, lbLine01_303, lbLine01_304, lbLine01_305, lbLine01_306, lbLine01_307, lbLine01_308, lbLine01_309, lbLine01_310,
                lbLine01_311, lbLine01_312, lbLine01_313, lbLine01_314, lbLine01_315, lbLine01_316, lbLine01_317, lbLine01_318, lbLine01_319, lbLine01_320,
                lbLine01_321, lbLine01_322, lbLine01_323, lbLine01_324, lbLine01_325, lbLine01_326, lbLine01_327, lbLine01_328, lbLine01_329, lbLine01_330,
                lbLine01_331, lbLine01_332, lbLine01_333, lbLine01_334, lbLine01_335, lbLine01_336, lbLine01_337, lbLine01_338, lbLine01_339, lbLine01_340,
                lbLine01_341, lbLine01_342, lbLine01_343, lbLine01_344, lbLine01_345, lbLine01_346, lbLine01_347, lbLine01_348, lbLine01_349, lbLine01_350,
            };

            LineNums = new Label[50]
            {
                lbLineNo01, lbLineNo02, lbLineNo03, lbLineNo04, lbLineNo05, lbLineNo06, lbLineNo07, lbLineNo08, lbLineNo09, lbLineNo10,
                lbLineNo11, lbLineNo12, lbLineNo13, lbLineNo14, lbLineNo15, lbLineNo16, lbLineNo17, lbLineNo18, lbLineNo19, lbLineNo20,
                lbLineNo21, lbLineNo22, lbLineNo23, lbLineNo24, lbLineNo25, lbLineNo26, lbLineNo27, lbLineNo28, lbLineNo29, lbLineNo30,
                lbLineNo31, lbLineNo32, lbLineNo33, lbLineNo34, lbLineNo35, lbLineNo36, lbLineNo37, lbLineNo38, lbLineNo39, lbLineNo40,
                lbLineNo41, lbLineNo42, lbLineNo43, lbLineNo44, lbLineNo45, lbLineNo46, lbLineNo47, lbLineNo48, lbLineNo49, lbLineNo50                
            };

            tblLines = new TextBlock[50];

            for (int i = 0; i < 50; ++i)
            {
                tblLines[i] = new TextBlock();
                tblLines[i].TextWrapping = TextWrapping.Wrap;

                LineNums[i].Content = tblLines[i];
            }

            btnPitchMoveBack.Axis = 0;
            btnPitchMoveForward.Axis = 0;

            btnPitchMoveBack.IsControlSelected = true;
            btnPitchMoveForward.IsControlSelected = true;

            //dataResult = new DataResult();

            //다음 공정 Node ID ( TSSF00, TSWG00, TSET00, TSAI00, TSQC00 )
            cbNextMachine.Items.Add("자동편집(TSET00)");   // 자동편집
            cbNextMachine.Items.Add("가편집(TSET01)");     // 가편집
            cbNextMachine.Items.Add("PI면검사(TSSG00)");   // PI면 검사
            cbNextMachine.Items.Add("회로면검사(TSCG00)"); // 회로면 검사
            cbNextMachine.Items.Add("AFVI(TSAI02)");      // S-IC AFVI
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Log_Trace.WriteLine("[ReportWindow] Show");

            dataService = DataService.Singleton;
            msgService = MsgService.Singleton;
            seqService = SequenceService.Singleton;
            motionService = MotionService.Singleton;
            dioService = DioService.Singleton;

            camService = VisionCamService.Singleton;
            
            selectedDate = DateTime.Now;

            camService.SendMode("TOPMOST", 0);


            if (true == IsJobEndView)
            {
                btnEnd.IsDefault = false;

                bdrCalender.Visibility = System.Windows.Visibility.Collapsed;


                tbxEnd.Text = "완 공";

                bdrList.Width = 0.0;
                bdrButton.Width = 0.0;

                dataResult = dataService.DataResult;


                // Next Machine
                if (true == IsVisibleNextMachine)
                {
                    tbNextMachineID.Visibility = System.Windows.Visibility.Visible;
                    cbNextMachine.Visibility = System.Windows.Visibility.Visible;

                    tbMESComment.Visibility = System.Windows.Visibility.Visible;
                    tbComment.Visibility = System.Windows.Visibility.Visible;

                    //cbNextMachine.SelectedItem = dataService.DataSystem.NextProcess;
                    // TSET00 추가 2024.11.20
                    if      ("TSET00" == dataService.DataSystem.NextProcess) cbNextMachine.SelectedIndex = 0;
                    else if ("TSET01" == dataService.DataSystem.NextProcess) cbNextMachine.SelectedIndex = 1;
                    else if ("TSSG00" == dataService.DataSystem.NextProcess) cbNextMachine.SelectedIndex = 2;
                    else if ("TSCG00" == dataService.DataSystem.NextProcess) cbNextMachine.SelectedIndex = 3;
                    else                                                     cbNextMachine.SelectedIndex = 4;
                }
                else
                {
                    tbNextMachineID.Visibility = System.Windows.Visibility.Hidden;
                    cbNextMachine.Visibility = System.Windows.Visibility.Hidden;

                    tbMESComment.Visibility = System.Windows.Visibility.Hidden;
                    tbComment.Visibility = System.Windows.Visibility.Hidden;
                }

                //// Section End
                //if (true == IsVisibleSectionEnd)
                //{
                //    btnSectionEnd.Visibility = System.Windows.Visibility.Visible;
                //}
                //else
                //{
                //    btnSectionEnd.Visibility = System.Windows.Visibility.Hidden;
                //}

                //// Pitch Move
                //if (true == IsVisiblePitchMove)
                //{
                //    btnPitchMoveBack.Visibility = System.Windows.Visibility.Visible;
                //    cbPtich.Visibility = System.Windows.Visibility.Visible;
                //    btnPitchMoveForward.Visibility = System.Windows.Visibility.Visible;
                //}
                //else
                //{
                //    btnPitchMoveBack.Visibility = System.Windows.Visibility.Hidden;
                //    cbPtich.Visibility = System.Windows.Visibility.Hidden;
                //    btnPitchMoveForward.Visibility = System.Windows.Visibility.Hidden;
                //}

                DisplayReport();

                int max = dataResult.Total.ListRaw.Count / 50;
                Line = max;
                DisplayResult(Line);

                //완공알림
                if (true == dataService.DataSystem.UseJobDoneBuzzer)
                {
                    SetTimerBuzzer();
                }
                SetTimerTwLamp();

                
            }
            else
            {
                

                tbxEnd.Text = "종 료";

                bdrCalender.Visibility = System.Windows.Visibility.Visible;
                //selectedDate = (DateTime)calendar.SelectedDate;

                btnEnd.IsDefault = true;
                this.Focus();

                bdrList.Width = 350.0;
                bdrButton.Width = 350.0;

                SearchReport();
                //ReadReport(dataService.CurrentResultName);

                //listbox.SelectedItem = dataService.CurrentResultName;
                listbox.SelectedItem = "";

                tbNextMachineID.Visibility = System.Windows.Visibility.Hidden;
                cbNextMachine.Visibility = System.Windows.Visibility.Hidden;

                tbMESComment.Visibility = System.Windows.Visibility.Hidden;
                tbComment.Visibility = System.Windows.Visibility.Hidden;

                btnSectionEnd.Visibility = System.Windows.Visibility.Hidden;

                btnPitchMoveBack.Visibility = System.Windows.Visibility.Hidden;
                cbPtich.Visibility = System.Windows.Visibility.Hidden;
                btnPitchMoveForward.Visibility = System.Windows.Visibility.Hidden;
            }

            
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Topmost = false;

            Log_Trace.WriteLine("[ReportWindow] Hide");

            if (null != camService)
                camService.SendMode("TOPMOST", 1);

            if (true == IsJobEndView)
            {
                KillTimerBuzzer();
                KillTimerTwLamp();
            }

            IsWindowClosed = true;
        }


        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (null != listbox.SelectedItem)
            {
                Log_Trace.WriteLine("[ReportWindow] Click Select" + listbox.SelectedItem.ToString());
                ReadReport(listbox.SelectedItem.ToString());
            }
        }

        private void listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void listboxItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if( null != listbox.SelectedItem )
                ReadReport(listbox.SelectedItem.ToString());
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (null == listbox.SelectedItem)
                return;

            // Current Model
            //string current = "";
            string selected = "";

            //if (null != lblLotIDTotal.Content)
            //    current = lblLotIDTotal.Content.ToString();

            if (null == listbox.SelectedItem)
                return;

            selected = listbox.SelectedItem.ToString();


            if (true == msgService.ShowMessage((int)EnumSmartIC.LightAlarms.delete, false))
            {
                Log_Trace.WriteLine("[ReportWindow] Click Delete" + selected);

                string dayFolder = selectedDate.ToString("yyMMdd");
                string path = dataService.DataSystem.ResultPath + "\\" + dayFolder + "\\" + listbox.SelectedItem.ToString() + ".rpt";
                Log_History.WriteLine("Delete Report : {0}", path);

                File.Delete(path);

                listbox.Items.Remove(selected);

                if (dataResult.RecipeName == selected)
                {
                    dataResult.Clear();

                    Line = 0;
                    DisplayResult(Line);
                }
            }
        }

        private void btnDeleteAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSectionBack_Click(object sender, RoutedEventArgs e)
        {
            //if (1 > --IndexSection)
            //    IndexSection = 1;

            //DisplayReport();
        }

        private void btnSectionForward_Click(object sender, RoutedEventArgs e)
        {
            //if (dataResult.ListLot.Count < ++IndexSection)
            //    IndexSection = dataResult.ListLot.Count;

            //DisplayReport();
        }

        private void btnSectionMapBack_Click(object sender, RoutedEventArgs e)
        {
            //if (1 > --IndexSectionMap)
            //    IndexSectionMap = 1;

            //Line = 0;
            //DisplayResult(Line);

            Line = 0;

            DisplayResult(Line);
        }

        private void btnSectionMapForward_Click(object sender, RoutedEventArgs e)
        {
            //if (dataResult.ListLot.Count < ++IndexSectionMap)
            //    IndexSectionMap = dataResult.ListLot.Count;

            //Line = 0;
            //DisplayResult(Line);

            int max = dataResult.Total.ListRaw.Count / 50;

            Line = max;

            DisplayResult(Line);
        }

        private void btnPitchMoveBack_Click(object sender, RoutedEventArgs e)
        {
            //if (null != dataResult)
            //{
            //    // 더블클릭 방지
            //    CheckBox btn = sender as CheckBox;
            //    if (false == btn.IsChecked)
            //    {
            //        btn.IsChecked = true;
            //        return;
            //    }

            //    if (false == motionService.IsMotionDone(0))
            //        return;

            //    double len = dataService.DataRecipe.Size * (double)(dataResult.LotData.TotalCount);

            //    motionService.SeqMove(0, MotionService.Sequences.reverseEnable);

            //    if (dataService.Pos > len)
            //        motionService.AMove(0, len, 250.0, 300.0);
            //    else
            //    {
            //        double pos = double.Parse(cbPtich.SelectedItem.ToString());
            //        motionService.RMove(0, -dataService.DataRecipe.Size * pos, 250.0, 300.0);
            //    }

            //}
        }

        private void btnPitchMoveForward_Click(object sender, RoutedEventArgs e)
        {
            //// 더블클릭 방지
            //CheckBox btn = sender as CheckBox;
            //if (false == btn.IsChecked)
            //{
            //    btn.IsChecked = true;
            //    return;
            //}

            //if (false == motionService.IsMotionDone(0))
            //    return;

            //motionService.SeqMove(0, MotionService.Sequences.reverseDisable);

            //if (dataService.Pos < 0.0)
            //    motionService.AMove(0, 0.0, 250.0, 300.0);
            //else
            //{
            //    double pos = double.Parse(cbPtich.SelectedItem.ToString());

            //    motionService.RMove(0, dataService.DataRecipe.Size * pos, 250.0, 300.0);
            //}
        }

        private void cbPtich_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            Log_Trace.WriteLine("[ReportWindow] Click End");

            if (true == IsVisibleNextMachine)
            {
                // TSET00 추가 2024.11.20
                if      (0 == cbNextMachine.SelectedIndex) { dataService.DataSystem.NextProcess = "TSET00"; }  // 자동편집
                else if (1 == cbNextMachine.SelectedIndex) { dataService.DataSystem.NextProcess = "TSET01"; }  // 가편집
                else if (2 == cbNextMachine.SelectedIndex) { dataService.DataSystem.NextProcess = "TSSG00"; }  // PI면 검사
                else if (3 == cbNextMachine.SelectedIndex) { dataService.DataSystem.NextProcess = "TSCG00"; }  // 회로면 검사
                else                                       { dataService.DataSystem.NextProcess = "TSAI02"; }  // AFVI

                dataService.DataSystem.Comment = tbComment.Text;    //2021.4.1+
            }

            IsJobEnd = true;
            IsSectionEnd = false;
            IsWindowClosed = true;
            this.Close();
        }

        private void btnSectionEnd_Click(object sender, RoutedEventArgs e)
        {
            Log_Trace.WriteLine("[ReportWindow] Click SectionEnd");
            IsJobEnd = false;
            IsSectionEnd = true;

            if (true == IsVisibleNextMachine)
            {
                //dataService.DataSystem.NextProcess = cbNextMachine.SelectedItem.ToString();
                // TSET00 추가 2024.11.20
                if      (0 == cbNextMachine.SelectedIndex) { dataService.DataSystem.NextProcess = "TSET00"; }  // 자동편집
                else if (1 == cbNextMachine.SelectedIndex) { dataService.DataSystem.NextProcess = "TSET01"; }  // 가편집
                else if (2 == cbNextMachine.SelectedIndex) { dataService.DataSystem.NextProcess = "TSSG00"; }  // PI면 검사
                else if (3 == cbNextMachine.SelectedIndex) { dataService.DataSystem.NextProcess = "TSCG00"; }  // 회로면 검사
                else                                       { dataService.DataSystem.NextProcess = "TSAI02"; }  // AFVI

                dataService.DataSystem.Comment = tbComment.Text;    //2021.4.1+
            }
            this.Close();
        }

        private void btnMapUp_Click(object sender, RoutedEventArgs e)
        {
            if (0 > --Line)
                Line = 0;

            DisplayResult(Line);
        }

        private void btnMapDown_Click(object sender, RoutedEventArgs e)
        {
            int max = dataResult.Total.ListRaw.Count / 50;

            if (max < ++Line)
                Line = max;

            DisplayResult(Line);
        }

        private void cbNextMachine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }


        private void OnEventUpdateCount(object sender, EventArgs e)
        {
            //if (true == IsJobEndView)
            //{
            //    int max = dataResult.LotData.TotalCount / 50;
            //    Line = max;

            //    dataResult = dataService.DataResult;

            //    ListCount = IndexSection = IndexSectionMap = dataResult.ListLot.Count;

            //    DisplayReport();
            //    DisplayResult(Line);
            //}
        }

        private void OnEventRemoveCount(object sender, EventArgs e)
        {
            //if (true == IsJobEndView)
            //{
            //    int max = dataResult.LotData.TotalCount / 50;
            //    Line = max;

            //    dataResult = dataService.DataResult;

            //    ListCount = IndexSection = IndexSectionMap = dataResult.ListLot.Count;

            //    DisplayReport();
            //    DisplayResult(Line);
            //}
        }


        private void SearchReport()
        {
            if (null == dataService)
                return;

            string dayFolder = selectedDate.ToString("yyMMdd");
            string path = dataService.DataSystem.ReportPath + "\\" + dayFolder;

            //if (0 == dataService.DataSystem.ResultPath.IndexOf(".\\"))
            //{
            //    path = dataService.DataSystem.ResultPath.Replace(".\\", dataService.CurrentPath + "\\") + "\\" + dayFolder;
            //}

            string filename;

            List<ReportInfo> listReport = new List<ReportInfo>();

            listbox.Items.Clear();
            listbox.Items.SortDescriptions.Clear();

            try
            {
                string[] files = Directory.GetFiles(path);


                //foreach (string file in files)
                for (int i = 0; i < files.Length; ++i)
                {
                    //FileInfo info = new FileInfo(file);
                    FileInfo info = new FileInfo(files[i]);

                    filename = info.Name;

                    if (".csv" == info.Extension)
                    {

                        ReportInfo report = new ReportInfo(filename.Replace(".csv", ""), info.LastWriteTime);
                        listReport.Add(report);
                    }
                }
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine("[ReportWindow], " + exc.Message);
            }

            // Sort
            if (0 < listReport.Count)
            {
                ReportInfo tempReport = new ReportInfo();

                for (int i = 0; i < listReport.Count; ++i)
                {
                    for (int j = i + 1; j < listReport.Count; ++j)
                    {
                        if (listReport[i].Time < listReport[j].Time)
                        {
                            tempReport.Name = listReport[i].Name;
                            tempReport.Time = listReport[i].Time;

                            listReport[i].Name = listReport[j].Name;
                            listReport[i].Time = listReport[j].Time;

                            listReport[j].Name = tempReport.Name;
                            listReport[j].Time = tempReport.Time;
                        }
                    }
                }

                for (int i = 0; i < listReport.Count; ++i)
                {
                    listbox.Items.Add(listReport[i].Name);
                }
            }

            listReport.Clear();
            GC.Collect();
        }

        private void ReadReport(string name)
        {
            if ("" == name)
                return;

            string dayFolder = selectedDate.ToString("yyMMdd");
            string path = dataService.DataSystem.ReportPath + "\\" + dayFolder + "\\" + name + ".csv";

            //lblRecipe.Content = "";
            tblRecipe.Text = "";
            lblLotIDTotal.Content = "";
            lblCountTotal.Content = "";
            lblOKCountTotal.Content = "";
            lblNGCountTotal.Content = "";
            lblYieldTotal.Content = "";
            lblNGYieldTotal.Content = "";
            lblRunTimeTotal.Content = "";
            lblJointCountTotal.Content = "";
            lblThroughHoleCountTotal.Content = "";
            lblNGContinueTotal.Content = "";

            lblLotIDSection1.Content = "";
            lblCountSection1.Content = "";
            lblOKCountSection1.Content = "";
            lblNGCountSection1.Content = "";
            lblYieldSection1.Content = "";
            lblNGYieldSection1.Content = "";
            //lblRunTimeSection1.Content = "";
            lblJointCount1.Content = "";
            lblThroughHoleCount1.Content = "";
            lblNGContinueSection1.Content = "";

            lblLotIDSection2.Content = "";
            lblCountSection2.Content = "";
            lblOKCountSection2.Content = "";
            lblNGCountSection2.Content = "";
            lblYieldSection2.Content = "";
            lblNGYieldSection2.Content = "";
            //lblRunTimeSection2.Content = "";
            lblJointCount2.Content = "";
            lblThroughHoleCount2.Content = "";
            lblNGContinueSection2.Content = "";

            lblLotIDSection3.Content = "";
            lblCountSection3.Content = "";
            lblOKCountSection3.Content = "";
            lblNGCountSection3.Content = "";
            lblYieldSection3.Content = "";
            lblNGYieldSection3.Content = "";
            //lblRunTimeSection3.Content = "";
            lblJointCount3.Content = "";
            lblThroughHoleCount3.Content = "";
            lblNGContinueSection3.Content = "";

            dataResult.Clear();

            if (0 != dataResult.Load(path))
                msgService.ShowMessage((int)EnumSmartIC.LightAlarms.cannotRead);

            //ListCount = IndexSection = IndexSectionMap = dataResult.ListLot.Count;

            Line = 0;

            listViewDefectCount.Items.Clear();

            DisplayReport();
            DisplayResult(Line);
        }

        private void DisplayReport()
        {
            //lblRecipe.Content = dataResult.RecipeName;
            tblRecipe.Text = dataResult.RecipeName;

            lblLotIDTotal.Content = dataResult.LotID;

            lblCountTotal.Content = dataResult.Total.CountTotal.ToString();

            lblOKCountTotal.Content = dataResult.Total.CountGood.ToString();

            lblJointCountTotal.Content = dataResult.Total.CountJoint.ToString();

            lblThroughHoleCountTotal.Content = dataResult.Total.CountTHole.ToString();

            lblPFHole.Content = dataResult.PF.ToString();

            lblLine.Content = dataResult.Line.ToString();

           

            int totalNGCount = dataResult.Total.CountTotal - dataResult.Total.CountGood;

            lblNGCountTotal.Content = totalNGCount.ToString();

            if (0 == dataResult.Total.CountTotal)
                return;


            double yieldGood = 0.0;
            double yieldGood1 = 0.0;
            double yieldGood2 = 0.0;
            double yieldNG = 0.0;
            yieldGood = (double)(dataResult.Total.CountGood * 100) / (double)(dataResult.Total.CountTotal);
            yieldNG = (double)(totalNGCount * 100) / (double)(dataResult.Total.CountTotal);

            if( 0 < dataResult.Total.CountTotal1 )
                yieldGood1 = (double)(dataResult.Total.CountGood1 * 100) / (double)(dataResult.Total.CountTotal1);
            if (0 < dataResult.Total.CountTotal2)
                yieldGood2 = (double)(dataResult.Total.CountGood2 * 100) / (double)(dataResult.Total.CountTotal2);

            lblYieldTotal.Content = yieldGood.ToString("0.00");
            lblNGYieldTotal.Content = yieldNG.ToString("0.00");

            lblYieldTotal1.Content = yieldGood1.ToString("0.00");
            lblYieldTotal2.Content = yieldGood2.ToString("0.00");


            lblNGContinueTotal.Content = dataResult.Total.CountContinuousNG.ToString();

            // 길이

            double length = (double)(dataResult.PF * dataResult.Total.ListRaw.Count) * 4.75*0.001;

            lblLengthValue.Content = length.ToString("0.00");

            // 가동시간
            TimeSpan span = new TimeSpan();
            span = dataResult.TimeEnd - dataResult.TimeStart;

            lblRunTimeTotal.Content = string.Format("{0:00}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds);



            // Top
            if (0 < dataResult.Top.CountTotal)
            {
                lblLotIDSection1.Content = dataResult.LotID;

                lblCountSection1.Content = dataResult.Top.CountTotal.ToString();

                lblOKCountSection1.Content = dataResult.Top.CountGood.ToString();

                lblJointCount1.Content = dataResult.Top.CountJoint.ToString();

                totalNGCount = dataResult.Top.CountTotal - dataResult.Top.CountGood;

                lblNGCountSection1.Content = totalNGCount.ToString();

                lblThroughHoleCount1.Content = dataResult.Top.CountTHole.ToString();

                yieldGood = (double)(dataResult.Top.CountGood * 100) / (double)(dataResult.Top.CountTotal);
                yieldNG = (double)(totalNGCount * 100) / (double)(dataResult.Top.CountTotal);

                lblYieldSection1.Content = yieldGood.ToString("0.00");
                lblNGYieldSection1.Content = yieldNG.ToString("0.00");

                lblNGContinueSection1.Content = dataResult.Top.CountContinuousNG.ToString();


                //// 가동시간
                //span = new TimeSpan();
                //span = dataResult.ListLot[index1].TimeEnd - dataResult.ListLot[index1].TimeStart;

                //lblRunTimeSection1.Content = string.Format("{0:00}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds);
            }

            // Bottom
            if (0 < dataResult.Bottom.CountTotal)
            {
                lblLotIDSection2.Content = dataResult.LotID;

                lblCountSection2.Content = dataResult.Bottom.CountTotal.ToString();

                lblOKCountSection2.Content = dataResult.Bottom.CountGood.ToString();

                lblJointCount2.Content = dataResult.Bottom.CountJoint.ToString();

                lblThroughHoleCount2.Content = dataResult.Bottom.CountTHole.ToString();

                totalNGCount = dataResult.Bottom.CountTotal - dataResult.Bottom.CountGood;

                lblNGCountSection2.Content = totalNGCount.ToString();

                yieldGood = (double)(dataResult.Bottom.CountGood * 100) / (double)(dataResult.Bottom.CountTotal);
                yieldNG = (double)(totalNGCount * 100) / (double)(dataResult.Bottom.CountTotal);

                lblYieldSection2.Content = yieldGood.ToString("0.00");
                lblNGYieldSection2.Content = yieldNG.ToString("0.00");

                lblNGContinueSection2.Content = dataResult.Bottom.CountContinuousNG.ToString();


                //// 가동시간
                //span = new TimeSpan();
                //span = dataResult.ListLot[index1].TimeEnd - dataResult.ListLot[index1].TimeStart;

                //lblRunTimeSection1.Content = string.Format("{0:00}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds);
            }

            // Mono
            if (0 < dataResult.Mono.CountTotal)
            {
                lblLotIDSection3.Content = dataResult.LotID;

                lblCountSection3.Content = dataResult.Mono.CountTotal.ToString();

                lblOKCountSection3.Content = dataResult.Mono.CountGood.ToString();

                lblJointCount3.Content = dataResult.Mono.CountJoint.ToString();

                lblThroughHoleCount3.Content = dataResult.Mono.CountTHole.ToString();

                totalNGCount = dataResult.Mono.CountTotal - dataResult.Mono.CountGood;

                lblNGCountSection3.Content = totalNGCount.ToString();

                yieldGood = (double)(dataResult.Mono.CountGood * 100) / (double)(dataResult.Mono.CountTotal);
                yieldNG = (double)(totalNGCount * 100) / (double)(dataResult.Mono.CountTotal);

                lblYieldSection3.Content = yieldGood.ToString("0.00");
                lblNGYieldSection3.Content = yieldNG.ToString("0.00");

                lblNGContinueSection3.Content = dataResult.Mono.CountContinuousNG.ToString();


                //// 가동시간
                //span = new TimeSpan();
                //span = dataResult.ListLot[index1].TimeEnd - dataResult.ListLot[index1].TimeStart;

                //lblRunTimeSection1.Content = string.Format("{0:00}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds);
            }

            // Defect Count
            listViewDefectCount.Items.Clear();
            int no = 0;
            for (int i = 0; i < dataResult.CountNGIDs.Length; ++i)
            {
                if (0 < dataResult.CountNGs[i])
                {
                    DefectCountInfo defect = new DefectCountInfo(++no, dataResult.CountNGNames[i], dataResult.CountNGs[i]);
                    listViewDefectCount.Items.Add(defect);
                }
            }
        }

        private void DisplayResult(int line)
        {
            if (3 == dataResult.Line)
            {
                DisplayResult3(line);
                return;
            }

            int start = line * 50;
            int end = start + 50;

            int length = dataResult.Total.CountTotal/2;

            if ( 0 == length )
            {
                for (int i = 0; i < 50; ++i)
                {
                    Lines01[i].IsEnabled = true;
                    Lines02[i].IsEnabled = true;
                    Lines03[i].IsEnabled = true;

                    Lines01[i].Content = "";
                    Lines02[i].Content = "";
                    Lines03[i].Content = "";
                }

                return;
            }

            if (end > length)
                end = length;

            lbLine01.Content = string.Format("{0}", line + 1);

            for (int i = 0; i < 50; ++i)
            {
                tblLines[i].Text = string.Format("{0}", line * 50 + i + 1);
            }



            for (int i = start; i < end; ++i)
            {
                Lines01[i - start].Content = dataResult.Total.ListRaw[i].Value1;
                Lines02[i - start].Content = dataResult.Total.ListRaw[i].Value2;
                Lines03[i - start].Content = "";

                if ("G" == dataResult.Total.ListRaw[i].Value1)
                {
                    Lines01[i - start].IsEnabled = true;
                }
                else
                {
                    Lines01[i - start].IsEnabled = false;
                }

                if ("G" == dataResult.Total.ListRaw[i].Value2)
                {
                    Lines02[i - start].IsEnabled = true;
                }
                else
                {
                    Lines02[i - start].IsEnabled = false;
                }

                Lines03[i - start].IsEnabled = true;
            }

            for (int i = end - start; i < 50; ++i)
            {
                Lines01[i].IsEnabled = true;
                Lines02[i].IsEnabled = true;
                Lines03[i].IsEnabled = true;

                Lines01[i].Content = "";
                Lines02[i].Content = "";
                Lines03[i].Content = "";
            }
        }

        private void DisplayResult3(int line)
        {
            int start = line * 50;
            int end = start + 50;

            int length = dataResult.Total.CountTotal / 3;

            if (0 == length)
            {
                for (int i = 0; i < 50; ++i)
                {
                    Lines01[i].IsEnabled = true;
                    Lines02[i].IsEnabled = true;
                    Lines03[i].IsEnabled = true;

                    Lines01[i].Content = "";
                    Lines02[i].Content = "";
                    Lines03[i].Content = "";
                }

                return;
            }

            if (end > length)
                end = length;

            lbLine01.Content = string.Format("{0}", line + 1);

            for (int i = 0; i < 50; ++i)
            {
                tblLines[i].Text = string.Format("{0}", line * 50 + i + 1);
            }



            for (int i = start; i < end; ++i)
            {
                Lines01[i - start].Content = dataResult.Total.ListRaw[i].Value1;
                Lines02[i - start].Content = dataResult.Total.ListRaw[i].Value2;
                Lines03[i - start].Content = dataResult.Total.ListRaw[i].Value3;

                if ("G" == dataResult.Total.ListRaw[i].Value1)
                {
                    Lines01[i - start].IsEnabled = true;
                }
                else
                {
                    Lines01[i - start].IsEnabled = false;
                }

                if ("G" == dataResult.Total.ListRaw[i].Value2)
                {
                    Lines02[i - start].IsEnabled = true;
                }
                else
                {
                    Lines02[i - start].IsEnabled = false;
                }

                if ("G" == dataResult.Total.ListRaw[i].Value3)
                {
                    Lines03[i - start].IsEnabled = true;
                }
                else
                {
                    Lines03[i - start].IsEnabled = false;
                }
            }

            for (int i = end - start; i < 50; ++i)
            {
                Lines01[i].IsEnabled = true;
                Lines02[i].IsEnabled = true;
                Lines03[i].IsEnabled = true;

                Lines01[i].Content = "";
                Lines02[i].Content = "";
                Lines03[i].Content = "";
            }
        }

        private void SetTimerBuzzer()
        {
            // Set Timer
            dioService.SetOutport((int)DioService.Outports.twBuzzer);

            timerBuzzer = new DispatcherTimer();
            timerBuzzer.Interval = new System.TimeSpan(0, 0, dataService.DataSystem.JobDoneBuzzerTime);
            timerBuzzer.IsEnabled = true;
            timerBuzzer.Tick += new EventHandler(timer_Buzzer);
        }

        private void KillTimerBuzzer()
        {
            dioService.ResetOutport((int)DioService.Outports.twBuzzer);

            if (null != timerBuzzer)
            {
                timerBuzzer.IsEnabled = false;
                timerBuzzer.Tick -= timer_Buzzer;
            }
        }

        private void timer_Buzzer(object sender, EventArgs e)
        {
            KillTimerBuzzer();
        }

        private void SetTimerTwLamp()
        {
            dioService.SetOutport((int)DioService.Outports.twLampYellow);

            timerTwLamp = new DispatcherTimer();
            timerTwLamp.Interval = new System.TimeSpan(0, 0, 0, 0, 500);
            timerTwLamp.IsEnabled = true;
            timerTwLamp.Tick += new EventHandler(timer_TwLamp);
        }

        private void KillTimerTwLamp()
        {
            if (null != timerTwLamp)
            {
                timerTwLamp.IsEnabled = false;
                timerTwLamp.Tick -= timer_TwLamp;
            }

            dioService.ResetOutport((int)DioService.Outports.twLampYellow);
        }

        private void timer_TwLamp(object sender, EventArgs e)
        {
            if( true == dioService.IsOutportOn((int)DioService.Outports.twLampYellow ))
            {
                dioService.ResetOutport((int)DioService.Outports.twLampYellow);
            }
            else
            {
                dioService.SetOutport((int)DioService.Outports.twLampYellow);
            }
        }

        protected void DoEvents(int mSec)
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

        private void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Calendar cal = sender as Calendar;

            if (null != cal)
            {
                selectedDate = (DateTime)cal.SelectedDate;

                SearchReport();
            }
        }
    }
}


