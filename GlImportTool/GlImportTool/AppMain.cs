using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GlImportTool
{
    public partial class AppMain : Form
    {
        private int childFormNumber = 0;

        public  static Boolean DemoState = true;

        public static string AppName;
        public static string UserName;
        public static string LogMAC;
        public static string LogDate;

        ReadData.SecStrHelper SSH = new ReadData.SecStrHelper();

        public AppMain(string[] args)
        {
            InitializeComponent();
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            if (!DemoState)
            {
                if (args.Length == 3)
                {
                    AppName = args[0];
                    UserName = args[1];

                    string Lic = SSH.DESLite(false, args[2]);
                    string[] LicInfo = Lic.Split(',');
                    LogMAC = LicInfo[0];
                    LogDate = LicInfo[1];
                    ReadData.HardwareHelper HWH = new ReadData.HardwareHelper();
                    if (LogDate != DateTime.Now.ToLongDateString())
                    {
                        MessageBox.Show("登陆超时，请重新登陆");
                        ProClose();
                    }
                    else if (LogMAC != HWH.GetMacAddress() || AppName!= Assembly.GetExecutingAssembly().GetName().Name.ToString())
                    {
                        MessageBox.Show("登录信息错误，请重新登陆");
                        ProClose();
                    }
                }
                else
                {
                    MessageBox.Show("程序错误，请重新启动或联系管理员！");
                    ProClose();
                }
            }
        }

        void ProClose()
        {
            if (!DemoState)
            {
                Application.Exit();
                this.Close();
            }
        }


        public void OpenForm(Form Last,Form New, Form MainForm)
        {
            if(Last != null)
                {
                Last.Hide();
                //Last.Close();

            }
            
            
            New.MdiParent = MainForm;
            New.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            New.ControlBox = false;
            New.Show();
            
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            OpenForm(null, new YXtoYonyou(this), this);
            /*
            Form childForm = new YXtoYonyou(this);
            childForm.MdiParent = this;
            childForm.Text = "窗口 " + childFormNumber++;
            childForm.Show();
            */
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog.FileName;
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }
    }
}
