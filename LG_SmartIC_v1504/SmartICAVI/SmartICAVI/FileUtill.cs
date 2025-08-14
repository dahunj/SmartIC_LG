using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace SmartICAVI
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct NetworkResource
    {
        // 범위
        public uint Scope;
        // 타입
        public uint Type;
        // 표시타입
        public uint DisplayType;
        // 용법
        public uint Usage;
        // 로컬명
        public string LocalName;
        // 원격명
        public string RemoteName;
        // 주석
        public string Comment;
        // 제공자
        public string Provider;
    }


    class FileUtill
    {

        [DllImport("mpr.dll", CharSet = CharSet.Auto)]
        public static extern int WNetUseConnection(IntPtr ownerWindowHandle, [MarshalAs(UnmanagedType.Struct)] ref NetworkResource networkResource,
                                                   string password, string userID, uint flag, StringBuilder accessNameStringBuilder, ref int bufferSize, out uint result);

        [DllImport("mpr.dll", CharSet = CharSet.Auto)]
        public static extern int WNetCancelConnection2A(string localName, int flag, int force);

        public static int ConnectNetworkDrive(string networkDrive, string shareFolder, string userID, string password)
        {
            NetworkResource networkResource = new NetworkResource();
            networkResource.Type = 1;
            networkResource.LocalName = networkDrive;
            networkResource.RemoteName = shareFolder;
            networkResource.Provider = null;

            uint flag = 0u;
            int bufferSize = 64;
            StringBuilder stringBuilder = new StringBuilder(bufferSize);
            uint result = 0u;

            return WNetUseConnection(IntPtr.Zero, ref networkResource, password, userID, flag, stringBuilder, ref bufferSize, out result);
        }

        public static void DisconnectNetworkDrive(string networkDrive)
        {
            WNetCancelConnection2A(networkDrive, 1, 0);
        }

        public static int DeleteFiles(string strPath, int period)
        {
            string[] files = Directory.GetFiles(strPath);
            DateTime time = DateTime.Now;

            //foreach (string file in files)
            //{
            //    FileInfo info = new FileInfo(file);
            for (int i = 0; i < files.Length; ++i)
            {
                string file = files[i];
                FileInfo info = new FileInfo(file);
                TimeSpan span = time - info.LastWriteTime;

                if (period < span.Days)
                {
                    //Log_Trace(string.Format("[로그] 이전 로그 삭제 (저장일 {0}) : {1}", SaveDays, file));
                    try
                    {
                        File.Delete(file);

                        Log_Trace.WriteLine("[DeleteFiles] : " + file);
                    }
                    catch (Exception exc)
                    {
                        Log_Exception.WriteLine("FileUtill.DeleteFiles() : " + exc.Message);
                    }
                }
            }

            return 0;
        }

        public static int DeleteAllFiles(string strPath)
        {
            string[] files = Directory.GetFiles(strPath);

            //foreach (string file in files)
            //{
            for (int i = 0; i < files.Length; ++i)
            {
                
                //FileInfo info = new FileInfo(file);

                try
                {
                    string file = files[i];
                    File.Delete(file);
                }
                catch (Exception exc)
                {
                    Log_Exception.WriteLine("FileUtill.DeleteAllFiles() : " + exc.Message);
                }
            }

            return 0;
        }

        public static int MoveFiles(string pathSource, string pathTarget)
        {
            if (!Directory.Exists(pathSource))
            {
                return -1;
            }

            if (!Directory.Exists(pathTarget))
            {
                Directory.CreateDirectory(pathTarget);
            }

            string[] files = Directory.GetFiles(pathSource);
            string rename;

            //foreach (string file in files)
            //{
            for (int i = 0; i < files.Length; ++i)
            {
                                
                try
                {
                    string file = files[i];
                    FileInfo info = new FileInfo(file);
                    rename = pathTarget + "\\" + info.Name;// +info.Extension;
                    File.Move(file, rename);
                }
                catch (Exception exc)
                {
                    Log_Exception.WriteLine("FileUtill.MoveFiles() : " + exc.Message);
                }
            }

            return 0;
        }

        public static int CopyFiles(string pathSource, string pathTarget)
        {
            if (!Directory.Exists(pathSource))
            {
                return -1;
            }

            if (!Directory.Exists(pathTarget))
            {
                Directory.CreateDirectory(pathTarget);
            }

            string[] files = Directory.GetFiles(pathSource);
            string rename;

            //foreach (string file in files)
            //{
            for (int i = 0; i < files.Length; ++i)
            {                                
                try
                {
                    string file = files[i];
                    FileInfo info = new FileInfo(file);
                    rename = pathTarget + "\\" + info.Name + info.Extension;
                    File.Copy(file, rename);
                }
                catch (Exception exc)
                {
                    Log_Exception.WriteLine("FileUtill.CopyFiles() : " + exc.Message);
                }
            }

            return 0;
        }

        // 파일 및 폴더 삭제
        ////"Application.StartupPath(현재 실행된 폼의 경로) + \\삭제하고싶은 원하는 폴더명" 을 dir로 선언
        // DirectoryInfo dir = new DirectoryInfo(Application.StartupPath + "\\삭제하고싶은 원하는 폴더명");
 
        //// ture값을 줘서 폴더내에 파일이 있어도 삭제 ture값을 안주면 폴더내에 파일이 있을경우 삭제가안됨
        //dir.Delete(true);


        //출처: http://pjsprogram.tistory.com/35 [자박꼼]

        // A 라는 하위폴더 모두 삭제
        //Rd 명령어로 해보세요 rd c:/a
        //rd /s/q 입니다.
        //rd : remove directory - 디렉토리 삭제
        ///s : subdirectory - 서브디렉토리까지 삭제
        ///q : question - 묻지않고 삭제하기

    }
}
