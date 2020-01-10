using Chanjet.TP.OpenAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GlImportTool.Tplus
{
    public partial class TPlusImportHandler : Form
    {
        //OpenAPI用参数
        string ApiUrl;
        OpenAPI TPlusAPI = null;
        Credentials credentials = null;

        /*源数据
        //T+相关信息
        DataTable Account = new DataTable();
        DataTable Vouch = new DataTable();
        DataTable VouchEntry = new DataTable();
        DataTable VouchEntrys = new DataTable();

        DataTable GLBegin = new DataTable();

        DataTable Units = new DataTable();

        //T+档案信息
        string[] AccountXLSTitle;
        string[] UnitsTitle;
        string[] GLBeginTitle;  
        string[] VouchTitle;
        */

        //程序相关参数
        AppMain MainForm;
        Form LastForm;
        bool DemoState = AppMain.DemoState;
        //TPlusImportProject TPIP;
        TPlusImportData TPID;


        //程序初始化
        public TPlusImportHandler(AppMain ParentForm,Form inForm ,TPlusImportData SourceTPID)
        {
            InitializeComponent();
            MainForm = ParentForm;
            LastForm = inForm;
            this.TBlogDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            this.TBTServer.Text = "192.168.1.254";
            
            demo();
            TPID = SourceTPID;
            this.CBAccYM.DataSource = TPID.AccYearMonth;
            CBAccYM.DisplayMember = "会计期间";
            CBAccYM.ValueMember = "期间日期";
        }

        //demo
        void demo()
        {
            if (DemoState)
            {
                this.TBTServer.Text = "192.168.100.93";
                this.TBTUserName.Text = "demo";
                this.TBTUserPwd.Text = "";
                this.TBTZTNum.Text = "2";
                this.TBlogDate.Text = "2014-01-31";
                this.TBTSqlUser.Text = "sa";
                this.TBTSqlPwd.Text = "Hc@3232327";
            }
        }

        //连接T+ OpenAPI
        private void BTNLinkT_Click(object sender, EventArgs e)
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

        //登陆T+ OpenAPI
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
                        }
                        catch (RestException Rex)
                        {
                            RTBMsg.Text = "\r\n Call:GetToken \r\n error:" + Rex.Response.StatusCode + "  " + Rex.Message + "\r\n" + Rex.ResponseBody;
                        }
                    }
                }
                else
                {
                    RTBMsg.Text = "\r\n Call:GetToken \r\n error:" + ex.Response.StatusCode + "  " + ex.Message + "\r\n" + ex.ResponseBody;
                }
            }
        }
        void LogSuccess()
        {
            this.BTNTLogin.Enabled = false;
            this.BTNTLogin.Visible = false;
            this.BTNTlogout.Enabled = true;
            this.BTNTlogout.Visible = true;
            this.BTNImpVouchS.Enabled = true;

            this.BTNTlogout.Size = this.BTNTLogin.Size;
            this.BTNTlogout.Location = this.BTNTLogin.Location;

            this.TBTServer.Enabled = false;
            this.BTNLinkT.Enabled = false;

            this.TBTUserName.Enabled = false;
            this.TBTUserPwd.Enabled = false;
            this.TBTZTNum.Enabled = false;

            MessageBox.Show("登陆成功");
        }

        //注销T+ OpenAPI
        private void BTNTlogout_Click(object sender, EventArgs e)
        {
            try
            {
                dynamic r = TPlusAPI.Logout();
                this.BTNTLogin.Enabled = true;
                this.BTNTLogin.Visible = true;
                this.BTNTlogout.Enabled = false;
                this.BTNTlogout.Visible = false;
                this.BTNImpVouchS.Enabled = false;

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

        //连接T+ SQLServer
        private void BTNLinkTSQL_Click(object sender, EventArgs e)
        {
            //
        }

        private void BTNBackup_Click(object sender, EventArgs e)
        {
            MainForm.OpenForm(this, LastForm, MainForm);
        }
    }

    public class TPlusImportData
    {
        //T+相关信息
        public DataTable Account = new DataTable();
        public DataTable Vouch = new DataTable();
        public DataTable VouchEntry = new DataTable();
        public DataTable VouchEntrys = new DataTable();
        public DataTable AccYearMonth;

        public DataTable GLBegin = new DataTable();

        public DataTable Units = new DataTable();

        //T+档案信息
        public string[] AccountXLSTitle;
        public string[] UnitsTitle;
        public string[] GLQCTitle;
        public string[] VouchTitle;
    }
}
