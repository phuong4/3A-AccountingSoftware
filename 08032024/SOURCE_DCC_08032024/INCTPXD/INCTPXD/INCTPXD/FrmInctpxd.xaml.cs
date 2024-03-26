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
using System.Threading;

namespace Inctpxd
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FrmInctpxd : SmVoucherLib.FormTrans
    {
        public static int iRow = 0;
        int iRow_old = 0;
        CodeValueBindingObject Voucher_Ma_nt0;
        CodeValueBindingObject Voucher_Lan0;
        CodeValueBindingObject IsInEditMode;
        CodeValueBindingObject IsCheckedSua_tien;
        CodeValueBindingObject IsCheckedPx_gia_dd;
        CodeValueBindingObject Ty_Gia_ValueChange;
        CodeValueBindingObject M_Ngay_lct;
        //public static ActionTask currActionTask = ActionTask.None;

        DataSet dsCheckData;
        public DataSet DsVitual;

        public FrmInctpxd()
        {
            InitializeComponent();
            LanguageProvider.Language = StartUp.M_LAN;
            this.BindingSysObj = StartUp.SysObj;
            C_QS = txtMa_qs;
            C_NgayHT = txtNgay_ct;
            C_Ma_nt = txtMa_nt;
            C_So_ct = txtSo_ct;
        }

        #region FrmInctpxd_Loaded
        void FrmInctpxd_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (StartUp.DsTrans.Tables[0].Rows.Count > 1)
                    iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;
                IsInEditMode = (CodeValueBindingObject)FormMain.FindResource("IsInEditMode");
                Voucher_Ma_nt0 = (CodeValueBindingObject)FormMain.FindResource("Voucher_Ma_nt0");
                Voucher_Lan0 = (CodeValueBindingObject)FormMain.FindResource("Voucher_Lan0");
                IsCheckedSua_tien = (CodeValueBindingObject)FormMain.FindResource("IsCheckedSua_tien");
                IsCheckedPx_gia_dd = (CodeValueBindingObject)FormMain.FindResource("IsCheckedPx_gia_dd");
                Ty_Gia_ValueChange = (CodeValueBindingObject)FormMain.FindResource("Ty_Gia_ValueChange");
                M_Ngay_lct = (CodeValueBindingObject)FormMain.FindResource("M_Ngay_lct");
                M_Ngay_lct.Value = StartUp.M_ngay_lct.Equals("1");

                Binding bind = new Binding("Value");
                bind.Source = IsInEditMode;
                bind.Mode = BindingMode.OneWay;
                this.SetBinding(FormTrans.IsEditModeProperty, bind);

                //Gán ngôn ngữ messagebox
                M_LAN = StartUp.M_LAN;
                GrdCt.Lan = StartUp.M_LAN;                               

                //Them cac truong tu do
                SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, GrdCt, StartUp.Ma_ct, 1);

                tblGhi_chu.Text = StartUp.M_LAN.Equals("V") ? "Ghi chú" : "Notes";

                //load form theo stt_rec
                if (StartUp.DsTrans.Tables[0].Rows.Count > 0)
                {
                    StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString());

                    LoadData();
                    
                    IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
                    IsCheckedSua_tien.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"].ToString() == "1");
                    IsCheckedPx_gia_dd.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["px_gia_dd"].ToString() == "1");
                    Ty_Gia_ValueChange.Value = false;
                    
                }

                Voucher_Lan0.Value = M_LAN.Equals("V");
                TabInfo.SelectedIndex = 0;
                SetFocusToolbar();
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

        #region V_Sau
        void V_Sau()
        {
            if (iRow < StartUp.DsTrans.Tables[0].Rows.Count - 1)
                iRow++;
            //filter Table[0], Table[1]
            StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString());
            IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());

        }
        #endregion

        #region V_Truoc
        void V_Truoc()
        {
            if (iRow > 1)
                iRow--;
            //filter Table[0], Table[1]
            StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString());
            IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
           
        }
        #endregion

        #region V_Dau
        void V_Dau()
        {
            iRow = StartUp.DsTrans.Tables[0].Rows.Count > 1 ? 1 : 0;
            //filter Table[0], Table[1]
            StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString());
            IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
          
        }
        #endregion

        #region V_Cuoi
        void V_Cuoi()
        {
            iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;
            //filter Table[0], Table[1]
            StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString());
            IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
           
        }
        #endregion

        #region V_Moi
        private void V_Moi()
        {
            try
            {
                //if (SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, DateTime.Now))
                //{
                    currActionTask = ActionTask.Add;
                    string newSttRec = DataProvider.NewTrans(StartUp.SysObj, StartUp.Ma_ct, StartUp.Ws_Id);
                    //khi thêm mới, gán lại giá trị 
                    if (!string.IsNullOrEmpty(newSttRec))
                    {
                        DsVitual = StartUp.DsTrans.Copy();
                        
                        //Them moi dong trong Ph
                        DataRow NewRecord = StartUp.DsTrans.Tables[0].NewRow();
                        NewRecord["stt_rec"] = newSttRec;
                        NewRecord["ma_ct"] = StartUp.Ma_ct;
                        NewRecord["row_id"] = 0;
                        
                        if (SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, txtNgay_ct.dValue))
                        {
                            NewRecord["ngay_ct"] = txtNgay_ct.dValue.Date;
                        }
                        else
                        {
                            NewRecord["ngay_ct"] = DateTime.Now.Date;
                        }


                        if (StartUp.DsTrans.Tables[0].Rows.Count == 1)
                        {
                            NewRecord["ma_nt"] = StartUp.DmctInfo["ma_nt"];
                            NewRecord["ma_gd"] = StartUp.DmctInfo["ma_gd"];
                            NewRecord["ma_qs"] = GetDMQS(BindingSysObj, StartUp.Ma_ct, Convert.ToDateTime(NewRecord["ngay_ct"]), StartUp.M_User_Id);
                        }
                        else
                        {
                            NewRecord["ma_gd"] = StartUp.DsTrans.Tables[0].Rows[iRow]["ma_gd"];
                            NewRecord["ma_nt"] = StartUp.DsTrans.Tables[0].Rows[iRow]["ma_nt"];
                            NewRecord["ma_qs"] = GetDMQS(BindingSysObj, StartUp.Ma_ct, Convert.ToDateTime(NewRecord["ngay_ct"]),
                            StartUp.M_User_Id, StartUp.DsTrans.Tables[0].Rows[iRow]["ma_qs"].ToString().Trim());
                        }

                        if (NewRecord["ma_nt"].ToString().Trim().Equals(StartUp.M_ma_nt0.Trim()))
                            NewRecord["ty_giaf"] = 1;
                        else
                            NewRecord["ty_giaf"] = StartUp.GetRates(NewRecord["ma_nt"].ToString().Trim(), Convert.ToDateTime(NewRecord["ngay_ct"]).Date);
                        
                        NewRecord["status"] = StartUp.DmctInfo["ma_post"];
                        NewRecord["sua_tien"] = 0;
                        NewRecord["px_gia_dd"] = 0;
                        NewRecord["t_tien_nt"] = 0;
                        NewRecord["t_tien"] = 0;
                        NewRecord["t_so_luong"] = 0;

                        //Them moi dong trong Ct
                        DataRow NewCtRecord = StartUp.DsTrans.Tables[1].NewRow();
                        NewCtRecord["stt_rec"] = newSttRec;
                        NewCtRecord["stt_rec0"] = "001";
                        NewCtRecord["ma_ct"] = StartUp.Ma_ct;
                        NewCtRecord["ngay_ct"] = NewRecord["ngay_ct"];
                        if (StartUp.DsTrans.Tables[1].DefaultView.Count > 0)
                            NewCtRecord["ma_kho_i"] = StartUp.DsTrans.Tables[1].DefaultView[0]["ma_kho_i"];
                        NewCtRecord["so_luong"] = 0;
                        NewCtRecord["gia_nt"] = 0;
                        NewCtRecord["tien_nt"] = 0;
                        NewCtRecord["gia"] = 0;
                        NewCtRecord["tien"] = 0;
                      
                        StartUp.DsTrans.Tables[0].Rows.Add(NewRecord);
                        StartUp.DsTrans.Tables[1].Rows.Add(NewCtRecord);

                        iRow_old = iRow;
                        iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;

                        //filter lại Table[0], Table[1]
                        StartUp.DataFilter(newSttRec);
                        IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].Rows[iRow]["ma_nt"].ToString());

                        DsVitual = null;
                        IsInEditMode.Value = true;
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            txtma_gd.IsFocus = true;
                        }));
                        
                    }
              //  }
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
                ExMessageBox.Show( 870,StartUp.SysObj, "Không có dữ liệu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                if (StartUp.DsTrans.Tables[0].Rows.Count == 1)
                    return;
                //if (!SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, txtNgay_ct.dValue))
                //{
                //    ExMessageBox.Show( 875,StartUp.SysObj, "Dữ liệu đã khóa sổ, không sửa được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                //    return;
                //}
                currActionTask = ActionTask.Edit;
                DsVitual = new DataSet();
                //copy Table[0], Table[1]
                DsVitual.Tables.Add(StartUp.DsTrans.Tables[0].DefaultView.ToTable());
                DsVitual.Tables.Add(StartUp.DsTrans.Tables[1].DefaultView.ToTable());

                IsInEditMode.Value = true;
                //UpdateTonKho();

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    txtma_gd.IsFocus = true;
                }));
            }
        }
        #endregion

        #region V_Huy
        private void V_Huy()
        {
            IsInEditMode.Value = false;
            if (StartUp.DsTrans.Tables[0].Rows.Count > 0)
            {
                //Refresh lại khi chọn edit
                switch (currActionTask)
                {
                    #region Edit
                    case ActionTask.Edit:
                        {
                            if (DsVitual != null)
                            {
                                //xóa các row trong table[1], table[2]
                                string stt_rec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();

                                // Nên dịch chuyển iRow lùi dòng 0
                                // Sau đó RowFilter lại Table[0], Table[1], Table[2]
                                StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString());

                                //Refresh lại grid hoạch toán
                                if (StartUp.DsTrans.Tables[1].Rows.Count > 0)
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
                                
                                //RowFilter lại Table[0], Table[1] với iRow trước khi edit
                                StartUp.DataFilter(stt_rec);
                            }

                        }
                        break;
                    #endregion
                    //Refresh lại khi chọn new
                    case ActionTask.Add:
                    case ActionTask.Copy:
                        {
                            Xoa();
                            iRow = iRow_old;
                            StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString());
                        }
                        break;
                }
                IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
            }
            TabInfo.SelectedIndex = 0;
            currActionTask = ActionTask.None;

        }
        #endregion

        #region V_Xoa
        void Xoa()
        {
            currActionTask = ActionTask.Delete;
            try
            {
                string _stt_rec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();

                //Delete tksd13
                StartUpTrans.UpdateTkSd13(1, 0);

                //xóa trong ph, ct, ctgt
                //xóa chứng từ
                SqlCommand cmd = new SqlCommand("exec [dbo].[DeleteVoucher] @ma_ct, @stt_rec");
                cmd.Parameters.Add("@ma_ct", SqlDbType.Char, 3).Value = StartUp.Ma_ct;
                cmd.Parameters.Add("@stt_rec", SqlDbType.Char, 11).Value = _stt_rec;
                StartUp.SysObj.ExcuteNonQuery(cmd);

                // ----Warning : Không nên xóa Table[0] trước, nếu xóa trước sẽ bị mất Binding -----------------------
                // Nên dịch chuyển iRow lùi 1 dòng
                // Sau đó RowFilter lại Table[0], Table[1]
                // Rồi mới xóa Table[0]
                //iRow = iRow > 0 ? iRow - 1 : 0;
                StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString());

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
                
                //Refresh lại Table[0], Table[1]
                if (StartUp.DsTrans.Tables[0].Rows.Count > 0)
                {
                    txtNgay_ct.Text = "";
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
        private void V_Xoa()
        {
            //if (!SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, txtNgay_ct.dValue))
            //{
            //    ExMessageBox.Show( 880,StartUp.SysObj, "Không thể xóa chứng từ đã khóa sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}
            if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim()))
                return;
            Xoa();
            IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
        }
        #endregion

        #region V_Nhan
        private void V_Nhan()
        {
            try
            {
                if (!IsSequenceSave)
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
                }

                if (CheckValid())
                {
                    DataTable tbCtToSave = StartUp.DsTrans.Tables[1].Clone();
                    if (!IsSequenceSave)
                    {

                        //Điền thông tin vào 1 số trường khác cho Ph.

                        if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_dvcs"].ToString()))
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ma_dvcs"] = StartUp.SysObj.GetOption("M_MA_DVCS").ToString();

                        // update userId, date, time cho Table0 (Ph)
                        //if (currActionTask == ActionTask.Add || currActionTask == ActionTask.Copy)
                        //{
                        //    StartUp.DsTrans.Tables[0].DefaultView[0]["user_id0"] = StartUp.SysObj.UserInfo.Rows[0]["user_id"].ToString();
                        //    StartUp.DsTrans.Tables[0].DefaultView[0]["date0"] = DateTime.Now.Date;
                        //    StartUp.DsTrans.Tables[0].DefaultView[0]["time0"] = DateTime.Now.ToString("HH:mm:ss");
                        //}
                        //StartUp.DsTrans.Tables[0].DefaultView[0]["user_id"] = StartUp.SysObj.UserInfo.Rows[0]["user_id"].ToString();
                        //StartUp.DsTrans.Tables[0].DefaultView[0]["user_name"] = StartUp.SysObj.UserInfo.Rows[0]["user_name"].ToString();
                        //StartUp.DsTrans.Tables[0].DefaultView[0]["date"] = DateTime.Now.Date;
                        //StartUp.DsTrans.Tables[0].DefaultView[0]["time"] = DateTime.Now.ToString("HH:mm:ss");

                        //DataProvider.DeleteRow(StartUp.SysObj, StartUp.DmctInfo["m_ctdbf"].ToString(), "stt_rec='" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"] + "'");

                        //Áp giá phiếu xuất
                        DateTime ngay_ct = (DateTime)StartUp.DsTrans.Tables[0].DefaultView[0]["ngay_ct"];
                        object px_gia_dd = StartUp.DsTrans.Tables[0].DefaultView[0]["px_gia_dd"];
                        string ma_nt = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                        string ma_kho;
                        string ma_vt;
                        decimal so_luong;

                        if (ngay_ct >= StartUp.ngay_gia_px && px_gia_dd.ToString() != "1")
                        {
                            DataSet dsApgia;
                            SqlCommand cmdApgia = new SqlCommand("Ingia_px");
                            cmdApgia.CommandType = CommandType.StoredProcedure;
                            cmdApgia.Parameters.Add("@Ngay_ct", SqlDbType.SmallDateTime).Value = ngay_ct;
                            cmdApgia.Parameters.Add("@Ma_kho", SqlDbType.VarChar).Value = "";
                            cmdApgia.Parameters.Add("@Ma_vt", SqlDbType.VarChar).Value = "";
                            cmdApgia.Parameters.Add("@So_luong", SqlDbType.Decimal).Value = 0;

                            for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
                            {
                                string gia_ton =StartUp.DsTrans.Tables[1].DefaultView[i]["gia_ton"].ToString().Trim();
                                if (gia_ton.Equals("1") || gia_ton.Equals("4"))
                                {
                                    ma_kho = StartUp.DsTrans.Tables[1].DefaultView[i]["ma_kho_i"].ToString();
                                    ma_vt = StartUp.DsTrans.Tables[1].DefaultView[i]["ma_vt"].ToString();
                                    so_luong = (decimal)StartUp.DsTrans.Tables[1].DefaultView[i]["so_luong"];

                                    cmdApgia.Parameters["@Ma_kho"].Value = ma_kho;
                                    cmdApgia.Parameters["@Ma_vt"].Value = ma_vt;
                                    cmdApgia.Parameters["@So_luong"].Value = so_luong;
                                    dsApgia = StartUp.SysObj.ExcuteReader(cmdApgia);
                                    if (dsApgia == null || dsApgia.Tables.Count == 0 || dsApgia.Tables[0].Rows.Count == 0)
                                        continue;
                                    StartUp.DsTrans.Tables[1].DefaultView[i]["gia"] = dsApgia.Tables[0].Rows[0]["gia"];
                                    StartUp.DsTrans.Tables[1].DefaultView[i]["tien"] = dsApgia.Tables[0].Rows[0]["tien"];

                                    if (StartUp.M_ma_nt0 == ma_nt)
                                    {
                                        StartUp.DsTrans.Tables[1].DefaultView[i]["gia_nt"] = dsApgia.Tables[0].Rows[0]["gia"];
                                        StartUp.DsTrans.Tables[1].DefaultView[i]["tien_nt"] = dsApgia.Tables[0].Rows[0]["tien"];
                                    }
                                    else
                                    {
                                        StartUp.DsTrans.Tables[1].DefaultView[i]["gia_nt"] = dsApgia.Tables[0].Rows[0]["gia_nt"];
                                        StartUp.DsTrans.Tables[1].DefaultView[i]["tien_nt"] = dsApgia.Tables[0].Rows[0]["tien_nt"];
                                    }
                                }
                            }
                            StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt"] = SumFunction(StartUp.DsTrans.Tables[1], "tien_nt", 0);
                            StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = SumFunction(StartUp.DsTrans.Tables[1], "tien", 0);
                        }

                        //Cân bằng tiền (không xài do đã áp giá và tiền vốn)
                        //if (StartUp.DsTrans.Tables[1].DefaultView.Count > 0)
                        //{
                        //    decimal t_tien_nt = ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt"], 0);
                        //    decimal ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["ty_gia"], 0);
                        //    decimal t_tien = ParseDecimal(StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"], 0);
                        //    decimal t_tien_temp = SysFunc.Round(t_tien_nt * ty_gia, StartUp.M_ROUND);

                        //    bool CanBang = false;
                        //    if (ty_gia == 0 || ChkSua_tien.IsChecked == true)
                        //        CanBang = true;
                        //    for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count && CanBang == false; i++)
                        //    {
                        //        decimal tien_nt = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["tien_nt"], 0);
                        //        decimal tien = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["tien"], 0);
                        //        string gia_ton = StartUp.DsTrans.Tables[1].DefaultView[i]["gia_ton"].ToString().Trim();
                        //        if (tien_nt != 0 && tien != 0 && (ChkPx_gia_dd.IsChecked == true || gia_ton == "2"))
                        //        {
                        //            StartUp.DsTrans.Tables[1].DefaultView[i]["tien"] = tien + (t_tien_temp - t_tien);
                        //            StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = t_tien_temp;
                        //            CanBang = true;
                        //        }
                        //    }

                        //}
                    }
                    tbCtToSave = StartUp.DsTrans.Tables[1].DefaultView.ToTable().Copy();
                    DataTable tbPhToSave = StartUp.DsTrans.Tables[0].Clone();
                    tbPhToSave.Rows.Add(StartUp.DsTrans.Tables[0].DefaultView[0].Row.ItemArray);
                    if (!IsSequenceSave)
                    {
                        tbPhToSave.Rows[0]["status"] = 0;
                    }
                    DataProvider.UpdateDataTable(StartUp.SysObj, StartUp.DmctInfo["m_phdbf"].ToString(), "stt_rec", tbPhToSave, "stt_rec;row_id");
                    if (!DataProvider.UpdateCtTable(StartUp.SysObj, StartUp.DmctInfo["m_ctdbf"].ToString(), tbCtToSave, StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()))
                    {
                        ExMessageBox.Show( 885,StartUp.SysObj, "Lưu không thành công, kiểm tra lại dữ liệu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    //StartUp.UpdateRates(tbPhToSave.Rows[0]["ma_nt"].ToString(), Convert.ToDateTime(txtNgay_ct.Value).Date, Convert.ToDecimal(txtTy_gia.Value));


                    //Check data trên server
                    /////////////////////////////////////////////
                    bool isError = false;
                    if (!IsSequenceSave)
                    {
                        if (!isError)
                        {
                            //if (dsCheckData == null || dsCheckData.Tables[0].Rows.Count == 0)
                                dsCheckData = StartUp.CheckData();

                            dsCheckData.Tables[0].AcceptChanges();
                            if (dsCheckData.Tables.Count > 0)
                            {
                                bool ismessage = false;
                                string ma_vt_xuat_am = "";
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
                                                    if (ExMessageBox.Show( 890,StartUp.SysObj, "Có chứng từ trùng số. Số cuối cùng là: " + "[" + GetLastSoct(StartUp.SysObj, txtMa_qs.Text).Trim() + "]" + ". Có lưu chứng từ này không?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                                                    {
                                                        txtSo_ct.SelectAll();
                                                        txtSo_ct.Focus();
                                                        isError = true;
                                                    }
                                                }
                                                else if (StartUp.M_trung_so.Equals("2"))
                                                {
                                                    ExMessageBox.Show( 895,StartUp.SysObj, "Số chứng từ đã tồn tại!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                                    txtSo_ct.SelectAll();
                                                    txtSo_ct.Focus();
                                                    isError = true;
                                                }
                                            }
                                            break;
                                        case "CT01":
                                            {
                                                int index = Convert.ToInt16(dv[1]);
                                                ExMessageBox.Show( 900,StartUp.SysObj, "Tk nợ là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                isError = true;
                                                GrdCt.ActiveCell = (GrdCt.Records[index] as DataRecord).Cells["tk_vt"];
                                                GrdCt.Focus();
                                            }
                                            break;
                                        case "CT02":
                                            {
                                                int index = Convert.ToInt16(dv[1]);
                                                ExMessageBox.Show( 905,StartUp.SysObj, "Tk có là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                isError = true;
                                                GrdCt.ActiveCell = (GrdCt.Records[index] as DataRecord).Cells["ma_nx_i"];
                                                GrdCt.Focus();
                                            }
                                            break;
                                        case "CT03":
                                            {
                                                StartUp.DsTrans.Tables[1].DefaultView[Convert.ToInt16(dv[1])]["ton13"] = InvtLib.InFuncLib.GetTon13(StartUp.SysObj, StartUp.DsTrans.Tables[1].DefaultView[Convert.ToInt16(dv[1])]["ma_kho_i"].ToString(), StartUp.DsTrans.Tables[1].DefaultView[Convert.ToInt16(dv[1])]["ma_vt"].ToString(), StartUp.DsTrans.Tables[1].DefaultView[Convert.ToInt16(dv[1])]["ma_vv_i"].ToString());
                                                //144089096 akhai yêu cầu sửa chỉ hiện ma_vt 
                                                if (!ma_vt_xuat_am.Contains(StartUp.DsTrans.Tables[1].DefaultView[Convert.ToInt16(dv[1])]["ma_vt"].ToString().Trim()))
                                                    ma_vt_xuat_am = ma_vt_xuat_am + StartUp.DsTrans.Tables[1].DefaultView[Convert.ToInt16(dv[1])]["ma_vt"].ToString().Trim() + ", ";
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
                                if (!string.IsNullOrEmpty(ma_vt_xuat_am))
                                {
                                    if (StartUp.M_CHK_TON_VT.Equals("2"))
                                    {
                                        ExMessageBox.Show(910, StartUp.SysObj, "Có vật tư [" + ma_vt_xuat_am + "] xuất âm hoặc tồn kho nhỏ hơn tồn tối thiếu, không lưu được!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                        isError = true;
                                    }
                                    else if (StartUp.M_CHK_TON_VT.Equals("1"))
                                    {
                                        ExMessageBox.Show(915, StartUp.SysObj, "Có vật tư [" + ma_vt_xuat_am + "] xuất âm hoặc tồn kho nhỏ hơn tồn tối thiếu!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                    }
                                }
                            }
                        }
                    }
                    if (!isError)
                    {
                        string _stt_rec1 = StartUp.DsTrans.Tables[1].DefaultView[0]["stt_rec"].ToString();
                        ThreadStart _thread = delegate()
                        {
                            //post
                            Post();

                            if (!IsSequenceSave)
                            {
                                Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                        new Action(() =>
                                        {
                                            if (StartUp.DsTrans.Tables[1].DefaultView[0]["stt_rec"].ToString().Equals(_stt_rec1))
                                            {
                                                //update ton kho 
                                                UpdateTonKho();
                                            }
                                        }));
                            }
                        };
                        new Thread(_thread).Start();

                        if (!IsSequenceSave)
                        {
                            //Update lại thứ tự các chứng từ
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

        #region Post
        void Post()
        {
            SqlCommand cmd = new SqlCommand("exec [dbo].[INCTPXD-Post] @stt_rec");
            cmd.Parameters.Add("@stt_rec", SqlDbType.VarChar, 50).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"];
            StartUp.SysObj.ExcuteNonQuery(cmd);
        }
        #endregion

        #region CheckValid
        bool CheckValid()
        {
            bool result = true;
            if (!IsSequenceSave)
            {
                string stt_rec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim();
                decimal ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);
                int sua_tien = 0;
                int.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"].ToString(), out sua_tien);
                if (IsInEditMode.Value == true)
                {
                    #region ma_gd
                    if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"].ToString()) && result == true)
                    {
                        ExMessageBox.Show( 920,StartUp.SysObj, "Chưa vào loại hóa đơn!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        result = false;
                        txtma_gd.IsFocus = true;
                    }
                    #endregion

                    #region ma_kh
                    if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_kh"].ToString()) && result == true)
                    {
                        ExMessageBox.Show( 925,StartUp.SysObj, "Chưa vào mã khách hàng!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        result = false;
                        txtMa_kh.IsFocus = true;
                    }
                    #endregion

                    #region ngay_ct
                    if ((txtNgay_ct.Value == null || txtNgay_ct.Value.ToString() == "") && result == true)
                    {
                        ExMessageBox.Show( 930,StartUp.SysObj, "Chưa vào ngày hạch toán!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        result = false;
                        txtNgay_ct.Focus();
                    }
                    if (txtNgay_ct.Value.ToString() != "" && result == true)
                    {
                        if (!txtNgay_ct.IsValueValid && result == true)
                        {
                            ExMessageBox.Show( 935,StartUp.SysObj, "Ngày hạch toán không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            result = false;
                            txtNgay_ct.Focus();
                        }
                        if (!SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, Convert.ToDateTime(txtNgay_ct.dValue)) && result == true)
                        {
                            ExMessageBox.Show( 940,StartUp.SysObj, "Ngày hạch toán phải sau ngày khóa sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            result = false;
                            txtNgay_ct.Focus();
                        }
                        if (result && Convert.ToDateTime(txtNgay_ct.dValue) < SmLib.NgayTC.GetStartDate(StartUp.M_ngay_ct0))
                        {
                            ExMessageBox.Show( 945,StartUp.SysObj, "Ngày hạch toán phải sau ngày mở sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            result = false;
                            txtNgay_ct.Focus();
                        }

                        if (result && StartUp.M_NGAY_BAT_DAU != null && (!txtNgay_ct.IsValueValid || txtNgay_ct.dValue < StartUp.M_NGAY_BAT_DAU || txtNgay_ct.dValue > StartUp.M_NGAY_KET_THUC))
                            {
                                ExMessageBox.Show(1024, StartUp.SysObj, "Ngày hạch toán không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                result = false;
                                txtNgay_ct.Focus();
                            }
                    }
                    #endregion

                    #region ngay_lct
                    if (result && StartUp.M_ngay_lct.Equals("1"))
                    {
                        if (txtNgay_lct.Value == null || txtNgay_lct.Value.ToString() == "")
                        {
                            ExMessageBox.Show( 950,StartUp.SysObj, "Chưa vào ngày lập px!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtNgay_lct.Focus();
                            return false;
                        }
                        if (!txtNgay_lct.IsValueValid)
                        {
                            ExMessageBox.Show( 955,StartUp.SysObj, "Ngày lập px không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtNgay_lct.Focus();
                            return false;
                        }
                        //if (!SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, Convert.ToDateTime(txtNgay_lct.dValue)))
                        //{
                        //    ExMessageBox.Show( 960,StartUp.SysObj, "Ngày lập px phải sau ngày khóa sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        //    txtNgay_lct.Focus();
                        //    return false;
                        //}
                    }
                    #endregion

                    #region ma_qs
                    if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_qs"].ToString()) && result == true)
                    {
                        ExMessageBox.Show( 970,StartUp.SysObj, "Chưa vào quyển sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        result = false;
                        txtMa_qs.IsFocus = true;
                    }
                    #endregion

                    #region so_ct
                    if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"].ToString()) && result == true)
                    {
                        ExMessageBox.Show( 975,StartUp.SysObj, "Chưa vào số chứng từ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        result = false;
                        txtSo_ct.Focus();
                    }
                    //if (CheckValidSoct(StartUp.SysObj, txtMa_qs.Text, txtSo_ct.Text, stt_rec) && result == true)
                    //{
                    //    if (StartUp.DmctInfo["m_trung_so"].ToString().Equals("1"))
                    //    {
                    //        if (ExMessageBox.Show( 980,StartUp.SysObj, "Có chứng từ trùng số. Số cuối cùng là " + "[" + GetLastSoct(StartUp.SysObj, txtMa_qs.Text).Trim() + "]" + ". Có lưu chứng từ này không?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    //        {
                    //            result = false;
                    //            txtSo_ct.SelectAll();
                    //            txtSo_ct.Focus();
                    //        }
                    //    }
                    //    else
                    //    {
                    //        ExMessageBox.Show( 985,StartUp.SysObj, "Số chứng từ đã có!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    //        result = false;
                    //        txtSo_ct.SelectAll();
                    //        txtSo_ct.Focus();
                    //    }
                    //}
                    #endregion

                    #region chi tiet HT
                    if (StartUp.DsTrans.Tables[1].DefaultView.Count == 0 && result == true)
                    {
                        ExMessageBox.Show( 990,StartUp.SysObj, "Chưa vào chi tiết vật tư, không lưu được!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        result = false;
                        SmLib.WinAPISenkey.SenKey(ModifierKeys.Alt, Key.D1);
                        GrdCt_AddNewRecord(null, null);
                        GrdCt.ActiveCell = (GrdCt.Records[0] as DataRecord).Cells["ma_vt"];
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            GrdCt.Focus();
                        }));
                    }
                    #endregion

                    var slvt = from o in StartUp.DsTrans.Tables[1].DefaultView.ToTable().AsEnumerable()
                               group o by new
                               {
                                   mvt = o.Field<string>("ma_vt"),
                                   mkho = o.Field<string>("ma_kho_i")
                               } into g
                               select new
                               {
                                   ma_vt = g.Key.mvt,
                                   ma_kho = g.Key.mkho,
                                   so_luong = g.Sum(p => p.Field<decimal?>("so_luong")),
                               };
                    DataTable tbVTTmp = StartUp.DsTrans.Tables[1].Clone();
                    if (slvt.ToArray().Length > 0)
                    {
                        foreach (var vt in slvt)
                        {
                            DataRow dr = tbVTTmp.NewRow();
                            dr["ma_vt"] = vt.ma_vt;
                            dr["ma_kho_i"] = vt.ma_kho;
                            dr["so_luong"] = vt.so_luong;
                            tbVTTmp.Rows.Add(dr);
                        }
                    }

                    #region kiểm tra ma_vt, ma_kho_i
                    for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count && result == true; i++)
                    {
                        StartUp.DsTrans.Tables[1].DefaultView[i]["ma_ct"] = StartUp.Ma_ct;
                        StartUp.DsTrans.Tables[1].DefaultView[i]["so_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"];
                        StartUp.DsTrans.Tables[1].DefaultView[i]["ngay_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ngay_ct"];

                        #region kiểm tra ma_vt, ma_kho_i
                        if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["ma_vt"].ToString()))
                        {
                            ExMessageBox.Show( 995,StartUp.SysObj, "Chưa vào chi tiết vật tư, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            result = false;
                            GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["ma_vt"];
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                GrdCt.Focus();
                            }));
                            return false;
                        }
                        if (result == true && string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["ma_kho_i"].ToString()))
                        {
                            ExMessageBox.Show( 1000,StartUp.SysObj, "Chưa vào chi tiết vật tư, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            result = false;
                            GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["ma_kho_i"];
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                GrdCt.Focus();
                            }));
                        }
                        #endregion

                        //#region kiểm tra tồn kho
                        //if (result && StartUp.M_TON_KHO13.Equals("1") && !StartUp.M_CHK_TON_VT.Equals("2") && !isTonKho)
                        //{
                        //    for (int j = 0; j < tbVTTmp.Rows.Count && !isTonKho; j++)
                        //    {
                        //        decimal ton_kho = GetTonKho(tbVTTmp.Rows[j]["ma_kho_i"].ToString(),
                        //                            tbVTTmp.Rows[j]["ma_vt"].ToString(),
                        //                            StartUp.DsTrans.Tables[0].DefaultView[0]["ngay_ct"]);

                        //        if (ton_kho < 0 || ton_kho < ParseDecimal(tbVTTmp.Rows[j]["so_luong"], 0)
                        //             || ton_kho < ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["sl_min"], 0))
                        //        {
                        //            isTonKho = true;
                        //            if (StartUp.M_CHK_TON_VT.Equals("0"))
                        //            {
                        //                ExMessageBox.Show( 1005,StartUp.SysObj, "Có vật tư xuất âm hoặc tồn kho nhỏ hơn tồn tối thiếu, không lưu được!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        //                result = false;
                        //            }
                        //            else if (StartUp.M_CHK_TON_VT.Equals("1"))
                        //            {
                        //                ExMessageBox.Show( 1010,StartUp.SysObj, "Có vật tư xuất âm hoặc tồn kho nhỏ hơn tồn tối thiếu!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        //            }
                        //        }
                        //    }
                        //}
                        //#endregion

                        #region kiểm tra ma_nx_i
                        if (result && string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["ma_nx_i"].ToString().Trim()))
                        {
                            ExMessageBox.Show( 1015,StartUp.SysObj, "Chưa vào tk nợ, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            result = false;
                            GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["ma_nx_i"];
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                GrdCt.Focus();
                            }));
                        }
                        //if (result && !string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["ma_nx_i"].ToString().Trim()))
                        //{
                        //    if (StartUp.IsTkMe(StartUp.DsTrans.Tables[1].DefaultView[i]["ma_nx_i"].ToString().Trim()))
                        //    {
                        //        ExMessageBox.Show( 1020,StartUp.SysObj, "Tk nợ là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        //        result = false;
                        //        GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["ma_nx_i"];
                        //        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        //        {
                        //            GrdCt.Focus();
                        //        }));
                        //    }
                        //}
                        #endregion

                        #region kiểm tra tk_vt
                        if (result && string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_vt"].ToString().Trim()))
                        {
                            ExMessageBox.Show( 1025,StartUp.SysObj, "Chưa vào tk có, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            result = false;
                            GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_vt"];
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                GrdCt.Focus();
                            }));
                        }
                        //if (result && !string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_vt"].ToString().Trim()))
                        //{
                        //    if (StartUp.IsTkMe(StartUp.DsTrans.Tables[1].DefaultView[i]["tk_vt"].ToString().Trim()))
                        //    {
                        //        ExMessageBox.Show( 1030,StartUp.SysObj, "Tk có là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        //        result = false;
                        //        GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["tk_vt"];
                        //        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        //        {
                        //            GrdCt.Focus();
                        //        }));
                        //    }
                        //}
                        #endregion

                        if (int.Parse(StartUp.DsTrans.Tables[1].DefaultView[i]["gia_ton"].ToString()) == 3)
                        {
                            if (decimal.Parse(StartUp.DsTrans.Tables[1].DefaultView[i]["so_luong"].ToString()) == 0)
                            {
                                ExMessageBox.Show( 1035,StartUp.SysObj, "Vật tư tính tồn kho theo phương pháp NTXT không được nhập số lượng = 0!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                GrdCt.ActiveCell = (GrdCt.Records[i] as DataRecord).Cells["so_luong"];
                                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                {
                                    GrdCt.Focus();
                                }));
                                result = false;
                            }
                        }
                    }
                    #endregion

                }
            }
            return result;
        }
        #endregion

        #region V_Copy
        FrmCopy _formcopy = null;
        private void V_Copy()
        {
            if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim()))
                return;
            currActionTask = ActionTask.Copy;
            _formcopy = new FrmCopy();
            SmLib.SysFunc.LoadIcon(_formcopy);

            DsVitual = null;
            _formcopy.Closed += new EventHandler(_formcopy_Closed);
            _formcopy.ShowDialog();

        }

        void _formcopy_Closed(object sender, EventArgs e)
        {
            if (_formcopy.isCopy == true)
            {
                string newSttRec = DataProvider.NewTrans(StartUp.SysObj, StartUp.Ma_ct, StartUp.Ws_Id);
                if (!string.IsNullOrEmpty(newSttRec))
                {
                    //Them moi dong trong Ph
                    DataRow NewRecord = StartUp.DsTrans.Tables[0].NewRow();
                    //copy dữ liệu từ row được chọn copy cho row mới
                    NewRecord.ItemArray = StartUp.DsTrans.Tables[0].Rows[iRow].ItemArray;
                    //gán lại stt_rec, ngày ct
                    NewRecord["stt_rec"] = newSttRec;
                    NewRecord["ngay_ct"] = _formcopy.ngay_ct;
                   // NewRecord["ngay_lct"] = _formcopy.ngay_ct;
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

                    iRow_old = iRow;
                    iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;
                    //load lại form
                    StartUp.DataFilter(newSttRec);

                    //IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());

                    IsInEditMode.Value = true;
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        txtma_gd.IsFocus = true;
                    }));
                }
            }
        }
        #endregion

        #region V_Xem
        private void V_Xem()
        {
            currActionTask = ActionTask.View;
             
            DataTable PhViewTablev = StartUp.DsTrans.Tables[0].Copy();
            PhViewTablev.Rows.RemoveAt(0);
 

            SmVoucherLib.FormView _frmView = new SmVoucherLib.FormView(StartUp.SysObj, PhViewTablev.DefaultView, StartUp.DsTrans.Tables[1].DefaultView, StartUp.stringBrowse1, StartUp.stringBrowse2, "stt_rec");
            _frmView.ListFieldSum = "t_tien_nt;t_tien";
            
            _frmView.frmBrw.Title = StartUp.M_Tilte;
            //Them cac truong tu do
            SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, _frmView.frmBrw.oBrowseCt, StartUp.Ma_ct, 1);

            _frmView.frmBrw.LanguageID  = "Inctpxd_5";
            _frmView.ShowDialog();
            //+		_frmView.DataGrid.ActiveRecord.GetType()	{Name = "SummaryRecord" FullName = "Infragistics.Windows.DataPresenter.SummaryRecord"}	System.Type {System.RuntimeType}
            //SummaryRecord
            // Set lai irow va rowfilter ...
            if (_frmView.DataGrid.ActiveRecord != null)
            {
                if (!_frmView.DataGrid.ActiveRecord.GetType().Name.Equals("DataRecord"))
                    return;
                int select_irow = (_frmView.DataGrid.ActiveRecord as DataRecord).Index;
                if (select_irow >= 0)
                {
                    string selected_stt_rec = (_frmView.DataGrid.DataSource as DataView)[select_irow]["stt_rec"].ToString();
                    FrmInctpxd.iRow = select_irow + 1;
                    StartUp.DataFilter(selected_stt_rec);
                    IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
                    
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
                //FrmSearchApctpn9 _FrmTim = new FrmSearchApctpn9(StartUp.SysObj, StartUp.DmctInfo["m_phdbf"].ToString(), StartUp.Ma_ct);
                FrmSearchInctpxd _FrmTim = new FrmSearchInctpxd(StartUp.SysObj, StartUp.filterId, StartUp.tableList);
                SmLib.SysFunc.LoadIcon(_FrmTim);
                _FrmTim.Closed += new EventHandler(_FrmTim_Closed);
                _FrmTim.ShowDialog();
                
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        void _FrmTim_Closed(object sender, EventArgs e)
        {
            IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
        }
        #endregion

        #region V_In
        private void V_In()
        {
            FrmPrintInctpxd oReport = new FrmPrintInctpxd();
            oReport.DsPrint = StartUp.DsTrans.Copy();
            oReport.DsPrint.Tables[0].TableName = "TablePH";
            oReport.DsPrint.Tables[1].TableName = "TableCT";

            DataColumn newcolumn = new DataColumn("so_lien", typeof(int));
            newcolumn.DefaultValue = 1;
            oReport.DsPrint.Tables["TablePH"].Columns.Add(newcolumn);

            newcolumn = new DataColumn("so_ct_goc", typeof(int));
            newcolumn.DefaultValue = 0;
            oReport.DsPrint.Tables["TablePH"].Columns.Add(newcolumn);

            string stt_rec = oReport.DsPrint.Tables["TablePH"].Rows[iRow]["stt_rec"].ToString();

            int so_dong_in = Convert.ToInt16(StartUp.DmctInfo["so_dong_in"]);
            int rowCountCT = StartUp.DsTrans.Tables[1].DefaultView.Count;

            //Thêm số dòng cho đủ ngầm định
            if (rowCountCT < so_dong_in)
            {

                for (int k = rowCountCT; k < so_dong_in; k++)
                {
                    DataRow row = oReport.DsPrint.Tables["TableCT"].NewRow();
                    row["stt_rec"] = stt_rec;
                    row["stt_rec0"] = "999";
                    oReport.DsPrint.Tables["TableCT"].Rows.Add(row);
                }

            }

            oReport.DsPrint.Tables["TablePH"].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";
            oReport.DsPrint.Tables["TableCT"].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";
            oReport.DsPrint.Tables["TableCT"].DefaultView.Sort = "stt_rec0";

            oReport.DsPrint.Tables.Add(StartUp.GetDmnt().Copy());
            oReport.DsPrint.Tables.Add(CreateTableInfo().Copy());
           
            oReport.ShowDialog();
        }

        #region CreateTableInfo
        DataTable CreateTableInfo()
        {
            DataTable dt = new DataTable();
            dt.TableName = "TableInfo";
            //DataColumn newcolumn = new DataColumn("M_MA_THUE", typeof(string));
            //newcolumn.DefaultValue = StartUp.M_MA_THUE;
            //dt.Columns.Add(newcolumn);

            DataColumn newcolumn = new DataColumn("M_PHONE", typeof(string));
            newcolumn.DefaultValue = StartUp.M_PHONE;
            dt.Columns.Add(newcolumn);

            DataRow dr = dt.NewRow();
            dt.Rows.Add(dr);
            return dt;
        }
        #endregion

   
        #endregion

        #region IsVisibilityFieldsXamDataGrid
        void IsVisibilityFieldsXamDataGrid(string ma_nt)
        {
            if (currActionTask != ActionTask.Add)
            {
                UpdateTonKho();
            }
            IsVisibilityFieldsXamDataGridByMa_NT(ma_nt);
            IsVisibilityFieldsXamDataGridBySua_Tien();
            IsVisibilityFieldsXamDataGridByPx_gia_dd();
           
        }
        #region IsVisibilityFieldsXamDataGridByMa_NT
        void IsVisibilityFieldsXamDataGridByMa_NT(string ma_nt)
        {
            //Nếu ngoại tệ = tiền hoạch toán
            if (ma_nt == StartUp.M_ma_nt0)
            {
                //GrdCt không hiển thị 
                GrdCt.FieldLayouts[0].Fields["gia"].Visibility = Visibility.Hidden;
                GrdCt.FieldLayouts[0].Fields["tien"].Visibility = Visibility.Hidden;
               

                GrdCt.FieldLayouts[0].Fields["gia"].Settings.CellMaxWidth = 0;
                GrdCt.FieldLayouts[0].Fields["tien"].Settings.CellMaxWidth = 0;
               
                
            }
            else
            {
                //GrdCt hiển thị 
                GrdCt.FieldLayouts[0].Fields["gia"].Visibility = Visibility.Visible;
                GrdCt.FieldLayouts[0].Fields["tien"].Visibility = Visibility.Visible;
               

                GrdCt.FieldLayouts[0].Fields["gia"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["gia"].Width.Value.Value;
                GrdCt.FieldLayouts[0].Fields["tien"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["tien"].Width.Value.Value;
                
            }
            Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
            Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
           // Debug.WriteLine("============= changlanguage ==============");
            //ChangeLanguage(StartUp.M_LAN, GrdCt, true);
            //Dispatcher.Invoke(new Action(delegate()
            //{
            //    try
            //    {
            //        LanguageProvider.ChangeLanguage(GrdCt as Visual, LanguageID.Trim() + ".TabInfo.tabItemHT", StartUp.M_LAN, false);
            //    }
            //    catch (Exception)
            //    {
            //    }
            //   // ChangeLanguage();

            //}), DispatcherPriority.Background, new object[] { });
            ChangeLanguage();
        }
        #endregion

        #region IsVisibilityFieldsXamDataGridBySua_Tien
        void IsVisibilityFieldsXamDataGridBySua_Tien()
        {
            IsCheckedSua_tien.Value = ChkSua_tien.IsChecked.Value;
        }
        #endregion

        #region IsVisibilityFieldsXamDataGridByPx_gia_dd
        void IsVisibilityFieldsXamDataGridByPx_gia_dd()
        {
            IsCheckedPx_gia_dd.Value = ChkPx_gia_dd.IsChecked.Value;
        }
        #endregion
        #endregion

        #region txtma_gd_PreviewLostFocus
        private void txtma_gd_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (IsInEditMode.Value == true)
            {
                if (txtma_gd.RowResult == null || string.IsNullOrEmpty(txtma_gd.Text.Trim()))
                    return;
                txtten_gd.Text = StartUp.M_LAN.Equals("V")?txtma_gd.RowResult["ten_gd"].ToString().Trim():txtma_gd.RowResult["ten_gd2"].ToString().Trim();
                    
            }
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
                    var ten_kh = txtMa_kh.RowResult["ten_kh"].ToString().Trim();
                    var ten_kh2 = txtMa_kh.RowResult["ten_kh2"].ToString().Trim();
                    txtTen_kh.Text = StartUp.M_LAN.Equals("V") ? ten_kh : ten_kh2;
                    StartUp.DsTrans.Tables[0].Rows[iRow]["ten_kh"] = ten_kh;
                    StartUp.DsTrans.Tables[0].Rows[iRow]["ten_kh2"] = ten_kh2;
                    StartUp.DsTrans.Tables[0].AcceptChanges();
                    //if (StartUp.M_ong_ba.Equals("1") || (StartUp.M_ong_ba.Equals("0") && !string.IsNullOrEmpty(txtMa_kh.RowResult["doi_tac"].ToString().Trim())))
                    //    StartUp.DsTrans.Tables[0].DefaultView[0]["ong_ba"] = txtMa_kh.RowResult["doi_tac"].ToString().Trim();

                    if (!string.IsNullOrEmpty(txtMa_kh.RowResult["doi_tac"].ToString().Trim()))//143758019
                    {
                        StartUp.DsTrans.Tables[0].DefaultView[0]["ong_ba"] = txtMa_kh.RowResult["doi_tac"].ToString().Trim();
                    }

                    StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nx"] = txtMa_kh.RowResult["tk"].ToString().Trim();

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
            if (!txtNgay_ct.IsFocusWithin && IsInEditMode.Value )
            {
                //if (!SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, Convert.ToDateTime(txtNgay_ct.dValue)))
                //{
                //    ExMessageBox.Show( 1040,StartUp.SysObj, "Ngày hạch toán phải sau ngày khóa sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                //    txtNgay_ct.Value = DateTime.Now.Date;
                //}
                if (StartUp.M_ngay_lct.Equals("0") || string.IsNullOrEmpty(txtNgay_lct.Text))
                    txtNgay_lct.Value = txtNgay_ct.Value;
                
            }
        }
        #endregion

        #region txtngay_lct_LostFocus
        private void txtNgay_lct_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!txtNgay_lct.IsFocusWithin && IsInEditMode.Value && txtNgay_lct.IsValueValid)
            {
                if (txtNgay_ct.Value.ToString() != txtNgay_lct.Value.ToString())
                {
                    ExMessageBox.Show( 1045,StartUp.SysObj, "Ngày lập chứng từ khác với ngày hạch toán!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        #endregion

        #region txtMa_qs_PreviewLostFocus
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
                    if (CheckValidSoct(StartUp.SysObj, txtMa_qs.Text, txtSo_ct.Text, StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()))
                    {
                        txtSo_ct.Text = GetNewSoct(StartUp.SysObj, txtMa_qs.Text);
                        StartUp.DsTrans.Tables[0].DefaultView[0]["so_cttmp"] = txtSo_ct.Text;
                        StartUp.DsTrans.Tables[0].DefaultView[0]["ma_qstmp"] = txtMa_qs.Text;
                    }
                }
        }
        #endregion

        #region Mã ngoại tệ lostfocus
        private void txtMa_nt_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Voucher_Ma_nt0 == null)
                return;
            //if (txtMa_nt.IsDataChanged)
            //{
                IsVisibilityFieldsXamDataGridByMa_NT(txtMa_nt.Text.Trim());
                if (txtMa_nt.RowResult != null)
                {
                    StartUp.DsTrans.Tables[0].DefaultView[0]["loai_tg"] = txtMa_nt.RowResult["loai_tg"];
                    if (txtMa_nt.Text.Trim().Equals(StartUp.M_ma_nt0.Trim()))
                    {
                        txtTy_gia.Value = 1;
                    }
                    else //if (currActionTask != ActionTask.Edit)
                    {
                        txtTy_gia.Value = StartUp.GetRates(txtMa_nt.Text.Trim(), Convert.ToDateTime(txtNgay_ct.Value).Date);
                    }
                }
                Ty_gia_ValueChanged(true);
            //}
        }

        #endregion

        #region Tỷ giá
        private void txtTy_gia_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtTy_gia.Value == DBNull.Value)
            {
                txtTy_gia.Value = 0;
            }
            if (txtTy_gia.OldValue == Convert.ToDecimal(txtTy_gia.Value))
                return;
            Ty_gia_ValueChanged(false);

        }

        void Ty_gia_ValueChanged(bool IsMa_ntChanged)
        {
            if (currActionTask == ActionTask.Delete || currActionTask == ActionTask.View)
                //|| txtTy_gia.OldValue == Convert.ToDecimal(txtTy_gia.Value)
                return;
            Ty_Gia_ValueChange.Value = (txtTy_gia.OldValue != Convert.ToDecimal(txtTy_gia.Value));
            
            decimal _ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);
            int sua_tien = ParseInt(StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"], 0);
            
            //Hạch toán thay đổi
           // if (sua_tien == 0 && _ty_gia != 0)
            if (_ty_gia != 0)
            {
                //ma_nt thay doi thi updatetotal or ty gia thay doi và sua_tien = 0 thì updatetotal
                if (sua_tien == 0 || IsMa_ntChanged)
                {
                    UpdateTotal("gia", "gia_nt", true);
                    UpdateTotal("tien", "tien_nt", false);
                    StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = SumFunction(StartUp.DsTrans.Tables[1], "tien", 0);
                }
            }
           
        }
        #endregion

        #region UpdateTotal
        void UpdateTotal(string columnname, string columnname_nt, bool isPrice)
        {
            decimal ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);
            for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
            {
                decimal tien_nt = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i][columnname_nt], 0);
                decimal tien = SysFunc.Round(tien_nt * ty_gia, isPrice == false ? StartUp.M_ROUND : StartUp.M_ROUND_GIA);
                if (tien != 0)
                    StartUp.DsTrans.Tables[1].DefaultView[i][columnname] = tien;
            }
        }
        #endregion

        #region txtTy_gia_GotFocus
       
        private void txtTy_gia_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Voucher_Ma_nt0.Value)
            {
                KeyboardNavigation.SetTabNavigation(GrNT, KeyboardNavigationMode.Continue);
                SmLib.WinAPISenkey.SenKey(ModifierKeys.None, Key.Tab);
            }
        }
        #endregion

        #region Check sửa tiền
        private void ChkSua_tien_Click(object sender, RoutedEventArgs e)
        {
            IsVisibilityFieldsXamDataGridBySua_Tien();
            if (ChkSua_tien.IsChecked == false && sender.GetType().Name.Equals("CheckBox"))
            {
                UpdateTotalChkSua_tien();
                //txtTy_gia.Focus();
                Ty_gia_ValueChanged(false);
            }
        }

        #endregion

        #region UpdateTotalChkSua_tien
        void UpdateTotalChkSua_tien()
        {
            int countCT = StartUp.DsTrans.Tables[1].DefaultView.Count;
            if (countCT > 0)
            {
                decimal so_luong, gia_nt;
                for (int i = 0; i < countCT; i++)
                {
                    so_luong = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["so_luong"], 0);
                    gia_nt = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["gia_nt"], 0);
                    if (so_luong != 0 && gia_nt != 0)
                    {
                        StartUp.DsTrans.Tables[1].DefaultView[i]["tien_nt"] = SysFunc.Round(so_luong * gia_nt, StartUp.M_ROUND_NT);
                    }
                }
                //tinh lai tong tien von
                StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt"] = SumFunction(StartUp.DsTrans.Tables[1], "tien_nt", 0);

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

        #region GrdCt_PreviewEditModeEnded
        private void GrdCt_PreviewEditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            try
            {
                if (IsInEditMode.Value == false)
                    return;

                if (GrdCt.ActiveCell != null && StartUp.DsTrans.Tables[1].GetChanges(DataRowState.Deleted) == null)
                {
                    decimal ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);                    
                    int sua_tien = ParseInt(StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"], 0);

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
                                    if (e.Cell.IsDataChanged)
                                    {


                                        //var dtCT = (from ct in StartUp.DsTrans.Tables[1].AsEnumerable()
                                        //            where ct["stt_rec"].ToString().Trim() == StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString().Trim()
                                        //            && ct["ma_vt"].ToString().Trim()==txt.RowResult["ma_vt"].ToString().Trim()
                                        //            select new
                                        //            {
                                        //                gia_nt = ParseDecimal(ct["gia_nt"], 0),
                                        //                gia = ParseDecimal(ct["gia"], 0),

                                        //            });
                                        //if (dtCT.Count() > 0)
                                        //{
                                        //    e.Cell.Record.Cells["gia_nt"].Value = dtCT.First().gia_nt;
                                        //    e.Cell.Record.Cells["gia"].Value = dtCT.First().gia;
                                        //}
                                        //134400343
                                        if (e.Cell.Record.Cells["ma_nx_i"].Value.ToString().Trim() == "")
                                            e.Cell.Record.Cells["ma_nx_i"].Value = txt.RowResult["tk_nvl"];

                                        e.Cell.Record.Cells["gia_ton"].Value = txt.RowResult["gia_ton"];
                                        e.Cell.Record.Cells["vt_ton_kho"].Value = txt.RowResult["vt_ton_kho"];
                                        //neu theo doi ton kho = 0 thi
                                        // so_luong = 0, gia = 0 (anhbtn, khaibl - 08-10-2010)
                                        if (ParseInt(txt.RowResult["vt_ton_kho"], 0) == 0)
                                        {
                                            e.Cell.Record.Cells["so_luong"].Value = 0;
                                            StartUp.DsTrans.Tables[0].Rows[iRow]["t_so_luong"] = SumFunction(StartUp.DsTrans.Tables[1], "so_luong", 0);

                                            e.Cell.Record.Cells["gia_nt"].Value = 0;
                                            e.Cell.Record.Cells["gia"].Value = 0;

                                            e.Cell.Record.Cells["tien_nt"].Value = 0;
                                            StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt"] = SumFunction(StartUp.DsTrans.Tables[1], "tien_nt", 0);

                                            e.Cell.Record.Cells["tien"].Value = 0;
                                            StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = SumFunction(StartUp.DsTrans.Tables[1], "tien", 0);
                                        }

                                        DataRowView drVCT = e.Cell.Record.DataItem as DataRowView;
                                        drVCT["sua_tk_vt"] = txt.RowResult["sua_tk_vt"];
                                        drVCT["tk_vt_dmvt"] = txt.RowResult["tk_vt"].ToString();
                                        drVCT["sl_min"] = txt.RowResult["sl_min"];
                                        //Lấy tk vật tư
                                        if (txt.RowResult["tk_vt"].ToString() != "")
                                            e.Cell.Record.Cells["tk_vt"].Value = txt.RowResult["tk_vt"].ToString();

                                        CellValuePresenter cell_Kho = CellValuePresenter.FromCell(e.Cell.Record.Cells["ma_kho_i"]);
                                        AutoCompleteTextBox autoCompleteKho = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(cell_Kho.Editor as ControlHostEditor);
                                        if (autoCompleteKho != null)
                                        {
                                            autoCompleteKho.SearchInit();
                                            if (autoCompleteKho.RowResult != null)
                                            {
                                                if (autoCompleteKho.RowResult["tk_dl"] != DBNull.Value && !string.IsNullOrEmpty(autoCompleteKho.RowResult["tk_dl"].ToString().Trim()))
                                                {
                                                    e.Cell.Record.Cells["tk_vt"].Value = autoCompleteKho.RowResult["tk_dl"].ToString();
                                                    drVCT["tk_vt_dmvt"] = autoCompleteKho.RowResult["tk_dl"].ToString();
                                                }
                                            }
                                        }

                                        //Update Binding
                                        CellValuePresenter cellV = CellValuePresenter.FromCell(e.Cell.Record.Cells["tk_vt"]);
                                        ControlFunction.RefreshSingleBinding(cellV, AutoCompleteTextBox.IsReadOnlyProperty);
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
                                }
                            }
                            break;
                        #endregion

                        #region ma_kho_i
                        case "ma_kho_i":
                            {
                                if (e.Editor.Value == null)
                                    return;
                                if (e.Cell.IsDataChanged)
                                {
                                    DataRowView drVCT = e.Cell.Record.DataItem as DataRowView;
                                    CellValuePresenter cell_VT = CellValuePresenter.FromCell(e.Cell.Record.Cells["ma_vt"]);
                                    AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(cell_VT.Editor as ControlHostEditor);
                                    if (txt.RowResult == null)
                                        txt.SearchInit();
                                    if (txt.RowResult != null)
                                    {
                                        if (txt.RowResult["sua_tk_vt"] != DBNull.Value && Convert.ToDecimal(txt.RowResult["sua_tk_vt"]) == 0)
                                        {
                                            e.Cell.Record.Cells["tk_vt"].Value = txt.RowResult["tk_vt"].ToString();
                                            drVCT["tk_vt_dmvt"] = txt.RowResult["tk_vt"].ToString();

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
                                                if (autoCompleteKho.RowResult["tk_dl"].ToString() != "")
                                                    e.Cell.Record.Cells["tk_vt"].Value = autoCompleteKho.RowResult["tk_dl"].ToString();
                                                drVCT["tk_vt_dmvt"] = autoCompleteKho.RowResult["tk_dl"].ToString();

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
                                }
                            }
                            break;
                        #endregion

                        #region so luong
                        case "so_luong":
                            {
                                if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                    e.Cell.Record.Cells["so_luong"].Value = 0;

                                decimal so_luong = ParseDecimal(e.Cell.Record.Cells["so_luong"].Value, 0);
                                AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(CellValuePresenter.FromCell(e.Cell.Record.Cells["ma_vt"]).Editor as ControlHostEditor);
                                if (txt.RowResult != null && txt.RowResult["gia_ton"] != DBNull.Value)
                                    if (int.Parse(txt.RowResult["gia_ton"].ToString()) == 3)
                                        if (so_luong == 0)
                                        {
                                            ExMessageBox.Show( 1050,StartUp.SysObj, "Vật tư tính tồn kho theo phương pháp NTXT không được nhập số lượng = 0!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                            return;
                                        }

                                if (e.Cell.IsDataChanged)
                                {
                                    StartUp.DsTrans.Tables[0].Rows[iRow]["t_so_luong"] = SumFunction(StartUp.DsTrans.Tables[1], "so_luong", 0);
                                    
                                    //Xuat ban khong co so luong thi nhat dinh khong co gia von
                                    if (so_luong == 0)
                                    {
                                        e.Cell.Record.Cells["gia_nt"].Value = 0;
                                        e.Cell.Record.Cells["gia"].Value = 0;
                                       
                                    }

                                    decimal gia_nt = ParseDecimal(e.Cell.Record.Cells["gia_nt"].Value, 0);

                                    // Neu ko check sua tien, gia_nt*ty_gia != 0 va so_luong != 0
                                    // thì gia = gia_nt*ty_gia
                                    decimal gia = SysFunc.Round(gia_nt * ty_gia, StartUp.M_ROUND_GIA);
                                    //if (sua_tien == 0 && so_luong != 0 && gia != 0)
                                    if (so_luong != 0 && gia != 0)
                                    {
                                        e.Cell.Record.Cells["gia"].Value = gia;
                                    }

                                    //Co the nhap Tien =Gia*So_luong
                                    decimal tien_nt = SysFunc.Round(so_luong * gia_nt, StartUp.M_ROUND_NT);
                                    //if (sua_tien == 0 && tien_nt != 0)
                                    if ( tien_nt != 0)
                                    {
                                        e.Cell.Record.Cells["tien_nt"].Value = tien_nt;
                                        //tinh lai tong tien hang, tong thue, tong tien sau ck
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt"] = SumFunction(StartUp.DsTrans.Tables[1], "tien_nt", 0);
                                    }

                                    // Neu ko check sua tien, tien_nt*ty_gia != 0 
                                    // thì tien = tien_nt*ty_gia
                                    decimal tien = SysFunc.Round(tien_nt * ty_gia, StartUp.M_ROUND);
                                    //if (sua_tien == 0 && tien != 0)
                                    if (tien != 0)
                                    {
                                        e.Cell.Record.Cells["tien"].Value = tien;
                                        //tinh lai tong tien hang
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = SumFunction(StartUp.DsTrans.Tables[1], "tien", 0);
                                    }

                                    if (txtMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["gia"].Value = e.Cell.Record.Cells["gia_nt"].Value;
                                        e.Cell.Record.Cells["tien"].Value = e.Cell.Record.Cells["tien_nt"].Value;
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt"];
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region gia nt
                        case "gia_nt":
                            {
                               
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                        e.Cell.Record.Cells["gia_nt"].Value = 0;

                                    decimal so_luong = ParseDecimal(e.Cell.Record.Cells["so_luong"].Value, 0);
                                    decimal gia_nt = ParseDecimal(e.Cell.Record.Cells["gia_nt"].Value, 0);

                                    // Neu ko check sua tien, gia_nt*ty_gia != 0 va so_luong != 0
                                    // thì gia = gia_nt*ty_gia
                                    decimal gia = SysFunc.Round(gia_nt * ty_gia, StartUp.M_ROUND_GIA);
                                    //if (sua_tien == 0 && so_luong != 0 && gia != 0)
                                    if (so_luong != 0 && gia != 0)
                                    {
                                        e.Cell.Record.Cells["gia"].Value = gia;
                                    }

                                    //Co the nhap Tien =Gia*So_luong
                                    decimal tien_nt = SysFunc.Round(so_luong * gia_nt, StartUp.M_ROUND_NT);
                                    //if (sua_tien == 0 && tien_nt != 0)
                                    if (tien_nt != 0)
                                    {
                                        e.Cell.Record.Cells["tien_nt"].Value = tien_nt;
                                        //tinh lai tong tien hang
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt"] = SumFunction(StartUp.DsTrans.Tables[1], "tien_nt", 0);
                                    }
                                    
                                    // Neu ko check sua tien, tien_nt*ty_gia != 0 
                                    // thì tien = tien_nt*ty_gia
                                    decimal tien = SysFunc.Round(tien_nt * ty_gia, StartUp.M_ROUND);
                                    //if (sua_tien == 0 && tien != 0)
                                    if (tien != 0)
                                    {
                                        e.Cell.Record.Cells["tien"].Value = tien;
                                        //tinh lai tong tien hang
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = SumFunction(StartUp.DsTrans.Tables[1], "tien", 0);
                                    }

                                    if (txtMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["gia"].Value = e.Cell.Record.Cells["gia_nt"].Value;
                                        e.Cell.Record.Cells["tien"].Value = e.Cell.Record.Cells["tien_nt"].Value;
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt"];
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region tien nt
                        case "tien_nt":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                        e.Cell.Record.Cells["tien_nt"].Value = 0;

                                    decimal tien_nt = ParseDecimal(e.Cell.Record.Cells["tien_nt"].Value, 0);
                                    //tinh lai tong tien hang
                                    StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt"] = SumFunction(StartUp.DsTrans.Tables[1], "tien_nt", 0);

                                    decimal tien = SmLib.SysFunc.Round(tien_nt * ty_gia, StartUp.M_ROUND);
                                    //if (tien != 0 && sua_tien == 0)
                                    if (tien != 0)
                                    {
                                        e.Cell.Record.Cells["tien"].Value = tien;
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = SumFunction(StartUp.DsTrans.Tables[1], "tien", 0);
                                    }

                                    if (txtMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["tien"].Value = e.Cell.Record.Cells["tien_nt"].Value;
                                        StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt"];
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region tien
                        case "tien":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                        e.Cell.Record.Cells["tien"].Value = 0;
                                    StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = SumFunction(StartUp.DsTrans.Tables[1], "tien", 0);
                                }

                            }
                            break;
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
            return NewRowCt();
        }

        bool NewRowCt()
        {
            try
            {
                DataRow NewCtRecord = StartUp.DsTrans.Tables[1].NewRow();
                NewCtRecord["stt_rec"] = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"];
                int Stt_rec0 = 1;
                var vtien_nt0_hd = StartUp.DsTrans.Tables[1].AsEnumerable()
                                .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                                .Max(x => x.Field<string>("stt_rec0"));
                if (vtien_nt0_hd != null)
                {
                    int.TryParse(vtien_nt0_hd.ToString(), out Stt_rec0);
                    Stt_rec0++;
                    NewCtRecord["ma_kho_i"] = (GrdCt.Records[GrdCt.Records.Count - 1] as DataRecord).Cells["ma_kho_i"].Value;
                }
                NewCtRecord["stt_rec0"] = string.Format("{0:000}", Stt_rec0);
                NewCtRecord["ma_ct"] = StartUp.Ma_ct;
                NewCtRecord["so_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"];
                NewCtRecord["ngay_ct"] = txtNgay_ct.Value == null ? DateTime.Now.Date : txtNgay_ct.dValue.Date;
                NewCtRecord["so_luong"] = 0;
                NewCtRecord["gia_nt"] = 0;
                NewCtRecord["tien_nt"] = 0;
                NewCtRecord["gia"] = 0;
                NewCtRecord["tien"] = 0;

                NewCtRecord["ton13"] = DBNull.Value;
              
                int count = StartUp.DsTrans.Tables[1].DefaultView.Count;
                if (count > 0)
                {
                    NewCtRecord["ma_kho_i"] = StartUp.DsTrans.Tables[1].DefaultView[count - 1].Row["ma_kho_i"];
                    NewCtRecord["ma_nx_i"] = StartUp.DsTrans.Tables[1].DefaultView[count - 1].Row["ma_nx_i"];

                    SqlCommand cmd = new SqlCommand("select ma_dm from dmctct where ma_ct=@ma_ct and ma_dm=@ma_dm");
                    cmd.Parameters.Add("@ma_ct", SqlDbType.Char).Value = StartUp.Ma_ct;
                    cmd.Parameters.Add("@ma_dm", SqlDbType.Char).Value = "dmvv";

                    //if (StartUp.SysObj.ExcuteReader(cmd).Tables[0].Rows.Count > 0)
                    //{
                    //    NewCtRecord["ma_vv_i"] = StartUp.DsTrans.Tables[1].DefaultView[count - 1].Row["ma_vv_i"];
                    //}
                }
                else
                {
                    NewCtRecord["ma_nx_i"] = StartUp.DsTrans.Tables[0].Rows[iRow]["ma_nx"];
                }
                FreeCodeFieldLib.CarryFreeCodeFields(StartUp.SysObj, StartUp.Ma_ct, StartUp.DsTrans.Tables[1].DefaultView, NewCtRecord, 1);    
                StartUp.DsTrans.Tables[1].Rows.Add(NewCtRecord);

                return true;
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
                return false;
            }
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

        #region GrdCt_KeyDown
        private void GrdCt_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsInEditMode.Value == false)
                return;
            if (Keyboard.IsKeyDown(Key.N) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                NewRowCt();
                GrdCt.ActiveRecord = GrdCt.Records[GrdCt.Records.Count - 1];
            }
            if (Keyboard.IsKeyDown(Key.Tab) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                GrdCt.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);
                (this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus();
            }
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
                    }
                    break;

                #region F5
                case Key.F5:
                    {
                        if (GrdCt.ActiveRecord != null)
                        {
                            CellValuePresenter cellV = CellValuePresenter.FromCell((GrdCt.ActiveRecord as DataRecord).Cells["ma_vt"]);
                            if (cellV != null)
                            {
                                ControlHostEditor controlHost = cellV.Editor as ControlHostEditor;
                                if (controlHost != null)
                                {
                                    AutoCompleteTextBox txt = ControlFunction.GetAutoCompleteControl(controlHost);
                                    if (string.IsNullOrEmpty(txt.Text.Trim()))
                                    {
                                        ExMessageBox.Show( 1055,StartUp.SysObj, "Chưa nhập mã vật tư!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                        return;
                                    }
                                    if (txt != null)
                                    {
                                        if (txt.CheckLostFocus())
                                        {
                                            string ma_vt = (GrdCt.ActiveRecord as DataRecord).Cells["ma_vt"].Value.ToString();
                                            string ten_vt = "";
                                            if (StartUp.M_LAN.Equals("V"))
                                            {
                                                ten_vt = ((GrdCt.ActiveRecord as DataRecord).DataItem as DataRowView)["ten_vt"].ToString();
                                            }
                                            else
                                            {
                                                ten_vt = ((GrdCt.ActiveRecord as DataRecord).DataItem as DataRowView)["ten_vt2"].ToString();
                                            }
                                            string ma_kho = (GrdCt.ActiveRecord as DataRecord).Cells["ma_kho_i"].Value.ToString();
                                            object ngay_ct = StartUp.DsTrans.Tables[0].Rows[iRow]["ngay_ct"];

                                            DataTable tb = StartUp.GetPN(ma_vt, ma_kho, ngay_ct);
                                            if (tb.Rows.Count > 0)
                                            {
                                                FrmInctpxd_Pn inctpxd_pn = new FrmInctpxd_Pn(tb, ten_vt);
                                                inctpxd_pn.ShowDialog();

                                                int currRow = 0;
                                                currRow = GrdCt.ActiveRecord.Index;
                                                DataRowView drvFrmPn;
                                                if (currRow >= 0 && currRow < GrdCt.Records.Count )
                                                {
                                                    drvFrmPn = inctpxd_pn.drvFrmINCTPXD_PN;
                                                    if (drvFrmPn != null)
                                                    {
                                                        if (StartUp.DsTrans.Tables[0].Rows[iRow]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0))
                                                            (GrdCt.DataSource as DataView)[currRow]["gia_nt"] = drvFrmPn["gia"];
                                                        else
                                                            (GrdCt.DataSource as DataView)[currRow]["gia_nt"] = drvFrmPn["gia_nt"];
                                                        (GrdCt.DataSource as DataView)[currRow]["gia"] = drvFrmPn["gia"];

                                                        if (ParseInt((GrdCt.DataSource as DataView)[currRow]["gia_ton"], 0) == 1 || ParseInt((GrdCt.DataSource as DataView)[currRow]["gia_ton"], 0) == 4)
                                                            StartUp.DsTrans.Tables[0].Rows[iRow]["px_gia_dd"] = 1;

                                                        decimal ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);
                                                        int sua_tien = ParseInt(StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"], 0);
                                                        decimal so_luong = ParseDecimal((GrdCt.DataSource as DataView)[currRow]["so_luong"], 0);
                                                        decimal gia_nt = ParseDecimal((GrdCt.DataSource as DataView)[currRow]["gia_nt"], 0);
                                                        decimal gia = ParseDecimal((GrdCt.DataSource as DataView)[currRow]["gia"], 0);

                                                        //Co the nhap Tien =Gia*So_luong
                                                        decimal tien_nt = SysFunc.Round(so_luong * gia_nt, StartUp.M_ROUND_NT);
                                                        //if (sua_tien == 0 && tien_nt != 0)
                                                        if (tien_nt != 0)
                                                        {
                                                            (GrdCt.DataSource as DataView)[currRow]["tien_nt"] = tien_nt;
                                                            StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt"] = SumFunction(StartUp.DsTrans.Tables[1], "tien_nt", 0);
                                                        }

                                                        // Neu ko check sua tien, tien_nt*ty_gia != 0 
                                                        // thì tien = tien_nt*ty_gia
                                                        decimal tien = SysFunc.Round(tien_nt * ty_gia, StartUp.M_ROUND);
                                                        //sua lai tien = gia * so_luong
                                                        //decimal tien = SysFunc.Round(gia * so_luong, StartUp.M_ROUND);
                                                        //if (sua_tien == 0 && tien != 0)
                                                        if (tien != 0)
                                                        {
                                                            (GrdCt.DataSource as DataView)[currRow]["tien"] = tien;
                                                            StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = SumFunction(StartUp.DsTrans.Tables[1], "tien", 0);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                ExMessageBox.Show( 1060,StartUp.SysObj, "Không có phiếu nhập cho vật tư này!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            ExMessageBox.Show( 1065,StartUp.SysObj, "Không có phiếu nhập cho vật tư này!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                            return;
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    }
                #endregion

                #region F8
                case Key.F8:
                    {
                        if (ExMessageBox.Show( 1070,StartUp.SysObj, "Có xoá dòng ghi hiện thời?", "Xoá dòng", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                        {
                            return;
                        }

                        DataRecord record = (GrdCt.ActiveRecord as DataRecord);
                        if (record != null)
                        {
                            int indexCell = GrdCt.ActiveCell == null ? 0 : GrdCt.ActiveCell.Field.Index;
                            int indexRecord = record.Index;
                            GrdCt.ExecuteCommand(DataPresenterCommands.EndEditModeAndDiscardChanges);
                            if (indexCell >= 0)
                            {
                                StartUp.DsTrans.Tables[1].Rows.Remove(StartUp.DsTrans.Tables[1].DefaultView[indexRecord].Row);
                                StartUp.DsTrans.Tables[1].AcceptChanges();
                                //tinh lai tong tien
                                StartUp.DsTrans.Tables[0].Rows[iRow]["t_so_luong"] = SumFunction(StartUp.DsTrans.Tables[1], "so_luong", 0);
                                StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien_nt"] = SumFunction(StartUp.DsTrans.Tables[1], "tien_nt", 0);
                                StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = SumFunction(StartUp.DsTrans.Tables[1], "tien", 0);
                              

                                if (indexRecord == 0 && GrdCt.Records.Count == 0)
                                    GrdCt_AddNewRecord(null, null);
                                if (GrdCt.Records.Count > 0)
                                    GrdCt.ActiveRecord = GrdCt.Records[indexRecord > GrdCt.Records.Count - 1 ? GrdCt.Records.Count - 1 : indexRecord];

                                
                                   
                            }
                        }
                    }
                    break;
                #endregion

                default:
                    break;
            }
        }
        #endregion

        #region SumFunction
        decimal SumFunction(DataTable datatable, string columnname, int km)
        {
            decimal result = 0;
            //decimal.TryParse(datatable.Compute("sum(" + columnname + ")", "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'").ToString(), out result);
            var SumTotal = datatable.AsEnumerable()
                        .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() )
                        .Sum(x => x.Field<decimal?>(columnname));
            if (SumTotal != null)
                decimal.TryParse(SumTotal.ToString(), out result);
            return result;
        }
        #endregion

        #region GetLanguageString
        public override string GetLanguageString(string code, string language)
        {
            //string test = StartUp.GetLanguageString(code, language);
            //Debug.WriteLine(string.Format("{0}=>{1}", code, test));
            return StartUp.GetLanguageString(code, language);
        }
        #endregion

        #region ParseDecimal
        public decimal ParseDecimal(object obj, decimal defaultvalue)
        {
            decimal ketqua = defaultvalue;
            decimal.TryParse(obj != null ? obj.ToString() : defaultvalue.ToString(), out ketqua);
            return ketqua;
        }
        #endregion

        #region ChkPx_gia_dd_Click
        private void ChkPx_gia_dd_Click(object sender, RoutedEventArgs e)
        {
            IsVisibilityFieldsXamDataGridByPx_gia_dd();
        }
        #endregion

        #region txtt_tien_nt_ValueChanged
        private void txtt_tien_nt_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UpdateMoney_NT();
        }
        #endregion

        #region UpdateMoney_NT
        void UpdateMoney_NT()
        {
            decimal ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);
            int sua_tien = ParseInt(StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"], 0);

            decimal t_tien_nt = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt"], 0);

            decimal t_tien = SysFunc.Round(t_tien_nt * ty_gia, StartUp.M_ROUND);
            //if (t_tien != 0 && sua_tien == 0)
            if (t_tien != 0)
                StartUp.DsTrans.Tables[0].Rows[iRow]["t_tien"] = t_tien;
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

        #region FormMain_Closed
        private void FormMain_Closed(object sender, EventArgs e)
        {
            //if currActinTask đang là none thì thoát luôn
            //nếu không phải là None thì Hủy phiếu đang thực hiện
            if (currActionTask == ActionTask.None || IsInEditMode.Value == false)
                return;
            V_Huy();
        } 
        #endregion

        private void FormMain_EditModeEnded(object sender, string menuItemName, RoutedEventArgs e)
        {
            if (!menuItemName.Equals("btnSave"))
            {
                UpdateTonKho();
            }
        }

        
    }
}
