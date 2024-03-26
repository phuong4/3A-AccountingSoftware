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
using System.Security.Cryptography;

namespace ARCTHD1
{
    /// <summary>
    /// Interaction logic for FrmLogin.xaml
    /// </summary>
    public partial class FrmLogin : Form
    {
        public bool IsLogined { get; set; }
        public FrmLogin()
        {
            InitializeComponent();
            SmLib.SysFunc.LoadIcon(this);
            IsLogined = false;
            this.Title = SmLib.SysFunc.Cat_Dau(StartUp.M_LAN.Equals("V") ? "Đăng nhập" : "Login");
            txtPassword.Focus();
        }

        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            //Mã hóa mật khẩu
            string password = txtPassword.Password;
            password = password.ToLower();
            Byte[] clearBytes = new UnicodeEncoding().GetBytes(password);
            Byte[] hashedBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(clearBytes);
            string _hashPassword = BitConverter.ToString(hashedBytes);
            if (_hashPassword == StartUp.SysObj.UserInfo.Rows[0]["password"].ToString())
            {
                IsLogined = true;
                this.Close();
            }
            else
            {
                ExMessageBox.Show( 485,StartUp.SysObj, "Mật khẩu không chính xác, vui lòng nhập lại mật khẩu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                txtPassword.SelectAll();
                txtPassword.Focus();
            }
        }
    }
}
