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
using SmReport;
using Sm.Windows.Controls;

namespace QLHD_BC03
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FrmFilter : FormFilter
    {
        public FrmFilter()
        {
            InitializeComponent();
            BindingSysObj = StartUp.SysObj;
            DisplayLanguage = StartUp.M_LAN;
        }

        void FrmFilter_Loaded(object sender, RoutedEventArgs e)
        {
            GridSearch.SysObj = StartUp.SysObj;
            GridSearch.tableList = StartUp.TableList;
            txtngay_ct1.Focus();
        }

        #region ConfirmGridView_OnOk
        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            if (CheckValid())
            {
                GridSearch._GenerateSQLString();
                GridSearch.GrdSearch.ExecuteCommand(Infragistics.Windows.DataPresenter.DataPresenterCommands.EndEditModeAndAcceptChanges);
                this.Hide();
                StartUp.CallReport(true, txtngay_ct1.Value, txtngay_ct2.Value, cbgiam_tru.Value, GetStringAdvance(), Convert.ToInt16(cbmau_bc.Value));
            }
        }
        #endregion
        
        #region GetStringAdvance
        public string GetStringAdvance()
        {
            string result = "1=1";
            if (!string.IsNullOrEmpty(cbloai_hd.Text.Trim()))
                result += " AND ma_gd = " + cbloai_hd.Text.Trim() + "";
            else
            {
                //Nếu không chọn loại hóa đơn phải lấy hết để lên được PNF
                //result += " AND (ma_gd = 1 OR ma_gd = 2)";
            }
            if (!string.IsNullOrEmpty(txtma_kh.Text))
                result += " AND ma_kh LIKE '" + txtma_kh.Text.Trim() + "%'";
            if (!string.IsNullOrEmpty(txtma_vt.Text))
                result += " AND ma_vt LIKE '" + txtma_vt.Text.Trim() + "%'";
            if (!string.IsNullOrEmpty(txtMaKho.Text))
            {
                result += " and ma_kho LIKE '" + txtMaKho.Text.Trim() + "%'";
            }
            if (!string.IsNullOrEmpty(txtMaDVCS.Text))
            {
                result += " and ma_dvcs Like '" + txtMaDVCS.Text + "%'";
            }

            if (!string.IsNullOrEmpty(GridSearch.arrStrFilter[0]))
                result += " AND " + GridSearch.arrStrFilter[0];
            return result;
        }
        #endregion

        #region CheckValid
        bool CheckValid()
        {
            bool result = true;
            if (result && (txtngay_ct1.Value == null || txtngay_ct1.Value.ToString() == ""))
            {
                ExMessageBox.Show( 2490,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                result = false;
                txtngay_ct1.Focus();
            }
            if (result && !txtngay_ct1.IsValueValid)
            {
                ExMessageBox.Show( 2495,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                result = false;
                txtngay_ct1.Focus();
                txtngay_ct1.SelectAll();
            }/*
            if (result && (DateTime)txtngay_ct1.Value < SmLib.NgayTC.GetStartDate(StartUp.M_ngay_ct0))
            {
                ExMessageBox.Show( 2500,StartUp.SysObj, "Từ ngày phải lớn hơn hoặc bằng ngày của kỳ mở sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                result = false;
                txtngay_ct1.Focus();
                txtngay_ct1.SelectAll();
            }*/
            if (result && (txtngay_ct2.Value == null || txtngay_ct2.Value.ToString() == ""))
            {
                ExMessageBox.Show( 2505,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                result = false;
                txtngay_ct2.Focus();
            }
            if (result && !txtngay_ct2.IsValueValid)
            {
                ExMessageBox.Show( 2510,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                result = false;
                txtngay_ct2.Focus();
                txtngay_ct2.SelectAll();
            }
            if (result && (DateTime)txtngay_ct1.Value > (DateTime)txtngay_ct2.Value)
            {
                ExMessageBox.Show( 2515,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                result = false;
                txtngay_ct1.Focus();
                txtngay_ct1.SelectAll();
            }
            //if (result && !txtma_kh.CheckLostFocus())
            //{
            //    ExMessageBox.Show( 2520,StartUp.SysObj, "Mã khách hàng không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            //    result = false;
            //    txtma_kh.IsFocus = true;
            //}
            //if (result && !txtma_vt.CheckLostFocus())
            //{
            //    ExMessageBox.Show( 2525,StartUp.SysObj, "Mã vật tư không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            //    result = false;
            //    txtma_vt.IsFocus = true;
            //}
            return result;
        }
        #endregion

        #region txtma_kh_PreviewLostFocus
        private void txtma_kh_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtma_kh.RowResult == null)
                tblten_kh.Text = "";
            else
                tblten_kh.Text = StartUp.M_LAN.Equals("V") ? txtma_kh.RowResult["ten_kh"].ToString() : txtma_kh.RowResult["ten_kh2"].ToString();
        }
        #endregion

        #region txtma_vt_PreviewLostFocus
        private void txtma_vt_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtma_vt.RowResult == null)
                tblten_vt.Text = "";
            else
                tblten_vt.Text = StartUp.M_LAN.Equals("V") ? txtma_vt.RowResult["ten_vt"].ToString() : txtma_vt.RowResult["ten_vt2"].ToString();
        }
        #endregion

        private void txtMaKho_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtMaKho.Text))
            {
                if (txtMaKho.RowResult != null)
                    lblTenKho.Text = StartUp.M_LAN.Equals("V") ? txtMaKho.RowResult["ten_kho"].ToString() : txtMaKho.RowResult["ten_kho2"].ToString();
            }
            else
            {
                lblTenKho.Text = String.Empty;
            }
        }

    }
}
