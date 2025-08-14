namespace SmartICAVI
{
    class VisionTop2Service : VisionUtill
    {
        #region Singletons
        private static VisionTop2Service singleton = null;

        public static VisionTop2Service Singleton
        {
            get
            {
                if (null == singleton)
                {
                    singleton = new VisionTop2Service();
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
            VisionType = "TOP2";

            if (null != dataService)
            {
                Log_Trace.WriteLine(string.Format("Top2 Vision UDP Service server open Port:{0}", dataService.DataSystem.Top2HPort));
                if (0 != server.Open(dataService.DataSystem.Top2HPort))
                {
                    // Error 
                    Log_Trace.WriteLine("Top2 Vision UDP Service server open failed");
                }
            }
        }

        public override void Connect()
        {
            if (null != dataService)
            {
                Log_Trace.WriteLine(string.Format("Top2 Vision UDP Service client connect IP:{0}, Port:{1}", dataService.DataSystem.Top2IPAddress, dataService.DataSystem.Top2Port));

                if (0 != client.Connect(dataService.DataSystem.Top2IPAddress, dataService.DataSystem.Top2Port))
                {
                    // Error
                    Log_Trace.WriteLine("Top2 Vision UDP Service client connect failed");
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
                        seqService.SetLight(1, true);
                    else
                        seqService.SetLight(1, false);
                }
            }
        }

        protected override void AddResultValues(int index, string value1, string value2, string vision="TOP2")
        {
            // Top vision 의 index 번호 확인
            //index = dataService.DataResult.Top.ListRaw.Count;
            if (false == dataService.IsEndTop)
            {
                bool isEnd = false;

                if ("END" == value1)
                {
                    dataService.IndexEndTop = index;
                    isEnd = true;
                }

                if ("END" == value2)
                {
                    dataService.IndexEndTop = index;
                    isEnd = true;
                }

                if (true == isEnd)
                {
                    if (false == dataService.IsEndTop)
                    {
                        dataService.IsEndTop = true;

                        bool isRemove = false;

                        // Job End 시 바로 앞의 조인트를 모두 Job End 처리한다.
                        int indexEnd = dataService.IndexEndTop - 1;
                        for (int i = indexEnd; i > 0; --i)
                        {
                            if ("BB006" == dataService.DataResult.Top.ListRaw[i].Value1)
                            {
                                dataService.IndexEndTop = i-1;
                                isRemove = true;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (true == isRemove)
                        {
                            //System.Diagnostics.Debug.WriteLine("END TOP2 = {0}", dataService.IndexEndTop);
                            dataService.DataResult.Remove(dataService.IndexEndTop+1);       // Index 위치까지 삭제
                        }

                        return;
                    }
                }
            }

            if ((true == dataService.IsEndTop) && (index > dataService.IndexEndTop))
            {
                //System.Diagnostics.Debug.WriteLine("END TOP2 return = {0}/EndIndex{1}", index, dataService.IndexEndTop);
                return;
            }

            //System.Diagnostics.Debug.WriteLine(string.Format("Top2 ==> {0}", index));


            switch (dataService.DataSystem.JobType)
            {
                case 0:
                    dataService.DataResult.AddTop(index, value1, value2, vision);
                    break;
                case 1:     // Total
                    if (dataService.DataResult.Top.CountTotal >= dataService.DataSystem.TotalCount)
                    {
                        dataService.IndexEndTop = index;
                        dataService.IsEndTop = true;

                        topSerivce.SetScanStop();
                        top2Serivce.SetScanStop();

                        //System.Diagnostics.Debug.WriteLine(string.Format("Top2 End ==> {0} : {1}, {2}", index, value1, value2));
                    }
                    else
                    {
                        dataService.DataResult.AddTop(index, value1, value2, vision);

                        //System.Diagnostics.Debug.WriteLine(string.Format("Top2 Add ==> {0} : {1}, {2}", index, value1, value2));
                    }
                    break;
                case 2:     // Good
                    if (dataService.DataResult.Top.CountGood >= dataService.DataSystem.GoodCount)
                    {
                        dataService.IndexEndTop = index;
                        dataService.IsEndTop = true;

                        topSerivce.SetScanStop();
                        top2Serivce.SetScanStop();
                    }
                    else
                    {
                        dataService.DataResult.AddTop(index, value1, value2, vision);


                    }
                    break;
                default:
                    dataService.DataResult.AddTop(index, value1, value2, vision);
                    break;
            }

            //// Job End 시 바로 앞의 조인트를 모두 Job End 처리한다.
            //index = dataService.IndexEndTop - 1;
            //for (int i = index; i > 0; --i)
            //{
            //    if ("V" == dataService.DataResult.Top.ListRaw[i].Value1)
            //    {
            //        dataService.IndexEndTop = i;
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}
        }

        protected override void AddResultValues3(int index, string value1, string value2, string value3, string vision = "TOP2")
        {
            // Top vision 의 index 번호 확인
            //index = dataService.DataResult.Top.ListRaw.Count;
            if (false == dataService.IsEndTop)
            {
                bool isEnd = false;

                if ("END" == value1)
                {
                    dataService.IndexEndTop = index;
                    isEnd = true;
                }

                if ("END" == value2)
                {
                    dataService.IndexEndTop = index;
                    isEnd = true;
                }

                if ("END" == value3)
                {
                    dataService.IndexEndTop = index;
                    isEnd = true;
                }

                if (true == isEnd)
                {
                    if (false == dataService.IsEndTop)
                    {
                        dataService.IsEndTop = true;

                        bool isRemove = false;

                        // Job End 시 바로 앞의 조인트를 모두 Job End 처리한다.
                        int indexEnd = dataService.IndexEndTop - 1;
                        for (int i = indexEnd; i > 0; --i)
                        {
                            if ("BB006" == dataService.DataResult.Top.ListRaw[i].Value1)
                            {
                                dataService.IndexEndTop = i - 1;
                                isRemove = true;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (true == isRemove)
                        {
                            dataService.DataResult.Remove(dataService.IndexEndTop + 1);       // Index 위치까지 삭제
                        }

                        return;
                    }
                }
            }

            if ((true == dataService.IsEndTop) && (index > dataService.IndexEndTop))
            {
                return;
            }


            switch (dataService.DataSystem.JobType)
            {
                case 0:
                    dataService.DataResult.AddTop3(index, value1, value2, value3, vision);
                    break;
                case 1:     // Total
                    if (dataService.DataResult.Top.CountTotal >= dataService.DataSystem.TotalCount)
                    {
                        dataService.IndexEndTop = index;
                        dataService.IsEndTop = true;

                        topSerivce.SetScanStop();
                        top2Serivce.SetScanStop();
                    }
                    else
                    {
                        dataService.DataResult.AddTop3(index, value1, value2, value3, vision);
                    }
                    break;
                case 2:     // Good
                    if (dataService.DataResult.Top.CountGood >= dataService.DataSystem.GoodCount)
                    {
                        dataService.IndexEndTop = index;
                        dataService.IsEndTop = true;

                        topSerivce.SetScanStop();
                        top2Serivce.SetScanStop();
                    }
                    else
                    {
                        dataService.DataResult.AddTop3(index, value1, value2, value3, vision);


                    }
                    break;
                default:
                    dataService.DataResult.AddTop3(index, value1, value2, value3, vision);
                    break;
            }
        }

        protected override int CheckIndex(int index, int length, string[] messages)
        {
            if (index != dataService.DataResult.Top.ListRaw.Count)
            {
                if (8 < dataService.DataTempTop.ListTemp.Count)     // 4 -> 8
                {
                    sysService.State = SystemService.States.heavyAlarm;
                    msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.index_top2);

                    Log_Top.WriteLine("Index Error Top2 : list({0}), input({1})", dataService.DataResult.Top.ListRaw.Count, index);
                }
                else
                {
                    Log_Top.WriteLine("AddList temp Top2 : list({0}), input({1})", dataService.DataResult.Top.ListRaw.Count, index);
                    dataService.DataTempTop.Add(index, length, messages, "TOP2");
                }

                return -1;
            }

            return 0;
        }

        protected override int CheckIndex3(int index, int length, string[] messages)
        {
            if (index != dataService.DataResult.Top.ListRaw.Count)
            {
                if (8 < dataService.DataTempTop.ListTemp.Count)     // 4 -> 8
                {
                    sysService.State = SystemService.States.heavyAlarm;
                    msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.index_top2);

                    Log_Top.WriteLine("Index Error Top2 : list({0}), input({1})", dataService.DataResult.Top.ListRaw.Count, index);
                }
                else
                {
                    Log_Top.WriteLine("AddList temp Top2 : list({0}), input({1})", dataService.DataResult.Top.ListRaw.Count, index);
                    dataService.DataTempTop.Add3(index, length, messages, "TOP2");
                }

                return -1;
            }

            return 0;
        }

        protected override void AddTempValues()
        {
            if (0 < dataService.DataTempTop.ListTemp.Count)
            {
                bool isFound = false;
                int index = 0;

                for (int i = 0; i < dataService.DataTempTop.ListTemp.Count; ++i)
                {
                    if (dataService.DataResult.Top.ListRaw.Count == dataService.DataTempTop.ListTemp[i].Index)
                    {
                        isFound = true;
                        index = dataService.DataTempTop.ListTemp[i].Index;

                        Log_Top.WriteLine("AddTempValues Top2 : list({0}), input({1})", dataService.DataResult.Top.ListRaw.Count, index);

                        for (int k = 0; k < dataService.DataTempTop.ListTemp[i].ListRaw.Count; ++k)
                        {
                            AddResultValues(index + k, dataService.DataTempTop.ListTemp[i].ListRaw[k].Value1, dataService.DataTempTop.ListTemp[i].ListRaw[k].Value2, dataService.DataTempTop.ListTemp[i].ListRaw[k].Type);
                        }

                        dataService.DataTempTop.ListTemp[i].ListRaw.Clear();
                        dataService.DataTempTop.ListTemp.RemoveAt(i);

                        break;
                    }

                    if (true == isFound)
                        AddTempValues();
                }
            }
        }

        protected override void AddTempValues3()
        {
            if (0 < dataService.DataTempTop.ListTemp.Count)
            {
                bool isFound = false;
                int index = 0;

                for (int i = 0; i < dataService.DataTempTop.ListTemp.Count; ++i)
                {
                    if (dataService.DataResult.Top.ListRaw.Count == dataService.DataTempTop.ListTemp[i].Index)
                    {
                        isFound = true;
                        index = dataService.DataTempTop.ListTemp[i].Index;

                        Log_Top.WriteLine("AddTempValues Top2 : list({0}), input({1})", dataService.DataResult.Top.ListRaw.Count, index);

                        for (int k = 0; k < dataService.DataTempTop.ListTemp[i].ListRaw.Count; ++k)
                        {
                            AddResultValues3(index + k, dataService.DataTempTop.ListTemp[i].ListRaw[k].Value1, dataService.DataTempTop.ListTemp[i].ListRaw[k].Value2, dataService.DataTempTop.ListTemp[i].ListRaw[k].Value3, dataService.DataTempTop.ListTemp[i].ListRaw[k].Type);
                        }

                        dataService.DataTempTop.ListTemp[i].ListRaw.Clear();
                        dataService.DataTempTop.ListTemp.RemoveAt(i);

                        break;
                    }

                    if (true == isFound)
                        AddTempValues3();
                }
            }
        }

        protected override void AddLightValue(int index, int value)
        {
            dataService.DataResult.AddLightTop(index, value);
        }

        protected override void WriteLog(string log)
        {
            Log_TopVision2.WriteLine(log);
        }

        protected override void SetOverFrame()
        {
            sysService.State = SystemService.States.heavyAlarm;
            msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.overframe_top2);
        }

        protected override void ShowLineError()
        {
            sysService.State = SystemService.States.heavyAlarm;
            msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.line_top2);

            //Log_Top.WriteLine("Line Error Top2");
        }
    }
}
