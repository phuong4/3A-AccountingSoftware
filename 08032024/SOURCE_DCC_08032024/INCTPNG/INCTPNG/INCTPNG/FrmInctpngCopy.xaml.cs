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

namespace Inctpng
{
    /// <summary>
    /// Interaction logic for FrmInctpngCopy.xaml
    /// </summary>
    public partial class FrmInctpngCopy : Form
    {
        public FrmInctpngCopy()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(FrmCopy_Loaded);
            SmLib.SysFunc.LoadIcon(this);
        }

        public static bool isCopy = false;
        public static DateTime ngay_ct;
        void FrmCopy_Loaded(object sender, RoutedEventArgs e)
        {
            txtNgay_ct_old.Value = StartUp.DsTrans.Tables[0].Rows[FrmInctpng.iRow]["ngay_ct"];
            txtNgay_ct_new.Value = DateTime.Now.Date;
            txtNgay_ct_new.Focus();
        }

        private void ConfirmGridView_OnOk1(object sender, RoutedEventArgs e)
        {
            if (txtNgay_ct_new.dValue == new DateTime() || string.IsNullOrEmpty(txtNgay_ct_new.Text.Trim().ToString()))
            {
                ExMessageBox.Show( 790,StartUp.SysObj, "Ngày chứng từ mới không hợp lệ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNgay_ct_new.Focus();
                return;
            }
            else
            {
                ngay_ct = DateTime.Parse(txtNgay_ct_new.Value.ToString());

                if (!SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, txtNgay_ct_old.dValue))
                {
                    ExMessageBox.Show( 795,StartUp.SysObj, "Ngày chứng từ cũ phải sau ngày khóa sổ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtNgay_ct_old.Focus();
                    return;
                }
                else if (!SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, ngay_ct.Date))
                {
                    ExMessageBox.Show( 800,StartUp.SysObj, "Ngày chứng từ mới phải sau ngày khóa sổ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtNgay_ct_new.Focus();
                    return;
                }
                else if (!txtNgay_ct_new.IsValueValid)
                {
                    ExMessageBox.Show( 805,StartUp.SysObj, "Ngày chứng từ mới không hợp lệ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtNgay_ct_new.Focus();
                    return;
                }
                else if (Convert.ToDateTime(txtNgay_ct_new.Value) < SmLib.NgayTC.GetStartDate(StartUp.M_ngay_ct0))
                {
                    ExMessageBox.Show( 810,StartUp.SysObj, "Ngày chứng từ mới phải sau ngày mở sổ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtNgay_ct_new.Focus();
                    return;
                }
                else
                {
                    isCopy = true;
                    this.Close();
                }
            }
        }

        #region ConfirmGridView_OnOk
        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            if (CheckValid())
            {
                ngay_ct = DateTime.Parse(txtNgay_ct_new.Value.ToString()).Date;
                isCopy = true;
                this.Close();
            }
        }
        #endregion

        #region CheckValid
        bool CheckValid()
        {
            bool result = true;
            if (result && (txtNgay_ct_new.Value == null || txtNgay_ct_new.Value.ToString() == ""))
            {
                ExMessageBox.Show(350, StartUp.SysObj, "Ngày chứng từ mới không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                result = false;
                txtNgay_ct_new.Focus();
            }
            if (result && txtNgay_ct_new.Value.ToString() != "")
            {
                if (!txtNgay_ct_new.IsValueValid)
                {
                    ExMessageBox.Show(355, StartUp.SysObj, "Ngày chứng từ mới không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    result = false;
                    txtNgay_ct_new.Focus();
                    txtNgay_ct_new.SelectAll();
                }
                if (result && !SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, Convert.ToDateTime(txtNgay_ct_new.dValue)))
                {
                    ExMessageBox.Show(360, StartUp.SysObj, "Ngày chứng từ mới phải sau ngày khóa sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    result = false;
                    txtNgay_ct_new.Focus();
                    txtNgay_ct_new.SelectAll();
                }
                if (result && Convert.ToDateTime(txtNgay_ct_new.dValue) < SmLib.NgayTC.GetStartDate(StartUp.M_ngay_ct0))
                {
                    ExMessageBox.Show(365, StartUp.SysObj, "Ngày chứng từ mới phải sau ngày mở sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    result = false;
                    txtNgay_ct_new.Focus();
                    txtNgay_ct_new.SelectAll();
                }
            }
            return result;
        }
        #endregion
        private void ConfirmGridView_OnCancel(object sender, RoutedEventArgs e)
        {
            isCopy = false;
            this.Close();
        }

        private void txtNgay_ct_new_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void Form_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                isCopy = false;
                this.Close();
            }
        }
    }
}
