using HalconDotNet;
using System;

namespace SmartICAVI
{
    class InspectSmartIC : IInspect
    {
        private DataService dataService = null;
        private GrabService grabService = null;

        protected bool isInitialized = false;

        public bool IsInitialized
        {
            get { return isInitialized; }
        }

        public int Initialize()
        {
            dataService = DataService.Singleton;
            grabService = GrabService.Singleton;

            isInitialized = true;

            return 0;
        }

        public int UnInitialize()
        {
            isInitialized = false;

            return 0;
        }

        public int SetThreshold(HWindow hWindow, HObject hImage, HTuple row1, HTuple col1, HTuple row2, HTuple col2, HTuple min, HTuple max)
        {
            HOperatorSet.DispObj(hImage, hWindow);

            HObject rectangle;
            HObject imageReduced;
            HObject threshold;

            HOperatorSet.GenEmptyObj(out rectangle);
            HOperatorSet.GenEmptyObj(out imageReduced);
            HOperatorSet.GenEmptyObj(out threshold);

            HOperatorSet.GenRectangle1(out rectangle, row1, col1, row2, col2);
            HOperatorSet.ReduceDomain(hImage, rectangle, out imageReduced);
            
            HOperatorSet.SetColor(hWindow, "blue");
            HOperatorSet.SetDraw(hWindow, "margin");

            HOperatorSet.DispRectangle1(hWindow, row1, col1, row2, col2);

            HOperatorSet.Threshold(imageReduced, out threshold, min, max);

            HOperatorSet.SetColor(hWindow, "green");
            HOperatorSet.SetDraw(hWindow, "fill");


            HOperatorSet.DispObj(threshold, hWindow);

            rectangle.Dispose();
            imageReduced.Dispose();
            threshold.Dispose();

            GC.Collect();

            return 0;
        }

        public int SetTeaching(HWindow hWindow, HObject hImage, HTuple row1, HTuple col1, HTuple row2, HTuple col2, HTuple row11, HTuple col11, HTuple row12, HTuple col12, HTuple min, HTuple max)
        {
            //HTuple modelID = 0;
            HTuple number = 0;
            HTuple row = 0, col = 0, angle = 0, score = 0;

            HObject rectangle;
            HObject imageReduced;
            HObject imageReduced2;
            HObject threshold;
            HObject contours;
            //HObject modelContours;
            HObject regionDilation;

            HOperatorSet.GenEmptyObj(out rectangle);
            HOperatorSet.GenEmptyObj(out imageReduced);
            HOperatorSet.GenEmptyObj(out imageReduced2);
            HOperatorSet.GenEmptyObj(out threshold);
            HOperatorSet.GenEmptyObj(out contours);
            //HOperatorSet.GenEmptyObj(out modelContours);
            HOperatorSet.GenEmptyObj(out regionDilation);

            HOperatorSet.SetColor(hWindow, "yellow");
            HOperatorSet.SetDraw(hWindow, "margin");

            HOperatorSet.DispRectangle1(hWindow, row11, col11, row12, col12);

            HOperatorSet.SetColor(hWindow, "blue");
            HOperatorSet.DispRectangle1(hWindow, row1, col1, row2, col2);


            HOperatorSet.GenRectangle1(out rectangle, row1, col1, row2, col2);
            HOperatorSet.ReduceDomain(hImage, rectangle, out imageReduced);
            HOperatorSet.Threshold(imageReduced, out threshold, min, max);

            HOperatorSet.DilationCircle(threshold, out regionDilation, 5);


            HOperatorSet.GenContourRegionXld(threshold, out contours, "border");

            HOperatorSet.CountObj(contours, out number);


            if ((HTuple)(0) < number)
            {
                HOperatorSet.ReduceDomain(hImage, regionDilation, out imageReduced2);

                HOperatorSet.PaintRegion(regionDilation, imageReduced2, out imageReduced2, 0, "fill");
                //HOperatorSet.DispObj(imageReduced2, hWindow);
                HOperatorSet.PaintRegion(threshold, imageReduced2, out imageReduced2, 255, "fill");
                //HOperatorSet.DispObj(imageReduced2, hWindow);

                HOperatorSet.ClearShapeModel(dataService.DataTeach.modelID);
                dataService.DataTeach.modelContours.Dispose();

                HOperatorSet.CreateShapeModel(imageReduced2, 10, 0.0, 3.141592 * 2.0, "auto", "auto", "use_polarity", "auto", "auto", out dataService.DataTeach.modelID);
                HOperatorSet.GetShapeModelContours(out dataService.DataTeach.modelContours, dataService.DataTeach.modelID, 1);

                HOperatorSet.FindShapeModel(imageReduced2, dataService.DataTeach.modelID, -1.570796, 3.141592, 0.5, 1, 0.5, "interpolation", 0, 0.9, out row, out col, out angle, out score);

                if ((HTuple)0 < row.TupleNumber())
                {
                    HTuple homMat2D;
                    HObject contoursAffinTrans;
                    HOperatorSet.GenEmptyObj(out contoursAffinTrans);


                    HOperatorSet.VectorAngleToRigid(0, 0, 0, row, col, angle, out homMat2D);
                    HOperatorSet.AffineTransContourXld(dataService.DataTeach.modelContours, out contoursAffinTrans, homMat2D);

                    HOperatorSet.SetColor(hWindow, "green");

                    HOperatorSet.DispObj(contoursAffinTrans, hWindow);

                    HOperatorSet.DispCross(hWindow, row, col, 100, 0);

                    contoursAffinTrans.Dispose();
                }

                //HOperatorSet.ClearShapeModel(modelID);
            }

            regionDilation.Dispose();
            rectangle.Dispose();
            imageReduced.Dispose();
            imageReduced2.Dispose();
            threshold.Dispose();
            contours.Dispose();
            //modelContours.Dispose();

            GC.Collect();

            return 0;
        }

