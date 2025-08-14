using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Controls;

namespace SmartICAVI.UserControls
{
    class GroupControl
    {
        List<CheckBox> controls = new List<CheckBox>();

        public int Count
        {
            get
            {
                return controls.Count;
            }
        }

        public GroupControl()
        {
        }

        ~GroupControl()
        {
            RemoveAll();
        }

        public void Add(CheckBox control)
        {
            if (!controls.Contains(control))
            {
                controls.Add(control);

                control.Checked += new System.Windows.RoutedEventHandler(control_Checked);
            }
        }

        void control_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            CheckBox btn = sender as CheckBox;

            if (null != btn)
            {
                if (true == btn.IsChecked)
                {
                    for (int i = 0; i < controls.Count; ++i)
                    {
                        if (btn != controls[i])
                            controls[i].IsChecked = false;
                    }
                }
            }
        }

        public void Remove(CheckBox control)
        {
            control.Checked -= control_Checked;

            controls.Remove(control);
        }

        public void RemoveAll()
        {
            if (0 < controls.Count)
            {
                for (int i = 0; i < controls.Count; ++i)
                {
                    controls[i].Checked -= control_Checked;

                    controls.RemoveAt(i);
                }
            }
        }
    }
}
