using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ReadData;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Threading;

namespace CallListUpdateTool
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            ListPhone();
        }


        private void ListPhone()
        {
            //初始化详单电话号
            this.UserPhoneNumber.Items.Add(13065436307);
            this.UserPhoneNumber.Items.Add(13065436308);
            this.UserPhoneNumber.Items.Add(13065436315);
            this.UserPhoneNumber.Items.Add(13065436317);
            this.UserPhoneNumber.Items.Add(13065436319);
            this.UserPhoneNumber.Items.Add(13065436373);

            //孙维谦13065436307
            //刘  志13065436308
            //王  莹13065436315
            //邹茂辉13065436317
            //杨学业13065436319
            //徐云峰13065436373
        }


        private void BTNSelectFile_Click(object sender, EventArgs e)
        {
            //选择详单
            try
            {
                string UID = this.UserPhoneNumber.SelectedItem.ToString();

                OpenFileDialog GetXlsPath = new OpenFileDialog();
                GetXlsPath.Multiselect = false;
                //GetXlsPath.Filter = "Excel 97|*.xls";
                GetXlsPath.Title = "通话详单选择";

                if (GetXlsPath.ShowDialog() == DialogResult.OK)
                {
                    this.LabelFilePath.Text = GetXlsPath.FileName;

                    ReadData.ExcelHelper k = new ReadData.ExcelHelper();
                    k.Path = GetXlsPath.FileName;
                    k.InitialExcelHelper();

                    //模板标题定义
                    //序号	业务类型	通话起始时间	通话时长	呼叫类型	对方号码	本机通话地	对方归属地	通话类型	通话费	其他费	小计
                    //序号	业务类型	通话起始时间	通话时长	呼叫类型	对方号码	本机通话地	对方归属地	通话类型	通话费	其他费	小计


                    string DataColName = "'" + UID + "' as 手机号,序号,year(通话起始时间) as 年,month(通话起始时间) as 月,  业务类型,	通话起始时间,	通话时长,	呼叫类型,	对方号码,	本机通话地,对方归属地,	通话类型,	通话费,	其他费,	小计";

                    //DataSet DSCalList = k.ExcelToDS(this.LabelFilePath.Text);
                    try
                    {                    
                        DataTable DSCalList = k.ReadExcelToDT(DataColName,"");
                        SGVListShow.DataSource = DSCalList;
                        SGVListShow.AllowUserToAddRows = false;

                        this.UserPhoneNumber.Enabled = false;
                        this.LRumMessage.Text = "详单加载完成";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Excel文件读取错误，请确认模板完整性！！\n\n *************************** \n\n *请将Excel文件打开并另存为xlsx格式，通过新文件导入。 \n\n *************************** \n\n 详细错误信息:\n" + ex.Message + "\n" + DataColName, "文件读取失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch
            {
                MessageBox.Show("请先选择记录对应手机号", "Error", MessageBoxButtons.OK);
            }
        }


        private void LLTMPDownload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //下载模板
            System.Diagnostics.Process.Start("iexplore.exe", "http://s4.hzcrj.com:8000/f/461f37bf85/");
        }


        private void BTNReset_Click(object sender, EventArgs e)
        {
            //清空已选
            if (MessageBox.Show("确认清空已选数据么？", "初始化确认", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                SGVListShow.DataSource = null;
                this.UserPhoneNumber.Enabled = true;
                this.LRumMessage.Text = "详单列表已经清空";
            }
        }


        Thread MyThread;
        public delegate void MyInvoke();


        private void BTNUpload_Click(object sender, EventArgs e)
        {
            //上传数据

            MyThread = new Thread(Send);
            MyThread.IsBackground = true;
            MyThread.Start();
        }

        protected void Send()
        {
            MyInvoke invoke = new MyInvoke(UpdateBill);
            this.BeginInvoke(invoke);
        }

        private void UpdateBill()
        {
            try
            {
                int RowsCount = this.SGVListShow.RowCount;
                int RowsRunned = 0;
                int RowsUpload = 0;
                int RowsUnLoad = 0;
                int RowsExistd = 0;
                SqlConnection Conn = new SqlConnection("server=192.168.100.254;user=sa;pwd=Fjkdashg@3344");
                Conn.Open();
                DateTime ClickBTNTime = DateTime.Now;
                String BillTS = ClickBTNTime.ToFileTimeUtc().ToString();

                foreach (DataGridViewRow row in this.SGVListShow.Rows)
                {
                    //Random ran = new Random();
                    //int RandKey = ran.Next(0, 9);
                    RowsRunned++;

                    this.PBUpdateList.Value = (int)RowsRunned * 100 / RowsCount;

                    try
                    {
                        string IFDoSql = "";
                        if (true)
                        {
                            IFDoSql = "if not exists(select * from [HHMobilephoneList].[dbo].[CallBill] where ";
                            IFDoSql = IFDoSql + "  [iYear]= '" + row.Cells[2].Value.ToString() + "' ";
                            IFDoSql = IFDoSql + " and [iMonth]= '" + row.Cells[3].Value.ToString() + "' ";
                            IFDoSql = IFDoSql + " and [BeginTime]= '" + row.Cells[5].Value.ToString() + "' ";
                            IFDoSql = IFDoSql + " and [CallTime]= '" + row.Cells[6].Value.ToString() + "' ";
                            IFDoSql = IFDoSql + " and [DialogType]= '" + row.Cells[7].Value.ToString() + "' ";
                            IFDoSql = IFDoSql + " and [CallTo]= '" + row.Cells[8].Value.ToString() + "' )";
                        }


                        string DoSql = IFDoSql + "insert into [HHMobilephoneList].[dbo].[CallBill] (";

                        //SQL字段
                        DoSql = DoSql + "[TS]";                  //时间戳
                        DoSql = DoSql + ",[PhoneNumber]";        //手机号
                        DoSql = DoSql + ",[BillID]";             //序号
                        DoSql = DoSql + ",[iYear]";              //年
                        DoSql = DoSql + ",[iMonth]";             //月
                        DoSql = DoSql + ",[BillType]";           //业务类型
                        DoSql = DoSql + ",[BeginTime]";          //通话起始时间
                        DoSql = DoSql + ",[CallTime]";           //通话时长
                        DoSql = DoSql + ",[DialogType]";         //呼叫类型
                        DoSql = DoSql + ",[CallTo]";             //对方号码
                        DoSql = DoSql + ",[CallPlacePhone]";     //通话地点
                        DoSql = DoSql + ",[CallPlaceTo]";        //对方归属地
                        DoSql = DoSql + ",[CallType]";           //通话类型
                        DoSql = DoSql + ",[CallPrice]";          //通话费
                        DoSql = DoSql + ",[OtrPrice]";           //其他费
                        DoSql = DoSql + ",[AllPrice]";           //小计
                        DoSql = DoSql + ",[UpdateTime]";         //执行时间
                        DoSql = DoSql + ",[CallTimmer]";         //通话时间

                        DoSql = DoSql + " )";

                        //SQL值获取
                        DoSql = DoSql + " values('" + BillTS + "'";                            //时间戳            13065436319
                        DoSql = DoSql + ",'" + row.Cells[0].Value.ToString() + "'";            //手机号            13065436319
                        DoSql = DoSql + ",'" + row.Cells[1].Value.ToString() + "'";            //序号                 1
                        DoSql = DoSql + ",'" + row.Cells[2].Value.ToString() + "'";            //年                2016
                        DoSql = DoSql + ",'" + row.Cells[3].Value.ToString() + "'";            //月                  4
                        DoSql = DoSql + ",'" + row.Cells[4].Value.ToString() + "'";            //业务类型           语音电话
                        DoSql = DoSql + ",'" + row.Cells[5].Value.ToString() + "'";            //通话起始时间       2016 - 04 - 30 18:55:28.000
                        DoSql = DoSql + ",'" + row.Cells[6].Value.ToString() + "'";            //通话时长           1分37秒
                        DoSql = DoSql + ",'" + row.Cells[7].Value.ToString() + "'";            //呼叫类型           被叫
                        DoSql = DoSql + ",'" + row.Cells[8].Value.ToString() + "'";            //对方号码           04123215265
                        DoSql = DoSql + ",'" + row.Cells[9].Value.ToString() + "'";            //通话地点           辽宁鞍山
                        DoSql = DoSql + ",'" + row.Cells[10].Value.ToString() + "'";           //对方归属           辽宁鞍山
                        DoSql = DoSql + ",'" + row.Cells[11].Value.ToString() + "'";           //通话类型           本地通话
                        DoSql = DoSql + "," + row.Cells[12].Value.ToString();                  //通话费              0.0000 
                        DoSql = DoSql + "," + row.Cells[13].Value.ToString();                  //其他费              0.0000
                        DoSql = DoSql + "," + row.Cells[14].Value.ToString();                  //小计                0.0000
                        DoSql = DoSql + ",'" + DateTime.Now.ToString() + "'";                  //执行时间

                        //计算通话时间 
                        DoSql = DoSql + "," + "case when charindex('分','" + row.Cells[6].Value.ToString() + "')=0    ";
                        DoSql = DoSql + "then  REPLACE(REPLACE('00:00:'+'" + row.Cells[6].Value.ToString() + "','分',':'),'秒','')";

                        DoSql = DoSql + " when charindex('时','" + row.Cells[6].Value.ToString() + "')=0    ";
                        DoSql = DoSql + " then  REPLACE(REPLACE('00:'+'" + row.Cells[6].Value.ToString() + "','分',':'),'秒','')";

                        DoSql = DoSql + " else replace(REPLACE(REPLACE('" + row.Cells[6].Value.ToString() + "', '秒', ''), '分', ':'), '时', ':')    end";

                        DoSql = DoSql + ")";

                        //PhoneNumber	BillID	iYear	iMonth	TheType	BeginTime	CallTime	CallType	CallTo	CallPlace	Type2	CallPrice	OtrPrice	AllPrice
                        //13065436319 1   2016    4   语音电话    2016 - 04 - 30 18:55:28.000 1分37秒 被叫  04123215265 辽宁鞍山 本地通话    0.0000  0.0000  0.0000
                        /*
                        if (MessageBox.Show(DoSql, "上传数据?", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                        {
                            break;
                        }
                        */

                        Thread.Sleep(5);

                        SqlCommand Comm = new SqlCommand(DoSql, Conn);
                        Comm.CommandTimeout = 20;
                        int DoCheck = Comm.ExecuteNonQuery();   //insert  update delete
                        if (DoCheck == 1)
                        {
                            RowsUpload++;
                        }
                        else
                        {
                            MessageBox.Show("[" + row.Cells[1].Value.ToString() + "]数据已经存在，未上传 \n", "数据上传失败", MessageBoxButtons.OK);
                            RowsExistd++;
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("[" + row.Cells[1].Value.ToString() + "]数据上传失败， \n" + ex.Message, "数据上传失败", MessageBoxButtons.OK);
                        RowsUnLoad++;
                    }

                    if (RowsRunned % 4 == 1)
                    {

                        this.LRumMessage.Text = null;
                    }

                    this.LRumMessage.Text = "已执行" + RowsRunned + "/" + RowsCount + "| 其中成功 " + RowsUpload + " 失败 " + RowsUnLoad + " 已存在记录 " + RowsExistd;
                    Thread.Sleep(10);
                }
                string OverSql = "insert into [HHMobilephoneList].[dbo].[BillTS] ([BillTS],[PhoneNumber],[iYear],[iMonth],[UpdateTime],[OtrTXT]) values('" + BillTS + "','";
                OverSql = OverSql + this.SGVListShow.Rows[1].Cells[0].Value.ToString() + "','";       //手机号  
                OverSql = OverSql + this.SGVListShow.Rows[1].Cells[2].Value.ToString() + "','";       //年
                OverSql = OverSql + this.SGVListShow.Rows[1].Cells[3].Value.ToString() + "','";       //月
                OverSql = OverSql + ClickBTNTime.ToString() + "','";                                  //执行时间
                OverSql = OverSql + this.LRumMessage.Text.ToString() + "')";                          //备注


                SqlCommand CommOver = new SqlCommand(OverSql, Conn);
                CommOver.CommandTimeout = 20;
                CommOver.ExecuteNonQuery();   //insert  update delete

                Conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("服务器连接失败，请检查网络设置 \n" + ex.Message, "服务器链接失败", MessageBoxButtons.OK);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form Counter = new UpdateCheck();
            Counter.Show();
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
            System.Diagnostics.Process.Start("iexplore.exe", "http://www.microsoft.com/zh-CN/download/details.aspx?id=13255");
        }
    }
}
