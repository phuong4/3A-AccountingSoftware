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
using Sm.Windows.Controls.ControlLib;
using System.Windows.Threading;
using System.Data.SqlClient;
using SysLib;
using ArapLib;
using System.Threading;

namespace CACTBN1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FrmCACTBN1 : SmVoucherLib.FormTrans, IPhValue
    {
        public static int iRow = 0;
        public int iOldRow = 0;

        public static CodeValueBindingObject IsInEditModeThue;
        public static CodeValueBindingObject IsInEditMode;

        CodeValueBindingObject Voucher_Ma_nt0;
        CodeValueBindingObject Voucher_Ma_nt;
        CodeValueBindingObject Voucher_Lan0;

        CodeValueBindingObject IsCheckedSua_tggs;
        CodeValueBindingObject IsCheckedSua_tien;
        CodeValueBindingObject Ty_Gia_ValueChange;
        CodeValueBindingObject Loai_tg;
        CodeValueBindingObject Ip_tien_hd;

        public static CodeValueBindingObject Ma_GD_Value;

        //Lưu lại dữ liệu khi thêm sửa
        private DataSet DsVitual;
        private DataSet dsCheckData;

        string stt_rec_magd1 = string.Empty;//dung cho ma_gd = 1 khi chon hoa don

        public FrmCACTBN1()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(FormTrans_Loaded);

            this.BindingSysObj = StartUp.SysObj;
            C_QS = txtMa_qs;
            C_NgayHT = txtNgay_ct;
            C_Ma_nt = cbMa_nt;
            C_So_ct = txtSo_ct;

            if (StartUp.SysObj.VersionInfo.Rows[0]["product_code"].ToString().Equals("FK") || (StartUp.dtRegInfo != null && StartUp.dtRegInfo.Rows[18]["content"].ToString().Trim().Equals("FK")))
            {
                btnSoHD.Visibility = Visibility.Collapsed;
                lblSo_ct_tt.Visibility = Visibility.Collapsed;
                ChkSua_tggs.Visibility = Visibility.Collapsed;
            }
        }

        #region load form
        void FormTrans_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                currActionTask = ActionTask.View;
                if (PhData.Rows.Count > 1)
                    iRow = PhData.Rows.Count - 1;
                IsInEditMode = (CodeValueBindingObject)FormMain.FindResource("IsInEditMode");
                Voucher_Ma_nt0 = (CodeValueBindingObject)FormMain.FindResource("Voucher_Ma_nt0");
                Voucher_Ma_nt = (CodeValueBindingObject)FormMain.FindResource("Voucher_Ma_nt");
                Voucher_Lan0 = (CodeValueBindingObject)FormMain.FindResource("Voucher_Lan0");

                IsCheckedSua_tggs = (CodeValueBindingObject)FormMain.FindResource("IsCheckedSua_tggs");
                IsCheckedSua_tien = (CodeValueBindingObject)FormMain.FindResource("IsCheckedSua_tien");
                IsInEditModeThue = (CodeValueBindingObject)FormMain.FindResource("IsInEditModeThue");
                Ty_Gia_ValueChange = (CodeValueBindingObject)FormMain.FindResource("Ty_Gia_ValueChange");
                Ma_GD_Value = (CodeValueBindingObject)FormMain.FindResource("Ma_GD_Value");
                Loai_tg = (CodeValueBindingObject)FormMain.FindResource("Loai_tg");
                Ip_tien_hd = (CodeValueBindingObject)FormMain.FindResource("Ip_tien_hd");
                Ip_tien_hd.Text = StartUp.M_IP_TIEN_HD;

                
                Binding bind = new Binding("Value");
                bind.Source = IsInEditMode;
                bind.Mode = BindingMode.OneWay;
                this.SetBinding(FormTrans.IsEditModeProperty, bind);


                //Gán ngôn ngữ messagebox
                M_LAN = StartUp.SysObj.GetOption("M_LAN").ToString();
                GrdCt.Lan = M_LAN;
                GrdCtgt.Lan = M_LAN;
                Voucher_Lan0.Value = M_LAN.Equals("V");
                tblGhi_chu.Text = M_LAN.Equals("V") ? "Ghi chú" : "Note";
                //Them cac truong tu do
                SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(this.BindingSysObj, Grdhd, StartUp.Ma_ct, 1);
                SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(this.BindingSysObj, GrdCt, StartUp.Ma_ct, 1);
                SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(this.BindingSysObj, GrdCtChi, StartUp.Ma_ct, 1);
                SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(this.BindingSysObj, GrdCtgt, StartUp.Ma_ct, 2);
                if (PhData.Rows.Count > 0)
                {
                    PhView.RowFilter = "stt_rec= '" + Stt_rec + "'";
                    CtView.RowFilter = "stt_rec= '" + Stt_rec + "'";
                    StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + Stt_rec + "'";

                    GrdLayout00.DataContext = PhView;

                    this.GrdCt.DataSource = CtView;
                    this.GrdCtChi.DataSource = CtView;
                    this.Grdhd.DataSource = CtView;

                    this.GrdCtgt.DataSource = StartUp.DsTrans.Tables[2].DefaultView;

                    txtStatus.ItemsSource = StartUp.tbStatus.DefaultView;

                    if (StartUp.tbStatus.DefaultView.Count == 1)
                    {
                        txtStatus.IsEnabled = false;
                    }
                    //Xét lại các Field khi thay đổi record hiển thị
                    PhView.ListChanged += new System.ComponentModel.ListChangedEventHandler(phValue_ListChanged);
                    CtView.ListChanged += new System.ComponentModel.ListChangedEventHandler(DefaultView_ListChanged);

                    IsCheckedSua_tggs.Value = (PhView[0]["sua_tggs"].ToString() == "1");
                    IsCheckedSua_tien.Value = (PhView[0]["sua_tien"].ToString() == "1");
                    Ty_Gia_ValueChange.Value = true;
                    Loai_tg.Text = PhView[0]["loai_tg"].ToString();
                }

                Voucher_Ma_nt0.Text = PhView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (PhView[0]["ma_nt"].ToString().Equals(M_Ma_nt0));

                Ma_GD_Value.Text = PhView[0]["ma_gd"].ToString();
                if (txtMa_gd.Text.Equals("8"))
                    Ma_GD_Value.Value = true;
                else
                    Ma_GD_Value.Value = false;

                if (IsInEditMode.Value && Ma_GD_Value.Value)
                    IsInEditModeThue.Value = (ChkSuaThue.IsChecked.Value);
                else
                    IsInEditModeThue.Value = false;

                SetStatusVisibleField();

                //LoadDataDu13();
                SetFocusToolbar();
                if (Grdhd.FieldLayouts[0].Fields.Any(x => x.Name == "ma_vv_i"))
                {
                    Grdhd.FieldLayouts[0].Fields["ma_vv_i"].Settings.AllowEdit = false;
                    Grdhd.FieldLayouts[0].Fields["ma_vv_i"].Settings.EditorStyle= null;
                }
                if (Grdhd.FieldLayouts[0].Fields.Any(x => x.Name == "ma_phi_i"))
                {
                    Grdhd.FieldLayouts[0].Fields["ma_phi_i"].Settings.AllowEdit = false;
                    Grdhd.FieldLayouts[0].Fields["ma_phi_i"].Settings.EditorStyle = null;
                }

                Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    TabInfo_SelectionChanged(null, null);
                }));
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        void phValue_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            IsCheckedSua_tggs.Value = (PhView[0]["sua_tggs"].ToString() == "1");
        }

        void DefaultView_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            SetStatusVisibleField();
        }

        #endregion

        private void V_Truoc()
        {
            if (iRow > 1)
            {
                iRow--;
                PhView.RowFilter = "stt_rec= '" + Stt_rec + "'";
                CtView.RowFilter = "stt_rec= '" + Stt_rec + "'";
                StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + Stt_rec + "'";
                //134088444
                //if (TabInfo.SelectedIndex == 0)
                //    UpdateTotalHT();
                //else
                //    UpdateTotalThue();
              
            }
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => UpdateTotalThue()));
        }

        private void V_Sau()
        {
            if (iRow < PhData.Rows.Count - 1)
            {
                iRow++;
                PhView.RowFilter = "stt_rec= '" + Stt_rec + "'";
                CtView.RowFilter = "stt_rec= '" + Stt_rec + "'";
                StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + Stt_rec + "'";
                //134088444
                //if (TabInfo.SelectedIndex == 0)
                //    UpdateTotalHT();
                //else
                //    UpdateTotalThue();
              
            }
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => UpdateTotalThue()));
        }

        private void V_Dau()
        {
            if (PhData.Rows.Count >= 2)
            {
                iRow = 1;
            }
            else
                iRow = 0;
            PhView.RowFilter = "stt_rec= '" + Stt_rec + "'";
            CtView.RowFilter = "stt_rec= '" + Stt_rec + "'";
            StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + Stt_rec + "'";
            //134088444
            //if (TabInfo.SelectedIndex == 0)
            //    UpdateTotalHT();
            //else
            //    UpdateTotalThue();
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => UpdateTotalThue()));
        }

        private void V_Cuoi()
        {
            iRow = PhData.Rows.Count - 1;
            PhView.RowFilter = "stt_rec= '" + Stt_rec + "'";
            CtView.RowFilter = "stt_rec= '" + Stt_rec + "'";
            StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + Stt_rec + "'";
            //134088444
            //if (TabInfo.SelectedIndex == 0)
            //    UpdateTotalHT();
            //else
            //    UpdateTotalThue();
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => UpdateTotalThue()));
        }

        private void V_Huy()
        {
            IsInEditMode.Value = false;
            if (DsVitual != null && PhData.Rows.Count > 0)
            {
                switch (currActionTask)
                {
                    case ActionTask.Edit:
                        {
                            currActionTask = ActionTask.View;
                            //xóa các row trong table[1], table[2]
                            string stt_rec = PhView[0]["stt_rec"].ToString();

                            // Nên dịch chuyển iRow lùi dòng 0
                            // Sau đó RowFilter lại Table[0], Table[1], Table[2]
                            PhView.RowFilter = "stt_rec= '" + PhData.Rows[0]["stt_rec"].ToString() + "'";
                            CtView.RowFilter = "stt_rec= '" + PhData.Rows[0]["stt_rec"].ToString() + "'";
                            StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + PhData.Rows[0]["stt_rec"].ToString() + "'";
                            //Refresh lại grid hạch toán
                            if (CtData.Rows.Count > 0)
                            {
                                //lấy các rowfilter trong grdct
                                DataRow[] _row = CtData.Select("stt_rec='" + stt_rec + "'");
                                foreach (DataRow dr in _row)
                                {
                                    //delete các row có trong grdct
                                    CtData.Rows.Remove(dr);
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
                            PhData.Rows.RemoveAt(iRow);

                            DataRow rowPh = PhData.NewRow();
                            rowPh.ItemArray = DsVitual.Tables[0].Rows[0].ItemArray;
                            PhData.Rows.InsertAt(rowPh, iRow);

                            PhView.RowFilter = "stt_rec= '" + stt_rec + "'";
                            CtView.RowFilter = "stt_rec= '" + stt_rec + "'";
                            StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";

                            CtData.Merge(DsVitual.Tables[1]);
                            StartUp.DsTrans.Tables[2].Merge(DsVitual.Tables[2]);
                            
                            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                if (Ma_GD_Value.Text.Equals("1"))
                                {
                                    string stt_rec_tt, ma_nt, ma_ct;
                                    decimal tt_qd, tt_dn, tt_dn_nt;

                                    stt_rec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();
                                    ma_nt = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                                    ma_ct = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_ct"].ToString();
                                    int user_id = (int)StartUp.SysObj.UserInfo.Rows[0]["user_id"];


                                    for (int i = 0; i < Grdhd.Records.Count; i++)
                                    {
                                        stt_rec_tt = (Grdhd.Records[i] as DataRecord).Cells["stt_rec_tt"].Value.ToString();
                                        decimal.TryParse((Grdhd.Records[i] as DataRecord).Cells["tt_qd"].Value.ToString(), out tt_qd);
                                        decimal.TryParse((Grdhd.Records[i] as DataRecord).Cells["tien"].Value.ToString(), out tt_dn);
                                        decimal.TryParse((Grdhd.Records[i] as DataRecord).Cells["tien_nt"].Value.ToString(), out tt_dn_nt);
                                        SqlCommand cmd = new SqlCommand();
                                        cmd.CommandText = "Exec [CACTPC1-IncludeVoucher] @Stt_rec, @Stt_rec_tt, @Ma_nt, @Tt_qd, @Tt_dn, @Tt_dn_nt";
                                        cmd.Parameters.Add("@Stt_rec", SqlDbType.VarChar).Value = stt_rec;
                                        cmd.Parameters.Add("@Stt_rec_tt", SqlDbType.VarChar).Value = stt_rec_tt;
                                        cmd.Parameters.Add("@Ma_nt", SqlDbType.VarChar).Value = ma_nt;
                                        cmd.Parameters.Add("@Tt_qd", SqlDbType.Decimal).Value = tt_qd;
                                        cmd.Parameters.Add("@Tt_dn", SqlDbType.Decimal).Value = tt_dn;
                                        cmd.Parameters.Add("@Tt_dn_nt", SqlDbType.Decimal).Value = tt_dn_nt;
                                        cmd.Parameters.Add("@User_id", SqlDbType.Int).Value = user_id;
                                        StartUp.SysObj.ExcuteNonQuery(cmd);

                                        cmd = new SqlCommand();
                                        cmd.CommandText = "EXEC Apttpb;70 @Stt_rec, @Ma_ct";
                                        cmd.Parameters.Add("@Stt_rec", SqlDbType.VarChar).Value = stt_rec;
                                        cmd.Parameters.Add("@Ma_ct", SqlDbType.VarChar).Value = ma_ct;
                                        StartUp.SysObj.ExcuteNonQuery(cmd);
                                    }
                                }
                            }));
                        }
                        break;
                    //Refresh lại khi chọn new
                    case ActionTask.Copy:
                    case ActionTask.Add:
                        {
                            V_Xoa();
                            if (PhData.Rows.Count > 0)
                            {
                                // iRow = 0;
                                iRow = iOldRow;
                                //load lại form theo stt_rec
                                PhView.RowFilter = "stt_rec= '" + Stt_rec + "'";
                                CtView.RowFilter = "stt_rec= '" + Stt_rec + "'";
                                StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + Stt_rec + "'";
                            }
                        }
                        break;
                }
            }
        }

        private void V_Xoa()
        {
            if (string.IsNullOrEmpty(PhView[0]["stt_rec"].ToString().Trim()))
                return;
            currActionTask = ActionTask.Delete;
            try
            {
                string _stt_rec = PhView[0]["stt_rec"].ToString();
                string _ma_gd = this.Ma_gd;
                string _ma_nt = this.Ma_nt;

                //xóa trong ph, ct, ctgt
                //xóa chứng từ
                StartUp.DeleteVoucher(_stt_rec);

                // ----Warning : Không nên xóa Table[0] trước, nếu xóa trước sẽ bị mất Binding -----------------------
                // Nên dịch chuyển iRow lùi dòng 0
                // Sau đó RowFilter lại Table[0], Table[1], Table[2]
                // Rồi mới xóa Table[0]
                PhView.RowFilter = "stt_rec= '" + PhData.Rows[0]["stt_rec"].ToString() + "'";
                CtView.RowFilter = "stt_rec= '" + PhData.Rows[0]["stt_rec"].ToString() + "'";
                StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + PhData.Rows[0]["stt_rec"].ToString() + "'";

                //Xóa row table[0]
                PhData.Rows.RemoveAt(iRow);

                //xóa các row trong Table[1]
                if (CtData.Rows.Count > 0)
                {
                    DataRow[] rows = CtData.Select("stt_rec='" + _stt_rec + "'");
                    if (_ma_gd == "1")
                    {
                        foreach (DataRow dr in rows)
                        {
                            SqlCommand cmd = new SqlCommand("[CACTPC1-Post];35");
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@Stt_rec", SqlDbType.NVarChar).Value = _stt_rec;
                            cmd.Parameters.Add("@Stt_rec_tt", SqlDbType.NVarChar).Value = dr["stt_rec_tt"];
                            cmd.Parameters.Add("@Ma_nt", SqlDbType.NVarChar).Value = _ma_nt;
                            cmd.Parameters.Add("@Tt_qd", SqlDbType.Decimal).Value = dr["tt_qd"];
                            cmd.Parameters.Add("@Tt_dn", SqlDbType.Decimal).Value = _ma_nt.Equals(StartUp.M_ma_nt0) ? dr["tien_nt"] : dr["tien"];
                            cmd.Parameters.Add("@Tt_dn_nt", SqlDbType.Decimal).Value = dr["tien_nt"];

                            DataSet ds = SysO.ExcuteReader(cmd);
                        }
                    } 
                    
                    foreach (DataRow dr in rows)
                    {
                        CtData.Rows.Remove(dr);
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
                if (PhData.Rows.Count > 0)
                {
                    // iRow = 0;
                    iRow = iRow > PhData.Rows.Count - 1 ? iRow - 1 : iRow;
                    //load lại form theo stt_rec
                    PhView.RowFilter = "stt_rec= '" + Stt_rec + "'";
                    CtView.RowFilter = "stt_rec= '" + Stt_rec + "'";
                    StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + Stt_rec + "'";
                }
                UpdateTotalHT();
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
            currActionTask = ActionTask.View;
        }

        private void V_In()
        {
            try
            {
                tiHT.Focus();
                StartUp.In();
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        private void V_Copy()
        {
            if (string.IsNullOrEmpty(PhView[0]["stt_rec"].ToString().Trim()))
                return;
            currActionTask = ActionTask.Copy;
            FrmCACTBN1Copy _formcopy = new FrmCACTBN1Copy();
            _formcopy.Closed += new EventHandler(_formcopy_Closed);
            _formcopy.ShowDialog();
        }

        void _formcopy_Closed(object sender, EventArgs e)
        {
            if ((sender as FrmCACTBN1Copy).isCopy == true)
            {
                string newSttRec = DataProvider.NewTrans(StartUp.SysObj, StartUp.Ma_ct, StartUp.Ws_Id);
                if (!string.IsNullOrEmpty(newSttRec))
                {
                    DsVitual = StartUp.DsTrans.Copy();
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        txtMa_kh.IsFocus = true;
                    }));
                    //Them moi dong trong Ph
                    DataRow NewRecord = PhData.NewRow();
                    //copy dữ liệu từ row được chọn copy cho row mới
                    NewRecord.ItemArray = PhData.Rows[iRow].ItemArray;
                    //gán lại stt_rec, ngày ct
                    NewRecord["stt_rec"] = newSttRec;
                    NewRecord["ngay_ct"] = FrmCACTBN1Copy.ngay_ct;
                    //NewRecord["ngay_lct"] = FrmCACTBN1Copy.ngay_ct;

                    NewRecord["ma_qs"] = GetDMQS(BindingSysObj, StartUp.Ma_ct, Convert.ToDateTime(NewRecord["ngay_ct"]),
                             StartUp.M_User_Id, NewRecord["ma_qs"].ToString().Trim());
                    if (NewRecord["ma_qs"].ToString().Trim() != "")
                        NewRecord["so_ct"] = GetNewSoct(StartUp.SysObj, NewRecord["ma_qs"].ToString());
                    else
                        NewRecord["so_ct"] = "";

                    if (NewRecord["ma_nt"].ToString().Trim().Equals(M_Ma_nt0))
                    {
                        NewRecord["ty_giaf"] = 1;
                    }
                    else
                    {
                        NewRecord["ty_giaf"] = (currActionTask == ActionTask.Copy ? NewRecord["ty_giaf"] : StartUp.GetRates(NewRecord["ma_nt"].ToString().Trim(), Convert.ToDateTime(NewRecord["ngay_ct"]).Date));
                    }

                    NewRecord["so_cttmp"] = NewRecord["so_ct"];
                    NewRecord["so_ct_tt"] = "";
                    NewRecord["sua_thue"] = 0;
                    PhData.Rows.Add(NewRecord);

                    //add các row trong GrdCt
                    if (CtView.Count > 0)
                    {
                        //lấy các rowfilter trong grdct
                        DataRow[] _row = CtData.Select("stt_rec='" + PhView[0]["stt_rec"].ToString() + "'");
                        DataRow NewCtRecord;
                        foreach (DataRow dr in _row)
                        {
                            //add 
                            NewCtRecord = CtData.NewRow();
                            NewCtRecord.ItemArray = dr.ItemArray;
                            NewCtRecord["stt_rec"] = newSttRec;
                            CtData.Rows.Add(NewCtRecord);
                        }
                    }

                    DataRow NewCtgtRecord;
                    NewCtgtRecord = StartUp.DsTrans.Tables[2].NewRow();
                    NewCtgtRecord["stt_rec"] = newSttRec;
                    StartUp.DsTrans.Tables[2].Rows.Add(NewCtgtRecord);
                    //add các row trong GrdCtgt
                    //if (StartUp.DsTrans.Tables[2].DefaultView.Count > 0)
                    //{
                    //    //lấy các rowfilter trong grdctgt
                    //    DataRow[] _row = StartUp.DsTrans.Tables[2].Select("stt_rec='" + PhView[0]["stt_rec"].ToString() + "'");
                    //    DataRow NewCtgtRecord;
                    //    foreach (DataRow dr in _row)
                    //    {
                    //        //add 
                    //        NewCtgtRecord = StartUp.DsTrans.Tables[2].NewRow();
                    //        NewCtgtRecord.ItemArray = dr.ItemArray;
                    //        NewCtgtRecord["stt_rec"] = newSttRec;
                    //        StartUp.DsTrans.Tables[2].Rows.Add(NewCtgtRecord);
                    //    }
                    //}
                    iOldRow = iRow;
                    iRow = PhData.Rows.Count - 1;
                    //load lại form
                    PhView.RowFilter = "stt_rec= '" + newSttRec + "'";
                    CtView.RowFilter = "stt_rec= '" + newSttRec + "'";                    
                    StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";

                    IsInEditMode.Value = true;

                    SetStatusVisibleField();
                }
            }
        }

        void NewRowCt()
        {
            DataRow NewRecord = CtData.NewRow();
            NewRecord["stt_rec"] = PhView[0]["stt_rec"];
            int Stt_rec0 = 0, Stt_rec0ct = 0, Stt_rec0ctgt = 0;
            if (GrdCt.Records.Count > 0)
            {
                var _max_sttrec0ct = CtData.AsEnumerable()
                                   .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                                   .Max(x => x.Field<string>("stt_rec0"));
                if (_max_sttrec0ct != null)
                    int.TryParse(_max_sttrec0ct.ToString(), out Stt_rec0ct);
            }
            if (GrdCtgt.Records.Count > 0)
            {
                var _max_sttrec0ctgt = StartUp.DsTrans.Tables[2].AsEnumerable()
                               .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                               .Max(x => x.Field<string>("stt_rec0"));
                if (_max_sttrec0ctgt != null)
                    int.TryParse(_max_sttrec0ctgt.ToString(), out Stt_rec0ctgt);
            }
            Stt_rec0 = Stt_rec0ct >= Stt_rec0ctgt ? Stt_rec0ct : Stt_rec0ctgt;
            Stt_rec0++;

            NewRecord["stt_rec0"] = string.Format("{0:000}", Stt_rec0);
            NewRecord["loai_hd"] = 0;
            NewRecord["tien_nt"] = 0;
            NewRecord["tien"] = 0;
            NewRecord["thue_nt"] = 0;
            NewRecord["thue"] = 0;
            NewRecord["ma_ms"] = StartUp.SysObj.GetOption("M_MA_MS");
                     
            int count = CtView.Count;
            if (count > 0)
            {
                NewRecord["dien_giaii"] = CtView[count - 1].Row["dien_giaii"];
                NewRecord["ty_giahtf2"] = CtView[CtView.Count - 1]["ty_giahtf2"];
                NewRecord["ty_gia_ht2"] = CtView[CtView.Count - 1]["ty_gia_ht2"];
                //NewRecord["ma_kh_t"] = CtView[CtView.Count - 1]["ma_kh_t"];
                //NewRecord["ten_kh_t"] = CtView[CtView.Count - 1]["ten_kh_t"];
            }
            else
            {
                NewRecord["dien_giaii"] = PhView[0].Row["dien_giai"];
                NewRecord["ty_giahtf2"] = 0;
                NewRecord["ty_gia_ht2"] = 0;
            }
            //if (CtView.Count == 0)
            //    NewRecord["ty_gia_ht2"] = 0;
            //else
            //    NewRecord["ty_gia_ht2"] = CtView[CtView.Count - 1]["ty_gia_ht2"];
            FreeCodeFieldLib.CarryFreeCodeFields(StartUp.SysObj, StartUp.Ma_ct, CtView, NewRecord, 1);
            CtData.Rows.Add(NewRecord);

        }

        void NewRowCtChi()
        {
            DataRow NewRecord = CtData.NewRow();
            NewRecord["stt_rec"] = PhView[0]["stt_rec"];
            int Stt_rec0 = 0, Stt_rec0ct = 0, Stt_rec0ctgt = 0;
            if (GrdCtChi.Records.Count > 0)
            {
                var _max_sttrec0ct = CtData.AsEnumerable()
                                   .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                                   .Max(x => x.Field<string>("stt_rec0"));
                if (_max_sttrec0ct != null)
                    int.TryParse(_max_sttrec0ct.ToString(), out Stt_rec0ct);
            }
            if (GrdCtgt.Records.Count > 0)
            {
                var _max_sttrec0ctgt = StartUp.DsTrans.Tables[2].AsEnumerable()
                               .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                               .Max(x => x.Field<string>("stt_rec0"));
                if (_max_sttrec0ctgt != null)
                    int.TryParse(_max_sttrec0ctgt.ToString(), out Stt_rec0ctgt);
            }
            Stt_rec0 = Stt_rec0ct >= Stt_rec0ctgt ? Stt_rec0ct : Stt_rec0ctgt;
            Stt_rec0++;

            NewRecord["stt_rec0"] = string.Format("{0:000}", Stt_rec0);
            NewRecord["tien_nt"] = 0;
            NewRecord["tien"] = 0;
            NewRecord["thue_nt"] = 0;
            NewRecord["thue"] = 0;
            int count = CtView.Count;
            if (count > 0)
            {
                NewRecord["dien_giaii"] = CtView[count - 1].Row["dien_giaii"];
                //NewRecord["ty_gia_ht2"] = CtView[CtView.Count - 1]["ty_gia_ht2"];
            }
            else
            {
                NewRecord["dien_giaii"] = PhView[0].Row["dien_giai"];
                NewRecord["ty_giahtf2"] = 0;
                NewRecord["ty_gia_ht2"] = 0;
            }
            //if (CtView.Count == 0)
            //    NewRecord["ty_gia_ht2"] = 0;
            //else
            //    NewRecord["ty_gia_ht2"] = CtView[CtView.Count - 1]["ty_gia_ht2"];
            FreeCodeFieldLib.CarryFreeCodeFields(StartUp.SysObj, StartUp.Ma_ct, CtView, NewRecord, 1);
            CtData.Rows.Add(NewRecord);
        }

        private bool GrdCtChi_AddNewRecord(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            NewRowCtChi();
            return true;
        }

        private bool GrdCt_AddNewRecord(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            NewRowCt();
            return true;
        }
        private DataRecord CalculateHd2(DataRecord record)
        {
            if (record.Cells["thue_suat"].Value != DBNull.Value)
            {
                Decimal _tien_nt = 0, _thue_suat, _tt_nt = 0, _ty_gia = 0, _tien_nt0 = 0;
                Decimal.TryParse(record.Cells["thue_suat"].Value.ToString(), out _thue_suat);
                Decimal.TryParse(record.Cells["tt_nt"].Value.ToString(), out _tt_nt);
                _ty_gia = txtTy_gia.nValue;
                _tien_nt = (_tt_nt / (1 + (_thue_suat / 100)));
                if (!cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
                {
                    _tien_nt = SmLib.SysFunc.Round(_tien_nt, M_Round_nt);
                }
                else
                {
                    _tien_nt = SmLib.SysFunc.Round(_tien_nt, M_Round);
                }
                record.Cells["tien_nt"].Value = _tien_nt;
                record.Cells["thue_nt"].Value = _tt_nt - _tien_nt;
                if (!cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
                {
                    _tien_nt0 = SmLib.SysFunc.Round(_tien_nt * _ty_gia, M_Round);
                    //if (_tien_nt0 > 0)
                    {
                        Decimal _thue_nt0 = 0, _tt = 0, _t_Lech = 0, _tien_Lech = 0, _thue_lech = 0;
                        _thue_nt0 = SmLib.SysFunc.Round(_tien_nt0 * _thue_suat / 100, M_Round);

                        _tt = SmLib.SysFunc.Round(_tt_nt * _ty_gia, M_Round);
                        _t_Lech = (_tt - (_tien_nt0 + _thue_nt0));
                        _tien_Lech = SmLib.SysFunc.Round((_t_Lech / (1 + (_thue_suat / 100))), M_Round);
                        _thue_lech = _t_Lech - _tien_Lech;

                        record.Cells["tien"].Value = _tien_nt0 + _tien_Lech;
                        record.Cells["thue"].Value = _thue_nt0 + _thue_lech;
                        record.Cells["tt"].Value = _tt;
                    }
                }
                else
                {
                    record.Cells["tien"].Value = _tien_nt;
                    record.Cells["thue"].Value = _tt_nt - _tien_nt;
                    record.Cells["tt"].Value = _tt_nt;
                }
            }
            else
            {
                Decimal _tt_nt = 0, thue_nt = 0, _tien_nt = 0;
                Decimal.TryParse(record.Cells["tt_nt"].Value.ToString(), out _tt_nt);
                if (record.Cells["thue_nt"].Value != DBNull.Value)
                    Decimal.TryParse(record.Cells["thue_nt"].Value.ToString(), out thue_nt);
                _tien_nt = _tt_nt - thue_nt;
                record.Cells["tien_nt"].Value = _tien_nt;
                if (!cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
                {
                    Decimal _Ty_gia = 0;
                    _Ty_gia = txtTy_gia.nValue;

                    Decimal tien = SmLib.SysFunc.Round(_Ty_gia * _tien_nt, M_Round);
                    //if (tien > 0)
                    {
                        Decimal thue_nt0 = 0;
                        record.Cells["tien"].Value = tien;
                        if (record.Cells["thue"].Value != DBNull.Value)
                            Decimal.TryParse(record.Cells["thue"].Value.ToString(), out thue_nt0);
                        record.Cells["tt"].Value = tien + thue_nt0;
                    }
                }
                else
                {
                    record.Cells["tien"].Value = _tien_nt;
                    record.Cells["thue"].Value = thue_nt;
                    record.Cells["tt"].Value = _tt_nt;

                }
            }
            return record;
        }

        private void GrdCtChi_PreviewEditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            try
            {
                //Kiểm tra activecell khác null và
                if (IsInEditMode.Value && GrdCtChi.ActiveCell != null && CtView.Count > GrdCtChi.ActiveRecord.Index && CtData.GetChanges(DataRowState.Deleted) == null)
                    switch (e.Cell.Field.Name)
                    {
                        case "tk_i":
                            {
                                AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                if (txt.Text == "")
                                    break;

                                //if (txt.RowResult == null)
                                //    txt.SearchInit();
                                if (txt.RowResult != null)
                                {
                                    //gán tên tài khoản
                                        e.Cell.Record.Cells["ten_tk"].Value = txt.RowResult["ten_tk"];                                   
                                        e.Cell.Record.Cells["ten_tk2"].Value = txt.RowResult["ten_tk2"];

                                    if (e.Cell.Record.Index == 0)
                                    {
                                        if (string.IsNullOrEmpty(e.Cell.Record.Cells["dien_giaii"].Value.ToString()))
                                        {
                                            e.Cell.Record.Cells["dien_giaii"].Value = PhView[0]["dien_giai"].ToString();
                                        }
                                    }
                                }
                                break;
                            }
                        case "ma_kh_i":
                            {
                                AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                if (txt.RowResult != null && txt.IsDataChanged)
                                {
                                    //gán tên tài khoản                                    
                                        e.Cell.Record.Cells["ten_kh_i"].Value = txt.RowResult["ten_kh"];                                    
                                        e.Cell.Record.Cells["ten_kh2_i"].Value = txt.RowResult["ten_kh2"];
                                }
                                break;
                            }
                        case "tien_nt":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (!string.IsNullOrEmpty(e.Editor.Text))
                                    {
                                        e.Cell.Record.Cells["tt_nt"].Value = e.Editor.Text;
                                        if (GrdCtChi.FieldLayouts[0].Fields["ty_giahtf2"].Visibility == Visibility.Visible)
                                        {
                                            Decimal _ty_gia = 0, _tien_nt0, _tien_nt;
                                            Decimal.TryParse((e.Cell.Record.DataItem as DataRowView)["ty_gia_ht2"].ToString(), out _ty_gia);
                                            if (decimal.TryParse(e.Editor.Text, out _tien_nt))
                                            {
                                                _tien_nt0 = SmLib.SysFunc.Round(_tien_nt * _ty_gia, M_Round);
                                                //if (_tien_nt0 > 0)
                                                {
                                                    e.Cell.Record.Cells["tien"].Value = _tien_nt0;
                                                    e.Cell.Record.Cells["tt"].Value = _tien_nt0;
                                                }
                                                //tiền thanh toán
                                                if (Decimal.TryParse(txtTy_gia.nValue.ToString(), out _ty_gia))
                                                {
                                                    _tien_nt0 = SmLib.SysFunc.Round(_tien_nt * _ty_gia, M_Round);
                                                   // if (_tien_nt0 > 0)
                                                    {
                                                        e.Cell.Record.Cells["tien_tt"].Value = _tien_nt0;
                                                    }
                                                }
                                            }
                                        }
                                        else if (txtTy_gia.Value != null)
                                        {
                                            Decimal _ty_gia = 0, _tien_nt0, _tien_nt;
                                            Decimal.TryParse(txtTy_gia.nValue.ToString(), out _ty_gia);
                                            if (decimal.TryParse(e.Editor.Text, out _tien_nt))
                                            {
                                                _tien_nt0 = SmLib.SysFunc.Round(_tien_nt * _ty_gia, M_Round);
                                                if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                                {
                                                    e.Cell.Record.Cells["tien"].Value = e.Cell.Record.Cells["tien_nt"].Value;
                                                    e.Cell.Record.Cells["tien_tt"].Value = e.Cell.Record.Cells["tien_nt"].Value;
                                                    e.Cell.Record.Cells["tt"].Value = e.Cell.Record.Cells["tien_nt"].Value;
                                                }
                                                else
                                                {
                                                    //if (_tien_nt0 != 0)
                                                    {
                                                        e.Cell.Record.Cells["tien"].Value = _tien_nt0;
                                                        e.Cell.Record.Cells["tien_tt"].Value = _tien_nt0;
                                                        e.Cell.Record.Cells["tt"].Value = _tien_nt0;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    UpdateTotalHT();
                                }
                            }
                            break;
                        case "ty_giahtf2":
                            if (e.Cell.IsDataChanged)
                            {
                                if (string.IsNullOrEmpty(e.Editor.Text))
                                {
                                    e.Editor.Value = 0;
                                }
                                Decimal _ty_gia = 0, _tien_nt0, _tien_nt;
                                Decimal.TryParse(((e.Cell.Record as DataRecord).DataItem as DataRowView)["ty_gia_ht2"].ToString(), out _ty_gia);
                                if (decimal.TryParse(e.Cell.Record.Cells["tien_nt"].Value.ToString(), out _tien_nt))
                                {
                                    _tien_nt0 = SmLib.SysFunc.Round(_tien_nt * _ty_gia, M_Round);
                                   // if (_tien_nt0 != 0)
                                    {
                                        e.Cell.Record.Cells["tien"].Value = _tien_nt0;
                                        e.Cell.Record.Cells["tt"].Value = _tien_nt0;

                                        UpdateTotalHT();
                                    }
                                }
                            }
                            break;

                        case "tien_tt":
                        case "tien":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    e.Cell.Record.Cells["tt"].Value = e.Editor.Value;
                                    UpdateTotalHT();
                                }
                            }
                            break;
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
                    }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        private void GrdCt_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            try
            {
                //Kiểm tra activecell khác null và
                if (IsInEditMode.Value && GrdCt.ActiveCell != null && CtView.Count > GrdCt.ActiveRecord.Index && CtData.GetChanges(DataRowState.Deleted) == null)
                    switch (e.Cell.Field.Name)
                    {
                        case "tk_i":
                            {
                                AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                if (txt.RowResult != null)
                                {
                                   //gán tên tài khoản                                    
                                    e.Cell.Record.Cells["ten_tk"].Value = txt.RowResult["ten_tk"];                                    
                                    e.Cell.Record.Cells["ten_tk2"].Value = txt.RowResult["ten_tk2"];
                                    //if (e.Cell.Record.Cells["ma_kh_t"].Value == DBNull.Value || string.IsNullOrEmpty(e.Cell.Record.Cells["ma_kh_t"].Value.ToString()))
                                    //{
                                    //    if (e.Cell.Record.Index == 0)
                                    //    {
                                    //        e.Cell.Record.Cells["ma_kh_t"].Value = txtMa_kh.Text;
                                    //        e.Cell.Record.Cells["ten_kh_t"].Value = txtTen_kh.Text;
                                    //        e.Cell.Record.Cells["dia_chi_t"].Value = txtDia_chi.Text;
                                    //        e.Cell.Record.Cells["mst_t"].Value = txtMaSoThue.Text;
                                    //    }
                                    //    else
                                    //    {
                                    //        e.Cell.Record.Cells["ma_kh_t"].Value = (GrdCt.Records[e.Cell.Record.Index - 1] as DataRecord).Cells["ma_kh_t"].Value;
                                    //        e.Cell.Record.Cells["ten_kh_t"].Value = (GrdCt.Records[e.Cell.Record.Index - 1] as DataRecord).Cells["ten_kh_t"].Value;
                                    //        e.Cell.Record.Cells["dia_chi_t"].Value = (GrdCt.Records[e.Cell.Record.Index - 1] as DataRecord).Cells["dia_chi_t"].Value;
                                    //        e.Cell.Record.Cells["mst_t"].Value = (GrdCt.Records[e.Cell.Record.Index - 1] as DataRecord).Cells["mst_t"].Value;
                                    //    }
                                    //}
                                    ////set readonly cho ma khách c nợ
                                    CtView[GrdCt.ActiveRecord.Index]["tk_cn"] = txt.RowResult["tk_cn"];

                                    if (e.Cell.Record.Index == 0)
                                    {
                                        if (string.IsNullOrEmpty(e.Cell.Record.Cells["dien_giaii"].Value.ToString()))
                                        {
                                            e.Cell.Record.Cells["dien_giaii"].Value = PhView[0]["dien_giai"].ToString();
                                        }
                                    }

                                }
                                break;
                            }
                        case "tien_nt":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (txtTy_gia.Value != null && !string.IsNullOrEmpty(e.Editor.Text))
                                    {

                                        if (e.Cell.Record.Cells["loai_hd"].Value.ToString().Trim().Equals("2"))
                                        {
                                            NumericTextBox ntTienNt = e.Editor as NumericTextBox;

                                            if (e.Cell.IsDataChanged)
                                            {
                                                e.Cell.Record.Cells["tt_nt"].Value = e.Editor.Value;
                                                CalculateHd2(GrdCt.ActiveRecord as DataRecord);
                                            }
                                        }
                                        else if (!e.Cell.Record.Cells["loai_hd"].Value.ToString().Trim().Equals("5"))
                                        {
                                            Decimal _Tien_nt = 0;
                                            _Tien_nt = (e.Editor as NumericTextBox).nValue;

                                            if (e.Cell.Record.Cells["thue_suat"].Value != DBNull.Value)
                                            {
                                                Decimal _thue_suat, _thue_nt = 0;
                                                Decimal.TryParse(e.Cell.Record.Cells["thue_suat"].Value.ToString(), out _thue_suat);

                                                _thue_nt = _Tien_nt * _thue_suat / 100;
                                                if (!cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
                                                {
                                                    _thue_nt = SmLib.SysFunc.Round(_thue_nt, M_Round_nt);
                                                }
                                                else
                                                {
                                                    _thue_nt = SmLib.SysFunc.Round(_thue_nt, M_Round);
                                                }
                                                e.Cell.Record.Cells["thue_nt"].Value = _thue_nt;
                                                e.Cell.Record.Cells["tt_nt"].Value = _thue_nt + _Tien_nt;

                                                if (!cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
                                                {
                                                    Decimal _ty_gia = 0, _tien_nt0;
                                                    Decimal.TryParse(txtTy_gia.nValue.ToString(), out _ty_gia);

                                                    _tien_nt0 = SmLib.SysFunc.Round(_Tien_nt * _ty_gia, M_Round);
                                                    //if (_tien_nt0 > 0)
                                                    {
                                                        Decimal _thue_nt0 = 0;
                                                        e.Cell.Record.Cells["tien"].Value = _tien_nt0;
                                                        _thue_nt0 = SmLib.SysFunc.Round(_tien_nt0 * _thue_suat / 100, M_Round);
                                                        e.Cell.Record.Cells["thue"].Value = _thue_nt0;
                                                        e.Cell.Record.Cells["tt"].Value = _tien_nt0 + _thue_nt0;
                                                    }
                                                }
                                                else
                                                {
                                                    e.Cell.Record.Cells["tien"].Value = _Tien_nt;
                                                    e.Cell.Record.Cells["thue"].Value = _thue_nt;
                                                    e.Cell.Record.Cells["tt"].Value = _thue_nt + _Tien_nt;
                                                }
                                            }
                                            else
                                            {
                                                Decimal thue_nt = 0;
                                                if (e.Cell.Record.Cells["thue_nt"].Value != DBNull.Value)
                                                    Decimal.TryParse(e.Cell.Record.Cells["thue_nt"].Value.ToString(), out thue_nt);
                                                e.Cell.Record.Cells["tt_nt"].Value = _Tien_nt + thue_nt;
                                                if (!cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
                                                {
                                                    Decimal _Ty_gia = 0;
                                                    _Ty_gia = txtTy_gia.nValue;

                                                    Decimal tien = SmLib.SysFunc.Round(_Ty_gia * _Tien_nt, M_Round);
                                                    //if (tien > 0)
                                                    {
                                                        Decimal thue_nt0 = 0;
                                                        e.Cell.Record.Cells["tien"].Value = tien;
                                                        if (e.Cell.Record.Cells["thue"].Value != DBNull.Value)
                                                            Decimal.TryParse(e.Cell.Record.Cells["thue"].Value.ToString(), out thue_nt0);
                                                        e.Cell.Record.Cells["tt"].Value = tien + thue_nt0;
                                                    }
                                                }
                                                else
                                                {
                                                    Decimal thue_nt0 = 0;
                                                    e.Cell.Record.Cells["tien"].Value = _Tien_nt;
                                                    if (e.Cell.Record.Cells["thue"].Value != DBNull.Value)
                                                        Decimal.TryParse(e.Cell.Record.Cells["thue"].Value.ToString(), out thue_nt0);
                                                    e.Cell.Record.Cells["tt"].Value = _Tien_nt + thue_nt0;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Decimal _Tien_nt = 0, thue_nt = 0;
                                            _Tien_nt = (e.Editor as NumericTextBox).nValue;

                                            if (e.Cell.Record.Cells["thue_nt"].Value != DBNull.Value)
                                                Decimal.TryParse(e.Cell.Record.Cells["thue_nt"].Value.ToString(), out thue_nt);

                                            e.Cell.Record.Cells["tt_nt"].Value = _Tien_nt + thue_nt;
                                            if (!cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
                                            {
                                                Decimal _ty_gia = 0, _tien_nt0;
                                                Decimal.TryParse(txtTy_gia.nValue.ToString(), out _ty_gia);
                                                _tien_nt0 = SmLib.SysFunc.Round(_Tien_nt * _ty_gia, M_Round);
                                                //if (_tien_nt0 > 0)
                                                {
                                                    Decimal thue_nt0 = 0;
                                                    e.Cell.Record.Cells["tien"].Value = _tien_nt0;
                                                    if (e.Cell.Record.Cells["thue"].Value != DBNull.Value)
                                                        Decimal.TryParse(e.Cell.Record.Cells["thue"].Value.ToString(), out thue_nt0);
                                                    e.Cell.Record.Cells["tt"].Value = _tien_nt0 + thue_nt0;
                                                }
                                            }
                                            else
                                            {
                                                Decimal thue_nt0 = 0;
                                                e.Cell.Record.Cells["tien"].Value = _Tien_nt;
                                                if (e.Cell.Record.Cells["thue"].Value != DBNull.Value)
                                                    Decimal.TryParse(e.Cell.Record.Cells["thue"].Value.ToString(), out thue_nt0);
                                                e.Cell.Record.Cells["tt"].Value = _Tien_nt + thue_nt0;
                                            }
                                        }
                                    }

                                    Decimal ty_gia = 0, tien_nt0 = 0, tien_nt = 0;
                                    Decimal.TryParse(txtTy_gia.nValue.ToString(), out ty_gia);
                                    Decimal.TryParse(e.Cell.Value.ToString(), out tien_nt);
                                    tien_nt0 = SmLib.SysFunc.Round(tien_nt * ty_gia, M_Round);
                                    e.Cell.Record.Cells["tien_tt"].Value = tien_nt0;

                                    UpdateTotalHT();
                                }
                            }
                            break;

                        case "loai_hd":
                            {
                                if (!string.IsNullOrEmpty(e.Editor.Text))
                                {
                                    ControlHostEditor ch_loai_hd = e.Editor as ControlHostEditor;
                                    if (ch_loai_hd != null)
                                        if (e.Cell.IsDataChanged)
                                        {
                                            switch (e.Cell.Value.ToString().Trim())
                                            {
                                                case "0":
                                                case "5":
                                                    {
                                                        //Tự đồng check nút sửa thuế(aKhai)
                                                        //if (e.Cell.Value.ToString().Trim().Equals("5"))
                                                        //    PhView[0]["sua_thue"] = 1;
                                                        e.Cell.Record.Cells["ma_thue_i"].Value = "";
                                                        e.Cell.Record.Cells["thue_suat"].Value = 0;
                                                        e.Cell.Record.Cells["thue_nt"].Value = 0;
                                                        e.Cell.Record.Cells["thue"].Value = 0;
                                                        e.Cell.Record.Cells["tt_nt"].Value = e.Cell.Record.Cells["tien_nt"].Value;
                                                        e.Cell.Record.Cells["tt"].Value = e.Cell.Record.Cells["tien"].Value;

                                                        UpdateTotalHT();
                                                    }
                                                    break;

                                                case "2":
                                                    {
                                                        CalculateHd2(GrdCt.ActiveRecord as DataRecord);
                                                        //decimal t_tt_nt = 0, t_tt = 0, t_thue_nt = 0, t_thue = 0, t_tien_nt = 0, t_tien = 0;
                                                        //var v_t_tt_n = CtData.AsEnumerable()
                                                        //           .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                                                        //           .Sum(x => x.Field<decimal?>("tien_nt"));
                                                        //t_tt_nt = FNum.ToDec(v_t_tt_n);

                                                        //var v_t_tt = CtData.AsEnumerable()
                                                        //           .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                                                        //           .Sum(x => x.Field<decimal?>("tien"));
                                                        //t_tt = FNum.ToDec(v_t_tt);

                                                        //var v_t_thue_nt = CtData.AsEnumerable()
                                                        //           .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                                                        //           .Sum(x => x.Field<decimal?>("thue_nt"));
                                                        //t_thue_nt = FNum.ToDec(v_t_thue_nt);

                                                        //var v_t_thue = CtData.AsEnumerable()
                                                        //           .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                                                        //           .Sum(x => x.Field<decimal?>("thue"));
                                                        //t_thue = FNum.ToDec(v_t_thue);

                                                        //t_tien_nt = t_tt_nt - t_thue_nt;
                                                        //t_tien = t_tt - t_thue;

                                                        //StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt"] = t_tien_nt;
                                                        //StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_nt"] = t_thue_nt;
                                                        //StartUp.DsTrans.Tables[0].DefaultView[0]["t_tt_nt"] = t_tt_nt;
                                                        //StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien"] = t_tien;
                                                        //StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue"] = t_thue;
                                                        //StartUp.DsTrans.Tables[0].DefaultView[0]["t_tt"] = t_tt;
                                                        //StartUp.DsTrans.Tables[1].AcceptChanges();
                                                    }
                                                    break;
                                            }
                                            //if (!e.Cell.Value.ToString().Trim().Equals("2"))
                                                UpdateTotalHT();
                                        }
                                }

                                if (e.Cell.Value.ToString().Trim() == "0")
                                {
                                    e.Cell.Record.Cells["ma_kh_t"].Value = "";
                                    //e.Cell.Record.Cells["ten_kh_t"].Value = "";
                                    //e.Cell.Record.Cells["dia_chi_t"].Value = "";
                                    //e.Cell.Record.Cells["mst_t"].Value = "";
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(e.Cell.Record.Cells["ma_kh_t"].Value.ToString()) &&
                                        string.IsNullOrEmpty(e.Cell.Record.Cells["ten_kh_t"].Value.ToString()))
                                    {
                                        if (e.Cell.Record.Index == 0)
                                        {
                                            e.Cell.Record.Cells["ma_kh_t"].Value = txtMa_kh.Text;
                                            e.Cell.Record.Cells["ten_kh_t"].Value = txtTen_kh.Text;
                                            e.Cell.Record.Cells["dia_chi_t"].Value = txtDia_chi.Text;
                                            e.Cell.Record.Cells["mst_t"].Value = txtMaSoThue.Text;
                                        }
                                        else
                                        {
                                            e.Cell.Record.Cells["ma_kh_t"].Value = (GrdCt.Records[e.Cell.Record.Index - 1] as DataRecord).Cells["ma_kh_t"].Value;
                                            e.Cell.Record.Cells["ten_kh_t"].Value = (GrdCt.Records[e.Cell.Record.Index - 1] as DataRecord).Cells["ten_kh_t"].Value;
                                            e.Cell.Record.Cells["dia_chi_t"].Value = (GrdCt.Records[e.Cell.Record.Index - 1] as DataRecord).Cells["dia_chi_t"].Value;
                                            e.Cell.Record.Cells["mst_t"].Value = (GrdCt.Records[e.Cell.Record.Index - 1] as DataRecord).Cells["mst_t"].Value;
                                        }
                                    }
                                }
                            }
                           
                            break;
                        case "ma_kh_t":
                            {
                                AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                if (txt.RowResult != null && !string.IsNullOrEmpty(e.Editor.Text.Trim()) && txt.IsDataChanged)
                                {
                                    e.Cell.Record.Cells["ten_kh_t"].Value = M_LAN.ToUpper().Equals("V") ? txt.RowResult["ten_kh"] : txt.RowResult["ten_kh2"];
                                    if (!string.IsNullOrEmpty(txt.RowResult["dia_chi"].ToString().Trim()))
                                        e.Cell.Record.Cells["dia_chi_t"].Value = txt.RowResult["dia_chi"];
                                    if (!string.IsNullOrEmpty(txt.RowResult["ma_so_thue"].ToString().Trim()))
                                        e.Cell.Record.Cells["mst_t"].Value = txt.RowResult["ma_so_thue"];
                                }
                            }
                            break;
                        case "ma_thue_i":
                            {
                                AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                //Cập nhật tài khoản thuế
                                if (txt.RowResult != null && txt.IsDataChanged)
                                {
                                    e.Cell.Record.Cells["tk_thue_i"].Value = txt.RowResult["tk_thue_no"];
                                    CellValuePresenter cellTk = CellValuePresenter.FromCell(e.Cell.Record.Cells["tk_thue_i"]);
                                    AutoCompleteTextBox txtTk = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(cellTk.Editor as ControlHostEditor);
                                    txtTk.SearchInit();
                                    if (txtTk.RowResult != null)
                                    {
                                        DataRowView drVCT = e.Cell.Record.DataItem as DataRowView;
                                        drVCT["tk_thue_cn"] = txtTk.RowResult["tk_cn"];

                                        //Update Binding
                                        CellValuePresenter cellV = CellValuePresenter.FromCell(e.Cell.Record.Cells["ma_kh2_t"]);
                                        ControlFunction.RefreshSingleBinding(cellV, AutoCompleteTextBox.IsReadOnlyProperty);
                                    }


                                    e.Cell.Record.Cells["thue_suat"].Value = txt.RowResult["thue_suat"];
                                    //if (!ChkSuaTien.IsChecked.Value)
                                    //{
                                    //Tính thuế
                                    if (!string.IsNullOrEmpty(e.Cell.Record.Cells["thue_suat"].Value.ToString()))
                                    {
                                        Decimal _thue_suat = 0;
                                        Decimal.TryParse(e.Cell.Record.Cells["thue_suat"].Value.ToString(), out _thue_suat);

                                        if (e.Cell.Record.Cells["loai_hd"].Value.ToString().Trim().Equals("2"))
                                        {
                                            ControlHostEditor ntTienNt = e.Editor as ControlHostEditor;
                                            CalculateHd2(GrdCt.ActiveRecord as DataRecord);
                                        }
                                        else
                                        {
                                            Decimal _Tien_nt = 0;
                                            Decimal.TryParse(e.Cell.Record.Cells["tien_nt"].Value.ToString(), out _Tien_nt);
                                            Decimal _thue_nt = 0;

                                            _thue_nt = _Tien_nt * _thue_suat / 100;
                                            if (!cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
                                            {
                                                _thue_nt = SmLib.SysFunc.Round(_thue_nt, M_Round_nt);
                                            }
                                            else
                                            {
                                                _thue_nt = SmLib.SysFunc.Round(_thue_nt, M_Round);
                                            }
                                            e.Cell.Record.Cells["thue_nt"].Value = _thue_nt;
                                            e.Cell.Record.Cells["tt_nt"].Value = _thue_nt + _Tien_nt;

                                            if (!cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
                                            {
                                                Decimal _tien_nt0;
                                                Decimal.TryParse(e.Cell.Record.Cells["tien"].Value.ToString(), out _tien_nt0);
                                                Decimal _thue_nt0 = 0;
                                                _thue_nt0 = SmLib.SysFunc.Round(_tien_nt0 * _thue_suat / 100, M_Round);
                                                e.Cell.Record.Cells["thue"].Value = _thue_nt0;
                                                e.Cell.Record.Cells["tt"].Value = _tien_nt0 + _thue_nt0;
                                            }
                                            else
                                            {
                                                e.Cell.Record.Cells["tien"].Value = _Tien_nt;
                                                e.Cell.Record.Cells["thue"].Value = _thue_nt;
                                                e.Cell.Record.Cells["tt"].Value = _thue_nt + _Tien_nt;
                                            }
                                        }
                                        //if (!e.Cell.Record.Cells["loai_hd"].Value.ToString().Trim().Equals("2"))
                                            UpdateTotalHT();
                                    }
                                    //}
                                }
                            }
                            break;
                        //case "thue_suat":
                        //    {

                        //        if (!string.IsNullOrEmpty(e.Editor.Text))
                        //        {
                        //            Decimal _thue_suat = 0;
                        //            Decimal.TryParse(e.Cell.Record.Cells["thue_suat"].Value.ToString(), out _thue_suat);

                        //            if (e.Cell.Record.Cells["loai_hd"].Value.ToString().Trim().Equals("2"))
                        //            {
                        //                NumericTextBox ntTienNt = e.Editor as NumericTextBox;
                        //                if (e.Cell.IsDataChanged)
                        //                {
                        //                    CalculateHd2(GrdCt.ActiveRecord as DataRecord);
                        //                }
                        //            }
                        //            else if (!e.Cell.Record.Cells["loai_hd"].Value.ToString().Trim().Equals("0"))
                        //            {
                        //                Decimal _Tien_nt = 0;
                        //                Decimal.TryParse(e.Cell.Record.Cells["tien_nt"].Value.ToString(), out _Tien_nt);
                        //                Decimal _thue_nt = 0;

                        //                _thue_nt = _Tien_nt * _thue_suat / 100;
                        //                if (!cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
                        //                {
                        //                    _thue_nt = SmLib.SysFunc.Round(_thue_nt, M_Round_nt);
                        //                }
                        //                else
                        //                {
                        //                    _thue_nt = SmLib.SysFunc.Round(_thue_nt, M_Round);
                        //                }
                        //                e.Cell.Record.Cells["thue_nt"].Value = _thue_nt;
                        //                e.Cell.Record.Cells["tt_nt"].Value = _thue_nt + _Tien_nt;

                        //                if (!cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
                        //                {
                        //                    Decimal _tien_nt0;
                        //                    Decimal.TryParse(e.Cell.Record.Cells["tien"].Value.ToString(), out _tien_nt0);
                        //                    Decimal _thue_nt0 = 0;
                        //                    _thue_nt0 = SmLib.SysFunc.Round(_tien_nt0 * _thue_suat / 100, M_Round);
                        //                    e.Cell.Record.Cells["thue"].Value = _thue_nt0;
                        //                    e.Cell.Record.Cells["tt"].Value = _tien_nt0 + _thue_nt0;
                        //                }
                        //                else
                        //                {
                        //                    e.Cell.Record.Cells["tien"].Value = _Tien_nt;
                        //                    e.Cell.Record.Cells["thue"].Value = _thue_nt;
                        //                    e.Cell.Record.Cells["tt"].Value = _thue_nt + _Tien_nt;
                        //                }
                        //            }
                        //            UpdateTotalHT();
                        //        }
                        //    }
                        //    break;
                        case "thue_nt":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (!string.IsNullOrEmpty(e.Editor.Text))
                                    {
                                        Decimal _thue_suat = 0;
                                        if (e.Cell.Record.Cells["thue_suat"].Value != DBNull.Value)
                                            Decimal.TryParse(e.Cell.Record.Cells["thue_suat"].Value.ToString(), out _thue_suat);
                                        if (e.Cell.Record.Cells["loai_hd"].Value.ToString().Trim().Equals("2"))
                                        {
                                            NumericTextBox ntTienNt = e.Editor as NumericTextBox;
                                            if (e.Cell.IsDataChanged)
                                            {
                                                Decimal _tien_nt = 0, _tt_nt = 0, _thue_nt = 0;
                                                Decimal.TryParse(e.Cell.Record.Cells["tt_nt"].Value.ToString(), out _tt_nt);
                                                Decimal.TryParse(e.Cell.Record.Cells["thue_nt"].Value.ToString(), out _thue_nt);
                                                _tien_nt = _tt_nt - _thue_nt;
                                                e.Cell.Record.Cells["tien_nt"].Value = _tien_nt;

                                                if (!cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
                                                {
                                                    Decimal _tien_nt0 = 0, _ty_gia = 0;
                                                    Decimal.TryParse(txtTy_gia.nValue.ToString(), out _ty_gia);

                                                    _tien_nt0 = SmLib.SysFunc.Round(_tien_nt * _ty_gia, M_Round);
                                                    if (_tien_nt0 > 0)
                                                    {
                                                        if (e.Cell.Record.Cells["thue_suat"].Value != DBNull.Value)
                                                        {
                                                            Decimal _thue_nt0 = 0, _tt = 0, _t_Lech = 0, _tien_Lech = 0, _thue_lech = 0;
                                                            _thue_nt0 = SmLib.SysFunc.Round(_tien_nt0 * _thue_suat / 100, M_Round);

                                                            _tt = SmLib.SysFunc.Round(_tt_nt * _ty_gia, M_Round);
                                                            _t_Lech = (_tt - (_tien_nt0 + _thue_nt0));
                                                            _tien_Lech = SmLib.SysFunc.Round((_t_Lech / (1 + (_thue_suat / 100))), M_Round);
                                                            _thue_lech = _t_Lech - _tien_Lech;

                                                            e.Cell.Record.Cells["tien"].Value = _tien_nt0 + _tien_Lech;
                                                            e.Cell.Record.Cells["thue"].Value = _thue_nt0 + _thue_lech;
                                                            e.Cell.Record.Cells["tt"].Value = _tt;
                                                        }
                                                        else
                                                        {
                                                            Decimal _thue_nt0 = 0;
                                                            e.Cell.Record.Cells["tien"].Value = _tien_nt0;
                                                            if (e.Cell.Record.Cells["thue"].Value != DBNull.Value)
                                                                Decimal.TryParse(e.Cell.Record.Cells["thue"].Value.ToString(), out _thue_nt0);
                                                            e.Cell.Record.Cells["tt"].Value = _tien_nt0 + _thue_nt0;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    e.Cell.Record.Cells["tien"].Value = _tien_nt;
                                                    e.Cell.Record.Cells["thue"].Value = _thue_nt;
                                                    e.Cell.Record.Cells["tt"].Value = _tt_nt;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Decimal _thue_nt = 0, _tien_nt = 0;
                                            Decimal.TryParse(e.Cell.Record.Cells["thue_nt"].Value.ToString(), out _thue_nt);
                                            Decimal.TryParse(e.Cell.Record.Cells["tien_nt"].Value.ToString(), out _tien_nt);
                                            e.Cell.Record.Cells["tt_nt"].Value = _thue_nt + _tien_nt;
                                            if (cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
                                            {
                                                e.Cell.Record.Cells["thue"].Value = _thue_nt;
                                                e.Cell.Record.Cells["tien"].Value = _tien_nt;
                                                e.Cell.Record.Cells["tt"].Value = _thue_nt + _tien_nt;

                                            }
                                            // Sửa thuế nt tự tính lại thuế vnd
                                            else
                                            {
                                                decimal _Ty_gia = txtTy_gia.nValue;
                                                decimal thue = _thue_nt * _Ty_gia;
                                                if (thue > 0)
                                                {
                                                    e.Cell.Record.Cells["thue"].Value = SmLib.SysFunc.Round(thue, M_Round);
                                                    decimal tien = Convert.ToDecimal(e.Cell.Record.Cells["tien"].Value.Equals(DBNull.Value) ? 0 : e.Cell.Record.Cells["tien"].Value);
                                                    e.Cell.Record.Cells["tt"].Value = tien + SmLib.SysFunc.Round(thue, M_Round);
                                                }
                                            }
                                        }
                                    }

                                    UpdateTotalHT();
                                }
                            }
                            break;
                        case "tk_thue_i":
                            {
                                AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                if (txt.RowResult != null)
                                {
                                    DataRowView drVCT = e.Cell.Record.DataItem as DataRowView;
                                    drVCT["tk_thue_cn"] = txt.RowResult["tk_cn"];

                                    //Update Binding
                                    CellValuePresenter cellV = CellValuePresenter.FromCell(e.Cell.Record.Cells["ma_kh2_t"]);
                                    ControlFunction.RefreshSingleBinding(cellV, AutoCompleteTextBox.IsReadOnlyProperty);
                                }
                            }
                            break;
                        case "tien":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (txtTy_gia.Value != null && !string.IsNullOrEmpty(e.Editor.Text.Trim()))
                                    {
                                        Decimal _tien_nt0 = 0, _thue_suat = 0, _thue_nt0 = 0;
                                        Decimal.TryParse(e.Editor.Value.ToString(), out _tien_nt0);
                                        Decimal.TryParse(e.Cell.Record.Cells["thue_suat"].Value.ToString(), out _thue_suat);
                                        _thue_nt0 = SmLib.SysFunc.Round(_tien_nt0 * _thue_suat / 100, M_Round);
                                        e.Cell.Record.Cells["thue"].Value = _thue_nt0;
                                        e.Cell.Record.Cells["tt"].Value = _tien_nt0 + _thue_nt0;
                                    }
                                    UpdateTotalHT();
                                }
                            }
                            break;
                        case "thue":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (!string.IsNullOrEmpty(e.Editor.Text))
                                    {
                                        Decimal _thue_nt0 = 0, _tien_nt0 = 0;
                                        Decimal.TryParse(e.Editor.Value.ToString(), out _thue_nt0);
                                        Decimal.TryParse(e.Cell.Record.Cells["tien"].Value.ToString(), out _tien_nt0);
                                        e.Cell.Record.Cells["tt"].Value = _thue_nt0 + _tien_nt0;
                                    }
                                    UpdateTotalHT();
                                }
                            }
                            break;
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
                    }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        void txtMa_kh_PreviewGotFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!IsInEditMode.Value)
                return;
            txtMa_kh.IsReadOnly = false;
            if (txtMa_gd.Text.Trim() != "1")
                return;

            var query = from q in CtView.Cast<DataRowView>()
                        where q["so_ct0"].ToString().Trim() != ""
                        select q;
            txtMa_kh.IsReadOnly = query.Any();

        }

        private void V_Sua()
        {
            if (PhData.Rows.Count == 0)
                ExMessageBox.Show( 215,StartUp.SysObj, "Không có dữ liệu!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                currActionTask = ActionTask.Edit;

                DsVitual = new DataSet();
                DsVitual.Tables.Add(PhView.ToTable());
                DsVitual.Tables.Add(CtView.ToTable());
                DsVitual.Tables.Add(StartUp.DsTrans.Tables[2].DefaultView.ToTable());



                int count = CtView.Count;
                if (count > 0)
                {
                    if (txtMa_gd.Text.Trim() != "1" && CtView[0]["tk_i"] != DBNull.Value && !string.IsNullOrEmpty(CtView[0]["tk_i"].ToString()))
                    {
                        txtMa_kh.IsFocus = true;
                    }
                }

                IsInEditMode.Value = true;
                if (txtMa_gd.Text.Trim() != "1")
                    ;
                else
                {

                    var query = from q in CtView.Cast<DataRowView>()
                                where q["so_ct0"].ToString().Trim() != ""
                                select q;
                    if (query.Any())
                    {
                        txtMa_kh.IsReadOnly = true;
                        txtDia_chi.Focus();
                    }
                    else
                        txtMa_kh.IsFocus = true;
                }

                Voucher_Ma_nt0.Text = PhView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (PhView[0]["ma_nt"].ToString().Equals(M_Ma_nt0));
                
                if (Ma_GD_Value.Text.Equals("1"))
                {
                    string stt_rec, stt_rec_tt, ma_nt, ma_ct;
                    decimal tt_qd, tt_dn, tt_dn_nt;

                    stt_rec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();
                    ma_nt = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                    ma_ct = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_ct"].ToString();
                    int user_id = (int)StartUp.SysObj.UserInfo.Rows[0]["user_id"];

                    for (int i = 0; i < Grdhd.Records.Count; i++)
                    {
                        stt_rec_tt = (Grdhd.Records[i] as DataRecord).Cells["stt_rec_tt"].Value.ToString();
                        decimal.TryParse((Grdhd.Records[i] as DataRecord).Cells["tt_qd"].Value.ToString(), out tt_qd);
                        decimal.TryParse((Grdhd.Records[i] as DataRecord).Cells["tien"].Value.ToString(), out tt_dn);
                        decimal.TryParse((Grdhd.Records[i] as DataRecord).Cells["tien_nt"].Value.ToString(), out tt_dn_nt);
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandText = "Exec [CACTPC1-ExcludeVoucher] @Stt_rec, @Stt_rec_tt, @Ma_nt, @Tt_qd, @Tt_dn, @Tt_dn_nt";
                        cmd.Parameters.Add("@Stt_rec", SqlDbType.VarChar).Value = stt_rec;
                        cmd.Parameters.Add("@Stt_rec_tt", SqlDbType.VarChar).Value = stt_rec_tt;
                        cmd.Parameters.Add("@Ma_nt", SqlDbType.VarChar).Value = ma_nt;
                        cmd.Parameters.Add("@Tt_qd", SqlDbType.Decimal).Value = tt_qd;
                        cmd.Parameters.Add("@Tt_dn", SqlDbType.Decimal).Value = tt_dn;
                        cmd.Parameters.Add("@Tt_dn_nt", SqlDbType.Decimal).Value = tt_dn_nt;
                        cmd.Parameters.Add("@User_id", SqlDbType.Int).Value = user_id;
                        StartUp.SysObj.ExcuteNonQuery(cmd);

                        cmd = new SqlCommand();
                        cmd.CommandText = "EXEC Ap4;70 @Stt_rec, @Ma_ct";
                        cmd.Parameters.Add("@Stt_rec", SqlDbType.VarChar).Value = stt_rec;
                        cmd.Parameters.Add("@Ma_ct", SqlDbType.VarChar).Value = ma_ct;
                        StartUp.SysObj.ExcuteNonQuery(cmd);
                    }
                }

                TabInfo.SelectedIndex = 0;
            }
        }

        private void ChkSua_tggs_Click(object sender, RoutedEventArgs e)
        {
            IsCheckedSua_tggs.Value = ChkSua_tggs.IsChecked.Value;
        }

        private void ChkSuaTien_Click(object sender, RoutedEventArgs e)
        {
            IsCheckedSua_tien.Value = ChkSuaTien.IsChecked.Value;
            if (ChkSuaTien.IsChecked == false && sender.GetType().Name.Equals("CheckBox"))
            {
                CalculateTyGia();
            }
        }

        private void V_Moi()
        {
            try
            {
                string newSttRec = DataProvider.NewTrans(StartUp.SysObj, StartUp.Ma_ct, StartUp.Ws_Id);
                currActionTask = ActionTask.Add;
                if (!string.IsNullOrEmpty(newSttRec))
                {
                    DsVitual = StartUp.DsTrans.Copy();
                    txtMa_gd.IsFocus = true;

                    //Them moi dong trong Ph
                    DataRow NewRecord = PhData.NewRow();
                    NewRecord["stt_rec"] = newSttRec;
                    NewRecord["ma_ct"] = StartUp.Ma_ct;

                   
                    //NewRecord["ma_nt"] = PhData.Rows.Count > 1 ? PhView[0]["ma_nt"].ToString() : StartUp.DmctInfo["ma_nt"].ToString();
                    if (SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, txtNgay_ct.dValue))
                    {
                        NewRecord["ngay_ct"] = txtNgay_ct.dValue.Date;
                        //NewRecord["ngay_lct"] = txtNgay_ct.dValue.Date;
                    }
                    else
                    {
                        NewRecord["ngay_ct"] = DateTime.Now.Date;
                        //NewRecord["ngay_lct"] = DateTime.Now.Date;
                    }
                    if (PhData.Rows.Count == 1)
                    {
                        NewRecord["ma_nt"] = StartUp.DmctInfo["ma_nt"];
                        NewRecord["ma_qs"] = GetDMQS(BindingSysObj, StartUp.Ma_ct, Convert.ToDateTime(NewRecord["ngay_ct"]), StartUp.M_User_Id);
                    }
                    else
                    {
                        NewRecord["ma_nt"] = PhData.Rows[iRow]["ma_nt"];
                        NewRecord["ma_qs"] = GetDMQS(BindingSysObj, StartUp.Ma_ct, Convert.ToDateTime(NewRecord["ngay_ct"]),
                            StartUp.M_User_Id, PhData.Rows[iRow]["ma_qs"].ToString().Trim());
                    }
                    if (NewRecord["ma_nt"].ToString().Trim().Equals(M_Ma_nt0))
                    {
                        NewRecord["ty_giaf"] = 1;
                    }
                    else
                    {
                        NewRecord["ty_giaf"] = StartUp.GetRates(NewRecord["ma_nt"].ToString().Trim(), Convert.ToDateTime(NewRecord["ngay_ct"]).Date);
                    }
                    NewRecord["ma_gd"] = PhData.Rows.Count > 1 ? PhView[0]["ma_gd"].ToString() : StartUp.DmctInfo["ma_gd"].ToString();
                    NewRecord["status"] = StartUp.DmctInfo["ma_post"];
                    NewRecord["tk"] = PhView[0]["tk"];
                    NewRecord["t_tien_nt"] = 0;
                    NewRecord["t_tien"] = 0;
                    NewRecord["t_thue_nt"] = 0;
                    NewRecord["t_thue"] = 0;
                    NewRecord["t_tt_nt"] = 0;
                    NewRecord["t_tt"] = 0;
                    NewRecord["han_tt"] = 0;
                    if (StartUp.DsTrans.Tables[0].Columns.Contains("loai_tg") && StartUp.DsTrans.Tables[0].Columns.Contains("ma_nt"))
                    {
                        NewRecord["loai_tg"] = StartUp.Getloai_tg(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
                    }
                    //Them moi dong trong Ct
                    DataRow NewCtRecord = CtData.NewRow();
                    NewCtRecord["stt_rec"] = newSttRec;
                    NewCtRecord["stt_rec0"] = "001";
                    NewCtRecord["ma_ct"] = StartUp.Ma_ct;
                    NewCtRecord["loai_hd"] = 0;
                    NewCtRecord["ngay_ct"] = txtNgay_ct.Value == null ? DateTime.Now.Date : txtNgay_ct.dValue.Date;

                    NewCtRecord["ma_ms"] = StartUp.M_ma_ms;
                    NewCtRecord["tien_nt"] = 0;
                    NewCtRecord["tien"] = 0;
                    NewCtRecord["thue_nt"] = 0;
                    NewCtRecord["thue"] = 0;
                    NewCtRecord["ty_giahtf2"] = 0;

                    PhData.Rows.Add(NewRecord);
                    CtData.Rows.Add(NewCtRecord);

                    //if (CtData.Rows.Count == 1)
                    //{
                    //   LoadData();
                    //    (TabInfo.Items[0] as TabItem).Focus();
                    //}
                    //Nhảy đến phiếu vừa thêm
                    PhView.RowFilter = "stt_rec= '" + newSttRec + "'";
                    CtView.RowFilter = "stt_rec= '" + newSttRec + "'";
                    StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";
                    iOldRow = iRow;
                    iRow = PhData.Rows.Count - 1;
                    TabInfo.SelectedIndex = 1;
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            IsInEditMode.Value = true;
                        }));

                    txtTen_kh.Text = "";
                    txtTenTK.Text = "";
                    TabInfo.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }


        #region btnNhan_Click
        private void V_Nhan()
        {
            Stopwatch ww = new Stopwatch();
            ww.Start();

            //Debug.WriteLine(DateTime.Now.ToString("hh:mm:ss"), "- + - + - + - + - + Begin V_Nhan");
            if (StartUp.SysObj.VersionInfo.Rows[0]["product_code"].ToString().Equals("FA") || (StartUp.dtRegInfo != null && !StartUp.dtRegInfo.Rows[18]["content"].ToString().Trim().Equals("FK")))
            {
                CatgLib.Catinhtg.Tinh(GrdCtChi.Records, this);

                //CalculateTyGia();
            }
            TinhLai_tien_tt();

            //Debug.WriteLine(PhView[0]["status"], "Status");
            try
            {
                    bool isError = false;
                    if (!IsSequenceSave)
                    {
                        GrdCt.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);
                        GrdCtChi.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);
                        Grdhd.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);
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

                        if (GrdCt.Records.Count == 0)
                        {
                            if (GrdCtChi.Visibility == Visibility.Visible)
                            {
                                ExMessageBox.Show( 220,StartUp.SysObj, "Chưa vào tài khoản nợ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                GrdCtChi.Focus();
                            }
                            if (Grdhd.Visibility == Visibility.Visible)
                            {
                                ExMessageBox.Show( 225,StartUp.SysObj, "Chưa vào chi tiết!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                Grdhd.Focus();
                            }
                            if (GrdCtgt.Visibility == Visibility.Visible)
                            {
                                ExMessageBox.Show( 230,StartUp.SysObj, "Chưa vào tài khoản nợ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                GrdCtgt.Focus();
                            }
                            return;
                        }

                        if (string.IsNullOrEmpty(PhView[0]["ma_kh"].ToString().Trim()))
                        {
                            ExMessageBox.Show( 235,StartUp.SysObj, "Chưa có mã khách hàng!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtMa_kh.IsFocus = true;
                            isError = true;
                        }
                        else if (string.IsNullOrEmpty(PhView[0]["tk"].ToString().Trim()))
                        {
                            ExMessageBox.Show( 240,StartUp.SysObj, "Chưa vào tài khoản có!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtMa_nx.IsFocus = true;
                            isError = true;
                        }
                        //Kiểm tra có ngày hạch toán hay chưa
                        else if (txtNgay_ct.dValue == new DateTime())
                        {
                            ExMessageBox.Show( 245,StartUp.SysObj, "Chưa vào ngày hạch toán!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtNgay_ct.Focus();
                            isError = true;
                        }

                        else if ( StartUp.M_NGAY_BAT_DAU != null && (!txtNgay_ct.IsValueValid || txtNgay_ct.dValue < StartUp.M_NGAY_BAT_DAU || txtNgay_ct.dValue > StartUp.M_NGAY_KET_THUC))
                            {
                                ExMessageBox.Show(1024, StartUp.SysObj, "Ngày hạch toán không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                isError = true;
                                txtNgay_ct.Focus();
                            }
                        else
                            if (StartUp.M_ngay_lct.Equals("1") && (txtngay_lct.Value == null || txtngay_lct.Value == DBNull.Value || txtngay_lct.dValue == new DateTime()))
                            {
                                ExMessageBox.Show(2012, StartUp.SysObj, "Chưa vào ngày lập chứng từ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                txtngay_lct.Focus();
                                isError = true;
                            }
                            else if (string.IsNullOrEmpty(CtView[0]["tk_i"].ToString().Trim()))
                        {
                            ExMessageBox.Show( 250,StartUp.SysObj, "Chưa vào tài khoản nợ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                            TabInfo.SelectedIndex = 0;
                            try
                            {
                                GrdCt.ExecuteCommand(DataPresenterCommands.CellFirstOverall);
                            }
                            catch (Exception x) { }
                            GrdCt.Focus();
                            isError = true;
                        }
                        else if (string.IsNullOrEmpty(PhView[0]["so_ct"].ToString().Trim()))
                        {
                            ExMessageBox.Show( 255,StartUp.SysObj, "Chưa vào số chứng từ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtSo_ct.Focus();
                            isError = true;
                        }
                        //else if (CheckValidSoct(StartUp.SysObj, txtMa_qs.Text, txtSo_ct.Text, PhView[0]["stt_rec"].ToString()))
                        //{
                        //    if (StartUp.M_trung_so.Equals("1"))
                        //    {
                        //        if (ExMessageBox.Show( 260,StartUp.SysObj, "Có chứng từ trùng số. Số cuối cùng là: " + "[" + GetLastSoct(StartUp.SysObj, txtMa_qs.Text).Trim() + "]" + ". Có lưu chứng từ này không?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                        //        {
                        //            txtSo_ct.SelectAll();
                        //            txtSo_ct.Focus();
                        //            isError = true;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        ExMessageBox.Show( 265,StartUp.SysObj, "Số chứng từ đã tồn tại!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        //        txtSo_ct.SelectAll();
                        //        txtSo_ct.Focus();
                        //        isError = true;
                        //    }
                        //}
                        //Kiểm tra mã số thuế và dữ liệu rỗng và các chi tiết có hợp lệ không
                        if (!isError)
                        {
                            //Đẩy wa tab thue đẻ lấy thông tin thuế chính xác trước khi lưu
                            if (Ma_GD_Value.Text.Trim().Equals("8"))
                            {
                                DataRow[] adr = StartUp.DsTrans.Tables[1].DefaultView.ToTable().Select("loai_hd <> 0");
                                if (TabInfo.SelectedIndex == 0 && adr.Length > 0)
                                    tabItem3.Focus();
                                else if (TabInfo.SelectedIndex == 1 && adr.Length == 0)
                                    tiHT.Focus();
                            }
                            bool showMessage = false;

                            //Kiem tra trung hoa don 
                            bool showMessageCheckHD = false;
                            string so_ct0 = "", so_seri0 = "", ma_so_thue = "";
                            string ngay_ct0;

                            if (StartUp.DsTrans.Tables[2].DefaultView.Count > 0)
                            {
                                for (int i = 0; i < GrdCtgt.Records.Count; i++)
                                {
                                    DataRowView drv = (GrdCtgt.Records[i] as DataRecord).DataItem as DataRowView;
                                    //Bỏ kiem tra số hd 03/01/2010
                                    //|| string.IsNullOrEmpty(drv.Row["so_ct0"].ToString().Trim())
                                    if (string.IsNullOrEmpty(drv.Row["ma_ms"].ToString().Trim()))
                                    {
                                        //SmLib.WinAPISenkey.SenKey(ModifierKeys.Alt, Key.D2);
                                        //this.GrdCtgt.ActiveCell = (GrdCtgt.Records[rowindex] as DataRecord).Cells["ma_ms"];
                                        //GrdCtgt.Focus();
                                        StartUp.DsTrans.Tables[2].Rows.Remove(drv.Row);
                                        StartUp.DsTrans.Tables[2].AcceptChanges();
                                        continue;
                                    }

                                    if (!StartUp.M_MST_CHECK.Equals("0"))
                                    {
                                        if (!SmLib.SysFunc.CheckSumMaSoThue(drv.Row["ma_so_thue"].ToString().Trim()) && !string.IsNullOrEmpty(drv.Row["ma_so_thue"].ToString().Trim()) && !showMessage)
                                        {
                                            ExMessageBox.Show( 270,StartUp.SysObj, "Mã số thuế không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                            showMessage = true;
                                            if (StartUp.M_MST_CHECK.Equals("2"))
                                            {
                                                return;
                                            }
                                        }
                                    }

                                    if (!StartUp.M_CHK_HD_VAO.Equals("0"))
                                    {
                                        so_ct0 = drv.Row["so_ct0"].ToString().Trim();
                                        so_seri0 = drv.Row["so_seri0"].ToString().Trim();
                                        ngay_ct0 = string.IsNullOrEmpty(drv.Row["ngay_ct0"].ToString().Trim()) ? "" : Convert.ToDateTime(drv.Row["ngay_ct0"].ToString().Trim()).Date.ToShortDateString().Substring(0, 10);
                                        ma_so_thue = drv.Row["ma_so_thue"].ToString().Trim();

                                        if (StartUp.CheckExistHDVao(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString(), so_ct0, so_seri0, ngay_ct0, ma_so_thue) && !showMessageCheckHD)
                                        {
                                            ExMessageBox.Show(420, StartUp.SysObj, string.Format("Hoá đơn số [{0}], ký hiệu [{1}], ngày [{2}], MST [{3}] đã tồn tại!", so_ct0, so_seri0, ngay_ct0, ma_so_thue), "", MessageBoxButton.OK, MessageBoxImage.Information);
                                            showMessageCheckHD = true;
                                            if (StartUp.M_MST_CHECK.Equals("2"))
                                            {
                                                //Cảnh báo và không cho lưu
                                                return;
                                            }
                                        }
                                    }


                                    if (!string.IsNullOrEmpty(drv.Row["so_ct0"].ToString().Trim()))                                    
                                    {
                                        if (string.IsNullOrEmpty(drv.Row["ngay_ct0"].ToString().Trim()))
                                        {
                                                ExMessageBox.Show(3000, StartUp.SysObj, "Chưa vào ngày hóa đơn. Không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                isError = true;
                                                GrdCtgt.ActiveCell = (GrdCtgt.Records[i] as DataRecord).Cells["ngay_ct0"];
                                                GrdCtgt.Focus();
                                        }

                                    }

                                    if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[2].DefaultView[i]["tk_thue_no"].ToString().Trim()))
                                    {
                                        decimal thue_nt = 0;
                                        decimal.TryParse(StartUp.DsTrans.Tables[2].DefaultView[i]["t_thue_nt"].ToString(), out thue_nt);

                                        if (!string.IsNullOrEmpty(StartUp.DsTrans.Tables[2].DefaultView[i]["so_ct0"].ToString().Trim()) && thue_nt > 0)
                                        {
                                            if (!isError)
                                            {
                                                ExMessageBox.Show( 275,StartUp.SysObj, "Chưa vào tk thuế, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                isError = true;
                                                GrdCtgt.ActiveCell = (GrdCtgt.Records[i] as DataRecord).Cells["tk_thue_no"];
                                                GrdCtgt.Focus();
                                            }
                                        }
                                    }
                                }
                            }
                            if(!CheckVoucherOutofDate())
                                isError = true;
                            if (CtView.Count > 0)
                            {
                                foreach (DataRecord da in GrdCt.Records)
                                {
                                    DataRowView drv = da.DataItem as DataRowView;
                                    if (string.IsNullOrEmpty(drv.Row["tk_i"].ToString().Trim()))
                                    {
                                        CtData.Rows.Remove(drv.Row);
                                        CtData.AcceptChanges();
                                        continue;
                                    }
                                    //if (!StartUp.M_MST_CHECK.Equals("0") && !showMessage)
                                    //{
                                    //    if (!string.IsNullOrEmpty(drv.Row["mst_t"].ToString().Trim()) && !SmLib.SysFunc.CheckSumMaSoThue(drv.Row["mst_t"].ToString().Trim()) && !showMessage)
                                    //    {
                                    //        ExMessageBox.Show( 280,StartUp.SysObj, "Mã số thuế không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                    //        showMessage = true;
                                    //        if (StartUp.M_MST_CHECK.Equals("2"))
                                    //        {
                                    //            return;
                                    //        }
                                    //    }
                                    //}



                                    if (!string.IsNullOrEmpty(drv["ma_thue_i"].ToString().Trim()) && string.IsNullOrEmpty(drv["tk_thue_i"].ToString().Trim()))
                                    {
                                        if (!isError)
                                        {
                                            ExMessageBox.Show( 285,StartUp.SysObj, "Chưa vào tk thuế, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                            isError = true;
                                            tiHT.Focus();
                                            GrdCt.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                            {
                                                GrdCt.ActiveCell = da.Cells["tk_thue_i"];
                                                GrdCt.Focus();
                                            }));
                                        }
                                    }
                                }
                            }
                         
                        }
                    }


              

                    if (!isError)
                    {
                        if (!IsSequenceSave)
                        {
                            if (Ma_GD_Value.Text.Equals("8"))
                            {
                                Decimal _t_tien = 0, _t_thue = 0;
                                Decimal _t_tien_nt0 = 0, _t_thue_nt0 = 0;

                                Decimal.TryParse(txtT_Tien_nt.nValue.ToString(), out _t_tien);
                                Decimal.TryParse(txtT_thue_nt.nValue.ToString(), out _t_thue);
                                if (!cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
                                {
                                    Decimal.TryParse(txtT_Tien_Nt0.nValue.ToString(), out _t_tien_nt0);
                                    Decimal.TryParse(txtT_thue_Nt0.nValue.ToString(), out _t_thue_nt0);
                                }
                                switch (TabInfo.SelectedIndex)
                                {
                                    case 0:
                                        // Tính tổng tiền ht
                                        var vtien = CtData.AsEnumerable()
                                                    .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString() && b.Field<object>("ngay_ct0") != null)
                                                    .Sum(x => x.Field<decimal?>("tien_nt"));
                                        if (vtien != null)
                                            Decimal.TryParse(vtien.ToString(), out _t_tien);

                                        vtien = CtData.AsEnumerable()
                                                    .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString() && b.Field<object>("ngay_ct0") != null)
                                                    .Sum(x => x.Field<decimal?>("tien"));
                                        if (vtien != null)
                                            Decimal.TryParse(vtien.ToString(), out _t_tien_nt0);

                                        var vthue = CtData.AsEnumerable()
                                                    .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString() && b.Field<object>("ngay_ct0") != null)
                                                    .Sum(x => x.Field<decimal?>("thue_nt"));
                                        if (vthue != null)
                                            Decimal.TryParse(vthue.ToString(), out _t_thue );



                                        vthue = CtData.AsEnumerable()
                                                    .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString() && b.Field<object>("ngay_ct0") != null)
                                                    .Sum(x => x.Field<decimal?>("thue"));
                                        if (vthue != null)
                                            Decimal.TryParse(vthue.ToString(), out _t_thue_nt0);

                                        //Tính tổng tiên bên tab thuế
                                        UpdateTotalThue();
                                        Decimal _t_tien_hdt = 0, _t_thue_hdt = 0;
                                        Decimal _t_tien_nt0_hdt = 0, _t_thue_nt0_hdt = 0;

                                        Decimal.TryParse(txtT_t_Tien_nt.nValue.ToString(), out _t_tien_hdt);
                                        Decimal.TryParse(txtT_t_thue_nt.nValue.ToString(), out _t_thue_hdt);
                                        if (!cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
                                        {
                                            Decimal.TryParse(txtT_t_Tien_Nt0.nValue.ToString(), out _t_tien_nt0_hdt);
                                            Decimal.TryParse(txtT_t_thue_Nt0.nValue.ToString(), out _t_thue_nt0_hdt);
                                        }
                                        if (cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
                                        {
                                            if (_t_tien != _t_tien_hdt)
                                            {
                                                if (PhView[0]["sua_thue"].ToString() == "0")
                                                {
                                                    ExMessageBox.Show(320, StartUp.SysObj, "Tổng tiền/ tiền ngoại tệ khác với tổng tiền/ tiền ngoại tệ trong các hóa đơn giá trị gia tăng!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);

                                                    isError = true;
                                                }
                                            }
                                            if (!isError && _t_thue != _t_thue_hdt)
                                            {
                                                if (PhView[0]["sua_thue"].ToString() == "1" || PhView[0]["sua_tien"].ToString() == "1")
                                                {
                                                    ExMessageBox.Show(325, StartUp.SysObj, "Tổng tiền thuế/ thuế ngoại tệ khác với tổng tiền thuế/ thuế ngoại tệ trong các hóa đơn giá trị gia tăng!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    isError = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (_t_tien_nt0 != _t_tien_nt0_hdt)
                                            {
                                                if (PhView[0]["sua_thue"].ToString() == "0")
                                                {
                                                    ExMessageBox.Show( 330,StartUp.SysObj, "Tổng tiền/ tiền ngoại tệ khác với tổng tiền/ tiền ngoại tệ trong các hóa đơn giá trị gia tăng!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    isError = true;
                                                }
                                            }
                                            if (_t_thue_nt0 != _t_thue_nt0_hdt)
                                            {
                                                if (PhView[0]["sua_thue"].ToString() == "1" || PhView[0]["sua_tien"].ToString() == "1")
                                                {
                                                    ExMessageBox.Show(335, StartUp.SysObj, "Tổng tiền thuế/ thuế ngoại tệ khác với tổng tiền thuế/ thuế ngoại tệ trong các hóa đơn giá trị gia tăng!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    isError = true;
                                                }
                                            }
                                        }
                                        //Cập nhật lại tổng tiền tab  hạch toán
                                        UpdateTotalHT();
                                        break;
                                    case 1:
                                        if (GrdCtgt.Records.Count > 0)
                                        {
                                            //Cập nhật lại tổng tiền tab thuế
                                            UpdateTotalHT();
                                            UpdateTotalThue();
                                            //Tính tổng tiên bên tab hạch toán
                                            Decimal _t_tien_ht = 0, _t_thue_ht = 0;
                                            Decimal _t_tien_nt0_ht = 0, _t_thue_nt0_ht = 0;

                                            var loaihd = CtData.AsEnumerable()
                                                           .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())  
                                                           .Select(x => x.Field<string>("loai_hd"));
                                            string sohd = "";
                                            foreach (string s in loaihd)
                                                sohd = s.Trim();
                                             

                                            vtien = CtData.AsEnumerable()
                                                        .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString()&& (sohd.Equals("5") ? 1 == 1 : b.Field<object>("ngay_ct0") != null))
                                                        .Sum(x => x.Field<decimal?>("tien_nt"));
                                            if (vtien != null)
                                                Decimal.TryParse(vtien.ToString(), out _t_tien_ht);

                                            vtien = CtData.AsEnumerable()
                                                        .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString() && (sohd.Equals("5") ? 1 == 1 : b.Field<object>("ngay_ct0") != null))
                                                        .Sum(x => x.Field<decimal?>("tien"));
                                            if (vtien != null)
                                                Decimal.TryParse(vtien.ToString(), out _t_tien_nt0_ht);

                                            vthue = CtData.AsEnumerable()
                                                        .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString() && (sohd.Equals("5") ? 1 == 1 : b.Field<object>("ngay_ct0") != null))
                                                        .Sum(x => x.Field<decimal?>("thue_nt"));
                                            if (vthue != null)
                                                Decimal.TryParse(vthue.ToString(), out _t_thue_ht);

                                            vthue = CtData.AsEnumerable()
                                                        .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())// && (sohd.Equals("5") ? 1 == 1 : b.Field<object>("ngay_ct0") != null))
                                                        .Sum(x => x.Field<decimal?>("thue"));
                                            if (vthue != null)
                                                Decimal.TryParse(vthue.ToString(), out _t_thue_nt0_ht);


                                            Decimal _t_tien_gt = 0, _t_thue_gt = 0;
                                            Decimal _t_tien_nt0_gt = 0, _t_thue_nt0_gt = 0;
                                            DataTable gtData = StartUp.DsTrans.Tables[2];

                                            vtien = gtData.AsEnumerable()
                                                        .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString() && (sohd.Equals("5") ? 1 == 1 : b.Field<object>("ngay_ct0") != null))
                                                        .Sum(x => x.Field<decimal?>("t_tien_nt"));
                                            if (vtien != null)
                                                Decimal.TryParse(vtien.ToString(), out _t_tien_gt);

                                            vtien = gtData.AsEnumerable()
                                                        .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString() && (sohd.Equals("5") ? 1 == 1 : b.Field<object>("ngay_ct0") != null))
                                                        .Sum(x => x.Field<decimal?>("t_tien"));
                                            if (vtien != null)
                                                Decimal.TryParse(vtien.ToString(), out _t_tien_nt0_gt);

                                            vthue = gtData.AsEnumerable()
                                                .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString() && (sohd.Equals("5") ? 1 == 1 : b.Field<object>("ngay_ct0") != null))
                                                        .Sum(x => x.Field<decimal?>("t_thue_nt"));
                                            if (vthue != null)
                                                Decimal.TryParse(vthue.ToString(), out _t_thue_gt);

                                            vthue = gtData.AsEnumerable()
                                                        .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString() && (sohd.Equals("5") ? 1 == 1 : b.Field<object>("ngay_ct0") != null))
                                                        .Sum(x => x.Field<decimal?>("t_thue"));
                                            if (vthue != null)
                                                Decimal.TryParse(vthue.ToString(), out _t_thue_nt0_gt);


                                          //  if (cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
                                        //    {
                                            if (_t_tien_gt != _t_tien_ht)
                                            {
                                                if (PhView[0]["sua_thue"].ToString() == "0")
                                                {
                                                    ExMessageBox.Show(340, StartUp.SysObj, "Tổng tiền/ tiền ngoại tệ khác với tổng tiền/ tiền ngoại tệ trong các hóa đơn giá trị gia tăng!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    isError = true;
                                                }
                                            }


                                            if (!isError && _t_thue_gt != _t_thue_ht)
                                            {
                                                if (PhView[0]["sua_thue"].ToString() == "1" && PhView[0]["sua_tien"].ToString() == "1")
                                                { }
                                                else
                                                {
                                                    ExMessageBox.Show(345, StartUp.SysObj, "Tổng tiền thuế/ thuế ngoại tệ khác với tổng tiền thuế/ thuế ngoại tệ trong các hóa đơn giá trị gia tăng!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    isError = true;
                                                }
                                            }

                                            if (!cbMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()) && !isError)
                                            {
                                         
                                                if (_t_tien_nt0_gt != _t_tien_nt0_ht)
                                                {
                                                    if (PhView[0]["sua_thue"].ToString() == "0")
                                                    {
                                                        ExMessageBox.Show( 350,StartUp.SysObj, "Tổng tiền/ tiền ngoại tệ khác với tổng tiền/ tiền ngoại tệ trong các hóa đơn giá trị gia tăng!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                                        isError = true;
                                                    }
                                                }
                                                else if (_t_thue_nt0_gt != _t_thue_nt0_ht)
                                                {
                                                    if (PhView[0]["sua_thue"].ToString() == "1" && PhView[0]["sua_tien"].ToString() == "1")
                                                    { }
                                                    else
                                                    {
                                                        ExMessageBox.Show(355, StartUp.SysObj, "Tổng tiền thuế/ thuế ngoại tệ khác với tổng tiền thuế/ thuế ngoại tệ trong các hóa đơn giá trị gia tăng!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                                        isError = true;
                                                    }
                                                }
                                            }
                                            //Cập nhật lại tổng tiền tab thuế
                                            UpdateTotalThue();                                     
                                        }
                                        else
                                        {
                                            UpdateTotalHT();
                                        }
                                        break;
                                }
                            }
                        }
                        if (!isError)
                        {
                            if (!IsSequenceSave)
                            {
                                //Cân bằng trường tiền
                                Decimal _ty_gia = txtTy_gia.nValue;
                                Decimal _ty_gia_ht = ArapLib.FNum.ToDec(txtTy_gia_ht.nValue);

                                if (!cbMa_nt.Text.Trim().Equals(M_Ma_nt0) && GrdCt.Records.Count > 0 && _ty_gia != 0 && !ChkSuaTien.IsChecked.Value)
                                {
                                    if (Ma_GD_Value.Text.Equals("2"))
                                    {
                                        Decimal t_tien_hang = 0;
                                        var vtien_nt = CtData.AsEnumerable()
                                                        .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                                                        .Sum(x => x.Field<decimal?>("tien_nt"));
                                        if (vtien_nt != null)
                                            Decimal.TryParse(vtien_nt.ToString(), out t_tien_hang);

                                        //Tính tiền hàng vnd
                                        Decimal _sum_tien = SmLib.SysFunc.Round(_ty_gia * t_tien_hang, M_Round);

                                        Decimal _sum_tien_nt0 = 0;
                                        var vtien_nt0 = CtData.AsEnumerable()
                                            .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                                            .Sum(x => x.Field<decimal?>("tien_tt"));
                                        _sum_tien_nt0 = FNum.ToDec(vtien_nt0);

                                        Decimal tien = FNum.ToDec(CtView[0]["tien_tt"]);
                                        tien += _sum_tien - _sum_tien_nt0;
                                        CtView[0]["tien_tt"] = tien;

                                        if (CtData.AsEnumerable().All(x => isEquals(FNum.ToDec(x.Field<object>("ty_gia_ht2")), Ty_gia, x["stt_rec"].ToString())))
                                            CtView[0]["tien"] = tien;
                                        UpdateTotalHT();
                                    }
                                    else
                                    {
                                        //Phiếu hd số 2
                                        var v_drHd2 = CtData.AsEnumerable()
                                                                .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString() && b.Field<string>("loai_hd") == "2")
                                                                .Count();
                                        //Phiếu sai: ngoại tệ=0, vnd!=0
                                        var v_so_phieu_sai = CtData.AsEnumerable()
                                                                           .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString() && (b.Field<decimal?>("tien_nt") == 0 || _ty_gia == 0) && b.Field<decimal?>("tien") != 0 && b.Field<string>("loai_hd") != "2")
                                                                           .Count();
                                        if (v_so_phieu_sai + v_drHd2 < GrdCt.Records.Count)
                                        {
                                            //Tính tiền hàng ngoại tệ
                                            Decimal t_tien_hang = 0;
                                            var vtien_nt = CtData.AsEnumerable()
                                                            .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                                                            .Sum(x => x.Field<decimal?>("tien_nt"));
                                            t_tien_hang = FNum.ToDec(vtien_nt);

                                            //Tính tiền hàng vnd
                                            Decimal _sum_tien = SmLib.SysFunc.Round(_ty_gia * t_tien_hang, M_Round);

                                            txtT_Tien_Nt0.Value = _sum_tien;
                                            ////Gán số dư cho phiếu đầu tiên
                                            Decimal _sum_tien_nt0 = 0;
                                            var vtien_nt0 = CtData.AsEnumerable()
                                                .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                                                .Sum(x => x.Field<decimal?>("tien"));
                                            _sum_tien_nt0 = FNum.ToDec(vtien_nt0);

                                            Decimal _sum_tien_ht = 0;
                                            if (StartUp.M_Gd_2Tg_List.Contains(Ma_gd))
                                                _sum_tien_ht = SmLib.SysFunc.Round(_ty_gia_ht * t_tien_hang, M_Round);
                                            else
                                                _sum_tien_ht = SmLib.SysFunc.Round(_ty_gia * t_tien_hang, M_Round);

                                            Decimal _sum_tien_tt = 0;
                                            var vtien_tt = CtData.AsEnumerable()
                                                .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                                                .Sum(x => x.Field<decimal?>("tien_tt"));
                                            _sum_tien_tt = FNum.ToDec(vtien_tt);



                                            Decimal tien_tt = FNum.ToDec(CtView[0]["tien_tt"]);
                                            tien_tt += _sum_tien_ht - _sum_tien_tt;
                                           // CtView[0]["tien_tt"] = tien_tt;

                                            for (int i = 0; i < GrdCt.Records.Count; i++)
                                            {
                                                DataRecord dr = GrdCt.Records[i] as DataRecord;
                                                Decimal tien = 0, tien_nt = 0, thue = 0;
                                                tien = FNum.ToDec(dr.Cells["tien"].Value);
                                                tien_nt = FNum.ToDec(dr.Cells["tien_nt"].Value);
                                                if (tien_nt == 0 && tien != 0)
                                                {
                                                    //Phiếu sai
                                                }
                                                //else if (dr.Cells["loai_hd"].Value.ToString().Equals("1"))
                                                else if (!string.IsNullOrEmpty(dr.Cells["ngay_ct0"].Value.ToString()))
                                                {
                                                    tien = tien + (_sum_tien - _sum_tien_nt0);
                                                    dr.Cells["tien"].Value = tien;
                                                    dr.Cells["tien_tt"].Value = tien_tt;
                                                    thue = FNum.ToDec(dr.Cells["thue"].Value);
                                                    dr.Cells["tt"].Value = tien + thue;
                                                    //chuyển cho tab hóa đơn.
                                                    for (int j = 0; j < GrdCtgt.Records.Count; j++)
                                                    {
                                                        DataRecord drgt = GrdCtgt.Records[j] as DataRecord;
                                                        if (drgt.Cells["so_ct0"].Value.Equals(dr.Cells["so_ct0"].Value))
                                                        {
                                                            drgt.Cells["t_tien"].Value = tien;
                                                            drgt.Cells["t_tt"].Value = tien + thue;
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                }
                                            }
                                            //Tính tiền thuế
                                            Decimal _sum_thue_nt0 = 0;
                                            _sum_thue_nt0 = txtT_thue_Nt0.Value == DBNull.Value ? 0 : Convert.ToDecimal(txtT_thue_Nt0.nValue);

                                            //Tính tổng thanh toán
                                            txtT_tt_Nt0.Value = _sum_thue_nt0 + _sum_tien;
                                        }
                                    }
                                }

                                PhData.AcceptChanges();
                                CtData.AcceptChanges();
                                StartUp.DsTrans.Tables[2].AcceptChanges();
                                ////Kiểm tra tiền tại quỹ
                                //if (StartUp.M_KT_CHI_TQ != 0)
                                //    for (int i = 0; i < StartUp.M_TK_TAI_QUY.Length; i++)
                                //    {
                                //        if (PhView[0]["tk"].ToString().StartsWith(StartUp.M_TK_TAI_QUY[i]))
                                //        {
                                //            if (!StartUp.CheckQuy(PhView[0]["tk"].ToString(), Convert.ToDateTime(PhView[0]["ngay_ct"].ToString())))
                                //            {
                                //                ExMessageBox.Show( 360,StartUp.SysObj, "Chi quá số tiền tại quỹ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                //                if (StartUp.M_KT_CHI_TQ == 2)
                                //                    return;
                                //            }
                                //        }
                                //    }
                                // update thông tin cho các record Table0 (Ph)
                                if (string.IsNullOrEmpty(PhView[0]["ma_gd"].ToString()))
                                    PhView[0]["ma_gd"] = StartUp.DmctInfo["ma_gd"];
                                if (string.IsNullOrEmpty(PhView[0]["ma_dvcs"].ToString()))
                                    PhView[0]["ma_dvcs"] = StartUp.SysObj.M_ma_dvcs.ToString();


                            }
                            Decimal _t_tien = 0, _t_tien_nt0 = 0;
                            var vtien = CtData.AsEnumerable()
                                        .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                                        .Sum(x => x.Field<decimal?>("tien_nt"));
                            if (vtien != null)
                                Decimal.TryParse(vtien.ToString(), out _t_tien_nt0);

                            vtien = CtData.AsEnumerable()
                                        .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                                        .Sum(x => x.Field<decimal?>("tien"));
                            if (vtien != null)
                                Decimal.TryParse(vtien.ToString(), out _t_tien);
                            if (_t_tien_nt0 == 0 && _t_tien == 0)
                            {
                                if (StartUp.M_CHK_ZERO == 1)
                                {
                                    ExMessageBox.Show( 365,StartUp.SysObj, "Hạch toán tiền bằng 0!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else if (StartUp.M_CHK_ZERO == 2)
                                {
                                    ExMessageBox.Show( 370,StartUp.SysObj, "Hạch toán tiền bằng 0, không lưu được!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                    isError = true;
                                }
                            }

                            if (!isError)
                            {
                                if (!IsSequenceSave)
                                {
                                    if (Ma_GD_Value.Text.Equals("8"))
                                    {
                                        if (TabInfo.SelectedIndex == 0)
                                            UpdateTotalHT();
                                        else
                                            UpdateTotalThue();
                                    }
                                }

                                DataTable tbPhToSave = PhData.Clone();
                                tbPhToSave.Rows.Add(PhView[0].Row.ItemArray);
                                if (!IsSequenceSave)
                                {
                                    tbPhToSave.Rows[0]["status"] = 0;
                                }
                                DataProvider.UpdateDataTable(StartUp.SysObj, StartUp.DmctInfo["m_phdbf"].ToString(), "stt_rec", tbPhToSave, "stt_rec;row_id");

                               
                                //DataProvider.DeleteRow(StartUp.SysObj, StartUp.DmctInfo["m_ctdbf"].ToString(), "stt_rec='" + PhView[0]["stt_rec"] + "'");
                                //DataProvider.DeleteRow(StartUp.SysObj, StartUp.DmctInfo["m_ctgtdbf"].ToString(), "stt_rec='" + PhView[0]["stt_rec"] + "'");

                                DataTable tbCtToSave = CtData.Clone();
                                DataTable tbCtGtToSave = StartUp.DsTrans.Tables[2].Clone();

                                foreach (DataRowView drv in CtView)
                                {
                                    if (!IsSequenceSave)
                                    {
                                        // update thông tin cho các record Table1 (Ct) 
                                        drv["ngay_ct"] = PhView[0]["ngay_ct"];
                                        drv["so_ct"] = PhView[0]["so_ct"];
                                        drv["ma_ct"] = StartUp.Ma_ct;
                                    }
                                    tbCtToSave.Rows.Add(drv.Row.ItemArray);
                                }

                                foreach (DataRowView drv in StartUp.DsTrans.Tables[2].DefaultView)
                                {
                                    if (!IsSequenceSave)
                                    {
                                        // update thông tin cho các record Table2 (Ctgt) 
                                        drv["ma_nt"] = PhView[0]["ma_nt"];
                                        drv["ty_gia"] = PhView[0]["ty_gia"];
                                        drv["ty_giaf"] = PhView[0]["ty_giaf"];
                                        drv["status"] = PhView[0]["status"];
                                        drv["ma_gd"] = PhView[0]["ma_gd"];
                                        drv["so_ct"] = PhView[0]["so_ct"];
                                    }
                                    tbCtGtToSave.Rows.Add(drv.Row.ItemArray);
                                }

                                if (!DataProvider.UpdateCtTable(StartUp.SysObj, StartUp.DmctInfo["m_ctdbf"].ToString(), tbCtToSave, PhView[0]["stt_rec"].ToString()))
                                {
                                    ExMessageBox.Show( 375,StartUp.SysObj, "Lưu không thành công, kiểm tra lại dữ liệu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                    return;
                                }
                                if (!DataProvider.UpdateCtTable(StartUp.SysObj, StartUp.DmctInfo["m_ctgtdbf"].ToString(), tbCtGtToSave, PhView[0]["stt_rec"].ToString()))
                                {
                                    ExMessageBox.Show( 380,StartUp.SysObj, "Lưu không thành công, kiểm tra lại dữ liệu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                    return;
                                }
                                //StartUp.UpdateRates(tbPhToSave.Rows[0]["ma_nt"].ToString(), Convert.ToDateTime(txtNgay_ct.Value).Date, Convert.ToDecimal(txtTy_gia.Value)); 
                            }
                        }
                        if (!IsSequenceSave)
                        {
                            #region kiểm tra dưới database
                            if (!isError)
                            {
                                    dsCheckData = StartUp.CheckData();

                                if (dsCheckData.Tables.Count > 0)
                                    dsCheckData.Tables[dsCheckData.Tables.Count - 1].AcceptChanges();
                                if (dsCheckData.Tables.Count > 0 && dsCheckData.Tables[dsCheckData.Tables.Count - 1].Rows.Count > 0)
                                {
                                    DataTable dtCheck = dsCheckData.Tables[dsCheckData.Tables.Count - 1];
                                    foreach (DataRowView dv in dtCheck.DefaultView)
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
                                                        if (ExMessageBox.Show( 385,StartUp.SysObj, "Có chứng từ trùng số. Số cuối cùng là: " + "[" + GetLastSoct(StartUp.SysObj, txtMa_qs.Text).Trim() + "]" + ". Có lưu chứng từ này không?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                                                        {
                                                            txtSo_ct.SelectAll();
                                                            txtSo_ct.Focus();
                                                            isError = true;
                                                        }
                                                    }
                                                    else if (StartUp.M_trung_so.Equals("2"))
                                                    {
                                                        ExMessageBox.Show( 390,StartUp.SysObj, "Số chứng từ đã tồn tại!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                                        txtSo_ct.SelectAll();
                                                        txtSo_ct.Focus();
                                                        isError = true;
                                                    }
                                                }
                                                break;
                                            case "PH02":
                                                {
                                                    ExMessageBox.Show( 395,StartUp.SysObj, "Tk có là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    isError = true;
                                                    txtMa_nx.IsFocus = true;
                                                }
                                                break;
                                            case "PH03":
                                                {
                                                    if (StartUp.M_KT_CHI_TQ != 0)
                                                    {
                                                        ExMessageBox.Show( 400,StartUp.SysObj, "Chi quá số tiền tại quỹ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                        if (StartUp.M_KT_CHI_TQ == 2)
                                                            isError = true;
                                                    }
                                                }
                                                break;
                                            case "PH04":
                                                {
                                                    #region kiem tra chi am

                                                    bool allowSave = true;
                                                    DataTable tbChiAm;
                                                    string tkChiAm = "";

                                                    tbChiAm = dsCheckData.Tables[0].Copy();
                                                    if (tbChiAm != null && tbChiAm.Rows.Count > 0)
                                                    {
                                                        for (int i = 0; i < tbChiAm.Rows.Count; i++)
                                                        {
                                                            if (tbChiAm.Rows[i]["warning"].ToString().Trim().Equals("1"))
                                                            {
                                                                tkChiAm += tbChiAm.Rows[i]["tk"].ToString().Trim();
                                                                tkChiAm += ",";
                                                            }
                                                            else if (tbChiAm.Rows[i]["warning"].ToString().Trim().Equals("2"))
                                                            {
                                                                allowSave = false;
                                                                tkChiAm += tbChiAm.Rows[i]["tk"].ToString().Trim();
                                                                tkChiAm += ",";
                                                            }
                                                        }

                                                        if (tkChiAm.Trim().Length > 0)
                                                        {
                                                            tkChiAm = tkChiAm.Substring(0, tkChiAm.Length - 1);
                                                            if (allowSave)
                                                            {
                                                                ExMessageBox.Show(900, StartUp.SysObj, string.Format("Tài khoản [{0}] chi quá số tiền tại quỹ!", tkChiAm), "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                            }
                                                            else
                                                            {
                                                                ExMessageBox.Show(905, StartUp.SysObj, string.Format("Tài khoản [{0}] chi quá số tiền tại quỹ, không lưu được!", tkChiAm), "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                                isError = true;
                                                            }
                                                        }
                                                    }
                                                    #endregion
                                                }
                                                break;
                                            case "CT01":
                                                {
                                                    int index = Convert.ToInt16(dv[1]);
                                                    ExMessageBox.Show( 405,StartUp.SysObj, "Tk nợ là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    isError = true;
                                                    if (Ma_GD_Value.Text.Trim().Equals("8"))
                                                    {
                                                        tiHT.Focus();
                                                        GrdCt.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                                        {
                                                            GrdCt.ActiveCell = (GrdCt.Records[index] as DataRecord).Cells["tk_i"];
                                                            GrdCt.Focus();
                                                        }));
                                                    }
                                                    else
                                                    {
                                                        GrdCtChi.ActiveCell = (GrdCtChi.Records[index] as DataRecord).Cells["tk_i"];
                                                        GrdCtChi.Focus();
                                                    }
                                                }
                                                break;
                                            case "CT02":
                                                {
                                                    int index = Convert.ToInt16(dv[1]);
                                                    if (!isError && Ma_GD_Value.Text.Trim().Equals("8"))
                                                    {
                                                        ExMessageBox.Show( 410,StartUp.SysObj, "Tk thuế là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                        isError = true;
                                                        tiHT.Focus();
                                                        GrdCt.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                                        {
                                                            GrdCt.ActiveCell = (GrdCt.Records[index] as DataRecord).Cells["tk_thue_i"];
                                                            GrdCt.Focus();
                                                        }));
                                                    }
                                                }
                                                break;
                                        case "CT08":
                                            {
                                                int index = Convert.ToInt16(dv[1]);

                                                ExMessageBox.Show(9410, StartUp.SysObj, "Chưa vào mã vv, mã bộ phận, mã phí => không lưu được kiểm tra lại dữ liệu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                isError = true;

                                            }
                                            break;
                                        case "GT01":
                                                {
                                                    int index = Convert.ToInt16(dv[1]);
                                                    ExMessageBox.Show( 415,StartUp.SysObj, "Tk thuế là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    isError = true;
                                                    GrdCtgt.ActiveCell = (GrdCtgt.Records[index] as DataRecord).Cells["tk_thue_no"];
                                                    GrdCtgt.Focus();
                                                }
                                                break;
                                        }
                                        dtCheck.Rows.Remove(dv.Row);
                                    }
                                }
                            }
                            #endregion
                        }
                        if (!isError)
                        {
                            ThreadStart _thread = delegate()
                            {
                                Post();
                            };
                            new Thread(_thread).Start();
                            if (!IsSequenceSave)
                            {
                                int iRowNew = GetiRow(PhData, CtView[0]["stt_rec"].ToString());
                                if (iRow != iRowNew)
                                {
                                    DataRow oldRow = PhView[0].Row;
                                    DataRow newRow = PhData.NewRow();
                                    newRow.ItemArray = oldRow.ItemArray;
                                    if (iRow > iRowNew)
                                        PhData.Rows.InsertAt(newRow, iRowNew);
                                    else
                                        PhData.Rows.InsertAt(newRow, iRowNew + 1);
                                    PhData.AcceptChanges();
                                    PhData.Rows.Remove(oldRow);
                                    PhData.AcceptChanges();
                                    iRow = iRowNew;
                                }
                                IsInEditMode.Value = false;
                                currActionTask = ActionTask.View;
                            }
                        }
                    }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }

            ww.Stop();
            Debug.WriteLine(ww.ElapsedMilliseconds, "StopWatch");
            Debug.WriteLine(DateTime.Now.ToString("hh:mm:ss"), "- * - * - * - * - * End V_Nhan");
        }

        void Post()
        {
            SqlCommand cmd = new SqlCommand("EXEC [dbo].[CACTPC1-Post] @Stt_rec, @Ma_ct");
            cmd.Parameters.Add("@stt_rec", SqlDbType.VarChar, 50).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"];
            cmd.Parameters.Add("@Ma_ct", SqlDbType.Char, 3).Value = StartUp.Ma_ct;
            //DataSet ds = StartUp.SysObj.ExcuteReader(cmd);
            StartUp.SysObj.ExcuteNonQuery(cmd);
        }
        private void btnNhan_Click(object sender, RoutedEventArgs e)
        {
            V_Nhan();
        }
        #endregion

        private void V_Xem()
        {
            currActionTask = ActionTask.View;
            //  set lai stringbrowse 
            string stringBrowse1 = StartUp.CommandInfo[StartUp.M_LAN == "V" ? "Vbrowse2" : "Ebrowse2"].ToString().Split('|')[0];
            string stringBrowse2 = StartUp.CommandInfo[StartUp.M_LAN == "V" ? "Vbrowse2" : "Ebrowse2"].ToString().Split('|')[1];
            DataTable PhViewTablev = PhData.Copy();
            PhViewTablev.Rows.RemoveAt(0);
            SmVoucherLib.FormView _frmView = new SmVoucherLib.FormView(StartUp.SysObj, PhViewTablev.DefaultView, CtView, stringBrowse1, stringBrowse2, "stt_rec");
            SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, _frmView.frmBrw.oBrowseCt, StartUp.Ma_ct, 1);
            _frmView.frmBrw.Title = SmLib.SysFunc.Cat_Dau(M_LAN.Equals("V") ? StartUp.CommandInfo["bar"].ToString() : StartUp.CommandInfo["bar2"].ToString());
            _frmView.frmBrw.ShowInTaskbar = false;
            _frmView.ListFieldSum = "t_tt_nt;t_tt";

            _frmView.frmBrw.LanguageID  = "CACTBN1_4";
            _frmView.ShowDialog();

            // Set lai irow va rowfilter ...
            if (_frmView.DataGrid.ActiveRecord != null)
            {

                int select_irow = (_frmView.DataGrid.ActiveRecord as DataRecord).Index;
                if (select_irow >= 0)
                {
                    string selected_stt_rec = (_frmView.DataGrid.DataSource as DataView)[select_irow]["stt_rec"].ToString();
                    FrmCACTBN1.iRow = select_irow + 1;
                    PhView.RowFilter = "stt_rec= '" + selected_stt_rec + "'";
                    CtView.RowFilter = "stt_rec= '" + selected_stt_rec + "'";
                    StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + selected_stt_rec + "'";

                }
            }
        }

        private void V_Tim()
        {
            try
            {
                currActionTask = ActionTask.View;
                FrmTim _FrmTim3 = new FrmTim(StartUp.SysObj, StartUp.filterId, StartUp.filterView);
                SmLib.SysFunc.LoadIcon(_FrmTim3);
                _FrmTim3.ShowDialog();

            }
            catch (Exception ex)
            {

                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        private void GrdCtgt_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            try
            {
                if (IsInEditMode.Value && GrdCtgt.ActiveCell != null && StartUp.DsTrans.Tables[2].DefaultView.Count > GrdCtgt.ActiveRecord.Index && StartUp.DsTrans.Tables[2].GetChanges(DataRowState.Deleted) == null)
                    switch (e.Cell.Field.Name)
                    {
                        case "ma_kh":
                            {
                                AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                if (txt.RowResult != null)
                                {
                                    
                                   e.Cell.Record.Cells["ten_kh"].Value = txt.RowResult["ten_kh"] ;//: txt.RowResult["ten_kh2"];
                                   e.Cell.Record.Cells["dia_chi"].Value = txt.RowResult["dia_chi"];
                                   e.Cell.Record.Cells["ma_so_thue"].Value = txt.RowResult["ma_so_thue"];
                                }
                            }
                            break;
                        case "t_tien_nt":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    //if (!IsCheckedSua_tien.Value)
                                        if (e.Editor.Value != DBNull.Value && txtTy_gia.Value != null)
                                        {
                                            Decimal _tien_nt = 0, _thue_nt = 0;
                                            Decimal.TryParse(e.Cell.Record.Cells["t_tien_nt"].Value.ToString(), out _tien_nt);
                                            if (e.Cell.Record.Cells["thue_suat"].Value != DBNull.Value)
                                            {
                                                Decimal _thue_suat = 0;
                                                Decimal.TryParse(e.Cell.Record.Cells["thue_suat"].Value.ToString(), out _thue_suat);
                                                _thue_nt = _tien_nt * _thue_suat / 100;
                                                if (!cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
                                                {
                                                    _thue_nt = SmLib.SysFunc.Round(_thue_nt, M_Round_nt);
                                                }
                                                else
                                                {
                                                    _thue_nt = SmLib.SysFunc.Round(_thue_nt, M_Round);
                                                }
                                                e.Cell.Record.Cells["t_thue_nt"].Value = _thue_nt;

                                                e.Cell.Record.Cells["t_tt_nt"].Value = _tien_nt + _thue_nt;

                                                if (!cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
                                                {
                                                    Decimal _tien_nt0 = 0, _thue_nt0 = 0;
                                                    Decimal _Ty_gia = txtTy_gia.nValue;
                                                    _tien_nt0 = SmLib.SysFunc.Round(_Ty_gia * _tien_nt, M_Round);
                                                    if (_tien_nt0 > 0)
                                                    {
                                                        e.Cell.Record.Cells["t_tien"].Value = _tien_nt0;

                                                        _thue_nt0 = SmLib.SysFunc.Round(_tien_nt0 * _thue_suat / 100, M_Round);
                                                        e.Cell.Record.Cells["t_thue"].Value = _thue_nt0;

                                                        e.Cell.Record.Cells["t_tt"].Value = _tien_nt0 + _thue_nt0;
                                                    }
                                                }
                                                else
                                                {
                                                    e.Cell.Record.Cells["t_tien"].Value = _tien_nt;
                                                    e.Cell.Record.Cells["t_thue"].Value = _thue_nt;
                                                    e.Cell.Record.Cells["t_tt"].Value = _tien_nt + _thue_nt;
                                                }
                                            }
                                            else
                                            {
                                                e.Cell.Record.Cells["t_tt_nt"].Value = _tien_nt;

                                                Decimal _Ty_gia = 0;
                                                _Ty_gia = txtTy_gia.nValue;
                                                Decimal tien = SmLib.SysFunc.Round(_Ty_gia * _tien_nt, M_Round);
                                                if (tien > 0)
                                                {
                                                    e.Cell.Record.Cells["t_tien"].Value = tien;
                                                    e.Cell.Record.Cells["t_tt"].Value = tien;
                                                }
                                            }
                                        }
                                    UpdateTotalThue();
                                }
                            }
                            break;
                        case "t_tien":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    //if (!IsCheckedSua_tien.Value)
                                    //{
                                        if (e.Editor.Value == DBNull.Value || e.Editor.Value.ToString().Trim().Equals("0"))
                                        {
                                            Decimal tien_nt = Convert.ToDecimal(e.Cell.Record.Cells["t_tien_nt"].Value.Equals(DBNull.Value) ? 0 : e.Cell.Record.Cells["t_tien_nt"].Value);
                                            Decimal _Ty_gia = txtTy_gia.nValue;
                                            e.Cell.Record.Cells["t_tien"].Value = SmLib.SysFunc.Round(_Ty_gia * tien_nt, M_Round);
                                        }

                                        Decimal _tien_nt0 = 0, _thue_nt0 = 0;
                                        Decimal.TryParse(e.Cell.Record.Cells["t_tien"].Value.ToString(), out _tien_nt0);
                                        if (e.Cell.Record.Cells["thue_suat"].Value != DBNull.Value && txtTy_gia.Value != null)
                                        {
                                            Decimal _thue_suat = 0;
                                            Decimal.TryParse(e.Cell.Record.Cells["thue_suat"].Value.ToString(), out _thue_suat);

                                            _thue_nt0 = SmLib.SysFunc.Round(_tien_nt0 * _thue_suat / 100, M_Round);
                                            e.Cell.Record.Cells["t_thue"].Value = _thue_nt0;

                                        }
                                        e.Cell.Record.Cells["t_tt"].Value = _tien_nt0 + _thue_nt0;
                                    //}
                                    UpdateTotalThue();
                                }
                            }
                            break;
                        case "ma_thue":
                            {
                                AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                if (txt.IsDataChanged)
                                {
                                    //Cập nhật tài khoản thuế
                                    if (txt.RowResult != null)
                                    {
                                        e.Cell.Record.Cells["tk_thue_no"].Value = txt.RowResult["tk_thue_no"];
                                        e.Cell.Record.Cells["thue_suat"].Value = txt.RowResult["thue_suat"];
                                    }

                                    if (!string.IsNullOrEmpty(e.Cell.Record.Cells["thue_suat"].Value.ToString()))
                                    {
                                        //txtT_thue_Nt0.Value = StartUp.DsTrans.Tables[2].Compute("sum(t_thue)", "stt_rec= '" + PhView[0]["stt_rec"].ToString() + "'");
                                        Decimal tien_nt = Convert.ToDecimal(e.Cell.Record.Cells["t_tien_nt"].Value.Equals(DBNull.Value) ? 0 : e.Cell.Record.Cells["t_tien_nt"].Value);
                                        Decimal tien = Convert.ToDecimal(e.Cell.Record.Cells["t_tien"].Value.Equals(DBNull.Value) ? 0 : e.Cell.Record.Cells["t_tien"].Value);
                                        Decimal thue_suat = Convert.ToDecimal(e.Cell.Record.Cells["thue_suat"].Value.Equals(DBNull.Value) ? 0 : e.Cell.Record.Cells["thue_suat"].Value);
                                        Decimal thue_nt = 0, thue = 0;

                                        if (!cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
                                        {
                                            thue_nt = SmLib.SysFunc.Round(tien_nt * thue_suat / 100, M_Round_nt);
                                        }
                                        else
                                        {
                                            thue_nt = SmLib.SysFunc.Round(tien_nt * thue_suat / 100, M_Round);
                                        }
                                        e.Cell.Record.Cells["t_thue_nt"].Value = thue_nt;
                                        thue = SmLib.SysFunc.Round(tien * thue_suat / 100, M_Round);
                                        e.Cell.Record.Cells["t_thue"].Value = thue;

                                        e.Cell.Record.Cells["t_tt_nt"].Value = thue_nt + tien_nt;
                                        e.Cell.Record.Cells["t_tt"].Value = thue + tien;
                                        UpdateTotalThue();
                                    }
                                }
                            }
                            break;
                        case "tk_thue_no":
                            {
                                AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                //Cập nhật tài khoản thuế
                                if (txt.RowResult != null)
                                {
                                    DataRowView drVCT = e.Cell.Record.DataItem as DataRowView;
                                    drVCT["tk_thue_cn"] = txt.RowResult["tk_cn"];
                                    //Update Binding
                                    CellValuePresenter cellV = CellValuePresenter.FromCell(e.Cell.Record.Cells["ma_kh2"]);
                                    ControlFunction.RefreshSingleBinding(cellV, AutoCompleteTextBox.IsReadOnlyProperty);
                                }
                            }
                            break;
                        case "t_thue_nt":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Cell.Record.Cells["t_thue_nt"].Value == DBNull.Value || e.Cell.Record.Cells["t_thue_nt"].Value.ToString().Trim().Equals("0"))
                                    {
                                        decimal tien_nt = Convert.ToDecimal(e.Cell.Record.Cells["t_tien_nt"].Value.Equals(DBNull.Value) ? 0 : e.Cell.Record.Cells["t_tien_nt"].Value);
                                        decimal thue_suat = Convert.ToDecimal(e.Cell.Record.Cells["thue_suat"].Value.Equals(DBNull.Value) ? 0 : e.Cell.Record.Cells["thue_suat"].Value);
                                        decimal thue_nt = tien_nt * thue_suat / 100;
                                        if (!cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
                                        {
                                            thue_nt = SmLib.SysFunc.Round(thue_nt, M_Round_nt);
                                        }
                                        else
                                        {
                                            thue_nt = SmLib.SysFunc.Round(thue_nt, M_Round);
                                        }
                                        e.Cell.Record.Cells["t_thue_nt"].Value = thue_nt;
                                    }
                                    if (txtTy_gia.Value != null && !string.IsNullOrEmpty(e.Editor.Text))
                                    {
                                        Decimal _t_tien = 0, _t_thue = 0;
                                        Decimal.TryParse(e.Cell.Record.Cells["t_tien_nt"].Value.ToString(), out _t_tien);
                                        Decimal.TryParse(e.Cell.Record.Cells["t_thue_nt"].Value.ToString(), out _t_thue);
                                        e.Cell.Record.Cells["t_tt_nt"].Value = _t_tien + _t_thue;
                                    }

                                    if (cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
                                    {
                                        e.Cell.Record.Cells["t_tien"].Value = e.Cell.Record.Cells["t_tien_nt"].Value;
                                        e.Cell.Record.Cells["t_thue"].Value = e.Editor.Value;
                                        e.Cell.Record.Cells["t_tt"].Value = e.Cell.Record.Cells["t_tt_nt"].Value;
                                    }
                                    // Sửa thuế nt tự tính lại thuế vnd
                                    else
                                    {
                                        decimal _Ty_gia = txtTy_gia.nValue;
                                        decimal thue_nt = Convert.ToDecimal(e.Cell.Record.Cells["t_thue_nt"].Value.Equals(DBNull.Value) ? 0 : e.Cell.Record.Cells["t_thue_nt"].Value);
                                        decimal thue = thue_nt * _Ty_gia;
                                        if (thue > 0)
                                        {
                                            e.Cell.Record.Cells["t_thue"].Value = SmLib.SysFunc.Round(thue, M_Round);
                                            decimal tien = Convert.ToDecimal(e.Cell.Record.Cells["t_tien"].Value.Equals(DBNull.Value) ? 0 : e.Cell.Record.Cells["t_tien"].Value);
                                            e.Cell.Record.Cells["t_tt"].Value = tien + SmLib.SysFunc.Round(thue, M_Round);
                                        }
                                    }
                                    UpdateTotalThue();
                                }
                            }
                            break;
                        case "t_thue":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Cell.Record.Cells["t_thue"].Value == DBNull.Value || e.Cell.Record.Cells["t_thue"].Value.ToString().Trim().Equals("0"))
                                    {
                                        decimal tien = Convert.ToDecimal(e.Cell.Record.Cells["t_tien"].Value.Equals(DBNull.Value) ? 0 : e.Cell.Record.Cells["t_tien"].Value);
                                        decimal thue_suat = Convert.ToDecimal(e.Cell.Record.Cells["thue_suat"].Value.Equals(DBNull.Value) ? 0 : e.Cell.Record.Cells["thue_suat"].Value);
                                        e.Cell.Record.Cells["t_thue"].Value = SmLib.SysFunc.Round(tien * thue_suat / 100, M_Round);
                                    }
                                    UpdateTotalThue();
                                }
                                break;
                            }
                        default:
                            break;
                    }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        private bool GrdCtgt_AddNewRecord(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            return NewRowCtGt();
        }

        private void cbMa_nt_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Voucher_Ma_nt0 == null)
                return;
            if (cbMa_nt.IsDataChanged)
            {
                StartUp.DsTrans.Tables[0].DefaultView[0]["loai_tg"] = cbMa_nt.RowResult["loai_tg"];
                Loai_tg.Text = cbMa_nt.RowResult["loai_tg"].ToString();

                Voucher_Ma_nt0.Text = PhView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (PhView[0]["ma_nt"].ToString().Equals(M_Ma_nt0));
                SetStatusVisibleField();
                if ((cbMa_nt.RowResult)["ma_nt"].ToString().Trim().Equals(M_Ma_nt0))
                {
                    txtTy_gia.Value = 1;
                }
                else
                {
                    txtTy_gia.Value = StartUp.GetRates((cbMa_nt.RowResult)["ma_nt"].ToString().Trim(), Convert.ToDateTime(txtNgay_ct.Value).Date);
                }
                if (Ma_gd == "1")
                {
                    foreach (DataRecord rec in Grdhd.Records)
                    {
                        if (rec.Cells["ma_nt_i"].Value.ToString() == Voucher_Ma_nt0.Text)
                            rec.Cells["tt_qd"].Value = rec.Cells["tien_nt"].Value;
                    }
                }
                
                CalculateTyGia();
            }
        }

        void SetStatusVisibleField()
        {
            ChangeLanguage();
            if (Ma_GD_Value.Text.Equals("2"))
            {
                if (PhView[0]["ma_nt"].ToString().Trim().Equals(M_Ma_nt0))
                {
                    GrdCtChi.FieldLayouts[0].Fields["tien"].Visibility = Visibility.Hidden;
                    GrdCtChi.FieldLayouts[0].Fields["ty_giahtf2"].Visibility = Visibility.Hidden;

                    GrdCtChi.FieldLayouts[0].Fields["tien"].Settings.CellMaxWidth = 0;
                    GrdCtChi.FieldLayouts[0].Fields["ty_giahtf2"].Settings.CellMaxWidth = 0;

                    GrdCtChi.FieldLayouts[0].Fields["tien_tt"].Visibility = Visibility.Hidden;
                    GrdCtChi.FieldLayouts[0].Fields["tien_tt"].Settings.CellMaxWidth = 0;
                }
                else
                {
                    GrdCtChi.FieldLayouts[0].Fields["tien"].Visibility = Visibility.Visible;
                    GrdCtChi.FieldLayouts[0].Fields["tien"].Settings.CellMaxWidth = GrdCtChi.FieldLayouts[0].Fields["tien"].Width.Value.Value;

                    GrdCtChi.FieldLayouts[0].Fields["ty_giahtf2"].Visibility = Visibility.Visible;
                    GrdCtChi.FieldLayouts[0].Fields["ty_giahtf2"].Settings.CellMaxWidth = GrdCtChi.FieldLayouts[0].Fields["ty_giahtf2"].Width.Value.Value;

                    GrdCtChi.FieldLayouts[0].Fields["tien_tt"].Visibility = Visibility.Visible;
                    GrdCtChi.FieldLayouts[0].Fields["tien_tt"].Settings.CellMaxWidth = GrdCtChi.FieldLayouts[0].Fields["tien_tt"].Width.Value.Value;
                }
            }
            else
            {
                if (Ma_GD_Value.Text.Equals("3"))
                {

                    GrdCtChi.FieldLayouts[0].Fields["ty_giahtf2"].Visibility = Visibility.Hidden;
                    GrdCtChi.FieldLayouts[0].Fields["ty_giahtf2"].Settings.CellMaxWidth = 0;

                    GrdCtChi.FieldLayouts[0].Fields["tien_tt"].Visibility = Visibility.Hidden;
                    GrdCtChi.FieldLayouts[0].Fields["tien_tt"].Settings.CellMaxWidth = 0;

                    if (PhView[0]["ma_nt"].ToString().Trim().Equals(M_Ma_nt0))
                    {
                        GrdCtChi.FieldLayouts[0].Fields["tien"].Visibility = Visibility.Hidden;
                        GrdCtChi.FieldLayouts[0].Fields["tien"].Settings.CellMaxWidth = 0;
                    }
                    else
                    {
                        GrdCtChi.FieldLayouts[0].Fields["tien"].Visibility = Visibility.Visible;
                        GrdCtChi.FieldLayouts[0].Fields["tien"].Settings.CellMaxWidth = GrdCtChi.FieldLayouts[0].Fields["tien"].Width.Value.Value;
                    }
                }
                else if (Ma_GD_Value.Text.Equals("9"))
                {

                    GrdCtChi.FieldLayouts[0].Fields["ma_kh_i"].Visibility = Visibility.Hidden;
                    GrdCtChi.FieldLayouts[0].Fields["ten_kh_i"].Visibility = Visibility.Hidden;

                    GrdCtChi.FieldLayouts[0].Fields["ma_kh_i"].Settings.CellMaxWidth = 0;
                    GrdCtChi.FieldLayouts[0].Fields["ten_kh_i"].Settings.CellMaxWidth = 0;

                    GrdCtChi.FieldLayouts[0].Fields["ty_giahtf2"].Visibility = Visibility.Hidden;
                    GrdCtChi.FieldLayouts[0].Fields["ty_giahtf2"].Settings.CellMaxWidth = 0;

                    GrdCtChi.FieldLayouts[0].Fields["tien_tt"].Visibility = Visibility.Hidden;
                    GrdCtChi.FieldLayouts[0].Fields["tien_tt"].Settings.CellMaxWidth = 0;

                    if (PhView[0]["ma_nt"].ToString().Trim().Equals(M_Ma_nt0))
                    {
                        GrdCtChi.FieldLayouts[0].Fields["tien"].Visibility = Visibility.Hidden;
                        GrdCtChi.FieldLayouts[0].Fields["tien"].Settings.CellMaxWidth = 0;
                    }
                    else
                    {
                        GrdCtChi.FieldLayouts[0].Fields["tien"].Visibility = Visibility.Visible;
                        GrdCtChi.FieldLayouts[0].Fields["tien"].Settings.CellMaxWidth = GrdCtChi.FieldLayouts[0].Fields["tien"].Width.Value.Value;
                    }
                }
                else if (Ma_GD_Value.Text.Equals("8"))
                {
                    if (StartUp.M_LAN.Equals("V"))
                    {
                        GrdCt.FieldLayouts[0].Fields["ten_tk"].Visibility = Visibility.Visible;
                        GrdCt.FieldLayouts[0].Fields["ten_tk"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["ten_tk"].Width.Value.Value;

                        GrdCt.FieldLayouts[0].Fields["ten_tk2"].Visibility = Visibility.Hidden;
                        GrdCt.FieldLayouts[0].Fields["ten_tk2"].Settings.CellMaxWidth = 0;
                    }
                    else
                    {
                        GrdCt.FieldLayouts[0].Fields["ten_tk"].Visibility = Visibility.Hidden;
                        GrdCt.FieldLayouts[0].Fields["ten_tk"].Settings.CellMaxWidth = 0;

                        GrdCt.FieldLayouts[0].Fields["ten_tk2"].Visibility = Visibility.Visible;
                        GrdCt.FieldLayouts[0].Fields["ten_tk2"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["ten_tk2"].Width.Value.Value;
                    }

                    //Không xài collapse vì đụng CellContainerGenerationMode PreLoad
                    if (PhView[0]["ma_nt"].ToString().Trim().Equals(M_Ma_nt0))
                    {
                        GrdCt.FieldLayouts[0].Fields["tien"].Visibility = Visibility.Hidden;
                        GrdCt.FieldLayouts[0].Fields["thue"].Visibility = Visibility.Hidden;
                        GrdCt.FieldLayouts[0].Fields["tt"].Visibility = Visibility.Hidden;

                        GrdCtgt.FieldLayouts[0].Fields["t_tien"].Visibility = Visibility.Hidden;
                        GrdCtgt.FieldLayouts[0].Fields["t_thue"].Visibility = Visibility.Hidden;
                        GrdCtgt.FieldLayouts[0].Fields["t_tt"].Visibility = Visibility.Hidden;

                        GrdCt.FieldLayouts[0].Fields["tien"].Settings.CellMaxWidth = 0;
                        GrdCt.FieldLayouts[0].Fields["thue"].Settings.CellMaxWidth = 0;
                        GrdCt.FieldLayouts[0].Fields["tt"].Settings.CellMaxWidth = 0;

                        GrdCtgt.FieldLayouts[0].Fields["t_tien"].Settings.CellMaxWidth = 0;
                        GrdCtgt.FieldLayouts[0].Fields["t_thue"].Settings.CellMaxWidth = 0;
                        GrdCtgt.FieldLayouts[0].Fields["t_tt"].Settings.CellMaxWidth = 0;
                    }
                    else
                    {
                        GrdCt.FieldLayouts[0].Fields["tien"].Visibility = Visibility.Visible;
                        GrdCt.FieldLayouts[0].Fields["thue"].Visibility = Visibility.Visible;
                        GrdCt.FieldLayouts[0].Fields["tt"].Visibility = Visibility.Visible;

                        GrdCtgt.FieldLayouts[0].Fields["t_tien"].Visibility = Visibility.Visible;
                        GrdCtgt.FieldLayouts[0].Fields["t_thue"].Visibility = Visibility.Visible;
                        GrdCtgt.FieldLayouts[0].Fields["t_tt"].Visibility = Visibility.Visible;

                        GrdCt.FieldLayouts[0].Fields["tien"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["tien"].Width.Value.Value;
                        GrdCt.FieldLayouts[0].Fields["thue"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["thue"].Width.Value.Value;
                        GrdCt.FieldLayouts[0].Fields["tt"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["tt"].Width.Value.Value;

                        GrdCtgt.FieldLayouts[0].Fields["t_tien"].Settings.CellMaxWidth = GrdCtgt.FieldLayouts[0].Fields["t_tien"].Width.Value.Value;
                        GrdCtgt.FieldLayouts[0].Fields["t_thue"].Settings.CellMaxWidth = GrdCtgt.FieldLayouts[0].Fields["t_thue"].Width.Value.Value;
                        GrdCtgt.FieldLayouts[0].Fields["t_tt"].Settings.CellMaxWidth = GrdCtgt.FieldLayouts[0].Fields["t_tt"].Width.Value.Value;
                    }
                }
                else
                {
                    if (PhView[0]["ma_nt"].ToString().Trim().Equals(M_Ma_nt0))
                    {
                        GrdCtChi.FieldLayouts[0].Fields["tien"].Visibility = Visibility.Hidden;
                        GrdCtChi.FieldLayouts[0].Fields["tien"].Settings.CellMaxWidth = 0;
                    }
                    else
                    {
                        GrdCtChi.FieldLayouts[0].Fields["tien"].Visibility = Visibility.Visible;
                        GrdCtChi.FieldLayouts[0].Fields["tien"].Settings.CellMaxWidth = GrdCtChi.FieldLayouts[0].Fields["tien"].Width.Value.Value;
                    }
                }

                GrdCtChi.FieldLayouts[0].Fields["ty_giahtf2"].Visibility = Visibility.Hidden;
                GrdCtChi.FieldLayouts[0].Fields["ty_giahtf2"].Settings.CellMaxWidth = 0;
                GrdCtChi.FieldLayouts[0].Fields["tien_tt"].Visibility = Visibility.Hidden;
                GrdCtChi.FieldLayouts[0].Fields["tien_tt"].Settings.CellMaxWidth = 0;

            }
        }

        private bool txtDiaChiFocusable = true;
        private void txtMa_kh_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (IsInEditMode.Value == true)
            {
                if (txtMa_kh.RowResult == null || string.IsNullOrEmpty(txtMa_kh.Text.Trim()))
                    return;
                if (M_LAN.ToUpper().Equals("V"))
                    txtTen_kh.Text = txtMa_kh.RowResult["ten_kh"].ToString();
                else
                    txtTen_kh.Text = txtMa_kh.RowResult["ten_kh2"].ToString();

                if (!string.IsNullOrEmpty(txtMa_kh.RowResult["doi_tac"].ToString().Trim()))
                    txtOng_ba.Text = txtMa_kh.RowResult["doi_tac"].ToString();

                txtMaSoThue.Text = txtMa_kh.RowResult["ma_so_thue"].ToString();
                if (!string.IsNullOrEmpty(txtMa_kh.RowResult["dia_chi"].ToString()))
                {
                  //  if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[0]["tk_i"].ToString().Trim()))
                        txtDia_chi.Text = txtMa_kh.RowResult["dia_chi"].ToString();
                }
                PhView[0]["tk_nh"] = txtMa_kh.RowResult["tk_nh"];
                PhView[0]["ten_nh"] = txtMa_kh.RowResult["ten_nh"];
                PhView[0]["tinh_thanh"] = txtMa_kh.RowResult["tinh_thanh"];

                //txtMa_nx.Text = string.IsNullOrEmpty(txtMa_nx.Text.Trim()) ? txtMa_kh.RowResult["tk"].ToString().Trim() : txtMa_nx.Text.Trim();

                if (CtView.Count == 1)
                {
                    if (CtView[0]["tk_i"] == DBNull.Value || string.IsNullOrEmpty(CtView[0]["tk_i"].ToString()))
                    {
                        CtView[0]["tk_i"] = txtMa_kh.RowResult["tk"].ToString();
                        DataSet ds = StartUp.SysObj.ExcuteReader(new SqlCommand(string.Format("SELECT ten_tk, ten_tk2 FROM dmtk WHERE tk = '{0}'", txtMa_kh.RowResult["tk"].ToString().Trim())));
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            if (StartUp.DsTrans.Tables[1].Columns.Contains("ten_tk"))
                                StartUp.DsTrans.Tables[1].DefaultView[0]["ten_tk"] = ds.Tables[0].Rows[0]["ten_tk"];
                            if (StartUp.DsTrans.Tables[1].Columns.Contains("ten_tk2"))
                                StartUp.DsTrans.Tables[1].DefaultView[0]["ten_tk2"] = ds.Tables[0].Rows[0]["ten_tk2"];
                        }
                    }

                }
                if (txtMa_kh.RowResult["dia_chi"].ToString().Trim().Equals(""))
                {
                    txtDiaChiFocusable = true;
                }
                else
                {
                    txtDiaChiFocusable = false;
                }
                //LoadDataDu13();
            }
        }

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

        private void txtMa_nx_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMa_nx.Text.Trim()) && !txtMa_nx.IsReadOnly)
            {
                if (M_LAN.ToUpper().Equals("V"))
                    txtTenTK.Text = txtMa_nx.RowResult["ten_nx"].ToString();
                else
                    txtTenTK.Text = txtMa_nx.RowResult["ten_nx2"].ToString();
                PhView[0]["ten_nx"] = txtMa_nx.RowResult["ten_nx"];
                PhView[0]["ten_nx2"] = txtMa_nx.RowResult["ten_nx2"];
            }
            //LoadDataDu13();
        }

        private void GrdCtChi_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsInEditMode.Value == false)
                return;
            if (Keyboard.IsKeyDown(Key.N) && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                NewRowCtChi();
                GrdCtChi.ActiveRecord = GrdCtChi.Records[GrdCtChi.Records.Count - 1];
            }
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

        private void GrdCtChi_KeyUp(object sender, KeyEventArgs e)
        {
            if (IsInEditMode.Value == false)
                return;

            switch (e.Key)
            {
                case Key.F4:
                    GrdCt.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);
                    GrdCtChi.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);
                    Grdhd.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);
                    GrdCtgt.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);
                    if (Keyboard.FocusedElement.GetType().Name.Equals("TextBoxAutoComplete"))
                    {
                        AutoCompleteTextBox txt = (Keyboard.FocusedElement as TextBoxAutoComplete).ParentControl;
                        if (!txt.CheckLostFocus())
                            return;
                    }
                    NewRowCt();
                    GrdCtChi.ActiveRecord = GrdCtChi.Records[GrdCtChi.Records.Count - 1];
                    GrdCtChi.ActiveCell = (GrdCtChi.ActiveRecord as DataRecord).Cells["tk_i"];
                    break;
                case Key.F5:
                    if (StartUp.SysObj.VersionInfo.Rows[0]["product_code"].ToString().Equals("FA") || (StartUp.dtRegInfo != null && !StartUp.dtRegInfo.Rows[18]["content"].ToString().Trim().Equals("FK")))
                    {
                        CatgLib.Catinhtg.Tinh(GrdCtChi.Records, this);
                    }
                    break;
                case Key.F8:
                    {
                        if (ExMessageBox.Show( 430,StartUp.SysObj, "Có xóa dòng ghi hiện thời không?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                        {
                            return;
                        }

                        DataRecord ARow = (GrdCtChi.ActiveRecord as DataRecord);
                        if (ARow != null)
                        {
                            int indexRecord = 0, indexCell = 0;
                            Cell cell = GrdCtChi.ActiveCell;

                            indexRecord = ARow.Index;
                            if (ARow.Index == 0)
                            {
                                if (GrdCtChi.Records.Count == 1)
                                {
                                    GrdCtChi_AddNewRecord(null, null);
                                    (GrdCtChi.Records[1] as DataRecord).Cells["dien_giaii"].Value = "";
                                }
                            }
                            else if (ARow.Index == GrdCtChi.Records.Count - 1)
                            {
                                indexRecord = ARow.Index - 1;
                            }

                            indexCell = GrdCtChi.ActiveCell == null ? 0 : GrdCtChi.ActiveCell.Field.Index;

                            GrdCtChi.ExecuteCommand(DataPresenterCommands.EndEditModeAndDiscardChanges);
                            if (indexCell >= 0)
                            {
                                CtData.Rows.Remove(CtView[ARow.Index].Row);
                                CtData.AcceptChanges();
                                if (GrdCtChi.Records.Count > 0)
                                    GrdCtChi.ActiveRecord = GrdCtChi.Records[indexRecord > GrdCtChi.Records.Count - 1 ? GrdCtChi.Records.Count - 1 : indexRecord];
                                UpdateTotalHT();
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void GrdCt_KeyUp(object sender, KeyEventArgs e)
        {
            if (IsInEditMode.Value == false)
                return;

            switch (e.Key)
            {
                case Key.F4:
                    GrdCt.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);
                    GrdCtChi.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);
                    Grdhd.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);
                    GrdCtgt.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);
                    if (Keyboard.FocusedElement.GetType().Name.Equals("TextBoxAutoComplete"))
                    {
                        AutoCompleteTextBox txt = (Keyboard.FocusedElement as TextBoxAutoComplete).ParentControl;
                        if (!txt.CheckLostFocus())
                            return;
                    }
                    NewRowCt();
                    GrdCt.ActiveRecord = GrdCt.Records[GrdCt.Records.Count - 1];
                    GrdCt.ActiveCell = (GrdCt.ActiveRecord as DataRecord).Cells["tk_i"];
                    break;
                case Key.F5:
                    if (StartUp.SysObj.VersionInfo.Rows[0]["product_code"].ToString().Equals("FA") || (StartUp.dtRegInfo != null && !StartUp.dtRegInfo.Rows[18]["content"].ToString().Trim().Equals("FK")))
                    {
                        CatgLib.Catinhtg.Tinh(GrdCtChi.Records, this);
                    }
                    break;
                case Key.F8:
                    {
                        if (ExMessageBox.Show( 435,StartUp.SysObj, "Có xóa dòng ghi hiện thời không?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
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
                                CtData.Rows.Remove(CtView[ARow.Index].Row);
                                CtData.AcceptChanges();
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
        }

        private void GrdCtgt_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsInEditMode.Value == false)
                return;
            if (Keyboard.IsKeyDown(Key.N) && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && PhView[0]["ma_gd"].ToString().Equals("8"))
            {
                NewRowCtGt();
                GrdCtgt.ActiveRecord = GrdCtgt.Records[GrdCtgt.Records.Count - 1];
            }
            if (Keyboard.IsKeyDown(Key.Tab) && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                txthan_tt.Focus();
            }
        }

        private void GrdCtgt_KeyUp(object sender, KeyEventArgs e)
        {
            if (IsInEditMode.Value == false || !PhView[0]["ma_gd"].ToString().Equals("8"))
                return;
          

            switch (e.Key)
            {
                case Key.F4:
                    if (Sua_thue != 1)
                        return; 
                    
                        GrdCtgt.ExecuteCommand(DataPresenterCommands.StartEditMode);
                    if (Keyboard.FocusedElement.GetType().Name.Equals("TextBoxAutoComplete"))
                    {
                        AutoCompleteTextBox txt = (Keyboard.FocusedElement as TextBoxAutoComplete).ParentControl;
                        if (!txt.CheckLostFocus())
                            return;
                    }
                    else if (GrdCtgt.ActiveCell != null && GrdCtgt.ActiveCell.Field.GetType().Equals(typeof(NotEmptyField)))
                    {
                        if (GrdCtgt.ActiveCell.Value == DBNull.Value || string.IsNullOrEmpty(GrdCtgt.ActiveCell.Value.ToString().Trim()))
                            return;
                    }
                    NewRowCtGt();
                    GrdCtgt.ActiveRecord = GrdCtgt.Records[GrdCtgt.Records.Count - 1];
                    GrdCtgt.ActiveCell = (GrdCtgt.ActiveRecord as DataRecord).Cells["so_ct0"];
                    break;
                //case Key.F5:
                //    CatgLib.Catinhtg.Tinh(GrdCtChi.Records, this);
                //    break;
                case Key.F8:
                    {
                        if (PhView[0]["sua_thue"].ToString() == "0")
                            return;
                        
                        if (ExMessageBox.Show(440, StartUp.SysObj, "Có xóa dòng ghi hiện thời không?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                        {
                            return;
                        }

                        DataRecord ARow = (GrdCtgt.ActiveRecord as DataRecord);
                        if (ARow != null)
                        {
                            int indexRecord = 0, indexCell = 0;
                            Cell cell = GrdCtgt.ActiveCell;
                            if (ARow.Index == 0)
                            {
                                if (GrdCtgt.Records.Count == 1)
                                    GrdCtgt_AddNewRecord(null, null);
                                indexRecord = ARow.Index;
                            }
                            else if (ARow.Index > 0)
                            {
                                indexRecord = ARow.Index - 1;
                            }
                            indexCell = GrdCtgt.ActiveCell == null ? 0 : GrdCtgt.ActiveCell.Field.Index;
                            if (indexCell >= 0)
                            {
                                GrdCtgt.ExecuteCommand(DataPresenterCommands.EndEditModeAndDiscardChanges);
                                StartUp.DsTrans.Tables[2].Rows.Remove(StartUp.DsTrans.Tables[2].DefaultView[ARow.Index].Row);
                                StartUp.DsTrans.Tables[2].AcceptChanges();
                                if (GrdCtgt.Records.Count > 0)
                                    GrdCtgt.ActiveRecord = GrdCtgt.Records[indexRecord > GrdCtgt.Records.Count - 1 ? GrdCtgt.Records.Count - 1 : indexRecord];
                                //GrdCtgt.DataSource = null;
                                //GrdCtgt.DataSource = StartUp.DsTrans.Tables[2].DefaultView;

                                //GrdCtgt.ActiveCell = (GrdCtgt.Records[indexRecord] as DataRecord).Cells[indexCell];
                                //GrdCtgt.Records[indexRecord].DataPresenter.BringRecordIntoView(GrdCtgt.Records[indexRecord]);
                                //GrdCtgt.ActiveCell = (GrdCtgt.Records[indexRecord] as DataRecord).Cells[indexCell];
                                UpdateTotalThue();
                            }
                        }
                    }
                    break;
                case Key.Enter:
                    {
                        if (GrdCtgt.Records.Count > 0)
                        {
                            Field field = GrdCtgt.Records[0].DataPresenter.ActiveCell.Field;
                            if (GrdCtgt.Records[0].DataPresenter.FieldLayouts[0].Fields.IndexOf(field) == GrdCtgt.FieldLayouts[0].Fields.Count - 1)
                            {
                                GrdCtgt.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                {
                                    GrdCtgt.Records[0].DataPresenter.ActiveCell = (GrdCtgt.Records[0] as DataRecord).Cells[0];
                                }));
                            }
                        }
                        else
                            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                (this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus();
                            }));
                            
                        break;
                    }
                default:
                    break;
            }
        }

        bool NewRowCtGt()
        {
            try
            {
                DataRow NewRecord = StartUp.DsTrans.Tables[2].NewRow();
                NewRecord["stt_rec"] = PhView[0]["stt_rec"];
                int Stt_rec0 = 0, Stt_rec0ct = 0, Stt_rec0ctgt = 0;
                if (GrdCt.Records.Count > 0)
                {
                    var _max_sttrec0ct = CtData.AsEnumerable()
                                       .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                                       .Max(x => x.Field<string>("stt_rec0"));
                    if (_max_sttrec0ct != null)
                        int.TryParse(_max_sttrec0ct.ToString(), out Stt_rec0ct);
                }
                if (GrdCtgt.Records.Count > 0)
                {
                    var _max_sttrec0ctgt = StartUp.DsTrans.Tables[2].AsEnumerable()
                                   .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                                   .Max(x => x.Field<string>("stt_rec0"));
                    if (_max_sttrec0ctgt != null)
                        int.TryParse(_max_sttrec0ctgt.ToString(), out Stt_rec0ctgt);
                }
                Stt_rec0 = Stt_rec0ct >= Stt_rec0ctgt ? Stt_rec0ct : Stt_rec0ctgt;
                Stt_rec0++;

                NewRecord["stt_rec0"] = string.Format("{0:000}", Stt_rec0);
                NewRecord["ma_ct"] = StartUp.Ma_ct;
                NewRecord["ngay_ct"] = txtNgay_ct.Value;
                NewRecord["ten_vt"] = txtDien_giai.Text;
                NewRecord["t_tien_nt"] = 0;
                NewRecord["t_tien"] = 0;
                NewRecord["thue_suat"] = 0;
                NewRecord["t_thue_nt"] = 0;
                NewRecord["t_thue"] = 0;

                if (GrdCtgt.Records.Count > 0 && GrdCtgt.ActiveRecord != null)
                {
                    DataRecord rec = GrdCtgt.ActiveRecord as DataRecord;

                    NewRecord["ma_ms"] = rec.Cells["ma_ms"].Value;
                }
                else
                    NewRecord["ma_ms"] = StartUp.SysObj.GetOption("M_MA_MS");

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

        private void UpdateTotalHT()
        {
            try
            {
                //if (currActionTask == ActionTask.View)
                //    return;

                CtData.AcceptChanges();
                //Cập nhật tổng thanh toán nguyên tệ
                Decimal _t_tien = 0, _t_thue = 0;

                var vtien = CtData.AsEnumerable()
                    .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                    .Sum(x => x.Field<decimal?>("tien_nt"));
                if (vtien != null)
                    Decimal.TryParse(vtien.ToString(), out _t_tien);
                txtT_Tien.Value = _t_tien;
                txtT_Tien_nt.Value = _t_tien;

                //Tính thuế
                Decimal.TryParse(CtData.Compute("sum(thue_nt)", "stt_rec= '" + PhView[0]["stt_rec"].ToString() + "'").ToString(), out _t_thue);

                txtT_thue.Value = _t_thue;
                txtT_thue_nt.Value = _t_thue;
                //Tính tổng thanh toán
                txtT_tt.Value = _t_tien + _t_thue;
                txtT_tt_nt.Value = _t_tien + _t_thue;
                //Cập nhật tổng thanh toán cho tien0
                if (!cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
                {
                    //Decimal t_tien_nt0 = 0, t_thue_nt0 = 0, t_t_nt0 = 0;
                    //Decimal _ty_gia = txtTy_gia.Value == DBNull.Value ? 1 : Convert.ToDecimal(txtTy_gia.nValue);

                    //var v_drHd2 = CtData.AsEnumerable()
                    //    .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString() && b.Field<string>("loai_hd") == "2")
                    //    .Select(x => x);
                    //DataRow[] drHd2 = v_drHd2.ToArray();

                    //var v_drHd = CtData.AsEnumerable()
                    //                        .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString() && b.Field<string>("loai_hd") != "2")
                    //                        .Select(x => x);
                    //DataRow[] drHd = v_drHd.ToArray();

                    //if (drHd2.Count() > 0)
                    //{
                    //    //Tính tiền hàng
                    //    Decimal _sum_tien_nt0 = 0;
                    //    var vtien_nt0 = drHd2.AsEnumerable()
                    //        .Sum(x => x.Field<decimal?>("tien"));
                    //    if (vtien_nt0 != null)
                    //        Decimal.TryParse(vtien_nt0.ToString(), out _sum_tien_nt0);
                    //    t_tien_nt0 += _sum_tien_nt0;

                    //    //Tính tiền thuế
                    //    Decimal _sum_thue_nt0 = 0;
                    //    var vthue_nt0 = drHd2.AsEnumerable()
                    //        .Sum(x => x.Field<decimal?>("thue"));
                    //    if (vthue_nt0 != null)
                    //        Decimal.TryParse(vthue_nt0.ToString(), out _sum_thue_nt0);
                    //    t_thue_nt0 += _sum_thue_nt0;

                    //    //Tính tổng thanh toán
                    //    t_t_nt0 += _sum_tien_nt0 + _sum_thue_nt0;
                    //}

                    //if (drHd.Count() > 0)
                    //{
                    //    Decimal _so_phieu_sai = 0;
                    //    var v_so_phieu_sai = CtData.AsEnumerable()
                    //       .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString() && (b.Field<decimal?>("tien_nt") == 0||_ty_gia==0) && b.Field<decimal?>("tien") != 0)
                    //       .Count();
                    //    if (v_so_phieu_sai != null)
                    //        Decimal.TryParse(v_so_phieu_sai.ToString(), out _so_phieu_sai);
                    //    if (_so_phieu_sai == 0 && !ChkSuaTien.IsChecked.Value)
                    //    {
                    //        Decimal t_tien_nt = 0;
                    //        //Tính tiền hàng
                    //        var vtien_nt = drHd.AsEnumerable()
                    //                        .Sum(x => x.Field<decimal?>("tien_nt"));
                    //        if (vtien_nt != null)
                    //            Decimal.TryParse(vtien_nt.ToString(), out t_tien_nt);
                    //        Decimal _sum_tien = SmLib.SysFunc.Round(_ty_gia * t_tien_nt, M_Round);
                    //        t_tien_nt0 += _sum_tien;

                    //        ////Gán số dư cho phiếu đầu tiên
                    //        if (GrdCt.Records.Count > 0 && _ty_gia != 0)
                    //        {
                    //            Decimal _sum_tien_nt0 = 0;
                    //            var vtien_nt0 = drHd.AsEnumerable()
                    //                .Sum(x => x.Field<decimal?>("tien"));
                    //            if (vtien_nt0 != null)
                    //                Decimal.TryParse(vtien_nt0.ToString(), out _sum_tien_nt0);

                    //            int indexFirstRow = 0;
                    //            bool fcont = true;
                    //            while (indexFirstRow < GrdCt.Records.Count && fcont)
                    //            {
                    //                if (!(GrdCt.Records[indexFirstRow] as DataRecord).Cells["loai_hd"].Value.Equals("2"))
                    //                {
                    //                    fcont = false;
                    //                }
                    //                else
                    //                {
                    //                    indexFirstRow++;
                    //                }
                    //            }
                    //            //indexFirstRow = CtData.Rows.IndexOf(drHd[0]);
                    //            //CtData.Rows[indexFirstRow]["tien"] = Convert.ToDecimal(CtData.Rows[indexFirstRow]["tien"]) + (_sum_tien - _sum_tien_nt0);
                    //            if (!fcont)
                    //                (GrdCt.Records[indexFirstRow] as DataRecord).Cells["tien"].Value = Convert.ToDecimal((GrdCt.Records[indexFirstRow] as DataRecord).Cells["tien"].Value) + (_sum_tien - _sum_tien_nt0);
                    //        }
                    //        //Tính tiền thuế
                    //        Decimal _sum_thue_nt0 = 0;
                    //        var vthue_nt0 = drHd.AsEnumerable()
                    //            .Sum(x => x.Field<decimal?>("thue"));
                    //        if (vthue_nt0 != null)
                    //            Decimal.TryParse(vthue_nt0.ToString(), out _sum_thue_nt0);
                    //        t_thue_nt0 += _sum_thue_nt0;

                    //        //Tính tổng thanh toán
                    //        t_t_nt0 += _sum_tien + _sum_thue_nt0;
                    //    }
                    //    else
                    //    {
                    //        Decimal _sum_tien_nt0 = 0;
                    //        var vtien_nt0 = drHd.AsEnumerable()
                    //                        .Sum(x => x.Field<decimal?>("tien"));
                    //        if (vtien_nt0 != null)
                    //            Decimal.TryParse(vtien_nt0.ToString(), out _sum_tien_nt0);
                    //        t_tien_nt0 += _sum_tien_nt0;
                    //        //Tính tiền thuế
                    //        Decimal _sum_thue_nt0 = 0;
                    //        var vthue_nt0 = drHd.AsEnumerable()
                    //            .Sum(x => x.Field<decimal?>("thue"));
                    //        if (vthue_nt0 != null)
                    //            Decimal.TryParse(vthue_nt0.ToString(), out _sum_thue_nt0);
                    //        t_thue_nt0 += _sum_thue_nt0;

                    //        //Tính tổng thanh toán
                    //        t_t_nt0 += _sum_tien_nt0 + _sum_thue_nt0;
                    //    }
                    //}
                    Decimal t_tien_nt0 = 0, t_thue_nt0 = 0, t_t_nt0 = 0;

                    var vphieu = CtData.AsEnumerable()
                       .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                       .Select(p => new { tien = p.Field<decimal?>("tien"), thue = p.Field<decimal?>("thue"), tt = p.Field<decimal?>("tt") });
                    if (Ma_GD_Value.Text.Equals("2"))
                    {
                        vphieu = CtData.AsEnumerable()
                       .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                       .Select(p => new { tien = p.Field<decimal?>("tien_tt"), thue = p.Field<decimal?>("thue"), tt = p.Field<decimal?>("tien_tt") });
                    }
                    if (vphieu != null)
                    {
                        var vtien_nt0 = vphieu.Sum(p => p.tien);
                        var vthue_nt0 = vphieu.Sum(p => p.thue);
                        var vtt_nt0 = vphieu.Sum(p => p.tt);
                        if (vtien_nt0 != null)
                            Decimal.TryParse(vtien_nt0.ToString(), out t_tien_nt0);
                        if (vthue_nt0 != null)
                            Decimal.TryParse(vthue_nt0.ToString(), out t_thue_nt0);
                        if (vtt_nt0 != null)
                            Decimal.TryParse(vtt_nt0.ToString(), out t_t_nt0);
                    }

                    txtT_Tien_Nt0.Value = t_tien_nt0;
                    txtT_thue_Nt0.Value = t_thue_nt0;
                    //txtT_tt_Nt0.Value = t_t_nt0;
                    txtT_tt_Nt0.Value = t_tien_nt0 + t_thue_nt0;
                }
                else
                {
                    txtT_Tien_Nt0.Value = _t_tien;
                    txtT_thue_Nt0.Value = _t_thue;
                    txtT_tt_Nt0.Value = _t_tien + _t_thue;
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        private void UpdateTotalThue()
        {
            try
            {
                if (!Ma_GD_Value.Text.Equals("8"))
                {
                    txtT_t_Tien.Value = txtT_Tien.Value;
                    txtT_t_Tien_nt.Value = txtT_Tien_nt.Value;
                    txtT_t_thue.Value = txtT_thue.Value;
                    txtT_t_thue_nt.Value = txtT_thue_nt.Value;
                    //Tính tổng thanh toán
                    txtT_t_tt.Value = txtT_tt.Value;
                    txtT_t_tt_nt.Value = txtT_tt_nt.Value;

                    txtT_t_Tien_Nt0.Value = txtT_Tien_Nt0.Value;
                    txtT_t_thue_Nt0.Value = txtT_thue_Nt0.Value;
                    txtT_t_tt_Nt0.Value = txtT_tt_Nt0.Value;
                    return;
                }
                //if (currActionTask == ActionTask.View)
                //    return;
                StartUp.DsTrans.Tables[2].AcceptChanges();
                //Cập nhật tổng thanh toán nguyên tệ
                Decimal _t_tien = 0, _t_thue = 0;

                //Tính tiền
                var vtien = StartUp.DsTrans.Tables[2].AsEnumerable()
                    .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                    .Sum(x => x.Field<decimal?>("t_tien_nt"));
                if (vtien != null)
                    Decimal.TryParse(vtien.ToString(), out _t_tien);

                txtT_t_Tien.Value = _t_tien;
                txtT_t_Tien_nt.Value = _t_tien;

                //Tính thuế
                var vthue = StartUp.DsTrans.Tables[2].AsEnumerable()
                                .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                                .Sum(x => x.Field<decimal?>("t_thue_nt"));
                if (vthue != null)
                    Decimal.TryParse(vthue.ToString(), out _t_thue);

                txtT_t_thue.Value = _t_thue;
                txtT_t_thue_nt.Value = _t_thue;
                //Tính tổng thanh toán
                txtT_t_tt.Value = _t_tien + _t_thue;
                txtT_t_tt_nt.Value = _t_tien + _t_thue;
                //Cập nhật tổng thanh toán cho tien0
                if (!cbMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
                {
                    //tiền nt0
                    Decimal _sum_tien_nt0 = 0;
                    var vtien_nt0 = StartUp.DsTrans.Tables[2].AsEnumerable()
                        .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                        .Sum(x => x.Field<decimal?>("t_tien"));
                    if (vtien_nt0 != null)
                        Decimal.TryParse(vtien_nt0.ToString(), out _sum_tien_nt0);
                    txtT_t_Tien_Nt0.Value = _sum_tien_nt0;


                    //thuế nt0
                    Decimal _sum_thue_nt0 = 0;
                    var vthue_nt0 = StartUp.DsTrans.Tables[2].AsEnumerable()
                                .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                                .Sum(x => x.Field<decimal?>("t_thue"));
                    if (vthue_nt0 != null)
                        Decimal.TryParse(vthue_nt0.ToString(), out _sum_thue_nt0);

                    txtT_t_thue_Nt0.Value = _sum_thue_nt0;
                    //Tính tổng thanh toán

                    txtT_t_tt_Nt0.Value = _sum_tien_nt0 + _sum_thue_nt0;

                }
                else
                {
                    txtT_t_Tien_Nt0.Value = _t_tien;
                    txtT_t_thue_Nt0.Value = _t_thue;
                    txtT_t_tt_Nt0.Value = _t_tien + _t_thue;
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        private void txtTy_gia_ht_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtTy_gia_ht.Value == DBNull.Value)
            {
                txtTy_gia_ht.Value = 0;
            }
        }

        private void txtTy_gia_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtTy_gia.Value == DBNull.Value)
            {
                txtTy_gia.Value = 0;
            }
            try
            {
                if (currActionTask == ActionTask.View)
                    return;
                if (txtTy_gia.OldValue != txtTy_gia.nValue)
                {
                    CalculateTyGia();
                    Ty_Gia_ValueChange.Value = !Ty_Gia_ValueChange.Value;
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }


        private void TinhLai_tien_tt()
        {

            if (PhView[0]["ma_nt"].ToString().Trim().Equals(StartUp.M_ma_nt0.Trim()))
            {
                for (int i = 0; i < GrdCtChi.Records.Count; i++)
                {
                    DataRecord rec = GrdCtChi.Records[i] as DataRecord;
                    rec.Cells["tien_tt"].Value = rec.Cells["tien"].Value;
                }
                return;
            } 
            
            Decimal _ty_gia_ht = 0;
            _ty_gia_ht = ArapLib.FNum.ToDec(txtTy_gia_ht.nValue);

            decimal _Ty_gia = ArapLib.FNum.ToDec(txtTy_gia.Value);

            if (StartUp.M_Gd_2Tg_List.Contains(Ma_gd))
            {
                for (int i = 0; i < GrdCtChi.Records.Count; i++)
                {
                    DataRecord rec = GrdCtChi.Records[i] as DataRecord;
                    rec.Cells["ty_giahtf2"].Value = txtTy_gia.Value;
                    (rec.DataItem as DataRowView)["ty_gia_ht2"] = (mLoai_tg == 1 ? _Ty_gia : (_Ty_gia == 0 ? 0 : SmLib.SysFunc.Round(1 / _Ty_gia, M_ROUND_TY_GIA))); ;

                    Decimal _tien_nt = 0, _tien_tt = 0;
                    _tien_nt = Convert.ToDecimal(rec.Cells["tien_nt"].Value);
                    _tien_tt = SmLib.SysFunc.Round(_ty_gia_ht * _tien_nt, StartUp.M_ROUND);
                    rec.Cells["tien_tt"].Value = _tien_tt;
                }
            }

        }
        private void CalculateTyGia()
        {
            //if (cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
            //    return;

            Decimal _ty_gia = txtTy_gia.nValue;
            Decimal _ty_gia_ht = txtTy_gia_ht.nValue;
            //if (_ty_gia_ht == 0 && "4,5,6,7,8,9".Contains(txtMa_gd.Text.Trim()))
            //    txtTy_gia_ht.Value = _ty_gia_ht = _ty_gia;

            if (cbMa_nt.Text.Trim().Equals(M_Ma_nt0))
            {
                txtTy_gia_ht.Value = txtTy_gia.Value;
            }

            if (Sua_tien == 1)
            {
                if (Ma_GD_Value.Text.Trim().ToString().IndexOfAny(new char[] { '2', '3', '4', '5', '6', '7','8','9' }) >= 0)
                {
                    if (PhView[0]["ma_nt"].ToString().ToUpper().Trim().Equals(M_Ma_nt0.ToUpper().Trim()))
                    {
                        for (int i = 0; i < GrdCtChi.Records.Count; i++)
                        {
                            (GrdCtChi.Records[i] as DataRecord).Cells["ty_giahtf2"].Value = txtTy_gia.Value;
                            ((GrdCt.Records[i] as DataRecord).DataItem as DataRowView)["ty_gia_ht2"] = (mLoai_tg == 1 ? _ty_gia : (_ty_gia == 0 ? 0 : SmLib.SysFunc.Round(1 / _ty_gia, M_ROUND_TY_GIA))); ; ;
                            ((GrdCt.Records[i] as DataRecord).DataItem as DataRowView)["tien_tt"] =
                            ((GrdCt.Records[i] as DataRecord).DataItem as DataRowView)["tien"] =
                            ((GrdCt.Records[i] as DataRecord).DataItem as DataRowView)["tien_nt"];
                            CalculateHd2(GrdCt.Records[i] as DataRecord);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < GrdCtChi.Records.Count; i++)
                        {
                            (GrdCtChi.Records[i] as DataRecord).Cells["ty_giahtf2"].Value = txtTy_gia.Value;
                            ((GrdCt.Records[i] as DataRecord).DataItem as DataRowView)["ty_gia_ht2"] = (mLoai_tg == 1 ? _ty_gia : (_ty_gia == 0 ? 0 : SmLib.SysFunc.Round(1 / _ty_gia, M_ROUND_TY_GIA))); ; ;
                            ((GrdCt.Records[i] as DataRecord).DataItem as DataRowView)["tien_tt"] =
                            ((GrdCt.Records[i] as DataRecord).DataItem as DataRowView)["tien"] =
                            SmLib.SysFunc.Round(Convert.ToDecimal(((GrdCt.Records[i] as DataRecord).DataItem as DataRowView)["tien_nt"]) * System.Convert.ToDecimal(txtTy_gia.Value), M_Round);
                            CalculateHd2(GrdCt.Records[i] as DataRecord);
                        }
                    }
                }

                UpdateTotalHT();
                return;
            }
            //if (txtTy_gia.nValue != 0)
            {
                //Hạch toán
                if (Ma_GD_Value.Text.Trim().ToString().IndexOfAny(new char[]{'2','3', '4', '5', '6', '7'}) >= 0)
                {
                    for (int i = 0; i < GrdCtChi.Records.Count; i++)
                    {
                        Decimal _tien_nt = 0, _tien_nt0 = 0;
                        _tien_nt = Convert.ToDecimal((GrdCtChi.Records[i] as DataRecord).Cells["tien_nt"].Value);
                        //if (_ty_gia * _tien_nt > 0)
                        {
                            _tien_nt0 = SmLib.SysFunc.Round(_ty_gia * _tien_nt, M_Round);
                            if (!PhView[0]["ma_nt"].ToString().ToUpper().Trim().Equals(M_Ma_nt0.ToUpper().Trim()))
                            {
                                if (PhView[0]["ma_gd"].ToString().IndexOfAny(new char[] { '2' }) >= 0)
                                {
  
                                    decimal _ty_gia_ct = 0;
                                    decimal.TryParse(((GrdCtChi.Records[i] as DataRecord).DataItem as DataRowView)["ty_giahtf2"].ToString(), out _ty_gia_ct);

                                    ((GrdCt.Records[i] as DataRecord).DataItem as DataRowView)["ty_gia_ht2"] = (mLoai_tg == 1 ? _ty_gia_ct : (_ty_gia_ct == 0 ? 0 : SmLib.SysFunc.Round(1 / _ty_gia_ct, M_ROUND_TY_GIA)));

                                    (GrdCtChi.Records[i] as DataRecord).Cells["tien_tt"].Value = _tien_nt0;
                                    //if (_ty_gia_ct * _tien_nt > 0)
                                    {
                                        (GrdCtChi.Records[i] as DataRecord).Cells["tien"].Value = SmLib.SysFunc.Round(_ty_gia_ct * _tien_nt, M_Round);
                                    }

                                }
                                else
                                {
                                    ((GrdCt.Records[i] as DataRecord).DataItem as DataRowView)["ty_giahtf2"] = txtTy_gia.Value;
                                    ((GrdCt.Records[i] as DataRecord).DataItem as DataRowView)["ty_gia_ht2"] = (mLoai_tg == 1 ? _ty_gia : (_ty_gia == 0 ? 0 : SmLib.SysFunc.Round(1 / _ty_gia, M_ROUND_TY_GIA)));
                                    
                                    (GrdCtChi.Records[i] as DataRecord).Cells["tien"].Value = _tien_nt0;
                                }
                            }
                            else
                            {
                                (GrdCtChi.Records[i] as DataRecord).Cells["ty_giahtf2"].Value = txtTy_gia.Value;
                                ((GrdCtChi.Records[i] as DataRecord).DataItem as DataRowView)["ty_gia_ht2"] = (mLoai_tg == 1 ? _ty_gia : (_ty_gia == 0 ? 0 : SmLib.SysFunc.Round(1 / _ty_gia, M_ROUND_TY_GIA))); ; ;
                             
                                (GrdCtChi.Records[i] as DataRecord).Cells["tien_tt"].Value = _tien_nt0;
                                (GrdCtChi.Records[i] as DataRecord).Cells["tien"].Value = _tien_nt0;
                            }
                        }
                    }
                }
                else if (Ma_GD_Value.Text.Equals("9"))
                {
                    for (int i = 0; i < GrdCtChi.Records.Count; i++)
                    {
                        Decimal _tien_nt = 0, _tien_nt0 = 0;
                        _tien_nt = Convert.ToDecimal((GrdCtChi.Records[i] as DataRecord).Cells["tien_nt"].Value);
                        _tien_nt0 = SmLib.SysFunc.Round(_ty_gia * _tien_nt, M_Round);
                        (GrdCtChi.Records[i] as DataRecord).Cells["tien"].Value = _tien_nt0;
                        (GrdCtChi.Records[i] as DataRecord).Cells["tien_tt"].Value = _tien_nt0;
                        (GrdCtChi.Records[i] as DataRecord).Cells["tt"].Value = _tien_nt0;
                    }
                }
                else if (Ma_GD_Value.Text.Equals("8"))
                {
                    for (int i = 0; i < GrdCt.Records.Count; i++)
                    {
                        if ((GrdCt.Records[i] as DataRecord).Cells["loai_hd"].Value.ToString().Trim().Equals("2"))
                            CalculateHd2(GrdCt.Records[i] as DataRecord);
                        else
                        {
                            Decimal _tien_nt = 0, _tien_nt0 = 0, thue_nt = 0, thue_nt0 = 0;
                            _tien_nt = Convert.ToDecimal((GrdCt.Records[i] as DataRecord).Cells["tien_nt"].Value);
                            if (_ty_gia * _tien_nt > 0)
                            {
                                _tien_nt0 = SmLib.SysFunc.Round(_ty_gia * _tien_nt, M_Round);
                                (GrdCt.Records[i] as DataRecord).Cells["tien"].Value = _tien_nt0;
                                if (ArapLib.FNum.ToDec(GetPhValue("sua_thue")) == 0 && !string.IsNullOrEmpty((GrdCt.Records[i] as DataRecord).Cells["thue_suat"].Value.ToString()))
                                {
                                    Decimal thue_suat = 0;
                                    thue_suat = Convert.ToDecimal((GrdCt.Records[i] as DataRecord).Cells["thue_suat"].Value);
                                    thue_nt = SmLib.SysFunc.Round(_tien_nt * thue_suat / 100, StartUp.M_ROUND_NT);
                                    thue_nt0 = SmLib.SysFunc.Round(_tien_nt0 * thue_suat / 100, M_Round);
                                    (GrdCt.Records[i] as DataRecord).Cells["thue_nt"].Value = thue_nt;
                                    (GrdCt.Records[i] as DataRecord).Cells["thue"].Value = thue_nt0;
                                }
                                (GrdCt.Records[i] as DataRecord).Cells["tt_nt"].Value = _tien_nt + thue_nt;
                                (GrdCt.Records[i] as DataRecord).Cells["tt"].Value = _tien_nt0 + thue_nt0;
                            }
                        }
                    }
                }
                else if (Ma_GD_Value.Text.Equals("1"))
                {
                    decimal _tien_nt = 0, _tien_nt0 = 0;
                    for (int i = 0; i < Grdhd.Records.Count; i++)
                    {
                        _tien_nt = Convert.ToDecimal((Grdhd.Records[i] as DataRecord).Cells["tien_nt"].Value);
                        _tien_nt0 = SmLib.SysFunc.Round(_tien_nt * _ty_gia, StartUp.M_ROUND);
                        if (_tien_nt0 > 0)
                        {
                            (Grdhd.Records[i] as DataRecord).Cells["tien"].Value = _tien_nt0;
                        }
                    }
                }
                UpdateTotalHT();

                //HĐ thuế
                if (GrdCtgt.Records.Count > 0)
                {
                    for (int i = 0; i < GrdCtgt.Records.Count; i++)
                    {
                        if ((GrdCtgt.Records[i] as DataRecord).Cells["t_tien_nt"].Value != DBNull.Value)
                        {
                            Decimal _tien_nt = 0, _tien_nt0 = 0, thue_nt0 = 0;
                            _tien_nt = Convert.ToDecimal((GrdCtgt.Records[i] as DataRecord).Cells["t_tien_nt"].Value);
                            if (_ty_gia * _tien_nt > 0)
                            {
                                _tien_nt0 = SmLib.SysFunc.Round(_ty_gia * _tien_nt, M_Round);
                                (GrdCtgt.Records[i] as DataRecord).Cells["t_tien"].Value = _tien_nt0;

                                if (ArapLib.FNum.ToDec(GetPhValue("sua_thue")) == 0 && !string.IsNullOrEmpty((GrdCtgt.Records[i] as DataRecord).Cells["thue_suat"].Value.ToString()))
                                {
                                    Decimal thue_suat = 0;
                                    thue_suat = Convert.ToDecimal((GrdCtgt.Records[i] as DataRecord).Cells["thue_suat"].Value);
                                    thue_nt0 = SmLib.SysFunc.Round(_tien_nt0 * thue_suat / 100, M_Round);
                                    if (StartUp.DsTrans.Tables[0].DefaultView[0]["sua_thue"].ToString() == "0")
                                        (GrdCtgt.Records[i] as DataRecord).Cells["t_thue"].Value = thue_nt0;
                                }
                                (GrdCtgt.Records[i] as DataRecord).Cells["t_tt"].Value = _tien_nt0 + thue_nt0;
                            }
                        }
                    }
                    UpdateTotalThue();
                }
            }
        }

        private void txtNgay_ct_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtNgay_ct.Value == DBNull.Value)
                txtNgay_ct.Value = DateTime.Now;
            if (!txtNgay_ct.IsFocusWithin)
                if ((currActionTask == ActionTask.Add || currActionTask == ActionTask.Edit || currActionTask == ActionTask.Copy))
                {
                    if ((StartUp.M_ngay_lct.Equals("0") || txtngay_lct.dValue == new DateTime()) && txtNgay_ct.dValue != new DateTime())
                        txtngay_lct.Value = txtNgay_ct.dValue.Date;
                }
        }


        private void GrdCt_RecordDelete(object sender, Infragistics.Windows.DataPresenter.Events.RecordsDeletedEventArgs e)
        {
            SmLib.WinAPISenkey.SenKey(ModifierKeys.Alt, Key.D2);

            //if (GrdCtgt.Records.Count > 0)
            //{
            //    GrdCtgt.ActiveCell = (GrdCtgt.Records[0] as DataRecord).Cells[0];
            //    GrdCtgt.Focus();
            //}
        }

        private void GrdCtChi_RecordDelete(object sender, Infragistics.Windows.DataPresenter.Events.RecordsDeletedEventArgs e)
        {
            if (Ma_GD_Value.Text.Equals("4"))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
                {
                    txthan_tt.Focus();
                }));
            }
            else
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        (this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus();
                    }));
            }
        }

        private void FormMain_Closed(object sender, EventArgs e)
        {
            if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
                App.Current.Shutdown();
        }
        #region GetLanguageString
        public override string GetLanguageString(string code, string language)
        {
            return StartUp.GetLanguageString(code, language);
        }
        #endregion

        private void TabInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender != null)
                VisibilityTextbox((sender as TabControl).SelectedIndex, cbMa_nt.Text);
            if (PhView[0]["ma_gd"] != DBNull.Value && !string.IsNullOrEmpty(PhView[0]["ma_gd"].ToString()) && PhView[0]["ma_gd"].ToString().Equals("8"))
            {
                if (TabInfo.SelectedIndex == 0)
                {
                    UpdateTotalHT();
                }
                else if (TabInfo.SelectedIndex == 1)
                {
                    if (IsInEditMode != null)
                        if (IsInEditMode.Value)
                        {
                            ChkSuaThue_Click(null, null);

                            if (ChkSuaThue.IsChecked == false)
                            {
                                DataRow[] drThue = StartUp.DsTrans.Tables[2].Select("stt_rec= '" + PhData.Rows[FrmCACTBN1.iRow]["stt_rec"].ToString() + "'");
                                foreach (DataRow rThue in drThue)
                                    StartUp.DsTrans.Tables[2].Rows.Remove(rThue);
                                PhView[0]["ispostgt"] = true;

                                bool _haskh_mau_hd = false;
                                foreach (Field _field in GrdCtgt.FieldLayouts[0].Fields)
                                {
                                    if (_field.Name.Equals("kh_mau_hd"))
                                    {
                                        _haskh_mau_hd = true;
                                    }
                                }

                                for (int i = 0; i < GrdCt.Records.Count; i++)
                                {
                                    DataRowView drVCT = (GrdCt.Records[i] as DataRecord).DataItem as DataRowView;
                                    //09/09/2011 Akhai đồng ý post toàn bộ qua tab thuế để lên ctgt30
                                    //if (!drVCT["loai_hd"].Equals(DBNull.Value) && !drVCT["loai_hd"].ToString().Trim().Equals("5") && !drVCT["loai_hd"].ToString().Trim().Equals("0"))
                                        if (!drVCT["ngay_ct0"].Equals(DBNull.Value) 
                                            && !string.IsNullOrEmpty(drVCT["ngay_ct0"].ToString())
                                            && !drVCT["loai_hd"].ToString().Trim().Equals("5"))
                                        {
                                            DataRow NewRecord = StartUp.DsTrans.Tables[2].NewRow();
                                            NewRecord["stt_rec"] = PhView[0]["stt_rec"];

                                            NewRecord["stt_rec0"] = drVCT["stt_rec0"];
                                            NewRecord["ma_ct"] = StartUp.Ma_ct;
                                            NewRecord["ngay_ct"] = txtNgay_ct.Value;

                                            NewRecord["ma_ms"] = drVCT["ma_ms"];
                                            NewRecord["so_ct0"] = drVCT["so_ct0"];
                                            NewRecord["so_seri0"] = drVCT["so_seri0"];

                                            if (_haskh_mau_hd)
                                                NewRecord["kh_mau_hd"] = drVCT["kh_mau_hd"];

                                            NewRecord["ngay_ct0"] = drVCT["ngay_ct0"];
                                            NewRecord["ma_kh"] = drVCT["ma_kh_t"];
                                            NewRecord["ten_kh"] = drVCT["ten_kh_t"];
                                            NewRecord["dia_chi"] = drVCT["dia_chi_t"];
                                            NewRecord["ma_so_thue"] = drVCT["mst_t"];
                                            NewRecord["ten_vt"] = drVCT["ten_vt_t"];
                                            //NewRecord["ten_vt"] = txtDien_giai.Text;
                                            //NewRecord["ten_vt"] = drVCT["dien_giaii"];

                                            NewRecord["t_tien_nt"] = drVCT["tien_nt"];
                                            NewRecord["t_tien"] = drVCT["tien"];

                                            NewRecord["ma_thue"] = drVCT["ma_thue_i"];
                                            NewRecord["thue_suat"] = drVCT["thue_suat"];
                                            NewRecord["t_thue_nt"] = drVCT["thue_nt"];
                                            NewRecord["t_thue"] = drVCT["thue"];
                                            NewRecord["t_tt_nt"] = drVCT["tt_nt"];
                                            NewRecord["t_tt"] = drVCT["tt"];
                                            NewRecord["tk_thue_no"] = drVCT["tk_thue_i"];
                                            NewRecord["tk_thue_cn"] = drVCT["tk_thue_cn"];
                                            NewRecord["ma_kh2"] = drVCT["ma_kh2_t"];
                                            NewRecord["ghi_chu"] = drVCT["ghi_chu_t"];

                                            NewRecord["ma_vv"] = drVCT["ma_vv_i"];
                                            NewRecord["ma_phi"] = drVCT["ma_phi_i"];

                                            if(GrdCtgt.DefaultFieldLayout.Fields.Any(x=>x.Name == "ma_td4"))
                                                NewRecord["ma_td4"] = drVCT["ma_td4_i"];

                                            StartUp.DsTrans.Tables[2].Rows.Add(NewRecord);
                                        }

                                        if (drVCT["loai_hd"].ToString().Trim().Equals("5"))
                                        {
                                            StartUp.DsTrans.Tables[0].DefaultView[0]["sua_thue"] = 1;
                                        }
                                }
                            }
                            if (GrdCtgt.Records.Count == 0)
                                GrdCtgt_AddNewRecord(null, null);

                        }
                    UpdateTotalThue();
                }
            }
        }

        public void VisibilityTextbox(int index, string ma_nt)
        {
            if (index == 0)
            {
                GrdLayout60.Visibility = Visibility.Collapsed;
                GrdLayout61.Visibility = Visibility.Collapsed;
            }
            else
            {
                GrdLayout60.Visibility = Visibility.Visible;
                if (!ma_nt.Equals(StartUp.M_ma_nt0))
                    GrdLayout61.Visibility = Visibility.Visible;
            }
        }

        private void ChkSuaThue_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    if (IsInEditMode.Value && Ma_GD_Value.Value)
                        IsInEditModeThue.Value = (ChkSuaThue.IsChecked.Value);
                    else
                        IsInEditModeThue.Value = false;
                }));
        }

        private void FormMain_EditModeEnded(object sender, string menuItemName, RoutedEventArgs e)
        {
            ChkSuaTien_Click(sender, e);

            Voucher_Ma_nt0.Text = PhView[0]["ma_nt"].ToString();
            Voucher_Ma_nt0.Value = (PhView[0]["ma_nt"].ToString().Equals(M_Ma_nt0));
            Loai_tg.Text = PhView[0]["loai_tg"].ToString();

            Ma_GD_Value.Text = txtMa_gd.Text;
            if (txtMa_gd.Text.Equals("8"))
                Ma_GD_Value.Value = true;
            else
                Ma_GD_Value.Value = false;
            SetStatusVisibleField();

            if (IsInEditMode.Value && Ma_GD_Value.Value)
                IsInEditModeThue.Value = (ChkSuaThue.IsChecked.Value);
            else
                IsInEditModeThue.Value = false;
            //LoadDataDu13();
            if (StartUp.DsTrans.Tables[0].Rows.Count <= 1)
                SetEmpty();
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                TabInfo_SelectionChanged(null, null);
            }));
        }


        private void SetEmpty()
        {
            txtT_t_Tien.Text = "";
            txtT_t_thue.Text = "";
            txtT_t_tt.Text = "";
            txtT_t_Tien_nt.Text = "";
            txtT_t_thue_nt.Text = "";
            txtT_t_tt_nt.Text = "";
            txtT_t_Tien_Nt0.Text = "";
            txtT_t_thue_Nt0.Text = "";
            txtT_t_tt_Nt0.Text = "";
        }
        //private void LoadDataDu13()
        //{
        //    txtSoDuKH.Value = ArapLib.ArFuncLib.GetSdkh13(StartUp.SysObj, PhView[0]["ma_kh"].ToString(), PhView[0]["tk"].ToString());
        //}

        private void txtTy_gia_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Voucher_Ma_nt0.Value)
            {
                KeyboardNavigation.SetTabNavigation(GrNT, KeyboardNavigationMode.Continue);
                SmLib.WinAPISenkey.SenKey(ModifierKeys.None, Key.Tab);
            }
        }

        private void txtMa_qs_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (IsInEditMode.Value && !e.NewFocus.GetType().Equals(typeof(SmVoucherLib.ToolBarButton)))
                if (!string.IsNullOrEmpty(PhView[0]["ma_qs"].ToString()))
                {
                    if (string.IsNullOrEmpty(PhView[0]["so_ct"].ToString().Trim()))
                    {
                        if (string.IsNullOrEmpty(PhView[0]["so_cttmp"].ToString().Trim()) || !PhView[0]["ma_qs"].ToString().Trim().Equals(PhView[0]["ma_qstmp"].ToString().Trim()))
                        {
                            txtSo_ct.Text = GetNewSoct(StartUp.SysObj, txtMa_qs.Text);
                            PhView[0]["so_cttmp"] = txtSo_ct.Text;
                            PhView[0]["ma_qstmp"] = txtMa_qs.Text;
                        }
                        else
                            txtSo_ct.Text = PhView[0]["so_cttmp"].ToString().Trim();
                    }
                }
        }

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

        private void txtngay_lct_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!txtngay_lct.IsFocusWithin)
                if (currActionTask == ActionTask.Add || currActionTask == ActionTask.Edit || currActionTask == ActionTask.Copy)
                {
                    if (!txtNgay_ct.dValue.Date.Equals(txtngay_lct.dValue.Date))
                    {
                        ExMessageBox.Show( 445,StartUp.SysObj, "Ngày lập chứng từ khác với ngày hạch toán!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
        }

        private void txtMa_gd_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtMa_gd.RowResult != null && !string.IsNullOrEmpty(txtMa_gd.Text.Trim()))
            {
                if (M_LAN.ToUpper().Equals("V"))
                    txtTen_gd.Text = txtMa_gd.RowResult["ten_gd"].ToString();
                else
                    txtTen_gd.Text = txtMa_gd.RowResult["ten_gd2"].ToString();
            }
            Ma_GD_Value.Text = txtMa_gd.Text;
            if (txtMa_gd.Text.Equals("8"))
                Ma_GD_Value.Value = true;
            else
            {
                Ma_GD_Value.Value = false;
                PhView[0]["sua_thue"] = 0;
            }
            SetStatusVisibleField();
        }

        private void txtHan_tt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                (this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus();
                e.Handled = true;
            }
        }

        private void txthan_tt_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!Ma_GD_Value.Text.Equals("4"))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
                    {
                        (this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus();
                    }));
                e.Handled = true;
            }
            //Binding binding = new Binding("Text");
            //binding.Source = Ma_GD_Value;
            //txthan_tt.SetBinding(NumericTextBox.IsReadOnlyProperty, binding);
            
        }

        #region GrdCt_PreviewGotKeyboardFocus
        private void GrdCt_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (currActionTask == ActionTask.Add)
            {
                if (CtView.Count == 1 && CtView[0]["dien_giaii"].ToString() == string.Empty)
                {
                    CtView[0]["dien_giaii"] = PhView[0].Row["dien_giai"];
                }
            }
        }
       

        private void GrdCtChi_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (currActionTask == ActionTask.Add)
            {
                if (CtView.Count == 1 && CtView[0]["dien_giaii"].ToString() == string.Empty)
                {
                    CtView[0]["dien_giaii"] = PhView[0].Row["dien_giai"];
                }
            }
        }
        #endregion

        private void btnSoHD_Click(object sender, RoutedEventArgs e)
        {
            if (IsEditMode)
            {
                ExMessageBox.Show( 450,StartUp.SysObj, "Phải lưu chứng từ rồi mới phân bổ cho các hóa đơn!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (!PhData.DefaultView[0]["status"].ToString().Equals("2"))
            {
                ExMessageBox.Show( 455,StartUp.SysObj, "Phải ghi vào sổ cái rồi mới phân bổ cho hóa đơn!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                string stt_rec = Stt_rec;
                object ngay = PhData.Rows[iRow]["ngay_ct"];
                PbInfo pb = new PbInfo(ngay, ngay, "", "", "");
                pb.TitleView = "Phan bo";
                string[] paras = StartUp.CommandInfo["parameter"].ToString().Split(new char[] { ';' });
                Apttpb.StartUp.Procedure = paras;
                (new Apttpb.StartUp()).Pb_tt(stt_rec, pb);


                Dispatcher.BeginInvoke(new Action(() =>
                {
                    SqlCommand cmd = new SqlCommand("SELECT so_ct_tt FROM " + StartUp.DmctInfo["m_phdbf"].ToString() + " WHERE stt_rec = @Stt_rec");
                    cmd.Parameters.Add("@stt_rec", SqlDbType.VarChar, 50).Value = Stt_rec;
                    DataSet ds = SysObj.ExcuteReader(cmd);
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        lblSo_ct_tt.Text = ds.Tables[0].Rows[0][0].ToString();
                    else
                        lblSo_ct_tt.Text = "";
                }), DispatcherPriority.Background);
            }
        }

        private void Grdhd_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            try
            {
                DataRecord rec = e.Cell.Record as DataRecord;
                //Kiểm tra activecell khác null và
                if (IsInEditMode.Value && Grdhd.ActiveCell != null && CtView.Count > Grdhd.ActiveRecord.Index && CtData.GetChanges(DataRowState.Deleted) == null)
                    switch (e.Cell.Field.Name)
                    {
                        case "tk_i":
                            {
                                AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                if (txt.RowResult != null)
                                {
                                    //gán tên tài khoản
                                    if (M_LAN.ToUpper().Equals("V"))
                                        e.Cell.Record.Cells["ten_tk"].Value = txt.RowResult["ten_tk"];
                                    else
                                        e.Cell.Record.Cells["ten_tk2"].Value = txt.RowResult["ten_tk2"];

                                    if (e.Cell.Record.Index == 0)
                                    {
                                        if (string.IsNullOrEmpty(e.Cell.Record.Cells["dien_giaii"].Value.ToString()))
                                        {
                                            e.Cell.Record.Cells["dien_giaii"].Value = PhView[0]["dien_giai"].ToString();
                                        }
                                    }
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
                        case "tien_nt":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    decimal _Tien_nt;
                                    _Tien_nt = 0;
                                    if (!string.IsNullOrEmpty(e.Editor.Text.Trim()))
                                        _Tien_nt = (e.Editor as NumericTextBox).nValue;

                                    if (txtTy_gia.Value != null && !string.IsNullOrEmpty(e.Editor.Text.Trim()))
                                    {
                                        decimal _Ty_gia, Tien = 0;
                                        _Ty_gia = txtTy_gia.nValue;
                                        Tien = SmLib.SysFunc.Round(_Ty_gia * _Tien_nt, M_Round);
                                        if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                        {
                                            e.Cell.Record.Cells["tien"].Value = e.Cell.Record.Cells["tien_nt"].Value;
                                        }
                                        else
                                        {
                                           //if (Tien > 0)
                                            {
                                                e.Cell.Record.Cells["tien"].Value = Tien;
                                            }
                                        }
                                    }

                                    if (e.Cell.Record.Cells["ma_nt_i"].Value.ToString() == Ma_nt /*|| Ma_nt == StartUp.M_ma_nt0*/)
                                        e.Cell.Record.Cells["tt_qd"].Value = _Tien_nt;
                                    UpdateTotalHT();
                                }
                                break;
                            }
                        case "tien":
                            {
                                if (e.Cell.IsDataChanged)
                                    UpdateTotalHT();
                            }
                            break;
                        case "so_ct0":
                            {

                                //if (e.Editor.Value is DBNull && ExMessageBox.Show( 460,M_LAN, "Có nhập tiếp không?", StartUp.SysObj.GetSysVar("M_FAST_VER").ToString(), MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                                //    break;
                                if ((e.Editor.Value is DBNull || string.IsNullOrEmpty(e.Editor.Value.ToString().Trim())) && ExMessageBox.Show(465, StartUp.SysObj, "Có nhập tiếp không?", StartUp.SysObj.GetSysVar("M_FAST_VER").ToString(), MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                                {
                                    DataRecord ARow = (Grdhd.ActiveRecord as DataRecord);
                                    if (ARow != null)
                                    {
                                        int indexRecord = 0, indexCell = 0;
                                        Cell cell = Grdhd.ActiveCell;

                                        indexRecord = ARow.Index;
                                        if (ARow.Index == 0)
                                        {
                                            if (Grdhd.Records.Count == 1)
                                                Grdhd_AddNewRecord(null, null);
                                        }
                                        else if (ARow.Index == Grdhd.Records.Count - 1)
                                        {
                                            indexRecord = ARow.Index - 1;
                                        }

                                        indexCell = Grdhd.ActiveCell == null ? 0 : Grdhd.ActiveCell.Field.Index;

                                        Grdhd.ExecuteCommand(DataPresenterCommands.EndEditModeAndDiscardChanges);
                                        if (indexCell >= 0)
                                        {
                                            CtData.Rows.Remove(CtView[ARow.Index].Row);
                                            CtData.AcceptChanges();
                                            if (Grdhd.Records.Count > 0)
                                                Grdhd.ActiveRecord = Grdhd.Records[Grdhd.Records.Count - 1];
                                            UpdateTotalHT();
                                        }
                                    }
                                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                    {
                                        (this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus();
                                    }));
                                    break;
                                }
                                //if (!e.Cell.IsDataChanged)
                                //    break;
                                if (isCurrentFormActive)
                                    return;

                                FrmCACTBN1DSHD frmDSHD = new FrmCACTBN1DSHD();
                                frmDSHD.ShowHd(e.Editor.Value.ToString(), stt_rec_magd1);

                                DataRecord dr = frmDSHD.grdDSHD.ActiveRecord as DataRecord;
                                if (dr != null && dr.RecordType == RecordType.DataRecord)
                                {
                                    decimal t_tt_qd = 0;
                                    string stt_rec_hd = string.Empty;

                                    t_tt_qd = FNum.ToDec(dr.Cells["t_tt_qd"].Value);
                                    stt_rec_hd = dr.Cells["stt_rec"].Value.ToString();
                                    stt_rec_magd1 = stt_rec_hd;
                                    Debug.WriteLine(stt_rec_hd);

                                    rec.Cells["so_ct0"].Value = dr.Cells["so_ct"].Value.ToString();
                                    rec.Cells["ngay_ct0"].Value = (DateTime)dr.Cells["ngay_ct"].Value;
                                    rec.Cells["tk_i"].Value = dr.Cells["tk"].Value.ToString();
                                    rec.Cells["ma_nt_i"].Value = dr.Cells["ma_nt"].Value.ToString();
                                    rec.Cells["t_tt_nt0"].Value = dr.Cells["tc_tt"].Value;
                                    ((rec.Cells.Record as DataRecord).DataItem as DataRowView)["ty_giahtf2"] = ((dr.Cells.Record as DataRecord).DataItem as DataRowView)["ty_giaf"];
                                    ((rec.Cells.Record as DataRecord).DataItem as DataRowView)["ty_gia_ht2"] = ((dr.Cells.Record as DataRecord).DataItem as DataRowView)["ty_gia"];
                                    rec.Cells["t_tt_qd"].Value = t_tt_qd;
                                    //rec.Cells["phai_tt_nt"].Value = dr.Cells["cl_tt"].Value;
                                    rec.Cells["phai_tt_nt"].Value = FNum.ToDec(rec.Cells["t_tt_nt0"].Value) - FNum.ToDec(rec.Cells["t_tt_qd"].Value);
                                    rec.Cells["stt_rec_tt"].Value = stt_rec_hd;


                                    Debug.WriteLine(rec.Cells["phai_tt_nt"].Value.ToString());

                                    if (rec.Cells.Any(x => x.Field.Name == "ma_vv_i"))
                                        rec.Cells["ma_vv_i"].Value = dr.Cells["ma_vv"].Value.ToString();

                                    if (rec.Cells.Any(x => x.Field.Name == "ma_td_i"))
                                        rec.Cells["ma_td_i"].Value = dr.Cells["ma_td"].Value.ToString();

                                    if (rec.Cells.Any(x => x.Field.Name == "ma_td2_i"))
                                        rec.Cells["ma_td2_i"].Value = dr.Cells["ma_td2"].Value.ToString();

                                    if (rec.Cells.Any(x => x.Field.Name == "ma_td3_i"))
                                        rec.Cells["ma_td3_i"].Value = dr.Cells["ma_td3"].Value.ToString();

                                    if (rec.Cells.Any(x => x.Field.Name == "so_lsx_i"))
                                        rec.Cells["so_lsx_i"].Value = dr.Cells["so_lsx"].Value.ToString();

                                    if (rec.Cells.Any(x => x.Field.Name == "so_dh_i"))
                                        rec.Cells["so_dh_i"].Value = dr.Cells["so_dh"].Value.ToString();

                                    if (rec.Cells.Any(x => x.Field.Name == "ma_bpht_i"))
                                        rec.Cells["ma_bpht_i"].Value = dr.Cells["ma_bpht"].Value.ToString();

                                    if (rec.Cells.Any(x => x.Field.Name == "ma_hd_i"))
                                        rec.Cells["ma_hd_i"].Value = dr.Cells["ma_hd"].Value.ToString();

                                    if (rec.Cells.Any(x => x.Field.Name == "ma_ku_i"))
                                        rec.Cells["ma_ku_i"].Value = dr.Cells["ma_ku"].Value.ToString();

                                    if (rec.Cells.Any(x => x.Field.Name == "ma_phi_i"))
                                        rec.Cells["ma_phi_i"].Value = dr.Cells["ma_phi"].Value;

                                    if (rec.Cells.Any(x => x.Field.Name == "ma_sp"))
                                        rec.Cells["ma_sp"].Value = dr.Cells["ma_sp"].Value;
                                }
                                else
                                {
                                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                    {
                                        Grdhd.ActiveCell = e.Cell;
                                        Grdhd.ExecuteCommand(DataPresenterCommands.StartEditMode);
                                    }));
                                    //this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                    //{
                                    //    Grdhd.ExecuteCommand(DataPresenterCommands.StartEditMode);
                                    //})); 
                                    //Grdhd.PreviewKeyUp += (a, b) => { b.Handled = true; };
                                    //this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                    //{
                                    //    if (e.Editor.Value.ToString().Trim() == "")
                                    //        Grdhd.ExecuteCommand(DataPresenterCommands.EndEditModeAndCommitRecord);
                                    //}));
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

        #region Grdhd_PreviewGotKeyboardFocus
        private void Grdhd_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (currActionTask == ActionTask.Add)
            {
                if (CtView.Count == 1 && CtView[0]["dien_giaii"].ToString() == string.Empty)
                {
                    CtView[0]["dien_giaii"] = PhView[0].Row["dien_giai"];
                }
            }
        }
        #endregion

        private bool Grdhd_AddNewRecord(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            NewRowCt();
            return true;
        }



        private void Grdhd_RecordDelete(object sender, Infragistics.Windows.DataPresenter.Events.RecordsDeletedEventArgs e)
        {
            //SmLib.WinAPISenkey.SenKey(ModifierKeys.Alt, Key.D2);
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                //if ((sender as DataGridView).Records.Count == 0)
                (this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus();
            }), DispatcherPriority.Background);
        }

        private void Grdhd_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsInEditMode.Value == false)
                return;
            if (Keyboard.IsKeyDown(Key.N) && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                NewRowCt();
                Grdhd.ActiveRecord = Grdhd.Records[Grdhd.Records.Count - 1];
            }
        }

        private void Grdhd_KeyUp(object sender, KeyEventArgs e)
        {
            if (IsInEditMode.Value == false)
                return;

            switch (e.Key)
            {
                case Key.F4:
                    Grdhd.ExecuteCommand(DataPresenterCommands.StartEditMode);
                    if (Keyboard.FocusedElement.GetType().Name.Equals("TextBoxAutoComplete"))
                    {
                        AutoCompleteTextBox txt = (Keyboard.FocusedElement as TextBoxAutoComplete).ParentControl;
                        if (!txt.CheckLostFocus())
                            return;
                    }
                    DataGridView g = sender as DataGridView;

                    if (g.Records.Any(x => (x as DataRecord).Cells["so_ct0"].Value.ToString().Trim() == ""))
                        return;

                

                    NewRowCt();
                    Grdhd.ActiveRecord = Grdhd.Records[Grdhd.Records.Count - 1];
                    Grdhd.ActiveCell = (Grdhd.ActiveRecord as DataRecord).Cells["so_ct0"];
                    break;
                case Key.F5:
                    if (StartUp.SysObj.VersionInfo.Rows[0]["product_code"].ToString().Equals("FA") || (StartUp.dtRegInfo != null && !StartUp.dtRegInfo.Rows[18]["content"].ToString().Trim().Equals("FK")))
                    {
                        CatgLib.Catinhtg.Tinh(GrdCtChi.Records, this);
                    }
                    break;
                case Key.F8:
                    {
                        if (ExMessageBox.Show( 470,StartUp.SysObj, "Có xóa dòng ghi hiện thời không?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                        {
                            return;
                        }

                        DataRecord ARow = (Grdhd.ActiveRecord as DataRecord);
                        if (ARow != null)
                        {
                            int indexRecord = 0, indexCell = 0;
                            Cell cell = Grdhd.ActiveCell;

                            indexRecord = ARow.Index;
                            if (ARow.Index == 0)
                            {
                                if (Grdhd.Records.Count == 1)
                                    Grdhd_AddNewRecord(null, null);
                            }
                            else if (ARow.Index == Grdhd.Records.Count - 1)
                            {
                                indexRecord = ARow.Index - 1;
                            }

                            indexCell = Grdhd.ActiveCell == null ? 0 : Grdhd.ActiveCell.Field.Index;

                            Grdhd.ExecuteCommand(DataPresenterCommands.EndEditModeAndDiscardChanges);
                            if (indexCell >= 0)
                            {
                                CtData.Rows.Remove(CtView[ARow.Index].Row);
                                CtData.AcceptChanges();
                                if (Grdhd.Records.Count > 0)
                                    Grdhd.ActiveRecord = Grdhd.Records[indexRecord > Grdhd.Records.Count - 1 ? Grdhd.Records.Count - 1 : indexRecord];
                                UpdateTotalHT();
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        bool isEquals(decimal a, decimal b, string stt_rec)
        {
            if (stt_rec != Stt_rec)
                return true;
            return a == b;
        }


        #region IPhValue
        public SysObject SysObj { get { return StartUp.SysObj; } }
        public int M_Round { get { return StartUp.M_ROUND; } }
        public int M_Round_nt { get { return StartUp.M_ROUND_NT; } }
        public int M_ROUND_TY_GIA { get { return Convert.ToInt16(StartUp.SysObj.GetSysVar("M_ROUND_TY_GIA")); } }
        public string M_Ma_nt0 { get { return StartUp.M_ma_nt0; } }

        public decimal Sua_thue
        {
            get { return ArapLib.FNum.ToDec(GetPhValue("sua_thue")); }
            set { SetPhValue("sua_thue", value); }
        }

        public decimal Sua_tien
        {
            get { return ArapLib.FNum.ToDec(GetPhValue("sua_tien")); }
            set { SetPhValue("sua_tien", value); }
        }
        public decimal Sua_tggs
        {
            get { return ArapLib.FNum.ToDec(GetPhValue("sua_tggs")); }
            set { SetPhValue("sua_tggs", value); }
        }

        public decimal Ty_gia
        {
            get { return ArapLib.FNum.ToDec(GetPhValue("ty_gia")); }
            set { SetPhValue("ty_gia", value); }
        }
        public decimal Ty_gia_ht
        {
            get { return ArapLib.FNum.ToDec(GetPhValue("ty_gia_ht")); }
            set { SetPhValue("ty_gia_ht", value); }
        }
        public decimal Ty_giaf
        {
            get { return ArapLib.FNum.ToDec(GetPhValue("ty_giaf")); }
            set { SetPhValue("ty_giaf", value); }
        }

        public decimal Ty_gia_htf
        {
            get { return ArapLib.FNum.ToDec(GetPhValue("ty_gia_htf")); }
            set { SetPhValue("ty_gia_htf", value); }
        }
        public decimal mLoai_tg
        {
            get { return ArapLib.FNum.ToDec(GetPhValue("loai_tg")); }
            set { SetPhValue("loai_tg", value); }
        }
        public DateTime Ngay_ct
        {
            get { return ArapLib.FDate.ToDate(GetPhValue("ngay_ct")); }
            set { SetPhValue("ngay_ct", value); }
        }
        public string Stt_rec
        {
            get { return GetPhValue("stt_rec").ToString(); }
            set { SetPhValue("stt_rec", value); }
        }
        public string Ma_ct
        {
            get { return GetPhValue("ma_ct").ToString(); }
            set { SetPhValue("ma_ct", value); }
        }
        public string Ma_gd
        {
            get { return GetPhValue("ma_gd").ToString(); }
            set { SetPhValue("ma_gd", value); }
        }
        public string Ma_nt
        {
            get { return GetPhValue("ma_nt").ToString(); }
            set { SetPhValue("ma_nt", value); }
        }
        public string Ma_kh
        {
            get { return GetPhValue("ma_kh").ToString(); }
            set { SetPhValue("ma_kh", value); }
        }
        public string Ma_dvcs
        {
            get { return GetPhValue("ma_dvcs").ToString(); }
            set { SetPhValue("ma_dvcs", value); }
        }
        public string Tk
        {
            get { return GetPhValue("tk").ToString(); }
            set { SetPhValue("tk", value); }
        }

        public string So_ct
        {
            get { return GetPhValue("so_ct").ToString(); }
            set { SetPhValue("so_ct", value); }
        }

        public object GetPhValue(string columnName)
        {
            try
            {
                return PhData.Rows[iRow][columnName];
            }
            catch 
            {
                Type t = PhData.Columns[columnName].DataType;
                if (t.Equals(typeof(string)))
                    return "";
                if (t.Equals(typeof(DateTime)))
                    return DateTime.Now.Date;

                return null;
            }
        }

        public void SetPhValue(string columnName, object value)
        {
            try
            {
                PhData.Rows[iRow][columnName] = value;
            }
            catch
            {
            }
        }

        public void UpdateChanged()
        {
            PhData.AcceptChanges();
        }

        #endregion //IPhValue

        #region Data, view
        private DataView PhView
        {
            get { return PhData.DefaultView; }
        }

        private DataTable PhData
        {
            get { return StartUp.DsTrans.Tables[0]; }
        }

        private DataView CtView
        {
            get { return CtData.DefaultView; }
        }

        private DataTable CtData
        {
            get { return StartUp.DsTrans.Tables[1]; }
        }
        #endregion



    }
}
