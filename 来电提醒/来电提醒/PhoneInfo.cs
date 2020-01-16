using Fleck;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhoneInfo
{
    public partial class PhoneWatcherService : ServiceBase
    {
        public static IDictionary<string, IWebSocketConnection> Client_Sockets = new Dictionary<string, IWebSocketConnection>();
        public static List<Phone> PhoneStateList = new List<Phone>();

        [DllImport("AD130Device.dll", EntryPoint = "AD130_InitDevice")]
        public static extern int AD130_InitDevice(int hWnd);

        public static string ServName;
        public PhoneWatcherService()
        {
            InitializeComponent();
            ServName = ServiceName;

            if (!EventLog.SourceExists(ServiceName))
            {
                EventLog.CreateEventSource(ServiceName, "Application");
            }
        }

        //System.Windows.Forms.Form s = new System.Windows.Forms.Form();


        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry(ServiceName, ServiceName+" On Start", EventLogEntryType.Information);
            PhoneStateList.Clear();
            for (int i = 0; i < DeviceVal.MAX_DEVICE; i++)
            {
                PhoneStateList.Add(new Phone(i));
            }
            System.Windows.Forms.Form SF = new ServiceForm();
            try
            {
                
                SF.Show();
                EventLog.WriteEntry(ServiceName, ServiceName + " Form Show\n"+SF.Handle.ToString(), EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ServiceName, "ServiceForm Error\n"+ex.Message, EventLogEntryType.Error);
            }
            

            

            //IntPtr ip = new WindowInteropHelper(this).Handle;
            

            try
            {
                //创建

                WebSocketServer server = new WebSocketServer("ws://0.0.0.0:9632");//监听所有的的地址
                                                                                  
                server.RestartAfterListenError = true; //出错后进行重启

                //开始监听
                server.Start(socket =>
                {
                    socket.OnOpen = () =>   //连接建立事件
                    {
                        //获取客户端网页的url
                        string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                        Client_Sockets.Add(clientUrl, socket);
                        socket.Send("Connectioned @" + DateTime.Now.ToLongTimeString());
                        //Debug.WriteLine(DateTime.Now.ToString() + "|服务器:和客户端网页:" + clientUrl + " 建立WebSock连接！");
                    };
                    socket.OnClose = () =>  //连接关闭事件
                    {
                        string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                        //如果存在这个客户端,那么对这个socket进行移除
                        if (Client_Sockets.ContainsKey(clientUrl))
                        {
                            //注:Fleck中有释放
                            //关闭对象连接 
                            //if (dic_Sockets[clientUrl] != null)
                            //{
                            //dic_Sockets[clientUrl].Close();
                            //}
                            Client_Sockets.Remove(clientUrl);
                        }
                        Debug.WriteLine(DateTime.Now.ToString() + "|服务器:和客户端网页:" + clientUrl + " 断开WebSock连接！");
                    };
                    socket.OnMessage = message =>  //接受客户端网页消息事件
                    {
                        string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                        Debug.WriteLine(DateTime.Now.ToString() + "|服务器:【收到】来客户端网页:" + clientUrl + "的信息：\n" + message);
                        socket.Send("Receive " + DateTime.Now.ToShortTimeString() + message);

                        if (message == "aaa")
                        {
                            foreach (var sse in Client_Sockets)
                            {
                                sse.Value.Send(sse.Key + " Say:" + message);
                            }
                        }
                    };
                });
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ServiceName, "Socket Error \n" + ex.Message,EventLogEntryType.Error);
                //Debug.WriteLine("Open Error" + ex.Message);
            }
            try
            {
                
                if (AD130_InitDevice(this.ServiceHandle.ToInt32()) == 0)
                {
                    EventLog.WriteEntry(ServiceName, "AD130 Initialed", EventLogEntryType.Information);
                    return;
                }
                else
                {
                    EventLog.WriteEntry(ServiceName, "AD130 Can't Initialed", EventLogEntryType.Information);
                }
                
            }
            catch(Exception ex)
            {
                EventLog.WriteEntry(ServiceName, "AD130 Error \n" + ex.Message, EventLogEntryType.Error);
            }
            EventLog.WriteEntry(ServiceName, ServiceName + " On Started", EventLogEntryType.Information);

            //this.ServiceHandle.ToPointer();
        }

        //protected override mes

        protected override void OnCustomCommand(int command)
        {
            //base.OnCustomCommand(command);
            EventLog.WriteEntry(ServiceName, "OnCustomCommand \n" + command.ToString(), EventLogEntryType.Error);
        }

        protected override void OnStop()
        {
        }
    }
}
