using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace AAA_QLHD3A
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SysLib.RemotingClient.InitClientRemoteObject(ref StartUp.SysObj);
            StartUp.Menu_Id = e.Args.Count() > 0 ? e.Args[0].ToString() : "40.06.00";
            StartUp oStartup = new StartUp();
            oStartup.Run();

        }
    }
}
