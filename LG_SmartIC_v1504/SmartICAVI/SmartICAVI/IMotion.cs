namespace SmartICAVI
{
    interface IMotion
    {
        bool IsInitialized { get; }
        int InitOnly();
        int Initialize();
        int UnInitialize();
        int ServoOn(int axis, bool servoOn);
        int HomeSearch(int axis);
        int AMove(int axis, double pos, double vel, double accel);
        int RMove(int axis, double pos, double vel, double accel);
        int JogP(int axis, double vel, double accel);
        int JogN(int axis, double vel, double accel);
        int Stop(int axis);
        int EStop(int axis);
        int Reset();
        int SeqMove(int axis, int seq);
        int AmpReset(int axis, bool on);
        bool IsMotionDone(int axis);

        int SetPosition(int axis, double pos);
        int SetLimitEnable(int axis, bool enable);

        double GetCurrentPosition(int axis);
        int GetStateLimitP(int axis);
        int GetStateLimitN(int axis);
        int GetStateInposition(int axis);
        int GetStateAmpAlarm(int axis);
        int GetStateHome(int axis);
        int GetStateHomeDone(int axis);
        int GetStateServoOn(int axis);

        int ApplyParams();

        int SetOverridePos(int axis, double pos);
        int SetOverrideVel(int axis, double vel);

        int SetVirtual(int command, int value, double value2 = 0.0);
    }
}
