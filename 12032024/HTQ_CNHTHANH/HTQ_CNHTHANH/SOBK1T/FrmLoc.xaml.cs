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
using Sm.Windows.Controls;
using System.Data;
using Infragistics.Windows.Editors;
using SmReport;

namespace HTQ_CNHTHANH
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FrmLoc : FormFilter
    {
       
        public FrmLoc()
        {
            InitializeComponent();
            SmLib.SysFunc.LoadIcon(this);
            this.BindingSysObj = StartUp.SysObj;
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            TxtStartDateTime.Focus();
            GridSearch.SysObj = StartUp.SysObj;
            //GridSearch.filterID = "POBK1";
            GridSearch.tableList = "v_ct65";
            //_formDetailFilter = new SmReport.DetailFilterWin(StartUp.SysObj, "cttt30");
            txtMaKhach_LostFocus(null, null);
        }

        protected override bool IsEnterToPassObject(object sender)
        {
            if (sender is XamComboEditor)
                return true;

            return base.IsEnterToPassObject(sender);
        }

        public string GetFilter()
        {
            int maxlenghtSo_ct = BindingSysObj.GetDatabaseFieldLength("so_ct");
            string filter = " and 1=1";

            //if (!string.IsNullOrEmpty(txtAccount.Text))
            //{
            //    filter += " and ma_nx like '" + txtAccount.Text + "%'";
            //}

            if (!string.IsNullOrEmpty(txtCustomer.Text))
            {
                filter += " and Ma_kh_i  like '" + txtCustomer.Text + "%'";
            }
            //if (!string.IsNullOrEmpty(txtStartNumberVouchers.Text))
            //{
            //    filter += " and so_ct >= '" + txtStartNumberVouchers.Text.Trim().PadLeft(maxlenghtSo_ct, ' ') + "'";
            //}
            if (!string.IsNullOrEmpty(txtMaDVCS.Text))
            {
                filter += " and ma_dvcs Like '" + txtMaDVCS.Text + "%'";
            }

            if (!string.IsNullOrEmpty(txtMaVT.Text))
            {
                filter += " and ma_vt Like '" + txtMaVT.Text + "%'";
            }
            //if (!string.IsNullOrEmpty(txtEndNumberVouchers.Text))
            //{
            //    filter += " and so_ct <= '" + txtEndNumberVouchers.Text.Trim().PadLeft(maxlenghtSo_ct, ' ') + "'";
            //}
            //filter += " AND dbo.TRIM(ma_ct) NOT IN " + StartUp.parameter;
            GridSearch._GenerateSQLString();
            if (GridSearch.arrStrFilter != null)
            {
                if (!string.IsNullOrEmpty(GridSearch.arrStrFilter[0]))
                {
                    filter += " and " + GridSearch.arrStrFilter[0];
                }
            }
            //else if (!string.IsNullOrEmpty(txtIdDo.Text))
            //{
            //    filter += " and Ma_vv  like '" + txtIdDo.Text + "%'";
            //}          
          

            return filter;
        }


        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            try
            {

                if (!txtCustomer.CheckLostFocus())
                {
                    ExMessageBox.Show(80, StartUp.SysObj, "Mã khách không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCustomer.IsFocus = true;
                }
                if (!txtMaVT.CheckLostFocus())
                {
                    ExMessageBox.Show(85, StartUp.SysObj, "Mã vật tư không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                   
                    txtMaVT.IsFocus = true;
                }

                if (!TxtStartDateTime.IsValueValid)
                {
                    ExMessageBox.Show( 115,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    TxtStartDateTime.Focus();
                }
                else if (!TxtEndDateTime.IsValueValid)
                {
                    ExMessageBox.Show( 120,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    TxtEndDateTime.Focus();
                }
                else if (TxtStartDateTime.Value == null || TxtStartDateTime.Value == DBNull.Value)
                {
                    ExMessageBox.Show( 125,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    TxtStartDateTime.Focus();
                }
                else if (TxtEndDateTime.Value == null || TxtEndDateTime.Value == DBNull.Value)
                {
                    ExMessageBox.Show( 130,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    TxtEndDateTime.Focus();
                }/*
                else if ((DateTime)TxtStartDateTime.Value < SmLib.NgayTC.GetStartDate(StartUp.M_ngay_ct0))
                {
                    ExMessageBox.Show( 135,StartUp.SysObj, "Từ ngày phải lớn hơn hoặc bằng ngày của kỳ mở sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    TxtStartDateTime.Focus();
                }*/




                else if ((DateTime)TxtStartDateTime.Value > (DateTime)TxtEndDateTime.Value)
                {
                    ExMessageBox.Show( 140,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    TxtEndDateTime.Focus();
                }
                else
                {
                    StartUp.DataSourceReport.Tables.Clear();
                    DataTable tbInfo = new DataTable("tbInfo");
                    tbInfo.Columns.Add("StartDateTime");
                    tbInfo.Columns.Add("EndDateTime");
                    tbInfo.Rows.Add(TxtStartDateTime.Text, TxtEndDateTime.Text);

                    StartUp.DataSourceReport.Tables.Add(tbInfo);

                    string filter = GetFilter();
                    this.Hide();
                    bool loaibc = true;
                    if (cbMauBaoCao2.Value.ToString().Equals("1"))
                        loaibc = false;

                    StartUp.CallGridVouchers(true, TxtStartDateTime.Value, TxtEndDateTime.Value, filter, loaibc);
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        private void txtMaKhach_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtCustomer.RowResult == null)
            {
                txtCustomer.SearchInit();
            }

            if (txtCustomer.RowResult != null)
            {
                if (StartUp.strLan.Equals("V"))
                {
                    lblTenKhach.Text = txtCustomer.RowResult["ten_kh"].ToString().Trim();
                }
                else
                {
                    lblTenKhach.Text = txtCustomer.RowResult["ten_kh2"].ToString().Trim();
                }
            }
            else
                lblTenKhach.Text = "";
        }

        private void txtMaVT_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtMaVT.Text))
            {
                if (txtMaVT.RowResult != null)
                    lblTenVT.Text = StartUp.M_LAN.Equals("V") ? txtMaVT.RowResult["ten_vt"].ToString() : txtMaVT.RowResult["ten_vt2"].ToString();
                else
                    lblTenVT.Text = "";
            }
            else
            {
                lblTenVT.Text = String.Empty;
            }
        }
    }
}
