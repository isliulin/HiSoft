using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace 来电提醒服务端.BGWork
{
    public class UploadRecFile
    {
        private int Channel;
        private string FilePath=null;
        private string RecInfoJson=null;

        BackgroundWorker UploadWorker = new BackgroundWorker();

        public UploadRecFile(int _Channel,string _FilePath,string _RecInfoJson=null)
        {
            Channel = _Channel;
            FilePath = _FilePath;
            RecInfoJson = _RecInfoJson;

            UploadWorker.DoWork += WorkFunction;
        }

        public void Running()
        {
            UploadWorker.RunWorkerAsync();
        }

        void WorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        void WorkFunction(object sender, DoWorkEventArgs e)
        {
            Phone nPhone = ServerWindow.PhoneStateList[Channel];
            //执行上传到致远OA表单动作
            if (SetConfig.GetConfig("RecFileUploadType", 99) == 0)
            {
                int ErrShowT = SetConfig.GetConfig("NoticeConf_ErrorShowT", 20) * 1000;

                if (string.IsNullOrWhiteSpace(SetConfig.GetConfig("SeeyonConf_URL", "")))
                {
                    NoticeX.Show("未设置致远OA服务器信息", "数据发送失败", MessageBoxIcon.Warning, ErrShowT);
                }
                else if (string.IsNullOrWhiteSpace(SetConfig.GetConfig("SeeyonConf_User", "")))
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
                else if (string.IsNullOrWhiteSpace(SetConfig.GetConfig("SeeyonConf_Rec__FormCode", "")))
                {
                    NoticeX.Show("未设置致远OA 录音存储表单模板代码", "数据发送失败", MessageBoxIcon.Warning, ErrShowT);
                }
                else if (string.IsNullOrWhiteSpace(FilePath))
                {
                    NoticeX.Show("录音文件路径为空", "数据发送失败", MessageBoxIcon.Warning, ErrShowT);
                }
                else
                {
                    ThreadSys.SeeyonOA.Basic OABasic = new ThreadSys.SeeyonOA.Basic();
                    string API = "";
                    string Memo = "";

                    JObject OANoteHead = new JObject();
                    JObject OANoteDada = new JObject();

                    //生成表单主数据，优先执行，以免异步上传等操作延时中 来电，造成参数变动
                    //也可使用 RecBack的信息字符串来配置，懒得弄了
                    string submit = SetConfig.GetConfig("SeeyonConf_Rec__FormSubmit", "");

                    OANoteHead.Add("templateCode", SetConfig.GetConfig("SeeyonConf_Rec__FormCode", ""));
                    OANoteHead.Add("senderLoginName", SetConfig.GetConfig("SeeyonConf_User", ""));
                    OANoteHead.Add("transfertype", "json");
                    if (submit == true.ToString())
                    {
                        OANoteHead.Add("param", "0");
                    }
                    else
                    {
                        OANoteHead.Add("param", "1");
                    }

                    string PhoneNoTitle = SetConfig.GetConfig("SeeyonConf_Rec__PhoneNoTitle", "");
                    string LineTitle = SetConfig.GetConfig("SeeyonConf_Rec__LineTitle", "");
                    string CallTypeTitle = SetConfig.GetConfig("SeeyonConf_Rec__CallTypeTitle", "");
                    string TimeSTitle = SetConfig.GetConfig("SeeyonConf_Rec__TimeSTitle", "");
                    string TimeETitle = SetConfig.GetConfig("SeeyonConf_Rec__TimeETitle", "");
                    string TalkTimeTitle = SetConfig.GetConfig("SeeyonConf_Rec__TimeETitle", "");
                    string MemoTitle = SetConfig.GetConfig("SeeyonConf_Rec__MemoTitle", "");
                    string MissStateTitle = SetConfig.GetConfig("SeeyonConf_Rec__TimeETitle", "");

                    if (string.IsNullOrWhiteSpace(PhoneNoTitle))
                    {
                        OANoteHead.Add(PhoneNoTitle, nPhone.PhoneNumber);
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
                    if (!string.IsNullOrWhiteSpace(TalkTimeTitle))
                    {
                        OANoteDada.Add(TalkTimeTitle, nPhone.MissedCall + nPhone.TalkTime);
                    }
                    if (!string.IsNullOrWhiteSpace(TalkTimeTitle))
                    {
                        OANoteDada.Add(TalkTimeTitle, nPhone.MissedCall + nPhone.TalkTime);
                    }
                    if (!string.IsNullOrWhiteSpace(MissStateTitle))
                    {
                        if (nPhone.MissedCall.Length > 0 || nPhone.CallType == Device.CallType_AutoRec)
                        {
                            OANoteDada.Add(MissStateTitle, 1);
                            OANoteHead.Add("subject", "【未接来电】" + nPhone.PhoneNumber + nPhone.ComName + nPhone.timeS.ToString("MM-dd HH:mm"));
                            Memo += "|" + nPhone.MissedCall;
                        }
                        else
                        {
                            OANoteHead.Add("subject", nPhone.PhoneNumber + nPhone.ComName + nPhone.timeS.ToString("MM-dd HH:mm"));
                        }

                    }

                    string hasFileTitle = SetConfig.GetConfig("SeeyonConf_Rec__FileStateTitle", "");
                    string FileIDTitle = SetConfig.GetConfig("SeeyonConf_RecFileTitle", "");

                    //上传录音文件
                    if (!File.Exists(FilePath))
                    {
                        Memo += "|" + FilePath + "路径错误。";
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(FileIDTitle))
                        {
                            long fileID = OABasic.UploadFile(FilePath);
                            if (fileID > 0 || fileID < -10)
                            {
                                OANoteDada.Add(FileIDTitle, fileID);
                                if (!string.IsNullOrWhiteSpace(hasFileTitle))
                                {
                                    OANoteDada.Add(hasFileTitle, 1);
                                }
                            }
                            else
                            {
                                Memo += "|" + FilePath + "上传失败(" + fileID + ")";
                            }
                        }
                    }

                    OANoteHead.Add("data", OANoteDada);
                    //发送表单
                    string OAmsg = OABasic.PostJsonRStr(API, JsonConvert.SerializeObject(OANoteHead));
                    Debug.WriteLine("录音文件上传\n" + JsonConvert.SerializeObject(OANoteHead) + "\n" + OAmsg);
                    NoticeX.Show(JsonConvert.SerializeObject(OANoteHead) + "\n" + OAmsg, "录音文件上传",SetConfig.GetConfig("NoticeConf_ErrorShowT", 3) * 1000);
                }
            }

        }
    }
}
