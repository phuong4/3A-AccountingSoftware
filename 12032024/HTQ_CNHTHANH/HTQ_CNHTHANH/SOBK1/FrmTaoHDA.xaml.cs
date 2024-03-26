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

namespace NN_BCLIN
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
            txtNgay_ct.dValue = DateTime.Today;
            txtMa_td2.IsFocus = true;
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

        private void txtMa_td2_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtMa_td2.RowResult != null)
            {
                txtTen_td2.Text = StartUp.M_LAN.Equals("V") ? txtMa_td2.RowResult["ten_td"].ToString() : txtMa_td2.RowResult["ten_td2"].ToString();
            }
        }

        private void txtMa_s1_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtMa_s1.RowResult != null)
            {
                txtTen_s1.Text = StartUp.M_LAN.Equals("V") ? txtMa_s1.RowResult["ten_td"].ToString() : txtMa_s1.RowResult["ten_td2"].ToString();
            }
        }

        private void txtMa_s2_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtMa_s2.RowResult != null)
            {
                txtTen_s2.Text = StartUp.M_LAN.Equals("V") ? txtMa_s2.RowResult["ten_td"].ToString() : txtMa_s2.RowResult["ten_td2"].ToString();
            }
        }

        private void txtMa_s3_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtMa_s3.RowResult != null)
            {
                txtTen_s3.Text = StartUp.M_LAN.Equals("V") ? txtMa_s3.RowResult["ten_td"].ToString() : txtMa_s3.RowResult["ten_td2"].ToString();
            }
        }


    }
}
