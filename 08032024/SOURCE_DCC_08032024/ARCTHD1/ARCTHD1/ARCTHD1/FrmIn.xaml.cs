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
using SmReport;
using System.Windows.Interop;

namespace ARCTHD1
{
    /// <summary>
    /// Interaction logic for FrmIn.xaml
    /// </summary>
    public partial class FrmIn : Form
    {
        DataSet dsSource = new DataSet();
        DataSet dsTmp = new DataSet();
        bool IsND51;
        public FrmIn(bool isND51)
        {
            InitializeComponent();
            SmLib.SysFunc.LoadIcon(this);
            GridSearch.LocalSysObj = StartUp.SysObj;
            IsND51 = isND51;
            if (isND51)
            {
                GridSearch.ReportGroupName = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_file"].ToString();
                GridSearch.DefaultFilter = "report not like 'SOCTHDA_GTGT_XKho%'";
            }
            else
            {
                GridSearch.ReportGroupName = StartUp.CommandInfo["rep_file"].ToString();
            }

            dsSource = StartUp.DsTrans.Copy();
            StartUp.GetDmnt(dsSource);
            DataColumn newcolumn = new DataColumn("so_lien", typeof(int));
            newcolumn.DefaultValue = 1;
            dsSource.Tables[0].Columns.Add(newcolumn);

            newcolumn = new DataColumn("so_ct_goc", typeof(int));
            newcolumn.DefaultValue = 0;
            dsSource.Tables[0].Columns.Add(newcolumn);

            newcolumn = new DataColumn("thue_suat", typeof(decimal));
            newcolumn.DefaultValue = 0;
            dsSource.Tables[0].Columns.Add(newcolumn);

            newcolumn = new DataColumn("ban_sao", typeof(string));
            newcolumn.DefaultValue = "";
            dsSource.Tables[0].Columns.Add(newcolumn);

            newcolumn = new DataColumn("stt", typeof(string));
            newcolumn.DefaultValue = "";
            dsSource.Tables[1].Columns.Add(newcolumn);

            //newcolumn = new DataColumn("ten_dv", typeof(string));
            //newcolumn.DefaultValue = "";
            //dsSource.Tables[1].Columns.Add(newcolumn);

            dsSource.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'";
            dsSource.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'";
            dsSource.Tables[1].DefaultView.Sort = "stt_rec0";

            dsSource.Tables[0].TableName = "TablePH";
            dsSource.Tables[1].TableName = "TableCT";
            dsSource.Tables.Add(CreateTableInfo().Copy());
            dsSource.Tables.Add(CreateTableMST(StartUp.M_MA_THUE, "TableMST_NB").Copy());
            dsSource.Tables.Add(CreateTableMST(dsSource.Tables["TablePH"].DefaultView[0]["ma_so_thue"].ToString().TrimEnd(), "TableMST_NM").Copy());


            if (!dsSource.Tables["TableCT"].Columns.Contains("dvt"))
                dsSource.Tables["TableCT"].Columns.Add("dvt", typeof(string));
            if (!dsSource.Tables["TableCT"].Columns.Contains("so_luong"))
                dsSource.Tables["TableCT"].Columns.Add("so_luong", typeof(int));
            //do co doan set stt cho nay nen mau in ko len thue suat
            //http://forum.fast.com.vn/showthread.php?t=14814&p=155748#post155748
            //int nRow = dsSource.Tables["TableCT"].DefaultView.Count;
            //for (int i = 0; i < nRow; i++)
            //{
            //    //dsSource.Tables["TableCT"].DefaultView[i]["dvt"] = dsSource.Tables["TableCT"].Rows[i]["dvt"];
            //    dsSource.Tables["TableCT"].DefaultView[i]["stt"] = i + 1;
            //    //dsSource.Tables["TableCT"].DefaultView[i]["so_luong"] = dsSource.Tables["TableCT"].Rows[i]["so_luong"];
            //}
            dsTmp = dsSource.Copy();
            dsTmp.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'";
            dsTmp.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'";
            dsTmp.Tables[1].DefaultView.Sort = "stt_rec0";

            UpdateTenTD();

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


        private void UpdateTenTD()
        {

            string sttrec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim();
            string cmd = "select ma_phi_i,ma_td_i,ma_td2_i,ma_td3_i,ten_phi, ten_td , ten_td2 ,ten_td3 from v_ct21 where  stt_rec= '" + sttrec + "'";
            SqlCommand sqlcmd = new SqlCommand(cmd);
            DataTable tb = new DataTable();
            tb = SmVoucherLib.DataProvider.FillCommand(StartUp.SysObj, sqlcmd).Tables[0].Copy();
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                for (int j = 0; j < dsSource.Tables[1].Rows.Count; j++)
                {
                    if (dsSource.Tables[1].Columns.Contains("ten_phi"))
                        if (dsSource.Tables[1].Rows[j]["ma_phi_i"].ToString().Trim().Equals(tb.Rows[i]["ma_phi_i"].ToString().Trim()))
                        {
                            dsSource.Tables[1].Rows[j]["ten_phi"] = tb.Rows[i]["ten_phi"];
                        }
                    if (dsSource.Tables[1].Columns.Contains("ten_td"))
                        if (dsSource.Tables[1].Rows[j]["ma_td_i"].ToString().Trim().Equals(tb.Rows[i]["ma_td_i"].ToString().Trim()))
                        {
                            dsSource.Tables[1].Rows[j]["ten_td"] = tb.Rows[i]["ten_td"];
                        }
                    if (dsSource.Tables[1].Columns.Contains("ten_td2"))
                        if (dsSource.Tables[1].Rows[j]["ma_td2_i"].ToString().Trim().Equals(tb.Rows[i]["ma_td2_i"].ToString().Trim()))
                        {
                            dsSource.Tables[1].Rows[j]["ten_td2"] = tb.Rows[i]["ten_td2"];
                        }
                    if (dsSource.Tables[1].Columns.Contains("ten_td3"))
                        if (dsSource.Tables[1].Rows[j]["ma_td3_i"].ToString().Trim().Equals(tb.Rows[i]["ma_td3_i"].ToString().Trim()))
                        {
                            dsSource.Tables[1].Rows[j]["ten_td3"] = tb.Rows[i]["ten_td3"];
                        }
                }
            }
            dsSource.Tables[1].AcceptChanges();
        }


