using System;
using System.Collections.Generic;
using System.Text;

namespace 来电提醒客户端
{
    public enum Msg_Type { channelSet,DevMsg,Welcome ,unknow,isServer,SQLConfig}
    public class CS_Msg
    {
        public string MsgType=Msg_Type.unknow.ToString();
        public string channel0 = "";
        public string channel1 = "";
        public string channel2 = "";
        public string channel3 = "";
        public int wParam = 0;
        public int Lparam = 0;
    }
}
