namespace SmartICAVI
{
    class BaseBoardAXT : IBaseBoard
    {
        private BaseBoardAXTWindow win = null;

        //bool isInitialized = false;

        public bool IsInitialized
        { 
            get 
            {
                if (null != win)
                    return win.IsInitialized;
                else
                    return false;
            } 
        }

        public int Initialize()
        {
            win = new BaseBoardAXTWindow();
            win.Show();
            win.Initialize();

            return 0;
        }

        public int UnInitialize()
        {
            if (null != win)
            {
                win.UnInitialize();
                win.Close();
                win = null;
            }
            return 0;
        }
    }
}
