using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RunningWindowsList;
using System.Diagnostics;

namespace 增宇监控程序功能验证
{
    public partial class Form1 : Form
    {
        string ExeName = "呼叫中心";
        string ExeStartBAT = "D:\\Tools\\Bat\\KRCC.bat";
        int listboxIndex = 0;

        public Form1()
        {
            InitializeComponent();

            this.RunMSG.Text = ExeStartBAT;
            RCK.WorkerReportsProgress = true;
            RCK.RunWorkerAsync();
        }



        private void RCK_DoWork(object sender, DoWorkEventArgs e)
        {
            int waittime = 1000 * 60 * 5;
            while (publicdata.ifrun)
            {
                RCK.ReportProgress(1);
                System.Threading.Thread.Sleep(waittime);
            }
            e.Result = "Stoped";
        }

        private void RCK_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.RunMSG.Text = e.Result.ToString();
        }


        private void RCK_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.RunMSG.Text = "上次检测 " + DateTime.Now.ToString();

            if (TimerBox())
            {
                WindowsList.Items.Clear();
                listboxIndex = 0;
                RunBAT();
                LBTimerMSG.Items.Add("定时启动" + DateTime.Now.ToString());
            }
            else
            {
                CheckWindows();
            }
        }

        private void CheckWindows()
        {
            List<string> runninglist = RunningWindowsList.RWL.GetRunApplicationList((int)this.Handle);

            WindowsList.Items.Clear();
            listboxIndex = 0;

            Boolean ExeIsRunning = false;
            foreach (string kkk in runninglist)
            {
                WindowsList.Items.Insert(listboxIndex, kkk);
                if (kkk == ExeName)
                {
                    ExeIsRunning = true;
                }
                listboxIndex++;
            }
            if (!ExeIsRunning)
            {
                RunBAT();
            }

        }

        private void RunBAT()
        {
            this.LastRunTimeMSG.Text = "启动程序" + DateTime.Now.ToString();
            //Process ps = new Process();
            //ps.StartInfo.FileName = ExeStartBAT;
            //ps.Start();
            System.Diagnostics.Process.Start(ExeStartBAT);
            WindowsList.Items.Insert(listboxIndex, "|||||||||||||| RunningBAT  " + DateTime.Now.ToString());
        }

        private Boolean TimerBox()
        {
            if (DateTime.Now.Hour == 7 && DateTime.Now.Minute >= 49 && DateTime.Now.Minute <= 54)
            {
                return true;
            }
            else if (DateTime.Now.Hour == 11 && DateTime.Now.Minute >= 49 && DateTime.Now.Minute <= 54)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void BTNRestartNow_Click(object sender, EventArgs e)
        {
            WindowsList.Items.Clear();
            listboxIndex = 0;
            RunBAT();
        }
    }

    public class publicdata
    {
        public static Boolean ifrun = true;
    }
}
