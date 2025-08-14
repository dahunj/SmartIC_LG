using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SmartICAVI
{
    class KeyButton
    {
        public string Up
        {
            get;
            set;
        }

        public string Down
        {
            get;
            set;
        }

        public FontWeight UpWeight
        {
            get;
            set;
        }

        public FontWeight DownWeight
        {
            get;
            set;
        }

        public double UpSize
        {
            get;
            set;
        }

        public double DownSize
        {
            get;
            set;
        }

        public bool Shift
        {
            set
            {
                if (true == value)
                {
                    UpWeight = FontWeights.Bold;
                    DownWeight = FontWeights.Light;

                    UpSize = 20;
                    DownSize = 12;
                }
                else
                {
                    UpWeight = FontWeights.Light;
                    DownWeight = FontWeights.Bold;

                    UpSize = 12;
                    DownSize = 20;
                }
            }
        }

        public KeyButton(string up, string down)
        {
            Up = up;
            Down = down;

            UpWeight = FontWeights.Light;
            DownWeight = FontWeights.Bold;

            UpSize = 12;
            DownSize = 20;
        }
    }
}
