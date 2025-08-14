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
    /// TeachRecipeNameWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TeachRecipeNameWindow : Window
    {
        public string RecipeName { get; set; }
        public TeachRecipeNameWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnOK.IsDefault = true;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            RecipeName = tbxRecipeName.Text;

            this.DialogResult = true;

            this.Close();
        }
    }
}
