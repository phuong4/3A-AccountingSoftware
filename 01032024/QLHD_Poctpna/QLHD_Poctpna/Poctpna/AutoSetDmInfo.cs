using Sm.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace QLHD_Poctpna
{
    public class AutoSetDmInfo
    {
        public Dictionary<string, object> FieldValues;
        public string WindowTypeName;
        public string NameSpace;
        public long Timeout;
        public int Interval;
        public void RunThreadCheck()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int foundCount = 0;
            while (sw.ElapsedMilliseconds < Timeout)
            {
                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    foreach (Window w in Application.Current.Windows)
                    {
                        Type t = w.GetType();
                        if (t.FullName == (string.IsNullOrEmpty(NameSpace) ? "" : NameSpace + ".") + WindowTypeName)
                        {
                            foundCount++;
                            if (foundCount == 1)
                                continue;
                            if (!w.IsLoaded)
                                continue;


                            foreach (KeyValuePair<string, object> kvp in FieldValues)
                            {
                                Object obj = w.FindName(kvp.Key);
                                if (obj is TextBoxAutoComplete)
                                    (obj as TextBoxAutoComplete).SetValue(AutoCompleteTextBox.TextProperty, kvp.Value.ToString());

                                if (obj is TextBox)
                                    (obj as TextBox).SetValue(TextBox.TextProperty, kvp.Value.ToString());

                            }
                            Timeout = 0;
                            return;
                        }
                        Debug.WriteLine(w.GetType().FullName + " " + w.Name + ":" + w.IsActive.ToString());
                    }
                }));

                Thread.Sleep(Interval);
            }
        }
    }
}
