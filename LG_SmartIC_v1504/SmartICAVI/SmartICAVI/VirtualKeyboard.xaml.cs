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
using System.Diagnostics;

namespace SmartICAVI
{
    /// <summary>
    /// VirtualKeyboard.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class VirtualKeyboard : Window
    {
        private Button[] twoButtons;
        private Button[] buttons;
        private KeyButton[] kButtons;

        private Key[] keys;
        private Key[] tags;
        private string[] keyText;
        private string[] upKeyText;

        TextBox textBox;
        PasswordBox pwdBox;

        bool shift = false;
        bool cap = false;

        private bool Password { get; set; }

        public VirtualKeyboard(object sender)
        {
            InitializeComponent();
            this.textBox = sender as TextBox;

            if (this.textBox != null)
            {

                tbxText.Text = this.textBox.Text;

                if (null != this.textBox.Tag)
                    lbText.Content = this.textBox.Tag.ToString();

                Password = false;

                pwdText.Visibility = System.Windows.Visibility.Hidden;
                tbxText.Visibility = System.Windows.Visibility.Visible;

                tbxText.CaretIndex = tbxText.Text.Length;

                tbxText.Focus();
            }
            else
            {
                pwdBox = sender as PasswordBox;

                pwdText.Password = pwdBox.Password;

                Password = true;

                if (null != this.pwdBox.Tag)
                    lbText.Content = this.pwdBox.Tag.ToString();

                pwdText.Visibility = System.Windows.Visibility.Visible;
                tbxText.Visibility = System.Windows.Visibility.Hidden;

                pwdText.Focus();
            }


            btnEnter.IsDefault = true;
            btnCancel.IsCancel = true;
            //pwdText.Password = tbxText.Text;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            shift = false;

            twoButtons = new Button[]{
                btnVK_Oem3, btnVK_1, btnVK_2, btnVK_3, btnVK_4, btnVK_5, btnVK_6, btnVK_7, btnVK_8, btnVK_9, btnVK_0, btnVK_OemMinus, btnVK_OemPlus,
                btnVK_Q, btnVK_W, btnVK_E, btnVK_R, btnVK_T, btnVK_Y, btnVK_U, btnVK_I, btnVK_O, btnVK_P, 
                btnVK_OemOpenBrackets, btnVK_Oem6, btnVK_Oem5,
                btnVK_A, btnVK_S, btnVK_D, btnVK_F, btnVK_G, btnVK_H, btnVK_J, btnVK_K, btnVK_L, 
                btnVK_Oem1, btnVK_Oem7,
                btnVK_Z, btnVK_X, btnVK_C, btnVK_V, btnVK_B, btnVK_N, btnVK_M, 
                btnVK_OemComma, btnVK_OemPeriod, btnVK_OemQuestion
            };

            kButtons = new KeyButton[] { new KeyButton("~", "`"), new KeyButton("!", "1"), new KeyButton("@", "2"), new KeyButton("#", "3"),
                new KeyButton("$", "4"), new KeyButton("%", "5"), new KeyButton("^", "6"), new KeyButton("&", "7"), new KeyButton("*", "8"),
                new KeyButton("(", "9"), new KeyButton(")", "0"), new KeyButton("_", "-"), new KeyButton("+", "="),
                new KeyButton("Q", "q"), new KeyButton("W", "w"), new KeyButton("E", "e"), new KeyButton("R", "r"), new KeyButton("T", "t"), new KeyButton("Y", "y"), new KeyButton("U", "u"), new KeyButton("I", "i"), new KeyButton("O", "o"), new KeyButton("P", "p"),
                new KeyButton("{", "["), new KeyButton("}", "]"), new KeyButton("|", "\\"),
                new KeyButton("A", "a"), new KeyButton("S", "s"), new KeyButton("D", "d"), new KeyButton("F", "f"), new KeyButton("G", "g"), new KeyButton("H", "h"), new KeyButton("J", "j"), new KeyButton("K", "k"), new KeyButton("L", "l"),
                new KeyButton(":", ";"), new KeyButton("\"", "\'"),
                new KeyButton("Z", "z"), new KeyButton("X", "x"), new KeyButton("C", "c"), new KeyButton("V", "v"), new KeyButton("B", "b"), new KeyButton("N", "n"), new KeyButton("M", "m"),
                new KeyButton("<", ","), new KeyButton(">", "."), new KeyButton("?", "/")
            };

            keys = new Key[]{
                Key.Oem3, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.D0, Key.OemMinus, Key.OemPlus,
                Key.Q, Key.W, Key.E, Key.R, Key.T, Key.Y, Key.U, Key.I, Key.O, Key.P, Key.OemOpenBrackets, Key.Oem6, Key.Oem5,
                Key.A, Key.S, Key.D, Key.F, Key.G, Key.H, Key.J, Key.K, Key.L, Key.Oem1, Key.OemQuotes, 
                Key.Z, Key.X, Key.C, Key.V, Key.B, Key.N, Key.M, Key.OemComma, Key.OemPeriod, Key.OemQuestion
            };

            keyText = new string[]{
                "`", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "=",
                "q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "[", "]", "\\",
                "a", "s", "d", "f", "g", "h", "j", "k", "l", ";", "'",
                "z", "x", "c", "v", "b", "n", "m", ",", ".", "/"
            };
            upKeyText = new string[]{
                "~", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_", "+",
                "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "{", "}", "|",
                "A", "S", "D", "F", "G", "H", "J", "K", "L", ":", "\"",
                "Z", "X", "C", "V", "B", "N", "M", "<", ">", "?"
            };


            for (int i = 0; i < twoButtons.Length; ++i)
            {
                twoButtons[i].Content = kButtons[i];
                //twoButtons[i].Tag = keyText[i];
                twoButtons[i].Tag = keys[i];
                twoButtons[i].Click += btnVK_Click;
            }

            buttons = new Button[]{
                btnBackspace, btnTab, btnCapsLock, btnEnter, btnShift_Left, btnShift_Right, btnVK_SPACE, btnOK, btnCancel
            };

            // Key.Back,
            // Key.Tab, Key.Capital, Key.LeftShift, Key.LeftC trl, Key.LWin, Key.System(left alt)
            // Key.HanjaMode, Key.Space, Key.ImeProcessed(한/영), Key.ImeProcessed(Right Alt),  Key.RWin, 
            tags = new Key[]{
                Key.Back, Key.Tab, Key.Capital, Key.Enter, Key.LeftShift, Key.RightShift, Key.Space, Key.Enter, Key.Escape
            };

            for (int i = 0; i < buttons.Length; ++i)
            {
                buttons[i].Tag = tags[i];


                buttons[i].Click += btnOneVK_Click;
            }

            this.KeyDown += OnKeyDownHandler;
            this.Focus();

            tbxText.IsEnabled = true;
            tbxText.Focus();

            //tbxText.SelectAll();

            //Log.Write("Virtual Keyboard Start");
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            this.Topmost = false;
        }


        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            //MessageBox.Show(e.Key.ToString());
            //Debug.WriteLine(string.Format("{0}, {1}", e.Key, e.KeyStates));

            //tbxText.Focus();
            //KeyToString(e.Key);  

            if (false == Password)
                tbxText.Focus();
            else
                pwdText.Focus();

            //btnKey(e.Key);
        }

