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
using Sm.Windows.Controls;
using Infragistics.Windows.Editors;
using System.Data;
using System.Data.SqlClient;
using Infragistics.Windows.DataPresenter;
using System.Windows.Interop;

namespace CACTBC1
{
    /// <summary>
    /// Interaction logic for FrmIn.xaml
    /// </summary>
    public partial class FrmIn : Form
    {
        DataSet dsSource;
        public FrmIn()
        {
            InitializeComponent();
            SmLib.SysFunc.LoadIcon(this);
            GridSearch.LocalSysObj = StartUp.SysObj;
            GridSearch.ReportGroupName = StartUp.CommandInfo["rep_file"].ToString();

            dsSource = StartUp.DsTrans.Copy();
            StartUp.GetDmnt(dsSource);
            StartUp.CreateTableInfo(dsSource);

            DataColumn newcolumn = new DataColumn("so_lien", typeof(int));
            newcolumn.DefaultValue = 1;
            dsSource.Tables[0].Columns.Add(newcolumn);

            newcolumn = new DataColumn("so_lienQD1548", typeof(int));
            newcolumn.DefaultValue = 1;
            dsSource.Tables[0].Columns.Add(newcolumn);

            newcolumn = new DataColumn("so_ct_goc", typeof(int));
            newcolumn.DefaultValue = 0;
            dsSource.Tables[0].Columns.Add(newcolumn);

            newcolumn = new DataColumn("dien_giai_ct_goc", typeof(string));
            newcolumn.DefaultValue = "";
            dsSource.Tables[0].Columns.Add(newcolumn);

            dsSource.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'";
            dsSource.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'";
            dsSource.Tables[1].DefaultView.Sort = "stt_rec0";
            UpdateTenTKVN_EN();

            GridSearch.DSource = dsSource;
            GridSearch.ReportPreviewMouseDoubleClick += new SmReport.ControlFilterReport.MouseClick(GridSearch_ReportPreviewMouseDoubleClick);

            if (BindingSysObj.GetOption("M_LAN").ToString().Equals("V"))
            {
                btnExport.Content = BindingSysObj.GetSysVar("M_EXPORT_SIGN").ToString();
            }
            else
            {
                btnExport.Content = BindingSysObj.GetSysVar2("M_EXPORT_SIGN").ToString();
            }
        }

        private void UpdateTenTKVN_EN()
        {

            string sttrec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim();
            string cmd = "select tk_i, ten_tk as ten_tk_i, ten_tk2 as ten_tk_i2  from ct51 a, dmtk b where a.tk_i=tk and stt_rec= '" + sttrec + "' ORDER BY stt_rec0";
            SqlCommand sqlcmd = new SqlCommand(cmd);
            DataTable tb = new DataTable();
            tb = SmVoucherLib.DataProvider.FillCommand(StartUp.SysObj, sqlcmd).Tables[0].Copy();
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                for (int j = 0; j < dsSource.Tables[1].Rows.Count; j++)


                    if (dsSource.Tables[1].Rows[j]["tk_i"].ToString().Trim().Equals(tb.Rows[i]["tk_i"].ToString().Trim()))
                    {
                        dsSource.Tables[1].Rows[j]["ten_tk_i"] = tb.Rows[i]["ten_tk_i"];
                        dsSource.Tables[1].Rows[j]["ten_tk_i2"] = tb.Rows[i]["ten_tk_i2"];
                    }
            }
            dsSource.Tables[1].AcceptChanges();
        }

