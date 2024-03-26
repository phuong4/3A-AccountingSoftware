using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Data;

namespace Inctpng
{

    #region MaNt0Converter

    public class MaNt0Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Console.WriteLine("Vao ne");
            return StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString() == StartUp.SysObj.GetOption("M_MA_NT0").ToString() ?
                "Visible" : "Hidden";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region NotMaNt0Converter

    public class NotMaNt0Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Console.WriteLine("Vao ne");
            return value.ToString().Trim().Equals(StartUp.SysObj.GetOption("M_MA_NT0").ToString().Trim()) ? "Hidden" : "Visible";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region BindingStatusVoucher

    class BindingStatusVoucher : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value.ToString().Trim()))
                return StartUp.tbStatus.Rows.IndexOf(StartUp.tbStatus.Select("Ma_post =" + value)[0] as DataRow);
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            //return value == null ? StartUp.DmctInfo["ma_post"] : value;
            if (value != null)
            {
                int select_index = -1;
                int.TryParse(value.ToString(), out select_index);
                if (select_index != -1)
                    return StartUp.tbStatus.Rows[select_index]["ma_post"];
            }
            return StartUp.DmctInfo["ma_post"];
        }
    }

    #endregion

    #region BindingTextStatus

    class BindingTextStatus : IValueConverter
    {
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
    }

    #endregion

    #region BindingReadOnly

    class BindingReadOnly : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isreadonly = true;
            if (FrmInctpng.IsInEditMode == null || !FrmInctpng.IsInEditMode.Value)
                return isreadonly;
            if (parameter != null)
            {
                string paraStr = parameter.ToString();
                switch (paraStr)
                {
                    case "gia_nt0":
                        {
                            //if (values[0] != DependencyProperty.UnsetValue)
                            //{
                            //bool IsInEditMode = false;
                            //bool.TryParse(values[1].ToString(), out IsInEditMode);
                            //if (IsInEditMode == true)
                            //{
                            decimal so_luong = 0;
                            decimal.TryParse(values[0].ToString(), out so_luong);
                            if (so_luong != 0)
                                isreadonly = false;
                            //}
                            //}
                        }
                        break;
                    case "tien_nt0":
                        {
                            //if (values[0] != DependencyProperty.UnsetValue
                            //&& values[1] != DependencyProperty.UnsetValue
                            //    && values[2] != DependencyProperty.UnsetValue)
                            //{
                            //bool IsInEditMode = false;
                            //bool.TryParse(values[3].ToString(), out IsInEditMode);
                            //if (IsInEditMode == true)
                            //{
                            decimal so_luong = 0, gia_nt0 = 0;
                            bool sua_tien = false;
                            bool.TryParse(values[2].ToString(), out sua_tien);

                            decimal.TryParse(values[0].ToString(), out so_luong);
                            decimal.TryParse(values[1].ToString(), out gia_nt0);

                            if (so_luong == 0 || (so_luong * gia_nt0) == 0 || sua_tien == true)
                                isreadonly = false;
                            //}
                            //}
                        }
                        break;
                    case "gia0":
                        {
                            //if (values[0] != DependencyProperty.UnsetValue
                            //    && values[1] != DependencyProperty.UnsetValue
                            //    && values[2] != DependencyProperty.UnsetValue
                            //    && values[3] != DependencyProperty.UnsetValue)
                            //{
                            //bool IsInEditMode = false;
                            //bool.TryParse(values[3].ToString(), out IsInEditMode);
                            //if (IsInEditMode == true)
                            //{
                            decimal so_luong = 0, gia_nt0 = 0, ty_gia = 0;
                            bool sua_tien = false;
                            bool.TryParse(values[2].ToString(), out sua_tien);

                            decimal.TryParse(values[0].ToString(), out so_luong);
                            decimal.TryParse(values[1].ToString(), out gia_nt0);
                            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

                            if (so_luong != 0 && ((gia_nt0 * ty_gia) == 0 || sua_tien == true))
                                isreadonly = false;
                            //}
                            //}
                        }
                        break;
                    case "tien0":
                        {
                            //if (values[0] != DependencyProperty.UnsetValue
                            //    && values[1] != DependencyProperty.UnsetValue
                            //    && values[2] != DependencyProperty.UnsetValue)
                            //{
                            //bool IsInEditMode = false;
                            //bool.TryParse(values[2].ToString(), out IsInEditMode);
                            //if (IsInEditMode == true)
                            //{
                            decimal tien_nt0 = 0, ty_gia = 0;
                            bool sua_tien = false;

                            decimal.TryParse(values[0].ToString(), out tien_nt0);
                            bool.TryParse(values[1].ToString(), out sua_tien);
                            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

                            if (sua_tien == true || (tien_nt0 * ty_gia) == 0)
                                isreadonly = false;
                            //}
                            //}
                        }
                        break;
                    case "tk_vt":
                        {
                            //if (values[0] != DependencyProperty.UnsetValue
                            //   && values[1] != DependencyProperty.UnsetValue)
                            //{
                            //    bool IsInEditMode = false;
                            //    bool.TryParse(values[1].ToString(), out IsInEditMode);
                            //   if (IsInEditMode == true)
                            //    {
                            decimal sua_tk_vt = 0;
                            decimal.TryParse(values[0].ToString(), out sua_tk_vt);
                            bool isInEditMode = false;
                            bool.TryParse(values[1].ToString(), out isInEditMode);

                            if (sua_tk_vt == 1 && isInEditMode == true)
                                isreadonly = false;
                            //   }
                            //}
                        }
                        break;
                    case "tk_nvl":
                        {
                            bool isInEditMode = false;
                            bool.TryParse(values[0].ToString(), out isInEditMode);

                            if (isInEditMode == true)
                                isreadonly = false;
                        }
                        break;

                    //Grid Chi Phi
                    case "cp_nt":
                        {
                            isreadonly = false;
                            break;
                        }
                    ////cp trong tab chi phí
                    case "cp":
                        {
                            //if (values[0] != DependencyProperty.UnsetValue
                            //    && values[1] != DependencyProperty.UnsetValue
                            //    && values[2] != DependencyProperty.UnsetValue)
                            //{
                            //    bool IsInEditMode = false;
                            //    bool.TryParse(values[2].ToString(), out IsInEditMode);
                            //    if (IsInEditMode == true)
                            //    {
                            decimal cp_nt = 0;
                            bool sua_tien = false;
                            decimal.TryParse(values[0].ToString(), out cp_nt);
                            bool.TryParse(values[1].ToString(), out sua_tien);
                            if (sua_tien == true || cp_nt == 0)
                                isreadonly = false;
                            //}
                            //}
                            break;
                        }

                    //cục thuế trong grd HD thuế
                    case "ma_kh2":
                        {
                            //if (values[0] != DependencyProperty.UnsetValue
                            //    && values[1] != DependencyProperty.UnsetValue)
                            //{
                            //    bool IsInEditMode = false;
                            //    bool.TryParse(values[1].ToString(), out IsInEditMode);
                            //    if (IsInEditMode == true)
                            //    {
                            decimal tk_cn = 0;
                            decimal.TryParse(values[0].ToString(), out tk_cn);
                            if (tk_cn == 1)
                                isreadonly = false;
                            //}
                            //}
                            break;
                        }

                    case "ten_kh":
                        {
                            string ma_kh = string.Empty;

                            ma_kh = values[0].ToString().Trim();
                            if (string.IsNullOrEmpty(ma_kh))
                            {
                                isreadonly = false;
                            }
                            else
                            {
                                isreadonly = true;
                            }
                            break;
                        }
                    case "dia_chi":
                        {
                            string ma_kh = string.Empty;
                            string dia_chi_dmkh = string.Empty;

                            ma_kh = values[2].ToString().Trim();
                            dia_chi_dmkh = values[0].ToString().Trim();

                            if (string.IsNullOrEmpty(dia_chi_dmkh) || string.IsNullOrEmpty(ma_kh))
                            {
                                isreadonly = false;
                            }
                            else
                            {
                                isreadonly = true;
                            }
                            break;
                        }

                    case "ma_so_thue":
                        {
                            string ma_kh = string.Empty;
                            string ma_so_thue_dmkh = string.Empty;

                            ma_kh = values[2].ToString().Trim();
                            ma_so_thue_dmkh = values[0].ToString().Trim();

                            if (string.IsNullOrEmpty(ma_so_thue_dmkh) || string.IsNullOrEmpty(ma_kh))
                            {
                                isreadonly = false;
                            }
                            else
                            {
                                isreadonly = true;
                            }
                            break;
                        }

                }
            }

            return isreadonly;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion 

    #region IsReadOnlyPh

    public class IsReadOnlyPh : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isInEditMode = false;
            bool.TryParse(value.ToString(), out isInEditMode);
            switch (parameter.ToString())
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
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    #endregion

    #region CheckBoxConverter

    public class CheckBoxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int suatien = 0;
            int.TryParse(value.ToString(), out suatien);
            return suatien == 1 ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool ischecked = bool.Parse(value.ToString());
            return ischecked == true ? 1 : 0;
        }
    }

    #endregion
}
