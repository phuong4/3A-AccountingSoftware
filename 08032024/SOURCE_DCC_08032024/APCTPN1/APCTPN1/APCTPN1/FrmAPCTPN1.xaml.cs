using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Infragistics.Windows.DataPresenter;
using Sm.Windows.Controls;
using SmDefine;
using SmVoucherLib;
using System.Threading;
using System.Data.SqlClient;

namespace APCTPN1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FrmAPCTPN1 : SmVoucherLib.FormTrans
    {
        public static int iRow = 0;
        private int iOldRow = 0;
        public static CodeValueBindingObject IsInEditMode;

        CodeValueBindingObject Voucher_Ma_nt0;
        CodeValueBindingObject IsCheckedSua_tien;
        CodeValueBindingObject Ty_Gia_ValueChange;
        CodeValueBindingObject Voucher_Lan0;
        DataSet dsCheckData;
        //Lưu lại dữ liệu khi thêm sửa
        private DataSet DsVitual;
        public FrmAPCTPN1()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(FormTrans_Loaded);
            this.BindingSysObj = StartUp.SysObj;
            //this.IsEdit = false;
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
                currActionTask = ActionTask.View;
                if (StartUp.DsTrans.Tables[0].Rows.Count > 1)
                    iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;

                Debug.WriteLine(string.Format("#5: {0}", DateTime.Now.ToString()));

                IsInEditMode = (CodeValueBindingObject)FormMain.FindResource("IsInEditMode");
                Voucher_Ma_nt0 = (CodeValueBindingObject)FormMain.FindResource("Voucher_Ma_nt0");
                IsCheckedSua_tien = (CodeValueBindingObject)FormMain.FindResource("IsCheckedSua_tien");
                Ty_Gia_ValueChange = (CodeValueBindingObject)FormMain.FindResource("Ty_Gia_ValueChange");
                Voucher_Lan0 = (CodeValueBindingObject)FormMain.FindResource("Voucher_Lan0");

                string M_CDKH13 = SysO.GetOption("M_CDKH13").ToString().Trim();
                if (M_CDKH13 != "1")
                    txtSoDuKH.Visibility = tblSoDuKH.Visibility = Visibility.Hidden;

                Binding bind = new Binding("Value");
                bind.Source = IsInEditMode;
                bind.Mode = BindingMode.OneWay;
                this.SetBinding(FormTrans.IsEditModeProperty, bind);

                //Gán ngôn ngữ messagebox
                M_LAN = StartUp.SysObj.GetOption("M_LAN").ToString();
                GrdCt.Lan = M_LAN;
                GrdCtgt.Lan = M_LAN;
                //Them cac truong tu do
                SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, GrdCt, StartUp.Ma_ct, 1);
                SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, GrdCtgt, StartUp.Ma_ct, 2);

                Debug.WriteLine(string.Format("#6: {0}", DateTime.Now.ToString()));

                if (StartUp.DsTrans.Tables[0].Rows.Count > 0)
                {
                    StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
                    StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
                    StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";

                    this.GrdLayout00.DataContext = StartUp.DsTrans.Tables[0].DefaultView;
                   
                    this.GrdCt.DataSource = StartUp.DsTrans.Tables[1].DefaultView;
                    this.GrdCtgt.DataSource = StartUp.DsTrans.Tables[2].DefaultView;

                    txtStatus.ItemsSource = StartUp.tbStatus.DefaultView;

                    if (StartUp.tbStatus.DefaultView.Count == 1)
                    {
                        txtStatus.IsEnabled = false;
                    }
                    //Xét lại các Field khi thay đổi record hiển thị
                    StartUp.DsTrans.Tables[1].DefaultView.ListChanged += new System.ComponentModel.ListChangedEventHandler(DefaultView_ListChanged);

                    IsCheckedSua_tien.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"].ToString() == "1");
                    Ty_Gia_ValueChange.Value = true;
                }

                Debug.WriteLine(string.Format("#7: {0}", DateTime.Now.ToString()));

                Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
                Voucher_Lan0.Value = M_LAN.Equals("V");

                Debug.WriteLine(string.Format("#8: {0}", DateTime.Now.ToString()));
                SetStatusVisibleField();
                Debug.WriteLine(string.Format("#9: {0}", DateTime.Now.ToString()));
                LoadDataDu13();
                ////Sửa lỗi binding numerictextbox format sai lần đâu tiên khi load form
                //SmLib.WinAPISenkey.SenKey(ModifierKeys.None, Key.End);
                SetFocusToolbar();
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        void DefaultView_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            SetStatusVisibleField();
        }
        #endregion
 
        private void FormMain_EditModeEnded(object sender, string menuItemName, RoutedEventArgs e)
        {
            Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
            Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));

            ChkSuaTien_Click(sender, e);
            if (!menuItemName.Equals("btnSave")) 
                LoadDataDu13();

        }
        private void V_Truoc()
        {
            if (iRow > 1)
            {
                iRow--;        
                StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
                StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
                StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
               
                Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
            }
        }

        private void V_Sau()
        {
            if (iRow < StartUp.DsTrans.Tables[0].Rows.Count - 1)
            {
                iRow++;
                StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
                StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
                StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";

                Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
            }
        }

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
            StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";

            Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
            Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
        }

        private void V_Cuoi()
        {
            iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;
            StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
            StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
            StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";

            Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
            Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
        }

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
                                // iRow = 0;
                                iRow = iOldRow;
                                //load lại form theo stt_rec
                                StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
                                StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
                                StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
                            }
                        }
                        break;
                }
            }
        }

        private void V_Xoa()
        {
            //if (!SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, txtNgay_ct.dValue) && currActionTask == ActionTask.View)
            //{
            //    ExMessageBox.Show( 65,StartUp.SysObj, "Không thể xóa được chứng từ đã khóa sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}
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
                // Nên dịch chuyển iRow lùi dòng 0
                // Sau đó RowFilter lại Table[0], Table[1], Table[2]
                // Rồi mới xóa Table[0]
                StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString() + "'";
                StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString() + "'";
                StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString() + "'";

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
                    // iRow = 0;
                    iRow = iRow > StartUp.DsTrans.Tables[0].Rows.Count - 1 ? iRow - 1 : iRow;
                    //load lại form theo stt_rec
                    StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
                    StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
                    StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString() + "'";
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
            //SmReport.ReportManager oReport = new SmReport.ReportManager(StartUp.SysObj, StartUp.CommandInfo["rep_file"].ToString());
            //StartUp.GetDmnt();
            //oReport.Preview(StartUp.DsTrans);
            
            FrmIn oReport = new FrmIn();
            oReport.ShowDialog();
        }

        private void V_Copy()
        {
            if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim()))
                return;
            currActionTask = ActionTask.Copy;
            FrmAPCTPN1Copy _formcopy = new FrmAPCTPN1Copy();
            _formcopy.Closed += new EventHandler(_formcopy_Closed);
            _formcopy.ShowDialog();
        }

        void _formcopy_Closed(object sender, EventArgs e)
        {
            if ((sender as FrmAPCTPN1Copy).isCopy == true)
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
                    DataRow NewRecord = StartUp.DsTrans.Tables[0].NewRow();
                    //copy dữ liệu từ row được chọn copy cho row mới
                    NewRecord.ItemArray = StartUp.DsTrans.Tables[0].Rows[iRow].ItemArray;
                    //gán lại stt_rec, ngày ct
                    NewRecord["stt_rec"] = newSttRec;
                    NewRecord["ngay_ct"] = FrmAPCTPN1Copy.ngay_ct;
                    //if (StartUp.M_ngay_lct.Trim().Equals("0"))
                    //{
                        NewRecord["ngay_lct"] = FrmAPCTPN1Copy.ngay_ct;
                    //}
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

                    iOldRow = iRow;
                    iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;
                    //load lại form
                    StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";
                    StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";
                    StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";
                    
                    IsInEditMode.Value = true;
                    //IsVisibilityFieldsXamDataGrid
                    SetStatusVisibleField();
                }
            }
        }

        void NewRowCt()
        {
            DataRow NewRecord = StartUp.DsTrans.Tables[1].NewRow();
            NewRecord["stt_rec"] = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"];
            int Stt_rec0 = 0, Stt_rec0ct = 0, Stt_rec0ctgt=0;
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
            NewRecord["tien_nt"] = 0;
            NewRecord["tien"] = 0;
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

        private bool GrdCt_AddNewRecord(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            NewRowCt();
            return true;
        }

        private void GrdCt_EditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            try
            {
                if (IsEditMode && GrdCt.ActiveCell != null && StartUp.DsTrans.Tables[1].DefaultView.Count > GrdCt.ActiveRecord.Index && StartUp.DsTrans.Tables[1].GetChanges(DataRowState.Deleted) == null)
                        switch (e.Cell.Field.Name)
                        {
                            case "tk_vt":
                                {
                                    AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                    if (txt.RowResult != null && !txt.Text.Trim().Equals(""))
                                    {
                                            e.Cell.Record.Cells["ten_tk"].Value = txt.RowResult["ten_tk"];
                                            e.Cell.Record.Cells["ten_tk2"].Value = txt.RowResult["ten_tk2"];
                                    }
                                    if (txt.Text.Trim().Equals(""))
                                    {
                                        e.Cell.Record.Cells["ten_tk"].Value = "";
                                        e.Cell.Record.Cells["ten_tk2"].Value = "";
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

                                            if (cbMa_nt.Text == StartUp.M_ma_nt0)
                                            {
                                                e.Cell.Record.Cells["tien"].Value = e.Cell.Record.Cells["tien_nt"].Value;
                                            }
                                            else
                                            {
                                                if (Tien != 0)
                                                {
                                                    e.Cell.Record.Cells["tien"].Value = Tien;
                                                }
                                            }
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
                            case "tien":
                                {
                                    if (e.Cell.IsDataChanged)
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
        private void V_Sua()
        {
            if (StartUp.DsTrans.Tables[0].Rows.Count == 0)
                ExMessageBox.Show( 70,StartUp.SysObj, "Không có dữ liệu!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                txtMa_kh.IsFocus = true;
                currActionTask = ActionTask.Edit;

                DsVitual = new DataSet();
                DsVitual.Tables.Add(StartUp.DsTrans.Tables[0].DefaultView.ToTable());
                DsVitual.Tables.Add(StartUp.DsTrans.Tables[1].DefaultView.ToTable());
                DsVitual.Tables.Add(StartUp.DsTrans.Tables[2].DefaultView.ToTable());

                IsInEditMode.Value = true;
                Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
            }
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
                    txtMa_kh.IsFocus = true;

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
                    NewRecord["t_tien_nt"] = 0;
                    NewRecord["t_tien"] = 0;
                    NewRecord["t_thue_nt"] = 0;
                    NewRecord["t_thue"] = 0;
                    NewRecord["t_tt_nt"] = 0;
                    NewRecord["t_tt"] = 0;
                    //Them moi dong trong Ct
                    DataRow NewCtRecord = StartUp.DsTrans.Tables[1].NewRow();
                    NewCtRecord["stt_rec"] = newSttRec;
                    NewCtRecord["stt_rec0"] = "001";
                    NewCtRecord["ma_ct"] = StartUp.Ma_ct;
                    NewCtRecord["ngay_ct"] = txtNgay_ct.Value == null ? DateTime.Now.Date : txtNgay_ct.dValue.Date;
                    NewCtRecord["tien_nt"] = 0;
                    NewCtRecord["tien"] = 0;

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
                    StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + newSttRec + "'";

                    iOldRow = iRow;
                    iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;
                    IsInEditMode.Value = true;

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
            try
            {
                StartUp.DsTrans.Tables[1].AcceptChanges();
                StartUp.DsTrans.Tables[2].AcceptChanges();
                bool isError = false;
                if (!IsSequenceSave)
                {
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

                    if (GrdCt.Records.Count == 0)
                    {
                        ExMessageBox.Show( 75,StartUp.SysObj, "Chưa vào tài khoản nợ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        SmLib.WinAPISenkey.SenKey(ModifierKeys.Alt, Key.D1);
                        return;
                    }

                    //Kiểm tra mã khách trong ph có chưa
                    if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_kh"].ToString().Trim()))
                    {
                        ExMessageBox.Show( 80,StartUp.SysObj, "Chưa có mã khách hàng!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtMa_kh.IsFocus = true;
                        isError = true;
                    }
                    //Kiểm tra tk có trong ph có chưa
                    else if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nx"].ToString().Trim()))
                    {
                        ExMessageBox.Show( 85,StartUp.SysObj, "Chưa vào tài khoản có!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtMa_nx.IsFocus = true;
                        isError = true;
                    }
                    //Kiểm tra có ngày hạch toán hay chưa
                    else if (txtNgay_ct.dValue == new DateTime())
                    {
                        ExMessageBox.Show( 90,StartUp.SysObj, "Chưa vào ngày hạch toán!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtNgay_ct.Focus();
                        isError = true;
                    }
                 else
                    if ( StartUp.M_NGAY_BAT_DAU != null && (!txtNgay_ct.IsValueValid || txtNgay_ct.dValue < StartUp.M_NGAY_BAT_DAU || txtNgay_ct.dValue > StartUp.M_NGAY_KET_THUC))
                    {
                        ExMessageBox.Show(1024, StartUp.SysObj, "Ngày hạch toán không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtNgay_ct.Focus();
                        isError = true;
                    }   else if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[0]["tk_vt"].ToString().Trim()))
                    {
                        ExMessageBox.Show( 95,StartUp.SysObj, "Chưa vào tài khoản nợ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        TabInfo.SelectedIndex = 0;
                        GrdCt.ExecuteCommand(DataPresenterCommands.CellFirstOverall);
                        GrdCt.Focus();
                        isError = true;
                    }
                    else if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"].ToString().Trim()))
                    {
                        ExMessageBox.Show( 100,StartUp.SysObj, "Chưa vào số chứng từ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtSo_ct.Focus();
                        isError = true;
                    }

                    if (!isError)
                    {
                        if (StartUp.DsTrans.Tables[2].DefaultView.Count > 0)
                        {
                            bool showMessage = false;

                            //Kiem tra trung hoa don 
                            bool showMessageCheckHD = false;
                            string so_ct0 = "", so_seri0 = "", ma_so_thue = "";
                            string ngay_ct0;


                            for (int i = 0; i < StartUp.DsTrans.Tables[2].DefaultView.Count; i++)
                            {
                                DataRowView drv = StartUp.DsTrans.Tables[2].DefaultView[i];
                                if (string.IsNullOrEmpty(drv.Row["ma_ms"].ToString().Trim()) || string.IsNullOrEmpty(drv.Row["so_ct0"].ToString().Trim()))
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
                                        ExMessageBox.Show( 105,StartUp.SysObj, "Mã số thuế không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                        showMessage = true;
                                        if (StartUp.M_MST_CHECK.Equals("2"))
                                        {
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

                                    if (StartUp.CheckExistHDVao(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString(), so_ct0, so_seri0, ngay_ct0, ma_so_thue) && !showMessageCheckHD)
                                    {
                                        ExMessageBox.Show( 110,StartUp.SysObj, string.Format("Hoá đơn số [{0}], ký hiệu [{1}], ngày [{2}], MST [{3}] đã tồn tại!", so_ct0, so_seri0, ngay_ct0, ma_so_thue), "", MessageBoxButton.OK, MessageBoxImage.Information);
                                        showMessageCheckHD = true;
                                        if (StartUp.M_CHK_HD_VAO == 2)
                                        {
                                            //Cảnh báo và không cho lưu
                                            return;
                                        }
                                    }
                                }

                                if (drv["tk_thue_cn"].ToString().Trim().Equals("1") && string.IsNullOrEmpty(drv["ma_kh2"].ToString().Trim()))
                                {
                                    ExMessageBox.Show( 115,StartUp.SysObj, "Chưa vào cục thuế!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                    GrdCtgt.ActiveCell = (GrdCtgt.Records[i] as DataRecord).Cells["ma_kh2"];
                                    GrdCtgt.Focus();
                                    return;
                                }

                            }
                            if(!CheckVoucherOutofDate())
                                isError = true;
                        }
                        if (StartUp.DsTrans.Tables[1].DefaultView.Count > 0)
                        {
                            foreach (DataRowView drv in StartUp.DsTrans.Tables[1].DefaultView)
                            {
                                if (string.IsNullOrEmpty(drv.Row["tk_vt"].ToString().Trim()))
                                {
                                    StartUp.DsTrans.Tables[1].Rows.Remove(drv.Row);
                                    StartUp.DsTrans.Tables[1].AcceptChanges();
                                    continue;
                                }
                            }
                        }
                    }
                }
                /////////////////////////////////////////////////////
                if (!isError)
                {
                    if (!IsSequenceSave)
                    {
                        object o_t_tien_thue = StartUp.DsTrans.Tables[2].Compute("sum(t_tien_nt)", "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'");
                        object o_t_tien_hang = StartUp.DsTrans.Tables[1].Compute("sum(tien_nt)", "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'");

                        object o_t_tien0_thue = StartUp.DsTrans.Tables[2].Compute("sum(t_tien)", "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'");
                        object o_t_tien0_hang = StartUp.DsTrans.Tables[1].Compute("sum(tien)", "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'");

                        decimal t_tien_thue = Convert.ToDecimal(o_t_tien_thue.Equals(DBNull.Value) ? 0 : o_t_tien_thue);
                        decimal t_tien_hang = Convert.ToDecimal(o_t_tien_hang.Equals(DBNull.Value) ? 0 : o_t_tien_hang);
                        decimal t_tien0_thue = Convert.ToDecimal(o_t_tien0_thue.Equals(DBNull.Value) ? 0 : o_t_tien0_thue);
                        decimal t_tien0_hang = Convert.ToDecimal(o_t_tien0_hang.Equals(DBNull.Value) ? 0 : o_t_tien0_hang);
                        if (currActionTask == ActionTask.Copy && (t_tien_hang != t_tien_thue || t_tien0_thue != t_tien0_hang))
                        {
                            if (txtT_thue.nValue != 0 && txtT_thue_nt.nValue != 0 && txtT_thue_Nt0.nValue != 0)
                            {
                                ExMessageBox.Show( 120,StartUp.SysObj, "Tổng tiền/ tiền ngoại tệ khác với tổng tiền/ tiền ngoại tệ trong các hóa đơn giá trị gia tăng!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                isError = true;
                            }
                        }

                        else if (t_tien_hang != t_tien_thue || t_tien0_thue != t_tien0_hang)
                        {
                            if (GrdCtgt.Records.Count > 0)
                                ExMessageBox.Show( 125,StartUp.SysObj, "Tổng tiền/ tiền ngoại tệ khác với tổng tiền/ tiền ngoại tệ trong các hóa đơn giá trị gia tăng!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        //Cân bằng trường tiền
                        decimal _ty_gia = txtTy_gia.nValue;

                        if (!cbMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()) && GrdCt.Records.Count > 0 && _ty_gia != 0 && !ChkSuaTien.IsChecked.Value)
                        {
                            decimal _so_phieu_sai = 0;
                            var v_so_phieu_sai = StartUp.DsTrans.Tables[1].AsEnumerable()
                               .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() && (b.Field<decimal?>("tien_nt") == 0) && b.Field<decimal?>("tien") != 0)
                               .Count();
                            if (v_so_phieu_sai != null)
                                decimal.TryParse(v_so_phieu_sai.ToString(), out _so_phieu_sai);
                            if (_so_phieu_sai == 0)
                            {
                                //Tính tiền hàng
                                decimal _sum_tien = SmLib.SysFunc.Round(_ty_gia * t_tien_hang, Convert.ToInt16(StartUp.M_ROUND));

                                txtT_Tien_Nt0.Value = _sum_tien;
                                ////Gán số dư cho phiếu đầu tiên
                                decimal _sum_tien_nt0 = 0;
                                var vtien_nt0 = StartUp.DsTrans.Tables[1].AsEnumerable()
                                    .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                                    .Sum(x => x.Field<decimal?>("tien"));
                                if (vtien_nt0 != null)
                                    decimal.TryParse(vtien_nt0.ToString(), out _sum_tien_nt0);
                                (GrdCt.Records[0] as DataRecord).Cells["tien"].Value = Convert.ToDecimal((GrdCt.Records[0] as DataRecord).Cells["tien"].Value) + (_sum_tien - _sum_tien_nt0);
                                //Tính tiền thuế
                                decimal _sum_thue_nt0 = 0;
                                _sum_thue_nt0 = txtT_thue_Nt0.Value == DBNull.Value ? 0 : Convert.ToDecimal(txtT_thue_Nt0.nValue);

                                //Tính tổng thanh toán
                                txtT_tt_Nt0.Value = _sum_thue_nt0 + _sum_tien;
                            }
                            else if (_so_phieu_sai < GrdCt.Records.Count)
                            {
                                //Tính tiền hàng
                                decimal _sum_tien = SmLib.SysFunc.Round(_ty_gia * t_tien_hang, Convert.ToInt16(StartUp.M_ROUND));

                                txtT_Tien_Nt0.Value = _sum_tien;
                                ////Gán số dư cho phiếu đầu tiên không sai
                                decimal _sum_tien_nt0 = 0;
                                var vtien_nt0 = StartUp.DsTrans.Tables[1].AsEnumerable()
                                    .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                                    .Sum(x => x.Field<decimal?>("tien"));
                                if (vtien_nt0 != null)
                                    decimal.TryParse(vtien_nt0.ToString(), out _sum_tien_nt0);
                                for (int i = 0; i < GrdCt.Records.Count; i++)
                                {
                                    DataRecord dr = GrdCt.Records[i] as DataRecord;
                                    decimal tien_nt = 0, tien = 0;
                                    decimal.TryParse(dr.Cells["tien_nt"].Value.ToString(), out tien_nt);
                                    decimal.TryParse(dr.Cells["tien"].Value.ToString(), out tien);
                                    if (tien_nt == 0 && tien != 0)
                                    {
                                        //Phiếu sai.
                                    }
                                    else
                                    {
                                        dr.Cells["tien"].Value = tien + (_sum_tien - _sum_tien_nt0);
                                        break;
                                    }
                                }
                                //Tính tiền thuế
                                decimal _sum_thue_nt0 = 0;
                                _sum_thue_nt0 = txtT_thue_Nt0.Value == DBNull.Value ? 0 : Convert.ToDecimal(txtT_thue_Nt0.nValue);

                                //Tính tổng thanh toán
                                txtT_tt_Nt0.Value = _sum_thue_nt0 + _sum_tien;
                            }
                        }

                        //Điền thông tin vào 1 số trường khác cho Ph.
                        if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"].ToString()))
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"] = StartUp.DmctInfo["ma_gd"];
                        if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_dvcs"].ToString()))
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ma_dvcs"] = StartUp.SysObj.GetOption("M_MA_DVCS").ToString();
                        // update so_ct0 , ngay_ct0,so_seri0 cho Table0 (Ph) , lấy thông tin từ record có tiền thuế lớn nhất trong tab HĐ Thuế
                        int index_max_thue = Lay_Index_Record_Co_TienThueMax();
                        if (index_max_thue != -1)
                        {
                            StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct0"] = StartUp.DsTrans.Tables[2].DefaultView[index_max_thue]["so_ct0"];
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ngay_ct0"] = StartUp.DsTrans.Tables[2].DefaultView[index_max_thue]["ngay_ct0"];
                            StartUp.DsTrans.Tables[0].DefaultView[0]["so_seri0"] = StartUp.DsTrans.Tables[2].DefaultView[index_max_thue]["so_seri0"];
                        }
                        
                    }
                    DataTable tbPhToSave = StartUp.DsTrans.Tables[0].Clone();
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
                        if (!IsSequenceSave)
                        {
                            // update thông tin cho các record Table1 (Ct) 
                            drv["ngay_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ngay_ct"];
                            drv["so_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"];
                            drv["ma_ct"] = StartUp.Ma_ct;
                        }

                        tbCtToSave.Rows.Add(drv.Row.ItemArray);
                    }

                    foreach (DataRowView drv in StartUp.DsTrans.Tables[2].DefaultView)
                    {
                        if (!IsSequenceSave)
                        {
                            // update thông tin cho các record Table2 (Ctgt) 
                            drv["ma_nt"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"];
                            drv["ty_gia"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"];
                            drv["ty_giaf"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ty_giaf"];
                            drv["status"] = StartUp.DsTrans.Tables[0].DefaultView[0]["status"];
                            drv["ma_gd"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"];
                            drv["so_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"];
                        }
                        tbCtGtToSave.Rows.Add(drv.Row.ItemArray);
                    }
                    if (!DataProvider.UpdateCtTable(StartUp.SysObj, StartUp.DmctInfo["m_ctdbf"].ToString(), tbCtToSave, StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()))
                    {
                        ExMessageBox.Show( 130,StartUp.SysObj, "Lưu không thành công, kiểm tra lại dữ liệu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    if (!DataProvider.UpdateCtTable(StartUp.SysObj, StartUp.DmctInfo["m_ctgtdbf"].ToString(), tbCtGtToSave, StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()))
                    {
                        ExMessageBox.Show( 135,StartUp.SysObj, "Lưu không thành công, kiểm tra lại dữ liệu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }
                /////////////////////////////////////////////
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
                                                if (ExMessageBox.Show( 140,StartUp.SysObj, "Có chứng từ trùng số. Số cuối cùng là: " + "[" + GetLastSoct(StartUp.SysObj, txtMa_qs.Text).Trim() + "]" + ". Có lưu chứng từ này không?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                                                {
                                                    txtSo_ct.SelectAll();
                                                    txtSo_ct.Focus();
                                                    isError = true;
                                                }
                                            }
                                            else if (StartUp.M_trung_so.Equals("2"))
                                            {
                                                ExMessageBox.Show( 145,StartUp.SysObj, "Số chứng từ đã tồn tại!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                                txtSo_ct.SelectAll();
                                                txtSo_ct.Focus();
                                                isError = true;
                                            }
                                            break;
                                        }
                                    case "PH02":
                                        {
                                            ExMessageBox.Show( 150,StartUp.SysObj, "Tk có là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                            isError = true;
                                            txtMa_nx.IsFocus = true;
                                            break;
                                        }
                                    case "CT01":
                                        {
                                            int index = Convert.ToInt16(dv[1]);
                                            ExMessageBox.Show( 155,StartUp.SysObj, "Tk vật tư là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                            isError = true;
                                            tiHT.Focus();
                                            GrdCt.ActiveCell = (GrdCt.Records[index] as DataRecord).Cells["tk_vt"];
                                            GrdCt.Focus();
                                            break;
                                        }
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
                                            ExMessageBox.Show( 160,StartUp.SysObj, "Tk thuế là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                            isError = true;
                                            tabItem3.Focus();
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
                //if (!isError && CheckValidSoct(StartUp.SysObj, txtMa_qs.Text, txtSo_ct.Text, StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()))
                //{
                //    if (StartUp.M_trung_so.Equals("1"))
                //    {
                //        if (ExMessageBox.Show( 165,StartUp.SysObj, "Có chứng từ trùng số. Số cuối cùng là: " + "[" + GetLastSoct(StartUp.SysObj, txtMa_qs.Text).Trim() + "]" + ". Có lưu chứng từ này không?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                //        {
                //            txtSo_ct.SelectAll();
                //            txtSo_ct.Focus();
                //            isError = true;
                //        }
                //    }
                //    else
                //    {
                //        ExMessageBox.Show( 170,StartUp.SysObj, "Số chứng từ đã tồn tại!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                //        txtSo_ct.SelectAll();
                //        txtSo_ct.Focus();
                //        isError = true;
                //    }
                //}

                //#region Kiểm tra tài khoản chi tiết
                //if (!isError && StartUp.IsTkMe(txtMa_nx.Text.ToString().Trim()))
                //{
                //    ExMessageBox.Show( 175,StartUp.SysObj, "Tk có là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                //    isError = true;
                //    txtMa_nx.IsFocus = true;
                //}

                //for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count && isError == false; i++)
                //{
                //    if (!isError && StartUp.IsTkMe(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_vt"].ToString().Trim()))
                //    {
                //        ExMessageBox.Show( 180,StartUp.SysObj, "Tk vật tư là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                //        isError = true;
                //        GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_vt"];
                //        GrdCt.Focus();
                //    }
                //}
                //for (int i = 0; i < StartUp.DsTrans.Tables[2].DefaultView.Count && isError == false; i++)
                //{
                //    if (!string.IsNullOrEmpty(StartUp.DsTrans.Tables[2].DefaultView[i]["tk_thue_no"].ToString().Trim()))
                //        if (!isError && StartUp.IsTkMe(StartUp.DsTrans.Tables[2].DefaultView[i]["tk_thue_no"].ToString().Trim()))
                //        {
                //            ExMessageBox.Show( 185,StartUp.SysObj, "Tk thuế là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                //            isError = true;
                //            GrdCtgt.ActiveCell = (GrdCtgt.Records[i] as DataRecord).Cells["tk_thue_no"];
                //            GrdCtgt.Focus();
                //        }
                //}
                //#endregion

                if (!isError)
                {
                    string _stt_rec1 = StartUp.DsTrans.Tables[1].DefaultView[0]["stt_rec"].ToString();
                    ThreadStart start = delegate()
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
                                    LoadDataDu13();
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
                        IsInEditMode.Value = false;
                        currActionTask = ActionTask.View;
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
            SqlCommand cmd = new SqlCommand("exec [dbo].[APCTPN1-Post] @stt_rec");
            cmd.Parameters.Add("@stt_rec", SqlDbType.Char, 11).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"];
            StartUp.SysObj.ExcuteNonQuery(cmd);
        }

        private int Lay_Index_Record_Co_TienThueMax()
        {
            int index = -1;
            decimal maxTien = 0;
            for (int i = 0; i < StartUp.DsTrans.Tables[2].DefaultView.Count; i++)
            {
                if (Parsedecimal(StartUp.DsTrans.Tables[2].DefaultView[i]["t_thue"], 0) > maxTien)
                {
                    maxTien = decimal.Parse(StartUp.DsTrans.Tables[2].DefaultView[i]["t_thue"].ToString());
                    index = i;
                }
            }
            return index;
        }

        public decimal Parsedecimal(object obj, decimal defaultvalue)
        {
            decimal ketqua = defaultvalue;
            decimal.TryParse(obj != null ? obj.ToString() : defaultvalue.ToString(), out ketqua);
            return ketqua;
        }

        private void btnNhan_Click(object sender, RoutedEventArgs e)
        {
            V_Nhan();
        }
        #endregion

        private void btnXem_Click(object sender, RoutedEventArgs e)
        {
            V_Xem();
        }

        private void V_Xem()
        {
            currActionTask = ActionTask.View;

            //  set lai stringbrowse 
            //string stringBrowse1 = "ngay_ct:fl:100:h=Ngày c.từ;so_ct:fl:70:h=Số c.từ;ma_kh:100:h=Mã khách;ten_kh:225:h=Tên khách;dien_giai:225:h=Diễn giải;t_tien_nt:n1:130:h=Tiền hàng nt;t_thue_nt:n1:130:h=Tiền thuế nt;t_tt_nt:n1:130:h=Tổng tiền tt nt;ma_nx:100:h=Mã nx;tk_thue_no:80:h=Tk thuế;t_tien:n0:130:h=Tiền hàng;t_thue:n0:130:h=Tiền thuế;t_tt:130:n0:h=Tổng tiền tt;ma_nt:80:h=Mã nt;ty_gia:140:h=Tỷ giá:r;[date]:140:h=Ngày cập nhật;[time]:140:h=Giờ cập nhật;[user_name]:180:h=Tên NSD";
            //string stringBrowse2 = "tk_vt:fl:100:h=Tk nợ;ten_tk:fl:180:h=Tên tài khoản;tien_nt:130:n1:h=Tiền nt;dien_giaii:225:h=Diễn giải;tien:130:n0:h=Tiền";
            string stringBrowse1, stringBrowse2;
            if (StartUp.M_LAN.Equals("V"))
            {
                stringBrowse1 = StartUp.CommandInfo["Vbrowse2"].ToString().Split('|')[0];
                stringBrowse2 = StartUp.CommandInfo["Vbrowse2"].ToString().Split('|')[1];
            }
            else
            {
                stringBrowse1 = StartUp.CommandInfo["Ebrowse2"].ToString().Split('|')[0];
                stringBrowse2 = StartUp.CommandInfo["Ebrowse2"].ToString().Split('|')[1];
            }
            DataTable PhViewTablev = StartUp.DsTrans.Tables[0].Copy();
            PhViewTablev.Rows.RemoveAt(0);
            SmVoucherLib.FormView _frmView = new SmVoucherLib.FormView(StartUp.SysObj, PhViewTablev.DefaultView, StartUp.DsTrans.Tables[1].DefaultView, stringBrowse1, stringBrowse2, "stt_rec");
            SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, _frmView.frmBrw.oBrowseCt, StartUp.Ma_ct, 1);
            _frmView.frmBrw.Title = SmLib.SysFunc.Cat_Dau(StartUp.M_LAN.Equals("V") ? StartUp.CommandInfo["bar"].ToString() : StartUp.CommandInfo["bar2"].ToString());
            _frmView.ListFieldSum = "t_tt_nt;t_tt";

            //_frmView.DataGrid.FieldLayouts[0].Fields[0].Settings.AllowFixing = AllowFieldFixing.No;






            _frmView.frmBrw.LanguageID  = "APCTPN1_4";
            _frmView.ShowDialog();

            // Set lai irow va rowfilter ...
            if (_frmView.DataGrid.ActiveRecord != null)
            {

                int select_irow = (_frmView.DataGrid.ActiveRecord as DataRecord).Index;
                if (select_irow >= 0)
                {
                    string selected_stt_rec = (_frmView.DataGrid.DataSource as DataView)[select_irow]["stt_rec"].ToString();
                    FrmAPCTPN1.iRow = select_irow + 1;
                    StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + selected_stt_rec + "'";
                    StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + selected_stt_rec + "'";
                    StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + selected_stt_rec + "'";
                    
                }
            }   
        }

        private void V_Tim()
        {
            try
            {
                currActionTask = ActionTask.View;
                //FrmTim2 _FrmTim2 = new FrmTim2(StartUp.SysObj, StartUp.DmctInfo["m_phdbf"].ToString(), StartUp.Ma_ct);
                //_FrmTim2.ShowDialog();
                FrmTim3 _FrmTim3 = new FrmTim3(StartUp.SysObj,StartUp.filterId, StartUp.filterView);
                SmLib.SysFunc.LoadIcon(_FrmTim3);
                _FrmTim3.ShowDialog();
                Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
                
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
                if (IsEditMode && GrdCtgt.ActiveCell != null && StartUp.DsTrans.Tables[2].DefaultView.Count > GrdCtgt.ActiveRecord.Index && StartUp.DsTrans.Tables[2].GetChanges(DataRowState.Deleted) == null)  
                        switch (e.Cell.Field.Name)
                        {
                            case "so_ct0":
                                {
                                    if (e.Cell.Record.Cells["ma_kh"].Value == DBNull.Value || (e.Cell.Record.Cells["ma_kh"].Value != null && string.IsNullOrEmpty(e.Cell.Record.Cells["ma_kh"].Value.ToString().Trim())))
                                    {
                                        //Cập nhật thông tin thuế
                                        e.Cell.Record.Cells["ma_kh"].Value = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_kh"];
                                        if (txtMa_kh.RowResult == null)
                                            txtMa_kh.SearchInit();
                                        if (txtMa_kh.RowResult != null)
                                        {
                                            e.Cell.Record.Cells["dia_chi_dmkh"].Value = txtMa_kh.RowResult["dia_chi"];
                                            e.Cell.Record.Cells["ma_so_thue_dmkh"].Value = txtMa_kh.RowResult["ma_so_thue"];
                                        }
                                        e.Cell.Record.Cells["ten_kh"].Value =  StartUp.DsTrans.Tables[0].DefaultView[0]["ten_kh"];
                                        e.Cell.Record.Cells["dia_chi"].Value = string.IsNullOrEmpty(txtDia_chi.Text.Trim()) ? StartUp.DsTrans.Tables[0].DefaultView[0]["dia_chi_kh"] : txtDia_chi.Text;
                                        e.Cell.Record.Cells["ma_so_thue"].Value = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_so_thue"];
                                        e.Cell.Record.Cells["ten_vt"].Value = txtDien_giai.Text;
                                        e.Cell.Record.Cells["han_tt"].Value = txtMa_kh.RowResult["han_tt"];
                                        if (GrdCtgt.FieldLayouts[0].Fields.IndexOf("ma_vv") != -1 && GrdCt.FieldLayouts[0].Fields.IndexOf("ma_vv_i") != -1)
                                        {
                                            if (GrdCt.Records.Count > 0)
                                            {
                                                e.Cell.Record.Cells["ma_vv"].Value = (GrdCt.Records[0] as DataRecord).Cells["ma_vv_i"].Value;
                                            }
                                        }
                                        if (GrdCtgt.FieldLayouts[0].Fields.IndexOf("ma_phi") != -1 && GrdCt.FieldLayouts[0].Fields.IndexOf("ma_phi_i") != -1)
                                        {
                                            if (GrdCt.Records.Count > 0)
                                            {
                                                e.Cell.Record.Cells["ma_phi"].Value = (GrdCt.Records[0] as DataRecord).Cells["ma_phi_i"].Value;
                                            }
                                        }

                                        decimal _T_tien_nt_hien_tai = 0, _T_tien_hien_tai = 0;
                                        var vtien_nt = StartUp.DsTrans.Tables[2].AsEnumerable()
                                                    .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                                                    .Sum(x => x.Field<decimal?>("t_tien_nt"));
                                        if (vtien_nt != null)
                                            decimal.TryParse(vtien_nt.ToString(), out _T_tien_nt_hien_tai);

                                        var vtien = StartUp.DsTrans.Tables[2].AsEnumerable()
                                                    .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                                                    .Sum(x => x.Field<decimal?>("t_tien"));
                                        if (vtien != null)
                                            decimal.TryParse(vtien.ToString(), out _T_tien_hien_tai);

                                        e.Cell.Record.Cells["t_tien_nt"].Value = (txtT_Tien_nt.Value == DBNull.Value ? 0 : (Convert.ToDecimal(txtT_Tien_nt.Value) - _T_tien_nt_hien_tai));
                                        e.Cell.Record.Cells["t_tien"].Value = (txtT_Tien_Nt0.Value == DBNull.Value ? 0 : (Convert.ToDecimal(txtT_Tien_Nt0.Value) - _T_tien_hien_tai));
                                    }
                                    UpdateTotalThue();
                                    break;
                                }
                            case "ma_kh":
                                {
                                    if (e.Editor.Value == null)
                                        return;
                                    AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                    if (txt.RowResult != null)
                                    {
                                        if (e.Editor.Value.ToString().Trim() != "")
                                        {
                                            if (!string.IsNullOrEmpty(txt.RowResult["ten_kh"].ToString()))
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
                            case "t_tien_nt":
                                {
                                    if (e.Cell.IsDataChanged)
                                    {
                                        if (!cbMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
                                        {
                                            if (e.Cell.Record.Cells["t_tien"].Value.ToString().Trim().Equals(0))
                                            {
                                                decimal tien_nt = Convert.ToDecimal(e.Cell.Record.Cells["t_tien_nt"].Value.Equals(DBNull.Value) ? 0 : e.Cell.Record.Cells["t_tien_nt"].Value);
                                                decimal _Ty_gia = txtTy_gia.nValue;
                                                decimal tien = SmLib.SysFunc.Round(_Ty_gia * tien_nt, Convert.ToInt16(StartUp.M_ROUND));
                                                if (tien != 0)
                                                    e.Cell.Record.Cells["t_tien"].Value = tien;
                                            }
                                        }
                                        else
                                        {
                                            e.Cell.Record.Cells["t_tien"].Value = e.Cell.Record.Cells["t_tien_nt"].Value;
                                        }
                                    }
                                }
                                break;
                            case "t_tien":
                                {
                                    if (e.Cell.IsDataChanged)
                                    {
                                        if (e.Cell.Record.Cells["t_tien"].Value == DBNull.Value || e.Cell.Record.Cells["t_tien"].Value.ToString().Trim().Equals("0"))
                                        {
                                            decimal tien_nt = Convert.ToDecimal(e.Cell.Record.Cells["t_tien_nt"].Value.Equals(DBNull.Value) ? 0 : e.Cell.Record.Cells["t_tien_nt"].Value);
                                            decimal _Ty_gia =  txtTy_gia.nValue;
                                            e.Cell.Record.Cells["t_tien"].Value = SmLib.SysFunc.Round(_Ty_gia * tien_nt, Convert.ToInt16(StartUp.M_ROUND));
                                        }
                                    }
                                }
                                break;
                            case "ma_thue":
                                {
                                    AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                    //if (txt.IsDataChanged)
                                    //{                                    
                                        //Cập nhật tài khoản thuế
                                        if (txt.RowResult != null)
                                        {
                                            e.Cell.Record.Cells["tk_thue_no"].Value = txt.RowResult["tk_thue_no"];
                                            if (GrdCtgt.ActiveRecord.Index.Equals(0))
                                                txttk_thue_no.Text = txt.RowResult["tk_thue_no"].ToString();

                                            e.Cell.Record.Cells["thue_suat"].Value = txt.RowResult["thue_suat"];

                                            CellValuePresenter cellTkThueNo = CellValuePresenter.FromCell(e.Cell.Record.Cells["tk_thue_no"]);
                                            AutoCompleteTextBox txtTkThueNo = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(cellTkThueNo.Editor as ControlHostEditor);
                                            if (txtTkThueNo.RowResult == null)
                                                txtTkThueNo.SearchInit();
                                            if (txtTkThueNo.RowResult != null)
                                            {
                                                e.Cell.Record.Cells["tk_thue_cn"].Value = txtTkThueNo.RowResult["tk_cn"];
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(e.Cell.Record.Cells["thue_suat"].Value.ToString()))
                                        {
                                            decimal tien_nt = Convert.ToDecimal(e.Cell.Record.Cells["t_tien_nt"].Value.Equals(DBNull.Value) ? 0 : e.Cell.Record.Cells["t_tien_nt"].Value);
                                            decimal tien = Convert.ToDecimal(e.Cell.Record.Cells["t_tien"].Value.Equals(DBNull.Value) ? 0 : e.Cell.Record.Cells["t_tien"].Value);
                                            decimal thue_suat = Convert.ToDecimal(e.Cell.Record.Cells["thue_suat"].Value.Equals(DBNull.Value) ? 0 : e.Cell.Record.Cells["thue_suat"].Value);
                                            decimal thue_nt = tien_nt * thue_suat / 100;
                                            if (!cbMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
                                            {
                                                thue_nt = SmLib.SysFunc.Round(thue_nt, Convert.ToInt16(StartUp.M_ROUND_NT));
                                                e.Cell.Record.Cells["t_thue_nt"].Value = thue_nt;
                                                e.Cell.Record.Cells["t_thue"].Value = SmLib.SysFunc.Round(tien * thue_suat / 100, Convert.ToInt16(StartUp.M_ROUND));

                                            }
                                            else
                                            {
                                                thue_nt = SmLib.SysFunc.Round(thue_nt, Convert.ToInt16(StartUp.M_ROUND));
                                                e.Cell.Record.Cells["t_thue_nt"].Value = thue_nt;
                                                e.Cell.Record.Cells["t_thue"].Value = e.Cell.Record.Cells["t_thue_nt"].Value;
                                            }



                                            UpdateTotalThue();
                                        }

                                        
                                    //}
                                }
                                break;
                            case "tk_thue_no":
                                {
                                    AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                    //Cập nhật tài khoản thuế
                                    if (GrdCtgt.ActiveRecord.Index.Equals(0))
                                    {
                                        txttk_thue_no.Text = txt.Text;
                                    }

                                    if (txt.RowResult != null)
                                    {
                                        e.Cell.Record.Cells["tk_thue_cn"].Value = txt.RowResult["tk_cn"];
                                        if (e.Cell.Record.Cells["tk_thue_cn"].Value.ToString().Trim().Equals("0"))
                                        {
                                            e.Cell.Record.Cells["ma_kh2"].Value = "";
                                        }
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
                                            if (!cbMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
                                            {
                                                thue_nt = SmLib.SysFunc.Round(thue_nt, Convert.ToInt16(StartUp.M_ROUND_NT));
                                            }
                                            else
                                            {
                                                thue_nt = SmLib.SysFunc.Round(thue_nt, Convert.ToInt16(StartUp.M_ROUND));
                                            }
                                            e.Cell.Record.Cells["t_thue_nt"].Value = thue_nt;
                                        }
                                        if (cbMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
                                        {
                                            e.Cell.Record.Cells["t_thue"].Value = e.Editor.Value;
                                        }
                                        else
                                        {
                                            decimal _Ty_gia = txtTy_gia.nValue;
                                            decimal thue_nt = Convert.ToDecimal(e.Cell.Record.Cells["t_thue_nt"].Value.Equals(DBNull.Value) ? 0 : e.Cell.Record.Cells["t_thue_nt"].Value);
                                            decimal thue = thue_nt * _Ty_gia;
                                            if (thue != 0)
                                            {
                                                e.Cell.Record.Cells["t_thue"].Value = SmLib.SysFunc.Round(thue, Convert.ToInt16(StartUp.M_ROUND));
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
                                            e.Cell.Record.Cells["t_thue"].Value = SmLib.SysFunc.Round(tien * thue_suat / 100, Convert.ToInt16(StartUp.M_ROUND));
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
                Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
                SetStatusVisibleField();
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
                CalculateTyGia();
            }
        }

        void SetStatusVisibleField()
        {
            ChangeLanguage();

            //if (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Trim().Equals(StartUp.M_ma_nt0.Trim()))
            //{
            //    //GrdCt.FieldLayouts[0].Fields["tien"].Visibility = Visibility.Hidden;
            //    GrdCtgt.FieldLayouts[0].Fields["t_tien"].Visibility = Visibility.Hidden;
            //    GrdCtgt.FieldLayouts[0].Fields["t_thue"].Visibility = Visibility.Hidden;
               
            //    GrdCt.FieldLayouts[0].Fields["tien"].Settings.CellMaxWidth = 0;
            //    GrdCtgt.FieldLayouts[0].Fields["t_tien"].Settings.CellMaxWidth = 0;
            //    GrdCtgt.FieldLayouts[0].Fields["t_thue"].Settings.CellMaxWidth = 0;
            //}
            //else
            //{
                
            //    //GrdCt.FieldLayouts[0].Fields["tien"].Visibility = Visibility.Visible;
            //    GrdCtgt.FieldLayouts[0].Fields["t_tien"].Visibility = Visibility.Visible;
            //    GrdCtgt.FieldLayouts[0].Fields["t_thue"].Visibility = Visibility.Visible;
            //    GrdCt.FieldLayouts[0].Fields["tien"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["tien"].Width.Value.Value;
            //    GrdCtgt.FieldLayouts[0].Fields["t_tien"].Settings.CellMaxWidth = GrdCtgt.FieldLayouts[0].Fields["t_tien"].Width.Value.Value;
            //    GrdCtgt.FieldLayouts[0].Fields["t_thue"].Settings.CellMaxWidth = GrdCtgt.FieldLayouts[0].Fields["t_thue"].Width.Value.Value;
            //}
           
        }
        private bool txtDiaChiFocusable = true;
        private void txtMa_kh_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (IsInEditMode.Value == true)
            {
                if (txtMa_kh.RowResult == null || string.IsNullOrEmpty(txtMa_kh.Text.Trim()))
                    return;
                if (M_LAN.ToUpper().Equals("V"))
                {
                    txtTen_kh.Text = txtMa_kh.RowResult["ten_kh"].ToString();
                    StartUp.DsTrans.Tables[0].Rows[iRow]["ten_kh2"] = txtMa_kh.RowResult["ten_kh2"].ToString();
                }
                else
                {
                    txtTen_kh.Text = txtMa_kh.RowResult["ten_kh2"].ToString();
                    StartUp.DsTrans.Tables[0].Rows[iRow]["ten_kh"] = txtMa_kh.RowResult["ten_kh"].ToString();
                }
                StartUp.DsTrans.Tables[0].AcceptChanges();

                if (string.IsNullOrEmpty(txtMa_kh.RowResult["dia_chi"].ToString().Trim()))
                {
                    txtDiaChiFocusable = true;
                }
                else
                {
                    StartUp.DsTrans.Tables[0].DefaultView[0]["dia_chi"] = txtMa_kh.RowResult["dia_chi"].ToString().Trim();
                    txtDiaChiFocusable = false;
                }

                if (!string.IsNullOrEmpty(txtMa_kh.RowResult["doi_tac"].ToString().Trim()))
                {
                    StartUp.DsTrans.Tables[0].DefaultView[0]["ong_ba"] = txtMa_kh.RowResult["doi_tac"].ToString().Trim();
                }
                StartUp.DsTrans.Tables[0].DefaultView[0]["ma_thck"] = txtMa_kh.RowResult["ma_thck"].ToString().Trim();
                txtMaSoThue.Text = txtMa_kh.RowResult["ma_so_thue"].ToString();
                txtMa_nx.Text = string.IsNullOrEmpty(txtMa_nx.Text.Trim()) ? txtMa_kh.RowResult["tk"].ToString().Trim() : txtMa_nx.Text.Trim();

                LoadDataDu13();
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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //IsInEditMode.Value = false;
            //GrdCt.FieldSettings.AllowEdit = IsInEditMode.Value;
            GrdLayout20.Visibility = Visibility.Hidden;
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
                    GrdCt.ActiveCell = (GrdCt.ActiveRecord as DataRecord).Cells["tk_vt"];
                    break;
                case Key.F8:
                    {
                        if (ExMessageBox.Show( 190,StartUp.SysObj, "Có xóa dòng ghi hiện thời không?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
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

        private void GrdCtgt_KeyUp(object sender, KeyEventArgs e)
        {
            if (IsInEditMode.Value == false)
                return;

            switch (e.Key)
            {
                case Key.F4:
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
                case Key.F8:
                    {
                        if (ExMessageBox.Show( 195,StartUp.SysObj, "Có xóa dòng ghi hiện thời không?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
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
                            GrdCtgt.ExecuteCommand(DataPresenterCommands.EndEditModeAndDiscardChanges);
                            if (indexCell >= 0)
                            {
                                StartUp.DsTrans.Tables[2].Rows.Remove(StartUp.DsTrans.Tables[2].DefaultView[ARow.Index].Row);
                                StartUp.DsTrans.Tables[2].AcceptChanges();

                                if (GrdCtgt.Records.Count > 0)
                                    GrdCtgt.ActiveRecord = GrdCtgt.Records[indexRecord > GrdCtgt.Records.Count - 1 ? GrdCtgt.Records.Count - 1 : indexRecord];
                                UpdateTotalThue();
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

        }
        bool NewRowCtGt()
        {
            try
            {
                DataRow NewRecord = StartUp.DsTrans.Tables[2].NewRow();
                NewRecord["stt_rec"] = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"];
                //NewRecord["ma_kho_i"] = (GrdCt.ActiveRecord as DataRecord).Cells["ma_kho_i"].Value;

                //int.TryParse(StartUp.DsTrans.Tables[1].DefaultView[(GrdCtgt.ActiveRecord as DataRecord).Index]["stt_rec0"].ToString(), out Stt_rec0);
                //Stt_rec0++;
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
                NewRecord["ma_ms"] = StartUp.M_ma_ms;
                NewRecord["ngay_ct"] = txtNgay_ct.Value;
                NewRecord["ten_vt"] = txtDien_giai.Text;
                NewRecord["t_tien_nt"] = 0;
                NewRecord["t_tien"] = 0;
                NewRecord["thue_suat"] = 0;
                NewRecord["t_thue_nt"] = 0;
                NewRecord["t_thue"] = 0;
                 
                FreeCodeFieldLib.CarryFreeCodeFields(StartUp.SysObj, StartUp.Ma_ct, StartUp.DsTrans.Tables[2].DefaultView, NewRecord, 2);
                StartUp.DsTrans.Tables[2].Rows.Add(NewRecord);
                UpdateTotalThue();
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
                if (currActionTask == ActionTask.View)
                    return;
                StartUp.DsTrans.Tables[1].AcceptChanges();
                //Cập nhật tổng thanh toán nguyên tệ
                decimal _t_tien_nt = 0, _t_thue_nt = 0;
               
                var vtien_nt = StartUp.DsTrans.Tables[1].AsEnumerable()
                    .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                    .Sum(x => x.Field<decimal?>("tien_nt"));
                if (vtien_nt != null)
                    decimal.TryParse(vtien_nt.ToString(), out _t_tien_nt);
                txtT_Tien.Value = _t_tien_nt;
                txtT_Tien_nt.Value = _t_tien_nt;

                //Tính thuế
                _t_thue_nt = txtT_thue_nt.Value == DBNull.Value ? 0 : Convert.ToDecimal(txtT_thue_nt.nValue);

                //Tính tổng thanh toán
                txtT_tt.Value = _t_tien_nt + _t_thue_nt;
                txtT_tt_nt.Value = _t_tien_nt + _t_thue_nt;
                //Cập nhật tổng thanh toán cho tien0
                if (!cbMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
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
                    decimal _t_tien = 0, _t_thue = 0;

                    var vtien = StartUp.DsTrans.Tables[1].AsEnumerable()
                        .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                        .Sum(x => x.Field<decimal?>("tien"));
                    if (vtien != null)
                        decimal.TryParse(vtien.ToString(), out _t_tien);
                    _t_thue = txtT_thue_Nt0.Value == DBNull.Value ? 0 : Convert.ToDecimal(txtT_thue_Nt0.nValue);

                    txtT_Tien_Nt0.Value = _t_tien;
                    txtT_tt_Nt0.Value = _t_tien + _t_thue;
                }
                else
                {
                    txtT_Tien_Nt0.Value = _t_tien_nt;
                    txtT_tt_Nt0.Value = _t_tien_nt + _t_thue_nt;
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
                if (currActionTask == ActionTask.View)
                    return;
                StartUp.DsTrans.Tables[2].AcceptChanges();
                //Cập nhật tổng thanh toán nguyên tệ
                decimal _t_tien = 0, _t_thue = 0;

                //Tiền hàng
                _t_tien = txtT_Tien_nt.Value == DBNull.Value ? 0 : Convert.ToDecimal(txtT_Tien_nt.nValue);

                //Tính thuế
                var vthue = StartUp.DsTrans.Tables[2].AsEnumerable()
                                .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                                .Sum(x => x.Field<decimal?>("t_thue_nt"));
                if (vthue != null)
                    decimal.TryParse(vthue.ToString(), out _t_thue);

                txtT_thue.Value = _t_thue;
                txtT_thue_nt.Value = _t_thue;
                //Tính tổng thanh toán
                txtT_tt.Value = _t_tien + _t_thue;
                txtT_tt_nt.Value = _t_tien + _t_thue;
                //Cập nhật tổng thanh toán cho tien0
                if (!cbMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
                {
                    //tiền nt0
                    decimal _sum_tien_nt0 = 0;
                    _sum_tien_nt0 = txtT_Tien_Nt0.Value == DBNull.Value ? 0 : Convert.ToDecimal(txtT_Tien_Nt0.nValue);
                    //thuế nt0
                    decimal _sum_thue_nt0 = 0;
                    var vthue_nt0 = StartUp.DsTrans.Tables[2].AsEnumerable()
                                .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                                .Sum(x => x.Field<decimal?>("t_thue"));
                    if (vthue_nt0 != null)
                        decimal.TryParse(vthue_nt0.ToString(), out _sum_thue_nt0);

                    txtT_thue_Nt0.Value = _sum_thue_nt0;
                    //Tính tổng thanh toán
                    txtT_tt_Nt0.Value = _sum_tien_nt0 + _sum_thue_nt0;
                }
                else
                {
                    txtT_thue_Nt0.Value = _t_thue;
                    txtT_tt_Nt0.Value = _t_tien + _t_thue;
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        private void GrdCt_RecordDelete(object sender, Infragistics.Windows.DataPresenter.Events.RecordsDeletedEventArgs e)
        {
            SmLib.WinAPISenkey.SenKey(ModifierKeys.Alt, Key.D2);
        }

        #region GetLanguageString
        public override string GetLanguageString(string code, string language)
        {
            return StartUp.GetLanguageString(code, language);
        }
        #endregion

        private void txtMa_nx_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMa_nx.Text.Trim()) && !txtMa_nx.IsReadOnly && txtMa_nx.RowResult != null)
            {
                if (M_LAN.ToUpper().Equals("V"))
                {
                    txtTenTK.Text = txtMa_nx.RowResult["ten_nx"].ToString();
                    StartUp.DsTrans.Tables[0].Rows[iRow]["ten_nx2"] = txtMa_nx.RowResult["ten_nx2"].ToString();
                }
                else
                {
                    txtTenTK.Text = txtMa_nx.RowResult["ten_nx2"].ToString();
                    StartUp.DsTrans.Tables[0].Rows[iRow]["ten_nx"] = txtMa_nx.RowResult["ten_nx"].ToString();
                }
            }
            LoadDataDu13();
        }

        private void FormMain_Closed(object sender, EventArgs e)
        {
            if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
                App.Current.Shutdown();
        }

        private void txtTy_gia_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Voucher_Ma_nt0.Value)
            {
                KeyboardNavigation.SetTabNavigation(GrNT, KeyboardNavigationMode.Continue);

                SmLib.WinAPISenkey.SenKey(ModifierKeys.None, Key.Tab);
            }
        }

        private void LoadDataDu13()
        {
            txtSoDuKH.Value = ArapLib.ArFuncLib.GetSdkh13(StartUp.SysObj, StartUp.DsTrans.Tables[0].DefaultView[0]["ma_kh"].ToString(), StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nx"].ToString());
        }

        private void txtMa_qs_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (IsInEditMode.Value && !e.NewFocus.GetType().Equals(typeof(SmVoucherLib.ToolBarButton)))
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
                }
        }

        private void GrdCtgt_RecordDelete(object sender, Infragistics.Windows.DataPresenter.Events.RecordsDeletedEventArgs e)
        {
            UpdateTotalThue();
            GrdCtgt.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);
            txtHan_ck.IsFocus = true;
        }

        private void txtghi_chu_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (IsInEditMode.Value)
                if (Keyboard.IsKeyDown(Key.Enter) && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
                {
                    TextBox txt = sender as TextBox;
                    txt.SelectedText = Environment.NewLine;
                    txt.SelectionStart = txt.SelectionStart + 1;
                    txt.SelectionLength = 0;
                    e.Handled = true;
                }
                else if (Keyboard.IsKeyDown(Key.Enter))
                {
                    (this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus();
                    e.Handled = true;
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

        private void txtngay_lct_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!txtngay_lct.IsFocusWithin)
                if (currActionTask == ActionTask.Add || currActionTask == ActionTask.Edit || currActionTask == ActionTask.Copy)
                {
                    if (!txtNgay_ct.dValue.Date.Equals(txtngay_lct.dValue.Date))
                    {
                        ExMessageBox.Show( 200,StartUp.SysObj, "Ngày lập chứng từ khác với ngày hạch toán!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
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
        private void CalculateTyGia()
        {
            if (txtTy_gia.nValue != 0)
            {
                ////Hạch toán
                decimal _ty_gia = 0;
                _ty_gia = txtTy_gia.nValue;

                for (int i = 0; i < GrdCt.Records.Count; i++)
                {
                    if ((GrdCt.Records[i] as DataRecord).Cells["tien_nt"].Value != DBNull.Value)
                    {
                        decimal _tien_nt = 0;
                        decimal.TryParse((GrdCt.Records[i] as DataRecord).Cells["tien_nt"].Value.ToString(), out _tien_nt);
                        if (_tien_nt * _ty_gia > 0)
                            (GrdCt.Records[i] as DataRecord).Cells["tien"].Value = SmLib.SysFunc.Round(_ty_gia * _tien_nt, Convert.ToInt16(StartUp.M_ROUND));
                    }
                }
                UpdateTotalHT();

                //HĐ thuế
                for (int i = 0; i < GrdCtgt.Records.Count; i++)
                {
                    if ((GrdCtgt.Records[i] as DataRecord).Cells["t_tien_nt"].Value != DBNull.Value)
                    {
                        decimal _tien_nt = Convert.ToDecimal((GrdCtgt.Records[i] as DataRecord).Cells["t_tien_nt"].Value);
                        if (_ty_gia * _tien_nt > 0)
                        {
                            decimal _tien_nt0 = SmLib.SysFunc.Round(_ty_gia * _tien_nt, Convert.ToInt16(StartUp.M_ROUND));
                            (GrdCtgt.Records[i] as DataRecord).Cells["t_tien"].Value = _tien_nt0;
                            if ((GrdCtgt.Records[i] as DataRecord).Cells["thue_suat"].Value != DBNull.Value)
                            {
                                decimal _thue_suat = Convert.ToDecimal((GrdCtgt.Records[i] as DataRecord).Cells["thue_suat"].Value);
                                decimal _thue_nt = SmLib.SysFunc.Round(_tien_nt * _thue_suat / 100, Convert.ToInt16(StartUp.M_ROUND_NT));
                                (GrdCtgt.Records[i] as DataRecord).Cells["t_thue_nt"].Value = _thue_nt;
                                //(GrdCtgt.Records[i] as DataRecord).Cells["t_thue"].Value = SmLib.SysFunc.Round(_thue_nt * _ty_gia, Convert.ToInt16(StartUp.M_ROUND));
                                (GrdCtgt.Records[i] as DataRecord).Cells["t_thue"].Value = SmLib.SysFunc.Round(_tien_nt0 * _thue_suat / 100, Convert.ToInt16(StartUp.M_ROUND));
                            }
                        }
                    }
                }
                UpdateTotalThue();
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

        private void txtHan_ck_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                (this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus();
            }));
        }
    }
}
