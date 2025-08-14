using System;

namespace SmartICAVI
{
    interface IMes
    {
        string MachineID { get; set; }
        string SendID { get; set; }
        //DateTime DateTime { get; set; }
        string ProcessType { get; set; }
        string LotID { get; set; }
        string OPID { get; set; }
        string ToolID1 { get; set; }
        string ToolID2 { get; set; }

        int LotUnits { get; set; }
        bool IsLot { get; set; }
        int ToolID1Count { get; set; }
        int ToolID2Count { get; set; }

        int GoodUnits { get; set; }
        int NGUnits { get; set; }
        int LossUnits { get; set; }

        int LotUnits1 { get; set; }
        int GoodUnits1 { get; set; }
        int NGUnits1 { get; set; }
        int LossUnits1 { get; set; }

        int LotUnits2 { get; set; }
        int GoodUnits2 { get; set; }
        int NGUnits2 { get; set; }
        int LossUnits2 { get; set; }

        int LotUnits3 { get; set; }
        int GoodUnits3 { get; set; }
        int NGUnits3 { get; set; }
        int LossUnits3 { get; set; }

        string NextOper { get; set; }
        string MapData1 { get; set; }
        string MapData2 { get; set; }
        string MapData3 { get; set; }

        string Comment { get; set; }

        int MachineStatus { get; set; }

        bool IsReply { get; set; }

        bool IsJobPermission { get; set; }
        string JobPermission { get; set; }

        int TOK { get; set; }
        int TNG { get; set; }
        double YIELD { get; set; }
        int TCNG { get; set; }
        int TJCNT { get; set; }
        string TJPOINT { get; set; }
        int UNITPITCH { get; set; }

        bool IsConnected { get; }

        Object Control { get; }


        //int CountNG1 { get; set; }
        //int CountNG2 { get; set; }
        //int CountNG3 { get; set; }
        //int CountNG4 { get; set; }
        //int CountNG5 { get; set; }
        //int CountNGA { get; set; }
        //int CountNGB { get; set; }
        //int CountNGC { get; set; }
        //int CountNGD { get; set; }
        //int CountNGE { get; set; }
        //int CountNGF { get; set; }
        //int CountNGH { get; set; }
        //int CountNGJ { get; set; }
        //int CountNGK { get; set; }
        //int CountNGL { get; set; }
        //int CountNGM { get; set; }
        //int CountNGN { get; set; }
        //int CountNGO { get; set; }
        //int CountNGP { get; set; }
        //int CountNGQ { get; set; }
        //int CountNGR { get; set; }
        //int CountNGS { get; set; }
        //int CountNGT { get; set; }
        //int CountNGU { get; set; }
        //int CountNGW { get; set; }
        //int CountNGY { get; set; }

        int Initialize();
        int UnInitialize();

        int Send(object command, int value=0, string strValue="");
        int Open();
        int Close();
        int SetParams();

        int SetCountNGs(int[] countNGs);
    }
}
