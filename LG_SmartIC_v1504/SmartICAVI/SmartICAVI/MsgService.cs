using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;
using System.Windows.Threading;

namespace SmartICAVI
{
    class MsgService
    {
        public Window OwnerWindow { get; set; }

        private List<MessageWindow> listMessage = new List<MessageWindow>();
        private List<AlarmWindow> listAlarm = new List<AlarmWindow>();
        private List<InputWindow> listInput = new List<InputWindow>();

        private MesService mesService = null;

        static bool InvokeRequired
        {
            get { return Dispatcher.CurrentDispatcher != Application.Current.Dispatcher; }
        }

        #region Singleton
        private static MsgService singleton = null;

        public static MsgService Singleton
        {
            get
            {
                if (null == singleton)
                {
                    singleton = new MsgService();
                }

                return singleton;
            }
        }

        public void ReleaseSingleton()
        {
            if (null != singleton)
            {
                singleton = null;
            }
        }
        #endregion

        public void Initialize(Window win)
        {
            OwnerWindow = win;

            mesService = MesService.Singleton;
        }

        public void UnInitialize()
        {
            HideMessageAll();
            HideAlarmAll();
            HideInputAll();
        }



        public bool ShowMessage(int number, bool modalless = true)
        {
            bool ret = false;

            if (InvokeRequired)
            {
                OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                            (ThreadStart)delegate()
                                            {
                                                ret = SetShowMessage(number, modalless);
                                            }
                );
            }
            else
            {
                ret = SetShowMessage(number, modalless);
            }

            return ret;
        }

        public bool SetShowMessage(int number, bool modalless = true)
        {
            for (int i = 0; i < listMessage.Count; ++i)
            {
                if (number == listMessage[i].Number)
                    return false;
            }

            DataService dataService = DataService.Singleton;

            MessageWindow msg = new MessageWindow();

            //string title = "메시지";
            string strNumber = string.Format("메시지 코드({0})", (int)number);
            string message;
            string description;
            string imgPath = dataService.DataPath + "\\Images\\LightAlarm";

            string path = dataService.IniPath + "\\LightAlarm.ini";
            string section = "LIGHT_ALARM";
            string key = string.Format("{0}", number);
            string key_description = string.Format("{0}_Description", number);
            string key_Image = string.Format("{0}_Image", number);

            IniFile ini = new IniFile();

            message = ini.Read(section, key, "Unknown Message", path);
            description = ini.Read(section, key_description, "설정되지 않은 메시지입니다. 메시지 번호를 통보 해 주십시오.", path);
            imgPath = dataService.DataPath + "\\Images\\LightAlarm\\" + ini.Read(section, key_Image, "Default.jpg", path);

            description = description.Replace("\\n", Environment.NewLine);

            Log_Trace.WriteLine("MsgService.ShowMessage() " + message);

            //msg.TitleText = title;
            msg.NumberText = strNumber;
            msg.Number = number;
            msg.Message = message;
            msg.Description = description;
            msg.ImagePath = imgPath;
            msg.Modalless = modalless;

            msg.Owner = OwnerWindow;

            listMessage.Add(msg);
            msg.CloseEvent += OnMessageCloseEvent;

            if (true == modalless)
            {
                msg.Show();
            }
            else
            {
                return (bool)msg.ShowDialog();
            }

            return true;
        }

