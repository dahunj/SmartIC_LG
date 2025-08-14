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

namespace SmartICAVI
{
    /// <summary>
    /// MonitorWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MonitorWindow : Window
    {
        public MonitorWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Log_Trace.WriteLine("[MonitorWindow] Show");
            //DataService dataService = DataService.Singleton;

            //switch (dataService.DataGUI.SelectedMonitor)
            //{
            //    case 0:         // Motion
                    btnCheckBox_Click((object)btnMotion, null);
            //        break;
            //    case 1:         // Dio
            //        btnCheckBox_Click((object)btnDio, null);
            //        break;
            //    case 2:         // Commu
            //        btnCheckBox_Click((object)btnCommu, null);
            //        break;
            //    case 3:         // Log
            //        btnCheckBox_Click((object)btnLog, null);
            //        break;
            //    case 4:         // System
            //        btnCheckBox_Click((object)btnSystem, null);
            //        break;
            //    default:
            //        btnCheckBox_Click((object)btnMotion, null);
            //        break;
            //}
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            Log_Trace.WriteLine("[MonitorWindow] Hide");
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            this.Topmost = false;
        }


        private void btnCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cBox = sender as CheckBox;
            //DataService dataService = DataService.Singleton;

            if (null != cBox)
            {
                btnMotion.IsChecked = false;
                btnDio.IsChecked = false;
                btnCommu.IsChecked = false;
                btnLog.IsChecked = false;
                btnSystem.IsChecked = false;

                string page;

                switch (cBox.Tag.ToString())
                {
                    case "Motion":
                        Log_Trace.WriteLine("[MonitorWindow] Click Motion");
                        btnMotion.IsChecked = true;
                        //dataService.DataGUI.SelectedMonitor = 0;
                        page = "MonitorMotionPage.xaml";
                        break;
                    case "Dio":
                        Log_Trace.WriteLine("[MonitorWindow] Click Dio");
                        btnDio.IsChecked = true;
                        //dataService.DataGUI.SelectedMonitor = 1;
                        page = "MonitorDioPage.xaml";
                        break;
                    case "Commu":
                        Log_Trace.WriteLine("[MonitorWindow] Click Commu");
                        btnCommu.IsChecked = true;
                        //dataService.DataGUI.SelectedMonitor = 2;
                        page = "MonitorMESPage.xaml";
                        break;
                    case "Log":
                        Log_Trace.WriteLine("[MonitorWindow] Click Log");
                        btnLog.IsChecked = true;
                        //dataService.DataGUI.SelectedMonitor = 3;
                        page = "MonitorLogPage.xaml";
                        break;
                    case "System":
                        Log_Trace.WriteLine("[MonitorWindow] Click System");
                        btnSystem.IsChecked = true;
                        //dataService.DataGUI.SelectedMonitor = 4;
                        page = "MonitorSystemPage.xaml";
                        break;
                    default:
                        btnMotion.IsChecked = true;
                        //dataService.DataGUI.SelectedMonitor = 0;
                        page = "MonitorMotionPage.xaml";
                        break;
                }

                monitorFrame.Source = new Uri(page, UriKind.Relative);
                //dataService.DataGUI.Save();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;
            this.Close();
        }
    }
}
