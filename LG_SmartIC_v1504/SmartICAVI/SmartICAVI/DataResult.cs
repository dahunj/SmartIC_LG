using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SmartICAVI
{
    class IPoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        public IPoint()
        {
            X = 0;
            Y = 0;
        }

        public IPoint(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    class DataResultRaw
    {
        public int Index { get; set; }
        public string Type { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Value3 { get; set; }
        public double Yield { get; set; }

        public DataResultRaw()
        {
            Index = 0;
            Value1 = "1";
            Value2 = "1";
            Value3 = "";
            Type = "";
            Yield = 0.0;
        }

        public DataResultRaw(int index, string value1, string value2, string type)
        {
            this.Index = index;
            this.Value1 = value1;
            this.Value2 = value2;
            this.Value3 = "";
            this.Type = type;
            Yield = 0.0;
        }

        public DataResultRaw(int index, string value1, string value2, string value3, string type)
        {
            this.Index = index;
            this.Value1 = value1;
            this.Value2 = value2;
            this.Value3 = value3;
            this.Type = type;
            Yield = 0.0;
        }
    }

    class DataResultVision
    {
        private int oldJoint;

        private int tempNG;
        private int countGood;
        private int countJoint;
        private int countCNG;
        private int countTHole;
        private List<DataResultRaw> listRaw;

        private int countGood1;
        private int countGood2;
        private int countTotal1;
        private int countTotal2;

        private bool foundJoint;

        public int ContinuousNG { get; set; }

        public int CountGood { get { return countGood; } }
        public int CountTotal
        {
            get
            {
                if (3 == Line)
                    return listRaw.Count * 3;
                return listRaw.Count * 2;
            }
        }
        public int CountContinuousNG { get { return countCNG; } }
        public int CountJoint { get { return countJoint; } }
        public int CountTHole { get { return countTHole; } }
        public List<DataResultRaw> ListRaw { get { return listRaw; } }

        public int CountGood1 { get { return countGood1; } }
        public int CountGood2 { get { return countGood2; } }
        public int CountTotal1 { get { return countTotal1; } }
        public int CountTotal2 { get { return countTotal2; } }

        public int SectionMinUnits { get; set; }

        public int Line { get; set; }

        public DataResultVision()
        {
            listRaw = new List<DataResultRaw>();
            //listNG = new List<DataResultRaw>();

            ContinuousNG = 8;

            countGood = 0;
            countJoint = 0;
            countCNG = 0;
            oldJoint = 0;
            countTHole = 0;

            tempNG = 0;
            //indexNG1 = -1;
            //indexNG2 = -1;

            //IsJobEnd = false;
            //IndexJobEnd = 0;
            //IsJobCompleted = false;

            SectionMinUnits = 13000;

            Line = 2;
        }

        public void Clear()
        {
            listRaw.Clear();
            //listNG.Clear();

            oldJoint = 0;

            countGood = 0;
            countJoint = 0;
            countCNG = 0;
            countTHole = 0;
            tempNG = 0;

            foundJoint = false;
            countGood1 = 0;
            countGood2 = 0;
            countTotal1 = 0;
            countTotal2 = 0;
            //indexNG1 = -1;
            //indexNG2 = -1;

            //IsJobEnd = false;
            //IndexJobEnd = 0;
            //IsJobCompleted = false;

            Line = 2;
        }

        public void Add(int index, string value1, string value2, string type = "")
        {
            if ("G" == value1 && "G" == value2)
            {
                countGood += 2;

                if (0 != tempNG)
                {
                    tempNG = 0;
                }

                if (false == foundJoint)
                {
                    countGood1 += 2;
                    countTotal1 += 2;
                }
                else
                {
                    countGood2 += 2;
                    countTotal2 += 2;
                }

            }
            else
            {
                if ("G" != value1)
                    ++tempNG;
                else
                    ++countGood;

                if ("G" != value2)
                    ++tempNG;
                else
                    ++countGood;


                if (false == foundJoint)
                {
                    if ("G" == value1)
                        ++countGood1;
                    if ("G" == value2)
                        ++countGood1;

                    countTotal1 += 2;
                }
                else
                {
                    if ("G" == value1)
                        ++countGood2;
                    if ("G" == value2)
                        ++countGood2;

                    countTotal2 += 2;
                }
            }

            if ("BB006" == value1 || "BB006" == value2)
            {
                if (0 == oldJoint)  // 연속으로 나온 조인트는 카운트 하지 않는다. 
                {
                    ++countJoint;

                    if (countTotal1 > SectionMinUnits)
                        foundJoint = true;
                }
                ++oldJoint;
            }
            else
            {
                oldJoint = 0;
            }

            if ("BB039" == value1) ++countTHole;
            if ("BB039" == value2) ++countTHole;

            DataResultRaw raw = new DataResultRaw(index, value1, value2, type);
            raw.Yield = ((double)(countGood) / (double)(listRaw.Count * 2 + 2)) * 100.0;

            //if ("BB006" == value1)
            //    raw.Yield1 = raw.Yield;

            listRaw.Add(raw);
        }

        public void Add3(int index, string value1, string value2, string value3, string type = "")
        {
            if ("G" == value1 && "G" == value2 && "G" == value3)
            {
                countGood += 3;

                if (0 != tempNG)
                {
                    tempNG = 0;
                }

                if (false == foundJoint)
                {
                    countGood1 += 3;
                    countTotal1 += 3;
                }
                else
                {
                    countGood2 += 3;
                    countTotal2 += 3;
                }

            }
            else
            {
                if ("G" != value1)
                    ++tempNG;
                else
                    ++countGood;

                if ("G" != value2)
                    ++tempNG;
                else
                    ++countGood;

                if ("G" != value3)
                    ++tempNG;
                else
                    ++countGood;


                if (false == foundJoint)
                {
                    if ("G" == value1)
                        ++countGood1;
                    if ("G" == value2)
                        ++countGood1;
                    if ("G" == value3)
                        ++countGood1;

                    countTotal1 += 3;
                }
                else
                {
                    if ("G" == value1)
                        ++countGood2;
                    if ("G" == value2)
                        ++countGood2;
                    if ("G" == value3)
                        ++countGood2;

                    countTotal2 += 3;
                }
            }

            if ("BB006" == value1 || "BB006" == value2 || "BB006" == value3)
            {
                if (0 == oldJoint)  // 연속으로 나온 조인트는 카운트 하지 않는다. 
                {
                    ++countJoint;

                    if (countTotal1 > SectionMinUnits)
                        foundJoint = true;
                }
                ++oldJoint;
            }
            else
            {
                oldJoint = 0;
            }

            if ("BB039" == value1) ++countTHole;
            if ("BB039" == value2) ++countTHole;
            if ("BB039" == value3) ++countTHole;

            DataResultRaw raw = new DataResultRaw(index, value1, value2, value3, type);
            raw.Yield = ((double)(countGood) / (double)(listRaw.Count * 3 + 3)) * 100.0;

            //if ("BB006" == value1)
            //    raw.Yield1 = raw.Yield;

            listRaw.Add(raw);
        }

        // index 이후 데이터를 모두 지운다.
        public void Remove(int index)
        {
            if (3 == Line)
            {
                Remove3(index);
                return;
            }

            // 조인트 관련 처리해 줘야 함. 
            if (0 > index)
                return;

            if (index > listRaw.Count)
                return;

            // index 까지 의 모든 데이터 삭제
            for (int i = listRaw.Count - 1; i >= index; --i)
            {
                listRaw.RemoveAt(i);
            }

            // Good Count 와 Joint Count 를 다시 계산한다.
            countGood = 0;
            countJoint = 0;
            oldJoint = 0;
            countTHole = 0;

            for (int i = 0; i < listRaw.Count; ++i)
            {
                if ("G" == listRaw[i].Value1)
                    ++countGood;
                if ("G" == listRaw[i].Value2)
                    ++countGood;

                if ("BB006" == listRaw[i].Value1 || "BB006" == listRaw[i].Value2)
                {
                    if (0 == oldJoint)  // 연속으로 나온 조인트는 카운트 하지 않는다. 
                    {
                        ++countJoint;
                    }
                    ++oldJoint;
                }
                else
                {
                    oldJoint = 0;
                }

                if ("BB039" == listRaw[i].Value1) ++countTHole;
                if ("BB039" == listRaw[i].Value2) ++countTHole;
            }
        }

        public void Remove3(int index)
        {
            // 조인트 관련 처리해 줘야 함. 
            if (0 > index)
                return;

            if (index > listRaw.Count)
                return;

            // index 까지 의 모든 데이터 삭제
            for (int i = listRaw.Count - 1; i >= index; --i)
            {
                listRaw.RemoveAt(i);
            }

            // Good Count 와 Joint Count 를 다시 계산한다.
            countGood = 0;
            countJoint = 0;
            oldJoint = 0;
            countTHole = 0;

            for (int i = 0; i < listRaw.Count; ++i)
            {
                if ("G" == listRaw[i].Value1)
                    ++countGood;
                if ("G" == listRaw[i].Value2)
                    ++countGood;
                if ("G" == listRaw[i].Value3)
                    ++countGood;

                if ("BB006" == listRaw[i].Value1 || "BB006" == listRaw[i].Value2 || "BB006" == listRaw[i].Value3)
                {
                    if (0 == oldJoint)  // 연속으로 나온 조인트는 카운트 하지 않는다. 
                    {
                        ++countJoint;
                    }
                    ++oldJoint;
                }
                else
                {
                    oldJoint = 0;
                }

                if ("BB039" == listRaw[i].Value1) ++countTHole;
                if ("BB039" == listRaw[i].Value2) ++countTHole;
                if ("BB039" == listRaw[i].Value3) ++countTHole;
            }
        }

        public void Modify1(int index, string value1)
        {
            if (0 > index)
                return;

            if (index > listRaw.Count - 1)
                return;

            if (listRaw[index].Value1 != value1)
            {
                listRaw[index].Value1 = value1;

                if ("G" == value1)
                    ++countGood;

                Modify(index);
            }//if (listRaw[index].Value1 != value1)
        }

        public void Modify2(int index, string value2)
        {
            if (0 > index)
                return;

            if (index > listRaw.Count - 1)
                return;

            if (listRaw[index].Value2 != value2)
            {
                listRaw[index].Value2 = value2;

                if ("G" == value2)
                    ++countGood;

                Modify(index);
            }//if (listRaw[index].Value1 != value1)
        }

        public void Modify3(int index, string value3)
        {
            if (0 > index)
                return;

            if (index > listRaw.Count - 1)
                return;

            if (listRaw[index].Value3 != value3)
            {
                listRaw[index].Value3 = value3;

                if ("G" == value3)
                    ++countGood;

                Modify(index);
            }//if (listRaw[index].Value1 != value1)
        }

        private void Modify(int index)
        {
            // - 값이 입력되므로 확인필요
            // must check
            return;

            //for (int indexNG = 0; indexNG < listNG.Count; ++indexNG)
            //{
            //    if ((listNG[indexNG].Value1) <= index && (listNG[indexNG].Value2 >= index))
            //    {
            //        int k = 0;
            //        int temp = 0;
            //        int end = 0;

            //        bool findFront = false;
            //        bool findRear = false;
            //        bool findSeperate = false;

            //        int frontValue1 = listNG[indexNG].Value1;
            //        int frontValue2 = listNG[indexNG].Value2;

            //        int rearValue1 = listNG[indexNG].Value1;
            //        int rearValue2 = listNG[indexNG].Value2;

            //        for (k = listNG[indexNG].Value1; k <= listNG[indexNG].Value2; ++k)
            //        {
            //            if (0 != listRaw[k].Value1)
            //                ++temp;
            //            if (0 != listRaw[k].Value2)
            //                ++temp;

            //            if ((0 == listRaw[k].Value1) && (0 == listRaw[k].Value2))
            //            {
            //                findSeperate = true;
            //                end = k;
            //                break;
            //            }
            //        }

            //        // 기존의 연속불량 데이터가 분리되었다면 다시 계산한다. 
            //        if (true == findSeperate)
            //        {
            //            // 앞쪽 데이터가 연속 불량에 만족할 경우 
            //            if (temp >= ContinuousNG)
            //            {
            //                findFront = true;
            //                frontValue2 = end - 1;
            //            }

            //            // 분리된 뒤쪽 데이터를 서치한다.
            //            temp = 0;
            //            rearValue1 = end + 1;
            //            for (k = rearValue1; k <= listNG[indexNG].Value2; ++k)
            //            {
            //                if (0 != listRaw[k].Value1)
            //                    ++temp;
            //                if (0 != listRaw[k].Value2)
            //                    ++temp;
            //            }
            //            if (temp >= ContinuousNG)
            //            {
            //                findRear = true;
            //            }

            //            // 분리된 데이터 모두 연속불량에 만족할 경우
            //            if (true == findRear && true == findFront)
            //            {
            //                // Rear 데이터를 기존데이터에 적용한다. 
            //                listNG[indexNG].Value1 = rearValue1;
            //                listNG[indexNG].Value2 = rearValue2;

            //                // Front 데이터를 새로 만들어 삽입한다.
            //                DataResultRaw ngData = new DataResultRaw(frontValue1, frontValue2);
            //                listNG.Insert(indexNG, ngData);
            //            }
            //            else if (true == findFront)
            //            {
            //                listNG[indexNG].Value1 = frontValue1;
            //                listNG[indexNG].Value2 = frontValue2;
            //            }
            //            else if (true == findFront)
            //            {
            //                listNG[indexNG].Value1 = rearValue1;
            //                listNG[indexNG].Value2 = rearValue2;
            //            }
            //            else
            //            {
            //                // 연속불량 데이터를 만족하지 못하므로 삭제한다. 
            //                listNG.RemoveAt(indexNG);
            //            }
            //        }
            //        break;
            //    }// if ((listNG[i].Value1) <= index && (listNG[i].Value2 >= index))
            //}// for (int i = 0; i < listNG.Count; ++i)
        }

        public int Save(StreamWriter writer)
        {
            string item;

            double yieldGood = 0.0;
            double yieldNG = 0.0;
            int ngCount = CountTotal - CountGood;

            int length = ListRaw.Count;
            int end = 50;

            if (0 >= CountTotal)
                return -1;

            yieldGood = (double)(CountGood * 100) / (double)(CountTotal);

            yieldNG = (double)(ngCount * 100) / (double)(CountTotal);

            item = "Version,1.0.0.2";
            writer.WriteLine(item);

            item = "검사수," + CountTotal.ToString();
            writer.WriteLine(item);

            item = "OK수," + CountGood.ToString();
            writer.WriteLine(item);

            item = "NG수," + ngCount.ToString();
            writer.WriteLine(item);

            item = "정상수율," + yieldGood.ToString("0.00");
            writer.WriteLine(item);

            item = "불량수율," + yieldNG.ToString("0.00");
            writer.WriteLine(item);

            item = "연속불량," + CountContinuousNG.ToString();
            writer.WriteLine(item);

            item = "조인트연결," + CountJoint.ToString();
            writer.WriteLine(item);

            // Version 1.0.0.1
            item = "쓰루홀," + CountTHole.ToString();
            writer.WriteLine(item);

            // Version 1.0.0.2
            item = "열수," + Line.ToString();
            writer.WriteLine(item);

            ////                 5         10         15         20         25         30         35         40         45         50

            ////      1: X X G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G

            ////     51: X G G G G  G G G G G  G G G G G  G G G G G  X G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G

            ////    101: X G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G

            item = " ,1, , , ,5, , , , ,10, , , , ,15, , , , ,20, , , , ,25, , , , ,30, , , , ,35, , , , ,40, , , , ,45, , , , ,50";
            writer.WriteLine(item);


            if (3 == Line)
            {
                for (int i = 0; i < length; i += 50)
                {
                    end = i + 50;

                    if (length < end)
                        end = length;

                    // Value1
                    item = (i + 1).ToString() + ",";
                    for (int j = i; j < end; ++j)
                    {
                        //if (0 == ListRaw[j].Value1)
                        //    item += "G,";
                        //else
                        //    item += "X,";
                        item += ListRaw[j].Value1 + ",";
                    }
                    writer.WriteLine(item);

                    // Value2
                    item = " ,";
                    for (int j = i; j < end; ++j)
                    {
                        //if (0 == ListRaw[j].Value2)
                        //    item += "G,";
                        //else
                        //    item += "X,";
                        item += ListRaw[j].Value2 + ",";
                    }
                    writer.WriteLine(item);


                    // Value3
                    item = " ,";
                    for (int j = i; j < end; ++j)
                    {
                        //if (0 == ListRaw[j].Value2)
                        //    item += "G,";
                        //else
                        //    item += "X,";
                        item += ListRaw[j].Value3 + ",";
                    }
                    writer.WriteLine(item);
                }
            }
            else
            {
                for (int i = 0; i < length; i += 50)
                {
                    end = i + 50;

                    if (length < end)
                        end = length;

                    // Value1
                    item = (i + 1).ToString() + ",";
                    for (int j = i; j < end; ++j)
                    {
                        //if (0 == ListRaw[j].Value1)
                        //    item += "G,";
                        //else
                        //    item += "X,";
                        item += ListRaw[j].Value1 + ",";
                    }
                    writer.WriteLine(item);

                    // Value2
                    item = " ,";
                    for (int j = i; j < end; ++j)
                    {
                        //if (0 == ListRaw[j].Value2)
                        //    item += "G,";
                        //else
                        //    item += "X,";
                        item += ListRaw[j].Value2 + ",";
                    }
                    writer.WriteLine(item);
                }
            }
            return 0;
        }

        public int Load(StreamReader reader)
        {
            string item;
            string item2;
            string item3 = "";

            int countTotal = 0;
            //int end = 0;
            int countJoint = 0;

            //int value1 = 0;
            //int value2 = 0;
            int index = 0;

            Clear();

            //item = "검사수," + CountTotal.ToString();
            item = reader.ReadLine();

            if (-1 != item.IndexOf("Version,"))
                item = reader.ReadLine();

            if (-1 != item.IndexOf("검사수,"))
            {
                countTotal = int.Parse(item.Replace("검사수,", "")) / 2;
            }

            // 검사 데이터가 없을 경우 바로 리턴
            if (1 > countTotal)
                return 0;

            string[] arrays = new string[200];
            string[] arrays2 = new string[200];
            string[] arrays3 = new string[200];



            //item = "OK수," + CountGood.ToString();
            item = reader.ReadLine();

            //item = "NG수," + ngCount.ToString();
            item = reader.ReadLine();

            //item = "정상수율," + yieldGood.ToString("0.00");
            item = reader.ReadLine();

            //item = "불량수율," + yieldNG.ToString("0.00");
            item = reader.ReadLine();

            //item = "연속불량," + CountContinuousNG.ToString();
            item = reader.ReadLine();

            //item = "조인트연결," + CountJoint.ToString();
            item = reader.ReadLine();
            if (-1 != item.IndexOf("조인트연결,"))
            {
                countJoint = int.Parse(item.Replace("조인트연결,", ""));
            }

            ////                 5         10         15         20         25         30         35         40         45         50

            ////      1: X X G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G

            ////     51: X G G G G  G G G G G  G G G G G  G G G G G  X G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G

            ////    101: X G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G

            //item = " ,1, , , ,5, , , , ,10, , , , ,15, , , , ,20, , , , ,25, , , , ,30, , , , ,35, , , , ,40, , , , ,45, , , , ,50";
            item = reader.ReadLine();

            if (-1 != item.IndexOf("쓰루홀"))
            {
                countTHole = int.Parse(item.Replace("쓰루홀,", ""));

                item = reader.ReadLine();
            }

            // Version 1.0.0.2
            if (-1 != item.IndexOf("열수"))
            {
                Line = int.Parse(item.Replace("열수,", ""));

                item = reader.ReadLine();
            }


            if (3 == Line)
            {
                for (int i = 0; i < countTotal; i += 50)
                {
                    // Value1
                    item = reader.ReadLine();
                    // Value2
                    item2 = reader.ReadLine();
                    // Value3
                    item3 = reader.ReadLine();

                    arrays = item.Split(',');
                    arrays2 = item2.Split(',');
                    arrays3 = item3.Split(',');

                    if (arrays.Length != arrays2.Length)
                        return -1;
                    if (arrays3.Length != arrays2.Length)
                        return -1;

                    for (int k = 1; k < arrays.Length - 1; ++k)
                    {
                        index = i + k - 1;

                        Add3(index, arrays[k], arrays2[k], arrays3[k]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < countTotal; i += 50)
                {
                    // Value1
                    item = reader.ReadLine();
                    // Value2
                    item2 = reader.ReadLine();

                    arrays = item.Split(',');
                    arrays2 = item2.Split(',');

                    if (arrays.Length != arrays2.Length)
                        return -1;

                    for (int k = 1; k < arrays.Length - 1; ++k)
                    {
                        index = i + k - 1;

                        Add(index, arrays[k], arrays2[k]);
                    }
                }
            }
            return 0;
        }
    }

    class DataResult
    {
        //private DataService dataService = null;

        private int countCNG;
        private int startCNG;
        private int startMissPrint;
        private bool isMissPrint;

        public DataResultVision Top;
        public DataResultVision Bottom;
        public DataResultVision Mono;
        public DataResultVision Total;
        public DataResultVision Review;
        public DataResultVision Temp;

        public List<IPoint> listCNG;
        public List<IPoint> listMissPrint;

        public List<IPoint> listLightTop;
        public List<IPoint> listLightBottom;
        public List<IPoint> listLightMono;

        public bool IsSelectedTop { get; set; }
        public bool IsSelectedBottom { get; set; }
        public bool IsSelectedMono { get; set; }
        public bool IsSelectedReview { get; set; }
        public bool IsSelectedPunch { get; set; }

        public bool IsStarted { get; set; }
        public bool IsCompleted { get; set; }

        public string RecipeName { get; set; }
        public string LotID { get; set; }
        public int PF { get; set; }
        public int Line
        {
            get
            {
                return Top.Line;
            }
            set
            {
                Top.Line = value;
                Bottom.Line = value;
                Mono.Line = value;
                Total.Line = value;
                Review.Line = value;
                Temp.Line = value;

            }
        }
        public int CNGLimit { get; set; }

        // 2020.07.14 khs - 너무 긴 불량일때 연속불량 구간 리스트에 저장 되기전에 인덱스가 넘어가 연속불량 안뜨는 현상 보완
        public int m_nCountCNG { get; set; }
        public int m_nStartCNG { get; set; }

        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }

        public int CountPunch { get; set; }

        public string NextProcess { get; set; }

        public int[] CountNGs;
        public string[] CountNGIDs;
        public string[] CountNGNames;

        public int IndexLastPunch { get; set; }   // 가장 최근의 펀치 Index
        public string LineLastPunch { get; set; }   // 가장 최근의 펀치 Line

        public int IndexCurPunch { get; set; }    // 현재 펀칭하고 있는 Index : IndexCurPunch 와 IndexLastPunch 가 일치할 경우 Line 까지 고려해야함. 다를경우 Restart 시 IndexCurPunch 부터 재 검사

        private int sectionMinUnits = 13000;
        public int SectionMinUnits // 1구간 최소 유닛수
        {
            get { return sectionMinUnits; }
            set
            {
                sectionMinUnits = value;

                if (null != Top)
                    Top.SectionMinUnits = value;
                if (null != Bottom)
                    Bottom.SectionMinUnits = value;
                if (null != Mono)
                    Mono.SectionMinUnits = value;
                if (null != Total)
                    Total.SectionMinUnits = value;
                if (null != Review)
                    Review.SectionMinUnits = value;
                if (null != Temp)
                    Temp.SectionMinUnits = value;
            }
        }


        public string MapData1
        {
            get {
                if (Total.ListRaw.Count < 1) return "";

                string ret = "";
                for (int i = 0; i < Total.ListRaw.Count; ++i) {
                    if      (Total.ListRaw[i].Value1 == "G")     ret += "G";
                    else if (Total.ListRaw[i].Value1 == "BB006") ret += "J"; // Joint 추가 (cskim, 2024-05-09)
                    else if (Total.ListRaw[i].Value1 == "BB039") ret += "T"; // Through Hole 추가 (cskim, 2024-05-09)
                    else                                         ret += "X";
                }
                return ret;
            }
        }

        public string MapData2
        {
            get {
                if (Total.ListRaw.Count < 1) return "";

                string ret = "";
                for (int i = 0; i < Total.ListRaw.Count; ++i) {
                    if      (Total.ListRaw[i].Value2 == "G")     ret += "G";
                    else if (Total.ListRaw[i].Value2 == "BB006") ret += "J"; // Joint 추가 (cskim, 2024-05-09)
                    else if (Total.ListRaw[i].Value2 == "BB039") ret += "T"; // Through Hole 추가 (cskim, 2024-05-09)
                    else                                         ret += "X";
                }
                return ret;
            }
        }

        public string MapData3
        {
            get {
                if (Total.ListRaw.Count < 1) return "";

                string ret = "";
                for (int i = 0; i < Total.ListRaw.Count; ++i) {
                    if      (Total.ListRaw[i].Value3 == "G")     ret += "G";
                    else if (Total.ListRaw[i].Value3 == "BB006") ret += "J"; // Joint 추가 (cskim, 2024-05-09)
                    else if (Total.ListRaw[i].Value3 == "BB039") ret += "T"; // Through Hole 추가 (cskim, 2024-05-09)
                    else                                         ret += "X";
                }
                return ret;
            }
        }

        public DataResult()
        {
            Top = new DataResultVision();
            Bottom = new DataResultVision();
            Mono = new DataResultVision();
            Total = new DataResultVision();
            Review = new DataResultVision();
            Temp = new DataResultVision();

            TimeStart = DateTime.Now;
            TimeEnd = DateTime.Now;

            RecipeName = "";
            LotID = "";
            PF = 2;
            Line = 2;
            CNGLimit = 8;

            IsStarted = false;
            IsCompleted = false;

            IsSelectedTop = true;
            IsSelectedBottom = true;
            IsSelectedMono = true;
            IsSelectedReview = true;
            IsSelectedPunch = true;

            listCNG = new List<IPoint>();
            listMissPrint = new List<IPoint>();

            listLightTop = new List<IPoint>();
            listLightBottom = new List<IPoint>();
            listLightMono = new List<IPoint>();

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


            CountPunch = 0;

            countCNG = 0;
            startCNG = 0;

            m_nCountCNG = 0;
            m_nStartCNG = 0;

            SectionMinUnits = 13000;

            Clear();
        }

        public void Clear()
        {
            Top.Clear();
            Bottom.Clear();
            Mono.Clear();
            Total.Clear();
            Review.Clear();
            Temp.Clear();

            listCNG.Clear();
            listMissPrint.Clear();
            listLightTop.Clear();
            listLightBottom.Clear();
            listLightMono.Clear();

            TimeStart = DateTime.Now;
            TimeEnd = DateTime.Now;

            RecipeName = "";
            LotID = "";
            PF = 2;
            Line = 2;

            IsStarted = false;
            IsCompleted = false;

            isMissPrint = false;
            startMissPrint = 0;

            CountPunch = 0;

            countCNG = 0;
            startCNG = 0;

            m_nCountCNG = 0;
            m_nStartCNG = 0;

            for (int i = 0; i < 63; ++i)
            {
                CountNGs[i] = 0;
            }

            IndexLastPunch = -1;
            LineLastPunch = "";

            IndexCurPunch = -1;

            SectionMinUnits = 13000;
        }

        public void AddTop(int index, string value1, string value2, string type = "")
        {
            // T (Through Hole -> BB039 변환)
            //if ("T" == value1) value1 = "BB039";
            //if ("T" == value2) value2 = "BB039";

            if ("JT" == value1) value1 = "BB006";
            if ("JT" == value2) value2 = "BB006";

            Log_Top.WriteLine("{0},{1},{2},{3}", index, value1, value2, type);
            Top.Add(index, value1, value2, type);
        }

        public void AddTop3(int index, string value1, string value2, string value3, string type = "")
        {
            // T (Through Hole -> BB039 변환)
            //if ("T" == value1) value1 = "BB039";
            //if ("T" == value2) value2 = "BB039";
            //if ("T" == value3) value3 = "BB039";

            if ("JT" == value1) value1 = "BB006";
            if ("JT" == value2) value2 = "BB006";
            if ("JT" == value3) value3 = "BB006";

            Log_Top.WriteLine("{0},{1},{2},{3},{4}", index, value1, value2, value3, type);
            Top.Add3(index, value1, value2, value3, type);
        }

        public void AddBottom(int index, string value1, string value2, string type = "")
        {
            // T (Through Hole -> BB039 변환)
            //if ("T" == value1) value1 = "BB039";
            //if ("T" == value2) value2 = "BB039";

            if ("JT" == value1) value1 = "BB006";
            if ("JT" == value2) value2 = "BB006";

            Log_Bottom.WriteLine("{0},{1},{2},{3}", index, value1, value2, type);
            Bottom.Add(index, value1, value2, type);
        }

        public void AddBottom3(int index, string value1, string value2, string value3, string type = "")
        {
            // T (Through Hole -> BB039 변환)
            //if ("T" == value1) value1 = "BB039";
            //if ("T" == value2) value2 = "BB039";
            //if ("T" == value3) value3 = "BB039";

            if ("JT" == value1) value1 = "BB006";
            if ("JT" == value2) value2 = "BB006";
            if ("JT" == value3) value3 = "BB006";

            Log_Bottom.WriteLine("{0},{1},{2},{3},{4}", index, value1, value2, value3, type);
            Bottom.Add3(index, value1, value2, value3, type);
        }

        public void AddMono(int index, string value1, string value2, string type = "")
        {
            // T (Through Hole -> BB039 변환)
            //if ("T" == value1) value1 = "BB039";
            //if ("T" == value2) value2 = "BB039";

            if ("JT" == value1) value1 = "BB006";
            if ("JT" == value2) value2 = "BB006";

            Log_Mono.WriteLine("{0},{1},{2},{3}", index, value1, value2, type);
            Mono.Add(index, value1, value2, type);
        }

        public void AddMono3(int index, string value1, string value2, string value3, string type = "")
        {
            // T (Through Hole -> BB039 변환)
            //if ("T" == value1) value1 = "BB039";
            //if ("T" == value2) value2 = "BB039";
            //if ("T" == value3) value3 = "BB039";

            if ("JT" == value1) value1 = "BB006";
            if ("JT" == value2) value2 = "BB006";
            if ("JT" == value3) value3 = "BB006";

            Log_Mono.WriteLine("{0},{1},{2},{3},{4}", index, value1, value2, value3, type);
            Mono.Add3(index, value1, value2, value3, type);
        }

        public void AddReview(int index, string value1, string value2, string type = "")
        {
            //Log_Review.WriteLine("{0},{1},{2},{3}", index, value1, value2, type);
            Review.Add(index, value1, value2, type);

            // 연속불량 설정.
            if (0 < CNGLimit)
            {
                if (("G" != value1 && "BB006" != value1) || ("G" != value2 && "BB006" != value2))
                {
                    if (0 == countCNG)
                    {
                        startCNG = index;
                        m_nStartCNG = index;
                    }

                    if ("G" != value1)
                    {
                        ++countCNG;
                        ++m_nCountCNG;
                    }
                    if ("G" != value2)
                    {
                        ++countCNG;
                        ++m_nCountCNG;
                    }
                }
                else
                {
                    if (CNGLimit <= countCNG)
                    {
                        IPoint pt = new IPoint(startCNG, index - 1);
                        listCNG.Add(pt);
                    }

                    countCNG = 0;
                    m_nCountCNG = 0;
                }
            }

            // 노광편차 설정.
            if (false == isMissPrint)
            {
                if ("BB019" == value1 || "BB019" == value2)
                {
                    isMissPrint = true;
                    startMissPrint = index;
                }
            }
            else
            {
                if ("BB019" != value1 && "BB019" != value2)
                {
                    isMissPrint = false;

                    IPoint pt = new IPoint(startMissPrint, index - 1);
                    listMissPrint.Add(pt);
                }
            }

            SetCountNG(value1);
            SetCountNG(value2);
        }

        public void AddReview3(int index, string value1, string value2, string value3, string type = "")
        {
            //Log_Review.WriteLine("{0},{1},{2},{3},{4}", index, value1, value2, value3, type);
            Review.Add3(index, value1, value2, value3, type);

            // 연속불량 설정.
            if (0 < CNGLimit)
            {
                if (("G" != value1 && "BB006" != value1) || ("G" != value2 && "BB006" != value2) || ("G" != value3 && "BB006" != value3))
                {
                    if (0 == countCNG)
                    {
                        startCNG = index;
                        m_nStartCNG = index;
                    }

                    if ("G" != value1)
                    {
                        ++countCNG;
                        ++m_nCountCNG;
                    }
                    if ("G" != value2)
                    {
                        ++countCNG;
                        ++m_nCountCNG;
                    }
                    if ("G" != value3)
                    {
                        ++countCNG;
                        ++m_nCountCNG;
                    }
                }
                else
                {
                    if (CNGLimit <= countCNG)
                    {
                        IPoint pt = new IPoint(startCNG, index - 1);
                        listCNG.Add(pt);
                    }

                    countCNG = 0;
                    m_nCountCNG = 0;
                }
            }

            // 노광편차 설정.
            if (false == isMissPrint)
            {
                if ("BB019" == value1 || "BB019" == value2 || "BB019" == value3)
                {
                    isMissPrint = true;
                    startMissPrint = index;
                }
            }
            else
            {
                if ("BB019" != value1 && "BB019" != value2 && "BB019" != value3)
                {
                    isMissPrint = false;

                    IPoint pt = new IPoint(startMissPrint, index - 1);
                    listMissPrint.Add(pt);
                }
            }

            SetCountNG(value1);
            SetCountNG(value2);
            SetCountNG(value3);
        }

        public void AddTemp(int index, string value1, string value2, string type = "")
        {
            Log_Temp.WriteLine("{0},{1},{2},{3}", index, value1, value2, type);
            Temp.Add(index, value1, value2, type);
        }

        public void AddTemp3(int index, string value1, string value2, string value3, string type = "")
        {
            Log_Temp.WriteLine("{0},{1},{2},{3},{4}", index, value1, value2, value3, type);
            Temp.Add3(index, value1, value2, value3, type);
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

        public int AddTotal(int index)
        {
            if (3 == Line)
            {
                return AddTotal3(index);
            }

            // Review 데이터 적용
            if (index > Review.ListRaw.Count)
                return -1;

            int ret = 0;

            if ("G" != Review.ListRaw[index].Value1)
                ++ret;
            if ("G" != Review.ListRaw[index].Value2)
                ++ret;

            if (index == 0 && "G" == Review.ListRaw[index].Value2 && "G" == Review.ListRaw[index].Value2)
            {
                ret = 2;
            }

            Total.Add(index, Review.ListRaw[index].Value1, Review.ListRaw[index].Value2, Review.ListRaw[index].Type);

            return ret;
        }

        public int AddTotal3(int index)
        {
            // Review 데이터 적용
            if (index > Review.ListRaw.Count)
                return -1;

            int ret = 0;

            if ("G" != Review.ListRaw[index].Value1)
                ++ret;
            if ("G" != Review.ListRaw[index].Value2)
                ++ret;
            if ("G" != Review.ListRaw[index].Value3)
                ++ret;

            if (index == 0 && "G" == Review.ListRaw[index].Value2 && "G" == Review.ListRaw[index].Value2 && "G" == Review.ListRaw[index].Value3)
            {
                ret = 3;
            }

            Total.Add3(index, Review.ListRaw[index].Value1, Review.ListRaw[index].Value2, Review.ListRaw[index].Value3, Review.ListRaw[index].Type);

            return ret;
        }

        public int AddPunch(int index, string line)
        {
            return ++CountPunch;
        }

        public void Remove(int index)
        {
            Top.Remove(index);
            Bottom.Remove(index);
            Mono.Remove(index);
            Temp.Remove(index);
            Review.Remove(index);
            Total.Remove(index);

            RemoveLightTop(index);
            RemoveLightBottom(index);
            RemoveLightMono(index);
        }

        private void RemoveLightTop(int index)
        {
            if (0 < listLightTop.Count)
            {
                int start = listLightTop.Count - 1;

                for (int i = start; i >= 0; --i)
                {
                    if (index <= listLightTop[i].X)
                        listLightTop.RemoveAt(i);
                    else
                        break;
                }
            }
        }

        private void RemoveLightBottom(int index)
        {
            if (0 < listLightBottom.Count)
            {
                int start = listLightBottom.Count - 1;

                for (int i = start; i >= 0; --i)
                {
                    if (index <= listLightBottom[i].X)
                        listLightBottom.RemoveAt(i);
                    else
                        break;
                }
            }
        }

        private void RemoveLightMono(int index)
        {
            if (0 < listLightMono.Count)
            {
                int start = listLightMono.Count - 1;

                for (int i = start; i >= 0; --i)
                {
                    if (index <= listLightMono[i].X)
                        listLightMono.RemoveAt(i);
                    else
                        break;
                }
            }
        }

        public void Modify1(int index, string value1)
        {
            // Good 일 경우에만 TOP,Bottom,Mono 적용
            if ("G" == value1)
            {
                Top.Modify1(index, value1);
                Bottom.Modify1(index, value1);
                Mono.Modify1(index, value1);
            }
            Temp.Modify1(index, value1);
            //Total.Modify1(index, value1);
        }

        public void Modify2(int index, string value2)
        {
            // Good 일 경우에만 TOP,Bottom,Mono 적용
            if ("G" == value2)
            {
                Top.Modify2(index, value2);
                Bottom.Modify2(index, value2);
                Mono.Modify2(index, value2);
            }
            Temp.Modify2(index, value2);
            //Total.Modify2(index, value2);
        }

        public void Modify3(int index, string value3)
        {
            // Good 일 경우에만 TOP,Bottom,Mono 적용
            if ("G" == value3)
            {
                Top.Modify3(index, value3);
                Bottom.Modify3(index, value3);
                Mono.Modify3(index, value3);
            }
            Temp.Modify3(index, value3);
            //Total.Modify2(index, value2);
        }

        // index 까지의 데이터를 모두 지운다.
        public void Restart(int index)
        {
            if (3 == Line)
            {
                Restart3(index);
                return;
            }

            if (0 > index)
                index = 0;

            // Remove ListTop
            Remove(index);

            // Clear ListCNG
            listCNG.Clear();
            listMissPrint.Clear();

            isMissPrint = false;
            startMissPrint = 0;

            for (int i = 0; i < CountNGs.Length; ++i)
                CountNGs[i] = 0;

            // CNG 재 계산
            countCNG = 0;
            m_nCountCNG = 0;

            for (int i = 0; i < Review.ListRaw.Count; ++i)
            {
                if (0 < CNGLimit)
                {
                    if (("G" != Review.ListRaw[i].Value1 && "BB006" != Review.ListRaw[i].Value1) || ("G" != Review.ListRaw[i].Value2 && "BB006" != Review.ListRaw[i].Value2))
                    {
                        if (0 == countCNG)
                        {
                            startCNG = i;
                            m_nStartCNG = i;
                        }

                        if ("G" != Review.ListRaw[i].Value1)
                        {
                            ++countCNG;
                            ++m_nCountCNG;
                        }
                        if ("G" != Review.ListRaw[i].Value2)
                        {
                            ++countCNG;
                            ++m_nCountCNG;
                        }
                    }
                    else
                    {
                        if (CNGLimit <= countCNG)
                        {
                            IPoint pt = new IPoint(startCNG, i - 1);
                            listCNG.Add(pt);
                        }

                        countCNG = 0;
                        m_nCountCNG = 0;
                    }
                }

                // 노광편차 설정.
                if (false == isMissPrint)
                {
                    if ("BB019" == Review.ListRaw[i].Value1 || "BB019" == Review.ListRaw[i].Value2)
                    {
                        isMissPrint = true;
                        startMissPrint = i;
                    }
                }
                else
                {
                    if ("BB019" != Review.ListRaw[i].Value1 && "BB019" != Review.ListRaw[i].Value2)
                    {
                        isMissPrint = false;

                        IPoint pt = new IPoint(startMissPrint, i - 1);
                        listMissPrint.Add(pt);
                    }
                }

                SetCountNG(Review.ListRaw[i].Value1);
                SetCountNG(Review.ListRaw[i].Value2);
            }
        }

        public void Restart3(int index)
        {
            if (0 > index)
                index = 0;

            // Remove ListTop
            Remove(index);

            // Clear ListCNG
            listCNG.Clear();
            listMissPrint.Clear();

            isMissPrint = false;
            startMissPrint = 0;

            for (int i = 0; i < CountNGs.Length; ++i)
                CountNGs[i] = 0;

            // CNG 재 계산
            countCNG = 0;
            m_nCountCNG = 0;

            for (int i = 0; i < Review.ListRaw.Count; ++i)
            {
                if (0 < CNGLimit)
                {
                    if (("G" != Review.ListRaw[i].Value1 && "BB006" != Review.ListRaw[i].Value1) || ("G" != Review.ListRaw[i].Value2 && "BB006" != Review.ListRaw[i].Value2) || ("G" != Review.ListRaw[i].Value3 && "BB006" != Review.ListRaw[i].Value3))
                    {
                        if (0 == countCNG)
                        {
                            startCNG = i;
                            m_nStartCNG = i;
                        }

                        if ("G" != Review.ListRaw[i].Value1)
                        {
                            ++countCNG;
                            ++m_nCountCNG;
                        }
                        if ("G" != Review.ListRaw[i].Value2)
                        {
                            ++countCNG;
                            ++m_nCountCNG;
                        }
                        if ("G" != Review.ListRaw[i].Value3)
                        {
                            ++countCNG;
                            ++m_nCountCNG;
                        }
                    }
                    else
                    {
                        if (CNGLimit <= countCNG)
                        {
                            IPoint pt = new IPoint(startCNG, i - 1);
                            listCNG.Add(pt);
                        }

                        countCNG = 0;
                        m_nCountCNG = 0;
                    }
                }

                // 노광편차 설정.
                if (false == isMissPrint)
                {
                    if ("BB019" == Review.ListRaw[i].Value1 || "BB019" == Review.ListRaw[i].Value2 || "BB019" == Review.ListRaw[i].Value3)
                    {
                        isMissPrint = true;
                        startMissPrint = i;
                    }
                }
                else
                {
                    if ("BB019" != Review.ListRaw[i].Value1 && "BB019" != Review.ListRaw[i].Value2 && "BB019" != Review.ListRaw[i].Value3)
                    {
                        isMissPrint = false;

                        IPoint pt = new IPoint(startMissPrint, i - 1);
                        listMissPrint.Add(pt);
                    }
                }

                SetCountNG(Review.ListRaw[i].Value1);
                SetCountNG(Review.ListRaw[i].Value2);
                SetCountNG(Review.ListRaw[i].Value3);
            }
        }

        public void SetCountNG(string value)
        {
            if ("G" == value)
                return;

            for (int i = 0; i < CountNGIDs.Length; ++i)
            {
                if (CountNGIDs[i] == value)
                {
                    CountNGs[i] += 1;
                    break;
                }
            }
        }

        public int Save(string path)
        {
            string dayFolder = DateTime.Now.ToString("yyMMdd");
            string pathName = path + "\\" + dayFolder + "\\" + LotID + ".csv";

            if (false == Directory.Exists(path + "\\" + dayFolder))
                Directory.CreateDirectory(path + "\\" + dayFolder);

            if (true == File.Exists(pathName))
            {
                pathName = path + "\\" + dayFolder + "\\" + LotID + DateTime.Now.ToString("_yyMMdd_HHmmss") + ".csv";
            }

            DataService.Singleton.LocalMapPathName = pathName;

            FileStream stream;
            StreamWriter writer;
            string item;

            ////    모델명 : LC0EC5A12

            ////    Lot No : SYD1H88AA
            ////    PF     :4

            ////    검사 시작시간 : 2015년 12월 19일 23시 26분 26초 

            ////    검사 종료시간 : 2015년 12월 19일 23시 51분 51초 

            ////    가동시간 : 00:25:25

            ////    검사수:16407     OK수:16395   NG수:   12    정상:99.93    불량율: 0.07 

            ////    연속불량구간:4   구간수율정지구간: 1  	    

            ////                 5         10         15         20         25         30         35         40         45         50

            ////      1: X X G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G

            ////     51: X G G G G  G G G G G  G G G G G  G G G G G  X G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G

            ////    101: X G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G  G G G G G

            stream = File.Create(pathName);
            //writer = File.CreateText(path);
            writer = new StreamWriter(stream, Encoding.Default);

            item = "모델명," + RecipeName;
            writer.WriteLine(item);

            item = "Lot No," + LotID;
            writer.WriteLine(item);

            item = "PF," + PF.ToString();
            writer.WriteLine(item);

            item = "검사 시작시간," + string.Format("{0}년 {1}월 {2}일 {3}시 {4}분 {5}초", TimeStart.Year, TimeStart.Month, TimeStart.Day, TimeStart.Hour, TimeStart.Minute, TimeStart.Second);
            writer.WriteLine(item);

            item = "검사 종료시간," + string.Format("{0}년 {1}월 {2}일 {3}시 {4}분 {5}초", TimeEnd.Year, TimeEnd.Month, TimeEnd.Day, TimeEnd.Hour, TimeEnd.Minute, TimeEnd.Second);
            writer.WriteLine(item);

            TimeSpan span = new TimeSpan(0);
            span = TimeEnd - TimeStart;
            item = "가동시간," + string.Format("{0:00}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds);
            writer.WriteLine(item);

            // Line
            item = "LINE," + Line.ToString();
            writer.WriteLine(item);

            writer.WriteLine("");

            // Total
            Total.Save(writer);
            writer.WriteLine("");

            // Top Vision
            writer.WriteLine("TOP");
            Top.Save(writer);
            writer.WriteLine("");

            // Bottom Vision
            writer.WriteLine("BOTTOM");
            Bottom.Save(writer);
            writer.WriteLine("");

            // Mono Vision
            writer.WriteLine("MONO");
            Mono.Save(writer);
            writer.WriteLine("");

            // NG Count
            writer.WriteLine("NGCOUNT");
            for (int i = 0; i < CountNGs.Length; ++i)
            {
                item = string.Format("{0},{1},{2},{3}", i + 1, CountNGIDs[i], CountNGNames[i], CountNGs[i]);
                writer.WriteLine(item);
            }
            writer.WriteLine("");

            writer.Close();
            stream.Close();

            return 0;
        }

        public int Load(string path)
        {
            FileStream stream;
            StreamReader reader;
            string item;

            try
            {
                stream = File.OpenRead(path);
                reader = new StreamReader(stream, Encoding.Default);

                Line = 2;

                //item = "모델명," + RecipeName;
                item = reader.ReadLine();
                RecipeName = item.Replace("모델명,", "");

                //item = "Lot No," + LotID;
                item = reader.ReadLine();
                LotID = item.Replace("Lot No,", "");

                //item = "PF," + PF.ToString();
                item = reader.ReadLine();
                PF = int.Parse(item.Replace("PF,", ""));

                //item = "검사 시작시간," + string.Format("{0}년 {1}월 {2}일 {3}시 {4}분 {5}초", TimeStart.Year, TimeStart.Month, TimeStart.Day, TimeStart.Hour, TimeStart.Minute, TimeStart.Second);
                item = reader.ReadLine();
                item = item.Replace("검사 시작시간,", "");
                TimeStart = Convert.ToDateTime(item);

                //item = "검사 종료시간," + string.Format("{0}년 {1}월 {2}일 {3}시 {4}분 {5}초", TimeEnd.Year, TimeEnd.Month, TimeEnd.Day, TimeEnd.Hour, TimeEnd.Minute, TimeEnd.Second);
                item = reader.ReadLine();
                item = item.Replace("검사 종료시간,", "");
                TimeEnd = Convert.ToDateTime(item);

                //item = "가동시간," + string.Format("{0:00}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds);
                item = reader.ReadLine();

                //writer.WriteLine("");
                item = reader.ReadLine();

                if ("LINE" == item)
                {
                    Line = int.Parse(item.Replace("LINE,", ""));
                    item = reader.ReadLine();
                }

                // Total
                Total.Load(reader);

                //writer.WriteLine("");
                item = reader.ReadLine();

                // Top Vision
                //writer.WriteLine("TOP");
                item = reader.ReadLine();
                if ("TOP" == item)
                {
                    Top.Load(reader);
                }

                //writer.WriteLine("");
                item = reader.ReadLine();

                // BOTTOM Vision
                item = reader.ReadLine();
                if ("BOTTOM" == item)
                {
                    Bottom.Load(reader);
                }

                //writer.WriteLine("");
                item = reader.ReadLine();

                // MONO Vision
                item = reader.ReadLine();
                if ("MONO" == item)
                {
                    Mono.Load(reader);
                }

                item = reader.ReadLine();

                // NG Count
                item = reader.ReadLine();
                if (null != item)
                {
                    if ("NGCOUNT" == item)
                    {
                        for (int i = 0; i < CountNGs.Length; ++i)
                        {
                            item = reader.ReadLine();

                            if (null == item)
                                break;

                            if ("" == item)
                                break;

                            string[] arrays = new string[item.Length];
                            arrays = item.Split(',');

                            if (4 <= arrays.Length)
                                CountNGs[i] = int.Parse(arrays[3]);
                        }
                    }
                }

                reader.Close();
                stream.Close();
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine("DataResult.Load() : " + exc.Message);
                return -1;
            }

            return 0;
        }
    }

    // 검사부 Data 버퍼
    class DataResultTempRaw
    {
        public int Index { get; set; }
        public int Length { get; set; }

        public List<DataResultRaw> ListRaw { get; set; }

        public DataResultTempRaw()
        {
            Index = 0;
            Length = 0;
            ListRaw = new List<DataResultRaw>();
        }

        public void Clear()
        {
            ListRaw.Clear();
        }
    }

    class DataResultTemp
    {
        public List<DataResultTempRaw> ListTemp;

        public DataResultTemp()
        {
            ListTemp = new List<DataResultTempRaw>();
        }

        public void Clear()
        {
            for (int i = 0; i < ListTemp.Count; ++i)
            {
                ListTemp[i].Clear();
            }
            ListTemp.Clear();
        }

        public void Add(int index, int length, string[] messages, string type = "")
        {
            DataResultTempRaw raw = new DataResultTempRaw();
            raw.Index = index;
            raw.Length = length;

            int start = 0;
            string value1;
            string value2;

            for (int i = 0; i < length; ++i)
            {
                start = 5 + i * 2;
                value1 = messages[start];
                value2 = messages[start + 1];

                DataResultRaw data = new DataResultRaw(index + i, value1, value2, type);

                raw.ListRaw.Add(data);
            }

            ListTemp.Add(raw);
        }

        public void Add3(int index, int length, string[] messages, string type = "")
        {
            DataResultTempRaw raw = new DataResultTempRaw();
            raw.Index = index;
            raw.Length = length;

            int start = 0;
            string value1;
            string value2;
            string value3;

            for (int i = 0; i < length; ++i)
            {
                start = 5 + i * 3;
                value1 = messages[start];
                value2 = messages[start + 1];
                value3 = messages[start + 2];

                DataResultRaw data = new DataResultRaw(index + i, value1, value2, value3, type);

                raw.ListRaw.Add(data);
            }

            ListTemp.Add(raw);
        }
    }
}
