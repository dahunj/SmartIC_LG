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

namespace SmartICAVI
{
    /// <summary>
    /// SetupUserAccountPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SetupUserAccountPage : Page
    {
        private DataService dataService = null;
        private MsgService msgService = null;

        public SetupUserAccountPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dataService = DataService.Singleton;
            msgService = MsgService.Singleton;

            cbAccountType.Items.Add("오퍼레이터");
            cbAccountType.Items.Add("관리자");
            cbAccountType.Items.Add("유지보수");

            cbAccountType.SelectedIndex = 0;

            btnAdd.IsEnabled = false;
            tbxPassword1.IsEnabled = false;
            tbxPassword2.IsEnabled = false;

            btnDelete.IsEnabled = false;

            btnAll.IsChecked = true;
            btnOperator.IsChecked = false;
            btnAdmin.IsChecked = false;
            btnMaint.IsChecked = false;


            btnUserAccount_Click((object)btnAll, null);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            lvUserAccount.Items.Clear();
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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (null != lvUserAccount.SelectedItem)
            {
                DataUserAccount data = (DataUserAccount)lvUserAccount.SelectedItem;
                string deleteID = data.ID;

                if ("" != deleteID)
                {
                    dataService.DataUserAccounts.DeleteAccount(deleteID);
                }

                tblSelectedID.Text = "";

                DisplayUserAccount();

                btnDelete.IsEnabled = false;

                dataService.DataUserAccounts.Save();
            }
        }

        private void btnIDCheck_Click(object sender, RoutedEventArgs e)
        {
            if ("" != tbxUserID.Text)
            {
                if (false == dataService.DataUserAccounts.IsExistID(tbxUserID.Text))
                {
                    btnAdd.IsEnabled = true;
                    tbxPassword1.IsEnabled = true;
                    tbxPassword2.IsEnabled = true;
                }
                else
                {
                    btnAdd.IsEnabled = false;
                    tbxPassword1.IsEnabled = false;
                    tbxPassword2.IsEnabled = false;

                    msgService.ShowMessage((int)EnumSmartIC.LightAlarms.sameUserID, false);
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if ("" == tbxUserID.Text)
            {
                msgService.ShowMessage((int)EnumSmartIC.LightAlarms.checkPassword, false);
                return;
            }

            if (true == dataService.DataUserAccounts.IsExistID(tbxUserID.Text))
            {
                msgService.ShowMessage((int)EnumSmartIC.LightAlarms.sameUserID, false);
                return;
            }

            if (tbxPassword2.Password != tbxPassword1.Password)
            {
                msgService.ShowMessage((int)EnumSmartIC.LightAlarms.checkPassword, false);
                return;
            }

            dataService.DataUserAccounts.AddAccount(cbAccountType.SelectedIndex, tbxUserID.Text, tbxPassword1.Password);
            DisplayUserAccount();

            btnAdd.IsEnabled = false;
            tbxPassword1.IsEnabled = false;
            tbxPassword2.IsEnabled = false;

            tbxPassword1.Password = "";
            tbxPassword2.Password = "";

            tbxUserID.Text = "";

            dataService.DataUserAccounts.Save();
        }

        private void lvUserAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null != lvUserAccount.SelectedItem)
            {
                DataUserAccount data = (DataUserAccount)lvUserAccount.SelectedItem;
                tblSelectedID.Text = data.ID;

                btnDelete.IsEnabled = true;
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
            }
            else
            {
                type = 2;
            }

            for (int i = 0; i < count; ++i)
            {
                DataUserAccount data = dataService.DataUserAccounts.ListUA[i];
                
                if( type == data.Type )
                    lvUserAccount.Items.Add(data);
            }
        }

    }
}
