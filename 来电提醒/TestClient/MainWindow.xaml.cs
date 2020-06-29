using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
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

namespace TestClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker ServerSocketWork = new BackgroundWorker();
        public MainWindow()
        {
            InitializeComponent();
            ServerSocketWork.WorkerSupportsCancellation = true;
            ServerSocketWork.WorkerReportsProgress = true;
            ServerSocketWork.ProgressChanged += SyncStatsChange_ReportProgress;
            ServerSocketWork.DoWork += GetServiceMsg;
        }

        private void ClientOnLoaded(object sender, RoutedEventArgs e)
        {
            ServerSocketWork.RunWorkerAsync();
        }

        private void GetServiceMsg(object _sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgWork = (BackgroundWorker)_sender;

            ClientWebSocket Socket_Client = new ClientWebSocket();
            System.Threading.CancellationToken ClientCT = new System.Threading.CancellationToken();
            Socket_Client.ConnectAsync(new Uri("ws://127.0.0.1:9632"), ClientCT);
            while (true)
            {
                //bgWork.ReportProgress(1, Socket_Client.State.ToString());

                try
                {
                    var result = new byte[1024];

                    //Socket_Client.SendAsync(result,WebSocketMessageType.Text,true,)

                    Socket_Client.ReceiveAsync(new ArraySegment<byte>(result), ClientCT);
                    
                    List<byte> lastbyte = new List<byte>();
                    foreach (var b in result)
                    {
                        if (b != 0x00)
                        {
                            //Debug.WriteLine(b.ToString());
                            lastbyte.Add(b);
                        }
                    }
                    //bgWork.ReportProgress(2, "result L:" + lastbyte.Count.ToString());
                    byte[] rbs = new byte[lastbyte.Count];
                    for (int i = 0; i < lastbyte.Count; i++)
                    {
                        rbs[i] = lastbyte[i];
                    }

                    string str = Encoding.UTF8.GetString(rbs, 0, rbs.Length);
                    //bgWork.ReportProgress(2, "result str:" + str);
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        bgWork.ReportProgress(2, str);
                    }
                    Thread.Sleep(500);
                }
                catch (Exception ex)
                {
                    bgWork.ReportProgress(2, ex.StackTrace+" "+ex.Message);
                    Thread.Sleep(1000);
                }
                
                
            }
        }
        string state = "";
        private void SyncStatsChange_ReportProgress(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 2)
            {
                string m = (string)e.UserState;
                if (!string.IsNullOrWhiteSpace(m))
                {
                    Paragraph para = new Paragraph();
                    para.Inlines.Add(new Run(m));
                    this.Msg.Document.Blocks.Add(para);
                    this.Msg.ScrollToEnd();
                }
            }
            else if (e.ProgressPercentage == 1)
            {
                string m = (string)e.UserState;
                if (!string.IsNullOrWhiteSpace(m))
                {
                    if (state != m)
                    {
                        Paragraph para = new Paragraph();
                        para.Inlines.Add(new Run(m));
                        this.Msg.Document.Blocks.Add(para);
                        this.Msg.ScrollToEnd();
                        state = m;
                    }
                }
            }
        }
    }
}
