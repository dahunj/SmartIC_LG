using System;
using System.IO;
using System.Threading;

using HalconDotNet;

namespace SmartICAVI
{
    class SeqAuto : SeqManual
    {
        protected Thread threadCopyToServer = null;

        private Thread threadJog = null;

        private double PunchPosMove { get; set; }
        private double PunchPosData { get; set; }

        private double VisionPosMove { get; set; }

        private int PunchDataStartUnit { get; set; }
        private int PunchDataEndIdx { get; set; }

        private bool IsReStarted { get; set; }

        protected bool IsCopyToServer { get; set; } // Defect 데이터 전송중 일 경우 True;

        public SeqAuto()
        {
            mode = (int)SystemService.Modes.auto;

            PunchPosMove = 0.0;
            PunchPosData = 0.0;

            VisionPosMove = 0.0;

            PunchDataStartUnit = 0;
            PunchDataEndIdx = 0;

            IsReStarted = false;
        }
        
        #region RUN
        public override void RunProcess()
        {
            if (null == mesService)
                return;
            if (null == mesService.Mes)
                return;

            #region InitVariables
            int step = 0;

            JobWindowService jobWindowService = JobWindowService.Singleton;
            ReviewService reviewService = ReviewService.Singleton;
            VisionCamService camService = VisionCamService.Singleton;

            ResetFlag((int)EnumSmartIC.SeqFlags.visionForward);
            ResetFlag((int)EnumSmartIC.SeqFlags.visionBackward);
            ResetFlag((int)EnumSmartIC.SeqFlags.punchForward);
            ResetFlag((int)EnumSmartIC.SeqFlags.punchBackward);

            double pos = 0.0;
            double feedMove = dataService.DataSystem.PunchStroke;
            double bufferLimitN = dataService.DataSystem.BufferLimitN;
            double bufferLimitP = dataService.DataSystem.BufferLimitP;

            //double offsetX = 0.0;
            //double offsetY = 0.0;

            HObject hImage;
            HOperatorSet.GenEmptyObj(out hImage);

            //int axisUncoiler = (int)EnumSmartIC.Axis.uncoilerReel;
            //int axisRecoiler = (int)EnumSmartIC.Axis.recoilerReel;

            int axisVisionFeed = (int)EnumSmartIC.Axis.visionFeed;
            int axisPunchFeed = (int)EnumSmartIC.Axis.punchFeed;
            //int axisBuffer = (int)EnumSmartIC.Axis.buffer;

            int axisTop = (int)EnumSmartIC.Axis.visionTop;
            int axisBottom = (int)EnumSmartIC.Axis.visionBottom;

            int axisPunchX = (int)EnumSmartIC.Axis.punchX;
            int axisPunchY = (int)EnumSmartIC.Axis.punchY;

            double vel = dataService.DataMotion[axisPunchFeed].VelMove;
            double accel = dataService.DataMotion[axisPunchFeed].AccelMove;

            double velVision = dataService.DataMotion[axisVisionFeed].VelMove;
            double accelVision = dataService.DataMotion[axisVisionFeed].AccelMove;

            double posTop = dataService.DataSystem.TopVisionZ;
            double velTop = dataService.DataMotion[axisTop].VelMove;
            double accelTop = dataService.DataMotion[axisTop].AccelMove;

            double posBottom = dataService.DataSystem.BottomVisionZ;
            double velBottom = dataService.DataMotion[axisBottom].VelMove;
            double accelBottom = dataService.DataMotion[axisBottom].AccelMove;

            double posAlignX = dataService.DataSystem.AlignVisionX;
            double velAlignX = dataService.DataMotion[axisPunchX].VelMove;
            double accelAlignX = dataService.DataMotion[axisPunchX].AccelMove;

            double posAlignY = dataService.DataRecipe.AlignY;
            double velAlignY = dataService.DataMotion[axisPunchY].VelMove;
            double accelAlignY = dataService.DataMotion[axisPunchY].AccelMove;

            //int oldStep = 0;

            double strokeLimit = dataService.DataRecipe.PunchStroke;
            int pf = dataService.DataRecipe.PF;
            int totalUnits = (int)strokeLimit / pf;

            double stroke = (double)totalUnits * 4.75;

            double punchOffsetX = dataService.DataSystem.PunchOffsetX;
            double punchOffsetY = dataService.DataSystem.PunchOffsetY;

            double punchDistance = dataService.DataSystem.PunchDistance + dataService.DataRecipe.OffsetInitX;

            //Log_Trace.WriteLine("punchDistance = PunchDistance + OffsetInitX : {0:0.000} = {1:0.000} + {2:0.000}",
            //    punchDistance, dataService.DataSystem.PunchDistance, dataService.DataRecipe.OffsetInitX);

            //int outPunchUp = (int)EnumSmartIC.Outports.punchUp;
            //int outPunchDown = (int)EnumSmartIC.Outports.punchDown;

            //int inPunchUp = (int)EnumSmartIC.Inports.punchUp;
            //int inPunchDown = (int)EnumSmartIC.Inports.punchDown;

            double scanDummyTop = dataService.DataSystem.ScanDummyTop;
            double scanDummyBottom = dataService.DataSystem.ScanDummyBottom;
            double scanDummyMono = dataService.DataSystem.ScanDummyMono;

            bool isLimitCheck = false;
            bool isReInspect = false;           // 재 검사 할 경우 true;

            bool isRecipeDone = false;
            #endregion

            #region InitIOs
            if (0 != CheckSensor())
            {
                sysService.State = SystemService.States.stop;
                return;
            }

            if (0 != CheckPunchUp())
            {
                sysService.State = SystemService.States.stop;
                return;
            }

            if (0 != CheckSwitch())
            {
                sysService.State = SystemService.States.stop;
                return;
            }

            // Punch Up
            dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
            dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);

            // Cooling On
            dioService.SetOutport((int)EnumSmartIC.Outports.topVisionCooling);
            dioService.SetOutport((int)EnumSmartIC.Outports.bottomVisionCooling);

            // Cleaner On
            if (dataService.DataSystem.IsSelectedCleanRoller)
                SetCleanRoller();
            else
                ResetCleanRoller();

            //Ionizer On
            dioService.SetOutport((int)EnumSmartIC.Outports.ionizer1On);
            dioService.SetOutport((int)EnumSmartIC.Outports.ionizer2On);
            dioService.SetOutport((int)EnumSmartIC.Outports.ionizer3On);
            dioService.SetOutport((int)EnumSmartIC.Outports.ionizer4On);
            dioService.SetOutport((int)EnumSmartIC.Outports.loaderIonizerBlow);
            dioService.SetOutport((int)EnumSmartIC.Outports.unloaderIonizerBlow);


            dataService.FeedVelocity = velVision;

            dataService.JobInitOffset = 0.0;
            dataService.IsJobDone = false;

            dataService.DataSystem.VerifyPath = "";


            sysService.Mode = SystemService.Modes.auto;

            dataService.DataResult.CNGLimit = dataService.DataRecipe.NGContinue;


            #endregion

            #region CheckData
            if (dataService.DataSystem.CamType != "EXTERN")
            {
                if (0 != grabService.Grab(out hImage))
                {
                    sysService.State = SystemService.States.stop;
                    dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                    msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.timeout_align);

                    if (null != hImage)
                        hImage.Dispose();
                    return;
                }
                if (null != hImage)
                    hImage.Dispose();
            }

            isReInspect = false;
            IsReStarted = false;
            // 이전 데이터가 남아 있을 경우 이어서 작업할 지 여부 확인.
            if ((dataService.DataSystem.LotID == dataService.DataResult.LotID) && (0 < dataService.DataResult.Total.ListRaw.Count))
            {
                if ((dataService.DataResult.IsStarted) && (false == dataService.DataResult.IsCompleted))
                {
                    // 이전 데이터가 남아있을 경우 LotID 가 같을 경우 재 검사 여부를 물어본다.
                    if (dataService.DataResult.LotID == dataService.DataSystem.LotID)
                    {
                        jobWindowService.ShowRecheckWindow((int)EnumSmartIC.LightAlarms.jobContinue);

                        while (false == jobWindowService.IsRecheckWindowClosed)
                        {
                            System.Windows.Forms.Application.DoEvents();
                        }

                        if (0 == jobWindowService.ResultRecheckWindow)
                        {
                            isReInspect = true;
                            IsReStarted = true;
                        }
                        else        // 2019.05.27 khs - 이어서 검사 취소시 한번더 물어보기
                        {
                            jobWindowService.ShowRecheckWindow((int)EnumSmartIC.LightAlarms.JobCancelSelect);

                            while (false == jobWindowService.IsRecheckWindowClosed)
                            {
                                System.Windows.Forms.Application.DoEvents();
                            }

                            if (0 == jobWindowService.ResultRecheckWindow)
                            {
                                isReInspect = true;
                                IsReStarted = true;
                            }
                        }       
                    }
                }
            }

            if (false == isReInspect)
            {
                // 재 검사가 아닐경우 데이터를 클리어 한다.
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

                dataService.DataResult.SectionMinUnits = dataService.DataRecipe.SectionMinUnits;

                // Defect Data 
                dataService.DataDefect.Clear();

                // Delete Punch Images
                try
                {
                    FileUtill.DeleteAllFiles(dataService.DataSystem.PunchPath);
                }
                catch (Exception exc)
                {
                    Log_Exception.WriteLine("SeqAuto.RunProcess().DeleteAllFiles() => ", exc.Message);
                }

                NextPunchInspect = 0;

                // 레이저 포인터 이송
                // 새로 시작 시 값을 위치값 초기화
                dataService.DataSystem.VisionLastPos = 0.0;
                dataService.DataSystem.FeedLastPos = 0.0;
                dataService.DataSystem.AlignXLastPos = 0.0;
                dataService.DataSystem.AlignYLastPos = 0.0;
                dataService.DataSystem.BufferLastPos = 0.0;
            }

            dataService.DataTempTop.Clear();
            dataService.DataTempBottom.Clear();
            dataService.DataTempMono.Clear();

            strobeService.Write((byte)dataService.DataRecipe.strobe1);
            #endregion

            int oldStep = -1;

            const int stepInit = 500;
            const int stepTrigger = 1000;
            const int stepScan = 1500;
            const int stepEnd = 2000;

            double triggerStart = 0.0;
            dataService.CurrentStatus = "검사 중";
            dataService.LocalMapPathName = "";
            dataService.ServerMapPath = "";

            dataService.IsBackFeeding = false;
            IsCopyToServer = false;

            dioService.ResetOutport(31);

            if (false == isReInspect)       // 이어서 검사일 경우 RestartProcess 에서 호출
                reviewService.Start();

            // Velocity 는 완공 후 조정할 수 있으므로, 시작시에 적용한다.
            dataService.DataDefect.Velocity = dataService.DataMotion[(int)EnumSmartIC.Axis.visionFeed].VelMove;

