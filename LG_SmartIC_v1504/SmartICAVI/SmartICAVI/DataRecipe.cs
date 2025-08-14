using System;
using HalconDotNet;

namespace SmartICAVI
{
    class DataRecipe : ICloneable
    {
        private string Path { get; set; }       // index 저장위치
        public string PathRecipe { get; set; }  // 실제 데이터 저장폴더

        public int PF { get; set; }           // Unit 당 IP Hole 갯수
        public int Line { get; set; }        // Width 방향의 Unit 갯수
        public int NGContinue { get; set; }   // 연속 불량갯수

        public int SectionMinUnits { get; set; }    // 최소구간 유닛

        public int ScanUnits { get; set; }    // 스캔할 유닛갯수
        public double PreScan { get; set; }     // 미리 스캔할 길이

        public double Width { get; set; }       // 소재 폭

        public double BottomX { get; set; }     // Top -> Bottom Vision 까지의 거리
        public double MonoX { get; set; }       // Top -> Mono Vision  까지의 거리

        public double TopZ { get; set; }        // Top Z 축 위치
        public double BottomZ { get; set; }     // Bottom Z 축 위치
        public double BufferZ { get; set; }     // Buffer Z 축 위치

        public double AlignX { get; set; }      // Align Vision X 측정위치 -- 20160613 이후 사용하지 않음
        public double AlignY { get; set; }      // Align Vision Y 축정위치 -- 20160613 이후 사용하지 않음

        public double AlignOffsetX { get; set; }      // Align Vision X 옵셋값 -- 20160613 이후 사용하지 않음
        public double AlignOffsetY { get; set; }      // Align Vision Y 옵셋값 -- 20160613 이후 사용하지 않음

        public double AlignXA { get; set; }      // Align Vision X 측정위치 (A열)
        public double AlignYA { get; set; }      // Align Vision Y 축정위치 (A열)

        public double AlignXB { get; set; }     // Align Vision X 측정위치 (B열)
        public double AlignYB { get; set; }     // Align Vision X 측정위치 (B열)

        public double PunchX { get; set; }      // 기준점에서 펀치 할 X 값
        public double PunchY { get; set; }      // 기준점에서 펀치 할 Y 값

        public double PunchCenterX { get; set; }      // 기준점에서 펀치 할 CenterX 값
        public double PunchCenterY { get; set; }      // 기준점에서 펀치 할 CenterY 값

        public double OffsetInitX { get; set; } // 첫번째 IP 홀 옵셋값
        public double OffsetInitY { get; set; }
        public double SearchAreaSize { get; set; }    // 서치에어리어 사이즈

        public double PunchStroke { get; set; } // 펀치소재를 한번에 Feeding 할 수 있는 최대 길이

        public double PunchToleranceX { get; set; } // 펀치 허용공차
        public double PunchToleranceY { get; set; }

        public HTuple modelID;                  // 기준점 Model ID  -- 20160613 이후 사용하지 않음
        public HObject modelContours;           // 기준점 Shape Contours  -- 20160613 이후 사용하지 않음

        public HTuple modelIDA;                  // 기준점 Model ID (A열)
        public HObject modelContoursA;           // 기준점 Shape Contours (A열)

        public HTuple modelIDB;                  // 기준점 Model ID (B열)
        public HObject modelContoursB;           // 기준점 Shape Contours (B열)

        public HTuple row;                      // 검사 중심 좌표
        public HTuple col;                      // 검사 중심 좌표

        public HTuple inspectRow1;
        public HTuple inspectCol1;
        public HTuple inspectRow2;
        public HTuple inspectCol2;

        public HTuple searchRow1;
        public HTuple searchCol1;
        public HTuple searchRow2;
        public HTuple searchCol2;

        public HTuple grayMin;
        public HTuple grayMax;


        public int strobe1;
        public int strobe2;
        public int strobe3;
        public int strobe4;

        public int[] sectionUnits;        // 구간수율 설정 12개 구간
        public double[] sectionYields;      // 구간 별 최소 수율
        public int minSectionUnit;        // 마지막 12 번째 구간 이후에 최소수율을 계산할 유닛, 즉 minSectionUnit 안에서는 minSectionYield 보다 아래여도 허용한다.
        public double minSectionYield;       // 마지막 구간의 최소수율 설정


        public HTuple inspectRow1A;
        public HTuple inspectCol1A;
        public HTuple inspectRow2A;
        public HTuple inspectCol2A;

