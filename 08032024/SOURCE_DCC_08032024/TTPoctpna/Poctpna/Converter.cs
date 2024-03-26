using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Data;

namespace TTPoctpna
{
    public class BindingReadonly: IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (FrmPoctpna.IsInEditMode != null)
                if (FrmPoctpna.IsInEditMode.Value)
                {
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
                                        //break;
                                        case "ngay_lct":
                                            if (isInEditMode && StartUp.M_ngay_lct.Equals("1"))
                                            {
                                                return false;
                                            }
                                            return true;
                                        //break;
                                    }
                                }
                                break;
                            case "CT":
                                {
                                    switch (paraStr[1])
                                    {
                                        case "gia_nt0":
                                            {
                                                decimal so_luong = 0;
                                                decimal.TryParse(values[0].ToString(), out so_luong);
                                                if (so_luong != 0)
                                                    return false;
                                                return true;
                                                //break;
                                            }
                                        case "tien_nt0":
                                            {
                                                decimal so_luong = 0, gia_nt0 = 0;
                                                bool sua_tien = false;
                                                bool.TryParse(values[2].ToString(), out sua_tien);

                                                decimal.TryParse(values[0].ToString(), out so_luong);
                                                decimal.TryParse(values[1].ToString(), out gia_nt0);

                                                if (so_luong == 0 || (so_luong * gia_nt0) == 0 || sua_tien == true)
                                                    return false;
                                                return true;
                                                //break;
                                            }
                                        case "gia0":
                                            {
                                                decimal so_luong = 0, gia_nt0 = 0, ty_gia = 0;
                                                bool sua_tien = false;
                                                bool.TryParse(values[2].ToString(), out sua_tien);

                                                decimal.TryParse(values[0].ToString(), out so_luong);
                                                decimal.TryParse(values[1].ToString(), out gia_nt0);
                                                decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

                                                if (so_luong != 0 && ((gia_nt0 * ty_gia) == 0 || sua_tien == true))
                                                    return false;
                                                return true;
                                                //break;
                                            }
                                        case "tien0":
                                            {
                                                decimal tien_nt0 = 0, ty_gia = 0;
                                                bool sua_tien = false;

                                                decimal.TryParse(values[0].ToString(), out tien_nt0);
                                                bool.TryParse(values[1].ToString(), out sua_tien);
                                                decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

                                                if (sua_tien == true || (tien_nt0 * ty_gia) == 0)
                                                    return false;
                                                return true;
                                                //break;
                                            }
                                        case "tk_vt":
                                            {
                                                decimal sua_tk_vt = 0;
                                                decimal.TryParse(values[0].ToString(), out sua_tk_vt);
                                                bool isInEditMode = false;
                                                bool.TryParse(values[1].ToString(), out isInEditMode);

                                                if (sua_tk_vt == 1 && isInEditMode == true)
                                                    return false;
                                                return true;
                                                //break;
                                            }

                                        //Grid Chi Phi
                                        case "cp_nt":
                                            {
                                                return false;
                                                //break;
                                            }
                                        ////cp trong tab chi phí
                                        case "cp":
                                            {
                                                decimal cp_nt = 0;
                                                bool sua_tien = false;
                                                decimal.TryParse(values[0].ToString(), out cp_nt);
                                                bool.TryParse(values[1].ToString(), out sua_tien);
                                                if (sua_tien == true || cp_nt == 0)
                                                    return false;
                                                return true;
                                                //break;
                                            }
                                        //cục thuế trong grd HD thuế
                                        case "ma_kh2":
                                            {
                                                decimal tk_cn = 0;
                                                decimal.TryParse(values[0].ToString(), out tk_cn);
                                                if (tk_cn == 1)
                                                    return false;
                                                return true;
                                                //break;
                                            }
                                    }
                                }
                                break;
                            case "GT":
                                {
                                    switch (paraStr[1])
                                    {
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

    public class IdRecordConverterXamDataGrid : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((int)value + 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

    }

    public class BindingXamDataGridIdField : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToInt32(value) + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public class BindingTextStatus : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value == true)
                return "Xử lý";
            else
                return "Trạng thái";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class BindingHeightCP : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double _heightGrdInfoCP=0;
            double _heightGrdMoneyCP = 238;
            switch ((Visibility)values[1])
            {
                case Visibility.Collapsed:
                    break;
                case Visibility.Visible:
                        if (double.TryParse(values[0].ToString(), out _heightGrdInfoCP))
                            _heightGrdMoneyCP = 238 - _heightGrdInfoCP;
                    break;
            }
            
            return _heightGrdMoneyCP;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
