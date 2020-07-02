using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using Panuon.UI.Silver;

namespace 来电提醒服务端
{
    class SetConfig
    {
        public const string ServiceName = "PhoneWatcher";

        public static string SQLConnStr = "";

        public static string SaveConfig(string Name, string Value)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser;
                RegistryKey software = key.CreateSubKey("software\\" + ServiceName);
                software = key.OpenSubKey("software\\" + ServiceName, true);
                software.SetValue(Name, Value);
                NoticeX.Show(Name + " = " + Value, "保存系统参数", MessageBoxIcon.Success, 1000*3);
                //Debug.WriteLine(Name + " : " + Value);
            }
            catch(Exception ex)
            {
                NoticeX.Show(Name + " = " + Value+"\n"+ex.Message, "保存系统参数失败",MessageBoxIcon.Error,1000*10);
                //Debug.WriteLine(ex.Message);
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
                
                foreach (string na in software.GetValueNames())
                {
                    if (Name == na)
                    {
                        //Debug.WriteLine(Name + " : " + software.GetValue(Name).ToString());
                        return software.GetValue(Name).ToString();
                    }
                    //Debug.Write("|" + na);
                }
                NoticeX.Show("参数名："+Name+"\n 未配置", "读取系统参数失败", MessageBoxIcon.Error, 1000*2);
                Debug.WriteLine(Name + " : Not Set");
                return "";
            }
            catch(Exception ex)
            {
                NoticeX.Show("参数名：" + Name + "\n" + ex.Message, "读取系统参数失败", MessageBoxIcon.Error, 1000*10);
                return "";
            }
        }
    }
}
