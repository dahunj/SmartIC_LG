using System;
using System.IO;

namespace SmartICAVI
{
    class SeqVirtual : SeqAuto
    {
        protected override int DancerControl(int axis, int channel, int flagRun, int flagReady, int flagRunning, double dir = 1.0, double pos = 150.0)
        {
            int step = 0;

            while (true == IsFlag(flagRun))
            {
                switch (step)
                {
                    case 0:
                        SetFlag(flagRunning);
                        step += 10;
                        break;

                    case 10:    // Start
                        SetFlag(flagReady);
                        step = 100;
                        break;
                    case 100:       // Run
                        break;
                    default:
                        ResetFlag(flagRun);
                        ResetFlag(flagReady);
                        ResetFlag(flagRunning);
                        return -1;
                }
            }

            ResetFlag(flagRun);
            ResetFlag(flagReady);
            ResetFlag(flagRunning);
            return 0;
        }

        protected override int SetBufferReference(double tolerance = 0.01)
        {
            int step = 0;
            int flagRun = (int)EnumSmartIC.SeqFlags.bufferRun;
            int flagReady = (int)EnumSmartIC.SeqFlags.bufferReady;

            ResetFlag(flagReady);

            while (true == IsFlag(flagRun))
            {
                switch (step)
                {
                    case 0:
                        step += 10;
                        break;

                    case 10:       // + 이동
                        step += 10;
                        break;
                    case 20:
                        step += 10;
                        break;
                    case 30:
                        step += 10;
                        break;
                    case 40:
                        step = 100;
                        break;

                    case 100:
                        step = 20000;
                        break;
                    case 20000:
                        ResetFlag(flagRun);
                        SetFlag(flagReady);
                        return 0;

                    default:
                        ResetFlag(flagRun);
                        ResetFlag(flagReady);

                        return -1;
                }
                System.Windows.Forms.Application.DoEvents();
            }

            ResetFlag(flagRun);
            ResetFlag(flagReady);

            return 0;
        }

        protected override int SetBufferInit()     // 버퍼의 초기위치 확인
        {
            int step = 0;

            while (SystemService.States.systemLock > sysService.State)
            {
                switch (step)
                {
                    case 0:
                        step += 10;
                        break;
                    case 10:            // Buffer Limit N 센서 확인
                        step = 100;     // Search Init
                        break;

                    case 100:       // + Move
                        step = 20000;
                        break;

                    case 20000:
                        seqService.IsBufferInitDone = true;
                        return 0;
                }
                System.Windows.Forms.Application.DoEvents();
            }
            return 0;
        }

        protected override int CheckSensor(bool isStop = true)
        {
            return 0;
        }

        protected override int CheckPunchUp(bool isStop = true)
        {
            return 0;
        }

        protected override int CheckLimit(bool isStop = true)
        {
            return 0;
        }

        protected override int SetInitSearch()
        {
            return 0;
        }

        protected override void VisionProcess()
        {
            
        }

        protected override void PunchProcess()
        {
            int step = 0;

            int indexStep = 20;
            int indexStart = 0;
            int indexEnd = indexStep;

            while (true == IsRun)
            {
                switch (step)
                {
                    case 0:
                        
                        step += 10;
                        break;
                    case 10:
                        //if (indexEnd < dataService.DataResult.Mono.ListRaw.Count)
                        if (indexEnd < dataService.DataResult.Review.ListRaw.Count)
                        {
                            for (int i = indexStart; i < indexEnd; ++i)
                            {
                                dataService.DataResult.AddTotal(i);
                            }
                            step += 10;
                        }
                        else
                        {
                            if (true == dataService.IsEndTop)
                            {
                                //if (dataService.IndexEndTop <= dataService.DataResult.Mono.ListRaw.Count)
                                if (dataService.IndexEndTop <= dataService.DataResult.Review.ListRaw.Count)
                                {
                                    //indexEnd = dataService.DataResult.Mono.ListRaw.Count;
                                    indexEnd = dataService.DataResult.Review.ListRaw.Count;

                                    for (int i = indexStart; i < indexEnd; ++i)
                                    {
                                        dataService.DataResult.AddTotal(i);
                                    }
                                    step = 100;
                                }
                            }
                        }
                        break;
                    case 20:
                        indexStart = indexEnd;
                        indexEnd += indexStep;
                        step += 10;
                        break;
                    case 30:
                        step = 0;
                        break;

                    case 100:
                        dataService.IsJobDone = true;
                        step = 20000;
                        break;

                    case 20000:
                        return;
                }

                System.Windows.Forms.Application.DoEvents();
            }
        }

