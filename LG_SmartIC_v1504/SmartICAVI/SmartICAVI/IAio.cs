namespace SmartICAVI
{
    interface IAio
    {
        bool IsInitialized { get; }

        int Initialize();
        int UnInitialize();

        int SetRange(int channel, double min, double max);
        int GetRange(int channel, ref double min, ref double max);
        int SetOutvalue(int channel, double value);
        double GetOutvalue(int channel);
    }
}
