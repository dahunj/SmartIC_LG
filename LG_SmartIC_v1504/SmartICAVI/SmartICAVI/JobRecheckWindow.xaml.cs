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
    /// JobRecheckWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class JobRecheckWindow : Window
    {
        private JobWindowService jobWindowService = null;
        private DataService dataService = null;

        private Image img;

        //private int count = 0;

        public int Index { get; set; }
        public string Message { get; set; }

        public JobRecheckWindow()
        {
            InitializeComponent();

            Index = 0;
            Message = "";
        }

        private void messageWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dataService = DataService.Singleton;
            jobWindowService = JobWindowService.Singleton;

            jobWindowService.IsRecheckWindowClosed = false;

            DisplayText();
        }

        private void messageWindow_Closed(object sender, EventArgs e)
        {
            jobWindowService.IsRecheckWindowClosed = true;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;

            jobWindowService.ResultRecheckWindow = 0;

            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;

            jobWindowService.ResultRecheckWindow = -1;
            this.Close();
        }

        private void DisplayText()
        {
            int number = Index;


            string strNumber = string.Format("메시지 코드({0})", (int)number);
            string message;
            string description;
            string imgPath = dataService.DataPath + "\\Images\\LightAlarm";

            string path = dataService.IniPath + "\\LightAlarm.ini";
            string section = "LIGHT_ALARM";
            string key = string.Format("{0}", number);
            string key_description = string.Format("{0}_Description", number);
            string key_Image = string.Format("{0}_Image", number);

            IniFile ini = new IniFile();

            message = ini.Read(section, key, "Unknown Message", path);
            description = ini.Read(section, key_description, "설정되지 않은 메시지입니다. 메시지 번호를 통보 해 주십시오.", path);
            imgPath = dataService.DataPath + "\\Images\\LightAlarm\\" + ini.Read(section, key_Image, "Default.jpg", path);

            description = description.Replace("\\n", Environment.NewLine);

            if ("" != Message)
            {
                description += Environment.NewLine + Environment.NewLine + Message;
            }

            Log_Trace.WriteLine(message);

            //msg.TitleText = title;
            tblMessageNum.Text = strNumber;
            tblMessage.Text = message;
            tblDescription.Text = description;

            img = new Image();

            Uri uriImage = new Uri(imgPath);
            BitmapImage bmp = null;

            try
            {
                bmp = new BitmapImage(uriImage);
            }
            catch (Exception exc)
            {
                // 기본 이미지로 연결해야 함. 
                string error = exc.Message;

                uriImage = new Uri("pack://application:,,/Images/Stop48.png");
                bmp = new BitmapImage(uriImage);
            }

            if (null != bmp)
            {
                image.Source = bmp;
                image.Stretch = Stretch.Fill;
            }
        }
    }
}
