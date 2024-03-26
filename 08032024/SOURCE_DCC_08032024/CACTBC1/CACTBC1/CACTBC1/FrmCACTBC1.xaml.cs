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
using ArapLib;
using SysLib;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace CACTBC1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FrmCACTBC1 : SmVoucherLib.FormTrans, IPhValue
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
        CodeValueBindingObject Ma_GD_Value;
        CodeValueBindingObject Loai_tg;
        CodeValueBindingObject Ip_tien_hd;

        
        //Lưu lại dữ liệu khi thêm sửa
        private DataSet DsVitual;
        DataSet dsCheckData;

        public FrmCACTBC1()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(FormTrans_Loaded);
            LanguageProvider.Language = StartUp.M_LAN;
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
                Voucher_Lan0 = (CodeValueBindingObject)FormMain.FindResource("Voucher_Lan0");
                Voucher_Ma_nt = (CodeValueBindingObject)FormMain.FindResource("Voucher_Ma_nt");
                IsCheckedSua_tggs = (CodeValueBindingObject)FormMain.FindResource("IsCheckedSua_tggs");
                IsCheckedSua_tien = (CodeValueBindingObject)FormMain.FindResource("IsCheckedSua_tien");
                IsInEditModeThue = (CodeValueBindingObject)FormMain.FindResource("IsInEditModeThue");
                Ty_Gia_ValueChange = (CodeValueBindingObject)FormMain.FindResource("Ty_Gia_ValueChange");
                Ma_GD_Value = (CodeValueBindingObject)FormMain.FindResource("Ma_GD_Value");
                Loai_tg = (CodeValueBindingObject)FormMain.FindResource("Loai_tg");
                Ip_tien_hd = (CodeValueBindingObject)FormMain.FindResource("Ip_tien_hd");
                Ip_tien_hd.Text = StartUp.M_IP_TIEN_HD;

                txtMa_gd.Filter = string.Format("ma_ct = '{0}' and status = 1 ", StartUp.Ma_ct);
                txtMa_qs.Filter = string.Format("ma_cts like '%{0}%' and status = 1 ", StartUp.Ma_ct);

                Binding bind = new Binding("Value");
                bind.Source = IsInEditMode;
                bind.Mode = BindingMode.OneWay;
                this.SetBinding(FormTrans.IsEditModeProperty, bind);

                //Gán ngôn ngữ messagebox
                M_LAN = StartUp.SysObj.GetOption("M_LAN").ToString();
                GrdCt.Lan = M_LAN;
                Voucher_Lan0.Value = M_LAN.Equals("V");
                tblGhi_chu.Text = M_LAN.Equals("V") ? "Ghi chú" : "Note";

                //Them cac truong tu do
                SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(this.BindingSysObj, Grdhd, StartUp.Ma_ct, 1);
                SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(this.BindingSysObj, GrdCt, StartUp.Ma_ct, 1);
                if (PhData.Rows.Count > 0)
                {
                    PhView.RowFilter = "stt_rec= '" + Stt_rec + "'";
                    CtView.RowFilter = "stt_rec= '" + Stt_rec + "'";

                    GrdLayout00.DataContext = PhView;

                    this.GrdCt.DataSource = CtView;
                    Grdhd.DataSource = CtView;

                    txtStatus.ItemsSource = StartUp.tbStatus.DefaultView;

                    if (StartUp.tbStatus.DefaultView.Count == 1)
                    {
                        txtStatus.IsEnabled = false;
                    }
                    //Xét lại các Field khi thay đổi record hiển thị
                    PhView.ListChanged += new ListChangedEventHandler(phValue_ListChanged);
                    CtView.ListChanged += new ListChangedEventHandler(DefaultView_ListChanged);

                    IsCheckedSua_tggs.Value = (PhView[0]["sua_tggs"].ToString() == "1");
                    IsCheckedSua_tien.Value = (PhView[0]["sua_tien"].ToString() == "1");
                    Ty_Gia_ValueChange.Value = true;
                    Loai_tg.Text = PhView[0]["loai_tg"].ToString();
                }

                Voucher_Ma_nt0.Text = PhView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (PhView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));

                Ma_GD_Value.Text = PhView[0]["ma_gd"].ToString();
                if (txtMa_gd.Text.Equals("8"))
                    Ma_GD_Value.Value = true;
                else
                    Ma_GD_Value.Value = false;

                SetStatusVisibleField();

                //LoadDataDu13();
                SetFocusToolbar();
                if (Grdhd.FieldLayouts[0].Fields.Any(x => x.Name == "ma_vv_i"))
                {
                    Grdhd.FieldLayouts[0].Fields["ma_vv_i"].Settings.AllowEdit = false;
                    Grdhd.FieldLayouts[0].Fields["ma_vv_i"].Settings.EditorStyle = null;
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        private void TinhLai_tien_tt()
        {
            Decimal _ty_gia = this.txtTy_gia.nValue;
            if (StartUp.M_Gd_2Tg_List.Contains(Ma_gd))
            {
                for (int i = 0; i < this.GrdCt.Records.Count; i++)
                {
                    DataRecord rec = GrdCt.Records[i] as DataRecord;
                    rec.Cells["ty_giahtf2"].Value = txtTy_gia.Value;
                    Decimal _ty_gia_ht2 = txtTy_gia.nValue;

                    _ty_gia_ht2 = (mLoai_tg == 1 ? _ty_gia_ht2 : (_ty_gia_ht2 == 0 ? 0 : SmLib.SysFunc.Round(1 / _ty_gia_ht2, M_ROUND_TY_GIA)));
                    (rec.DataItem as DataRowView)["ty_gia_ht2"] = _ty_gia_ht2;

                    //Decimal _tien_nt = 0;
                    //_tien_nt = Convert.ToDecimal(rec.Cells["tien_nt"].Value);
                }
            }
        }
        void phValue_ListChanged(object sender, ListChangedEventArgs e)
        {
            IsCheckedSua_tggs.Value = (PhView[0]["sua_tggs"].ToString() == "1");
        }

        void DefaultView_ListChanged(object sender, ListChangedEventArgs e)
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

                Voucher_Ma_nt0.Text = PhView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (PhView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
                SetStatusVisibleField();
            }
        }

        private void V_Sau()
        {
            if (iRow < PhData.Rows.Count - 1)
            {
                iRow++;
                PhView.RowFilter = "stt_rec= '" + Stt_rec + "'";
                CtView.RowFilter = "stt_rec= '" + Stt_rec + "'";

                Voucher_Ma_nt0.Text = PhView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (PhView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
                SetStatusVisibleField();
            }  
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

            Voucher_Ma_nt0.Text = PhView[0]["ma_nt"].ToString();
            Voucher_Ma_nt0.Value = (PhView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
            SetStatusVisibleField();
        }

        private void V_Cuoi()
        {
            iRow = PhData.Rows.Count - 1;
            PhView.RowFilter = "stt_rec= '" + Stt_rec + "'";
            CtView.RowFilter = "stt_rec= '" + Stt_rec + "'";

            Voucher_Ma_nt0.Text = PhView[0]["ma_nt"].ToString();
            Voucher_Ma_nt0.Value = (PhView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
            SetStatusVisibleField();
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
                            //xóa các row trong table[1]
                            string stt_rec = PhView[0]["stt_rec"].ToString();

                            // Nên dịch chuyển iRow lùi dòng 0
                            // Sau đó RowFilter lại Table[0], Table[1]
                            PhView.RowFilter = "stt_rec= '" + PhData.Rows[0]["stt_rec"].ToString() + "'";
                            CtView.RowFilter = "stt_rec= '" + PhData.Rows[0]["stt_rec"].ToString() + "'";
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

                            //Refresh lại table[0]
                            PhData.Rows.RemoveAt(iRow);

                            DataRow rowPh = PhData.NewRow();
                            rowPh.ItemArray = DsVitual.Tables[0].Rows[0].ItemArray;
                            PhData.Rows.InsertAt(rowPh, iRow);

                            PhView.RowFilter = "stt_rec= '" + stt_rec + "'";
                            CtView.RowFilter = "stt_rec= '" + stt_rec + "'";

                            CtData.Merge(DsVitual.Tables[1]);

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
                                        cmd.CommandText = "Exec [CACTPT1-IncludeVoucher] @Stt_rec, @Stt_rec_tt, @Ma_nt, @Tt_qd, @Tt_dn, @Tt_dn_nt";
                                        cmd.Parameters.Add("@Stt_rec", SqlDbType.VarChar).Value = stt_rec;
                                        cmd.Parameters.Add("@Stt_rec_tt", SqlDbType.VarChar).Value = stt_rec_tt;
                                        cmd.Parameters.Add("@Ma_nt", SqlDbType.VarChar).Value = ma_nt;
                                        cmd.Parameters.Add("@Tt_qd", SqlDbType.Decimal).Value = tt_qd;
                                        cmd.Parameters.Add("@Tt_dn", SqlDbType.Decimal).Value = tt_dn;
                                        cmd.Parameters.Add("@Tt_dn_nt", SqlDbType.Decimal).Value = tt_dn_nt;
                                        cmd.Parameters.Add("@User_id", SqlDbType.Int).Value = user_id;
                                        StartUp.SysObj.ExcuteNonQuery(cmd);

                                        cmd = new SqlCommand();
                                        cmd.CommandText = "EXEC Arttpb;70 @Stt_rec, @Ma_ct";
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
                

                //Delete tksd13
                StartUpTrans.UpdateTkSd13(1, 0);

                //xóa trong ph, ct, ctgt
                //xóa chứng từ
                StartUp.DeleteVoucher(_stt_rec);

                // ----Warning : Không nên xóa Table[0] trước, nếu xóa trước sẽ bị mất Binding -----------------------
                // Nên dịch chuyển iRow lùi dòng 0
                // Sau đó RowFilter lại Table[0], Table[1]
                // Rồi mới xóa Table[0]
                PhView.RowFilter = "stt_rec= '" + PhData.Rows[0]["stt_rec"].ToString() + "'";
                CtView.RowFilter = "stt_rec= '" + PhData.Rows[0]["stt_rec"].ToString() + "'";

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
                            SqlCommand cmd = new SqlCommand("[CACTPT1-Post];25");
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@Stt_rec", SqlDbType.NVarChar).Value = _stt_rec;
                            cmd.Parameters.Add("@Stt_rec_tt", SqlDbType.NVarChar).Value = dr["stt_rec_tt"];
                            cmd.Parameters.Add("@Ma_nt", SqlDbType.NVarChar).Value = _ma_nt;
                            cmd.Parameters.Add("@Tt_qd", SqlDbType.Decimal).Value = dr["tt_qd"];
                            cmd.Parameters.Add("@Tt_dn", SqlDbType.Decimal).Value = _ma_nt.Equals(StartUp.M_ma_nt0)? dr["tien_nt"]: dr["tien"];
                            cmd.Parameters.Add("@Tt_dn_nt", SqlDbType.Decimal).Value = dr["tien_nt"];

                            DataSet ds = SysO.ExcuteReader(cmd);
                        }
                    }
                    foreach (DataRow dr in rows)
                    {
                        CtData.Rows.Remove(dr);
                    }
                }

                //Refresh lại Table[0], Table[1]
                if (PhData.Rows.Count > 0)
                {
                    // iRow = 0;
                    iRow = iRow > PhData.Rows.Count - 1 ? iRow - 1 : iRow;
                    //load lại form theo stt_rec
                    PhView.RowFilter = "stt_rec= '" + Stt_rec + "'";
                    CtView.RowFilter = "stt_rec= '" + Stt_rec + "'";
                }
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
                StartUp.In();

            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        private void V_Copy()
        {
            if (SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, DateTime.Now.Date))
            {
                if (string.IsNullOrEmpty(PhView[0]["stt_rec"].ToString().Trim()))
                    return;
                currActionTask = ActionTask.Copy;
                FrmCACTBC1Copy _formcopy = new FrmCACTBC1Copy();
                _formcopy.Closed += new EventHandler(_formcopy_Closed);
                _formcopy.ShowDialog();
            }
        }

        void _formcopy_Closed(object sender, EventArgs e)
        {
            if ((sender as FrmCACTBC1Copy).isCopy == true)
            {
                string newSttRec = DataProvider.NewTrans(StartUp.SysObj, StartUp.Ma_ct, StartUp.Ws_Id);
                if (!string.IsNullOrEmpty(newSttRec))
                {
                    DsVitual = StartUp.DsTrans.Copy();
                    //txtMa_kh.IsFocus = true;
                    //Them moi dong trong Ph
                    DataRow NewRecord = PhData.NewRow();
                    //copy dữ liệu từ row được chọn copy cho row mới
                    NewRecord.ItemArray = PhData.Rows[iRow].ItemArray;
                    //gán lại stt_rec, ngày ct
                    NewRecord["stt_rec"] = newSttRec;
                    NewRecord["stt_rec_hd"] = "";
                    NewRecord["ngay_ct"] = FrmCACTBC1Copy.ngay_ct;
                    //NewRecord["ngay_lct"] = FrmCACTBC1Copy.ngay_ct;
                    NewRecord["ma_qs"] = GetDMQS(BindingSysObj, StartUp.Ma_ct, Convert.ToDateTime(NewRecord["ngay_ct"]),
                             StartUp.M_User_Id, NewRecord["ma_qs"].ToString().Trim());
                    if (NewRecord["ma_qs"].ToString().Trim() != "")
                        NewRecord["so_ct"] = GetNewSoct(StartUp.SysObj, NewRecord["ma_qs"].ToString());
                    else
                        NewRecord["so_ct"] = "";
                    NewRecord["so_cttmp"] = NewRecord["so_ct"];
                    NewRecord["so_ct_tt"] = "";
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

                    iOldRow = iRow;
                    iRow = PhData.Rows.Count - 1;
                    //load lại form
                    PhView.RowFilter = "stt_rec= '" + newSttRec + "'";
                    CtView.RowFilter = "stt_rec= '" + newSttRec + "'";


                    IsInEditMode.Value = true;

                    //IsVisibilityFieldsXamDataGrid
                    SetStatusVisibleField();
                    this.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new Action(() =>
                    {
                        txtMa_kh.IsFocus = true;
                    }
                    ));

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

            Stt_rec0 = Stt_rec0ct >= Stt_rec0ctgt ? Stt_rec0ct : Stt_rec0ctgt;
            Stt_rec0++;

            NewRecord["stt_rec0"] = string.Format("{0:000}", Stt_rec0);
            NewRecord["tien_nt"] = 0;
            NewRecord["tien"] = 0;
            NewRecord["tien_tt"] = 0;
            NewRecord["ty_giahtf2"] = 0; // CtView.Count > 0 ? CtView[CtView.Count - 1]["ty_gia_ht2"] : 1;
            NewRecord["ty_gia_ht2"] = 0;
            int count = CtView.Count;
            if (count > 0)
            {
                NewRecord["dien_giaii"] = CtView[count - 1].Row["dien_giaii"];
            }
            else
            {
                NewRecord["dien_giaii"] = PhView[0].Row["dien_giai"];
            }
            FreeCodeFieldLib.CarryFreeCodeFields(StartUp.SysObj, StartUp.Ma_ct, CtView, NewRecord, 1);

            CtData.Rows.Add(NewRecord);

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
                if (!cbMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
                {
                    _tien_nt = SmLib.SysFunc.Round(_tien_nt, Convert.ToInt16(StartUp.SysObj.GetSysVar("M_ROUND_NT")));
                }
                else
                {
                    _tien_nt = SmLib.SysFunc.Round(_tien_nt, Convert.ToInt16(StartUp.SysObj.GetSysVar("M_ROUND")));
                }
                record.Cells["tien_nt"].Value = _tien_nt;
                record.Cells["thue_nt"].Value = _tt_nt - _tien_nt;
                if (!cbMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
                {
                    _tien_nt0 = SmLib.SysFunc.Round(_tien_nt * _ty_gia, Convert.ToInt16(StartUp.SysObj.GetSysVar("M_ROUND")));
                    if (_tien_nt0 > 0)
                    {
                        Decimal _thue_nt0 = 0, _tt = 0, _t_Lech = 0, _tien_Lech = 0, _thue_lech = 0;
                        _thue_nt0 = SmLib.SysFunc.Round(_tien_nt0 * _thue_suat / 100, Convert.ToInt16(StartUp.SysObj.GetSysVar("M_ROUND")));

                        _tt = SmLib.SysFunc.Round(_tt_nt * _ty_gia, Convert.ToInt16(StartUp.SysObj.GetSysVar("M_ROUND")));
                        _t_Lech = (_tt - (_tien_nt0 + _thue_nt0));
                        _tien_Lech = SmLib.SysFunc.Round((_t_Lech / (1 + (_thue_suat / 100))), Convert.ToInt16(StartUp.SysObj.GetSysVar("M_ROUND")));
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
                _tt_nt = FNum.ToDec(record.Cells["tt_nt"].Value);
                thue_nt = FNum.ToDec(record.Cells["thue_nt"].Value);
                _tien_nt = _tt_nt - thue_nt;
                record.Cells["tien_nt"].Value = _tien_nt;
                if (!cbMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
                {
                    Decimal _Ty_gia = FNum.ToDec(txtTy_gia.nValue);

                    Decimal tien = SmLib.SysFunc.Round(_Ty_gia * _tien_nt, Convert.ToInt16(StartUp.SysObj.GetSysVar("M_ROUND")));
                    if (tien > 0)
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
                                        e.Cell.Record.Cells["ten_tk_i"].Value = txt.RowResult["ten_tk"];                                 
                                        e.Cell.Record.Cells["ten_tk_i2"].Value = txt.RowResult["ten_tk2"];
                                }
                                break;
                            }
                        case "ma_kh_i":
                            {
                                AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                if (txt.RowResult != null)
                                {
                                   
                                        e.Cell.Record.Cells["ten_kh_i"].Value = txt.RowResult["ten_kh"];
                                        e.Cell.Record.Cells["ten_kh_i2"].Value = txt.RowResult["ten_kh2"];
                                }
                                break;
                            }
                        case "tien_nt":
                            {
                                if (!ChkSuaTien.IsChecked.Value && !string.IsNullOrEmpty(e.Editor.Text))
                                {
                                    Decimal _Tien_nt = 0, _Ty_gia = 0, _Ty_gia_ht = 0;
                                    _Ty_gia = FNum.ToDec(txtTy_gia.nValue);
                                    _Ty_gia_ht = FNum.ToDec((e.Cell.Record.DataItem as DataRowView)["ty_gia_ht2"]);
                                    _Tien_nt = (e.Editor as NumericTextBox).nValue;

                                    Decimal _tien = SmLib.SysFunc.Round(_Ty_gia * _Tien_nt, Convert.ToInt16(StartUp.SysObj.GetSysVar("M_ROUND")));
                                    string ma_gd = PhView[0]["ma_gd"].ToString();
                                    string[] ma_gd_tggd = new string[]{"3", "4", "6", "7", "9"};

                                    switch (ma_gd)
                                    {
                                        case "2":
                                        case "3":
                                        case "4":
                                        case "5":
                                        case "6":
                                        case "7":
                                        case "8":
                                        case "9":
                                            {
                                                if (ma_gd_tggd.Contains(ma_gd))
                                                {
                                                    e.Cell.Record.Cells["ty_giahtf2"].Value = txtTy_gia.Value;
                                                    (e.Cell.Record.DataItem as DataRowView)["ty_gia_ht2"] = _Ty_gia_ht = _Ty_gia;
                                                }
                                                if (!PhView[0]["ma_nt"].ToString().ToUpper().Trim().Equals(StartUp.M_ma_nt0.ToUpper().Trim()))
                                                {
                                                   // if (_Ty_gia_ht * _Tien_nt != 0)
                                                        e.Cell.Record.Cells["tien"].Value = SmLib.SysFunc.Round(_Ty_gia_ht * _Tien_nt, Convert.ToInt16(StartUp.SysObj.GetSysVar("M_ROUND")));
                                                   // if (_Ty_gia * _Tien_nt != 0)
                                                        e.Cell.Record.Cells["tien_tt"].Value = SmLib.SysFunc.Round(_Ty_gia * _Tien_nt, Convert.ToInt16(StartUp.SysObj.GetSysVar("M_ROUND")));
                                                }
                                                else
                                                {
                                                    e.Cell.Record.Cells["tien"].Value = SmLib.SysFunc.Round(_Ty_gia * _Tien_nt, Convert.ToInt16(StartUp.SysObj.GetSysVar("M_ROUND")));
                                                    e.Cell.Record.Cells["tien_tt"].Value = SmLib.SysFunc.Round(_Ty_gia * _Tien_nt, Convert.ToInt16(StartUp.SysObj.GetSysVar("M_ROUND")));
                                                }
                                            }
                                            break;
                                        default:
                                            {
                                                //if (_Ty_gia * _Tien_nt != 0)
                                                    e.Cell.Record.Cells["tien"].Value = _tien;
                                            }
                                            break;
                                    }

                                }
                                UpdateTotalHT();
                            }
                            break;
                        case "ty_giahtf2":
                            {
                                Decimal _ty_gia_ht2 = FNum.ToDec((e.Cell.Record.DataItem as DataRowView)["ty_giahtf2"]); ;

                                _ty_gia_ht2 = (mLoai_tg == 1 ? _ty_gia_ht2 : (_ty_gia_ht2 == 0 ? 0 : SmLib.SysFunc.Round(1 / _ty_gia_ht2, M_ROUND_TY_GIA)));
                                (e.Cell.Record.DataItem as DataRowView)["ty_gia_ht2"] = _ty_gia_ht2;
                                
                                if (!ChkSuaTien.IsChecked.Value)
                                {
                                    if (!string.IsNullOrEmpty(e.Editor.Text) && PhView[0]["ma_gd"].ToString().IndexOfAny(new char[] { '2', '5' }) >= 0)
                                    {
                                        Decimal _Tien_nt = 0, _Ty_gia_ht2 = 0;
                                        _Tien_nt = FNum.ToDec(e.Cell.Record.Cells["tien_nt"].Value);
                                        _Ty_gia_ht2 = (e.Editor as ExRateTextBox).nValue;

                                        Decimal _tien = SmLib.SysFunc.Round(_Ty_gia_ht2 * _Tien_nt, Convert.ToInt16(StartUp.SysObj.GetSysVar("M_ROUND")));
                                       // if (_tien != 0)
                                            e.Cell.Record.Cells["tien"].Value = _tien;
                                    }
                                }
                                UpdateTotalHT();
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

                        case "tk_thue_i":
                            {
                                AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                //Cập nhật tài khoản thuế
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
                                if (txtTy_gia.Value != null && !string.IsNullOrEmpty(e.Editor.Text.Trim()))
                                {
                                    Decimal _tien_nt0 = 0;
                                    _tien_nt0 = FNum.ToDec(e.Editor.Value);
                                    //e.Cell.Record.Cells["tt"].Value = _tien_nt0 + _thue_nt0;
                                }
                                UpdateTotalHT();
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
                ExMessageBox.Show( 50,StartUp.SysObj, "Không có dữ liệu!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                currActionTask = ActionTask.Edit;

                DsVitual = new DataSet();
                DsVitual.Tables.Add(PhView.ToTable());
                DsVitual.Tables.Add(CtView.ToTable());


                IsInEditMode.Value = true;
                if (txtMa_gd.Text.Trim() != "1")
                    txtMa_kh.IsFocus = true;
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
                Voucher_Ma_nt0.Value = (PhView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
                SetStatusVisibleField();

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
                        cmd.CommandText = "Exec [CACTPT1-ExcludeVoucher] @Stt_rec, @Stt_rec_tt, @Ma_nt, @Tt_qd, @Tt_dn, @Tt_dn_nt";
                        cmd.Parameters.Add("@Stt_rec", SqlDbType.VarChar).Value = stt_rec;
                        cmd.Parameters.Add("@Stt_rec_tt", SqlDbType.VarChar).Value = stt_rec_tt;
                        cmd.Parameters.Add("@Ma_nt", SqlDbType.VarChar).Value = ma_nt;
                        cmd.Parameters.Add("@Tt_qd", SqlDbType.Decimal).Value = tt_qd;
                        cmd.Parameters.Add("@Tt_dn", SqlDbType.Decimal).Value = tt_dn;
                        cmd.Parameters.Add("@Tt_dn_nt", SqlDbType.Decimal).Value = tt_dn_nt;
                        cmd.Parameters.Add("@User_id", SqlDbType.Int).Value = user_id;
                        StartUp.SysObj.ExcuteNonQuery(cmd);

                        cmd = new SqlCommand();
                        cmd.CommandText = "EXEC Arttpb;70 @Stt_rec, @Ma_ct";
                        cmd.Parameters.Add("@Stt_rec", SqlDbType.VarChar).Value = stt_rec;
                        cmd.Parameters.Add("@Ma_ct", SqlDbType.VarChar).Value = ma_ct;
                        StartUp.SysObj.ExcuteNonQuery(cmd);
                    }
                }
            }
        }

        private void ChkSua_tggs_Click(object sender, RoutedEventArgs e)
        {
            IsCheckedSua_tggs.Value = ChkSua_tggs.IsChecked.Value;
        }

        private void ChkSuaTien_Click(object sender, RoutedEventArgs e)
        {
            IsCheckedSua_tien.Value = ChkSuaTien.IsChecked.Value;

        }

        private void V_Moi()
        {
            try
            {
                if (SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, DateTime.Now.Date))
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

                        //NewRecord["ma_nt"] = PhData.Rows.Count > 1 ? Ma_nt.ToString() : StartUp.DmctInfo["ma_nt"].ToString();
                        NewRecord["ma_gd"] = PhData.Rows.Count > 1 ? Ma_gd : StartUp.DmctInfo["ma_gd"].ToString();
                        NewRecord["tk"] = PhData.Rows.Count > 1 ? Tk : "";
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
                            NewRecord["ma_nt"] = Ma_nt;
                            NewRecord["ma_qs"] = GetDMQS(BindingSysObj, StartUp.Ma_ct, Convert.ToDateTime(NewRecord["ngay_ct"]),
                            StartUp.M_User_Id, PhData.Rows[iRow]["ma_qs"].ToString().Trim());
                        }
                        //NewRecord["ma_nt"] = string.IsNullOrEmpty(cbMa_nt.Text.Trim()) ? StartUp.M_ma_nt0 : cbMa_nt.Text;
                        if (NewRecord["ma_nt"].ToString().Trim().Equals(StartUp.M_ma_nt0.Trim()))
                        {
                            NewRecord["ty_giaf"] = 1;
                        }
                        else
                        {
                            NewRecord["ty_giaf"] = StartUp.GetRates(NewRecord["ma_nt"].ToString().Trim(), Convert.ToDateTime(NewRecord["ngay_ct"]).Date);
                        }

                        NewRecord["status"] = StartUp.DmctInfo["ma_post"];
                        NewRecord["t_tien_nt"] = 0;
                        NewRecord["t_tien"] = 0;
                        NewRecord["t_thue_nt"] = 0;
                        NewRecord["t_thue"] = 0;
                        NewRecord["t_tt_nt"] = 0;
                        NewRecord["t_tt"] = 0;
                        NewRecord["han_tt"] = 0;

                        //Them moi dong trong Ct
                        DataRow NewCtRecord = CtData.NewRow();
                        NewCtRecord["stt_rec"] = newSttRec;
                        NewCtRecord["stt_rec0"] = "001";
                        NewCtRecord["ma_ct"] = StartUp.Ma_ct;
                        NewCtRecord["ngay_ct"] = txtNgay_ct.Value == null ? DateTime.Now.Date : txtNgay_ct.dValue.Date;

                        NewCtRecord["tien_nt"] = 0;
                        NewCtRecord["tien"] = 0;
                        NewCtRecord["tien_tt"] = 0;
                        NewCtRecord["thue_nt"] = 0;
                        NewCtRecord["thue"] = 0;
                        NewCtRecord["ty_giahtf2"] = 0;
                        NewCtRecord["ty_gia_ht2"] = 0;

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
                        iOldRow = iRow;
                        iRow = PhData.Rows.Count - 1;

                        IsInEditMode.Value = true;

                        txtTen_kh.Text = "";
                        txtTenTK.Text = "";
                        TabInfo.SelectedIndex = 0;
                    }
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
            if (StartUp.SysObj.VersionInfo.Rows[0]["product_code"].ToString().Equals("FA") || (StartUp.dtRegInfo != null && !StartUp.dtRegInfo.Rows[18]["content"].ToString().Trim().Equals("FK")))
            {
                CatgLib.Catinhtg.Tinh(GrdCt.Records, this);
                CalculateTyGia();
            }
          // TinhLai_tien_tt();
            try
            {
                bool isError = false;
                if (!IsSequenceSave)
                {
                    if (IsInEditMode.Value)
                    {
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
                            ExMessageBox.Show(55, StartUp.SysObj, "Chưa vào chi tiết!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                            GrdCt.Focus();
                            return;
                        }

                        if (string.IsNullOrEmpty(PhView[0]["ma_kh"].ToString().Trim()))
                        {
                            ExMessageBox.Show(60, StartUp.SysObj, "Chưa có mã khách hàng!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtMa_kh.IsFocus = true;
                            isError = true;
                        }
                        else if (string.IsNullOrEmpty(PhView[0]["tk"].ToString().Trim()))
                        {
                            ExMessageBox.Show(65, StartUp.SysObj, "Chưa vào tài khoản nợ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtTk.IsFocus = true;
                            isError = true;
                        }
                        //Kiểm tra có ngày hạch toán hay chưa
                        else if (txtNgay_ct.dValue == new DateTime())
                        {
                            ExMessageBox.Show(70, StartUp.SysObj, "Chưa vào ngày hạch toán!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtNgay_ct.Focus();
                            isError = true;
                        }
                        else
                            if (StartUp.M_ngay_lct.Equals("1") && (txtngay_lct.Value == null || txtngay_lct.Value == DBNull.Value || txtngay_lct.dValue == new DateTime()))
                            {
                                ExMessageBox.Show(2012, StartUp.SysObj, "Chưa vào ngày lập chứng từ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                txtngay_lct.Focus();
                                isError = true;
                            }
                            else if ( StartUp.M_NGAY_BAT_DAU != null && (!txtNgay_ct.IsValueValid || txtNgay_ct.dValue < StartUp.M_NGAY_BAT_DAU || txtNgay_ct.dValue > StartUp.M_NGAY_KET_THUC))
                                {
                                    ExMessageBox.Show(1024, StartUp.SysObj, "Ngày hạch toán không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                    isError = true;
                                    txtNgay_ct.Focus();
                                }
                            else if (string.IsNullOrEmpty(CtView[0]["tk_i"].ToString().Trim())
                                && txtMa_gd.Text.Trim() != "1")
                            {
                                ExMessageBox.Show(75, StartUp.SysObj, "Chưa vào tài khoản có!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                TabInfo.SelectedIndex = 0;
                                GrdCt.ExecuteCommand(DataPresenterCommands.CellFirstOverall);
                                GrdCt.Focus();
                                isError = true;
                            }
                            else if (string.IsNullOrEmpty(CtView[0]["so_ct0"].ToString().Trim())
                                && txtMa_gd.Text.Trim() == "1")
                            {
                                ExMessageBox.Show(80, StartUp.SysObj, "Chưa vào chi tiết!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                TabInfo.SelectedIndex = 0;
                                Grdhd.ExecuteCommand(DataPresenterCommands.CellFirstOverall);
                                Grdhd.Focus();
                                isError = true;
                            }
                            else if (string.IsNullOrEmpty(PhView[0]["so_ct"].ToString().Trim()))
                            {
                                ExMessageBox.Show(85, StartUp.SysObj, "Chưa vào số chứng từ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                txtSo_ct.Focus();
                                isError = true;
                            }
                        //else if (CheckValidSoct(StartUp.SysObj, txtMa_qs.Text, txtSo_ct.Text, PhView[0]["stt_rec"].ToString()))
                        //{
                        //    if (StartUp.M_trung_so.Equals("1"))
                        //    {
                        //        if (ExMessageBox.Show( 90,StartUp.SysObj, "Có chứng từ trùng số. Số cuối cùng là: " + "[" + GetLastSoct(StartUp.SysObj, txtMa_qs.Text).Trim() + "]" + ". Có lưu chứng từ này không?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                        //        {
                        //            txtSo_ct.SelectAll();
                        //            txtSo_ct.Focus();
                        //            isError = true;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        ExMessageBox.Show( 95,StartUp.SysObj, "Đã có chứng từ trùng số. Số cuối cùng là: " + "[" + GetLastSoct(StartUp.SysObj + "]", txtMa_qs.Text).Trim(), "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        //        txtSo_ct.SelectAll();
                        //        txtSo_ct.Focus();
                        //        isError = true;
                        //    }
                        //}
                        //Kiểm tra mã số thuế và dữ liệu rỗng và các chi tiết có hợp lệ không
                        if (!isError)
                        {
                            if (CtView.Count > 0)
                            {
                                foreach (DataRowView drv in CtView)
                                {
                                    if (string.IsNullOrEmpty(drv.Row["tk_i"].ToString().Trim()))
                                    {
                                        CtData.Rows.Remove(drv.Row);
                                        CtData.AcceptChanges();
                                        continue;
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
                        //Đẩy wa tab thue đẻ lấy thông tin thuế chính xác trước khi lưu
                        Decimal _t_tien = 0;
                        Decimal _t_tien_nt0 = 0;

                        Decimal.TryParse(txtT_Tien_nt.nValue.ToString(), out _t_tien);
                        switch (TabInfo.SelectedIndex)
                        {
                            case 0:
                                //Tính tổng tiên bên tab thuế
                                //UpdateTotalThue();
                                Decimal _t_tien_hdt = 0;
                                Decimal _t_tien_nt0_hdt = 0;

                                Decimal.TryParse(txtT_Tien_nt.nValue.ToString(), out _t_tien_hdt);
                                if (_t_tien != _t_tien_hdt)
                                {
                                    ExMessageBox.Show(100, StartUp.SysObj, "Tổng tiền/ tiền ngoại tệ khác với tổng tiền/ tiền ngoại tệ trong các hóa đơn giá trị gia tăng!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                    isError = true;
                                }
                                else if (!cbMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
                                {
                                    if (_t_tien_nt0 != _t_tien_nt0_hdt)
                                    {
                                        ExMessageBox.Show(105, StartUp.SysObj, "Tổng tiền/ tiền ngoại tệ khác với tổng tiền/ tiền ngoại tệ trong các hóa đơn giá trị gia tăng!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                        isError = true;
                                    }
                                }
                                //Cập nhật lại tổng tiền tab  hạch toán
                                UpdateTotalHT();
                                break;
                        }
                    }
                    if (!isError)
                    {
                        if (!IsSequenceSave)
                        {
                            //Cân bằng trường tiền
                            Decimal _ty_gia = Convert.ToDecimal(txtTy_gia.nValue);
                            string _FieldTien = PhView[0]["ma_gd"].ToString().IndexOfAny(new char[] { '2', '5' }) >= 0 ? "tien_tt" : "tien";
                            if (!cbMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()) && GrdCt.Records.Count > 0 && _ty_gia != 0 && !ChkSuaTien.IsChecked.Value)
                            {
                                //Phiếu sai: ngoại tệ=0, vnd!=0
                                var v_so_phieu_sai = CtData.AsEnumerable()
                                                                   .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString() && (b.Field<decimal?>("tien_nt") == 0 || _ty_gia == 0) && b.Field<decimal?>("tien") != 0)
                                                                   .Count();
                                if (v_so_phieu_sai < GrdCt.Records.Count)
                                {
                                    //Tính tiền hàng ngoại tệ
                                    Decimal t_tien_nt = 0;
                                    var vtien_nt = CtData.AsEnumerable()
                                                    .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                                                    .Sum(x => x.Field<decimal?>("tien_nt"));
                                    if (vtien_nt != null)
                                        Decimal.TryParse(vtien_nt.ToString(), out t_tien_nt);

                                    //Tính tiền hàng vnd
                                    Decimal t_tien_ph = SmLib.SysFunc.Round(_ty_gia * t_tien_nt, Convert.ToInt16(StartUp.SysObj.GetSysVar("M_ROUND")));

                                    ////Gán số dư cho phiếu đầu tiên
                                    Decimal t_tien_ct = 0;
                                    var vtien_nt0 = CtData.AsEnumerable()
                                        .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                                        .Sum(x => x.Field<decimal?>(_FieldTien));

                                    if (vtien_nt0 != null)
                                        Decimal.TryParse(vtien_nt0.ToString(), out t_tien_ct);

                                    for (int i = 0; i < GrdCt.Records.Count; i++)
                                    {
                                        DataRecord dr = GrdCt.Records[i] as DataRecord;
                                        Decimal tien = 0, tien_nt = 0;
                                        tien = FNum.ToDec(dr.Cells[_FieldTien].Value);
                                        tien_nt = FNum.ToDec(dr.Cells["tien_nt"].Value);
                                        if (tien_nt != 0 && tien != 0)
                                        {
                                            dr.Cells[_FieldTien].Value = tien + t_tien_ph - t_tien_ct;
                                            PhView[0]["t_tien"] = t_tien_ph;
                                            if (CtData.AsEnumerable().All(x => isEquals(FNum.ToDec(x.Field<object>("ty_gia_ht2")), Ty_gia, x["stt_rec"].ToString())))
                                                dr.Cells["tien"].Value = tien + t_tien_ph - t_tien_ct;

                                            break;
                                        }

                                    }
                                }
                            }


                            // update thông tin cho các record Table0 (Ph)
                            if (string.IsNullOrEmpty(PhView[0]["ma_gd"].ToString()))
                                PhView[0]["ma_gd"] = StartUp.DmctInfo["ma_gd"];
                            if (string.IsNullOrEmpty(PhView[0]["ma_dvcs"].ToString()))
                                PhView[0]["ma_dvcs"] = StartUp.SysObj.GetOption("M_MA_DVCS").ToString();

                            if (string.IsNullOrEmpty(PhView[0]["loai_ct"].ToString().Trim()))
                            {
                                if (txtMa_gd.RowResult == null)
                                    txtMa_gd.SearchInit();
                                if (txtMa_gd.RowResult != null)
                                    PhView[0]["loai_ct"] = txtMa_gd.RowResult["loai_ct"];
                            }
                            if (txtT_Tien.nValue == 0 && txtT_Tien_nt.nValue == 0)
                            {
                                if (StartUp.M_CHK_ZERO == 1)
                                {
                                    ExMessageBox.Show(110, StartUp.SysObj, "Hạch toán tiền bằng 0!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else if (StartUp.M_CHK_ZERO == 2)
                                {
                                    ExMessageBox.Show(115, StartUp.SysObj, "Hạch toán tiền bằng 0, không lưu được!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                    isError = true;
                                }
                            }
                        }
                        if (!isError)
                        {
                            DataTable tbPhToSave = PhData.Clone();
                            tbPhToSave.Rows.Add(PhView[0].Row.ItemArray);
                            if (!IsSequenceSave)
                            {
                                tbPhToSave.Rows[0]["status"] = 0;
                            }
                            DataProvider.UpdateDataTable(StartUp.SysObj, StartUp.DmctInfo["m_phdbf"].ToString(), "stt_rec", tbPhToSave, "stt_rec;row_id");

                            DataProvider.DeleteRow(StartUp.SysObj, StartUp.DmctInfo["m_ctdbf"].ToString(), "stt_rec='" + PhView[0]["stt_rec"] + "'");

                            DataTable tbCtToSave = CtData.Clone();

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

                            if (!DataProvider.UpLoadDataTable(StartUp.SysObj, StartUp.DmctInfo["m_ctdbf"].ToString(), tbCtToSave))
                                return;

                            //StartUp.UpdateRates(tbPhToSave.Rows[0]["ma_nt"].ToString(), Convert.ToDateTime(txtNgay_ct.Value).Date, Convert.ToDecimal(txtTy_gia.Value)); 
                        }
                    }

                    #region kiểm tra dưới database
                    if (!isError)
                    {
                        if (!IsSequenceSave)
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
                                                    if (ExMessageBox.Show(120, StartUp.SysObj, "Có chứng từ trùng số. Số cuối cùng là: " + "[" + GetLastSoct(StartUp.SysObj, txtMa_qs.Text).Trim() + "]" + ". Có lưu chứng từ này không?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                                                    {
                                                        txtSo_ct.SelectAll();
                                                        txtSo_ct.Focus();
                                                        isError = true;
                                                    }
                                                }
                                                else if (StartUp.M_trung_so.Equals("2"))
                                                {
                                                    ExMessageBox.Show(125, StartUp.SysObj, "Đã có chứng từ trùng số. Số cuối cùng là: " + "[" + GetLastSoct(StartUp.SysObj, txtMa_qs.Text).Trim() + "]", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    txtSo_ct.SelectAll();
                                                    txtSo_ct.Focus();
                                                    isError = true;
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
                                    }
                                    dsCheckData.Tables[0].Rows.Remove(dv.Row);
                                }
                            }
                        }
                    }
                    #endregion

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
        }

        void Post()
        {
            SqlCommand cmd = new SqlCommand("EXEC [dbo].[CACTPT1-Post] @Stt_rec, @Ma_ct");
            cmd.Parameters.Add("@stt_rec", SqlDbType.Char, 11).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"];
            cmd.Parameters.Add("@Ma_ct", SqlDbType.Char, 3).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_ct"];
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
            DataTable PhViewTablev = PhData.Copy();
            PhViewTablev.Rows.RemoveAt(0);
            SmVoucherLib.FormView _frmView = new SmVoucherLib.FormView(StartUp.SysObj, PhViewTablev.DefaultView, CtView, StartUp.stringBrowse1, StartUp.stringBrowse2, "stt_rec");
            SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, _frmView.frmBrw.oBrowseCt, StartUp.Ma_ct, 1);
            _frmView.frmBrw.Title = SmLib.SysFunc.Cat_Dau(M_LAN.Equals("V") ? StartUp.CommandInfo["bar"].ToString() : StartUp.CommandInfo["bar2"].ToString());
            _frmView.frmBrw.ShowInTaskbar = false;
            _frmView.ListFieldSum = "t_tien_nt;t_tien";

            _frmView.frmBrw.LanguageID  = "CACTBC1_4";
            _frmView.ShowDialog();

            // Set lai irow va rowfilter ...
            if (_frmView.DataGrid.ActiveRecord != null)
            {

                int select_irow = (_frmView.DataGrid.ActiveRecord as DataRecord).Index;
                if (select_irow >= 0)
                {
                    string selected_stt_rec = (_frmView.DataGrid.DataSource as DataView)[select_irow]["stt_rec"].ToString();
                    FrmCACTBC1.iRow = select_irow + 1;
                    PhView.RowFilter = "stt_rec= '" + selected_stt_rec + "'";
                    CtView.RowFilter = "stt_rec= '" + selected_stt_rec + "'";

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
                Voucher_Ma_nt0.Text = PhView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (PhView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
            }
            catch (Exception ex)
            {

                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        private void cbMa_nt_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Voucher_Ma_nt0 == null)
                return;
            //Voucher_Ma_nt0 = (CodeValueBindingObject)FormMain.FindResource("Voucher_Ma_nt0");
            if (cbMa_nt.IsDataChanged)
            {
                StartUp.DsTrans.Tables[0].DefaultView[0]["loai_tg"] = cbMa_nt.RowResult["loai_tg"];
                Loai_tg.Text = cbMa_nt.RowResult["loai_tg"].ToString();

                Voucher_Ma_nt0.Text = PhView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (PhView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
                SetStatusVisibleField();
                if ((cbMa_nt.RowResult)["ma_nt"].ToString().Trim().Equals(StartUp.M_ma_nt0.Trim()))
                {
                    txtTy_gia.Value = 1;
                }
                else
                {
                    txtTy_gia.Value = StartUp.GetRates((cbMa_nt.RowResult)["ma_nt"].ToString().Trim(), Convert.ToDateTime(txtNgay_ct.Value).Date);
                }
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

        void SetStatusVisibleField()
        {
            ChangeLanguage();
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
                if (!string.IsNullOrEmpty(txtMa_kh.RowResult["dia_chi"].ToString().Trim()))
                    txtDia_chi.Text = txtMa_kh.RowResult["dia_chi"].ToString();

                if (StartUp.DsTrans.Tables[1].DefaultView.Count > 0)
                {
                    if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[0]["tk_i"].ToString().Trim()))
                    {
                        StartUp.DsTrans.Tables[1].DefaultView[0]["tk_i"] = txtMa_kh.RowResult["tk"].ToString();
                        DataSet ds = StartUp.SysObj.ExcuteReader(new SqlCommand(string.Format("SELECT ten_tk, ten_tk2 FROM dmtk WHERE tk = '{0}'", txtMa_kh.RowResult["tk"].ToString().Trim())));
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            if (StartUp.DsTrans.Tables[1].Columns.Contains("ten_tk_i"))
                                StartUp.DsTrans.Tables[1].DefaultView[0]["ten_tk_i"] = ds.Tables[0].Rows[0]["ten_tk"];
                            if (StartUp.DsTrans.Tables[1].Columns.Contains("ten_tk_i2"))
                                StartUp.DsTrans.Tables[1].DefaultView[0]["ten_tk_i2"] = ds.Tables[0].Rows[0]["ten_tk2"];
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
                if (CtView.Count > 0)
                    CtView[0]["tk_i"] = string.IsNullOrEmpty(CtView[0]["tk_i"].ToString()) ? txtMa_kh.RowResult["tk"].ToString() : CtView[0]["tk_i"];
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

        private void txttk_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTk.Text.Trim()) && !txtTk.IsReadOnly && txtTk.RowResult != null)
            {
                if (M_LAN.ToUpper().Equals("V"))
                    txtTenTK.Text = txtTk.RowResult["ten_nx"].ToString();
                else
                    txtTenTK.Text = txtTk.RowResult["ten_nx2"].ToString();
            }
            PhView[0]["ten_nx"] = txtTk.RowResult["ten_nx"].ToString();
            PhView[0]["ten_nx2"] = txtTk.RowResult["ten_nx2"].ToString();
            //LoadDataDu13();
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
                    GrdCt.ExecuteCommand(DataPresenterCommands.StartEditMode);
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
                        CatgLib.Catinhtg.Tinh(GrdCt.Records, this);
                    }
                    break;
                case Key.F8:
                    {
                        if (ExMessageBox.Show( 130,StartUp.SysObj, "Có xóa dòng ghi hiện thời không?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
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

        private void UpdateTotalHT()
        {
            try
            {
                if (currActionTask == ActionTask.View)
                    return;

                CtData.AcceptChanges();
                //Cập nhật tổng thanh toán nguyên tệ
                Decimal _t_tien_nt = 0, _t_tien = 0;

                var vtien = CtData.AsEnumerable()
                    .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
                    .Select(x => new { tien_nt = x.Field<decimal?>("tien_nt"), tien = x.Field<decimal?>("tien"), tien_tt = x.Field<decimal?>("tien_tt") });
                if (vtien != null)
                {
                    if ((PhView[0]["ma_gd"].ToString() == "2" || PhView[0]["ma_gd"].ToString() == "5") && PhView[0]["ma_nt"].ToString() != StartUp.M_ma_nt0)
                    {
                        Decimal.TryParse(vtien.Sum(p => p.tien_nt).ToString(), out _t_tien_nt);
                        Decimal.TryParse(vtien.Sum(p => p.tien_tt).ToString(), out _t_tien);
                    }
                    else
                    {
                        Decimal.TryParse(vtien.Sum(p => p.tien_nt).ToString(), out _t_tien_nt);
                        Decimal.TryParse(vtien.Sum(p => p.tien).ToString(), out _t_tien);
                    }

                    PhView[0]["t_tien_nt"] = _t_tien_nt;
                    PhView[0]["t_tien"] = _t_tien;
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        //private void UpdateTotalThue()
        //{
        //    try
        //    {
        //        if (currActionTask == ActionTask.View || GrdCtgt.Records.Count == 0)
        //            return;
        //        //Cập nhật tổng thanh toán nguyên tệ
        //        Decimal _t_tien = 0, _t_thue = 0;

        //        //Tính tiền
        //        if (vtien != null)
        //            Decimal.TryParse(vtien.ToString(), out _t_tien);

        //        txtT_Tien.Value = _t_tien;
        //        txtT_Tien_nt.Value = _t_tien;

        //        //Tính thuế
        //        var vthue = StartUp.DsTrans.Tables[2].AsEnumerable()
        //                        .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
        //                        .Sum(x => x.Field<decimal?>("t_thue_nt"));
        //        if (vthue != null)
        //            Decimal.TryParse(vthue.ToString(), out _t_thue);

        //        txtT_thue.Value = _t_thue;
        //        txtT_thue_nt.Value = _t_thue;
        //        //Tính tổng thanh toán
        //        txtT_tt.Value = _t_tien + _t_thue;
        //        txtT_tt_nt.Value = _t_tien + _t_thue;
        //        //Cập nhật tổng thanh toán cho tien0
        //        if (!cbMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
        //        {
        //            //tiền nt0
        //            Decimal _sum_tien_nt0 = 0;
        //            var vtien_nt0 = StartUp.DsTrans.Tables[2].AsEnumerable()
        //                .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
        //                .Sum(x => x.Field<decimal?>("t_tien"));
        //            if (vtien_nt0 != null)
        //                Decimal.TryParse(vtien_nt0.ToString(), out _sum_tien_nt0);
        //            txtT_Tien_Nt0.Value = _sum_tien_nt0;


        //            //thuế nt0
        //            Decimal _sum_thue_nt0 = 0;
        //            var vthue_nt0 = StartUp.DsTrans.Tables[2].AsEnumerable()
        //                        .Where(b => b.Field<string>("stt_rec") == PhView[0]["stt_rec"].ToString())
        //                        .Sum(x => x.Field<decimal?>("t_thue"));
        //            if (vthue_nt0 != null)
        //                Decimal.TryParse(vthue_nt0.ToString(), out _sum_thue_nt0);

        //            txtT_thue_Nt0.Value = _sum_thue_nt0;
        //            //Tính tổng thanh toán

        //            txtT_tt_Nt0.Value = _sum_tien_nt0 + _sum_thue_nt0;

        //        }
        //        else
        //        {
        //            txtT_Tien_Nt0.Value = _t_tien;
        //            txtT_thue_Nt0.Value = _t_thue;
        //            txtT_tt_Nt0.Value = _t_tien + _t_thue;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SmErrorLib.ErrorLog.CatchMessage(ex);
        //    }
        //}
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

        private void CalculateTyGia()
        {
            Decimal _ty_gia = 0;
            _ty_gia = txtTy_gia.nValue;


            if (Sua_tien == 1)
            {
                if (Ma_GD_Value.Text.Trim().ToString().IndexOfAny(new char[] { '2', '3', '4', '5', '6', '7','8','9' }) >= 0)
                {
                    //if (PhView[0]["ma_nt"].ToString().ToUpper().Trim().Equals(M_Ma_nt0.ToUpper().Trim()))
                    //{
                    //    for (int i = 0; i < GrdCt.Records.Count; i++)
                    //    {
                    //        (GrdCt.Records[i] as DataRecord).Cells["ty_giahtf2"].Value = txtTy_gia.Value;
                    //        ((GrdCt.Records[i] as DataRecord).DataItem as DataRowView)["ty_gia_ht2"] = (mLoai_tg == 1 ? _ty_gia : (_ty_gia == 0 ? 0 : SmLib.SysFunc.Round(1 / _ty_gia, M_ROUND_TY_GIA))); ; ;
                    //        ((GrdCt.Records[i] as DataRecord).DataItem as DataRowView)["tien_tt"] =
                    //            ((GrdCt.Records[i] as DataRecord).DataItem as DataRowView)["tien"] =
                    //            ((GrdCt.Records[i] as DataRecord).DataItem as DataRowView)["tien_nt"];
                    //    }
                    //}
                    if (PhView[0]["ma_nt"].ToString().ToUpper().Trim().Equals(M_Ma_nt0.ToUpper().Trim()))
                    {
                        for (int i = 0; i < GrdCt.Records.Count; i++)
                        {
                            (GrdCt.Records[i] as DataRecord).Cells["ty_giahtf2"].Value = txtTy_gia.Value;
                            ((GrdCt.Records[i] as DataRecord).DataItem as DataRowView)["ty_gia_ht2"] = (mLoai_tg == 1 ? _ty_gia : (_ty_gia == 0 ? 0 : SmLib.SysFunc.Round(1 / _ty_gia, M_ROUND_TY_GIA))); ; ;
                            ((GrdCt.Records[i] as DataRecord).DataItem as DataRowView)["tien_tt"] =
                            ((GrdCt.Records[i] as DataRecord).DataItem as DataRowView)["tien"] =
                            ((GrdCt.Records[i] as DataRecord).DataItem as DataRowView)["tien_nt"];
                            //CalculateHd2(GrdCt.Records[i] as DataRecord);
                        }
                    }
                    else
                    {
                        //for (int i = 0; i < GrdCt.Records.Count; i++)
                        //{
                        //    (GrdCt.Records[i] as DataRecord).Cells["ty_giahtf2"].Value = txtTy_gia.Value;
                        //    ((GrdCt.Records[i] as DataRecord).DataItem as DataRowView)["ty_gia_ht2"] = (mLoai_tg == 1 ? _ty_gia : (_ty_gia == 0 ? 0 : SmLib.SysFunc.Round(1 / _ty_gia, M_ROUND_TY_GIA))); ; ;
                        //    ((GrdCt.Records[i] as DataRecord).DataItem as DataRowView)["tien_tt"] =
                        //    ((GrdCt.Records[i] as DataRecord).DataItem as DataRowView)["tien"] =
                        //    SmLib.SysFunc.Round(Convert.ToDecimal(((GrdCt.Records[i] as DataRecord).DataItem as DataRowView)["tien_nt"]) * System.Convert.ToDecimal(txtTy_gia.Value), M_Round);
                        //    //CalculateHd2(GrdCt.Records[i] as DataRecord);
                        //}
                    }
                }
                
                UpdateTotalHT();
                return;
            }
           // if (txtTy_gia.nValue != 0)
            {
                //Hạch toán
           
                DataRecord rec;
                for (int i = 0; i < GrdCt.Records.Count; i++)
                {
                    rec = GrdCt.Records[i] as DataRecord;
                    //Gán tỷ giá gs
                    decimal ty_gia_ht2 = 0;
                    if(((rec.Cells.Record as DataRecord).DataItem as DataRowView)["ty_gia_ht2"] != null)
                    decimal.TryParse(((rec.Cells.Record as DataRecord).DataItem as DataRowView)["ty_gia_ht2"].ToString(), out ty_gia_ht2);
                    //if (ty_gia_ht2 == 0)
                    //    rec.Cells["ty_gia_ht2"].Value = ty_gia_ht2 = txtTy_gia.nValue;

                    //Tính tiền
                    Decimal _tien_nt = 0, _tien_nt0 = 0;
                    _tien_nt = Convert.ToDecimal(rec.Cells["tien_nt"].Value);
                    //if (_ty_gia * _tien_nt > 0)
                    {
                        _tien_nt0 = SmLib.SysFunc.Round(_ty_gia * _tien_nt, Convert.ToInt16(StartUp.M_ROUND));
                        if (!PhView[0]["ma_nt"].ToString().ToUpper().Trim().Equals(StartUp.M_ma_nt0.ToUpper().Trim()))
                        {
                            //135988012
                            rec.Cells["tien_tt"].Value = _tien_nt0;
                            if (PhView[0]["ma_gd"].ToString().IndexOfAny(new char[] { '2', '5' }) >= 0)
                            {                                
                                //if(ty_gia_ht2 * _tien_nt > 0)
                                rec.Cells["tien"].Value = SmLib.SysFunc.Round(ty_gia_ht2 * _tien_nt, StartUp.M_ROUND);
                            }
                        }
                        else
                        {
                            if (mLoai_tg != 1)
                            {
                                (GrdCt.Records[i] as DataRecord).Cells["ty_giahtf2"].Value = txtTy_gia.Value;
                                Decimal _ty_gia_ht2 = txtTy_gia.nValue;

                                _ty_gia_ht2 = (_ty_gia_ht2 == 0 ? 0 : SmLib.SysFunc.Round(1 / _ty_gia_ht2, M_ROUND_TY_GIA));
                                ((GrdCt.Records[i] as DataRecord).DataItem as DataRowView)["ty_gia_ht2"] = _ty_gia_ht2;
                            }

                            (GrdCt.Records[i] as DataRecord).Cells["tien_tt"].Value = _tien_nt0;
                            (GrdCt.Records[i] as DataRecord).Cells["tien"].Value = _tien_nt0;
                        }
                        if (PhView[0]["ma_gd"].ToString().IndexOfAny(new char[] { '2', '5' }) < 0)
                            rec.Cells["tien"].Value = _tien_nt0;
                    }
                }
                UpdateTotalHT();
            }
        }

        private void txtNgay_ct_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtNgay_ct.Value == DBNull.Value)
                txtNgay_ct.Value = DateTime.Now;
            if (!txtNgay_ct.IsFocusWithin)
            {                
                if ((currActionTask == ActionTask.Add || currActionTask == ActionTask.Edit || currActionTask == ActionTask.Copy))
                {
                    if ((StartUp.M_ngay_lct.Equals("0") || txtngay_lct.dValue == new DateTime()) && txtNgay_ct.dValue != new DateTime())
                        txtngay_lct.Value = txtNgay_ct.dValue.Date;
                }
            }
        }


        private void GrdCt_RecordDelete(object sender, Infragistics.Windows.DataPresenter.Events.RecordsDeletedEventArgs e)
        {
            if (txtMa_gd.Text.Trim().Equals("4"))
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
                {
                    txtHan_tt.Focus();
                }));
            }
            else
            {
                SmLib.WinAPISenkey.SenKey(ModifierKeys.Alt, Key.D2);
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    (this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus(); ;
                }));
            }
            //if (GrdCtgt.Records.Count > 0)
            //{
            //    GrdCtgt.ActiveCell = (GrdCtgt.Records[0] as DataRecord).Cells[0];
            //    GrdCtgt.Focus();
            //}
        }

        private void FormMain_Closed(object sender, EventArgs e)
        {
            if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
                App.Current.Shutdown();
        }

        #region GetLanguageString
        public override string GetLanguageString(string code, string language)
        {
            string sReturn = "";
           // if (language == "V")
            {
                switch (code)
                {
                    case "M_MA_NT":

                        if (!PhView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0))
                            sReturn = PhView[0]["ma_nt"].ToString();
                        break;
                    case "M_MA_NT0":
                        sReturn = StartUp.M_ma_nt0;
                        break;
                    case "COLTIEN":
                        //if (Ma_GD_Value.Text.IndexOfAny(new char[] { '2', '5' }) < 0)
                        //{
                        //    sReturn = "Phát sinh có";
                        //    GrdCt.FieldLayouts[0].Fields["tien_tt"].Label = "Tiền ht";
                        //}
                        //else
                        //{
                        //    GrdCt.FieldLayouts[0].Fields["tien"].Label = "Phát sinh có";
                        //}
                        break;
                }

            }
            return sReturn;
        }
        #endregion

        private void TabInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsInEditMode != null)
                if (IsInEditMode.Value)
                {
                    if (TabInfo.SelectedIndex == 0)
                    {
                        UpdateTotalHT();
                    }
                    else if (TabInfo.SelectedIndex == 1)
                    {
                        ChkSuaThue_Click(null, null);

                        //UpdateTotalThue();
                    }
                }
        }

        private void ChkSuaThue_Click(object sender, RoutedEventArgs e)
        {
            IsInEditModeThue.Value = false;
        }
        private void ChkSuaTien_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ChkSuaTien.IsChecked == false && sender.GetType().Name.Equals("CheckBox"))
            {
                CalculateTyGia();
            }
        }
        private void FormMain_EditModeEnded(object sender, string menuItemName, RoutedEventArgs e)
        {
            ChkSuaTien_Click(sender, e);

            Voucher_Ma_nt0.Text = PhView[0]["ma_nt"].ToString();
            Voucher_Ma_nt0.Value = (PhView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
            Loai_tg.Text = PhView[0]["loai_tg"].ToString();

            Ma_GD_Value.Text = txtMa_gd.Text;
            if (txtMa_gd.Text.Equals("9"))
                Ma_GD_Value.Value = true;
            else
                Ma_GD_Value.Value = false;

            IsInEditModeThue.Value = false;
            //LoadDataDu13();
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
                        ExMessageBox.Show( 135,StartUp.SysObj, "Ngày lập chứng từ khác với ngày hạch toán!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
        }

        private void txtMa_gd_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtMa_gd.RowResult == null || string.IsNullOrEmpty(txtMa_gd.Text.Trim()))
                return;
            Ma_GD_Value.Text = txtMa_gd.Text;
            if (M_LAN.ToUpper().Equals("V"))
                txtTen_gd.Text = txtMa_gd.RowResult["ten_gd"].ToString();
            else
                txtTen_gd.Text = txtMa_gd.RowResult["ten_gd2"].ToString();
            Ma_GD_Value.Text = txtMa_gd.Text;
            if (txtMa_gd.Text.Equals("9"))
                Ma_GD_Value.Value = true;
            else
                Ma_GD_Value.Value = false;
            if (!txtMa_gd.Text.Trim().Equals("4"))
            {
                txtHan_tt.Value = 0;
            }
            SetStatusVisibleField();
        }

        #region GrdCt_PreviewGotKeyboardFocus
        private void GrdCt_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (currActionTask == ActionTask.Add)
            {
                if (CtView.Count == 1 && string.IsNullOrEmpty(CtView[0]["dien_giaii"].ToString().Trim()))
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
                ExMessageBox.Show( 140,StartUp.SysObj, "Phải lưu chứng từ rồi mới phân bổ cho các hóa đơn!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (!PhData.DefaultView[0]["status"].ToString().Equals("2"))
            {
                ExMessageBox.Show( 145,StartUp.SysObj, "Phải ghi vào sổ cái rồi mới phân bổ cho hóa đơn!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                object ngay = Ngay_ct;
                PbInfo pb = new PbInfo(ngay, ngay, "", "", "");
                pb.TitleView = "Phan bo";
                string[] paras = StartUp.CommandInfo["parameter"].ToString().Split(new char[] { ';' });
                Arttpb.StartUp.Procedure = paras;
                (new Arttpb.StartUp()).Pb_tt(Stt_rec, pb);
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
                                        e.Cell.Record.Cells["ten_tk_i"].Value = txt.RowResult["ten_tk"];
                                    else
                                        e.Cell.Record.Cells["ten_tk_i2"].Value = txt.RowResult["ten_tk2"];
                                }
                                break;
                            }
                        case "tien_nt":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (txtTy_gia.Value != null && !string.IsNullOrEmpty(e.Editor.Text.Trim()))
                                    {
                                        decimal _Ty_gia, _Tien_nt, Tien = 0;
                                        _Ty_gia = txtTy_gia.nValue;
                                        _Tien_nt = (e.Editor as NumericTextBox).nValue;
                                        Tien = SmLib.SysFunc.Round(_Ty_gia * _Tien_nt, Convert.ToInt16(StartUp.M_ROUND));
                                        if (!PhView[0]["ma_nt"].ToString().ToUpper().Trim().Equals(M_Ma_nt0.ToUpper().Trim()))
                                        {
                                            if (Tien != 0)
                                                e.Cell.Record.Cells["tien"].Value = Tien;
                                        }
                                        else
                                            e.Cell.Record.Cells["tien"].Value = Tien;
                                    }
                                    if (Voucher_Ma_nt0.Text.Equals(e.Cell.Record.Cells["ma_nt_i"].Value.ToString()))
                                        e.Cell.Record.Cells["tt_qd"].Value = e.Editor.Value;
                                    else
                                        e.Cell.Record.Cells["tt_qd"].Value = 0;
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
                        case "tien":
                            {
                                if (e.Cell.IsDataChanged)
                                    UpdateTotalHT();
                            }
                            break;
                        case "so_ct0":
                            {
                                if ((e.Editor.Value is DBNull || string.IsNullOrEmpty(e.Editor.Value.ToString().Trim())) && ExMessageBox.Show(150, StartUp.SysObj, "Có nhập tiếp không?", StartUp.SysObj.GetSysVar("M_FAST_VER").ToString(), MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
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
                                
                                FrmCACTBC1DSHD frmDSHD = new FrmCACTBC1DSHD();
                                frmDSHD.ShowHd(e.Editor.Value.ToString());

                                DataRecord dr = frmDSHD.grdDSHD.ActiveRecord as DataRecord;
                                if (dr != null && dr.RecordType == RecordType.DataRecord)
                                {

                                    decimal t_tt_qd = 0;
                                    string stt_rec_hd = string.Empty;

                                    t_tt_qd = FNum.ToDec(dr.Cells["t_tt_qd"].Value);
                                    stt_rec_hd = dr.Cells["stt_rec"].Value.ToString();

                                    if (currActionTask == ActionTask.Edit)
                                    {
                                        SqlCommand cmd = new SqlCommand("Exec [CACTPT1-UpdateDaTt] @stt_rec, @stt_rec_hd");
                                        cmd.Parameters.Add("@stt_rec", SqlDbType.VarChar, 50).Value = PhView[0]["stt_rec"].ToString();
                                        cmd.Parameters.Add("@stt_rec_hd", SqlDbType.VarChar, 50).Value = stt_rec_hd;
                                        t_tt_qd = FNum.ToDec(StartUp.SysObj.ExcuteScalar(cmd));
                                    }

                                    rec.Cells["so_ct0"].Value = dr.Cells["so_ct"].Value.ToString();
                                    rec.Cells["ngay_ct0"].Value = Convert.ToDateTime(dr.Cells["ngay_ct"].Value.ToString());
                                    rec.Cells["tk_i"].Value = dr.Cells["tk"].Value.ToString();
                                    rec.Cells["ma_nt_i"].Value = dr.Cells["ma_nt"].Value.ToString();
                                    rec.Cells["t_tt_nt0"].Value = dr.Cells["tc_tt"].Value;
                                    rec.Cells["t_tt_qd"].Value = t_tt_qd;
                                    ((rec.Cells.Record as DataRecord).DataItem as DataRowView)["ty_giahtf2"] = ((dr.Cells.Record as DataRecord).DataItem as DataRowView)["ty_giaf"];
                                    ((rec.Cells.Record as DataRecord).DataItem as DataRowView)["ty_gia_ht2"] = ((dr.Cells.Record as DataRecord).DataItem as DataRowView)["ty_gia"];
                                    //rec.Cells["phai_tt_nt"].Value = dr.Cells["cl_tt"].Value;
                                    rec.Cells["phai_tt_nt"].Value = FNum.ToDec(rec.Cells["t_tt_nt0"].Value) - FNum.ToDec(rec.Cells["t_tt_qd"].Value);
                                    rec.Cells["stt_rec_tt"].Value = stt_rec_hd;

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
                                    // Grdhd.PreviewKeyUp += (a, b) => { b.Handled = true; };
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
                if (CtView.Count == 1 && string.IsNullOrEmpty(CtView[0]["dien_giaii"].ToString().Trim()))
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
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                (this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus(); ;
            }));
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
                    NewRowCt();
                    Grdhd.ActiveRecord = Grdhd.Records[Grdhd.Records.Count - 1];
                    if (txtMa_gd.Text == "1")
                        Grdhd.ActiveCell = (Grdhd.ActiveRecord as DataRecord).Cells["so_ct0"];
                    else
                        Grdhd.ActiveCell = (Grdhd.ActiveRecord as DataRecord).Cells["tk_i"];
                    break;
                case Key.F5:
                    if (StartUp.SysObj.VersionInfo.Rows[0]["product_code"].ToString().Equals("FA") || (StartUp.dtRegInfo != null && !StartUp.dtRegInfo.Rows[18]["content"].ToString().Trim().Equals("FK")))
                    {
                        CatgLib.Catinhtg.Tinh(Grdhd.Records, this);
                    }
                    break;
                case Key.F8:
                    {
                        if (ExMessageBox.Show( 155,StartUp.SysObj, "Có xóa dòng ghi hiện thời không?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
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

        private void txtHan_tt_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!txtHan_tt.IsReadOnly)
            {
                txtHan_tt.SelectAll();
            }
        }

        private void txtHan_tt_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Tab) || Keyboard.IsKeyDown(Key.Enter))
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    (this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus(); ;
                }));
            }
        }



    }
}