        public int MakeRecipe(HWindow hWindow, HObject hImage, HTuple row1, HTuple col1, HTuple row2, HTuple col2, HTuple row11, HTuple col11, HTuple row12, HTuple col12, HTuple min, HTuple max)
        {
            if (null == dataService)
                return -1;

            int ret = 0;

            HTuple number = 0;
            HTuple row = 0, col = 0, angle = 0, score = 0;

            HObject rectangle;
            HObject imageReduced;
            HObject imageReduced2;
            HObject threshold;
            HObject contours;
            HObject regionDilation;

            HOperatorSet.GenEmptyObj(out rectangle);
            HOperatorSet.GenEmptyObj(out imageReduced);
            HOperatorSet.GenEmptyObj(out imageReduced2);
            HOperatorSet.GenEmptyObj(out threshold);
            HOperatorSet.GenEmptyObj(out contours);
            HOperatorSet.GenEmptyObj(out regionDilation);


            HOperatorSet.GenRectangle1(out rectangle, row1, col1, row2, col2);
            HOperatorSet.ReduceDomain(hImage, rectangle, out imageReduced);
            HOperatorSet.Threshold(imageReduced, out threshold, min, max);

            HOperatorSet.DilationCircle(threshold, out regionDilation, 2);


            HOperatorSet.GenContourRegionXld(threshold, out contours, "border");

            HOperatorSet.CountObj(contours, out number);


            if ((HTuple)(0) < number)
            {
                HOperatorSet.ReduceDomain(hImage, regionDilation, out imageReduced2);

                HOperatorSet.PaintRegion(regionDilation, imageReduced2, out imageReduced2, 0, "fill");
                HOperatorSet.PaintRegion(threshold, imageReduced2, out imageReduced2, 255, "fill");

                HOperatorSet.ClearShapeModel(dataService.DataRecipe.modelID);
                dataService.DataRecipe.modelContours.Dispose();

                HOperatorSet.CreateShapeModel(imageReduced2, "auto", 0.0, 3.141592 * 2.0, "auto", "auto", "use_polarity", "auto", "auto", out dataService.DataRecipe.modelID);
                HOperatorSet.GetShapeModelContours(out dataService.DataRecipe.modelContours, dataService.DataRecipe.modelID, 1);

                HOperatorSet.FindShapeModel(imageReduced2, dataService.DataRecipe.modelID, -1.570796, 3.141592, 0.5, 1, 0.5, "interpolation", 0, 0.9, out row, out col, out angle, out score);

                if ((HTuple)0 < row.TupleNumber())
                {
                    HTuple homMat2D;
                    HObject contoursAffinTrans;
                    HOperatorSet.GenEmptyObj(out contoursAffinTrans);


                    HOperatorSet.VectorAngleToRigid(0, 0, 0, row, col, angle, out homMat2D);
                    HOperatorSet.AffineTransContourXld(dataService.DataRecipe.modelContours, out contoursAffinTrans, homMat2D);

                    HOperatorSet.SetDraw(hWindow, "margin");
                    HOperatorSet.SetColor(hWindow, "green");

                    HOperatorSet.DispObj(contoursAffinTrans, hWindow);

                    HOperatorSet.DispCross(hWindow, row, col, 100, 0);

                    contoursAffinTrans.Dispose();
                }

                //dataService.DataRecipe.modelID = modelID;
                //dataService.DataRecipe.modelContours = modelContours;

                dataService.DataRecipe.inspectCol1 = col1;
                dataService.DataRecipe.inspectCol2 = col2;
                dataService.DataRecipe.inspectRow1 = row1;
                dataService.DataRecipe.inspectRow2 = row2;

                dataService.DataRecipe.searchRow1 = row11;
                dataService.DataRecipe.searchRow2 = row12;
                dataService.DataRecipe.searchCol1 = col11;
                dataService.DataRecipe.searchCol2 = col12;

                dataService.DataRecipe.row = row;
                dataService.DataRecipe.col = col;

                dataService.DataRecipe.grayMin = min;
                dataService.DataRecipe.grayMax = max;

                dataService.DataRecipe.Save();

                HOperatorSet.WriteImage(hImage, "bmp", 0, dataService.DataRecipe.PathRecipe + "\\Recipe.bmp");
            }
            else
            {
                ret = -1;
            }

            regionDilation.Dispose();
            rectangle.Dispose();
            imageReduced.Dispose();
            imageReduced2.Dispose();
            threshold.Dispose();
            contours.Dispose();
            
            GC.Collect();

            return ret;
        }

