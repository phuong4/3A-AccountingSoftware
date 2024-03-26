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
using System.Data.SqlClient;
using SysLib;

namespace HTQ_CNHTHANH
{
    /// <summary>
    /// Interaction logic for FrmTaoPT.xaml
    /// </summary>
    public partial class FrmTaoHDA : Form
    {
        public bool isOk = false;
        public int kind = 1;
        
        public DataTable tbInfoPX = null;
        

        public FrmTaoHDA()
        {
            InitializeComponent();
            SmLib.SysFunc.LoadIcon(this);
            BindingSysObj = StartUp.SysObj;
            GrdOkCancel.pnlButton.btnCancel.Visibility = Visibility.Collapsed;
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            //txtNgay_ct.dValue = DateTime.Today;
            txtNgay_hd.dValue = StartUp.M_ngay_ct0;
            //txtso_hd.IsFocus = true;
        }

        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            //if (string.IsNullOrEmpty(txtTk.Text.Trim()))
            //{
            //    ExMessageBox.Show(592, StartUp.SysObj, "Chưa vào tài khoản có!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
            //    txtTk.IsFocus = true;
            //    return;
            //}

            isOk=true;

            this.Close();
        }

     
    }
}
