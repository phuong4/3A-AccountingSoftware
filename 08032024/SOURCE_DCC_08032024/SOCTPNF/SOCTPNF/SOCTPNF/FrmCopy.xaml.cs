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

namespace SOCTPNF
{
    /// <summary>
    /// Interaction logic for FrmCopy.xaml
    /// </summary>
    public partial class FrmCopy : Form
    {
        public FrmCopy()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(FrmCopy_Loaded);
            SmLib.SysFunc.LoadIcon(this);
        }

        public static bool isCopy = false;
        public static DateTime ngay_ct;
        void FrmCopy_Loaded(object sender, RoutedEventArgs e)
        {
            txtNgay_ct_old.Value = StartUp.DsTrans.Tables[0].Rows[FrmSOCTPNF.iRow]["ngay_ct"];
            txtNgay_ct_new.Value = DateTime.Now.Date;
            txtNgay_ct_new.Focus();
        }

        private void Form_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                isCopy = false;
                this.Close();
            }
        }

        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            if (txtNgay_ct_new.dValue == new DateTime() || string.IsNullOrEmpty(txtNgay_ct_new.Text.Trim().ToString()))
            {
                ExMessageBox.Show( 1320,StartUp.SysObj, "Ngày chứng từ mới không hợp lệ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNgay_ct_new.Focus();
                return;
            }
            else
            {
                ngay_ct = DateTime.Parse(txtNgay_ct_new.Value.ToString());

                if (!SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, txtNgay_ct_old.dValue))
                {
                    ExMessageBox.Show( 1325,StartUp.SysObj, "Ngày chứng từ cũ phải sau ngày khóa sổ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtNgay_ct_old.Focus();
                    return;
                }
                else if (!SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, ngay_ct.Date))
                {
                    ExMessageBox.Show( 1330,StartUp.SysObj, "Ngày chứng từ mới phải sau ngày khóa sổ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtNgay_ct_new.Focus();
                    return;
                }
                else if (!txtNgay_ct_new.IsValueValid)
                {
                    ExMessageBox.Show( 1335,StartUp.SysObj, "Ngày chứng từ không hợp lệ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtNgay_ct_new.Focus();
                    return;
                }
                else if (Convert.ToDateTime(txtNgay_ct_new.Value) < SmLib.NgayTC.GetStartDate(StartUp.M_ngay_ct0))
                {
                    ExMessageBox.Show( 1340,StartUp.SysObj, "Ngày chứng từ mới phải sau ngày mở sổ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void ConfirmGridView_OnCancel(object sender, RoutedEventArgs e)
        {
            isCopy = false;
            this.Close();
        }
    }
}