            while (IsRun)
            {
                // Pause 상태에서는 Sensor 와 Limit 를 검사하지 않는다. 

                if ((SystemService.States.pause != sysService.State))
                {
                    if (0 != CheckSensor())
                        return;
                    if (isLimitCheck)
                        if (0 != CheckLimit())
                            return;
                }

                if (oldStep != step)
                {
                    //System.Diagnostics.Debug.WriteLine("RunProcess step = {0}", step);
                    oldStep = step;
                }

                switch (step)
                {
                    // 0
                    #region Init
                    case 0:
                        step += 10;
                        break;
                    case 10:        // Recipe Update
                        dataService.CurrentStatus = "검사PC 모델 데이터 로딩 중...";

                        camService.SendLoad(dataService.DataSystem.RecipeName);

                        isRecipeDone = true;
                        if (dataService.DataSystem.IsSelectedTop)
                        {
                            topService.SetRecipe(dataService.DataSystem.RecipeName);
                            top2Service.SetRecipe(dataService.DataSystem.RecipeName);
                        }
                        if (dataService.DataSystem.IsSelectedBottom)
                        {
                            bottomService.SetRecipe(dataService.DataSystem.RecipeName);
                            bottom2Service.SetRecipe(dataService.DataSystem.RecipeName);
                        }
                        if (dataService.DataSystem.IsSelectedMono)
                        {
                            monoService.SetRecipe(dataService.DataSystem.RecipeName);
                            mono2Service.SetRecipe(dataService.DataSystem.RecipeName);
                        }

                        SetTimeout(20000);
                        step += 10;
                        break;
                    case 20:
                        isRecipeDone = true;

                        if (dataService.DataSystem.IsSelectedTop)
                        {
                            if (false == topService.IsFlag((int)EnumSmartIC.VisionFlags.recipe))
                            {
                                isRecipeDone = false;
                            }
                            if (false == top2Service.IsFlag((int)EnumSmartIC.VisionFlags.recipe))
                            {
                                isRecipeDone = false;
                            }
                        }
                        if (dataService.DataSystem.IsSelectedBottom)
                        {
                            if (false == bottomService.IsFlag((int)EnumSmartIC.VisionFlags.recipe))
                                isRecipeDone = false;
                            if (false == bottom2Service.IsFlag((int)EnumSmartIC.VisionFlags.recipe))
                                isRecipeDone = false;
                        }
                        if (dataService.DataSystem.IsSelectedMono)
                        {
                            if (false == monoService.IsFlag((int)EnumSmartIC.VisionFlags.recipe))
                                isRecipeDone = false;
                            if (false == mono2Service.IsFlag((int)EnumSmartIC.VisionFlags.recipe))
                                isRecipeDone = false;
                        }

                        if (isRecipeDone)
                        {
                            step += 10;
                            dataService.CurrentStatus = "검사PC 모델 데이터 로딩... 완료";
                        }
                        else
                        {
                            if (IsTimeout())
                            {
                                if (dataService.DataSystem.IsSelectedTop)
                                {
                                    if (false == topService.IsFlag((int)EnumSmartIC.VisionFlags.recipe))
                                        msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.noRecipe_top);
                                    if (false == top2Service.IsFlag((int)EnumSmartIC.VisionFlags.recipe))
                                        msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.noRecipe_top2);
                                }
                                if (dataService.DataSystem.IsSelectedBottom)
                                {
                                    if (false == bottomService.IsFlag((int)EnumSmartIC.VisionFlags.recipe))
                                        msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.noRecipe_bottom);
                                    if (false == bottom2Service.IsFlag((int)EnumSmartIC.VisionFlags.recipe))
                                        msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.noRecipe_bottom2);
                                }
                                if (dataService.DataSystem.IsSelectedMono)
                                {
                                    if (false == monoService.IsFlag((int)EnumSmartIC.VisionFlags.recipe))
                                        msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.noRecipe_mono);
                                    if (false == mono2Service.IsFlag((int)EnumSmartIC.VisionFlags.recipe))
                                        msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.noRecipe_mono2);
                                }

                                sysService.State = SystemService.States.stop;
                                dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);

                                dataService.CurrentStatus = "검사PC 모델 데이터 로딩... 실패";
                            }
                        }
                        break;
                        
                    case 30:
                        step += 10;
                        break;
                    case 40:
                        step += 10;
                        break;
                    case 50:
                        step = 100;
                        break;

                    case 100:       // 설비 초기위치 이동
                        step += 10;
                        break;

                    case 110:
                        // Uncoiler 가동
                        if (false == dataService.IsUncoilerReady)
                        {
                            SetSequence((int)EnumSmartIC.Sequences.uncoilerBuffer, 1);
                            step += 10;
                        }
                        break;

                    case 120:
                        // Recoiler 가동
                        if (false == dataService.IsRecoilerReady)
                        {
                            SetSequence((int)EnumSmartIC.Sequences.recoilerBuffer, 1);
                            step += 10;
                        }
                        break;

                    case 130:
                        // Uncoiler Ready Check
                        if (dataService.IsUncoilerReady)
                            step += 10;
                        break;

                    case 140:
                        if (true == dataService.IsRecoilerReady)
                            step += 10;
                        break;

                    case 150:
                        if (false == seqService.IsBufferInitDone)
                            step += 10;
                        else
                            step = 200;
                        break;

                    case 160:
                        // 버퍼 초기화
                        SetSequence((int)EnumSmartIC.Sequences.bufferInit, 1);
                        step += 10;
                        break;

                    case 170:
                        if (seqService.IsBufferInitDone)
                            step += 10;
                        break;

                    case 180:
                        step = 200;
                        break;

                    case 200:
                        // 버퍼 위치 이동
                        SetSequence((int)EnumSmartIC.Sequences.bufferReference, 1);
                        step += 10;
                        break;

                    case 210:
                        if (true == IsFlag((int)EnumSmartIC.SeqFlags.bufferReady))
                        {
                            isLimitCheck = true;
                            step += 10;
                        }
                        break;

                    case 220:
                        motionService.AMove(axisTop, posTop, velTop, accelTop);
                        motionService.AMove(axisBottom, posBottom, velBottom, accelBottom);
                        step += 10;
                        break;

                    case 230:
                        if (motionService.IsMotionDone(axisTop))
                            step += 10;
                        break;

                    case 240:
                        if (motionService.IsMotionDone(axisBottom))
                            step += 10;
                        break;

                    case 250:
                        step += 10;
                        break;

                    case 260:
                        if (false == isReInspect)
                            step = stepInit;
                        else
                            step += 10;
                        break;

                    case 270:
                        //Rescan
                        if (0 == RestartProcess())
                        {
                            reviewService.Start();
                            isReInspect = false;

                            step = stepScan;     // Scan
                        }
                        break;

                    #endregion

                    //  500
                    #region ShowInitWindow
                    case stepInit:
                        // Show Init Window
                        jobWindowService.ShowInitJobWindow();
                        step += 10;
                        break;
                    case stepInit + 10:
                        if (true == jobWindowService.IsInitJobWindowDone)
                        {
                            if (true == jobWindowService.InitJobResult)
                                step += 10;
                            else
                                sysService.State = SystemService.States.stop;
                        }
                        break;

                    case stepInit + 20:
                        dataService.CurrentStatus = "MES 확인...";

                        mesService.Mes.MachineID = dataService.DataSystem.MachineID;
                        mesService.Mes.ProcessType = dataService.DataSystem.ProcessID;
                        mesService.Mes.LotID = dataService.DataSystem.LotID;
                        mesService.Mes.OPID = dataService.DataSystem.UserID;
                        mesService.Mes.ToolID1 = dataService.DataSystem.ToolID;

                        // 2020.07.14 khs - MES 전달시 진행한 Lot에 사용된 펀치 카운트만 보내야 한다. (원호연 선임 요청사항)
                        dataService.DataSystem.MES_PunchCount = 0;
                        //mesService.Mes.ToolID1Count = dataService.DataSystem.PunchCount;
                        mesService.Mes.ToolID1Count = dataService.DataSystem.MES_PunchCount;

                        mesService.Mes.ToolID2 = "";
                        mesService.Mes.ToolID2Count = 0;

                        mesService.Send(MesService.commands.send_jobstart);

                        //SetTimeout(3000);    // 일단 0.5 초 이내에 응답을 확인 하고, 응답시간이 지났을 경우 레디 위치 이동 후 다시 확인한다.
                        
                        step += 10;
                        break;
                    


                    case stepInit + 30:
                        // Offset 과 Init Unit 갯수만큼 이동한다. 
                        //pos = dataService.JobInitOffset + (double)(dataService.JobInitUnit * dataService.DataRecipe.PF);// - (dataService.DataSystem.ScanTolerance + dataService.DataSystem.ImageTolerance);
                        //dataService.JobInitOffset 은 InitWindow 에서 이미 이동했으므로, 생략한다. --> 추후 자동으로 이동할 경우 InitWindow 에서 실행할 지 시퀀스에서 실행할 지 결정해야함. 
                        pos = (double)(dataService.JobInitUnit * dataService.DataRecipe.PF);

                        motionService.RMove(axisVisionFeed, pos, velVision, accelVision);
                        motionService.RMove(axisPunchFeed, pos, velVision, accelVision);

                        //CreateServerDir();

                        step += 10;
                        break;

                    

                    case stepInit + 40:
                        if (true == motionService.IsMotionDone(axisVisionFeed))
                        {
                            if (true == motionService.IsMotionDone(axisPunchFeed))
                            {
                                SetTimeout(100);
                                step += 10;
                            }
                        }
                        break;
                    
                    case stepInit + 50:
                        // Set Position
                        if (true == IsTimeout())
                        {
                            //Log_Trace.WriteLine("Current Pos (Vision) : {0:0.000}", motionService.GetCurrentPosition(axisVision));
                            //Log_Trace.WriteLine("Current Pos (Punch) : {0:0.000}", motionService.GetCurrentPosition(axisPunch));

                            motionService.SetPosition(axisVisionFeed, 0.0);

                            cntService.SetPosition(0, 0.0);
                            cntService.SetPosition(1, 0.0);
                            cntService.SetPosition(2, 0.0);
                            cntService.SetPosition(3, 0.0);

                            SetTimeout(100);
                            step += 10;
                        }
                        break;
                    case stepInit + 60:
                        // Buffer RePosition
                        if (true == IsTimeout())
                        {
                            ResetFlag((int)EnumSmartIC.SeqFlags.bufferReady);
                            SetSequence((int)EnumSmartIC.Sequences.bufferReference, 1, 0.01);
                            step += 10;
                        }
                        break;
                    case stepInit + 70:
                        if (true == IsFlag((int)EnumSmartIC.SeqFlags.bufferReady))
                        {
                            //Log_Trace.WriteLine("Current Pos (Buffer) : {0:0.000}", motionService.GetCurrentPosition((int)EnumSmartIC.Axis.buffer));

                            isLimitCheck = true;
                            step += 10;
                        }
                        break;
                    case stepInit + 80:
                        // Set Punch Feed Position
                        //Log_Trace.WriteLine("Current Pos (Vision) : {0:0.000}", motionService.GetCurrentPosition(axisVision));
                        //Log_Trace.WriteLine("Current Pos (Punch) : {0:0.000}", motionService.GetCurrentPosition(axisPunch));
                        //Log_Trace.WriteLine("Current Pos (Buffer) : {0:0.000}", motionService.GetCurrentPosition((int)EnumSmartIC.Axis.buffer));

                        motionService.SetPosition(axisVisionFeed, 0.0);
                        cntService.SetPosition(0, 0.0);
                        cntService.SetPosition(1, 0.0);
                        cntService.SetPosition(2, 0.0);
                        cntService.SetPosition(3, 0.0);

                        motionService.SetPosition(axisPunchFeed, -punchDistance);   // 펀치 시작유닛의 위치를 0.0 으로 맞추기위해 
                        SetTimeout(1000);
                        //Log_Trace.WriteLine("Current Pos (Vision) : {0:0.000}", motionService.GetCurrentPosition(axisVision));
                        //Log_Trace.WriteLine("Current Pos (Punch) : {0:0.000}", motionService.GetCurrentPosition(axisPunch));
                        step += 10;
                        break;

                    case stepInit + 90:
                        if (true == mesService.Mes.IsJobPermission)
                        {
                            if ("Y" == mesService.Mes.JobPermission)
                            {
                                dataService.CurrentStatus = "MES 확인...완료";
                                step += 10;
                            }
                            else
                            {
                                sysService.State = SystemService.States.stop;
                                msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.wrongLotID);

                                dataService.CurrentStatus = "MES 확인...실패";
                            }
                        }
                        else
                        {
                            if (true == IsTimeout())
                            {
                                sysService.State = SystemService.States.stop;
                                msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.mesReply);

                                dataService.CurrentStatus = "MES 확인...실패";
                            }
                        }
                        break;


                    case stepInit + 100:
                        // Move Back ( Vision Offset + Feeding Offset )
                        motionService.RMove(axisVisionFeed, -(dataService.DataSystem.ScanTolerance + scanDummyTop * 2.0), velVision, accelVision);
                        step += 10;
                        break;
                    case stepInit + 110:
                        if (true == motionService.IsMotionDone(axisVisionFeed))
                        {
                            SetTimeout(300);
                            step += 10;
                        }
                        break;
                    case stepInit + 120:
                        if (true == IsTimeout())
                        {
                            SetTimeout(100);
                            step = stepTrigger;
                        }
                        break;
                    
                    #endregion

                    // 1000
                    #region Trigger
                    case stepTrigger:
                        // Set Trigger Position
                        if (true == IsTimeout())
                        {
                            //// Trigger Setting
                            //if (true == dataService.DataSystem.IsSelectedVision)
                            //{
                            //    cntService.SetTriggerParams(0, -dataService.DataSystem.ImageTolerance - scanDummy, 1000000.0, 0.0095, 10.0);
                            //    cntService.SetTriggerParams(1, dataService.DataSystem.MonoDistance - dataService.DataSystem.ImageTolerance - scanDummy, 1000000.0, 0.00475, 5.0);
                            //    cntService.SetTriggerParams(2, dataService.DataSystem.BottomDistance - dataService.DataSystem.ImageTolerance - scanDummy, 1000000.0, 0.0095, 10.0);
                            //}

                            //if( true == dataService.DataSystem.IsSelectedTop )
                            //    cntService.SetTriggerParams(0, -dataService.DataSystem.ImageTolerance - scanDummy, 1000000.0, 0.0095, 10.0);
                            //if( true == dataService.DataSystem.IsSelectedBottom )
                            //    cntService.SetTriggerParams(2, dataService.DataSystem.BottomDistance - dataService.DataSystem.ImageTolerance - scanDummy, 1000000.0, 0.0095, 10.0);
                            //if( true == dataService.DataSystem.IsSelectedMono )
                            //    cntService.SetTriggerParams(1, dataService.DataSystem.MonoDistance - dataService.DataSystem.ImageTolerance - scanDummy, 1000000.0, 0.00475, 5.0);
                            triggerStart = -scanDummyTop;
                            //triggerEnd = 1000000.0;

                            //if (true == dataService.DataSystem.IsSelectedTop)
                            //    cntService.SetTriggerParams(0, -scanDummyTop - dataService.DataSystem.ImageTolerance, 1000000.0, dataService.DataSystem.TriggerColor_Period, dataService.DataSystem.TriggerColor_Width, dataService.DataSystem.TriggerColor_Level);
                            //if (true == dataService.DataSystem.IsSelectedBottom)
                            //    cntService.SetTriggerParams(2, dataService.DataSystem.BottomDistance - scanDummyBottom - dataService.DataSystem.ImageTolerance, 1000000.0, dataService.DataSystem.TriggerColor_Period, dataService.DataSystem.TriggerColor_Width, dataService.DataSystem.TriggerColor_Level);
                            //if (true == dataService.DataSystem.IsSelectedMono)
                            //    cntService.SetTriggerParams(1, dataService.DataSystem.MonoDistance - scanDummyMono - dataService.DataSystem.ImageTolerance, 1000000.0, dataService.DataSystem.TriggerMono_Period, dataService.DataSystem.TriggerMono_Width, dataService.DataSystem.TriggerMono_Level);

                            if (true == dataService.DataSystem.IsSelectedTop)
                                cntService.SetTriggerParams(0, triggerStart, 1000000.0, dataService.DataSystem.TriggerColor_Period, dataService.DataSystem.TriggerColor_Width, dataService.DataSystem.TriggerColor_Level);

                            triggerStart = dataService.DataSystem.BottomDistance - scanDummyBottom;
                            if (true == dataService.DataSystem.IsSelectedBottom)
                                cntService.SetTriggerParams(2, triggerStart, 1000000.0, dataService.DataSystem.TriggerColor_Period, dataService.DataSystem.TriggerColor_Width, dataService.DataSystem.TriggerColor_Level);

                            triggerStart = dataService.DataSystem.MonoDistance - scanDummyMono;
                            if (true == dataService.DataSystem.IsSelectedMono)
                                cntService.SetTriggerParams(1, triggerStart, 1000000.0, dataService.DataSystem.TriggerMono_Period, dataService.DataSystem.TriggerMono_Width, dataService.DataSystem.TriggerMono_Level);


                            step += 10;
                        }
                        break;
                    case stepTrigger + 10:
                        // Trigger Enable
                        //if (true == dataService.DataSystem.IsSelectedVision)
                        //{
                        //    cntService.SetTriggerEnable(0);
                        //    cntService.SetTriggerEnable(1);
                        //    cntService.SetTriggerEnable(2);

                        //    step += 10;
                        //}
                        if (true == dataService.DataSystem.IsSelectedTop)
                            cntService.SetTriggerEnable(0);
                        if (true == dataService.DataSystem.IsSelectedBottom)
                            cntService.SetTriggerEnable(2);
                        if (true == dataService.DataSystem.IsSelectedMono)
                            cntService.SetTriggerEnable(1);

                        step += 10;

                        break;
                    case stepTrigger + 20:
                        // Send Scan
                        if (true == dataService.DataSystem.IsSelectedTop)
                        {
                            topService.SetScanStart();
                            top2Service.SetScanStart();
                        }
                        if (true == dataService.DataSystem.IsSelectedBottom)
                        {
                            bottomService.SetScanStart();
                            bottom2Service.SetScanStart();
                        }
                        if (true == dataService.DataSystem.IsSelectedMono)
                        {
                            monoService.SetScanStart();
                            mono2Service.SetScanStart();
                        }

                        SetTimeout(10000);
                        step += 10;
                        break;
                    case stepTrigger + 30:
                        if (true == dataService.DataSystem.IsSelectedTop)
                        {
                            if (true == topService.IsReply((int)EnumSmartIC.VisionReplys.scan))
                            {
                                if (true == top2Service.IsReply((int)EnumSmartIC.VisionReplys.scan))
                                {
                                    step += 10;
                                }
                                else
                                {
                                    if (true == IsTimeout())
                                    {
                                        sysService.State = SystemService.States.stop;
                                        msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.timeout_ready_top2);
                                    }
                                }
                            }
                            else
                            {
                                if (true == IsTimeout())
                                {
                                    sysService.State = SystemService.States.stop;
                                    msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.timeout_ready_top);
                                }
                            }
                        }
                        else
                        {
                            step += 10;
                        }
                        break;
                    case stepTrigger + 40:
                        if (true == dataService.DataSystem.IsSelectedBottom)
                        {
                            if (true == bottomService.IsReply((int)EnumSmartIC.VisionReplys.scan))
                            {
                                if (true == bottom2Service.IsReply((int)EnumSmartIC.VisionReplys.scan))
                                    step += 10;
                                else
                                {
                                    if (true == IsTimeout())
                                    {
                                        sysService.State = SystemService.States.stop;
                                        msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.timeout_ready_bottom2);
                                    }
                                }
                            }
                            else
                            {
                                if (true == IsTimeout())
                                {
                                    sysService.State = SystemService.States.stop;
                                    msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.timeout_ready_bottom);
                                }
                            }
                        }
                        else
                        {
                            step += 10;
                        }
                        break;
                    case stepTrigger + 50:
                        if (true == dataService.DataSystem.IsSelectedMono)
                        {
                            if (true == monoService.IsReply((int)EnumSmartIC.VisionReplys.scan))
                            {
                                if (true == mono2Service.IsReply((int)EnumSmartIC.VisionReplys.scan))
                                {
                                    step += 10;
                                }
                                else
                                {
                                    if (true == IsTimeout())
                                    {
                                        sysService.State = SystemService.States.stop;
                                        msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.timeout_ready_mono2);
                                    }
                                }
                            }
                            else
                            {
                                if (true == IsTimeout())
                                {
                                    sysService.State = SystemService.States.stop;
                                    msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.timeout_ready_mono);
                                }
                            }
                        }
                        else
                        {
                            step += 10;
                        }
                        break;
                    case stepTrigger + 60:
                        SetTimeout(100);
                        step += 10;
                        break;
                    case stepTrigger + 70:
                        if (true == IsTimeout())
                            step = stepScan;
                        break;
                    #endregion

                    // 1500
                    #region Scan
                    case stepScan:
                        // 초기위치 찾기 부터 시작 시간으로 간주
                        //if (false == isReInspect)
                        //    dataService.DataResult.TimeStart = DateTime.Now;

                        dioService.SetOutport(31);
                        dataService.CurrentStatus = "검사 시작...";
                        step += 10;
                        break;
                    case stepScan + 10:
                        //posBuffer = motionService.GetCurrentPosition(axisBuffer);
                        SetVisionAutoFeeding();
                        SetPunchAutoFeeding();
                        step += 10;
                        break;
                    case stepScan + 20:
                        //// posBuffer 는 Vision/Punch Auto feeding 함수에서 공통으로 사용함. --> 각각의 쓰레드에서 읽어서 처리
                        //posBuffer = motionService.GetCurrentPosition(axisBuffer);

                        if (true == dataService.IsJobDone)
                        {
                            dataService.DataResult.TimeEnd = DateTime.Now;
                            step = stepEnd;
                        }
                        break;
                    #endregion

                    // 2000
                    #region END
                    case stepEnd:
                        cntService.ResetTriggerEnable(0);
                        cntService.ResetTriggerEnable(1);
                        cntService.ResetTriggerEnable(2);
                        cntService.ResetTriggerEnable(3);

                        //dioService.ResetOutport(30);
                        dioService.ResetOutport(31);


                        // Uncoiler 정지
                        SetSequence((int)EnumSmartIC.Sequences.uncoilerBuffer, 0);
                        // Recoiler 정지
                        SetSequence((int)EnumSmartIC.Sequences.recoilerBuffer, 0);

                        motionService.Stop(axisVisionFeed);
                        motionService.Stop(axisPunchFeed);

                        topService.SetScanStop();
                        top2Service.SetScanStop();
                        bottomService.SetScanStop();
                        bottom2Service.SetScanStop();
                        monoService.SetScanStop();
                        mono2Service.SetScanStop();

                        // Review 정지
                        reviewService.Stop();

                        motionService.AMove(axisTop, 0.0, 10.0, 400.0);
                        motionService.AMove(axisBottom, 0.0, 10.0, 400.0);



                        step += 10;
                        break;

                    case stepEnd + 10:
                        if (true == motionService.IsMotionDone(axisTop))
                        {
                            if (true == motionService.IsMotionDone(axisBottom))
                            {
                                // 2019.07.03 khs - LotEnd 직전에 사번 입력 받도록 입력창 추가 
                                msgService.ShowInput(0, false);
                                step += 5;
                            }
                        }
                        break;
                    case stepEnd + 15:
                        if (true == msgService.IsInputWindowClosed)     // 2019.07.03 khs - LotEnd 직전에 사번 입력 받도록 입력창 추가 
                        {
                            CopyToServer();
                            jobWindowService.ShowReportWindow();
                            step += 5;
                        }                     
                        break;
                    case stepEnd + 20:
                        if (true == jobWindowService.IsReportWindowClosed)
                        {
                            step += 10;
                        }
                        break;
                    case stepEnd + 30:
                        // MES 보고
                        dataService.CurrentStatus = "MES 보고";
                        mesService.Mes.MapData1 = dataService.DataResult.MapData1;
                        mesService.Mes.MapData2 = dataService.DataResult.MapData2;
                        mesService.Mes.MapData3 = dataService.DataResult.MapData3;

                        // 2020.07.14 khs - MES 전달시 진행한 Lot에 사용된 펀치 카운트만 보내야 한다. (원호연 선임 요청사항)
                        mesService.Mes.ToolID1Count = dataService.DataSystem.MES_PunchCount;

                        mesService.Mes.LotUnits = dataService.DataResult.Total.CountTotal;
                        mesService.Mes.GoodUnits = dataService.DataResult.Total.CountGood;
                        mesService.Mes.NGUnits = dataService.DataResult.Total.CountTotal - dataService.DataResult.Total.CountGood;
                        mesService.Mes.NextOper = dataService.DataResult.NextProcess = dataService.DataSystem.NextProcess;
                        mesService.Mes.Comment = dataService.DataSystem.Comment;

                        mesService.Mes.SetCountNGs(dataService.DataResult.CountNGs);


                        mesService.Send(MesService.commands.send_jobend);

                        dataService.DataResult.IsCompleted = true;
                        step += 10;
                        break;
                    case stepEnd + 40:
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
                    case stepEnd + 50:
                        if (true == IsCopyToServer)
                        {
                            // Show Wait Copy Done
                            msgService.ShowMessage((int)EnumSmartIC.LightAlarms.waitCopyToServer);
                            dataService.CurrentStatus = "Defect 데이터 전송중...";
                            step += 10;
                        }
                        else
                            step = stepEnd + 100;
                        break;
                    case stepEnd + 60:
                        if (false == IsCopyToServer)
                        {
                            step += 10;
                        }
                        break;
                    case stepEnd + 70:
                        msgService.HideMessage((int)EnumSmartIC.LightAlarms.waitCopyToServer);
                        step = stepEnd + 100;
                        break;

                    case stepEnd + 100:
                        sysService.State = SystemService.States.stop;
                        step = 20000;
                        break;


                    case 20000: 
                        // Punch Up
                        dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);

                        // Cleaner Off
                        ResetCleanRoller();

                        //Ionizer Off
                        dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer1On);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer2On);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer3On);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer4On);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.loaderIonizerBlow);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.unloaderIonizerBlow);

                        // Uncoiler 정지
                        SetSequence((int)EnumSmartIC.Sequences.uncoilerBuffer, 0);
                        // Recoiler 정지
                        SetSequence((int)EnumSmartIC.Sequences.recoilerBuffer, 0);

                        dataService.CurrentStatus = "검사 완료";


                        if (dataService.DataSystem.PunchCount >= dataService.DataSystem.PunchCountLimit)
                            msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.punchCountLimit);
                        
                        if( null != hImage)
                            hImage.Dispose();

                        GC.Collect();

                        return;

                    #endregion

                    default:
                        break;

                }
                System.Windows.Forms.Application.DoEvents();
                Thread.Sleep(1);
            }
            // Motion Stop
            motionService.Stop(axisVisionFeed);
            motionService.Stop(axisPunchFeed);

            // Punch Up
            dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
            dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);

            dioService.ResetOutport(31);
            
            // Cleaner Off
            ResetCleanRoller();

            //Ionizer Off
            dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer1On);
            dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer2On);
            dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer3On);
            dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer4On);
            dioService.ResetOutport((int)EnumSmartIC.Outports.loaderIonizerBlow);
            dioService.ResetOutport((int)EnumSmartIC.Outports.unloaderIonizerBlow);

            cntService.ResetTriggerEnable(0);
            cntService.ResetTriggerEnable(1);
            cntService.ResetTriggerEnable(2);
            cntService.ResetTriggerEnable(3);

            strobeService.Write(0);

            topService.SetScanStop();
            top2Service.SetScanStop();
            bottomService.SetScanStop();
            bottom2Service.SetScanStop();
            monoService.SetScanStop();
            mono2Service.SetScanStop();

            // Review 정지
            reviewService.Stop();

            // Uncoiler 정지
            SetSequence((int)EnumSmartIC.Sequences.uncoilerBuffer, 0);
            // Recoiler 정지
            SetSequence((int)EnumSmartIC.Sequences.recoilerBuffer, 0);

            dataService.CurrentStatus = "검사 중지";

            if (null != hImage)
                hImage.Dispose();

            GC.Collect();

        }
        #endregion

        protected override void VisionProcess()
        {
            Log_Trace.WriteLine("SeqAuto.VisionProcess()");

            int step = 0;
            int axisFeed = (int)EnumSmartIC.Axis.visionFeed;
            int axisBuffer = (int)EnumSmartIC.Axis.buffer;

            double vel = dataService.DataMotion[axisFeed].VelMove;
            double accel = dataService.DataMotion[axisFeed].AccelMove;

            double limitN = dataService.DataSystem.BufferLimitN;
            double limitP = dataService.DataSystem.BufferLimitP;

            double posCurrent = 0.0;
            double posStroke = 600.0;
            double posCheck = 300.0;
            VisionPosMove = 600.0;

            Log_Trace.WriteLine("LimitN = {0}", limitN);

            double middle = limitN + (limitP - limitN) * 0.5;

            double posBuffer = motionService.GetCurrentPosition(axisBuffer);

            DateTime start = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 500);
            DateTime timeout = start.Add(duration);

            dataService.IsVisionPaused = false;

            while (true == IsRun)
            {
                switch (step)
                {
                    case 0:
                        step += 10;
                        break;
                    case 10:
                        step += 10;
                        break;
                    case 20:
                        step += 10;
                        break;
                    case 30:
                        step = 100;
                        break;

                    case 100:
                        if (false == dataService.IsBackFeeding)        // BackFeeding 이 아닐 경우에만 진행
                            step += 10;
                        break;
                    case 110:
                        // Vision Feeding
                        //motionService.JogP(axisFeed, vel, accel);
                        posCurrent = motionService.GetCurrentPosition(axisFeed);
                        VisionPosMove = posCurrent + posStroke;
                        posCheck = posCurrent + posStroke * 0.5;

                        motionService.AMove(axisFeed, VisionPosMove, vel, accel);

                        dataService.IsVisionPaused = false;

                        step += 10;
                        break;
                    case 120:
                        if (SystemService.States.pause == sysService.State)
                        {
                            dataService.IsVisionPaused = true;
                            step = 200;
                        }
                        else    // Check Limit N
                        {
                            posBuffer = motionService.GetCurrentPosition(axisBuffer);

                            if (posBuffer < limitN)
                            {
                                Log_Trace.WriteLine("posBuffer ={0} < LimitN = {1}", posBuffer, limitN);

                                motionService.Stop(axisFeed);
                                step += 5;
                            }
                            else if (true == dataService.IsBackFeeding)
                            {
                                step = 100;
                            }
                            else
                            {
                                posCurrent = motionService.GetCurrentPosition(axisFeed);

                                if (posCurrent > posCheck)
                                {
                                    VisionPosMove += posStroke;
                                    posCheck += posStroke;
                                    motionService.SetOverridePos(axisFeed, VisionPosMove);
                                }
                            }
                        }
                        break;
                    case 125:
                        if (true == motionService.IsMotionDone(axisFeed))
                        {
                            start = DateTime.Now;
                            duration = new TimeSpan(0, 0, 0, 0, 100);
                            timeout = start.Add(duration);
                            step += 1;
                        }
                        break;
                    case 126:
                        if (timeout < DateTime.Now)
                            step += 1;
                        break;
                    case 127:
                        if (0 == SetBackFeeding())
                        {
                            if (false == dataService.IsEndTop)
                            {
                                double currentPos = motionService.GetCurrentPosition(axisFeed);
                                int topIndex = dataService.DataResult.Top.ListRaw.Count;
                                int botIndex = dataService.DataResult.Bottom.ListRaw.Count;
                                int monIndex = dataService.DataResult.Mono.ListRaw.Count;
                                double pf = (double)dataService.DataRecipe.PF;
                                double top_bot = dataService.DataSystem.BottomDistance;
                                double top_mon = dataService.DataSystem.MonoDistance;
                                double nextPosTop = (double)(topIndex + 1) * pf * 4.75;
                                double nextPosBot = (double)(botIndex + 1) * pf * 4.75;
                                double nextPosMon = (double)(monIndex + 1) * pf * 4.75;

                                Log_Trace.WriteLine("=== Vision Feeding Stop ===");
                                Log_Trace.WriteLine("Current Vision Feed Position = {0:0.000}", currentPos);
                                Log_Trace.WriteLine("Current pf = {0:0.000}", pf);
                                Log_Trace.WriteLine("Current Top Index = {0}", topIndex);
                                Log_Trace.WriteLine("Current Bot Index = {0}", botIndex);
                                Log_Trace.WriteLine("Current Mon Index = {0}", monIndex);

                                Log_Trace.WriteLine("BackPos Top = {0:0.000}", currentPos - nextPosTop);
                                Log_Trace.WriteLine("BackPos Bot = {0:0.000}", currentPos - nextPosBot - top_bot);
                                Log_Trace.WriteLine("BackPos Mon = {0:0.000}", currentPos - nextPosMon - top_mon);
                            }

                            step = 130;
                        }
                        break;
                    case 130:
                        // Wait middle Position
                        posBuffer = motionService.GetCurrentPosition(axisBuffer);

                        if (posBuffer > limitP)
                        {
                            if (false == dataService.IsEndTop)
                            {
                                double currentPos = motionService.GetCurrentPosition(axisFeed);
                                int topIndex = dataService.DataResult.Top.ListRaw.Count;
                                int botIndex = dataService.DataResult.Bottom.ListRaw.Count;
                                int monIndex = dataService.DataResult.Mono.ListRaw.Count;
                                double pf = (double)dataService.DataRecipe.PF;
                                double top_bot = dataService.DataSystem.BottomDistance;
                                double top_mon = dataService.DataSystem.MonoDistance;
                                double nextPosTop = (double)(topIndex + 1) * pf * 4.75;
                                double nextPosBot = (double)(botIndex + 1) * pf * 4.75;
                                double nextPosMon = (double)(monIndex + 1) * pf * 4.75;

                                Log_Trace.WriteLine("=== Vision Feeding Restart ===");
                                Log_Trace.WriteLine("Current Vision Feed Position = {0:0.000}", currentPos);
                                Log_Trace.WriteLine("Current pf = {0:0.000}", pf);
                                Log_Trace.WriteLine("Current Top Index = {0}", topIndex);
                                Log_Trace.WriteLine("Current Bot Index = {0}", botIndex);
                                Log_Trace.WriteLine("Current Mon Index = {0}", monIndex);

                                Log_Trace.WriteLine("BackPos Top = {0:0.000}", currentPos - nextPosTop);
                                Log_Trace.WriteLine("BackPos Bot = {0:0.000}", currentPos - nextPosBot - top_bot);
                                Log_Trace.WriteLine("BackPos Mon = {0:0.000}", currentPos - nextPosMon - top_mon);
                            }
                            step = 100;
                        }
                        break;

                    // Pause
                    case 200:
                        dataService.IsVisionPaused = true;
                        motionService.Stop(axisFeed);
                        step += 10;
                        break;
                    case 210:
                        if( true == motionService.IsMotionDone(axisFeed) )
                            step += 10;
                        break;
                    case 220:
                        if (SystemService.States.pause != sysService.State)
                        {
                            dataService.DataResult.SectionMinUnits = dataService.DataRecipe.SectionMinUnits;

                            

                            step = 100;
                        }
                        break;

                    default:
                        break;
                }

                System.Windows.Forms.Application.DoEvents();
                Thread.Sleep(1);
            }

            dataService.IsVisionPaused = false;

            motionService.Stop(axisFeed);
        }

        protected override void PunchProcess()
        {
            GC.Collect();

            #region InitVariables
            JobWindowService winJobService = JobWindowService.Singleton;

            int step = 0;
            int stepResume = 0;
            int axisFeed = (int)EnumSmartIC.Axis.punchFeed;
            int axisVision = (int)EnumSmartIC.Axis.visionFeed;
            int axisX = (int)EnumSmartIC.Axis.punchX;
            int axisY = (int)EnumSmartIC.Axis.punchY;
            int axisBuffer = (int)EnumSmartIC.Axis.buffer;
            int inspectResult = 0;


            bool isMissPrintStart = false;       // Miss Print 판정
            bool isMissPrintEnd = false;       // Miss Print 판정

            HObject hImage;
            HOperatorSet.GenEmptyObj(out hImage);

            double velPunch = dataService.DataMotion[axisFeed].VelMove;
            double accelPunch = dataService.DataMotion[axisFeed].AccelMove;

            double velX = dataService.DataMotion[axisX].VelMove;
            double accelX = dataService.DataMotion[axisX].AccelMove;

            double velY = dataService.DataMotion[axisY].VelMove;
            double accelY = dataService.DataMotion[axisY].AccelMove;

            double velVision = dataService.DataMotion[axisVision].VelMove;
            double accelVision = dataService.DataMotion[axisVision].AccelMove;

            double limitN = dataService.DataSystem.BufferLimitN;
            double limitP = dataService.DataSystem.BufferLimitP;

            double middle = limitN + (limitP - limitN) * 0.5;

            double posBuffer = motionService.GetCurrentPosition(axisBuffer);


            double strokeLimit = dataService.DataSystem.PunchStroke;
            int pf = dataService.DataRecipe.PF;
            double totalUnits = strokeLimit / ((double)pf * 4.75);
            int units = (int)totalUnits;

            Log_Trace.WriteLine("TotalUnits : {0:0.000} = {1:0.000}/({2:0.000} * 4.75)", totalUnits, strokeLimit, pf);

            double posStroke = (double)(units * pf) * 4.75;

            // 0번째....
            double posX = dataService.DataSystem.AlignVisionX + dataService.DataSystem.AlignPosOffsetX;
            double posY = dataService.DataSystem.AlignVisionY + dataService.DataSystem.AlignPosOffsetY;

            double offsetX = 0.0, offsetY = 0.0;


            if (false == IsReStarted)
            {
                PunchDataStartUnit = 0;
                PunchDataEndIdx = units;
            }

            int punchUnit = 0;
            int punchStartIdx = 0;
            int ipHole = 0;
            int indexSectionYield = 0;
            int sectionYieldUnit = 0;
            int sectionYieldStart = 0;
            int indexJoint = 0;
            bool punchEnable = false;

            bool isCheckPunchUp = false;
            bool isFindPunchData = false;
            bool isFindSectionYield = false;
            bool isTHolePunch = false;
            bool isJoint = false;
            bool isJointBottom = false;      // Bottom 조인트
            bool isDualError = false;        // Top, Bottom 동시 불량일 경우 
            bool isStopNG = false;
            bool isCheckDefect = false;     // 조인트, 연불, 쓰루홀 을 제외한 사용자 설정 Stop 메시지
            int indexCheckDefect = 0;         // isCheckDefect 시 Defect index

            bool alignAnyTime = dataService.DataSystem.IsSelectedAlwaysAlign;
            bool isAligned = false;

            bool isJobEnd = false;

            double vel = velVision;
            double accel = accelVision;

            bool isMotionMove = false;

            int[] m_nCountNGs = new int[dataService.DataDefectInfo.IDs.Length];
            for (int i = 0; i < dataService.DataDefectInfo.IDs.Length; i++)
                m_nCountNGs[i] = 0;

            bool isSetIndex = false;
            bool isHole = false;
            bool isCNGStart = false;
            bool isCNGEnd = false;
            bool isCNGSection = false;
            bool isCNGPuchSkip = false;

            bool isPunchAll = false;

            int cngStartUnit = 0;

            string imageName = "";
            string path = "";
            string line = "";

            const int stepCheckPos = 100;
            const int stepAddData = 200;
            const int stepSectionYield = 500;
            const int stepPunch = 1000;
            const int stepCheckData = 2000;
            const int stepJobEnd = 3000;
            const int stepRestart = 4000;
            const int stepPause = 5000;

            int oldStep = -1;
            int ret = 0;
            int countGrab = 0;

            dataService.DataRecipe.AlignOffsetX = 0.0;
            dataService.DataRecipe.AlignOffsetY = 0.0;

            #endregion

            bool bFirstIndexPause = true;
            if (true == IsRun)
            {
                motionService.AMove(axisX, posX, velX, accelX);
                motionService.AMove(axisY, posY, velY, accelY);
            }

            while (true == IsRun)
            {
                //
                if (true == isCheckPunchUp)
                {
                    if (0 != CheckPunchUp())
                        break;
                }

                if (oldStep != step)
                {
                    oldStep = step;
                }

                switch (step)
                {
                    // 0
                    #region stepINIT
                    case 0:
                        step += 10;
                        break;
                    case 10:
                        // Move to Init Position
                        isCheckPunchUp = true;

                        posBuffer = motionService.GetCurrentPosition(axisBuffer);

                        isMotionMove = false;
                        if (posBuffer < limitP)
                        {
                            vel = velVision;
                            accel = accelPunch;
                        }
                        else
                        {
                            vel = velPunch;
                            accel = accelPunch;
                        }

                        if (false == IsReStarted)
                        {
                            PunchPosMove = 0.0;          // 첫번째 유닛의 위치 (SeqAuto 에서 초기위치를 -punchDistance 로 설정함)
                            PunchPosData = PunchPosMove - posStroke;      // 데이터 확인 위치는 펀치 위치보다 1 Stroke 이전에서 확인한다. 
                        }

                        step += 10;
                        break;
                    case 20:
                        if (false == dataService.IsVisionPaused)
                        {
                            isMotionMove = true;
                            if (false == IsReStarted)
                            {
                                motionService.AMove(axisFeed, PunchPosMove, vel, accel);
                                //Log_Debug.WriteLine("punch steo 20 PunchPosData {0:0.000}", PunchPosData);
                            }
                            step = stepCheckPos;
                        }
                        break;
                    #endregion

                    //100
                    #region CheckPos    
                    case stepCheckPos:
                        if (motionService.GetCurrentPosition(axisFeed) > PunchPosData)
                        {
                            if (dataService.IsEndTop)
                            {
                                if (PunchDataEndIdx > dataService.IndexEndTop)
                                {
                                    PunchDataEndIdx = dataService.IndexEndTop + 1;
                                    isJobEnd = true;
                                }
                            }

                            // 데이터 존재유무 확인
                            if (true == CheckDataLength(PunchDataEndIdx))
                            {
                                step = stepAddData;
                            }
                            else
                            {
                                motionService.Stop(axisFeed);
                                isMotionMove = false;
                                step = stepCheckData;        // 데이터 입력 까지 대기
                            }
                        }
                        // END 확인
                        else
                        {
                            if (dataService.IsEndTop)
                            {
                                if (PunchDataEndIdx > dataService.IndexEndTop)
                                {
                                    PunchDataEndIdx = dataService.IndexEndTop + 1;
                                    isJobEnd = true;

                                    step += 10;
                                }
                            }
                        }
                        break;
                    case stepCheckPos + 10:
                        // 데이터 존재유무 확인
                        if (true == CheckDataLength(PunchDataEndIdx))
                        {
                            step = stepAddData;
                        }
                        break;
                    #endregion 

                    //200
                    #region AddData
                    case stepAddData:
                        isFindPunchData = false;
                        isFindSectionYield = false;
                        PunchPosMove = PunchDataEndIdx * dataService.DataRecipe.PF * 4.75;

                        for (int i = PunchDataStartUnit; i < PunchDataEndIdx; ++i)
                        {
                            bool bIsPunch = false;

                            // 2019.05.27 khs - 처음과 끝 유닛 펀칭기능
                            if (dataService.DataSystem.IsSelectedPunchFirstUnit)
                            {
                                //bIsPunch = (i == 0 || i == 1);
                                bIsPunch = (i == 0);
                            }
                            if (dataService.DataSystem.IsSelectedPunchLastUnit)
                            {
                                //bIsPunch = (i == dataService.IndexEndTop - 1 || i == dataService.IndexEndTop);
                                bIsPunch = (i == dataService.IndexEndTop);
                            }
                            if (dataService.DataSystem.IsSelectedPunchSetUnit)
                            {
                                bIsPunch = (i == dataService.DataSystem.PunchUnitIndex);
                            }
                            if (0 != dataService.DataResult.AddTotal(i))
                            {
                                bIsPunch = true;
                            }

                            // NG 데이터 발견시 NG 데이터 까지 이동한다.
                            if ( bIsPunch && dataService.DataSystem.IsSelectedPunch )
                            {
                                // Find Punch
                                isFindPunchData = true;
                                punchUnit = i;

                                PunchPosMove = i * dataService.DataRecipe.PF * 4.75;

                                break;
                            }

                            if (false == CheckSectionYield(i))
                                isFindSectionYield = true;
                        }

                        step += 10;

                        // 구간 수율 Check // NG 데이터가 있을 경우 Punching 중에 처리한다.
                        // 펀칭 데이터가 없을 경우
                        if (false == isFindPunchData)
                        {
                            if (true == isFindSectionYield)
                            {
                                step = stepSectionYield;
                            }
                        }
                        break;
                    case stepAddData + 10:
                        motionService.SetOverridePos(axisFeed, PunchPosMove);
                        step += 10;
                        break;
                    case stepAddData + 20:
                        if (true == isFindPunchData)
                        {
                            step += 10;
                        }
                        else
                        {
                            if (true == isJobEnd)
                                step = stepJobEnd;
                            else
                            {

                                PunchDataStartUnit = PunchDataEndIdx;
                                PunchDataEndIdx += units;

                                dataService.DataResult.IndexCurPunch = PunchDataStartUnit - 1;
                                if (0 > dataService.DataResult.IndexCurPunch)
                                    dataService.DataResult.IndexCurPunch = 0;

                                PunchPosData = PunchPosMove - posStroke;

                                step = stepCheckPos;
                            }
                        }
                        break;
                    case stepAddData + 30:
                        // 모션 완료 채크
                        if (true == motionService.IsMotionDone(axisFeed))
                        {
                            if (IsInPos(axisFeed, PunchPosMove, dataService.DataSystem.MotionTolerance * 2.0))
                            {
                                isMotionMove = false;

                                // yjs 20161215 첫번째 유닛 펀칭시 위치오차 문제로 500mSec Delay 기능 추가
                                SetTimeout(dataService.DataSystem.DelayFeeding);
                                step += 5;
                            }
                        }
                        break;
                    case stepAddData + 35:
                        if (IsTimeout())
                            step += 5;
                        break;
                    case stepAddData + 40:                      // 이 스텝에서 마지막 구간인지 판단할 수 있다???
                        PunchDataStartUnit = punchUnit;
                        PunchDataEndIdx = PunchDataStartUnit + units;

                        if (true == dataService.IsEndTop)       // Top 이 종료이고.
                        {
                            if (PunchDataEndIdx > dataService.IndexEndTop) // 아직 마지막 펀칭 인덱스가 갱신되지 않았으면.
                            {
                                PunchDataEndIdx = dataService.IndexEndTop + 1;
                                isJobEnd = true;
                            }
                        }
                        step += 10;
                        break;
                    case stepAddData + 50:
                        // 처음 펀치 위치는 위에서 입력되어 있음.
                        // 데이터 존재유무 확인
                        if (CheckDataLength(PunchDataEndIdx)) // 리뷰 데이터 카운터가 현재 index 보다 작으면 false 아니면 true
                        {
                            // 처음 펀치 위치에서 나머지 데이터 입력한다.
                            for (int i = PunchDataStartUnit + 1; i < PunchDataEndIdx; ++i)
                            {
                                dataService.DataResult.AddTotal(i);
                            }
                            step += 10;
                        }
                        break;
                    case stepAddData + 60:
                        step = stepPunch;
                        break;
                    #endregion

                    //500:
                    #region SectionYield
                    case stepSectionYield:
                        step += 10;
                        break;

                    case stepSectionYield + 10:
                        // 모션 완료 채크
                        if (true == motionService.IsMotionDone(axisFeed))
                        {
                            isFindSectionYield = false;
                            isMotionMove = false;

                            sectionYieldStart = PunchDataStartUnit;

                            step += 10;
                        }
                        break;

                    case stepSectionYield + 20:
                        isFindSectionYield = false;
                        for (int i = sectionYieldStart; i < PunchDataEndIdx; ++i)
                        {
                            if (false == CheckSectionYield(i))
                            {
                                indexSectionYield = i;
                                sectionYieldStart = i + 1;
                                isFindSectionYield = true;
                                break;
                            }
                        }
                        step += 10;
                        break;

                    case stepSectionYield + 30:
                        if (1 == SectionYieldProcess(indexSectionYield, PunchDataStartUnit))
                            step = stepRestart;
                        else
                            step += 10;
                        break;

                    case stepSectionYield + 40:
                        if (false == isFindSectionYield)
                            step = stepSectionYield + 100;
                        else
                            step = stepSectionYield + 20;
                        break;

                    case stepSectionYield + 100:
                        if (false == isJobEnd)
                        {
                            step += 10;
                        }
                        else
                        {
                            step = stepJobEnd;
                        }
                        break;

                    case stepSectionYield + 110:
                        if (false == dataService.IsVisionPaused)
                        {
                            isMotionMove = true;

                            PunchDataStartUnit = PunchDataEndIdx;
                            PunchDataEndIdx += units;

                            PunchPosMove = PunchDataEndIdx * dataService.DataRecipe.PF * 4.75;
                            PunchPosData = PunchPosMove - posStroke;

                            step = stepCheckPos;

                            motionService.AMove(axisFeed, PunchPosMove, vel, accel);

                            //0번째
                            posX = dataService.DataSystem.AlignVisionX + dataService.DataRecipe.AlignOffsetX + dataService.DataSystem.AlignPosOffsetX;// - 4.75 * (double)ipHole * dataService.DataRecipe.PF;
                            posY = dataService.DataSystem.AlignVisionY + dataService.DataRecipe.AlignOffsetY + dataService.DataSystem.AlignPosOffsetY;

                            motionService.AMove(axisX, posX, velX, accelX);
                            motionService.AMove(axisY, posY, velY, accelY);


                            step = stepCheckPos;

                            isAligned = false;
                        }
                        break;

                    #endregion

                    // 1000
                    #region stepPUNCH
                    case stepPunch:
                        step += 10;
                        break;

                    case stepPunch + 10:
                        if (true == motionService.IsMotionDone(axisFeed))
                        {
                            isMotionMove = false;

                            punchStartIdx = PunchDataStartUnit;

                            // 0번째 자동퍼즈......
                            if (PunchDataStartUnit == 0 && bFirstIndexPause == true)
                            {
                                Log_Debug.WriteLine("First Index zoro pause {0}", PunchDataStartUnit);
                                msgService.ShowMessage((int)EnumSmartIC.LightAlarms.zeroPosComp);
                                bFirstIndexPause = false;
                                sysService.State = SystemService.States.pause;
                                dataService.DataSystem.FirstIndexPause = true;
                                break;
                            }

                            step += 10;
                        }
                        break;

                    case stepPunch + 20:
                        punchEnable = false;
                        isSetIndex = false;
                        isHole = false;
                        isCNGStart = false;
                        isCNGEnd = false;
                        isFindSectionYield = false;
                        isTHolePunch = true;
                        isJoint = false;
                        isJointBottom = false;
                        isDualError = false;
                        isStopNG = false;
                        isCheckDefect = false;

                        isPunchAll = false;

                        isMissPrintEnd = false;
                        isMissPrintStart = false;

                        Log_Debug.WriteLine("punchStart = {0}    punchEnd = {1}", punchStartIdx, PunchDataEndIdx);

                        // 3열
                        #region 3열
                        if (3 == dataService.DataRecipe.Line)
                        {
                            for (int i = punchStartIdx; i < PunchDataEndIdx; ++i)
                            {
                                dataService.DataResult.IndexCurPunch = i;

                                // 2019.05.27 khs - 처음과 끝 유닛 펀칭기능
                                if (dataService.DataSystem.IsSelectedPunchFirstUnit)
                                {
                                    //if (i == 0 || i == 1)
                                    if (i == 0)
                                    {
                                        punchEnable = true;

                                        Log_Trace.WriteLine("punchIndex = {0} / {1}", i + 1, PunchDataEndIdx);

                                        punchUnit = i;
                                        punchStartIdx = punchUnit + 1;

                                        Log_Debug.WriteLine("punchIndex = {0} / {1}", i + 1, PunchDataEndIdx);

                                        isPunchAll = true;

                                        seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.punch);

                                        break;
                                    }
                                }

                                if (dataService.DataSystem.IsSelectedPunchLastUnit)
                                {
                                    //if (i == dataService.IndexEndTop - 1 || i == dataService.IndexEndTop)
                                    if (i == dataService.IndexEndTop)
                                    {
                                        punchEnable = true;

                                        Log_Trace.WriteLine("punchIndex = {0} / {1}", i + 1, PunchDataEndIdx);

                                        punchUnit = i;
                                        punchStartIdx = punchUnit + 1;

                                        Log_Debug.WriteLine("punchIndex = {0} / {1}", i + 1, PunchDataEndIdx);

                                        isPunchAll = true;                                        

                                        seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.punch);

                                        break;
                                    }
                                }

                                if (dataService.DataSystem.IsSelectedPunchSetUnit)
                                {
                                    if (i == dataService.DataSystem.PunchUnitIndex)
                                    {
                                        punchEnable = true;

                                        Log_Trace.WriteLine("punchIndex = {0} / {1}", i + 1, PunchDataEndIdx);

                                        punchUnit = i;
                                        punchStartIdx = punchUnit + 1;

                                        Log_Debug.WriteLine("punchIndex = {0} / {1}", i + 1, PunchDataEndIdx);

                                        isPunchAll = true;
                                        isSetIndex = true;

                                        seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.punch);

                                        break;
                                    }
                                }

                                if ("G" != dataService.DataResult.Total.ListRaw[i].Value1 || "G" != dataService.DataResult.Total.ListRaw[i].Value2 || "G" != dataService.DataResult.Total.ListRaw[i].Value3)
                                {
                                    punchEnable = true;

                                    Log_Trace.WriteLine("punchIndex = {0} / {1}", i + 1, PunchDataEndIdx);

                                    punchUnit = i;
                                    punchStartIdx = punchUnit + 1;

                                    // Joint 설정
                                    if ("BB006" == dataService.DataResult.Total.ListRaw[i].Value1 || "BB006" == dataService.DataResult.Total.ListRaw[i].Value2 || "BB006" == dataService.DataResult.Total.ListRaw[i].Value3)
                                    {
                                        if (i != indexJoint + 1)
                                        {
                                            isJoint = true;
                                            isStopNG = true;

                                            if ("BB006" == dataService.DataResult.Top.ListRaw[i].Value1 || "BB006" == dataService.DataResult.Top.ListRaw[i].Value2 || "BB006" == dataService.DataResult.Top.ListRaw[i].Value3)
                                            {
                                                isJointBottom = false;
                                            }
                                            else
                                            {
                                                isJointBottom = true;
                                            }
                                        }
                                        indexJoint = i;
                                    }

                                    // 쓰루홀 정지 설정
                                    if (true == dataService.DataSystem.IsSelectedHoleStop)
                                    {
                                        if ("BB039" == dataService.DataResult.Total.ListRaw[i].Value1 || "BB039" == dataService.DataResult.Total.ListRaw[i].Value2 || "BB039" == dataService.DataResult.Total.ListRaw[i].Value3)
                                        {
                                            isHole = true;
                                            isAligned = false;

                                            isStopNG = true;

                                            if      ("BB039" == dataService.DataResult.Total.ListRaw[i].Value1) line = "A";
                                            else if ("BB039" == dataService.DataResult.Total.ListRaw[i].Value2) line = "B";
                                            else                                                                line = "C";
                                        }
                                        else
                                        {
                                            isHole = false;
                                        }
                                    }

                                    // 연속불량 정지 설정.
                                    if (0 < dataService.DataRecipe.NGContinue)  // CNG Limit 가 0 일 경우에는 무시한다.
                                    {
                                        // Continuous NG 확인
                                        if (0 < dataService.DataResult.listCNG.Count)
                                        {
                                            for (int k = 0; k < dataService.DataResult.listCNG.Count; ++k)
                                            {
                                                if (punchUnit == dataService.DataResult.listCNG[k].X)
                                                {
                                                    isCNGStart = true;
                                                    isAligned = false;

                                                    isCNGSection = true;
                                                    cngStartUnit = punchUnit;

                                                    break;
                                                }
                                                else if (punchUnit == dataService.DataResult.listCNG[k].Y)
                                                {
                                                    isCNGEnd = true;
                                                    isAligned = false;

                                                    isCNGSection = false;

                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    // 구간수율 오류일 경우
                                    if (false == CheckSectionYield(i))
                                    {
                                        isFindSectionYield = true;
                                        punchStartIdx = i + 1;
                                        sectionYieldUnit = i;
                                    }

                                    // 노광편차 설정
                                    if (true == dataService.DataSystem.IsSelectedMissPrint)
                                    {
                                        if (0 < dataService.DataResult.listMissPrint.Count)
                                        {
                                            for (int i_miss = dataService.DataResult.listMissPrint.Count - 1; i_miss >= 0; --i_miss)
                                            {
                                                if (punchUnit > dataService.DataResult.listMissPrint[i_miss].Y)
                                                    break;

                                                if (punchUnit == dataService.DataResult.listMissPrint[i_miss].X)
                                                {
                                                    isMissPrintStart = true;
                                                    isAligned = false;
                                                    break;
                                                }
                                                else if (punchUnit == dataService.DataResult.listMissPrint[i_miss].Y)
                                                {
                                                    isMissPrintEnd = true;
                                                    isAligned = false;
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    if (false == isHole)
                                        seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.punch);
                                    else
                                        seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.tHole, line);
                                    
                                    // 일반 NG 일 경우 Top/Bottom 동시 오류인지 확인
                                    if (true == dataService.DataSystem.IsSelectedDualErrorStop)
                                    {
                                        if (false == isStopNG)
                                        {
                                            isDualError = false;

                                            // Joint 구간이 아니어야 한다.
                                            if ("BB006" != dataService.DataResult.Total.ListRaw[i].Value1 && "BB006" != dataService.DataResult.Total.ListRaw[i].Value2 && "BB006" != dataService.DataResult.Total.ListRaw[i].Value3)
                                            {
                                                // NG 홀이 아니어야 한다.
                                                if ("G" != dataService.DataResult.Top.ListRaw[i].Value1 && "G" != dataService.DataResult.Bottom.ListRaw[i].Value1)
                                                {
                                                    if ("C" != dataService.DataResult.Top.ListRaw[i].Value1 && "C" != dataService.DataResult.Bottom.ListRaw[i].Value1)
                                                        isDualError = true;
                                                }

                                                if ("G" != dataService.DataResult.Top.ListRaw[i].Value2 && "G" != dataService.DataResult.Bottom.ListRaw[i].Value2)
                                                {
                                                    if ("C" != dataService.DataResult.Top.ListRaw[i].Value2 && "C" != dataService.DataResult.Bottom.ListRaw[i].Value2)
                                                        isDualError = true;
                                                }

                                                if ("G" != dataService.DataResult.Top.ListRaw[i].Value3 && "G" != dataService.DataResult.Bottom.ListRaw[i].Value3)
                                                {
                                                    if ("C" != dataService.DataResult.Top.ListRaw[i].Value3 && "C" != dataService.DataResult.Bottom.ListRaw[i].Value3)
                                                        isDualError = true;
                                                }

                                                // 조인트 바로 옆 유닛이 아니어야 한다.
                                                if (true == isDualError)
                                                {
                                                    if ((0 < i) && (PunchDataEndIdx - 1 > i))
                                                    {
                                                        if ("BB006" == dataService.DataResult.Total.ListRaw[i + 1].Value1 || "BB006" == dataService.DataResult.Total.ListRaw[i + 1].Value2 || "BB006" == dataService.DataResult.Total.ListRaw[i + 1].Value3)
                                                            isDualError = false;
                                                        if ("BB006" == dataService.DataResult.Total.ListRaw[i - 1].Value1 || "BB006" == dataService.DataResult.Total.ListRaw[i - 1].Value2 || "BB006" == dataService.DataResult.Total.ListRaw[i - 1].Value3)
                                                            isDualError = false;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    // 메시지 출력이 아닌 오류일 경우 사용자 설정에 따라 정지 여부를 설정한다. 
                                    if (false == isStopNG)
                                    {
                                        for (int isub = 0; isub < dataService.DataDefectInfo.IDs.Length; ++isub)
                                        {
                                            if (dataService.DataDefectInfo.IDs[isub] == dataService.DataResult.Total.ListRaw[i].Value1)
                                            {
                                                m_nCountNGs[isub] += 1;

                                                if (true == dataService.DataDefectInfo.IsShowMessages[isub])
                                                {
                                                    if (m_nCountNGs[isub] % dataService.DataDefectInfo.Accumulates[isub] == 0)
                                                    {
                                                        isCheckDefect = true;
                                                        indexCheckDefect = isub;
                                                    }
                                                    break;
                                                }
                                            }
                                            if (dataService.DataDefectInfo.IDs[isub] == dataService.DataResult.Total.ListRaw[i].Value2)
                                            {
                                                m_nCountNGs[isub] += 1;

                                                if (true == dataService.DataDefectInfo.IsShowMessages[isub])
                                                {
                                                    if (m_nCountNGs[isub] % dataService.DataDefectInfo.Accumulates[isub] == 0)
                                                    {
                                                        isCheckDefect = true;
                                                        indexCheckDefect = isub;
                                                    }
                                                    break;
                                                }
                                            }
                                            if (dataService.DataDefectInfo.IDs[isub] == dataService.DataResult.Total.ListRaw[i].Value3)
                                            {
                                                m_nCountNGs[isub] += 1;

                                                if (true == dataService.DataDefectInfo.IsShowMessages[isub])
                                                {
                                                    if (m_nCountNGs[isub] % dataService.DataDefectInfo.Accumulates[isub] == 0)
                                                    {
                                                        isCheckDefect = true;
                                                        indexCheckDefect = isub;
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }
                                else
                                {
                                    // Good  유닛에서 구간수율 오류일 경우
                                    if (false == CheckSectionYield(i))
                                    {
                                        sectionYieldUnit = i;
                                        isFindSectionYield = true;

                                        break;
                                    }
                                }
                            }
                        }
                        #endregion
                        // 2열
                        #region 2열
                        else
                        {
                            for (int i = punchStartIdx; i < PunchDataEndIdx; ++i)
                            {
                                dataService.DataResult.IndexCurPunch = i;

                                // 2019.05.27 khs - 처음과 끝 유닛 펀칭기능
                                if (dataService.DataSystem.IsSelectedPunchFirstUnit)
                                {
                                    //if (i == 0 || i == 1)
                                    if (i == 0)
                                    {
                                        punchEnable = true;

                                        Log_Trace.WriteLine("punchIndex = {0} / {1}", i + 1, PunchDataEndIdx);

                                        punchUnit = i;
                                        punchStartIdx = punchUnit + 1;

                                        Log_Debug.WriteLine("punchIndex = {0} / {1}", i + 1, PunchDataEndIdx);

                                        isPunchAll = true;

                                        seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.punch);

                                        break;
                                    }
                                }

                                if (dataService.DataSystem.IsSelectedPunchLastUnit)
                                {
                                    //if (i == dataService.IndexEndTop - 1 || i == dataService.IndexEndTop)
                                    if (i == dataService.IndexEndTop)
                                    {
                                        punchEnable = true;

                                        Log_Trace.WriteLine("punchIndex = {0} / {1}", i + 1, PunchDataEndIdx);

                                        punchUnit = i;
                                        punchStartIdx = punchUnit + 1;

                                        Log_Debug.WriteLine("punchIndex = {0} / {1}", i + 1, PunchDataEndIdx);

                                        isPunchAll = true;

                                        seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.punch);

                                        break;
                                    }
                                }

                                if (dataService.DataSystem.IsSelectedPunchSetUnit)
                                {
                                    if (i == dataService.DataSystem.PunchUnitIndex)
                                    {
                                        punchEnable = true;

                                        Log_Trace.WriteLine("punchIndex = {0} / {1}", i + 1, PunchDataEndIdx);

                                        punchUnit = i;
                                        punchStartIdx = punchUnit + 1;

                                        Log_Debug.WriteLine("punchIndex = {0} / {1}", i + 1, PunchDataEndIdx);

                                        isPunchAll = true;
                                        isSetIndex = true;

                                        seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.punch);

                                        break;
                                    }
                                }

                                if ("G" != dataService.DataResult.Total.ListRaw[i].Value1 || "G" != dataService.DataResult.Total.ListRaw[i].Value2)
                                {
                                    punchEnable = true;

                                    Log_Trace.WriteLine("punchIndex = {0} / {1}", i + 1, PunchDataEndIdx);

                                    punchUnit = i;
                                    punchStartIdx = punchUnit + 1;

                                    Log_Debug.WriteLine("punchIndex = {0} / {1}", i + 1, PunchDataEndIdx);
                                    Log_Debug.WriteLine("Punch {0}, {1}, {2}", i, dataService.DataResult.Total.ListRaw[i].Value1, dataService.DataResult.Total.ListRaw[i].Value2);

                                    // Joint 설정
                                    if ("BB006" == dataService.DataResult.Total.ListRaw[i].Value1 || "BB006" == dataService.DataResult.Total.ListRaw[i].Value2)
                                    {
                                        if (i != indexJoint + 1)
                                        {
                                            isJoint = true;
                                            isStopNG = true;

                                            if ("BB006" == dataService.DataResult.Top.ListRaw[i].Value1 || "BB006" == dataService.DataResult.Top.ListRaw[i].Value2)
                                            {
                                                isJointBottom = false;
                                            }
                                            else
                                            {
                                                isJointBottom = true;
                                            }
                                        }
                                        indexJoint = i;
                                    }

                                    // 쓰루홀 정지 설정
                                    if (true == dataService.DataSystem.IsSelectedHoleStop) // 쓰루홀은 잘 메세지 띄운다.....
                                    {
                                        if ("BB039" == dataService.DataResult.Total.ListRaw[i].Value1 || "BB039" == dataService.DataResult.Total.ListRaw[i].Value2)
                                        {
                                            isHole = true;
                                            isAligned = false;

                                            isStopNG = true;

                                            if ("BB039" == dataService.DataResult.Total.ListRaw[i].Value1) line = "A";
                                            else                                                           line = "B";
                                        }
                                        else
                                        {
                                            isHole = false;
                                        }
                                    }

                                    // 연속불량 정지 설정.
                                    if (0 < dataService.DataRecipe.NGContinue)  // CNG Limit 가 0 일 경우에는 무시한다.
                                    {
                                        // Continuous NG 확인
                                        if (0 < dataService.DataResult.listCNG.Count)
                                        {
                                            for (int k = 0; k < dataService.DataResult.listCNG.Count; ++k)
                                            {
                                                if (punchUnit == dataService.DataResult.listCNG[k].X)
                                                {
                                                    isCNGStart = true;
                                                    isAligned = false;

                                                    isCNGSection = true;
                                                    cngStartUnit = punchUnit;

                                                    break;
                                                }
                                                else if (punchUnit == dataService.DataResult.listCNG[k].Y)
                                                {
                                                    isCNGEnd = true;
                                                    isAligned = false;

                                                    isCNGSection = false;

                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    // 구간수율 오류일 경우
                                    if (false == CheckSectionYield(i))
                                    {
                                        isFindSectionYield = true;
                                        punchStartIdx = i + 1;
                                        sectionYieldUnit = i;
                                    }

                                    // 노광편차 설정
                                    if (dataService.DataSystem.IsSelectedMissPrint)
                                    {
                                        if (0 < dataService.DataResult.listMissPrint.Count)
                                        {
                                            for (int i_miss = dataService.DataResult.listMissPrint.Count - 1; i_miss >= 0; --i_miss)
                                            {
                                                if (punchUnit > dataService.DataResult.listMissPrint[i_miss].Y)
                                                    break;

                                                if (punchUnit == dataService.DataResult.listMissPrint[i_miss].X)
                                                {
                                                    isMissPrintStart = true;
                                                    isAligned = false;
                                                    break;
                                                }
                                                else if (punchUnit == dataService.DataResult.listMissPrint[i_miss].Y)
                                                {
                                                    isMissPrintEnd = true;
                                                    isAligned = false;
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    if (false == isHole)
                                        seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.punch);
                                    else
                                        seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.tHole, line);

                                    // 일반 NG 일 경우 Top/Bottom 동시 오류인지 확인
                                    if (dataService.DataSystem.IsSelectedDualErrorStop)
                                    {
                                        if (false == isStopNG)
                                        {
                                            isDualError = false;

                                            // Joint 구간이 아니어야 한다.
                                            if ("BB006" != dataService.DataResult.Total.ListRaw[i].Value1 && "BB006" != dataService.DataResult.Total.ListRaw[i].Value2)
                                            {
                                                // NG 홀이 아니어야 한다.
                                                if ("G" != dataService.DataResult.Top.ListRaw[i].Value1 && "G" != dataService.DataResult.Bottom.ListRaw[i].Value1)
                                                {
                                                    if ("C" != dataService.DataResult.Top.ListRaw[i].Value1 && "C" != dataService.DataResult.Bottom.ListRaw[i].Value1)
                                                        isDualError = true;
                                                }

                                                if ("G" != dataService.DataResult.Top.ListRaw[i].Value2 && "G" != dataService.DataResult.Bottom.ListRaw[i].Value2)
                                                {
                                                    if ("C" != dataService.DataResult.Top.ListRaw[i].Value2 && "C" != dataService.DataResult.Bottom.ListRaw[i].Value2)
                                                        isDualError = true;
                                                }

                                                // 조인트 바로 옆 유닛이 아니어야 한다.
                                                if (true == isDualError)
                                                {
                                                    if ((0 < i) && (PunchDataEndIdx - 1 > i))
                                                    {
                                                        if ("BB006" == dataService.DataResult.Total.ListRaw[i + 1].Value1 || "BB006" == dataService.DataResult.Total.ListRaw[i + 1].Value2)
                                                            isDualError = false;
                                                        if ("BB006" == dataService.DataResult.Total.ListRaw[i - 1].Value1 || "BB006" == dataService.DataResult.Total.ListRaw[i - 1].Value2)
                                                            isDualError = false;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    // 메시지 출력이 아닌 오류일 경우 사용자 설정에 따라 정지 여부를 설정한다. 
                                    if (false == isStopNG)
                                    {
                                        for (int isub = 0; isub < dataService.DataDefectInfo.IDs.Length; ++isub)
                                        {
                                            if (dataService.DataDefectInfo.IDs[isub] == dataService.DataResult.Total.ListRaw[i].Value1)
                                            {
                                                m_nCountNGs[isub] += 1;

                                                if (true == dataService.DataDefectInfo.IsShowMessages[isub])
                                                {
                                                    if (m_nCountNGs[isub] % dataService.DataDefectInfo.Accumulates[isub] == 0)
                                                    {
                                                        isCheckDefect = true;
                                                        indexCheckDefect = isub;
                                                    }
                                                    break;
                                                }
                                            }
                                            if (dataService.DataDefectInfo.IDs[isub] == dataService.DataResult.Total.ListRaw[i].Value2)
                                            {
                                                m_nCountNGs[isub] += 1;

                                                if (true == dataService.DataDefectInfo.IsShowMessages[isub])
                                                {
                                                    if (m_nCountNGs[isub] % dataService.DataDefectInfo.Accumulates[isub] == 0)
                                                    {
                                                        isCheckDefect = true;
                                                        indexCheckDefect = isub;
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }
                                else
                                {
                                    // Good  유닛에서 구간수율 오류일 경우
                                    if (false == CheckSectionYield(i))
                                    {
                                        sectionYieldUnit = i;
                                        isFindSectionYield = true;

                                        break;
                                    }
                                }
                            }
                        }                        
                        #endregion

                        step += 10;

                        break;

                    case stepPunch + 30:
                        // Good 유닛에서 구간수율 오류가 발생했을 경우
                        if (false == punchEnable && isFindSectionYield)
                        {
                            if (1 == SectionYieldProcess(sectionYieldUnit, PunchDataStartUnit))
                                step = stepRestart;
                            else
                            {
                                punchStartIdx = sectionYieldUnit + 1;
                                step = stepPunch + 20;
                            }
                        }
                        else
                            step += 10;
                        break;

                    case stepPunch + 40:
                        // 얼라인 카메라 이동
                        if (punchEnable)
                        {
                            // IP Hole 검사위치 세팅
                            ipHole = punchUnit - PunchDataStartUnit;

                            if (true == alignAnyTime || false == isAligned)
                            {
                                // 0번째
                                posX = dataService.DataSystem.AlignVisionX + dataService.DataRecipe.AlignOffsetX - 4.75 * (double)ipHole * dataService.DataRecipe.PF + dataService.DataSystem.AlignPosOffsetX;
                                posY = dataService.DataSystem.AlignVisionY + dataService.DataRecipe.AlignOffsetY + dataService.DataSystem.AlignPosOffsetY;

                                motionService.AMove(axisX, posX, velX, accelX);
                                motionService.AMove(axisY, posY, velY, accelY);

                                if (null != hImage)
                                    hImage.Dispose();
                                GC.Collect();

                                countGrab = 0;
                            }
                        }
                        step += 5;
                        break;

                    case stepPunch + 45:
                        if (punchEnable)
                        {
                            if (motionService.IsMotionDone(axisX))
                            {
                                if (motionService.IsMotionDone(axisY))
                                {
                                    if (IsInPos(axisX, posX, dataService.DataSystem.MotionTolerance))
                                    {
                                        SetTimeout(10);
                                        step += 5;
                                    }
                                }
                            }
                        }
                        else
                        {
                            step += 5;
                        }
                        break;

                    case stepPunch + 50:
                        if (punchEnable)
                        {
                            if (motionService.IsMotionDone(axisX))
                            {
                                if (motionService.IsMotionDone(axisY))
                                {
                                    if (dataService.DataSystem.IsSelectedPunch)
                                    {
                                        if (alignAnyTime || false == isAligned)
                                        {
                                            if (IsTimeout())
                                            {
                                                if (0 == grabService.Grab(out hImage))
                                                {
                                                    if (null != hImage)
                                                        HOperatorSet.DispObj(hImage, hWindow);

                                                    imageName = string.Format("index{0:000000}_Align_A.jpg", punchUnit + 1);
                                                    path = dataService.DataSystem.PunchPath + "\\" + imageName;
                                                    
                                                    inspectResult = inspectSrvice.Inspect(hWindow, hImage, out offsetX, out offsetY, path);
                                                    isAligned = true;

                                                    dataService.DataDefect.AddAlign(punchUnit, "A", offsetX, offsetY, imageName);

                                                    Log_Trace.WriteLine("Align Inspect  : {0:0.000},  {1:0.000}", offsetX, offsetY);

                                                    // 검사 오류
                                                    if (0 != inspectResult)
                                                        step += 5;
                                                    else
                                                    {
                                                        step += 10;

                                                        if (true == dataService.DataSystem.IsSelectedAlignTolerance)
                                                        {
                                                            if (dataService.DataSystem.AlignTolerance < Math.Abs(offsetX))
                                                                step = stepPunch + 59;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (3 > ++countGrab)
                                                    {
                                                        Log_Trace.WriteLine("SeqAuto.PunchProcess() : AlignGrabError({0})", countGrab);

                                                        // for test
                                                        SetTimeout(200);
                                                    }
                                                    else
                                                    {
                                                        sysService.State = SystemService.States.stop;
                                                        dioService.SetOutport((int)EnumSmartIC.Outports.buzzer1);
                                                        msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.timeout_align);
                                                    }
                                                }
                                            }
                                        }
                                        else    // yjs 2017.11.25 bugfix
                                        {
                                            step += 10;
                                        }

                                    }
                                    else
                                    {
                                        step += 10;
                                    }
                                }
                            }
                        }
                        else
                        {
                            step += 10;
                        }
                        break;

                    case stepPunch + 55:
                        winJobService.ShowMessageWindow((int)EnumSmartIC.LightAlarms.alignError);
                        step += 1;
                        break;
                        
                    case stepPunch + 56:
                        if (winJobService.IsMessageWindowClosed)
                        {
                            step = stepPunch + 60;
                        }
                        break;

                    case stepPunch + 59:
                        ret = CheckAlignProcess();
                        if (1 == ret)
                            step = stepRestart;
                        else
                            step = stepPunch + 60;
                        break;

                    // 설정된 인덱스에 펀칭
                    case stepPunch + 60:
                        ret = 0;
                        if (isSetIndex)
                        {
                            ret = SetIndexProcess(dataService.DataSystem.PunchUnitIndex);
                            step = stepPunch + 200;
                        }
                        else
                        {
                            step += 10;
                        }
                        break;

                    // Hole 또는 CNG 일 경우 사용자 확인 후 펀칭 진행
                    case stepPunch + 70:
                        ret = 0;
                        if (isHole)
                        {
                            ret = HoleProcess(ref isTHolePunch, punchUnit);
                        }
                        else
                        {
                            // T-Hole Skip 일 경우 무조건 펀칭한다.
                            isTHolePunch = true;
                        }

                        if (1 == ret)
                            step = stepRestart;
                        else
                            step += 10;
                        break;

                    // CNG 확인 후 진행
                    case stepPunch + 80:
                        ret = 0;
                        if (isCNGStart || isCNGEnd)
                        {
                            // 3열
                            #region 3열
                            if (3 == dataService.DataRecipe.Line)
                            {
                                if (true == isCNGStart)
                                {
                                    if ("G" != dataService.DataResult.Total.ListRaw[punchUnit].Value1)
                                        seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.cngStart, "A");
                                    else if ("G" != dataService.DataResult.Total.ListRaw[punchUnit].Value2)
                                        seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.cngStart, "B");
                                    else
                                        seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.cngStart, "C");
                                }

                                else if (true == isCNGEnd)
                                {
                                    if ("G" != dataService.DataResult.Total.ListRaw[punchUnit].Value3)
                                        seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.cngEnd, "C");
                                    else if ("G" != dataService.DataResult.Total.ListRaw[punchUnit].Value2)
                                        seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.cngEnd, "B");
                                    else
                                        seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.cngEnd, "A");
                                }
                            }
                            #endregion
                            // 2열
                            else
                            {
                                if (true == isCNGStart)
                                {
                                    if ("G" != dataService.DataResult.Total.ListRaw[punchUnit].Value1)
                                        seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.cngStart, "A");
                                    else
                                        seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.cngStart, "B");
                                }

                                else if (true == isCNGEnd)
                                {
                                    if ("G" != dataService.DataResult.Total.ListRaw[punchUnit].Value2)
                                        seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.cngEnd, "B");
                                    else
                                        seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.cngEnd, "A");
                                }
                            }
                            ret = CNGProcess(punchUnit, isCNGStart);

                        }
                        else if ((punchUnit == dataService.DataResult.m_nStartCNG)
                            && (dataService.DataRecipe.NGContinue <= dataService.DataResult.m_nCountCNG))
                        {
                            // 너무 긴 불량일때 연속불량 구간 리스트에 저장 되기전에 인덱스가 넘어가는 것을 보완 

                            #region 3열
                            if (3 == dataService.DataRecipe.Line)
                            {
                                if ("G" != dataService.DataResult.Total.ListRaw[punchUnit].Value1)
                                    seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.cngStart, "A");
                                else if ("G" != dataService.DataResult.Total.ListRaw[punchUnit].Value2)
                                    seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.cngStart, "B");
                                else
                                    seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.cngStart, "C");

                            }
                            #endregion
                            // 2열
                            else
                            {
                                if ("G" != dataService.DataResult.Total.ListRaw[punchUnit].Value1)
                                    seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.cngStart, "A");
                                else
                                    seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.cngStart, "B");
                            }
                            ret = CNGProcess(punchUnit, true);
                        }

                        if (1 == ret)
                            step = stepRestart;
                        else
                            step += 10;
                        break;

                    // Joint 확인 후 진행
                    case stepPunch + 90:
                        ret = 0;
                        if (isJoint)
                        {
                            if (dataService.DataSystem.IsSelectedJointStop)
                                ret = JointProcess(punchUnit, isJointBottom);
                        }
                        step += 10;
                        break;

                    // 노광편차 확인
                    case stepPunch + 100:
                        ret = 0;
                        if (dataService.DataSystem.IsSelectedMissPrint)
                        {
                            if (isMissPrintStart || isMissPrintEnd)
                            {
                                // 3열
                                #region 3열
                                if (3 == dataService.DataRecipe.Line)
                                {
                                    if (true == isMissPrintStart)
                                    {
                                        if ("G" != dataService.DataResult.Total.ListRaw[punchUnit].Value1)
                                            seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.missPrintStart, "A");
                                        else if ("G" != dataService.DataResult.Total.ListRaw[punchUnit].Value2)
                                            seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.missPrintStart, "B");
                                        else
                                            seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.missPrintStart, "C");
                                    }

                                    else if (true == isMissPrintEnd)
                                    {
                                        if ("G" != dataService.DataResult.Total.ListRaw[punchUnit].Value3)
                                            seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.missPrintEnd, "C");
                                        else if ("G" != dataService.DataResult.Total.ListRaw[punchUnit].Value2)
                                            seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.missPrintEnd, "B");
                                        else
                                            seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.missPrintEnd, "A");
                                    }
                                }
                                #endregion 3열
                                // 2열
                                else
                                {
                                    if (isMissPrintStart)
                                    {
                                        if ("G" != dataService.DataResult.Total.ListRaw[punchUnit].Value1)
                                            seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.missPrintStart, "A");
                                        else
                                            seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.missPrintStart, "B");
                                    }
                                    else if (isMissPrintEnd)
                                    {
                                        if ("G" != dataService.DataResult.Total.ListRaw[punchUnit].Value2)
                                            seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.missPrintEnd, "B");
                                        else
                                            seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.missPrintEnd, "A");
                                    }
                                }

                                ret = MissPrintProcess(punchUnit, isMissPrintStart);
                            }
                        }
                        if (1 == ret)
                            step = stepRestart;
                        else
                            step += 10;
                        break;

                    // Dual Error (Top & Bottom) 확인
                    case stepPunch + 110:
                        ret = 0;
                        if (isDualError)
                        {
                            if (dataService.DataSystem.IsSelectedDualErrorStop)
                                ret = DualErrorProcess(punchUnit);
                        }
                        if (1 == ret)
                            step = stepRestart;
                        else
                            step += 10;
                        break;

                    // Check Defect
                    case stepPunch + 120:
                        if (isCheckDefect)
                        {
                            if (1 == CheckDefectProcess(punchUnit, indexCheckDefect))
                                step = stepRestart;
                            else
                                step = stepPunch + 200;
                        }
                        else
                        {
                            step = stepPunch + 200;
                        }
                        break;

                    case stepPunch + 200:
                        if (punchEnable)
                        {
                            isCNGPuchSkip = (isCNGSection) && (punchUnit > cngStartUnit);

                            if (dataService.DataSystem.IsSelectedCNGPunch)
                                isCNGPuchSkip = false;

                            if (false == isCNGPuchSkip)
                                SetPunch(offsetX, offsetY, punchUnit, ipHole, isTHolePunch, isPunchAll);
                        }
                        step += 10;
                        break;

                    case stepPunch + 210:
                        if (punchEnable)
                        {
                            if (false == isCNGPuchSkip)
                                SetPunchInspect(offsetX, offsetY, punchUnit, ipHole, isTHolePunch, isPunchAll);
                        }

                        seqService.FireEventPunch(punchUnit, (int)EnumSmartIC.PunchStates.punchDone);
                        step += 10;
                        break;

                    case stepPunch + 220:
                        // NG Punch 이후에 구간수율 검사.
                        ret = 0;
                        if (isFindSectionYield)
                        {
                            ret = SectionYieldProcess(sectionYieldUnit, PunchDataStartUnit);
                        }

                        if (1 == ret)
                            step = stepRestart;
                        else
                            step += 10;
                        break;

                    case stepPunch + 230:
                        if (punchEnable || isHole)
                        {
                            step = stepPunch + 20;
                        }
                        else
                        {
                            step += 10;
                        }
                        break;

                    case stepPunch + 240:
                        if (false == isJobEnd)
                        {
                            step += 10;
                        }
                        else
                        {
                            step = stepJobEnd;
                        }
                        break;

                    case stepPunch + 250:               // 한 구간의 펀칭 종료 및 새 구간 이동스텝.............
                        if (false == dataService.IsVisionPaused)
                        {
                            isMotionMove = true;

                            PunchDataStartUnit = PunchDataEndIdx;
                            PunchDataEndIdx += units;

                            PunchPosMove += posStroke;
                            PunchPosData = PunchPosMove - posStroke;

                            step = stepCheckPos;

                            motionService.AMove(axisFeed, PunchPosMove, vel, accel);

                            // 0번째
                            posX = dataService.DataSystem.AlignVisionX + dataService.DataRecipe.AlignOffsetX + dataService.DataSystem.AlignPosOffsetX;// - 4.75 * (double)ipHole * dataService.DataRecipe.PF;
                            posY = dataService.DataSystem.AlignVisionY + dataService.DataRecipe.AlignOffsetY + dataService.DataSystem.AlignPosOffsetY;

                            motionService.AMove(axisX, posX, velX, accelX);
                            motionService.AMove(axisY, posY, velY, accelY);


                            step = stepCheckPos;                                        // 체크 포지션으로 재 이동.....

                            isAligned = false;
                        }
                        break;

                    #endregion

                    //2000
                    #region stepCHECKDATA
                    case stepCheckData:     // CheckData
                        motionService.Stop(axisFeed);
                        isMotionMove = false;
                        step += 10;
                        break;
                    case stepCheckData + 10:
                        // Wait Move Done
                        if (true == motionService.IsMotionDone(axisFeed))
                            step += 10;
                        break;
                    case stepCheckData + 20:
                        if (false == dataService.IsVisionPaused)
                        {
                            // 데이터 존재유무 확인
                            if (true == CheckDataLength(PunchDataEndIdx))
                            {
                                motionService.AMove(axisFeed, PunchPosMove, vel, accel);
                                isMotionMove = true;

                                step = stepAddData;
                            }
                        }
                        break;
                    #endregion

                    //3000
                    #region stepJOBEND
                    case stepJobEnd:
                        // Job End
                        step += 10;
                        break;
                    case stepJobEnd + 10:
                        if (false == dataService.IsVisionPaused)
                        {
                            // 마지막 위치로 이동
                            PunchPosMove = PunchDataEndIdx * dataService.DataRecipe.PF * 4.75;

                            if (true == motionService.IsMotionDone(axisFeed))
                                motionService.AMove(axisFeed, PunchPosMove, vel, accel);
                            else
                                motionService.SetOverridePos(axisFeed, PunchPosMove);

                            step += 10;
                        }
                        break;
                    case stepJobEnd + 20:
                        if (true == motionService.IsMotionDone(axisFeed))
                            step += 10;
                        break;
                    case stepJobEnd + 30:
                        // IP Hole 검사위치 세팅 
                        ipHole = PunchDataEndIdx - PunchDataStartUnit;

                        System.Diagnostics.Debug.WriteLine(string.Format("end={0} - start={1} = ipHole = {2}", PunchDataEndIdx, PunchDataStartUnit, ipHole));

                        if (0 > ipHole)
                            ipHole = 0;

                        // 0번째
                        posX = dataService.DataSystem.AlignVisionX + dataService.DataRecipe.AlignOffsetX + dataService.DataSystem.AlignPosOffsetX;
                        posY = dataService.DataSystem.AlignVisionY + dataService.DataRecipe.AlignOffsetY + dataService.DataSystem.AlignPosOffsetY;

                        motionService.AMove(axisX, posX, velX, accelX);
                        motionService.AMove(axisY, posY, velY, accelY);
                        step += 10;
                        break;
                    case stepJobEnd + 40:
                        // Wait Move Done
                        if (true == motionService.IsMotionDone(axisX))
                        {
                            if (true == motionService.IsMotionDone(axisY))
                            {
                                step += 10;
                            }
                        }
                        break;
                    case stepJobEnd + 50:
                        dataService.IsJobDone = true;

                        if (null != hImage)
                            hImage.Dispose();
                        GC.Collect();
                        step += 10;
                        break;
                    case stepJobEnd + 60:
                        break;
                    #endregion

                    //4000 
                    #region stepRESTART    
                    case stepRestart:
                        dataService.IsBackFeeding = true;
                        step += 10;
                        break;

                    case stepRestart + 10:
                        if (0 == RestartProcess())
                        {
                            isCNGStart = false;
                            isCNGEnd = false;
                            isCNGSection = false;
                            isPunchAll = false;
                            step = stepCheckPos;
                        }
                        break;
                    #endregion

                    //5000
                    #region stepPAUSE
                    case stepPause: // Pause
                        // Pause
                        motionService.Stop(axisFeed);
                        step += 10;
                        break;
                    case stepPause + 10:
                        // Wait Move Done
                        if (true == motionService.IsMotionDone(axisFeed))
                        {
                            step += 10;

                            //dataService.DataSystem.punchPause = true;
                        }
                        break;
                    case stepPause + 20:
                        if ((SystemService.States.resume == sysService.State) || (SystemService.States.run == sysService.State))
                        {
                            // yjs 201612087 버퍼 위치가 LimitP 보다 클경우 LimitP 아래로 내려올 때 까지 대기한다.
                            //if (false == dataService.IsBackFeeding)     // backFeeding 이 아닐경우에만 진행
                            //{
                            //    if (true == isMotionMove)
                            //        motionService.AMove(axisFeed, PunchPosMove, vel, accel);
                            //}

                            //step = stepResume;

                            if (false == dataService.IsBackFeeding)     // backFeeding 이 아닐경우에만 진행
                            {
                                if (true == isMotionMove)
                                {
                                    //motionService.AMove(axisFeed, PunchPosMove, vel, accel);
                                    step += 10;
                                }
                                else
                                {
                                    step = stepResume;
                                }
                            }
                            else
                            {
                                step = stepResume;
                            }
                        }
                        break;
                    case stepPause + 30:
                        if (SystemService.States.pause == sysService.State)
                        {
                            step = stepPause;
                        }
                        else
                        {
                            posBuffer = motionService.GetCurrentPosition(axisBuffer);

                            if (posBuffer < limitP - 15.0)
                            {
                                if (false == dataService.IsVisionPaused)
                                {
                                    motionService.AMove(axisFeed, PunchPosMove, vel, accel);
                                    step = stepResume;
                                }
                            }
                        }
                        break;
                    #endregion

                    default:
                        break;
                }// switch

                // Pause 확인
                #region CHECKPAUSE

                if (SystemService.States.pause == sysService.State)
                {
                    if (step < stepPause)
                    {
                        stepResume = step;
                        step = stepPause;
                        motionService.Stop(axisFeed);
                        step = stepPause;
                    }
                }
                else
                {
                    // 속도 조절
                    if (true == isMotionMove)
                    {
                        posBuffer = motionService.GetCurrentPosition(axisBuffer);

                        // 급이송
                        if (posBuffer < limitP)
                        {
                            if (vel != velPunch)
                            {
                                vel = velPunch;
                                motionService.SetOverrideVel(axisFeed, vel);
                            }
                        }
                        else
                        {
                            if (vel != velVision)
                            {
                                vel = velVision;
                                motionService.SetOverrideVel(axisFeed, vel);
                            }
                        }

                    }
                }// Pause
                #endregion

                System.Windows.Forms.Application.DoEvents();
                Thread.Sleep(1);

            }// while

            motionService.Stop(axisFeed);

            SetSequence((int)EnumSmartIC.Sequences.recoilerBuffer, 0);
            SetSequence((int)EnumSmartIC.Sequences.uncoilerBuffer, 0);

            if (null != hImage)
                hImage.Dispose();

            GC.Collect();

        }

        protected bool CheckDataLength(int index)
        {
            if (dataService.DataResult.Review.ListRaw.Count < index)
                return false;

            return true;
        }

        protected bool CheckSectionYield(int index)
        {
            if (false == dataService.DataSystem.IsSelectedSectionYield)
                return true;

            int unit = (index + 1)*2;

            int section = 11;
            double yield = 0.0;

            int last = dataService.DataRecipe.sectionUnits.Length - 1;
            
            // 마지막 구간
            if (unit >= dataService.DataRecipe.sectionUnits[last])
            {
                section = last;
            }
            else
            {
                for (int i = 1; i <= last; ++i)
                {
                    if (unit < dataService.DataRecipe.sectionUnits[i])
                    {
                        section = i - 1;
                        break;
                    }
                }
            }

            yield = dataService.DataRecipe.sectionYields[section];

            if (yield > dataService.DataResult.Total.ListRaw[index].Yield)
            {
                return false;
            }

            return true;
        }

        protected int CreateServerDir()
        {
            return 0;
        }

        protected int CopyToServer()
        {
            ReviewService.Singleton.Stop();

            threadCopyToServer = new Thread(ThreadCopyToServer);
            threadCopyToServer.IsBackground = true;
            threadCopyToServer.Start();


            return 0;
        }

        protected void ThreadCopyToServer()
        {
            IsCopyToServer = true;

            dataService.CurrentStatus = "Defect 데이터 전송중...";

            dataService.DataDefect.RecipeName = dataService.DataSystem.RecipeName;
            dataService.DataDefect.LotID = dataService.DataSystem.LotID;
            dataService.DataDefect.UserID = dataService.DataSystem.UserID;
            dataService.DataDefect.ToolID = dataService.DataSystem.ToolID;
            dataService.DataDefect.NextProcess = dataService.DataSystem.NextProcess;
            dataService.DataDefect.Comment = dataService.DataSystem.Comment;

            dataService.DataDefect.PF = dataService.DataRecipe.PF;

            dataService.DataDefect.StartTime = dataService.DataResult.TimeStart;
            dataService.DataDefect.EndTime = dataService.DataResult.TimeEnd;
            dataService.DataDefect.TotalUnits = dataService.DataResult.Total.CountTotal;
            dataService.DataDefect.GoodUnits = dataService.DataResult.Total.CountGood;
            dataService.DataDefect.JointCount = dataService.DataResult.Total.CountJoint;
            dataService.DataDefect.CNGCount = dataService.DataResult.listCNG.Count;
            dataService.DataDefect.PunchCount = dataService.DataResult.CountPunch;
            dataService.DataDefect.PunchTotalCount = dataService.DataSystem.PunchCount;

            dataService.DataDefect.LightTimeTop = dataService.DataSystem.LightTimeTop;
            dataService.DataDefect.LightTimeBottom = dataService.DataSystem.LightTimeBottom;
            dataService.DataDefect.LightTimeMono = dataService.DataSystem.LightTimeMono;

            for (int i = 0; i < dataService.DataResult.CountNGs.Length; ++i)
                dataService.DataDefect.CountNGs[i] = dataService.DataResult.CountNGs[i];

            // Top Light
            for (int i = 0; i < dataService.DataResult.listLightTop.Count; ++i)
            {
                dataService.DataDefect.AddLightTop(dataService.DataResult.listLightTop[i].X, dataService.DataResult.listLightTop[i].Y);
            }

            // Bottom Light
            for (int i = 0; i < dataService.DataResult.listLightBottom.Count; ++i)
            {
                dataService.DataDefect.AddLightBottom(dataService.DataResult.listLightBottom[i].X, dataService.DataResult.listLightBottom[i].Y);
            }

            // Mono Light
            for (int i = 0; i < dataService.DataResult.listLightMono.Count; ++i)
            {
                dataService.DataDefect.AddLightMono(dataService.DataResult.listLightMono[i].X, dataService.DataResult.listLightMono[i].Y);
            }


            try
            {
                dataService.DataDefect.Save(dataService.DataSystem.ServerPath);
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine("SeqAuto.CopyToServer() => " + exc.Message);
            }

            dataService.CurrentStatus = "Defect 데이터 전송 완료";
            IsCopyToServer = false;

            System.Diagnostics.Debug.WriteLine(string.Format("Defect 데이터 전송 완료"));

        }

        protected int SetIndexProcess(int nIndex)
        {
            int step = 0;
            int oldStep = -1;
            int ret = 0;

            JobWindowService jobWindowService = JobWindowService.Singleton;

            while (true == IsRun)
            {
                if (oldStep != step)
                {
                    oldStep = step;
                }

                switch (step)
                {
                    case 0:
                        step += 10;
                        break;
                    case 10:
                        // Show Set Index Message
                        jobWindowService.ShowMessageWindow((int)EnumSmartIC.LightAlarms.setIndex, nIndex.ToString());
                        step += 10;
                        break;
                    case 20:
                        // Check Message Close
                        if (jobWindowService.IsMessageWindowClosed)
                            step += 10;
                        break;
                    case 30:
                        ret = jobWindowService.ResultMessageWindow;

                        if (-1 == ret)
                            sysService.State = SystemService.States.stop;

                        step += 10;
                        break;
                    case 40:
                        step = 20000;
                        break;

                    case 20000:
                        return ret;

                    default:
                        break;
                }

                System.Windows.Forms.Application.DoEvents();
            }

            return 0;
        }

        protected int HoleProcess(ref bool punchEnable, int punchUnit)
        {
            int step = 0;
            int oldStep = -1;
            int ret = 0;

            JobWindowService jobWindowService = JobWindowService.Singleton;

            punchEnable = true;

            while (true == IsRun)
            {
                if (oldStep != step)
                {
                    oldStep = step;
                }

                switch (step)
                {
                    case 0:
                        step += 10;
                        break;
                    case 10:
                        // Show Hole Message
                        jobWindowService.ShowMessageWindow((int)EnumSmartIC.LightAlarms.thole);
                        step += 10;
                        break;
                    case 20:
                        // Check Message Close
                        if (true == jobWindowService.IsMessageWindowClosed)
                            step += 10;
                        break;
                    case 30:
                        ret = jobWindowService.ResultMessageWindow;

                        if (-1 == ret)
                            sysService.State = SystemService.States.stop;
                        
                        step += 10;
                        break;
                    case 40:
                        step = 20000;
                        break;

                    case 20000:
                        return ret;

                    default:
                        break;
                }

                System.Windows.Forms.Application.DoEvents();
            }

            return 0;
        }

        protected int CNGProcess(int punchUnit, bool isStart)
        {
            int step = 0;
            int oldStep = -1;
            int ret = 0;

            JobWindowService jobWindowService = JobWindowService.Singleton;

            while (true == IsRun)
            {
                if (oldStep != step)
                {
                    oldStep = step;
                }

                switch (step)
                {
                    case 0:
                        step += 10;
                        break;

                    case 10:
                        // Show CNG Message
                        if(isStart)
                            jobWindowService.ShowMessageWindow((int)EnumSmartIC.LightAlarms.cngStart);
                        else
                            jobWindowService.ShowMessageWindow((int)EnumSmartIC.LightAlarms.cngEnd);
                        step += 10;
                        break;
                    case 20:
                        // Check Message Close
                        if( jobWindowService.IsMessageWindowClosed )
                            step += 10;
                        break;
                    case 30:
                        ret = jobWindowService.ResultMessageWindow;

                        if (-1 == ret)
                            sysService.State = SystemService.States.stop;
                        
                        step += 10;
                        break;
                    case 40:
                        step = 20000;
                        break;

                    case 20000:
                        return ret;

                    default:
                        break;
                }

                System.Windows.Forms.Application.DoEvents();
            }

            return -1;
        }

        protected int CheckAlignProcess()
        {
            int step = 0;
            int oldStep = -1;
            int ret = 0;

            JobWindowService jobWindowService = JobWindowService.Singleton;

            while (true == IsRun)
            {
                if (oldStep != step)
                {
                    oldStep = step;
                }

                switch (step)
                {
                    case 0:
                        step += 10;
                        break;

                    case 10:
                        // Show CNG Message
                        jobWindowService.ShowMessageWindow((int)EnumSmartIC.LightAlarms.alignTolerance);
                        step += 10;
                        break;
                    case 20:
                        // Check Message Close
                        if(jobWindowService.IsMessageWindowClosed )
                            step += 10;
                        break;
                    case 30:
                        ret = jobWindowService.ResultMessageWindow;

                        if (-1 == ret)
                            sysService.State = SystemService.States.stop;
                        
                        step += 10;
                        break;
                    case 40:
                        step = 20000;
                        break;

                    case 20000:
                        return ret;

                    default:
                        break;
                }

                System.Windows.Forms.Application.DoEvents();
            }

            return -1;
        }

        protected int MissPrintProcess(int punchUnit, bool isStart)
        {
            int step = 0;
            int oldStep = -1;
            int ret = 0;

            JobWindowService jobWindowService = JobWindowService.Singleton;

            while (true == IsRun)
            {
                if (oldStep != step)
                {
                    oldStep = step;
                }

                switch (step)
                {
                    case 0:
                        step += 10;
                        break;

                    case 10:
                        // Show CNG Message
                        if (true == isStart)
                            jobWindowService.ShowMessageWindow((int)EnumSmartIC.LightAlarms.missPrintStart);
                        else
                            jobWindowService.ShowMessageWindow((int)EnumSmartIC.LightAlarms.missPrintEnd);
                        step += 10;
                        break;
                    case 20:
                        // Check Message Close
                        if (true == jobWindowService.IsMessageWindowClosed)
                            step += 10;
                        break;
                    case 30:
                        ret = jobWindowService.ResultMessageWindow;

                        if (-1 == ret)
                            sysService.State = SystemService.States.stop;

                        step += 10;
                        break;
                    case 40:
                        step = 20000;
                        break;

                    case 20000:
                        return ret;

                    default:
                        break;
                }

                System.Windows.Forms.Application.DoEvents();
            }

            return -1;
        }

        protected int JointProcess(int punchUnit, bool isJointBottom = false)
        {
            int step = 0;
            int oldStep = -1;

            int ret = 0;

            JobWindowService jobWindowService = JobWindowService.Singleton;

            while (true == IsRun)
            {
                if (oldStep != step)
                {
                    oldStep = step;
                }

                switch (step)
                {
                    case 0:
                        step += 10;
                        break;

                    case 10:
                        // Show Joint Message
                        if( false == isJointBottom )
                            jobWindowService.ShowMessageWindow((int)EnumSmartIC.LightAlarms.joint);
                        else
                            jobWindowService.ShowMessageWindow((int)EnumSmartIC.LightAlarms.jointBottom);
                        step += 10;
                        break;
                    case 20:
                        // Check Message Close
                        if (true == jobWindowService.IsMessageWindowClosed)
                            step += 10;
                        break;
                    case 30:
                        ret = jobWindowService.ResultMessageWindow;
                        
                        if (-1 == ret)
                            sysService.State = SystemService.States.stop;

                        step += 10;
                        break;
                    case 40:
                        step = 20000;
                        break;

                    case 20000:
                        return ret;

                    default:
                        break;
                }

                System.Windows.Forms.Application.DoEvents();
            }

            return -1;
        }

        protected int DualErrorProcess(int punchUnit)
        {
            int step = 0;
            int oldStep = -1;

            int ret = 0;

            JobWindowService jobWindowService = JobWindowService.Singleton;

            while (true == IsRun)
            {
                if (oldStep != step)
                {
                    oldStep = step;
                }

                switch (step)
                {
                    case 0:
                        step += 10;
                        break;

                    case 10:
                        // Show Joint Message
                        jobWindowService.ShowMessageWindow((int)EnumSmartIC.LightAlarms.dualError);
                        step += 10;
                        break;
                    case 20:
                        // Check Message Close
                        if (true == jobWindowService.IsMessageWindowClosed)
                            step += 10;
                        break;
                    case 30:
                        ret = jobWindowService.ResultMessageWindow;

                        if (-1 == ret)
                            sysService.State = SystemService.States.stop;

                        step += 10;
                        break;
                    case 40:
                        step = 20000;
                        break;

                    case 20000:
                        return ret;

                    default:
                        break;
                }

                System.Windows.Forms.Application.DoEvents();
            }

            return -1;
        }

        protected int CheckDefectProcess(int punchUnit, int defectIndex)
        {
            int step = 0;
            int oldStep = -1;

            int ret = 0;

            JobWindowService jobWindowService = JobWindowService.Singleton;

            while (true == IsRun)
            {
                if (oldStep != step)
                {
                    oldStep = step;
                }

                switch (step)
                {
                    case 0:
                        step += 10;
                        break;

                    case 10:
                        // Show Joint Message
                        jobWindowService.ShowMessageWindow((int)EnumSmartIC.LightAlarms.checkDefect, dataService.DataDefectInfo.Names[defectIndex]);
                        step += 10;
                        break;
                    case 20:
                        // Check Message Close
                        if (true == jobWindowService.IsMessageWindowClosed)
                            step += 10;
                        break;
                    case 30:
                        ret = jobWindowService.ResultMessageWindow;

                        if (-1 == ret)
                            sysService.State = SystemService.States.stop;

                        step += 10;
                        break;
                    case 40:
                        step = 20000;
                        break;

                    case 20000:
                        return ret;

                    default:
                        break;
                }

                System.Windows.Forms.Application.DoEvents();
            }

            return -1;
        }

        protected int SectionYieldProcess(int indexSectionYield, int indexStart)
        {
            int step = 0;
            int oldStep = -1;
            int ipHole = 0;

            int ret = 0;

            int axisX = (int)EnumSmartIC.Axis.punchX;
            int axisY = (int)EnumSmartIC.Axis.punchY;

            double velX = dataService.DataMotion[axisX].VelMove;
            double accelX = dataService.DataMotion[axisX].AccelMove;

            double velY = dataService.DataMotion[axisY].VelMove;
            double accelY = dataService.DataMotion[axisY].AccelMove;

            double posX = dataService.DataSystem.AlignVisionX;
            double posY = dataService.DataSystem.AlignVisionY;

            JobWindowService jobWindowService = JobWindowService.Singleton;

            // Punch Up
            dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
            dioService.SetOutport((int)EnumSmartIC.Outports.punchDown);

            dataService.DataRecipe.AlignOffsetX = 0.0;
            dataService.DataRecipe.AlignOffsetY = 0.0;

            while (true == IsRun)
            {
                if (oldStep != step)
                {
                    //System.Diagnostics.Debug.WriteLine("SectionYieldProcess step = {0}", step);
                    oldStep = step;
                }

                CheckPunchUp();

                switch (step)
                {
                    case 0:
                        step += 10;
                        break;
                    case 10:
                        // IP Hole 검사위치 세팅
                        ipHole = indexSectionYield - indexStart;

                        //posX = dataService.DataSystem.AlignVisionX + dataService.DataRecipe.AlignOffsetX - 4.75 * (double)ipHole * dataService.DataRecipe.PF;
                        //posY = dataService.DataSystem.AlignVisionY + dataService.DataRecipe.AlignOffsetY;
                        // 0번째...
                        posX = dataService.DataSystem.AlignVisionX + dataService.DataRecipe.AlignOffsetX - 4.75 * (double)ipHole * dataService.DataRecipe.PF + dataService.DataSystem.AlignPosOffsetX;
                        posY = dataService.DataSystem.AlignVisionY + dataService.DataRecipe.AlignOffsetY + dataService.DataSystem.AlignPosOffsetY;

                        motionService.AMove(axisX, posX, velX, accelX);
                        motionService.AMove(axisY, posY, velY, accelY);

                        seqService.FireEventPunch(indexSectionYield, (int)EnumSmartIC.PunchStates.sectionYield);

                        step += 10;
                        break;
                    case 20:
                        if (true == motionService.IsMotionDone(axisX))
                        {
                            if (true == motionService.IsMotionDone(axisY))
                            {
                                step += 10;
                            }
                        }
                        break;

                    case 30:
                        jobWindowService.ShowMessageWindow((int)EnumSmartIC.LightAlarms.ngSection);
                        step += 10;
                        break;
                    case 40:
                        // Check Message Close
                        if (true == jobWindowService.IsMessageWindowClosed)
                            step += 10;
                        break;
                    case 50:
                        ret = jobWindowService.ResultMessageWindow;
                        // 정지
                        if (-1 == ret)
                        {
                            sysService.State = SystemService.States.stop;
                        }

                        seqService.FireEventPunch(indexSectionYield, (int)EnumSmartIC.PunchStates.sectionYieldDone);

                        step += 10;
                        break;
                    case 60:
                        step = 20000;
                        break;

                    case 20000:
                        return ret;

                    default:
                        break;
                }

                System.Windows.Forms.Application.DoEvents();
            }

            return -1;
        }

        protected int GrabErrorProcess(int punchUnit)
        {
            return 0;
        }

        protected int RestartProcess()
        {
            dataService.IsBackFeeding = true;

            ReviewService reviewService = ReviewService.Singleton;

            int step = 0;
            int oldStep = -1;

            double pos = 0.0;
            double posVision = 0.0;
            double posPunch = 0.0;
            double scanDummyTop = dataService.DataSystem.ScanDummyTop;
            double scanDummyBottom = dataService.DataSystem.ScanDummyBottom;
            double scanDummyMono = dataService.DataSystem.ScanDummyMono;
            double posDummy = dataService.DataSystem.ScanTolerance + scanDummyTop * 2.0;

            double strokeLimit = dataService.DataRecipe.PunchStroke;
            int pf = dataService.DataRecipe.PF;
            double totalUnits = strokeLimit / ((double)pf * 4.75);
            int units = (int)totalUnits;
            double posStroke = (double)(units * pf) * 4.75;

            int axisVision = (int)EnumSmartIC.Axis.visionFeed;
            int axisPunch = (int)EnumSmartIC.Axis.punchFeed;
            int axisX = (int)EnumSmartIC.Axis.punchX;
            int axisY = (int)EnumSmartIC.Axis.punchY;

            double vel = dataService.DataMotion[axisVision].VelMove;
            double accel = dataService.DataMotion[axisVision].AccelMove;

            double velX = dataService.DataMotion[axisX].VelMove;
            double accelX = dataService.DataMotion[axisX].AccelMove;

            double velY = dataService.DataMotion[axisY].VelMove;
            double accelY = dataService.DataMotion[axisY].AccelMove;

            double posX = dataService.DataSystem.AlignVisionX + dataService.DataSystem.AlignPosOffsetX;
            double posY = dataService.DataSystem.AlignVisionY + dataService.DataSystem.AlignPosOffsetY;

            int index = dataService.DataResult.IndexCurPunch;

            System.Diagnostics.Debug.WriteLine("Index Current Punch {0}", index);

            JobWindowService jobWindowService = JobWindowService.Singleton;

            // Punch Up
            dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
            dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);

            if (0 > index)
                index = 0;

            int maxCount = 0;

            maxCount = Math.Max(maxCount, dataService.DataResult.Top.ListRaw.Count);
            maxCount = Math.Max(maxCount, dataService.DataResult.Bottom.ListRaw.Count);
            maxCount = Math.Max(maxCount, dataService.DataResult.Mono.ListRaw.Count);

            maxCount += 100;

            while (true == IsRun)
            {
                if (oldStep != step)
                {
                    //System.Diagnostics.Debug.WriteLine("RestartProcess step = {0}", step);
                    oldStep = step;
                }

                if (0 != CheckPunchUp())
                    break;
                if (0 != CheckSensor())
                    break;
                if (0 != CheckLimit())
                    break;

                switch (step)
                {
                    case 0:
                        step += 10;

                        dataService.CurrentStatus = "재검사 (Back Feeding)";
                        break;

                    case 10:
                        // Motion Stop
                        motionService.Stop(axisVision);
                        motionService.Stop(axisPunch);

                        // Punch Up
                        dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);

                        // Scan Stop
                        topService.SetScanStop();
                        top2Service.SetScanStop();
                        bottomService.SetScanStop();
                        bottom2Service.SetScanStop();
                        monoService.SetScanStop();
                        mono2Service.SetScanStop();

                        // Trigger Off
                        cntService.ResetTriggerEnable(0);
                        cntService.ResetTriggerEnable(1);
                        cntService.ResetTriggerEnable(2);
                        cntService.ResetTriggerEnable(3);

                        step += 10;
                        break;

                    case 20:
                        if (true == motionService.IsMotionDone(axisVision))
                        {
                            if (true == motionService.IsMotionDone(axisPunch))
                            {
                                //step += 10;
                                step = 21;
                            }
                        }
                        break;

                    case 21:
                        Log_Debug.WriteLine("index {0} posVision = {1:0.000} posPunch = {2:0.000} ",
                            index, index * dataService.DataRecipe.PF * 4.75, motionService.GetCurrentPosition(axisVision) - posVision);
                        Log_Debug.WriteLine("laser index {0} laser posVision = {1:0.000} posPunch = {2:0.000}",
                            dataService.DataSystem.LaserIndex, dataService.DataSystem.VisionLastPos, dataService.DataSystem.FeedLastPos);
                        if (dataService.DataResult.IndexCurPunch > 0 && dataService.DataSystem.IsSelectedPunch == true)//IndexCurPunch 의 값은 검사 중 발견된 에러의 위치이다.
                        {
                            jobWindowService.ShowRecheckWindow((int)EnumSmartIC.LightAlarms.laserPosMove);
                            step = 22;
                        }
                        else
                        {
                            step = 30;
                        }
                        break;

                    case 22:
                        if (true == jobWindowService.IsRecheckWindowClosed)
                        {
                            step = 23;
                        }
                        break;

                    case 23:
                        if (0 == jobWindowService.ResultRecheckWindow)
                        {
                            step = 24;
                        }
                        else
                        {
                            step = 30;
                        }
                        break;

                    case 24: //  레이저 포지션 이동.....
                        seqService.SetSequence((int)EnumSmartIC.Sequences.laserMove, 0, 0.0);
                        step = 25;
                        break;
                    case 25:

                        jobWindowService.ShowRecheckWindow((int)EnumSmartIC.LightAlarms.laserMoveComp);
                        step = 26;
                        break;

                    case 26:
                        if (true == jobWindowService.IsRecheckWindowClosed)
                        {
                            step = 27;
                        }
                        break;

                    case 27:
                        if (0 == jobWindowService.ResultRecheckWindow)
                        {
                            dioService.ResetOutport((int)EnumSmartIC.Outports.laserOn);
                            // 백 피딩은 마지막 펀칭 인덱스로
                            index = dataService.DataSystem.LaserIndex;
                            step = 30;
                        }
                        else
                        {
                            dioService.ResetOutport((int)EnumSmartIC.Outports.laserOn);
                            sysService.State = SystemService.States.stop;
                        }
                        break;

                    case 28:
                        break;

                    case 29:
                        break;

                    case 30:
                        // 버퍼 위치 이동
                        SetSequence((int)EnumSmartIC.Sequences.bufferReference, 1, 10.0);
                        step += 10;
                        break;

                    case 40:
                        // Check Last Punch Position
                        pos = motionService.GetCurrentPosition(axisVision);

                        posVision = index * dataService.DataRecipe.PF * 4.75;
                        posPunch = pos - posVision;
                        // A Move
                        motionService.AMove(axisVision, (posVision - posDummy), dataService.DataMotion[axisVision].VelMove, dataService.DataMotion[axisVision].AccelMove);
                        motionService.RMove(axisPunch, -(posPunch - posDummy), dataService.DataMotion[axisVision].VelMove, dataService.DataMotion[axisVision].AccelMove);

                        motionService.AMove(axisX, posX, velX, accelX);
                        motionService.AMove(axisY, posY, velY, accelY);

                        step += 10;
                        break;

                    case 50:
                        // Delete Data
                        dataService.DataResult.Restart(index);
                        dataService.DataDefect.Restart(index);
                        dataService.DataTempTop.Clear();
                        dataService.DataTempBottom.Clear();
                        dataService.DataTempMono.Clear();

                        dataService.IsEndTop = false;
                        dataService.IsEndBottom = false;
                        dataService.IsEndMono = false;

                        reviewService.Restart(index);

                        dataService.DeleteReviewData(index, maxCount);

                        step += 10;
                        break;

                    case 60:
                        if (true == motionService.IsMotionDone(axisVision))
                        {
                            //Log_Debug.WriteLine("RESTART STEP 60 AXIS VISION DONE");
                            if (true == motionService.IsMotionDone(axisPunch))
                            {
                                //Log_Debug.WriteLine("RESTART STEP 60 AXIS PUNCH DONE MOVE STEP 70");
                                step += 10;
                            }
                        }
                        break;

                    case 70:
                        jobWindowService.ShowRecheckWindow((int)EnumSmartIC.LightAlarms.reStart);
                        step += 10;
                        break;

                    case 80:
                        if (jobWindowService.IsRecheckWindowClosed)
                            step += 10;
                        break;

                    case 90:
                        if (0 == jobWindowService.ResultRecheckWindow)
                            step += 10;
                        else
                            sysService.State = SystemService.States.stop;
                        break;

                    case 100:
                        // Trigger, Punching Position, Setting
                        if (true == dataService.DataSystem.IsSelectedTop)
                            cntService.SetTriggerParams(0, posVision - scanDummyTop, 1000000.0, dataService.DataSystem.TriggerColor_Period, dataService.DataSystem.TriggerColor_Width, dataService.DataSystem.TriggerColor_Level);
                        if (true == dataService.DataSystem.IsSelectedBottom)
                            cntService.SetTriggerParams(2, dataService.DataSystem.BottomDistance + posVision - scanDummyBottom, 1000000.0, dataService.DataSystem.TriggerColor_Period, dataService.DataSystem.TriggerColor_Width, dataService.DataSystem.TriggerColor_Level);
                        if (true == dataService.DataSystem.IsSelectedMono)
                            cntService.SetTriggerParams(1, dataService.DataSystem.MonoDistance + posVision - scanDummyMono, 1000000.0, dataService.DataSystem.TriggerMono_Period, dataService.DataSystem.TriggerMono_Width, dataService.DataSystem.TriggerMono_Level);
                        step += 10;
                        break;

                    case 110:
                        // Trigger Enable
                        if (true == dataService.DataSystem.IsSelectedTop)
                            cntService.SetTriggerEnable(0);
                        if (true == dataService.DataSystem.IsSelectedBottom)
                            cntService.SetTriggerEnable(2);
                        if (true == dataService.DataSystem.IsSelectedMono)
                            cntService.SetTriggerEnable(1);

                        step += 10;
                        break;

                    case 120:
                        // Punch Pos Reset
                        PunchPosMove = posVision;          // 첫번째 유닛의 위치 (SeqAuto 에서 초기위치를 -punchDistance 로 설정함)
                        PunchPosData = PunchPosMove - posStroke;      // 데이터 확인 위치는 펀치 위치보다 1 Stroke 이전에서 확인한다. 
                        PunchDataStartUnit = index;
                        PunchDataEndIdx = index + units;

                        step = 400;
                        break;

                    case 400:
                        step += 10;
                        break;

                    case 410:
                        step += 10;
                        break;

                    case 420:
                        // Send Scan
                        if (true == dataService.DataSystem.IsSelectedTop)
                        {
                            topService.SetScanStart(index);
                            top2Service.SetScanStart(index);
                        }
                        if (true == dataService.DataSystem.IsSelectedBottom)
                        {
                            bottomService.SetScanStart(index);
                            bottom2Service.SetScanStart(index);
                        }
                        if (true == dataService.DataSystem.IsSelectedMono)
                        {
                            monoService.SetScanStart(index);
                            mono2Service.SetScanStart(index);
                        }

                        SetTimeout(10000);
                        step += 10;
                        break;

                    case 430:
                        if (dataService.DataSystem.IsSelectedTop)
                        {
                            if (topService.IsReply((int)EnumSmartIC.VisionReplys.scan))
                            {
                                if (top2Service.IsReply((int)EnumSmartIC.VisionReplys.scan))
                                {
                                    step += 10;
                                }
                                else
                                {
                                    if (IsTimeout())
                                    {
                                        sysService.State = SystemService.States.stop;
                                        msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.timeout_ready_top2);
                                    }
                                }
                            }
                            else
                            {
                                if (IsTimeout())
                                {
                                    sysService.State = SystemService.States.stop;
                                    msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.timeout_ready_top);
                                }
                            }
                        }
                        else
                        {
                            step += 10;
                        }
                        break;

                    case 440:
                        if (dataService.DataSystem.IsSelectedBottom)
                        {
                            if (bottomService.IsReply((int)EnumSmartIC.VisionReplys.scan))
                            {
                                if ( bottom2Service.IsReply((int)EnumSmartIC.VisionReplys.scan))
                                {
                                    step += 10;
                                }   
                                else
                                {
                                    if (IsTimeout())
                                    {
                                        sysService.State = SystemService.States.stop;
                                        msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.timeout_ready_bottom2);
                                    }
                                }
                            }
                            else
                            {
                                if (IsTimeout())
                                {
                                    sysService.State = SystemService.States.stop;
                                    msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.timeout_ready_bottom);
                                }
                            }
                        }
                        else
                        {
                            step += 10;
                        }
                        break;

                    case 450:
                        if (dataService.DataSystem.IsSelectedMono)
                        {
                            if (monoService.IsReply((int)EnumSmartIC.VisionReplys.scan))
                            {
                                if (mono2Service.IsReply((int)EnumSmartIC.VisionReplys.scan))
                                {
                                    step += 10;
                                }
                                else
                                {
                                    if (IsTimeout())
                                    {
                                        sysService.State = SystemService.States.stop;
                                        msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.timeout_ready_mono2);
                                    }
                                }
                            }
                            else
                            {
                                if (IsTimeout())
                                {
                                    sysService.State = SystemService.States.stop;
                                    msgService.ShowAlarm((int)EnumSmartIC.HeavyAlarms.timeout_ready_mono);
                                }
                            }
                        }
                        else
                        {
                            step += 10;
                        }
                        break;

                    case 460:
                        SetTimeout(100);
                        step += 10;
                        break;

                    case 470:
                        if (IsTimeout())
                            step = 500;
                        break;

                    case 500:
                        step = 20000;
                        break;

                    case 20000:
                        dataService.IsBackFeeding = false;
                        motionService.AMove(axisPunch, PunchPosMove, vel, accel);       // Vision 은 스스로 구동
                        return 0;

                    default:
                        break;
                }

                System.Windows.Forms.Application.DoEvents();
            }

            dataService.IsBackFeeding = false;
            return -1;
        }

        protected override int SetAutoJog()
        {
            threadJog = new Thread(ThreadJog);
            threadJog.IsBackground = true;
            threadJog.Start();

            return 0;
        }

        private void ThreadJog()
        {
            if ((null == sysService) || (null == seqService) || (null == dioService) || (null == motionService))
            {
                ResetFlag((int)EnumSmartIC.SeqFlags.autoJogRun);
                return;
            }

            if (SystemService.States.run <= sysService.State)
            {
                ResetFlag((int)EnumSmartIC.SeqFlags.autoJogRun);
                return;
            }

            if (false == seqService.IsHomeDone)
            {
                ResetFlag((int)EnumSmartIC.SeqFlags.autoJogRun);
                return;
            }

            if (dataService.DataSystem.MachineName == "Virtual")
                return;

            const int stepJog = 1000;

            int step = 0;
            int oldStep = -1;

            int inport = 0;
            int outport = 0;
            int sequence = 0;
            double speed = 1.0;

            while (IsFlag((int)EnumSmartIC.SeqFlags.autoJogRun))
            {
                if (oldStep != step)
                {
                    oldStep = step;
                }

                switch (step)
                {
                    case 0:
                        step += 10;
                        break;

                    case 10:
                        if (dioService.IsInportOn((int)EnumSmartIC.Inports.visionJogP))
                        {
                            inport = (int)EnumSmartIC.Inports.visionJogP;
                            outport = (int)EnumSmartIC.Outports.visionJogPLamp;
                            sequence = (int)EnumSmartIC.Sequences.autoJogP;

                            if (true == dioService.IsInportOn((int)EnumSmartIC.Inports.visionJogMode))
                                speed = 2.0;
                            else
                                speed = 1.0;

                            step += 10;
                        }
                        else if (dioService.IsInportOn((int)EnumSmartIC.Inports.visionJogN))
                        {
                            inport = (int)EnumSmartIC.Inports.visionJogN;
                            outport = (int)EnumSmartIC.Outports.visionJogNLamp;
                            sequence = (int)EnumSmartIC.Sequences.autoJogN;

                            if (true == dioService.IsInportOn((int)EnumSmartIC.Inports.visionJogMode))
                                speed = 2.0;
                            else
                                speed = 1.0;

                            step += 10;
                        }
                        else if (dioService.IsInportOn((int)EnumSmartIC.Inports.visionJogP2))
                        {
                            inport = (int)EnumSmartIC.Inports.visionJogP2;
                            outport = (int)EnumSmartIC.Outports.visionJogPLamp;
                            sequence = (int)EnumSmartIC.Sequences.autoJogP;

                            if (true == dioService.IsInportOn((int)EnumSmartIC.Inports.visionJogMode2))
                                speed = 2.0;
                            else
                                speed = 1.0;

                            step += 10;
                        }
                        else if (dioService.IsInportOn((int)EnumSmartIC.Inports.visionJogN2))
                        {
                            inport = (int)EnumSmartIC.Inports.visionJogN2;
                            outport = (int)EnumSmartIC.Outports.visionJogNLamp;
                            sequence = (int)EnumSmartIC.Sequences.autoJogN;

                            if (true == dioService.IsInportOn((int)EnumSmartIC.Inports.visionJogMode2))
                                speed = 2.0;
                            else
                                speed = 1.0;

                            step += 10;
                        }
                        else if (dioService.IsInportOn((int)EnumSmartIC.Inports.punchJogP))
                        {
                            inport = (int)EnumSmartIC.Inports.punchJogP;
                            outport = (int)EnumSmartIC.Outports.punchJogPLamp;
                            sequence = (int)EnumSmartIC.Sequences.autoJogP;

                            if (true == dioService.IsInportOn((int)EnumSmartIC.Inports.punchJogMode))
                                speed = 2.0;
                            else
                                speed = 1.0;

                            step += 10;
                        }
                        else if (dioService.IsInportOn((int)EnumSmartIC.Inports.punchJogN))
                        {
                            inport = (int)EnumSmartIC.Inports.punchJogN;
                            outport = (int)EnumSmartIC.Outports.punchJogNLamp;
                            sequence = (int)EnumSmartIC.Sequences.autoJogN;

                            if (true == dioService.IsInportOn((int)EnumSmartIC.Inports.punchJogMode))
                                speed = 2.0;
                            else
                                speed = 1.0;

                            step += 10;
                        }
                        break;

                    case 20:
                        SetTimeout(200);
                        step += 10;
                        break;

                    case 30:
                        if (false == dioService.IsInportOn(inport))
                            step = 0;
                        else
                        {
                            if (IsTimeout())
                                step += 10;
                        }
                        break;

                    case 40:
                        step = stepJog;
                        break;

                    case stepJog:
                        step += 10;
                        break;

                    case stepJog + 10:
                        if (true == dioService.IsInportOn(inport))
                        {
                            seqService.FireEventAutoJog(1);
                            dioService.SetOutport(outport);
                            dataService.CurrentStatus = "조그 구동";                            
                            SetSequence(sequence, 1, speed);
                            
                            step += 10;
                        }
                        else
                        {
                            step = 0;
                        }
                        break;

                    case stepJog + 20:
                        if (false == dioService.IsInportOn(inport))
                        {
                            dioService.ResetOutport(outport);
                            SetSequence(sequence, 0);

                            step += 10;
                        }
                        break;

                    case stepJog + 30:
                        motionService.Stop((int)EnumSmartIC.Axis.visionFeed);
                        motionService.Stop((int)EnumSmartIC.Axis.punchFeed);
                        StopUncoilerRun();
                        StopRecoilerRun();

                        step += 10;
                        break;

                    case stepJog + 40:
                        if (motionService.IsMotionDone((int)EnumSmartIC.Axis.visionFeed))
                        {
                            if (motionService.IsMotionDone((int)EnumSmartIC.Axis.punchFeed))
                            {
                                SetTimeout(200);
                                step += 10;
                            }
                        }
                        break;

                    case stepJog + 50:
                        if (IsTimeout())
                        {
                            dioService.ResetOutport((int)EnumSmartIC.Outports.visionJogPLamp);
                            dioService.ResetOutport((int)EnumSmartIC.Outports.visionJogNLamp);
                            dioService.ResetOutport((int)EnumSmartIC.Outports.punchJogPLamp);
                            dioService.ResetOutport((int)EnumSmartIC.Outports.punchJogNLamp);

                            // Punch Up
                            dioService.SetOutport((int)EnumSmartIC.Outports.punchUp);
                            dioService.ResetOutport((int)EnumSmartIC.Outports.punchDown);

                            // Cleaner Off
                            ResetCleanRoller();

                            //Ionizer Off
                            dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer1On);
                            dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer2On);
                            dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer3On);
                            dioService.ResetOutport((int)EnumSmartIC.Outports.ionizer4On);
                            dioService.ResetOutport((int)EnumSmartIC.Outports.loaderIonizerBlow);
                            dioService.ResetOutport((int)EnumSmartIC.Outports.unloaderIonizerBlow);

                            seqService.FireEventAutoJog(0);

                            dataService.CurrentStatus = "대기 상태";
                            step += 10;
                        }
                        break;

                    case stepJog + 60:
                        step = 0;
                        break;

                    case 20000:
                        motionService.Stop((int)EnumSmartIC.Axis.visionFeed);
                        motionService.Stop((int)EnumSmartIC.Axis.punchFeed);
                        StopUncoilerRun();
                        StopRecoilerRun();
                        dioService.ResetOutport((int)EnumSmartIC.Outports.visionJogPLamp);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.visionJogNLamp);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.punchJogPLamp);
                        dioService.ResetOutport((int)EnumSmartIC.Outports.punchJogNLamp);

                        seqService.FireEventAutoJog(0);
                        return;

                    default:
                        break;
                }

                System.Windows.Forms.Application.DoEvents();
            }

            motionService.Stop((int)EnumSmartIC.Axis.visionFeed);
            motionService.Stop((int)EnumSmartIC.Axis.punchFeed);
            StopUncoilerRun();
            StopRecoilerRun();
            dioService.ResetOutport((int)EnumSmartIC.Outports.visionJogPLamp);
            dioService.ResetOutport((int)EnumSmartIC.Outports.visionJogNLamp);
            dioService.ResetOutport((int)EnumSmartIC.Outports.punchJogPLamp);
            dioService.ResetOutport((int)EnumSmartIC.Outports.punchJogNLamp);

            seqService.FireEventAutoJog(0);

            ResetFlag((int)EnumSmartIC.SeqFlags.autoJogRun);

            threadJog.Abort();
        }

    }
}
