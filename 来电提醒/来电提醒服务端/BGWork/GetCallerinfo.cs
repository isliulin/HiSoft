using Microsoft.Data.SqlClient;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Channels;

namespace 来电提醒服务端.BGWork
{
    public class GetCallerinfo
    {
        BackgroundWorker GetCallerInfoWork = new BackgroundWorker();

        private int Channel;
        private string PhoneNo;

        public GetCallerinfo(int _channel,string _PhoneNo)
        {
            Channel = _channel;
            PhoneNo = _PhoneNo;
            
            GetCallerInfoWork.DoWork += WorkFunction;
            GetCallerInfoWork.RunWorkerCompleted += WorkComplete;
            
        }

        public void Running()
        {
            GetCallerInfoWork.RunWorkerAsync();
        }

        void WorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            CallNotice CN = new CallNotice(Channel, PhoneNo);
            CN.Running();
            /*
            string NoteMsg = "来电号码：" + ServerWindow.PhoneStateList[Channel].PhoneNumber;
            NoteMsg += "\n单位信息：" + ServerWindow.PhoneStateList[Channel].ComName;
            NoteMsg += "\n来电时间：" + ServerWindow.PhoneStateList[Channel].timeS.ToString();
            switch (SetConfig.GetConfig("NoticeType"))
            {
                case "0":
                    //程序弹窗
                    NoticeX.Show(NoteMsg, ServerWindow.PhoneStateList[Channel].ChannelName + "来电", 1000 * 5);
                    break;
                default:
                    //默认使用程序系统通知
                    NoticeX.Show(NoteMsg, ServerWindow.PhoneStateList[Channel].ChannelName + "来电", 1000 * 5);
                    break;
            }
            */
        }

        void WorkFunction(object sender, DoWorkEventArgs e)
        {
            ServerWindow.PhoneStateList[Channel].ComName = "  ";
            BackgroundWorker work = sender as BackgroundWorker;
            if (string.IsNullOrWhiteSpace(PhoneNo))
            {
                ServerWindow.PhoneStateList[Channel].ComName = "  ";
            }
            else if (PhoneNo == "13065436319")
            {
                ServerWindow.PhoneStateList[Channel].ComName= "海汇软件 杨";
            }
            else
            {
                ServerWindow.PhoneStateList[Channel].ComName = "正在搜索……";
                SQLConfig sc = new SQLConfig();
                sc.SQL_URL = "192.168.100.254";
                //sc.SQL_Port = "1433";
                sc.SQL_User = "sa";
                sc.SQL_Pwd = "Fjkdashg@3344";

                string r = "^_^";
                using (SqlConnection connection = new SqlConnection(sc.SQL_ConnStr))
                {
                    string sql = string.Format("SELECT top 3 '【' +[field0012] + '】' +[field0002] FROM [HiOA].[dbo].[formmain_0022] WHERE field0008 like '%{0}%' or field0009 LIKE '%{0}%' or field0010 LIKE '%{0}%'  for xml path('')", PhoneNo);
                    SqlCommand command = new SqlCommand(sql, connection);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(String.Format("{0}, {1}", reader[0], reader[1]));
                            r = String.Format("{0}, {1}", reader[0], reader[1]);
                        }
                    }
                }
                ServerWindow.PhoneStateList[Channel].ComName =r;
            }
        }
    }
}