        public override void RunProcess()
        {
            ReviewService reviewService = ReviewService.Singleton;
            JobWindowService jobWindowService = JobWindowService.Singleton;

            dataService.IsJobDone = false;

            reviewService.Start();

            dataService.DataResult.Clear();

            dataService.DataResult.IsStarted = true;
            dataService.DataResult.IsSelectedTop = dataService.DataSystem.IsSelectedTop;
            dataService.DataResult.IsSelectedBottom = dataService.DataSystem.IsSelectedBottom;
            dataService.DataResult.IsSelectedMono = dataService.DataSystem.IsSelectedMono;
            dataService.DataResult.IsSelectedReview = dataService.DataSystem.IsSelectedModify;
            dataService.DataResult.IsSelectedPunch = dataService.DataSystem.IsSelectedPunch;

            dataService.DataResult.RecipeName = dataService.DataSystem.RecipeName;
            dataService.DataResult.LotID = dataService.DataSystem.LotID;
            dataService.DataResult.PF = dataService.DataRecipe.PF;
            dataService.DataResult.Line = dataService.DataRecipe.Line;

            //dataService.DataResult.SectionMinUnits = dataService.DataSystem.SectionMinUnits;
            dataService.DataResult.SectionMinUnits = dataService.DataRecipe.SectionMinUnits;

            // Defect Data 
            dataService.DataDefect.Clear();

            int step = 0;

            while (true == IsRun)
            {
                System.Windows.Forms.Application.DoEvents();

                switch (step)
                {
                    case 0:
                        SetPunchAutoFeeding();
                        step += 10;
                        break;
                    case 10:
                        if (true == dataService.IsJobDone)
                        {
                            dataService.DataResult.TimeEnd = DateTime.Now;
                            step = 2000;
                        }
                        break;

                    case 2000:
                        // Review 정지
                        reviewService.Stop();
                        step += 10;
                        break;
                    case 2010:
                        CopyToServer();

                        jobWindowService.ShowReportWindow();
                        step += 10;
                        break;
                    case 2020:
                        if (true == jobWindowService.IsReportWindowClosed)
                        {
                            step += 10;
                        }
                        break;
                    case 2040:
                        if (0 == dataService.DataResult.Save(dataService.DataSystem.ReportPath))
                        {
                            try
                            {
                                File.Copy(dataService.LocalMapPathName, dataService.ServerMapPath + "\\MapData.csv");
                            }
                            catch (Exception exc)
                            {
                                Log_Exception.WriteLine("SeqAuto(CopyMapData Local to Server) : " + exc.Message);
                            }
                        }

                        step += 10;
                        break;
                    case 2050:
                        if (true == IsCopyToServer)
                        {
                            // Show Wait Copy Done
                            msgService.ShowMessage((int)EnumSmartIC.LightAlarms.waitCopyToServer);
                            dataService.CurrentStatus = "Defect 데이터 전송중...";
                            step += 10;
                        }
                        else
                            step = 2100;
                        break;
                    case 2060:
                        if (false == IsCopyToServer)
                        {
                            step += 10;
                        }
                        break;
                    case 2070:
                        msgService.HideMessage((int)EnumSmartIC.LightAlarms.waitCopyToServer);
                        step = 200;
                        break;

                    case 2100:
                        sysService.State = SystemService.States.stop;
                        step = 20000;
                        break;


                    case 20000:
                        dataService.CurrentStatus = "검사 완료";
                        sysService.State = SystemService.States.stop;
                        GC.Collect();
                        return;


                }
                                
            }
        }
    }
}