        #region CreateTableInfo
        DataTable CreateTableInfo()
        {
            DataTable table = new DataTable();
            table.TableName = "TableInfo";

            DataColumn column = new DataColumn("M_PHONE", typeof(string));
            column.DefaultValue = StartUp.M_PHONE;
            table.Columns.Add(column);

            column = new DataColumn("M_MST", typeof(string));
            column.DefaultValue = StartUp.M_MA_THUE;
            table.Columns.Add(column);

            column = new DataColumn("M_Ten_CTY", typeof(string));
            column.DefaultValue = StartUp.SysObj.GetSysVar("M_Ten_CTY").ToString();
            table.Columns.Add(column);

            column = new DataColumn("M_DIA_CHI", typeof(string));
            column.DefaultValue = StartUp.SysObj.GetSysVar("M_DIA_CHI").ToString();
            table.Columns.Add(column);

            column = new DataColumn("M_TK_NH", typeof(string));
            column.DefaultValue = StartUp.SysObj.GetOption("M_TK_NH").ToString();
            table.Columns.Add(column);

            column = new DataColumn("M_CUC_THUE", typeof(string));
            column.DefaultValue = StartUp.SysObj.GetOption("M_CUC_THUE").ToString().ToUpper();
            table.Columns.Add(column);

            DataRow dr = table.NewRow();
            table.Rows.Add(dr);
            return table;
        }
        #endregion

        #region CreateTableMST
        DataTable CreateTableMST(string ma_so_thue, string tablename)
        {
            DataTable table = new DataTable();
            table.TableName = tablename;
            int length = ma_so_thue.Length;
            int i = -1;
            DataColumn column = null;
            for (i = 0; i < length; i++)
            {
                column = new DataColumn("m" + (i + 1).ToString(), typeof(string));
                column.DefaultValue = ma_so_thue.Substring(i, 1);
                table.Columns.Add(column);
            }
            while (i < 14)
            {
                column = new DataColumn("m" + (i + 1).ToString(), typeof(string));
                column.DefaultValue = "";
                table.Columns.Add(column);
                i++;
            }

            DataRow row = table.NewRow();
            table.Rows.Add(row);
            return table;
        }
        #endregion

