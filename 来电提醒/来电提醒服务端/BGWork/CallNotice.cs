using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
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
            Phone nPhone = ServerWindow.PhoneStateList[Channel];
            if (nPhone.CallType != Device.CallType_Out)
            {
                //基本提示信息 参数
                string NoteMsg = "来电号码：" + nPhone.PhoneNumber;
                NoteMsg += "\n单位信息：" + nPhone.ComName;
                NoteMsg += "\n来电时间：" + nPhone.timeS.ToString();
                NoteMsg += "\n" + nPhone.MissedCall;
                NoteMsg += "\n来电线路：" +Channel.ToString()+"-"+ nPhone.ChannelName.ToString();

                string CallTitle = "电话呼入";
                MessageBoxIcon ico = MessageBoxIcon.Info;

                int ShowSec = SetConfig.GetConfig("NoticeConf_CallInShowT",10)*1000;

                if (isMissCall)
                {
                    CallTitle = "未接来电";
                    ico = MessageBoxIcon.Warning;
                    ShowSec = SetConfig.GetConfig("NoticeConf_MissCallShowT", 10) * 1000;
                }
                //服务端提醒
                NoticeX.Show(NoteMsg, CallTitle, ico, ShowSec);

                //SEEYON 致远OA提醒
                {
                    string SeeyonRingForm_Str = SetConfig.GetConfig("NoticeCong_RingSend_SeeyonOAForm","");
                    string SeeyonRingMsg_Str = SetConfig.GetConfig("NoticeCong_RingSend_SeeyonOAMsg","");
                    string SeeyonMissForm_Str = SetConfig.GetConfig("NoticeCong_MissSend_SeeyonOAForm","");
                    string SeeyonMissMsg_Str = SetConfig.GetConfig("NoticeCong_MissSend_SeeyonOAMsg","");
                    Boolean SeeyonRingForm_Bool = false;
                    Boolean SeeyonRingMsg_Bool = false;
                    Boolean SeeyonMissForm_Bool = false;
                    Boolean SeeyonMissMsg_Bool = false;
                    //来电响铃
                    if (!string.IsNullOrWhiteSpace(SeeyonRingForm_Str))
                    {
                        SeeyonRingForm_Bool = Boolean.Parse(SeeyonRingForm_Str);
                    }
                    if (!string.IsNullOrWhiteSpace(SeeyonRingMsg_Str))
                    {
                        SeeyonRingMsg_Bool = Boolean.Parse(SeeyonRingMsg_Str);
                    }
                    //漏接电话
                    if (!string.IsNullOrWhiteSpace(SeeyonMissForm_Str))
                    {
                        SeeyonMissForm_Bool = Boolean.Parse(SeeyonMissForm_Str);
                    }
                    if (!string.IsNullOrWhiteSpace(SeeyonMissMsg_Str))
                    {
                        SeeyonMissMsg_Bool = Boolean.Parse(SeeyonMissMsg_Str);
                    }

                    if (SeeyonRingForm_Bool || SeeyonRingMsg_Bool|| SeeyonMissForm_Bool || SeeyonMissMsg_Bool)
                    {
                        int ErrShowT = SetConfig.GetConfig("NoticeConf_ErrorShowT", 20) * 1000;

                        if (string.IsNullOrWhiteSpace(SetConfig.GetConfig("SeeyonConf_URL","")))
                        {
                            NoticeX.Show("未设置致远OA服务器信息", "数据发送失败", MessageBoxIcon.Warning, ErrShowT);
                        }
                        else if (string.IsNullOrWhiteSpace(SetConfig.GetConfig("SeeyonConf_User","")))
                        {
                            NoticeX.Show("未设置致远OA操作员信息", "数据发送失败", MessageBoxIcon.Warning, ErrShowT);
                        }
                        else if (string.IsNullOrWhiteSpace(SetConfig.GetConfig("SeeyonConf_RestUser", "")))
                        {
                            NoticeX.Show("未设置致远OA Rest用户信息", "数据发送失败", MessageBoxIcon.Warning, ErrShowT);
                        }
                        else if (string.IsNullOrWhiteSpace(SetConfig.GetConfig("SeeyonConf_RestPwd", "")))
                        {
                            NoticeX.Show("未设置致远OA Rest密钥", "数据发送失败", MessageBoxIcon.Warning, ErrShowT);
                        }
                        else if (string.IsNullOrWhiteSpace(SetConfig.GetConfig("SeeyonConf_UserCode", "")))
                        {
                            NoticeX.Show("未设置致远OA操作员代码", "数据发送失败", MessageBoxIcon.Warning, ErrShowT);
                        }
                        else if (string.IsNullOrWhiteSpace(SetConfig.GetConfig("SeeyonConf_Ring_FormCode", "")) &&SeeyonRingForm_Bool)
                        {
                            NoticeX.Show("未设置致远OA 来电响铃表单模板代码", "数据发送失败", MessageBoxIcon.Warning, ErrShowT);
                        }
                        else if (string.IsNullOrWhiteSpace(SetConfig.GetConfig("SeeyonConf_Miss_FormCode", ""))&& SeeyonMissForm_Bool)
                        {
                            NoticeX.Show("未设置致远OA 未接来电表单模板代码", "数据发送失败", MessageBoxIcon.Warning, ErrShowT);
                        }
                        else
                        {
                            ThreadSys.SeeyonOA.Basic OABasic = new ThreadSys.SeeyonOA.Basic();
                            if (SeeyonRingForm_Bool || SeeyonMissForm_Bool)
                            {
                                JObject OANoteHead = new JObject();
                                JObject OANoteDada = new JObject();

                                string API = "";
                                string submit = SetConfig.GetConfig("SeeyonConf_Ring_FormSubmit","");

                                string PhoneNoTitle = SetConfig.GetConfig("SeeyonConf_Ring_PhoneNoTitle", "");
                                string LineTitle = SetConfig.GetConfig("SeeyonConf_Ring_LineTitle", "");
                                string CallTypeTitle = SetConfig.GetConfig("SeeyonConf_Ring_CallTypeTitle", "");
                                string TimeSTitle = SetConfig.GetConfig("SeeyonConf_Ring_TimeSTitle", "");
                                string TimeETitle = SetConfig.GetConfig("SeeyonConf_Ring_TimeETitle", "");
                                string MemoTitle = SetConfig.GetConfig("SeeyonConf_Ring_MemoTitle", "");

                                if (isMissCall)
                                {
                                    API = "flow/" + SetConfig.GetConfig("SeeyonConf_Miss_FormCode", "");
                                    OANoteHead.Add("templateCode", SetConfig.GetConfig("SeeyonConf_Miss_FormCode", ""));
                                    submit = SetConfig.GetConfig("SeeyonConf_Miss_FormSubmit", "");

                                    PhoneNoTitle = SetConfig.GetConfig("SeeyonConf_Miss_PhoneNoTitle", "");
                                    LineTitle = SetConfig.GetConfig("SeeyonConf_Miss_LineTitle", "");
                                    CallTypeTitle = SetConfig.GetConfig("SeeyonConf_Miss_CallTypeTitle", "");
                                    TimeSTitle = SetConfig.GetConfig("SeeyonConf_Miss_TimeSTitle", "");
                                    TimeETitle = SetConfig.GetConfig("SeeyonConf_Miss_TimeETitle", "");
                                    MemoTitle = SetConfig.GetConfig("SeeyonConf_Miss_MemoTitle", "");

                                    if (!string.IsNullOrWhiteSpace(SetConfig.GetConfig("SeeyonConf_Miss_MissStateTitle", "")))
                                    {
                                        OANoteDada.Add(SetConfig.GetConfig("SeeyonConf_Miss_MissStateTitle", ""), 1);
                                    }
                                }
                                else
                                {
                                    API = "flow/" + SetConfig.GetConfig("SeeyonConf_Ring_FormCode", "");
                                    OANoteHead.Add("templateCode", SetConfig.GetConfig("SeeyonConf_Ring_FormCode", ""));
                                }
                                if (!string.IsNullOrWhiteSpace(PhoneNoTitle))
                                {
                                    OANoteDada.Add(PhoneNoTitle, nPhone.PhoneNumber);
                                }
                                if (!string.IsNullOrWhiteSpace(LineTitle))
                                {
                                    OANoteDada.Add(LineTitle, nPhone.ChannelName);
                                }
                                if (!string.IsNullOrWhiteSpace(CallTypeTitle))
                                {
                                    OANoteDada.Add(CallTypeTitle, nPhone.CallType);
                                }
                                if (!string.IsNullOrWhiteSpace(TimeSTitle))
                                {
                                    OANoteDada.Add(TimeSTitle, nPhone.timeS.ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                                if (!string.IsNullOrWhiteSpace(TimeETitle))
                                {
                                    OANoteDada.Add(TimeETitle, nPhone.timeE.ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                                if (!string.IsNullOrWhiteSpace(MemoTitle))
                                {
                                    OANoteDada.Add(MemoTitle, nPhone.MissedCall + nPhone.ComName);
                                }

                                if (submit == "True")
                                {
                                    OANoteHead.Add("param", "0");
                                }
                                else
                                {
                                    OANoteHead.Add("param", "1");
                                }
                                OANoteHead.Add("subject", CallTitle + nPhone.PhoneNumber + nPhone.ComName + nPhone.timeS.ToString("MM-dd HH:mm"));
                                OANoteHead.Add("senderLoginName", SetConfig.GetConfig("SeeyonConf_User", ""));
                                OANoteHead.Add("transfertype", "json");

                                OANoteHead.Add("data", OANoteDada);


                                string OAmsg = OABasic.PostJsonRStr(API, JsonConvert.SerializeObject(OANoteHead));
                                Debug.WriteLine("发送来电(未接)信息至Seeyon表单\n" + OAmsg);
                                NoticeX.Show(JsonConvert.SerializeObject(OANoteHead) + "\n" + OAmsg, "发送至Seeyon表单",ShowSec);
                            }
                            if (SeeyonRingMsg_Bool || SeeyonMissMsg_Bool)
                            {

                            }
                        }
                    }
                }
                
                //QQ


            }
        }
    }
}
