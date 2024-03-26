using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Data;
using System.Diagnostics;

namespace Glctpk1
{
    public class BindingReadonly: IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (FrmGlctpk1.IsInEditMode != null)
                if (FrmGlctpk1.IsInEditMode.Value)
                    if (values != null)
                    {
                        string[] paraStr = parameter.ToString().Trim().Split(';');
                        switch (paraStr[0])
                        {

                            #region ma_kh_i, ngay_ct0, so_ct0, so_seri0, han_tt trong GrdCt 

                            case "ma_kh_i":
                            case "ngay_ct0":
                            case "so_ct0":
                            case "so_seri0":
                            case "han_tt":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue)
                                    {
                                        Debug.WriteLine(values[0], "tk_cn");
                                        int tk_cn = 0;
                                        int.TryParse(values[0].ToString(), out tk_cn);
                                        //nếu là tk cn thì bắt buộc nhập ma_kh_i
                                        if (tk_cn == 1)
                                            return false;
                                        else
                                            return true;
                                    }
                                }
                                break;

                            #endregion

                            #region ps_no, ps_co GrdCt

                            case "ps_no":
                            case "ps_co":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue
                                        && values[2] != DependencyProperty.UnsetValue
                                        && values[3] != DependencyProperty.UnsetValue
                                        && !string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString()))
                                    {
                                        decimal ps_no_nt = 0, ps_co_nt = 0;
                                        bool sua_tien = false;
                                        bool.TryParse(values[2].ToString(), out sua_tien);

                                        decimal.TryParse(values[0].ToString(), out ps_no_nt);
                                        decimal.TryParse(values[1].ToString(), out ps_co_nt);

                                        decimal ty_gia = 0;
                                        decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);
                                        if ((ps_no_nt == 0 && ps_co_nt == 0) || sua_tien == true || ty_gia == 0)
                                            return false;
                                        return true;
                                    }
                                }
                                break;

                            #endregion

                            #region t_thue Grd HD thue

                            case "t_thue":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue
                                        && values[2] != DependencyProperty.UnsetValue)
                                    {
                                        decimal t_tien = 0, thue_suat = 0;
                                        bool sua_tien = false;
                                        decimal.TryParse(values[0].ToString(), out t_tien);
                                        decimal.TryParse(values[1].ToString(), out thue_suat);
                                        bool.TryParse(values[2].ToString(), out sua_tien);
                                        if (sua_tien == true || (t_tien * thue_suat) == 0)
                                            return false;
                                        return true;
                                    }
                                }
                                break;

                            #endregion

                            #region cục thuế trong grd HD thuế

                            case "ma_kh2":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue)
                                    {
                                        decimal tk_cn = 0;
                                        decimal.TryParse(values[0].ToString(), out tk_cn);
                                        if (tk_cn == 1)
                                            return false;
                                        return true;
                                    }
                                }
                                break;

                            #endregion


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
}
