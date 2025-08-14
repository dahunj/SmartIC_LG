using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

// 어셈블리의 일반 정보는 다음 특성 집합을 통해 제어됩니다.
// 어셈블리와 관련된 정보를 수정하려면
// 이 특성 값을 변경하십시오.
[assembly: AssemblyTitle("SmartICAVI")]
[assembly: AssemblyDescription("SmartIC AVI")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("SyanpseImaging")]
[assembly: AssemblyProduct("SmartICAVI")]
[assembly: AssemblyCopyright("Copyright ©  2016 Synapse Imaging Co.,Ltd.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// ComVisible을 false로 설정하면 이 어셈블리의 형식이 COM 구성 요소에 
// 표시되지 않습니다. COM에서 이 어셈블리의 형식에 액세스하려면 
// 해당 형식에 대해 ComVisible 특성을 true로 설정하십시오.
[assembly: ComVisible(false)]

//지역화 가능 응용 프로그램 빌드를 시작하려면 
//.csproj 파일에서 <PropertyGroup> 내에 <UICulture>CultureYouAreCodingWith</UICulture>를
//설정하십시오.  예를 들어 소스 파일에서 영어(미국)를
//사용하는 경우 <UICulture>를 en-US로 설정합니다.  그런 다음 아래
//NeutralResourceLanguage 특성의 주석 처리를 제거합니다.  아래 줄의 "en-US"를 업데이트하여
//프로젝트 파일의 UICulture 설정과 일치시킵니다.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //테마별 리소스 사전의 위치
    //(페이지 또는 응용 프로그램 리소스 사전에 
    // 리소스가 없는 경우에 사용됨)
    ResourceDictionaryLocation.SourceAssembly //제네릭 리소스 사전의 위치
    //(페이지, 응용 프로그램 또는 모든 테마별 리소스 사전에 
    // 리소스가 없는 경우에 사용됨)
)]


// 어셈블리의 버전 정보는 다음 네 가지 값으로 구성됩니다.
//
//      주 버전
//      부 버전 
//      빌드 번호
//      수정 버전
//
// 모든 값을 지정하거나 아래와 같이 '*'를 사용하여 빌드 번호 및 수정 버전이 자동으로
// 지정되도록 할 수 있습니다.
// [assembly: AssemblyVersion("1.0.*")]

//[assembly: AssemblyVersion("1.2.0.1")]
//[assembly: AssemblyFileVersion("16.11.22.1800")]

[assembly: AssemblyVersion("1.5.0.4")]
[assembly: AssemblyFileVersion("25.06.10")]


// 불량항목 Stop 기능 사용자 선택 가능하도록.

