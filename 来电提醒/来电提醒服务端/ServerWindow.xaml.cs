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
using System.CodeDom;
using System.Windows.Controls.Primitives;
using System.IO;
using System.Web;

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
                string pn = SetConfig.GetConfig("LanName" + (i + 1).ToString(),"线路"+(i+1).ToString());
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
                        Phone nphone = JsonConvert.DeserializeObject<Phone>(e.UserState.ToString());

                        BGWork.UploadRecFile Upload = new BGWork.UploadRecFile(e.ProgressPercentage - 100, nphone.RecFileName);
                        Upload.Running();
                        //NoticeX.Show(e.UserState.ToString(), "录音完成，准备上传", MessageBoxIcon.Success);
                    }
                    break;
            }
        }

        void SavePhoneRecoreingFile(object sender, DoWorkEventArgs e)
        {
            
        }
        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
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

        //获取设置参数
        private void ConfigWindowLoaded(object sender, RoutedEventArgs e)
        {
            List<object> ConfigObjList = new List<object>();
            //获取参数控件
            if (sender.GetType() == typeof(Grid))
            {
                Grid SenderTab = sender as Grid;
                foreach (object ConfigObject in SenderTab.Children)
                {
                    ConfigObjList.Add(ConfigObject);
                }
            }
            if (sender.GetType() == typeof(UniformGrid))
            {
                UniformGrid SenderTab = sender as UniformGrid;
                foreach (object ConfigObject in SenderTab.Children)
                {
                    ConfigObjList.Add(ConfigObject);
                }
            }
            if (sender.GetType() == typeof(Grid)|| sender.GetType() == typeof(UniformGrid))
            {
                
            }
            //获取(参数)控件值
            foreach (object ConfigObj in ConfigObjList)
            {
                //Debug.WriteLine(ConfigObj.GetType());
                if (ConfigObj.GetType() == typeof(TextBox))
                {
                    TextBox ConfigControl = ConfigObj as TextBox;
                    if (!string.IsNullOrEmpty(ConfigControl.Name))
                    {
                        ConfigControl.Text = SetConfig.GetConfig(ConfigControl.Name,"");
                    }
                }
                if (ConfigObj.GetType() == typeof(Slider))
                {
                    Slider ConfigControl = ConfigObj as Slider;
                    try
                    {
                        if (!string.IsNullOrEmpty(ConfigControl.Name))
                        {
                            if (ConfigControl.IsSnapToTickEnabled)
                            {
                                int sv = SetConfig.GetConfig(ConfigControl.Name,0);
                                ConfigControl.Value = sv;
                            }
                            else
                            {
                                double sv = double.Parse(SetConfig.GetConfig(ConfigControl.Name,"0"));
                                ConfigControl.Value = sv;
                            }
                            //Debug.WriteLine("加载参数" + ConfigControl.Name + ":" + sv);
                        }
                    }
                    catch
                    {
                        if (ConfigControl.Name == AutoRecRingCount.Name)
                        {
                            ConfigControl.Value = 0;
                        }
                    }
                    //初始化提示文本
                    if (ConfigControl.Name == AutoRecRingCount.Name)
                    {
                        AutoRecRingCountShow.Content =Math.Floor( AutoRecRingCount.Value);
                    }
                }
                if (ConfigObj.GetType() == typeof(ComboBox))
                {
                    ComboBox ConfigControl = ConfigObj as ComboBox;
                    if (!string.IsNullOrEmpty(ConfigControl.Name))
                    {
                        string sv = SetConfig.GetConfig(ConfigControl.Name,"");
                        if (!string.IsNullOrWhiteSpace(sv))
                        {
                            ConfigControl.SelectedIndex = int.Parse(sv);
                        }
                    }
                }
                if (ConfigObj.GetType() == typeof(CheckBox))
                {
                    CheckBox ConfigControl = ConfigObj as CheckBox;
                    if (!string.IsNullOrEmpty(ConfigControl.Name))
                    {
                        string sv = SetConfig.GetConfig(ConfigControl.Name, "");
                        if (!string.IsNullOrEmpty(sv))
                        {
                            Boolean svb = Boolean.Parse(sv);
                            ConfigControl.IsChecked = svb;
                        }
                    }
                }
                if (ConfigObj.GetType() == typeof(RadioButton))
                {
                    RadioButton ConfigControl = ConfigObj as RadioButton;
                    if (!string.IsNullOrEmpty(ConfigControl.Name))
                    {
                        string sv = SetConfig.GetConfig(ConfigControl.Name, "");
                        if (!string.IsNullOrEmpty(sv))
                        {
                            Boolean svb = Boolean.Parse(sv);
                            ConfigControl.IsChecked = svb;
                        }
                    }
                }
                if (ConfigObj.GetType() == typeof(PasswordBox))
                {
                    PasswordBox ConfigControl = ConfigObj as PasswordBox;
                    if (!string.IsNullOrEmpty(ConfigControl.Name))
                    {
                        ConfigControl.Password = SetConfig.GetConfig(ConfigControl.Name, "");
                    }
                }
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
                                        //PhoneStateList[nChannel].PhoneNumber = "";
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
                                        //PhoneStateList[nChannel].PhoneNumber = "";
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
                                        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
                                        PhoneStateList[nChannel].timeS_UTC = (long)(DateTime.Now - startTime).TotalMilliseconds;
                                    }
                                    string szRing = "响铃:" + string.Format("{0:D2}", iRing);
                                    PhoneStateList[nChannel].LineState = szRing;
                                    PhoneStateList[nChannel].TalkTime = "";
                                    //PhoneStateList[nChannel].DialedNum = "";
                                    PhoneStateList[nChannel].CallType = Device.CallType_In;

                                    int AutoRecRingCount = SetConfig.GetConfig("AutoRecRingCount",0);
                                    
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
                        PhoneStateList[nChannel].MissedCall = "漏接:" + PhoneStateList[nChannel].PhoneNumber+"|";
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

                                if (PhoneStateList[nChannel].CallType == Device.CallType_In&&!PhoneStateList[nChannel].bAutodial)
                                {
                                    Device.HangupPhone(nChannel);
                                }
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
            string SenderName = "";
            string SenderValue = "";
            if (sender.GetType() == typeof(TextBox))
            {
                TextBox SenderObject = sender as TextBox;
                SenderName = SenderObject.Name;
                SenderValue = SenderObject.Text;
            }
            if (sender.GetType() == typeof(Slider))
            {
                Slider SenderObject = sender as Slider;
                SenderName = SenderObject.Name;
                if (SenderObject.IsSnapToTickEnabled)
                {
                    SenderValue = Math.Floor(SenderObject.Value).ToString();
                    SenderObject.Value = int.Parse(SenderValue);
                }
                else
                {
                    SenderValue = SenderObject.Value.ToString();
                }
            }
            if (sender.GetType() == typeof(ComboBox))
            {
                ComboBox SenderObject = sender as ComboBox;
                SenderName = SenderObject.Name;
                SenderValue = SenderObject.SelectedIndex.ToString();
            }
            if (sender.GetType() == typeof(CheckBox))
            {
                CheckBox SenderObject = sender as CheckBox;
                SenderName = SenderObject.Name;
                SenderValue = SenderObject.IsChecked.ToString();
            }
            if (sender.GetType() == typeof(RadioButton))
            {
                RadioButton SenderObject = sender as RadioButton;
                SenderName = SenderObject.Name;
                SenderValue = SenderObject.IsChecked.ToString();
            }
            if (sender.GetType() == typeof(PasswordBox))
            {
                PasswordBox SenderObject = sender as PasswordBox;
                SenderName = SenderObject.Name;
                SenderValue = SenderObject.Password.ToString();
            }
            if (!string.IsNullOrEmpty(SenderName))
            {
                if (SenderValue != SetConfig.GetConfig(SenderName,""))
                {
                    string ConfigValue = SetConfig.SaveConfig(SenderName, SenderValue);
                    if (SenderValue != ConfigValue)
                    {
                        NoticeX.Show("参数名：" + SenderName + "\n应存值：" + SenderValue + "\n实存值：" + ConfigValue, "保存参数异常", MessageBoxIcon.Question,SetConfig.GetConfig(NoticeConf_ErrorShowT.Name,10) * 1000);
                        Debug.WriteLine("保存参数异常:  参数名：" + SenderName + "|应存值：" + SenderValue + "|实存值：" + ConfigValue);
                    }
                    else
                    {
                        
                    }
                }
            }
            if (SenderName == AutoRecRingCount.Name)
            {
                AutoRecRingCountShow.Content = Math.Floor(AutoRecRingCount.Value).ToString();
            }
        }

        //前端交互控制
        private void UIControl(object sender, RoutedEventArgs e)
        {
            try
            {

                //致远参数面板

                if (RecFileUploadType.SelectedIndex == 0)
                {
                    SeeyonConfigPage.Visibility = Visibility.Visible;
                }
                else if ((Boolean)NoticeCong_RingSend_SeeyonOAForm.IsChecked || (Boolean)NoticeCong_RingSend_SeeyonOAMsg.IsChecked)
                {
                    SeeyonConfigPage.Visibility = Visibility.Visible;
                }
                else if ((Boolean)NoticeCong_MissSend_SeeyonOAForm.IsChecked || (Boolean)NoticeCong_MissSend_SeeyonOAMsg.IsChecked)
                {
                    SeeyonConfigPage.Visibility = Visibility.Visible;
                }
                else
                {
                    SeeyonConfigPage.Visibility = Visibility.Collapsed;
                }
                //QQ参数面板
                if ((Boolean)NoticeCong_RingSend_QQMsg.IsChecked || (Boolean)NoticeCong_RingSend_QQWin.IsChecked)
                {
                    QQConfigPage.Visibility = Visibility.Visible;
                }
                else if ((Boolean)NoticeCong_MissSend_QQMsg.IsChecked || (Boolean)NoticeCong_MissSend_QQWin.IsChecked)
                {
                    QQConfigPage.Visibility = Visibility.Visible;
                }
                else
                {
                    QQConfigPage.Visibility = Visibility.Collapsed;
                }

                
            }
            catch (Exception ex)
            {

            }
        }


        private void TestWork_Click(object sender, RoutedEventArgs e)
        {
            ThreadSys.SeeyonOA.Basic OAB = new ThreadSys.SeeyonOA.Basic();
            var txtFiles = Directory.EnumerateFiles("E:\\", "*.wav");

            foreach (string currentFile in txtFiles)
            {
               var regkey = Microsoft.Win32.Registry.ClassesRoot;
               var extn = System.IO.Path.GetExtension(currentFile);
              //look for extension
              var fileextkey = regkey.OpenSubKey(extn);

             //retrieve Content Type value
               var filecontenttype = fileextkey.GetValue("Content Type", "application/unknown").ToString();

                //System.Web.MimeMapping.GetMimeMapping(fileName)
                //GetMimeMapping(path)
                Debug.WriteLine(currentFile+"|"+extn + "|" + filecontenttype);
                //string fileName = currentFile.Substring(sourceDirectory.Length + 1);
                //Directory.Move(currentFile, Path.Combine(archiveDirectory, fileName));
            }

            //System.IO.Directory directory = new System.IO.Directory("E:\\");
            //long u = OAB.UploadFile(@"E:\PhoneCall_02_20200701_194358_Record.wav");
            //Task<string> u = OAB.UploadFile(@"E:\PhoneCall_02_20200701_194358_Record.wav");
            
        }

        //备份恢复页面动作
        private void BackupConfPageLoaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser;
            Microsoft.Win32.RegistryKey software = key.CreateSubKey("software\\" + SetConfig.ServiceName);
            software = key.OpenSubKey("software\\" + SetConfig.ServiceName, true);

            JObject SysConfigJson = new JObject();
            foreach (string ParamName in software.GetValueNames())
            {
                SysConfigJson.Add(ParamName, software.GetValue(ParamName).ToString());
            }
            Clipboard.SetText(SysConfigJson.ToString());

            BackupConfigShow.Paste();
        }

        private void ResetSYSConfigDO(object sender, RoutedEventArgs e)
        {
            TextRange ConfigText = new TextRange(BackupConfigShow.Document.ContentStart, BackupConfigShow.Document.ContentEnd);
            JObject SysConfigJson = JsonConvert.DeserializeObject<JObject>(ConfigText.Text);

            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser;
            Microsoft.Win32.RegistryKey software = key.CreateSubKey("software\\" + SetConfig.ServiceName);
            software = key.OpenSubKey("software\\" + SetConfig.ServiceName, true);

            foreach (var param in SysConfigJson)
            {
                if (param.Value.ToString() != SetConfig.GetConfig(param.Key, ""))
                {
                    software.SetValue(param.Key, param.Value);
                }
            }
            MessageBox.Show("恢复设置完成，请重新启动程序", "恢复设置");
            Application.Current.Shutdown();
        }

        private void OpenBackupPage(object sender, RoutedEventArgs e)
        {
            BackupConfigPage.Visibility = Visibility.Visible;
            BackupConfigPage.IsSelected = true;
            BackupConfigPage.Visibility = Visibility.Collapsed;
        }
    }
}
