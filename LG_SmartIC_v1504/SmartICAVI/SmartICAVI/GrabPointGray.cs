using FlyCapture2Managed;
using HalconDotNet;
using System;
using System.Threading;

namespace SmartICAVI
{
    class GrabPointGray : IGrab
    {
        private ManagedGigECamera cam = null;
        private ManagedImage rawImage = null;
        private ManagedImage convertedImage = null;
        private ManagedBusManager busMgr = null;
        private ManagedPGRGuid guid = null;

        private int camWidth = 0;
        private int camHeight = 0;
        #region Interfaces
        public bool IsConnected
        {
            get 
            {
                if (null != cam)
                    return cam.IsConnected();

                return false;
            }
        }

        public int Width { get { return camWidth; } }
        public int Height { get { return camHeight; } }

        public int Initialize()
        {
            Connect();
            
            return 0;
        }

        public int UnInitialize()
        {
            Disconnect();

            return 0;
        }

        public int Connect()
        {
            try
            {
                GigEImageSettingsInfo imageSettingsInfo = null;
                GigEImageSettings imageSettings = null;
                EmbeddedImageInfo embeddedInfo = null;

                if (null == busMgr)
                    busMgr = new ManagedBusManager();

                CameraInfo[] camInfos = ManagedBusManager.DiscoverGigECameras();

                uint numCameras = busMgr.GetNumOfCameras();

                for (uint i = 0; i < numCameras; i++)
                {
                    guid = busMgr.GetCameraFromIndex(i);

                    if (busMgr.GetInterfaceTypeFromGuid(guid) != InterfaceType.GigE)
                    {
                        continue;
                    }

                    cam = new ManagedGigECamera();

                    cam.Connect(guid);

                    imageSettingsInfo = cam.GetGigEImageSettingsInfo();

                    imageSettings = new GigEImageSettings();
                    imageSettings.offsetX = 0;
                    imageSettings.offsetY = 0;
                    imageSettings.height = imageSettingsInfo.maxHeight;
                    imageSettings.width = imageSettingsInfo.maxWidth;
                    imageSettings.pixelFormat = FlyCapture2Managed.PixelFormat.PixelFormatMono8;

                    camWidth = (int)imageSettingsInfo.maxWidth;
                    camHeight = (int)imageSettingsInfo.maxHeight;

                    cam.SetGigEImageSettings(imageSettings);

                    // Get embedded image info from camera
                    embeddedInfo = cam.GetEmbeddedImageInfo();

                    // Enable timestamp collection	
                    if (embeddedInfo.timestamp.available == true)
                    {
                        embeddedInfo.timestamp.onOff = true;
                    }

                    // Set embedded image info to camera
                    cam.SetEmbeddedImageInfo(embeddedInfo);

                    rawImage = new ManagedImage();
                    convertedImage = new ManagedImage();

                    break;
                }
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine("GrabPointGray.Connect() : " + exc.Message);
            }

            return 0;
        }

        public int Disconnect()
        {
            if (null != cam)
            {
                try
                {
                    if (true == cam.IsConnected())
                        cam.Disconnect();
                }
                catch (Exception exc)
                {
                    Log_Exception.WriteLine("GrabPointGray.Disconnect() : " + exc.Message);
                }
            }

            return 0;
        }

        public int Grab(out HalconDotNet.HObject HImage)
        {
            HOperatorSet.GenEmptyObj(out HImage);

            
            if (null == cam)
                return -1;
            if (false == cam.IsConnected())
                return -1;

            try
            {
                //Disconnect();
                //Connect();

                cam.StartCapture();

                // Retrieve an image
                cam.RetrieveBuffer(rawImage);

                unsafe
                {
                    HOperatorSet.GenImage1(out HImage, "byte", Width, Height, (IntPtr)rawImage.data);
                }

                cam.StopCapture();

                //Disconnect();
                GC.Collect();
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine("GrabPointGray.Grab() : " + exc.Message);
                
                GC.Collect();
                return -1;
            }

            return 0;
        }

        public int Reset()
        {
            uint numCameras = 0;
            int count = 0;

            if (null == cam)
                return -1;
            try
            {
                do
                {
                    busMgr.RescanBus();
                    CameraInfo[] camInfos = ManagedBusManager.DiscoverGigECameras();

                    Thread.Sleep(100);

                    numCameras = busMgr.GetNumOfCameras();
                    

                    if (0 < numCameras)
                    {
                        break;
                    }

                    if (++count > 3)
                        return -1;

                } while (2 != numCameras);

                return Connect();
            }
            catch (Exception exc)
            {
                Log_Exception.WriteLine("GrabPointGray.Reset() : " + exc.Message);
            }

            return -1;
        }

        public int Save(string path)
        {
            return 0;
        }
        #endregion
    }
}
