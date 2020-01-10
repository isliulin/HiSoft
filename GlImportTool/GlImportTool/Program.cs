using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GlImportTool
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        /// 


        static Boolean AllowRun = false;

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length == 3)
            {
                Application.Run(new AppMain(args));
            }
            else
            {
                if (AppMain.DemoState)
                {
                    Application.Run(new AppMain(new string[3]));
                }
                else
                {
                    string msg = "";
                    if (args.Length > 0)
                    {
                        foreach (string k in args)
                        {
                            msg += " " + k;
                        }
                    }
                    MessageBox.Show("程序错误，请重试 \n" + msg);
                    Application.ExitThread();
                    Application.Exit();
                }
                
            }

        }
    }
}
