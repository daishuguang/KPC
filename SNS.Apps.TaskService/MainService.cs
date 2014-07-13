using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Apps.TaskService
{
    public partial class MainService : ServiceBase
    {
        public MainService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
			HostServiceManager.Start();
			TaskServiceManager.Start();
        }

        protected override void OnStop()
        {
			HostServiceManager.Stop();
			TaskServiceManager.Stop();
        }
    }
}
