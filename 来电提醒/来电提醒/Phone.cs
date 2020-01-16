using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace PhoneInfo
{
    public class Phone
    {
        //通道
        public int Channel { get; set; } = 0;

        //通道名称
        public string ChannelName { get; set; } = "";

        //设备码
        public string DeviceCode { get; set; } = "";

        //设备状态
        public string DeviceInfo { get; set; } = "";


        //线路状态
        public string LineState { get; set; }

        public string CallerID { get; set; }
        public string DialedNum { get; set; }
        public string TalkTime { get; set; }
        public DateTime time1 { get; set; }
        public DateTime time2 { get; set; }


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

        public Phone(int _Channel)
        {
            Channel = _Channel;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class DeviceVal
    {
        public const int DEVICE_AD130 = 1;
        public const int DEVICE_AD230 = 2;
        public const int DEVICE_AD430 = 3;




        public const int MAX_DEVICE = 0x04;


        public const int MCU_BACKSTATE = 0x08;		// Return channel State
        public const int MCU_BACKCID = 0x09;		// Return CallerID
        public const int MCU_BACKDIGIT = 0x0A;		// Return Dial Digit
        public const int MCU_BACKPARAM = 0x0C;		// Return Paramter
        public const int MCU_BACKCPUVER = 0x0D;		// Return CPU version
        public const int MCU_BACKCOLLATERAL = 0x0E;	// Return Collateral phone dialed
        public const int MCU_BUSYTONE = 0x0F;		// Return detect line busy tone
        public const int MCU_PLAYVOLUME = 0X10;     // Return volume value of playback
        public const int MCU_RECORDLVOLUME = 0X11;  // Return left volume value of record
        public const int MCU_RECORDRVOLUME = 0X12;  // Return right volume value of record
        public const int MCU_LINEMODE = 0X13;       // Return line voltage mode
        public const int MCU_DEVICECODE = 0X14;     // Return device code
        public const int MCU_MICPARAM = 0X15;       // Return MIC trigger param
        public const int MCU_EARPARAM = 0X16;       // Return EAR trigger param
        public const int MCU_TIMEOUTPARAM = 0X17;   // Return Timeout param
        public const int MCU_MICTRIGGERED = 0X18;   // Return MIC triggered state
        public const int MCU_EARTRIGGERED = 0X19;   // Return EAR triggered state
        public const int MCU_SWPLAYVOLUME = 0X20;	// Return software boosted playback volume
        public const int MCU_SWRECVOLUME = 0X21;	// Return software boosted recording volume

        public const int MCU_BACKDISABLE = 0xFF;		// Return Device is disabled
        public const int MCU_BACKENABLE = 0xEE;		// Return Device is enabled
        public const int MCU_BACKMISSED = 0xAA;		// Return Missed call 
        public const int MCU_BACKTALK = 0xBB;		// Return talk time
        public const int MCU_PLAYOVER = 0xCC;     // Return play sound over


        public const int HOOKON_POSITIVEPOLARITY = 0x01; // Phone hook on with positive polarity
        public const int HOOKON_NEGATIVEPOLARITY = 0x02;	// Phone hook on with negative polarity
        public const int HAVE_POLARITY = 0x03;	// Line have polarity 
        public const int HOOKOFF_POSITIVEPOLARITY = 0x04;	// Phone hook off with positive polarity
        public const int HOOKOFF_NEGATIVEPOLARITY = 0x05;	// Phone hook off with negative polarity
        public const int NOLINE_NOPOLARITY = 0x06; // No line with no polarity
        public const int RINGON = 0x07;	// Phone ring on
        public const int RINGOFF = 0x08;	// Phone ring off 
        public const int NOHOOK_POSITIVEPOLARITY = 0x09; // No hook(doesn't operate phone) with positive polarity
        public const int NOHOOK_NEGATIVEPOLARITY = 0x0A;	// No hook(doesn't operate phone) with negative polarity
        public const int NOHOOK_NOPOLARITY = 0x0B;	// No hook(doesn't operate phone) with no polarity

        public const int CHANNE_LINEBUSY = 0x00;		// Make line of specified channel to busy
        public const int CHANNEL_LINEFREE = 0x01;		// Make line of specified channel to free
        public const int CHANNEL_CONNECTPHONE = 0x02;		// Make line of specified to connect with phone
        public const int CHANNEL_DISCONNECTPHONE = 0x03;		// Make line of specified to disconnect with phone

        public const int CLEAR_CALLERID = 0x00;	// clear caler id 
        public const int CLEAR_DILAEDNUM = 0x01;	// clear dialed number
        public const int CLEAR_RINGCOUNT = 0x02;	// clear ring count
        public const int CLEAR_TALKTIME = 0x03;	// clear talk time

        public const int DEVICE_PLAY = 0x00;	// use AD130 deivce to play sound
        public const int LINE_PLAY = 0x01;	// play sound to line(the other side can hear the sound played);

        public const int VOLUME_PLAY = 0X00;	// volume for device plays sound
        public const int VOLUME_RECORDLINE = 0X01; // line mode, volume for device records
        public const int VOLUME_RECORDEAR = 0X02; // handset mode, Ear volume for device records
        public const int VOLUME_RECORDMIC = 0X03; // handset mode, Mic volume for device records


        public const int LINEMODE_24V = 0X24; // line voltage is 24v mode
        public const int LINEMODE_48V = 0X48; // line voltage is 48v mode




        public const int COLUMN_CHANNELNO = 0;
        public const int COLUMN_CHANNELSTATE = 1;
        public const int COLUMN_LINESTATE = 2;
        public const int COLUMN_CALLERID = 3;
        public const int COLUMN_DIALEDNUM = 4;
        public const int COLUMN_TALKTIME = 5;
        public const int COLUMN_CPUVER = 6;
        public const int COLUMN_DEVICECODE = 7;

        public const int WM_AD130MSG = 1024 + 220;

        //TAGSTATE[] m_State = new TAGSTATE[MAX_DEVICE];
    }


}
