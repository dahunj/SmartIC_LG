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

using SmartICAVI.Animation;

namespace SmartICAVI
{
    /// <summary>
    /// SetupPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SetupPage : Page
    {
        private DataService dataService = null;

        public SetupPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dataService = DataService.Singleton;

            btnSystem.IsChecked = true;

            if (0 == dataService.LogonType)
            {
                btnUserAccount.IsEnabled = false;
            }
            else
            {
                btnUserAccount.IsEnabled = true;
            }

            dataService.LogonTime = DateTime.Now;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            dataService.LogonTime = DateTime.Now;
        }

        private void btnCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cBox = sender as CheckBox;

            if (false == cBox.IsChecked)
            {
                cBox.IsChecked = true;
                return;
            }
            
            if (null != cBox)
            {
                btnMotion.IsChecked = false;
                btnMes.IsChecked = false;
                btnSystem.IsChecked = false;
                btnVerify.IsChecked = false;
                btnUserAccount.IsChecked = false;


                cBox.IsChecked = true;

                string page;
                //Page targetPage;

                switch (cBox.Tag.ToString())
                {
                    case "btnMes":
                        page = "SetupMESPage.xaml";
                        //targetPage = new SetupMESPage();
                        break;
                    case "btnMotion":
                        page = "SetupMotionPage.xaml";
                        //targetPage = new SetupMotionPage();
                        break;
                    case "btnSystem":
                        page = "SetupSystemPage.xaml";
                        //targetPage = new SetupSystemPage();
                        break;
                    case "btnVerify":
                        page = "SetupVerifyPage.xaml";
                        break;
                    case "btnUserAccount":
                        page = "SetupUserAccountPage.xaml";
                        break;
                    default:
                        return;
                }

                setupFrame.Source = new Uri(page, UriKind.Relative);
                //this.setupFrame.NavigateToExample(targetPage);
            }
        }
    }
}
