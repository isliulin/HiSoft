using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace 来电提醒服务端.BGWork
{
    public class CallNotice
    {
        BackgroundWorker CallNoticeWork = new BackgroundWorker();

        private int Channel;
        private string PhoneNo;
        private Boolean isMissCall;

        public CallNotice(int _channel, string _PhoneNo, Boolean _isMissCall =false)
        {
            Channel = _channel;
            PhoneNo = _PhoneNo;
            isMissCall = _isMissCall;

            CallNoticeWork.DoWork += WorkFunction;
            CallNoticeWork.RunWorkerCompleted += WorkComplete;
        }
        public void Running()
        {
            CallNoticeWork.RunWorkerAsync();
        }

        void WorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        void WorkFunction(object sender, DoWorkEventArgs e)
        {
            if (ServerWindow.PhoneStateList[Channel].CallType != Device.CallType_Out)
            {
                string NoteMsg = "来电号码：" + ServerWindow.PhoneStateList[Channel].PhoneNumber;
                NoteMsg += "\n单位信息：" + ServerWindow.PhoneStateList[Channel].ComName;
                NoteMsg += "\n来电时间：" + ServerWindow.PhoneStateList[Channel].timeS.ToString();
                NoteMsg += "\n" + ServerWindow.PhoneStateList[Channel].MissedCall;
                switch (SetConfig.GetConfig("NoticeType"))
                {
                    case "0":
                        Panuon.UI.Silver.NoticeX.Show(NoteMsg, ServerWindow.PhoneStateList[Channel].ChannelName + "来电", 1000 * 5);
                        break;
                    default:
                        Panuon.UI.Silver.NoticeX.Show(NoteMsg, ServerWindow.PhoneStateList[Channel].ChannelName + "来电", 1000 * 5);
                        break;
                }
                if (isMissCall)
                {
                    //NoteMsg+="      响铃次数："+ ServerWindow.PhoneStateList[Channel].
                    Panuon.UI.Silver.NoticeX.Show(NoteMsg, ServerWindow.PhoneStateList[Channel].ChannelName + "来电无人接听",MessageBoxIcon.Warning);
                }
            }
        }
    }
}
