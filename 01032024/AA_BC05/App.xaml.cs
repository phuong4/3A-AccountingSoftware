using Sm.Windows.Controls;
using SysLib;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace AA_BC05
{
	public partial class App : Application
	{
        //public App()
        //{
        //}

        private void Application_Startup(object sender, StartupEventArgs e)
		{
            //RemotingClient.InitClientRemoteObject(ref StartupBase.SysObj);
            //StartupBase.Menu_Id = (e.Args.Count<string>() > 0 ? e.Args[0].ToString() : "08.30.07");
            SysLib.RemotingClient.InitClientRemoteObject(ref StartUp.SysObj);
            StartUp.Menu_Id = e.Args.Count() > 0 ? e.Args[0].ToString() : "08.30.07";
            StartUp oStartup = new StartUp();
            oStartup.Run();
        }
	}
}