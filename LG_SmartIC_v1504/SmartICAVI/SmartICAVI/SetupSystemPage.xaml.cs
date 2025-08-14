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
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Threading;

using SmartICAVI.UserControls;

namespace SmartICAVI
{
    /// <summary>
    /// SetupSystemPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SetupSystemPage : Page
    {
        private DataService m_dataService = null;
        private MsgService msgService = null;

        private string oldValue = "";

        private bool isPunchCountReset;

        private bool isLightResetTop;
        private bool isLightResetBottom;
        private bool isLightResetMono;

        private bool IsInitDone { get; set; }

        public SetupSystemPage()
        {
            InitializeComponent();

            IsInitDone = false;
            isPunchCountReset = false;

            isLightResetBottom = false;
            isLightResetMono = false;
            isLightResetTop = false;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            m_dataService = DataService.Singleton;
            msgService = MsgService.Singleton;

            Cancel();

            IsInitDone = true;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            IsInitDone = false;
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
            if (true == IsInitDone)
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
        }

        private string CheckModify_String(string control, string data, string description)
        {
            if (control != data)
            {
                Log_History.WriteLine("[SetupPage] {2} : {0} => {1}", data, control, description);
                return control;
            }

            return data;
        }

        private double CheckModify_Double(string control, double data, string description)
        {
            if (control != data.ToString())
            {
                double value;
                if (true == double.TryParse(control, out value))
                {
                    Log_History.WriteLine("[SetupPage] {2} : {0:0.00000} => {1:0.00000}", data, value, description);
                    return value;
                }
            }

            return data;
        }

        private int CheckModify_Int32(string control, int data, string description)
        {
            if (control != data.ToString())
            {
                int value;
                if (true == int.TryParse(control, out value))
                {
                    Log_History.WriteLine("[SetupPage] {2} : {0} => {1}", data, value, description);
                    return value;
                }
            }

            return data;
        }

        private bool CheckModify_Boolean(bool control, bool data, string description)
        {
            if (control != data)
            {
                Log_History.WriteLine("[SetupPage] {2} : {0} => {1}", data, control, description);
                return control;
            }

            return data;
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void btnPath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            Button btn = sender as Button;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                switch (btn.Tag.ToString())
                {
                    case "LogPath":
                        tbxLogPath.Text = dlg.SelectedPath;
                        Log_History.WriteLine("[SetupPage] Click LogPath=" + tbxLogPath.Text);
                        break;
                    case "ReportDataPath":
                        tbxReportDataPath.Text = dlg.SelectedPath;
                        Log_History.WriteLine("[SetupPage] Click ReportDataPath=" + tbxReportDataPath.Text);
                        break;
                    case "ResultDataPath":
                        tbxResultDataPath.Text = dlg.SelectedPath;
                        Log_History.WriteLine("[SetupPage] Click ResultDataPath=" + tbxResultDataPath.Text);
                        break;
                    case "Top1Path":
                        tbxTop1Path.Text = dlg.SelectedPath;
                        Log_History.WriteLine("[SetupPage] Click Top1Path=" + tbxTop1Path.Text);
                        break;
                    case "Top2Path":
                        tbxTop2Path.Text = dlg.SelectedPath;
                        Log_History.WriteLine("[SetupPage] Click Top2Path=" + tbxTop2Path.Text);
                        break;
                    case "Bottom1Path":
                        tbxBottom1Path.Text = dlg.SelectedPath;
                        Log_History.WriteLine("[SetupPage] Click Bottom1Path=" + tbxBottom1Path.Text);
                        break;
                    case "Bottom2Path":
                        tbxBottom2Path.Text = dlg.SelectedPath;
                        Log_History.WriteLine("[SetupPage] Click Bottom2Path=" + tbxBottom2Path.Text);
                        break;
                    case "Mono1Path":
                        tbxMono1Path.Text = dlg.SelectedPath;
                        Log_History.WriteLine("[SetupPage] Click Mono1Path=" + tbxMono1Path.Text);
                        break;
                    case "Mono2Path":
                        tbxMono2Path.Text = dlg.SelectedPath;
                        Log_History.WriteLine("[SetupPage] Click Mono2Path=" + tbxMono2Path.Text);
                        break;
                    case "ServerPath":
                        tbxServerPath.Text = dlg.SelectedPath;
                        Log_History.WriteLine("[SetupPage] Click ServerPath=" + tbxServerPath.Text);
                        break;

                    default:
                        break;
                }
            }
        }


