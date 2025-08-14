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
    /// SetupVerifyPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SetupVerifyPage : Page
    {
        private DataService dataService = null;

        public SetupVerifyPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dataService = DataService.Singleton;

            ComboBox[] comboBoxs = new ComboBox[10] {cbCode0, cbCode1, cbCode2, cbCode3, cbCode4, cbCode5, cbCode6, cbCode7, cbCode8, cbCode9};

            comboBoxs[0].Items.Add("G");
            comboBoxs[0].SelectedIndex = 0;

            for (int i = 0; i < dataService.DataResult.CountNGNames.Length; ++i)
            {
                string item = string.Format("{0} [{1}]", dataService.DataResult.CountNGIDs[i], dataService.DataResult.CountNGNames[i]);
                
                for( int k=1; k<10; ++k )
                    comboBoxs[k].Items.Add(item);
            }

            for( int k=1; k<10; ++k )
            {
                for (int i = 0; i < dataService.DataResult.CountNGNames.Length; ++i)
                {
                    if (dataService.DataSystem.verifyCodes[k] == dataService.DataResult.CountNGIDs[i])
                    {
                        comboBoxs[k].SelectedIndex = i;
                        break;
                    }
                }
            }

            rdVerifyCodeUse.IsChecked = dataService.DataSystem.IsUseVerifyCodes;
            rdVerifyCodeDisuse.IsChecked = !dataService.DataSystem.IsUseVerifyCodes;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ComboBox[] comboBoxs = new ComboBox[10] { cbCode0, cbCode1, cbCode2, cbCode3, cbCode4, cbCode5, cbCode6, cbCode7, cbCode8, cbCode9 };

            for (int k = 1; k < 10; ++k)
            {
                for (int i = 0; i < dataService.DataResult.CountNGNames.Length; ++i)
                {
                    if (dataService.DataSystem.verifyCodes[k] == dataService.DataResult.CountNGIDs[i])
                    {
                        comboBoxs[k].SelectedIndex = i;
                        break;
                    }
                }
            }

            rdVerifyCodeUse.IsChecked = dataService.DataSystem.IsUseVerifyCodes;
            rdVerifyCodeDisuse.IsChecked = !dataService.DataSystem.IsUseVerifyCodes;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ComboBox[] comboBoxs = new ComboBox[10] { cbCode0, cbCode1, cbCode2, cbCode3, cbCode4, cbCode5, cbCode6, cbCode7, cbCode8, cbCode9 };

            for (int i = 1; i < 10; ++i)
            {
                int index = comboBoxs[i].SelectedIndex;

                dataService.DataSystem.verifyCodes[i] = dataService.DataResult.CountNGIDs[index];

            }

            dataService.DataSystem.verifyCodes[0] = "G";

            dataService.DataSystem.IsUseVerifyCodes = rdVerifyCodeUse.IsChecked.Value;

            MsgService.Singleton.ShowMessage((int)EnumSmartIC.LightAlarms.savedone, false);
        }

        private void cbCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
