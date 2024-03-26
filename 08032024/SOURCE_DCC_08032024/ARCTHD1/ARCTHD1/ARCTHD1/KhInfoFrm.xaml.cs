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

namespace ARCTHD1
{
    /// <summary>
    /// Interaction logic for NewSDFrm.xaml
    /// </summary>
    public partial class KhInfoFrm : Form
    {


       

        public Boolean IsAllowSave=true;
        public KhInfoFrm()
        {
          InitializeComponent();
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            SmLib.SysFunc.LoadIcon(this);
            txtTenkh.Text = StartUp.DsTrans.Tables[0].DefaultView[0][StartUp.M_LAN.Equals("V")? "ten_kh" : "ten_kh2"].ToString();
            //lbTenKH.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ten_kh"].ToString();
            txtDiaChi.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["dia_chi"].ToString();
            txtMaSoThue.Text=  StartUp.DsTrans.Tables[0].DefaultView[0]["ma_so_thue"].ToString();
            txtTenkh.Focus();
        }

      
        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            IsAllowSave = true;
            if (!SmLib.SysFunc.CheckSumMaSoThue(txtMaSoThue.Text.Trim()) && !string.IsNullOrEmpty(txtMaSoThue.Text.Trim()))
            {
                switch (StartUp.M_MST_CHECK.Trim())
                { 
                    case "0":
                        break;
                    case "1":
                        break;
                    case "2":
                        ExMessageBox.Show( 510,StartUp.SysObj, "Mã số thuế không hợp lệ, không lưu được!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                        IsAllowSave = false;
                        break;
                } 
            }
            if (IsAllowSave)
            {
                //if (!String.IsNullOrEmpty(txtTenkh.Text))
                {
                    StartUp.DsTrans.Tables[0].DefaultView[0][StartUp.M_LAN.Equals("V") ? "ten_kh" : "ten_kh2"] = txtTenkh.Text;
                }
                if (!String.IsNullOrEmpty(txtDiaChi.Text))
                {
                    StartUp.DsTrans.Tables[0].DefaultView[0]["dia_chi"] = txtDiaChi.Text;
                }
                //if (!String.IsNullOrEmpty(txtMaSoThue.Text))
                //{
                StartUp.DsTrans.Tables[0].DefaultView[0]["ma_so_thue"] = txtMaSoThue.Text;
                //}
                this.Close();
            }
        }

        private void ConfirmGridView_OnCancel(object sender, RoutedEventArgs e)
        {
           //// App.Current.Shutdown();
            this.Close();
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Escape))
            {
                this.Close();
            }
        }

     
    }
}
