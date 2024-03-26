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
using Sm.Windows.Controls;
using System.Data;
using SmVoucherLib;
using System.Data.SqlClient;
using Infragistics.Windows.DataPresenter;
using SmLib;
using System.Windows.Threading;
using System.Diagnostics;
using System.Threading;

namespace Glctpk1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FrmGlctpk1 : SmVoucherLib.FormTrans
    {
        public static int iRow = 0;
        int iRow_old = 0;
        DataSet dsCheckData;
        public static CodeValueBindingObject IsInEditMode;
        CodeValueBindingObject Voucher_Ma_nt0;
        CodeValueBindingObject IsCheckedSua_tien;
        CodeValueBindingObject Ty_Gia_ValueChange;
        CodeValueBindingObject M_Ngay_lct;
        CodeValueBindingObject Voucher_Lan0;
        //CodeValueBindingObject Voucher_Ma_nt0;
        //CodeValueBindingObject IsInEditMode;
        //CodeValueBindingObject IsCheckedSua_tien;
        //CodeValueBindingObject Ty_Gia_ValueChange;
        
        int user_id;

        public DataSet DsVitual;
        public FrmGlctpk1()
        {
            InitializeComponent();
            LanguageProvider.Language = StartUp.M_LAN;
            this.BindingSysObj = StartUp.SysObj;
            C_QS = txtMa_qs;
            C_NgayHT = txtNgay_ct;
            C_Ma_nt = txtMa_nt;
            C_So_ct = txtSo_ct;
        }

        #region FrmGlctpk1_Loaded
        void FrmGlctpk1_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                if (StartUp.DsTrans.Tables[0].Rows.Count > 1)
                    iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;
                
                IsInEditMode = (CodeValueBindingObject)FormMain.FindResource("IsInEditMode");
                Voucher_Ma_nt0 = (CodeValueBindingObject)FormMain.FindResource("Voucher_Ma_nt0");
                IsCheckedSua_tien = (CodeValueBindingObject)FormMain.FindResource("IsCheckedSua_tien");
                Ty_Gia_ValueChange = (CodeValueBindingObject)FormMain.FindResource("Ty_Gia_ValueChange");
                M_Ngay_lct = (CodeValueBindingObject)FormMain.FindResource("M_Ngay_lct");
                Voucher_Lan0 = (CodeValueBindingObject)FormMain.FindResource("Voucher_Lan0");
                M_Ngay_lct.Value = StartUp.M_Ngay_lct.Equals("1");

                user_id = Convert.ToInt16(StartUp.SysObj.UserInfo.Rows[0]["user_id"]);

                Binding bind = new Binding("Value");
                bind.Source = IsInEditMode;
                bind.Mode = BindingMode.OneWay;
                this.SetBinding(FormTrans.IsEditModeProperty, bind);

                //Gán ngôn ngữ messagebox
                GrdCt.Lan = StartUp.M_LAN;
                GrdCtgt.Lan = StartUp.M_LAN;
                M_LAN = StartUp.M_LAN;

                //Them cac truong tu do
                SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, GrdCt, StartUp.Ma_ct, 1);
                SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj,GrdCtgt, StartUp.Ma_ct,2);

                //load form theo stt_rec
                if (StartUp.DsTrans.Tables[0].Rows.Count > 0)
                {
                    StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString());

                    LoadData();
                    
                    IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
                    IsCheckedSua_tien.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"].ToString() == "1");
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

            //grid hd thue
            this.GrdCtgt.DataSource = StartUp.DsTrans.Tables[2].DefaultView;

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
            //filter Table[0], Table[1], Table[2]
            StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString());
            IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
        }
        #endregion

        #region V_Truoc
        void V_Truoc()
        {
            if (iRow > 1)
                iRow--;
            //filter Table[0], Table[1], Table[2]
            StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString());

            IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
        }
        #endregion

        #region V_Dau
        void V_Dau()
        {
            iRow = StartUp.DsTrans.Tables[0].Rows.Count > 1 ? 1 : 0;
            //filter Table[0], Table[1], Table[2]
            StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[iRow]["stt_rec"].ToString());
            IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
            
        }
        #endregion

        #region V_Cuoi
        void V_Cuoi()
        {
            iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;
            //filter Table[0], Table[1], Table[2]
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
                        if (txtNgay_ct.Value == null)
                        {
                            txtNgay_ct.Value = DateTime.Now.Date;
                           // txtngay_lct.Value = DateTime.Now;
                        }
                        //Them moi dong trong Ph
                        DataRow NewRecord = StartUp.DsTrans.Tables[0].NewRow();
                        NewRecord["stt_rec"] = newSttRec;
                        NewRecord["ma_ct"] = StartUp.Ma_ct;
                        NewRecord["row_id"] = 0;
                        if (SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, txtNgay_ct.dValue))
                        {
                            NewRecord["ngay_ct"] = txtNgay_ct.dValue.Date;//txtNgay_ct.Value == null ? DateTime.Now.Date : Convert.ToDateTime(txtNgay_ct.Value).Date;

                        }
                        else
                        {
                            NewRecord["ngay_ct"] = DateTime.Now.Date;
                        }
                        NewRecord["status"] = StartUp.DmctInfo["ma_post"];
                       // NewRecord["ma_nt"] = StartUp.M_ma_nt0;
                        if (StartUp.DsTrans.Tables[0].Rows.Count == 1)
                        {
                            NewRecord["ma_nt"] = StartUp.DmctInfo["ma_nt"];
                            NewRecord["ma_qs"] = GetDMQS(BindingSysObj, StartUp.Ma_ct, Convert.ToDateTime(NewRecord["ngay_ct"]), user_id);
                        }
                        else
                        {
                            NewRecord["ma_nt"] = StartUp.DsTrans.Tables[0].Rows[iRow]["ma_nt"];
                            NewRecord["ma_qs"] = GetDMQS(BindingSysObj, StartUp.Ma_ct, Convert.ToDateTime(NewRecord["ngay_ct"]),
                             user_id, StartUp.DsTrans.Tables[0].Rows[iRow]["ma_qs"].ToString().Trim());
                        }
                        NewRecord["sua_tien"] = 0;
                        //NewRecord["ty_gia"] = 1;
                        if (NewRecord["ma_nt"].ToString().Trim().Equals(StartUp.M_ma_nt0.Trim()))
                            NewRecord["ty_giaf"] = 1;
                        else
                            NewRecord["ty_giaf"] = StartUp.GetRates(NewRecord["ma_nt"].ToString().Trim(), Convert.ToDateTime(NewRecord["ngay_ct"]).Date);
                        
                        //Them moi dong trong Ct
                        DataRow NewCtRecord = StartUp.DsTrans.Tables[1].NewRow();
                        NewCtRecord["stt_rec"] = newSttRec;
                        NewCtRecord["stt_rec0"] = "001";
                        NewCtRecord["ma_ct"] = StartUp.Ma_ct;
                        NewCtRecord["ngay_ct"] = NewRecord["ngay_ct"];
                        NewCtRecord["ps_no_nt"] = 0;
                        NewCtRecord["ps_co_nt"] = 0;
                        NewCtRecord["ps_no"] = 0;
                        NewCtRecord["ps_co"] = 0;
                        NewCtRecord["nh_dk"] = "";
                        NewCtRecord["han_tt"] = 0;
                        StartUp.DsTrans.Tables[0].Rows.Add(NewRecord);
                        StartUp.DsTrans.Tables[1].Rows.Add(NewCtRecord);

                        iRow_old = iRow;
                        iRow = StartUp.DsTrans.Tables[0].Rows.Count - 1;

                        //filter lại Table[0], Table[1], Table[2]
                        StartUp.DataFilter(newSttRec);

                        IsInEditMode.Value = true;
                        IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
                        TabInfo.SelectedIndex = 0;
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            txtNgay_ct.Focus();
                        })); 
                    }
                //}
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
                ExMessageBox.Show( 450,StartUp.SysObj, "Không có dữ liệu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                if (StartUp.DsTrans.Tables[0].Rows.Count == 1)
                    return;
                currActionTask = ActionTask.Edit;
                DsVitual = new DataSet();
                //copy Table[0], Table[1], Table[2]
                DsVitual.Tables.Add(StartUp.DsTrans.Tables[0].DefaultView.ToTable().Copy());

                DsVitual.Tables.Add(StartUp.DsTrans.Tables[1].DefaultView.ToTable().Copy());

                DsVitual.Tables.Add(StartUp.DsTrans.Tables[2].DefaultView.ToTable().Copy());

                IsInEditMode.Value = true;

                TabInfo.SelectedIndex = 0;
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    txtNgay_ct.Focus();
                }));
            }
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

            if (StartUp.M_LAN != "V")
                _formcopy.Title = "Copy";

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
                            user_id, NewRecord["ma_qs"].ToString().Trim());
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
                    TabInfo.SelectedIndex = 0;
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        txtNgay_ct.Focus();
                    }));
                }
            }
        }
        #endregion

        #region V_In
        private void V_In()
        {
            FrmPrintGlctpk1 oReport = new FrmPrintGlctpk1();
            if (M_LAN != "V")
                oReport.Title = "Report list";

            oReport.DsPrint = StartUp.DsTrans.Copy();
            DataColumn newcolumn = new DataColumn("so_lien", typeof(int));
            newcolumn.DefaultValue = 1;
            oReport.DsPrint.Tables[0].Columns.Add(newcolumn);

            newcolumn = new DataColumn("so_ct_goc", typeof(int));
            newcolumn.DefaultValue = 0;
            oReport.DsPrint.Tables[0].Columns.Add(newcolumn);

            for (int r = 0; r < oReport.DsPrint.Tables[0].Rows.Count; r++)
            {
                if (oReport.DsPrint.Tables[0].Rows[r]["ma_nt"].ToString() == StartUp.M_ma_nt0)
                {
                    oReport.DsPrint.Tables[1].DefaultView.RowFilter = "stt_rec= '" + oReport.DsPrint.Tables[0].Rows[r]["stt_rec"].ToString() + "'";
                    oReport.DsPrint.Tables[1].DefaultView.Sort = "stt_rec0";
                    for (int i = 0; i < oReport.DsPrint.Tables[1].DefaultView.Count; i++)
                    {
                        oReport.DsPrint.Tables[1].DefaultView[i]["ps_no_nt"] = 0;
                        oReport.DsPrint.Tables[1].DefaultView[i]["ps_co_nt"] = 0;
                    }
                }
            }
            string stt_rec = oReport.DsPrint.Tables[0].Rows[iRow]["stt_rec"].ToString();
            int so_dong_in = Convert.ToInt16(StartUp.DmctInfo["so_dong_in"]);
            int rowCountCT = StartUp.DsTrans.Tables[1].DefaultView.Count;

            //Thêm số dòng cho đủ ngầm định
            if (rowCountCT < so_dong_in)
            {

                for (int k = rowCountCT; k < so_dong_in; k++)
                {
                    DataRow row = oReport.DsPrint.Tables[1].NewRow();
                    row["stt_rec"] = stt_rec;
                    row["stt_rec0"] = "999";
                    oReport.DsPrint.Tables[1].Rows.Add(row);
                }

            }


            oReport.DsPrint.Tables[0].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";
            oReport.DsPrint.Tables[1].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";
            oReport.DsPrint.Tables[1].DefaultView.Sort = "stt_rec0";
            oReport.DsPrint.Tables[2].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";
            oReport.DsPrint.Tables.Add(StartUp.GetDmnt().Copy());


            oReport.ShowDialog();
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
                    UpdateMoney();
                }
                if (CheckValid())
                {
                    if (!IsSequenceSave)
                    {
                        //Điền thông tin vào 1 số trường khác cho Ph.
                        if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"].ToString()))
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"] = StartUp.DmctInfo["ma_gd"];
                        if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_dvcs"].ToString()))
                            StartUp.DsTrans.Tables[0].DefaultView[0]["ma_dvcs"] = StartUp.SysObj.GetOption("M_MA_DVCS").ToString();

                        StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt"] = SumFunction(StartUp.DsTrans.Tables[1], "ps_no_nt");
                        StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien"] = SumFunction(StartUp.DsTrans.Tables[1], "ps_no");
                    }
                    DataTable tbPhToSave = StartUp.DsTrans.Tables[0].Clone();
                    tbPhToSave.Rows.Add(StartUp.DsTrans.Tables[0].DefaultView[0].Row.ItemArray);
                    /* sửa lỗi 136801940
                    if (!IsSequenceSave)
                    {
                        tbPhToSave.Rows[0]["status"] = 0;
                    }*/
                    DataProvider.UpdateDataTable(StartUp.SysObj, StartUp.DmctInfo["m_phdbf"].ToString(), "stt_rec", tbPhToSave, "stt_rec;row_id");
                    DataTable tbCtToSave = StartUp.DsTrans.Tables[1].Clone();
                    DataTable tbCtGtToSave = StartUp.DsTrans.Tables[2].Clone();
                    if (StartUp.DsTrans.Tables[1].DefaultView.Count > 0)
                    {
                        tbCtToSave = StartUp.DsTrans.Tables[1].DefaultView.ToTable().Copy();
                    }
                    if (StartUp.DsTrans.Tables[2].DefaultView.Count > 0)
                    {
                        tbCtGtToSave = StartUp.DsTrans.Tables[2].DefaultView.ToTable().Copy();
                    }

                    if (!DataProvider.UpdateCtTable(StartUp.SysObj, StartUp.DmctInfo["m_ctdbf"].ToString(), tbCtToSave, StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()))
                    {
                        ExMessageBox.Show( 455,StartUp.SysObj, "Lưu không thành công, kiểm tra lại dữ liệu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    if (!DataProvider.UpdateCtTable(StartUp.SysObj, StartUp.DmctInfo["m_ctgtdbf"].ToString(), tbCtGtToSave, StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString()))
                    {
                        ExMessageBox.Show( 460,StartUp.SysObj, "Lưu không thành công, kiểm tra lại dữ liệu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    bool isError = false;
                    if (!IsSequenceSave)
                    {
                        #region kiểm tra dưới database
                        //if (dsCheckData == null || dsCheckData.Tables[0].Rows.Count == 0)
                            dsCheckData = StartUp.CheckData();

                        dsCheckData.Tables[0].AcceptChanges();
                        if (dsCheckData.Tables.Count > 0)
                        {
                            foreach (DataRowView dv in dsCheckData.Tables[dsCheckData.Tables.Count - 1].DefaultView)
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
                                                if (ExMessageBox.Show( 465,StartUp.SysObj, "Có chứng từ trùng số. Số cuối cùng là: " + "[" + GetLastSoct(StartUp.SysObj, txtMa_qs.Text).Trim() + "]" + ". Có lưu chứng từ này không?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                                                {
                                                    txtSo_ct.SelectAll();
                                                    txtSo_ct.Focus();
                                                    isError = true;
                                                }
                                            }
                                            else if (StartUp.M_trung_so.Equals("2"))
                                            {
                                                ExMessageBox.Show( 470,StartUp.SysObj, "Số chứng từ đã tồn tại!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                                txtSo_ct.SelectAll();
                                                txtSo_ct.Focus();
                                                isError = true;
                                            }
                                        }
                                        break;
                                    case "PH03":
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
                                            ExMessageBox.Show( 475,StartUp.SysObj, "Tk là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                            tabItemHT.Focus();
                                            GrdCt.ActiveCell = (GrdCt.Records[index] as DataRecord).Cells["tk_i"];
                                            GrdCt.Focus();
                                            isError = true;
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
                                            ExMessageBox.Show( 480,StartUp.SysObj, "Tk thuế là tk tổng hợp, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                            tabItemHD_Thue.Focus();
                                            GrdCtgt.ActiveCell = (GrdCtgt.Records[index] as DataRecord).Cells["tk_thue_no"];
                                            GrdCtgt.Focus();
                                            isError = true;
                                        }
                                        break;
                                }
                                dsCheckData.Tables[dsCheckData.Tables.Count - 1].Rows.Remove(dv.Row);
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
                            //Update lại thứ tự các chứng từ


                            //khi số liệu trắng
                            for (int i = 0; i < StartUp.DsTrans.Tables[0].Rows.Count; i++)
                            {
                                if (StartUp.DsTrans.Tables[0].Rows[i]["stt_rec"].ToString().Trim() == "" && (!String.IsNullOrEmpty(StartUp.DsTrans.Tables[0].Rows[i]["ngay_ct"].ToString())))
                                    StartUp.DsTrans.Tables[0].Rows[i]["ngay_ct"] = txtNgay_ct.Value;

                            }
                            //
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

        void Post()
        {
            SqlCommand cmd = new SqlCommand("exec [dbo].[GLCTPK1-Post] @stt_rec, @ma_ct");
            cmd.Parameters.Add("@stt_rec", SqlDbType.VarChar, 50).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"];
            cmd.Parameters.Add("@ma_ct", SqlDbType.Char, 3).Value = StartUp.Ma_ct;
            StartUp.SysObj.ExcuteNonQuery(cmd);
        }

        //private int Lay_Index_Record_Co_TienThueMax()
        //{
        //    int index = -1;
        //    decimal maxTien = 0;
        //    for (int i = 0; i < StartUp.DsTrans.Tables[2].DefaultView.Count; i++)
        //    {
        //        if (Parsedecimal(StartUp.DsTrans.Tables[2].DefaultView[i]["t_thue"], 0) > maxTien)
        //        {
        //            maxTien = decimal.Parse(StartUp.DsTrans.Tables[2].DefaultView[i]["t_thue"].ToString());
        //            index = i;
        //        }
        //    }
        //    return index;
        //}

        //public decimal Parsedecimal(object obj, decimal defaultvalue)
        //{
        //    decimal ketqua = defaultvalue;
        //    decimal.TryParse(obj != null ? obj.ToString() : defaultvalue.ToString(), out ketqua);
        //    return ketqua;
        //}

        private void btnNhan_Click(object sender, RoutedEventArgs e)
        {
            V_Nhan();
        }

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
                    #region ngay_ct
                    if (txtNgay_ct.Value.ToString() == "" && result == true)
                    {
                        ExMessageBox.Show( 485,StartUp.SysObj, "Chưa vào ngày hạch toán!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        result = false;
                        txtNgay_ct.Focus();
                    }
                    if (result && (txtNgay_ct.Value == null || txtNgay_ct.Value.ToString() != ""))
                    {
                        if (!txtNgay_ct.IsValueValid && result == true)
                        {
                            ExMessageBox.Show( 490,StartUp.SysObj, "Ngày hạch toán không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            result = false;
                            txtNgay_ct.Focus();
                        }
                        if (!SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, Convert.ToDateTime(txtNgay_ct.dValue)) && result == true)
                        {
                            ExMessageBox.Show( 495,StartUp.SysObj, "Ngày hạch toán phải sau ngày khóa sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            result = false;
                            txtNgay_ct.Focus();
                        }
                        if (result && Convert.ToDateTime(txtNgay_ct.dValue) < SmLib.NgayTC.GetStartDate(StartUp.M_ngay_ct0))
                        {
                            ExMessageBox.Show( 500,StartUp.SysObj, "Ngày hạch toán phải sau ngày mở sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
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
                            ExMessageBox.Show( 505,StartUp.SysObj, "Chưa vào ngày lập px!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtNgay_lct.Focus();
                            return false;
                        }
                        if (!txtNgay_lct.IsValueValid)
                        {
                            ExMessageBox.Show( 510,StartUp.SysObj, "Ngày lập px không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtNgay_lct.Focus();
                            return false;
                        }
                    }
                    #endregion

                    #region ma_qs
                    if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_qs"].ToString()) && result == true)
                    {
                        ExMessageBox.Show( 515,StartUp.SysObj, "Chưa vào quyển sổ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        result = false;
                        txtMa_qs.IsFocus = true;
                    }
                    #endregion

                    #region so_ct
                    if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"].ToString().Trim()) && result == true)
                    {
                        ExMessageBox.Show( 520,StartUp.SysObj, "Chưa vào số chứng từ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        result = false;
                        txtSo_ct.Focus();
                    }
                    #endregion

                    #region chi tiet HT
                    if (StartUp.DsTrans.Tables[1].DefaultView.Count == 0 && result == true)
                    {
                        ExMessageBox.Show( 525,StartUp.SysObj, "Chưa vào chi tiết, không lưu được!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        result = false;
                        SmLib.WinAPISenkey.SenKey(ModifierKeys.Alt, Key.D1);
                        GrdCt_AddNewRecord(null, null);
                        GrdCt.ActiveCell = (GrdCt.Records[0] as DataRecord).Cells["tk_i"];
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            GrdCt.Focus();
                        }));
                    }
                    #endregion

                    #region kiểm tra tk_i
                    if (StartUp.DsTrans.Tables[1].DefaultView.Count > 0 && result == true)
                    {
                        foreach (DataRowView drv in StartUp.DsTrans.Tables[1].DefaultView)
                        {
                            if (string.IsNullOrEmpty(drv.Row["tk_i"].ToString().Trim()))
                            {
                                StartUp.DsTrans.Tables[1].Rows.Remove(drv.Row);
                            }
                            else
                            {
                                drv.Row["ma_ct"] = StartUp.Ma_ct;
                                drv.Row["so_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"];
                                drv.Row["ngay_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ngay_ct"];
                            }
                            if (StartUp.DsTrans.Tables[1].DefaultView.Count == 0)
                            {
                                ExMessageBox.Show( 530,StartUp.SysObj, "Chưa vào chi tiết, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                result = false;
                                SmLib.WinAPISenkey.SenKey(ModifierKeys.Alt, Key.D1);
                                GrdCt_AddNewRecord(null, null);
                                GrdCt.ActiveCell = (GrdCt.Records[0] as DataRecord).Cells["tk_i"];
                                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                {
                                    GrdCt.Focus();
                                }));
                            }
                          
                        }
                    }
                    #endregion

                    #region Kiểm tra mã khách
                    foreach (DataRecord rec in GrdCt.Records)
                    {
                        if (rec.Cells["tk_cn"].Value.ToString() == "1" && rec.Cells["ma_kh_i"].Value.ToString().Trim() == "")
                        {
                            ExMessageBox.Show( 535,StartUp.SysObj, "Chưa vào mã khách!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            GrdCt.ActiveCell = rec.Cells["ma_kh_i"];
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                GrdCt.Focus();
                            }));
                            return false;
                        }
                    }
                    #endregion

                    #region theo nhóm dk
                    if (result == true)
                    {

                        //lấy các dòng trong tab hạch toán
                        //kiểm tra dòng là nợ hay có
                        var ct = from tb_ct in StartUp.DsTrans.Tables[1].AsEnumerable()
                                 where tb_ct["stt_rec"].ToString() == stt_rec
                                 select new
                                 {
                                     nh_dk = tb_ct["nh_dk"].ToString().TrimEnd(),
                                     ps_no_nt = tb_ct["ps_no_nt"] == null ? 0 : tb_ct["ps_no_nt"],
                                     ps_co_nt = tb_ct["ps_co_nt"] == null ? 0 : tb_ct["ps_co_nt"],
                                     ps_no = tb_ct["ps_no"] == null ? 0 : tb_ct["ps_no"],
                                     ps_co = tb_ct["ps_co"] == null ? 0 : tb_ct["ps_co"],
                                     n_c = (tb_ct["ps_no_nt"] == null ? 0 : Convert.ToDecimal(tb_ct["ps_no_nt"])) != 0 ||
                                     (tb_ct["ps_no"] == null ? 0 : Convert.ToDecimal(tb_ct["ps_no"])) != 0 ? "n" : "c"
                                 };
                        //group by theo nh_dk và no_co
                        //để tính tổng ps_no, ps_co
                        //đếm có bao nhiêu dòng nợ, có trong tab HT

                        //--Ghi chu: tinh sum thi phai convert ve kieu decimal hoac float
                        //--neu de kieu double se co truong hop bi sai
                        //--sai voi nhung so co so le la (.23 + .22) vd: 50.23 + 50.22
                        var gbnh_dk = from table in ct
                                      group table by new
                                      {
                                          table.nh_dk,
                                          table.n_c
                                      } into gbtable
                                      select new
                                      {
                                          nh_dk = gbtable.Key.nh_dk,
                                          n_c = gbtable.Key.n_c,
                                          sbg = gbtable.Count(),
                                          t_ps_no_nt = gbtable.Sum(p => (decimal)p.ps_no_nt),
                                          t_ps_co_nt = gbtable.Sum(p => (decimal)p.ps_co_nt),
                                          t_ps_no = gbtable.Sum(p => (decimal)p.ps_no),
                                          t_ps_co = gbtable.Sum(p => (decimal)p.ps_co)
                                      };

                        #region kiểm tra hạch toán nhiều nợ, nhiều có
                        //kiểm tra hạch toán nhiều nợ, nhiều có
                        var kt_nn_nc = (from nn_nc in gbnh_dk
                                        where nn_nc.sbg > 1
                                        group nn_nc by nn_nc.nh_dk into gb
                                        select new
                                        {
                                            nh_dk = gb.Key,
                                            sbg1 = gb.Count()
                                        }).Where(p => p.sbg1 > 1);
                        if (kt_nn_nc.Count() > 0)
                        {
                            string nh_dks = string.Empty;
                            foreach (var kt in kt_nn_nc)
                            {
                                nh_dks += kt.nh_dk.ToString().Trim() + ", ";
                            }
                            ExMessageBox.Show(1540,StartUp.SysObj, "Có hạch toán nhiều nợ, nhiều có trong (các) nhóm định khoản " + "[" + nh_dks.Substring(0, nh_dks.Length - 2) + "]" + "!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                            result = false;
                        }
                        #endregion

                        #region kiểm tra nhập đủ tk nợ, có
                        //kiểm tra nhập đủ tk nợ, có
                        if (result == true)
                        {
                            var kt_tk = (from tk in gbnh_dk
                                         group tk by tk.nh_dk into gb
                                         select new
                                         {
                                             nh_dk = gb.Key,
                                             sbg1 = gb.Count()
                                         }).Where(p => p.sbg1 <= 1);
                            if (kt_tk.Count() > 0)
                            {
                                string nh_dks = string.Empty;
                                foreach (var kt in kt_tk)
                                {
                                    nh_dks += kt.nh_dk.ToString().Trim() + ", ";
                                }
                                ExMessageBox.Show( 545,StartUp.SysObj, "Chưa nhập đủ tk nợ, có trong (các) nhóm định khoản " + "[" + nh_dks.Substring(0, nh_dks.Length - 2) + "]" + "!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                result = false;
                            }
                        }
                        #endregion

                        #region kiểm tra ps_no_nt, ps_co_nt có bằng nhau
                        //kiểm tra ps_no_nt, ps_co_nt có bằng nhau ko??
                        if (result == true)
                        {
                            var kt_ps_nt = (from ps_nt in gbnh_dk
                                            group ps_nt by ps_nt.nh_dk into gbps_nt
                                            select new
                                            {
                                                nh_dk = gbps_nt.Key,
                                                ps_nt = gbps_nt.Sum(p => p.t_ps_no_nt - p.t_ps_co_nt)
                                            }).Where(p => p.ps_nt != 0);

                            if (kt_ps_nt.Count() > 0)
                            {
                                string nh_dks = string.Empty;
                                foreach (var kt in kt_ps_nt)
                                {
                                    nh_dks += kt.nh_dk.ToString().Trim() + ", ";
                                }
                                ExMessageBox.Show( 550,StartUp.SysObj, "Phát sinh nợ khác phát sinh có trong (các) nhóm định khoản " + "[" + nh_dks.Substring(0, nh_dks.Length - 2) + "]" + "!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                result = false;
                            }
                        }
                        #endregion

                        #region kiểm tra ps_no, ps_co có bằng nhau
                        //kiểm tra ps_no, ps_co có bằng nhau ko???
                        if (result == true)
                        {
                            var kt_ps = (from ps in gbnh_dk
                                         group ps by ps.nh_dk into gbps
                                         select new
                                         {
                                             nh_dk = gbps.Key,
                                             ps = gbps.Sum(p => p.t_ps_no - p.t_ps_co)
                                         }).Where(p => p.ps != 0);
                            //nếu có cặp nhóm dk ko bằng nhau
                            //thì kiểm tra đối với nhóm dk có ps!=0, có bao nhiêu dòng là ps_no, bao nhiêu dòng là ps_co
                            if (kt_ps.Count() > 0)
                            {

                                #region can bang ps no, ps co
                                if (result)
                                {
                                    foreach (var kt in kt_ps)
                                    {
                                        int count_no = (from tb in gbnh_dk
                                                        where tb.nh_dk.ToString() == kt.nh_dk.ToString() && tb.n_c == "n"
                                                        select tb.sbg).First();
                                        bool isupdate = false;
                                        //nếu có 1 dòng ps_no thì cộng giá trị chênh lệch vô dòng đầu tiên của ps_co
                                        if (count_no == 1)
                                        {
                                            for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count && isupdate == false; i++)
                                            {
                                                decimal ps_co = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["ps_co"], 0);
                                                if (StartUp.DsTrans.Tables[1].DefaultView[i]["nh_dk"].ToString().TrimEnd() == kt.nh_dk.ToString())// && ps_co != 0)
                                                {
                                                    if (txtMa_nt.Text != StartUp.M_ma_nt0)
                                                    {
                                                        if (ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["ps_co_nt"], 0) != 0)
                                                        {
                                                            StartUp.DsTrans.Tables[1].DefaultView[i]["ps_co"] = ps_co + kt.ps;
                                                            isupdate = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        StartUp.DsTrans.Tables[1].DefaultView[i]["ps_co"] = ps_co + kt.ps;
                                                        isupdate = true;
                                                    }
                                                }
                                            }
                                        }
                                        //nếu có lớn hơn 1 dòng ps_no thì cộng giá trị chênh lệch vô dòng đầu tiên của ps_no
                                        else
                                        {
                                            for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count && isupdate == false; i++)
                                            {
                                                decimal ps_no = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["ps_no"], 0);
                                                if (StartUp.DsTrans.Tables[1].DefaultView[i]["nh_dk"].ToString().TrimEnd() == kt.nh_dk.ToString()) //&& ps_no != 0)
                                                {
                                                    if (txtMa_nt.Text != StartUp.M_ma_nt0)
                                                    {
                                                        if (ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i]["ps_no_nt"], 0) != 0)
                                                        {
                                                            StartUp.DsTrans.Tables[1].DefaultView[i]["ps_no"] = ps_no - kt.ps;
                                                            isupdate = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        StartUp.DsTrans.Tables[1].DefaultView[i]["ps_no"] = ps_no - kt.ps;
                                                        isupdate = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion


                                //}
                            }
                        }
                        #endregion
                    }
                    #endregion

                    #region chi tiet HD thue
                    if (StartUp.DsTrans.Tables[2].DefaultView.Count > 0 && result == true)
                    {

                        #region HD thue
                        int rowindex = 0;
                        bool showMessageCheckHD = false;
                        // string tk_thue = "";
                        if (!CheckVoucherOutofDate())
                        {
                            result = false;
                            return result;
                        }
                        foreach (DataRowView drv in StartUp.DsTrans.Tables[2].DefaultView)
                        {
                            if (string.IsNullOrEmpty(drv["ma_ms"].ToString().Trim()))
                            {
                                StartUp.DsTrans.Tables[2].Rows.Remove(drv.Row);
                                StartUp.DsTrans.Tables[2].AcceptChanges();
                                continue;
                            }
                            else
                            {
                                drv["ma_ct"] = StartUp.Ma_ct;
                                drv["ngay_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ngay_ct"];

                                drv["ma_nt"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"];
                                drv["ty_gia"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"];
                                drv["ty_giaf"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ty_giaf"];
                                drv["status"] = StartUp.DsTrans.Tables[0].DefaultView[0]["status"];
                                drv["ma_gd"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"];
                                drv["so_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"];

                                #region ma_ms
                                if (string.IsNullOrEmpty(drv["ma_ms"].ToString().Trim()) && result == true)
                                {
                                    ExMessageBox.Show( 555,StartUp.SysObj, "Chưa vào nhóm!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                    result = false;

                                    SmLib.WinAPISenkey.SenKey(ModifierKeys.Alt, Key.D2);
                                    this.GrdCtgt.ActiveCell = (GrdCtgt.Records[rowindex] as DataRecord).Cells["ma_ms"];
                                    this.GrdCtgt.Focus();
                                }
                                #endregion

                                #region ma_so_thue
                                if (result == true && drv["ma_so_thue"].ToString().Trim() != "")
                                {
                                    if (!StartUp.M_MST_CHECK.Equals("0"))
                                    {
                                        if (!SmLib.SysFunc.CheckSumMaSoThue(drv["ma_so_thue"].ToString().Trim()) &&
                                            !string.IsNullOrEmpty(drv["ma_so_thue"].ToString().Trim()))
                                        {
                                            if (StartUp.M_MST_CHECK.Equals("1"))
                                                ExMessageBox.Show( 560,StartUp.SysObj, "Mã số thuế không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);

                                            else //if (StartUp.M_MST_CHECK.Equals("2"))
                                            {
                                                ExMessageBox.Show( 565,StartUp.SysObj, "Mã số thuế không hợp lệ, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                                result = false;
                                                SmLib.WinAPISenkey.SenKey(ModifierKeys.Alt, Key.D2);
                                                this.GrdCtgt.ActiveCell = (GrdCtgt.Records[rowindex] as DataRecord).Cells["ma_so_thue"];
                                                this.GrdCtgt.Focus();
                                            }
                                        }
                                    }
                                }
                                #endregion

                                if (StartUp.M_CHK_HD_VAO != 0)
                                {
                                    string so_ct0 = drv.Row["so_ct0"].ToString().Trim();
                                    string so_seri0 = drv.Row["so_seri0"].ToString().Trim();
                                    string ngay_ct0 = string.IsNullOrEmpty(drv.Row["ngay_ct0"].ToString().Trim()) ? "" : Convert.ToDateTime(drv.Row["ngay_ct0"].ToString().Trim()).Date.ToShortDateString().Substring(0, 10);
                                    string ma_so_thue = drv.Row["ma_so_thue"].ToString().Trim();

                                    if (StartUp.CheckExistHDVao(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString(), so_ct0, so_seri0, ngay_ct0, ma_so_thue) && !showMessageCheckHD)
                                    {
                                        ExMessageBox.Show(310, StartUp.SysObj, string.Format("Hoá đơn số [{0}], ký hiệu [{1}], ngày [{2}], MST [{3}] đã tồn tại!", so_ct0, so_seri0, ngay_ct0, ma_so_thue), "", MessageBoxButton.OK, MessageBoxImage.Information);
                                        showMessageCheckHD = true;
                                        if (StartUp.M_CHK_HD_VAO == 2)
                                        {
                                            result = false;
                                            return result;
                                        }
                                    }
                                }

                                rowindex = rowindex + 1;
                            }
                        }
                        #endregion

                        #region kiểm tra tiền thuế theo tk
                        if (result == true)
                        {

                            var CtRows = from tb_ct in StartUp.DsTrans.Tables[1].AsEnumerable()
                                         where tb_ct["stt_rec"].ToString() == stt_rec &&
                                         (tb_ct["ps_no_nt"] == null ? 0 : Convert.ToDecimal(tb_ct["ps_no_nt"].ToString())) != 0 &&
                                         (tb_ct["ps_no"] == null ? 0 : Convert.ToDecimal(tb_ct["ps_no"].ToString())) != 0
                                         select new
                                         {
                                             tk = tb_ct["tk_i"].ToString().Trim(),
                                             ps_no_nt = tb_ct["ps_no_nt"] == null ? 0 : tb_ct["ps_no_nt"],
                                             ps_no = tb_ct["ps_no"] == null ? 0 : tb_ct["ps_no"]
                                         };
                            var GroupByCtRows = from ct in CtRows
                                                group ct by ct.tk into gbct
                                                select new
                                                {
                                                    tk = gbct.Key.ToString().Trim(),
                                                    t_ps_no_nt = SysFunc.Round(gbct.Sum(p =>
                                                    {
                                                        if (p.ps_no_nt == null || p.ps_no_nt == DBNull.Value)
                                                            return 0;
                                                        decimal value = 0;
                                                        decimal.TryParse(p.ps_no_nt.ToString(), out value);
                                                        return value;
                                                    }), StartUp.M_ROUND_NT),
                                                    t_ps_no = SysFunc.Round(gbct.Sum(p =>
                                                    {
                                                        if (p.ps_no == null || p.ps_no == DBNull.Value)
                                                            return 0;
                                                        decimal value = 0;
                                                        decimal.TryParse(p.ps_no.ToString(), out value);
                                                        return value;
                                                    }), StartUp.M_ROUND),
                                                };
                            var CtgtRows = from tb_ctgt in StartUp.DsTrans.Tables[2].AsEnumerable()
                                           where tb_ctgt["stt_rec"].ToString() == stt_rec
                                           group tb_ctgt by tb_ctgt["tk_thue_no"].ToString().Trim() into gbctgt
                                           select new
                                           {
                                               tk_thue = gbctgt.Key.ToString().Trim(),
                                               t_thue_nt = SysFunc.Round(gbctgt.Sum(p =>
                                               {
                                                   object value = p.Field<object>("t_thue_nt");
                                                   if (value == null)
                                                       return 0;
                                                   if (value.ToString() == "")
                                                       return 0;
                                                   decimal d = 0;
                                                   decimal.TryParse(value.ToString(), out d);
                                                   return d;
                                               }
                                                   ), StartUp.M_ROUND_NT),
                                               t_thue = SysFunc.Round(gbctgt.Sum(p =>
                                               {
                                                   object value = p.Field<object>("t_thue");
                                                   if (value == null)
                                                       return 0;
                                                   if (value.ToString() == "")
                                                       return 0;
                                                   decimal d = 0;
                                                   decimal.TryParse(value.ToString(), out d);
                                                   return d;
                                               }), StartUp.M_ROUND)
                                           };
                            var kttien_thue = from rowct in GroupByCtRows
                                              from rowctgt in CtgtRows
                                              where rowct.tk == rowctgt.tk_thue &&
                                              (rowct.t_ps_no_nt != rowctgt.t_thue_nt || rowct.t_ps_no != rowctgt.t_thue)
                                              select rowct.tk;

                            var kttk_thue = from rowct in GroupByCtRows
                                            from rowctgt in CtgtRows
                                            where !GroupByCtRows.Any(x => x.tk == rowctgt.tk_thue)
                                            select rowctgt.tk_thue;

                            if (kttien_thue.Count() > 0) // || kttk_thue.Count() > 0)
                            {
                                ExMessageBox.Show(570, StartUp.SysObj, "Thuế bảng kê khác thuế trong chứng từ, không lưu được!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                result = false;
                            }
                        }
                        #endregion
                    }
                    #endregion

                }
            }
            return result;
        }
        #endregion

        #region UpdateMoney
        void UpdateMoney()
        {
            if (StartUp.M_ma_nt0.Equals(txtMa_nt.Text))
            {
                if (StartUp.DsTrans.Tables[1].DefaultView.Count > 0)
                {
                    for (int ct = 0; ct < StartUp.DsTrans.Tables[1].DefaultView.Count; ct++)
                    {
                        StartUp.DsTrans.Tables[1].DefaultView[ct]["ps_no"] = StartUp.DsTrans.Tables[1].DefaultView[ct]["ps_no_nt"];
                        StartUp.DsTrans.Tables[1].DefaultView[ct]["ps_co"] = StartUp.DsTrans.Tables[1].DefaultView[ct]["ps_co_nt"];
                    }
                }
                if (StartUp.DsTrans.Tables[2].DefaultView.Count > 0)
                {
                    for (int ctgt = 0; ctgt < StartUp.DsTrans.Tables[2].DefaultView.Count; ctgt++)
                    {
                        StartUp.DsTrans.Tables[2].DefaultView[ctgt]["t_thue"] = StartUp.DsTrans.Tables[2].DefaultView[ctgt]["t_thue_nt"];
                        StartUp.DsTrans.Tables[2].DefaultView[ctgt]["t_tien"] = StartUp.DsTrans.Tables[2].DefaultView[ctgt]["t_tien_nt"];
                        StartUp.DsTrans.Tables[2].DefaultView[ctgt]["t_tt"] = StartUp.DsTrans.Tables[2].DefaultView[ctgt]["t_tt_nt"];
                    }
                }
            }
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

        #region V_Tim
        private void V_Tim()
        {
            try
            {
                currActionTask = ActionTask.View;
                //FrmSearchApctpn9 _FrmTim = new FrmSearchApctpn9(StartUp.SysObj, StartUp.DmctInfo["m_phdbf"].ToString(), StartUp.Ma_ct);
                FrmSearchGlctpk1 _FrmTim = new FrmSearchGlctpk1(StartUp.SysObj, StartUp.filterId, StartUp.filterView);
                SmLib.SysFunc.LoadIcon(_FrmTim);

                if (StartUp.M_LAN != "V")
                    _FrmTim.Title = "Search";

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

        #region V_Xem
        private void V_Xem()
        {
            currActionTask = ActionTask.View;
            //  set lai stringbrowse 
            //string stringBrowse1 = "ngay_ct:fl:100:h=Ngày c.từ; so_ct:fl:70:h=Số c.từ; t_tien_nt:n1:130:h=Tổng phát sinh; ty_gia:r:130:h=Tỷ giá; t_tien:n0:130:h=Tổng ps VND; date:d:105:h=Ngày cập nhật; time:100:h=Giờ cập nhật; user_id:n:100:h=Số hiệu NSD; user_name:100:h=Tên NSD";
            //string stringBrowse2 = "tk_i:fl:80:h=Tài khoản; ps_no_nt:n1:130:h=Ps nợ nt; ps_co_nt:n1:130:h=Ps có nt; dien_giaii:225:h=Diễn giải; ps_no:n0:130:h=Ps nợ VND; ps_co:n0:130:h=Ps có VND";
            DataTable PhViewTablev = StartUp.DsTrans.Tables[0].Copy();
            PhViewTablev.Rows.RemoveAt(0);

            SmVoucherLib.FormView _frmView = new SmVoucherLib.FormView(StartUp.SysObj, PhViewTablev.DefaultView, StartUp.DsTrans.Tables[1].DefaultView, StartUp.stringBrowse1, StartUp.stringBrowse2, "stt_rec");
            _frmView.ListFieldSum = "t_tien_nt;t_tien";
            _frmView.frmBrw.Title = StartUp.M_Tilte;
            //Them cac truong tu do
            SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, _frmView.frmBrw.oBrowseCt, StartUp.Ma_ct, 1);



            _frmView.frmBrw.LanguageID  = "Glctpk1ViewXem";
            _frmView.ShowDialog();

            // Set lai irow va rowfilter ...
            if (_frmView.DataGrid.ActiveRecord != null)
            {

                int select_irow = (_frmView.DataGrid.ActiveRecord as DataRecord).Index;
                if (select_irow >= 0)
                {
                    string selected_stt_rec = (_frmView.DataGrid.DataSource as DataView)[select_irow]["stt_rec"].ToString();
                    FrmGlctpk1.iRow = select_irow + 1;
                    StartUp.DataFilter(selected_stt_rec);
                    IsVisibilityFieldsXamDataGrid(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString());
                }
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

                                //Refresh lại grid hd thuế
                                if (StartUp.DsTrans.Tables[2].Rows.Count > 0)
                                {
                                    //lấy các rowfilter trong grid hd thuế
                                    DataRow[] _row = StartUp.DsTrans.Tables[2].Select("stt_rec='" + stt_rec + "'");
                                    foreach (DataRow dr in _row)
                                    {
                                        //delete các row có trong grid hd thuế
                                        StartUp.DsTrans.Tables[2].Rows.Remove(dr);
                                    }
                                }

                                //Refresh lại table[0]
                                StartUp.DsTrans.Tables[0].Rows[iRow].ItemArray = DsVitual.Tables[0].Rows[0].ItemArray;

                                StartUp.DsTrans.Tables[1].Merge(DsVitual.Tables[1]);
                                StartUp.DsTrans.Tables[2].Merge(DsVitual.Tables[2]);
                                //RowFilter lại Table[0], Table[1], Table[2] với iRow trước khi edit
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
            if (StartUp.DsTrans.Tables[0].Rows.Count == 1)
                return;
            currActionTask = ActionTask.Delete;
            try
            {
                string _stt_rec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();
                //xoa tksd13
                StartUpTrans.UpdateTkSd13(1, 0);    

                //xóa trong ph, ct, ctgt
                //xóa chứng từ
                SqlCommand cmd = new SqlCommand("exec [dbo].[DeleteVoucher] @ma_ct, @stt_rec");
                cmd.Parameters.Add("@ma_ct", SqlDbType.Char, 3).Value = StartUp.Ma_ct;
                cmd.Parameters.Add("@stt_rec", SqlDbType.Char, 11).Value = _stt_rec;
                StartUp.SysObj.ExcuteNonQuery(cmd);

                // ----Warning : Không nên xóa Table[0] trước, nếu xóa trước sẽ bị mất Binding -----------------------
                // Nên dịch chuyển iRow lùi 1 dòng
                // Sau đó RowFilter lại Table[0], Table[1], Table[2]
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
            //    ExMessageBox.Show( 575,StartUp.SysObj, "Không thể xóa chứng từ đã khóa sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}
            if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim()))
                return;
            Xoa();
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
                //txtTy_gia.IsReadOnly = true;
                //GrdCt không hiển thị ps_no, ps_co
                GrdCt.FieldLayouts[0].Fields["ps_no"].Visibility = Visibility.Hidden;
                GrdCt.FieldLayouts[0].Fields["ps_co"].Visibility = Visibility.Hidden;

                GrdCt.FieldLayouts[0].Fields["ps_no"].Settings.CellMaxWidth = 0;
                GrdCt.FieldLayouts[0].Fields["ps_co"].Settings.CellMaxWidth = 0;

                //GrdCtgt không hiển thị tiền, thuế, tt
                GrdCtgt.FieldLayouts[0].Fields["t_tien"].Visibility = Visibility.Hidden;
                GrdCtgt.FieldLayouts[0].Fields["t_thue"].Visibility = Visibility.Hidden;
                GrdCtgt.FieldLayouts[0].Fields["t_tt"].Visibility = Visibility.Hidden;

                GrdCtgt.FieldLayouts[0].Fields["t_tien"].Settings.CellMaxWidth = 0;
                GrdCtgt.FieldLayouts[0].Fields["t_thue"].Settings.CellMaxWidth = 0;
                GrdCtgt.FieldLayouts[0].Fields["t_tt"].Settings.CellMaxWidth = 0;
            }
            else
            {
                //GrdCt hiển thị ps_no, ps_co
                GrdCt.FieldLayouts[0].Fields["ps_no"].Visibility = Visibility.Visible;
                GrdCt.FieldLayouts[0].Fields["ps_co"].Visibility = Visibility.Visible;

                GrdCt.FieldLayouts[0].Fields["ps_no"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["ps_no"].Width.Value.Value;
                GrdCt.FieldLayouts[0].Fields["ps_co"].Settings.CellMaxWidth = GrdCt.FieldLayouts[0].Fields["ps_co"].Width.Value.Value;

                //GrdCtgt hiển thị tiền, thuế, tt
                GrdCtgt.FieldLayouts[0].Fields["t_tien"].Visibility = Visibility.Visible;
                GrdCtgt.FieldLayouts[0].Fields["t_thue"].Visibility = Visibility.Visible;
                GrdCtgt.FieldLayouts[0].Fields["t_tt"].Visibility = Visibility.Visible;

                GrdCtgt.FieldLayouts[0].Fields["t_tien"].Settings.CellMaxWidth = GrdCtgt.FieldLayouts[0].Fields["t_tien"].Width.Value.Value;
                GrdCtgt.FieldLayouts[0].Fields["t_thue"].Settings.CellMaxWidth = GrdCtgt.FieldLayouts[0].Fields["t_thue"].Width.Value.Value;
                GrdCtgt.FieldLayouts[0].Fields["t_tt"].Settings.CellMaxWidth = GrdCtgt.FieldLayouts[0].Fields["t_tt"].Width.Value.Value;
            }
            Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
            Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
            ChangeLanguage();
        }
        #endregion

        #region IsVisibilityFieldsXamDataGridBySua_Tien
        void IsVisibilityFieldsXamDataGridBySua_Tien()
        {
            IsCheckedSua_tien.Value = ChkSuatien.IsChecked.Value;
        }
        #endregion
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
                //    ExMessageBox.Show( 580,StartUp.SysObj, "Ngày hạch toán phải sau ngày khóa sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                //    txtNgay_ct.Value = DateTime.Now.Date;
                //}
                if (StartUp.M_Ngay_lct.Equals("0") || string.IsNullOrEmpty(txtNgay_lct.Text))
                    txtNgay_lct.Value = txtNgay_ct.Value;
                //if (txtNgay_ct.Value.ToString() != txtNgay_lct.Value.ToString())
                //    ExMessageBox.Show( 585,StartUp.SysObj, "Ngày lập chứng từ khác với ngày hạch toán!", "", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    ExMessageBox.Show( 590,StartUp.SysObj, "Ngày lập chứng từ khác với ngày hạch toán!", "", MessageBoxButton.OK, MessageBoxImage.Information);
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
                        if (string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["so_cttmp"].ToString().Trim()) 
                            || !StartUp.DsTrans.Tables[0].DefaultView[0]["ma_qs"].ToString().Trim().Equals(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_qstmp"].ToString().Trim()))
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
               //|| txtTy_gia.OldValue == Convert.ToDecimal(txtTy_gia.Value))
                return;
            Ty_Gia_ValueChange.Value = (txtTy_gia.OldValue != Convert.ToDecimal(txtTy_gia.Value));
            decimal _ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);
            int sua_tien = 0;
            int.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"].ToString(), out sua_tien);
            //Hạch toán thay đổi
           // if (sua_tien == 0 && _ty_gia != 0)
            if (_ty_gia != 0)
            {
                //ma_nt thay doi thi updatetotal or ty gia thay doi và sua_tien = 0 thì updatetotal
                if (sua_tien == 0 || IsMa_ntChanged)
                {
                    UpdateTotal("ps_no", "ps_no_nt");
                    UpdateTotal("ps_co", "ps_co_nt");

                    #region Hd thue thay doi
                    decimal _t_tien_nt_hdthue = 0, _thue_suat = 0;
                    decimal _t_tien_hdthue = 0, _t_thue_hdthue = 0;
                    if (StartUp.DsTrans.Tables[2].DefaultView.Count > 0)
                    {
                        foreach (DataRowView drv in StartUp.DsTrans.Tables[2].DefaultView)
                        {
                            decimal.TryParse(drv.Row["t_tien_nt"].ToString(), out _t_tien_nt_hdthue);
                            decimal.TryParse(drv.Row["thue_suat"].ToString(), out _thue_suat);
                            _t_tien_hdthue = SysFunc.Round(_t_tien_nt_hdthue * _ty_gia, StartUp.M_ROUND);
                            drv.Row["t_tien"] = _t_tien_hdthue;
                            _t_thue_hdthue = SysFunc.Round((_t_tien_hdthue * _thue_suat) / 100, StartUp.M_ROUND);
                            if (_t_thue_hdthue != 0)
                                drv.Row["t_thue"] = _t_thue_hdthue;
                            drv.Row["t_tt"] = _t_tien_hdthue + Convert.ToDecimal(drv.Row["t_thue"]);

                        }
                    }
                    #endregion
                }
            }
           
        }
        #endregion

        #region UpdateTotal
        //void UpdateTotal(DataView dtview, string columnname, string columnname_nt)//, decimal t_tien)
        //{
        //    decimal tien = 0, tien_nt = 0;
        //    decimal ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);
        //    if (dtview.Count > 0)
        //    {
        //        foreach (DataRowView drv in dtview)
        //        {
        //            decimal.TryParse(drv.Row[columnname_nt].ToString(), out tien_nt);
        //            tien = SysFunc.Round(tien_nt * ty_gia, StartUp.M_ROUND);
        //            drv.Row[columnname] = tien;
        //        }
        //    }
        //}
        
        void UpdateTotal(string columnname, string columnname_nt)
        {
            decimal ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);
            for (int i = 0; i < StartUp.DsTrans.Tables[1].DefaultView.Count; i++)
            {
                decimal tien_nt = ParseDecimal(StartUp.DsTrans.Tables[1].DefaultView[i][columnname_nt], 0);
                decimal tien = SysFunc.Round(tien_nt * ty_gia, StartUp.M_ROUND);
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
                    int sua_tien = 0;
                    int.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"].ToString(), out sua_tien);
                    
                    switch (e.Cell.Field.Name)
                    {
                        #region tk_i
                        case "tk_i":
                            {
                                if (e.Editor.Value == null)
                                    return;
                                if (e.Cell.IsDataChanged)
                                {

                                    AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                    if (txt.RowResult != null && !txt.Text.Trim().Equals(""))
                                    {
                                        e.Cell.Record.Cells["ten_tk"].Value = txt.RowResult["ten_tk"];
                                        e.Cell.Record.Cells["ten_tk2"].Value = txt.RowResult["ten_tk2"];
                                        e.Cell.Record.Cells["tk_cn"].Value = txt.RowResult["tk_cn"];
                                    }
                                    if (txt.Text.Trim().Equals(""))
                                    {
                                        e.Cell.Record.Cells["ten_tk"].Value = "";
                                        e.Cell.Record.Cells["ten_tk2"].Value = "";
                                        e.Cell.Record.Cells["tk_cn"].Value = 0;
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region ma_kh_i
                        case "ma_kh_i":
                            {
                                if (e.Editor.Value == null)
                                    return;
                                AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                if (txt.RowResult != null && !txt.Text.Trim().Equals(""))
                                {
                                    e.Cell.Record.Cells["ten_kh"].Value = txt.RowResult["ten_kh"];
                                    e.Cell.Record.Cells["ten_kh2"].Value = txt.RowResult["ten_kh2"];
                                }
                                if (txt.Text.Trim().Equals(""))
                                {
                                    e.Cell.Record.Cells["ten_kh"].Value = "";
                                    e.Cell.Record.Cells["ten_kh2"].Value = "";
                                }
                            }
                            break;
                        #endregion

                        #region ps_no_nt
                        case "ps_no_nt":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                        e.Cell.Record.Cells["ps_no_nt"].Value = 0;

                                    decimal ps_no_nt = ParseDecimal(e.Editor.Value, 0);

                                    if (ps_no_nt != 0)
                                    {
                                        decimal ps_no = SysFunc.Round(ps_no_nt * ty_gia, StartUp.M_ROUND);
                                        //if (sua_tien == 0 && ps_no != 0)
                                        if (ps_no != 0)
                                            e.Cell.Record.Cells["ps_no"].Value = ps_no;
                                        e.Cell.Record.Cells["ps_co_nt"].Value = 0;
                                        e.Cell.Record.Cells["ps_co"].Value = 0;
                                    }else
                                        e.Cell.Record.Cells["ps_no"].Value = 0;

                                    if (txtMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["ps_no"].Value = e.Cell.Record.Cells["ps_no_nt"].Value;
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region ps_co_nt
                        case "ps_co_nt":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                        e.Cell.Record.Cells["ps_co_nt"].Value = 0;

                                    decimal ps_co_nt = ParseDecimal(e.Editor.Value, 0);
                                    if (ps_co_nt != 0)
                                    {
                                        decimal ps_co = SysFunc.Round(ps_co_nt * ty_gia, StartUp.M_ROUND);
                                        //if (sua_tien == 0 && ps_co != 0)
                                        if (ps_co != 0)
                                            e.Cell.Record.Cells["ps_co"].Value = ps_co;
                                        e.Cell.Record.Cells["ps_no_nt"].Value = 0;
                                        e.Cell.Record.Cells["ps_no"].Value = 0;
                                    }else
                                        e.Cell.Record.Cells["ps_co"].Value = 0;

                                    if (txtMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["ps_co"].Value = e.Cell.Record.Cells["ps_co_nt"].Value;
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region ps_no
                        case "ps_no":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                        e.Cell.Record.Cells["ps_no"].Value = 0;

                                    decimal ps_no = ParseDecimal(e.Editor.Value, 0); ;
                                    if (ps_no != 0)
                                    {
                                        e.Cell.Record.Cells["ps_co"].Value = 0;
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region ps_co
                        case "ps_co":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                        e.Cell.Record.Cells["ps_co"].Value = 0;
                                    decimal ps_co = ParseDecimal(e.Editor.Value, 0);
                                    if (ps_co != 0)
                                    {
                                        e.Cell.Record.Cells["ps_no"].Value = 0;
                                    }
                                }
                            }
                            break;
                        #endregion

                        
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
                }
                NewCtRecord["stt_rec0"] = string.Format("{0:000}", Stt_rec0);
                NewCtRecord["ma_ct"] = StartUp.Ma_ct;
                NewCtRecord["so_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["so_ct"];
                NewCtRecord["ngay_ct"] = StartUp.DsTrans.Tables[0].DefaultView[0]["ngay_ct"];
                NewCtRecord["nh_dk"] = "";
                NewCtRecord["han_tt"] = 0;

                int count = StartUp.DsTrans.Tables[1].DefaultView.Count;
                if (count > 0)
                {
                    NewCtRecord["ma_kh_i"] = StartUp.DsTrans.Tables[1].DefaultView[count - 1].Row["ma_kh_i"];
                    NewCtRecord["ten_kh"] = StartUp.DsTrans.Tables[1].DefaultView[count - 1].Row["ten_kh"];
                    NewCtRecord["ten_kh2"] = StartUp.DsTrans.Tables[1].DefaultView[count - 1].Row["ten_kh2"];
                    NewCtRecord["dien_giaii"] = StartUp.DsTrans.Tables[1].DefaultView[count - 1].Row["dien_giaii"];
                    NewCtRecord["nh_dk"] = StartUp.DsTrans.Tables[1].DefaultView[count - 1].Row["nh_dk"];
                    
                    SqlCommand cmd = new SqlCommand("select ma_dm from dmctct where ma_ct=@ma_ct and ma_dm=@ma_dm");
                    cmd.Parameters.Add("@ma_ct", SqlDbType.Char).Value = StartUp.Ma_ct;
                    cmd.Parameters.Add("@ma_dm", SqlDbType.Char).Value = "dmvv";
                    
                    //if (StartUp.SysObj.ExcuteReader(cmd).Tables[0].Rows.Count > 0)
                    //{
                    //    NewCtRecord["ma_vv_i"] = StartUp.DsTrans.Tables[1].DefaultView[count - 1].Row["ma_vv_i"];
                    //}

                    decimal t_ps_no_nt = SumFunction(StartUp.DsTrans.Tables[1], "ps_no_nt");
                    decimal t_ps_co_nt = SumFunction(StartUp.DsTrans.Tables[1], "ps_co_nt");
                    decimal ps_nt = t_ps_no_nt - t_ps_co_nt;
                    if (ps_nt >= 0)
                    {
                        NewCtRecord["ps_no_nt"] = 0;
                        NewCtRecord["ps_co_nt"] = ps_nt;
                    }
                    else
                    {
                        NewCtRecord["ps_no_nt"] = -ps_nt;
                        NewCtRecord["ps_co_nt"] = 0;
                    }

                    
                    int sua_tien = 0;
                    int.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"].ToString(), out sua_tien);
                    if (sua_tien == 0)
                    {
                        decimal ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);
                        if (ps_nt >= 0)
                        {
                            NewCtRecord["ps_no"] = 0;
                            NewCtRecord["ps_co"] = SysFunc.Round(ps_nt * ty_gia, StartUp.M_ROUND);
                        }
                        else
                        {
                            NewCtRecord["ps_no"] = SysFunc.Round((-1)*ps_nt * ty_gia, StartUp.M_ROUND);
                            NewCtRecord["ps_co"] = 0;
                        }
                    }
                    else
                    {
                        decimal t_ps_no = SumFunction(StartUp.DsTrans.Tables[1], "ps_no");
                        decimal t_ps_co = SumFunction(StartUp.DsTrans.Tables[1], "ps_co");
                        decimal ps = t_ps_no - t_ps_co;
                        if (ps >= 0)
                        {
                            NewCtRecord["ps_no"] = 0;
                            NewCtRecord["ps_co"] = ps;
                        }
                        else
                        {
                            NewCtRecord["ps_no"] = -ps;
                            NewCtRecord["ps_co"] = 0;
                        }
                    }
                }
                else
                {
                    NewCtRecord["ps_no_nt"] = 0;
                    NewCtRecord["ps_co_nt"] = 0;
                    NewCtRecord["ps_no"] = 0;
                    NewCtRecord["ps_co"] = 0;
                    NewCtRecord["dien_giaii"] = StartUp.DsTrans.Tables[0].DefaultView[0].Row["dien_giai"];
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

        #region GrdCt_RecordDelete
        private void GrdCt_RecordDelete(object sender, Infragistics.Windows.DataPresenter.Events.RecordsDeletedEventArgs e)
        {
            //SmLib.WinAPISenkey.SenKey(ModifierKeys.Alt, Key.D2);
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

           // GrdCtgt.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                (this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus();
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
                        if (record.Cells[0].Value == null || record.Cells[0].Value.ToString() == "")
                            return;
                        NewRowCt();
                        GrdCt.ActiveRecord = GrdCt.Records[GrdCt.Records.Count - 1];
                        //GrdCt.ActiveCell = (GrdCt.ActiveRecord as DataRecord).Cells["tk_i"];
                        GrdCt.ActiveCell = (GrdCt.ActiveRecord as DataRecord).Cells[0];
                    }
                    break;
                case Key.F8:
                    {
                        if (ExMessageBox.Show( 595,StartUp.SysObj, "Có xoá dòng ghi hiện thời?", "Xoá dòng", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
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
                                if (indexRecord == 0 && GrdCt.Records.Count == 0)
                                    GrdCt_AddNewRecord(null, null);
                                if (GrdCt.Records.Count > 0)
                                    GrdCt.ActiveRecord = GrdCt.Records[indexRecord > GrdCt.Records.Count - 1 ? GrdCt.Records.Count - 1 : indexRecord];
                            }
                        }
                    }
                    break;

                default:
                    break;
            }
        }
        #endregion

        #region GrdCtgt_PreviewEditModeEnded
        private void GrdCtgt_PreviewEditModeEnded(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            try
            {
                if (IsInEditMode.Value == false)
                    return;
                if (GrdCtgt.ActiveCell != null)
                {
                    decimal ty_gia = ParseDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"], 0);
                    int sua_tien = 0;
                    int.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["sua_tien"].ToString(), out sua_tien);
                    switch (e.Cell.Field.Name)
                    {

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
                                        if (!string.IsNullOrEmpty(txt.RowResult["ten_kh"].ToString()))
                                            e.Cell.Record.Cells["ten_kh"].Value = txt.RowResult["ten_kh"];
                                        if (!string.IsNullOrEmpty(txt.RowResult["dia_chi"].ToString()))
                                            e.Cell.Record.Cells["dia_chi"].Value = txt.RowResult["dia_chi"];
                                        if (!string.IsNullOrEmpty(txt.RowResult["ma_so_thue"].ToString()))
                                            e.Cell.Record.Cells["ma_so_thue"].Value = txt.RowResult["ma_so_thue"];

                                        e.Cell.Record.Cells["dia_chi_dmkh"].Value = txt.RowResult["dia_chi"];
                                        e.Cell.Record.Cells["ma_so_thue_dmkh"].Value = txt.RowResult["ma_so_thue"];
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region ma_thue
                        case "ma_thue":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                        return;

                                    AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                    if (txt.RowResult == null)
                                        return;
                                    e.Cell.Record.Cells["thue_suat"].Value = txt.RowResult["thue_suat"];
                                    e.Cell.Record.Cells["tk_thue_no"].Value = txt.RowResult["tk_thue_no"];
                                    CellValuePresenter cellTkThueI = CellValuePresenter.FromCell(e.Cell.Record.Cells["tk_thue_no"]);
                                    AutoCompleteTextBox txtTkThueI = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(cellTkThueI.Editor as ControlHostEditor);
                                    if (txtTkThueI.RowResult == null)
                                        txtTkThueI.SearchInit();
                                    if (txtTkThueI.RowResult != null)
                                    {
                                        e.Cell.Record.Cells["tk_cn"].Value = txtTkThueI.RowResult["tk_cn"];
                                    }

                                    decimal tien_nt = ParseDecimal(e.Cell.Record.Cells["t_tien_nt"].Value, 0);
                                    decimal tien = ParseDecimal(e.Cell.Record.Cells["t_tien"].Value, 0);
                                    decimal thue_suat = ParseDecimal(e.Cell.Record.Cells["thue_suat"].Value, 0);

                                    //nếu tiền nt * thuế suất != 0 thì mới gán lại giá trị tiền thuế nt
                                    decimal thue_nt = 0;
                                    if (!StartUp.M_ma_nt0.Equals(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString()))
                                    {
                                        thue_nt = SysFunc.Round((tien_nt * thue_suat) / 100, StartUp.M_ROUND_NT);
                                    }
                                    else
                                    {
                                        thue_nt = SysFunc.Round((tien_nt * thue_suat) / 100, StartUp.M_ROUND);
                                    }
                                    if (thue_nt != 0)
                                    {
                                        e.Cell.Record.Cells["t_thue_nt"].Value = thue_nt;
                                        e.Cell.Record.Cells["t_tt_nt"].Value = tien_nt + thue_nt;
                                    }

                                    //nếu tiền VND * thuế suất != 0 thì mới gán lại giá trị tiền thuế VND
                                    decimal thue = SysFunc.Round((tien * thue_suat) / 100, StartUp.M_ROUND);
                                    if (thue != 0)
                                    {
                                        e.Cell.Record.Cells["t_thue"].Value = thue;
                                        e.Cell.Record.Cells["t_tt"].Value = tien + thue;
                                    }

                                    if (txtMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["t_thue"].Value = e.Cell.Record.Cells["t_thue_nt"].Value;
                                        e.Cell.Record.Cells["t_tt"].Value = e.Cell.Record.Cells["t_tt_nt"].Value;
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region thue_suat
                        case "thue_suat":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value == null ||
                                        (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                        return;
                                    decimal tien_nt = ParseDecimal(e.Cell.Record.Cells["t_tien_nt"].Value, 0);
                                    decimal tien = ParseDecimal(e.Cell.Record.Cells["t_tien"].Value, 0);
                                    decimal thue_suat = ParseDecimal(e.Editor.Value, 0);

                                    //nếu tiền nt * thuế suất != 0 thì mới gán lại giá trị tiền thuế nt
                                    decimal thue_nt = 0;
                                    if (!StartUp.M_ma_nt0.Equals(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString()))
                                    {
                                        thue_nt = SysFunc.Round((tien_nt * thue_suat) / 100, StartUp.M_ROUND_NT);
                                    }
                                    else
                                    {
                                        thue_nt = SysFunc.Round((tien_nt * thue_suat) / 100, StartUp.M_ROUND);
                                    } 
                                    if (thue_nt != 0)
                                    {
                                        e.Cell.Record.Cells["t_thue_nt"].Value = thue_nt;
                                        e.Cell.Record.Cells["t_tt_nt"].Value = tien_nt + thue_nt;
                                    }

                                    //nếu tiền VND * thuế suất != 0 thì mới gán lại giá trị tiền thuế VND
                                    decimal thue = SysFunc.Round((tien * thue_suat) / 100, StartUp.M_ROUND);
                                    if (thue != 0)
                                    {
                                        e.Cell.Record.Cells["t_thue"].Value = thue;
                                        e.Cell.Record.Cells["t_tt"].Value = tien + thue;
                                    }

                                    if (txtMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["t_thue"].Value = e.Cell.Record.Cells["t_thue_nt"].Value;
                                        e.Cell.Record.Cells["t_tt"].Value = e.Cell.Record.Cells["t_tt_nt"].Value;
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region t_thue_nt
                        case "t_thue_nt":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value == null ||
                                         (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                        e.Cell.Record.Cells["t_thue_nt"].Value = 0;

                                    decimal tien_nt = ParseDecimal(e.Cell.Record.Cells["t_tien_nt"].Value, 0);
                                    decimal thue_nt = ParseDecimal(e.Cell.Record.Cells["t_thue_nt"].Value, 0);
                                    //neu thue nt = 0 thì tính lại thue_nt = tien_nt*thue_suat
                                    if (thue_nt == 0)
                                    {
                                        decimal thue_suat = ParseDecimal(e.Cell.Record.Cells["thue_suat"].Value, 0);
                                        if (!StartUp.M_ma_nt0.Equals(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString()))
                                        {
                                            thue_nt = SysFunc.Round((tien_nt * thue_suat / 100), StartUp.M_ROUND_NT);
                                        }
                                        else
                                        {
                                            thue_nt = SysFunc.Round((tien_nt * thue_suat / 100), StartUp.M_ROUND);
                                        }
                                        
                                        e.Cell.Record.Cells["t_thue_nt"].Value = thue_nt;
                                    }
                                    e.Cell.Record.Cells["t_tt_nt"].Value = tien_nt + thue_nt;

                                    //tiền thuế nt thay đổi thì tiền thuế VND cũng thay đổi
                                    decimal thue = SysFunc.Round(thue_nt * ty_gia, StartUp.M_ROUND);
                                    if (thue != 0)
                                    {
                                        e.Cell.Record.Cells["t_thue"].Value = thue;
                                        e.Cell.Record.Cells["t_tt"].Value = ParseDecimal(e.Cell.Record.Cells["t_tien"].Value, 0) + thue;
                                    }

                                    if (txtMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["t_thue"].Value = e.Cell.Record.Cells["t_thue_nt"].Value;
                                        e.Cell.Record.Cells["t_tt"].Value = e.Cell.Record.Cells["t_tt_nt"].Value;
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region t_thue
                        case "t_thue":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value == null ||
                                        (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                        e.Cell.Record.Cells["t_thue"].Value = 0;

                                    decimal tien = ParseDecimal(e.Cell.Record.Cells["t_tien"].Value, 0);
                                    decimal thue = ParseDecimal(e.Editor.Value, 0);
                                    //neu thue = 0 thì tính lại thue = tien*thue_suat
                                    if (thue == 0)
                                    {
                                        decimal thue_suat = ParseDecimal(e.Cell.Record.Cells["thue_suat"], 0);
                                        thue = SysFunc.Round((tien * thue_suat / 100), StartUp.M_ROUND);
                                        e.Cell.Record.Cells["t_thue"].Value = thue;
                                    }
                                    e.Cell.Record.Cells["t_tt"].Value = tien + thue;
                                }
                            }
                            break;
                        #endregion

                        #region t_tien_nt
                        case "t_tien_nt":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                        e.Cell.Record.Cells["t_tien_nt"].Value = 0;

                                    decimal tien_nt = ParseDecimal(e.Cell.Record.Cells["t_tien_nt"].Value, 0);
                                    decimal thue_suat = ParseDecimal(e.Cell.Record.Cells["thue_suat"].Value, 0);

                                    //nếu tiền nt * thuế suất != 0 thì mới gán lại giá trị tiền thuế nt
                                    decimal thue_nt = 0;
                                    if (!StartUp.M_ma_nt0.Equals(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString()))
                                    {
                                        thue_nt = SysFunc.Round((tien_nt * thue_suat) / 100, StartUp.M_ROUND_NT);
                                    }
                                    else
                                    {
                                        thue_nt = SysFunc.Round((tien_nt * thue_suat / 100), StartUp.M_ROUND);
                                    }
                                    if (thue_nt != 0)
                                    {
                                        e.Cell.Record.Cells["t_thue_nt"].Value = thue_nt;

                                        //tiền thuế nt thay đổi thì tiền thuế VND cũng thay đổi
                                        decimal thue = SysFunc.Round(thue_nt * ty_gia, StartUp.M_ROUND);
                                        if (thue != 0)
                                        {
                                            e.Cell.Record.Cells["t_thue"].Value = thue;
                                            e.Cell.Record.Cells["t_tt"].Value = ParseDecimal(e.Cell.Record.Cells["t_tien"].Value, 0) + thue;
                                        }
                                    }

                                    e.Cell.Record.Cells["t_tt_nt"].Value = tien_nt + Convert.ToDecimal(e.Cell.Record.Cells["t_thue_nt"].Value);

                                    decimal tien = ParseDecimal(e.Cell.Record.Cells["t_tien"].Value, 0);
                                    //if (tien == 0 && sua_tien == 0 && ty_gia != 0)
                                    if (tien == 0 && ty_gia != 0)
                                    {
                                        tien = SysFunc.Round(tien_nt * ty_gia, StartUp.M_ROUND);
                                        e.Cell.Record.Cells["t_tien"].Value = tien;

                                        //nếu tiền VND * thuế suất != 0 thì mới gán lại giá trị tiền thuế VND
                                        decimal thue = SysFunc.Round((tien * thue_suat) / 100, StartUp.M_ROUND);
                                        if (thue != 0)
                                        {
                                            e.Cell.Record.Cells["t_thue"].Value = thue;
                                        }
                                        e.Cell.Record.Cells["t_tt"].Value = tien + Convert.ToDecimal(e.Cell.Record.Cells["t_thue"].Value);
                                    }

                                    if (txtMa_nt.Text == StartUp.M_ma_nt0)
                                    {
                                        e.Cell.Record.Cells["t_thue"].Value = e.Cell.Record.Cells["t_thue_nt"].Value;
                                        e.Cell.Record.Cells["t_tt"].Value = e.Cell.Record.Cells["t_tt_nt"].Value;
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region t_tien
                        case "t_tien":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    decimal tien = 0;
                                    if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == "") || (decimal.Parse(e.Editor.Value.ToString()) == 0))
                                    {
                                        //if (sua_tien == 0)
                                        //{
                                        decimal tien_nt = ParseDecimal(e.Cell.Record.Cells["t_tien_nt"].Value, 0);
                                        tien = SysFunc.Round(tien_nt * ty_gia, StartUp.M_ROUND);
                                        //}
                                        e.Cell.Record.Cells["t_tien"].Value = tien;
                                    }

                                    decimal thue_suat = ParseDecimal(e.Cell.Record.Cells["thue_suat"].Value, 0);
                                    tien = ParseDecimal(e.Cell.Record.Cells["t_tien"].Value, 0);
                                    decimal thue = SysFunc.Round((tien * thue_suat) / 100, StartUp.M_ROUND);
                                    if (thue != 0)
                                    {
                                        e.Cell.Record.Cells["t_thue"].Value = thue;
                                    }
                                    e.Cell.Record.Cells["t_tt"].Value = tien + Convert.ToDecimal(e.Cell.Record.Cells["t_thue"].Value);
                                }
                            }
                            break;
                        #endregion

                        #region tk_thue_no
                        case "tk_thue_no":
                            {
                                if (e.Cell.IsDataChanged)
                                {
                                    if (e.Editor.Value == null || (e.Editor.Value != null && e.Editor.Value.ToString().Trim() == ""))
                                        return;
                                    AutoCompleteTextBox txt = Sm.Windows.Controls.ControlLib.ControlFunction.GetAutoCompleteControl(e.Editor as ControlHostEditor);
                                    e.Cell.Record.Cells["tk_cn"].Value = txt.RowResult["tk_cn"];

                                    Debug.WriteLine(e.Cell.Record.Cells["tk_cn"].Value, "tk_thue_no");
                                }
                            }
                            break;
                        #endregion

                        default:
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

        #region GrdCtgt_AddNewRecord
        private bool GrdCtgt_AddNewRecord(object sender, Infragistics.Windows.DataPresenter.Events.EditModeEndedEventArgs e)
        {
            return NewRowCtGt();
        }
        #endregion

        #region NewRowCtGt
        bool NewRowCtGt()
        {
            try
            {
                DataRow NewRecord = StartUp.DsTrans.Tables[2].NewRow();
                NewRecord["stt_rec"] = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"];
                int Stt_rec0 = 1;
                var vtien_nt0_hd = StartUp.DsTrans.Tables[2].AsEnumerable()
                                .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                                .Max(x => x.Field<string>("stt_rec0"));
                if (vtien_nt0_hd != null)
                {
                    int.TryParse(vtien_nt0_hd.ToString(), out Stt_rec0);
                    Stt_rec0++;
                }
                NewRecord["stt_rec0"] = string.Format("{0:000}", Stt_rec0);
                NewRecord["ma_ms"] = StartUp.SysObj.GetOption("M_MA_MS");
                NewRecord["t_tien_nt"] = 0;
                NewRecord["t_tien"] = 0;
                NewRecord["thue_suat"] = 0;
                NewRecord["t_thue_nt"] = 0;
                NewRecord["t_thue"] = 0;
                NewRecord["t_tt_nt"] = 0;
                NewRecord["t_tt"] = 0;
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

        #region GrdCtgt_KeyUp
        private void GrdCtgt_KeyUp(object sender, KeyEventArgs e)
        {
            if (IsInEditMode.Value == false)
                return;

            switch (e.Key)
            {
                case Key.F4:

                    NewRowCtGt();
                    GrdCtgt.ActiveRecord = GrdCtgt.Records[GrdCtgt.Records.Count - 1];
                    GrdCtgt.ActiveCell = (GrdCtgt.ActiveRecord as DataRecord).Cells["ma_ms"];
                    break;
                case Key.F8:
                    {
                        if (ExMessageBox.Show( 600,StartUp.SysObj, "Có xóa dòng ghi hiện thời không?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                        {
                            return;
                        }

                        DataRecord record = (GrdCtgt.ActiveRecord as DataRecord);
                        if (record != null)
                        {
                            int indexCell = GrdCtgt.ActiveCell == null ? 0 : GrdCtgt.ActiveCell.Field.Index;
                            int indexRecord = record.Index;
                            GrdCtgt.ExecuteCommand(DataPresenterCommands.EndEditModeAndDiscardChanges);
                            if (indexCell >= 0)
                            {
                                StartUp.DsTrans.Tables[2].Rows.Remove(StartUp.DsTrans.Tables[2].DefaultView[indexRecord].Row);
                                StartUp.DsTrans.Tables[2].AcceptChanges();
                                if (indexRecord == 0 && GrdCtgt.Records.Count == 0)
                                    GrdCtgt_AddNewRecord(null, null);
                                if (GrdCtgt.Records.Count > 0)
                                    GrdCtgt.ActiveRecord = GrdCtgt.Records[GrdCtgt.Records.Count - 1];
                                
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

        #region GrdCtgt_RecordDelete
        private void GrdCtgt_RecordDelete(object sender, Infragistics.Windows.DataPresenter.Events.RecordsDeletedEventArgs e)
        {
           // GrdCtgt.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                (this.Toolbar.FindName("btnSave") as SmVoucherLib.ToolBarButton).Focus();
            }));
        }
        #endregion

        #region Check sửa tiền
        private void ChkSuatien_Click(object sender, RoutedEventArgs e)
        {
            if (ChkSuatien.IsChecked == false && sender.GetType().Name.Equals("CheckBox"))
            {
                //txtTy_gia.Focus();
                Ty_gia_ValueChanged(false);
            }
            IsVisibilityFieldsXamDataGridBySua_Tien();
        }

        #endregion

        #region SumFunction
        decimal SumFunction(DataTable datatable, string columnname)
        {
            decimal result = 0;
            //decimal.TryParse(datatable.Compute("sum(" + columnname + ")", "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'").ToString(), out result);
            var SumTotal = datatable.AsEnumerable()
                        .Where(b => b.Field<string>("stt_rec") == StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString())
                        .Sum(x => x.Field<decimal?>(columnname));
            if (SumTotal != null)
                decimal.TryParse(SumTotal.ToString(), out result);
            return result;
        }
        #endregion

        #region GetLanguageString
        public override string GetLanguageString(string code, string language)
        {
            return StartUp.GetLanguageString(code, language);
        }
        #endregion

        #region FormMain_Closed
        private void FormMain_Closed(object sender, EventArgs e)
        {
            //if currActinTask đang là none thì thoát luôn
            //nếu không phải là None thì Hủy phiếu đang thực hiện
            try
            {
                if (currActionTask == ActionTask.None || IsInEditMode.Value == false)
                    return;
                V_Huy();
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        private void FormMain_EditModeEnded(object sender, string menuItemName, RoutedEventArgs e)
        {
            Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
            Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));
        }
    }
}
