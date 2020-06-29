using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Services.Description;

namespace PhoneWatcher
{
    public partial class MainPhoneService : ServiceBase
    {
        public MainPhoneService()
        {
            InitializeComponent();
        }

        WebServiceHost service_host;
        protected override void OnStart(string[] args)
        {

            OSWork.InterOP.ShowMessageBox("服务启动中", "OnStart");
            try
            {
                Uri server_set = new Uri("http://127.0.0.1:15963/");

                service_host = new WebServiceHost(null, server_set);
                {
                    WebHttpBinding binding = new WebHttpBinding
                    {
                        TransferMode = TransferMode.Buffered,
                        MaxBufferSize = 2147483647,
                        MaxReceivedMessageSize = 2147483647,
                        Security = { Mode = WebHttpSecurityMode.None }
                    };

                    string theArgs ="";
                    if (args.Length > 1)
                    {
                        foreach (string arg in args)
                        {
                            theArgs += arg + ",";
                        }
                    }
                    
                    

                    service_host.Opened += delegate { OSWork.InterOP.ShowMessageBox(DateTime.Now.ToString()+"\n"+ theArgs, "OnStart 服务正在运行"); };
                    service_host.Open();
                }
            }
            catch (Exception ex)
            {
                OSWork.InterOP.ShowMessageBox(DateTime.Now.ToString()+"\n"+ex.Message.ToString(), "OnStart 服务启动异常");
            }
        }

        protected override void OnStop()
        {
            OSWork.InterOP.ShowMessageBox("服务停止", "OnStop");
        }
    }



}
