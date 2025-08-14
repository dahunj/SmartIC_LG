using System;
using System.Collections.Generic;

namespace SmartICAVI
{
    class DataUserAccount
    {
        public int Type { get; set; } // 0 : User, 1 : Admin, 2 : Maint
        public string ID { get; set; }
        public string Password { get; set; }
        public string AccountType
        {
            get
            {
                switch (Type)
                {
                    case 1:
                        return "관리자";
                    case 2:
                        return "유지보수";
                    default:
                        return "오퍼레이터";

                }
            }
        }

        public DataUserAccount()
        {
            Type = 0;
            ID = "USER";
            Password = "1";
        }

        public DataUserAccount(int type, string id, string password)
        {
            Type = type;
            ID = id;
            Password = password;
        }
    }

    class DataUserAccounts
    {
        private List<DataUserAccount> listUA = new List<DataUserAccount>();

        public List<DataUserAccount> ListUA { get { return listUA; } }

        public string Path { get; set; }

        public int AddAccount(int type, string id, string password)
        {
            int count = listUA.Count;

            for (int i = 0; i < count; ++i)
            {
                if (id == listUA[i].ID)
                    return -1;
            }

            DataUserAccount data = new DataUserAccount(type, id, password);
            listUA.Add(data);

            return 0;
        }

        public int DeleteAccount(string id)
        {
            int count = listUA.Count;

            for (int i = 0; i < count; ++i)
            {
                if (id == listUA[i].ID)
                {
                    listUA.RemoveAt(i);
                    GC.Collect();
                    return 0;
                }
            }
            return 0;
        }

        public bool IsExistID(string id)
        {
            int count = listUA.Count;

            for (int i = 0; i < count; ++i)
            {
                if (id == listUA[i].ID)
                {
                    return true;
                }
            }
            return false;
        }

        public int GetAccountType(string id, string password)
        {
            int count = listUA.Count;

            for (int i = 0; i < count; ++i)
            {
                if (id == listUA[i].ID)
                {
                    if (password == listUA[i].Password)
                        return listUA[i].Type;
                    else
                        return -1;
                }
            }
            return -1;
        }

        public void Clear()
        {
            listUA.Clear();
        }

        public int Load(string path)
        {
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
            Clear();

            IniFile ini = new IniFile();

            string section = "MEMBERS";
            string key = "";

            int count = int.Parse(ini.Read(section, "Count", "0", Path));
            int type = 0;
            string id = "";
            string password = "";

            section = "USER_ACCOUNT";

            for (int i = 0; i < count; ++i)
            {
                key = "Type_" + i.ToString("000");
                type = int.Parse(ini.Read(section, key, "0", Path));

                key = "ID_" + i.ToString("000");
                id = ini.Read(section, key, key, Path);

                key = "Password_" + i.ToString("000");
                password = ini.Read(section, key, "0", Path);

                DataUserAccount data = new DataUserAccount(type, id, password);
                listUA.Add(data);
            }

            return 0;
        }

        public int Save()
        {
            IniFile ini = new IniFile();

            string section = "MEMBERS";
            string key = "";

            int count = listUA.Count;

            ini.Write(section, "Count", count.ToString(), Path);

            section = "USER_ACCOUNT";

            for (int i = 0; i < count; ++i)
            {
                key = "Type_" + i.ToString("000");
                ini.Write(section, key, listUA[i].Type.ToString(), Path);

                key = "ID_" + i.ToString("000");
                ini.Write(section, key, listUA[i].ID, Path);

                key = "Password_" + i.ToString("000");
                ini.Write(section, key, listUA[i].Password, Path);
            }

            return 0;
        }
    }
}
