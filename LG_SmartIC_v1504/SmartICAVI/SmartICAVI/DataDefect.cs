using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SmartICAVI
{
    class DataDefectRaw
    {
        public string Path { get; set; }
        public string Verify { get; set; }
        public string VerifyImage {get; set;}

        public int Index { get; set; }
        
        public string VisionType { get; set; }
        public string LineType { get; set; }
        public string Delegate { get; set; }        // 2020.06.10 khs - 대표불량 표시
        public string DefectID { get; set; }
        public string DefectName { get; set; }

        public int DefectNumber { get; set; }
        public double Area { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public string ColorImage { get; set; }
        public string HSIImage { get; set; }
        public string BigImage { get; set; }

        public DataDefectRaw()
        {
            Path = "";
            Verify = "";
            VerifyImage = "";

            Index = -1;

            VisionType = "";
            LineType = "";
            Delegate = "";       // 2020.06.10 khs - 대표불량 표시
            DefectID = "";
            DefectName = "";
            
            DefectNumber = 0;
            Area = 0.0;
            Length = 0.0;
            Width = 0.0;
            Height = 0.0;
            X = 0.0;
            Y = 0.0;
            
            ColorImage = "";
            HSIImage = "";
            BigImage = "";
        }
    }
        
    class DataDefectIndex
    {
        private List<DataDefectRaw> listRaw;

        public string PathImage { get; set; }
        public int Index { get; set; }
        public string Line { get; set; }
        public string Verify { get; set; }
        public string VerifyVision { get; set; }
        public string VerifyImage { get; set; }

        public List<DataDefectRaw> ListRaw { get { return listRaw; } }

        public DataDefectIndex()
        {
            listRaw = new List<DataDefectRaw>();
            Index = -1;
            PathImage = "";
        }

        public void Clear()
        {
            listRaw.Clear();
            Index = -1;
            Verify = "";
            VerifyVision = "";
            VerifyImage = "";
        }

        public void Add(int index, string path, string visionType, string lineType, string defectID, string defectName, double area, double length, double width, double height, double x, double y, string colorImage, string hsiImage, string bigImage)
        {
            DataDefectRaw raw = new DataDefectRaw();
            raw.Path = path;
            raw.Index = index;

            raw.VisionType = visionType;
            raw.LineType = lineType;
            //raw.DefectNumber = number;
            raw.DefectID = defectID;
            raw.DefectName = defectName;
            raw.Area = area;
            raw.Length = length;
            raw.Width = width;
            raw.Height = height;
            raw.X = x;
            raw.Y = y;
            raw.ColorImage = colorImage;
            raw.HSIImage = hsiImage;
            raw.BigImage = bigImage;

            listRaw.Add(raw);

            //System.Diagnostics.Debug.WriteLine("Defect add {0}, {1}, {2}, {3}", path, index, colorImage, hsiImage);
        }

        public void Save(StreamWriter writer, int no)
        {
            //string tempLine = "A";
            //string tempColor

            for (int i = 0; i < listRaw.Count; ++i)
            {
                //Index,Verify,VerifyVision,VisionType,LineType,DefectID,Area,Length,Width,Height,X,Y,Color,HSI,Big
                //writer.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6:0.000},{7:0.000},{8:0.000},{9:0.000},{10:0.000},{11:0.000},{12},{13},{14}",
                //    Index, Verify, VerifyVision, listRaw[i].VisionType, listRaw[i].LineType, listRaw[i].DefectID,
                //    listRaw[i].Area, listRaw[i].Length, listRaw[i].Width, listRaw[i].Height, listRaw[i].X, listRaw[i].Y,
                //    listRaw[i].ColorImage, listRaw[i].HSIImage, listRaw[i].BigImage));

                writer.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8:0.000},{9:0.000},{10:0.000},{11:0.000},{12:0.000},{13:0.000},{14},{15},{16},{17}",
                    no + 1, Index, Line, Verify, listRaw[i].Delegate,
                    listRaw[i].VisionType, listRaw[i].DefectID, listRaw[i].DefectName, listRaw[i].Area, listRaw[i].Length, listRaw[i].Width, listRaw[i].Height, listRaw[i].X, listRaw[i].Y,
                    listRaw[i].ColorImage, listRaw[i].HSIImage, listRaw[i].BigImage,
                    VerifyImage));
            }
        }

        public void MoveImages(string pathImage)
        {
            string source = "";
            string dest = "";

            for (int i = 0; i < listRaw.Count; ++i)
            {
                if( "" != listRaw[i].ColorImage )
                {
                    source = listRaw[i].Path + "\\" + listRaw[i].ColorImage;
                    dest = pathImage + "\\" + listRaw[i].VisionType +"\\" + listRaw[i].ColorImage;
                    File.Copy(source, dest, true);
                }

                if ("" != listRaw[i].HSIImage)
                {
                    source = listRaw[i].Path + "\\" + listRaw[i].HSIImage;
                    dest = pathImage + "\\" + listRaw[i].VisionType + "\\" + listRaw[i].HSIImage;
                    File.Copy(source, dest, true);
                }

                if ("" != listRaw[i].BigImage)
                {
                    source = listRaw[i].Path + "\\" + listRaw[i].BigImage;
                    dest = pathImage + "\\" + listRaw[i].VisionType + "\\" + listRaw[i].BigImage;
                    File.Copy(source, dest, true);
                }
            }
        }

        public void DeleteImages()
        {
            string source = "";
            
            for (int i = 0; i < listRaw.Count; ++i)
            {
                try
                {
                    if ("" != listRaw[i].ColorImage)
                    {
                        source = listRaw[i].Path + "\\" + listRaw[i].ColorImage;
                        File.Delete(source);
                        //System.Diagnostics.Debug.WriteLine(source);
                    }

                    if ("" != listRaw[i].HSIImage)
                    {
                        source = listRaw[i].Path + "\\" + listRaw[i].HSIImage;
                        File.Delete(source);
                        //System.Diagnostics.Debug.WriteLine(source);
                    }

                    if ("" != listRaw[i].BigImage)
                    {
                        source = listRaw[i].Path + "\\" + listRaw[i].BigImage;
                        File.Delete(source);
                        //System.Diagnostics.Debug.WriteLine(source);
                    }
                }
                catch (Exception exc)
                {
                    Log_Exception.WriteLine("DataDefectIndex.DeleteImage() : " + exc.Message);
                }
            }
        }

        public void DeleteFile()
        {
            string fileName = "";

            if ("A" == Line)
                fileName = string.Format("index{0:000000}_A.dat", Index);
            else
                fileName = string.Format("index{0:000000}_B.dat", Index);

            string path = PathImage + "\\" + fileName;

            if (true == File.Exists(path))
            {
                try
                {
                    File.Delete(path);

                    //System.Diagnostics.Debug.WriteLine(path);
                }
                catch (Exception exc)
                {
                    Log_Exception.WriteLine("DataDefectIndex.DeleteFile() : " + exc.Message);
                }
            }
        }
       
    }

    class DataAlign
    {
        public int Index { get; set; }    // 소재 Index Number 0 ~
        public string Line { get; set; }    // 라인 A열/B열
        public double X { get; set; }       // X 옵셋값
        public double Y { get; set; }       // Y 옵셋값
        public string ImageName { get; set; } // 이미지 이름

        public DataAlign(int index, string line, double x, double y, string imageName)
        {
            Index = index;
            Line = line;
            X = x;
            Y = y;
            ImageName = imageName;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2:0.000},{3:0.000},{4}", Index + 1, Line, X, Y, ImageName);
        }
    }

    class DataDefect
    {
        private string Ver { get; set; }
        public string RecipeName { get; set; }
        public string LotID { get; set; }
        public string UserID { get; set; }
        public string ToolID { get; set; }
        public string NextProcess { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int TotalUnits { get; set; }
        public int GoodUnits { get; set; }
        public int CNGCount { get; set; }
        public int PunchCount { get; set; }
        public int PunchTotalCount { get; set; }
        public int JointCount { get; set; }
        public int PF { get; set; }
        public int Line { get; set; }
        public string Comment { get; set; }
        public double Velocity { get; set; }

        public bool IsSaved { get; set; }

        public TimeSpan LightTimeTop { get; set; }
        public TimeSpan LightTimeBottom { get; set; }
        public TimeSpan LightTimeMono { get; set; }


        public int[] CountNGs;
        public string[] CountNGIDs;
        public string[] CountNGNames;

        public List<DataDefectIndex> ListIndex { get { return listIndex; } }
        private List<DataDefectIndex> listIndex;


        private List<DataAlign> listAlign;
        private List<DataAlign> listPunch;

        public List<DataAlign> ListAlign { get { return listAlign; } }
        public List<DataAlign> ListPunch { get { return listPunch; } }

        private List<IPoint> listLightTop;
        private List<IPoint> listLightBottom;
        private List<IPoint> listLightMono;


        public DataDefect()
        {
            Ver = "1.1";        // DefectName 추가

            listIndex = new List<DataDefectIndex>();

            listAlign = new List<DataAlign>();
            listPunch = new List<DataAlign>();

            listLightTop = new List<IPoint>();
            listLightBottom = new List<IPoint>();
            listLightMono = new List<IPoint>();

            LightTimeTop = new TimeSpan(0, 0, 0);
            LightTimeBottom = new TimeSpan(0, 0, 0);
            LightTimeMono = new TimeSpan(0, 0, 0);

            CountNGs = new int[63];

            CountNGIDs = new string[63]{
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

            CountNGNames = new string[63]{
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

            Clear();
        }

        public void Clear()
        {
            if (0 < listIndex.Count)
            {
                for (int i = 0; i < listIndex.Count; ++i)
                    listIndex[i].Clear();

                listIndex.Clear();
            }

            ListAlign.Clear();
            ListPunch.Clear();

            LightTimeTop = new TimeSpan(0, 0, 0);
            LightTimeBottom = new TimeSpan(0, 0, 0);
            LightTimeMono = new TimeSpan(0, 0, 0);

            RecipeName = "";
            LotID = "";
            UserID = "";
            ToolID = "";
            NextProcess = "";
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;

            TotalUnits = 0;
            GoodUnits = 0;
            CNGCount = 0;
            PunchCount = 0;
            JointCount = 0;
            PF = 0;
            Line = 2;
            Velocity = 1.0;

            IsSaved = false;

            Comment = "";

            for (int i = 0; i < CountNGs.Length; ++i)
                CountNGs[i] = 0;


            listLightBottom.Clear();
            listLightMono.Clear();
            listLightTop.Clear();

        }

        public void Add(int index, string path, string visionType, string lineType, string defectID, string defectName, double area, double length, double width, double height, double x, double y, string colorImage, string hsiImage, string bigImage)
        {
            int count = listIndex.Count;
            bool isCreate = false;

            if (0 == count)
            {
                isCreate = true;
            }
            else
            {
                if ((index != listIndex[count - 1].Index) || (lineType != listIndex[count - 1].Line))
                {
                    isCreate = true;

                    //System.Diagnostics.Debug.WriteLine("Index={0}, Line={1}", index, lineType);
                }
            }

            if (true == isCreate)
            {
                DataDefectIndex dataIndex = new DataDefectIndex();
                dataIndex.Index = index;
                dataIndex.Line = lineType;
                dataIndex.PathImage = path;
                dataIndex.Add(index, path, visionType, lineType, defectID, defectName, area, length, width, height, x, y, colorImage, hsiImage, bigImage);

                listIndex.Add(dataIndex);
            }
            else
            {
                listIndex[count - 1].Add(index, path, visionType, lineType, defectID, defectName, area, length, width, height, x, y, colorImage, hsiImage, bigImage);
            }
        }

        public void AddAlign(int index, string line, double x, double y, string imageName)
        {
            DataAlign align = new DataAlign(index, line, x, y, imageName);
            ListAlign.Add(align);

            string path = DataService.Singleton.DataSystem.PunchPath + "\\" + "index_align.txt";
            File.AppendAllText(path, string.Format("{0},{1},{2:0.000},{3:0.000},{4}\r\n", index+1, line, x, y, imageName), Encoding.Default);
        }

        public void AddPunchInspect(int index, string line, double x, double y, string imageName)
        {
            DataAlign punch = new DataAlign(index, line, x, y, imageName);
            ListPunch.Add(punch);

            string path = DataService.Singleton.DataSystem.PunchPath + "\\" + "index_punch.txt";
            File.AppendAllText(path, string.Format("{0},{1},{2:0.000},{3:0.000},{4}\r\n", index+1, line, x, y, imageName), Encoding.Default);
        }

        public void Verify(int index, string line, string verify)
        {
            if (0 < listIndex.Count)
            {
                for (int i = 0; i < listIndex.Count; ++i)
                {
                    if ((index + 1) == listIndex[i].Index)
                    {
                        if( line == listIndex[i].Line )
                        {
                            if ("G" == verify)
                            {
                                listIndex[i].Verify = "OK";
                            }
                            else
                            {
                                listIndex[i].Verify = "NG";
                            }

                            //listIndex[i].VerifyVision = "ALL";
                            listIndex[i].VerifyVision = "";

                            for (int k = 0; k < listIndex[i].ListRaw.Count; ++k)
                            {
                                if ("G" == verify)
                                {
                                    listIndex[i].Verify = "OK";
                                }
                                else
                                {
                                    listIndex[i].Verify = "NG";

                                    if (listIndex[i].ListRaw[k].DefectID == verify)
                                    {
                                        listIndex[i].ListRaw[k].Delegate = "V";
                                    }
                                }
                            }

                            return;
                        }
                    }
                }
            }
        }

        public void VerifyImage(int index, string line, string imageName, string verify)
        {
            if (0 < listIndex.Count)
            {
                bool isFound = false;

                for (int i = 0; i < listIndex.Count; ++i)
                {
                    if ((index + 1) == listIndex[i].Index)
                    {
                        if (line == listIndex[i].Line)
                        {
                            for (int k = 0; k < listIndex[i].ListRaw.Count; ++k)
                            {
                                if (imageName == listIndex[i].ListRaw[k].ColorImage)
                                {
                                    if ("G" != verify)
                                    {
                                        //listIndex[i].Verify = verify;
                                        //listIndex[i].VerifyVision = m_listIndex[i].ListRaw[k].VisionType;
                                        listIndex[i].Verify = "";
                                        listIndex[i].VerifyVision = "";
                                        listIndex[i].VerifyImage = imageName;

                                        isFound = true;
                                    }
                                    //listIndex[i].ListRaw[k].Verify = verify;
                                    listIndex[i].ListRaw[k].Verify = "";
                                    break;
                                }
                            }

                            // Good 이 아닐경우 나머지 모두 Verify 적용한다.
                            if (true == isFound)
                            {
                                for (int k = 0; k < listIndex[i].ListRaw.Count; ++k)
                                {
                                    if ("G" != listIndex[i].ListRaw[k].Verify)
                                    {
                                        //listIndex[i].ListRaw[k].Verify = verify;
                                        listIndex[i].ListRaw[k].Verify = "";
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void VerifyAuto(int index, string line, string verifyVision, string verify)
        {
            if (0 < listIndex.Count)
            {
                for (int i = 0; i < listIndex.Count; ++i)
                {
                    if ((index + 1) == listIndex[i].Index)
                    {
                        if (line == listIndex[i].Line)
                        {
                            //m_listIndex[i].Verify = verify;
                            listIndex[i].Verify = "";
                            listIndex[i].VerifyVision = "";

                            for (int k = 0; k < listIndex[i].ListRaw.Count; ++k)
                            {
                                //listIndex[i].Verify = verify;
                                listIndex[i].Verify = "";

                                if (listIndex[i].ListRaw[k].DefectID == verify)
                                {
                                    listIndex[i].ListRaw[k].Delegate = "V";
                                }
                            }

                            return;
                        }
                    }
                }
            }
        }

        public void AddLightTop(int index, int value)
        {
            IPoint pt = new IPoint(index, value);
            listLightTop.Add(pt);
        }

        public void AddLightBottom(int index, int value)
        {
            IPoint pt = new IPoint(index, value);
            listLightBottom.Add(pt);
        }

        public void AddLightMono(int index, int value)
        {
            IPoint pt = new IPoint(index, value);
            listLightMono.Add(pt);
        }

        // index 까지의 데이터를 모두 지운다.
        public void Restart(int index)
        {
            if (0 > index)
                index = 0;

            int length = ListIndex.Count;

            for (int i = length - 1; i >= index; --i)
            {
                //System.Diagnostics.Debug.WriteLine("Restart Index={0}, Line={1}", ListIndex[i].Index, ListIndex[i].Line);

                ListIndex[i].DeleteFile();
                ListIndex[i].DeleteImages();
                ListIndex[i].Clear();
                ListIndex.RemoveAt(i);
            }
        }

        public void Save(string path) // server path
        {
            GC.Collect();

            if ("" == RecipeName)
                RecipeName = "NoDefine";
            if ("" == LotID)
                LotID = "NoDefine";

            if ("" == path)
                return;

            if (false == Directory.Exists(path))
                return;

            if( '\\' != path[path.Length - 1] )
                path += "\\";

            string pathModel = path + RecipeName;
            string pathDay = pathModel + "\\" + DateTime.Now.ToString("yyMMdd");
            string pathLot = pathDay + "\\" + LotID;
            

            // Create Model Directory
            if (false == Directory.Exists(pathModel))
                Directory.CreateDirectory(pathModel);

            // Create Day Directory
            if (false == Directory.Exists(pathDay))
                Directory.CreateDirectory(pathDay);

            // Create Lot Directory
            if (true == Directory.Exists(pathLot))
            {
                pathLot += DateTime.Now.ToString("_yyMMdd_HHmmss");
            }
            Directory.CreateDirectory(pathLot);

            string pathImages = pathLot + "\\" + "Images";
            string pathTop = pathImages + "\\" + "TOP";
            string pathBottom = pathImages + "\\" + "BOTTOM";
            string pathMono = pathImages + "\\" + "MONO";
            //string pathAlign = pathImages + "\\" + "ALIGN";
            string pathPunch = pathImages + "\\" + "PUNCH";

            string pathMap = pathLot + "\\" + "MapData";

            DataService.Singleton.ServerMapPath = pathMap;

            //string pathMaster = pathLot + "\\" + "Master";
            //string pathMasterTop = pathMaster + "\\" + "TOP";
            //string pathMasterBottom = pathMaster + "\\" + "BOTTOM";
            //string pathMasterMono = pathMaster + "\\" + "MONO";

            // Create Image Directory
            Directory.CreateDirectory(pathImages);
            Directory.CreateDirectory(pathTop);
            Directory.CreateDirectory(pathBottom);
            Directory.CreateDirectory(pathMono);
            //Directory.CreateDirectory(pathAlign);
            Directory.CreateDirectory(pathPunch);

            Directory.CreateDirectory(pathMap);

            //Directory.CreateDirectory(pathMaster);
            //Directory.CreateDirectory(pathMasterTop);
            //Directory.CreateDirectory(pathMasterBottom);
            //Directory.CreateDirectory(pathMasterMono);

            TimeSpan span = EndTime - StartTime;

            
            // Save Index & Raw Data
            string pathName = pathLot + "\\" + LotID + ".def";
            
            FileStream stream;
            StreamWriter writer;
            //string item;

            stream = File.Create(pathName);
            writer = new StreamWriter(stream, Encoding.Default);

            writer.WriteLine(string.Format("FileVersion,{0}", Ver));
            writer.WriteLine(string.Format("ModelName,{0}", RecipeName));
            writer.WriteLine(string.Format("LotID,{0}", LotID));
            writer.WriteLine(string.Format("UserID,{0}", UserID));
            writer.WriteLine(string.Format("ToolID,{0}", ToolID));
            writer.WriteLine(string.Format("PF,{0}", PF));
            writer.WriteLine(string.Format("NextProcess,{0}", NextProcess));

            writer.WriteLine(string.Format("StartTime,{0}", StartTime));
            writer.WriteLine(string.Format("EndTime,{0}", EndTime));
            writer.WriteLine(string.Format("RunTime,{0}", span));

            writer.WriteLine(string.Format("TotalUnits,{0}", TotalUnits));
            writer.WriteLine(string.Format("GoodUnits,{0}", GoodUnits));
            writer.WriteLine(string.Format("NGUnits,{0}", TotalUnits-GoodUnits));
            writer.WriteLine(string.Format("Yield,{0:0.000}", ((double)GoodUnits/(double)TotalUnits)*100.0));
            writer.WriteLine(string.Format("CNGCount,{0}", CNGCount));
            writer.WriteLine(string.Format("PunchCount,{0}", PunchCount));
            writer.WriteLine(string.Format("PunchTotalCount,{0}", PunchTotalCount));
            writer.WriteLine(string.Format("JointCount,{0}", JointCount));
            writer.WriteLine(string.Format("Velocity,{0:0.000}", Velocity));

            writer.WriteLine(string.Format("LightTimeTop,{0}", LightTimeTop.ToString()));
            writer.WriteLine(string.Format("LightTimeBottom,{0}", LightTimeBottom.ToString()));
            writer.WriteLine(string.Format("LightTimeMono,{0}", LightTimeMono.ToString()));
            
            writer.WriteLine();

            writer.WriteLine("DEFECT_COUNT");
            writer.WriteLine("NO,DEFECTID,DEFECTNAME,COUNT");
            for (int i = 0; i < CountNGs.Length; ++i)
            {
                writer.WriteLine("{0},{1},{2},{3}", i+1, CountNGIDs[i], CountNGNames[i], CountNGs[i]);
            }
            
            writer.WriteLine("ENDSEC");

            writer.WriteLine("");

            writer.WriteLine("DEFECT");
            writer.WriteLine("NO,INDEX,LINE,VERIFY,DELEGATE,VISIONTYPE,DEFECTID,DEFECTNAME,AREA,LENGTH,WIDTH,HEIGHT,X,Y,COLORIMAGE,HSIIMAGE,BIGIMAGE,VERIFYIMAGE");
            if (0 < listIndex.Count)
            {
                for (int i = 0; i < listIndex.Count; ++i)
                {
                    listIndex[i].Save(writer, i);
                }
            }
            writer.WriteLine("ENDSEC");
            writer.WriteLine();

            writer.WriteLine("LIGHT_TOP");
            writer.WriteLine("INDEX,LIGHTVALUE");
            if (0 < listLightTop.Count)
            {
                for (int i = 0; i < listLightTop.Count; ++i)
                {
                    writer.WriteLine("{0},{1}", listLightTop[i].X, listLightTop[i].Y);
                }
            }
            writer.WriteLine("ENDSEC");
            writer.WriteLine();

            writer.WriteLine("LIGHT_BOTTOM");
            writer.WriteLine("INDEX,LIGHTVALUE");
            if (0 < listLightBottom.Count)
            {
                for (int i = 0; i < listLightBottom.Count; ++i)
                {
                    writer.WriteLine("{0},{1}", listLightBottom[i].X, listLightBottom[i].Y);
                }
            }
            writer.WriteLine("ENDSEC");
            writer.WriteLine();

            writer.WriteLine("LIGHT_MONO");
            writer.WriteLine("INDEX,LIGHTVALUE");
            if (0 < listLightMono.Count)
            {
                for (int i = 0; i < listLightMono.Count; ++i)
                {
                    writer.WriteLine("{0},{1}", listLightMono[i].X, listLightMono[i].Y);
                }
            }
            writer.WriteLine("ENDSEC");
            writer.WriteLine();
            
            writer.WriteLine("PUNCH_ALIGN");
            writer.WriteLine("INDEX,LINE,OFFSET_X,OFFSET_Y,IMAGE");
            if (0 < ListAlign.Count)
            {
                for (int i = 0; i < ListAlign.Count; ++i)
                {
                    writer.WriteLine(ListAlign[i].ToString());
                }
            }
            writer.WriteLine("ENDSEC");

            writer.WriteLine();
            writer.WriteLine("PUNCH_INSPECT");
            writer.WriteLine("INDEX,LINE,OFFSET_X,OFFSET_Y,IMAGE");
            if (0 < ListPunch.Count)
            {
                for (int i = 0; i < ListPunch.Count; ++i)
                {
                    writer.WriteLine(ListPunch[i].ToString());
                }
            }
            writer.WriteLine("ENDSEC");




            writer.WriteLine();
            writer.WriteLine("COMMENT");
            writer.WriteLine(Comment);
            writer.WriteLine("ENDSEC");
            writer.WriteLine();



            writer.WriteLine("EOF");

            writer.Close();
            stream.Close();

            // Copy Images
            //for (int i = 0; i < listIndex.Count; ++i)
            //{
            //    listIndex[i].MoveImages(pathImages);
            //}

            //Move Image Top1
            string pathImg = DataService.Singleton.DataSystem.Top1Path;
            if ('\\' != pathImg[pathImg.Length - 1])
                pathImg += "\\";
            string pathTop1 = pathImg + DataService.Singleton.DataResult.LotID;
            if (true == Directory.Exists(pathTop1))
                FileUtill.MoveFiles(pathTop1, pathTop);

            //Move Image Top2
            pathImg = DataService.Singleton.DataSystem.Top2Path;
            if ('\\' != pathImg[pathImg.Length - 1])
                pathImg += "\\";
            string pathTop2 = pathImg + DataService.Singleton.DataResult.LotID;
            if (true == Directory.Exists(pathTop2))
                FileUtill.MoveFiles(pathTop2, pathTop);


            //Move Image Bottom1
            pathImg = DataService.Singleton.DataSystem.Bottom1Path;
            if ('\\' != pathImg[pathImg.Length - 1])
                pathImg += "\\";
            string pathBottom1 = pathImg + DataService.Singleton.DataResult.LotID;
            if (true == Directory.Exists(pathBottom1))
                FileUtill.MoveFiles(pathBottom1, pathBottom);

            //Move Image Bottom2
            pathImg = DataService.Singleton.DataSystem.Bottom2Path;
            if ('\\' != pathImg[pathImg.Length - 1])
                pathImg += "\\";
            string pathBottom2 = pathImg + DataService.Singleton.DataResult.LotID;
            if (true == Directory.Exists(pathBottom2))
                FileUtill.MoveFiles(pathBottom2, pathBottom);


            //Move Image Mono1
            pathImg = DataService.Singleton.DataSystem.Mono1Path;
            if ('\\' != pathImg[pathImg.Length - 1])
                pathImg += "\\";
            string pathMono1 = pathImg + DataService.Singleton.DataResult.LotID;
            if (true == Directory.Exists(pathMono1))
                FileUtill.MoveFiles(pathMono1, pathMono);

            //Move Image Mono2
            pathImg = DataService.Singleton.DataSystem.Mono2Path;
            if ('\\' != pathImg[pathImg.Length - 1])
                pathImg += "\\";
            string pathMono2 = pathImg + DataService.Singleton.DataResult.LotID;
            if (true == Directory.Exists(pathMono2))
                FileUtill.MoveFiles(pathMono2, pathMono);


            //string source = "";
            //string dest = "";
            string sourcePath = DataService.Singleton.DataSystem.PunchPath;

            if (true == Directory.Exists(sourcePath))
                FileUtill.MoveFiles(sourcePath, pathPunch);

            //if( '\\' != sourcePath[sourcePath.Length - 1] )
            //    sourcePath += "\\";

            //// Align Images
            //for (int i = 0; i < ListAlign.Count; ++i)
            //{
            //    source = sourcePath + ListAlign[i].ImageName;
            //    dest = pathAlign + "\\" + ListAlign[i].ImageName;
            //    File.Copy(source, dest, true);
            //}
            //// Punch Images
            //for (int i = 0; i < ListPunch.Count; ++i)
            //{
            //    source = sourcePath + ListPunch[i].ImageName;
            //    dest = pathPunch + "\\" + ListPunch[i].ImageName;
            //    File.Copy(source, dest, true);
            //}


            //DataResult dataResult = DataService.Singleton.DataResult;
            //dataResult.Save(pathMap

            // Delete Directory
            if (true == Directory.Exists(pathTop1))
                Directory.Delete(pathTop1);
            if (true == Directory.Exists(pathTop2))
                Directory.Delete(pathTop2);
            if (true == Directory.Exists(pathBottom1))
                Directory.Delete(pathBottom1);
            if (true == Directory.Exists(pathBottom2))
                Directory.Delete(pathBottom2);
            if (true == Directory.Exists(pathMono1))
                Directory.Delete(pathMono1);
            if (true == Directory.Exists(pathMono2))
                Directory.Delete(pathMono2);

            IsSaved = true;
        }
    }
}
