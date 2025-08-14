namespace SmartICAVI
{
    interface IBaseBoard
    {
        bool IsInitialized { get; }
        int Initialize();
        int UnInitialize();
    }
}
