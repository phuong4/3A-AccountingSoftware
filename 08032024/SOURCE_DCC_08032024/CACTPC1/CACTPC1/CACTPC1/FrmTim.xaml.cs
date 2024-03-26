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
using System.Windows.Shapes;
using Sm.Windows.Controls;
using System.Data;
using SmVoucherLib;
using Infragistics.Windows.DataPresenter;
using System.Windows.Threading;
using System.Data.SqlClient;
using ArapLib;
using System.Diagnostics;
using SmReport;
using SmDefine;

namespace CACTPC1
{
    /// <summary>
    /// Interaction logic for FrmTim.xaml
    /// </summary>
    public partial class FrmTim : FormFilter
    {
        public FrmTim()
        {
            InitializeComponent();
            this.BindingSysObj = StartUp.SysObj;
        }
        public SysLib.SysObject SysObj
        {
            get { return (SysLib.SysObject)GetValue(SysObjProperty); }
            set { SetValue(SysObjProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SysObj.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SysObjProperty =
            DependencyProperty.Register("SysObj", typeof(SysLib.SysObject), typeof(FrmTim), new UIPropertyMetadata(null));



        public FrmTim(SysLib.SysObject _SysObj, string _filterID, string _tableList)
        {
            InitializeComponent();

            this.SysObj = _SysObj;
            //GridSearch = new SmReport.ControlDetailFilterWin();
            this.BindingSysObj = _SysObj;
            GridSearch.filterID = _filterID;
            GridSearch.tableList = _tableList;
            GridSearch.SysObj = _SysObj;

            this.Title = SmLib.SysFunc.Cat_Dau(this.Title);

        }

        void FrmTim_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        void FrmTim_Loaded(object sender, RoutedEventArgs e)
        {

            txtSo_ct1.Focus();
            //DateTime firstdate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).Date;
            //DateTime lastdate = new DateTime(DateTime.Today.Year, DateTime.Today.Month + 1, 1).Date.AddDays(-1);
            txtloc_nsd.Value = StartUp.DmctInfo["m_loc_nsd"] == DBNull.Value ? 0 : StartUp.DmctInfo["m_loc_nsd"];
            txtNgay_ct1.Value = (DateTime)SysObj.GetSysVar("M_ngay_ct1");
            txtNgay_ct2.Value = (DateTime)SysObj.GetSysVar("M_ngay_ct2");
            txtSo_ct1.Focus();

            txtMa_kh.SearchInit();
            txtTk_co.SearchInit();
            txtMaDVCS.SearchInit();
            lblTenDVCS.Text = txtMaDVCS.RowResult == null ? "" : (StartUp.SysObj.GetOption("M_LAN").ToString() == "V" ? txtMaDVCS.RowResult["ten_dvcs"].ToString() : txtMaDVCS.RowResult["ten_dvcs2"].ToString());
            lbltenkh.Text = txtMa_kh.RowResult == null ? "" : (StartUp.SysObj.GetOption("M_LAN").ToString() == "V" ? txtMa_kh.RowResult["ten_kh"].ToString() : txtMa_kh.RowResult["ten_kh2"].ToString());
            lbltentk.Text = txtTk_co.RowResult == null ? "" : (StartUp.SysObj.GetOption("M_LAN").ToString() == "V" ? txtTk_co.RowResult["ten_tk"].ToString() : txtTk_co.RowResult["ten_tk2"].ToString());
            // set default date  
            //txtMaDVCS.Text = "";
            //DataTable dvcs = StartUp.SysObj.DmdvcsInfo;

            //if (dvcs.Rows.Count > 0)
            //    txtMaDVCS.Text = dvcs.Rows[0]["ma_dvcs"].ToString();
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    
                    DataSet ds = StartUp.SysObj.ExcuteReader(new SqlCommand("select CAST(count(1) as Numeric(16, 0)) from dmdvcs"));
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        txtMaDVCS.IsReadOnly = FNum.ToDec(ds.Tables[0].Rows[0][0]) <= 1;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }), DispatcherPriority.Background);
       
        }