        public int SaveTeach(HWindow hWindow, HObject hImage)
        {
            if (null == dataService)
                return -1;

            int ret = 0;

            //HTuple number = 0;
            //HTuple row = 0, col = 0, angle = 0, score = 0;

            //HTuple row1 = dataService.DataRecipe.inspectCol1; 
            //HTuple col1 = dataService.DataRecipe.inspectCol2; 
            //HTuple row2 = dataService.DataRecipe.inspectRow1; 
            //HTuple col2 = dataService.DataRecipe.inspectRow2; 
            
            //HTuple min = dataService.DataRecipe.grayMin;
            //HTuple max = dataService.DataRecipe.grayMax;  

            //HObject rectangle;
            //HObject imageReduced;
            //HObject imageReduced2;
            //HObject threshold;
            //HObject contours;
            //HObject regionDilation;

            //HOperatorSet.GenEmptyObj(out rectangle);
            //HOperatorSet.GenEmptyObj(out imageReduced);
            //HOperatorSet.GenEmptyObj(out imageReduced2);
            //HOperatorSet.GenEmptyObj(out threshold);
            //HOperatorSet.GenEmptyObj(out contours);
            //HOperatorSet.GenEmptyObj(out regionDilation);


            //HOperatorSet.GenRectangle1(out rectangle, row1, col1, row2, col2);
            //HOperatorSet.ReduceDomain(hImage, rectangle, out imageReduced);
            //HOperatorSet.Threshold(imageReduced, out threshold, min, max);

            //HOperatorSet.DilationCircle(threshold, out regionDilation, 5);


            //HOperatorSet.GenContourRegionXld(threshold, out contours, "border");

            //HOperatorSet.CountObj(contours, out number);


            //if ((HTuple)(0) < number)
            //{
            //    HOperatorSet.ReduceDomain(hImage, regionDilation, out imageReduced2);

            //    HOperatorSet.PaintRegion(regionDilation, imageReduced2, out imageReduced2, 0, "fill");
            //    HOperatorSet.PaintRegion(threshold, imageReduced2, out imageReduced2, 255, "fill");

            //    HOperatorSet.ClearShapeModel(dataService.DataRecipe.modelID);
            //    dataService.DataRecipe.modelContours.Dispose();

            //    HOperatorSet.CreateShapeModel(imageReduced2, "auto", 0.0, 3.141592 * 2.0, "auto", "auto", "use_polarity", "auto", "auto", out dataService.DataRecipe.modelID);
            //    HOperatorSet.GetShapeModelContours(out dataService.DataRecipe.modelContours, dataService.DataRecipe.modelID, 1);

            //    HOperatorSet.FindShapeModel(imageReduced2, dataService.DataRecipe.modelID, -1.570796, 3.141592, 0.5, 1, 0.5, "interpolation", 0, 0.9, out row, out col, out angle, out score);

            //    if ((HTuple)0 < row.TupleNumber())
            //    {
            //        HTuple homMat2D;
            //        HObject contoursAffinTrans;
            //        HOperatorSet.GenEmptyObj(out contoursAffinTrans);


            //        HOperatorSet.VectorAngleToRigid(0, 0, 0, row, col, angle, out homMat2D);
            //        HOperatorSet.AffineTransContourXld(dataService.DataRecipe.modelContours, out contoursAffinTrans, homMat2D);

            //        HOperatorSet.SetDraw(hWindow, "margin");
            //        HOperatorSet.SetColor(hWindow, "green");

            //        HOperatorSet.DispObj(contoursAffinTrans, hWindow);

            //        HOperatorSet.DispCross(hWindow, row, col, 100, 0);

            //        contoursAffinTrans.Dispose();
            //    }

            //    dataService.DataRecipe.Save();

            //    HOperatorSet.WriteImage(hImage, "bmp", 0, dataService.DataRecipe.PathRecipe + "\\Recipe.bmp");
            //}
            //else
            //{
            //    ret = -1;
            //}

            //regionDilation.Dispose();
            //rectangle.Dispose();
            //imageReduced.Dispose();
            //imageReduced2.Dispose();
            //threshold.Dispose();
            //contours.Dispose();

            //GC.Collect();

            return ret;
        }

