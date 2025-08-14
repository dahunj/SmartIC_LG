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
    /// JobLogonWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class JobLogonWindow : Window
    {
        private DataService dataService = null;
        private MsgService msgService = null;

        public JobLogonWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataService = DataService.Singleton;
            msgService = MsgService.Singleton;

            btnAll.IsChecked = true;
            btnOperator.IsChecked = false;
            btnAdmin.IsChecked = false;
            btnMaint.IsChecked = false;

            tbxPassword.IsEnabled = false;
            btnOK.IsEnabled = false;

            btnOK.IsDefault = true;


            btnUserAccount_Click((object)btnAll, null);

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            lvUserAccount.Items.Clear();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
            
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {

            string userID = tbSelectedlD.Text;
            string password = tbxPassword.Password;

            if ("SYNAPSE" == userID.ToUpper())
            {
                if ("2033" == password)
                {
                    Log_Trace.WriteLine("LogOn : " + userID);

                    dataService.LogonID = userID;
                    dataService.LogonType = 2;
                    dataService.LogonTime = DateTime.Now;

                    this.DialogResult = true;
                    this.Close();

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

                    this.DialogResult = true;
                    this.Close();

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

                        this.DialogResult = true;
                        this.Close();

                        return;
                    }
                }
            }

            msgService.ShowMessage((int)EnumSmartIC.LightAlarms.password, false);

           
        }

        private void btnUserAccount_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;

            if (null != sender)
            {
                tbxPassword.Password = "";
                tbSelectedlD.Text = "";

                tbxPassword.IsEnabled = false;
                btnOK.IsEnabled = false;

                btnAll.IsChecked = false;
                btnOperator.IsChecked = false;
                btnAdmin.IsChecked = false;
                btnMaint.IsChecked = false;

                cbx.IsChecked = true;

                DisplayUserAccount();
            }
        }

        private void lvUserAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null != lvUserAccount.SelectedItem)
            {
                DataUserAccount data = (DataUserAccount)lvUserAccount.SelectedItem;
                tbSelectedlD.Text = data.ID;

                tbxPassword.IsEnabled = true;

                tbxPassword.Password = "";
                tbxPassword.Focus();

                btnOK.IsEnabled = true;
            }
        }

        private void DisplayUserAccount()
        {
            lvUserAccount.Items.Clear();
            GC.Collect();

            int count = dataService.DataUserAccounts.ListUA.Count;
            int type = 0;

            if (true == btnAll.IsChecked)
            {
                DataUserAccount dataMaint = new DataUserAccount();
                dataMaint.Type = 2;
                dataMaint.ID = "SYNAPSE";
                lvUserAccount.Items.Add(dataMaint);

                DataUserAccount dataAdmin = new DataUserAccount();
                dataAdmin.Type = 1;
                dataAdmin.ID = "ADMIN";
                lvUserAccount.Items.Add(dataAdmin);

                for (int i = 0; i < count; ++i)
                {
                    DataUserAccount data = dataService.DataUserAccounts.ListUA[i];
                    lvUserAccount.Items.Add(data);
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

                DataUserAccount dataAdmin = new DataUserAccount();
                dataAdmin.Type = 1;
                dataAdmin.ID = "ADMIN";
                lvUserAccount.Items.Add(dataAdmin);
            }
            else
            {
                type = 2;

                DataUserAccount dataMaint = new DataUserAccount();
                dataMaint.Type = 2;
                dataMaint.ID = "SYNAPSE";
                lvUserAccount.Items.Add(dataMaint);
            }

            for (int i = 0; i < count; ++i)
            {
                DataUserAccount data = dataService.DataUserAccounts.ListUA[i];

                if (type == data.Type)
                    lvUserAccount.Items.Add(data);
            }
        }
    }
}
