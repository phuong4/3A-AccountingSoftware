using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Data;
using Infragistics.Windows.DataPresenter;
using System.Windows;
using System.Windows.Input;

namespace CACTPC1
{
    class BindingStatusVoucher : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value.ToString().Trim()))
            {
                DataRow[] rows = StartUp.tbStatus.Select("Ma_post =" + value);
                if (rows.Count() > 0)
                {
                    DataRow row = rows[0] as DataRow;
                    return StartUp.tbStatus.Rows.IndexOf(row);
                }
            }
            DataRow[] drs = StartUp.tbStatus.Select("Default = 1");
            if (drs.Length > 0)
                return StartUp.tbStatus.Rows.IndexOf(drs[0] as DataRow);
            else
                return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? StartUp.DmctInfo["ma_post"] : value;
        }

        #endregion
    }
    class BindingReadonlyCNo : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (FrmCACTPC1.IsInEditMode != null)
                if (FrmCACTPC1.IsInEditMode.Value)
                    if (value != null)
                        return value.ToString().Trim().Equals("1") ? false : true;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    class BindingVisibleGrdCTChi : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string result = "Visible";
            if (value != null)
                if (value != DBNull.Value && value != DependencyProperty.UnsetValue)
                {
                    switch (parameter.ToString().Trim())
                    {
                        case "ma_gd":
                            {
                                int ma_gd = 2;
                                int.TryParse(value.ToString(), out ma_gd);
                                if (ma_gd == 2 || ma_gd == 3)
                                {
                                    result = "Visible";
                                }
                                else if (ma_gd == 9)
                                {
                                    result = "Collapsed";
                                }
                            }
                            break;
                    }
                }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    class BindingReadonlyMaGD : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (FrmCACTPC1.IsInEditMode != null)
                if (FrmCACTPC1.IsInEditMode.Value)
                    if (values != null)
                    {
                        if (values[0] != DBNull.Value && values[0] != DependencyProperty.UnsetValue)
                        {
                            int count = StartUp.DsTrans.Tables[1].DefaultView.Count;
                            if (count > 0)
                            {
                                if (StartUp.DsTrans.Tables[0].DefaultView[0]["ispostgt"] != DBNull.Value && !string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ispostgt"].ToString()))
                                {
                                    if (StartUp.DsTrans.Tables[0].DefaultView[0]["ispostgt"].ToString().Equals("True"))
                                        return true;
                                }
                                if (StartUp.DsTrans.Tables[1].DefaultView[0]["tk_i"] != DBNull.Value && !string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[0]["tk_i"].ToString()))
                                {
                                    return true;
                                }
                            }
                        }
                        return false;
                    }
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    class BindingReadonlyHD : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (FrmCACTPC1.IsInEditMode != null)
                if (FrmCACTPC1.IsInEditMode.Value)
                    if (values != null)
                    {
                        if (values[0] != DBNull.Value && values[0] != DependencyProperty.UnsetValue)
                        {
                            string hdStr = values[0].ToString().Trim();
                            string paraStr = parameter.ToString().Trim();
                            switch (hdStr)
                            {
                                //trường hợp loại hd là 0
                                case "0":
                                    {
                                        string[] FieldsNotAllowEdit = { "so_seri0", "so_ct0", "ma_kh_t", "ma_thue_i", "thue_suat", "thue_nt", "tk_thue_i", "thue", "tt_nt", "tt" };

                                        if (FieldsNotAllowEdit.Contains(paraStr))
                                        {
                                            return true;
                                        }
                                    }
                                    break;
                                case "1":
                                    {
                                        string[] FieldsNotAllowEdit = { "tt_nt", "tt" };
                                        if (FieldsNotAllowEdit.Contains(paraStr))
                                        {
                                            return true;
                                        }
                                    }
                                    break;
                                case "2":
                                    {
                                        string[] FieldsNotAllowEdit = { "tt_nt", "tt" };
                                        if (FieldsNotAllowEdit.Contains(paraStr))
                                        {
                                            return true;
                                        }
                                    }
                                    break;
                                case "4":
                                    {
                                        string[] FieldsNotAllowEdit = { "tt_nt", "tt" };
                                        if (FieldsNotAllowEdit.Contains(paraStr))
                                        {
                                            return true;
                                        }
                                    }
                                    break;
                                case "5":
                                    {
                                        string[] FieldsNotAllowEdit = { "ngay_ct0", "so_seri0", "so_ct0", "ma_kh_t", "ten_kh_t", "dia_chi_t", "mst_t", "ten_vt_t", "ma_thue_i", "thue_suat", "tt_nt", "tt", "ghi_chu_t" };
                                        if (FieldsNotAllowEdit.Contains(paraStr))
                                        {
                                            return true;
                                        }
                                    }
                                    break;
                            }
                            if (paraStr.Equals("tien"))
                            {
                                if (!values[3].Equals(DependencyProperty.UnsetValue))
                                {
                                    bool checkSuaTien = (bool)values[3];
                                    if (checkSuaTien)
                                        return false;
                                }
                                if (!values[1].Equals(DependencyProperty.UnsetValue) && !values[2].Equals(DependencyProperty.UnsetValue))
                                {
                                    double tien_nt = System.Convert.ToDouble(values[1]);
                                    double ty_gia = System.Convert.ToDouble(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"]);
                                    if (tien_nt * ty_gia == 0)
                                        return false;
                                }
                                return true;
                            }
                            else if (paraStr.Equals("thue"))
                            {
                                if (!values[3].Equals(DependencyProperty.UnsetValue))
                                {
                                    bool checkSuaTien = (bool)values[3];
                                    if (checkSuaTien)
                                        return false;
                                }
                                if (!values[1].Equals(DependencyProperty.UnsetValue) && !values[2].Equals(DependencyProperty.UnsetValue))
                                {
                                    double tien = System.Convert.ToDouble(values[1]);
                                    double thue_suat = System.Convert.ToDouble(values[2]);
                                    if (tien * thue_suat == 0)
                                        return false;
                                }
                                return true;
                            }
                            else if (paraStr.Equals("ma_kh2_t"))
                            {
                                if (values[1] != null)
                                    return values[1].ToString().Trim().Equals("1") ? false : true;
                            }
                            return false;
                        }
                    }
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    class BindingReadonlyHDChi : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (FrmCACTPC1.IsInEditMode != null)
                if (FrmCACTPC1.IsInEditMode.Value)
                    if (values != null)
                    {
                        string paraStr = parameter.ToString().Trim();
                        switch (paraStr)
                        {
                            case "tien_tt":
                                if (!values[2].Equals(DependencyProperty.UnsetValue))
                                {
                                    bool Voucher_Ma_nt0 = true;
                                    bool.TryParse(values[2].ToString(), out Voucher_Ma_nt0);
                                    if (!Voucher_Ma_nt0)
                                    {
                                        //Check sửa tiền
                                        if (!values[3].Equals(DependencyProperty.UnsetValue))
                                        {
                                            if ((bool)values[3])
                                                return false;
                                        }
                                        if (!values[0].Equals(DependencyProperty.UnsetValue))
                                        {
                                            decimal ty_gia = 0, tien_nt = 0;
                                            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);
                                            decimal.TryParse(values[0].ToString(), out tien_nt);
                                            if (ty_gia * tien_nt == 0)
                                                return false;
                                        }
                                    }
                                }
                                break;
                            case "tien":
                                if (!values[3].Equals(DependencyProperty.UnsetValue))
                                {
                                    bool Voucher_Ma_nt0 = true;
                                    bool.TryParse(values[3].ToString(), out Voucher_Ma_nt0);
                                    if (!Voucher_Ma_nt0)
                                    {
                                        if (!values[5].Equals(DependencyProperty.UnsetValue))
                                        {
                                            if ((bool)values[5])
                                                return false;
                                        }
                                        if (!values[4].Equals(DependencyProperty.UnsetValue))
                                        {
                                            string ma_gd = values[4].ToString();
                                            if (ma_gd.Equals("2") || ma_gd.Equals("3"))
                                            {
                                                if (!values[0].Equals(DependencyProperty.UnsetValue) && !values[1].Equals(DependencyProperty.UnsetValue))
                                                {
                                                    decimal ty_gia = 0, tien_nt = 0;
                                                    decimal.TryParse(values[0].ToString(),out ty_gia);
                                                    decimal.TryParse(values[1].ToString(), out tien_nt);
                                                    if (ty_gia * tien_nt == 0)
                                                        return false;
                                                }
                                            }
                                            else if (ma_gd.Equals("9"))
                                            {
                                                if (!values[1].Equals(DependencyProperty.UnsetValue))
                                                {
                                                    decimal ty_gia = 0, tien_nt = 0;
                                                    decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);
                                                    decimal.TryParse(values[1].ToString(), out tien_nt);
                                                    if (ty_gia * tien_nt == 0)
                                                        return false;
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    //Binding dia chi
    class BindingReadonlyThue : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (FrmCACTPC1.IsInEditModeThue != null)
                if (FrmCACTPC1.IsInEditModeThue.Value)
                {
                    if (values != null)
                    {
                        int count = StartUp.DsTrans.Tables[0].DefaultView.Count;
                        if (count > 0)
                        {
                            if (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"] != DBNull.Value && !string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"].ToString()) && StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"].ToString().Equals("8"))
                            {
                                string paraStr = parameter.ToString().Trim();
                                if (paraStr.Equals("ma_so_thue"))
                                {
                                    if (values[1].ToString().Equals("True"))
                                        return (values[0].ToString().Trim().Equals("") || values[0] == DependencyProperty.UnsetValue) ? false : true;
                                }
                                else if (paraStr.Equals("t_tien"))
                                {
                                    if (!values[2].Equals(DependencyProperty.UnsetValue))
                                    {
                                        bool checkSuaTien = (bool)values[2];
                                        if (checkSuaTien)
                                            return false;
                                    }
                                    if (!values[0].Equals(DependencyProperty.UnsetValue) && !values[1].Equals(DependencyProperty.UnsetValue))
                                    {
                                        double tien_nt = System.Convert.ToDouble(values[0]);
                                        double ty_gia = System.Convert.ToDouble(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"]);
                                        if (tien_nt * ty_gia == 0)
                                            return false;
                                    }
                                }
                                else if (paraStr.Equals("t_thue"))
                                {
                                    if (!values[2].Equals(DependencyProperty.UnsetValue))
                                    {
                                        bool checkSuaTien = (bool)values[2];
                                        if (checkSuaTien)
                                            return false;
                                    }
                                    if (!values[0].Equals(DependencyProperty.UnsetValue) && !values[1].Equals(DependencyProperty.UnsetValue))
                                    {
                                        double tien = System.Convert.ToDouble(values[0]);
                                        double thue_suat = System.Convert.ToDouble(values[1]);
                                        if (tien * thue_suat == 0)
                                            return false;
                                    }
                                }
                                else if (paraStr.Equals("ma_kh2_t"))
                                {
                                    if (values[0] != null)
                                        return values[0].ToString().Trim().Equals("1") ? false : true;
                                }
                            }
                        }
                    }
                }
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class KindCT : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string result = "Visible";
            int ma_gd = 2;
            int.TryParse(value.ToString(), out ma_gd);
            if (ma_gd == 2 || ma_gd == 3 || ma_gd == 9)
            {
                if (parameter.ToString().Equals("Chi"))
                {
                    result = "Visible";
                }
                else if (parameter.ToString().Equals("CP"))
                {
                    result = "Collapsed";
                }
            }
            else if (ma_gd == 8)
            {
                if (parameter.ToString().Equals("Chi"))
                {
                    result = "Collapsed";
                }
                else if (parameter.ToString().Equals("CP"))
                {
                    result = "Visible";
                }
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
