using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using SmLib.SM.FormBrowse;
using System.Data.SqlClient;
using Sm.Windows.Controls;
using Infragistics.Windows.DataPresenter;
using SmDefine;
using System.Windows.Threading;

namespace COSXLSX.COLSX
{
    /// <summary>
    /// Interaction logic for ChonBPHT.xaml
    /// </summary>
    public partial class ChonBPHT : Form
    {

        static public bool flag = false;
        public ChonBPHT()
        {
            SmLib.SysFunc.LoadIcon(this);
            InitializeComponent();
        }

     
        private void SelectEntry()
        {
            DataRecord rec = GrdBPHT.ActiveRecord as DataRecord;
            if (rec == null || rec.RecordType != RecordType.DataRecord)
                return;

            Cell cell = rec.Cells["tag"];
            if (cell.Value == null || cell.Value is DBNull)
                cell.Value = false;
            else
                cell.Value = !((bool)cell.Value);
        }

        private void SelectAll(bool tag)
        {
            DataRecord rec;
            for (int i = 0; i < GrdBPHT.Records.Count; i++)
            {
                rec = GrdBPHT.Records[i] as DataRecord;
                rec.Cells["tag"].Value = tag;
                rec.Cells["tag"].EndEditMode();
                rec.Update();
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                flag = true;
                this.Close();
                return;
            }
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (Keyboard.IsKeyDown(Key.A))
                    SelectAll(true);
                if (Keyboard.IsKeyDown(Key.U))
                    SelectAll(false);
            }


            if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.Space)
                SelectEntry();
               
        }


        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
           
            GrdBPHT.DataSource = FrmCapNhat.listBpht.DefaultView;
            if (GrdBPHT.Records.Count > 0)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    GrdBPHT.ActiveRecord = (GrdBPHT.Records[0] as DataRecord);
                    GrdBPHT.Focus();
                }));
            }
            
        }

        private void Form_PreviewKeyUp(object sender, KeyEventArgs e)
        {
           

         
        }
    }
}