        private void Cancel()
        {
            if( null == m_dataService )
                return;

            isLightResetBottom = false;
            isLightResetMono = false;
            isLightResetTop = false;

            tbMachineID.Text = m_dataService.DataSystem.MachineID;
            tbProcessID.Text = m_dataService.DataSystem.ProcessID;
            tbToolID.Text = m_dataService.DataSystem.ToolID;

            if (true == m_dataService.DataSystem.UseCheckGuide)
            {
                rdCheckGuideUse.IsChecked = true;
                rdCheckGuideDisuse.IsChecked = false;
            }
            else
            {
                rdCheckGuideUse.IsChecked = false;
                rdCheckGuideDisuse.IsChecked = true;
            }


            if( true == m_dataService.DataSystem.UseVirtualKeyboard )
            {
                rdVirtualKeyboardUse.IsChecked = true;
                rdVIrtualKeyboardDisuse.IsChecked = false;
            }
            else
            {
                rdVirtualKeyboardUse.IsChecked = false;
                rdVIrtualKeyboardDisuse.IsChecked = true;
            }

            if( true == m_dataService.DataSystem.UseSelectParam )
            {
                rdSelectParamsUse.IsChecked = true;
                rdSelectparamsDisuse.IsChecked = false;
            }
            else
            {
                rdSelectParamsUse.IsChecked = false;
                rdSelectparamsDisuse.IsChecked = true;
            }

            if (true == m_dataService.DataSystem.IsSelectedVision)
            {
                rdVisionUse.IsChecked = true;
                rdVisionDisuse.IsChecked = false;
            }
            else
            {
                rdVisionUse.IsChecked = false;
                rdVisionDisuse.IsChecked = true;
            }

            if (true == m_dataService.DataSystem.IsSelectedPunch)
            {
                rdPunchUse.IsChecked = true;
                rdPunchDisuse.IsChecked = false;
            }
            else
            {
                rdPunchUse.IsChecked = false;
                rdPunchDisuse.IsChecked = true;
            }

            if (true == m_dataService.DataSystem.IsSelectedPunchInspect)
            {
                rdPunchInspectUse.IsChecked = true;
                rdPunchInspectDisuse.IsChecked = false;
            }
            else
            {
                rdPunchInspectUse.IsChecked = false;
                rdPunchInspectDisuse.IsChecked = true;
            }

            if (true == m_dataService.DataSystem.IsSelectedPunchTolerance)
            {
                rdPunchToleranceUse.IsChecked = true;
                rdPunchToleranceDisuse.IsChecked = false;
            }
            else
            {
                rdPunchToleranceUse.IsChecked = false;
                rdPunchToleranceDisuse.IsChecked = true;
            }

            if (true == m_dataService.DataSystem.IsSelectedCNGPunch)
            {
                rdPunchCNGUse.IsChecked = true;
                rdPunchCNGDisuse.IsChecked = false;
            }
            else
            {
                rdPunchCNGUse.IsChecked = false;
                rdPunchCNGDisuse.IsChecked = true;
            }

            tbPunchInspectUnit.Text = m_dataService.DataSystem.PunchInspectUnit.ToString();

            // Hole Stop
            rdHoleStopUse.IsChecked = m_dataService.DataSystem.IsSelectedHoleStop;
            rdHoleStopDisuse.IsChecked = !m_dataService.DataSystem.IsSelectedHoleStop;

            // Joint Stop
            rdJointStopUse.IsChecked = m_dataService.DataSystem.IsSelectedJointStop;
            rdJointStopDisuse.IsChecked = !m_dataService.DataSystem.IsSelectedJointStop;

            // SectionYield
            rdSectionYieldUse.IsChecked = m_dataService.DataSystem.IsSelectedSectionYield;
            rdSectionYieldDisuse.IsChecked = !m_dataService.DataSystem.IsSelectedSectionYield;

            // Always Align
            rdAlwaysAlignUse.IsChecked = m_dataService.DataSystem.IsSelectedAlwaysAlign;
            rdAlwaysAlignDisuse.IsChecked = !m_dataService.DataSystem.IsSelectedAlwaysAlign;

            // Clean Roller
            rdCleanRollerUse.IsChecked = m_dataService.DataSystem.IsSelectedCleanRoller;
            rdCleanRollerDisuse.IsChecked = !m_dataService.DataSystem.IsSelectedCleanRoller;

            // Miss Print
            rdMissPrintUse.IsChecked = m_dataService.DataSystem.IsSelectedMissPrint;
            rdMissPrintDisuse.IsChecked = !m_dataService.DataSystem.IsSelectedMissPrint;

            // Dual Error
            rdDualErrorUse.IsChecked = m_dataService.DataSystem.IsSelectedDualErrorStop;
            rdDualErrorDisuse.IsChecked = !m_dataService.DataSystem.IsSelectedDualErrorStop;

            if (true == m_dataService.DataSystem.IsSelectedModify)
            {
                rdModifyUse.IsChecked = true;
                rdModifyDisuse.IsChecked = false;
            }
            else
            {
                rdModifyUse.IsChecked = false;
                rdModifyDisuse.IsChecked = true;
            }

            rdTopVisionUse.IsChecked = m_dataService.DataSystem.IsSelectedTop;
            rdTopVisionDisuse.IsChecked = !m_dataService.DataSystem.IsSelectedTop;

            rdBottomVisionUse.IsChecked = m_dataService.DataSystem.IsSelectedBottom;
            rdBottomVisionDisuse.IsChecked = !m_dataService.DataSystem.IsSelectedBottom;

            rdMonoVisionUse.IsChecked = m_dataService.DataSystem.IsSelectedMono;
            rdMonoVisionDisuse.IsChecked = !m_dataService.DataSystem.IsSelectedMono;


            if( true == m_dataService.DataSystem.UseJobDoneBuzzer )
            {
                rdJobDoneBuzzerUse.IsChecked = true;
                rdUseJobDoneBuzzerDisuse.IsChecked = false;
            }
            else
            {
                rdJobDoneBuzzerUse.IsChecked = false;
                rdUseJobDoneBuzzerDisuse.IsChecked = true;
            }
            tbJobDoneBuzzer.Text = m_dataService.DataSystem.JobDoneBuzzerTime.ToString();


            if( true == m_dataService.DataSystem.UseLightAlarmBuzzer )
            {
                rdLightAlarmBuzzerUse.IsChecked = true;
                rdLightAlarmBuzzerDisuse.IsChecked = false;
            }
            else
            {
                rdLightAlarmBuzzerUse.IsChecked = false;
                rdLightAlarmBuzzerDisuse.IsChecked = true;
            }
            tbLightAlarmBuzzer.Text = m_dataService.DataSystem.LightAlarmBuzzerTime.ToString();

            if (true == m_dataService.DataSystem.IsSelectedBackFeeding)
            {
                rdBackFeedingUse.IsChecked = true;
                rdBackFeedingDisuse.IsChecked = false;
            }
            else
            {
                rdBackFeedingUse.IsChecked = false;
                rdBackFeedingDisuse.IsChecked = true;
            }
            tbBackFeeding.Text = m_dataService.DataSystem.PosBackFeeding.ToString("0.000");

            rdAlignToleranceDisuse.IsChecked = !m_dataService.DataSystem.IsSelectedAlignTolerance;
            rdAlignToleranceUse.IsChecked = m_dataService.DataSystem.IsSelectedAlignTolerance;
            tbxAlignTolerance.Text = m_dataService.DataSystem.AlignTolerance.ToString("0.000");

            // Punch First Unit
            rdPunchFirstUnitUse.IsChecked = m_dataService.DataSystem.IsSelectedPunchFirstUnit;
            rdPunchFirstUnitDisuse.IsChecked = !m_dataService.DataSystem.IsSelectedPunchFirstUnit;

            // Punch Last Unit
            rdPunchLastUnitUse.IsChecked = m_dataService.DataSystem.IsSelectedPunchLastUnit;
            rdPunchLastUnitDisuse.IsChecked = !m_dataService.DataSystem.IsSelectedPunchLastUnit;

            // Punch Set Unit
            rdPunchSetUnitUse.IsChecked = m_dataService.DataSystem.IsSelectedPunchSetUnit;
            rdPunchSetUnitDisuse.IsChecked = !m_dataService.DataSystem.IsSelectedPunchSetUnit;
            tbPunchSetUnit.Text = m_dataService.DataSystem.PunchUnitIndex.ToString();

            rdHeavyAlarmBuzzerUse.IsChecked = m_dataService.DataSystem.UseHeavyAlarmBuzzer;
            rdHeavyAlarmBuzzerDisuse.IsChecked = !m_dataService.DataSystem.UseHeavyAlarmBuzzer;
            tbHeavyAlarmBuzzer.Text = m_dataService.DataSystem.HeavyAlarmBuzzerTime.ToString();
                        
            tbDistanceTopBottom.Text = m_dataService.DataSystem.BottomDistance.ToString("0.000");
            tbDistanceTopMono.Text = m_dataService.DataSystem.MonoDistance.ToString("0.000");
            tbTopVisionZ.Text = m_dataService.DataSystem.TopVisionZ.ToString("0.000");
            tbBottomVisionZ.Text = m_dataService.DataSystem.BottomVisionZ.ToString("0.000");

            tbAlignVisionX.Text = m_dataService.DataSystem.AlignVisionX.ToString("0.000");
            tbAlignVisionY.Text = m_dataService.DataSystem.AlignVisionY.ToString("0.000");

            tbPunchX.Text = m_dataService.DataSystem.PunchOffsetX.ToString("0.000");
            tbPunchY.Text = m_dataService.DataSystem.PunchOffsetY.ToString("0.000");

            tbPunchCenterX.Text = m_dataService.DataSystem.PunchOffsetCenterX.ToString("0.000");
            tbPunchCenterY.Text = m_dataService.DataSystem.PunchOffsetCenterY.ToString("0.000");

            tbPunchBX.Text = m_dataService.DataSystem.PunchOffsetBX.ToString("0.000");
            tbPunchBY.Text = m_dataService.DataSystem.PunchOffsetBY.ToString("0.000");

            tbUncoilerReference.Text = m_dataService.DataSystem.UncoilerReference.ToString("0.000");
            tbRecoilerReference.Text = m_dataService.DataSystem.RecoilerReference.ToString("0.000");

            tbBufferLimitN.Text = m_dataService.DataSystem.BufferLimitN.ToString("0.000");
            tbBufferLimitP.Text = m_dataService.DataSystem.BufferLimitP.ToString("0.000");

            tbPunchStroke.Text = m_dataService.DataSystem.PunchStroke.ToString("0.000");
            tbPunchDistance.Text = m_dataService.DataSystem.PunchDistance.ToString("0.000");
            tbPunchDelay.Text = m_dataService.DataSystem.DelayPunch.ToString();

            tbDelayFeed.Text = m_dataService.DataSystem.DelayFeeding.ToString();

            tbScanTolerance.Text = m_dataService.DataSystem.ScanTolerance.ToString("0.000");
            tbImageTolerance.Text = m_dataService.DataSystem.ImageTolerance.ToString("0.000");
            tbIndexTolerance.Text = m_dataService.DataSystem.IndexTolerance.ToString("0.000");

            tbScanDummyTop.Text = m_dataService.DataSystem.ScanDummyTop.ToString("0.00000");
            tbScanDummyBottom.Text = m_dataService.DataSystem.ScanDummyBottom.ToString("0.00000");
            tbScanDummyMono.Text = m_dataService.DataSystem.ScanDummyMono.ToString("0.00000");
            tbMotionTolerance.Text = m_dataService.DataSystem.MotionTolerance.ToString("0.000");

            tbxLogPath.Text = m_dataService.DataSystem.LogPath;
            tbLogSavePeriod.Text = m_dataService.DataSystem.LogSave.ToString();

            tbxResultDataPath.Text = m_dataService.DataSystem.ResultPath;
            tbResultDataPeriod.Text = m_dataService.DataSystem.ResultSave.ToString();

            tbxReportDataPath.Text = m_dataService.DataSystem.ReportPath;

            tbxTop1Path.Text = m_dataService.DataSystem.Top1Path;
            tbxTop2Path.Text = m_dataService.DataSystem.Top2Path;
            tbxBottom1Path.Text = m_dataService.DataSystem.Bottom1Path;
            tbxBottom2Path.Text = m_dataService.DataSystem.Bottom2Path;
            tbxMono1Path.Text = m_dataService.DataSystem.Mono1Path;
            tbxMono2Path.Text = m_dataService.DataSystem.Mono2Path;
            tbxServerPath.Text = m_dataService.DataSystem.ServerPath;

            tbLightTimeTop.Text = m_dataService.DataSystem.LightTimeTop.TotalHours.ToString();
            tbLightTimeBottom.Text = m_dataService.DataSystem.LightTimeBottom.TotalHours.ToString();
            tbLightTimeMono.Text = m_dataService.DataSystem.LightTimeMono.TotalHours.ToString();

            tbTiriggerColor_Period.Text = m_dataService.DataSystem.TriggerColor_Period.ToString("0.0000000");
            tbTiriggerColor_Width.Text = m_dataService.DataSystem.TriggerColor_Width.ToString("0.0");
            tbTiriggerColor_Level.Text = m_dataService.DataSystem.TriggerColor_Level.ToString();

            tbTiriggerMono_Period.Text = m_dataService.DataSystem.TriggerMono_Period.ToString("0.0000000");
            tbTiriggerMono_Width.Text = m_dataService.DataSystem.TriggerMono_Width.ToString("0.0");
            tbTiriggerMono_Level.Text = m_dataService.DataSystem.TriggerMono_Level.ToString();

            tbLaser_X_Pos.Text = m_dataService.DataSystem.LaserXpos.ToString();
            tbLaser_Y_Pos.Text = m_dataService.DataSystem.LaserYpos.ToString();

            tbPunchImageLimit.Text = m_dataService.DataSystem.PunchImageLimit.ToString();
            tbSectionMinUnits.Text = m_dataService.DataSystem.SectionMinUnits.ToString();
            
        }

