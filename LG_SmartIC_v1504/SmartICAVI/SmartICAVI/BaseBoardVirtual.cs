namespace SmartICAVI
{
    class BaseBoardVirtual : IBaseBoard
    {

        public bool IsInitialized
        {
            get
            {
                return true;
            }
        }

        public int Initialize()
        {
            return 0;
        }

        public int UnInitialize()
        {
            return 0;
        }
    }
}
