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
using Sm.Windows.Controls;
using Infragistics.Windows.DataPresenter;
using System.Windows.Interop;

namespace Inctpxd
{
    /// <summary>
    /// Interaction logic for FrmPrintGlctpk1.xaml
    /// </summary>
    public partial class FrmPrintInctpxd : Form
    {
        public DataSet DsPrint = new DataSet();
        public FrmPrintInctpxd()
        {
            InitializeComponent();
            SmLib.SysFunc.LoadIcon(this); 
            GridSearch.LocalSysObj = StartUp.SysObj;
            GridSearch.ReportGroupName = StartUp.CommandInfo["rep_file"].ToString();
            if (BindingSysObj.GetOption("M_LAN").ToString().Equals("V"))
            {
                btnExport.Content = BindingSysObj.GetSysVar("M_EXPORT_SIGN").ToString();
            }
            else
            {
                btnExport.Content = BindingSysObj.GetSysVar2("M_EXPORT_SIGN").ToString();
            }
        }
        #region Form_Loaded
        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            GridSearch.DSource = DsPrint;
            //txtctu0.Value = 0;
            //txtlien.Value = StartUp.DmctInfo["so_lien"];
            DataTable tbIn = StartUp.GetPhIn();
            if (tbIn.Rows.Count == 0)
            {
                DataRow dr = tbIn.NewRow();
                dr["ma_ct"] = StartUp.Ma_ct;
                dr["stt_rec"] = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim();
                dr["so01"] = 0;//StartUp.so_ct0;
                dr["so02"] = StartUp.DmctInfo["so_lien"] == DBNull.Value ? 1 : StartUp.DmctInfo["so_lien"];

                tbIn.Rows.Add(dr);

            }

            this.DataContext = tbIn;
        }
        #endregion

        #region btnin_Click
        private void btnin_Click(object sender, RoutedEventArgs e)
        {
            if (txtlien.Value != null)
            {
                int so_lien = 1;
                int so_lan_in = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(txtlien.Text) / Convert.ToDouble((GridSearch.XGReport.ActiveRecord as DataRecord).Cells["so_lien"].Value)));
                while (so_lien <= so_lan_in)
                {
                    DsPrint.Tables["TablePH"].DefaultView[0]["so_lien"] = so_lan_in;
                    GridSearch.V_In(1);
                    so_lien++;
                }
                StartUp.SetPhIn(this.DataContext as DataTable);
            }
        	this.Close();
		}
        #endregion

        #region btnin_lt_Click
        private void btnin_lt_Click(object sender, RoutedEventArgs e)
        {
            if (StartUp.SysObj.GetOption("M_IN_HOI_CK").ToString() == "1")
            {
                if (ExMessageBox.Show( 1075,StartUp.SysObj, "Có chắc chắn in tất cả các chứng từ đã được lọc?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    int so_lien;
                    if (txtlien.Value != null)
                    {
                        int iRowTmp = FrmInctpxd.iRow;

                        so_lien = 1;
                        int so_lan_in = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(txtlien.Text) / Convert.ToDouble((GridSearch.XGReport.ActiveRecord as DataRecord).Cells["so_lien"].Value)));
                        while (so_lien <= so_lan_in)
                        {
                            for (int i = 1; i < DsPrint.Tables[0].Rows.Count; i++)
                            {

                                DsPrint.Tables["TablePH"].DefaultView.RowFilter = "stt_rec= '" + DsPrint.Tables[0].Rows[i]["stt_rec"].ToString() + "'";
                                DsPrint.Tables["TableCT"].DefaultView.RowFilter = "stt_rec= '" + DsPrint.Tables[0].Rows[i]["stt_rec"].ToString() + "'";
                                DsPrint.Tables["TableCT"].DefaultView.Sort = "stt_rec0";
                                DsPrint.Tables["TablePH"].DefaultView[0]["so_lien"] = so_lan_in;
                                GridSearch.V_In(1);                                
                            }
                            so_lien++;
                        }
                        DsPrint.Tables["TablePH"].DefaultView.RowFilter = "stt_rec= '" + DsPrint.Tables["TablePH"].Rows[iRowTmp]["stt_rec"].ToString() + "'";
                        DsPrint.Tables["TableCT"].DefaultView.RowFilter = "stt_rec= '" + DsPrint.Tables["TablePH"].Rows[iRowTmp]["stt_rec"].ToString() + "'";
                        DsPrint.Tables["TableCT"].DefaultView.Sort = "stt_rec0";
                        StartUp.SetPhIn(this.DataContext as DataTable);
                    }
                }
            }
            this.Close();
        }
        #endregion

        #region btnxem_Click
        private void btnxem_Click(object sender, RoutedEventArgs e)
        {
            StartUp.SetPhIn(this.DataContext as DataTable);
            GridSearch.V_Xem();
        }
        #endregion

        #region btnthoat_Click
        private void btnthoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        #region txtlien_LostFocus
        private void txtlien_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!txtlien.IsFocusWithin)
            {
                if (txtlien.Value.ToString() == "")
                    txtlien.Value = 0;
            }
        }
        #endregion

        #region txtctu0_LostFocus
        private void txtctu0_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!txtctu0.IsFocusWithin)
            {
                if (txtctu0.Value.ToString() == "")
                    txtctu0.Value = 0;
                DsPrint.Tables["TablePH"].DefaultView[0]["so_ct_goc"] = txtctu0.Value;
            }
        }
        #endregion

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (DsPrint.Tables[0].DefaultView.Count == 1)
            {
                string file_name = StartUp.GetFileNameExportWithSignature(DsPrint.Tables[0].DefaultView[0]);
                IntPtr windowHandle = new WindowInteropHelper(this).Handle;
                GridSearch.V_XuatPdf(file_name, windowHandle);
            }
        }
    }
}
