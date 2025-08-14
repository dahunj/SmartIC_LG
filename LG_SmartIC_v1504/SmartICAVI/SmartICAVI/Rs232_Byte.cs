using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartICAVI
{
    class Rs232_Byte : Rs232
    {
        Byte[] bt = new Byte[512];

        protected override void ThreadRead()
        {
            while (true == readContinue)
            {
                try
                {
                    int ret = serial.Read(bt, 0, 512);

                    if (bt[0] != 0 && ret != 0)
                    {
                        readData = "";
                        for (int i = 0; i < ret; ++i)
                        {
                            Char ch = Convert.ToChar(bt[i]);
                            readData += ch.ToString();
                        }
                        FireMessageEvent(readData);
                    }
                }
                catch (TimeoutException exc)
                {
                    string str = exc.Message;
                }
                catch (Exception exc)
                {
                    readContinue = false;
                    Log_Exception.WriteLine("Exception Rs232.ThreadRead() => " + exc.Message);
                }
            }

            if (serial.IsOpen)
            {
                serial.Close();
            }
        }
    }
}
