using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Data;
using Infragistics.Windows.DataPresenter;
using System.Windows;

namespace INCTPND
{
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
            if (FrmINCTPND.IsInEditMode == null || FrmINCTPND.IsInEditMode.Value == false)
            {
                return true;
            }
            
            if (parameter != null)
            {
                string paraStr = parameter.ToString();
                switch (paraStr)
                {
                    case "gia_nt"://Gia USD
                        {
                            decimal so_luong = 0, gia_ton = 1;
                            bool sua_tien = false, pn_gia_tb = false;

                            decimal.TryParse(values[0].ToString(), out so_luong);
                            bool.TryParse(values[1].ToString(), out sua_tien);
                            bool.TryParse(values[2].ToString(), out pn_gia_tb);
                            decimal.TryParse(values[3].ToString(), out gia_ton);

                            if ((gia_ton == 1 || gia_ton == 4) && pn_gia_tb == true)
                            {

                            }
                            else
                            {
                                if (so_luong != 0 || sua_tien == true)
                                    isreadonly = false;
                            }
                            break;
                        }

                    case "tien_nt"://Tien USD
                        {
                            decimal so_luong = 0, gia_nt = 0, gia_ton = 1;
                            bool sua_tien = false, pn_gia_tb = false;
                            decimal.TryParse(values[0].ToString(), out so_luong);
                            decimal.TryParse(values[1].ToString(), out gia_nt);
                            bool.TryParse(values[2].ToString(), out sua_tien);
                            bool.TryParse(values[3].ToString(), out pn_gia_tb);
                            decimal.TryParse(values[4].ToString(), out gia_ton);
                            if ((gia_ton == 1 || gia_ton == 4) && pn_gia_tb == true)
                            {

                            }
                            else
                            {
                                if ((so_luong * gia_nt == 0) || sua_tien == true)
                                    isreadonly = false;
                            }
                            break;
                        }

                    case "gia"://Gia VND
                        {
                            decimal so_luong = 0, gia_nt = 0, ty_gia = 0, gia_ton = 1;
                            bool sua_tien = false, pn_gia_tb = false;

                            decimal.TryParse(values[0].ToString(), out so_luong);
                            decimal.TryParse(values[1].ToString(), out gia_nt);
                            bool.TryParse(values[2].ToString(), out sua_tien);
                            bool.TryParse(values[3].ToString(), out pn_gia_tb);
                            decimal.TryParse(values[4].ToString(), out gia_ton);
                            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

                            if ((gia_ton == 1 || gia_ton == 4) && pn_gia_tb == true)
                            {

                            }
                            else
                            {
                                if ((so_luong != 0 && gia_nt == 0) || (so_luong != 0 && gia_nt != 0 && sua_tien == true) || (so_luong != 0 && gia_nt * ty_gia == 0))
                                    isreadonly = false;
                            }
                            break;
                        }

                    case "tien"://Tien VND
                        {
                            decimal tien_nt = 0, ty_gia = 0, gia_ton = 1;
                            bool sua_tien = false, pn_gia_tb = false;
                            decimal.TryParse(values[0].ToString(), out tien_nt);
                            bool.TryParse(values[1].ToString(), out sua_tien);
                            bool.TryParse(values[2].ToString(), out pn_gia_tb);
                            decimal.TryParse(values[3].ToString(), out gia_ton);
                            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

                            if ((gia_ton == 1 || gia_ton == 4) && pn_gia_tb == true)
                            {

                            }
                            else
                            {
                                if ((tien_nt * ty_gia == 0) || sua_tien == true)
                                    isreadonly = false;
                            }

                            break;
                        }

                    case "tk_vt":
                        {
                            decimal sua_tk_vt = 0;
                            decimal.TryParse(values[0].ToString(), out sua_tk_vt);
                            bool isInEditMode = false;
                            bool.TryParse(values[1].ToString(), out isInEditMode);

                            if (sua_tk_vt == 1 && isInEditMode == true)
                                isreadonly = false;

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

    #region BindingReadonly

    public class BindingReadonly : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isInEditMode = false;
            bool.TryParse(value.ToString(), out isInEditMode);
            string[] para = parameter.ToString().Split(new char[] { ';' });
            if (para.Length < 2)
                return true;
            switch (para[0])
            {
                case "PH":
                    switch (para[1])
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
                        case "ma_bp":
                            if (isInEditMode && StartUp.M_bp_bh == 1)
                            {
                                return false;
                            }
                            break;
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
}