        public HTuple searchRow1A;
        public HTuple searchCol1A;
        public HTuple searchRow2A;
        public HTuple searchCol2A;

        public HTuple grayMinA;
        public HTuple grayMaxA;

        public HTuple inspectRow1B;
        public HTuple inspectCol1B;
        public HTuple inspectRow2B;
        public HTuple inspectCol2B;

        public HTuple searchRow1B;
        public HTuple searchCol1B;
        public HTuple searchRow2B;
        public HTuple searchCol2B;

        public HTuple grayMinB;
        public HTuple grayMaxB;

        public HTuple contrastMinA;
        public HTuple contrastMaxA;

        public HTuple contrastMinB;
        public HTuple contrastMaxB;


        public DataRecipe()
        {
            Clear();
        }

        private void Clear()
        {
            Path = "";
            PathRecipe = "";

            PF = 2;
            Line = 2;
            NGContinue = 8;

            ScanUnits = 10;
            PreScan = 5.0;

            SectionMinUnits = 13000;

            Width = 31.83;

            BottomX = 154.0;
            MonoX = 308.0;

            TopZ = 0.0;
            BottomZ = 0.0;
            BufferZ = 150.0;

            AlignX = 0.0;
            AlignY = 0.0;

            AlignOffsetX = 0.0;
            AlignOffsetY = 0.0;

            PunchX = 0.0;
            PunchY = 3.0;

            PunchCenterX = 0.0;
            PunchCenterY = 3.0;

            PunchStroke = 250.0;

            PunchToleranceX = 0.2;
            PunchToleranceY = 0.2;

            row = 0;
            col = 0;

            inspectRow1 = 0;
            inspectCol1 = 0;
            inspectRow2 = 0;
            inspectCol2 = 0;

            searchRow1 = 0;
            searchCol1 = 0;
            searchRow2 = 0;
            searchCol2 = 0;

            grayMin = 100;
            grayMax = 255;

            strobe1 = 100;
            strobe2 = 0;
            strobe3 = 0;
            strobe4 = 0;

            inspectRow1A = 0;
            inspectCol1A = 0;
            inspectRow2A = 0;
            inspectCol2A = 0;

            searchRow1A = 0;
            searchCol1A = 0;
            searchRow2A = 0;
            searchCol2A = 0;

            grayMinA = 100;
            grayMaxA = 255;

            inspectRow1B = 0;
            inspectCol1B = 0;
            inspectRow2B = 0;
            inspectCol2B = 0;

            searchRow1B = 0;
            searchCol1B = 0;
            searchRow2B = 0;
            searchCol2B = 0;

            grayMinB = 100;
            grayMaxB = 255;

            contrastMinA = 10;
            contrastMaxA = 100;

            contrastMinB = 10;
            contrastMaxB = 100;

            modelID = -1;
            //HOperatorSet.GenEmptyObj(out modelContours);

            modelIDA = -1;
            //HOperatorSet.GenEmptyObj(out modelContoursA);

            modelIDB = -1;
            //HOperatorSet.GenEmptyObj(out modelContoursB);

            sectionUnits = new int[20] { 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000, 11000, 12000, 13000, 14000, 15000, 16000, 17000, 18000, 19000, 20000 };
            sectionYields = new double[20] { 80.0, 85.0, 90.0, 91.0, 92.0, 93.0, 94.0, 95.0, 96.0, 97.0, 98.0, 99.0, 99.0, 99.0, 99.0, 99.0, 99.0, 99.0, 99.0, 99.0 };
            minSectionUnit = 500;
            minSectionYield = 95.0;


            OffsetInitX = 0.0; // 첫번째 IP 홀 옵셋값
            OffsetInitY = 0.0;
            SearchAreaSize = 5.0;    // 서치에어리어 사이즈
        }

        //public ~DataRecipe()
        //{
        //    modelContours.Dispose();
        //}

        public void Dispose()
        {
            //if( -1 != modelID )
            //    HOperatorSet.ClearShapeModel(modelID);
            if( null != modelContours )
                modelContours.Dispose();

            if (null != modelContoursA)
                modelContoursA.Dispose();

            if (null != modelContoursB)
                modelContoursB.Dispose();
        }

        public int Load(string path)
        {
            Clear();

            Path = path;
            return Load();
        }

        public int Save(string path)
        {
            Path = path;
            return Save();
        }

