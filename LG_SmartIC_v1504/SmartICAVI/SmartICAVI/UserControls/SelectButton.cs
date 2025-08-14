using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Controls;

using System.Threading;
using System.Windows.Threading;

namespace SmartICAVI.UserControls
{
    class SelectButton : Button, ISelectControl
    {
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

        public SelectButton()
        {
        }

        private void SetEnable(bool enable)
        {
            Dispatcher.BeginInvoke(new Action(() => { IsEnabled = enable; }), DispatcherPriority.ContextIdle, null);
        }
    }
}
