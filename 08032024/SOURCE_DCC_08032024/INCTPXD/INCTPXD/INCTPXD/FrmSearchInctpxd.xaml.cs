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

namespace Inctpxd
{
    /// <summary>
    /// Interaction logic for FrmSearchGlctpk1.xaml
    /// </summary>
    public partial class FrmSearchInctpxd : FormFilter
    {
        public FrmSearchInctpxd(SysLib.SysObject _SysObj, string _filterID, string _tableList)
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
        //public static readonly DependencyProperty SysObjProperty =
        //    DependencyProperty.Register("SysObj", typeof(SysLib.SysObject), typeof(FrmSearchInctpxd), new UIPropertyMetadata(null));



        #region FrmSearchSocthda_Loaded
        void FrmSearchSocthda_Loaded(object sender, RoutedEventArgs e)
        {
            txtMaDVCS.SearchInit();
            txtMa_kh.SearchInit();
            txtMa_vt.SearchInit();
            if (StartUp.SysObj.GetOption("M_LAN").ToString().ToUpper().Equals("V"))
            {
                if (txtMaDVCS.RowResult != null)
                    lblTenDVCS.Text = txtMaDVCS.RowResult["ten_dvcs"].ToString();
                if (txtMa_kh.RowResult != null)
                    tblTen_kh.Text = txtMa_kh.RowResult["ten_kh"].ToString();
                if (txtMa_vt.RowResult != null)
                    tblTen_vt.Text = txtMa_vt.RowResult["ten_vt"].ToString();
            }
            else
            {
                if (txtMaDVCS.RowResult != null)
                    lblTenDVCS.Text = txtMaDVCS.RowResult["ten_dvcs2"].ToString();
                if (txtMa_kh.RowResult != null)
                    tblTen_kh.Text = txtMa_kh.RowResult["ten_kh2"].ToString();
                if (txtMa_vt.RowResult != null)
                    tblTen_vt.Text = txtMa_vt.RowResult["ten_vt2"].ToString();
            }
            txtSo_ct1.Focus();
            txtNgay_ct1.Value = (DateTime)SysObj.GetSysVar("M_ngay_ct1");
            txtNgay_ct2.Value = (DateTime)SysObj.GetSysVar("M_ngay_ct2");
            txtloc_nsd.Value = StartUp.DmctInfo["m_loc_nsd"] == DBNull.Value ? 0 : StartUp.DmctInfo["m_loc_nsd"];

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
                //int soct1;
                //bool isNumber1 = int.TryParse(txtSo_ct1.Text, out soct1);
                //if (isNumber1 == false)
                //{
                //    sPhFilter += " and so_ct >= '" + txtSo_ct1.Text.Trim().PadLeft(maxlenghtSo_ct, ' ') + "'";
                //}
                //else
                //{
                //    sPhFilter += "and IsNumeric(so_ct)=1 and  so_ct >= " + soct1;
                //}
                sPhFilter += " and so_ct >= '" + txtSo_ct1.Text.Trim().PadLeft(maxlenghtSo_ct, ' ') + "'";
            }
            if (!string.IsNullOrEmpty(txtSo_ct2.Text))
            {
                //int soct2;
                //bool isNumber2 = int.TryParse(txtSo_ct2.Text, out soct2);
                //if (isNumber2 == false)
                //{
                //    sPhFilter += " and so_ct <= '" + txtSo_ct2.Text.Trim().PadLeft(maxlenghtSo_ct, ' ') + "'";
                //}
                //else
                //{
                //    sPhFilter += "and IsNumeric(so_ct)=1 and  so_ct <= " + soct2;
                //}
                sPhFilter += " and so_ct <= '" + txtSo_ct2.Text.Trim().PadLeft(maxlenghtSo_ct, ' ') + "'";
            }
            if (!string.IsNullOrEmpty(txtMa_kh.Text))
            {
                sPhFilter += " and ma_kh = " + ConvertDataToSql(txtMa_kh.Text.Trim(), typeof(string));
            }
            if (!string.IsNullOrEmpty(txtMa_gd.Text))
                sPhFilter += " and ma_gd = " + ConvertDataToSql(txtMa_gd.Text.Trim(), typeof(string));
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
            if (!string.IsNullOrEmpty(txtMa_vt.Text))
                sCtFilter += " and ma_vt = " + ConvertDataToSql(txtMa_vt.Text.Trim(), typeof(string));


