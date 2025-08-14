using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;

namespace SmartICAVI.UserControls
{
    class RunMoveCheckBox : CheckBox, ISelectControl
    {
        public enum MoveTypes { none = 0, abs, rel, home, seq };
        private SystemService sysService = null;
        private MotionService motionService = null;
        private DioService dioSerivce = null;
        public TextBox TextBox { get; set; }

        public int Axis { get; set; }
        public bool IsMoved { get; set; }
        public MoveTypes MoveType { get; set; }
        public int SequenceType { get; set; }


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
                    OnEventState((object)sysService.State, null);
            }
        }

        public RunMoveCheckBox()
        {
            Axis = -1;
            IsMoved = false;
            MoveType = MoveTypes.none;
            SequenceType = 0;

            sysService = SystemService.Singleton;
            motionService = MotionService.Singleton;
            dioSerivce = DioService.Singleton;
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
                    if (null == dioSerivce)
                        dioSerivce = DioService.Singleton;


                    sysService.EventState += OnEventState;
                    //motionService.EventMotionDone += OnEventMotionDone;

                    if (null != sysService)
                        OnEventState((object)sysService.State, null);
                }
                else
                {
                    if (null != sysService)
                        sysService.EventState -= OnEventState;

                    //if (null != motionService)
                    //    motionService.EventMotionDone -= OnEventMotionDone;

                    sysService = null;
                    motionService = null;
                }
            }
        }

        protected override void OnClick()
        {
            base.OnClick();
            return;

            //IsMoved = true;
            //base.OnClick();

            //// double Click 방지
            //if (false == IsChecked)
            //{
            //    IsChecked = true;
            //}
            //else
            //{
            //    //IsMoved = true;
            //    if ((null != motionService) && (-1 < Axis))
            //    {
            //        switch (MoveType)
            //        {
            //            case MoveTypes.abs:
            //                if (null != TextBox)
            //                    motionService.AMove(Axis, double.Parse(TextBox.Text), 0.0, 0.0);
            //                break;
            //            case MoveTypes.rel:
            //                if (null != TextBox)
            //                    motionService.RMove(Axis, double.Parse(TextBox.Text), 0.0, 0.0);
            //                break;
            //            case MoveTypes.home:
            //                motionService.HomeSearch(Axis);
            //                break;
            //            //case MoveTypes.seq:
            //            //    if (0 != SequenceType)
            //            //        motionService.SeqMove(Axis, (MotionService.Sequences)SequenceType);
            //            //    break;
            //        }
            //    }
            //}
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
                    IsChecked = false;
                    IsMoved = false;

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
                        enable = true;
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

        private void SetEnable(bool enable)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                IsEnabled = enable;

                if (false == enable)
                    IsChecked = false;

            }), DispatcherPriority.ContextIdle, null);
        }
    }
}