        private void AddtbPsNo()
        {
            /*
            foreach (DataRowView dr in dsSource.Tables[1].DefaultView)
            {
                if (dsSource.Tables[0].DefaultView[0]["ma_gd"].ToString().IndexOfAny(new char[] { '2', '5' }) >= 0)
                    dr["tien"] = dr["tien_tt"];
            }
            */
            var psNoCT = from o in dsSource.Tables[1].DefaultView.ToTable().AsEnumerable()
                         group o by o.Field<string>("tk_i") into g
                         select new
                         {
                             TK = g.Key,
                             Tien_nt = g.Sum(p => p.Field<decimal?>("tien_nt")),
                             Tien = g.Sum(p => p.Field<decimal?>("tien"))
                         };
            DataTable tbPsNo = dsSource.Tables[1].Clone();
            tbPsNo.TableName = "TablePsNo";
            if (psNoCT.ToArray().Length > 0)
            {
                foreach (var psNoct in psNoCT)
                {
                    DataRow dr = tbPsNo.NewRow();
                    dr["tk_i"] = psNoct.TK;
                    dr["tien_nt"] = psNoct.Tien_nt;
                    dr["tien"] = psNoct.Tien;
                    tbPsNo.Rows.Add(dr);
                }
            }
            if (dsSource.Tables.Contains("TablePsNo"))
                dsSource.Tables.Remove("TablePsNo");
            dsSource.Tables.Add(tbPsNo);
        }
        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Title = SmLib.SysFunc.Cat_Dau(this.Title);
                DataTable tbIn = StartUp.GetPhIn();
                if (tbIn.Rows.Count == 0)
                {
                    DataRow dr = tbIn.NewRow();
                    dr["ma_ct"] = StartUp.Ma_ct;
                    dr["stt_rec"] = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim();
                    dr["so01"] = 0;
                    dr["gc01"] = "";
                    dr["so02"] = StartUp.DmctInfo["so_lien"] == DBNull.Value ? 1 : StartUp.DmctInfo["so_lien"];
                    tbIn.Rows.Add(dr);
                }
                this.DataContext = tbIn;
                AddTbTkPs(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim());
                txtdiengiaict0.Focus();
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        private void AddTbTkPs(string sttrec)
        {
            string cmd = "select MIN(stt_rec0) stt_rec0, tk_i as tk, case when abs(sum(tien_tt))> 0 then sum(tien_tt) else sum(tien) end as tien, sum(tien_nt) as tien_nt from ct51 where stt_rec='" + sttrec + "' group by tk_i order by stt_rec0";
            SqlCommand sqlcmd = new SqlCommand(cmd);
            DataTable tb = new DataTable();
            tb = SmVoucherLib.DataProvider.FillCommand(StartUp.SysObj, sqlcmd).Tables[0].Copy();
            tb.TableName = "tbTaiKhoanPS";
            if (dsSource.Tables.Contains("tbTaiKhoanPS"))
            {
                dsSource.Tables["tbTaiKhoanPS"].Clear();
                foreach (DataRow _dr in tb.Rows)
                {
                    dsSource.Tables["tbTaiKhoanPS"].Rows.Add(_dr.ItemArray);
                }
            }
            else
            {
                dsSource.Tables.Add(tb);
            }
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void BtnIn_Click(object sender, RoutedEventArgs e)
        {
            if (txtlien.Value != null)
            {
                int so_lien = 1, so_lienQD1548 = 1;
                dsSource.Tables[0].DefaultView[0]["so_ct_goc"] = txtctu0.Text;
                dsSource.Tables[0].DefaultView[0]["dien_giai_ct_goc"] = txtdiengiaict0.Text;
                if (GridSearch.XGReport.ActiveRecord != null)
                {
                    int so_lan_in = Convert.ToInt16(txtlien.Text) / Convert.ToInt16((GridSearch.XGReport.ActiveRecord as DataRecord).Cells["so_lien"].Value);
                    if (Convert.ToInt16(txtlien.Text) % Convert.ToInt16((GridSearch.XGReport.ActiveRecord as DataRecord).Cells["so_lien"].Value) != 0)
                        so_lan_in += 1;
                    AddTbTkPs(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim());
                    while (so_lien <= so_lan_in)
                    {
                        dsSource.Tables[0].DefaultView[0]["so_lien"] = so_lien;
                        dsSource.Tables[0].DefaultView[0]["so_lienQD1548"] = so_lienQD1548;
                        AddtbPsNo();
                        GridSearch.InsertSubRow(StartUp.Ma_ct, 1);
                        GridSearch.V_In(1);
                        so_lien++;
                        so_lienQD1548 += 2;
                    }
                    StartUp.SetPhIn(this.DataContext as DataTable);
                }
            }
        	this.Close();
		}

        private void BtnInLT_Click(object sender, RoutedEventArgs e)
        {
            int so_lien, so_lienQD1548;
            if (StartUp.M_IN_HOI_CK == 1)
            {
                if (ExMessageBox.Show( 175,StartUp.SysObj, "Có chắc chắn in tất cả các chứng từ đã được lọc ?", "Fast Book 11 .NET", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                {
                    return;
                }
            }
            if (txtlien.Value != null)
            {
                int iRowTmp = FrmCACTBC1.iRow;

                dsSource.Tables[0].DefaultView[0]["so_ct_goc"] = txtctu0.Text;
                dsSource.Tables[0].DefaultView[0]["dien_giai_ct_goc"] = txtdiengiaict0.Text;
                so_lien = 1;
                so_lienQD1548 = 1;
                if (GridSearch.XGReport.ActiveRecord != null)
                {
                    int so_lan_in = Convert.ToInt16(txtlien.Text) / Convert.ToInt16((GridSearch.XGReport.ActiveRecord as DataRecord).Cells["so_lien"].Value);
                    if (Convert.ToInt16(txtlien.Text) % Convert.ToInt16((GridSearch.XGReport.ActiveRecord as DataRecord).Cells["so_lien"].Value) != 0)
                        so_lan_in += 1;
                    while (so_lien <= so_lan_in)
                    {
                        for (int i = 1; i < dsSource.Tables[0].Rows.Count; i++)
                        {
                            dsSource.Tables[0].DefaultView.RowFilter = "stt_rec= '" + dsSource.Tables[0].Rows[i]["stt_rec"].ToString() + "'";
                            dsSource.Tables[1].DefaultView.RowFilter = "stt_rec= '" + dsSource.Tables[0].Rows[i]["stt_rec"].ToString() + "'";
                            dsSource.Tables[1].DefaultView.Sort = "stt_rec0";

                            dsSource.Tables[0].DefaultView[0]["so_lien"] = so_lien;
                            dsSource.Tables[0].DefaultView[0]["so_lienQD1548"] = so_lienQD1548;
                            AddTbTkPs(dsSource.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim());
                            AddtbPsNo();
                            GridSearch.InsertSubRow(StartUp.Ma_ct, 1);
                            GridSearch.V_In(1);
                        }
                        so_lien++;
                        so_lienQD1548 += 2;
                    }
                    StartUp.SetPhIn(this.DataContext as DataTable);
                    StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRowTmp]["stt_rec"].ToString() + "'";
                    StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRowTmp]["stt_rec"].ToString() + "'";
                    StartUp.DsTrans.Tables[1].DefaultView.Sort = "stt_rec0";
                }
            }
        	this.Close();
		}

        void GridSearch_ReportPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BtnXem_Click(sender, e);
        }

