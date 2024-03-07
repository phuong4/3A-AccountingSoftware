using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace QLHD_BC03
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SysLib.RemotingClient.InitClientRemoteObject(ref StartUp.SysObj);
            StartUp oStartUp = new StartUp();
            StartUp.Menu_Id = e.Args.Count() > 0 ? e.Args[0].ToString() : "40.16.01";
            oStartUp.Run();
        }
    }
}
