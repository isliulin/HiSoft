using System;
using System.Collections.Generic;
using System.Text;

namespace 来电提醒客户端
{
    public enum Msg_Type { channelSet,DevMsg,Welcome ,unknow}
    public class CS_Msg
    {
        public string MsgType=Msg_Type.unknow.ToString();
        public List<string> channelConfig = new List<string>();
        public int wParam = new int();
        public int Lparam = new int();
    }
}