        public int Load()
        {
            //systemFilePath = iniPath + "\\System.ini";

            //string path1 = System.IO.Path.GetDirectoryName(systemFilePath); //iniPath
            //string path2 = System.IO.Path.GetFileNameWithoutExtension(systemFilePath); //System
            //string path3 = System.IO.Path.GetFileName(systemFilePath);  //System.ini
            //string path4 = System.IO.Path.GetExtension(systemFilePath); //.ini
            //string path5 = System.IO.Path.GetFullPath(systemFilePath);  // iniPath + "\\System.ini";
            //string path6 = System.IO.Path.GetPathRoot(systemFilePath);  // D:\\

            string dir = System.IO.Path.GetDirectoryName(this.Path);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(this.Path);
            PathRecipe = dir + "\\" + fileName;

            string path = PathRecipe + "\\Recipe.ini";
            string pathModelID = PathRecipe + "\\ModelID.shm";
            string pathModelContours = PathRecipe + "\\ModelContours.xld";

            string pathModelIDA = PathRecipe + "\\ModelIDA.shm";
            string pathModelContoursA = PathRecipe + "\\ModelContoursA.xld";
            string pathModelIDB = PathRecipe + "\\ModelIDB.shm";
            string pathModelContoursB = PathRecipe + "\\ModelContoursB.xld";
            

            // Check Data Directory
            if (false == System.IO.Directory.Exists(PathRecipe))
            {
                return -1;
            }

            try
            {
                IniFile ini = new IniFile();
                string section = "RECIPE";

                PF = int.Parse(ini.Read(section, "PF", PF.ToString(), path));
                Line = int.Parse(ini.Read(section, "Line", Line.ToString(), path));
                NGContinue = int.Parse(ini.Read(section, "NGContinue", NGContinue.ToString(), path));
                SectionMinUnits = int.Parse(ini.Read(section, "SectionMinUnits", SectionMinUnits.ToString(), path));

                ScanUnits = int.Parse(ini.Read(section, "ScanUnits", ScanUnits.ToString(), path));
                PreScan = double.Parse(ini.Read(section, "PreScan", PreScan.ToString(), path));

                Width = double.Parse(ini.Read(section, "Width", Width.ToString(), path));

                BottomX = double.Parse(ini.Read(section, "BottomX", BottomX.ToString(), path));
                MonoX = double.Parse(ini.Read(section, "MonoX", MonoX.ToString(), path));

                TopZ = double.Parse(ini.Read(section, "TopZ", TopZ.ToString(), path));
                BottomZ = double.Parse(ini.Read(section, "BottomZ", BottomZ.ToString(), path));
                BufferZ = double.Parse(ini.Read(section, "BufferZ", BufferZ.ToString(), path));

                AlignX = double.Parse(ini.Read(section, "AlignX", AlignX.ToString(), path));
                AlignY = double.Parse(ini.Read(section, "AlignY", AlignY.ToString(), path));
                AlignOffsetX = double.Parse(ini.Read(section, "AlignOffsetX", AlignOffsetX.ToString(), path));
                AlignOffsetY = double.Parse(ini.Read(section, "AlignOffsetY", AlignOffsetY.ToString(), path));

                PunchX = double.Parse(ini.Read(section, "PunchX", PunchX.ToString(), path));
                PunchY = double.Parse(ini.Read(section, "PunchY", PunchY.ToString(), path));

                PunchCenterX = double.Parse(ini.Read(section, "PunchCenterX", PunchCenterX.ToString(), path));
                PunchCenterY = double.Parse(ini.Read(section, "PunchCenterY", PunchCenterY.ToString(), path));

                PunchStroke = double.Parse(ini.Read(section, "PunchStroke", PunchStroke.ToString(), path));
                PunchToleranceX = double.Parse(ini.Read(section, "PunchToleranceX", PunchToleranceX.ToString(), path));
                PunchToleranceY = double.Parse(ini.Read(section, "PunchToleranceY", PunchToleranceY.ToString(), path));


                OffsetInitX = double.Parse(ini.Read(section, "OffsetInitX", OffsetInitX.ToString(), path));
                OffsetInitY = double.Parse(ini.Read(section, "OffsetInitY", OffsetInitY.ToString(), path));
                SearchAreaSize = double.Parse(ini.Read(section, "SearchAreaSize", SearchAreaSize.ToString(), path));

                //inspectRow1 = int.Parse(ini.Read(section, "inspectRow1", inspectRow1.I.ToString(), path));
                //inspectCol1 = int.Parse(ini.Read(section, "inspectCol1", inspectCol1.I.ToString(), path));
                //inspectRow2 = int.Parse(ini.Read(section, "inspectRow2", inspectRow2.I.ToString(), path));
                //inspectCol2 = int.Parse(ini.Read(section, "inspectCol2", inspectCol2.I.ToString(), path));

                //searchRow1 = int.Parse(ini.Read(section, "searchRow1", searchRow1.I.ToString(), path));
                //searchCol1 = int.Parse(ini.Read(section, "searchCol1", searchCol1.I.ToString(), path));
                //searchRow2 = int.Parse(ini.Read(section, "searchRow2", searchRow2.I.ToString(), path));
                //searchCol2 = int.Parse(ini.Read(section, "searchCol2", searchCol2.I.ToString(), path));

                //grayMin = int.Parse(ini.Read(section, "thresholdMin", grayMin.I.ToString(), path));
                //grayMax = int.Parse(ini.Read(section, "thresholdMax", grayMax.I.ToString(), path));

                //inspectRow1A = int.Parse(ini.Read(section, "inspectRow1A", inspectRow1.I.ToString(), path));
                //inspectCol1A = int.Parse(ini.Read(section, "inspectCol1A", inspectCol1.I.ToString(), path));
                //inspectRow2A = int.Parse(ini.Read(section, "inspectRow2A", inspectRow2.I.ToString(), path));
                //inspectCol2A = int.Parse(ini.Read(section, "inspectCol2A", inspectCol2.I.ToString(), path));

                //searchRow1A = int.Parse(ini.Read(section, "searchRow1A", searchRow1.I.ToString(), path));
                //searchCol1A = int.Parse(ini.Read(section, "searchCol1A", searchCol1.I.ToString(), path));
                //searchRow2A = int.Parse(ini.Read(section, "searchRow2A", searchRow2.I.ToString(), path));
                //searchCol2A = int.Parse(ini.Read(section, "searchCol2A", searchCol2.I.ToString(), path));

                //grayMinA = int.Parse(ini.Read(section, "grayMinA", grayMin.I.ToString(), path));
                //grayMaxA = int.Parse(ini.Read(section, "grayMaxA", grayMax.I.ToString(), path));

                //inspectRow1B = int.Parse(ini.Read(section, "inspectRow1B", inspectRow1.I.ToString(), path));
                //inspectCol1B = int.Parse(ini.Read(section, "inspectCol1B", inspectCol1.I.ToString(), path));
                //inspectRow2B = int.Parse(ini.Read(section, "inspectRow2B", inspectRow2.I.ToString(), path));
                //inspectCol2B = int.Parse(ini.Read(section, "inspectCol2B", inspectCol2.I.ToString(), path));

                //searchRow1B = int.Parse(ini.Read(section, "searchRow1B", searchRow1.I.ToString(), path));
                //searchCol1B = int.Parse(ini.Read(section, "searchCol1B", searchCol1.I.ToString(), path));
                //searchRow2B = int.Parse(ini.Read(section, "searchRow2B", searchRow2.I.ToString(), path));
                //searchCol2B = int.Parse(ini.Read(section, "searchCol2B", searchCol2.I.ToString(), path));

                //grayMinB = int.Parse(ini.Read(section, "grayMinB", grayMin.I.ToString(), path));
                //grayMaxB = int.Parse(ini.Read(section, "grayMaxB", grayMax.I.ToString(), path));

                //contrastMinA = int.Parse(ini.Read(section, "contrastMinA", contrastMinA.I.ToString(), path));
                //contrastMaxA = int.Parse(ini.Read(section, "contrastMaxA", contrastMaxA.I.ToString(), path));

                //contrastMinB = int.Parse(ini.Read(section, "contrastMinB", contrastMinB.I.ToString(), path));
                //contrastMaxB = int.Parse(ini.Read(section, "contrastMaxB", contrastMaxB.I.ToString(), path));

                //row = double.Parse(ini.Read(section, "row", row.D.ToString(), path));
                //col = double.Parse(ini.Read(section, "col", col.D.ToString(), path));

                strobe1 = int.Parse(ini.Read(section, "strobe1", strobe1.ToString(), path));
                strobe2 = int.Parse(ini.Read(section, "strobe2", strobe2.ToString(), path));
                strobe3 = int.Parse(ini.Read(section, "strobe3", strobe3.ToString(), path));
                strobe4 = int.Parse(ini.Read(section, "strobe4", strobe4.ToString(), path));

                // SectionYield
                minSectionUnit = int.Parse(ini.Read(section, "minSectionUnit", minSectionUnit.ToString(), path));
                minSectionYield = double.Parse(ini.Read(section, "minSectionYield", minSectionYield.ToString(), path));

                //string temp;
                for (int i = 0; i < sectionUnits.Length; ++i)
                {
                    sectionUnits[i] = int.Parse(ini.Read(section, string.Format("SectionUnit_{0:00}", i), sectionUnits[i].ToString(), path));
                    sectionYields[i] = double.Parse(ini.Read(section, string.Format("SectionYield_{0:00}", i), sectionYields[i].ToString(), path));
                }

                if( null != modelContours )
                    modelContours.Dispose();

                //if( true == System.IO.File.Exists(pathModelID) )
                //    HOperatorSet.ReadShapeModel(pathModelID, out modelID);
                //if (true == System.IO.File.Exists(pathModelContours))
                //    HOperatorSet.ReadContourXldArcInfo(out modelContours, pathModelContours);

                //if (true == System.IO.File.Exists(pathModelIDA))
                //    HOperatorSet.ReadShapeModel(pathModelIDA, out modelIDA);
                //if (true == System.IO.File.Exists(pathModelContoursA))
                //    HOperatorSet.ReadContourXldArcInfo(out modelContoursA, pathModelContoursA);

                //if (true == System.IO.File.Exists(pathModelIDB))
                //    HOperatorSet.ReadShapeModel(pathModelIDB, out modelIDB);
                //if (true == System.IO.File.Exists(pathModelContoursB))
                //    HOperatorSet.ReadContourXldArcInfo(out modelContoursB, pathModelContoursB);
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine("DataRecipe.Load() : " + exc.Message);
                return -1;
            }

            return 0;
        }

