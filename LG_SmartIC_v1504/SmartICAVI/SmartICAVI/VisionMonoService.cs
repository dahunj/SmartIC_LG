namespace SmartICAVI
{
    class VisionMonoService : VisionUtill
    {
        #region Singletons
        private static VisionMonoService singleton = null;

        public static VisionMonoService Singleton
        {
            get
            {
                if (null == singleton)
                {
                    singleton = new VisionMonoService();
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
            VisionType = "MONO1";

            if (null != dataService)
            {
                Log_Trace.WriteLine(string.Format("Mono Vision UDP Service server open Port:{0}", dataService.DataSystem.MonoHPort));
                if (0 != server.Open(dataService.DataSystem.MonoHPort))
                {
                    // Error 
                    Log_Trace.WriteLine("Mono Vision UDP Service server open failed");
                }
            }
        }

        public override void Connect()
        {
            if (null != dataService)
            {
                Log_Trace.WriteLine(string.Format("Mono Vision UDP Service client connect IP:{0}, Port:{1}", dataService.DataSystem.MonoIPAddress, dataService.DataSystem.MonoPort));

                if (0 != client.Connect(dataService.DataSystem.MonoIPAddress, dataService.DataSystem.MonoPort))
                {
                    // Error
                    Log_Trace.WriteLine("Mono Vision UDP Service client connect failed");
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
                        seqService.SetLight(4, true);
                    else
                        seqService.SetLight(4, false);
                }
            }
        }

        protected override void AddResultValues(int index, string value1, string value2, string vision="MONO1")
        {
            

            //// must have delete
            //value1 = "G";
            //value2 = "G";
            ////

            // Bottom vision 의 index 번호 확인
            //index = dataService.DataResult.Bottom.ListRaw.Count;


            if ((true == dataService.IsEndTop) && (index > dataService.IndexEndTop))
            {
                if (false == dataService.IsEndMono)
                {
                    dataService.IsEndMono = true;
                    monoSerivce.SetScanStop();
                    mono2Serivce.SetScanStop();
                }
                return;
            }

            if ("END" != value1 || "END" != value2)
            {
                dataService.DataResult.AddMono(index, value1, value2, vision);
            }
            //else
            //{
            //    if (false == dataService.IsEndMono)
            //    {
            //        dataService.IsEndMono = true;
            //        monoSerivce.SetScanStop();
            //        mono2Serivce.SetScanStop();
            //    }
            //}
        }

        protected override void AddResultValues3(int index, string value1, string value2, string value3, string vision = "MONO1")
        {
            // Bottom vision 의 index 번호 확인
            //index = dataService.DataResult.Bottom.ListRaw.Count;

            if ((true == dataService.IsEndTop) && (index > dataService.IndexEndTop))
            {
                if (false == dataService.IsEndMono)
                {
                    dataService.IsEndMono = true;
                    monoSerivce.SetScanStop();
                    mono2Serivce.SetScanStop();
                }
                return;
            }

            if ("END" != value1 || "END" != value2 || "END" != value3)
            {
                dataService.DataResult.AddMono3(index, value1, value2, value3, vision);
            }
        }

        protected override int CheckIndex(int index, int length, string[] messages)
        {
            if (index != dataService.DataResult.Mono.ListRaw.Count)
            {
                if (8 < dataService.DataTempMono.ListTemp.Count)     // 4 -> 8
                {
                    sysService.State = SystemService.States.heavyAlarm;
                    msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.index_mono);

                    Log_Mono.WriteLine("Index Error Mono1 : list({0}), input({1})", dataService.DataResult.Mono.ListRaw.Count, index);
                }
                else
                {
                    Log_Mono.WriteLine("AddList temp Mono1 : list({0}), input({1})", dataService.DataResult.Mono.ListRaw.Count, index);
                    dataService.DataTempMono.Add(index, length, messages, "MONO1");
                }

                return -1;
            }

            return 0;
        }

        protected override int CheckIndex3(int index, int length, string[] messages)
        {
            if (index != dataService.DataResult.Mono.ListRaw.Count)
            {
                if (8 < dataService.DataTempMono.ListTemp.Count)     // 4 -> 8
                {
                    sysService.State = SystemService.States.heavyAlarm;
                    msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.index_mono);

                    Log_Mono.WriteLine("Index Error Mono1 : list({0}), input({1})", dataService.DataResult.Mono.ListRaw.Count, index);
                }
                else
                {
                    Log_Mono.WriteLine("AddList temp Mono1 : list({0}), input({1})", dataService.DataResult.Mono.ListRaw.Count, index);
                    dataService.DataTempMono.Add3(index, length, messages, "MONO1");
                }

                return -1;
            }

            return 0;
        }

        protected override void AddTempValues()
        {
            if (0 < dataService.DataTempMono.ListTemp.Count)
            {
                bool isFound = false;
                int index = 0;

                for (int i = 0; i < dataService.DataTempMono.ListTemp.Count; ++i)
                {
                    if (dataService.DataResult.Mono.ListRaw.Count == dataService.DataTempMono.ListTemp[i].Index)
                    {
                        isFound = true;
                        index = dataService.DataTempMono.ListTemp[i].Index;

                        Log_Mono.WriteLine("AddTempValues Mono1 : list({0}), input({1})", dataService.DataResult.Mono.ListRaw.Count, index);

                        for (int k = 0; k < dataService.DataTempMono.ListTemp[i].ListRaw.Count; ++k)
                        {
                            AddResultValues(index + k, dataService.DataTempMono.ListTemp[i].ListRaw[k].Value1, dataService.DataTempMono.ListTemp[i].ListRaw[k].Value2, dataService.DataTempMono.ListTemp[i].ListRaw[k].Type);
                        }

                        dataService.DataTempMono.ListTemp[i].ListRaw.Clear();
                        dataService.DataTempMono.ListTemp.RemoveAt(i);

                        break;
                    }

                    if (true == isFound)
                        AddTempValues();
                }
            }
        }

        protected override void AddTempValues3()
        {
            if (0 < dataService.DataTempMono.ListTemp.Count)
            {
                bool isFound = false;
                int index = 0;

                for (int i = 0; i < dataService.DataTempMono.ListTemp.Count; ++i)
                {
                    if (dataService.DataResult.Mono.ListRaw.Count == dataService.DataTempMono.ListTemp[i].Index)
                    {
                        isFound = true;
                        index = dataService.DataTempMono.ListTemp[i].Index;

                        Log_Mono.WriteLine("AddTempValues Mono1 : list({0}), input({1})", dataService.DataResult.Mono.ListRaw.Count, index);

                        for (int k = 0; k < dataService.DataTempMono.ListTemp[i].ListRaw.Count; ++k)
                        {
                            AddResultValues3(index + k, dataService.DataTempMono.ListTemp[i].ListRaw[k].Value1, dataService.DataTempMono.ListTemp[i].ListRaw[k].Value2, dataService.DataTempMono.ListTemp[i].ListRaw[k].Value3, dataService.DataTempMono.ListTemp[i].ListRaw[k].Type);
                        }

                        dataService.DataTempMono.ListTemp[i].ListRaw.Clear();
                        dataService.DataTempMono.ListTemp.RemoveAt(i);

                        break;
                    }

                    if (true == isFound)
                        AddTempValues3();
                }
            }
        }

        protected override void AddLightValue(int index, int value)
        {
            dataService.DataResult.AddLightMono(index, value);
        }

        protected override void WriteLog(string log)
        {
            Log_MonoVision.WriteLine(log);
        }

        protected override void SetOverFrame()
        {
            sysService.State = SystemService.States.heavyAlarm;
            msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.overframe_mono);
        }

        protected override void ShowLineError()
        {
            sysService.State = SystemService.States.heavyAlarm;
            msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.line_mono);
        }
    }


}
