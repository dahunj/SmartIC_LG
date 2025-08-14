namespace SmartICAVI
{
    class EnumSmartIC
    {
        public enum LightAlarms
        {
            // 0
            close = 0,           inputError,         save,               savedone,               delete,
            currentModel,        sameModel,          noFile,             password,               noModel,
            // 10                
            cannotRead = 10,     sameLotID,          manualReport,       manualReportDone,       waitCopyToServer,
            resetPunchCount,     pauseAfterNUnit,    msg17,              msg18,                  msg19,
            // 20                
            jobPermission = 20,  jobDone,            jobCancel,          jobAbort,               jobPause,
            jobResume,           jobOffline,         jobMesDisconnect,   jobContinue,            jobInitPos,
            // 30                
            ngContinue = 30,     ngSection,          thole,              cngStart,               cngEnd,
            joint,               reStart,            missPrintStart,     missPrintEnd,           dualError,
            setIndex,
            // 50                
            punchTolerance = 50, punchInspect,       alignError,         jointBottom,			checkDefect,
            sameUserID,          checkPassword,      PasswordError,      alignTolerance,         msg49,
            // 60                
            zeroPosComp = 60,    laserPosMove,       laserMoveComp,
            // 70
            JobCancelSelect = 70,       // 2019.05.27 khs - 이어서 검사 취소시 한번더 물어보기
        }

        public enum HeavyAlarms
        {
            // 0
            emo1 = 0,                           emo2,                           lightCurtain,                       mainAir,                            airFlow,
            ionizer1,                           ionizer2,                       ionizer3,                           ionizer4,                           alarm09,
            // 10
            notDetected_limitN_uncoiler,        notDetected_limitN_buffer,      notDetected_limitN_recoiler,        limitN_uncoiler,                    limitP_uncoiler, 
            limitN_buffer,                      limitP_buffer,                  limitN_recoiler,                    limitP_recoiler,                    stopperDown_vision,
            // 20
            stopperDown_buffer,                 stopperDown_punch,              deviation_vision,                   deviation_punch,                    punch_down,
            uncoilerReel_clutch,                uncoilerReel_airchuck,          uncoilerSheet_clutch,               uncoilerSheet_airchuck,             uncoilerSheet_torque,
            // 30
            recoilerReel_clutch,                recoilerReel_airchuck,          recoilerSheet_clutch,               recoilerSheet_airchuck,             recoilerSheet_torque,

            // 100
            wrongLotID = 100,                   wrongDir,                       mesReply,                           mesDisconnect,                      specialChar,
            noRecipe,                           saveFail,                       networkDrive,                       continuousNG,                       punchCountLimit,                                       

            // 200
            noRecipe_top = 200,                 noRecipe_top2,                  noRecipe_bottom,                    noRecipe_bottom2,                   noRecipe_mono,
            noRecipe_mono2,                     noRecipe206,                    noRecipe207,                        noRecipe208,                        noRecipe209,  

            // 210
            timeout_ready_top,                  timeout_ready_top2,             timeout_ready_bottom,               timeout_ready_bottom2,              timeout_ready_mono, 
            timeout_ready_mono2,                timeout216,                     timeout217,                         timeout218,                         timeout_align,
            // 220
            index_top,                          index_top2,                     index_bottom,                       index_bottom2,                      index_mono,
            index_mono2,                        index_verify,                   index227,                           index228,                           index229,
            // 230
            overframe_top,                      overframe_top2,                 overframe_bottom,                   overframe_bottom2,                  overframe_mono,
            overframe_mono2,                    overframe236,                   overframe237,                       overframe238,                       overframe239,
            // 240
            line_top,                           line_top2,                      line_bottom,                        line_bottom2,                       line_mono,
            line_mono2,                         line246,                        line247,                            line248,                            line249,
            
            // 500
            servoOff_Down = 500,                servoOff_limitN_uncoiler,       servoOff_limitP_uncoiler,           servoOff_limitN_buffer,             servoOff_limitP_buffer,
            servoOff_limitN_recoiler,           servoOff_limitP_recoiler,
            

            //1000
            servoAlarm_Axis0 = 1000,            limitN_Axis0,                   limitP_Axis0,                       timeoutMotion_Axis0,                timeoutHome_Axis0,
            motionAlarm_Axis0,                  inposAlarm_Axis0,               homeAlarm_Axis0,
            servoAlarm_Axis1 = 1010,            limitN_Axis1,                   limitP_Axis1,                       timeoutMotion_Axis1,                timeoutHome_Axis1,
            motionAlarm_Axis1,                  inposAlarm_Axis1,               homeAlarm_Axis1,
            servoAlarm_Axis2 = 1020,            limitN_Axis2,                   limitP_Axis2,                       timeoutMotion_Axis2,                timeoutHome_Axis2,
            motionAlarm_Axis2,                  inposAlarm_Axis2,               homeAlarm_Axis2,
            servoAlarm_Axis3 = 1030,            limitN_Axis3,                   limitP_Axis3,                       timeoutMotion_Axis3,                timeoutHome_Axis3,
            motionAlarm_Axis3,                  inposAlarm_Axis3,               homeAlarm_Axis3,
            servoAlarm_Axis4 = 1040,            limitN_Axis4,                   limitP_Axis4,                       timeoutMotion_Axis4,                timeoutHome_Axis4,
            motionAlarm_Axis4,                  inposAlarm_Axis4,               homeAlarm_Axis4,
            servoAlarm_Axis5 = 1050,            limitN_Axis5,                   limitP_Axis5,                       timeoutMotion_Axis5,                timeoutHome_Axis5,
            motionAlarm_Axis5,                  inposAlarm_Axis5,               homeAlarm_Axis5,
            servoAlarm_Axis6 = 1060,            limitN_Axis6,                   limitP_Axis6,                       timeoutMotion_Axis6,                timeoutHome_Axis6,
            motionAlarm_Axis6,                  inposAlarm_Axis6,               homeAlarm_Axis6,
            servoAlarm_Axis7 = 1070,            limitN_Axis7,                   limitP_Axis7,                       timeoutMotion_Axis7,                timeoutHome_Axis7,
            motionAlarm_Axis7,                  inposAlarm_Axis7,               homeAlarm_Axis7,
            servoAlarm_Axis8 = 1080,            limitN_Axis8,                   limitP_Axis8,                       timeoutMotion_Axis8,                timeoutHome_Axis8,
            motionAlarm_Axis8,                  inposAlarm_Axis8,               homeAlarm_Axis8,
            servoAlarm_Axis9 = 1090,            limitN_Axis9,                   limitP_Axis9,                       timeoutMotion_Axis9,                timeoutHome_Axis9,
            motionAlarm_Axis9,                  inposAlarm_Axis9,               homeAlarm_Axis9,
        }

        public enum Inports
        {
            emo1 = 0,               emo2,                   lightCurtain,           mainAir,                airFlow,
            in05,                   in06,                   in07,                   uncoilerReelDir,        uncoilerClutch,

            uncoilerAirChuck,       uncoilerSheetClutch,    uncoilerSheetAirChuck,  uncoilerSheetTorque,    visionTorque,
            visionJogMode,          visionJogP,             visionJogN,             punchTorque,            punchJogMode,

            punchJogP,              punchJogN,              recoilerReelDir,        recoilerReelClutch,     recoilerReelAirChuck,
            recoilerSheetClutch,    recoilerSheetAirChuck,  recoilerSheetTorque,    in28,                   visionJogMode2,
            visionJogP2,            visionJogN2,

            cleanUnitOn,            cleanUnitOff,           punchUp,                punchDown,              visionStopperUp,
            visionStopperDown,      visionGuideUp,          visionGuideDown,        visionDeviation,        bufferStopperUp,

            bufferStopperDown,      punchGuideUp,           punchGuideDown,         punchDeviation,         punchStopperUp,
            punchStopperDown,       uncoilerSheetTorqueAlarm, visionTorqueAlarm,    punchTorqueAlarm,       recoilerSheetTorqueAlarm,

            ionizer1Error,          ionizer2Error,          ionizer3Error,          ionizer4Error,          in124,

            in125,                  in126,                  in127,                  in128,                  in129,
            in130,                  in131,
        }

        public enum Outports
        {
            twlampRed = 0,      twlampYellow,               twlampGreen,        buzzer1,            buzzer2,
            out05,              machineLight,               out07,              loaderIonizerBlow,  unloaderIonizerBlow,

            topVisionCooling,   bottomVisionCooling,        cleanUnitOn,        cleanUnitOff,       punchUp,
            punchDown,          uncoilerSheetTorqueReset,   visionTorqueReset,  punchTorqueReset,   recoilerSheetTorqueReset,

            visionJogPLamp,     visionJogNLamp,             punchJogPLamp,      punchJogNLamp,      ionizer1On,
            ionizer2On,         ionizer3On,                 ionizer4On,         laserOn,            out29,

            runCheck,           runMode,
        }

        public enum Axis
        {
            uncoilerReel = 0, visionFeed, visionTop, visionBottom, punchFeed,
            punchX, punchY, recoilerReel, buffer,
        }

        public enum TriggerAxis { top = 0, bottom, mono }

        public enum Sequences
        {
            uncoilerBuffer = 0,             recoilerBuffer,             bufferInit,                     bufferReference,
            visionForward,                  visionBackward,             punchForward,                   punchBackward,

            feedJogN,                       feedJogP,                   feedRMoveN,                     feedRMoveP,                     feedAMove,

            teachTop,                       teachBottom,                teachMono,                      teachPunch,                     teachToZero,
            teachPunchTest, 

            initSearch,                     initRMove,                  autoJog,                        autoJogP,                       autoJogN,
            backFeeding,                    laserMove,
        }

        public enum SeqFlags
        {
            uncoilerRun=0,              uncoilerReady,              recoilerRun,                recoilerReady,              bufferRun,
            bufferReady,                visionForward,              visionBackward,             punchForward,               punchBackward,

            autoJogRun,                 autoJogStop,                manualFeedStop,             uncoilerRunning,            recoilerRunning,
        }

        public enum VisionReplys
        {
            connect = 0, status, mode, recipe, load, scan, result, initpos, light, grab, inspect,
        }

        public enum VisionFlags
        {
            connect = 0, status, modeTeach, modeInspect, recipe, initPos, grab, inspect,
        }

        public enum PunchStates { punch=0, punchDone, punchStart, punchEnd, tHole, tHoleDone, sectionYield, sectionYieldDone, punchLine, cngStart, cngEnd, missPrintStart, missPrintEnd}

        public enum DefectIDs
        {
            NG1=0,      NG2,        NG3,        NG4,        NG5,
            A,          B,          BB001,      BB006,      BB012,
            // 10
            BB018,      BB019,      BB025,      BB026,      BB029,
            BB034,      BB038,      BB039,      BB040,      BB042,
            // 20
            BB043,      BB045,      BB047,      BB053,      BB062,
            BB064,      BB066,      BB068,      BB072,      BB074,
            // 30
            BB088,      BB089,      BB090,      BB091,      BB092,
            BB093,      BB094,      BB095,      BB096,      BB097,
            // 40
            BB098,      BB099,      BB100,      C,          D,
            E,          F,          H,          J,          K,
            // 50
            L,          M,          N,          O,          P,
            Q,          R,          S,          T,          U,
            // 60
            V,          W,          Y,
        }

        //1	Scratch(회로)	1	도금부 스크래치	
        //2	얼룩	2		
        //3	이물(PI)	3		
        //4	Scratch(PI)	4		
        //5	TOP 패임	5		
        //6	비금속 이물	A		
        //7	SR B/O	B		
        //8	Damage	BB001		
        //9	기타이물	BB006		
        //10	미도금	BB012	미도금	Bonding Hole 미도금
        //11	위치 어긋남	BB018		
        //12	인쇄 불량	BB019		
        //13	접착제 B/O	BB025	Adhesive Flow	
        //14	층간들뜸	BB026	PPG 이물	
        //15	펀칭오염	BB029		
        //16	Burr	BB034		
        //17	FLEX 코팅불량	BB038		
        //18	FLEX핀홀	BB039		
        //19	Scratch(Emboss)	BB040		
        //20	Scratch(차폐판)	BB042		
        //21	LEAD변형	BB043		
        //22	Marking불량	BB045		
        //23	Over 에칭	BB047		
        //24	S/H 파손	BB053	Sprocket Hole Dimension	
        //25	SR Filmy	BB062		
        //26	T/L불량	BB064		
        //27	Tool Mark	BB066		
        //28	Warpage	BB068		
        //29	미펀칭	BB072		
        //30	미에칭	BB074		
        //31	V-cut불량	BB088		
        //32	돌기	BB089		
        //33	도금 Pin Hole	BB090	핀홀	
        //34	PI면 불량	BB091		
        //35	FIDS 마크 불량	BB092		
        //36	FIDS PI Scratch	BB093		
        //37	CL 기포	BB094		
        //38	CL 위치 어긋남	BB095		
        //39	CL 하부 오염	BB096		
        //40	CL 접착제 B/O	BB097		
        //41	CL 하부 변색	BB098		
        //42	CL 겹침	BB099		
        //43	LEAD 단락	BB100		
        //44	전공정(AOI Punching)불량	C		
        //45	Dent	D		
        //46	금속이물	E	도금부 이물	
        //47	변색	F		
        //48	SR 핀홀	H		
        //49	잔류 Cu	J	Metal Residue	
        //50	검은이물	K		
        //51	도금불량	L		
        //52	패임	M		
        //53	SR 불량	N		
        //54	Open	O	Reduction of Pattern	
        //55	돌출	P		
        //56	SR Misalign	Q		
        //57	S/C AM	R		
        //58	Short	S	Reduction of Space	
        //59	SR 튐	T		
        //60	오염	U	도금부 오염	Bonding Hole 오염
        //61	기타	V		
        //62	SR 기포	W		
        //63	Punching Misalign	Y		

    }
}




