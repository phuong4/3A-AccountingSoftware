using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Data;
using Infragistics.Windows.DataPresenter;
using System.Windows;
using System.Data.SqlClient;
using SmErrorLib;

namespace Inctpxd
{
    #region BindingReadonly

    class BindingReadonly : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isreadonly = true;

            string[] para = parameter.ToString().Split(new char[] { ';' });
            if (para.Length < 2)
                return true;

            switch (para[0])
            {
                case "PH":
                    switch (para[1])
                    {
                        case "ong_ba":
                            if (values[0] != DependencyProperty.UnsetValue)
                            {
                                bool IsInEditMode = false;
                                bool.TryParse(values[0].ToString(), out IsInEditMode);
                                if (IsInEditMode == true && StartUp.M_ong_ba.Equals("1"))
                                {
                                    isreadonly = false;
                                }
                            }
                            break;
                        case "ngay_lct":
                            if (values[0] != DependencyProperty.UnsetValue)
                            {
                                bool IsInEditMode = false;
                                bool.TryParse(values[0].ToString(), out IsInEditMode);
                                if (IsInEditMode == true && StartUp.M_ngay_lct.Equals("1"))
                                {
                                    isreadonly = false;
                                }
                            }
                            break;
                    }
                    break;
                case "CT":

                    if (true)
                    {
                        string paraStr = para[1];
                        switch (paraStr)
                        {
                            #region so_luong
                            case "so_luong":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                           && values[1] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[1].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            //if (values[0].ToString() == "1")
                                                isreadonly = false;
                                        }
                                    }
                                }
                                break;
                            #endregion

                            #region gia_nt
                            case "gia_nt":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue
                                        && values[2] != DependencyProperty.UnsetValue
                                        && values[3] != DependencyProperty.UnsetValue
                                        && values[4] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[3].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            decimal so_luong = 0;
                                            decimal.TryParse(values[0].ToString(), out so_luong);
                                            decimal gia_ton = 0;
                                            decimal.TryParse(values[1].ToString(), out gia_ton);
                                            bool Px_gia_dd = false;
                                            bool.TryParse(values[2].ToString(), out Px_gia_dd);
                                            if (so_luong != 0 && ((Px_gia_dd == true && (gia_ton == 1||gia_ton==4)) || gia_ton == 2) && values[4].ToString() == "1")
                                                isreadonly = false;
                                        }
                                    }
                                }
                                break;
                            #endregion

                            #region tien_nt
                            case "tien_nt":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue
                                        && values[2] != DependencyProperty.UnsetValue
                                        && values[3] != DependencyProperty.UnsetValue
                                        && values[4] != DependencyProperty.UnsetValue
                                        && values[5] != DependencyProperty.UnsetValue
                                        && values[6] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[4].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            decimal so_luong = 0, gia_nt = 0;
                                            bool sua_tien = false;
                                            bool.TryParse(values[3].ToString(), out sua_tien);
                                            bool Px_gia_dd = false;
                                            bool.TryParse(values[2].ToString(), out Px_gia_dd);

                                            decimal.TryParse(values[0].ToString(), out so_luong);
                                            decimal.TryParse(values[1].ToString(), out gia_nt);

                                            decimal gia_ton = 0;
                                            decimal.TryParse(values[6].ToString(), out gia_ton);

                                            if ((so_luong == 0 || (so_luong * gia_nt) == 0 || sua_tien == true)
                                                && ((Px_gia_dd == true && (gia_ton == 1 || gia_ton == 4)) || gia_ton == 2)
                                                && values[5].ToString() == "1")
                                                isreadonly = false;
                                        }
                                    }
                                }
                                break;
                            #endregion

                            #region gia
                            case "gia":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue
                                        && values[2] != DependencyProperty.UnsetValue
                                        && values[3] != DependencyProperty.UnsetValue
                                        && values[4] != DependencyProperty.UnsetValue
                                        && values[5] != DependencyProperty.UnsetValue
                                        && values[6] != DependencyProperty.UnsetValue
                                        && values[7] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[4].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            decimal so_luong = 0, gia_nt = 0, ty_gia = 0;
                                            bool px_gia_dd = false;
                                            bool sua_tien = false;

                                            decimal.TryParse(values[0].ToString(), out so_luong);
                                            decimal.TryParse(values[1].ToString(), out gia_nt);
                                            bool.TryParse(values[2].ToString(), out sua_tien);
                                            bool.TryParse(values[3].ToString(), out px_gia_dd);
                                            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

                                            decimal gia_ton = 0;
                                            decimal.TryParse(values[7].ToString(), out gia_ton);

                                            if (so_luong != 0 && values[6].ToString() == "1"
                                                && ((px_gia_dd == true && (gia_ton == 1 || gia_ton == 4)) || gia_ton == 2)
                                                && (sua_tien == true || gia_nt * ty_gia == 0))
                                                isreadonly = false;
                                        }
                                    }
                                }
                                break;
                            #endregion

                            #region tien
                            case "tien":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue
                                        && values[2] != DependencyProperty.UnsetValue
                                        && values[3] != DependencyProperty.UnsetValue
                                        && values[4] != DependencyProperty.UnsetValue
                                        && values[5] != DependencyProperty.UnsetValue
                                        && values[6] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[3].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            decimal tien_nt = 0, ty_gia = 0;
                                            bool px_gia_dd = false;
                                            bool sua_tien = false;

                                            decimal.TryParse(values[0].ToString(), out tien_nt);
                                            bool.TryParse(values[1].ToString(), out sua_tien);
                                            bool.TryParse(values[2].ToString(), out px_gia_dd);
                                            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

                                            decimal gia_ton = 0;
                                            decimal.TryParse(values[6].ToString(), out gia_ton);

                                            if ((sua_tien == true || tien_nt * ty_gia == 0)
                                                && ((px_gia_dd == true && (gia_ton == 1 || gia_ton == 4)) || gia_ton == 2)
                                                && values[5].ToString() == "1")
                                                isreadonly = false;
                                        }
                                    }
                                }
                                break;
                            #endregion

                            #region tk_vt
                            case "tk_vt":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue
                                        && values[2] != DependencyProperty.UnsetValue
                                        && values[3] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[2].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            string tk_vt = values[1].ToString().Trim();
                                            string vt_ton_kho = values[3].ToString().Trim();
                                            if (vt_ton_kho == "1" && (values[0].ToString() == "1" || tk_vt == "" || StartUp.IsTkMe(tk_vt)))
                                                isreadonly = false;
                                        }
                                    }
                                }
                                break;
                            #endregion
                        }
                    }

                    return isreadonly;
            }

            return isreadonly;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}
