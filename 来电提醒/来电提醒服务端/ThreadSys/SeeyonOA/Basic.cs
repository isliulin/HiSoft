using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace 来电提醒服务端.ThreadSys.SeeyonOA
{
    public class Basic
    {
        public enum OAEnumTitle { ID = 0, ENUMVALUE = 1, SHOWVALUE = 2 };
        public enum OAEnumPath { Head = 0, Sub = 1 }
        public enum OADepTitle { id = 0, code = 1, name = 2 }

        private string oaurl;
        public string Token;
        private string CompanyID;
        private string CompanyName;
        private XmlNode EnumRoot;
        private JArray Departments;


        public Basic()
        {
            oaurl = string.Format("{0}/seeyon/rest/", SetConfig.GetConfig("SeeyonConf_URL","http://127.0.0.1"));
            //RJ.StepLog.Add("准备登录"+oaurl);
            
            JObject UserAuth = new JObject();
            UserAuth.Add("userName", SetConfig.GetConfig("SeeyonConf_RestUser",""));
            UserAuth.Add("password", SetConfig.GetConfig("SeeyonConf_RestPwd",""));
            UserAuth.Add("loginName", SetConfig.GetConfig("SeeyonConf_User",""));
            UserAuth.Add("memberCode", SetConfig.GetConfig("SeeyonConf_UserCode",""));
            string UserData = UserAuth.ToString(Newtonsoft.Json.Formatting.None, null);
            
            JObject Tokenjson = PostJson("token/", UserData);
            
            if (Tokenjson.Property("id")==null)
            {
                //RJ.ex = Service.Error.ErrorSet.未能获取到记录_请确认相关参数是否正确;
                Debug.WriteLine("登陆【失败】"); 
            }
            else
            {
                Debug.WriteLine("OA登陆【成功】");
                //RJ.StepLog.Add("Tokenjson.Property(id)" + Tokenjson.Property("id").ToString());
                Token = Tokenjson["id"].ToString();
                CompanyID = Tokenjson["bindingUser"]["loginAccount"].ToString();
                CompanyName = Tokenjson["bindingUser"]["loginAccountName"].ToString();
                
                /*
                //获取枚举档案 
                EnumRoot = GetXMLNode("enum/export/false?unitIds=" + CompanyID);
                RJ.StepLog.Add("获取枚举档案完成");

                // 获取部门档案 
                Departments = GetJArray("orgDepartments/" + CompanyID);
                RJ.StepLog.Add("获取部门档案完成");
                */
            }
        }

        private HttpClient OA_HttpClient()
        {
            HttpClient hc = new HttpClient();
            hc.DefaultRequestHeaders.Add("Accept", " application/json");
            hc.Timeout = TimeSpan.FromSeconds(1000000) ;
            
            if (!string.IsNullOrWhiteSpace(Token))
            {
                hc.DefaultRequestHeaders.Add("token", Token);
            }
            return hc;
        }

        private HttpContent OA_PostBody(string PostBody)
        {
            HttpContent RPostBody = new StringContent(PostBody);
            RPostBody.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            RPostBody.Headers.ContentType.CharSet = "utf-8";
            return RPostBody;
        }

        public long UploadFile(string filePath)
        {
            try
            {
                //long ResultCode = 0;
                //判断文件路径是否正确
                if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                {
                    return -1;
                }

                string fileName = Path.GetFileName(filePath);
                var fileExtName = System.IO.Path.GetExtension(filePath);
                string API = "attachment?token=" + Token + "&file=" + fileName;

                //设置文件流间隔字段，自定义
                string boundary = string.Format("----HiSoft{0}", DateTime.Now.Ticks.ToString("x"));

                //设置文件流参数
                MultipartFormDataContent FormDataContent = new MultipartFormDataContent(boundary);
                FormDataContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data; boundary =" + boundary);
                FormDataContent.Headers.Add("client", "true");
                //获取文件类型
                var fileContentType = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(fileExtName).GetValue("Content Type", "application/unknown").ToString();

                #region Stream请求
                //获取文件流
                FileStream fStream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                StreamContent streamContent = new StreamContent(fStream, (int)fStream.Length);
                //设置文件流文件类型
                streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(fileContentType);

                //添加文件
                FormDataContent.Add(streamContent, fileName, fileName);

                #endregion

                var result = OA_HttpClient().PostAsync(oaurl+API, FormDataContent).Result;

                try
                {
                    if (result.IsSuccessStatusCode)
                    {
                        string rslt = result.Content.ReadAsStringAsync().Result;

                        JObject RJ = JsonConvert.DeserializeObject<JObject>(rslt);
                        string fileID = RJ["atts"][0]["fileUrl"].ToString();
                        Debug.WriteLine(rslt);
                        Debug.WriteLine(fileID);
                        if (string.IsNullOrEmpty(fileID))
                        {
                            return -2;
                        }
                        else
                        {
                            return long.Parse(fileID);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(string.Format("获取服务器返回结果错误：消息：{0},堆栈：{1}", ex.Message, ex.StackTrace));
                    return -3;
                }
                finally
                {
                    fStream.Close();
                }
                return 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message,ex.HResult.ToString());
                NoticeX.Show("----"+ex.Message, "保存file异常",SetConfig.GetConfig("NoticeConf_ErrorShowT",30)* 1000 );
                return 0;
                throw new Exception("保存file异常");
            }
        }



        public JObject GetJson(string api)
        {
            JObject ReceiveJsonObj = new JObject();
            Task<HttpResponseMessage> GetTokenHRM = OA_HttpClient().GetAsync(oaurl + api);
            if (GetTokenHRM.Result.IsSuccessStatusCode)
            {
                string ReceiveStr = GetTokenHRM.Result.Content.ReadAsStringAsync().Result;
                ReceiveJsonObj = JsonConvert.DeserializeObject<JObject>(ReceiveStr);
            }
            return ReceiveJsonObj;
        }

        public JArray GetJArray(string api)
        {
            JArray ReceiveJsonObj = new JArray();
            Task<HttpResponseMessage> GetTokenHRM = OA_HttpClient().GetAsync(oaurl + api);
            if (GetTokenHRM.Result.IsSuccessStatusCode)
            {
                string ReceiveStr = GetTokenHRM.Result.Content.ReadAsStringAsync().Result;
                ReceiveJsonObj = JsonConvert.DeserializeObject<JArray>(ReceiveStr);
            }
            return ReceiveJsonObj;
        }

        public XmlNode GetXMLNode(string api)
        {
            XmlDocument TempXD = new XmlDocument();
            Console.WriteLine(oaurl + api);
            Task<HttpResponseMessage> GetTokenHRM = OA_HttpClient().GetAsync(oaurl + api);
            if (GetTokenHRM.Result.IsSuccessStatusCode)
            {
                string ReceiveStr = GetTokenHRM.Result.Content.ReadAsStringAsync().Result;
                Console.WriteLine("Get XML Node RStr:" + ReceiveStr);
                TempXD.LoadXml(ReceiveStr);
            }
            else
            {
                Console.WriteLine(GetTokenHRM.Result.StatusCode);
            }
            XmlNode TempXDRoot = TempXD.DocumentElement;
            return TempXDRoot;
        }

        public JObject PostJson(string api, string PostBody)
        {
            JObject ReceiveJsonObj = new JObject();
            Task<HttpResponseMessage> GetTokenHRM = OA_HttpClient().PostAsync(oaurl + api, OA_PostBody(PostBody));

            if (GetTokenHRM.Result.IsSuccessStatusCode)
            {
                string ReceiveStr = GetTokenHRM.Result.Content.ReadAsStringAsync().Result;
                ReceiveJsonObj = JsonConvert.DeserializeObject<JObject>(ReceiveStr);
            }
            return ReceiveJsonObj;
        }
        public string PostJsonRStr(string api, string PostBody)
        {
            //JObject ReceiveJsonObj = new JObject();
            string ReceiveStr="";
            Task<HttpResponseMessage> GetTokenHRM = OA_HttpClient().PostAsync(oaurl + api, OA_PostBody(PostBody));

            if (GetTokenHRM.Result.IsSuccessStatusCode)
            {
                ReceiveStr = GetTokenHRM.Result.Content.ReadAsStringAsync().Result;
                
                return ReceiveStr;
                //ReceiveJsonObj = JsonConvert.DeserializeObject<JObject>(ReceiveStr);
            }
            return ReceiveStr;
        }

        public string GetEnumValTrue(string valid, OAEnumTitle SouT = OAEnumTitle.ID, OAEnumTitle TarT = OAEnumTitle.ENUMVALUE)
        {
            return EnumRoot.SelectSingleNode("//*[@" + SouT.ToString() + "='" + valid + "']").Attributes[TarT.ToString()].Value.ToString();
        }
        public string GetEnumValTableCol(JObject FormDataJson, string valid, string TableName = "Head", OAEnumPath SearchPath = OAEnumPath.Head, OAEnumTitle SouT = OAEnumTitle.ID, OAEnumTitle TarT = OAEnumTitle.ENUMVALUE)
        {
            string SearchVal = "";
            switch (SearchPath)
            {
                case OAEnumPath.Head:
                    SearchVal = FormDataJson["Head"][valid]["Value"].Value<string>();
                    break;
                case OAEnumPath.Sub:
                    SearchVal = FormDataJson["Sub"][TableName][valid]["Value"].Value<string>();
                    break;
                default:
                    SearchVal = FormDataJson["Head"][valid]["Value"].Value<string>();
                    break;
            }
            return EnumRoot.SelectSingleNode("//*[@" + SouT.ToString() + "='" + SearchVal + "']").Attributes[TarT.ToString()].Value.ToString();
        }
        
        public string GetDepVal(string valid, OADepTitle SouT = OADepTitle.id, OADepTitle TarT = OADepTitle.code)
        {
            foreach (JObject dps in Departments)
            {
                if (dps[SouT.ToString()].Value<string>().ToString() == valid)
                {
                    return dps[TarT.ToString()].Value<string>().ToString();
                }
            }
            return "";
        }
    }
}
