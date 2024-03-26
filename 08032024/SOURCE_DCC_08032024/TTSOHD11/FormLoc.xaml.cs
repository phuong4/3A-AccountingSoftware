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
using System.Diagnostics;
using Infragistics.Windows.Editors;
using System.Windows.Threading;

namespace SOHD11
{
    /// <summary>
    /// Interaction logic for FormLoc.xaml
    /// </summary>
    public partial class FormLoc : SmReport.FormFilter
    {
        public FormLoc()
        {
            InitializeComponent();
            this.BindingSysObj = StartUp.SysObj;
        }


        private void TransactionFrm_Loaded(object sender, RoutedEventArgs e)
        {
            //object rd = FindResource("TabGroupBoxStyle");
            //GridSearch.GroupBoxStyle = rd as Style;
           
            DateTime t1 = DateTime.Now;
            
            txtMaKhach.SysObj = txtMaVT.SysObj = BindingSysObj;
            txtMaKhach.SearchInit();
            txtMaKho.SearchInit();
            txtMaVT.SearchInit();
            if (StartUp.M_LAN.Trim().ToUpper().Equals("V"))
            {
                if(txtMaKhach.RowResult != null)
                    lblTenKhach.Text = txtMaKhach.RowResult["ten_kh"].ToString();
                if(txtMaKho.RowResult != null)
                    lblTenKho.Text = txtMaKho.RowResult["ten_kho"].ToString();
                if(txtMaVT.RowResult != null)
                    lblTenVT.Text = txtMaVT.RowResult["ten_vt"].ToString();
            }
            else
            {
                if (txtMaKhach.RowResult != null)
                    lblTenKhach.Text = txtMaKhach.RowResult["ten_kh2"].ToString();
                if (txtMaKho.RowResult != null)
                    lblTenKho.Text = txtMaKho.RowResult["ten_kho2"].ToString();
                if (txtMaVT.RowResult != null)
                    lblTenVT.Text = txtMaVT.RowResult["ten_vt2"].ToString();
            }

            TxtStartDateTime.Focus();
            GridSearch.SysObj = BindingSysObj;
            GridSearch.tableList = "v_ct70hd";
            SmLib.SysFunc.LoadIcon(this);
            DateTime t2 = DateTime.Now;

        }

        private void btnHuy_Click(object sender, RoutedEventArgs e)
        {

            this.Close();

        }

        private void btnNhan_Click(object sender, RoutedEventArgs e)
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

