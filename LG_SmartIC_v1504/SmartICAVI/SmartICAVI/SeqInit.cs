namespace SmartICAVI
{
    class SeqInit : SeqManual
    {
        public SeqInit()
        {
            mode = (int)SystemService.Modes.home;
        }

        public override void RunProcess()
        {
            int step = 0;

            int homeDone_TopZ = 0;
            int homeDone_BottomZ = 0;
            int homeDone_PunchX = 0;
            int homeDone_PunchY = 0;

            bool uncoilerSet = false;

            dataService.DataResult.Clear();

            //dataService.DataResult.SectionMinUnits = dataService.DataSystem.SectionMinUnits;
            dataService.DataResult.SectionMinUnits = dataService.DataRecipe.SectionMinUnits;

            dataService.CurrentStatus = "원점수행";

            seqService.IsHomeDone = false;
            seqService.IsBufferInitDone = false;

            //motionService.SetServoOn((int)EnumSmartIC.Axis.uncoilerReel, true);
            //motionService.SetServoOn((int)EnumSmartIC.Axis.recoilerReel, true);
            //dioService.ResetOutport(30);
            dioService.ResetOutport(31);

            while (IsRun)
            {
                switch (step)
                {
                    case 0:     
                        // Sensor Check
                        sysService.State = SystemService.States.homing;

                        seqService.FireEventInitStep(step);
                        seqService.FireEventInitString("Sensor Check Start");

                        
                        step += 10;
                        break;
                    case 10:    
                        // Recoiler Sensor Check
                        if (1 == motionService.GetStateLimitN((int)EnumSmartIC.Axis.recoilerReel))
                        {
                            seqService.FireEventInitString("Recoiler (-) Limit Check ... OK");
                            RecoilerRelease(0);
                            step = 100;
                        }
                        else
                        {
                            SetTimeout(10000);
                            RecoilerRelease(1);
                            step += 10;
                        }
                        break;
                    case 20:
                        if (false == IsTimeout())
                        {
                            if (1 == motionService.GetStateLimitN((int)EnumSmartIC.Axis.recoilerReel))
                            {
                                RecoilerRelease(0);
                                seqService.FireEventInitString("Recoiler (-) Limit Check ... OK");
                                step = 100;
                            }
                        }
                        else
                        {
                            RecoilerRelease(0);
                            seqService.FireEventInitString("Recoiler (-) Limit Check ... Fail");
                            step = 40000;
                        }
                        break;

                    case 100:
                        // Buffer Check
                        RecoilerRelease(0);
                        if (1 == motionService.GetStateLimitN((int)EnumSmartIC.Axis.buffer))
                        {
                            seqService.FireEventInitString("Buffer (-) Limit Check ... OK");
                            step = 200;
                        }
                        else
                        {
                            SetTimeout(30000);
                            motionService.JogP((int)EnumSmartIC.Axis.visionFeed, 50.0, 100.0);
                            step += 10;
                        }
                        break;
                    case 110:
                        // Buffer Check 
                        if (false == IsTimeout())
                        {
                            if (1 == motionService.GetStateLimitN((int)EnumSmartIC.Axis.buffer))
                            {
                                UncoilerRelease(0);
                                motionService.Stop((int)EnumSmartIC.Axis.visionFeed);
                                seqService.FireEventInitString("Buffer (-) Limit Check ... OK");
                                step = 200;
                            }
                            else
                            {
                                if (1 == motionService.GetStateLimitN((int)EnumSmartIC.Axis.uncoilerReel))
                                {
                                    UncoilerRelease(0);
                                    
                                    if (false == uncoilerSet)
                                    {
                                        motionService.SetPosition((int)EnumSmartIC.Axis.uncoilerReel, 0.0);
                                        uncoilerSet = true;
                                    }
                                }
                                else
                                {
                                    if (true == uncoilerSet)
                                    {
                                        if (150.0 < motionService.GetCurrentPosition((int)EnumSmartIC.Axis.uncoilerReel))
                                        {
                                            UncoilerRelease(1);
                                        }
                                    }
                                    else
                                    {
                                        UncoilerRelease(1);
                                    }
                                }
                            }
                        }
                        else
                        {
                            UncoilerRelease(0);
                            motionService.Stop((int)EnumSmartIC.Axis.visionFeed);
                            seqService.FireEventInitString("Buffer (-) Limit Check ... Fail");
                            step = 40000;
                        }
                        break;

                    case 200:
                        // Uncoiler Sensor Check
                        if (1 == motionService.GetStateLimitN((int)EnumSmartIC.Axis.uncoilerReel))
                        {
                            seqService.FireEventInitString("Uncoiler (-) Limit Check ... OK");
                            UncoilerRelease(0);
                            step = 300;
                        }
                        else
                        {
                            SetTimeout(10000);
                            step += 10;
                        }
                        break;
                    case 210:
                        if (false == IsTimeout())
                        {
                            if (1 == motionService.GetStateLimitN((int)EnumSmartIC.Axis.uncoilerReel))
                            {
                                seqService.FireEventInitString("Uncoiler (-) Limit Check ... OK");
                                UncoilerRelease(0);
                                step = 300;
                            }
                            else
                            {
                                UncoilerRelease(1);
                            }
                        }
                        else
                        {
                            seqService.FireEventInitString("Uncoiler (-) Limit Check ... Fail");
                            UncoilerRelease(0);
                            step = 4000;
                        }
                        break;


                    case 300:
                        motionService.SetPosition((int)EnumSmartIC.Axis.uncoilerReel, 0.0);
                        motionService.SetPosition((int)EnumSmartIC.Axis.buffer, 0.0);
                        motionService.SetPosition((int)EnumSmartIC.Axis.recoilerReel, 0.0);

                        seqService.FireEventInitStep(step);
                        step = 1000;
                        break;



                    case 1000:
                        seqService.FireEventInitStep(step);
                        step += 100;
                        break;

                    case 1100:           // TopZ Home
                        seqService.FireEventInitStep(step);
                        seqService.FireEventInitString("TopVision Z Start");

                        motionService.HomeSearch((int)EnumSmartIC.Axis.visionTop);
                        step += 10;
                        break;
                    case 1110:           // Bottom Z Home
                        seqService.FireEventInitStep(step);
                        seqService.FireEventInitString("BottomVision Z Start");

                        motionService.HomeSearch((int)EnumSmartIC.Axis.visionBottom);
                        step += 10;
                        break;
                    case 1120:           // Punch X
                        seqService.FireEventInitStep(step);
                        seqService.FireEventInitString("Punch X Start");

                        motionService.HomeSearch((int)EnumSmartIC.Axis.punchX);
                        step += 10;
                        break;
                    case 1130:           // Punch Y
                        seqService.FireEventInitStep(step);
                        seqService.FireEventInitString("Punch Y Start");

                        motionService.HomeSearch((int)EnumSmartIC.Axis.punchY);
                        step += 10;
                        break;
                    case 1140:
                        step = 1200;
                        break;

                    case 1200:
                        if (0 == homeDone_TopZ)
                        {
                            if (1 == motionService.GetStateHomeDone((int)EnumSmartIC.Axis.visionTop))
                            {
                                seqService.FireEventInitStep(step);
                                seqService.FireEventInitString("TopVision Z Done");
                                homeDone_TopZ = 1;
                            }
                        }
                        step += 10;
                        break;
                    case 1210:
                        if (0 == homeDone_BottomZ)
                        {
                            if (1 == motionService.GetStateHomeDone((int)EnumSmartIC.Axis.visionBottom))
                            {
                                seqService.FireEventInitStep(step);
                                seqService.FireEventInitString("BottomVision Z Done");
                                homeDone_BottomZ = 1;
                            }
                        }
                        step += 10;
                        break;
                    case 1220:
                        if (0 == homeDone_PunchX)
                        {
                            if (1 == motionService.GetStateHomeDone((int)EnumSmartIC.Axis.punchX))
                            {
                                seqService.FireEventInitStep(step);
                                seqService.FireEventInitString("Punch X Done");
                                homeDone_PunchX = 1;
                            }
                        }
                        step += 10;
                        break;
                    case 1230:
                        if (0 == homeDone_PunchY)
                        {
                            if (1 == motionService.GetStateHomeDone((int)EnumSmartIC.Axis.punchY))
                            {
                                seqService.FireEventInitStep(step);
                                seqService.FireEventInitString("Punch Y Done");
                                homeDone_PunchY = 1;
                            }
                        }
                        step += 10;
                        break;
                    case 1240:
                        if (4 == homeDone_TopZ + homeDone_BottomZ + homeDone_PunchX + homeDone_PunchY)
                            step = 1300;
                        else
                            step = 1200;
                        break;

                    case 1300:
                        SetTimeout(500);
                        step += 10;
                        break;
                    case 1310:
                        //seqService.FireEventInitStep(step);
                        step = 20000;
                        break;




                    case 20000:
                        motionService.SetPosition(0, 0.0);
                        motionService.SetPosition(1, 0.0);
                        motionService.SetPosition(2, 0.0);
                        motionService.SetPosition(3, 0.0);
                        motionService.SetPosition(4, 0.0);
                        motionService.SetPosition(5, 0.0);
                        motionService.SetPosition(6, 0.0);
                        motionService.SetPosition(7, 0.0);
                        motionService.SetPosition(8, 0.0);
                        seqService.FireEventInitStep(step);

                        seqService.IsHomeDone = true;

                        sysService.State = SystemService.States.homeDone;
                        sysService.State = SystemService.States.ready;

                        dataService.CurrentStatus = "원점수행 완료";

                        //motionService.SetServoOn((int)EnumSmartIC.Axis.uncoilerReel, false);
                        //motionService.SetServoOn((int)EnumSmartIC.Axis.recoilerReel, false);
                        return;
                    case 40000:
                        return;

                    default:
                        sysService.State = SystemService.States.homeDone;

                        //motionService.SetServoOn((int)EnumSmartIC.Axis.uncoilerReel, false);
                        //motionService.SetServoOn((int)EnumSmartIC.Axis.recoilerReel, false);
                        return;

                }

                System.Windows.Forms.Application.DoEvents();
            }

            UncoilerRelease(0);
            RecoilerRelease(0);
            motionService.Stop((int)EnumSmartIC.Axis.visionFeed);
            motionService.Stop((int)EnumSmartIC.Axis.punchFeed);

            //motionService.SetServoOn((int)EnumSmartIC.Axis.uncoilerReel, false);
            //motionService.SetServoOn((int)EnumSmartIC.Axis.recoilerReel, false);
        }

        private void UncoilerRelease(int on)
        {
            double offset = dataService.DataMotion[0].Voffset;

            if (1 == on)
            {
                double jog = dataService.DataMotion[0].Vjog;


                double dir = 1.0;

                if (true == dioService.IsInportOn((int)EnumSmartIC.Inports.uncoilerReelDir))
                {
                    dir = -1.0;
                }

                aioService.SetOutvalue(0, offset + jog * dir);
            }
            else
            {
                aioService.SetOutvalue(0, offset);
            }
        }

        private void RecoilerRelease(int on)
        {
            int axis = (int)EnumSmartIC.Axis.recoilerReel;
            double offset = dataService.DataMotion[axis].Voffset;

            if (1 == on)
            {
                double jog = dataService.DataMotion[axis].Vjog;
                double dir = 1.0;

                if (true == dioService.IsInportOn((int)EnumSmartIC.Inports.recoilerReelDir))
                {
                    dir = -1.0;
                }
                aioService.SetOutvalue(1, offset - jog * dir);
            }
            else
            {
                aioService.SetOutvalue(1, offset);
            }
        }
    }
}
