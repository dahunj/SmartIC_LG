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

namespace SmartICAVI
{
    /// <summary>
    /// InputWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InputWindow : Window
    {
        public event EventHandler CloseEvent;

        private DioService dioService = null;
/*
        private Image img;
        private string imgPath;


        public string NumberText
        {
            get
            {
                return tblMessageNum.Text;
            }
            set
            {
                tblMessageNum.Text = value;
            }
        }
*/
        public string Message
        {
            get
            {
                return tblMessage.Text;
            }
            set
            {
                tblMessage.Text = value;
            }
        }
        public string Description
        {
            get
            {
                return tblDescription.Text;
            }

            set
            {
                tblDescription.Text = value;
            }
        }
/*
        public string ImagePath
        {
            get
            {
                return imgPath;
            }
            set
            {
                imgPath = value;

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

                    //img.Width = canvas.ActualWidth;
                    //img.Height = canvas.ActualHeight;

                    //canvas.Children.Add(img);
                    //Canvas.SetLeft(img, 0);
                    //Canvas.SetTop(img, 0);
                }
            }
        }
*/
        public int Number { get; set; }

        public bool Modalless { get; set; }

        public InputWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dioService = DioService.Singleton;
            dioService.SetOutport((int)DioService.Outports.twBuzzer);
            Thread.Sleep(1000);
            dioService.ResetOutport((int)DioService.Outports.twBuzzer);
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }


        private void inputWindow_Closed(object sender, EventArgs e)
        {
            this.Topmost = false;
            FireCloseEvent();
        }


        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //DragMove();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (tbInput.Text == "")
            {
                return;
            }

            Log_LastPunchCheck.WriteLine(tbInput.Text);

            this.Topmost = false;

            if (false == Modalless)
                this.DialogResult = true;

            this.Close();
        }

        private void FireCloseEvent()
        {
            if (null != CloseEvent)
                CloseEvent(this, null);
        }
    }
}