        private void BtnXem_Click(object sender, RoutedEventArgs e)
        {
            dsSource.Tables[0].DefaultView[0]["so_ct_goc"] = txtctu0.Text;
            dsSource.Tables[0].DefaultView[0]["dien_giai_ct_goc"] = txtdiengiaict0.Text;
            AddTbTkPs(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim());
            AddtbPsNo();
            GridSearch.InsertSubRow(StartUp.Ma_ct, 1);
            GridSearch.V_Xem();
            StartUp.SetPhIn(this.DataContext as DataTable);
        }

        private void BtnThoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtctu0_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!txtctu0.IsFocusWithin)
            {
                if (txtctu0.Value.ToString() == "")
                    txtctu0.Value = 0;
                dsSource.Tables[0].DefaultView[0]["so_ct_goc"] = txtctu0.Value;
            }
        }

        private void txtlien_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!txtlien.IsFocusWithin)
            {
                if (txtlien.Value.ToString() == "")
                    txtlien.Value = 0;
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (dsSource.Tables[0].DefaultView.Count == 1)
            {
                string file_name = StartUp.GetFileNameExportWithSignature(dsSource.Tables[0].DefaultView[0]);
                IntPtr windowHandle = new WindowInteropHelper(this).Handle;
                GridSearch.V_XuatPdf(file_name, windowHandle);
            }
        }
    }
}
