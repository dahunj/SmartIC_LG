namespace SmartICAVI
{
    class InspectVirtual : IInspect
    {
        public bool IsInitialized
        {
            get { return true; }
        }

        public int Initialize()
        {
            return 0;
        }

        public int UnInitialize()
        {
            return 0;
        }

        public int SaveImage(string path)
        {
            return 0;
        }

        public int SetThreshold(HalconDotNet.HWindow hWindow, HalconDotNet.HObject hImage, HalconDotNet.HTuple row1, HalconDotNet.HTuple col1, HalconDotNet.HTuple row2, HalconDotNet.HTuple col2, HalconDotNet.HTuple min, HalconDotNet.HTuple max)
        {
            return 0;
        }

        public int SetTeaching(HalconDotNet.HWindow hWindow, HalconDotNet.HObject hImage, HalconDotNet.HTuple row1, HalconDotNet.HTuple col1, HalconDotNet.HTuple row2, HalconDotNet.HTuple col2, HalconDotNet.HTuple row11, HalconDotNet.HTuple col11, HalconDotNet.HTuple row12, HalconDotNet.HTuple col12, HalconDotNet.HTuple min, HalconDotNet.HTuple max)
        {
            return 0;
        }

        public int MakeRecipe(HalconDotNet.HWindow hWindow, HalconDotNet.HObject hImage, HalconDotNet.HTuple row1, HalconDotNet.HTuple col1, HalconDotNet.HTuple row2, HalconDotNet.HTuple col2, HalconDotNet.HTuple row11, HalconDotNet.HTuple col11, HalconDotNet.HTuple row12, HalconDotNet.HTuple col12, HalconDotNet.HTuple min, HalconDotNet.HTuple max)
        {
            return 0;
        }

        public int SaveTeach(HalconDotNet.HWindow hWindow, HalconDotNet.HObject hImage)
        {
            return 0;
        }

        public int Inspect(HalconDotNet.HWindow hWindow, HalconDotNet.HObject hImage, out double offsetX, out double offsetY, string path = "")
        {
            offsetX = offsetY = 0;
            return 0;
        }

        public int Inspect(HalconDotNet.HWindow hWindow, HalconDotNet.HObject hImage, out double offsetX, out double offsetY, HalconDotNet.HTuple row1, HalconDotNet.HTuple col1, HalconDotNet.HTuple row2, HalconDotNet.HTuple col2, HalconDotNet.HTuple min, HalconDotNet.HTuple max)
        {
            offsetX = offsetY = 0;
            return 0;
        }

        public int InspectTeach(HalconDotNet.HWindow hWindow, HalconDotNet.HObject hImage, out double offsetX, out double offsetY)
        {
            offsetX = offsetY = 0;
            return 0;
        }

        public int InspectCircle(HalconDotNet.HWindow hWindow, HalconDotNet.HObject hImage, out double offsetX, out double offsetY, HalconDotNet.HTuple row1, HalconDotNet.HTuple col1, HalconDotNet.HTuple row2, HalconDotNet.HTuple col2, HalconDotNet.HTuple min, HalconDotNet.HTuple max, string path = "")
        {
            offsetX = 0.0;
            offsetY = 0.0;

            return 0;
        }

        public int SetTeachingA(HalconDotNet.HWindow hWindow, HalconDotNet.HObject hImage)
        {
            return 0;
        }

        public int SetTeachingB(HalconDotNet.HWindow hWindow, HalconDotNet.HObject hImage)
        {
            return 0;
        }
    }
}
