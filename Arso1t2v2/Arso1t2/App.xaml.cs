using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Sm.Languages;
using System.Diagnostics;

namespace Arso1t2_LSX
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SysLib.RemotingClient.InitClientRemoteObject(ref Arso1t2_LSX.StartUp.SysObj);

            Arso1t2_LSX.StartUp.Menu_Id = e.Args.Count() > 0 ? e.Args[0].ToString() : "12.04.14";
            (new Arso1t2_LSX.StartUp()).Run();
            if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
                App.Current.Shutdown();
        }
    }
}
