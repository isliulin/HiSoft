using Fleck;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

public struct AD130DEVICEPARAMETER
{
    public int nRingOn;
    public int nRingOff;
    public int nHookOn;
    public int nHookOff;
    public int nStopCID;
    public int nNoLine;
}

public struct TAGSTATE
{
    public bool bChannelState;
    public bool bRecord;
    public bool bDevicePlay;
    public bool bLinePlay;
    public bool bMonitor;
    public bool bLineBusy;
    public bool bDisableRinging;
    public bool bDisconnectPhone;
    public bool bAutodial;
    public bool bHookOff;
    public int iDeviceType;
};


namespace 来电提醒客户端
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static List<Phone> PhoneStateList = new List<Phone>();

        IDictionary<string, IWebSocketConnection> Socket_Clients = new Dictionary<string, IWebSocketConnection>();
        private ClientWebSocket Socket_Client = new ClientWebSocket();
        private System.Threading.CancellationToken ClientCT = new System.Threading.CancellationToken();

        private BackgroundWorker ListeningMsg = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            this.PhoneStateListView.DataContext = PhoneStateList;
            for (int i = 0; i < Device.MAX_DEVICE; i++)
            {
                PhoneStateList.Add(new Phone(i));
            }
        }
        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            ListeningMsg.WorkerReportsProgress = true;
            ListeningMsg.WorkerSupportsCancellation = true;
            ListeningMsg.ProgressChanged += SyncStatsChange_ReportProgress;
            ListeningMsg.DoWork += GetServiceMsg;

            Boolean sb = bool.Parse(SetConfig.GetConfig(ShowMoreInfo.Name));
            ShowMoreInfo.IsChecked = sb;
            Boolean cb = bool.Parse(SetConfig.GetConfig(isClient.Name));
            isClient.IsChecked = cb;
            ShowMoreInfo.Checked += ShowMoreChange;
            ShowMoreInfo.Unchecked += ShowMoreChange;
            isClient.Checked += isClientChange;
            isClient.Unchecked += isClientChange;

            if (!(bool)isClient.IsChecked)
            {
                try
                {
                    SIPG.Visibility = Visibility.Collapsed;
                    LinkState.Visibility = Visibility.Hidden;
                }
                catch
                { }
                
                try
                {
                    WebSocketServer Socket_Server = new WebSocketServer("ws://0.0.0.0:9632");

                    Socket_Server.RestartAfterListenError = true;
                    Socket_Server.Start(socket =>
                    {
                        socket.OnOpen = () =>   //连接建立事件
                        {
                            string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                            Socket_Clients.Add(clientUrl, socket);
                            CS_Msg Wel_Msg = new CS_Msg();
                            Wel_Msg.MsgType = Msg_Type.Welcome.ToString();
                            socket.Send(JsonConvert.SerializeObject(Wel_Msg));
                        };
                        socket.OnClose = () =>  //连接关闭事件
                        {
                            string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                            if (Socket_Clients.ContainsKey(clientUrl))
                            {
                                Socket_Clients.Remove(clientUrl);
                            }
                        };
                        socket.OnMessage = message =>  //接受客户端网页消息事件
                        {
                            string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                            //Debug.WriteLine(DateTime.Now.ToString() + "|服务器:【收到】来客户端网页:" + clientUrl + "的信息：\n" + message);
                            //socket.Send("Receive " + DateTime.Now.ToShortTimeString() + message);
                        };
                    });

                    IntPtr ip = new WindowInteropHelper(this).Handle;
                    if (Device.InitDevice(ip.ToInt32()) == 0)
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Start Server Error");
                }
            }
            else
            {
                try
                {
                    SIPG.Visibility = Visibility.Visible;
                    LinkState.Visibility = Visibility.Visible;
                    Socket_Client.ConnectAsync(new Uri("ws://127.0.0.1:9632"), ClientCT);
                    

                    
                    ListeningMsg.RunWorkerAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Start Client Error");
                }
            }
        }
       

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;

            if (hwndSource != null)
            {
                IntPtr handle = hwndSource.Handle;
                hwndSource.AddHook(new HwndSourceHook(WndProc));
            }
        }

        //用于接收消息并执行自定义行为的消息函数示例代码如下：
        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == Device.WM_AD130MSG)
            {
                OnDeviceMsg(wParam,lParam);
                CS_Msg DevMsg = new CS_Msg();
                DevMsg.MsgType = Msg_Type.DevMsg.ToString();
                DevMsg.wParam = wParam.ToInt32();
                DevMsg.Lparam = lParam.ToInt32();
                string msgStr = JsonConvert.SerializeObject(DevMsg);
                foreach (var Socket in Socket_Clients)
                {
                    Socket.Value.Send(msgStr);
                }
            }
            return hwnd;
        }

        private void GetServiceMsg(object _sender, DoWorkEventArgs e)
        {
            BackgroundWorker BG = (BackgroundWorker)_sender;
            while (Socket_Client.State == WebSocketState.Connecting)
            {
                BG.ReportProgress(1, "正在连接服务器");
                Thread.Sleep(200);
            }
            BG.ReportProgress(0, (Socket_Client.State == WebSocketState.Open));

            if (Socket_Client.State == WebSocketState.Open)
            {
                BG.ReportProgress(1, "已连接");
            }
            else if (Socket_Client.State == WebSocketState.Closed)
            {
                BG.ReportProgress(1, "未能连接到服务器");
            }
            else
            {
                BG.ReportProgress(1, Socket_Client.State.ToString());
            }
            while (!BG.CancellationPending && Socket_Client.State== WebSocketState.Open)
            {
                var result = new byte[1024];
                Socket_Client.ReceiveAsync(new ArraySegment<byte>(result), ClientCT);

                List<byte> lastbyte = new List<byte>();
                foreach (var b in result)
                {
                    if (b != 0x00)
                    {
                        lastbyte.Add(b);
                    }
                }
                byte[] rbs = new byte[lastbyte.Count];
                for (int i = 0; i < lastbyte.Count; i++)
                {
                    rbs[i] = lastbyte[i];
                }

                var str = Encoding.UTF8.GetString(rbs, 0, rbs.Length);
                BG.ReportProgress(2, str);
                Thread.Sleep(500);
            }
        }
        private void SyncStatsChange_ReportProgress(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
            {
                LinkState.IsChecked = (bool)e.UserState;
                ConnectBTN.IsEnabled = true;
            }
            else if(e.ProgressPercentage==1)
            {
                DiaMsg.Content = (string)e.UserState;
            }
            else if (e.ProgressPercentage == 2)
            {
                string MSG_Str = (string)e.UserState;
                Debug.WriteLine("MSG_Str:  " + MSG_Str);
                JObject mj = JsonConvert.DeserializeObject<JObject>(MSG_Str);
                CS_Msg msg = JsonConvert.DeserializeObject<CS_Msg>(MSG_Str);
                Msg_Type mt = Msg_Type.unknow;
                try
                {
                    mt = (Msg_Type)Enum.Parse(typeof(Msg_Type), msg.MsgType);
                }
                catch
                {
                    mt = (Msg_Type)Enum.Parse(typeof(Msg_Type), mj["MsgType"].ToString());
                }
                
                
                switch (mt)
                {
                    case Msg_Type.Welcome:
                        {
                            try
                            {
                                this.ConnectBTN.Visibility = Visibility.Collapsed;
                            }
                            catch
                            { }
                            break;
                        }
                    case Msg_Type.channelSet:
                        {
                            if (msg.channelConfig.Count == 4)
                            {
                                for (int i = 0; i < msg.channelConfig.Count; i++)
                                {
                                    PhoneStateList[i].ChannelName = msg.channelConfig[i].ToString();
                                }
                            }
                            break;
                        }
                    case Msg_Type.DevMsg:
                        {
                            OnDeviceMsg((IntPtr)msg.wParam, (IntPtr)msg.Lparam);
                            break;
                        }
                }
            }
        }
        private void OnDeviceMsg(IntPtr wParam, IntPtr Lparam)
        {
            int nMsg = new int();
            int nChannel = new int();

            nMsg = wParam.ToInt32() % 65536;
            nChannel = wParam.ToInt32() / 65536;

            switch (nMsg)
            {
                case Device.MCU_BACKDISABLE: //0xFF,  返回通道状态,通道不可用 
                    {
                        PhoneStateList[nChannel].DeviceInfo = "————";
                        PhoneStateList[nChannel].DeviceCode = "";

                        PhoneStateList[nChannel].bChannelState = false;
                        PhoneStateList[nChannel].bRecord = false;
                        PhoneStateList[nChannel].bDevicePlay = false;
                        PhoneStateList[nChannel].bLinePlay = false;
                        PhoneStateList[nChannel].bMonitor = false;
                        PhoneStateList[nChannel].bLineBusy = false;
                        PhoneStateList[nChannel].bDisableRinging = false;
                        PhoneStateList[nChannel].bDisconnectPhone = false;
                        PhoneStateList[nChannel].bAutodial = false;
                        PhoneStateList[nChannel].iDeviceType = 0;
                    }
                    break;
                case Device.MCU_BACKENABLE:  // 0xEE,  返回通道状态,通道可用 
                    {
                        switch (Lparam.ToInt32())
                        {
                            case Device.DEVICE_AD130:
                                PhoneStateList[nChannel].DeviceInfo = "AD130";
                                break;
                            case Device.DEVICE_AD230:
                                PhoneStateList[nChannel].DeviceInfo = "AD230";
                                break;
                            case Device.DEVICE_AD430:
                                PhoneStateList[nChannel].DeviceInfo = "AD430";
                                break;
                            default:
                                PhoneStateList[nChannel].DeviceInfo = "未知设备";
                                break;
                        }
                        PhoneStateList[nChannel].LineState = "";
                        PhoneStateList[nChannel].CallerID = "";
                        PhoneStateList[nChannel].DialedNum = "";
                        PhoneStateList[nChannel].TalkTime = "";
                        PhoneStateList[nChannel].bChannelState = true;
                        PhoneStateList[nChannel].iDeviceType = Lparam.ToInt32();

                        break;
                    }
                case Device.MCU_BACKSTATE: // 0x08,  返回通道外线状态,提挂机,拨号,铃声状态等 
                    {
                        switch (Lparam.ToInt32())
                        {
                            case Device.HOOKON_POSITIVEPOLARITY: // 0x01,  挂机状态,并且外线极性为正 
                                {
                                    PhoneStateList[nChannel].LineState = "挂机+";

                                    PhoneStateList[nChannel].bRecord = false;
                                    PhoneStateList[nChannel].bLinePlay = false;
                                    PhoneStateList[nChannel].bMonitor = false;
                                    PhoneStateList[nChannel].bAutodial = false;
                                    PhoneStateList[nChannel].bHookOff = false;
                                    break;
                                }
                            case Device.HOOKON_NEGATIVEPOLARITY:// 0x02, 挂机状态,并且外线极性为负 
                                {
                                    PhoneStateList[nChannel].LineState = "挂机-";

                                    PhoneStateList[nChannel].bRecord = false;
                                    PhoneStateList[nChannel].bLinePlay = false;
                                    PhoneStateList[nChannel].bMonitor = false;
                                    PhoneStateList[nChannel].bAutodial = false;
                                    PhoneStateList[nChannel].bHookOff = false;
                                    break;
                                }
                            case Device.HAVE_POLARITY: // 0x03, 此状态没用到 
                                {
                                    PhoneStateList[nChannel].LineState = "HOOK ON & NOPOLARITY";
                                    break;
                                }
                            case Device.HOOKOFF_POSITIVEPOLARITY: //提机状态,并且外线极性为正 
                                {
                                    PhoneStateList[nChannel].LineState = "提机+";
                                    if (PhoneStateList[nChannel].bHookOff)
                                    {
                                        break;
                                    }

                                    StringBuilder szCallerID = new StringBuilder(128);

                                    if (Device.GetCallerID(nChannel, szCallerID) < 1 || Device.GetRingCount(nChannel) < 1)
                                    {
                                        PhoneStateList[nChannel].CallerID = "";
                                    }
                                    PhoneStateList[nChannel].DialedNum = "";
                                    PhoneStateList[nChannel].TalkTime = "";

                                    PhoneStateList[nChannel].bHookOff = true;
                                    PhoneStateList[nChannel].bDevicePlay = false;

                                    break;
                                }
                            case Device.HOOKOFF_NEGATIVEPOLARITY: //提机状态,并且外线极性为负 
                                {
                                    PhoneStateList[nChannel].LineState = "提机-";
                                    if (PhoneStateList[nChannel].bHookOff)
                                    {
                                        break;
                                    }
                                    StringBuilder szCallerID = new StringBuilder(128);
                                    if (Device.GetCallerID(nChannel, szCallerID) < 1 || Device.GetRingCount(nChannel) < 1)
                                    {
                                        PhoneStateList[nChannel].CallerID = "";
                                    };
                                    PhoneStateList[nChannel].DialedNum = "";
                                    PhoneStateList[nChannel].TalkTime = "";
                                    PhoneStateList[nChannel].bHookOff = true;
                                    PhoneStateList[nChannel].bDevicePlay = false;

                                    break;
                                }
                            case Device.NOLINE_NOPOLARITY: //0x06,  没有外线 
                                {
                                    PhoneStateList[nChannel].LineState = "No Line";
                                    PhoneStateList[nChannel].CallerID = "";
                                    PhoneStateList[nChannel].DialedNum = "";
                                    PhoneStateList[nChannel].TalkTime = "";
                                    PhoneStateList[nChannel].bHookOff = false;

                                    break;
                                }
                            case Device.RINGON: // 0x07, 铃声ON状态 
                                {
                                    int iRing = Device.GetRingCount(nChannel);
                                    string szRing = "响铃:" + string.Format("{0:D2}", iRing);
                                    PhoneStateList[nChannel].LineState = szRing;
                                    PhoneStateList[nChannel].TalkTime = "";
                                    PhoneStateList[nChannel].DialedNum = "";
                                    break;
                                }
                            case Device.RINGOFF: // 0x08, 铃声OFF状态 
                                {
                                    PhoneStateList[nChannel].LineState = "Ring Off";
                                    break;
                                }
                            case Device.NOHOOK_POSITIVEPOLARITY: //0x09,  挂机状态,并且外线极性为正 
                                {
                                    PhoneStateList[nChannel].LineState = "挂机+";
                                    PhoneStateList[nChannel].bHookOff = false;
                                    break;
                                }
                            case Device.NOHOOK_NEGATIVEPOLARITY: // 0x0A, 挂机状态,并且外线极性为负 
                                {
                                    PhoneStateList[nChannel].LineState = "挂机-";
                                    PhoneStateList[nChannel].bHookOff = false;
                                    break;
                                }
                            case Device.NOHOOK_NOPOLARITY: // 0x0B, 没有外线 
                                {
                                    PhoneStateList[nChannel].LineState = "没插电话线";
                                    PhoneStateList[nChannel].bHookOff = false;
                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
                case Device.MCU_BACKCID: //0x09,  返回来电号码 
                    {
                        StringBuilder szCallerID = new StringBuilder(128);

                        int nLen = Device.GetCallerID(nChannel, szCallerID);
                        PhoneStateList[nChannel].CallerID = szCallerID.ToString();
                        break;
                    }
                case Device.MCU_DEVICECODE: // 0X14,  返回设备码 
                    {
                        PhoneStateList[nChannel].DeviceCode = string.Format("{0:D}", Lparam.ToInt32());
                        break;
                    }
                case Device.MCU_BACKDIGIT: // 0x0A,  返回电话拨的号码 
                    {
                        StringBuilder szDialDigit = new StringBuilder(128);
                        Device.GetDialDigit(nChannel, szDialDigit);
                        PhoneStateList[nChannel].DialedNum = szDialDigit.ToString();
                        break;
                    }
                case Device.MCU_BACKCPUVER://0x0D,  返回设备版本 
                    {
                        StringBuilder szCPUID = new StringBuilder(4);
                        Device.GetCPUVersion(nChannel, szCPUID);
                        break;
                    }
                case Device.MCU_BACKPARAM: //0x0C,   返回铃声/提挂机设定参数
                    {
                        AD130DEVICEPARAMETER tagParameter = new AD130DEVICEPARAMETER();

                        Device.GetParameter(nChannel, ref tagParameter);
                        /*
                        IDC_EDIT_RINGON.Text = tagParameter.nRingOn.ToString();
                        IDC_EDIT_RINGOFF.Text = tagParameter.nRingOff.ToString();
                        IDC_EDIT_HOOKON.Text = tagParameter.nHookOn.ToString();
                        IDC_EDIT_HOOKOFF.Text = tagParameter.nHookOff.ToString();
                        IDC_EDIT_CALLERID.Text = tagParameter.nStopCID.ToString();
                        IDC_EDIT_NOLINE.Text = tagParameter.nNoLine.ToString();
                        */
                    }
                    break;
                case Device.MCU_BUSYTONE:// 0x0F,   返回忙音状态,有忙音或没有忙音 
                    {
                        break;
                    }
                case Device.MCU_BACKMISSED: // 0xAA,  返回未接来电,表示有未接来电 
                    {
                        PhoneStateList[nChannel].MissedCall = "漏接电话 "+PhoneStateList[nChannel].CallerID;
                        break;
                    }
                case Device.MCU_BACKTALK: // 0xBB,  返回通话计时 
                    {
                        // return talk time
                        string strTalk;
                        strTalk = string.Format("{0:D2}", Lparam.ToInt32() / 3600) + ":" + string.Format("{0:D2}", (Lparam.ToInt32() % 3600) / 60) + ":" + string.Format("{0:D2}", Lparam.ToInt32() % 60);
                        PhoneStateList[nChannel].TalkTime = strTalk;
                        break;
                    }
                case Device.MCU_PLAYOVER: // 0xCC,     返回播放结束，当调用播放函数播放声音时,播放结 束后会收此状态 
                    {
                        // return play sound over
                        if (Lparam.ToInt32() == Device.DEVICE_PLAY)
                        {
                            // Stop playing file
                            if (Device.StopPlayFile(nChannel, Device.DEVICE_PLAY) != 0)
                            {
                                PhoneStateList[nChannel].bDevicePlay = false;
                                /*
                                m_State[nChannel].bDevicePlay = false;
                                IDC_BUTTON_DEVICE_PLAY.Text = "Device - Start Playing";
                                */
                                Device.EnablePlayFile(nChannel, Device.DEVICE_PLAY, 0);
                            }
                        }
                        else if (Lparam.ToInt32() == Device.LINE_PLAY)
                        {

                            if (Device.StopPlayFile(nChannel, Device.LINE_PLAY) != 0)
                            {
                                PhoneStateList[nChannel].bLinePlay = false;
                                //m_State[nChannel].bLinePlay = false;
                                //IDC_BUTTON_LINE_PLAY.Text = "Line - Start Playing";
                                Device.EnablePlayFile(nChannel, Device.LINE_PLAY, 0);
                            }
                        }
                        break;
                    }
                default:
                    break;
            }
            PhoneStateListView.Items.Refresh();
        }


        private void IDC_BUTTON_RECORD_Click(object sender, EventArgs e)
        {
            int nChannel = -1;
            if (PhoneStateListView.SelectedItems.Count > 0)
            {
                nChannel = PhoneStateListView.SelectedIndex;
            }

            if (nChannel != -1)
            {
                if (PhoneStateList[nChannel].bRecord)
                {
                    if (Device.StopRecordFile(nChannel) != 0)
                    {
                        PhoneStateList[nChannel].bRecord = false;
                        //IDC_BUTTON_RECORD.Text = "Start Recording";
                    }
                }
                else
                {
                    string strFileName;
                    strFileName = "C22625" + string.Format("{0:D2}", nChannel + 1) + "Record.wav";

                    if (Device.StartRecordFile(nChannel, strFileName.ToCharArray()) != 0)
                    {
                        PhoneStateList[nChannel].bRecord = true;
                        //IDC_BUTTON_RECORD.Text = "Stop Recording";
                    }
                    else
                    {
                        MessageBox.Show("Please pick up your phone first.\n If you want to start recording your phone must be hook off.");
                    }
                }
            }
        }

        private void isClientChange(object sender, RoutedEventArgs e)
        {
            CheckBox isClientCB = (CheckBox)sender;
            string r = SetConfig.SaveConfig(isClientCB.Name, isClientCB.IsChecked.ToString());
            if (string.IsNullOrWhiteSpace(r))
            {
                isClientCB.IsChecked = false;
            }
            else
            {
                Boolean b = bool.Parse(r);
                isClientCB.IsChecked = b;
            }

            if ((bool)isClientCB.IsChecked)
            {
                try
                {
                    LinkState.Visibility = Visibility.Visible;
                    SIPG.Visibility = Visibility.Visible;
                }
                catch
                { }
            }
            else
            {
                try
                {
                    LinkState.Visibility = Visibility.Hidden;
                    SIPG.Visibility = Visibility.Collapsed;
                }
                catch
                { }
            }
        }

        private void CloseDo(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Device.FreeDevice();
        }

        private void ShowMoreChange(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            string r = SetConfig.SaveConfig(cb.Name, cb.IsChecked.ToString());
            if (string.IsNullOrWhiteSpace(r))
            {
                cb.IsChecked = false;
            }
            else
            {
                Boolean b = bool.Parse(r);
                cb.IsChecked = b;
            }
        }

        private void ReConnection(object sender, RoutedEventArgs e)
        {
            ConnectBTN.IsEnabled = false;
            while (ListeningMsg.IsBusy)
            {
                ListeningMsg.CancelAsync();
            }
            while (Socket_Client.State == WebSocketState.Connecting)
            {
                DiaMsg.Content = "is Connection,Can't Stop";
                Thread.Sleep(100);
            }
            if (Socket_Client.State == WebSocketState.Open)
            {
                Socket_Client.CloseAsync( WebSocketCloseStatus.NormalClosure,"",ClientCT);
            }
            Socket_Client.Abort();
            Socket_Client = new ClientWebSocket();
            Socket_Client.ConnectAsync(new Uri("ws://127.0.0.1:9632"), ClientCT);
            ListeningMsg.RunWorkerAsync();
        }
    }
}
