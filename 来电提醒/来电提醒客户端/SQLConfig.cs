using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace 来电提醒客户端
{
    [DataContract]
    public class SQLConfig : INotifyPropertyChanged
    {
        private string _SQL_URL = "";
        private string _SQL_Port = "1433";
        private string _SQL_User = "";
        private string _SQL_Pwd = "";
        private string _SQL_Instance = "MSSQLSERVER";
        private string _SQL_DBName = "";
        private string _SQL_Type = "MSSQLSERVER";

        [DataMember]
        public string SQL_URL { get { return _SQL_URL; } set { if (value != _SQL_URL) { _SQL_URL = value; NotifyPropertyChanged(); } } }
        [DataMember]
        public string SQL_Port { get { return _SQL_Port; } set { if (value != _SQL_Port) { _SQL_Port = value; NotifyPropertyChanged(); } } }
        [DataMember]
        public string SQL_User { get { return _SQL_User; } set { if (value != _SQL_User) { _SQL_User = value; NotifyPropertyChanged(); } } }
        [DataMember]
        public string SQL_Pwd { get { return _SQL_Pwd; } set { if (value != _SQL_Pwd) { _SQL_Pwd = value; NotifyPropertyChanged(); } } }
        [DataMember]
        public string SQL_Instance { get { return _SQL_Instance; } set { if (value != _SQL_Instance) { _SQL_Instance = value; NotifyPropertyChanged(); } } }
        [DataMember]
        public string SQL_DBName { get { return _SQL_DBName; } set { if (value != _SQL_DBName) { _SQL_DBName = value; NotifyPropertyChanged(); } } }
        [DataMember]
        public string SQL_Type { get { return _SQL_Type; } set { if (value != _SQL_Type) { _SQL_Type = value; NotifyPropertyChanged(); } } }

        public string SQL_ConnStr
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SQL_DBName))
                {
                    return string.Format("data source={0},{1};user id={2};pwd={3} ", SQL_URL, SQL_Port, SQL_User, SQL_Pwd);
                }
                else
                {
                    return string.Format("data source={0},{1};initial catalog={2};user id={3};pwd={4} ", SQL_URL, SQL_Port, SQL_DBName, SQL_User, SQL_Pwd);
                }
            }
        }

        public void LoadConfig(SQLConfig s)
        {
            SQL_URL = s.SQL_URL;
            SQL_Port = s.SQL_Port;
            SQL_User = s.SQL_User;
            SQL_Pwd = s.SQL_Pwd;
            SQL_Instance = s.SQL_Instance;
            SQL_DBName = s.SQL_DBName;
            SQL_Type = s.SQL_Type;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            //Debug.WriteLine(propertyName + "  Changed");
        }

        public JObject ToJson()
        {
            return JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(this));
        }

        public static SQLConfig FromJson(JObject JWC)
        {
            return JsonConvert.DeserializeObject<SQLConfig>(JsonConvert.SerializeObject(JWC));
        }
    }
}
