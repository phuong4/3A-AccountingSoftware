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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Diagnostics;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using Sm.Windows.Controls;
using SmVoucherLib;
using SmDefine;
using System.Data.SqlClient;
using SmLib;
using Sm.Windows.Controls.ControlLib;
using System.Windows.Threading;
using System.Threading;


namespace INCTPND
{
    /// <summary>
    /// Interaction logic for FrmINCTPND.xaml
    /// </summary>
    public partial class FrmINCTPND : SmVoucherLib.FormTrans
    {
        public static int iRow = 0;
        public static int OldiRow = 0;
        public string Old_ma_kho = string.Empty;
        CodeValueBindingObject Voucher_Ma_nt0;
        CodeValueBindingObject Voucher_Lan0;
        public static CodeValueBindingObject IsInEditMode;
        CodeValueBindingObject IsCheckedSua_tien;
        CodeValueBindingObject IsCheckedPn_gia_tb;

        //Lưu lại dữ liệu khi thêm sửa
        DataSet dsCheckData;
        private DataSet DsVitual;

        public FrmINCTPND()
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
                currActionTask = ActionTask.View;

                //Gan1 iRow ở phiếu cuối cùng
                if (StartUp.DsTrans.Tables[0].Rows.Count > 1)
                    iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;

                IsInEditMode = (CodeValueBindingObject)FormMain.FindResource("IsInEditMode");
                Voucher_Ma_nt0 = (CodeValueBindingObject)FormMain.FindResource("Voucher_Ma_nt0");
                Voucher_Lan0 = (CodeValueBindingObject)FormMain.FindResource("Voucher_Lan0");
                IsCheckedSua_tien = (CodeValueBindingObject)FormMain.FindResource("IsCheckedSua_tien");
                IsCheckedPn_gia_tb = (CodeValueBindingObject)FormMain.FindResource("IsCheckedPn_gia_tb");

                //Binding EditMode cho FormTrans
                Binding bind = new Binding("Value");
                bind.Source = IsInEditMode;
                bind.Mode = BindingMode.OneWay;
                this.SetBinding(FormTrans.IsEditModeProperty, bind);

                //Gán ngôn ngữ messagebox
                M_LAN = StartUp.M_LAN;
                GrdCt.Lan = StartUp.M_LAN;

                LanguageProvider.Language = StartUp.M_LAN;

                txtGhi_chu.Text = M_LAN.Equals("V") ? "Ghi chú" : "Notes";
                //Them cac truong tu do
                SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, GrdCt, StartUp.Ma_ct, 1);