/*
 * - 1.5.0.0        17.07.13.18
 *      . Align X 위치 편차가 1.8mm 이상일 경우 알람기능 추가
 *      . Verify 시 Space bar 누를경우 검사결과 그대로 적용.
 * 
 * 
 * - 1.5.0.0        17.07.12.18
 *      . LogOn 기능 추가
 *      . History 기능 추가
 *      . private void Modify(int modify) 함수에서 RemoveAt() 시 다운현상 발생
 *          -> if( 0 < listNG.Count ) 추가.
 * 
 * - 1.4.0.0        17.06.22.18
 *      . SetupPage 에서 Verify 버튼 클릭 리셋 안되는 버그 수정. (btnCheckBox_Click)
 * 
 * - 1.4.0.0        17.05.19.18
 *      . 사용자 정의 Defect Stop 기능 추가.
 *      . 2열 Align 홀 계속 검사 오류 수정. Value3 를 검사하고 있었음. 
 *      . 검사길이(m) 표시
 *      
 * - 1.3.0.1        17.05.12.18
 *      . 3열 프로그램
 *          - 수동 Verify 오류 수정. 
 *      . 불량항목 Stop 기능
 *          - DataDefectInfo 추가
 *  
 * - 1.3.0.1        17.05.11.18
 *      . 3열 프로그램
 *          - 중앙열 펀치 위치 1열로 적용되는 버그 수정. 
 *          
 * - 1.3.0.0        17.03.29.18 
 *      . 중복실행 방지 : Mutex 에서 Process 로 바꿈
 *      . 3열 프로그램
 *          - 
 * 
 * - 1.3.0.0        17.03.28.18 
 *      . 3열 프로그램
 *          - DataSystem 에 PunchOffsetCenterX, PunchOffsetCenterY 추가. SetupSystemPage 에 항목 추가.
 *          - DataRecipe 에 PunchCenterX, PunchCenterX 추가. TeachInitPage 에 항목 추가. RecipePage 에 항목 추가.
 *          - PunchProcess 적용.
 *          - TeachPunchPage 에 3열 펀치 가능하도록 추가.
 * 
 * - 1.3.0.0        17.03.27.18 
 *      . 3열 프로그램
 *          - Review 처리.
 *          - Report 처리.
 *          - MES MAP3 처리. ################################## MES 확인 필요 #####################################
 * 
 * - 1.3.0.0        17.03.20.18 
 *      . 3열 프로그램 시작. 
 * 
 * - 1.2.0.8        17.03.06.18 
 *      . Punch 이미지 전체 파일 복사.
 * 
 * - 1.2.0.8        17.02.21.18 
 *      . Verify 부분 Log_Debug 수정 (SetNG, Verify 위주 : 검사 진행 안되는 현상 파악용)
 *      . Log_SeqPunch 제거
 *      . ExternGrab  에서 재 기동 후 Initparams 추가.
 *      
 * - 1.2.0.8        17.01.20.18
 *      . Log_SeqPunch 추가하여 펀치 스텝 확인 (검사 진행 안되는 현상 파악용)
 *      
 * - 1.2.0.8        17.01.18.18
 *      . Manual Page 시 초기화면 DIO -> Feeding 으로 바꿈. 
 *      . Pause->Resume 시 Vision 구동 후 펀치 구동하도록 수정. IsVisionPaused
 *      
 * - 1.2.0.8        17.01.18.18
 *      . 중복실행 방지기능 추가.
 * 
 * - 1.2.0.8        17.01.17.18
 *      . CAM Restart 기능 다시 살림 (3회 반복)
 *      . runCheck(30) 제거, 정상적인 상황에서도 다운으로 인식되는 경우 발생
 *      . Top/Bottom 동시 불량시 조인트 구간에서는 알람발생 하지않도록 수정. 
 *      . Teaching Save 시 메시지창 생성 (연속 저장 후 모드 변경시 CAM 다운됨)
 * 
 * - 1.2.0.7        17.01.13.12
 *      . runCheck(30), runMode(31) 추가. runCheck==1 일경우 0 으로 세팅해야 한다. (SmartIC_Motion 에서 runCheck==1 일경우 서보OFF 함)
 *      
 * - 1.2.0.7        17.01.12.1800
 *      . SmartIC_Motion 추가.
 *      . AutoPage Focus 될 때 마다 CAM.Topmost 호출
 *      . Dump 시 MotionService 새로 생성하여 UnInitialize 호출 
 *      
 * - 1.2.0.7        17.01.10.1800
 *      . 권취부 리미트 오류 수정 (timer End 처리 오류)
 *      
 * - 1.2.0.7        17.01.10.1800
 *      . Dump
 *          - Strobe Disconnet Exception 처리
 *          - Mono Listraw 에서 에러 발생 초기화 시 Value1=Value2="1" 로 설정기능 추가.
 *              .인덱스가 범위를 벗어났습니다. 인덱스는 음수가 아니어야 하며 컬렉션의 크기보다 작아야 합니다.
 *              .SmartICAVI.AutoPage.DisplayPunchMonoValue(int indexLabel, int indexValue)
 *              . IsTimerOn 확인후 tick_timer 에서 Display() 처리
 *          - Log_Debug 추가 : Index 오류를 확인하기 위해 Index 및 List.Count 를 여기에 저장.
 *          - ReviewDisplay 시 Mono Index 오류
 *              . Exception 처리 -> 로그 저장 -> 알람메시지 출력 -> 정지.
 * 
 * - 1.2.0.6        17.01.05.1800
 *      . PunchImage 제한기능 추가 : PunchImageLimit
 *      . 구간수율 적용시 최소 1구간 유닛수 적용 : SectionMinUnits
 *      . Joint Top/Bottom 구분 : jointBottom
 *      . Vision Feeding 도 Punch Feeding 과 같이 Position Override 기능 추가. : VisionPosMove
 *          . protected override void VisionProcess()
 *      . Punch Up 신호 3번 이상 감지되어야 알람처리 : countPunchUp
 *      . Dump Exception 처리
 *          . SmartICAVI.MesSmartIC.get_IsConnected()     
 * 
 * - 1.2.0.5        16.12.23.1800
 *      . MES 에서 int.Parsing 시 double 형으로 내려보내 오류 발생 -> double -> int 변환하도록 수정. 
 *      . Reel Control 버그 수정.
 *      . BackFeeding 기능 추가.
 *      
 * - 1.2.0.5        16.12.22.1800
 *      . Dump File 분석
 *          . 개체 참조가 개체의 인스턴스로 설정되지 않았습니다. (SmartICAVI)
 *              . AutoPage.DisplayMonoValue()
 *              . AutoPage.DisplayTopValue()
 *          . 닫혀 있는 파일에 액세스할 수 없습니다.(mscorlib)
 *              . LogUtill.Close() -> FileStream.Flush()
 *          . 메모리가 부족하여 프로그램을 계속 실행할 수 없습니다. (PresentationCore)
 *              . System.Windows.Threading.ExceptionWrapper.TryCatchWhen()
 *      . CAM SearchArea 적용안되는 버그 수정.
 *      . Application Down 시 Sequence Stop 하도록 추가.
 *      
 * - 1.2.0.4        16.12.16.1800
 *      . Review Punch 화면에서 좌표를 소숫점 2자리에서 3자리까지 표현.
 *      . Dump File 추가
 *      
 * - 1.2.0.4        16.12.15.1800
 *      . Recipe 파일에 펀치 허용공차 추가
 *      . System 파일에 펀치 허용공차 검사유무 설정 추가. 
 *      . MES PunchCount : Lot PunchCount 에서 누적 PunchCount 로 변경.
 *      . Align 및 Punch Hole 검사 오류시 -10 이하의 값이 들어오면 알람기능 추가
 *      . 초기위치 확인 전에는 검사시작 버튼을 노란색으로 , 초기위치 확인 후 파란색
 *      . 첫번째 펀칭위치 오류(급이송 및 정지)시 펀칭오류현상으로 Feeding 후 500mSec Delay 추가.
 *      
 * - 1.2.0.3        16.12.08.1800
 *      . Review 에서 Mono 사용시 TopEndIndex 보다 큰 값이 들어올 경우 end 값을 TopEndIndex 로 변환
 *      . Pause -> Resume 시 PunchFeeding 은 LimitP 보다 작을 경우 Start 하도록 수정
 *      . Recipe 창에서 Recipe 삭제할 수 있도록 수정.
 *      . DualError(Top, Bottom 동시 오류)일 경우 확인기능 추가.
 *      . MapData Server Copy 기능 추가 
 *      . Uncoiler, Recoiler Stop timer 구동시 다시 Stop 명령을 주면, Timer 정지 후 강제 정지하도록 수정. 
 *      
 * - 1.2.0.2        16.12.05.1800
 *      . Punch Stroke bug 수정, Recipe.PunchStroke -> System.PunchStroke
 *      . Punch Feeding 정확도를 높이기위해 정위치 확인기능 추가 IsInPos()
 *      
 * - 16.11.16
 *      . Reveiw 에서 펀치 이미지 사라지는 버그 수정.
 *      . NG Hole 펀치 스킵기능 추가 (H -> C)
 *      
 * - 16.10.21
 *      . Trigger System Setup 에서 설정할 수 있도록 수정.
 *      . Align Camera MIL 적용     
 * 
 * 
 * - 16.09.12
 *      . Defect Code 변경
 *          - JOINT (V->JT) (JT 로 들어오면 BB006 으로 변환하여 처리)
 *          - Through Hole (T->BB039) 로 변경
 *      . CAM 분리
 *          - DataSystem.CamType == EXTERN 으로 설정되어 있을 경우
 * 
 * - 16.08.29
 *      . 기능추가
 *          - Report 파일에 Through Hole Count 추가
 *          - Light Time 모니터기능 추가. (Defect(SPC) 데이터에 Light Time 추가)
 *          - Punch 이미지 Display 기능 추가. 
 *          - CNG 상태일 경우 처음과 끝 부분만 Punch 중간부분은 Punch 하지 않음. 
 *      . 기능수정
 *          - NG 이미지 로딩 시 각 이미지별 DoEvent() 추가 : 다른 Thread 로 제어권 넘기기 위함
 *          - VisionProcess/PunchProcess 에서 posBuffer 를 따로 읽어드리도록 수정 : 시스템에 로드가 많이 걸릴 경우 제어가 제대로 안됨. 
 *          
 * - 16.07.26
 *      . 기능추가
 *          - 노광편차(BB019) 발생시 정지->알람->사용자확인 후 진행 
 *          
 * - 16.07.25
 *      . 기능추가
 *          - hImage Grab() 함수 전에 Dispose() 기능 추가
 *          - Punch Alarm 시 부저 시간 설정하도록 수정. 
 *      . 기능수정.
 *          - Align 카메라 리셋시 오류 생기므로, Grab 오류시 알람 처리하도록 복구.
 *          - AutoSequence 시작시 Align 카메라 그랩가능여부 채크기능 추가.
 *          
 * - 16.07.23
 *      . 버그수정
 *          - Vision Temp 결과 데이터 저장시 VISION1/VISION2 구분하도록 수정. (이미지 Path 가 잘못 적용됨.)
 *      . 기능추가
 *          - 완공 후 알림기능 추가
 *          
 * - 16.07.22
 *      . 기능변경
 *          - MES 보고를 처음 시작 -> 초기위치 잡은 이후 로 변경 (펀치위치 찾을 경우 착공을 취소하고 다시 착공을 해야하는 불편이 있음)
 *          
 * - 16.07.21
 *      . 기능추가
 *          - Grab Reset 기능 추가 (Test 필요)
 *          
 * - 16.07.20
 *      . 기능추가
 *          - Verify 코드 설정창 추가
 *          - Clean Roller 사용유무 설정.
 *      
 * - 16.07.18
 *      . 기능추가
 *          - Report 파일에 Total 수량만 색상 바꿈. 
 *          - Recipe 명 Lable -> TextBlock 으로 교체.
 * 
 * - 16.07.16
 *      . 기능추가
 *          - Report 파일에 불량 카운트 표시
 *          - Align Vision 오류시 재시도 기능 추가.
 *          
 * - 16.07.15
 *      . 버그 수정
 *          - Abort 후 재 검사시 PunchDataStartUnit 초기가 되는 오류 수정. <-- IsReStarted property 추가.
 *          
 * - 16.07.14
 *      . 기능추가
 *          - 영상 밝기 데이터 추가.
 *          - Recipe 명 '_' 처리 문제로 Label -> TextBlock 으로 바꿈.
 *          
 * - 16.07.13
 *      . 버그수정
 *          - 수동완공시 End 시간 Start 시간과 같게 올라가는 버그 수정. 
 *          
 * - 16.07.12
 *      . Rapid 속도 변환되는 버그 수정 : SetTeachScan() 함수에서 Rapid 속도를 재세팅하고 있었음. 
 *      . Mono 사용시 마지막 프레임 데이터 저장안되는 버그 수정. ReadProc_Result2() 에서 index >= dataService.IndexEndTop 일 경우 리턴함. 
 *          -> index > dataService.IndexEndTop 로 수정 (IndexEndTop 위치가 마지막 index 임. 여기까지 펀칭을 해야 함.)
 *      
 * 
 * - 16.07.11
 *      . Recipe 데이터
 *          - 소재 Offset X 값 / Search Area 티칭시 적용.
 *      . 완공 후 Defect 데이터 전송이 남아 있을 경우 메시지 생성.
 *      . Motion Data 저장 중 punchFeed.RapidVel 를 visionFeed.RapidVel 과 같게 설정 --- 다를 경우 동시 구동 시 문제 발생
 *      . 구간수율 적용 여부도 Auto화면에서 설정할 수 있도록 추가.
 *      . Review 에서 PunchView 선택시 마지막 펀치 이미지 보이도록 수정. + 상단의 Index 표시 추가.
 *      . Restart 시 펀치시작 부터 Top.ListRaw.Count + 100 까지 .dat 파일을 찾아서 데이터 및 이미지 삭제기능 추가. DataService.DeleteReviewData()
 *      . 펀치 타발 리미트 설정. 리셋 기능 필요.
 *      . Restart 시 Verify 기능 Skip 하도록 수정. 
 *          
 * - 16.07.10
 *      . Recipe 데이터
 *          - 초기IPHole Offset X, Y 값 설정할 수 있도록 추가. 
 *          - 검사영역 설정할 수 있도록 추가 mm^2 단위 기본(4.0mm)
 *      . Map 데이터
 *          - 불량항목으로 올리는대신 G, X 로만 올림.
 *      . 소재 IPHole <-> IPHole 폭방향 걸이 : 31.83 mm
 *      . 마지막 index Count 오류 수정. 
 * 
 * - 16.07.09
 *      . Manual Dio 화면에서 실시간으로 Outport 갱신하도록 수정.
 *      . Manual Feeding 화면에서도 조그버튼 다시한번 누르면 정지하도록 수정. 
 *      
 *      
 * - 16.07.07
 *      . MES 테스트 완료
 *      
 * - 16.07.06
 *      . 기능추가
 *          - JobMessageWindow 에서 [재검사] 버튼 추가. 
 *      . Sequence
 *          - 재검사 시퀀스 추가 SeqAuto.RestartProcess();
 *          
 * - 16.07.05
 *      . 기능추가
 *          - T-Hole 감지시 Pass 일 경우 무조건 Punch 작업으로 변경. (Punch or Stop)
 *          - Joint 확인여부 설정기능 추가     
 *          - Result 파일에 Defect Count 기능추가, MES, DefectData 저장시 저장하도록 수정.
 * 
 * - 16.07.04
 *      . 기능추가 
 *          - Review 에서 DisplayNGImage~() 시 ViewType 설정하도록 변경, -->> ViewType 이 변경되면 이미지데이터 찾는중 바로 리턴되도록 수정. 
 *          - Defect Data 에 Comment 추가
 *          - 검사중 Pause 상태에서는 Sensor,Limit,Punch 를 검사하지 않도록 수정. 
 * 
 * - 16.07.02
 *      . 기능추가
 *          - Review 파일이 없을 경우 무한 대기에서 10초 대기 후 Skip 하는 것으로 변경.
 *          - Through Hole/CNG/ 일 경우도 Punch View 에서 Display 할 수 있도록 Line 정보 전송. 
 *          - 같은 LotID 로 작업할 경우 기존 메시지만 출력에서 알람기능 추가.
 *          - Image Display 시 IsDrawing 이 true 일 경우 무한 대기에서 3초 대기로 변경
 *          
 * - 16.07.01
 *      . 기능추가
 *          - Review 이미지 선택기능 (Verify, Punch, Select)
 *          
 * - 16.06.30
 *      . Bug
 *          - Review 이미지 폴더에서 삭제 안되는 버그 수정.
 *          - AutoStart 시 이전 LotID 확인 기능 버그 수정. 
 *      . Sequence
 *          - Job End 후 DefectData 서버 전송을 Thread 에서 실행하도록 수정. 
 *          - 수동완공 기능 추가.
 *          - SW 조그버튼 활성화 : Auto Mode 에서 Run 상태가 아닐 경우 동작.
 *      . GUI
 *          - Verify 시 Verify Index 표시기능 추가.
 * 
 * - 16.06.29
 *      . Bug
 *          - 통합 데이터 A 열로만 저장되는 버그 수정.
 *      . Sequence
 *          - Manual Feeding 에서도 버튼을 다시한번 눌렀을 경우 Manual Stop 으로 전환
 *      . Data
 *          - 구간수율 범위 12 -> 20 개로 증가
 *          
 * - 16.06.28
 *      . Sequence
 *          - Review 이미지 Skip 버그 수정
 *          - Verify 기능 버그 수정. 
 *          
 * - 16.06.25
 *      . Sequence
 *          - Joint 연결 구간 정지기능 추가.
 *          - 검사시 초기위치 자동이동 기능 추가.
 *          
 * - 16.06.22
 *      . Sequence
 *          - 구간수율 오류시 정지/진행 기능 추가.
 *          
 * - 16.06.21
 *      . Sequence
 *          - Throw Hole 메시지 창 생성기능 추가 + 시퀀스 추가
 *          - CNG Start/End 메시지 창 생성기능 추가. + 시퀀스 추가
 *          - NG Hole 일 경우 Punching Skip 기능 추가.
 *          - Defect Data 전송시 Model 폴더 아래 날짜 폴더 추가, 날짜 폴더 안에 Lot 폴더가 위치하도록 변경.
 *          - Uncoiler, Recoiler 동작을 Stop 시에 타이머로 1초 후에 정지하도록 수정.  
 * 
 * - 16.06.17
 *      . Sequence
 *          - Punch 이후에 Punch Align 위치로 이동하여 대기하도록 수정. 
 *          
 * - 16.06.16
 *      . Sequence
 *          - Review 에서 timer -> thread 로 변경 (이미지 로딩하면서 UDP 통신에서 데이터를 놓치는 경우 발생)
 *          - Vision Result 1개 유닛에서 10개유닛 한번에 받는것으로 변경. + Vision Index 와 List 의 Count 가 일치하지 않을 경우 알람 발생
 *      . Bug
 *          - 자동검사시 End 처리 안돼는 것 처리
 * 
 * - 16.06.14
 *      . Punch 시 A 열과 B 열의 위치오차 때문에 PunchOffsetB 를 펀칭할 때 빼 주는데, 검사시에는 그 값을 빼지 말고, 원래 위치에서 검사 해야함. (B열 펀치 위치옵셋 원인 파악 필요)
 *      . Align 및 Punch 데이터/이미지 서버 전송완료.
 *      . Sequence
 *          - Modify 시 TOP/BOTTOM/MONO 는 Good 으로 변경시에만 적용.
 *     
 * 
 * - 16.06.10
 *      . Map data string 확인
 *      . Review 시 Verify Skip 시 Review data add 안되는 버그 수정.
 * 
 * - 16.06.09
 *      . Bug
 *          - Start 위치 오류 수정 : 트리거 시작위치는 무조건 -dummy(0.095) 에서 시작 하도록 설정.
 *      . GUI
 *          - Auto/Report 에 NextProcess 추가.
 *      . Sequence
 *          - Map 데이터를 int -> string 로 변경. 
 *          - MES 완공 적용.
 * 
 * - 16.06.06
 *      . MES 추가
 * 
 * - 16.06.04.1800
 *      . Sequence:
 *          . 검사 및 매뉴얼 피딩시 수동조작버튼(클러치,에어척) On 상태 확인기능 추가.
 *          . Light/Heavy Alarm List 및 Imge 작성.
 *          
 * 
 * - 16.06.03.1800
 *      . Bug :
 *          . InitPosSearch 시 Punch 좌표를 초기화 하지 않아 검사후 0.0 위치 이동시 반대로 구동되는 버그 수정.
 *          . Recipe 선택 안되는 버그 수정. 
 *          . Joint 수량 카운트 오류 : Vision에서는 2 로 입력 DataResult 에서는 3 으로 판단함, -> DataResult 를 2번으로 수정. 완료.
 *          . Result data load 시 raw 데이터 정보가 없을 경우 바로 리턴하도록 수정. (읽기오류 발생)
 *      . Sequence :
 *          . 티칭스캔/조그 Feeding시 쿨링에어 및 이오나이저 자동 On 기능 추가
 *          . Verify Disable 시 실시간 NG Image Display 기능 필요. -->> 방법 : Review 의 listNG 를 무조건 저장 -> Timer 에서 listNG.Count > 0 경우 Modify 진행 (Top>Bottom>Mono 순으로 데이터 입력)
 *          
 * 
 * - 16.06.02.2220
 *      . Sequence :    
 *          . Job End 처리 추가 (마지막 End Unit 이 Punch 의 초기 위치에서 정지 한 후 완공 보고)
 *          . Top 에서 Job End 신호입력 시 이전의 Joint(=2)인식 유닛을 모두 END 처리            . 
 *      . Review :
 *          . 데이터 확인 완료 (Total 데이터 연계 확인)
 *      . Data : 
 *          . Server Path 추가
 *          . Review 관련 파라미터 추가(ReviewImageSize, ReviewImageNumber, ReviewImageNumber)
 *      . GUI : 
 *          . Setup Page : Server Path 설정 추가
 *          . Auto Page : Verify Enable/Disable 설정기능 추가.
 * 
- 16.06.01.20
    . Review : 데이터 및 이미지 확인중, 
    . Data : DataSystem.ToolID 추가
    . GUI : SystemSetupPage -> ToolID 입력창 추가
            AutoPage -> ToolID 입력창 추가.
 
- 16.05.31.22
    . Review : GUI 완료
    . Punch 데이터 확인시 Review 데이터로 확인.
 
- 16.05.29.18
    . 조명 컨트롤 : 자동검사 및 티칭 일 경우에만 켜지도록 변경.
    . Review : 리뷰 윈도우 작업시작.
 
- 16.05.28.18
    . 펀치 시퀀스 : 스텝이동 -> 연속구동(펀치시에만 정지 하도록 수정)
*/