        private string GetPhFilterExpr()
        {
            int maxlenghtSo_ct = BindingSysObj.GetDatabaseFieldLength("so_ct");
            string sPhFilter = "1=1 ";
            if (!string.IsNullOrEmpty(txtNgay_ct1.Text))
            {
                sPhFilter += " and ngay_ct >= " + ConvertDataToSql(txtNgay_ct1.Value, typeof(DateTime));
            }
            if (!string.IsNullOrEmpty(txtNgay_ct2.Text))
            {
                sPhFilter += " and ngay_ct <= " + ConvertDataToSql(txtNgay_ct2.Value, typeof(DateTime));
            }
            if (!string.IsNullOrEmpty(txtSo_ct1.Text))
            {
                sPhFilter += " and so_ct >= '" + txtSo_ct1.Text.Trim().PadLeft(maxlenghtSo_ct, ' ') + "'";
            }
            if (!string.IsNullOrEmpty(txtSo_ct2.Text))
            {
                sPhFilter += " and so_ct <= '" + txtSo_ct2.Text.Trim().PadLeft(maxlenghtSo_ct, ' ') + "'";
            }
            if (!string.IsNullOrEmpty(txtMa_kh.Text))
            {
                sPhFilter += " and ma_kh = " + ConvertDataToSql(txtMa_kh.Text.Trim(), typeof(string));
            }
            if (!string.IsNullOrEmpty(txtTk_co.Text))
            {
                sPhFilter += " and tk like " + ConvertDataToSql(txtTk_co.Text.Trim() + "%", typeof(string));
            }
            if (Convert.ToInt16(txtloc_nsd.Value) == 1)
            {
                sPhFilter += " and [user_id] = " + StartUp.M_User_Id;
            }
            if (!string.IsNullOrEmpty(txtMaDVCS.Text))
            {
                sPhFilter += " and ma_dvcs LIKE '" + txtMaDVCS.Text.Trim() + "%'";
            }

            if (!SmLib.SysFunc.CheckPermission(SysObj, ActionTask.View, StartUp.Menu_Id))
                sPhFilter += " and " + " AND user_id0 = " + SysObj.UserInfo.Rows[0]["user_id"].ToString();

            if (!string.IsNullOrEmpty(GridSearch.arrStrFilter[0]))
            {
                sPhFilter += " and " + GridSearch.arrStrFilter[0];
            }
            return sPhFilter;
        }
        private string GetCtFilterExpr()
        {
            string sCtFilter = "1=1";
            if (!string.IsNullOrEmpty(GridSearch.arrStrFilter[1]))
            {
                sCtFilter += " and " + GridSearch.arrStrFilter[1];
            }
            return sCtFilter;
        }
        private string GetCtgtFilterExpr()
        {
            string sCtgtFilter = "1=1";
            if (!string.IsNullOrEmpty(GridSearch.arrStrFilter[2]))
            {
                sCtgtFilter += " and " + GridSearch.arrStrFilter[2];
            }
            return sCtgtFilter;
        }
        public string ConvertDataToSql(object value, Type ValueType)
        {
            string sResult = "";
            switch (ValueType.ToString())
            {
                case "System.String":
                    sResult = string.Format("'{0}'", (value as string).Replace("'", "'"));
                    break;
                case "System.DateTime":
                    sResult = string.Format("'{0}'", ((DateTime)value).ToString("yyyyMMdd"));
                    break;
                default:
                    sResult = string.Format("'{0}'", value);
                    break;
            }

            return sResult;
        }

        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Keyboard.FocusedElement.GetType().Equals(typeof(TextBoxAutoComplete)))
                {
                    AutoCompleteTextBox txt = (Keyboard.FocusedElement as TextBoxAutoComplete).ParentControl;
                    if (!txt.CheckLostFocus())
                    {
                        return;
                    }
                }
                if (IsHopLe())
                {
                    SysObj.SetSysvar("M_ngay_ct1",txtNgay_ct1.dValue);
                    SysObj.SetSysvar("M_ngay_ct2", txtNgay_ct2.dValue);
                    bool IsshowView = false;

                    GridSearch._GenerateSQLString();
                    GridSearch.GrdSearch.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);

                    StartUp.TransFilterCmd.Parameters["@PhFilter"].Value = GetPhFilterExpr(); // "ngay_ct between '20100101' and '20100131'";
                    StartUp.TransFilterCmd.Parameters["@CtFilter"].Value = GetCtFilterExpr();
                    StartUp.TransFilterCmd.Parameters["@GtFilter"].Value = GetCtgtFilterExpr();
                    StartUp.TransFilterCmd.Parameters["@sl_ct"].Value = 0;

                    DataSet newDs = DataProvider.FillCommand(StartUp.SysObj, StartUp.TransFilterCmd);
                    // xuất thông báo tìm kiếm
                    int n = 0;
                    Decimal a = (from p
                                    in newDs.Tables[0].AsEnumerable()
                                  select p.Field<Decimal?>("t_tt")).Sum().Value;