        private void InsertSubRow()
        {
            //Thêm dữ lieu cho tien sau ck
            dsSource.Tables[0].DefaultView[0]["t_tien_sau_ck_nt"] = dsSource.Tables[0].DefaultView[0]["t_tien_nt2"];
            dsSource.Tables[0].DefaultView[0]["t_tien_sau_ck"] = dsSource.Tables[0].DefaultView[0]["t_tien2"];
            dsSource.Tables[0].DefaultView[0]["so_seri"] = dsSource.Tables[0].DefaultView[0]["so_seri"];
            foreach (DataRowView dr in dsSource.Tables[1].DefaultView)
            {
                if (dsSource.Tables[0].DefaultView[0]["ma_gd"].ToString().IndexOfAny(new char[] { '2', '5' }) >= 0)
                    dr["tien"] = dr["tien_tt"];
            }
            if (dsSource.Tables[1].DefaultView.ToTable().Select("stt <> ''").Count() == 0 /*&& ((GridSearch.XGReport.ActiveRecord as DataRecord).DataItem as DataRowView)["nd51"].ToString() == "1"*/)
            {
                //ph
                dsSource.Tables[0].DefaultView[0]["thue_suat"] = dsSource.Tables[1].DefaultView[0]["thue_suati"];
                //ct
                int _index = 1;
                foreach (DataRowView dr in dsSource.Tables[1].DefaultView)
                {
                    dr["stt"] = _index;
                    //dr["ten_dv"] = dr["ten_vt"].ToString().Trim();
                    _index++;
                }
                /*Chị VANTT bảo không lên đoạn này
                //Thêm dòng ghi chú
                if (!string.IsNullOrEmpty(dsSource.Tables[0].DefaultView[0]["gc_thue"].ToString().Trim()))
                {
                    DataRow drv = dsSource.Tables[1].NewRow();
                    drv["ten_vt"] = "(" + dsSource.Tables[0].DefaultView[0]["gc_thue"].ToString().Trim() + ")";
                    drv["stt_rec"] = dsSource.Tables[0].DefaultView[0]["stt_rec"].ToString();
                    dsSource.Tables[1].Rows.Add(drv);
                }
                */
               
            }
            //Thêm dòng chiết khấu
            if (Convert.ToDecimal(dsSource.Tables[0].DefaultView[0]["t_ck_nt"]) > 0 || Convert.ToDecimal(dsSource.Tables[0].DefaultView[0]["t_ck"]) > 0)
            {
                if (dsSource.Tables[1].Select("ten_vt LIKE 'Chiết khấu'").Length == 0)
                {
                    DataRecord dr = GridSearch.XGReport.ActiveRecord as DataRecord;

                    DataRow drv = dsSource.Tables[1].NewRow();
                    drv["stt_rec"] = dsSource.Tables[0].DefaultView[0]["stt_rec"].ToString();
                    drv["stt_rec0"] = -1;
                    drv["ten_vt"] = "Chiết khấu";
                    if (dr != null)
                    {
                        drv["dien_giaii"] = dr.Cells["lan"].Value.ToString().Trim().ToLower().Equals("anh") ? "Discount" : "Chiết khấu";
                    }
                    drv["tien_nt2"] = dsSource.Tables[0].DefaultView[0]["t_ck_nt"];
                    drv["tien2"] = dsSource.Tables[0].DefaultView[0]["t_ck"];
                    dsSource.Tables[1].Rows.Add(drv);
                }
            }
            dsSource.Tables[1].AcceptChanges();
            //Thêm số dòng cho đủ ngầm định
            GridSearch.InsertSubRow(StartUp.Ma_ct, 1);
        }

