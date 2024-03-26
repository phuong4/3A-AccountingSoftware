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
using SmDefine;
using Infragistics.Windows.DataPresenter;
using SmVoucherLib;
using System.Data;
using Sm.Windows.Controls;
using System.Data.SqlClient;
using SmDataLib;
using Sm.Windows.Controls.ControlLib;
using SmLib;
using System.Diagnostics;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using System.Threading;

namespace ARCTHD1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FrmArcthd1 : SmVoucherLib.FormTrans
    {
        public static int iRow = 0;
        int iRow_old = 0;
        public static CodeValueBindingObject IsInEditMode;
        CodeValueBindingObject Voucher_Ma_nt0;
        CodeValueBindingObject Voucher_Lan0;
        CodeValueBindingObject IsCheckedSua_tien;
        CodeValueBindingObject IsCheckedSua_HT_Thue;
        CodeValueBindingObject IsCheckedSua_Thue;
        CodeValueBindingObject Ty_Gia_ValueChange;
        CodeValueBindingObject M_Ngay_lct;
        CodeValueBindingObject M_BP_BH;
        CodeValueBindingObject IsUseCK;

        public DataSet DsVitual;
        private DataSet dsCheckData;

        public FrmArcthd1()
        {
            InitializeComponent();
            LanguageProvider.Language = StartUp.M_LAN;
            this.BindingSysObj = StartUp.SysObj;
           
            Loaded += new RoutedEventHandler(FrmSocthda_Loaded);
            C_QS = txtMa_qs;
            C_NgayHT = txtNgay_ct;
            C_So_ct = txtSo_ct;
            C_Ma_nt = txtMa_nt;
        }

        #region FrmSocthda_Loaded
        void FrmSocthda_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                StartUp.M_AR_CK = Convert.ToInt16(BindingSysObj.GetOption(stt_mau_temlate.ToString(), "M_AR_CK"));
                if (StartUp.DsTrans.Tables[0].Rows.Count > 1)
                {
                    iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;
                }
               
                IsInEditMode = (CodeValueBindingObject)FormMain.FindResource("IsInEditMode");
                Voucher_Ma_nt0 = (CodeValueBindingObject)FormMain.FindResource("Voucher_Ma_nt0");
                Voucher_Lan0 = (CodeValueBindingObject)FormMain.FindResource("Voucher_Lan0");
                IsCheckedSua_tien = (CodeValueBindingObject)FormMain.FindResource("IsCheckedSua_tien");
                IsCheckedSua_HT_Thue = (CodeValueBindingObject)FormMain.FindResource("IsCheckedSua_HT_Thue");
                IsCheckedSua_Thue = (CodeValueBindingObject)FormMain.FindResource("IsCheckedSua_Thue");
                Ty_Gia_ValueChange = (CodeValueBindingObject)FormMain.FindResource("Ty_Gia_ValueChange");
                M_Ngay_lct = (CodeValueBindingObject)FormMain.FindResource("M_Ngay_lct");
                M_BP_BH = (CodeValueBindingObject)FormMain.FindResource("M_BP_BH");
                M_Ngay_lct.Value = StartUp.M_Ngay_lct.Equals("1");
                M_BP_BH.Value = StartUp.M_BP_BH.Equals("1");
                IsUseCK = (CodeValueBindingObject)FormMain.FindResource("IsUseCK");
                IsUseCK.Value = StartUp.M_AR_CK == 1 ? true : false;
                Binding bind = new Binding("Value");
                bind.Source = IsInEditMode;
                bind.Mode = BindingMode.TwoWay;
                this.SetBinding(FormTrans.IsEditModeProperty, bind);

                string M_CDKH13 = SysO.GetOption("M_CDKH13").ToString().Trim();
                if (M_CDKH13 != "1")
                    txtSoDuKH.Visibility = tblSoDuKH.Visibility = Visibility.Hidden;

                //Gán ngôn ngữ messagebox
                GrdCt.Lan = StartUp.M_LAN;

                M_LAN = StartUp.M_LAN;

                //Them cac truong tu do
                SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, GrdCt, StartUp.Ma_ct, 1);

                //load form theo stt_rec
                if (StartUp.DsTrans.Tables[0].Rows.Count > 0)
                {
                    StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString());
                    LoadData();
                    LoadDataDu13();
                    StartUp.DsTrans.Tables[0].DefaultView.ListChanged += new System.ComponentModel.ListChangedEventHandler(DefaultView_ListChanged);
                    IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
                    IsCheckedSua_tien.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"].ToString() == "1");
                    Ty_Gia_ValueChange.Value = false;
                    Voucher_Lan0.Value = M_LAN.Trim().Equals("V");
                }
                TabInfo.SelectedIndex = 0;
                ExSetFocusToolBar();
       
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        #region LoadData
        void LoadData()
        {

            GrdLayout00.DataContext = StartUp.DsTrans.Tables[0].DefaultView;

            //grid Hạch toán
            this.GrdCt.DataSource = StartUp.DsTrans.Tables[1].DefaultView;

            

            //trạng thái
            txtStatus.ItemsSource = StartUp.tbStatus.DefaultView;
            if (StartUp.tbStatus.DefaultView.Count == 1)
            {
                txtStatus.IsEnabled = false;
            }
        }
        #endregion
        #endregion

        public void ExSetFocusToolBar()
        {
            try
            {
                //if (StartUp.DsTrans.Tables[0].Rows.Count > 1)
                //{
                //    SmVoucherLib.ToolBarButton btnEdit = Toolbar.FindName("btnEdit") as SmVoucherLib.ToolBarButton;
                //    Action aMoi = () => btnEdit.Focus();
                //    btnEdit.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, aMoi);
                //}
                //else
                //{
                    SmVoucherLib.ToolBarButton btnMoi = Toolbar.FindName("btnNew") as SmVoucherLib.ToolBarButton;
                    Action aMoi = () => btnMoi.Focus();
                    btnMoi.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, aMoi);
                //}
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        #region DefaultView_ListChanged
        void DefaultView_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
        }
        #endregion

    

        #region IsVisibilityFieldsXamDataGrid
        void IsVisibilityFieldsXamDataGrid(string ma_nt)
        {
            IsVisibilityFieldsXamDataGridByMa_NT(ma_nt);
            IsVisibilityFieldsXamDataGridBySua_Tien();


        }
        #region IsVisibilityFieldsXamDataGridByMa_NT
        void IsVisibilityFieldsXamDataGridByMa_NT(string ma_nt)
        {
            //Nếu ngoại tệ = tiền hoạch toán
            if (ma_nt == StartUp.M_ma_nt0)
            {
               // txtTy_gia.IsReadOnly = true;
                //GrdCt không hiển thị ps_no, ps_co
                GrdCt.FieldLayouts[0].Fields["tien2"].Visibility = Visibility.Hidden;
                GrdCt.FieldLayouts[0].Fields["gia2"].Visibility = Visibility.Hidden;
                GrdCt.FieldLayouts[0].Fields["ck"].Visibility = Visibility.Hidden;
                GrdCt.FieldLayouts[0].Fields["thue"].Visibility = Visibility.Hidden;

                GrdCt.FieldLayouts[0].Fields["tien2"].Settings.CellMaxWidth = 0;
                GrdCt.FieldLayouts[0].Fields["gia2"].Settings.CellMaxWidth = 0;
                GrdCt.FieldLayouts[0].Fields["ck"].Settings.CellMaxWidth = 0;
                GrdCt.FieldLayouts[0].Fields["thue"].Settings.CellMaxWidth = 0;
            }
            else
            {
                //GrdCt hiển thị ps_no, ps_co
                GrdCt.FieldLayouts[0].Fields["tien2"].Visibility = Visibility.Visible;
                GrdCt.FieldLayouts[0].Fields["gia2"].Visibility = Visibility.Visible;
                if (StartUp.M_AR_CK == 1)
                    GrdCt.FieldLayouts[0].Fields["ck"].Visibility = Visibility.Visible;
                GrdCt.FieldLayouts[0].Fields["thue"].Visibility = Visibility.Visible;

                GrdCt.FieldLayouts[0].Fields["tien2"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["tien2"].Width.Value.Value;
                GrdCt.FieldLayouts[0].Fields["gia2"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["gia2"].Width.Value.Value;
                if (StartUp.M_AR_CK == 1)
                    GrdCt.FieldLayouts[0].Fields["ck"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["ck"].Width.Value.Value;
                GrdCt.FieldLayouts[0].Fields["thue"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["thue"].Width.Value.Value;

                
            }
            //if (StartUp.M_AR_CK == 0)
            //{
            //    GrdCt.FieldLayouts[0].Fields["tl_ck"].Visibility = Visibility.Hidden;
            //    GrdCt.FieldLayouts[0].Fields["ck_nt"].Visibility = Visibility.Hidden;
            //    GrdCt.FieldLayouts[0].Fields["tk_ck"].Visibility = Visibility.Hidden;
            //    GrdCt.FieldLayouts[0].Fields["ck"].Visibility = Visibility.Hidden;

            //    GrdCt.FieldLayouts[0].Fields["tl_ck"].Settings.CellMaxWidth = 0;
            //    GrdCt.FieldLayouts[0].Fields["ck_nt"].Settings.CellMaxWidth = 0;
            //    GrdCt.FieldLayouts[0].Fields["tk_ck"].Settings.CellMaxWidth = 0;
            //    GrdCt.FieldLayouts[0].Fields["ck"].Settings.CellMaxWidth = 0;
            //}
            Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
            Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
            ChangeLanguage();
           

        }
        #endregion

        #region IsVisibilityFieldsXamDataGridBySua_Tien
        void IsVisibilityFieldsXamDataGridBySua_Tien()
        {
            IsCheckedSua_tien.Value = Chksua_tien.IsChecked.Value;
        }
        #endregion
        #endregion

        #region ParseDecimal
        public decimal ParseDecimal(object obj, decimal defaultvalue)
        {
            decimal ketqua = defaultvalue;
            decimal.TryParse(obj != null ? obj.ToString() : defaultvalue.ToString(), out ketqua);
            return ketqua;
        }
        #endregion

       

        #region LoadDataDu13
        private void LoadDataDu13()
        {
            txtSoDuKH.Value = ArapLib.ArFuncLib.GetSdkh13(StartUp.SysObj, StartUp.DsTrans.Tables[0].DefaultView[0]["ma_kh"].ToString(), StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nx"].ToString()) ;
        }
        #endregion

        #region txtMa_kh_PreviewLostFocus
        private bool txtDiaChiFocusable = true;
        private void txtMa_kh_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

            if (IsInEditMode.Value == true)
            {
                if (txtMa_kh.RowResult == null || string.IsNullOrEmpty(txtMa_kh.Text.Trim()))
                    return;
                
                if (txtMa_kh.IsDataChanged)
                {
                    if (M_LAN.ToUpper().Equals("V"))
                        StartUp.DsTrans.Tables[0].DefaultView[0]["ten_kh"] = txtMa_kh.RowResult["ten_kh"].ToString().Trim();
                    else
                        StartUp.DsTrans.Tables[0].DefaultView[0]["ten_kh2"] = txtMa_kh.RowResult["ten_kh2"].ToString().Trim();
                    if (e != null)
                        StartUp.DsTrans.Tables[0].DefaultView[0]["ma_so_thue"] = txtMa_kh.RowResult["ma_so_thue"].ToString().Trim();

                    if (txtMa_kh.RowResult["tk"].ToString().Trim() != "")
                        StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nx"] = txtMa_kh.RowResult["tk"].ToString().Trim();

                    StartUp.DsTrans.Tables[0].DefaultView[0]["ma_thck"] = txtMa_kh.RowResult["ma_thck"].ToString().Trim();
                    if (txtMa_kh.RowResult["han_tt"] != null && !String.IsNullOrEmpty(txtMa_kh.RowResult["han_tt"].ToString()))
                        txtHan_tt.Text = txtMa_kh.RowResult["han_tt"].ToString();

                    LoadDataDu13();
                }

                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ong_ba"].ToString().Trim()))
                    StartUp.DsTrans.Tables[0].DefaultView[0]["ong_ba"] = txtMa_kh.RowResult["doi_tac"].ToString().Trim();


                if (string.IsNullOrEmpty(txtMa_kh.RowResult["dia_chi"].ToString().Trim()))
                {
                    //134094579 Lien yeu cau de lai dia chi
                    //StartUp.DsTrans.Tables[0].DefaultView[0]["dia_chi"] = "";
                    txtDiaChiFocusable = true;
                }
                else
                {
                    StartUp.DsTrans.Tables[0].DefaultView[0]["dia_chi"] = txtMa_kh.RowResult["dia_chi"].ToString().Trim();
                    txtDiaChiFocusable = false;
                }

                StartUp.DsTrans.Tables[0].DefaultView[0]["tk_nh"] = txtMa_kh.RowResult["tk_nh"].ToString().Trim();
            }
        }
        #endregion

        #region txtDia_chi_GotFocus
        private void txtDia_chi_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!txtDiaChiFocusable)
            {
                if (Keyboard.IsKeyDown(Key.Tab) && Keyboard.Modifiers == ModifierKeys.Shift)
                    SmLib.WinAPISenkey.SenKey(ModifierKeys.Shift, Key.Tab);
                else
                    SmLib.WinAPISenkey.SenKey(ModifierKeys.None, Key.Tab);
            }
        }
         #endregion

        #region txtMa_nx_PreviewLostFocus
        private void txtMa_nx_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMa_nx.Text.Trim()))
            {
                if (M_LAN.ToUpper().Equals("V"))
                    StartUp.DsTrans.Tables[0].DefaultView[0]["ten_nx"] = txtMa_nx.RowResult["ten_nx"].ToString();
                else
                {
                    StartUp.DsTrans.Tables[0].DefaultView[0]["ten_nx2"] = txtMa_nx.RowResult["ten_nx2"].ToString();
                }

                if (Chksua_tk_thue.IsChecked.Value != true)
                {
                    txtTk_du_voi_Tk_thue.Text = txtMa_nx.Text;
                }
            }
            LoadDataDu13();
        }
        #endregion

        #region txtNgay_ct_LostFocus

        private void txtNgay_ct_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtNgay_ct.Value == DBNull.Value)
                txtNgay_ct.Value = DateTime.Now;
            if (!txtNgay_ct.IsFocusWithin)
                if ((currActionTask == ActionTask.Add || currActionTask == ActionTask.Edit || currActionTask == ActionTask.Copy))
                {
                    if ((StartUp.M_Ngay_lct.Equals("0") || txtNgay_lhd.dValue == new DateTime()) && txtNgay_ct.dValue != new DateTime())
                        txtNgay_lhd.Value = txtNgay_ct.dValue.Date;
                }
        }
        #endregion

        #region txtNgay_lhd_LostFocus
        private void txtNgay_lhd_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!txtNgay_lhd.IsFocusWithin && IsInEditMode.Value)
            {
                if (txtNgay_ct.Value.ToString() != txtNgay_lhd.Value.ToString())
                {
                    ExMessageBox.Show( 225,StartUp.SysObj, "Ngày lập chứng từ khác với ngày hạch toán!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        #endregion

        #region txtMa_qs_PreviewLostFocus
        private void txtMa_qs_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!IsInEditMode.Value)
                return;
            if (!string.IsNullOrEmpty(txtMa_qs.RowResult["so_seri"].ToString().Trim()))
                StartUp.DsTrans.Tables[0].DefaultView[0]["so_seri"] = txtMa_qs.RowResult["so_seri"].ToString().Trim();
            if (!string.IsNullOrEmpty(txtMa_qs.RowResult["so_ct1"].ToString()))
                StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct1"] = txtMa_qs.RowResult["so_ct1"].ToString();
            if (!string.IsNullOrEmpty(txtMa_qs.RowResult["so_ct2"].ToString()))
                StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct2"] = txtMa_qs.RowResult["so_ct2"].ToString();
            if (!string.IsNullOrEmpty(txtMa_qs.RowResult["transform"].ToString()))
                StartUp.DsTrans.Tables[0].DefaultView[0]["transform"] = txtMa_qs.RowResult["transform"].ToString();

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background
                    , new Action(() =>
                    {
                        if (IsInEditMode.Value && !e.NewFocus.GetType().Equals(typeof(SmVoucherLib.ToolBarButton)))
                            if (!string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_qs"].ToString()))
                            {
                                //if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["so_seri"].ToString().Trim()) && txtMa_qs.RowResult != null)
                                //{
                                //    txtSo_seri.Text = txtMa_qs.RowResult["so_seri"].ToString();
                                //}

                                if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"].ToString().Trim()) || (IsNd51 && txtMa_qs.IsDataChanged))
                                {
                                    if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["so_cttmp"].ToString().Trim()) || !StartUp.DsTrans.Tables[0].DefaultView[0]["ma_qs"].ToString().Trim().Equals(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_qstmp"].ToString().Trim()) || IsNd51)
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

                                if (!string.IsNullOrEmpty(txtMa_qs.RowResult["so_lien_hd"].ToString()))
                                    StartUp.DsTrans.Tables[0].DefaultView[0]["so_lien_hd"] = txtMa_qs.RowResult["so_lien_hd"];
                                if (!string.IsNullOrEmpty(txtMa_qs.RowResult["ten_lien1"].ToString()))
                                    StartUp.DsTrans.Tables[0].DefaultView[0]["ten_lien1"] = txtMa_qs.RowResult["ten_lien1"].ToString();
                                if (!string.IsNullOrEmpty(txtMa_qs.RowResult["ten_lien2"].ToString()))
                                    StartUp.DsTrans.Tables[0].DefaultView[0]["ten_lien2"] = txtMa_qs.RowResult["ten_lien2"].ToString();
                                if (!string.IsNullOrEmpty(txtMa_qs.RowResult["ten_lien3"].ToString()))
                                    StartUp.DsTrans.Tables[0].DefaultView[0]["ten_lien3"] = txtMa_qs.RowResult["ten_lien3"].ToString();
                                if (!string.IsNullOrEmpty(txtMa_qs.RowResult["ten_lien4"].ToString()))
                                    StartUp.DsTrans.Tables[0].DefaultView[0]["ten_lien4"] = txtMa_qs.RowResult["ten_lien4"].ToString();
                                if (!string.IsNullOrEmpty(txtMa_qs.RowResult["ten_lien5"].ToString()))
                                    StartUp.DsTrans.Tables[0].DefaultView[0]["ten_lien5"] = txtMa_qs.RowResult["ten_lien5"].ToString();
                                if (!string.IsNullOrEmpty(txtMa_qs.RowResult["ten_lien6"].ToString()))
                                    StartUp.DsTrans.Tables[0].DefaultView[0]["ten_lien6"] = txtMa_qs.RowResult["ten_lien6"].ToString();
                                if (!string.IsNullOrEmpty(txtMa_qs.RowResult["ten_lien7"].ToString()))
                                    StartUp.DsTrans.Tables[0].DefaultView[0]["ten_lien7"] = txtMa_qs.RowResult["ten_lien7"].ToString();
                                if (!string.IsNullOrEmpty(txtMa_qs.RowResult["ten_lien8"].ToString()))
                                    StartUp.DsTrans.Tables[0].DefaultView[0]["ten_lien8"] = txtMa_qs.RowResult["ten_lien8"].ToString();
                                if (!string.IsNullOrEmpty(txtMa_qs.RowResult["ten_lien9"].ToString()))
                                    StartUp.DsTrans.Tables[0].DefaultView[0]["ten_lien9"] = txtMa_qs.RowResult["ten_lien9"].ToString();

                                if (!string.IsNullOrEmpty(txtMa_qs.RowResult["ten_dn_in"].ToString()))
                                    StartUp.DsTrans.Tables[0].DefaultView[0]["ten_dn_in"] = txtMa_qs.RowResult["ten_dn_in"].ToString();
                                if (!string.IsNullOrEmpty(txtMa_qs.RowResult["mst_dn_in"].ToString()))
                                    StartUp.DsTrans.Tables[0].DefaultView[0]["mst_dn_in"] = txtMa_qs.RowResult["mst_dn_in"].ToString();

                                if (!string.IsNullOrEmpty(txtMa_qs.RowResult["mau_hd"].ToString()))
                                    StartUp.DsTrans.Tables[0].DefaultView[0]["mau_hd"] = txtMa_qs.RowResult["mau_hd"].ToString();
                                if (!string.IsNullOrEmpty(txtMa_qs.RowResult["ma_file"].ToString()))
                                    StartUp.DsTrans.Tables[0].DefaultView[0]["ma_file"] = txtMa_qs.RowResult["ma_file"].ToString();
                            }
                    }));
        }
        #endregion

        #region Mã ngoại tệ lostfocus
        private void txtMa_nt_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Voucher_Ma_nt0 == null)
                return;

            if (txtMa_nt.IsDataChanged)
            {
                StartUp.DsTrans.Tables[0].DefaultView[0]["loai_tg"] = txtMa_nt.RowResult["loai_tg"];
                IsVisibilityFieldsXamDataGridByMa_NT(txtMa_nt.Text.Trim());
                if (txtMa_nt.RowResult != null)
                {
                    if (txtMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
                    {
                        txtTy_gia.Value = 1;
                    }
                    else
                    {
                        txtTy_gia.Value = StartUp.GetRates(txtMa_nt.Text.Trim(), Convert.ToDateTime(txtNgay_ct.Value).Date);
                    }
                }
                CalculateTyGia();
                // txtTy_gia.Style.Triggers[0].InvalidateProperty(ExRateTextBox.IsReadOnlyProperty);
            }
        }

        #endregion

        #region Tỷ giá
        private void txtTy_gia_LostFocus(object sender, RoutedEventArgs e)
        {
            if (currActionTask == ActionTask.Delete || currActionTask == ActionTask.View)
                return;
            if (txtTy_gia.Value == DBNull.Value)
            {
                txtTy_gia.Value = 0;
            }
            if (txtTy_gia.OldValue != txtTy_gia.nValue)
            {
                CalculateTyGia();
                Ty_Gia_ValueChange.Value = !Ty_Gia_ValueChange.Value;
            }
        }

        private void CalculateTyGia()
        {
            decimal _ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);
            int sua_tien = 0;
            int.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"].ToString(), out sua_tien);
            //Hạch toán thay đổi
            if (_ty_gia != 0)
            {
                UpdateTotal(StartUp.DsTrans.Tables[1].DefaultView, "tien2", "tien_nt2");
                UpdateTotal(StartUp.DsTrans.Tables[1].DefaultView, "gia2", "gia_nt2");
                UpdateTotal(StartUp.DsTrans.Tables[1].DefaultView, "thue", "thue_nt");
                UpdateTotal(StartUp.DsTrans.Tables[1].DefaultView, "ck", "ck_nt");
                //Tính lại ck,thue
                decimal ck = 0, /*tl_ck = 0, tien = 0, thue = 0,gia=0,*/ thue_suat = 0, ck_nt = 0;
                
                foreach (DataRowView drv in StartUp.DsTrans.Tables[1].DefaultView)
                {
                    //decimal.TryParse(drv.Row["tl_ck"].ToString(), out tl_ck);
                    //decimal.TryParse(drv.Row["tien2"].ToString(), out tien);
                    //decimal.TryParse(drv.Row["gia2"].ToString(), out gia);     
                    decimal.TryParse(drv.Row["thue_suati"].ToString(), out thue_suat);                    
                    //ck = SysFunc.Round(tien * tl_ck / 100, StartUp.M_ROUND);
                    decimal.TryParse(drv.Row["ck_nt"].ToString(), out ck_nt);

                    
                    ck = SysFunc.Round(ck_nt * _ty_gia, StartUp.M_ROUND);
                    drv.Row["ck"] = ck;
                    ///chị nhung bảo không tính lại thuế khi bỏ sửa trường tiền 125842889
                    /*
                    thue = TinhThue(tien, ck, thue_suat, Chkthue_ck0.IsChecked.Value);
                    drv.Row["thue"] = thue;
                    */
                }

                //UpdateTotal(StartUp.DsTrans.Tables[1].DefaultView, "thue", "thue_nt");
                UpdateTotalHT();
            }
        }
        #endregion

        #region UpdateTotal
        void UpdateTotal(DataView dtview, string columnname, string columnname_nt)//, decimal t_tien)
        {
            decimal tien = 0, tien_nt = 0;
            decimal ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);
            if (dtview.Count > 0)
            {
                foreach (DataRowView drv in dtview)
                {
                    decimal.TryParse(drv.Row[columnname_nt].ToString(), out tien_nt);
                    tien = SysFunc.Round(tien_nt * ty_gia, StartUp.M_ROUND);
                    drv.Row[columnname] = tien;
                }
            }
        }
        #endregion

        #region txtTy_gia_GotFocus
        private void txtTy_gia_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Voucher_Ma_nt0.Value)
            {
                KeyboardNavigation.SetTabNavigation(GrNT, KeyboardNavigationMode.Continue);
                SmLib.WinAPISenkey.SenKey(ModifierKeys.None, Key.Enter);
            }
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
        #region V_Dau
        private void V_Dau()
        {
            if (StartUp.DsTrans.Tables[0].Rows.Count >= 2)
            {
                iRow = 1;
            }
            else
                iRow = 0;
            StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
            StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";

            Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
            Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
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
                    DsVitual = StartUp.DsTrans.Copy();


                    //Them moi dong trong Ph
                    DataRow NewRecord = StartUp.DsTrans.Tables[0].NewRow();
                    NewRecord["stt_rec"] = newSttRec;
                    NewRecord["ma_ct"] = StartUp.Ma_ct;


                    if (SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, txtNgay_ct.dValue))
                    {
                        NewRecord["ngay_ct"] = txtNgay_ct.dValue.Date;
                        //NewRecord["ngay_lct"] = txtngay_lct.dValue.Date;
                    }
                    else
                    {
                        NewRecord["ngay_ct"] = DateTime.Now.Date;
                        //NewRecord["ngay_lct"] = DateTime.Now.Date;
                    }

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
                   // NewRecord["ma_nt"] = StartUp.DsTrans.Tables[0].Rows.Count > 1 ? StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString() : StartUp.DmctInfo["ma_nt"].ToString();
                    if (NewRecord["ma_nt"].ToString().Trim().Equals(StartUp.M_ma_nt0.Trim()))
                    {
                        NewRecord["ty_giaf"] = 1;
                    }
                    else
                    {
                        NewRecord["ty_giaf"] = StartUp.GetRates(NewRecord["ma_nt"].ToString().Trim(), Convert.ToDateTime(NewRecord["ngay_ct"]).Date);
                    }

                    NewRecord["status"] = StartUp.DmctInfo["ma_post"];
                    NewRecord["sl_in"] = 0;
                    //NewRecord["t_tien_nt"] = 0;
                    //NewRecord["t_tien"] = 0;
                    //NewRecord["t_thue"] = 0;
                    NewRecord["t_thue_nt"] = 0;
                    
                    NewRecord["t_tt_nt"] = 0;
               
                    NewRecord["ma_dvcs"] = StartUp.SysObj.M_ma_dvcs;
                    //Lỗi 1048195                         
                    NewRecord["so_seri"] = StartUp.DsTrans.Tables[0].DefaultView[0]["so_seri"];
                    NewRecord["so_dh"] = String.Empty;
                    NewRecord["so_lo"] = String.Empty;

                    NewRecord["dia_chi"] = String.Empty;
                    NewRecord["ma_vv"] = String.Empty;

                    NewRecord["tk_ck"] = String.Empty;
                    NewRecord["t_ck_nt"] = 0;
                    NewRecord["han_tt"] = 0;
                    NewRecord["dien_giai"] = String.Empty;
                    //NewRecord["t_tt"] = 0;
                    NewRecord["tien_hg"] = 0;
                    NewRecord["tien_hg_nt"] = 0;
                    NewRecord["t_tien2"] = 0;
                    NewRecord["t_tien_nt2"] = 0;
                    NewRecord["sua_thue"] = 0;
                    NewRecord["sua_tkthue"] = 0;
                    NewRecord["sua_tien"] = 0;
                    //Them moi dong trong Ct
                    DataRow NewCtRecord = StartUp.DsTrans.Tables[1].NewRow();
                    NewCtRecord["stt_rec"] = newSttRec;
                    NewCtRecord["stt_rec0"] = "001";
                    NewCtRecord["ma_ct"] = StartUp.Ma_ct;
                    NewCtRecord["ngay_ct"] = txtNgay_ct.Value == null ? DateTime.Now.Date : txtNgay_ct.dValue.Date;
                    NewCtRecord["tien_nt2"] = 0;
                    NewCtRecord["tien2"] = 0;
                    NewCtRecord["gia_nt2"] = 0;
                    NewCtRecord["gia2"] = 0;
                    NewCtRecord["so_luong"] = 0;
                    NewCtRecord["tl_ck"] = 0;
                    NewCtRecord["ck_nt"] = 0;
                    NewCtRecord["thue_suati"] = 0;
                    NewCtRecord["ma_kh2_i"] = String.Empty;
                    NewCtRecord["thue_nt"] = 0;
                    NewCtRecord["ck"] = 0;
                    NewCtRecord["thue"] = 0;
              
                    StartUp.DsTrans.Tables[0].Rows.Add(NewRecord);
                    StartUp.DsTrans.Tables[1].Rows.Add(NewCtRecord);

                    //if (StartUp.DsTrans.Tables[1].Rows.Count == 1)
                    //{
                    //   LoadData();
                    //    (TabInfo.Items[0] as TabItem).Focus();
                    //}
                    //Nhảy đến phiếu vừa thêm
                    StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";
                    StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";
                    //StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";

                    iRow_old = iRow;
                    iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;
                    IsInEditMode.Value = true;
                    txtSoDuKH.Text = "";
                    txtTen_kh.Text = "";
                    //txtTenTK.Text = "";
                    TabInfo.SelectedIndex = 0;
                    Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background
                    , new Action(() =>
                    {
                        txtMa_kh.IsFocus = true;
                    }));
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
            if (StartUp.DsTrans.Tables[0].Rows.Count == 0)
                ExMessageBox.Show( 230,StartUp.SysObj, "Không có dữ liệu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                currActionTask = ActionTask.Edit;
                DsVitual = new DataSet();
                //copy Table[0], Table[1]
                DsVitual.Tables.Add(StartUp.DsTrans.Tables[0].DefaultView.ToTable());

                DsVitual.Tables.Add(StartUp.DsTrans.Tables[1].DefaultView.ToTable());

                IsInEditMode.Value = true;

                IsVisibilityFieldsXamDataGridBySua_Tien();
                TabInfo.SelectedIndex = 0;

                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background
                , new Action(() =>
                {
                    txtMa_kh.IsFocus = true;
                }));
            }
            txtMa_kh.SearchInit();
            txtMa_kh_PreviewLostFocus(null, null);
        }
        #endregion

        #region V_Copy
        private void V_Copy()
        {
            if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim()))
                return;
            currActionTask = ActionTask.Copy;
            FrmARCTHD1Copy _formcopy = new FrmARCTHD1Copy();
            _formcopy.Closed += new EventHandler(_formcopy_Closed);
            _formcopy.ShowDialog();
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background
            , new Action(() =>
            {
                txtMa_kh.IsFocus = true;
            }));
        }
        void _formcopy_Closed(object sender, EventArgs e)
        {
            if ((sender as FrmARCTHD1Copy).isCopy == true)
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
                    NewRecord["stt_rec_pt"] = "";
                    NewRecord["so_ct_pt"] = "";
                    NewRecord["ma_ct_pt"] = "";
                    NewRecord["ngay_ct"] = FrmARCTHD1Copy.ngay_ct;
                    NewRecord["status"] = StartUp.DmctInfo["ma_post"];
                    NewRecord["ten_post"] = StartUp.tbStatus.Select("ma_post =" + StartUp.DmctInfo["ma_post"].ToString())[0]["ten_post"];
                    if (StartUp.M_Ngay_lct.Trim().Equals("0"))
                    {
                        NewRecord["ngay_lct"] = FrmARCTHD1Copy.ngay_ct;
                    }

                    NewRecord["ma_qs"] = GetDMQS(BindingSysObj, StartUp.Ma_ct, Convert.ToDateTime(NewRecord["ngay_ct"]),
                             StartUp.M_User_Id, NewRecord["ma_qs"].ToString().Trim());
                    if (NewRecord["ma_qs"].ToString().Trim() != "")
                        NewRecord["so_ct"] = GetNewSoct(StartUp.SysObj, NewRecord["ma_qs"].ToString());
                    else
                        NewRecord["so_ct"] = "";

                    NewRecord["so_cttmp"] = NewRecord["so_ct"];
                    NewRecord["sl_in"] = 0;
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

                    iRow_old = iRow;
                    iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;
                    //load lại form
                    StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";
                    StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";

                    IsInEditMode.Value = true;
                    IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
                    //IsVisibilityFieldsXamDataGrid
                    //SetStatusVisibleField();
              
                }
            }
        } 
        #endregion

        #region V_Xoa
        private void V_Xoa()
        {
            if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim()))
                return;

            if (currActionTask == ActionTask.None || currActionTask == ActionTask.View)
                currActionTask = ActionTask.Delete;
            try
            {
                string _stt_rec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();
                if (!string.IsNullOrEmpty(StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"].ToString().Trim()))
                    if (ExMessageBox.Show(391, StartUp.SysObj, "Hóa đơn đã được thanh toán, có muốn xóa phiếu thanh toán hay không?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                    {
                        StartUp.DeletePT(StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"].ToString().Trim(), StartUpTrans.DsTrans.Tables[0].DefaultView[0]["ma_ct_pt"].ToString().Trim());
                    }

                //Delete tksd13
                StartUpTrans.UpdateTkSd13(1, 0);
                
                //xóa trong ph, ct, ctgt
                //xóa chứng từ
                StartUp.DeleteVoucher(_stt_rec, txtMa_qs.Text, currActionTask, IsNd51);

                // ----Warning : Không nên xóa Table[0] trước, nếu xóa trước sẽ bị mất Binding -----------------------
                // Nên dịch chuyển iRow lùi 1 dòng
                // Sau đó RowFilter lại Table[0], Table[1], Table[2]
                // Rồi mới xóa Table[0]
                //iRow = iRow > 0 ? iRow - 1 : 0;
                StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString() + "'";
                StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString() + "'";

                //Xóa row table[0]
                StartUp.DsTrans.Tables[0].Rows.RemoveAt(iRow);

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

        #region V_Huy
        private void V_Huy()
        {
            IsInEditMode.Value = false;

            //Refresh lại khi chọn edit
            switch (currActionTask)
            {
                case ActionTask.Edit:
                    {
                        //xóa các row trong table[1], table[2]
                        string stt_rec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();
                        //Refresh lại grid chi phi
                        if (StartUp.DsTrans.Tables[1].DefaultView.Count > 0)
                        {
                            //lấy các rowfilter trong grdcp
                            DataRow[] _row = StartUp.DsTrans.Tables[1].Select("stt_rec='" + stt_rec + "'");
                            foreach (DataRow dr in _row)
                            {
                                //delete các row có trong grdcp
                                StartUp.DsTrans.Tables[1].Rows.Remove(dr);
                            }
                        }

                       

                        //Refresh lại table[0]
                        StartUp.DsTrans.Tables[0].Rows[iRow].ItemArray = DsVitual.Tables[0].Rows[0].ItemArray;

                        StartUp.DsTrans.Tables[1].Merge(DsVitual.Tables[1]);
                    }
                    IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());

                    break;
                //Refresh lại khi chọn new
                case ActionTask.Add:
                case ActionTask.Copy:
                    {
                        V_Xoa();
                        if (StartUp.DsTrans.Tables[0].Rows.Count > 0)
                        {
                            iRow = iRow_old;
                            //load lại form theo stt_rec
                            StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString());
                        }
                    }
                    break;

            }
            //txtDia_chi.IsReadOnly = true;
            TabInfo.SelectedIndex = 0;
            try
            {
                txtMa_kh.IsFocus = true;
            }
            catch{}
            currActionTask = ActionTask.None;

        }
        #endregion

        #region Cm_HuyHD
        private void V_HuyHD()
        {
            try
            {
                bool isError = false;

                //Kiem tra Qs
                if (C_QS != null && C_NgayHT != null)
                {
                    int _resultCheck = StartUpTrans.CheckQS(C_QS.Text, C_NgayHT.dValue.ToString("yyyyMMdd"), Convert.ToInt16(SysO.UserInfo.Rows[0]["user_id"].ToString()));
                    if (_resultCheck == 1)
                    {
                        ExMessageBox.Show( 235,SysO, "Ngày bắt đầu sử dụng quyển sổ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        isError = true;
                    }
                    else if (_resultCheck == 2)
                    {
                        ExMessageBox.Show( 240,SysO, "Quyền sử dụng quyển sổ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        isError = true;
                    }
                }
                if (!isError)
                {
                    FrmLogin _frmIn = new FrmLogin();
                    _frmIn.ShowDialog();
                    if (_frmIn.IsLogined)
                    {
                        //Lưu
                        StartUpTrans.DsTrans.Tables[0].DefaultView[0]["status"] = 3;
                        DataTable tbPhToSave = StartUp.DsTrans.Tables[0].Clone();
                        tbPhToSave.Rows.Add(StartUp.DsTrans.Tables[0].DefaultView[0].Row.ItemArray);
                        DataProvider.UpdateDataTable(StartUp.SysObj, StartUp.DmctInfo["m_phdbf"].ToString(), "stt_rec", tbPhToSave, "stt_rec;row_id");

                        DataTable tbCtToSave = StartUp.DsTrans.Tables[1].Clone();
                        foreach (DataRowView drv in StartUp.DsTrans.Tables[1].DefaultView)
                        {
                            // update thông tin cho các record Table1 (Ct) 
                            drv["ngay_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ngay_ct"];
                            drv["so_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"];
                            drv["ma_ct"] = StartUp.Ma_ct;
                            tbCtToSave.Rows.Add(drv.Row.ItemArray);
                        }

                        if (!DataProvider.UpdateCtTable(StartUp.SysObj, StartUp.DmctInfo["m_ctdbf"].ToString(), tbCtToSave, StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()))
                        {
                            ExMessageBox.Show( 245,StartUp.SysObj, "Lưu không thành công, kiểm tra lại dữ liệu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }


                        ThreadStart _thread = delegate()
                        {
                            Post(0);
                        };

                        (new Thread(_thread)).Start();
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
                bool isError = false;
                if (!IsSequenceSave)
                {
                    StartUp.DsTrans.Tables[1].AcceptChanges();
                    //StartUp.DsTrans.Tables[2].AcceptChanges();
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
                    if (GrdCt.Records.Count == 0)
                    {
                        ExMessageBox.Show( 250,StartUp.SysObj, "Chưa vào tài khoản nợ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        SmLib.WinAPISenkey.SenKey(ModifierKeys.Alt, Key.D1);
                        return;
                    }
                    //Load data cho dmqs
                    if (!isError)
                    {
                        txtMa_qs.SearchInit();
                    }

                    //Kiểm tra mã khách trong ph có chưa
                    if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_kh"].ToString().Trim()))
                    {
                        ExMessageBox.Show( 255,StartUp.SysObj, "Chưa có mã khách hàng!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtMa_kh.IsFocus = true;
                        isError = true;
                    }
                    //Kiểm tra tk có trong ph có chưa
                    else if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nx"].ToString().Trim()))
                    {
                        ExMessageBox.Show( 260,StartUp.SysObj, "Chưa vào tài khoản có!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtMa_nx.IsFocus = true;
                        isError = true;
                    }
                    //Kiểm tra có ngày hạch toán hay chưa
                    else if (txtNgay_ct.dValue == new DateTime())
                    {
                        ExMessageBox.Show( 265,StartUp.SysObj, "Chưa vào ngày hạch toán!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtNgay_ct.Focus();
                        isError = true;
                    }
                        else

                        if ( StartUp.M_NGAY_BAT_DAU != null && (!txtNgay_ct.IsValueValid || txtNgay_ct.dValue < StartUp.M_NGAY_BAT_DAU || txtNgay_ct.dValue > StartUp.M_NGAY_KET_THUC))
                        {
                            ExMessageBox.Show(1024, StartUp.SysObj, "Ngày hạch toán không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            isError = true;
                            txtNgay_ct.Focus();
                        }
                    else if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[0]["tk_dt"].ToString().Trim()))
                    {
                        ExMessageBox.Show( 270,StartUp.SysObj, "Chưa vào tài khoản doanh thu!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        TabInfo.SelectedIndex = 0;
                        GrdCt.ExecuteCommand(DataPresenterCommands.CellFirstOverall);
                        GrdCt.Focus();
                        isError = true;
                    }

                    //Kiểm tra số chứng từ
                    if (!IsNd51)
                    {
                        if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"].ToString().Trim()))
                        {
                            ExMessageBox.Show( 275,StartUp.SysObj, "Chưa vào số chứng từ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtSo_ct.Focus();
                            isError = true;
                        }
                        //Kiểm tra so_ct có thuộc quyển ct không
                        else if (!CheckSo_ct(txtMa_qs.RowResult["transform"].ToString(), Convert.ToDecimal(txtMa_qs.RowResult["so_ct1"].ToString()), Convert.ToDecimal(txtMa_qs.RowResult["so_ct2"].ToString()), txtSo_ct.Text.Trim()))
                        {
                            ExMessageBox.Show( 280,StartUp.SysObj, "Hóa đơn không thuộc quyển chứng từ hiện hành!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtSo_ct.Focus();
                            isError = true;
                        }
                        //else if (CheckValidSoct(StartUp.SysObj, txtMa_qs.Text, txtSo_ct.Text, StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()))
                        //{
                        //    if (StartUp.M_trung_so.Equals("1"))
                        //    {
                        //        if (ExMessageBox.Show( 285,StartUp.SysObj, "Có chứng từ trùng số. Số cuối cùng là " + "[" + GetLastSoct(StartUp.SysObj, txtMa_qs.Text).Trim() + "]" + ". Có lưu chứng từ này không?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                        //        {
                        //            txtSo_ct.SelectAll();
                        //            txtSo_ct.Focus();
                        //            isError = true;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        ExMessageBox.Show( 290,StartUp.SysObj, "Số chứng từ đã tồn tại!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        //        txtSo_ct.SelectAll();
                        //        txtSo_ct.Focus();
                        //        isError = true;
                        //    }
                        //}
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"].ToString()) && !isError)
                        {
                            ExMessageBox.Show( 295,StartUp.SysObj, "Chưa vào số hóa đơn!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            isError = true;
                            txtSo_ct.Text = GetNewSoct(StartUp.SysObj, txtMa_qs.Text);
                        }
                        //Kiểm tra so_ct có thuộc quyển ct không
                        if (!isError && !CheckSo_ct(StartUp.DsTrans.Tables[0].Rows[iRow]["transform"].ToString(), ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["so_ct1"], 0), ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["so_ct2"], 0), txtSo_ct.Text.Trim()))
                        {
                            ExMessageBox.Show( 300,StartUp.SysObj, "Số hóa đơn không thuộc ký hiệu hiện hành!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtSo_ct.Text = GetNewSoct(StartUp.SysObj, txtMa_qs.Text);
                            isError = true;
                        }
                        //if (!isError && CheckValidSoct(StartUp.SysObj, txtMa_qs.Text, txtSo_ct.Text, StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()))
                        //{
                        //    ExMessageBox.Show( 305,StartUp.SysObj, "Số hóa đơn đã có!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        //    isError = true;
                        //    txtSo_ct.Text = GetNewSoct(StartUp.SysObj, txtMa_qs.Text);
                        //}
                        //else if (!isError && currActionTask != ActionTask.Edit && !CheckValidSoctLT(StartUp.SysObj, txtMa_qs.Text, txtSo_ct.Text))
                        //{
                        //    ExMessageBox.Show( 310,StartUp.SysObj, "Số hóa đơn không liên tục!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        //    isError = true;
                        //    txtSo_ct.Text = GetNewSoct(StartUp.SysObj, txtMa_qs.Text);
                        //}

                        //if (!isError)
                        //{
                        //    if (currActionTask != ActionTask.Edit)
                        //    {
                        //        DataSet ds = StartUpTrans.CheckQS(txtMa_qs.Text, txtNgay_ct.dValue.ToString("yyyyMMdd"), Convert.ToInt16(SysO.UserInfo.Rows[0]["user_id"].ToString()), txtSo_ct.Text, txtSo_ct.Text, "1;2;3;4;5");
                        //        if (ds.Tables.Count > 0)
                        //        {
                        //            int isresult = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                        //            string ma_qs = ds.Tables[1].Rows[0][0].ToString().Trim();
                        //            int index = Convert.ToInt32(ds.Tables[2].Rows[0][0]);

                        //            if (isresult == 1)
                        //            {
                        //                ExMessageBox.Show( 315,SysO, "Ngày bắt đầu sử dụng ký hiệu không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        //                txtNgay_ct.Focus();
                        //                isError = true;
                        //            }
                        //            else if (isresult == 2)
                        //            {
                        //                ExMessageBox.Show( 320,SysO, "Quyền sử dụng ký hiệu không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        //                txtMa_qs.IsFocus = true;
                        //                isError = true;
                        //            }
                        //            else if (isresult == 3)
                        //            {
                        //                string ten_tthd = ds.Tables[3].Rows[0]["ten_tthd"].ToString().Trim().ToLower();
                        //                ExMessageBox.Show( 325,SysO, "Số hóa đơn của ký hiệu " + "[" + txtMa_qs.Text.Trim() + "]" + " đã " + "[" + ten_tthd + "]" + "!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        //                txtSo_ct.Text = GetNewSoct(StartUp.SysObj, txtMa_qs.Text);
                        //                isError = true;
                        //            }
                        //        }


                        //        if (!isError)
                        //        {
                        //            if (!CheckValidSoctLT(StartUp.SysObj, txtMa_qs.Text, txtSo_ct.Text))
                        //            {
                        //                ExMessageBox.Show( 330,StartUp.SysObj, "Số hóa đơn không liên tục!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        //                isError = true;
                        //                txtSo_ct.Text = GetNewSoct(StartUp.SysObj, txtMa_qs.Text);
                        //            }
                        //            if (!isError)
                        //                UpdateNewSoCt(SysO, txtMa_qs.Text);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        UpdateNewNgayCt(SysO, txtMa_qs.Text, GetCurrentSo_ct(txtMa_qs.RowResult["transform"].ToString(), txtSo_ct.Text.Trim()));
                        //    }
                        //}
                    }
                }
                if (!isError)
                {
                    if (!IsSequenceSave)
                    {
                        if (txtMa_kh.RowResult == null)
                        {
                            txtMa_kh.SearchInit();
                        }
                        if (String.IsNullOrEmpty(txtMa_kh.RowResult["dia_chi"].ToString().Trim()) || String.IsNullOrEmpty(txtMa_kh.RowResult["ma_so_thue"].ToString().Trim()) /*|| !SmLib.SysFunc.CheckSumMaSoThue(txtMa_kh.RowResult["ma_so_thue"].ToString().Trim())*/)
                        {
                            KhInfoFrm KhInfoFrm = new KhInfoFrm();
                            KhInfoFrm.ShowDialog();
                            if (KhInfoFrm.IsAllowSave == false)
                            {
                                return;
                            }
                        }

                        string ten_kh = StartUp.DsTrans.Tables[0].DefaultView[0][StartUp.M_LAN.Equals("V") ? "ten_kh" : "ten_kh2"].ToString().Trim();
                        if (ten_kh == "")
                        {
                            txtMa_kh.SearchInit();
                            txtMa_kh.IsDataChanged = true;
                            txtMa_kh_PreviewLostFocus(txtMa_kh, null);
                        }
                        if (!SmLib.SysFunc.CheckSumMaSoThue(txtma_so_thue.Text.Trim()) && !string.IsNullOrEmpty(txtma_so_thue.Text.Trim()))
                        {
                            switch (StartUp.M_MST_CHECK.Trim())
                            {
                                case "0":
                                    break;
                                case "1":
                                    ExMessageBox.Show( 335,StartUp.SysObj, "Mã số thuế không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                    break;
                                case "2":
                                    ExMessageBox.Show( 340,StartUp.SysObj, "Mã số thuế không hợp lệ, không lưu được!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                    isError = true;
                                    break;
                            }
                        }

                        if (StartUp.DsTrans.Tables[1].DefaultView.Count > 0)
                        {
                            int i = 0;
                            foreach (DataRowView drv in StartUp.DsTrans.Tables[1].DefaultView)
                            {
                                if (string.IsNullOrEmpty(drv.Row["tk_dt"].ToString().Trim()))
                                {
                                    StartUp.DsTrans.Tables[1].Rows.Remove(drv.Row);
                                    StartUp.DsTrans.Tables[1].AcceptChanges();
                                    continue;
                                }
                                if (string.IsNullOrEmpty(drv.Row["tk_ck"].ToString().Trim()))
                                {
                                    if (Convert.ToDecimal(drv.Row["tl_ck"].ToString()) > 0)
                                    {
                                        ExMessageBox.Show( 345,StartUp.SysObj, "Chưa vào tk ck, không lưu được!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                        isError = true;
                                        GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_ck"];
                                        GrdCt.Focus();
                                        GrdCt.ExecuteCommand(DataPresenterCommands.StartEditMode);
                                    }
                                }
                                i++;
                            }
                        }

                        object o_t_tien_thue_nt = StartUp.DsTrans.Tables[1].Compute("sum(thue_nt)", "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'");
                        object o_t_tien_nt2 = StartUp.DsTrans.Tables[1].Compute("sum(tien_nt2)", "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'");

                        object o_t_tien_thue = StartUp.DsTrans.Tables[1].Compute("sum(thue)", "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'");
                        object o_t_tien2 = StartUp.DsTrans.Tables[1].Compute("sum(tien2)", "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'");

                        decimal t_tien_thue_nt = Convert.ToDecimal(o_t_tien_thue_nt.Equals(DBNull.Value) ? 0 : o_t_tien_thue_nt);
                        decimal t_tien_nt2 = Convert.ToDecimal(o_t_tien_nt2.Equals(DBNull.Value) ? 0 : o_t_tien_nt2);
                        decimal t_tien_thue = Convert.ToDecimal(o_t_tien_thue.Equals(DBNull.Value) ? 0 : o_t_tien_thue);
                        decimal t_tien2 = Convert.ToDecimal(o_t_tien2.Equals(DBNull.Value) ? 0 : o_t_tien2);
                        if (currActionTask == ActionTask.Copy && (t_tien_nt2 != t_tien_thue_nt || t_tien_thue != t_tien2))
                        {
                            //if (txtT_thue.nValue != 0 && txtT_thue_nt.nValue != 0 && txtT_thue_nt.nValue != 0)
                            //{
                            //    ExMessageBox.Show( 350,StartUp.SysObj, "Tổng tiền/ tiền ngoại tệ khác với tổng tiền/ tiền ngoại tệ trong các hóa đơn giá trị gia tăng!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                            //    isError = true;
                            //}
                        }

                        else if (t_tien_nt2 != t_tien_thue_nt || t_tien_thue != t_tien2)
                        {
                            //if (GrdCt.Records.Count > 0)
                            //    ExMessageBox.Show( 355,StartUp.SysObj, "Tổng tiền/ tiền ngoại tệ khác với tổng tiền/ tiền ngoại tệ trong các hóa đơn giá trị gia tăng!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        }

                        if (!isError)
                        {
                            //Cân bằng trường tiền
                            decimal _ty_gia = txtTy_gia.nValue;

                            if (!txtMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()) && GrdCt.Records.Count > 0 && _ty_gia != 0 && !Chksua_tien.IsChecked.Value)
                            {
                                decimal _so_phieu_sai = 0;
                                var v_so_phieu_sai = StartUp.DsTrans.Tables[1].AsEnumerable()
                                   .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() && (b.Field<decimal?>("tien_nt2") == 0) && b.Field<decimal?>("tien2") != 0)
                                   .Count();
                                //if (v_so_phieu_sai != null)
                                decimal.TryParse(v_so_phieu_sai.ToString(), out _so_phieu_sai);
                                if (_so_phieu_sai == 0)
                                {
                                    //Tính tiền hàng
                                    decimal _sum_tien = SmLib.SysFunc.Round(_ty_gia * t_tien_nt2, Convert.ToInt16(StartUp.M_ROUND));

                                    txtt_tien.Value = _sum_tien;
                                    txttien_sau_ck.Value = _sum_tien - txtt_ck.nValue;
                                    ////Gán số dư cho phiếu đầu tiên
                                    decimal _sum_tien_nt0 = 0;
                                    var vtien_nt0 = StartUp.DsTrans.Tables[1].AsEnumerable()
                                        .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                                        .Sum(x => x.Field<decimal?>("tien2"));
                                    if (vtien_nt0 != null)
                                        decimal.TryParse(vtien_nt0.ToString(), out _sum_tien_nt0);
                                    (GrdCt.Records[0] as DataRecord).Cells["tien2"].Value = Convert.ToDecimal((GrdCt.Records[0] as DataRecord).Cells["tien2"].Value) + (_sum_tien - _sum_tien_nt0);
                                    //Tính tiền thuế
                                    //decimal _sum_thue_nt0 = 0;
                                    //_sum_thue_nt0 = txtT_thue_nt.Value == DBNull.Value ? 0 : Convert.ToDecimal(txtT_thue_nt.nValue);

                                    //Tính tổng thanh toán
                                    //--------------------////// txtT_tt_nt.Value = _sum_thue_nt0 + _sum_tien_nt0;
                                    UpdateTotalHT();
                                }
                                else if (_so_phieu_sai < GrdCt.Records.Count)
                                {
                                    //Tính tiền hàng
                                    decimal _sum_tien = SmLib.SysFunc.Round(_ty_gia * t_tien_nt2, Convert.ToInt16(StartUp.M_ROUND));

                                    txtt_tien.Value = _sum_tien;
                                    txttien_sau_ck.Value = _sum_tien + txtt_ck.nValue;
                                    ////Gán số dư cho phiếu đầu tiên không sai
                                    decimal _sum_tien_nt0 = 0;
                                    var vtien_nt0 = StartUp.DsTrans.Tables[1].AsEnumerable()
                                        .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                                        .Sum(x => x.Field<decimal?>("tien2"));
                                    if (vtien_nt0 != null)
                                        decimal.TryParse(vtien_nt0.ToString(), out _sum_tien_nt0);
                                    for (int i = 0; i < GrdCt.Records.Count; i++)
                                    {
                                        DataRecord dr = GrdCt.Records[i] as DataRecord;
                                        decimal tien_nt = 0, tien = 0;
                                        decimal.TryParse(dr.Cells["tien_nt2"].Value.ToString(), out tien_nt);
                                        decimal.TryParse(dr.Cells["tien2"].Value.ToString(), out tien);
                                        if (tien_nt == 0 && tien != 0)
                                        {
                                            //Phiếu sai.
                                        }
                                        else
                                        {
                                            dr.Cells["tien2"].Value = tien + (_sum_tien - _sum_tien_nt0);
                                            break;
                                        }
                                    }
                                    ////Tính tiền thuế
                                    //decimal _sum_thue_nt0 = 0;
                                    //_sum_thue_nt0 = txtT_thue_nt.Value == DBNull.Value ? 0 : Convert.ToDecimal(txtT_thue_nt.nValue);

                                    ////Tính tổng thanh toán
                                    //txtT_tt_nt.Value = _sum_thue_nt0 + _sum_tien_nt0;
                                    UpdateTotalHT();
                                }
                            }

                            //Điền thông tin vào 1 số trường khác cho Ph.
                            if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"].ToString()))
                                StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"] = StartUp.DmctInfo["ma_gd"];
                            if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_dvcs"].ToString()))
                                StartUp.DsTrans.Tables[0].DefaultView[0]["ma_dvcs"] = StartUp.SysObj.GetOption("M_MA_DVCS").ToString();
                        }
                        #region Kiểm tra tài khoản chi tiết
                        //if (!isError && StartUp.IsTkMe(txtMa_nx.Text.ToString().Trim()))
                        //{
                        //    ExMessageBox.Show( 360,StartUp.SysObj, "Tk nợ là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        //    isError = true;
                        //    txtMa_nx.IsFocus = true;
                        //}

                        //if (!isError && !string.IsNullOrEmpty(txtTk_du_voi_Tk_thue.Text.ToString().Trim()) && StartUp.IsTkMe(txtTk_du_voi_Tk_thue.Text.ToString().Trim()))
                        //{
                        //    ExMessageBox.Show( 365,StartUp.SysObj, "Tk đ.ứng với tk thuế là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        //    isError = true;
                        //    txtTk_du_voi_Tk_thue.IsFocus = true;
                        //}

                        //for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count && isError == false; i++)
                        //{
                        //    if (!isError && StartUp.IsTkMe(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_dt"].ToString().Trim()))
                        //    {
                        //        ExMessageBox.Show( 370,StartUp.SysObj, "Tk doanh thu là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        //        isError = true;
                        //        GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_dt"];
                        //        GrdCt.Focus();
                        //    }
                        //    if (!string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_ck"].ToString().Trim()))
                        //        if (!isError && StartUp.IsTkMe(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_ck"].ToString().Trim()))
                        //        {
                        //            ExMessageBox.Show( 375,StartUp.SysObj, "Tk ck là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        //            isError = true;
                        //            GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_ck"];
                        //            GrdCt.Focus();
                        //        }
                        //    if (!string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_thue_i"].ToString().Trim()))
                        //        if (!isError && StartUp.IsTkMe(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_thue_i"].ToString().Trim()))
                        //        {
                        //            ExMessageBox.Show( 380,StartUp.SysObj, "Tk thuế là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        //            isError = true;
                        //            GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_thue_i"];
                        //            GrdCt.Focus();
                        //        }

                        //}
                        #endregion
                    }
                    if (!isError)
                    {
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
                                // update thông tin cho các record Table1 (Ct) 
                                drv["ngay_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ngay_ct"];
                                drv["so_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"];
                                drv["ma_ct"] = StartUp.Ma_ct;
                                drv["ten_vt"] = drv["dien_giaii"];

                                if (txtMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
                                {
                                    drv["tien2"] = drv["tien_nt2"];
                                    drv["ck"] = drv["ck_nt"];
                                    drv["thue"] = drv["thue_nt"];
                                }

                            }
                            tbCtToSave.Rows.Add(drv.Row.ItemArray);
                        }

                        if (!DataProvider.UpdateCtTable(StartUp.SysObj, StartUp.DmctInfo["m_ctdbf"].ToString(), tbCtToSave, StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()))
                        {
                            ExMessageBox.Show( 385,StartUp.SysObj, "Lưu không thành công, kiểm tra lại dữ liệu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        if (!IsSequenceSave)
                        {
                            if (!isError)
                            {
                                //if (dsCheckData == null || dsCheckData.Tables[0].Rows.Count == 0)
                                    dsCheckData = StartUp.CheckData(currActionTask == ActionTask.Edit ? 0 : 1);

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
                                                        if (ExMessageBox.Show( 390,StartUp.SysObj, "Có chứng từ trùng số. Số cuối cùng là: " + "[" + GetLastSoct(StartUp.SysObj, txtMa_qs.Text).Trim() + "]" + ". Có lưu chứng từ này không?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                                                        {
                                                            txtSo_ct.SelectAll();
                                                            txtSo_ct.Focus();
                                                            isError = true;
                                                        }
                                                    }
                                                    else if (StartUp.M_trung_so.Equals("2"))
                                                    {
                                                        ExMessageBox.Show( 395,StartUp.SysObj, "Số chứng từ đã tồn tại!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                                        txtSo_ct.SelectAll();
                                                        txtSo_ct.Focus();
                                                        isError = true;
                                                    }
                                                }
                                                break;
                                            case "PH02":
                                                {
                                                    if (currActionTask != ActionTask.Edit)
                                                    {
                                                        if (dsCheckData.Tables.Count > 1)
                                                        {
                                                            int isresult = Convert.ToInt32(dsCheckData.Tables[1].Rows[0][0]);
                                                            string ma_qs = dsCheckData.Tables[2].Rows[0][0].ToString().Trim();
                                                            int index = Convert.ToInt32(dsCheckData.Tables[3].Rows[0][0]);

                                                            if (isresult == 1)
                                                            {
                                                                ExMessageBox.Show( 400,SysO, "Ngày bắt đầu sử dụng ký hiệu hóa đơn không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                                txtNgay_ct.Focus();
                                                                isError = true;
                                                            }
                                                            else if (isresult == 2)
                                                            {
                                                                ExMessageBox.Show( 405,SysO, "Quyền sử dụng ký hiệu hóa đơn không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                                txtMa_qs.IsFocus = true;
                                                                isError = true;
                                                            }
                                                            else if (isresult == 3)
                                                            {
                                                                string ten_tthd = dsCheckData.Tables[4].Rows[0]["ten_tthd"].ToString().Trim().ToLower();
                                                                ExMessageBox.Show( 410,SysO, "Số hóa đơn của ký hiệu " + "[" + txtMa_qs.Text.Trim() + "]" + " đã " + "[" + ten_tthd + "]" + "!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                                txtSo_ct.Text = GetNewSoct(StartUp.SysObj, txtMa_qs.Text);
                                                                isError = true;
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                            case "PH03":
                                                {
                                                    ExMessageBox.Show( 415,StartUp.SysObj, "Số hóa đơn không liên tục!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    isError = true;
                                                    txtSo_ct.Text = GetNewSoct(StartUp.SysObj, txtMa_qs.Text);
                                                }
                                                break;
                                            case "PH04":
                                                {
                                                    ExMessageBox.Show( 420,StartUp.SysObj, "Mã nx là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    isError = true;
                                                    txtMa_nx.IsFocus = true;
                                                }
                                                break;
                                            case "PH05":
                                                {
                                                    ExMessageBox.Show( 425,StartUp.SysObj, "Tk đ.ứng với tk thuế là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    isError = true;
                                                    txtTk_du_voi_Tk_thue.IsFocus = true;
                                                }
                                                break;
                                            case "CT01":
                                                {
                                                    int index = Convert.ToInt16(dv[1]);
                                                    ExMessageBox.Show( 430,StartUp.SysObj, "Tk doanh thu là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    isError = true;
                                                    GrdCt.ActiveCell = (GrdCt.Records[index] as DataRecord).Cells["tk_dt"];
                                                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                                    {
                                                        GrdCt.Focus();
                                                    }));
                                                }
                                                break;
                                            case "CT02":
                                                {
                                                    if (StartUp.M_AR_CK == 1)
                                                    {
                                                        int index = Convert.ToInt16(dv[1]);
                                                        ExMessageBox.Show(435, StartUp.SysObj, "Tk c.khấu là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                        isError = true;
                                                        GrdCt.ActiveCell = (GrdCt.Records[index] as DataRecord).Cells["tk_ck"];
                                                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                                        {
                                                            GrdCt.Focus();
                                                        }));
                                                    }
                                                }
                                                break;
                                            case "CT03":
                                                {
                                                    int index = Convert.ToInt16(dv[1]);
                                                    ExMessageBox.Show( 440,StartUp.SysObj, "Tk thuế là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    isError = true;
                                                    GrdCt.ActiveCell = (GrdCt.Records[index] as DataRecord).Cells["tk_thue_i"];
                                                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                                    {
                                                        GrdCt.Focus();
                                                    }));
                                                }
                                                break;
                                        }
                                        dsCheckData.Tables[0].Rows.Remove(dv.Row);
                                    }
                                }
                            }

                            if (!isError && IsNd51)
                            {
                                if (txtMa_qs.RowResult == null)
                                    txtMa_qs.SearchInit();
                                UpdateNewSoCt(SysO, txtMa_qs.Text);

                                if (currActionTask == ActionTask.Edit && txtMa_qs.RowResult != null)
                                {
                                    UpdateNewNgayCt(SysO, txtMa_qs.Text, GetCurrentSo_ct(txtMa_qs.RowResult["transform"].ToString(), txtSo_ct.Text.Trim()));
                                }
                            }

                        }
                        if (!isError)
                        {
                            bool _createPT1 = false;
                            string newstt_recPt1 = "";
                            DataTable dt = new DataTable();
                            dt.Columns.Add("ma_ct", typeof(string));
                            dt.Columns.Add("stt_rec", typeof(string));
                            dt.Columns.Add("stt_recPT", typeof(string));
                            dt.Columns.Add("ma_qs", typeof(string));
                            dt.Columns.Add("so_ct", typeof(string));
                            dt.Columns.Add("ma_nt", typeof(string));
                            dt.Columns.Add("ty_gia", typeof(decimal));
                            dt.Columns.Add("ty_giaf", typeof(decimal));
                            dt.Columns.Add("nguoinop", typeof(string));
                            dt.Columns.Add("lydonop", typeof(string));
                            dt.Columns.Add("ma_gd", typeof(string));

                            //string menu_idPT1 = BindingSysObj.CommandInfo.Select("ma_ct LIKE 'PT1'").First()["menu_id"].ToString();
                            string menu_idPT1 = BindingSysObj.ExcuteScalar(new SqlCommand("Select top 1 menu_id From command Where ma_ct like 'PT1'")).ToString().Trim();
                            if (SmLib.SysFunc.CheckPermission(BindingSysObj, ActionTask.Add, menu_idPT1))
                            {
                                if (StartUp.DsTrans.Tables[0].DefaultView[0]["status"].ToString().Equals("2"))
                                {
                                    bool _isTienMat = false;
                                    string[] _ds_tkkt = BindingSysObj.GetOption("M_TK_TK_VT").ToString().Split(',');
                                    foreach (string tk in _ds_tkkt)
                                    {
                                        if (txtMa_nx.Text.Trim().StartsWith(tk.ToString().Trim()))
                                            _isTienMat = true;
                                    }
                                    if (_isTienMat)
                                    {
                                        switch (BindingSysObj.GetOption("M_TAO_PT_TM").ToString())
                                        {
                                            case "1":
                                                if (ExMessageBox.Show(696, StartUp.SysObj, "Có tạo phiếu thu tiền ngay cho hóa đơn bán hàng?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                                                {
                                                    _createPT1 = true;
                                                }
                                                break;
                                            case "2":
                                                _createPT1 = true;
                                                break;
                                        }
                                        if (_createPT1)
                                        {
                                            //156033596
                                            if (string.IsNullOrEmpty(StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"].ToString().Trim()))
                                            {
                                                DataTable tbInfoPT = null;
                                                if (!string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"].ToString()))
                                                {
                                                    SqlCommand _cmd = new SqlCommand();
                                                    _cmd.CommandText = string.Format("SELECT stt_rec,ma_ct,ma_gd,ma_qs,so_ct,ma_nt,ong_ba,dien_giai FROM {0} WHERE stt_rec LIKE '{1}'", StartUp.DsTrans.Tables[0].DefaultView[0]["ma_ct_pt"].ToString().Equals("PT1") ? "ph41" : "ph51", StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"].ToString());
                                                    tbInfoPT = BindingSysObj.ExcuteReader(_cmd).Tables[0];
                                                    if (tbInfoPT.Rows.Count == 1)
                                                        StartUp.DeletePT(StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"].ToString().Trim(), StartUpTrans.DsTrans.Tables[0].DefaultView[0]["ma_ct_pt"].ToString().Trim());
                                                }
                                                int _isTk = 0;
                                                //Nếu tiền thanh toán > M_MUC_TIEN_PT1 thì tự động tạo bc1
                                                if (txtT_tt.nValue > Convert.ToDecimal(BindingSysObj.GetSysVar("M_MUC_TIEN_PT1")))
                                                {
                                                    _isTk = 1;
                                                }
                                                else
                                                {
                                                    //nếu là tk ngân hàng mặc định tạo phiếu báo có
                                                    SqlCommand _cmd = new SqlCommand();
                                                    _cmd.CommandText = "SELECT COUNT(1) FROM dmtknh WHERE tk LIKE @tk";
                                                    _cmd.Parameters.Add(new SqlParameter("@tk", SqlDbType.VarChar)).Value = txtMa_nx.Text.Trim();
                                                    _isTk = (int)BindingSysObj.ExcuteScalar(_cmd);
                                                }

                                                DataRow _dr = dt.NewRow();
                                                dt.Rows.Add(_dr);

                                                FrmTaoPT _form = new FrmTaoPT();
                                                _form.tbInfoPT = tbInfoPT;
                                                _form.DataContext = dt.DefaultView;
                                                _form.txtMa_qs_pt.Text = StartUpTrans.DsTrans.Tables[0].DefaultView[0]["ma_qs_pt"].ToString();
                                                _form.txtso_ct_pt.Text = StartUpTrans.DsTrans.Tables[0].DefaultView[0]["so_ct_pt"].ToString();
                                                _form.txtnguoi_nop.Text = txtOng_ba.Text;
                                                _form.kind = (_isTk > 0 ? 2 : 1);
                                                _form.Ma_nt_ht = txtMa_nt.Text;
                                                _form.so_hd = txtSo_ct.Text.Trim();
                                                _form.ngay_hd = txtNgay_ct.dValue.ToShortDateString();
                                                _form.filterma_qs = txtMa_qs.Filter;
                                                _form.ShowDialog();
                                                if (!_form.isOk)
                                                    _createPT1 = false;
                                                else
                                                {
                                                    dt = dt.Copy();

                                                    if (_form.txtKind.Text.Equals("1"))
                                                        newstt_recPt1 = DataProvider.NewTrans(StartUp.SysObj, "PT1", StartUp.Ws_Id);
                                                    else
                                                        newstt_recPt1 = DataProvider.NewTrans(StartUp.SysObj, "BC1", StartUp.Ws_Id);
                                                    dt.Rows[0]["ma_ct"] = (_form.txtKind.Text.Equals("1") ? "PT1" : "BC1");
                                                    dt.Rows[0]["stt_rec"] = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"];
                                                    dt.Rows[0]["stt_recPT"] = newstt_recPt1;
                                                    dt.Rows[0]["ma_qs"] = _form.txtMa_qs_pt.Text;
                                                    dt.Rows[0]["so_ct"] = _form.txtso_ct_pt.Text.PadLeft(_form.txtso_ct_pt.MaxLength, ' ');
                                                    dt.Rows[0]["ma_nt"] = _form.txtMa_nt.Text;
                                                    dt.Rows[0]["ty_gia"] = _form.txtMa_nt.Text.Equals(StartUp.M_MA_NT0) ? 1 : txtTy_gia.Rate;
                                                    dt.Rows[0]["ty_giaf"] = _form.txtMa_nt.Text.Equals(StartUp.M_MA_NT0) ? 1 : txtTy_gia.RateF;
                                                    dt.Rows[0]["nguoinop"] = _form.txtnguoi_nop.Text;
                                                    dt.Rows[0]["lydonop"] = _form.txtlydo_nop.Text;
                                                    dt.Rows[0]["ma_gd"] = _form.txtMa_gd.Text;

                                                    //Cap nhat lai dstrans
                                                    StartUpTrans.DsTrans.Tables[0].DefaultView[0]["ma_qs_pt"] = _form.txtMa_qs_pt.Text;
                                                    StartUpTrans.DsTrans.Tables[0].DefaultView[0]["so_ct_pt"] = _form.txtso_ct_pt.Text;
                                                    StartUpTrans.DsTrans.Tables[0].DefaultView[0]["ma_ct_pt"] = (_form.txtKind.Text.Equals("1") ? "PT1" : "BC1");
                                                    StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"] = newstt_recPt1;
                                                }
                                            }
                                            else
                                            {
                                                _createPT1 = false;
                                            }
                                        }
                                    }
                                }
                            }
                            string _stt_rec1 = StartUp.DsTrans.Tables[1].DefaultView[0]["stt_rec"].ToString();
                            ThreadStart _thread = delegate()
                            {
                                Post(_createPT1 ? 1 : 0);
                                if (_createPT1)
                                {
                                    CreatePT1(dt);
                                }

                                Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                        new Action(() =>
                                        {
                                            if (!IsSequenceSave)
                                            {
                                                if (StartUp.DsTrans.Tables[1].DefaultView[0]["stt_rec"].ToString().Equals(_stt_rec1))
                                                {
                                                    LoadDataDu13();
                                                }

                                                if (_createPT1)
                                                    if (!string.IsNullOrEmpty(newstt_recPt1))
                                                    {
                                                        DataRow[] drs = StartUp.DsTrans.Tables[0].Select("stt_rec = '" + _stt_rec1 + "'");
                                                        if (drs.Length == 1)
                                                        {
                                                            drs[0]["stt_rec_pt"] = newstt_recPt1;
                                                            drs[0]["so_ct_pt"] = dt.Rows[0]["so_ct"];
                                                            drs[0]["ma_ct_pt"] = dt.Rows[0]["ma_ct"];
                                                        }
                                                    }
                                            }
                                        }));
                            };

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
                                IsInEditMode.Value = false;
                                currActionTask = ActionTask.View;
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

        void CreatePT1(DataTable dt)
        {
            try
            {

                SqlCommand cmd = new SqlCommand("exec [dbo].[ARCTHD1-CREATEPT1] @Stt_rec, @Stt_recPT, @ma_qs, @so_ct, @ma_nt, @ty_gia, @ty_giaf, @nguoinop, @lydonop, @ma_gd, @ma_ct");
                cmd.Parameters.Add("@Stt_rec", SqlDbType.VarChar).Value = dt.Rows[0]["stt_rec"];
                cmd.Parameters.Add("@Stt_recPT", SqlDbType.VarChar).Value = dt.Rows[0]["stt_recPT"];
                cmd.Parameters.Add("@ma_qs", SqlDbType.VarChar).Value = dt.Rows[0]["ma_qs"];
                cmd.Parameters.Add("@so_ct", SqlDbType.VarChar).Value = dt.Rows[0]["so_ct"];
                cmd.Parameters.Add("@ma_nt", SqlDbType.VarChar).Value = dt.Rows[0]["ma_nt"];
                cmd.Parameters.Add("@ty_gia", SqlDbType.Decimal).Value = dt.Rows[0]["ty_gia"];
                cmd.Parameters.Add("@ty_giaf", SqlDbType.Decimal).Value = dt.Rows[0]["ty_giaf"];
                cmd.Parameters.Add("@nguoinop", SqlDbType.NVarChar).Value = dt.Rows[0]["nguoinop"];
                cmd.Parameters.Add("@lydonop", SqlDbType.NVarChar).Value = dt.Rows[0]["lydonop"];
                cmd.Parameters.Add("@ma_gd", SqlDbType.VarChar).Value = dt.Rows[0]["ma_gd"];
                cmd.Parameters.Add("@ma_ct", SqlDbType.Char, 3).Value = dt.Rows[0]["ma_ct"];
                StartUp.SysObj.ExcuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }


        #region V_Xem
        private void V_Xem()
        {
            currActionTask = ActionTask.View;

            string stringBrowse1 = "";
            string stringBrowse2 = "";

            if (StartUp.M_LAN.Equals("V"))
            {
                if (StartUp.CommandInfo["Vbrowse1"] != null)
                {
                    String strVbrowse1 = StartUp.CommandInfo["Vbrowse1"].ToString();
                    stringBrowse1 = strVbrowse1.Split('|')[0];
                    stringBrowse2 = strVbrowse1.Split('|')[1];
                }
            }
            else
            {
                if (StartUp.CommandInfo["Ebrowse1"] != null)
                {
                    String strVbrowse1 = StartUp.CommandInfo["Ebrowse1"].ToString();
                    stringBrowse1 = strVbrowse1.Split('|')[0];
                    stringBrowse2 = strVbrowse1.Split('|')[1];
                }
            }
            DataTable PhViewTablev = StartUp.DsTrans.Tables[0].Copy();
            PhViewTablev.Rows.RemoveAt(0);
            SmVoucherLib.FormView _frmView = new SmVoucherLib.FormView(StartUp.SysObj, PhViewTablev.DefaultView, StartUp.DsTrans.Tables[1].DefaultView, stringBrowse1, stringBrowse2, "stt_rec");
            SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, _frmView.frmBrw.oBrowseCt, StartUp.Ma_ct, 1);
            _frmView.frmBrw.Title = SmLib.SysFunc.Cat_Dau(M_LAN.Equals("V") ? StartUp.CommandInfo["bar"].ToString() : StartUp.CommandInfo["bar2"].ToString());
            _frmView.ListFieldSum = "t_tt_nt;t_tt";

            _frmView.TongCongLabel = "Tổng cộng:";
            //_frmView.DataGrid.FieldLayouts[0].Fields[0].Settings.AllowFixing = AllowFieldFixing.No;
  


            _frmView.frmBrw.LanguageID  = "ARCTHD1_6";
            _frmView.ShowDialog();

            // Set lai irow va rowfilter ...
            if (_frmView.DataGrid.ActiveRecord != null)
            {

                int select_irow = (_frmView.DataGrid.ActiveRecord as DataRecord).Index;
                if (select_irow >= 0)
                {
                    string selected_stt_rec = (_frmView.DataGrid.DataSource as DataView)[select_irow]["stt_rec"].ToString();
                    FrmArcthd1.iRow = select_irow + 1;
                    StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + selected_stt_rec + "'";
                    StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + selected_stt_rec + "'";

                }
            }
        }
        #endregion
        #region V_Tim
        private void V_Tim()
        {
            try
            {
                currActionTask = ActionTask.View;
                //FrmTim2 _FrmTim2 = new FrmTim2(StartUp.SysObj, StartUp.DmctInfo["m_phdbf"].ToString(), StartUp.Ma_ct);
                //_FrmTim2.ShowDialog();
                FrmTim3 _FrmTim3 = new FrmTim3(StartUp.SysObj, StartUp.filterId, StartUp.filterView);
                SmLib.SysFunc.LoadIcon(_FrmTim3);
                _FrmTim3.ShowDialog();

            }
            catch (Exception ex)
            {

                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        #endregion
        #region V_In
        private void V_In()
        {
            FrmIn oReport = new FrmIn(IsNd51);
            oReport.ShowDialog();
        }
        #endregion
        void NewRowCt()
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
            }
            Stt_rec0 = Stt_rec0ct;
            Stt_rec0++;

            NewRecord["stt_rec0"] = string.Format("{0:000}", Stt_rec0);
            NewRecord["ma_ct"] = StartUp.Ma_ct;
            NewRecord["ngay_ct"] = txtNgay_ct.Value == null ? DateTime.Now.Date : txtNgay_ct.dValue.Date;
            NewRecord["tien_nt2"] = 0;
            NewRecord["tien2"] = 0;
            NewRecord["gia_nt2"] = 0;
            NewRecord["gia2"] = 0;
            NewRecord["so_luong"] = 0;
            NewRecord["tl_ck"] = 0;
            NewRecord["ck_nt"] = 0;
            NewRecord["thue_suati"] = 0;
            NewRecord["ma_kh2_i"] = String.Empty;
            NewRecord["thue_nt"] = 0;
            NewRecord["ck"] = 0;
            NewRecord["thue"] = 0;
            int count = StartUp.DsTrans.Tables[1].DefaultView.Count;
            if (count > 0)
            {
                NewRecord["dien_giaii"] = StartUp.DsTrans.Tables[1].DefaultView[count - 1].Row["dien_giaii"];
            }
            else
            {
                NewRecord["dien_giaii"] = StartUp.DsTrans.Tables[0].DefaultView[0].Row["dien_giai"];
            }
            FreeCodeFieldLib.CarryFreeCodeFields(StartUp.SysObj, StartUp.Ma_ct, StartUp.DsTrans.Tables[1].DefaultView, NewRecord, 1);
            StartUp.DsTrans.Tables[1].Rows.Add(NewRecord);


        }

        #region GrdCt Events
        private void GrdCt_RecordDelete(object sender, Infragistics.Windows.DataPresenter.Events.RecordsDeletedEventArgs e)
        {
            txtTk_du_voi_Tk_thue.IsFocus = true;
           // txtNhom_hh.Focus();
        }
        private void GrdCt_PreviewEditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            try
            {
                if (IsEditMode && GrdCt.ActiveCell != null && StartUp.DsTrans.Tables[1].DefaultView.Count > GrdCt.ActiveRecord.Index && StartUp.DsTrans.Tables[1].GetChanges(DataRowState.Deleted) == null)
                {
                    
                    decimal ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);
                    switch (e.Cell.Field.Name)
                    {
                        case "tk_dt":
                            {
                                AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                if (txt.RowResult != null && !txt.Text.Trim().Equals(""))
                                {
                                    e.Cell.Record.Cells["ten_tk_dt"].Value = txt.RowResult["ten_tk"];
                                    e.Cell.Record.Cells["ten_tk_dt2"].Value = txt.RowResult["ten_tk2"];
                                }
                                if (txt.Text.Trim().Equals(""))
                                {
                                    e.Cell.Record.Cells["ten_tk_dt"].Value = "";
                                }

                                break;
                            }


                        case "so_luong":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (txtTy_gia.Value != null && !string.IsNullOrEmpty(e.Editor.Text.Trim()))
                                    {
                                        Decimal _ty_gia = 0, _gia_nt2 = 0, _so_luong = 0, _tien_nt2 = 0;
                                        _gia_nt2 = ParseDecimal(e.Cell.Record.Cells["gia_nt2"].Value, 0);
                                        _so_luong = ParseDecimal(e.Cell.Record.Cells["so_luong"].Value, 0);
                                        _ty_gia = txtTy_gia.nValue;
                                        if (_so_luong == 0)
                                        {
                                            e.Cell.Record.Cells["gia_nt2"].Value = 0;
                                            e.Cell.Record.Cells["gia2"].Value = 0;
                                            return;
                                        }
                                        if (_gia_nt2 * _so_luong != 0)
                                        {
                                            if(txtMa_nt.Text.Equals(StartUp.M_MA_NT0))
                                                _tien_nt2 = SmLib.SysFunc.Round(_gia_nt2 * _so_luong, StartUp.M_ROUND);
                                            else
                                                _tien_nt2 = SmLib.SysFunc.Round(_gia_nt2 * _so_luong, StartUp.M_ROUND_NT);

                                            e.Cell.Record.Cells["tien_nt2"].Value = _tien_nt2;

                                            if (!(bool)Chksua_tien.IsChecked)
                                            {
                                                e.Cell.Record.Cells["tien2"].Value = _tien_nt2 * _ty_gia == 0 ? _tien_nt2 : SmLib.SysFunc.Round(_tien_nt2 * _ty_gia, StartUp.M_ROUND);
                                            }

                                            GrdCt_PreviewEditModeEnded(GrdCt,new Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs(e.Cell.Record.Cells["tien_nt2"],CellValuePresenter.FromCell(e.Cell.Record.Cells["tien_nt2"]).Editor,true));
                                            //UpdateTotalHT();
                                        }
                                    }
                                }
                            }
                            break;
                        case "gia_nt2":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (txtTy_gia.Value != null && !string.IsNullOrEmpty(e.Editor.Text.Trim()))
                                    {
                                        Decimal _ty_gia = 0, _gia_nt2 = 0, _so_luong = 0, _tien_nt2 = 0;
                                        _gia_nt2 = ParseDecimal(e.Cell.Record.Cells["gia_nt2"].Value, 0);
                                        _so_luong = ParseDecimal(e.Cell.Record.Cells["so_luong"].Value, 0);
                                        _ty_gia = txtTy_gia.nValue;
                                        if (_gia_nt2 * _so_luong != 0)
                                        {
                                            if (txtMa_nt.Text.Equals(StartUp.M_MA_NT0))
                                                _tien_nt2 = SmLib.SysFunc.Round(_gia_nt2 * _so_luong, StartUp.M_ROUND);
                                            else
                                                _tien_nt2 = SmLib.SysFunc.Round(_gia_nt2 * _so_luong, StartUp.M_ROUND_NT);

                                            e.Cell.Record.Cells["tien_nt2"].Value = _tien_nt2;
                                            if (!(bool)Chksua_tien.IsChecked)
                                            {
                                                e.Cell.Record.Cells["gia2"].Value = _tien_nt2 * _ty_gia == 0 ? _tien_nt2 : SmLib.SysFunc.Round(_gia_nt2 * _ty_gia, StartUp.M_ROUND_GIA);
                                                e.Cell.Record.Cells["tien2"].Value = _tien_nt2 * _ty_gia == 0 ? _tien_nt2 : SmLib.SysFunc.Round(_tien_nt2 * _ty_gia, StartUp.M_ROUND);
                                            }
                                        }

                                        if (txtMa_nt.Text == StartUp.M_ma_nt0)
                                        {
                                            e.Cell.Record.Cells["gia2"].Value = e.Cell.Record.Cells["gia_nt2"].Value;
                                        }


                                        GrdCt_PreviewEditModeEnded(GrdCt, new Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs(e.Cell.Record.Cells["tien_nt2"], CellValuePresenter.FromCell(e.Cell.Record.Cells["tien_nt2"]).Editor, true));
                                        //UpdateTotalHT();
                                    }
                                }
                            }
                            break;
                        case "gia2":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (txtTy_gia.Value != null && !string.IsNullOrEmpty(e.Editor.Text.Trim()))
                                    {
                                        Decimal _gia2 = 0, _so_luong = 0;
                                        _gia2 = ParseDecimal(e.Cell.Record.Cells["gia2"].Value, 0);
                                        _so_luong = ParseDecimal(e.Cell.Record.Cells["so_luong"].Value, 0);
                                        if (_so_luong * _gia2 != 0)
                                            e.Cell.Record.Cells["tien2"].Value = SmLib.SysFunc.Round(_so_luong * _gia2, StartUp.M_ROUND);
                                        GrdCt_PreviewEditModeEnded(GrdCt, new Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs(e.Cell.Record.Cells["tien2"], CellValuePresenter.FromCell(e.Cell.Record.Cells["tien2"]).Editor, true));
                                        //UpdateTotalHT();
                                    }
                                }
                            }
                            break;
                        case "tien_nt2":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (txtTy_gia.Value != null && !string.IsNullOrEmpty(e.Editor.Text.Trim()))
                                    {

                                        decimal _Ty_gia, _Tien_nt2, Tien2 = 0, _tl_ck = 0, ck_nt = 0, ck = 0, thue_nt = 0, thue = 0, thue_suat = 0;

                                        _tl_ck = ParseDecimal(e.Cell.Record.Cells["tl_ck"].Value, 0);
                                        _Tien_nt2 = (e.Editor as NumericTextBox).nValue;
                                        _Ty_gia = txtTy_gia.nValue;
                                        thue_suat = ParseDecimal(e.Cell.Record.Cells["thue_suati"].Value, 0);

                                        Tien2 = SmLib.SysFunc.Round(_Ty_gia * _Tien_nt2, Convert.ToInt16(StartUp.M_ROUND));
                                        if (Tien2 != 0)
                                        {
                                            e.Cell.Record.Cells["tien2"].Value = Tien2;
                                        }
                                        ck = SmLib.SysFunc.Round((_tl_ck * Tien2) / 100, Convert.ToInt16(StartUp.M_ROUND));
                                        if (ck != 0)
                                        {
                                            e.Cell.Record.Cells["ck"].Value = ck;
                                        }
                                        thue = SmLib.SysFunc.Round(TinhThue(Tien2, ck, thue_suat, Chkthue_ck0.IsChecked.Value), StartUp.M_ROUND);
                                        e.Cell.Record.Cells["thue"].Value = thue;


                                        ck_nt = SmLib.SysFunc.Round((_Tien_nt2 * _tl_ck) / 100, Convert.ToInt16(StartUp.M_ROUND));
                                        if (ck_nt != 0)
                                        {
                                            e.Cell.Record.Cells["ck_nt"].Value = ck_nt;
                                        }
                                        thue_nt = TinhThue(_Tien_nt2, ck_nt, thue_suat, Chkthue_ck0.IsChecked.Value);
                                        if (txtMa_nt.Text != StartUp.M_ma_nt0)
                                            thue_nt = SmLib.SysFunc.Round(thue_nt, StartUp.M_ROUND_NT);
                                        else
                                            thue_nt = SmLib.SysFunc.Round(thue_nt, StartUp.M_ROUND);
                                        e.Cell.Record.Cells["thue_nt"].Value = thue_nt;
                                    }

                                    if (txtMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["tien2"].Value = e.Cell.Record.Cells["tien_nt2"].Value;
                                        e.Cell.Record.Cells["ck"].Value = e.Cell.Record.Cells["ck_nt"].Value;
                                        e.Cell.Record.Cells["thue"].Value = e.Cell.Record.Cells["thue_nt"].Value;
                                    }

                                    UpdateTotalHT();
                                }
                                break;
                            }
                        case "tien2":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (!IsCheckedSua_tien.Value)
                                    {
                                        //Tính lại thuế post lỗi ngày 08/04/2011 1115066 
                                        if (!string.IsNullOrEmpty(e.Editor.Text.Trim()))
                                        {
                                            decimal tien2, tl_ck, thue_suat, ck, thue;
                                            tien2 = ParseDecimal(e.Cell.Record.Cells["tien2"].Value, 0);
                                            tl_ck = ParseDecimal(e.Cell.Record.Cells["tl_ck"].Value, 0) / 100;
                                            thue_suat = ParseDecimal(e.Cell.Record.Cells["thue_suati"].Value, 0);
                                            ck = SmLib.SysFunc.Round(tien2 * tl_ck, StartUp.M_ROUND);
                                            thue = SmLib.SysFunc.Round(TinhThue(tien2, ck, thue_suat, Chkthue_ck0.IsChecked.Value), StartUp.M_ROUND);
                                            e.Cell.Record.Cells["ck"].Value = ck;
                                            e.Cell.Record.Cells["thue"].Value = thue;
                                        }
                                    }
                                    UpdateTotalHT();
                                }
                                break;
                            }
                        case "tl_ck":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (txtTy_gia.Value != null && !string.IsNullOrEmpty(e.Editor.Text.Trim()))
                                    {

                                        decimal _Ty_gia, _Tien_nt2, Tien2 = 0, _tl_ck = 0, ck_nt = 0, ck = 0, thue_nt = 0, thue = 0, thue_suat = 0;
                                        _tl_ck = (e.Editor as NumericTextBox).nValue;
                                        _Tien_nt2 = ParseDecimal(e.Cell.Record.Cells["tien_nt2"].Value, 0);
                                        //Code cũ quên gán thuế suất nên tính tiền thuế luôn bằng 0
                                        thue_suat = ParseDecimal(e.Cell.Record.Cells["thue_suati"].Value, 0);

                                        _Ty_gia = txtTy_gia.nValue;

                                        Tien2 = SmLib.SysFunc.Round(_Ty_gia * _Tien_nt2, Convert.ToInt16(StartUp.M_ROUND));
                                        if (Tien2 != 0)
                                        {
                                            e.Cell.Record.Cells["tien2"].Value = Tien2;
                                        }


                                        ck_nt = SmLib.SysFunc.Round((_Tien_nt2 * _tl_ck) / 100, Convert.ToInt16(StartUp.M_ROUND_NT));
                                        e.Cell.Record.Cells["ck_nt"].Value = ck_nt;
                                        thue_nt = TinhThue(_Tien_nt2, ck_nt, thue_suat, Chkthue_ck0.IsChecked.Value);
                                        if (txtMa_nt.Text != StartUp.M_ma_nt0)
                                            thue_nt = SmLib.SysFunc.Round(thue_nt, StartUp.M_ROUND_NT);
                                        else
                                            thue_nt = SmLib.SysFunc.Round(thue_nt, StartUp.M_ROUND);
                                        e.Cell.Record.Cells["thue_nt"].Value = thue_nt;


                                        ck = SmLib.SysFunc.Round(ck_nt * _Ty_gia, Convert.ToInt16(StartUp.M_ROUND));
                                        e.Cell.Record.Cells["ck"].Value = ck;
                                        thue = SmLib.SysFunc.Round(TinhThue(Tien2, ck, thue_suat, Chkthue_ck0.IsChecked.Value), StartUp.M_ROUND);
                                        e.Cell.Record.Cells["thue"].Value = thue;
                                    }

                                    if (txtMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["tien2"].Value = e.Cell.Record.Cells["tien_nt2"].Value;
                                        e.Cell.Record.Cells["ck"].Value = e.Cell.Record.Cells["ck_nt"].Value;
                                        e.Cell.Record.Cells["thue"].Value = e.Cell.Record.Cells["thue_nt"].Value;
                                    }

                                    UpdateTotalHT();
                                }
                                break;
                            }

                        case "dien_giaii":
                            {
                                if (e.Cell.Record.Index == 0 && string.IsNullOrEmpty(e.Editor.Text.Trim()))
                                {
                                    e.Cell.Value = txtDien_giai.Text;
                                }
                                //else if (e.Cell.Record.Index > 0 && string.IsNullOrEmpty(e.Editor.Text.Trim()))
                                //    e.Cell.Value = (GrdCt.Records[e.Cell.Record.Index - 1] as DataRecord).Cells["dien_giaii"].Value;
                            }
                            break;
                        case "ma_thue_i":
                            {

                                AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                if (txt.RowResult != null)
                                {
                                    e.Cell.Record.Cells["tk_thue_i"].Value = txt.RowResult["tk_thue_co"];
                                    AutoCompleteTextBox txttk_thue_i = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(CellValuePresenter.FromCell(e.Cell.Record.Cells["tk_thue_i"]).Editor as ControlHostEditor);
                                    if (txttk_thue_i.RowResult == null)
                                        txttk_thue_i.SearchInit();
                                    if (txttk_thue_i.RowResult != null)
                                    {
                                        e.Cell.Record.Cells["tk_cn"].Value = txttk_thue_i.RowResult["tk_cn"];
                                    }

                                    //if (GrdCt.ActiveRecord.Index.Equals(0))
                                    //    txttk_thue_no.Text = txt.RowResult["tk_thue_no"].ToString();
                                    e.Cell.Record.Cells["thue_suati"].Value = txt.RowResult["thue_suat"];
                                }
                                if (!string.IsNullOrEmpty(e.Cell.Record.Cells["thue_suati"].Value.ToString()))
                                {
                                    // Nếu tính theo giá trước chiết khấu
                                    if (Chkthue_ck0.IsChecked == true)
                                    {
                                        decimal tien_nt = ParseDecimal(e.Cell.Record.Cells["tien_nt2"].Value, 0);
                                        decimal tien = ParseDecimal(e.Cell.Record.Cells["tien2"].Value, 0);
                                        decimal thue_suat = ParseDecimal(e.Cell.Record.Cells["thue_suati"].Value, 0);
                                        decimal thue_nt = tien_nt * thue_suat / 100;
                                        if (!txtMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
                                        {
                                            thue_nt = SmLib.SysFunc.Round(thue_nt, Convert.ToInt16(StartUp.M_ROUND_NT));
                                        }
                                        else
                                        {
                                            thue_nt = SmLib.SysFunc.Round(thue_nt, Convert.ToInt16(StartUp.M_ROUND));
                                        }
                                        e.Cell.Record.Cells["thue_nt"].Value = thue_nt;
                                        e.Cell.Record.Cells["thue"].Value = SmLib.SysFunc.Round(tien * thue_suat / 100, Convert.ToInt16(StartUp.M_ROUND));


                                    }
                                    else
                                    {
                                        decimal tien_nt = ParseDecimal(e.Cell.Record.Cells["tien_nt2"].Value, 0);
                                        decimal tien = ParseDecimal(e.Cell.Record.Cells["tien2"].Value, 0);
                                        decimal thue_suat = ParseDecimal(e.Cell.Record.Cells["thue_suati"].Value, 0);
                                        decimal ck_nt = ParseDecimal(e.Cell.Record.Cells["ck_nt"].Value, 0);
                                        decimal ck = ParseDecimal(e.Cell.Record.Cells["ck"].Value, 0);
                                        decimal thue_nt = (tien_nt - ck_nt) * thue_suat / 100;
                                        if (!txtMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
                                        {
                                            thue_nt = SmLib.SysFunc.Round(thue_nt, Convert.ToInt16(StartUp.M_ROUND_NT));
                                        }
                                        else
                                        {
                                            thue_nt = SmLib.SysFunc.Round(thue_nt, Convert.ToInt16(StartUp.M_ROUND));
                                        }
                                        e.Cell.Record.Cells["thue_nt"].Value = thue_nt;
                                        e.Cell.Record.Cells["thue"].Value = SmLib.SysFunc.Round((tien - ck) * thue_suat / 100, Convert.ToInt16(StartUp.M_ROUND));

                                    }

                                    if (txtMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["thue"].Value = e.Cell.Record.Cells["thue_nt"].Value;
                                    }

                                    UpdateTotalHT();
                                }
                            }
                            break;

                        #region tk_thue_no
                        case "tk_thue_i":
                            {
                                //Cập nhật tài khoản thuế
                                if (e.Editor.Value == null)
                                    return;
                                AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                if (txt.RowResult != null)
                                {
                                    e.Cell.Record.Cells["tk_cn"].Value = txt.RowResult["tk_cn"];
                                }
                                break;
                            }
                        #endregion

                        case "thue_nt":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (txtMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
                                        e.Cell.Record.Cells["thue"].Value = e.Cell.Record.Cells["thue_nt"].Value;
                                    else
                                    {
                                        decimal _Ty_gia = txtTy_gia.nValue;
                                        e.Cell.Record.Cells["thue"].Value = SmLib.SysFunc.Round(Convert.ToDecimal(e.Cell.Record.Cells["thue_nt"].Value) * _Ty_gia, StartUp.M_ROUND);
                                    }

                                    UpdateTotalHT();
                                }
                            }
                            break;
                        case "ck_nt":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                        e.Cell.Record.Cells["ck_nt"].Value = 0;

                                    if (txtMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
                                        e.Cell.Record.Cells["ck"].Value = e.Cell.Record.Cells["ck_nt"].Value;
                                    else
                                    {
                                        decimal ck_nt = ParseDecimal(e.Cell.Record.Cells["ck_nt"].Value, 0);
                                        decimal ck = SysFunc.Round(ck_nt * ty_gia, StartUp.M_ROUND);
                                        if (ck != 0)
                                        {
                                            e.Cell.Record.Cells["ck"].Value = ck;
                                        }
                                    }
                                    UpdateTotalHT();
                                }
                            }
                            break;
                        case "thue":
                        case "ck":
                            {
                                UpdateTotalHT();
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
        private bool GrdCt_AddNewRecord(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            NewRowCt();
            return true;
        }
        private void GrdCt_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsInEditMode.Value == false)
                return;
            if (Keyboard.IsKeyDown(Key.N) && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                NewRowCt();
                GrdCt.ActiveRecord = GrdCt.Records[GrdCt.Records.Count - 1];
            }
        }
        private void GrdCt_KeyUp(object sender, KeyEventArgs e)
        {
            if (IsInEditMode.Value == false)
                return;

            switch (e.Key)
            {
                case Key.F4:
                    NewRowCt();
                    GrdCt.ActiveRecord = GrdCt.Records[GrdCt.Records.Count - 1];
                    GrdCt.ActiveCell = (GrdCt.ActiveRecord as DataRecord).Cells[0];
                    break;
                case Key.F8:
                    {
                        if (ExMessageBox.Show( 445,StartUp.SysObj, "Có xóa dòng ghi hiện thời không?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                        {
                            return;
                        }

                        DataRecord ARow = (GrdCt.ActiveRecord as DataRecord);
                        if (ARow != null)
                        {
                            int indexRecord = 0, indexCell = 0;
                            Cell cell = GrdCt.ActiveCell;

                            indexRecord = ARow.Index;
                            if (ARow.Index == 0)
                            {
                                if (GrdCt.Records.Count == 1)
                                    GrdCt_AddNewRecord(null, null);
                            }
                            else if (ARow.Index == GrdCt.Records.Count - 1)
                            {
                                indexRecord = ARow.Index - 1;
                            }

                            indexCell = GrdCt.ActiveCell == null ? 0 : GrdCt.ActiveCell.Field.Index;

                            GrdCt.ExecuteCommand(DataPresenterCommands.EndEditModeAndDiscardChanges);
                            if (indexCell >= 0)
                            {
                                StartUp.DsTrans.Tables[1].Rows.Remove(StartUp.DsTrans.Tables[1].DefaultView[ARow.Index].Row);
                                StartUp.DsTrans.Tables[1].AcceptChanges();
                                if (GrdCt.Records.Count > 0)
                                    GrdCt.ActiveRecord = GrdCt.Records[indexRecord > GrdCt.Records.Count - 1 ? GrdCt.Records.Count - 1 : indexRecord];

                                UpdateTotalHT();
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            if (Keyboard.IsKeyDown(Key.Tab) && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {

            }
        } 
        #endregion

        Decimal TinhThue(Decimal tien_hang,Decimal ck,Decimal thue_suat,bool TinhThueTruocCK)
        {
            Decimal result = 0;
            if (TinhThueTruocCK == true)
            {
                result = tien_hang * thue_suat / 100;
            }
            else 
            {
                result = (tien_hang-ck) * thue_suat / 100;
            }
            return result;
        }
        private void UpdateTotalHT()
        {
            try
            {
                if (currActionTask == ActionTask.View)
                    return;
                StartUp.DsTrans.Tables[1].AcceptChanges();
                //Cập nhật tổng thanh toán nguyên tệ
                decimal _t_tien_nt2 = 0, _t_thue_nt = 0;

                var vtien_nt = StartUp.DsTrans.Tables[1].AsEnumerable()
                    .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                    .Sum(x => x.Field<decimal?>("tien_nt2"));
                if (vtien_nt != null)
                    decimal.TryParse(vtien_nt.ToString(), out _t_tien_nt2);
                txtt_tien.Value = SysFunc.Round(_t_tien_nt2, StartUp.M_ROUND);
                txtt_tien_nt.Value = SysFunc.Round(_t_tien_nt2, StartUp.M_ROUND_NT);
                //Tính tiền chiết khấu
                decimal _t_ck_nt=0;
                var vck_nt = StartUp.DsTrans.Tables[1].AsEnumerable()
                 .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                 .Sum(x => x.Field<decimal?>("ck_nt"));
                if (vck_nt != null)
                    decimal.TryParse(vck_nt.ToString(), out _t_ck_nt);
                txtt_ck_nt.Value = SysFunc.Round(_t_ck_nt, StartUp.M_ROUND_NT);
                txtt_ck.Value = SysFunc.Round(_t_ck_nt, StartUp.M_ROUND);
                //Tính thuế
                var vthue_nt = StartUp.DsTrans.Tables[1].AsEnumerable()
                .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                .Sum(x => x.Field<decimal?>("thue_nt"));
                if (vthue_nt != null)
                    decimal.TryParse(vthue_nt.ToString(), out _t_thue_nt);
                //_t_thue_nt = txtT_thue_nt.Value == DBNull.Value ? 0 : Convert.ToDecimal(txtT_thue_nt.nValue);
                txtT_thue_nt.Value = SysFunc.Round(_t_thue_nt, StartUp.M_ROUND_NT);
                txtT_thue.Value = SysFunc.Round(_t_thue_nt, StartUp.M_ROUND);
                //Tính tổng thanh toán
                txtT_tt.Value = SysFunc.Round(_t_tien_nt2 - _t_ck_nt + _t_thue_nt, StartUp.M_ROUND);
                txtT_tt_nt.Value = SysFunc.Round(_t_tien_nt2 - _t_ck_nt + _t_thue_nt, StartUp.M_ROUND_NT);

                //Tính tổng tiền sau ck
                txttien_sau_ck.Value = SysFunc.Round(_t_tien_nt2 - _t_ck_nt, StartUp.M_ROUND);
                txttien_sau_ck_nt.Value = SysFunc.Round(_t_tien_nt2 - _t_ck_nt, StartUp.M_ROUND_NT);

                //Cập nhật tổng thanh toán cho tien0
                if (!txtMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
                {
                    //decimal _so_phieu_sai = 0;
                    //var v_so_phieu_sai = StartUp.DsTrans.Tables[1].AsEnumerable()
                    //   .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() && (b.Field<decimal?>("tien_nt") == 0 || _ty_gia == 0) && b.Field<decimal?>("tien") != 0)
                    //   .Count();
                    //if (v_so_phieu_sai != null)
                    //    decimal.TryParse(v_so_phieu_sai.ToString(), out _so_phieu_sai);
                    //if (_so_phieu_sai == 0 && !ChkSuaTien.IsChecked.Value)
                    //{
                    //    //Tính tiền hàng
                    //    decimal _sum_tien = SmLib.SysFunc.Round(_ty_gia * _t_tien, Convert.ToInt16(StartUp.M_ROUND));

                    //    txtT_Tien_Nt0.Value = _sum_tien;
                    //    ////Gán số dư cho phiếu đầu tiên
                    //    if (GrdCt.Records.Count > 0 && _ty_gia != 0)
                    //    {
                    //        decimal _sum_tien_nt0 = 0;
                    //        var vtien_nt0 = StartUp.DsTrans.Tables[1].AsEnumerable()
                    //            .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                    //            .Sum(x => x.Field<decimal?>("tien"));
                    //        if (vtien_nt0 != null)
                    //            decimal.TryParse(vtien_nt0.ToString(), out _sum_tien_nt0);
                    //        (GrdCt.Records[0] as DataRecord).Cells["tien"].Value = Convert.ToDecimal((GrdCt.Records[0] as DataRecord).Cells["tien"].Value) + (_sum_tien - _sum_tien_nt0);
                    //    }
                    //    //Tính tiền thuế
                    //    decimal _sum_thue_nt0 = 0;
                    //    _sum_thue_nt0 = txtT_thue_Nt0.Value == DBNull.Value ? 0 : Convert.ToDecimal(txtT_thue_Nt0.nValue);

                    //    //Tính tổng thanh toán
                    //    txtT_tt_Nt0.Value = _sum_thue_nt0 + _sum_tien;
                    //}
                    //else
                    //{
                    //    decimal _sum_tien_nt0 = 0;
                    //    var vtien_nt0 = StartUp.DsTrans.Tables[1].AsEnumerable()
                    //        .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                    //        .Sum(x => x.Field<decimal?>("tien"));
                    //    if (vtien_nt0 != null)
                    //        decimal.TryParse(vtien_nt0.ToString(), out _sum_tien_nt0);
                    //    txtT_Tien_Nt0.Value = _sum_tien_nt0;

                    //    //Tính tiền thuế
                    //    decimal _sum_thue_nt0 = 0;
                    //    _sum_thue_nt0 = txtT_thue_Nt0.Value == DBNull.Value ? 0 : Convert.ToDecimal(txtT_thue_Nt0.nValue);

                    //    //Tính tổng thanh toán
                    //    txtT_tt_Nt0.Value = _sum_thue_nt0 + _sum_tien_nt0;
                    //}

                    // Tính tổng ck
                    decimal _t_ck = 0;
                    var vck = StartUp.DsTrans.Tables[1].AsEnumerable()
                     .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                     .Sum(x => x.Field<decimal?>("ck"));
                    if (vck != null)
                        decimal.TryParse(vck.ToString(), out _t_ck);
                    txtt_ck.Value = SysFunc.Round(_t_ck, StartUp.M_ROUND);
                    // Tính thuế 
                    decimal _t_thue = 0;
                    var vthue = StartUp.DsTrans.Tables[1].AsEnumerable()
                     .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                     .Sum(x => x.Field<decimal?>("thue"));
                    if (vthue != null)
                        decimal.TryParse(vthue.ToString(), out _t_thue);
                    txtT_thue.Value = SysFunc.Round(_t_thue, StartUp.M_ROUND);
                    // Tính tổng tiền hàng
                    decimal _t_tien = 0;
                    var vtien = StartUp.DsTrans.Tables[1].AsEnumerable()
                        .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                        .Sum(x => x.Field<decimal?>("tien2"));
                    if (vtien != null)
                        decimal.TryParse(vtien.ToString(), out _t_tien);
                    _t_thue = txtT_thue.Value == DBNull.Value ? 0 : Convert.ToDecimal(txtT_thue.nValue);
                    txtt_tien.Value = SysFunc.Round(_t_tien, StartUp.M_ROUND);
                    txtT_tt.Value = SysFunc.Round(_t_tien -_t_ck + _t_thue, StartUp.M_ROUND);
                   

                    // Tính tổng tien sau ck
                    txttien_sau_ck.Value = SysFunc.Round(_t_tien - _t_ck, StartUp.M_ROUND);
                }
                else
                {
                    txtt_tien_nt.Value = SysFunc.Round(_t_tien_nt2, StartUp.M_ROUND_NT);
                    txtT_tt_nt.Value = SysFunc.Round(_t_tien_nt2 - _t_ck_nt + _t_thue_nt, StartUp.M_ROUND_NT);
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        private void UpdateTotalThue()
        {
            //try
            //{
            //    if (currActionTask == ActionTask.View)
            //        return;
            //    StartUp.DsTrans.Tables[2].AcceptChanges();
            //    //Cập nhật tổng thanh toán nguyên tệ
            //    decimal _t_tien = 0, _t_thue = 0;

            //    //Tiền hàng
            //    _t_tien = txtT_Tien_nt.Value == DBNull.Value ? 0 : Convert.ToDecimal(txtT_Tien_nt.nValue);

            //    //Tính thuế
            //    var vthue = StartUp.DsTrans.Tables[2].AsEnumerable()
            //                    .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
            //                    .Sum(x => x.Field<decimal?>("t_thue_nt"));
            //    if (vthue != null)
            //        decimal.TryParse(vthue.ToString(), out _t_thue);

            //    txtT_thue.Value = _t_thue;
            //    txtT_thue_nt.Value = _t_thue;
            //    //Tính tổng thanh toán
            //    txtT_tt.Value = _t_tien + _t_thue;
            //    txtT_tt_nt.Value = _t_tien + _t_thue;
            //    //Cập nhật tổng thanh toán cho tien0
            //    if (!cbMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
            //    {
            //        //tiền nt0
            //        decimal _sum_tien_nt0 = 0;
            //        _sum_tien_nt0 = txtT_Tien_Nt0.Value == DBNull.Value ? 0 : Convert.ToDecimal(txtT_Tien_Nt0.nValue);
            //        //thuế nt0
            //        decimal _sum_thue_nt0 = 0;
            //        var vthue_nt0 = StartUp.DsTrans.Tables[2].AsEnumerable()
            //                    .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
            //                    .Sum(x => x.Field<decimal?>("t_thue"));
            //        if (vthue_nt0 != null)
            //            decimal.TryParse(vthue_nt0.ToString(), out _sum_thue_nt0);

            //        txtT_thue_Nt0.Value = _sum_thue_nt0;
            //        //Tính tổng thanh toán
            //        txtT_tt_Nt0.Value = _sum_tien_nt0 + _sum_thue_nt0;
            //    }
            //    else
            //    {
            //        txtT_thue_Nt0.Value = _t_thue;
            //        txtT_tt_Nt0.Value = _t_tien + _t_thue;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    SmErrorLib.ErrorLog.CatchMessage(ex);
            //}
        }
        #region UpdateTongThue
        void UpdateTongThue()
        {
            decimal thue_suat = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["thue_suat"], 0);
            int tinh_ck = ParseInt(StartUp.DsTrans.Tables[0].DefaultView[0]["tinh_ck"], 0);
            int sua_thue = ParseInt(StartUp.DsTrans.Tables[0].DefaultView[0]["sua_thue"], 0);

            decimal t_thue_nt = 0, t_thue = 0;
            if (tinh_ck == 1)
            {
                decimal t_tien_nt2 = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt2"], 0);
                decimal t_tien2 = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien2"], 0);
                t_thue_nt = SysFunc.Round((t_tien_nt2 * thue_suat) / 100, StartUp.M_ROUND_NT);
                t_thue = SysFunc.Round((t_tien2 * thue_suat) / 100, StartUp.M_ROUND);
            }
            else
            {
                decimal t_tien_sau_ck_nt = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_sau_ck_nt"], 0);
                decimal t_tien_sau_ck = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_sau_ck"], 0);
                t_thue_nt = SysFunc.Round((t_tien_sau_ck_nt * thue_suat) / 100, StartUp.M_ROUND_NT);
                t_thue = SysFunc.Round((t_tien_sau_ck * thue_suat) / 100, StartUp.M_ROUND);
            }

            if (sua_thue == 0)
            {
                //  if ( t_thue_nt != 0 || StartUp.DsTrans.Tables[1].DefaultView.Count == 0)
                StartUp.DsTrans.Tables[0].Rows[iRow]["t_thue_nt"] = t_thue_nt;
                // if ( t_thue != 0 || StartUp.DsTrans.Tables[1].DefaultView.Count == 0)
                StartUp.DsTrans.Tables[0].Rows[iRow]["t_thue"] = t_thue;
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

        #region CheckBoxs Events
        private void Chksua_tk_thue_Checked(object sender, RoutedEventArgs e)
        {
            IsCheckedSua_HT_Thue.Value = true;
            txtTk_du_voi_Tk_thue.IsFocus = true;
            
        }

        private void Chksua_tk_thue_Unchecked(object sender, RoutedEventArgs e)
        {
            //128624886
            //txtTk_du_voi_Tk_thue.Text = txtMa_nx.Text;
            IsCheckedSua_HT_Thue.Value = false;
            txtHan_tt.Focus();
        }

        private void Chksua_thue_Checked(object sender, RoutedEventArgs e)
        {
            IsCheckedSua_Thue.Value = true;
        }

        private void Chksua_thue_Unchecked(object sender, RoutedEventArgs e)
        {
            IsCheckedSua_Thue.Value = false;
            //Tính lại tiền thuế
            for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
            {
                DataRowView drv = StartUp.DsTrans.Tables[1].DefaultView[i];
                if (txtTy_gia.Value != null && !string.IsNullOrEmpty(drv["tien_nt2"].ToString()))
                {

                    decimal _Ty_gia, _Tien_nt2, Tien2 = 0, _tl_ck = 0, ck_nt = 0, ck = 0, thue_nt = 0, thue = 0, thue_suat = 0;

                    _tl_ck = ParseDecimal(drv["tl_ck"].ToString(), 0);
                    _Tien_nt2 = ParseDecimal(drv["tien_nt2"].ToString(), 0);
                    _Ty_gia = txtTy_gia.nValue;
                    thue_suat = ParseDecimal(drv["thue_suati"].ToString(), 0);

                    //sửa đoạn này 125842889
                    Tien2 = SmLib.SysFunc.Round(_Ty_gia * _Tien_nt2, Convert.ToInt16(StartUp.M_ROUND));
                    ck = SmLib.SysFunc.Round(((_Tien_nt2 * _tl_ck) / 100) * _Ty_gia, Convert.ToInt16(StartUp.M_ROUND));
                    
                    if (!Chksua_tien.IsChecked.Value || txtMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
                    {
                        //Tien2 = SmLib.SysFunc.Round(_Ty_gia * _Tien_nt2, Convert.ToInt16(StartUp.M_ROUND));
                        if (Tien2 > 0)
                        {
                            drv["tien2"] = SmLib.SysFunc.Round(Tien2, StartUp.M_ROUND);
                        }
                        //ck = SmLib.SysFunc.Round(((_Tien_nt2 * _tl_ck) / 100)*_Ty_gia, Convert.ToInt16(StartUp.M_ROUND));
                        if (ck > 0)
                        {
                            drv["ck"] = SmLib.SysFunc.Round(ck, StartUp.M_ROUND);
                        }
                        //thue = TinhThue(Tien2, ck, thue_suat, Chkthue_ck0.IsChecked.Value);
                        //A
                    }
                    ///từ A
                    thue = TinhThue(Tien2, ck, thue_suat, Chkthue_ck0.IsChecked.Value);
                    drv["thue"] = SmLib.SysFunc.Round(thue, StartUp.M_ROUND);
                    //
                    ck_nt = SmLib.SysFunc.Round((_Tien_nt2 * _tl_ck) / 100, StartUp.M_ROUND_NT);
                    if (ck_nt > 0)
                    {
                        drv["ck_nt"] = ck_nt;
                    }
                    thue_nt = TinhThue(_Tien_nt2, ck_nt, thue_suat, Chkthue_ck0.IsChecked.Value);
                    drv["thue_nt"] = SmLib.SysFunc.Round(thue_nt, StartUp.M_ROUND_NT);
                }
            }
            UpdateTotalHT();
        }

 
        private void Chksua_tien_Checked(object sender, RoutedEventArgs e)
        {
            IsCheckedSua_tien.Value = true;
        }

        private void Chksua_tien_Unchecked(object sender, RoutedEventArgs e)
        {
            IsCheckedSua_tien.Value = false;
            if (Chksua_tien.IsChecked == false && sender.GetType().Name.Equals("CheckBox"))
            {
                UpdateTotalChkSua_tien();
                CalculateTyGia();
            }
        }

        #endregion

        #region UpdateTotalChkSua_tien
        void UpdateTotalChkSua_tien()
        {
            int countCT = StartUp.DsTrans.Tables[1].DefaultView.Count;
            if (countCT > 0)
            {
                decimal so_luong, gia_nt2;//, gia_nt;
                decimal tien_nt2;
                decimal ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);
                for (int i = 0; i < countCT; i++)
                {
                    so_luong = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["so_luong"], 0);
                    gia_nt2 = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["gia_nt2"], 0);
                    //gia_nt = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["gia_nt"], 0);
                    if (so_luong != 0)
                    {
                        if (gia_nt2 != 0)
                        {
                            tien_nt2 = SysFunc.Round(so_luong * gia_nt2, StartUp.M_ROUND_NT);
                            StartUp.DsTrans.Tables[1].DefaultView[i]["tien_nt2"] = tien_nt2;

                            //tinh lai ck_nt
                            decimal tl_ck = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["tl_ck"], 0);
                            if (tl_ck != 0)
                            {
                                decimal ck_nt = SysFunc.Round((tien_nt2 * tl_ck) / 100, StartUp.M_ROUND_NT);
                                StartUp.DsTrans.Tables[1].DefaultView[i]["ck_nt"] = ck_nt;

                                decimal ck = SysFunc.Round(ck_nt * ty_gia, StartUp.M_ROUND);
                                if (ck != 0)
                                {
                                    StartUp.DsTrans.Tables[1].DefaultView[i]["ck"] = ck;
                                }
                            }
                        }
                        //if (gia_nt != 0)
                        //{
                        //    StartUp.DsTrans.Tables[1].DefaultView[i]["tien_nt"] = SysFunc.Round(so_luong * gia_nt, StartUp.M_ROUND_NT);
                        //}
                    }
                }
                UpdateTotalHT();
                

            }
        }
        #endregion

        public override string GetLanguageString(string code, string language)
        {
            return StartUp.GetLanguageString(code, language);
        }

        private void Chkthue_ck0_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsCheckedSua_tien.Value)
            {
                decimal tien_nt2, tien2, ts, thue_nt, thue;
                foreach (DataRowView drv in StartUp.DsTrans.Tables[1].DefaultView)
                {
                    tien_nt2 = ParseDecimal(drv["tien_nt2"], 0);
                    tien2 = ParseDecimal(drv["tien2"], 0);
                    //ck_nt = ParseDecimal(drv["ck_nt"], 0);
                    //ck = ParseDecimal(drv["ck"], 0);
                    ts = ParseDecimal(drv["thue_suati"], 0) / 100;
                    thue_nt = SmLib.SysFunc.Round(tien_nt2 * ts, StartUp.M_ROUND_NT);
                    thue = SmLib.SysFunc.Round(tien2 * ts, StartUp.M_ROUND);
                    drv["thue_nt"] = thue_nt;
                    drv["thue"] = thue;
                }
                UpdateTotalHT();
            }
        }

        private void Chkthue_ck0_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!IsCheckedSua_tien.Value)
            {
                decimal tien_nt2, tien2, ts, ck_nt, ck, thue_nt, thue;
                foreach (DataRowView drv in StartUp.DsTrans.Tables[1].DefaultView)
                {
                    tien_nt2 = ParseDecimal(drv["tien_nt2"], 0);
                    tien2 = ParseDecimal(drv["tien2"], 0);
                    ck_nt = ParseDecimal(drv["ck_nt"], 0);
                    ck = ParseDecimal(drv["ck"], 0);
                    ts = ParseDecimal(drv["thue_suati"], 0) / 100;
                    thue_nt = SmLib.SysFunc.Round((tien_nt2 - ck_nt) * ts, StartUp.M_ROUND_NT);
                    thue = SmLib.SysFunc.Round((tien2 - ck) * ts, StartUp.M_ROUND);
                    drv["thue_nt"] = thue_nt;
                    drv["thue"] = thue;
                }
                UpdateTotalHT();
            }
        }

        private void FormMain_EditModeEnded(object sender, string menuItemName, RoutedEventArgs e)
        {
            if (StartUp.DsTrans.Tables[0].DefaultView.Count > 0)
            {
                if (!menuItemName.Equals("btnSave"))
                    LoadDataDu13();
                Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
            }
        }

        private void txtTk_du_voi_Tk_thue_PreviewGotFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!IsCheckedSua_HT_Thue.Value)
            {
                SmLib.WinAPISenkey.SenKey(ModifierKeys.None, Key.Enter);
            }
        }

        #region GrdCt_PreviewGotKeyboardFocus
        private void GrdCt_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (currActionTask == ActionTask.Add)
            {
                if (StartUp.DsTrans.Tables[1].DefaultView.Count == 1 && StartUp.DsTrans.Tables[1].DefaultView[0]["dien_giaii"].ToString() == string.Empty)
                {
                    StartUp.DsTrans.Tables[1].DefaultView[0]["dien_giaii"] = StartUp.DsTrans.Tables[0].DefaultView[0].Row["dien_giai"];
                }
            }
        }
        #endregion

        private void txtHan_tt_GotFocus(object sender, RoutedEventArgs e)
        {
            txtHan_tt.SelectAll();
        }

        private void txtSo_seri_GotFocus(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background
                    , new Action(() =>
                    {
                        if (txtSo_seri.IsReadOnly == false)
                        {
                            txtSo_seri.Text = txtSo_seri.Text.Trim();
                            txtSo_seri.CaretIndex = txtSo_seri.Text.Length;
                            //txtSo_seri.Select(0, txtSo_seri.Text.Length);
                        }
                    }));
        }

        private void txtma_bp_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (StartUp.M_BP_BH.Equals("0"))
            {
                SmLib.WinAPISenkey.SenKey(ModifierKeys.None, Key.Enter);
            }
            else
            {
                if (txtma_bp.RowResult != null)
                {
                    if (M_LAN.ToUpper().Equals("V"))
                        StartUp.DsTrans.Tables[0].DefaultView[0]["ten_bp"] = txtma_bp.RowResult["ten_bp"].ToString();
                    else
                        StartUp.DsTrans.Tables[0].DefaultView[0]["ten_bp2"] = txtma_bp.RowResult["ten_bp2"].ToString();
                }
                else
                {
                    if (M_LAN.ToUpper().Equals("V"))
                        StartUp.DsTrans.Tables[0].DefaultView[0]["ten_bp"] = "";
                    else
                        StartUp.DsTrans.Tables[0].DefaultView[0]["ten_bp2"] = "";
                }
            }
        }

        private void txtHan_ck_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtHan_ck.RowResult != null)
                if (txtHan_tt.Value == DBNull.Value || txtHan_tt.nValue == 0)
                {
                    txtHan_tt.Value = txtHan_ck.RowResult["han_tt"];
                }
        }

        void Post(int ispostck)
        {
            SqlCommand cmd = new SqlCommand("exec [dbo].[ARCTHD1-Post] @stt_rec,@IsHasPT1");
            cmd.Parameters.Add("@stt_rec", SqlDbType.VarChar, 50).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"];
            cmd.Parameters.Add("@IsHasPT1", SqlDbType.Int).Value = ispostck;
            StartUp.SysObj.ExcuteNonQuery(cmd);
        }

        private void btnViewPT1_Click(object sender, RoutedEventArgs e)
        {
            //Kiểm tra PT1 có tồn tại hay không? nếu không thì reset lại stt_rec_pt bằng rỗng
            if (string.IsNullOrEmpty(StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"].ToString().Trim()))
            {
                return;
            }
            if (StartUpTrans.DsTrans.Tables[0].DefaultView[0]["ma_ct_pt"].ToString().Trim().Equals("PT1"))
            {
                SqlCommand _cmd = new SqlCommand();
                _cmd.CommandText = "Select count(1) from ph41 WHERE stt_Rec = @stt_rec_pt";
                _cmd.Parameters.Add(new SqlParameter("@stt_rec_pt", SqlDbType.VarChar)).Value = StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"].ToString();
                if ((int)BindingSysObj.ExcuteScalar(_cmd) == 0)
                {
                    if (ExMessageBox.Show(693, StartUp.SysObj, "Phiếu thu không tồn tại, có xóa thông tin phiếu thu trên hóa đơn không?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                    {
                        _cmd = new SqlCommand();
                        _cmd.CommandText = "UPDATE ph21 Set stt_rec_pt = '', so_ct_pt = '', ma_ct_pt = '' WHERE stt_rec = @stt_rec_pt; ";
                        _cmd.CommandText += "UPDATE cttt20 Set tat_toan = 0, stt_rec_tt = '' WHERE stt_rec = @stt_rec";
                        _cmd.Parameters.Add(new SqlParameter("@stt_rec_pt", SqlDbType.VarChar)).Value = StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();
                        _cmd.Parameters.Add(new SqlParameter("@stt_rec", SqlDbType.VarChar)).Value = StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();
                        BindingSysObj.ExcuteNonQuery(_cmd);
                        StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"] = "";
                        StartUpTrans.DsTrans.Tables[0].DefaultView[0]["so_ct_pt"] = "";
                        StartUpTrans.DsTrans.Tables[0].DefaultView[0]["ma_ct_pt"] = "";
                    }
                }
                else
                    SysFunc.EditVoucherFromBrowse(BindingSysObj, "PT1", StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"].ToString(), System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), BindingSysObj.M_ProcessName);
            }
            else
            {
                SqlCommand _cmd = new SqlCommand();
                _cmd.CommandText = "Select count(1) from ph51 WHERE stt_Rec = @stt_rec_pt";
                _cmd.Parameters.Add(new SqlParameter("@stt_rec_pt", SqlDbType.VarChar)).Value = StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"].ToString();
                if ((int)BindingSysObj.ExcuteScalar(_cmd) == 0)
                {
                    if (ExMessageBox.Show(693, StartUp.SysObj, "Phiếu thu không tồn tại, có xóa thông tin phiếu thu trên hóa đơn không?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                    {
                        _cmd = new SqlCommand();
                        _cmd.CommandText = "UPDATE ph21 Set stt_rec_pt = '', so_ct_pt = '', ma_ct_pt = '' WHERE stt_rec = @stt_rec_pt; ";
                        _cmd.CommandText += "UPDATE cttt20 Set tat_toan = 0, stt_rec_tt = '' WHERE stt_rec = @stt_rec";
                        _cmd.Parameters.Add(new SqlParameter("@stt_rec_pt", SqlDbType.VarChar)).Value = StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();
                        _cmd.Parameters.Add(new SqlParameter("@stt_rec", SqlDbType.VarChar)).Value = StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();
                        BindingSysObj.ExcuteNonQuery(_cmd);
                        StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"] = "";
                        StartUpTrans.DsTrans.Tables[0].DefaultView[0]["so_ct_pt"] = "";
                        StartUpTrans.DsTrans.Tables[0].DefaultView[0]["ma_ct_pt"] = "";
                    }
                }
                else
                    SysFunc.EditVoucherFromBrowse(BindingSysObj, "BC1", StartUpTrans.DsTrans.Tables[0].DefaultView[0]["stt_rec_pt"].ToString(), System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), BindingSysObj.M_ProcessName);
            }
        }
    }
}
