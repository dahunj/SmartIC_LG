using HalconDotNet;

namespace SmartICAVI
{
    interface IGrab
    {
        bool IsConnected { get; }
        int Width { get; }
        int Height { get; }

        int Initialize();
        int UnInitialize();
        int Connect();
        int Disconnect();
        int Grab(out HObject HImage);

        int Reset();

        int Save(string path);
    }
}
