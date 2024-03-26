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

namespace AA_POCTPNB
{
    /// <summary>
    /// Interaction logic for FrmPoctpnaCopy.xaml
    /// </summary>
    public partial class FrmPoctpnbCopy : Form
    {
        public FrmPoctpnbCopy()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(FrmCopy_Loaded);
            SmLib.SysFunc.LoadIcon(this);
        }

        public static bool isCopy = false;
        public static DateTime ngay_ct;
        void FrmCopy_Loaded(object sender, RoutedEventArgs e)
        {
            txtNgay_ct_old.Value = StartUp.DsTrans.Tables[0].Rows[FrmPoctpnb.iRow]["ngay_ct"];
            txtNgay_ct_new.Value = DateTime.Now.Date;
            txtNgay_ct_new.Focus();
        }

        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
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
                else
                {
                    ngay_ct = DateTime.Parse(txtNgay_ct_new.Value.ToString()).Date;
                    isCopy = true;
                    this.Close();
                }
            }
        }

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
