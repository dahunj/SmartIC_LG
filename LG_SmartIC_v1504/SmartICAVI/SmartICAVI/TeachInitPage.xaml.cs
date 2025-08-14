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
using Microsoft.Win32;

using SmartICAVI.UserControls;

namespace SmartICAVI
{
    /// <summary>
    /// TeachInitPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TeachInitPage : Page
    {
        private string oldValue = "";

        private DataService dataService = null;
        private MsgService msgService = null;

        private DataRecipe recipe = null;

        public TextBlock RecipeName { get; set; }

        public TeachInitPage()
        {
            InitializeComponent();
            recipe = new DataRecipe();
            RecipeName = new TextBlock();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dataService = DataService.Singleton;
            msgService = MsgService.Singleton;

            recipe = dataService.DataTeach;
            tblRecipeName.Text = dataService.TeachName;

            DisplayParams();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            ApplyParams();

            GC.Collect();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Multiselect = false;
            openFileDlg.Filter = "레시피파일(*.rcp)|*.rcp";
            openFileDlg.Title = "레시피(모델) 파일 열기";
            openFileDlg.DefaultExt = "rcp";
            openFileDlg.InitialDirectory = dataService.RecipePath;

            if (true == openFileDlg.ShowDialog())
            {
                tblRecipeName.Text = openFileDlg.SafeFileName.Replace(".rcp", "");

                recipe = new DataRecipe();
                if (0 == recipe.Load(openFileDlg.FileName))
                {
                    DisplayParams();
                }
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            TeachRecipeNameWindow win = new TeachRecipeNameWindow();

            if (true == win.ShowDialog())
            {
                tblRecipeName.Text = win.RecipeName;
                recipe = new DataRecipe();
                recipe.LoadDefault();

                DisplayParams();
            }
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            TeachRecipeNameWindow win = new TeachRecipeNameWindow();

            if (true == win.ShowDialog())
            {
                tblRecipeName.Text = win.RecipeName;

                DisplayParams();
            }
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

        private void btnScanLength_Click(object sender, RoutedEventArgs e)
        {
            double length = double.Parse(tbxScanUnit.Text) * double.Parse(tbxPF.Text);
            tblScanLength.Text = (length * 4.75).ToString("0.000");

            // Temp Test
            dataService.DataResult.Save(dataService.DataSystem.ReportPath);
        }

        private void DisplayParams()
        {
            tbxPunchPosX.Text = recipe.PunchX.ToString("0.000");
            tbxPunchPosY.Text = recipe.PunchY.ToString("0.000");
            tbxPunchCenterPosX.Text = recipe.PunchCenterX.ToString("0.000");
            tbxPunchCenterPosY.Text = recipe.PunchCenterY.ToString("0.000");
            tbxPF.Text = recipe.PF.ToString();
            tbxLineNo.Text = recipe.Line.ToString();
            tbxContinuousNG.Text = recipe.NGContinue.ToString();
            tbxSectionMinUnits.Text = recipe.SectionMinUnits.ToString();
            tbxScanUnit.Text = recipe.ScanUnits.ToString();
            tbxPreScan.Text = recipe.PreScan.ToString("0.000");

            tblScanLength.Text = ((double)(recipe.ScanUnits*recipe.PF) * 4.75).ToString("0.000");

            tbxOffsetInitX.Text = recipe.OffsetInitX.ToString("0.000");
            tbxSearchArea.Text = recipe.SearchAreaSize.ToString("0.000");

            tbxPunchToleranceX.Text = recipe.PunchToleranceX.ToString("0.000");
            tbxPunchToleranceY.Text = recipe.PunchToleranceY.ToString("0.000");

            RecipeName.Text = tblRecipeName.Text;

            tbxSectionUnit1.Text = recipe.sectionUnits[0].ToString();
            tbxSectionUnit2.Text = recipe.sectionUnits[1].ToString();
            tbxSectionUnit3.Text = recipe.sectionUnits[2].ToString();
            tbxSectionUnit4.Text = recipe.sectionUnits[3].ToString();
            tbxSectionUnit5.Text = recipe.sectionUnits[4].ToString();
            tbxSectionUnit6.Text = recipe.sectionUnits[5].ToString();
            tbxSectionUnit7.Text = recipe.sectionUnits[6].ToString();
            tbxSectionUnit8.Text = recipe.sectionUnits[7].ToString();
            tbxSectionUnit9.Text = recipe.sectionUnits[8].ToString();
            tbxSectionUnit10.Text = recipe.sectionUnits[9].ToString();
            tbxSectionUnit11.Text = recipe.sectionUnits[10].ToString();
            tbxSectionUnit12.Text = recipe.sectionUnits[11].ToString();
            tbxSectionUnit13.Text = recipe.sectionUnits[12].ToString();
            tbxSectionUnit14.Text = recipe.sectionUnits[13].ToString();
            tbxSectionUnit15.Text = recipe.sectionUnits[14].ToString();
            tbxSectionUnit16.Text = recipe.sectionUnits[15].ToString();
            tbxSectionUnit17.Text = recipe.sectionUnits[16].ToString();
            tbxSectionUnit18.Text = recipe.sectionUnits[17].ToString();
            tbxSectionUnit19.Text = recipe.sectionUnits[18].ToString();
            tbxSectionUnit20.Text = recipe.sectionUnits[19].ToString();
            tbxSectionUnitMin.Text = recipe.minSectionUnit.ToString();

            tbxSectionYield1.Text = recipe.sectionYields[0].ToString("0.00");
            tbxSectionYield2.Text = recipe.sectionYields[1].ToString("0.00");
            tbxSectionYield3.Text = recipe.sectionYields[2].ToString("0.00");
            tbxSectionYield4.Text = recipe.sectionYields[3].ToString("0.00");
            tbxSectionYield5.Text = recipe.sectionYields[4].ToString("0.00");
            tbxSectionYield6.Text = recipe.sectionYields[5].ToString("0.00");
            tbxSectionYield7.Text = recipe.sectionYields[6].ToString("0.00");
            tbxSectionYield8.Text = recipe.sectionYields[7].ToString("0.00");
            tbxSectionYield9.Text = recipe.sectionYields[8].ToString("0.00");
            tbxSectionYield10.Text = recipe.sectionYields[9].ToString("0.00");
            tbxSectionYield11.Text = recipe.sectionYields[10].ToString("0.00");
            tbxSectionYield12.Text = recipe.sectionYields[11].ToString("0.00");
            tbxSectionYield13.Text = recipe.sectionYields[12].ToString("0.00");
            tbxSectionYield14.Text = recipe.sectionYields[13].ToString("0.00");
            tbxSectionYield15.Text = recipe.sectionYields[14].ToString("0.00");
            tbxSectionYield16.Text = recipe.sectionYields[15].ToString("0.00");
            tbxSectionYield17.Text = recipe.sectionYields[16].ToString("0.00");
            tbxSectionYield18.Text = recipe.sectionYields[17].ToString("0.00");
            tbxSectionYield19.Text = recipe.sectionYields[18].ToString("0.00");
            tbxSectionYield20.Text = recipe.sectionYields[19].ToString("0.00");
            tbxSectionYieldMin.Text = recipe.minSectionYield.ToString("0.00");
        }

        private void ApplyParams()
        {
            recipe.PunchX = double.Parse(tbxPunchPosX.Text);
            recipe.PunchY = double.Parse(tbxPunchPosY.Text);
            recipe.PunchCenterX = double.Parse(tbxPunchCenterPosX.Text);
            recipe.PunchCenterY = double.Parse(tbxPunchCenterPosY.Text);
            recipe.PF = int.Parse(tbxPF.Text);
            recipe.Line = int.Parse(tbxLineNo.Text);
            recipe.NGContinue = int.Parse(tbxContinuousNG.Text);
            recipe.SectionMinUnits = int.Parse(tbxSectionMinUnits.Text);
            recipe.ScanUnits = int.Parse(tbxScanUnit.Text);
            recipe.PreScan = double.Parse(tbxPreScan.Text);

            recipe.OffsetInitX = double.Parse(tbxOffsetInitX.Text);
            recipe.SearchAreaSize = double.Parse(tbxSearchArea.Text);

            recipe.PunchToleranceX = double.Parse(tbxPunchToleranceX.Text);
            recipe.PunchToleranceY = double.Parse(tbxPunchToleranceY.Text);

            dataService.TeachName = tblRecipeName.Text;
            dataService.DataTeach = recipe;


            recipe.sectionUnits[0] = int.Parse(tbxSectionUnit1.Text);
            recipe.sectionUnits[1] = int.Parse(tbxSectionUnit2.Text);
            recipe.sectionUnits[2] = int.Parse(tbxSectionUnit3.Text);
            recipe.sectionUnits[3] = int.Parse(tbxSectionUnit4.Text);
            recipe.sectionUnits[4] = int.Parse(tbxSectionUnit5.Text);
            recipe.sectionUnits[5] = int.Parse(tbxSectionUnit6.Text);
            recipe.sectionUnits[6] = int.Parse(tbxSectionUnit7.Text);
            recipe.sectionUnits[7] = int.Parse(tbxSectionUnit8.Text);
            recipe.sectionUnits[8] = int.Parse(tbxSectionUnit9.Text);
            recipe.sectionUnits[9] = int.Parse(tbxSectionUnit10.Text);
            recipe.sectionUnits[10] = int.Parse(tbxSectionUnit11.Text);
            recipe.sectionUnits[11] = int.Parse(tbxSectionUnit12.Text);
            recipe.sectionUnits[12] = int.Parse(tbxSectionUnit13.Text);
            recipe.sectionUnits[13] = int.Parse(tbxSectionUnit14.Text);
            recipe.sectionUnits[14] = int.Parse(tbxSectionUnit15.Text);
            recipe.sectionUnits[15] = int.Parse(tbxSectionUnit16.Text);
            recipe.sectionUnits[16] = int.Parse(tbxSectionUnit17.Text);
            recipe.sectionUnits[17] = int.Parse(tbxSectionUnit18.Text);
            recipe.sectionUnits[18] = int.Parse(tbxSectionUnit19.Text);
            recipe.sectionUnits[19] = int.Parse(tbxSectionUnit20.Text);
            recipe.minSectionUnit = int.Parse(tbxSectionUnitMin.Text);

            recipe.sectionYields[0] = double.Parse(tbxSectionYield1.Text);
            recipe.sectionYields[1] = double.Parse(tbxSectionYield2.Text);
            recipe.sectionYields[2] = double.Parse(tbxSectionYield3.Text);
            recipe.sectionYields[3] = double.Parse(tbxSectionYield4.Text);
            recipe.sectionYields[4] = double.Parse(tbxSectionYield5.Text);
            recipe.sectionYields[5] = double.Parse(tbxSectionYield6.Text);
            recipe.sectionYields[6] = double.Parse(tbxSectionYield7.Text);
            recipe.sectionYields[7] = double.Parse(tbxSectionYield8.Text);
            recipe.sectionYields[8] = double.Parse(tbxSectionYield9.Text);
            recipe.sectionYields[9] = double.Parse(tbxSectionYield10.Text);
            recipe.sectionYields[10] = double.Parse(tbxSectionYield11.Text);
            recipe.sectionYields[11] = double.Parse(tbxSectionYield12.Text);
            recipe.sectionYields[12] = double.Parse(tbxSectionYield13.Text);
            recipe.sectionYields[13] = double.Parse(tbxSectionYield14.Text);
            recipe.sectionYields[14] = double.Parse(tbxSectionYield15.Text);
            recipe.sectionYields[15] = double.Parse(tbxSectionYield16.Text);
            recipe.sectionYields[16] = double.Parse(tbxSectionYield17.Text);
            recipe.sectionYields[17] = double.Parse(tbxSectionYield18.Text);
            recipe.sectionYields[18] = double.Parse(tbxSectionYield19.Text);
            recipe.sectionYields[19] = double.Parse(tbxSectionYield20.Text);
            recipe.minSectionYield = double.Parse(tbxSectionYieldMin.Text);
        }
    }
}
