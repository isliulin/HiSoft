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


namespace 来电提醒服务端
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ServerWindow : Window
    {

        public static List<Phone> PhoneStateList = new List<Phone>();

        public ServerWindow()
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
            //基础参数
            {
                if (string.IsNullOrWhiteSpace(SetConfig.GetConfig(ShowMoreInfo.Name)))
                {
                    ShowMoreInfo.IsChecked = false;
                }
                else
                {
                    Boolean sb = bool.Parse(SetConfig.GetConfig(ShowMoreInfo.Name));
                    ShowMoreInfo.IsChecked = sb;
                }
                ShowMoreInfo.Checked += ShowMoreChange;
                ShowMoreInfo.Unchecked += ShowMoreChange;
            }
            //InitDevice
            try
            {
                IntPtr ip = new WindowInteropHelper(this).Handle;
                if (Device.InitDevice(ip.ToInt32()) == 0)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "InitDevice");
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
            }
            return hwnd;
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
                        PhoneStateList[nChannel].ComName = szCallerID.ToString();
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
                        PhoneStateList[nChannel].ComName = szDialDigit.ToString();
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
    }
}
