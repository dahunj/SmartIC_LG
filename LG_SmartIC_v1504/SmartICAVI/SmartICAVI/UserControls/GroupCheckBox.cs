using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace SmartICAVI.UserControls
{
    class GroupCheckBox : CheckBox
    {
        List<ISelectControl> selecteds = new List<ISelectControl>();

        public GroupCheckBox()
        {
        }

        public void Add(ISelectControl selected)
        {
            if (!selecteds.Contains(selected))
                selecteds.Add(selected);

            bool value = false;

            if (false == (bool)IsEnabled)
                value = true;
            else
                value = (bool)IsChecked;

            selected.IsControlSelected = value;
        }

        public void Remove(ISelectControl selected)
        {
            selecteds.Remove(selected);
        }

        protected override void OnClick()
        {
            base.OnClick();

            SetChecked();
        }

        protected override void OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);


            if ("IsVisible" == e.Property.ToString())
            {
                if (true == (bool)e.NewValue)
                {
                    IsEnabled = DataService.Singleton.DataSystem.UseSelectParam;
                }
                else
                {

                }
            }
            else if ("IsEnabled" == e.Property.ToString())
            {
                bool value = false;
                if (true == (bool)e.NewValue)
                {
                    value = (bool)IsChecked;
                }
                else
                {
                    value = true;
                }

                //foreach (ISelectControl selected in selecteds)
                //    selected.IsControlSelected = value;
                for (int i = 0; i < selecteds.Count; ++i)
                    selecteds[i].IsControlSelected = value;
            }
            else if ("IsChecked" == e.Property.ToString())
            {
                SetChecked();
            }
        }

        private void SetChecked()
        {
            //foreach (ISelectControl selected in selecteds)
            //    selected.IsControlSelected = (bool)IsChecked;

            for (int i = 0; i < selecteds.Count; ++i)
                selecteds[i].IsControlSelected = (bool)IsChecked;
        }
    }
}
