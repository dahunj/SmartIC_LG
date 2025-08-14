using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;

namespace SmartICAVI.UserControls
{
    class ApplyButton : Button, ISelectControl
    {
        public TextBox TextBox { get; set; }
        public int Axis { get; set; }

        private MotionService motionService = null;

        bool selected = false;
        public bool IsControlSelected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;

                SetEnable(selected);
            }
        }

        public ApplyButton()
        {
            TextBox = null;
            Axis = -1;
        }

        protected override void OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if ("IsVisible" == e.Property.ToString())
            {
                if (true == (bool)e.NewValue)
                {
                    if (null == motionService)
                        motionService = MotionService.Singleton;
                }
                else
                {
                    ;
                }
            }

        }

        protected override void OnClick()
        {
            base.OnClick();

            if (null != TextBox)
            {
                if ((null != motionService) && (-1 < Axis))
                {
                    double pos = 0.001;
                    pos = motionService.GetCurrentPosition(Axis);
                    TextBox.Text = string.Format("{0:0.000}", pos);
                }
            }
        }

        private void SetEnable(bool enable)
        {
            Dispatcher.BeginInvoke(new Action(() => { IsEnabled = enable; }), DispatcherPriority.ContextIdle, null);
        }
    }
}
