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

namespace CACTBN1
{
    /// <summary>
    /// Interaction logic for FrmCACTBN1Copy.xaml
    /// </summary>
    public partial class FrmCACTBN1Copy : Form
    {
        public FrmCACTBN1Copy()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(FrmCopy_Loaded);
            SmLib.SysFunc.LoadIcon(this);
        }
        public bool isCopy = false;
        public static DateTime ngay_ct;
        void FrmCopy_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = SmLib.SysFunc.Cat_Dau(this.Title);
            txtNgay_ct_old.Value = StartUp.DsTrans.Tables[0].Rows[FrmCACTBN1.iRow]["ngay_ct"];
            txtNgay_ct_new.Value = DateTime.Now.Date;
            txtNgay_ct_new.Focus();
        }

        private void btnNhan_Click(object sender, RoutedEventArgs e)
        {
            if (!txtNgay_ct_new.IsValueValid || txtNgay_ct_new.Value == DBNull.Value)
            {
                ExMessageBox.Show( 475,StartUp.SysObj, "Ngày chứng từ mới không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNgay_ct_new.Focus();
                txtNgay_ct_new.SelectAll();
            }
            else if (!SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, txtNgay_ct_new.dValue))
            {
                ExMessageBox.Show( 480,StartUp.SysObj, "Ngày hạch toán phải sau ngày khóa sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNgay_ct_new.Focus();
            }
            else if (!SmLib.SysFunc.CheckValidNgayMs(StartUp.SysObj, txtNgay_ct_new.dValue))
            {
                ExMessageBox.Show( 485,StartUp.SysObj, "Ngày hạch toán phải sau ngày mở sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNgay_ct_new.Focus();
            }
            else
            {
                ngay_ct = DateTime.Parse(txtNgay_ct_new.Value.ToString());
                isCopy = true;
                this.Close();
            }
        }

        private void btnHuy_Click(object sender, RoutedEventArgs e)
        {
            isCopy = false;
            this.Close();
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