        private void btnShift_Left_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < twoButtons.Length; ++i)
            {

                kButtons[i].Shift = !cap;

                twoButtons[i].Content = null;
                twoButtons[i].Content = kButtons[i];
            }
        }

        private void btnVK_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < twoButtons.Length; ++i)
            {
                kButtons[i].Shift = cap;

                twoButtons[i].Content = null;
                twoButtons[i].Content = kButtons[i];
            }

            Button btn = sender as Button;
            //tbxText.Text += (string)btn.Tag;
            KeyToString((Key)btn.Tag);

            shift = cap;

            if (false == Password)
                tbxText.Focus();
            else
                pwdText.Focus();
        }

        private void KeyToString(Key key)
        {
            int index = 0;

            string strF = "";
            string strKey = "";

            if ((Keyboard.Modifiers == ModifierKeys.Shift) || (true == shift))
            {
                for (int i = 0; i < keys.Length; ++i)
                {
                    if (keys[i] == key)
                    {
                        index = tbxText.CaretIndex;
                        strKey = upKeyText[i];
                        //strF = tbxText.Text.Insert(index, upKeyText[i]);

                        //tbxText.Text = strF;
                        //tbxText.CaretIndex = ++index;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < keys.Length; ++i)
                {
                    if (keys[i] == key)
                    {
                        index = tbxText.CaretIndex;
                        strKey = keyText[i];

                        //strF = tbxText.Text.Insert(index, keyText[i]);
                        //tbxText.Text = strF;
                        //tbxText.CaretIndex = ++index;

                        break;
                    }
                }
            }

            if (false == Password)
            {
                strF = tbxText.Text.Insert(index, strKey);
                tbxText.Text = strF;
                tbxText.CaretIndex = ++index;
            }
            else
            {
                pwdText.Password += strKey;

                //pwdBox
                //pwdText.CaretIndex = ++index;
            }
            //    pwdText.Focus();

            //pwdText.Password = tbxText.Text;

        }


        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            if (false == Password)
            {
                int index = tbxText.CaretIndex;

                if (0 < index)
                    tbxText.CaretIndex = --index;

                tbxText.Focus();
            }
            else
            {
                pwdText.Focus();
            }
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            if (false == Password)
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
            else
            {
                pwdText.Focus();
            }

            //pwdText.Password = tbxText.Text;
        }

        private void btnDelAll_Click(object sender, RoutedEventArgs e)
        {
            if (false == Password)
            {
                tbxText.Text = "";
                tbxText.Focus();
            }
            else
            {
                pwdText.Password = "";
                pwdText.Focus();
            }

            //pwdText.Password = tbxText.Text;
        }


        private void btnOneVK_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            btnKey((Key)btn.Tag);
        }

        private void btnKey(Key key)
        {

            switch (key)
            {
                case Key.Back:// = 2,
                    KeyBack();
                    break;
                case Key.Tab:// = 3,
                    KeyTab();
                    break;
                case Key.Enter:     // 6
                    if (false == Password)
                        textBox.Text = tbxText.Text;
                    else
                        pwdBox.Password = pwdText.Password;
                    this.Close();
                    break;
                case Key.Capital:   // 8
                    KeyCap();
                    break;
                case Key.Escape:// = 13,
                    this.Close();
                    break;
                case Key.Space:// = 18
                    KeySpace();
                    break;
                case Key.LeftShift:// = 116,
                case Key.RightShift:// = 117,
                    KeyShift();

                    break;
                case Key.LeftCtrl:// = 118,
                    break;
                case Key.RightCtrl:// = 119,
                    break;
                case Key.LeftAlt:// = 120,
                    break;
                case Key.RightAlt:// = 121,
                    break;
            }
        }

        private void KeyBack()
        {
            if (false == Password)
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
            else
            {
                pwdText.Focus();
            }

            //pwdText.Password = tbxText.Text;
        }

        private void KeySpace()
        {
            if (false == Password)
            {
                int index = tbxText.CaretIndex;
                string strF = tbxText.Text.Insert(index, " ");
                tbxText.Text = strF;
                tbxText.CaretIndex = ++index;

                tbxText.Focus();

                //pwdText.Password = tbxText.Text;
            }
            else
            {
                pwdText.Focus();
            }
        }

        private void KeyCap()
        {
            cap = !cap;
            shift = cap;
            for (int i = 0; i < twoButtons.Length; ++i)
            {
                kButtons[i].Shift = cap;

                twoButtons[i].Content = null;
                twoButtons[i].Content = kButtons[i];
            }
        }

        private void KeyTab()
        {
            if (false == Password)
            {
                int index = tbxText.CaretIndex;
                int length = tbxText.Text.Length;
                string strF = "";
                string strE = "";

                if (index - 1 > 0)
                    strF = tbxText.Text.Substring(0, index);

                if (index < length - 1)
                    strE = tbxText.Text.Substring(index, length - index);

                tbxText.Text = strF + "\t" + strE;

                if (0 < index)
                    tbxText.CaretIndex = ++index;

                tbxText.Focus();

                //pwdText.Password = tbxText.Text;
            }
            else
            {
                pwdText.Focus();
            }
        }

        private void KeyShift()
        {
            bool temp = shift;

            shift = !cap;

            if (shift == temp)
                shift = !temp;

            for (int i = 0; i < twoButtons.Length; ++i)
            {
                kButtons[i].Shift = shift;

                twoButtons[i].Content = null;
                twoButtons[i].Content = kButtons[i];
            }
        }

    }
}