                if (StartUp.DsTrans.Tables[0].Rows.Count > 0)
                {
                    LoadData();
                    //Xét lại các Field khi thay đổi record hiển thị
                    IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
                    IsCheckedSua_tien.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"].ToString() == "1");
                    IsCheckedPn_gia_tb.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["pn_gia_tb"].ToString() == "1");
                    UpdateTonKho();
                }
                Voucher_Lan0.Value = M_LAN.Equals("V");
                Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
                //Luu lai ma kho cua phieu truoc
                if (StartUp.DsTrans.Tables[1].DefaultView.Count > 0)
                {
                    Old_ma_kho = StartUp.DsTrans.Tables[1].DefaultView[0]["ma_kho_i"].ToString();
                }
                //Sửa lỗi binding numerictextbox format sai lần đâu tiên khi load form
 
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
            StartUp.DsTrans.Tables[1].DefaultView.Sort = "stt_rec0 ASC";

            this.GrdLayout00.DataContext = StartUp.DsTrans.Tables[0].DefaultView;
            this.GrdLayout10.DataContext = StartUp.DsTrans.Tables[0].DefaultView;
            this.GrdLayout20.DataContext = StartUp.DsTrans.Tables[0].DefaultView;
            this.GrdLayout21.DataContext = StartUp.DsTrans.Tables[0].DefaultView;
            //GroupBox Tổng cộng: số lượng
            this.GrdLayout22.DataContext = StartUp.DsTrans.Tables[0].DefaultView;

            //Nạp dữ liệu cho Grid hàng hóa, chi phí và hd thuế
            this.GrdCt.DataSource = StartUp.DsTrans.Tables[1].DefaultView;

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
                        txtMa_gd.IsFocus = true;
                    }));
                    DsVitual = StartUp.DsTrans.Copy();

                    //Them moi dong trong Ph
                    DataRow NewRecord = StartUp.DsTrans.Tables[0].NewRow();
                    NewRecord["stt_rec"] = newSttRec;
                    NewRecord["ma_ct"] = StartUp.Ma_ct;
                    NewRecord["ma_gd"] = StartUp.M_ma_gd;
                    if (SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, txtNgay_ct.dValue))
                    {
                        NewRecord["ngay_ct"] = txtNgay_ct.dValue.Date;
                    }
                    else
                    {
                        NewRecord["ngay_ct"] = DateTime.Now.Date;
                    }
                    NewRecord["status"] = StartUp.DmctInfo["ma_post"];
                   // NewRecord["ma_nt"] = string.IsNullOrEmpty(cbMa_nt.Text) ? StartUp.M_ma_nt0 : cbMa_nt.Text;
                    NewRecord["sua_tien"] = 0;
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

                    if (NewRecord["ma_nt"].ToString().Trim().Equals(StartUp.M_ma_nt0.Trim()))
                    {
                        NewRecord["ty_giaf"] = 1;
                    }
                    else
                    {
                        NewRecord["ty_giaf"] = StartUp.GetRates(NewRecord["ma_nt"].ToString().Trim(), Convert.ToDateTime(NewRecord["ngay_ct"]).Date);
                    }
                    NewRecord["t_tien"] = 0;
                    NewRecord["t_tien_nt"] = 0;
                    NewRecord["t_so_luong"] = 0;

                    StartUp.DsTrans.Tables[0].Rows.Add(NewRecord);

                    StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";
                    StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";

                    //Them moi dong trong CT
                    NewRowCt();

                    //Refresh lai form
                    StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";
                    StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";

                    txtngay_lct.Text = "";
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
            FrmINCTPNDCopy _formcopy = new FrmINCTPNDCopy();
            _formcopy.Closed += new EventHandler(_formcopy_Closed);
            _formcopy.ShowDialog();
        }
        #endregion

        #region _formcopy_Closed
        void _formcopy_Closed(object sender, EventArgs e)
        {
            if (FrmINCTPNDCopy.isCopy == true)
            {
                string newSttRec = DataProvider.NewTrans(StartUp.SysObj, StartUp.Ma_ct, StartUp.Ws_Id);
                if (!string.IsNullOrEmpty(newSttRec))
                {
                    DsVitual = StartUp.DsTrans.Copy();
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background
                    , new Action(() =>
                    {
                        txtMa_gd.IsFocus = true;
                    }));
                    //Them moi dong trong Ph
                    DataRow NewRecord = StartUp.DsTrans.Tables[0].NewRow();
                    //copy dữ liệu từ row được chọn copy cho row mới
                    NewRecord.ItemArray = StartUp.DsTrans.Tables[0].Rows[iRow].ItemArray;
                    //gán lại stt_rec, ngày ct
                    NewRecord["stt_rec"] = newSttRec;
                    NewRecord["ngay_ct"] = FrmINCTPNDCopy.ngay_ct;
                        //NewRecord["ngay_lct"] = FrmINCTPNDCopy.ngay_ct;

                    NewRecord["ma_qs"] = GetDMQS(BindingSysObj, StartUp.Ma_ct, Convert.ToDateTime(NewRecord["ngay_ct"]),
                             StartUp.M_User_Id, NewRecord["ma_qs"].ToString().Trim());
                    if (NewRecord["ma_qs"].ToString().Trim() != "")
                        NewRecord["so_ct"] = GetNewSoct(StartUp.SysObj, NewRecord["ma_qs"].ToString());
                    else
                        NewRecord["so_ct"] = "";
                    NewRecord["so_cttmp"] = NewRecord["so_ct"];
                    StartUp.DsTrans.Tables[0].Rows.Add(NewRecord);

                    //add các row trong GrdCt
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

        #region V_Sua
        private void V_Sua()
        {
            if (StartUp.DsTrans.Tables[0].Rows.Count == 0)
                ExMessageBox.Show( 420,StartUp.SysObj, "Không có dữ liệu!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                if (!SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, txtNgay_ct.dValue))
                {
                    ExMessageBox.Show( 425,StartUp.SysObj, "Ngày hạch toán phải sau ngày khóa sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background
                , new Action(() =>
                {
                    txtMa_gd.IsFocus = true;
                }));
                currActionTask = ActionTask.Edit;

                DsVitual = new DataSet();
                DsVitual.Tables.Add(StartUp.DsTrans.Tables[0].DefaultView.ToTable());
                DsVitual.Tables.Add(StartUp.DsTrans.Tables[1].DefaultView.ToTable());
                IsInEditMode.Value = true;

                Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
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
                            //xóa các row trong table[1]
                            string stt_rec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();

                            // Nên dịch chuyển iRow lùi dòng 0
                            // Sau đó RowFilter lại Table[0], Table[1]
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
        private void V_Xoa()
        {
            if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim()))
                return;

            currActionTask = ActionTask.Delete;
            try
            {
                string _stt_rec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();

                //Delete tksd13
                StartUpTrans.UpdateTkSd13(1, 0);

                //xóa trong ph, ct, ctgt
                //xóa chứng từ
                StartUp.DeleteVoucher(_stt_rec);

                // ----Warning : Không nên xóa Table[0] trước, nếu xóa trước sẽ bị mất Binding -----------------------
                // Nên dịch chuyển iRow lùi 1 dòng
                // Sau đó RowFilter lại Table[0], Table[1]
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
                stringBrowse1 = StartUp.CommandInfo["Vbrowse1"].ToString().Split('|')[0];//"ngay_ct:fl:100:h=Ngày c.từ;so_ct:fl:70:h=Số c.từ;ma_kh:100:h=Mã khách;ten_kh:180:h=Tên khách;t_tien_nt:130:n1:h=Tổng tiền nt;t_tien:130:n0:h=Tổng tiền;dien_giai:225:h=Diễn giải;ma_nt:80:h=Loại tiền;ty_gia:130:h=Tỷ giá:r;date:140:h=Ngày cập nhật;time:140:h=Giờ cập nhật;user_id:80:n:h=Số hiệu NSD;user_name:180:h=Tên NSD";
                stringBrowse2 = StartUp.CommandInfo["Vbrowse1"].ToString().Split('|')[1];//"ma_vt:fl:100:h=Mã vật tư;ten_vt:fl:270:h=Tên vật tư;dvt:60:h=Đvt;ma_kho_i:70:h=Mã kho;so_luong:q:130:h=Số lượng;gia_nt:p1:130:h=Giá nt;tien_nt:130:n1:h=Tiền nt;tk_vt:80:h=Tk nợ;ma_nx_i:80:h=Tk có;gia:130:p0:h=Giá;tien:130:n0:h=Tiền";
            }
            else
            {
                stringBrowse1 = StartUp.CommandInfo["Ebrowse1"].ToString().Split('|')[0];
                stringBrowse2 = StartUp.CommandInfo["Ebrowse1"].ToString().Split('|')[1];
            }
            StartUp.DsTrans.Tables[0].AcceptChanges();
            DataTable PhViewTablev = StartUp.DsTrans.Tables[0].Copy();
            PhViewTablev.Rows.RemoveAt(0);
            SmVoucherLib.FormView _frmView = new SmVoucherLib.FormView(StartUp.SysObj, PhViewTablev.DefaultView, StartUp.DsTrans.Tables[1].DefaultView, stringBrowse1, stringBrowse2, "stt_rec");
            _frmView.ListFieldSum = "t_tien_nt;t_tien";
            _frmView.frmBrw.Title = M_LAN.Equals("V") ? SmLib.SysFunc.Cat_Dau(StartUp.CommandInfo["bar"].ToString()) : SmLib.SysFunc.Cat_Dau(StartUp.CommandInfo["bar2"].ToString());

            SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, _frmView.frmBrw.oBrowseCt, StartUp.Ma_ct, 1);

            _frmView.frmBrw.LanguageID  = "INCTPND_5";
            _frmView.ShowDialog();

            // Set lai irow va rowfilter ...
            if (_frmView.DataGrid.ActiveRecord != null)
            {
                int select_item_index = (_frmView.DataGrid.ActiveRecord as DataRecord).DataItemIndex;
                //int select_irow = (_frmView.DataGrid.ActiveRecord as DataRecord).Index;
                if (select_item_index >= 0)
                {
                    string selected_stt_rec = (_frmView.DataGrid.DataSource as DataView)[select_item_index]["stt_rec"].ToString();
                    FrmINCTPND.iRow = select_item_index + 1;
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

        #region V_Nhan
        private void V_Nhan()
        {
            try
            {
                bool isError = false;
                if (!IsSequenceSave)
                {
                    StartUp.DsTrans.Tables[0].AcceptChanges();
                    StartUp.DsTrans.Tables[1].AcceptChanges();

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
                        ExMessageBox.Show( 430,StartUp.SysObj, "Chưa vào mã khách hàng!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtMa_kh.IsFocus = true;
                        isError = true;
                    }
                    else if (string.IsNullOrEmpty(txtNgay_ct.Text.ToString()))
                    {
                        ExMessageBox.Show( 435,StartUp.SysObj, "Chưa vào ngày hạch toán!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtNgay_ct.Focus();
                        isError = true;
                    }

                    else if ( StartUp.M_NGAY_BAT_DAU != null && (!txtNgay_ct.IsValueValid || txtNgay_ct.dValue < StartUp.M_NGAY_BAT_DAU || txtNgay_ct.dValue > StartUp.M_NGAY_KET_THUC))
                        {
                            ExMessageBox.Show(1024, StartUp.SysObj, "Ngày hạch toán không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            isError = true;
                            txtNgay_ct.Focus();
                        }
                    else if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"].ToString()))
                    {
                        ExMessageBox.Show( 440,StartUp.SysObj, "Chưa vào số chứng từ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtSo_ct.Focus();
                        isError = true;
                    }

                    //else if (CheckValidSoct(StartUp.SysObj, txtMa_qs.Text, txtSo_ct.Text, StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()))
                    //{
                    //    if (StartUp.M_trung_so.Equals("1"))
                    //    {
                    //        if (ExMessageBox.Show( 445,StartUp.SysObj, "Có chứng từ trùng số. Số cuối cùng là " + "[" + GetLastSoct(StartUp.SysObj, txtMa_qs.Text).Trim() + "]" + ". Có lưu chứng từ này không?", "Fast Book 11 .NET", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                    //        {
                    //            txtSo_ct.SelectAll();
                    //            txtSo_ct.Focus();
                    //            isError = true;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        ExMessageBox.Show( 450,StartUp.SysObj, "Số chứng từ đã tồn tại!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                    //        txtSo_ct.SelectAll();
                    //        txtSo_ct.Focus();
                    //        isError = true;
                    //    }
                    //}
                }
                if (!isError)
                {
                    if (!IsSequenceSave)
                    {
                        if (StartUp.DsTrans.Tables[1].DefaultView.Count > 0)
                        {
                            for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
                            {
                                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["ma_vt"].ToString()))
                                {
                                    ExMessageBox.Show( 455,StartUp.SysObj, "Chưa vào chi tiết vật tư, không lưu được!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                    GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["ma_vt"];
                                    GrdCt.Focus();
                                    return;
                                }
                                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["ma_kho_i"].ToString()))
                                {
                                    ExMessageBox.Show( 460,StartUp.SysObj, "Chưa vào chi tiết vật tư, không lưu được!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                    GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["ma_kho_i"];
                                    GrdCt.Focus();
                                    return;
                                }

                                //Kiem tra tk_vt(tk nợ)
                                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_vt"].ToString().Trim()))
                                {
                                    ExMessageBox.Show( 465,StartUp.SysObj, "Chưa vào tk nợ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                    GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_vt"];
                                    GrdCt.Focus();
                                    return;
                                }


                                //Kiem tra tk_vt(tk nợ)
                                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["ma_nx_i"].ToString().Trim()))
                                {
                                    ExMessageBox.Show( 470,StartUp.SysObj, "Chưa vào tk có!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                    GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["ma_nx_i"];
                                    GrdCt.Focus();
                                    return;
                                }

                                if (int.Parse(StartUp.DsTrans.Tables[1].DefaultView[i]["gia_ton"].ToString()) == 3)
                                {
                                    if (decimal.Parse(StartUp.DsTrans.Tables[1].DefaultView[i]["so_luong"].ToString()) == 0)
                                    {
                                        ExMessageBox.Show( 475,StartUp.SysObj, "Vật tư tính tồn kho theo phương pháp NTXT không được nhập số lượng = 0!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                        GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["so_luong"];
                                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                        {
                                            GrdCt.Focus();
                                        }));
                                        return;
                                    }
                                }
                                //ma loi 1220A001
                                if (StartUp.DsTrans.Tables[1].DefaultView[i]["loai_vt"].ToString().Equals("51"))
                                {
                                    if (decimal.Parse(StartUp.DsTrans.Tables[1].DefaultView[i]["so_luong"].ToString()) == 0)
                                    {
                                        ExMessageBox.Show(476, StartUp.SysObj, "Chưa có số lượng nhập kho!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                        {
                                            GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["so_luong"];
                                            GrdCt.Focus();
                                        }));
                                        return;
                                    }
                                }
                                
                                //if (StartUp.IsTkMe(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_vt"].ToString().Trim()))
                                //{
                                //    ExMessageBox.Show( 480,StartUp.SysObj, "Tk nợ là tk tổng hợp, không lưu được!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                //    GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_vt"];
                                //    GrdCt.Focus();
                                //    return;
                                //}
                                //if (StartUp.IsTkMe(StartUp.DsTrans.Tables[1].DefaultView[i]["ma_nx_i"].ToString().Trim()))
                                //{
                                //    ExMessageBox.Show( 485,StartUp.SysObj, "Tk có là tk tổng hợp, không lưu được!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                //    GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["ma_nx_i"];
                                //    GrdCt.Focus();
                                //    return;
                                //}


                            }
                        }
                        else
                        {
                            ExMessageBox.Show( 490,StartUp.SysObj, "Chưa vào chi tiết vật tư, không lưu được!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                            GrdCt.Focus();
                            isError = true;
                        }
                    }
                    if (!isError)
                    {
                        // update thông tin cho các record Table1 (Ct) 
                        if (!IsSequenceSave)
                        {
                            for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
                            {
                                StartUp.DsTrans.Tables[1].DefaultView[i]["ngay_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ngay_ct"];
                                StartUp.DsTrans.Tables[1].DefaultView[i]["so_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"];
                                StartUp.DsTrans.Tables[1].DefaultView[i]["ma_ct"] = StartUp.Ma_ct;
                            }

                            // update date0 , time0 , user_id 
                            //if (currActionTask == ActionTask.Add || currActionTask == ActionTask.Copy)
                            //{
                            //    StartUp.DsTrans.Tables[0].DefaultView[0]["user_id0"] = StartUp.M_User_Id;
                            //    StartUp.DsTrans.Tables[0].DefaultView[0]["date0"] = DateTime.Now.Date;
                            //    StartUp.DsTrans.Tables[0].DefaultView[0]["time0"] = DateTime.Now.ToString("HH:mm:ss");
                            //}

                            // update date , time , user_id 
                            //StartUp.DsTrans.Tables[0].DefaultView[0]["date"] = DateTime.Now.Date;
                            //StartUp.DsTrans.Tables[0].DefaultView[0]["time"] = DateTime.Now.ToString("HH:mm:ss");
                            //StartUp.DsTrans.Tables[0].DefaultView[0]["user_id"] = StartUp.M_User_Id;
                            //StartUp.DsTrans.Tables[0].DefaultView[0]["user_name"] = StartUp.M_User_name;
                            // update ty_giaf = ty_gia
                            decimal _ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);

                            

                            //Cân bằng tiền
                            if (ChkSuaTien.IsChecked == false && _ty_gia != 0)
                            {
                                CanBangTien();
                            }

                            // kết thúc update
                            StartUp.DsTrans.Tables[0].AcceptChanges();
                            StartUp.DsTrans.Tables[1].AcceptChanges();
                        }
                        DataTable tbPhToSave = StartUp.DsTrans.Tables[0].Clone();
                        tbPhToSave.Rows.Add(StartUp.DsTrans.Tables[0].DefaultView[0].Row.ItemArray);
                        if (!IsSequenceSave)
                        {
                            tbPhToSave.Rows[0]["status"] = 0;
                        }
                        DataProvider.UpdateDataTable(StartUp.SysObj, StartUp.DmctInfo["m_phdbf"].ToString(), "stt_rec", tbPhToSave, "stt_rec;row_id");

                        //DataProvider.DeleteRow(StartUp.SysObj, StartUp.DmctInfo["m_ctdbf"].ToString(), "stt_rec='" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"] + "'");

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
                            ExMessageBox.Show( 495,StartUp.SysObj, "Lưu không thành công, kiểm tra lại dữ liệu!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        if (!IsSequenceSave)
                        {
                            //Check data trên server
                            /////////////////////////////////////////////
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
                                                        if (ExMessageBox.Show( 500,StartUp.SysObj, "Có chứng từ trùng số. Số cuối cùng là: " + "[" + GetLastSoct(StartUp.SysObj, txtMa_qs.Text).Trim() + "]" + ". Có lưu chứng từ này không?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                                                        {
                                                            txtSo_ct.SelectAll();
                                                            txtSo_ct.Focus();
                                                            isError = true;
                                                        }
                                                    }
                                                    else if (StartUp.M_trung_so.Equals("2"))
                                                    {
                                                        ExMessageBox.Show( 505,StartUp.SysObj, "Số chứng từ đã tồn tại!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                                        txtSo_ct.SelectAll();
                                                        txtSo_ct.Focus();
                                                        isError = true;
                                                    }
                                                    break;
                                                }
                                            case "CT01":
                                                {
                                                    int index = Convert.ToInt16(dv[1]);
                                                    ExMessageBox.Show( 510,StartUp.SysObj, "Tk nợ là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    isError = true;
                                                    GrdCt.ActiveCell = (GrdCt.Records[index] as DataRecord).Cells["tk_vt"];
                                                    GrdCt.Focus();
                                                }
                                                break;
                                            case "CT02":
                                                {
                                                    int index = Convert.ToInt16(dv[1]);
                                                    ExMessageBox.Show( 515,StartUp.SysObj, "Tk có là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    isError = true;
                                                    GrdCt.ActiveCell = (GrdCt.Records[index] as DataRecord).Cells["ma_nx_i"];
                                                    GrdCt.Focus();
                                                }
                                                break;
                                            case "CT08":
                                                {
                                                    int index = Convert.ToInt16(dv[1]);

                                                    ExMessageBox.Show(9410, StartUp.SysObj, "Chưa vào mã vv, mã bộ phận, mã phí => không lưu được kiểm tra lại dữ liệu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    isError = true;

                                                }
                                                break;
                                        }
                                        dsCheckData.Tables[0].Rows.Remove(dv.Row);
                                    }
                                }
                            }
                        }
                        if (!isError)
                        {
                            string stt_rec1 = StartUp.DsTrans.Tables[1].DefaultView[0]["stt_rec"].ToString();
                            ThreadStart _thread = delegate()
                            {
                                Post();
                                if (!IsSequenceSave)
                                {
                                    Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                           new Action(() =>
                                           {
                                               if (StartUp.DsTrans.Tables[1].DefaultView[0]["stt_rec"].ToString().Equals(stt_rec1))
                                                   UpdateTonKho();
                                           }));
                                }
                            };
                            new Thread(_thread).Start();
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
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        #endregion

        void Post()
        {
            SqlCommand PostCmd = new SqlCommand("exec [INCTPND-Post] @stt_rec");
            PostCmd.Parameters.Add("@stt_rec", SqlDbType.VarChar).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();
            StartUp.SysObj.ExcuteNonQuery(PostCmd);
        }

        #region FormMain_EditModeEnded
        //Ham nay dung de xu ly sau khi an mot button 
        private void FormMain_EditModeEnded(object sender, string menuItemName, RoutedEventArgs e)
        {
            IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
            Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
            Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
            if (!menuItemName.Equals("btnSave"))
            {
                UpdateTonKho();
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

                int Stt_rec0 = 0, Stt_rec0ct = 0;
                if (GrdCt.Records.Count > 0)
                {
                    var _max_sttrec0ct = StartUp.DsTrans.Tables[1].AsEnumerable()
                                       .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                                       .Max(x => x.Field<string>("stt_rec0"));
                    if (_max_sttrec0ct != null)
                        int.TryParse(_max_sttrec0ct.ToString(), out Stt_rec0ct);
                    NewRecord["ma_kho_i"] = (GrdCt.Records[0] as DataRecord).Cells["ma_kho_i"].Value;
                    NewRecord["ma_nx_i"] = (GrdCt.Records[0] as DataRecord).Cells["ma_nx_i"].Value;

                }
                else
                {
                    NewRecord["ma_kho_i"] = "";
                    NewRecord["ma_nx_i"] = "";
                }
                Stt_rec0 = Stt_rec0ct;
                Stt_rec0++;

                NewRecord["stt_rec0"] = string.Format("{0:000}", Stt_rec0);
                NewRecord["ma_ct"] = StartUp.Ma_ct;
                NewRecord["ngay_ct"] = txtNgay_ct.Value == null ? DateTime.Now.Date : txtNgay_ct.dValue.Date;
                
                NewRecord["so_luong"] = 0;
                NewRecord["gia_nt"] = 0;
                NewRecord["tien_nt"] = 0;
                NewRecord["tien"] = 0;
                NewRecord["gia"] = 0;
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

        #region txtMa_kh_PreviewLostFocus
        private bool txtDiaChiFocusable = true;
        private void txtMa_kh_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (IsInEditMode.Value == true)
            {
                if (txtMa_kh.RowResult == null)
                    return;

                var ten_kh = txtMa_kh.RowResult["ten_kh"].ToString().Trim();
                var ten_kh2 = txtMa_kh.RowResult["ten_kh2"].ToString().Trim();
                tblTen_kh.Text = StartUp.M_LAN.Equals("V") ? ten_kh : ten_kh2;
                StartUp.DsTrans.Tables[0].Rows[iRow]["ten_kh"] = ten_kh;
                StartUp.DsTrans.Tables[0].Rows[iRow]["ten_kh2"] = ten_kh2;
                StartUp.DsTrans.Tables[0].AcceptChanges();
                //StartUp.DsTrans.Tables[0].DefaultView[0]["ten_kh"] = txtMa_kh.RowResult["ten_kh"].ToString().Trim();
                if (!string.IsNullOrEmpty(txtMa_kh.RowResult["doi_tac"].ToString().Trim()))
                {
                    StartUp.DsTrans.Tables[0].DefaultView[0]["ong_ba"] = txtMa_kh.RowResult["doi_tac"].ToString().Trim();
                }

                if (string.IsNullOrEmpty(txtMa_kh.RowResult["dia_chi"].ToString().Trim()))
                {
                    txtDiaChiFocusable = true;
                }
                else
                {
                    StartUp.DsTrans.Tables[0].DefaultView[0]["dia_chi"] = txtMa_kh.RowResult["dia_chi"].ToString().Trim();
                    txtDiaChiFocusable = false;
                }
            }
        }
        #endregion

        #region txtDia_chi_GotFocus
        private void txtDia_chi_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!txtDiaChiFocusable)
            {
                SmLib.WinAPISenkey.SenKey(ModifierKeys.None, Key.Tab);
            }
        }
        #endregion

        #region txtNgay_ct_LostFocus
        private void txtNgay_ct_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtNgay_ct.Value == DBNull.Value)
                txtNgay_ct.Value = DateTime.Now;
            if (!txtNgay_ct.IsFocusWithin)
            {
                if (currActionTask == ActionTask.Add || currActionTask == ActionTask.Edit || currActionTask == ActionTask.Copy)
                {
                    if (txtNgay_ct.dValue == new DateTime())
                    {
                    }
                    //else if (!SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, Convert.ToDateTime(txtNgay_ct.dValue)))
                    //{
                    //    ExMessageBox.Show( 520,StartUp.SysObj, "Ngày hạch toán phải sau ngày khóa sổ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                    //    txtNgay_ct.Value = DateTime.Now.Date;
                    //}

                    if (StartUp.M_ngay_lct.Equals("0") && !string.IsNullOrEmpty(txtNgay_ct.Text.ToString()))
                    {
                        txtngay_lct.Value = txtNgay_ct.Value;
                    }
                }
            }
        }
        #endregion

        #region txtngay_lct_GotFocus
        private void txtngay_lct_GotFocus(object sender, RoutedEventArgs e)
        {
            if (StartUp.M_ngay_lct.Equals("0") && !string.IsNullOrEmpty(txtNgay_ct.Text.ToString()))
            {
                txtngay_lct.Value = txtNgay_ct.Value;
            }
            else
            {
                if (string.IsNullOrEmpty(txtngay_lct.Text.ToString()))
                {
                    txtngay_lct.Value = txtNgay_ct.Value;
                }
            }
        }
        #endregion

        #region txtngay_lct_LostFocus
        private void txtngay_lct_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!txtngay_lct.IsFocusWithin)
            {
                if (currActionTask == ActionTask.Copy || currActionTask == ActionTask.Add || currActionTask == ActionTask.Edit)
                {
                    if (!txtNgay_ct.dValue.Date.Equals(txtngay_lct.dValue.Date))
                    {
                        ExMessageBox.Show( 525,StartUp.SysObj, "Ngày lập chứng từ khác với ngày hạch toán!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
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
            DataTable tableFields = null;
            tableFields = SmDataLib.ListFunc.GetSqlTableFieldList(StartUp.SysObj, "v_PH71");
            txtSo_ct.MaxLength = SmDataLib.ListFunc.GetLengthColumn(tableFields, "so_ct");
        }
        #endregion

        #region cbMa_nt_PreviewLostFocus
        private void cbMa_nt_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Voucher_Ma_nt0 == null)
                return;

            Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
            Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
            IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
            if (cbMa_nt.RowResult != null)
            {
                StartUp.DsTrans.Tables[0].DefaultView[0]["loai_tg"] = cbMa_nt.RowResult["loai_tg"];
                if ((cbMa_nt.RowResult)["ma_nt"].ToString().Trim().Equals(StartUp.M_ma_nt0.Trim()))
                {
                    txtTy_gia.Value = 1;
                }
                else
                {
                    txtTy_gia.Value = StartUp.GetRates((cbMa_nt.RowResult)["ma_nt"].ToString().Trim(), Convert.ToDateTime(txtNgay_ct.Value).Date);
                }
            }
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
                txtTy_gia.Value = cbMa_nt.RowResult["ma_nt"].ToString().Trim() == StartUp.M_ma_nt0 ? 1 : txtTy_gia.Value;
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
                    if (txtTy_gia.Value != null && txtTy_gia.Value != DBNull.Value && txtTy_gia.nValue != 0)
                    {
                        if (txtTy_gia.Value != null && txtTy_gia.Value != DBNull.Value && txtTy_gia.nValue != 0)
                        {

                            //decimal ty_gia = 0, tien_nt = 0, gia_nt = 0;
                            //decimal t_tien_nt = 0;
                            //ty_gia = Convert.ToDecimal(txtTy_gia.Value);
                            //t_tien_nt = txtT_Tien_nt.Value == DBNull.Value ? 0 : Convert.ToDecimal(txtT_Tien_nt.Value);

                            if (GrdCt.Records.Count > 0 && (GrdCt.DataSource as DataView).Table.DefaultView[0]["ma_vt"] != DBNull.Value)
                            {
                                decimal ty_gia = 0, tien_nt0 = 0, gia_nt0 = 0, so_luong = 0;
                                decimal t_tien_nt0 = 0;
                                ty_gia = txtTy_gia.nValue;

                                t_tien_nt0 = txtT_Tien_nt.Value == DBNull.Value ? 0 : Convert.ToDecimal(txtT_Tien_nt.Value);
                              //  t_cp_nt = txttong_cp_nt.Value == DBNull.Value ? 0 : Convert.ToDecimal(txttong_cp_nt.Value);



                                for (int i = 0; i < GrdCt.Records.Count; i++)
                                {
                                    //if ((GrdCt.Records[i] as DataRecord).Cells["tien_nt"].Value != DBNull.Value)
                                    //{
                                    //    tien_nt = (GrdCt.DataSource as DataView)[i]["tien_nt"] == DBNull.Value ? 0 : Convert.ToDecimal((GrdCt.Records[i] as DataRecord).Cells["tien_nt"].Value);
                                    //    gia_nt = (GrdCt.DataSource as DataView)[i]["gia_nt"] == DBNull.Value ? 0 : Convert.ToDecimal((GrdCt.Records[i] as DataRecord).Cells["gia_nt"].Value);
                                    //    if (ty_gia * gia_nt != 0)
                                    //    {
                                    //        (GrdCt.DataSource as DataView)[i]["gia"] = SmLib.SysFunc.Round(ty_gia * gia_nt, StartUp.M_ROUND_GIA);
                                    //    }
                                    //    if (ty_gia * tien_nt != 0)
                                    //    {
                                    //        (GrdCt.DataSource as DataView)[i]["tien"] = SmLib.SysFunc.Round(ty_gia * tien_nt, StartUp.M_ROUND);
                                    //    }
                                    //}
                                    //for (int i = 0; i < GrdCt.Records.Count; i++)
                                    //{
                                        if ((GrdCt.Records[i] as DataRecord).Cells["tien_nt"].Value != DBNull.Value)
                                        {
                                            so_luong = (GrdCt.DataSource as DataView)[i]["so_luong"] == DBNull.Value ? 0 : Convert.ToDecimal((GrdCt.Records[i] as DataRecord).Cells["so_luong"].Value);
                                            gia_nt0 = (GrdCt.DataSource as DataView)[i]["gia_nt"] == DBNull.Value ? 0 : Convert.ToDecimal((GrdCt.Records[i] as DataRecord).Cells["gia_nt"].Value);
                                            if (so_luong * gia_nt0 != 0)
                                            {
                                                //tien_nt0 = (GrdCt.DataSource as DataView)[i]["tien_nt0"] == DBNull.Value ? 0 : Convert.ToDecimal((GrdCt.Records[i] as DataRecord).Cells["tien_nt0"].Value);
                                                tien_nt0 = SmLib.SysFunc.Round(so_luong * gia_nt0, StartUp.M_ROUND_NT);
                                                (GrdCt.DataSource as DataView)[i]["tien_nt"] = tien_nt0;
                                            }
                                            if (ty_gia * gia_nt0 != 0)
                                            {
                                                (GrdCt.DataSource as DataView)[i]["gia"] = SmLib.SysFunc.Round(ty_gia * gia_nt0, StartUp.M_ROUND_GIA);
                                            }
                                            if (ty_gia * tien_nt0 != 0)
                                            {
                                                (GrdCt.DataSource as DataView)[i]["tien"] = SmLib.SysFunc.Round(ty_gia * tien_nt0, StartUp.M_ROUND);
                                            }
                                        }
                                   // }

                                }
                                Sum_ALL();
                            }
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

        #region ParseInt
        public int ParseInt(object obj, int defaultvalue)
        {
            int ketqua = defaultvalue;
            int.TryParse(obj != null ? obj.ToString() : defaultvalue.ToString(), out ketqua);
            return ketqua;
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

                        #region "ma_vt"
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
                                    e.Cell.Record.Cells["loai_vt"].Value = txt.RowResult["loai_vt"];
                                    if (string.IsNullOrEmpty(e.Cell.Record.Cells["tk_vt"].Value.ToString()))
                                    {
                                        e.Cell.Record.Cells["tk_vt"].Value = txt.RowResult["tk_vt"];
                                    }
                                    if (string.IsNullOrEmpty(e.Cell.Record.Cells["ma_nx_i"].Value.ToString().Trim()))
                                    {
                                        e.Cell.Record.Cells["ma_nx_i"].Value = txt.RowResult["tk_spdd"];
                                    }

                                    CellValuePresenter cell_Kho = CellValuePresenter.FromCell(e.Cell.Record.Cells["ma_kho_i"]);
                                    AutoCompleteTextBox autoCompleteKho = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(cell_Kho.Editor as ControlHostEditor);
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

                                    DataRowView drVCT = e.Cell.Record.DataItem as DataRowView;
                                    drVCT["sua_tk_vt"] = txt.RowResult["sua_tk_vt"];

                                    e.Cell.Record.Cells["gia_ton"].Value = txt.RowResult["gia_ton"];

                                    if (txt.RowResult["vt_ton_kho"].ToString().Equals("0"))
                                    {
                                        e.Cell.Record.Cells["so_luong"].Value = 0;
                                        StartUp.DsTrans.Tables[0].DefaultView[0]["t_so_luong"] = StartUp.DsTrans.Tables[1].Compute("sum(so_luong)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter);

                                        e.Cell.Record.Cells["gia_nt"].Value = 0;
                                        e.Cell.Record.Cells["gia"].Value = 0;

                                        //CellValuePresenter cell_so_luong = CellValuePresenter.FromCell(e.Cell.Record.Cells["so_luong"]);
                                        //cell_so_luong.Editor.IsReadOnly = true;
                                    }
                                    else
                                    {
                                        //CellValuePresenter cell_so_luong = CellValuePresenter.FromCell(e.Cell.Record.Cells["so_luong"]);
                                        //cell_so_luong.Editor.IsReadOnly = false;
                                    }

                                    if (ParseInt(txt.RowResult["vt_ton_kho"], 0) == 1)
                                    {

                                        if (!string.IsNullOrEmpty(e.Cell.Record.Cells["ma_vt"].Value.ToString()) && !string.IsNullOrEmpty(e.Cell.Record.Cells["ma_kho_i"].Value.ToString()))
                                        {
                                            e.Cell.Record.Cells["ton13"].Value = InvtLib.InFuncLib.GetTon13(StartUp.SysObj, e.Cell.Record.Cells["ma_kho_i"].Value.ToString(), e.Cell.Record.Cells["ma_vt"].Value.ToString(), (e.Cell.Record.DataItem as DataRowView)["ma_vv_i"].ToString());
                                        }
                                    }
                                    else
                                    {
                                        e.Cell.Record.Cells["ton13"].Value = DBNull.Value;
                                    }

                                    ////CellValuePresenter cell_ma_sp = CellValuePresenter.FromCell(e.Cell.Record.Cells["ma_sp"]);
                                    //if (cell_ma_sp != null)
                                    //{
                                    //}
                                    if (txt.RowResult["loai_vt"].ToString().Trim().Equals("51") && txtMa_gd.Text.Trim().Equals("4"))
                                    {
                                        //cell_ma_sp.Value = e.Editor.Value;
                                        
                                        //(e.Cell.Record.DataItem as DataRowView)["ma_sp"] = e.Editor.Value;
                                        //(e.Cell.Record.DataItem as DataRowView).Row.Table.AcceptChanges();

                                        if (e.Cell.Record.Cells.Any(x => x.Field.Name == "ma_sp"))
                                            e.Cell.Record.Cells["ma_sp"].Value = e.Editor.Value;
                                        else
                                            (e.Cell.Record.DataItem as DataRowView)["ma_sp"] = e.Editor.Value;
                                    }
                                    else
                                    {
                                    }

                                }

                                
                                break;
                            }
                        #endregion

                        #region ma_sp
                        case "ma_sp":
                            {
                                if (e.Editor.Value == null)
                                    return;

                                CellValuePresenter cell_ma_vt = CellValuePresenter.FromCell(e.Cell.Record.Cells["ma_vt"]);
                                if (cell_ma_vt != null)
                                {
                                    AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(cell_ma_vt.Editor as ControlHostEditor);
                                    if (txt.RowResult != null)
                                    {
                                        if (txt.RowResult["loai_vt"].ToString().Trim().Equals("51") && txtMa_gd.Text.Trim().Equals("4"))
                                        {
                                            e.Editor.Value = txt.Text.Trim();
                                            e.Editor.IsReadOnly = true;
                                            
                                        }
                                        else
                                        {
                                            e.Editor.IsReadOnly = false;
                                        }
                                    }
                                }



                                break;
                            }
                        #endregion

                        #region ma_kho_i
                        case "ma_kho_i":
                            {
                                if (e.Editor.Value == null)
                                    return;

                                CellValuePresenter cell_VT = CellValuePresenter.FromCell(e.Cell.Record.Cells["ma_vt"]);
                                AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(cell_VT.Editor as ControlHostEditor);
                                if (txt != null)
                                {
                                    txt.SearchInit();
                                    if (txt.RowResult != null)
                                    {
                                        if (txt.RowResult["sua_tk_vt"].ToString().Equals("0"))
                                            e.Cell.Record.Cells["tk_vt"].Value = txt.RowResult["tk_vt"];
                                    }
                                }

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

                                if (ParseInt(txt.RowResult["vt_ton_kho"], 0) == 1)
                                {

                                    if (!string.IsNullOrEmpty(e.Cell.Record.Cells["ma_vt"].Value.ToString()) && !string.IsNullOrEmpty(e.Cell.Record.Cells["ma_kho_i"].Value.ToString()))
                                    {
                                        e.Cell.Record.Cells["ton13"].Value = InvtLib.InFuncLib.GetTon13(StartUp.SysObj, e.Cell.Record.Cells["ma_kho_i"].Value.ToString(), e.Cell.Record.Cells["ma_vt"].Value.ToString(), (e.Cell.Record.DataItem as DataRowView)["ma_vv_i"].ToString());
                                    }
                                }
                                else
                                {
                                    e.Cell.Record.Cells["ton13"].Value = DBNull.Value;
                                }
                                break;
                            }
                        #endregion

                        #region so_luong
                        case "so_luong":
                            {
                                try
                                {
                                    decimal so_luong, gia_nt, gia, tien_nt, tien, ty_gia;
                                    so_luong = (e.Editor as NumericTextBox).nValue;

                                    if (int.Parse(e.Cell.Record.Cells["gia_ton"].Value.ToString()) == 3)
                                        if (so_luong == 0)
                                        {
                                            ExMessageBox.Show( 530,StartUp.SysObj, "Vật tư tính tồn kho theo phương pháp NTXT không được nhập số lượng = 0!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                            return;
                                        }

                                    if (e.Cell.IsDataChanged)
                                    {
                                        decimal.TryParse(e.Cell.Record.Cells["gia_nt"].Value.ToString(), out gia_nt);
                                        decimal.TryParse(e.Cell.Record.Cells["gia"].Value.ToString(), out gia);
                                        decimal.TryParse(e.Cell.Record.Cells["tien_nt"].Value.ToString(), out tien_nt);
                                        decimal.TryParse(e.Cell.Record.Cells["tien"].Value.ToString(), out tien);
                                        ty_gia = txtTy_gia.nValue;

                                        if (so_luong == 0)
                                        {
                                            gia_nt = 0;
                                            e.Cell.Record.Cells["gia_nt"].Value = 0;
                                            gia = 0;
                                            e.Cell.Record.Cells["gia"].Value = 0;
                                        }

                                        if (so_luong * gia_nt != 0)
                                        {
                                            tien_nt = SmLib.SysFunc.Round(so_luong * gia_nt, StartUp.M_ROUND_NT);
                                            e.Cell.Record.Cells["tien_nt"].Value = tien_nt;
                                        }

                                        if (tien_nt * ty_gia != 0)
                                        {
                                            tien = SmLib.SysFunc.Round(tien_nt * ty_gia, StartUp.M_ROUND);
                                            e.Cell.Record.Cells["tien"].Value = tien;
                                        }
                                        else
                                        {
                                            if (so_luong * gia != 0)
                                            {
                                                tien = SmLib.SysFunc.Round(so_luong * gia, StartUp.M_ROUND);
                                                e.Cell.Record.Cells["tien"].Value = tien;
                                            }
                                        }

                                        if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                        {
                                            e.Cell.Record.Cells["tien"].Value = SmLib.SysFunc.Round(tien_nt, StartUp.M_ROUND_NT);
                                            e.Cell.Record.Cells["gia"].Value = SmLib.SysFunc.Round(gia_nt, StartUp.M_ROUND_GIA);
                                        }
                                    }
                                    Sum_ALL();

                                }
                                catch (Exception ex)
                                {
                                    SmErrorLib.ErrorLog.CatchMessage(ex);
                                }
                                break;
                            }
                        #endregion

                        #region gia_nt
                        case "gia_nt":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    decimal so_luong, gia_nt, gia, tien_nt, tien, ty_gia;
                                    gia_nt = (e.Editor as NumericTextBox).nValue;
                                    decimal.TryParse(e.Cell.Record.Cells["so_luong"].Value.ToString(), out so_luong);
                                    decimal.TryParse(e.Cell.Record.Cells["gia"].Value.ToString(), out gia);
                                    decimal.TryParse(e.Cell.Record.Cells["tien_nt"].Value.ToString(), out tien_nt);
                                    decimal.TryParse(e.Cell.Record.Cells["tien"].Value.ToString(), out tien);
                                    ty_gia = txtTy_gia.nValue;

                                    if (so_luong * gia_nt != 0)
                                    {
                                        tien_nt = SmLib.SysFunc.Round(so_luong * gia_nt, StartUp.M_ROUND_NT);
                                        e.Cell.Record.Cells["tien_nt"].Value = tien_nt;
                                    }

                                    if (gia_nt * ty_gia != 0)
                                    {
                                        gia = SmLib.SysFunc.Round(gia_nt * ty_gia, StartUp.M_ROUND_GIA);
                                        e.Cell.Record.Cells["gia"].Value = gia;
                                    }

                                    if (tien_nt * ty_gia != 0)
                                    {
                                        tien = SmLib.SysFunc.Round(tien_nt * ty_gia, StartUp.M_ROUND);
                                        e.Cell.Record.Cells["tien"].Value = tien;
                                    }
                                    else
                                    {
                                        if (so_luong * gia != 0)
                                        {
                                            tien = SmLib.SysFunc.Round(so_luong * gia, StartUp.M_ROUND);
                                            e.Cell.Record.Cells["tien"].Value = tien;
                                        }
                                    }

                                    if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["tien"].Value = SmLib.SysFunc.Round(tien_nt, StartUp.M_ROUND_NT);
                                        e.Cell.Record.Cells["gia"].Value = SmLib.SysFunc.Round(gia_nt, StartUp.M_ROUND_GIA);
                                    }
                                }
                                Sum_ALL();
                                break;
                            }
                        #endregion

                        #region tien_nt
                        case "tien_nt":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    decimal so_luong, gia_nt, gia, tien_nt, tien, ty_gia;
                                    tien_nt = (e.Editor as NumericTextBox).nValue;
                                    decimal.TryParse(e.Cell.Record.Cells["so_luong"].Value.ToString(), out so_luong);
                                    decimal.TryParse(e.Cell.Record.Cells["gia"].Value.ToString(), out gia);
                                    decimal.TryParse(e.Cell.Record.Cells["gia_nt"].Value.ToString(), out gia_nt);
                                    decimal.TryParse(e.Cell.Record.Cells["tien"].Value.ToString(), out tien);
                                    ty_gia = txtTy_gia.nValue;

                                    if (tien_nt * ty_gia != 0)
                                    {
                                        tien = SmLib.SysFunc.Round(tien_nt * ty_gia, StartUp.M_ROUND);
                                        e.Cell.Record.Cells["tien"].Value = tien;
                                    }

                                    if (so_luong != 0)
                                    {
                                        //gia_nt = SmLib.SysFunc.Round(tien_nt / so_luong, StartUp.M_ROUND_GIA_NT);
                                        //e.Cell.Record.Cells["gia_nt"].Value = gia_nt;

                                        //gia = SmLib.SysFunc.Round(tien / so_luong, StartUp.M_ROUND_GIA);
                                        //e.Cell.Record.Cells["gia"].Value = gia;
                                    }

                                    if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["tien"].Value = SmLib.SysFunc.Round(tien_nt, StartUp.M_ROUND_NT);
                                        e.Cell.Record.Cells["gia"].Value = SmLib.SysFunc.Round(gia_nt, StartUp.M_ROUND_GIA);
                                    }
                                }
                                Sum_ALL();
                                break;
                            }
                        #endregion

                        #region gia
                        case "gia":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    decimal so_luong, gia, tien;
                                    gia = (e.Editor as NumericTextBox).nValue;
                                    decimal.TryParse(e.Cell.Record.Cells["so_luong"].Value.ToString(), out so_luong);
                                    decimal.TryParse(e.Cell.Record.Cells["tien"].Value.ToString(), out tien);

                                    if (so_luong * gia != 0)
                                    {
                                        tien = SmLib.SysFunc.Round(so_luong * gia, StartUp.M_ROUND);
                                        e.Cell.Record.Cells["tien"].Value = tien;
                                    }

                                    if (so_luong != 0)
                                    {
                                        //gia = SmLib.SysFunc.Round(tien / so_luong, StartUp.M_ROUND_GIA);
                                        //e.Cell.Record.Cells["gia"].Value = gia;
                                    }
                                }

                                Sum_ALL();
                                break;
                            }
                        #endregion

                        #region tien
                        case "tien":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    decimal so_luong, gia, tien;
                                    tien = (e.Editor as NumericTextBox).nValue;
                                    decimal.TryParse(e.Cell.Record.Cells["so_luong"].Value.ToString(), out so_luong);
                                    decimal.TryParse(e.Cell.Record.Cells["gia"].Value.ToString(), out gia);
                                   
                                    if (so_luong != 0)
                                    {
                                        //gia = SmLib.SysFunc.Round(tien / so_luong, StartUp.M_ROUND_GIA);
                                        //e.Cell.Record.Cells["gia"].Value = gia;
                                    }
                                }

                                Sum_ALL();
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

        #region GrdCt_AddNewRecord
        private bool GrdCt_AddNewRecord(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            NewRowCt();
            return true;
        }
        #endregion

        #region GrdCt_RecordDelete
        private void GrdCt_RecordDelete(object sender, Infragistics.Windows.DataPresenter.Events.RecordsDeletedEventArgs e)
        {
            GrdCt.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);
            //(this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus();
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                (this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus(); ;
            }));
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
                        if (record.Cells["ma_vt"].Value == null || record.Cells["ma_vt"].Value.ToString() == "")
                            return;

                        NewRowCt();
                        GrdCt.ActiveRecord = GrdCt.Records[GrdCt.Records.Count - 1];
                        GrdCt.ActiveCell = (GrdCt.ActiveRecord as DataRecord).Cells["ma_vt"];
                        break;
                    }
                case Key.F5:
                    {
                        if (GrdCt.ActiveRecord != null)
                        {
                            GrdCt.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);

                            CellValuePresenter cellV = CellValuePresenter.FromCell((GrdCt.ActiveRecord as DataRecord).Cells["ma_vt"]);
                            if (cellV != null)
                            {
                                ControlHostEditor controlHost = cellV.Editor as ControlHostEditor;
                                if (controlHost != null)
                                {
                                    AutoCompleteTextBox txt = ControlFunction.GetAutoCompleteControl(controlHost);
                                    if (string.IsNullOrEmpty(txt.Text.Trim()))
                                    {
                                        ExMessageBox.Show( 535,StartUp.SysObj, "Chưa nhập mã vật tư!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                        return;
                                    }
                                    if (txt != null)
                                    {
                                        if (txt.CheckLostFocus())
                                        {
                                            string ma_vt = (GrdCt.ActiveRecord as DataRecord).Cells["ma_vt"].Value.ToString();
                                            string ten_vt = (GrdCt.ActiveRecord as DataRecord).Cells["ten_vt"].Value.ToString();
                                            if(!StartUp.M_LAN.Equals("V"))
                                                ten_vt = (GrdCt.ActiveRecord as DataRecord).Cells["ten_vt2"].Value.ToString();
                                            string ma_kho = (GrdCt.ActiveRecord as DataRecord).Cells["ma_kho_i"].Value.ToString();

                                            DataTable tb = StartUp.GetINCTPND_PX(ma_vt, ma_kho);
                                            if (tb.Rows.Count > 0)
                                            {
                                                FrmINCTPND_PX frm_inctpnd_px = new FrmINCTPND_PX(tb, ten_vt);
                                                frm_inctpnd_px.ShowDialog();

                                                int currRow = 0;
                                                currRow = GrdCt.ActiveRecord.Index;
                                                DataRowView drv_inctpnd_px;
                                                if (currRow >= 0 && currRow <= GrdCt.Records.Count - 1)
                                                {
                                                    drv_inctpnd_px = frm_inctpnd_px.drvFrmINCTPND_PX;
                                                    if (drv_inctpnd_px != null)
                                                    {
                                                        if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                                        {
                                                            (GrdCt.DataSource as DataView)[currRow]["gia_nt"] = drv_inctpnd_px["gia"];
                                                            (GrdCt.DataSource as DataView)[currRow]["gia"] = drv_inctpnd_px["gia"];
                                                        }
                                                        else
                                                        {
                                                            (GrdCt.DataSource as DataView)[currRow]["gia_nt"] = drv_inctpnd_px["gia_nt"];
                                                            (GrdCt.DataSource as DataView)[currRow]["gia"] = drv_inctpnd_px["gia"];
                                                        }
                                                        decimal so_luong = 0, gia_nt = 0, gia = 0, tien_nt = 0, tien = 0, ty_gia = 0;
                                                        gia_nt = ParseDecimal((GrdCt.DataSource as DataView)[currRow]["gia_nt"].ToString(),0);
                                                        gia = ParseDecimal((GrdCt.DataSource as DataView)[currRow]["gia"].ToString(), 0);
                                                        so_luong = ParseDecimal((GrdCt.DataSource as DataView)[currRow]["so_luong"].ToString(), 0);
                                                        ty_gia = txtTy_gia.nValue;

                                                        if (gia_nt * so_luong != 0 && ChkSuaTien.IsChecked == false)
                                                        {
                                                            tien_nt = SmLib.SysFunc.Round(gia_nt * so_luong, StartUp.M_ROUND_NT);
                                                            (GrdCt.DataSource as DataView)[currRow]["tien_nt"] = tien_nt;
                                                        }

                                                        if (tien_nt * ty_gia != 0 && ChkSuaTien.IsChecked == false)
                                                        {
                                                            (GrdCt.DataSource as DataView)[currRow]["tien"] = SmLib.SysFunc.Round(tien_nt * ty_gia, StartUp.M_ROUND);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                ExMessageBox.Show( 540,StartUp.SysObj, "Không có phiếu xuất cho vật tư này!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            ExMessageBox.Show( 545,StartUp.SysObj, "Không có phiếu xuất cho vật tư này!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                            return;
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    }

                case Key.F8:
                    {
                        if (ExMessageBox.Show( 550,StartUp.SysObj, "Có xóa dòng ghi hiện thời không?", "Fast Book 11 .NET", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
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
                                //if (GrdCt.Records.Count == 1)
                                //    GrdCt_AddNewRecord(null, null);
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

                                //StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt"] = StartUp.DsTrans.Tables[1].Compute("sum(tien_nt)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter);
                                //StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien"] = StartUp.DsTrans.Tables[1].Compute("sum(tien)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter);
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
                NewRowCt();
                GrdCt.ActiveRecord = GrdCt.Records[GrdCt.Records.Count - 1];
                //GrdCt.ActiveCell = (GrdCt.Records[GrdCt.Records.Count - 1] as DataRecord).Cells["ma_vt"];
            }
        }
        #endregion

        //#region ChkSuaTien_Click
        //private void ChkSuaTien_Click(object sender, RoutedEventArgs e)
        //{
        //    IsCheckedSua_tien.Value = ChkSuaTien.IsChecked.Value;
        //    if (ChkSuaTien.IsChecked == false && sender.GetType().Name.Equals("CheckBox"))
        //    {
        //        TyGiaValueChange();
        //    }
        //    //if (ChkSuaTien.IsChecked == false && sender.GetType().Name.Equals("CheckBox"))
        //    //{
        //    //    txtTy_gia.Focus();
        //    //}
        //}
        //#endregion

        #region ChkSuaTien_Click
        private void ChkSuaTien_Click(object sender, RoutedEventArgs e)
        {
            IsCheckedSua_tien.Value = ChkSuaTien.IsChecked.Value;
            if (ChkSuaTien.IsChecked == false && sender.GetType().Name.Equals("CheckBox"))
            {
                TyGiaValueChange();
            }
        }
        #endregion

        private void ChkNhapGiaTB_Click(object sender, RoutedEventArgs e)
        {
            IsCheckedPn_gia_tb.Value = ChkNhapGiaTB.IsChecked.Value;
        }

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

        #region txtMa_gd_PreviewLostFocus
        private void txtMa_gd_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtMa_gd.RowResult != null)
            {
                tblTen_gd.Text = StartUp.M_LAN.Equals("V") ? txtMa_gd.RowResult["ten_gd"].ToString() : txtMa_gd.RowResult["ten_gd2"].ToString();
            }
        }
        #endregion

        #region IsVisibilityFieldsXamDataGrid
        void IsVisibilityFieldsXamDataGrid(string ma_nt)
        {
            if (ma_nt == StartUp.M_ma_nt0)
            {
                GrdCt.FieldLayouts[0].Fields["tien"].Visibility = Visibility.Hidden;
                GrdCt.FieldLayouts[0].Fields["gia"].Visibility = Visibility.Hidden;
                GrdCt.FieldLayouts[0].Fields["sua_tk_vt"].Visibility = Visibility.Hidden;

                GrdCt.FieldLayouts[0].Fields["tien"].Settings.CellMaxWidth = 0;
                GrdCt.FieldLayouts[0].Fields["gia"].Settings.CellMaxWidth = 0;
                GrdCt.FieldLayouts[0].Fields["sua_tk_vt"].Settings.CellMaxWidth = 0;

                txtTy_gia.IsReadOnly = true;
            }
            else
            {
                GrdCt.FieldLayouts[0].Fields["tien"].Visibility = Visibility.Visible;
                GrdCt.FieldLayouts[0].Fields["gia"].Visibility = Visibility.Visible;
                GrdCt.FieldLayouts[0].Fields["sua_tk_vt"].Visibility = Visibility.Hidden;

                GrdCt.FieldLayouts[0].Fields["tien"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["tien"].Width.Value.Value;
                GrdCt.FieldLayouts[0].Fields["gia"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["gia"].Width.Value.Value;
                GrdCt.FieldLayouts[0].Fields["sua_tk_vt"].Settings.CellMaxWidth = 0;

                txtTy_gia.IsReadOnly = false;

            }

            LanguageProvider.ChangeLanguage(GrdCt as Visual, LanguageID.Trim() + ".TabInfo.tabItemHT", StartUp.M_LAN, false);
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

        #region Sum_ALL
        void Sum_ALL()
        {
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt"] = StartUp.DsTrans.Tables[1].Compute("sum(tien_nt)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter);
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien"] = StartUp.DsTrans.Tables[1].Compute("sum(tien)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter);
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_so_luong"] = StartUp.DsTrans.Tables[1].Compute("sum(so_luong)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter);
        }
        #endregion

        #region GetLanguageString
        public override string GetLanguageString(string code, string language)
        {
            return StartUp.GetLanguageString(code, language);
        }
        #endregion

        #region CanBangTien
        public void CanBangTien()
        {
            decimal t_tien_nt_InPH = 0, t_tien_InPH = 0, t_tien_nt_InGrdCT = 0, t_tien_InGrdCT = 0, ty_gia = 1;

            StartUp.DsTrans.Tables[0].AcceptChanges();
            StartUp.DsTrans.Tables[1].AcceptChanges();

            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt"].ToString(), out t_tien_nt_InPH);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien"].ToString(), out t_tien_InPH);
            decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien_nt)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), out t_tien_nt_InGrdCT);
            decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), out t_tien_InGrdCT);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

            //Tiền VND trong PH bằng tiền nt trong PH * tỷ giá
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien"] = SysFunc.Round(t_tien_nt_InPH * ty_gia,StartUp.M_ROUND);
            //Lấy tổng tiền VND trong PH trừ tổng tiền VND trong GrdCT, phần còn dư gán vào dòng đầu tiên tổng tiền VND trong GrdCT
            for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
            {
                if (ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["tien_nt"], 0) != 0)
                {
                    StartUp.DsTrans.Tables[1].DefaultView[i]["tien"] = SysFunc.Round(ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["tien"], 0) + (ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien"], 0) - t_tien_InGrdCT), StartUp.M_ROUND);
                    break;
                }
            }

            StartUp.DsTrans.Tables[0].AcceptChanges();
            StartUp.DsTrans.Tables[1].AcceptChanges();

        }
        #endregion

        #region UpdateTonKho
        private void UpdateTonKho()
        {
            string lstma_kho = "";
            string lstma_vt = "";
            string lstma_vv = "";
            for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
            {
                if (ParseInt(StartUp.DsTrans.Tables[1].DefaultView[i]["vt_ton_kho"], 0) == 1)
                {
                    //decimal ton_moi = InvtLib.InFuncLib.GetTon13(StartUp.SysObj, StartUp.DsTrans.Tables[1].DefaultView[i]["ma_kho_i"].ToString(), StartUp.DsTrans.Tables[1].DefaultView[i]["ma_vt"].ToString());
                    //StartUp.DsTrans.Tables[1].DefaultView[i]["ton13"] = ton_moi;
                    lstma_kho += ";" + StartUp.DsTrans.Tables[1].DefaultView[i]["ma_kho_i"].ToString().Trim();
                    lstma_vt += ";" + StartUp.DsTrans.Tables[1].DefaultView[i]["ma_vt"].ToString().Trim();
                    lstma_vv += ";" + StartUp.DsTrans.Tables[1].DefaultView[i]["ma_vv_i"].ToString().Trim();
                }
                else
                    StartUp.DsTrans.Tables[1].DefaultView[i]["ton13"] = DBNull.Value;
            }
            if (!string.IsNullOrEmpty(lstma_kho) && !string.IsNullOrEmpty(lstma_vt))
            {
                lstma_kho = lstma_kho.Substring(1);
                lstma_vt = lstma_vt.Substring(1);
                lstma_vv = lstma_vv.Substring(1);
                DataTable dt = InvtLib.InFuncLib.GetListTon13(StartUp.SysObj, lstma_kho, lstma_vt, lstma_vv);
                if (dt != null)
                    for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
                    {
                        DataRow[] _listdr = dt.Select("ma_kho LIKE '" + StartUp.DsTrans.Tables[1].DefaultView[i]["ma_kho_i"].ToString().Trim() + "' AND ma_vt LIKE '" + StartUp.DsTrans.Tables[1].DefaultView[i]["ma_vt"].ToString().Trim() + "' AND ma_vv LIKE '" + StartUp.DsTrans.Tables[1].DefaultView[i]["ma_vv_i"].ToString().Trim() + "'");
                        if (StartUp.SysObj.GetOption("M_TON_KHO13").ToString().Trim().Equals("1"))
                        {
                            _listdr = dt.Select("ma_kho LIKE '" + StartUp.DsTrans.Tables[1].DefaultView[i]["ma_kho_i"].ToString().Trim() + "' AND ma_vt LIKE '" + StartUp.DsTrans.Tables[1].DefaultView[i]["ma_vt"].ToString().Trim() + "'");
                        }
                        if (_listdr.Length > 0)
                        {
                            StartUp.DsTrans.Tables[1].DefaultView[i]["ton13"] = _listdr[0]["ton13"];
                        }
                        else
                            StartUp.DsTrans.Tables[1].DefaultView[i]["ton13"] = 0;
                    }
            }
            StartUp.DsTrans.Tables[1].AcceptChanges();
        }
        #endregion

    }
}
