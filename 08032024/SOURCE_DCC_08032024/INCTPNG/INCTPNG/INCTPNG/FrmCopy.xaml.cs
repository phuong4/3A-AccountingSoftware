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

namespace INCTPNG
{
    /// <summary>
    /// Interaction logic for FrmCopy.xaml
    /// </summary>
    public partial class FrmCopy : Sm.Windows.Controls.Form
    {
        public FrmCopy()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(FrmCopy_Loaded);
        }
        public static bool isCopy = false;
        public static DateTime ngay_ct;
        void FrmCopy_Loaded(object sender, RoutedEventArgs e)
        {
            txtNgay_ct_old.Value = StartUp.DsTrans.Tables[0].Rows[FrmInctpng.iRow]["ngay_ct"];
            txtNgay_ct_new.Value = DateTime.Now.Date;
        }

        private void btnNhan_Click(object sender, RoutedEventArgs e)
        {
            ngay_ct = DateTime.Parse(txtNgay_ct_new.Value.ToString());
            isCopy = true;
            this.Close();
        }

        private void btnHuy_Click(object sender, RoutedEventArgs e)
        {
            isCopy = false;
            this.Close();
        }
    }
}