        public int Inspect(HWindow hWindow, HObject hImage, out double offsetX, out double offsetY, string path = "")
        {
            HTuple Row1 = dataService.DataRecipe.searchRow1;
            HTuple Row2 = dataService.DataRecipe.searchRow2;
            HTuple Col1 = dataService.DataRecipe.searchCol1;
            HTuple Col2 = dataService.DataRecipe.searchCol2;

            HTuple grayMin = dataService.DataRecipe.grayMin;
            HTuple grayMax = dataService.DataRecipe.grayMax;

            return Inspect(hWindow, hImage, out offsetX, out offsetY, Row1, Col1, Row2, Col2, grayMin, grayMax);
        }

        public int Inspect(HWindow hWindow, HObject hImage, out double offsetX, out double offsetY, HTuple row1, HTuple col1, HTuple row2, HTuple col2, HTuple min, HTuple max)
        {
            offsetX = offsetY = 0.0;

            if (null == dataService)
                return -1;

            double cx = (double)(grabService.Width) * 0.5;
            double cy = (double)(grabService.Height) * 0.5;

            int ret = 0;

            HTuple row = 0, col = 0, angle = 0, score = 0;

            HObject rectangle;
            HObject imageReduced;
            HObject imageReduced2;
            HObject threshold;

            HOperatorSet.GenEmptyObj(out rectangle);
            HOperatorSet.GenEmptyObj(out imageReduced);
            HOperatorSet.GenEmptyObj(out imageReduced2);
            HOperatorSet.GenEmptyObj(out threshold);

            try
            {
                HOperatorSet.GenRectangle1(out rectangle, row1, col1, row2, col2);
                HOperatorSet.ReduceDomain(hImage, rectangle, out imageReduced);
                HOperatorSet.Threshold(imageReduced, out threshold, min, max);

                //HOperatorSet.DispObj(threshold, hWindow);

                HOperatorSet.PaintRegion(rectangle, imageReduced, out imageReduced, 0, "fill");
                //HOperatorSet.DispObj(imageReduced, hWindow);

                HOperatorSet.PaintRegion(threshold, imageReduced, out imageReduced, 255, "fill");

                //HOperatorSet.DispObj(imageReduced, hWindow);

                //HOperatorSet.DispObj(dataService.DataRecipe.modelContours, hWindow);
                HOperatorSet.SetColor(hWindow, "gray");
                HOperatorSet.SetDraw(hWindow, "margin");
                HOperatorSet.DispRectangle1(hWindow, row1, col1, row2, col2);

                HOperatorSet.FindShapeModel(imageReduced, dataService.DataRecipe.modelID, -1.570796, 3.141592, 0.2, 1, 0.5, "interpolation", 10, 0.9, out row, out col, out angle, out score);

                if ((HTuple)0 < row.TupleNumber())
                {
                    HTuple homMat2D;
                    HObject contoursAffinTrans;
                    HOperatorSet.GenEmptyObj(out contoursAffinTrans);


                    HOperatorSet.VectorAngleToRigid(0, 0, 0, row, col, angle, out homMat2D);
                    HOperatorSet.AffineTransContourXld(dataService.DataRecipe.modelContours, out contoursAffinTrans, homMat2D);

                    HOperatorSet.SetColor(hWindow, "green");


                    HOperatorSet.DispCross(hWindow, row, col, 100, 0);
                    contoursAffinTrans.DispObj(hWindow);
                    //HOperatorSet.DispObj(contoursAffinTrans, hWindow);

                    offsetX = (col.D - cx) * dataService.DataSystem.CalX;
                    offsetY = (row.D - cy) * dataService.DataSystem.CalY;

                    string result = string.Format("Offset X={0:0.000}, Y={1:0.000}", offsetX, offsetY);

                    HOperatorSet.SetFont(hWindow, "-Arial-18-*-*-*-*-1-");
                    HOperatorSet.SetTposition(hWindow, 10, 10);
                    HOperatorSet.WriteString(hWindow, (HTuple)result);

                    contoursAffinTrans.Dispose();
                }
                else
                {
                    offsetX = 0.0;
                    offsetY = 0.0;
                    ret = -1;
                }

                rectangle.Dispose();
                imageReduced.Dispose();
                imageReduced2.Dispose();
                threshold.Dispose();

                GC.Collect();
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine("InspectSmartIC.Inspect() : " + exc.Message);
                rectangle.Dispose();
                imageReduced.Dispose();
                imageReduced2.Dispose();
                threshold.Dispose();

                GC.Collect();
            }

            return ret;
        }

