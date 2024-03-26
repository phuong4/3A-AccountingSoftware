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
using SmReport;
using SmDefine;

namespace Glctpk1
{
    /// <summary>
    /// Interaction logic for FrmSearchGlctpk1.xaml
    /// </summary>
    public partial class FrmSearchGlctpk1 : FormFilter
    {
        public FrmSearchGlctpk1(SysLib.SysObject _SysObj, string _filterID, string _tableList)
        {
            InitializeComponent();            
            this.SysObj = _SysObj;
            this.BindingSysObj = _SysObj;
            GridSearch.filterID = _filterID;
            GridSearch.tableList = _tableList;
            GridSearch.SysObj = _SysObj;

        }
        public SysLib.SysObject SysObj
        {
            get { return (SysLib.SysObject)GetValue(SysObjProperty); }
            set { SetValue(SysObjProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SysObj.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SysObjProperty =
            DependencyProperty.Register("SysObj", typeof(SysLib.SysObject), typeof(FrmSearchGlctpk1), new UIPropertyMetadata(null));

     

        #region FrmSearchGlctpk1_Loaded
        void FrmSearchGlctpk1_Loaded(object sender, RoutedEventArgs e)
        {
            txtSo_ct1.Focus();
            txtNgay_ct1.Value = (DateTime)SysObj.GetSysVar("M_ngay_ct1");
            txtNgay_ct2.Value = (DateTime)SysObj.GetSysVar("M_ngay_ct2");
            txtloc_nsd.Value = StartUp.DmctInfo["m_loc_nsd"] == DBNull.Value ? 0 : StartUp.DmctInfo["m_loc_nsd"];

            // set default date  
           // txtMaDVCS.Text = "";
            DataTable dvcs = StartUp.SysObj.DmdvcsInfo;

            if (dvcs.Rows.Count > 0)
                txtMaDVCS.Text = dvcs.Rows[0]["ma_dvcs"].ToString();
            
            txtMaDVCS.SearchInit();
            txttk.SearchInit();
            if (StartUp.SysObj.GetOption("M_LAN").ToString().ToUpper().Equals("V"))
            {
                if (txtMaDVCS.RowResult != null)
                    lblTenDVCS.Text = txtMaDVCS.RowResult["ten_dvcs"].ToString();
                if (txttk.RowResult != null)
                    tblten_tk.Text = txttk.RowResult["ten_tk"].ToString();
            }
            else
            {
                if (txtMaDVCS.RowResult != null)
                    lblTenDVCS.Text = txtMaDVCS.RowResult["ten_dvcs2"].ToString();
                if (txttk.RowResult != null)
                    tblten_tk.Text = txttk.RowResult["ten_tk2"].ToString();
            }
            txtghi_no_co.Text = "*";
        }
        #endregion

        #region GetPhFilterExpr
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
        #endregion

        #region GetCtFilterExpr
        private string GetCtFilterExpr()
        {
            string sCtFilter = "1=1 ";
            if (!string.IsNullOrEmpty(txttk.Text))
                sCtFilter += " and tk_i like " + ConvertDataToSql(txttk.Text.Trim() + "%", typeof(string));
            if (txtghi_no_co.Text.Trim().Equals("1"))
                sCtFilter += " and ps_no<>0 ";
            else if (txtghi_no_co.Text.Trim().Equals("2"))
                sCtFilter += " and ps_co<>0 ";
            
            if (!string.IsNullOrEmpty(GridSearch.arrStrFilter[1]))
            {
                sCtFilter += " and " + GridSearch.arrStrFilter[1];
            }
            return sCtFilter;
        }
        #endregion

        #region GetCtgtFilterExpr
        private string GetCtgtFilterExpr()
        {
            string sCtgtFilter = "1=1 ";
            if (!string.IsNullOrEmpty(GridSearch.arrStrFilter[2]))
            {
                sCtgtFilter += " and " + GridSearch.arrStrFilter[2];
            }
            return sCtgtFilter;
        }
        #endregion

        #region ConvertDataToSql
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
        #endregion

        #region grdConfirm_OnOk
        private void grdConfirm_OnOk(object sender, RoutedEventArgs e)
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
                if (CheckValid())
                {
                    SysObj.SetSysvar("M_ngay_ct1", txtNgay_ct1.dValue);
                    SysObj.SetSysvar("M_ngay_ct2", txtNgay_ct2.dValue);
                    bool IsshowView = false;

                    GridSearch._GenerateSQLString();
                    GridSearch.GrdSearch.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);

                    StartUp.TransFilterCmd.Parameters["@PhFilter"].Value = GetPhFilterExpr(); // "ngay_ct between '20100101' and '20100131'";
                    StartUp.TransFilterCmd.Parameters["@CtFilter"].Value = GetCtFilterExpr();
                    StartUp.TransFilterCmd.Parameters["@GtFilter"].Value = GetCtgtFilterExpr();
                    StartUp.TransFilterCmd.Parameters["@Sl_ct"].Value = 0;

                    DataSet newDs = DataProvider.FillCommand(StartUp.SysObj, StartUp.TransFilterCmd);

                    // xuất thông báo tìm kiếm
                    int n = 0;
                    Decimal a = (from p
                                    in newDs.Tables[0].AsEnumerable()
                                 select p.Field<Decimal?>("t_tien")).Sum().Value;

                    string tongPsVND = a.ToString(SysObj.GetOption("M_IP_TIEN").ToString());
                    Decimal tongPsNT = (from p
                                    in newDs.Tables[0].AsEnumerable()
                                        select p.Field<Decimal?>("t_tien_nt")).Sum().Value;
                    string _tongPsNT = tongPsNT.ToString(SysObj.GetOption("M_IP_TIEN_NT").ToString());
                    n = newDs.Tables[0].Rows.Count;
                    //a = StartUp.DsTrans.Tables[0].AsEnumerable().Sum("t_tt").Value.ToString();
                    if (n > 0)
                    {
                        IsshowView = true;
                        Sm.Windows.Controls.ExMessageBox.Show( 610,StartUp.SysObj, "Có " + "[" + n + "]" + " chứng từ. Tổng phát sinh " + "[" + _tongPsNT + "]" + " / " + "[" + tongPsVND + "]", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        Sm.Windows.Controls.ExMessageBox.Show( 615,StartUp.SysObj, "Không có chứng từ nào như vậy!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    if (IsshowView == true)
                    {
                        // show form View
                        //string stringBrowse1 = "ngay_ct:fl:100:h=Ngày c.từ; so_ct:fl:70:h=Số c.từ; t_tien_nt:n1:130:h=Tổng phát sinh; ty_gia:r:130:h=Tỷ giá; t_tien:n0:130:h=Tổng ps " + StartUp.M_ma_nt0 + "; date:d:105:h=Ngày cập nhật; time:100:h=Giờ cập nhật; user_id:n:100:h=Số hiệu NSD; user_name:100:h=Tên NSD";
                        //string stringBrowse2 = "tk_i:fl:80:h=Tài khoản; ps_no_nt:n1:130:h=Ps nợ nt; ps_co_nt:n1:130:h=Ps có nt; dien_giaii:225:h=Diễn giải; ps_no:n0:130:h=Ps nợ " + StartUp.M_ma_nt0 + "; ps_co:n0:130:h=Ps có " + StartUp.M_ma_nt0;

                        SmVoucherLib.FormView _frmView = new SmVoucherLib.FormView(SysObj, newDs.Tables[0].DefaultView, newDs.Tables[1].DefaultView, StartUp.stringBrowse1, StartUp.stringBrowse2, "stt_rec");
                        _frmView.ListFieldSum = "t_tien_nt;t_tien";
                        _frmView.frmBrw.Title = StartUp.M_Tilte + (StartUp.M_LAN.Equals("V") ? ". Ky " : ". Period ") + txtNgay_ct1.Text + " - " + txtNgay_ct2.Text;
                        //Them cac truong tu do
                        SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, _frmView.frmBrw.oBrowseCt, StartUp.Ma_ct, 1);



                        _frmView.frmBrw.LanguageID  = "Glctpk1ViewSearch";
                        _frmView.ShowDialog();

                        StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString());

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

                        // ko xoá dòng thứ 0 của table[0] vì dòng đó là dòng tạm.

                        if (newDs.Tables[0].Rows.Count > 0)
                        {
                            //Xét lại irow
                            if (FrmGlctpk1.iRow > newDs.Tables[0].Rows.Count - 1)
                                FrmGlctpk1.iRow = newDs.Tables[0].Rows.Count - 1;

                            StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[FrmGlctpk1.iRow]["stt_rec"].ToString());
                        }

                        // Set lai irow va rowfilter ...
                        if (_frmView.DataGrid.ActiveRecord != null)
                        {
                            int select_irow = (_frmView.DataGrid.ActiveRecord as DataRecord).Index;
                            if (select_irow >= 0)
                            {
                                string selected_stt_rec = (_frmView.DataGrid.DataSource as DataView)[select_irow]["stt_rec"].ToString();
                                FrmGlctpk1.iRow = select_irow + 1;
                                //refresh lại rowfilter
                                StartUp.DataFilter(selected_stt_rec);

                            }
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
        #endregion

        #region CheckValid
        bool CheckValid()
        {
            bool result = true;
            if (result && (txtNgay_ct1.Value == null || txtNgay_ct1.Value.ToString() == ""))
            {

                ExMessageBox.Show( 620,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNgay_ct1.Focus();
                result = false;
            }
            if (result && !txtNgay_ct1.IsValueValid)
            {
                ExMessageBox.Show( 625,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                result = false;
                txtNgay_ct1.Focus();
                txtNgay_ct1.SelectAll();
            }
            if (result && (txtNgay_ct2.Value == null || txtNgay_ct2.Value.ToString() == ""))
            {
                ExMessageBox.Show( 630,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNgay_ct2.Focus();
                result = false;
            }
            if (result && !txtNgay_ct2.IsValueValid)
            {
                ExMessageBox.Show( 635,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                result = false;
                txtNgay_ct2.Focus();
                txtNgay_ct2.SelectAll();
            }
            if (result && Convert.ToDateTime(txtNgay_ct1.Value) > Convert.ToDateTime(txtNgay_ct2.Value))
            {
                ExMessageBox.Show( 640,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNgay_ct1.Focus();
                result = false;
            }
            return result;
        }
        #endregion

        #region txtghi_no_co_LostFocus
        private void txtghi_no_co_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtghi_no_co.Text.Trim() == "")
                txtghi_no_co.Text = "*";
        }
        #endregion

        #region txtloc_nsd_LostFocus
        private void txtloc_nsd_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtloc_nsd.Text.Trim() == "")
                txtloc_nsd.Value = 1;
        }
        #endregion

        #region txttk_LostFocus
        private void txttk_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txttk.RowResult != null)
            {
                if (StartUp.M_LAN == "V")
                    tblten_tk.Text = txttk.RowResult["ten_tk"].ToString();
                else
                    tblten_tk.Text = txttk.RowResult["ten_tk2"].ToString();
            }
            else
                tblten_tk.Text = "";
        }
        #endregion

        private void txtMaDVCS_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtMaDVCS.RowResult != null)
            {
                if (StartUp.M_LAN == "V")
                    lblTenDVCS.Text = txtMaDVCS.RowResult["ten_dvcs"].ToString();
                else
                    lblTenDVCS.Text = txtMaDVCS.RowResult["ten_dvcs2"].ToString();
            }
            else
                lblTenDVCS.Text = "";
        }
    }
}