        public int Save()
        {
            string dir = System.IO.Path.GetDirectoryName(this.Path);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(this.Path);
            PathRecipe = dir + "\\" + fileName;

            // Check Data Directory
            if (false == System.IO.Directory.Exists(PathRecipe))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(PathRecipe);
                }
                catch (Exception exc)
                {
                    Log_Exception.WriteLine("DataRecipe.Save() : " + exc.Message);
                    return -1;
                }
            }


            string path = PathRecipe + "\\Recipe.ini";
            string pathModelID = PathRecipe + "\\ModelID.shm";
            string pathModelContours = PathRecipe + "\\ModelContours.xld";
            string pathModelIDA = PathRecipe + "\\ModelIDA.shm";
            string pathModelContoursA = PathRecipe + "\\ModelContoursA.xld";
            string pathModelIDB = PathRecipe + "\\ModelIDB.shm";
            string pathModelContoursB = PathRecipe + "\\ModelContoursB.xld";

            IniFile rcp = new IniFile();
            string section = "UPDATE";

            rcp.Write(section, "date", DateTime.Now.ToLocalTime().ToString(), Path);



            try
            {
                IniFile ini = new IniFile();
                section = "RECIPE";

                CheckData(ini, section, "PF", PF.ToString(), path);
                CheckData(ini, section, "Line", Line.ToString(), path);
                CheckData(ini, section, "NGContinue", NGContinue.ToString(), path);
                CheckData(ini, section, "SectionMinUnits", SectionMinUnits.ToString(), path);

                CheckData(ini, section, "ScanUnits", ScanUnits.ToString(), path);
                CheckData(ini, section, "PreScan", PreScan.ToString(), path);

                CheckData(ini, section, "Width", Width.ToString(), path);

                CheckData(ini, section, "BottomX", BottomX.ToString(), path);
                CheckData(ini, section, "MonoX", MonoX.ToString(), path);

                CheckData(ini, section, "TopZ", TopZ.ToString(), path);
                CheckData(ini, section, "BottomZ", BottomZ.ToString(), path);
                CheckData(ini, section, "BufferZ", BufferZ.ToString(), path);

                CheckData(ini, section, "AlignX", AlignX.ToString(), path);
                CheckData(ini, section, "AlignY", AlignY.ToString(), path);
                CheckData(ini, section, "AlignOffsetX", AlignOffsetX.ToString(), path);
                CheckData(ini, section, "AlignOffsetY", AlignOffsetY.ToString(), path);

                CheckData(ini, section, "PunchX", PunchX.ToString(), path);
                CheckData(ini, section, "PunchY", PunchY.ToString(), path);

                CheckData(ini, section, "PunchCenterX", PunchCenterX.ToString(), path);
                CheckData(ini, section, "PunchCenterY", PunchCenterY.ToString(), path);

                CheckData(ini, section, "PunchStroke", PunchStroke.ToString(), path);
                CheckData(ini, section, "PunchToleranceX", PunchToleranceX.ToString(), path);
                CheckData(ini, section, "PunchToleranceY", PunchToleranceY.ToString(), path);

                CheckData(ini, section, "OffsetInitX", OffsetInitX.ToString(), path);
                CheckData(ini, section, "OffsetInitY", OffsetInitY.ToString(), path);
                CheckData(ini, section, "SearchAreaSize", SearchAreaSize.ToString(), path);


                CheckData(ini, section, "strobe1", strobe1.ToString(), path);
                CheckData(ini, section, "strobe2", strobe2.ToString(), path);
                CheckData(ini, section, "strobe3", strobe3.ToString(), path);
                CheckData(ini, section, "strobe4", strobe4.ToString(), path);

                // SectionYield
                CheckData(ini, section, "minSectionUnit", minSectionUnit.ToString(), path);
                CheckData(ini, section, "minSectionYield", minSectionYield.ToString(), path);

                //string temp;
                for (int i = 0; i < sectionUnits.Length; ++i)
                {
                    CheckData(ini, section, string.Format("SectionUnit_{0:00}", i), sectionUnits[i].ToString(), path);
                    CheckData(ini, section, string.Format("SectionYield_{0:00}", i), sectionYields[i].ToString(), path);
                }


                #region OLD
                ini.Write(section, "PF", PF.ToString(), path);
                ini.Write(section, "Line", Line.ToString(), path);
                ini.Write(section, "NGContinue", NGContinue.ToString(), path);
                ini.Write(section, "SectionMinUnits", SectionMinUnits.ToString(), path);

                ini.Write(section, "ScanUnits", ScanUnits.ToString(), path);
                ini.Write(section, "PreScan", PreScan.ToString(), path);

                ini.Write(section, "Width", Width.ToString(), path);

                ini.Write(section, "BottomX", BottomX.ToString(), path);
                ini.Write(section, "MonoX", MonoX.ToString(), path);

                ini.Write(section, "TopZ", TopZ.ToString(), path);
                ini.Write(section, "BottomZ", BottomZ.ToString(), path);
                ini.Write(section, "BufferZ", BufferZ.ToString(), path);

                ini.Write(section, "AlignX", AlignX.ToString(), path);
                ini.Write(section, "AlignY", AlignY.ToString(), path);
                ini.Write(section, "AlignOffsetX", AlignOffsetX.ToString(), path);
                ini.Write(section, "AlignOffsetY", AlignOffsetY.ToString(), path);

                ini.Write(section, "PunchX", PunchX.ToString(), path);
                ini.Write(section, "PunchY", PunchY.ToString(), path);

                ini.Write(section, "PunchCenterX", PunchCenterX.ToString(), path);
                ini.Write(section, "PunchCenterY", PunchCenterY.ToString(), path);

                ini.Write(section, "PunchStroke", PunchStroke.ToString(), path);
                ini.Write(section, "PunchToleranceX", PunchToleranceX.ToString(), path);
                ini.Write(section, "PunchToleranceY", PunchToleranceY.ToString(), path);

                ini.Write(section, "OffsetInitX", OffsetInitX.ToString(), path);
                ini.Write(section, "OffsetInitY", OffsetInitY.ToString(), path);
                ini.Write(section, "SearchAreaSize", SearchAreaSize.ToString(), path);

                //ini.Write(section, "inspectRow1", inspectRow1.I.ToString(), path);
                //ini.Write(section, "inspectCol1", inspectCol1.I.ToString(), path);
                //ini.Write(section, "inspectRow2", inspectRow2.I.ToString(), path);
                //ini.Write(section, "inspectCol2", inspectCol2.I.ToString(), path);

                //ini.Write(section, "searchRow1", searchRow1.I.ToString(), path);
                //ini.Write(section, "searchCol1", searchCol1.I.ToString(), path);
                //ini.Write(section, "searchRow2", searchRow2.I.ToString(), path);
                //ini.Write(section, "searchCol2", searchCol2.I.ToString(), path);

                //ini.Write(section, "thresholdMin", grayMin.I.ToString(), path);
                //ini.Write(section, "thresholdMax", grayMax.I.ToString(), path);

                //ini.Write(section, "inspectRow1A", inspectRow1A.I.ToString(), path);
                //ini.Write(section, "inspectCol1A", inspectCol1A.I.ToString(), path);
                //ini.Write(section, "inspectRow2A", inspectRow2A.I.ToString(), path);
                //ini.Write(section, "inspectCol2A", inspectCol2A.I.ToString(), path);

                //ini.Write(section, "searchRow1A", searchRow1A.I.ToString(), path);
                //ini.Write(section, "searchCol1A", searchCol1A.I.ToString(), path);
                //ini.Write(section, "searchRow2A", searchRow2A.I.ToString(), path);
                //ini.Write(section, "searchCol2A", searchCol2A.I.ToString(), path);

                //ini.Write(section, "grayMinA", grayMinA.I.ToString(), path);
                //ini.Write(section, "grayMaxA", grayMaxA.I.ToString(), path);

                //ini.Write(section, "inspectRow1B", inspectRow1B.I.ToString(), path);
                //ini.Write(section, "inspectCol1B", inspectCol1B.I.ToString(), path);
                //ini.Write(section, "inspectRow2B", inspectRow2B.I.ToString(), path);
                //ini.Write(section, "inspectCol2B", inspectCol2B.I.ToString(), path);

                //ini.Write(section, "searchRow1B", searchRow1B.I.ToString(), path);
                //ini.Write(section, "searchCol1B", searchCol1B.I.ToString(), path);
                //ini.Write(section, "searchRow2B", searchRow2B.I.ToString(), path);
                //ini.Write(section, "searchCol2B", searchCol2B.I.ToString(), path);

                //ini.Write(section, "grayMinB", grayMinB.I.ToString(), path);
                //ini.Write(section, "grayMaxB", grayMaxB.I.ToString(), path);

                //ini.Write(section, "contrastMinA", contrastMinA.I.ToString(), path);
                //ini.Write(section, "contrastMaxA", contrastMaxA.I.ToString(), path);

                //ini.Write(section, "contrastMinB", contrastMinB.I.ToString(), path);
                //ini.Write(section, "contrastMaxB", contrastMaxB.I.ToString(), path);

                //ini.Write(section, "row", row.D.ToString(), path);
                //ini.Write(section, "col", col.D.ToString(), path);

                ini.Write(section, "strobe1", strobe1.ToString(), path);
                ini.Write(section, "strobe2", strobe2.ToString(), path);
                ini.Write(section, "strobe3", strobe3.ToString(), path);
                ini.Write(section, "strobe4", strobe4.ToString(), path);

                // SectionYield
                ini.Write(section, "minSectionUnit", minSectionUnit.ToString(), path);
                ini.Write(section, "minSectionYield", minSectionYield.ToString(), path);

                //string temp;
                for (int i = 0; i < sectionUnits.Length; ++i)
                {
                    ini.Write(section, string.Format("SectionUnit_{0:00}", i), sectionUnits[i].ToString(), path);
                    ini.Write(section, string.Format("SectionYield_{0:00}", i), sectionYields[i].ToString(), path);
                }


                //if( -1 != modelID )
                //{
                //    HOperatorSet.WriteShapeModel(modelID, pathModelID);
                //    HOperatorSet.WriteContourXldArcInfo(modelContours, pathModelContours);
                //}

                //if (-1 != modelIDA)
                //{
                //    HOperatorSet.WriteShapeModel(modelIDA, pathModelIDA);
                //    HOperatorSet.WriteContourXldArcInfo(modelContoursA, pathModelContoursA);
                //}

                //if (-1 != modelIDB)
                //{
                //    HOperatorSet.WriteShapeModel(modelIDB, pathModelIDB);
                //    HOperatorSet.WriteContourXldArcInfo(modelContoursB, pathModelContoursB);
                //}
                #endregion
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine("DataRecipe.Save() : " + exc.Message);
                return -1;
            }

