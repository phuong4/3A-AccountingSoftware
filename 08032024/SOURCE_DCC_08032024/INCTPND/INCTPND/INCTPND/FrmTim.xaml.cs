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
using System.Diagnostics;
using SmDefine;



namespace INCTPND
{
    /// <summary>
    /// Interaction logic for FrmTim.xaml
    /// </summary>
    public partial class FrmTim : FormFilter
    {
        public FrmTim()
        {
            InitializeComponent();
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
            this.BindingSysObj = _SysObj;
            GridSearch.filterID = _filterID;
            GridSearch.tableList = _tableList;
            GridSearch.SysObj = _SysObj;
        }


        private void FrmTim_Loaded(object sender, RoutedEventArgs e)
        {
            txtNgay_ct1.Value = (DateTime)SysObj.GetSysVar("M_ngay_ct1");
            txtNgay_ct2.Value = (DateTime)SysObj.GetSysVar("M_ngay_ct2");
            txtUser.Text = StartUp.M_loc_nsd.ToString().Trim();
            txtMaDVCS.SearchInit();
            txtMa_kh.SearchInit();
            if (StartUp.SysObj.GetOption("M_LAN").ToString().ToUpper().Equals("V"))
            {
                if (txtMaDVCS.RowResult != null)
                    lblTenDVCS.Text = txtMaDVCS.RowResult["ten_dvcs"].ToString();
                if (txtMa_kh.RowResult != null)
                    tblten_kh.Text = txtMa_kh.RowResult["ten_kh"].ToString();
;
            }
            else
            {
                if (txtMaDVCS.RowResult != null)
                    lblTenDVCS.Text = txtMaDVCS.RowResult["ten_dvcs2"].ToString();
                if (txtMa_kh.RowResult != null)
                    tblten_kh.Text = txtMa_kh.RowResult["ten_kh2"].ToString();

            }
            txtSo_ct1.Focus();
        }

        private void FrmTim_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
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
                    SysObj.SetSysvar("M_ngay_ct1", txtNgay_ct1.dValue);
                    SysObj.SetSysvar("M_ngay_ct2", txtNgay_ct2.dValue);
                    bool IsshowView = false;

                    GridSearch._GenerateSQLString();
                    GridSearch.GrdSearch.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);

                    StartUp.TransFilterCmd.Parameters["@PhFilter"].Value = GetPhFilterExpr(); // "ngay_ct between '20100101' and '20100131'";
                    StartUp.TransFilterCmd.Parameters["@CtFilter"].Value = GetCtFilterExpr();
                    StartUp.TransFilterCmd.Parameters["@GtFilter"].Value = "";
                    StartUp.TransFilterCmd.Parameters["@sl_ct"].Value = 0;

                    DataSet newDs = DataProvider.FillCommand(StartUp.SysObj, StartUp.TransFilterCmd);
                    // xuất thông báo tìm kiếm
                    int n = 0;
                    Decimal a = (from p
                                    in newDs.Tables[0].AsEnumerable()
                                 select p.Field<Decimal?>("t_tien")).Sum().Value;

                    string tongPsVND = a.ToString(StartUp.SysObj.GetOption("M_IP_TIEN").ToString());
                    Decimal tongPsNT = (from p
                                    in newDs.Tables[0].AsEnumerable()
                                        select p.Field<Decimal?>("t_tien_nt")).Sum().Value;
                    string _tongPsNT = tongPsNT.ToString(StartUp.SysObj.GetOption("M_IP_TIEN_NT").ToString());
                    n = newDs.Tables[0].Rows.Count;

