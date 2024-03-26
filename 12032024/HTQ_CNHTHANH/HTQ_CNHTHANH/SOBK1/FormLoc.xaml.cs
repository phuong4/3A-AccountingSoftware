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

namespace NN_BCLIN
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
         // public DetailFilterWin _detailFilterWin;
       

        private void TransactionFrm_Loaded(object sender, RoutedEventArgs e)
        {
            object rd = FindResource("TabGroupBoxStyle");
            //GridSearch.GroupBoxStyle = rd as Style;
          
            DateTime t1 = DateTime.Now;

            txtMaKhach.SysObj = txtMaVT.SysObj = BindingSysObj;
            TxtStartDateTime.Focus();
            GridSearch.SysObj = BindingSysObj;
            GridSearch.tableList = "v_ct70";
            SmLib.SysFunc.LoadIcon(this);
            DateTime t2 = DateTime.Now;
            txtMaKhach.SearchInit();
            //txtMaKho.SearchInit();
            txtMaVT.SearchInit();
            if(txtMaKhach.RowResult != null)
                lblTenKhach.Text = StartUp.M_LAN.Equals("V") ? txtMaKhach.RowResult["ten_kh"].ToString() : txtMaKhach.RowResult["ten_kh2"].ToString();
            //if (txtMaKho.RowResult != null)
            //    lblTenKho.Text = StartUp.M_LAN.Equals("V") ? txtMaKho.RowResult["ten_kho"].ToString() : txtMaKho.RowResult["ten_kho2"].ToString();
            if (txtMaVT.RowResult != null)
                lblTenVT.Text = StartUp.M_LAN.Equals("V") ? txtMaVT.RowResult["ten_vt"].ToString() : txtMaVT.RowResult["ten_vt2"].ToString();
            
        }

        private void btnHuy_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnNhan_Click(object sender, RoutedEventArgs e)
        {
            if (CheckValid())
            {
                string filter = GetFilter();
                TxtStartDateTime.ValueToDisplayTextConverter = TxtStartDateTime.ValueToDisplayTextConverter;
                if (TxtStartDateTime.dValue != new DateTime())
                {
                    //SysObj.SetSysvar("M_ngay_ct1", TxtStartDateTime.dValue);
                    StartUp.dtInfo.Rows[0]["StartDate"] = Convert.ToDateTime(TxtStartDateTime.Value).Date;
                }
                if (TxtEndDateTime.dValue != new DateTime())
                {
                    //SysObj.SetSysvar("M_ngay_ct2", TxtEndDateTime.dValue);
                    StartUp.dtInfo.Rows[0]["EndDate"] = Convert.ToDateTime(TxtEndDateTime.Value).Date;
                }
                //if (!string.IsNullOrEmpty(txtLoaiPN.Text.Trim().ToString()))
                //{
                //    filter += " and ma_gd = '" + txtLoaiPN.Text.ToString() + "'";
                //}
                //MessageBox.Show(filter);
                this.Hide();
                if (!string.IsNullOrEmpty(txtMaVT.Text))
                {
                    StartUp.QueryData(true, (object)TxtStartDateTime.Value, (object)TxtEndDateTime.Value, filter,  txtMaVT.Text.Trim(), int.Parse(cbMauBaoCao2.Value.ToString()));
                    //StartUp.CallGridVouchers((object)TxtStartDateTime.Value, (object)TxtEndDateTime.Value, filter, Convert.ToInt32(cbKieuLoc.Value.ToString()), txtMaVT.Text.Trim(), int.Parse(cbMauBaoCao2.Value.ToString()), loai);
                }
                else
                    StartUp.QueryData(true, (object)TxtStartDateTime.Value, (object)TxtEndDateTime.Value, filter,  txtMaVT.Text.Trim(), int.Parse(cbMauBaoCao2.Value.ToString()));
            }
        }
        //protected override bool IsEnterToPassObject(object sender)
        //{
        //    if (sender is XamComboEditor)
        //        return true;

        //    return base.IsEnterToPassObject(sender);
        //}

        #region CheckValid
        bool CheckValid()
        {
            bool result = true;

            if (result && (TxtStartDateTime.Value == null || TxtStartDateTime.Value.ToString() == ""))
            {
                ExMessageBox.Show( 50,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                result = false;
                TxtStartDateTime.Focus();
            }
            if (result && !TxtStartDateTime.IsValueValid)
            {
                ExMessageBox.Show( 55,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                result = false;
                TxtStartDateTime.Focus();
                TxtStartDateTime.SelectAll();
            }/*
            if (result && (DateTime)TxtStartDateTime.Value < SmLib.NgayTC.GetStartDate(StartUp.M_ngay_ct0))
            {
                ExMessageBox.Show( 60,StartUp.SysObj, "Từ ngày phải lớn hơn hoặc bằng ngày của kỳ mở sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                result = false;
                TxtStartDateTime.Focus();
            }*/
            if (result && (TxtEndDateTime.Value == null || TxtEndDateTime.Value.ToString() == ""))
            {
                ExMessageBox.Show( 65,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                result = false;
                TxtEndDateTime.Focus();
            }
            if (result && !TxtEndDateTime.IsValueValid)
            {
                ExMessageBox.Show( 70,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                result = false;
                TxtEndDateTime.Focus();
                TxtEndDateTime.SelectAll();
            }
            if (result && (DateTime)TxtStartDateTime.Value > (DateTime)TxtEndDateTime.Value)
            {
                ExMessageBox.Show( 75,StartUp.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                result = false;
                TxtStartDateTime.Focus();
                TxtStartDateTime.SelectAll();
            }
            if (result && !txtMaKhach.CheckLostFocus())
            {
                ExMessageBox.Show( 80,StartUp.SysObj, "Mã khách không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                result = false;
                txtMaKhach.IsFocus = true;
            }
            if (result && !txtMaVT.CheckLostFocus())
            {
                ExMessageBox.Show( 85,StartUp.SysObj, "Mã vật tư không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                result = false;
                txtMaVT.IsFocus = true;
            }
            return result;
        }
        #endregion

        #region GetFilter()
        public string GetFilter()
        {
            int maxlenghtSo_ct = BindingSysObj.GetDatabaseFieldLength("so_ct");
            string filter = " and 1=1 ";
            //string filter = " ";
            if (!string.IsNullOrEmpty(TxtStartDateTime.Text))
            {
                filter += " and ngay_ct >= " + ConvertDataToSql(TxtStartDateTime.Value, typeof(DateTime));
            }
            if (!string.IsNullOrEmpty(TxtEndDateTime.Text))
            {
                filter += " and ngay_ct <= " + ConvertDataToSql(TxtEndDateTime.Value, typeof(DateTime));
            }
            //if (!string.IsNullOrEmpty(txtLoaiPhieuNhap.Text))
            //{
            //    filter += " and ma_gd = '" + txtLoaiPhieuNhap.Text+"'" ;
            //}

           // Delegate a = Delegate.CreateDelegate(
            //if (!string.IsNullOrEmpty(txtSoCtBatDau.Text))
            //{
            //    filter += " and so_ct >= '" + txtSoCtBatDau.Text.Trim().PadLeft(maxlenghtSo_ct, ' ') + "'";
            //}
            //if (!string.IsNullOrEmpty(txtSoCtKetThuc.Text))
            //{
            //    filter += " and so_ct <= '" + txtSoCtKetThuc.Text.Trim().PadLeft(maxlenghtSo_ct, ' ') + "'";
            //}
            if (!string.IsNullOrEmpty(txtMaKhach.Text))
            {
                filter += " and ma_kh Like '" + txtMaKhach.Text + "%'";
            }
            //if (!string.IsNullOrEmpty(txtMaKho.Text))
            //{
            //    filter += " and ma_kho Like '" + txtMaKho.Text + "%'";
            //}
            if (!string.IsNullOrEmpty(txtMaVT.Text))
            {
                filter += " and ma_vt Like '" + txtMaVT.Text + "%'";
            }
            if (!string.IsNullOrEmpty(txtMaDVCS.Text))
            {
                filter += " and ma_dvcs Like '" + txtMaDVCS.Text + "%'";
            }
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
                lblTenKhach.Text = StartUp.M_LAN.Equals("V") ? txtMaKhach.RowResult["ten_kh"].ToString() : txtMaKhach.RowResult["ten_kh2"].ToString();
            else
                lblTenKhach.Text = "";
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
                    lblTenVT.Text = StartUp.M_LAN.Equals("V") ? txtMaVT.RowResult["ten_vt"].ToString() : txtMaVT.RowResult["ten_vt2"].ToString();
                else
                    lblTenVT.Text = "";
            }
            else
            {
                lblTenVT.Text = String.Empty;
            }
        }

        private void TransactionFrm_Closed(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }

        private void txtMaKho_LostFocus(object sender, RoutedEventArgs e)
        {
            //if (!String.IsNullOrEmpty(txtMaKho.Text))
            //{
            //    if (txtMaKho.RowResult != null)
            //        lblTenKho.Text = StartUp.M_LAN.Equals("V") ? txtMaKho.RowResult["ten_kho"].ToString() : txtMaKho.RowResult["ten_kho2"].ToString();
            //    else
            //        lblTenKho.Text = "";
            //}
            //else
            //{
            //    lblTenKho.Text = String.Empty;
            //}
        }

    

    }
    
}
