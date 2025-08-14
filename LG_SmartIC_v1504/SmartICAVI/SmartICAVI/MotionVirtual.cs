namespace SmartICAVI
{
    class MotionVirtual : IMotion
    {
        #region Interfaces
        private bool isInitialized = false;

        public bool IsInitialized
        {
            get { return isInitialized; }
        }

        public int InitOnly()
        {
            return 0;
        }

        public int Initialize()
        {
            isInitialized = true;

            Log_Trace.WriteLine("MotionVirtual.Initialize().InitializeCAMCFS20()");

            return 0;
        }

        public int UnInitialize()
        {
            isInitialized = false;
                        
            return 0;
        }

        public int ServoOn(int axis, bool servoOn)
        {
            return 0;
        }

        public int GetStateServoOn(int axis)
        {
            return 0;
        }

        public int AmpReset(int axis, bool on)
        {
            return 0;
        }

        public int HomeSearch(int axis)
        {
            
            return 0;
        }

        public int AMove(int axis, double pos, double vel, double accel)
        {
            return 0;
        }

        public int RMove(int axis, double pos, double vel, double accel)
        {
            return 0;
        }

        public int JogP(int axis, double vel, double accel)
        {
            return 0;
        }

        public int JogN(int axis, double vel, double accel)
        {
            return 0;
        }

        public int Stop(int axis)
        {
            return 0;
        }

        public int EStop(int axis)
        {
            return 0;
        }

        public int Reset()
        {
            return 0;
        }

        public int SeqMove(int axis, int seq)
        {
            return 0;
        }

        public bool IsMotionDone(int axis)
        {
            return true;
        }

        public int SetPosition(int axis, double pos)
        {
            return 0;
        }

        public int SetLimitEnable(int axis, bool enable)
        {
            return 0;
        }

        public double GetCurrentPosition(int axis)
        {
            return 0.0;
        }

        public int GetStateLimitP(int axis)
        {
            return 0;
        }

        public int GetStateLimitN(int axis)
        {
            return 0;
        }

        public int GetStateInposition(int axis)
        {
            return 0;
        }

        public int GetStateAmpAlarm(int axis)
        {
            return 0;
        }

        public int GetStateHome(int axis)
        {
            return 0;
        }

        public int GetStateHomeDone(int axis)
        {
            return 1;
        }

        public int SetVirtual(int command, int value, double value2 = 0.0)
        {
            return 0;
        }

        public int ApplyParams()
        {
            return 0;
        }

        public int SetOverridePos(int axis, double pos)
        {
            return 0;
        }

        public int SetOverrideVel(int axis, double vel)
        {
            return 0;
        }
        #endregion
    }
}
