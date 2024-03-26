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


namespace SOCTPNF
{
    /// <summary>
    /// Interaction logic for FrmSOCTPNF.xaml
    /// </summary>
    public partial class FrmSOCTPNF : SmVoucherLib.FormTrans
    {
        public static int iRow = 0;
        public static int OldiRow = 0;

        public string Old_ma_thue = string.Empty;
        public string Old_thue_suat = string.Empty;

        public static CodeValueBindingObject IsInEditMode;
        CodeValueBindingObject Voucher_Ma_nt0;
        CodeValueBindingObject Voucher_Lan0;
        CodeValueBindingObject IsCheckedSua_tien;
        CodeValueBindingObject IsCheckedPn_gia_tb;
        private DataSet DsVitual;
        private DataSet dsCheckData;

        #region FrmSOCTPNF
        public FrmSOCTPNF()
        {
            InitializeComponent();
            this.BindingSysObj = StartUp.SysObj;
            Loaded += new RoutedEventHandler(FrmSOCTPNF_Loaded);
            C_QS = txtMa_qs;
            C_NgayHT = txtNgay_ct;
            C_So_ct = txtSo_ct;
            C_Ma_nt = cbMa_nt;
        }
        #endregion

        #region FrmSOCTPNF_Loaded
        void FrmSOCTPNF_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.BindingSysObj = StartUp.SysObj;
                StartUp.M_KM_CK = Convert.ToInt16(BindingSysObj.GetOption(stt_mau_temlate.ToString(), "M_KM_CK"));
                StartUp.M_AR_CK = Convert.ToInt16(BindingSysObj.GetOption(stt_mau_temlate.ToString(), "M_AR_CK"));

                currActionTask = ActionTask.View;

                //Gan iRow ở phiếu cuối cùng
                if (StartUp.DsTrans.Tables[0].Rows.Count > 1)
                    iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;


                string M_CDKH13 = SysO.GetOption("M_CDKH13").ToString().Trim();
                if (M_CDKH13 != "1")
                    txtso_du_kh.Visibility = tblso_du_kh.Visibility = Visibility.Hidden;

                IsInEditMode = (CodeValueBindingObject)FormMain.FindResource("IsInEditMode");
                Voucher_Ma_nt0 = (CodeValueBindingObject)FormMain.FindResource("Voucher_Ma_nt0");
                Voucher_Lan0 = (CodeValueBindingObject)FormMain.FindResource("Voucher_Lan0");
                IsCheckedSua_tien = (CodeValueBindingObject)FormMain.FindResource("IsCheckedSua_tien");
                IsCheckedPn_gia_tb = (CodeValueBindingObject)FormMain.FindResource("IsCheckedPn_gia_tb");

                ////Gán ngôn ngữ messagebox
                M_LAN = StartUp.M_LAN;
                GrdCt.Lan = StartUp.M_LAN;
                LanguageProvider.Language = StartUp.M_LAN;

                ////Them cac truong tu do
                SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, GrdCt, StartUp.Ma_ct, 1);

                if (StartUp.DsTrans.Tables[0].Rows.Count > 0)
                {
                    LoadData();
                    //Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background
                    //, new Action(() => 
                    //{
                        //Binding EditMode cho FormTrans (xu ly an hien nut luu va huy)
                        Binding bind = new Binding("Value");
                        bind.Source = IsInEditMode;
                        bind.Mode = BindingMode.OneWay;
                        this.SetBinding(FormTrans.IsEditModeProperty, bind);
                       
                        IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
                        IsCheckedSua_tien.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"].ToString() == "1");
                        IsCheckedPn_gia_tb.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["pn_gia_tb"].ToString() == "1");
                        Voucher_Lan0.Value = M_LAN.Trim().Equals("V");
                        SetFocusToolbar();
             
