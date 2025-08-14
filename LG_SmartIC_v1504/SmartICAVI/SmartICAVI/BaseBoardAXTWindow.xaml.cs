using System;
using System.Windows;
using System.Windows.Interop;

namespace SmartICAVI
{
    /// <summary>
    /// BaseBoardAXTWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BaseBoardAXTWindow : Window
    {
        private IntPtr handle;
        
        public BaseBoardAXTWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Visibility = System.Windows.Visibility.Hidden;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        bool isInitialized = false;

        public new bool IsInitialized { get { return isInitialized; } }

        public int Initialize()
        {
            isInitialized = false;

            handle = new WindowInteropHelper(this).Handle;

            // AJIN Library 가 초기화 되어있지 않았을 경우 초기화 한다. 
            if (1 != CAxtLib.AxtIsInitialized())
            {
                if (1 != CAxtLib.AxtInitialize(handle, 0))
                {
                    Log_Trace.WriteLine("Error BaseBoardAXTWindow.Initialize().AxtInitialize()");
                    return -1;
                }
            }

            // 사용하는 베이스보드에 맞추어 Device를 Open하면 됩니다.
            // BUSTYPE_ISA					:	0
            // BUSTYPE_PCI					:	1
            // BUSTYPE_VME					:	2
            // BUSTYPE_CPCI(Compact PCI)	:	3

            if (CAxtLib.AxtIsInitializedBus(1) == 0)			// 지정한 버스(PCI)가 초기화 되었는지를 확인한다
            {
                if (CAxtLib.AxtOpenDeviceAuto(1) == 0)			// 새로운 베이스보드를 자동으로 통합라이브러리에 추가한다
                {
                    Log_Trace.WriteLine("Error BaseBoardAXTWindow.Initialize().AxtOpenDeviceAuto()");

                    return -1;
                }
            }

            return 0;
        }

        public int UnInitialize()
        {
            isInitialized = false;

            // BUSTYPE_ISA					:	0
            // BUSTYPE_PCI					:	1
            // BUSTYPE_VME					:	2
            // BUSTYPE_CPCI(Compact PCI)	:	3

            if (1 == CAxtLib.AxtIsInitializedBus(1))	// 보드가 활성화 되어있느면, 닫아준다. 
            {
                Log_Trace.WriteLine("BaseBoardAXTWindow.UnInitialize().AxtCloseDeviceAll()");
                CAxtLib.AxtCloseDeviceAll();
            }

            if (1 == CAxtLib.AxtIsInitialized())	// AJIN Library 가 초기화 되었있을 경우 닫아준다. 
            {
                Log_Trace.WriteLine("BaseBoardAXTWindow.UnInitialize().AxtClose()");
                CAxtLib.AxtClose();
            }

            return 0;
        }
    }
}