        public int InspectTeach(HWindow hWindow, HObject hImage, out double offsetX, out double offsetY)
        {
            offsetX = offsetY = 0.0;

            if (null == dataService)
                return -1;

            double cx = (double)(grabService.Width) * 0.5;
            double cy = (double)(grabService.Height) * 0.5;

            HTuple row1 = dataService.DataTeach.searchRow1;
            HTuple row2 = dataService.DataTeach.searchRow2;
            HTuple col1 = dataService.DataTeach.searchCol1;
            HTuple col2 = dataService.DataTeach.searchCol2;

            HTuple min = dataService.DataTeach.grayMin;
            HTuple max = dataService.DataTeach.grayMax;

            int ret = 0;

            HTuple row = 0, col = 0, angle = 0, score = 0;

            HObject rectangle;
            HObject imageReduced;
            HObject threshold;

            HOperatorSet.GenEmptyObj(out rectangle);
            HOperatorSet.GenEmptyObj(out imageReduced);
            HOperatorSet.GenEmptyObj(out threshold);


            HOperatorSet.GenRectangle1(out rectangle, row1, col1, row2, col2);
            HOperatorSet.ReduceDomain(hImage, rectangle, out imageReduced);

            HOperatorSet.Threshold(imageReduced, out threshold, min, max);

            HOperatorSet.PaintRegion(rectangle, imageReduced, out imageReduced, 0, "fill");
            HOperatorSet.PaintRegion(threshold, imageReduced, out imageReduced, 255, "fill");

            //HOperatorSet.DispObj(hImage, hWindow);

            //HOperatorSet.DispObj(dataService.DataRecipe.modelContours, hWindow);
            HOperatorSet.SetColor(hWindow, "gray");
            HOperatorSet.SetDraw(hWindow, "margin");
            HOperatorSet.DispRectangle1(hWindow, row1, col1, row2, col2);

            HOperatorSet.FindShapeModel(imageReduced, dataService.DataTeach.modelID, -1.570796, 3.141592, 0.5, 1, 0.5, "interpolation", 0, 0.9, out row, out col, out angle, out score);

            if ((HTuple)0 < row.TupleNumber())
            {
                HTuple homMat2D;
                HObject contoursAffinTrans;
                HOperatorSet.GenEmptyObj(out contoursAffinTrans);


                HOperatorSet.VectorAngleToRigid(0, 0, 0, row, col, angle, out homMat2D);
                HOperatorSet.AffineTransContourXld(dataService.DataTeach.modelContours, out contoursAffinTrans, homMat2D);

                HOperatorSet.SetColor(hWindow, "green");
                HOperatorSet.DispObj(contoursAffinTrans, hWindow);

                HOperatorSet.DispCross(hWindow, row, col, 100, 0);

                offsetX = (col.D - cx) * dataService.DataSystem.CalX;
                offsetY = (row.D - cy) * dataService.DataSystem.CalY;

                string result = string.Format("Offset X={0:0.000}, Y={1:0.000}", offsetX, offsetY);

                HOperatorSet.SetFont(hWindow, "-Arial-18-*-*-*-*-1-");
                HOperatorSet.SetTposition(hWindow, 10, 10);
                HOperatorSet.WriteString(hWindow, (HTuple)result);

                contoursAffinTrans.Dispose();
            }
            else
            {
                offsetX = 0.0;
                offsetY = 0.0;
                ret = -1;
            }

            rectangle.Dispose();
            imageReduced.Dispose();

            GC.Collect();

            return ret;
        }

