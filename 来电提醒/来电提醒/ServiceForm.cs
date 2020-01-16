using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhoneInfo
{
    public partial class ServiceForm : Form
    {
        [DllImport("AD130Device.dll", EntryPoint = "AD130_InitDevice")]
        public static extern int AD130_InitDevice(int hWnd);

        public ServiceForm()
        {
            InitializeComponent();
        }

        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case DeviceVal.WM_AD130MSG:
                    EventLog.WriteEntry("PhoneService", "Phone MSG"+m.WParam.ToString(),EventLogEntryType.Information);
                    //OnDeviceMsg(m.WParam, m.LParam);
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        private void onLoaded(object sender, EventArgs e)
        {
            /*
            if (AD130_InitDevice(this.Handle.ToInt32()) == 0)
            {
                EventLog.WriteEntry(PhoneWatcherService.ServName, "AD130 Initialed", EventLogEntryType.Information);
                return;
            }
            else
            {
                EventLog.WriteEntry(PhoneWatcherService.ServName, "AD130 Can't Initialed\n"+DateTime.Now.ToShortTimeString(), EventLogEntryType.Information);
            }
            */
        }
    }
}