        private void ResetData()
        {
            dsSource = dsTmp.Copy();
            dsSource.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'";
            dsSource.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'";
            dsSource.Tables[1].DefaultView.Sort = "stt_rec0";
            UpdateTenTD();
            GridSearch.DSource = dsSource;
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            DataTable tbIn = StartUp.GetPhIn();
            if (tbIn.Rows.Count == 0)
            {
                DataRow dr = tbIn.NewRow();
                dr["ma_ct"] = StartUp.Ma_ct;
                dr["stt_rec"] = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim();
                dr["so01"] = 0;
                dr["so02"] = StartUp.DsTrans.Tables[0].DefaultView[0]["so_lien_hd"].ToString().Trim();
                tbIn.Rows.Add(dr);
            }

            if (tbIn.Rows.Count == 1)
            {
                DataRow dr = tbIn.Rows[0];
                if (IsND51)
                {
                    dr["so02"] = StartUp.DsTrans.Tables[0].DefaultView[0]["so_lien_hd"].ToString().Trim();
                }
                else
                {
                    //dr["so02"] = StartUp.DmctInfo["so_lien"] == DBNull.Value ? 1 : StartUp.DmctInfo["so_lien"];
                    //txtlien.IsReadOnly = false;
                }
            }
            GridSearch.XGReport.RecordActivated += new EventHandler<Infragistics.Windows.DataPresenter.Events.RecordActivatedEventArgs>(XGReport_RecordActivated);
            this.DataContext = tbIn;
            txtctu0.Focus();
        }

