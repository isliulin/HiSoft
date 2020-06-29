using Fleck;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;

namespace PhoneInfo
{
    public partial class PhoneWatcherService : ServiceBase
    {
        //public static IDictionary<string, IWebSocketConnection> Client_Sockets = new Dictionary<string, IWebSocketConnection>();
        public static List<Phone> PhoneStateList = new List<Phone>();

        [DllImport("AD130Device.dll", EntryPoint = "AD130_InitDevice")]
        public static extern int AD130_InitDevice(int hWnd);

        public static string ServName;
        public PhoneWatcherService()
        {
            //InitializeComponent();
            ServName = ServiceName;

            if (!EventLog.SourceExists(ServiceName))
            {
                EventLog.CreateEventSource(ServiceName, "Application");
            }
        }

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry(ServiceName, ServiceName+" On Start", EventLogEntryType.Information);
            PhoneStateList.Clear();
            for (int i = 0; i < DeviceVal.MAX_DEVICE; i++)
            {
                PhoneStateList.Add(new Phone(i));
            }

            
            //TestMessageFilter ms = new TestMessageFilter();

            try
            {
                Form MsgF = new MessageForm();
                Application.Run(MsgF);
                if (AD130_InitDevice(MsgF.Handle.ToInt32()) == 0)
                {
                    EventLog.WriteEntry(ServiceName, "AD130 Initialed", EventLogEntryType.Information);
                    return;
                }
                else
                {
                    EventLog.WriteEntry(ServiceName, "AD130 Can't Initialed", EventLogEntryType.Information);
                }
                
            }
            catch(Exception ex)
            {
                EventLog.WriteEntry(ServiceName, "AD130 Error \n" + ex.Message, EventLogEntryType.Error);
            }
            EventLog.WriteEntry(ServiceName, ServiceName + " On Started", EventLogEntryType.Information);

            
            //MsgF.Handle
            //this.ServiceHandle.ToPointer();
        }

        //protected override mes

        protected override void OnCustomCommand(int command)
        {
            //base.OnCustomCommand(command);
            EventLog.WriteEntry(ServiceName, "OnCustomCommand \n" + command.ToString(), EventLogEntryType.Error);
        }

        protected override void OnStop()
        {
        }


    }
    /*
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public class TestMessageFilter : IMessageFilter
    {
        public bool PreFilterMessage(ref Message m)
        {
            // Blocks all the messages relating to the left mouse button.
            if (m.Msg >= 513 && m.Msg <= 515)
            {
                Console.WriteLine("Processing the messages : " + m.Msg);
                return true;
            }
            return false;
        }
    }
    */
    public class  MessageForm  : System.Windows.Forms.Form
    {
        public MessageForm()
        { }

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            // Listen for operating system messages.
            switch (m.Msg)
            {
                // The WM_ACTIVATEAPP message occurs when the application
                // becomes the active application or becomes inactive.
                case DeviceVal.WM_AD130MSG:
                    EventLog.WriteEntry(PhoneWatcherService.ServName, PhoneWatcherService.ServName + " On Message \n"+m.WParam+"\n"+m.LParam, EventLogEntryType.Information);
                    // The WParam value identifies what is occurring.
                    //appActive = (((int)m.WParam != 0));

                    // Invalidate to get new text painted.
                    this.Invalidate();

                    break;
            }
            base.WndProc(ref m);
        }
        //IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)

    }
 

}