        public int InspectCircle(HWindow hWindow, HObject hImage, out double offsetX, out double offsetY, HTuple row1, HTuple col1, HTuple row2, HTuple col2, HTuple min, HTuple max, string path = "")
        {
            offsetX = 0.0;
            offsetY = 0.0;

            int ret = 0;

            double cx = (double)(grabService.Width) * 0.5;
            double cy = (double)(grabService.Height) * 0.5;

            HTuple number = 0;
            HTuple row = 0, col = 0, angle = 0, score = 0;

            HTuple circleRow, circleCol, circleR, circleStartPhi, circleEndPhi;
            HTuple circlePointOrder;

            HObject rectangle;
            HObject imageReduced;
            HObject regionThreshold;
            HObject connectedRegions;
            HObject selectedRegions;
            HObject contours;

            HOperatorSet.GenEmptyObj(out rectangle);
            HOperatorSet.GenEmptyObj(out imageReduced);
            HOperatorSet.GenEmptyObj(out regionThreshold);
            HOperatorSet.GenEmptyObj(out connectedRegions);
            HOperatorSet.GenEmptyObj(out selectedRegions);
            HOperatorSet.GenEmptyObj(out contours);


            try
            {
                HOperatorSet.GenRectangle1(out rectangle, row1, col1, row2, col2);
                HOperatorSet.ReduceDomain(hImage, rectangle, out imageReduced);

                HOperatorSet.SetColor(hWindow, "gray");
                HOperatorSet.SetDraw(hWindow, "margin");
                HOperatorSet.DispRectangle1(hWindow, row1, col1, row2, col2);

                HOperatorSet.Threshold(imageReduced, out regionThreshold, min, max);
                HOperatorSet.Connection(regionThreshold, out connectedRegions);
                HOperatorSet.SelectShape(connectedRegions, out selectedRegions, "area", "and", 15000, 45000);
                HOperatorSet.SelectShape(selectedRegions, out selectedRegions, "circularity", "and", 0.65, 1.0);
                HOperatorSet.GenContourRegionXld(selectedRegions, out contours, "border");

                HOperatorSet.CountObj(contours, out number);

                if ((HTuple)(0) < number)
                {
                    HOperatorSet.FitCircleContourXld(contours, "algebraic", -1, 0, 0, 3, 2, out circleRow, out circleCol, out circleR, out circleStartPhi, out circleEndPhi, out circlePointOrder);

                    offsetX = (circleCol.D - cx) * dataService.DataSystem.CalX;
                    offsetY = (circleRow.D - cy) * dataService.DataSystem.CalY;

                    HOperatorSet.SetColor(hWindow, "green");

                    //contours.DispObj(hWindow);
                    HOperatorSet.DispCircle(hWindow, circleRow, circleCol, circleR);
                    HOperatorSet.DispCross(hWindow, circleRow, circleCol, 100, 0);

                    string result = string.Format("Offset X={0:0.000}, Y={1:0.000}", offsetX, offsetY);

                    HOperatorSet.SetFont(hWindow, "-Arial-18-*-*-*-*-1-");
                    HOperatorSet.SetTposition(hWindow, 10, 10);
                    HOperatorSet.WriteString(hWindow, (HTuple)result);
                }

                rectangle.Dispose();
                imageReduced.Dispose();
                regionThreshold.Dispose();
                connectedRegions.Dispose();
                selectedRegions.Dispose();
                contours.Dispose();

                GC.Collect();
            }
            catch (Exception exc)
            {

                Log_Exception.WriteLine("InspectSmartIC.InspectCircle() : " + exc.Message); 

                rectangle.Dispose();
                imageReduced.Dispose();
                regionThreshold.Dispose();
                connectedRegions.Dispose();
                selectedRegions.Dispose();
                contours.Dispose();

                GC.Collect();
            }

            return ret;
        }


