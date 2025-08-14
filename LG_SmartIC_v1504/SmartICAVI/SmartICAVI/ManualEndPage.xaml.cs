using System;
using System.IO;
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

namespace SmartICAVI
{
    /// <summary>
    /// ManualEndPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ManualEndPage : Page
    {
        private DataService dataService = null;
        private MsgService msgService = null;
        private MesService mesService = null;

        public ManualEndPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dataService = DataService.Singleton;
            msgService = MsgService.Singleton;
            mesService = MesService.Singleton;


            bool enable = false;
            // 데이터가 존재
            if (0 < dataService.DataResult.Total.CountTotal)
            {
                // 완공이 되지 않았을 경우
                //if (false == dataService.DataResult.IsCompleted)
                {
                    enable = true;
                }
            }

            btnReport.IsEnabled = enable;
            cbxMes.IsEnabled = enable;
            tbxComment.IsEnabled = enable;

            if (true == enable)
            {
                lblLotID.Content = dataService.DataResult.LotID;
                lblTotal.Content = dataService.DataResult.Total.CountTotal.ToString();
                lblGood.Content = dataService.DataResult.Total.CountGood.ToString();
                lblNG.Content = (dataService.DataResult.Total.CountTotal - dataService.DataResult.Total.CountGood).ToString();
                lblJoint.Content = dataService.DataResult.Total.CountJoint.ToString();
                lblYield.Content = (((double)dataService.DataResult.Total.CountGood / (double)dataService.DataResult.Total.CountTotal) * 100.0).ToString("0.00");
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            if (true == msgService.ShowMessage((int)EnumSmartIC.LightAlarms.manualReport, false))
            {
                dataService.DataResult.TimeEnd = DateTime.Now;

                if (true == cbxMes.IsChecked)
                {
                    dataService.CurrentStatus = "MES 데이터 전송중";

                    mesService.Mes.MapData1 = dataService.DataResult.MapData1;
                    mesService.Mes.MapData2 = dataService.DataResult.MapData2;

                    // 2020.07.14 khs - MES 전달시 진행한 Lot에 사용된 펀치 카운트만 보내야 한다. (원호연 선임 요청사항)
                    mesService.Mes.ToolID1Count = dataService.DataSystem.MES_PunchCount;

                    mesService.Mes.LotUnits = dataService.DataResult.Total.CountTotal;
                    mesService.Mes.GoodUnits = dataService.DataResult.Total.CountGood;
                    mesService.Mes.NGUnits = dataService.DataResult.Total.CountTotal - dataService.DataResult.Total.CountGood;
                    mesService.Mes.NextOper = dataService.DataResult.NextProcess = dataService.DataSystem.NextProcess;
                    mesService.Mes.Comment = tbxComment.Text;

                    mesService.Mes.SetCountNGs(dataService.DataResult.CountNGs);
                    //mesService.Mes.CountNG1 = dataService.DataResult.CountNG1;
                    //mesService.Mes.CountNG2 = dataService.DataResult.CountNG2;
                    //mesService.Mes.CountNG3 = dataService.DataResult.CountNG3;
                    //mesService.Mes.CountNG4 = dataService.DataResult.CountNG4;
                    //mesService.Mes.CountNG5 = dataService.DataResult.CountNG5;
                    //mesService.Mes.CountNGA = dataService.DataResult.CountNGA;
                    //mesService.Mes.CountNGB = dataService.DataResult.CountNGB;
                    //mesService.Mes.CountNGC = dataService.DataResult.CountNGC;
                    //mesService.Mes.CountNGD = dataService.DataResult.CountNGD;
                    //mesService.Mes.CountNGE = dataService.DataResult.CountNGE;
                    //mesService.Mes.CountNGF = dataService.DataResult.CountNGF;
                    //mesService.Mes.CountNGH = dataService.DataResult.CountNGH;
                    //mesService.Mes.CountNGJ = dataService.DataResult.CountNGJ;
                    //mesService.Mes.CountNGK = dataService.DataResult.CountNGK;
                    //mesService.Mes.CountNGL = dataService.DataResult.CountNGL;
                    //mesService.Mes.CountNGM = dataService.DataResult.CountNGM;
                    //mesService.Mes.CountNGN = dataService.DataResult.CountNGN;
                    //mesService.Mes.CountNGO = dataService.DataResult.CountNGO;
                    //mesService.Mes.CountNGP = dataService.DataResult.CountNGP;
                    //mesService.Mes.CountNGQ = dataService.DataResult.CountNGQ;
                    //mesService.Mes.CountNGR = dataService.DataResult.CountNGR;
                    //mesService.Mes.CountNGS = dataService.DataResult.CountNGS;
                    //mesService.Mes.CountNGT = dataService.DataResult.CountNGT;
                    //mesService.Mes.CountNGU = dataService.DataResult.CountNGU;
                    //mesService.Mes.CountNGW = dataService.DataResult.CountNGW;
                    //mesService.Mes.CountNGY = dataService.DataResult.CountNGY;

                    mesService.Send(MesService.commands.send_jobend);
                }

                


                btnReport.IsEnabled = false;
                cbxMes.IsEnabled = false;
                tbxComment.IsEnabled = false;


                // Defect File
                dataService.CurrentStatus = "Defect 데이터 전송중...";
                dataService.DataDefect.RecipeName = dataService.DataSystem.RecipeName;
                dataService.DataDefect.LotID = dataService.DataSystem.LotID;
                dataService.DataDefect.UserID = dataService.DataSystem.UserID;
                dataService.DataDefect.ToolID = dataService.DataSystem.ToolID;
                dataService.DataDefect.NextProcess = dataService.DataSystem.NextProcess;

                dataService.DataDefect.PF = dataService.DataRecipe.PF;

                dataService.DataDefect.StartTime = dataService.DataResult.TimeStart;
                dataService.DataDefect.EndTime = dataService.DataResult.TimeEnd;
                dataService.DataDefect.TotalUnits = dataService.DataResult.Total.CountTotal;
                dataService.DataDefect.GoodUnits = dataService.DataResult.Total.CountGood;
                dataService.DataDefect.JointCount = dataService.DataResult.Total.CountJoint;
                dataService.DataDefect.CNGCount = dataService.DataResult.listCNG.Count;// dataService.DataResult.Total.CountContinuousNG;
                dataService.DataDefect.PunchCount = dataService.DataResult.CountPunch;
                dataService.DataDefect.PunchTotalCount = dataService.DataSystem.PunchCount;

                for (int i = 0; i < dataService.DataResult.CountNGs.Length; ++i)
                    dataService.DataDefect.CountNGs[i] = dataService.DataResult.CountNGs[i];

                // Top Light
                for (int i = 0; i < dataService.DataResult.listLightTop.Count; ++i)
                {
                    dataService.DataDefect.AddLightTop(dataService.DataResult.listLightTop[i].X, dataService.DataResult.listLightTop[i].Y);
                }

                // Bottom Light
                for (int i = 0; i < dataService.DataResult.listLightBottom.Count; ++i)
                {
                    dataService.DataDefect.AddLightBottom(dataService.DataResult.listLightBottom[i].X, dataService.DataResult.listLightBottom[i].Y);
                }

                // Mono Light
                for (int i = 0; i < dataService.DataResult.listLightMono.Count; ++i)
                {
                    dataService.DataDefect.AddLightMono(dataService.DataResult.listLightMono[i].X, dataService.DataResult.listLightMono[i].Y);
                }

                dataService.DataDefect.Comment = tbxComment.Text;

                try
                {
                    dataService.DataDefect.Save(dataService.DataSystem.ServerPath);
                }
                catch (Exception exc)
                {
                    Log_Exception.WriteLine("SeqAuto.CopyToServer() => " + exc.Message);
                }


                // Report File
                dataService.CurrentStatus = "Report 데이터 저장중...";
                // 2019.05.22 khs 수동완공시 MapData 생기도록 수정
                //dataService.DataResult.Save(dataService.DataSystem.ReportPath);
                if (0 == dataService.DataResult.Save(dataService.DataSystem.ReportPath))
                {
                    try
                    {
                        File.Copy(dataService.LocalMapPathName, dataService.ServerMapPath + "\\MapData.csv");
                    }
                    catch (Exception exc)
                    {
                        Log_Exception.WriteLine("btnReport_Click(CopyMapData Local to Server) : " + exc.Message);
                    }
                }


                dataService.CurrentStatus = "Defect 데이터 전송 완료";

                dataService.DataResult.IsCompleted = true;

                msgService.ShowMessage((int)EnumSmartIC.LightAlarms.manualReportDone);
            }
        }
    }
}
