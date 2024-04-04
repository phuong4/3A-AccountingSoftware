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
using System.Windows.Navigation;
using System.Windows.Shapes;
using SmReport;
using System.Data;
using Sm.Windows.Controls;
using Infragistics.Windows.Editors;

namespace COSXLSX.COLSX
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FrmLoc : FormFilter
    {
        public bool isClose = false;

        public FrmLoc()
        {
            InitializeComponent();
            SmLib.SysFunc.LoadIcon(this);
        }

        private void FrmcodmnvlLoc_Loaded(object sender, RoutedEventArgs e)
        {
            this.BindingSysObj = StartUp.SysObj;
            TxtStartDateTime.Focus();

        }

        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {

            if (Keyboard.FocusedElement.GetType().Equals(typeof(TextBoxAutoComplete)))
            {
                TextBoxAutoComplete txt = Keyboard.FocusedElement as TextBoxAutoComplete;
                if (txt.ParentControl != null)
                {
                    if (!txt.ParentControl.CheckLostFocus())
                    {
                        return;
                    }
                }
            }

            if (!string.IsNullOrEmpty(TxtStartDateTime.Text))
            {
                //if (!TxtStartDateTime.IsValueValid)
                //{
                //    ExMessageBox.Show(1000, StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);                    
                //    TxtStartDateTime.Focus();
                //    return;
                //}
                /*if ((DateTime)TxtStartDateTime.Value < StartUp.M_NGAY_KY1)
                {
                    ExMessageBox.Show(1000, StartUp.SysObj, "Từ ngày phải lớn hơn hoặc bằng ngày của kỳ mở sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    TxtStartDateTime.Focus();
                    TxtStartDateTime.SelectAll();
                    return;

                }*/

            }
            //if (!string.IsNullOrEmpty(TxtEndDateTime.Text))
            //{
            //    if (!TxtEndDateTime.IsValueValid)
            //    {
            //        ExMessageBox.Show(1200, StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            //        TxtEndDateTime.Focus();
            //        return;
            //    }
            //}

            if (!string.IsNullOrEmpty(TxtEndDateTime.Text) && !string.IsNullOrEmpty(TxtEndDateTime.Text))
            {
                if ((DateTime)TxtStartDateTime.Value > (DateTime)TxtEndDateTime.Value)
                {
                    ExMessageBox.Show(1100, StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    TxtEndDateTime.Focus();
                    TxtEndDateTime.SelectAll();
                    return;
                }
            }
           
             
            StartUp.sMalsx = txtLsx.Text.Trim().ToString();
            StartUp.sMapx = txtPx.Text.Trim().ToString();

            StartUp.sStartDate =(object)TxtStartDateTime.Value;
            StartUp.sEndDate = (object)TxtEndDateTime.Value;
            if (StartUp.sStartDate == null)
            {
                StartUp.sStartDate = "";
            }
            if (StartUp.sEndDate == null)
            {
                StartUp.sEndDate = "";
            }
            this.Hide();

            StartUp.CallGridVouchers(true, StartUp.sStartDate, StartUp.sEndDate, StartUp.sMalsx, StartUp.sMapx);
        }

        private void txtPx_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            txtPx.SearchInit();
            lblTenPx.Text = txtPx.RowResult == null ? "" : (StartUp.SysObj.GetOption("M_LAN").ToString() == "V" ? txtPx.RowResult["ten_px"].ToString() : txtPx.RowResult["ten_px2"].ToString());
        }

        private void ConfirmGridView_OnCancel(object sender, RoutedEventArgs e)
        {
            isClose = true;
            this.Close();
        }

        private void txtLsx_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            txtLsx.SearchInit();
            lblTenLsx.Text = txtLsx.RowResult == null ? "" : (StartUp.SysObj.GetOption("M_LAN").ToString() == "V" ? txtLsx.RowResult["dien_giai"].ToString() : txtLsx.RowResult["dien_giai"].ToString());
        }
    }
}

