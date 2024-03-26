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
using System.Windows.Threading;

namespace CACTBC1
{
    /// <summary>
    /// Interaction logic for FrmAPCTPN2Copy.xaml
    /// </summary>
    public partial class FrmCACTBC1Copy : Form
    {
        public FrmCACTBC1Copy()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(FrmCopy_Loaded);
            KeyDown += new KeyEventHandler(FrmCACTBC1Copy_KeyDown);
            SmLib.SysFunc.LoadIcon(this);
        }

        void FrmCACTBC1Copy_KeyDown(object sender, KeyEventArgs e)
        {

        }
        public bool isCopy = false;
        public static DateTime ngay_ct;
        void FrmCopy_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = SmLib.SysFunc.Cat_Dau(this.Title);
            txtNgay_ct_old.Value = StartUp.DsTrans.Tables[0].Rows[FrmCACTBC1.iRow]["ngay_ct"];
            txtNgay_ct_new.Value = DateTime.Now.Date;
            txtNgay_ct_new.Focus();
        }

        private void btnNhan_Click(object sender, RoutedEventArgs e)
        {
            if (!txtNgay_ct_new.IsValueValid || txtNgay_ct_new.Value == null)
            {
                ExMessageBox.Show( 160,StartUp.SysObj, "Ngày chứng từ mới không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNgay_ct_new.Focus();
                return;
            }
            if (txtNgay_ct_new.dValue == new DateTime())
            {
                txtNgay_ct_new.Value = DateTime.Now.Date;
            }
            else if (!SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, txtNgay_ct_new.dValue))
            {
                ExMessageBox.Show( 165,StartUp.SysObj, "Ngày hạch toán phải sau ngày khóa sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNgay_ct_new.Focus();
            }
            else if (!SmLib.SysFunc.CheckValidNgayMs(StartUp.SysObj, txtNgay_ct_new.dValue))
            {
                ExMessageBox.Show( 170,StartUp.SysObj, "Ngày hạch toán phải sau ngày mở sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
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
