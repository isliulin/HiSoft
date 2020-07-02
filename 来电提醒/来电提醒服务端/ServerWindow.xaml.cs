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
using Panuon.UI.Silver;

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
                Phone nPhone = new Phone(i);
                nPhone.SavePhoneRecord.ProgressChanged += PhoneRecoreingStepRep;
                string pn = SetConfig.GetConfig("LanName" + (i + 1).ToString());
                if (!string.IsNullOrWhiteSpace(pn))
                {
                    nPhone.ChannelName = pn;
                }
                PhoneStateList.Add(nPhone);
            }
        }

        //录音程序提示
        void PhoneRecoreingStepRep(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case 0:

                    mMsg0.Inlines.Clear();
                    mMsg0.Inlines.Add("\r\n"+e.UserState.ToString());
                    break;
                case 1:
                    mMsg1.Text = "";
                    //mMsg1.Inlines.Clear();
                    //mMsg1.Inlines.Add("\r\n" + e.UserState.ToString());
                    mMsg1.Text = e.UserState.ToString();
                    break;
                case 2:
                    mMsg2.Inlines.Clear();
                    mMsg2.Inlines.Add("\r\n" + e.UserState.ToString());
                    break;
                case 3:
                    mMsg3.Inlines.Clear();
                    mMsg0.Inlines.Add("\r\n" + e.UserState.ToString());
                    break;
                default:
                    if (e.ProgressPercentage >= 100)
                    {
                        NoticeX.Show(e.UserState.ToString(), "录音完成，准备上传", MessageBoxIcon.Success);
                    }
                    break;
            }
        }

        void SavePhoneRecoreingFile(object sender, DoWorkEventArgs e)
        {
            
        }
        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            //获取设置参数
            foreach (object SC in ConfGrid.Children)
            {
                if (SC.GetType() == typeof(System.Windows.Controls.TextBox))
                {
                    TextBox sctb = SC as TextBox;
                    Debug.WriteLine(SC.GetType().ToString() + "  |" + sctb.Name);
                    sctb.Text=SetConfig.GetConfig(sctb.Name);
                }
                if (SC.GetType() == typeof(Slider))
                {
                    Slider scs = SC as Slider;
                    Debug.WriteLine(SC.GetType().ToString() + "  |" + scs.Name);

                    try
                    {
                        int sv = int.Parse(SetConfig.GetConfig(scs.Name));
                        scs.Value = sv;
                    }
                    catch
                    {
                        scs.Value = 0;
                    }
                    //初始化提示文本
                    if (scs.Name == AutoRecRingCount.Name)
                    {
                        AutoRecRingCountShow.Content = scs.Value;
                    }
                }
                if (SC.GetType() == typeof(ComboBox))
                {
                    ComboBox sccb = SC as ComboBox;
                    Debug.WriteLine(SC.GetType().ToString() + "  |" + sccb.Name);
                    string sccbcs = SetConfig.GetConfig(sccb.Name);
                    if (!string.IsNullOrWhiteSpace(sccbcs))
                    {
                        sccb.SelectedIndex = int.Parse(sccbcs);
                    }  
                }
            }
            
            //初始化硬件设备 ad130录音盒
            //InitDevice
            try
            {
                IntPtr WH = new WindowInteropHelper(this).Handle;
                if (Device.InitDevice(WH.ToInt32()) == 0)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "InitDevice");
            }
        }
       

        //WIndows窗口消息钩子
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

        //设备消息处理
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
                        PhoneStateList[nChannel].CallType = "";
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

                        Debug.WriteLine(nChannel.ToString() + " " + "MCU_BACKDISABLE");
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
                        //PhoneStateList[nChannel].CallerID = "";
                        //PhoneStateList[nChannel].DialedNum = "";
                        PhoneStateList[nChannel].PhoneNumber = "";
                        PhoneStateList[nChannel].TalkTime = "";
                        PhoneStateList[nChannel].bChannelState = true;
                        PhoneStateList[nChannel].iDeviceType = Lparam.ToInt32();
                        Debug.WriteLine(nChannel.ToString() + " " + "MCU_BACKENABLE");
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
                                    Debug.WriteLine(nChannel.ToString() + " " + "MCU_BACKSTATE"+" HOOKON_POSITIVEPOLARITY");
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
                                    Debug.WriteLine(nChannel.ToString() + " " + "MCU_BACKSTATE"+ " HOOKON_NEGATIVEPOLARITY");
                                    break;
                                }
                            case Device.HAVE_POLARITY: // 0x03, 此状态没用到 
                                {
                                    PhoneStateList[nChannel].LineState = "HOOK ON & NOPOLARITY";
                                    Debug.WriteLine(nChannel.ToString() + " " + "MCU_BACKSTATE" + " HAVE_POLARITY");
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
                                        //PhoneStateList[nChannel].CallerID = "";
                                        PhoneStateList[nChannel].PhoneNumber = "";
                                    }
                                    //PhoneStateList[nChannel].DialedNum = "";
                                    PhoneStateList[nChannel].TalkTime = "";

                                    PhoneStateList[nChannel].bHookOff = true;
                                    PhoneStateList[nChannel].bDevicePlay = false;

                                    PhoneStateList[nChannel].timeS = DateTime.Now;
                                    PhoneStateList[nChannel].timeS_UTC = DateTime.UtcNow.Ticks;

                                    Debug.WriteLine(nChannel.ToString() + " " + "MCU_BACKSTATE" + " HOOKOFF_POSITIVEPOLARITY");
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
                                        //PhoneStateList[nChannel].CallerID = "";
                                        PhoneStateList[nChannel].PhoneNumber = "";
                                    };
                                    //PhoneStateList[nChannel].DialedNum = "";
                                    PhoneStateList[nChannel].TalkTime = "";
                                    PhoneStateList[nChannel].bHookOff = true;
                                    PhoneStateList[nChannel].bDevicePlay = false;

                                    PhoneStateList[nChannel].timeS = DateTime.Now;
                                    PhoneStateList[nChannel].timeS_UTC = DateTime.UtcNow.Ticks;

                                    Debug.WriteLine(nChannel.ToString() + " " + "MCU_BACKSTATE" + " HOOKOFF_NEGATIVEPOLARITY");
                                    break;
                                }
                            case Device.NOLINE_NOPOLARITY: //0x06,  没有外线 
                                {
                                    PhoneStateList[nChannel].LineState = "No Line";
                                    //PhoneStateList[nChannel].CallerID = "";
                                    //PhoneStateList[nChannel].DialedNum = "";
                                    PhoneStateList[nChannel].PhoneNumber = "";
                                    PhoneStateList[nChannel].TalkTime = "";
                                    PhoneStateList[nChannel].bHookOff = false;

                                    Debug.WriteLine(nChannel.ToString() + " " + "MCU_BACKSTATE" + " NOLINE_NOPOLARITY");
                                    break;
                                }
                            case Device.RINGON: // 0x07, 铃声ON状态 
                                {
                                    int iRing = Device.GetRingCount(nChannel);
                                    if (iRing == 1)
                                    {
                                        PhoneStateList[nChannel].timeS = DateTime.Now;
                                        PhoneStateList[nChannel].timeS_UTC = DateTime.UtcNow.Ticks;
                                    }
                                    string szRing = "响铃:" + string.Format("{0:D2}", iRing);
                                    PhoneStateList[nChannel].LineState = szRing;
                                    PhoneStateList[nChannel].TalkTime = "";
                                    //PhoneStateList[nChannel].DialedNum = "";
                                    PhoneStateList[nChannel].CallType = Device.CallType_In;
                                    int AutoRecRingCount = 0;
                                    try
                                    {
                                        string AutoRecRingCountStr = SetConfig.GetConfig("AutoRecRingCount");
                                        if (!string.IsNullOrWhiteSpace(AutoRecRingCountStr))
                                        {
                                            AutoRecRingCount = int.Parse(AutoRecRingCountStr);
                                        }
                                    }
                                    catch
                                    {
                                        AutoRecRingCount = 0;
                                    }
                                    if (iRing == AutoRecRingCount)
                                    {
                                        if (Device.PickupPhone(nChannel) == 1)
                                        {
                                            PhoneStateList[nChannel].bAutodial = true;
                                            PhoneStateList[nChannel].CallType = Device.CallType_AutoRec;

                                            if (Device.StartDetectBusytone(nChannel) != 0)
                                            {
                                                Debug.WriteLine(nChannel.ToString() + " " + "StartDetectBusytone");
                                            }
                                        }
                                        //AD130_PickupPhone
                                    }
                                    Debug.WriteLine(nChannel.ToString() + " " + "MCU_BACKSTATE" + " RINGON");
                                    break;
                                }
                            case Device.RINGOFF: // 0x08, 铃声OFF状态 
                                {
                                    //PhoneStateList[nChannel].LineState = "Ring Off";
                                    Debug.WriteLine(nChannel.ToString() + " " + "MCU_BACKSTATE" + " RINGOFF");
                                    break;
                                }
                            case Device.NOHOOK_POSITIVEPOLARITY: //0x09,  挂机状态,并且外线极性为正 
                                {
                                    PhoneStateList[nChannel].LineState = "挂机+";
                                    PhoneStateList[nChannel].bHookOff = false;

                                    Debug.WriteLine(nChannel.ToString() + " " + "MCU_BACKSTATE" + " NOHOOK_POSITIVEPOLARITY");
                                    break;
                                }
                            case Device.NOHOOK_NEGATIVEPOLARITY: // 0x0A, 挂机状态,并且外线极性为负 
                                {
                                    PhoneStateList[nChannel].LineState = "挂机-";
                                    PhoneStateList[nChannel].bHookOff = false;

                                    Debug.WriteLine(nChannel.ToString() + " " + "MCU_BACKSTATE" + " NOHOOK_NEGATIVEPOLARITY");
                                    break;
                                }
                            case Device.NOHOOK_NOPOLARITY: // 0x0B, 没有外线 
                                {
                                    PhoneStateList[nChannel].LineState = "没插电话线";
                                    PhoneStateList[nChannel].bHookOff = false;

                                    Debug.WriteLine(nChannel.ToString() + " " + "MCU_BACKSTATE" + " NOHOOK_NOPOLARITY");
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
                        string pn = "";
                        foreach (char s in szCallerID.ToString())
                        {
                            if (s == '0' || s == '1' || s == '2' || s == '3' || s == '4' || s == '5' || s == '6' || s == '7' || s == '8' || s == '9')
                            {
                                pn += s;
                            }
                        }
                        //PhoneStateList[nChannel].CallerID = szCallerID.ToString();
                        //PhoneStateList[nChannel].CallerID = pn;
                        PhoneStateList[nChannel].PhoneNumber = pn;
                        PhoneStateList[nChannel].CallType = Device.CallType_In;
                        //PhoneStateList[nChannel].ComName = szCallerID.ToString();

                        BGWork.GetCallerinfo get = new BGWork.GetCallerinfo(nChannel,pn);
                        get.Running();

                        //Panuon.UI.Silver.NoticeX.Show("电话号码" + pn + "", PhoneStateList[nChannel].ChannelName+"有来电",1000*5);


                        Debug.WriteLine(nChannel.ToString() + " " + "MCU_BACKCID" + " ");
                        break;
                    }
                case Device.MCU_DEVICECODE: // 0X14,  返回设备码 
                    {
                        PhoneStateList[nChannel].DeviceCode = string.Format("{0:D}", Lparam.ToInt32());

                        Debug.WriteLine(nChannel.ToString() + " " + "MCU_DEVICECODE" + " ");
                        break;
                    }
                case Device.MCU_BACKDIGIT: // 0x0A,  返回电话拨的号码 
                    {
                        StringBuilder szDialDigit = new StringBuilder(128);
                        Device.GetDialDigit(nChannel, szDialDigit);

                        PhoneStateList[nChannel].timeS = DateTime.Now;
                        PhoneStateList[nChannel].timeS_UTC = DateTime.UtcNow.Ticks;

                        string pn = "";
                        foreach (char s in szDialDigit.ToString())
                        {
                            if (s == '0' || s == '1' || s == '2' || s == '3' || s == '4' || s == '5' || s == '6' || s == '7' || s == '8' || s == '9')
                            {
                                pn += s;
                            }
                        }

                        //PhoneStateList[nChannel].DialedNum = szDialDigit.ToString();
                        //PhoneStateList[nChannel].DialedNum = pn;
                        PhoneStateList[nChannel].PhoneNumber = pn;
                        PhoneStateList[nChannel].CallType = Device.CallType_Out;
                        BGWork.GetCallerinfo get = new BGWork.GetCallerinfo(nChannel, pn);
                        get.Running();
                        //PhoneStateList[nChannel].ComName = szDialDigit.ToString();

                        Debug.WriteLine(nChannel.ToString() + " " + "MCU_BACKDIGIT" + " ");
                        break;
                    }
                case Device.MCU_BACKCPUVER://0x0D,  返回设备版本 
                    {
                        StringBuilder szCPUID = new StringBuilder(4);
                        Device.GetCPUVersion(nChannel, szCPUID);

                        Debug.WriteLine(nChannel.ToString() + " " + "MCU_BACKCPUVER" + " ");
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

                        Debug.WriteLine(nChannel.ToString() + " " + "MCU_BACKPARAM" + " ");
                    }
                    break;
                case Device.MCU_BUSYTONE:// 0x0F,   返回忙音状态,有忙音或没有忙音 
                    {
                        if (PhoneStateList[nChannel].bAutodial)
                        {
                            Device.HangupPhone(nChannel);
                            Device.StopDetectBusytone(nChannel);
                        }

                        Debug.WriteLine(nChannel.ToString() + " " + "MCU_BUSYTONE" + " ");
                        break;
                    }
                case Device.MCU_BACKMISSED: // 0xAA,  返回未接来电,表示有未接来电 
                    {
                        
                        //PhoneStateList[nChannel].MissedCall = "漏接电话 "+PhoneStateList[nChannel].CallerID;
                        PhoneStateList[nChannel].MissedCall = "漏接电话： " + PhoneStateList[nChannel].PhoneNumber;
                        BGWork.CallNotice CN = new BGWork.CallNotice(nChannel, PhoneStateList[nChannel].PhoneNumber, true);
                        CN.Running();
                        //NoticeX.Show("漏接电话\n电话号码:" + PhoneStateList[nChannel].PhoneNumber + "", "漏接电话",1000*60);
                        Debug.WriteLine(nChannel.ToString() + " " + "MCU_BACKMISSED" + " ");
                        break;
                    }
                case Device.MCU_BACKTALK: // 0xBB,  返回通话计时 
                    {
                        // return talk time
                        string strTalk;
                        strTalk = string.Format("{0:D2}", Lparam.ToInt32() / 3600) + ":" + string.Format("{0:D2}", (Lparam.ToInt32() % 3600) / 60) + ":" + string.Format("{0:D2}", Lparam.ToInt32() % 60);
                        PhoneStateList[nChannel].TalkTime = strTalk;

                        PhoneStateList[nChannel].timeE = DateTime.Now;
                        PhoneStateList[nChannel].timeE_UTC = DateTime.UtcNow.Ticks;
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
                                Device.EnablePlayFile(nChannel, Device.DEVICE_PLAY, 0);
                            }
                        }
                        else if (Lparam.ToInt32() == Device.LINE_PLAY)
                        {

                            if (Device.StopPlayFile(nChannel, Device.LINE_PLAY) != 0)
                            {
                                PhoneStateList[nChannel].bLinePlay = false;
                                /*
                                if (Device.SetLine(nChannel, Device.CHANNEL_LINEFREE) != 0)
                                {
                                    PhoneStateList[nChannel].bLineBusy = false;
                                }
                                */
                                Device.EnablePlayFile(nChannel, Device.LINE_PLAY, 0);
                            }
                        }
                        Debug.WriteLine(nChannel.ToString() + " " + "MCU_PLAYOVER" + " ");
                        break;
                    }
                default:
                    Debug.WriteLine(nChannel.ToString() + " " + "default other:" + nMsg.ToString() + " , " +Lparam.ToString());
                    break;
            }

            if (PhoneStateList[nChannel].bHookOff)
            {
                if (!PhoneStateList[nChannel].SavePhoneRecord.IsBusy)
                {
                    PhoneStateList[nChannel].SavePhoneRecord.RunWorkerAsync();
                }
            }

            PhoneStateListView.Items.Refresh();
        }


        private void CloseDo(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Boolean hasMsg = false;
            foreach (Phone p in PhoneStateList)
            {
                if (p.SavePhoneRecord.IsBusy)
                {
                    if (!hasMsg)
                    {
                        MessageBox.Show("设备正在录音，请等候");
                        hasMsg = true;
                    }
                    Thread.Sleep(500);
                }
            }
            if (hasMsg)
            {
                MessageBox.Show("即将关闭程序");
            }
            
            Device.FreeDevice();
        }

        //保存系统设置参数
        private void ConfigValueChange(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(System.Windows.Controls.TextBox))
            {
                TextBox sctb = sender as TextBox;
                if (sctb.Text != SetConfig.GetConfig(sctb.Name))
                {
                    string SV = SetConfig.SaveConfig(sctb.Name, sctb.Text);
                    if (sctb.Text != SV)
                    {
                        NoticeX.Show("参数名：" + sctb.Name + "\n应存值：" + sctb.Text + "\n实存值：" + SV, "保存系统参数异常", MessageBoxIcon.Question, 1000 * 10);
                        Debug.WriteLine(sctb.Name + " Change Fai");
                    }
                }
            }
            if (sender.GetType() == typeof(Slider))
            {
                Slider scs = sender as Slider;
                int v = int.Parse(Math.Floor(scs.Value).ToString());
                if (v.ToString() != SetConfig.GetConfig(scs.Name))
                {
                    AutoRecRingCountShow.Content = v.ToString();
                    string SV = SetConfig.SaveConfig(scs.Name, v.ToString());
                    if (v.ToString() != SV)
                    {
                        NoticeX.Show("参数名：" + scs.Name + "\n应存值：" + v.ToString() + "\n实存值：" + SV, "保存系统参数异常", MessageBoxIcon.Question, 1000 * 10);
                        Debug.WriteLine(scs.Name + " Change Fai");
                    }
                }
            }
            if (sender.GetType() == typeof(ComboBox))
            {
                ComboBox sccb = sender as ComboBox;
                if (sccb.SelectedIndex.ToString() != SetConfig.GetConfig(sccb.Name))
                {
                    string SV = SetConfig.SaveConfig(sccb.Name, sccb.SelectedIndex.ToString());
                    if (sccb.SelectedIndex.ToString() != SV)
                    {
                        NoticeX.Show("参数名：" + sccb.Name + "\n应存值：" + sccb.SelectedIndex.ToString() + "\n实存值：" + SV, "保存系统参数异常", MessageBoxIcon.Question, 1000 * 10);
                        Debug.WriteLine(sccb.Name + " Change Fai");
                    }
                }
            }
        }
    }
}
