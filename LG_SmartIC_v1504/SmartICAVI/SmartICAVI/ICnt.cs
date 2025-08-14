namespace SmartICAVI
{
    interface ICnt
    {
        bool IsInitialized { get; }

        int Initialize();
        int UnInitialize();

        int SetTriggerEnable(int channel);
        int ResetTriggerEnable(int channel);

        int SetPosition(int channel, double pos);

        int SetTriggerParams(int channel, double row, double high, double period, double width, int level = 0, int dir = 1);
    }
}