                    string tongPsVND = a.ToString(SysObj.GetOption("M_IP_TIEN").ToString());
                    Decimal tongPsNT = (from p
                                    in newDs.Tables[0].AsEnumerable()
                                        select p.Field<Decimal?>("t_tt_nt")).Sum().Value;
                    string _tongPsNT = tongPsNT.ToString(SysObj.GetOption("M_IP_TIEN_NT").ToString());
                    n = newDs.Tables[0].Rows.Count ;
                    //a = StartUp.DsTrans.Tables[0].AsEnumerable().Sum("t_tt").Value.ToString();
                    if (n > 0)
                    {
                        IsshowView = true;
                        Sm.Windows.Controls.ExMessageBox.Show( 825,StartUp.SysObj, "Có " + "[" + n + "]" + " chứng từ. Tổng phát sinh  " + "[" + _tongPsNT + "]" + " / " + "[" + tongPsVND + "]", "FK.NET", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        Sm.Windows.Controls.ExMessageBox.Show( 830,StartUp.SysObj, "Không có chứng từ nào như vậy! ", "FK.NET", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    if (IsshowView == true)
                    {
                        // show form View
                        string stringBrowse1 = StartUp.CommandInfo[StartUp.M_LAN.Equals("V")?"Vbrowse2":"Ebrowse2"].ToString().Split('|')[0];
                        string stringBrowse2 = StartUp.CommandInfo[StartUp.M_LAN.Equals("V")?"Vbrowse2":"Ebrowse2"].ToString().Split('|')[1];
                        SmVoucherLib.FormView _frmView = new SmVoucherLib.FormView(SysObj, newDs.Tables[0].DefaultView, newDs.Tables[1].DefaultView, stringBrowse1, stringBrowse2, "stt_rec");
                        SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, _frmView.frmBrw.oBrowseCt, StartUp.Ma_ct, 1);
                        _frmView.frmBrw.Title = (SmLib.SysFunc.Cat_Dau(StartUp.M_LAN.Equals("V") ? StartUp.CommandInfo["bar"].ToString() + ". Ky " : StartUp.CommandInfo["bar2"].ToString() + ". Period ") + txtNgay_ct1.Text + " - " + txtNgay_ct2.Text);

                        _frmView.ListFieldSum = "t_tt_nt;t_tt";


                        _frmView.frmBrw.LanguageID  = "CACTPC1_6";
                        _frmView.ShowDialog();


                        // thiếu đoạn gán thông tin tìm được sang cho StartUp.DsTrans
                        //DataRow dr = newDs.Tables[0].NewRow();
                        //dr["stt_rec"] = string.Empty;
                        //dr["ma_nt"] =StartUp.M_ma_nt0;
                        //newDs.Tables[0].Rows.InsertAt(dr, 0);

                        StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString() + "'";
                        StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString() + "'";
                        StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString() + "'";
                        int Count1 = StartUp.DsTrans.Tables[0].Rows.Count;
                        int Count2 = StartUp.DsTrans.Tables[1].Rows.Count;
                        int Count3 = StartUp.DsTrans.Tables[2].Rows.Count;

                        for (int i = Count1 - 1; i >= 1; i--)
                            StartUp.DsTrans.Tables[0].Rows.RemoveAt(i);

                        for (int i = 0; i < Count2; i++)
                            StartUp.DsTrans.Tables[1].Rows.RemoveAt(0);

                        for (int i = 0; i < Count3; i++)
                            StartUp.DsTrans.Tables[2].Rows.RemoveAt(0);


                        int Count = 0;
                        Count = newDs.Tables[0].Rows.Count;
                        for (int i = 0; i < Count; i++)
                        {
                            StartUp.DsTrans.Tables[0].Rows.Add(newDs.Tables[0].Rows[i].ItemArray);
                        }

                        Count = newDs.Tables[1].Rows.Count;
                        for (int i = 0; i < Count; i++)
                        {
                            StartUp.DsTrans.Tables[1].Rows.Add(newDs.Tables[1].Rows[i].ItemArray);
                        }
                        Count = newDs.Tables[2].Rows.Count;
                        for (int i = 0; i < Count; i++)
                        {
                            StartUp.DsTrans.Tables[2].Rows.Add(newDs.Tables[2].Rows[i].ItemArray);
                        }
                        //StartUp.DsTrans.Tables[0].DefaultView.RowFilter()

                        // ko xoá dòng thứ 0 của table[0] vì dòng đó là dòng tạm.


                        //StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec='" + StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString() + "'";
                        //StartUp.DsTrans.Tables[0].DefaultView.DataViewManager.CreateDataView(StartUp.DsTrans.Tables[0]);
                        //StartUp.DsTrans.Tables[0].DefaultView.DataViewManager.CreateDataView(StartUp.DsTrans.Tables[0]);
                        //StartUp.DsTrans.Tables[0].DefaultView.DataViewManager.CreateDataView(StartUp.DsTrans.Tables[0]);
                        //if (newDs.Tables[0].Rows.Count > 0)
                        //{
                        //    //Xét lại irow
                        //    if (FrmCACTPC1.iRow > newDs.Tables[0].Rows.Count - 1)
                        //        FrmCACTPC1.iRow = newDs.Tables[0].Rows.Count - 1;

                        //    StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[FrmCACTPC1.iRow]["stt_rec"].ToString() + "'";
                        //    StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[FrmCACTPC1.iRow]["stt_rec"].ToString() + "'";
                        //    StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[FrmCACTPC1.iRow]["stt_rec"].ToString() + "'";
                        //}
                        //LoadData();

                        // Set lai irow va rowfilter ...
                        if (_frmView.DataGrid.ActiveRecord != null)
                        {
                            int select_irow = (_frmView.DataGrid.ActiveRecord as DataRecord).Index;

                            // Nếu user chọn các dong ko phai DataRow thì set lai select_irow = 0
                            if (select_irow == -1)
                                select_irow = 0;

                            string selected_stt_rec = (_frmView.DataGrid.DataSource as DataView)[select_irow]["stt_rec"].ToString();
                            FrmCACTPC1.iRow = select_irow + 1;
                            StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + selected_stt_rec + "'";
                            StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + selected_stt_rec + "'";
                            StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + selected_stt_rec + "'";
                        }

                        this.Close();
                    }
                }
               
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        //void LoadData()
        //{
        //    vPh = new DataView(StartUp.DsTrans.Tables[0]);
        //    vPh.RowFilter = "stt_rec <> ''";
        //    vCt = new DataView(StartUp.DsTrans.Tables[1]);
        //    if (vPh.Count > 0)
        //    {
        //        vCt.RowFilter = "stt_rec='" + vPh[0]["stt_rec"] + "'";

        //        GrdPh.DataSource = vPh;
        //        GrdCt.DataSource = vCt;
        //        GrdPh.ActiveRecord = (GrdPh.Records[0] as DataRecord);
        //    }
        //}

        private void ConfirmGridView_OnCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private bool IsHopLe()
        {
            if (String.IsNullOrEmpty(txtNgay_ct1.Value.ToString()) || !txtNgay_ct1.IsValueValid)
            {
                Sm.Windows.Controls.ExMessageBox.Show( 835,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "FK.NET", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNgay_ct1.Focus();
                txtNgay_ct1.SelectAll();
                return false;
            }
            else if (String.IsNullOrEmpty(txtNgay_ct2.Value.ToString()) || !txtNgay_ct2.IsValueValid)
            {
                Sm.Windows.Controls.ExMessageBox.Show( 840,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "FK.NET", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNgay_ct2.Focus();
                txtNgay_ct2.SelectAll();
                return false;
            }
            else if (txtNgay_ct1.dValue > txtNgay_ct2.dValue)
            {
                Sm.Windows.Controls.ExMessageBox.Show( 845,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNgay_ct1.Focus();
                txtNgay_ct1.SelectAll();
                return false;
            }
            else
            {
                return true;
            }
        }

        private void txtloc_nsd_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtloc_nsd.Text))
                txtloc_nsd.Value = StartUp.DmctInfo["m_loc_nsd"] == DBNull.Value ? 0 : StartUp.DmctInfo["m_loc_nsd"];
        }

        private void txtMa_kh_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtMa_kh.Text.Trim()))
            {
                lbltenkh.Text = "";
            }
            else if (txtMa_kh.RowResult != null)
            {
                if (StartUp.M_LAN.ToUpper().Equals("V"))
                    lbltenkh.Text = txtMa_kh.RowResult["ten_kh"].ToString();
                else
                    lbltenkh.Text = txtMa_kh.RowResult["ten_kh2"].ToString();
            }
        }

        private void txtTk_co_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtTk_co.Text.Trim()))
            {
                lbltentk.Text = "";
            }
            else if (txtTk_co.RowResult != null)
            {
                if (StartUp.M_LAN.ToUpper().Equals("V"))
                    lbltentk.Text = txtTk_co.RowResult["ten_tk"].ToString();
                else
                    lbltentk.Text = txtTk_co.RowResult["ten_tk2"].ToString();
            }
        }

        private void txtMaDVCS_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtMaDVCS.RowResult != null)
                lblTenDVCS.Text = txtMaDVCS.RowResult["ten_dvcs"].ToString();
            else
                lblTenDVCS.Text = "";
        }
    }
}
