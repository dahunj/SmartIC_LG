using HalconDotNet;

namespace SmartICAVI
{
    class InspectService
    {
        private IInspect iInspect = null;
        private DataService dataService = null;

        public bool IsInitialized
        {
            get 
            {
                if (null != iInspect)
                    return iInspect.IsInitialized;

                return false; 
            }
        }

        #region Singleton
        private static InspectService singleton = null;

        public static InspectService Singleton
        {
            get
            {
                if (null == singleton)
                {
                    singleton = new InspectService();
                }

                return singleton;
            }
        }

        public void ReleaseSingleton()
        {
            if (null != singleton)
            {
                singleton.UnInitialize();
                singleton = null;
            }
        }
        #endregion

        public int Initialize()
        {
            dataService = DataService.Singleton;
            switch (dataService.DataSystem.MachineName)
            {
                case "SmartICAVI":
                    if( "EXTERN" == dataService.DataSystem.CamType )
                        iInspect = new InspectExtern();
                    else
                        iInspect = new InspectSmartIC();
                    break;
                case "Virtual":
                    if ("EXTERN" == dataService.DataSystem.CamType)
                        iInspect = new InspectExtern();
                    else
                        iInspect = new InspectVirtual();
                    break;
                default:
                    if ("EXTERN" == dataService.DataSystem.CamType)
                        iInspect = new InspectExtern();
                    else
                        iInspect = new InspectSmartIC();
                    break;
            }
            
            return iInspect.Initialize();
        }

        public int UnInitialize()
        {
            if (null != iInspect)
            {
                iInspect.UnInitialize();
            }
            return 0;
        }

        public int SetThreshold(HWindow hWindow, HObject hImage, HTuple row1, HTuple col1, HTuple row2, HTuple col2, HTuple min, HTuple max)
        {
            if (null == iInspect)
                return -1;

            return iInspect.SetThreshold(hWindow, hImage, row1, col1, row2, col2, min, max);
        }

        public int SetTeaching(HWindow hWindow, HObject hImage, HTuple row1, HTuple col1, HTuple row2, HTuple col2, HTuple row11, HTuple col11, HTuple row12, HTuple col12, HTuple min, HTuple max)
        {
            if (null == iInspect)
                return -1;

            return iInspect.SetTeaching(hWindow, hImage, row1, col1, row2, col2, row11, col11, row12, col12, min, max);
        }

        public int SetTeaching(string line, HWindow hWindow, HObject hImage)
        {
            if (null == iInspect)
                return -1;

            if ("A" == line)
            {
                //HTuple Row1 = dataService.DataTeach.inspectRow1A;
                //HTuple Col1 = dataService.DataTeach.inspectCol1A;
                //HTuple Row2 = dataService.DataTeach.inspectRow2A;
                //HTuple Col2 = dataService.DataTeach.inspectRow2A;

                //HTuple Row11 = dataService.DataTeach.searchRow1A;
                //HTuple Col11 = dataService.DataTeach.searchCol1A;
                //HTuple Row12 = dataService.DataTeach.searchRow2A;
                //HTuple Col12 = dataService.DataTeach.searchRow2A;

                //HTuple grayMin = dataService.DataTeach.grayMin;
                //HTuple grayMax = dataService.DataTeach.grayMax;

                return iInspect.SetTeachingA(hWindow, hImage);
            }

            else if ("B" == line)
            {
                //HTuple Row1 = dataService.DataTeach.inspectRow1B;
                //HTuple Col1 = dataService.DataTeach.inspectCol1B;
                //HTuple Row2 = dataService.DataTeach.inspectRow2B;
                //HTuple Col2 = dataService.DataTeach.inspectRow2B;
                                                               
                //HTuple Row11 = dataService.DataTeach.searchRow1B;
                //HTuple Col11 = dataService.DataTeach.searchCol1B;
                //HTuple Row12 = dataService.DataTeach.searchRow2B;
                //HTuple Col12 = dataService.DataTeach.searchRow2B;

                //HTuple grayMin = dataService.DataTeach.grayMin;
                //HTuple grayMax = dataService.DataTeach.grayMax;

                return iInspect.SetTeachingB(hWindow, hImage);
            }

            return -1;
        }

        public int MakeRecipe(HWindow hWindow, HObject hImage, HTuple row1, HTuple col1, HTuple row2, HTuple col2, HTuple row11, HTuple col11, HTuple row12, HTuple col12, HTuple min, HTuple max)
        {
            if (null == iInspect)
                return -1;

            return iInspect.MakeRecipe(hWindow, hImage, row1, col1, row2, col2, row11, col11, row12, col12, min, max);
        }


        public int SaveTeach(HWindow hWindow, HObject hImage)
        {
            if (null == iInspect)
                return -1;

            return iInspect.SaveTeach(hWindow, hImage);
        }

        public int Inspect(HWindow hWindow, HObject hImage, out double offsetX, out double offsetY, string path = "NO")
        {
            if (null == iInspect)
            {
                offsetX = offsetY = 0.0;
                return -1;
            }

            return iInspect.Inspect(hWindow, hImage, out offsetX, out offsetY, path);
        }

        public int Inspect(HWindow hWindow, HObject hImage, out double offsetX, out double offsetY, HTuple row1, HTuple col1, HTuple row2, HTuple col2, HTuple min, HTuple max)
        {
            if (null == iInspect)
            {
                offsetX = offsetY = 0.0;
                return -1;
            }

            return iInspect.Inspect(hWindow, hImage, out offsetX, out offsetY, row1, col1, row2, col2, min, max);
        }


        public int InspectTeach(HWindow hWindow, HObject hImage, out double offsetX, out double offsetY)
        {
            if (null == iInspect)
            {
                offsetX = offsetY = 0.0;
                return -1;
            }

            return iInspect.InspectTeach(hWindow, hImage, out offsetX, out offsetY);
        }

        public int InspectCircle(HWindow hWindow, HObject hImage, out double offsetX, out double offsetY, HTuple row1, HTuple col1, HTuple row2, HTuple col2, HTuple min, HTuple max, string path = "NO")
        {
            if (null == iInspect)
            {
                offsetX = offsetY = 0.0;
                return -1;
            }

            return iInspect.InspectCircle(hWindow, hImage, out offsetX, out offsetY, row1, col1, row2, col2, min, max, path);
        }

        public int SaveImage(string path)
        {
            if (null != iInspect)
                return iInspect.SaveImage(path);

            return 0;
        }
    }
}