            if (!string.IsNullOrEmpty(GridSearch.arrStrFilter[1]))
            {
                sCtFilter += " and " + GridSearch.arrStrFilter[1];
            }
            return sCtFilter;
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
                        Sm.Windows.Controls.ExMessageBox.Show( 1080,StartUp.SysObj, "Có " + "[" + n + "]" + " chứng từ. Tổng phát sinh " + "[" + _tongPsNT + "]" + " / " + "[" + tongPsVND + "]", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        Sm.Windows.Controls.ExMessageBox.Show( 1085,StartUp.SysObj, "Không có chứng từ nào như vậy!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    if (IsshowView == true)
                    { 
                        SmVoucherLib.FormView _frmView = new SmVoucherLib.FormView(SysObj, newDs.Tables[0].DefaultView, newDs.Tables[1].DefaultView, StartUp.stringBrowse1, StartUp.stringBrowse2, "stt_rec");
                        _frmView.ListFieldSum = "t_tien_nt;t_tien";

                        _frmView.frmBrw.Title = StartUp.M_Tilte + (StartUp.M_LAN.Equals("V") ? ". Ky " : ". Period ") + txtNgay_ct1.Text + " - " + txtNgay_ct2.Text;
                        //Them cac truong tu do
                        SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, _frmView.frmBrw.oBrowseCt, StartUp.Ma_ct, 1);

                        _frmView.frmBrw.LanguageID  = "Inctpxd_6";
                        _frmView.ShowDialog();

                        StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString());

                        int Count1 = StartUp.DsTrans.Tables[0].Rows.Count;
                        int Count2 = StartUp.DsTrans.Tables[1].Rows.Count;


                        for (int i = Count1 - 1; i >= 1; i--)
                            StartUp.DsTrans.Tables[0].Rows.RemoveAt(i);

                        for (int i = 0; i < Count2; i++)
                            StartUp.DsTrans.Tables[1].Rows.RemoveAt(0);




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


                        // ko xoá dòng thứ 0 của table[0] vì dòng đó là dòng tạm.

                        if (newDs.Tables[0].Rows.Count > 0)
                        {
                            //Xét lại irow
                            if (FrmInctpxd.iRow > newDs.Tables[0].Rows.Count - 1)
                                FrmInctpxd.iRow = newDs.Tables[0].Rows.Count - 1;

                            StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[FrmInctpxd.iRow]["stt_rec"].ToString());
                        }

                        // Set lai irow va rowfilter ...
                        if (_frmView.DataGrid.ActiveRecord != null)
                        {
                            int select_irow = (_frmView.DataGrid.ActiveRecord as DataRecord).Index;
                            if (select_irow >= 0)
                            {
                                string selected_stt_rec = (_frmView.DataGrid.DataSource as DataView)[select_irow]["stt_rec"].ToString();
                                FrmInctpxd.iRow = select_irow + 1;
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

                ExMessageBox.Show( 1090,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNgay_ct1.Focus();
                result = false;
            }
            if (result && !txtNgay_ct1.IsValueValid)
            {
                ExMessageBox.Show( 1095,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                result = false;
                txtNgay_ct1.Focus();
                txtNgay_ct1.SelectAll();
            }
            if (result && (txtNgay_ct2.Value == null || txtNgay_ct2.Value.ToString() == ""))
            {
                ExMessageBox.Show( 1100,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNgay_ct2.Focus();
                result = false;
            }
            if (result && !txtNgay_ct2.IsValueValid)
            {
                ExMessageBox.Show( 1105,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                result = false;
                txtNgay_ct2.Focus();
                txtNgay_ct2.SelectAll();
            }
            if (result && Convert.ToDateTime(txtNgay_ct1.Value) > Convert.ToDateTime(txtNgay_ct2.Value))
            {
                ExMessageBox.Show( 1110,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNgay_ct1.Focus();
                result = false;
            }
            return result;
        }
        #endregion

        #region txtloc_nsd_LostFocus
        private void txtloc_nsd_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtloc_nsd.Text.Trim() == "")
                txtloc_nsd.Value = 1;
        }
        #endregion

        #region txtMa_kh_LostFocus
        private void txtMa_kh_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtMa_kh.RowResult != null)
                tblTen_kh.Text = StartUp.M_LAN.Equals("V") ? txtMa_kh.RowResult["ten_kh"].ToString():txtMa_kh.RowResult["ten_kh2"].ToString();
            else
                tblTen_kh.Text = "";
        }
        #endregion

        #region txtMa_nx_LostFocus
        private void txtMa_gd_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtMa_gd.RowResult != null)
                tblTen_gd.Text = StartUp.M_LAN.Equals("V") ? txtMa_gd.RowResult["ten_gd"].ToString() : txtMa_gd.RowResult["ten_gd2"].ToString();
            else
                tblTen_gd.Text = "";
        }

        #endregion

        #region txtMa_vt_LostFocus
        private void txtMa_vt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtMa_vt.RowResult != null)
                tblTen_vt.Text = StartUp.M_LAN.Equals("V") ? txtMa_vt.RowResult["ten_vt"].ToString() : txtMa_vt.RowResult["ten_vt2"].ToString();
            else
                tblTen_vt.Text = "";
        }
        #endregion

        #region txtMaDVCS_LostFocus

        private void txtMaDVCS_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtMaDVCS.RowResult != null)
                lblTenDVCS.Text = StartUp.M_LAN.Equals("V") ? txtMaDVCS.RowResult["ten_dvcs"].ToString() : txtMaDVCS.RowResult["ten_dvcs2"].ToString();
            else
                lblTenDVCS.Text = "";
        }

        #endregion
    }
}
