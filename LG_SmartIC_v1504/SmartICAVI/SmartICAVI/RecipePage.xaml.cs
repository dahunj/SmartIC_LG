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
using System.Windows.Threading;
using System.Threading;

using SmartICAVI.UserControls;
using HalconDotNet;

namespace SmartICAVI
{
    /// <summary>
    /// RecipePage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RecipePage : Page
    {
        private DataService dataService = null;
        private MsgService msgService = null;
        private SystemService sysService = null;
        private VisionCamService camService = null;

        private string oldValue = "";

        public RecipePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            bdRecipe.Visibility = System.Windows.Visibility.Visible;

            sysService = SystemService.Singleton;
            dataService = DataService.Singleton;
            msgService = MsgService.Singleton;
            camService = VisionCamService.Singleton;

            SearchModel();

            Cancel();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            GC.Collect();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            SelectedToCurrent(listbox.SelectedItem.ToString());
        }

        private void listboxItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if( null != listbox.SelectedItem )
                SelectedToCurrent(listbox.SelectedItem.ToString());
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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Log_Trace.WriteLine("[RecipePage] Click Delete");
            Delete();
        }

        private void Cancel()
        {
            tbxLineNo.Text = dataService.DataRecipe.Line.ToString();
            tbxPunchPosX.Text = dataService.DataRecipe.PunchX.ToString("0.00");
            tbxPunchPosY.Text = dataService.DataRecipe.PunchY.ToString("0.00");
            tbxPunchCenterPosX.Text = dataService.DataRecipe.PunchCenterX.ToString("0.00");
            tbxPunchCenterPosY.Text = dataService.DataRecipe.PunchCenterY.ToString("0.00");
            tbxBufferPosZ.Text = dataService.DataRecipe.BufferZ.ToString("0.00");
            tbxPFHole.Text = dataService.DataRecipe.PF.ToString();
            tbxNGContinue.Text = dataService.DataRecipe.NGContinue.ToString();

            tbSelectedModel.Text = dataService.DataSystem.RecipeName;
        }

        private void Delete()
        {
            if (null != listbox.SelectedItem)
            {
                if (listbox.SelectedItem.ToString() == tbSelectedModel.Text)
                {
                    msgService.ShowMessage((int)EnumSmartIC.LightAlarms.currentModel);
                    return;
                }
                else
                {
                    string path = dataService.RecipePath + "\\" + listbox.SelectedItem.ToString() + ".rcp";
                    
                    if (true == File.Exists(path))
                    {
                        try
                        {
                            File.Delete(path);

                            SearchModel();

                            Cancel();
                        }
                        catch (Exception exc)
                        {
                            Log_Exception.WriteLine("RecipePage.Delete() : " + exc.Message);
                        }
                    }
                }
            }
        }

        private void Selected(string name)
        {
            if (null == dataService)
                return;

            if (0 == dataService.LoadRecipe(name))
            {
                Cancel();

                if( null != camService )
                {
                    camService.SendLoad(name);
                }
            }
        }

        private void SelectedToCurrent(string name)
        {
            if (null == dataService)
                return;

            if (null != name)
            {
                Selected(name);
            }
        }

        private void SearchModel()
        {
            if (null == dataService)
                return;

            string path = dataService.RecipePath;
            string filename;
            string name;

            string[] files = System.IO.Directory.GetFiles(path);

            listbox.Items.Clear();

            //foreach (string file in files)
            for (int i = 0; i < files.Length; ++i)
            {
                //System.IO.FileInfo info = new System.IO.FileInfo(file);
                System.IO.FileInfo info = new System.IO.FileInfo(files[i]);

                filename = info.Name;

                if (".rcp" == info.Extension)
                {
                    name = filename.Replace(".rcp", "");
                    listbox.Items.Add(name);
                }
            }

            listbox.Items.SortDescriptions.Clear();
            listbox.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription());
            listbox.Items.Refresh();
        }
    }
}
