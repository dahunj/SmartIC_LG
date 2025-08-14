namespace SmartICAVI
{
    class DataMotionParams
    {
        #region Variables
        private int Axis { get; set; }

        public int LevelLimitP { get; set; }
        public int LevelLimitN { get; set; }
        public int LevelInpos { get; set; }
        public int LevelAlarm { get; set; }

        public int UseInpos { get; set; }
        public int UseAlarm { get; set; }

        public int PulseOut { get; set; }
        public int EncoderType { get; set; }

        public double UnitPulse { get; set; }
        public double StartSpeed { get; set; }
        
        public double VelSlow { get; set; }
        public double AccelSlow { get; set; }
        public double VelNormal { get; set; }
        public double AccelNormal { get; set; }
        public double VelMove { get; set; }
        public double AccelMove { get; set; }
        public double VelRapid { get; set; }
        public double AccelRapid { get; set; }

        public double InitOffset { get; set; }
        public double InitPos { get; set; }

        public double Voffset { get; set; } // 아날로그 옵셋값
        public double Vjog { get; set; }    // 아날로그 조그값
        public double Kp0 { get; set; }    // 초기 구동시
        public double Ki0 { get; set; }    // 초기 구동시
        public double Kd0 { get; set; }    // 초기 구동시
        public double Vmin0 { get; set; }  // 모터가 구동되는 최소 토크값
        public double Vmax0 { get; set; }  // 모터가 구동되는 최대 토크값
        public double Inpos0 { get; set; } // Inposition 값을 설정한다. 이값 이하로는 제어하지 않는다. 

        public double Kp { get; set; }
        public double Ki { get; set; }
        public double Kd { get; set; }
        public double Vmin { get; set; }
        public double Vmax { get; set; }
        public double Inpos { get; set; }
        #endregion

        public DataMotionParams(int axis)
        {
            Axis = axis;

            Initialize();
        }

        public void Initialize()
        {
            LevelLimitP = 0;
            LevelLimitN = 0;
            LevelInpos = 0;
            LevelAlarm = 0;

            UseInpos = 1;
            UseAlarm = 1;

            PulseOut = 6;
            EncoderType = 1;

            UnitPulse = 0.001;
            StartSpeed = 1.0;

            VelSlow = 10.0;
            AccelSlow = 40.0;
            VelNormal = 50.0;
            AccelNormal = 200.0;
            VelMove = 120.0;
            AccelMove = 480.0;
            VelRapid = 300.0;
            AccelRapid = 1000.0;

            InitOffset = 1.0;
            InitPos = 0.0;

            Voffset = 0.0;
            Vjog = 0.0;

            Kp0 = 0.1;
            Ki0 = 0.015;
            Kd0 = 0.001;
            Vmin0 = 0.35;
            Vmax0 = 1.0;
            Inpos0 = 0.1;

            Kp = 0.1;
            Ki = 0.015;
            Kd = 0.001;
            Vmin = 0.35;
            Vmax = 1.0;
            Inpos = 0.1;
        }

        public int Load(string path)
        {
            IniFile ini = new IniFile();
            string section = string.Format("AXIS_{0:00}", Axis);

            LevelLimitP = int.Parse(ini.Read(section, "LevelLimitP", LevelLimitP.ToString(), path) );
            LevelLimitN = int.Parse(ini.Read(section, "LevelLimitN", LevelLimitN.ToString(), path) );
            LevelInpos = int.Parse(ini.Read(section, "LevelInpos", LevelInpos.ToString(), path) );
            LevelAlarm = int.Parse(ini.Read(section, "LevelAlarm", LevelAlarm.ToString(), path) );
            UseInpos = int.Parse(ini.Read(section, "UseInpos", UseInpos.ToString(), path) );
            UseAlarm = int.Parse(ini.Read(section, "UseAlarm", UseAlarm.ToString(), path) );

            PulseOut = int.Parse(ini.Read(section, "PulseOut", PulseOut.ToString(), path) );
            EncoderType = int.Parse(ini.Read(section, "EncoderType", EncoderType.ToString(), path) );

            UnitPulse = double.Parse(ini.Read(section, "UnitPulse", UnitPulse.ToString(), path) );
            StartSpeed = double.Parse(ini.Read(section, "StartSpeed", StartSpeed.ToString(), path) );


            VelSlow = double.Parse(ini.Read(section, "VelSlow", VelSlow.ToString(), path) );
            AccelSlow = double.Parse(ini.Read(section, "AccelSlow", AccelSlow.ToString(), path) );
            VelNormal = double.Parse(ini.Read(section, "VelNormal", VelNormal.ToString(), path) );
            AccelNormal = double.Parse(ini.Read(section, "AccelNormal", AccelNormal.ToString(), path) );
            VelMove = double.Parse(ini.Read(section, "VelMove", VelMove.ToString(), path) );
            AccelMove = double.Parse(ini.Read(section, "AccelMove", AccelMove.ToString(), path) );
            VelRapid = double.Parse(ini.Read(section, "VelRapid", VelRapid.ToString(), path) );
            AccelRapid = double.Parse(ini.Read(section, "AccelRapid", AccelRapid.ToString(), path) );

            InitOffset = double.Parse(ini.Read(section, "InitOffset", InitOffset.ToString(), path) );
            InitPos = double.Parse(ini.Read(section, "InitPos", InitPos.ToString(), path) );

            Voffset = double.Parse(ini.Read(section, "Voffset", Voffset.ToString(), path));
            Vjog = double.Parse(ini.Read(section, "Vjog", Vjog.ToString(), path));

            Kp0 = double.Parse(ini.Read(section, "Kp0", Kp0.ToString(), path) );
            Ki0 = double.Parse(ini.Read(section, "Ki0", Ki0.ToString(), path) );
            Kd0 = double.Parse(ini.Read(section, "Kd0", Kd0.ToString(), path) );
            Vmin0 = double.Parse(ini.Read(section, "Vmin0", Vmin0.ToString(), path) );
            Vmax0 = double.Parse(ini.Read(section, "Vmax0", Vmax0.ToString(), path) );
            Inpos0 = double.Parse(ini.Read(section, "Inpos0", Inpos0.ToString(), path) );

            Kp = double.Parse(ini.Read(section, "Kp", Kp.ToString(), path) );
            Ki = double.Parse(ini.Read(section, "Ki", Ki.ToString(), path) );
            Kd = double.Parse(ini.Read(section, "Kd", Kd.ToString(), path) );
            Vmin = double.Parse(ini.Read(section, "Vmin", Vmin.ToString(), path) );
            Vmax = double.Parse(ini.Read(section, "Vmax", Vmax.ToString(), path) );
            Inpos = double.Parse(ini.Read(section, "Inpos", Inpos.ToString(), path) );

            return 0;
        }

        public int Save(string path)
        {
            IniFile ini = new IniFile();
            string section = string.Format("AXIS_{0:00}", Axis);

            CheckData(ini, section, "LevelLimitP", LevelLimitP.ToString(), path);
            CheckData(ini, section, "LevelLimitN", LevelLimitN.ToString(), path);
            CheckData(ini, section, "LevelInpos", LevelInpos.ToString(), path);
            CheckData(ini, section, "LevelAlarm", LevelAlarm.ToString(), path);
            CheckData(ini, section, "UseInpos", UseInpos.ToString(), path);
            CheckData(ini, section, "UseAlarm", UseAlarm.ToString(), path);

            CheckData(ini, section, "PulseOut", PulseOut.ToString(), path);
            CheckData(ini, section, "EncoderType", EncoderType.ToString(), path);

            CheckData(ini, section, "UnitPulse", UnitPulse.ToString(), path);
            CheckData(ini, section, "StartSpeed", StartSpeed.ToString(), path);


            CheckData(ini, section, "VelSlow", VelSlow.ToString(), path);
            CheckData(ini, section, "AccelSlow", AccelSlow.ToString(), path);
            CheckData(ini, section, "VelNormal", VelNormal.ToString(), path);
            CheckData(ini, section, "AccelNormal", AccelNormal.ToString(), path);
            CheckData(ini, section, "VelMove", VelMove.ToString(), path);
            CheckData(ini, section, "AccelMove", AccelMove.ToString(), path);
            CheckData(ini, section, "VelRapid", VelRapid.ToString(), path);
            CheckData(ini, section, "AccelRapid", AccelRapid.ToString(), path);

            CheckData(ini, section, "InitOffset", InitOffset.ToString(), path);
            CheckData(ini, section, "InitPos", InitPos.ToString(), path);

            CheckData(ini, section, "Voffset", Voffset.ToString(), path);
            CheckData(ini, section, "Vjog", Vjog.ToString(), path);

            CheckData(ini, section, "Kp0", Kp0.ToString(), path);
            CheckData(ini, section, "Ki0", Ki0.ToString(), path);
            CheckData(ini, section, "Kd0", Kd0.ToString(), path);
            CheckData(ini, section, "Vmin0", Vmin0.ToString(), path);
            CheckData(ini, section, "Vmax0", Vmax0.ToString(), path);
            CheckData(ini, section, "Inpos0", Inpos0.ToString(), path);

            CheckData(ini, section, "Kp", Kp.ToString(), path);
            CheckData(ini, section, "Ki", Ki.ToString(), path);
            CheckData(ini, section, "Kd", Kd.ToString(), path);
            CheckData(ini, section, "Vmin", Vmin.ToString(), path);
            CheckData(ini, section, "Vmax", Vmax.ToString(), path);
            CheckData(ini, section, "Inpos", Inpos.ToString(), path);

            #region OLD
            //ini.Write(section, "LevelLimitP", LevelLimitP.ToString(), path);
            //ini.Write(section, "LevelLimitN", LevelLimitN.ToString(), path);
            //ini.Write(section, "LevelInpos", LevelInpos.ToString(), path);
            //ini.Write(section, "LevelAlarm", LevelAlarm.ToString(), path);
            //ini.Write(section, "UseInpos", UseInpos.ToString(), path);
            //ini.Write(section, "UseAlarm", UseAlarm.ToString(), path);

            //ini.Write(section, "PulseOut", PulseOut.ToString(), path);
            //ini.Write(section, "EncoderType", EncoderType.ToString(), path);

            //ini.Write(section, "UnitPulse", UnitPulse.ToString(), path);
            //ini.Write(section, "StartSpeed", StartSpeed.ToString(), path);


            //ini.Write(section, "VelSlow", VelSlow.ToString(), path);
            //ini.Write(section, "AccelSlow", AccelSlow.ToString(), path);
            //ini.Write(section, "VelNormal", VelNormal.ToString(), path);
            //ini.Write(section, "AccelNormal", AccelNormal.ToString(), path);
            //ini.Write(section, "VelMove", VelMove.ToString(), path);
            //ini.Write(section, "AccelMove", AccelMove.ToString(), path);
            //ini.Write(section, "VelRapid", VelRapid.ToString(), path);
            //ini.Write(section, "AccelRapid", AccelRapid.ToString(), path);

            //ini.Write(section, "InitOffset", InitOffset.ToString(), path);
            //ini.Write(section, "InitPos", InitPos.ToString(), path);

            //ini.Write(section, "Voffset", Voffset.ToString(), path);
            //ini.Write(section, "Vjog", Vjog.ToString(), path);

            //ini.Write(section, "Kp0", Kp0.ToString(), path);
            //ini.Write(section, "Ki0", Ki0.ToString(), path);
            //ini.Write(section, "Kd0", Kd0.ToString(), path);
            //ini.Write(section, "Vmin0", Vmin0.ToString(), path);
            //ini.Write(section, "Vmax0", Vmax0.ToString(), path);
            //ini.Write(section, "Inpos0", Inpos0.ToString(), path);

            //ini.Write(section, "Kp", Kp.ToString(), path);
            //ini.Write(section, "Ki", Ki.ToString(), path);
            //ini.Write(section, "Kd", Kd.ToString(), path);
            //ini.Write(section, "Vmin", Vmin.ToString(), path);
            //ini.Write(section, "Vmax", Vmax.ToString(), path);
            //ini.Write(section, "Inpos", Inpos.ToString(), path);
            #endregion

            return 0;
        }

        private void CheckData(IniFile ini, string section, string key, string value, string path)
        {
            string read = "";

            read = ini.Read(section, key, "0", path);
            if (read != value)
            {
                Log_History.WriteLine("{0,-30},{1,-30},{2,-30},{3,-30}", "[DataMotionParams]", section + "_" + key, read, value);
                ini.Write(section, key, value, path);
            }
        }
    }

    class DataMotion
    {
        private DataMotionParams[] motions;
        
        private string Path { get; set; }

        public int Length { get{ return motions.Length; } }
       
        public DataMotion()
        {
            Path = "./Data/Ini/Motion.ini";

            motions = new DataMotionParams[10];

            for (int i = 0; i < 10; ++i)
                motions[i] = new DataMotionParams(i);
        }

        public DataMotionParams this[int index]
        {
            get
            {
                if (0 <= index && motions.Length > index)
                    return motions[index];
                else
                    return null;
            }
            set
            {
                if (0 <= index && motions.Length > index)
                    motions[index] = value;
            }
        }

        public int Save(string path)
        {
            Path = path;

            return Save();
        }

        public int Load(string path)
        {
            Path = path;

            return Load();
        }

        public int Save()
        {
            for (int i = 0; i < motions.Length; ++i)
                motions[i].Save(Path);

            return 0;
        }

        public int Load()
        {
            for (int i = 0; i < motions.Length; ++i)
                motions[i].Load(Path);

            return 0;
        }
        
    }
}
