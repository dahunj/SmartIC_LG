namespace SmartICAVI
{
    class CntAXT : ICnt
    {
        DataService dataService = null;

        #region Interfaces
        protected bool isInitialized = false;
        public bool IsInitialized
        {
            get { return isInitialized; }
        }

        public int Initialize()
        {
            isInitialized = false;

            dataService = DataService.Singleton;

            int axisFeed = (int)EnumSmartIC.Axis.visionFeed;

            if (0 == AxtCNT.CNTIsInitialized())
            {
                if (1 == AxtCNT.InitializeCNT(1))
                    return -1;
            }

            for (int i = 0; i < 4; ++i)
            {
                AxtCNT.CNTset_trigger_mode((short)i, 1);    // 아진 버그로 인해 1로 세팅 이후 다시 0으로 세팅해야 함. 
                AxtCNT.CNTset_moveunit_perpulse((short)i, dataService.DataMotion[axisFeed].UnitPulse);
                AxtCNT.CNTset_enc_input_method((short)i, (byte)dataService.DataMotion[axisFeed].EncoderType);
                AxtCNT.CNTset_enc_source_sel((short)i, 0);
                AxtCNT.CNTset_trigger_mode((short)i, 0);
            }

            isInitialized = true;
            return 0;
        }

        public int UnInitialize()
        {
            isInitialized = false;

            ResetTriggerEnable(0);
            ResetTriggerEnable(1);
            ResetTriggerEnable(2);
            ResetTriggerEnable(3);

            return 0;
        }

        public int SetTriggerEnable(int channel)
        {
            if (false == IsInitialized)
                return -1;

            AxtCNT.CNTset_trigger_enable((short)channel);

            return 0;
        }

        public int ResetTriggerEnable(int channel)
        {
            if (false == IsInitialized)
                return -1;

            AxtCNT.CNTset_trigger_disable((short)channel);

            return 0;
        }

        public int SetPosition(int channel, double pos)
        {
            if (false == IsInitialized)
                return -1;

            AxtCNT.CNTset_pulse_counter((short)channel, pos);

            return 0;
        }

        public int SetTriggerParams(int channel, double row, double high, double period, double width, int level = 0, int dir = 1)
        {
            if (false == IsInitialized)
                return -1;

            short ch = (short)channel;

            AxtCNT.CNTset_trigger_mode(ch, 1);    // 아진 버그로 인해 1로 세팅 이후 다시 0으로 세팅해야 함. 
            
            AxtCNT.CNTset_trigger_block(ch, row, high, period);
            AxtCNT.CNTset_trigger_pulse_width(ch, width);
            AxtCNT.CNTset_trigger_active_level(ch, (byte)level);
            AxtCNT.CNTset_trigger_direction_check(ch, (byte)dir);

            AxtCNT.CNTset_trigger_mode(ch, 0); 

            return 0;
        }
        #endregion
    }
}
