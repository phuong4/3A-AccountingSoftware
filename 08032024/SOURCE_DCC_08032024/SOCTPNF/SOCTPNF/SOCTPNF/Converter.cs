using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Data;
using Infragistics.Windows.DataPresenter;
using System.Windows;

namespace SOCTPNF
{
    #region BindingXamDataGridIdField

    class BindingXamDataGridIdField : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToInt32(value) + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
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

    #region BindingReadonly

    class BindingReadonly : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isreadonly = true;

            if (FrmSOCTPNF.IsInEditMode != null && FrmSOCTPNF.IsInEditMode.Value != true)
                return true;
            string[] para = parameter.ToString().Split(new char[] { ';' });
            if (para.Length < 2)
                return true;

            switch (para[0])
            {
                #region PH

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
                        case "ma_bp":
                            if (values[0] != DependencyProperty.UnsetValue)
                            {
                                bool IsInEditMode = false;
                                bool.TryParse(values[0].ToString(), out IsInEditMode);
                                if (IsInEditMode == true && StartUp.M_bp_bh.ToString().Equals("1"))
                                {
                                    isreadonly = false;
                                }
                            }
                            break;
                        case "ma_tc":
                            if (values[0] != DependencyProperty.UnsetValue && values[1] != DependencyProperty.UnsetValue)
                            {
                                bool IsInEditMode = false;
                                bool.TryParse(values[0].ToString(), out IsInEditMode);
                                bool thue_dau_vao = false;
                                if (values[1].ToString().Equals("1"))
                                    thue_dau_vao = true;
                                if (IsInEditMode == true && thue_dau_vao)
                                {
                                    isreadonly = false;
                                }
                            }
                            break;
                    }
                    break;

                #endregion

                #region CT

                case "CT":

                    if (true)
                    {
                        string paraStr = para[1];
                        switch (paraStr)
                        {
                            case "gia_nt2"://Gia ban USD
                                {
                                    decimal so_luong = 0;
                                    decimal.TryParse(values[0].ToString(), out so_luong);

                                    if (so_luong != 0)
                                        isreadonly = false;

                                    break;
                                }

                            case "tien_nt2"://Tien ban USD
                                {
                                    decimal so_luong = 0, gia_nt0 = 0;
                                    bool sua_tien = false;

                                    decimal.TryParse(values[0].ToString(), out so_luong);
                                    decimal.TryParse(values[1].ToString(), out gia_nt0);
                                    bool.TryParse(values[2].ToString(), out sua_tien);

                                    if (so_luong == 0 || (so_luong * gia_nt0) == 0 || sua_tien == true)
                                        isreadonly = false;

                                    break;
                                }

                            case "ck_nt"://Chiet khau USD
                                {
                                    decimal tl_ck = 0;
                                    decimal.TryParse(values[0].ToString(), out tl_ck);

                                    if (tl_ck != 0)
                                        isreadonly = false;

                                    break;
                                }

                            case "ck"://Chiet khau VND
                                {
                                    decimal ck_nt = 0, ty_gia = 0, tl_ck = 0;
                                    bool sua_tien = false;
                                    decimal.TryParse(values[0].ToString(), out ck_nt);
                                    bool.TryParse(values[1].ToString(), out sua_tien);
                                    decimal.TryParse(values[2].ToString(), out tl_ck);

                                    decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

                                    if (tl_ck != 0 && ((ck_nt * ty_gia) == 0 || sua_tien == true))
                                        isreadonly = false;

                                    break;
                                }

                            case "gia_nt"://Gia von USD
                                {
                                    decimal so_luong = 0, gia_ton = 1;
                                    bool pn_gia_tb = false;

                                    decimal.TryParse(values[0].ToString(), out so_luong);
                                    //bool.TryParse(values[1].ToString(), out sua_tien);
                                    bool.TryParse(values[1].ToString(), out pn_gia_tb);
                                    decimal.TryParse(values[2].ToString(), out gia_ton);
                                    if (pn_gia_tb == false)
                                    {
                                        isreadonly = false;
                                    }
                                    else
                                        if (gia_ton == 1 || gia_ton == 4)
                                        {
                                            isreadonly = true;
                                        }
                                        else
                                            isreadonly = false;
                                    //else
                                    //{
                                    //    if (so_luong != 0)
                                    //        isreadonly = false;
                                    //}
                                    break;
                                }

                            case "tien_nt"://Tien von USD
                                {

                                    decimal so_luong = 0, gia_nt = 0, gia_ton = 1;
                                    bool sua_tien = false, pn_gia_tb = false;
                                    decimal.TryParse(values[0].ToString(), out so_luong);
                                    decimal.TryParse(values[1].ToString(), out gia_nt);
                                    bool.TryParse(values[2].ToString(), out sua_tien);
                                    bool.TryParse(values[3].ToString(), out pn_gia_tb);
                                    decimal.TryParse(values[4].ToString(), out gia_ton);
                                    if ((gia_ton == 1 && pn_gia_tb == false && so_luong != 0) || gia_ton == 0)
                                    {
                                        if (gia_nt == 0)
                                            isreadonly = false;
                                    }
                                    //else
                                    //{
                                    //    if ((so_luong != 0 && gia_nt == 0) || sua_tien == true)
                                    //        isreadonly = false;
                                    //}
                                    break;
                                }

                            case "gia2"://Gia ban VND
                                {
                                    decimal so_luong = 0, gia_nt2 = 0, ty_gia = 0;
                                    bool sua_tien = false;

                                    decimal.TryParse(values[0].ToString(), out so_luong);
                                    decimal.TryParse(values[1].ToString(), out gia_nt2);
                                    bool.TryParse(values[2].ToString(), out sua_tien);
                                    decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);


                                    if ((so_luong != 0 && gia_nt2 == 0) || (so_luong != 0 && gia_nt2 != 0 && sua_tien == true) || (so_luong != 0 && gia_nt2 * ty_gia == 0))
                                        isreadonly = false;

                                    break;
                                }

                            case "tien2"://Thanh tien VND
                                {
                                    decimal tien_nt2 = 0, ty_gia = 0;
                                    bool sua_tien = false;
                                    decimal.TryParse(values[0].ToString(), out tien_nt2);
                                    bool.TryParse(values[1].ToString(), out sua_tien);

                                    decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);


                                    if (sua_tien == true || (tien_nt2 * ty_gia) == 0)
                                        isreadonly = false;

                                    break;
                                }

