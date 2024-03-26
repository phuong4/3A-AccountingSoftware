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
using Infragistics.Windows.DataPresenter;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Threading;
using Sm.Windows.Controls;
using SmLib;

namespace Inctpng
{
    /// <summary>
    /// Interaction logic for FrmBrowse.xaml
    /// </summary>
    public partial class FrmView : Form
    {
        public bool isOk;
        public DataSet dsHdm;

        public FrmView(string filter)
        {
            InitializeComponent();
            SmLib.SysFunc.LoadIcon(this);
            Title = SysFunc.Cat_Dau("Danh mục hợp đồng");

            dsHdm = StartUp.GetHdm(filter);
            GrdBrowse.DataSource = dsHdm.Tables[0].DefaultView;
            GrdBrowseCt.DataSource = dsHdm.Tables[1].DefaultView;

            //PH
            string strPh = StartUp.M_LAN.Equals("V") ? StartUp.CommandInfo["Vbrowse1"].ToString().Split('|')[2] : StartUp.CommandInfo["Ebrowse1"].ToString().Split('|')[2];
            FieldLayout GrdLayoutPH = SysFunc.CreateFieldLayout(StartUp.SysObj, GrdBrowse, strPh, dsHdm.Tables[0]);
            GrdBrowse.FieldLayouts.Add(GrdLayoutPH);
            SysFunc.CreateSumFieldList(StartUp.SysObj, GrdBrowse, strPh);

            //CT
            string strCT = StartUp.M_LAN.Equals("V") ? StartUp.CommandInfo["Vbrowse1"].ToString().Split('|')[3]:StartUp.CommandInfo["Ebrowse1"].ToString().Split('|')[3];
            FieldLayout GrdLayoutCT = SysFunc.CreateFieldLayout(StartUp.SysObj, GrdBrowseCt, strCT, dsHdm.Tables[1]);
            GrdBrowseCt.FieldLayouts.Add(GrdLayoutCT);
            SysFunc.CreateSumFieldList(StartUp.SysObj, GrdBrowseCt, strCT);

        }

        void GrdBrowse_RecordActivated(object sender, Infragistics.Windows.DataPresenter.Events.RecordActivatedEventArgs e)
        {
            try
            {

                BasicGridView datagrd = sender as BasicGridView;
                if (datagrd.ActiveRecord != null)
                {
                    if (datagrd.ActiveRecord.Index >= 0 && datagrd.ActiveRecord.RecordType == RecordType.DataRecord)
                    {
                        dsHdm.Tables[1].DefaultView.RowFilter = "";
                        string sFilterCt = "1 =1 ";

                        sFilterCt += string.Format("{0} {1} = '{2}'", sFilterCt == "" ? "" : " and ", "stt_rec",
                            ((datagrd.ActiveRecord as DataRecord).DataItem as DataRowView)["stt_rec"].ToString());

                        dsHdm.Tables[1].DefaultView.RowFilter += sFilterCt;

                        GrdBrowseCt.DataSource = dsHdm.Tables[1].DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            isOk = false;
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                if (GrdBrowse.Records.Count > 0)
                    GrdBrowse.ActiveRecord = GrdBrowse.Records[0];
            }));
        }

        private void Form_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Escape))
                this.Close();
        }

        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            if (GrdBrowse.ActiveRecord != null && GrdBrowse.ActiveRecord.RecordType == RecordType.DataRecord)
            {
                Stt_rec = (GrdBrowse.ActiveRecord as DataRecord).Cells["stt_rec"].Value.ToString();
                isOk = true;
                Close();
            }
        }

        public string Stt_rec { get; set; }
        
    }
}