        public void HideMessage(int number)
        {
            for (int i = 0; i < listMessage.Count; ++i)
            {
                if (listMessage[i].Number == number)
                {
                    Log_Trace.WriteLine("Close Message Number=" + number.ToString());

                    if (InvokeRequired)
                    {
                        OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                                    (ThreadStart)delegate()
                                                    {
                                                        listMessage[i].Close();
                                                    }
                        );
                    }
                    else
                    {
                        listMessage[i].Close();
                    }


                    break;
                }
            }
        }

        public void HideMessageAll()
        {
            for (int i = 0; i < listMessage.Count; ++i)
            {
                Log_Trace.WriteLine("Close Message Number=" + listMessage[i].Number.ToString());
                listMessage[i].Close();
            }
        }

        private void OnMessageCloseEvent(object sender, EventArgs e)
        {
            MessageWindow msg = sender as MessageWindow;
            if (null != msg)
            {
                if (0 != listMessage.Count)
                {
                    msg.CloseEvent -= OnMessageCloseEvent;
                    listMessage.Remove(msg);
                }
            }
        }



        public bool ShowAlarm(int number, bool modalless = true)
        {
            bool ret = false;

            if (InvokeRequired)
            {
                OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                            (ThreadStart)delegate()
                                            {
                                                ret = SetShowAlarm(number, modalless);
                                            }
                );
            }
            else
            {
                ret = SetShowAlarm(number, modalless);
            }

            return ret;
        }

        public bool SetShowAlarm(int number, bool modalless = true)
        {
            for (int i = 0; i < listAlarm.Count; ++i)
            {
                if (number == listAlarm[i].Number)
                    return false;
            }



            AlarmWindow alaWindow = new AlarmWindow();

            DataService dataService = DataService.Singleton;

            //string title = "알람";
            string txtNumber = string.Format("알람코드 ({0})", number);
            string alarm;
            string description;
            string imgPath = dataService.DataPath + "\\Images\\HeavyAlarm";

            string path = dataService.IniPath + "\\HeavyAlarm.ini";
            string section = "HEAVY_ALARM";
            string key = string.Format("{0}", number);
            string key_description = string.Format("{0}_Description", number);
            string key_Image = string.Format("{0}_Image", number);
            string key_altxt = string.Format("{0}__ALTX", number);

            IniFile ini = new IniFile();



            alarm = ini.Read(section, key, "Unknown Alarm", path);
            description = ini.Read(section, key_description, "설정되지 않은 알람입니다. 알람 번호를 통보 해 주십시오.", path);
            imgPath = dataService.DataPath + "\\Images\\HeavyAlarm\\" + ini.Read(section, key_Image, "Default.jpg", path);

            key = string.Format("{0}_UNITNO", number);
            string unitNo = ini.Read(section, key, "21", path);

            string altext = ini.Read(section, key_altxt, "", path);


            description = description.Replace("\\n", Environment.NewLine);

            //Log_Error.WriteLine(number, alarm);


            alaWindow.NumberText = txtNumber;
            alaWindow.Number = number;
            alaWindow.Message = alarm;
            alaWindow.Description = description;
            alaWindow.ImagePath = imgPath;
            alaWindow.Modalless = modalless;

            alaWindow.Owner = OwnerWindow;

            listAlarm.Add(alaWindow);
            alaWindow.CloseEvent += OnAlarmCloseEvent;

            Log_Error.WriteLine(alaWindow.Number, alarm);

            mesService.Send(MesService.commands.send_alarm, 1, number.ToString());

            if (true == modalless)
            {
                alaWindow.Show();
            }
            else
            {
                return (bool)alaWindow.ShowDialog();
            }

            return true;
        }

        public void HideAlarm(int number)
        {
            for (int i = 0; i < listAlarm.Count; ++i)
            {
                if (listAlarm[i].Number == number)
                {
                    Log_Trace.WriteLine("Close Alarm Number=" + number.ToString());
                    listAlarm[i].Close();
                    break;
                }
            }
        }

        public void HideAlarmAll()
        {
            for (int i = 0; i < listAlarm.Count; ++i)
            {
                Log_Trace.WriteLine("Close Alarm Number=" + listAlarm[i].Number.ToString());
                listAlarm[i].Close();

                mesService.Send(MesService.commands.send_alarm, 0, listAlarm[i].Number.ToString());
            }
        }

        private void OnAlarmCloseEvent(object sender, EventArgs e)
        {
            AlarmWindow alarm = sender as AlarmWindow;
            if (null != alarm)
            {
                if (0 != listAlarm.Count)
                {
                    alarm.CloseEvent -= OnAlarmCloseEvent;

                    mesService.Send(MesService.commands.send_alarm, 0, alarm.Number.ToString());

                    listAlarm.Remove(alarm);
                }
            }
        }


        // 2019.07.03 khs - LotEnd 직전에 사번 입력 받도록 입력창 추가 
        public bool ShowInput(int number, bool modalless = true)
        {
            bool ret = false;
            IsInputWindowClosed = false;

            if (InvokeRequired)
            {
                OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                            (ThreadStart)delegate()
                                            {
                                                ret = SetShowInput(number, modalless);
                                                IsInputWindowClosed = true;     // 창이 닫힐때
                                            }
                );
            }
            else
            {
                ret = SetShowInput(number, modalless);
            }

            return ret;
        }

        public bool SetShowInput(int number, bool modalless = true)
        {
            /*
            for (int i = 0; i < listInput.Count; ++i)
            {
                if (number == listInput[i].Number)
                    return false;
            }
            */

            DataService dataService = DataService.Singleton;

            InputWindow msg = new InputWindow();

            //string title = "메시지";
            string strNumber = string.Format("사번입력");
            string message;
            string description;
            //string imgPath = dataService.DataPath + "\\Images\\LightAlarm";

            message = string.Format(" 마지막 Punch 상태 확인");
            description = string.Format("리뷰 화면의 Punch 이미지와 실제 Punch 상태를 확인하시고, 사번을 입력 후 확인 버튼을 눌러주세요.");
            //imgPath = dataService.DataPath + "\\Images\\LightAlarm\\" + "Default.jpg";

            description = description.Replace("\\n", Environment.NewLine);


            //msg.NumberText = strNumber;
            msg.Number = number;
            msg.Message = message;
            msg.Description = description;
            //msg.ImagePath = imgPath;
            msg.Modalless = modalless;

            msg.Owner = OwnerWindow;

            listInput.Add(msg);
            msg.CloseEvent += OnInputCloseEvent;

            if (true == modalless)
            {
                msg.Show();
            }
            else
            {
                return (bool)msg.ShowDialog();
            }

            return true;
        }

        public void HideInput(int number)
        {
            for (int i = 0; i < listInput.Count; ++i)
            {
                if (listInput[i].Number == number)
                {
                    Log_Trace.WriteLine("Close Input Number=" + number.ToString());

                    if (InvokeRequired)
                    {
                        OwnerWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                                    (ThreadStart)delegate()
                                                    {
                                                        listInput[i].Close();
                                                    }
                        );
                    }
                    else
                    {
                        listInput[i].Close();
                    }


                    break;
                }
            }
        }

        public void HideInputAll()
        {
            for (int i = 0; i < listInput.Count; ++i)
            {
                Log_Trace.WriteLine("Close Input Number=" + listInput[i].Number.ToString());
                listInput[i].Close();
            }
        }

        private void OnInputCloseEvent(object sender, EventArgs e)
        {
            InputWindow msg = sender as InputWindow;
            if (null != msg)
            {
                if (0 != listMessage.Count)
                {
                    msg.CloseEvent -= OnInputCloseEvent;
                    listInput.Remove(msg);
                }
            }
        }

        public bool IsInputWindowClosed { get; set; }



    }
}
