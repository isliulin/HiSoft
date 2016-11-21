using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RunEXE;
using System.Runtime.InteropServices;

namespace 服务器锁屏
{
    public partial class LockScreen : Form
    {
        public LockScreen()
        {
            InitializeComponent();
            LockTheScreen();
        }

        public static  RunEXE.RunEXEProcess REP = new RunEXE.RunEXEProcess();
        void LockTheScreen()
        {
            REP.LockScreen(); //锁屏
            System.Environment.Exit(0);  //退出当前程序
        }
    }
}
