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
using System.Diagnostics;
using System.Data;
using Infragistics.Windows.DataPresenter;
using System.Data.SqlClient;
using System.Threading;
using System.Windows.Threading;
using SmDataLib;
using SmErrorLib;
using SmLib;



namespace COSXLSX.COLSX
{
    /// <summary>
    /// Interaction logic for FrmCapNhat.xaml
    /// </summary>
    public partial class FrmCapNhat : Form
    {
        DataSet dsTmp;
        static public DataSet dsReport = new DataSet();
        static SqlCommand cmd = new SqlCommand();
        static SqlCommand cmd1 = new SqlCommand();
        static public DataSet ds = new DataSet();
        DataTable newDataTable = new DataTable();

        string temp = "0";
        string g_so_lsx = string.Empty;
        public bool isCloseForm = false;
        static public string ten_ytcp = "";
        CodeValueBindingObject Voucher_Lan0;
        static public DataTable listBpPxNew = new DataTable();
        static public DataTable listBpht_lsx = new DataTable();
        static public DataTable listBpht = new DataTable();
        DataTable OldRow = null;
        public FrmCapNhat()
        {
            InitializeComponent();
        }

        public FrmCapNhat(string so_lsx)
        {
            InitializeComponent();
            SmLib.SysFunc.LoadIcon(this);
            this.BindingSysObj = StartUp.SysObj;
            g_so_lsx = so_lsx;



        }

        #region LoadForm
        void LoadForm()
        {
            DataTable TableFields = ListFunc.GetSqlTableFieldList(StartUp.SysObj, StartUp.sqlTableName);
            txtSo_lsx.MaxLength = ListFunc.GetLengthColumn(TableFields, "so_lsx");
            txtma_tra_cuu.MaxLength = ListFunc.GetLengthColumn(TableFields, "ma_tra_cuu");
            txtDien_giai.MaxLength = ListFunc.GetLengthColumn(TableFields, "dien_giai");
       
            //tblten_ma_dvcs.Text = txtma_dvcs.RowResult == null ? "" : (StartUp.SysObj.GetOption("M_LAN").ToString() == "V" ? txtma_dvcs.RowResult["ten_dvcs"].ToString() : txtma_dvcs.RowResult["ten_dvcs2"].ToString());
            //this.Title = StartUp.titleWindow;
            GetDmbplsx();


        }
        #endregion

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            LoadForm();

            //Gán ngôn ngữ messagebox
            GrdCt.Lan = StartUp.M_LAN;
            LanguageProvider.Language = StartUp.M_LAN;
            Voucher_Lan0 = (CodeValueBindingObject)this.FindResource("Voucher_Lan0");
            Voucher_Lan0.Value = StartUp.M_LAN.Equals("V");

            if (StartUp.isNew == true)
            {
                //txtSo_lsx.IsReadOnly = false;
                newDataTable = GetRow(StartUp.sqlTableName);
                DataRow newRow = newDataTable.NewRow();

                newRow[StartUp.SqlTableKey] = SysFunc.GetNewMadm(StartUp.SysObj, StartUp.sqlTableName);
                newRow["ma_dvcs"] = txtma_dvcs.Text = StartUp.SysObj.M_ma_dvcs.Trim();
                newRow["ma_px"] = "PXSX";
                newRow["tk_dd"] = "154";
                newRow["ngay_lkh"] = DateTime.Now.Date;
                newDataTable.Rows.Add(newRow);

                dsTmp = StartUp.DataSourceReport.Clone();
                this.gridLayout10.DataContext = newDataTable;// dsTmp.Tables[0].DefaultView;
                this.gridLayout106.DataContext = newDataTable;
                this.gridLayout108.DataContext = newDataTable;
                //this.gridLayout200.DataContext = newDataTable;

                this.gridLayout20.DataContext = dsTmp.Tables[1].DefaultView;
                this.gridLayout201.DataContext = dsTmp.Tables[2].DefaultView;
                this.gridLayout202.DataContext = dsTmp.Tables[3].DefaultView;
                this.GrdCt.DataSource = dsTmp.Tables[1].DefaultView;
                this.GrdCt2.DataSource = dsTmp.Tables[2].DefaultView;
                this.GrdCt3.DataSource = dsTmp.Tables[3].DefaultView;
                txtSo_lsx.Focus();

            }
            else
            {
                //txtSo_lsx.IsReadOnly = true;
                Edit_BPHT();
                newDataTable = GetRow(StartUp.sqlTableName);
                if (newDataTable.Rows.Count > 0)
                {
                    OldRow = newDataTable.Copy();
                }

                dsTmp = StartUp.DataSourceReport.Copy();
                dsTmp.Tables[0].Rows[0]["so_lsx"] = dsTmp.Tables[0].Rows[0]["so_lsx"].ToString().Trim();

                dsTmp.Tables[0].DefaultView.RowFilter = string.Format("so_lsx = '{0}'", g_so_lsx);
                dsTmp.Tables[1].DefaultView.RowFilter = string.Format("so_lsx = '{0}'", g_so_lsx);
                dsTmp.Tables[2].DefaultView.RowFilter = string.Format("so_lsx = '{0}'", g_so_lsx);
                dsTmp.Tables[3].DefaultView.RowFilter = string.Format("so_lsx = '{0}'", g_so_lsx);

                this.gridLayout10.DataContext = dsTmp.Tables[0].DefaultView;
                this.gridLayout106.DataContext = dsTmp.Tables[0].DefaultView;
                this.gridLayout108.DataContext = dsTmp.Tables[0].DefaultView;
                //this.gridLayout200.DataContext = dsTmp.Tables[0].DefaultView;

                this.gridLayout20.DataContext = dsTmp.Tables[1].DefaultView;
                this.gridLayout201.DataContext = dsTmp.Tables[2].DefaultView;
                this.gridLayout202.DataContext = dsTmp.Tables[3].DefaultView;

                this.GrdCt.DataSource = dsTmp.Tables[1].DefaultView;

                this.GrdCt2.DataSource = dsTmp.Tables[2].DefaultView;
                this.GrdCt3.DataSource = dsTmp.Tables[3].DefaultView;
                if (Co_BPHT_Ko() == true)
                {
                    List_YTCP();
                    txtChonBp.Text = "1";
                }
                System.Data.SqlClient.SqlCommand cmdGet = new System.Data.SqlClient.SqlCommand("exec dbo.CheckDeleteListId @ma_dm, @" + StartUp.SqlTableKey);
                cmdGet.Parameters.Add("@ma_dm", SqlDbType.Char).Value = StartUp.sqlTableName;
                cmdGet.Parameters.Add("@" + StartUp.SqlTableKey, SqlDbType.Char).Value = g_so_lsx;
                int ListDelete = (int)StartUp.SysObj.ExcuteScalar(cmdGet);
                if (ListDelete <= 0)
                {
                    if (txtSo_lsx != null)
                        txtSo_lsx.IsReadOnly = true;
                    // txtngay_lsx.Focus();


                }
                txtngay_lsx.Focus();
                //else
                //    txtSo_lsx.Focus();


            }
            txtMapx.SearchInit();
            txtMapx_PreviewLostFocus(null, null);
            txttk_dd.SearchInit();
            txttk_dd_LostFocus(null, null);
            txtma_dvcs.SearchInit();
            txtma_dvcs_PreviewLostFocus(null, null);

