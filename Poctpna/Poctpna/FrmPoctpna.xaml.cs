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
using System.Globalization;
using System.Text.RegularExpressions;

namespace Poctpna
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FrmPoctpna : SmVoucherLib.FormTrans
    {
        public static int iRow = 0;
        public static int OldiRow = 0;
        public string Old_ma_kho = string.Empty;

        public static CodeValueBindingObject IsInEditMode;
        CodeValueBindingObject Voucher_Ma_nt0;
        CodeValueBindingObject IsCheckedSua_tien;
        CodeValueBindingObject Ty_Gia_ValueChange;
        CodeValueBindingObject Voucher_Lan0;

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

            if (StartUp.SysObj.VersionInfo.Rows[0]["product_code"].ToString().Equals("FK") || (StartUp.dtRegInfo != null && StartUp.dtRegInfo.Rows[18]["content"].ToString().Trim().Equals("FK")))
            {
                btnChonHDM.Visibility = Visibility.Collapsed;
            }
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
                IsCheckedSua_tien = (CodeValueBindingObject)FormMain.FindResource("IsCheckedSua_tien");
                Ty_Gia_ValueChange = (CodeValueBindingObject)FormMain.FindResource("Ty_Gia_ValueChange");
                Voucher_Lan0 = (CodeValueBindingObject)FormMain.FindResource("Voucher_Lan0");

                //Binding EditMode cho FormTrans
                Binding bind = new Binding("Value");
                bind.Source = IsInEditMode;
                bind.Mode = BindingMode.OneWay;
                this.SetBinding(FormTrans.IsEditModeProperty, bind);

                string M_CDKH13 = SysO.GetOption("M_CDKH13").ToString().Trim();
                if (M_CDKH13 != "1")
                    txtso_du_kh.Visibility = tblso_du_kh.Visibility = Visibility.Hidden;

                //Gán ngôn ngữ messagebox
                M_LAN = StartUp.M_LAN;
                GrdCt.Lan = StartUp.M_LAN;
                GrdCtgt.Lan = StartUp.M_LAN;
                LanguageProvider.Language = StartUp.M_LAN;

                //Them cac truong tu do
                SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, GrdCt, StartUp.Ma_ct, 1);
                SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, GrdCtgt, StartUp.Ma_ct, 2);

                if (StartUp.DsTrans.Tables[0].Rows.Count > 0)
                {
                    LoadData();
                    //Xét lại các Field khi thay đổi record hiển thị
                    IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
                    IsCheckedSua_tien.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"].ToString() == "1");
                }

                Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
                Voucher_Lan0.Value = M_LAN.Equals("V");

                //Lấy số dư khách hàng tức thời
                loaddataDu13();
                UpdateTonKho();
                //Xử lý lưu mã kho của phiếu trước đó
                if (StartUp.DsTrans.Tables[1].DefaultView.Count > 0)
                {
                    Old_ma_kho = StartUp.DsTrans.Tables[1].DefaultView[0]["ma_kho_i"].ToString();
                }

                SetFocusToolbar();
                //this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                //{
                //    StartUp.swMain.Stop();
                //    MessageBox.Show(StartUp.swMain.ElapsedMilliseconds.ToString());
                //}));

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
            StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";

            this.GrdLayout00.DataContext = StartUp.DsTrans.Tables[0].DefaultView;
            this.GrdLayout10.DataContext = StartUp.DsTrans.Tables[0].DefaultView;
            this.GrdLayout20.DataContext = StartUp.DsTrans.Tables[0].DefaultView;
            this.GrdLayout21.DataContext = StartUp.DsTrans.Tables[0].DefaultView;
            //GroupBox Tổng cộng: số lượng
            this.GrdLayout22.DataContext = StartUp.DsTrans.Tables[0].DefaultView;
            //Tổng chi phí trong tab Chi phí
            this.GrdTongChiPhi.DataContext = StartUp.DsTrans.Tables[0].DefaultView;

            //GrdLayoutNT.DataContext = StartUp.DsTrans.Tables[0].DefaultView;
            //Nạp dữ liệu cho Grid hàng hóa, chi phí và hd thuế
            this.GrdCt.DataSource = StartUp.DsTrans.Tables[1].DefaultView;
            this.GrdCp.DataSource = StartUp.DsTrans.Tables[1].DefaultView;
            this.GrdCtgt.DataSource = StartUp.DsTrans.Tables[2].DefaultView;

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
            StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
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
                StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
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
                StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
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
            StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
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
                    NewRecord["loai_pb"] = 1;
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
                    NewRecord["cp_thue_ck"] = 1;
                    NewRecord["ma_gd"] = StartUp.DsTrans.Tables[0].Rows.Count > 1 ? StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"].ToString() : StartUp.DmctInfo["ma_gd"].ToString();
                    if (NewRecord["ma_nt"].ToString().Trim().Equals(StartUp.M_ma_nt0.Trim()))
                    {
                        NewRecord["ty_giaf"] = 1;
                    }
                    else
                    {
                        NewRecord["ty_giaf"] = StartUp.GetRates(NewRecord["ma_nt"].ToString().Trim(), Convert.ToDateTime(NewRecord["ngay_ct"]).Date);
                    }
                    NewRecord["status"] = StartUp.DmctInfo["ma_post"];
                    NewRecord["t_cp_nt"] = 0;
                    NewRecord["t_cp"] = 0;
                    NewRecord["t_tien"] = 0;
                    NewRecord["t_tien_nt"] = 0;
                    NewRecord["t_tien0"] = 0;
                    NewRecord["t_tien_nt0"] = 0;
                    NewRecord["t_thue_nt"] = 0;
                    NewRecord["t_thue"] = 0;
                    NewRecord["t_tt_nt"] = 0;
                    NewRecord["t_tt"] = 0;
                    NewRecord["t_so_luong"] = 0;

                    StartUp.DsTrans.Tables[0].Rows.Add(NewRecord);

                    StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";
                    StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";
                    StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";

                    //Them moi dong trong CT
                    NewRowCt();

                    //Refresh lai form
                    StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";
                    StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";
                    StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";

                    txtngay_lct.Text = "";
                    OldiRow = iRow;
                    iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;
                    IsInEditMode.Value = true;

                    TabInfo.SelectedIndex = 0;
                    ChkSuaTien.IsChecked = false;
                    ChkChiPhiCoThue.IsChecked = true;
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
            if (M_LAN != "V")
                _formcopy.Title = "Copy";

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
                    if (StartUp.M_ngay_lct.Equals("0"))
                    {
                        NewRecord["ngay_lct"] = FrmPoctpnaCopy.ngay_ct;
                    }
                    NewRecord["t_thue_nt"] = 0;
                    NewRecord["t_thue"] = 0;

                    NewRecord["ma_qs"] = GetDMQS(BindingSysObj, StartUp.Ma_ct, Convert.ToDateTime(NewRecord["ngay_ct"]),
                            StartUp.M_User_Id, NewRecord["ma_qs"].ToString().Trim());
                    if (NewRecord["ma_qs"].ToString().Trim() != "")
                        NewRecord["so_ct"] = GetNewSoct(StartUp.SysObj, NewRecord["ma_qs"].ToString());
                    else
                        NewRecord["so_ct"] = "";
                    NewRecord["so_cttmp"] = NewRecord["so_ct"];
                    NewRecord["ma_kh_i"] = "";
                    NewRecord["tk_i"] = "";

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
                    StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";

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
                ExMessageBox.Show(345, StartUp.SysObj, "Không có dữ liệu!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                if (!SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, txtNgay_ct.dValue))
                {
                    ExMessageBox.Show(350, StartUp.SysObj, "Ngày hạch toán phải sau ngày khóa sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
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
                DsVitual.Tables.Add(StartUp.DsTrans.Tables[2].DefaultView.ToTable());
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
                            StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString() + "'";
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

                            //Refresh lại grid hd thuế
                            if (StartUp.DsTrans.Tables[2].Rows.Count > 0)
                            {
                                //lấy các rowfilter trong grid hd thuế
                                DataRow[] _row = StartUp.DsTrans.Tables[2].Select("stt_rec='" + stt_rec + "'");
                                foreach (DataRow dr in _row)
                                {
                                    //delete các row có trong grdctgt
                                    StartUp.DsTrans.Tables[2].Rows.Remove(dr);
                                }
                            }

                            //Refresh lại table[0]
                            StartUp.DsTrans.Tables[0].Rows.RemoveAt(iRow);

                            DataRow rowPh = StartUp.DsTrans.Tables[0].NewRow();
                            rowPh.ItemArray = DsVitual.Tables[0].Rows[0].ItemArray;
                            StartUp.DsTrans.Tables[0].Rows.InsertAt(rowPh, iRow);

                            StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";
                            StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";
                            StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";

                            StartUp.DsTrans.Tables[1].Merge(DsVitual.Tables[1]);
                            StartUp.DsTrans.Tables[2].Merge(DsVitual.Tables[2]);
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
                // Sau đó RowFilter lại Table[0], Table[1], Table[2]
                // Rồi mới xóa Table[0]
                //iRow = iRow > 0 ? iRow - 1 : 0;
                StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString() + "'";
                StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString() + "'";
                StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString() + "'";

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
                //xóa các row trong Table[2]
                if (StartUp.DsTrans.Tables[2].Rows.Count > 0)
                {
                    DataRow[] rows = StartUp.DsTrans.Tables[2].Select("stt_rec='" + _stt_rec + "'");
                    foreach (DataRow dr in rows)
                    {
                        StartUp.DsTrans.Tables[2].Rows.Remove(dr);
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
                if (M_LAN != "V")
                    _FrmTim.Title = "Search";

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
            //string stringBrowse1 = StartUp.CommandInfo["Vbrowse1"].ToString().Split('|')[0];//"ngay_ct:fl:100:h=Ngày c.từ;so_ct:fl:70:h=Số c.từ;ma_kh:100:h=Mã khách;ten_kh:180:h=Tên khách;t_tien_nt0:130:n1:h=Tiền hàng nt;t_cp_nt:130:n1:h=Chi phí nt;t_thue_nt:130:n1:h=Tiền thuế nt;t_tt_nt:130:n1:h=Tổng tiền nt;ma_nx:80:h=Mã nx;tk_thue_no:80:h=Tk thuế;dien_giai:225:h=Diễn giải;t_tien0:130:n0:h=Tiền hàng;t_cp:130:n0:h=Chi phí;t_thue:130:h=Tiền thuế:n0;t_tt:130:h=Tổng tiền:n0;ma_nt:80:h=Mã nt;ty_gia:130:h=Tỷ giá:r;[date]:140:h=Ngày cập nhật;[time]:140:h=Giờ cập nhật;[user_name]:180:h=Tên NSD";
            //string stringBrowse2 = StartUp.CommandInfo["Vbrowse1"].ToString().Split('|')[1];//"ma_vt:fl:100:h=Mã vật tư; ten_vt:fl:270:h=Tên vật tư;dvt:60:h=Đvt;ma_kho_i:70:h=Mã kho;so_luong:q:130:h=Số lượng;gia_nt0:130:p1:h=Giá gốc nt;cp_nt:130:n1:h=Chi phí nt;gia_nt:p1:130:h=Giá nt;tien_nt:130:n1:h=Tiền nt;tk_vt:80:h=Tk vật tư;gia0:130:p0:h=Giá gốc;cp:130:n0:h=Chi phí;gia:130:p0:h=Giá;tien:130:n0:h=Tiền";
            //StartUp.DsTrans.Tables[0].AcceptChanges();
            DataTable PhViewTablev = StartUp.DsTrans.Tables[0].Copy();
            PhViewTablev.Rows.RemoveAt(0);
            SmVoucherLib.FormView _frmView = new SmVoucherLib.FormView(StartUp.SysObj, PhViewTablev.DefaultView, StartUp.DsTrans.Tables[1].DefaultView, StartUp.stringBrowse1, StartUp.stringBrowse2, "stt_rec");
            _frmView.ListFieldSum = "t_tt_nt;t_tt";
            _frmView.frmBrw.Title = SmLib.SysFunc.Cat_Dau(StartUp.CommandInfo["bar"].ToString()).ToString();
            if (M_LAN != "V")
                _frmView.frmBrw.Title = SmLib.SysFunc.Cat_Dau(StartUp.CommandInfo["bar2"].ToString()).ToString();

            SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, _frmView.frmBrw.oBrowseCt, StartUp.Ma_ct, 1);

            _frmView.frmBrw.LanguageID = "PoctpnaXem";
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
                    StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + selected_stt_rec + "'";

                }
            }

        }
        #endregion

        #region V_In
        private void V_In()
        {
            FrmIn oReport = new FrmIn();
            if (StartUp.M_LAN != "V")
                oReport.Title = "Report form list";

            oReport.ShowDialog();

        }
        #endregion

        #region FormMain_EditModeEnded
        //Ham nay dung de xu ly sau khi an mot button 
        private void FormMain_EditModeEnded(object sender, string menuItemName, RoutedEventArgs e)
        {
            //MessageBox.Show(menuItemName.ToString());
            IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());

            if (StartUp.DsTrans.Tables[0].DefaultView.Count > 0)
            {
                Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
                if (!menuItemName.Equals("btnSave"))
                {
                    loaddataDu13();
                    UpdateTonKho();
                }
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
                if (GrdCtgt.Records.Count > 0)
                {
                    var _max_sttrec0ctgt = StartUp.DsTrans.Tables[2].AsEnumerable()
                                   .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                                   .Max(x => x.Field<string>("stt_rec0"));
                    if (_max_sttrec0ctgt != null)
                        int.TryParse(_max_sttrec0ctgt.ToString(), out Stt_rec0ctgt);
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
                NewRecord["gia_nt0"] = 0;
                NewRecord["tien_nt0"] = 0;
                NewRecord["tien0"] = 0;
                NewRecord["cp_nt"] = 0;
                NewRecord["cp"] = 0;
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

        #region NewRowCtGt
        bool NewRowCtGt()
        {
            try
            {
                DataRow NewRecord = StartUp.DsTrans.Tables[2].NewRow();
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
                if (GrdCtgt.Records.Count > 0)
                {
                    var _max_sttrec0ctgt = StartUp.DsTrans.Tables[2].AsEnumerable()
                                   .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                                   .Max(x => x.Field<string>("stt_rec0"));
                    if (_max_sttrec0ctgt != null)
                        int.TryParse(_max_sttrec0ctgt.ToString(), out Stt_rec0ctgt);
                }

                Stt_rec0 = Stt_rec0ct >= Stt_rec0ctgt ? Stt_rec0ct : Stt_rec0ctgt;
                Stt_rec0++;

                NewRecord["stt_rec0"] = string.Format("{0:000}", Stt_rec0);
                NewRecord["ma_ct"] = StartUp.Ma_ct;
                NewRecord["ma_ms"] = StartUp.M_MA_MS;
                NewRecord["ngay_ct"] = txtNgay_ct.Value;
                NewRecord["so_luong"] = 0;
                NewRecord["gia_nt"] = 0;
                NewRecord["gia"] = 0;
                NewRecord["t_tien_nt"] = 0;
                NewRecord["t_tien"] = 0;
                NewRecord["thue_suat"] = 0;
                NewRecord["t_thue_nt"] = 0;
                NewRecord["t_thue"] = 0;
                NewRecord["ma_vv"] = StartUp.DsTrans.Tables[1].DefaultView.Count > 0 ? StartUp.DsTrans.Tables[1].DefaultView[0]["ma_vv_i"].ToString() : "";
                NewRecord["ma_phi"] = StartUp.DsTrans.Tables[1].DefaultView.Count > 0 ? StartUp.DsTrans.Tables[1].DefaultView[0]["ma_phi_i"].ToString() : "";
                FreeCodeFieldLib.CarryFreeCodeFields(StartUp.SysObj, StartUp.Ma_ct, StartUp.DsTrans.Tables[2].DefaultView, NewRecord, 2);
                StartUp.DsTrans.Tables[2].Rows.Add(NewRecord);
                return true;
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
                return false;
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
                        #region ma_vt
                        case "ma_vt":
                            {

                                if (e.Editor.Value == null || !e.Cell.IsDataChanged)
                                    return;
                                AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                if (txt.RowResult != null)
                                {
                                    e.Cell.Record.Cells["ten_vt"].Value = txt.RowResult["ten_vt"];
                                    e.Cell.Record.Cells["ten_vt2"].Value = txt.RowResult["ten_vt2"];

                                    e.Cell.Record.Cells["dvt"].Value = txt.RowResult["dvt"];
                                    e.Cell.Record.Cells["so_khung"].Value = txt.RowResult["so_khung"];
                                    e.Cell.Record.Cells["so_may"].Value = txt.RowResult["so_may"];
                                    e.Cell.Record.Cells["nam_san_xuat"].Value = txt.RowResult["nam_san_xuat"];
                                    e.Cell.Record.Cells["nuoc_san_xuat"].Value = txt.RowResult["nuoc_san_xuat"];
                                    
                                    ////Lấy mã kho dòng trên
                                    //if (e.Cell.Record.Index > 0 && string.IsNullOrEmpty(e.Cell.Record.Cells["ma_kho_i"].Value.ToString().Trim()))
                                    //{
                                    //    e.Cell.Record.Cells["ma_kho_i"].Value = (GrdCt.Records[e.Cell.Record.Index - 1] as DataRecord).Cells["ma_kho_i"].Value;
                                    //}

                                    if (string.IsNullOrEmpty(e.Cell.Record.Cells["tk_vt"].Value.ToString().Trim()) || txt.RowResult["sua_tk_vt"].ToString().Trim().Equals("0"))
                                    {
                                        e.Cell.Record.Cells["tk_vt"].Value = txt.RowResult["tk_vt"];
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

                                    //Update Binding
                                    CellValuePresenter cellV = CellValuePresenter.FromCell(e.Cell.Record.Cells["tk_vt"]);
                                    ControlFunction.RefreshSingleBinding(cellV, AutoCompleteTextBox.IsReadOnlyProperty);


                                    if (txt.RowResult["vt_ton_kho"].ToString().Equals("0"))
                                    {
                                        e.Cell.Record.Cells["so_luong"].Value = 0;
                                        StartUp.DsTrans.Tables[0].DefaultView[0]["t_so_luong"] = ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(so_luong)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), 0);

                                        e.Cell.Record.Cells["gia_nt0"].Value = 0;
                                        e.Cell.Record.Cells["gia0"].Value = 0;
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
                                    if (e.Editor.Value == DBNull.Value)
                                        e.Cell.Record.Cells["so_luong"].Value = 0;

                                    decimal so_luong = 0, gia_nt0 = 0, gia0 = 0;
                                    so_luong = (e.Editor as NumericTextBox).nValue;

                                    if (so_luong == 0)
                                    {
                                        CellValuePresenter cellSoLuong = CellValuePresenter.FromCell(e.Cell.Record.Cells["ma_vt"]);
                                        AutoCompleteTextBox txtMavt = ControlFunction.GetAutoCompleteControl(cellSoLuong.Editor as ControlHostEditor);
                                        if (txtMavt.RowResult == null)
                                        {
                                            txtMavt.SearchInit();
                                        }
                                        else
                                        {
                                            if (txtMavt.RowResult["gia_ton"].ToString().Trim().Equals("3"))
                                            {
                                                ExMessageBox.Show(355, StartUp.SysObj, "Vật tư tính tồn theo phương pháp NTXT không được nhập số lượng = 0!", "", MessageBoxButton.OK);
                                                GrdCt.Dispatcher.BeginInvoke(new Action(() =>
                                                    {
                                                        e.Cell.Record.Cells["so_luong"].IsActive = true;
                                                        GrdCt.ExecuteCommand(DataPresenterCommands.StartEditMode);
                                                    }), DispatcherPriority.Background);
                                            }
                                        }
                                    }

                                    if (e.Cell.IsDataChanged)
                                    {
                                        decimal.TryParse(e.Cell.Record.Cells["gia_nt0"].Value.ToString(), out gia_nt0);
                                        decimal.TryParse(e.Cell.Record.Cells["gia0"].Value.ToString(), out gia0);

                                        if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                        {
                                            if (gia_nt0 * so_luong != 0)
                                            {
                                                e.Cell.Record.Cells["tien_nt0"].Value = SmLib.SysFunc.Round(gia_nt0 * so_luong, StartUp.M_ROUND_NT);
                                                e.Cell.Record.Cells["tien0"].Value = e.Cell.Record.Cells["tien_nt0"].Value;
                                            }
                                        }
                                        else
                                        {
                                            if (gia_nt0 * so_luong != 0)
                                            {
                                                e.Cell.Record.Cells["tien_nt0"].Value = SmLib.SysFunc.Round(gia_nt0 * so_luong, StartUp.M_ROUND_NT);
                                            }

                                            if (gia0 * so_luong != 0)
                                            {
                                                e.Cell.Record.Cells["tien0"].Value = SmLib.SysFunc.Round(gia0 * so_luong, StartUp.M_ROUND);
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

                        #region gia_nt0
                        case "gia_nt0":
                            {
                                if (e.Editor.Value == DBNull.Value)
                                    e.Cell.Record.Cells["gia_nt0"].Value = 0;

                                if (e.Cell.IsDataChanged)
                                {
                                    decimal so_luong = 0, gia_nt0 = 0, tien_nt0 = 0, ty_gia = 0;
                                    decimal.TryParse(e.Cell.Record.Cells["so_luong"].Value.ToString(), out so_luong);
                                    gia_nt0 = (e.Editor as NumericTextBox).nValue;
                                    ty_gia = txtTy_gia.nValue;

                                    if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        if (gia_nt0 * so_luong != 0)
                                        {
                                            tien_nt0 = SmLib.SysFunc.Round(so_luong * gia_nt0, StartUp.M_ROUND_NT);
                                            e.Cell.Record.Cells["tien_nt0"].Value = tien_nt0;
                                            e.Cell.Record.Cells["tien0"].Value = tien_nt0;
                                        }
                                        e.Cell.Record.Cells["gia0"].Value = gia_nt0;
                                    }
                                    else
                                    {
                                        if (gia_nt0 * so_luong != 0)
                                        {
                                            tien_nt0 = SmLib.SysFunc.Round(so_luong * gia_nt0, StartUp.M_ROUND_NT);
                                            e.Cell.Record.Cells["tien_nt0"].Value = tien_nt0;
                                        }

                                        if (gia_nt0 * ty_gia != 0)
                                        {
                                            e.Cell.Record.Cells["gia0"].Value = SmLib.SysFunc.Round(gia_nt0 * ty_gia, StartUp.M_ROUND_GIA);
                                        }

                                        if (tien_nt0 * ty_gia != 0)
                                        {
                                            e.Cell.Record.Cells["tien0"].Value = SmLib.SysFunc.Round(tien_nt0 * ty_gia, StartUp.M_ROUND);
                                        }
                                    }
                                    Sum_ALL();
                                }
                                break;
                            }
                        #endregion

                        #region tien_nt0
                        case "tien_nt0":
                            {
                                if (e.Editor.Value == DBNull.Value)
                                    e.Cell.Record.Cells["tien_nt0"].Value = 0;

                                if (e.Cell.IsDataChanged)
                                {
                                    decimal ty_gia = 0, tien_nt0 = 0;
                                    tien_nt0 = (e.Editor as NumericTextBox).nValue;
                                    ty_gia = txtTy_gia.nValue;

                                    if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["tien0"].Value = e.Cell.Record.Cells["tien_nt0"].Value;
                                    }
                                    else
                                    {
                                        if (tien_nt0 * ty_gia != 0)
                                        {
                                            e.Cell.Record.Cells["tien0"].Value = SmLib.SysFunc.Round(tien_nt0 * ty_gia, StartUp.M_ROUND);
                                        }
                                    }
                                    Sum_ALL();
                                }
                                break;
                            }
                        #endregion

                        #region gia0
                        case "gia0":
                            {
                                if (e.Editor.Value == DBNull.Value)
                                    e.Cell.Record.Cells["gia0"].Value = 0;

                                if (e.Cell.IsDataChanged)
                                {
                                    decimal so_luong = 0, gia0 = 0;
                                    gia0 = (e.Editor as NumericTextBox).nValue;
                                    decimal.TryParse(e.Cell.Record.Cells["so_luong"].Value.ToString(), out so_luong);

                                    if (gia0 * so_luong != 0)
                                    {
                                        e.Cell.Record.Cells["tien0"].Value = SmLib.SysFunc.Round(gia0 * so_luong, StartUp.M_ROUND);
                                    }

                                    Sum_ALL();
                                }
                                break;
                            }
                        #endregion

                        #region tien0
                        case "tien0":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    Sum_ALL();
                                }
                                break;
                            }
                        #endregion

                        #region nam_san_xuat
                        //case "nam_san_xuat":
                        //    {
                        //        // Kiểm tra nếu ô nhập vào là ô "nam_san_xuat"
                        //        if (e.Editor.Value != null && e.Cell.IsDataChanged)
                        //        {
                        //            // Lấy giá trị nhập vào từ ô "nam_san_xuat"
                        //            string nam_san_xuat = e.Editor.Value.ToString();

                        //            // Kiểm tra xem chuỗi nhập vào có phải là số và có độ dài từ 1 đến 4 ký tự không
                        //            Regex regex = new Regex(@"^\d{1,4}$");
                        //            if (!regex.IsMatch(nam_san_xuat))
                        //            {
                        //                // Nếu không đáp ứng yêu cầu, hiển thị thông báo và không cho phép lưu giá trị
                        //                ExMessageBox.Show("Error", "Please enter a numeric value with length between 1 and 4 characters.");
                        //                e.Handled = true;
                        //                return;
                        //            }
                        //        }
                        //        break;
                        //    }
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
            SmLib.WinAPISenkey.SenKey(ModifierKeys.Alt, Key.D3);
            //Cell curCell = GrdCtgt.ActiveCell as Cell;
            //if (curCell == null && GrdCtgt.Records.Count > 0)
            //{
            //    Cell nextCell = (GrdCtgt.Records[0] as DataRecord).Cells[0];
            //    GrdCtgt.ActiveCell = nextCell;
            //}
            //else
            //{
            //    if (GrdCtgt.Records.Count == 0 && IsInEditMode.Value == true)
            //    {
            //        GrdCtgt_AddNewRecord(null, null);
            //    }
            //}
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
                        if (ExMessageBox.Show(360, StartUp.SysObj, "Có xóa dòng ghi hiện thời không?", "Fast Book 11 .NET", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
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

                                StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt0"] = StartUp.DsTrans.Tables[1].Compute("sum(tien_nt0)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter);
                                StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien0"] = StartUp.DsTrans.Tables[1].Compute("sum(tien0)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter);
                                StartUp.DsTrans.Tables[0].DefaultView[0]["t_cp_nt"] = StartUp.DsTrans.Tables[1].Compute("sum(cp_nt)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter);
                                StartUp.DsTrans.Tables[0].DefaultView[0]["t_cp"] = StartUp.DsTrans.Tables[1].Compute("sum(cp)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter);
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

        #region GrdCtgt_AddNewRecord
        private bool GrdCtgt_AddNewRecord(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            return NewRowCtGt();
        }
        #endregion

        #region GrdCtgt_EditModeEnded
        private void GrdCtgt_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            try
            {
                if (IsEditMode && GrdCtgt.ActiveCell != null && StartUp.DsTrans.Tables[2].DefaultView.Count > GrdCtgt.ActiveRecord.Index && StartUp.DsTrans.Tables[2].GetChanges(DataRowState.Deleted) == null)
                    switch (e.Cell.Field.Name)
                    {
                        #region so_ct0
                        case "so_ct0":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Cell.Record.Cells["ma_kh"].Value == DBNull.Value || (e.Cell.Record.Cells["ma_kh"].Value != null && string.IsNullOrEmpty(e.Cell.Record.Cells["ma_kh"].Value.ToString().Trim())))
                                    {
                                        e.Cell.Record.Cells["ma_kh"].Value = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_kh"];
                                        if (txtMa_kh.RowResult == null)
                                        {
                                            txtMa_kh.SearchInit();
                                        }
                                        if (txtMa_kh.RowResult != null)
                                        {
                                            e.Cell.Record.Cells["dia_chi_dmkh"].Value = txtMa_kh.RowResult["dia_chi"];
                                            e.Cell.Record.Cells["ma_so_thue_dmkh"].Value = txtMa_kh.RowResult["ma_so_thue"];
                                        }
                                        e.Cell.Record.Cells["ten_kh"].Value = StartUp.M_LAN.Equals("V") ? StartUp.DsTrans.Tables[0].DefaultView[0]["ten_kh"] : StartUp.DsTrans.Tables[0].DefaultView[0]["ten_kh2"];
                                        e.Cell.Record.Cells["dia_chi"].Value = string.IsNullOrEmpty(txtDia_chi.Text.Trim()) ? StartUp.DsTrans.Tables[0].DefaultView[0]["dia_chi"] : txtDia_chi.Text;
                                        e.Cell.Record.Cells["ma_so_thue"].Value = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_so_thue"];
                                        if (StartUp.DsTrans.Tables[1].DefaultView.Count > 0)
                                        {
                                            //e.Cell.Record.Cells["ten_vt"].Value = StartUp.DsTrans.Tables[1].DefaultView[Lay_Record_Co_TienHangMax()]["ten_vt"].ToString();
                                            //e.Cell.Record.Cells["ma_vv"].Value = StartUp.DsTrans.Tables[1].DefaultView[Lay_Record_Co_TienHangMax()]["ma_vv_i"].ToString();
                                            //e.Cell.Record.Cells["ma_kho"].Value = StartUp.DsTrans.Tables[1].DefaultView[Lay_Record_Co_TienHangMax()]["ma_kho_i"];

                                            e.Cell.Record.Cells["ten_vt"].Value = StartUp.DsTrans.Tables[1].DefaultView[0]["ten_vt"].ToString();
                                            e.Cell.Record.Cells["ma_kho"].Value = StartUp.DsTrans.Tables[1].DefaultView[0]["ma_kho_i"];
                                            StartUp.DsTrans.Tables[2].DefaultView[e.Cell.Record.Index]["ma_vv"] = StartUp.DsTrans.Tables[1].DefaultView[0]["ma_vv_i"].ToString();
                                            StartUp.DsTrans.Tables[2].DefaultView[e.Cell.Record.Index]["ma_phi"] = StartUp.DsTrans.Tables[1].DefaultView[0]["ma_phi_i"].ToString();

                                        }
                                    }
                                    //if (ChkSuaTien.IsChecked == false)
                                    //{
                                    decimal _T_tien_nt_hien_tai = 0, _T_tien_hien_tai = 0;
                                    decimal _t_tt_nt, _t_tt;
                                    decimal.TryParse(StartUp.DsTrans.Tables[2].Compute("sum(t_tien_nt)", "stt_rec= '" + StartUp.DsTrans.Tables[2].DefaultView[0]["stt_rec"].ToString() + "'").ToString(), out _T_tien_nt_hien_tai);
                                    decimal.TryParse(StartUp.DsTrans.Tables[2].Compute("sum(t_tien)", "stt_rec= '" + StartUp.DsTrans.Tables[2].DefaultView[0]["stt_rec"].ToString() + "'").ToString(), out _T_tien_hien_tai);

                                    if (!e.Cell.Record.Cells["t_tien_nt"].Value.ToString().Equals(""))
                                    {
                                        _T_tien_nt_hien_tai = SysFunc.Round(_T_tien_nt_hien_tai - decimal.Parse(e.Cell.Record.Cells["t_tien_nt"].Value.ToString()), StartUp.M_ROUND_NT);
                                        _T_tien_hien_tai = SysFunc.Round(_T_tien_hien_tai - decimal.Parse(e.Cell.Record.Cells["t_tien"].Value.ToString()), StartUp.M_ROUND);
                                    }
                                    _t_tt_nt = (StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt0"] == DBNull.Value ? 0 : (Convert.ToDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt0"].ToString())));
                                    _t_tt = (StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien0"] == DBNull.Value ? 0 : (Convert.ToDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien0"].ToString())));
                                    // nếu có, tính thuế luôn phần chi phí
                                    if (ChkChiPhiCoThue.IsChecked == true)
                                    {
                                        _t_tt_nt = SysFunc.Round(_t_tt_nt + decimal.Parse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_cp_nt"].ToString()), StartUp.M_ROUND_NT);
                                        _t_tt = SysFunc.Round(_t_tt + decimal.Parse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_cp"].ToString()), StartUp.M_ROUND);
                                    }

                                    //_t_tt = _t_tt_nt * ty_gia;
                                    if (ParseDecimal(e.Cell.Record.Cells["t_tien_nt"].Value.ToString(), 0) == 0)
                                    {
                                        e.Cell.Record.Cells["t_tien_nt"].Value = SysFunc.Round(_t_tt_nt - _T_tien_nt_hien_tai, StartUp.M_ROUND_NT);
                                    }
                                    if (ParseDecimal(e.Cell.Record.Cells["t_tien"].Value.ToString(), 0) == 0)
                                    {
                                        e.Cell.Record.Cells["t_tien"].Value = SysFunc.Round(_t_tt - _T_tien_hien_tai, StartUp.M_ROUND);
                                    }
                                    // tính lại thuế
                                    decimal thue_suat;
                                    thue_suat = ParseDecimal(e.Cell.Record.Cells["thue_suat"].Value, 0);

                                    decimal thue_nt = SysFunc.Round((thue_suat * (_t_tt_nt - _T_tien_nt_hien_tai)) / 100, StartUp.M_ROUND_NT);
                                    decimal thue = SmLib.SysFunc.Round((thue_suat * (_t_tt - _T_tien_hien_tai)) / 100, StartUp.M_ROUND);
                                    e.Cell.Record.Cells["t_thue_nt"].Value = thue_nt;
                                    e.Cell.Record.Cells["t_thue"].Value = thue;

                                    decimal tong_thue_nt = 0, tong_thue = 0;
                                    decimal.TryParse(StartUp.DsTrans.Tables[2].Compute("sum(t_thue_nt)", "stt_rec= '" + StartUp.DsTrans.Tables[2].DefaultView[0]["stt_rec"].ToString() + "'").ToString(), out tong_thue_nt);
                                    decimal.TryParse(StartUp.DsTrans.Tables[2].Compute("sum(t_thue)", "stt_rec= '" + StartUp.DsTrans.Tables[2].DefaultView[0]["stt_rec"].ToString() + "'").ToString(), out tong_thue);
                                    StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_nt"] = tong_thue_nt;
                                    StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue"] = tong_thue;

                                    Sum_ALL();
                                    //}
                                }
                                break;
                            }
                        #endregion

                        #region ma_kh
                        case "ma_kh":
                            {
                                if (e.Editor.Value == null)
                                    return;
                                AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                if (txt.RowResult != null)
                                {
                                    if (e.Editor.Value.ToString().Trim() != "")
                                    {
                                        //if (!string.IsNullOrEmpty(txt.RowResult["ten_kh"].ToString()))
                                        e.Cell.Record.Cells["ten_kh"].Value = txt.RowResult["ten_kh"];

                                        if (!string.IsNullOrEmpty(txt.RowResult["dia_chi"].ToString()))
                                            e.Cell.Record.Cells["dia_chi"].Value = txt.RowResult["dia_chi"];
                                        if (!string.IsNullOrEmpty(txt.RowResult["ma_so_thue"].ToString()))
                                            e.Cell.Record.Cells["ma_so_thue"].Value = txt.RowResult["ma_so_thue"];
                                        if (!string.IsNullOrEmpty(txt.RowResult["han_tt"].ToString()))
                                            e.Cell.Record.Cells["han_tt"].Value = txt.RowResult["han_tt"];

                                        e.Cell.Record.Cells["dia_chi_dmkh"].Value = txt.RowResult["dia_chi"];
                                        e.Cell.Record.Cells["ma_so_thue_dmkh"].Value = txt.RowResult["ma_so_thue"];
                                    }
                                }

                                break;
                            }

                        #endregion

                        case "ma_ms":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value.ToString().Trim() != "6")
                                    {
                                        e.Cell.Record.Cells["so_luong"].Value = 0;
                                        e.Cell.Record.Cells["gia_nt"].Value = 0;
                                        e.Cell.Record.Cells["gia"].Value = 0;
                                    }
                                }
                            }
                            break;
                        #region so_luong, gia_nt, gia
                        case "so_luong":
                        case "gia_nt":
                        case "gia":
                            {
                                try
                                {
                                    if (e.Editor.Value == DBNull.Value)
                                        e.Editor.Value = 0;

                                    decimal so_luong = 0, gia_nt0 = 0, gia0 = 0;
                                    if (e.Cell.IsDataChanged)
                                    {
                                        decimal thue_suat = 0;
                                        thue_suat = ParseDecimal(e.Cell.Record.Cells["thue_suat"].Value, 0);

                                        decimal.TryParse(e.Cell.Record.Cells["so_luong"].Value.ToString(), out so_luong);
                                        decimal.TryParse(e.Cell.Record.Cells["gia_nt"].Value.ToString(), out gia_nt0);
                                        decimal.TryParse(e.Cell.Record.Cells["gia"].Value.ToString(), out gia0);

                                        if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                        {
                                            if (gia_nt0 * so_luong != 0)
                                            {
                                                e.Cell.Record.Cells["t_tien_nt"].Value = SmLib.SysFunc.Round(gia_nt0 * so_luong, StartUp.M_ROUND_NT);
                                                e.Cell.Record.Cells["t_tien"].Value = e.Cell.Record.Cells["t_tien_nt"].Value;

                                                e.Cell.Record.Cells["t_thue_nt"].Value = SmLib.SysFunc.Round(ParseDecimal(e.Cell.Record.Cells["t_tien_nt"].Value.ToString(), 0) * thue_suat / 100, StartUp.M_ROUND_NT);
                                                e.Cell.Record.Cells["t_thue"].Value = e.Cell.Record.Cells["t_thue_nt"].Value;
                                                e.Cell.Record.Cells["t_tt_nt"].Value = ParseDecimal(e.Cell.Record.Cells["t_tien_nt"].Value.ToString(), 0) + ParseDecimal(e.Cell.Record.Cells["t_thue_nt"].Value.ToString(), 0);
                                                e.Cell.Record.Cells["t_tt"].Value = e.Cell.Record.Cells["t_tt_nt"].Value;
                                            }
                                        }
                                        else
                                        {
                                            if (gia_nt0 * so_luong != 0)
                                            {
                                                e.Cell.Record.Cells["t_tien_nt"].Value = SmLib.SysFunc.Round(gia_nt0 * so_luong, StartUp.M_ROUND_NT);
                                                e.Cell.Record.Cells["t_thue_nt"].Value = SmLib.SysFunc.Round(ParseDecimal(e.Cell.Record.Cells["t_tien_nt"].Value.ToString(), 0) * thue_suat / 100, StartUp.M_ROUND_NT);
                                                e.Cell.Record.Cells["t_tt_nt"].Value = ParseDecimal(e.Cell.Record.Cells["t_tien_nt"].Value.ToString(), 0) + ParseDecimal(e.Cell.Record.Cells["t_thue_nt"].Value.ToString(), 0);
                                            }

                                            if (gia0 * so_luong != 0)
                                            {
                                                e.Cell.Record.Cells["t_tien"].Value = SmLib.SysFunc.Round(gia0 * so_luong, StartUp.M_ROUND);
                                                e.Cell.Record.Cells["t_thue"].Value = SmLib.SysFunc.Round(ParseDecimal(e.Cell.Record.Cells["t_tien"].Value.ToString(), 0) * thue_suat / 100, StartUp.M_ROUND_NT);
                                                e.Cell.Record.Cells["t_tt"].Value = ParseDecimal(e.Cell.Record.Cells["t_tien"].Value.ToString(), 0) + ParseDecimal(e.Cell.Record.Cells["t_thue"].Value.ToString(), 0);
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

                        #region t_tien_nt
                        case "t_tien_nt":
                            {
                                if (e.Editor.Value == DBNull.Value)
                                    e.Cell.Record.Cells["t_tien_nt"].Value = 0;

                                if (e.Cell.IsDataChanged)
                                {
                                    decimal t_tien_nt = 0, thue_suat = 0;
                                    t_tien_nt = ParseDecimal(e.Cell.Record.Cells["t_tien_nt"].Value, 0);
                                    thue_suat = ParseDecimal(e.Cell.Record.Cells["thue_suat"].Value, 0);


                                    if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["t_thue_nt"].Value = SmLib.SysFunc.Round(t_tien_nt * thue_suat / 100, StartUp.M_ROUND_NT);
                                        e.Cell.Record.Cells["t_thue"].Value = e.Cell.Record.Cells["t_thue_nt"].Value;
                                        e.Cell.Record.Cells["t_tien"].Value = t_tien_nt;
                                        e.Cell.Record.Cells["t_tt_nt"].Value = ParseDecimal(e.Cell.Record.Cells["t_tien_nt"].Value.ToString(), 0) + ParseDecimal(e.Cell.Record.Cells["t_thue_nt"].Value.ToString(), 0);
                                        e.Cell.Record.Cells["t_tt"].Value = e.Cell.Record.Cells["t_tt_nt"].Value;
                                    }
                                    else
                                    {
                                        if (ChkSuaTien.IsChecked == false)
                                        {
                                            e.Cell.Record.Cells["t_thue_nt"].Value = SmLib.SysFunc.Round(t_tien_nt * thue_suat / 100, StartUp.M_ROUND_NT);
                                            e.Cell.Record.Cells["t_tt_nt"].Value = ParseDecimal(e.Cell.Record.Cells["t_tien_nt"].Value.ToString(), 0) + ParseDecimal(e.Cell.Record.Cells["t_thue_nt"].Value.ToString(), 0);
                                        }
                                    }
                                    Sum_ALL();
                                }
                                break;
                            }
                        #endregion

                        #region t_tien
                        case "t_tien":
                            {
                                if (e.Editor.Value == DBNull.Value)
                                    e.Cell.Record.Cells["t_tien"].Value = 0;

                                if (e.Cell.IsDataChanged)
                                {
                                    //Tiền dùng để tính thuế
                                    decimal t_tien = 0, thue_suat = 0;
                                    t_tien = ParseDecimal(e.Cell.Record.Cells["t_tien"].Value, 0);
                                    thue_suat = ParseDecimal(e.Cell.Record.Cells["thue_suat"].Value, 0);

                                    decimal ty_gia = 0;
                                    decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

                                    //Neu nhap = 0 thi gan lai bang tien nguyen te * ty_gia
                                    if (ParseDecimal(e.Cell.Record.Cells["t_tien"].Value, 0) == 0)
                                    {
                                        e.Cell.Record.Cells["t_tien"].Value = SysFunc.Round(ParseDecimal(e.Cell.Record.Cells["t_tien_nt"].Value, 0) * ty_gia, StartUp.M_ROUND);
                                        t_tien = ParseDecimal(e.Cell.Record.Cells["t_tien"].Value, 0);
                                    }

                                    e.Cell.Record.Cells["t_thue"].Value = SmLib.SysFunc.Round(t_tien * thue_suat / 100, StartUp.M_ROUND);
                                    e.Cell.Record.Cells["t_tt"].Value = ParseDecimal(e.Cell.Record.Cells["t_tien"].Value.ToString(), 0) + ParseDecimal(e.Cell.Record.Cells["t_thue"].Value.ToString(), 0);

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
                                    decimal t_tien_nt = 0, t_tien = 0, thue_suat = 0;

                                    if (txt.RowResult != null)
                                    {
                                        e.Cell.Record.Cells["thue_suat"].Value = txt.RowResult["thue_suat"];

                                        t_tien_nt = ParseDecimal(e.Cell.Record.Cells["t_tien_nt"].Value, 0);
                                        t_tien = ParseDecimal(e.Cell.Record.Cells["t_tien"].Value, 0);
                                        thue_suat = ParseDecimal(e.Cell.Record.Cells["thue_suat"].Value, 0);

                                        if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                        {
                                            e.Cell.Record.Cells["t_thue_nt"].Value = SmLib.SysFunc.Round(t_tien_nt * thue_suat / 100, StartUp.M_ROUND_NT);
                                            e.Cell.Record.Cells["t_thue"].Value = e.Cell.Record.Cells["t_thue_nt"].Value;
                                            e.Cell.Record.Cells["t_tt_nt"].Value = SysFunc.Round(t_tien_nt + ParseDecimal(e.Cell.Record.Cells["t_thue_nt"].Value, 0), StartUp.M_ROUND_NT);
                                            e.Cell.Record.Cells["t_tt"].Value = e.Cell.Record.Cells["t_tt_nt"].Value;
                                        }
                                        else
                                        {
                                            e.Cell.Record.Cells["t_thue_nt"].Value = SmLib.SysFunc.Round(t_tien_nt * thue_suat / 100, StartUp.M_ROUND_NT);
                                            e.Cell.Record.Cells["t_thue"].Value = SmLib.SysFunc.Round(t_tien * thue_suat / 100, StartUp.M_ROUND);
                                            e.Cell.Record.Cells["t_tt_nt"].Value = SysFunc.Round(t_tien_nt + ParseDecimal(e.Cell.Record.Cells["t_thue_nt"].Value, 0), StartUp.M_ROUND_NT);
                                            e.Cell.Record.Cells["t_tt"].Value = SysFunc.Round(t_tien + ParseDecimal(e.Cell.Record.Cells["t_thue"].Value, 0), StartUp.M_ROUND);
                                        }

                                        // cập nhật tk_thue_no 
                                        e.Cell.Record.Cells["tk_thue_no"].Value = txt.RowResult["tk_thue_no"];
                                        StartUp.DsTrans.Tables[0].DefaultView[0]["tk_thue_no"] = e.Cell.Record.Cells["tk_thue_no"].Value.ToString();

                                        CellValuePresenter cellTkThueNo = CellValuePresenter.FromCell(e.Cell.Record.Cells["tk_thue_no"]);
                                        AutoCompleteTextBox txtTkThueNo = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(cellTkThueNo.Editor as ControlHostEditor);
                                        if (txtTkThueNo.RowResult == null)
                                            txtTkThueNo.SearchInit();
                                        if (txtTkThueNo.RowResult != null)
                                        {
                                            e.Cell.Record.Cells["tk_cn"].Value = txtTkThueNo.RowResult["tk_cn"];
                                        }
                                        Sum_ALL();
                                    }
                                }
                                break;
                            }
                        #endregion

                        #region t_thue_nt
                        case "t_thue_nt":
                            {
                                if (e.Editor.Value == DBNull.Value)
                                    e.Cell.Record.Cells["t_thue_nt"].Value = 0;

                                if (e.Cell.IsDataChanged)
                                {
                                    decimal ty_gia = 0;
                                    ty_gia = txtTy_gia.nValue;

                                    if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        if (ParseDecimal(e.Cell.Record.Cells["t_thue_nt"].Value, 0) == 0)
                                        {
                                            e.Cell.Record.Cells["t_thue_nt"].Value = SmLib.SysFunc.Round(ParseDecimal(e.Cell.Record.Cells["t_tien_nt"].Value, 0) * ParseDecimal(e.Cell.Record.Cells["thue_suat"].Value, 0) / 100, StartUp.M_ROUND_NT);
                                            e.Cell.Record.Cells["t_thue"].Value = e.Cell.Record.Cells["t_thue_nt"].Value;
                                        }
                                        e.Cell.Record.Cells["t_thue"].Value = e.Cell.Record.Cells["t_thue_nt"].Value;
                                        e.Cell.Record.Cells["t_tt_nt"].Value = SysFunc.Round(ParseDecimal(e.Cell.Record.Cells["t_tien_nt"].Value, 0) + ParseDecimal(e.Cell.Record.Cells["t_thue_nt"].Value, 0), StartUp.M_ROUND_NT);
                                        e.Cell.Record.Cells["t_tt"].Value = e.Cell.Record.Cells["t_tt_nt"].Value;
                                    }
                                    else
                                    {
                                        if (ParseDecimal(e.Cell.Record.Cells["t_thue_nt"].Value, 0) == 0)
                                        {
                                            e.Cell.Record.Cells["t_thue_nt"].Value = SmLib.SysFunc.Round(ParseDecimal(e.Cell.Record.Cells["t_tien_nt"].Value, 0) * ParseDecimal(e.Cell.Record.Cells["thue_suat"].Value, 0) / 100, StartUp.M_ROUND_NT);
                                        }

                                        if (ParseDecimal(e.Cell.Record.Cells["t_thue_nt"].Value, 0) * ty_gia != 0)
                                        {
                                            e.Cell.Record.Cells["t_thue"].Value = SysFunc.Round(ParseDecimal(e.Cell.Record.Cells["t_thue_nt"].Value, 0) * ty_gia, StartUp.M_ROUND);
                                        }

                                        e.Cell.Record.Cells["t_tt_nt"].Value = SysFunc.Round(ParseDecimal(e.Cell.Record.Cells["t_tien_nt"].Value, 0) + ParseDecimal(e.Cell.Record.Cells["t_thue_nt"].Value, 0), StartUp.M_ROUND_NT);
                                        e.Cell.Record.Cells["t_tt"].Value = SysFunc.Round(ParseDecimal(e.Cell.Record.Cells["t_tien"].Value, 0) + ParseDecimal(e.Cell.Record.Cells["t_thue"].Value, 0), StartUp.M_ROUND);
                                    }

                                    Sum_ALL();
                                }
                                break;
                            }
                        #endregion

                        #region t_thue
                        case "t_thue":
                            {
                                if (e.Editor.Value == DBNull.Value)
                                    e.Cell.Record.Cells["t_thue"].Value = 0;

                                if (e.Cell.IsDataChanged)
                                {
                                    if (ParseDecimal(e.Cell.Record.Cells["t_thue"].Value, 0) == 0)
                                    {
                                        e.Cell.Record.Cells["t_thue"].Value = SmLib.SysFunc.Round(ParseDecimal(e.Cell.Record.Cells["t_tien"].Value, 0) * ParseDecimal(e.Cell.Record.Cells["thue_suat"].Value, 0) / 100, StartUp.M_ROUND);
                                    }
                                    e.Cell.Record.Cells["t_tt"].Value = SysFunc.Round(ParseDecimal(e.Cell.Record.Cells["t_tien"].Value, 0) + ParseDecimal(e.Cell.Record.Cells["t_thue"].Value, 0), StartUp.M_ROUND);
                                    Sum_ALL();
                                }
                                break;
                            }
                        #endregion

                        #region tk_thue_no
                        case "tk_thue_no":
                            {
                                //Cập nhật tài khoản thuế
                                if (e.Editor.Value == null)
                                    return;
                                AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                if (txt.RowResult != null)
                                {
                                    e.Cell.Record.Cells["tk_cn"].Value = txt.RowResult["tk_cn"];
                                    if (e.Cell.Record.Cells["tk_cn"].Value.ToString().Trim().Equals("0"))
                                    {
                                        e.Cell.Record.Cells["ma_kh2"].Value = "";
                                    }
                                }
                                break;
                            }

                        case "ma_kh2":
                            {
                                if (e.Editor.Value == null)
                                    return;
                                break;
                            }
                        #endregion

                        default:
                            break;
                    }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        #endregion

        #region GrdCtgt_RecordDelete
        private void GrdCtgt_RecordDelete(object sender, Infragistics.Windows.DataPresenter.Events.RecordsDeletedEventArgs e)
        {
            GrdCtgt.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);
            txt_thck.IsFocus = true;


        }
        #endregion

        #region GrdCtgt_KeyUp
        private void GrdCtgt_KeyUp(object sender, KeyEventArgs e)
        {
            if (IsInEditMode.Value == false)
                return;

            switch (e.Key)
            {
                case Key.F4:

                    DataRecord recordF4 = (GrdCtgt.ActiveRecord as DataRecord);
                    if (recordF4 == null || recordF4.Cells["so_ct0"].Value == null || recordF4.Cells["so_ct0"].Value.ToString() == "")
                        return;

                    NewRowCtGt();
                    GrdCtgt.ActiveRecord = GrdCtgt.Records[GrdCtgt.Records.Count - 1];
                    GrdCtgt.ActiveCell = (GrdCtgt.ActiveRecord as DataRecord).Cells["ma_ms"];
                    break;
                case Key.F8:
                    {
                        if (ExMessageBox.Show(365, StartUp.SysObj, "Có xóa dòng ghi hiện thời không?", "Fast Book 11 .NET", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                        {
                            return;
                        }

                        DataRecord record = (GrdCtgt.ActiveRecord as DataRecord);
                        if (record != null)
                        {
                            int indexRecord = 0, indexCell = 0;
                            Cell cell = GrdCtgt.ActiveCell;
                            if (record.Index == 0)
                            {
                                if (GrdCtgt.Records.Count == 1)
                                    GrdCtgt_AddNewRecord(null, null);
                            }
                            else if (record.Index == GrdCtgt.Records.Count - 1)
                            {
                                indexRecord = record.Index - 1;
                            }
                            indexCell = GrdCtgt.ActiveCell == null ? 0 : GrdCtgt.ActiveCell.Field.Index;
                            GrdCtgt.ExecuteCommand(DataPresenterCommands.EndEditModeAndDiscardChanges);
                            if (indexCell >= 0)
                            {
                                StartUp.DsTrans.Tables[2].Rows.Remove(StartUp.DsTrans.Tables[2].DefaultView[record.Index].Row);
                                StartUp.DsTrans.Tables[2].AcceptChanges();
                                if (GrdCtgt.Records.Count > 0)
                                {
                                    GrdCtgt.ActiveRecord = GrdCtgt.Records[indexRecord > GrdCtgt.Records.Count - 1 ? GrdCtgt.Records.Count - 1 : indexRecord];
                                }
                                StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_nt"] = StartUp.DsTrans.Tables[2].Compute("sum(t_thue_nt)", StartUp.DsTrans.Tables[2].DefaultView.RowFilter);
                                StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue"] = StartUp.DsTrans.Tables[2].Compute("sum(t_thue)", StartUp.DsTrans.Tables[2].DefaultView.RowFilter);
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

        #region GrdCtgt_KeyDown
        private void GrdCtgt_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsInEditMode.Value == false)
                return;
            if (Keyboard.IsKeyDown(Key.N) && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                NewRowCtGt();
                GrdCtgt.ActiveRecord = GrdCtgt.Records[GrdCtgt.Records.Count - 1];
            }
            if (Keyboard.IsKeyDown(Key.Tab) && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                GrdCtgt.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);
                (this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus();
                e.Handled = true;
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

        #region ChkChiPhiCoThue_Click
        private void ChkChiPhiCoThue_Click(object sender, RoutedEventArgs e)
        {
            if (currActionTask.Equals(ActionTask.Add) || currActionTask.Equals(ActionTask.Edit) || currActionTask.Equals(ActionTask.Copy))
            {
                if ((sender as CheckBox).IsChecked == true)
                {
                    StartUp.DsTrans.Tables[0].DefaultView[0]["cp_thue_ck"] = 1;
                }
                else
                {
                    StartUp.DsTrans.Tables[0].DefaultView[0]["cp_thue_ck"] = 0;
                }
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
                StartUp.DsTrans.Tables[2].AcceptChanges();
                Sum_ALL();

                bool isError = false;
                if (!IsSequenceSave)
                {
                    object o_t_tien_hang_CtGt_nt;
                    object o_t_tien_hang_Ph_nt;
                    object o_t_tien_hang_CtGt;
                    object o_t_tien_hang_Ph;
                    if (ChkChiPhiCoThue.IsChecked == true)
                    {
                        o_t_tien_hang_CtGt_nt = ParseDecimal(StartUp.DsTrans.Tables[2].Compute("sum(t_tien_nt)", "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'"), 0);
                        o_t_tien_hang_Ph_nt = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt0"], 0) + ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_cp_nt"], 0);
                        o_t_tien_hang_CtGt = ParseDecimal(StartUp.DsTrans.Tables[2].Compute("sum(t_tien)", "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'"), 0);
                        o_t_tien_hang_Ph = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien0"], 0) + ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_cp"], 0);
                    }
                    else
                    {
                        o_t_tien_hang_CtGt_nt = StartUp.DsTrans.Tables[2].Compute("sum(t_tien_nt)", "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'");
                        o_t_tien_hang_Ph_nt = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt0"], 0);
                        o_t_tien_hang_CtGt = StartUp.DsTrans.Tables[2].Compute("sum(t_tien)", "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'");
                        o_t_tien_hang_Ph = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien0"], 0);
                    }
                    //tổng chi phí của các vật tư
                    decimal _tong_cp_nt_vt = 0, _t_cp_nt = 0, _tong_cp_vt = 0, _t_cp = 0;
                    decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(cp_nt)", "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'").ToString(), out _tong_cp_nt_vt);
                    decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(cp)", "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'").ToString(), out _tong_cp_vt);
                    //tổng chi phí 
                    decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_cp_nt"].ToString(), out _t_cp_nt);
                    decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_cp"].ToString(), out _t_cp);
                    decimal _ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);


                    GrdCt.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);
                    GrdCtgt.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);

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
                        ExMessageBox.Show(370, StartUp.SysObj, "Chưa vào mã khách hàng!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtMa_kh.IsFocus = true;
                        isError = true;
                    }
                    else if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nx"].ToString()))
                    {
                        ExMessageBox.Show(375, StartUp.SysObj, "Chưa vào tài khoản có!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtMa_nx.IsFocus = true;
                        isError = true;
                    }
                    else if (string.IsNullOrEmpty(txtNgay_ct.Text.ToString()))
                    {
                        ExMessageBox.Show(380, StartUp.SysObj, "Chưa vào ngày hạch toán!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtNgay_ct.Focus();
                        isError = true;
                    }

                    else if (StartUp.M_NGAY_BAT_DAU != null && (!txtNgay_ct.IsValueValid || txtNgay_ct.dValue < StartUp.M_NGAY_BAT_DAU || txtNgay_ct.dValue > StartUp.M_NGAY_KET_THUC))
                    {
                        ExMessageBox.Show(1024, StartUp.SysObj, "Ngày hạch toán không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        isError = true;
                        txtNgay_ct.Focus();
                    }
                    else if (StartUp.DsTrans.Tables[1].DefaultView.Count == 0 || string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[0]["ma_vt"].ToString()))
                    {
                        ExMessageBox.Show(385, StartUp.SysObj, "Chưa vào chi tiết vật tư, không lưu được!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                        TabInfo.SelectedIndex = 0;
                        GrdCt.ExecuteCommand(DataPresenterCommands.CellFirstOverall);
                        GrdCt.Focus();
                        isError = true;

                    }
                    //else if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[0]["tk_vt"].ToString()))
                    //{
                    //    ExMessageBox.Show( 390,StartUp.SysObj, "Chưa vào tài khoản nợ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                    //    TabInfo.SelectedIndex = 0;
                    //    GrdCt.ExecuteCommand(DataPresenterCommands.CellFirstOverall);
                    //    GrdCt.Focus();
                    //    isError = true;
                    //}
                    else if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"].ToString()))
                    {
                        ExMessageBox.Show(395, StartUp.SysObj, "Chưa vào số chứng từ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtSo_ct.Focus();
                        isError = true;
                    }
                    //else if (CheckValidSoct(StartUp.SysObj, txtMa_qs.Text, txtSo_ct.Text, StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()))
                    //{
                    //    if (StartUp.M_trung_so.Equals("1"))
                    //    {
                    //        if (ExMessageBox.Show( 400,StartUp.SysObj, "Có chứng từ trùng số. Số cuối cùng là " + "[" + GetLastSoct(StartUp.SysObj, txtMa_qs.Text).Trim() + "]" + ". Có lưu chứng từ này không?", "Fast Book 11 .NET", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                    //        {
                    //            txtSo_ct.SelectAll();
                    //            txtSo_ct.Focus();
                    //            isError = true;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        ExMessageBox.Show( 405,StartUp.SysObj, "Số chứng từ đã tồn tại!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                    //        txtSo_ct.SelectAll();
                    //        txtSo_ct.Focus();
                    //        isError = true;
                    //    }
                    //}

                    //so sánh tổng chi phí nt của các vật tư với tổng chi phí nt
                    else if ((_tong_cp_nt_vt != _t_cp_nt) || (_tong_cp_vt != _t_cp))
                    {
                        ExMessageBox.Show(410, StartUp.SysObj, "Tổng chi phí khác với chi phí tổng cộng của các vật tư!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                        SmLib.WinAPISenkey.SenKey(ModifierKeys.Alt, Key.D2);
                        GrdCp.ActiveCell = (GrdCp.Records[0] as DataRecord).Cells["cp_nt"];
                        GrdCp.Focus();
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                txttong_cp_nt.Focus();
                            }));
                        isError = true;
                    }
                    if (!isError)
                    {
                        if (StartUp.DsTrans.Tables[1].DefaultView.Count > 0)
                        {
                            for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
                            {
                                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["ma_vt"].ToString().Trim()))
                                {
                                    ExMessageBox.Show(415, StartUp.SysObj, "Chưa vào chi tiết vật tư, không lưu được!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                    GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["ma_vt"];
                                    GrdCt.Focus();
                                    return;
                                }

                                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["ma_kho_i"].ToString().Trim()))
                                {
                                    ExMessageBox.Show(420, StartUp.SysObj, "Chưa vào mã kho, không lưu được!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                    GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["ma_kho_i"];
                                    GrdCt.Focus();
                                    return;
                                }

                                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_vt"].ToString().Trim()))
                                {
                                    ExMessageBox.Show(425, StartUp.SysObj, "Chưa vào tài khoản vật tư, không lưu được!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                    GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_vt"];
                                    GrdCt.Focus();
                                    return;
                                }

                                //if (StartUp.IsTkMe(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_vt"].ToString().Trim()))
                                //{
                                //    ExMessageBox.Show( 430,StartUp.SysObj, "Tk nợ là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                //    GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_vt"];
                                //    GrdCt.Focus();
                                //    return;
                                //}
                            }

                        }

                        //if (StartUp.DsTrans.Tables[1].DefaultView.Count > 0)
                        //{
                        //    foreach (DataRowView drv in StartUp.DsTrans.Tables[1].DefaultView)
                        //    {
                        //        if (string.IsNullOrEmpty(drv.Row["ma_vt"].ToString().Trim()))
                        //        {
                        //            StartUp.DsTrans.Tables[1].Rows.Remove(drv.Row);
                        //            StartUp.DsTrans.Tables[1].AcceptChanges();
                        //            continue;
                        //        }
                        //    }
                        //}

                        if (StartUp.DsTrans.Tables[2].DefaultView.Count > 0)
                        {
                            foreach (DataRowView drv in StartUp.DsTrans.Tables[2].DefaultView)
                            {
                                if (string.IsNullOrEmpty(drv.Row["ma_ms"].ToString().Trim()) || string.IsNullOrEmpty(drv.Row["so_ct0"].ToString().Trim()))
                                {
                                    StartUp.DsTrans.Tables[2].Rows.Remove(drv.Row);
                                    StartUp.DsTrans.Tables[2].AcceptChanges();
                                    continue;
                                }
                            }
                        }

                        if (StartUp.DsTrans.Tables[2].DefaultView.Count > 0)
                        {
                            //Kiem tra ma so thue
                            bool showMessage = false;
                            int i = 0;

                            //Kiem tra trung hoa don 
                            bool showMessageCheckHD = false;
                            string so_ct0 = "", so_seri0 = "", ma_so_thue = "";
                            string ngay_ct0;

                            foreach (DataRowView drv in StartUp.DsTrans.Tables[2].DefaultView)
                            {
                                if (!StartUp.M_MST_CHECK.Equals("0"))
                                {
                                    if (!SmLib.SysFunc.CheckSumMaSoThue(drv.Row["ma_so_thue"].ToString().Trim()) && !string.IsNullOrEmpty(drv.Row["ma_so_thue"].ToString().Trim()) && !showMessage)
                                    {
                                        ExMessageBox.Show(435, StartUp.SysObj, "Mã số thuế không hợp lệ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                        showMessage = true;
                                        if (StartUp.M_MST_CHECK.Equals("2"))
                                        {
                                            //Cảnh báo và không cho lưu
                                            return;
                                        }
                                    }
                                }

                                if (StartUp.M_CHK_HD_VAO != 0)
                                {
                                    so_ct0 = drv.Row["so_ct0"].ToString().Trim();
                                    so_seri0 = drv.Row["so_seri0"].ToString().Trim();
                                    ngay_ct0 = string.IsNullOrEmpty(drv.Row["ngay_ct0"].ToString().Trim()) ? "" : Convert.ToDateTime(drv.Row["ngay_ct0"].ToString().Trim()).Date.ToShortDateString().Substring(0, 10);
                                    ma_so_thue = drv.Row["ma_so_thue"].ToString().Trim();

                                    if (StartUp.CheckExistHDVao(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString(), so_ct0, so_seri0, ngay_ct0, ma_so_thue) && !showMessageCheckHD && (!string.IsNullOrEmpty(ngay_ct0)))
                                    {
                                        ExMessageBox.Show(440, StartUp.SysObj, string.Format("Hoá đơn số [{0}], ký hiệu [{1}], ngày [{2}], MST [{3}] đã tồn tại!", so_ct0, so_seri0, ngay_ct0, ma_so_thue), "", MessageBoxButton.OK, MessageBoxImage.Information);
                                        showMessageCheckHD = true;
                                        if (StartUp.M_CHK_HD_VAO == 2)
                                        {
                                            //Cảnh báo và không cho lưu
                                            return;
                                        }
                                    }
                                }

                                if (drv["tk_cn"].ToString().Trim().Equals("1") && string.IsNullOrEmpty(drv["ma_kh2"].ToString().Trim()))
                                {
                                    ExMessageBox.Show(445, StartUp.SysObj, "Chưa vào cục thuế!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                    GrdCtgt.ActiveCell = (GrdCtgt.Records[i] as DataRecord).Cells["ma_kh2"];
                                    GrdCtgt.Focus();
                                    return;
                                }


                                i++;
                            }
                            if (!CheckVoucherOutofDate())
                            {
                                isError = true;
                                return;
                            }
                        }

                        if (GrdCtgt.Records.Count > 0)
                        {
                            decimal t_tien_hang_CtGt_nt = Convert.ToDecimal(o_t_tien_hang_CtGt_nt.Equals(DBNull.Value) ? 0 : o_t_tien_hang_CtGt_nt);
                            decimal t_tien_hang_Ph_nt = Convert.ToDecimal(o_t_tien_hang_Ph_nt.Equals(DBNull.Value) ? 0 : o_t_tien_hang_Ph_nt);
                            decimal t_tien_hang_CtGt = Convert.ToDecimal(o_t_tien_hang_CtGt.Equals(DBNull.Value) ? 0 : o_t_tien_hang_CtGt);
                            decimal t_tien_hang_Ph = Convert.ToDecimal(o_t_tien_hang_Ph.Equals(DBNull.Value) ? 0 : o_t_tien_hang_Ph);

                            if (cbMa_nt.Text == StartUp.M_ma_nt0)
                            {
                                t_tien_hang_CtGt_nt = SysFunc.Round(t_tien_hang_CtGt_nt, StartUp.M_ROUND);
                                t_tien_hang_CtGt = t_tien_hang_CtGt_nt;
                                t_tien_hang_Ph_nt = SysFunc.Round(t_tien_hang_Ph_nt, StartUp.M_ROUND);
                                t_tien_hang_Ph = t_tien_hang_Ph_nt;
                            }
                            else
                            {
                                t_tien_hang_CtGt_nt = SysFunc.Round(t_tien_hang_CtGt_nt, StartUp.M_ROUND_NT);
                                t_tien_hang_CtGt = SysFunc.Round(t_tien_hang_CtGt, StartUp.M_ROUND);
                                t_tien_hang_Ph_nt = SysFunc.Round(t_tien_hang_Ph_nt, StartUp.M_ROUND_NT);
                                t_tien_hang_Ph = SysFunc.Round(t_tien_hang_Ph, StartUp.M_ROUND);
                            }

                            if ((t_tien_hang_Ph_nt != t_tien_hang_CtGt_nt) || (t_tien_hang_Ph != t_tien_hang_CtGt))
                                ExMessageBox.Show(460, StartUp.SysObj, "Tổng tiền/ tiền ngoại tệ khác với tổng tiền/ tiền ngoại tệ trong các hóa đơn giá trị gia tăng!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                if (!isError)
                {
                    if (!IsSequenceSave)
                    {
                        if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"].ToString().Trim()))
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"] = StartUp.DmctInfo["ma_gd"];

                        // update thông tin cho các record Table1 (Ct) 
                        for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
                        {
                            StartUp.DsTrans.Tables[1].DefaultView[i]["ngay_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ngay_ct"];
                            StartUp.DsTrans.Tables[1].DefaultView[i]["so_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"];
                            StartUp.DsTrans.Tables[1].DefaultView[i]["ma_ct"] = StartUp.Ma_ct;
                            // tinh tien va tien_nt
                            decimal tien = 0, tien0 = 0, tien_nt = 0, tien_nt0 = 0;
                            tien_nt0 = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["tien_nt0"], 0);
                            tien_nt = SmLib.SysFunc.Round(tien_nt0 + ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["cp_nt"], 0), StartUp.M_ROUND_NT);
                            tien0 = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["tien0"], 0);
                            tien = SmLib.SysFunc.Round(tien0 + ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["cp"], 0), StartUp.M_ROUND);
                            StartUp.DsTrans.Tables[1].DefaultView[i]["tien"] = tien;
                            StartUp.DsTrans.Tables[1].DefaultView[i]["tien_nt"] = tien_nt;
                            StartUp.DsTrans.Tables[1].DefaultView[i]["tien0"] = tien0;
                            StartUp.DsTrans.Tables[1].DefaultView[i]["tien_nt0"] = tien_nt0;
                            // tinh gia va gia_nt
                            decimal gia = 0, gia_nt = 0, so_luong = 0;
                            decimal gia0 = 0, gia_nt0 = 0;
                            so_luong = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["so_luong"], 0);

                            if (so_luong > 0)
                            {
                                gia_nt = SmLib.SysFunc.Round(tien_nt / so_luong, StartUp.M_ROUND_GIA_NT);
                                gia = SmLib.SysFunc.Round(tien / so_luong, StartUp.M_ROUND_GIA);
                            }
                            else
                            {
                                gia_nt = gia = gia_nt0 = gia0 = 0;
                            }
                            StartUp.DsTrans.Tables[1].DefaultView[i]["gia"] = gia;
                            StartUp.DsTrans.Tables[1].DefaultView[i]["gia_nt"] = gia_nt;
                        }

                        // update thông tin cho các record Table2 (Ctgt) 
                        for (int i = 0; i < StartUp.DsTrans.Tables[2].DefaultView.Count; i++)
                        {
                            StartUp.DsTrans.Tables[2].DefaultView[i]["tk_du"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nx"];
                            StartUp.DsTrans.Tables[2].DefaultView[i]["ma_nt"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"];
                            StartUp.DsTrans.Tables[2].DefaultView[i]["ty_gia"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"];
                            StartUp.DsTrans.Tables[2].DefaultView[i]["ty_giaf"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ty_giaf"];
                            StartUp.DsTrans.Tables[2].DefaultView[i]["status"] = StartUp.DsTrans.Tables[0].DefaultView[0]["status"];
                            StartUp.DsTrans.Tables[2].DefaultView[i]["ma_gd"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"];
                        }
                        // update so_ct0 , ngay_ct0,so_seri0 cho Table0 (Ph) , lấy thông tin từ record có tiền thuế lớn nhất trong tab HĐ Thuế
                        if (StartUp.DsTrans.Tables[2].DefaultView.Count > 0)
                        {
                            int index_max_thue = Lay_Index_Record_Co_TienThueMax();
                            if (index_max_thue != -1)
                            {
                                StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct0"] = StartUp.DsTrans.Tables[2].DefaultView[index_max_thue]["so_ct0"];
                                StartUp.DsTrans.Tables[0].DefaultView[0]["ngay_ct0"] = StartUp.DsTrans.Tables[2].DefaultView[index_max_thue]["ngay_ct0"];
                                StartUp.DsTrans.Tables[0].DefaultView[0]["so_seri0"] = StartUp.DsTrans.Tables[2].DefaultView[index_max_thue]["so_seri0"];
                            }

                            for (int i = 0; i < StartUp.DsTrans.Tables[2].DefaultView.Count; i++)
                                if (StartUp.DsTrans.Tables[2].DefaultView[i]["ngay_ct0"] == StartUp.DsTrans.Tables[0].DefaultView[0]["ngay_ct0"] &&
                                    StartUp.DsTrans.Tables[2].DefaultView[i]["so_ct0"] == StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct0"])
                                {
                                    StartUp.DsTrans.Tables[0].DefaultView[0]["so_seri0"] = StartUp.DsTrans.Tables[2].DefaultView[0]["so_seri0"];
                                    break;
                                }
                        }
                        //update han_tt
                        if (txtMa_kh.RowResult != null)
                        {
                            if (StartUp.DsTrans.Tables[0].DefaultView[0]["han_tt"] == DBNull.Value || string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["han_tt"].ToString()))
                                StartUp.DsTrans.Tables[0].DefaultView[0]["han_tt"] = txtMa_kh.RowResult["han_tt"].ToString().Trim();
                        }
                        // update ty_giaf = ty_gia
                        decimal _ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);


                        //Cân bằng tiền
                        if (ChkSuaTien.IsChecked == false && _ty_gia != 0)
                        {
                            CanBangTien();
                        }
                        //Lưu tiền thuế trong tab hạch toán để lên bảng kê phiếu nhâp(mẫu nhập mua)
                        PhanBoThueInCT();
                        //Tính t_tien_nt và t_tien (PH)
                        StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt"] = SmLib.SysFunc.Round(ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt0"], 0) + ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_cp_nt"], 0), StartUp.M_ROUND_NT);
                        StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien"] = SmLib.SysFunc.Round(ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt"], 0) * ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0), StartUp.M_ROUND);

                        // kết thúc update

                        StartUp.DsTrans.Tables[0].AcceptChanges();
                        StartUp.DsTrans.Tables[1].AcceptChanges();
                        StartUp.DsTrans.Tables[2].AcceptChanges();
                    }
                    DataTable tbPhToSave = StartUp.DsTrans.Tables[0].Clone();
                    StartUp.DsTrans.Tables[0].DefaultView[0]["loai_ct"] = StartUp.DmctInfo["ct_nxt"];
                    tbPhToSave.Rows.Add(StartUp.DsTrans.Tables[0].DefaultView[0].Row.ItemArray);
                    if (!IsSequenceSave)
                    {
                        tbPhToSave.Rows[0]["status"] = 0;
                    }
                    DataProvider.UpdateDataTable(StartUp.SysObj, StartUp.DmctInfo["m_phdbf"].ToString(), "stt_rec", tbPhToSave, "stt_rec;row_id");

                    //DataProvider.DeleteRow(StartUp.SysObj, StartUp.DmctInfo["m_ctdbf"].ToString(), "stt_rec='" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"] + "'");
                    //DataProvider.DeleteRow(StartUp.SysObj, StartUp.DmctInfo["m_ctgtdbf"].ToString(), "stt_rec='" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"] + "'");

                    DataTable tbCtToSave = StartUp.DsTrans.Tables[1].Clone();
                    DataTable tbCtGtToSave = StartUp.DsTrans.Tables[2].Clone();

                    foreach (DataRowView drv in StartUp.DsTrans.Tables[1].DefaultView)
                    {
                        //drv.Row["ma_px_i"] = "123456789123456789";
                        if (!IsSequenceSave)
                        {
                            drv.Row["so_ct"] = txtSo_ct.Text;
                        }
                        tbCtToSave.Rows.Add(drv.Row.ItemArray);
                    }

                    foreach (DataRowView drv in StartUp.DsTrans.Tables[2].DefaultView)
                    {
                        if (!IsSequenceSave)
                        {
                            drv.Row["so_ct"] = txtSo_ct.Text;
                        }
                        tbCtGtToSave.Rows.Add(drv.Row.ItemArray);
                    }
                    if (DataProvider.UpdateCtTable(StartUp.SysObj, StartUp.DmctInfo["m_ctdbf"].ToString(), tbCtToSave, StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()) == false)
                    {
                        ExMessageBox.Show(465, StartUp.SysObj, "Lưu không thành công, kiểm tra lại dữ liệu!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    if (DataProvider.UpdateCtTable(StartUp.SysObj, StartUp.DmctInfo["m_ctgtdbf"].ToString(), tbCtGtToSave, StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()) == false)
                    {
                        ExMessageBox.Show(470, StartUp.SysObj, "Lưu không thành công, kiểm tra lại dữ liệu!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
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
                                                    if (ExMessageBox.Show(475, StartUp.SysObj, "Có chứng từ trùng số. Số cuối cùng là: " + "[" + GetLastSoct(StartUp.SysObj, txtMa_qs.Text).Trim() + "]" + ". Có lưu chứng từ này không?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                                                    {
                                                        txtSo_ct.SelectAll();
                                                        txtSo_ct.Focus();
                                                        isError = true;
                                                    }
                                                }
                                                else if (StartUp.M_trung_so.Equals("2"))
                                                {
                                                    ExMessageBox.Show(480, StartUp.SysObj, "Số chứng từ đã tồn tại!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    txtSo_ct.SelectAll();
                                                    txtSo_ct.Focus();
                                                    isError = true;
                                                }
                                                break;
                                            }
                                        case "PH02":
                                            {
                                                ExMessageBox.Show(485, StartUp.SysObj, "Tk có là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                isError = true;
                                                txtMa_nx.IsFocus = true;
                                                break;
                                            }
                                        case "CT01":
                                            {
                                                int index = Convert.ToInt16(dv[1]);
                                                ExMessageBox.Show(490, StartUp.SysObj, "Tk vật tư là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                isError = true;
                                                tiHT.Focus();
                                                GrdCt.ActiveCell = (GrdCt.Records[index] as DataRecord).Cells["tk_vt"];
                                                GrdCt.Focus();
                                                break;
                                            }
                                        case "GT01":
                                            {
                                                int index = Convert.ToInt16(dv[1]);
                                                ExMessageBox.Show(495, StartUp.SysObj, "Tk thuế là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                isError = true;
                                                tiThue.Focus();
                                                GrdCtgt.ActiveCell = (GrdCtgt.Records[index] as DataRecord).Cells["tk_thue_no"];
                                                GrdCtgt.Focus();
                                                break;
                                            }
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
                        ThreadStart start = delegate ()
                        {
                            Post();

                            //Update lại tồn kho tức thời
                            if (!IsSequenceSave)
                            {
                                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(()
                                =>
                                {
                                    if (StartUp.DsTrans.Tables[1].DefaultView[0]["stt_rec"].ToString().Equals(_stt_rec1))
                                    {
                                        UpdateTonKho();
                                        loaddataDu13();
                                    }
                                }));
                            }
                        };
                        new Thread(start).Start();
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
                //if (txtMa_kh_i.Text.Equals(""))
                //{
                //    txtMa_kh_i.Text = txtMa_kh.Text;
                //}
                StartUp.DsTrans.Tables[0].DefaultView[0]["ten_kh"] = txtMa_kh.RowResult["ten_kh"].ToString().Trim();
                StartUp.DsTrans.Tables[0].DefaultView[0]["ten_kh2"] = txtMa_kh.RowResult["ten_kh2"].ToString().Trim();

                StartUp.DsTrans.Tables[0].DefaultView[0]["ma_so_thue"] = txtMa_kh.RowResult["ma_so_thue"].ToString().Trim();
                if (!string.IsNullOrEmpty(txtMa_kh.RowResult["doi_tac"].ToString().Trim()))
                {
                    StartUp.DsTrans.Tables[0].DefaultView[0]["ong_ba"] = txtMa_kh.RowResult["doi_tac"].ToString().Trim();
                }
                StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nx"] = string.IsNullOrEmpty(txtMa_nx.Text.Trim()) ? txtMa_kh.RowResult["tk"].ToString().Trim() : txtMa_nx.Text.Trim();
                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_thck"].ToString().Trim()))
                {
                    StartUp.DsTrans.Tables[0].DefaultView[0]["ma_thck"] = txtMa_kh.RowResult["ma_thck"];
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

                loaddataDu13();
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

        #region txtMa_nx_PreviewLostFocus
        private void txtMa_nx_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtMa_nx.RowResult != null)
            {
                StartUp.DsTrans.Tables[0].DefaultView[0]["ten_tk"] = txtMa_nx.RowResult["ten_nx"].ToString();
                StartUp.DsTrans.Tables[0].DefaultView[0]["ten_tk2"] = txtMa_nx.RowResult["ten_nx2"].ToString();

                //if (txttk_i.Text.Equals(""))
                //{
                //    txttk_i.Text = txtMa_nx.Text;
                //}
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
                        decimal ty_gia = 0, tien_nt0 = 0, tien0 = 0, gia_nt0 = 0, so_luong = 0;
                        decimal t_tien_nt0 = 0, t_cp_nt = 0, thue_nt = 0, thue = 0;
                        ty_gia = txtTy_gia.nValue;

                        t_tien_nt0 = txtT_Tien_nt.Value == DBNull.Value ? 0 : Convert.ToDecimal(txtT_Tien_nt.Value);
                        t_cp_nt = txttong_cp_nt.Value == DBNull.Value ? 0 : Convert.ToDecimal(txttong_cp_nt.Value);


                        if (GrdCt.Records.Count > 0 && (GrdCt.DataSource as DataView).Table.DefaultView[0]["ma_vt"] != DBNull.Value)
                        {
                            for (int i = 0; i < GrdCt.Records.Count; i++)
                            {
                                if ((GrdCt.Records[i] as DataRecord).Cells["tien_nt0"].Value != DBNull.Value)
                                {
                                    so_luong = (GrdCt.DataSource as DataView)[i]["so_luong"] == DBNull.Value ? 0 : Convert.ToDecimal((GrdCt.Records[i] as DataRecord).Cells["so_luong"].Value);
                                    gia_nt0 = (GrdCt.DataSource as DataView)[i]["gia_nt0"] == DBNull.Value ? 0 : Convert.ToDecimal((GrdCt.Records[i] as DataRecord).Cells["gia_nt0"].Value);
                                    if (so_luong * gia_nt0 != 0)
                                    {
                                        //tien_nt0 = (GrdCt.DataSource as DataView)[i]["tien_nt0"] == DBNull.Value ? 0 : Convert.ToDecimal((GrdCt.Records[i] as DataRecord).Cells["tien_nt0"].Value);
                                        tien_nt0 = SmLib.SysFunc.Round(so_luong * gia_nt0, StartUp.M_ROUND_NT);
                                        (GrdCt.DataSource as DataView)[i]["tien_nt0"] = tien_nt0;
                                    }
                                    if (ty_gia * gia_nt0 != 0)
                                    {
                                        (GrdCt.DataSource as DataView)[i]["gia0"] = SmLib.SysFunc.Round(ty_gia * gia_nt0, StartUp.M_ROUND_GIA);
                                    }
                                    if (ty_gia * tien_nt0 != 0)
                                    {
                                        (GrdCt.DataSource as DataView)[i]["tien0"] = SmLib.SysFunc.Round(ty_gia * tien_nt0, StartUp.M_ROUND);
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
                            t_cp_nt = txttong_cp_nt.Value == DBNull.Value ? 0 : Convert.ToDecimal(txttong_cp_nt.Value.ToString());
                            if (GrdCp.Records.Count > 0)
                            {
                                StartUp.DsTrans.Tables[0].DefaultView[0]["t_cp"] = SmLib.SysFunc.Round(t_cp_nt * ty_gia, StartUp.M_ROUND);
                                // Phân bổ lại chi phí  
                                if (!string.IsNullOrEmpty(txtloai_pb.Text.Trim().ToString()))
                                {
                                    PhanBo();
                                }
                            }

                            //----------------------------------------HĐ thuế------------------------------------

                            if (GrdCtgt.Records.Count > 0)
                            {
                                for (int i = 0; i < GrdCtgt.Records.Count; i++)
                                {
                                    if ((GrdCtgt.Records[i] as DataRecord).Cells["t_tien_nt"].Value != DBNull.Value)
                                    {
                                        tien_nt0 = Convert.ToDecimal((GrdCtgt.Records[i] as DataRecord).Cells["t_tien_nt"].Value);
                                        (GrdCtgt.DataSource as DataView)[i]["t_tien"] = SmLib.SysFunc.Round(ty_gia * tien_nt0, StartUp.M_ROUND);
                                    }
                                    if ((GrdCtgt.Records[i] as DataRecord).Cells["t_thue_nt"].Value != DBNull.Value)
                                    {
                                        thue_nt = Convert.ToDecimal((GrdCtgt.Records[i] as DataRecord).Cells["t_thue_nt"].Value);
                                        (GrdCtgt.DataSource as DataView)[i]["t_thue"] = SmLib.SysFunc.Round(ty_gia * thue_nt, StartUp.M_ROUND);
                                    }

                                    if ((GrdCtgt.Records[i] as DataRecord).Cells["t_tien"].Value != DBNull.Value && (GrdCtgt.Records[i] as DataRecord).Cells["t_thue"].Value != DBNull.Value)
                                    {
                                        tien0 = Convert.ToDecimal((GrdCtgt.Records[i] as DataRecord).Cells["t_tien"].Value);
                                        thue = Convert.ToDecimal((GrdCtgt.Records[i] as DataRecord).Cells["t_thue"].Value);
                                        (GrdCtgt.DataSource as DataView)[i]["t_tt"] = SmLib.SysFunc.Round(tien0 + thue, StartUp.M_ROUND);
                                    }
                                }
                                /*
                                _sum_thue_nt = Convert.ToDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_nt"].ToString());
                                _sum_thue = SmLib.SysFunc.Round(_ty_gia * _sum_thue_nt, StartUp.M_ROUND);
                                StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue"] = _sum_thue;
                                //Gán số dư cho phiếu đầu tiên
                                if (StartUp.DsTrans.Tables[2].DefaultView.Count > 0)
                                {
                                    object _sum_thue_Obj = StartUp.DsTrans.Tables[2].Compute("sum(t_thue)", "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'");
                                    decimal _sum_thue_ctgt = Convert.ToDecimal(_sum_thue_Obj.Equals(DBNull.Value) ? 0 : _sum_thue_Obj);
                                    StartUp.DsTrans.Tables[2].Rows[0]["t_thue"] = Convert.ToDecimal(StartUp.DsTrans.Tables[2].Rows[0]["t_thue"]) + (_sum_thue - _sum_thue_ctgt);
                                }*/
                            }
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
                    //    ExMessageBox.Show( 500,StartUp.SysObj, "Ngày hạch toán phải sau ngày khóa sổ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
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
                        ExMessageBox.Show(505, StartUp.SysObj, "Ngày lập chứng từ khác với ngày hạch toán!", "", MessageBoxButton.OK, MessageBoxImage.Information);
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
            //txtSo_ct.Text = txtSo_ct.Text.Trim().ToString();
            DataTable tableFields = null;
            tableFields = SmDataLib.ListFunc.GetSqlTableFieldList(StartUp.SysObj, "v_PH71");
            txtSo_ct.MaxLength = SmDataLib.ListFunc.GetLengthColumn(tableFields, "so_ct");
        }
        #endregion

        #region Sum_ALL
        void Sum_ALL()
        {
            decimal t_cp = 0, t_cp_nt = 0, t_thue = 0, t_thue_nt = 0, t_tien0 = 0, t_tien_nt0 = 0;

            StartUp.DsTrans.Tables[0].AcceptChanges();
            StartUp.DsTrans.Tables[1].AcceptChanges();
            StartUp.DsTrans.Tables[2].AcceptChanges();

            if (cbMa_nt.Text.Equals(StartUp.M_ma_nt0))
            {
                //t_tien0 = SysFunc.Round(ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(tien0)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), 0), StartUp.M_ROUND);
                t_tien_nt0 = SysFunc.Round(ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(tien_nt0)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), 0), StartUp.M_ROUND);
                t_tien0 = t_tien_nt0;

                //t_cp = SysFunc.Round(ParseDecimal(txttong_cp.nValue.ToString(), 0), StartUp.M_ROUND);
                t_cp_nt = SysFunc.Round(ParseDecimal(txttong_cp_nt.nValue.ToString(), 0), StartUp.M_ROUND);
                t_cp = t_cp_nt;

                //t_thue = SysFunc.Round(ParseDecimal(StartUp.DsTrans.Tables[2].Compute("sum(t_thue)", StartUp.DsTrans.Tables[2].DefaultView.RowFilter).ToString(), 0), StartUp.M_ROUND);
                t_thue_nt = SysFunc.Round(ParseDecimal(StartUp.DsTrans.Tables[2].Compute("sum(t_thue_nt)", StartUp.DsTrans.Tables[2].DefaultView.RowFilter).ToString(), 0), StartUp.M_ROUND);
                t_thue = t_thue_nt;
            }
            else
            {
                t_tien0 = SysFunc.Round(ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(tien0)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), 0), StartUp.M_ROUND);
                t_tien_nt0 = SysFunc.Round(ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(tien_nt0)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), 0), StartUp.M_ROUND_NT);

                t_cp = SysFunc.Round(ParseDecimal(txttong_cp.nValue.ToString(), 0), StartUp.M_ROUND);
                t_cp_nt = SysFunc.Round(ParseDecimal(txttong_cp_nt.nValue.ToString(), 0), StartUp.M_ROUND_NT);

                t_thue = SysFunc.Round(ParseDecimal(StartUp.DsTrans.Tables[2].Compute("sum(t_thue)", StartUp.DsTrans.Tables[2].DefaultView.RowFilter).ToString(), 0), StartUp.M_ROUND);
                t_thue_nt = SysFunc.Round(ParseDecimal(StartUp.DsTrans.Tables[2].Compute("sum(t_thue_nt)", StartUp.DsTrans.Tables[2].DefaultView.RowFilter).ToString(), 0), StartUp.M_ROUND_NT);
            }

            StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien0"] = t_tien0;
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt0"] = t_tien_nt0;
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_cp"] = t_cp;
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_cp_nt"] = t_cp_nt;
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue"] = t_thue;
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_nt"] = t_thue_nt;
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_tt"] = t_tien0 + t_cp + t_thue;
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_tt_nt"] = t_tien_nt0 + t_cp_nt + t_thue_nt;

            StartUp.DsTrans.Tables[0].DefaultView[0]["t_so_luong"] = ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(so_luong)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), 0);
        }
        #endregion

        #region IsVisibilityFieldsXamDataGrid
        void IsVisibilityFieldsXamDataGrid(string ma_nt)
        {
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

            int _cp_co_thue = 0;
            int.TryParse(StartUp.DsTrans.Tables[0].Rows[iRow]["cp_thue_ck"].ToString(), out _cp_co_thue);
            switch (_cp_co_thue)
            {
                case 0:
                    {
                        this.ChkChiPhiCoThue.IsChecked = false;
                        break;
                    }
                case 1:
                    {
                        this.ChkChiPhiCoThue.IsChecked = true;
                        break;
                    }
            }

            IsVisibilityFieldsXamDataGridBySua_Tien();
            ChangeLanguage();
        }
        #endregion

        #region IsVisibilityFieldsXamDataGridBySua_Tien
        void IsVisibilityFieldsXamDataGridBySua_Tien()
        {
            int sua_tien = 0;
            int.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"].ToString(), out sua_tien);
            switch (sua_tien)
            {
                #region sua_tien=1
                case 1:
                    {
                        //nếu check sửa trường tiền
                        //và đang ở chế độ chỉnh sửa
                        //thì cho sửa tổng cp hoạch toán
                        if (IsInEditMode.Value == true)
                        {
                            txttong_cp.IsReadOnly = false;
                            txttong_cp.IsTabStop = true;
                        }
                        else
                        {
                            txttong_cp.IsReadOnly = true;
                            txttong_cp.IsTabStop = false;
                        }
                    }
                    break;
                #endregion

                #region sua_tien=0
                case 0:
                    {
                        //nếu không check sửa trường tiền
                        //và tổng cp nt bằng 0
                        //thì cho sửa tổng cp hoạch toán
                        decimal _t_cp_nt = 0;
                        decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_cp_nt"].ToString(), out _t_cp_nt);
                        if (_t_cp_nt == 0 && IsInEditMode.Value == true)
                        {
                            txttong_cp.IsReadOnly = false;
                            txttong_cp.IsTabStop = true;
                        }
                        else
                        {
                            //nếu không check sửa trường tiền
                            //thì không cho sửa tổng cp hoạch toán
                            txttong_cp.IsReadOnly = true;
                            txttong_cp.IsTabStop = false;
                        }
                    }
                    break;
                    #endregion
            }
            IsCheckedSua_tien.Value = ChkSuaTien.IsChecked.Value;
        }
        #endregion

        #region PhanBoThueInCT
        void PhanBoThueInCT()
        {
            if (StartUp.DsTrans.Tables[1].DefaultView.Count == 0)
                return;

            decimal tong_thue_nt = 0, tong_thue = 0;
            decimal tong_tien_nt0 = 0, tong_tien0 = 0;
            decimal tong_cp_nt = 0, tong_cp = 0;
            decimal cp_nt = 0, cp = 0;
            decimal tien_nt0 = 0, tien0 = 0;
            decimal thue_nt = 0, thue = 0;
            decimal thue_nt_temp = 0, thue_temp = 0;
            decimal ty_gia = 0;
            string stt_rec = StartUp.DsTrans.Tables[1].DefaultView[0]["stt_rec"].ToString();
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);
            //Chỉ lấy thuế của tiền hàng
            if ((txtMa_kh_i.Text.Equals("") || !txtMa_kh_i.Text.Equals(txtMa_kh.Text)) &&
                (txttk_i.Text.Equals("") || !txttk_i.Text.Equals(txtMa_nx.Text)))
            {
                var _t_thue_nt = StartUp.DsTrans.Tables[2].AsEnumerable()
                                 .Where(b => b.Field<string>("stt_rec") == stt_rec && b.Field<string>("ma_kh").Trim() != txtMa_kh_i.Text.Trim())
                                 .Sum(x => x.Field<decimal>("t_thue_nt"));
                tong_thue_nt = _t_thue_nt;
                var _t_thue = StartUp.DsTrans.Tables[2].AsEnumerable()
                                 .Where(b => b.Field<string>("stt_rec") == stt_rec && b.Field<string>("ma_kh").Trim() != txtMa_kh_i.Text.Trim())
                                 .Sum(x => x.Field<decimal>("t_thue"));
                tong_thue = _t_thue;
            }
            else
            {
                decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_nt"].ToString(), out tong_thue_nt);
                decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue"].ToString(), out tong_thue);
            }

            decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien_nt0)", "stt_rec= '" + stt_rec + "'").ToString(), out tong_tien_nt0);
            decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien0)", "stt_rec= '" + stt_rec + "'").ToString(), out tong_tien0);
            decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(cp_nt)", "stt_rec= '" + stt_rec + "'").ToString(), out tong_cp_nt);
            decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(cp)", "stt_rec= '" + stt_rec + "'").ToString(), out tong_cp);

            for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
            {
                decimal.TryParse(StartUp.DsTrans.Tables[1].DefaultView[i]["tien_nt0"].ToString(), out tien_nt0);
                decimal.TryParse(StartUp.DsTrans.Tables[1].DefaultView[i]["tien0"].ToString(), out tien0);

                decimal.TryParse(StartUp.DsTrans.Tables[1].DefaultView[i]["cp_nt"].ToString(), out cp_nt);
                decimal.TryParse(StartUp.DsTrans.Tables[1].DefaultView[i]["cp"].ToString(), out cp);
                //nếu loại tiền là ngoại tệ
                if (cbMa_nt.Text != StartUp.M_ma_nt0)
                {
                    //nếu tiền nguyên tệ = 0
                    if (tien_nt0 == 0)
                    {
                        thue_nt = 0;
                    }
                    else
                    {
                        thue_nt = tong_tien_nt0 == 0 ? 0 : SmLib.SysFunc.Round(((tien_nt0 + cp_nt) / (tong_tien_nt0 + tong_cp_nt)) * tong_thue_nt, StartUp.M_ROUND_NT);
                    }

                    //nếu tiền ngoại tệ = 0
                    if (tien0 == 0)
                    {
                        thue = 0;
                    }
                    else
                    {
                        thue = tong_tien0 == 0 ? 0 : SmLib.SysFunc.Round(((tien0 + cp) / (tong_tien0 + tong_cp)) * tong_thue, StartUp.M_ROUND);
                    }
                }
                else
                {
                    thue_nt = (tong_tien_nt0 == 0 ? 0 : SmLib.SysFunc.Round(((tien_nt0 + cp_nt) / (tong_tien_nt0 + tong_cp_nt)) * tong_thue_nt, StartUp.M_ROUND_NT));
                    thue = tong_tien0 == 0 ? 0 : SmLib.SysFunc.Round(((tien0 + cp) / (tong_tien0 + tong_cp)) * tong_thue, StartUp.M_ROUND);
                }

                StartUp.DsTrans.Tables[1].DefaultView[i]["thue_nt"] = thue_nt;
                StartUp.DsTrans.Tables[1].DefaultView[i]["thue"] = thue;
                thue_nt_temp += thue_nt;
                thue_temp += thue;
            }
            StartUp.DsTrans.Tables[1].DefaultView[0]["thue_nt"] = decimal.Parse(StartUp.DsTrans.Tables[1].DefaultView[0]["thue_nt"].ToString()) + (tong_thue_nt - thue_nt_temp);
            StartUp.DsTrans.Tables[1].DefaultView[0]["thue"] = decimal.Parse(StartUp.DsTrans.Tables[1].DefaultView[0]["thue"].ToString()) + (tong_thue - thue_temp);

            StartUp.DsTrans.Tables[0].AcceptChanges();
            StartUp.DsTrans.Tables[1].AcceptChanges();
            StartUp.DsTrans.Tables[2].AcceptChanges();
        }
        #endregion

        #region PhanBo
        //Phan bo chi phi
        void PhanBo()
        {
            if (StartUp.DsTrans.Tables[1].DefaultView.Count == 0)
                return;
            decimal tong_cp = 0, tong_cp_nt = 0;
            decimal tong_sl = 0;
            decimal tong_tien = 0, tong_tien_nt = 0;
            decimal cp_temp = 0, cp_nt_temp = 0;
            decimal cp = 0, cp_nt = 0;
            decimal ty_gia = 0;
            string stt_rec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_cp"].ToString(), out tong_cp);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_cp_nt"].ToString(), out tong_cp_nt);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);
            int loai_pb = 0;
            int.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["loai_pb"].ToString(), out loai_pb);

            switch (loai_pb)
            {
                case 1:// phan bo theo tien
                    {
                        //tổng tiền, tiền nt 
                        decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien0)", "stt_rec= '" + stt_rec + "'").ToString(), out tong_tien);
                        decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien_nt0)", "stt_rec= '" + stt_rec + "'").ToString(), out tong_tien_nt);
                        decimal tien = 0;
                        decimal tien_nt = 0;

                        for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
                        {
                            decimal.TryParse(StartUp.DsTrans.Tables[1].DefaultView[i]["tien0"].ToString(), out tien);
                            decimal.TryParse(StartUp.DsTrans.Tables[1].DefaultView[i]["tien_nt0"].ToString(), out tien_nt);
                            //nếu loại tiền là ngoại tệ
                            if (cbMa_nt.Text != StartUp.M_ma_nt0)
                            {
                                //tiền ngoại tệ = 0
                                //thì tính cp_nt theo tiền VND
                                if (tien_nt == 0)
                                {
                                    cp_nt = tong_tien == 0 ? 0 : SmLib.SysFunc.Round((tien / tong_tien) * tong_cp_nt, StartUp.M_ROUND_NT);
                                }
                                else
                                    cp_nt = tong_tien_nt == 0 ? 0 : SmLib.SysFunc.Round((tien_nt / tong_tien_nt) * tong_cp_nt, StartUp.M_ROUND_NT);
                            }
                            else
                            {
                                cp_nt = tong_tien_nt == 0 ? 0 : SmLib.SysFunc.Round((tien_nt / tong_tien_nt) * tong_cp_nt, StartUp.M_ROUND_NT);
                            }
                            //chi phí = cp nt nhân với tỷ giá
                            if (cp_nt != 0)
                            {
                                cp = SmLib.SysFunc.Round(cp_nt * ty_gia, StartUp.M_ROUND);
                            }
                            else
                            {
                                if (tong_tien_nt != 0)
                                {
                                    cp = SmLib.SysFunc.Round((tien_nt / tong_tien_nt) * tong_cp, StartUp.M_ROUND);
                                }
                                else if (tong_tien != 0)
                                {
                                    cp = SmLib.SysFunc.Round((tien / tong_tien) * tong_cp, StartUp.M_ROUND);
                                }
                                else
                                {
                                    cp = 0;
                                }

                            }
                            StartUp.DsTrans.Tables[1].DefaultView[i]["cp"] = cp;
                            StartUp.DsTrans.Tables[1].DefaultView[i]["cp_nt"] = cp_nt;
                            cp_temp += cp;
                            cp_nt_temp += cp_nt;
                        }
                    }
                    break;
                case 2: //phan bo theo so luong
                    {
                        //tổng sl
                        decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(so_luong)", "stt_rec= '" + stt_rec + "'").ToString(), out tong_sl);
                        decimal sl = 0;

                        for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
                        {
                            decimal.TryParse(StartUp.DsTrans.Tables[1].DefaultView[i]["so_luong"].ToString(), out sl);

                            cp_nt = tong_sl == 0 ? 0 : SmLib.SysFunc.Round((sl / tong_sl) * tong_cp_nt, StartUp.M_ROUND_NT);
                            cp = SmLib.SysFunc.Round(cp_nt * ty_gia, StartUp.M_ROUND);
                            StartUp.DsTrans.Tables[1].DefaultView[i]["cp"] = cp;
                            StartUp.DsTrans.Tables[1].DefaultView[i]["cp_nt"] = cp_nt;
                            cp_temp += cp;
                            cp_nt_temp += cp_nt;
                        }
                    }
                    break;
            }
            //cộng phần dư vô dòng đầu tiên
            if (loai_pb == 1 || loai_pb == 2)
            {
                StartUp.DsTrans.Tables[1].DefaultView[0]["cp"] = decimal.Parse(StartUp.DsTrans.Tables[1].DefaultView[0]["cp"].ToString()) + (tong_cp - cp_temp);
                StartUp.DsTrans.Tables[1].DefaultView[0]["cp_nt"] = decimal.Parse(StartUp.DsTrans.Tables[1].DefaultView[0]["cp_nt"].ToString()) + (tong_cp_nt - cp_nt_temp);
            }
        }
        #endregion

        #region txttong_cp_nt_LostFocus
        private void txttong_cp_nt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (currActionTask == ActionTask.Delete || currActionTask == ActionTask.View)
                return;
            IsVisibilityFieldsXamDataGridBySua_Tien();
            //tính lại giá trị của tổng thanh toán nguyên tệ
            if ((txttong_cp_nt.OldValue != txttong_cp_nt.nValue) || (ChkSuaTien.IsChecked == false))
            {
                if (cbMa_nt.Text == StartUp.M_ma_nt0)
                {
                    txttong_cp.nValue = txttong_cp_nt.nValue;
                }
                else
                {
                    if (txttong_cp_nt.nValue * txtTy_gia.nValue != 0)
                    {
                        txttong_cp.nValue = txttong_cp_nt.nValue * txtTy_gia.nValue;
                    }
                }
                Sum_ALL();
            }
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
            ExMessageBox.Show(510, StartUp.SysObj, "Đã thực hiện xong phân bổ chi phí!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

        #region GrdCp khi ở dòng cuối cùng, cột cuối cùng và Enter thì qua tab HD thuế
        private bool GrdCp_AddNewRecord(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            SmLib.WinAPISenkey.SenKey(ModifierKeys.Alt, Key.D3);

            Cell curCell = GrdCtgt.ActiveCell as Cell;
            if (curCell == null && GrdCtgt.Records.Count > 0)
            {
                Cell nextCell = (GrdCtgt.Records[0] as DataRecord).Cells[0];
                GrdCtgt.ActiveCell = nextCell;
            }
            else
            {
                if (GrdCtgt.Records.Count == 0 && IsInEditMode.Value == true)
                {
                    GrdCtgt_AddNewRecord(null, null);
                }
            }
            this.GrdCtgt.Focus();
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
                                        e.Cell.Record.Cells["cp"].Value = SmLib.SysFunc.Round(cp_nt * ty_gia, StartUp.M_ROUND);
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

        #region Lay_Record_Co_TienThueMax
        private int Lay_Index_Record_Co_TienThueMax()
        {
            int index = -1;
            double maxTien = 0;
            for (int i = 0; i < StartUp.DsTrans.Tables[2].DefaultView.Count; i++)
            {
                if (double.Parse(StartUp.DsTrans.Tables[2].DefaultView[i]["t_thue"].ToString()) > maxTien)
                {
                    maxTien = double.Parse(StartUp.DsTrans.Tables[2].DefaultView[i]["t_thue"].ToString());
                    index = i;
                }
            }
            return index;
        }
        #endregion

        #region txtloai_pb_LostFocus
        private void txtloai_pb_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtloai_pb.Text.Trim()))
                {
                    txtloai_pb.Text = "1";
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
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
            decimal t_tien_nt0_InPH = 0, t_tien0_InPH = 0, t_tien_nt0_InGrdCT = 0, t_tien0_InGrdCT = 0, ty_gia = 1;

            StartUp.DsTrans.Tables[0].AcceptChanges();
            StartUp.DsTrans.Tables[1].AcceptChanges();
            StartUp.DsTrans.Tables[2].AcceptChanges();

            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt0"].ToString(), out t_tien_nt0_InPH);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien0"].ToString(), out t_tien0_InPH);
            decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien_nt0)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), out t_tien_nt0_InGrdCT);
            decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien0)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), out t_tien0_InGrdCT);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

            //Tiền VND trong PH bằng tiền nt trong PH * tỷ giá
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien0"] = SmLib.SysFunc.Round(t_tien_nt0_InPH * ty_gia, StartUp.M_ROUND);
            //Lấy tổng tiền VND trong PH trừ tổng tiền VND trong GrdCT, phần còn dư gán vào dòng đầu tiên tổng tiền VND trong GrdCT
            for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
            {
                if (ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["tien_nt0"], 0) != 0)
                {
                    StartUp.DsTrans.Tables[1].DefaultView[i]["tien0"] = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["tien0"], 0) + (ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien0"], 0) - t_tien0_InGrdCT);
                    StartUp.DsTrans.Tables[1].DefaultView[i]["tien"] = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["tien0"], 0) + ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["cp"], 0);
                    break;
                }
            }

            //Tính lại tổng thanh toán
            decimal t_tien_nt0 = 0, t_tien0 = 0, t_cp_nt = 0, t_cp = 0, t_thue_nt = 0, t_thue = 0;

            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt0"].ToString(), out t_tien_nt0);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien0"].ToString(), out t_tien0);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_cp_nt"].ToString(), out t_cp_nt);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_cp"].ToString(), out t_cp);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_nt"].ToString(), out t_thue_nt);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue"].ToString(), out t_thue);

            StartUp.DsTrans.Tables[0].DefaultView[0]["t_tt_nt"] = t_tien_nt0 + t_cp_nt + t_thue_nt;
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_tt"] = t_tien0 + t_cp + t_thue;

            StartUp.DsTrans.Tables[0].AcceptChanges();
            StartUp.DsTrans.Tables[1].AcceptChanges();
            StartUp.DsTrans.Tables[2].AcceptChanges();

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

        private void txttong_cp_nt_GotFocus(object sender, RoutedEventArgs e)
        {
            txttong_cp_nt.SelectAll();
        }

        private void txttong_cp_GotFocus(object sender, RoutedEventArgs e)
        {
            txttong_cp.SelectAll();
        }

        private void txt_thck_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txt_thck.RowResult != null)
                StartUp.DsTrans.Tables[0].DefaultView[0]["han_tt"] = txt_thck.RowResult["han_tt"];
            else
                StartUp.DsTrans.Tables[0].DefaultView[0]["han_tt"] = 0;
            txt_thck.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                (this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus();
            }));
        }

        private void btnChonHDM_Click(object sender, RoutedEventArgs e)
        {
            if (IsInEditMode.Value)
            {
                try
                {
                    FrmPoctpnaGetHDM form = new FrmPoctpnaGetHDM();
                    if (M_LAN != "V")
                        form.Title = "Select contract";

                    form.ShowDialog();
                    if (form.isOk)
                    {
                        int count = StartUp.DsTrans.Tables[1].DefaultView.Count;
                        for (int i = 0; i < count; i++)
                        {
                            StartUp.DsTrans.Tables[1].DefaultView.Delete(0);
                        }
                        StartUp.DsTrans.Tables[1].AcceptChanges();
                        string ma_ntPH = cbMa_nt.Text.ToUpper();
                        string ma_ntHD = form.dsHdm.Tables[0].DefaultView[0]["ma_nt"].ToString().ToUpper();

                        for (int i = 0; i < form.dsHdm.Tables[1].DefaultView.Count; i++)
                        {
                            DataRow rowHdm = form.dsHdm.Tables[1].DefaultView[i].Row;
                            DataRow NewRecord = StartUp.DsTrans.Tables[1].NewRow();

                            DataTable dt1 = form.dsHdm.Tables[1].Clone();
                            dt1.Rows.Add(rowHdm.ItemArray);
                            DataTable dt2 = StartUp.DsTrans.Tables[1].Clone();
                            dt2.Merge(dt1, true, MissingSchemaAction.Ignore);

                            //NewRecord.ItemArray = rowHdm.ItemArray;
                            if (dt2.Rows.Count > 0)
                            {
                                NewRecord.ItemArray = dt2.Rows[0].ItemArray;
                            }
                            NewRecord["ton13"] = DBNull.Value;
                            decimal so_luong = 0;
                            decimal.TryParse(rowHdm["so_luong"].ToString(), out so_luong);

                            //trường hợp ma_nt của phiếu và hợp đồng giống nhau
                            if (ma_ntPH.Equals(ma_ntHD))
                            {
                                //Bằng ma_nt gốc
                                if (ma_ntPH.Equals(StartUp.M_ma_nt0))
                                {
                                    NewRecord["gia_nt0"] = rowHdm["gia0"];
                                    NewRecord["gia0"] = rowHdm["gia0"];
                                    NewRecord["tien_nt0"] = rowHdm["tien0"];
                                    NewRecord["tien0"] = rowHdm["tien0"];
                                }
                                //Khác ma_nt gốc
                                else
                                {
                                    NewRecord["gia_nt0"] = rowHdm["gia_nt0"];
                                    NewRecord["gia0"] = SmLib.SysFunc.Round(Convert.ToDecimal(rowHdm["gia_nt0"].ToString()) * txtTy_gia.nValue, StartUp.M_ROUND_GIA);
                                    NewRecord["tien_nt0"] = rowHdm["tien_nt0"];
                                    NewRecord["tien0"] = SmLib.SysFunc.Round(Convert.ToDecimal(NewRecord["gia0"].ToString()) * so_luong, StartUp.M_ROUND_GIA);
                                }
                            }
                            //trường hợp ma_nt khác nhau
                            else
                            {
                                //ma_nt trong phiếu bằng ma_nt gốc
                                if (ma_ntPH.Equals(StartUp.M_ma_nt0))
                                {
                                    NewRecord["gia_nt0"] = rowHdm["gia_nt0"];
                                    NewRecord["gia0"] = rowHdm["gia_nt0"];
                                    NewRecord["tien_nt0"] = rowHdm["tien_nt0"];
                                    NewRecord["tien0"] = rowHdm["tien_nt0"];
                                }
                                else
                                {
                                    NewRecord["gia_nt0"] = SysFunc.Round(Convert.ToDecimal(rowHdm["gia0"]) / txtTy_gia.nValue, StartUp.M_ROUND_GIA_NT);
                                    NewRecord["gia0"] = rowHdm["gia0"];
                                    NewRecord["tien_nt0"] = SysFunc.Round(Convert.ToDecimal(NewRecord["gia_nt0"]) * so_luong, StartUp.M_ROUND_NT);
                                    NewRecord["tien0"] = rowHdm["tien0"];
                                }
                            }

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
                            if (GrdCtgt.Records.Count > 0)
                            {
                                var _max_sttrec0ctgt = StartUp.DsTrans.Tables[2].AsEnumerable()
                                               .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                                               .Max(x => x.Field<string>("stt_rec0"));
                                if (_max_sttrec0ctgt != null)
                                    int.TryParse(_max_sttrec0ctgt.ToString(), out Stt_rec0ctgt);
                            }
                            Stt_rec0 = Stt_rec0ct >= Stt_rec0ctgt ? Stt_rec0ct : Stt_rec0ctgt;
                            Stt_rec0++;

                            NewRecord["stt_rec0"] = string.Format("{0:000}", Stt_rec0);

                            StartUp.DsTrans.Tables[1].Rows.Add(NewRecord);

                            txttong_cp_nt.Value = ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(cp_nt)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), 0);
                            txttong_cp.Value = ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(cp)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), 0);

                        }
                        Sum_ALL();
                        GrdCt.Focus();
                    }
                }
                catch (Exception ex)
                {
                    SmErrorLib.ErrorLog.CatchMessage(ex);
                }
            }
        }

        private void Post()
        {
            SqlCommand PostCmd = new SqlCommand("exec [POCTPNA-Post] @stt_rec");
            PostCmd.Parameters.Add("@stt_rec", SqlDbType.VarChar).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();
            StartUp.SysObj.ExcuteNonQuery(PostCmd);
        }

        private void btnDetailInfo_Click(object sender, RoutedEventArgs e)
        {
            switch (GrdInfoCP.Visibility)
            {
                case Visibility.Collapsed:
                    GrdInfoCP.Visibility = Visibility.Visible;
                    break;
                default:
                    GrdInfoCP.Visibility = Visibility.Collapsed;
                    break;
            }
        }
    }
}
