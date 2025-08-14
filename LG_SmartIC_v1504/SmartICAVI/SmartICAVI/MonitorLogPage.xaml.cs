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
using System.IO;

namespace SmartICAVI
{
    /// <summary>
    /// MonitorLogPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MonitorLogPage : Page
    {
        private DateTime selectedDate;
        private int logType;

        public MonitorLogPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            selectedDate = DateTime.Now;

            rdLogTrace.IsChecked = true;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }


        private void btnToday_Click(object sender, RoutedEventArgs e)
        {
            calendar.SelectedDate = DateTime.Now;
        }

        private void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Calendar cal = sender as Calendar;

            if (null != cal)
            {
                selectedDate = (DateTime)cal.SelectedDate;

                lblSelectedDay.Content = selectedDate.ToString("yyyy-MM-dd");

                DisplayLog();
            }
        }

        private void rdLogType_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rd = sender as RadioButton;

            if (null != rd)
            {
                logType = int.Parse(rd.Tag.ToString());
                lblTitleSelectedLogType.Content = rd.Content.ToString();

                DisplayLog();
            }
        }

        private void DisplayLog()
        {
            string path;
            string old = "";
            string select = selectedDate.ToString("yyyy-MM-dd");

            if (DateTime.Now.ToString("yyyy-MM-dd") != select)
                old = "\\Old";

            switch (logType)
            {
                case 0:         // Trace
                    path = DataService.Singleton.LogTracePath + old + "\\Trace_" + select + ".txt";
                    break;
                case 1:         // Error
                    path = DataService.Singleton.LogErrorPath + old + "\\Error_" + select + ".txt";
                    break;
                case 2:         // History
                    path = DataService.Singleton.LogHistoryPath + old + "\\History_" + select + ".txt";
                    break;


                default:
                    path = DataService.Singleton.LogTracePath + old + "\\Trace_" + select + ".txt";
                    break;
            }

            FileInfo info = new FileInfo(path);

            if (null != info)
            {
                if (true != info.Exists)
                    MessageBox.Show("해당 파일을 찾을 수 없습니다. " + path);
                else
                {
                    StreamReader reader = null;
                    try
                    {
                        lbxLog.Items.Clear();

                        FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        reader = new StreamReader(stream, Encoding.UTF8);

                        string read = "";
                        while (null != (read = reader.ReadLine()))
                        {
                            lbxLog.Items.Insert(0, read);
                        }
                        reader.Close();
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show("Exception MonitorLogPage.Page_Loaded() => " + exc.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("해당 파일을 찾을 수 없습니다. " + path);
            }
        }
    }
}
