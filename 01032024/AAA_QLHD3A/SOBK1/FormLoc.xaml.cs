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
using GdtLib;

namespace AAA_QLHD3A
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
            Chkloaddata.IsChecked = false;
            TxtStartDateTime.Focus();
            GridSearch.SysObj = BindingSysObj;
            GridSearch.tableList = "v_ct70";
            SmLib.SysFunc.LoadIcon(this);
            DateTime t2 = DateTime.Now;
            
        }

        private void btnHuy_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnNhan_Click(object sender, RoutedEventArgs e)
        {
            if (CheckValid())
            {
                if(Chkloaddata.IsChecked == true )
                {
                    try
                    {
                   
                        var Loai = Convert.ToInt32(cbKieuLoc.Value) + 1;
                       var lst = GdtLib.SyncUtils.SearchTctInvoice((DateTime)TxtStartDateTime.Value, (DateTime)TxtEndDateTime.Value, Loai, 50);
                        SyncUtils.SaveInvoice(lst, Loai);
                    }
                    catch (Exception ex)
                    {
                        SmErrorLib.ErrorLog.CatchMessage(ex);
                    }
                }

               

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
                StartUp.M_MA_DVCS = txtMaDVCS.Text;
                StartUp.zkieu_loc = Convert.ToInt32(cbKieuLoc.Value.ToString());
                StartUp.zfilter = filter;
                    StartUp.QueryData(true, (object)TxtStartDateTime.Value, (object)TxtEndDateTime.Value, filter, Convert.ToInt32(cbKieuLoc.Value.ToString()));

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

        #endregion

        private void TransactionFrm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Escape))
                this.Close();
        }



        private void TransactionFrm_Closed(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }
    

    }
    
}
