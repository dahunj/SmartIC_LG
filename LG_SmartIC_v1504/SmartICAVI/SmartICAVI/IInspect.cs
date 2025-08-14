using HalconDotNet;

namespace SmartICAVI
{
    interface IInspect
    {
        bool IsInitialized { get; }

        int Initialize();
        int UnInitialize();

        int SetThreshold(HWindow hWindow, HObject hImage, HTuple row1, HTuple col1, HTuple row2, HTuple col2, HTuple min, HTuple max);
        int SetTeaching(HWindow hWindow, HObject hImage, HTuple row1, HTuple col1, HTuple row2, HTuple col2, HTuple row11, HTuple col11, HTuple row12, HTuple col12, HTuple min, HTuple max);

        int MakeRecipe(HWindow hWindow, HObject hImage, HTuple row1, HTuple col1, HTuple row2, HTuple col2, HTuple row11, HTuple col11, HTuple row12, HTuple col12, HTuple min, HTuple max);

        int SaveTeach(HWindow hWindow, HObject hImage);
        
        int Inspect(HWindow hWindow, HObject hImage, out double offsetX, out double offsetY, string path="");
        int Inspect(HWindow hWindow, HObject hImage, out double offsetX, out double offsetY, HTuple row1, HTuple col1, HTuple row2, HTuple col2, HTuple min, HTuple max);

        int InspectTeach(HWindow hWindow, HObject hImage, out double offsetX, out double offsetY);
        int InspectCircle(HWindow hWindow, HObject hImage, out double offsetX, out double offsetY, HTuple row1, HTuple col1, HTuple row2, HTuple col2, HTuple min, HTuple max, string path="");

        int SetTeachingA(HWindow hWindow, HObject hImage);
        int SetTeachingB(HWindow hWindow, HObject hImage);

        int SaveImage(string path);
    }
}
