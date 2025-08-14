using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

using SmartICAVI.Animation;

namespace SmartICAVI
{
    /// <summary>
    /// TeachPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TeachPage : Page
    {
        private DataService dataService = null;
        private SystemService sysService = null;
        private StrobeService strobeService = null;
        private VisionCamService camService = null;

        public TeachPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dataService = DataService.Singleton;
            dataService.DataTeach = (DataRecipe)dataService.DataRecipe.Clone();
            dataService.TeachName = dataService.DataSystem.RecipeName;

            camService = VisionCamService.Singleton;

            sysService = SystemService.Singleton;

            // 0번째 인덱스
            if (sysService.State == SystemService.States.pause && dataService.DataSystem.FirstIndexPause == true)
            {
                cbxInitPage.IsEnabled = false;
                cbxTopPage.IsEnabled = false;
                cbxBottomPage.IsEnabled = false;
                cbxMonoPage.IsEnabled = false;
                cbxPunchPage.IsEnabled = false;
                btnSave.IsEnabled = false;
                cbxPage_Click(cbxPunchPage, null);
            }
            else
            {
                sysService.Mode = SystemService.Modes.recipe;

                cbxPage_Click(cbxInitPage, null);
            }

            strobeService = StrobeService.Singleton;
            strobeService.Write((byte)dataService.DataRecipe.strobe1);


            dataService.LogonTime = DateTime.Now;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            dataService.LogonTime = DateTime.Now;

            if( null != strobeService )
                strobeService.Write(0);
        }

        private void cbxPage_Click(object sender, RoutedEventArgs e)
        {
            CheckBox btn = sender as CheckBox;

            Page targetPage = null;

            // 0번째
            if (sysService.State == SystemService.States.pause && dataService.DataSystem.FirstIndexPause == true)
            {
                targetPage = new TeachPunchPage();
            }
            else
            {
                cbxInitPage.IsChecked = false;
                cbxTopPage.IsChecked = false;
                cbxBottomPage.IsChecked = false;
                cbxMonoPage.IsChecked = false;
                cbxPunchPage.IsChecked = false;

                if (null != btn)
                    btn.IsChecked = true;

                switch (btn.Tag.ToString())
                {
                    case "cbxBottomPage":
                        targetPage = new TeachBottomPage();
                        btnSave.IsEnabled = true;
                        break;
                    case "cbxInitPage":
                        TeachInitPage page = new TeachInitPage();
                        targetPage = page;
                        page.RecipeName = tblRecipeName;
                        btnSave.IsEnabled = false;
                        break;
                    case "cbxMonoPage":
                        targetPage = new TeachMonoPage();
                        btnSave.IsEnabled = true;
                        break;
                    case "cbxPunchPage":
                        targetPage = new TeachPunchPage();
                        btnSave.IsEnabled = true;
                        break;
                    case "cbxTopPage":
                        targetPage = new TeachTopPage();
                        btnSave.IsEnabled = true;
                        break;
                    default:
                        return;
                }
            }
            
            this.teachPage.NavigateToExample(targetPage);

            GC.Collect();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            MsgService msgService = MsgService.Singleton;

            string path = dataService.RecipePath + "\\" + tblRecipeName.Text + ".rcp";
            int ret = dataService.DataTeach.Save(path);

            if (null != camService)
            {
                camService.SendSearchArea(dataService.DataTeach.SearchAreaSize);
                camService.SendSave(path);
            }

            DoEvents(10);
            if (0 == ret)
            {
                msgService.ShowMessage((int)EnumSmartIC.LightAlarms.savedone, false);
            }
            else
            {
                sysService.State = SystemService.States.stop;
                msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.saveFail);
            }

            dataService.DataRecipe.Load();            
        }

        protected void DoEvents(int mSec)
        {
            DateTime start = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, mSec);
            DateTime timeout = start.Add(duration);

            do
            {
                System.Windows.Forms.Application.DoEvents();
                Thread.Sleep(1);
            } while (timeout > DateTime.Now);
        }

    }
}
