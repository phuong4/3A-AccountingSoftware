using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Data;
using Infragistics.Windows.DataPresenter;
using System.Windows;
using System.Diagnostics;

namespace ARCTHD1
{
    class BindingReadonly : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isreadonly = true;

            if (FrmArcthd1.IsInEditMode != null && !FrmArcthd1.IsInEditMode.Value)
                return true;

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
                    if (FrmArcthd1.IsInEditMode !=null && FrmArcthd1.IsInEditMode.Value == false)
                        return true;
                    if (true)
                    {
                        string paraStr = para[1];
                        switch (paraStr)
                        {
                            #region ma_kh_i, ngay_ct0, so_ct0, so_seri0, han_tt trong GrdCt
                            case "ma_kh_i":
                            case "ngay_ct0":
                            case "so_ct0":
                            case "so_seri0":
                            case "han_tt":
                                {
                                    int tk_cn = 0;
                                    int.TryParse(values[0].ToString(), out tk_cn);
                                    //nếu là tk cn thì bắt buộc nhập ma_kh_i
                                    if (tk_cn == 1)
                                        isreadonly = false;
                                }
                                break;
                            #endregion

                            #region gia_nt2
                            case "gia_nt2":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue)
                                    //&& values[2] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[1].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            decimal so_luong = 0;
                                            decimal.TryParse(values[0].ToString(), out so_luong);
                                            if (so_luong != 0)// && values[2].ToString() == "1")
                                                isreadonly = false;
                                        }
                                    }                                    
                                }
                                break;
                            #endregion

                            #region tien_nt2
                            case "tien_nt2":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue
                                        && values[2] != DependencyProperty.UnsetValue
                                        && values[3] != DependencyProperty.UnsetValue)
                                    //&& values[4] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[3].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            decimal so_luong = 0, gia_nt2 = 0;
                                            bool sua_tien = false;
                                            bool.TryParse(values[2].ToString(), out sua_tien);

                                            decimal.TryParse(values[0].ToString(), out so_luong);
                                            decimal.TryParse(values[1].ToString(), out gia_nt2);

                                            if (so_luong == 0 || (so_luong * gia_nt2) == 0 || sua_tien == true)
                                                //|| values[4].ToString() == "0") 
                                                isreadonly = false;
                                        }
                                    }
                                }
                                break;
                            #endregion
                            #region gia2
                            case "gia2":
                                {
                                    if (values[0] != DependencyProperty.UnsetValue
                                        && values[1] != DependencyProperty.UnsetValue
                                        && values[2] != DependencyProperty.UnsetValue
                                        && values[3] != DependencyProperty.UnsetValue)
                                    //&& values[4] != DependencyProperty.UnsetValue)
                                    {
                                        bool IsInEditMode = false;
                                        bool.TryParse(values[3].ToString(), out IsInEditMode);
                                        if (IsInEditMode == true)
                                        {
                                            decimal so_luong = 0, gia_nt2 = 0, ty_gia = 0;
                                            bool sua_tien = false;

                                            decimal.TryParse(values[0].ToString(), out so_luong);
                                            decimal.TryParse(values[1].ToString(), out gia_nt2);
                                            bool.TryParse(values[2].ToString(), out sua_tien);
                                            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

                                            if (so_luong != 0 && ((gia_nt2 * ty_gia) == 0 || sua_tien == true))
                                                //&& values[4].ToString() == "1")
                                                isreadonly = false;
                                        }
                                    }

                                }
                                break;
                            #endregion

                            #region tien2 trong  GrdCt
                            case "tien2":
                                {
                                    decimal tien_nt2 = 0, ty_gia = 0;

                                    bool sua_tien = false;
                                    bool.TryParse(values[1].ToString(), out sua_tien);
                                    decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);
                                    decimal.TryParse(values[0].ToString(), out tien_nt2);
                                    //decimal.TryParse(values[2].ToString(), out ty_gia);

                                    if ((tien_nt2 * ty_gia == 0) || sua_tien == true)
                                        isreadonly = false;

                                }
                                break;
                            #endregion

                            //#region t_thue Grd HD thue
                            //case "t_thue":
                            //    {
                            //        decimal t_tien = 0, thue_suat = 0;
                            //        bool sua_tien = false;
                            //        decimal.TryParse(values[0].ToString(), out t_tien);
                            //        decimal.TryParse(values[1].ToString(), out thue_suat);
                            //        bool.TryParse(values[2].ToString(), out sua_tien);
                            //        if (sua_tien == true || (t_tien * thue_suat) == 0)
                            //            isreadonly = false;

                            //    }
                            //    break;
                            //#endregion

                            #region cục thuế trong grd HD thuế
                            case "ma_kh2":
                                {

                                    decimal tk_cn = 0;
                                    decimal.TryParse(values[0].ToString(), out tk_cn);
                                    if (tk_cn == 1)
                                        isreadonly = false;
                                }
                                break;
                            #endregion

                            case "tk_ck":
                                {
                                    if (values[0] != null && values[0] != DependencyProperty.UnsetValue)
                                        return Double.Parse(values[0].ToString()) != 0 ? false : true;
                                }
                                break;
                            #region ck ,thue trong GrdCt
                            case "ck":
                                {
                                    decimal tien2 = 0;
                                    bool sua_tien = false;
                                    bool.TryParse(values[1].ToString(), out sua_tien);

                                    decimal.TryParse(values[0].ToString(), out tien2);
                                    //decimal.TryParse(values[1].ToString(), out ps_co_nt);

                                    if ((tien2 == 0) || sua_tien == true)
                                        isreadonly = false;

                                }
                                break;
                            #endregion
                            #region ck_nt ,thue_nt trong GrdCt
                            case "ck_nt":
                                {
                                    //decimal tien_nt2 = 0;
                                    decimal tk_ck = 0;
                                    //bool sua_tien = false;
                                    // bool.TryParse(values[1].ToString(), out sua_tien);

                                    // decimal.TryParse(values[0].ToString(), out tien_nt2);
                                    decimal.TryParse(values[0].ToString(), out tk_ck);
                                    //decimal.TryParse(values[1].ToString(), out ps_co_nt);

                                    //if ((tien_nt2 == 0))
                                    //    isreadonly = false;
                                    if (tk_ck != 0)
                                        isreadonly = false;

                                }
                                break;
                            #endregion
                            #region thue_nt trong GrdCt
                            case "thue_nt":
                                {
                                    decimal tien_nt2 = 0;
                                    bool sua_thue = false;
                                    bool.TryParse(values[1].ToString(), out sua_thue);

                                    decimal.TryParse(values[0].ToString(), out tien_nt2);
                                    //decimal.TryParse(values[1].ToString(), out ps_co_nt);

                                    if ((tien_nt2 == 0) || sua_thue == true)
                                        isreadonly = false;

                                }
                                break;
                            #endregion

                            #region thue trong GrdCt
                            case "thue":
                                {
                                    decimal tien2 = 0;
                                    bool sua_thue = false, sua_tien = false;
                                    bool.TryParse(values[1].ToString(), out sua_tien);
                                    bool.TryParse(values[2].ToString(), out sua_thue);
                                    decimal.TryParse(values[0].ToString(), out tien2);
                                    //decimal.TryParse(values[1].ToString(), out ps_co_nt);

                                    if ((tien2 == 0) || (sua_thue == true && sua_thue == true))
                                        isreadonly = false;

                                }
                                break;
                            #endregion

                            #region tk_thue_i trong GrdCt
                            case "tk_thue_i":
                                {
                                    bool sua_ht_thue = false;
                                    bool.TryParse(values[0].ToString(), out sua_ht_thue);
                                    if (sua_ht_thue == true)
                                        isreadonly = false;
                                }
                                break;
                            #endregion
                        }
                    }
                    break;
            }
            return isreadonly;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    class StringEmpty : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.IsNullOrEmpty(value.ToString().Trim());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
