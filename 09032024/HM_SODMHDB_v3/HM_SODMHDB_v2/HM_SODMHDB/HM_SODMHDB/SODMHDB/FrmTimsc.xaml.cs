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

namespace AAA_SODMHDB
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

        void FrmTim_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        void FrmTim_Loaded(object sender, RoutedEventArgs e)
        {
            txtNgay_ct1.Value = (DateTime)SysObj.GetSysVar("M_ngay_ct1");
            txtNgay_ct2.Value = (DateTime)SysObj.GetSysVar("M_ngay_ct2");
            txtUser.Text = StartUp.M_loc_nsd.ToString().Trim();

            txtMa_kh.SearchInit();
            txtMa_dvcs.SearchInit();
            txtMa_nx.SearchInit();
            txtMa_bp.SearchInit();
            if (StartUp.SysObj.GetOption("M_LAN").ToString().ToUpper().Equals("V"))
            {
                if (txtMa_kh.RowResult != null)
                    tblten_kh.Text = txtMa_kh.RowResult["ten_kh"].ToString();
                if (txtMa_dvcs.RowResult != null)
                    tblten_dvcs.Text = txtMa_dvcs.RowResult["ten_dvcs"].ToString();
                if (txtMa_nx.RowResult != null)
                    tblten_nx.Text = txtMa_nx.RowResult["ten_tk"].ToString();
                if(txtMa_bp.RowResult != null)
                    tblten_bp.Text = txtMa_bp.RowResult["ten_bp"].ToString();
            }
            else
            {
                if (txtMa_kh.RowResult != null)
                    tblten_kh.Text = txtMa_kh.RowResult["ten_kh2"].ToString();
                if (txtMa_dvcs.RowResult != null)
                    tblten_dvcs.Text = txtMa_dvcs.RowResult["ten_dvcs2"].ToString();
                if (txtMa_nx.RowResult != null)
                    tblten_nx.Text = txtMa_nx.RowResult["ten_tk2"].ToString();
                if (txtMa_bp.RowResult != null)
                    tblten_bp.Text = txtMa_bp.RowResult["ten_bp2"].ToString();
            }
            //if (!string.IsNullOrEmpty(txtMa_dvcs.Text.Trim()))
            //{
            //    txtMa_dvcs.SearchInit();
            //    if (txtMa_dvcs.RowResult != null)
            //    {
            //        if (StartUp.M_LAN.Trim().ToUpper().Equals("V"))
            //        {
            //            tblten_dvcs.Text = txtMa_dvcs.RowResult["ten_dvcs"].ToString();
            //        }
            //        else
            //        {
            //            tblten_dvcs.Text = txtMa_dvcs.RowResult["ten_dvcs2"].ToString();
            //        }
            //    }
            //}

            if (!string.IsNullOrEmpty(tblten_kh.Text.Trim()))
            {
                txtMa_kh.SearchInit();
                if (txtMa_kh.RowResult != null)
                {
                    if (StartUp.M_LAN.Trim().ToUpper().Equals("V"))
                    {
                        tblten_kh.Text = txtMa_kh.RowResult["ten_kh"].ToString();
                    }
                    else
                    {
                        tblten_kh.Text = txtMa_kh.RowResult["ten_kh2"].ToString();
                    }
                }
            }


            if (!string.IsNullOrEmpty(tblten_nx.Text.Trim()))
            {
                txtMa_nx.SearchInit();
                if (txtMa_nx.RowResult != null)
                {
                    if (StartUp.M_LAN.Trim().ToUpper().Equals("V"))
                    {
                        tblten_nx.Text = txtMa_nx.RowResult["ten_tk"].ToString();
                    }
                    else
                    {
                        tblten_nx.Text = txtMa_nx.RowResult["ten_tk2"].ToString();
                    }
                }
            }

            txtSo_ct1.Focus();
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
            if (!string.IsNullOrEmpty(cb_status.Value.ToString().Trim()))
            {
                sPhFilter += " and status = '" + cb_status.Value.ToString().Trim() + " '";
            }
            if (!string.IsNullOrEmpty(txtMa_nx.Text))
            {
                sPhFilter += " and ma_nx like " + ConvertDataToSql(txtMa_nx.Text.Trim() + "%", typeof(string));
            }
            if(string.IsNullOrEmpty(txtSoTien1.Text.Trim()))
            {
                txtSoTien1.Value = 0;
            }
            if (txtSoTien1.Value.ToString().Trim() != "0")
            {
                sPhFilter += " and t_tt >= " + txtSoTien1.Value;
            }
            if (string.IsNullOrEmpty(txtSoTien2.Text.Trim()))
            {
                txtSoTien2.Value = 0;
            }

            if (txtSoTien2.Value.ToString().Trim() != "0")
            {
                sPhFilter += " and t_tt <= " + txtSoTien2.Value;
            }

            if (string.IsNullOrEmpty(txtHanThanhToan1.Text.Trim()))
            {
                txtHanThanhToan1.Value = 0;
            }
            if (txtHanThanhToan1.Value.ToString().Trim() != "0")
            {
                sPhFilter += " and han_tt >= " + txtHanThanhToan1.Value;
            }
            if (string.IsNullOrEmpty(txtHanThanhToan2.Text.Trim()))
            {
                txtHanThanhToan2.Value = 0;
            }
            if (txtHanThanhToan2.Value.ToString().Trim() != "0")
            {
                sPhFilter += " and han_tt <= " +txtHanThanhToan2.Value;
            }
            if (!string.IsNullOrEmpty(txtMa_bp.Text))
            {
                sPhFilter += " and ma_bp = " + ConvertDataToSql(txtMa_bp.Text.Trim(), typeof(string));
            }
            if (!string.IsNullOrEmpty(txtMa_dvcs.Text))
            {
                sPhFilter += " and ma_dvcs =  " + ConvertDataToSql(txtMa_dvcs.Text.Trim(), typeof(string));
            }

            if (!SmLib.SysFunc.CheckPermission(SysObj, ActionTask.View, StartUp.Menu_Id))
                sPhFilter += " and " + " AND user_id0 = " + SysObj.UserInfo.Rows[0]["user_id"].ToString();

            if (Convert.ToInt16(txtUser.Value) == 1)
            {
                sPhFilter += " and [user_id] = " + StartUp.M_User_Id;
            }
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
                    SysObj.SetSysvar("M_ngay_ct1", txtNgay_ct1.dValue);
                    SysObj.SetSysvar("M_ngay_ct2", txtNgay_ct2.dValue);
                    bool IsshowView = false;

                    GridSearch._GenerateSQLString();
                    GridSearch.GrdSearch.ExecuteCommand(DataPresenterCommands.EndEditModeAndAcceptChanges);
                //    StartUp.TransFilterCmd.Parameters["@ma_ct"].Value = "HDB";
                    StartUp.TransFilterCmd.Parameters["@PhFilter"].Value = GetPhFilterExpr(); // "ngay_ct between '20100101' and '20100131'";
                    StartUp.TransFilterCmd.Parameters["@CtFilter"].Value = GetCtFilterExpr();
               //     StartUp.TransFilterCmd.Parameters["@GtFilter"].Value = " 1 = 1";
                    StartUp.TransFilterCmd.Parameters["@sl_ct"].Value = 0;

                    DataSet newDs = DataProvider.FillCommand(StartUp.SysObj, StartUp.TransFilterCmd);
                    // xuất thông báo tìm kiếm
                    int n = 0;
                    Decimal a = (from p
                                    in newDs.Tables[0].AsEnumerable()
                                 select p.Field<Decimal?>("t_tt")).Sum().Value;

                    string tongPsVND = a.ToString(StartUp.SysObj.GetOption("M_IP_TIEN").ToString());
                    Decimal tongPsNT = (from p
                                    in newDs.Tables[0].AsEnumerable()
                                        select p.Field<Decimal?>("t_tt_nt")).Sum().Value;
                    string _tongPsNT = tongPsNT.ToString(StartUp.SysObj.GetOption("M_IP_TIEN_NT").ToString());
                    n = newDs.Tables[0].Rows.Count;
                    //a = StartUp.DsTrans.Tables[0].AsEnumerable().Sum("t_tt").Value.ToString();
                    if (n > 0)
                    {
                        IsshowView = true;
                        ExMessageBox.Show( 2340,StartUp.SysObj, "Có " + "[" + n + "]" + " chứng từ. Tổng phát sinh  " + "[" + _tongPsNT + "]" + " / " + "[" + tongPsVND + "]", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        ExMessageBox.Show( 2345,StartUp.SysObj, "Không có chứng từ nào như vậy! ", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    if (IsshowView == true)
                    {
                        // show form View
                        string stringBrowse1, stringBrowse2;
                        if (StartUp.M_LAN.Equals("V"))
                        {
                            stringBrowse1 = StartUp.CommandInfo["Vbrowse1"].ToString().Split('|')[0];
                            stringBrowse2 = StartUp.CommandInfo["Vbrowse1"].ToString().Split('|')[1];
                        }
                        else
                        {
                            stringBrowse1 = StartUp.CommandInfo["Ebrowse1"].ToString().Split('|')[0];
                            stringBrowse2 = StartUp.CommandInfo["Ebrowse1"].ToString().Split('|')[1];
                        }
                        SmVoucherLib.FormView _frmView = new SmVoucherLib.FormView(SysObj, newDs.Tables[0].DefaultView, newDs.Tables[1].DefaultView, stringBrowse1, stringBrowse2, "stt_rec");
                        _frmView.ListFieldSum = "t_tt_nt;t_tt";
                        string title;
                        if (StartUp.M_LAN.Equals("V"))
                        {
                            title = "Hop dong don ban hang. Ky " + txtNgay_ct1.Text + " - " + txtNgay_ct2.Text;
                            if (!string.IsNullOrEmpty(tblten_dvcs.Text.Trim()))
                                title += " - Don vi: " + tblten_dvcs.Text.Trim();
                        }
                        else
                        {
                            title = "Sales contract. Period " + txtNgay_ct1.Text + " - " + txtNgay_ct2.Text;
                            if (!string.IsNullOrEmpty(tblten_dvcs.Text.Trim()))
                                title += " - Unit: " + tblten_dvcs.Text.Trim();
                        }
                        _frmView.frmBrw.Title = title;

                        SmVoucherLib.FreeCodeFieldLib.InitFreeCodeField(StartUp.SysObj, _frmView.frmBrw.oBrowseCt, StartUp.Ma_ct, 1);



                        _frmView.frmBrw.LanguageID  = "AAA_SODMHDB_5";
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
                            if (FrmPoctpna.iRow > newDs.Tables[0].Rows.Count - 1)
                                FrmPoctpna.iRow = newDs.Tables[0].Rows.Count - 1;

                            StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[FrmPoctpna.iRow]["stt_rec"].ToString() + "'";
                            StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[FrmPoctpna.iRow]["stt_rec"].ToString() + "'";
                        }

                        // Set lai irow va rowfilter ...
                        if (_frmView.DataGrid.ActiveRecord != null)
                        {
                            int select_item_index = (_frmView.DataGrid.ActiveRecord as DataRecord).DataItemIndex;
                            //int select_irow = (_frmView.DataGrid.ActiveRecord as DataRecord).Index;
                            if (select_item_index >= 0)
                            {
                                string selected_stt_rec = (_frmView.DataGrid.DataSource as DataView)[select_item_index]["stt_rec"].ToString();
                                FrmPoctpna.iRow = select_item_index + 1;
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
        private bool IsHopLe()
        {
            if (String.IsNullOrEmpty(txtNgay_ct1.Value.ToString()))
            {
                ExMessageBox.Show( 2350,StartUp.SysObj, "Thưa ngài! Hãy nhập từ ngày!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNgay_ct1.Focus();
                txtNgay_ct1.SelectAll();
                return false;
            }
            else if (String.IsNullOrEmpty(txtNgay_ct2.Value.ToString()))
            {
                ExMessageBox.Show( 2355,StartUp.SysObj, "Thưa ngài! Hãy nhập đến ngày!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNgay_ct2.Focus();
                txtNgay_ct2.SelectAll();
                return false;
            }
            else
            {
                return true;
            }
        }

        private void txtUser_TextChanged(object sender, RoutedPropertyChangedEventArgs<string> e)
        {
            if (string.IsNullOrEmpty(txtUser.Text.ToString()))
            {
                txtUser.Text = StartUp.M_loc_nsd.ToString().Trim();
            }
        }

        private void txtMa_kh_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtMa_kh.RowResult == null)
            {
                tblten_kh.Text = "";
                return;
            }
            if (StartUp.M_LAN.Equals("V"))
                tblten_kh.Text = txtMa_kh.RowResult["ten_kh"].ToString();
            else
                tblten_kh.Text = txtMa_kh.RowResult["ten_kh2"].ToString();
        }


        private void txtMa_nx_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtMa_nx.RowResult == null)
            {
                tblten_nx.Text = "";
                return;
            }
            if (StartUp.M_LAN.Equals("V"))
                tblten_nx.Text = txtMa_nx.RowResult["ten_tk"].ToString();
            else
                tblten_nx.Text = txtMa_nx.RowResult["ten_tk2"].ToString();
        }

        private void txtMa_dvcs_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtMa_dvcs.RowResult == null)
            {
                tblten_dvcs.Text = "";
                return;
            }
            if (StartUp.M_LAN.Equals("V"))
                tblten_dvcs.Text = txtMa_dvcs.RowResult["ten_dvcs"].ToString();
            else
                tblten_dvcs.Text = txtMa_dvcs.RowResult["ten_dvcs2"].ToString();
        }

        private void txtMa_bp_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtMa_bp.RowResult == null)
            {
                tblten_bp.Text = "";
                return;
            }
            if (StartUp.M_LAN.Equals("V"))
                tblten_bp.Text = txtMa_bp.RowResult["ten_bp"].ToString();
            else
                tblten_bp.Text = txtMa_bp.RowResult["ten_bp2"].ToString();
        }

    }
}
