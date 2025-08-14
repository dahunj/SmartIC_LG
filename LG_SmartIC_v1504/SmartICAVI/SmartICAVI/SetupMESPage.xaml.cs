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
using System.Windows.Threading;
using System.Threading;

using SmartICAVI.UserControls;

namespace SmartICAVI
{
    /// <summary>
    /// SetupMESPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SetupMESPage : Page
    {
        private DataService dataService = null;
        private MsgService msgService = null;

        private string oldValue = "";

        
        private new bool IsInitialized { get; set; }

        public SetupMESPage()
        {
            InitializeComponent();

            IsInitialized = false;
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


            btnCancel_Click(this, null);

            IsInitialized = true;
        }

        private void page_Unloaded(object sender, RoutedEventArgs e)
        {
            IsInitialized = false;
            
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
            if (true == IsInitialized)
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
        }

        private string CheckModify_String(string control, string data, string description)
        {
            if (control != data)
            {
                Log_History.WriteLine("[SetupCommu] {2},{0},{1}", data, control, description);
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
                    Log_History.WriteLine("[SetupCommu] {2},{0},{1}", data, value, description);
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
                    Log_History.WriteLine("[SetupCommu] {2},{0},{1}", data, value, description);
                    return value;
                }
            }

            return data;
        }

        private bool CheckModify_Boolean(bool control, bool data, string description)
        {
            if (control != data)
            {
                Log_History.WriteLine("[SetupCommu] {2},{0},{1}", data, control, description);
                return control;
            }

            return data;
        }

        private void cbOnline_Click(object sender, RoutedEventArgs e)
        {
            cbOnline.IsChecked = true;
            cbOffline.IsChecked = false;

            dataService.DataSystem.OnlineState = 3;     // Online Remote, 2:Online Local
        }

        private void cbOffline_Click(object sender, RoutedEventArgs e)
        {
            cbOnline.IsChecked = false;
            cbOffline.IsChecked = true;

            dataService.DataSystem.OnlineState = 1;     // Offline
        }

