using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;

namespace 来电提醒客户端
{
    class SetConfig
    {
        public const string ServiceName = "PhoneWatcher";

        public static string SQLConnStr = "";

        public static string ComName(string Number)
        {
            if (string.IsNullOrWhiteSpace(Number))
            {
                return "Null";
            }
            else if (Number == "7")
            {
                return "公司名称";
            }
            else
            {
                SQLConfig sc = new SQLConfig();
                sc.SQL_URL = "192.168.100.254";
                //sc.SQL_Port = "1433";
                sc.SQL_User = "sa";
                sc.SQL_Pwd = "Fjkdashg@3344";

                string r = "^_^";
                using (SqlConnection connection = new SqlConnection(sc.SQL_ConnStr))
                {
                    string sql = string.Format("SELECT top 3 '【' +[field0012] + '】' +[field0002] FROM [HiOA].[dbo].[formmain_0022] WHERE field0008 like '%{0}%' or field0009 LIKE '%{0}%' or field0010 LIKE '%{0}%'  for xml path('')", Number);
                    SqlCommand command = new SqlCommand(sql, connection);
                    connection.Open();
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(String.Format("{0}, {1}", reader[0], reader[1]));
                            r = String.Format("{0}, {1}",reader[0], reader[1]);
                        }
                    }
                }
                return r;
            }
        }
        public static string SaveConfig(string Name, string Value)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser;
                RegistryKey software = key.CreateSubKey("software\\" + ServiceName);
                software = key.OpenSubKey("software\\" + ServiceName, true);
                software.SetValue(Name, Value);
                Debug.WriteLine(Name + " : " + Value);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return GetConfig(Name);
            
        }
        public static string GetConfig(string Name)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser;
                RegistryKey software = key.CreateSubKey("software\\" + ServiceName);
                software = key.OpenSubKey("software\\" + ServiceName, true);
                Debug.WriteLine(Name + " : " + software.GetValue(Name).ToString());
                return software.GetValue(Name).ToString();
            }
            catch(Exception ex)
            {
                return "";
            }
        }
    }
}
