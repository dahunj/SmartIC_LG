namespace SmartICAVI
{
    class CntVirtual : ICnt
    {
        #region Interfaces
        protected bool isInitialized = false;
        public bool IsInitialized
        {
            get { return isInitialized; }
        }

        public int Initialize()
        {
            isInitialized = true;
            return 0;
        }

        public int UnInitialize()
        {
            isInitialized = false;

            return 0;
        }

        public int SetTriggerEnable(int channel)
        {
            return 0;
        }

        public int ResetTriggerEnable(int channel)
        {
            return 0;
        }

        public int SetPosition(int channel, double pos)
        {
            return 0;
        }

        public int SetTriggerParams(int channel, double row, double high, double period, double width, int level = 0, int dir = 1)
        {
            return 0;
        }
        #endregion
    }
}
