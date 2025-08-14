using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SmartICAVI
{
    /// <summary>
    /// VirtualKeyNumber.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class VirtualKeyNumber : Window
    {
        private TextBox textBox;
        private Button[] buttons;
        private Key[] tags;

        private Button[] numberButtons;
        private Key[] keys;
        private string[] keyText;

        public VirtualKeyNumber(object sender)
        {
            InitializeComponent();

            this.textBox = sender as TextBox;

            if (this.textBox != null)
            {

                tbxText.Text = this.textBox.Text;

                if (null != this.textBox.Tag)
                    lbText.Content = this.textBox.Tag.ToString();

                tbxText.Visibility = System.Windows.Visibility.Visible;

                tbxText.CaretIndex = tbxText.Text.Length;

                tbxText.Focus();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            numberButtons = new Button[]{
                btnVK_1, btnVK_2, btnVK_3, btnVK_4, btnVK_5, btnVK_6, btnVK_7, btnVK_8, btnVK_9, btnVK_0, 
                btnVK_OemMinus, btnVK_OemPeriod
            };

            keys = new Key[]{
                Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.D0, 
                Key.OemMinus, Key.OemPeriod, 
            };

            keyText = new string[]{
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", 
                "-", "."
            };

            for (int i = 0; i < numberButtons.Length; ++i)
            {
                numberButtons[i].Tag = keys[i];
                numberButtons[i].Click += btnVK_Click;
            }

            buttons = new Button[]{
                btnBackspace, btnEnter, btnCancel
            };

            tags = new Key[]{
                Key.Back, Key.Enter, Key.Escape
            };

            for (int i = 0; i < buttons.Length; ++i)
            {
                buttons[i].Tag = tags[i];


                buttons[i].Click += btnOneVK_Click;
            }

            btnEnter.IsDefault = true;
            btnCancel.IsCancel = true;

            tbxText.Focus();
            //tbxText.SelectAll();
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            this.Topmost = false;
        }


        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {

            int index = tbxText.CaretIndex;

            if (0 < index)
                tbxText.CaretIndex = --index;

            tbxText.Focus();
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            int index = tbxText.CaretIndex;
            int length = tbxText.Text.Length;

            if (length > 0)
            {
                if (length > index)
                    tbxText.CaretIndex = ++index;
            }
            tbxText.Focus();
        }

        private void btnDelAll_Click(object sender, RoutedEventArgs e)
        {
            tbxText.Text = "";
            tbxText.Focus();
        }

        private void KeyToString(Key key)
        {
            int index = 0;

            string strF = "";
            string strKey = "";

            for (int i = 0; i < keys.Length; ++i)
            {
                if (keys[i] == key)
                {
                    index = tbxText.CaretIndex;
                    strKey = keyText[i];

                    strF = tbxText.Text.Insert(index, keyText[i]);
                    tbxText.Text = strF;
                    tbxText.CaretIndex = ++index;

                    break;
                }
            }

        }

        private void btnVK_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            KeyToString((Key)btn.Tag);

            tbxText.Focus();
        }
        private void btnOneVK_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            switch ((Key)btn.Tag)
            {
                case Key.Back:// = 2,
                    KeyBack();
                    break;
                case Key.Enter:     // 6
                    textBox.Text = tbxText.Text;

                    this.Close();
                    break;
                case Key.Escape:// = 13,
                    this.Close();
                    break;
            }
        }

        private void KeyBack()
        {
            int index = tbxText.CaretIndex;
            int length = tbxText.Text.Length;
            string strF = "";
            string strE = "";

            if (index - 1 > 0)
                strF = tbxText.Text.Substring(0, index - 1);

            if (index < length - 1)
                strE = tbxText.Text.Substring(index, length - index);

            tbxText.Text = strF + strE;

            if (0 < index)
                tbxText.CaretIndex = --index;

            tbxText.Focus();
        }
    }
}