            if (!TxtStartDateTime.IsValueValid)
            {
                ExMessageBox.Show( 2460,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                TxtStartDateTime.Focus();
            }
            else if (!TxtEndDateTime.IsValueValid)
            {
                ExMessageBox.Show( 2465,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                TxtEndDateTime.Focus();
            }
            else if (TxtStartDateTime.Value == null || TxtStartDateTime.Value == DBNull.Value)
            {
                ExMessageBox.Show( 2470,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                TxtStartDateTime.Focus();
            }
            else if (TxtEndDateTime.Value == null || TxtEndDateTime.Value == DBNull.Value)
            {
                ExMessageBox.Show( 2475,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                TxtEndDateTime.Focus();
            }/*
            else if ((DateTime)TxtStartDateTime.Value < SmLib.NgayTC.GetStartDate(StartUp.M_ngay_ct0))
            {
                ExMessageBox.Show( 2480,StartUp.SysObj, "Từ ngày phải lớn hơn hoặc bằng ngày của kỳ mở sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                TxtStartDateTime.Focus();
            }*/
            else if ((DateTime)TxtStartDateTime.Value > (DateTime)TxtEndDateTime.Value)
            {
                ExMessageBox.Show( 2485,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                TxtEndDateTime.Focus();
            }
            else
            {
                string filter = GetFilter();

                TxtStartDateTime.ValueToDisplayTextConverter = TxtStartDateTime.ValueToDisplayTextConverter;
                StartUp.dtInfo.Rows.Add(TxtStartDateTime.Text, TxtEndDateTime.Text);


                this.Hide();
                StartUp.QueryData(true, (object)TxtStartDateTime.Value, (object)TxtEndDateTime.Value, filter, int.Parse(cbMauBaoCao2.Value.ToString()));
            }
        }

        #region GetFilter()
        public string GetFilter()
        {
            int maxlenghtSo_ct = BindingSysObj.GetDatabaseFieldLength("so_ct");
            string filter = " 1=1 ";

            if (!string.IsNullOrEmpty(txtSoCtBatDau.Text.Trim()))
            {
                filter += " and Ma_hd  LIKE '" + txtSoCtBatDau.Text.Trim() + "%'";
            }

            if (!string.IsNullOrEmpty(txtMaKhach.Text.Trim()))
            {
                filter += " and ma_kh LIKE '" + txtMaKhach.Text.Trim() + "%'";
            }

            if (!string.IsNullOrEmpty(txtMaVT.Text.Trim()))
            {
                filter += " and ma_vt LIKE '" + txtMaVT.Text.Trim() + "%'";
            }

            if (!string.IsNullOrEmpty(txtMaKho.Text.Trim()))
            {
                filter += " and ma_kho LIKE '" + txtMaKho.Text.Trim() + "%'";
            }

            if (!string.IsNullOrEmpty(txtMa_dvcs.Text.Trim()))
            {
                filter += " and ma_dvcs LIKE '" + txtMa_dvcs.Text.Trim() + "%'";
            }
            filter += " and nxt = 2 and ma_ct LIKE " + StartUp.parameter;
            
            
            GridSearch._GenerateSQLString();
            if (GridSearch.arrStrFilter != null)
            {
                if (!string.IsNullOrEmpty(GridSearch.arrStrFilter[0]))
                {
                    filter += " and " + GridSearch.arrStrFilter[0];
                }
            }
            return filter;
        }

        #endregion

        #region ConvertDataToSql
        public string ConvertDataToSql(object value, Type ValueType)
        {
            string sResult = "";
            switch (ValueType.ToString())
            {
                case "System.String":
                    sResult = string.Format("'{0}'", (value as string).Replace("'", "'"));
                    break;
                case "System.DateTime":
                    sResult = string.Format("'{0}'", ((DateTime)value).ToString("yyyyMMdd"));
                    break;
                default:
                    sResult = string.Format("'{0}'", value);
                    break;
            }

            return sResult;
        }
        #endregion

        #region Lost_focus Event

        private void txtMaKhach_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtMaKhach.RowResult != null)
            {
                if (StartUp.M_LAN.Equals("V"))
                {
                    lblTenKhach.Text = txtMaKhach.RowResult["ten_kh"].ToString();
                }
                else
                {
                    lblTenKhach.Text = txtMaKhach.RowResult["ten_kh2"].ToString();
                }
            }
            else
                txtMaKhach.Text = "";

        }

        #endregion

        private void TransactionFrm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Escape))
                this.Close();
        }

        private void txtMaVT_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtMaVT.Text))
            {
                if (txtMaVT.RowResult != null)
                {
                    if (StartUp.M_LAN.Equals("V"))
                    {
                        lblTenVT.Text = txtMaVT.RowResult["ten_vt"].ToString();
                    }
                    else
                    {
                        lblTenVT.Text = txtMaVT.RowResult["ten_vt2"].ToString();
                    }
                }
            }
            else
            {
                lblTenVT.Text = String.Empty;
            }
        }

        private void txtMaKho_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtMaKho.Text))
            {
                if (txtMaKho.RowResult != null)
                {
                    if (StartUp.M_LAN.Equals("V"))
                    {
                        lblTenKho.Text = txtMaKho.RowResult["ten_kho"].ToString();
                    }
                    else
                    {
                        lblTenKho.Text = txtMaKho.RowResult["ten_kho2"].ToString();
                    }
                }
            }
            else
            {
                lblTenKho.Text = String.Empty;
            }
        }

        private void txtSoCtBatDau_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtSoCtBatDau.Text))
            {
                if (txtSoCtBatDau.RowResult != null)
                {
                    if (StartUp.M_LAN.Equals("V"))
                    {
                        lblTenHd.Text = txtSoCtBatDau.RowResult["dien_giai"].ToString();
                    }
                    else
                    {
                        lblTenHd.Text = txtSoCtBatDau.RowResult["dien_giai2"].ToString();
                    }
                }
            }
            else
            {
                lblTenHd.Text = String.Empty;
            }
        }

        private void txtMaKhach_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtMaKhach.Text))
            {
                if (txtMaKhach.RowResult != null)
                {
                    if (StartUp.M_LAN.Equals("V"))
                    {
                        lblTenKhach.Text = txtMaKhach.RowResult["ten_kh"].ToString();
                    }
                    else
                    {
                        lblTenKhach.Text = txtMaKhach.RowResult["ten_kh2"].ToString();
                    }
                }
            }
            else
                lblTenKhach.Text = "";

        }



    }

}
