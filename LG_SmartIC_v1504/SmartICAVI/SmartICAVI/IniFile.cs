using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace SmartICAVI
{
    class IniFile
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public void Write(string section, string key, string val, string filePath)
        {
            WritePrivateProfileString(section, key, val, filePath);
        }

        public string Read(string section, string key, string def, string filePath)
        {
            StringBuilder temp = new StringBuilder(256);
            int ret = GetPrivateProfileString(section, key, def, temp, 255, filePath);
            return temp.ToString();
        }

        public static bool IsSameFile(string path, string name)
        {
            try
            {
                string[] files = Directory.GetFiles(path);

                //foreach (string file in files)
                //{
                for (int i = 0; i < files.Length; ++i)
                {
                    string file = files[i];
                    FileInfo info = new FileInfo(file);
                   
                    if (info.Name == name)
                        return true;
                }
            }
            catch (Exception exc)
            {
                //Log.Write("Exception : IniFile.IsSameFile() => " + exc.Message);
                string str = exc.Message;
            }
            return false;
        }
    }
}
