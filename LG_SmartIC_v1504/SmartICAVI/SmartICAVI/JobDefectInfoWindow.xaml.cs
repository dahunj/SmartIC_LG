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
    /// JobDefectInfoWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class JobDefectInfoWindow : Window
    {
        private CheckBox[] cbxCheckMessages;
        private Label[] lblDefectCodes;
        private Label[] lblDefectDefines;
        private TextBox[] tbxDefectNames;
        private TextBox[] tbxDefectAccumulates;

        private DataService dataService = null;


        public JobDefectInfoWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataService = DataService.Singleton;

            cbxCheckMessages = new CheckBox[63]{
                cbxMessage_00, cbxMessage_01, cbxMessage_02, cbxMessage_03, cbxMessage_04, cbxMessage_05, cbxMessage_06, cbxMessage_07, cbxMessage_08, cbxMessage_09,
                cbxMessage_10, cbxMessage_11, cbxMessage_12, cbxMessage_13, cbxMessage_14, cbxMessage_15, cbxMessage_16, cbxMessage_17, cbxMessage_18, cbxMessage_19,
                cbxMessage_20, cbxMessage_21, cbxMessage_22, cbxMessage_23, cbxMessage_24, cbxMessage_25, cbxMessage_26, cbxMessage_27, cbxMessage_28, cbxMessage_29,
                cbxMessage_30, cbxMessage_31, cbxMessage_32, cbxMessage_33, cbxMessage_34, cbxMessage_35, cbxMessage_36, cbxMessage_37, cbxMessage_38, cbxMessage_39,
                cbxMessage_40, cbxMessage_41, cbxMessage_42, cbxMessage_43, cbxMessage_44, cbxMessage_45, cbxMessage_46, cbxMessage_47, cbxMessage_48, cbxMessage_49,
                cbxMessage_50, cbxMessage_51, cbxMessage_52, cbxMessage_53, cbxMessage_54, cbxMessage_55, cbxMessage_56, cbxMessage_57, cbxMessage_58, cbxMessage_59,
                cbxMessage_60, cbxMessage_61, cbxMessage_62, 
            };

            lblDefectCodes = new Label[63]{
                lblDefectCode_00, lblDefectCode_01, lblDefectCode_02, lblDefectCode_03, lblDefectCode_04, lblDefectCode_05, lblDefectCode_06, lblDefectCode_07, lblDefectCode_08, lblDefectCode_09,
                lblDefectCode_10, lblDefectCode_11, lblDefectCode_12, lblDefectCode_13, lblDefectCode_14, lblDefectCode_15, lblDefectCode_16, lblDefectCode_17, lblDefectCode_18, lblDefectCode_19,
                lblDefectCode_20, lblDefectCode_21, lblDefectCode_22, lblDefectCode_23, lblDefectCode_24, lblDefectCode_25, lblDefectCode_26, lblDefectCode_27, lblDefectCode_28, lblDefectCode_29,
                lblDefectCode_30, lblDefectCode_31, lblDefectCode_32, lblDefectCode_33, lblDefectCode_34, lblDefectCode_35, lblDefectCode_36, lblDefectCode_37, lblDefectCode_38, lblDefectCode_39,
                lblDefectCode_40, lblDefectCode_41, lblDefectCode_42, lblDefectCode_43, lblDefectCode_44, lblDefectCode_45, lblDefectCode_46, lblDefectCode_47, lblDefectCode_48, lblDefectCode_49,
                lblDefectCode_50, lblDefectCode_51, lblDefectCode_52, lblDefectCode_53, lblDefectCode_54, lblDefectCode_55, lblDefectCode_56, lblDefectCode_57, lblDefectCode_58, lblDefectCode_59,
                lblDefectCode_60, lblDefectCode_61, lblDefectCode_62, 
            };

            lblDefectDefines = new Label[63]{
                lblDefectDefine_00, lblDefectDefine_01, lblDefectDefine_02, lblDefectDefine_03, lblDefectDefine_04, lblDefectDefine_05, lblDefectDefine_06, lblDefectDefine_07, lblDefectDefine_08, lblDefectDefine_09,
                lblDefectDefine_10, lblDefectDefine_11, lblDefectDefine_12, lblDefectDefine_13, lblDefectDefine_14, lblDefectDefine_15, lblDefectDefine_16, lblDefectDefine_17, lblDefectDefine_18, lblDefectDefine_19,
                lblDefectDefine_20, lblDefectDefine_21, lblDefectDefine_22, lblDefectDefine_23, lblDefectDefine_24, lblDefectDefine_25, lblDefectDefine_26, lblDefectDefine_27, lblDefectDefine_28, lblDefectDefine_29,
                lblDefectDefine_30, lblDefectDefine_31, lblDefectDefine_32, lblDefectDefine_33, lblDefectDefine_34, lblDefectDefine_35, lblDefectDefine_36, lblDefectDefine_37, lblDefectDefine_38, lblDefectDefine_39,
                lblDefectDefine_40, lblDefectDefine_41, lblDefectDefine_42, lblDefectDefine_43, lblDefectDefine_44, lblDefectDefine_45, lblDefectDefine_46, lblDefectDefine_47, lblDefectDefine_48, lblDefectDefine_49,
                lblDefectDefine_50, lblDefectDefine_51, lblDefectDefine_52, lblDefectDefine_53, lblDefectDefine_54, lblDefectDefine_55, lblDefectDefine_56, lblDefectDefine_57, lblDefectDefine_58, lblDefectDefine_59,
                lblDefectDefine_60, lblDefectDefine_61, lblDefectDefine_62, 
            };

            tbxDefectNames = new TextBox[63]{
                tbxDefectName_00, tbxDefectName_01, tbxDefectName_02, tbxDefectName_03, tbxDefectName_04, tbxDefectName_05, tbxDefectName_06, tbxDefectName_07, tbxDefectName_08, tbxDefectName_09,
                tbxDefectName_10, tbxDefectName_11, tbxDefectName_12, tbxDefectName_13, tbxDefectName_14, tbxDefectName_15, tbxDefectName_16, tbxDefectName_17, tbxDefectName_18, tbxDefectName_19,
                tbxDefectName_20, tbxDefectName_21, tbxDefectName_22, tbxDefectName_23, tbxDefectName_24, tbxDefectName_25, tbxDefectName_26, tbxDefectName_27, tbxDefectName_28, tbxDefectName_29,
                tbxDefectName_30, tbxDefectName_31, tbxDefectName_32, tbxDefectName_33, tbxDefectName_34, tbxDefectName_35, tbxDefectName_36, tbxDefectName_37, tbxDefectName_38, tbxDefectName_39,
                tbxDefectName_40, tbxDefectName_41, tbxDefectName_42, tbxDefectName_43, tbxDefectName_44, tbxDefectName_45, tbxDefectName_46, tbxDefectName_47, tbxDefectName_48, tbxDefectName_49,
                tbxDefectName_50, tbxDefectName_51, tbxDefectName_52, tbxDefectName_53, tbxDefectName_54, tbxDefectName_55, tbxDefectName_56, tbxDefectName_57, tbxDefectName_58, tbxDefectName_59,
                tbxDefectName_60, tbxDefectName_61, tbxDefectName_62, 
            };

            tbxDefectAccumulates = new TextBox[63]
            {
                tbxDefectAccumulate_00, tbxDefectAccumulate_01, tbxDefectAccumulate_02, tbxDefectAccumulate_03, tbxDefectAccumulate_04,
                tbxDefectAccumulate_05, tbxDefectAccumulate_06, tbxDefectAccumulate_07, tbxDefectAccumulate_08, tbxDefectAccumulate_09,
                tbxDefectAccumulate_10, tbxDefectAccumulate_11, tbxDefectAccumulate_12, tbxDefectAccumulate_13, tbxDefectAccumulate_14,
                tbxDefectAccumulate_15, tbxDefectAccumulate_16, tbxDefectAccumulate_17, tbxDefectAccumulate_18, tbxDefectAccumulate_19,
                tbxDefectAccumulate_20, tbxDefectAccumulate_21, tbxDefectAccumulate_22, tbxDefectAccumulate_23, tbxDefectAccumulate_24,
                tbxDefectAccumulate_25, tbxDefectAccumulate_26, tbxDefectAccumulate_27, tbxDefectAccumulate_28, tbxDefectAccumulate_29,
                tbxDefectAccumulate_30, tbxDefectAccumulate_31, tbxDefectAccumulate_32, tbxDefectAccumulate_33, tbxDefectAccumulate_34,
                tbxDefectAccumulate_35, tbxDefectAccumulate_36, tbxDefectAccumulate_37, tbxDefectAccumulate_38, tbxDefectAccumulate_39,
                tbxDefectAccumulate_40, tbxDefectAccumulate_41, tbxDefectAccumulate_42, tbxDefectAccumulate_43, tbxDefectAccumulate_44,
                tbxDefectAccumulate_45, tbxDefectAccumulate_46, tbxDefectAccumulate_47, tbxDefectAccumulate_48, tbxDefectAccumulate_49,
                tbxDefectAccumulate_50, tbxDefectAccumulate_51, tbxDefectAccumulate_52, tbxDefectAccumulate_53, tbxDefectAccumulate_54,
                tbxDefectAccumulate_55, tbxDefectAccumulate_56, tbxDefectAccumulate_57, tbxDefectAccumulate_58, tbxDefectAccumulate_59,
                tbxDefectAccumulate_60, tbxDefectAccumulate_61, tbxDefectAccumulate_62,
            };

            for (int i = 0; i < tbxDefectNames.Length; ++i)
            {
                cbxCheckMessages[i].IsChecked = dataService.DataDefectInfo.IsShowMessages[i];
                lblDefectCodes[i].Content = dataService.DataDefectInfo.IDs[i];
                lblDefectDefines[i].Content = dataService.DataDefectInfo.Defines[i];
                tbxDefectNames[i].Text = dataService.DataDefectInfo.Names[i];
                tbxDefectAccumulates[i].Text = dataService.DataDefectInfo.Accumulates[i].ToString();

                // Joint
                if ("BB006" == dataService.DataDefectInfo.IDs[i])
                {
                    cbxCheckMessages[i].IsChecked = true;
                    cbxCheckMessages[i].IsEnabled = false;
                }

                // 쓰루홀 BB039

            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                for (int i = 0; i < tbxDefectNames.Length; ++i)
                {
                    dataService.DataDefectInfo.IsShowMessages[i] = (bool)cbxCheckMessages[i].IsChecked;
                    dataService.DataDefectInfo.Names[i] = tbxDefectNames[i].Text;
                    dataService.DataDefectInfo.Accumulates[i] = int.Parse(tbxDefectAccumulates[i].Text);
                }
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine("JobDefectInfoWindow.btnOK_Click() : " + exc.Message);
            }
            finally
            {
                this.Close();
            }
        }
    }
}
