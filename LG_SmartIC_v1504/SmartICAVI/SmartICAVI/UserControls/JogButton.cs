using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;

namespace SmartICAVI.UserControls
{
    class JogButton : Button, ISelectControl
    {
        public enum JogTypes { none = 0, minus, puls };

        private SystemService sysService = null;
        private MotionService motionService = null;
        //private DioService dioService = null;

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

                if (null != sysService)
                    OnEventState((object)sysService, null);

            }
        }

        public int Axis { get; set; }
        public JogTypes JogType { get; set; }
        public bool IsMoved { get; set; }
        public bool IsRunEnabled { get; set; }

        public JogButton()
        {
            IsMoved = false;
            Axis = -1;
            JogType = JogTypes.none;
            IsRunEnabled = false;
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
                    if (null == motionService)
                        motionService = MotionService.Singleton;
                    //if (null == dioService)
                    //    dioService = DioService.Singleton;

                    sysService.EventState += OnEventState;
                    motionService.EventMotionDone += OnEventMotionDone;
                    //dioService.EventInport += OnEventInport;

                    //if (null != sysService)
                    //    StatusEvent(sysService.State);
                }
                else
                {
                    if (null != sysService)
                        sysService.EventState -= OnEventState;

                    if (null != motionService)
                        motionService.EventMotionDone -= OnEventMotionDone;

                    sysService = null;
                    motionService = null;
                    //dioService = null;
                }
            }
        }

        protected override void OnPreviewMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            IsMoved = true;

            //if ((null != motionService) && (-1 < Axis))
            //{
            //    switch (JogType)
            //    {
            //        case JogTypes.minus:
            //            motionService.JogN(Axis, 0.0, 0.0);
            //            break;
            //        case JogTypes.puls:
            //            motionService.JogP(Axis, 0.0, 0.0);
            //            break;
            //        default:
            //            break;
            //    }
            //}
        }

        protected override void OnPreviewMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);

            IsMoved = false;

            //if ((null != motionService) && (-1 < Axis))
            //    motionService.Stop(Axis);
        }

        private void OnEventMotionDone(int axis, int value)
        {
            if (axis == Axis)
            {
                // 구동중
                if (0 == value)
                {
                    if (false == IsMoved)
                        SetEnable(false);
                }
                else
                {
                    OnEventState((object)sysService.State, null);
                }
            }
        }

        private void OnEventState(object sender, EventArgs arg)
        {
            //SystemService.States states = (SystemService.States)sender;
            SystemService.States states = SystemService.Singleton.State;
            if (true == IsVisible)
            {
                bool enable = true;

                switch (states)
                {
                    case SystemService.States.none:             // 0
                    case SystemService.States.ready:            // 1    
                    case SystemService.States.idle:             // 2
                        enable = true;
                        break;
                    case SystemService.States.run:              // 3
                        enable = IsRunEnabled;
                        break;
                    case SystemService.States.pause:            // 4
                    case SystemService.States.homing:           // 5
                        enable = false;
                        break;
                    case SystemService.States.homeDone:         // 6
                    case SystemService.States.jobDone:          // 7
                    case SystemService.States.reset:            // 10
                        enable = true;
                        break;
                    case SystemService.States.lightAlarm:       // 11
                        break;
                    case SystemService.States.stop:
                    case SystemService.States.heavyAlarm:       // 20
                    case SystemService.States.emg:              // 40
                        enable = false;
                        break;
                    default:
                        break;
                } // switch (status)

                if ((true == enable) && (true == IsControlSelected))
                {
                    SetEnable(true);
                }
                else
                {
                    IsMoved = false;
                    SetEnable(false);
                }

            } // if (true == IsControlSelected)
        }

        //private void OnEventInport(int port, int value)
        //{
        //}

        private void SetEnable(bool enable)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            { 
                IsEnabled = enable;
            }), DispatcherPriority.ContextIdle, null);
        }
    }
}
