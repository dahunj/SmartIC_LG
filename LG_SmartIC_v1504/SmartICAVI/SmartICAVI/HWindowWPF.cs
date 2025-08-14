using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interop;
using System.Runtime.InteropServices;

using HalconDotNet;

namespace SmartICAVI
{
    class HWindowWPF : HwndHost
    {
        HWindow window = null;

        int width = -1;
        int height = -1;

        public HWindowWPF(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            HSystem.SetCheck("~father");
            window = new HWindow(0, 0, width, height, hwndParent.Handle, "visible", "");
            window.SetPart(0, 0, height - 1, width - 1);
            IntPtr unused;
            IntPtr hwnd = window.GetOsWindowHandle(out unused);
            return new HandleRef(this, hwnd);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            window.Dispose();
        }

        public HWindow HalconWindow
        {
            get { return window; }
        }

        protected override void OnPreviewMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
        }

        //protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    base.OnMouseLeftButtonDown(e);
        //}

        //protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    base.OnMouseLeftButtonUp(e);
        //}

        //protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        //{
        //    base.OnMouseMove(e);
        //}
    }
}