                    if (n > 0)
                    {
                        IsshowView = true;
                        ExMessageBox.Show( 580,StartUp.SysObj, "Có " + "[" + n + "]" + " chứng từ. Tổng phát sinh  " + "[" + _tongPsNT + "]" + " / " + "[" + tongPsVND + "]", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        ExMessageBox.Show( 585,StartUp.SysObj, "Không có chứng từ nào như vậy! ", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    if (IsshowView == true)
                    {
                        // show form View
                        string stringBrowse1, stringBrowse2;
                        if (StartUp.M_LAN.Equals("V"))
                        {
                            stringBrowse1 = StartUp.CommandInfo["Vbrowse1"].ToString().Split('|')[0];//"ngay_ct:fl:100:h=Ngày c.từ;so_ct:fl:70:h=Số c.từ;ma_kh:100:h=Mã khách;ten_kh:180:h=Tên khách;t_tien_nt:130:n1:h=Tổng tiền nt;t_tien:130:n0:h=Tổng tiền;dien_giai:225:h=Diễn giải;ma_nt:80:h=Loại tiền;ty_gia:130:h=Tỷ giá:r;[date]:140:h=Ngày cập nhật;[time]:140:h=Giờ cập nhật;[user_id]:80:n:h=Số hiệu NSD;[user_name]:180:h=Tên NSD";
                            stringBrowse2 = StartUp.CommandInfo["Vbrowse1"].ToString().Split('|')[1]; //"ma_vt:fl:100:h=Mã vật tư;ten_vt:fl:270:h=Tên vật tư;dvt:60:h=Đvt;ma_kho_i:70:h=Mã kho;so_luong:q:130:h=Số lượng;gia_nt:p1:130:h=Giá nt;tien_nt:130:n1:h=Tiền nt;tk_vt:80:h=Tk nợ;ma_nx_i:80:h=Tk có;gia:130:p0:h=Giá;tien:130:n0:h=Tiền"; 
                        }
                        else
                        {
                            stringBrowse1 = StartUp.CommandInfo["Ebrowse1"].ToString().Split('|')[0];//"ngay_ct:fl:100:h=Ngày c.từ;so_ct:fl:70:h=Số c.từ;ma_kh:100:h=Mã khách;ten_kh:180:h=Tên khách;t_tien_nt:130:n1:h=Tổng tiền nt;t_tien:130:n0:h=Tổng tiền;dien_giai:225:h=Diễn giải;ma_nt:80:h=Loại tiền;ty_gia:130:h=Tỷ giá:r;[date]:140:h=Ngày cập nhật;[time]:140:h=Giờ cập nhật;[user_id]:80:n:h=Số hiệu NSD;[user_name]:180:h=Tên NSD";
                            stringBrowse2 = StartUp.CommandInfo["Ebrowse1"].ToString().Split('|')[1]; //"ma_vt:fl:100:h=Mã vật tư;ten_vt:fl:270:h=Tên vật tư;dvt:60:h=Đvt;ma_kho_i:70:h=Mã kho;so_luong:q:130:h=Số lượng;gia_nt:p1:130:h=Giá nt;tien_nt:130:n1:h=Tiền nt;tk_vt:80:h=Tk nợ;ma_nx_i:80:h=Tk có;gia:130:p0:h=Giá;tien:130:n0:h=Tiền"; 
                        }

                        SmVoucherLib.FormView _frmView = new SmVoucherLib.FormView(SysObj, newDs.Tables[0].DefaultView, newDs.Tables[1].DefaultView, stringBrowse1, stringBrowse2, "stt_rec");
                        _frmView.ListFieldSum = "t_tien_nt;t_tien";
                        _frmView.frmBrw.Title = (StartUp.M_LAN.Equals("V")? "Phieu nhap kho. Ky ":"Receipt. Period ") + txtNgay_ct1.Text + " - " + txtNgay_ct2.Text;

                        SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, _frmView.frmBrw.oBrowseCt, StartUp.Ma_ct, 1);

                        _frmView.frmBrw.LanguageID  = "INCTPND_6";
                        _frmView.ShowDialog();

                        StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString() + "'";
                        StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString() + "'";
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
                            if (FrmINCTPND.iRow > newDs.Tables[0].Rows.Count - 1)
                                FrmINCTPND.iRow = newDs.Tables[0].Rows.Count - 1;

                            StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[FrmINCTPND.iRow]["stt_rec"].ToString() + "'";
                            StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[FrmINCTPND.iRow]["stt_rec"].ToString() + "'";
                        }

                        // Set lai irow va rowfilter ...
                        if (_frmView.DataGrid.ActiveRecord != null)
                        {
                            int select_item_index = (_frmView.DataGrid.ActiveRecord as DataRecord).DataItemIndex;
                            //int select_irow = (_frmView.DataGrid.ActiveRecord as DataRecord).Index;
                            if (select_item_index >= 0)
                            {
                                string selected_stt_rec = (_frmView.DataGrid.DataSource as DataView)[select_item_index]["stt_rec"].ToString();
                                FrmINCTPND.iRow = select_item_index + 1;
                                StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + selected_stt_rec + "'";
                                StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + selected_stt_rec + "'";
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

        private void ConfirmGridView_OnCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtMa_kh_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtMa_kh.RowResult == null)
            {
                tblten_kh.Text = "";
                return;
            }
            tblten_kh.Text = StartUp.M_LAN.Equals("V") ? txtMa_kh.RowResult["ten_kh"].ToString() : txtMa_kh.RowResult["ten_kh2"].ToString();
        }

        private void txtUser_TextChanged(object sender, RoutedPropertyChangedEventArgs<string> e)
        {
            if (string.IsNullOrEmpty(txtUser.Text.ToString()))
            {
                txtUser.Text = StartUp.M_loc_nsd.ToString().Trim();
            }
        }

        private string GetPhFilterExpr()
        {
            int maxlenghtSo_ct = BindingSysObj.GetDatabaseFieldLength("so_ct");
            string sPhFilter = "";
            if (!string.IsNullOrEmpty(txtNgay_ct1.Text))
            {
                sPhFilter += "  ngay_ct >= " + ConvertDataToSql(txtNgay_ct1.Value, typeof(DateTime));
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
            if (!string.IsNullOrEmpty(txtma_gd.Text))
            {
                sPhFilter += " and ma_gd = " + ConvertDataToSql(txtma_gd.Text.Trim(), typeof(string));
            }
            if (!string.IsNullOrEmpty(txtMaDVCS.Text))
            {
                sPhFilter += " and ma_dvcs LIKE '" + txtMaDVCS.Text.Trim() + "%'";
            }
            if (Convert.ToInt16(txtUser.Value) == 1)
            {
                sPhFilter += " and [user_id] = " + StartUp.M_User_Id;
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

        private bool IsHopLe()
        {
            if (String.IsNullOrEmpty(txtNgay_ct1.Value.ToString()))
            {
                ExMessageBox.Show(590, StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNgay_ct1.Focus();
                txtNgay_ct1.SelectAll();
                return false;
            }
            if (String.IsNullOrEmpty(txtNgay_ct2.Value.ToString()) || txtNgay_ct1.dValue > txtNgay_ct2.dValue)
            {
                ExMessageBox.Show(595, StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNgay_ct2.Focus();
                txtNgay_ct2.SelectAll();
                return false;
            }
            return true;
        }

        private void txtMaDVCS_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtMaDVCS.RowResult != null)
                lblTenDVCS.Text = StartUp.M_LAN.Equals("V") ? txtMaDVCS.RowResult["ten_dvcs"].ToString() : txtMaDVCS.RowResult["ten_dvcs2"].ToString();
            else
                lblTenDVCS.Text = "";
        }

        private void txtMa_gd_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtma_gd.RowResult != null)
                tblTen_gd.Text = StartUp.M_LAN.Equals("V") ? txtma_gd.RowResult["ten_gd"].ToString() : txtma_gd.RowResult["ten_gd2"].ToString();
            else
                tblTen_gd.Text = "";
        }
    }
}
