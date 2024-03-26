using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Diagnostics;

namespace APCTPN1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Debug.WriteLine(string.Format("#1: {0}", DateTime.Now.ToString()));
            SysLib.RemotingClient.InitClientRemoteObject(ref StartUp.SysObj);
            System.Threading.Thread.CurrentThread.CurrentCulture = StartUp.SysObj.SysCultureInfo;
            if (StartUp.SysObj == null)
            {
                System.Windows.MessageBox.Show("Can not connect to server. Please login again", "Connection Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            Debug.WriteLine(string.Format("#1.1: {0}", DateTime.Now.ToString()));
            StartUp oStartup = new StartUp();
            Debug.WriteLine(string.Format("#1.2: {0}", DateTime.Now.ToString()));
            StartUp.Menu_Id = e.Args.Count() > 0 ? e.Args[0].ToString() : "05.01.11";
            StartUp.Editing_Stt_Rec = e.Args.Count() > 1 ? e.Args[1].ToString() : string.Empty;
            oStartup.Run();
        }
    }
}