        public int SetTeachingA(HalconDotNet.HWindow hWindow, HalconDotNet.HObject hImage)
        {
            HTuple row1 = dataService.DataTeach.inspectRow1A;
            HTuple col1 = dataService.DataTeach.inspectCol1A;
            HTuple row2 = dataService.DataTeach.inspectRow2A;
            HTuple col2 = dataService.DataTeach.inspectCol2A;

            HTuple row11 = dataService.DataTeach.searchRow1A;
            HTuple col11 = dataService.DataTeach.searchCol1A;
            HTuple row12 = dataService.DataTeach.searchRow2A;
            HTuple col12 = dataService.DataTeach.searchCol2A;

            HTuple grayMin = dataService.DataTeach.grayMin;
            HTuple grayMax = dataService.DataTeach.grayMax;

            HTuple contrast = dataService.DataTeach.contrastMinA;


            HTuple number = 0;
            HTuple row = 0, col = 0, angle = 0, score = 0;

            HTuple Row21;
            HTuple Col21;
            HTuple Row22;
            HTuple Col22;

            HObject rectangle;
            HObject imageReduced;
            HObject imageReduced2;
            HObject threshold;
            HObject contours;
            HObject regionDilation;

            HOperatorSet.GenEmptyObj(out rectangle);
            HOperatorSet.GenEmptyObj(out imageReduced);
            HOperatorSet.GenEmptyObj(out imageReduced2);
            HOperatorSet.GenEmptyObj(out threshold);
            HOperatorSet.GenEmptyObj(out contours);
            HOperatorSet.GenEmptyObj(out regionDilation);

            HOperatorSet.SetColor(hWindow, "yellow");
            HOperatorSet.SetDraw(hWindow, "margin");

            HOperatorSet.DispRectangle1(hWindow, row11, col11, row12, col12);

            HOperatorSet.SetColor(hWindow, "blue");
            HOperatorSet.DispRectangle1(hWindow, row1, col1, row2, col2);


            HOperatorSet.GenRectangle1(out rectangle, row1, col1, row2, col2);
            HOperatorSet.ReduceDomain(hImage, rectangle, out imageReduced);
            //HOperatorSet.Threshold(imageReduced, out threshold, grayMin, grayMax);

            //HOperatorSet.GenContourRegionXld(threshold, out contours, "border");
            //HOperatorSet.CountObj(contours, out number);

            //if ((HTuple)(0) < number)
            {
                //HOperatorSet.PaintRegion(rectangle, imageReduced, out imageReduced, 0, "fill");
                //HOperatorSet.PaintRegion(threshold, imageReduced, out imageReduced, 255, "fill");

                if( -1 != dataService.DataTeach.modelIDA )
                    HOperatorSet.ClearShapeModel(dataService.DataTeach.modelIDA);
                dataService.DataTeach.modelContoursA.Dispose();

                HOperatorSet.CreateShapeModel(imageReduced, "auto", 0.0, 3.141592 * 0.5, "auto", "auto", "use_polarity", "auto", contrast, out dataService.DataTeach.modelIDA);
                HOperatorSet.GetShapeModelContours(out dataService.DataTeach.modelContoursA, dataService.DataTeach.modelIDA, 1);

                HOperatorSet.FindShapeModel(imageReduced, dataService.DataTeach.modelIDA, -1.570796, 1.570796, 0.5, 1, 0.5, "interpolation", 0, 0.9, out row, out col, out angle, out score);

                if ((HTuple)0 < row.TupleNumber())
                {
                    HTuple homMat2D;
                    HObject contoursAffinTrans;
                    HOperatorSet.GenEmptyObj(out contoursAffinTrans);


                    HOperatorSet.VectorAngleToRigid(0, 0, 0, row, col, angle, out homMat2D);
                    HOperatorSet.AffineTransContourXld(dataService.DataTeach.modelContoursA, out contoursAffinTrans, homMat2D);

                    HOperatorSet.SetColor(hWindow, "red");

                    HOperatorSet.DispObj(contoursAffinTrans, hWindow);

                    //HOperatorSet.DispCross(hWindow, row, col, 100, 0);

                    HOperatorSet.SmallestRectangle1Xld(contoursAffinTrans, out Row21, out Col21, out Row22, out Col22);
                    HOperatorSet.DispCross(hWindow, Row21[0].D, Col21[0].D, 100, 0);

                    contoursAffinTrans.Dispose();
                }

                //HOperatorSet.ClearShapeModel(modelID);
            }

            regionDilation.Dispose();
            rectangle.Dispose();
            imageReduced.Dispose();
            imageReduced2.Dispose();
            threshold.Dispose();
            contours.Dispose();
            //modelContours.Dispose();

            GC.Collect();

            return 0;
        }

