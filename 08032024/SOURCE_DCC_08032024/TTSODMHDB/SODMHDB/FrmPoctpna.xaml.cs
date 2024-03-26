
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Infragistics.Windows.DataPresenter;
using Sm.Windows.Controls;
using SmDefine;
using SmLib;
using SmVoucherLib;
using System.Data.SqlClient;

namespace TTSODMHDB
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FrmPoctpna : SmVoucherLib.FormTrans
    {
        public static int iRow = 0;
        public static int OldiRow = 0;
        public string Old_ma_kho = string.Empty;
        string ma_hd;

        public static CodeValueBindingObject IsInEditMode;
        CodeValueBindingObject Voucher_Ma_nt0;
        CodeValueBindingObject Voucher_Lan0;
        CodeValueBindingObject IsCheckedSua_tien;
        CodeValueBindingObject Ty_Gia_ValueChange;

        //Lưu lại dữ liệu khi thêm sửa
        private DataSet DsVitual;
        DataSet dsCheckData;

        public FrmPoctpna()
        {
            InitializeComponent();
            this.BindingSysObj = StartUp.SysObj;
            Loaded += new RoutedEventHandler(FormTrans_Loaded);
            C_QS = txtMa_qs;
            C_NgayHT = txtNgay_ct;
            C_Ma_nt = cbMa_nt;
            C_So_ct = txtSo_ct;
        }

        #region load form
        void FormTrans_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.BindingSysObj = StartUp.SysObj;
                StartUp.M_AR_CK = Convert.ToInt16(BindingSysObj.GetOption(stt_mau_temlate.ToString(), "M_AR_CK"));
                currActionTask = ActionTask.View;

                //Gan1 iRow ở phiếu cuối cùng
                if (StartUp.DsTrans.Tables[0].Rows.Count > 1)
                    iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;

                IsInEditMode = (CodeValueBindingObject)FormMain.FindResource("IsInEditMode");
                Voucher_Ma_nt0 = (CodeValueBindingObject)FormMain.FindResource("Voucher_Ma_nt0");
                Voucher_Lan0 = (CodeValueBindingObject)FormMain.FindResource("Voucher_Lan0");
                IsCheckedSua_tien = (CodeValueBindingObject)FormMain.FindResource("IsCheckedSua_tien");
                Ty_Gia_ValueChange = (CodeValueBindingObject)FormMain.FindResource("Ty_Gia_ValueChange");

                string M_CDKH13 = SysO.GetOption("M_CDKH13").ToString().Trim();
                if (M_CDKH13 != "1")
                    txtso_du_kh.Visibility = tblso_du_kh.Visibility = Visibility.Hidden;

                //Binding EditMode cho FormTrans
                Binding bind = new Binding("Value");
                bind.Source = IsInEditMode;
                bind.Mode = BindingMode.OneWay;
                this.SetBinding(FormTrans.IsEditModeProperty, bind);

                //Gán ngôn ngữ messagebox
                M_LAN = StartUp.M_LAN;
                GrdCt.Lan = StartUp.M_LAN;
                if (StartUp.M_BP_BH == "1")
                    txtMa_bp.IsTabStop = true;
                LanguageProvider.Language = StartUp.M_LAN;

                //Them cac truong tu do
                SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, GrdCt, StartUp.Ma_ct, 1);

                if (StartUp.DsTrans.Tables[0].Rows.Count > 0)
                {
                    LoadData();
                    //Xét lại các Field khi thay đổi record hiển thị
                    IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
                    IsCheckedSua_tien.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"].ToString() == "1");
                    Voucher_Lan0.Value = M_LAN.Trim().Equals("V");
                }

                Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
                //Update số dư vật tư
                UpdateTonKho();
                //Load số sư khách hàng
                loaddataDu13();
                //Xử lý lưu mã kho của phiếu trước đó
                if (StartUp.DsTrans.Tables[1].DefaultView.Count > 0)
                {
                    Old_ma_kho = StartUp.DsTrans.Tables[1].DefaultView[0]["ma_kho_i"].ToString();
                }
                SetFocusToolbar();
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        #region LoadData()
        private void LoadData()
        {
            //RowFilter lại theo stt_rec
            StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
            StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";

            this.GrdLayout00.DataContext = StartUp.DsTrans.Tables[0].DefaultView;
            this.GrdLayout10.DataContext = StartUp.DsTrans.Tables[0].DefaultView;
            this.GrdLayout20.DataContext = StartUp.DsTrans.Tables[0].DefaultView;
            this.GrdLayout21.DataContext = StartUp.DsTrans.Tables[0].DefaultView;
            //GroupBox Tổng cộng: số lượng
            this.GrdLayout22.DataContext = StartUp.DsTrans.Tables[0].DefaultView;
            //Tổng chi phí trong tab Chi phí
         //   this.GrdTongChiPhi.DataContext = StartUp.DsTrans.Tables[0].DefaultView;

            //GrdLayoutNT.DataContext = StartUp.DsTrans.Tables[0].DefaultView;
            //Nạp dữ liệu cho Grid hàng hóa, chi phí và hd thuế
            this.GrdCt.DataSource = StartUp.DsTrans.Tables[1].DefaultView;
         //   this.GrdCp.DataSource = StartUp.DsTrans.Tables[1].DefaultView;

            //Nạp dữ liệu cho trạng thái chứng từ
            txtStatus.ItemsSource = StartUp.tbStatus.DefaultView;

            if (StartUp.tbStatus.DefaultView.Count == 1)
            {
                txtStatus.IsEnabled = false;
            }
        }
        #endregion

        #endregion

        #region V_Dau
        private void V_Dau()
        {
            if (StartUp.DsTrans.Tables[0].Rows.Count >= 2)
            {
                iRow = 1;
            }
            else
            {
                iRow = 0;
            }
            StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
            StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";

            //IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
            Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
            Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
        }
        #endregion

        #region V_Truoc
        private void V_Truoc()
        {
            if (iRow > 1)
            {
                iRow--;
                StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
                StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";

                //IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
                Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
            }
        }
        #endregion

        #region V_Sau
        private void V_Sau()
        {
            if (iRow < StartUp.DsTrans.Tables[0].Rows.Count - 1)
            {
                iRow++;
                StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
                StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";

                //IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
                Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
            }
        }
        #endregion

        #region V_Cuoi
        private void V_Cuoi()
        {
            iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;
            StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
            StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";

            //IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
            Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
            Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
        }

        #endregion

        #region V_Moi
        private void V_Moi()
        {
            try
            {
                string newSttRec = DataProvider.NewTrans(StartUp.SysObj, StartUp.Ma_ct, StartUp.Ws_Id);
           
                currActionTask = ActionTask.Add;
                if (!string.IsNullOrEmpty(newSttRec))
                {
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background
                    , new Action(() =>
                    {
                        txtMa_kh.IsFocus = true;
                    }));
                    DsVitual = StartUp.DsTrans.Copy();

                    //Them moi dong trong Ph
                    DataRow NewRecord = StartUp.DsTrans.Tables[0].NewRow();
                    NewRecord["stt_rec"] = newSttRec;
                    NewRecord["ma_ct"] = StartUp.Ma_ct;

                    if (SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, txtNgay_ct.dValue))
                    {
                        NewRecord["ngay_ct"] = txtNgay_ct.dValue.Date;
                    }
                    else
                    {
                        NewRecord["ngay_ct"] = DateTime.Now.Date;
                    }

                    NewRecord["status"] = StartUp.DmctInfo["ma_post"];
                    //NewRecord["ma_nt"] = string.IsNullOrEmpty(cbMa_nt.Text) ?  StartUp.M_ma_nt0 : cbMa_nt.Text;
                    if (StartUp.DsTrans.Tables[0].Rows.Count == 1)
                    {
                        NewRecord["ma_nt"] = StartUp.DmctInfo["ma_nt"];
                        NewRecord["ma_qs"] = GetDMQS(BindingSysObj, StartUp.Ma_ct, Convert.ToDateTime(NewRecord["ngay_ct"]), StartUp.M_User_Id);

                    }
                    else
                    {
                        NewRecord["ma_nt"] = StartUp.DsTrans.Tables[0].Rows[iRow]["ma_nt"];
                        NewRecord["ma_qs"] = GetDMQS(BindingSysObj, StartUp.Ma_ct, Convert.ToDateTime(NewRecord["ngay_ct"]),
                            StartUp.M_User_Id, StartUp.DsTrans.Tables[0].Rows[iRow]["ma_qs"].ToString().Trim());
                    }
                    NewRecord["sua_tien"] = 0;
                    if (NewRecord["ma_nt"].ToString().Trim().Equals(StartUp.M_ma_nt0.Trim()))
                    {
                        NewRecord["ty_giaf"] = 1;
                    }
                    else
                    {
                        NewRecord["ty_giaf"] = StartUp.GetRates(NewRecord["ma_nt"].ToString().Trim(), Convert.ToDateTime(NewRecord["ngay_ct"]).Date);
                    }
                    NewRecord["status"] = StartUp.DmctInfo["ma_post"];
                    NewRecord["t_ck_nt"] = 0;
                    NewRecord["t_ck"] = 0;
                    NewRecord["t_tien"] = 0;
                    NewRecord["t_tien_nt"] = 0;
                    NewRecord["t_tien2"] = 0;
                    NewRecord["t_tien_nt2"] = 0;
                    NewRecord["t_thue_nt"] = 0;
                    NewRecord["t_thue"] = 0;
                    NewRecord["t_tt_nt"] = 0;
                    NewRecord["t_tt"] = 0;
                    NewRecord["t_so_luong"] = 0;

                    NewRecord["t_sau_ck_nt"] = 0;
                    NewRecord["t_sau_ck"] = 0;

                    StartUp.DsTrans.Tables[0].Rows.Add(NewRecord);

                    StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";
                    StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";


                    //Them moi dong trong CT
                    NewRowCt();

                    //Refresh lai form
                    StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";
                    StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";


                    OldiRow = iRow;
                    iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;
                    IsInEditMode.Value = true;

                    TabInfo.SelectedIndex = 0;
                    ChkSuaTien.IsChecked = false;
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        #endregion

        #region V_Copy
        private void V_Copy()
        {
            if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim()))
                return;
            currActionTask = ActionTask.Copy;
            FrmPoctpnaCopy _formcopy = new FrmPoctpnaCopy();
            _formcopy.Closed += new EventHandler(_formcopy_Closed);
            _formcopy.ShowDialog();
          
        }
        #endregion

        #region _formcopy_Closed
        void _formcopy_Closed(object sender, EventArgs e)
        {
            if (FrmPoctpnaCopy.isCopy == true)
            {
                string newSttRec = DataProvider.NewTrans(StartUp.SysObj, StartUp.Ma_ct, StartUp.Ws_Id);
                if (!string.IsNullOrEmpty(newSttRec))
                {
                    DsVitual = StartUp.DsTrans.Copy();
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background
                    , new Action(() =>
                    {
                        txtMa_kh.IsFocus = true;
                    }));
                    //Them moi dong trong Ph
                    DataRow NewRecord = StartUp.DsTrans.Tables[0].NewRow();
                    //copy dữ liệu từ row được chọn copy cho row mới
                    NewRecord.ItemArray = StartUp.DsTrans.Tables[0].Rows[iRow].ItemArray;
                    //gán lại stt_rec, ngày ct
                    NewRecord["stt_rec"] = newSttRec;
                    NewRecord["ngay_ct"] = FrmPoctpnaCopy.ngay_ct;
                        NewRecord["ngay_lct"] = FrmPoctpnaCopy.ngay_ct;
                    NewRecord["t_thue_nt"] = 0;
                    NewRecord["t_thue"] = 0;

                    NewRecord["ma_qs"] = GetDMQS(BindingSysObj, StartUp.Ma_ct, Convert.ToDateTime(NewRecord["ngay_ct"]),
                            StartUp.M_User_Id, NewRecord["ma_qs"].ToString().Trim());
                    if (NewRecord["ma_qs"].ToString().Trim() != "")
                        NewRecord["so_ct"] = GetNewSoct(StartUp.SysObj, NewRecord["ma_qs"].ToString());
                    else
                        NewRecord["so_ct"] = "";
                    NewRecord["so_cttmp"] = NewRecord["so_ct"];

                    StartUp.DsTrans.Tables[0].Rows.Add(NewRecord);

                    //add các row trong GrdCp
                    if (StartUp.DsTrans.Tables[1].DefaultView.Count > 0)
                    {
                        //lấy các rowfilter trong grdcp
                        DataRow[] _row = StartUp.DsTrans.Tables[1].Select("stt_rec='" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'");
                        DataRow NewCtRecord;
                        foreach (DataRow dr in _row)
                        {
                            //add 
                            NewCtRecord = StartUp.DsTrans.Tables[1].NewRow();
                            NewCtRecord.ItemArray = dr.ItemArray;
                            NewCtRecord["stt_rec"] = newSttRec;
                            StartUp.DsTrans.Tables[1].Rows.Add(NewCtRecord);
                        }
                    }
                    OldiRow = iRow;
                    iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;
                    //load lại form
                    StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";
                    StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";

                    IsInEditMode.Value = true;
                    IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
                }
            }
        }
        #endregion

        bool KiemTraCoPhatSinh()
        {
            var cmd = new SqlCommand("select so_ct from ct00 where rtrim(ltrim(ma_hd)) = @ma_hd or rtrim(ltrim(ma_hdm)) = @ma_hd");
            cmd.Parameters.Add("@ma_hd", SqlDbType.Char).Value = ma_hd;
            if (StartUp.SysObj.ExcuteScalar(cmd) != null)
                return true;

            cmd.CommandText = "select so_ct from ct70 where rtrim(ltrim(ma_hd)) = @ma_hd or rtrim(ltrim(ma_hdm)) = @ma_hd";
            if (StartUp.SysObj.ExcuteScalar(cmd) != null)
                return true;

            cmd.CommandText = "select so_ct from cttt20 where rtrim(ltrim(ma_hd)) = @ma_hd or rtrim(ltrim(ma_hdm)) = @ma_hd";
            if (StartUp.SysObj.ExcuteScalar(cmd) != null)
                return true;

            return false;
        }

        #region V_Sua
        private void V_Sua()
        {
            ma_hd = txtSo_ct.Text.Trim();

            if (StartUp.DsTrans.Tables[0].Rows.Count == 0)
                ExMessageBox.Show( 2215,StartUp.SysObj, "Không có dữ liệu!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                if (!SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, txtNgay_ct.dValue))
                {
                    ExMessageBox.Show( 2220,StartUp.SysObj, "Ngày hạch toán phải sau ngày khóa sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background
                , new Action(() =>
                {
                    txtMa_kh.IsFocus = true;
                }));
                currActionTask = ActionTask.Edit;

                DsVitual = new DataSet();
                DsVitual.Tables.Add(StartUp.DsTrans.Tables[0].DefaultView.ToTable());
                DsVitual.Tables.Add(StartUp.DsTrans.Tables[1].DefaultView.ToTable());
                IsInEditMode.Value = true;

                Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
                //IsVisibilityFieldsXamDataGridBySua_Tien();

            }
        }
        #endregion

        #region V_Huy
        private void V_Huy()
        {
            IsInEditMode.Value = false;
            if (DsVitual != null && StartUp.DsTrans.Tables[0].Rows.Count > 0)
            {
                switch (currActionTask)
                {
                    case ActionTask.Edit:
                        {
                            currActionTask = ActionTask.View;
                            //xóa các row trong table[1], table[2]
                            string stt_rec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();

                            // Nên dịch chuyển iRow lùi dòng 0
                            // Sau đó RowFilter lại Table[0], Table[1], Table[2]
                            StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString() + "'";
                            StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString() + "'";

                            //Refresh lại grid hạch toán
                            if (StartUp.DsTrans.Tables[1].Rows.Count > 0)
                            {
                                //lấy các rowfilter trong grdct
                                DataRow[] _row = StartUp.DsTrans.Tables[1].Select("stt_rec='" + stt_rec + "'");
                                foreach (DataRow dr in _row)
                                {
                                    //delete các row có trong grdct
                                    StartUp.DsTrans.Tables[1].Rows.Remove(dr);
                                }
                            }

                            //Refresh lại table[0]
                            StartUp.DsTrans.Tables[0].Rows.RemoveAt(iRow);

                            DataRow rowPh = StartUp.DsTrans.Tables[0].NewRow();
                            rowPh.ItemArray = DsVitual.Tables[0].Rows[0].ItemArray;
                            StartUp.DsTrans.Tables[0].Rows.InsertAt(rowPh, iRow);

                            StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";
                            StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";

                            StartUp.DsTrans.Tables[1].Merge(DsVitual.Tables[1]);
                        }
                        break;
                    //Refresh lại khi chọn new
                    case ActionTask.Copy:
                    case ActionTask.Add:
                        {
                            V_Xoa();
                            if (StartUp.DsTrans.Tables[0].Rows.Count > 0)
                            {
                                iRow = OldiRow;
                                //load lại form theo stt_rec
                                StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString());
                            }
                        }
                        break;
                }
            }
        }
        #endregion

        #region V_Xoa
        protected override bool CheckCanDelete()
        {
            ma_hd = txtSo_ct.Text.Trim();
            if (KiemTraCoPhatSinh())
            {
                ExMessageBox.Show(2375, StartUp.SysObj, "Hợp đồng đã có phát sinh, không được xóa!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            return true;
        }
        private void V_Xoa()
        {
            if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim()))
                return;
            
            currActionTask = ActionTask.Delete;
            try
            {
                string _stt_rec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();
           
                //xóa chứng từ
                StartUp.DeleteVoucher(_stt_rec);

                // ----Warning : Không nên xóa Table[0] trước, nếu xóa trước sẽ bị mất Binding -----------------------
                // Nên dịch chuyển iRow lùi 1 dòng
                // Sau đó RowFilter lại Table[0], Table[1], Table[2]
                // Rồi mới xóa Table[0]
                //iRow = iRow > 0 ? iRow - 1 : 0;
                StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString() + "'";
                StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString() + "'";

                DataRow[] phRow = StartUp.DsTrans.Tables[0].Select("stt_rec='" + _stt_rec + "'");
                //Xóa row table[0]
                StartUp.DsTrans.Tables[0].Rows.Remove(phRow[0]);
                //xóa các row trong Table[1]
                if (StartUp.DsTrans.Tables[1].Rows.Count > 0)
                {
                    DataRow[] rows = StartUp.DsTrans.Tables[1].Select("stt_rec='" + _stt_rec + "'");
                    foreach (DataRow dr in rows)
                    {
                        StartUp.DsTrans.Tables[1].Rows.Remove(dr);
                    }
                }

                //Refresh lại Table[0], Table[1], Table[2]
                if (StartUp.DsTrans.Tables[0].Rows.Count > 0)
                {
                    iRow = iRow > StartUp.DsTrans.Tables[0].Rows.Count - 1 ? iRow - 1 : iRow;
                    //load lại form theo stt_rec
                    StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString());

                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
            currActionTask = ActionTask.None;
        }
        #endregion

        #region V_Tim
        private void V_Tim()
        {
            try
            {
                currActionTask = ActionTask.View;
                FrmTim _FrmTim = new FrmTim(StartUp.SysObj, StartUp.filterId, StartUp.tableList);
                SmLib.SysFunc.LoadIcon(_FrmTim);
                _FrmTim.ShowDialog();
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }

        }
        #endregion

        #region V_Xem
        private void V_Xem()
        {
            currActionTask = ActionTask.View;
            //  set lai stringbrowse 
            string stringBrowse1, stringBrowse2;
            if (StartUp.M_LAN.Equals("V"))
            {
                stringBrowse1 = StartUp.CommandInfo["Vbrowse1"].ToString().Split('|')[0];
                stringBrowse2 = StartUp.CommandInfo["Vbrowse1"].ToString().Split('|')[1];
            }
            else
            {
                stringBrowse1 = StartUp.CommandInfo["Ebrowse1"].ToString().Split('|')[0];
                stringBrowse2 = StartUp.CommandInfo["Ebrowse1"].ToString().Split('|')[1];
            }
            //StartUp.DsTrans.Tables[0].AcceptChanges();
            DataTable PhViewTablev = StartUp.DsTrans.Tables[0].Copy();
            PhViewTablev.Rows.RemoveAt(0);
            SmVoucherLib.FormView _frmView = new SmVoucherLib.FormView(StartUp.SysObj, PhViewTablev.DefaultView, StartUp.DsTrans.Tables[1].DefaultView, stringBrowse1, stringBrowse2, "stt_rec");
            _frmView.ListFieldSum = "t_tt_nt;t_tt";
            _frmView.frmBrw.Title = SmLib.SysFunc.Cat_Dau(M_LAN.Equals("V") ? StartUp.CommandInfo["bar"].ToString() : StartUp.CommandInfo["bar2"].ToString());

            SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, _frmView.frmBrw.oBrowseCt, StartUp.Ma_ct, 1);

            _frmView.frmBrw.LanguageID  = "SODMHDB_4";
            _frmView.ShowDialog();

            // Set lai irow va rowfilter ...
            if (_frmView.DataGrid.ActiveRecord != null)
            {
                //int select_item_index = (_frmView.DataGrid.ActiveRecord as DataRecord).DataItemIndex;
                int select_irow = (_frmView.DataGrid.ActiveRecord as DataRecord).Index;
                if (select_irow >= 0)
                {
                    string selected_stt_rec = (_frmView.DataGrid.DataSource as DataView)[select_irow]["stt_rec"].ToString();
                    FrmPoctpna.iRow = select_irow + 1;
                    StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + selected_stt_rec + "'";
                    StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + selected_stt_rec + "'";

                }
            }

        }
        #endregion

        #region V_In
        private void V_In()
        {
            
            FrmIn oReport = new FrmIn();
            oReport.ShowDialog();

        }
        #endregion

        #region FormMain_EditModeEnded
        //Ham nay dung de xu ly sau khi an mot button 
        private void FormMain_EditModeEnded(object sender, string menuItemName, RoutedEventArgs e)
        {
            //MessageBox.Show(menuItemName.ToString());
            IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
            Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
            Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
            if (StartUp.DsTrans.Tables[0].DefaultView.Count > 0)
            {
                if(!menuItemName.Equals("btnSave"))
                    loaddataDu13();
            }
            //Luu lai ma kho cua phieu truoc
            if (StartUp.DsTrans.Tables[1].DefaultView.Count > 0)
            {
                Old_ma_kho = StartUp.DsTrans.Tables[1].DefaultView[0]["ma_kho_i"].ToString();
            }
        }
        #endregion

        #region NewRowCt
        void NewRowCt()
        {
            try
            {
                DataRow NewRecord = StartUp.DsTrans.Tables[1].NewRow();
                NewRecord["stt_rec"] = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"];

                int Stt_rec0 = 0, Stt_rec0ct = 0, Stt_rec0ctgt = 0;
                if (GrdCt.Records.Count > 0)
                {
                    var _max_sttrec0ct = StartUp.DsTrans.Tables[1].AsEnumerable()
                                       .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                                       .Max(x => x.Field<string>("stt_rec0"));
                    if (_max_sttrec0ct != null)
                        int.TryParse(_max_sttrec0ct.ToString(), out Stt_rec0ct);
                }
                Stt_rec0 = Stt_rec0ct >= Stt_rec0ctgt ? Stt_rec0ct : Stt_rec0ctgt;
                Stt_rec0++;

                NewRecord["stt_rec0"] = string.Format("{0:000}", Stt_rec0);
                NewRecord["ma_ct"] = StartUp.Ma_ct;
                NewRecord["ngay_ct"] = txtNgay_ct.Value == null ? DateTime.Now.Date : txtNgay_ct.dValue.Date;
                if (StartUp.DsTrans.Tables[1].DefaultView.Count > 0)
                {
                    NewRecord["ma_kho_i"] = StartUp.DsTrans.Tables[1].DefaultView[StartUp.DsTrans.Tables[1].DefaultView.Count - 1]["ma_kho_i"];
                }
                else
                {
                    NewRecord["ma_kho_i"] = Old_ma_kho;
                }
                NewRecord["so_luong"] = 0;
                NewRecord["gia_nt2"] = 0;
                NewRecord["tien_nt2"] = 0;
                NewRecord["gia_nt"] = 0;
                NewRecord["tien_nt"] = 0;

                NewRecord["tien2"] = 0;
                NewRecord["gia2"] = 0;
                NewRecord["tien"] = 0;
                NewRecord["gia"] = 0;

                NewRecord["ck_nt"] = 0;
                NewRecord["ck"] = 0;
                NewRecord["ton13"] = 0;

                FreeCodeFieldLib.CarryFreeCodeFields(StartUp.SysObj, StartUp.Ma_ct, StartUp.DsTrans.Tables[1].DefaultView, NewRecord, 1);
                StartUp.DsTrans.Tables[1].Rows.Add(NewRecord);
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        #endregion

        #region GrdCt_AddNewRecord
        private bool GrdCt_AddNewRecord(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            NewRowCt();
            return true;
        }
        #endregion

        #region GrdCt_EditModeEnded
        private void GrdCt_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            try
            {
                if (IsInEditMode.Value == false)
                    return;

                if (GrdCt.ActiveCell != null && StartUp.DsTrans.Tables[1].GetChanges(DataRowState.Deleted) == null)
                {
                    switch (e.Cell.Field.Name)
                    {
                        #region ma_vt
                        case "ma_vt":
                            {

                                if (e.Editor.Value == null)
                                    return;
                                AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                if (txt.RowResult != null)
                                {
                                    e.Cell.Record.Cells["ten_vt"].Value = txt.RowResult["ten_vt"];
                                    e.Cell.Record.Cells["ten_vt2"].Value = txt.RowResult["ten_vt2"];
                                    e.Cell.Record.Cells["dvt"].Value = txt.RowResult["dvt"];
                                    e.Cell.Record.Cells["so_khung"].Value = txt.RowResult["so_khung"];
                                    e.Cell.Record.Cells["so_may"].Value = txt.RowResult["so_may"];
                                    e.Cell.Record.Cells["ma_mau"].Value = txt.RowResult["ma_mau"];                                    

                                    if (string.IsNullOrEmpty((e.Cell.Record.DataItem as DataRowView)["tk_vt"].ToString().Trim()))
                                    {
                                        (e.Cell.Record.DataItem as DataRowView)["tk_vt"] = txt.RowResult["tk_vt"];

                                    }

                                    CellValuePresenter cell_Kho = CellValuePresenter.FromCell(e.Cell.Record.Cells["ma_kho_i"]);
                                    AutoCompleteTextBox autoCompleteKho = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(cell_Kho.Editor as ControlHostEditor);
                                    //if (autoCompleteKho != null)
                                    //{
                                    //    autoCompleteKho.SearchInit();
                                    //    if (autoCompleteKho.RowResult != null)
                                    //    {
                                    //        if (autoCompleteKho.RowResult["tk_dl"] != DBNull.Value && !string.IsNullOrEmpty(autoCompleteKho.RowResult["tk_dl"].ToString().Trim()))
                                    //        {
                                    //            (e.Cell.Record.DataItem as DataRowView)["tk_vt"] = autoCompleteKho.RowResult["tk_dl"];
                                    //        }
                                    //    }
                                    //}

                                    //DataRowView drVCT = e.Cell.Record.DataItem as DataRowView;
                               


                                    //if (txt.RowResult["vt_ton_kho"].ToString().Equals("0"))
                                    //{
                                    //    e.Cell.Record.Cells["so_luong"].Value = 0;
                                    //    e.Cell.Record.Cells["gia_nt0"].Value = 0;
                                    //    e.Cell.Record.Cells["gia0"].Value = 0;

                                    //    CellValuePresenter cell_so_luong = CellValuePresenter.FromCell(e.Cell.Record.Cells["so_luong"]);
                                    //    cell_so_luong.Editor.IsReadOnly = true;
                                    //}
                                    //else
                                    //{
                                        //CellValuePresenter cell_so_luong = CellValuePresenter.FromCell(e.Cell.Record.Cells["so_luong"]);
                                        //cell_so_luong.Editor.IsReadOnly = false;
                                  //  }

                                  
                                }
                                break;
                            }
                        #endregion

                        #region ma_kho_i
                        case "ma_kho_i":
                            {
                                if (e.Editor.Value == null)
                                    return;

                                AutoCompleteTextBox autoCompleteKho = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                if (autoCompleteKho != null)
                                {
                                    autoCompleteKho.SearchInit();
                                    if (autoCompleteKho.RowResult != null)
                                    {
                                        if (autoCompleteKho.RowResult["tk_dl"] != DBNull.Value && !string.IsNullOrEmpty(autoCompleteKho.RowResult["tk_dl"].ToString().Trim()))
                                        {
                                            e.Cell.Record.Cells["tk_vt"].Value = autoCompleteKho.RowResult["tk_dl"];
                                        }
                                    }
                                }

                                //if (!string.IsNullOrEmpty(e.Cell.Record.Cells["ma_vt"].Value.ToString()) && !string.IsNullOrEmpty(e.Cell.Record.Cells["ma_kho_i"].Value.ToString()) && StartUp.M_TON_KHO13.Equals("1"))
                                //{
                                //    e.Cell.Record.Cells["ton13"].Value = InvtLib.InFuncLib.GetTon13(StartUp.SysObj, e.Cell.Record.Cells["ma_kho_i"].Value.ToString(), e.Cell.Record.Cells["ma_vt"].Value.ToString());
                                //}
                                break;
                            }
                        #endregion

                        #region so_luong
                        case "so_luong":
                            {
                                try
                                {
                                    if (e.Editor.Value == DBNull.Value)
                                        e.Cell.Record.Cells["so_luong"].Value = 0;

                                    if (e.Cell.IsDataChanged)
                                    {
                                        decimal so_luong = 0, gia_nt2 = 0, gia2 = 0, thue_suat = 0;
                                        decimal tien_nt2 = 0, tien2 = 0, ck_nt = 0, ck = 0, gia = 0, tien = 0, gia_nt = 0, tien_nt = 0, tl_ck ;
                                        so_luong = (e.Editor as NumericTextBox).nValue;

                                        decimal.TryParse(e.Cell.Record.Cells["thue_suat"].Value.ToString(), out thue_suat);
                                        decimal.TryParse(e.Cell.Record.Cells["gia_nt2"].Value.ToString(), out gia_nt2);
                                        decimal.TryParse(e.Cell.Record.Cells["gia2"].Value.ToString(), out gia2);

                                        decimal.TryParse(e.Cell.Record.Cells["gia_nt"].Value.ToString(), out gia_nt);
                                        decimal.TryParse(e.Cell.Record.Cells["gia"].Value.ToString(), out gia);
                                        decimal.TryParse(e.Cell.Record.Cells["tl_ck"].Value.ToString(), out tl_ck);

                                        if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                        {
                                            if (gia_nt2 * so_luong != 0)
                                            {
                                                tien_nt2 = SmLib.SysFunc.Round(gia_nt2 * so_luong, StartUp.M_ROUND_NT); // tien ban
                                                tien_nt = SmLib.SysFunc.Round(gia_nt * so_luong, StartUp.M_ROUND_NT); // tien von
                                                tien2 = tien_nt2;
                                                tien = tien_nt;

                                                e.Cell.Record.Cells["tien_nt2"].Value = tien_nt2;
                                                e.Cell.Record.Cells["tien2"].Value = tien2;

                                                e.Cell.Record.Cells["tien_nt"].Value = tien_nt;
                                                e.Cell.Record.Cells["tien"].Value = tien;

                                                ck_nt = SmLib.SysFunc.Round(tien_nt2 * tl_ck / 100,StartUp.M_ROUND_NT);
                                                ck = ck_nt;
                                                e.Cell.Record.Cells["ck_nt"].Value = ck_nt;
                                                e.Cell.Record.Cells["ck"].Value = ck;

                                                e.Cell.Record.Cells["thue_nt"].Value = SmLib.SysFunc.Round((tien_nt2 - ck_nt) * thue_suat / 100, StartUp.M_ROUND_NT);
                                                e.Cell.Record.Cells["thue"].Value = SmLib.SysFunc.Round((tien_nt2 - ck_nt) * thue_suat / 100, StartUp.M_ROUND_NT);
                                            }
                                        }
                                        else
                                        {
                                            if (gia_nt2 * so_luong != 0)
                                            {
                                                tien_nt2 = SmLib.SysFunc.Round(gia_nt2 * so_luong, StartUp.M_ROUND_NT);
                                                e.Cell.Record.Cells["tien_nt2"].Value = tien_nt2;
                                                
                                                tien_nt = SmLib.SysFunc.Round(gia_nt * so_luong, StartUp.M_ROUND_NT); // tien von
                                                e.Cell.Record.Cells["tien_nt"].Value = tien_nt;

                                                ck_nt = SmLib.SysFunc.Round(tien_nt2 * tl_ck / 100,StartUp.M_ROUND_NT);
                                                e.Cell.Record.Cells["ck_nt"].Value = ck_nt;
                                                e.Cell.Record.Cells["thue_nt"].Value = SmLib.SysFunc.Round((tien_nt2 - ck_nt) * thue_suat / 100, StartUp.M_ROUND_NT);
                                            }

                                            if (gia2 * so_luong != 0)
                                            {
                                                tien2 = SmLib.SysFunc.Round(gia2 * so_luong, StartUp.M_ROUND);
                                                e.Cell.Record.Cells["tien2"].Value = tien2;

                                                ck = SmLib.SysFunc.Round(tien2 * tl_ck / 100, StartUp.M_ROUND);
                                                e.Cell.Record.Cells["ck"].Value = ck;

                                                tien = SmLib.SysFunc.Round(gia * so_luong, StartUp.M_ROUND); // tien von
                                                e.Cell.Record.Cells["tien"].Value = tien;

                                                e.Cell.Record.Cells["thue"].Value = SmLib.SysFunc.Round((tien2 - ck) * thue_suat / 100, StartUp.M_ROUND);
                                            }
                                        }
                                        Sum_ALL();
                                    }

                                }
                                catch (Exception ex)
                                {
                                    SmErrorLib.ErrorLog.CatchMessage(ex);
                                }
                                break;
                            }
                        #endregion

                        #region gia_nt2 giá bán
                        case "gia_nt2":
                            {
                                if (e.Editor.Value == DBNull.Value)
                                    e.Cell.Record.Cells["gia_nt2"].Value = 0;

                                if (e.Cell.IsDataChanged)
                                {
                                    decimal so_luong = 0, gia_nt2 = 0, tien_nt2 = 0, tien2 = 0, ty_gia = 0, thue_suat = 0;
                                  
                                    decimal  ck_nt = 0, ck = 0, gia = 0, tien = 0, gia_nt = 0, tien_nt = 0, tl_ck = 0;
                                      
                                    decimal.TryParse(e.Cell.Record.Cells["thue_suat"].Value.ToString(), out thue_suat);
                                    decimal.TryParse(e.Cell.Record.Cells["so_luong"].Value.ToString(), out so_luong);
                                    gia_nt2 = (e.Editor as NumericTextBox).nValue;
                                    decimal.TryParse(e.Cell.Record.Cells["tl_ck"].Value.ToString(), out tl_ck);
                                    ty_gia = txtTy_gia.nValue;


                                    if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        if (gia_nt2 * so_luong != 0)
                                        {
                                            
                                            tien_nt2 = SmLib.SysFunc.Round(so_luong * gia_nt2, StartUp.M_ROUND_NT);
                                            e.Cell.Record.Cells["tien_nt2"].Value = tien_nt2;
                                            ck_nt = SmLib.SysFunc.Round(tien_nt2 * tl_ck / 100, StartUp.M_ROUND_NT);
                                            e.Cell.Record.Cells["ck_nt"].Value = ck_nt;
                                            e.Cell.Record.Cells["thue_nt"].Value = SmLib.SysFunc.Round((tien_nt2 - ck_nt) * thue_suat / 100, StartUp.M_ROUND_NT);

                                            e.Cell.Record.Cells["gia2"].Value = gia_nt2;
                                            e.Cell.Record.Cells["tien2"].Value = tien_nt2;
                                            e.Cell.Record.Cells["ck"].Value = ck_nt;
                                            e.Cell.Record.Cells["thue"].Value = SmLib.SysFunc.Round((tien_nt2 - ck_nt) * thue_suat / 100, StartUp.M_ROUND_NT);
                                        }
                                        e.Cell.Record.Cells["gia2"].Value = gia_nt2;
                                    }
                                    else
                                    {
                                        if (gia_nt2 * so_luong != 0)
                                        {
                                            tien_nt2 = SmLib.SysFunc.Round(so_luong * gia_nt2, StartUp.M_ROUND_NT);
                                            e.Cell.Record.Cells["tien_nt2"].Value = tien_nt2;
                                            ck_nt = SmLib.SysFunc.Round(tien_nt2 * tl_ck / 100, StartUp.M_ROUND_NT);
                                            e.Cell.Record.Cells["ck_nt"].Value = ck_nt;
                                            e.Cell.Record.Cells["thue_nt"].Value = SmLib.SysFunc.Round((tien_nt2 - ck_nt) * thue_suat / 100, StartUp.M_ROUND_NT);
                                        }

                                        if (gia_nt2 * ty_gia != 0)
                                        {
                                            e.Cell.Record.Cells["gia2"].Value = SmLib.SysFunc.Round(gia_nt2 * ty_gia, StartUp.M_ROUND_GIA);
                                        }

                                        if (tien_nt2 * ty_gia != 0)
                                        {
                                            tien2 = SmLib.SysFunc.Round(tien_nt2 * ty_gia, StartUp.M_ROUND);
                                            e.Cell.Record.Cells["tien2"].Value = tien2;
                                            ck = SmLib.SysFunc.Round(tl_ck * tien2 / 100, StartUp.M_ROUND);
                                            e.Cell.Record.Cells["ck"].Value = ck;
                                            e.Cell.Record.Cells["thue"].Value = SmLib.SysFunc.Round((tien2 - ck) * thue_suat / 100, StartUp.M_ROUND_NT);
                                        }
                                    }
                                   Sum_ALL();
                                }
                                break;
                            }
                        #endregion 

                        #region gia_nt  giá vốn
                        case "gia_nt":
                            {
                                if (e.Editor.Value == DBNull.Value)
                                    e.Cell.Record.Cells["gia_nt"].Value = 0;

                                if (e.Cell.IsDataChanged)
                                {
                                    decimal so_luong = 0, gia_nt2 = 0, tien_nt2 = 0, tien2 = 0, ty_gia = 0, thue_suat = 0;
                                    decimal ck_nt = 0, ck = 0, gia = 0, tien = 0, gia_nt = 0, tien_nt = 0, tl_ck = 0;

                                    decimal.TryParse(e.Cell.Record.Cells["so_luong"].Value.ToString(), out so_luong);
                                    gia_nt = (e.Editor as NumericTextBox).nValue;
                                    ty_gia = txtTy_gia.nValue;
                                    if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        if (gia_nt * so_luong != 0)
                                        {
                                            tien_nt = SmLib.SysFunc.Round(so_luong * gia_nt, StartUp.M_ROUND);
                                            e.Cell.Record.Cells["tien_nt"].Value = tien_nt;
                                            e.Cell.Record.Cells["gia"].Value = gia_nt;
                                            e.Cell.Record.Cells["tien"].Value = tien_nt;
                                        }
                                        //else
                                        //{
                                        //    tien_nt = SmLib.SysFunc.Round(so_luong * gia_nt, StartUp.M_ROUND);
                                        //    e.Cell.Record.Cells["tien_nt"].Value = 0;
                                        //    e.Cell.Record.Cells["gia"].Value = 0;
                                        //    e.Cell.Record.Cells["tien"].Value = 0;
                                        //}
                                    }
                                    else
                                    {
                                        if (gia_nt * so_luong != 0)
                                        {
                                            tien_nt = SmLib.SysFunc.Round(so_luong * gia_nt, StartUp.M_ROUND_NT);
                                            e.Cell.Record.Cells["tien_nt"].Value = tien_nt;
                                        }

                                        if (gia_nt * ty_gia != 0)
                                        {
                                            
                                            gia =  SmLib.SysFunc.Round(gia_nt * ty_gia, StartUp.M_ROUND_GIA);
                                            e.Cell.Record.Cells["gia"].Value = gia;
                                            e.Cell.Record.Cells["tien"].Value = SmLib.SysFunc.Round(gia * so_luong, StartUp.M_ROUND);
                                        }
                                    }
                                      Sum_ALL();
                                }
                                break;
                            }
                        #endregion

                        #region tien_nt2
                        case "tien_nt2":
                            {
                                if (e.Editor.Value == DBNull.Value)
                                    e.Cell.Record.Cells["tien_nt2"].Value = 0;

                                if (e.Cell.IsDataChanged)
                                {
                                    decimal ty_gia = 0, tien_nt2 = 0, tien2 = 0, thue_suat = 0, tl_ck = 0, ck_nt = 0, ck = 0;
                                    tien_nt2 = (e.Editor as NumericTextBox).nValue;
                                    ty_gia = txtTy_gia.nValue;
                                    decimal.TryParse(e.Cell.Record.Cells["thue_suat"].Value.ToString(), out thue_suat);
                                    decimal.TryParse(e.Cell.Record.Cells["tl_ck"].Value.ToString(), out tl_ck);
                                    if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        
                                        ck_nt = SmLib.SysFunc.Round(tien_nt2 * tl_ck / 100, StartUp.M_ROUND_NT);
                                        e.Cell.Record.Cells["ck_nt"].Value = ck_nt;
                                        e.Cell.Record.Cells["thue_nt"].Value = SmLib.SysFunc.Round((tien_nt2 - ck_nt) * thue_suat / 100, StartUp.M_ROUND_NT);

                                        e.Cell.Record.Cells["tien2"].Value = tien_nt2;
                                        e.Cell.Record.Cells["ck"].Value = ck_nt;
                                        e.Cell.Record.Cells["thue"].Value = SmLib.SysFunc.Round((tien_nt2 - ck_nt) * thue_suat / 100, StartUp.M_ROUND_NT);

                                    }
                                    else
                                    {
                                        if (tien_nt2 * ty_gia != 0)
                                        {
                                            ck_nt = SmLib.SysFunc.Round(tien_nt2 * tl_ck / 100, StartUp.M_ROUND_NT);
                                            e.Cell.Record.Cells["ck_nt"].Value = ck_nt;
                                            e.Cell.Record.Cells["thue_nt"].Value = SmLib.SysFunc.Round((tien_nt2 - ck_nt) * thue_suat / 100, StartUp.M_ROUND_NT);

                                            tien2 = SmLib.SysFunc.Round(tien_nt2 * ty_gia, StartUp.M_ROUND);
                                            e.Cell.Record.Cells["tien2"].Value = tien2;
                                            ck = SmLib.SysFunc.Round(tien2 * tl_ck / 100, StartUp.M_ROUND);
                                            e.Cell.Record.Cells["ck"].Value = ck;
                                            e.Cell.Record.Cells["thue"].Value = SmLib.SysFunc.Round((tien2 - ck) * thue_suat / 100, StartUp.M_ROUND);
                                        }
                                    }

                                 Sum_ALL();
                                }
                                break;
                            }
                        #endregion

                        #region gia2
                        case "gia2":
                            {
                                if (e.Editor.Value == DBNull.Value)
                                    e.Cell.Record.Cells["gia2"].Value = 0;

                                if (e.Cell.IsDataChanged)
                                {
                                    decimal so_luong = 0, gia2 = 0, tien2 = 0, thue_suat = 0, tl_ck = 0, ck = 0;
                                    decimal.TryParse(e.Cell.Record.Cells["thue_suat"].Value.ToString(), out thue_suat);
                                    gia2 = (e.Editor as NumericTextBox).nValue;
                                    decimal.TryParse(e.Cell.Record.Cells["so_luong"].Value.ToString(), out so_luong);
                                    decimal.TryParse(e.Cell.Record.Cells["tl_ck"].Value.ToString(), out tl_ck);
                                    if (gia2 * so_luong != 0)
                                    {
                                        
                                        tien2 = SmLib.SysFunc.Round(gia2 * so_luong, StartUp.M_ROUND);
                                        ck = SmLib.SysFunc.Round(tien2 * tl_ck / 100, StartUp.M_ROUND);
                                        e.Cell.Record.Cells["tien2"].Value = tien2;
                                        e.Cell.Record.Cells["ck"].Value = ck;
                                        e.Cell.Record.Cells["thue"].Value = SmLib.SysFunc.Round((tien2 - ck) * thue_suat / 100, StartUp.M_ROUND);
                                    }

                                  Sum_ALL();
                                }
                                break;
                            }
                        #endregion

                        #region tien2
                        case "tien2":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (!IsCheckedSua_tien.Value)
                                    {
                                        decimal tien2 = 0, thue_suat = 0, tl_ck = 0, ck = 0;
                                        tien2 = (e.Editor as NumericTextBox).nValue;
                                        decimal.TryParse(e.Cell.Record.Cells["tl_ck"].Value.ToString(), out tl_ck);
                                        ck = SmLib.SysFunc.Round(tien2 * tl_ck / 100, StartUp.M_ROUND);
                                        e.Cell.Record.Cells["ck"].Value = ck;
                                        decimal.TryParse(e.Cell.Record.Cells["thue_suat"].Value.ToString(), out thue_suat);
                                        e.Cell.Record.Cells["thue"].Value = SmLib.SysFunc.Round((tien2 - ck) * thue_suat / 100, StartUp.M_ROUND);
                                    }
                                  Sum_ALL();
                                }
                                break;
                            }
                        #endregion

                        #region tl_ck
                        case "tl_ck":
                        {
                            if (e.Cell.IsDataChanged)
                            {
                                decimal tl_ck = 0, ck = 0, tien_nt2 = 0, ck_nt = 0, tien2 = 0, thue_suat = 0;
                                tl_ck = (e.Editor as NumericTextBox).nValue;
                                
                                decimal.TryParse(e.Cell.Record.Cells["tien_nt2"].Value.ToString(), out tien_nt2);
                                decimal.TryParse(e.Cell.Record.Cells["tien2"].Value.ToString(), out tien2);
                                decimal.TryParse(e.Cell.Record.Cells["thue_suat"].Value.ToString(), out thue_suat);
                                decimal.TryParse(e.Cell.Record.Cells["tl_ck"].Value.ToString(), out tl_ck);

                                ck_nt = tien_nt2 * tl_ck / 100;
                                ck = tien2 * tl_ck / 100;

                                e.Cell.Record.Cells["ck_nt"].Value = SmLib.SysFunc.Round(ck_nt, StartUp.M_ROUND_NT);
                                e.Cell.Record.Cells["ck"].Value = SmLib.SysFunc.Round(ck, StartUp.M_ROUND);

                                e.Cell.Record.Cells["thue_nt"].Value = SmLib.SysFunc.Round((tien_nt2 - ck_nt) * thue_suat / 100, StartUp.M_ROUND_NT);
                                e.Cell.Record.Cells["thue"].Value = SmLib.SysFunc.Round((tien2 - ck) * thue_suat / 100, StartUp.M_ROUND_NT);

                                Sum_ALL();
                            }
                            break;
                        }
                        #endregion

                        #region ma_thue
                        case "ma_thue":
                            {
                                AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                if (txt.IsDataChanged)
                                {
                                    decimal t_tien_nt = 0, t_tien = 0, thue_suat = 0, ck = 0, ck_nt = 0;

                                    if (txt.RowResult != null)
                                    {
                                        e.Cell.Record.Cells["thue_suat"].Value = txt.RowResult["thue_suat"];

                                        t_tien_nt = ParseDecimal(e.Cell.Record.Cells["tien_nt2"].Value, 0);
                                        t_tien = ParseDecimal(e.Cell.Record.Cells["tien2"].Value, 0);
                                        thue_suat = ParseDecimal(e.Cell.Record.Cells["thue_suat"].Value, 0);
                                        decimal.TryParse(e.Cell.Record.Cells["ck_nt"].Value.ToString(), out ck_nt);
                                        decimal.TryParse(e.Cell.Record.Cells["ck"].Value.ToString(), out ck);

                                        if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                        {
                                            e.Cell.Record.Cells["thue_nt"].Value = SmLib.SysFunc.Round((t_tien_nt - ck_nt) * thue_suat / 100, StartUp.M_ROUND);
                                            e.Cell.Record.Cells["thue"].Value = e.Cell.Record.Cells["thue_nt"].Value;
                                        }
                                        else
                                        {
                                            e.Cell.Record.Cells["thue_nt"].Value = SmLib.SysFunc.Round((t_tien_nt - ck_nt) * thue_suat / 100, StartUp.M_ROUND_NT);
                                            e.Cell.Record.Cells["thue"].Value = SmLib.SysFunc.Round((t_tien - ck)* thue_suat / 100, StartUp.M_ROUND);
                                        }
                                       
                                    }
                                    else
                                    {
                                        e.Cell.Record.Cells["thue_suat"].Value = 0;
                                        e.Cell.Record.Cells["thue_nt"].Value = 0;
                                        e.Cell.Record.Cells["thue"].Value = 0;
                                    }
                                    Sum_ALL();
                                }
                                break;
                            }
                        #endregion

                        #region ck_nt
                        case "ck_nt":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (cbMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
                                        e.Cell.Record.Cells["ck"].Value = e.Cell.Record.Cells["ck_nt"].Value;
                                    else
                                    {
                                        decimal _Ty_gia = txtTy_gia.nValue;
                                        e.Cell.Record.Cells["ck"].Value = SmLib.SysFunc.Round(Convert.ToDecimal(e.Cell.Record.Cells["ck_nt"].Value) * _Ty_gia, StartUp.M_ROUND);
                                    }
                                    Sum_ALL();
                                }
                                break;
                            }
                        #endregion


                        #region ck
                        case "ck":
                            {
                                Sum_ALL();
                                break;
                            }
                        #endregion

                        #region thue
                        case "thue":
                            {
                                Sum_ALL();
                                break;
                            }
                        #endregion

                        #region thue_nt
                        case "thue_nt":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (cbMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
                                        e.Cell.Record.Cells["thue"].Value = e.Cell.Record.Cells["thue_nt"].Value;
                                    else
                                    {
                                        decimal _Ty_gia = txtTy_gia.nValue;
                                        e.Cell.Record.Cells["thue"].Value = SmLib.SysFunc.Round(Convert.ToDecimal(e.Cell.Record.Cells["thue_nt"].Value) * _Ty_gia, StartUp.M_ROUND);
                                    }
                                    Sum_ALL();
                                }
                                break;
                            }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        #endregion

        #region GrdCt_RecordDelete
        private void GrdCt_RecordDelete(object sender, Infragistics.Windows.DataPresenter.Events.RecordsDeletedEventArgs e)
        {

            //if (GrdCt.Records.Count == 0 )
            //{
            //    Dispatcher.BeginInvoke((ThreadStart)delegate
            //    {
            //        (this.Toolbar.FindName("btnCancel") as SmVoucherLib.ToolBarButton).Focus();
            //    });

            //}
            //else 
            //{
            //    DataRecord record = (GrdCt.Records[0] as DataRecord);
            //    if (!string.IsNullOrEmpty(record.Cells["ma_vt"].ToString().Trim()))
            Dispatcher.BeginInvoke((ThreadStart)delegate
                {
                    txtHan_tt.Focus();
                });
            //Dispatcher.BeginInvoke((ThreadStart)delegate
            //{
            //    (this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus();
            //});
            //}
            //GrdCt.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);  
            //(this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus();       
        }
        #endregion

        #region GrdCt_KeyUp
        private void GrdCt_KeyUp(object sender, KeyEventArgs e)
        {
            if (IsInEditMode.Value == false)
                return;

            switch (e.Key)
            {
                case Key.F4:
                    {
                        DataRecord record = (GrdCt.ActiveRecord as DataRecord);
                        if (record == null || record.Cells["ma_vt"].Value == null || record.Cells["ma_vt"].Value.ToString() == "")
                            return;

                        NewRowCt();
                        GrdCt.ActiveRecord = GrdCt.Records[GrdCt.Records.Count - 1];
                        GrdCt.ActiveCell = (GrdCt.ActiveRecord as DataRecord).Cells["ma_vt"];
                        break;
                    }

                case Key.F8:
                    {
                        if (ExMessageBox.Show( 2225,StartUp.SysObj, "Có xóa dòng ghi hiện thời không?", "Fast Book 11 .NET", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
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
                                StartUp.DsTrans.Tables[1].Rows.Remove(StartUp.DsTrans.Tables[1].DefaultView[record.Index].Row);
                                StartUp.DsTrans.Tables[1].AcceptChanges();

                                if (GrdCt.Records.Count > 0)
                                {
                                    GrdCt.ActiveRecord = GrdCt.Records[indexRecord > GrdCt.Records.Count - 1 ? GrdCt.Records.Count - 1 : indexRecord];
                                }

                                //StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt0"] = StartUp.DsTrans.Tables[1].Compute("sum(tien_nt0)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter);
                                //StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien0"] = StartUp.DsTrans.Tables[1].Compute("sum(tien0)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter);
                                //StartUp.DsTrans.Tables[0].DefaultView[0]["t_cp_nt"] = StartUp.DsTrans.Tables[1].Compute("sum(cp_nt)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter);
                                //StartUp.DsTrans.Tables[0].DefaultView[0]["t_cp"] = StartUp.DsTrans.Tables[1].Compute("sum(cp)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter);
                               Sum_ALL();
                            }
                        }

                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region GrdCt_KeyDown
        private void GrdCt_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsInEditMode.Value == false)
                return;
            if (Keyboard.IsKeyDown(Key.N) && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                DataRecord record = (GrdCt.ActiveRecord as DataRecord);
                if (record == null || record.Cells["ma_vt"].Value == null || record.Cells["ma_vt"].Value.ToString() == "")
                    return;

                NewRowCt();
                GrdCt.ActiveRecord = GrdCt.Records[GrdCt.Records.Count - 1];
                GrdCt.ActiveCell = (GrdCt.Records[GrdCt.Records.Count - 1] as DataRecord).Cells["ma_vt"];
           
            }
          
        }
        #endregion

        #region ChkSuaTien_Click
        private void ChkSuaTien_Click(object sender, RoutedEventArgs e)
        {
            IsVisibilityFieldsXamDataGridBySua_Tien();
            if (ChkSuaTien.IsChecked == false && sender.GetType().Name.Equals("CheckBox"))
            {
                TyGiaValueChange();
            }
        }
        #endregion

        #region V_Nhan
        private void V_Nhan()
        {
            try
            {
                StartUp.DsTrans.Tables[0].AcceptChanges();
                StartUp.DsTrans.Tables[1].AcceptChanges();
                bool isError = false;
                if (!IsSequenceSave)
                {

                    //tổng chi phí của các vật tư
                    decimal _tong_ck_nt_vt = 0, _t_ck_nt = 0, _tong_ck_vt = 0, _t_ck = 0;
                    decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(ck_nt)", "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'").ToString(), out _tong_ck_nt_vt);
                    decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(ck)", "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'").ToString(), out _tong_ck_vt);
                    //tổng chi phí 
                    decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_ck_nt"].ToString(), out _t_ck_nt);
                    decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_ck"].ToString(), out _t_ck);
                    decimal _ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);


                    GrdCt.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);

                    //Nếu dữ liệu đang sửa bị sai là autocompletetextbox thì ko chi lưu.
                    if (Keyboard.FocusedElement.GetType().Equals(typeof(TextBoxAutoComplete)))
                    {
                        TextBoxAutoComplete txt = Keyboard.FocusedElement as TextBoxAutoComplete;
                        if (txt.ParentControl != null)
                        {
                            if (!txt.ParentControl.CheckLostFocus())
                            {
                                return;
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_kh"].ToString()))
                    {
                        ExMessageBox.Show( 2230,StartUp.SysObj, "Chưa vào mã khách hàng!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtMa_kh.IsFocus = true;
                        isError = true;
                    }
                    else if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nx"].ToString()))
                    {
                        ExMessageBox.Show( 2235,StartUp.SysObj, "Chưa vào tài khoản có!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtMa_nx.IsFocus = true;
                        isError = true;
                    }
                    //if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_bp"].ToString()))
                    //{
                    //    ExMessageBox.Show( 2240,StartUp.SysObj, "Chưa vào mã người bán!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                    //    txtMa_bp.IsFocus = true;
                    //    isError = true;
                    //}
                    else if (string.IsNullOrEmpty(txtNgay_ct.Text.ToString()))
                    {
                        ExMessageBox.Show( 2245,StartUp.SysObj, "Chưa vào ngày hạch toán!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtNgay_ct.Focus();
                        isError = true;
                    }

                    else if ( StartUp.M_NGAY_BAT_DAU != null && (!txtNgay_ct.IsValueValid || txtNgay_ct.dValue < StartUp.M_NGAY_BAT_DAU || txtNgay_ct.dValue > StartUp.M_NGAY_KET_THUC))
                        {
                            ExMessageBox.Show(1024, StartUp.SysObj, "Ngày hạch toán không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            isError = true;
                            txtNgay_ct.Focus();
                        }
                    else if (StartUp.DsTrans.Tables[1].DefaultView.Count == 0 || string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[0]["ma_vt"].ToString()))
                    {
                        ExMessageBox.Show( 2250,StartUp.SysObj, "Chưa vào chi tiết vật tư, không lưu được!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                        TabInfo.SelectedIndex = 0;
                        GrdCt.ExecuteCommand(DataPresenterCommands.CellFirstOverall);
                        GrdCt.Focus();
                        isError = true;

                    }
                    //else if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[0]["tk_vt"].ToString()))
                    //{
                    //    ExMessageBox.Show( 2255,StartUp.SysObj, "Chưa vào tài khoản nợ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                    //    TabInfo.SelectedIndex = 0;
                    //    GrdCt.ExecuteCommand(DataPresenterCommands.CellFirstOverall);
                    //    GrdCt.Focus();
                    //    isError = true;
                    //}
                    else if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"].ToString()))
                    {
                        ExMessageBox.Show( 2260,StartUp.SysObj, "Chưa vào số chứng từ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtSo_ct.Focus();
                        isError = true;
                    }
                 
                    //so sánh tổng chi phí nt của các vật tư với tổng chi phí nt
                    else if ((_tong_ck_nt_vt != _t_ck_nt) || (_tong_ck_vt != _t_ck))
                    {
                        ExMessageBox.Show( 2265,StartUp.SysObj, "Tổng chiết khấu khác với chiết khấu tổng cộng của các vật tư!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                        SmLib.WinAPISenkey.SenKey(ModifierKeys.Alt, Key.D2);
                        //GrdCp.ActiveCell = (GrdCp.Records[0] as DataRecord).Cells["cp_nt"];
                      //  GrdCp.Focus();
                        isError = true;
                    }
                    if (!isError)
                    {
                        if (StartUp.DsTrans.Tables[1].DefaultView.Count > 0)
                        {
                            for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
                            {
                                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["ma_vt"].ToString()))
                                {
                                    ExMessageBox.Show( 2270,StartUp.SysObj, "Chưa vào chi tiết vật tư, không lưu được!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                    GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["ma_vt"];
                                    GrdCt.Focus();
                                    return;
                                }

                                //if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["ma_kho_i"].ToString()))
                                //{
                                //    ExMessageBox.Show( 2275,StartUp.SysObj, "Chưa vào chi tiết vật tư, không lưu được!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                //    GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["ma_kho_i"];
                                //    GrdCt.Focus();
                                //    return;
                                //}

                                //khong kiem tra ma_thue
                                //if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["ma_thue"].ToString()))
                                //{
                                //    ExMessageBox.Show( 2280,StartUp.SysObj, "Chưa vào mã thuế, không lưu được!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                //    GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["ma_thue"];
                                //    GrdCt.Focus();
                                //    return;
                                //}
                              
                            }

                        }

                      
                    }
                }
                if (!isError)
                {
                    if (!IsSequenceSave)
                    {
                        StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"] = StartUp.DmctInfo["ma_gd"];

                      //  

                        // update thông tin cho các record Table1 (Ct) 
                        for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
                        {
                            StartUp.DsTrans.Tables[1].DefaultView[i]["ngay_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ngay_ct"];
                            StartUp.DsTrans.Tables[1].DefaultView[i]["so_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"];
                            StartUp.DsTrans.Tables[1].DefaultView[i]["ma_ct"] = StartUp.Ma_ct;
                            StartUp.DsTrans.Tables[1].DefaultView[i]["ma_hd"] = StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"].ToString().Trim();
                            StartUp.DsTrans.Tables[1].DefaultView[i]["ma_kh"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_kh"];
                            if (ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["tl_ck"], 0) == 0)
                            {
                                StartUp.DsTrans.Tables[1].DefaultView[i]["tk_ck"] = string.Empty;
                            }
                        }

                        // update so_ct0 , ngay_ct0,so_seri0 cho Table0 (Ph) , lấy thông tin từ record có tiền thuế lớn nhất trong tab HĐ Thuế
                        decimal _t_tien_nt = 0, _t_tien = 0;
                        decimal _ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);
                        decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien_nt)", "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'").ToString(), out _t_tien_nt);
                        decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien)", "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'").ToString(), out _t_tien);
                        StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt"] = _t_tien_nt;
                        StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien"] = _t_tien;
                       // Cân bằng tiền
                        if (ChkSuaTien.IsChecked == false && _ty_gia != 0)
                        {
                           CanBangTien();
                        }
                        //Lưu tiền thuế trong tab hạch toán để lên bảng kê phiếu nhâp(mẫu nhập mua)
                        PhanBoThueInCT();
                      
                        // kết thúc update

                        StartUp.DsTrans.Tables[0].AcceptChanges();
                        StartUp.DsTrans.Tables[1].AcceptChanges();
                    }
                    DataTable tbPhToSave = StartUp.DsTrans.Tables[0].Clone();
                    StartUp.DsTrans.Tables[0].DefaultView[0]["ngay_lct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ngay_ct"];
                    StartUp.DsTrans.Tables[0].DefaultView[0]["ma_hd"] = StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"].ToString().Trim();
                    StartUp.DsTrans.Tables[0].DefaultView[0]["loai_ct"] = StartUp.DmctInfo["ct_nxt"];
                    //update trường search;
                    if (StartUp.DsTrans.Tables[0].Columns.Contains("search"))
                    {
                        DataTable temp = (StartUp.DsTrans.Tables[0].DefaultView.ToTable());
                        SmLib.SysFunc.SetStrSearch(StartUp.SysObj, "dmhd", ref temp);
                        StartUp.DsTrans.Tables[0].DefaultView[0]["search"] = temp.Rows[0]["search"].ToString().Trim();
                    }
                    tbPhToSave.Rows.Add(StartUp.DsTrans.Tables[0].DefaultView[0].Row.ItemArray);
                    if (!IsSequenceSave)
                    {
                        tbPhToSave.Rows[0]["status"] = 0;
                    }
                    DataProvider.UpdateDataTable(StartUp.SysObj, StartUp.DmctInfo["m_phdbf"].ToString(), "stt_rec", tbPhToSave, "stt_rec;row_id");

                    //DataProvider.DeleteRow(StartUp.SysObj, StartUp.DmctInfo["m_ctdbf"].ToString(), "stt_rec='" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"] + "'");
                    //DataProvider.DeleteRow(StartUp.SysObj, StartUp.DmctInfo["m_ctgtdbf"].ToString(), "stt_rec='" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"] + "'");

                    DataTable tbCtToSave = StartUp.DsTrans.Tables[1].Clone();

                    foreach (DataRowView drv in StartUp.DsTrans.Tables[1].DefaultView)
                    {
                        if (!IsSequenceSave)
                        {
                            drv.Row["so_ct"] = txtSo_ct.Text;
                        }
                        tbCtToSave.Rows.Add(drv.Row.ItemArray);
                    }

                    if (DataProvider.UpdateCtTable(StartUp.SysObj, StartUp.DmctInfo["m_ctdbf"].ToString(), tbCtToSave, StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()) == false)
                    {
                        ExMessageBox.Show( 2285,StartUp.SysObj, "Lưu không thành công, kiểm tra lại dữ liệu!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    #region kiểm tra dưới database
                    if (!IsSequenceSave)
                    {
                        if (!isError)
                        {
                            //if (dsCheckData == null || dsCheckData.Tables[0].Rows.Count == 0)
                                dsCheckData = StartUp.CheckData();

                            dsCheckData.Tables[0].AcceptChanges();
                            if (dsCheckData.Tables.Count > 0)
                            {
                                foreach (DataRowView dv in dsCheckData.Tables[0].DefaultView)
                                {
                                    if (isError)
                                        break;
                                    string result = dv[0].ToString();
                                    //Số ct trùng
                                    switch (result)
                                    {
                                        case "PH01":
                                            {
                                                if (StartUp.M_trung_so.Equals("1"))
                                                {
                                                    if (ExMessageBox.Show( 2290,StartUp.SysObj, "Có chứng từ trùng số. Số cuối cùng là: " + "[" + GetLastSoct(StartUp.SysObj, txtMa_qs.Text).Trim() + "]" + ". Có lưu chứng từ này không?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                                                    {
                                                        txtSo_ct.SelectAll();
                                                        txtSo_ct.Focus();
                                                        isError = true;
                                                    }
                                                }
                                                else if (StartUp.M_trung_so.Equals("2"))
                                                {
                                                    ExMessageBox.Show( 2295,StartUp.SysObj, "Số chứng từ đã tồn tại!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    txtSo_ct.SelectAll();
                                                    txtSo_ct.Focus();
                                                    isError = true;
                                                }
                                            }
                                            break;
                                        case "PH02":
                                            {
                                                ExMessageBox.Show( 2300,StartUp.SysObj, "Tk có là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                isError = true;
                                                txtMa_nx.IsFocus = true;
                                            }
                                            break;
                                        case "CT01":
                                            {
                                                int index = Convert.ToInt16(dv[1]);
                                                ExMessageBox.Show( 2305,StartUp.SysObj, "Tk vật tư là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                isError = true;
                                                tiHT.Focus();
                                                GrdCt.ActiveCell = (GrdCt.Records[index] as DataRecord).Cells["tk_vt"];
                                                GrdCt.Focus();
                                            }
                                            break;
                                    }
                                    dsCheckData.Tables[0].Rows.Remove(dv.Row);
                                }
                            }
                        }
                    }
                    #endregion
                    if (!isError)
                    {
                        string _stt_rec1 = StartUp.DsTrans.Tables[1].DefaultView[0]["stt_rec"].ToString();
                        ThreadStart _thread = delegate()
                        {
                        Post();
                        //Update lại tồn kho tức thời
                        if (!IsSequenceSave)
                        {
                            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                        new Action(() =>
                                        {
                                            if (StartUp.DsTrans.Tables[1].DefaultView[0]["stt_rec"].ToString().Equals(_stt_rec1))
                                            {
                                                //Update số dư vật tư
                                                UpdateTonKho();
                                                //Load số sư khách hàng
                                                loaddataDu13();
                                            }
                                        }));
                        }};

                        (new Thread(_thread)).Start();
                        if (!IsSequenceSave)
                        {
                            //Update thứ tự ph
                            int iRowNew = GetiRow(StartUp.DsTrans.Tables[0], StartUp.DsTrans.Tables[1].DefaultView[0]["stt_rec"].ToString());
                            if (iRow != iRowNew)
                            {
                                DataRow oldRow = StartUp.DsTrans.Tables[0].DefaultView[0].Row;
                                DataRow newRow = StartUp.DsTrans.Tables[0].NewRow();
                                newRow.ItemArray = oldRow.ItemArray;
                                if (iRow > iRowNew)
                                    StartUp.DsTrans.Tables[0].Rows.InsertAt(newRow, iRowNew);
                                else
                                    StartUp.DsTrans.Tables[0].Rows.InsertAt(newRow, iRowNew + 1);
                                StartUp.DsTrans.Tables[0].AcceptChanges();
                                StartUp.DsTrans.Tables[0].Rows.Remove(oldRow);
                                StartUp.DsTrans.Tables[0].AcceptChanges();
                                iRow = iRowNew;

                            }
                            currActionTask = ActionTask.View;
                            IsInEditMode.Value = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        #endregion

        #region cbMa_nt_PreviewLostFocus
        private void cbMa_nt_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Voucher_Ma_nt0 == null)
                return;
            if (cbMa_nt.IsDataChanged)
            {
                StartUp.DsTrans.Tables[0].DefaultView[0]["loai_tg"] = cbMa_nt.RowResult["loai_tg"];
                Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
                IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
                if (cbMa_nt.RowResult != null)
                {
                    if ((cbMa_nt.RowResult)["ma_nt"].ToString().Trim().Equals(StartUp.M_ma_nt0.Trim()))
                    {
                        txtTy_gia.Value = 1;
                    }
                    else
                    {
                        txtTy_gia.Value = StartUp.GetRates((cbMa_nt.RowResult)["ma_nt"].ToString().Trim(), Convert.ToDateTime(txtNgay_ct.Value).Date);
                    }
                }
                TyGiaValueChange();
            }
        }
        #endregion

        #region txtMa_kh_PreviewLostFocus
        private bool txtDiaChiFocusable = true;
        private void txtMa_kh_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (IsInEditMode.Value == true)
            {
                if (txtMa_kh.RowResult == null)
                    return;
                if(!string.IsNullOrEmpty(txtMa_kh.RowResult["dia_chi"].ToString().Trim()))
                    StartUp.DsTrans.Tables[0].DefaultView[0]["dia_chi"] = txtMa_kh.RowResult["dia_chi"].ToString().Trim();
                if (StartUp.M_LAN.Equals("V"))
                {
                    StartUp.DsTrans.Tables[0].DefaultView[0]["ten_kh"] = txtMa_kh.RowResult["ten_kh"].ToString().Trim();
                }
                else
                {
                    StartUp.DsTrans.Tables[0].DefaultView[0]["ten_kh2"] = txtMa_kh.RowResult["ten_kh2"].ToString().Trim();
                }
                if (StartUp.DsTrans.Tables[0].DefaultView[0]["han_tt"] == DBNull.Value || string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["han_tt"].ToString()))
                    StartUp.DsTrans.Tables[0].DefaultView[0]["han_tt"] = txtMa_kh.RowResult["han_tt"].ToString().Trim();
                StartUp.DsTrans.Tables[0].DefaultView[0]["ma_so_thue"] = txtMa_kh.RowResult["ma_so_thue"].ToString().Trim();
                if (string.IsNullOrEmpty(txtOng_ba.Text.Trim()))
                {
                    StartUp.DsTrans.Tables[0].DefaultView[0]["ong_ba"] = txtMa_kh.RowResult["doi_tac"].ToString().Trim();
                }
                StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nx"] = string.IsNullOrEmpty(txtMa_nx.Text.Trim()) ? txtMa_kh.RowResult["tk"].ToString().Trim() : txtMa_nx.Text.Trim();

                if (string.IsNullOrEmpty(txtMa_kh.RowResult["dia_chi"].ToString().Trim()))
                {
                    txtDiaChiFocusable = true;
                }
                else
                {
                    txtDiaChiFocusable = false;
                }
                loaddataDu13();
            }
        }
        #endregion

        #region txtDia_chi_GotFocus
        private void txtDia_chi_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!txtDiaChiFocusable)
            {
                txtDia_chi.IsTabStop = false;
                SmLib.WinAPISenkey.SenKey(ModifierKeys.None, Key.Tab);
            }
        }
        #endregion

        #region txtMa_nx_PreviewLostFocus
        private void txtMa_nx_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtMa_nx.RowResult != null)
            {
                StartUp.DsTrans.Tables[0].DefaultView[0]["ten_nx"] = txtMa_nx.RowResult["ten_nx"].ToString();
                StartUp.DsTrans.Tables[0].DefaultView[0]["ten_nx2"] = txtMa_nx.RowResult["ten_nx2"].ToString();
            }
            else
            {
                StartUp.DsTrans.Tables[0].DefaultView[0]["ten_nx"] = "";
                StartUp.DsTrans.Tables[0].DefaultView[0]["ten_nx2"] = "";
            }
            loaddataDu13();
        }
        #endregion

        #region txtMa_nguoi_ban_PreviewLostFocus
        private void txtMa_nguoi_ban_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtMa_bp.RowResult != null)
            {
                StartUp.DsTrans.Tables[0].DefaultView[0]["ten_bp"] = txtMa_bp.RowResult["ten_bp"].ToString();
                StartUp.DsTrans.Tables[0].DefaultView[0]["ten_bp2"] = txtMa_bp.RowResult["ten_bp2"].ToString();
            }
            else
            {
                StartUp.DsTrans.Tables[0].DefaultView[0]["ten_bp"] = "";
                StartUp.DsTrans.Tables[0].DefaultView[0]["ten_bp2"] = "";
            }
            loaddataDu13();
        }
        #endregion

        #region txtTy_gia_GotFocus
        private void txtTy_gia_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Voucher_Ma_nt0.Value)
            {
                KeyboardNavigation.SetTabNavigation(GrdLayoutNT, KeyboardNavigationMode.Continue);
                SmLib.WinAPISenkey.SenKey(ModifierKeys.None, Key.Tab);
                    }
        }
        #endregion

        #region txtTy_gia_LostFocus
        private void txtTy_gia_LostFocus(object sender, RoutedEventArgs e)
        {
            if (currActionTask == ActionTask.View || currActionTask == ActionTask.None)
            {
                return;
            }

            if (txtTy_gia.OldValue != txtTy_gia.nValue)
            {
                TyGiaValueChange();
            }
        }
        #endregion

        #region TyGiaValueChange
        public void TyGiaValueChange()
        {
            if (cbMa_nt.RowResult != null)
            {
                txtTy_gia.Value = cbMa_nt.RowResult["ma_nt"] == StartUp.M_ma_nt0 ? 1 : txtTy_gia.Value;
            }
            if (string.IsNullOrEmpty(txtTy_gia.Text.ToString()))
            {
                txtTy_gia.Value = 0;
            }

            try
            {
                if (currActionTask == ActionTask.Delete)
                    return;
                if (IsInEditMode.Value == true)
                {
                    if (txtTy_gia.Value != null && txtTy_gia.Value != DBNull.Value && ParseDecimal(txtTy_gia.Value, 0) != 0)
                    {
                        decimal ty_gia = 0, tien_nt2 = 0, tien2 = 0, gia_nt2 = 0, so_luong = 0;
                        decimal t_tien_nt2 = 0, t_cp_nt = 0, thue_nt = 0, thue = 0, ck_nt = 0, gia_nt = 0, tien_nt = 0;
                        ty_gia = txtTy_gia.nValue;

                        t_tien_nt2 = txtT_Tien_nt.Value == DBNull.Value ? 0 : Convert.ToDecimal(txtT_Tien_nt.Value);
                   //     t_cp_nt = txttong_cp_nt.Value == DBNull.Value ? 0 : Convert.ToDecimal(txttong_cp_nt.Value);


                        if (GrdCt.Records.Count > 0 && (GrdCt.DataSource as DataView).Table.DefaultView[0]["ma_vt"] != DBNull.Value)
                        {
                            for (int i = 0; i < GrdCt.Records.Count; i++)
                            {
                                if ((GrdCt.Records[i] as DataRecord).Cells["tien_nt2"].Value != DBNull.Value)
                                {
                                    so_luong = (GrdCt.DataSource as DataView)[i]["so_luong"] == DBNull.Value ? 0 : Convert.ToDecimal((GrdCt.Records[i] as DataRecord).Cells["so_luong"].Value);
                                    gia_nt2 = (GrdCt.DataSource as DataView)[i]["gia_nt2"] == DBNull.Value ? 0 : Convert.ToDecimal((GrdCt.Records[i] as DataRecord).Cells["gia_nt2"].Value);
                                    tien_nt2 = (GrdCt.DataSource as DataView)[i]["tien_nt2"] == DBNull.Value ? 0 : Convert.ToDecimal((GrdCt.Records[i] as DataRecord).Cells["tien_nt2"].Value);
                                    thue_nt = (GrdCt.DataSource as DataView)[i]["thue_nt"] == DBNull.Value ? 0 : Convert.ToDecimal((GrdCt.Records[i] as DataRecord).Cells["thue_nt"].Value);
                                    ck_nt = (GrdCt.DataSource as DataView)[i]["ck_nt"] == DBNull.Value ? 0 : Convert.ToDecimal((GrdCt.Records[i] as DataRecord).Cells["ck_nt"].Value);
                                    gia_nt = (GrdCt.DataSource as DataView)[i]["gia_nt"] == DBNull.Value ? 0 : Convert.ToDecimal((GrdCt.Records[i] as DataRecord).Cells["gia_nt"].Value);
                                    tien_nt = (GrdCt.DataSource as DataView)[i]["tien_nt"] == DBNull.Value ? 0 : Convert.ToDecimal((GrdCt.Records[i] as DataRecord).Cells["tien_nt"].Value);
                                    if (so_luong * gia_nt2 != 0)
                                    {
                                        //tien_nt0 = (GrdCt.DataSource as DataView)[i]["tien_nt0"] == DBNull.Value ? 0 : Convert.ToDecimal((GrdCt.Records[i] as DataRecord).Cells["tien_nt0"].Value);
                                        tien_nt2 = SmLib.SysFunc.Round(so_luong * gia_nt2, StartUp.M_ROUND_NT);
                                        (GrdCt.DataSource as DataView)[i]["tien_nt2"] = tien_nt2;
                                    }
                                    if (ty_gia * gia_nt2 != 0)
                                    {
                                        (GrdCt.DataSource as DataView)[i]["gia2"] = SmLib.SysFunc.Round(ty_gia * gia_nt2, StartUp.M_ROUND_GIA);
                                    }
                                    if (ty_gia * tien_nt2 != 0)
                                    {
                                        (GrdCt.DataSource as DataView)[i]["tien2"] = SmLib.SysFunc.Round(ty_gia * tien_nt2, StartUp.M_ROUND);
                                    }
                                    if (ty_gia * thue_nt != 0)
                                    {
                                        (GrdCt.DataSource as DataView)[i]["thue"] = SmLib.SysFunc.Round(ty_gia * thue_nt, StartUp.M_ROUND);
                                    }
                                    if (ty_gia * ck_nt != 0)
                                    {
                                        (GrdCt.DataSource as DataView)[i]["ck"] = SmLib.SysFunc.Round(ty_gia * ck_nt, StartUp.M_ROUND);
                                    }
                                    if (ty_gia * gia_nt != 0)
                                    {
                                        (GrdCt.DataSource as DataView)[i]["gia"] = SmLib.SysFunc.Round(ty_gia * gia_nt, StartUp.M_ROUND);
                                    }
                                    if (ty_gia * tien_nt != 0)
                                    {
                                        (GrdCt.DataSource as DataView)[i]["tien"] = SmLib.SysFunc.Round(ty_gia * tien_nt, StartUp.M_ROUND);
                                    }
                                }
                            }

                            //decimal t_tien0_InGrdCT = 0;
                            //decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien0)", "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'").ToString(), out t_tien0_InGrdCT);
                            //(GrdLayout20.DataContext as DataView).Table.DefaultView[0]["t_tien0"] = t_tien0_InGrdCT;

                            //for (int i = 0; i < GrdCt.Records.Count; i++)
                            //{
                            //    if (ParseDecimal(((GrdCt.Records[i] as DataRecord).Cells["tien_nt0"].Value.ToString()), 0) != 0)
                            //    {
                            //        decimal tien0_tmp = ParseDecimal(((GrdCt.Records[i] as DataRecord).Cells["tien0"].Value.ToString()), 0);
                            //        //Gán số tiền dư 
                            //        (GrdCt.DataSource as DataView)[i]["tien0"] = tien0_tmp + (_t_tien0_InGrdCT - _t_tien0_InGrdPH);
                            //        break;
                            //    }
                            //}

                            //----------------------------------------Chi Phí------------------------------------
                           //t_cp_nt = txttong_cp_nt.Value == DBNull.Value ? 0 : Convert.ToDecimal(txttong_cp_nt.Value.ToString());
                           // if (GrdCp.Records.Count > 0)
                           // {
                           //     StartUp.DsTrans.Tables[0].DefaultView[0]["t_cp"] = SmLib.SysFunc.Round(t_cp_nt * ty_gia, StartUp.M_ROUND);
                           //     // Phân bổ lại chi phí  
                           //     PhanBo();
                           // }
                            Sum_ALL();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        #endregion

        #region txtMa_qs_PreviewLostFocus
        private void txtMa_qs_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (IsInEditMode.Value)
                if (!string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_qs"].ToString()))
                {
                    if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"].ToString().Trim()))
                    {
                        if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["so_cttmp"].ToString().Trim()) || !StartUp.DsTrans.Tables[0].DefaultView[0]["ma_qs"].ToString().Trim().Equals(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_qstmp"].ToString().Trim()))
                        {
                            txtSo_ct.Text = GetNewSoct(StartUp.SysObj, txtMa_qs.Text);
                            StartUp.DsTrans.Tables[0].DefaultView[0]["so_cttmp"] = txtSo_ct.Text;
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ma_qstmp"] = txtMa_qs.Text;
                        }
                        else
                            txtSo_ct.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["so_cttmp"].ToString().Trim();
                    }
                    if (CheckValidSoct(StartUp.SysObj, txtMa_qs.Text, txtSo_ct.Text, StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()))
                    {
                        txtSo_ct.Text = GetNewSoct(StartUp.SysObj, txtMa_qs.Text);
                        StartUp.DsTrans.Tables[0].DefaultView[0]["so_cttmp"] = txtSo_ct.Text;
                        StartUp.DsTrans.Tables[0].DefaultView[0]["ma_qstmp"] = txtMa_qs.Text;
                    }
                }
        }
        #endregion

        #region txtSo_ct_GotFocus
        private void txtSo_ct_GotFocus(object sender, RoutedEventArgs e)
        {
            //txtSo_ct.Text = txtSo_ct.Text.Trim().ToString();
            DataTable tableFields = null;
            tableFields = SmDataLib.ListFunc.GetSqlTableFieldList(StartUp.SysObj, "v_PH71");
            txtSo_ct.MaxLength = SmDataLib.ListFunc.GetLengthColumn(tableFields, "so_ct");
        }
        #endregion

        #region Sum_ALL
        void Sum_ALL()
        {
            decimal t_ck = 0, t_ck_nt = 0, t_thue = 0, t_thue_nt = 0, t_tien2 = 0, t_tien_nt2 = 0, t_tien_sau = 0, t_sau_ck = 0, t_sau_ck_nt = 0;

            StartUp.DsTrans.Tables[0].AcceptChanges();
            StartUp.DsTrans.Tables[1].AcceptChanges();

            if (cbMa_nt.Text.Equals(StartUp.M_ma_nt0))
            {
                //t_tien0 = SysFunc.Round(ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(tien0)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), 0), StartUp.M_ROUND);
                t_tien_nt2 = SysFunc.Round(ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(tien_nt2)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), 0), StartUp.M_ROUND_NT);
                t_tien2 = t_tien_nt2;

                t_ck = SysFunc.Round(ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(ck)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), 0), StartUp.M_ROUND);
                t_ck_nt = SysFunc.Round(ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(ck_nt)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), 0), StartUp.M_ROUND_NT);
                //t_cp = SysFunc.Round(ParseDecimal(txttong_cp.nValue.ToString(), 0), StartUp.M_ROUND);
               //t_cp = t_cp_nt;

                t_sau_ck = t_tien2 - t_ck;
                t_sau_ck_nt = t_tien_nt2 - t_ck_nt;
                

                t_thue_nt = SysFunc.Round(ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(thue_nt)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), 0), StartUp.M_ROUND_NT);
                t_thue = t_thue_nt;
            }
            else
            {
                t_tien2 = SysFunc.Round(ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(tien2)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), 0), StartUp.M_ROUND);
                t_tien_nt2 = SysFunc.Round(ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(tien_nt2)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), 0), StartUp.M_ROUND_NT);

                t_ck = SysFunc.Round(ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(ck)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), 0), StartUp.M_ROUND);
                t_ck_nt = SysFunc.Round(ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(ck_nt)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), 0), StartUp.M_ROUND_NT);

                t_sau_ck = t_tien2 - t_ck;
                t_sau_ck_nt = t_tien_nt2 - t_ck_nt;
                
                t_thue = SysFunc.Round(ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(thue)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), 0), StartUp.M_ROUND);
                t_thue_nt = SysFunc.Round(ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(thue_nt)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), 0), StartUp.M_ROUND_NT);
            }

            StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien2"] = t_tien2;
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt2"] = t_tien_nt2;
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_ck"] = t_ck;
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_ck_nt"] = t_ck_nt;
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_sau_ck"] = t_sau_ck;
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_sau_ck_nt"] = t_sau_ck_nt;
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue"] = t_thue;
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_nt"] = t_thue_nt;
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_tt"] = t_sau_ck + t_thue;
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_tt_nt"] = t_sau_ck_nt + t_thue_nt;

            StartUp.DsTrans.Tables[0].DefaultView[0]["t_so_luong"] = ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(so_luong)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), 0);
        }
        #endregion

        #region IsVisibilityFieldsXamDataGrid
        void IsVisibilityFieldsXamDataGrid(string ma_nt)
        {
            if (StartUp.M_AR_CK == 0)
            {
                GrdCt.FieldLayouts[0].Fields["ck"].Visibility = Visibility.Hidden;
                GrdCt.FieldLayouts[0].Fields["ck"].Settings.CellMaxWidth = 0;
            }
            //Nếu ngoại tệ = tiền hoạch toán
            if (ma_nt == StartUp.M_ma_nt0)
            {

                //tỷ giá không được sửa
                txtTy_gia.IsReadOnly = true;
            }
            else
            {
                //tỷ giá đựơc sửa
                if (IsInEditMode.Value == true)
                {
                    //tỷ giá đựơc sửa
                    txtTy_gia.IsReadOnly = false;
                }
                else
                {
                    txtTy_gia.IsReadOnly = true;
                }
            }

            //if (currActionTask != ActionTask.Add && currActionTask != ActionTask.Copy)
            //{
            //}
            IsVisibilityFieldsXamDataGridBySua_Tien();
            ChangeLanguage();
        }
        #endregion

        #region IsVisibilityFieldsXamDataGridBySua_Tien
        void IsVisibilityFieldsXamDataGridBySua_Tien()
        {
            //int sua_tien = 0;
            //int.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"].ToString(), out sua_tien);
            //switch (sua_tien)
            //{
            //    #region sua_tien=1
            //    case 1:
            //        {
            //            //nếu check sửa trường tiền
            //            //và đang ở chế độ chỉnh sửa
            //            //thì cho sửa tổng cp hoạch toán
            //            if (IsInEditMode.Value == true)
            //            {
                           
            //                tien_nt2.IsReadOnly = false;
            //                tien_nt2.IsTabStop = true;
            //            }
            //            else
            //            {
            //                tien_nt2.IsReadOnly = true;
            //                tien_nt2.IsTabStop = false;
            //            }
            //        }
            //        break;
            //    #endregion

            //    #region sua_tien=0
            //    case 0:
            //        {
            //            //nếu không check sửa trường tiền
            //            //và tổng cp nt bằng 0
            //            //thì cho sửa tổng cp hoạch toán
            //            decimal _t_cp_nt = 0;
            //            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_cp_nt"].ToString(), out _t_cp_nt);
            //            if (_t_cp_nt == 0)
            //            {
            //                tien_nt2.IsReadOnly = false;
            //                tien_nt2.IsTabStop = true;
            //            }
            //            else
            //            {
                            
            //                //nếu không check sửa trường tiền
            //                //thì không cho sửa tổng cp hoạch toán
            //                tien_nt2.IsReadOnly = true;
            //                txttong_cp.IsTabStop = false;
            //            }
            //        }
            //        break;
            //    #endregion
            //}
            IsCheckedSua_tien.Value = ChkSuaTien.IsChecked.Value;
        }
        #endregion

        #region PhanBoThueInCT
        void PhanBoThueInCT()
        {
            if (StartUp.DsTrans.Tables[1].DefaultView.Count == 0)
                return;

            decimal tong_thue_nt2 = 0, tong_thue2 = 0;
            decimal tong_tien_nt2 = 0, tong_tien2 = 0;
            decimal tong_ck_nt = 0, tong_ck = 0;
            decimal ck_nt = 0, ck = 0;
            decimal tien_nt2 = 0, tien2 = 0;
            decimal thue_nt = 0, thue = 0;
            decimal thue_nt_temp = 0, thue_temp = 0;
            decimal ty_gia = 0;
            string stt_rec = StartUp.DsTrans.Tables[1].DefaultView[0]["stt_rec"].ToString();
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_nt"].ToString(), out tong_thue_nt2);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue"].ToString(), out tong_thue2);
            decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien_nt2)", "stt_rec= '" + stt_rec + "'").ToString(), out tong_tien_nt2);
            decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien2)", "stt_rec= '" + stt_rec + "'").ToString(), out tong_tien2);
            decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(ck_nt)", "stt_rec= '" + stt_rec + "'").ToString(), out tong_ck_nt);
            decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(ck)", "stt_rec= '" + stt_rec + "'").ToString(), out tong_ck);

            for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
            {
                decimal.TryParse(StartUp.DsTrans.Tables[1].DefaultView[i]["tien_nt2"].ToString(), out tien_nt2);
                decimal.TryParse(StartUp.DsTrans.Tables[1].DefaultView[i]["tien2"].ToString(), out tien2);

                decimal.TryParse(StartUp.DsTrans.Tables[1].DefaultView[i]["ck_nt"].ToString(), out ck_nt);
                decimal.TryParse(StartUp.DsTrans.Tables[1].DefaultView[i]["ck"].ToString(), out ck);
                //nếu loại tiền là ngoại tệ
                if (cbMa_nt.Text != StartUp.M_ma_nt0)
                {
                    //nếu tiền nguyên tệ = 0
                    if (tien_nt2 == 0)
                    {
                        thue_nt = 0;
                    }
                    else
                    {
                        thue_nt = tong_tien_nt2 == 0 ? 0 : SmLib.SysFunc.Round(((tien_nt2 - ck_nt) / (tong_tien_nt2 - tong_ck_nt)) * tong_thue_nt2, StartUp.M_ROUND_NT);
                    }

                    //nếu tiền ngoại tệ = 0
                    if (tien2 == 0)
                    {
                        thue = 0;
                    }
                    else
                    {
                        thue = tong_tien2 == 0 ? 0 : SmLib.SysFunc.Round(((tien2 - ck) / (tong_tien2 - tong_ck)) * tong_thue2, StartUp.M_ROUND);
                    }
                }
                else
                {
                    thue_nt = (tong_tien_nt2 == 0 ? 0 : SmLib.SysFunc.Round(((tien_nt2 - ck_nt) / (tong_tien_nt2 - tong_ck_nt)) * tong_thue_nt2, StartUp.M_ROUND_NT));
                    thue = tong_tien2 == 0 ? 0 : SmLib.SysFunc.Round(((tien2 - ck) / (tong_tien2 - tong_ck)) * tong_thue2, StartUp.M_ROUND);
                }

                StartUp.DsTrans.Tables[1].DefaultView[i]["thue_nt"] = thue_nt;
                StartUp.DsTrans.Tables[1].DefaultView[i]["thue"] = thue;
                thue_nt_temp += thue_nt;
                thue_temp += thue;
            }
            StartUp.DsTrans.Tables[1].DefaultView[0]["thue_nt"] = decimal.Parse(StartUp.DsTrans.Tables[1].DefaultView[0]["thue_nt"].ToString()) + (tong_thue_nt2 - thue_nt_temp);
            StartUp.DsTrans.Tables[1].DefaultView[0]["thue"] = decimal.Parse(StartUp.DsTrans.Tables[1].DefaultView[0]["thue"].ToString()) + (tong_thue2 - thue_temp);

            StartUp.DsTrans.Tables[0].AcceptChanges();
            StartUp.DsTrans.Tables[1].AcceptChanges();
        }
        #endregion

        #region PhanBo
        //Phan bo chi phi
        void PhanBo()
        {
            if (StartUp.DsTrans.Tables[1].DefaultView.Count == 0)
                return;
            decimal tong_ck = 0, tong_ck_nt = 0;
            decimal tong_sl = 0;
            decimal tong_tien2 = 0, tong_tien_nt2 = 0;
            decimal ck_temp = 0, ck_nt_temp = 0;
            decimal ck = 0, ck_nt = 0;
            decimal ty_gia = 0;
            string stt_rec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_ck"].ToString(), out tong_ck);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_ck_nt"].ToString(), out tong_ck_nt);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

            //tổng tiền, tiền nt 
            decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien2)", "stt_rec= '" + stt_rec + "'").ToString(), out tong_tien2);
            decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien_nt2)", "stt_rec= '" + stt_rec + "'").ToString(), out tong_tien_nt2);
            decimal tien2 = 0;
            decimal tien_nt2 = 0;

            for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
            {
                decimal.TryParse(StartUp.DsTrans.Tables[1].DefaultView[i]["tien2"].ToString(), out tien2);
                decimal.TryParse(StartUp.DsTrans.Tables[1].DefaultView[i]["tien_nt2"].ToString(), out tien_nt2);
                //nếu loại tiền là ngoại tệ
                if (cbMa_nt.Text != StartUp.M_ma_nt0)
                {
                    //tiền ngoại tệ = 0
                    //thì tính cp_nt theo tiền VND
                    if (tien_nt2 == 0)
                    {
                        ck_nt = tong_tien2 == 0 ? 0 : SmLib.SysFunc.Round((tien2 / tong_tien2) * tong_ck_nt, StartUp.M_ROUND_NT);
                    }
                    else
                        ck_nt = tong_tien_nt2 == 0 ? 0 : SmLib.SysFunc.Round((tien_nt2 / tong_tien_nt2) * tong_ck_nt, StartUp.M_ROUND_NT);
                }
                else
                {
                    ck_nt = tong_tien_nt2 == 0 ? 0 : SmLib.SysFunc.Round((tien_nt2 / tong_tien_nt2) * tong_ck_nt, StartUp.M_ROUND_NT);
                }
                //chi phí = cp nt nhân với tỷ giá
                if (ck_nt != 0)
                {
                    ck = SmLib.SysFunc.Round(ck_nt * ty_gia, StartUp.M_ROUND);
                }
                else
                {
                    if (tong_tien_nt2 != 0)
                    {
                        ck = SmLib.SysFunc.Round((tien_nt2 / tong_tien_nt2) * tong_ck, StartUp.M_ROUND);
                    }
                    else if (tong_tien2 != 0)
                    {
                        ck = SmLib.SysFunc.Round((tien2 / tong_tien2) * tong_ck, StartUp.M_ROUND);
                    }
                    else
                    {
                        ck = 0;
                    }

                }
                StartUp.DsTrans.Tables[1].DefaultView[i]["ck"] = ck;
                StartUp.DsTrans.Tables[1].DefaultView[i]["ck_nt"] = ck_nt;
                ck_temp += ck;
                ck_nt_temp += ck_nt;
            }

            //cộng phần dư vô dòng đầu tiên
            StartUp.DsTrans.Tables[1].DefaultView[0]["ck"] = decimal.Parse(StartUp.DsTrans.Tables[1].DefaultView[0]["ck"].ToString()) + (tong_ck - ck_temp);
            StartUp.DsTrans.Tables[1].DefaultView[0]["ck_nt"] = decimal.Parse(StartUp.DsTrans.Tables[1].DefaultView[0]["ck_nt"].ToString()) + (tong_ck_nt - ck_nt_temp);
        }
        #endregion

        #region txttong_cp_nt_LostFocus
        private void txttong_cp_nt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (currActionTask == ActionTask.Delete || currActionTask == ActionTask.View)
                return;
            IsVisibilityFieldsXamDataGridBySua_Tien();
            //tính lại giá trị của tổng thanh toán nguyên tệ
            //if ((txttong_cp_nt.OldValue != txttong_cp_nt.nValue) || (ChkSuaTien.IsChecked == false))
            //{
            //    if (cbMa_nt.Text == StartUp.M_ma_nt0)
            //    {
            //        txttong_cp.nValue = txttong_cp_nt.nValue;
            //    }
            //    else
            //    {
            //        if (ParseDecimal(txttong_cp_nt.Text.ToString(), 0) * ParseDecimal(txtTy_gia.Text.ToString(), 0) != 0)
            //        {
            //            txttong_cp.nValue = ParseDecimal(txttong_cp_nt.Text.ToString(), 0) * ParseDecimal(txtTy_gia.Text.ToString(), 0);
            //        }
            //    }
            //    Sum_ALL();
            //}
        }
        #endregion

        #region txttong_cp_LostFocus
        private void txttong_cp_LostFocus(object sender, RoutedEventArgs e)
        {
            if (currActionTask == ActionTask.Delete || currActionTask == ActionTask.View)
                return;

            decimal _T_Tien0 = 0, _T_Cp = 0, _T_Thue = 0;
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien0"].ToString(), out _T_Tien0);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_cp"].ToString(), out _T_Cp);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue"].ToString(), out _T_Thue);
            //tính lại giá trị của tổng thanh toán hoạch toán
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_tt"] = _T_Tien0 + _T_Cp + _T_Thue;
        }
        #endregion

        #region btnPhanBo_Click
        private void btnPhanBo_Click(object sender, RoutedEventArgs e)
        {
            PhanBo();
            ExMessageBox.Show( 2310,StartUp.SysObj, "Đã thực hiện xong phân bổ chi phí!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

        #region GrdCp khi ở dòng cuối cùng, cột cuối cùng và Enter thì qua tab HD thuế
        private bool GrdCp_AddNewRecord(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            SmLib.WinAPISenkey.SenKey(ModifierKeys.Alt, Key.D3);
            (this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus();
            return false;
        }
        #endregion

        #region GrdCp_EditModeEnded
        private void GrdCp_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            try
            {
                switch (e.Cell.Field.Name)
                {
                    case "cp_nt":
                        {
                            if (e.Editor.Value == DBNull.Value)
                                e.Cell.Record.Cells["cp_nt"].Value = 0;

                            if (e.Cell.IsDataChanged)
                            {
                                decimal ty_gia = 0, cp_nt = 0;
                                ty_gia = txtTy_gia.nValue;
                                cp_nt = (e.Editor as NumericTextBox).nValue;

                                if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                {
                                    e.Cell.Record.Cells["cp"].Value = e.Cell.Record.Cells["cp_nt"].Value;
                                }
                                else
                                {
                                    if (cp_nt * ty_gia != 0)
                                    {
                                        e.Cell.Record.Cells["cp"].Value = cp_nt * ty_gia;
                                    }
                                }
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        #endregion

        #region Lay_Record_Co_TienHangMax
        private int Lay_Record_Co_TienHangMax()
        {
            int index = 0;
            double maxTien = 0;
            for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
            {
                if (double.Parse(StartUp.DsTrans.Tables[1].DefaultView[i]["tien0"].ToString()) > maxTien)
                {
                    maxTien = double.Parse(StartUp.DsTrans.Tables[1].DefaultView[i]["tien0"].ToString());
                    index = i;
                }
            }
            return index;
        }
        #endregion

        #region loaddataDu13
        void loaddataDu13()
        {
            txtso_du_kh.Value = ArapLib.ArFuncLib.GetSdkh13(StartUp.SysObj, StartUp.DsTrans.Tables[0].DefaultView[0]["ma_kh"].ToString(), StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nx"].ToString());
        }
        #endregion

        #region ParseDecimal
        public decimal ParseDecimal(object obj, decimal defaultvalue)
        {
            decimal ketqua = 0;
            decimal.TryParse(obj != null ? obj.ToString() : defaultvalue.ToString(), out ketqua);
            return ketqua;
        }
        #endregion

        #region GetLanguageString
        public override string GetLanguageString(string code, string language)
        {
            return StartUp.GetLanguageString(code, language);
        }
        #endregion

        #region txtghi_chu_PreviewKeyDown
        private void txtghi_chu_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Enter) && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
            {
                TextBox txt = sender as TextBox;
                txt.SelectedText = Environment.NewLine;
                txt.SelectionStart = txt.SelectionStart + 1;
                txt.SelectionLength = 1;
                e.Handled = true;
            }
            else if (Keyboard.IsKeyDown(Key.Enter))
            {
                (this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus();
                e.Handled = true;
            }
        }
        #endregion

        #region CanBangTien
        public void CanBangTien()
        {
            decimal t_tien_nt2_InPH = 0, t_tien2_InPH = 0, t_tien_nt2_InGrdCT = 0, t_tien2_InGrdCT = 0, ty_gia = 1;
            StartUp.DsTrans.Tables[0].AcceptChanges();
            StartUp.DsTrans.Tables[1].AcceptChanges();

            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt2"].ToString(), out t_tien_nt2_InPH);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien2"].ToString(), out t_tien2_InPH);
            decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien_nt2)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), out t_tien_nt2_InGrdCT);
            decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien2)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), out t_tien2_InGrdCT);

            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

            //Tiền VND trong PH bằng tiền nt trong PH * tỷ giá
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien2"] = SmLib.SysFunc.Round(t_tien_nt2_InPH * ty_gia, StartUp.M_ROUND);
            //Lấy tổng tiền VND trong PH trừ tổng tiền VND trong GrdCT, phần còn dư gán vào dòng đầu tiên tổng tiền VND trong GrdCT
            for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
            {
                if (ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["tien_nt2"], 0) != 0)
                {
                    StartUp.DsTrans.Tables[1].DefaultView[i]["tien2"] = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["tien2"], 0) + (ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien2"], 0) - t_tien2_InGrdCT);
             //       StartUp.DsTrans.Tables[1].DefaultView[i]["t_tien"] = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["tien2"], 0) + ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["ck"], 0);
                    break;
                }
            }

            //Tính lại tổng thanh toán
            decimal t_tien_nt2 = 0, t_tien2 = 0, t_ck_nt = 0, t_ck = 0, t_thue_nt = 0, t_thue = 0;

            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt2"].ToString(), out t_tien_nt2);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien2"].ToString(), out t_tien2);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_ck_nt"].ToString(), out t_ck_nt);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_ck"].ToString(), out t_ck);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_nt"].ToString(), out t_thue_nt);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue"].ToString(), out t_thue);
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_tt_nt"] = t_tien_nt2 - t_ck_nt + t_thue_nt;
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_tt"] = t_tien2 - t_ck + t_thue;
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_sau_ck"] = t_tien2 - t_ck;
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_sau_ck_nt"] = t_tien_nt2 - t_ck_nt;
            StartUp.DsTrans.Tables[0].AcceptChanges();
            StartUp.DsTrans.Tables[1].AcceptChanges();

        }
        #endregion

        #region ParseInt
        public int ParseInt(object obj, int defaultvalue)
        {
            int ketqua = defaultvalue;
            int.TryParse(obj != null ? obj.ToString() : defaultvalue.ToString(), out ketqua);
            return ketqua;
        }
        #endregion

        #region UpdateTonKho
        private void UpdateTonKho()
        {
            for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
            {
                if (ParseInt(StartUp.DsTrans.Tables[1].DefaultView[i]["vt_ton_kho"], 0) == 1)
                {
                    object ton_moi = InvtLib.InFuncLib.GetTon13(StartUp.SysObj, StartUp.DsTrans.Tables[1].DefaultView[i]["ma_kho_i"].ToString(), StartUp.DsTrans.Tables[1].DefaultView[i]["ma_vt"].ToString(), StartUp.DsTrans.Tables[1].DefaultView[i]["ma_vv_i"].ToString());
                    StartUp.DsTrans.Tables[1].DefaultView[i]["ton13"] = ton_moi;
                }
                else
                    StartUp.DsTrans.Tables[1].DefaultView[i]["ton13"] = DBNull.Value;
            }
            StartUp.DsTrans.Tables[1].AcceptChanges();
        }
        #endregion

        private void txtSo_ct_me_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

        }

        private void txtHan_tt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!txtHan_tt.IsFocusWithin)
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    (this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus();
                }));

        }

        private void txtMa_bp_GotFocus(object sender, RoutedEventArgs e)
        {
            if (StartUp.M_BP_BH != "1")
                txtNgay_ct.Focus();
        }

        private void txtHan_tt_GotFocus(object sender, RoutedEventArgs e)
        {
            txtHan_tt.SelectAll();
        }

        private void txtHan_tt_TextChanged(object sender, RoutedPropertyChangedEventArgs<string> e)    
        {
            Dispatcher.BeginInvoke((ThreadStart)delegate
            {
                if (txtHan_tt.Text.IndexOf('-') >= 0)
                txtHan_tt.Value = 0;
            });
           
        }

        private void txtNgay_ct_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtNgay_ct.Value == DBNull.Value)
                txtNgay_ct.Value = DateTime.Now;
        }

        private void txtSo_ct_LostFocus(object sender, RoutedEventArgs e)
        {
            if (currActionTask == ActionTask.Edit)
            {
                if (txtSo_ct.Text.Trim() != ma_hd)
                {
                    if (KiemTraCoPhatSinh())
                    {
                        ExMessageBox.Show(2370, StartUp.SysObj, "Hợp đồng đã có phát sinh, không được sửa số hợp đồng!","", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtSo_ct.Text = ma_hd;
                    }
                }
            }
        }

        void Post()
        {
            SqlCommand PostCmd = new SqlCommand("exec [TTSODMHDB-Post] @stt_rec");
            PostCmd.Parameters.Add("@stt_rec", SqlDbType.VarChar).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();
            StartUp.SysObj.ExcuteNonQuery(PostCmd);
        }

    }
}