        void XGReport_RecordActivated(object sender, Infragistics.Windows.DataPresenter.Events.RecordActivatedEventArgs e)
        {
            if (GridSearch.XGReport.ActiveRecord == null || GridSearch.XGReport.ActiveRecord.RecordType != RecordType.DataRecord)
                return;
            DataRowView drvXReport = (GridSearch.XGReport.ActiveRecord as DataRecord).DataItem as DataRowView;
            string nd51 = drvXReport["nd51"].ToString();
            if (nd51 == "1")
            {
                txtlien.IsReadOnly = true;
                txtlien.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["so_lien_hd"].ToString().Trim();
            }
            else
            {
                txtlien.IsReadOnly = false;
                txtlien.Text = StartUp.DmctInfo["so_lien"].ToString().Trim();
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
            if (GridSearch.XGReport.ActiveRecord == null)
                return;

            if (txtlien.Value != null)
            {
                if (((GridSearch.XGReport.ActiveRecord as DataRecord).DataItem as DataRowView)["nd51"].ToString() == "1")
                    if (!StartUp.GetSl_in(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()).ToString().Equals("0"))
                        if (ExMessageBox.Show( 465,StartUp.SysObj, "Hóa đơn đã được in, có muốn in lại hay không?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                            return;
                        else
                        {
                            FrmLogin _frmIn = new FrmLogin();
                            _frmIn.ShowDialog();
                            if (!_frmIn.IsLogined)
                                return;
                        }
                dsSource.Tables[0].DefaultView[0]["so_ct_goc"] = txtctu0.Text;
                dsSource.Tables[0].DefaultView[0]["so_lien"] = Convert.ToInt16(txtlien.Text);

                int so_lien = 1, so_lien_hd = 0, so_lien_xac_minh = 0; ;
                int so_lien_hien_thoi = Convert.ToInt32(StartUp.GetSo_lien((DataRecord)GridSearch.XGReport.ActiveRecord, StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()));
                int.TryParse(dsSource.Tables[0].DefaultView[0]["so_lien_hd"].ToString(), out so_lien_hd);
                int.TryParse(StartUp.DmctInfo["so_lien_xac_minh"].ToString(), out so_lien_xac_minh);

                if (so_lien_hien_thoi > so_lien_hd)
                    dsSource.Tables[0].DefaultView[0]["ban_sao"] = "BẢN SAO";

                int so_lan_in = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(txtlien.Text) / Convert.ToDouble((GridSearch.XGReport.ActiveRecord as DataRecord).Cells["so_lien"].Value)));
                if (((GridSearch.XGReport.ActiveRecord as DataRecord).DataItem as DataRowView)["nd51"].ToString() != "1")
                    so_lien_hd = so_lan_in;

                while (so_lien <= so_lan_in)
                {
                    if (so_lien_hien_thoi <= so_lien_hd)
                    {
                        if (so_lien <= so_lien_hien_thoi || so_lien > so_lien_hd)
                            dsSource.Tables[0].DefaultView[0]["ban_sao"] = "BẢN SAO";
                        else
                            dsSource.Tables[0].DefaultView[0]["ban_sao"] = "";
                    }

                    dsSource.Tables[0].DefaultView[0]["so_lien"] = so_lien_hd !=0 && so_lien % so_lien_hd > 0 ? so_lien % so_lien_hd : so_lien_hd;
                    InsertSubRow();

                    //doan code duoi gay loi http://forum.fast.com.vn/showthread.php?t=12407
                    //int stt = 1;
                    //foreach (DataRowView dr in dsSource.Tables[1].DefaultView)
                    //{
                    //    dr["stt"] = stt;
                    //    stt++;
                    //}

                    GridSearch.V_In(1, (so_lien_xac_minh >= so_lien && string.IsNullOrEmpty(dsSource.Tables[0].DefaultView[0]["ban_sao"].ToString())));
                    so_lien++;
                }
                //update sl_in và so_lien in
                if (((GridSearch.XGReport.ActiveRecord as DataRecord).DataItem as DataRowView)["nd51"].ToString() == "1" && GridSearch.PrintSuccess)
                {
                    StartUp.UpdateSl_in(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString(), ((GridSearch.XGReport.ActiveRecord as DataRecord).DataItem as DataRowView)["id"].ToString(), txtlien.Text);
                    StartUp.DsTrans.Tables[0].DefaultView[0]["sl_in"] = StartUp.GetSl_in(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()).ToString();
                }

                ResetData();
                StartUp.SetPhIn(this.DataContext as DataTable);
            }
        	this.Close();
		}

        private void BtnInLT_Click(object sender, RoutedEventArgs e)
        {
            if (GridSearch.XGReport.ActiveRecord == null)
                return;
            DataRowView drvXReport = (GridSearch.XGReport.ActiveRecord as DataRecord).DataItem as DataRowView;
            string mau_tu_in = drvXReport["mau_tu_in"].ToString();

            if (StartUp.IsQLHD && mau_tu_in == "1")
            {
                ExMessageBox.Show( 470,StartUp.SysObj, "Có chứng từ thuộc mẫu hóa đơn tự in, không in liên tục được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (StartUp.M_IN_HOI_CK == 1)
            {
                if (ExMessageBox.Show( 475,StartUp.SysObj, "Có chắc chắn in tất cả các chứng từ đã được lọc?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                {
                    return;
                }
            }
            if (txtlien.Value != null)
            {
                if (((GridSearch.XGReport.ActiveRecord as DataRecord).DataItem as DataRowView)["nd51"].ToString() == "1")
                    if (!StartUp.GetSl_in(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()).ToString().Equals("0"))
                        if (ExMessageBox.Show( 480,StartUp.SysObj, "Hóa đơn đã được in, có muốn in lại hay không?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                            return;
                        else
                        {
                            FrmLogin _frmIn = new FrmLogin();
                            _frmIn.ShowDialog();
                            if (!_frmIn.IsLogined)
                                return;
                            else
                            {
                                dsSource.Tables[0].DefaultView[0]["ban_sao"] = "BẢN SAO";
                            }
                        }

                int iRowTmp = FrmArcthd1.iRow;

                int so_lien = 1;
                List<int> lstSo_lien = new List<int>();

                int so_lien_xac_minh = 0;
                int.TryParse(StartUp.DmctInfo["so_lien_xac_minh"].ToString(), out so_lien_xac_minh);

                int so_lan_in = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(txtlien.Text) / Convert.ToDouble((GridSearch.XGReport.ActiveRecord as DataRecord).Cells["so_lien"].Value)));
                while (so_lien <= so_lan_in)
                {
                    for (int i = 1; i < StartUp.DsTrans.Tables[0].Rows.Count; i++)
                    {
                        string stt_rec = dsSource.Tables[0].Rows[i]["stt_rec"].ToString();
                        dsSource.Tables[0].DefaultView.RowFilter = "stt_rec = '" + stt_rec + "'";
                        dsSource.Tables[1].DefaultView.RowFilter = "stt_rec = '" + stt_rec + "'";
                        dsSource.Tables[1].DefaultView.Sort = "stt_rec0";
                        if (so_lien == 1)
                        {
                            lstSo_lien.Add(Convert.ToInt32(StartUp.GetSo_lien((DataRecord)GridSearch.XGReport.ActiveRecord, dsSource.Tables[0].DefaultView[0]["stt_rec"].ToString())));
                        }
                        if (dsSource.Tables[0].DefaultView[0]["status"].ToString() != "3")
                        {
                            int so_lien_hien_thoi = lstSo_lien[i - 1], so_lien_hd = 0;
                            int.TryParse(dsSource.Tables[0].DefaultView[0]["so_lien_hd"].ToString(), out so_lien_hd);
                            if (((GridSearch.XGReport.ActiveRecord as DataRecord).DataItem as DataRowView)["nd51"].ToString() != "1")
                                so_lien_hd = so_lan_in;
                            if (so_lien_hien_thoi > so_lien_hd)
                                dsSource.Tables[0].DefaultView[0]["ban_sao"] = "BẢN SAO";
                            else
                            {
                                if (so_lien <= so_lien_hien_thoi || so_lien > so_lien_hd)
                                    dsSource.Tables[0].DefaultView[0]["ban_sao"] = "BẢN SAO";
                                else
                                    dsSource.Tables[0].DefaultView[0]["ban_sao"] = "";
                            }

                            dsSource.Tables[0].DefaultView[0]["so_ct_goc"] = txtctu0.Text;
                            dsSource.Tables[0].DefaultView[0]["so_lien"] = so_lien_hd != 0 && so_lien % so_lien_hd > 0 ? so_lien % so_lien_hd : so_lien_hd;

                            InsertSubRow();

                            //doan code duoi gay loi http://forum.fast.com.vn/showthread.php?t=12407
                            //int stt = 1;
                            //foreach (DataRowView dr in dsSource.Tables[1].DefaultView)
                            //{
                            //    dr["stt"] = stt;
                            //    stt++;
                            //}

                            GridSearch.V_In(Convert.ToInt16(1), (so_lien_xac_minh >= so_lien && string.IsNullOrEmpty(dsSource.Tables[0].DefaultView[0]["ban_sao"].ToString())));

                            //Cập nhật thông tin in cho chừng chứng từ
                            if (((GridSearch.XGReport.ActiveRecord as DataRecord).DataItem as DataRowView)["nd51"].ToString() == "1" && so_lien == 1 && GridSearch.PrintSuccess)
                            {
                                StartUp.UpdateSl_in(dsSource.Tables[0].Rows[i]["stt_rec"].ToString(), ((GridSearch.XGReport.ActiveRecord as DataRecord).DataItem as DataRowView)["id"].ToString(), txtlien.Text);
                                StartUp.DsTrans.Tables[0].DefaultView[0]["sl_in"] = StartUp.GetSl_in(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()).ToString();
                            }
                        }
                    }
                    so_lien++;
                }

                StartUp.SetPhIn(this.DataContext as DataTable);
                ResetData();

                StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRowTmp]["stt_rec"].ToString() + "'";
                StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRowTmp]["stt_rec"].ToString() + "'";
                
            }
        	this.Close();
		}

        void GridSearch_ReportPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BtnXem_Click(sender, e);
        }

        private void BtnXem_Click(object sender, RoutedEventArgs e)
        {
            if (GridSearch.XGReport.ActiveRecord == null)
                return;

            string _str_sl_in = StartUp.GetSl_in(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()).ToString();
            if (!_str_sl_in.Equals("0"))
            {
                dsSource.Tables[0].DefaultView[0]["ban_sao"] = "BẢN SAO";
                StartUp.DsTrans.Tables[0].DefaultView[0]["sl_in"] = _str_sl_in;
            }
            dsSource.Tables[0].DefaultView[0]["so_ct_goc"] = txtctu0.Text;
            InsertSubRow();
            GridSearch.V_Xem();
            ResetData();
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