        public int SetTeachingB(HalconDotNet.HWindow hWindow, HalconDotNet.HObject hImage)
        {
            HTuple row1 = dataService.DataTeach.inspectRow1B;
            HTuple col1 = dataService.DataTeach.inspectCol1B;
            HTuple row2 = dataService.DataTeach.inspectRow2B;
            HTuple col2 = dataService.DataTeach.inspectCol2B;

            HTuple row11 = dataService.DataTeach.searchRow1B;
            HTuple col11 = dataService.DataTeach.searchCol1B;
            HTuple row12 = dataService.DataTeach.searchRow2B;
            HTuple col12 = dataService.DataTeach.searchCol2B;

            HTuple grayMin = dataService.DataTeach.grayMin;
            HTuple grayMax = dataService.DataTeach.grayMax;

            HTuple contrast = dataService.DataTeach.contrastMinB;


            HTuple number = 0;
            HTuple row = 0, col = 0, angle = 0, score = 0;

            HTuple Row21;
            HTuple Col21;
            HTuple Row22;
            HTuple Col22;

            HObject rectangle;
            HObject imageReduced;
            HObject imageReduced2;
            HObject threshold;
            HObject contours;
            HObject regionDilation;

            HOperatorSet.GenEmptyObj(out rectangle);
            HOperatorSet.GenEmptyObj(out imageReduced);
            HOperatorSet.GenEmptyObj(out imageReduced2);
            HOperatorSet.GenEmptyObj(out threshold);
            HOperatorSet.GenEmptyObj(out contours);
            HOperatorSet.GenEmptyObj(out regionDilation);

            HOperatorSet.SetColor(hWindow, "yellow");
            HOperatorSet.SetDraw(hWindow, "margin");

            HOperatorSet.DispRectangle1(hWindow, row11, col11, row12, col12);

            HOperatorSet.SetColor(hWindow, "blue");
            HOperatorSet.DispRectangle1(hWindow, row1, col1, row2, col2);


            HOperatorSet.GenRectangle1(out rectangle, row1, col1, row2, col2);
            HOperatorSet.ReduceDomain(hImage, rectangle, out imageReduced);
            //HOperatorSet.Threshold(imageReduced, out threshold, grayMin, grayMax);

            //HOperatorSet.GenContourRegionXld(threshold, out contours, "border");
            //HOperatorSet.CountObj(contours, out number);

            //if ((HTuple)(0) < number)
            {
                //HOperatorSet.PaintRegion(rectangle, imageReduced, out imageReduced, 0, "fill");
                //HOperatorSet.PaintRegion(threshold, imageReduced, out imageReduced, 255, "fill");

                if (-1 != dataService.DataTeach.modelIDA)
                    HOperatorSet.ClearShapeModel(dataService.DataTeach.modelIDA);
                dataService.DataTeach.modelContoursA.Dispose();

                HOperatorSet.CreateShapeModel(imageReduced, "auto", 0.0, 3.141592 * 0.5, "auto", "auto", "use_polarity", "auto", contrast, out dataService.DataTeach.modelIDA);
                HOperatorSet.GetShapeModelContours(out dataService.DataTeach.modelContoursA, dataService.DataTeach.modelIDA, 1);

                HOperatorSet.FindShapeModel(imageReduced, dataService.DataTeach.modelIDA, -1.570796, 1.570796, 0.5, 1, 0.5, "interpolation", 0, 0.9, out row, out col, out angle, out score);

                if ((HTuple)0 < row.TupleNumber())
                {
                    HTuple homMat2D;
                    HObject contoursAffinTrans;
                    HOperatorSet.GenEmptyObj(out contoursAffinTrans);


                    HOperatorSet.VectorAngleToRigid(0, 0, 0, row, col, angle, out homMat2D);
                    HOperatorSet.AffineTransContourXld(dataService.DataTeach.modelContoursA, out contoursAffinTrans, homMat2D);

                    HOperatorSet.SetColor(hWindow, "red");

                    HOperatorSet.DispObj(contoursAffinTrans, hWindow);

                    //HOperatorSet.DispCross(hWindow, row, col, 100, 0);

                    HOperatorSet.SmallestRectangle1Xld(contoursAffinTrans, out Row21, out Col21, out Row22, out Col22);
                    HOperatorSet.DispCross(hWindow, Row22[0].D, Col21[0].D, 100, 0);

                    contoursAffinTrans.Dispose();
                }

                //HOperatorSet.ClearShapeModel(modelID);
            }

            regionDilation.Dispose();
            rectangle.Dispose();
            imageReduced.Dispose();
            imageReduced2.Dispose();
            threshold.Dispose();
            contours.Dispose();
            //modelContours.Dispose();

            GC.Collect();

            return 0;
        }

        public int SaveImage(string path)
        {
            return 0;
        }

    }
}