                    //})); 
                }
            
                Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));

                //Lấy số dư khách hàng tức thời
                loaddataDu13();
                UpdateTonKho();
                //Sửa lỗi binding numerictextbox format sai lần đâu tiên khi load form
                //SmLib.WinAPISenkey.SenKey(ModifierKeys.None, Key.End);

            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        #endregion

        #region LoadData()
        private void LoadData()
        {
            try
            {
                //RowFilter lại theo stt_rec
                StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
                StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";

                this.GrdLayout00.DataContext = StartUp.DsTrans.Tables[0].DefaultView;
                this.GrdLayout10.DataContext = StartUp.DsTrans.Tables[0].DefaultView;
                this.GrdLayout11.DataContext = StartUp.DsTrans.Tables[0].DefaultView;
                this.GrdLayout12.DataContext = StartUp.DsTrans.Tables[0].DefaultView;

                this.GrdLayout20.DataContext = StartUp.DsTrans.Tables[0].DefaultView;
                this.GrdLayout21.DataContext = StartUp.DsTrans.Tables[0].DefaultView;
                this.GrdLayout22.DataContext = StartUp.DsTrans.Tables[0].DefaultView;
                this.GrdLayout23.DataContext = StartUp.DsTrans.Tables[0].DefaultView;

                ////Nạp dữ liệu cho Grid hàng hóa, chi phí và hd thuế
                this.GrdCt.DataSource = StartUp.DsTrans.Tables[1].DefaultView;

                ////Nạp dữ liệu cho trạng thái chứng từ
                txtStatus.ItemsSource = StartUp.tbStatus.DefaultView;

                if (StartUp.tbStatus.DefaultView.Count == 1)
                {
                    txtStatus.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
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
                    IsInEditMode.Value = true;
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
                       // NewRecord["ma_nt"] = string.IsNullOrEmpty(cbMa_nt.Text) ? StartUp.M_ma_nt0 : cbMa_nt.Text;
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
                        NewRecord["ma_gd"] = StartUp.DsTrans.Tables[0].Rows.Count > 1 ? StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"].ToString() : StartUp.DmctInfo["ma_gd"].ToString();
                        if (NewRecord["ma_nt"].ToString().Trim().Equals(StartUp.M_ma_nt0.Trim()))
                        {
                            NewRecord["ty_giaf"] = 1;
                        }
                        else
                        {
                            NewRecord["ty_giaf"] = StartUp.GetRates(NewRecord["ma_nt"].ToString().Trim(), Convert.ToDateTime(NewRecord["ngay_ct"]).Date);
                        }

                        NewRecord["t_so_luong"] = 0;
                        NewRecord["ma_thue"] = txtMa_thue.Text.ToString();//Old_ma_thue;
                        NewRecord["thue_suat"] = ParseDecimal(txtThue_suat.Text.ToString(), 0);//Old_thue_suat;
                        if (string.IsNullOrEmpty(txtMa_thue.Text.ToString().Trim()))
                        {
                            NewRecord["tk_thue_no"] = "";
                        }
                        else
                        {
                            if (txtMa_thue.RowResult == null)
                            {
                                txtMa_thue.SearchInit();
                            }
                            NewRecord["tk_thue_no"] = txtMa_thue.RowResult["tk_thue_no"].ToString();
                            //NewRecord["loai_tk_no"] = txtMa_thue.RowResult["loai_tk_no"].ToString();
                        }
                        NewRecord["thue_dau_vao"] = 0;
                        NewRecord["t_tien"] = 0;
                        NewRecord["t_tien_nt"] = 0;
                        NewRecord["t_tien2"] = 0;
                        NewRecord["t_tien_nt2"] = 0;
                        NewRecord["t_thue_nt"] = 0;
                        NewRecord["t_thue"] = 0;
                        NewRecord["t_ck_nt"] = 0;
                        NewRecord["t_ck"] = 0;
                        NewRecord["t_tt_nt"] = 0;
                        NewRecord["t_tt"] = 0;
                        txtso_du_kh.Value = 0;

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
            try
            {
                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim()))
                    return;
                currActionTask = ActionTask.Copy;
                FrmCopy _formcopy = new FrmCopy();
                _formcopy.Closed += new EventHandler(_formcopy_Closed);
                _formcopy.ShowDialog();
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        #endregion

        #region _formcopy_Closed
        void _formcopy_Closed(object sender, EventArgs e)
        {
            try
            {
                if (FrmCopy.isCopy == true)
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
                        NewRecord["ngay_ct"] = FrmCopy.ngay_ct;
                            //NewRecord["ngay_lct"] = FrmCopy.ngay_ct;
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
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        #endregion

        #region V_Sua
        private void V_Sua()
        {
            try
            {
                if (StartUp.DsTrans.Tables[0].Rows.Count == 0)
                    ExMessageBox.Show( 1360,StartUp.SysObj, "Không có dữ liệu!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                {
                    if (!SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, txtNgay_ct.dValue))
                    {
                        ExMessageBox.Show( 1365,StartUp.SysObj, "Ngày hạch toán phải sau ngày khóa sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    IsInEditMode.Value = true;
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background
                    , new Action(() =>
                    {

                        txtMa_kh.IsFocus = true;
                    }));
                        currActionTask = ActionTask.Edit;

                        DsVitual = new DataSet();
                        DsVitual.Tables.Add(StartUp.DsTrans.Tables[0].DefaultView.ToTable());
                        DsVitual.Tables.Add(StartUp.DsTrans.Tables[1].DefaultView.ToTable());

                        Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                        Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
                }
                txtMa_kh.SearchInit();
                //txtMa_kh_PreviewLostFocus(null, null);
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        #endregion

        #region V_Huy
        private void V_Huy()
        {
            try
            {
                IsInEditMode.Value = false;
                //Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Send
                //, new Action(() => 
                //{
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
                //}));
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
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

                //Refresh lại Table[0], Table[1]
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

        #region V_In
        private void V_In()
        {
            try
            {
                StartUp.GetDmnt();
                FrmIn oReport = new FrmIn();
                oReport.ShowDialog();
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
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
            try
            {
                currActionTask = ActionTask.View;
                //  set lai stringbrowse
                string stringBrowse1, stringBrowse2;
                if (StartUp.M_LAN.Equals("V"))
                {
                    stringBrowse1 = StartUp.CommandInfo["Vbrowse1"].ToString().Split('|')[0];//"ngay_ct:fl:100:h=Ngày c.từ;so_ct:fl:70:h=Số c.từ;so_seri0:70:h=Số seri;ma_kh:100:h=Mã khách;ten_kh:180:h=Tên khách;dien_giai:225:h=Diễn giải;ma_bp:100:h=Mã bộ phận;t_tien_nt2:130:n1:h=Tiền hàng nt;thue_suat:130:F=2:h=Thuế suất;t_thue_nt:130:n1:h=Tiền thuế nt;t_tt_nt:130:n1:h=Tổng tiền nt;ma_nx:80:h=Mã nx;tk_thue_no:80:h=Tk thuế;t_tien2:130:n0:h=Tiền hàng;t_thue:n0:130:h=Tiền thuế;t_tt:n0:130:h=Tổng tiền;ma_nt:80:h=Mã nt;ty_gia:r:130:h=Tỷ giá;[date]:140:h=Ngày cập nhật;[time]:140:h=Giờ cập nhật;user_id:100:h=Số hiệu NSD:N;[user_name]:180:h=Tên NSD";
                    stringBrowse2 = StartUp.CommandInfo["Vbrowse1"].ToString().Split('|')[1];//"ma_vt:fl:100:h=Mã vật tư; ten_vt:fl:270:h=Tên vật tư;dvt:60:h=Đvt;ma_kho_i:70:h=Mã kho;so_luong:q:130:h=Số lượng;gia_nt2:130:p1:h=Giá bán nt;tien_nt2:130:n1:h=Thành tiền nt;tk_tl:80:h=Tk tl;gia_nt:130:p1:h=Giá vốn nt;tien_nt:130:n1:h=Tiền vốn nt;tk_vt:80:h=Tk kho;tk_gv:80:h=Tk gv;gia2:130:p0:h=Giá bán;tien2:130:n0:h=Thành tiền;gia:130:p0:h=Giá vốn;tien:130:n0:h=Tiền vốn";
                }
                else
                {
                    stringBrowse1 = StartUp.CommandInfo["Ebrowse1"].ToString().Split('|')[0];//"ngay_ct:fl:100:h=Ngày c.từ;so_ct:fl:70:h=Số c.từ;so_seri0:70:h=Số seri;ma_kh:100:h=Mã khách;ten_kh:180:h=Tên khách;dien_giai:225:h=Diễn giải;ma_bp:100:h=Mã bộ phận;t_tien_nt2:130:n1:h=Tiền hàng nt;thue_suat:130:F=2:h=Thuế suất;t_thue_nt:130:n1:h=Tiền thuế nt;t_tt_nt:130:n1:h=Tổng tiền nt;ma_nx:80:h=Mã nx;tk_thue_no:80:h=Tk thuế;t_tien2:130:n0:h=Tiền hàng;t_thue:n0:130:h=Tiền thuế;t_tt:n0:130:h=Tổng tiền;ma_nt:80:h=Mã nt;ty_gia:r:130:h=Tỷ giá;[date]:140:h=Ngày cập nhật;[time]:140:h=Giờ cập nhật;user_id:100:h=Số hiệu NSD:N;[user_name]:180:h=Tên NSD";
                    stringBrowse2 = StartUp.CommandInfo["Ebrowse1"].ToString().Split('|')[1];//"ma_vt:fl:100:h=Mã vật tư; ten_vt:fl:270:h=Tên vật tư;dvt:60:h=Đvt;ma_kho_i:70:h=Mã kho;so_luong:q:130:h=Số lượng;gia_nt2:130:p1:h=Giá bán nt;tien_nt2:130:n1:h=Thành tiền nt;tk_tl:80:h=Tk tl;gia_nt:130:p1:h=Giá vốn nt;tien_nt:130:n1:h=Tiền vốn nt;tk_vt:80:h=Tk kho;tk_gv:80:h=Tk gv;gia2:130:p0:h=Giá bán;tien2:130:n0:h=Thành tiền;gia:130:p0:h=Giá vốn;tien:130:n0:h=Tiền vốn";
                }
                StartUp.DsTrans.Tables[0].AcceptChanges();
                DataTable PhViewTablev = StartUp.DsTrans.Tables[0].Copy();
                PhViewTablev.Rows.RemoveAt(0);
                SmVoucherLib.FormView _frmView = new SmVoucherLib.FormView(StartUp.SysObj, PhViewTablev.DefaultView, StartUp.DsTrans.Tables[1].DefaultView, stringBrowse1, stringBrowse2, "stt_rec");
                _frmView.ListFieldSum = "t_tt_nt;t_tt";
                _frmView.frmBrw.Title = SmLib.SysFunc.Cat_Dau(M_LAN.Equals("V") ? StartUp.CommandInfo["bar"].ToString() : StartUp.CommandInfo["bar2"].ToString());

                SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, _frmView.frmBrw.oBrowseCt, StartUp.Ma_ct, 1);



                _frmView.frmBrw.LanguageID  = "SOCTPNF_7";
                _frmView.ShowDialog();

                // Set lai irow va rowfilter ...
                if (_frmView.DataGrid.ActiveRecord != null)
                {
                    int select_item_index = (_frmView.DataGrid.ActiveRecord as DataRecord).DataItemIndex;
                    //int select_irow = (_frmView.DataGrid.ActiveRecord as DataRecord).Index;
                    if (select_item_index >= 0)
                    {
                        string selected_stt_rec = (_frmView.DataGrid.DataSource as DataView)[select_item_index]["stt_rec"].ToString();
                        FrmSOCTPNF.iRow = select_item_index + 1;
                        StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + selected_stt_rec + "'";
                        StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + selected_stt_rec + "'";

                    }
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }

        }
        #endregion

        #region V_Nhan
        private void V_Nhan()
        {
            try
            {
                if (IsInEditMode.Value == false)
                    return;
                bool isError = false;
                if (!IsSequenceSave)
                {
                    StartUp.DsTrans.Tables[0].AcceptChanges();
                    StartUp.DsTrans.Tables[1].AcceptChanges();

                    


                    GrdCt.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);

                    //Nếu dữ liệu đang sửa bị sai là autocompletetextbox thì ko cho lưu.
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
                        ExMessageBox.Show( 1370,StartUp.SysObj, "Chưa vào mã khách hàng!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtMa_kh.IsFocus = true;
                        isError = true;
                    }
                    else if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nx"].ToString()))
                    {
                        ExMessageBox.Show( 1375,StartUp.SysObj, "Chưa vào tài khoản có!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtMa_nx.IsFocus = true;
                        isError = true;
                    }
                    else if (string.IsNullOrEmpty(txtNgay_ct.Text.ToString()))
                    {
                        ExMessageBox.Show( 1380,StartUp.SysObj, "Chưa vào ngày hạch toán!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtNgay_ct.Focus();
                        isError = true;
                    }

                    else if ( StartUp.M_NGAY_BAT_DAU != null && (!txtNgay_ct.IsValueValid || txtNgay_ct.dValue < StartUp.M_NGAY_BAT_DAU || txtNgay_ct.dValue > StartUp.M_NGAY_KET_THUC))
                        {
                            ExMessageBox.Show(1024, StartUp.SysObj, "Ngày hạch toán không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            isError = true;
                            txtNgay_ct.Focus();
                        }
                    else if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"].ToString().Trim()))
                    {
                        ExMessageBox.Show( 1385,StartUp.SysObj, "Chưa vào số chứng từ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtSo_ct.Focus();
                        isError = true;
                    }
                    if (ChkThueDauVao.IsChecked == true && string.IsNullOrEmpty(txtMa_Tc.Text))
                    {
                        ExMessageBox.Show(1061, StartUp.SysObj, "Chưa vào mã tính chất!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtMa_Tc.IsFocus = true;
                        isError = true;
                    }
                    if (!CheckVoucherOutofDate())
                        isError = true;
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
                                    ExMessageBox.Show( 1390,StartUp.SysObj, "Chưa vào chi tiết vật tư, không lưu được!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                    GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["ma_vt"];
                                    GrdCt.Focus();
                                    return;
                                }
                                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["ma_kho_i"].ToString()))
                                {
                                    ExMessageBox.Show( 1395,StartUp.SysObj, "Chưa vào chi tiết vật tư, không lưu được!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                    GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["ma_kho_i"];
                                    GrdCt.Focus();
                                    return;
                                }

                                //Kiem tra tk_vt(tk kho)
                                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_vt"].ToString().Trim()) && StartUp.DsTrans.Tables[1].DefaultView[i]["vt_ton_kho"].ToString().Equals("1"))
                                {
                                    ExMessageBox.Show( 1400,StartUp.SysObj, "Chưa vào tk kho!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                    GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_vt"];
                                    GrdCt.Focus();
                                    return;
                                }
                                //if (StartUp.IsTkMe(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_vt"].ToString().Trim()))
                                //{
                                //    ExMessageBox.Show( 1405,StartUp.SysObj, "Tk kho là tk tổng hợp, không lưu được!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                //    GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_vt"];
                                //    GrdCt.Focus();
                                //    return; 
                                //}

                                //Kiem tra tk hang ban tra lai
                                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_tl"].ToString().Trim()))
                                {
                                    ExMessageBox.Show( 1410,StartUp.SysObj, "Chưa vào tk hbtl!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                    GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_tl"];
                                    GrdCt.Focus();
                                    return;
                                }
                                //if (StartUp.IsTkMe(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_tl"].ToString().Trim()))
                                //{
                                //    ExMessageBox.Show( 1415,StartUp.SysObj, "Tk hbtl là tk tổng hợp, không lưu được!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                //    GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_tl"];
                                //    GrdCt.Focus();
                                //    return;
                                //}

                                //Kiem tra tk gia von
                                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_gv"].ToString().Trim()) && StartUp.DsTrans.Tables[1].DefaultView[i]["vt_ton_kho"].ToString().Equals("1"))
                                {
                                    ExMessageBox.Show( 1420,StartUp.SysObj, "Chưa vào tk giá vốn!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                    GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_gv"];
                                    GrdCt.Focus();
                                    return;
                                }
                                //if (StartUp.IsTkMe(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_gv"].ToString().Trim()))
                                //{
                                //    ExMessageBox.Show( 1425,StartUp.SysObj, "Tk giá vốn là tk tổng hợp, không lưu được!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                //    GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_gv"];
                                //    GrdCt.Focus();
                                //    return;
                                //}

                                //Kiem tra tai khoan chiet khau
                                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_ck"].ToString().Trim()))
                                {
                                    if (StartUp.M_AR_CK == 1 && ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["tl_ck"], 0) != 0)
                                    {
                                        ExMessageBox.Show( 1430,StartUp.SysObj, "Chưa vào tk ck!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                        GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_ck"];
                                        GrdCt.Focus();
                                        return;
                                    }
                                }
                                //else
                                //{
                                //    if (StartUp.M_AR_CK == 1 && ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["tl_ck"], 0) != 0)
                                //    {
                                //        if (StartUp.IsTkMe(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_ck"].ToString().Trim()))
                                //        {
                                //            ExMessageBox.Show( 1435,StartUp.SysObj, "Tk ck là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                //            GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_ck"];
                                //            GrdCt.Focus();
                                //            return;
                                //        }
                                //    }
                                //}

                                //Kiem tra tai khoan khuyen mai
                                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_km_i"].ToString().Trim()))
                                {
                                    if (StartUp.M_KM_CK == 1 && StartUp.DsTrans.Tables[1].DefaultView[i]["km_ck"].ToString().Trim() == "1")
                                    {
                                        ExMessageBox.Show( 1440,StartUp.SysObj, "Chưa vào tk cp km!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                        GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_km_i"];
                                        GrdCt.Focus();
                                        return;
                                    }
                                }
                                if (int.Parse(StartUp.DsTrans.Tables[1].DefaultView[i]["gia_ton"].ToString()) == 3)
                                {
                                    if (decimal.Parse(StartUp.DsTrans.Tables[1].DefaultView[i]["so_luong"].ToString()) == 0)
                                    {
                                        ExMessageBox.Show( 1445,StartUp.SysObj, "Vật tư tính tồn kho theo phương pháp NTXT không được nhập số lượng = 0!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                        GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["so_luong"];
                                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                        {
                                            GrdCt.Focus();
                                        }));
                                        return;
                                    }
                                }
                                //else
                                //{
                                //    if (StartUp.M_KM_CK == 1 && StartUp.DsTrans.Tables[1].DefaultView[i]["km_ck"].ToString().Trim() == "1")
                                //    {
                                //        if (StartUp.IsTkMe(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_km_i"].ToString().Trim()))
                                //        {
                                //            ExMessageBox.Show( 1450,StartUp.SysObj, "Tk cp km là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                //            GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_km_i"];
                                //            GrdCt.Focus();
                                //            return;
                                //        }
                                //    }
                                //}
                            }
                        }

                        //Kiem tra ma so thue
                        if (!StartUp.M_MST_CHECK.Equals("0"))
                        {
                            if (string.IsNullOrEmpty(ma_so_thue_dmkh) || string.IsNullOrEmpty(txtMaSoThue.Text.Trim().ToString()) || !SmLib.SysFunc.CheckSumMaSoThue(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_so_thue"].ToString().Trim()))
                            {
                                FrmKHInfo formKh = new FrmKHInfo();
                                formKh.ShowDialog();

                                if (formKh.isError)
                                {
                                    if (!SmLib.SysFunc.CheckSumMaSoThue(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_so_thue"].ToString().Trim()))
                                    {
                                        if (StartUp.M_MST_CHECK.Equals("1"))
                                            ExMessageBox.Show( 1455,StartUp.SysObj, "Mã số thuế không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);

                                        else//if (StartUp.M_MST_CHECK.Equals("2"))
                                        {
                                            ExMessageBox.Show( 1460,StartUp.SysObj, "Mã số thuế không hợp lệ, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                            isError = true;
                                            txtMaSoThue.Focus();
                                        }
                                    }
                                }
                            }
                        }

                        #region tk_thue_no
                        if (!isError && txtTk_thue_no.Text == "")
                        {
                            ExMessageBox.Show(725, StartUp.SysObj, "Chưa vào tk thuế!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            isError = true;
                            txtTk_thue_no.IsFocus = true;
                        }
                        if (!isError && !txtTk_thue_no.CheckLostFocus())
                        {
                            ExMessageBox.Show(730, StartUp.SysObj, "Tk thuế không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            isError = true;
                            txtTk_thue_no.IsFocus = true;
                        }
                        #endregion

                        #region tk_thue_co
                        if (!isError && txtTk_thue_co.Text == "")
                        {
                            ExMessageBox.Show(735, StartUp.SysObj, "Chưa vào tk thuế!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            isError = true;
                            txtMa_thue.IsFocus = true;
                        }
                        if (!isError && !txtTk_thue_co.CheckLostFocus())
                        {
                            ExMessageBox.Show(740, StartUp.SysObj, "Tk thuế không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            isError = true;
                            txtTk_thue_co.IsFocus = true;
                        }
                        #endregion
                    }
                    if (!isError)
                    {
                        //Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background
                        //, new Action(() => 
                        //{
                        if (!IsSequenceSave)
                        {
                            if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"].ToString()))
                                StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"] = StartUp.DmctInfo["ma_gd"];

                            // update thông tin cho các record Table1 (Ct) 
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

                            ////Cân bằng tiền
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
                            ExMessageBox.Show( 1465,StartUp.SysObj, "Lưu không thành công, kiểm tra lại dữ liệu!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        if (!IsSequenceSave)
                        {
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
                                                            if (ExMessageBox.Show( 1470,StartUp.SysObj, "Có chứng từ trùng số. Số cuối cùng là: " + "[" + GetLastSoct(StartUp.SysObj, txtMa_qs.Text).Trim() + "]" + ". Có lưu chứng từ này không?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                                                            {
                                                                txtSo_ct.SelectAll();
                                                                txtSo_ct.Focus();
                                                                isError = true;
                                                            }
                                                        }
                                                        else if (StartUp.M_trung_so.Equals("2"))
                                                        {
                                                            ExMessageBox.Show( 1475,StartUp.SysObj, "Số chứng từ đã tồn tại!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                                            txtSo_ct.SelectAll();
                                                            txtSo_ct.Focus();
                                                            isError = true;
                                                        }
                                                    }
                                                    break;
                                                case "PH02":
                                                    {
                                                        ExMessageBox.Show( 1480,StartUp.SysObj, "Tk có là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                        isError = true;
                                                        txtMa_nx.IsFocus = true;  
                                                    }
                                                    break;
                                                case "CT01":
                                                    {
                                                        int index = Convert.ToInt16(dv[1]);
                                                        ExMessageBox.Show( 1485,StartUp.SysObj, "Tk kho là tk tổng hợp, không lưu được!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                                        isError = true;
                                                        GrdCt.ActiveCell = (GrdCt.Records[index] as DataRecord).Cells["tk_vt"];
                                                        GrdCt.Focus();
                                                    }
                                                    break;
                                                case "CT02":
                                                    {
                                                        int index = Convert.ToInt16(dv[1]);
                                                        ExMessageBox.Show( 1490,StartUp.SysObj, "Tk hbtl là tk tổng hợp, không lưu được!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                                        isError = true;
                                                        GrdCt.ActiveCell = (GrdCt.Records[index] as DataRecord).Cells["tk_tl"];
                                                        GrdCt.Focus();
                                                    }
                                                    break;
                                                case "CT03":
                                                    {
                                                        int index = Convert.ToInt16(dv[1]);
                                                        ExMessageBox.Show( 1495,StartUp.SysObj, "Tk giá vốn là tk tổng hợp, không lưu được!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                                        isError = true;
                                                        GrdCt.ActiveCell = (GrdCt.Records[index] as DataRecord).Cells["tk_gv"];
                                                        GrdCt.Focus();
                                                    }
                                                    break;
                                                case "CT04":
                                                    {
                                                        if (StartUp.M_AR_CK == 1)
                                                        {
                                                            int index = Convert.ToInt16(dv[1]);
                                                            ExMessageBox.Show(1500, StartUp.SysObj, "Tk ck là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                            isError = true;
                                                            GrdCt.ActiveCell = (GrdCt.Records[index] as DataRecord).Cells["tk_ck"];
                                                            GrdCt.Focus();
                                                        }
                                                    }
                                                    break;
                                                case "CT05":
                                                    {
                                                        int index = Convert.ToInt16(dv[1]);
                                                        ExMessageBox.Show( 1505,StartUp.SysObj, "Tk cp km là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                        isError = true;
                                                        GrdCt.ActiveCell = (GrdCt.Records[index] as DataRecord).Cells["tk_km_I"];
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
                        }
                        //  StartUp.UpdateRates(tbPhToSave.Rows[0]["ma_nt"].ToString(), Convert.ToDateTime(txtNgay_ct.Value).Date, Convert.ToDecimal(txtTy_gia.Value));
                        if (!isError)
                        {
                            string _stt_rec = StartUp.DsTrans.Tables[1].DefaultView[0]["stt_rec"].ToString();
                            ThreadStart _thread = delegate()
                            {
                                Post();
                                if (!IsSequenceSave)
                                {
                                    //Update lại tồn kho tức thời
                                    Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                        new Action(() =>
                                        {
                                            if (_stt_rec.Equals(StartUp.DsTrans.Tables[1].DefaultView[0]["stt_rec"].ToString()))
                                            {
                                                loaddataDu13();
                                                UpdateTonKho();
                                            }
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
                                currActionTask = ActionTask.None;
                                IsInEditMode.Value = false;
                                //}));
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

        #region FormMain_EditModeEnded
        //Ham nay dung de xu ly sau khi an mot button 
        private void FormMain_EditModeEnded(object sender, string menuItemName, RoutedEventArgs e)
        {
            //MessageBox.Show(menuItemName.ToString());
            IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
            
            if (StartUp.DsTrans.Tables[0].DefaultView.Count > 0)
            {
                if (!menuItemName.Equals("btnSave"))
                {
                    loaddataDu13();
                    UpdateTonKho();
                }

                Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
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
                string ma_kho = "";
                if (StartUp.DsTrans.Tables[1].DefaultView.Count > 0)
                {
                    var _max_sttrec0ct = StartUp.DsTrans.Tables[1].AsEnumerable()
                                       .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                                       .Max(x => x.Field<string>("stt_rec0"));
                    if (_max_sttrec0ct != null)
                        int.TryParse(_max_sttrec0ct.ToString(), out Stt_rec0ct);
                    ma_kho = StartUp.DsTrans.Tables[1].DefaultView[GrdCt.Records.Count - 1]["ma_kho_i"].ToString();
                }
                Stt_rec0 = Stt_rec0ct;
                Stt_rec0++;

                NewRecord["stt_rec0"] = string.Format("{0:000}", Stt_rec0);
                NewRecord["ma_ct"] = StartUp.Ma_ct;
                NewRecord["ngay_ct"] = txtNgay_ct.Value == null ? DateTime.Now.Date : txtNgay_ct.dValue.Date;
                NewRecord["ma_kho_i"] = ma_kho;
                NewRecord["km_ck"] = 0;
                NewRecord["so_luong"] = 0;
                //Tien ban
                NewRecord["gia_nt2"] = 0;
                NewRecord["tien_nt2"] = 0;
                NewRecord["tl_ck"] = 0;
                NewRecord["ck_nt"] = 0;
                //Tien von
                NewRecord["gia_nt"] = 0;
                NewRecord["tien_nt"] = 0;

                NewRecord["gia2"] = 0;
                NewRecord["tien2"] = 0;
                NewRecord["gia"] = 0;
                NewRecord["tien"] = 0;
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

        #region GrdCt_RecordDelete
        private void GrdCt_RecordDelete(object sender, Infragistics.Windows.DataPresenter.Events.RecordsDeletedEventArgs e)
        {
            txtMa_thue.IsFocus = true;
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
                                        ExMessageBox.Show( 1510,StartUp.SysObj, "Chưa nhập mã vật tư!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                        return;
                                    }
                                    if (txt != null)
                                    {
                                        if (txt.CheckLostFocus())
                                        {
                                            string ma_vt = (GrdCt.ActiveRecord as DataRecord).Cells["ma_vt"].Value.ToString();
                                            string ten_vt;
                                            if (StartUp.M_LAN.Equals("V"))
                                            {
                                                ten_vt = (GrdCt.ActiveRecord as DataRecord).Cells["ten_vt"].Value.ToString();
                                            }
                                            else
                                            {
                                                ten_vt = (GrdCt.ActiveRecord as DataRecord).Cells["ten_vt2"].Value.ToString();
                                            }
                                            string ma_kho = (GrdCt.ActiveRecord as DataRecord).Cells["ma_kho_i"].Value.ToString();
                                            string ma_kh = txtMa_kh.Text.ToString().Trim();
                                            string ngay_ct = string.IsNullOrEmpty(txtNgay_ct.Text.ToString()) ? "" : string.Format("{0:yyyyMMdd}", (DateTime)txtNgay_ct.Value);

                                            DataTable tb = StartUp.GetSOCTPNF_PN(ma_vt, ma_kho, ma_kh, ngay_ct);
                                            if (tb.Rows.Count > 0)
                                            {
                                                FrmSOCTPNF_PN soctpnf_pn = new FrmSOCTPNF_PN(tb, ten_vt);
                                                soctpnf_pn.ShowDialog();

                                                int currRow = 0;
                                                currRow = GrdCt.ActiveRecord.Index;
                                                DataRowView drvsoctpnf_pn;
                                                if (currRow >= 0 && currRow <= GrdCt.Records.Count - 1)
                                                {
                                                    drvsoctpnf_pn = soctpnf_pn.drvFrmSOCTPNF_PN;
                                                    decimal ty_gia = txtTy_gia.nValue;

                                                    if (drvsoctpnf_pn != null)
                                                    {
                                                        string ma_ntPH = cbMa_nt.Text;
                                                        string ma_ntHD = drvsoctpnf_pn["ma_nt"].ToString();

                                                        //trường hợp ma_nt của phiếu và hợp đồng giống nhau
                                                        if (ma_ntPH.Equals(ma_ntHD))
                                                        {
                                                            //Bằng ma_nt gốc
                                                            if (ma_ntPH.Equals(StartUp.M_ma_nt0))
                                                            {
                                                                (GrdCt.DataSource as DataView)[currRow]["gia_nt2"] = drvsoctpnf_pn["gia2"];
                                                                (GrdCt.DataSource as DataView)[currRow]["gia_nt"] = drvsoctpnf_pn["gia"];
                                                                (GrdCt.DataSource as DataView)[currRow]["gia2"] = drvsoctpnf_pn["gia2"];
                                                                (GrdCt.DataSource as DataView)[currRow]["gia"] = drvsoctpnf_pn["gia"];
                                                            }
                                                            //Khác ma_nt gốc
                                                            else
                                                            {
                                                                (GrdCt.DataSource as DataView)[currRow]["gia_nt2"] = drvsoctpnf_pn["gia_nt2"];
                                                                (GrdCt.DataSource as DataView)[currRow]["gia_nt"] = drvsoctpnf_pn["gia_nt"];
                                                                (GrdCt.DataSource as DataView)[currRow]["gia2"] = SysFunc.Round(Convert.ToDecimal(drvsoctpnf_pn["gia_nt2"].ToString()) * ty_gia, StartUp.M_ROUND_GIA);
                                                                (GrdCt.DataSource as DataView)[currRow]["gia"] = SysFunc.Round(Convert.ToDecimal(drvsoctpnf_pn["gia_nt"].ToString()) * ty_gia, StartUp.M_ROUND_GIA);
                                                            }
                                                        }
                                                        //trường hợp ma_nt khác nhau
                                                        else
                                                        {
                                                            //ma_nt trong phiếu bằng ma_nt gốc
                                                            if (ma_ntPH.Equals(StartUp.M_ma_nt0))
                                                            {
                                                                (GrdCt.DataSource as DataView)[currRow]["gia_nt2"] = drvsoctpnf_pn["gia2"];
                                                                (GrdCt.DataSource as DataView)[currRow]["gia_nt"] = drvsoctpnf_pn["gia"];
                                                                (GrdCt.DataSource as DataView)[currRow]["gia2"] = drvsoctpnf_pn["gia2"];
                                                                (GrdCt.DataSource as DataView)[currRow]["gia"] = drvsoctpnf_pn["gia"];
                                                            }
                                                            else
                                                            {
                                                                (GrdCt.DataSource as DataView)[currRow]["gia_nt2"] = SysFunc.Round(Convert.ToDecimal(drvsoctpnf_pn["gia2"]) / txtTy_gia.nValue, StartUp.M_ROUND_GIA_NT);
                                                                (GrdCt.DataSource as DataView)[currRow]["gia2"] = drvsoctpnf_pn["gia2"];
                                                                (GrdCt.DataSource as DataView)[currRow]["gia_nt"] = SysFunc.Round(Convert.ToDecimal(drvsoctpnf_pn["gia"]) / txtTy_gia.nValue, StartUp.M_ROUND_GIA_NT);
                                                                (GrdCt.DataSource as DataView)[currRow]["gia"] = drvsoctpnf_pn["gia"];
                                                            }
                                                        }

                                                        (GrdCt.DataSource as DataView)[currRow]["stt_rec_px"] = drvsoctpnf_pn["stt_rec"];
                                                        (GrdCt.DataSource as DataView)[currRow]["stt_rec0px"] = drvsoctpnf_pn["stt_rec0"];

                                                        decimal gia_nt2 = 0, gia_nt = 0, gia2 = 0, gia = 0, tien_nt2 = 0, tien_nt = 0, so_luong = 0;
                                                        gia_nt2 = ParseDecimal((GrdCt.DataSource as DataView)[currRow]["gia_nt2"].ToString(), 0);
                                                        gia_nt = ParseDecimal((GrdCt.DataSource as DataView)[currRow]["gia_nt"].ToString(), 0);
                                                        gia2 = ParseDecimal((GrdCt.DataSource as DataView)[currRow]["gia2"].ToString(), 0);
                                                        gia = ParseDecimal((GrdCt.DataSource as DataView)[currRow]["gia"].ToString(), 0);
                                                        so_luong = ParseDecimal((GrdCt.DataSource as DataView)[currRow]["so_luong"].ToString(), 0);

                                                        if (gia_nt2 * so_luong != 0 && ChkSuaTien.IsChecked == false)
                                                        {
                                                            tien_nt2 = SmLib.SysFunc.Round(gia_nt2 * so_luong, StartUp.M_ROUND_NT);
                                                            (GrdCt.DataSource as DataView)[currRow]["tien_nt2"] = tien_nt2;
                                                        }
                                                        if (gia_nt * so_luong != 0 && ChkSuaTien.IsChecked == false)
                                                        {
                                                            tien_nt = SmLib.SysFunc.Round(gia_nt * so_luong, StartUp.M_ROUND_NT);
                                                            (GrdCt.DataSource as DataView)[currRow]["tien_nt"] = tien_nt;
                                                        }
                                                        if (tien_nt2 * ty_gia != 0 && ChkSuaTien.IsChecked == false)
                                                        {
                                                            (GrdCt.DataSource as DataView)[currRow]["tien2"] = SmLib.SysFunc.Round(tien_nt2 * ty_gia, StartUp.M_ROUND);
                                                        }
                                                        if (tien_nt * ty_gia != 0 && ChkSuaTien.IsChecked == false)
                                                        {
                                                            (GrdCt.DataSource as DataView)[currRow]["tien"] = SmLib.SysFunc.Round(tien_nt * ty_gia, StartUp.M_ROUND);
                                                        }
                                                        Sum_ALL();
                                                        //StartUp.DsTrans.Tables[1].AcceptChanges();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                ExMessageBox.Show( 1515,StartUp.SysObj, "Không có hóa đơn cho vật tư này!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            ExMessageBox.Show( 1520,StartUp.SysObj, "Không có hóa đơn cho vật tư này!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
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
                        if (ExMessageBox.Show( 1525,StartUp.SysObj, "Có xóa dòng ghi hiện thời không?", "Fast Book 11 .NET", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
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
                GrdCt.ActiveCell = (GrdCt.ActiveRecord as DataRecord).Cells["ma_vt"];
            }
        }
        #endregion

        #region GrdCt_EditModeStarted
        private void GrdCt_EditModeStarted(object sender, Infragistics.Windows.DataPresenter.Events.EditModeStartedEventArgs e)
        {
           
        }
        #endregion

        #region GrdCt_EditModeStarting
        private void GrdCt_EditModeStarting(object sender, Infragistics.Windows.DataPresenter.Events.EditModeStartingEventArgs e)
        {
            try
            {
                if (IsInEditMode.Value == false)
                    return;

                if (GrdCt.ActiveCell != null && StartUp.DsTrans.Tables[1].GetChanges(DataRowState.Deleted) == null)
                {
                    switch (e.Cell.Field.Name)
                    {

                        #region  case "tk_tl"
                        case "tk_tl": //Tai khoan hang ban tra lai
                            {
                                CellValuePresenter cellV = CellValuePresenter.FromCell(e.Cell.Record.Cells["ma_vt"]);
                                AutoCompleteTextBox autoCompleteMa_vt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(cellV.Editor as ControlHostEditor);

                                if (autoCompleteMa_vt != null)
                                {
                                    autoCompleteMa_vt.SearchInit();
                                    if (autoCompleteMa_vt.RowResult != null)
                                    {
                                        AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                        if (txt != null)
                                        {
                                            if (autoCompleteMa_vt.RowResult["tk_tl"] != DBNull.Value && !string.IsNullOrEmpty(autoCompleteMa_vt.RowResult["tk_tl"].ToString().Trim()) && !StartUp.IsTkMe(autoCompleteMa_vt.RowResult["tk_tl"].ToString().Trim()))
                                            {
                                                txt.IsReadOnly = true;
                                            }
                                            else
                                            {
                                                txt.IsReadOnly = false;
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        #endregion

                        #region  case "tk_gv"
                        case "tk_gv": //Tai khoan gia von
                            {
                                CellValuePresenter cellV = CellValuePresenter.FromCell(e.Cell.Record.Cells["ma_vt"]);
                                AutoCompleteTextBox autoCompleteMa_vt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(cellV.Editor as ControlHostEditor);

                                if (autoCompleteMa_vt != null)
                                {
                                    autoCompleteMa_vt.SearchInit();
                                    if (autoCompleteMa_vt.RowResult != null)
                                    {
                                        AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                        if (txt != null)
                                        {
                                            if ((autoCompleteMa_vt.RowResult["vt_ton_kho"].ToString().Equals("0")) || (autoCompleteMa_vt.RowResult["tk_gv"] != DBNull.Value && !string.IsNullOrEmpty(autoCompleteMa_vt.RowResult["tk_gv"].ToString().Trim()) && !StartUp.IsTkMe(autoCompleteMa_vt.RowResult["tk_gv"].ToString().Trim())))
                                            {
                                                txt.IsReadOnly = true;
                                            }
                                            else
                                            {
                                                txt.IsReadOnly = false;
                                            }
                                        }
                                    }
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
                        #region case "ma_vt"
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
                                    e.Cell.Record.Cells["vt_ton_kho"].Value = txt.RowResult["vt_ton_kho"];
                                    if (string.IsNullOrEmpty(e.Cell.Record.Cells["tk_vt"].Value.ToString()))
                                    {
                                        e.Cell.Record.Cells["tk_vt"].Value = txt.RowResult["tk_vt"];
                                    }

                                    //Gan  gia tri cho tk_vt
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

                                    DataRowView drvCT = e.Cell.Record.DataItem as DataRowView;
                                    drvCT["sua_tk_vt"] = txt.RowResult["sua_tk_vt"];

                                    e.Cell.Record.Cells["gia_ton"].Value = txt.RowResult["gia_ton"];

                                    //Gan gia tri cho tk_hbtl
                                    CellValuePresenter cell_Tk_tl = CellValuePresenter.FromCell(e.Cell.Record.Cells["tk_tl"]);
                                    AutoCompleteTextBox autoCompleteTk_tl = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(cell_Tk_tl.Editor as ControlHostEditor);
                                    if (txt.RowResult["tk_tl"] != DBNull.Value && !string.IsNullOrEmpty(txt.RowResult["tk_tl"].ToString().Trim()))
                                    {
                                        e.Cell.Record.Cells["tk_tl"].Value = txt.RowResult["tk_tl"];
                                        if (autoCompleteTk_tl != null)
                                        {
                                            autoCompleteTk_tl.IsReadOnly = true;
                                        }
                                    }
                                    else
                                    {
                                        if (autoCompleteTk_tl != null)
                                        {
                                            autoCompleteTk_tl.IsReadOnly = false;
                                        }
                                    }

                                    //Gan gia tri cho tk_gv
                                    CellValuePresenter cell_Tk_gv = CellValuePresenter.FromCell(e.Cell.Record.Cells["tk_gv"]);
                                    AutoCompleteTextBox autoCompleteTk_gv = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(cell_Tk_gv.Editor as ControlHostEditor);
                                    if (txt.RowResult["tk_gv"] != DBNull.Value && !string.IsNullOrEmpty(txt.RowResult["tk_gv"].ToString().Trim()))
                                    {
                                        e.Cell.Record.Cells["tk_gv"].Value = txt.RowResult["tk_gv"];
                                        if (autoCompleteTk_gv != null)
                                        {
                                            autoCompleteTk_gv.IsReadOnly = true;
                                        }
                                    }
                                    else
                                    {
                                        if (autoCompleteTk_gv != null)
                                        {
                                            autoCompleteTk_gv.IsReadOnly = false;
                                        }
                                    }
                                    if (txt.RowResult["vt_ton_kho"].ToString().Equals("0"))
                                    {
                                        //CellValuePresenter cell_Tk_gv = CellValuePresenter.FromCell(e.Cell.Record.Cells["tk_gv"]);
                                        //AutoCompleteTextBox autoCompleteTk_gv = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(cell_Tk_gv.Editor as ControlHostEditor);
                                        if (autoCompleteTk_gv != null)
                                        {
                                            autoCompleteTk_gv.IsReadOnly = true;
                                        }
                                    }

                                    //Gan gia tri cho tk_ck (ko gan khi tk_ck trong dmvt = null
                                    if (txt.RowResult["tk_ck"] != DBNull.Value && !string.IsNullOrEmpty(txt.RowResult["tk_ck"].ToString().Trim()))
                                    {
                                        e.Cell.Record.Cells["tk_ck"].Value = txt.RowResult["tk_ck"];
                                    }

                                    if (txt.RowResult["vt_ton_kho"].ToString().Equals("0"))
                                    {
                                        e.Cell.Record.Cells["so_luong"].Value = 0;
                                        StartUp.DsTrans.Tables[0].DefaultView[0]["t_so_luong"] = ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(so_luong)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter), 0);
                                        e.Cell.Record.Cells["gia_nt2"].Value = 0;
                                        e.Cell.Record.Cells["gia_nt"].Value = 0;
                                        e.Cell.Record.Cells["gia2"].Value = 0;
                                        e.Cell.Record.Cells["gia"].Value = 0;

                                        //CellValuePresenter cell_so_luong = CellValuePresenter.FromCell(e.Cell.Record.Cells["so_luong"]);
                                        //cell_so_luong.Editor.IsReadOnly = true;
                                    }
                                    else
                                    {
                                        //CellValuePresenter cell_so_luong = CellValuePresenter.FromCell(e.Cell.Record.Cells["so_luong"]);
                                        //cell_so_luong.Editor.IsReadOnly = false;
                                    }

                                    if (!string.IsNullOrEmpty(e.Cell.Record.Cells["ma_vt"].Value.ToString()) && !string.IsNullOrEmpty(e.Cell.Record.Cells["ma_kho_i"].Value.ToString()))
                                    {
                                        e.Cell.Record.Cells["ton13"].Value = InvtLib.InFuncLib.GetTon13(StartUp.SysObj, e.Cell.Record.Cells["ma_kho_i"].Value.ToString(), e.Cell.Record.Cells["ma_vt"].Value.ToString(), (e.Cell.Record.DataItem as DataRowView)["ma_vv_i"].ToString());
                                    }
                                }

                                
                                break;
                            }
                        #endregion

                        #region case "ma_kho_i"
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

                                if (!string.IsNullOrEmpty(e.Cell.Record.Cells["ma_vt"].Value.ToString()) && !string.IsNullOrEmpty(e.Cell.Record.Cells["ma_kho_i"].Value.ToString()))
                                {
                                    e.Cell.Record.Cells["ton13"].Value = InvtLib.InFuncLib.GetTon13(StartUp.SysObj, e.Cell.Record.Cells["ma_kho_i"].Value.ToString(), e.Cell.Record.Cells["ma_vt"].Value.ToString(), (e.Cell.Record.DataItem as DataRowView)["ma_vv_i"].ToString());
                                }
                                break;
                            }
                        #endregion

                        #region case "so_luong"
                        case "so_luong":
                            {
                                try
                                {
                                    if (e.Editor.Value == DBNull.Value)
                                        e.Cell.Record.Cells["so_luong"].Value = 0;

                                    decimal so_luong, gia_nt, gia_nt2, gia, gia2, tien_nt2, tien2, tien_nt, ty_gia;
                                    so_luong = (e.Editor as NumericTextBox).nValue;

                                    if (int.Parse(e.Cell.Record.Cells["gia_ton"].Value.ToString()) == 3)
                                        if (so_luong == 0)
                                        {
                                            ExMessageBox.Show(1530, StartUp.SysObj, "Vật tư tính tồn kho theo phương pháp NTXT không được nhập số lượng = 0!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                            {
                                                GrdCt.ActiveCell = e.Cell.Record.Cells["so_luong"];
                                            }));
                                            return;
                                        }

                                    if (e.Cell.IsDataChanged)
                                    {
                                        decimal.TryParse(e.Cell.Record.Cells["gia_nt"].Value.ToString(), out gia_nt);
                                        decimal.TryParse(e.Cell.Record.Cells["gia_nt2"].Value.ToString(), out gia_nt2);
                                        decimal.TryParse(e.Cell.Record.Cells["gia"].Value.ToString(), out gia);
                                        decimal.TryParse(e.Cell.Record.Cells["gia2"].Value.ToString(), out gia2);
                                        decimal.TryParse(e.Cell.Record.Cells["tien_nt"].Value.ToString(), out tien_nt);
                                        decimal.TryParse(e.Cell.Record.Cells["tien_nt2"].Value.ToString(), out tien_nt2);
                                        decimal.TryParse(e.Cell.Record.Cells["tien2"].Value.ToString(), out tien2);
                                        ty_gia = txtTy_gia.nValue;

                                        if (so_luong == 0)
                                        {
                                            e.Cell.Record.Cells["gia_nt"].Value = 0;
                                            e.Cell.Record.Cells["gia_nt2"].Value = 0;
                                            e.Cell.Record.Cells["gia"].Value = 0;
                                            e.Cell.Record.Cells["gia2"].Value = 0;
                                        }

                                        if (so_luong * gia_nt2 != 0)
                                        {
                                            e.Cell.Record.Cells["tien_nt2"].Value = SmLib.SysFunc.Round(so_luong * gia_nt2, StartUp.M_ROUND_NT);
                                        }

                                        if (so_luong * gia2 != 0)
                                        {
                                            e.Cell.Record.Cells["tien2"].Value = SmLib.SysFunc.Round(so_luong * gia2, StartUp.M_ROUND);
                                        }

                                        if (so_luong * gia_nt != 0)
                                        {
                                            tien_nt = SmLib.SysFunc.Round(so_luong * gia_nt, StartUp.M_ROUND_NT);
                                            e.Cell.Record.Cells["tien_nt"].Value = tien_nt;
                                        }

                                        if(tien_nt * ty_gia != 0)
                                        {
                                            e.Cell.Record.Cells["tien"].Value = SmLib.SysFunc.Round(tien_nt * ty_gia, StartUp.M_ROUND);
                                        }

                                        if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                        {
                                            e.Cell.Record.Cells["tien2"].Value = e.Cell.Record.Cells["tien_nt2"].Value;
                                            e.Cell.Record.Cells["tien"].Value = e.Cell.Record.Cells["tien_nt"].Value;
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

                        #region case "gia_nt"
                        case "gia_nt"://Gia von USD
                            {
                                if (e.Editor.Value == DBNull.Value)
                                    e.Cell.Record.Cells["gia_nt"].Value = 0;

                                if (e.Cell.IsDataChanged)
                                {
                                    decimal so_luong, gia_nt, tien_nt, gia, tien, ty_gia;
                                    gia_nt = (e.Editor as NumericTextBox).nValue;
                                    decimal.TryParse(e.Cell.Record.Cells["so_luong"].Value.ToString(), out so_luong);
                                    decimal.TryParse(e.Cell.Record.Cells["gia"].Value.ToString(), out gia);
                                    decimal.TryParse(e.Cell.Record.Cells["tien"].Value.ToString(), out tien);
                                    decimal.TryParse(e.Cell.Record.Cells["tien_nt"].Value.ToString(), out tien_nt);
                                    ty_gia = txtTy_gia.nValue;

                                    if (so_luong * gia_nt != 0)
                                    {
                                        tien_nt = SmLib.SysFunc.Round(so_luong * gia_nt, StartUp.M_ROUND_NT);
                                        e.Cell.Record.Cells["tien_nt"].Value = tien_nt;
                                    }

                                    if (ty_gia * gia_nt != 0)
                                    {
                                        gia = SmLib.SysFunc.Round(ty_gia * gia_nt, StartUp.M_ROUND_GIA);
                                        e.Cell.Record.Cells["gia"].Value = gia;
                                    }

                                    if (so_luong * gia != 0)
                                    {
                                        tien = SmLib.SysFunc.Round(so_luong * gia, StartUp.M_ROUND);
                                        e.Cell.Record.Cells["tien"].Value = tien;
                                    }

                                    if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["gia"].Value = SmLib.SysFunc.Round(gia_nt, StartUp.M_ROUND_GIA);
                                        e.Cell.Record.Cells["tien"].Value = SmLib.SysFunc.Round(tien_nt, StartUp.M_ROUND_NT);
                                    }

                                }
                                Sum_ALL();
                                break;
                            }
                        #endregion

                        #region case "tien_nt"
                        case "tien_nt"://Tien von USD
                            {
                                if (e.Editor.Value == DBNull.Value)
                                    e.Cell.Record.Cells["tien_nt"].Value = 0;

                                if (e.Cell.IsDataChanged)
                                {
                                    decimal tien_nt, tien, ty_gia;
                                    tien_nt = (e.Editor as NumericTextBox).nValue;
                                    decimal.TryParse(e.Cell.Record.Cells["tien"].Value.ToString(), out tien);
                                    ty_gia = txtTy_gia.nValue;

                                    if (tien_nt * ty_gia != 0)
                                    {
                                        e.Cell.Record.Cells["tien"].Value = SmLib.SysFunc.Round(tien_nt * ty_gia, StartUp.M_ROUND);
                                    }

                                    if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["tien"].Value = SmLib.SysFunc.Round(tien_nt, StartUp.M_ROUND_NT);
                                    }
                                }
                                Sum_ALL();
                                break;
                            }
                        #endregion

                        #region case "gia_nt2"
                        case "gia_nt2"://Gia ban USD
                            {
                                if (e.Editor.Value == DBNull.Value)
                                    e.Cell.Record.Cells["gia_nt2"].Value = 0;

                                if (e.Cell.IsDataChanged)
                                {
                                    decimal so_luong, gia_nt2, tien_nt2, gia2, tien2, ty_gia;
                                    gia_nt2 = (e.Editor as NumericTextBox).nValue;
                                    decimal.TryParse(e.Cell.Record.Cells["so_luong"].Value.ToString(), out so_luong);
                                    decimal.TryParse(e.Cell.Record.Cells["gia2"].Value.ToString(), out gia2);
                                    decimal.TryParse(e.Cell.Record.Cells["tien_nt2"].Value.ToString(), out tien_nt2);
                                    decimal.TryParse(e.Cell.Record.Cells["tien2"].Value.ToString(), out tien2);
                                    ty_gia = txtTy_gia.nValue;

                                    if (so_luong * gia_nt2 != 0)
                                    {
                                        tien_nt2  = SmLib.SysFunc.Round(so_luong * gia_nt2, StartUp.M_ROUND_NT);
                                        e.Cell.Record.Cells["tien_nt2"].Value = tien_nt2;
                                    }

                                    if (ty_gia * gia_nt2 != 0)
                                    {
                                        gia2 = SmLib.SysFunc.Round(ty_gia * gia_nt2, StartUp.M_ROUND_GIA);
                                        e.Cell.Record.Cells["gia2"].Value = gia2;
                                    }

                                    if (so_luong * gia2 != 0)
                                    {
                                        tien2 = SmLib.SysFunc.Round(so_luong * gia2, StartUp.M_ROUND);
                                        e.Cell.Record.Cells["tien2"].Value = tien2;
                                    }

                                    if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["gia2"].Value = SmLib.SysFunc.Round(gia_nt2, StartUp.M_ROUND_GIA);
                                        e.Cell.Record.Cells["tien2"].Value = SmLib.SysFunc.Round(tien_nt2, StartUp.M_ROUND);
                                    }
                                }
                                Sum_ALL();
                                break;
                            }
                        #endregion

                        #region case "tien_nt2"
                        case "tien_nt2":// Tien ban USD
                            {
                                if (e.Editor.Value == DBNull.Value)
                                    e.Cell.Record.Cells["tien_nt2"].Value = 0;

                                if (e.Cell.IsDataChanged)
                                {
                                    decimal tien_nt2, tien2, ty_gia;
                                    tien_nt2 = (e.Editor as NumericTextBox).nValue;
                                    decimal.TryParse(e.Cell.Record.Cells["tien2"].Value.ToString(), out tien2);
                                    ty_gia = txtTy_gia.nValue;

                                    if (tien_nt2 * ty_gia != 0)
                                    {
                                        e.Cell.Record.Cells["tien2"].Value = SmLib.SysFunc.Round(tien_nt2 * ty_gia, StartUp.M_ROUND);
                                    }

                                    if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["tien2"].Value = SmLib.SysFunc.Round(tien_nt2, StartUp.M_ROUND);
                                    }
                                }
                                Sum_ALL();
                                break;
                            }
                        #endregion

                        #region case "gia"
                        case "gia":// Gia von VND
                            {
                                if (e.Editor.Value == DBNull.Value)
                                    e.Cell.Record.Cells["gia"].Value = 0;

                                if (e.Cell.IsDataChanged)
                                {
                                    decimal gia, so_luong, tien;
                                    gia = (e.Editor as NumericTextBox).nValue;
                                    decimal.TryParse(e.Cell.Record.Cells["so_luong"].Value.ToString(), out so_luong);
                                    decimal.TryParse(e.Cell.Record.Cells["tien"].Value.ToString(), out tien);

                                    if (gia * so_luong != 0)
                                    {
                                        e.Cell.Record.Cells["tien"].Value = SmLib.SysFunc.Round(gia * so_luong, StartUp.M_ROUND);
                                        //e.Cell.Record.Cells["gia"].Value = SmLib.SysFunc.Round(tien / so_luong, StartUp.M_ROUND_GIA);
                                    }
                                }
                                Sum_ALL();
                                break;
                            }
                        #endregion

                        #region case "tien"
                        case "tien"://Tien von VND
                            {
                                if (e.Editor.Value == DBNull.Value)
                                    e.Cell.Record.Cells["tien"].Value = 0;

                                Sum_ALL();
                                break;
                            }
                        #endregion

                        #region case "gia2"
                        case "gia2":// Gia ban VND
                            {
                                if (e.Editor.Value == DBNull.Value)
                                    e.Cell.Record.Cells["gia2"].Value = 0;

                                if (e.Cell.IsDataChanged)
                                {
                                    decimal gia2, so_luong, tien2;
                                    gia2 = (e.Editor as NumericTextBox).nValue;
                                    decimal.TryParse(e.Cell.Record.Cells["so_luong"].Value.ToString(), out so_luong);
                                    decimal.TryParse(e.Cell.Record.Cells["tien2"].Value.ToString(), out tien2);

                                    if (gia2 * so_luong != 0)
                                    {
                                        e.Cell.Record.Cells["tien2"].Value = SmLib.SysFunc.Round(gia2 * so_luong, StartUp.M_ROUND);
                                        //e.Cell.Record.Cells["gia2"].Value = SmLib.SysFunc.Round(tien2 / so_luong, StartUp.M_ROUND_GIA);
                                    }
                                }
                                Sum_ALL();
                                break;
                            }
                        #endregion

                        #region case "tien2"
                        case "tien2"://Tien ban VND
                            {
                                if (e.Editor.Value == DBNull.Value)
                                    e.Cell.Record.Cells["tien2"].Value = 0;

                                Sum_ALL();
                                break;
                            }
                        #endregion

                        #region case "tl_ck"
                        case "tl_ck"://Ty le chiet khau
                            {
                                if (e.Editor.Value == DBNull.Value)
                                    e.Cell.Record.Cells["tl_ck"].Value = 0;

                                if (e.Cell.IsDataChanged)
                                {
                                    decimal ck_nt, tien_nt2, tl_ck, ck, tien2;
                                    tl_ck = (e.Editor as NumericTextBox).nValue;
                                    decimal.TryParse(e.Cell.Record.Cells["ck_nt"].Value.ToString(), out ck_nt);
                                    decimal.TryParse(e.Cell.Record.Cells["tien_nt2"].Value.ToString(), out tien_nt2);
                                    decimal.TryParse(e.Cell.Record.Cells["ck"].Value.ToString(), out ck);
                                    decimal.TryParse(e.Cell.Record.Cells["tien2"].Value.ToString(), out tien2);

                                    e.Cell.Record.Cells["ck_nt"].Value = SmLib.SysFunc.Round(tien_nt2 * tl_ck / 100, StartUp.M_ROUND_NT);
                                    e.Cell.Record.Cells["ck"].Value = SmLib.SysFunc.Round(tien2 * tl_ck / 100, StartUp.M_ROUND);

                                    if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["ck"].Value = e.Cell.Record.Cells["ck_nt"].Value;
                                    }

                                }
                                Sum_ALL();
                                break;
                            }
                        #endregion

                        #region case "ck_nt"
                        case "ck_nt": //Chiet khau USD
                            {
                                if (e.Editor.Value == DBNull.Value)
                                    e.Cell.Record.Cells["ck_nt"].Value = 0;

                                if (e.Cell.IsDataChanged)
                                {
                                    decimal ck, ck_nt, ty_gia;
                                    ck_nt = (e.Editor as NumericTextBox).nValue;
                                    decimal.TryParse(e.Cell.Record.Cells["ck"].Value.ToString(), out ck);
                                    ty_gia = txtTy_gia.nValue;

                                    if (ck_nt * ty_gia != 0)
                                    {
                                        e.Cell.Record.Cells["ck"].Value = SmLib.SysFunc.Round(ck_nt * ty_gia, StartUp.M_ROUND);
                                    }

                                    if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["ck"].Value = e.Cell.Record.Cells["ck_nt"].Value;
                                    }

                                }
                                Sum_ALL();
                                break;
                            }
                        #endregion

                        #region case "ck"
                        case "ck": //Chiet khau VND
                            {
                                if (e.Editor.Value == DBNull.Value)
                                    e.Cell.Record.Cells["ck"].Value = 0;

                                Sum_ALL();
                                break;
                            }
                        #endregion

                        #region case "km_ck"
                        case "km_ck": //Km
                            {
                                if (e.Editor.Value == DBNull.Value)
                                    e.Cell.Record.Cells["km_ck"].Value = 0;

                                decimal km_ck;
                                decimal.TryParse(e.Cell.Record.Cells["km_ck"].Value.ToString(), out km_ck);

                                if (km_ck == 1)
                                {
                                    CellValuePresenter cell_mavt = CellValuePresenter.FromCell(e.Cell.Record.Cells["ma_vt"]);
                                    AutoCompleteTextBox autoCompleteMavt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(cell_mavt.Editor as ControlHostEditor);

                                    if (autoCompleteMavt != null)
                                    {
                                        autoCompleteMavt.SearchInit();
                                        if (autoCompleteMavt.RowResult != null)
                                        {
                                            if (autoCompleteMavt.RowResult["tk_km"] != DBNull.Value && !string.IsNullOrEmpty(autoCompleteMavt.RowResult["tk_km"].ToString()) && string.IsNullOrEmpty(e.Cell.Record.Cells["tk_km_i"].Value.ToString().Trim()))
                                            {
                                                e.Cell.Record.Cells["tk_km_i"].Value = autoCompleteMavt.RowResult["tk_km"];
                                            }
                                        }
                                    }
                                }
                                if (km_ck == 0)
                                {
                                    e.Cell.Record.Cells["tk_km_i"].Value = "";
                                }
                                Sum_ALL();
                                break;
                            }
                        #endregion

                        #region  case "tk_tl"
                        case "tk_tl": //Tai khoan hang ban tra lai
                            {
                                break;
                            }
                        #endregion

                        #region  case "tk_gv"
                        case "tk_gv": //Tai khoan gia von
                            {
                                break;
                            }
                        #endregion

                        #region  case "tk_km_i"
                        case "tk_km_i": //Tai khoan khuyen mai
                            {
                                Sum_ALL();
                                break;
                            }
                        #endregion

                        #region ma_vv_i
                        case "ma_vv_i":
                            {
                                if(e.Editor.Value==null)
                                    return;

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

        #region cbMa_nt_PreviewLostFocus
        private void cbMa_nt_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            try
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
                TyGiaValueChange();
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        #endregion

        #region ChkTinhThueTruocCK_Click
        private void ChkTinhThueTruocCK_Click(object sender, RoutedEventArgs e)
        {
            Sum_ALL();
        }
        #endregion

        #region ChkSuaTienThue_Click
        private void ChkSuaTienThue_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                txtT_thue_nt.IsTabStop = true;
                txtT_thue_nt.Focus();
            }
            else
            {
                txtMa_kh.IsFocus = true;
            }
            Sum_ALL();
        }
        #endregion

        #region ChkSuaHtThue_Click
        private void ChkSuaHtThue_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                txtTk_thue_co.IsTabStop = true;
                txtTk_thue_co.IsFocus = true;
            }
            else
            {
                txtMa_kh.IsFocus = true;
            }
        }
        #endregion

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

        #region ChkNhapGiaTB_Click
        private void ChkNhapGiaTB_Click(object sender, RoutedEventArgs e)
        {
            IsCheckedPn_gia_tb.Value = ChkNhapGiaTB.IsChecked.Value;
        }
        #endregion

        #region txtMa_thue_PreviewLostFocus
        private void txtMa_thue_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtMa_thue.RowResult != null)
            {
                Old_ma_thue = txtMa_thue.Text.ToString();
                Old_thue_suat = txtMa_thue.RowResult["thue_suat"].ToString();
                txtThue_suat.Text = txtMa_thue.RowResult["thue_suat"].ToString();
                txtTk_thue_no.Text = txtMa_thue.RowResult["tk_thue_no"].ToString();
                StartUp.DsTrans.Tables[0].DefaultView[0]["loai_tk_no"] = txtMa_thue.RowResult["loai_tk_no"];

                txtTk_thue_no.SearchInit();
                if (txtTk_thue_no.RowResult != null && !string.IsNullOrEmpty(txtTk_thue_no.Text.Trim()))
                {
                    StartUp.DsTrans.Tables[0].DefaultView[0]["tk_thue_no_cn"] = txtTk_thue_no.RowResult["tk_cn"];
                }
                Sum_ALL();
            }
        }
        #endregion

        #region txtT_thue_nt_LostFocus
        private void txtT_thue_nt_LostFocus(object sender, RoutedEventArgs e)
        {
            decimal ty_gia = 0, t_thue_nt = 0;
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_nt"].ToString(), out t_thue_nt);
            if (t_thue_nt * ty_gia != 0 && ChkSuaTien.IsChecked == false)
            {
                StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue"] = SmLib.SysFunc.Round(t_thue_nt * ty_gia, StartUp.M_ROUND);
            }
            Sum_ALL();
        }
        #endregion

        private void txtT_thue_LostFocus(object sender, RoutedEventArgs e)
        {
            Sum_ALL();
        }

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
            TyGiaValueChange();
        }
        #endregion

        #region TyGiaValueChange
        //Xu ly khi thay doi ty gia
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
                        decimal tien_nt = 0, tien_nt2 = 0, gia_nt = 0, gia_nt2 = 0, ck_nt = 0, ty_gia = 0, so_luong = 0;
                        ty_gia = txtTy_gia.nValue;
                        if (GrdCt.Records.Count > 0 && (GrdCt.DataSource as DataView).Table.DefaultView[0]["ma_vt"] != DBNull.Value)
                        {
                            for (int i = 0; i < GrdCt.Records.Count; i++)
                            {
                                so_luong = (GrdCt.DataSource as DataView)[i]["so_luong"] == DBNull.Value ? 0 : Convert.ToDecimal((GrdCt.Records[i] as DataRecord).Cells["so_luong"].Value);
                                gia_nt = (GrdCt.DataSource as DataView)[i]["gia_nt"] == DBNull.Value ? 0 : Convert.ToDecimal((GrdCt.Records[i] as DataRecord).Cells["gia_nt"].Value);
                                gia_nt2 = (GrdCt.DataSource as DataView)[i]["gia_nt2"] == DBNull.Value ? 0 : Convert.ToDecimal((GrdCt.Records[i] as DataRecord).Cells["gia_nt2"].Value);
                                ck_nt = (GrdCt.DataSource as DataView)[i]["ck_nt"] == DBNull.Value ? 0 : Convert.ToDecimal((GrdCt.Records[i] as DataRecord).Cells["ck_nt"].Value);

                                if (so_luong * gia_nt != 0)
                                {
                                    (GrdCt.DataSource as DataView)[i]["tien_nt"] = SmLib.SysFunc.Round(so_luong * gia_nt, StartUp.M_ROUND_NT);
                                }

                                if (so_luong * gia_nt2 != 0)
                                {
                                    (GrdCt.DataSource as DataView)[i]["tien_nt2"] = SmLib.SysFunc.Round(so_luong * gia_nt2, StartUp.M_ROUND_NT);
                                }

                                tien_nt = (GrdCt.DataSource as DataView)[i]["tien_nt"] == DBNull.Value ? 0 : Convert.ToDecimal((GrdCt.Records[i] as DataRecord).Cells["tien_nt"].Value);
                                tien_nt2 = (GrdCt.DataSource as DataView)[i]["tien_nt2"] == DBNull.Value ? 0 : Convert.ToDecimal((GrdCt.Records[i] as DataRecord).Cells["tien_nt2"].Value);

                                if (ty_gia * tien_nt != 0)
                                {
                                    (GrdCt.DataSource as DataView)[i]["tien"] = SmLib.SysFunc.Round(ty_gia * tien_nt, StartUp.M_ROUND);
                                }
                                if (ty_gia * tien_nt2 != 0)
                                {
                                    (GrdCt.DataSource as DataView)[i]["tien2"] = SmLib.SysFunc.Round(ty_gia * tien_nt2, StartUp.M_ROUND);
                                }
                                if (ty_gia * gia_nt != 0)
                                {
                                    (GrdCt.DataSource as DataView)[i]["gia"] = SmLib.SysFunc.Round(ty_gia * gia_nt, StartUp.M_ROUND_GIA);
                                }
                                if (ty_gia * gia_nt2 != 0)
                                {
                                    (GrdCt.DataSource as DataView)[i]["gia2"] = SmLib.SysFunc.Round(ty_gia * gia_nt2, StartUp.M_ROUND_GIA);
                                }
                                if (ty_gia * ck_nt != 0)
                                {
                                    (GrdCt.DataSource as DataView)[i]["ck"] = SmLib.SysFunc.Round(ty_gia * ck_nt, StartUp.M_ROUND);
                                }
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

        #region txtMa_nx_PreviewLostFocus
        private void txtMa_nx_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtMa_nx.RowResult != null)
            {
                StartUp.DsTrans.Tables[0].DefaultView[0]["ten_tk"] = txtMa_nx.RowResult["ten_nx"].ToString();
                StartUp.DsTrans.Tables[0].DefaultView[0]["ten_tk2"] = txtMa_nx.RowResult["ten_nx2"].ToString();
            }
            loaddataDu13();
            if (ChkSuaHtThue.IsChecked == false)
            {
                txtTk_thue_co.Text = txtMa_nx.Text.ToString();
            }
        }
        #endregion

        #region txtMa_kh_PreviewLostFocus
        private bool txtDiaChiFocusable = true;

        public string ma_so_thue_dmkh = string.Empty;
        private void txtMa_kh_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            try
            {
                if (IsInEditMode.Value == true)
                {
                    if (txtMa_kh.RowResult == null)
                        return;
                    StartUp.DsTrans.Tables[0].DefaultView[0]["ten_kh"] = txtMa_kh.RowResult["ten_kh"].ToString().Trim();
                    StartUp.DsTrans.Tables[0].DefaultView[0]["ten_kh2"] = txtMa_kh.RowResult["ten_kh2"].ToString().Trim();

                    if (txtMa_kh.IsDataChanged)
                    {
                        ma_so_thue_dmkh = txtMa_kh.RowResult["ma_so_thue"].ToString().Trim();

                        StartUp.DsTrans.Tables[0].DefaultView[0]["ma_so_thue"] = txtMa_kh.RowResult["ma_so_thue"].ToString().Trim();

                        if (string.IsNullOrEmpty(txtOng_ba.Text.Trim()))
                        {
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ong_ba"] = txtMa_kh.RowResult["doi_tac"].ToString().Trim();
                        }
                        StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nx"] = string.IsNullOrEmpty(txtMa_nx.Text.Trim()) ? txtMa_kh.RowResult["tk"].ToString().Trim() : txtMa_nx.Text.Trim();
                        if (ChkSuaHtThue.IsChecked == false)
                        {
                            txtTk_thue_co.Text = txtMa_nx.Text.ToString();
                        }

                        loaddataDu13();
                    }

                    if (txtMa_kh.RowResult["dia_chi"].ToString().Trim().Equals(""))
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
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
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
                    //    ExMessageBox.Show( 1535,StartUp.SysObj, "Ngày hạch toán phải sau ngày khóa sổ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
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
                        ExMessageBox.Show( 1540,StartUp.SysObj, "Ngày lập chứng từ khác với ngày hạch toán!", "", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    if (ChkThueDauVao.IsChecked == false)
                    {
                        if (txtMa_qs.RowResult != null)
                        {
                            txtkh_mau_hd.Text = txtMa_qs.RowResult["mau_hd"].ToString();
                            if (string.IsNullOrEmpty(txtkh_mau_hd.Text.Trim()))
                            {
                                txtkh_mau_hd.Text = txtMa_qs.RowResult["kh_mau_hd"].ToString();
                            }

                        }
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
            txtSo_ct.Text = txtSo_ct.Text.Trim().ToString();
            DataTable tableFields = null;
            tableFields = SmDataLib.ListFunc.GetSqlTableFieldList(StartUp.SysObj, "v_PH76");
            txtSo_ct.MaxLength = SmDataLib.ListFunc.GetLengthColumn(tableFields, "so_ct");
        }
        #endregion

        #region btnThongTin_Click
        private void btnThongTin_Click(object sender, RoutedEventArgs e)
        {
            //Nếu dữ liệu đang sửa bị sai là autocompletetextbox thì ko cho lưu.
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
            LoadFormThongTin();
        }
        #endregion

        #region LoadFormThongTin
        public void LoadFormThongTin()
        {
            try
            {
                decimal so_luong_km = 0, t_tien_km_nt = 0, t_tien_km = 0, t_thue_km_nt = 0, t_thue_km = 0;
                decimal thue_suat = 0;
                int km_ck = 0;

                decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["thue_suat"].ToString(), out thue_suat);

                foreach (DataRowView drv in StartUp.DsTrans.Tables[1].DefaultView)
                {
                    int.TryParse(drv.Row["km_ck"].ToString().Trim(), out km_ck);
                    if (km_ck == 1 && !string.IsNullOrEmpty(drv.Row["tk_km_i"].ToString().Trim()))
                    {
                        so_luong_km = so_luong_km + ParseDecimal(drv.Row["so_luong"].ToString(), 0);
                        t_tien_km_nt = t_tien_km_nt + ParseDecimal(drv.Row["tien_nt2"].ToString(), 0);
                        t_tien_km = t_tien_km + ParseDecimal(drv.Row["tien2"].ToString(), 0);

                    }
                }
                if (StartUp.M_THUE_KM_CK == 0)
                {
                    t_thue_km_nt = 0;
                    t_thue_km = 0;
                }
                else
                {
                    t_thue_km_nt = SmLib.SysFunc.Round(t_tien_km_nt * thue_suat / 100, StartUp.M_ROUND_NT);
                    t_thue_km = SmLib.SysFunc.Round(t_tien_km * thue_suat / 100, StartUp.M_ROUND);
                }

                FrmThongTin frmThongTin = new FrmThongTin();
                frmThongTin.Title = SmLib.SysFunc.Cat_Dau(M_LAN.Equals("V") ? StartUp.CommandInfo["bar"].ToString() : StartUp.CommandInfo["bar2"].ToString());
                frmThongTin.so_luong_km = so_luong_km;
                frmThongTin.t_tien_km_nt = t_tien_km_nt;
                frmThongTin.t_tien_km = t_tien_km;
                frmThongTin.t_thue_km_nt = t_thue_km_nt;
                frmThongTin.t_thue_km = t_thue_km;

                if (cbMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0))
                {
                    frmThongTin.GridLayout12.Visibility = Visibility.Hidden;
                }
                else
                {
                    frmThongTin.GridLayout12.Visibility = Visibility.Visible;
                }
                frmThongTin.ShowDialog();

            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        #endregion
        
        #region Sum_ALL
        void Sum_ALL()
        {
            try
            {
                decimal t_tien_nt2 = 0, t_thue_nt = 0, t_ck_nt = 0, t_tien2 = 0, t_thue = 0, t_ck = 0;

                string filter = "";
                if (StartUp.M_KM_CK == 1)
                {
                    StartUp.DsTrans.Tables[1].AcceptChanges();
                    filter = "km_ck = 0 AND stt_rec = '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
                }
                else
                {
                    filter = "stt_rec = '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
                }

                StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt2"] = ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(tien_nt2)", filter), 0);
                StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien2"] = ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(tien2)", filter), 0);
                StartUp.DsTrans.Tables[0].DefaultView[0]["t_ck_nt"] = ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(ck_nt)", filter), 0);
                StartUp.DsTrans.Tables[0].DefaultView[0]["t_ck"] = ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(ck)", filter), 0);
                TinhThue();

                //decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt"].ToString(), out t_tien_nt);
                decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt2"].ToString(), out t_tien_nt2);
                //decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien"].ToString(), out t_tien);
                decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien2"].ToString(), out t_tien2);
                decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_nt"].ToString(), out t_thue_nt);
                decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue"].ToString(), out t_thue);
                decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_ck_nt"].ToString(), out t_ck_nt);
                decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_ck"].ToString(), out t_ck);

                StartUp.DsTrans.Tables[0].DefaultView[0]["t_tt_nt"] = t_tien_nt2 + t_thue_nt - t_ck_nt;
                StartUp.DsTrans.Tables[0].DefaultView[0]["t_tt"] = t_tien2 + t_thue - t_ck;
                StartUp.DsTrans.Tables[0].DefaultView[0]["t_so_luong"] = ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(so_luong)", filter), 0);
                if (StartUp.M_KM_CK == 1)
                {
                    StartUp.DsTrans.Tables[1].AcceptChanges();
                    filter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
                }
                StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt"] = ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(tien_nt)", filter), 0);
                StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien"] = ParseDecimal(StartUp.DsTrans.Tables[1].Compute("sum(tien)", filter), 0);
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
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
            //return StartUp.GetLanguageString(code, language);
            string sResult = code;
            switch(code)
            {
                case "M_MA_NT":
                    {
                        //if (cbMa_nt.Text.Trim() != "VND")
                        if (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString() != StartUp.M_ma_nt0)
                        
                            sResult = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                        else
                            sResult = "";
                    }
                    break;
                case "M_MA_NT0":
                    {
                        sResult = StartUp.M_ma_nt0;
                    }
                    break;
            }
            return sResult;

        }
        #endregion

        #region TinhThue
        //Tinh Thue (theo gia truoc chiet khau hay ko)
        public void TinhThue()
        {
            try
            {
                int TinhThueTruocCk = 0, SuaTienThue = 0;
                int.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["sua_thue"].ToString(), out SuaTienThue);
                int.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["thue_ck0"].ToString(), out TinhThueTruocCk);

                decimal t_tien_nt2 = 0, t_tien2 = 0, thue_suat = 0, t_ck_nt = 0, t_ck = 0;
                decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt2"].ToString(), out t_tien_nt2);
                decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien2"].ToString(), out t_tien2);
                decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["thue_suat"].ToString(), out thue_suat);
                decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_ck_nt"].ToString(), out t_ck_nt);
                decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_ck"].ToString(), out t_ck);

                if (SuaTienThue == 0)
                {
                    if (TinhThueTruocCk == 1)
                    {
                        StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_nt"] = SmLib.SysFunc.Round(t_tien_nt2 * thue_suat / 100, StartUp.M_ROUND_NT);
                        StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue"] = SmLib.SysFunc.Round(t_tien2 * thue_suat / 100, StartUp.M_ROUND);
                    }
                    else
                    {
                        StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_nt"] = SmLib.SysFunc.Round((t_tien_nt2 - t_ck_nt) * thue_suat / 100, StartUp.M_ROUND_NT);
                        StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue"] = SmLib.SysFunc.Round((t_tien2 - t_ck) * thue_suat / 100, StartUp.M_ROUND);
                    }
                }
                else
                {
                    if (cbMa_nt.Text == StartUp.M_ma_nt0)
                    {
                        StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_nt"] = SmLib.SysFunc.Round(ParseDecimal(txtT_thue_nt.Text.Trim().ToString(), 0), StartUp.M_ROUND_NT);
                        StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue"] = StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_nt"];
                    }
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        #endregion

        #region IsVisibilityFieldsXamDataGrid
        void IsVisibilityFieldsXamDataGrid(string ma_nt)
        {
            //Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background
            //, new Action(() => { ChangeLanguage(); }));

            //Dispatcher.Invoke(new Action(delegate()
            //{
            //    try
            //    {
            //        LanguageProvider.ChangeLanguage(GrdCt as Visual, LanguageID.Trim() + ".TabInfo.tabItemHT", StartUp.M_LAN, false);
            //    }
            //    catch (Exception)
            //    {
            //    }

            //}), DispatcherPriority.Background, new object[] { });

            #region Visible column

            if (ma_nt == StartUp.M_ma_nt0)
            {
                GrdCt.FieldLayouts[0].Fields["tien"].Visibility = Visibility.Hidden;
                GrdCt.FieldLayouts[0].Fields["tien2"].Visibility = Visibility.Hidden;
                GrdCt.FieldLayouts[0].Fields["gia"].Visibility = Visibility.Hidden;
                GrdCt.FieldLayouts[0].Fields["gia2"].Visibility = Visibility.Hidden;
                GrdCt.FieldLayouts[0].Fields["ck"].Visibility = Visibility.Hidden;
                GrdCt.FieldLayouts[0].Fields["sua_tk_vt"].Visibility = Visibility.Hidden;

                GrdCt.FieldLayouts[0].Fields["tien"].Settings.CellMaxWidth = 0;
                GrdCt.FieldLayouts[0].Fields["tien2"].Settings.CellMaxWidth = 0;
                GrdCt.FieldLayouts[0].Fields["gia"].Settings.CellMaxWidth = 0;
                GrdCt.FieldLayouts[0].Fields["gia2"].Settings.CellMaxWidth = 0;
                GrdCt.FieldLayouts[0].Fields["ck"].Settings.CellMaxWidth = 0;
                GrdCt.FieldLayouts[0].Fields["sua_tk_vt"].Settings.CellMaxWidth = 0;

                txtTy_gia.IsReadOnly = true;
            }
            else
            {
                GrdCt.FieldLayouts[0].Fields["tien"].Visibility = Visibility.Visible;
                GrdCt.FieldLayouts[0].Fields["tien2"].Visibility = Visibility.Visible;
                GrdCt.FieldLayouts[0].Fields["gia"].Visibility = Visibility.Visible;
                GrdCt.FieldLayouts[0].Fields["gia2"].Visibility = Visibility.Visible;
                if (StartUp.M_AR_CK == 0)
                    GrdCt.FieldLayouts[0].Fields["ck"].Visibility = Visibility.Visible;
                GrdCt.FieldLayouts[0].Fields["sua_tk_vt"].Visibility = Visibility.Hidden;

                GrdCt.FieldLayouts[0].Fields["tien"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["tien"].Width.Value.Value;
                GrdCt.FieldLayouts[0].Fields["tien2"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["tien2"].Width.Value.Value;
                GrdCt.FieldLayouts[0].Fields["gia"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["gia"].Width.Value.Value;
                GrdCt.FieldLayouts[0].Fields["gia2"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["gia2"].Width.Value.Value;
                if (StartUp.M_AR_CK == 0)
                    GrdCt.FieldLayouts[0].Fields["ck"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["ck"].Width.Value.Value;
                GrdCt.FieldLayouts[0].Fields["sua_tk_vt"].Settings.CellMaxWidth = 0;

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

            //Chiet khau khi xuat hoa don ban hang
            if (StartUp.M_AR_CK == 0)
            {
                ChkTinhThueTruocCK.IsEnabled = false;

                //GrdCt.FieldLayouts[0].Fields["tl_ck"].Visibility = Visibility.Hidden;
                //GrdCt.FieldLayouts[0].Fields["ck"].Visibility = Visibility.Hidden;
                //GrdCt.FieldLayouts[0].Fields["ck_nt"].Visibility = Visibility.Hidden;
                //GrdCt.FieldLayouts[0].Fields["tk_ck"].Visibility = Visibility.Hidden;

                //GrdCt.FieldLayouts[0].Fields["tl_ck"].Settings.CellMaxWidth = 0;
                //GrdCt.FieldLayouts[0].Fields["ck"].Settings.CellMaxWidth = 0;
                //GrdCt.FieldLayouts[0].Fields["ck_nt"].Settings.CellMaxWidth = 0;
                //GrdCt.FieldLayouts[0].Fields["tk_ck"].Settings.CellMaxWidth = 0;
            }
            else
            {
                if (IsInEditMode.Value == true)
                {
                    ChkTinhThueTruocCK.IsEnabled = true;
                }
                else
                {
                    ChkTinhThueTruocCK.IsEnabled = false;
                }
                //if (ma_nt != StartUp.M_ma_nt0)
                //{
                //    GrdCt.FieldLayouts[0].Fields["ck"].Visibility = Visibility.Visible;
                //    GrdCt.FieldLayouts[0].Fields["ck"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["ck"].Width.Value.Value;
                //}
                //GrdCt.FieldLayouts[0].Fields["tl_ck"].Visibility = Visibility.Visible;
                //GrdCt.FieldLayouts[0].Fields["ck_nt"].Visibility = Visibility.Visible;
                //GrdCt.FieldLayouts[0].Fields["tk_ck"].Visibility = Visibility.Visible;

                //GrdCt.FieldLayouts[0].Fields["tl_ck"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["tl_ck"].Width.Value.Value;
                //GrdCt.FieldLayouts[0].Fields["ck_nt"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["ck_nt"].Width.Value.Value;
                //GrdCt.FieldLayouts[0].Fields["tk_ck"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["tk_ck"].Width.Value.Value;
            }

            //Khuyen mai khi xuat hoa don
            //an theo template
            //if (StartUp.M_KM_CK == 0)
            //{
            //    GrdCt.FieldLayouts[0].Fields["km_ck"].Visibility = Visibility.Hidden;
            //    GrdCt.FieldLayouts[0].Fields["tk_km_i"].Visibility = Visibility.Hidden;
            //    GrdCt.FieldLayouts[0].Fields["km_ck"].Settings.CellMaxWidth = 0;
            //    GrdCt.FieldLayouts[0].Fields["tk_km_i"].Settings.CellMaxWidth = 0;

            //}
            //else
            //{
            //    GrdCt.FieldLayouts[0].Fields["km_ck"].Visibility = Visibility.Visible;
            //    GrdCt.FieldLayouts[0].Fields["tk_km_i"].Visibility = Visibility.Visible;
            //    GrdCt.FieldLayouts[0].Fields["km_ck"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["km_ck"].Width.Value.Value;
            //    GrdCt.FieldLayouts[0].Fields["tk_km_i"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["tk_km_i"].Width.Value.Value;
            //}


            Old_ma_thue = txtMa_thue.Text.ToString();
            Old_thue_suat = txtThue_suat.Text.ToString();

            //LanguageProvider.ChangeLanguage(GrdCt as Visual, LanguageID.Trim() + ".TabInfo.tabItemHT", StartUp.M_LAN, false);
            ChangeLanguage();
            #endregion
        }
        #endregion

        #region PhanBoThueInCT
        void PhanBoThueInCT()
        {
            try
            {
                if (StartUp.DsTrans.Tables[1].DefaultView.Count == 0)
                    return;

                //Tong tien trong grdCt khong co khuyen mai
                decimal t_tien_nt2 = 0, t_tien2 = 0;
                //Tong tien trong grdct co  khuyen mai
                decimal t_tien_nt2_km = 0, t_tien2_km = 0;
                decimal thue_suat = 0;
                int TinhThueTruocCk = 0, SuaTienThue = 0;

                string stt_rec = StartUp.DsTrans.Tables[1].DefaultView[0]["stt_rec"].ToString();
                int.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["sua_thue"].ToString(), out SuaTienThue);
                int.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["thue_ck0"].ToString(), out TinhThueTruocCk);


                //if (StartUp.M_THUE_KM_CK == 1)
                //{
                //decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien_nt2)", "stt_rec= '" + stt_rec + "' and km_ck = 0").ToString(), out t_tien_nt2);
                //decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien2)", "stt_rec= '" + stt_rec + "' and km_ck = 0").ToString(), out t_tien2);
                //decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien_nt2)", "stt_rec= '" + stt_rec + "' and km_ck = 1").ToString(), out t_tien_nt2_km);
                //decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien2)", "stt_rec= '" + stt_rec + "' and km_ck = 1").ToString(), out t_tien2_km);


                decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["thue_suat"].ToString(), out thue_suat);

                decimal tien_nt2 = 0, tien2 = 0;
                decimal thue_nt = 0, thue = 0;
                decimal ck_nt = 0, ck = 0;
                decimal thue_nt_temp = 0, thue_temp = 0;
                //decimal thue_nt_temp_km = 0, thue_temp_km = 0;
                decimal ty_gia = 0, km_ck = 0;
                decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);
                int idCB = 0;
                bool isCB = false;
                for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
                {
                    decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien_nt2)", "stt_rec= '" + stt_rec + "' and km_ck = 0").ToString(), out t_tien_nt2);
                    decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien2)", "stt_rec= '" + stt_rec + "' and km_ck = 0").ToString(), out t_tien2);
                    decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien_nt2)", "stt_rec= '" + stt_rec + "' and km_ck = 1").ToString(), out t_tien_nt2_km);
                    decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien2)", "stt_rec= '" + stt_rec + "' and km_ck = 1").ToString(), out t_tien2_km);

                    decimal.TryParse(StartUp.DsTrans.Tables[1].DefaultView[i]["tien_nt2"].ToString(), out tien_nt2);
                    decimal.TryParse(StartUp.DsTrans.Tables[1].DefaultView[i]["tien2"].ToString(), out tien2);
                    decimal.TryParse(StartUp.DsTrans.Tables[1].DefaultView[i]["ck_nt"].ToString(), out ck_nt);
                    decimal.TryParse(StartUp.DsTrans.Tables[1].DefaultView[i]["ck"].ToString(), out ck);
                    decimal.TryParse(StartUp.DsTrans.Tables[1].DefaultView[i]["km_ck"].ToString(), out km_ck);

                    /*------------------Hang khong co khuyen mai-------------------*/
                    if (km_ck == 0)
                    {
                        if (!isCB)
                        {
                            idCB = i;
                            isCB = true;
                        }
                        //nếu loại tiền là ngoại tệ
                        //if (cbMa_nt.Text != StartUp.M_ma_nt0)
                        //{
                        //nếu tiền nguyên tệ = 0
                        if (tien_nt2 == 0)
                        {
                            thue_nt = 0;
                        }
                        else
                        {
                            if (TinhThueTruocCk == 1)
                            {

                            }
                            else
                            {
                                tien_nt2 = tien_nt2 - ck_nt;
                                t_tien_nt2 = t_tien_nt2 - ParseDecimal(txtT_ck_nt.Text.Trim().ToString(), 0);
                            }

                            //Neu khong check sua tien thue
                            if (SuaTienThue == 0)
                            {
                                thue_nt = t_tien_nt2 == 0 ? 0 : SmLib.SysFunc.Round(tien_nt2 * thue_suat / 100, StartUp.M_ROUND_NT);
                            }
                            else
                            {
                                thue_nt = SmLib.SysFunc.Round(tien_nt2 / t_tien_nt2 * ParseDecimal(txtT_thue_nt.Text.Trim().ToString(), 0), StartUp.M_ROUND_NT);
                            }

                        }

                        //nếu tiền ngoại tệ = 0
                        if (tien2 == 0)
                        {
                            thue = 0;
                        }
                        else
                        {
                            if (TinhThueTruocCk == 1)
                            {

                            }
                            else
                            {
                                tien2 = tien2 - ck;
                                t_tien2 = t_tien2 - ParseDecimal(txtT_ck.Text.Trim().ToString(), 0); 
                            }

                            //Neu khong check sua tien thue
                            if (SuaTienThue == 0)
                            {
                                thue = t_tien2 == 0 ? 0 : SmLib.SysFunc.Round(tien2 * thue_suat / 100, StartUp.M_ROUND);
                            }
                            else
                            {
                                thue = SmLib.SysFunc.Round(tien2 / t_tien2 * ParseDecimal(txtT_thue.Text.Trim().ToString(), 0), StartUp.M_ROUND);
                            }
                        }
                        //}
                        //else
                        //{
                        //    thue_nt = t_tien_nt2 == 0 ? 0 : SmLib.SysFunc.Round((tien_nt2 / t_tien_nt2) * t_thue_nt, StartUp.M_ROUND_NT);
                        //    thue = t_tien2 == 0 ? 0 : SmLib.SysFunc.Round((tien2 / t_tien2) * t_thue, StartUp.M_ROUND);
                        //}

                        StartUp.DsTrans.Tables[1].DefaultView[i]["thue_nt"] = thue_nt;
                        StartUp.DsTrans.Tables[1].DefaultView[i]["thue"] = thue;
                        thue_nt_temp += thue_nt;
                        thue_temp += thue;
                    }
                    else/*------------------Hang co khuyen mai-------------------*/
                    {
                        //nếu loại tiền là ngoại tệ
                        //if (cbMa_nt.Text != StartUp.M_ma_nt0)
                        //{
                        //nếu tiền nguyên tệ = 0
                        if (tien_nt2 == 0 || StartUp.M_THUE_KM_CK == 0) //Nếu không hạch toán thuế khuyến mãi thì thuế của hàng khuyến mãi trong grdct = 0
                        {
                            thue_nt = 0;
                        }
                        else
                        {
                            if (TinhThueTruocCk == 1)
                            {

                            }
                            else
                            {
                                tien_nt2 = tien_nt2 - ck_nt;
                            }

                            thue_nt = t_tien_nt2_km == 0 ? 0 : SmLib.SysFunc.Round(tien_nt2 * thue_suat / 100, StartUp.M_ROUND_NT);
                        }

                        //nếu tiền ngoại tệ = 0
                        if (tien2 == 0 || StartUp.M_THUE_KM_CK == 0)
                        {
                            thue = 0;
                        }
                        else
                        {
                            if (TinhThueTruocCk == 1)
                            {

                            }
                            else
                            {
                                tien2 = tien2 - ck;
                            }

                            thue = t_tien2_km == 0 ? 0 : SmLib.SysFunc.Round(tien2 * thue_suat / 100, StartUp.M_ROUND);
                        }
                        //}
                        //else
                        //{
                        //    thue_nt = t_tien_nt2 == 0 ? 0 : SmLib.SysFunc.Round((tien_nt2 / t_tien_nt2) * t_thue_nt, StartUp.M_ROUND_NT);
                        //    thue = t_tien2 == 0 ? 0 : SmLib.SysFunc.Round((tien2 / t_tien2) * t_thue, StartUp.M_ROUND);
                        //}

                        StartUp.DsTrans.Tables[1].DefaultView[i]["thue_nt"] = thue_nt;
                        StartUp.DsTrans.Tables[1].DefaultView[i]["thue"] = thue;
                        thue_nt_temp += thue_nt;
                        thue_temp += thue;
                    }
                }
                decimal t_thue_nt = 0, t_thue = 0;
                decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_nt"].ToString(), out t_thue_nt);
                decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue"].ToString(), out t_thue);
                StartUp.DsTrans.Tables[1].DefaultView[idCB]["thue_nt"] = decimal.Parse(StartUp.DsTrans.Tables[1].DefaultView[idCB]["thue_nt"].ToString()) + (t_thue_nt - thue_nt_temp);
                StartUp.DsTrans.Tables[1].DefaultView[idCB]["thue"] = decimal.Parse(StartUp.DsTrans.Tables[1].DefaultView[idCB]["thue"].ToString()) + (t_thue - thue_temp);

                StartUp.DsTrans.Tables[0].AcceptChanges();
                StartUp.DsTrans.Tables[1].AcceptChanges();
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

        #region CanBangTien
        public void CanBangTien()
        {
            decimal t_tien_nt2_InPH = 0, t_tien2_InPH = 0, t_tien_nt2_InGrdCT = 0, t_tien2_InGrdCT = 0, ty_gia = 1;

            StartUp.DsTrans.Tables[0].AcceptChanges();
            StartUp.DsTrans.Tables[1].AcceptChanges();
            if (StartUp.M_KM_CK == 1)
            {
                StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "km_ck = 0 AND stt_rec = '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
            }

            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt2"].ToString(), out t_tien_nt2_InPH);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien2"].ToString(), out t_tien2_InPH);
            decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien_nt2)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), out t_tien_nt2_InGrdCT);
            decimal.TryParse(StartUp.DsTrans.Tables[1].Compute("sum(tien2)", StartUp.DsTrans.Tables[1].DefaultView.RowFilter).ToString(), out t_tien2_InGrdCT);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

            //Tiền VND trong PH bằng tiền nt trong PH * tỷ giá
            StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien2"] = SmLib.SysFunc.Round(t_tien_nt2_InPH * ty_gia,StartUp.M_ROUND);
            //Lấy tổng tiền VND trong PH trừ tổng tiền VND trong GrdCT, phần còn dư gán vào dòng đầu tiên tổng tiền VND trong GrdCT
            for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
            {
                if (ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["tien_nt2"], 0) != 0)
                {
                    StartUp.DsTrans.Tables[1].DefaultView[i]["tien2"] = SmLib.SysFunc.Round(ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["tien2"], 0) + (ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien2"], 0) - t_tien2_InGrdCT),StartUp.M_ROUND);
                    break;
                }
            }

            //Tính lại tổng thanh toán
            Sum_ALL();

            if (StartUp.M_KM_CK == 1)
            {
                StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
            }
            StartUp.DsTrans.Tables[0].AcceptChanges();
            StartUp.DsTrans.Tables[1].AcceptChanges();

        }
        #endregion

        #region txtGc_thue_PreviewKeyDown
        private void txtGc_thue_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                (this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus();
                e.Handled = true;
            }
        }
        #endregion

        #region txtTk_thue_no_PreviewLostFocus
        private void txtTk_thue_no_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (IsInEditMode.Value == true)
            {
                if (txtTk_thue_no.RowResult == null || string.IsNullOrEmpty(txtTk_thue_no.Text.Trim()))
                    return;
                StartUp.DsTrans.Tables[0].DefaultView[0]["tk_thue_no_cn"] = txtTk_thue_no.RowResult["tk_cn"];
            }
        }
        #endregion

        #region txtMa_kh2_GotFocus
        private void txtMa_kh2_GotFocus(object sender, RoutedEventArgs e)
        {
            if (StartUp.DsTrans.Tables[0].DefaultView[0]["tk_thue_no_cn"].ToString() == "0")
                SmLib.WinAPISenkey.SenKey(ModifierKeys.None, Key.Tab);
        }
        #endregion

        private void txtMa_bp_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (IsInEditMode.Value == true)
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
            }
        }

        void Post()
        {
            SqlCommand PostCmd = new SqlCommand("exec [SOCTPNF-Post] @stt_rec");
            PostCmd.Parameters.Add("@stt_rec", SqlDbType.VarChar).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();
            StartUp.SysObj.ExcuteNonQuery(PostCmd);
        }
    }
}
