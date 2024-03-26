using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Data;
using Infragistics.Windows.DataPresenter;
using System.Windows;
using System.Windows.Input;

namespace CACTBN1
{
    public class BindingVisibility : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility result = Visibility.Visible;

            if (values != null && values[0] != DependencyProperty.UnsetValue)
            {
                string[] paraStr = parameter.ToString().Trim().Split(';');
                switch (paraStr[0])
                {
                    case "MaNt0":
                    case "NotMaNt0":
                        {
                            return new SmVoucherLib.BindingVisibility().Convert(values, targetType, parameter, culture);
                        }
                    case "FieldCT":
                        {
                            switch (paraStr[1])
                            {
                                case "ma_gd":
                                    {
                                        int ma_gd = 2;
                                        int.TryParse(values[0].ToString(), out ma_gd);
                                        if (ma_gd == 2 || ma_gd == 3)
                                        {
                                            result = Visibility.Visible;
                                        }
                                        else if (ma_gd == 9)
                                        {
                                            result = Visibility.Collapsed;
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    case "KindCT":
                        {
                            int ma_gd = 2;
                            int.TryParse(values[0].ToString(), out ma_gd);
                            if (ma_gd == 2 || ma_gd == 3 || ma_gd == 9)
                            {
                                if (parameter.ToString().Equals("Chi"))
                                {
                                    result = Visibility.Visible;
                                }
                                else if (parameter.ToString().Equals("CP"))
                                {
                                    result = Visibility.Collapsed;
                                }
                            }
                            else if (ma_gd == 8)
                            {
                                if (parameter.ToString().Equals("Chi"))
                                {
                                    result = Visibility.Collapsed;
                                }
                                else if (parameter.ToString().Equals("CP"))
                                {
                                    result = Visibility.Visible;
                                }
                            }
                        }
                        break;
                }
            }
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class BindingReadonly : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (FrmCACTBN1.IsInEditMode != null)
                if (FrmCACTBN1.IsInEditMode.Value)
                    if (values != null)
                    {
                        string[] paraStr = parameter.ToString().Trim().Split(';');
                        switch (paraStr[0])
                        {
                            case "PH":
                                {
                                    switch (paraStr[1].ToString())
                                    {
                                        case "ong_ba":
                                            bool isInEditMode = false;
                                            bool.TryParse(values[0].ToString(), out isInEditMode);

                                            if (isInEditMode && StartUp.M_ong_ba.Equals("1"))
                                            {
                                                return false;
                                            }
                                            break;
                                        case "ngay_lct":

                                            bool.TryParse(values[0].ToString(), out isInEditMode);
                                            if (isInEditMode && StartUp.M_ngay_lct.Equals("1"))
                                            {
                                                return false;
                                            }
                                            break;
                                        case "ma_gd":
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
                                            return false;
                                            break;
                                    }
                                    return true;
                                }
                                break;
                            case "CT":
                                {
                                    if (values[0] != DBNull.Value && values[0] != DependencyProperty.UnsetValue)
                                    {
                                        string hdStr = values[0].ToString().Trim();
                                        switch (hdStr)
                                        {
                                            //trường hợp loại hd là 0
                                            case "0":
                                                {
                                                    string[] FieldsNotAllowEdit = { "so_seri0", "so_ct0", "ma_kh_t", "ma_thue_i", "thue_suat", "thue_nt", "tk_thue_i", "thue", "tt_nt", "tt" };

                                                    if (FieldsNotAllowEdit.Contains(paraStr[1]))
                                                    {
                                                        return true;
                                                    }
                                                }
                                                break;
                                            case "1":
                                                {
                                                    string[] FieldsNotAllowEdit = { "tt_nt", "tt" };
                                                    if (FieldsNotAllowEdit.Contains(paraStr[1]))
                                                    {
                                                        return true;
                                                    }
                                                }
                                                break;
                                            case "2":
                                                {
                                                    string[] FieldsNotAllowEdit = { "tt_nt", "tt" };
                                                    if (FieldsNotAllowEdit.Contains(paraStr[1]))
                                                    {
                                                        return true;
                                                    }
                                                }
                                                break;
                                            case "4":
                                                {
                                                    string[] FieldsNotAllowEdit = { "tt_nt", "tt" };
                                                    if (FieldsNotAllowEdit.Contains(paraStr[1]))
                                                    {
                                                        return true;
                                                    }
                                                }
                                                break;
                                            case "5":
                                                {
                                                    string[] FieldsNotAllowEdit = { "ngay_ct0", "so_seri0", "so_ct0", "ma_kh_t", "ten_kh_t", "dia_chi_t", "mst_t", "ten_vt_t", "ma_thue_i", "thue_suat", "tt_nt", "tt", "ghi_chu_t" };
                                                    if (FieldsNotAllowEdit.Contains(paraStr[1]))
                                                    {
                                                        return true;
                                                    }
                                                }
                                                break;
                                        }
                                        if (paraStr[1].Equals("tien"))
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
                                        else if (paraStr[1].Equals("thue"))
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
                                        else if (paraStr[1].Equals("ma_kh2_t"))
                                        {
                                            if (values[1] != null)
                                                return values[1].ToString().Trim().Equals("1") ? false : true;
                                        }
                                    }
                                    return false;
                                }
                                break;
                            case "CTCHI":
                                {
                                    switch (paraStr[1])
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
                                                                decimal.TryParse(values[0].ToString(), out ty_gia);
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
                                break;
                            case "CTGT":
                                {
                                    int count = StartUp.DsTrans.Tables[0].DefaultView.Count;
                                    if (count > 0)
                                    {
                                        if (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"] != DBNull.Value && !string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"].ToString()) && StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"].ToString().Equals("8"))
                                        {
                                            if (paraStr[1].Equals("ma_so_thue"))
                                            {
                                                if (values[1].ToString().Equals("True"))
                                                    return (values[0].ToString().Trim().Equals("") || values[0] == DependencyProperty.UnsetValue) ? false : true;
                                            }
                                            else if (paraStr[1].Equals("t_tien"))
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
                                            else if (paraStr[1].Equals("t_thue"))
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
                                            else if (paraStr[1].Equals("ma_kh2_t"))
                                            {
                                                if (values[0] != null)
                                                    return values[0].ToString().Trim().Equals("1") ? false : true;
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
}
