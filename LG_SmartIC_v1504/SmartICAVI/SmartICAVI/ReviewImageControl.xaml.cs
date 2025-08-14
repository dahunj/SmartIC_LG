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
    /// ReviewImageControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ReviewImageControl : UserControl
    {
        private string pathColor;
        private string pathHSI;
        private int imgWidth;
        private int imgHeight;
        private int columns;

        private MemoryStream msColor = null;
        private MemoryStream msHSI = null;
        
        public string ColorName { get; set; }
        public string HSIName { get; set; }

        #region Origin
        //public string PathColor
        //{
        //    get
        //    {
        //        return pathColor;
        //    }
        //    set
        //    {
        //        pathColor = value;

        //        Uri uriImage = null;// new Uri(pathColor);
        //        BitmapImage bmpImage = null;

        //        //ImageSource source = new ImageSource

        //        // Image Load
        //        try
        //        {
        //            uriImage = new Uri(pathColor);
        //            bmpImage = new BitmapImage(uriImage);
        //        }
        //        catch (Exception exc)
        //        {
        //            uriImage = new Uri("pack://application:,,/Images/Stop48.png");//new Uri("pack://application:,,/Images/Stop48.png");
        //            bmpImage = new BitmapImage(uriImage);
        //        }

        //        if (null != bmpImage)
        //        {

        //            //bmpColor = bmpImage.Clone();
        //            //imageColor.Source = bmpImage.Clone();
        //            imageColor.Source = bmpImage;
        //        }

        //        uriImage = null;
        //        bmpImage = null;

        //        //System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(pathColor);

        //        //System.Drawing.Imaging.
        //        //System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(pathColor);
        //        //IntPtr hBitmap = bitmap.GetHbitmap();

        //        //System.Windows.Media.ImageDrawing img;



        //        //ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()); ;
        //        //img.Source = wpfBitmap;
        //        //bitmap.Dispose();


        //        //GC.Collect();
        //    }
        //}

        //public string PathHSI
        //{
        //    get
        //    {
        //        return pathHSI;
        //    }
        //    set
        //    {
        //        pathHSI = value;

        //        Uri uriImage = null;// new Uri(pathHSI);
        //        BitmapImage bmpImage = null;

        //        // Image Load
        //        try
        //        {
        //            uriImage = new Uri(pathHSI);
        //            bmpImage = new BitmapImage(uriImage);
        //        }
        //        catch (Exception exc)
        //        {
        //            uriImage = new Uri("pack://application:,,/Images/Stop48.png");
        //            bmpImage = new BitmapImage(uriImage);
        //        }

        //        if (null != bmpImage)
        //        {
        //            //bmpHSI = bmpImage.Clone();
        //            //imageHSI.Source = bmpImage.Clone();
        //            imageHSI.Source = bmpImage;
        //        }

        //        uriImage = null;
        //        bmpImage = null;

        //        //GC.Collect();
        //    }
        //}
        #endregion

        #region Test1_Stream
        public string PathColor
        {
            get
            {
                return pathColor;
            }
            set
            {
                pathColor = value;

                try
                {
                    if (File.Exists(pathColor))
                    {
                        if (null != msColor)
                        {
                            msColor.Close();
                            msColor = null;
                        }

                        msColor = new MemoryStream();
                        FileStream stream = new FileStream(pathColor, FileMode.Open, FileAccess.Read);

                        msColor.SetLength(stream.Length);

                        stream.Read(msColor.GetBuffer(), 0, (int)stream.Length);

                        msColor.Flush();
                        stream.Close();

                        BitmapImage bmpImage = new BitmapImage();
                        bmpImage.BeginInit();
                        bmpImage.StreamSource = msColor;
                        bmpImage.EndInit();
                        imageColor.Source = bmpImage;
                    }
                }
                catch (Exception exc)
                {
                    Log_Exception.WriteLine("ReviewImageControl.PathColor : " + exc.Message);
                }
            }
        }

        public string PathHSI
        {
            get
            {
                return pathHSI;
            }
            set
            {
                pathHSI = value;

                try
                {
                    if (File.Exists(pathHSI))
                    {
                        if (null != msHSI)
                        {
                            msHSI.Close();
                            msHSI = null;
                        }

                        msHSI = new MemoryStream();
                        FileStream stream = new FileStream(pathHSI, FileMode.Open, FileAccess.Read);

                        msHSI.SetLength(stream.Length);

                        stream.Read(msHSI.GetBuffer(), 0, (int)stream.Length);

                        msHSI.Flush();
                        stream.Close();

                        BitmapImage bmpImage = new BitmapImage();
                        bmpImage.BeginInit();
                        bmpImage.StreamSource = msHSI;
                        bmpImage.EndInit();
                        imageHSI.Source = bmpImage;

                    }
                }
                catch (Exception exc)
                {
                    Log_Exception.WriteLine("ReviewImageControl.PathColor : " + exc.Message);
                }
            }
        }
        #endregion

        #region Test2_OnLoad
        //public string PathColor
        //{
        //    get
        //    {
        //        return pathColor;
        //    }
        //    set
        //    {
        //        pathColor = value;

        //        Uri uriImage = null;// new Uri(pathColor);
        //        BitmapImage bmpImage = null;

        //        //ImageSource source = new ImageSource

        //        // Image Load
        //        try
        //        {
        //            //bmpImage = new BitmapImage(uriImage);
        //            bmpImage = new BitmapImage();
        //            bmpImage.BeginInit();
        //            uriImage = new Uri(pathColor);
        //            bmpImage.UriSource = uriImage;
        //            bmpImage.CacheOption = BitmapCacheOption.OnLoad;
        //            bmpImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
        //            bmpImage.EndInit();


        //        }
        //        catch (Exception exc)
        //        {
        //            uriImage = new Uri("pack://application:,,/Images/Stop48.png");
        //            bmpImage = new BitmapImage(uriImage);
        //        }

        //        if (null != bmpImage)
        //        {

        //            //bmpColor = bmpImage.Clone();
        //            //imageColor.Source = bmpImage.Clone();
        //            imageColor.Source = bmpImage;
        //        }

        //        uriImage = null;
        //        bmpImage = null;

        //        //GC.Collect();
        //    }
        //}

        //public string PathHSI
        //{
        //    get
        //    {
        //        return pathHSI;
        //    }
        //    set
        //    {
        //        pathHSI = value;

        //        Uri uriImage = null;// new Uri(pathHSI);
        //        BitmapImage bmpImage = null;

        //        // Image Load
        //        //try
        //        //{
        //        //    bmpImage = new BitmapImage();
        //        //    bmpImage.BeginInit();
        //        //    uriImage = new Uri(pathHSI);
        //        //    bmpImage.UriSource = uriImage;
        //        //    bmpImage.CacheOption = BitmapCacheOption.OnLoad;
        //        //    bmpImage.EndInit();


        //        //}
        //        //catch (Exception exc)
        //        {
        //            uriImage = new Uri("pack://application:,,/Images/Stop48.png");
        //            bmpImage = new BitmapImage(uriImage);
        //        }

        //        if (null != bmpImage)
        //        {
        //            //bmpHSI = bmpImage.Clone();
        //            //imageHSI.Source = bmpImage.Clone();
        //            imageHSI.Source = bmpImage;
        //        }

        //        uriImage = null;
        //        bmpImage = null;

        //        //GC.Collect();
        //    }
        //}
        #endregion

        public int ImageWidth
        {
            get
            {
                return imgWidth;
            }
            set
            {
                imgWidth = value;

                imageColor.Width = value;
                imageHSI.Width = value;
            }
        }

        public int ImageHeight
        {
            get
            {
                return imgHeight;
            }
            set
            {
                imgHeight = value;

                imageColor.Height = value;
                imageHSI.Height = value;
            }
        }

        public int Columns
        {
            get
            {
                return columns;
            }
            set
            {
                columns = value;

                if (0 == columns)
                    uniGrid.Columns = 1;
                else
                    uniGrid.Columns = 2;
            }
        }

        public bool IsVisibleColor
        {
            get
            {
                if (imageColor.Visibility == System.Windows.Visibility.Visible)
                    return true;
                else
                    return false;
            }
            set
            {
                if (true == value)
                    imageColor.Visibility = System.Windows.Visibility.Visible;
                else
                    imageColor.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public bool IsVisibleHSI
        {
            get
            {
                if (imageHSI.Visibility == System.Windows.Visibility.Visible)
                    return true;
                else
                    return false;
            }
            set
            {
                if (true == value)
                    imageHSI.Visibility = System.Windows.Visibility.Visible;
                else
                    imageHSI.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public string Title
        {
            get
            {
                return lblTitle.Content.ToString();
            }
            set
            {
                lblTitle.Content = value;
            }
        }

        public Brush TitleForeground
        {
            get
            {
                return lblTitle.Foreground;
            }
            set
            {
                lblTitle.Foreground = value;
            }
        }

        public double TitleFontSize
        {
            get
            {
                return lblTitle.FontSize;
            }
            set
            {
                lblTitle.FontSize = value;
            }
        }

        public ReviewImageControl()
        {
            InitializeComponent();

            imgWidth = 150;
            imgHeight = 150;
            columns = 0;

            imageColor.Width = imgWidth;
            imageHSI.Width = imgWidth;
            imageColor.Height = imgHeight;
            imageHSI.Height = imgHeight;

            //if (0 == columns)
            uniGrid.Columns = 1;
            //else
            //    uniGrid.Columns = 2;

            //bmpColor = new BitmapImage();
            //bmpHSI = new BitmapImage();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //imageColor.Source = null;
            //imageHSI.Source = null;

            //Dispose();
        }

        public void Dispose()
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

            imageColor.Source = null;
            imageHSI.Source = null;
            
            //GC.Collect();
        }
    }
}
