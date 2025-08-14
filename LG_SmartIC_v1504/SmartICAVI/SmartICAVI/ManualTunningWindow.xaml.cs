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
    /// ManualTunningWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ManualTunningWindow : Window
    {
        public int Axis { get; set; }

        private DataService dataService = null;

        public ManualTunningWindow()
        {
            InitializeComponent();

            Axis = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataService = DataService.Singleton;

            tbKp0.Text = dataService.DataMotion[Axis].Kp0.ToString("0.000");
            tbKi0.Text = dataService.DataMotion[Axis].Ki0.ToString("0.000");
            tbKd0.Text = dataService.DataMotion[Axis].Kd0.ToString("0.000");

            tbVmin0.Text = dataService.DataMotion[Axis].Vmin0.ToString("0.000");
            tbVmax0.Text = dataService.DataMotion[Axis].Vmax0.ToString("0.000");
            tbInpos0.Text = dataService.DataMotion[Axis].Inpos0.ToString("0.000");

            tbVoffset.Text = dataService.DataMotion[Axis].Voffset.ToString("0.000000000000");
            tbVjog.Text = dataService.DataMotion[Axis].Vjog.ToString("0.000");

            tbKp.Text = dataService.DataMotion[Axis].Kp.ToString("0.000");
            tbKi.Text = dataService.DataMotion[Axis].Ki.ToString("0.000");
            tbKd.Text = dataService.DataMotion[Axis].Kd.ToString("0.000");

            tbVmin.Text = dataService.DataMotion[Axis].Vmin.ToString("0.000");
            tbVmax.Text = dataService.DataMotion[Axis].Vmax.ToString("0.000");
            tbInpos.Text = dataService.DataMotion[Axis].Inpos.ToString("0.000");
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Topmost = false;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;

            this.Close();
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dataService.DataMotion[Axis].Kp0 = double.Parse(tbKp0.Text);
                dataService.DataMotion[Axis].Ki0 = double.Parse(tbKi0.Text);
                dataService.DataMotion[Axis].Kd0 = double.Parse(tbKd0.Text);

                dataService.DataMotion[Axis].Vmin0 = double.Parse(tbVmin0.Text);
                dataService.DataMotion[Axis].Vmax0 = double.Parse(tbVmax0.Text);
                dataService.DataMotion[Axis].Inpos0 = double.Parse(tbInpos0.Text);

                dataService.DataMotion[Axis].Voffset = double.Parse(tbVoffset.Text);
                dataService.DataMotion[Axis].Vjog = double.Parse(tbVjog.Text);

                dataService.DataMotion[Axis].Kp = double.Parse(tbKp.Text);
                dataService.DataMotion[Axis].Ki = double.Parse(tbKi.Text);
                dataService.DataMotion[Axis].Kd = double.Parse(tbKd.Text);

                dataService.DataMotion[Axis].Vmin = double.Parse(tbVmin.Text);
                dataService.DataMotion[Axis].Vmax = double.Parse(tbVmax.Text);
                dataService.DataMotion[Axis].Inpos = double.Parse(tbInpos.Text);

                dataService.DataMotion.Save();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
    }
}
