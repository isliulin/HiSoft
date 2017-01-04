using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using ReadData;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CallListUpdateTool
{
    public partial class UpdateCheck : Form
    {
        ReadData.MSSQLHelper MSSQLH = new ReadData.MSSQLHelper();
        public UpdateCheck()
        {
            InitializeComponent();
            loadYearList();
        }

        void loadYearList()
        {
            MSSQLH.MSSQLConnSTR = "server=192.168.100.254;user=sa;pwd=Fjkdashg@3344";
            MSSQLH.InitialMSSQLDB();
            string YearSQL = "select iYear from [HHMobilephoneList].[dbo].[CallBill] group by iYear order by iYear Desc ";
            DataTable YearList= MSSQLH.MSSQLSelectTB(YearSQL);
            this.CBYear.DataSource = YearList;
            this.CBYear.DisplayMember = "iYear";
            this.CBYear.ValueMember = "iyear";
        }

        private void loadListCounter(string iYear)
        {
            string monthList = "select iMonth from [HHMobilephoneList].[dbo].[CallBill] where iYear = '"+ iYear + "'  group by iMonth  order by iMonth";
            DataTable Listmonth = MSSQLH.MSSQLSelectTB(monthList);

            string CounterSQL = " SELECT    [PhoneNumber] as 电话号码 ";
            foreach (DataRow Callmonth in Listmonth.Rows)
            {
                string Month = Callmonth["iMonth"].ToString();
                CounterSQL = CounterSQL + ",(select COUNT([BeginTime]) from (select * from  [HHMobilephoneList].[dbo].[CallBill]) as b where  b.[PhoneNumber]=a.[PhoneNumber] and b.iMonth=" + Month + " and b.iYear=" + iYear + " ) as [" + Month + "月]";
            }

            CounterSQL = CounterSQL + ",(select COUNT([BeginTime]) from (select * from  [HHMobilephoneList].[dbo].[CallBill]) as b where  b.[PhoneNumber]=a.[PhoneNumber] and b.iYear=" + iYear + " ) as [合计]";
            CounterSQL = CounterSQL + "  FROM (select * from [HHMobilephoneList].[dbo].[CallBill] where iYear='"+iYear+"' ) a group by [PhoneNumber]";
            DataTable CallListCounter = MSSQLH.MSSQLSelectTB(CounterSQL);
            DGVCounterShow.DataSource = CallListCounter;

            //MSSQLH.CloseConn();
        }


        private void BTNLoading_Click(object sender, EventArgs e)
        {
            loadListCounter(this.CBYear.SelectedValue.ToString());
            this.BTNLoading.Text = "刷新次数统计";
            this.LTitleNote.Text = this.CBYear.SelectedValue.ToString()+"年度通话次数统计，单位：次";
        }
    }
}
