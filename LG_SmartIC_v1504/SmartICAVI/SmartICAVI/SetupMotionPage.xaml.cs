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

using SmartICAVI.UserControls;

namespace SmartICAVI
{
    /// <summary>
    /// SetupMotionPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SetupMotionPage : Page
    {
        private DataService dataService = null;
        private MsgService msgService = null;

        private string oldValue = "";

        private GroupControl gpMotion = null;


        public SetupMotionPage()
        {
            InitializeComponent();
        }

        private void page_Loaded(object sender, RoutedEventArgs e)
        {
            //DoubleAnimation animationMain = new DoubleAnimation();
            //animationMain.Duration = TimeSpan.FromSeconds(0.7); // 시간  
            //animationMain.From = 0;
            //animationMain.To = page.ActualWidth;
            //Storyboard.SetTargetName(animationMain, "page"); // 타겟 컨트롤 설정  
            //Storyboard.SetTargetProperty(animationMain, new PropertyPath(Page.WidthProperty));
            //Storyboard storyboardMain = new Storyboard();
            //storyboardMain.Children.Add(animationMain);
            //storyboardMain.Begin(this);  

            dataService = DataService.Singleton;
            msgService = MsgService.Singleton;

            gpMotion = new GroupControl();

            gpMotion.Add(cbVisionFeed);
            gpMotion.Add(cbVisionTop);
            gpMotion.Add(cbVisionBottom);
            gpMotion.Add(cbPunchFeed);
            gpMotion.Add(cbPunchX);
            gpMotion.Add(cbPunchY);

            cbVisionFeed.IsChecked = false;
            cbVisionTop.IsChecked = false;
            cbVisionBottom.IsChecked = false;
            cbPunchFeed.IsChecked = false;
            cbPunchX.IsChecked = false;
            cbPunchY.IsChecked = false;

            cbVisionFeed.Add((ISelectControl)tbVisionFeed_VelSlow);
            cbVisionFeed.Add((ISelectControl)tbVisionFeed_AccelSlow);
            cbVisionFeed.Add((ISelectControl)tbVisionFeed_VelNormal);
            cbVisionFeed.Add((ISelectControl)tbVisionFeed_AccelNormal);
            cbVisionFeed.Add((ISelectControl)tbVisionFeed_VelMove);
            cbVisionFeed.Add((ISelectControl)tbVisionFeed_AccelMove);
            cbVisionFeed.Add((ISelectControl)tbVisionFeed_VelRapid);
            cbVisionFeed.Add((ISelectControl)tbVisionFeed_AccelRapid);

            cbVisionTop.Add((ISelectControl)tbVisionTop_VelSlow);
            cbVisionTop.Add((ISelectControl)tbVisionTop_AccelSlow);
            cbVisionTop.Add((ISelectControl)tbVisionTop_VelNormal);
            cbVisionTop.Add((ISelectControl)tbVisionTop_AccelNormal);
            cbVisionTop.Add((ISelectControl)tbVisionTop_VelMove);
            cbVisionTop.Add((ISelectControl)tbVisionTop_AccelMove);
            cbVisionTop.Add((ISelectControl)tbVisionTop_VelRapid);
            cbVisionTop.Add((ISelectControl)tbVisionTop_AccelRapid);

            cbVisionBottom.Add((ISelectControl)tbVisionBottom_VelSlow);
            cbVisionBottom.Add((ISelectControl)tbVisionBottom_AccelSlow);
            cbVisionBottom.Add((ISelectControl)tbVisionBottom_VelNormal);
            cbVisionBottom.Add((ISelectControl)tbVisionBottom_AccelNormal);
            cbVisionBottom.Add((ISelectControl)tbVisionBottom_VelMove);
            cbVisionBottom.Add((ISelectControl)tbVisionBottom_AccelMove);
            cbVisionBottom.Add((ISelectControl)tbVisionBottom_VelRapid);
            cbVisionBottom.Add((ISelectControl)tbVisionBottom_AccelRapid);

            
            cbPunchFeed.Add((ISelectControl)tbPunchFeed_VelSlow);
            cbPunchFeed.Add((ISelectControl)tbPunchFeed_AccelSlow);
            cbPunchFeed.Add((ISelectControl)tbPunchFeed_VelNormal);
            cbPunchFeed.Add((ISelectControl)tbPunchFeed_AccelNormal);
            cbPunchFeed.Add((ISelectControl)tbPunchFeed_VelMove);
            cbPunchFeed.Add((ISelectControl)tbPunchFeed_AccelMove);
            cbPunchFeed.Add((ISelectControl)tbPunchFeed_VelRapid);
            cbPunchFeed.Add((ISelectControl)tbPunchFeed_AccelRapid);

            cbPunchX.Add((ISelectControl)tbPunchX_VelSlow);
            cbPunchX.Add((ISelectControl)tbPunchX_AccelSlow);
            cbPunchX.Add((ISelectControl)tbPunchX_VelNormal);
            cbPunchX.Add((ISelectControl)tbPunchX_AccelNormal);
            cbPunchX.Add((ISelectControl)tbPunchX_VelMove);
            cbPunchX.Add((ISelectControl)tbPunchX_AccelMove);
            cbPunchX.Add((ISelectControl)tbPunchX_VelRapid);
            cbPunchX.Add((ISelectControl)tbPunchX_AccelRapid);

            cbPunchY.Add((ISelectControl)tbPunchY_VelSlow);
            cbPunchY.Add((ISelectControl)tbPunchY_AccelSlow);
            cbPunchY.Add((ISelectControl)tbPunchY_VelNormal);
            cbPunchY.Add((ISelectControl)tbPunchY_AccelNormal);
            cbPunchY.Add((ISelectControl)tbPunchY_VelMove);
            cbPunchY.Add((ISelectControl)tbPunchY_AccelMove);
            cbPunchY.Add((ISelectControl)tbPunchY_VelRapid);
            cbPunchY.Add((ISelectControl)tbPunchY_AccelRapid);


            btnCancel_Click(this, null);
        }



        private void page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (null != gpMotion)
            {
                gpMotion.RemoveAll();
            }

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

        private string CheckModify_String(string control, string data, string description)
        {
            if (control != data)
            {
                Log_History.WriteLine("[SetupPage] {2},{0},{1}", data, control, description);
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
                    Log_History.WriteLine("[SetupPage] {2},{0},{1}", data, value, description);
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
                    Log_History.WriteLine("[SetupPage] {2},{0},{1}", data, value, description);
                    return value;
                }
            }

            return data;
        }

