namespace SmartICAVI
{
    class DataDefectInfo
    {
        private string Path { get; set; }

        public string[] IDs;                // MES IDs
        public string[] Defines;            // MES Defect names
        public string[] Names;              // EQ Defect names
        public int[] Accumulates;           // Defect Accumulate Count
        public bool[] IsShowMessages;    // Punch Stop -> Show Messsage

        public DataDefectInfo()
        {
            IDs = new string[63]{
                        "1",        "2",        "3",        "4",        "5",
                        "A",        "B",        "BB001",    "BB006",    "BB012",

                        "BB018",    "BB019",    "BB025",    "BB026",    "BB029",
                        "BB034",    "BB038",    "BB039",    "BB040",    "BB042",

                        "BB043",    "BB045",    "BB047",    "BB053",    "BB062",
                        "BB064",    "BB066",    "BB068",    "BB072",    "BB074",

                        "BB088",    "BB089",    "BB090",    "BB091",    "BB092",
                        "BB093",    "BB094",    "BB095",    "BB096",    "BB097",

                        "BB098",    "BB099",    "BB100",    "C",        "D",
                        "E",        "F",        "H",        "J",        "K",

                        "L",        "M",        "N",        "O",        "P",    
                        "Q",        "R",        "S",        "T",        "U",

                        "V",        "W",        "Y",    
            };

            Defines = new string[63]{
                            "Scratch(회로)",      "얼룩",           "이물(PI)",           "Scratch(PI)",              "TOP 패임",
                            "비금속 이물",        "SR B/O",         "Damage",             "기타이물",                 "미도금",
                            
                            "위치 어긋남",        "인쇄 불량",      "접착제 B/O",         "층간들뜸",                 "펀칭오염",
                            "Burr",               "FLEX 코팅불량",  "FLEX핀홀",           "Scratch(Emboss)",          "Scratch(차폐판)",

                            "LEAD변형",           "Marking불량",    "Over 에칭",          "S/H 파손",                 "SR Filmy",
                            "T/L불량",            "Tool Mark",      "Warpage",            "미펀칭",                   "미에칭",

                            "V-cut불량",          "돌기",           "도금 Pin Hole",      "PI면 불량",                "FIDS 마크 불량",
                            "FIDS PI Scratch",    "CL 기포",        "CL 위치 어긋남",     "CL 하부 오염",             "CL 접착제 B/O",

                            "CL 하부 변색",       "CL 겹침",        "LEAD 단락",          "전공정(AOI Punching)불량", "Dent",
                            "금속이물",           "변색",           "SR 핀홀",            "잔류 Cu",                  "검은이물",

                            "도금불량",           "패임",           "SR 불량",            "Open",                     "돌출",
                            "SR Misalign",        "S/C AM",         "Short",              "SR 튐",                    "오염",

                            "기타",               "SR 기포",        "Punching Misalign",
            };

            Names = new string[63];
            Accumulates = new int[63];
            IsShowMessages = new bool[63];

            for (int i = 0; i < Names.Length; ++i)
            {
                Names[i] = Defines[i];
                Accumulates[i] = 1;
                IsShowMessages[i] = false;
            }
        }


        public void Load(string path)
        {
            Path = path;
            Load();
        }

        public void Save(string path)
        {
            Path = path;
            Save();
        }

        public void Load()
        {
            IniFile ini = new IniFile();
            string section = "DEFECT_INFO";

            for (int i = 0; i < IDs.Length; ++i)
            {
                section = string.Format("DEFECT_{0:000}", i+1);

                IDs[i] = ini.Read(section, "ID", IDs[i], Path);

                Defines[i] = ini.Read(section, "Define", Defines[i], Path);

                Names[i] = ini.Read(section, "Name", Names[i], Path);

                Accumulates[i] = int.Parse(ini.Read(section, "Accumulate", Accumulates[i].ToString(), Path));

                IsShowMessages[i] = bool.Parse(ini.Read(section, "IsShowMessage", IsShowMessages[i].ToString(), Path));
            }
        }

        public void Save()
        {
            IniFile ini = new IniFile();
            string section = "DEFECT_INFO";
            
            for (int i = 0; i < IDs.Length; ++i)
            {
                section = string.Format("DEFECT_{0:000}", i + 1);

                ini.Write(section, "ID", IDs[i], Path);

                ini.Write(section, "Define", Defines[i], Path);

                ini.Write(section, "Name", Names[i], Path);

                ini.Write(section, "Accumulate", Accumulates[i].ToString(), Path);

                ini.Write(section, "IsShowMessage", IsShowMessages[i].ToString(), Path);
            }
        }

    }
}
