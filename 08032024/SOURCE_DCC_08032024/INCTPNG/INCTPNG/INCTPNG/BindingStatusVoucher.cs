using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Data;
using Infragistics.Windows.DataPresenter;
using System.Windows;

namespace INCTPNG
{
    public class BindingStatusVoucher : IValueConverter
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
            return StartUp.tbStatus.Rows.IndexOf(StartUp.tbStatus.Select("Default = 1")[0] as DataRow);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? StartUp.DmctInfo["ma_post"] : value;
        }

        #endregion
    }

    public class BindingReadOnly : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isreadonly = true;
            if (parameter != null)
            {
                string paraStr = parameter.ToString();
                switch (paraStr)
                {
                    #region gia_nt0 GrdCt
                    case "gia_nt0":
                        {
                            //if (values[0] != DependencyProperty.UnsetValue)
                            //{
                                double so_luong = 0;
                                double.TryParse(values[0].ToString(), out so_luong);
                                if (so_luong != 0)
                                    isreadonly = false;
                            //}
                        }
                        break;
                    #endregion

                    #region tien_nt0 GrdCt
                    case "tien_nt0":
                        {
                            //if (values[0] != DependencyProperty.UnsetValue 
                            //    && values[1] != DependencyProperty.UnsetValue
                            //    && values[2] != DependencyProperty.UnsetValue)
                            //{
                                double so_luong = 0, gia_nt = 0;
                                bool sua_tien = false;
                                bool.TryParse(values[2].ToString(), out sua_tien);

                                double.TryParse(values[0].ToString(), out so_luong);
                                double.TryParse(values[1].ToString(), out gia_nt);

                                if (so_luong == 0 || (so_luong * gia_nt) == 0 || sua_tien == true)
                                    isreadonly = false;
                            //}
                        }
                        break;
                    #endregion

                    #region gia0 GrdCt
                    case "gia0":
                        {
                            //if (values[0] != DependencyProperty.UnsetValue
                            //    && values[1] != DependencyProperty.UnsetValue
                            //    && values[2] != DependencyProperty.UnsetValue
                            //    && values[3] != DependencyProperty.UnsetValue)
                            //{
                                double so_luong = 0, gia_nt = 0, ty_gia = 0;
                                bool sua_tien = false;

                                double.TryParse(values[0].ToString(), out so_luong);
                                double.TryParse(values[1].ToString(), out gia_nt);
                                bool.TryParse(values[2].ToString(), out sua_tien);
                                double.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

                                if (so_luong != 0 && ((gia_nt * ty_gia) == 0 || sua_tien == true))
                                    isreadonly = false;
                            //}
                        }
                        break;
                    #endregion

                    #region tien0 GrdCt và t_tien GrdCtgt
                    case "t_tien":
                    case "tien0":
                        {
                            //if (values[0] != DependencyProperty.UnsetValue 
                            //    && values[1]!=DependencyProperty.UnsetValue
                            //    && values[2] != DependencyProperty.UnsetValue)
                            //{
                                double tien_nt = 0, ty_gia = 0;
                                bool sua_tien = false;
                                
                                double.TryParse(values[0].ToString(), out tien_nt);
                                bool.TryParse(values[1].ToString(), out sua_tien);

                                double.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

                                if (sua_tien == true || (tien_nt * ty_gia) == 0)
                                    isreadonly = false;
                            //}
                        }
                        break;
                    #endregion

                    #region tk_vt GrdCt
                    case "tk_vt":
                        {
                            //if (values[0] != DependencyProperty.UnsetValue)
                            //{
                                double sua_tk_vt = 0;
                                double.TryParse(values[0].ToString(), out sua_tk_vt);
                                if (sua_tk_vt == 1)
                                    isreadonly = false;
                            //}
                        }
                        break;
                    #endregion
                    
                    #region t_thue Grd HD thue
                    case "t_thue":
                        {
                            //if (values[0] != DependencyProperty.UnsetValue
                            //    && values[1] != DependencyProperty.UnsetValue
                            //    && values[2] != DependencyProperty.UnsetValue)
                            //{
                                double t_tien = 0, thue_suat = 0;
                                bool sua_tien = false;
                                double.TryParse(values[0].ToString(), out t_tien);
                                double.TryParse(values[1].ToString(), out thue_suat);
                                bool.TryParse(values[2].ToString(), out sua_tien);
                                if (sua_tien == true || (t_tien * thue_suat) == 0)
                                    isreadonly = false;
                            //}
                        }
                        break;
                    #endregion

                    #region cục thuế trong grd HD thuế
                    case "ma_kh2":
                        {
                            //if (values[0] != DependencyProperty.UnsetValue)
                            //{
                                double tk_cn = 0;
                                double.TryParse(values[0].ToString(), out tk_cn);
                                if (tk_cn == 1)
                                    isreadonly = false;
                            //}
                        }
                        break;
                    #endregion

                    #region cp trong tab chi phí
                    case "cp":
                        {
                            //if (values[0] != DependencyProperty.UnsetValue
                            //    && values[1] != DependencyProperty.UnsetValue)
                            //{
                                double cp_nt = 0;
                                bool sua_tien = false;
                                double.TryParse(values[0].ToString(), out cp_nt);
                                bool.TryParse(values[1].ToString(), out sua_tien);
                                if (sua_tien == true || cp_nt == 0)
                                    isreadonly = false;
                            //}
                        }
                        break;
                    #endregion
                }
            }
            
            return isreadonly;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
