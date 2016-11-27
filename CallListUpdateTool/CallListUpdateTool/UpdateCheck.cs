using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using ReadDate;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CallListUpdateTool
{
    public partial class UpdateCheck : Form
    {
        public UpdateCheck()
        {
            InitializeComponent();
        }

        private void loadListCounter()
        {
            ReadDate.MSSQLHelper k = new ReadDate.MSSQLHelper();
            k.MSSQLConnSTR = "server=192.168.100.254;user=sa;pwd=Fjkdashg@3344";
            k.InitialMSSQLDB();
            
            string monthList = "select iYear, iMonth from [HHMobilephoneList].[dbo].[CallBill] where iYear = (select MAX(iYear) from [HHMobilephoneList].[dbo].[CallBill]) group by iYear,iMonth";
            DataTable Listmonth = k.MSSQLSelectTB(monthList);
            string Year = Listmonth.Rows[0]["iYear"].ToString();

            string CounterSQL = " SELECT    [PhoneNumber] as 电话号码 ";
            foreach (DataRow Callmonth in Listmonth.Rows)
            {
                
                string Month = Callmonth["iMonth"].ToString();
                CounterSQL = CounterSQL + ",(select COUNT([BeginTime]) from (select * from  [HHMobilephoneList].[dbo].[CallBill]) as b where  b.[PhoneNumber]=a.[PhoneNumber] and b.iMonth="+ Month + " and b.iYear="+ Year + " ) as [" + Month + "月]";
            }

            CounterSQL = CounterSQL + ",(select COUNT([BeginTime]) from (select * from  [HHMobilephoneList].[dbo].[CallBill]) as b where  b.[PhoneNumber]=a.[PhoneNumber] and b.iYear=" + Year + " ) as [合计]";

            CounterSQL =CounterSQL+ "  FROM (select * from [HHMobilephoneList].[dbo].[CallBill]) a group by [PhoneNumber]";
            DataTable CallListCounter = k.MSSQLSelectTB(CounterSQL);
            DGVCounterShow.DataSource = CallListCounter;

            k.CloseConn();
        }


        private void BTNLoading_Click(object sender, EventArgs e)
        {
            loadListCounter();
            this.BTNLoading.Text = "刷新";
        }
    }
}
