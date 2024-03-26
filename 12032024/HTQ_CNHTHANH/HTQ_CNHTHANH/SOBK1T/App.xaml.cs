using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace HTQ_CNHTHANH
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SysLib.RemotingClient.InitClientRemoteObject(ref StartUp.SysObj);
            StartUp.Menu_Id = e.Args.Count() > 0 ? e.Args[0].ToString() : "05.02.21";
            StartUp oStartup = new StartUp();
            oStartup.Run(); 
        }
    }
}