            //txtSo_lsx.Focus();
        }

        #region GetRow
        public static DataTable GetRow(string sqlTableName)
        {
            DataTable dt = null;
            SqlCommand cmdGet = new SqlCommand();
            try
            {
                cmdGet.CommandText = "select * from " + sqlTableName + " where 1=0";
                dt = StartUp.SysObj.ExcuteReader(cmdGet).Tables[0];
                //if (dt.Rows.Count > 0)
                //    dt.Rows[0][StartUp.SqlTableKey] = dt.Rows[0][StartUp.SqlTableKey].ToString().Trim();
            }
            catch (SqlException sqlex)
            {
                ErrorLog.CatchMessage(sqlex);
            }
            return dt;
        }
        #endregion


        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && ChonBPHT.flag == false)
            {

                this.Close();
            }
            if (ChonBPHT.flag == true)
                ChonBPHT.flag = false;
        }

        private void Form_Closed(object sender, EventArgs e)
        {

        }

        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            if (StartUp.isNew == true)
            {
                if (checkValidAddNew())
                {
                    StartUp.so_lsx_new = txtSo_lsx.Text.ToString();
                    AddNew();
                }
            }
            else
            {
                if (checkValidEdit())
                {
                    SqlCommand cmd = new SqlCommand();
                    //Xoa Ph
                    cmd.CommandText = "Delete lsxph Where so_lsx = @so_lsx";
                    cmd.Parameters.Add("@so_lsx", SqlDbType.Char, 16).Value = StartUp.so_lsx_old.Trim();
                    StartUp.SysObj.ExcuteNonQuery(cmd);
                    //Xoa Ct
                    SqlCommand cmd1 = new SqlCommand();
                    cmd1.CommandText = "Delete lsxct Where so_lsx = @so_lsx";
                    cmd1.Parameters.Add("@so_lsx", SqlDbType.Char, 16).Value = StartUp.so_lsx_old.Trim();
                    StartUp.SysObj.ExcuteNonQuery(cmd1);
                    //Xoa dmbplsx
                    SqlCommand cmd2 = new SqlCommand();
                    cmd2.CommandText = "Delete from dmbplsx where so_lsx = '" + StartUp.so_lsx_old.Trim() + "'";
                    StartUp.SysObj.ExcuteScalar(cmd2);
                    StartUp.so_lsx_new = txtSo_lsx.Text.ToString();
                    AddNew();
                }
            }

        }

        private void AddNew()
        {
            //Add new Ph


            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "EXEC [COSXLSX-COLSX_F4_PH] @so_lsx, @tk_dd, @ma_px, @ma_tra_cuu, @dien_giai, @ma_bpht, @ma_dvcs, @ngay_lkh, @ngay_kh1, @ngay_kh2, @ngay_tt1, @ngay_tt2,@mau_sac,@cap_giay,@ngay_maket,@yc_khac,@ma_kh,@ten_kh,@ng_lien_he,@dien_thoai,@gh_time,@gh_lien_he,@gh_dia_chi,@gh_yc_khac";
            cmd.Parameters.Add("@so_lsx", SqlDbType.VarChar).Value = txtSo_lsx.Text.ToString();
            cmd.Parameters.Add("@tk_dd", SqlDbType.VarChar).Value = txttk_dd.Text.ToString();
            cmd.Parameters.Add("@ma_px", SqlDbType.VarChar).Value = txtMapx.Text.ToString();
            cmd.Parameters.Add("@ma_tra_cuu", SqlDbType.VarChar).Value = txtma_tra_cuu.Text.ToString();
            cmd.Parameters.Add("@dien_giai", SqlDbType.NVarChar).Value = txtDien_giai.Text.Trim().ToString();
            cmd.Parameters.Add("@ngay_lkh", SqlDbType.VarChar).Value = Convert.ToDateTime(txtngay_lsx.dValue) == new DateTime() ? "" : String.Format("{0:yyyyMMdd}", txtngay_lsx.Value);
            cmd.Parameters.Add("@ngay_kh1", SqlDbType.VarChar).Value = Convert.ToDateTime(txtNgaybd_kh.dValue) == new DateTime() ? "" : String.Format("{0:yyyyMMdd}", txtNgaybd_kh.Value);
            cmd.Parameters.Add("@ngay_kh2", SqlDbType.VarChar).Value = Convert.ToDateTime(txtNgaykt_kh.dValue) == new DateTime() ? "" : String.Format("{0:yyyyMMdd}", txtNgaykt_kh.Value);
            cmd.Parameters.Add("@ngay_tt1", SqlDbType.VarChar).Value = Convert.ToDateTime(txtNgaybd_tt.dValue) == new DateTime() ? "" : String.Format("{0:yyyyMMdd}", txtNgaybd_tt.Value);
            cmd.Parameters.Add("@ngay_tt2", SqlDbType.VarChar).Value = Convert.ToDateTime(txtNgaykt_tt.dValue) == new DateTime() ? "" : String.Format("{0:yyyyMMdd}", txtNgaykt_tt.Value);
            cmd.Parameters.Add("@mau_sac", SqlDbType.VarChar).Value = txtmau_sac.Text.ToString();
            cmd.Parameters.Add("@cap_giay", SqlDbType.VarChar).Value = txtcap_giay.Text.ToString();
            cmd.Parameters.Add("@ngay_maket", SqlDbType.VarChar).Value = Convert.ToDateTime(txtngay_maket.dValue) == new DateTime() ? "" : String.Format("{0:yyyyMMdd}", txtngay_maket.Value);
            cmd.Parameters.Add("@yc_khac", SqlDbType.VarChar).Value = txtyc_khac.Text.ToString();
            cmd.Parameters.Add("@ma_kh", SqlDbType.NVarChar).Value = txtMa_kh.Text.Trim().ToString();
            cmd.Parameters.Add("@ten_kh", SqlDbType.NVarChar).Value = lblTenkh.Text.Trim().ToString();
            cmd.Parameters.Add("@ng_lien_he", SqlDbType.NVarChar).Value = txtng_lien_he.Text.Trim().ToString();
            cmd.Parameters.Add("@dien_thoai", SqlDbType.NVarChar).Value = txtdien_thoai.Text.Trim().ToString();
            cmd.Parameters.Add("@gh_time", SqlDbType.NVarChar).Value = txtgh_time.Text.ToString();
            cmd.Parameters.Add("@gh_lien_he", SqlDbType.NVarChar).Value = txtgh_lien_he.Text.Trim().ToString();
            cmd.Parameters.Add("@gh_dia_chi", SqlDbType.NVarChar).Value = txtgh_dia_chi.Text.Trim().ToString();
            cmd.Parameters.Add("@gh_yc_khac", SqlDbType.NVarChar).Value = txtgh_yc_khac.Text.Trim().ToString();
            cmd.Parameters.Add("@ma_bpht", SqlDbType.VarChar).Value = txtChonBp.Text.ToString();
            cmd.Parameters.Add("@ma_dvcs", SqlDbType.VarChar).Value = txtma_dvcs.Text.ToString();

            StartUp.SysObj.ExcuteNonQuery(cmd);

            SqlCommand cmd1;
            for (int i = 0; i < dsTmp.Tables[1].DefaultView.Count; i++)
            {
                cmd1 = new SqlCommand();
                cmd1.CommandText = "EXEC [COSXLSX-COLSX_F4_CT] @so_lsx, @ma_hd, @ma_sp, @sl_kh, @sl_sx, @sl_kt, @sl_nhap, @sl_hong, @sl_ll, @ngay_kh1, @ngay_kh2";
                cmd1.Parameters.Add("@so_lsx", SqlDbType.VarChar).Value = txtSo_lsx.Text.ToString();
                cmd1.Parameters.Add("@ma_hd", SqlDbType.VarChar).Value = dsTmp.Tables[1].DefaultView[i]["ma_hd"].ToString();
                cmd1.Parameters.Add("@ma_sp", SqlDbType.VarChar).Value = dsTmp.Tables[1].DefaultView[i]["ma_sp"].ToString();
                cmd1.Parameters.Add("@sl_kh", SqlDbType.Decimal).Value = decimal.Parse(dsTmp.Tables[1].DefaultView[i]["sl_kh"].ToString());
                cmd1.Parameters.Add("@sl_sx", SqlDbType.Decimal).Value = decimal.Parse(dsTmp.Tables[1].DefaultView[i]["sl_sx"].ToString());
                cmd1.Parameters.Add("@sl_kt", SqlDbType.Decimal).Value = decimal.Parse(dsTmp.Tables[1].DefaultView[i]["sl_kt"].ToString());
                cmd1.Parameters.Add("@sl_nhap", SqlDbType.Decimal).Value = decimal.Parse(dsTmp.Tables[1].DefaultView[i]["sl_nhap"].ToString());
                cmd1.Parameters.Add("@sl_hong", SqlDbType.Decimal).Value = decimal.Parse(dsTmp.Tables[1].DefaultView[i]["sl_hong"].ToString());
                cmd1.Parameters.Add("@sl_ll", SqlDbType.Decimal).Value = decimal.Parse(dsTmp.Tables[1].DefaultView[i]["sl_ll"].ToString());
                cmd1.Parameters.Add("@ngay_kh1", SqlDbType.VarChar).Value = string.IsNullOrEmpty(dsTmp.Tables[1].DefaultView[i]["ngay_kh1"].ToString()) ? "" : String.Format("{0:yyyyMMdd}", (DateTime)dsTmp.Tables[1].DefaultView[i]["ngay_kh1"]);
                cmd1.Parameters.Add("@ngay_kh2", SqlDbType.VarChar).Value = string.IsNullOrEmpty(dsTmp.Tables[1].DefaultView[i]["ngay_kh2"].ToString()) ? "" : String.Format("{0:yyyyMMdd}", (DateTime)dsTmp.Tables[1].DefaultView[i]["ngay_kh2"]);
                //cmd1.Parameters.Add("@gc_td1_i", SqlDbType.NVarChar).Value = dsTmp.Tables[1].DefaultView[i]["gc_td1_i"].ToString();

                StartUp.SysObj.ExcuteNonQuery(cmd1);
            }

            SqlCommand cmd2;
            for (int i = 0; i < dsTmp.Tables[2].DefaultView.Count; i++)
            {
                cmd2 = new SqlCommand();
                cmd2.CommandText = "EXEC [COSXLSX-COLSX_F4_CT2] @so_lsx, @ma_hd, @ma_sp, @mau_sac, @cap_giay, @ngay_maket, @yc_khac, @hang_muc, @loai_giay, @k_thuoc1, @k_thuoc2,@so_kem,@qc_in,@sl_g_can,@bh_in,@bh_gc,@tong_giay";
                cmd2.Parameters.Add("@so_lsx", SqlDbType.VarChar).Value = txtSo_lsx.Text.ToString();
                cmd2.Parameters.Add("@ma_hd", SqlDbType.VarChar).Value = dsTmp.Tables[2].DefaultView[i]["ma_hd"].ToString();
                cmd2.Parameters.Add("@ma_sp", SqlDbType.VarChar).Value = dsTmp.Tables[2].DefaultView[i]["ma_sp"].ToString();
                cmd2.Parameters.Add("@mau_sac", SqlDbType.NVarChar).Value = txtmau_sac.Text.ToString();
                cmd2.Parameters.Add("@cap_giay", SqlDbType.NVarChar).Value = txtcap_giay.Text.ToString();
                cmd2.Parameters.Add("@ngay_maket", SqlDbType.VarChar).Value = txtngay_maket.Value;
                cmd2.Parameters.Add("@yc_khac", SqlDbType.NVarChar).Value = txtyc_khac.Text.ToString();
                cmd2.Parameters.Add("@hang_muc", SqlDbType.NVarChar).Value = dsTmp.Tables[2].DefaultView[i]["hang_muc"].ToString();
                cmd2.Parameters.Add("@loai_giay", SqlDbType.NVarChar).Value = dsTmp.Tables[2].DefaultView[i]["loai_giay"].ToString();
                cmd2.Parameters.Add("@k_thuoc1", SqlDbType.Decimal).Value = decimal.Parse(dsTmp.Tables[2].DefaultView[i]["k_thuoc1"].ToString());
                cmd2.Parameters.Add("@k_thuoc2", SqlDbType.Decimal).Value = decimal.Parse(dsTmp.Tables[2].DefaultView[i]["k_thuoc2"].ToString());
                cmd2.Parameters.Add("@so_kem", SqlDbType.Decimal).Value = decimal.Parse(dsTmp.Tables[2].DefaultView[i]["so_kem"].ToString());
                cmd2.Parameters.Add("@qc_in", SqlDbType.NVarChar).Value = dsTmp.Tables[2].DefaultView[i]["qc_in"].ToString();
                cmd2.Parameters.Add("@sl_g_can", SqlDbType.Decimal).Value = decimal.Parse(dsTmp.Tables[2].DefaultView[i]["sl_g_can"].ToString());
                cmd2.Parameters.Add("@bh_in", SqlDbType.Decimal).Value = decimal.Parse(dsTmp.Tables[2].DefaultView[i]["bh_in"].ToString());
                cmd2.Parameters.Add("@bh_gc", SqlDbType.Decimal).Value = decimal.Parse(dsTmp.Tables[2].DefaultView[i]["bh_gc"].ToString());
                cmd2.Parameters.Add("@tong_giay", SqlDbType.Decimal).Value = decimal.Parse(dsTmp.Tables[2].DefaultView[i]["tong_giay"].ToString());
                

                //cmd1.Parameters.Add("@gc_td1_i", SqlDbType.NVarChar).Value = dsTmp.Tables[1].DefaultView[i]["gc_td1_i"].ToString();

                StartUp.SysObj.ExcuteNonQuery(cmd2);
            }

            SqlCommand cmd3;
            for (int i = 0; i < dsTmp.Tables[3].DefaultView.Count; i++)
            {
                cmd3 = new SqlCommand();
                cmd3.CommandText = "EXEC [COSXLSX-COLSX_F4_CT3] @so_lsx, @ma_hd, @ma_sp, @so_bat, @khuon";
                cmd3.Parameters.Add("@so_lsx", SqlDbType.VarChar).Value = txtSo_lsx.Text.ToString();
                cmd3.Parameters.Add("@ma_hd", SqlDbType.VarChar).Value = dsTmp.Tables[3].DefaultView[i]["ma_hd"].ToString();
                cmd3.Parameters.Add("@ma_sp", SqlDbType.VarChar).Value = dsTmp.Tables[3].DefaultView[i]["ma_sp"].ToString();
                cmd3.Parameters.Add("@so_bat", SqlDbType.NVarChar).Value = dsTmp.Tables[3].DefaultView[i]["so_bat"].ToString();
                cmd3.Parameters.Add("@khuon", SqlDbType.NVarChar).Value = dsTmp.Tables[3].DefaultView[i]["khuon"].ToString(); ;



                StartUp.SysObj.ExcuteNonQuery(cmd3);
            }


            for (int i = 0; i < listBpht.Rows.Count; i++)
            {
                if (listBpht.Rows[i]["tag"].ToString() == "True")
                {
                    DataRow new_Row = listBpht_lsx.NewRow();
                    new_Row["so_lsx"] = txtSo_lsx.Text.Trim();
                    new_Row["ma_bpht"] = listBpht.Rows[i]["ma_bpht"];
                    ListFunc.inserRowInDataBase("dmbplsx", new_Row, StartUp.SysObj);

                }
            }

            StartUp.CallGridVouchers(false, StartUp.sStartDate, StartUp.sEndDate, StartUp.sMalsx, StartUp.sMapx);
            this.Close();
        }

        private bool checkValidEdit()
        {
            if (string.IsNullOrEmpty(txtSo_lsx.Text.Trim().ToString()))
            {
                ExMessageBox.Show(1890, StartUp.SysObj, "Chưa nhập số lệnh sản xuất!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                txtSo_lsx.Focus();
                return false;
            }



            //kiem tra lệnh sản xuất có trùng ko
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Select so_lsx From lsxph Where  so_lsx = @so_lsx and rtrim(ltrim(so_lsx)) <> @so_lsx_cu";
            cmd.Parameters.Add("@so_lsx", SqlDbType.Char, 16).Value = txtSo_lsx.Text.ToString();
            cmd.Parameters.Add("@so_lsx_cu", SqlDbType.Char, 16).Value = StartUp.so_lsx_old.Trim();
            DataTable dt = StartUp.SysObj.ExcuteReader(cmd).Tables[0];
            if (dt.Rows.Count > 0)
            {
                ExMessageBox.Show(1895, StartUp.SysObj, "Mã lệnh sản xuất đã có!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                txtSo_lsx.Focus();
                return false;
            }

            DataTable dttmp = ((DataView)GrdCt.DataSource).ToTable();
            dttmp.DefaultView.RowFilter = dsTmp.Tables[1].DefaultView.RowFilter;
            dttmp.DefaultView.Sort = "ma_sp ASC";

            //Check ngay lenh san xuat
            if (!string.IsNullOrEmpty(StartUp.sStartDate.ToString()) && Convert.ToDateTime(txtngay_lsx.dValue) != new DateTime())
            {
                if ((Convert.ToDateTime(txtngay_lsx.dValue) < Convert.ToDateTime(StartUp.sStartDate)))
                {
                    ExMessageBox.Show(1870, StartUp.SysObj, "Ngày lệnh sản xuất không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtngay_lsx.Focus();
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(StartUp.sEndDate.ToString()))
            {
                if ((Convert.ToDateTime(txtngay_lsx.dValue) > Convert.ToDateTime(StartUp.sEndDate)) && Convert.ToDateTime(txtngay_lsx.dValue) != new DateTime())
                {
                    ExMessageBox.Show(1875, StartUp.SysObj, "Ngày lệnh sản xuất không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtngay_lsx.Focus();
                    return false;
                }
            }

            if (dttmp.DefaultView.Count == 0 || string.IsNullOrEmpty(dttmp.DefaultView[0]["ma_sp"].ToString().Trim()))
            {
                ExMessageBox.Show(1865, StartUp.SysObj, "Chưa nhập mã sản phẩm!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                GrdCt.Focus();
                return false;
            }

            if (dttmp.DefaultView.Count > 0)
            {
                for (int i = 0; i < dttmp.DefaultView.Count; i++)
                {
                    if (string.IsNullOrEmpty(dttmp.DefaultView[i]["ma_sp"].ToString().Trim()))
                    {
                        ExMessageBox.Show(1966, StartUp.SysObj, "Chưa nhập mã sản phẩm!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        GrdCt.Focus();
                        return false;
                    }
                }
            }

            //if (dttmp.DefaultView.Count > 0)
            //{
            //    for (int i = 0; i < dttmp.DefaultView.Count - 1; i++)
            //    {
            //        if (dttmp.DefaultView[i]["ma_sp"].ToString().Trim().Equals(dttmp.DefaultView[i + 1]["ma_sp"].ToString().Trim()))
            //        {
            //            ExMessageBox.Show(1967, StartUp.SysObj, "Vào trùng mã sản phẩm!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            //            GrdCt.Focus();
            //            return false;
            //        }
            //    }
            //}

            return true;
        }

        private bool checkValidAddNew()
        {


            //kiem tra the tai san ko dc bỏ trống
            if (string.IsNullOrEmpty(txtSo_lsx.Text.Trim().ToString()))
            {
                ExMessageBox.Show(1890, StartUp.SysObj, "Chưa nhập số lệnh sản xuất!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                txtSo_lsx.Focus();
                return false;
            }

            string sb = SmLib.SysFunc.CheckInValidCode(StartUp.SysObj, txtSo_lsx.Text.Trim());
            if (sb != "")
            {
                ExMessageBox.Show(1940, StartUp.SysObj, "Số lệnh sản xuất không được chứa các ký tự " + "[" + sb + "]" + "!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                txtSo_lsx.SelectAll();
                txtSo_lsx.Focus();
                return false;
            }



            //kiem tra lệnh sản xuất có trùng ko
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Select so_lsx From lsxph Where  so_lsx = @so_lsx";
            cmd.Parameters.Add("@so_lsx", SqlDbType.Char, 16).Value = txtSo_lsx.Text.ToString();
            DataTable dt = StartUp.SysObj.ExcuteReader(cmd).Tables[0];
            if (dt.Rows.Count > 0)
            {
                ExMessageBox.Show(1895, StartUp.SysObj, "Mã lệnh sản xuất đã có!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                txtSo_lsx.Focus();
                return false;
            }

            DataTable dttmp = dsTmp.Tables[1].Copy();
            dttmp.DefaultView.RowFilter = dsTmp.Tables[1].DefaultView.RowFilter;
            dttmp.DefaultView.Sort = "ma_sp ASC";



            //Check ngay lenh san xuat
            if (!string.IsNullOrEmpty(StartUp.sStartDate.ToString()) && Convert.ToDateTime(txtngay_lsx.dValue) != new DateTime())
            {
                if ((Convert.ToDateTime(txtngay_lsx.dValue) < Convert.ToDateTime(StartUp.sStartDate)))
                {
                    ExMessageBox.Show(1905, StartUp.SysObj, "Ngày lệnh sản xuất không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtngay_lsx.Focus();
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(StartUp.sEndDate.ToString()) && Convert.ToDateTime(txtngay_lsx.dValue) != new DateTime())
            {
                if ((Convert.ToDateTime(txtngay_lsx.dValue) > Convert.ToDateTime(StartUp.sEndDate)))
                {
                    ExMessageBox.Show(1910, StartUp.SysObj, "Ngày lệnh sản xuất không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtngay_lsx.Focus();
                    return false;
                }
            }

            //if (dttmp.DefaultView.Count > 0)
            //{
            //    int flag1 = 0;
            //    for (int i = 0; i < dttmp.DefaultView.Count; i++)
            //    {
            //        double iValue = 0;
            //        double.TryParse(dttmp.DefaultView[i]["sl_kh"].ToString(), out iValue);
            //        if (iValue != 0)
            //            flag1 = 1;

            //    }
            //    if (flag1 == 0)
            //    {
            //        ExMessageBox.Show( 1915,StartUp.SysObj, "Chưa vào số lượng sản phẩm!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            //        //oBrowse.frmBrw.oBrowse.Records[_vitri] as DataRecord
            //        GrdCt.Focus();
            //        return false;
            //    }
            //}

            //Kiem tra ma san pham
            if (dttmp.DefaultView.Count == 0)
            {
                ExMessageBox.Show(1900, StartUp.SysObj, "Chưa nhập mã sản phẩm!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                GrdCt.Focus();
                return false;
            }

            if (dttmp.DefaultView.Count > 0)
            {
                for (int i = 0; i < dttmp.DefaultView.Count; i++)
                {
                    if (string.IsNullOrEmpty(dttmp.DefaultView[i]["ma_sp"].ToString().Trim()))
                    {
                        ExMessageBox.Show(1901, StartUp.SysObj, "Chưa nhập mã sản phẩm!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        GrdCt.Focus();
                        return false;
                    }
                }
            }
            //kiem tra san pham có trung ko            

            //if (dttmp.DefaultView.Count > 0)
            //{
            //    for (int i = 0; i < dttmp.DefaultView.Count - 1; i++)
            //    {
            //        if (dttmp.DefaultView[i]["ma_sp"].ToString().Trim().Equals(dttmp.DefaultView[i + 1]["ma_sp"].ToString().Trim()))
            //        {
            //            ExMessageBox.Show(1920, StartUp.SysObj, "Vào trùng mã sản phẩm!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            //            GrdCt.Focus();
            //            return false;
            //        }
            //    }
            //}

            return true;
        }


        private void ConfirmGridView_OnCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void GrdCt_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            if (GrdCt.ActiveCell != null)
            {
                switch (e.Cell.Field.Name)
                {
                    case "ma_sp":
                        {
                            if (e.Editor.Value == null)
                                return;
                            AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                            if (txt.RowResult != null)
                            {
                                e.Cell.Record.Cells["ten_sp"].Value = txt.RowResult["ten_vt"];
                                e.Cell.Record.Cells["ten_sp2"].Value = txt.RowResult["ten_vt2"];


                            }
                            break;
                        }
                    case "ma_sp2":
                        {
                            if (e.Editor.Value == null)
                                return;
                            AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                            if (txt.RowResult != null)
                            {
                                e.Cell.Record.Cells["ten_sp"].Value = txt.RowResult["ten_vt"];
                                e.Cell.Record.Cells["ten_sp2"].Value = txt.RowResult["ten_vt2"];


                            }
                            break;
                        }
                }
            }

        }

        private bool GrdCt_AddNewRecord(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            NewRowCt();
            return true;
        }

        private void GrdCt_RecordDelete(object sender, Infragistics.Windows.DataPresenter.Events.RecordsDeletedEventArgs e)
        {


            //if (isCloseForm == false)
            //{
            //    this.Close();
            //}
            //else
            //{
            GridOkCancel.pnlButton.btnOk.Focus();
            //}
        }

        private void GrdCt_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F4:
                    {
                        DataRecord record = (GrdCt.ActiveRecord as DataRecord);
                        if (record == null || record.Cells["ma_sp"].Value == null || record.Cells["ma_sp"].Value.ToString() == "")
                            return;

                        NewRowCt();
                        GrdCt.ActiveRecord = GrdCt.Records[GrdCt.Records.Count - 1];
                        GrdCt.ActiveCell = (GrdCt.ActiveRecord as DataRecord).Cells["ma_sp"];
                        break;
                    }

                case Key.F8:
                    {
                        if (ExMessageBox.Show(1925, StartUp.SysObj, "Có chắc chắn xóa không?", "Fast Accounting 11 .NET", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                        {
                            return;
                        }

                        DataRecord record = (GrdCt.ActiveRecord as DataRecord);
                        if (record != null)
                        {
                            //MessageBox.Show(GrdCt.ActiveCell.Field.Index.ToString());
                            int indexRecord = 0, indexCell = 0;
                            Cell cell = GrdCt.ActiveCell;
                            if (record.Index == 0)
                            {
                                if (GrdCt.Records.Count == 1)
                                    GrdCt_AddNewRecord(null, null);
                            }
                            else if (record.Index == GrdCt.Records.Count - 1)
                            {
                                //GrdCt.ActiveCell = (GrdCt.Records[record.Index - 1] as DataRecord).Cells[/*record.Index*/0];
                                indexRecord = record.Index - 1;
                            }
                            indexCell = GrdCt.ActiveCell == null ? 0 : GrdCt.ActiveCell.Field.Index;
                            GrdCt.ExecuteCommand(DataPresenterCommands.EndEditModeAndDiscardChanges);

                            if (indexCell >= 0)
                            {
                                SqlCommand cmd1 = new SqlCommand();
                                cmd1.CommandText = "Delete lsxct Where id = @id";
                                cmd1.Parameters.Add("@id", SqlDbType.BigInt).Value = dsTmp.Tables[1].DefaultView[record.Index]["id"];
                                StartUp.SysObj.ExcuteNonQuery(cmd1);

                                dsTmp.Tables[1].Rows.Remove(dsTmp.Tables[1].DefaultView[record.Index].Row);
                                dsTmp.Tables[1].AcceptChanges();

                                StartUp.CallGridVouchers(false, StartUp.sStartDate, StartUp.sEndDate, StartUp.sMalsx, StartUp.sMapx);

                                if (GrdCt.Records.Count > 0)
                                {
                                    GrdCt.ActiveRecord = GrdCt.Records[GrdCt.Records.Count - 1];
                                }
                            }
                        }

                    }
                    break;
                default:
                    break;
            }
        }

        private void GrdCt_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.N) && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                NewRowCt();
                GrdCt.ActiveRecord = GrdCt.Records[GrdCt.Records.Count - 1];

            }
        }



        void NewRowCt()
        {
            try
            {
                DataRow NewRecord = dsTmp.Tables[1].NewRow();
                NewRecord["so_lsx"] = g_so_lsx;
                NewRecord["sl_kh"] = 0;
                NewRecord["sl_sx"] = 0;
                NewRecord["sl_kt"] = 0;
                NewRecord["sl_nhap"] = 0;
                NewRecord["sl_hong"] = 0;
                NewRecord["sl_ll"] = 0;
                dsTmp.Tables[1].Rows.Add(NewRecord);
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        void NewRowCt2()
        {
            try
            {
                DataRow NewRecord = dsTmp.Tables[2].NewRow();
                NewRecord["so_lsx"] = g_so_lsx;
                NewRecord["k_thuoc1"] = 0;
                NewRecord["k_thuoc2"] = 0;
                NewRecord["tong_giay"] = 0;
                NewRecord["sl_g_can"] = 0;
                NewRecord["bh_in"] = 0;
                NewRecord["bh_gc"] = 0;
                dsTmp.Tables[2].Rows.Add(NewRecord);
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        void NewRowCt3()
        {
            try
            {
                DataRow NewRecord = dsTmp.Tables[3].NewRow();
                NewRecord["so_lsx"] = g_so_lsx;
                NewRecord["so_bat"] = 0;
                dsTmp.Tables[3].Rows.Add(NewRecord);
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        private void Form_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (GrdCt.ActiveCell != null && GrdCt.ActiveCell.Record.Cells[0].IsActive == true && (GrdCt.ActiveCell.Record.Cells[0].Value == DBNull.Value || string.IsNullOrEmpty(GrdCt.ActiveCell.Record.Cells[0].Value.ToString())))
            {
                GridOkCancel.pnlButton.btnCancel.Focus();
                e.Cancel = true;
                isCloseForm = true;
            }
            else
            {
                e.Cancel = false;
            }
        }

        private void GridOkCancel_GotFocus(object sender, RoutedEventArgs e)
        {
            GridOkCancel.pnlButton.GotFocus += new RoutedEventHandler(pnlButton_GotFocus);
        }

        void pnlButton_GotFocus(object sender, RoutedEventArgs e)
        {
            isCloseForm = true;
        }

        private void txtMapx_GotFocus(object sender, RoutedEventArgs e)
        {
            txtMapx.Text = txtMapx.Text.Trim();
            //txtMapx.SelectAllOnFocus .SelectAll();
        }



        private void txtChonBp_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            int iValue = -1;
            if (Int32.TryParse(textBox.Text, out iValue) == false)
            {
                TextChange textChange = e.Changes.ElementAt<TextChange>(0);
                int iAddedLength = textChange.AddedLength;
                int iOffset = textChange.Offset;
                textBox.Text = textBox.Text.Remove(iOffset, iAddedLength);
            }
            if (iValue == 0 || iValue == 1)
            {
                textBox.Text = iValue.ToString();
                textBox.SelectAll();
                temp = iValue.ToString();
            }
            else
            {
                textBox.Text = temp;
                textBox.SelectAll();
            }
        }

        private void txtChonBp_LostFocus(object sender, RoutedEventArgs e)
        {
            temp = txtChonBp.Text.Trim();
            if (temp == "1")
            {
                ChonBPHT _bpht = new ChonBPHT();
                _bpht.ShowDialog();
                if (Co_BPHT_Ko() == false)
                {
                    txtChonBp.Text = "0";
                    tblList_Bp.Text = "";
                }
                else
                    List_YTCP();
            }
            else
            {
                for (int j = 0; j < listBpht.Rows.Count; j++)
                {
                    listBpht.Rows[j]["tag"] = "False";
                }
                tblList_Bp.Text = "";

            }
        }
        private bool Co_BPHT_Ko()
        {
            for (int j = 0; j < listBpht.Rows.Count; j++)
            {
                if ((bool)listBpht.Rows[j]["tag"] == true)
                {
                    return true;
                }
            }
            return false;
        }

        public void Edit_BPHT()
        {
            listBpht_lsx.DefaultView.RowFilter = "so_lsx = '" + StartUp.so_lsx_old.Trim() + "'";
            for (int i = 0; i < listBpht_lsx.Rows.Count; i++)
            {
                for (int j = 0; j < listBpht.Rows.Count; j++)
                {
                    if (listBpht.Rows[j]["ma_bpht"].ToString().Trim() == listBpht_lsx.Rows[i]["ma_bpht"].ToString().Trim() && listBpht_lsx.Rows[i]["so_lsx"].ToString().Trim() == StartUp.so_lsx_old.Trim())
                        listBpht.Rows[j]["tag"] = 1;
                }
            }
        }

        public void GetDmbplsx()
        {
            cmd.CommandText = "Exec getbplsx ";
            ds = StartUp.SysObj.ExcuteReader(cmd);
            listBpht = ds.Tables[1].Copy();
            listBpht_lsx = ds.Tables[0].Copy();
        }


        private void List_YTCP()
        {
            string list_ma_bpht = "";
            for (int i = 0; i < listBpht.Rows.Count; i++)
            {
                if (listBpht.Rows[i]["tag"].ToString() == "True")
                {
                    list_ma_bpht += " ," + listBpht.Rows[i]["ma_bpht"].ToString().Trim();
                }
            }
            if (list_ma_bpht != "")
                list_ma_bpht = list_ma_bpht.Substring(2, list_ma_bpht.Length - 2);
            tblList_Bp.Text = list_ma_bpht;
        }

        private void txtma_dvcs_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtma_dvcs.RowResult == null)
                tblten_ma_dvcs.Text = "";
            else
                tblten_ma_dvcs.Text = StartUp.M_LAN.Equals("V") ? txtma_dvcs.RowResult["ten_dvcs"].ToString() : txtma_dvcs.RowResult["ten_dvcs2"].ToString();
        }

        private void txtMapx_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtMapx.RowResult == null)
                lblTenpx.Text = "";
            else
                lblTenpx.Text = StartUp.M_LAN.Equals("V") ? (txtMapx.RowResult["ten_px"]).ToString() : (txtMapx.RowResult["ten_px2"]).ToString();
        }



        private void txttk_dd_LostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txttk_dd.RowResult == null)
                tbltk_dd.Text = "";
            else
                tbltk_dd.Text = StartUp.M_LAN.Equals("V") ? (txttk_dd.RowResult["ten_tk"]).ToString() : (txttk_dd.RowResult["ten_tk2"]).ToString();
        }

        private void txtso_hd2_LostFocus(object sender, RoutedEventArgs e)
        {
            string so_hd2 = txtSo_hd2.Text.Trim();


            if (so_hd2 != "")
            {
                get_hd2();
            }
        }

        #region get_hd2
        void get_hd2()
        {
            SqlCommand cmd = new SqlCommand("exec [dbo].[AAA_Inctpxd-gethm] @ngay_hd2, @so_hd2");
            cmd.Parameters.Add("@ngay_hd2", SqlDbType.VarChar, 50).Value = String.Format("{0:yyyyMMdd}", StartUp.DsTrans.Tables[0].DefaultView[0]["ngay_hd2"]);
            cmd.Parameters.Add("@so_hd2", SqlDbType.VarChar, 50).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["so_hd2"];



            int count = StartUp.DsTrans.Tables[1].DefaultView.Count;
            for (int i = 0; i < count; i++)
            {
                StartUp.DsTrans.Tables[1].DefaultView.Delete(0);
            }

            dsReport = StartUp.SysObj.ExcuteReader(cmd);

            for (int i = 0; i < dsReport.Tables[0].DefaultView.Count; i++)
            {
                DataRow rowHdm = dsReport.Tables[0].DefaultView[i].Row;
                DataRow NewRecord = StartUp.DsTrans.Tables[1].NewRow();

                DataTable dt1 = dsReport.Tables[0].Clone();
                dt1.Rows.Add(rowHdm.ItemArray);
                DataTable dt2 = StartUp.DsTrans.Tables[1].Clone();
                dt2.Merge(dt1, true, MissingSchemaAction.Ignore);

                //NewRecord.ItemArray = rowHdm.ItemArray;
                if (dt2.Rows.Count > 0)
                {
                    NewRecord.ItemArray = dt2.Rows[0].ItemArray;
                }

                decimal so_luong = 0;
                decimal.TryParse(rowHdm["so_luong"].ToString(), out so_luong);

                NewRecord["sl_td1_i"] = rowHdm["so_luong"];
                //NewRecord["gc_td1_i"] = rowHdm["gc_td1_i"];
                NewRecord["ma_phi_i"] = rowHdm["ma_phi_i"];
                NewRecord["ma_hd_i"] = rowHdm["ma_hd"];
                NewRecord["ma_nx_i"] = rowHdm["tk_gv"]; ;
                NewRecord["so_luong"] = 0;
                NewRecord["gia_ton"] = 1;
                NewRecord["stt_rec"] = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"];

                int Stt_rec0 = 0, Stt_rec0ct = 0;

                Stt_rec0 = Stt_rec0ct;
                Stt_rec0++;

                StartUp.DsTrans.Tables[1].Rows.Add(NewRecord);
            }

            if (dsReport.Tables[1].DefaultView.Count > 0)
            {
                DataRow[] founds = dsReport.Tables[1].Select("stt_rec='" + dsReport.Tables[1].DefaultView[0]["stt_rec"].ToString() + "'");
                if (founds.Length > 0)
                {
                    //StartUp.DsTrans.Tables[0].Rows[iRow]["ma_kh"] = founds[0]["ma_kh"];
                    //StartUp.DsTrans.Tables[0].Rows[iRow]["dia_chi"] = founds[0]["dia_chi"];
                    ////StartUp.DsTrans.Tables[0].Rows[iRow]["ma_so_thue"] = founds[0]["ma_so_thue"];
                    //StartUp.DsTrans.Tables[0].Rows[iRow]["dien_giai"] = founds[0]["dien_giai"];
                    //StartUp.DsTrans.Tables[0].Rows[iRow]["ngay_ct"] = founds[0]["ngay_ct"];
                    //StartUp.DsTrans.Tables[0].Rows[iRow]["ngay_lct"] = founds[0]["ngay_lct"];
                    //StartUp.DsTrans.Tables[0].Rows[iRow]["ong_ba"] = founds[0]["ong_ba"];
                    ////StartUp.DsTrans.Tables[0].Rows[iRow]["ma_bp"] = founds[0]["ma_bp"];
                    ////StartUp.DsTrans.Tables[0].Rows[iRow]["t_so_luong"] = SumFunction(StartUp.DsTrans.Tables[1], "so_luong", 0);
                    ////StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt2"] = SumFunction(StartUp.DsTrans.Tables[1], "tien_nt2", 0);
                    ////StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien2"] = SumFunction(StartUp.DsTrans.Tables[1], "tien2", 0);
                }


            }

        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                FrmAA_SOCTHDAHdm form = new FrmAA_SOCTHDAHdm();
                form.txtMa_kh.Text = "";
                form.tblTen_kh.Text = "";
                form.ShowDialog();
                if (form.isOk)
                {
                    form.dsHdm.Tables[0].DefaultView.RowFilter = "";
                    form.dsHdm.Tables[1].DefaultView.RowFilter = "";




                    var selecteds = form.dsHdm.Tables[0].Select("tag=true");
                    if (selecteds.Length == 0)
                        return;

                    

                    int count = dsTmp.Tables[1].DefaultView.Count;
                    for (int i = 0; i < count; i++)
                    {
                        dsTmp.Tables[1].DefaultView.Delete(0);
                    }
                    dsTmp.Tables[1].AcceptChanges();

                    int count3 = dsTmp.Tables[3].DefaultView.Count;
                    for (int i = 0; i < count3; i++)
                    {
                        dsTmp.Tables[3].DefaultView.Delete(0);
                    }
                    dsTmp.Tables[3].AcceptChanges();
                    //string ma_ntPH = txtMa_nt.Text.ToUpper();
                    string ma_ntHD = form.dsHdm.Tables[0].DefaultView[0]["ma_nt"].ToString().ToUpper();


                    for (int j = 0; j < selecteds.Length; j++)
                    {
                        DataRow rowHdmPH = selecteds[j];

                        string stt_rec = rowHdmPH["stt_rec"].ToString().Trim();
                        //Mã khách hàng
                        txtMa_kh.Text = rowHdmPH["ma_kh"].ToString();
                        lblTenkh.Text = rowHdmPH["ten_kh"].ToString();
                        txtng_lien_he.Text = rowHdmPH["ong_ba"].ToString();
                        //Làm tương tự với địa chỉ
                        DataRow[] detailRows = form.dsHdm.Tables[1].Select("stt_rec='" + stt_rec + "'");

                        for (int i = 0; i < detailRows.Length; i++)
                        {
                            DataRow rowHdmct = detailRows[i];

                            DataRow NewRecord = dsTmp.Tables[1].NewRow();

                            MegreRow(rowHdmct, NewRecord);

                            decimal tl_ck = 0;
                            decimal.TryParse(rowHdmct["tl_ck"].ToString(), out tl_ck);

                            decimal so_luong = 0;
                            decimal.TryParse(rowHdmct["so_luong"].ToString(), out so_luong);
                            //trường hợp ma_nt của phiếu và hợp đồng giống nhau

                            NewRecord["so_lsx"] = g_so_lsx;
                            NewRecord["ma_sp"] = rowHdmct["ma_vt"];
                            NewRecord["ten_sp"] = rowHdmct["ten_vt"];
                            NewRecord["so_po"] = rowHdmPH["so_po"].ToString();
                            //NewRecord["gc_td1_i"] = rowHdmct["gc_td1_i"];
                            NewRecord["sl_kh"] = so_luong;
                            NewRecord["sl_sx"] = so_luong;
                            NewRecord["sl_kt"] = so_luong;
                            NewRecord["sl_nhap"] = 0;
                            NewRecord["sl_hong"] = 0;
                            NewRecord["sl_ll"] = 0;
                            dsTmp.Tables[1].Rows.Add(NewRecord);
                        
                        }

                        DataRow[] detailRows3 = form.dsHdm.Tables[1].Select("stt_rec='" + stt_rec + "'");

                        for (int k = 0; k < detailRows.Length; k++)
                        {
                            DataRow rowHdmct = detailRows[k];

                            DataRow NewRecord = dsTmp.Tables[3].NewRow();

                            MegreRow(rowHdmct, NewRecord);

                            decimal tl_ck = 0;
                            decimal.TryParse(rowHdmct["tl_ck"].ToString(), out tl_ck);

                            decimal so_luong = 0;
                            decimal.TryParse(rowHdmct["so_luong"].ToString(), out so_luong);
                            //trường hợp ma_nt của phiếu và hợp đồng giống nhau

                            NewRecord["so_lsx"] = g_so_lsx;
                            NewRecord["ma_sp"] = rowHdmct["ma_vt"];
                            NewRecord["ten_sp"] = rowHdmct["ten_vt"];
                            NewRecord["yc_gc"] = rowHdmct["chat_lieu"];

                            dsTmp.Tables[3].Rows.Add(NewRecord);

                        }
                    }
                    //--lấy PH
  

                }

            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }

        }


        /// <summary>
        /// Sao chép giá trị từ fromRow sang toRow ( Chỉ sao chép các cột có cả hai bên)
        /// </summary>
        /// <param name="fromRow">Dòng nguồn</param>
        /// <param name="toRow"> Dòng đích</param>
        public void MegreRow(DataRow fromRow,DataRow toRow)
        {
            foreach(DataColumn col in toRow.Table.Columns)
            {
                if (fromRow.Table.Columns.Contains(col.ColumnName))
                    toRow[col] = fromRow[col.ColumnName];
            }
        }

        private void txtNgay_hd_LostFocus(object sender, RoutedEventArgs e)
        {

        }



        private void GrdCt2_RecordDelete(object sender, Infragistics.Windows.DataPresenter.Events.RecordsDeletedEventArgs e)
        {

        }

        private void GrdCt2_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            if (GrdCt2.ActiveCell != null)
            {
                switch (e.Cell.Field.Name)
                {
                    #region ma_sp
                    case "ma_sp":
                        {
                            if (e.Editor.Value == null)
                                return;
                            AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                            if (txt.RowResult != null)
                            {
                                e.Cell.Record.Cells["ten_sp"].Value = txt.RowResult["ten_vt"];
                                e.Cell.Record.Cells["ten_sp2"].Value = txt.RowResult["ten_vt2"];


                            }
                            break;
                        }
                    #endregion

                    #region sl_g_can
                    case "sl_g_can":
                        {
                            if (e.Cell.IsDataChanged)
                            {
                                if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                e.Cell.Record.Cells["sl_g_can"].Value = 0;

                            decimal sl_g_can = ParseDecimal(e.Cell.Record.Cells["sl_g_can"].Value, 0);
                            decimal bh_in = ParseDecimal(e.Cell.Record.Cells["bh_in"].Value, 0);
                            decimal bh_gc = ParseDecimal(e.Cell.Record.Cells["bh_gc"].Value, 0);


                            decimal tong_giay = sl_g_can + bh_in + bh_gc;

                            e.Cell.Record.Cells["tong_giay"].Value = tong_giay;
                            }
                            break;
                        }
                    #endregion

                    #region Bh In
                    case "bh_in":
                        {
                            if (e.Cell.IsDataChanged)
                            {
                                if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                    e.Cell.Record.Cells["bh_in"].Value = 0;

                                decimal sl_g_can = ParseDecimal(e.Cell.Record.Cells["sl_g_can"].Value, 0);
                                decimal bh_in = ParseDecimal(e.Cell.Record.Cells["bh_in"].Value, 0);
                                decimal bh_gc = ParseDecimal(e.Cell.Record.Cells["bh_gc"].Value, 0);


                                decimal tong_giay = sl_g_can + bh_in + bh_gc;

                                e.Cell.Record.Cells["tong_giay"].Value = tong_giay;
                            }
                            break;
                        }
                    #endregion

                    #region Bh Gc
                    case "bh_gc":
                        {
                            if (e.Cell.IsDataChanged)
                            {
                                if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                    e.Cell.Record.Cells["bh_gc"].Value = 0;

                                decimal sl_g_can = ParseDecimal(e.Cell.Record.Cells["sl_g_can"].Value, 0);
                                decimal bh_in = ParseDecimal(e.Cell.Record.Cells["bh_in"].Value, 0);
                                decimal bh_gc = ParseDecimal(e.Cell.Record.Cells["bh_gc"].Value, 0);


                                decimal tong_giay = sl_g_can + bh_in + bh_gc;

                                e.Cell.Record.Cells["tong_giay"].Value = tong_giay;
                            }
                            break;
                        }
                        #endregion
                }
            }
        }

        private decimal ParseDecimal(object obj, decimal defaultvalue)
        {

            decimal ketqua = 0;
            decimal.TryParse(obj != null ? obj.ToString() : defaultvalue.ToString(), out ketqua);
            return ketqua;

            //throw new NotImplementedException();
        }

        private bool GrdCt2_AddNewRecord(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            NewRowCt2();
            return true;
        }

        private bool GrdCt3_AddNewRecord(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            NewRowCt3();
            return true;
        }

        private void txtMakh_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

        }

        private void GrdCt3_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            if (GrdCt2.ActiveCell != null)
            {
                switch (e.Cell.Field.Name)
                {
                    case "ma_sp":
                        {
                            if (e.Editor.Value == null)
                                return;
                            AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                            if (txt.RowResult != null)
                            {
                                e.Cell.Record.Cells["ten_sp"].Value = txt.RowResult["ten_vt"];
                                e.Cell.Record.Cells["ten_sp2"].Value = txt.RowResult["ten_vt2"];


                            }
                            break;
                        }
                }
            }
        }

        private void txtManhomsp_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //if (txtnhom_sp.RowResult == null)
            //    lblnhom_sp.Text = "";
            //else
            //    lblnhom_sp.Text = StartUp.M_LAN.Equals("V") ? (txtnhom_sp.RowResult["ten_td"]).ToString() : (txtnhom_sp.RowResult["ten_td2"]).ToString();
        }

        private void txtMatd_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtMa_td2.RowResult == null)
                lblTennhomsp.Text = "";
            else
                lblTennhomsp.Text = StartUp.M_LAN.Equals("V") ? (txtMa_td2.RowResult["ten_td"]).ToString() : (txtMa_td2.RowResult["ten_td2"]).ToString();
        }
    }
}
