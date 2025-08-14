using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Controls;

namespace SmartICAVI.UserControls
{
    class SelectTextBox : TextBox, ISelectControl
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
                IsEnabled = value;
            }
        }

        public SelectTextBox()
        {
        }
    }
}