                            case "gia"://Gia von VND
                                {
                                    decimal so_luong = 0, gia_nt = 0, ty_gia = 0, gia_ton = 1;
                                    bool sua_tien = false, pn_gia_tb = false;

                                    decimal.TryParse(values[0].ToString(), out so_luong);
                                    decimal.TryParse(values[1].ToString(), out gia_nt);
                                    bool.TryParse(values[2].ToString(), out sua_tien);
                                    bool.TryParse(values[3].ToString(), out pn_gia_tb);
                                    decimal.TryParse(values[4].ToString(), out gia_ton);
                                    decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

                                    if (pn_gia_tb == false && (sua_tien == true || gia_nt * ty_gia == 0))
                                    {
                                        isreadonly = false;
                                    }
                                    else
                                        if (gia_ton == 1 || gia_ton == 4)
                                        {
                                            isreadonly = true;
                                        }
                                        else
                                            isreadonly = false;

                                    //if ((gia_ton == 1 && pn_gia_tb == true) || gia_ton == 0)
                                    //{
                                    //    isreadonly = false;
                                    //}
                                    //else
                                    //{
                                    //    if ((so_luong != 0 && gia_nt == 0) || (so_luong != 0 && gia_nt != 0 && sua_tien == true) || (so_luong != 0 && gia_nt * ty_gia == 0))
                                    //        isreadonly = false;
                                    //}
                                    break;
                                }

                            case "tien"://Tien von VND
                                {
                                    decimal tien_nt = 0, gia_ton = 1, ty_gia = 0;
                                    bool sua_tien = false, pn_gia_tb = false;
                                    decimal.TryParse(values[0].ToString(), out tien_nt);
                                    bool.TryParse(values[1].ToString(), out sua_tien);
                                    bool.TryParse(values[2].ToString(), out pn_gia_tb);
                                    decimal.TryParse(values[3].ToString(), out gia_ton);

                                    decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);
                                    if ((gia_ton == 1 && pn_gia_tb == true) || gia_ton == 0)
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
                                    decimal vt_ton_kho = 0;
                                    decimal.TryParse(values[2].ToString(), out vt_ton_kho);
                                    if (vt_ton_kho == 1)
                                    {
                                        decimal sua_tk_vt = 0;
                                        decimal.TryParse(values[0].ToString(), out sua_tk_vt);

                                        if (sua_tk_vt == 1)
                                            isreadonly = false;
                                    }
                                }
                                break;

                            case "tk_km_i":
                                {
                                    decimal km_ck = 0;
                                    decimal.TryParse(values[0].ToString(), out km_ck);

                                    if (km_ck == 1)
                                        isreadonly = false;

                                    break;
                                }

                            case "tk_ck"://Tai khoan chiet khau
                                {
                                    decimal tl_ck = 0;
                                    decimal.TryParse(values[0].ToString(), out tl_ck);
                                    if (tl_ck != 0)
                                        isreadonly = false;

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

                            case "ma_kh2_ph":
                            case "tk_thue_co_IsReadOnly":
                                {
                                    bool IsInEditMode = false;
                                    bool.TryParse(values[1].ToString(), out IsInEditMode);

                                    if ((values[0].ToString().Equals("1") || (values.Length > 2 && values[2].ToString() == "0")) && IsInEditMode == true)
                                        isreadonly = false;

                                    break;
                                }

                            #region tk_thue_no_IsReadOnly
                            case "tk_thue_no_IsReadOnly":
                                {
                                    bool IsInEditMode = false;
                                    bool.TryParse(values[1].ToString(), out IsInEditMode);
                                    if (IsInEditMode == true)
                                    {
                                        string tk_thue_no_dmthue = values[0].ToString().Trim();
                                        if (tk_thue_no_dmthue == "" || StartUp.IsTkMe(tk_thue_no_dmthue))
                                            isreadonly = false;
                                    }
                                }
                                break;
                            #endregion

                        }
                    }
                    break;

                #endregion
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
