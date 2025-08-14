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

namespace SmartICAVI
{
    /// <summary>
    /// SystemLockPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SystemLockPage : Page
    {
        private DataService dataService = null;
        private MsgService msgService = null;

        static bool InvokeRequired
        {
            get { return Dispatcher.CurrentDispatcher != Application.Current.Dispatcher; }
        }

        public SystemLockPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Log_Trace.WriteLine("[SystemLockPage] Show");

            dataService = DataService.Singleton;
            msgService = MsgService.Singleton;

            string imgPath = DataService.Singleton.DataPath + "\\Images\\Common\\NoTouch.png";

            Uri uriMachine = new Uri(imgPath);
            BitmapImage bmpMachine = null;

            // Image Load
            try
            {
                bmpMachine = new BitmapImage(uriMachine);
            }
            catch (Exception exc)
            {
                uriMachine = new Uri("pack://application:,,/Images/Stop48.png");
                bmpMachine = new BitmapImage(uriMachine);

                Log_Trace.WriteLine("[SystemLockPage] " + exc.Message);
            }

            image.Source = bmpMachine;


            // Warning Image
            imgPath = DataService.Singleton.DataPath + "\\Images\\Common\\Warning.png";

            uriMachine = new Uri(imgPath);
            bmpMachine = null;

            // Image Load
            try
            {
                bmpMachine = new BitmapImage(uriMachine);
            }
            catch (Exception exc)
            {
                uriMachine = new Uri("pack://application:,,/Images/Stop48.png");
                bmpMachine = new BitmapImage(uriMachine);

                Log_Trace.WriteLine("[SystemLockPage] " + exc.Message);
            }

            imgWarning.Source = bmpMachine;

            tbxPassword.Focus();
            btnOK.IsDefault = true;

            btnAll.IsChecked = true;
            btnOperator.IsChecked = false;
            btnAdmin.IsChecked = false;
            btnMaint.IsChecked = false;


            btnUserAccount_Click((object)btnAll, null);

            SystemService.Singleton.State = SystemService.States.systemLock;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Log_Trace.WriteLine("[SystemLockPage] Hide");
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            //string password = tbxPassword.Password.ToUpper();

            //if ("1" == password)
            //{
            //    SystemService.Singleton.State = SystemService.States.systemRelease;
            //    DataService.Singleton.LogonTime = new DateTime(0);
            //}
            //else if("SYN2033" == password )
            //{
            //    DataService.Singleton.LogonTime = DateTime.Now;
            //    SystemService.Singleton.State = SystemService.States.systemRelease;
            //}
            //else if ("SYNAPSE2033" == password)
            //{
            //    DataService.Singleton.LogonTime = DateTime.Now;
            //    SystemService.Singleton.State = SystemService.States.systemRelease;
            //}
            //else
            //    MsgService.Singleton.ShowMessage((int)EnumSmartIC.LightAlarms.password);

            string userID = cbUserID.SelectedItem.ToString();
            string password = tbxPassword.Password;

            if ("SYNAPSE" == userID.ToUpper())
            {
                if ("2033" == password)
                {
                    Log_Trace.WriteLine("LogOn : " + userID);

                    dataService.LogonID = userID;
                    dataService.LogonType = 2;
                    dataService.LogonTime = DateTime.Now;

                    SystemService.Singleton.State = SystemService.States.systemRelease;

                    return;
                }
            }
            else if ("ADMIN" == userID.ToUpper())
            {
                if ("1234" == password)
                {
                    Log_Trace.WriteLine("LogOn : " + userID);

                    dataService.LogonID = userID;
                    dataService.LogonType = 1;
                    dataService.LogonTime = DateTime.Now;

                    SystemService.Singleton.State = SystemService.States.systemRelease;

                    return;
                }
            }
            else
            {
                if (true == dataService.DataUserAccounts.IsExistID(userID))
                {
                    int user = dataService.DataUserAccounts.GetAccountType(userID, password);
                    if (-1 != user)
                    {
                        Log_Trace.WriteLine("LogOn : " + userID);

                        dataService.LogonID = userID;
                        dataService.LogonType = user;
                        dataService.LogonTime = DateTime.Now;

                        SystemService.Singleton.State = SystemService.States.systemRelease;

                        return;
                    }
                }
            }

            msgService.ShowMessage((int)EnumSmartIC.LightAlarms.password, false);
        }

        private void tbxPassword_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            VirtualKeyboardService.GetSingleton().FireVirtualKeyboard(sender);
        }

        private void btnUserAccount_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;

            if (null != sender)
            {
                btnAll.IsChecked = false;
                btnOperator.IsChecked = false;
                btnAdmin.IsChecked = false;
                btnMaint.IsChecked = false;

                cbx.IsChecked = true;

                DisplayUserAccount();
            }
        }

        private void DisplayUserAccount()
        {
            cbUserID.Items.Clear();
            GC.Collect();

            int count = dataService.DataUserAccounts.ListUA.Count;
            int type = 0;

            if (true == btnAll.IsChecked)
            {
                cbUserID.Items.Add("SYNAPSE");

                cbUserID.Items.Add("ADMIN");

                for (int i = 0; i < count; ++i)
                {
                    DataUserAccount data = dataService.DataUserAccounts.ListUA[i];

                    cbUserID.Items.Add(data.ID);
                }


                if (0 < cbUserID.Items.Count)
                {
                    for (int i = 0; i < cbUserID.Items.Count; ++i)
                    {
                        if (cbUserID.Items[i].ToString() == dataService.LogonID)
                        {
                            cbUserID.SelectedValue = dataService.LogonID;
                            return;
                        }
                    }

                    cbUserID.SelectedIndex = 0;
                }

                return;
            }
            else if (true == btnOperator.IsChecked)
            {
                type = 0;
            }
            else if (true == btnAdmin.IsChecked)
            {
                type = 1;

                cbUserID.Items.Add("ADMIN");
            }
            else
            {
                type = 2;

                cbUserID.Items.Add("SYNAPSE");
            }

            for (int i = 0; i < count; ++i)
            {
                DataUserAccount data = dataService.DataUserAccounts.ListUA[i];

                if (type == data.Type)
                    cbUserID.Items.Add(data.ID);
            }

            if (0 < cbUserID.Items.Count)
            {
                for (int i = 0; i < cbUserID.Items.Count; ++i)
                {
                    if (cbUserID.Items[i].ToString() == dataService.LogonID)
                    {
                        cbUserID.SelectedValue = dataService.LogonID;
                        return;
                    }
                }

                cbUserID.SelectedIndex = 0;
            }

            
        }
    }
}
