using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Sm.Languages;
using System.Data.OleDb;

namespace AA_SMIMEXCT
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SysLib.RemotingClient.InitClientRemoteObject(ref AA_SMIMEXCT.StartUp.SysObj);

            AA_SMIMEXCT.StartUp.Menu_Id = e.Args.Count() > 0 ? e.Args[0].ToString() : "01.03.20";
            (new AA_SMIMEXCT.StartUp()).Run();
        }
    }
}
