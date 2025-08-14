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
using System.Reflection;

namespace SmartICAVI
{
    /// <summary>
    /// MonitorSystemPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MonitorSystemPage : Page
    {
        public MonitorSystemPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Version
            string copyright = "";
            string company = "";
            string product = "";
            string fileversion = "";
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            // Copyright

            Type attType = typeof(AssemblyCopyrightAttribute);
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(attType, false);

            if (attributes.Length != 0)
            {
                AssemblyCopyrightAttribute att = (AssemblyCopyrightAttribute)attributes[0];
                copyright = att.Copyright;
            }
            else
            {
                copyright = string.Empty;
            }

            // Company
            attType = typeof(AssemblyCompanyAttribute);
            attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(attType, false);
            if (attributes.Length != 0)
            {
                AssemblyCompanyAttribute att = (AssemblyCompanyAttribute)attributes[0];
                company = att.Company;
            }
            else
            {
                company = string.Empty;
            }

            // Product
            attType = typeof(AssemblyProductAttribute);
            attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(attType, false);
            if (attributes.Length != 0)
            {
                AssemblyProductAttribute att = (AssemblyProductAttribute)attributes[0];
                product = att.Product;
            }
            else
            {
                product = string.Empty;
            }

            // File Version 
            attType = typeof(AssemblyFileVersionAttribute);
            attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(attType, false);
            if (attributes.Length != 0)
            {
                AssemblyFileVersionAttribute att = (AssemblyFileVersionAttribute)attributes[0];
                fileversion = att.Version;
            }
            else
            {
                product = string.Empty;
            }

            lblVersion.Content = version;
            lblFileVersion.Content = fileversion;
            lblCopyright.Content = copyright;

            //lblStatus.Content = sysService.State.ToString();
            //lblMode.Content = sysService.Mode.ToString();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
