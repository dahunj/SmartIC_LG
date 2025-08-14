using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartICAVI
{
    class VirtualKeyboardService
    {
        public delegate void VirtualKeyboardEventHandler(object sender, int type);
        public event VirtualKeyboardEventHandler VirtualKeyboardEvent;
        public void FireVirtualKeyboard(object sender, int type = 0)
        {
            if (null != VirtualKeyboardEvent)
                VirtualKeyboardEvent(sender, type);
        }

        public delegate void VirtualKeyNumberEventHandler(object sender);
        public event VirtualKeyNumberEventHandler VirtualKeyNumberEvent;
        public void FireVirtualKeyNumber(object sender)
        {
            if (null != VirtualKeyNumberEvent)
                VirtualKeyNumberEvent(sender);
        }

        private static VirtualKeyboardService singleton = null;

        public static VirtualKeyboardService Singleton
        {
            get
            {
                if (null == singleton)
                {
                    singleton = new VirtualKeyboardService();
                }

                return singleton;
            }
        }

        public static VirtualKeyboardService GetSingleton()
        {
            if (null == singleton)
            {
                singleton = new VirtualKeyboardService();
            }

            return singleton;
        }

        public void ReleaseSingleton()
        {
            if (null != singleton)
            {
                singleton = null;
            }
        }
    }
}
