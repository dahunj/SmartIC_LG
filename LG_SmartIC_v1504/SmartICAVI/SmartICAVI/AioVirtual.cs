namespace SmartICAVI
{
    class AioVirtual : IAio
    {
        #region Interfaces
        private bool isInitialized = false;

        public bool IsInitialized
        {
            get { return isInitialized; }
        }

        public int Initialize()
        {
            isInitialized = true;

            Log_Trace.WriteLine("AioVirtual.Initialize().InitializeAIO()");

            return 0;
        }

        public int UnInitialize()
        {
            return 0;
        }

        public int SetRange(int channel, double min, double max)
        {
            return 0;
        }

        public int GetRange(int channel, ref double min, ref double max)
        {
            return 0;
        }

        public int SetOutvalue(int channel, double value)
        {
            return 0;
        }

        public double GetOutvalue(int channel)
        {
            return 0;
        }
        #endregion
    }
}
