using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Controls;

using System.Threading;
using System.Windows.Threading;

namespace SmartICAVI.UserControls
{
    class StopButton : CheckBox
    {
        private SystemService sysService = null;
        private bool statusEvent = false;

        public bool AllowChangeState { get; set; }

        public StopButton()
        {
            AllowChangeState = true;
        }

        protected override void OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if ("IsVisible" == e.Property.ToString())
            {
                if (true == (bool)e.NewValue)
                {
                    if (null == sysService)
                        sysService = SystemService.Singleton;

                    sysService.EventState += OnEventState;

                    if (null != sysService)
                        OnEventState((object)sysService, null);
                }
                else
                {
                    if (null != sysService)
                        sysService.EventState -= OnEventState;

                    sysService = null;
                }
            }
            else if ("IsChecked" == e.Property.ToString())
            {
                if (null == sysService)
                    sysService = SystemService.Singleton;

                if (true == statusEvent)
                {
                    statusEvent = false;
                    //    return;
                }

                if (true == AllowChangeState)
                {
                    if (true == (bool)e.NewValue)
                    {
                        if (SystemService.States.stop > sysService.State)
                            sysService.State = SystemService.States.stop;
                    }
                    else
                    {
                        if (SystemService.States.reset != sysService.State)
                            sysService.State = SystemService.States.reset;
                    }
                }
            }
        }

        private void OnEventState(object sender, EventArgs e)
        {
            //SystemService.States status = sender as SystemService.States;
            SystemService sys = SystemService.Singleton;

            if (true == IsVisible)
            {
                switch (sys.State)
                {
                    case SystemService.States.none:             // 0
                    case SystemService.States.ready:            // 1    
                    case SystemService.States.idle:             // 2
                    case SystemService.States.run:              // 3
                    case SystemService.States.pause:            // 4
                    case SystemService.States.homing:           // 5
                    case SystemService.States.homeDone:         // 6
                    case SystemService.States.jobDone:          // 7
                        SetChecked(false);
                        break;
                    case SystemService.States.reset:            // 8
                        break;
                    case SystemService.States.lightAlarm:       // 9
                        break;
                     case SystemService.States.stop:            // 10
                    case SystemService.States.heavyAlarm:       // 11
                    case SystemService.States.emg:              // 12
                        //if (false == IsChecked)
                        SetChecked(true);
                        break;
                    default:
                        break;
                } // switch (status)
            } // if (true == IsControlSelected)
        }

        private void SetChecked(bool enable)
        {
            statusEvent = true;
            Dispatcher.BeginInvoke(new Action(() => { IsChecked = enable; }), DispatcherPriority.ContextIdle, null);
        }
    }
}
