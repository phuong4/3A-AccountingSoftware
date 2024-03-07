using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace AA_SmImexCt0
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SysLib.RemotingClient.InitClientRemoteObject(ref StartUp.SysObj);
            StartUp oStartup = new StartUp();
            StartUp.Menu_Id = e.Args.Count() > 0 ? e.Args[0].ToString() : "04.30.05";
            oStartup.Run();
        }
    }
}
