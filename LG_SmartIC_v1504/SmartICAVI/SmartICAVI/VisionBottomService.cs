namespace SmartICAVI
{
    class VisionBottomService : VisionUtill
    {
        #region Singletons
        private static VisionBottomService singleton = null;

        public static VisionBottomService Singleton
        {
            get
            {
                if (null == singleton)
                {
                    singleton = new VisionBottomService();
                }

                return singleton;
            }
        }

        public void ReleaseSingleton()
        {
            if (null != singleton)
            {
                singleton.UnInitialize();
                singleton = null;
            }
        }
        #endregion

        public override void Open()
        {
            VisionType = "BOTTOM1";

            if (null != dataService)
            {
                Log_Trace.WriteLine(string.Format("Bottom Vision UDP Service server open Port:{0}", dataService.DataSystem.BottomHPort));
                if (0 != server.Open(dataService.DataSystem.BottomHPort))
                {
                    // Error 
                    Log_Trace.WriteLine("Bottom Vision UDP Service server open failed");
                }
            }
        }

        public override void Connect()
        {
            if (null != dataService)
            {
                Log_Trace.WriteLine(string.Format("Bottom Vision UDP Service client connect IP:{0}, Port:{1}", dataService.DataSystem.BottomIPAddress, dataService.DataSystem.BottomPort));

                if (0 != client.Connect(dataService.DataSystem.BottomIPAddress, dataService.DataSystem.BottomPort))
                {
                    // Error
                    Log_Trace.WriteLine("Bottom Vision UDP Service client connect failed");
                }
            }
        }

        protected override void ReadProc_Light(string[] messages)
        {
            if ("UPDATE" == messages[1])
            {
                Write("LIGHT,REPLY");

                if (2 < messages.Length)
                {
                    if ("ON" == messages[2])
                        seqService.SetLight(2, true);
                    else
                        seqService.SetLight(2, false);
                }
            }
        }

        protected override void AddResultValues(int index, string value1, string value2, string vision="BOTTOM1")
        {
            //// must have delete
            //value1 = "G";
            //value2 = "G";
            ////

            // Bottom vision 의 index 번호 확인
            //index = dataService.DataResult.Bottom.ListRaw.Count;

            //System.Diagnostics.Debug.WriteLine("index = {0}", index);

            if ((true == dataService.IsEndTop) && (index > dataService.IndexEndTop))
            {
                if (false == dataService.IsEndBottom)
                {
                    bottom2Serivce.SetScanStop();
                    bottomSerivce.SetScanStop();
                }

                dataService.IsEndBottom = true;

                return;
            }

            if ("END" != value1 && "END" != value2)
            {
                dataService.DataResult.AddBottom(index, value1, value2, vision);
            }
            else
            {
                if (false == dataService.IsEndBottom)
                {
                    bottom2Serivce.SetScanStop();
                    bottomSerivce.SetScanStop();
                }

                dataService.IsEndBottom = true;
            }
        }

        protected override void AddResultValues3(int index, string value1, string value2, string value3, string vision = "BOTTOM1")
        {
            if ((true == dataService.IsEndTop) && (index > dataService.IndexEndTop))
            {
                if (false == dataService.IsEndBottom)
                {
                    bottom2Serivce.SetScanStop();
                    bottomSerivce.SetScanStop();
                }

                dataService.IsEndBottom = true;

                return;
            }

            if ("END" != value1 && "END" != value2 && "END" != value3)
            {
                dataService.DataResult.AddBottom3(index, value1, value2, value3, vision);
            }
            else
            {
                if (false == dataService.IsEndBottom)
                {
                    bottom2Serivce.SetScanStop();
                    bottomSerivce.SetScanStop();
                }

                dataService.IsEndBottom = true;
            }
        }

        protected override int CheckIndex(int index, int length, string[] messages)
        {
            if (false == dataService.IsEndBottom)
            {
                if (index != dataService.DataResult.Bottom.ListRaw.Count)
                {
                    if (8 < dataService.DataTempBottom.ListTemp.Count)     // 4 -> 8
                    {
                        //System.Diagnostics.Debug.WriteLine("Bottom1 Index = {0}/{1}", index, dataService.DataResult.Bottom.ListRaw.Count);

                        sysService.State = SystemService.States.heavyAlarm;
                        msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.index_bottom);

                        Log_Bottom.WriteLine("Index Error Bottom1 : list({0}), input({1})", dataService.DataResult.Bottom.ListRaw.Count, index);
                    }
                    else
                    {
                        Log_Bottom.WriteLine("AddList temp Bottom1 : list({0}), input({1})", dataService.DataResult.Bottom.ListRaw.Count, index);
                        dataService.DataTempBottom.Add(index, length, messages, "BOTTOM1");
                    }

                    return -1;
                }
            }

            return 0;
        }

        protected override int CheckIndex3(int index, int length, string[] messages)
        {
            if (false == dataService.IsEndBottom)
            {
                if (index != dataService.DataResult.Bottom.ListRaw.Count)
                {
                    if (8 < dataService.DataTempBottom.ListTemp.Count)     // 4 -> 8
                    {
                        //System.Diagnostics.Debug.WriteLine("Bottom1 Index = {0}/{1}", index, dataService.DataResult.Bottom.ListRaw.Count);

                        sysService.State = SystemService.States.heavyAlarm;
                        msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.index_bottom);

                        Log_Bottom.WriteLine("Index Error Bottom1 : list({0}), input({1})", dataService.DataResult.Bottom.ListRaw.Count, index);
                    }
                    else
                    {
                        Log_Bottom.WriteLine("AddList temp Bottom1 : list({0}), input({1})", dataService.DataResult.Bottom.ListRaw.Count, index);
                        dataService.DataTempBottom.Add3(index, length, messages, "BOTTOM1");
                    }

                    return -1;
                }
            }

            return 0;
        }

        protected override void AddTempValues()
        {
            if (0 < dataService.DataTempBottom.ListTemp.Count)
            {
                bool isFound = false;
                int index = 0;

                for (int i = 0; i < dataService.DataTempBottom.ListTemp.Count; ++i)
                {
                    if (dataService.DataResult.Bottom.ListRaw.Count == dataService.DataTempBottom.ListTemp[i].Index)
                    {
                        isFound = true;
                        index = dataService.DataTempBottom.ListTemp[i].Index;

                        Log_Bottom.WriteLine("AddTempValues Bottom1 : list({0}), input({1})", dataService.DataResult.Bottom.ListRaw.Count, index);

                        for (int k = 0; k < dataService.DataTempBottom.ListTemp[i].ListRaw.Count; ++k)
                        {
                            AddResultValues(index + k, dataService.DataTempBottom.ListTemp[i].ListRaw[k].Value1, dataService.DataTempBottom.ListTemp[i].ListRaw[k].Value2, dataService.DataTempBottom.ListTemp[i].ListRaw[k].Type);
                        }

                        dataService.DataTempBottom.ListTemp[i].ListRaw.Clear();
                        dataService.DataTempBottom.ListTemp.RemoveAt(i);

                        break;
                    }

                    if (true == isFound)
                        AddTempValues();
                }
            }
        }

        protected override void AddTempValues3()
        {
            if (0 < dataService.DataTempBottom.ListTemp.Count)
            {
                bool isFound = false;
                int index = 0;

                for (int i = 0; i < dataService.DataTempBottom.ListTemp.Count; ++i)
                {
                    if (dataService.DataResult.Bottom.ListRaw.Count == dataService.DataTempBottom.ListTemp[i].Index)
                    {
                        isFound = true;
                        index = dataService.DataTempBottom.ListTemp[i].Index;

                        Log_Bottom.WriteLine("AddTempValues Bottom1 : list({0}), input({1})", dataService.DataResult.Bottom.ListRaw.Count, index);

                        for (int k = 0; k < dataService.DataTempBottom.ListTemp[i].ListRaw.Count; ++k)
                        {
                            AddResultValues3(index + k, dataService.DataTempBottom.ListTemp[i].ListRaw[k].Value1, dataService.DataTempBottom.ListTemp[i].ListRaw[k].Value2, dataService.DataTempBottom.ListTemp[i].ListRaw[k].Value3, dataService.DataTempBottom.ListTemp[i].ListRaw[k].Type);
                        }

                        dataService.DataTempBottom.ListTemp[i].ListRaw.Clear();
                        dataService.DataTempBottom.ListTemp.RemoveAt(i);

                        break;
                    }

                    if (true == isFound)
                        AddTempValues3();
                }
            }
        }

        protected override void AddLightValue(int index, int value)
        {
            dataService.DataResult.AddLightBottom(index, value);
        }

        protected override void WriteLog(string log)
        {
            Log_BottomVision.WriteLine(log);
        }

        protected override void SetOverFrame()
        {
            sysService.State = SystemService.States.heavyAlarm;
            msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.overframe_bottom);
        }

        protected override void ShowLineError()
        {
            sysService.State = SystemService.States.heavyAlarm;
            msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.line_bottom);
        }
    }
}
