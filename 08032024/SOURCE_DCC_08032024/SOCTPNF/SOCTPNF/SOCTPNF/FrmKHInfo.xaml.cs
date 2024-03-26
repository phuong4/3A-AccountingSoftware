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
    /// Interaction logic for FrmKHInfo.xaml
    /// </summary>
    public partial class FrmKHInfo : Form
    {
        public bool isError = true;

        public FrmKHInfo()
        {
            InitializeComponent();
            DisplayLanguage = StartUp.M_LAN;
            BindingSysObj = StartUp.SysObj;
            SmLib.SysFunc.LoadIcon(this);
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            txtTen_kh.Text = StartUp.DsTrans.Tables[0].Rows[FrmSOCTPNF.iRow][StartUp.M_LAN.Equals("V") ? "ten_kh" : "ten_kh2"].ToString();
            txtDia_chi.Text = StartUp.DsTrans.Tables[0].Rows[FrmSOCTPNF.iRow]["dia_chi"].ToString();
            txtMa_so_thue.Text = StartUp.DsTrans.Tables[0].Rows[FrmSOCTPNF.iRow]["ma_so_thue"].ToString();
            txtTen_kh.Focus();
        }

        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            if (CheckValid())
            {
                StartUp.DsTrans.Tables[0].Rows[FrmSOCTPNF.iRow][StartUp.M_LAN.Equals("V") ? "ten_kh" : "ten_kh2"] = txtTen_kh.Text;
                StartUp.DsTrans.Tables[0].Rows[FrmSOCTPNF.iRow]["dia_chi"] = txtDia_chi.Text;
                StartUp.DsTrans.Tables[0].Rows[FrmSOCTPNF.iRow]["ma_so_thue"] = txtMa_so_thue.Text;
                isError = false;
                this.Close();
            }
        }

        #region CheckValid
        bool CheckValid()
        {
            bool result = true;
            #region ma_so_thue
            if (!StartUp.M_MST_CHECK.Equals("0"))
            {
                if (!string.IsNullOrEmpty(txtMa_so_thue.Text.Trim().ToString()))
                {
                    if (!SmLib.SysFunc.CheckSumMaSoThue(txtMa_so_thue.Text.Trim()))
                    {
                        if (StartUp.M_MST_CHECK.Equals("1"))
                        {
                            ExMessageBox.Show( 1350,StartUp.SysObj, "Mã số thuế không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        }

                        else
                        {
                            ExMessageBox.Show( 1355,StartUp.SysObj, "Mã số thuế không hợp lệ, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            result = false;
                            txtMa_so_thue.Focus();
                        }
                    }
                }
            }
            #endregion
            return result;
        }
        #endregion

        private void ConfirmGridView_OnCancel(object sender, RoutedEventArgs e)
        {
            isError = true;
            this.Close();
        }
    }
}
