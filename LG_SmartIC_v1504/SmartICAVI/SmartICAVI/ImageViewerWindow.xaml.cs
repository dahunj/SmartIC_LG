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
using System.IO;

namespace SmartICAVI
{
    /// <summary>
    /// ImageViewerWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImageViewerWindow : Window
    {
        private MemoryStream msColor = null;
        private MemoryStream msHSI = null;

        public string PathImage1
        {
            set
            {
                if (null != value)
                {
                    if ("" == value)
                    {
                        lbxImage1.Visibility = System.Windows.Visibility.Collapsed;
                        uniGrid.Columns = 1;
                    }
                    else
                    {
                        // Image Load
                        try
                        {
                            if (null != msColor)
                            {
                                msColor.Close();
                                msColor = null;
                            }

                            msColor = new MemoryStream();
                            FileStream stream = new FileStream(value, FileMode.Open, FileAccess.Read);

                            msColor.SetLength(stream.Length);

                            stream.Read(msColor.GetBuffer(), 0, (int)stream.Length);

                            msColor.Flush();
                            stream.Close();

                            BitmapImage bmpImage = new BitmapImage();
                            bmpImage.BeginInit();
                            bmpImage.StreamSource = msColor;
                            bmpImage.EndInit();

                            image1.Source = bmpImage;
                            lbxImage1.SelectedIndex = 0;
                        }
                        catch (Exception exc)
                        {
                            Log_Exception.WriteLine("ImageViewerWindow.PathImage1 : " + exc.Message);
                        }

                        lbxImage1.Focus();
                    }
                }
            }
        }

        public string PathImage2
        {
            set
            {
                if (null != value)
                {

                    if ("" == value)
                    {
                        lbxImage2.Visibility = System.Windows.Visibility.Collapsed;
                        uniGrid.Columns = 1;
                    }
                    else
                    {

                        try
                        {
                            if (null != msHSI)
                            {
                                msHSI.Close();
                                msHSI = null;
                            }

                            msHSI = new MemoryStream();
                            FileStream stream = new FileStream(value, FileMode.Open, FileAccess.Read);

                            msHSI.SetLength(stream.Length);

                            stream.Read(msHSI.GetBuffer(), 0, (int)stream.Length);

                            msHSI.Flush();
                            stream.Close();

                            BitmapImage bmpImage = new BitmapImage();
                            bmpImage.BeginInit();
                            bmpImage.StreamSource = msHSI;
                            bmpImage.EndInit();

                            image2.Source = bmpImage;
                            lbxImage2.SelectedIndex = 0;
                        }
                        catch (Exception exc)
                        {
                            Log_Exception.WriteLine("ImageViewerWindow.PathImage2 : " + exc.Message);
                        }
                    }
                }
            }
        }

        public ImageViewerWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (null != msColor)
            {
                msColor.Close();
                msColor = null;
            }

            if (null != msHSI)
            {
                msHSI.Close();
                msHSI = null;
            }

            GC.Collect();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Return:// = 6
                    Close();
                    break;
                case Key.Escape:// = 13
                    Close();
                    break;
                case Key.Left:// = 23,
                    break;
                case Key.Up:// = 24,
                    break;
                case Key.Right:// = 25,
                    break;
                case Key.Down:// = 26,
                    break;
                
                default:
                    break;
            }
        }
    }
}
