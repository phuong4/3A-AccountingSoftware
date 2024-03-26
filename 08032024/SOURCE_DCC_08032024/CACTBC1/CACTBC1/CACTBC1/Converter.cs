using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Data;
using Infragistics.Windows.DataPresenter;
using System.Windows;
using System.Diagnostics;

namespace CACTBC1
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
                            if (values != null)
                            {
                                if (values[0] != DBNull.Value && values[0] != DependencyProperty.UnsetValue)
                                {
                                    switch (paraStr[1])
                                    {
                                        case "ma_kh_i":
                                            {
                                                int ma_gd = 2;
                                                int.TryParse(values[0].ToString(), out ma_gd);
                                                result = ma_gd == 3 ? Visibility.Visible : Visibility.Collapsed;
                                            }
                                            break;
                                        case "tien_tt":
                                            {
                                                result = values[0].ToString().IndexOfAny(new char[] { '2', '5' }) >= 0 && values[1].ToString() != StartUp.M_ma_nt0 ? Visibility.Visible : Visibility.Collapsed;
                                            }
                                            break;
                                        case "tien":
                                            {
                                                result = values[0].ToString() != StartUp.M_ma_nt0 ? Visibility.Visible : Visibility.Collapsed;
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                        break;
                    case "KindCT":
                        {
                            int ma_gd = 2;
                            int.TryParse(values[0].ToString(), out ma_gd);
                            if (ma_gd == 2 || ma_gd == 3 || ma_gd == 9)
                            {
                                if (paraStr[1].Equals("CT"))
                                {
                                    result = Visibility.Visible;
                                }
                                else if (paraStr[1].Equals("CTTT"))
                                {
                                    result = Visibility.Collapsed;
                                }
                            }
                            else if (ma_gd == 1)
                            {
                                if (paraStr[1].Equals("CT"))
                                {
                                    result = Visibility.Collapsed;
                                }
                                else if (paraStr[1].Equals("CTTT"))
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

    class BindingReadonly : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (FrmCACTBC1.IsInEditMode != null)
                if (FrmCACTBC1.IsInEditMode.Value)
                    if (values != null)
                    {
                        if (values[0] != DBNull.Value && values[0] != DependencyProperty.UnsetValue)
                        {
                            string[] paraStr = parameter.ToString().Trim().Split(';');
                            switch (paraStr[0])
                            {
                                case "CT":
                                    {
                                        if (paraStr[1].Equals("tien"))
                                        {
                                            if (!values[2].Equals(DependencyProperty.UnsetValue))
                                            {
                                                bool checkSuaTien = (bool)values[2];
                                                if (checkSuaTien)
                                                    return false;
                                            }
                                            if (!values[0].Equals(DependencyProperty.UnsetValue) && !values[3].Equals(DependencyProperty.UnsetValue) && !values[1].Equals(DependencyProperty.UnsetValue))
                                            {
                                                double tien_nt = System.Convert.ToDouble(values[0]);
                                                double ty_gia = System.Convert.ToDouble(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"].ToString().IndexOfAny(new char[] { '2', '5' }) >= 0 ? values[3] : StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"]);
                                                if (tien_nt * ty_gia == 0)
                                                    return false;
                                            }
                                            return true;
                                        }
                                        else if (paraStr[1].Equals("tien_tt"))
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
                                                double ty_gia = System.Convert.ToDouble(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString());
                                                if (tien_nt * ty_gia == 0)
                                                    return false;
                                            }
                                            return true;
                                        }
                                        return false;
                                    }
                                case "PH":
                                    {
                                        bool isInEditMode = false;
                                        bool.TryParse(values[0].ToString(), out isInEditMode);
                                        switch (paraStr[1].ToString())
                                        {
                                            case "ong_ba":
                                                if (isInEditMode && StartUp.M_ong_ba.Equals("1"))
                                                {
                                                    return false;
                                                }
                                                break;
                                            case "ngay_lct":
                                                if (isInEditMode && StartUp.M_ngay_lct.Equals("1"))
                                                {
                                                    return false;
                                                }
                                                break;
                                            case "ma_gd":
                                                {
                                                    int count = StartUp.DsTrans.Tables[1].DefaultView.Count;
                                                    if (count == 0)
                                                    {
                                                        return false;
                                                    }
                                                    else
                                                    {
                                                        return !(StartUp.DsTrans.Tables[1].DefaultView[0]["tk_i"] == DBNull.Value || string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[0]["tk_i"].ToString()));
                                                    }
                                                }
                                        }
                                        return true;
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
}
