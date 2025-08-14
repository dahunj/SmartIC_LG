using HalconDotNet;

namespace SmartICAVI
{
    interface ISequence
    {
        bool IsInitialized { get; }
        int Mode { get; }

        int Initialize();
        int UnInitialize();

        int Start();
        int Pause();
        int Resume();
        int Stop();
        int End();
        int Reset();

        void RunProcess();

        int SetSequence(int seq, int type=0, double value = 0.0);

        int CheckMachine(int type=0, bool isStop = true);

        bool IsFlag(int flag);

        void SetHWindow(HWindow hWindow);

        int SetLight(int light, bool on);
    }
}
