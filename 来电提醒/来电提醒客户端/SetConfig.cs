using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace 来电提醒客户端
{
    class SetConfig
    {
        public const string ServiceName = "PhoneWatcher";
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