        private bool CheckModify_Boolean(bool control, bool data, string description)
        {
            if (control != data)
            {
                Log_History.WriteLine("[SetupPage] {2},{0},{1}", data, control, description);
                return control;
            }

            return data;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if( null == dataService )
                return ;

            int axis = (int)EnumSmartIC.Axis.visionFeed;
            tbVisionFeed_VelSlow.Text = dataService.DataMotion[axis].VelSlow.ToString("0.00");
            tbVisionFeed_AccelSlow.Text = dataService.DataMotion[axis].AccelSlow.ToString("0.00");
            tbVisionFeed_VelNormal.Text = dataService.DataMotion[axis].VelNormal.ToString("0.00");
            tbVisionFeed_AccelNormal.Text = dataService.DataMotion[axis].AccelNormal.ToString("0.00");
            tbVisionFeed_VelMove.Text = dataService.DataMotion[axis].VelMove.ToString("0.00");
            tbVisionFeed_AccelMove.Text = dataService.DataMotion[axis].AccelMove.ToString("0.00");
            tbVisionFeed_VelRapid.Text = dataService.DataMotion[axis].VelRapid.ToString("0.00");
            tbVisionFeed_AccelRapid.Text = dataService.DataMotion[axis].AccelRapid.ToString("0.00");

            axis = (int)EnumSmartIC.Axis.visionTop;
            tbVisionTop_VelSlow.Text = dataService.DataMotion[axis].VelSlow.ToString("0.00");
            tbVisionTop_AccelSlow.Text = dataService.DataMotion[axis].AccelSlow.ToString("0.00");
            tbVisionTop_VelNormal.Text = dataService.DataMotion[axis].VelNormal.ToString("0.00");
            tbVisionTop_AccelNormal.Text = dataService.DataMotion[axis].AccelNormal.ToString("0.00");
            tbVisionTop_VelMove.Text = dataService.DataMotion[axis].VelMove.ToString("0.00");
            tbVisionTop_AccelMove.Text = dataService.DataMotion[axis].AccelMove.ToString("0.00");
            tbVisionTop_VelRapid.Text = dataService.DataMotion[axis].VelRapid.ToString("0.00");
            tbVisionTop_AccelRapid.Text = dataService.DataMotion[axis].AccelRapid.ToString("0.00");

            axis = (int)EnumSmartIC.Axis.visionBottom;
            tbVisionBottom_VelSlow.Text = dataService.DataMotion[axis].VelSlow.ToString("0.00");
            tbVisionBottom_AccelSlow.Text = dataService.DataMotion[axis].AccelSlow.ToString("0.00");
            tbVisionBottom_VelNormal.Text = dataService.DataMotion[axis].VelNormal.ToString("0.00");
            tbVisionBottom_AccelNormal.Text = dataService.DataMotion[axis].AccelNormal.ToString("0.00");
            tbVisionBottom_VelMove.Text = dataService.DataMotion[axis].VelMove.ToString("0.00");
            tbVisionBottom_AccelMove.Text = dataService.DataMotion[axis].AccelMove.ToString("0.00");
            tbVisionBottom_VelRapid.Text = dataService.DataMotion[axis].VelRapid.ToString("0.00");
            tbVisionBottom_AccelRapid.Text = dataService.DataMotion[axis].AccelRapid.ToString("0.00");

            


            axis = (int)EnumSmartIC.Axis.punchFeed;
            tbPunchFeed_VelSlow.Text = dataService.DataMotion[axis].VelSlow.ToString("0.00");
            tbPunchFeed_AccelSlow.Text = dataService.DataMotion[axis].AccelSlow.ToString("0.00");
            tbPunchFeed_VelNormal.Text = dataService.DataMotion[axis].VelNormal.ToString("0.00");
            tbPunchFeed_AccelNormal.Text = dataService.DataMotion[axis].AccelNormal.ToString("0.00");
            tbPunchFeed_VelMove.Text = dataService.DataMotion[axis].VelMove.ToString("0.00");
            tbPunchFeed_AccelMove.Text = dataService.DataMotion[axis].AccelMove.ToString("0.00");
            tbPunchFeed_VelRapid.Text = dataService.DataMotion[axis].VelRapid.ToString("0.00");
            tbPunchFeed_AccelRapid.Text = dataService.DataMotion[axis].AccelRapid.ToString("0.00");

            axis = (int)EnumSmartIC.Axis.punchX;
            tbPunchX_VelSlow.Text = dataService.DataMotion[axis].VelSlow.ToString("0.00");
            tbPunchX_AccelSlow.Text = dataService.DataMotion[axis].AccelSlow.ToString("0.00");
            tbPunchX_VelNormal.Text = dataService.DataMotion[axis].VelNormal.ToString("0.00");
            tbPunchX_AccelNormal.Text = dataService.DataMotion[axis].AccelNormal.ToString("0.00");
            tbPunchX_VelMove.Text = dataService.DataMotion[axis].VelMove.ToString("0.00");
            tbPunchX_AccelMove.Text = dataService.DataMotion[axis].AccelMove.ToString("0.00");
            tbPunchX_VelRapid.Text = dataService.DataMotion[axis].VelRapid.ToString("0.00");
            tbPunchX_AccelRapid.Text = dataService.DataMotion[axis].AccelRapid.ToString("0.00");

            axis = (int)EnumSmartIC.Axis.punchY;
            tbPunchY_VelSlow.Text = dataService.DataMotion[axis].VelSlow.ToString("0.00");
            tbPunchY_AccelSlow.Text = dataService.DataMotion[axis].AccelSlow.ToString("0.00");
            tbPunchY_VelNormal.Text = dataService.DataMotion[axis].VelNormal.ToString("0.00");
            tbPunchY_AccelNormal.Text = dataService.DataMotion[axis].AccelNormal.ToString("0.00");
            tbPunchY_VelMove.Text = dataService.DataMotion[axis].VelMove.ToString("0.00");
            tbPunchY_AccelMove.Text = dataService.DataMotion[axis].AccelMove.ToString("0.00");
            tbPunchY_VelRapid.Text = dataService.DataMotion[axis].VelRapid.ToString("0.00");
            tbPunchY_AccelRapid.Text = dataService.DataMotion[axis].AccelRapid.ToString("0.00");
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (null == dataService)
                return;

            try
            {
                int axis = (int)EnumSmartIC.Axis.visionFeed;
                dataService.DataMotion[axis].VelSlow = double.Parse(tbVisionFeed_VelSlow.Text);
                dataService.DataMotion[axis].AccelSlow = double.Parse(tbVisionFeed_AccelSlow.Text);
                dataService.DataMotion[axis].VelNormal = double.Parse(tbVisionFeed_VelNormal.Text);
                dataService.DataMotion[axis].AccelNormal = double.Parse(tbVisionFeed_AccelNormal.Text);
                dataService.DataMotion[axis].VelMove = double.Parse(tbVisionFeed_VelMove.Text);
                dataService.DataMotion[axis].AccelMove = double.Parse(tbVisionFeed_AccelMove.Text);
                dataService.DataMotion[axis].VelRapid = double.Parse(tbVisionFeed_VelRapid.Text);
                dataService.DataMotion[axis].AccelRapid = double.Parse(tbVisionFeed_AccelRapid.Text);

                axis = (int)EnumSmartIC.Axis.visionTop;
                dataService.DataMotion[axis].VelSlow = double.Parse(tbVisionTop_VelSlow.Text);
                dataService.DataMotion[axis].AccelSlow = double.Parse(tbVisionTop_AccelSlow.Text);
                dataService.DataMotion[axis].VelNormal = double.Parse(tbVisionTop_VelNormal.Text);
                dataService.DataMotion[axis].AccelNormal = double.Parse(tbVisionTop_AccelNormal.Text);
                dataService.DataMotion[axis].VelMove = double.Parse(tbVisionTop_VelMove.Text);
                dataService.DataMotion[axis].AccelMove = double.Parse(tbVisionTop_AccelMove.Text);
                dataService.DataMotion[axis].VelRapid = double.Parse(tbVisionTop_VelRapid.Text);
                dataService.DataMotion[axis].AccelRapid = double.Parse(tbVisionTop_AccelRapid.Text);

                axis = (int)EnumSmartIC.Axis.visionBottom;
                dataService.DataMotion[axis].VelSlow = double.Parse(tbVisionBottom_VelSlow.Text);
                dataService.DataMotion[axis].AccelSlow = double.Parse(tbVisionBottom_AccelSlow.Text);
                dataService.DataMotion[axis].VelNormal = double.Parse(tbVisionBottom_VelNormal.Text);
                dataService.DataMotion[axis].AccelNormal = double.Parse(tbVisionBottom_AccelNormal.Text);
                dataService.DataMotion[axis].VelMove = double.Parse(tbVisionBottom_VelMove.Text);
                dataService.DataMotion[axis].AccelMove = double.Parse(tbVisionBottom_AccelMove.Text);
                dataService.DataMotion[axis].VelRapid = double.Parse(tbVisionBottom_VelRapid.Text);
                dataService.DataMotion[axis].AccelRapid = double.Parse(tbVisionBottom_AccelRapid.Text);


                axis = (int)EnumSmartIC.Axis.punchFeed;
                dataService.DataMotion[axis].VelSlow = double.Parse(tbPunchFeed_VelSlow.Text);
                dataService.DataMotion[axis].AccelSlow = double.Parse(tbPunchFeed_AccelSlow.Text);
                dataService.DataMotion[axis].VelNormal = double.Parse(tbPunchFeed_VelNormal.Text);
                dataService.DataMotion[axis].AccelNormal = double.Parse(tbPunchFeed_AccelNormal.Text);
                dataService.DataMotion[axis].VelMove = double.Parse(tbPunchFeed_VelMove.Text);
                dataService.DataMotion[axis].AccelMove = double.Parse(tbPunchFeed_AccelMove.Text);
                dataService.DataMotion[axis].VelRapid = double.Parse(tbVisionFeed_VelRapid.Text);// double.Parse(tbPunchFeed_VelRapid.Text); // 동시 구동시 문제가 발생하므로 visionFeed 의 최고 속도를 같이 맞춘다.
                dataService.DataMotion[axis].AccelRapid = double.Parse(tbPunchFeed_AccelRapid.Text);

                axis = (int)EnumSmartIC.Axis.punchX;
                dataService.DataMotion[axis].VelSlow = double.Parse(tbPunchX_VelSlow.Text);
                dataService.DataMotion[axis].AccelSlow = double.Parse(tbPunchX_AccelSlow.Text);
                dataService.DataMotion[axis].VelNormal = double.Parse(tbPunchX_VelNormal.Text);
                dataService.DataMotion[axis].AccelNormal = double.Parse(tbPunchX_AccelNormal.Text);
                dataService.DataMotion[axis].VelMove = double.Parse(tbPunchX_VelMove.Text);
                dataService.DataMotion[axis].AccelMove = double.Parse(tbPunchX_AccelMove.Text);
                dataService.DataMotion[axis].VelRapid = double.Parse(tbPunchX_VelRapid.Text);
                dataService.DataMotion[axis].AccelRapid = double.Parse(tbPunchX_AccelRapid.Text);

                axis = (int)EnumSmartIC.Axis.punchY;
                dataService.DataMotion[axis].VelSlow = double.Parse(tbPunchY_VelSlow.Text);
                dataService.DataMotion[axis].AccelSlow = double.Parse(tbPunchY_AccelSlow.Text);
                dataService.DataMotion[axis].VelNormal = double.Parse(tbPunchY_VelNormal.Text);
                dataService.DataMotion[axis].AccelNormal = double.Parse(tbPunchY_AccelNormal.Text);
                dataService.DataMotion[axis].VelMove = double.Parse(tbPunchY_VelMove.Text);
                dataService.DataMotion[axis].AccelMove = double.Parse(tbPunchY_AccelMove.Text);
                dataService.DataMotion[axis].VelRapid = double.Parse(tbPunchY_VelRapid.Text);
                dataService.DataMotion[axis].AccelRapid = double.Parse(tbPunchY_AccelRapid.Text);

                dataService.DataMotion.Save();

                MotionService.Singleton.ApplyParams();

                msgService.ShowMessage((int)EnumSmartIC.LightAlarms.savedone, false);
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine("SetupMotionPage.Save() : " + exc.Message );
            }
        }


    }
}
