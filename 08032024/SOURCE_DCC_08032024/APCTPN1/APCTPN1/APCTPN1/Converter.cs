using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Data;

namespace APCTPN1
{
    public class BindingReadonly: IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (FrmAPCTPN1.IsInEditMode != null)
                if (FrmAPCTPN1.IsInEditMode.Value)
                    if (values != null)
                    {
                        string[] paraStr = parameter.ToString().Trim().Split(';');
                        switch (paraStr[0].ToUpper())
                        {
                            case "PH":
                                {
                                    bool isInEditMode = false;
                                    bool.TryParse(values[0].ToString(), out isInEditMode);

                                    switch (paraStr[1])
                                    {
                                        case "ong_ba":
                                            if (isInEditMode && StartUp.M_ong_ba.Equals("1"))
                                            {
                                                return false;
                                            }
                                            return true;
                                        case "ngay_lct":
                                            if (isInEditMode && StartUp.M_ngay_lct.Equals("1"))
                                            {
                                                return false;
                                            }
                                            return true;
                                    }
                                }
                                break;
                            case "CT":
                                {
                                    switch (paraStr[1])
                                    {
                                        case "tien":
                                            {
                                                bool result = true;
                                                if (!values[0].Equals(DependencyProperty.UnsetValue) && !values[1].Equals(DependencyProperty.UnsetValue))
                                                {
                                                    decimal tien_nt = System.Convert.ToDecimal(values[0]);
                                                    decimal ty_gia = 1;
                                                    if (StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"] != DBNull.Value)
                                                    {
                                                        ty_gia = System.Convert.ToDecimal(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"]);
                                                    }
                                                    if (tien_nt * ty_gia == 0)
                                                        result = false;
                                                }
                                                if (!values[2].Equals(DependencyProperty.UnsetValue))
                                                {
                                                    bool checkSuaTien = (bool)values[2];
                                                    if (checkSuaTien)
                                                        result = false;
                                                }
                                                return result;
                                            }
                                    }
                                }
                                break;
                            case "GT":
                                {
                                    switch (paraStr[1])
                                    {
                                        case "ma_kh2":
                                            {
                                                if (values[0] != null)
                                                    return values[0].ToString().Trim().Equals("1") ? false : true;
                                            }
                                            break;
                                        case "ten_kh":
                                            {
                                                string ma_kh = string.Empty;

                                                ma_kh = values[0].ToString().Trim();
                                                if (string.IsNullOrEmpty(ma_kh))
                                                {
                                                    return false;
                                                }
                                                else
                                                {
                                                    return true;
                                                }
                                            }
                                        case "dia_chi":
                                            {
                                                string ma_kh = string.Empty;
                                                string dia_chi_dmkh = string.Empty;

                                                ma_kh = values[2].ToString().Trim();
                                                dia_chi_dmkh = values[0].ToString().Trim();

                                                if (string.IsNullOrEmpty(dia_chi_dmkh) || string.IsNullOrEmpty(ma_kh))
                                                {
                                                    return false;
                                                }
                                                else
                                                {
                                                    return true;
                                                }
                                            }

                                        case "ma_so_thue":
                                            {
                                                string ma_kh = string.Empty;
                                                string ma_so_thue_dmkh = string.Empty;

                                                ma_kh = values[2].ToString().Trim();
                                                ma_so_thue_dmkh = values[0].ToString().Trim();

                                                if (string.IsNullOrEmpty(ma_so_thue_dmkh) || string.IsNullOrEmpty(ma_kh))
                                                {
                                                    return false;
                                                }
                                                else
                                                {
                                                    return true;
                                                }
                                            }
                                        default:
                                            break;
                                    }
                                }
                                break;
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
}