            return 0;
            
        }

        public int LoadDefault()
        {
            DataService dataService = DataService.Singleton;

            // System Default Data
            BottomX = dataService.DataSystem.BottomDistance;
            MonoX = dataService.DataSystem.MonoDistance;

            TopZ = dataService.DataSystem.TopVisionZ;
            BottomZ = dataService.DataSystem.BottomVisionZ;
            BufferZ = dataService.DataSystem.BufferReference;

            AlignX = dataService.DataSystem.AlignVisionX;
            AlignY = dataService.DataSystem.AlignVisionY;

            PunchStroke = dataService.DataSystem.PunchStroke;

            OffsetInitX = 0.0;
            OffsetInitY = 0.0;
            SearchAreaSize = 4.0;

            Line = 2;

            return 0;
        }

        public object Clone()
        {
            DataRecipe data = new DataRecipe();

            data.Path = this.Path;
            data.PathRecipe = this.PathRecipe;

            data.PF = this.PF;
            data.Line = this.Line;
            data.NGContinue = this.NGContinue;
            data.SectionMinUnits = this.SectionMinUnits;

            data.ScanUnits = this.ScanUnits;
            data.PreScan = this.PreScan;

            data.Width = this.Width;

            data.BottomX = this.BottomX;
            data.MonoX = this.MonoX;

            data.TopZ = this.TopZ;
            data.BottomZ = this.BottomZ;
            data.BufferZ = this.BufferZ;

            data.AlignX = this.AlignX;
            data.AlignY = this.AlignY;

            data.PunchX = this.PunchX;
            data.PunchY = this.PunchY;

            data.PunchCenterX = this.PunchCenterX;
            data.PunchCenterY = this.PunchCenterY;

            data.PunchStroke = this.PunchStroke;
            data.PunchToleranceX = this.PunchToleranceX;
            data.PunchToleranceY = this.PunchToleranceY;

            data.OffsetInitX = this.OffsetInitX;
            data.OffsetInitY = this.OffsetInitY;
            data.SearchAreaSize = this.SearchAreaSize;

            data.row = this.row.Clone();
            data.col = this.col.Clone();

            data.inspectRow1 = this.inspectRow1.Clone();
            data.inspectCol1 = this.inspectCol1.Clone();
            data.inspectRow2 = this.inspectRow2.Clone();
            data.inspectCol2 = this.inspectCol2.Clone();

            data.searchRow1 = this.searchRow1.Clone();
            data.searchCol1 = this.searchCol1.Clone();
            data.searchRow2 = this.searchRow2.Clone();
            data.searchCol2 = this.searchCol2.Clone();

            data.grayMin = this.grayMin.Clone();
            data.grayMax = this.grayMax.Clone();

            data.inspectRow1A = this.inspectRow1A.Clone();
            data.inspectCol1A = this.inspectCol1A.Clone();
            data.inspectRow2A = this.inspectRow2A.Clone();
            data.inspectCol2A = this.inspectCol2A.Clone();

            data.searchRow1A = this.searchRow1A.Clone();
            data.searchCol1A = this.searchCol1A.Clone();
            data.searchRow2A = this.searchRow2A.Clone();
            data.searchCol2A = this.searchCol2A.Clone();

            data.grayMinA = this.grayMinA.Clone();
            data.grayMaxA = this.grayMaxA.Clone();

            data.inspectRow1B = this.inspectRow1B.Clone();
            data.inspectCol1B = this.inspectCol1B.Clone();
            data.inspectRow2B = this.inspectRow2B.Clone();
            data.inspectCol2B = this.inspectCol2B.Clone();

            data.searchRow1B = this.searchRow1B.Clone();
            data.searchCol1B = this.searchCol1B.Clone();
            data.searchRow2B = this.searchRow2B.Clone();
            data.searchCol2B = this.searchCol2B.Clone();

            data.grayMinB = this.grayMinB.Clone();
            data.grayMaxB = this.grayMaxB.Clone();

            data.strobe1 = this.strobe1;
            data.strobe2 = this.strobe2;
            data.strobe3 = this.strobe3;
            data.strobe4 = this.strobe4;

            data.modelID = this.modelID.Clone();
            data.modelContours = this.modelContours;

            data.modelIDA = this.modelIDA.Clone();
            data.modelContoursA = this.modelContoursA;

            data.modelIDB = this.modelIDB.Clone();
            data.modelContoursB = this.modelContoursB;

            data.contrastMinA = this.contrastMinA;
            data.contrastMaxA = this.contrastMaxA;

            data.contrastMinB = this.contrastMinB;
            data.contrastMaxB = this.contrastMaxB;


            for (int i = 0; i < this.sectionUnits.Length; ++i)
            {
                data.sectionUnits[i] = this.sectionUnits[i];
                data.sectionYields[i] = this.sectionYields[i];
            }
            data.minSectionUnit = this.minSectionUnit;
            data.minSectionYield = this.minSectionYield;       

            return data;
        }

        private void CheckData(IniFile ini, string section, string key, string value, string path)
        {
            string read = "";

            read = ini.Read(section, key, "0", path);
            if (read != value)
            {
                Log_History.WriteLine("{0,-30},{1,-30},{2,-30},{3,-30}", "[DataRecipe]", key, read, value);
                ini.Write(section, key, value, path);
            }
        }
    }
}