        private void Save()
        {
            if (null == m_dataService)
                return;

            m_dataService.DataSystem.MachineID              = tbMachineID.Text;
            m_dataService.DataSystem.ProcessID              = tbProcessID.Text;
            m_dataService.DataSystem.ToolID                 = tbToolID.Text;

            m_dataService.DataSystem.UseCheckGuide          = rdCheckGuideUse.IsChecked.Value;
            m_dataService.DataSystem.UseVirtualKeyboard     = rdVirtualKeyboardUse.IsChecked.Value;
            m_dataService.DataSystem.UseSelectParam         = rdSelectParamsUse.IsChecked.Value;
            m_dataService.DataSystem.IsSelectedVision       = rdVisionUse.IsChecked.Value;
            m_dataService.DataSystem.IsSelectedHoleStop     = rdHoleStopUse.IsChecked.Value;
            m_dataService.DataSystem.IsSelectedJointStop    = rdJointStopUse.IsChecked.Value;
            m_dataService.DataSystem.IsSelectedSectionYield = rdSectionYieldUse.IsChecked.Value;
            m_dataService.DataSystem.IsSelectedAlwaysAlign  = rdAlwaysAlignUse.IsChecked.Value;
            m_dataService.DataSystem.IsSelectedCleanRoller  = rdCleanRollerUse.IsChecked.Value;
            m_dataService.DataSystem.IsSelectedMissPrint    = rdMissPrintUse.IsChecked.Value;
            m_dataService.DataSystem.IsSelectedDualErrorStop = rdDualErrorUse.IsChecked.Value;
            m_dataService.DataSystem.IsSelectedModify       = rdModifyUse.IsChecked.Value;
            m_dataService.DataSystem.IsSelectedPunch        = rdPunchUse.IsChecked.Value;            
            m_dataService.DataSystem.IsSelectedTop          = rdTopVisionUse.IsChecked.Value;
            m_dataService.DataSystem.IsSelectedBottom       = rdBottomVisionUse.IsChecked.Value;
            m_dataService.DataSystem.IsSelectedMono         = rdMonoVisionUse.IsChecked.Value;
            m_dataService.DataSystem.IsSelectedPunchInspect = rdPunchInspectUse.IsChecked.Value;
            m_dataService.DataSystem.IsSelectedPunchTolerance = rdPunchToleranceUse.IsChecked.Value;
            m_dataService.DataSystem.IsSelectedCNGPunch     = rdPunchCNGUse.IsChecked.Value;
            m_dataService.DataSystem.PunchInspectUnit       = int.Parse(tbPunchInspectUnit.Text);
            m_dataService.DataSystem.UseJobDoneBuzzer       = rdJobDoneBuzzerUse.IsChecked.Value;
            m_dataService.DataSystem.JobDoneBuzzerTime      = int.Parse(tbJobDoneBuzzer.Text);
            m_dataService.DataSystem.UseLightAlarmBuzzer    = rdLightAlarmBuzzerUse.IsChecked.Value;
            m_dataService.DataSystem.LightAlarmBuzzerTime   = int.Parse(tbLightAlarmBuzzer.Text);
            m_dataService.DataSystem.IsSelectedBackFeeding  = rdBackFeedingUse.IsChecked.Value;            
            m_dataService.DataSystem.PosBackFeeding         = double.Parse(tbBackFeeding.Text);
            m_dataService.DataSystem.IsSelectedAlignTolerance = rdAlignToleranceUse.IsChecked.Value;
            m_dataService.DataSystem.AlignTolerance         = double.Parse(tbxAlignTolerance.Text);
            m_dataService.DataSystem.IsSelectedPunchFirstUnit = rdPunchFirstUnitUse.IsChecked.Value;
            m_dataService.DataSystem.IsSelectedPunchLastUnit = rdPunchLastUnitUse.IsChecked.Value;
            m_dataService.DataSystem.IsSelectedPunchSetUnit = rdPunchSetUnitUse.IsChecked.Value;
            m_dataService.DataSystem.PunchUnitIndex         = int.Parse(tbPunchSetUnit.Text);
            m_dataService.DataSystem.UseHeavyAlarmBuzzer    = rdHeavyAlarmBuzzerUse.IsChecked.Value;
            m_dataService.DataSystem.HeavyAlarmBuzzerTime   = int.Parse(tbHeavyAlarmBuzzer.Text);

            m_dataService.DataSystem.BottomDistance         = double.Parse(tbDistanceTopBottom.Text);
            m_dataService.DataSystem.MonoDistance           = double.Parse(tbDistanceTopMono.Text);
            m_dataService.DataSystem.TopVisionZ             = double.Parse(tbTopVisionZ.Text);
            m_dataService.DataSystem.BottomVisionZ          = double.Parse(tbBottomVisionZ.Text);
            m_dataService.DataSystem.AlignVisionX           = double.Parse(tbAlignVisionX.Text);
            m_dataService.DataSystem.AlignVisionY           = double.Parse(tbAlignVisionY.Text);
            m_dataService.DataSystem.PunchOffsetX           = double.Parse(tbPunchX.Text);
            m_dataService.DataSystem.PunchOffsetY           = double.Parse(tbPunchY.Text);      
            m_dataService.DataSystem.PunchOffsetCenterX     = double.Parse(tbPunchCenterX.Text);
            m_dataService.DataSystem.PunchOffsetCenterY     = double.Parse(tbPunchCenterY.Text);
            m_dataService.DataSystem.PunchOffsetBX          = double.Parse(tbPunchBX.Text);
            m_dataService.DataSystem.PunchOffsetBY          = double.Parse(tbPunchBY.Text);
            m_dataService.DataSystem.UncoilerReference      = double.Parse(tbUncoilerReference.Text);
            m_dataService.DataSystem.RecoilerReference      = double.Parse(tbRecoilerReference.Text);
            m_dataService.DataSystem.BufferLimitN           = double.Parse(tbBufferLimitN.Text);
            m_dataService.DataSystem.BufferLimitP           = double.Parse(tbBufferLimitP.Text);
            m_dataService.DataSystem.PunchStroke            = double.Parse(tbPunchStroke.Text);
            m_dataService.DataSystem.PunchDistance          = double.Parse(tbPunchDistance.Text);
            m_dataService.DataSystem.DelayPunch             = int.Parse(tbPunchDelay.Text);
            m_dataService.DataSystem.DelayFeeding           = int.Parse(tbDelayFeed.Text);
            m_dataService.DataSystem.ScanTolerance          = double.Parse(tbScanTolerance.Text);
            m_dataService.DataSystem.ImageTolerance         = double.Parse(tbImageTolerance.Text);
            m_dataService.DataSystem.IndexTolerance         = double.Parse(tbIndexTolerance.Text);
            m_dataService.DataSystem.ScanDummyTop           = double.Parse(tbScanDummyTop.Text);
            m_dataService.DataSystem.ScanDummyBottom        = double.Parse(tbScanDummyBottom.Text);
            m_dataService.DataSystem.ScanDummyMono          = double.Parse(tbScanDummyMono.Text);
            m_dataService.DataSystem.MotionTolerance        = double.Parse(tbMotionTolerance.Text);
            m_dataService.DataSystem.LogPath                = tbxLogPath.Text;
            m_dataService.DataSystem.LogSave                = int.Parse(tbLogSavePeriod.Text);
            m_dataService.DataSystem.ResultPath             = tbxResultDataPath.Text;
            m_dataService.DataSystem.ResultSave             = int.Parse(tbResultDataPeriod.Text);
            m_dataService.DataSystem.ReportPath             = tbxReportDataPath.Text;
            m_dataService.DataSystem.Top1Path               = tbxTop1Path.Text;
            m_dataService.DataSystem.Top2Path               = tbxTop2Path.Text;
            m_dataService.DataSystem.Bottom1Path            = tbxBottom1Path.Text;
            m_dataService.DataSystem.Bottom2Path            = tbxBottom2Path.Text;
            m_dataService.DataSystem.Mono1Path              = tbxMono1Path.Text;
            m_dataService.DataSystem.Mono2Path              = tbxMono2Path.Text;
            m_dataService.DataSystem.ServerPath             = tbxServerPath.Text;

            if (isPunchCountReset)
                m_dataService.DataSystem.PunchCount = 0;

            if (isLightResetTop)
            {
                int hour = int.Parse(tbLightTimeTop.Text);
                m_dataService.DataSystem.LightTimeTop = new TimeSpan(hour, 0, 0);
            }

            if (isLightResetBottom)
            {
                int hour = int.Parse(tbLightTimeBottom.Text);
                m_dataService.DataSystem.LightTimeBottom = new TimeSpan(hour, 0, 0);
            }

            if (isLightResetMono)
            {
                int hour = int.Parse(tbLightTimeMono.Text);
                m_dataService.DataSystem.LightTimeMono = new TimeSpan(hour, 0, 0);
            }

            m_dataService.DataSystem.TriggerColor_Period = double.Parse(tbTiriggerColor_Period.Text);
            m_dataService.DataSystem.TriggerColor_Width = double.Parse(tbTiriggerColor_Width.Text);
            m_dataService.DataSystem.TriggerColor_Level = int.Parse(tbTiriggerColor_Level.Text);

            m_dataService.DataSystem.TriggerMono_Period = double.Parse(tbTiriggerMono_Period.Text);
            m_dataService.DataSystem.TriggerMono_Width = double.Parse(tbTiriggerMono_Width.Text);
            m_dataService.DataSystem.TriggerMono_Level = int.Parse(tbTiriggerMono_Level.Text);

            m_dataService.DataSystem.LaserXpos = double.Parse(tbLaser_X_Pos.Text);
            m_dataService.DataSystem.LaserYpos = double.Parse(tbLaser_Y_Pos.Text);

            m_dataService.DataSystem.PunchImageLimit = int.Parse(tbPunchImageLimit.Text);
            m_dataService.DataSystem.SectionMinUnits = int.Parse(tbSectionMinUnits.Text);

            m_dataService.DataSystem.Save();

            msgService.ShowMessage((int)EnumSmartIC.LightAlarms.savedone, false);
        }

        private void btnPunchCountReset_Click(object sender, RoutedEventArgs e)
        {
            isPunchCountReset = false;

            if (true == msgService.ShowMessage((int)EnumSmartIC.LightAlarms.resetPunchCount, false))
            {
                isPunchCountReset = true;
            }
        }

        private void btnLightTimeTopSet_Click(object sender, RoutedEventArgs e)
        {
            isLightResetTop = true;
        }

        private void btnLightTimeBottomSet_Click(object sender, RoutedEventArgs e)
        {
            isLightResetBottom = true;
        }

        private void btnLightTimeMonoSet_Click(object sender, RoutedEventArgs e)
        {
            isLightResetMono = true;
        }


    }
}
