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
using SysLib;
using Sm.Languages;
using Infragistics.Windows.Editors;
using System.Diagnostics;
using SmReport;

namespace Arso1t2
{
    /// <summary>
    /// Interaction logic for Window1.xaml 
    /// </summary>
    public partial class Arso1t2Loc : FormFilter
    {
        bool bResult = false;

        public Arso1t2Loc() { InitializeComponent(); }

        public string AdvanceFilter
        {
            get
            {
                GridSearch._GenerateSQLString();
                return GridSearch.arrStrFilter[0];
            }
        }

        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            if (CheckValid())
            {
                bResult = true;
                Hide();
            }
        }

        #region CheckValid
        bool CheckValid()
        {
            bool result = true;

            if (string.IsNullOrEmpty(txtTk.Text.Trim().ToString()))
            {
                ExMessageBox.Show( 2100,StartUp.SysObj, "Chưa vào tài khoản!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                txtTk.IsFocus = true;
                result = false;
            }

            DateTime M_ngay_ct0 = Convert.ToDateTime(StartUp.SysObj.GetSysVar("M_NGAY_KY1"));
            if (result && (txtNgay_ct1.Value == null || (txtNgay_ct1.Value != null && txtNgay_ct1.Value.ToString() == "")))
            {
                ExMessageBox.Show( 2105,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                result = false;
                txtNgay_ct1.Focus();
            }
            if (result && !txtNgay_ct1.IsValueValid)
            {
                ExMessageBox.Show( 2110,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                result = false;
                txtNgay_ct1.Focus();
                txtNgay_ct1.SelectAll();
            }
            //if (result && Convert.ToDateTime(txtNgay_ct1.Value) < M_ngay_ct0)
            //{
            //    ExMessageBox.Show(2115, StartUp.SysObj, "Từ ngày phải lớn hơn hoặc bằng ngày của kỳ mở sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            //    result = false;
            //    txtNgay_ct1.Focus();
            //    txtNgay_ct1.SelectAll();
            //}
            if (result && (txtNgay_ct2.Value == null || (txtNgay_ct2.Value != null && txtNgay_ct2.Value.ToString() == "")))
            {
                ExMessageBox.Show( 2120,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                result = false;
                txtNgay_ct2.Focus();
            }
            if (result && !txtNgay_ct2.IsValueValid)
            {
                ExMessageBox.Show( 2125,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                result = false;
                txtNgay_ct2.SelectAll();
                txtNgay_ct2.Focus();
            }
            if (result && Convert.ToDateTime(txtNgay_ct1.Value) > Convert.ToDateTime(txtNgay_ct2.Value))
            {
                ExMessageBox.Show(2135, StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                result = false;
                txtNgay_ct1.Focus();
                txtNgay_ct1.SelectAll();
            }
            //if (result && Convert.ToDateTime(txtNgay_ct2.Value) < M_ngay_ct0)
            //{
            //    ExMessageBox.Show(2130, StartUp.SysObj, "Đến ngày phải lớn hơn hoặc bằng ngày của kỳ mở sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            //    result = false;
            //    txtNgay_ct2.Focus();
            //    txtNgay_ct2.SelectAll();
            //}


            return result;
        }
        #endregion

        public new bool ShowDialog()
        {
            base.ShowDialog();
            return bResult;
        }

        #region FrmArso1t2Loc_Loaded
        private void FrmArso1t2Loc_Loaded(object sender, RoutedEventArgs e)
        {
            GridSearch.SysObj = BindingSysObj;
            GridSearch.filterID = "Arso1t2";
            GridSearch.tableList = "v_Arso1t2";
            txtMaKhach.SearchInit();
            txtTk.SearchInit();
            if (StartUp.SysObj.GetOption("M_LAN").ToString().ToUpper().Equals("V"))
            {
                if (txtMaKhach.RowResult != null)
                    lblTenKhach.Text = txtMaKhach.RowResult["ten_kh"].ToString();
                if (txtTk.RowResult != null)
                    txtTen_tk.Text = txtTk.RowResult["ten_tk"].ToString();
            }
            else
            {
                if (txtMaKhach.RowResult != null)
                    lblTenKhach.Text = txtMaKhach.RowResult["ten_kh2"].ToString();
                if (txtTk.RowResult != null)
                    txtTen_tk.Text = txtTk.RowResult["ten_tk2"].ToString();
            }
        }
        #endregion

        protected override bool IsEnterToPassObject(object sender)
        {
            if (sender is XamComboEditor)
                return true;

            return base.IsEnterToPassObject(sender);
        }

        #region txtTk_LostFocus
        private void txtTk_LostFocus(object sender, RoutedEventArgs e)
        {
            AutoCompleteTextBox txt = sender as AutoCompleteTextBox;

            if (txt.RowResult == null)
            {
                txtTen_tk.Text = "";
                return;
            }

            try
            {
                txtTen_tk.Text = StartUp.M_LAN.Equals("V") ? txt.RowResult["ten_tk"].ToString() : txt.RowResult["ten_tk2"].ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        #endregion

        private void txtMaKhach_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtMaKhach.RowResult != null)
                lblTenKhach.Text = StartUp.M_LAN.Equals("V") ? txtMaKhach.RowResult["ten_kh"].ToString() : txtMaKhach.RowResult["ten_kh2"].ToString();
            else
                lblTenKhach.Text = "";
        }
    }
}
