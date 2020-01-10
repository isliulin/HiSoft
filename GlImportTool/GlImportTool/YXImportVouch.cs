using Chanjet.TP.OpenAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GlImportTool
{
    public partial class YXImportVouch : Form
    {
        //OpenAPI用参数
        string ApiUrl;
        OpenAPI TPlusAPI = null;
        Credentials credentials = null;

        //科目转换参数
        ReadData.OleDBHelper YXAccDB = new ReadData.OleDBHelper();
        DataTable AccInfo = new DataTable();

        public YXImportVouch(DataTable AccInformation , ReadData.OleDBHelper YXAccDB1)
        {
            InitializeComponent();

            if (AppMain.DemoState)
            {
                this.TBTServer.Text = "192.168.100.93";
                this.TBTUserName.Text = "demo";
                this.TBTUserPwd.Text = "";
                this.TBTZTNum.Text = "000002";
            }
            TBlogDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            TBlogDate.Text = "2014-01-31";
            AccInfo = AccInformation;

            YXAccDB = YXAccDB1;

            if (YXAccDB.conn.State==ConnectionState.Open)
            {
                getAccYearmonth();
            }
            else
            {
                MessageBox.Show("源数据链接错误");
            }
        }


        //测试链接
        private void BTNLinkTServer_Click(object sender, EventArgs e)
        {
            ApiUrl = string.Format("http://{0}/{1}", TBTServer.Text, "Tplus/api/v1/");
            credentials = new Credentials()
            {
                AppKey = "myAppKey",
                AppSecret = "myAppSecret"
            };

            TPlusAPI = new OpenAPI(ApiUrl, credentials);
            try
            {
                dynamic r = TPlusAPI.ConnectTest();
                MessageBox.Show("链接成功", r.status);
                BTNTLogin.Enabled = true;
                BTNLinkT.Enabled = false;
                TBTServer.Enabled = false;
            }
            catch (RestException ex)
            {
                MessageBox.Show("链接失败 \n" + ex.Code + "\n" + ex.Message, "链接失败");
            }
        }

        //用户登录
        private void BTNTLogin_Click(object sender, EventArgs e)
        {
            credentials = new Credentials()
            {
                AppKey = "myAppKey",
                AppSecret = "myAppSecret",
                UserName = TBTUserName.Text,
                Password = TBTUserPwd.Text,
                LoginDate = TBlogDate.Text,
                AccountNumber = TBTZTNum.Text
            };

            TPlusAPI = new OpenAPI(ApiUrl, credentials);
            try
            {
                dynamic r = TPlusAPI.GetToken();
                LogSuccess();
                RTBMsg.Text = r.access_token;
            }
            catch (RestException ex)
            {
                RTBMsg.Text = "\r\n Call:GetToken \r\n error:" + ex.Response.StatusCode + "  " + ex.Code + "  " + ex.Data + "  " + ex.Message + "\r\n" + ex.ResponseBody;
                if (ex.Code == "EXSM0004")
                {
                    if (MessageBox.Show(ex.Message, ex.Code, MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        try
                        {
                            dynamic r = this.TPlusAPI.ReLogin();
                            LogSuccess();
                            RTBMsg.Text = credentials.Access_Token;
                        }
                        catch (RestException Rex)
                        {
                            RTBMsg.Text = "\r\n Call:GetToken \r\n error:" + Rex.Response.StatusCode + "  " + Rex.Message + "\r\n" + Rex.ResponseBody;
                        }
                    }
                }
            }
        }

        //登陆成功
        private void LogSuccess()
        {
            this.BTNTLogin.Enabled = false;
            this.BTNTLogin.Visible = false;
            this.BTNTlogout.Enabled = true;
            this.BTNTlogout.Visible = true;
            this.BTNImportAcc.Enabled = true;


            this.BTNTlogout.Size = this.BTNTLogin.Size;
            this.BTNTlogout.Location = this.BTNTLogin.Location;

            this.TBTServer.Enabled = false;
            this.BTNLinkT.Enabled = false;

            this.TBTUserName.Enabled = false;
            this.TBTUserPwd.Enabled = false;
            this.TBTZTNum.Enabled = false;

            MessageBox.Show("登陆成功");
        }

        //注销登陆
        private void BTNTlogout_Click(object sender, EventArgs e)
        {
            try
            {
                dynamic r = TPlusAPI.Logout();
                this.BTNTLogin.Enabled = true;
                this.BTNTLogin.Visible = true;
                this.BTNTlogout.Enabled = false;
                this.BTNTlogout.Visible = false;
                this.BTNImportAcc.Enabled = false;

                this.TBTServer.Enabled = true;
                this.BTNLinkT.Enabled = true;

                this.TBTUserName.Enabled = true;
                this.TBTUserPwd.Enabled = true;
                this.TBTZTNum.Enabled = true;


                RTBMsg.Text = ("\r\n Call:Logout \r\n result:" + r);
            }
            catch (RestException ex)
            {
                RTBMsg.Text = ("\r\n Call:Logout \r\n error:" + ex.Message + "\r\n" + ex.ResponseBody);
            }
        }

        void getAccYearmonth()
        {
            string MinYM = "";
            object MinAccYMs = YXAccDB.ReadSingleValue("select min([年月]) from [金额账册] where [年月] not in (select [凭证年月] from [凭证] group by [凭证年月]) ");
            if (!string.IsNullOrEmpty(MinAccYMs as string))
            {
                MinYM = MinAccYMs as string;
                Console.WriteLine("MinYM: " + MinYM);
                this.BTNBuildQC.Enabled = true;
                //生成期初相关
            }
            /*
             * 期末相关
            string MaxYM = "";
            object MaxAccYMs= YXAccDB.ReadValue("select max([年月]) from [金额账册] where [年月] not in (select [凭证年月] from [凭证] group by [凭证年月]) ");
            if (!string.IsNullOrEmpty(MaxAccYMs as string))
            {
                MaxYM = MaxAccYMs as string;
            }
            */
            DataTable AccYMDT = YXAccDB.ReadDT("select [凭证年月] from [凭证] group by [凭证年月]");
            CBAccYM.DisplayMember = "凭证年月";
            CBAccYM.ValueMember = "凭证年月";

            this.CBAccYM.DataSource = AccYMDT;
            /*
            foreach (DataRow YM in AccYMDT.Rows)
            {
                this.CBAccYM.Items.Add(YM[0]);
            }
            */
            DGVMain.DataSource = AccInfo;
        }

        private void BTNLoadAcc_Click(object sender, EventArgs e)
        {
            //获取凭证信息
            string RunYM = CBAccYM.SelectedValue.ToString();
            DataTable VouchDT = YXAccDB.ReadDT("select [id],[凭证月编号],[凭证类别],[日期],[附件],[备注],[凭证年月] from [凭证] where [凭证年月]='"+ RunYM+ "' order by [凭证月编号]");
            this.PBImpVch.Maximum = VouchDT.Rows.Count;
            int index= 1;
            
            foreach (DataRow Vch in VouchDT.Rows)
            {
                
                this.TBExtCode.Text = Vch["凭证月编号"].ToString();
                this.TBDocType.Text = Vch["凭证类别"].ToString() + "=> 记账凭证";
                this.TBVouDate.Text=DateTime.Parse( Vch["日期"].ToString()).ToString("yyyy-MM-dd");
                this.TBAttVouNum.Text= Vch["附件"].ToString();
                this.TBVchMemo.Text= Vch["备注"].ToString();
                
                //格式化凭证信息
                string Json1 = "ExternalCode:\"" + this.TBExtCode.Text + "\",";
                Json1 += "DocType:{ Name:\"记账凭证\",Code:\"记\"}, ";
                Json1 += "VoucherDate:\""+ this.TBVouDate.Text + "\",";
                Json1 += "AttachedVoucherNum:\""+ this.TBAttVouNum.Text + "\",";
                if (!String.IsNullOrEmpty(this.TBVchMemo.Text))
                {
                    Json1 += "Memo:\"" + this.TBVchMemo.Text + "\",";
                }

                DataTable Entrys = GetVchEntrys(Vch["id"].ToString());

                

                object OSumRate= Entrys.Compute("sum(汇率)", null);
                string SSumRate = OSumRate.ToString();
                double SumRate =Double.Parse(SSumRate);
                Console.WriteLine(SumRate);
                bool ifOrig = (SumRate > 0);
                Console.WriteLine(SumRate > 0);
                Console.WriteLine(ifOrig);
                this.DGVMain.DataSource = Entrys;

                //格式化凭证分录
                Json1 += "Entrys:[";
                foreach (DataRow Entry in Entrys.Rows)
                {
                    Json1 += "{";
                    Json1 += "Summary: \""+ Entry["摘要"]+ "\"";
                    Json1 += ",Account:{Code:\""+ Entry["科目"] + "\"}";

                    if (!(String.IsNullOrEmpty(Entry["汇率"].ToString()) || Entry["汇率"].ToString() == "0"))
                    {
                        Json1 += ",Currency:{Code:\"USD\"}";
                    }
                    else 
                    {
                        Json1 += ",Currency:{Code:\"RMB\"}";
                    }
                    
                    if (!(String.IsNullOrEmpty(Entry["借方金额"].ToString())|| Entry["借方金额"].ToString()=="0"))
                    {
                        Json1 += ",AmountDr: \"" + Entry["借方金额"] + "\"";
                        //ifDr = true;
                    }

                    if (!(String.IsNullOrEmpty(Entry["贷方金额"].ToString()) || Entry["贷方金额"].ToString() == "0"))
                    {
                        Json1 += ",AmountCr: \"" + Entry["贷方金额"] + "\"";
                        //ifDr = false;
                    }

                    if (ifOrig)
                    {
                        if (!(String.IsNullOrEmpty(Entry["汇率"].ToString()) || Entry["汇率"].ToString() == "0"))
                        {
                            Json1 += ",ExchangeRate: \"" + Entry["汇率"].ToString() + "\"";
                        }
                        else
                        {
                            Json1 += ",ExchangeRate: \"1\"";
                        }
                    }
                   
                   if (!(String.IsNullOrEmpty(Entry["借方原币金额"].ToString()) || Entry["借方原币金额"].ToString() == "0"))
                    {
                        Json1 += ",OrigAmountDr: \"" + Entry["借方原币金额"] + "\"";
                        //ifDr = false;
                    }

                    if (!(String.IsNullOrEmpty(Entry["贷方原币金额"].ToString()) || Entry["贷方原币金额"].ToString() == "0"))
                    {
                        Json1 += ",OrigAmountCr: \"" + Entry["贷方原币金额"] + "\"";
                        //ifDr = false;
                    }

                    if (!(String.IsNullOrEmpty(Entry["借方数量"].ToString()) || Entry["借方数量"].ToString() == "0"))
                    {
                        Json1 += ",Quantity: \"" + Entry["借方数量"] + "\"";
                        //ifDr = false;
                    }

                    if (!(String.IsNullOrEmpty(Entry["贷方数量"].ToString()) || Entry["贷方数量"].ToString() == "0"))
                    {
                        Json1 += ",Quantity: \"" + Entry["借方数量"] + "\"";
                        //ifDr = false;
                    }

                    Json1 += "}";

                    if (Entry != Entrys.Rows[Entrys.Rows.Count-1])
                    {
                        Json1 += ",";
                    }
                }

                Json1 += "]";

                string Json = "{   dto:  { " +Json1+" } } ";
                RTBMsg.Text = Json;
                try
                {
                    dynamic r = TPlusAPI.Call("doc/Create", Json);
                }
                catch (RestException ex)
                {
                    MessageBox.Show(ex.Message+"\n" + this.TBExtCode.Text , ex.Code);
                }

                this.PBImpVch.Value = index;
                if (MessageBox.Show(this.TBExtCode.Text, "继续?", MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    break;
                }
                index++;
            }
        }

        //获取凭证分录
        DataTable GetVchEntrys(String VchID)
        {
            DataTable YXPZ = YXAccDB.ReadDT("select * from [凭证分录] where [单据id]="+VchID+" order by [序号]");
            DataTable Entrys = YXPZ.Clone();
            Entrys.Columns["科目"].DataType = Type.GetType("System.String");
            
            
            for (int i = 0; i < YXPZ.Rows.Count;i++)
            {
                DataRow Entry = Entrys.NewRow();

                for (int j = 0; j < YXPZ.Columns.Count; j++)
                {
                    object m = YXPZ.Rows[i][j];
                    
                    Entry[j] = YXPZ.Rows[i][j].ToString();
                }
                string oldid = YXPZ.Rows[i]["科目"].ToString();
                DataRow[] AccCheckInfo = AccInfo.Select("旧id='" + oldid + "'");
                Entry["科目"] = AccCheckInfo[0]["科目编码"];

                Entrys.Rows.Add(Entry);
            }
            return Entrys;
        }
    }
}
