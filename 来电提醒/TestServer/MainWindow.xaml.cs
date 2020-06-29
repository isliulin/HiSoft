using Fleck;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WebSocketServer Socket_Server = new WebSocketServer("ws://0.0.0.0:9632");
        public static IDictionary<string, IWebSocketConnection> Socket_Clients = new Dictionary<string, IWebSocketConnection>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Socket_Server.RestartAfterListenError = true;

            Socket_Server.Start(socket =>
            {
                socket.OnOpen = () =>   //连接建立事件
                {
                    string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                    Socket_Clients.Add(clientUrl, socket);

                    socket.Send("Linked");

                    try
                    {
                        Paragraph para = new Paragraph();
                        para.Inlines.Add(new Run(clientUrl));
                        this.Msg.Document.Blocks.Add(para);
                        this.Msg.ScrollToEnd();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(clientUrl);
                        Debug.WriteLine(ex.Message);
                        /*
                        Paragraph para = new Paragraph();
                        para.Inlines.Add(new Run(ex.Message));
                        this.Msg.Document.Blocks.Add(para);
                        this.Msg.ScrollToEnd();
                        */
                    }
                    //MessageBox.Show(clientUrl, "Server Link Open");
                };

                socket.OnClose = () =>  //连接关闭事件
                {
                    string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                    if (Socket_Clients.ContainsKey(clientUrl))
                    {
                        Socket_Clients.Remove(clientUrl);
                    }

                    MessageBox.Show(clientUrl, "Server link Close");
                };

                socket.OnMessage = message =>  //接受客户端网页消息事件
                {
                    string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;

                    try
                    {
                        Paragraph para = new Paragraph();
                        para.Inlines.Add(new Run(message));
                        this.Msg.Document.Blocks.Add(para);
                        this.Msg.ScrollToEnd();
                    }
                    catch(Exception ex)
                    {
                        Paragraph para = new Paragraph();
                        para.Inlines.Add(new Run(ex.Message));
                        this.Msg.Document.Blocks.Add(para);
                        this.Msg.ScrollToEnd();
                    }
                };
            });
        }

        private void SendMsgToClient(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            btn.Content = "Send (" + Socket_Clients.Count.ToString() + ")";
            byte[] m= Encoding.UTF8.GetBytes(DateTime.Now.ToLongTimeString() + " From Server");
            foreach (var s in Socket_Clients)
            {
                s.Value.Send(m);
            }
        }
    }
}