        private void btnMESReset_Click(object sender, RoutedEventArgs e)
        {
            MesService mesService = MesService.Singleton;

            mesService.Mes.Close();
            System.Windows.Forms.Application.DoEvents();
            mesService.Mes.SetParams();
            System.Windows.Forms.Application.DoEvents();
            mesService.Mes.Open();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (null == dataService)
                return;

            tbxTopVisionIPAddress.Text = dataService.DataSystem.TopIPAddress;
            tbxTopVisionPort.Text = dataService.DataSystem.TopPort.ToString();
            tbxTopVisionHandlerPort.Text = dataService.DataSystem.TopHPort.ToString();

            tbxTop2VisionIPAddress.Text = dataService.DataSystem.Top2IPAddress;
            tbxTop2VisionPort.Text = dataService.DataSystem.Top2Port.ToString();
            tbxTop2VisionHandlerPort.Text = dataService.DataSystem.Top2HPort.ToString();

            tbxBottomVisionIPAddress.Text = dataService.DataSystem.BottomIPAddress;
            tbxBottomVisionPort.Text = dataService.DataSystem.BottomPort.ToString();
            tbxBottomVisionHandlerPort.Text = dataService.DataSystem.BottomHPort.ToString();

            tbxBottom2VisionIPAddress.Text = dataService.DataSystem.Bottom2IPAddress;
            tbxBottom2VisionPort.Text = dataService.DataSystem.Bottom2Port.ToString();
            tbxBottom2VisionHandlerPort.Text = dataService.DataSystem.Bottom2HPort.ToString();

            tbxMonoVisionIPAddress.Text = dataService.DataSystem.MonoIPAddress;
            tbxMonoVisionPort.Text = dataService.DataSystem.MonoPort.ToString();
            tbxMonoVisionHandlerPort.Text = dataService.DataSystem.MonoHPort.ToString();

            tbxMono2VisionIPAddress.Text = dataService.DataSystem.Mono2IPAddress;
            tbxMono2VisionPort.Text = dataService.DataSystem.Mono2Port.ToString();
            tbxMono2VisionHandlerPort.Text = dataService.DataSystem.Mono2HPort.ToString();

            tbxServerIPAddress.Text = dataService.DataSystem.ServerPCIPAddress;
            tbxServerPort.Text = dataService.DataSystem.ServerPCPort.ToString();
            tbxServerHandlerPort.Text = dataService.DataSystem.ServerPCHPort.ToString();

            tbxTopVisionNetworkDrive.Text = dataService.DataSystem.NTDriveTop1;
            tbxTop2VisionNetworkDrive.Text = dataService.DataSystem.NTDriveTop2;
            tbxBottomVisionNetworkDrive.Text = dataService.DataSystem.NTDriveBottom1;
            tbxBottom2VisionNetworkDrive.Text = dataService.DataSystem.NTDriveBottom2;
            tbxMonoVisionNetworkDrive.Text = dataService.DataSystem.NTDriveMono1;
            tbxMono2VisionNetworkDrive.Text = dataService.DataSystem.NTDriveMono2;
            tbxServerNetworkDrive.Text = dataService.DataSystem.NTDriveServer;

            tbxTopVisionPublicFolder.Text = dataService.DataSystem.PublicFolderTop1;
            tbxTop2VisionPublicFolder.Text = dataService.DataSystem.PublicFolderTop2;
            tbxBottomVisionPublicFolder.Text = dataService.DataSystem.PublicFolderBottom1;
            tbxBottom2VisionPublicFolder.Text = dataService.DataSystem.PublicFolderBottom2;
            tbxMonoVisionPublicFolder.Text = dataService.DataSystem.PublicFolderMono1;
            tbxMono2VisionPublicFolder.Text = dataService.DataSystem.PublicFolderMono2;
            tbxServerPublicFolder.Text = dataService.DataSystem.PublicFolderServer;

            tbxService.Text = dataService.DataSystem.Service;
            tbxNetwork.Text = dataService.DataSystem.Network;
            tbxDeamon.Text = dataService.DataSystem.Daemon;


            tbxPubSubject.Text = dataService.DataSystem.PubSubject;
            tbxPubTimeout.Text = dataService.DataSystem.PubTimeout.ToString();
            tbxSubSubject.Text = dataService.DataSystem.SubSubject;

            // 1: Offline, 2:Local, 3:Remote
            if (1 < dataService.DataSystem.OnlineState)
            {
                cbOnline.IsChecked = true;
                cbOffline.IsChecked = false;
            }
            else
            {
                cbOnline.IsChecked = false;
                cbOffline.IsChecked = true;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (null == dataService)
                return;

            dataService.DataSystem.TopIPAddress = tbxTopVisionIPAddress.Text;
            dataService.DataSystem.TopPort = int.Parse(tbxTopVisionPort.Text);
            dataService.DataSystem.TopHPort = int.Parse(tbxTopVisionHandlerPort.Text);

            dataService.DataSystem.Top2IPAddress = tbxTop2VisionIPAddress.Text;
            dataService.DataSystem.Top2Port = int.Parse(tbxTop2VisionPort.Text);
            dataService.DataSystem.Top2HPort = int.Parse(tbxTop2VisionHandlerPort.Text);

            dataService.DataSystem.BottomIPAddress = tbxBottomVisionIPAddress.Text;
            dataService.DataSystem.BottomPort = int.Parse(tbxBottomVisionPort.Text);
            dataService.DataSystem.BottomHPort = int.Parse(tbxBottomVisionHandlerPort.Text);

            dataService.DataSystem.Bottom2IPAddress = tbxBottom2VisionIPAddress.Text;
            dataService.DataSystem.Bottom2Port = int.Parse(tbxBottom2VisionPort.Text);
            dataService.DataSystem.Bottom2HPort = int.Parse(tbxBottom2VisionHandlerPort.Text);

            dataService.DataSystem.MonoIPAddress = tbxMonoVisionIPAddress.Text;
            dataService.DataSystem.MonoPort = int.Parse(tbxMonoVisionPort.Text);
            dataService.DataSystem.MonoHPort = int.Parse(tbxMonoVisionHandlerPort.Text);

            dataService.DataSystem.Mono2IPAddress = tbxMono2VisionIPAddress.Text;
            dataService.DataSystem.Mono2Port = int.Parse(tbxMono2VisionPort.Text);
            dataService.DataSystem.Mono2HPort = int.Parse(tbxMono2VisionHandlerPort.Text);

            dataService.DataSystem.ServerPCIPAddress = tbxServerIPAddress.Text;
            dataService.DataSystem.ServerPCPort = int.Parse(tbxServerPort.Text);
            dataService.DataSystem.ServerPCHPort = int.Parse(tbxServerHandlerPort.Text);


            dataService.DataSystem.NTDriveTop1 = tbxTopVisionNetworkDrive.Text;
            dataService.DataSystem.NTDriveTop2 = tbxTop2VisionNetworkDrive.Text;
            dataService.DataSystem.NTDriveBottom1 = tbxBottomVisionNetworkDrive.Text;
            dataService.DataSystem.NTDriveBottom2 = tbxBottom2VisionNetworkDrive.Text;
            dataService.DataSystem.NTDriveMono1 = tbxMonoVisionNetworkDrive.Text;
            dataService.DataSystem.NTDriveMono2 = tbxMono2VisionNetworkDrive.Text;
            dataService.DataSystem.NTDriveServer = tbxServerNetworkDrive.Text;

            dataService.DataSystem.PublicFolderTop1 = tbxTopVisionPublicFolder.Text;
            dataService.DataSystem.PublicFolderTop2 = tbxTop2VisionPublicFolder.Text;
            dataService.DataSystem.PublicFolderBottom1 = tbxBottomVisionPublicFolder.Text;
            dataService.DataSystem.PublicFolderBottom2 = tbxBottom2VisionPublicFolder.Text;
            dataService.DataSystem.PublicFolderMono1 = tbxMonoVisionPublicFolder.Text;
            dataService.DataSystem.PublicFolderMono2 = tbxMono2VisionPublicFolder.Text;
            dataService.DataSystem.PublicFolderServer = tbxServerPublicFolder.Text;

            dataService.DataSystem.Service = tbxService.Text;
            dataService.DataSystem.Network = tbxNetwork.Text;
            dataService.DataSystem.Daemon = tbxDeamon.Text;


            dataService.DataSystem.PubSubject = tbxPubSubject.Text;
            dataService.DataSystem.PubTimeout = int.Parse(tbxPubTimeout.Text);
            dataService.DataSystem.SubSubject = tbxSubSubject.Text;

            // 1: Offline, 2:Local, 3:Remote
            if (true == cbOnline.IsChecked)
            {
                dataService.DataSystem.OnlineState = 3;
            }
            else
            {
                dataService.DataSystem.OnlineState = 1;
            }
            
            dataService.DataSystem.Save();

            msgService.ShowMessage((int)EnumSmartIC.LightAlarms.savedone, false);
        }

    }
}
